namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20211220 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Gara_DanhMucXe", "BienSo", c => c.String(maxLength: 20));
            Sql(@"CREATE FUNCTION [dbo].[ReportDiscount_GetThucThu]
(	
	@IDChiNhanhs varchar(max),
	@DateFrom datetime,
	@DateTo datetime
)
RETURNS TABLE 
AS
RETURN 
(
		select
			qhd.ID,qhd.NgayLapHoaDon, 
			qct.ID_HoaDonLienQuan, 
			SUM(iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu)) as ThucThu			
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID 
		where (qhd.TrangThai is null or qhd.TrangThai = '1')
		and qhd.ID_DonVi in (select * from dbo.splitstring(@IDChiNhanhs))
		and qhd.NgayLapHoaDon >= @DateFrom
    	and qhd.NgayLapHoaDon < @DateTo 
		and qct.HinhThucThanhToan not in (4,5)
    	group by  qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID
)
GO");

			CreateStoredProcedure(name: "[dbo].[UpdateTonKho_multipleDVT]", parametersAction: p => new
			{
				isUpdate = p.Int(),
				ID_DonVi = p.Guid(),
				ID_DonViQuyDoi = p.Guid(),
				ID_LoHang = p.Guid(),
				TonKho = p.Int()
			}, body: @"SET NOCOUNT ON;

	if @isUpdate = 2
	begin
		---- get infor donviquidoi by ID
		declare @ID_HangHoa uniqueidentifier, @TyLeChuyenDoi float, @LaDonViChuan bit
		select @ID_HangHoa = ID_HangHoa, 
			@TyLeChuyenDoi= iif(TyLeChuyenDoi is null or TyLeChuyenDoi =0,1, TyLeChuyenDoi), 
			@LaDonViChuan= LaDonViChuan
		from DonViQuiDoi where id= @ID_DonViQuyDoi

		--- get infor hanghoa
		declare @QuanLyTheoLo bit, @LaHangHoa bit
		select @QuanLyTheoLo = QuanLyTheoLoHang, @LaHangHoa = LaHangHoa from DM_HangHoa where ID = @ID_HangHoa

		---- get all dvt
		select ID, iif(TyLeChuyenDoi is null or TyLeChuyenDoi =0,1, TyLeChuyenDoi) as TyLeChuyenDoi,LaDonViChuan
		into #allDVT
		from DonViQuiDoi where ID_HangHoa = @ID_HangHoa

		if @LaHangHoa='0'
			begin
			--- reset tonkho if change hanghoa --> dichvu
				update tk  set TonKho = 0 
				from DM_HangHoa_TonKho tk
				where exists (select ID from #allDVT qd where tk.ID_DonViQuyDoi = qd.ID)
			end
		else
		begin
			declare @cur_ID_DonViQuiDoi uniqueidentifier, @cur_TyLeChuyenDoi float
			if @LaDonViChuan ='1'
			begin
				declare _cur cursor for
				select ID, TyLeChuyenDoi from #allDVT
				open _cur
				fetch next from _cur into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
				while @@FETCH_STATUS =0
				begin
					--- update tonkho for dvt #
					update DM_HangHoa_TonKho set TonKho = @TonKho / @cur_TyLeChuyenDoi
					where ID_DonVi = @ID_DonVi and ID_DonViQuyDoi = @cur_ID_DonViQuiDoi
					and (@QuanLyTheoLo='0' or @QuanLyTheoLo is null or ID_LoHang= @ID_LoHang)

					fetch next from _cur
					into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
				end
				close _cur
				deallocate _cur
			end
			else
			begin
				declare @IDQuiDoiChuan uniqueidentifier 
				select @IDQuiDoiChuan = ID from #allDVT where LaDonViChuan='1'

				declare @tonkhoAll float = @TonKho * @TyLeChuyenDoi 
				---- update tonkho for dvt chuan
				update DM_HangHoa_TonKho set TonKho = @tonkhoAll
				where ID_DonVi = @ID_DonVi and ID_DonViQuyDoi = @IDQuiDoiChuan
					and (@QuanLyTheoLo='0' or @QuanLyTheoLo is null or ID_LoHang= @ID_LoHang)

				---- update tokho for dvt # (vd: hang co 3 dvt tro len)
				declare _cur cursor for
				select ID, TyLeChuyenDoi from #allDVT where ID != @IDQuiDoiChuan and ID != @ID_DonViQuyDoi
				open _cur
				fetch next from _cur into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
				while @@FETCH_STATUS =0
				begin					
					update DM_HangHoa_TonKho set TonKho = @tonkhoAll / @cur_TyLeChuyenDoi
					where ID_DonVi = @ID_DonVi and ID_DonViQuyDoi = @cur_ID_DonViQuiDoi
					and (@QuanLyTheoLo='0' or @QuanLyTheoLo is null or ID_LoHang= @ID_LoHang)

					fetch next from _cur
					into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
				end
				close _cur
				deallocate _cur
			end		
		end					
	end");

			Sql(@"ALTER FUNCTION [dbo].[DiscountSale_NVBanHang]
(	
	@IDChiNhanhs varchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanVien varchar(40)
)
RETURNS TABLE 
AS
RETURN 
(
		select 	
			1 as LoaiNhanVienApDung,
			tblNVBan.ID_NhanVien,
			tblNVBan.DoanhThu,
			tblNVBan.ThucThu,
			case when tblNVBan.LaPhanTram =1 then
    				case when tblNVBan.TinhChietKhauTheo=2 then DoanhThu * GiaTriChietKhau / 100 else 0 end 
    				else case when tblNVBan.TinhChietKhauTheo=2 then GiaTriChietKhau else 0 end end as HoaHongDoanhThu,   
			case when tblNVBan.LaPhanTram =1 then
    				case when tblNVBan.TinhChietKhauTheo=1 then ThucThu * GiaTriChietKhau / 100 else 0 end 
    			else case when tblNVBan.TinhChietKhauTheo=1 then GiaTriChietKhau else 0 end end as HoaHongThucThu   ,
				tblNVBan.ID as IDChiTietCK
		from
		(

				select  b.* ,  
					ckct.GiaTriChietKhau, 
					ckct.LaPhanTram,
					ckct.ID,
					ROW_NUMBER() over (PARTITION  by b.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
				from
				(
						select 
							a.ID_NhanVien,
    						a.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    						sum(a.TongThanhToan) -sum(GiaTriTra)  as DoanhThu, 
    						sum(TienThu) -sum(TienTraKhach) as ThucThu,						
							a.ID_ChietKhauDoanhThu 
								
						from
						(
								select 
									ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								case when hd.TongThanhToan is null or hd.TongThanhToan= 0 then hd.PhaiThanhToan
									else hd.TongThanhToan - isnull(hd.TongTienThue,0) end as TongThanhToan, 
    								0 as TienThu,
    								0 as GiaTriTra,
    								0 as TienTraKhach,
									ckdtnv.ID_ChietKhauDoanhThu
								from ChietKhauDoanhThu ckdt
								join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
								join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
								join BH_HoaDon hd on nvth.ID_HoaDon = hd.ID and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
								where hd.ChoThanhToan= 0 
								and  exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)
								and hd.LoaiHoaDon in (1,19, 25)
								and ckdt.LoaiNhanVienApDung= 1
								and hd.NgayLapHoaDon >= @FromDate  and hd.NgayLapHoaDon < @ToDate
								and nvth.ID_NhanVien like @IDNhanVien
								and ckdt.TrangThai= 1
								group by ckdt.TinhChietKhauTheo,  hd.TongThanhToan,  hd.PhaiThanhToan, hd.TongTienThue, ckdtnv.ID_ChietKhauDoanhThu, hd.ID, ckdtnv.ID_NhanVien

								union all

							--- trahang
								select  ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
									0 as TienThu,
    								hdt.PhaiThanhToan - isnull(hdt.TongTienThue,0) as GiaTriTra,
    								sum(ISNULL(qhdct.TienThu, 0)) as TienTraKhach,
    								ckdtnv.ID_ChietKhauDoanhThu
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
								join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
    							join BH_HoaDon hdt on nvth.ID_HoaDon = hdt.ID 
								and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							Join BH_HoaDon hd on hd.ID = hdt.ID_HoaDon
    							left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    							left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							 exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name) 
    							and ckdt.LoaiNhanVienApDung=1
    							and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
								and hdt.LoaiHoaDon= 6
    							and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
								and (qhd.TrangThai is null or qhd.TrangThai != 0)
								and nvth.ID_NhanVien like @IDNhanVien
								and ckdt.TrangThai= 1
								group by ckdt.TinhChietKhauTheo,  
								hdt.TongThanhToan,  hdt.PhaiThanhToan, hdt.TongTienThue, ckdtnv.ID_ChietKhauDoanhThu, hdt.ID, ckdtnv.ID_NhanVien
								union all

								--- thucthu

								select ckdtnv.ID_NhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
									sum(isnull(soquy.ThucThu,0)) as ThucThu,
									0 as GiaTriTra,
    								0 as TienTraKhach,
    								ckdtnv.ID_ChietKhauDoanhThu
									
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu			
    							join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
								join BH_HoaDon hd on nvth.ID_HoaDon = hd.ID and ckdt.ID_DonVi = hd.ID_DonVi 
								and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
								left join (
									select ID_HoaDonLienQuan,
										sum(ThucThu) as ThucThu
									from dbo.ReportDiscount_GetThucThu(@IDChiNhanhs, @FromDate, @ToDate)
									group by ID_HoaDonLienQuan
								) soquy on hd.ID = soquy.ID_HoaDonLienQuan
    							where 
    							 exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)
    							and ckdt.LoaiNhanVienApDung=1
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
								and nvth.ID_NhanVien like @IDNhanVien
    							and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
								and ckdt.TrangThai= 1
								group by ckdtnv.ID_NhanVien, ckdt.TinhChietKhauTheo,  ckdtnv.ID_ChietKhauDoanhThu 
						) a
						group by a.ID_ChietKhauDoanhThu, a.ID_NhanVien,  a.TinhChietKhauTheo
					)b
					join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
					and ((b.DoanhThu >= ckct.DoanhThuTu and b.TinhChietKhauTheo = 2) 
								or (b.ThucThu >= ckct.DoanhThuTu and b.TinhChietKhauTheo = 1))
			) tblNVBan where Rn= 1
)");

			Sql(@"ALTER FUNCTION [dbo].[DiscountSale_NVLapHoaDon]
(	
	@IDChiNhanhs varchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanVien varchar(40)
)
RETURNS TABLE 
AS
RETURN 
(

	select 3 as LoaiNhanVienApDung, 
		tblMax.ID_NhanVien,
		tblMax.DoanhThu,
		tblMax.ThucThu,
		case when tblMax.LaPhanTram =1 then
    		case when tblMax.TinhChietKhauTheo=2 then tblMax.DoanhThu * tblMax.GiaTriChietKhau / 100 else 0 end 
    		else case when tblMax.TinhChietKhauTheo=2 then tblMax.GiaTriChietKhau else 0 end end as HoaHongDoanhThu,   
		case when tblMax.LaPhanTram =1 then
    		case when tblMax.TinhChietKhauTheo=1 then tblMax.ThucThu * tblMax.GiaTriChietKhau / 100 else 0 end 
    		else case when tblMax.TinhChietKhauTheo=1 then tblMax.GiaTriChietKhau else 0 end end as HoaHongThucThu ,
		tblMax.ID
	from
	(
			select tblNVLap.ID_NhanVien, tblNVLap.DoanhThu, tblNVLap.ThucThu,tblNVLap.TinhChietKhauTheo, ckct.GiaTriChietKhau,ckct.LaPhanTram, ckct.ID,
			ROW_NUMBER() over (PARTITION  by tblNVLap.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
	
			from
			(
					select a.ID_NhanVien,
						a.ID_ChietKhauDoanhThu,
						a.ApDungTuNgay,
						a.ApDungDenNgay,
						a.TinhChietKhauTheo,
						SUM(a.PhaiThanhToan - a.GiaTriTra) as DoanhThu,
						SUM(a.TienThu - a.TienTraKhach) as ThucThu
					from
					(
						-- doanh thu
							select  ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								iif(hd.TongThanhToan is null or hd.TongThanhToan = 0, hd.PhaiThanhToan, hd.TongThanhToan - hd.TongTienThue) as PhaiThanhToan, 
    								0 as TienThu,
    								0 as GiaTriTra,
    								0 as TienTraKhach,
									ckdt.ID as ID_ChietKhauDoanhThu, 
									ckdt.ApDungTuNgay,
									ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu    													
    							join BH_HoaDon hd on ckdtnv.ID_NhanVien = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi
								and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							where ckdt.LoaiNhanVienApDung=3
								and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)    							    							
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19, 25) -- daonhthu: khong tinh daonhthu thegiatri
								and hd.ID_NhanVien like @IDNhanVien
    							and hd.NgayLapHoaDon >= @FromDate  and hd.NgayLapHoaDon < @ToDate
								and ckdt.TrangThai= 1

								-- thucthu
								union all
								select  ckdtnv.ID_NhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
									ISNULL(soquy.ThucThu,0) as ThucThu,
    								0 as GiaTriTra,
    								0 as TienTraKhach,
    								ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu			
    							join BH_HoaDon hd on ckdtnv.ID_NhanVien = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi 
								and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
								left join (
									select ID_HoaDonLienQuan,
										sum(ThucThu) as ThucThu
									from dbo.ReportDiscount_GetThucThu(@IDChiNhanhs, @FromDate, @ToDate)
									group by ID_HoaDonLienQuan
								) soquy on hd.ID = soquy.ID_HoaDonLienQuan
    							where 
    							exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)    
    							and ckdt.LoaiNhanVienApDung=3
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,25, 3) --- thegiatri: khong lay thucthu (chi ap dung khi NV)
    							and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
								and hd.ID_NhanVien like @IDNhanVien
								and ckdt.TrangThai= 1

								union all
								-- hdtra
								select ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 0 as TienThu,
    								hdt.PhaiThanhToan - hdt.TongTienThue as GiaTriTra,
    								-ISNULL(soquy.ThucThu, 0) as TienTraKhach,
    								ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu					
    							join BH_HoaDon hdt on ckdtnv.ID_NhanVien = hdt.ID_NhanVien and ckdt.ID_DonVi = hdt.ID_DonVi and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							Join BH_HoaDon hd on hd.ID = hdt.ID_HoaDon
								left join (
									select ID_HoaDonLienQuan,
										sum(ThucThu) as ThucThu
									from dbo.ReportDiscount_GetThucThu(@IDChiNhanhs, @FromDate, @ToDate)
									group by ID_HoaDonLienQuan
								) soquy on hdt.ID = soquy.ID_HoaDonLienQuan
    							where 
    							 exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)  
    							and 
								ckdt.LoaiNhanVienApDung=3
								and ckdt.TrangThai= 1
								and hdt.LoaiHoaDon= 6
    							and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
    							and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
								and hdt.ID_NhanVien like @IDNhanVien
								) a 
								group by a.ID_ChietKhauDoanhThu, a.ID_NhanVien, a.ApDungTuNgay,  a.ApDungDenNgay, a.TinhChietKhauTheo
								) tblNVLap
								join ChietKhauDoanhThu_ChiTiet ckct on
								tblNVLap.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
								and ((tblNVLap.DoanhThu >= DoanhThuTu and tblNVLap.TinhChietKhauTheo = 2) 
								or (tblNVLap.ThucThu >= ckct.DoanhThuTu and tblNVLap.TinhChietKhauTheo = 1))
								)tblMax where Rn= 1
)
");

			Sql(@"ALTER FUNCTION [dbo].[GetMaBangLuongMax_byTemp]
(
	@ID_DonVi uniqueidentifier
)
RETURNS varchar(50)
AS
BEGIN
	DECLARE @mabangluong varchar(50)
	DECLARE @LoaiHoaDon int = 24

	DECLARE @Return float = 1
	declare @lenMaMax int = 0
	DECLARE @isDefault bit = (select top 1 SuDungMaChungTu from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi)-- co/khong thiet lap su dung Ma MacDinh
	DECLARE @isSetup int = (select top 1 ID_LoaiChungTu from HT_MaChungTu where ID_LoaiChungTu = @LoaiHoaDon)-- da ton tai trong bang thiet lap chua

	if @isDefault='1' and @isSetup is not null
		begin
			DECLARE @machinhanh varchar(15) = (select MaDonVi from DM_DonVi where ID= @ID_DonVi)
			DECLARE @lenMaCN int = Len(@machinhanh)
			DECLARE @isUseMaChiNhanh varchar(15) = (select SuDungMaDonVi from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon) -- co/khong su dung MaChiNhanh
			DECLARE @kituphancach1 varchar(1) = (select KiTuNganCach1 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @kituphancach2 varchar(1) = (select KiTuNganCach2 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @kituphancach3 varchar(1) = (select KiTuNganCach3 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @dinhdangngay varchar(8) = (select NgayThangNam from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @dodaiSTT INT = (select CAST(DoDaiSTT AS INT) from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @kihieuchungtu varchar(10) = (select MaLoaiChungTu from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @lenMaKiHieu int = Len(@kihieuchungtu);
			DECLARE @namthangngay varchar(10) = convert(varchar(10), getdate(), 112)
			DECLARE @year varchar(4) = Left(@namthangngay,4)
			DECLARE @date varchar(4) = right(@namthangngay,2)
			DECLARE @month varchar(4) = substring(@namthangngay,5,2)
			DECLARE @datecompare varchar(10)='';
			
			if	@isUseMaChiNhanh='0'
				begin 
					set @machinhanh=''
					set @lenMaCN=0
				end

			if @dinhdangngay='ddMMyyyy'
				set @datecompare = CONCAT(@date,@month,@year)
			else	
				if @dinhdangngay='ddMMyy'
					set @datecompare = CONCAT(@date,@month,right(@year,2))
				else 
					if @dinhdangngay='MMyyyy'
						set @datecompare = CONCAT(@month,@year)
					else	
						if @dinhdangngay='MMyy'
							set @datecompare = CONCAT(@month,right(@year,2))
						else
							if @dinhdangngay='yyyyMMdd'
								set @datecompare = CONCAT(@year,@month,@date)
							else 
								if @dinhdangngay='yyMMdd'
									set @datecompare = CONCAT(right(@year,2),@month,@date)
								else	
									if @dinhdangngay='yyyyMM'
										set @datecompare = CONCAT(@year,@month)
									else	
										if @dinhdangngay='yyMM'
											set @datecompare = CONCAT(right(@year,2),@month)
										else 
											if @dinhdangngay='yyyy'
												set @datecompare = @year							

			DECLARE @sMaFull varchar(50) = concat(@machinhanh,@kituphancach1,@kihieuchungtu,@kituphancach2, @datecompare, @kituphancach3)	

			declare @sCompare varchar(30) = @sMaFull
			if @sMaFull= concat(@kihieuchungtu,'_') set @sCompare = concat(@kihieuchungtu,'[_]') -- like %_% không nhận kí tự _ nên phải [_] theo quy tắc của sql

			-- lay ma max hien tai
			declare @maxCodeNow varchar(30) = (
			select top 1 MaBangLuong from NS_BangLuong 
			where MaBangLuong like @sCompare +'%'  
			order by MaBangLuong desc)
			select @Return = CAST(dbo.udf_GetNumeric(RIGHT(@maxCodeNow, LEN(@maxCodeNow) -LEN (@sMaFull))) AS float) -- lay chuoi so ben phai
	
			-- lay chuoi 000
			declare @stt int =0;
			declare @strstt varchar (10) ='0'
			while @stt < @dodaiSTT- 1
				begin
					set @strstt= CONCAT('0',@strstt)
					SET @stt = @stt +1;
				end 
			declare @lenSst int = len(@strstt)
			if	@Return is null 
				set @mabangluong = CONCAT(@sMaFull,left(@strstt,@lenSst-1),1)-- bỏ bớt 1 số 0			
			else 
				begin
					set @Return = @Return + 1
					set @lenMaMax =  len(@Return)
					set @mabangluong = (select 
						case when @lenMaMax = 1 then CONCAT(@sMaFull,left(@strstt,@lenSst-1),@Return)
							when @lenMaMax = 2 then case when @lenSst - 2 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-2), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 3 then case when @lenSst - 3 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-3), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 4 then case when @lenSst - 4 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-4), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 5 then case when @lenSst - 5 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-5), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 6 then case when @lenSst - 6 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-6), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 7 then case when @lenSst - 7 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-7), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 8 then case when @lenSst - 8 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-8), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 9 then case when @lenSst - 9 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-9), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 10 then case when @lenSst - 10 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return) else CONCAT(@sMaFull, @Return) end
						else '' end)
				end 
		end
	else
		begin
			declare @machungtu varchar(10) = (select top 1 MaLoaiChungTu from DM_LoaiChungTu where ID= @LoaiHoaDon)
			declare @lenMaChungTu int= LEN(@machungtu)

			select @Return = MAX(CAST(dbo.udf_GetNumeric(RIGHT(MaBangLuong,LEN(MaBangLuong)- @lenMaChungTu))AS float))
			from NS_BangLuong where SUBSTRING(MaBangLuong, 1, len(@machungtu)) = @machungtu and CHARINDEX('O',MaBangLuong) = 0 -- not HDO, GDVO, THO, DHO
			
			-- do dai STT (toida = 10)
			if	@Return is null 
					set @mabangluong = (select
						case when @lenMaChungTu = 2 then CONCAT(@machungtu, '00000000',1)
							when @lenMaChungTu = 3 then CONCAT(@machungtu, '0000000',1)
							when @lenMaChungTu = 4 then CONCAT(@machungtu, '000000',1)
							when @lenMaChungTu = 5 then CONCAT(@machungtu, '00000',1)
						else CONCAT(@machungtu,'000000',1)
						end )
			else 
				begin
					set @Return = @Return + 1
					set @lenMaMax = len(@Return)
					set @mabangluong = (select 
						case when @lenMaMax = 1 then CONCAT(@machungtu,'000000000',@Return)
							when @lenMaMax = 2 then CONCAT(@machungtu,'00000000',@Return)
							when @lenMaMax = 3 then CONCAT(@machungtu,'0000000',@Return)
							when @lenMaMax = 4 then CONCAT(@machungtu,'000000',@Return)
							when @lenMaMax = 5 then CONCAT(@machungtu,'00000',@Return)
							when @lenMaMax = 6 then CONCAT(@machungtu,'0000',@Return)
							when @lenMaMax = 7 then CONCAT(@machungtu,'000',@Return)
							when @lenMaMax = 8 then CONCAT(@machungtu,'00',@Return)
							when @lenMaMax = 9 then CONCAT(@machungtu,'0',@Return)								
						else CONCAT(@machungtu,CAST(@Return  as decimal(22,0))) end)
				end 
		end

	RETURN @mabangluong
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_TheoKhachHang]
    @SearchString NVARCHAR(MAX),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang uniqueidentifier,
    @ID_NhomKhachHang [nvarchar](max),
	@LoaiChungTu [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN	

	declare @tblChiNhanh table(ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh 
	select * from dbo.splitstring(@ID_ChiNhanh)
	
	DECLARE @LoadNhomMacDinh BIT = 0;
	IF(CHARINDEX('00000000-0000-0000-0000-000000000000', @ID_NhomKhachHang) > 0)
	BEGIN
		SET @LoadNhomMacDinh = 1;
	END
	
	DECLARE @tblIDNhoms TABLE (ID NVARCHAR(MAX))
	INSERT INTO @tblIDNhoms
	SELECT Name FROM splitstring(@ID_NhomKhachHang)

	DECLARE @tblIDNhomHang TABLE (ID NVARCHAR(MAX))
	INSERT INTO @tblIDNhomHang
	SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)

	declare @tblCTHD table (
		NgayLapHoaDon datetime,
		MaHoaDon nvarchar(max),
		LoaiHoaDon int,
		ID_DonVi uniqueidentifier,
		ID_PhieuTiepNhan uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID_NhanVien uniqueidentifier,
		TongTienHang float,
		TongGiamGia	float,
		KhuyeMai_GiamGia float,
		ChoThanhToan bit,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		ID_ChiTietGoiDV	uniqueidentifier,
		ID_ChiTietDinhLuong uniqueidentifier,
		ID_ParentCombo uniqueidentifier,
		SoLuong float,
		DonGia float,
		GiaVonfloat float,
		TienChietKhau float,
		TienChiPhi float,
		ThanhTien float,
		ThanhToan float,
		GhiChu nvarchar(max),
		ChatLieu nvarchar(max),
		LoaiThoiGianBH int,
		ThoiGianBaoHanh float,
		TenHangHoaThayThe nvarchar(max),
		TienThue float,	
		GiamGiaHD float,
		GiaVon float,
		TienVon float
		)

	insert into @tblCTHD
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu

	declare @tblChiPhi table (ID_ParentCombo uniqueidentifier,ID_DonViQuiDoi uniqueidentifier, ChiPhi float, 
		ID_NhanVien uniqueidentifier,ID_DoiTuong uniqueidentifier)
	insert into @tblChiPhi
	exec BCBanHang_GetChiPhi @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu
	
	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    	Case when nd.LaAdmin = '1' then '1' else
    	Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    	From
    	HT_NguoiDung nd	
    	where nd.ID = @ID_NguoiDung);

		DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);

	
	select 	distinct
	tblV.ID_DoiTuong as ID_KhachHang, 
	tblV.TenNhomDoiTuongs as NhomKhachHang,
	tblV.MaDoiTuong as  MaKhachHang,
	tblV.TenDoiTuong as TenKhachHang,
	tblV.DienThoai, 
	round(tblV.SoLuongMua,3) as SoLuongMua,
	round(tblV.SoLuongTra,3) as SoLuongTra,
	round(tblV.GiaTriMua,3) as GiaTriMua,
	round(tblV.GiaTriTra,3) as GiaTriTra,
	round(tblV.SoLuong,3) as SoLuong,
	round(tblV.ThanhTien,3) as ThanhTien,
	round(iif(@XemGiaVon='1',tblV.TienVon,0),3) as TienVon,
	round(tblV.ThanhTien - tblV.GiamGiaHD,3) as DoanhThu,
	round(tblV.GiamGiaHD,3) as GiamGiaHD, 
	tblV.TongTienThue,
	round(iif(@XemGiaVon='1',tblV.ThanhTien - tblV.TienVon - tblV.GiamGiaHD - tblV.ChiPhi,0),3) as LaiLo,
	tblV.NguoiGioiThieu, tblV.NguoiQuanLy,
	ISNULL(tblV.TenNguonKhach,'') as TenNguonKhach,
	 tblV.ChiPhi
