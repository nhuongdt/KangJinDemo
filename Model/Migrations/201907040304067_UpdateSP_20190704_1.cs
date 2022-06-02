namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190704_1 : DbMigration
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
	SELECT SUM(hhton.TonKho) as TonKho FROM DM_HangHoa_TonKho hhton
	inner join DonViQuiDoi dvqd on hhton.ID_DonViQuyDoi = dvqd.ID 
	where dvqd.LaDonViChuan = 1 and hhton.ID_DonVi = @ID_ChiNhanh and dvqd.ID_HangHoa = @ID_HangHoa
	GROUP BY ID_DonViQuyDoi
END");
        }
        
        public override void Down()
        {
        }
    }
}
