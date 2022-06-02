namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableCSKH_20220107 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CSKH_DatLich_HangHoa",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IDDatLich = c.Guid(nullable: false),
                        IDHangHoa = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CSKH_DatLich", t => t.IDDatLich, cascadeDelete: true)
                .ForeignKey("dbo.DM_HangHoa", t => t.IDHangHoa, cascadeDelete: true)
                .Index(t => t.IDDatLich)
                .Index(t => t.IDHangHoa);
            
            CreateTable(
                "dbo.CSKH_DatLich",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IDDonVi = c.Guid(nullable: false),
                        IDDoiTuong = c.Guid(),
                        IDXe = c.Guid(),
                        TenDoiTuong = c.String(maxLength: 4000),
                        SoDienThoai = c.String(maxLength: 4000),
                        DiaChi = c.String(maxLength: 4000),
                        BienSo = c.String(maxLength: 4000),
                        LoaiXe = c.String(maxLength: 4000),
                        NgaySinh = c.DateTime(),
                        ThoiGian = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        LoaiDatLich = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gara_DanhMucXe", t => t.IDXe)
                .ForeignKey("dbo.DM_DonVi", t => t.IDDonVi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.IDDoiTuong)
                .Index(t => t.IDDonVi)
                .Index(t => t.IDDoiTuong)
                .Index(t => t.IDXe);
            
            CreateTable(
                "dbo.CSKH_DatLich_NhanVien",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IDDatLich = c.Guid(nullable: false),
                        IDNhanVien = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CSKH_DatLich", t => t.IDDatLich, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.IDNhanVien, cascadeDelete: true)
                .Index(t => t.IDDatLich)
                .Index(t => t.IDNhanVien);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CSKH_DatLich_NhanVien", "IDNhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.CSKH_DatLich", "IDDoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.CSKH_DatLich", "IDDonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.CSKH_DatLich_HangHoa", "IDHangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.CSKH_DatLich", "IDXe", "dbo.Gara_DanhMucXe");
            DropForeignKey("dbo.CSKH_DatLich_NhanVien", "IDDatLich", "dbo.CSKH_DatLich");
            DropForeignKey("dbo.CSKH_DatLich_HangHoa", "IDDatLich", "dbo.CSKH_DatLich");
            DropIndex("dbo.CSKH_DatLich_NhanVien", new[] { "IDNhanVien" });
            DropIndex("dbo.CSKH_DatLich_NhanVien", new[] { "IDDatLich" });
            DropIndex("dbo.CSKH_DatLich", new[] { "IDXe" });
            DropIndex("dbo.CSKH_DatLich", new[] { "IDDoiTuong" });
            DropIndex("dbo.CSKH_DatLich", new[] { "IDDonVi" });
            DropIndex("dbo.CSKH_DatLich_HangHoa", new[] { "IDHangHoa" });
            DropIndex("dbo.CSKH_DatLich_HangHoa", new[] { "IDDatLich" });
            DropTable("dbo.CSKH_DatLich_NhanVien");
            DropTable("dbo.CSKH_DatLich");
            DropTable("dbo.CSKH_DatLich_HangHoa");
        }
    }
}
