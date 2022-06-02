namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20191003 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[LoadDanhMucTheGiaTri]", body: @"
SET NOCOUNT ON;
     SELECT hd.ID, hd.ID_DonVi,qct.ID as ID_QuyCT, hd.MaHoaDon ,hd.LoaiHoaDon, hd.NgayLapHoaDon, dt.TenDoiTuong as TenKhachHang, dt.ID as ID_DoiTuong,
	dt.DiaChi as DiaChiKhachHang, dv.DiaChi as DiaChiChiNhanh, dv.SoDienThoai as DienThoaiChiNhanh, dt.DienThoai as DienThoaiKhachHang, ISNULL(qct.TienMat, 0) as TienMat, ISNULL(qct.TienGui,0) as TienGui,
	dv.TenDonVi,hd.NguoiTao, hd.TongChiPhi as MucNap, hd.TongChietKhau as KhuyenMaiVND, hd.TongTienHang as TongTienNap, hd.TongTienThue as SoDuSauNap, hd.TongGiamGia as ChietKhauVND,
	hd.PhaiThanhToan as PhaiThanhToan, hd.DienGiai as GhiChu, hd.ChoThanhToan, tknh.TaiKhoanPOS, Main.NhanVienThucHien
	FROM BH_HoaDon hd
	LEFT JOIN DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
	LEFT JOIN DM_DonVi dv on hd.ID_DonVi = dv.ID
	LEFT JOIN Quy_HoaDon_ChiTiet qct on hd.ID = qct.ID_HoaDonLienQuan
	LEFT JOIN DM_TaiKhoanNganHang tknh on qct.ID_TaiKhoanNganHang = tknh.ID
	left join (
    Select distinct nv_khnv.ID_NhanVien,
    (
    Select nv.TenNhanVien + ',' AS [text()]
					From dbo.BH_NhanVienThucHien nvth
    				inner join dbo.NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
					Where nvth.ID_QuyHoaDon = nv_khnv.ID_QuyHoaDon
    For XML PATH ('')
    ) NhanVienThucHien, nv_khnv.ID_HoaDon
    From dbo.BH_NhanVienThucHien nv_khnv
    ) Main on hd.ID = Main.ID_HoaDon
	WHERE hd.LoaiHoaDon = 22
	group by hd.ID, hd.ID_DonVi,qct.ID, qct.ID_HoaDonLienQuan, hd.MaHoaDon,hd.LoaiHoaDon, hd.NgayLapHoaDon, dt.TenDoiTuong, dt.ID,
	dt.DiaChi, dv.DiaChi, dv.SoDienThoai, dt.DienThoai, qct.TienMat, qct.TienGui,
	dv.TenDonVi,hd.NguoiTao, hd.TongChiPhi, hd.TongChietKhau, hd.TongTienHang, hd.TongTienThue, hd.TongGiamGia,
	hd.PhaiThanhToan, hd.DienGiai, hd.ChoThanhToan, tknh.TaiKhoanPOS,Main.NhanVienThucHien
	order by NgayLapHoaDon desc
");

            CreateStoredProcedure(name: "[dbo].[UpdateAgainNhomDT_InDMDoiTuong_AfterChangeDKNangNhom]", parametersAction: p => new
            {
                ID_DoiTuongs = p.String(),
                ID_NhomDoiTuong = p.String(36)
            }, body: @"delete from DM_DoiTuong_Nhom where ID_NhomDoiTuong = @ID_NhomDoiTuong 
	and not exists (select Name from dbo.splitstring(@ID_DoiTuongs) a where a.Name= ID_DoiTuong)

	declare @TenNhomDT nvarchar = (select TenNhomDoiTuong from DM_NhomDoiTuong where ID= @ID_NhomDoiTuong)
	update DM_DoiTuong 
		set TenNhomDoiTuongs= replace (TenNhomDoiTuongs, concat(@TenNhomDT,', '),''), 
			IDNhomDoiTuongs = replace (IDNhomDoiTuongs, concat(@ID_NhomDoiTuong,', '),'')
	where IDNhomDoiTuongs like '%'+ @ID_NhomDoiTuong + '%'
	and TheoDoi='0'
	and not exists (select Name from dbo.splitstring(@ID_DoiTuongs) a where a.Name= ID)

	update DM_DoiTuong set TenNhomDoiTuongs =N'Nhóm mặc định' where TenNhomDoiTuongs=''	");


            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_BieuDoDoanhThuToHour]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_NguoiDung [uniqueidentifier],
	@ID_DonVi nvarchar (max)