from(
select 
	tbldt.*,
	isnull(nvql.TenNhanVien,'') AS NguoiQuanLy,
	ISNULL(nvql.MaNhanVien,'') as MaNVQuanLy,
	isnull(dtgt.TenDoiTuong,'') AS NguoiGioiThieu,
	isnull(dtgt.MaDoiTuong,'') AS MaNguoiGioiThieu,
	isnull(dtgt.TenDoiTuong_KhongDau,'') AS NguoiGioiThieu_KhongDau,
	isnull(nk.TenNguonKhach,'') AS TenNguonKhach,
	isnull(cp.ChiPhi,0) as ChiPhi	
from 
(
Select 
	tbl.ID_DoiTuong,
	dt.MaDoiTuong,
	dt.TenDoiTuong,
	dt.TenDoiTuong_KhongDau,
	dt.TenDoiTuong_ChuCaiDau,
	dt.DienThoai,
	dt.TenNhomDoiTuongs,
	dt.ID_NguoiGioiThieu,
	dt.ID_NhanVienPhuTrach,
	dt.ID_NguonKhach,
	iif(dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs ='','00000000-0000-0000-0000-000000000000',dt.IDNhomDoiTuongs)  as IDNhomDoiTuongs,
	isnull(tbl.SoLuongMua,0) as SoLuongMua,
	isnull(tbl.SoLuongTra,0) as SoLuongTra,
	isnull(tbl.SoLuongMua,0) - isnull(tbl.SoLuongTra,0) as SoLuong,
	isnull(tbl.GiaTriTra,0) - isnull(GiamGiaHangTra,0) as GiaTriTra,
	isnull(tbl.GiaTriMua,0) - isnull(GiamGiaHangMua,0) as GiaTriMua,
	isnull(tbl.GiaTriMua,0) - isnull(tbl.GiaTriTra,0) as ThanhTien,
	isnull(tbl.GiamGiaHangMua,0) - isnull(tbl.GiamGiaHangTra,0) as GiamGiaHD,
	isnull(tbl.TongThueHangMua,0) - isnull(tbl.TongThueHangTra ,0) as TongTienThue,
	isnull(tbl.GiaVonHangMua,0) - isnull(tbl.GiaVonHangTra,0) as TienVon
	

from (

	select 
		tblMuaTra.ID_DoiTuong, 
		sum(SoLuongMua  * isnull(qd.TyLeChuyenDoi,1)) as SoLuongMua,
		sum(GiaTriMua) as GiaTriMua,
		sum(TongThueHangMua) as TongThueHangMua,
		sum(GiamGiaHangMua) as GiamGiaHangMua,
		sum(GiaVonHangMua) as GiaVonHangMua,
		sum(SoLuongTra  * isnull(qd.TyLeChuyenDoi,1)) as SoLuongTra,
		sum(GiaTriTra) as GiaTriTra,
		sum(TongThueHangTra) as TongThueHangTra,
		sum(GiamGiaHangTra) as GiamGiaHangTra,
		sum(GiaVonHangTra) as GiaVonHangTra
	from
		(
		select 
				tbl.ID_DoiTuong,tbl.ID_DonViQuiDoi, tbl.ID_LoHang,
				sum(tbl.SoLuong) as SoLuongMua,
				sum(tbl.ThanhTien) as GiaTriMua,
				sum(tbl.TienThue) as TongThueHangMua,
				sum(tbl.GiamGiaHD) as GiamGiaHangMua,
				sum(tbl.TienVon) as GiaVonHangMua,
				0 as SoLuongTra,
				0 as GiaTriTra,
				0 as TongThueHangTra,
				0 as GiamGiaHangTra,
				0 as GiaVonHangTra
		from
		(
		---- giatrimua + giavon hangmua
			select 
				ct.ID_DoiTuong,ct.ID_DonViQuiDoi, ct.ID_LoHang,
				ct.SoLuong,
				ct.ThanhTien,
				ct.TienThue,
				ct.GiamGiaHD,
				ct.TienVon
			from @tblCTHD CT			
			where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)		
			and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)			
			) tbl
			group by tbl.ID_DoiTuong,tbl.ID_DonViQuiDoi, tbl.ID_LoHang

			---- giatritra + giavon hangtra
		union all
		select 
			hd.ID_DoiTuong,ct.ID_DonViQuiDoi, ct.ID_LoHang,
			0 as SoLuongMua,
			0 as GiaTriMua,
			0 as TongThueHangMua,
			0 as GiamGiaHangMua,
			0 as GiaVonHangMua,
			sum(SoLuong) as SoLuongTra,
			sum(ThanhTien) as GiaTriTra,
			sum(ct.TienThue * ct.SoLuong) as TienThueHangTra,
			sum(iif(hd.TongTienHang=0,0, ct.ThanhTien  * hd.TongGiamGia /hd.TongTienHang)) as GiamGiaHangTra,
			sum(ct.SoLuong * ct.GiaVon) as GiaVonHangTra
		from BH_HoaDon hd
		join BH_HoaDon_ChiTiet ct on hd.id= ct.ID_HoaDon 
		where hd.ChoThanhToan= 0
		and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
		and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)
		and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and exists (select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
		and hd.LoaiHoaDon =6
		and (ct.ChatLieu is null or ct.ChatLieu !='4') ---- khong lay ct sudung dichvu
		group by hd.ID_DoiTuong,ct.ID_DonViQuiDoi, ct.ID_LoHang
	) tblMuaTra 
	join DonViQuiDoi qd on tblMuaTra.ID_DonViQuiDoi= qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	where 
	exists (SELECT ID FROM @tblIDNhomHang nhomhh where hh.ID_NhomHang= nhomhh.ID)
	and hh.TheoDoi like @TheoDoi
	and qd.Xoa like @TrangThai
	and iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) in (select name from dbo.splitstring(@LoaiHangHoa))
	group by tblMuaTra.ID_DoiTuong
) tbl 
join DM_DoiTuong dt on tbl.ID_DoiTuong= dt.ID
) tbldt
LEFT JOIN NS_NhanVien nvql ON nvql.ID = tbldt.ID_NhanVienPhuTrach
LEFT JOIN DM_DoiTuong dtgt ON dtgt.ID = tbldt.ID_NguoiGioiThieu
left join DM_NguonKhachHang nk on tbldt.ID_NguonKhach = nk.ID
left join (
		select ID_DoiTuong, sum(ChiPhi) as ChiPhi from @tblChiPhi group by ID_DoiTuong
		) cp on tbldt.ID_DoiTuong = cp.ID_DoiTuong
