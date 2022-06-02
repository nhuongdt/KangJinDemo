namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableNS_20200831 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NS_BangLuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong", "dbo.NS_MaChamCong");
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_KyHieuCong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_NgayNghiLe", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_BangLuong_ChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropIndex("dbo.NS_BangLuong_ChiTiet", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_BangLuong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_KyHieuCong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_MaChamCong" });
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_NgayNghiLe", new[] { "ID_KyTinhCong" });
            AddColumn("dbo.NS_BangLuong", "ID_DonVi", c => c.Guid());
            AddColumn("dbo.NS_BangLuong", "TuNgay", c => c.DateTime());
            AddColumn("dbo.NS_BangLuong", "DenNgay", c => c.DateTime());
            AddColumn("dbo.NS_CongBoSung", "HeSoLuong", c => c.Double());
            AddColumn("dbo.NS_CongBoSung", "HeSoLuongOT", c => c.Double());
            AddColumn("dbo.NS_NgayNghiLe", "ID_BangLuong", c => c.Guid());
            AddColumn("dbo.NS_Luong_PhuCap", "ID_DonVi", c => c.Guid());
            AddColumn("dbo.NS_LoaiLuong", "LoaiLuong", c => c.Int());
            CreateIndex("dbo.NS_BangLuong", "ID_DonVi");
            CreateIndex("dbo.NS_NgayNghiLe", "ID_BangLuong");
            CreateIndex("dbo.NS_Luong_PhuCap", "ID_DonVi");
            AddForeignKey("dbo.NS_NgayNghiLe", "ID_BangLuong", "dbo.NS_BangLuong", "ID");
            AddForeignKey("dbo.NS_BangLuong", "ID_DonVi", "dbo.DM_DonVi", "ID");
            AddForeignKey("dbo.NS_Luong_PhuCap", "ID_DonVi", "dbo.DM_DonVi", "ID");
            DropColumn("dbo.NS_BangLuong_ChiTiet", "ID_BangCong_ChiTiet");
            DropColumn("dbo.NS_BangLuong_ChiTiet", "ID_CaLamViec");
            DropColumn("dbo.NS_BangLuong", "ID_KyTinhCong");
            DropColumn("dbo.NS_KyHieuCong", "ID_KyTinhCong");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "SoNgayNghi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongCong");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "PhutDiMuon");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "SoGioOT");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongGioOTQuyDoi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongCongQuyDoi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghiQuyDoi");
            DropColumn("dbo.NS_NgayNghiLe", "ID_KyTinhCong");

            DropForeignKey("dbo.NS_NgayNghiLe", "ID_BangLuong", "dbo.NS_BangLuong");
            DropIndex("dbo.NS_NgayNghiLe", new[] { "ID_BangLuong" });
            CreateTable(
                "dbo.NS_BangLuongOTChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_BangLuongChiTiet = c.Guid(nullable: false),
                    ID_CaLamViec = c.Guid(nullable: false),
                    LoaiLuong = c.Int(nullable: false),
                    LoaiNgay = c.Int(nullable: false),
                    SoTien = c.Double(nullable: false),
                    HeSoLuongOT = c.Double(nullable: false),
                    ThanhTien = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_CaLamViec", t => t.ID_CaLamViec, cascadeDelete: true)
                .ForeignKey("dbo.NS_BangLuong_ChiTiet", t => t.ID_BangLuongChiTiet, cascadeDelete: true)
                .Index(t => t.ID_BangLuongChiTiet)
                .Index(t => t.ID_CaLamViec);

            CreateTable(
                "dbo.NS_ThietLapLuongChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_LuongPhuCap = c.Guid(nullable: false),
                    LuongNgayThuong = c.Double(nullable: false),
                    Thu7_LaPhanTramLuong = c.Int(nullable: false),
                    Thu7_GiaTri = c.Double(nullable: false),
                    CN_LaPhanTramLuong = c.Int(nullable: false),
                    ThCN_GiaTri = c.Double(nullable: false),
                    NgayNghi_LaPhanTramLuong = c.Int(nullable: false),
                    NgayNghi_GiaTri = c.Double(nullable: false),
                    NgayLe_LaPhanTramLuong = c.Int(nullable: false),
                    NgayLe_GiaTri = c.Double(nullable: false),
                    LaOT = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_Luong_PhuCap", t => t.ID_LuongPhuCap, cascadeDelete: true)
                .Index(t => t.ID_LuongPhuCap);

            DropColumn("dbo.NS_NgayNghiLe", "ID_BangLuong");
            DropColumn("dbo.NS_NgayNghiLe", "CongQuyDoi");
            DropColumn("dbo.NS_NgayNghiLe", "HeSoLuong");
            DropColumn("dbo.NS_NgayNghiLe", "HeSoLuongOT");
            DropColumn("dbo.NS_CongBoSung", "HeSoLuong");
            DropColumn("dbo.NS_CongBoSung", "HeSoLuongOT");

            AddColumn("dbo.NS_ThietLapLuongChiTiet", "ID_CaLamViec", c => c.Guid());
            CreateIndex("dbo.NS_ThietLapLuongChiTiet", "ID_CaLamViec");
            AddForeignKey("dbo.NS_ThietLapLuongChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec", "ID");

            AddColumn("dbo.NS_ThietLapLuongChiTiet", "NgayThuong_LaPhanTramLuong", c => c.Int(nullable: false));

            AddColumn("dbo.Quy_HoaDon_ChiTiet", "ID_BangLuongChiTiet", c => c.Guid());
            AddColumn("dbo.NS_BangLuong_ChiTiet", "MaBangLuongChiTiet", c => c.String());
            AddColumn("dbo.NS_KyHieuCong", "ID_DonVi", c => c.Guid());
            AddColumn("dbo.NS_CongBoSung", "ID_NhanVien", c => c.Guid());
            AddColumn("dbo.NS_CongBoSung", "ID_CaLamViec", c => c.Guid());
            AddColumn("dbo.NS_CongBoSung", "ID_DonVi", c => c.Guid());
            AddColumn("dbo.NS_CongBoSung", "ID_BangLuongChiTiet", c => c.Guid());
            CreateIndex("dbo.Quy_HoaDon_ChiTiet", "ID_BangLuongChiTiet");
            CreateIndex("dbo.NS_KyHieuCong", "ID_DonVi");
            CreateIndex("dbo.NS_CongBoSung", "ID_NhanVien");
            CreateIndex("dbo.NS_CongBoSung", "ID_CaLamViec");
            CreateIndex("dbo.NS_CongBoSung", "ID_DonVi");
            CreateIndex("dbo.NS_CongBoSung", "ID_BangLuongChiTiet");
            AddForeignKey("dbo.NS_CongBoSung", "ID_CaLamViec", "dbo.NS_CaLamViec", "ID");
            AddForeignKey("dbo.NS_CongBoSung", "ID_BangLuongChiTiet", "dbo.NS_BangLuong_ChiTiet", "ID");
            AddForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_BangLuongChiTiet", "dbo.NS_BangLuong_ChiTiet", "ID");
            AddForeignKey("dbo.NS_CongBoSung", "ID_DonVi", "dbo.DM_DonVi", "ID");
            AddForeignKey("dbo.NS_KyHieuCong", "ID_DonVi", "dbo.DM_DonVi", "ID");
            AddForeignKey("dbo.NS_CongBoSung", "ID_NhanVien", "dbo.NS_NhanVien", "ID");

            DropForeignKey("dbo.NS_ChamCong", "ID_KyHieu", "dbo.NS_KyHieuCong");
            DropForeignKey("dbo.NS_ChamCong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_ChamCong", "ID_MaChamCong", "dbo.NS_MaChamCong");
            DropForeignKey("dbo.NS_CongBoSung", "ID_ChamCongChiTiet", "dbo.NS_ChamCong_ChiTiet");
            DropForeignKey("dbo.NS_ChamCong", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_ChamCong", "ID_NhanVien", "dbo.NS_NhanVien");
            DropIndex("dbo.NS_ChamCong", new[] { "ID_MaChamCong" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_KyHieu" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_CongBoSung", new[] { "ID_ChamCongChiTiet" });
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_DonVi" });
            DropTable("dbo.NS_ChamCong");
            DropTable("dbo.NS_ChamCong_ChiTiet");

            CreateTable(
                "dbo.NS_CongNoTamUngLuong",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_DonVi = c.Guid(nullable: false),
                        ID_NhanVien = c.Guid(nullable: false),
                        CongNo = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_NhanVien);
            
            AddColumn("dbo.Quy_HoaDon_ChiTiet", "TruTamUngLuong", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AddColumn("dbo.NS_NgayNghiLe", "ID_KyTinhCong", c => c.Guid());
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghiQuyDoi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongCongQuyDoi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongGioOTQuyDoi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "SoGioOT", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "PhutDiMuon", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongCong", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "SoNgayNghi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong", c => c.Guid(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong", c => c.Guid());
            AddColumn("dbo.NS_KyHieuCong", "ID_KyTinhCong", c => c.Guid());
            AddColumn("dbo.NS_BangLuong", "ID_KyTinhCong", c => c.Guid(nullable: false));
            AddColumn("dbo.NS_BangLuong_ChiTiet", "ID_CaLamViec", c => c.Guid(nullable: false));
            AddColumn("dbo.NS_BangLuong_ChiTiet", "ID_BangCong_ChiTiet", c => c.Guid(nullable: false));
            DropForeignKey("dbo.NS_Luong_PhuCap", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_BangLuong", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_NgayNghiLe", "ID_BangLuong", "dbo.NS_BangLuong");
            DropIndex("dbo.NS_Luong_PhuCap", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_NgayNghiLe", new[] { "ID_BangLuong" });
            DropIndex("dbo.NS_BangLuong", new[] { "ID_DonVi" });
            DropColumn("dbo.NS_LoaiLuong", "LoaiLuong");
            DropColumn("dbo.NS_Luong_PhuCap", "ID_DonVi");
            DropColumn("dbo.NS_NgayNghiLe", "ID_BangLuong");
            DropColumn("dbo.NS_CongBoSung", "HeSoLuongOT");
            DropColumn("dbo.NS_CongBoSung", "HeSoLuong");
            DropColumn("dbo.NS_BangLuong", "DenNgay");
            DropColumn("dbo.NS_BangLuong", "TuNgay");
            DropColumn("dbo.NS_BangLuong", "ID_DonVi");
            CreateIndex("dbo.NS_NgayNghiLe", "ID_KyTinhCong");
            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong");
            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong");
            CreateIndex("dbo.NS_KyHieuCong", "ID_KyTinhCong");
            CreateIndex("dbo.NS_BangLuong", "ID_KyTinhCong");
            CreateIndex("dbo.NS_BangLuong_ChiTiet", "ID_CaLamViec");
            AddForeignKey("dbo.NS_BangLuong_ChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_NgayNghiLe", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID");
            AddForeignKey("dbo.NS_KyHieuCong", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID");
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong", "dbo.NS_MaChamCong", "ID");
            AddForeignKey("dbo.NS_BangLuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID", cascadeDelete: true);

            AddColumn("dbo.NS_CongBoSung", "HeSoLuongOT", c => c.Double());
            AddColumn("dbo.NS_CongBoSung", "HeSoLuong", c => c.Double());
            AddColumn("dbo.NS_NgayNghiLe", "HeSoLuongOT", c => c.Double(nullable: false));
            AddColumn("dbo.NS_NgayNghiLe", "HeSoLuong", c => c.Double(nullable: false));
            AddColumn("dbo.NS_NgayNghiLe", "CongQuyDoi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_NgayNghiLe", "ID_BangLuong", c => c.Guid());
            DropForeignKey("dbo.NS_ThietLapLuongChiTiet", "ID_LuongPhuCap", "dbo.NS_Luong_PhuCap");
            DropForeignKey("dbo.NS_BangLuongOTChiTiet", "ID_BangLuongChiTiet", "dbo.NS_BangLuong_ChiTiet");
            DropForeignKey("dbo.NS_BangLuongOTChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropIndex("dbo.NS_ThietLapLuongChiTiet", new[] { "ID_LuongPhuCap" });
            DropIndex("dbo.NS_BangLuongOTChiTiet", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_BangLuongOTChiTiet", new[] { "ID_BangLuongChiTiet" });
            DropTable("dbo.NS_ThietLapLuongChiTiet");
            DropTable("dbo.NS_BangLuongOTChiTiet");
            CreateIndex("dbo.NS_NgayNghiLe", "ID_BangLuong");
            AddForeignKey("dbo.NS_NgayNghiLe", "ID_BangLuong", "dbo.NS_BangLuong", "ID");

            DropForeignKey("dbo.NS_ThietLapLuongChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropIndex("dbo.NS_ThietLapLuongChiTiet", new[] { "ID_CaLamViec" });
            DropColumn("dbo.NS_ThietLapLuongChiTiet", "ID_CaLamViec");

            DropColumn("dbo.NS_ThietLapLuongChiTiet", "NgayThuong_LaPhanTramLuong");

            DropForeignKey("dbo.NS_CongBoSung", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_KyHieuCong", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_CongBoSung", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_BangLuongChiTiet", "dbo.NS_BangLuong_ChiTiet");
            DropForeignKey("dbo.NS_CongBoSung", "ID_BangLuongChiTiet", "dbo.NS_BangLuong_ChiTiet");
            DropForeignKey("dbo.NS_CongBoSung", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropIndex("dbo.NS_CongBoSung", new[] { "ID_BangLuongChiTiet" });
            DropIndex("dbo.NS_CongBoSung", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_CongBoSung", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_CongBoSung", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_KyHieuCong", new[] { "ID_DonVi" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_BangLuongChiTiet" });
            DropColumn("dbo.NS_CongBoSung", "ID_BangLuongChiTiet");
            DropColumn("dbo.NS_CongBoSung", "ID_DonVi");
            DropColumn("dbo.NS_CongBoSung", "ID_CaLamViec");
            DropColumn("dbo.NS_CongBoSung", "ID_NhanVien");
            DropColumn("dbo.NS_KyHieuCong", "ID_DonVi");
            DropColumn("dbo.NS_BangLuong_ChiTiet", "MaBangLuongChiTiet");
            DropColumn("dbo.Quy_HoaDon_ChiTiet", "ID_BangLuongChiTiet");

            CreateTable(
                "dbo.NS_ChamCong_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_CaLamViec = c.Guid(nullable: false),
                    LoaiCong = c.Int(nullable: false),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                    Thang = c.Int(nullable: false),
                    Nam = c.Int(nullable: false),
                    Ngay1 = c.String(maxLength: 100),
                    Ngay2 = c.String(maxLength: 100),
                    Ngay3 = c.String(maxLength: 100),
                    Ngay4 = c.String(maxLength: 100),
                    Ngay5 = c.String(maxLength: 100),
                    Ngay6 = c.String(maxLength: 100),
                    Ngay7 = c.String(maxLength: 100),
                    Ngay8 = c.String(maxLength: 100),
                    Ngay9 = c.String(maxLength: 100),
                    Ngay10 = c.String(maxLength: 100),
                    Ngay11 = c.String(maxLength: 100),
                    Ngay12 = c.String(maxLength: 100),
                    Ngay13 = c.String(maxLength: 100),
                    Ngay14 = c.String(maxLength: 100),
                    Ngay15 = c.String(maxLength: 100),
                    Ngay16 = c.String(maxLength: 100),
                    Ngay17 = c.String(maxLength: 100),
                    Ngay18 = c.String(maxLength: 100),
                    Ngay19 = c.String(maxLength: 100),
                    Ngay20 = c.String(maxLength: 100),
                    Ngay21 = c.String(maxLength: 100),
                    Ngay22 = c.String(maxLength: 100),
                    Ngay23 = c.String(maxLength: 100),
                    Ngay24 = c.String(maxLength: 100),
                    Ngay25 = c.String(maxLength: 100),
                    Ngay26 = c.String(maxLength: 100),
                    Ngay27 = c.String(maxLength: 100),
                    Ngay28 = c.String(maxLength: 100),
                    Ngay29 = c.String(maxLength: 100),
                    Ngay30 = c.String(maxLength: 100),
                    Ngay31 = c.String(maxLength: 100),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ID_DonVi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_ChamCong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_MaChamCong = c.Guid(),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_KyTinhCong = c.Guid(nullable: false),
                    ID_KyHieu = c.Guid(nullable: false),
                    ID_CaLamViec = c.Guid(nullable: false),
                    NgayThucHien = c.DateTime(nullable: false),
                    NgayCham = c.DateTime(nullable: false),
                    LoaiCong = c.Int(nullable: false),
                    GioVaoCa = c.DateTime(),
                    GioRaCa = c.DateTime(),
                    GioVaoOT = c.DateTime(),
                    GioRaOT = c.DateTime(),
                    SoGioOTBanNgay = c.Double(nullable: false),
                    SoGioOTBanDem = c.Double(nullable: false),
                    SoPhutDiMuon = c.Int(nullable: false),
                    SoPhutVeSom = c.Int(nullable: false),
                    GhiChu = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_DonVi");
            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec");
            CreateIndex("dbo.NS_CongBoSung", "ID_ChamCongChiTiet");
            CreateIndex("dbo.NS_ChamCong", "ID_CaLamViec");
            CreateIndex("dbo.NS_ChamCong", "ID_KyHieu");
            CreateIndex("dbo.NS_ChamCong", "ID_KyTinhCong");
            CreateIndex("dbo.NS_ChamCong", "ID_NhanVien");
            CreateIndex("dbo.NS_ChamCong", "ID_MaChamCong");
            AddForeignKey("dbo.NS_ChamCong", "ID_NhanVien", "dbo.NS_NhanVien", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_DonVi", "dbo.DM_DonVi", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_ChamCong", "ID_CaLamViec", "dbo.NS_CaLamViec", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_CongBoSung", "ID_ChamCongChiTiet", "dbo.NS_ChamCong_ChiTiet", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_ChamCong", "ID_MaChamCong", "dbo.NS_MaChamCong", "ID");
            AddForeignKey("dbo.NS_ChamCong", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_ChamCong", "ID_KyHieu", "dbo.NS_KyHieuCong", "ID", cascadeDelete: true);

            DropForeignKey("dbo.NS_CongNoTamUngLuong", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_CongNoTamUngLuong", "ID_DonVi", "dbo.DM_DonVi");
            DropIndex("dbo.NS_CongNoTamUngLuong", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_CongNoTamUngLuong", new[] { "ID_DonVi" });
            DropColumn("dbo.Quy_HoaDon_ChiTiet", "TruTamUngLuong");
            DropTable("dbo.NS_CongNoTamUngLuong");
        }
    }
}
