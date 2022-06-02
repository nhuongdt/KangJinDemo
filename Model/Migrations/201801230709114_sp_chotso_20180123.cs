namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sp_chotso_20180123 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[insertChotSo_XuatNhapTon]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body:
            @"DELETE FROM ChotSo_HangHoa where ID_DonVi = @ID_ChiNhanh
 INSERT INTO ChotSo_HangHoa EXEC selectHangHoa_XuatNhapTon @timeEnd, @ID_ChiNhanh");

            CreateStoredProcedure(name: "[dbo].[insertChotSoKhachHang_CongNo]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"DELETE FROM ChotSo_KhachHang where ID_DonVi = @ID_ChiNhanh
 INSERT INTO ChotSo_KhachHang EXEC selectKhachHang_CongNo @timeEnd, @ID_ChiNhanh");

            CreateStoredProcedure(name: "[dbo].[selectHangHoa_XuatNhapTon]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT  NEWID() AS ID, @ID_ChiNhanh as ID_DonVi, a.ID as ID_HangHoa,(a.TonCuoiKy / dvqd3.TyLeChuyenDoi) as TonKho FROM 
    (
    SELECT 
    dhh.ID,
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
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
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi
    ) AS td
    GROUP BY td.ID_DonViQuiDoi
    ) 
    AS HangHoa
    --LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    GROUP BY dhh.ID
    ) a
	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
	where dvqd3.ladonvichuan = 1
    order by MaHangHoa");

            CreateStoredProcedure(name: "[dbo].[selectKhachHang_CongNo]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT 
		NEWID() AS ID,
		*, @ID_ChiNhanh as ID_DonVi
		FROM
		(
			SELECT
			td.ID_DoiTuong AS ID_KhachHang,
			SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS CongNo
			FROM
			(
			-- Doanh thu tu ban hang
			SELECT 
			bhd.ID_DoiTuong,
			NULL AS GiaTriTra,
			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
			NULL AS TienThu,
			NULL AS TienChi
			FROM BH_HoaDon bhd
			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
			AND bhd.ID_DonVi = @ID_ChiNhanh
			GROUP BY bhd.ID_DoiTuong
			-- gia tri tra tu ban hang
			UNION All
			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
			NULL AS DoanhThu,
			NULL AS TienThu,
			NULL AS TienChi
			FROM BH_HoaDon bhd
			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
			AND bhd.ID_DonVi = @ID_ChiNhanh
			GROUP BY bhd.ID_DoiTuong
			
			UNION ALL
			SELECT 
			qhdct.ID_DoiTuong AS ID_KhachHang,
			NULL AS GiaTriTra,
			NULL AS DoanhThu,
			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
			NULL AS TienChi
			FROM Quy_HoaDon qhd
			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeEnd
			AND qhd.ID_DonVi = @ID_ChiNhanh
			GROUP BY qhdct.ID_DoiTuong

			
			UNION ALL
			SELECT 
			qhdct.ID_DoiTuong AS ID_KhachHang,
			NULL AS GiaTriTra,
			NULL AS DoanhThu,
			NULL AS TienThu,
			SUM(ISNULL(qhd.TongTienThu,0)) AS TienChi
			FROM Quy_HoaDon qhd
			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeEnd
			AND qhd.ID_DonVi = @ID_ChiNhanh
			GROUP BY qhdct.ID_DoiTuong
			) AS td
		    GROUP BY td.ID_DoiTuong
		) 
		  AS HangHoa");
        }
        
        public override void Down()
        {
        }
    }
}
