namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181212_1 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SP_Check_LoaiCongViec_Exist]", parametersAction: p => new
            {
                LoaiCongViec = p.String(),
                ID_LoaiCongViec = p.String()
            }, body: @"DECLARE @valReturn bit ='0'
	 DECLARE @ID nvarchar(max);

	 IF @ID_LoaiCongViec='null' 
		 SELECT @ID =  ID from NS_CongViec_PhanLoai WHERE TrangThai != '0'  AND LoaiCongViec like  @LoaiCongViec
	 ELSE
		 SELECT @ID = ID from NS_CongViec_PhanLoai WHERE TrangThai != '0'  AND LoaiCongViec like  @LoaiCongViec  AND ID not like  @ID_LoaiCongViec 
    
    	IF @ID IS NULL SET @valReturn= '0'
		ELSE SET @valReturn= '1'
    
    	SELECT @valReturn AS Exist");

            CreateStoredProcedure(name: "[dbo].[SP_GetListQuyen_Where]", parametersAction: p => new
            {
                Where = p.String()
            }, body: @"declare @sql nvarchar(max)
	set @sql ='SELECT  NEWID() as ID, MaQuyen 
    		FROM HT_NguoiDung_Nhom nnd
			JOIN HT_Quyen_Nhom qn on nnd.IDNhomNguoiDung = qn.ID_NhomNguoiDung
			JOIN HT_NguoiDung htnd on nnd.IDNguoiDung= htnd.ID '  + @Where
	exec (@sql)");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_Check_LoaiCongViec_Exist]");
            DropStoredProcedure("[dbo].[SP_GetListQuyen_Where]");
        }
    }
}
