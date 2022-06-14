namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_DM_KhoanThuChi_20220614 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quy_KhoanThuChi", "LoaiChungTu", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quy_KhoanThuChi", "LoaiChungTu");
        }
    }
}