where 
 exists (select nhom1.Name 
			from dbo.splitstring(tbldt.IDNhomDoiTuongs) nhom1 
		 join @tblIDNhoms nhom2 on nhom1.Name = nhom2.ID) 
) tblV

where (select count(Name) from @tblSearchString b where 
				tblV.MaDoiTuong like '%'+b.Name+'%' 
    			OR tblV.TenDoiTuong like '%'+b.Name+'%' 
    			or tblV.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%' 
    			or tblV.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    			or tblV.TenNhomDoiTuongs like '%' +b.Name +'%' 
				or tblV.DienThoai like '%' +b.Name +'%'
    			or tblV.TenDoiTuong like '%'+b.Name+'%'
				or tblV.MaDoiTuong like '%'+b.Name+'%'
    				or tblV.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or tblV.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or tblV.MaNVQuanLy like '%'+b.Name+'%'
					or tblV.NguoiQuanLy like '%'+b.Name+'%'
					or tblV.MaNguoiGioiThieu like '%'+b.Name+'%'
					or tblV.NguoiGioiThieu like '%'+b.Name+'%'
					or tblV.NguoiGioiThieu_KhongDau like '%'+b.Name+'%'
    				)=@count or @count=0		


    END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDatHang_ChiTiet]
    @Text_Search [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN

	set nocount on;

	declare @tblLoai table(LoaiHang int)
	insert into @tblLoai select name from dbo.splitstring(@LoaiHangHoa)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);

	declare @tblCTHD table (
		NgayLapHoaDon datetime,
		MaHoaDon nvarchar(max),
		LoaiHoaDon int,
		ID_DonVi uniqueidentifier,
		ID_PhieuTiepNhan uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID_NhanVien uniqueidentifier,
		TongTienHang float,
		TongGiamGia	float,
		KhuyeMai_GiamGia float,
		ChoThanhToan bit,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		ID_ChiTietGoiDV	uniqueidentifier,
		ID_ChiTietDinhLuong uniqueidentifier,
		ID_ParentCombo uniqueidentifier,
		SoLuong float,
		DonGia float,
		GiaVonfloat float,
		TienChietKhau float,
		TienChiPhi float,
		ThanhTien float,
		ThanhToan float,
		GhiChu nvarchar(max),
		ChatLieu nvarchar(max),
		LoaiThoiGianBH int,
		ThoiGianBaoHanh float,
		TenHangHoaThayThe nvarchar(max),
		TienThue float,	
		GiamGiaHD float,
		GiaVon float,
		TienVon float
		)

	insert into @tblCTHD
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, '3'

	---- get cthd da xuly
	select ctxl.ID_ChiTietGoiDV,
		sum(ctxl.SoLuong) as SoLuongNhan,
		sum(ctxl.ThanhTien) as GiaTriNhan
	into #tblXuLy
	from BH_HoaDon_ChiTiet ctxl
	join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
	where hdxl.LoaiHoaDon in (1,25)
	and hdxl.ChoThanhToan= 0
	and (ctxl.ID_ChiTietDinhLuong = ctxl.ID or ctxl.ID_ChiTietDinhLuong is null)			 
	and (ctxl.ID_ParentCombo is null or ctxl.ID_ParentCombo= ctxl.ID)	
	and exists (
	select ct.ID from @tblCTHD ct where ctxl.ID_ChiTietGoiDV= ct.ID
	)
	group by ctxl.ID_ChiTietGoiDV


	select *
	from
	(
		select ct.MaHoaDon, ct.NgayLapHoaDon, 
			ct.SoLuong as SoLuongDat, 
			ct.ThanhTien as TongTienHang,
			ct.GiamGiaHD,
			ct.ThanhTien - ct.GiamGiaHD as GiaTriDat,
			isnull(xl.SoLuongNhan,0) as SoLuongNhan,
			dt.MaDoiTuong,  
			dt.TenDoiTuong as TenKhachHang,
			qd.MaHangHoa,
			qd.TenDonViTinh,
			hh.TenHangHoa,
			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			CONCAT(hh.TenHangHoa, qd.ThuocTinhGiaTri) as TenHangHoaFull,
			lo.MaLoHang as TenLoHang,
			nv.TenNhanVien,
			ct.GhiChu,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa
		from @tblCTHD ct
		left join #tblXuLy xl on ct.ID= xl.ID_ChiTietGoiDV 
		left join DM_DoiTuong dt on ct.ID_DoiTuong= dt.ID
		left join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
		left join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
		left join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		left join DM_LoHang lo on ct.ID_LoHang= lo.ID
		left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID	
		where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)			 
				and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)		
		and hh.TheoDoi like @TheoDoi
		and qd.Xoa like @TrangThai
		and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhh.ID= allnhh.ID))	
		AND
			((select count(Name) from @tblSearchString b where 
    				hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    				or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'					
    				or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
					or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
					or ct.MaHoaDon like '%'+b.Name+'%'
					)=@count or @count=0)
	)tbl where tbl.LoaiHangHoa in (select LoaiHang from @tblLoai)
	order by tbl.NgayLapHoaDon desc    
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDatHang_NhomHang]
    @TenNhomHang [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	set nocount on;

	declare @tblLoai table(LoaiHang int)
	insert into @tblLoai select name from dbo.splitstring(@LoaiHangHoa)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TenNhomHang, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);

	declare @tblCTHD table (
		NgayLapHoaDon datetime,
		MaHoaDon nvarchar(max),
		LoaiHoaDon int,
		ID_DonVi uniqueidentifier,
		ID_PhieuTiepNhan uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID_NhanVien uniqueidentifier,
		TongTienHang float,
		TongGiamGia	float,
		KhuyeMai_GiamGia float,
		ChoThanhToan bit,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		ID_ChiTietGoiDV	uniqueidentifier,
		ID_ChiTietDinhLuong uniqueidentifier,
		ID_ParentCombo uniqueidentifier,
		SoLuong float,
		DonGia float,
		GiaVonfloat float,
		TienChietKhau float,
		TienChiPhi float,
		ThanhTien float,
		ThanhToan float,
		GhiChu nvarchar(max),
		ChatLieu nvarchar(max),
		LoaiThoiGianBH int,
		ThoiGianBaoHanh float,
		TenHangHoaThayThe nvarchar(max),
		TienThue float,	
		GiamGiaHD float,
		GiaVon float,
		TienVon float
		)

	insert into @tblCTHD
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, '3'

	---- get cthd da xuly
	select ctxl.ID_ChiTietGoiDV,
		sum(ctxl.SoLuong) as SoLuongNhan,
		sum(ctxl.ThanhTien) as GiaTriNhan
	into #tblXuLy
	from BH_HoaDon_ChiTiet ctxl
	join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
	where hdxl.LoaiHoaDon in (1,25)
	and hdxl.ChoThanhToan= 0
	and (ctxl.ID_ChiTietDinhLuong = ctxl.ID or ctxl.ID_ChiTietDinhLuong is null)			 
	and (ctxl.ID_ParentCombo is null or ctxl.ID_ParentCombo= ctxl.ID)	
	and exists (
	select ct.ID from @tblCTHD ct where ctxl.ID_ChiTietGoiDV= ct.ID
	)
	group by ctxl.ID_ChiTietGoiDV

	select 
		nhh.ID,
		sum(tblDH.SoLuongDat) as SoLuongDat,
		sum(tblDH.ThanhTien) as ThanhTien,
		sum(tblDH.GiamGiaHD) as GiamGiaHD,
		sum(tblDH.ThanhTien - tblDH.GiamGiaHD) as GiaTriDat,	
		sum(isnull(tblDH.SoLuongNhan,0)) as SoLuongNhan,
		isnull(nhh.TenNhomHangHoa,N'Nhóm mặc định') as TenNhomHangHoa
	from(
	select ct.ID_DonViQuiDoi, ct.ID_LoHang, 
		sum(ct.SoLuong) as SoLuongDat, 
		sum(ct.ThanhTien) as ThanhTien,
		sum(ct.GiamGiaHD) as GiamGiaHD,
		sum(isnull(xl.SoLuongNhan,0)) as SoLuongNhan
	from @tblCTHD ct
	left join #tblXuLy xl on ct.ID= xl.ID_ChiTietGoiDV 
	where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)			 
			and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)	
	group by ct.ID_DonViQuiDoi, ct.ID_LoHang
	)tblDH
	join DonViQuiDoi qd on tblDH.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	left join DM_LoHang lo on tblDH.ID_LoHang= lo.ID	
	left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
	where hh.TheoDoi like @TheoDoi
		and qd.Xoa like @TrangThai
		and iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) in (select LoaiHang from @tblLoai)
		and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhh.ID= allnhh.ID))		  		
    	AND
		((select count(Name) from @tblSearchString b where 
    			hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'					
    				or qd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
	group by nhh.ID, nhh.TenNhomHangHoa   
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDatHang_TongHop]
    @Text_Search [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	set nocount on;
	declare @tblLoai table(LoaiHang int)
	insert into @tblLoai select name from dbo.splitstring(@LoaiHangHoa)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);

	declare @tblCTHD table (
		NgayLapHoaDon datetime,
		MaHoaDon nvarchar(max),
		LoaiHoaDon int,
		ID_DonVi uniqueidentifier,
		ID_PhieuTiepNhan uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID_NhanVien uniqueidentifier,
		TongTienHang float,
		TongGiamGia	float,
		KhuyeMai_GiamGia float,
		ChoThanhToan bit,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		ID_ChiTietGoiDV	uniqueidentifier,
		ID_ChiTietDinhLuong uniqueidentifier,
		ID_ParentCombo uniqueidentifier,
		SoLuong float,
		DonGia float,
		GiaVonfloat float,
		TienChietKhau float,
		TienChiPhi float,
		ThanhTien float,
		ThanhToan float,
		GhiChu nvarchar(max),
		ChatLieu nvarchar(max),
		LoaiThoiGianBH int,
		ThoiGianBaoHanh float,
		TenHangHoaThayThe nvarchar(max),
		TienThue float,	
		GiamGiaHD float,
		GiaVon float,
		TienVon float
		)

	insert into @tblCTHD
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, '3'

	---- get cthd da xuly
	select ctxl.ID_ChiTietGoiDV,
		sum(ctxl.SoLuong) as SoLuongNhan,sum(ctxl.ThanhTien) as GiaTriNhan
	into #tblXuLy
	from BH_HoaDon_ChiTiet ctxl
	join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
	where hdxl.LoaiHoaDon in (1,25)
	and hdxl.ChoThanhToan= 0
	and (ctxl.ID_ChiTietDinhLuong = ctxl.ID or ctxl.ID_ChiTietDinhLuong is null)			 
	and (ctxl.ID_ParentCombo is null or ctxl.ID_ParentCombo= ctxl.ID)	
	and exists (
	select ct.ID from @tblCTHD ct where ctxl.ID_ChiTietGoiDV= ct.ID
	)
	group by ctxl.ID_ChiTietGoiDV

	select *
	from
	(
	select 
		tblDH.ID_DonViQuiDoi,
		tblDH.ID_LoHang,
		qd.MaHangHoa,
		lo.MaLoHang,
		hh.TenHangHoa,
		concat(hh.TenHangHoa,ThuocTinhGiaTri) as TenHangHoaFull,
		qd.TenDonViTinh,
		qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
		tblDH.SoLuongDat,
		tblDH.ThanhTien,
		tblDH.GiamGiaHD,
		tblDH.ThanhTien - tblDH.GiamGiaHD as GiaTriDat,
		tblDH.SoLuongNhan,
		isnull(nhh.TenNhomHangHoa,N'Nhóm mặc định') as TenNhomHangHoa,
		iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa
	from(
	select ct.ID_DonViQuiDoi, ct.ID_LoHang, 
		sum(ct.SoLuong) as SoLuongDat, 
		sum(ct.ThanhTien) as ThanhTien,
		sum(ct.GiamGiaHD) as GiamGiaHD,
		sum(isnull(xl.SoLuongNhan,0)) as SoLuongNhan
	from @tblCTHD ct
	left join #tblXuLy xl on ct.ID= xl.ID_ChiTietGoiDV 
	where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)			 
			and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)	
	group by ct.ID_DonViQuiDoi, ct.ID_LoHang
	)tblDH
	join DonViQuiDoi qd on tblDH.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	left join DM_LoHang lo on tblDH.ID_LoHang= lo.ID	
	left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
	where hh.TheoDoi like @TheoDoi	  
	and qd.Xoa like @TrangThai
	and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhh.ID= allnhh.ID)		  		)
    	AND
		((select count(Name) from @tblSearchString b where 
    			hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'					
    
	or qd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
	) tbl
	where tbl.LoaiHangHoa in (select LoaiHang from @tblLoai)
	order by tbl.TenHangHoa desc 
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDichVu_SoDuChiTiet]
    @Text_Search [nvarchar](max),
    @MaHH [nvarchar](max),
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
	@ThoiHan [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NhomHang_SP [nvarchar](max),
	@ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	set nocount on;
	declare @tblChiNhanh table( ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@ID_ChiNhanh)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';

    Select @count =  (Select count(*) from @tblSearchString);
	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)

	declare @dtNow datetime = getdate()

	---- get gdvmua
	declare @tblCTMua table(
		MaHoaDon nvarchar(max),
		NgayLapHoaDon datetime,
		NgayApDungGoiDV datetime,
		HanSuDungGoiDV datetime,
		ID_DonVi uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		SoLuong float,
		DonGia float,
		TienChietKhau float,
		ThanhTien float,
		GiamGiaHD float)
	insert into @tblCTMua
	exec BaoCaoGoiDV_GetCTMua @ID_ChiNhanh,@timeStart,@timeEnd
	
	
	---- get by idnhom, thoihan --> check where
				select *,
				concat(b.TenHangHoa, b.ThuocTinh_GiaTri) as TenHangHoaFull
				from
				(
			
					select 
						ctm.ID_HoaDon,
						ctm.MaHoaDon,
						ctm.NgayLapHoaDon,
						ctm.NgayApDungGoiDV,
						ctm.HanSuDungGoiDV,
						dt.MaDoiTuong as MaKhachHang,
						dt.TenDoiTuong as TenKhachHang,
						dt.DienThoai,
						Case when dt.GioiTinhNam = 1 then N'Nam' else N'Nữ' end as GioiTinh,
						gt.TenDoiTuong as NguoiGioiThieu,
						nk.TenNguonKhach,
						isnull(dt.TenNhomDoiTuongs, N'Nhóm mặc định') as NhomKhachHang ,
						iif( hh.ID_NhomHang is null, '00000000-0000-0000-0000-000000000000',hh.ID_NhomHang) as ID_NhomHang,
						iif(@dtNow <=ctm.HanSuDungGoiDV,1,0) as ThoiHan,						
						ctm.SoLuong,
						ctm.DonGia,
						ctm.SoLuong* ctm.DonGia as ThanhTienChuaCK,
						ctm.SoLuong*ctm.TienChietKhau as TienChietKhau,
						ctm.ThanhTien,
						isnull(tbl.SoLuongTra,0) as SoLuongTra,
						isnull(tbl.GiaTriTra,0) as GiaTriTra,
						isnull(tbl.SoLuongSuDung,0) as SoLuongSuDung,
						iif(@XemGiaVon='0',0, round(isnull(tbl.GiaVon,0),2)) as GiaVon,
						round(ctm.SoLuong- isnull(tbl.SoLuongTra,0) - isnull(tbl.SoLuongSuDung,0),2)  as SoLuongConLai,
						qd.MaHangHoa,
						qd.TenDonViTinh,
						hh.TenHangHoa,
						qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
						lo.MaLoHang,
						nv.NhanVienChietKhau,
						ctm.GiamGiaHD
					from @tblCTMua ctm
					inner join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
					inner join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
					left join DM_LoHang lo on ctm.ID_LoHang= lo.ID
					left join DM_DoiTuong dt on ctm.ID_DoiTuong = dt.ID
					left join DM_DoiTuong gt on dt.ID_NguoiGioiThieu = gt.ID
					left join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
					left join (Select Main.ID_ChiTietHoaDon,
					Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As NhanVienChietKhau
    					From
    					(
    						Select distinct hh_tt.ID_ChiTietHoaDon,
    						(
    							Select tt.TenNhanVien + ' - ' AS [text()]
    							From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
    							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) tt
    							Where tt.ID_ChiTietHoaDon = hh_tt.ID_ChiTietHoaDon
    							For XML PATH ('')
    						) hanghoa_thuoctinh
    					From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
    						inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) hh_tt
    					) Main) as nv on ctm.ID = nv.ID_ChiTietHoaDon
					left join (
						select 
							tblSD.ID_ChiTietGoiDV,
							sum(tblSD.SoLuongTra) as SoLuongTra,
							sum(tblSD.GiaTriTra) as GiaTriTra,
							sum(tblSD.SoLuongSuDung) as SoLuongSuDung,
							sum(tblSD.GiaVon) as GiaVon
						from 
						(
							---- hdsudung
							Select 								
								ct.ID_ChiTietGoiDV,														
								0 as SoLuongTra,
								0 as GiaTriTra,
								ct.SoLuong as SoLuongSuDung,
								ct.SoLuong * ct.GiaVon as GiaVon
							FROM BH_HoaDon hd
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							where hd.ChoThanhToan= 0
							and hd.LoaiHoaDon in (1,25)
							and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
							and exists (select ID from @tblCTMua ctm where ct.ID_ChiTietGoiDV= ctm.ID)

							union all
							--- hdtra
							Select 							
								ct.ID_ChiTietGoiDV,															
								ct.SoLuong as SoLuongTra,
								ct.ThanhTien as GiaTriTra,
								0 as SoLuongSuDung,
								0 as GiaVon
							FROM BH_HoaDon hd
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							where hd.ChoThanhToan= 0
							and hd.LoaiHoaDon = 6
							and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
							and exists (select ID from @tblCTMua ctm where ct.ID_ChiTietGoiDV= ctm.ID)
							)tblSD group by tblSD.ID_ChiTietGoiDV

					) tbl on ctm.ID= tbl.ID_ChiTietGoiDV
				where hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    			and qd.Xoa like @TrangThai
				AND ((select count(Name) from @tblSearchString b where 
					ctm.MaHoaDon like '%'+b.Name+'%'
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or qd.MaHangHoa like '%'+b.Name+'%'
    				or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'
    				or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoituong like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
					)=@count or @count=0)
			) b where b.ThoiHan like @ThoiHan
				and (b.ID_NhomHang like @ID_NhomHang or b.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
				order by NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_ChiTietHangNhap]
    @ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
	@timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@LoaiChungTu [nvarchar](max)
