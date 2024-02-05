namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20220728 : DbMigration
    {
        public override void Up()
        {
			Sql(@"ALTER FUNCTION [dbo].[DiscountSale_NVienDichVu]
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
	select  2 as LoaiNhanVienApDung, tblNVienDV.ID_NhanVien, tblNVienDV.DoanhThu, 
	case when tblNVienDV.LaPhanTram = 1 then DoanhThu * GiaTriChietKhau / 100 else  GiaTriChietKhau end as HoaHongDoanhThu,
	tblNVienDV.ID
from
(
	select b.ID_NhanVien, b.DoanhThu, ckct.GiaTriChietKhau,ckct.LaPhanTram,ckct.ID,
	ROW_NUMBER() over (PARTITION  by b.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
	from
	(
			select  a.ID_NhanVien, sum(a.DoanhThu) - sum(a.GiaTriTra) as DoanhThu, a.ID_ChietKhauDoanhThu
			from
			(

				select 
					ckdtnv.ID_NhanVien , 
					nvth.ID_ChiTietHoaDon, 
					hd.MaHoaDon,   							
					---- HeSo * hoahong truoc/sau CK - phiDV
					iif(nvth.HeSo is null,1,nvth.HeSo) * (case when iif(nvth.TinhHoaHongTruocCK is null,0,nvth.TinhHoaHongTruocCK) = 1 
							then cthd.SoLuong * cthd.DonGia
							else cthd.SoLuong * (cthd.DonGia - cthd.TienChietKhau)
							end )
					- iif(hd.LoaiHoaDon=19,0, case when hh.ChiPhiTinhTheoPT =1 then cthd.SoLuong * cthd.DonGia * hh.ChiPhiThucHien/100
							else hh.ChiPhiThucHien * cthd.SoLuong end) as DoanhThu,
    				0 as GiaTriTra,
					ckdtnv.ID_ChietKhauDoanhThu, 
					ckdt.ApDungTuNgay,
					ckdt.ApDungDenNgay
				from ChietKhauDoanhThu ckdt
				join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
				join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
				join BH_HoaDon_ChiTiet cthd on nvth.ID_ChiTietHoaDon = cthd.ID 
				join DonViQuiDoi qd on cthd.ID_DonViQuiDoi = qd.ID
				join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
				join BH_HoaDon hd on cthd.ID_HoaDon= hd.ID
				and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
									and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
				where hd.ChoThanhToan= 0 
				and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)
				and hd.LoaiHoaDon in (1,19,22,25,32,2)
				and ckdt.LoaiNhanVienApDung= 2
				and hd.NgayLapHoaDon >= @FromDate  and hd.NgayLapHoaDon < @ToDate
				and cthd.ChatLieu!=4
				and nvth.ID_NhanVien like @IDNhanVien
				and ckdt.TrangThai= 1

				union all


				--- trahang
				select  ckdtnv.ID_NhanVien ,
					nvth.ID_ChiTietHoaDon,
					hdt.MaHoaDon,    								
    				0 as DoanhThu, 									
    				cthd.ThanhTien  as GiaTriTra,
    				ckdtnv.ID_ChietKhauDoanhThu,
					ckdt.ApDungTuNgay, 
					ckdt.ApDungDenNgay
    			from ChietKhauDoanhThu ckdt
    			join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
				join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
				join BH_HoaDon_ChiTiet cthd on nvth.ID_ChiTietHoaDon = cthd.ID 
    			join BH_HoaDon hdt on cthd.ID_HoaDon = hdt.ID 
				and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    			left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    			left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    			where 
    			exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hdt.ID_DonVi= dv.Name)
    			and ckdt.LoaiNhanVienApDung=2
    			and hdt.ChoThanhToan = '0' and hdt.LoaiHoaDon = 6
    			and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
				and (qhd.TrangThai is null or qhd.TrangThai != 0)
				and nvth.ID_NhanVien like @IDNhanVien
				and ckdt.TrangThai= 1
			) a group by a.ID_NhanVien, a.ID_ChietKhauDoanhThu
	) b 
join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
and (b.DoanhThu >= ckct.DoanhThuTu) 								
)tblNVienDV where tblNVienDV.Rn= 1
)
");

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

			Sql(@"ALTER FUNCTION [dbo].[TinhSoDuKHTheoThoiGian]
(
	@ID_DoiTuong [uniqueidentifier],
	@Time [datetime]
)
RETURNS FLOAT
AS
BEGIN
		DECLARE @SoDu AS FLOAT;		
		DECLARE @TongNap AS FLOAT, @TongThuNapThe float;
		DECLARE @SuDungThe AS FLOAT;
		DECLARE @HoanTraTienThe AS FLOAT;
		DECLARE @TongDieuChinh AS FLOAT;
		DECLARE @TraLaiSoDu AS FLOAT;

		-- thuc thu thegiatri
    		SELECT @TongThuNapThe = sum(qct.TienThu)    		
    		from Quy_HoaDon_ChiTiet qct
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    		join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    		where hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and qhd.NgayLapHoaDon < @Time and qct.ID_DoiTuong like @ID_DoiTuong
    		and (qhd.PhieuDieuChinhCongNo= 0 or qhd.PhieuDieuChinhCongNo  is  null)
			and (qhd.TrangThai= 1 or qhd.TrangThai is  null)

		SELECT @TongNap = SUM(TongTienHang) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @TongDieuChinh = SUM(TongChiPhi) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 23 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @SuDungThe = SUM(qhdct.TienThu) FROM Quy_HoaDon_ChiTiet qhdct
		JOIN Quy_HoaDon qhd
		ON qhdct.ID_HoaDon = qhd.ID
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 11 and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and qhdct.HinhThucThanhToan=4

		SELECT @HoanTraTienThe = SUM(qhdct.TienThu) FROM Quy_HoaDon_ChiTiet qhdct
		JOIN Quy_HoaDon qhd ON qhdct.ID_HoaDon = qhd.ID		
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 12 and (qhd.TrangThai = 1 or qhd.TrangThai is null)
			and qhdct.HinhThucThanhToan=4

		SELECT @TraLaiSoDu = SUM(hd.TongTienHang) FROM BH_HoaDon hd			
		WHERE hd.ID_DoiTuong = @ID_DoiTuong AND hd.NgayLapHoaDon < @Time and hd.LoaiHoaDon = 32 and hd.ChoThanhToan= 0		

		SET @SoDu = ISNULL(@TongThuNapThe, 0) +  ISNULL(@TongDieuChinh, 0)  - ISNULL(@SuDungThe, 0) + ISNULL(@HoanTraTienThe,0) - ISNULL(@TraLaiSoDu, 0);
	RETURN @SoDu
END");

			CreateStoredProcedure(name: "[dbo].[GetGiaVonTieuChuan_byTime]", parametersAction: p => new
			{
				IDChiNhanhs = p.Guid(),
				ToDate = p.DateTime(),
				IDDonViQuyDois = p.String(),
				IDLoHangs = p.String()
			}, body: @"SET NOCOUNT ON;

		declare @tblChiNhanh table (ID_DonVi uniqueidentifier)		    
		insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs) 
    	

	if @IDDonViQuyDois is null or @IDDonViQuyDois =''
	begin
		select 
			tbl.ID_DonVi,
			tbl.ID_DonViQuiDoi,
			tbl.ID_LoHang, 
			tbl.GiaVonTieuChuan			
		from
			(
			select 
				gvGanNhat.ID_DonVi, 
				gvGanNhat.ID_DonViQuiDoi, 
				gvGanNhat.ID_LoHang, 
				gvGanNhat.GiaVonTieuChuan,
				ROW_NUMBER() over (partition by gvGanNhat.ID_DonVi, gvGanNhat.ID_DonViQuiDoi, gvGanNhat.ID_LoHang order by gvGanNhat.NgayLapHoaDon) as RN
			from
			(
				select ct.ID_DonViQuiDoi, ct.ID_LoHang, 
					ct.ThanhTien as GiaVonTieuChuan,
					hd.ID_DonVi,
					hd.NgayLapHoaDon				
				from BH_HoaDon_ChiTiet ct 
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
				where hd.ChoThanhToan=0
				and  hd.ID_DonVi in (select ID_DonVi from @tblChiNhanh)
				and hd.LoaiHoaDon= 16
				and hd.NgayLapHoaDon < @ToDate				
			 ) gvGanNhat   
		 ) tbl where tbl.RN= 1
	end
	else
	begin
		declare @tblIDQuiDoi table (ID_DonViQuyDoi uniqueidentifier)
    	declare @tblIDLoHang table (ID_LoHang uniqueidentifier)

		insert into @tblIDQuiDoi
    	select Name from dbo.splitstring(@IDDonViQuyDois) 
    	insert into @tblIDLoHang
    	select Name from dbo.splitstring(@IDLoHangs) where Name not like '%null%' and Name !=''

		select 
			tbl.ID_DonVi,
			tbl.ID_DonViQuiDoi,
			tbl.ID_LoHang, 
			tbl.GiaVonTieuChuan			
		from
			(
			select 
				gvGanNhat.ID_DonVi, 
				gvGanNhat.ID_DonViQuiDoi, 
				gvGanNhat.ID_LoHang, 
				gvGanNhat.GiaVonTieuChuan,
				ROW_NUMBER() over (partition by gvGanNhat.ID_DonVi, gvGanNhat.ID_DonViQuiDoi, gvGanNhat.ID_LoHang order by gvGanNhat.NgayLapHoaDon) as RN
			from
			(
				select ct.ID_DonViQuiDoi, ct.ID_LoHang, 
					ct.ThanhTien as GiaVonTieuChuan,
					hd.ID_DonVi,
					hd.NgayLapHoaDon
				from @tblIDQuiDoi qd
				join BH_HoaDon_ChiTiet ct on qd.ID_DonViQuyDoi = ct.ID_DonViQuiDoi
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
				where hd.ChoThanhToan=0
				and  hd.ID_DonVi in (select ID_DonVi from @tblChiNhanh)
				and hd.LoaiHoaDon= 16
				and hd.NgayLapHoaDon < @ToDate
				and exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= ct.ID_LoHang Or (lo2.ID_LoHang is null and ct.ID_LoHang is null)) 
			 ) gvGanNhat   
		 ) tbl where tbl.RN= 1
	end");

			CreateStoredProcedure(name: "[dbo].[GetListHoaDon_ChuaPhanBoCK]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				ID_NhanVienLogin = p.String(),
				TextSearch = p.String(),
				LoaiChungTus = p.String(),
				DateFrom = p.String(),
				DateTo = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
    	set @DateTo = dateadd(day,1, @DateTo) 

		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');

    
		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)

    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);
		
    
    	declare @tblHoaDon table (ID uniqueidentifier, ID_DonVi uniqueidentifier, LoaiHoaDon int,
			MaHoaDon  nvarchar(max), NgayLapHoaDon datetime,
			IDSoQuy uniqueidentifier, MaPhieuThu nvarchar(max), NgayLapPhieuThu datetime,
			MaDoiTuong nvarchar(max), TenDoiTuong nvarchar(max), DienThoai nvarchar(max),			
    		DoanhThu float, ThucThu float)
		insert into @tblHoaDon
			select hd.ID, 
				hd.ID_DonVi,
				hd.LoaiHoaDon,
				hd.MaHoaDon,
				hd.NgayLapHoaDon, 			
				sq.ID as IDSoQuy,
				sq.MaHoaDon as MaPhieuThu,
				sq.NgayLapHoaDon as NgayLapPhieuThu,				
				dt.MaDoiTuong,
				dt.TenDoiTuong,
				dt.DienThoai,	
				case hd.LoaiHoaDon
					when 6 then - hd.PhaiThanhToan
					when 32 then - hd.PhaiThanhToan
				else hd.PhaiThanhToan end as PhaiThanhToan,				
				isnull(ThucThu,0) as ThucThu
			from BH_HoaDon hd
			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
			left join (
				select qct.ID_HoaDonLienQuan, 
						qhd.ID,
						qhd.MaHoaDon,
						 qhd.NgayLapHoaDon,
					SUM(iif(qhd.LoaiHoaDon=11, qct.TienThu,- qct.TienThu)) as ThucThu
    			from Quy_HoaDon_ChiTiet qct
    			join  Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
				where (qhd.TrangThai is null or qhd.TrangThai = '1')
				and qhd.ID_DonVi in (select ID from @tblChiNhanh)				
				and (qct.HinhThucThanhToan is null or qct.HinhThucThanhToan != 4)
				group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID,qhd.MaHoaDon
			) sq on hd.ID= sq.ID_HoaDonLienQuan
			where hd.ChoThanhToan='0'
			and hd.NgayLapHoaDon between @DateFrom and @DateTo		
			and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
			and hd.LoaiHoaDon in (select LoaiChungTu from @tblChungTu)
			and not exists
				(select ID_HoaDon 
				from BH_NhanVienThucHien th 
				where hd.ID= th.ID_HoaDon and th.TienChietKhau > 0)
			and
    				((select count(Name) from @tblSearchString b where     			
    				dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or dt.DienThoai like '%'+b.Name+'%'			
					or hd.MaHoaDon like '%'+b.Name+'%'
    				)=@count or @count=0)	
    	
		declare @tongDoanhThu float
		select @tongDoanhThu = sum(hd.DoanhThu)
		from (select distinct hd.ID,  hd.DoanhThu
		from @tblHoaDon hd) hd		
    	
    			
    		;with data_cte
    		as(
    		select * from @tblHoaDon
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(ThucThu) as TongThucThu,
					@tongDoanhThu as TongDoanhThu
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[SearchHangHoa_withGiaVonTieuChuan]", parametersAction: p => new
			{
				ID_DonVi = p.Guid(),
				TextSearch = p.String(),
				DateTo = p.DateTime(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
  
	declare @txtSeachUnsign nvarchar(max) = (select dbo.FUNC_ConvertStringToUnsign(@TextSearch));    
	set @TextSearch = CONCAT('%',@TextSearch,'%')
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString)


	select 
		hh.ID,
		qd.ID as ID_DonViQuiDoi,
		lo.ID as ID_LoHang,
		qd.MaHangHoa,
		hh.TenHangHoa, 
		isnull(qd.TenDonViTinh,'') as TenDonViTinh, 
		qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
		hh.QuanLyTheoLoHang,
		hh.LaHangHoa,
		lo.MaLoHang,
		lo.NgaySanXuat,
		lo.NgayHetHan,
		isnull(tk.TonKho,0) as TonKho, --- tonkho hientai
		isnull(gv.GiaVon,0) as GiaVon -- giavon hientai
	from DonViQuiDoi qd 
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	left join DM_LoHang lo on hh.ID= lo.ID_HangHoa	
	left join DM_HangHoa_TonKho tk on qd.ID= tk.ID_DonViQuyDoi and (lo.ID= tk.ID_LoHang or hh.QuanLyTheoLoHang = 0) and tk.ID_DonVi= @ID_DonVi
	left join DM_GiaVon gv on qd.ID= gv.ID_DonViQuiDoi and (lo.ID= gv.ID_LoHang or hh.QuanLyTheoLoHang = 0) and gv.ID_DonVi= @ID_DonVi
	where hh.LaHangHoa=1
	and (select count(Name) from @tblSearchString b where     			
    					hh.TenHangHoa like '%'+b.Name+'%'
    					or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    					or qd.MaHangHoa like '%'+b.Name+'%'		
    					or lo.MaLoHang like '%'+b.Name+'%'		
    					)=@count or @count=0");

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
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, '31'

	---- get cthd da xuly
	select ctxl.ID_ChiTietGoiDV,
		sum(ctxl.SoLuong) as SoLuongNhan,
		sum(ctxl.ThanhTien) as GiaTriNhan
	into #tblXuLy
	from BH_HoaDon_ChiTiet ctxl
	join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
	where hdxl.LoaiHoaDon in (4)
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
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, '31'

	---- get cthd da xuly
	select ctxl.ID_ChiTietGoiDV,
		sum(ctxl.SoLuong) as SoLuongNhan,
		sum(ctxl.ThanhTien) as GiaTriNhan
	into #tblXuLy
	from BH_HoaDon_ChiTiet ctxl
	join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
	where hdxl.LoaiHoaDon in (4)
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
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, '31'

	---- get cthd da xuly
	select ctxl.ID_ChiTietGoiDV,
		sum(ctxl.SoLuong) as SoLuongNhan,sum(ctxl.ThanhTien) as GiaTriNhan
	into #tblXuLy
	from BH_HoaDon_ChiTiet ctxl
	join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
	where hdxl.LoaiHoaDon in (4)
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

			Sql(@"ALTER PROCEDURE [dbo].[GetAll_DiscountSale]
    @IDChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TextSearch [nvarchar](max),
    @Status_DoanhThu [nvarchar](4),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	set @ToDate = dateadd(day,1, @ToDate) 
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');

		DECLARE @tblDepartment TABLE (ID_PhongBan uniqueidentifier)
		if @DepartmentIDs =''
			insert into @tblDepartment
			select distinct ID_PhongBan from NS_QuaTrinhCongTac pb
		else
			insert into @tblDepartment
			select * from splitstring(@DepartmentIDs)

	----- get nv thuoc PB
		declare @tblNhanVien table (ID uniqueidentifier)
		insert into @tblNhanVien
		select nv.ID
		from @tblNhanVienAll nv
		join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
		where exists (select ID_PhongBan from @tblDepartment pb where pb.ID_PhongBan= ct.ID_PhongBan) 
    	
    	declare @tblSale_NVLapHD table(LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float,IDChiTietCK uniqueidentifier)
    	insert into @tblSale_NVLapHD
    	select * from dbo.DiscountSale_NVLapHoaDon(@IDChiNhanhs, @FromDate, @ToDate,'%%')
    
    	declare @tblSale_NBanHang table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float,IDChiTietCK uniqueidentifier)
    	insert into @tblSale_NBanHang
    	select * from dbo.DiscountSale_NVBanHang (@IDChiNhanhs,@FromDate,@ToDate,'%%')
    
    	declare @tblSale_NVDichVu table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, HoaHongDoanhThu float,IDChiTietCK uniqueidentifier)
    	insert into @tblSale_NVDichVu
    	select * from dbo.DiscountSale_NVienDichVu (@IDChiNhanhs,@FromDate,@ToDate,'%%');
    
    	with data_cte
    	as (
    	select *
    	from
    	(
    	select tblOut.ID_NhanVien,nv.MaNhanVien, nv.TenNhanVien,
    		tblOut.DoanhThu, tblOut.ThucThu, 
    		tblOut.HoaHongDoanhThu,tblOut.HoaHongThucThu,
    		tblOut.TongAll, 
    		case when HoaHongDoanhThu > 0 then case when HoaHongThucThu > 0 then '21' else '20' end			
    				else case when HoaHongThucThu > 0 then '11' else '10' end end as Status_DoanhThu	
    	from
    		(select tbl.ID_NhanVien,			
    			sum(tbl.DoanhThu) as DoanhThu,
    			sum(tbl.ThucThu) as ThucThu,
    			sum(tbl.HoaHongDoanhThu) as HoaHongDoanhThu,
    			sum(tbl.HoaHongThucThu) as HoaHongThucThu,
    			sum(tbl.HoaHongDoanhThu)  + sum(tbl.HoaHongThucThu) as TongAll							
    		from
    			(select * from @tblSale_NVLapHD 
    			union all
    			select * from  @tblSale_NBanHang 
    			union all
    			select LoaiNhanVienApDung, ID_NhanVien, DoanhThu, 0 as ThucThu, HoaHongDoanhThu, 0 as HoaHongThucThu, IDChiTietCK from @tblSale_NVDichVu 
    			) tbl 	
    			where (exists (select ID from @tblNhanVien nv where tbl.ID_NhanVien = nv.ID))
    			group by tbl.ID_NhanVien
    		)tblOut
    	join NS_NhanVien nv on tblOut.ID_NhanVien= nv.ID
    	where
    			((select count(Name) from @tblSearchString b where     			
    			nv.TenNhanVien like '%'+b.Name+'%'
    			or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    			or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    			or nv.MaNhanVien like '%'+b.Name+'%'				
    			)=@count or @count=0)
    			) tblView
    	where tblView.Status_DoanhThu like '%'+ @Status_DoanhThu +'%'
    	),
    	count_cte
    	as (
    			select count(ID_NhanVien) as TotalRow,
    				CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float ))  as TotalPage,
    				sum(DoanhThu) as TongDoanhThu,
    				sum(ThucThu) as TongThucThu,
    				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
    				sum(HoaHongThucThu) as TongHoaHongThucThu,
    				sum(TongAll) as TongAllAll
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaNhanVien
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetChiTietHoaDon_afterTraHang]
    @ID_HoaDon [nvarchar](max)
