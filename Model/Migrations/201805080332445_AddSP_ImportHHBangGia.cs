namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSP_ImportHHBangGia : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ImportHHBangGia]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_KhoHang = p.Guid(),
                ID_NgoaiTe = p.Guid(),
                GiaBan = p.Double(),
                ID_GiaBan = p.Guid(),
                ID_DonViQuiDoi = p.Guid()
            }, body: @"INSERT INTO DM_GiaBan_ChiTiet values(@ID, @ID_GiaBan,@ID_KhoHang,@GiaBan,@ID_NgoaiTe,null,null,@ID_DonViQuiDoi, GETDATE())");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ImportHHBangGia]");
        }
    }
}