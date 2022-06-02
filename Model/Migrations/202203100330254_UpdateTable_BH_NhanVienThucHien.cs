namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_BH_NhanVienThucHien : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BH_NhanVienThucHien", "TinhHoaHongTruocCK", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BH_NhanVienThucHien", "TinhHoaHongTruocCK");
        }
    }
}
