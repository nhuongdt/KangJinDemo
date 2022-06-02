namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableBH_HoaDon_Anh : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BH_HoaDon_Anh",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IdHoaDon = c.Guid(nullable: false),
                        URLAnh = c.String(maxLength: 4000),
                        NgayTao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BH_HoaDon", t => t.IdHoaDon, cascadeDelete: true)
                .Index(t => t.IdHoaDon);
            
            AddColumn("dbo.BH_HoaDon_ChiPhi", "GhiChu", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BH_HoaDon_Anh", "IdHoaDon", "dbo.BH_HoaDon");
            DropIndex("dbo.BH_HoaDon_Anh", new[] { "IdHoaDon" });
            DropColumn("dbo.BH_HoaDon_ChiPhi", "GhiChu");
            DropTable("dbo.BH_HoaDon_Anh");
        }
    }
}
