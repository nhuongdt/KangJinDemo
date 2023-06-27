namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableBH_ChiTiet_DinhDanh : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BH_ChiTiet_DinhDanh",
                c => new
                    {
                        MaDinhDanh = c.Int(nullable: false, identity: true),
                        IdHoaDonChiTiet = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.MaDinhDanh)
                .ForeignKey("dbo.BH_HoaDon_ChiTiet", t => t.IdHoaDonChiTiet, cascadeDelete: true)
                .Index(t => t.IdHoaDonChiTiet);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BH_ChiTiet_DinhDanh", "IdHoaDonChiTiet", "dbo.BH_HoaDon_ChiTiet");
            DropIndex("dbo.BH_ChiTiet_DinhDanh", new[] { "IdHoaDonChiTiet" });
            DropTable("dbo.BH_ChiTiet_DinhDanh");
        }
    }
}
