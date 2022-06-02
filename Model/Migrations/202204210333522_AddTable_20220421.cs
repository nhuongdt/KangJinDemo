namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_20220421 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Gara_Xe_PhieuBanGiao",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MaPhieu = c.String(maxLength: 4000),
                        IdXe = c.Guid(nullable: false),
                        NgayGiaoXe = c.DateTime(nullable: false),
                        NgayNhanXe = c.DateTime(),
                        NgayNhanXeDuKien = c.DateTime(),
                        SoKmBanGiao = c.Int(nullable: false),
                        IdNhanVienBanGiao = c.Guid(nullable: false),
                        IdNhanVienTiepNhan = c.Guid(),
                        IdNhanVien = c.Guid(),
                        IdKhachHang = c.Guid(),
                        LaNhanVien = c.Int(nullable: false),
                        GhiChuBanGiao = c.String(maxLength: 4000),
                        GhiChuTiepNhan = c.String(maxLength: 4000),
                        TrangThai = c.Int(nullable: false),
                        NgayTaoBanGiao = c.DateTime(nullable: false),
                        NgaySuaBanGiao = c.DateTime(),
                        NguoiTaoBanGiao = c.String(maxLength: 4000),
                        NguoiSuaBanGiao = c.String(maxLength: 4000),
                        NgayTaoTiepNhan = c.DateTime(),
                        NgaySuaTiepNhan = c.DateTime(),
                        NguoiTaoTiepNhan = c.String(maxLength: 4000),
                        NguoiSuaTiepNhan = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gara_DanhMucXe", t => t.IdXe, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.IdKhachHang)
                .ForeignKey("dbo.NS_NhanVien", t => t.IdNhanVien)
                .ForeignKey("dbo.NS_NhanVien", t => t.IdNhanVienBanGiao, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.IdNhanVienTiepNhan)
                .Index(t => t.IdXe)
                .Index(t => t.IdNhanVienBanGiao)
                .Index(t => t.IdNhanVienTiepNhan)
                .Index(t => t.IdNhanVien)
                .Index(t => t.IdKhachHang);
            
            CreateTable(
                "dbo.Gara_Xe_NhatKyHoatDong",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IdPhieuBanGiao = c.Guid(nullable: false),
                        IdNhanVienThucHien = c.Guid(),
                        IdKhachHang = c.Guid(),
                        LaNhanVien = c.Int(nullable: false),
                        ThoiGianHoatDong = c.DateTime(nullable: false),
                        SoGioHoatDong = c.Double(nullable: false),
                        SoKmHienTai = c.Int(nullable: false),
                        GhiChu = c.String(maxLength: 4000),
                        TrangThai = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiTao = c.String(maxLength: 4000),
                        NgaySua = c.DateTime(),
                        NguoiSua = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gara_Xe_PhieuBanGiao", t => t.IdPhieuBanGiao, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.IdKhachHang)
                .ForeignKey("dbo.NS_NhanVien", t => t.IdNhanVienThucHien)
                .Index(t => t.IdPhieuBanGiao)
                .Index(t => t.IdNhanVienThucHien)
                .Index(t => t.IdKhachHang);

            AddColumn("dbo.Gara_DanhMucXe", "NguoiSoHuu", c => c.Int());

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gara_Xe_PhieuBanGiao", "IdNhanVienTiepNhan", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Gara_Xe_PhieuBanGiao", "IdNhanVienBanGiao", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Gara_Xe_PhieuBanGiao", "IdNhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Gara_Xe_NhatKyHoatDong", "IdNhanVienThucHien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Gara_Xe_PhieuBanGiao", "IdKhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.Gara_Xe_NhatKyHoatDong", "IdKhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.Gara_Xe_PhieuBanGiao", "IdXe", "dbo.Gara_DanhMucXe");
            DropForeignKey("dbo.Gara_Xe_NhatKyHoatDong", "IdPhieuBanGiao", "dbo.Gara_Xe_PhieuBanGiao");
            DropIndex("dbo.Gara_Xe_NhatKyHoatDong", new[] { "IdKhachHang" });
            DropIndex("dbo.Gara_Xe_NhatKyHoatDong", new[] { "IdNhanVienThucHien" });
            DropIndex("dbo.Gara_Xe_NhatKyHoatDong", new[] { "IdPhieuBanGiao" });
            DropIndex("dbo.Gara_Xe_PhieuBanGiao", new[] { "IdKhachHang" });
            DropIndex("dbo.Gara_Xe_PhieuBanGiao", new[] { "IdNhanVien" });
            DropIndex("dbo.Gara_Xe_PhieuBanGiao", new[] { "IdNhanVienTiepNhan" });
            DropIndex("dbo.Gara_Xe_PhieuBanGiao", new[] { "IdNhanVienBanGiao" });
            DropIndex("dbo.Gara_Xe_PhieuBanGiao", new[] { "IdXe" });
            DropTable("dbo.Gara_Xe_NhatKyHoatDong");
            DropTable("dbo.Gara_Xe_PhieuBanGiao");
        }
    }
}
