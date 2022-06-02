namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateStoreProcedure20180209 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[Report_BanHang]", parametersAction: p => new
            {
                MaHD = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT 
		a.MaHoaDon,
		a.NgayLapHoaDon,
		case when a.TenDonViTinh = '' then a.TenHangHoa else a.TenHangHoa + ' (' + a.TenDonViTinh + ')' end as TenHangHoa,
		CAST(ROUND((a.SoLuong), 3) as float) as SoLuong, 
		CAST(ROUND((a.GiaBan), 0) as float) as GiaBan,
		CAST(ROUND((a.ThanhTien), 0) as float) as ThanhTien,
		CAST(ROUND((a.GiaVon), 0) as float) as GiaVon,
		CAST(ROUND((a.SoLuong * a.GiaVon), 0) as float) as TienVon,
		CAST(ROUND((a.ThanhTien - (a.SoLuong * a.GiaVon)), 0) as float) as LaiLo, 
		a.TenNhanVien,
		a.TenDonViTinh,
		a.ID_NhomHang  
	FROM
	(
		Select hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hh.TenHangHoa,
		dvqd.TenDonViTinh,
		ISNULL(hdct.SoLuong, 0) as SoLuong,
		ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0) as GiaBan,
		ISNULL(hdct.GiaVon, 0) as GiaVon, 
		nv.TenNhanVien,
		hh.ID_NhomHang,
		hdct.ThanhTien
		FROM BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) and hd.ID_DonVi = @ID_ChiNhanh 
		and hd.ChoThanhToan = 0 
		and hd.LoaiHoaDon = 1
		and hh.LaHangHoa like @LaHangHoa
		and hd.MaHoaDon like @maHD
	) a
	order by a.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[Report_DatHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
	dvqd.ID as ID_DonViQuiDoi,
	dvqd.MaHangHoa,
	case when dvqd.TenDonViTinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
	dvqd.TenDonViTinh,
	a.SoLuongDat,
	a.GiaTriDat,
	hh.ID_NhomHang,
	a.ID_NhanVien
	FROM
	(
		SELECT
		dvqd.ID as ID_DonViQuiDoi,
		dt.ID as ID_DoiTuong,
		hd.ID_NhanVien as ID_NhanVien,
		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongDat,
		SUM(ISNULL(hdct.ThanhTien, 0) * (1- ISNULL(hd.TongGiamGia / hd.TongTienHang, 0))) as GiaTriDat
		FROM
		BH_HoaDon_ChiTiet hdct
		inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi = @ID_ChiNhanh
		and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
		and hh.LaHangHoa like @LaHangHoa
		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
		and (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH)
		GROUP BY dvqd.ID, dt.ID, hd.ID_NhanVien
	) a
    left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join DM_DoiTuong dt on a.ID_DoiTuong = dt.ID");

            CreateStoredProcedure(name: "[dbo].[ReportHangBan_NhanVien]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
		nv.ID as ID_NhanVien,
		nv.TenNhanVien,
		SUM(ISNULL(hdct.SoLuong * dvqd.TyLeChuyenDoi, 0)) as SoLuong,
		SUM(ISNULL(hd.TongTienHang, 0)) as GiaTri,
		hh.ID_NhomHang
		FROM
		BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		inner join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi = @ID_ChiNhanh
		and hd.LoaiHoaDon = 1
		and hh.LaHangHoa like @LaHangHoa
		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
		GROUP BY dvqd.ID, nv.ID, hh.ID_NhomHang, nv.TenNhanVien");

            CreateStoredProcedure(name: "[dbo].[ReportNCC_MuaHang]", parametersAction: p => new
            {
                MaNCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT * FROM
	(
		SELECT
			a.ID_NCC,
			case when dt.MaDoiTuong is null then N'NCC lẻ' else dt.MaDoiTuong end AS MaNCC,
			case when dt.TenDoiTuong is null then N'Nhà cung cấp lẻ' else dt.TenDoiTuong end AS TenNCC,
			case when dt.TenDoiTuong_KhongDau is null then N'nha cung cap le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
			case when dt.TenDoiTuong_ChuCaiDau is null then N'nccl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
			CAST(ROUND(a.SoLuongSanPham, 3) as float) as SoLuongSanPham, 
			CAST(ROUND(a.GiaTri, 0) as float) as GiaTri
		FROM
		(
			SELECT
				NCC.ID_NCC,
				SUM(ISNULL(NCC.SoLuongSanPham, 0)) as SoLuongSanPham,
				SUM(ISNULL(NCC.GiaTri, 0)) as GiaTri
			FROM
			(
				SELECT
				hd.ID_DoiTuong as ID_NCC,
				NULL AS SoLuongSanPham,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTri
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 4
				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_NCC,
				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongSanPham,
				NULL AS GiaTri
				FROM
				BH_HoaDon hd 
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 4
				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
			) AS NCC
			Group by NCC.ID_NCC
		) a
		left join DM_DoiTuong dt on a.ID_NCC = dt.ID
	) b
	where MaNCC like @MaNCC or TenDoiTuong_KhongDau like @MaNCC or TenDoiTuong_ChuCaiDau like @MaNCC
	ORDER BY GiaTri DESC");

            CreateStoredProcedure(name: "[dbo].[ReportNCC_MuaHangChiTiet]", parametersAction: p => new
            {
                ID_NCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"IF (@ID_NCC != '')
	BEGIN
		SELECT
		dvqd.MaHangHoa,
		case when dvqd.TenDonViTinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
		CAST(ROUND(a.SoLuongSanPham,3) as float) as SoLuongSanPham,
		CAST(ROUND(a.GiaTri, 0) as float) as GiaTri
		FROM
		 (
			SELECT 
			dvqd.ID,
			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
			SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ID_DonVi = @ID_ChiNhanh
			and hd.ID_DoiTuong = @ID_NCC
			and hd.ChoThanhToan = 0
			and hd.loaihoadon = 4
			GROUP BY dvqd.ID
		) a
		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		ORDER BY GiaTri DESC
	END
	ELSE
	BEGIN
		SELECT
		dvqd.MaHangHoa,
		case when dvqd.TenDonViTinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
		CAST(ROUND(a.SoLuongSanPham,3) as float) as SoLuongSanPham,
		CAST(ROUND(a.GiaTri, 0) as float) as GiaTri
		FROM
		 (
			SELECT 
			dvqd.ID,
			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
			SUM(ISNULL(hdct.ThanhTien * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang,  0))), 0)) as GiaTri
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ID_DonVi = @ID_ChiNhanh
			and hd.ID_DoiTuong is null
			and hd.ChoThanhToan = 0
			and hd.loaihoadon = 4
			GROUP BY dvqd.ID
		) a
		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		ORDER BY GiaTri DESC
	END");

            CreateStoredProcedure(name: "[dbo].[ReportNCC_NhapHang]", parametersAction: p => new
            {
                MaNCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT * FROM
	(
		SELECT
			a.ID_NCC,
			case when dt.MaDoiTuong is null then N'NCC lẻ' else dt.MaDoiTuong end AS MaNCC,
			case when dt.TenDoiTuong is null then N'Nhà cung cấp lẻ' else dt.TenDoiTuong end AS TenNCC,
			case when dt.TenDoiTuong_KhongDau is null then N'nha cung cap le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
			case when dt.TenDoiTuong_ChuCaiDau is null then N'nccl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
			CAST(ROUND((a.GiaTriNhap), 0) as float) as GiaTriNhap, 
			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
			CAST(ROUND((a.GiaTriNhap - a.GiaTriTra), 0) as float) as GiaTriThuan
		FROM
		(
			SELECT
				NCC.ID_NCC,
				SUM(ISNULL(NCC.GiaTriNhap, 0)) as GiaTriNhap,
				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra
			FROM
			(
				SELECT
				hd.ID_DoiTuong as ID_NCC,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriNhap,
				NULL AS GiaTriTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 4
				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_NCC,
				NULL AS GiaTriNhap,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 7
				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
			) AS NCC
			Group by NCC.ID_NCC
		) a
		left join DM_DoiTuong dt on a.ID_NCC = dt.ID
	) b
	where MaNCC like @MaNCC or TenDoiTuong_KhongDau like @MaNCC or TenDoiTuong_ChuCaiDau like @MaNCC
	ORDER BY GiaTriThuan DESC");

            CreateStoredProcedure(name: "[dbo].[ReportNCC_NhapHangChiTiet]", parametersAction: p => new
            {
                ID_NCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"IF (@ID_NCC != '')
	BEGIN
		SELECT
		hd.MaHoaDon as MaPhieu,
		hd.NgayLapHoaDon,
		a.SoLuongSanPham,
		Case when hd.LoaiHoaDon = 4 then ISNULL(hd.PhaiThanhToan, 0) else ISNULL(hd.PhaiThanhToan, 0) * (-1) end as TongGiaTri
		FROM
		 (
			SELECT 
			hd.ID,
			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ID_DonVi = @ID_ChiNhanh
			and hd.ID_DoiTuong = @ID_NCC
			and hd.ChoThanhToan = 0
			and (hd.loaihoadon = 4 or hd.loaihoadon = 7)
			GROUP BY hd.ID
		) a
		left join BH_HoaDon hd on a.ID = hd.ID
		ORDER BY hd.NgayLapHoaDon DESC
	END
	ELSE
	BEGIN
		SELECT
	hd.MaHoaDon as MaPhieu,
	hd.NgayLapHoaDon,
	a.SoLuongSanPham,
	Case when hd.LoaiHoaDon = 4 then ISNULL(hd.PhaiThanhToan, 0) else ISNULL(hd.PhaiThanhToan, 0) * (-1) end as TongGiaTri
	FROM
	 (
		SELECT 
		hd.ID,
		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham
		FROM
		BH_HoaDon hd
		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and hd.ID_DonVi = @ID_ChiNhanh
		and hd.ID_DoiTuong is null
		and hd.ChoThanhToan = 0
		and (hd.loaihoadon = 4 or hd.loaihoadon = 7)
		GROUP BY hd.ID
	) a
	left join BH_HoaDon hd on a.ID = hd.ID
	ORDER BY hd.NgayLapHoaDon DESC
	END");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_BanHang]", parametersAction: p => new
            {
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT * FROM
	(
		SELECT
			a.ID_KhachHang,
			case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end AS MaKhachHang,
			case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end AS TenKhachHang,
			case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
			case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
			CAST(ROUND((a.DoanhThu), 0) as float) as DoanhThu, 
			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
			CAST(ROUND((a.DoanhThu - a.GiaTriTra), 0) as float) as DoanhThuThuan
		FROM
		(
			SELECT
				NCC.ID_KhachHang,
				SUM(ISNULL(NCC.DoanhThu, 0)) as DoanhThu,
				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra
			FROM
			(
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
				NULL AS GiaTriTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 1
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				NULL AS DoanhThu,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 6
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
			) AS NCC
			Group by NCC.ID_KhachHang
		) a
		left join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
	) b
	where MaKhachHang like @MaKH or TenDoiTuong_KhongDau like @MaKH or TenDoiTuong_ChuCaiDau like @MaKH
	ORDER BY DoanhThuThuan DESC");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_BanHangChiTiet]", parametersAction: p => new
            {
                ID_KhachHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"IF (@ID_KhachHang != '')
	BEGIN
		SELECT
		hd.MaHoaDon,
		hd.NgayLapHoaDon as ThoiGian,
		Case when hd.LoaiHoaDon = 1 then ISNULL(hd.PhaiThanhToan, 0) else ISNULL(hd.PhaiThanhToan, 0) * (-1) end as DoanhThu
		FROM
			BH_HoaDon hd
			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ID_DonVi = @ID_ChiNhanh
			and hd.ID_DoiTuong = @ID_KhachHang
			and hd.ChoThanhToan = 0
			and (hd.loaihoadon = 1 or hd.loaihoadon = 6)
		ORDER BY hd.NgayLapHoaDon DESC
	END
	ELSE
	BEGIN
		SELECT
		hd.MaHoaDon,
		hd.NgayLapHoaDon as ThoiGian,
		Case when hd.LoaiHoaDon = 1 then ISNULL(hd.PhaiThanhToan, 0) else ISNULL(hd.PhaiThanhToan, 0) * (-1) end as DoanhThu
		FROM
			BH_HoaDon hd
			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ID_DonVi = @ID_ChiNhanh
			and hd.ID_DoiTuong is null
			and hd.ChoThanhToan = 0
			and (hd.loaihoadon = 1 or hd.loaihoadon = 6)
		ORDER BY hd.NgayLapHoaDon DESC
	END");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_LoiNhuan]", parametersAction: p => new
            {
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT * FROM
	(
		SELECT
			a.ID_KhachHang,
			case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end AS MaKhachHang,
			case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end AS TenKhachHang,
			case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
			case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
			CAST(ROUND((a.TongTienHang), 0) as float) as TongTienHang,
			CAST(ROUND((a.DoanhThu - a.TongTienHang), 0) as float) as GiamGiaHD,
			CAST(ROUND((a.DoanhThu), 0) as float) as DoanhThu, 
			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
			CAST(ROUND((a.DoanhThu - a.GiaTriTra), 0) as float) as GiaTriThuan,
			CAST(ROUND((a.TongGiaVonBan - a.TongGiaVonTra), 0) as float) as TongGiaVon,
			CAST(ROUND((a.DoanhThu - a.GiaTriTra - a.TongGiaVonBan + a.TongGiaVonTra), 0) as float) as LoiNhuanGop
		FROM
		(
			SELECT
				NCC.ID_KhachHang,
				SUM(ISNULL(NCC.DoanhThu, 0)) as DoanhThu,
				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra,
				SUM(ISNULL(NCC.TongTienHang, 0)) as TongTienHang,
				SUM(ISNULL(NCC.TongGiaVonBan, 0)) as TongGiaVonBan,
				SUM(ISNULL(NCC.TongGiaVonTra, 0)) as TongGiaVonTra
			FROM
			(
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
				NULL AS GiaTriTra,
				SUM(ISNULL(hd.TongTienHang, 0)) as TongTienHang, 
				NULL AS TongGiaVonBan,
				NULL AS TongGiaVonTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 1
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				NULL AS DoanhThu,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra,
				NULL as TongTienHang, 
				NULL AS TongGiaVonBan,
				NULL AS TongGiaVonTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 6
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				NULL AS DoanhThu,
				NULL as GiaTriTra,
				NULL as TongTienHang, 
				SUM(ISNULL(hdct.SoLuong * hdct.GiaVon, 0)) AS TongGiaVonBan,
				NULL AS TongGiaVonTra
				FROM
				BH_HoaDon hd 
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 1
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong

				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				NULL AS DoanhThu,
				NULL as GiaTriTra,
				NULL as TongTienHang, 
				NULL AS TongGiaVonBan,
				SUM(ISNULL(hdct.SoLuong * hdct.GiaVon, 0)) AS TongGiaVonTra
				FROM
				BH_HoaDon hd 
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 6
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong 
			) AS NCC
			Group by NCC.ID_KhachHang
		) a
		left join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
	) b
	where MaKhachHang like @MaKH or TenDoiTuong_KhongDau like @MaKH or TenDoiTuong_ChuCaiDau like @MaKH
	ORDER BY LoiNhuanGop DESC");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_MuaHang]", parametersAction: p => new
            {
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT * FROM
	(
		SELECT
			a.ID_KhachHang,
			case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end AS MaKhachHang,
			case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end AS TenKhachHang,
			case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
			case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
			CAST(ROUND((a.SoLuongMua), 0) as float) as SoLuongMua, 
			CAST(ROUND((a.GiaTriMua), 0) as float) as GiaTriMua,
			CAST(ROUND((a.SoLuongTra), 0) as float) as SoLuongTra, 
			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
			CAST(ROUND((a.GiaTriMua - a.GiaTriTra), 0) as float) as GiaTriThuan
		FROM
		(
			SELECT
				NCC.ID_KhachHang,
				SUM(ISNULL(NCC.GiaTriMua, 0)) as GiaTriMua,
				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra,
				SUM(ISNULL(NCC.SoLuongMua, 0)) as SoLuongMua,
				SUM(ISNULL(NCC.SoLuongTra, 0)) as SoLuongTra
			FROM
			(
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriMua,
				NULL AS GiaTriTra,
				NULL AS SoLuongMua,
				NULL AS SoLuongTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 1
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				NULL AS GiaTriMua,
				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra,
				NULL AS SoLuongMua,
				NULL AS SoLuongTra
				FROM
				BH_HoaDon hd 
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 6
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong
				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				NULL AS GiaTriMua,
				NULL as GiaTriTra,
				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongMua,
				NULL AS SoLuongTra
				FROM
				BH_HoaDon hd 
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 1
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong

				UNION ALL
				SELECT
				hd.ID_DoiTuong as ID_KhachHang,
				NULL AS GiaTriMua,
				NULL as GiaTriTra,
				NULL AS SoLuongMua,
				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongTra
				FROM
				BH_HoaDon hd 
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ChoThanhToan = 0
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.LoaiHoaDon = 6
				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.MaDoiTuong is NULL)
				GROUP BY hd.ID_DoiTuong 
			) AS NCC
			Group by NCC.ID_KhachHang
		) a
		left join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
	) b
	where MaKhachHang like @MaKH or TenDoiTuong_KhongDau like @MaKH or TenDoiTuong_ChuCaiDau like @MaKH
	ORDER BY GiaTriThuan DESC");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_MuaHangChiTiet]", parametersAction: p => new
            {
                ID_KhachHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"IF (@ID_KhachHang != '')
	BEGIN
		SELECT
		dvqd.MaHangHoa,
		case when dvqd.TenDonViTinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
		CAST(ROUND(a.SoLuongMua,3) as float) as SoLuongMua,
		CAST(ROUND(a.GiaTriMua, 0) as float) as GiaTriMua,
		CAST(ROUND(a.SoLuongTra,3) as float) as SoLuongTra,
		CAST(ROUND(a.GiaTriTra * (-1), 0) as float) as GiaTriTra,
		CAST(ROUND(a.GiaTriMua - a.GiaTriTra, 0) as float) as GiaTriThuan
		FROM
		 (
			SELECT 
			KH.ID,
			SUM(ISNULL(KH.SoLuongMua, 0)) as SoLuongMua,
			SUM(ISNULL(KH.SoLuongTra, 0)) as SoLuongTra,
			SUM(ISNULL(KH.GiaTriMua, 0)) as GiaTriMua,
			SUM(ISNULL(KH.GiaTriTra, 0)) as GiaTriTra
			FROM
			(
				SELECT 
				dvqd.ID,
				SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongMua,
				NULL AS SoLuongTra,
				SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.SoLuong * hdct.TienChietKhau, 0)) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriMua,
				NULL As GiaTriTra
				FROM
				BH_HoaDon hd
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.ID_DoiTuong = @ID_KhachHang
				and hd.ChoThanhToan = 0
				and hd.loaihoadon = 1 
				GROUP BY dvqd.ID
				UNION ALL
				SELECT 
				dvqd.ID,
				NULL as SoLuongMua,
				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongTra,
				NULL As GiaTriMua,
				SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.SoLuong * hdct.TienChietKhau, 0)) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriTra
				FROM
				BH_HoaDon hd
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.ID_DoiTuong = @ID_KhachHang
				and hd.ChoThanhToan = 0
				and hd.loaihoadon = 6
				GROUP BY dvqd.ID
			) as KH
			GROUP BY KH.ID
		) a
		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		ORDER BY GiaTriThuan DESC
	END
	ELSE
	BEGIN
		SELECT
		dvqd.MaHangHoa,
		case when dvqd.TenDonViTinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
		CAST(ROUND(a.SoLuongMua,3) as float) as SoLuongMua,
		CAST(ROUND(a.GiaTriMua, 0) as float) as GiaTriMua,
		CAST(ROUND(a.SoLuongTra,3) as float) as SoLuongTra,
		CAST(ROUND(a.GiaTriTra * (-1), 0) as float) as GiaTriTra,
		CAST(ROUND(a.GiaTriMua - a.GiaTriTra, 0) as float) as GiaTriThuan
		FROM
		 (
			SELECT 
			KH.ID,
			SUM(ISNULL(KH.SoLuongMua, 0)) as SoLuongMua,
			SUM(ISNULL(KH.SoLuongTra, 0)) as SoLuongTra,
			SUM(ISNULL(KH.GiaTriMua, 0)) as GiaTriMua,
			SUM(ISNULL(KH.GiaTriTra, 0)) as GiaTriTra
			FROM
			(
				SELECT 
				dvqd.ID,
				SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongMua,
				NULL AS SoLuongTra,
				SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriMua,
				NULL As GiaTriTra
				--SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
				FROM
				BH_HoaDon hd
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.ID_DoiTuong is null
				and hd.ChoThanhToan = 0
				and hd.loaihoadon = 1 
				GROUP BY dvqd.ID
				UNION ALL
				SELECT 
				dvqd.ID,
				NULL as SoLuongMua,
				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongTra,
				NULL As GiaTriMua,
				SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriTra
				--SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
				FROM
				BH_HoaDon hd
				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
				and hd.ID_DonVi = @ID_ChiNhanh
				and hd.ID_DoiTuong is null
				and hd.ChoThanhToan = 0
				and hd.loaihoadon = 6
				GROUP BY dvqd.ID
			) as KH
			GROUP BY KH.ID
		) a
		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		ORDER BY GiaTriThuan DESC
	END");

            CreateStoredProcedure(name: "[dbo].[LoadHangHoaCungLoai]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_CungLoai = p.Guid()
            }, body: @"DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.LaHangHoa, aa.LaChaCungLoai, aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho FROM (
    (select dvqd.ID,hh.ID as ID_HangHoa,hh.ID_HangHoaCungLoai,hh.LaHangHoa,hh.LaChaCungLoai, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DM_HangHoa hh
    left join DonViQuiDoi dvqd on hh.ID = dvqd.ID_hangHoa
    left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    where dvqd.xoa is null and dvqd.ladonvichuan = '1' and hh.ID_HangHoaCungLoai = @ID_CungLoai) aa
    left join
    (
    SELECT  dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    (
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
    SUM(ISNULL(HangHoa.TonDau,0)) AS TonCuoiKy
    FROM
    (
    SELECT
    td.ID_DonViQuiDoi,
    SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    NULL AS SoLuongNhap,
    NULL AS SoLuongXuat
    FROM
    (
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi 
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	)bb on aa.ID = bb.ID_DonViQuiDoi)");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[Report_BanHang]");
            DropStoredProcedure("[dbo].[Report_DatHang]");
            DropStoredProcedure("[dbo].[ReportHangBan_NhanVien]");
            DropStoredProcedure("[dbo].[ReportNCC_MuaHang]");
            DropStoredProcedure("[dbo].[ReportNCC_MuaHangChiTiet]");    
            DropStoredProcedure("[dbo].[ReportNCC_NhapHang]");
            DropStoredProcedure("[dbo].[ReportNCC_NhapHangChiTiet]");
            DropStoredProcedure("[dbo].[ReportKhachHang_BanHang]");
            DropStoredProcedure("[dbo].[ReportKhachHang_BanHangChiTiet]");
            DropStoredProcedure("[dbo].[ReportKhachHang_LoiNhuan]");
            DropStoredProcedure("[dbo].[ReportKhachHang_MuaHang]");
            DropStoredProcedure("[dbo].[ReportKhachHang_MuaHangChiTiet]");
            Sql("DROP FUNCTION [dbo].[splitstring]");
            DropStoredProcedure("[dbo].[LoadHangHoaCungLoai]");
        }
    }
}
