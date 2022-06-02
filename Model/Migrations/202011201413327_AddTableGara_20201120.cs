namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableGara_20201120 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Gara_PhieuTiepNhan",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        MaPhieuTiepNhan = c.String(),
                        NgayVaoXuong = c.DateTime(nullable: false),
                        ID_NhanVien = c.Guid(nullable: false),
                        ID_CoVanDichVu = c.Guid(),
                        ID_Xe = c.Guid(nullable: false),
                        ID_KhachHang = c.Guid(nullable: false),
                        ID_DonVi = c.Guid(nullable: false),
                        NgayXuatXuongDuKien = c.DateTime(),
                        TenLienHe = c.String(),
                        SoDienThoaiLienHe = c.String(maxLength: 20),
                        SoKmVao = c.Int(nullable: false),
                        GhiChu = c.String(),
                        TrangThai = c.Int(nullable: false),
                        SoKmRa = c.Int(nullable: false),
                        NgayXuatXuong = c.DateTime(),
                        XuatXuong_GhiChu = c.String(),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Gara_DanhMucXe", t => t.ID_Xe, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_KhachHang, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_CoVanDichVu)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_CoVanDichVu)
                .Index(t => t.ID_Xe)
                .Index(t => t.ID_KhachHang)
                .Index(t => t.ID_DonVi);
            
            CreateTable(
                "dbo.Gara_DanhMucXe",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        BienSo = c.String(maxLength: 10),
                        ID_KhachHang = c.Guid(),
                        ID_MauXe = c.Guid(nullable: false),
                        NamSanXuat = c.Int(nullable: false),
                        SoMay = c.String(),
                        SoKhung = c.String(),
                        MauSon = c.String(),
                        DungTich = c.String(),
                        HopSo = c.String(),
                        GhiChu = c.String(),
                        TrangThai = c.Int(nullable: false),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Gara_MauXe", t => t.ID_MauXe, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_KhachHang)
                .Index(t => t.ID_KhachHang)
                .Index(t => t.ID_MauXe);
            
            CreateTable(
                "dbo.Gara_MauXe",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TenMauXe = c.String(),
                        ID_HangXe = c.Guid(nullable: false),
                        ID_LoaiXe = c.Guid(nullable: false),
                        GhiChu = c.String(),
                        TrangThai = c.Int(nullable: false),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Gara_HangXe", t => t.ID_HangXe, cascadeDelete: true)
                .ForeignKey("dbo.Gara_LoaiXe", t => t.ID_LoaiXe, cascadeDelete: true)
                .Index(t => t.ID_HangXe)
                .Index(t => t.ID_LoaiXe);
            
            CreateTable(
                "dbo.Gara_HangXe",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        MaHangXe = c.String(),
                        TenHangXe = c.String(),
                        Logo = c.String(),
                        TrangThai = c.Int(nullable: false),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Gara_LoaiXe",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        MaLoaiXe = c.String(),
                        TenLoaiXe = c.String(),
                        TrangThai = c.Int(nullable: false),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Gara_GiayToKemTheo",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_PhieuTiepNhan = c.Guid(nullable: false),
                        TieuDe = c.String(),
                        SoLuong = c.Int(nullable: false),
                        FileDinhKem = c.String(),
                        TrangThai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Gara_PhieuTiepNhan", t => t.ID_PhieuTiepNhan, cascadeDelete: true)
                .Index(t => t.ID_PhieuTiepNhan);
            
            CreateTable(
                "dbo.Gara_HangMucSuaChua",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_PhieuTiepNhan = c.Guid(nullable: false),
                        TenHangMuc = c.String(),
                        TinhTrang = c.String(),
                        PhuongAnSuaChua = c.String(),
                        TrangThai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Gara_PhieuTiepNhan", t => t.ID_PhieuTiepNhan, cascadeDelete: true)
                .Index(t => t.ID_PhieuTiepNhan);
            
            AddColumn("dbo.BH_HoaDon", "ID_PhieuTiepNhan", c => c.Guid());
            AddColumn("dbo.BH_HoaDon", "ID_BaoHiem", c => c.Guid());
            AddColumn("dbo.BH_HoaDon", "LienHeBaoHiem", c => c.String());
            AddColumn("dbo.BH_HoaDon", "SoDienThoaiLienHeBaoHiem", c => c.String(maxLength: 20));
            AddColumn("dbo.BH_HoaDon", "PhaiThanhToanBaoHiem", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "TongThanhToan", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "ChiPhi", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "ChiPhi_GhiChu", c => c.String());
            AddColumn("dbo.ChietKhauDoanhThu", "LoaiNhanVienApDung", c => c.Int());
            AddColumn("dbo.DM_HangHoa", "SoKmBaoDuong", c => c.Int());
            CreateIndex("dbo.BH_HoaDon", "ID_PhieuTiepNhan");
            CreateIndex("dbo.BH_HoaDon", "ID_BaoHiem");
            AddForeignKey("dbo.BH_HoaDon", "ID_BaoHiem", "dbo.DM_DoiTuong", "ID");
            AddForeignKey("dbo.BH_HoaDon", "ID_PhieuTiepNhan", "dbo.Gara_PhieuTiepNhan", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gara_PhieuTiepNhan", "ID_CoVanDichVu", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Gara_PhieuTiepNhan", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Gara_PhieuTiepNhan", "ID_KhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.Gara_DanhMucXe", "ID_KhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.Gara_PhieuTiepNhan", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.Gara_HangMucSuaChua", "ID_PhieuTiepNhan", "dbo.Gara_PhieuTiepNhan");
            DropForeignKey("dbo.Gara_GiayToKemTheo", "ID_PhieuTiepNhan", "dbo.Gara_PhieuTiepNhan");
            DropForeignKey("dbo.Gara_PhieuTiepNhan", "ID_Xe", "dbo.Gara_DanhMucXe");
            DropForeignKey("dbo.Gara_MauXe", "ID_LoaiXe", "dbo.Gara_LoaiXe");
            DropForeignKey("dbo.Gara_MauXe", "ID_HangXe", "dbo.Gara_HangXe");
            DropForeignKey("dbo.Gara_DanhMucXe", "ID_MauXe", "dbo.Gara_MauXe");
            DropForeignKey("dbo.BH_HoaDon", "ID_PhieuTiepNhan", "dbo.Gara_PhieuTiepNhan");
            DropForeignKey("dbo.BH_HoaDon", "ID_BaoHiem", "dbo.DM_DoiTuong");
            DropIndex("dbo.Gara_HangMucSuaChua", new[] { "ID_PhieuTiepNhan" });
            DropIndex("dbo.Gara_GiayToKemTheo", new[] { "ID_PhieuTiepNhan" });
            DropIndex("dbo.Gara_MauXe", new[] { "ID_LoaiXe" });
            DropIndex("dbo.Gara_MauXe", new[] { "ID_HangXe" });
            DropIndex("dbo.Gara_DanhMucXe", new[] { "ID_MauXe" });
            DropIndex("dbo.Gara_DanhMucXe", new[] { "ID_KhachHang" });
            DropIndex("dbo.Gara_PhieuTiepNhan", new[] { "ID_DonVi" });
            DropIndex("dbo.Gara_PhieuTiepNhan", new[] { "ID_KhachHang" });
            DropIndex("dbo.Gara_PhieuTiepNhan", new[] { "ID_Xe" });
            DropIndex("dbo.Gara_PhieuTiepNhan", new[] { "ID_CoVanDichVu" });
            DropIndex("dbo.Gara_PhieuTiepNhan", new[] { "ID_NhanVien" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_BaoHiem" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_PhieuTiepNhan" });
            DropColumn("dbo.DM_HangHoa", "SoKmBaoDuong");
            DropColumn("dbo.ChietKhauDoanhThu", "LoaiNhanVienApDung");
            DropColumn("dbo.BH_HoaDon", "ChiPhi_GhiChu");
            DropColumn("dbo.BH_HoaDon", "ChiPhi");
            DropColumn("dbo.BH_HoaDon", "TongThanhToan");
            DropColumn("dbo.BH_HoaDon", "PhaiThanhToanBaoHiem");
            DropColumn("dbo.BH_HoaDon", "SoDienThoaiLienHeBaoHiem");
            DropColumn("dbo.BH_HoaDon", "LienHeBaoHiem");
            DropColumn("dbo.BH_HoaDon", "ID_BaoHiem");
            DropColumn("dbo.BH_HoaDon", "ID_PhieuTiepNhan");
            DropTable("dbo.Gara_HangMucSuaChua");
            DropTable("dbo.Gara_GiayToKemTheo");
            DropTable("dbo.Gara_LoaiXe");
            DropTable("dbo.Gara_HangXe");
            DropTable("dbo.Gara_MauXe");
            DropTable("dbo.Gara_DanhMucXe");
            DropTable("dbo.Gara_PhieuTiepNhan");
        }
    }
}