AS
BEGIN

    SET NOCOUNT ON;
	
	DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblIdDonVi
	SELECT donviinput.Name FROM [dbo].[splitstring](@ID_DonVi) donviinput

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblHoaDon TABLE(MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, DienGiai NVARCHAR(max), TenNhomHang NVARCHAR(MAX), 
	MaHangHoa NVARCHAR(MAX), TenHangHoaFull NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), ThuocTinh_GiaTri NVARCHAR(MAX),
	TenDonViTinh NVARCHAR(MAX), TenLoHang NVARCHAR(MAX),
	ID_DonVi UNIQUEIDENTIFIER,
	LoaiHoaDon INT, TyLeChuyenDoi FLOAT,
	SoLuong FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, GiaVon FLOAT, YeuCau NVARCHAR(MAX), GiamGiaHDPT FLOAT, GhiChu nvarchar(max));
	INSERT INTO @tblHoaDon

	select 
		MaHoaDon,
		NgayLapHoaDon,
		DienGiai,
		TenNhomHang,
		MaHangHoa,
		TenHangHoaFull,
		TenHangHoa,
		ThuocTinh_GiaTri, 
		TenDonViTinh,
		TenLoHang,
		ID_DonVi,
		LoaiHoaDon,
		TyLeChuyenDoi,
		SoLuong,
		TienChietKhau,
		ThanhTien,
		GiaVon,
		YeuCau,
		GianGiaHD,
		GhiChu
	from
	(
	SELECT bhd.MaHoaDon, IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) AS NgayLapHoaDon,
		bhd.DienGiai,
		nhh.TenNhomHangHoa AS TenNhomHang, 
		nhh.TenNhomHangHoa_KhongDau AS TenNhomHangHoa_KhongDau, 
		dvqdChuan.MaHangHoa, 
		concat(hh.TenHangHoa , dvqd.ThuocTinhGiaTri) AS TenHangHoaFull,
		hh.TenHangHoa, 
		ISNULL(dvqd.ThuocTinhGiaTri, '') AS ThuocTinh_GiaTri, 
		dvqdChuan.TenDonViTinh,
		lh.MaLoHang AS TenLoHang, 
		iif(bhd.LoaiHoaDon=10, bhd.ID_CheckIn,bhd.ID_DonVi) as ID_DonVi,	
		bhd.LoaiHoaDon, 
		dvqd.TyLeChuyenDoi, 
		bhdct.SoLuong,
		bhdct.TienChietKhau,
		bhdct.ThanhTien, 
		case bhd.LoaiHoaDon
			when 6 then case when ctm.GiaVon is null or bhdct.ID_ChiTietDinhLuong is null then bhdct.GiaVon else ctm.GiaVon end
			when 10 then case when bhd.ID_CheckIn= dv.ID then bhdct.GiaVon_NhanChuyenHang else bhdct.GiaVon end
		else bhdct.GiaVon end as GiaVon,		
		--- neu trahanghoadon voi dichvu co TPdinhluong --> lay giavon cua hanghoa)
		--iif(bhd.LoaiHoaDon= 6,  iif(ctm.GiaVon is null, bhdct.GiaVon, iif(bhdct.ID_ChiTietDinhLuong is null, ctm.GiaVon, bhdct.GiaVon)), bhdct.GiaVon) as GiaVon,		
		bhd.YeuCau,
		IIF(bhd.TongTienHang = 0, 0, bhd.TongGiamGia / bhd.TongTienHang) as GianGiaHD,
		bhdct.GhiChu,
		hh.TenHangHoa_KhongDau,
		hh.TenHangHoa_KyTuDau,
		iif(@SearchString='',bhdct.GhiChu, dbo.FUNC_ConvertStringToUnsign(bhdct.GhiChu)) as GhiChuUnsign,
		iif(@SearchString='',bhd.DienGiai, dbo.FUNC_ConvertStringToUnsign(bhd.DienGiai)) as DienGiaiUnsign
    FROM BH_HoaDon_ChiTiet bhdct
	left join BH_HoaDon_ChiTiet ctm on bhdct.ID_ChiTietGoiDV = ctm.ID
    INNER JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
	join @tblIdDonVi dv on (bhd.ID_DonVi = dv.ID and bhd.LoaiHoaDon!=10) or (bhd.ID_CheckIn = dv.ID and bhd.YeuCau='4')
    INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
	INNER JOIN DonViQuiDoi dvqdChuan ON dvqdChuan.ID_HangHoa = dvqd.ID_HangHoa AND dvqdChuan.LaDonViChuan = 1
	INNER JOIN (select Name from splitstring(@LoaiChungTu)) lhd ON bhd.LoaiHoaDon = lhd.Name
	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
	INNER JOIN DM_NhomHangHoa nhh  ON nhh.ID = hh.ID_NhomHang   
    INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh  ON nhh.ID = allnhh.ID   
	LEFT JOIN DM_LoHang lh   ON lh.ID = bhdct.ID_LoHang OR (bhdct.ID_LoHang IS NULL AND lh.ID IS NULL)
    WHERE bhd.ChoThanhToan = 0
	and (bhdct.ChatLieu is null or bhdct.ChatLieu !='2')
    AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) >= @timeStart
	AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) < @timeEnd
	--and exists (select ID from @tblIdDonVi dv where (bhd.ID_DonVi = dv.ID and bhd.LoaiHoaDon!=10) or (bhd.ID_CheckIn = dv.ID and bhd.YeuCau='4'))
	AND hh.LaHangHoa = 1 AND hh.TheoDoi LIKE @TheoDoi AND dvqd.Xoa LIKE @TrangThai 
	) tbl 
	where ((select count(Name) from @tblSearchString b where 
    		tbl.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    	
    			or tbl.TenHangHoa like '%'+b.Name+'%'
    			or tbl.TenLoHang like '%' +b.Name +'%' 
    			or tbl.MaHangHoa like '%'+b.Name+'%'
				or tbl.MaHangHoa like '%'+b.Name+'%'
    			or tbl.TenNhomHang like '%'+b.Name+'%'
    			or tbl.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or tbl.TenDonViTinh like '%'+b.Name+'%'
    			or tbl.ThuocTinh_GiaTri like '%'+b.Name+'%'
				or tbl.DienGiai like N'%'+b.Name+'%'
				or tbl.MaHoaDon like N'%'+b.Name+'%'
    			or tbl.GhiChu like N'%'+b.Name+'%'
				or tbl.GhiChuUnsign like '%'+b.Name+'%'
				or tbl.DienGiaiUnsign like '%'+b.Name+'%'
				)=@count or @count=0);	

	SELECT ct.TenLoaiChungTu as LoaiHoaDon, pstk.MaHoaDon, pstk.NgayLapHoaDon, pstk.TenNhomHang, pstk.MaHangHoa, pstk.TenHangHoaFull,
	pstk.TenHangHoa, pstk.ThuocTinh_GiaTri, pstk.TenDonViTinh, pstk.TenLoHang,DienGiai, pstk.GhiChu,
	dv.TenDonVi, dv.MaDonVi,  pstk.SoLuongNhap AS SoLuong, IIF(@XemGiaVon = '1',ROUND(pstk.GiaTriNhap,0) , 0) as ThanhTien
	FROM 
	(
    SELECT 
    MaHoaDon, NgayLapHoaDon, TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	LoaiHoaDon, DienGiai, GhiChu,
	IIF(LoaiHoaDon = 10 and YeuCau = '4', TienChietKhau* TyLeChuyenDoi, SoLuong * TyLeChuyenDoi) AS SoLuongNhap,
	IIF(LoaiHoaDon = 10 and YeuCau = '4' ,TienChietKhau* GiaVon, iif(LoaiHoaDon = 6, SoLuong * GiaVon,   ThanhTien*(1-GiamGiaHDPT))) AS GiaTriNhap
    FROM @tblHoaDon WHERE LoaiHoaDon != 9

	UNION ALL
    SELECT 
    MaHoaDon, NgayLapHoaDon, TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	LoaiHoaDon,DienGiai, GhiChu,
	sum(SoLuong * TyLeChuyenDoi) as SoLuongNhap,	
	SUM(SoLuong * TyLeChuyenDoi * GiaVon) AS GiaTriNhap
    FROM @tblHoaDon
    WHERE LoaiHoaDon = 9 and SoLuong > 0 
    GROUP BY LoaiHoaDon, MaHoaDon, NgayLapHoaDon,TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,DienGiai, GhiChu
	) pstk
	join DM_DonVi dv on pstk.ID_DonVi= dv.ID
	INNER JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
	order by pstk.NgayLapHoaDon desc

