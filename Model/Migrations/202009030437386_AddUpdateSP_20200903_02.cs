namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20200903_02 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[GetDayOfWeek_byPhieuPhanCa]
(
	@ID_PhieuPhanCa uniqueidentifier,
	@ID_CaLamViec uniqueidentifier
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @DateOfWeek int

	select @DateOfWeek = GiaTri
	from NS_PhieuPhanCa_CaLamViec
	where ID_PhieuPhanCa= @ID_PhieuPhanCa
	and ID_CaLamViec =  @ID_CaLamViec

	return @DateOfWeek + 1
END");
        }
        
        public override void Down()
        {
            Sql("DROP FUNCTION [dbo].[GetDayOfWeek_byPhieuPhanCa]");
        }
    }
}
