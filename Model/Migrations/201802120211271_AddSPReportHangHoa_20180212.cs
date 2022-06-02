namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSPReportHangHoa_20180212 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_BanHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
	dvqd.MaHangHoa,
	Case when dvqd.TenDonVitinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
	hh.ID_NhomHang,
	CAST(ROUND(a.SoLuongban , 3) as float ) as SoLuongBan,
	CAST(ROUND(a.GiaTriBan , 0) as float ) as GiaTriBan,
	CAST(ROUND(a.SoLuongTra , 3) as float ) as SoLuongTra,
	CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
	CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan
	FROM
	(
		SELECT 
		dvqd.ID as ID_DonViQuiDoi,
		SUM(Case when hd.LoaiHoaDon = 1 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongBan,
		SUM(Case when hd.LoaiHoaDon = 6 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongTra,
		SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
		SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra
		FROM
		BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi = @ID_ChiNhanh
		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
		and hh.LaHangHoa like @LaHangHoa
		GROUP BY dvqd.ID
	) a
	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	ORDER BY DoanhThuThuan DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_KhachHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
	dvqd.MaHangHoa,
	Case when dvqd.TenDonVitinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
	hh.ID_NhomHang,
	a.SoLuongKhachHang,
	CAST(ROUND(a.SoLuongMua , 3) as float ) as SoLuongMua,
	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
	FROM
	(
		SELECT 
		c.ID_DonViQuiDoi,
		COUNT(*) as SoLuongKhachHang,
		SUM(ISNULL(c.SoLuongMua, 0)) as SoLuongMua,
		SUM(ISNULL(c.GiaTri, 0)) as GiaTri
		FROM
		(
			SELECT 
			dvqd.ID as ID_DonViQuiDoi,
			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongMua,
			SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) as GiaTri
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.LoaiHoaDon = 1
			and hd.ChoThanhToan = 0
			and hd.ID_DonVi = @ID_ChiNhanh
			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
			and hh.LaHangHoa like @LaHangHoa
			GROUP BY dvqd.ID, hd.ID_DoiTuong
		) c
		GROUP BY ID_DonViQuiDoi
	) a
	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	ORDER BY SoLuongKhachHang DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_LoiNhuan]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT 
	*, 
	Case When HH.DoanhThuThuan != 0 then CAST(ROUND((HH.LoiNhuan / HH.DoanhThuThuan) * 100, 2) as float ) else 0 end  as TySuat
	FROM
	(
		SELECT
		dvqd.MaHangHoa,
		Case when dvqd.TenDonVitinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
		hh.ID_NhomHang,
		CAST(ROUND(a.SoLuongban , 3) as float ) as SoLuongBan,
		CAST(ROUND(a.GiaTriBan , 0) as float ) as DoanhThu,
		CAST(ROUND(a.SoLuongTra , 3) as float ) as SoLuongTra,
		CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
		CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan,
		CAST(ROUND(a.TongGiaVonBan - a.TongGiaVonTra , 0) as float ) as TongGiaVon,
		CAST(ROUND(a.GiaTriBan - a.GiaTriTra - a.TongGiaVonBan + a.TongGiaVonTra , 0) as float ) as LoiNhuan
		FROM
		(
			SELECT 
			dvqd.ID as ID_DonViQuiDoi,
			SUM(Case when hd.LoaiHoaDon = 1 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongBan,
			SUM(Case when hd.LoaiHoaDon = 6 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongTra,
			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra,
			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.SoLuong, 0) * ISNULL(hdct.GiaVon, 0)) else 0 end) as TongGiaVonBan,
			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.SoLuong, 0) * ISNULL(hdct.GiaVon, 0)) else 0 end) as TongGiaVonTra
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
			and hd.ChoThanhToan = 0
			and hd.ID_DonVi = @ID_ChiNhanh
			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
			and hh.LaHangHoa like @LaHangHoa
			GROUP BY dvqd.ID
		) a
		left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	) as HH
	ORDER BY LoiNhuan DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_NhaCungCap]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
	dvqd.MaHangHoa,
	Case when dvqd.TenDonVitinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
	hh.ID_NhomHang,
	a.SoLuongNhaCungCap,
	CAST(ROUND(a.SoLuongSanPham , 3) as float ) as SoLuongSanPham,
	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
	FROM
	(
		SELECT 
		c.ID_DonViQuiDoi,
		COUNT(*) as SoLuongNhaCungCap,
		SUM(ISNULL(c.SoLuongSanPham, 0)) as SoLuongSanPham,
		SUM(ISNULL(c.GiaTri, 0)) as GiaTri
		FROM
		(
			SELECT 
			dvqd.ID as ID_DonViQuiDoi,
			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
			SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) as GiaTri
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.LoaiHoaDon = 4
			and hd.ChoThanhToan = 0
			and hd.ID_DonVi = @ID_ChiNhanh
			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
			and hh.LaHangHoa like @LaHangHoa
			GROUP BY dvqd.ID, hd.ID_DoiTuong
		) c
		GROUP BY ID_DonViQuiDoi
	) a
	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	ORDER BY SoLuongNhaCungCap DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_NhanVien]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
	dvqd.MaHangHoa,
	Case when dvqd.TenDonVitinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
	hh.ID_NhomHang,
	a.SoLuongNhanVien,
	CAST(ROUND(a.SoLuongban , 3) as float ) as SoLuongBan,
	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
	FROM
	(
		SELECT 
		c.ID_DonViQuiDoi,
		COUNT(*) as SoLuongNhanVien,
		SUM(ISNULL(c.SoLuongBan, 0)) as SoLuongBan,
		SUM(ISNULL(c.GiaTri, 0)) as GiaTri
		FROM
		(
			SELECT 
			dvqd.ID as ID_DonViQuiDoi,
			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongBan,
			SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) as GiaTri
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.LoaiHoaDon = 1
			and hd.ChoThanhToan = 0
			and hd.ID_DonVi = @ID_ChiNhanh
			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
			and hh.LaHangHoa like @LaHangHoa
			GROUP BY dvqd.ID, hd.ID_NhanVien
		) c
		GROUP BY ID_DonViQuiDoi
	) a
	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	ORDER BY SoLuongNhanVien DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatHuy]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
	dvqd.MaHangHoa,
	Case when dvqd.TenDonVitinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
	hh.ID_NhomHang,
	CAST(ROUND(a.TongSoLuongHuy , 3) as float) AS TongSoLuongHuy,
	CAST(ROUND(a.TongGiaTriHuy , 0) as float ) AS TongGiaTriHuy
	FROM
	(
		SELECT 
		dvqd.ID as ID_DonViQuiDoi,
		SUM(ISNULL(hdct.SoLuong, 0)) as TongSoLuongHuy,
		SUM(ISNULL(hdct.ThanhTien, 0)) as TongGiaTriHuy
		FROM
		BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and hd.LoaiHoaDon = 8
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi = @ID_ChiNhanh
		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
		and hh.LaHangHoa like @LaHangHoa
		GROUP BY dvqd.ID
	) a
	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	ORDER BY TongGiaTriHuy DESC");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportHangHoa_BanHang]");
            DropStoredProcedure("[dbo].[ReportHangHoa_KhachHang]");
            DropStoredProcedure("[dbo].[ReportHangHoa_LoiNhuan]");
            DropStoredProcedure("[dbo].[ReportHangHoa_NhaCungCap]");
            DropStoredProcedure("[dbo].[ReportHangHoa_NhanVien]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatHuy]");
        }
    }
}
