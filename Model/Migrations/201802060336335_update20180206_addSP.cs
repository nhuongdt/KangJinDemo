namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update20180206_addSP : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[splitstring] ( @stringToSplit VARCHAR(MAX) )
RETURNS
 @returnList TABLE ([Name] [nvarchar] (500))
AS
BEGIN

 DECLARE @name NVARCHAR(255)
 DECLARE @pos INT

 WHILE CHARINDEX(',', @stringToSplit) > 0
 BEGIN
  SELECT @pos  = CHARINDEX(',', @stringToSplit)  
  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)

  INSERT INTO @returnList 
  SELECT @name

  SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
 END

 INSERT INTO @returnList
 SELECT @stringToSplit

 RETURN
END");
            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietChungCongPhanTram]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DonViQuiDoi SET GiaBan = GiaBan + @giaTri * GiaBan/ 100;
	END
	else if(@LoaiGiaChon = 2) 
	BEGIN
		update DonViQUiDoi SET GiaBan =GiaNhap + @giaTri * GiaNhap/ 100;
	END
	else
	BEGIN
		update DonViQUiDoi SET GiaBan = GiaVon + @giaTri * GiaVon/ 100;
	END");

            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietChungCongVND]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DonViQuiDoi SET GiaBan = GiaBan + @giaTri  
	END
	else if(@LoaiGiaChon = 2) 
	BEGIN
		update DonViQUiDoi SET GiaBan =GiaNhap + @giaTri  
	END
	else
	BEGIN
		update DonViQUiDoi SET GiaBan = GiaVon + @giaTri  
	END");

            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietChungTruPhanTram]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DonViQuiDoi SET GiaBan = GiaBan - @giaTri * GiaBan/ 100;
	END
	else if(@LoaiGiaChon = 2) 
	BEGIN
		update DonViQUiDoi SET GiaBan =GiaNhap - @giaTri * GiaNhap/ 100;
	END
	else
	BEGIN
		update DonViQUiDoi SET GiaBan = GiaVon - @giaTri * GiaVon/ 100;
	END");

            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietChungTruVND]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DonViQuiDoi SET GiaBan = GiaBan - @giaTri;
	END
	else if(@LoaiGiaChon = 2) 
	BEGIN
		update DonViQUiDoi SET GiaBan =GiaNhap - @giaTri;
	END
	else
	BEGIN
		update DonViQUiDoi SET GiaBan = GiaVon - @giaTri;
	END");

            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietCongPhanTram]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double(),
                ID = p.Guid()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan + (@giaTri* GiaBan /100)  where ID_GiaBan = @ID 
	END
	else if(@LoaiGiaChon = 1) 
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri*(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) /100 where ID_GiaBan = @ID  
	END
	else if(@LoaiGiaChon = 2)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri *(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID  
	END
	else
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaVon from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri * (select GiaVon from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID  
	END");

            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietCongVND]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double(),
                ID = p.Guid()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan + @giaTri where ID_GiaBan = @ID 
	END
	else if(@LoaiGiaChon = 1) 
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri where ID_GiaBan = @ID  
	END
	else if(@LoaiGiaChon = 2)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri where ID_GiaBan = @ID  
	END
	else
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaVon from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri where ID_GiaBan = @ID  
	END");

            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietTruPhanTram]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double(),
                ID = p.Guid()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan - (@giaTri* GiaBan /100)  where ID_GiaBan = @ID 
	END
	else if(@LoaiGiaChon = 1) 
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri*(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) /100 where ID_GiaBan = @ID  
	END
	else if(@LoaiGiaChon = 2)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri *(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID  
	END
	else
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaVon from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri * (select GiaVon from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID  
	END");

            CreateStoredProcedure(name: "[dbo].[PutGiaBanChiTietTruVND]", parametersAction: p => new
            {
                LoaiGiaChon = p.Int(),
                giaTri = p.Double(),
                ID = p.Guid()
            }, body: @"if(@LoaiGiaChon = 0)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan - @giaTri where ID_GiaBan = @ID 
	END
	else if(@LoaiGiaChon = 1) 
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri where ID_GiaBan = @ID  
	END
	else if(@LoaiGiaChon = 2)
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri where ID_GiaBan = @ID  
	END
	else
	BEGIN
		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaVon from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri where ID_GiaBan = @ID  
	END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[PutGiaBanChiTietChungCongPhanTram]");
            DropStoredProcedure("[dbo].[PutGiaBanChiTietChungCongVND]");
            DropStoredProcedure("[dbo].[PutGiaBanChiTietChungTruPhanTram]");
            DropStoredProcedure("[dbo].[PutGiaBanChiTietChungTruVND]");
            DropStoredProcedure("[dbo].[PutGiaBanChiTietCongPhanTram]");
            DropStoredProcedure("[dbo].[PutGiaBanChiTietCongVND]");
            DropStoredProcedure("[dbo].[PutGiaBanChiTietTruPhanTram]");
            DropStoredProcedure("[dbo].[PutGiaBanChiTietTruVND]");
            Sql("DROP FUNCTION [dbo].[splitstring]");
        }
    }
}
