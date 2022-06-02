namespace Model.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMenu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DM_Menu", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DM_Menu", "Title");
        }
    }
}
