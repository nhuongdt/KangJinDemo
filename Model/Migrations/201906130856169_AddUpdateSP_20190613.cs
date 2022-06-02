namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190613 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetNoKhachHang_byDate]", parametersAction: p => new
            {
                ID_DoiTuong = p.Guid(),
                ToDate = p.String()
            }, body: @"set nocount on
	select dt.ID, dt.MaDoiTuong, dt.TenDoiTuong,
		ISNULL(tbl.NoHienTai,0) as NoHienTai
	from DM_DoiTuong dt
	left join 
		(
			SELECT
    			td.ID_DoiTuong ,
    			SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
    			SUM(ISNULL(td.DoanhThu,0))- SUM(ISNULL(td.GiaTriTra,0)) AS TongBanTruTraHang,
    			SUM(ISNULL(td.DoanhThu,0)) AS TongMua,
    			0 AS SoLanMuaHang

    			FROM
    			(
    				SELECT 
    					bhd.ID_DoiTuong,							
    					0 AS CongNo,
    					0 AS GiaTriTra,
    					SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi
    				FROM BH_HoaDon bhd
    				WHERE bhd.ID_DoiTuong= @ID_DoiTuong
					AND (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false'
					AND bhd.NgayLapHoaDon < @ToDate
    				GROUP BY bhd.ID_DoiTuong
    				-- gia tri trả từ bán hàng
    				UNION All
    				SELECT bhd.ID_DoiTuong,						
    					0 AS CongNo,
    					SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi
    				FROM BH_HoaDon bhd
    				WHERE bhd.ID_DoiTuong= @ID_DoiTuong
					AND (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'
					AND bhd.NgayLapHoaDon < @ToDate
    				GROUP BY bhd.ID_DoiTuong
    				-- sổ quỹ
    				UNION ALL
    				SELECT 
    					qhdct.ID_DoiTuong,							
    					0 AS CongNo,
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    					0 AS TienChi
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
					Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    				WHERE qhdct.ID_DoiTuong= @ID_DoiTuong
					AND qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
					AND qhd.NgayLapHoaDon < @ToDate
					AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    				GROUP BY qhdct.ID_DoiTuong
    
    				UNION ALL
    				SELECT 
    					qhdct.ID_DoiTuong,							
    					0 AS CongNo,
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				WHERE qhdct.ID_DoiTuong= @ID_DoiTuong
					AND qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
					AND qhd.NgayLapHoaDon < @ToDate
    				GROUP BY qhdct.ID_DoiTuong	
		) AS td GROUP BY td.ID_DoiTuong
	) tbl on dt.ID= tbl.ID_DoiTuong  Where dt.ID = @ID_DoiTuong	");

            CreateStoredProcedure(name: "[dbo].[ListHangHoaTheKho]", parametersAction: p => new
            {
                ID_HangHoa = p.Guid(),
                IDChiNhanh = p.Guid()
            }, body: @"SET NOCOUNT ON;
	SELECT hd.ID as ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, CASE WHEN hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4' and hd.LoaiHoaDon = 10 THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon,
	bhct.SoLuong, bhct.ThanhTien, bhct.TienChietKhau, bhct.GiaVon / dvqd.TyLeChuyenDoi as GiaVon, bhct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi as GiaVon_NhanChuyenHang, dvqd.TyLeChuyenDoi,
	hd.YeuCau, hd.ID_CheckIn, hd.ID_DonVi, hh.QuanLyTheoLoHang, dvqd.LaDonViChuan
	FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	WHERE hd.LoaiHoaDon != 3 and hd.LoaiHoaDon != 19 and hh.ID = @ID_HangHoa and hd.ChoThanhToan = 0 and ((hd.ID_DonVi = @IDChiNhanh and (hd.YeuCau  != '2' or hd.YeuCau is null)) or (hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4') )
	ORDER BY hd.NgayLapHoaDon desc , hd.LoaiHoaDon desc, hd.MaHoaDon desc, bhct.SoThuTu desc");

            CreateStoredProcedure(name: "[dbo].[ListHangHoaTheKhoTheoLoHang]", parametersAction: p => new
            {
                ID_HangHoa = p.Guid(),
                IDChiNhanh = p.Guid(),
                ID_LoHang = p.Guid()
            }, body: @"SET NOCOUNT ON;
	SELECT hd.ID as ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, CASE WHEN hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4' and hd.LoaiHoaDon = 10 THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon,
	bhct.SoLuong, bhct.ThanhTien, bhct.TienChietKhau, bhct.GiaVon / dvqd.TyLeChuyenDoi as GiaVon, bhct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi as GiaVon_NhanChuyenHang, dvqd.TyLeChuyenDoi,
	hd.YeuCau, hd.ID_CheckIn, hd.ID_DonVi, hh.QuanLyTheoLoHang, dvqd.LaDonViChuan
	FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	WHERE bhct.ID_LoHang = @ID_LoHang and hd.LoaiHoaDon != 3 and hd.LoaiHoaDon != 19 and hh.ID = @ID_HangHoa and hd.ChoThanhToan = 0 and ((hd.ID_DonVi = @IDChiNhanh and (hd.YeuCau  != '2' or hd.YeuCau is null)) or (hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4') )
	ORDER BY hd.NgayLapHoaDon desc , hd.LoaiHoaDon desc, hd.MaHoaDon desc, bhct.SoThuTu desc");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetNoKhachHang_byDate]");
            DropStoredProcedure("[dbo].[ListHangHoaTheKho]");
            DropStoredProcedure("[dbo].[ListHangHoaTheKhoTheoLoHang]");
        }
    }
}