AS
BEGIN
    set nocount on
    
    	---- get cthdmua
    	select ID		
    	into #temCTMua
    	from BH_HoaDon_ChiTiet ctm where ctm.ID_HoaDon= @ID_HoaDon
    
    	---- get cttra or ctsudung
    	select 
    		ct.ID_ChiTietGoiDV,
    		SUM(ct.SoLuong) as SoLuongTra
    	into #tmpHDTra
    	from BH_HoaDon_ChiTiet ct 
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    	where hd.ChoThanhToan='0' and hd.LoaiHoaDon not in (8,2) ---- 8.xuatkho, 2.baohanh
    	and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    	and exists (select ctm.ID from #temCTMua ctm where ct.ID_ChiTietGoiDV= ctm.ID)
    	group by ct.ID_ChiTietGoiDV
    
    
    	---- get soluong xuatkho of hdsc
    		select SUM(ctxk.SoLuong) as SoLuongXuat, ctxk.ID_ChiTietGoiDV
    		into #ctxk
    		from BH_HoaDon_ChiTiet ctxk 
    		join BH_HoaDon hdxk on ctxk.ID_HoaDon = hdxk.ID
    		where hdxk.ID_HoaDon = @ID_HoaDon
    		and hdxk.LoaiHoaDon = 8 and hdxk.ChoThanhToan='0'		
    		group by ctxk.ID_ChiTietGoiDV			
    			
    
    
    select  distinct
    		CAST(ctm.SoThuTu as float) as SoThuTu,
    		ctm.ID, 
    		ctm.ID_DonViQuiDoi,
    		ctm.ID_LoHang,
    		ctm.ID_TangKem, 
    		ctm.TangKem, 
    		ctm.ID_ParentCombo,
    		ctm.ID_ChiTietDinhLuong,
    		ctm.SoLuong,
    		ISNULL(ctt.SoLuongTra,0) as SoLuongTra,
    		iif(hd.LoaiHoaDon =25 and hh.LaHangHoa ='1', 
    		isnull(xk.SoLuongXuat,0) - ISNULL(ctt.SoLuongTra,0) ,ctm.SoLuong - ISNULL(ctt.SoLuongTra,0)) as SoLuongConLai,
    		ctm.DonGia, isnull(gv.GiaVon,0) as GiaVon, ctm.ThanhTien, ctm.ThanhToan, 
    		ctm.TienChietKhau, 
    		ctm.TienChietKhau as GiamGia,
    		ctm.ThoiGian, ctm.GhiChu, ctm.PTChietKhau,
    		ctm.ID_HoaDon, ctm.ID_ViTri,
    		ctm.ID_LichBaoDuong,
    		isnull(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
    		CAST(ISNULL(ctm.TienThue,0) as float) as TienThue,CAST(ISNULL(ctm.PTThue,0) as float) as PTThue, 
    		CAST(ISNULL(ctm.ThoiGianBaoHanh,0) as float) as ThoiGianBaoHanh,
    		CAST(ISNULL(ctm.LoaiThoiGianBH,0) as float) as LoaiThoiGianBH,
    		Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
    			Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
    		lo.NgaySanXuat, lo.NgayHetHan, isnull(lo.MaLoHang,'') as MaLoHang, isnull(tk.TonKho,0) as TonKho,
    		hh.QuanLyTheoLoHang,
    		hh.LaHangHoa,
    		iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
    		hd.MaHoaDon,
    		hh.DichVuTheoGio,
    		hh.DuocTichDiem,
    		hh.SoPhutThucHien,
			isnull(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
    		isnull(hh.ChietKhauMD_NVTheoPT,'1') as ChietKhauMD_NVTheoPT,
    		qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		qd.TenDonViTinh,
			qd.ID_HangHoa,
			qd.MaHangHoa,
    		ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan,
    		CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
    		hh.LaHangHoa, 
    		hh.TenHangHoa, 
    		CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach, 
    		hh.ID_NhomHang as ID_NhomHangHoa, 
    		ISNULL(hh.GhiChu,'') as GhiChuHH,
    		ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
    		isnull(ctm.DonGiaBaoHiem,0) as DonGiaBaoHiem,
    		iif(ctm.TenHangHoaThayThe is null or ctm.TenHangHoaThayThe ='', hh.TenHangHoa, ctm.TenHangHoaThayThe) as TenHangHoaThayThe		
    
    	from BH_HoaDon_ChiTiet ctm
    	join BH_HoaDon hd on ctm.ID_HoaDon= hd.ID
    	join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID 
    	join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    	LEFT JOIN DM_LoHang lo ON ctm.ID_LoHang = lo.ID
    	left join DM_HangHoa_TonKho tk on (ctm.ID_DonViQuiDoi = tk.ID_DonViQuyDoi and (ctm.ID_LoHang = tk.ID_LoHang or ctm.ID_LoHang is null) and  tk.ID_DonVi = hd.ID_DonVi)
    	left join DM_GiaVon gv on (tk.ID_DonViQuyDoi = gv.ID_DonViQuiDoi and (ctm.ID_LoHang = gv.ID_LoHang or ctm.ID_LoHang is null) and gv.ID_DonVi = hd.ID_DonVi) 
    	left join  #tmpHDTra ctt  on ctm.ID = ctt.ID_ChiTietGoiDV 
    	left join #ctxk xk on ctm.ID = xk.ID_ChiTietGoiDV
    	where ctm.ID_HoaDon= @ID_HoaDon
    	and (ctm.ID_ChiTietDinhLuong is null or ctm.ID_ChiTietDinhLuong = ctm.ID)
    	and (ctm.ID_ParentCombo is null or ctm.ID_ParentCombo = ctm.ID)
    	and (hh.LaHangHoa = 0 or (hh.LaHangHoa = 1 and tk.TonKho is not null))
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetDSGoiDichVu_ofKhachHang]
    @IDChiNhanhs [nvarchar](50) = null,
	@IDCustomers [nvarchar](max) = null,
	@IDCars nvarchar(max) = null,
	@TextSearch nvarchar(max) = null,
	@DateFrom datetime = null,
	@DateTo datetime = null    
AS
BEGIN
    SET NOCOUNT ON;
	declare @sql nvarchar(max)='', @where nvarchar(max)='', @paramDefined nvarchar(max)=''
	declare @tbldefined nvarchar(max) =' declare @tblChiNhanh table(ID uniqueidentifier) 
								declare @tblCus table(ID uniqueidentifier)
								declare @tblCar table(ID uniqueidentifier)'

	set @where =' where 1 = 1 and hd.LoaiHoaDon = 19 
    			and hd.ChoThanhToan=0
				and ctm.ChatLieu != 5
				and (ctm.ID_ChiTietDinhLuong is null or ctm.ID= ctm.ID_ChiTietDinhLuong) '

	if isnull(@IDChiNhanhs,'')!=''
		begin
			set @sql = CONCAT(@sql, ' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanh_In) ')
			set @where= CONCAT(@where, ' and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)')
		end

	if isnull(@IDCustomers,'')!=''
		begin			
			set @where = CONCAT(@where , ' and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblCus select name from dbo.splitstring(@IDCustomers_In) ;')
		end
	if isnull(@IDCars,'')!=''
		begin
			set @where = CONCAT(@where , ' and exists (select ID from @tblCar car where hd.ID_Xe = car.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblCar select name from dbo.splitstring(@IDCars_In) ;')
		end

	if isnull(@TextSearch,'')!=''
		set @where= CONCAT(@where, ' and (hd.MaHoaDon like N''%'' + @TextSearch_In + ''%'' 
			or qd.MaHangHoa like N''%'' + @TextSearch_In + ''%''  or hh.TenHangHoa like N''%'' + @TextSearch_In + ''%''
			 or hh.TenHangHoa_KhongDau like N''%'' + @TextSearch_In + ''%'')    ')

	if isnull(@DateFrom,'')!=''
		set @where= CONCAT(@where, ' and (hd.HanSuDungGoiDV is null or hd.HanSuDungGoiDV >= @DateFrom_In)   ')

	if isnull(@DateTo,'')!=''
		set @where= CONCAT(@where, ' and (hd.HanSuDungGoiDV is not null and hd.HanSuDungGoiDV < @DateTo_In)   ')

	set @sql = concat(@tbldefined, @sql, '

    select  
    		hd.ID as ID_GoiDV, MaHoaDon, 
			hd.NgayLapHoaDon,
    		convert(varchar,hd.NgayApDungGoiDV, 103) as NgayApDungGoiDV,
    		convert(varchar,hd.HanSuDungGoiDV, 103) as HanSuDungGoiDV, 		
    		ctm.ID as ID_ChiTietGoiDV, ctm.ID_DonViQuiDoi, ctm.ID_LoHang, 
    		ISNULL(ctm.ID_TangKem, ''00000000-0000-0000-0000-000000000000'') as ID_TangKem, ISNULL(ctm.TangKem,0) as TangKem, 
			ctm.TienChietKhau ,
			ctm.PTChietKhau ,
    		ctm.DonGia  as GiaBan,
    		ctm.SoLuong, 
    		ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) as SoLuongMua,
    		ISNULL(ctt.SoLuongDung,0) as SoLuongDung,
    		round(ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) - ISNULL(ctt.SoLuongDung,0),2) as SoLuongConLai,		
    		qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,ISNULL(qd.LaDonViChuan,0) as LaDonViChuan, CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
    		hh.LaHangHoa, 
			iif(ctm.TenHangHoaThayThe is null or ctm.TenHangHoaThayThe ='''', hh.TenHangHoa, ctm.TenHangHoaThayThe) as TenHangHoa,
			hh.TonToiThieu, CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
    		ISNULL(hh.ID_NhomHang,''00000000-0000-0000-0000-000000000001'') as ID_NhomHangHoa,
    		ISNULL(hh.SoPhutThucHien,0) as SoPhutThucHien,
    		case when hh.LaHangHoa = 1 then ''0'' else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
    		Case when hh.LaHangHoa=1 then ''0'' else ISNULL(hh.ChiPhiTinhTheoPT,''0'') end as LaPTPhiDichVu,
    		ISNULL(hh.GhiChu,'''') as GhiChuHH,
    		ISNULL(ctm.GhiChu,'''') as GhiChu,
    		isnull(hh.QuanLyTheoLoHang,''0'') as QuanLyTheoLoHang,
    		lo.MaLoHang, lo.NgaySanXuat, lo.NgayHetHan, xe.BienSo, hd.ID_Xe,			
			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
			ISNULL(hh.ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
			ISNULL(hh.LoaiBaoHanh,0) as LoaiBaoHanh,			
			ISNULL(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
			ISNULL(hh.ChietKhauMD_NVTheoPT,''1'') as ChietKhauMD_NVTheoPT,
			ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			hh.LaHangHoa,
			ctm.ID_ParentCombo,			
			iif(ctm.ID_ParentCombo = ctm.ID, 0,1) as SoThuTu,
			isnull(ctm.GiaVon,0) as GiaVon
    	from BH_HoaDon_ChiTiet ctm
    	join BH_HoaDon hd on ctm.ID_HoaDon = hd.ID
		left join Gara_DanhMucXe xe on hd.ID_Xe= xe.ID
    	join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
    	left join DM_LoHang lo on ctm.ID_LoHang = lo.ID
    	left join 
    	(
    		select a.ID_ChiTietGoiDV,
    			SUM(a.SoLuongTra) as SoLuongTra,
    			SUM(a.SoLuongDung) as SoLuongDung
    		from
    			(-- sum soluongtra
    			select ct.ID_ChiTietGoiDV,
    				SUM(ct.SoLuong) as SoLuongTra,
    				0 as SoLuongDung
    			from BH_HoaDon_ChiTiet ct 
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    			where hd.ChoThanhToan= 0 and hd.LoaiHoaDon = 6
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			group by ct.ID_ChiTietGoiDV
    
    			union all
    			-- sum soluong sudung
    			select ct.ID_ChiTietGoiDV,
    				0 as SoLuongDung,
    				SUM(ct.SoLuong) as SoLuongDung
    			from BH_HoaDon_ChiTiet ct 
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    			where hd.ChoThanhToan=0 and hd.LoaiHoaDon in (1,25)
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			group by ct.ID_ChiTietGoiDV
    			) a group by a.ID_ChiTietGoiDV
    	) ctt on ctm.ID = ctt.ID_ChiTietGoiDV ', @where, ' order by hd.NgayLapHoaDon desc')

		print @sql
    	
		set @paramDefined =' @IDChiNhanh_In nvarchar(max),
							@IDCustomers_In nvarchar(max),
							@IDCars_In nvarchar(max),
							@TextSearch_in nvarchar(max),
							@DateFrom_In nvarchar(max),
							@DateTo_in nvarchar(max)'
		
			exec sp_executesql @sql, @paramDefined,
							@IDChiNhanh_In = @IDChiNhanhs,
							@IDCustomers_In = @IDCustomers,
							@IDCars_in = @IDCars,
							@TextSearch_In = @TextSearch,
							@DateFrom_In = @DateFrom,
							@DateTo_in = @DateTo

END");

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
    		set @LoaiHoaDons = '1,19,25,2' --- 2.hoadon baohanh, 25.hdsc
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

			Sql(@"ALTER PROCEDURE [dbo].[Load_DMHangHoa_TonKho]
	 @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN

	SET NOCOUNT ON;
	declare @dateNow datetime = FORMAT(getdate(),'yyyyMMdd')
	declare @next10Year Datetime = FORMAT(dateadd(year,10, getdate()),'yyyyMMdd')

		select 
			dhh1.ID,
			dvqd1.ID as ID_DonViQuiDoi,		
			MAX(ROUND(ISNULL(tk.TonKho,0),2)) as TonKho,
			MAX(CAST(ROUND(( ISNULL(gv.GiaVon,0)), 0) as float)) as GiaVon,
			TenHangHoa,
			MaHangHoa,
			LaDonViChuan,  
			LaHangHoa,
			dhh1.TonToiThieu,
			ID_NhomHang as ID_NhomHangHoa,
			ISNULL(QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
			Case when dhh1.LaHangHoa='1' then 0 else CAST(ISNULL(dhh1.ChiPhiThucHien,0) as float) end as PhiDichVu,
			Case when dhh1.LaHangHoa='1' then '0' else ISNULL(dhh1.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
			ISNULL(lh1.ID,  NEWID()) as ID_LoHang,
			case when MAX(ISNULL(QuyCach,0)) = 0 then MAX(TyLeChuyenDoi) else MAX(QuyCach) * MAX(TyLeChuyenDoi) end as QuyCach,	
			MAX(ISNULL(TyLeChuyenDoi,0)) as TyLeChuyenDoi, 		
			isnull(TenHangHoa_KhongDau,'') as TenHangHoa_KhongDau,
			CONCAT(MaHangHoa, ' ' , lower(MaHangHoa) ,' ', TenHangHoa, ' ', TenHangHoa_KhongDau,' ',
			MAX(MaLoHang),' ', Cast(max(GiaBan) as decimal(22,0)), MAX(ISNULL(dvqd1.ThuocTinhGiaTri,''))) as Name,
    		MAX(ISNULL(dvqd1.ThuocTinhGiaTri,'')) as ThuocTinh_GiaTri,
    		MAX(GiaBan) as GiaBan, 
    		MAX (TenDonViTinh) as TenDonViTinh, 	
			case when MAX(ISNULL(an.URLAnh,'')) = '' then '' else 'CssImg' end as CssImg,		
    		MAX(ISNULL(an.URLAnh,'')) as SrcImage, 		
    		MAX(ISNULL(MaLoHang,'')) as MaLoHang,
    		MAX(NgaySanXuat) as NgaySanXuat,
    		MAX(NgayHetHan) as NgayHetHan,
			MAX(ISNULL(DonViTinhQuyCach,'')) as DonViTinhQuyCach,
			MAX(ISNULL(ThoiGianBaoHanh,0)) as ThoiGianBaoHanh,
			MAX(ISNULL(LoaiBaoHanh,0)) as LoaiBaoHanh,
			MAX(ISNULL(SoPhutThucHien,0)) as SoPhutThucHien, 
			MAX(ISNULL(dhh1.GhiChu,'')) as GhiChuHH ,
			MAX(ISNULL(dhh1.DichVuTheoGio,0)) as DichVuTheoGio, 
			MAX(ISNULL(dhh1.DuocTichDiem,0)) as DuocTichDiem, 
			MAX(ISNULL(dhh1.DichVuTheoGio,0)) as DichVuTheoGio, 
			MAX(ISNULL(dhh1.ChietKhauMD_NV,0)) as ChietKhauMD_NV, 
			ISNULL(dhh1.ChietKhauMD_NVTheoPT,'1') as ChietKhauMD_NVTheoPT, 
			0 as SoGoiDV,
			@next10Year as HanSuDungGoiDV_Min,
			'' as BackgroundColor,
			iif(dhh1.LoaiHangHoa is null, iif(dhh1.LaHangHoa='1',1,2), dhh1.LoaiHangHoa) as LoaiHangHoa,
			isnull(nhom.TenNhomHangHoa,N'Nhóm mặc định') as TenNhomHangHoa
		from DonViQuiDoi dvqd1 
		join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
		left join DM_NhomHangHoa nhom on dhh1.ID_NhomHang = nhom.ID
		left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa and lh1.TrangThai='1'
		left join DM_HangHoa_TonKho tk on (dvqd1.ID = tk.ID_DonViQuyDoi and (lh1.ID = tk.ID_LoHang or lh1.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		left join DM_HangHoa_Anh an on (dvqd1.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))		
		left join DM_GiaVon gv on (dvqd1.ID = gv.ID_DonViQuiDoi and (lh1.ID = gv.ID_LoHang or lh1.ID is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where (dvqd1.xoa ='0'  or dvqd1.Xoa is null)
		and dhh1.TheoDoi = '1'	
		and dhh1.DuocBanTrucTiep = '1' --- chi lay hanghoa DuocBanTrucTiep
		and (dhh1.LaHangHoa = 0 or (dhh1.LaHangHoa = 1 and tk.TonKho is not null)) -- chi lay HangHoa neu exsit in DM_TonKho_HangHoa
		and (lh1.NgayHetHan is null or (lh1.NgayHetHan >= @dateNow))		
		group by dhh1.ID, dvqd1.ID, lh1.ID, MaHangHoa,ID_NhomHang,TenHangHoa,TenHangHoa_KhongDau,TenHangHoa_KyTuDau,
		LaDonViChuan,LaHangHoa,ChiPhiThucHien,ChiPhiTinhTheoPT, dhh1.QuanLyTheoLoHang, dhh1.TonToiThieu, dhh1.LoaiHangHoa,nhom.TenNhomHangHoa,  dhh1.ChietKhauMD_NVTheoPT
		order by MaHangHoa,NgayHetHan
END");

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountProduct_General]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs [nvarchar](max),
    @TextSearch [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    
    	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHangHoa_XemDS_PhongBan','BCCKHangHoa_XemDS_HeThong');
    
		DECLARE @tblDepartment TABLE (ID_PhongBan uniqueidentifier)
		if @DepartmentIDs =''
			insert into @tblDepartment
			select distinct ID_PhongBan from NS_QuaTrinhCongTac pb
		else
			insert into @tblDepartment
			select * from splitstring(@DepartmentIDs)

		----- xoa nhanvien if not exists in phongban filter
		----- get nv thuoc PB
		declare @tblNhanVien table (ID uniqueidentifier)
		insert into @tblNhanVien
		select nv.ID
		from @tblNhanVienAll nv
		join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
		where exists (select ID_PhongBan from @tblDepartment pb where pb.ID_PhongBan= ct.ID_PhongBan) 
    
    
    	set @DateTo = DATEADD(day,1,@DateTo);
    
    	with data_cte
    	as (
    
    	select nv.MaNhanVien, nv.TenNhanVien, b.*
    	from
    	(
    		SELECT 
    			a.ID_NhanVien,
    			CAST(ROUND(SUM(a.HoaHongThucHien), 0) as float) as HoaHongThucHien,
    				CAST(ROUND(SUM(a.HoaHongThucHien_TheoYC), 0) as float) as HoaHongThucHien_TheoYC,
    			CAST(ROUND(SUM(a.HoaHongTuVan), 0) as float) as HoaHongTuVan,
    			CAST(ROUND(SUM(a.HoaHongBanGoiDV), 0) as float) as HoaHongBanGoiDV,   		
    			case @Status_ColumHide
    				when  1 then cast(0 as float)
    				when  2 then SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  3 then SUM(ISNULL(HoaHongBanGoiDV,0.0))
    				when  4 then SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  5 then SUM(ISNULL(HoaHongTuVan,0.0))
    				when  6 then SUM(ISNULL(HoaHongThucHien_TheoYC,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
    				when  7 then SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
    					when  8 then SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  9 then SUM(ISNULL(HoaHongThucHien,0.0))
    				when  10 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  11 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0)) 
    				when  12 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  13 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
    					when  14 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  15 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0))
    			else SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0))+  SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    			end as Tong
    		FROM
    		(
    				select ckout.ID_NhanVien,
    					ckout.TrangThaiHD,
    					case when ckout.LoaiHoaDon= 6 then - HoaHongThucHien else HoaHongThucHien end as HoaHongThucHien,
    					case when ckout.LoaiHoaDon= 6 then - HoaHongTuVan else HoaHongTuVan end as HoaHongTuVan,
    					case when ckout.LoaiHoaDon= 6 then - HoaHongThucHien_TheoYC else HoaHongThucHien_TheoYC end as HoaHongThucHien_TheoYC,
    					case when ckout.LoaiHoaDon= 6 then - HoaHongBanGoiDV else HoaHongBanGoiDV end as HoaHongBanGoiDV				
    				from
    				(Select 
    						ck.ID_NhanVien,
    						hd.LoaiHoaDon,
    					Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien_TheoYC,
    					Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
    						case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    				from
    				BH_NhanVienThucHien ck
    				inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    				inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    				Where hd.ChoThanhToan is not null
    					and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
						and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    					and hd.NgayLapHoaDon >= @DateFrom 
    					and hd.NgayLapHoaDon < @DateTo   
    						and (exists (select ID from @tblNhanVien nv where ck.ID_NhanVien = nv.ID))
    					) ckout
    		) a where a.TrangThaiHD = @StatusInvoice
    		GROUP BY a.ID_NhanVien
    	) b
    		join NS_NhanVien nv on b.ID_NhanVien= nv.ID
    		where 
    			((select count(Name) from @tblSearchString b where     			
    				nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'				
    				)=@count or @count=0)		
    		),
    		count_cte
    		as (
    			select count(ID_NhanVien) as TotalRow,
    				CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float ))  as TotalPage,
    				sum(HoaHongThucHien) as TongHoaHongThucHien,
    				sum(HoaHongThucHien_TheoYC) as TongHoaHongThucHien_TheoYC,
    				sum(HoaHongTuVan) as TongHoaHongTuVan,
    				sum(HoaHongBanGoiDV) as TongHoaHongBanGoiDV,
    				sum(Tong) as TongAll
    			from data_cte
    			)
    		select dt.*, cte.*
    		from data_cte dt			
    		cross join count_cte cte
    		order by dt.MaNhanVien
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[ReportValueCard_DiaryUsed]
    @ID_ChiNhanhs [nvarchar](max),
    @TextSearch [nvarchar](max),
    @DateFrom [nvarchar](14),
    @DateTo [nvarchar](14),
    @Status [nvarchar](5),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    DECLARE @TblHisCard TABLE(
    				STT INT, ID UNIQUEIDENTIFIER, ID_DoiTuong UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(50),TenDoiTuong NVARCHAR(500), MaHoaDon NVARCHAR(500),  SLoaiHoaDon nvarchar(max), 
    				MaHoaDonSQ NVARCHAR(MAX),LoaiHoaDonSQ INT, NgayLapHoaDon DATETIME, TienThe FLOAT, ThuChiThe FLOAT, SoDuTruoc FLOAT, SoDuSau FLOAT, TrangThai_TheGiaTri int)
    	INSERT INTO @TblHisCard
    		SELECT 
    				ROW_NUMBER() OVER(ORDER BY qhd.ID) AS STT,
    				qhd.ID,
    				dt.ID as ID_DoiTuong,-- used to caculator sodutruoc phatsinh
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				MaHoaDons,
    				ISNULL(LoaiHoaDons, N'Loại khác, ') as LoaiHoaDons,
    				qhd.MaHoaDon as MaHoaDonSQ, 
    				qhd.LoaiHoaDon as LoaiHoaDonSQ,
    				qhd.NgayLapHoaDon,
    				SUM(ISNULL(qct.TienThu,0)) as TienThe,
    				case when qhd.LoaiHoaDon = 11 then - SUM(ISNULL(qct.TienThu,0)) else SUM(ISNULL(qct.TienThu,0)) end as ThuChiThe,
    				0 as SoDuTruoc,
    				0 as SoDuSau,
    				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
    				else '12' end as TrangThai
    			FROM Quy_HoaDon qhd
    			join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    			join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
    			join (
    				--merger text MaHoaDon to 1 row
    				Select distinct qhdXML.ID, 
    							 (
    						select distinct hd.MaHoaDon +', '  AS [text()]
    						from BH_HoaDon hd
    						join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    						where qct.ID_HoaDon = qhdXML.ID
    						and hd.LoaiHoaDon in (1, 3, 6, 19,25)
    						and ChoThanhToan ='0'
    						For XML PATH ('')
    					) MaHoaDons
    				from Quy_HoaDon qhdXML
    			) tbl on qhd.ID= tbl.ID
    
    			-- get LoaiHoaDon
    			join (
    				Select distinct qhdXML2.ID, 
    					 (
    					 -- merger text LoaiHoaDon to 1 row
    						select distinct 
    							 tbl1.SLoaiHoaDon +', '  AS [text()]
    						from 
    							(
    							-- get text HoaDon by LoaiHoaDon
    							select 
    								case hd.LoaiHoaDon
    									when 1 then N'Bán hàng'
    									when 3 then N'Đặt hàng'
    									when 6 then N'Trả hàng'
    									when 25 then N'Sửa chữa'
    								else 
    									N'Gói dịch vụ' end as SLoaiHoaDon
    							from BH_HoaDon hd
    							join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    							where hd.LoaiHoaDon in (1, 3, 6, 19, 25)
    							and ChoThanhToan ='0' 
    							and qct.ID_HoaDon = qhdXML2.ID
    							) tbl1
    						For XML PATH ('')
    					) LoaiHoaDons
    				from Quy_HoaDon qhdXML2
    		) tbl2 on qhd.ID= tbl2.ID
    
    	where (qhd.TrangThai = 1 or qhd.TrangThai is null)		
    	and qct.HinhThucThanhToan = 4
    	and FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') >= @DateFrom 
    	and FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') <= @DateTo
    	and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    	and				 
    		((select count(Name) from @tblSearchString b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
    		or dt.DienThoai like '%'+b.Name+'%'			
    		)=@count or @count=0)	
    	GROUP BY qhd.ID,
    		dt.ID,
    		dt.MaDoiTuong,
    		dt.TenDoiTuong,
    		qhd.MaHoaDon,
    		qhd.LoaiHoaDon,
    		qhd.NgayLapHoaDon,
    		dt.TrangThai_TheGiaTri,
    		MaHoaDons,
    		LoaiHoaDons
    		    	
    			DECLARE @SoDuTruocPhatSinh INT;
    
    			DECLARE @STT INT;
    			DECLARE @ID UNIQUEIDENTIFIER;
    			DECLARE @ID_DoiTuong UNIQUEIDENTIFIER;
    			DECLARE @MaDoiTuong NVARCHAR(50);
    			DECLARE @TenDoiTuong NVARCHAR(500);
    			DECLARE @MaHoaDon NVARCHAR(500);
    			DECLARE @LoaiHoaDon nvarchar(max);
    			DECLARE @MaHoaDonSQ NVARCHAR(50);
    			DECLARE @LoaiHoaDonSQ INT;
    			DECLARE @NgayLapHoaDon DATETIME;
    			DECLARE @TienThe FLOAT;
    			DECLARE @ThuChiThe FLOAT;
    			DECLARE @SoDuTruoc FLOAT;
    			DECLARE @SoDuSau FLOAT;
    			DECLARE @TrangThai_TheGiaTri INT;
    
    			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR SELECT STT, ID,ID_DoiTuong, MaDoiTuong, TenDoiTuong, MaHoaDon, SLoaiHoaDon, MaHoaDonSQ, LoaiHoaDonSQ, NgayLapHoaDon, TienThe, ThuChiThe, SoDuTruoc, SoDuSau, TrangThai_TheGiaTri
    			FROM @TblHisCard
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe,@ThuChiThe, @SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
    					SET @SoDuTruocPhatSinh = [dbo].[TinhSoDuKHTheoThoiGian](@ID_DoiTuong,@NgayLapHoaDon)
    
    					UPDATE @TblHisCard SET SoDuTruoc= @SoDuTruocPhatSinh, SoDuSau = @SoDuTruocPhatSinh + @ThuChiThe
    					WHERE STT = @STT
    
    					FETCH NEXT FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe, @ThuChiThe,@SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    
    					
    			END
    		CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT;
    
    			with data_cte
    			as (    
    				SELECT ID, ID_DoiTuong,MaDoiTuong,TenDoiTuong,
    						LEFT(MaHoaDon, LEN(MaHoaDon) - 1) as MaHoaDon,
    						LEFT(SLoaiHoaDon, LEN(SLoaiHoaDon) - 1) as SLoaiHoaDon,
    						MaHoaDonSQ, LoaiHoaDonSQ,NgayLapHoaDon,TienThe,SoDuTruoc, 
    						IIF(LoaiHoaDonSQ = 12, TienThe, 0) AS PhatSinhTang,
    						IIF(LoaiHoaDonSQ = 11, TienThe, 0) AS PhatSinhGiam, 
    						SoDuSau, TrangThai_TheGiaTri				
    				FROM @TblHisCard 
    				WHERE TrangThai_TheGiaTri like @Status
    				),
    			count_cte
    		as (
    				select count(ID) as TotalRow,
    					CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    					sum(SoDuTruoc) as TongSoDuDauKy,
    					sum(PhatSinhTang) as TongPhatSinhTang,
    					sum(PhatSinhGiam) as TongPhatSinhGiam,
    					sum(SoDuSau) as TongSoDuCuoiKy
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[TheGiaTri_GetLichSuNapTien]
    @IDChiNhanhs [nvarchar](max),
    @ID_Cutomer [nvarchar](max),
    @TextSearch [nvarchar](max) = null,
    @DateFrom [nvarchar](max) = null,
    @DateTo [nvarchar](max) = null,
    @CurrentPage [int] = null,
    @PageSize [int] = null
AS
BEGIN
    SET NOCOUNT ON;
    	
    	declare @paramIn nvarchar(max)=' declare @isNull_txtSearch int = 1 '
    
    	declare @tblDefined nvarchar(max)='', @sql1 nvarchar(max) ='',  @sql2 nvarchar(max) ='',
    	@whereIn nvarchar(max)='', @whereOut nvarchar(max)='',
    	
    	@paramDefined nvarchar(max)= N'
    			@IDChiNhanhs_In [nvarchar](max) ,
    			@ID_Cutomer_In [nvarchar](max),
    			@TextSearch_In [nvarchar](max),
    			@DateFrom_In [datetime],
    			@DateTo_In [datetime],		
    			@CurrentPage_In [int],
    			@PageSize_In [int]
    			 '
    		set @whereIn = ' where 1 = 1 and hd.LoaiHoaDon in (22,23, 32) and hd.ChoThanhToan = 0'
    		set @whereOut = ' where 1 = 1'
    
    		if isnull(@CurrentPage,'') ='' set @CurrentPage = 0
    		if isnull(@PageSize,'') ='' set @PageSize = 20
    
    		if isnull(@IDChiNhanhs,'')!=''
    			begin
    				set @tblDefined = concat(@tblDefined, N' declare @tblChiNhanh table (ID uniqueidentifier)
    					insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ')
    				set @whereIn= CONCAT(@whereIn, ' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)')
    			end
    
    		if isnull(@ID_Cutomer,'')!=''
    		begin
    			set @whereIn= CONCAT(@whereIn, ' and hd.ID_DoiTuong = @ID_Cutomer_In')
    		end
    
    	   if isnull(@DateFrom,'')!=''
    		begin
    			set @whereIn= CONCAT(@whereIn, ' and hd.NgayLapHoaDon >= @DateFrom_In')
    		end
    
    		if isnull(@DateTo,'')!=''
    		begin
    			set @whereIn= CONCAT(@whereIn, ' and hd.NgayLapHoaDon < @DateTo_In')
    		end
    
    		if isnull(@TextSearch,'')!='' and isnull(@TextSearch,'')!='%%'
    			begin			
    				set @paramIn = CONCAT(@paramIn, ' set @isNull_txtSearch = 0')
    				set @TextSearch = CONCAT(N'%', @TextSearch, '%')
    				set @whereOut= CONCAT(@whereOut, ' 
    					and (MaHoaDon like @TextSearch_In
    						OR MaDoiTuong like @TextSearch_In
    						OR TenDoiTuong like @TextSearch_In
    						OR TenDoiTuong_KhongDau like @TextSearch_In
    						OR MaHoaDon like @TextSearch_In
    						OR DienGiai like @TextSearch_In
    						OR DienGiaiUnSign like @TextSearch_In)'
    					)					
    			end
    
    			set @sql1 = concat(N'
    
    			select hd.ID,
    				hd.ID_DoiTuong,			
					hd.MaHoaDon,
					hd.LoaiHoaDon,
					hd.NgayLapHoaDon,
					hd.TongChiPhi,
					hd.TongChietKhau,
					hd.TongTienHang,
					hd.TongTienThue,
					hd.TongGiamGia,
					hd.PhaiThanhToan,  					
					hd.TongTienHang as SoDuSauNap,
					hd.DienGiai
    				into #htThe
    			from BH_HoaDon hd ', @whereIn)
    			   
    		
    			set @sql2 = concat(N'
				---- get luyke soduSauNap
				declare @soduLuyke float = 0
				declare @ID_HoaDon uniqueidentifier, @SoDuSauNap float
				declare _cur cursor for

				select ID, SoDuSauNap
				from #htThe order by NgayLapHoaDon 
	
				open _cur
				FETCH NEXT FROM _cur
				INTO @ID_HoaDon, @SoDuSauNap
				WHILE @@FETCH_STATUS = 0
				BEGIN   
					 set @soduLuyke = @soduLuyke + @SoDuSauNap
					 update #htThe set SoDuSauNap= @soduLuyke where ID= @ID_HoaDon		
					FETCH NEXT FROM _cur

				INTO @ID_HoaDon, @SoDuSauNap

				END
				CLOSE _cur;
				DEALLOCATE _cur;

    	
    			select tbl.*, 
    				dt.MaDoiTuong as MaKhachHang,
    				dt.TenDoiTuong as TenKhachHang
    				from
    				(
    				select hd.ID,
    					hd.ID_DoiTuong,
    					hd.LoaiHoaDon,
    					hd.MaHoaDon,
    					hd.NgayLapHoaDon,
    					hd.TongChiPhi as MucNap,
    					hd.TongChietKhau as KhuyenMaiVND,
    					hd.TongTienHang as TongTienNap,
    					hd.SoDuSauNap,
    					hd.TongGiamGia as ChietKhauVND,
    					hd.PhaiThanhToan,
    					hd.DienGiai,
    					iif(@isNull_txtSearch =0, dbo.FUNC_ConvertStringToUnsign(hd.DienGiai), hd.DienGiai ) as DienGiaiUnSign,
    					isnull(thu.KhachDaTra,0) as KhachDaTra
    				from #htThe hd
    				left join
    				(
    					select 
    						qct.ID_HoaDonLienQuan,
    						sum(qct.TienThu) as KhachDaTra
    					from Quy_HoaDon_ChiTiet qct
    					join #htThe hd on qct.ID_HoaDonLienQuan = hd.ID
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					where qhd.TrangThai= 1 or qhd.TrangThai= 1
    					group by qct.ID_HoaDonLienQuan
    				) thu
    				on hd.ID = thu.ID_HoaDonLienQuan
    			) tbl 
    			join DM_DoiTuong dt on tbl.ID_DoiTuong = dt.ID
    			', @whereOut , ' order by tbl.NgayLapHoaDon desc')
    
    		set @sql2= CONCAT(@paramIn, '; ', @tblDefined, @sql1,'; ', @sql2)		 
    		
		print @sql2
    		exec sp_executesql @sql2, 
    			@paramDefined,
    			@IDChiNhanhs_In= @IDChiNhanhs,
    			@ID_Cutomer_In = @ID_Cutomer,
    			@TextSearch_In = @TextSearch,
    			@DateFrom_In = @DateFrom,
    			@DateTo_In = @DateTo,			
    			@CurrentPage_In = @CurrentPage,
    			@PageSize_In = @PageSize
END");
        }
        
        public override void Down()
        {
			DropStoredProcedure("[dbo].[GetGiaVonTieuChuan_byTime]");
			DropStoredProcedure("[dbo].[GetListHoaDon_ChuaPhanBoCK]");
			DropStoredProcedure("[dbo].[SearchHangHoa_withGiaVonTieuChuan]");
        }
    }
}
