namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSPReportNhanVien_20180213 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportNhanVien_BanHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT 
	a.ID_NhanVien,
	nv.TenNhanVien, 
	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)), 0) as float) as DoanhThu,
	CAST(ROUND(SUM(ISNULL(a.GiaTriTra, 0)) * (-1), 0) as float) as GiaTriTra,
	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.GiaTriTra, 0)), 0) as float) as DoanhThuThuan
	FROM
	(
		SELECT 
			nv.ID as ID_NhanVien,
			Sum(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
			NULL as GiaTriTra
		FROM
		NS_NhanVien nv
		join BH_HoaDon hd on nv.ID = hd.ID_NhanVien
		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi = @ID_ChiNhanh
		and hd.LoaiHoaDon = 1
		GROUP BY nv.ID
		UNION ALL
		SELECT
			nv.ID as ID_NhanVien,
			NULL as DoanhThu,
			Sum((ISNULL(hdct.ThanhTien, 0)- ISNULL(hdct.TienChietKhau, 0)) * (1 - (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang,0)))) as GiaTriTra
		FROM
		NS_NhanVien nv
		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
		and hdb.ChoThanhToan = 0
		and hdb.ID_DonVi = @ID_ChiNhanh
		and hdb.LoaiHoaDon = 1
		GROUP BY nv.ID
	) a
	left join NS_NhanVien nv on a.ID_NhanVien = nv.ID
	GROUP BY a.ID_NhanVien, nv.TenNhanVien");

            CreateStoredProcedure(name: "[dbo].[ReportNhanVien_LoiNhuan]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT 
	a.ID_NhanVien,
	nv.TenNhanVien, 
	CAST(ROUND(SUM(ISNULL(a.TongTienHang, 0)), 0) as float) as TongTienHang,
	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.TongTienHang, 0)), 0) as float) as GiamGiaHD,
	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)), 0) as float) as DoanhThu,
	CAST(ROUND(SUM(ISNULL(a.GiaTriTra, 0)) * (-1), 0) as float) as GiaTriTra,
	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.GiaTriTra, 0)), 0) as float) as DoanhThuThuan,
	CAST(ROUND(SUM(ISNULL(a.TongGiaVonBan, 0)) - SUM(ISNULL(a.TongGiaVonTra, 0)), 0) as float) as TongGiaVon, 
	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.GiaTriTra, 0)) - SUM(ISNULL(a.TongGiaVonBan, 0)) + SUM(ISNULL(a.TongGiaVonTra, 0)), 0) as float) as LoiNhuanGop
	FROM
	(
		SELECT 
			nv.ID as ID_NhanVien,
			SUM(ISNULL(hd.TongTienHang, 0))  as TongTienHang,
			NULL as GiamGiaHD,
			SUM(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
			NULL as GiaTriTra,
			NULL as TongGiaVonBan,
			NULL as TongGiaVonTra
		FROM
		NS_NhanVien nv
		join BH_HoaDon hd on nv.ID = hd.ID_NhanVien
		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and hd.ChoThanhToan = 0
		and hd.ID_DonVi = @ID_ChiNhanh
		and hd.LoaiHoaDon = 1
		GROUP BY nv.ID

		UNION ALL
		SELECT
			nv.ID as ID_NhanVien,
			NULL as TongTienHang,
			NULL as GiamGiaHD,
			NULL as DoanhThu,
			NULL as GiaTriTra,
			SUM(ISNULL(hdct.SoLuong, 0) * (ISNULL(hdct.GiaVon, 0))) as TongGiaVonBan,
			NULL as TongGiaVonTra
		FROM
		NS_NhanVien nv
		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon 
		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
		and hdb.ChoThanhToan = 0
		and hdb.ID_DonVi = @ID_ChiNhanh
		and hdb.LoaiHoaDon = 1
		GROUP BY nv.ID

		UNION ALL
		SELECT
			nv.ID as ID_NhanVien,
			NULL as TongTienHang,
			NULL as GiamGiaHD,
			NULL as DoanhThu,
			Sum((ISNULL(hdct.ThanhTien, 0)- ISNULL(hdct.TienChietKhau, 0)) * (1 - (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang,0)))) as GiaTriTra,
			NULL as TongGiaVonBan,
			SUM(ISNULL(hdct.SoLuong, 0) * (ISNULL(hdct.GiaVon, 0))) as TongGiaVonTra
		FROM
		NS_NhanVien nv
		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
		and hdb.ChoThanhToan = 0
		and hdb.ID_DonVi = @ID_ChiNhanh
		and hdb.LoaiHoaDon = 1
		GROUP BY nv.ID
	) a
	left join NS_NhanVien nv on a.ID_NhanVien = nv.ID
	GROUP BY a.ID_NhanVien, nv.TenNhanVien");

            CreateStoredProcedure(name: "[dbo].[ReportNhanVien_MuaHangChiTiet]", parametersAction: p => new
            {
                ID_NV = p.String(),
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
		hh.ID_NhomHang,
    	dvqd.MaHangHoa,
    	case when dvqd.TenDonViTinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
    	CAST(ROUND(a.SoLuongSanPham,3) as float) as SoLuong,
    	CAST(ROUND(a.GiaTri, 0) as float) as GiaTri
    	FROM
    		(
    		SELECT 
    		dvqd.ID,
    		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
    		SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.ID_DonVi = @ID_ChiNhanh
    		and hd.ID_NhanVien = @ID_NV
    		and hd.ChoThanhToan = 0
    		and hd.loaihoadon = 1
			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
			and hh.LaHangHoa like @LaHangHoa
    		GROUP BY dvqd.ID
    	) a
    	left join DonViQuiDoi dvqd on a.ID = dvqd.ID
    	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	ORDER BY GiaTri DESC");

            CreateStoredProcedure(name: "[dbo].[AddChiTietGia]", parametersAction: p => new
            {
                ListID_NhomHang = p.String(),
                ID_GiaBan = p.Guid(),
                ID_KhoHang = p.Guid(),
                ID_NgoaiTe = p.Guid()
            }, body: @"if(@ListID_NhomHang != '%%')
	BEGIN
	select ID_DonViQuiDoi  into #tableIDDVQD  from DM_GiaBan_ChiTiet where ID_GiaBan = @ID_GiaBan 
	INSERT INTO DM_GiaBan_ChiTiet(ID, GiaBan, ID_GiaBan,ID_KhoHang, ID_NgoaiTe,ID_DonViQuiDoi)
	SELECT NEWID() as ID, dvqd.GiaBan, @ID_GiaBan as ID_GiaBan,@ID_KhoHang as ID_KhoHang,@ID_NgoaiTe as ID_NgoaiTe, dvqd.ID as ID_DonViQuiDoi
		 from DonViQuiDoi dvqd
    	 LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		 WHERE hh.ID_NhomHang=(select * from splitstring(@ListID_NhomHang) where [name] like hh.ID_NhomHang) and dvqd.ID not in (select ID_DonViQuiDoi from #tableIDDVQD)
	END
	ELSE
	BEGIN
		select ID_DonViQuiDoi  into #tableIDDVQD1  from DM_GiaBan_ChiTiet where ID_GiaBan = @ID_GiaBan 
		INSERT INTO DM_GiaBan_ChiTiet(ID, GiaBan, ID_GiaBan,ID_KhoHang, ID_NgoaiTe,ID_DonViQuiDoi)
		SELECT NEWID() as ID, dvqd.GiaBan, @ID_GiaBan as ID_GiaBan,@ID_KhoHang as ID_KhoHang,@ID_NgoaiTe as ID_NgoaiTe, dvqd.ID as ID_DonViQuiDoi
		from DonViQuiDoi dvqd
    	 LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		 WHERE dvqd.ID not in (select ID_DonViQuiDoi from #tableIDDVQD1)
	END");

            CreateStoredProcedure(name: "[dbo].[XoaBangGia]", parametersAction: p => new
            {
                ID_GiaBan = p.Guid()
            }, body: @"delete from DM_GiaBan_ChiTiet where ID_GiaBan = @ID_GiaBan
	delete from DM_GiaBan where ID = @ID_GiaBan");

            CreateStoredProcedure(name: "[dbo].[XoaChiTietbangGia]", parametersAction: p => new
            {
                ID_GiaBan = p.Guid()
            }, body: @"delete from DM_GiaBan_ChiTiet where ID_GiaBan = @ID_GiaBan");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportNhanVien_BanHang]");
            DropStoredProcedure("[dbo].[ReportNhanVien_LoiNhuan]");
            DropStoredProcedure("[dbo].[ReportNhanVien_MuaHangChiTiet]");
            DropStoredProcedure("[dbo].[AddChiTietGia]");
            DropStoredProcedure("[dbo].[XoaBangGia]");
            DropStoredProcedure("[dbo].[XoaChiTietbangGia]");
        }
    }
}
