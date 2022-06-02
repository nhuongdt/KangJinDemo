namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableDMHangHoa_20200922 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DM_HangHoa", "DichVuTheoGio", c => c.Int(nullable: false));
            AddColumn("dbo.DM_HangHoa", "DuocTichDiem", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DM_HangHoa", "DuocTichDiem");
            DropColumn("dbo.DM_HangHoa", "DichVuTheoGio");
        }
    }
}
