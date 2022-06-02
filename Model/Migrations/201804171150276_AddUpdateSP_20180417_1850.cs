namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180417_1850 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhMonth_ChiPhiBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.ThangLapHoaDon,
	CAST(ROUND(SUM(ISNULL(a.GiaTriHuy, 0)), 0) as float) as GiaTriHuy,
	CAST(ROUND(SUM(ISNULL(a.DiemThanhToan, 0)), 0) as float) as DiemThanhToan
	FROM
	(
		Select 
		DATEPART(MONTH, hd.NgayLapHoaDon) as ThangLapHoaDon,
		ISNULL(hdct.ThanhTien ,0) as GiaTriHuy,
		null as DiemThanhToan
		From BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		where hd.LoaiHoaDon = 8
		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		UNION ALL
		Select 
		DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
		null as GiaTriHuy,
		ISNULL(qhdct.TienThu ,0) as DiemThanhToan
		From Quy_HoaDon qhd
		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
		and DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		and qhdct.DiemThanhToan > 0
	) as a
	GROUP BY
	a.ThangLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhMonth_DoanhThuBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.ThangLapHoaDon,
	CAST(ROUND(SUM(a.DoanhThu), 0) as float) as DoanhThu,
	CAST(ROUND(SUM(a.GiaTriTra), 0) as float) as GiaTriTra,
	CAST(ROUND(SUM(a.GiamGiaHD), 0) as float) as GiamGiaHD
	FROM
	(
		Select 
		DATEPART(MONTH, hd.NgayLapHoaDon) as ThangLapHoaDon,
		hd.LoaiHoaDon,
		Case When hd.LoaiHoaDon = 1 then ISNULL(hd.TongTienHang ,0) else 0 end as DoanhThu,
		Case When hd.LoaiHoaDon = 6 then ISNULL(hd.PhaiThanhToan ,0) else 0 end as GiaTriTra,
		Case When hd.LoaiHoaDon = 1 then ISNULL(hd.TongGiamGia ,0) else 0 end as GiamGiaHD
		From BH_HoaDon hd
		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	) as a
	GROUP BY
	a.ThangLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhMonth_GiaVonBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.ThangLapHoaDon,
	CAST(ROUND(SUM(a.TongGiaVonBan), 0) as float) as TongGiaVonBan,
	CAST(ROUND(SUM(a.TongGiaVonTra), 0) as float) as TongGiaVonTra
	FROM
	(
		Select 
		DATEPART(MONTH, hd.NgayLapHoaDon) as ThangLapHoaDon,
		hd.LoaiHoaDon,
		Case When hd.LoaiHoaDon = 1 then ISNULL(hdct.SoLuong * hdct.GiaVon,0) else 0 end as TongGiaVonBan,
		Case When hd.LoaiHoaDon = 6 then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as TongGiaVonTra
		From BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	) as a
	GROUP BY
	a.ThangLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhMonth_SoQuyBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.ThangLapHoaDon,
	CAST(ROUND(SUM(ISNULL(a.ThuNhapKhac, 0)), 0) as float) as ThuNhapKhac,
	CAST(ROUND(SUM(ISNULL(a.ChiPhiKhac, 0)), 0) as float) as ChiPhiKhac,
	CAST(ROUND(SUM(ISNULL(a.PhiTraHangNhap, 0)), 0) as float) as PhiTraHangNhap,
	CAST(ROUND(SUM(ISNULL(a.KhachThanhToan, 0)), 0) as float) as KhachThanhToan
	FROM
	(
		Select 
		DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then ISNULL(qhdct.TienThu, 0) else 0 end as ThuNhapKhac,
		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) then ISNULL(qhdct.TienThu, 0) else 0 end as ChiPhiKhac,
		Case when (hd.LoaiHoaDon = 7 and qhd.LoaiHoaDon = 11) then ISNULL(qhdct.TienThu, 0) else 0 end as PhiTraHangNhap,
		Case when (hd.LoaiHoaDon = 1 and qhd.LoaiHoaDon = 11) then ISNULL(qhdct.TienThu, 0) else 0 end as KhachThanhToan
		From Quy_HoaDon qhd
		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
		and DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	) as a
	GROUP BY
	a.ThangLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhYear_ChiPhiBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.NamLapHoaDon,
	CAST(ROUND(SUM(ISNULL(a.GiaTriHuy, 0)), 0) as float) as GiaTriHuy,
	CAST(ROUND(SUM(ISNULL(a.DiemThanhToan, 0)), 0) as float) as DiemThanhToan
	FROM
	(
		Select 
		DATEPART(YEAR, hd.NgayLapHoaDon) as NamLapHoaDon,
		ISNULL(hdct.ThanhTien ,0) as GiaTriHuy,
		null as DiemThanhToan
		From BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		where hd.LoaiHoaDon = 8
		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		UNION ALL
		Select 
		DATEPART(YEAR, qhd.NgayLapHoaDon) as NamLapHoaDon,
		null as GiaTriHuy,
		ISNULL(qhdct.TienThu ,0) as DiemThanhToan
		From Quy_HoaDon qhd
		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
		and DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		and qhdct.DiemThanhToan > 0
	) as a
	GROUP BY
	a.NamLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhYear_DoanhThuBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.NamLapHoaDon,
	CAST(ROUND(SUM(a.DoanhThu), 0) as float) as DoanhThu,
	CAST(ROUND(SUM(a.GiaTriTra), 0) as float) as GiaTriTra,
	CAST(ROUND(SUM(a.GiamGiaHD), 0) as float) as GiamGiaHD
	FROM
	(
		Select 
		DATEPART(YEAR, hd.NgayLapHoaDon) as NamLapHoaDon,
		hd.LoaiHoaDon,
		Case When hd.LoaiHoaDon = 1 then ISNULL(hd.TongTienHang ,0) else 0 end as DoanhThu,
		Case When hd.LoaiHoaDon = 6 then ISNULL(hd.PhaiThanhToan ,0) else 0 end as GiaTriTra,
		Case When hd.LoaiHoaDon = 1 then ISNULL(hd.TongGiamGia ,0) else 0 end as GiamGiaHD
		From BH_HoaDon hd
		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	) as a
	GROUP BY
	a.NamLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhYear_GiaVonBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.NamLapHoaDon,
	CAST(ROUND(SUM(a.TongGiaVonBan), 0) as float) as TongGiaVonBan,
	CAST(ROUND(SUM(a.TongGiaVonTra), 0) as float) as TongGiaVonTra
	FROM
	(
		Select 
		DATEPART(YEAR, hd.NgayLapHoaDon) as NamLapHoaDon,
		hd.LoaiHoaDon,
		Case When hd.LoaiHoaDon = 1 then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as TongGiaVonBan,
		Case When hd.LoaiHoaDon = 6 then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as TongGiaVonTra
		From BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	) as a
	GROUP BY
	a.NamLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportTaiChinhYear_SoQuyBanHang]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.NamLapHoaDon,
	CAST(ROUND(SUM(ISNULL(a.ThuNhapKhac, 0)), 0) as float) as ThuNhapKhac,
	CAST(ROUND(SUM(ISNULL(a.ChiPhiKhac, 0)), 0) as float) as ChiPhiKhac,
	CAST(ROUND(SUM(ISNULL(a.PhiTraHangNhap, 0)), 0) as float) as PhiTraHangNhap,
	CAST(ROUND(SUM(ISNULL(a.KhachThanhToan, 0)), 0) as float) as KhachThanhToan
	FROM
	(
		Select 
		DATEPART(YEAR, qhd.NgayLapHoaDon) as NamLapHoaDon,
		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then ISNULL(qhdct.TienThu, 0) else 0 end as ThuNhapKhac,
		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) then ISNULL(qhdct.TienThu, 0) else 0 end as ChiPhiKhac,
		Case when (hd.LoaiHoaDon = 7 and qhd.LoaiHoaDon = 11) then ISNULL(qhdct.TienThu, 0) else 0 end as PhiTraHangNhap,
		Case when (hd.LoaiHoaDon = 1 and qhd.LoaiHoaDon = 11) then ISNULL(qhdct.TienThu, 0) else 0 end as KhachThanhToan
		From Quy_HoaDon qhd
		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
		and DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	) as a
	GROUP BY
	a.NamLapHoaDon");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportTaiChinhMonth_ChiPhiBanHang]");
            DropStoredProcedure("[dbo].[ReportTaiChinhMonth_DoanhThuBanHang]");
            DropStoredProcedure("[dbo].[ReportTaiChinhMonth_GiaVonBanHang]");
            DropStoredProcedure("[dbo].[ReportTaiChinhMonth_SoQuyBanHang]");
            DropStoredProcedure("[dbo].[ReportTaiChinhYear_ChiPhiBanHang]");
            DropStoredProcedure("[dbo].[ReportTaiChinhYear_DoanhThuBanHang]");
            DropStoredProcedure("[dbo].[ReportTaiChinhYear_GiaVonBanHang]");
            DropStoredProcedure("[dbo].[ReportTaiChinhYear_SoQuyBanHang]");
        }
    }
}
