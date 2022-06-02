namespace Model.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewsTable_HoSoUngTuyen : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DS_HoSoUngTuyen",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ID_TuyenDung = c.Int(nullable: false),
                        HoTen = c.String(),
                        NgaySinh = c.DateTime(),
                        NgayTao = c.DateTime(nullable: false),
                        GioiTinh = c.Boolean(),
                        Email = c.String(),
                        SoDienThoai = c.String(),
                        DiaChi = c.String(),
                        TruongTotNghiep = c.String(),
                        HeDaoTao = c.String(),
                        ChuyenNganh = c.String(),
                        KyNang = c.String(),
                        TrangThai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_TuyenDung", t => t.ID_TuyenDung, cascadeDelete: true)
                .Index(t => t.ID_TuyenDung);
            
            CreateTable(
                "dbo.DS_FileDinhKem",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenFile = c.String(),
                        LinkFile = c.String(),
                        Size = c.Int(nullable: false),
                        ID_HoSoUngTuyen = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DS_HoSoUngTuyen", t => t.ID_HoSoUngTuyen, cascadeDelete: true)
                .Index(t => t.ID_HoSoUngTuyen);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DS_HoSoUngTuyen", "ID_TuyenDung", "dbo.DM_TuyenDung");
            DropForeignKey("dbo.DS_FileDinhKem", "ID_HoSoUngTuyen", "dbo.DS_HoSoUngTuyen");
            DropIndex("dbo.DS_FileDinhKem", new[] { "ID_HoSoUngTuyen" });
            DropIndex("dbo.DS_HoSoUngTuyen", new[] { "ID_TuyenDung" });
            DropTable("dbo.DS_FileDinhKem");
            DropTable("dbo.DS_HoSoUngTuyen");
        }
    }
}
