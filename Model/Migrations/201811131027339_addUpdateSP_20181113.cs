namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20181113 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SP_DeleteHoaDon_whenTimeout]", parametersAction: p => new
            {
                NgayLapHoaDon = p.String()
            }, body: @"declare @ID varchar(40)

	select @ID = ( Select top 1 ID from BH_HoaDon
	where convert(varchar, NgayLapHoaDon, 120) = @NgayLapHoaDon
	order by NgayLapHoaDon desc)

	IF @ID IS NOT NULL 
    		--Update BH_HoaDon Set ChoThanhToan =null  where ID = @ID
    		delete from BH_HoaDon_ChiTiet where ID_HoaDon = @ID
    		delete from BH_HoaDon where ID = @ID");

            
        }
        
        public override void Down()
        {
            DropStoredProcedure(@"[dbo].[SP_DeleteHoaDon_whenTimeout]");
        }
    }
}