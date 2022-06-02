namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableBHHoaDonChiPhi_20211102 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BH_HoaDon_ChiPhi",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_HoaDon = c.Guid(nullable: false),
                        ID_HoaDon_ChiTiet = c.Guid(nullable: false),
                        ID_NhaCungCap = c.Guid(nullable: false),
                        SoLuong = c.Double(nullable: false),
                        DonGia = c.Double(nullable: false),
                        ThanhTien = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.BH_HoaDon_ChiTiet", t => t.ID_HoaDon_ChiTiet, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_NhaCungCap, cascadeDelete: true)
                .ForeignKey("dbo.BH_HoaDon", t => t.ID_HoaDon, cascadeDelete: true)
                .Index(t => t.ID_HoaDon)
                .Index(t => t.ID_HoaDon_ChiTiet)
                .Index(t => t.ID_NhaCungCap);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BH_HoaDon_ChiPhi", "ID_HoaDon", "dbo.BH_HoaDon");
            DropForeignKey("dbo.BH_HoaDon_ChiPhi", "ID_NhaCungCap", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.BH_HoaDon_ChiPhi", "ID_HoaDon_ChiTiet", "dbo.BH_HoaDon_ChiTiet");
            DropIndex("dbo.BH_HoaDon_ChiPhi", new[] { "ID_NhaCungCap" });
            DropIndex("dbo.BH_HoaDon_ChiPhi", new[] { "ID_HoaDon_ChiTiet" });
            DropIndex("dbo.BH_HoaDon_ChiPhi", new[] { "ID_HoaDon" });
            DropTable("dbo.BH_HoaDon_ChiPhi");
        }
    }
}
