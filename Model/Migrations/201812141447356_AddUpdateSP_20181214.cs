namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181214 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[getlist_DoiTuong_HinhThuc3]
    @SqlQuery [nvarchar](max)
AS
BEGIN
    declare @sql  [nvarchar](max);
    	declare @sql2  [nvarchar](max);
    	declare @sql3  [nvarchar](max);
    	set @sql = 'select hd.ID_DoiTuong from BH_HoaDon hd
    					inner join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.ChoThanhToan = 0
    				and (hd.LoaiHoaDon = 1 or  hd.LoaiHoaDon = 19)
    					and dt.theodoi = 0
    				and hd.ID_DoiTuong is not null
    				and NgayLapHoaDon';
    	set @sql2 = ' GROUP by hd.ID_DoiTuong'
    	set @sql3 = @sql + @SqlQuery + @sql2;
    	exec (@sql3);
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_DoiTuong_HinhThuc4]
    @SqlQuery [nvarchar](max)
AS
BEGIN
    declare @sql  [nvarchar](max);
    	declare @sql2  [nvarchar](max);
    	set @sql = 'select a.ID_DoiTuong, a.SoLanMuaHang from
    				(
    				select hd.ID_DoiTuong, Count(*) as SoLanMuaHang from BH_HoaDon hd
    				where hd.ChoThanhToan = 0
    				and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    				and hd.ID_DoiTuong is not null
    				GROUP by hd.ID_DoiTuong
    				) a
    				where SoLanMuaHang';
    	set @sql2 = @sql + @SqlQuery;
    	exec (@sql2);
END");
            
            Sql(@"ALTER PROCEDURE [dbo].[insert_DoiTuong_Nhom]
    @LoaiCapNhat [int],
    @DK_Xoa [int],
    @ID_DoiTuong [uniqueidentifier],
    @ID_NhomDoiTuong [uniqueidentifier]
AS
BEGIN
    IF (@LoaiCapNhat = 1 or @LoaiCapNhat = 2)
    	BEGIN
    		if (@DK_xoa = 1)
    		begin
    			DELETE from DM_DoiTuong_Nhom where ID_NhomDoiTuong = @ID_NhomDoiTuong
    			SET @DK_xoa = @DK_xoa + 1;
    		end
    		Insert into DM_DoiTuong_Nhom (ID, ID_DoiTuong, ID_NhomDoiTuong)
    		Values (NEWID(), @ID_DoiTuong, @ID_NhomDoiTuong)
    	END
    	ELSE -- thêm mới khách hàng chưa thuộc nhóm
    	BEGIN
    		Declare @DK [int];
    		set @DK = (select COUNT(*) from DM_DoiTuong_Nhom where ID_DoiTuong = @ID_DoiTuong and ID_NhomDoiTuong = @ID_NhomDoiTuong);
    		if (@DK = 0)
    		BEGIN  
    			Insert into DM_DoiTuong_Nhom (ID, ID_DoiTuong, ID_NhomDoiTuong)
    			Values (NEWID(), @ID_DoiTuong, @ID_NhomDoiTuong)
    		END
    	END
END");
            
        }
        
        public override void Down()
        {
        }
    }
}
