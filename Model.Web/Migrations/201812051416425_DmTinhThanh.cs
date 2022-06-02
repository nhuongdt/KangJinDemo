namespace Model.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DmTinhThanh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DM_TinhThanh", "Priority", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DM_TinhThanh", "Priority");
        }
    }
}
