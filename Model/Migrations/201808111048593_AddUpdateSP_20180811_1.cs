namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180811_1 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetListTonTheoLoHangHoa]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                ID_HangHoa = p.Guid()
            }, body: @"SELECT
    td.ID_LoHang,td.MaLoHang, td.NgaySanXuat, td.NgayHetHan,
    ROUND(SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)),2) AS TonKho
    FROM
    (
    SELECT 
    bhdct.ID_LoHang,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM DM_LoHang dmlo
	Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false' and hh.LaHangHoa =1
	AND bhd.NgayLapHoaDon < @timeEnd
	AND dvqd.ID_HangHoa = @ID_HangHoa
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_LoHang,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan                                                                                                                                                                                                                                                             
    
    UNION ALL
    SELECT 
    bhdct.ID_LoHang,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
     FROM DM_LoHang dmlo
	Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
	AND dvqd.ID_HangHoa = @ID_HangHoa
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_LoHang,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan
    
    UNION ALL
    SELECT 
    bhdct.ID_LoHang,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
     FROM DM_LoHang dmlo
	Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
	AND dvqd.ID_HangHoa = @ID_HangHoa
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_LoHang,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan
    
    UNION ALL
    SELECT 
    bhdct.ID_LoHang,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM DM_LoHang dmlo
	Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon < @timeEnd
	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY bhdct.ID_LoHang,bhdct.ID_DonViQuiDoi,dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan
    ) AS td
    GROUP BY td.ID_LoHang,td.MaLoHang, td.NgaySanXuat, td.NgayHetHan");
        }
        
        public override void Down()
        {
        }
    }
}


/*
 * Create:
 * [dbo].[GetListTonTheoLoHangHoa]
 */
