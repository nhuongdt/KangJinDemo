namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20200903_04 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[GetMaPhieuThuChi_whenUpdateHD]
    @MaPhieuThuChiGoc [nvarchar](50)
AS
BEGIN
    SET NOCOUNT ON;
    
    declare @count int = (select count(id) from Quy_HoaDon where CHARINDEX(@MaPhieuThuChiGoc, MaHoaDon) > 0 and MaHoaDon like '%'+ @MaPhieuThuChiGoc +'%')
    	if @count = 0
    		select @MaPhieuThuChiGoc as MaxCode
    	else 
    		 select CONCAT(@MaPhieuThuChiGoc,'_',@count + 1) as MaxCode
END");
        }
        
        public override void Down()
        {
        }
    }
}
