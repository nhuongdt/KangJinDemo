namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180413 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[Report_DatHang_GiaoDichChiTiet]", parametersAction: p => new
            {
                MaHD = p.String(),
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
			a.ID_NhomHang,
    		a.MaHangHoa,
			a.TenHangHoaFull,
			a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    		a.TenDonViTinh,
			CAST(ROUND(a.SoLuongDat, 3) as float) as SoLuongDat,
			CAST(ROUND(ISNULL(a.SoLuongNhan, 0), 3) as float) as SoLuongNhan
    		FROM
    		(
    			SELECT
				hd.ID as ID_HoaDon,
				hh.ID_NhomHang,
				dvqd.MaHangHoa,
				hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    			ISNULL(hdct.SoLuong, 0) as SoLuongDat,
				(Select hdctx.SoLuong from BH_HoaDon hdx
				inner join BH_HoaDon_ChiTiet hdctx on hdx.ID = hdctx.ID_HoaDon
				where hdx.LoaiHoaDon = 1 
				and hdctx.ID_DonViQuiDoi = dvqd.ID
				and hdx.ID_HoaDon = hd.ID
				) as SoLuongNhan
    			FROM
    			BH_HoaDon_ChiTiet hdct
    			inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			left join (Select Main.id_hanghoa,
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ChoThanhToan = 0
				and hd.MaHoaDon like @MaHD
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    		) a");

            CreateStoredProcedure(name: "[dbo].[Report_DatHang_HangHoaChiTiet]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"SELECT 
    	hh.MaHoaDon,
    	hh.NgayLapHoaDon,
    	hh.TenKhachHang,
		CAST(Round(hh.SoLuongDat, 3) as float) as SoLuongDat,
		CAST(Round(hh.TongTienHang, 0) as float) as TongTienHang,
		CAST(Round(hh.GiamGiaHD, 0) as float) as GiamGiaHD,
    	CAST(Round(hh.GiaTriDat, 0) as float) as GiaTriDat
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
    			and (dvqd.MaHangHoa like @maHH)
    			and (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is null)
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
            DropStoredProcedure("[dbo].[Report_DatHang_GiaoDichChiTiet]");
            DropStoredProcedure("[dbo].[Report_DatHang_HangHoaChiTiet]");
        }
    }
}