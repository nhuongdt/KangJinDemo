namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTable_20190802 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NS_CaLamViec_DonVi",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_CaLamViec = c.Guid(nullable: false),
                        ID_DonVi = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_CaLamViec", t => t.ID_CaLamViec, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_CaLamViec)
                .Index(t => t.ID_DonVi);
            
            AddColumn("dbo.BH_NhanVienThucHien", "ID_QuyHoaDon", c => c.Guid());
            AddColumn("dbo.NS_PhieuPhanCa", "LoaiPhanCa", c => c.Int(nullable: false));
            AddColumn("dbo.NS_PhieuPhanCa", "ID_DonVi", c => c.Guid(nullable: false));
            CreateIndex("dbo.BH_NhanVienThucHien", "ID_QuyHoaDon");
            CreateIndex("dbo.NS_PhieuPhanCa", "ID_DonVi");
            AddForeignKey("dbo.BH_NhanVienThucHien", "ID_QuyHoaDon", "dbo.Quy_HoaDon", "ID");
            AddForeignKey("dbo.NS_PhieuPhanCa", "ID_DonVi", "dbo.DM_DonVi", "ID", cascadeDelete: true);
            DropColumn("dbo.NS_PhieuPhanCa_ChiTiet", "LoaiPhanCa");
            AddColumn("dbo.Quy_HoaDon", "PhieuDieuChinhCongNo", c => c.Int());
        }
        
        public override void Down()
        {
            AddColumn("dbo.NS_PhieuPhanCa_ChiTiet", "LoaiPhanCa", c => c.Int(nullable: false));
            DropForeignKey("dbo.NS_PhieuPhanCa", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_CaLamViec_DonVi", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_CaLamViec_DonVi", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.BH_NhanVienThucHien", "ID_QuyHoaDon", "dbo.Quy_HoaDon");
            DropIndex("dbo.NS_PhieuPhanCa", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_CaLamViec_DonVi", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_CaLamViec_DonVi", new[] { "ID_CaLamViec" });
            DropIndex("dbo.BH_NhanVienThucHien", new[] { "ID_QuyHoaDon" });
            DropColumn("dbo.NS_PhieuPhanCa", "ID_DonVi");
            DropColumn("dbo.NS_PhieuPhanCa", "LoaiPhanCa");
            DropColumn("dbo.BH_NhanVienThucHien", "ID_QuyHoaDon");
            DropTable("dbo.NS_CaLamViec_DonVi");
            DropColumn("dbo.Quy_HoaDon", "PhieuDieuChinhCongNo");
        }
    }
}
