namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableCSKH_20220108 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DM_HangHoa", "HienThiDatLich", c => c.Int());
            AddColumn("dbo.CSKH_DatLich", "NgayTao", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CSKH_DatLich", "NgayTao");
            DropColumn("dbo.DM_HangHoa", "HienThiDatLich");
        }
    }
}
