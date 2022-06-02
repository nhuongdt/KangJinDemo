namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190701_2 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[TinhSLTon]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON;
	SELECT
    ISNULL(SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)),0) AS TonKho
    FROM
    (
    SELECT 
    dvqd.ID_HangHoa,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false' and hh.LaHangHoa =1 AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	And dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa                                                                                                                                                                                                                                                             
    
    UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND dvqd.ID_HangHoa = @ID_HangHoa
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY dvqd.ID_HangHoa
    
    UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa

	UNION ALL
	SELECT 
    dvqd.ID_HangHoa,
	SUM(ISNULL(bhdct.ThanhTien,0)* dvqd.TyLeChuyenDoi) - MAX(ISNULL(bhdct.TienChietKhau,0) * dvqd.TyLeChuyenDoi)  AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_HoaDon 
    
    UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgaySua < @timeEnd
    	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa
    ) AS td
END");
        }
        
        public override void Down()
        {
        }
    }
}
