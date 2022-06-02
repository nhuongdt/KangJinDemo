namespace Model.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DM_Anh_Slider",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Text = c.String(),
                        Link = c.String(),
                        ThuTuHienThi = c.Int(),
                        Mota = c.String(),
                        TrangThai = c.Boolean(),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DM_BaiViet",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenBaiViet = c.String(),
                        NoiDung = c.String(),
                        Anh = c.String(),
                        ID_NhomBaiViet = c.Int(nullable: false),
                        MetaDescriptions = c.String(),
                        MetaTitle = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiTao = c.String(),
                        NgaySua = c.DateTime(),
                        NguoiSua = c.String(),
                        ThuTuHienThi = c.Int(),
                        TrangThai = c.Boolean(),
                        Link = c.String(),
                        LuotXem = c.Int(),
                        NgayDangBai = c.DateTime(),
                        Mota = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomBaiViet", t => t.ID_NhomBaiViet, cascadeDelete: true)
                .Index(t => t.ID_NhomBaiViet);
            
            CreateTable(
                "dbo.DM_NhomBaiViet",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenNhomBaiViet = c.String(),
                        LoaiNhomBaiViet = c.Int(nullable: false),
                        GhiChu = c.String(),
                        ID_NhomCha = c.Int(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiTao = c.String(),
                        NgaySua = c.DateTime(),
                        NguoiSua = c.String(),
                        Link = c.String(),
                        TrangThai = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomBaiViet", t => t.ID_NhomCha)
                .Index(t => t.ID_NhomCha);
            
            CreateTable(
                "dbo.DM_TuyenDung",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ID_NhomBaiViet = c.Int(nullable: false),
                        TieuDe = c.String(),
                        MoTa = c.String(),
                        QuyenLoi = c.String(),
                        DiaChi = c.String(),
                        MetaDescription = c.String(),
                        MetaTitle = c.String(),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                        MucLuong = c.String(),
                        TuNgay = c.DateTime(nullable: false),
                        Link = c.String(),
                        DenNgay = c.DateTime(nullable: false),
                        NgayDangBai = c.DateTime(nullable: false),
                        TrangThai = c.Int(),
                        SoLuong = c.Int(),
                        MaTinhThanh = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomBaiViet", t => t.ID_NhomBaiViet, cascadeDelete: true)
                .Index(t => t.ID_NhomBaiViet);
            
            CreateTable(
                "dbo.DM_BaiViet_Tag",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_Tag = c.String(nullable: false, maxLength: 128),
                        ID_BaiViet = c.Int(nullable: false),
                        Loai = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_Tags", t => t.ID_Tag, cascadeDelete: true)
                .Index(t => t.ID_Tag);
            
            CreateTable(
                "dbo.DM_Tags",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        TenTheTag = c.String(),
                        KeyWordTag = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DM_DonHang",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TenKhachHang = c.String(),
                        SoDienThoai = c.String(),
                        DiaChi = c.String(),
                        Email = c.String(),
                        TrangThai = c.Int(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiTao = c.String(),
                        NgaySua = c.DateTime(),
                        NguoiSua = c.String(),
                        DaThanhToan = c.Double(nullable: false),
                        ConNo = c.Double(nullable: false),
                        TenNguoiNhan = c.String(),
                        SoDienThoaiNguoiNhan = c.String(),
                        DiaChiNguoiNhan = c.String(),
                        EmailNguoiNhan = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DM_DonHangChiTiet",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_DonHang = c.Guid(nullable: false),
                        ID_SanPham = c.Guid(nullable: false),
                        SoLuong = c.Double(nullable: false),
                        DonGia = c.Double(nullable: false),
                        GhiChu = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_SanPham", t => t.ID_SanPham, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonHang", t => t.ID_DonHang, cascadeDelete: true)
                .Index(t => t.ID_DonHang)
                .Index(t => t.ID_SanPham);
            
            CreateTable(
                "dbo.DM_SanPham",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TenSanPham = c.String(),
                        MaSanPham = c.String(),
                        GhiChu = c.String(),
                        Anh = c.String(),
                        ListAnh = c.String(),
                        GiaNhap = c.Double(nullable: false),
                        GiaBan = c.Double(nullable: false),
                        SoLuong = c.Double(nullable: false),
                        ID_NhomSanPham = c.Guid(nullable: false),
                        MetaKeywords = c.String(),
                        MetaDescription = c.String(),
                        TrangThai = c.Int(),
                        TopHot = c.DateTime(),
                        SoLuotXem = c.Int(),
                        QuanLyKhoHang = c.Boolean(),
                        LyDoKM = c.String(),
                        GiaKM = c.Double(nullable: false),
                        NhaCungCap = c.String(),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomSanPham", t => t.ID_NhomSanPham, cascadeDelete: true)
                .Index(t => t.ID_NhomSanPham);
            
            CreateTable(
                "dbo.DM_NhomSanPham",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TenNhomSanPham = c.String(),
                        MaNhomSanPham = c.String(),
                        ID_NhomCha = c.Guid(),
                        ViTriHienThi = c.Int(nullable: false),
                        MetaTitle = c.String(),
                        MetaKeywords = c.String(),
                        MetaDescription = c.String(),
                        TrangThai = c.Boolean(),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomSanPham", t => t.ID_NhomCha)
                .Index(t => t.ID_NhomCha);
            
            CreateTable(
                "dbo.DM_KhachHang",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TenKhachHang = c.String(),
                        DiaChi = c.String(),
                        Email = c.String(),
                        SoDienThoai = c.String(),
                        MaTinhThanh = c.String(),
                        ID_SanPham = c.String(),
                        Mota = c.String(),
                        NoiDung = c.String(),
                        MetaTitle = c.String(),
                        MetaDescription = c.String(),
                        TrangThai = c.Int(),
                        Anh = c.String(),
                        Link = c.String(),
                        NguoiTao = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiSua = c.String(),
                        NgaySua = c.DateTime(),
                        SoLuotXem = c.Int(nullable: false),
                        HienThiTrangChu = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DM_LienHe",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TenNguoiLienHe = c.String(),
                        Email = c.String(),
                        DiaChi = c.String(),
                        GhiChu = c.String(),
                        SoDienThoai = c.String(),
                        TrangThai = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DM_Menu",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        DuongDan = c.String(),
                        Link = c.String(),
                        ThuTuHienThi = c.Int(),
                        Description = c.String(),
                        TrangThai = c.Boolean(),
                        ID_Loaimenu = c.Int(nullable: false),
                        KeyWord = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DM_TinhThanh",
                c => new
                    {
                        MaTinhThanh = c.String(nullable: false, maxLength: 128),
                        TenTinhThanh = c.String(),
                        GhiChu = c.String(),
                    })
                .PrimaryKey(t => t.MaTinhThanh);
            
            CreateTable(
                "dbo.HT_NguoiDung",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TaiKhoan = c.String(),
                        MatKhau = c.String(),
                        TenNguoiDung = c.String(),
                        DiaChi = c.String(),
                        DienThoai = c.String(),
                        Email = c.String(),
                        LaAdmin = c.Boolean(nullable: false),
                        SinhNhat = c.DateTime(),
                        ID_NhomNguoiDung = c.Guid(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiTao = c.String(),
                        NgaySua = c.DateTime(),
                        NguoiSua = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_NhomNguoiDung", t => t.ID_NhomNguoiDung, cascadeDelete: true)
                .Index(t => t.ID_NhomNguoiDung);
            
            CreateTable(
                "dbo.HT_NhomNguoiDung",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TenNhomNguoiDung = c.String(),
                        GhiChu = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        NguoiTao = c.String(),
                        NgaySua = c.DateTime(),
                        NguoiSua = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.HT_NhomNguoiDung_Quyen",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_NhomNguoiDung = c.Guid(nullable: false),
                        MaQuyen = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_Quyen", t => t.MaQuyen, cascadeDelete: true)
                .ForeignKey("dbo.HT_NhomNguoiDung", t => t.ID_NhomNguoiDung, cascadeDelete: true)
                .Index(t => t.ID_NhomNguoiDung)
                .Index(t => t.MaQuyen);
            
            CreateTable(
                "dbo.HT_Quyen",
                c => new
                    {
                        MaQuyen = c.String(nullable: false, maxLength: 128),
                        TenQuyen = c.String(),
                        MoTa = c.String(),
                        DuocSuDung = c.Boolean(nullable: false),
                        QuyenCha = c.String(),
                    })
                .PrimaryKey(t => t.MaQuyen);
            
            CreateTable(
                "dbo.HT_ThongTinCuaHang",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TenKhachHang = c.String(),
                        SoDienThoai = c.String(),
                        Email = c.String(),
                        PageTinTucHome = c.Int(),
                        PageSanPhamHome = c.Int(),
                        PageTinTuc = c.Int(),
                        PageKhachHangHome = c.Int(),
                        PageKhachHang = c.Int(),
                        DiaChi = c.String(),
                        Theme = c.Int(),
                        AnhLogo = c.String(),
                        ApiMap = c.String(),
                        LinkPageFacebook = c.String(),
                        TenCuaHang = c.String(),
                        DomainOpen = c.String(),
                        APIKey = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HT_NhomNguoiDung_Quyen", "ID_NhomNguoiDung", "dbo.HT_NhomNguoiDung");
            DropForeignKey("dbo.HT_NhomNguoiDung_Quyen", "MaQuyen", "dbo.HT_Quyen");
            DropForeignKey("dbo.HT_NguoiDung", "ID_NhomNguoiDung", "dbo.HT_NhomNguoiDung");
            DropForeignKey("dbo.DM_DonHangChiTiet", "ID_DonHang", "dbo.DM_DonHang");
            DropForeignKey("dbo.DM_SanPham", "ID_NhomSanPham", "dbo.DM_NhomSanPham");
            DropForeignKey("dbo.DM_NhomSanPham", "ID_NhomCha", "dbo.DM_NhomSanPham");
            DropForeignKey("dbo.DM_DonHangChiTiet", "ID_SanPham", "dbo.DM_SanPham");
            DropForeignKey("dbo.DM_BaiViet_Tag", "ID_Tag", "dbo.DM_Tags");
            DropForeignKey("dbo.DM_TuyenDung", "ID_NhomBaiViet", "dbo.DM_NhomBaiViet");
            DropForeignKey("dbo.DM_NhomBaiViet", "ID_NhomCha", "dbo.DM_NhomBaiViet");
            DropForeignKey("dbo.DM_BaiViet", "ID_NhomBaiViet", "dbo.DM_NhomBaiViet");
            DropIndex("dbo.HT_NhomNguoiDung_Quyen", new[] { "MaQuyen" });
            DropIndex("dbo.HT_NhomNguoiDung_Quyen", new[] { "ID_NhomNguoiDung" });
            DropIndex("dbo.HT_NguoiDung", new[] { "ID_NhomNguoiDung" });
            DropIndex("dbo.DM_NhomSanPham", new[] { "ID_NhomCha" });
            DropIndex("dbo.DM_SanPham", new[] { "ID_NhomSanPham" });
            DropIndex("dbo.DM_DonHangChiTiet", new[] { "ID_SanPham" });
            DropIndex("dbo.DM_DonHangChiTiet", new[] { "ID_DonHang" });
            DropIndex("dbo.DM_BaiViet_Tag", new[] { "ID_Tag" });
            DropIndex("dbo.DM_TuyenDung", new[] { "ID_NhomBaiViet" });
            DropIndex("dbo.DM_NhomBaiViet", new[] { "ID_NhomCha" });
            DropIndex("dbo.DM_BaiViet", new[] { "ID_NhomBaiViet" });
            DropTable("dbo.HT_ThongTinCuaHang");
            DropTable("dbo.HT_Quyen");
            DropTable("dbo.HT_NhomNguoiDung_Quyen");
            DropTable("dbo.HT_NhomNguoiDung");
            DropTable("dbo.HT_NguoiDung");
            DropTable("dbo.DM_TinhThanh");
            DropTable("dbo.DM_Menu");
            DropTable("dbo.DM_LienHe");
            DropTable("dbo.DM_KhachHang");
            DropTable("dbo.DM_NhomSanPham");
            DropTable("dbo.DM_SanPham");
            DropTable("dbo.DM_DonHangChiTiet");
            DropTable("dbo.DM_DonHang");
            DropTable("dbo.DM_Tags");
            DropTable("dbo.DM_BaiViet_Tag");
            DropTable("dbo.DM_TuyenDung");
            DropTable("dbo.DM_NhomBaiViet");
            DropTable("dbo.DM_BaiViet");
            DropTable("dbo.DM_Anh_Slider");
        }
    }
}
