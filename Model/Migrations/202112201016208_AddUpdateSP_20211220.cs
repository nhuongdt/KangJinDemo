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

        }
        
        public override void Down()
        {
            AlterColumn("dbo.Gara_DanhMucXe", "BienSo", c => c.String(maxLength: 10));
            DropStoredProcedure("[dbo].[UpdateTonKho_multipleDVT]");
        }
    }
}
