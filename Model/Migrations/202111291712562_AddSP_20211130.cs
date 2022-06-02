namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSP_20211130 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportTaiChinh_ChiPhiSuaChua]", parametersAction: p => new
            {
                Year = p.Int(),
                IdChiNhanh = p.Guid(),
                LoaiBaoCao = p.Int()
            }, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF(@LoaiBaoCao = 1)
	BEGIN
		SELECT Thang, SUM(ThanhTien) AS ChiPhi FROM
		(SELECT MONTH(hd.NgayLapHoaDon) AS Thang, hdcp.ThanhTien FROM BH_HoaDon hd
		INNER JOIN BH_HoaDon_ChiPhi hdcp ON hd.ID = hdcp.ID_HoaDon
		WHERE YEAR(hd.NgayLapHoaDon) = @Year AND hd.ID_DonVi = @IdChiNhanh) AS a
		GROUP BY Thang
	END
	ELSE IF(@LoaiBaoCao = 2)
	BEGIN
		SELECT Thang, SUM(ThanhTien) AS ChiPhi FROM
		(SELECT YEAR(hd.NgayLapHoaDon) AS Thang, hdcp.ThanhTien FROM BH_HoaDon hd
		INNER JOIN BH_HoaDon_ChiPhi hdcp ON hd.ID = hdcp.ID_HoaDon
		WHERE YEAR(hd.NgayLapHoaDon) = @Year AND hd.ID_DonVi = @IdChiNhanh) AS a
		GROUP BY Thang
	END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportTaiChinh_ChiPhiSuaChua]");
        }
    }
}
