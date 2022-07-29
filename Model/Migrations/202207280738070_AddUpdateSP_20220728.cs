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
	SoLuong FLOAT, GiaNHap FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, GiaVon FLOAT, YeuCau NVARCHAR(MAX), GiamGiaHDPT FLOAT, GhiChu nvarchar(max),
	NguoiTao nvarchar(max),MaDoiTuong nvarchar(max), TenDoiTuong nvarchar(max));

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
		GiaNhap,
		TienChietKhau,
		ThanhTien,
		GiaVon,
		YeuCau,
		GianGiaHD,
		GhiChu,
		tbl.NguoiTao,
		tbl.MaDoiTuong, 
		tbl.TenDoiTuong
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
		case bhd.LoaiHoaDon
			when 4 then bhdct.DonGia - bhdct.TienChietKhau
			when 13 then  bhdct.DonGia - bhdct.TienChietKhau
			when 14 then  bhdct.DonGia - bhdct.TienChietKhau
			when 10 then case when YeuCau='4' then bhdct.GiaVon_NhanChuyenHang else bhdct.GiaVon end
		else bhdct.GiaVon end as GiaNhap,
		bhdct.TienChietKhau,
		bhdct.ThanhTien, 
		bhd.NguoiTao,
		case bhd.LoaiHoaDon
			when 4 then dt.MaDoiTuong
			when 14 then dt.MaDoiTuong
			when 6 then dt.MaDoiTuong
			when 13 then iif(dt.ID='00000000-0000-0000-0000-000000000002' or dt.ID is null, nv.MaNhanVien, dt.MaDoiTuong)
		else nv.MaNhanVien end as MaDoiTuong,
		case bhd.LoaiHoaDon
			when 4 then dt.TenDoiTuong
			when 14 then dt.TenDoiTuong
			when 6 then dt.TenDoiTuong
			when 13 then iif(dt.ID='00000000-0000-0000-0000-000000000002' or dt.ID is null, nv.TenNhanVien, dt.TenDoiTuong)
		else nv.TenNhanVien end as TenDoiTuong,
		case bhd.LoaiHoaDon
			when 4 then dt.TenDoiTuong_KhongDau
			when 14 then dt.TenDoiTuong_KhongDau
			when 6 then dt.TenDoiTuong_KhongDau
			when 13 then iif(dt.ID='00000000-0000-0000-0000-000000000002' or dt.ID is null, nv.TenNhanVienKhongDau, dt.TenDoiTuong_KhongDau)
		else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
		case bhd.LoaiHoaDon
			when 6 then case when ctm.GiaVon is null or bhdct.ID_ChiTietDinhLuong is null then bhdct.GiaVon else ctm.GiaVon end
			when 10 then case when bhd.ID_CheckIn= dv.ID then bhdct.GiaVon_NhanChuyenHang else bhdct.GiaVon end
		else bhdct.GiaVon end as GiaVon,			
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
	left join DM_DoiTuong dt on bhd.ID_DoiTuong= dt.ID
	left join NS_NhanVien nv on bhd.ID_NhanVien= nv.ID
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
	AND hh.LaHangHoa = 1 AND hh.TheoDoi LIKE @TheoDoi AND dvqd.Xoa LIKE @TrangThai 
	) tbl 
	where ((select count(Name) from @tblSearchString b where 
    		tbl.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or tbl.NguoiTao like '%'+b.Name+'%'
				or tbl.MaDoiTuong like '%'+b.Name+'%'
				or tbl.TenDoiTuong like '%'+b.Name+'%'
				or tbl.TenDoiTuong_KhongDau like '%'+b.Name+'%'
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
	dv.TenDonVi, dv.MaDonVi, 
	pstk.SoLuongNhap AS SoLuong,
	IIF(@XemGiaVon = '1',pstk.GiaNhap,0) as GiaNhap,
	IIF(@XemGiaVon = '1',pstk.GiaTriNhap,0) as ThanhTien,
	pstk.NguoiTao,
	iif(LoaiHoaDon = 10, dv.MaDonVi, pstk.MaDoiTuong) as MaDoiTuong, 
	iif(LoaiHoaDon = 10, dv.TenDonVi, pstk.TenDoiTuong) as TenDoiTuong
	FROM 
	(
    SELECT 
    MaHoaDon, NgayLapHoaDon, TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	LoaiHoaDon, DienGiai, GhiChu,
	NguoiTao,
	MaDoiTuong, 
	TenDoiTuong,
	IIF(LoaiHoaDon = 10 and YeuCau = '4', TienChietKhau* TyLeChuyenDoi, SoLuong * TyLeChuyenDoi) AS SoLuongNhap,
	GiaNhap,
	IIF(LoaiHoaDon = 10 and YeuCau = '4' ,TienChietKhau* GiaVon, iif(LoaiHoaDon = 6, SoLuong * GiaVon,   ThanhTien*(1-GiamGiaHDPT))) AS GiaTriNhap
    FROM @tblHoaDon WHERE LoaiHoaDon != 9

	UNION ALL
    SELECT 
    MaHoaDon, NgayLapHoaDon, TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	LoaiHoaDon,DienGiai, GhiChu,
	NguoiTao,
	MaDoiTuong, 
	TenDoiTuong,
	sum(SoLuong * TyLeChuyenDoi) as SoLuongNhap,	
	max(GiaVon) as GiaNhap,
	SUM(SoLuong * TyLeChuyenDoi * GiaVon) AS GiaTriNhap
    FROM @tblHoaDon
    WHERE LoaiHoaDon = 9 and SoLuong > 0 
    GROUP BY LoaiHoaDon, MaHoaDon, NgayLapHoaDon,TenNhomHang, MaHangHoa, 
	TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,DienGiai, GhiChu,NguoiTao,MaDoiTuong, 
	TenDoiTuong
	) pstk
	join DM_DonVi dv on pstk.ID_DonVi= dv.ID
	INNER JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
	order by pstk.NgayLapHoaDon desc

END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_ChiTietHangXuat]
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
	 DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER);
    INSERT INTO @tblChiNhanh SELECT Name FROM splitstring(@ID_DonVi)

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
	
			select 
				dv.MaDonVi, dv.TenDonVi, nv.TenNhanVien,
				tblQD.NgayLapHoaDon, tblQD.MaHoaDon,
				tblQD.BienSo,
				isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
				isnull(lo.MaLoHang,'') as TenLoHang,
				qd.MaHangHoa, qd.TenDonViTinh, 
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				hh.TenHangHoa,
				CONCAT(hh.TenHangHoa,qd.ThuocTinhGiaTri) as TenHangHoaFull,
				tblQD.GhiChu,
				tblQD.DienGiai,
				round(SoLuong,3) as SoLuong,
				iif(@XemGiaVon='1', round(GiaTriXuat,3),0) as ThanhTien,
				case tblQD.LoaiXuatKho
					when 1 then N'Hóa đơn bán lẻ'
					when 2 then N'Xuất sử dụng gói dịch vụ'
					when 3 then N'Xuất bán dịch vụ định lượng'
					when 11 then N'Xuất sửa chữa'
					when 7 then N'Trả hàng nhà cung cấp'
					when 8 then N'Xuất kho'
					when 9 then N'Phiếu kiểm kê'
					when 10 then N'Chuyển hàng'		
					when 12 then N'Xuất bảo hành'	
				end as LoaiHoaDon,
				tblQD.NguoiTao,
				case when tblQD.LoaiHoaDon in (1,2,3,7) then dt.MaDoiTuong else nv.MaNhanVien end as MaDoiTuong,
				case when tblQD.LoaiHoaDon in (1,2,3,7) then dt.TenDoiTuong else nv.TenNhanVien end as TenDoiTuong
			from
			(
					select 
						qd.ID_HangHoa,
						tblHD.ID_LoHang, 
						tblHD.ID_DonVi,
						tblHD.ID_CheckIn,
						tblHD.NgayLapHoaDon,
						tblHD.MaHoaDon, 
						tblHD.ID_NhanVien,
						tblHD.LoaiHoaDon,
						tblHD.LoaiXuatKho,
						tblHD.BienSo,
						tblHD.DienGiai,
						tblHD.NguoiTao,
						tblHD.ID_DoiTuong,
						iif(@SearchString='',tblHD.DienGiai,dbo.FUNC_ConvertStringToUnsign(tblHD.DienGiai)) as DienGiaiUnsign,
						max(tblHD.GhiChu) as GhiChu,
						iif(@SearchString='',max(tblHD.GhiChu),max(dbo.FUNC_ConvertStringToUnsign(tblHD.GhiChu))) as GhiChuUnsign,
						sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
						sum(tblHD.GiaTriXuat) as GiaTriXuat
					from
					(
						select 
							hd.NgayLapHoaDon, 
							hd.MaHoaDon,
							hd.LoaiHoaDon,
							hd.ID_HoaDon,
							hd.ID_CheckIn,
							hd.ID_NhanVien, 
							hd.DienGiai, 
							xe.BienSo,
							hd.NguoiTao,
							hd.ID_DoiTuong, 
							case hd.LoaiHoaDon
								when 2 then 12 --- xuat baohanh (Vì loai = 2 đã dùng cho xuat sudung gdv)
								when 8 then case when hd.ID_PhieuTiepNhan is not null then case when ct.ChatLieu= 4 then 2 else  11 end -- xuat suachua
									else 8 end
								when 1 then case when hd.ID_HoaDon is null and ct.ID_ChiTietGoiDV is not null then 2 --- xuat sudung gdv
									else case when ct.ID_ChiTietDinhLuong is not null then 3 --- xuat ban dinhluong
									else 1 end end -- xuat banle
								else hd.LoaiHoaDon end as LoaiXuatKho, -- xuat khac: traNCC, chuyenang,..
							ct.ID_ChiTietGoiDV,
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_DonVi,
							case hd.LoaiHoaDon
							when 9 then iif(ct.SoLuong < 0, -ct.SoLuong, 0)
							when 10 then ct.TienChietKhau else ct.SoLuong end as SoLuong,
							ct.TienChietKhau,
							ct.GiaVon,
							ct.GhiChu,
							case hd.LoaiHoaDon
								when 7 then ct.SoLuong * ct.DonGia
								when 10 then ct.TienChietKhau * ct.GiaVon
								when 9 then iif(ct.SoLuong < 0, -ct.SoLuong, 0) * ct.GiaVon
								else ct.SoLuong* ct.GiaVon end as GiaTriXuat
						from BH_HoaDon_ChiTiet ct
						join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
						left join Gara_PhieuTiepNhan tn on hd.ID_PhieuTiepNhan = tn.ID
						left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
						WHERE hd.ChoThanhToan = 0 
						AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
						and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
						and hd.LoaiHoaDon not in (3,4,6,19,25)
						and iif(hd.LoaiHoaDon=9,ct.SoLuong, -1) < 0 -- phieukiemke: chi lay neu soluong < 0 (~xuatkho)
					) tblHD
				join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID			
				where exists (select Name from dbo.splitstring(@LoaiChungTu) loaict where tblHD.LoaiXuatKho = loaict.Name)				
				group by qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_DonVi,
				 tblHD.ID_CheckIn,tblHD.NgayLapHoaDon, tblHD.MaHoaDon, tblHD.ID_NhanVien, tblHD.ID_DoiTuong,
				 tblHD.LoaiHoaDon, tblHD.LoaiXuatKho, tblHD.DienGiai, tblHD.BienSo, tblHD.NguoiTao	
			)tblQD
			join DM_DonVi dv on tblQD.ID_DonVi = dv.ID
			join DM_LoaiChungTu ct on tblQD.LoaiHoaDon = ct.ID
			left join DM_DoiTuong dt on tblQD.ID_DoiTuong= dt.ID
			left join NS_NhanVien nv on tblQD.ID_NhanVien= nv.ID
			join DM_HangHoa hh on tblQD.ID_HangHoa= hh.ID
			join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
			left join DM_LoHang lo on tblQD.ID_LoHang= lo.ID and (lo.ID= tblQD.ID_LoHang or (tblQD.ID_LoHang is null and lo.ID is null))
			where hh.LaHangHoa = 1
			and hh.TheoDoi like @TheoDoi
			and qd.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID= allnhh.ID)
			AND ((select count(Name) from @tblSearchString b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa like '%'+b.Name+'%'
    		or lo.MaLoHang like '%' +b.Name +'%' 
			or qd.MaHangHoa like '%'+b.Name+'%'
			or qd.MaHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    		or qd.TenDonViTinh like '%'+b.Name+'%'
    		or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
			or dv.MaDonVi like '%'+b.Name+'%'
			or dv.TenDonVi like '%'+b.Name+'%'
			or nv.TenNhanVien like '%'+b.Name+'%'
			or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
			or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
			or nv.MaNhanVien like '%'+b.Name+'%'
			or dt.MaDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
			or tblQD.GhiChu like N'%'+b.Name+'%'
			or tblQD.MaHoaDon like N'%'+b.Name+'%'
			or tblQD.BienSo like N'%'+b.Name+'%'
			or tblQD.GhiChuUnsign like '%'+b.Name+'%'
			or tblQD.DienGiai like N'%'+b.Name+'%'
			or tblQD.DienGiaiUnsign like N'%'+b.Name+'%'
			)=@count or @count=0)
			order by tblQD.NgayLapHoaDon desc


END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_XuatDichVuDinhLuong]
    @SearchString [nvarchar](max),
    @LoaiChungTu [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	 DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	   		
    		where nd.ID = @ID_NguoiDung)

			select 
				ctsdQD.MaHoaDon,
				ctsdQD.NgayLapHoaDon,
				ctsdQD.TenLoaiChungTu as LoaiHoaDon,
				dv.TenDonVi,
				dv.MaDonVi,
				dt.MaDoiTuong,
				dt.TenDoiTuong,
				isnull(nv.TenNhanVien,'') as TenNhanVien,
				isnull(tn.MaPhieuTiepNhan,'') as MaPhieuTiepNhan,
				isnull(xe.BienSo,'') as BienSo,

				-- dichvu
				qddv.MaHangHoa as MaDichVu,
				qddv.TenDonViTinh as TenDonViDichVu,
				qddv.ThuocTinhGiaTri as ThuocTinhDichVu,
				hhdv.TenHangHoa as TenDichVu,
				ctsdQD.ID_DichVu,
				ctsdQD.SoLuongDichVu,
				ctsdQD.GiaBanDichVu,
				ctsdQD.ThanhTienDichVu,
				ctsdQD.GiaTriDichVu,
				ctsdQD.NVThucHiens,
				iif(ctsdQD.ThanhTienDichVu=0,100, isnull(ctsdQD.GiaTriDichVu,0)/ isnull(ctsdQD.ThanhTienDichVu,1) *100) as PtramSuDung,
				ISNULL(nhomDV.TenNhomHangHoa, N'Nhóm Dịch Vụ Mặc Định') as NhomDichVu,

				-- dinhluong
				ctsdQD.ID_DonViQuiDoi,						
				ctsdQD.SoLuongXuat as SoLuongThucTe,
				iif(@XemGiaVon='1', ctsdQD.GiaTriXuat,0) as GiaTriThucTe,
				ctsdQD.SoLuongDinhLuongBanDau,				
				iif(@XemGiaVon='1', ctsdQD.GiaTriDinhLuongBanDau,0) as GiaTriDinhLuongBanDau,
				qddl.MaHangHoa,
				qddl.TenDonViTinh,
				qddl.ThuocTinhGiaTri,
				ctsdQD.GhiChu,
				hhdl.TenHangHoa,
				concat(hhdl.TenHangHoa, qddl.ThuocTinhGiaTri) as TenHangHoaFull,
				ISNULL(nhomHH.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,


				ctsdQD.SoLuongXuat - ctsdQD.SoLuongDinhLuongBanDau as SoLuongChenhLech,
				iif(@XemGiaVon='1', ctsdQD.GiaTriXuat - ctsdQD.GiaTriDinhLuongBanDau,0) as GiaTriChenhLech,

				Case when ctsdQD.SoLuongXuat = 0 then N'Không xuất'
				when ctsdQD.SoLuongChenhLech < 0 then N'Xuất thiếu'
				when ctsdQD.SoLuongChenhLech = 0 then N'Xuất đủ'
				when (ctsdQD.SoLuongDinhLuongBanDau = 0) and ctsdQD.SoLuongXuat > 0 then N'Xuất thêm'
				else N'Xuất thừa' end as TrangThai

			from
			(select 
				ctsd.MaHoaDon,
				ctsd.NgayLapHoaDon,
				ctsd.ID_DonVi,
				ctsd.ID_NhanVien,
				ctsd.ID_PhieuTiepNhan,
				ctsd.ID_DoiTuong,
				ctsd.LoaiHoaDon,
				case ctsd.LoaiHoaDon
					when 2 then N'Xuất sử dụng gói dịch vụ'
					when 3 then N'Xuất bán dịch vụ định lượng'
					when 11 then N'Xuất sửa chữa'
					when 12 then N'Xuất bảo hành'
					else '' end TenLoaiChungTu,
				ctsd.SoLuongXuat * iif(qddl.TyLeChuyenDoi=0,1, qddl.TyLeChuyenDoi) as SoLuongXuat,
				ctsd.SoLuongDinhLuongBanDau * iif(qddl.TyLeChuyenDoi=0,1, qddl.TyLeChuyenDoi) as SoLuongDinhLuongBanDau,

				ctsd.SoLuongXuat- ctsd.SoLuongDinhLuongBanDau as SoLuongChenhLech,

				ctsd.ID_DonViQuiDoi,
				ctsd.ID_DichVu,
				ctsd.SoLuongDichVu,
				ctsd.GiaBanDichVu,
				ctsd.ThanhTienDichVu,
				ctsd.GiaTriDichVu,
				ctsd.GiaTriXuat,
				ctsd.NVThucHiens,
				ctsd.GiaTriDinhLuongBanDau,
				ctsd.GhiChu,
				qddl.ID_HangHoa

			from(

			select 
				a.MaHoaDon,
				a.ID_DonVi,
				a.ID_NhanVien,
				a.ID_DoiTuong,
				a.NgayLapHoaDon,
				a.ID_PhieuTiepNhan,
				a.LoaiHoaDon,
				a.ID_DichVu, 
				a.SoLuongDichVu,
				a.GiaBanDichVu,
				a.ThanhTienDichVu,
				a.GiaTriDichVu,		
				a.NVThucHiens,		
				max(SoLuongDinhLuongBanDau) as SoLuongDinhLuongBanDau,			
				max(GiaTriDinhLuongBanDau) as GiaTriDinhLuongBanDau,			
				sum(isnull(SoLuongXuat, 0)) as SoLuongXuat,
				sum(isnull(GiaTriXuat, 0)) as GiaTriXuat,
				(a.ID_DonViQuiDoi) as ID_DonViQuiDoi,
				max(isnull(a.GhiChu,'')) as GhiChu
			from
			(
					--- xuatban, xuatsudung
					select 
						hd.MaHoaDon,
						hd.ID_DonVi,
						hd.ID_NhanVien,
						hd.ID_DoiTuong,
						hd.NgayLapHoaDon,
						hd.ID_PhieuTiepNhan,
    					ctdl.ID_DonViQuiDoi,
						ctdl.ID_LoHang,
						ctdl.GhiChu,			
						case hd.LoaiHoaDon
							when 25 then 11 -- xuat sua chua
							when 8 then 11
							when 2 then 12 -- xuat baohanh							
						else case  when ctdl.ID_ChiTietGoiDV is not null
							then case when hd.ID_HoaDon is null then 2 else 3 end
						when ctdl.ID_ChiTietGoiDV is null and ctdl.ID_ChiTietDinhLuong is not null then 3 
						else hd.LoaiHoaDon end end as LoaiHoaDon,

						ctmua.ID_DonViQuiDoi as ID_DichVu,		
						ISNULL(ctmua.SoLuong,0) AS SoLuongDichVu,
						isnull(ctmua.DonGia,0) - isnull(ctmua.TienChietKhau,0)  AS GiaBanDichVu,
						(isnull(ctmua.DonGia,0) - isnull(ctmua.TienChietKhau,0)) *  ISNULL(ctmua.SoLuong,0) as ThanhTienDichVu,
						ISNULL(ctmua.SoLuong,0)* ctmua.GiaVon AS GiaTriDichVu,
						
    					iif(hd.LoaiHoaDon=25,isnull(xkhdsc.SoLuongXuat,0), ISNULL(ctdl.SoLuong,0)) AS SoLuongXuat,
						iif(hd.LoaiHoaDon= 25,isnull(xkhdsc.GiaTriXuat,0), ISNULL(ctdl.SoLuong,0) * ctdl.GiaVon) AS GiaTriXuat,
						

						iif(hd.LoaiHoaDon=25,isnull(ctdl.SoLuong,0),ISNULL(ctdl.SoLuongDinhLuong_BanDau,0)) AS SoLuongDinhLuongBanDau,
						iif(hd.LoaiHoaDon=25,isnull(ctdl.SoLuong,0),ISNULL(ctdl.SoLuongDinhLuong_BanDau,0)) * ctdl.GiaVon AS GiaTriDinhLuongBanDau,					
						isnull(nvth.NVThucHiens,'') as NVThucHiens,
						0 as LaDinhLuongBoSung
					from BH_HoaDon_ChiTiet ctdl
					join BH_HoaDon hd on ctdl.ID_HoaDon = hd.ID 
					left join BH_HoaDon_ChiTiet ctmua on ctmua.ID = ctdl.ID_ChiTietDinhLuong
					left join(
							select distinct thout.ID_ChiTietHoaDon,
							(
								select nv.TenNhanVien +', ' as [text()]
								from Bh_NhanVienThucHien th
								join BH_HoaDon_ChiTiet ct on th.ID_ChiTietHoaDon= ct.ID
								join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
								join NS_NhanVien nv on th.ID_NhanVien = nv.ID
								where hd.ChoThanhToan='0'
								and hd.NgayLapHoaDon between @timeStart and @timeEnd
								and hd.LoaiHoaDon in ( 1,25,2)
								and th.ID_ChiTietHoaDon= thout.ID_ChiTietHoaDon
								 For XML PATH ('')
							) NVThucHiens 
						from Bh_NhanVienThucHien thout where thout.ID_ChiTietHoaDon is not null
					) nvth on ctmua.ID= nvth.ID_ChiTietHoaDon
					left join
					(
						select 
						hd.ID_HoaDon,
						ctxk.ID_ChiTietGoiDV,
						ctxk.ID_DonViQuiDoi,
						max(isnull(ctxk.GhiChu,'')) as GhiChu,					
						sum(ISNULL(ctxk.SoLuong,0)) AS SoLuongXuat,
						sum(ISNULL(ctxk.SoLuong,0)* ctxk.GiaVon) AS GiaTriXuat				
						from BH_HoaDon_ChiTiet ctxk
						join BH_HoaDon hd on ctxk.ID_HoaDon = hd.ID 
						where hd.ChoThanhToan='0' 
						and hd.LoaiHoaDon = 8
						group by hd.ID_HoaDon, ctxk.ID_ChiTietGoiDV,  ctxk.ID_DonViQuiDoi
					) xkhdsc on hd.ID= xkhdsc.ID_HoaDon and xkhdsc.ID_ChiTietGoiDV= ctdl.ID 
					where hd.ChoThanhToan='0' 
					and hd.LoaiHoaDon in ( 1,25,2)
					and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
					AND ctdl.ID_ChiTietDinhLuong is not null -- thành phần định lượng
					AND ctdl.ID_ChiTietDinhLuong != ctdl.ID		

					---- get dinhluong them vao khi tao phieu xuatkho sua chua (ID_ChiTietGoiDV la dichvu)
							union all					

							select 
								hdm.MaHoaDon,
								hdm.ID_DonVi,
								hdm.ID_NhanVien,
								hdm.ID_DoiTuong,
								hdm.NgayLapHoaDon,
								hdm.ID_PhieuTiepNhan,
								ctxkThem.ID_DonViQuiDoi,
								ctxkThem.ID_LoHang,
								ctxkThem.GhiChu,					 
    							11 as LoaiHoaDon,
    			
								ctm.ID_DonViQuiDoi as ID_DichVu,		
								ISNULL(ctm.SoLuong,0) AS SoLuongDichVu,
								isnull(ctm.DonGia,0) - isnull(ctm.TienChietKhau,0)  AS GiaBanDichVu,
								isnull(ctm.ThanhTien,0) AS ThanhTienDichVu,
								ISNULL(ctm.SoLuong,0)* isnull(ctm.GiaVon,0) AS GiaTriDichVu,

    							isnull(ctxkThem.SoLuong,0) AS SoLuongXuat,
								isnull(ctxkThem.SoLuong * ctxkThem.GiaVon,0) AS GiaTriXuat,

								0 AS SoLuongDinhLuongBanDau,
								0 AS GiaTriDinhLuongBanDau,
								'' as NVThucHiens,
								1 as LaDinhLuongBoSung
				
							from BH_HoaDon_ChiTiet ctm
							join BH_HoaDon hdm on ctm.ID_HoaDon= hdm.ID
							left join BH_HoaDon_ChiTiet ctxkThem on ctm.ID = ctxkThem.ID_ChiTietGoiDV
							left join BH_HoaDon hdxk on ctxkThem.ID_HoaDon= hdxk.ID and hdm.ID= hdxk.ID_HoaDon
							where hdm.LoaiHoaDon= 25 and hdxk.LoaiHoaDon= 8
							and hdxk.ChoThanhToan='0'
							and hdm.ChoThanhToan='0'
							and ctm.ID = ctm.ID_ChiTietDinhLuong --- chi get dichvu			
							and hdm.NgayLapHoaDon >= @timeStart and hdm.NgayLapHoaDon < @timeEnd
							and exists (select id from @tblIdDonVi dv2 where hdm.ID_DonVi= dv2.ID)
				) a group by
						a.MaHoaDon,
						a.ID_DonVi,
						a.ID_NhanVien,a.ID_DoiTuong,
						a.NgayLapHoaDon,
						a.ID_PhieuTiepNhan,
						a.LoaiHoaDon,
						a.ID_DichVu, a.SoLuongDichVu, a.GiaTriDichVu ,a.GiaBanDichVu,a.ThanhTienDichVu,	a.NVThucHiens,			
						a.ID_DonViQuiDoi, a.ID_LoHang

		) ctsd
		join DonViQuiDoi qddl on ctsd.ID_DonViQuiDoi = qddl.ID
		) ctsdQD
		left join DonViQuiDoi qddv on ctsdQD.ID_DichVu = qddv.ID
		left join DonViQuiDoi qddl on ctsdQD.ID_HangHoa = qddl.ID_HangHoa and qddl.LaDonViChuan= 1
		left join DM_DonVi dv on ctsdQD.ID_DonVi = dv.ID
		left join DM_DoiTuong dt on ctsdQD.ID_DoiTuong = dt.ID
		left join DM_HangHoa hhdl on qddl.ID_HangHoa = hhdl.ID
		left join DM_HangHoa hhdv on qddv.ID_HangHoa = hhdv.ID 
		left join DM_NhomHangHoa nhomHH on hhdl.ID_NhomHang= nhomHH.ID
		left join DM_NhomHangHoa nhomDV on hhdv.ID_NhomHang= nhomDV.ID
		left join NS_NhanVien nv on ctsdQD.ID_NhanVien= nv.ID	
		left join Gara_PhieuTiepNhan tn on ctsdQD.ID_PhieuTiepNhan= tn.ID
		left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
    	where exists (select Name from splitstring(@LoaiChungTu) ct where ctsdQD.LoaiHoaDon = ct.Name ) 				
    		and
			hhdv.TheoDoi like @TheoDoi			
			and qddv.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhomDV.ID = allnhh.ID )
			and exists (select id from @tblIdDonVi dv2 where dv.ID= dv2.ID)
			and
			 ((select count(Name) from @tblSearchString b where 
    			ctsdQD.MaHoaDon like '%'+b.Name+'%' 
				or ctsdQD.GhiChu like '%'+b.Name+'%'
    			or qddl.MaHangHoa like '%'+b.Name+'%' 
    			or hhdv.TenHangHoa like '%'+b.Name+'%'
    			or hhdv.TenHangHoa_KhongDau like '%' +b.Name +'%' 
				or hhdl.TenHangHoa like '%'+b.Name+'%'
    			or hhdl.TenHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhomDV.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhomHH.TenNhomHangHoa like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    			or qddv.MaHangHoa like '%'+b.Name+'%' 
    			or TenNhanVien like '%'+b.Name+'%'    
				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'    
				or xe.BienSo like '%'+b.Name+'%' 
				)=@count or @count=0)
		ORDER BY NgayLapHoaDon DESC, qddv.MaHangHoa
