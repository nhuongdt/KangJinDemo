namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20180527 : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[Report_DatHang_GiaoDich]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT 
    	hh.ID as ID_HoaDon,
    	hh.MaHoaDon,
    	hh.NgayLapHoaDon,
    	hh.TenKhachHang,
    		CAST(Round(hh.SoLuongDat, 3) as float) as SoLuongDat,
    		CAST(Round(hh.TongTienHang, 0) as float) as TongTienHang,
    		CAST(Round(hh.GiamGiaHD, 0) as float) as GiamGiaHD,
    	CAST(Round(hh.GiaTriDat, 0) as float) as GiaTriDat,
    	hh.ID_NhomHang
    	FROM
    	(
    		SELECT
    		hd.ID,
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    		Case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end as MaDoiTuong,
    		Case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
    		Case when dt.MaDoiTuong is null then 'khachle' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
    		Case when dt.MaDoiTuong is null then 'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
    		a.SoLuongDat,
    			a.TongTienHang,
    			a.GiamGiaHD,
    		a.GiaTriDat,
    		a.ID_NhomHang
    		FROM
    		(
    			SELECT
    			hd.ID as ID_HoaDon,
    			dvqd.ID_HangHoa,
    			hd.ID_DoiTuong,
    			hh.ID_NhomHang,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongDat,
    				SUM(ISNULL(hdct.ThanhTien, 0)) as TongTienHang,
    				SUM(ISNULL(hdct.ThanhTien, 0) * ISNULL(hd.TongGiamGia / hd.TongTienHang, 0)) as GiamGiaHD,
    			SUM(ISNULL(hdct.ThanhTien, 0) * (1- ISNULL(hd.TongGiamGia / hd.TongTienHang, 0))) as GiaTriDat
    			FROM
    			BH_HoaDon_ChiTiet hdct
    			inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
    			and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
    			and hh.LaHangHoa like @LaHangHoa
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is null)
				and hd.TongTienHang != 0
    			GROUP BY hd.MaHoaDon, hd.ID, dvqd.ID_HangHoa, hd.ID_DoiTuong, hh.ID_NhomHang
    		) a
    		left join BH_HoaDon hd on a.ID_HoaDon = hd.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    		) as hh
    		where MaDoiTuong like @maKH or TenDoiTuong_KhongDau like @maKH or TenDoiTuong_ChuCaiDau like @maKH
    		ORDER BY hh.NgayLapHoaDon DESC");
        }
        
        public override void Down()
        {
        }
    }
}