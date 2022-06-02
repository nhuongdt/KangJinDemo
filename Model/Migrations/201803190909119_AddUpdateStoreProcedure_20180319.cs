namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateStoreProcedure_20180319 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SP_GetMaDoiTuong_Max]", parametersAction: p => new
            {
                LoaiDoiTuong = p.Int()
            }, body: @"DECLARE @MaDTuongOffline varchar(5);
if @LoaiDoiTuong = 1 
	set @MaDTuongOffline ='KHO'
if @LoaiDoiTuong = 2 
	set @MaDTuongOffline ='NCCO'

	-- get list DoiTuong not offline
	SELECT MAX(CAST (dbo.udf_GetNumeric(MaDoiTuong) AS INT)) MaxCode
	FROM DM_DoiTuong WHERE LoaiDoiTuong = @LoaiDoiTuong and CHARINDEX(@MaDTuongOffline,MaDoiTuong)=0");

            CreateStoredProcedure(name: "[dbo].[SP_GetMaHoaDon_Max]", parametersAction: p => new
            {
                LoaiHoaDon = p.Int()
            }, body: @"DECLARE @MaHoaDonOffline varchar(5);
if @LoaiHoaDon = 1 
	set @MaHoaDonOffline ='HDO'
if @LoaiHoaDon = 3 
	set @MaHoaDonOffline ='DHO'
if @LoaiHoaDon = 6 
	set @MaHoaDonOffline ='THO'
	-- get list HoaDon not offline
	SELECT MAX(CAST(dbo.udf_GetNumeric(mahoadon)AS INT)) AS MaxCode
	FROM BH_HoaDon WHERE loaihoadon = @LoaiHoaDon and CHARINDEX(@MaHoaDonOffline,MaHoaDon)=0");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GetMaDoiTuong_Max]");
            DropStoredProcedure("[dbo].[SP_GetMaHoaDon_Max]");
        }
    }
}