namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStoreProcedure_20180323 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_NhapChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
		dvqd.ID,
		hh.ID_NhomHang,
    	dvqd.MaHangHoa,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
    	CAST(ROUND(a.SoLuong , 3) as float ) as SoLuong,
    	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
    	FROM
    	(
			SELECT
				b.ID_DonViQuiDoi as ID_DonViQuiDoi,
				SUM(ISNULL(b.SoLuong, 3)) as SoLuong,
				SUM(ISNULL(b.GiaTri, 0)) as GiaTri
			FROM
			(
				SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
				ISNULL(hdct.tienchietkhau,0) as SoLuong,
				ISNULL(hdct.ThanhTien,0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.ID_CheckIn is not null and hd.ID_CheckIn = @ID_ChiNhanh and hd.LoaiHoaDon = '10' and hd.YeuCau = '4')
    			and hd.ChoThanhToan = 0
			) b
			GROUP BY b.ID_DonViQuiDoi
    	) a
    	inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		where (dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
		and hh.LaHangHoa like  @LaHangHoa
    	ORDER BY MaHangHoa DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_NhapChuyenHangChiTiet]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"Select 
		bhhd.MaHoaDon,
		bhhd.NgayLapHoaDon,
		dvn.TenDonVi as TenDonVi,
		CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
		CAST(ROUND(ISNULL(hdct.DonGia, 0), 0) as float) as DonGia,
		CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float) as ThanhTien,
		N'Đã nhận hàng' as TrangThai
	From
	BH_HoaDon bhhd 
	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
	where hdct.ID_DonViQuiDoi = @ID_DonViQuiDoi
	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
	and (bhhd.ID_CheckIn is not null and bhhd.ID_CheckIn = @ID_ChiNhanh and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4')
	Order by TrangThai, NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TraHangNhap]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
    	dvqd.MaHangHoa,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
    	hh.ID_NhomHang,
    	CAST(ROUND(a.SoLuong , 3) as float) AS SoLuong,
    	CAST(ROUND(a.GiaTri , 0) as float ) AS GiaTri
    	FROM
    	(
    		SELECT 
    		dvqd.ID as ID_DonViQuiDoi,
    		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuong,
    		SUM(ISNULL(hdct.ThanhTien, 0)) as GiaTri
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.LoaiHoaDon = 7
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi = @ID_ChiNhanh
    		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    		and hh.LaHangHoa like @LaHangHoa
    		GROUP BY dvqd.ID
    	) a
    	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	ORDER BY GiaTri DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT
		dvqd.ID,
		hh.ID_NhomHang,
    	dvqd.MaHangHoa,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
    	CAST(ROUND(a.SoLuong , 3) as float ) as SoLuong,
    	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
    	FROM
    	(
			SELECT
				b.ID_DonViQuiDoi as ID_DonViQuiDoi,
				SUM(ISNULL(b.SoLuong, 0)) as SoLuong,
				SUM(ISNULL(b.GiaTri, 0)) as GiaTri
			FROM
			(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
				ISNULL(hdct.SoLuong,0) as SoLuong,
				ISNULL(hdct.ThanhTien, 0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.LoaiHoaDon = '10' and hd.YeuCau = '1')
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi = @ID_ChiNhanh

    			Union ALL
				SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
				ISNULL(hdct.tienchietkhau,0) as SoLuong,
				ISNULL(hdct.ThanhTien, 0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.ID_CheckIn is not null and hd.ID_CheckIn != @ID_ChiNhanh and hd.LoaiHoaDon = '10' and hd.YeuCau = '4')
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi = @ID_ChiNhanh
			) b
			GROUP BY b.ID_DonViQuiDoi
    	) a
    	inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		where (dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
		and hh.LaHangHoa like  @LaHangHoa
    	ORDER BY MaHangHoa DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatChuyenHangChiTiet]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"Select 
		bhhd.MaHoaDon,
		bhhd.NgayLapHoaDon,
		dv.TenDonVi as TenDonVi,
		CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
		CAST(ROUND(ISNULL(hdct.DonGia, 0), 0) as float) as DonGia,
		CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float) as ThanhTien,
		Case when bhhd.YeuCau = '1' then N'Đang chuyển hàng' else N'Đã chuyển hàng' end as TrangThai
	From
	BH_HoaDon bhhd 
	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
	where hdct.ID_DonViQuiDoi = @ID_DonViQuiDoi
	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
	and ((bhhd.loaihoadon = '10' and bhhd.YeuCau = '1' and bhhd.ID_DonVi = @ID_ChiNhanh) or
	(bhhd.ID_CheckIn is not null and bhhd.ID_CheckIn != @ID_ChiNhanh and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4' and bhhd.ID_DonVi = @ID_ChiNhanh))
	Order by TrangThai, NgayLapHoaDon DESC");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportHangHoa_NhapChuyenHang]");
            DropStoredProcedure("[dbo].[ReportHangHoa_NhapChuyenHangChiTiet]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TraHangNhap]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatChuyenHang]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatChuyenHangChiTiet]");
        }
    }
}