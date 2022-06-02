namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateTableNS_20191008 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DM_DoiTuong_CongNo",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DoiTuong = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    NoHienTai = c.Double(nullable: false),
                    TongBan = c.Double(nullable: false),
                    TongTra = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong, cascadeDelete: true)
                .Index(t => t.ID_DoiTuong)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.NS_BangLuong_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_BangLuong = c.Guid(nullable: false),
                    ID_BangCong_ChiTiet = c.Guid(nullable: false),
                    ID_CaLamViec = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    NgayCongThuc = c.Double(nullable: false),
                    NgayCongChuan = c.Double(nullable: false),
                    LuongCoBan = c.Double(nullable: false),
                    PhuCapCoBan = c.Double(nullable: false),
                    PhuCapKhac = c.Double(nullable: false),
                    KhenThuong = c.Double(nullable: false),
                    ThueTNCN = c.Double(nullable: false),
                    MienGiamThue = c.Double(nullable: false),
                    BaoHiem = c.Double(nullable: false),
                    KyLuat = c.Double(nullable: false),
                    PhatDiMuon = c.Double(nullable: false),
                    LuongOT = c.Double(nullable: false),
                    LuongThucNhan = c.Double(nullable: false),
                    TongTienPhat = c.Double(nullable: false),
                    TongLuongNhan = c.Double(nullable: false),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(maxLength: 4000),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(maxLength: 4000),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_BangLuong", t => t.ID_BangLuong, cascadeDelete: true)
                .ForeignKey("dbo.NS_CaLamViec", t => t.ID_CaLamViec, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_BangLuong)
                .Index(t => t.ID_CaLamViec)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_BangLuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaBangLuong = c.String(),
                    TenBangLuong = c.String(),
                    ID_KyTinhCong = c.Guid(nullable: false),
                    ID_NhanVienDuyet = c.Guid(),
                    TrangThai = c.Int(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 4000),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(maxLength: 4000),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_KyTinhCong", t => t.ID_KyTinhCong, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienDuyet)
                .Index(t => t.ID_KyTinhCong)
                .Index(t => t.ID_NhanVienDuyet);

            CreateTable(
                "dbo.NS_LoaiKhenThuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenLoaiKhenThuong = c.String(maxLength: 4000),
                    PhanLoai = c.Int(nullable: false),
                    MoTa = c.String(),
                    TienThuong = c.Double(nullable: false),
                    TuNgay = c.DateTime(nullable: false),
                    DenNgay = c.DateTime(),
                    TrangThai = c.Int(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 4000),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(maxLength: 4000),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_LoaiBaoHiem",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenBaoHiem = c.String(maxLength: 4000),
                    NgayApDung = c.DateTime(nullable: false),
                    TrangThai = c.Int(nullable: false),
                    TyLeCongTy = c.Double(nullable: false),
                    TyLeNhanVien = c.Double(nullable: false),
                    Tong = c.Double(nullable: false),
                    NguoiTao = c.String(maxLength: 4000),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(maxLength: 4000),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            AddColumn("dbo.HT_CongTy", "DangKyNhanSu", c => c.Boolean());
            AddColumn("dbo.HT_CongTy", "NgayCongChuan", c => c.Double());
            AddColumn("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", c => c.Guid(nullable: false));
            AddColumn("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", c => c.Guid(nullable: false));
            AddColumn("dbo.NS_KhenThuong", "ID_KyTinhCong", c => c.Guid(nullable: false));
            CreateIndex("dbo.NS_KhenThuong", "ID_LoaiKhenThuong");
            CreateIndex("dbo.NS_KhenThuong", "ID_KyTinhCong");
            CreateIndex("dbo.NS_BaoHiem", "ID_LoaiBaoHiem");
            AddForeignKey("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", "dbo.NS_LoaiKhenThuong", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_KhenThuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", "dbo.NS_LoaiBaoHiem", "ID", cascadeDelete: true);
            DropForeignKey("dbo.NS_Luong_PhuCap", "ID_LoaiLuong", "dbo.NS_LoaiLuong");
            DropIndex("dbo.NS_Luong_PhuCap", new[] { "ID_LoaiLuong" });
            AddColumn("dbo.BH_HoaDon", "DiemKhuyenMai", c => c.Double());
            AddColumn("dbo.BH_HoaDon_ChiTiet", "DiemKhuyenMai", c => c.Double());
            AddColumn("dbo.NS_BangLuong", "NgayThanhToanLuong", c => c.DateTime());
            AddColumn("dbo.NS_BangLuong", "SuDungHRM", c => c.Boolean(nullable: false));
            AddColumn("dbo.NS_Luong_PhuCap", "LoaiLuong", c => c.Int(nullable: false));
            AlterColumn("dbo.NS_Luong_PhuCap", "ID_LoaiLuong", c => c.Guid());
            CreateIndex("dbo.NS_Luong_PhuCap", "ID_LoaiLuong");
            AddForeignKey("dbo.NS_Luong_PhuCap", "ID_LoaiLuong", "dbo.NS_LoaiLuong", "ID");

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", "dbo.NS_LoaiBaoHiem");
            DropForeignKey("dbo.NS_BangLuong", "ID_NhanVienDuyet", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_BangLuong_ChiTiet", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.DM_DoiTuong_CongNo", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.NS_BangLuong_ChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.NS_KhenThuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", "dbo.NS_LoaiKhenThuong");
            DropForeignKey("dbo.NS_BangLuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_BangLuong_ChiTiet", "ID_BangLuong", "dbo.NS_BangLuong");
            DropForeignKey("dbo.DM_DoiTuong_CongNo", "ID_DonVi", "dbo.DM_DonVi");
            DropIndex("dbo.NS_BaoHiem", new[] { "ID_LoaiBaoHiem" });
            DropIndex("dbo.NS_KhenThuong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_KhenThuong", new[] { "ID_LoaiKhenThuong" });
            DropIndex("dbo.NS_BangLuong", new[] { "ID_NhanVienDuyet" });
            DropIndex("dbo.NS_BangLuong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_BangLuong_ChiTiet", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_BangLuong_ChiTiet", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_BangLuong_ChiTiet", new[] { "ID_BangLuong" });
            DropIndex("dbo.DM_DoiTuong_CongNo", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_DoiTuong_CongNo", new[] { "ID_DoiTuong" });
            DropColumn("dbo.NS_KhenThuong", "ID_KyTinhCong");
            DropColumn("dbo.NS_KhenThuong", "ID_LoaiKhenThuong");
            DropColumn("dbo.NS_BaoHiem", "ID_LoaiBaoHiem");
            DropColumn("dbo.HT_CongTy", "NgayCongChuan");
            DropColumn("dbo.HT_CongTy", "DangKyNhanSu");
            DropTable("dbo.NS_LoaiBaoHiem");
            DropTable("dbo.NS_LoaiKhenThuong");
            DropTable("dbo.NS_BangLuong");
            DropTable("dbo.NS_BangLuong_ChiTiet");
            DropTable("dbo.DM_DoiTuong_CongNo");
            DropForeignKey("dbo.NS_Luong_PhuCap", "ID_LoaiLuong", "dbo.NS_LoaiLuong");
            DropIndex("dbo.NS_Luong_PhuCap", new[] { "ID_LoaiLuong" });
            AlterColumn("dbo.NS_Luong_PhuCap", "ID_LoaiLuong", c => c.Guid(nullable: false));
            DropColumn("dbo.NS_Luong_PhuCap", "LoaiLuong");
            DropColumn("dbo.NS_BangLuong", "SuDungHRM");
            DropColumn("dbo.NS_BangLuong", "NgayThanhToanLuong");
            DropColumn("dbo.BH_HoaDon_ChiTiet", "DiemKhuyenMai");
            DropColumn("dbo.BH_HoaDon", "DiemKhuyenMai");
            CreateIndex("dbo.NS_Luong_PhuCap", "ID_LoaiLuong");
            AddForeignKey("dbo.NS_Luong_PhuCap", "ID_LoaiLuong", "dbo.NS_LoaiLuong", "ID", cascadeDelete: true);
        }
    }
}