END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TongHopHangNhap]
    @ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
	@timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@LoaiChungTu [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblIdDonVi
	SELECT donviinput.Name FROM [dbo].[splitstring](@ID_DonVi) donviinput

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblHoaDon TABLE(TenNhomHang NVARCHAR(MAX), MaHangHoa NVARCHAR(MAX), TenHangHoaFull NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), 
	ThuocTinh_GiaTri NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), TenLoHang NVARCHAR(MAX),
	ID_DonVi UNIQUEIDENTIFIER,
	LoaiHoaDon INT, TyLeChuyenDoi FLOAT, 
	SoLuong FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, GiaVon FLOAT, YeuCau NVARCHAR(MAX), GiamGiaHDPT FLOAT);
	INSERT INTO @tblHoaDon

	SELECT nhh.TenNhomHangHoa AS TenNhomHang, dvqdChuan.MaHangHoa, 
		CONCAT(hh.TenHangHoa,dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
		hh.TenHangHoa, 
		ISNULL(dvqd.ThuocTinhGiaTri, '') AS ThuocTinh_GiaTri, 
		dvqdChuan.TenDonViTinh,
		lh.MaLoHang AS TenLoHang, 
		iif(bhd.LoaiHoaDon=10, bhd.ID_CheckIn,bhd.ID_DonVi) as ID_DonVi,
		bhd.LoaiHoaDon, dvqd.TyLeChuyenDoi, 
		bhdct.SoLuong, 
		bhdct.TienChietKhau,
		bhdct.ThanhTien, 
		case bhd.LoaiHoaDon
			when 6 then case when ctm.GiaVon is null  then bhdct.GiaVon else ctm.GiaVon end
			when 10 then case when bhd.ID_CheckIn= dv.ID then bhdct.GiaVon_NhanChuyenHang else bhdct.GiaVon end
		else bhdct.GiaVon end as GiaVon,		
		--iif(bhd.LoaiHoaDon=6, iif(ctm.GiaVon is null, bhdct.GiaVon, ctm.GiaVon), bhdct.GiaVon) as GiaVon,
		bhd.YeuCau, 
		IIF(bhd.TongTienHang = 0, 0, bhd.TongGiamGia / bhd.TongTienHang) as GiamGiaHDPT
    FROM BH_HoaDon_ChiTiet bhdct
	left join BH_HoaDon_ChiTiet ctm on bhdct.ID_ChiTietGoiDV = ctm.ID --- khachtrahang: lay giavon tu hoadonmua
    INNER JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
	join @tblIdDonVi dv on (bhd.ID_DonVi = dv.ID and bhd.LoaiHoaDon!=10) or (bhd.ID_CheckIn = dv.ID and bhd.YeuCau='4')
    INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
	INNER JOIN DonViQuiDoi dvqdChuan ON dvqdChuan.ID_HangHoa = dvqd.ID_HangHoa AND dvqdChuan.LaDonViChuan = 1
	INNER JOIN (select Name from splitstring(@LoaiChungTu)) lhd ON bhd.LoaiHoaDon = lhd.Name
	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
	INNER JOIN DM_NhomHangHoa nhh  ON nhh.ID = hh.ID_NhomHang   
    INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh   ON nhh.ID = allnhh.ID  
	LEFT JOIN DM_LoHang lh  ON lh.ID = bhdct.ID_LoHang OR (bhdct.ID_LoHang IS NULL and lh.ID is null)   
    WHERE bhd.ChoThanhToan = 0
	and (bhdct.ChatLieu is null or bhdct.ChatLieu!='2')
    AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) >= @timeStart 
	AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) < @timeEnd
	-- chi lay phieu chuyenhang (neu la chinhanh nhan)
	--and exists (select ID from @tblIdDonVi dv where (bhd.ID_DonVi = dv.ID and bhd.LoaiHoaDon!=10) or (bhd.ID_CheckIn = dv.ID and bhd.YeuCau='4'))
	AND hh.LaHangHoa = 1 AND hh.TheoDoi LIKE @TheoDoi AND dvqd.Xoa LIKE @TrangThai 
	AND ((select count(Name) from @tblSearchString b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa like '%'+b.Name+'%'
    			or lh.MaLoHang like '%' +b.Name +'%' 
    		or dvqd.MaHangHoa like '%'+b.Name+'%'
			or dvqdChuan.MaHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    			or dvqd.TenDonViTinh like '%'+b.Name+'%'
    			or dvqd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)	
	

	SELECT pstk.TenNhomHang, pstk.MaHangHoa, pstk.TenHangHoaFull, pstk.TenHangHoa, pstk.ThuocTinh_GiaTri,
	pstk.TenDonViTinh, pstk.TenLoHang,
	dv.TenDonVi, dv.MaDonVi, 
	SUM(pstk.SoLuongNhap) AS SoLuong, IIF(@XemGiaVon = '1',ROUND(SUM(pstk.GiaTriNhap),0) , 0) as ThanhTien
	FROM 
	(
	-- hoadon
    SELECT 
    TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	IIF(LoaiHoaDon = 10 and YeuCau = '4', TienChietKhau* TyLeChuyenDoi, SoLuong * TyLeChuyenDoi) AS SoLuongNhap,
	IIF(LoaiHoaDon = 10 and YeuCau = '4' , TienChietKhau* GiaVon,iif(LoaiHoaDon = 6, SoLuong * GiaVon, ThanhTien*(1-GiamGiaHDPT))) AS GiaTriNhap
    FROM @tblHoaDon WHERE LoaiHoaDon != 9

	UNION ALL
    SELECT 
	-- phieukiemke
    TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	Sum(SoLuong * TyLeChuyenDoi) as SoLuongNhap,	
	SUM(SoLuong * TyLeChuyenDoi * GiaVon) AS GiaTriNhap
    FROM @tblHoaDon
    WHERE LoaiHoaDon = 9 and SoLuong > 0 --- chi lay chitiet kiemke neu soluonglech tang
    GROUP BY TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi
	) pstk
	join DM_DonVi dv on pstk.ID_DonVi= dv.ID
	GROUP BY pstk.TenNhomHang, pstk.MaHangHoa, pstk.TenHangHoaFull, pstk.TenHangHoa, pstk.ThuocTinh_GiaTri, pstk.TenDonViTinh, pstk.TenLoHang,
	pstk.ID_DonVi, dv.TenDonVi, dv.MaDonVi
	order by MaHangHoa

END");

			Sql(@"ALTER PROCEDURE [dbo].[GetInforSoQuy_ByID]
    @ID_PhieuThuChi [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    select 
    		qhd.ID,
    		qhd.MaHoaDon,
    		qhd.NgayLapHoaDon,
			qhd.ID_NhanVien,
    			qhd.PhieuDieuChinhCongNo,
    			isnull(qhd.TrangThai,'1') as TrangThai,
    			case when sum(qct.TienThu)=0 and sum(ISNULL(qct.DiemThanhToan,0)) > 0 then 1 else 0 end as PhieuDieuChinhDiem,
    		MAX(qhd.LoaiHoaDon) as LoaiHoaDon ,
    			case when sum(isnull(qct.TienThu,0))=0 then sum(ISNULL(qct.DiemThanhToan,0)) else sum(isnull(qct.TienThu,0)) end as TongTienThu,
    		MAX(ISNULL(qhd.NoiDungThu,'')) as NoiDungThu,
    		MAX(ISNULL(nv.TenNhanVien,'')) as TenNhanVien,
    		MAX(ISNULL(dt.TenDoiTuong,'')) as NguoiNopTien
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    	left join NS_NhanVien nv on qhd.ID_NhanVien= nv.ID
    	left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID
    	where qhd.ID like @ID_PhieuThuChi 
    	group by qhd.ID, qhd.MaHoaDon,qhd.NgayLapHoaDon,qhd.LoaiHoaDon,qhd.PhieuDieuChinhCongNo, qhd.TrangThai,qhd.ID_NhanVien
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetInForStaff_Working_byChiNhanh]
    @ID_DonVi [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
    SELECT distinct nv.ID,MaNhanVien,TenNhanVien,DienThoaiDiDong, GioiTinh, qt.ID_DonVi, ISNULL(AnhNV.URLAnh, '') AS Image
    	from NS_NhanVien nv
    	left join NS_QuaTrinhCongTac qt on nv.ID = qt.ID_NhanVien
		LEFT JOIN (SELECT ID_NhanVien, URLAnh FROM NS_NhanVien_Anh
		WHERE SoThuTu = 1
		GROUP BY ID_NhanVien, URLAnh) AS AnhNV ON AnhNV.ID_NhanVien = nv.ID
    	where qt.ID_DonVi like @ID_DonVi and nv.DaNghiViec!='1'
    	and (nv.TrangThai is null OR nv.TrangThai='1') 
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_DMHangHoa_Import]
    @MaHH [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @ID_LoHang [uniqueidentifier]
AS
BEGIN
	SELECT TOP(1) dvqd.ID as ID_DonViQuiDoi, dvqd.MaHangHoa,
    hh.TenHangHoa,
    gv.ID as ID_GiaVon,
	tk.ID as ID_TonKho,
    gv.ID_LoHang as ID_LoHang,
	dvqd.LaDonViChuan,
	dvqd.ID_HangHoa,
    Case when gv.ID is null then 0 else CAST(ROUND((gv.GiaVon), 0) as float) end  as GiaVon, 
    CAST(ROUND((dvqd.GiaBan), 0) as float) as GiaBan,  
    Case when hh.LaHangHoa = 0 then 0 else CAST(ROUND(ISNULL(tk.TonKho, 0), 3) as float) end as TonCuoiKy
    FROM DonViQuiDoi dvqd 
	JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	LEFT join DM_HangHoa_TonKho tk on tk.ID_DonViQuyDoi = dvqd .ID
	LEFT Join DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and (gv.ID_LoHang = @ID_LoHang or @ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh
	where dvqd.Xoa = 0 and tk.ID_DonVi = @ID_ChiNhanh
	and (tk.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
	and dvqd.MaHangHoa like @MaHH
	and hh.TheoDoi = 1
END

");

			Sql(@"ALTER PROCEDURE [dbo].[getList_DMLoHang_TonKho_byMaLoHang]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_DonViQuiDoi [nvarchar](max),
    @MaLoHang [nvarchar](max)
AS
BEGIN
    SELECT 
    	dvqd.ID as ID_DonViQuiDoi,
		dvqd.ID_HangHoa,
    	gv.ID as ID_GiaVon,
    	lh.ID as ID_LoHang,
		tk.ID as ID_TonKho,
    	Case when gv.ID is null then 0 else	CAST (ROUND(gv.GiaVon, 0) as float) end as GiaVon,
    	CAST(ROUND((ISNULL(tk.TonKho,0)), 3) as float) as TonCuoiKy
    FROM DonViQuiDoi dvqd     		
	JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
	LEFT JOIN DM_LoHang lh on dhh.ID = lh.ID_HangHoa
	LEFT JOIN DM_HangHoa_TonKho tk on dvqd.ID = tk.ID_DonViQuyDoi and tk.ID_DonVi = @ID_ChiNhanh   and lh.ID= tk.ID_LoHang
    left join DM_GiaVon gv on gv.ID_DonViQuiDoi = dvqd.ID and gv.ID_LoHang = tk.ID_LoHang and gv.ID_DonVi = @ID_ChiNhanh
    where dvqd.ID = @ID_DonViQuiDoi
    	and lh.MaLoHang = @MaLoHang
    	order by lh.NgayTao DESC
END
");

			Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoaXuatHuybyID]
    @ID_HoaDon [uniqueidentifier],
	@ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
  set nocount on;

		declare @loaiHD int, @ID_HoaDonGoc uniqueidentifier		
		select @loaiHD = LoaiHoaDon, @ID_HoaDonGoc= ID_HoaDon from BH_HoaDon where ID= @ID_HoaDon

		select 
			ctxk.ID,ctxk.ID_DonViQuiDoi,ctxk.ID_LoHang,
			dvqd.ID_HangHoa,
			ctxk.SoLuong, ctxk.SoLuong as SoLuongXuatHuy,
			ctxk.DonGia,
			ctxk.GiaVon, 
			ctxk.GiaTriHuy, ctxk.TienChietKhau as GiamGia,
			ctxk.GhiChu,
			ctxk.SoThuTu,
			ctxk.ChatLieu,
			hd.MaHoaDon,
			hd.NgayLapHoaDon,
			hd.ID_NhanVien,
    		nv.TenNhanVien,
			lh.NgaySanXuat,
    		lh.NgayHetHan,    			
    		dvqd.MaHangHoa,
    		hh.TenHangHoa,
			Case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang, 
    		concat(hh.TenHangHoa , '', dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
    		dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND(3, 0) as float) as TrangThaiMoPhieu,
			ROUND(ISNULL(TonKho,0),2) as TonKho
		from 
		(
		--- get ct if has tpdinhluong
		select max(ct.ID) as ID,
			max(ct.SoThuTu) as SoThuTu,
			ct.ChatLieu,
			ct.ID_DonViQuiDoi,
			ct.ID_LoHang,
			@ID_HoaDon as ID_HoaDon,
			max(ct.GiaVon) as DonGia,
			max(ct.GiaVon) as GiaVon,
			sum(ct.SoLuong) as SoLuong, 
			--max(iif(@loaiHD=8, ct.DonGia, ct.GiaVon)) as  DonGia, --- xuatbanle dinhluong: get GiaVon, else: xuatkho get DonGia
			--max(iif(@loaiHD=8, ct.DonGia, ct.GiaVon)) as GiaVon,
			sum(iif(@loaiHD=8, ct.ThanhTien , ct.SoLuong *ct.GiaVon)) as GiaTriHuy,					
			max(ct.TienChietKhau) as TienChietKhau,
			max(ct.GhiChu) as GhiChu
		from BH_HoaDon_ChiTiet ct
		where ct.ID_HoaDon= @ID_HoaDon		
		group by ct.ID_DonViQuiDoi, ct.ID_LoHang, ct.ChatLieu	
		)ctxk
		join BH_HoaDon hd on hd.ID= ctxk.ID_HoaDon
		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		where (hh.LaHangHoa = 1 and tk.TonKho is not null)
END");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDon_afterTraHang_DichVu]
   @IDChiNhanhs nvarchar(max) = null,
   @IDNhanViens nvarchar(max) = null,
   @DateFrom datetime = null,
   @DateTo datetime = null,
   @TextSearch nvarchar(max) = null,
   @CurrentPage int =null,
   @PageSize int = null
AS
BEGIN
	set nocount on;

	if isnull(@CurrentPage,'') =''
		set @CurrentPage = 0
	if isnull(@PageSize,'') =''
		set @PageSize = 10

	declare @sql nvarchar(max) ='', @sqlTemp nvarchar(max),  	
				@whereTemp nvarchar(max)= '', @whereTempOut nvarchar(max)= '', 
				@paramDefined nvarchar(max)=''

	
	set @whereTemp = N' where 1 = 1 and hd.ChoThanhToan = 0 and hd.LoaiHoaDon in (1,25) and
						(hdct.ID_ChiTietDinhLuong is null OR hdct.ID_ChiTietDinhLuong = hdct.ID)'
	set @whereTempOut= N' where 1 = 1  '

	set @paramDefined = N' @IDChiNhanhs_In nvarchar(max) ,
		   @IDNhanViens_In nvarchar(max) ,
		   @DateFrom_In datetime ,
		   @DateTo_In datetime ,
		   @TextSearch_In nvarchar(max) ,
		   @CurrentPage_In int =null,
		   @PageSize_In int  '

	if isnull(@IDChiNhanhs,'') !=''
	begin
		set @sqlTemp = N' DECLARE @tblChiNhanh table (ID nvarchar(max)) insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) '
		set @whereTemp = CONCAT(@whereTemp,  N' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID) ')	
	end

	if isnull(@IDNhanViens,'') !=''
	begin
		set @sqlTemp = CONCAT(@sqlTemp, N'
			DECLARE @tblNhanVien table (ID uniqueidentifier)
			insert into @tblNhanVien select name from dbo.splitstring(@IDNhanViens_In)')
		set @whereTemp = CONCAT(@whereTemp,  ' and exists (select nv.ID from @tblNhanVien nv where hd.ID_NhanVien = nv.ID)')
	end

	if isnull(@TextSearch,'') !=''
	begin
		set @sqlTemp = CONCAT(@sqlTemp, N'
								DECLARE @tblSearch TABLE (Name [nvarchar](max))
								DECLARE @count int
								 INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''' 
								 select @count =  (Select count(*) from @tblSearch)')
		set @whereTempOut = CONCAT(@whereTempOut,  ' and
					((select count(Name) from @tblSearch b where     			
					a.MaHoaDon like ''%''+b.Name+''%''								
					or dt.MaDoiTuong like ''%''+b.Name+''%''		
					or dt.TenDoiTuong like ''%''+b.Name+''%''
					or dt.TenDoiTuong_KhongDau like ''%''+b.Name+''%''
					or dt.DienThoai like ''%''+b.Name+''%''		
					or xe.BienSo like ''%''+b.Name+''%''			
					)=@count or @count=0)')
	end

	if isnull(@DateFrom,'') !=''
	begin		
		set @whereTemp= CONCAT(@whereTemp,  N' and hd.NgayLapHoaDon >= @DateFrom_In')
	end

	if isnull(@DateTo,'') !=''
	begin		
		set @DateTo = DATEADD(day, 1, @DateTo)
		set @whereTemp= CONCAT(@whereTemp,  N' and hd.NgayLapHoaDon < @DateTo_In')
	end

	set @sqlTemp = CONCAT(@sqlTemp, N'
			select *
			into #tblView
			from 
			(
				select a.*,
					row_number() over (order by a.NgayLapHoaDon desc) as Rn,
					COUNT(a.ID) OVER () as TotalRow,
					dt.DienThoai,
					xe.BienSo,
					ISNULL(dt.MaDoiTuong,''kl'' ) as MaDoiTuong,
					ISNULL(dt.TenDoiTuong,N''Khách lẻ'' ) as TenDoiTuong,
					ISNULL(dt.TenDoiTuong_KhongDau,''kl'' ) as TenDoiTuong_KhongDau,
					ISNULL(dt.TenDoiTuong_ChuCaiDau,''kl'' ) as TenDoiTuong_ChuCaiDau
				from
				(
				select 
					conlai.ID,
					conlai.ID_DoiTuong,
					max(conlai.ID_Xe) as ID_Xe,
					max(conlai.MaHoaDon) as MaHoaDon,
					max(conlai.NgayLapHoaDon) as NgayLapHoaDon,
					sum(conlai.SoLuongBan) as SoLuongBan,
					sum(conlai.SoLuongTra) as SoLuongTra,
					sum(conlai.SoLuongBan) - ISNULL(sum(conlai.SoLuongTra),0) as SoLuongConLai
				from
				(
					-- sum SLMua
    					Select 
    						hd.ID as ID,									
							hd.ID_DoiTuong,
							hd.ID_Xe,
							hd.MaHoaDon,
							hd.NgayLapHoaDon,							
    						sum(iif(hd.LoaiHoaDon = 1 
								or ((hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
								and (hdct.ID_ParentCombo is null or hdct.ID_ParentCombo= hdct.ID)) , 
							hdct.SoLuong,  isnull(xk.SoLuongXuat,0))) as SoLuongBan,						
    						0 as SoLuongTra
    					from BH_HoaDon hd   				
    					inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
						left join 
						(
							select SUM(ctxk.SoLuong) as SoLuongXuat, ctxk.ID_ChiTietGoiDV, hdxk.ID_HoaDon
							from BH_HoaDon_ChiTiet ctxk 
							join BH_HoaDon hdxk on ctxk.ID_HoaDon = hdxk.ID
							where hdxk.ID_HoaDon is not null
							and hdxk.LoaiHoaDon = 8 and hdxk.ChoThanhToan=0
							group by ctxk.ID_ChiTietGoiDV, hdxk.ID_HoaDon			
						) xk on hd.ID= xk.ID_HoaDon and hdct.ID= xk.ID_ChiTietGoiDV  						
						', @whereTemp, 
						' Group by hd.ID,hd.NgayLapHoaDon , hd.LoaiHoaDon,hd.ID_DoiTuong,hd.MaHoaDon, hd.ID_Xe
					
						union all
						-- sum SLTra
						select 
    						hd.ID_HoaDon as ID,			
							hd.ID_DoiTuong,
							''00000000-0000-0000-0000-000000000000'' as ID_Xe,
							'''' as MaHoaDon,
							null as NgayLapHoaDon,
    						0 as SoLuongBan,						
    						Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongTra
    					from BH_HoaDon hd    				
    					inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon  
						where 1 = 1 and hd.ChoThanhToan = 0  and hd.loaihoadon = 6
						and (hdct.ID_ChiTietDinhLuong is null OR hdct.ID_ChiTietDinhLuong = hdct.ID)						
						and hdct.ChatLieu= 1
    					Group by hd.ID_HoaDon,hd.ID_DoiTuong
				) conlai group by conlai.ID, conlai.ID_DoiTuong						
			) a
			left join DM_DoiTuong dt on a.ID_DoiTuong = dt.ID 
			left join Gara_DanhMucXe xe on a.ID_Xe = xe.ID
			',
			@whereTempOut,  ' and a.SoLuongConLai > 0 ' ,
			') b where b.Rn between  (@CurrentPage_In * @PageSize_In) + 1 and @PageSize_In * (@CurrentPage_In + 1)')

	

		set @sql= CONCAT(@sql, N' select 
							tbl.ID,
							tbl.SoLuongTra,
    						tbl.SoLuongBan,
							tbl.TotalRow,
							tbl.MaDoiTuong,
							tbl.TenDoiTuong,
							tbl.DienThoai,
							tbl.BienSo,
    						hd.MaHoaDon,
    						hd.LoaiHoaDon,
    						hd.NgayLapHoaDon,   						
    						hd.ID_DoiTuong,	
    						hd.ID_HoaDon,
    						hd.ID_BangGia,
    						hd.ID_NhanVien,
    						hd.ID_DonVi,
							hd.ID_Xe,
    						hd.NguoiTao,	
    						hd.DienGiai,	    						
							isnull(thuchi.TienThu,0) as KhachDaTra,
							ISNULL(thuchi.ThuTuThe, 0) as ThuTuThe,
    						ISNULL(nv.TenNhanVien,'''' ) as TenNhanVien,							
    						ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
    						ISNULL(hd.TongTienHang, 0) as TongTienHang,
    						ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
							isnull(hd.TongChietKhau,0) as  TongChietKhau,
    						ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich,							
							ISNULL(hd.TongTienThue, 0) as TongTienThue,						
							isnull(hd.PTThueHoaDon,0) as  PTThueHoaDon,
							ISNULL(hd.TongThueKhachHang, 0) as TongThueKhachHang,	
							isnull(hd.TongTienThueBaoHiem,0) as  TongTienThueBaoHiem,
							isnull(hd.TongTienBHDuyet,0) as  TongTienBHDuyet,
							isnull(hd.SoVuBaoHiem,0) as  SoVuBaoHiem,
							isnull(hd.PTThueBaoHiem,0) as  PTThueBaoHiem,
							isnull(hd.KhauTruTheoVu,0) as  KhauTruTheoVu,
							isnull(hd.GiamTruBoiThuong,0) as  GiamTruBoiThuong,
							isnull(hd.PTGiamTruBoiThuong,0) as  PTGiamTruBoiThuong,
							isnull(hd.BHThanhToanTruocThue,0) as  BHThanhToanTruocThue,
							isnull(hd.PhaiThanhToanBaoHiem,0) as  PhaiThanhToanBaoHiem
						from #tblView tbl
						join BH_HoaDon hd on tbl.ID= hd.ID
						left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
						left join (
							select 
								tblQuy.ID_HoaDonLienQuan,
								sum(tblQuy.ThuTuThe) as ThuTuThe,
								sum(tblQuy.TienThu) as TienThu
							from
							(
								---- Thu tu HDMua
								select qct.ID_HoaDonLienQuan,
									case when qhd.TrangThai= 0 then 0 else SUM(iif(qct.HinhThucThanhToan= 4, qct.TienThu,0)) end as ThuTuThe,							
									Case when qhd.TrangThai = 0 then 0 else 
										Case when qhd.LoaiHoaDon = 11 then SUM(ISNULL(qct.Tienthu, 0)) else - SUM(ISNULL(qct.Tienthu, 0)) end end as TienThu
								from Quy_HoaDon_ChiTiet qct					
								join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID		
								join #tblView tbl on qct.ID_HoaDonLienQuan = tbl.ID				
								group by qct.ID_HoaDonLienQuan, qhd.TrangThai,qhd.LoaiHoaDon
							) tblQuy group by tblQuy.ID_HoaDonLienQuan
						) thuchi on tbl.ID= thuchi.ID_HoaDonLienQuan')

		--print @sqlTemp
		--print @sql

		set @sql = CONCAT(@sqlTemp, @sql)
				
		exec sp_executesql @sql, @paramDefined, 
							@IDChiNhanhs_In = @IDChiNhanhs,
							@IDNhanViens_In = @IDNhanViens,
							@DateFrom_In = @DateFrom,
							@DateTo_In = @DateTo,
							@TextSearch_In = @TextSearch,
							@CurrentPage_In = @CurrentPage,
							@PageSize_In = @PageSize
    	
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_HoaDonDieuChinh]
    @ID_ChiNhanh [nvarchar](max),
    @MaHoaDon [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @TrangThai1 [nvarchar](max),
    @TrangThai2 [nvarchar](max),
    @TrangThai3 [nvarchar](max)
AS
BEGIN
    Select 
    	hd.ID as ID_HoaDon,
		hd.ID_DonVi,
    	hd.ID_NhanVien,
    	Max(hd.MaHoaDon) as MaHoaDon,
    	hd.ChoThanhToan,
    	Max(hd.NgayLapHoaDon) as NgayLapHoaDon,
    	COUNT(*) as SoLuongHangHoa,
    	CAST(ROUND(SUM(hdct.PTChietKhau), 3) as float) as TongGiaVonTang,
    	CAST(ROUND(SUM (hdct.TienChietKhau), 3) as float) as TongGiaVonGiam,
    	Max(dv.TenDonVi) as TenDonVi,
    	--Max (hd.DienGiai) as DienGiai,
		Case when Max(hd.DienGiai) not like N'%Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa:%' then Max(hd.DienGiai) else Max(hd.DienGiai) + ' ' + Max(dvqd.MaHangHoa) + 
		Case when Max(lh.MaLoHang) is null then '' else '. Lô: ' + Max(lh.MaLoHang) end end as DienGiai,
    	MAX (hd.NguoiTao) as NguoiTao,
    	MAx(nv.TenNhanVien) as NguoiDieuChinh,
    	Case when hd.YeuCau = N'Tạm lưu' then N'Phiếu tạm' else
    	Case when hd.YeuCau = N'Hoàn thành' then N'Đã điều chỉnh' else N'Đã hủy' end end as TrangThai
    	FROM BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    	inner join DM_DonVi dv on hd.ID_DonVi = dv.ID
		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    	where hd.LoaiHoaDon = 18
    	and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	and (hd.YeuCau like @TrangThai1 or hd.YeuCau like @TrangThai2 or hd.YeuCau like @TrangThai3)
    	and hd.MaHoaDon like @MaHoaDon
    	and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    	GROUP BY hd.ID, hd.YeuCau, hd.ChoThanhToan, hd.ID_NhanVien,hd.ID_DonVi
    	ORDER BY MAX(hd.NgayLapHoaDon) DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_HoaDonDieuChinh_ChiTiet]
    @ID_HoaDon [uniqueidentifier]
AS
BEGIN
    Select 
    	dvqd.ID as ID_DonViQuiDoi,
		hdct.ID_LoHang,
		hh.QuanLyTheoLoHang,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
			Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
			Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
			Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan,
    	    CAST(ROUND(hdct.DonGia, 3) as float) as GiaVonHienTai,
    		CAST(ROUND(hdct.GiaVon, 3) as float) as GiaVonMoi,
    		CAST(ROUND(hdct.PTChietKhau, 0) as float) as GiaVonTang,
    		CAST(ROUND(hdct.TienChietKhau, 3) as float) as GiaVonGiam,
			CAST(ROUND(hdct.GiaVon - hdct.DonGia, 3) as float) as ChenhLech,
    			hdct.SoThuTu
    	From BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    	left join 
    			(
    			Select Main.id_hanghoa,
    			Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    			From
    			(
    			Select distinct hh_tt.id_hanghoa,
    				(
    					Select tt.GiaTri + ' - ' AS [text()]
    					From dbo.hanghoa_thuoctinh tt
    					Where tt.id_hanghoa = hh_tt.id_hanghoa
    					order by tt.ThuTuNhap 
    					For XML PATH ('')
    				) hanghoa_thuoctinh
    			From dbo.hanghoa_thuoctinh hh_tt
    			) Main
    		) tt on hh.ID = tt.id_hanghoa
    	where hd.ID = @ID_HoaDon
    	ORDER BY hdct.SoThuTu DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_SuKienToDay_v2]
    @ID_DonVi [uniqueidentifier],
    @Date [datetime]
AS
BEGIN
    set nocount on;
    	DECLARE @SoNgay int;
    	DECLARE @DateNow datetime= @Date;
    	DECLARE @tblCalendar table (ID uniqueidentifier, ID_DonVi uniqueidentifier, ID_NhanVien uniqueidentifier, NgayGio datetime, PhanLoai int)
    	insert into @tblCalendar exec GetListLichHen_FullCalendar_Dashboard @ID_DonVi,'%%',@Date ;
    	DECLARE @SinhNhat FLOAT, @CongViec FLOAT, @LichHen FLOAT, @SoLoSapHetHan FLOAT, @SoLoHetHan FLOAT, @XeMoiTiepNhan FLOAT, @XeXuatXuong FLOAT;
    
    	select @SinhNhat = Count(ID) from DM_DoiTuong 
    		where TheoDoi != 1 and NgaySinh_NgayTLap is not null
    		and DAY(NgaySinh_NgayTLap) = DAY(@DateNow)
    		and MONTH(NgaySinh_NgayTLap)= MONTH(@DateNow);
    
    		Select @CongViec = COUNT(CASE WHEN PhanLoai = 4 THEN 1 END),
    		@LichHen = COUNT(CASE WHEN PhanLoai = 3 THEN 1 END)
    		from @tblCalendar where PhanLoai IN (3, 4);
    
    		Select 
    		@SoLoSapHetHan = COUNT(CASE WHEN a.SoNgayConHan < =@SoNgay and a.SoNgayConHan > 0 THEN 1 END),
    		@SoLoHetHan = COUNT(CASE WHEN a.SoNgayConHan < 1 THEN 1 END)
    		from (
    			SELECT DATEDIFF(day,DATEADD(day,-1,@DateNow), lh.NgayHetHan) as SoNgayConHan FROM DM_LoHang lh
    			JOIN (Select ID_LoHang, SUM(TonKho) as TonKho from DM_HangHoa_TonKho where ID_DonVi = @ID_DonVi GROUP BY ID_LoHang) tk on lh.ID = tk.ID_LoHang
    			where lh.NgayHetHan is not null 
    			and tk.TonKho > 0
    			) as a;
    
    		SELECT @XeMoiTiepNhan = COUNT(CASE WHEN convert(varchar(10), NgayVaoXuong, 102) 
    		= convert(varchar(10), @DateNow, 102) THEN 1 END),
    			@XeXuatXuong = COUNT(CASE WHEN convert(varchar(10), NgayXuatXuong, 102) 
    		= convert(varchar(10), @DateNow, 102) THEN 1 END)
    		FROM Gara_PhieuTiepNhan
    		WHERE TrangThai != 0;
    
    		DECLARE @KhachHangMoi FLOAT;
    		SELECT @KhachHangMoi = COUNT(ID)
    		FROM DM_DoiTuong
    		WHERE convert(varchar(10), NgayTao, 102) 
    		= convert(varchar(10), @DateNow, 102)
    		and TheoDoi= 0
			and LoaiDoiTuong = 1 and ID not like '%00000000-0000-0000-0000-0000000000%'
    
    	SELECT @SinhNhat AS SinhNhat, @CongViec AS CongViec, @LichHen AS LichHen, @SoLoSapHetHan AS SoLoSapHetHan, @SoLoHetHan AS SoLoHetHan,
    	@XeMoiTiepNhan AS XeMoiTiepNhan, @XeXuatXuong AS XeXuatXuong, @KhachHangMoi AS KhachHangMoi;
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListDoiTuongByLoai]
    @LoaiDoiTuong [int],
    @Search [nvarchar](max),
	@ID_DonVi uniqueidentifier null
AS
BEGIN
	set nocount on;
	declare @tblSearch table (ColumnSearch nvarchar(max))
	insert into @tblSearch
	select Name from dbo.splitstringByChar(REPLACE(@Search,'%',''),' ')		

	declare @findAll int = 0

	if @ID_DonVi is null
	begin
		set @findAll = 1
	end
	else
	begin
		declare @counQLTheoCN bit = 0;
		select @counQLTheoCN = count(id) from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi and QuanLyKhachHangTheoDonVi='1'
		if @counQLTheoCN = 0
			set @findAll = 1
	end

	if @findAll = 1
		begin
			select top 20 dt.ID, TenDoiTuong as NguoiNopTien,
					MaDoiTuong as MaNguoiNop, DienThoai as SoDienThoai , 
					Email ,
					DiaChi,
					iif(dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='','00000000-0000-0000-0000-000000000000', dt.IDNhomDoiTuongs) as IDNhomDoiTuongs				
				from DM_DoiTuong dt
				where LoaiDoiTuong = @LoaiDoiTuong and TheoDoi = 0 				
				and (DienThoai like @Search or TenDoiTuong like @Search COLLATE Vietnamese_CI_AI 
				or TenDoiTuong_KhongDau like @Search
				or MaDoiTuong like @Search
				or TenDoiTuong like @Search
				or TenDoiTuong_KhongDau like @Search COLLATE Vietnamese_CI_AI 
				or TenDoiTuong_ChuCaiDau like @Search COLLATE Vietnamese_CI_AI 
				or MaDoiTuong like @Search COLLATE Vietnamese_CI_AI
				)
		end
	else
	begin
		declare @tblNhomKH table (ID uniqueidentifier)
				insert into @tblNhomKH(ID) values ('00000000-0000-0000-0000-000000000000') -- nhom macdinh
		--- find by chinhanh
		insert into @tblNhomKH(ID)
    	select *  from (
    		-- get Nhom not not exist in NhomDoiTuong_DonVi
    		select ID from DM_NhomDoiTuong nhom  
    		where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID)
    		and LoaiDoiTuong = @LoaiDoiTuong
    	union all
    	-- get Nhom at this ChiNhanh
    	select ID_NhomDoiTuong  from NhomDoiTuong_DonVi where ID_DonVi like @ID_DonVi) tbl

		select top 50*
				from
				(
				select dt.ID, TenDoiTuong as NguoiNopTien,
					MaDoiTuong as MaNguoiNop, DienThoai as SoDienThoai , 
					Email ,
					DiaChi,
					iif(dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='','00000000-0000-0000-0000-000000000000', dt.IDNhomDoiTuongs) as IDNhomDoiTuongs				
				from DM_DoiTuong dt
				where LoaiDoiTuong = @LoaiDoiTuong and TheoDoi = 0 
				and (DienThoai like @Search or TenDoiTuong like @Search COLLATE Vietnamese_CI_AI 
					or TenDoiTuong_KhongDau like @Search
					or MaDoiTuong like @Search
					or TenDoiTuong like @Search
					or TenDoiTuong_KhongDau like @Search COLLATE Vietnamese_CI_AI 
					or TenDoiTuong_ChuCaiDau like @Search COLLATE Vietnamese_CI_AI 
					or MaDoiTuong like @Search COLLATE Vietnamese_CI_AI)
				)dt
				where EXISTS(SELECT Name FROM splitstring(dt.IDNhomDoiTuongs) nhom 
				inner JOIN @tblNhomKH tblsearch ON nhom.Name = tblsearch.ID
				where nhom.Name!='')
	end
			

END");

			Sql(@"ALTER PROCEDURE [dbo].[getlistNhanVien_CaiDatChietKhau]
    @ID_DonVi [uniqueidentifier],
    @Text_NhanVien [nvarchar](max),
    @Text_NhanVien_TV [nvarchar](max),
    @TrangThai [nvarchar](max)
AS
BEGIN
    Select a.ID as ID_NhanVien,
    	a.MaNhanVien, a.TenNhanVien, a.DienThoaiDiDong, ISNULL(AnhNV.URLAnh, '') AS URLAnh, a.GioiTinh
    	From
    	(   	
			Select DISTINCT nv.ID, nv.MaNhanVien, nv.TenNhanVien,nv.TenNhanVienChuCaiDau, nv.TenNhanVienKhongDau, nv.DienThoaiDiDong, 
    		Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
    		Case when ck.ID_NhanVien is null then 0 else 1 end as CaiDat, nv.GioiTinh
    		From NS_NhanVien nv
    		inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
    		left join (select ck.ID_NhanVien
					from ChietKhauMacDinh_NhanVien ck 
					where ID_DonVi= @ID_DonVi 
					and exists (select ID from DonViQuiDoi qd where ID_DonViQuiDoi= qd.ID and Xoa='0')
					group by ck.ID_NhanVien) ck on nv.ID= ck.ID_NhanVien    
    		where (nv.MaNhanVien like @Text_NhanVien or nv.MaNhanVien like @Text_NhanVien_TV 
    		or nv.DienThoaiDiDong like @Text_NhanVien_TV or nv.TenNhanVienKhongDau like @Text_NhanVien or nv.TenNhanVienChuCaiDau like @Text_NhanVien)
    		and DaNghiViec != '1'
    		and ct.ID_DonVi = @ID_DonVi
    	)a
		left join (SELECT ID_NhanVien, URLAnh FROM NS_NhanVien_Anh WHERE SoThuTu = 1 GROUP BY ID_NhanVien, URLAnh) as AnhNV
		ON a.ID = AnhNV.ID_NhanVien
    	where a.CaiDat like @TrangThai
    	and a.TrangThai != 0
    	order by MaNhanVien
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetMaMaPhieuTiepNhan_byTemp]
    @LoaiHoaDon [int],
    @ID_DonVi [uniqueidentifier],
    @NgayLapHoaDon [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @Return float = 1
    	declare @lenMaMax int = 0
    	DECLARE @isDefault bit = (select top 1 SuDungMaChungTu from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi)-- co/khong thiet lap su dung Ma MacDinh
    	DECLARE @isSetup int = (select top 1 ID_LoaiChungTu from HT_MaChungTu where ID_LoaiChungTu = @LoaiHoaDon)-- da ton tai trong bang thiet lap chua
    
    	if @isDefault='1' and @isSetup is not null
    		begin
    			DECLARE @machinhanh varchar(15) = (select MaDonVi from DM_DonVi where ID= @ID_DonVi)
    			DECLARE @lenMaCN int = Len(@machinhanh)
    			DECLARE @isUseMaChiNhanh varchar(15) = (select SuDungMaDonVi from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon) -- co/khong su dung MaChiNhanh
    			DECLARE @kituphancach1 varchar(1) = (select KiTuNganCach1 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
    			DECLARE @kituphancach2 varchar(1) = (select KiTuNganCach2 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
    			DECLARE @kituphancach3 varchar(1) = (select KiTuNganCach3 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
    			DECLARE @dinhdangngay varchar(8) = (select NgayThangNam from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
    			DECLARE @dodaiSTT INT = (select CAST(DoDaiSTT AS INT) from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
    			DECLARE @kihieuchungtu varchar(10) = (select MaLoaiChungTu from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
    			DECLARE @lenMaKiHieu int = Len(@kihieuchungtu);
    			DECLARE @namthangngay varchar(10) = convert(varchar(10), @NgayLapHoaDon, 112)
    			DECLARE @year varchar(4) = Left(@namthangngay,4)
    			DECLARE @date varchar(4) = right(@namthangngay,2)
    			DECLARE @month varchar(4) = substring(@namthangngay,5,2)
    			DECLARE @datecompare varchar(10)='';
    			
    			if	@isUseMaChiNhanh='0'
    				begin 
    					set @machinhanh=''
    					set @lenMaCN=0
    				end
    
    			if @dinhdangngay='ddMMyyyy'
    				set @datecompare = CONCAT(@date,@month,@year)
    			else	
    				if @dinhdangngay='ddMMyy'
    					set @datecompare = CONCAT(@date,@month,right(@year,2))
    				else 
    					if @dinhdangngay='MMyyyy'
    						set @datecompare = CONCAT(@month,@year)
    					else	
    						if @dinhdangngay='MMyy'
    							set @datecompare = CONCAT(@month,right(@year,2))
    						else
    							if @dinhdangngay='yyyyMMdd'
    								set @datecompare = CONCAT(@year,@month,@date)
    							else 
    								if @dinhdangngay='yyMMdd'
    									set @datecompare = CONCAT(right(@year,2),@month,@date)
    								else	
    									if @dinhdangngay='yyyyMM'
    										set @datecompare = CONCAT(@year,@month)
    									else	
    										if @dinhdangngay='yyMM'
    											set @datecompare = CONCAT(right(@year,2),@month)
    										else 
    											if @dinhdangngay='yyyy'
    												set @datecompare = @year							
    
    			DECLARE @sMaFull varchar(50) = concat(@machinhanh,@kituphancach1,@kihieuchungtu,@kituphancach2, @datecompare, @kituphancach3)	
    
    			declare @sCompare varchar(30) = @sMaFull
    			if @sMaFull= concat(@kihieuchungtu,'_') set @sCompare = concat(@kihieuchungtu,'[_]') -- like %_% không nhận kí tự _ nên phải [_] theo quy tắc của sql

				if @sMaFull='PTN'
					select @Return = MAX(CAST(dbo.udf_GetNumeric(RIGHT(MaPhieuTiepNhan,LEN(MaPhieuTiepNhan)- len(@sMaFull)))AS float))
    				from Gara_PhieuTiepNhan where SUBSTRING(MaPhieuTiepNhan, 1, len(@sMaFull)) = @sMaFull and CHARINDEX('_', MaPhieuTiepNhan)=0
				else
					begin
						 -- lay STTmax
						set @Return = (select max(maxSTT)
										from
										(
										select MaPhieuTiepNhan,
											CAST(dbo.udf_GetNumeric(RIGHT(MaPhieuTiepNhan, LEN(MaPhieuTiepNhan) -LEN (@sMaFull))) AS float) as maxSTT -- lay chuoi so ben phai
										from Gara_PhieuTiepNhan 
										where MaPhieuTiepNhan like @sCompare +'%' 
										) a 
									)	
					end
        			
    			-- lay chuoi 000
    			declare @stt int =0;
    			declare @strstt varchar (10) ='0'
    			while @stt < @dodaiSTT- 1
    				begin
    					set @strstt= CONCAT('0',@strstt)
    					SET @stt = @stt +1;
    				end 
    			declare @lenSst int = len(@strstt)
    			if	@Return is null 
    					select CONCAT(@sMaFull,left(@strstt,@lenSst-1),1) as MaxCode-- left(@strstt,@lenSst-1): bỏ bớt 1 số 0			
    			else 
    				begin
    					set @Return = @Return + 1
    					set @lenMaMax =  len(@Return)
						declare @madai nvarchar(max)= CONCAT(@sMaFull, CONVERT(numeric(22,0), @Return))
    					select 
    						case when @lenMaMax = 1 then CONCAT(@sMaFull,left(@strstt,@lenSst-1),@Return)
    							when @lenMaMax = 2 then case when @lenSst - 2 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-2), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 3 then case when @lenSst - 3 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-3), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 4 then case when @lenSst - 4 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-4), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 5 then case when @lenSst - 5 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-5), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 6 then case when @lenSst - 6 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-6), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 7 then case when @lenSst - 7 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-7), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 8 then case when @lenSst - 8 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-8), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 9 then case when @lenSst - 9 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-9), @Return) else CONCAT(@sMaFull, @Return) end
    							when @lenMaMax = 10 then case when @lenSst - 10 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return) else CONCAT(@sMaFull, @Return) end
    						else 
							case when  @lenMaMax > 10
								 then iif(@lenSst - 10 > -1, CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return),  @madai)
								 else '' end
						end as MaxCode		
    				end 
    		end
    	else
    		begin
    			declare @machungtu varchar(10) = (select top 1 MaLoaiChungTu from DM_LoaiChungTu where ID= @LoaiHoaDon)
    			declare @lenMaChungTu int= LEN(@machungtu)
    
    			select @Return = MAX(CAST(dbo.udf_GetNumeric(RIGHT(MaPhieuTiepNhan,LEN(MaPhieuTiepNhan)- @lenMaChungTu))AS float))
    			from Gara_PhieuTiepNhan where SUBSTRING(MaPhieuTiepNhan, 1, len(@machungtu)) = @machungtu and CHARINDEX('O',MaPhieuTiepNhan) = 0 -- not HDO, GDVO, THO, DHO
    			
    			-- do dai STT (toida = 10)
    			if	@Return is null 
    					select
    						case when @lenMaChungTu = 2 then CONCAT(@machungtu, '00000000',1)
    							when @lenMaChungTu = 3 then CONCAT(@machungtu, '0000000',1)
    							when @lenMaChungTu = 4 then CONCAT(@machungtu, '000000',1)
    							when @lenMaChungTu = 5 then CONCAT(@machungtu, '00000',1)
    						else CONCAT(@machungtu,'000000',1)
    						end as MaxCode
    			else 
    				begin
    					set @Return = @Return + 1
    					set @lenMaMax = len(@Return)
    					select 
    						case when @lenMaMax = 1 then CONCAT(@machungtu,'000000000',@Return)
    							when @lenMaMax = 2 then CONCAT(@machungtu,'00000000',@Return)
    							when @lenMaMax = 3 then CONCAT(@machungtu,'0000000',@Return)
    							when @lenMaMax = 4 then CONCAT(@machungtu,'000000',@Return)
    							when @lenMaMax = 5 then CONCAT(@machungtu,'00000',@Return)
    							when @lenMaMax = 6 then CONCAT(@machungtu,'0000',@Return)
    							when @lenMaMax = 7 then CONCAT(@machungtu,'000',@Return)
    							when @lenMaMax = 8 then CONCAT(@machungtu,'00',@Return)
    							when @lenMaMax = 9 then CONCAT(@machungtu,'0',@Return)								
    						else CONCAT(@machungtu,CAST(@Return  as decimal(22,0))) end as MaxCode
    				end 
    		end
END
");

			Sql(@"ALTER PROCEDURE [dbo].[GetNhatKyGiaoDich_ofCus]
    @IDChiNhanhs [nvarchar](max),
    @IDCustomers [nvarchar](max),
    @IDCars [nvarchar](max),
    @LoaiHoaDons [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	declare @sql nvarchar(max) ='', @where nvarchar(max), @paramDefined nvarchar(max)
    	declare @sqlSoQuy nvarchar(max), @where2 nvarchar(max), @whereHDTra nvarchar(max), @sqlHDTra nvarchar(max)
    	declare @tblDefined nvarchar(max)= N' declare @tblChiNhanh table(ID uniqueidentifier)
    								declare @tblCus table(ID uniqueidentifier)
    								declare @tblCar table(ID uniqueidentifier)'
    
    	set @where = N' where 1 = 1 '
    	set @where2 = N' where 1 = 1  '
		set @whereHDTra = N' where 1 = 1  '
    
    	if isnull(@CurrentPage,'') =''
    		set @CurrentPage = 0
    	if isnull(@PageSize,'') =''
    		set @PageSize = 20
    
    	if isnull(@LoaiHoaDons,'') =''
    		set @LoaiHoaDons = '1,19,15'
    	if isnull(@LoaiHoaDons,'') !=''
    	begin
    		set @where = CONCAT(@where , ' and hd.LoaiHoaDon in (select name from dbo.splitstring(@LoaiHoaDons_In))')		
    	end
    	if isnull(@IDChiNhanhs,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ;')			
    		end
    	if isnull(@IDCustomers,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblCus select name from dbo.splitstring(@IDCustomers_In) ;')
    			set @where2 = CONCAT(@where2 , ' and exists (select ID from @tblCus cus where ID_DoiTuong = cus.ID)')
				set @whereHDTra = CONCAT(@whereHDTra , ' and exists (select ID from @tblCus cus where hdt.ID_DoiTuong = cus.ID)')
    		end
    	
    	if isnull(@IDCars,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblCar car where hd.ID_Xe = car.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblCar select name from dbo.splitstring(@IDCars_In) ;')
    		end
    
    		set @sqlSoQuy = CONCAT(N'
    				select qct.ID_HoaDonLienQuan,
						qct.ID_DoiTuong,
    					SUM(qct.TienThu) as TongTienThu,
    					max(qhd.MaHoaDon) as MaPhieuThu
    				into #tmpThuChi
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    				', @where2, 'and (qhd.TrangThai = 1 or qhd.TrangThai is null) 
    			group by qct.ID_HoaDonLienQuan,qct.ID_DoiTuong; ')
    
    		set @sqlHDTra = CONCAT(N'
    			select hdt.ID_HoaDon, 
    			SUM(hdt.TongTienHang) as TongTienHang,
    			SUM(hdt.TongGiamGia) as TongGiamGia ,
    			SUM(hdt.KhuyeMai_GiamGia) as KhuyeMai_GiamGia, 
    			SUM(hdt.PhaiThanhToan) as PhaiThanhToan,
    				SUM(ISNULL(thuchi.TongTienThu,0)) as TongTienThu
    	from BH_HoaDon hdt	
    		left join #tmpThuChi thuchi on thuchi.ID_HoaDonLienQuan = hdt.ID
    		', @whereHDTra, ' and hdt.LoaiHoaDon= 6 and hdt.ChoThanhToan = 0 group by hdt.ID_HoaDon ' )
    
    
    		set @sql = CONCAT(@tblDefined, @sql, @sqlSoQuy ,  '
    		with data_cte
    		as (
    				select hd.MaHoaDon, 
    					hd.ID,
    					hd.ID_Xe, 
    					xe.BienSo,
    				hd.NgayLapHoaDon,
    					hd.PhaiThanhToan as TongMua,
    					HDTra.PhaiThanhToan as TongTra,
    					ISNULL(thuchi.MaPhieuThu,'''') as MaPhieuThu,
    					hd.PhaiThanhToan - ISNULL(thuchi.TongTienThu,0) - (ISNULL(HDTra.PhaiThanhToan,0) - ISNULL(HDTra.TongTienThu,0)) as ConNo,
    					-- PhaiTT of GDV = MuaGoi - TraGoi
    					hd.PhaiThanhToan - ISNULL(HDTra.PhaiThanhToan,0) as PhaiThanhToan,
    					-- chỉ trừ TongTienThu của HDTra nếu khi Trả hàng, mình chi tiền cho khách
    					ISNULL(thuchi.TongTienThu,0) - ISNULL(HDTra.TongTienThu,0) as DaThanhToan,  
    					hd.DienGiai as GhiChu   		
    	from BH_HoaDon hd
    		left join Gara_DanhMucXe xe on hd.ID_Xe= xe.ID
    		left join #tmpThuChi thuchi on thuchi.ID_HoaDonLienQuan = hd.ID and hd.ID_DoiTuong = thuchi.ID_DoiTuong
    		left join ( ', @sqlHDTra , ') HDTra on hd.ID= HDTra.ID_HoaDon ',
    		@where, 'and hd.ChoThanhToan = 0 ),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				sum(TongMua) as SumTongMua,
    				sum(TongTra) as SumTongTra,							
    				sum(PhaiThanhToan) as SumPhaiThanhToan,
    				sum(DaThanhToan) as SumDaThanhToan,
    				sum(ConNo) as SumConNo
    			from data_cte
    		)
    	select dt.*,			
    		cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage_In * @PageSize_In) ROWS
    		FETCH NEXT @PageSize_In ROWS ONLY 
    		')
    
    	set @paramDefined =N'
    			@IDChiNhanhs_In nvarchar(max) = null,
    			@IDCustomers_In nvarchar(max) = null,
    			@IDCars_In nvarchar(max) = null,
    			@LoaiHoaDons_In nvarchar(max) = null,
    			@CurrentPage_In int =null,
    			@PageSize_In int =null'
    
    			print @sql
    
    
    		exec sp_executesql @sql, 
    		@paramDefined,
    		@IDChiNhanhs_In = @IDChiNhanhs,
    		@IDCustomers_In = @IDCustomers,
    		@IDCars_In = @IDCars,
    		@LoaiHoaDons_In = @LoaiHoaDons,
    		@CurrentPage_In = @CurrentPage,
    		@PageSize_In = @PageSize
END");

			Sql(@"ALTER PROCEDURE [dbo].[TongQuanDoanhThuCongNo]
    @IdChiNhanhs [nvarchar](max),
    @DateFrom [datetime],
    @DateTo [datetime]
AS
BEGIN
    SET NOCOUNT ON;
	--	DECLARE @IdChiNhanhs [nvarchar](max),
 --   @DateFrom [datetime],
 --   @DateTo [datetime];
	--SET @IdChiNhanhs = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de';
	--SET @DateFrom = '2021-10-20'
	--SET @DateTo = '2021-10-21'

    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    -- Insert statements for procedure here
    	DECLARE @tblHoaDon TABLE(ID UNIQUEIDENTIFIER, LoaiHoaDon INT, TongThanhToan FLOAT, PhaiThanhToan FLOAT, TongTienThue FLOAT, IDHoaDon UNIQUEIDENTIFIER);
    	INSERT INTO @tblHoaDon
    	SELECT ID, LoaiHoaDon, TongThanhToan, PhaiThanhToan, TongTienThue + TongTienThueBaoHiem, hd.ID_HoaDon FROM BH_HoaDon hd
    	INNER JOIN @tblDonVi dv ON hd.ID_DonVi = dv.ID_DonVi
    	WHERE hd.LoaiHoaDon IN (1, 4, 6, 7, 19, 25)
    	AND hd.ChoThanhToan = 0
    	AND hd.NgayLapHoaDon BETWEEN @DateFrom AND @DateTo;

		--SELECT * FROM @tblHoaDon
    
    	DECLARE @tblSoQuy TABLE(ID UNIQUEIDENTIFIER, LoaiHoaDon INT, TienMat FLOAT, TienGui FLOAT, TienThu FLOAT, IDHoaDonLienQuan UNIQUEIDENTIFIER);
    	INSERT INTO @tblSoQuy
    	SELECT qhd.ID, qhd.LoaiHoaDon, qhdct.TienMat, qhdct.TienGui, qhdct.TienThu, qhdct.ID_HoaDonLienQuan FROM Quy_HoaDon qhd
    	INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    	INNER JOIN @tblDonVi dv ON qhd.ID_DonVi = dv.ID_DonVi
    	WHERE qhd.NgayLapHoaDon BETWEEN @DateFrom AND @DateTo
		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and (qhd.PhieuDieuChinhCongNo = 0 or qhd.PhieuDieuChinhCongNo is null)
    	AND qhd.LoaiHoaDon IN (11, 12);
		--SELECT * FROM @tblSoQuy
    
    	DECLARE @DoanhThuSuaChua FLOAT,
    	@DoanhThuBanHang FLOAT,
    	@PhaiTraKhachHang FLOAT,
    	@PhaiTraNhaCungCap FLOAT,
    	@PhaiThuNhaCungCap FLOAT,
    	@Thu_TienMat FLOAT,
    	@Thu_NganHang FLOAT,
    	@Chi_TienMat FLOAT,
    	@Chi_NganHang FLOAT,
    	@HoaDonDaThu FLOAT,
    	@HoaDonDaChi FLOAT,
		@ThueSuaChua FLOAT,
		@ThueBanHang FLOAT;
    	SELECT @DoanhThuSuaChua =SUM(CASE WHEN hd.LoaiHoaDon = 25 THEN hd.TongThanhToan ELSE 0 END), 
    		@PhaiTraKhachHang = SUM(CASE WHEN hd.LoaiHoaDon = 6 THEN hd.TongThanhToan ELSE 0 END),
    		@DoanhThuBanHang = SUM(CASE WHEN hd.LoaiHoaDon IN (1, 19)
    		THEN
    			hd.PhaiThanhToan
    			ELSE 0
    		END),
    	@PhaiTraNhaCungCap = SUM(CASE WHEN hd.LoaiHoaDon = 4 THEN hd.PhaiThanhToan ELSE 0 END),
    	@PhaiThuNhaCungCap = SUM(CASE WHEN hd.LoaiHoaDon = 7 THEN hd.PhaiThanhToan ELSE 0 END),
		@ThueSuaChua = SUM(CASE WHEN hd.LoaiHoaDon = 25 THEN hd.TongTienThue ELSE 0 END),
		@ThueBanHang = SUM(CASE WHEN hd.LoaiHoaDon IN (1, 19) THEN hd.TongTienThue ELSE 0 END)
    	FROM @tblHoaDon hd;
    
    	SELECT @Thu_TienMat = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.TienMat ELSE 0 END), 
    	@Thu_NganHang = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.TienGui ELSE 0 END),
    	@Chi_TienMat = SUM(CASE WHEN sq.LoaiHoaDon = 12 THEN sq.TienMat ELSE 0 END),
    	@Chi_NganHang = SUM(CASE WHEN sq.LoaiHoaDon = 12 THEN sq.TienGui ELSE 0 END)
    	FROM @tblSoQuy sq;
    
    	SELECT @HoaDonDaThu = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.ThanhToan ELSE 0 END),  
    	@HoaDonDaChi = SUM(CASE WHEN sq.LoaiHoaDon = 12 AND hd.LoaiHoaDon != 7 THEN sq.ThanhToan ELSE 0 END) FROM @tblHoaDon hd
    	INNER JOIN (SELECT IDHoaDonLienQuan, SUM(TienThu) AS ThanhToan, LoaiHoaDon FROM @tblSoQuy GROUP BY IDHoaDonLienQuan, LoaiHoaDon) sq
    	ON hd.ID = sq.IDHoaDonLienQuan OR (hd.IDHoaDon = sq.IDHoaDonLienQuan AND hd.LoaiHoaDon != 6)
    
    	SET @DoanhThuSuaChua = ISNULL(@DoanhThuSuaChua, 0);
    	SET @DoanhThuBanHang = ISNULL(@DoanhThuBanHang, 0);
    	SET @PhaiTraKhachHang = ISNULL(@PhaiTraKhachHang, 0);
    	SET @PhaiTraNhaCungCap = ISNULL(@PhaiTraNhaCungCap, 0);
    	SET @PhaiThuNhaCungCap = ISNULL(@PhaiThuNhaCungCap, 0);
    	SET @Thu_TienMat = ISNULL(@Thu_TienMat, 0);
    	SET @Thu_NganHang = ISNULL(@Thu_NganHang, 0);
    	SET @Chi_TienMat = ISNULL(@Chi_TienMat, 0);
    	SET @Chi_NganHang = ISNULL(@Chi_NganHang, 0);
    	SET @HoaDonDaThu = ISNULL(@HoaDonDaThu, 0);
    	SET @HoaDonDaChi = ISNULL(@HoaDonDaChi, 0);
		SET @ThueBanHang = ISNULL(@ThueBanHang, 0);
		SET @ThueSuaChua = ISNULL(@ThueSuaChua, 0)
    
    	SELECT @DoanhThuSuaChua - @ThueSuaChua AS DoanhThuSuaChua, @DoanhThuBanHang - @ThueBanHang AS DoanhThuBanHang,
    	@DoanhThuBanHang + @DoanhThuSuaChua - @ThueBanHang - @ThueSuaChua AS TongDoanhThu, @DoanhThuBanHang + @DoanhThuSuaChua + @PhaiThuNhaCungCap - @HoaDonDaThu AS CongNoPhaiThu,
    	@PhaiTraNhaCungCap + @PhaiTraKhachHang - @HoaDonDaChi AS CongNoPhaiTra, 
    	-(@DoanhThuBanHang + @DoanhThuSuaChua + @PhaiThuNhaCungCap - @HoaDonDaThu) + (@PhaiTraNhaCungCap + @PhaiTraKhachHang - @HoaDonDaChi) AS TongCongNo,
    	@Thu_TienMat AS ThuTienMat, @Thu_NganHang AS ThuNganHang, @Thu_TienMat + @Thu_NganHang AS TongTienThu,
    	@Chi_TienMat AS ChiTienMat, @Chi_NganHang AS ChiNganHang, @Chi_TienMat + @Chi_NganHang AS TongTienChi;
END");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Gara_DanhMucXe", "BienSo", c => c.String(maxLength: 10));
            DropStoredProcedure("[dbo].[UpdateTonKho_multipleDVT]");
        }
    }
}