END");

			Sql(@"ALTER PROCEDURE [dbo].[DanhMucKhachHang_CongNo_Paging]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @MaKH [nvarchar](max),
    @LoaiKH [int],
    @ID_NhomKhachHang [nvarchar](max),
    @timeStartKH [datetime],
    @timeEndKH [datetime],
    @CurrentPage [int],
    @PageSize [float],
    @Where [nvarchar](max),
    @SortBy [nvarchar](100)
AS
BEGIN
    set nocount on
    	
    	if @SortBy ='' set @SortBy = ' dt.NgayTao DESC'
    	if @Where='' set @Where= ''
    	else set @Where= ' AND '+ @Where 
    
    	declare @from int= @CurrentPage * @PageSize + 1  
    	declare @to int= (@CurrentPage + 1) * @PageSize 
    
    	declare @sql1 nvarchar(max)= concat('
    	declare @tblIDNhoms table (ID varchar(36)) 
    	declare @idNhomDT nvarchar(max) = ''', @ID_NhomKhachHang, '''
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh select * from splitstring(''',@ID_ChiNhanh, ''')
    	
    	if @idNhomDT =''%%''
    		begin
				insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')

    			-- check QuanLyKHTheochiNhanh
    			--declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))  
				
				declare @QLTheoCN bit = 0;
				declare @countQL int=0;
				select distinct QuanLyKhachHangTheoDonVi into #temp from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)
				set @countQL = (select COUNT(*) from #temp)
				if	@countQL= 1 
						set @QLTheoCN = (select QuanLyKhachHangTheoDonVi from #temp)
				
    			if @QLTheoCN = 1
    				begin									
    					insert into @tblIDNhoms(ID)
    					select *  from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = ',@LoaiKH ,'
    						union all
    						-- get Nhom at this ChiNhanh
    						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) ) tbl
    				end
    			else
    				begin				
    				-- insert all
    				insert into @tblIDNhoms(ID)
    				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    				where LoaiDoiTuong = ',@LoaiKH, ' 
    				end		
    		end
    	else
    		begin
    			set @idNhomDT = REPLACE( @idNhomDT,''%'','''')
    			insert into @tblIDNhoms(ID) values (@idNhomDT)
    		end
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](''',@MaKH,''', '' '') where Name!='''';
    	Select @count =  (Select count(*) from @tblSearchString);')
    
    	declare @sql2 nvarchar(max)= concat(' WITH Data_CTE ',
    									' AS ',
    									' ( ',
    
    N'SELECT  * 
    		FROM
    		(  
    			SELECT 
    		  dt.ID as ID,
    		  dt.MaDoiTuong, 
    			  case when dt.IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' 
				  else  ISNULL(dt.IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
    		  dt.TenDoiTuong_KhongDau,
    		  dt.TenDoiTuong_ChuCaiDau,
    			  dt.ID_TrangThai,
				   dt.TheoDoi,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
			   dt.NgayGiaoDichGanNhat,
    			  ISNULL(dt.DienThoai,'''') as DienThoai,
    			  ISNULL(dt.Email,'''') as Email,
    			  ISNULL(dt.DiaChi,'''') as DiaChi,
    			  ISNULL(dt.MaSoThue,'''') as MaSoThue,
				  dt.TaiKhoanNganHang,
    		  ISNULL(dt.GhiChu,'''') as GhiChu,
    		  dt.NgayTao,
    		  dt.DinhDang_NgaySinh,
    		  ISNULL(dt.NguoiTao,'''') as NguoiTao,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    			  dt.ID_DonVi, --- Collate Vietnamese_CI_AS
    		  dt.LaCaNhan,
    		  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
    			 case when right(rtrim(dt.TenNhomDoiTuongs),1) ='','' then LEFT(Rtrim(dt.TenNhomDoiTuongs),
				  len(dt.TenNhomDoiTuongs)-1) else				  
				  ISNULL(dt.TenNhomDoiTuongs, N''Nhóm mặc định'') end   as TenNhomDT,-- remove last coma
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
    			ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
			  a.PhiDichVu,
    			  CAST(0 as float) as TongNapThe , 
    			  CAST(0 as float) as SuDungThe , 
    			  CAST(0 as float) as HoanTraTheGiaTri , 
    			  CAST(0 as float) as SoDuTheGiaTri , 
    			  concat(dt.MaDoiTuong,'' '',lower(dt.MaDoiTuong) ,'' '', dt.TenDoiTuong,'' '', dt.DienThoai,'' '', dt.TenDoiTuong_KhongDau)  as Name_Phone			
    		  FROM DM_DoiTuong dt  ','')
    	
    	declare @sql3 nvarchar(max)= concat('
    				LEFT JOIN (
    					SELECT tblThuChi.ID_DoiTuong,
    						SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.HoanTraThe,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) + sum(ISNULL(tblThuChi.ThuTuThe,0))
							- sum(isnull(tblThuChi.PhiDichVu,0)) 
						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    					SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
						sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    					SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    					SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang,
						sum(isnull(tblThuChi.PhiDichVu,0)) as PhiDichVu
    				FROM
    				(
    					select 
							cp.ID_NhaCungCap as ID_DoiTuong,
							0 as GiaTriTra,
    						0 as DoanhThu,
							0 AS TienThu,
    						0 AS TienChi, 
    						0 AS SoLanMuaHang,
							0 as ThuTuThe,
							sum(cp.ThanhTien) as PhiDichVu,
							0 as HoanTraThe --- hoantra lai sodu co trong TGT cho khach
						from BH_HoaDon_ChiPhi cp
						join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
						where hd.ChoThanhToan = 0
						and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
						group by cp.ID_NhaCungCap

						union all

						---- hoantra sodu TGT cho khach (giam sodu TGT)
						SELECT 
    							bhd.ID_DoiTuong,    	
								0 as GiaTriTra,
    							0 as DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								-sum(bhd.PhaiThanhToan) as HoanTraThe
    					FROM BH_HoaDon bhd
						where bhd.LoaiHoaDon = 32 and bhd.ChoThanhToan = 0 
						and exists (select * from @tblChiNhanh cn where bhd.ID_DonVi= cn.ID)
						group by bhd.ID_DoiTuong

						union all
    						---- tongban
    						SELECT 
    							bhd.ID_DoiTuong,    	
								0 as GiaTriTra,
    							bhd.PhaiThanhToan as DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,25) AND bhd.ChoThanhToan = ''0'' 
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 

							
							union all
							---- doanhthu tuthe
							select 
								bhd.ID_DoiTuong,
								0 as GiaTriTra,
    							0 AS DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								bhd.PhaiThanhToan as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe
							from BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon = 22 AND bhd.ChoThanhToan = ''0'' 
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 						

						
    							 union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,
								bhd.PhaiThanhToan AS GiaTriTra,    							
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe
    						FROM BH_HoaDon bhd   						
    						WHERE bhd.LoaiHoaDon in (6,4, 13,14) AND bhd.ChoThanhToan = ''0''  
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,		
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
							   							
    							 union all
    
    							-- tienthu
    							SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon   						
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.HinhThucThanhToan!= 6
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
    						AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID) 
    							AND qhd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd  ,
    							
    							''' union all
    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon 							
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.HinhThucThanhToan!= 6
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
    							AND qhd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd  ,
    						''' AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)' )
    
    declare @sql4 nvarchar(max)= concat(' Union All
    							-- solan mua hang
    						Select 
    							hd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    								0 as TienChi,
    							COUNT(*) AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe
    						From BH_HoaDon hd 
    						where hd.LoaiHoaDon in (1,19,25)
    						and hd.ChoThanhToan = 0
    						AND hd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd ,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    							 GROUP BY hd.ID_DoiTuong  	
    							)AS tblThuChi
    						GROUP BY tblThuChi.ID_DoiTuong
    				) a on dt.ID = a.ID_DoiTuong  					
    						WHERE  dt.loaidoituong =', @loaiKH  ,					
    						' and dt.NgayTao between ''',@timeStartKH, ''' and ''',@timeEndKH,
    							''' and ( MaDoiTuong like ''%', @MaKH, '%'' OR  TenDoiTuong like ''%',@MaKH, '%'' or TenDoiTuong_KhongDau like ''%',@MaKH, '%'' or DienThoai like ''%',@MaKH, '%'' or Email like ''%',@MaKH, '%'' )',
    						
    						  '', @Where, ')b
    				 where b.ID not like ''%00000000-0000-0000-0000-0000%''
    				 and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    				  ),
    			Count_CTE ',
    			' AS ',
    			' ( ',
    			' SELECT COUNT(*) AS TotalRow, CEILING(COUNT(*) / CAST(',@PageSize, ' as float )) as TotalPage ,
					SUM(TongBan) as TongBanAll,
					SUM(TongBanTruTraHang) as TongBanTruTraHangAll,
					SUM(TongTichDiem) as TongTichDiemAll,
					SUM(NoHienTai) as NoHienTaiAll,
					SUM(PhiDichVu) as TongPhiDichVu
					FROM Data_CTE ',
    			' ) ',
    			' SELECT dt.*, cte.TotalPage, cte.TotalRow, 
					cte.TongBanAll, 
					cte.TongBanTruTraHangAll,
					cte.TongTichDiemAll,
					cte.NoHienTaiAll,
					cte.TongPhiDichVu,
    				  ISNULL(trangthai.TenTrangThai,'''') as TrangThaiKhachHang,
    				  ISNULL(qh.TenQuanHuyen,'''') as PhuongXa,
    				  ISNULL(tt.TenTinhThanh,'''') as KhuVuc,
    				  ISNULL(dv.TenDonVi,'''') as ConTy,
    				  ISNULL(dv.SoDienThoai,'''') as DienThoaiChiNhanh,
    				  ISNULL(dv.DiaChi,'''') as DiaChiChiNhanh,
    				  ISNULL(nk.TenNguonKhach,'''') as TenNguonKhach,
    				  ISNULL(dt2.TenDoiTuong,'''') as NguoiGioiThieu,
					  ISNULL(nvpt.MaNhanVien,'''') as MaNVPhuTrach,
					 ISNULL(nvpt.TenNhanVien,'''') as TenNhanVienPhuTrach',
    			' FROM Data_CTE dt',
    			' CROSS JOIN Count_CTE cte',
    			' LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID ',
    		' LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID ',
    			' LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID ',
    			' LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID ',
				' LEFT join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID ',
    		' LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID ',
    			' LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID ',
    			' ORDER BY ',@SortBy,
    			' OFFSET (', @CurrentPage, ' * ', @PageSize ,') ROWS ',
    			' FETCH NEXT ', @PageSize , ' ROWS ONLY ')
    
    			print @sql1
				print @sql2
				print @sql3
				print @sql4


    			exec (@sql1 + @sql2 + @sql3 + @sql4)
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

			Sql(@"ALTER PROCEDURE [dbo].[GetChiTietHoaDon_ByIDHoaDon]
    @ID_HoaDon [uniqueidentifier]	
AS
BEGIN
  set nocount on;

  declare @ID_DonVi uniqueidentifier, @loaiHD int, @ID_HoaDonGoc uniqueidentifier		
	select top 1 @ID_DonVi= ID_DonVi, @ID_HoaDonGoc= ID_HoaDon, @loaiHD= LoaiHoaDon from BH_HoaDon where ID= @ID_HoaDon

  if @loaiHD= 8
		begin
		select 
			ctxk.ID,ctxk.ID_DonViQuiDoi,ctxk.ID_LoHang,
			ctxk.SoLuong,
			ctxk.SoLuong as SoLuongXuatHuy,
			ctxk.DonGia,
			ctxk.GiaVon, 
			ctxk.GiaTriHuy as ThanhTien, 
			ctxk.GiaTriHuy as ThanhToan, 
			ctxk.TienChietKhau as GiamGia,
			ctxk.GhiChu,
			cast(ctxk.SoThuTu as float) as SoThuTu,
			hd.MaHoaDon,
			hd.NgayLapHoaDon,
			hd.ID_NhanVien,
    		nv.TenNhanVien,
			lh.NgaySanXuat,
    		lh.NgayHetHan,    			
    		dvqd.MaHangHoa,
    		hh.TenHangHoa,
			hh.TenHangHoa as TenHangHoaThayThe,
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
			ct.ID_DonViQuiDoi,
			ct.ID_LoHang,
			@ID_HoaDon as ID_HoaDon,
			sum(ct.SoLuong) as SoLuong, 
			max(ct.DonGia) as DonGia,
			max(ct.DonGia) as GiaVon,
			sum(ct.SoLuong * ct.DonGia) as GiaTriHuy,			
			max(ct.TienChietKhau) as TienChietKhau,
			max(ct.GhiChu) as GhiChu
		from BH_HoaDon_ChiTiet ct
		where ct.ID_HoaDon= @ID_HoaDon		
		and (ct.ChatLieu is null or ct.ChatLieu!='5')
		group by ct.ID_DonViQuiDoi, ct.ID_LoHang		
		)ctxk
		join BH_HoaDon hd on hd.ID= ctxk.ID_HoaDon
		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_DonVi)
		where (hh.LaHangHoa = 1 and tk.TonKho is not null)
		end
	else
	begin
		if @loaiHD in (1,3,2,25) ---- 1.hoadonle, 2.hoadon baohanh, 3. baogia, 25. hoadon suachua
		begin
			select ctsd.ID_ChiTietGoiDV, sum(SoLuong) as SoLuongSuDung
			into #tblSDDV 
			from BH_HoaDon_ChiTiet ctsd
			join BH_HoaDon hd on ctsd.ID_HoaDon= hd.ID
			where exists (select ID from BH_HoaDon_ChiTiet ct where ct.ID_HoaDon= @ID_HoaDon and ct.ID_ChiTietGoiDV =  ctsd.ID_ChiTietGoiDV)
			and hd.ChoThanhToan= 0
			AND (ctsd.ID_ChiTietDinhLuong IS NULL OR ctsd.ID_ChiTietDinhLuong = ctsd.ID) --- khong get tpdinhluong khi sudung GDV
			group by ctsd.ID_ChiTietGoiDV

					select DISTINCT tbl.*, 
					isnull(hdXK.SoLuongXuat,0) as SoLuongXuat,
					isnull(hdmua.SoLuongMua,0) as SoLuongMua,
					isnull(hdmua.SoLuongMua,0) - isnull(hdmua.SoLuongDVDaSuDung,0) as SoLuongDVConLai,
					isnull(hdmua.SoLuongDVDaSuDung,0) as SoLuongDVDaSuDung
					FROM 
						 (SELECT
    							cthd.ID,cthd.ID_HoaDon,DonGia,cthd.GiaVon,SoLuong,ThanhTien,ThanhToan,cthd.ID_DonViQuiDoi, cthd.ID_ChiTietDinhLuong, cthd.ID_ChiTietGoiDV,
    							cthd.TienChietKhau AS GiamGia,PTChietKhau,cthd.GhiChu,cthd.TienChietKhau,
    							(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
								qd.GiaBan as GiaBanHH, ---- used to nhaphang from hoadon
    							CAST(SoThuTu AS float) AS SoThuTu,cthd.ID_KhuyenMai, ISNULL(cthd.TangKem,'0') as TangKem, cthd.ID_TangKem,
								-- replace char enter --> char space
    							(REPLACE(REPLACE(TenHangHoa,CHAR(13),''),CHAR(10),'') +
    							CASE WHEN (qd.ThuocTinhGiaTri is null or qd.ThuocTinhGiaTri = '') then '' else '_' + qd.ThuocTinhGiaTri end +
    							CASE WHEN TenDonVitinh = '' or TenDonViTinh is null then '' else ' (' + TenDonViTinh + ')' end +
    							CASE WHEN MaLoHang is null then '' else '. Lô: ' + MaLoHang end) as TenHangHoaFull,
    				
    							hh.ID AS ID_HangHoa,
								hh.LaHangHoa,
								hh.QuanLyTheoLoHang,
								hh.TenHangHoa, 
								isnull(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
								ISNULL(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
								ISNULL(ID_NhomHang,'00000000-0000-0000-0000-000000000000') as ID_NhomHangHoa,	
    							TenDonViTinh,MaHangHoa,YeuCau,
    							lo.ID AS ID_LoHang,
								ISNULL(MaLoHang,'') as MaLoHang,
								lo.NgaySanXuat, lo.NgayHetHan,
								qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
								ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, 
								CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
								CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
								CAST(ISNULL(cthd.PTThue,0) as float) as PTThue,
								CAST(ISNULL(cthd.TienThue,0) as float) as TienThue,
								CAST(ISNULL(cthd.ThoiGianBaoHanh,0) as float) as ThoiGianBaoHanh,
								CAST(ISNULL(cthd.LoaiThoiGianBH,0) as float) as LoaiThoiGianBH,
								Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
								Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end LaPTPhiDichVu,
								CAST(0 as float) as TongPhiDichVu, -- set default PhiDichVu = 0 (caculator again .js)
								CAST(ISNULL(cthd.Bep_SoLuongYeuCau,0) as float) as Bep_SoLuongYeuCau,
								CAST(ISNULL(cthd.Bep_SoLuongHoanThanh,0) as float) as Bep_SoLuongHoanThanh, -- view in CTHD NhaHang
								CAST(ISNULL(cthd.Bep_SoLuongChoCungUng,0) as float) as Bep_SoLuongChoCungUng,
								ISNULL(hh.SoPhutThucHien,0) as SoPhutThucHien, -- lay so phut theo cai dat
								ISNULL(cthd.ThoiGianThucHien,0)  as ThoiGianThucHien,-- sophut thuc te thuchien	
								ISNULL(cthd.QuaThoiGian,0)  as QuaThoiGian,
				
								case when hh.LaHangHoa='0' then 0 else ISNULL(tk.TonKho,0) end as TonKho,
								cthd.ID_ViTri,
								ISNULL(vt.TenViTri,'') as TenViTri,			
								ThoiGian,cthd.ThoiGianHoanThanh, ISNULL(hh.GhiChu,'') as GhiChuHH,
								ISNULL(cthd.DiemKhuyenMai,0) as DiemKhuyenMai,
								ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
								ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
								ISNULL(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
								ISNULL(hh.ChietKhauMD_NVTheoPT,'0') as ChietKhauMD_NVTheoPT,
								cthd.ChatLieu,
								isnull(cthd.DonGiaBaoHiem,0) as DonGiaBaoHiem,
								iif(cthd.TenHangHoaThayThe is null or cthd.TenHangHoaThayThe ='',hh.TenHangHoa, cthd.TenHangHoaThayThe) as TenHangHoaThayThe,					
								cthd.ID_LichBaoDuong,
								iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
								cthd.ID_ParentCombo,
								qd.GiaNhap
					
    					FROM BH_HoaDon hd
    					JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    					JOIN DonViQuiDoi qd ON cthd.ID_DonViQuiDoi = qd.ID
    					JOIN DM_HangHoa hh ON qd.ID_HangHoa= hh.ID    		
    					left JOIN DM_NhomHangHoa nhh ON hh.ID_NhomHang= nhh.ID    							
    					LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
						left join DM_HangHoa_TonKho tk on cthd.ID_DonViQuiDoi= tk.ID_DonViQuyDoi and tk.ID_DonVi= @ID_DonVi
						left join DM_ViTri vt on cthd.ID_ViTri= vt.ID
    					-- chi get CT khong phai la TP dinh luong
    					WHERE cthd.ID_HoaDon = @ID_HoaDon
								and (cthd.ChatLieu is null or cthd.ChatLieu!='5')
								AND (cthd.ID_ChiTietDinhLuong IS NULL OR cthd.ID_ChiTietDinhLuong = cthd.ID)
								and ((tk.ID_DonVi = hd.ID_DonVi and hh.LaHangHoa='1') 
								or tk.ID_DonVi is null
								or (hh.LaHangHoa='0'))
								and (cthd.ID_LoHang= tk.ID_LoHang OR (cthd.ID_LoHang is null and tk.ID_LoHang is null)) 
								and (cthd.ID_ParentCombo IS NULL OR cthd.ID_ParentCombo = cthd.ID) --- khong get tpcombo
						) tbl
						left join
						(
							select ctm.ID as ID_ChiTietGoiDV, ctm.SoLuong as SoLuongMua, isnull(ctsd.SoLuongSuDung,0) as SoLuongDVDaSuDung
							from BH_HoaDon_ChiTiet ctm
							join #tblSDDV ctsd  on ctm.ID= ctsd.ID_ChiTietGoiDV			
						) hdmua on tbl.ID_ChiTietGoiDV = hdmua.ID_ChiTietGoiDV
						left join 
						(
						--- soluongxuatkho
							select SUM(ctxk.SoLuong) as SoLuongXuat, ctxk.ID_ChiTietGoiDV
							from BH_HoaDon_ChiTiet ctxk 
							join BH_HoaDon hdxk on ctxk.ID_HoaDon = hdxk.ID
							where hdxk.ID_HoaDon = @ID_HoaDon
							and hdxk.LoaiHoaDon = 8 and hdxk.ChoThanhToan='0'
							group by ctxk.ID_ChiTietGoiDV			
						) hdXK on tbl.ID = hdXK.ID_ChiTietGoiDV 
						order by tbl.SoThuTu
		end
		else
			if @loaiHD= 4 and @ID_HoaDonGoc is not null
			begin	
				SELECT 
    				cthd.ID,
					cthd.ID_HoaDon, 
					cthd.ID_ParentCombo,
					cthd.ID_ChiTietDinhLuong,
					cthd.ID_ChiTietGoiDV, ---- used to update cthd (check nhapmua from PO)
					cthd.DonGia, 
					cthd.GiaVon, 
					isnull(cthd.TonLuyKe,0) as TonLuyKe, --- tonkho tai thoidiem xxx cua {NgayLapHoaDon}: used to print PO
					cast(cthd.SoThuTu as float) as SoThuTu,
					SoLuong, 
					isnull(ctConLai.SoLuongConLai,0) as SoLuongConLai,
					cthd.ThanhTien, 
					TienChietKhau, 
					cthd.ThanhToan, 
					cthd.TienThue, 
					isnull(cthd.PTThue,0) as PTThue, 
					dvqd.ID as ID_DonViQuiDoi,
    				dvqd.ID_HangHoa, dvqd.TenDonViTinh, dvqd.MaHangHoa,
					TienChietKhau as GiamGia, PTChietKhau,
					(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
					hd.NgayLapHoaDon as ThoiGian, cthd.GhiChu,
    				cthd.ID_KhuyenMai,			
					lo.NgaySanXuat, lo.NgayHetHan,
    				dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
					concat(TenHangHoa ,
    				dvqd.ThuocTinhGiaTri, 
    				Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lo.MaLoHang is null then '' else '. Lô: ' + lo.MaLoHang end) as TenHangHoaFull,
    				LaHangHoa, QuanLyTheoLoHang,
					TenHangHoa,		
					hh.TenHangHoa as TenHangHoaThayThe,
    				TyLeChuyenDoi, YeuCau,
    				lo.ID AS ID_LoHang, ISNULL(MaLoHang,'') as MaLoHang, 			
					ISNULL(hhtonkho.TonKho, 0) as TonKho, 
					hd.ID_DonVi, dvqd.GiaNhap, 
					dvqd.GiaBan as GiaBanMaVach, hh.ID_NhomHang as ID_NhomHangHoa,
					dvqd.LaDonViChuan, hh.ChiPhiThucHien as PhiDichVu, cast(ISNULL(hh.QuyCach,1) as float) as QuyCach, 
					Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
					dvqd.GiaBan, dvqd.GiaBan as GiaBanHH, -- use to get banggiachung  of cthd (at NhapHangChiTiet),
					ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
					ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
					ISNULL(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
					ISNULL(hh.ChietKhauMD_NVTheoPT,'0') as ChietKhauMD_NVTheoPT,
					ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
					cthd.ID_LichBaoDuong,
					iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa
    			FROM BH_HoaDon hd
    			JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
				left join
				(
						----- get sl conlai saukhi xuly nhapmua
					select ctpo.ID, ctpo.SoLuong - isnull(ctXL.SoLuong,0) as SoLuongConLai
					from BH_HoaDon_ChiTiet ctpo
					left join
					(
						---- ctxuly != hd current
						select ctxl.SoLuong, ctxl.ID_ChiTietGoiDV
						from BH_HoaDon_ChiTiet ctxl
						join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
						where hdxl.ID_HoaDon= @ID_HoaDonGoc and hdxl.ChoThanhToan='0' and hdxl.LoaiHoaDon= 4
						and hdxl.ID != @ID_HoaDon
					) ctXL on ctpo.ID= ctXL.ID_ChiTietGoiDV
				) ctConLai on cthd.ID_ChiTietGoiDV= ctConLai.ID
    			JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    			JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
    			LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi
				and (hhtonkho.ID_LoHang = cthd.ID_LoHang or cthd.ID_LoHang is null) and hhtonkho.ID_DonVi = @ID_DonVi				
    			WHERE cthd.ID_HoaDon = @ID_HoaDon 
				and (cthd.ID_ChiTietDinhLuong = cthd.ID or cthd.ID_ChiTietDinhLuong is null)
				and (cthd.ID_ParentCombo IS NULL OR cthd.ID_ParentCombo = cthd.ID)
				and (cthd.ChatLieu is null or cthd.ChatLieu!='5')
				order by cthd.SoThuTu desc			
			end
			else

			begin
				SELECT 
    			cthd.ID,
				cthd.ID_HoaDon, 
				cthd.ID_ParentCombo,
				cthd.ID_ChiTietDinhLuong,
				cthd.ID_ChiTietGoiDV, ---- used to update cthd (check nhapmua from PO)
				cthd.DonGia, 
				cthd.GiaVon, 
				isnull(cthd.TonLuyKe,0) as TonLuyKe, --- tonkho tai thoidiem xxx cua {NgayLapHoaDon}: used to print PO
				cast(cthd.SoThuTu as float) as SoThuTu,
				SoLuong, 
				cthd.ThanhTien, 
				TienChietKhau, 
				cthd.ThanhToan, 
				cthd.TienThue, 
				isnull(cthd.PTThue,0) as PTThue, 
				dvqd.ID as ID_DonViQuiDoi,
    			dvqd.ID_HangHoa, dvqd.TenDonViTinh, dvqd.MaHangHoa,
				TienChietKhau as GiamGia, PTChietKhau,
				(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
				hd.NgayLapHoaDon as ThoiGian, cthd.GhiChu,
    			cthd.ID_KhuyenMai,			
				lo.NgaySanXuat, lo.NgayHetHan,
    			dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				concat(TenHangHoa ,
    			dvqd.ThuocTinhGiaTri, 
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    			Case when lo.MaLoHang is null then '' else '. Lô: ' + lo.MaLoHang end) as TenHangHoaFull,
    			LaHangHoa, QuanLyTheoLoHang,
				TenHangHoa,		
				hh.TenHangHoa as TenHangHoaThayThe,
    			TyLeChuyenDoi, YeuCau,
    			lo.ID AS ID_LoHang, ISNULL(MaLoHang,'') as MaLoHang, 			
				ISNULL(hhtonkho.TonKho, 0) as TonKho, 
				hd.ID_DonVi, dvqd.GiaNhap, 
				dvqd.GiaBan as GiaBanMaVach, hh.ID_NhomHang as ID_NhomHangHoa,
				dvqd.LaDonViChuan, hh.ChiPhiThucHien as PhiDichVu, cast(ISNULL(hh.QuyCach,1) as float) as QuyCach, 
				Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
				dvqd.GiaBan, dvqd.GiaBan as GiaBanHH, -- use to get banggiachung  of cthd (at NhapHangChiTiet),
				ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
				ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
				ISNULL(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
				ISNULL(hh.ChietKhauMD_NVTheoPT,'0') as ChietKhauMD_NVTheoPT,
				ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
				cthd.ID_LichBaoDuong,
				iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa
    			FROM BH_HoaDon hd
    			JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    			JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    			JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
    			LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi
				and (hhtonkho.ID_LoHang = cthd.ID_LoHang or cthd.ID_LoHang is null) and hhtonkho.ID_DonVi = @ID_DonVi
    			WHERE cthd.ID_HoaDon = @ID_HoaDon 
				and (cthd.ID_ChiTietDinhLuong = cthd.ID or cthd.ID_ChiTietDinhLuong is null)
				and (cthd.ID_ParentCombo IS NULL OR cthd.ID_ParentCombo = cthd.ID)
				and (cthd.ChatLieu is null or cthd.ChatLieu!='5')
				order by cthd.SoThuTu desc
			end
	end
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

			Sql(@"ALTER PROCEDURE [dbo].[GetHisChargeValueCard]
    @ID_DoiTuong [uniqueidentifier],
    @IDChiNhanhs [nvarchar](max),
	@FromDate datetime,
	@ToDate datetime,
	@CurrentPage int,
	@PageSize int
AS
BEGIN
    SET NOCOUNT ON;
	with data_cte
		as(
		select a.*, 
			b.MaPhieuThu,
			b.ID_SoQuy,
			ISNULL(b.TienThu,0) as KhachDaTra			
		from  
    		   (select hd.ID, 
					hd.ID_DonVi,
					MaHoaDon, 
					LoaiHoaDon,
					NgayLapHoaDon, 			
    				hd.TongTienHang,
					hd.TongGiamGia,
					hd.TongChietKhau,
					case hd.LoaiHoaDon
						when 22 then hd.TongTienHang
						when 32 then 0
						when 22 then iif(hd.TongTienHang > 0, hd.TongTienHang,0)
					end as PhatSinhTang,
					case hd.LoaiHoaDon
						when 22 then 0
						when 32 then hd.TongTienHang
						when 22 then iif(hd.TongTienHang < 0, hd.TongTienHang,0)
					end as PhatSinhGiam,
    				case hd.LoaiHoaDon
						when 22 then N'Nạp thẻ'
						when 32 then N'Hoàn trả cọc'
						when 22 then iif(hd.TongTienHang > 0,  N'Điều chỉnh tăng', N'Điều chỉnh giảm')
					end as SLoaiHoaDon
    			from BH_HoaDon hd
    			join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			join DM_DonVi dv on hd.ID_DonVi= dv.ID
    			where LoaiHoaDon in (22,23, 32) and ChoThanhToan='0' and ID_DoiTuong= @ID_DoiTuong
				and hd.NgayLapHoaDon between @FromDate and @ToDate
    			) a
    		left join (select qct.ID_HoaDonLienQuan, 
								max(qct.ID_HoaDon) as ID_SoQuy,
								sum(qct.TienThu) as TienThu, MAX(qhd.MaHoaDon) as MaPhieuThu
    					from Quy_HoaDon_ChiTiet qct 
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    					where qct.ID_DoiTuong= @ID_DoiTuong
    					and qhd.TrangThai ='1'
    					group by qct.ID_HoaDonLienQuan) b
    		on a.ID= b.ID_HoaDonLienQuan
			),
			count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(PhatSinhTang) as TongTang,
				sum(PhatSinhGiam) as TongGiam
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROC [dbo].[getList_ChietKhauNhanVienTongHop]
@ID_ChiNhanhs [nvarchar](max),
@ID_NhanVienLogin nvarchar(max),
@DepartmentIDs [nvarchar](max),
@TextSearch [nvarchar](500),
@DateFrom [nvarchar](max),
@DateTo [nvarchar](max),
@CurrentPage int,
@PageSize int
AS
BEGIN
set nocount on;
	
	declare @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
	Status_DoanhThu varchar(4),	TotalRow int, TotalPage float, TongAllDoanhThu float, TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float)
	INSERT INTO @tab_DoanhSo exec GetAll_DiscountSale @ID_ChiNhanhs,@ID_NhanVienLogin, @DepartmentIDs, @DateFrom, @DateTo, @TextSearch, '', 0,500;	

	DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
	TotalRow int, TotalPage float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongHoaHongVND float, TongAllAll float)
	INSERT INTO @tab_HoaDon exec ReportDiscountInvoice @ID_ChiNhanhs,@ID_NhanVienLogin, @DepartmentIDs, @TextSearch,'0,1,6,19,22,25,32', @DateFrom, @DateTo, 8, 1, 0, 0,500;

	DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
	TotalRow int, TotalPage float, TongHoaHongThucHien float, TongHoaHongThucHien_TheoYC float,  TongHoaHongTuVan float, TongHoaHongBanGoiDV float, TongAll float)
	INSERT INTO @tab_HangHoa exec ReportDiscountProduct_General @ID_ChiNhanhs,@ID_NhanVienLogin, @DepartmentIDs, @TextSearch,'0,1,6,19,22,25,2', @DateFrom, @DateTo, 16,1, 0,500;	

	with data_cte
	as
	 (
		SELECT a.ID_NhanVien, a.MaNhanVien, a.TenNhanVien, 
			SUM(HoaHongThucHien) as HoaHongThucHien,
			SUM(HoaHongThucHien_TheoYC) as HoaHongThucHien_TheoYC,
			SUM(HoaHongTuVan) as HoaHongTuVan,
			SUM(HoaHongBanGoiDV) as HoaHongBanGoiDV,
			SUM(TongHangHoa) as TongHangHoa,
			SUM(HoaHongDoanhThuHD) as HoaHongDoanhThuHD,
			SUM(HoaHongThucThuHD) as HoaHongThucThuHD,
			SUM(HoaHongVND) as HoaHongVND,
			SUM(TongHoaDon) as TongHoaDon,
			SUM(DoanhThu) as DoanhThu,
			SUM(ThucThu) as ThucThu,
			SUM(HoaHongDoanhThuDS) as HoaHongDoanhThuDS,
			SUM(HoaHongThucThuDS) as HoaHongThucThuDS,
			SUM(TongDoanhSo) as TongDoanhSo,
			SUM(TongDoanhSo + TongHoaDon + TongHangHoa) as Tong
		FROM 
		(
		select ID_NhanVien, MaNhanVien, TenNhanVien, 
		HoaHongThucHien, 
		HoaHongThucHien_TheoYC, 
		HoaHongTuVan, 
		HoaHongBanGoiDV, 
		Tong as TongHangHoa,
		0 as HoaHongDoanhThuHD,
		0 as HoaHongThucThuHD,
		0 as HoaHongVND,
		0 as TongHoaDon,
		0 as DoanhThu,
		0 as ThucThu,
		0 as HoaHongDoanhThuDS,
		0 as HoaHongThucThuDS,
		0 as TongDoanhSo
		from @tab_HangHoa
		UNION ALL
		Select ID_NhanVien, MaNhanVien, TenNhanVien, 
		0 as HoaHongThucHien,
		0 as HoaHongThucHien_TheoYC,
		0 as HoaHongTuVan,
		0 as HoaHongBanGoiDV,
		0 as TongHangHoa,
		HoaHongDoanhThu as HoaHongDoanhThuHD,
		HoaHongThucThu as HoaHongThucThuHD,
		HoaHongVND,
		TongAll as TongHoaDon,
		0 as DoanhThu,
		0 as ThucThu,
		0 as HoaHongDoanhThuDS,
		0 as HoaHongThucThuDS,
		0 as TongDoanhSo
		from @tab_HoaDon
		UNION ALL
		Select ID_NhanVien, MaNhanVien, TenNhanVien, 
		0 as HoaHongThucHien,
		0 as HoaHongThucHien_TheoYC,
		0 as HoaHongTuVan,
		0 as HoaHongBanGoiDV,
		0 as TongHangHoa,
		0 as HoaHongDoanhThuHD,
		0 as HoaHongThucThuHD,
		0 as HoaHongVND,
		0 as TongHoaDon,
		TongDoanhThu as DoanhThu,
		TongThucThu as ThucThu,
		HoaHongDoanhThu as HoaHongDoanhThuDS,
		HoaHongThucThu as HoaHongThucThuDS,
		TongAll as TongDoanhSo
		from @tab_DoanhSo
		) as a
		GROUP BY a.ID_NhanVien, a.MaNhanVien, a.TenNhanVien
	),
	count_cte
	as (
		select count(ID_NhanVien) as TotalRow,
			CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float ))  as TotalPage,
			sum(HoaHongThucHien) as TongHoaHongThucHien,
			sum(HoaHongThucHien_TheoYC) as TongHoaHongThucHien_TheoYC,
			sum(HoaHongTuVan) as TongHoaHongTuVan,
			sum(HoaHongBanGoiDV) as TongHoaHongBanGoiDV,
			sum(TongHangHoa) as TongHoaHong_TheoHangHoa,

			sum(HoaHongDoanhThuHD) as TongHoaHongDoanhThu,
			sum(HoaHongThucThuHD) as TongHoaHongThucThu,
			sum(HoaHongVND) as TongHoaHongVND,
			sum(TongHoaDon) as TongHoaHong_TheoHoaDon,
			sum(DoanhThu) as TongDoanhThu,
			sum(ThucThu) as TongThucThu,

			sum(HoaHongDoanhThuDS) as TongHoaHongDoanhThuDS,
			sum(HoaHongThucThuDS) as TongHoaHongThucThuDS,
			sum(TongDoanhSo) as TongHoaHong_TheoDoanhSo,
			sum(Tong) as TongHoaHongAll

		from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.MaNhanVien
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_Where]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max)
AS
BEGIN
	set nocount on;

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	if @timeStart='2016-01-01'		
		select @timeStart = min(NgayLapHoaDon) from BH_HoaDon where LoaiHoaDon=19
    SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.ID_ViTri,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
		c.ID_Xe,
		xe.BienSo,
		c.ID_PhieuTiepNhan,
    	c.TheoDoi,
    	c.ID_DonVi,
    	c.ID_KhuyenMai,
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.MaHoaDonGoc,
    	c.TongTienHDTra,
    	c.NgayLapHoaDon,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
    	c.NguoiTaoHD,
    	c.DiaChiKhachHang,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TenPhongBan,
    	c.TongTienHang, c.TongGiamGia, 
		c.TongThanhToan,
		case when c.PhaiThanhToan < c.TongTienHDTra then 0 else  c.PhaiThanhToan - c.TongTienHDTra - c.KhachDaTra end as ConNo,
		case when c.PhaiThanhToan < c.TongTienHDTra then 0 else  c.PhaiThanhToan - c.TongTienHDTra end as PhaiThanhToan,
		c.ThuTuThe, c.TienMat, c.TienATM,c.TienDoiDiem, c.ChuyenKhoan, c.KhachDaTra,c.TongChietKhau,c.TongTienThue,PTThueHoaDon,
		c.TongThueKhachHang,
		ID_TaiKhoanPos,
		ID_TaiKhoanChuyenKhoan,
    	c.TrangThai,
    	c.KhuyenMai_GhiChu,
    	c.KhuyeMai_GiamGia,
    	c.LoaiHoaDonGoc,
    	c.LoaiHoaDon,
    	c.DiaChiChiNhanh,
    	c.DienThoaiChiNhanh,
    	c.DiemGiaoDich,
    	c.DiemSauGD, -- add 02.08.2018 (bind InHoaDon)
    	c.HoaDon_HangHoa, -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	CONVERT(nvarchar(10),c.NgayApDungGoiDV,103) as NgayApDungGoiDV,
    	CONVERT(nvarchar(10),c.HanSuDungGoiDV,103) as HanSuDungGoiDV
    	FROM
    	(
    		select 
    		a.ID as ID,
    		hdXMLOut.HoaDon_HangHoa,
    		bhhd.ID_DoiTuong,
    			-- Neu theo doi = null --> kiem tra neu la khach le --> theodoi = true, nguoc lai = 1
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,
    		bhhd.ID_HoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DonVi,
			bhhd.ID_Xe,
			bhhd.ID_PhieuTiepNhan,
    		bhhd.ChoThanhToan,
    		bhhd.ID_KhuyenMai,
    		bhhd.KhuyenMai_GhiChu,
    		bhhd.LoaiHoaDon,
			isnull(bhhd.PTThueHoaDon,0) as  PTThueHoaDon,
    		ISNULL(bhhd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(bhhd.DiemGiaoDich,0) AS DiemGiaoDich,
    		ISNULL(gb.ID,N'00000000-0000-0000-0000-000000000000') as ID_BangGia,
			ISNULL(vt.ID,N'00000000-0000-0000-0000-000000000000') as ID_ViTri,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		bhhd.MaHoaDon,
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
    		bhhd.NgayLapHoaDon,
    		bhhd.NgayApDungGoiDV,
    		bhhd.HanSuDungGoiDV,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
			ISNULL(nv.TenNhanVienKhongDau, N'') as TenNhanVienKhongDau,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		bhhd.TongChietKhau,
			bhhd.TongThanhToan,
			ISNULL(bhhd.TongThueKhachHang,0) as TongThueKhachHang,
			ISNULL(bhhd.TongTienThue,0) as TongTienThue,
			bhhd.TongTienHang,
			bhhd.TongGiamGia,
			bhhd.PhaiThanhToan,
			ISNULL(hdt.PhaiThanhToan,0) as TongTienHDTra,
    		a.ThuTuThe,
    		a.TienMat,
			a.TienATM,
			a.TienDoiDiem,
    		a.ChuyenKhoan,
    		a.KhachDaTra,
			ID_TaiKhoanPos,
			ID_TaiKhoanChuyenKhoan,
    		ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
    		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			Select 
    			b.ID,
    			SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
				SUM(ISNULL(b.TienATM, 0)) as TienATM,
    			SUM(ISNULL(b.TienCK, 0)) as ChuyenKhoan,
				SUM(ISNULL(b.TienDoiDiem, 0)) as TienDoiDiem,
    			SUM(ISNULL(b.TienThu, 0)) as KhachDaTra,
				max(b.ID_TaiKhoanPos) as ID_TaiKhoanPos,
				max(b.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan
    			from
    			(
    				Select 
    				bhhd.ID,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then case when TaiKhoanPOS = 1 then ISNULL(hdct.TienGui, 0) else 0 end else ISNULL(hdct.TienGui, 0) * (-1) end end as TienATM,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end else ISNULL(hdct.TienGui, 0) * (-1) end end as TienCK,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then 
							case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else ISNULL(hdct.Tienthu, 0) end
							else case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else -ISNULL(hdct.Tienthu, 0) end end end as TienDoiDiem,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu,
					case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 1 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanPos,
					case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 0 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanChuyenKhoan
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID  
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang		
    				where bhhd.LoaiHoaDon = '19' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd 
					and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))     			
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
			
    			left join 
    				(Select distinct hdXML.ID, 
    					 (
    						select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
    						from BH_HoaDon_ChiTiet ct
    						join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    						join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
    						where ct.ID_HoaDon = hdXML.ID
    						For XML PATH ('')
    					) HoaDon_HangHoa
    				from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
			left join Gara_DanhMucXe xe on c.ID_Xe= xe.ID
			where 
				((select count(Name) from @tblSearch b where     			
				c.MaHoaDon like '%'+b.Name+'%'
				or c.NguoiTaoHD like '%'+b.Name+'%'				
				or c.TenNhanVien like '%'+b.Name+'%'
				or c.TenNhanVienKhongDau like '%'+b.Name+'%'
				or c.DienGiai like '%'+b.Name+'%'
				or c.MaDoiTuong like '%'+b.Name+'%'		
				or c.TenDoiTuong like '%'+b.Name+'%'
				or c.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or c.DienThoai like '%'+b.Name+'%'						
				or xe.BienSo like '%'+b.Name+'%'	
				or c.HoaDon_HangHoa like '%'+b.Name+'%'			
				)=@count or @count=0)	
    	ORDER BY c.NgayLapHoaDon DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonBanHang]	
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max),
	@ID_NhanVienLogin uniqueidentifier,
	@NguoiTao nvarchar(max),
	@IDViTris nvarchar(max),
	@IDBangGias nvarchar(max),
	@TrangThai nvarchar(max),
	@PhuongThucThanhToan nvarchar(max),
	@ColumnSort varchar(max),
	@SortBy varchar(max),
	@CurrentPage int,
	@PageSize int,
	@LaHoaDonSuaChua nvarchar(10),
	@BaoHiem int
AS
BEGIN

  set nocount on;
 declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'HoaDon_XemDS_PhongBan','HoaDon_XemDS_HeThong');

	declare @tblChiNhanh table (ID varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh);

	declare @tblPhuongThuc table (PhuongThuc varchar(4))
	insert into @tblPhuongThuc
	select Name from dbo.splitstring(@PhuongThucThanhToan)

	declare @tblTrangThai table (TrangThaiHD varchar(40))
	insert into @tblTrangThai
	select Name from dbo.splitstring(@TrangThai);


	declare @tblViTri table (ID varchar(40))
	insert into @tblViTri
	select Name from dbo.splitstring(@IDViTris)

	declare @tblBangGia table (ID varchar(40))
	insert into @tblBangGia
	select Name from dbo.splitstring(@IDBangGias)

	declare @tblLoaiHoaDon table (Loai varchar(40))
	insert into @tblLoaiHoaDon
	select Name from dbo.splitstring(@LaHoaDonSuaChua)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	CREATE TABLE #TempIndex (	
		ID uniqueidentifier PRIMARY KEY, 
    	ID_DoiTuong uniqueidentifier,
    	ID_BaoHiem uniqueidentifier, 
    	ID_HoaDon uniqueidentifier, 
    	NgayLapHoaDon datetime,
    	ChoThanhToan bit
	)
		
	insert into #TempIndex WITH(TABLOCKX)
	select ID, ID_DoiTuong, ID_BaoHiem, ID_HoaDon,NgayLapHoaDon, ChoThanhToan
	from BH_HoaDon hd
		where exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)				
					and hd.NgayLapHoadon between @timeStart and  @timeEnd 
					and hd.LoaiHoaDon in (1,25,2);

	with data_cte
	as(
	select c.*, iif(c.ChoThanhToan is null, 0,iif( c.ConNo1 - c.TongTienHDTra > 0, c.ConNo1 - c.TongTienHDTra,0)) as ConNo
	from
	(
	select 
			hd.ID,
    		hd.ID_DoiTuong,
    			
    			CASE 
    				WHEN dt.TheoDoi IS NULL THEN 
    					CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    				ELSE dt.TheoDoi
    			END AS TheoDoi,
    		hd.ID_HoaDon,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
			hd.ID_BaoHiem,
			hd.ID_PhieuTiepNhan,
    		hd.ChoThanhToan,
    		hd.ID_KhuyenMai,
    		hd.KhuyenMai_GhiChu,
    		hd.LoaiHoaDon,

			isnull(hd.TongThueKhachHang,0) as  TongThueKhachHang,
			isnull(hd.CongThucBaoHiem,0) as  CongThucBaoHiem,
			isnull(hd.GiamTruThanhToanBaoHiem,0) as  GiamTruThanhToanBaoHiem,
			isnull(hd.PTThueHoaDon,0) as  PTThueHoaDon,
			isnull(hd.TongTienThueBaoHiem,0) as  TongTienThueBaoHiem,
			isnull(hd.TongTienBHDuyet,0) as  TongTienBHDuyet,
			isnull(hd.SoVuBaoHiem,0) as  SoVuBaoHiem,
			isnull(hd.PTThueBaoHiem,0) as  PTThueBaoHiem,
			isnull(hd.KhauTruTheoVu,0) as  KhauTruTheoVu,
			isnull(hd.GiamTruBoiThuong,0) as  GiamTruBoiThuong,
			isnull(hd.PTGiamTruBoiThuong,0) as  PTGiamTruBoiThuong,
			isnull(hd.BHThanhToanTruocThue,0) as  BHThanhToanTruocThue,

    		ISNULL(hd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(hd.DiemGiaoDich,0) AS DiemGiaoDich,
			ISNULL(hd.ID_BangGia,N'00000000-0000-0000-0000-000000000000') as ID_BangGia,
			ISNULL(hd.ID_ViTri,N'00000000-0000-0000-0000-000000000000') as ID_ViTri,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		hd.MaHoaDon,
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
    		hd.NgayLapHoaDon,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
			dt.NgaySinh_NgayTLap,
			dt.MaSoThue,
			dt.TaiKhoanNganHang,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			isnull(bh.MaDoiTuong,'') as MaBaoHiem,
			isnull(bh.TenDoiTuong,'') as TenBaoHiem,
			isnull(bh.DienThoai,'') as BH_SDT,
			isnull(bh.DiaChi,'') as BH_DiaChi,
			isnull(bh.Email,'') as BH_Email,
			isnull(bh.TenDoiTuong_KhongDau,'') as TenBaoHiem_KhongDau,

			iif(hd.ID_BaoHiem is null,'', tn.NguoiLienHeBH) as LienHeBaoHiem,
			iif(hd.ID_BaoHiem is null,'', tn.SoDienThoaiLienHeBH) as SoDienThoaiLienHeBaoHiem,
			
			dt.ID_TinhThanh, 
			dt.ID_QuanHuyen,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
			ISNULL(nv.TenNhanVienKhongDau, N'') as TenNhanVienKhongDau,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		hd.DienGiai,
			dbo.FUNC_ConvertStringToUnsign(hd.DienGiai) as DienGiaiKhongDau,
    		hd.NguoiTao as NguoiTaoHD,
			ISNULL(hd.TongChietKhau,0) as TongChietKhau,
			ISNULL(hd.TongTienHang,0) as TongTienHang,
			ISNULL(hd.ChiPhi,0) as TongChiPhi, --- chiphi cuahang phaitra
			ISNULL(hd.TongGiamGia,0) as TongGiamGia,
			ISNULL(hd.TongTienThue,0) as TongTienThue,
			ISNULL(hd.PhaiThanhToan,0) as PhaiThanhToan,
			ISNULL(hd.TongThanhToan,0) as TongThanhToan,
			ISNULL(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,
			iif(hdt.LoaiHoaDon=6,ISNULL(hdt.TongThanhToan,0),0) as TongTienHDTra, -- hdgoc: co the la baogia/hoactrahang
			ISNULL(hd.TongThanhToan,0) - ISNULL(a.DaThanhToan,0) as ConNo1,		
			ISNULL(a.ThuTuThe,0) as ThuTuThe,
			ISNULL(a.TienMat,0) as TienMat,
			ISNULL(a.TienATM,0) as TienATM,
			ISNULL(a.ChuyenKhoan,0) as ChuyenKhoan,
			ISNULL(a.TienDoiDiem,0) as TienDoiDiem,
			ISNULL(a.TienDatCoc,0) as TienDatCoc,
			ISNULL(a.GiaTriSDDV,0) as GiaTriSDDV,
			ISNULL(a.GiamGiaCT,0) as GiamGiaCT,
			ISNULL(a.ThanhTienChuaCK,0) as ThanhTienChuaCK,
			ISNULL(a.KhachDaTra,0) as KhachDaTra,
			ISNULL(a.DaThanhToan,0) as DaThanhToan,
			ISNULL(a.BaoHiemDaTra,0) as BaoHiemDaTra,

			cx.MaDoiTuong as MaChuXe,
			cx.TenDoiTuong as ChuXe,

			hd.PhaiThanhToan - ISNULL(a.KhachDaTra,0) as KhachConNo,
			isnull(hd.PhaiThanhToanBaoHiem,0) - ISNULL(a.BaoHiemDaTra,0) as BHConNo,

			hd.ID_Xe,
			isnull(tn.MaPhieuTiepNhan,'') as MaPhieuTiepNhan,
			isnull(xe.BienSo,'') as BienSo,
    		ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
    		Case When hd.ChoThanhToan = '1' then N'Phiếu tạm' when hd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai,
			case  hd.ChoThanhToan
				when 1 then '1'
				when 0 then '0'
			else '4' end as TrangThaiHD,
			case when a.TienMat > 0 then
				case when a.TienATM > 0 then	
					case when a.ChuyenKhoan > 0 then
						case when a.ThuTuThe > 0 then '1,2,3,4' else '1,2,3' end												
						else 
							case when a.ThuTuThe > 0 then  '1,2,4' else '1,2' end end
						else
							case when a.ChuyenKhoan > 0 then 
								case when a.ThuTuThe > 0 then '1,3,4' else '1,3' end
								else 
										case when a.ThuTuThe > 0 then '1,4' else '1' end end end
				else
					case when a.TienATM > 0 then
						case when a.ChuyenKhoan > 0 then
								case when a.ThuTuThe > 0 then '2,3,4' else '2,3' end	
								else 
									case when a.ThuTuThe > 0 then '2,4' else '2' end end
							else 		
								case when a.ChuyenKhoan > 0 then
									case when a.ThuTuThe > 0 then '3,4' else '3' end
									else 
									case when a.ThuTuThe > 0 then '4' else '5' end end end end
									
						as PTThanhToan,
			ID_TaiKhoanPos,
			ID_TaiKhoanChuyenKhoan
	from BH_HoaDon hd
	left join
	(
	
	Select 
    			b.ID,    			
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
				SUM(ISNULL(b.TienATM, 0)) as TienATM,
    			SUM(ISNULL(b.TienCK, 0)) as ChuyenKhoan,
				SUM(ISNULL(b.TienDoiDiem, 0)) as TienDoiDiem,
				SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienThu, 0)) as DaThanhToan, --- = khach + baohiem tra
				SUM(ISNULL(b.KhachDaTra, 0)) as KhachDaTra,
				SUM(ISNULL(b.BaoHiemDaTra, 0)) as BaoHiemDaTra,
				SUM(ISNULL(b.GiaTriSDDV, 0)) as GiaTriSDDV,
				SUM(ISNULL(b.GiamGiaCT, 0)) as GiamGiaCT,
				SUM(ISNULL(b.ThanhTien, 0)) as ThanhTienChuaCK,
				max(b.ID_TaiKhoanPOS) as ID_TaiKhoanPos,
				max(b.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan,
				SUM(ISNULL(b.TienDatCoc, 0)) as TienDatCoc
    			from
    			(
					---- quyct of thishoadon
					select 
						soquyHD.ID_HoaDonLienQuan as ID,
						sum(soquyHD.TienMat) as TienMat,
						sum(soquyHD.TienATM) as TienATM,
						sum(soquyHD.TienCK) as TienCK,
						sum(soquyHD.TienDoiDiem) as TienDoiDiem,
						sum(soquyHD.ThuTuThe) as ThuTuThe,					
						sum(soquyHD.TienThu) as TienThu,
						0 as GiaTriSDDV,
						0 as GiamGiaCT,
						0 as ThanhTien,					
						max(soquyHD.ID_TaiKhoanPos) as ID_TaiKhoanPos,
						max(soquyHD.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan,
						sum(soquyHD.KhachDaTra) as KhachDaTra,
						sum(soquyHD.BaoHiemDaTra) as BaoHiemDaTra,
						sum(soquyHD.TienDatCoc) as TienDatCoc					
				from
				(
						select 
							soquy.ID_HoaDonLienQuan,
							soquy.ID_DoiTuong,					
							sum(isnull(soquy.TienMat,0)) as TienMat,
							sum(isnull(soquy.TienATM,0)) as TienATM,
							sum(isnull(soquy.TienCK,0)) as TienCK,
							sum(isnull(soquy.TienDoiDiem,0)) as TienDoiDiem,
							sum(isnull(soquy.ThuTuThe,0)) as ThuTuThe,
							sum(isnull(soquy.TienDatCoc,0)) as TienDatCoc,
							sum(isnull(soquy.TienThu,0)) as TienThu,
							max(soquy.ID_TaiKhoanPos) as ID_TaiKhoanPos,
							max(soquy.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan,
							iif(soquy.LoaiDoiTuong =1, sum(isnull(soquy.TienThu,0)),0) as KhachDaTra,
							iif(soquy.LoaiDoiTuong =3, sum(isnull(soquy.TienThu,0)),0) as BaoHiemDaTra
						from
						(
								select
									qct.ID_HoaDonLienQuan,
									qct.ID_DoiTuong,
									qct.ID_HoaDon,
									dt.LoaiDoiTuong,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=1, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=1, -qct.TienThu,0) end as TienMat,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=2, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=2, -qct.TienThu,0) end as TienATM,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=3, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=3, -qct.TienThu,0) end as TienCK,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=5, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=5, -qct.TienThu,0) end as TienDoiDiem,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=4, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=4, -qct.TienThu,0) end as ThuTuThe,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=6, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=6, -qct.TienThu,0) end as TienDatCoc,
									iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as TienThu,
									iif(TaiKhoanPOS = 1 , qct.ID_TaiKhoanNganHang,  '00000000-0000-0000-0000-000000000000' ) as ID_TaiKhoanPos,
									iif(TaiKhoanPOS = 0 , qct.ID_TaiKhoanNganHang,  '00000000-0000-0000-0000-000000000000' ) as ID_TaiKhoanChuyenKhoan						
								from Quy_HoaDon_ChiTiet qct
								join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
								join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
								left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID
								left join DM_TaiKhoanNganHang tk on tk.ID= qct.ID_TaiKhoanNganHang 
								where (qhd.TrangThai= 1 or qhd.TrangThai is null) and dt.LoaiDoiTuong in (1,3)	
								and exists (select cn.ID from @tblChiNhanh cn where cn.ID= hd.ID_DonVi)
								and hd.NgayLapHoaDon between @timeStart and @timeEnd
								and hd.LoaiHoaDon in (1,25)
						) soquy				
						group by soquy.ID_HoaDonLienQuan, soquy.ID_DoiTuong,soquy.LoaiDoiTuong
				) soquyHD 					
				group by soquyHD.ID_HoaDonLienQuan

			Union all
						---- get TongThu from HDDatHang: chi get hdXuly first
    					select 
							ID,
							TienMat, TienATM,ChuyenKhoan,
							TienDoiDiem, ThuTuThe, TienThu,
							0 AS GiaTriSDDV,
							0 as GiamGiaCT,
							0 as ThanhTien,
							'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
							'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan,
							TienThu as KhachDaTra,
							0 as BaoHiemDaTra,
							TienDatCoc

						from
						(	
								Select 
										ROW_NUMBER() OVER(PARTITION BY ID_HoaDonLienQuan ORDER BY NgayLapHoaDon ASC) AS isFirst,						
    									d.ID,
										ID_HoaDonLienQuan,
										d.NgayLapHoaDon,
    									sum(d.TienMat) as TienMat,
    									SUM(ISNULL(d.TienATM, 0)) as TienATM,
    									SUM(ISNULL(d.TienCK, 0)) as ChuyenKhoan,
										SUM(ISNULL(d.TienDoiDiem, 0)) as TienDoiDiem,
										sum(d.ThuTuThe) as ThuTuThe,
    									sum(d.TienThu) as TienThu,
										sum(d.TienDatCoc) as TienDatCoc
									
    								FROM
    								(
									
											select hd.ID, hd.NgayLapHoaDon,
												qct.ID_HoaDonLienQuan,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=1, qct.TienThu, 0), iif(qct.HinhThucThanhToan=1, -qct.TienThu, 0)) as TienMat,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=2, qct.TienThu, 0), iif(qct.HinhThucThanhToan=2, -qct.TienThu, 0)) as TienATM,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=3, qct.TienThu, 0), iif(qct.HinhThucThanhToan=3, -qct.TienThu, 0)) as TienCK,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=5, qct.TienThu, 0), iif(qct.HinhThucThanhToan=5, -qct.TienThu, 0)) as TienDoiDiem,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=4, qct.TienThu, 0), iif(qct.HinhThucThanhToan=4, -qct.TienThu, 0)) as ThuTuThe,
												iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu) as TienThu,
												iif(qct.HinhThucThanhToan=6,qct.TienThu,0) as TienDatCoc
											from Quy_HoaDon_ChiTiet qct
											join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
											join BH_HoaDon hdd on hdd.ID= qct.ID_HoaDonLienQuan
											join #TempIndex hd on hd.ID_HoaDon= hdd.ID
											where hdd.LoaiHoaDon = '3' 	
											and hd.ChoThanhToan = 0
											and (qhd.TrangThai= 1 Or qhd.TrangThai is null)
    								) d group by d.ID,d.NgayLapHoaDon,ID_HoaDonLienQuan						
						) thuDH
						where isFirst= 1

					union all

					-- tong giatri sudung goiudv
					select 
						ctsd.ID_HoaDon as ID,
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,						
						0 as TienThu,
						--- tinh %giamgia cua hoadon --> thanhtien moi dichvu sau giamgia
						ctsd.SoLuong * (ct.DonGia - ct.TienChietKhau) * ( 1 -  gdv.TongGiamGia/iif(gdv.TongTienHang =0,1,gdv.TongTienHang))  as GiaTriSDDV,
						0 as GiamGiaCT,
						0 as ThanhTien,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan,
						0 as KhachDaTra,
						0 as BaoHiemDaTra,
						0 as TienDatCoc
					from BH_HoaDon_ChiTiet ctsd
					join BH_HoaDon_ChiTiet ct on ctsd.ID_ChiTietGoiDV= ct.ID
					join BH_HoaDon gdv on ct.ID_HoaDon= gdv.ID
					where  gdv.LoaiHoaDon = 19									
					and 
					 (ctsd.ID_ChiTietDinhLuong= ctsd.ID or ctsd.ID_ChiTietDinhLuong is null)-- khong lay TPDinhLuong
					and exists 
					(				
						select ID from #TempIndex hd where ctsd.ID_HoaDon=  hd.ID
					)

					union all
					---- sum cthd
					select 
						ct.ID_HoaDon,
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,
						0 as TienThu,
						0 as GiaTriSDDV,
						ct.SoLuong * ct.TienChietKhau as GiamGiaCT,
						ct.SoLuong * ct.DonGia  as ThanhTien,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan,
						0 as KhachDaTra,
						0 as BaoHiemDaTra,
						0 as TienDatCoc
					from BH_HoaDon_ChiTiet ct
					where (ct.ID_ChiTietDinhLuong= ct.ID or ct.ID_ChiTietDinhLuong is null)
					and (ct.ID_ParentCombo= ct.ID or ct.ID_ParentCombo is null)
					and exists 
					(
						select ID from #TempIndex hd where ct.ID_HoaDon=  hd.ID
					)				
					
	) b group by b.ID
	) a on hd.ID= a.ID
	left join BH_HoaDon hdt on hd.ID_HoaDon = hdt.ID
    	left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		left join DM_DoiTuong bh on hd.ID_BaoHiem = bh.ID and bh.LoaiDoiTuong = 3
    	left join DM_DonVi dv on hd.ID_DonVi = dv.ID
    	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID 
    	left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    	left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    	left join DM_GiaBan gb on hd.ID_BangGia = gb.ID
    	left join DM_ViTri vt on hd.ID_ViTri = vt.ID    	
		left join Gara_PhieuTiepNhan tn on hd.ID_PhieuTiepNhan = tn.ID
		left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID	
		left join DM_DoiTuong cx on xe.ID_KhachHang= cx.ID	
		
		where (@IDViTris ='' or exists (select ID from @tblViTri vt2 where vt2.ID= vt.ID))
		AND ((@BaoHiem = 0 AND 1 = 0) OR (@BaoHiem = 1 AND hd.ID_BaoHiem IS NOT NULL) OR (@BaoHiem = 2 AND hd.ID_BaoHiem IS NULL)
		OR @BaoHiem = 3 AND 1 = 1)
		and (@IDBangGias='' or exists (select bg.ID from @tblBangGia bg where bg.ID= gb.ID))	
		and
		(exists( select * from @tblNhanVien nv2 where nv2.ID= nv.ID or hd.NguoiTao = @NguoiTao))
		and hd.LoaiHoaDon in (select Loai from @tblLoaiHoaDon)
		and exists 
				(
				select ID from #TempIndex hd2 where hd.ID=  hd2.ID
				)
				and
				((select count(Name) from @tblSearch b where     			
				hd.MaHoaDon like '%'+b.Name+'%'
				or 	hdt.MaHoaDon like '%'+b.Name+'%'
				or hd.NguoiTao like '%'+b.Name+'%'				
				or nv.TenNhanVien like '%'+b.Name+'%'
				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
				or hd.DienGiai like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'		
				or dt.TenDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or dt.DienThoai like '%'+b.Name+'%'		
				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'
				or xe.BienSo like '%'+b.Name+'%'	
				or bh.MaDoiTuong like '%'+b.Name+'%'
				or bh.TenDoiTuong like '%'+b.Name+'%'	
				or bh.TenDoiTuong_KhongDau like '%'+b.Name+'%'	
				or cx.MaDoiTuong like '%'+b.Name+'%'
				or cx.TenDoiTuong like '%'+b.Name+'%'	
				or cx.TenDoiTuong_KhongDau like '%'+b.Name+'%'	
				)=@count or @count=0)	
) c
 where 
		exists (select TrangThaiHD from @tblTrangThai tt where c.TrangThaiHD = tt.TrangThaiHD)
		and ( exists(SELECT Name FROM splitstring(c.PTThanhToan) pt join @tblPhuongThuc pt2 on pt.Name = pt2.PhuongThuc))		
),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumKhachDaTra,
				sum(DaThanhToan) as SumDaThanhToan,
				sum(BaoHiemDaTra) as SumBaoHiemDaTra,
				sum(KhuyeMai_GiamGia) as SumKhuyeMai_GiamGia,
				sum(TongChiPhi) as SumTongChiPhi,
				sum(TongTienHDTra) as SumTongTongTienHDTra,
				sum(PhaiThanhToan) as SumPhaiThanhToan,
				sum(PhaiThanhToanBaoHiem) as SumPhaiThanhToanBaoHiem,
				sum(TongThanhToan) as SumTongThanhToan,
				sum(TienDoiDiem) as SumTienDoiDiem,
				sum(ThuTuThe) as SumThuTuThe,
				sum(TienDatCoc) as SumTienCoc,
				sum(ThanhTienChuaCK) as SumThanhTienChuaCK,
				sum(GiamGiaCT) as SumGiamGiaCT,
				sum(TienMat) as SumTienMat,
				sum(TienATM) as SumPOS,
				sum(ChuyenKhoan) as SumChuyenKhoan,
				sum(GiaTriSDDV) as TongGiaTriSDDV,
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo,

				sum(TongTienThueBaoHiem) as SumTongTienThueBaoHiem,
				sum(TongTienBHDuyet) as SumTongTienBHDuyet,
				sum(KhauTruTheoVu) as SumKhauTruTheoVu,
				sum(GiamTruBoiThuong) as SumGiamTruBoiThuong,
				sum(BHThanhToanTruocThue) as SumBHThanhToanTruocThue
				
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by 
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='ConNo' then ConNo end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='ConNo' then ConNo end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end DESC,
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TienMat' then TienMat end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TienMat' then TienMat end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='ChuyenKhoan' then ChuyenKhoan end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='ChuyenKhoan' then ChuyenKhoan end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TienATM' then TienATM end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TienATM' then TienATM end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiaTriSDDV' then GiaTriSDDV end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiaTriSDDV' then GiaTriSDDV end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='ThuTuThe' then ThuTuThe end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='ThuTuThe' then ThuTuThe end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TienDatCoc' then TienDatCoc end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TienDatCoc' then TienDatCoc end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='BaoHiemDaTra' then BaoHiemDaTra end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='BaoHiemDaTra' then BaoHiemDaTra end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='PhaiThanhToanBaoHiem' then PhaiThanhToanBaoHiem end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='PhaiThanhToanBaoHiem' then PhaiThanhToanBaoHiem end DESC ,

			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TongTienThueBaoHiem' then TongTienThueBaoHiem end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TongTienThueBaoHiem' then TongTienThueBaoHiem end DESC,			
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhauTruTheoVu' then KhauTruTheoVu end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhauTruTheoVu' then KhauTruTheoVu end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiamTruBoiThuong' then GiamTruBoiThuong end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiamTruBoiThuong' then GiamTruBoiThuong end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='BHThanhToanTruocThue' then BHThanhToanTruocThue end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='BHThanhToanTruocThue' then BHThanhToanTruocThue end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TongTienBHDuyet' then TongTienBHDuyet end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TongTienBHDuyet' then TongTienBHDuyet end DESC					
			
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY; 
		

		--drop table #TempIndex
		END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_HoaDonNhapHang]
    @TextSearch [nvarchar](max),
    @LoaiHoaDon varchar(50), ---- dùng chung cho nhập hàng + trả hàng nhập + nhập kho nội bộ
    @IDChiNhanhs [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TrangThais [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int],
    @ColumnSort [nvarchar](max),
    @SortBy [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @tblChiNhanh table (ID varchar(40))
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs)

		declare @tblLoaiHD table (Loai int)
    	insert into @tblLoaiHD
    	select Name from dbo.splitstring(@LoaiHoaDon)
    
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    	with data_cte
    	as (
    	select hdQuy.*	, hdQuy.PhaiThanhToan - hdQuy.KhachDaTra as ConNo
    	from
    	(	
    	select hd.id, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hd.DienGiai, hd.PhaiThanhToan, hd.ChoThanhToan,
    	hd.NgayLapHoaDon, hd.ID_NhanVien, hd.ID_BangGia, hd.TongTienHang, hd.TongChietKhau, hd.TongGiamGia, hd.TongChiPhi,
    	hd.TongTienThue, hd.TongThanhToan, hd.ID_DoiTuong, 
		ctHD.ThanhTienChuaCK,
		ctHD.GiamGiaCT,	
		iif(@LoaiHoaDon='7', -isnull(quy.TienThu,0),  isnull(quy.TienThu,0))  as KhachDaTra,
		isnull(quy.TienMat,0) as TienMat,
		isnull(quy.ChuyenKhoan,0) as ChuyenKhoan,
		isnull(quy.TienATM,0) as TienATM,
		isnull(quy.TienDatCoc,0) as TienDatCoc,
		hd.NguoiTao, hd.NguoiTao as NguoiTaoHD,
    	dv.TenDonVi,hd.ID_DonVi,
		isnull(hd.PTThueHoaDon,0) as PTThueHoaDon,
    	isnull(dv.SoDienThoai,'') as DienThoaiChiNhanh,
    	isnull(dv.DiaChi,'') as DiaChiChiNhanh,
		iif(hd.LoaiHoaDon = 13, iif(hd.ID_DoiTuong='00000000-0000-0000-0000-000000000002',4,dt.LoaiDoiTuong), dt.LoaiDoiTuong) as LoaiDoiTuong,
		---- nhapnoibo: lay nhacc/nhanvien chung 1 cot
		iif(hd.LoaiHoaDon = 13, iif(hd.ID_DoiTuong='00000000-0000-0000-0000-000000000002',nv.MaNhanVien,dt.MaDoiTuong), dt.MaDoiTuong) as MaDoiTuong,
    	iif(hd.LoaiHoaDon = 13, iif(hd.ID_DoiTuong='00000000-0000-0000-0000-000000000002',nv.TenNhanVien,dt.TenDoiTuong), dt.TenDoiTuong) as TenDoiTuong,		
    	isnull(dt.DienThoai,'') as DienThoai,
    	isnull(dt.TenDoiTuong_KhongDau,'') as TenDoiTuong_KhongDau,
    	isnull(nv.MaNhanVien,'') as MaNhanVien,	
    	isnull(nv.TenNhanVien,'') as TenNhanVien,	
    	isnull(nv.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
		po.MaHoaDon as MaHoaDonGoc,
		isnull(po.LoaiHoaDon,0) as LoaiHoaDonGoc,
		hd.YeuCau,
		case hd.LoaiHoaDon
			when 4 then N'Nhập hàng'
			when 13 then N'Nhập kho nội bộ'
			when 14 then N'Nhập hàng khách thừa'
			when 31 then N'Đặt hàng nhà cung cấp'
		else 'Nhập hàng' end as strLoaiHoaDon,
		case hd.ChoThanhToan			
				when 1 then N'Tạm lưu' 
				when 0 then 
					case hd.YeuCau
						when '1' then  N'Đã lưu' 
						when '2' then  N'Đang xử lý' 
						when '3' then  N'Hoàn thành' 
						when '4' then  N'Đã hủy' 
						else iif(hd.LoaiHoaDon = 31, N'Đã lưu' , N'Đã nhập hàng') end
				else  N'Đã hủy'
				end as TrangThai,
		case hd.ChoThanhToan			
			when 1 then N'0' 
			when 0 then 
				case hd.YeuCau
					when '1' then '1'
					when '2' then '2'
					when '3' then '3'
					when '4' then '4'
					else '1' end
			else '4' end as TrangThaiHD				    	
    	from BH_HoaDon hd
    	join DM_DonVi dv on hd.ID_DonVi= dv.ID
		left join BH_HoaDon po on hd.ID_HoaDon= po.ID
    	left join  DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
		left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
		left join (
			select 
				ct.ID_HoaDon,
				sum(ct.SoLuong * ct.TienChietKhau) as GiamGiaCT,			
				sum(ct.SoLuong * ct.DonGia) as ThanhTienChuaCK
			from BH_HoaDon_ChiTiet ct
    		join BH_HoaDon hd on ct.ID_HoaDon= hd.ID   			
    		where exists (select Loai from @tblLoaiHD loaiHD where hd.LoaiHoaDon= loaiHD.Loai)	
    		and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    		and  exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
			group by ct.ID_HoaDon
		) ctHD on ctHD.ID_HoaDon = hd.ID
    	left join
    	(
				select 
					tblTongChi.ID_HoaDonLienQuan,
					sum(tblTongChi.TienThu) as TienThu,
					sum(tblTongChi.TienMat) as TienMat,
					sum(tblTongChi.TienATM) as TienATM,				
					sum(tblTongChi.ChuyenKhoan) as ChuyenKhoan,
					sum(tblTongChi.TienDatCoc) as TienDatCoc
				from
				(
    				select a.ID_HoaDonLienQuan, 
						sum(TienThu) as TienThu,
						sum(a.TienMat) as TienMat,
						sum(a.TienATM) as TienATM,				
						sum(a.ChuyenKhoan) as ChuyenKhoan,
						sum(a.TienDatCoc) as TienDatCoc
    				from(
    					select qct.ID_HoaDonLienQuan,   
						iif(qct.HinhThucThanhToan =1, qct.TienThu, 0) as TienMat,
						iif(qct.HinhThucThanhToan = 2, qct.TienThu,0) as TienATM,
						iif(qct.HinhThucThanhToan = 3, qct.TienThu,0) as ChuyenKhoan,
						iif(qct.HinhThucThanhToan = 6, qct.TienThu,0) as TienDatCoc,
						iif(qhd.LoaiHoaDon = 11,-qct.TienThu, qct.TienThu) as TienThu
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
    					where exists (select Loai from @tblLoaiHD loaiHD where hd.LoaiHoaDon= loaiHD.Loai)	
    					and (qhd.TrangThai= 1 or qhd.TrangThai is null)
    					and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    					and  exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
    					) a group by a.ID_HoaDonLienQuan

						Union all
						---- get TongChi from PO: chi get hdXuly first
    					select 
							ID,
							TienThu,
							TienMat, 
							TienATM,
							ChuyenKhoan,
							TienDatCoc
						from
						(	
								Select 
										ROW_NUMBER() OVER(PARTITION BY ID_HoaDonLienQuan ORDER BY NgayLapHoaDon ASC) AS isFirst,						
    									d.ID,
										ID_HoaDonLienQuan,
										d.NgayLapHoaDon,
    									sum(d.TienMat) as TienMat,
    									SUM(ISNULL(d.TienATM, 0)) as TienATM,
    									SUM(ISNULL(d.TienCK, 0)) as ChuyenKhoan,
										SUM(ISNULL(d.TienDoiDiem, 0)) as TienDoiDiem,
										sum(d.ThuTuThe) as ThuTuThe,
    									sum(d.TienThu) as TienThu,
										sum(d.TienDatCoc) as TienDatCoc
									
    								FROM
    								(									
											select hd.ID, hd.NgayLapHoaDon,
												qct.ID_HoaDonLienQuan,
												iif(qhd.LoaiHoaDon = 12, iif(qct.HinhThucThanhToan=1, qct.TienThu, 0), iif(qct.HinhThucThanhToan=1, -qct.TienThu, 0)) as TienMat,
												iif(qhd.LoaiHoaDon = 12, iif(qct.HinhThucThanhToan=2, qct.TienThu, 0), iif(qct.HinhThucThanhToan=2, -qct.TienThu, 0)) as TienATM,
												iif(qhd.LoaiHoaDon = 12, iif(qct.HinhThucThanhToan=3, qct.TienThu, 0), iif(qct.HinhThucThanhToan=3, -qct.TienThu, 0)) as TienCK,
												iif(qhd.LoaiHoaDon = 12, iif(qct.HinhThucThanhToan=5, qct.TienThu, 0), iif(qct.HinhThucThanhToan=5, -qct.TienThu, 0)) as TienDoiDiem,
												iif(qhd.LoaiHoaDon = 12, iif(qct.HinhThucThanhToan=4, qct.TienThu, 0), iif(qct.HinhThucThanhToan=4, -qct.TienThu, 0)) as ThuTuThe,
												iif(qhd.LoaiHoaDon = 12, qct.TienThu, -qct.TienThu) as TienThu,
												iif(qct.HinhThucThanhToan=6,qct.TienThu,0) as TienDatCoc
											from Quy_HoaDon_ChiTiet qct
											join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
											join BH_HoaDon hdd on hdd.ID= qct.ID_HoaDonLienQuan
											left join BH_HoaDon hd on hd.ID_HoaDon= hdd.ID
											where hdd.LoaiHoaDon = 31	
											and hd.ChoThanhToan = 0
											and (qhd.TrangThai= 1 Or qhd.TrangThai is null)
    								) d group by d.ID,d.NgayLapHoaDon,ID_HoaDonLienQuan						
						) thuDH
						where isFirst= 1
					)tblTongChi
					group by tblTongChi.ID_HoaDonLienQuan
    	) quy on hd.id = quy.ID_HoaDonLienQuan
    	where exists (select Loai from @tblLoaiHD loaiHD where hd.LoaiHoaDon= loaiHD.Loai)	
		and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    	and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
    ) hdQuy
    where 
    exists (select ID from dbo.splitstring(@TrangThais) tt where hdQuy.TrangThaiHD= tt.Name)	
    	and
    	((select count(Name) from @tblSearch b where     			
    		hdQuy.MaHoaDon like '%'+b.Name+'%'
    		or hdQuy.NguoiTao like '%'+b.Name+'%'				
    		or hdQuy.TenNhanVien like '%'+b.Name+'%'
    		or hdQuy.TenNhanVienKhongDau like '%'+b.Name+'%'
    		or hdQuy.DienGiai like '%'+b.Name+'%'
    		or hdQuy.MaDoiTuong like '%'+b.Name+'%'		
    		or hdQuy.TenDoiTuong like '%'+b.Name+'%'
    		or hdQuy.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or hdQuy.DienThoai like '%'+b.Name+'%'		
    		)=@count or @count=0)	
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
					sum(ThanhTienChuaCK) as SumThanhTienChuaCK,
					sum(GiamGiaCT) as SumGiamGiaCT,
    				sum(TongTienHang) as SumTongTienHang,
    				sum(TongGiamGia) as SumTongGiamGia,
					sum(TienMat) as SumTienMat,	
					sum(TienATM) as SumPOS,	
					sum(ChuyenKhoan) as SumChuyenKhoan,	
					sum(TienDatCoc) as SumTienCoc,	
    				sum(KhachDaTra) as SumDaThanhToan,				
    				sum(TongChiPhi) as SumTongChiPhi,
    				sum(PhaiThanhToan) as SumPhaiThanhToan,
    				sum(TongThanhToan) as SumTongThanhToan,				
    				sum(TongTienThue) as SumTongTienThue,
    				sum(ConNo) as SumConNo
    			from data_cte
    		)
    		select dt.*, cte.*	
    		from data_cte dt
    		cross join count_cte cte
    		order by 
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='ConNo' then ConNo end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='ConNo' then ConNo end DESC,
    			case when @SortBy <>'ASC' then ''
    			when @ColumnSort='MaHoaDon' then MaHoaDon end ASC,
    			case when @SortBy <>'DESC' then ''
    			when @ColumnSort='MaHoaDon' then MaHoaDon end DESC,
    			case when @SortBy <>'ASC' then ''
    			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end ASC,
    			case when @SortBy <>'DESC' then ''
    			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end DESC,
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='TongTienHang' then TongTienHang end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='TongTienHang' then TongTienHang end DESC,
    			case when @SortBy <>'ASC' then 0
    			when @ColumnSort='GiamGia' then TongGiamGia end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='GiamGia' then TongGiamGia end DESC,
    			case when @SortBy <>'ASC' then 0
    			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end DESC,
    			case when @SortBy <>'ASC' then 0
    			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC			
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_XuatHuy]
    @IDChiNhanhs nvarchar(max)= null,
	@DateFrom datetime= null,
	@DateTo datetime= null,
	@LoaiHoaDons nvarchar(max)= null,
	@TrangThais nvarchar(max)= null,
	@TextSearch nvarchar(max)=null,
	@CurrentPage int= null,
	@PageSize int = null
AS
BEGIN
	set nocount on;
	declare @sqlSub nvarchar(max) ='', @whereSub nvarchar(max) ='',@tblSub varchar(max) ='',
	@sql nvarchar(max) ='', @where nvarchar(max) ='', @tblOut varchar(max) ='',
	@paramDefined nvarchar(max) =''

	declare @paramIn nvarchar(max) =' declare @textSeach_isNull int = 1'

	set @whereSub =' where 1 = 1 and hd.loaiHoaDon in (1,8,2) and (hdct.ID_ChiTietDinhLuong is null or hdct.ID_ChiTietDinhLuong != hdct.ID) and hh.LaHangHoa = 1'
	set @where =' where 1 = 1 '
	set @paramDefined = N'@IDChiNhanhs_In nvarchar(max)= null,
							@DateFrom_In datetime= null,
							@DateTo_In datetime= null,
							@LoaiHoaDons_In nvarchar(max)= null,
							@TrangThais_In nvarchar(max)= null,
							@TextSearch_In nvarchar(max)= null,
							@CurrentPage_In int= null,
							@PageSize_In int = null '


	if isnull(@CurrentPage,'') ='' set @CurrentPage= 0
	if isnull(@PageSize,'') ='' set @PageSize= 10

	if isnull(@IDChiNhanhs,'') !=''
	begin
		set @tblSub = CONCAT(@tblSub ,N'  declare @tblChiNhanh table(ID uniqueidentifier)
										 insert into @tblChiNhanh 
										 select name from dbo.splitstring(@IDChiNhanhs_In); ')
		set @whereSub = CONCAT(@whereSub, N' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)' )
	end
	
	if isnull(@DateFrom,'') !=''
		begin
			set @whereSub = CONCAT(@whereSub, N' and hd.NgayLapHoaDon >= @DateFrom_In' )
		end
	if isnull(@DateTo,'') !=''
		begin
			set @whereSub = CONCAT(@whereSub, N' and hd.NgayLapHoaDon < @DateTo_In' )
		end

	if isnull(@LoaiHoaDons,'') !=''
	begin
		set @tblOut = CONCAT(@tblOut ,N'  declare @tblLoai table(Loai int)
									 insert into @tblLoai 
									 select name from dbo.splitstring(@LoaiHoaDons_In) ;')
		set @where = CONCAT(@where, N' and exists (select Loai from @tblLoai loai where tbl.LoaiHoaDon= loai.Loai)' )
	end

	if isnull(@TrangThais,'') !=''
	begin
		set @tblOut = CONCAT(@tblOut ,N'  declare @tblTrangThai table(TrangThai int)
									 insert into @tblTrangThai 
									 select name from dbo.splitstring(@TrangThais_In) ;')
		set @where = CONCAT(@where, N' and exists (select TrangThai from @tblTrangThai tt where tbl.TrangThai= tt.TrangThai)' )
	end

	if isnull(@TextSearch,'') !=''
	begin
		set @paramIn = concat(@paramIn, N' set @textSeach_isNull =0 ')
		set @tblOut = CONCAT(@tblOut ,N'  DECLARE @tblSearchString TABLE (Name [nvarchar](max));
									DECLARE @count int;
									INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''';
									Select @count =  (Select count(*) from @tblSearchString) ;')
		set @where = CONCAT(@where, N' and ((select count(Name) from @tblSearchString b 
									where tbl.MaHoaDon like ''%'' +b.Name +''%'' 										
										or tbl.NguoiTaoHD like ''%'' +b.Name +''%'' 
										or tbl.DienGiai like N''%''+b.Name+''%''
										or nv.TenNhanVien like ''%''+b.Name+''%''
										or nv.TenNhanVienKhongDau like ''%''+b.Name+''%''
										or nv.TenNhanVienChuCaiDau like ''%''+b.Name+''%''
										or nv.MaNhanVien like ''%''+b.Name+''%''
										or tn.MaPhieuTiepNhan like ''%''+b.Name+''%''
										or hdsc.MaHoaDon like ''%''+b.Name+''%''
										or xe.BienSo like ''%''+b.Name+''%''									
										or tbl.DienGiaiUnsign like N''%''+b.Name+''%''
										)=@count or @count=0)' )
	end

	set @sqlSub = CONCAT(
	N' select 
			hd.ID,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
			hd.ID_HoaDon,
			hd.ID_PhieuTiepNhan,			
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,   	
    		ISNULL(hdct.SoLuong * hdct.GiaVon, 0) as TongTienHang,
    		hd.DienGiai,
			hd.DienGiai as DienGiaiUnsign,
			---iif(@textSeach_isNull= 1, hd.DienGiai, dbo.FUNC_ConvertStringToUnsign(hd.DienGiai)) as DienGiaiUnsign,
			hd.ChoThanhToan,
			hd.NguoiTao as NguoiTaoHD, 
			
			case hd.ChoThanhToan
				when 1 then 1
				when 0 then 2
			else 3 end as TrangThai,
			case hd.ChoThanhToan
				when 1 then N''Tạm lưu''
				when 0 then N''Hoàn thành''
			else N''Hủy bỏ'' end as YeuCau,    
			---- 1.sudung gdv, 2.xuat banle, 3.xuat suachua, 8.xuatkho thuong, 12.xuatbaohanh
			case hd.LoaiHoaDon
				when 8 then case when hd.ID_PhieuTiepNhan is not null then case when hdct.ChatLieu = 4 then 1 else 3 end else 8 end	
				when 1 then case when hdct.ChatLieu = 4 then 1 else 2 end
				when 2 then 12
			else hd.LoaiHoaDon end LoaiHoaDon,
			case hd.LoaiHoaDon
				when 8 then 
					case when hd.ID_PhieuTiepNhan is not null then 
						case when hdct.ChatLieu = 4 then N''Phiếu xuất sử dụng gói dịch vụ'' else N''Xuất sửa chữa'' end else  N''Phiếu xuất kho'' end	
				when 1 then case when hdct.ChatLieu = 4 then N''Phiếu xuất sử dụng gói dịch vụ'' else  N''Xuất bán lẻ'' end
				when 2 then N''Xuất bảo hành''
			else N''Xuất bán lẻ'' end LoaiPhieu,

			case hd.LoaiHoaDon
				when 8 then case when hd.ID_PhieuTiepNhan is not null then dt.TenDoiTuong else '''' end	
				when 1 then dt.TenDoiTuong
				when 2 then dt.TenDoiTuong
			else '''' end TenDoiTuong

			into #hdXK
			from BH_HoaDon hd
			join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_Doituong= dt.ID
									', @whereSub)

		set @sql= CONCAT(@paramIn, @tblSub, @tblOut, @sqlSub, '; ',
			N' with data_cte
			as(
				select tbl.ID,
					tbl.MaHoaDon,
					tbl.NgayLapHoaDon,
					tbl.ID_HoaDon,
					tbl.ID_PhieuTiepNhan,
					tbl.ID_NhanVien,
					tbl.ID_DonVi,				
					sum(tbl.TongTienHang) as TongTienHang,
					tbl.ChoThanhToan,
					tbl.LoaiHoaDon,
					tbl.LoaiPhieu,
					tbl.NguoiTaoHD,
					tbl.YeuCau,
					tbl.DienGiai,
					tn.MaPhieuTiepNhan,
					hdsc.MaHoaDon as MaHoaDonGoc,
					xe.BienSo,					
					dv.TenDonVi,
					nv.TenNhanVien,
					tbl.TenDoiTuong
			from #hdXK tbl 
			join DM_DonVi dv on tbl.ID_DonVi = dv.ID
			left join BH_HoaDon hdsc on tbl.ID_HoaDon= hdsc.ID
			left join Gara_PhieuTiepNhan tn on tbl.ID_PhieuTiepNhan= tn.ID
			left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
    		left join NS_NhanVien nv on tbl.ID_NhanVien = nv.ID
			', @where ,
			N' group by 
					tbl.ID,
					tbl.MaHoaDon,
					tbl.NgayLapHoaDon,
					tbl.ID_HoaDon,
					tbl.ID_PhieuTiepNhan,
					tbl.ID_NhanVien,
					tbl.ID_DonVi,		
					tbl.ChoThanhToan,
					tbl.LoaiHoaDon,
					tbl.LoaiPhieu,
					tbl.NguoiTaoHD,
					tbl.YeuCau,
					tbl.DienGiai,
					tn.MaPhieuTiepNhan,
					hdsc.MaHoaDon,
					xe.BienSo,					
					dv.TenDonVi,
					nv.TenNhanVien,
					tbl.TenDoiTuong
			),
			count_cte
			as (
			select count(ID) as TotalRow,
				sum(TongTienHang) as SumTongTienHang
			from data_cte
			)
			select dt.*, cte.*
			from data_cte dt
			cross join count_cte cte
			order by dt.NgayLapHoaDon desc
			OFFSET (@CurrentPage_In* @PageSize_In) ROWS
			FETCH NEXT @PageSize_In ROWS ONLY; 
			')

			print @sql
		
			
			exec sp_executesql @sql, @paramDefined,
					 @IDChiNhanhs_In = @IDChiNhanhs,
					@DateFrom_In = @DateFrom,
					@DateTo_In = @DateTo,
					@LoaiHoaDons_In = @LoaiHoaDons,
					@TrangThais_In = @TrangThais,
					@TextSearch_In =@TextSearch,
					@CurrentPage_In = @CurrentPage,
					@PageSize_In = @PageSize
				
	
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListCashFlow_Paging]
	@IDDonVis [nvarchar](max),
    @ID_NhanVien [nvarchar](40),
    @ID_NhanVienLogin [uniqueidentifier],
    @ID_TaiKhoanNganHang [nvarchar](40),
    @ID_KhoanThuChi [nvarchar](40),
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiSoQuy [nvarchar](15),	-- mat/nganhang/all
    @LoaiChungTu [nvarchar](2), -- thu/chi
    @TrangThaiSoQuy [nvarchar](2),
    @TrangThaiHachToan [nvarchar](2),
    @TxtSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int],
	@LoaiNapTien [nvarchar](15) -- 11.tiencoc, 10. khongphai tiencoc, 1.all
AS
BEGIN

	SET NOCOUNT ON;

   SET NOCOUNT ON;
	declare @isNullSearch int = 1
	if isnull(@TxtSearch,'')='' OR @TxtSearch ='%%'
		begin
			set @isNullSearch =0 
			set @TxtSearch ='%%'
		end
	else
		set @TxtSearch= CONCAT(N'%',@TxtSearch, '%')

	declare @tblChiNhanh table (ID uniqueidentifier)
    insert into @tblChiNhanh
	select name from dbo.splitstring(@IDDonVis)

	--declare #tblQuyHD table (ID uniqueidentifier, MaHoaDon nvarchar(40), NgayLapHoaDon datetime, ID_DonVi uniqueidentifier,
	--LoaiHoaDon int, NguoiTao nvarchar(100), HachToanKinhDoanh bit, PhieuDieuChinhCongNo int,
	--NoiDungThu nvarchar(max), ID_NhanVienPT uniqueidentifier, TrangThai bit)

	--insert into #tblQuyHD
	select qhd.ID,
		qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.ID_DonVi, qhd.LoaiHoaDon, qhd.NguoiTao,
    	qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo, qhd.NoiDungThu,
    	qhd.ID_NhanVien as ID_NhanVienPT, qhd.TrangThai	
	into #tblQuyHD
	from Quy_HoaDon qhd	
	where qhd.NgayLapHoaDon between  @DateFrom and  @DateTo		
	and qhd.ID_DonVi in (select * from dbo.splitstring(@IDDonVis))
	and(qhd.PhieuDieuChinhCongNo != '1' or qhd.PhieuDieuChinhCongNo is null)


    	declare @nguoitao nvarchar(100) = (select taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDDonVis,'SoQuy_XemDS_PhongBan','SoQuy_XemDS_HeThong');
    	
    	with data_cte
    	as(

    select tblView.*
    	from
    		(
			select 
    			tblQuy.ID,
    			tblQuy.MaHoaDon,
    			tblQuy.NgayLapHoaDon,
    			tblQuy.ID_DonVi,
    			tblQuy.LoaiHoaDon,
    			tblQuy.NguoiTao,
				ISNUll(nv2.TenNhanVien,'') as TenNhanVien,
				ISNUll(nv2.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
			
				ISNUll(dv.TenDonVi,'') as TenChiNhanh,
				ISNUll(dv.SoDienThoai,'') as DienThoaiChiNhanh,
				ISNUll(dv.DiaChi,'') as DiaChiChiNhanh,
				ISNUll(nguon.TenNguonKhach,'') as TenNguonKhach,
    			ISNUll(tblQuy.TrangThai,'1') as TrangThai,
    			tblQuy.NoiDungThu,
				iif(@isNullSearch=0, dbo.FUNC_ConvertStringToUnsign(NoiDungThu), tblQuy.NoiDungThu) as NoiDungThuUnsign,
    			tblQuy.PhieuDieuChinhCongNo,
    			tblQuy.ID_NhanVienPT as ID_NhanVien,
    			iif(LoaiHoaDon=11, TienMat,0) as ThuMat,
    			iif(LoaiHoaDon=12, TienMat,0) as ChiMat,
    			iif(LoaiHoaDon=11, TienGui,0) as ThuGui,
    			iif(LoaiHoaDon=12, TienGui,0) as ChiGui,
    			TienMat + TienGui as TienThu,
    			TienMat + TienGui as TongTienThu,
				TienGui,
				TienMat, 
				ChuyenKhoan, 
				TienPOS,
				TienDoiDiem, 
				TTBangTienCoc,
				TienTheGiaTri,
    			TenTaiKhoanPOS, TenTaiKhoanNOTPOS,
    			cast(ID_TaiKhoanNganHang as varchar(max)) as ID_TaiKhoanNganHang,
    			ID_KhoanThuChi,
    			NoiDungThuChi,
				tblQuy.ID_NhanVienPT,
				dt.ID_NguonKhach,
				isnull(dt.LoaiDoiTuong,0) as LoaiDoiTuong,
    			ISNULL(tblQuy.HachToanKinhDoanh,'1') as HachToanKinhDoanh,
    			case when tblQuy.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
    		case when tblQuy.HachToanKinhDoanh = '1' or tblQuy.HachToanKinhDoanh is null  then '11' else '10' end as TrangThaiHachToan,
    		case when tblQuy.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong  else nv.TenNhanVien end as NguoiNopTien,
			case when nv.TenNhanVien is null then  dt.DiaChi  else nv.DiaChiCoQuan end as DiaChiKhachHang,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong_KhongDau  else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
    		case when nv.MaNhanVien is null then dt.MaDoiTuong else  nv.MaNhanVien end as MaDoiTuong,
    		case when nv.MaNhanVien is null then dt.DienThoai else  nv.DienThoaiDiDong  end as SoDienThoai,
    			case when qct.TienMat > 0 then case when qct.TienGui > 0 then '2' else '1' end 
    			else case when qct.TienGui > 0 then '0'
    				else case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then '0' else '1' end end end as LoaiSoQuy,
    			-- check truong hop tongthu = 0
    		case when qct.TienMat > 0 then case when qct.TienGui > 0 then N'Tiền mặt, chuyển khoản' else N'Tiền mặt' end 
    			else case when qct.TienGui > 0 then N'Chuyển khoản' else 
    				case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then  N'Chuyển khoản' else N'Tiền mặt' end end end as PhuongThuc	
    							
    		from #tblQuyHD tblQuy
			 join 
    			(select 
    				 a.ID_hoaDon,
    				 sum(isnull(a.TienMat, 0)) as TienMat,
    				 sum(isnull(a.TienGui, 0)) as TienGui,
					 sum(isnull(a.TienPOS, 0)) as TienPOS,
					 sum(isnull(a.ChuyenKhoan, 0)) as ChuyenKhoan,
					 sum(isnull(a.TienDoiDiem, 0)) as TienDoiDiem,
					 sum(isnull(a.TienTheGiaTri, 0)) as TienTheGiaTri,
					 sum(isnull(a.TTBangTienCoc, 0)) as TTBangTienCoc,
    				 max(a.TenTaiKhoanPOS) as TenTaiKhoanPOS,
    				 max(a.TenTaiKhoanNOPOS) as TenTaiKhoanNOTPOS,
    				 max(a.ID_DoiTuong) as ID_DoiTuong,
    				 max(a.ID_NhanVien) as ID_NhanVien,
    				 max(a.ID_TaiKhoanNganHang) as ID_TaiKhoanNganHang,
    				 max(a.ID_KhoanThuChi) as ID_KhoanThuChi,
    				 max(a.NoiDungThuChi) as NoiDungThuChi
    			from
    			(
    				select *
    				from(
    					select 
    					qct.ID_HoaDon,
						iif(qct.HinhThucThanhToan= 1, qct.TienThu,0) as TienMat,
						iif(qct.HinhThucThanhToan= 2 or qct.HinhThucThanhToan = 3, qct.TienThu,0) as TienGui,			
						iif(qct.HinhThucThanhToan= 2, qct.TienThu,0) as TienPOS,
						iif(qct.HinhThucThanhToan= 3, qct.TienThu,0) as ChuyenKhoan,
						iif(qct.HinhThucThanhToan= 4, qct.TienThu,0) as TienDoiDiem,
						iif(qct.HinhThucThanhToan= 5, qct.TienThu,0) as TienTheGiaTri,
						iif(qct.HinhThucThanhToan= 6, qct.TienThu,0) as TTBangTienCoc,						
						qct.ID_DoiTuong, qct.ID_NhanVien, 
    					ISNULL(qct.ID_TaiKhoanNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
    					ISNULL(qct.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi,
    					case when tk.TaiKhoanPOS='1' then IIF(tk.TrangThai = 0, '<span style=""color: red; text - decoration: line - through; "" title=""Đã xóa"">' + tk.TenChuThe + '</span>', tk.TenChuThe) else '' end as TenTaiKhoanPOS,
    					case when tk.TaiKhoanPOS = '0' then IIF(tk.TrangThai = 0, '<span style=""color:red;text-decoration: line-through;"" title=""Đã xóa"">' + tk.TenChuThe + '</span>', tk.TenChuThe) else '' end as TenTaiKhoanNOPOS,
    					iif(ktc.NoiDungThuChi is null, '',
						iif(ktc.TrangThai = 0, concat(ktc.NoiDungThuChi, '{DEL}'), ktc.NoiDungThuChi)) as NoiDungThuChi,
						----11.coc, 13.khongbutru congno, 10.khong coc


						iif(qct.LoaiThanhToan = 1, '11', iif(qct.LoaiThanhToan = 3, '13', '10')) as LaTienCoc, 
						IIF(qct.ID_HoaDonLienQuan IS NULL AND qct.ID_KhoanThuChi IS NULL, 1, 0) AS LaThuChiMacDinh


						from #tblQuyHD  qhd		
						left join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon = qhd.ID


						left
						join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID



				   left
						join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi = ktc.ID


						where qct.HinhThucThanhToan not in (4, 5, 6)-- diem, thegiatri, coc
    					)qct
					where qct.ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang


					and(qct.ID_KhoanThuChi like @ID_KhoanThuChi OR(qct.LaThuChiMacDinh = 1 AND @ID_KhoanThuChi = '00000000-0000-0000-0000-000000000001'))


					and qct.LaTienCoc like '%' + @LoaiNapTien + '%'
    			) a group by a.ID_HoaDon
    		) qct on tblQuy.ID = qct.ID_HoaDon


			left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID


			left join NS_NhanVien nv on qct.ID_NhanVien = nv.ID


		left join DM_DonVi dv on tblQuy.ID_DonVi = dv.ID


		left join NS_NhanVien nv2 on tblQuy.ID_NhanVienPT = nv2.ID


		left join DM_NguonKhachHang nguon on dt.ID_NguonKhach = nguon.ID
    	 ) tblView

		 where tblView.TrangThaiHachToan like '%' + @TrangThaiHachToan + '%'



		and tblView.TrangThaiSoQuy like '%' + @TrangThaiSoQuy + '%'



		and tblView.LoaiChungTu like '%' + @LoaiChungTu + '%'




			and tblView.ID_NhanVienPT like @ID_NhanVien



			and(exists(select ID from @tblNhanVien nv where tblView.ID_NhanVienPT = nv.ID) or tblView.NguoiTao like @nguoitao)



		and exists(select Name from dbo.splitstring(@LoaiSoQuy) where LoaiSoQuy= Name)


			and(MaHoaDon like @TxtSearch


			OR MaDoiTuong like @TxtSearch


			OR NguoiNopTien like @TxtSearch


			OR SoDienThoai like @TxtSearch


			OR TenNhanVien like @TxtSearch--nvlap


			OR TenNhanVienKhongDau like @TxtSearch


			OR TenDoiTuong_KhongDau like @TxtSearch-- nguoinoptien


			OR NoiDungThuUnsign like @TxtSearch
			)
    	),
    	count_cte
		as (
		select count(dt.ID) as TotalRow,
    		CEILING(count(dt.ID) / cast(@PageSize as float)) as TotalPage,
    		sum(ThuMat) as TongThuMat,
    		sum(ChiMat) as TongChiMat,
    		sum(ThuGui) as TongThuCK,
    		sum(ChiGui) as TongChiCK



		from data_cte dt
    	)
    	select*
		from data_cte dt



		cross join count_cte
		order by dt.NgayLapHoaDon desc



		OFFSET(@CurrentPage * @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END
");

			Sql(@"ALTER PROCEDURE [dbo].[GetListTheGiaTri]
    @IDDonVis [nvarchar](max),
	@LoaiHoaDons varchar(20) = null, --- 
    @TextSearch [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TrangThais [nvarchar](10),
    @MucNapFrom [float],
    @MucNapTo [float],
    @KhuyenMaiFrom [float],
    @KhuyenMaiTo [float],
    @KhuyenMaiLaPTram [int],
    @ChietKhauFrom [float],
    @ChietKhauTo [float],
    @ChietKhauLaPTram [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;

	declare @tblLoaiHD table(LoaiHoaDon int)
	insert into @tblLoaiHD
	select Name from dbo.splitstring(@LoaiHoaDons)
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @MucNapMax float= (select max(TongChiPhi) from BH_HoaDon where ChoThanhToan= 0 and LoaiHoaDon= 22 );
    	if @MucNapTo is null
    		set @MucNapTo= @MucNapMax
    	if @KhuyenMaiTo is null
    		set @KhuyenMaiTo = @MucNapMax
    	if @ChietKhauTo is null
    		set @ChietKhauTo= @MucNapMax;
    	
    	with data_cte
    	as
    	(
    
    	select tblThe.ID,
			tblThe.MaHoaDon,
			tblThe.NgayLapHoaDon,
			tblThe.LoaiHoaDon,
			tblThe.NgayTao,
    		tblThe.TongChiPhi as MucNap,
    		tblThe.TongChietKhau as KhuyenMaiVND,
    		tblThe.TongTienHang as TongTienNap,
    		tblThe.TongTienThue as SoDuSauNap,
			tblThe.TongGiamGia  as ChietKhauVND,    	
    		ISNULL(tblThe.DienGiai,'') as GhiChu,
    		tblThe.NguoiTao,
    		ISNULL(tblThe.ID_DoiTuong,'00000000-0000-0000-0000-000000000000') as ID_DoiTuong,
    		tblThe.PhaiThanhToan,
    		ISNULL(tblThe.NhanVienThucHien,'') as NhanVienThucHien,
    		tblThe.MaDoiTuong as MaKhachHang,
    		tblThe.TenDoiTuong as TenKhachHang,
    		tblThe.DienThoai as SoDienThoai,
    		tblThe.DiaChi as DiaChiKhachHang,
    		tblThe.ChoThanhToan,
    		tblThe.ChietKhauPT,
    		tblThe.KhuyenMaiPT,
			tblThe.ID_DonVi,
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienMat,0),-ISNULL(soquy.TienMat,0))  as TienMat,
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienPOS,0),-ISNULL(soquy.TienPOS,0))  as TienATM,
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienCK,0),-ISNULL(soquy.TienCK,0))  as TienGui,
    		iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienThu,0),-ISNULL(soquy.TienThu,0)) as KhachDaTra,
    		dv.TenDonVi,
    		dv.SoDienThoai as DienThoaiChiNhanh,
    		dv.DiaChi as DiaChiChiNhanh
    	from
    		(
    		select *
    		from
    			(select hd.ID, 
						hd.MaHoaDon,
						hd.LoaiHoaDon,
						hd.NgayLapHoaDon,
						hd.ID_DonVi,
						hd.ID_DoiTuong,
						hd.ID_NhanVien,
						hd.NguoiTao,
						hd.NgayTao,
						hd.TongChiPhi,
						hd.TongTienHang,
						hd.TongChietKhau,
						hd.TongGiamGia,
						hd.TongTienThue,
						hd.ChoThanhToan,		
						hd.PhaiThanhToan,						
						hd.DienGiai,
						iif(hd.TongChiPhi=0,0, hd.TongGiamGia/hd.TongChiPhi * 100) as ChietKhauPT,
						iif(hd.TongChiPhi=0,0, hd.TongChietKhau/hd.TongChiPhi * 100) as KhuyenMaiPT,
    					dt.MaDoiTuong, dt.TenDoiTuong,
    					dt.DienThoai, 
    					dt.DiaChi,
    					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai,
    					NhanVienThucHien
    				from BH_HoaDon hd
    				join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				left join (
    						Select distinct
    							(
    								Select distinct nv.TenNhanVien + ',' AS [text()]
    								From dbo.BH_NhanVienThucHien th
    								join dbo.NS_NhanVien nv on th.ID_NhanVien = nv.ID
    								where th.ID_HoaDon= nvth.ID_HoaDon
    								For XML PATH ('')
    							) NhanVienThucHien, nvth.ID_HoaDon
    							From dbo.BH_NhanVienThucHien nvth
    							) nvThucHien on hd.ID = nvThucHien.ID_HoaDon
    				where exists (select name from dbo.splitstring(@IDDonVis) dv where hd.ID_DonVi= dv.Name)	
    				and hd.LoaiHoaDon in (select LoaiHoaDon from @tblLoaiHD)
    				and hd.TongChiPhi >= @MucNapFrom and hd.TongChiPhi <= @MucNapTo -- mucnap
    				and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon <=@ToDate
    					AND ((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    						or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    						or dt.DienThoai like '%'+b.Name+'%'			
    					or hd.MaHoaDon like '%' +b.Name +'%' 
    						or hd.NguoiTao like '%' +b.Name +'%' 				
    						)=@count or @count=0)	
    			) the
    			where IIF(@KhuyenMaiLaPTram = 1, the.TongChietKhau, the.KhuyenMaiPT) >= @KhuyenMaiFrom -- khuyenmai
    				and IIF(@KhuyenMaiLaPTram = 1, the.TongChietKhau, the.KhuyenMaiPT) <= @KhuyenMaiTo
    				and IIF(@ChietKhauLaPTram = 1, the.TongGiamGia, the.ChietKhauPT) >= @ChietKhauFrom -- giamgia
    				and IIF(@ChietKhauLaPTram = 1, the.TongGiamGia, the.ChietKhauPT) <= @ChietKhauTo
    				and the.TrangThai like @TrangThais 
    		) tblThe		
    	join DM_DonVi dv on tblThe.ID_DonVi= dv.ID
    	left join ( select quy.ID_HoaDonLienQuan, 
    					sum(quy.TienThu) as TienThu,
    					sum(quy.TienMat) as TienMat,
    					sum(quy.TienPOS) as TienPOS,
    					sum(quy.TienCK) as TienCK
    				from
    				(
    					select qct.ID_HoaDonLienQuan,
    						iif(qct.HinhThucThanhToan = 1, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0) as TienMat,
    						iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) as TienThu,
    						case when tk.TaiKhoanPOS = '1' then iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) else 0 end as TienPOS,
    						case when tk.TaiKhoanPOS = '0' then iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) else 0 end as TienCK
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang= tk.ID
    					where qhd.TrangThai= 1 or qhd.TrangThai is null
    				) quy 
    				group by quy.ID_HoaDonLienQuan) soquy on tblThe.ID= soquy.ID_HoaDonLienQuan
    	),
    	count_cte
    	as (
    		select count(ID) as TotalRow,
    			CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    			sum(MucNap) as TongMucNapAll,
    			sum(KhuyenMaiVND) as TongKhuyenMaiAll,
    			sum(TongTienNap) as TongTienNapAll,			
    			sum(ChietKhauVND) as TongChietKhauAll,
    			sum(SoDuSauNap) as SoDuSauNapAll,
    			sum(PhaiThanhToan) as PhaiThanhToanAll,			
    			sum(TienMat) as TienMatAll,
    			sum(TienATM) as TienATMAll,
    			sum(TienGui) as TienGuiAll,
    			sum(KhachDaTra) as KhachDaTraAll
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
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

			Sql(@"ALTER PROCEDURE [dbo].[GetNhatKySuDung_GDV]
    @IDChiNhanhs [nvarchar](max) = null,
    @IDCustomers [nvarchar](max) = null,  
	@TextSearch nvarchar(max) = null,
	@DateFrom datetime = null,
	@DateTo datetime = null,
	@LoaiHoaDons [nvarchar](max) = null,
    @CurrentPage [int] = null,
    @PageSize [int] = null
AS
BEGIN
    SET NOCOUNT ON;
    	declare @sql nvarchar(max) ='', @where nvarchar(max), @paramDefined nvarchar(max)
    	declare @tblDefined nvarchar(max)= N' declare @tblChiNhanh table(ID uniqueidentifier)
    								declare @tblCus table(ID uniqueidentifier)
    								declare @tblCar table(ID uniqueidentifier)'
    
    	set @where = N' where 1 = 1 and hd.LoaiHoaDon = 1 and hd.ChoThanhToan = 0  
						and (ct.ID_ChiTietDinhLuong= ct.id OR ct.ID_ChiTietDinhLuong IS NULL) 
						and (ct.ID_ParentCombo != ct.ID OR ct.ID_ParentCombo IS NULL)'
    
    	if isnull(@CurrentPage,'') =''
    		set @CurrentPage = 0
    	if isnull(@PageSize,'') =''
    		set @PageSize = 20

		if isnull(@LoaiHoaDons,'') !=''
			begin
				if @LoaiHoaDons = 19 ---- hoadon dudung GDV
    				set @where = CONCAT(@where, N' and ct.ChatLieu = ''4''')
			end

    	if isnull(@IDChiNhanhs,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblChiNhanh cn where ID_DonVi = cn.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ;')
    		end
    	if isnull(@IDCustomers,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblCus select name from dbo.splitstring(@IDCustomers_In) ;')
    		end
    	
    	if isnull(@DateFrom,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and hd.NgayLapHoaDon > @DateFrom_In')
    			
    		end
		if isnull(@DateTo,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and hd.NgayLapHoaDon < @DateTo_In')    			
    		end

    	if isnull(@TextSearch,'') !=''
    		begin
    			set @where = CONCAT(@where , N' and (hd.MaHoaDon like N''%'' + @TextSearch_In + ''%''  
							or dt.MaDoiTuong like N''%'' + @TextSearch_In + ''%'' or  dt.TenDoiTuong like N''%'' + @TextSearch_In + ''%'' 
							or dt.TenDoiTuong_KhongDau like N''%'' + @TextSearch_In + ''%'' or dt.DienThoai like N''%'' + @TextSearch_In + ''%''
							or hh.TenHangHoa like N''%'' + @TextSearch_In + ''%'' or  hh.TenHangHoa_KhongDau like N''%'' + @TextSearch_In + ''%''
							or qd.MaHangHoa like N''%'' + @TextSearch_In + ''%'')' )
    			
    		end
    	
    	set @sql = CONCAT(@tblDefined, @sql, N'
    		;with data_cte
    as (
		SELECT ct.ID as ID_ChiTietGoiDV,
			hd.MaHoaDon, 
			hd.NgayLapHoaDon,
			hd.ID_DoiTuong, 
			dt.MaDoiTuong,
			dt.TenDoiTuong,
    		qd.MaHangHoa,
			isnull(qd.ThuocTinhGiaTri,'''') as ThuocTinh_GiaTri,
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='''', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoa,
    		hh.TenHangHoa_KhongDau,		
			hh.GhiChu as GhiChuHH,
			hh.LaHangHoa,
			case when hh.LaHangHoa = 1 then ''0'' else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
    		Case when hh.LaHangHoa=1 then ''0'' else ISNULL(hh.ChiPhiTinhTheoPT,''0'') end as LaPTPhiDichVu,
			isnull(hh.ID_NhomHang,''00000000-0000-0000-0000-000000000000'') as ID_NhomHangHoa,
			ISNULL(qd.LaDonViChuan,0) as LaDonViChuan, 
			CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
			isnull(hh.QuanLyTheoLoHang,''0'') as QuanLyTheoLoHang,
			ISNULL(hh.ChietKhauMD_NVTheoPT,''1'') as ChietKhauMD_NVTheoPT,
			ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
			CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
    		lo.MaLoHang, 
			lo.NgaySanXuat, 
			lo.NgayHetHan,
			qd.TenDonViTinh,
			qd.ID_HangHoa,
    		ct.SoLuong as SoLuongMua,
			ct.ID_DonViQuiDoi,
			ct.ID_LoHang,
			ct.DonGia as GiaBan,	
			ct.ThoiGianBaoHanh,
			ct.LoaiThoiGianBH,
			ct.GhiChu,
			ct.SoThuTu,
			isnull(gv.GiaVon,0) as GiaVon,
			isnull(tk.TonKho,0) as TonKho,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			nhomhh.TenNhomHangHoa,
    		hdXMLOut.HDCT_NhanVien as NhanVienThucHien,
    		CT_ChietKhauNV.TongChietKhau
    	FROM BH_HoaDon_ChiTiet ct
    	join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.id
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		left join DM_NhomHangHoa nhomhh on hh.ID_NhomHang = nhomhh.ID
		left join DM_LoHang lo on ct.ID_LoHang = lo.ID
		left join DM_GiaVon gv on ct.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and gv.ID_DonVi = hd.ID_DonVi
		left join DM_HangHoa_TonKho tk on ct.ID_DonViQuiDoi = tk.ID_DonViQuyDoi and tk.ID_DonVi = hd.ID_DonVi
    	left join 
    			(Select distinct hdXML.ID,
    					(
    					select distinct (nv.TenNhanVien) +'', ''  AS [text()]
    					from BH_HoaDon_ChiTiet ct2
    					left join BH_NhanVienThucHien nvth on ct2.ID = nvth.ID_ChiTietHoaDon
    					left join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    					where ct2.ID = hdXML.ID 
    					For XML PATH ('''')
    				) HDCT_NhanVien
    			from BH_HoaDon_ChiTiet hdXML) hdXMLOut on ct.ID= hdXMLOut.ID
    	 left join 
    			(select ct3.ID, SUM(isnull(nvth2.TienChietKhau,0)) as TongChietKhau from BH_HoaDon_ChiTiet ct3
    			left join BH_NhanVienThucHien nvth2 on ct3.ID = nvth2.ID_ChiTietHoaDon
    			group by ct3.ID) CT_ChietKhauNV on CT_ChietKhauNV.ID = ct.ID        
    	', @where, 
    		'),
    		count_cte
    		as (
    			select count(ID_ChiTietGoiDV) as TotalRow,
    				CEILING(COUNT(ID_ChiTietGoiDV) / CAST(@PageSize_In as float ))  as TotalPage,
    				sum(SoLuongMua) as TongSoLuong,
    				sum(TongChietKhau) as TongHoaHong			
    			from data_cte
    		)
    	select dt.*,
    		cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage_In * @PageSize_In) ROWS
    		FETCH NEXT @PageSize_In ROWS ONLY ')
    
    		print @sql
    
    		set @paramDefined =N'
    			@IDChiNhanhs_In nvarchar(max),
    			@IDCustomers_In nvarchar(max),
				@TextSearch_In nvarchar(max),
				@DateFrom_In datetime,
				@DateTo_In datetime,
				@LoaiHoaDons_In nvarchar(max),
    			@CurrentPage_In int,
    			@PageSize_In int'
    
    		exec sp_executesql @sql, 
    		@paramDefined,
    		@IDChiNhanhs_In = @IDChiNhanhs,
    		@IDCustomers_In = @IDCustomers,
			@TextSearch_In = @TextSearch,
			@DateFrom_In = @DateFrom,
			@DateTo_In = @DateTo,
			@LoaiHoaDons_In = @LoaiHoaDons,
    		@CurrentPage_In = @CurrentPage,
    		@PageSize_In = @PageSize
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetSoDuTheGiaTri_ofKhachHang]
    @ID_DoiTuong [uniqueidentifier],
    @DateTime [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	set @DateTime= DATEADD(DAY,1,@DateTime)
    	select 
    		TongThuTheGiaTri,
			TraLaiSoDu,
			SuDungThe, 
			HoanTraTheGiatri,
    		ThucThu,
			PhaiThanhToan,
			SoDuTheGiaTri,
    		iif(CongNoThe<0,0,CongNoThe) as CongNoThe
    	from
    	(
    	select 		
    		sum(TongThuTheGiaTri) - sum(TraLaiSoDu) as TongThuTheGiaTri, 
			sum(TraLaiSoDu) as TraLaiSoDu,
    		cast(sum(SuDungThe) as float) as SuDungThe,
    		cast(sum(HoanTraTheGiatri) as float) as HoanTraTheGiatri,
    		cast(sum(ThucThu) as float) as ThucThu,
    		cast(sum(PhaiThanhToan) as float) as PhaiThanhToan,
    		cast(SUM(ThucThu)- sum(TraLaiSoDu)  - SUM(SuDungThe) + SUM(HoanTraTheGiatri) as float) as SoDuTheGiaTri, --- kangjin: soddu = tongthuthucte - sudung + hoantra
    		cast(sum(PhaiThanhToan) - sum(TraLaiSoDu) - sum(ThucThu) as float) as CongNoThe
    	from (
    		-- so du nap the va thuc te phai thanh toan
    		SELECT 
    			TongTienHang as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			hd.PhaiThanhToan, -- dieu chinh the (khong lien quan den cong no)
				0 as TraLaiSoDu
    		FROM BH_HoaDon hd
    		where hd.ID_DoiTuong like @ID_DoiTuong and hd.ChoThanhToan ='0' and hd.LoaiHoaDon in (22,23) 
    		and hd.NgayLapHoaDon  < @DateTime
    
    		union all
    		-- su dung the
    		SELECT 
    			0 as TongThuTheGiaTri,
    			SUM(qct.TienThu) as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu
    		FROM Quy_HoaDon_ChiTiet qct
    		INNER JOIN Quy_HoaDon qhd
    		ON qct.ID_HoaDon = qhd.ID
    		WHERE qct.ID_DoiTuong like @ID_DoiTuong AND qhd.NgayLapHoaDon  < @DateTime and qhd.LoaiHoaDon = 11 
    		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    		and qct.HinhThucThanhToan=4

			
    		union all
    		---- hoàn trả số dư còn trong TGT cho khách --> giảm số dư
    		SELECT
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				SUM(hd.TongTienHang) as TraLaiSoDu
    		FROM BH_HoaDon hd
    		where hd.LoaiHoaDon= 32 and hd.ChoThanhToan= 0
			and hd.ID_DoiTuong like @ID_DoiTuong
    	
    		union all
    		-- hoàn trả tiền vào TGT ---> tăng số dư
    		SELECT
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			SUM(qct.TienThu) as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu
    		FROM Quy_HoaDon_ChiTiet qct
    		INNER JOIN Quy_HoaDon qhd
    		ON qct.ID_HoaDon = qhd.ID
    		WHERE qct.ID_DoiTuong like @ID_DoiTuong AND qhd.NgayLapHoaDon  < @DateTime and qhd.LoaiHoaDon = 12
    		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    			and qct.HinhThucThanhToan=4
    
    		union all
    		-- thuc thu thegiatri
    		SELECT
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			qct.TienThu as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu
    		from Quy_HoaDon_ChiTiet qct
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    		join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    		where hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and qhd.NgayLapHoaDon < @DateTime and qct.ID_DoiTuong like @ID_DoiTuong
    		and (qhd.PhieuDieuChinhCongNo= 0 or PhieuDieuChinhCongNo  is  null)
			and (qhd.TrangThai = 1 or qhd.TrangThai is null)

    		-- thucthu do dieuchinh congno khachhang
    		union all
    		select
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			qct.TienThu as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu
    		from Quy_HoaDon_ChiTiet qct
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    		where qhd.PhieuDieuChinhCongNo= 1 and qhd.LoaiHoaDon= 11
    		and (qhd.TrangThai= 1 or qhd.TrangThai is null)
    		and qct.ID_DoiTuong like @ID_DoiTuong
    		) tbl  
    		) tbl2
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetTonKho_byIDQuyDois]
    @ID_ChiNhanh [uniqueidentifier],
    @ToDate [datetime],
    @IDDonViQuyDois [nvarchar](max),
    @IDLoHangs [nvarchar](max)
AS
BEGIN
	 SET NOCOUNT ON;
    	declare @tblIDQuiDoi table (ID_DonViQuyDoi uniqueidentifier)
    	declare @tblIDLoHang table (ID_LoHang uniqueidentifier)
    
    	insert into @tblIDQuiDoi
    	select Name from dbo.splitstring(@IDDonViQuyDois) 
    	insert into @tblIDLoHang
    	select Name from dbo.splitstring(@IDLoHangs) where Name not like '%null%' and Name !=''

		
		---- get tonluyke theo thoigian 
		SELECT 
    		ID_DonViQuiDoi,
    		ID_HangHoa, 		
    		ID_LoHang,		
    		IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, TonLuyKe_NhanChuyenHang, TonLuyKe) AS TonKho, 
    		IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, GiaVon_NhanChuyenHang, GiaVon) AS GiaVon
		into #tblTon
    	FROM (
    		SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang, 
			tbltemp.ID_DonViInput ORDER BY tbltemp.ThoiGian DESC) AS RN 
		
    	FROM (
				select 
						hd.LoaiHoaDon, 
						hd.ID_DonVi,
						dvqd.ID_HangHoa,
						ct.ID_DonViQuiDoi,
						hd.ID_CheckIn, 					
						@ID_ChiNhanh as ID_DonViInput, 
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
						ct.TonLuyKe_NhanChuyenHang, ct.TonLuyKe) AS TonLuyKe,
    					ct.TonLuyKe_NhanChuyenHang,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
    					ct.GiaVon_NhanChuyenHang, 
    					ct.GiaVon)/ISNULL(dvqd.TyLeChuyenDoi,1) AS GiaVon,
    					ct.GiaVon_NhanChuyenHang, 
    					ct.ID_LoHang ,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh,
						hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
				from @tblIDQuiDoi qd
				join BH_HoaDon_ChiTiet ct on qd.ID_DonViQuyDoi = ct.ID_DonViQuiDoi
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
				join DonViQuiDoi dvqd on qd.ID_DonViQuyDoi= dvqd.ID
				join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			where (hd.ID_DonVi= @ID_ChiNhanh or (hd.ID_CheckIn = @ID_ChiNhanh and hd.YeuCau = '4'))
			and hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10,18,2,16) --- 2.hd baohanh, 16. dieuchinh gvtieuchuan
			and (exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= ct.ID_LoHang) Or hh.QuanLyTheoLoHang= 0)    
			) as tbltemp
    	WHERE tbltemp.ThoiGian < @ToDate) tblTonKhoTemp
    	WHERE tblTonKhoTemp.RN = 1;


		---- get giavon tieuchuan theo thoigian
		declare @tblGVTieuChuan table(ID_DonVi uniqueidentifier,
							ID_DonViQuiDoi uniqueidentifier,
							ID_LoHang uniqueidentifier, 
							GiaVonTieuChuan	float)
		insert into @tblGVTieuChuan
		exec GetGiaVonTieuChuan_byTime @ID_ChiNhanh, @ToDate, @IDDonViQuyDois, @IDLoHangs 

	

		select TenHangHoa, 
			lo.MaLoHang,
			qd2.ID_HangHoa,
			qd.ID_DonViQuyDoi as ID_DonViQuiDoi,
			qd2.GiaNhap,
			lo.ID as ID_LoHang, 
			isnull(tk.TonKho,0) as TonKho,
			isnull(tk.GiaVon,0) as GiaVon,
			isnull(gvtc.GiaVonTieuChuan,0) as GiaVonTieuChuan
		from @tblIDQuiDoi qd 	
		join DonViQuiDoi qd2 on qd.ID_DonViQuyDoi= qd2.ID 
		join DM_HangHoa hh on hh.ID = qd2.ID_HangHoa
		left join DM_LoHang lo on hh.ID = lo.ID_HangHoa and hh.QuanLyTheoLoHang = 1   
		left join @tblGVTieuChuan gvtc on qd.ID_DonViQuyDoi = gvtc.ID_DonViQuiDoi and (lo.ID = gvtc.ID_LoHang or (gvtc.ID_LoHang is null and lo.ID is null) )
		left join #tblTon tk on qd.ID_DonViQuyDoi = tk.ID_DonViQuiDoi and qd2.ID = tk.ID_DonViQuiDoi
		and (tk.ID_LoHang = lo.ID or hh.QuanLyTheoLoHang =0)
		where (exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= lo.ID) Or hh.QuanLyTheoLoHang= 0)
		order by qd.ID_DonViQuyDoi, lo.ID
END");

			Sql(@"ALTER PROCEDURE [dbo].[ListHangHoaTheKho]
    @ID_HangHoa [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT ID_HoaDon, 
		MaHoaDon, 
		NgayLapHoaDon,
		LoaiHoaDon, 
		ID_DonVi,
		ID_CheckIn,		
		sum(table1.SoLuong) as SoLuong,
		max(table1.GiaVon) as GiaVon,
		max(table1.TonKho) as TonKho,
	case table1.LoaiHoaDon
			when 10 then case when table1.ID_CheckIn = @IDChiNhanh then N'Nhận chuyển hàng' else N'Chuyển hàng' end
			when 1 then N'Bán hàng'
			when 2 then N'Bảo hành'
			when 4 then N'Nhập hàng'
			when 6 then N'Khách trả hàng'
			when 7 then N'Trả hàng nhập'
			when 8 then N'Xuất kho'
			when 9 then N'Kiểm hàng'
			when 13 then N'Nhập nội bộ'
			when 14 then N'Nhập hàng thừa'
			when 18 then N'Điều chỉnh giá vốn'			
		end as LoaiChungTu
	FROM
	(
		SELECT hd.ID as ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, 
		CASE WHEN hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4' and hd.LoaiHoaDon = 10 THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon,
		bhct.ThanhTien * dvqd.TyLeChuyenDoi as ThanhTien,
		bhct.TienChietKhau * dvqd.TyLeChuyenDoi TienChietKhau, 
		dvqd.TyLeChuyenDoi,
		hd.YeuCau, 
		hd.ID_CheckIn,
		hd.ID_DonVi,
		hh.QuanLyTheoLoHang,
		dvqd.LaDonViChuan, 
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.TonLuyKe, bhct.TonLuyKe_NhanChuyenHang) as TonKho,
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.GiaVon / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi),bhct.GiaVon_NhanChuyenHang / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi)) as GiaVon,	
		bhct.SoThuTu,
		(case hd.LoaiHoaDon
			when 9 then bhct.SoLuong ---- Số lượng lệch = SLThucTe - TonKhoDB        (-) Giảm  (+): Tăng
			when 10 then
				case when hd.ID_CheckIn= @IDChiNhanh and hd.YeuCau = '4' then bhct.TienChietKhau  ---- da nhanhang
				else -bhct.SoLuong end 
			--- xuat
			when 1 then - bhct.SoLuong
			when 2 then - bhct.SoLuong
			when 7 then - bhct.SoLuong
			when 8 then - bhct.SoLuong
			--- conlai: nhap
			else bhct.SoLuong end
		) * dvqd.TyLeChuyenDoi as SoLuong
		
	FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	WHERE hd.LoaiHoaDon not in (3 , 25,29,31)
	AND IIF(hd.LoaiHoaDon != 6, 1, 
	IIF(bhct.ChatLieu != '2' OR bhct.ChatLieu IS NULL, 1, 0)) = 1 
	and hd.LoaiHoaDon != 19 and hh.ID = @ID_HangHoa and hd.ChoThanhToan = 0 
	and ((hd.ID_DonVi = @IDChiNhanh and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)) or (hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4'))
	)  table1
	group by ID_HoaDon, MaHoaDon, NgayLapHoaDon,LoaiHoaDon, ID_DonVi, ID_CheckIn
	ORDER BY NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[ListHangHoaTheKhoTheoLoHang]
    @ID_HangHoa [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier],
    @ID_LoHang [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;

	SELECT ID_HoaDon, 
		MaHoaDon, 
		NgayLapHoaDon,
		LoaiHoaDon, 
		ID_DonVi,
		ID_CheckIn,		
		sum(table1.SoLuong) as SoLuong,
		max(table1.GiaVon) as GiaVon,
		max(table1.TonKho) as TonKho,
		case table1.LoaiHoaDon
			when 10 then case when table1.ID_CheckIn = @IDChiNhanh then N'Nhận chuyển hàng' else N'Chuyển hàng' end
			when 1 then N'Bán hàng'
			when 2 then N'Bảo hành'
			when 4 then N'Nhập hàng'
			when 6 then N'Khách trả hàng'
			when 7 then N'Trả hàng nhập'
			when 8 then N'Xuất kho'
			when 9 then N'Kiểm hàng'
			when 13 then N'Nhập kho nội bộ'
			when 14 then N'Nhập hàng khách thừa'
			when 18 then N'Điều chỉnh giá vốn'
		end as LoaiChungTu
		FROM
	(
    	SELECT hd.ID as ID_HoaDon, 
		hd.MaHoaDon, 
		hd.LoaiHoaDon, 
		hd.YeuCau, 
		hd.ID_CheckIn, 
		hd.ID_DonVi, 
		bhct.ThanhTien * dvqd.TyLeChuyenDoi as ThanhTien,
		bhct.TienChietKhau * dvqd.TyLeChuyenDoi TienChietKhau, 
		CASE WHEN hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4' and hd.LoaiHoaDon = 10 THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon,    		
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.TonLuyKe, bhct.TonLuyKe_NhanChuyenHang) as TonKho,
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.GiaVon / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi),bhct.GiaVon_NhanChuyenHang / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi)) as GiaVon,	
		(case hd.LoaiHoaDon
			when 9 then bhct.SoLuong ---- Số lượng lệch = SLThucTe - TonKhoDB        (-) Giảm  (+): Tăng
			when 10 then
				case when hd.ID_CheckIn= @IDChiNhanh and hd.YeuCau = '4' then bhct.TienChietKhau  ---- da nhanhang
				else -bhct.SoLuong end 
			--- xuat
			when 1 then - bhct.SoLuong
			when 2 then - bhct.SoLuong ---- hoadon baohanh
			when 7 then - bhct.SoLuong
			when 8 then - bhct.SoLuong
			--- conlai: nhap
			else bhct.SoLuong end
		) * dvqd.TyLeChuyenDoi as SoLuong
    	FROM BH_HoaDon hd
    	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
    	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID		
    	WHERE bhct.ID_LoHang = @ID_LoHang 
		and hd.LoaiHoaDon not in (3,19,25,31) 
		and hh.ID = @ID_HangHoa 
		and hd.ChoThanhToan = 0 
		and (bhct.ChatLieu is null or bhct.ChatLieu!='2')
		and ((hd.ID_DonVi = @IDChiNhanh 
		and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)) or (hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4'))
	) as table1
    group by ID_HoaDon, MaHoaDon, NgayLapHoaDon,LoaiHoaDon, ID_DonVi, ID_CheckIn
	ORDER BY NgayLapHoaDon desc
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

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountInvoice]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs [nvarchar](max),
    @TextSearch [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @Status_DoanhThu [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	set @DateTo = dateadd(day,1, @DateTo) 

		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');

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
    
		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @tblDiscountInvoice table (ID uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), 
    		HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float)
    	
    	-- bang tam chua DS phieu thu theo Ngay timkiem
    	select qct.ID_HoaDonLienQuan, SUM(qct.TienThu) as ThucThu, qhd.NgayLapHoaDon, qhd.ID
    	into #temp
    	from Quy_HoaDon_ChiTiet qct
    	join (
    			select qhd.ID, qhd.NgayLapHoaDon
    			from Quy_HoaDon qhd
    			join BH_NhanVienThucHien th on qhd.ID= th.ID_QuyHoaDon
    			where (qhd.TrangThai is null or qhd.TrangThai = '1')
    			and qhd.ID_DonVi in (select ID from @tblChiNhanh)
    			and qhd.NgayLapHoaDon >= @DateFrom
    			and qhd.NgayLapHoaDon < @DateTo
    			group by qhd.ID, qhd.NgayLapHoaDon) qhd on qct.ID_HoaDon = qhd.ID
    	where (qct.HinhThucThanhToan is null or qct.HinhThucThanhToan != 4)
    	group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID;
    	
    	select ID, MaNhanVien, 
    			TenNhanVien,
    		SUM(ISNULL(HoaHongThucThu,0.0)) as HoaHongThucThu,
    		SUM(ISNULL(HoaHongDoanhThu,0.0)) as HoaHongDoanhThu,
    		SUM(ISNULL(HoaHongVND,0.0)) as HoaHongVND,			
    		case @Status_ColumHide
    			when  1 then cast(0 as float)
    			when  2 then SUM(ISNULL(HoaHongVND,0.0))
    			when  3 then SUM(ISNULL(HoaHongThucThu,0.0))
    			when  4 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    			when  5 then SUM(ISNULL(HoaHongDoanhThu,0.0)) 
    			when  6 then SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    			when  7 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0))
    		else SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    		end as TongAll
    		into #temp2
    	from 
    	(
    		select nv.ID, MaNhanVien, TenNhanVien,
    			case when TinhChietKhauTheo =1 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau  end end as HoaHongThucThu,
    				case when TinhChietKhauTheo =3 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end as HoaHongVND,
    				-- neu HD tao thang truoc, nhung PhieuThu thuoc thang nay: HoaHongDoanhThu = 0
    				case when hd.NgayLapHoaDon between @DateFrom and @DateTo and hd.ID_DonVi in (select ID from @tblChiNhanh) then
    					case when TinhChietKhauTheo = 2 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end else 0 end as HoaHongDoanhThu,
    				-- timkiem theo NgayLapHD or NgayLapPhieuThu
    				case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then hd.NgayLapHoaDon else tblQuy.NgayLapHoaDon end as NgayLapHoaDon,
    				case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    		from BH_NhanVienThucHien th
    		join BH_HoaDon hd on th.ID_HoaDon= hd.ID
    		join NS_NhanVien nv on th.ID_NhanVien= nv.ID
    			left join #temp tblQuy on hd.ID= tblQuy.ID_HoaDonLienQuan and (th.ID_QuyHoaDon= tblQuy.ID)	
    		where th.ID_HoaDon is not null
    		and hd.ChoThanhToan  is not null
    		and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))  
				and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    			and (@DepartmentIDs ='' or exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID))
    			-- chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
    			and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 and ( exists (select ID from #temp where th.ID_QuyHoaDon = #temp.ID) or  LoaiHoaDon=6)))
    			and
    				((select count(Name) from @tblSearchString b where     			
    				nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'				
    				)=@count or @count=0)	
    	) tbl
    		where tbl.NgayLapHoaDon >= @DateFrom and tbl.NgayLapHoaDon < @DateTo and TrangThaiHD = @StatusInvoice
    	group by MaNhanVien, TenNhanVien, ID
    		having SUM(ISNULL(HoaHongThucThu,0)) + SUM(ISNULL(HoaHongDoanhThu,0)) + SUM(ISNULL(HoaHongVND,0)) > 0 -- chi lay NV co CK > 0
    		
    		if @Status_DoanhThu =0
    			insert into @tblDiscountInvoice
    			select *
    			from #temp2 
    		else
    			begin
    				if @Status_DoanhThu= 1
    					insert into @tblDiscountInvoice
    					select *
    					from #temp2 where HoaHongDoanhThu > 0 or HoaHongThucThu > 0
    				else
    					if @Status_DoanhThu= 2
    						insert into @tblDiscountInvoice
    						select *
    						from #temp2 where HoaHongDoanhThu > 0 or HoaHongVND > 0
    					else		
    						if @Status_DoanhThu= 3
    							insert into @tblDiscountInvoice
    							select *
    							from #temp2 where HoaHongDoanhThu > 0
    						else	
    							if @Status_DoanhThu= 4
    								insert into @tblDiscountInvoice
    								select *
    								from #temp2 where HoaHongVND > 0 Or HoaHongThucThu > 0
    							else
    								if @Status_DoanhThu= 5
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2 where  HoaHongThucThu > 0
    								else -- 6
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2  where HoaHongVND > 0
    								
    			end;
    			
    		with data_cte
    		as(
    		select * from @tblDiscountInvoice
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
    				sum(HoaHongThucThu) as TongHoaHongThucThu,
    				sum(HoaHongVND) as TongHoaHongVND,
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

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountInvoice_Detail]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs [nvarchar](max),
    @TextSearch [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @Status_DoanhThu [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;

		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');
		
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

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    
    	set @DateTo = DATEADD(day,1,@DateTo)
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @tblDiscountInvoice table (ID uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), NgayLapHoaDon datetime, NgayLapPhieu datetime, NgayLapPhieuThu datetime, MaHoaDon nvarchar(50),
    		DoanhThu float,
			ThucThu float,
			ChiPhiNganHang float,
			TongChiPhiNganHang float,
			ThucThu_ThucTinh float,
			HeSo float, HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, PTThucThu float, PTDoanhThu float, 
    		MaKhachHang nvarchar(max), TenKhachHang nvarchar(max), DienThoaiKH nvarchar(max), TongAll float)
    
  --  	-- bang tam chua DS phieu thu theo Ngay timkiem
		select qct.ID_HoaDonLienQuan, 
			qhd.ID,
			qhd.NgayLapHoaDon, 
			max(isnull(qct.ChiPhiNganHang,0)) as ChiPhiNganHang,
			SUM(iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu)) as ThucThu,
			---- chi get chiphi with POS
			sum(iif(qct.HinhThucThanhToan != 2,0, iif(qct.LaPTChiPhiNganHang='0', qct.ChiPhiNganHang,  qct.ChiPhiNganHang * qct.TienThu/100))) as TongChiPhiNganHang					
    	into #tempQuy
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID 
		where (qhd.TrangThai is null or qhd.TrangThai = '1')
		and qhd.ID_DonVi in (select ID from @tblChiNhanh)
		and qhd.NgayLapHoaDon >= @DateFrom
    	and qhd.NgayLapHoaDon < @DateTo 
		and qct.HinhThucThanhToan not in (4,5)
    	group by  qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID --, qct.LaPTChiPhiNganHang

		---- thucthu theo hoadon
		select ctquy.*, tblTong.TongThuThucTe
		into #tblQuyThucTe
		from #tempQuy ctquy
		left join
		(
		select ID_HoaDonLienQuan,		
			sum(ThucThu) as TongThuThucTe
		from #tempQuy
		group by ID_HoaDonLienQuan
		) tblTong on ctquy.ID_HoaDonLienQuan= tblTong.ID_HoaDonLienQuan
		
    
    		select
				tbl.ID, ---- id of hoadon
				MaNhanVien, 
    			tbl.TenNhanVien,
    			tbl.NgayLapHoaDon,
    			tbl.NgayLapPhieu, -- used to check at where condition
    			tbl.NgayLapPhieuThu,
    			tbl.MaHoaDon,						
    			-- taoHD truoc, PhieuThu sau --> khong co doanh thu
    			case when  tbl.NgayLapHoaDon between @DateFrom and @DateTo then PhaiThanhToan else 0 end as DoanhThu, 
    			ISNULL(ThucThu,0) as ThucThu,
				tbl.ChiPhiNganHang,
				tbl.TongChiPhiNganHang,
				tbl.ThucThu - tbl.TongChiPhiNganHang as ThucThu_ThucTinh,
    			ISNULL(HeSo,0) as HeSo,
    			ISNULL(HoaHongThucThu,0) as HoaHongThucThu,
    			ISNULL(HoaHongDoanhThu,0) as HoaHongDoanhThu,
    			ISNULL(HoaHongVND,0) as HoaHongVND,
    			ISNULL(PTThucThu,0) as PTThucThu,
    			ISNULL(PTDoanhThu,0) as PTDoanhThu,
    			ISNULL(MaDoiTuong,'') as MaKhachHang,
    			ISNULL(TenDoiTuong,N'Khách lẻ') as TenKhachHang,
    			ISNULL(dt.DienThoai,'') as DienThoaiKH,		
    		case @Status_ColumHide
    			when  1 then cast(0 as float)
    			when  2 then ISNULL(HoaHongVND,0.0)
    			when  3 then ISNULL(HoaHongThucThu,0.0)
    			when  4 then ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongVND,0.0)
    			when  5 then ISNULL(HoaHongDoanhThu,0.0) 
    			when  6 then ISNULL(HoaHongDoanhThu,0.0) + ISNULL(HoaHongVND,0.0)
    			when  7 then ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongDoanhThu,0.0)
    		else ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongDoanhThu,0.0) + ISNULL(HoaHongVND,0.0)
    		end as TongAll
    		into #temp2
    	from 
    	(    		
    				select distinct MaNhanVien, TenNhanVien, hd.MaHoaDon, 
    					case when hd.LoaiHoaDon= 6 then - TongThanhToan + isnull(TongTienThue,0)
    					else case when hd.ID_DonVi in (select ID from @tblChiNhanh) then
							iif(hd.LoaiHoaDon=22, PhaiThanhToan, TongThanhToan - TongTienThue) else 0 end end as PhaiThanhToan,
    					hd.NgayLapHoaDon,
						tblQuy.ThucThu ,	
						tblQuy.ChiPhiNganHang ,					
						tblQuy.TongChiPhiNganHang,
						hd.LoaiHoaDon,
    					hd.ID_DoiTuong,
						hd.ID,
    					th.HeSo,
    					tblQuy.NgayLapHoaDon as NgayLapPhieuThu,
    				-- huy PhieuThu --> PTThucThu,HoaHongThucThu = 0		
    					case when TinhChietKhauTheo =1 
    						then case when LoaiHoaDon= 6 then -TienChietKhau else 
    							case when ISNULL(ThucThu,0)= 0 then 0  else TienChietKhau end end end as HoaHongThucThu,
    					case when TinhChietKhauTheo =1 
    						then case when LoaiHoaDon= 6 then PT_ChietKhau else 
    							case when ISNULL(ThucThu,0)= 0 then 0  else PT_ChietKhau end end end as PTThucThu,			    				
    					case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then
    						case when TinhChietKhauTheo = 2 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end else 0 end as HoaHongDoanhThu,
    					case when TinhChietKhauTheo =3 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end as HoaHongVND,
    					case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then
    						case when TinhChietKhauTheo = 2 then PT_ChietKhau end else 0 end as PTDoanhThu,
    					-- timkiem theo NgayLapHD or NgayLapPhieuThu
    					case when @DateFrom <= hd.NgayLapHoaDon and hd.NgayLapHoaDon < @DateTo then hd.NgayLapHoaDon else tblQuy.NgayLapHoaDon end as NgayLapPhieu ,
    					case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    			
    			from BH_NhanVienThucHien th		
    			join NS_NhanVien nv on th.ID_NhanVien= nv.ID
    			join BH_HoaDon hd on th.ID_HoaDon= hd.ID
    			left join #tblQuyThucTe tblQuy on th.ID_QuyHoaDon = tblQuy.ID and tblQuy.ID_HoaDonLienQuan = hd.ID --- join hoadon (truong hop phieuthu nhieu hoadon)
    			where th.ID_HoaDon is not null
    				and hd.LoaiHoaDon in (1,19,22,6, 25,3)
    				and hd.ChoThanhToan is not null    				
					and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    				and (exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID))
    				--chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
    				and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 
					and ( exists (select ID from #tempQuy where th.ID_QuyHoaDon = #tempQuy.ID) or  LoaiHoaDon=6)))		
    				and
    				((select count(Name) from @tblSearchString b where     			
    				nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'	
    				or hd.MaHoaDon like '%'+b.Name+'%'								
    				)=@count or @count=0)	
    	) tbl
    		left join DM_DoiTuong dt on tbl.ID_DoiTuong= dt.ID
    		where tbl.NgayLapPhieu >= @DateFrom and tbl.NgayLapPhieu < @DateTo and TrangThaiHD = @StatusInvoice

    
    	if @Status_DoanhThu =0
    		insert into @tblDiscountInvoice
    		select *
    		from #temp2
    	else
    		begin
    				if @Status_DoanhThu= 1
    					insert into @tblDiscountInvoice
    					select *
    					from #temp2 where HoaHongDoanhThu > 0 or HoaHongThucThu > 0
    				else
    					if @Status_DoanhThu= 2
    						insert into @tblDiscountInvoice
    						select *
    						from #temp2 where HoaHongDoanhThu > 0 or HoaHongVND > 0
    					else		
    						if @Status_DoanhThu= 3
    							insert into @tblDiscountInvoice
    							select *
    							from #temp2 where HoaHongDoanhThu > 0
    						else	
    							if @Status_DoanhThu= 4
    								insert into @tblDiscountInvoice
    								select *
    								from #temp2 where HoaHongVND > 0 Or HoaHongThucThu > 0
    							else
    								if @Status_DoanhThu= 5
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2 where  HoaHongThucThu > 0
    								else -- 6
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2  where HoaHongVND > 0
    								
    			end;

				declare @tongDoanhThu float, @tongThucThu float

				select @tongDoanhThu = (select sum (tblDT.DoanhThu)
											from
											(
												select  id, MaHoaDon, NgayLapHoaDon, max(DoanhThu) as DoanhThu
												from @tblDiscountInvoice
												group by ID, MaHoaDon, NgayLapHoaDon
											)tblDT
										)
	
				select @tongThucThu = (select sum(tblTT.ThucThu)
										from
										(
											select sum(ThucThu) as ThucThu
											from
											(
											select  id, MaHoaDon, max(ThucThu)  as ThucThu
											from @tblDiscountInvoice
											group by ID, MaHoaDon, NgayLapPhieuThu
											) tbl2 group by ID, MaHoaDon
										)tblTT
										);
    
    	with data_cte
    		as(
    		select * from @tblDiscountInvoice
    		),
    		count_cte
    		as (
    			select count(*) as TotalRow,
    				CEILING(COUNT(*) / CAST(@PageSize as float ))  as TotalPage,
					@tongDoanhThu as TongDoanhThu,
					@tongThucThu as TongThucThu,
    				sum(HoaHongThucThu) as TongHoaHongThucThu,
    				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
    				sum(HoaHongVND) as TongHoaHongVND,
    				sum(TongAll) as TongAllAll,
					sum(TongChiPhiNganHang) as SumAllChiPhiNganHang,
					@tongThucThu - sum(TongChiPhiNganHang) as SumThucThu_ThucTinh
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountProduct_Detail]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs nvarchar(max),
    @ID_NhomHang [nvarchar](max),
	@LaHangHoas [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @TextSearch [nvarchar](max),
    @TextSearchHangHoa [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
    	set @DateTo = DATEADD(day,1,@DateTo)
    
		declare @tblLoaiHang table (LoaiHang int)
    	insert into @tblLoaiHang
    	select Name from dbo.splitstring(@LaHangHoas)

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)

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

			----- get nv thuoc PB
		declare @tblNhanVien table (ID uniqueidentifier)
		insert into @tblNhanVien
		select nv.ID
		from @tblNhanVienAll nv
		join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
		where exists (select ID_PhongBan from @tblDepartment pb where pb.ID_PhongBan= ct.ID_PhongBan) 
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    
    	DECLARE @tblSearchHH TABLE (Name [nvarchar](max));	
    INSERT INTO @tblSearchHH(Name) select  Name from [dbo].[splitstringByChar](@TextSearchHangHoa, ' ') where Name!='';
    DECLARE @countHH int =  (Select count(*) from @tblSearchHH);
    
    	declare @tblIDNhom table (ID uniqueidentifier);
    	if @ID_NhomHang='%%' OR @ID_NhomHang =''
    		begin
    			insert into @tblIDNhom
    			select ID from DM_NhomHangHoa
    		end
    	else
    		begin
    			insert into @tblIDNhom
    			select cast(Name as uniqueidentifier) from dbo.splitstring(@ID_NhomHang)
    		end;
    
    		select 
				ID_HoaDon,
				ID_ChiTietHoaDon,
				ID_DonViQuiDoi,
				ID_LoHang,
				ID_NhanVien,
				MaHoaDon, 
				LoaiHoaDon,
    			NgayLapHoaDon,
    			MaHangHoa,
    			MaNhanVien,
    			TenNhanVien,
    			TenNhomHangHoa,
    			ID_NhomHang,
    			TenHangHoa,
    			TenHangHoaFull,
    			TenDonViTinh,
    			TenLoHang,
    			ThuocTinh_GiaTri,
    			HoaHongThucHien,
    			PTThucHien,
    			HoaHongTuVan,
    			PTTuVan,
    			HoaHongBanGoiDV,
    			PTBanGoi,
    			HoaHongThucHien_TheoYC,
    			PTThucHien_TheoYC,
    			SoLuong,
    			ThanhTien,
    			HeSo,
				ThanhTien * HeSo as GtriSauHeSo,
    			ISNULL(MaDoiTuong,'') as MaKhachHang,
    			ISNULL(TenDoiTuong,N'Khách lẻ') as TenKhachHang,
    			ISNULL(dt.DienThoai,'') as DienThoaiKH,		
    		case @Status_ColumHide
    					when  1 then cast(0 as float)
    					when  2 then ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  3 then ISNULL(HoaHongBanGoiDV,0.0)
    					when  4 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  5 then ISNULL(HoaHongTuVan,0.0)
    					when  6 then ISNULL(HoaHongThucHien_TheoYC,0.0) + ISNULL(HoaHongTuVan,0.0)
    					when  7 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  8 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  9 then ISNULL(HoaHongThucHien,0.0)
    					when  10 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  11 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    					when  12 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  13 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  14 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  15 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    		else ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    		end as TongAll
			into #tblHoaHong
    		from
    		(
    				select 
							tbl.ID as ID_ChiTietHoaDon,
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
    						tbl.MaHoaDon,			
    						tbl.LoaiHoaDon,
    						tbl.NgayLapHoaDon,
    						tbl.ID_DoiTuong,
    						tbl.MaHangHoa,
    						tbl.ID_NhanVien,
    						TenHangHoa,
    						CONCAT(TenHangHoa,ThuocTinh_GiaTri) as TenHangHoaFull ,
    						TenDonViTinh,
    						ThuocTinh_GiaTri,
    						TenLoHang,
    						ID_NhomHang,
    						TenNhomHangHoa,
    						SoLuong,
    						--ThanhTien,
    						HeSo,
    						TrangThaiHD,
							
    						tbl.GiaTriTinhCK_NotCP - iif(tbl.LoaiHoaDon=19,0,tbl.TongChiPhiDV) as ThanhTien,

    						case when LoaiHoaDon=6 then - HoaHongThucHien else HoaHongThucHien end as HoaHongThucHien,
    						case when LoaiHoaDon=6 then - PTThucHien else PTThucHien end as PTThucHien,
    						case when LoaiHoaDon=6 then - HoaHongTuVan else HoaHongTuVan end as HoaHongTuVan,
    						case when LoaiHoaDon=6 then - PTTuVan else PTTuVan end as PTTuVan,
    						case when LoaiHoaDon=6 then - PTBanGoi else PTBanGoi end as PTBanGoi,
    						case when LoaiHoaDon=6 then - HoaHongBanGoiDV else HoaHongBanGoiDV end as HoaHongBanGoiDV,
    						case when LoaiHoaDon=6 then - HoaHongThucHien_TheoYC else HoaHongThucHien_TheoYC end as HoaHongThucHien_TheoYC,
    						case when LoaiHoaDon=6 then - PTThucHien_TheoYC else PTThucHien_TheoYC end as PTThucHien_TheoYC
    				from
    				(Select 
						hdct.ID_HoaDon,
						hdct.ID,
    					hd.MaHoaDon,			
    					hd.LoaiHoaDon,
    					hd.NgayLapHoaDon,
    					hd.ID_DoiTuong,
    					dvqd.MaHangHoa,
						hdct.ID_DonViQuiDoi,
						hdct.ID_LoHang,
    					ck.ID_NhanVien,
						hdct.SoLuong,
						IIF(hdct.TenHangHoaThayThe is null or hdct.TenHangHoaThayThe='', hh.TenHangHoa, hdct.TenHangHoaThayThe) as TenHangHoa,
						iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,    					
    					ISNULL(hh.ID_NhomHang,N'00000000-0000-0000-0000-000000000000') as ID_NhomHang,
    					ISNULL(nhh.TenNhomHangHoa,N'') as TenNhomHangHoa,

						case when hh.ChiPhiTinhTheoPT =1 then hdct.SoLuong * hdct.DonGia * hh.ChiPhiThucHien/100
							else hh.ChiPhiThucHien * hdct.SoLuong end as TongChiPhiDV,

						---- gtri cthd (truoc/sau CK)
						case when iif(ck.TinhHoaHongTruocCK is null,0,ck.TinhHoaHongTruocCK) = 1 
							then hdct.SoLuong * hdct.DonGia
							else hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau)
							end as GiaTriTinhCK_NotCP,

    					ISNULL(dvqd.TenDonVitinh,'')  as TenDonViTinh,
    					ISNULL(lh.MaLoHang,'')  as TenLoHang,
    					ck.HeSo,
    					Case when (dvqd.ThuocTinhGiaTri is null or dvqd.ThuocTinhGiaTri ='') then '' else '_' + dvqd.ThuocTinhGiaTri end as ThuocTinh_GiaTri,
    					Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    						Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTTuVan,
    						Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTBanGoi,
    					Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien_TheoYC,   				
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien_TheoYC,
    						case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    			
    																																		
    				from
    				BH_NhanVienThucHien ck
    				inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    				inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    					left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
    				left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    				Where hd.ChoThanhToan is not null
    					and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and hd.NgayLapHoaDon >= @DateFrom 
    					and hd.NgayLapHoaDon < @DateTo   							
    					and (exists (select ID from @tblNhanVien nv where ck.ID_NhanVien = nv.ID))
						and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    						and 
    						((select count(Name) from @tblSearchHH b where     									
    							 dvqd.MaHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
    							or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'	
    							)=@countHH or @countHH=0)
    			) tbl
				where tbl.LoaiHangHoa in (select LoaiHang from @tblLoaiHang)
    			) tblView
    			join NS_NhanVien nv on tblView.ID_NhanVien= nv.ID
    			left join DM_DoiTuong dt on tblView.ID_DoiTuong= dt.ID		
    			where tblView.TrangThaiHD = @StatusInvoice
    			and exists(select ID from @tblIDNhom a where ID_NhomHang= a.ID)
    			and
    				((select count(Name) from @tblSearchString b where     			
    					nv.TenNhanVien like N'%'+b.Name+'%'
    					or nv.TenNhanVienKhongDau like N'%'+b.Name+'%'
    					or nv.TenNhanVienChuCaiDau like N'%'+b.Name+'%'
    					or nv.MaNhanVien like N'%'+b.Name+'%'	
    					or tblView.MaHoaDon like '%'+b.Name+'%'	
    					)=@count or @count=0)	


				declare @TotalRow int, @TotalPage float, 
					@TongHoaHongThucHien float, @TongHoaHongThucHien_TheoYC float,
					@TongHoaHongTuVan float, @TongHoaHongBanGoiDV float,
					@TongAllAll float, @TongSoLuong float,
					@TongThanhTien float, @TongThanhTienSauHS float

				---- count all row		
				select 
					@TotalRow= count(tbl.ID_HoaDon),
    				@TotalPage= CEILING(COUNT(tbl.ID_HoaDon ) / CAST(@PageSize as float )) ,
    				@TongHoaHongThucHien= sum(HoaHongThucHien) ,
    				@TongHoaHongThucHien_TheoYC = sum(HoaHongThucHien_TheoYC),
    				@TongHoaHongTuVan = sum(HoaHongTuVan),
    				@TongHoaHongBanGoiDV = sum(HoaHongBanGoiDV),
					@TongAllAll = sum(TongAll)
				from #tblHoaHong tbl

				---- sum and group by hoadon + idquydoi
				select 
					@TongSoLuong= sum(SoLuong) ,			   				
    				@TongThanhTien = sum(ThanhTien),
					@TongThanhTienSauHS= sum(GtriSauHeSo) 
				from 
				(
					select  
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
							max(tbl.SoLuong) as SoLuong,
							max(tbl.ThanhTien) as ThanhTien,
							max(tbl.GtriSauHeSo) as GtriSauHeSo
					from #tblHoaHong tbl
					group by tbl.ID_HoaDon , tbl.ID_DonViQuiDoi ,tbl.ID_LoHang	, tbl.ID_ChiTietHoaDon	
				) tbl
				

				select tbl.*, 
					@TotalRow as TotalRow,
					@TotalPage as TotalPage,
					@TongHoaHongThucHien as TongHoaHongThucHien,
					@TongHoaHongThucHien_TheoYC as TongHoaHongThucHien_TheoYC,
					@TongHoaHongTuVan as TongHoaHongTuVan,
					@TongHoaHongBanGoiDV as TongHoaHongBanGoiDV,
					@TongAllAll as TongAllAll,
					@TongSoLuong as TongSoLuong,
					@TongThanhTien as TongThanhTien,
					@TongThanhTienSauHS as TongThanhTienSauHS
				from #tblHoaHong tbl
				order by tbl.NgayLapHoaDon desc
    			OFFSET (@CurrentPage* @PageSize) ROWS
    			FETCH NEXT @PageSize ROWS ONLY

				
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

			Sql(@"ALTER PROCEDURE [dbo].[ReportValueCard_Balance]
    @TextSearch [nvarchar](max),
    @ID_ChiNhanhs [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
	set @DateTo = DATEADD(day, 1, @DateTo)
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	with data_cte
    	as (
    		select 
    				tblView.ID, tblView.MaDoiTuong, tblView.TenDoiTuong, 
    				ISNULL(tblView.DienThoai,'') as DienThoaiKhachHang,
    				CAST(ISNULL(tblView.SoDuDauKy,0) as float) as SoDuDauKy,
    				CAST(ISNULL(tblView.PhatSinhTang,0) as float) as PhatSinhTang,
    				CAST(ISNULL(tblView.PhatSinhGiam,0) as float) as PhatSinhGiam,
    				ISNULL(tblView.SoDuDauKy,0) + ISNULL(tblView.PhatSinhTang,0)- ISNULL(tblView.PhatSinhGiam,0) as SoDuCuoiKy,
    				case when tblView.TrangThai_TheGiaTri is null or tblView.TrangThai_TheGiaTri = 1 then N'Đang hoạt động'
    				else N'Ngừng hoạt động' end as TrangThai_TheGiaTri,
    				TrangThai
    		from 
    		(
    			select 
    				dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, 
    				dt.TrangThai_TheGiaTri,
    				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
    				else '12' end as TrangThai, -- used to where TrangThai_TheGiaTri (1: all, 11: dang hoat dong, 2. Ngung hoat dong)
    				dt.DienThoai,
    				tblTemp.SoDuDauKy,
    				tblTemp.PhatSinhTang,
    				tblTemp.PhatSinhGiam
    			from DM_DoiTuong dt
    			left join 
    			( 
    				select 
    					ID_DoiTuong,
    					SUM(ISNULL(TongThuTheGiaTri,0)) as TongThuTheGiaTri,
    					SUM(ISNULL(SuDungThe,0)) as SuDungThe,
    					SUM(ISNULL(HoanTraTheGiatri,0)) as HoanTraTheGiaTri,
    					SUM(ISNULL(TongThuTheGiaTri,0))  - SUM(ISNULL(SuDungThe,0)) + SUM(ISNULL(HoanTraTheGiatri,0)) as SoDuDauKy,
    					SUM(ISNULL(PhatSinh_ThuTuThe,0)) + SUM(ISNULL(PhatSinh_HoanTraTheGiatri,0)) + SUM(ISNULL(PhatSinhTang_DieuChinhThe,0)) as PhatSinhTang,
    					SUM(ISNULL(PhatSinh_SuDungThe,0)) + SUM(ISNULL(PhatSinhGiam_DieuChinhThe,0)) as PhatSinhGiam
    
    				from (
							----- ===== Dau ky =======
    					 ---- thu the gtri trước thời gian tìm kiếm (lấy luôn cả gtrị điều chỉnh)
    							 SELECT hd.ID_DoiTuong,
    								  sum(hd.TongTienHang) as TongThuTheGiaTri,
									  null as SuDungThe,
    								  null as HoanTraTheGiatri,						 
    								  null as PhatSinh_ThuTuThe,
    								  null as PhatSinh_SuDungThe,
    								  null as PhatSinh_HoanTraTheGiatri,
    								  null as PhatSinhTang_DieuChinhThe,
    								  null as PhatSinhGiam_DieuChinhThe
    							 from BH_HoaDon hd    							 
    							 where hd.NgayLapHoaDon < @DateFrom 
    							 and hd.ChoThanhToan='0' 
								 and hd.LoaiHoaDon in (22,23)
    							 group by hd.ID_DoiTuong
    						 
    
    					 union all
    					 ---- su dung the giatri    						
    						SELECT qct.ID_DoiTuong,
								null as TongThuTheGiaTri,
    							sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0))  as SuDungThe,
								null as HoanTraTheGiatri,						
    							null as PhatSinh_ThuTuThe,
    							null as PhatSinh_SuDungThe,
    							null as PhatSinh_HoanTraTheGiatri,
    							null as PhatSinhTang_DieuChinhThe,
    							null as PhatSinhGiam_DieuChinhThe
    						from Quy_HoaDon_ChiTiet qct
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon < @DateFrom 
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 11
    						group by qct.ID_DoiTuong
    						 
    
    				 union all
    					  -- hoan tra tien vao the (tang sodu)   						
    						SELECT qct.ID_DoiTuong,
								null as TongThuTheGiaTri,
    							null as SuDungThe,
    							sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0)) as HoanTraTheGiatri,
								null as PhatSinh_ThuTuThe,
    							null as PhatSinh_SuDungThe,
    							null as PhatSinh_HoanTraTheGiatri,
    							null as PhatSinhTang_DieuChinhThe,
    							null as PhatSinhGiam_DieuChinhThe
    						from Quy_HoaDon_ChiTiet qct   								
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon < @DateFrom 
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 12
    						group by qct.ID_DoiTuong
    						
						 union all
    					  -- giam do hoantracoc			
    					 SELECT hd.ID_DoiTuong,
    							null TongThuTheGiaTri,
								sum(hd.TongTienHang) as SuDungThe,
    							null as HoanTraTheGiatri,						 
    							null as PhatSinh_ThuTuThe,
    							null as PhatSinh_SuDungThe,
    							null as PhatSinh_HoanTraTheGiatri,
    							null as PhatSinhTang_DieuChinhThe,
    							null as PhatSinhGiam_DieuChinhThe								
    					from BH_HoaDon hd    							 
    					where hd.NgayLapHoaDon < @DateFrom 
    					and hd.ChoThanhToan='0' 
						and hd.LoaiHoaDon = 32
    					group by hd.ID_DoiTuong
    
					-----=========== Trong ky ==============
    					 union all
    					   --- thu the gtri tại thời điểm hiện tại
    						SELECT hd.ID_DoiTuong,
    								  null as TongThuTheGiaTri,
									  null as SuDungThe,
    								  null as HoanTraTheGiatri,						 
    								  sum(hd.TongTienHang) as PhatSinh_ThuTuThe,
    								  null as PhatSinh_SuDungThe,
    								  null as PhatSinh_HoanTraTheGiatri,
    								  null as PhatSinhTang_DieuChinhThe,
    								  null as PhatSinhGiam_DieuChinhThe
    							 from BH_HoaDon hd    							 
    							 where hd.NgayLapHoaDon between @DateFrom  and @DateTo
    							 and hd.ChoThanhToan='0' 
								 and hd.LoaiHoaDon = 22
    							 group by hd.ID_DoiTuong
    
    				union all
    					 -- su dung the giatri tại thời điểm hiện tại
    						SELECT qct.ID_DoiTuong,
								null as TongThuTheGiaTri,
    							null  as SuDungThe,
								null as HoanTraTheGiatri,						
    							null as PhatSinh_ThuTuThe,
    							sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0)) as PhatSinh_SuDungThe,
    							null as PhatSinh_HoanTraTheGiatri,
    							null as PhatSinhTang_DieuChinhThe,
    							null as PhatSinhGiam_DieuChinhThe
    						from Quy_HoaDon_ChiTiet qct
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon between @DateFrom  and @DateTo
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 11
    						group by qct.ID_DoiTuong
    
							---- tang/giam do dieu chinh the hoac hoantra tiencoc
							 union all
							 SELECT hd.ID_DoiTuong,
    								  null as TongThuTheGiaTri,
									  null as SuDungThe,
    								  null as HoanTraTheGiatri,						 
    								  null as PhatSinh_ThuTuThe,
    								  null as PhatSinh_SuDungThe,
    								  null as PhatSinh_HoanTraTheGiatri,
    								  sum(iif(hd.LoaiHoaDon = 32,0, iif(hd.TongTienHang > 0,hd.TongTienHang,0)))  as PhatSinhTang_DieuChinhThe,
    								  sum(iif(hd.LoaiHoaDon = 32, hd.TongTienHang, iif(hd.TongTienHang < 0,-hd.TongTienHang,0)))  as PhatSinhGiam_DieuChinhThe
    							 from BH_HoaDon hd    							 
    							 where hd.NgayLapHoaDon between @DateFrom  and @DateTo
    							 and hd.ChoThanhToan='0' 
								 and hd.LoaiHoaDon in (23,32)
    							 group by hd.ID_DoiTuong   
    
    					union all
    					  -- hoan tra tien the giatri tại thời điểm hiện tại					
    						SELECT qct.ID_DoiTuong,
								null as TongThuTheGiaTri,
    							null as SuDungThe,
    							null as HoanTraTheGiatri,
								null as PhatSinh_ThuTuThe,
    							null as PhatSinh_SuDungThe,
    							sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0)) as PhatSinh_HoanTraTheGiatri,
    							null as PhatSinhTang_DieuChinhThe,
    							null as PhatSinhGiam_DieuChinhThe
    						from Quy_HoaDon_ChiTiet qct   								
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon between @DateFrom  and @DateTo
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 12
    						group by qct.ID_DoiTuong   
    					) tblDoiTuong_The group by tblDoiTuong_The.ID_DoiTuong
						
    			) tblTemp on dt.ID= tblTemp.ID_DoiTuong
    			where (dt.TheoDoi is null or dt.TheoDoi = 0) and dt.LoaiDoiTuong =1
    				and
    					 
    							((select count(Name) from @tblSearchString b where    
								dt.DienThoai like '%'+b.Name+'%'
    							or dt.MaDoiTuong like '%'+b.Name+'%'
    							or dt.TenDoiTuong like '%'+b.Name+'%'
    							or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    							or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'				
    							)=@count or @count=0)	
    
    		) tblView 
    		where tblView.TrangThai like @Status
    		and ISNULL(tblView.SoDuDauKy,0) + ISNULL(tblView.PhatSinhTang,0)- ISNULL(tblView.PhatSinhGiam,0) > 0
    	),
    	count_cte
    	as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(SoDuDauKy) as TongSoDuDauKy,
    				sum(PhatSinhTang) as TongPhatSinhTang,
    				sum(PhatSinhGiam) as TongPhatSinhGiam,
    				sum(SoDuCuoiKy) as TongSoDuCuoiKy
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaDoiTuong
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

			Sql(@"ALTER PROCEDURE [dbo].[SP_GetHoaDonAndSoQuy_FromIDDoiTuong]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;

declare @tblChiNhanh table (ID uniqueidentifier)
insert into @tblChiNhanh
select name from dbo.splitstring(@ID_DonVi)


	DECLARE @tblHoaDon TABLE(ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, LoaiHoaDon INT, GiaTri FLOAT);
	DECLARE @LoaiDoiTuong INT;
	SELECT @LoaiDoiTuong = LoaiDoiTuong FROM DM_DoiTuong WHERE ID = @ID_DoiTuong;
	IF(@LoaiDoiTuong = 3)
	BEGIN
		INSERT INTO @tblHoaDon
    	select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.LoaiHoaDon,
			hd.PhaiThanhToanBaoHiem as GiaTri
    	from BH_HoaDon hd
    	where hd.ID_BaoHiem like @ID_DoiTuong and hd.ID_DonVi in (select ID from @tblChiNhanh)
    	and hd.LoaiHoaDon not in (3,23) -- dieu chinh the: khong lien quan cong no
		and hd.ChoThanhToan ='0'
	END
	ELSE
	BEGIN
		INSERT INTO @tblHoaDon
		select *
		from
		(
		select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.LoaiHoaDon,
			case hd.LoaiHoaDon
				when 4 then ISNULL(hd.TongThanhToan,0)
				when 6 then - ISNULL(hd.TongThanhToan,0)
				when 7 then - ISNULL(hd.TongThanhToan,0)
				when 32 then - ISNULL(hd.TongThanhToan,0)
				when 14 then - ISNULL(hd.TongThanhToan,0) ---- nhaphang khachthua
			else
    			ISNULL(hd.PhaiThanhToan,0)
    		end as GiaTri
    	from BH_HoaDon hd
		join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
    	where hd.ID_DoiTuong like @ID_DoiTuong 
    	and hd.LoaiHoaDon not in (3,23,31) 
		and hd.ChoThanhToan ='0'

		union all
		---- chiphi dichvu
		select 
			cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon, 125 as LoaiHoaDon,
			sum(cp.ThanhTien) as GiaTri			
		from BH_HoaDon_ChiPhi cp
		join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
		join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
		where hd.ChoThanhToan= 0
		and cp.ID_NhaCungCap= @ID_DoiTuong
		group by cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon,hd.LoaiHoaDon
		)a
	END

	---select * from @tblHoaDon
		
		SELECT *, 0 as LoaiThanhToan
		FROM @tblHoaDon
    	union
    	-- get list Quy_HD (không lấy Quy_HoaDon thu từ datcoc)
		select * from
		(
    		select qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon ,
			case when dt.LoaiDoiTuong = 1 OR dt.LoaiDoiTuong = 3 then
				case when qhd.LoaiHoaDon= 11 then -sum(qct.TienThu) else iif (max(LoaiThanhToan)=4, -sum(qct.TienThu),sum(qct.TienThu)) end
			else 
    			case when qhd.LoaiHoaDon = 11 then sum(qct.TienThu) else -sum(qct.TienThu) end
    		end as GiaTri,
			iif(qhd.PhieuDieuChinhCongNo='1',2, max(qct.LoaiThanhToan)) as LoaiThanhToan --- 1.coc, 2.dieuchinhcongno, 3.khong butru congno			
    		from Quy_HoaDon qhd	
    		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    		join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
			left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID		
    		where qct.ID_DoiTuong like @ID_DoiTuong
			and qct.HinhThucThanhToan !=6			
			and (qct.LoaiThanhToan is null or qct.LoaiThanhToan !=3) ---- LoaiThnahToan = 3. thu/chi khong lienquan congno
			and (qhd.TrangThai is null or qhd.TrangThai='1') -- van phai lay phieu thu tu the --> trừ cong no KH
			group by qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon,dt.LoaiDoiTuong,qhd.PhieuDieuChinhCongNo
		) a where a.GiaTri != 0 -- khong lay phieudieuchinh diem

	
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_GetInforKhachHang_ByID]
    @ID_DoiTuong uniqueidentifier,
    @ID_ChiNhanh [nvarchar](max),
    @timeStart [nvarchar](max),
    @timeEnd [nvarchar](max)
AS
BEGIN
	SET NOCOUNT ON;
	declare @LoaiDoiTuong int
	select @LoaiDoiTuong= LoaiDoiTuong from DM_DoiTuong where ID = @ID_DoiTuong

    SELECT 
    			  dt.ID as ID,
    			  dt.MaDoiTuong, 
    			  case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as ID_NhomDoiTuong,
    			  dt.TenDoiTuong,
    			  dt.TenDoiTuong_KhongDau,
    			  dt.TenDoiTuong_ChuCaiDau,
    			  dt.ID_TrangThai,
    			  dt.GioiTinhNam,
    			  dt.NgaySinh_NgayTLap,
    			  ISNULL(dt.DienThoai,'') as DienThoai,
    			  ISNULL(dt.Email,'') as Email,
    			  ISNULL(dt.DiaChi,'') as DiaChi,
    			  ISNULL(dt.MaSoThue,'') as MaSoThue,
    			  ISNULL(dt.GhiChu,'') as GhiChu,
				  dt.TaiKhoanNganHang,
    			  dt.NgayTao,
    			  dt.DinhDang_NgaySinh,
    			  ISNULL(dt.NguoiTao,'') as NguoiTao,
    			  dt.ID_NguonKhach,
    			  dt.ID_NhanVienPhuTrach,
    			  dt.ID_NguoiGioiThieu,
    			  dt.ID_DonVi,
    			  dt.LaCaNhan,
    			  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
				  dt.TenNhomDoiTuongs as TenNhomDT,    			 
    			  dt.ID_TinhThanh,
    			  dt.ID_QuanHuyen,
				  dt.TheoDoi,
				  dt.NgayGiaoDichGanNhat,
    			  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    			  CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    			  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    			  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    			  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    			  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
    			  CAST(0 as float) as TongNapThe , 
    			  CAST(0 as float) as SuDungThe , 
    			  CAST(0 as float) as HoanTraTheGiaTri , 
    			  CAST(0 as float) as SoDuTheGiaTri , 
				  ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
    			  concat(dt.MaDoiTuong,' ',lower(dt.MaDoiTuong) ,' ', dt.TenDoiTuong,' ', dt.DienThoai,' ', dt.TenDoiTuong_KhongDau)  as Name_Phone			
    		  FROM DM_DoiTuong dt
			  left join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
			  LEFT JOIN (
    					SELECT tblThuChi.ID_DoiTuong,   						
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) + SUM(ISNULL(tblThuChi.HoanTraSoDuTGT,0))
						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
					sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    					FROM
    					(
						-- doanhthu
							SELECT 
    							iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) as ID_DoiTuong,
    							0 AS GiaTriTra,    							
								iif(@LoaiDoiTuong=1,bhd.PhaiThanhToan,isnull(bhd.PhaiThanhToanBaoHiem,0)) as DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = 0 
							and iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) = @ID_DoiTuong

    						
    						 union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,    							
								iif(@LoaiDoiTuong=1,bhd.PhaiThanhToan,0)  AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) 
							AND bhd.ChoThanhToan = 0 
							and iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) = @ID_DoiTuong						
    							
    						union all
    
    							-- tienthu
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon    						
    						WHERE qhd.LoaiHoaDon = 11 
							and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
							and qhdct.ID_DoiTuong = @ID_DoiTuong
							and qhdct.HinhThucThanhToan!=6

    							
    						 union all    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 12 
							AND (qhd.TrangThai != 0 OR qhd.TrangThai is null)
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
							and qhdct.HinhThucThanhToan!=6
							and qhdct.ID_DoiTuong = @ID_DoiTuong

							union all
							---- hoantra sodu TGT cho khach (giam sodu TGT)
						SELECT 
    							bhd.ID_DoiTuong,    	
								0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								-sum(bhd.PhaiThanhToan) as HoanTraSoDuTGT
    					FROM BH_HoaDon bhd
						where bhd.LoaiHoaDon = 32 and bhd.ChoThanhToan = 0 	
						group by bhd.ID_DoiTuong
    				)AS tblThuChi GROUP BY tblThuChi.ID_DoiTuong   					
    		) a on dt.ID = a.ID_DoiTuong
			where dt.ID= @ID_DoiTuong
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

            Sql(@"ALTER PROCEDURE [dbo].[UpdateGiaVon_WhenEditCTHD]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier],
    @NgayLapHDMin [datetime]
AS
BEGIN
    SET NOCOUNT ON;	
	

    		declare @tblCTHD ChiTietHoaDonEdit
    		INSERT INTO @tblCTHD
    		SELECT 
    			qd.ID_HangHoa, ct.ID_LoHang, ct.ID_DonViQuiDoi, qd.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet ct
    		INNER JOIN BH_HoaDon hd ON hd.ID = ct.ID_HoaDon			
    		INNER JOIN DonViQuiDoi qd ON qd.ID = ct.ID_DonViQuiDoi			
    		INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa    		
    		WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1
    		GROUP BY qd.ID_HangHoa,ct.ID_DonViQuiDoi,qd.TyLeChuyenDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;	
    
    		-- get cthd can update GiaVon
    		DECLARE @cthd_NeedUpGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT,TongChiPhi FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT,  TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @cthd_NeedUpGiaVon
    		select * from 
    		(
    	select distinct hdupdate.ID as IDHoaDon, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID as ID_ChiTietHoaDon, 
    		CASE WHEN hdupdate.YeuCau = '4' AND @IDChiNhanh = hdupdate.ID_CheckIn THEN hdupdate.NgaySua ELSE hdupdate.NgayLapHoaDon END AS NgayLapHoaDon, 				    			    				    							    			
    			hdctupdate.SoThuTu, hdctupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, isnull(hdupdate.TongChiPhi,0) as TongChiPhi,
				hdctupdate.TienChietKhau, hdctupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		[dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanh, hhupdate.ID, hdctupdate.ID_LoHang, 
    				CASE WHEN hdupdate.YeuCau = '4' AND @IDChiNhanh = hdupdate.ID_CheckIn THEN hdupdate.NgaySua ELSE hdupdate.NgayLapHoaDon  		    		    			    					
    		END) as TonKho, 	    	
    			hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi as GiaVon, 
    			hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi as GiaVonNhan,
    		hhupdate.ID as ID_HangHoa, hhupdate.LaHangHoa, dvqdupdate.ID as IDDonViQuiDoi, hdctupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, 
    			@IDChiNhanh as IDChiNhanh, hdupdate.ID_CheckIn, hdupdate.YeuCau 
    		FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate ON hdupdate.ID = hdctupdate.ID_HoaDon    	
    	INNER JOIN DonViQuiDoi dvqdupdate ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID   	
    	INNER JOIN DM_HangHoa hhupdate on hhupdate.ID = dvqdupdate.ID_HangHoa   	
    	INNER JOIN @tblCTHD cthdthemmoiupdate  ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID   
    	WHERE hdupdate.ChoThanhToan = 0 
    			AND hdupdate.LoaiHoaDon NOT IN (3, 19, 25)
    			AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) 
    			AND
    		((hdupdate.ID_DonVi = @IDChiNhanh and hdupdate.NgayLapHoaDon >= @NgayLapHDMin
    				and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = @IDChiNhanh and hdupdate.NgaySua >= @NgayLapHDMin))
    		) as table1
    	order by NgayLapHoaDon, SoThuTu desc, LoaiHoaDon, MaHoaDon;
    
    		--Begin TinhGiaVonTrungBinh
    		DECLARE @TinhGiaVonTrungBinh BIT;
    		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanh;
    		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
    		BEGIN
    			-- get GiaVon DauKy by ID_QuiDoi
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT, TongChiPhi float,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select
    				hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu, hdct.SoLuong, hdct.DonGia, 
					hd.TongTienHang, isnull(hd.TongChiPhi,0) as TongChiPhi,
    				hdct.TienChietKhau, hdct.ThanhTien, hd.TongGiamGia, 
    				dvqd.TyLeChuyenDoi, 0, hdct.GiaVon / dvqd.TyLeChuyenDoi, -- giavon
    			hdct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi, --giavonnhan
    			hh.ID, hh.LaHangHoa, hdct.ID_DonViQuiDoi, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, 
    				@IDChiNhanh, hd.ID_CheckIn, hd.YeuCau 
			FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct 	ON hd.ID = hdct.ID_HoaDon    	
    		INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID    		
    		INNER JOIN DM_HangHoa hh on hh.ID = dvqd.ID_HangHoa    		
    		INNER JOIN @tblCTHD cthdthemmoi ON cthdthemmoi.ID_HangHoa = hh.ID    		
    		WHERE hd.ChoThanhToan = 0 AND hd.LoaiHoaDon NOT IN (3, 19, 25)
    				AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) 
    				AND
    				((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon < @NgayLapHDMin and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    					or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua < @NgayLapHDMin))
    		order by NgayLapHoaDon desc, SoThuTu desc, hd.LoaiHoaDon, hd.MaHoaDon;
    			
    			--select * from @ChiTietHoaDonGiaVon order by NgayLapHoaDon
    			--select * from @cthd_NeedUpGiaVon order by NgayLapHoaDon
    
		DECLARE @ChiTietHoaDonGiaVon1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT, TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
			
			INSERT INTO @ChiTietHoaDonGiaVon1
			SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    					FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon1 WHERE cthdGiaVon1.RN = 1;

    			-- assign again GiaVon to cthd was edit by ID_HangHoa
    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
				TongTienHang FLOAT,TongChiPhi FLOAT,
    			ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    			ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @BangUpdateGiaVon
    		SELECT *, 
    				ROW_NUMBER() OVER (PARTITION BY tableUpdateGiaVon.ID_HangHoa, tableUpdateGiaVon.ID_LoHang ORDER BY tableUpdateGiaVon.NgayLapHoaDon) AS RN 
    			FROM
    			(SELECT * 
    					FROM @cthd_NeedUpGiaVon
    			UNION ALL
    			SELECT 
    					cthdGiaVon.IDHoaDon, cthdGiaVon.IDHoaDonBan, cthdGiaVon.MaHoaDon, cthdGiaVon.LoaiHoaDon, cthdGiaVon.ID_ChiTietHoaDon, cthdGiaVon.NgayLapHoaDon,
    					cthdGiaVon.SoThuThu, cthdGiaVon.SoLuong, cthdGiaVon.DonGia, cthdGiaVon.TongTienHang,cthdGiaVon.TongChiPhi,
    				cthdGiaVon.ChietKhau, cthdGiaVon.ThanhTien, cthdGiaVon.TongGiamGia, cthdGiaVon.TyLeChuyenDoi, cthdGiaVon.TonKho, 
    					CASE WHEN cthdGiaVon.GiaVon IS NULL THEN 0 ELSE cthdGiaVon.GiaVon END AS GiaVon, 											
    					CASE WHEN cthdGiaVon.GiaVonNhan IS NULL THEN 0 ELSE cthdGiaVon.GiaVonNhan END AS GiaVonNhan,								
    					cthd1.ID_HangHoa, cthdGiaVon.LaHangHoa, cthdGiaVon.IDDonViQuiDoi, cthd1.ID_LoHang , cthdGiaVon.ID_ChiTietDinhLuong,
    				@IDChiNhanh, cthdGiaVon.ID_CheckIn, cthdGiaVon.YeuCau 
    				FROM @tblCTHD cthd1
    				LEFT JOIN 
    					@ChiTietHoaDonGiaVon1 AS cthdGiaVon
    				ON cthd1.ID_HangHoa = cthdGiaVon.ID_HangHoa 
    				AND (cthd1.ID_LoHang = cthdGiaVon.ID_LoHang OR cthdGiaVon.ID_LoHang IS NULL)) AS tableUpdateGiaVon;
    		--select * from @BangUpdateGiaVon order by NgayLapHoaDon
    
    			-- caculator again GiaVon by ID_HangHoa
    			DECLARE @TableTrungGianUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER,IDHangHoa UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, GiaVonNhapHang FLOAT, GiaVonNhanHang FLOAT)
    			INSERT INTO @TableTrungGianUpDate
    			SELECT 
    				IDHoaDon, ID_HangHoa, ID_LoHang, 
					sum(GiaVon) as GiaVonNhapHang,
					sum(GiaVonNhan) as GiaVonNhanHang
    				--CASE WHEN MAX(TongTienHang) != 0 THEN SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) * (1-(MAX(TongGiamGia) / MAX(TongTienHang))) 
    				--	ELSE SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) END as GiaVonNhapHang,
    				--CASE WHEN LoaiHoaDon = 10 THEN SUM(ChietKhau * DonGia)/ SUM(IIF(ChietKhau = 0, 1, ChietKhau) * TyLeChuyenDoi) 
    				--	ELSE 0 END as GiaVonNhanHang
    			FROM @BangUpdateGiaVon
    			WHERE IDHoaDon = @IDHoaDonInput
    				AND ID_HangHoa in (SELECT ID_HangHoa FROM @BangUpdateGiaVon WHERE IDHoaDon = @IDHoaDonInput AND RN = 1)
    			GROUP BY ID_HangHoa, ID_LoHang, IDHoaDon,LoaiHoaDon
    			
    			--select * from @TableTrungGianUpDate 
    
    			DECLARE @RNCheck INT;
    			SELECT @RNCheck = MAX(RN) FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang
    			IF(@RNCheck = 1)
    			BEGIN
    				UPDATE @BangUpdateGiaVon SET RN = 2
    			END
    
    			-- update GiaVon, GiaVonNhan to @BangUpdateGiaVon if Loai =(4,10), else keep old
    		UPDATE bhctup 
    			SET bhctup.GiaVon = 
    			CASE WHEN bhctup.LoaiHoaDon in (4,13) THEN giavontbup.GiaVonNhapHang	    						
    			ELSE bhctup.GiaVon END,    		
    			bhctup.GiaVonNhan = 
    				CASE WHEN bhctup.LoaiHoaDon = 10 AND bhctup.YeuCau = '4' AND bhctup.ID_CheckIn = ID_ChiNhanhThemMoi THEN giavontbup.GiaVonNhanHang   		    			
    			ELSE bhctup.GiaVonNhan END  		
    			FROM @BangUpdateGiaVon bhctup
    			JOIN @TableTrungGianUpDate giavontbup on bhctup.IDHoaDon =giavontbup.IDHoaDon and bhctup.ID_HangHoa = giavontbup.IDHangHoa and (bhctup.ID_LoHang = giavontbup.IDLoHang or giavontbup.IDLoHang is null)
    		WHERE bhctup.IDHoaDon = @IDHoaDonInput AND bhctup.RN = 1;
    			--END tính giá vốn trung bình cho lần nhập hàng và chuyển hàng đầu tiền
    
    		DECLARE @GiaVonCapNhat TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, IDHoaDonCu UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, IDChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT,TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, GiaVonCu FLOAT, IDHangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, IDChiTietDinhLuong UNIQUEIDENTIFIER,
    		IDChiNhanhThemMoi UNIQUEIDENTIFIER, IDCheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @GiaVonCapNhat
    		SELECT 
    				tableUpdate.IDHoaDon, tableUpdate.IDHoaDonBan, tableGiaVon.IDHoaDon, tableUpdate.MaHoaDon, tableUpdate.LoaiHoaDon, tableUpdate.ID_ChiTietHoaDon,tableUpdate.NgayLapHoaDon, tableUpdate.SoLuong, tableUpdate.DonGia,
    			tableUpdate.TongTienHang, tableUpdate.TongChiPhi,
				tableUpdate.ChietKhau, tableUpdate.ThanhTien, tableUpdate.TongGiamGia, tableUpdate.TyLeChuyenDoi, tableUpdate.TonKho, tableUpdate.GiaVon, tableUpdate.GiaVonNhan, tableGiaVon.GiaVon, tableUpdate.ID_HangHoa, tableUpdate.LaHangHoa,
    			tableUpdate.IDDonViQuiDoi, tableUpdate.ID_LoHang, tableUpdate.ID_ChiTietDinhLuong, tableUpdate.ID_ChiNhanhThemMoi, tableUpdate.ID_CheckIn, tableUpdate.YeuCau, tableUpdate.RN 
    			FROM @BangUpdateGiaVon tableUpdate
    		LEFT JOIN (SELECT (CASE WHEN ID_CheckIn = ID_ChiNhanhThemMoi THEN GiaVonNhan ELSE GiaVon END) AS GiaVon, ID_HangHoa, IDHoaDon, ID_LoHang, RN + 1 AS RN 
    						FROM @BangUpdateGiaVon) AS tableGiaVon
    		ON tableUpdate.ID_HangHoa = tableGiaVon.ID_HangHoa AND tableUpdate.RN = tableGiaVon.RN 
    			AND (tableUpdate.ID_LoHang = tableGiaVon.ID_LoHang OR tableUpdate.ID_LoHang IS NULL);
    
    			--select * from @GiaVonCapNhat
    			
    		DECLARE @IDHoaDon UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonBan UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonCu UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX);
    		DECLARE @LoaiHoaDon INT;
    		DECLARE @IDChiTietHoaDon UNIQUEIDENTIFIER;
    		DECLARE @SoLuong FLOAT;
    		DECLARE @DonGia FLOAT;
    		DECLARE @TongTienHang FLOAT, @TongChiPhi float;
    		DECLARE @ChietKhau FLOAT;
    		DECLARE @ThanhTien FLOAT;
    		DECLARE @TongGiamGia FLOAT;
    		DECLARE @TyLeChuyenDoi FLOAT;
    		DECLARE @TonKho FLOAT;
    		DECLARE @GiaVonCu FLOAT;
    		DECLARE @IDHangHoa UNIQUEIDENTIFIER;
    		DECLARE @IDDonViQuiDoi UNIQUEIDENTIFIER;
    		DECLARE @IDLoHang UNIQUEIDENTIFIER;
    		DECLARE @IDChiNhanhThemMoi UNIQUEIDENTIFIER;
    		DECLARE @IDCheckIn UNIQUEIDENTIFIER;
    		DECLARE @YeuCau NVARCHAR(MAX);
    		DECLARE @RN INT;
    			DECLARE @GiaVon FLOAT;
    			DECLARE @GiaVonNhan FLOAT;
    		DECLARE @GiaVonMoi FLOAT;
    		DECLARE @GiaVonCuUpdate FLOAT;
    		DECLARE @IDHangHoaUpdate UNIQUEIDENTIFIER;
    		DECLARE @IDLoHangUpdate UNIQUEIDENTIFIER;
    			DECLARE @GiaVonHoaDonBan FLOAT;
    		DECLARE @TongTienHangDemo FLOAT;
    		DECLARE @SoLuongDemo FLOAT;
    			DECLARE @ThanhTienDemo FLOAT;
    			DECLARE @ChietKhauDemo FLOAT;
    
    		DECLARE CS_GiaVon CURSOR SCROLL LOCAL FOR 
    			SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, TongChiPhi, ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
    			GiaVonCu, IDHangHoa, IDDonViQuiDoi, IDLoHang, IDChiNhanhThemMoi, IDCheckIn, YeuCau, RN, GiaVon, GiaVonNhan 
    			FROM @GiaVonCapNhat WHERE RN > 1 and LaHangHoa = 1 ORDER BY IDHangHoa, RN
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon 
    			INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau,@ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    			iF(@IDHangHoaUpdate = @IDHangHoa AND (@IDLoHangUpdate = @IDLoHang OR @IDLoHang IS NULL))
    			BEGIN
    				SET @GiaVonCu = @GiaVonCuUpdate;
    			END
    			ELSE
    			BEGIN
    				SET @IDHangHoaUpdate = @IDHangHoa;
    				SET @IDLoHangUpdate = @IDLoHang;
    					SET @GiaVonCuUpdate = @GiaVonCu;
    			END
    				IF(@GiaVonCu IS NOT NULL)
    				BEGIN
    				IF(@LoaiHoaDon in (4,13))
    				BEGIN
    					
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia -  bhctdm.ChietKhau)),  ---- cot thanhtien bhct
								@SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    
    						--select @IDHoaDon, @MaHoaDon, @TongTienHangDemo, @SoLuongDemo, @TonKho
    					IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    					BEGIN
    						IF(@TongTienHang != 0)
    						BEGIN
								---- giavon: tinh sau khi tru giam gia hoadon + phi ship
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo ----  (@TongTienHangDemo* (1-(@TongGiamGia/@TongTienHang))) GiamGiaHoaDon - khong tinh vao GiaVon
									+ (@TongTienHangDemo* @TongChiPhi/@TongTienHang))/(@SoLuongDemo + @TonKho);
								
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo)/(@SoLuongDemo + @TonKho);
    						END
    					END
    					ELSE                                                                                                                                                                                                                                                                                                                                                                                                                          
    					BEGIN
    					
    						IF(@TongTienHang != 0)
    						BEGIN
    							SET	@GiaVonMoi = (@TongTienHangDemo / @SoLuongDemo)  ---- * (1 - (@TongGiamGia / @TongTienHang))
								+ (@TongTienHangDemo *(@TongChiPhi / @TongTienHang));
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
    						END
    					END
    
    						--select @GiaVonMoi
    				END
    				ELSE IF (@LoaiHoaDon = 7)
    				BEGIN
    					--select @IDHoaDon, @MaHoaDon, @TongTienHangDemo, @SoLuongDemo, @TonKho
    						
    					SELECT @TongTienHangDemo = 
							SUM(bhctdm.SoLuong * bhctdm.DonGia * ( 1- bhctdm.TongGiamGia/iif(bhctdm.TongTienHang=0,1,bhctdm.TongTienHang))) ,
						@SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@TonKho - @SoLuongDemo > 0)
    					BEGIN
    						SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) - @TongTienHangDemo)/(@TonKho - @SoLuongDemo);
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    						--select @GiaVonMoi
    				END
    				ELSE IF(@LoaiHoaDon = 10)
    				BEGIN
    					SELECT @TongTienHangDemo = SUM(bhctdm.ChietKhau * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.ChietKhau * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    
    					IF(@YeuCau = '1' OR (@YeuCau = '4' AND @IDChiNhanhThemMoi != @IDCheckIn))
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    					ELSE IF (@YeuCau = '4' AND @IDChiNhanhThemMoi = @IDCheckIn)
    					BEGIN
    						IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    						BEGIN
    							SET @GiaVonMoi = (@GiaVonCu * @TonKho + @TongTienHangDemo)/(@TonKho + @SoLuongDemo);
    						END
    						ELSE
    						BEGIN
    								IF(@SoLuongDemo = 0)
    								BEGIN
    									SET @GiaVonMoi = @GiaVonCu;
    								END
    								ELSE
    								BEGIN
    								SET @GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
    								END
    						END
    					END
    				END
    				ELSE IF (@LoaiHoaDon = 6)
    				BEGIN
    					SELECT @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@IDHoaDonBan IS NOT NULL)
    					BEGIN
    						SET @GiaVonHoaDonBan = -1;
    						SELECT @GiaVonHoaDonBan = GiaVon FROM @GiaVonCapNhat WHERE IDHoaDon = @IDHoaDonBan AND IDDonViQuiDoi = @IDDonViQuiDoi AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL);
    						
    						IF(@GiaVonHoaDonBan = -1)
    						BEGIN
    							
    							SELECT @GiaVonHoaDonBan = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDonBan AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
    						
    						END
    						IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    						BEGIN
    							
    							SET @GiaVonMoi = (@GiaVonCu * @TonKho + @GiaVonHoaDonBan * @SoLuongDemo) / (@TonKho + @SoLuongDemo);
    						END
    						ELSE
    						BEGIN
    							SET @GiaVonMoi = @GiaVonHoaDonBan;
    						END
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    				END
    				ELSE IF(@LoaiHoaDon = 18) ----phieu dieuchinh giavon
    					BEGIN
    						SELECT @GiaVonMoi = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDon AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
    					END
    					ELSE
    				BEGIN
    					SET @GiaVonMoi = @GiaVonCu;
    				END
    				END
    				ELSE
    				BEGIN
    					IF(@IDCheckIn = @IDChiNhanhThemMoi)
    					BEGIN
    						SET @GiaVonMoi = @GiaVonNhan
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVon
    					END
    				END
    
    			IF(@IDHoaDon = @IDHoaDonCu)
    			BEGIN
    				SET @GiaVonMoi = @GiaVonCuUpdate;	
    			END
    			ELSE
    			BEGIN
    				SET @GiaVonCuUpdate = @GiaVonMoi;
    			END
    			IF(@LoaiHoaDon = 10 AND @YeuCau = '4' AND @IDCheckIn = @IDChiNhanhThemMoi)
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVonNhan = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			ELSE
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVon = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon		
			
			update gv set GiaVonCu = isnull(GiaVonCu,0)
			from @GiaVonCapNhat gv 
    
    		--	select * from @GiaVonCapNhat
    		--Update BH_HoaDon_ChiTiet
    		UPDATE hoadonchitiet1
    		SET hoadonchitiet1.GiaVon = _giavoncapnhat1.GiaVon * _giavoncapnhat1.TyLeChuyenDoi,
    			hoadonchitiet1.GiaVon_NhanChuyenHang = _giavoncapnhat1.GiaVonNhan * _giavoncapnhat1.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet1
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat1 ON hoadonchitiet1.ID = _giavoncapnhat1.IDChiTietHoaDon   		
    		WHERE _giavoncapnhat1.LoaiHoaDon != 8 AND _giavoncapnhat1.LoaiHoaDon != 18 AND _giavoncapnhat1.LoaiHoaDon != 9 AND _giavoncapnhat1.RN > 1;
    
    		---- update GiaVon to phieu KiemKe
    			UPDATE hoadonchitiet4
    		SET hoadonchitiet4.GiaVon = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi, 
    			hoadonchitiet4.ThanhToan = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi *hoadonchitiet4.SoLuong
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet4
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat4 ON hoadonchitiet4.ID = _giavoncapnhat4.IDChiTietHoaDon    		
    		WHERE _giavoncapnhat4.LoaiHoaDon = 9 AND _giavoncapnhat4.RN > 1;
    
    			-- update GiaVon to phieu XuatKho
    		UPDATE hoadonchitiet2
    		SET hoadonchitiet2.GiaVon = _giavoncapnhat2.GiaVon * _giavoncapnhat2.TyLeChuyenDoi, 
				--hoadonchitiet2.DonGia = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong* _giavoncapnhat2.TyLeChuyenDoi,
    			hoadonchitiet2.ThanhTien = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong* _giavoncapnhat2.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet2
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat2 ON hoadonchitiet2.ID = _giavoncapnhat2.IDChiTietHoaDon    		
    		WHERE _giavoncapnhat2.LoaiHoaDon = 8 AND _giavoncapnhat2.RN > 1;
    
    			-- update GiaVon to Loai = 18 (Phieu DieuChinh GiaVon)
    		UPDATE hoadonchitiet3
    		SET hoadonchitiet3.DonGia = _giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi, 
    				hoadonchitiet3.PTChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) ELSE 0 END,
    			hoadonchitiet3.TienChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN 0 ELSE hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) END
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet3
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat3
    		ON hoadonchitiet3.ID = _giavoncapnhat3.IDChiTietHoaDon
    		WHERE _giavoncapnhat3.LoaiHoaDon = 18 AND _giavoncapnhat3.RN > 1;
    
    		UPDATE chitietdinhluong
    		SET chitietdinhluong.GiaVon = gvDinhLuong.GiaVonDinhLuong / iif(chitietdinhluong.SoLuong=0,1,chitietdinhluong.SoLuong)
    		FROM BH_HoaDon_ChiTiet AS chitietdinhluong
    		INNER JOIN
    			(SELECT 
    					SUM(ct.GiaVon * ct.SoLuong) AS GiaVonDinhLuong, ct.ID_ChiTietDinhLuong 
    				FROM BH_HoaDon_ChiTiet ct
    			INNER JOIN (SELECT IDChiTietDinhLuong FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDChiTietDinhLuong) gv
    			ON (ct.ID_ChiTietDinhLuong = gv.IDChiTietDinhLuong and ct.ID_ChiTietDinhLuong IS NOT NULL)
    			WHERE gv.IDChiTietDinhLuong IS NOT NULL AND ct.ID != ct.ID_ChiTietDinhLuong
    			GROUP BY ct.ID_ChiTietDinhLuong) AS gvDinhLuong
    		ON chitietdinhluong.ID = gvDinhLuong.ID_ChiTietDinhLuong
    		--END Update BH_HoaDon_ChiTiet
    		--Update DM_GiaVon
    		UPDATE _dmGiaVon1
    		SET _dmGiaVon1.GiaVon = ISNULL(_gvUpdateDM1.GiaVon, 0)
    		FROM 
    				(SELECT dvqd1.ID AS IDDonViQuiDoi, _giavon1.IDLoHang AS IDLoHang, (CASE WHEN _giavon1.IDCheckIn = _giavon1.IDChiNhanhThemMoi THEN _giavon1.GiaVonNhan ELSE _giavon1.GiaVon END) * dvqd1.TyLeChuyenDoi AS GiaVon, _giavon1.IDChiNhanhThemMoi AS IDChiNhanh 
    				FROM @GiaVonCapNhat _giavon1
    			INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN > 1 GROUP BY IDHangHoa,IDLoHang) AS _maxGiaVon1
    			ON _giavon1.IDHangHoa = _maxGiaVon1.IDHangHoa AND _giavon1.RN = _maxGiaVon1.RN AND (_giavon1.IDLoHang = _maxGiaVon1.IDLoHang OR _maxGiaVon1.IDLoHang IS NULL)
    			INNER JOIN DonViQuiDoi dvqd1
    			ON dvqd1.ID_HangHoa = _giavon1.IDHangHoa) AS _gvUpdateDM1
    		LEFT JOIN DM_GiaVon _dmGiaVon1
    		ON _gvUpdateDM1.IDChiNhanh = _dmGiaVon1.ID_DonVi AND _gvUpdateDM1.IDDonViQuiDoi = _dmGiaVon1.ID_DonViQuiDoi AND (_gvUpdateDM1.IDLoHang = _dmGiaVon1.ID_LoHang OR _gvUpdateDM1.IDLoHang IS NULL)
    		WHERE _dmGiaVon1.ID IS NOT NULL;
    
    		INSERT INTO DM_GiaVon (ID, ID_DonVi, ID_DonViQuiDoi, ID_LoHang, GiaVon)
    		SELECT NEWID(), _gvUpdateDM.IDChiNhanh, _gvUpdateDM.IDDonViQuiDoi, _gvUpdateDM.IDLoHang, _gvUpdateDM.GiaVon 
    			FROM 
    			(SELECT dvqd2.ID AS IDDonViQuiDoi, _giavon2.IDLoHang AS IDLoHang, 
    					(CASE WHEN _giavon2.IDCheckIn = _giavon2.IDChiNhanhThemMoi THEN _giavon2.GiaVonNhan ELSE _giavon2.GiaVon END) * dvqd2.TyLeChuyenDoi AS GiaVon, 
    					_giavon2.IDChiNhanhThemMoi AS IDChiNhanh 
    				FROM @GiaVonCapNhat _giavon2
    			INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDHangHoa, IDLoHang) AS _maxGiaVon
    		ON _giavon2.IDHangHoa = _maxGiaVon.IDHangHoa AND _giavon2.RN = _maxGiaVon.RN AND (_giavon2.IDLoHang = _maxGiaVon.IDLoHang OR _maxGiaVon.IDLoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd2 ON dvqd2.ID_HangHoa = _giavon2.IDHangHoa) AS _gvUpdateDM    		
    		LEFT JOIN DM_GiaVon _dmGiaVon
    		ON _gvUpdateDM.IDChiNhanh = _dmGiaVon.ID_DonVi AND _gvUpdateDM.IDDonViQuiDoi = _dmGiaVon.ID_DonViQuiDoi AND (_gvUpdateDM.IDLoHang = _dmGiaVon.ID_LoHang OR _gvUpdateDM.IDLoHang IS NULL)
    		WHERE _dmGiaVon.ID IS NULL;
    		--End Update DM_GiaVon
    		END
    		--END TinhGiaVonTrungBinh
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateTonLuyKeCTHD_whenUpdate]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanhInput [uniqueidentifier],
    @NgayLapHDOld [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    
    		DECLARE @NgayLapHDNew DATETIME;   
    		DECLARE @NgayNhanHang DATETIME;
    		declare @tblHoaDonChiTiet ChiTietHoaDonEdit -- table user defined
    		DECLARE @IDCheckIn  UNIQUEIDENTIFIER, @YeuCau NVARCHAR(MAX),  @LoaiHoaDon INT, @NgayLapHDMin DATETIME;
    		DECLARE @tblChiTiet TABLE (ID_HangHoa UNIQUEIDENTIFIER not null, ID_LoHang UNIQUEIDENTIFIER null, ID_DonViQuiDoi UNIQUEIDENTIFIER not null, TyLeChuyenDoi float not null)
    		DECLARE @LuyKeDauKy TABLE(ID_LoHang UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TonLuyKe FLOAT);
    		DECLARE @hdctUpdate TABLE(ID UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, TonLuyKe FLOAT, LoaiHoaDon INT, 
    		MaHoaDon nvarchar(max), NgayLapHoaDon datetime, YeuCau nvarchar(max));
    
    		--  get NgayLapHD by IDHoaDon: if update HDNhanHang (loai 10, yeucau = 4 --> get NgaySua
    		select 
    			@NgayLapHDNew = NgayLapHoaDon,
    			@NgayNhanHang = NgaySua,
    			@LoaiHoaDon = LoaiHoaDon, @YeuCau = YeuCau, @IDCheckIn = ID_CheckIn
				--@IDChiNhanhInput = ID_DonVi
    		from (
    					select LoaiHoaDon, YeuCau, ID_CheckIn, ID_DonVi, NgaySua, 
						case when @IDChiNhanhInput = ID_CheckIn and YeuCau !='1' then NgaySua else NgayLapHoaDon end as NgayLapHoaDon
    					from BH_HoaDon where ID = @IDHoaDonInput) a
    
    		-- alway get Ngay min --> compare to update TonLuyKe
    		IF(@NgayLapHDOld > @NgayLapHDNew)
    			SET @NgayLapHDMin = @NgayLapHDNew;
    		ELSE
    			SET @NgayLapHDMin = @NgayLapHDOld;
    
    		-- get cthd update by IDHoaDon
    		INSERT INTO @tblChiTiet
    		SELECT 
    			qd.ID_HangHoa, ct.ID_LoHang, ct.ID_DonViQuiDoi, qd.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet ct
    		INNER JOIN BH_HoaDon hd ON hd.ID = ct.ID_HoaDon			
    		INNER JOIN DonViQuiDoi qd ON qd.ID = ct.ID_DonViQuiDoi			
    		INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa    		
    		WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1 
    		GROUP BY qd.ID_HangHoa,ct.ID_DonViQuiDoi,qd.TyLeChuyenDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;	

    		insert into @tblHoaDonChiTiet select * from @tblChiTiet			
    				
    		-- get cthd has KiemKe group by ID_HangHoa, ID_LoHang
    		declare @tblHangKiemKe table (NgayKiemKe datetime, ID_HangHoa uniqueidentifier null, ID_LoHang uniqueidentifier null)
    		insert into @tblHangKiemKe
    		select distinct NgayLapHoaDon, qd.ID_HangHoa, ct.ID_LoHang
    			from BH_HoaDon_ChiTiet ct 
    			join BH_HoaDon hd ON hd.ID = ct.ID_HoaDon		
    			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    			join @tblChiTiet tblct ON qd.ID_HangHoa = tblct.ID_HangHoa AND (ct.ID_LoHang = tblct.ID_LoHang OR ct.ID_LoHang IS NULL)	
    			WHERE hd.ChoThanhToan = 0
    			and hd.LoaiHoaDon= 9
    			and hd.ID_DonVi = @IDChiNhanhInput and NgayLapHoaDon >= @NgayLapHDMin
    			group by qd.ID_HangHoa, ct.ID_LoHang, hd.NgayLapHoaDon				
    		
    		-- get cthd liên quan
    		select
    			ct.ID, 
    			ct.ID_LoHang,
				-- chatlieu = 5 (cthd bi xoa khi updateHD), chatlieu =2 (tra gdv  - khong cong lai tonkho)
    			case when ct.ChatLieu= '5' or ct.ChatLieu ='2' then 0 else SoLuong end as SoLuong, 
    			case when ct.ChatLieu= '5' then 0 else TienChietKhau end as TienChietKhau,
    			case when ct.ChatLieu= '5' then 0 else ct.ThanhTien end as ThanhTien,-- kiemke bi huy
    			case when hd.LoaiHoaDon= 10 and  hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput then ct.TonLuyKe_NhanChuyenHang else ct.TonLuyKe end as TonDauKy,
    			qd.ID_HangHoa,
    			qd.TyLeChuyenDoi,
    			hd.MaHoaDon,
    			hd.LoaiHoaDon,
    			hd.ID_DonVi,
    			hd.ID_CheckIn,
    			hd.YeuCau,								
    			case when hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon
    		into #temp
    		from BH_HoaDon_ChiTiet ct
    		join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    		join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    		join @tblChiTiet ctupdate on qd.ID_HangHoa = ctupdate.ID_HangHoa AND (ct.ID_LoHang = ctupdate.ID_LoHang OR ct.ID_LoHang IS NULL)	
    		WHERE (hd.ChoThanhToan = 0 or (hd.LoaiHoaDon= 3 and hd.ChoThanhToan= '1'))	-- used to tao BG (chuaduyet), sau do clcik Duyet
    		AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
    
    		-- table cthd has ID_HangHoa exist cthd kiemke
    		declare @cthdHasKiemKe table (ID_HangHoa uniqueidentifier, ID_LoHang uniqueidentifier)
    		declare @tblNgayKiemKe table (NgayKiemKe datetime)
    
    		declare @count float= (select count(*) from @tblHangKiemKe)
    		--if @count > 0
    			begin						
    				declare @ID_HangHoa uniqueidentifier, @ID_LoHang uniqueidentifier				
    				DECLARE Cur_tblKiemKe CURSOR SCROLL LOCAL FOR
    				select ID_HangHoa, ID_LoHang
    				from @tblHangKiemKe
    				order by NgayKiemKe
    
    				OPEN Cur_tblKiemKe -- cur 1
    			FETCH FIRST FROM Cur_tblKiemKe
    				INTO @ID_HangHoa, @ID_LoHang
    				WHILE @@FETCH_STATUS = 0
    				BEGIN	
    						if not exists (select * from @cthdHasKiemKe kk where kk.ID_HangHoa= @ID_HangHoa and (kk.ID_LoHang= @ID_LoHang OR kk.ID_LoHang is null))
    							begin
    								-- get list NgayKiemKe by ID_HangHoa & ID_LoHang								
    								declare @NgayKiemKe datetime
    								declare @FromDate datetime = @NgayLapHDMin
    
    								-- get cac khoang thoigian kiemke
    								insert into @tblNgayKiemKe
    								select *
    								from
    									( select NgayKiemKe 
    									from @tblHangKiemKe kk where kk.ID_HangHoa = @ID_HangHoa and (kk.ID_LoHang= @ID_LoHang or kk.ID_LoHang is null)						
    									union 
    										select GETDATE() as NgayKiemKe
    									) b order by NgayKiemKe
    
    								DECLARE Cur_NgayKiemKe CURSOR SCROLL LOCAL FOR								
    								select NgayKiemKe from @tblNgayKiemKe
    
    								OPEN Cur_NgayKiemKe -- cur 2
    							FETCH FIRST FROM Cur_NgayKiemKe
    								INTO @NgayKiemKe
    								WHILE @@FETCH_STATUS = 0
    									begin											
    										insert into @cthdHasKiemKe values(@ID_HangHoa, @ID_LoHang)
    										-- get tondauky 
    										if @FromDate = @NgayLapHDMin and @LoaiHoaDon !=9		
    											begin
    												insert into @LuyKeDauKy
    												select 
    													ID_LoHang,ID_HangHoa,TonDauKy																		
    												from
    													(
    													select 
    														ID_LoHang,ID_HangHoa,TonDauKy,										
    														ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    													from #temp
    													where NgayLapHoaDon < @FromDate		
    													and #temp.ID_HangHoa = @ID_HangHoa AND (#temp.ID_LoHang = @ID_LoHang OR #temp.ID_LoHang IS NULL)										
    													) luyke	
    												where luyke.RN= 1									
    											end
    										else
    											begin
    												insert into @LuyKeDauKy
    												select 
    													ID_LoHang,ID_HangHoa,TonDauKy
    												from
    													(
    													select 
    														ID_LoHang,ID_HangHoa,TonDauKy,
    														ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    													from #temp
    													where NgayLapHoaDon <=  @FromDate 
    													and #temp.ID_HangHoa = @ID_HangHoa AND (#temp.ID_LoHang = @ID_LoHang OR #temp.ID_LoHang IS NULL)		
    													) luyke	
    												where luyke.RN= 1
    											end
    		
    										--- tinh lai tonluyke
    										INSERT INTO @hdctUpdate
    										select ID, ID_DonVi, ID_CheckIn,
    												ISNULL(lkdk.TonLuyKe, 0) + 
    												(SUM(IIF(LoaiHoaDon IN (1, 5, 7, 8, 2), -1 * a.SoLuong* a.TyLeChuyenDoi, 
    											IIF(LoaiHoaDon IN (4, 6, 18,13,14), SoLuong * TyLeChuyenDoi, 				
    												IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    											IIF(a.LoaiHoaDon = 10 AND a.YeuCau = '4' AND a.ID_CheckIn = @IDChiNhanhInput, a.TienChietKhau* a.TyLeChuyenDoi, 0))))) 
    												OVER(PARTITION BY a.ID_HangHoa, a.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe,
    												LoaiHoaDon, MaHoaDon,NgayLapHoaDon, YeuCau
    										from
    											(							
    											select distinct
    												ID,
    												ID_LoHang,
    												SoLuong,
    												TienChietKhau,
    												ThanhTien,
    												ID_HangHoa,
    												TyLeChuyenDoi,
    												MaHoaDon,
    												LoaiHoaDon,
    												NgayLapHoaDon,
    												ID_DonVi,
    												ID_CheckIn,
    												YeuCau
    											from #temp
    											where NgayLapHoaDon >= @FromDate
    												and NgayLapHoaDon < @NgayKiemKe
    												and #temp.ID_HangHoa = @ID_HangHoa AND (#temp.ID_LoHang = @ID_LoHang or #temp.ID_LoHang IS NULL	)						
    											) a
    										LEFT JOIN @LuyKeDauKy lkdk ON lkdk.ID_HangHoa = a.ID_HangHoa AND (lkdk.ID_LoHang = a.ID_LoHang OR a.ID_LoHang IS NULL)	
    						
    										-- xóa TonLuyKe trước đó để lấy TonLuyKe mới theo khoảng thời gian		
    										set @FromDate = @NgayKiemKe
    										--select *, 1 as after1 from @LuyKeDauKy
    										delete from @LuyKeDauKy															
    										FETCH NEXT FROM Cur_NgayKiemKe INTO @NgayKiemKe
    									end
    								CLOSE Cur_NgayKiemKe  
    								DEALLOCATE Cur_NgayKiemKe 
    							end		
    
    						-- delete & assign again in for loop
    						delete from @tblNgayKiemKe
    						FETCH NEXT FROM Cur_tblKiemKe INTO @ID_HangHoa,@ID_LoHang
    					END
    				CLOSE Cur_tblKiemKe  
    				DEALLOCATE Cur_tblKiemKe 				
    			end
    
    			-- get luyke dauky of HangHoa not exist in ctkiemke
    			begin
    				insert into @LuyKeDauKy
    				select 
    					ID_LoHang,ID_HangHoa,TonDauKy											
    				from
    					(
    					select 
    						ID_LoHang,ID_HangHoa,TonDauKy,
    						ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    					from #temp
    					where NgayLapHoaDon < @NgayLapHDMin 
    						and not exists (select * from @tblHangKiemKe kk where #temp.ID_HangHoa =  kk.ID_HangHoa and (#temp.ID_LoHang = kk.ID_LoHang OR #temp.ID_LoHang is null))
    					) luyke	
    				where luyke.RN= 1
    
    				-- caculator again TonLuyKe for all cthd 'liên quan'
    				INSERT INTO @hdctUpdate
    				select ID, ID_DonVi, ID_CheckIn,
    						ISNULL(lkdk.TonLuyKe, 0) + 
    						(SUM(IIF(LoaiHoaDon IN (1, 5, 7, 8, 2), -1 * a.SoLuong* a.TyLeChuyenDoi, 
    					IIF(LoaiHoaDon IN (4, 6, 18,13,14), SoLuong * TyLeChuyenDoi, 				
    						IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    					IIF(a.LoaiHoaDon = 10 AND a.YeuCau = '4' AND a.ID_CheckIn = @IDChiNhanhInput, a.TienChietKhau* a.TyLeChuyenDoi, 0))))) 
    						OVER(PARTITION BY a.ID_HangHoa, a.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe,
    						LoaiHoaDon, MaHoaDon,NgayLapHoaDon,YeuCau
    				from
    					(
    					select distinct
    						ID,
    						ID_LoHang,
    						SoLuong,
    						TienChietKhau,
    						ThanhTien,
    						ID_HangHoa,
    						TyLeChuyenDoi,
    						MaHoaDon,
    						LoaiHoaDon,
    						NgayLapHoaDon,
    						ID_DonVi,
    						ID_CheckIn,
    						YeuCau
    					from #temp
    					where NgayLapHoaDon >= @NgayLapHDMin
    					and not exists (select * from @tblHangKiemKe kk where #temp.ID_HangHoa =  kk.ID_HangHoa and (#temp.ID_LoHang = kk.ID_LoHang OR #temp.ID_LoHang is null))
    					) a
    				LEFT JOIN @LuyKeDauKy lkdk ON lkdk.ID_HangHoa = a.ID_HangHoa AND (lkdk.ID_LoHang = a.ID_LoHang OR a.ID_LoHang IS NULL)					
    			end
    		
    		--select *, 1 as after2 from @LuyKeDauKy
    		--select * , @NgayLapHDMin as NgayMin from @hdctUpdate order by NgayLapHoaDon desc
    
    		UPDATE hdct
    	SET hdct.TonLuyKe = IIF(tlkupdate.ID_DonVi = @IDChiNhanhInput, tlkupdate.TonLuyKe, hdct.TonLuyKe), 
    		hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput and tlkupdate.LoaiHoaDon = 10, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
    	FROM BH_HoaDon_ChiTiet hdct
    	INNER JOIN @hdctUpdate tlkupdate ON hdct.ID = tlkupdate.ID where tlkupdate.LoaiHoaDon !=9 -- don't update TonLuyKe of HD KiemKe
    
    		-- get TonKho hientai full ID_QuiDoi, ID_LoHang of ID_HangHoa
    		DECLARE @tblTonKho1 TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, TonKho FLOAT, ID_LoHang UNIQUEIDENTIFIER)
    		INSERT INTO @tblTonKho1
    		SELECT qd.ID, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput,qd.ID_HangHoa,ID_LoHang, DATEADD(HOUR, 1,GETDATE()))/qd.TyLeChuyenDoi as TonKho, ID_LoHang 
    		FROM @tblChiTiet ct
    		join DonViQuiDoi qd on ct.ID_HangHoa = qd.ID_HangHoa 
    		
    		--select * from @tblTonKho1
    
    		-- UPDATE TonKho in DM_HangHoa_TonKho
    		UPDATE hhtonkho SET hhtonkho.TonKho = ISNULL(cthoadon.TonKho, 0)
    		FROM DM_HangHoa_TonKho hhtonkho
    		INNER JOIN @tblTonKho1 as cthoadon on hhtonkho.ID_DonViQuyDoi = cthoadon.ID_DonViQuiDoi 
    			and (hhtonkho.ID_LoHang = cthoadon.ID_LoHang or cthoadon.ID_LoHang is null) and hhtonkho.ID_DonVi = @IDChiNhanhInput

		--- insert row into DM_HangHoa_TonKho if not exists
    INSERT INTO DM_HangHoa_TonKho(ID, ID_DonVi, ID_DonViQuyDoi, ID_LoHang, TonKho)
	SELECT NEWID(), @IDChiNhanhInput, cthoadon1.ID_DonViQuiDoi, cthoadon1.ID_LoHang, cthoadon1.TonKho
    FROM @tblTonKho1 AS cthoadon1
    LEFT JOIN DM_HangHoa_TonKho hhtonkho1 on hhtonkho1.ID_DonViQuyDoi = cthoadon1.ID_DonViQuiDoi 
	and (hhtonkho1.ID_LoHang = cthoadon1.ID_LoHang or cthoadon1.ID_LoHang is null) and hhtonkho1.ID_DonVi = @IDChiNhanhInput
	WHERE hhtonkho1.ID IS NULL

	
	exec Insert_ThongBaoHetTonKho @IDChiNhanhInput, @LoaiHoaDon, @tblHoaDonChiTiet

    
    		-- delete cthd was delete in cthd update
    		--delete from BH_HoaDon_ChiTiet where id in (select id from @tblChiTiet where ChatLieu='5') OR(ID_HoaDon = @IDHoaDonInput and ChatLieu='5')	
    		delete from BH_HoaDon_ChiTiet where ID_HoaDon = @IDHoaDonInput and ChatLieu='5'
    
    		-- neu update NhanHang --> goi ham update TonKho 2 lan
    		-- update GiaVon neu tontai phieu NhapHang,ChuyenHang/NhanHang, DieuChinhGiaVon 
    		declare @count2 float = (select count(ID) from @hdctUpdate where LoaiHoaDon in (4,7,10, 18))
    		select ISNULL(@count2,0) as UpdateGiaVon, ISNULL(@count,0) as UpdateKiemKe, @NgayLapHDMin as NgayLapHDMin

		
	


END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_ThuChi_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    SELECT 
    MAX(b.TenNhomDoiTuong) as NhomDoiTuong,
    b.MaHoaDon,
    MAX(b.MaPhieuThu) as MaPhieuThu,
    MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    MAX(b.ManguoiNop) as ManguoiNop, 
    MAX(b.TenNguoiNop) as TenNguoiNop, 
	MAX(b.TienMat) AS TienMat,
	MAX(b.TienGui) AS TienGui,
	MAX(b.TienPOS) AS TienPOS,
    MAX(b.ThuChi) as ThuChi, 
    MAX(b.NoiDungThuChi) as NoiDungThuChi,
    MAX(b.GhiChu) as GhiChu,
    MAX(b.LoaiThuChi) as LoaiThuChi,
    	dv.TenDonVi AS TenChiNhanh,
		b.SoTaiKhoan, b.TenNganHang
    FROM
    (
    	  select 
    		a.ID_DoiTuong,
    		a.ID_HoaDon,
    		a.TenNhomDoiTuong,
    		a.ID_NhomDoiTuong,
    	a.MaHoaDon,
    		a.MaPhieuThu,
    	a.NgayLapHoaDon,
    		a.MaNguoiNop,
    	a.TenNguoiNop,
    	--a.ThuChi,
		a.TienMat,
		a.TienGui,
		a.TienPOS,
    		a.TienMat + a.TienGui + a.TienPOS as ThuChi,
    	a.NoiDungThuChi,
    	a.GhiChu,
    	Case when a.LoaiThuChi = 1 then N'Phiếu thu khác'  
    	when a.LoaiThuChi = 2 then N'Phiếu chi khác' 
    	when a.LoaiThuChi = 3 then N'Thu tiền khách trả'  
    	when a.LoaiThuChi = 4 then N'Chi tiền đổi trả hàng'  
    	when a.LoaiThuChi = 5 then N'Thu tiền nhà NCC'  
    	when a.LoaiThuChi = 6 then N'Chi tiền trả NCC' else '' end as LoaiThuChi,
    		a.ID_DonVi,
			a.SoTaiKhoan,
			a.TenNganHang
    	From
    	(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			tknh.SoTaiKhoan as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
    				--Max(dt.TenNhomDoiTuongs) as TenNhomDoiTuong,
    				case when qhdct.ID_NhanVien is not null then N'Nhân viên' else MAX(dt.TenNhomDoiTuongs) end as TenNhomDoiTuong,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case WHEN qhdct.ID_NhanVien is not null
    				then
    				'00000000-0000-0000-0000-000000000000' 
    				else 
    				case When dtn.ID_NhomDoiTuong is null 
    					
    				then '00000000-0000-0000-0000-000000000000'  else dtn.ID_NhomDoiTuong 
    				end
    				end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    				case when qhdct.ID_NhanVien is not null then nv.MaNhanVien else dt.MaDoiTuong end as ManguoiNop,
    			Sum(qhdct.TienMat) as TienMat,
    			IIF(tknh.TaiKhoanPOS = 1, 0, Sum(qhdct.TienGui)) as TienGui,
				IIF(tknh.TaiKhoanPOS = 1, SUM(qhdct.TienGui), 0) AS TienPOS,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
    				qhd.ID_DonVi,
				nh.TenNganHang
    		From Quy_HoaDon qhd 			
    			join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    				left join NS_NhanVien nv on qhdct.ID_NhanVien= nv.ID
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on tknh.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart and @timeEnd 
    				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, dt.loaidoituong) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and qhdct.HinhThucThanhToan NOT IN (4, 5, 6)
    				AND ((select count(Name) from @tblSearch b where     			
    			dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or qhd.MaHoaDon like '%' + b.Name + '%'
    				or hd.MaHoaDon like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhdct.ID_NhanVien, dt.MaDoiTuong,dt.LoaiDoiTuong,  nv.MaNhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID, qhd.ID_DonVi,
				 tknh.TaiKhoanPOS, tknh.SoTaiKhoan, nh.TenNganHang
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) 
    	) b
    		inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    		where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = ''
    	Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi, dv.TenDonVi, b.SoTaiKhoan, b.TenNganHang
    	ORDER BY NgayLapHoaDon DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_SoQuy_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit],
    @LoaiTien [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    --	tinh ton dau ky
    	Declare @TonDauKy float
    	Set @TonDauKy = (Select
    	CAST(ROUND(SUM(TienThu - TienChi), 0) as float) as TonDauKy
    	FROM
    	(
    		select 
    			case when qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as TienThu,
    			Case when qhd.LoaiHoaDon = 12 then qhdct.TienThu else 0 end as TienChi,
    			Case when qhdct.TienMat > 0 and qhdct.TienGui = 0 then '1' 
    			when qhdct.TienGui > 0 and qhdct.TienMat = 0 then '2'
    			when qhdct.TienGui > 0 and qhdct.TienMat > 0 then '12' else '' end as LoaiTien,
    				qhd.HachToanKinhDoanh as HachToanKinhDoanh
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		where qhd.NgayLapHoaDon < @timeStart
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and qhdct.HinhThucThanhToan not in (4,5,6)
    		) a 
    		where LoaiTien like @LoaiTien
    			and (HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    	) 
    		
    	if (@TonDauKy is null)
    	BeGin
    		Set @TonDauKy = 0;
    	END
    	Declare @tmp table (ID_HoaDon UNIQUEIDENTIFIER,MaPhieuThu nvarchar(max), NgayLapHoaDon datetime, KhoanMuc nvarchar(max), TenDoiTac nvarchar(max),
    	TienMat float, TienGui float, TienThu float, TienChi float, ThuTienMat float, ChiTienMat float, ThuTienGui float, ThuTienPOS FLOAT,
		ChiTienGui float, ChiTienPOS FLOAT, TonLuyKeTienMat float,TonLuyKeTienGui float,TonLuyKe float, SoTaiKhoan nvarchar(max), NganHang nvarchar(max), GhiChu nvarchar(max),
    		IDDonVi UNIQUEIDENTIFIER, TenDonVi NVARCHAR(MAX));
    	Insert INTO @tmp
    		 SELECT 
    				b.ID_HoaDon,
    				b.MaPhieuThu as MaPhieuThu,
    			b.NgayLapHoaDon as NgayLapHoaDon,
    				MAX(b.NoiDungThuChi) as KhoanMuc,
    			MAX(b.TenNguoiNop) as TenDoiTac, 
    			SUM (b.TienMat) as TienMat,
    			SUM (b.TienGui) as TienGui,
    			SUM (b.TienThu) as TienThu,
    			SUM (b.TienChi) as TienChi,
    			SUM (b.ThuTienMat) as ThuTienMat,
    			SUM (b.ChiTienMat) as ChiTienMat, 
    			SUM (b.ThuTienGui) as ThuTienGui,
				SUM (b.ThuTienPOS) as ThuTienPOS,
    			SUM (b.ChiTienGui) as ChiTienGui, 
				SUM (b.ChiTienPOS) as ChiTienPOS, 
    				0 as TonLuyKe,
    			0 as TonLuyKeTienMat,
    			0 as TonLuyKeTienGui,
    			MAX(b.SoTaiKhoan) as SoTaiKhoan,
    			MAX(b.NganHang) as NganHang,
    			MAX(b.GhiChu) as GhiChu,
    				dv.ID,
    				dv.TenDonVi
    		FROM
    		(
    				select 
    			a.HachToanKinhDoanh,
    			a.ID_DoiTuong,
    			a.ID_HoaDon,
    			a.MaHoaDon,
    			a.MaPhieuThu,
    			a.NgayLapHoaDon,
    			a.TenNguoiNop,
    			a.TienMat,
    			a.TienGui,
				IIF(a.LoaiHoaDon = 11, IIF(a.TaiKhoanPOS = 1, 0, a.TienGui) , 0) AS ThuTienGui,
				IIF(a.LoaiHoaDon = 11, IIF(a.TaiKhoanPOS = 1, a.TienGui, 0) , 0) AS ThuTienPOS,
    			--case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
				IIF(a.LoaiHoaDon = 12, IIF(a.TaiKhoanPOS = 1, 0, a.TienGui) , 0) AS ChiTienGui,
				IIF(a.LoaiHoaDon = 12, IIF(a.TaiKhoanPOS = 1, a.TienGui, 0) , 0) AS ChiTienPOS,
    			--Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat,
    			case when a.LoaiHoaDon = 11 then a.TienThu else 0 end as TienThu,
    			Case when a.LoaiHoaDon = 12 then a.TienThu else 0 end as TienChi,
    			a.NoiDungThuChi,
    			a.NganHang,
    			a.SoTaiKhoan,
    			a.GhiChu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' else '' end  as LoaiTien,
    				a.ID_DonVi
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			tknh.SoTaiKhoan as SoTaiKhoan,
    			nh.TenNganHang as NganHang,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else 4 end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    			max(IIF(qhdct.HinhThucThanhToan = 1, qhdct.TienThu, 0)) as TienMat,
    			max(IIF(qhdct.HinhThucThanhToan IN (2,3) , qhdct.TienThu, 0)) as TienGui,
    			max(qhdct.TienThu) as TienThu,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
    				qhd.ID_DonVi,
					tknh.TaiKhoanPOS
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on tknh.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    				and qhdct.HinhThucThanhToan not in (4,5,6)
    				AND ((select count(Name) from @tblSearch b where     			
    			dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or qhd.MaHoaDon like '%' + b.Name + '%'
    				or qhd.NguoiNopTien like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau,qhdct.ID_NhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, qhd.ID_DonVi, qhdct.ID, qhdct.HinhThucThanhToan,
				 tknh.TaiKhoanPOS, tknh.SoTaiKhoan, nh.TenNganHang
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    			inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    			where LoaiTien like @LoaiTien
    		Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaPhieuThu, b.NgayLapHoaDon, dv.TenDonVi, dv.ID, b.SoTaiKhoan
    		ORDER BY NgayLapHoaDon
    -- tính tồn lũy kế
    	    IF (EXISTS (select * from @tmp))
    		BEGIN
    			DECLARE @Ton float;
    			SET @Ton = @TonDauKy;
    			DECLARE @TonTienMat float;
    			SET @TonTienMat = @TonDauKy;
    			DECLARE @TonTienGui float;
    			SET @TonTienGui = @TonDauKy;
    			
    			DECLARE @TienThu float;
    			DECLARE @TienChi float;
    			DECLARE @ThuTienMat float;
    			DECLARE @ChiTienMat float;
    			DECLARE @ThuTienGui float;
    			DECLARE @ChiTienGui float;
    			DECLARE @TonLuyKe float;
				DECLARE @ThuTienPOS float;
				DECLARE @ChiTienPOS float;
    				DECLARE @ID_HoaDon UNIQUEIDENTIFIER;
    	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT TienThu, TienChi, ThuTienGui, ThuTienMat, ChiTienGui, ChiTienMat, ID_HoaDon, ThuTienPOS, ChiTienPOS FROM @tmp ORDER BY NgayLapHoaDon
    	OPEN CS_ItemUpDate 
    FETCH FIRST FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon, @ThuTienPOS, @ChiTienPOS
    WHILE @@FETCH_STATUS = 0
    BEGIN
    	SET @Ton = @Ton + @ThuTienMat + @ThuTienGui + @ThuTienPOS - @ChiTienMat - @ChiTienGui - @ChiTienPOS;
    	SET @TonTienMat = @TonTienMat + @ThuTienMat - @ChiTienMat;
    	SET @TonTienGui = @TonTienGui + @ThuTienGui - @ChiTienGui + @ThuTienPOS - @ChiTienPOS;
    	UPDATE @tmp SET TonLuyKe = @Ton, TonLuyKeTienMat = @TonTienMat, TonLuyKeTienGui = @TonTienGui WHERE ID_HoaDon = @ID_HoaDon
		AND ThuTienMat = @ThuTienMat AND ThuTienGui = @ThuTienGui AND ThuTienPOS = @ThuTienPOS
		AND ChiTienMat = @ChiTienMat AND ChiTienGui = @ChiTienGui AND ChiTienPOS = @ChiTienPOS
    	FETCH NEXT FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon, @ThuTienPOS, @ChiTienPOS
    END
    CLOSE CS_ItemUpDate
    DEALLOCATE CS_ItemUpDate
    	END
    	ELSE
    	BEGIN
    		Insert INTO @tmp
    	SELECT '00000000-0000-0000-0000-000000000000', 'TRINH0001', '1989-04-07','','','0','0','0','0','0','0','0','0', '0', '0', @TonDauKy, @TonDauKy, @TonDauKy, '','','', '00000000-0000-0000-0000-000000000000', ''
    	END
    	Select 
    		ID_HoaDon,
    	MaPhieuThu,
    	NgayLapHoaDon,
    	KhoanMuc,
    	TenDoiTac,
    	@TonDauKy as TonDauKy,
    	TienMat,
    	TienGui,
    	TienThu,
    	TienChi,
    	ThuTienMat,
    	ChiTienMat,
    	ThuTienGui,
		ThuTienPOS,
    	ChiTienGui,
		ChiTienPOS,
    	TonLuyKe,
    	TonLuyKeTienMat,
    	TonLuyKeTienGui,
    	SoTaiKhoan, 
    	NganHang, 
    	GhiChu,
    		IDDonVi, TenDonVi
    	 from @tmp order by NgayLapHoaDon
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