AS
BEGIN
	 DECLARE @LaAdmin as nvarchar
     Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd	where nd.ID = @ID_NguoiDung)
	 IF(@LaAdmin = 1)
	 BEGIN
		SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
			-- tongmua
    		SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		hdb.PhaiThanhToan as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

			union all
			-- tongtra
			SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		- hdb.PhaiThanhToan as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))
		) a
    	GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon
		END
	ELSE
	BEGIN
	SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
			-- tongmua
    		SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		hdb.PhaiThanhToan as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

			union all
			-- tongtra
			SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		- hdb.PhaiThanhToan as ThanhTien 
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))
		) a
    	GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon
	END
END

--BaoCaoTongQuan_BieuDoDoanhThuToHour '2019-09-13','2019-09-14','28FEF5A1-F0F2-4B94-A4AD-081B227F3B77','D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_DoiTuong_HinhThuc1]
    @SqlQuery [nvarchar](max)
AS
BEGIN
    declare @sql  [nvarchar](max);
    	declare @sql2  [nvarchar](max);
    	set @sql = 'SELECT b.ID_DoiTuong
    	FROM
    	(
    		SELECT
    		a.ID_DoiTuong,
    		SUM(CAST(ROUND(a.GiaTriBan + a.DieuChinhThe, 0) as float )) as GiaTriBan,
    		SUM(CAST(ROUND(a.GiaTriTra * (-1), 0) as float )) as GiaTriTra,
    		SUM(CAST(ROUND(a.GiaTriBan + a.DieuChinhThe - a.GiaTriTra , 0) as float )) as DoanhThuThuan
    		FROM
    		(
				-- ban + trahang
    			SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			Case when (hd.LoaiHoaDon = 1 OR hd.LoaiHoaDon = 19) then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiaTriBan,
    			Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiaTriTra,
				0 as DieuChinhThe
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6 or hd.LoaiHoaDon = 19)
    			and hd.ChoThanhToan = 0 and hd.TongTienHang > 0
    			and hd.ID_DoiTuong is not null

				union all
				-- doanh thu the
				SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			hd.PhaiThanhToan as GiaTriBan,
    			0 as GiaTriTra,
				0 as DieuChinhThe
    			FROM BH_HoaDon hd    			
    			where hd.LoaiHoaDon = 22
    			and hd.ChoThanhToan = 0 
    			and hd.ID_DoiTuong is not null

				union all
				-- dieuchinhthe
				SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			0 as GiaTriBan,
    			0 as GiaTriTra,
				ISNULL(hd.TongChiPhi,0) as DieuChinhThe
    			FROM BH_HoaDon hd    			
    			where hd.LoaiHoaDon = 23
    			and hd.ChoThanhToan = 0 
    			and hd.ID_DoiTuong is not null		
    		) a
			group by a.ID_DoiTuong
    	) b
    	where DoanhThuThuan'
    	set @sql2 = @sql + @SqlQuery;
    	exec (@sql2);
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_DoiTuong_HinhThuc2]
    @SqlQuery [nvarchar](max)
AS
BEGIN
    declare @sql  [nvarchar](max);
    	declare @sql2  [nvarchar](max);
    	set @sql = 'SELECT b.ID_DoiTuong
    	FROM
    	(
    		-- chi get GiaTriBan
			SELECT
    			a.ID_DoiTuong,
    			SUM(CAST(ROUND(a.GiaTriBan + a.DieuChinhThe, 0) as float )) as GiaTriBan,
    			CAST (0 as float) as GiaTriTra,
    			CAST(0 as float) as DoanhThuThuan
    		FROM
    		(
				-- banhang
    			SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			ISNULL(hdct.ThanhTien, 0) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) as GiaTriBan,
				0 as DieuChinhThe
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    			and hd.ChoThanhToan = 0 and hd.TongTienHang > 0
    			and hd.ID_DoiTuong is not null

				union all
				-- doanh thu the
				SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			hd.PhaiThanhToan as GiaTriBan,
				0 as DieuChinhThe
    			FROM BH_HoaDon hd    			
    			where hd.LoaiHoaDon = 22
    			and hd.ChoThanhToan = 0 
    			and hd.ID_DoiTuong is not null

				union all
				-- dieuchinhthe
				SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			0 as GiaTriBan,
				ISNULL(hd.TongChiPhi,0) as DieuChinhThe
    			FROM BH_HoaDon hd    			
    			where hd.LoaiHoaDon = 23
    			and hd.ChoThanhToan = 0 
    			and hd.ID_DoiTuong is not null		
    		) a
			group by a.ID_DoiTuong
    	) b
    	where GiaTriBan'
    	set @sql2 = @sql + @SqlQuery;
    	exec (@sql2);
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_DoiTuong_HinhThuc4]
    @SqlQuery [nvarchar](max)
