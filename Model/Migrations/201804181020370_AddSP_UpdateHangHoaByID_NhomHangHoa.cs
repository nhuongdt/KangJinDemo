namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSP_UpdateHangHoaByID_NhomHangHoa : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[UpdateHangHoaByID_NhomHangHoa]", parametersAction: p => new
            {
                ID_NhomHangHoa = p.Guid()
            }, body: @"update DM_HangHoa set ID_NhomHang = null where ID_NhomHang = @ID_NhomHangHoa");

            CreateStoredProcedure(name: "[dbo].[SP_GetMaQuyHoaDon_Max]", parametersAction: p => new
            {
                LoaiHoaDon = p.Int()
            }, body: @"DECLARE @MaHoaDon varchar(5);
    if @LoaiHoaDon = 11 --Phieu Thu --
    	set @MaHoaDon ='SQPT'
    if @LoaiHoaDon = 12 --Phieu Chi --
    	set @MaHoaDon ='SQPC'	
    if @LoaiHoaDon = 15 --Dieu Chinh --
    	set @MaHoaDon ='CB'

    	SELECT MAX(CAST (dbo.udf_GetNumeric(MaHoaDon) AS INT)) MaxCode
    	FROM Quy_HoaDon WHERE CHARINDEX(@MaHoaDon,MaHoaDon) > 0 and CHARINDEX('Copy',MaHoaDon)= 0 and CHARINDEX('_',MaHoaDon)= 0");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[UpdateHangHoaByID_NhomHangHoa]");
            DropStoredProcedure("[dbo].[SP_GetMaQuyHoaDon_Max]");
        }
    }
}