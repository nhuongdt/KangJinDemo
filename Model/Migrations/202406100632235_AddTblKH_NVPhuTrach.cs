namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTblKH_NVPhuTrach : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KH_NVPhuTrach",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_KhachHang = c.Guid(nullable: false),
                        ID_NhanVienPhuTrach = c.Guid(nullable: false),
                        VaiTro = c.Byte(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_KhachHang, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienPhuTrach, cascadeDelete: true)
                .Index(t => t.ID_KhachHang)
                .Index(t => t.ID_NhanVienPhuTrach);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KH_NVPhuTrach", "ID_NhanVienPhuTrach", "dbo.NS_NhanVien");
            DropForeignKey("dbo.KH_NVPhuTrach", "ID_KhachHang", "dbo.DM_DoiTuong");
            DropIndex("dbo.KH_NVPhuTrach", new[] { "ID_NhanVienPhuTrach" });
            DropIndex("dbo.KH_NVPhuTrach", new[] { "ID_KhachHang" });
            DropTable("dbo.KH_NVPhuTrach");
        }
    }
}
