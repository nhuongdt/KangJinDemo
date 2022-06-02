namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_DM_HangHoa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DM_HangHoa", "HoaHongTruocChietKhau", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DM_HangHoa", "HoaHongTruocChietKhau");
        }
    }
}
