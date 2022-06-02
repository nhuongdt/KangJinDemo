namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_20211129 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DM_GiaBan", "TrangThai", c => c.Int());
            AlterColumn("dbo.DonViQuiDoi", "MaHangHoa", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DM_GiaBan", "TrangThai");
            AlterColumn("dbo.DonViQuiDoi", "MaHangHoa", c => c.String(maxLength: 50));
        }
    }
}
