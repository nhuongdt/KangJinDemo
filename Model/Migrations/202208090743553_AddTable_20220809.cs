namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_20220809 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NhomHang_ChiTietSanPhamHoTro",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Id_NhomHang = c.Guid(nullable: false),
                        Id_DonViQuiDoi = c.Guid(nullable: false),
                        Id_LoHang = c.Guid(),
                        SoLuong = c.Double(nullable: false),
                        LaSanPhamNgayThuoc = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DM_NhomHangHoa", t => t.Id_NhomHang, cascadeDelete: true)
                .ForeignKey("dbo.DM_LoHang", t => t.Id_LoHang)
                .ForeignKey("dbo.DonViQuiDoi", t => t.Id_DonViQuiDoi, cascadeDelete: true)
                .Index(t => t.Id_NhomHang)
                .Index(t => t.Id_DonViQuiDoi)
                .Index(t => t.Id_LoHang);
            
            CreateTable(
                "dbo.NhomHang_KhoangApDung",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        STT = c.Int(nullable: false),
                        Id_NhomHang = c.Guid(nullable: false),
                        GiaTriSuDungTu = c.Double(nullable: false),
                        GiaTriSuDungDen = c.Double(nullable: false),
                        GiaTriHoTro = c.Double(nullable: false),
                        KieuHoTro = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DM_NhomHangHoa", t => t.Id_NhomHang, cascadeDelete: true)
                .Index(t => t.Id_NhomHang);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NhomHang_ChiTietSanPhamHoTro", "Id_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.NhomHang_ChiTietSanPhamHoTro", "Id_LoHang", "dbo.DM_LoHang");
            DropForeignKey("dbo.NhomHang_KhoangApDung", "Id_NhomHang", "dbo.DM_NhomHangHoa");
            DropForeignKey("dbo.NhomHang_ChiTietSanPhamHoTro", "Id_NhomHang", "dbo.DM_NhomHangHoa");
            DropIndex("dbo.NhomHang_KhoangApDung", new[] { "Id_NhomHang" });
            DropIndex("dbo.NhomHang_ChiTietSanPhamHoTro", new[] { "Id_LoHang" });
            DropIndex("dbo.NhomHang_ChiTietSanPhamHoTro", new[] { "Id_DonViQuiDoi" });
            DropIndex("dbo.NhomHang_ChiTietSanPhamHoTro", new[] { "Id_NhomHang" });
            DropTable("dbo.NhomHang_KhoangApDung");
            DropTable("dbo.NhomHang_ChiTietSanPhamHoTro");
        }
    }
}
