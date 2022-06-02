namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20200903_01 : DbMigration
    {
        public override void Up()
        {
            Sql(@"DROP PROCEDURE IF EXISTS GetMaPhieuThuChi_whenUpdateHD;");
            CreateStoredProcedure(name: "[dbo].[GetMaPhieuThuChi_whenUpdateHD]", parametersAction: p => new
            {
                MaPhieuThuChiGoc = p.String(50)
            }, body: @"SET NOCOUNT ON;

    declare @count int = (select count(id) from Quy_HoaDon where CHARINDEX(MaHoaDon, @MaPhieuThuChiGoc) > 0)
	if @count = 0
		select @MaPhieuThuChiGoc as MaxCode
	else 
		 select CONCAT(@MaPhieuThuChiGoc,'_',@count) as MaxCode");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetMaPhieuThuChi_whenUpdateHD]");
        }
    }
}