AS
BEGIN
    declare @sql  [nvarchar](max);
    	declare @sql2  [nvarchar](max);
    	set @sql = 'select a.ID_DoiTuong, a.SoLanMuaHang from
    				(
    				select hd.ID_DoiTuong, Count(hd.ID) as SoLanMuaHang from BH_HoaDon hd
    				where hd.ChoThanhToan = 0
    				and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22)
    				and hd.ID_DoiTuong is not null
    				GROUP by hd.ID_DoiTuong
    				) a
    				where SoLanMuaHang';
    	set @sql2 = @sql + @SqlQuery;
    	exec (@sql2);
END");

            Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMucCongViec]
    @ListID_DonVi [nvarchar](max),
	@ListID_NVPhoiHop [nvarchar](max),
	@ListID_LoaiCongViec [nvarchar](max),
	@ListID_NhanVien [nvarchar](max),
	@ListID_NhanVienQL [nvarchar](max),
	@DayStart [DATETIME],
	@DayEnd [DATETIME]
AS
BEGIN
    SET NOCOUNT ON;

	SELECT cv.ID, cv.ID_DonVi,cv.ID_LoaiTuVan, Main.ChuoiNhanVienPhoiHop, cv.ID_NhanVienQuanLy,loaicv.TenLoaiTuVanLichHen as LoaiCongViec, cv.Ma_TieuDe, dt.TenDoiTuong as TenKhachHang,
	dt.MaDoiTuong, lienhe.TenLienHe, nv.TenNhanVien as TenNhanVienPhuTrach, cv.NgayGio, cv.NgayGioKetThuc, cv.NhacNho, cv.NoiDung, dt.LoaiDoiTuong as LoaiDoiTuongCV,
	cv.TrangThai, dt.ID_TrangThai as TrangThaiKhach, cv.NguoiTao, cv.NgayTao, cv.GhiChu,cv.KetQua, cv.MucDoUuTien, cv.FileDinhKem,
	cv.NgayHoanThanh, dt.DienThoai as SoDienThoai, nguonk.TenNguonKhach as NguonKhach FROM ChamSocKhachHangs cv
	inner join DM_LoaiTuVanLichHen loaicv on cv.ID_LoaiTuVan = loaicv.ID
	inner join DM_DoiTuong dt on cv.ID_KhachHang = dt.ID
	left join DM_NguonKhachHang nguonk on dt.ID_NguonKhach = nguonk.ID
	left join DM_LienHe lienhe on cv.ID_LienHe = lienhe.ID
	left join NS_NhanVien nv on cv.ID_NhanVien = nv.ID
	left join (
        Select distinct cs_khnv.ID_ChamSocKhachHang,
            (
                Select nvl.TenNhanVien + ',' AS [text()]
                From dbo.ChamSocKhachHang_NhanVien cskhnv
				inner join dbo.NS_NhanVien nvl on cskhnv.ID_NhanVien = nvl.ID
                Where cskhnv.ID_ChamSocKhachHang = cs_khnv.ID_ChamSocKhachHang
                For XML PATH ('')
            ) ChuoiNhanVienPhoiHop, cs_khnv.ID_NhanVien
        From dbo.ChamSocKhachHang_NhanVien cs_khnv
    ) Main on cv.ID = Main.ID_ChamSocKhachHang
	where cv.PhanLoai = 4 and cv.TrangThai != '0'
	and cv.NgayGio >= @DayStart and cv.NgayGioKetThuc < @DayEnd
	and ((select TOP 1 [name] from splitstring(@ListID_DonVi) ORDER BY [name]) = '' or cv.ID_DonVi=(select * from splitstring(@ListID_DonVi) where [name] like cv.ID_DonVi))
	and ((select TOP 1 [name] from splitstring(@ListID_NVPhoiHop) ORDER BY [name]) = '' or Main.ID_NhanVien=(select * from splitstring(@ListID_DonVi) where [name] like Main.ID_NhanVien))
	and ((select TOP 1 [name] from splitstring(@ListID_LoaiCongViec) ORDER BY [name]) = '' or cv.ID_LoaiTuVan=(select * from splitstring(@ListID_LoaiCongViec) where [name] like cv.ID_LoaiTuVan))
	and ((select TOP 1 [name] from splitstring(@ListID_NhanVien) ORDER BY [name]) = '' or cv.ID_NhanVien=(select * from splitstring(@ListID_NhanVien) where [name] like cv.ID_NhanVien))
	and ((select TOP 1 [name] from splitstring(@ListID_NhanVienQL) ORDER BY [name]) = '' or cv.ID_NhanVienQuanLy=(select * from splitstring(@ListID_NhanVienQL) where [name] like cv.ID_NhanVienQuanLy) or cv.ID_NhanVien=(select * from splitstring(@ListID_NhanVienQL) where [name] like cv.ID_NhanVien))
	order by NgayTao desc
END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[LoadDanhMucTheGiaTri]");
            DropStoredProcedure("[dbo].[UpdateAgainNhomDT_InDMDoiTuong_AfterChangeDKNangNhom]");
        }
    }
}
