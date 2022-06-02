namespace Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate2018 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BH_HoaDon",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaHoaDon = c.String(nullable: false, maxLength: 50),
                    NgayLapHoaDon = c.DateTime(nullable: false),
                    GioVao = c.DateTime(),
                    GioRa = c.DateTime(),
                    ID_ViTri = c.Guid(),
                    ID_CheckIn = c.Guid(),
                    ID_DoiTuong = c.Guid(),
                    ID_NgoaiTe = c.Guid(),
                    ID_BangGia = c.Guid(),
                    ID_NhanVien = c.Guid(),
                    ID_HoaDon = c.Guid(),
                    LoaiHoaDon = c.Int(nullable: false),
                    ChoThanhToan = c.Boolean(),
                    TongTienHang = c.Double(nullable: false),
                    TongChietKhau = c.Double(nullable: false),
                    TongTienThue = c.Double(nullable: false),
                    TongGiamGia = c.Double(nullable: false),
                    TongChiPhi = c.Double(nullable: false),
                    PhaiThanhToan = c.Double(nullable: false),
                    DienGiai = c.String(),
                    SoLanIn = c.Int(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NgaySua = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    ID_DonVi = c.Guid(nullable: false),
                    YeuCau = c.String(),
                    An_Hien = c.Boolean(),
                    TyGia = c.Double(),
                    ID_KhuyenMai = c.Guid(),
                    KhuyeMai_GiamGia = c.Double(),
                    KhuyenMai_GhiChu = c.String(),
                    DiemGiaoDich = c.Double(),
                    NgayApDungGoiDV = c.DateTime(),
                    HanSuDungGoiDV = c.DateTime(),
                    SoLuongKhachHang = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.DM_GiaBan", t => t.ID_BangGia)
                .ForeignKey("dbo.DM_TienTe", t => t.ID_NgoaiTe)
                .ForeignKey("dbo.DM_LoaiChungTu", t => t.LoaiHoaDon, cascadeDelete: true)
                .ForeignKey("dbo.DM_ViTri", t => t.ID_ViTri)
                .ForeignKey("dbo.DM_KhuyenMai", t => t.ID_KhuyenMai)
                .Index(t => t.ID_ViTri)
                .Index(t => t.ID_DoiTuong)
                .Index(t => t.ID_NgoaiTe)
                .Index(t => t.ID_BangGia)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.LoaiHoaDon)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_KhuyenMai);

            CreateTable(
                "dbo.BH_HoaDon_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_HoaDon = c.Guid(nullable: false),
                    SoThuTu = c.Int(nullable: false),
                    ThoiGian = c.DateTime(),
                    ThoiGianBaoHanh = c.Double(),
                    LoaiThoiGianBH = c.Int(),
                    ID_KhoHang = c.Guid(),
                    ID_LoHang = c.Guid(),
                    ChatLieu = c.String(maxLength: 250),
                    MauSac = c.String(maxLength: 250),
                    KichCo = c.String(maxLength: 250),
                    SoLuong = c.Double(nullable: false),
                    DonGia = c.Double(nullable: false),
                    ThanhTien = c.Double(nullable: false),
                    PTChietKhau = c.Double(nullable: false),
                    TienChietKhau = c.Double(nullable: false),
                    ID_ThueSuat = c.Guid(),
                    TienThue = c.Double(nullable: false),
                    PTChiPhi = c.Double(nullable: false),
                    TienChiPhi = c.Double(nullable: false),
                    ThanhToan = c.Double(nullable: false),
                    GiaVon = c.Double(),
                    GhiChu = c.String(),
                    SoLanDaIn = c.Double(),
                    ID_TangKem = c.Guid(),
                    TangKem = c.Boolean(),
                    ThoiGianThucHien = c.Double(),
                    SoLuong_TL = c.Double(),
                    SoLuong_YC = c.Double(),
                    Chieu = c.Boolean(),
                    Sang = c.Boolean(),
                    PTThue = c.Double(),
                    TonLuyKe = c.Double(),
                    TonLuyKe_NhanChuyenHang = c.Double(),
                    An_Hien = c.Boolean(nullable: false),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                    Bep_SoLuongYeuCau = c.Double(),
                    Bep_SoLuongChoCungUng = c.Double(),
                    Bep_SoLuongHoanThanh = c.Double(),
                    ID_KhuyenMai = c.Guid(),
                    GiaVon_NhanChuyenHang = c.Double(),
                    ID_ChiTietDinhLuong = c.Guid(),
                    ID_ChiTietGoiDV = c.Guid(),
                    SoLuongDinhLuong_BanDau = c.Double(),
                    ID_ViTri = c.Guid(),
                    ThoiGianHoanThanh = c.DateTime(),
                    QuaThoiGian = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .ForeignKey("dbo.DM_Kho", t => t.ID_KhoHang)
                .ForeignKey("dbo.DM_LoHang", t => t.ID_LoHang)
                .ForeignKey("dbo.BH_HoaDon", t => t.ID_HoaDon)
                .ForeignKey("dbo.DM_KhuyenMai", t => t.ID_KhuyenMai)
                .ForeignKey("dbo.DM_ViTri", t => t.ID_ViTri)
                .Index(t => t.ID_HoaDon)
                .Index(t => t.ID_KhoHang)
                .Index(t => t.ID_LoHang)
                .Index(t => t.ID_DonViQuiDoi)
                .Index(t => t.ID_KhuyenMai)
                .Index(t => t.ID_ViTri);

            CreateTable(
                "dbo.BH_NhanVienThucHien",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_ChiTietHoaDon = c.Guid(),
                    TienChietKhau = c.Double(nullable: false),
                    TheoYeuCau = c.Boolean(nullable: false),
                    PT_ChietKhau = c.Double(nullable: false),
                    ThucHien_TuVan = c.Boolean(nullable: false),
                    ID_HoaDon = c.Guid(),
                    TinhChietKhauTheo = c.Int(),
                    HeSo = c.Double(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .ForeignKey("dbo.BH_HoaDon_ChiTiet", t => t.ID_ChiTietHoaDon)
                .ForeignKey("dbo.BH_HoaDon", t => t.ID_HoaDon)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_ChiTietHoaDon)
                .Index(t => t.ID_HoaDon);

            CreateTable(
                "dbo.NS_PhongBan",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenPhongBan = c.String(),
                    ID_PhongBanCha = c.Guid(),
                    TrangThai = c.Int(nullable: false),
                    MaPhongBan = c.String(),
                    ID_DonVi = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_PhongBan", t => t.ID_PhongBanCha)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_PhongBanCha)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.NS_NhanVien",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaNhanVien = c.String(nullable: false, maxLength: 50),
                    TenNhanVien = c.String(nullable: false),
                    NgaySinh = c.DateTime(),
                    GioiTinh = c.Boolean(nullable: false),
                    NoiSinh = c.String(),
                    NguyenQuan = c.String(),
                    ThuongTru = c.String(),
                    TamTru = c.String(),
                    DienThoaiCoQuan = c.String(maxLength: 50),
                    DienThoaiNhaRieng = c.String(maxLength: 50),
                    DienThoaiDiDong = c.String(maxLength: 50),
                    SoFax = c.String(maxLength: 50),
                    DiaChiCoQuan = c.String(),
                    SoCMND = c.String(maxLength: 50),
                    SoBHXH = c.String(maxLength: 50),
                    Email = c.String(maxLength: 50),
                    Website = c.String(maxLength: 50),
                    CapTaiKhoan = c.Boolean(),
                    GhiChu = c.String(),
                    DaNghiViec = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    NgayCap = c.DateTime(),
                    NoiCap = c.String(),
                    ID_NSPhongBan = c.Guid(),
                    ID_TinhThanhTT = c.Guid(),
                    ID_QuanHuyenTT = c.Guid(),
                    ID_XaPhuongTT = c.Guid(),
                    ID_TinhThanhHKTT = c.Guid(),
                    ID_QuanHuyenHKTT = c.Guid(),
                    ID_XaPhuongHKTT = c.Guid(),
                    NgayVaoLamViec = c.DateTime(),
                    DiaChiTT = c.String(),
                    DiaChiHKTT = c.String(),
                    DanTocTonGiao = c.String(),
                    TonGiao = c.String(),
                    NoiVaoDoan = c.String(),
                    NgayVaoDoan = c.DateTime(),
                    NoiSinhHoatDang = c.String(),
                    NgayVaoDang = c.DateTime(),
                    NgayVaoDangChinhThuc = c.DateTime(),
                    NgayRoiDang = c.DateTime(),
                    LyDoRoiDang = c.String(),
                    NgayNhapNgu = c.DateTime(),
                    NgayXuatNgu = c.DateTime(),
                    ChucVuCaoNhat = c.String(),
                    GhiChuThongTinChinhTri = c.String(),
                    ID_QuocGia = c.Guid(),
                    TrangThai = c.Int(),
                    TinhTrangHonNhan = c.Int(),
                    TenNhanVienKhongDau = c.String(),
                    TenNhanVienChuCaiDau = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_XaPhuong", t => t.ID_XaPhuongTT)
                .ForeignKey("dbo.DM_XaPhuong", t => t.ID_XaPhuongHKTT)
                .ForeignKey("dbo.DM_QuanHuyen", t => t.ID_QuanHuyenTT)
                .ForeignKey("dbo.DM_QuanHuyen", t => t.ID_QuanHuyenHKTT)
                .ForeignKey("dbo.DM_TinhThanh", t => t.ID_TinhThanhTT)
                .ForeignKey("dbo.DM_TinhThanh", t => t.ID_TinhThanhHKTT)
                .ForeignKey("dbo.NS_PhongBan", t => t.ID_NSPhongBan)
                .ForeignKey("dbo.DM_QuocGia", t => t.ID_QuocGia)
                .Index(t => t.ID_XaPhuongTT)
                .Index(t => t.ID_XaPhuongHKTT)
                .Index(t => t.ID_QuanHuyenTT)
                .Index(t => t.ID_QuanHuyenHKTT)
                .Index(t => t.ID_TinhThanhTT)
                .Index(t => t.ID_TinhThanhHKTT)
                .Index(t => t.ID_NSPhongBan)
                .Index(t => t.ID_QuocGia);

            CreateTable(
                "dbo.NS_NhanVien_Anh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    SoThuTu = c.Int(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    URLAnh = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.ChamSocKhachHangs",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_KhachHang = c.Guid(),
                    ID_LoaiTuVan = c.Guid(),
                    ID_DonVi = c.Guid(nullable: false),
                    Ma_TieuDe = c.String(),
                    PhanLoai = c.Int(nullable: false),
                    NgayGio = c.DateTime(nullable: false),
                    NgayGioKetThuc = c.DateTime(),
                    NoiDung = c.String(),
                    TraLoi = c.String(),
                    TrangThai = c.String(),
                    NhacNho = c.Int(nullable: false),
                    MucDoPhanHoi = c.Int(nullable: false),
                    ID_NhanVien = c.Guid(),
                    ID_NhanVienQuanLy = c.Guid(nullable: false),
                    NgayTao = c.DateTime(nullable: false),
                    NgaySua = c.DateTime(),
                    NguoiTao = c.String(),
                    NguoiSua = c.String(),
                    ThoiGianHenLai = c.DateTime(),
                    SoLuong = c.Int(),
                    ID_HangHoa = c.Guid(),
                    ID_ChamSocKhachHang = c.Guid(),
                    MucDoUuTien = c.Int(),
                    FileDinhKem = c.String(),
                    NgayHoanThanh = c.DateTime(),
                    KetQua = c.String(),
                    GhiChu = c.String(),
                    ID_LienHe = c.Guid(),
                    CaNgay = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_KhachHang)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienQuanLy)
                .ForeignKey("dbo.DM_LoaiTuVanLichHen", t => t.ID_LoaiTuVan)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .ForeignKey("dbo.ChamSocKhachHangs", t => t.ID_ChamSocKhachHang)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa)
                .ForeignKey("dbo.DM_LienHe", t => t.ID_LienHe)
                .Index(t => t.ID_KhachHang)
                .Index(t => t.ID_LoaiTuVan)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_NhanVienQuanLy)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_ChamSocKhachHang)
                .Index(t => t.ID_HangHoa)
                .Index(t => t.ID_LienHe);

            CreateTable(
                "dbo.DM_DoiTuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    LoaiDoiTuong = c.Int(nullable: false),
                    LaCaNhan = c.Boolean(nullable: false),
                    ID_NhomDoiTuong = c.Guid(),
                    MaDoiTuong = c.String(nullable: false, maxLength: 50),
                    TenDoiTuong = c.String(nullable: false),
                    DienThoai = c.String(),
                    Fax = c.String(),
                    Email = c.String(),
                    Website = c.String(),
                    MaSoThue = c.String(),
                    TaiKhoanNganHang = c.String(),
                    GioiHanCongNo = c.Double(),
                    GhiChu = c.String(),
                    ID_TinhThanh = c.Guid(),
                    NgaySinh_NgayTLap = c.DateTime(),
                    ChiaSe = c.Boolean(nullable: false),
                    TheoDoi = c.Boolean(nullable: false),
                    ID_Index = c.Int(),
                    TheoDoiVanTay = c.Boolean(),
                    ID_QuanHuyen = c.Guid(),
                    ID_NhanVienPhuTrach = c.Guid(),
                    NgayDoiNhom = c.DateTime(),
                    DiemKhoiTao = c.Double(),
                    DoanhSoKhoiTao = c.Double(),
                    ID_NguoiGioiThieu = c.Guid(),
                    ID_NguonKhach = c.Guid(),
                    CapTai_DKKD = c.String(),
                    DiaChi = c.String(),
                    GioiTinhNam = c.Boolean(),
                    NgayCapCMTND_DKKD = c.DateTime(),
                    NoiCapCMTND_DKKD = c.String(),
                    SDT_CoQuan = c.String(maxLength: 255),
                    SDT_NhaRieng = c.String(maxLength: 255),
                    SoCMTND_DKKD = c.String(maxLength: 50),
                    ThuongTru = c.String(),
                    XungHo = c.String(maxLength: 50),
                    ID_QuocGia = c.Guid(),
                    NgayGiaoDichGanNhat = c.DateTime(),
                    ID_NhomCu = c.Guid(),
                    ID_DonVi = c.Guid(),
                    ChucVu = c.String(maxLength: 255),
                    LinhVuc = c.String(maxLength: 255),
                    NgheNghiep = c.String(maxLength: 255),
                    TenKhac = c.String(),
                    ID_NganHang = c.Guid(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    TenDoiTuong_KhongDau = c.String(),
                    TenDoiTuong_ChuCaiDau = c.String(),
                    TongTichDiem = c.Double(),
                    DinhDang_NgaySinh = c.String(),
                    ID_TrangThai = c.Guid(),
                    TrangThai_TheGiaTri = c.Int(),
                    ID_NhanVienGioiThieu = c.Guid(),
                    TenNhomDoiTuongs = c.String(),
                    IDNhomDoiTuongs = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_NguoiGioiThieu)
                .ForeignKey("dbo.DM_NhomDoiTuong", t => t.ID_NhomDoiTuong)
                .ForeignKey("dbo.DM_QuocGia", t => t.ID_QuocGia)
                .ForeignKey("dbo.DM_NganHang", t => t.ID_NganHang)
                .ForeignKey("dbo.DM_TinhThanh", t => t.ID_TinhThanh)
                .ForeignKey("dbo.DM_QuanHuyen", t => t.ID_QuanHuyen)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.DM_NguonKhachHang", t => t.ID_NguonKhach)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienPhuTrach)
                .ForeignKey("dbo.DM_DoiTuong_TrangThai", t => t.ID_TrangThai)
                .Index(t => t.ID_NhomDoiTuong)
                .Index(t => t.ID_TinhThanh)
                .Index(t => t.ID_QuanHuyen)
                .Index(t => t.ID_NhanVienPhuTrach)
                .Index(t => t.ID_NguoiGioiThieu)
                .Index(t => t.ID_NguonKhach)
                .Index(t => t.ID_QuocGia)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_NganHang)
                .Index(t => t.ID_TrangThai);

            CreateTable(
                "dbo.DM_DoiTuong_Anh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    SoThuTu = c.Int(nullable: false),
                    ID_DoiTuong = c.Guid(nullable: false),
                    URLAnh = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong, cascadeDelete: true)
                .Index(t => t.ID_DoiTuong);

            CreateTable(
                "dbo.DM_DonVi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaDonVi = c.String(nullable: false),
                    TenDonVi = c.String(nullable: false),
                    ID_Parent = c.Guid(),
                    DiaChi = c.String(),
                    Website = c.String(),
                    MaSoThue = c.String(),
                    SoTaiKhoan = c.String(),
                    ID_NganHang = c.Guid(),
                    SoDienThoai = c.String(),
                    SoFax = c.String(),
                    KiTuDanhMa = c.String(),
                    HienThi_Chinh = c.Boolean(),
                    HienThi_Phu = c.Boolean(),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                    TrangThai = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NganHang", t => t.ID_NganHang)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_Parent)
                .Index(t => t.ID_Parent)
                .Index(t => t.ID_NganHang);

            CreateTable(
                "dbo.ChietKhauMacDinh_NhanVien",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ChietKhau = c.Double(nullable: false),
                    LaPhanTram = c.Boolean(nullable: false),
                    ChietKhau_YeuCau = c.Double(nullable: false),
                    LaPhanTram_YeuCau = c.Boolean(nullable: false),
                    ChietKhau_TuVan = c.Double(nullable: false),
                    LaPhanTram_TuVan = c.Boolean(nullable: false),
                    ChietKhau_BanGoi = c.Double(),
                    LaPhanTram_BanGoi = c.Boolean(),
                    TheoChietKhau_ThucHien = c.Int(),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                    NgayNhap = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_DonViQuiDoi);

            CreateTable(
                "dbo.DonViQuiDoi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaHangHoa = c.String(maxLength: 50),
                    ID_HangHoa = c.Guid(nullable: false),
                    TenDonViTinh = c.String(maxLength: 50),
                    TyLeChuyenDoi = c.Double(nullable: false),
                    LaDonViChuan = c.Boolean(nullable: false),
                    GiaVon = c.Double(nullable: false),
                    GiaNhap = c.Double(nullable: false),
                    GiaBan = c.Double(nullable: false),
                    GhiChu = c.String(),
                    Xoa = c.Boolean(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ThuocTinhGiaTri = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa)
                .Index(t => t.ID_HangHoa);

            CreateTable(
                "dbo.DinhLuongDichVu",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DichVu = c.Guid(nullable: false),
                    SoLuong = c.Double(nullable: false),
                    GhiChu = c.String(storeType: "ntext"),
                    STT = c.Int(),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .Index(t => t.ID_DonViQuiDoi);

            CreateTable(
                "dbo.DM_GiaBan",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenGiaBan = c.String(maxLength: 250),
                    ApDung = c.Boolean(nullable: false),
                    TuNgay = c.DateTime(),
                    DenNgay = c.DateTime(),
                    GhiChu = c.String(storeType: "ntext"),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    TatCaDoiTuong = c.Boolean(nullable: false),
                    TatCaDonVi = c.Boolean(nullable: false),
                    TatCaNhanVien = c.Boolean(nullable: false),
                    NgayTrongTuan = c.String(),
                    LoaiChungTuApDung = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_GiaBan_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_GiaBan = c.Guid(nullable: false),
                    ID_KhoHang = c.Guid(nullable: false),
                    GiaBan = c.Double(nullable: false),
                    ID_NgoaiTe = c.Guid(),
                    PhuongThucTinhGiaBan = c.Int(),
                    TheoPT = c.Boolean(),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                    NgayNhap = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_TienTe", t => t.ID_NgoaiTe)
                .ForeignKey("dbo.DM_Kho", t => t.ID_KhoHang)
                .ForeignKey("dbo.DM_GiaBan", t => t.ID_GiaBan)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .Index(t => t.ID_GiaBan)
                .Index(t => t.ID_KhoHang)
                .Index(t => t.ID_NgoaiTe)
                .Index(t => t.ID_DonViQuiDoi);

            CreateTable(
                "dbo.DM_GiaBan_ApDung",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_GiaBan = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(),
                    ID_NhanVien = c.Guid(),
                    ID_NhomKhachHang = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomDoiTuong", t => t.ID_NhomKhachHang)
                .ForeignKey("dbo.DM_GiaBan", t => t.ID_GiaBan, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .Index(t => t.ID_GiaBan)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_NhomKhachHang);

            CreateTable(
                "dbo.DM_NhomDoiTuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    LoaiDoiTuong = c.Int(nullable: false),
                    MaNhomDoiTuong = c.String(maxLength: 50),
                    TenNhomDoiTuong = c.String(nullable: false, maxLength: 255),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    TrangThai = c.Boolean(),
                    GiamGia = c.Double(),
                    GiamGiaTheoPhanTram = c.Boolean(),
                    TuDongCapNhat = c.Boolean(),
                    TenNhomDoiTuong_KhongDau = c.String(),
                    TenNhomDoiTuong_KyTuDau = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_DoiTuong_Nhom",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DoiTuong = c.Guid(nullable: false),
                    ID_NhomDoiTuong = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomDoiTuong", t => t.ID_NhomDoiTuong, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong, cascadeDelete: true)
                .Index(t => t.ID_DoiTuong)
                .Index(t => t.ID_NhomDoiTuong);

            CreateTable(
                "dbo.DM_NhomDoiTuong_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhomDoiTuong = c.Guid(nullable: false),
                    LoaiDieuKien = c.Int(),
                    LoaiSoSanh = c.Int(),
                    GiaTriSo = c.Double(),
                    GiaTriBool = c.Boolean(),
                    GiaTriThoiGian = c.DateTime(),
                    GiaTriKhuVuc = c.Guid(),
                    GiaTriVungMien = c.Guid(),
                    STT = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomDoiTuong", t => t.ID_NhomDoiTuong, cascadeDelete: true)
                .ForeignKey("dbo.DM_VungMien", t => t.GiaTriVungMien)
                .ForeignKey("dbo.DM_TinhThanh", t => t.GiaTriKhuVuc)
                .Index(t => t.ID_NhomDoiTuong)
                .Index(t => t.GiaTriVungMien)
                .Index(t => t.GiaTriKhuVuc);

            CreateTable(
                "dbo.DM_KhuyenMai_ApDung",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_KhuyenMai = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(),
                    ID_NhanVien = c.Guid(),
                    ID_NhomKhachHang = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_KhuyenMai", t => t.ID_KhuyenMai, cascadeDelete: true)
                .ForeignKey("dbo.DM_NhomDoiTuong", t => t.ID_NhomKhachHang)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .Index(t => t.ID_KhuyenMai)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_NhomKhachHang);

            CreateTable(
                "dbo.DM_KhuyenMai",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaKhuyenMai = c.String(),
                    TenKhuyenMai = c.String(),
                    GhiChu = c.String(),
                    TrangThai = c.Boolean(nullable: false),
                    HinhThuc = c.Int(nullable: false),
                    LoaiKhuyenMai = c.Int(nullable: false),
                    ThoiGianBatDau = c.DateTime(nullable: false),
                    ThoiGianKetThuc = c.DateTime(nullable: false),
                    NgayApDung = c.String(),
                    ThangApDung = c.String(),
                    ThuApDung = c.String(),
                    GioApDung = c.String(),
                    ApDungNgaySinhNhat = c.Int(nullable: false),
                    TatCaDonVi = c.Boolean(nullable: false),
                    TatCaDoiTuong = c.Boolean(nullable: false),
                    TatCaNhanVien = c.Boolean(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_KhuyenMai_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_KhuyenMai = c.Guid(nullable: false),
                    TongTienHang = c.Double(),
                    GiamGia = c.Double(),
                    GiamGiaTheoPhanTram = c.Boolean(),
                    ID_DonViQuiDoi = c.Guid(),
                    ID_NhomHangHoa = c.Guid(),
                    SoLuong = c.Double(),
                    ID_DonViQuiDoiMua = c.Guid(),
                    ID_NhomHangHoaMua = c.Guid(),
                    SoLuongMua = c.Double(),
                    GiaKhuyenMai = c.Double(),
                    STT = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomHangHoa", t => t.ID_NhomHangHoa)
                .ForeignKey("dbo.DM_NhomHangHoa", t => t.ID_NhomHangHoaMua)
                .ForeignKey("dbo.DM_KhuyenMai", t => t.ID_KhuyenMai, cascadeDelete: true)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoiMua)
                .Index(t => t.ID_KhuyenMai)
                .Index(t => t.ID_DonViQuiDoi)
                .Index(t => t.ID_NhomHangHoa)
                .Index(t => t.ID_DonViQuiDoiMua)
                .Index(t => t.ID_NhomHangHoaMua);

            CreateTable(
                "dbo.DM_NhomHangHoa",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaNhomHangHoa = c.String(nullable: false, maxLength: 50),
                    TenNhomHangHoa = c.String(nullable: false, maxLength: 255),
                    GhiChu = c.String(),
                    ID_Parent = c.Guid(),
                    LaNhomHangHoa = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    HienThi_Chinh = c.Boolean(),
                    HienThi_Phu = c.Boolean(),
                    MayIn = c.String(),
                    ID_Kho = c.Guid(),
                    HienThi_BanThe = c.Boolean(),
                    MauHienThi = c.Int(),
                    TrangThai = c.Boolean(),
                    TenNhomHangHoa_KhongDau = c.String(),
                    TenNhomHangHoa_KyTuDau = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomHangHoa", t => t.ID_Parent)
                .Index(t => t.ID_Parent);

            CreateTable(
                "dbo.DM_HangHoa",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenHangHoa = c.String(nullable: false),
                    LaHangHoa = c.Boolean(),
                    ID_NhomHang = c.Guid(),
                    ID_PhanLoai = c.Guid(),
                    ID_QuocGia = c.Guid(),
                    ID_DoiTuong = c.Guid(),
                    ID_HangHoaCungLoai = c.Guid(),
                    QuyCach = c.Double(),
                    ID_DVTQuyCach = c.Guid(),
                    LoaiBaoHanh = c.Int(),
                    ThoiGianBaoHanh = c.Int(),
                    TenTGBaoHanh = c.String(),
                    ChiPhiThucHien = c.Double(nullable: false),
                    ChiPhiTinhTheoPT = c.Boolean(nullable: false),
                    TinhCPSauChietKhau = c.Boolean(),
                    GhiChu = c.String(),
                    SoPhutThucHien = c.Int(),
                    ChietKhauMD_NV = c.Double(),
                    ChietKhauMD_NVTheoPT = c.Boolean(),
                    TinhGiaVon = c.Int(),
                    TheoDoi = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    TenKhac = c.String(),
                    DuocBanTrucTiep = c.Boolean(nullable: false),
                    TenHangHoa_KhongDau = c.String(),
                    TenHangHoa_KyTuDau = c.String(),
                    TonCuoiKy = c.Double(),
                    LaChaCungLoai = c.Boolean(),
                    TonToiDa = c.Double(nullable: false, defaultValue: 0),
                    TonToiThieu = c.Double(nullable: false, defaultValue: 0),
                    QuanLyTheoLoHang = c.Boolean(),
                    DonViTinhQuyCach = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_PhanLoaiHangHoaDichVu", t => t.ID_PhanLoai)
                .ForeignKey("dbo.DM_QuocGia", t => t.ID_QuocGia)
                .ForeignKey("dbo.DM_NhomHangHoa", t => t.ID_NhomHang)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong)
                .Index(t => t.ID_NhomHang)
                .Index(t => t.ID_PhanLoai)
                .Index(t => t.ID_QuocGia)
                .Index(t => t.ID_DoiTuong);

            CreateTable(
                "dbo.DM_HangHoa_Anh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    SoThuTu = c.Int(nullable: false),
                    ID_HangHoa = c.Guid(nullable: false),
                    URLAnh = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa, cascadeDelete: true)
                .Index(t => t.ID_HangHoa);

            CreateTable(
                "dbo.DM_PhanLoaiHangHoaDichVu",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaPhanLoai = c.String(nullable: false, maxLength: 50),
                    TenPhanLoai = c.String(nullable: false),
                    GhiChu = c.String(storeType: "ntext"),
                    ThoiGianBaoHanh = c.Int(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_QuocGia",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaQuocGia = c.String(maxLength: 50),
                    TenQuocGia = c.String(nullable: false, maxLength: 255),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_TienTe",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaNgoaiTe = c.String(maxLength: 50),
                    TenNgoaiTe = c.String(nullable: false, maxLength: 100),
                    GhiChu = c.String(maxLength: 255),
                    ID_QuocGia = c.Guid(nullable: false),
                    LaNoiTe = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_QuocGia", t => t.ID_QuocGia)
                .Index(t => t.ID_QuocGia);

            CreateTable(
                "dbo.DM_TyGia",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_TienTe = c.Guid(nullable: false),
                    TyGia = c.Double(nullable: false),
                    NgayTyGia = c.DateTime(nullable: false),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_TienTe", t => t.ID_TienTe)
                .Index(t => t.ID_TienTe);

            CreateTable(
                "dbo.Kho_HoaDon",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(),
                    MaHoaDon = c.String(maxLength: 50),
                    NgayLapHoaDon = c.DateTime(nullable: false),
                    LoaiChungTu = c.Int(nullable: false),
                    NgayTao = c.DateTime(),
                    NguoiTao = c.String(maxLength: 50),
                    ID_DoiTuong = c.Guid(),
                    ID_NhanVien = c.Guid(),
                    ID_NgoaiTe = c.Guid(),
                    NguoiGiao = c.String(),
                    TongThanhTien = c.Double(),
                    ID_ChungTuLienQuan = c.Guid(),
                    DienGiai = c.String(),
                    NgaySua = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    ID_PhieuSuaChua = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LoaiChungTu", t => t.LoaiChungTu)
                .ForeignKey("dbo.DM_TienTe", t => t.ID_NgoaiTe)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .ForeignKey("dbo.BH_HoaDon", t => t.ID_ChungTuLienQuan)
                .Index(t => t.ID_DonVi)
                .Index(t => t.LoaiChungTu)
                .Index(t => t.ID_DoiTuong)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_NgoaiTe)
                .Index(t => t.ID_ChungTuLienQuan);

            CreateTable(
                "dbo.DM_LoaiChungTu",
                c => new
                {
                    ID = c.Int(nullable: false),
                    MaLoaiChungTu = c.String(nullable: false, maxLength: 50),
                    TenLoaiChungTu = c.String(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_MauIn",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_LoaiChungTu = c.Int(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    TenMauIn = c.String(),
                    KhoGiay = c.String(),
                    DuLieuMauIn = c.String(),
                    LaMacDinh = c.Boolean(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LoaiChungTu", t => t.ID_LoaiChungTu, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_LoaiChungTu)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.Quy_HoaDon",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaHoaDon = c.String(maxLength: 50),
                    NgayLapHoaDon = c.DateTime(nullable: false),
                    NgayTao = c.DateTime(),
                    ID_NhanVien = c.Guid(),
                    ID_NgoaiTe = c.Guid(),
                    NguoiNopTien = c.String(maxLength: 250),
                    NoiDungThu = c.String(maxLength: 500),
                    TongTienThu = c.Double(nullable: false),
                    ThuCuaNhieuDoiTuong = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    ID_DonVi = c.Guid(),
                    NgaySua = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    LoaiHoaDon = c.Int(),
                    TrangThai = c.Boolean(),
                    HachToanKinhDoanh = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LoaiChungTu", t => t.LoaiHoaDon)
                .ForeignKey("dbo.DM_TienTe", t => t.ID_NgoaiTe)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_NgoaiTe)
                .Index(t => t.ID_DonVi)
                .Index(t => t.LoaiHoaDon);

            CreateTable(
                "dbo.DM_NganHang",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaNganHang = c.String(maxLength: 50),
                    TenNganHang = c.String(nullable: false),
                    ChiNhanh = c.String(),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ChiPhiThanhToan = c.Double(),
                    TheoPhanTram = c.Boolean(),
                    MacDinh = c.Boolean(),
                    ThuPhiThanhToan = c.Boolean(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_TaiKhoanNganHang",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_NganHang = c.Guid(nullable: false),
                    TenChuThe = c.String(),
                    SoTaiKhoan = c.String(),
                    TaiKhoanPOS = c.Boolean(nullable: false),
                    GhiChu = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NganHang", t => t.ID_NganHang, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_NganHang);

            CreateTable(
                "dbo.Quy_HoaDon_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_HoaDon = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(),
                    ID_DoiTuong = c.Guid(),
                    ID_TheKhachHang = c.Guid(),
                    ThuTuThe = c.Double(nullable: false),
                    TienMat = c.Double(nullable: false),
                    TienGui = c.Double(nullable: false),
                    TienThu = c.Double(nullable: false),
                    GhiChu = c.String(),
                    ID_HoaDonLienQuan = c.Guid(),
                    ChiPhiNganHang = c.Double(),
                    ID_NganHang = c.Guid(),
                    LaPTChiPhiNganHang = c.Boolean(),
                    ThuPhiTienGui = c.Boolean(),
                    ID_KhoanThuChi = c.Guid(),
                    DiemThanhToan = c.Double(),
                    ID_TaiKhoanNganHang = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NganHang", t => t.ID_NganHang)
                .ForeignKey("dbo.Quy_KhoanThuChi", t => t.ID_KhoanThuChi)
                .ForeignKey("dbo.The_TheKhachHang", t => t.ID_TheKhachHang)
                .ForeignKey("dbo.Quy_HoaDon", t => t.ID_HoaDon)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .ForeignKey("dbo.BH_HoaDon", t => t.ID_HoaDonLienQuan)
                .ForeignKey("dbo.DM_TaiKhoanNganHang", t => t.ID_TaiKhoanNganHang)
                .Index(t => t.ID_HoaDon)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_DoiTuong)
                .Index(t => t.ID_TheKhachHang)
                .Index(t => t.ID_HoaDonLienQuan)
                .Index(t => t.ID_NganHang)
                .Index(t => t.ID_KhoanThuChi)
                .Index(t => t.ID_TaiKhoanNganHang);

            CreateTable(
                "dbo.HT_CongTy",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenCongTy = c.String(nullable: false),
                    DiaChi = c.String(),
                    SoDienThoai = c.String(),
                    SoFax = c.String(),
                    MaSoThue = c.String(),
                    Mail = c.String(),
                    Website = c.String(),
                    TenGiamDoc = c.String(),
                    TenKeToanTruong = c.String(),
                    Logo = c.Binary(storeType: "image"),
                    GhiChu = c.String(),
                    TaiKhoanNganHang = c.String(),
                    ID_NganHang = c.Guid(),
                    DiaChiNganHang = c.String(),
                    TenVT = c.String(),
                    DiaChiVT = c.String(),
                    DangHoatDong = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NganHang", t => t.ID_NganHang)
                .Index(t => t.ID_NganHang);

            CreateTable(
                "dbo.HT_ThongBao",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    LoaiThongBao = c.Int(nullable: false),
                    NoiDungThongBao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiDungDaDoc = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.Quy_KhoanThuChi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaKhoanThuChi = c.String(nullable: false, maxLength: 50),
                    NoiDungThuChi = c.String(maxLength: 500),
                    GhiChu = c.String(maxLength: 500),
                    LaKhoanThu = c.Boolean(nullable: false),
                    BuTruCongNo = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    TinhLuong = c.Boolean(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.The_TheKhachHang",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaThe = c.String(nullable: false, maxLength: 50),
                    ID_NhomThe = c.Guid(nullable: false),
                    NgayMua = c.DateTime(nullable: false),
                    NgayApDung = c.DateTime(nullable: false),
                    NgayHetHan = c.DateTime(),
                    ID_DoiTuong = c.Guid(nullable: false),
                    MenhGiaThe = c.Double(nullable: false),
                    PTChietKhau = c.Double(),
                    TienChietKhau = c.Double(),
                    PhaiThanhToan = c.Double(nullable: false),
                    ID_TienTe = c.Guid(),
                    TyGia = c.Double(),
                    ID_NhanVienLap = c.Guid(),
                    ApDungTatCaSanPham = c.Boolean(nullable: false),
                    DuocChoMuon = c.Boolean(nullable: false),
                    TheGiaTri_SoLan_GiamGia = c.Int(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    GhiChu = c.String(),
                    ID_DonVi = c.Guid(),
                    PTTangThem = c.Double(),
                    TienTangThem = c.Double(),
                    ID_LienHe = c.Guid(),
                    HuyThe = c.Boolean(),
                    NgayHuy = c.DateTime(),
                    SoLanDuocSuDung = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LienHe", t => t.ID_LienHe)
                .ForeignKey("dbo.The_NhomThe", t => t.ID_NhomThe)
                .ForeignKey("dbo.DM_TienTe", t => t.ID_TienTe)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienLap)
                .Index(t => t.ID_NhomThe)
                .Index(t => t.ID_DoiTuong)
                .Index(t => t.ID_TienTe)
                .Index(t => t.ID_NhanVienLap)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_LienHe);

            CreateTable(
                "dbo.DM_LienHe",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DoiTuong = c.Guid(nullable: false),
                    MaLienHe = c.String(maxLength: 50),
                    TenLienHe = c.String(nullable: false, maxLength: 255),
                    SoDienThoai = c.String(maxLength: 50),
                    Email = c.String(maxLength: 50),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    DiaChi = c.String(),
                    NgaySinh = c.DateTime(),
                    CanNang = c.String(maxLength: 50),
                    ChieuCao = c.String(maxLength: 50),
                    ID_ChucVu = c.Guid(),
                    TrangThai = c.Int(),
                    ChucVu = c.String(),
                    ID_TinhThanh = c.Guid(),
                    ID_QuanHuyen = c.Guid(),
                    XungHo = c.Int(),
                    DienThoaiCoDinh = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_ChucVu", t => t.ID_ChucVu)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_DoiTuong)
                .ForeignKey("dbo.DM_QuanHuyen", t => t.ID_QuanHuyen)
                .ForeignKey("dbo.DM_TinhThanh", t => t.ID_TinhThanh)
                .Index(t => t.ID_DoiTuong)
                .Index(t => t.ID_ChucVu)
                .Index(t => t.ID_QuanHuyen)
                .Index(t => t.ID_TinhThanh);

            CreateTable(
                "dbo.DM_LienHe_Anh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    SoThuTu = c.Int(nullable: false),
                    ID_LienHe = c.Guid(nullable: false),
                    URLAnh = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LienHe", t => t.ID_LienHe, cascadeDelete: true)
                .Index(t => t.ID_LienHe);

            CreateTable(
                "dbo.DM_ChucVu",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaChucVu = c.String(nullable: false, maxLength: 50),
                    TenChucVu = c.String(nullable: false, maxLength: 255),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_QuaTrinhCongTac",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_ChucVu = c.Guid(),
                    NgayApDung = c.DateTime(nullable: false),
                    NgayHetHan = c.DateTime(),
                    LaChucVuHienThoi = c.Boolean(nullable: false),
                    LaDonViHienThoi = c.Boolean(nullable: false),
                    NgayLap = c.DateTime(),
                    NguoiLap = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    ID_PhongBan = c.Guid()
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_ChucVu", t => t.ID_ChucVu)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .ForeignKey("dbo.NS_PhongBan", t => t.ID_PhongBan)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_ChucVu)
                .Index(t => t.ID_PhongBan);

            CreateTable(
                "dbo.The_NhomThe",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaNhomThe = c.String(nullable: false, maxLength: 50),
                    TenNhomThe = c.String(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.The_TheKhachHang_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_TheKhachHang = c.Guid(nullable: false),
                    SoLuong = c.Double(),
                    DonGia = c.Double(),
                    PTChietKhau = c.Double(),
                    TienChietKhau = c.Double(),
                    ThanhToan = c.Double(),
                    ID_LopHoc = c.Guid(),
                    GhiChu = c.String(),
                    SoLuongTang = c.Double(),
                    NgayTraLai = c.DateTime(),
                    SoLuongTraLai = c.Double(),
                    TienDaSuDung = c.Double(),
                    TraLaiHHDV = c.Boolean(),
                    ID_SanPhamChinh = c.Guid(),
                    LaTangKem = c.Boolean(),
                    SoLuongDaSuDung = c.Double(),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.The_TheKhachHang", t => t.ID_TheKhachHang)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .Index(t => t.ID_TheKhachHang)
                .Index(t => t.ID_DonViQuiDoi);

            CreateTable(
                "dbo.Kho_HoaDon_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    SoThuTu = c.Int(),
                    ID_HoaDon = c.Guid(nullable: false),
                    ID_Kho = c.Guid(nullable: false),
                    ID_LoHang = c.Guid(),
                    SoLuong = c.Double(nullable: false),
                    DonGia = c.Double(nullable: false),
                    ThanhTien = c.Double(nullable: false),
                    GhiChu = c.String(),
                    ID_CTChungTuLienQuan = c.Guid(),
                    An_Hien = c.Boolean(),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_Kho", t => t.ID_Kho)
                .ForeignKey("dbo.DM_LoHang", t => t.ID_LoHang)
                .ForeignKey("dbo.Kho_HoaDon", t => t.ID_HoaDon)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .ForeignKey("dbo.BH_HoaDon_ChiTiet", t => t.ID_CTChungTuLienQuan)
                .Index(t => t.ID_HoaDon)
                .Index(t => t.ID_Kho)
                .Index(t => t.ID_LoHang)
                .Index(t => t.ID_CTChungTuLienQuan)
                .Index(t => t.ID_DonViQuiDoi);

            CreateTable(
                "dbo.DM_Kho",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaKho = c.String(nullable: false, maxLength: 50),
                    TenKho = c.String(nullable: false, maxLength: 255),
                    DiaDiem = c.String(maxLength: 255),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Kho_DonVi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_Kho = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_Kho", t => t.ID_Kho)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_Kho)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.Kho_TonKhoKhoiTao",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    NgayChungTu = c.DateTime(nullable: false),
                    NamHachToan = c.Int(nullable: false),
                    ID_Kho = c.Guid(nullable: false),
                    ID_LoHang = c.Guid(),
                    SoLuong = c.Double(nullable: false),
                    DonGia = c.Double(nullable: false),
                    ThanhTien = c.Double(nullable: false),
                    ID_DonVi = c.Guid(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LoHang", t => t.ID_LoHang)
                .ForeignKey("dbo.DM_Kho", t => t.ID_Kho)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_Kho)
                .Index(t => t.ID_LoHang)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_DonViQuiDoi);

            CreateTable(
                "dbo.DM_LoHang",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_HangHoa = c.Guid(nullable: false),
                    MaLoHang = c.String(nullable: false, maxLength: 50),
                    NgaySanXuat = c.DateTime(),
                    NgayHetHan = c.DateTime(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    TenLoHang = c.String(),
                    TrangThai = c.Boolean(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_GiaVon",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonViQuiDoi = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_LoHang = c.Guid(),
                    GiaVon = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LoHang", t => t.ID_LoHang)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuiDoi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonViQuiDoi)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_LoHang);

            CreateTable(
                "dbo.Quy_TonQuyKhoiTao",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    NgayChungTu = c.DateTime(nullable: false),
                    NamHachToan = c.Int(nullable: false),
                    TonQuy = c.Double(nullable: false),
                    ID_DonVi = c.Guid(),
                    ID_TienTe = c.Guid(nullable: false),
                    TyGia = c.Double(nullable: false),
                    LaTienMat = c.Boolean(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_TienTe", t => t.ID_TienTe)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_TienTe);

            CreateTable(
                "dbo.DM_TinhThanh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaTinhThanh = c.String(maxLength: 50),
                    TenTinhThanh = c.String(nullable: false, maxLength: 255),
                    ID_QuocGia = c.Guid(),
                    ID_VungMien = c.Guid(),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_VungMien", t => t.ID_VungMien)
                .ForeignKey("dbo.DM_QuocGia", t => t.ID_QuocGia)
                .Index(t => t.ID_QuocGia)
                .Index(t => t.ID_VungMien);

            CreateTable(
                "dbo.DM_QuanHuyen",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaQuanHuyen = c.String(maxLength: 50),
                    TenQuanHuyen = c.String(nullable: false),
                    ID_TinhThanh = c.Guid(nullable: false),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_TinhThanh", t => t.ID_TinhThanh)
                .Index(t => t.ID_TinhThanh);

            CreateTable(
                "dbo.DM_XaPhuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenXaPhuong = c.String(),
                    ID_QuanHuyen = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_QuanHuyen", t => t.ID_QuanHuyen, cascadeDelete: true)
                .Index(t => t.ID_QuanHuyen);

            CreateTable(
                "dbo.DM_VungMien",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaVung = c.String(maxLength: 50),
                    TenVung = c.String(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.HangHoa_ThuocTinh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_HangHoa = c.Guid(nullable: false),
                    ID_ThuocTinh = c.Guid(nullable: false),
                    GiaTri = c.String(),
                    ThuTuNhap = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_ThuocTinh", t => t.ID_ThuocTinh)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa, cascadeDelete: true)
                .Index(t => t.ID_HangHoa)
                .Index(t => t.ID_ThuocTinh);

            CreateTable(
                "dbo.DM_ThuocTinh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenThuocTinh = c.String(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NhomHangHoa_DonVi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhomHangHoa = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomHangHoa", t => t.ID_NhomHangHoa)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_NhomHangHoa)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.HT_CauHinh_TichDiemApDung",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_TichDiem = c.Guid(nullable: false),
                    ID_NhomDoiTuong = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_CauHinh_TichDiemChiTiet", t => t.ID_TichDiem, cascadeDelete: true)
                .ForeignKey("dbo.DM_NhomDoiTuong", t => t.ID_NhomDoiTuong, cascadeDelete: true)
                .Index(t => t.ID_TichDiem)
                .Index(t => t.ID_NhomDoiTuong);

            CreateTable(
                "dbo.HT_CauHinh_TichDiemChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_CauHinh = c.Guid(nullable: false),
                    TyLeDoiDiem = c.Double(nullable: false),
                    ThanhToanBangDiem = c.Boolean(nullable: false),
                    DiemThanhToan = c.Int(nullable: false),
                    TienThanhToan = c.Double(nullable: false),
                    TichDiemGiamGia = c.Boolean(nullable: false),
                    TichDiemHoaDonDiemThuong = c.Boolean(nullable: false),
                    ToanBoKhachHang = c.Boolean(nullable: false),
                    KhoiTaoTichDiem = c.Boolean(nullable: false),
                    SoLanMua = c.Int(),
                    TichDiemHoaDonGiamGia = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_CauHinhPhanMem", t => t.ID_CauHinh, cascadeDelete: true)
                .Index(t => t.ID_CauHinh);

            CreateTable(
                "dbo.HT_CauHinhPhanMem",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    GiaVonTrungBinh = c.Boolean(nullable: false),
                    CoDonViTinh = c.Boolean(nullable: false),
                    DatHang = c.Boolean(nullable: false),
                    XuatAm = c.Boolean(nullable: false),
                    DatHangXuatAm = c.Boolean(nullable: false),
                    ThayDoiThoiGianBanHang = c.Boolean(nullable: false),
                    SoLuongTrenChungTu = c.Boolean(nullable: false),
                    TinhNangTichDiem = c.Boolean(nullable: false),
                    GioiHanThoiGianTraHang = c.Boolean(nullable: false),
                    SanPhamCoThuocTinh = c.Boolean(nullable: false),
                    BanVaChuyenKhiHangDaDat = c.Boolean(nullable: false),
                    TinhNangSanXuatHangHoa = c.Boolean(nullable: false),
                    SuDungCanDienTu = c.Boolean(nullable: false),
                    KhoaSo = c.Boolean(nullable: false),
                    InBaoGiaKhiBanHang = c.Boolean(nullable: false),
                    QuanLyKhachHangTheoDonVi = c.Boolean(nullable: false),
                    KhuyenMai = c.Boolean(),
                    LoHang = c.Boolean(),
                    SuDungMauInMacDinh = c.Boolean(),
                    ApDungGopKhuyenMai = c.Boolean(),
                    ThongTinChiTietNhanVien = c.Boolean(),
                    BanHangOffline = c.Boolean(),
                    ThoiGianNhacHanSuDungLo = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.HT_CauHinh_GioiHanTraHang",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_CauHinh = c.Guid(nullable: false),
                    SoNgayGioiHan = c.Int(nullable: false),
                    ChoPhepTraHang = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_CauHinhPhanMem", t => t.ID_CauHinh, cascadeDelete: true)
                .Index(t => t.ID_CauHinh);

            CreateTable(
                "dbo.NhomDoiTuong_DonVi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhomDoiTuong = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_NhomDoiTuong", t => t.ID_NhomDoiTuong)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_NhomDoiTuong)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.DM_LopHoc",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaLopHoc = c.String(nullable: false, maxLength: 50),
                    TenLopHoc = c.String(nullable: false),
                    NgayBD = c.DateTime(),
                    NgayKT = c.DateTime(),
                    GioBD = c.DateTime(),
                    GioKT = c.DateTime(),
                    NgayTrongTuan = c.String(),
                    GiaoVienPhuTrach = c.Guid(),
                    ID_DonVi = c.Guid(nullable: false),
                    GhiChu = c.String(storeType: "ntext"),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ChietKhau_GiaoVienPhuTrach = c.Double(),
                    LaPhanTram_CKGV = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.HT_MaChungTu",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    LoaiChungTu = c.String(nullable: false, maxLength: 50),
                    TienTo = c.String(maxLength: 50),
                    NganCach1 = c.String(maxLength: 50),
                    NgayThangNam = c.String(maxLength: 50),
                    NganCach2 = c.String(maxLength: 50),
                    DoDaiSTT = c.Int(nullable: false),
                    SuDungUserName = c.Boolean(),
                    SuDungMaDonVi = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.HT_NguoiDung",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(),
                    TaiKhoan = c.String(nullable: false, maxLength: 50),
                    MatKhau = c.String(nullable: false, maxLength: 150),
                    LaNhanVien = c.Boolean(nullable: false),
                    LaAdmin = c.Boolean(nullable: false),
                    DangHoatDong = c.Boolean(nullable: false),
                    IsSystem = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ID_DonVi = c.Guid(),
                    XemGiaVon = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.DM_LoaiTuVanLichHen",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenLoaiTuVanLichHen = c.String(),
                    TuVan_LichHen = c.Int(nullable: false),
                    NgayTao = c.DateTime(nullable: false),
                    NgaySua = c.DateTime(),
                    NguoiTao = c.String(),
                    NguoiSua = c.String(),
                    TrangThai = c.Int(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.HT_NguoiDung_Nhom",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    IDNhomNguoiDung = c.Guid(nullable: false),
                    IDNguoiDung = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_NhomNguoiDung", t => t.IDNhomNguoiDung)
                .ForeignKey("dbo.HT_NguoiDung", t => t.IDNguoiDung)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi)
                .Index(t => t.IDNhomNguoiDung)
                .Index(t => t.IDNguoiDung)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.HT_NhomNguoiDung",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaNhom = c.String(nullable: false, maxLength: 50),
                    TenNhom = c.String(nullable: false, maxLength: 100),
                    MoTa = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.HT_Quyen_Nhom",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhomNguoiDung = c.Guid(nullable: false),
                    MaQuyen = c.String(nullable: false, maxLength: 100),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_Quyen", t => t.MaQuyen)
                .ForeignKey("dbo.HT_NhomNguoiDung", t => t.ID_NhomNguoiDung)
                .Index(t => t.ID_NhomNguoiDung)
                .Index(t => t.MaQuyen);

            CreateTable(
                "dbo.HT_Quyen",
                c => new
                {
                    MaQuyen = c.String(nullable: false, maxLength: 100),
                    TenQuyen = c.String(),
                    MoTa = c.String(),
                    QuyenCha = c.String(),
                    DuocSuDung = c.Boolean(),
                })
                .PrimaryKey(t => t.MaQuyen);

            CreateTable(
                "dbo.HT_QuyenMacDinh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    IDNguoiDung = c.Guid(nullable: false),
                    NhapGiaBan = c.Boolean(),
                    NhapChietKhau = c.Boolean(),
                    IDDoiTuong_HDB = c.Guid(),
                    IDKho_HDB = c.Guid(),
                    IDDoiTuong_HDBL = c.Guid(),
                    IDKho_HDBL = c.Guid(),
                    IDKho_NhapKho = c.Guid(),
                    IDDoiTuong_NhapKho = c.Guid(),
                    IDKho_XuatKho = c.Guid(),
                    IDDoiTuong_XuatKho = c.Guid(),
                    IDDoiTuong_MuaHang = c.Guid(),
                    IDKho_MuaHang = c.Guid(),
                    IDDoiTuong_HBTL = c.Guid(),
                    IDKho_HBTL = c.Guid(),
                    IDDoiTuong_TraLaiNCC = c.Guid(),
                    IDKho_TraLaiNCC = c.Guid(),
                    IDDoiTuong_PhieuThu = c.Guid(),
                    IDDoiTuong_PhieuChi = c.Guid(),
                    IDKho_DieuChuyen = c.Guid(),
                    SuaNgayChungTu = c.Boolean(),
                    SuaSoChungTu = c.Boolean(),
                    ThayDoiNhanVien = c.Boolean(),
                    ID_NhomDichVu = c.Guid(),
                    ID_NhomDoiTuong = c.Guid(),
                    ID_NhomHangHoa = c.Guid(),
                    NhapChietKhauChung = c.Boolean(),
                    NhapGiamGia = c.Boolean(),
                    IDDoiTuong_BaoGia = c.Guid(),
                    IDDoiTuong_DonDatMua = c.Guid(),
                    IDKho_BaoGia = c.Guid(),
                    IDKho_DonDatMua = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_NguoiDung", t => t.IDNguoiDung)
                .Index(t => t.IDNguoiDung);

            CreateTable(
                "dbo.HT_NhatKySuDung",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ChucNang = c.String(),
                    ThoiGian = c.DateTime(nullable: false),
                    NoiDung = c.String(),
                    LoaiNhatKy = c.Int(nullable: false),
                    NoiDungChiTiet = c.String(),
                    //ID_HoaDon = c.Guid(),
                    //LoaiHoaDon = c.Int(),
                    //ThoiGianUpdateGV = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.DM_NguonKhachHang",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenNguonKhach = c.String(),
                    NgayTao = c.DateTime(),
                    NgaySua = c.DateTime(),
                    NguoiTao = c.String(),
                    NguoiSua = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_HoSoLuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    NgayBD = c.DateTime(nullable: false),
                    NgayKT = c.DateTime(),
                    MucLuongCB = c.Double(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ID_LuongDoanhThu = c.Guid(),
                    LuongTuVan = c.Boolean(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_LuongDoanhThu", t => t.ID_LuongDoanhThu)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_LuongDoanhThu);

            CreateTable(
                "dbo.NS_LuongDoanhThu",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    Ma = c.String(nullable: false, maxLength: 50),
                    Ten = c.String(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    ApDung_HangHoaDichVu = c.Int(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_LuongDoanhThu_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_LuongDoanhThu = c.Guid(nullable: false),
                    DoanhThu_Min = c.Double(nullable: false),
                    DoanhThu_Max = c.Double(),
                    LuongDuocNhan = c.Double(nullable: false),
                    TheoPT = c.Boolean(nullable: false),
                    LaThoChinh = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_LuongDoanhThu", t => t.ID_LuongDoanhThu)
                .Index(t => t.ID_LuongDoanhThu);

            CreateTable(
                "dbo.DM_ViTri",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaViTri = c.String(nullable: false, maxLength: 50),
                    ID_KhuVuc = c.Guid(nullable: false),
                    TinhTrang = c.Boolean(),
                    GhiChu = c.String(maxLength: 500),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    DonGiaGio = c.Double(),
                    Anh = c.Binary(storeType: "image"),
                    DonGiaNgay = c.Double(),
                    ID_LoaiPhong = c.Guid(),
                    Tang = c.Int(),
                    TenViTri = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_KhuVuc", t => t.ID_KhuVuc)
                .ForeignKey("dbo.DM_LoaiPhong", t => t.ID_LoaiPhong)
                .Index(t => t.ID_KhuVuc)
                .Index(t => t.ID_LoaiPhong);

            CreateTable(
                "dbo.DM_KhuVuc",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaKhuVuc = c.String(nullable: false, maxLength: 50),
                    TenKhuVuc = c.String(nullable: false, maxLength: 250),
                    GhiChu = c.String(maxLength: 500),
                    Tang = c.Int(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    SoDoKhuVuc = c.Binary(storeType: "image"),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_LoaiPhong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaLoai = c.String(nullable: false, maxLength: 50),
                    TenLoai = c.String(nullable: false),
                    SoNguoi_Min = c.Int(),
                    SoNguoi_Max = c.Int(),
                    SuDung = c.Boolean(nullable: false),
                    MauSac = c.Int(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.CongDoan_DichVu",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DichVu = c.Guid(nullable: false),
                    STT = c.Int(nullable: false),
                    ID_CongDoan = c.Guid(nullable: false),
                    ThoiGian = c.String(),
                    SoPhutThucHien = c.Double(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.CongNoDauKi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_DoiTuong = c.Guid(nullable: false),
                    NamHachToan = c.Int(nullable: false),
                    LaPhaiThu = c.Boolean(nullable: false),
                    SoTien = c.Double(nullable: false),
                    ID_TienTe = c.Guid(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DanhSachThi_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DanhSachThi = c.Guid(nullable: false),
                    ID_ThiSinh = c.Guid(nullable: false),
                    MaDuThi = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DanhSachThi",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    Ma = c.String(nullable: false, maxLength: 50),
                    Ten = c.String(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_MonThi = c.Guid(nullable: false),
                    ID_GiaoVien = c.Guid(),
                    ID_TrongTai = c.Guid(),
                    ThoiGianBatDau = c.DateTime(),
                    ThoiGianKetThuc = c.DateTime(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_HinhThucThanhToan",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaHinhThuc = c.String(maxLength: 50),
                    TenHinhThuc = c.String(nullable: false, maxLength: 255),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_HinhThucVanChuyen",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaHinhThuc = c.String(maxLength: 50),
                    TenHinhThuc = c.String(nullable: false, maxLength: 255),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_KhoanPhuCap",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaKhoanPhuCap = c.String(nullable: false, maxLength: 50),
                    TenKhoanPhuCap = c.String(nullable: false),
                    GhiChu = c.String(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_LoaiGiaPhong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaLoai = c.String(nullable: false, maxLength: 50),
                    TenLoai = c.String(nullable: false, maxLength: 50),
                    LaGiaNgay = c.Boolean(nullable: false),
                    ThoiGian_Min = c.Double(),
                    ThoiGian_Max = c.Double(),
                    GhiChu = c.String(),
                    SoGioToiThieu = c.Double(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_LoaiNhapXuat",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaLoai = c.String(nullable: false, maxLength: 50),
                    TenLoai = c.String(nullable: false),
                    GhiChu = c.String(storeType: "ntext"),
                    Nhap_Xuat_DieuChuyen = c.Int(nullable: false),
                    SuDung = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_LoaiPhieuThanhToan",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaLoai = c.String(nullable: false, maxLength: 50),
                    TenLoai = c.String(nullable: false),
                    SoLanSuDung = c.Double(),
                    ThanhToanTheoPhanTram = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_LyDoHuyLichHen",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    LyDoHuyLichHen = c.String(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_MaVach",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_HangHoa = c.Guid(nullable: false),
                    MaVach = c.String(nullable: false, maxLength: 50),
                    LaHienTai = c.Boolean(nullable: false),
                    SuDung = c.Boolean(),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_MayChamCong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaMayChamCong = c.Int(nullable: false),
                    TenMayChamCong = c.String(),
                    IP = c.String(maxLength: 15),
                    Port = c.Int(),
                    TenMien = c.String(),
                    COMPort = c.Int(),
                    KieuKetNoi = c.Int(),
                    BaudRate = c.Int(),
                    SuDungTenMien = c.Boolean(),
                    SuDung = c.Boolean(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_NoiDungQuanTam",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaNoiDung = c.String(nullable: false, maxLength: 50),
                    TenNoiDung = c.String(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_ThueSuat",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaThueSuat = c.String(nullable: false, maxLength: 50),
                    ThueSuat = c.Int(nullable: false),
                    GhiChu = c.String(maxLength: 255),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_TichDiem",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaTichDiem = c.String(nullable: false, maxLength: 50),
                    NgayBatDauApDung = c.DateTime(nullable: false),
                    TienThuVe_Min = c.Double(nullable: false),
                    TienThuVe_Max = c.Double(),
                    DiemQuyDoi = c.Double(nullable: false),
                    MucDiem = c.Double(nullable: false),
                    TienThanhToanQuyDoi = c.Double(nullable: false),
                    DiemToiThieu = c.Double(nullable: false),
                    TienToiThieu = c.Double(nullable: false),
                    NhomKhachHang = c.String(),
                    ChungTu = c.String(),
                    ThuTrongTuan = c.String(maxLength: 255),
                    ApDung = c.Boolean(nullable: false),
                    NgayBatDau = c.DateTime(nullable: false),
                    NgayKetThuc = c.DateTime(),
                    NgayTao = c.DateTime(),
                    NguoiTao = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    ApDung_NguoiGioiThieu = c.Boolean(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.HT_PhimTat",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaPhim = c.String(maxLength: 50),
                    TenPhim = c.String(maxLength: 255),
                    KeyFn = c.Int(),
                    KeyCode = c.Int(),
                    DienGiai = c.String(maxLength: 255),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_ChamCong_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
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
                    SoNgayNghi = c.Double(nullable: false),
                    TongCong = c.Double(nullable: false),
                    NguoiTao = c.String(maxLength: 50),
                    NgayTao = c.DateTime(),
                    NguoiSua = c.String(maxLength: 50),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.ChotSo_KhachHang",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_KhachHang = c.Guid(nullable: false),
                    CongNo = c.Double(nullable: false),
                    NgayChotSo = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_KhachHang, cascadeDelete: true)
                .Index(t => t.ID_KhachHang);

            CreateTable(
                "dbo.ChotSo_HangHoa",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_HangHoa = c.Guid(nullable: false),
                    TonKho = c.Double(nullable: false),
                    NgayChotSo = c.DateTime(nullable: false),
                    ID_LoHang = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.DM_LoHang", t => t.ID_LoHang)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_HangHoa)
                .Index(t => t.ID_LoHang);

            CreateTable(
                "dbo.ChotSo",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    NgayChotSo = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.HT_ThongBao_CaiDat",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    NhacSinhNhat = c.Boolean(nullable: false),
                    NhacCongNo = c.Boolean(nullable: false),
                    NhacTonKho = c.Boolean(nullable: false),
                    NhacDieuChuyen = c.Boolean(nullable: false),
                    ID_NguoiDung = c.Guid(nullable: false),
                    NhacLoHang = c.Boolean(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_NguoiDung", t => t.ID_NguoiDung, cascadeDelete: true)
                .Index(t => t.ID_NguoiDung);

            CreateTable(
                "dbo.NS_CongViec_PhanLoai",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    LoaiCongViec = c.String(),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_CongViec",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_LoaiCongViec = c.Guid(nullable: false),
                    ID_KhachHang = c.Guid(),
                    ID_LienHe = c.Guid(),
                    ID_DonVi = c.Guid(nullable: false),
                    ThoiGianTu = c.DateTime(nullable: false),
                    ThoiGianDen = c.DateTime(),
                    NhacTruoc = c.Int(nullable: false),
                    ID_NhanVienChiaSe = c.Guid(),
                    NoiDung = c.String(),
                    ThoiGianLienHeLai = c.DateTime(),
                    NhacTruocLienHeLai = c.Int(nullable: false),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                    ID_NhanVienQuanLy = c.Guid(),
                    KetQuaCongViec = c.String(),
                    LyDoHenLai = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_CongViec_PhanLoai", t => t.ID_LoaiCongViec, cascadeDelete: true)
                .ForeignKey("dbo.DM_LienHe", t => t.ID_LienHe)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_KhachHang)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienChiaSe)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienQuanLy)
                .Index(t => t.ID_LoaiCongViec)
                .Index(t => t.ID_KhachHang)
                .Index(t => t.ID_LienHe)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_NhanVienChiaSe)
                .Index(t => t.ID_NhanVienQuanLy);

            CreateTable(
                "dbo.NS_NhanVien_CongTac",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    TuNgay = c.DateTime(nullable: false),
                    DenNgay = c.DateTime(),
                    CoQuan = c.String(),
                    ViTri = c.String(),
                    DiaChi = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_NhanVien_DaoTao",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    TuNgay = c.DateTime(nullable: false),
                    DenNgay = c.DateTime(),
                    NoiHoc = c.String(),
                    NganhHoc = c.String(),
                    HeDaoTao = c.String(),
                    BangCap = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_NhanVien_GiaDinh",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    HoTen = c.String(),
                    NgaySinh = c.Int(),
                    NoiO = c.String(),
                    QuanHe = c.String(),
                    DiaChi = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_NhanVien_SucKhoe",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ChieuCao = c.Double(nullable: false),
                    CanNang = c.Double(nullable: false),
                    TinhHinhSucKhoe = c.String(),
                    NgayKham = c.DateTime(nullable: false),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_BaoHiem",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    LoaiBaoHiem = c.Int(nullable: false),
                    NoiBaoHiem = c.String(),
                    NgayCap = c.DateTime(nullable: false),
                    NgayHetHan = c.DateTime(),
                    GhiChu = c.String(),
                    SoBaoHiem = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_HopDong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    SoHopDong = c.String(),
                    LoaiHopDong = c.Int(nullable: false),
                    NgayKy = c.DateTime(nullable: false),
                    GhiChu = c.String(),
                    ThoiHan = c.Int(nullable: false),
                    DonViThoiHan = c.Int(nullable: false),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_KhenThuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    HinhThuc = c.String(),
                    SoQuyetDinh = c.String(),
                    NgayBanHang = c.DateTime(nullable: false),
                    NoiDung = c.String(),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_Luong_PhuCap",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_LoaiLuong = c.Guid(nullable: false),
                    NgayApDung = c.DateTime(nullable: false),
                    NgayKetThuc = c.DateTime(),
                    SoTien = c.Double(nullable: false),
                    HeSo = c.Double(nullable: false),
                    Bac = c.String(),
                    NoiDung = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_LoaiLuong", t => t.ID_LoaiLuong, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_LoaiLuong);

            CreateTable(
                "dbo.NS_LoaiLuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenLoaiLuong = c.String(),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_MienGiamThue",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    KhoanMienGiam = c.String(),
                    SoTien = c.Double(nullable: false),
                    NgayApDung = c.DateTime(nullable: false),
                    NgayKetThuc = c.DateTime(),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.HeThong_SMS",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NguoiGui = c.Guid(nullable: false),
                    ID_KhachHang = c.Guid(),
                    ID_DonVi = c.Guid(nullable: false),
                    SoDienThoai = c.String(),
                    SoTinGui = c.Int(nullable: false),
                    NoiDung = c.String(),
                    ThoiGianGui = c.DateTime(nullable: false),
                    TrangThai = c.Int(nullable: false),
                    GiaTien = c.Double(nullable: false),
                    LoaiTinNhan = c.Int(nullable: false),
                    IDTinNhan = c.String(),
                    ID_HoaDon = c.Guid(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_NguoiDung", t => t.ID_NguoiGui, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DoiTuong", t => t.ID_KhachHang)
                .ForeignKey("dbo.BH_HoaDon", t => t.ID_HoaDon)
                .Index(t => t.ID_NguoiGui)
                .Index(t => t.ID_KhachHang)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_HoaDon);

            CreateTable(
                "dbo.HeThong_SMS_TaiKhoan",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NguoiChuyenTien = c.Guid(nullable: false),
                    ID_NguoiNhanTien = c.Guid(nullable: false),
                    ThoiGian = c.DateTime(nullable: false),
                    SoTien = c.Double(nullable: false),
                    GhiChu = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HT_NguoiDung", t => t.ID_NguoiChuyenTien)
                .ForeignKey("dbo.HT_NguoiDung", t => t.ID_NguoiNhanTien)
                .Index(t => t.ID_NguoiChuyenTien)
                .Index(t => t.ID_NguoiNhanTien);

            CreateTable(
                "dbo.HeThong_SMS_TinMau",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    NoiDung = c.String(),
                    LoaiTin = c.Int(nullable: false),
                    LaMacDinh = c.Boolean(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_ViTriHangHoa",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_HangHoa = c.Guid(nullable: false),
                    ID_ViTri = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_HangHoa_ViTri", t => t.ID_ViTri, cascadeDelete: true)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa, cascadeDelete: true)
                .Index(t => t.ID_HangHoa)
                .Index(t => t.ID_ViTri);

            CreateTable(
                "dbo.DM_HangHoa_ViTri",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaViTri = c.String(),
                    TenViTri = c.String(),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_MayChamCong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaMCC = c.String(),
                    TenMCC = c.String(),
                    TenHienThi = c.String(),
                    IP = c.String(),
                    ID_ChiNhanh = c.Guid(nullable: false),
                    LoaiKetNoi = c.Int(nullable: false),
                    CongCOM = c.Int(nullable: false),
                    TocDoCOM = c.Int(nullable: false),
                    TenMien = c.String(),
                    LoaiHinh = c.Int(nullable: false),
                    SoDangKy = c.String(),
                    MatMa = c.Int(nullable: false),
                    VaoRa = c.Int(nullable: false),
                    Port = c.Int(nullable: false),
                    SoSeries = c.String(),
                    GhiChu = c.String(),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_ChiNhanh, cascadeDelete: true)
                .Index(t => t.ID_ChiNhanh);

            CreateTable(
                "dbo.NS_DuLieuCongTho",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaChamCong = c.String(),
                    ThoiGian = c.DateTime(nullable: false),
                    ID_MCC = c.Guid(nullable: false),
                    VaoRa = c.Int(nullable: false),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_MayChamCong", t => t.ID_MCC, cascadeDelete: true)
                .Index(t => t.ID_MCC);

            CreateTable(
                "dbo.NS_MaChamCong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    ID_MCC = c.Guid(nullable: false),
                    MaChamCong = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_MayChamCong", t => t.ID_MCC, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_MCC);

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
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_CaLamViec", t => t.ID_CaLamViec, cascadeDelete: true)
                .ForeignKey("dbo.NS_KyHieuCong", t => t.ID_KyHieu, cascadeDelete: true)
                .ForeignKey("dbo.NS_KyTinhCong", t => t.ID_KyTinhCong, cascadeDelete: true)
                .ForeignKey("dbo.NS_MaChamCong", t => t.ID_MaChamCong)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_MaChamCong)
                .Index(t => t.ID_NhanVien)
                .Index(t => t.ID_KyTinhCong)
                .Index(t => t.ID_KyHieu)
                .Index(t => t.ID_CaLamViec);

            CreateTable(
                "dbo.NS_CaLamViec",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaCa = c.String(),
                    TenCa = c.String(),
                    GioVao = c.DateTime(nullable: false),
                    GioRa = c.DateTime(nullable: false),
                    TongGioCong = c.Double(nullable: false),
                    NghiGiuaCaTu = c.DateTime(),
                    NghiGiuaCaDen = c.DateTime(),
                    GioOTBanNgayTu = c.DateTime(),
                    GioOTBanNgayDen = c.DateTime(),
                    GioOTBanDemTu = c.DateTime(),
                    GioOTBanDemDen = c.DateTime(),
                    ThoiGianDiMuonVeSom = c.Int(),
                    SoPhutDiMuon = c.Int(),
                    SoPhutVeSom = c.Int(),
                    GioVaoTu = c.DateTime(),
                    GioVaoDen = c.DateTime(),
                    GioRaTu = c.DateTime(),
                    GioRaDen = c.DateTime(),
                    TinhOTBanNgayTu = c.DateTime(),
                    TinhOTBanNgayDen = c.DateTime(),
                    TinhOTBanDemTu = c.DateTime(),
                    TinhOTBanDemDen = c.DateTime(),
                    LaCaDem = c.Int(nullable: false),
                    CachLayGioCong = c.Int(nullable: false),
                    SoGioOTToiThieu = c.Double(nullable: false),
                    GhiChuCaLamViec = c.String(),
                    GhiChuTinhGio = c.String(),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                    CaLamViec_KhongDau = c.String(),
                    CaLamViec_ChuCaiDau = c.String(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_PhanCaChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_PhieuPhanCaChiTiet = c.Guid(nullable: false),
                    ID_CaLamViec = c.Guid(nullable: false),
                    GiaTri = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_PhieuPhanCa_ChiTiet", t => t.ID_PhieuPhanCaChiTiet, cascadeDelete: true)
                .ForeignKey("dbo.NS_CaLamViec", t => t.ID_CaLamViec, cascadeDelete: true)
                .Index(t => t.ID_PhieuPhanCaChiTiet)
                .Index(t => t.ID_CaLamViec);

            CreateTable(
                "dbo.NS_PhieuPhanCa_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_PhieuPhanCa = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                    LoaiPhanCa = c.Int(nullable: false),
                    GhiChu = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_PhieuPhanCa", t => t.ID_PhieuPhanCa, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_PhieuPhanCa)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.NS_PhieuPhanCa",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    MaPhieu = c.String(),
                    TuNgay = c.DateTime(nullable: false),
                    DenNgay = c.DateTime(),
                    ID_NhanVienTao = c.Guid(nullable: false),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                    GhiChu = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienTao)
                .Index(t => t.ID_NhanVienTao);

            CreateTable(
                "dbo.NS_KyHieuCong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    KyHieu = c.String(),
                    MoTa = c.String(),
                    TrangThai = c.Int(nullable: false),
                    TrangThaiCong = c.String(),
                    CongQuyDoi = c.Double(nullable: false),
                    LayGioMacDinh = c.Boolean(nullable: false),
                    GioVao = c.DateTime(),
                    GioRa = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.NS_KyTinhCong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    Ky = c.Int(nullable: false),
                    TuNgay = c.DateTime(nullable: false),
                    DenNgay = c.DateTime(nullable: false),
                    NamNhuan = c.Int(nullable: false),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.DM_DoiTuong_TrangThai",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenTrangThai = c.String(),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.ChietKhauMacDinh_HoaDon",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    TinhChietKhauTheo = c.Int(nullable: false),
                    GiaTriChietKhau = c.Double(nullable: false),
                    ChungTuApDung = c.String(),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                    NgayTao = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.ChietKhauDoanhThu",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    TinhChietKhauTheo = c.Int(nullable: false),
                    ApDungTuNgay = c.DateTime(nullable: false),
                    ApDungDenNgay = c.DateTime(),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                    NgayTao = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonVi);

            CreateTable(
                "dbo.ChietKhauDoanhThu_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_ChietKhauDoanhThu = c.Guid(nullable: false),
                    DoanhThuTu = c.Double(nullable: false),
                    DoanhThuDen = c.Double(nullable: false),
                    GiaTriChietKhau = c.Double(nullable: false),
                    LaPhanTram = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ChietKhauDoanhThu", t => t.ID_ChietKhauDoanhThu, cascadeDelete: true)
                .Index(t => t.ID_ChietKhauDoanhThu);

            CreateTable(
                "dbo.ChietKhauDoanhThu_NhanVien",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_ChietKhauDoanhThu = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ChietKhauDoanhThu", t => t.ID_ChietKhauDoanhThu, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_ChietKhauDoanhThu)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.ChietKhauMacDinh_HoaDon_ChiTiet",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_ChietKhauHoaDon = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ChietKhauMacDinh_HoaDon", t => t.ID_ChietKhauHoaDon, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_ChietKhauHoaDon)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.ChamSocKhachHang_NhanVien",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_ChamSocKhachHang = c.Guid(nullable: false),
                    ID_NhanVien = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ChamSocKhachHangs", t => t.ID_ChamSocKhachHang, cascadeDelete: true)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVien, cascadeDelete: true)
                .Index(t => t.ID_ChamSocKhachHang)
                .Index(t => t.ID_NhanVien);

            CreateTable(
                "dbo.OptinForm_DoiTuong",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    AnhDaiDien = c.String(),
                    TenDoiTuong = c.String(),
                    TenDoiTuong_KhongDau = c.String(),
                    TenDoiTuong_ChuCaiDau = c.String(),
                    GioiTinh = c.Int(nullable: false),
                    NgaySinh = c.DateTime(),
                    SoDienThoai = c.String(),
                    Email = c.String(),
                    DiaChi = c.String(),
                    ID_TinhThanh = c.Guid(),
                    ID_QuanHuyen = c.Guid(),
                    MaSoThue = c.String(),
                    LaCaNhan = c.Boolean(nullable: false),
                    NguoiGioiThieu = c.String(),
                    ID_NhanVienPhuTrach = c.Guid(),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                    NgayTao = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_QuanHuyen", t => t.ID_QuanHuyen)
                .ForeignKey("dbo.DM_TinhThanh", t => t.ID_TinhThanh)
                .ForeignKey("dbo.NS_NhanVien", t => t.ID_NhanVienPhuTrach)
                .Index(t => t.ID_TinhThanh)
                .Index(t => t.ID_QuanHuyen)
                .Index(t => t.ID_NhanVienPhuTrach);

            CreateTable(
                "dbo.OptinForm_Link",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_OptinForm = c.Guid(nullable: false),
                    ID_DoiTuongOptinForm = c.Guid(),
                    ID_ChamSocKhachHang = c.Guid(),
                    Link = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OptinForm", t => t.ID_OptinForm, cascadeDelete: true)
                .ForeignKey("dbo.OptinForm_DoiTuong", t => t.ID_DoiTuongOptinForm)
                .ForeignKey("dbo.ChamSocKhachHangs", t => t.ID_ChamSocKhachHang)
                .Index(t => t.ID_OptinForm)
                .Index(t => t.ID_DoiTuongOptinForm)
                .Index(t => t.ID_ChamSocKhachHang);

            CreateTable(
                "dbo.OptinForm",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenOptinForm = c.String(),
                    TenOptinForm_KhongDau = c.String(),
                    TenOptinForm_ChuCaiDau = c.String(),
                    LoaiOptinForm = c.Int(nullable: false),
                    NoiDung = c.String(),
                    MaNhung = c.String(),
                    TuNgay = c.DateTime(nullable: false),
                    DenNgay = c.DateTime(),
                    TrangThaiThoiGian = c.Boolean(nullable: false),
                    LoaiThoiGian = c.Int(nullable: false),
                    KhoangCachThoiGian = c.Int(nullable: false),
                    SoLuotTruyCap = c.Double(nullable: false),
                    NguoiTao = c.String(),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(),
                    NgaySua = c.DateTime(),
                    GhiChu = c.String(),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.OptinForm_NgayLamViec",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_OptinForm = c.Guid(nullable: false),
                    NgayLamViec = c.Int(nullable: false),
                    TrangThaiLamViec = c.Boolean(nullable: false),
                    ThoiGianBatDau = c.Int(nullable: false),
                    ThoiGianKetThuc = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OptinForm", t => t.ID_OptinForm, cascadeDelete: true)
                .Index(t => t.ID_OptinForm);

            CreateTable(
                "dbo.OptinForm_NgayNghiLe",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_OptinForm = c.Guid(nullable: false),
                    NgayNghiLe = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OptinForm", t => t.ID_OptinForm, cascadeDelete: true)
                .Index(t => t.ID_OptinForm);

            CreateTable(
                "dbo.OptinForm_ThietLap",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_OptinForm = c.Guid(nullable: false),
                    ID_TruongThongTin = c.Guid(nullable: false),
                    TrangThaiSuDung = c.Boolean(nullable: false),
                    TrangThaiBatBuoc = c.Boolean(nullable: false),
                    HienThiGoiY = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OptinForm_TruongThongTin", t => t.ID_TruongThongTin, cascadeDelete: true)
                .ForeignKey("dbo.OptinForm", t => t.ID_OptinForm, cascadeDelete: true)
                .Index(t => t.ID_OptinForm)
                .Index(t => t.ID_TruongThongTin);

            CreateTable(
                "dbo.OptinForm_TruongThongTin",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    TenTruongThongTin = c.String(),
                    LoaiTruongThongTin = c.Int(nullable: false),
                    STT = c.Int(nullable: false),
                    TrangThai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.OptinForm_ThietLapThongBao",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_OptinForm = c.Guid(nullable: false),
                    NoiDungThongBao = c.String(),
                    WebDieuHuong = c.String(),
                    ButtonName = c.String(),
                    NoiDungHieuLuc = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OptinForm", t => t.ID_OptinForm, cascadeDelete: true)
                .Index(t => t.ID_OptinForm);

            CreateTable(
                "dbo.DM_HangHoa_TonKho",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_DonViQuyDoi = c.Guid(nullable: false),
                    ID_DonVi = c.Guid(nullable: false),
                    ID_LoHang = c.Guid(),
                    TonKho = c.Double(nullable: false, defaultValue: 0),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_LoHang", t => t.ID_LoHang)
                .ForeignKey("dbo.DonViQuiDoi", t => t.ID_DonViQuyDoi, cascadeDelete: true)
                .ForeignKey("dbo.DM_DonVi", t => t.ID_DonVi, cascadeDelete: true)
                .Index(t => t.ID_DonViQuyDoi)
                .Index(t => t.ID_DonVi)
                .Index(t => t.ID_LoHang);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_HoaDonLienQuan", "dbo.BH_HoaDon");
            DropForeignKey("dbo.Kho_HoaDon", "ID_ChungTuLienQuan", "dbo.BH_HoaDon");
            DropForeignKey("dbo.DM_ViTri", "ID_LoaiPhong", "dbo.DM_LoaiPhong");
            DropForeignKey("dbo.DM_ViTri", "ID_KhuVuc", "dbo.DM_KhuVuc");
            DropForeignKey("dbo.BH_HoaDon", "ID_ViTri", "dbo.DM_ViTri");
            DropForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_HoaDon", "dbo.BH_HoaDon");
            DropForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_ViTri", "dbo.DM_ViTri");
            DropForeignKey("dbo.Kho_HoaDon_ChiTiet", "ID_CTChungTuLienQuan", "dbo.BH_HoaDon_ChiTiet");
            DropForeignKey("dbo.BH_NhanVienThucHien", "ID_ChiTietHoaDon", "dbo.BH_HoaDon_ChiTiet");
            DropForeignKey("dbo.The_TheKhachHang", "ID_NhanVienLap", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.Quy_HoaDon", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_QuaTrinhCongTac", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_HoSoLuong", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_LuongDoanhThu_ChiTiet", "ID_LuongDoanhThu", "dbo.NS_LuongDoanhThu");
            DropForeignKey("dbo.NS_HoSoLuong", "ID_LuongDoanhThu", "dbo.NS_LuongDoanhThu");
            DropForeignKey("dbo.Kho_HoaDon", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.HT_NhatKySuDung", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.HT_NguoiDung", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.DM_KhuyenMai_ApDung", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.DM_GiaBan_ApDung", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.DM_DoiTuong", "ID_NhanVienPhuTrach", "dbo.NS_NhanVien");
            DropForeignKey("dbo.ChietKhauMacDinh_NhanVien", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_NhanVienQuanLy", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_NhanVien_Anh", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.The_TheKhachHang", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.Kho_HoaDon", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.DM_DoiTuong", "ID_NguonKhach", "dbo.DM_NguonKhachHang");
            DropForeignKey("dbo.DM_LienHe", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.DM_HangHoa", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.The_TheKhachHang", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.Quy_TonQuyKhoiTao", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.Quy_HoaDon", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_QuaTrinhCongTac", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NhomHangHoa_DonVi", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NhomDoiTuong_DonVi", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.Kho_TonKhoKhoiTao", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.Kho_HoaDon", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.Kho_DonVi", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.HT_NhatKySuDung", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.HT_NguoiDung", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.HT_QuyenMacDinh", "IDNguoiDung", "dbo.HT_NguoiDung");
            DropForeignKey("dbo.HT_NguoiDung_Nhom", "IDNguoiDung", "dbo.HT_NguoiDung");
            DropForeignKey("dbo.HT_Quyen_Nhom", "ID_NhomNguoiDung", "dbo.HT_NhomNguoiDung");
            DropForeignKey("dbo.HT_Quyen_Nhom", "MaQuyen", "dbo.HT_Quyen");
            DropForeignKey("dbo.HT_NguoiDung_Nhom", "IDNhomNguoiDung", "dbo.HT_NhomNguoiDung");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_LoaiTuVan", "dbo.DM_LoaiTuVanLichHen");
            DropForeignKey("dbo.HT_MaChungTu", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.HT_CauHinhPhanMem", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_LopHoc", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_KhuyenMai_ApDung", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_GiaBan_ApDung", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_DonVi", "ID_Parent", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_DoiTuong", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.ChietKhauMacDinh_NhanVien", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.The_TheKhachHang_ChiTiet", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.Kho_TonKhoKhoiTao", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.Kho_HoaDon_ChiTiet", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.DM_KhuyenMai_ChiTiet", "ID_DonViQuiDoiMua", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.DM_KhuyenMai_ChiTiet", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.DM_GiaBan_ChiTiet", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.DM_GiaBan_ChiTiet", "ID_GiaBan", "dbo.DM_GiaBan");
            DropForeignKey("dbo.DM_GiaBan_ApDung", "ID_GiaBan", "dbo.DM_GiaBan");
            DropForeignKey("dbo.DM_DoiTuong_Nhom", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.DM_NhomDoiTuong_ChiTiet", "ID_NhomDoiTuong", "dbo.DM_NhomDoiTuong");
            DropForeignKey("dbo.DM_DoiTuong_Nhom", "ID_NhomDoiTuong", "dbo.DM_NhomDoiTuong");
            DropForeignKey("dbo.NhomDoiTuong_DonVi", "ID_NhomDoiTuong", "dbo.DM_NhomDoiTuong");
            DropForeignKey("dbo.HT_CauHinh_TichDiemApDung", "ID_NhomDoiTuong", "dbo.DM_NhomDoiTuong");
            DropForeignKey("dbo.HT_CauHinh_TichDiemChiTiet", "ID_CauHinh", "dbo.HT_CauHinhPhanMem");
            DropForeignKey("dbo.HT_CauHinh_GioiHanTraHang", "ID_CauHinh", "dbo.HT_CauHinhPhanMem");
            DropForeignKey("dbo.HT_CauHinh_TichDiemApDung", "ID_TichDiem", "dbo.HT_CauHinh_TichDiemChiTiet");
            DropForeignKey("dbo.DM_KhuyenMai_ApDung", "ID_NhomKhachHang", "dbo.DM_NhomDoiTuong");
            DropForeignKey("dbo.DM_KhuyenMai_ChiTiet", "ID_KhuyenMai", "dbo.DM_KhuyenMai");
            DropForeignKey("dbo.NhomHangHoa_DonVi", "ID_NhomHangHoa", "dbo.DM_NhomHangHoa");
            DropForeignKey("dbo.DM_NhomHangHoa", "ID_Parent", "dbo.DM_NhomHangHoa");
            DropForeignKey("dbo.DM_KhuyenMai_ChiTiet", "ID_NhomHangHoaMua", "dbo.DM_NhomHangHoa");
            DropForeignKey("dbo.DM_KhuyenMai_ChiTiet", "ID_NhomHangHoa", "dbo.DM_NhomHangHoa");
            DropForeignKey("dbo.DM_HangHoa", "ID_NhomHang", "dbo.DM_NhomHangHoa");
            DropForeignKey("dbo.HangHoa_ThuocTinh", "ID_HangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.HangHoa_ThuocTinh", "ID_ThuocTinh", "dbo.DM_ThuocTinh");
            DropForeignKey("dbo.DonViQuiDoi", "ID_HangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.DM_TinhThanh", "ID_QuocGia", "dbo.DM_QuocGia");
            DropForeignKey("dbo.DM_TinhThanh", "ID_VungMien", "dbo.DM_VungMien");
            DropForeignKey("dbo.DM_NhomDoiTuong_ChiTiet", "GiaTriKhuVuc", "dbo.DM_TinhThanh");
            DropForeignKey("dbo.DM_NhomDoiTuong_ChiTiet", "GiaTriVungMien", "dbo.DM_VungMien");
            DropForeignKey("dbo.DM_QuanHuyen", "ID_TinhThanh", "dbo.DM_TinhThanh");
            DropForeignKey("dbo.DM_DoiTuong", "ID_QuanHuyen", "dbo.DM_QuanHuyen");
            DropForeignKey("dbo.DM_XaPhuong", "ID_QuanHuyen", "dbo.DM_QuanHuyen");
            DropForeignKey("dbo.DM_DoiTuong", "ID_TinhThanh", "dbo.DM_TinhThanh");
            DropForeignKey("dbo.DM_TienTe", "ID_QuocGia", "dbo.DM_QuocGia");
            DropForeignKey("dbo.The_TheKhachHang", "ID_TienTe", "dbo.DM_TienTe");
            DropForeignKey("dbo.Quy_TonQuyKhoiTao", "ID_TienTe", "dbo.DM_TienTe");
            DropForeignKey("dbo.Quy_HoaDon", "ID_NgoaiTe", "dbo.DM_TienTe");
            DropForeignKey("dbo.Kho_HoaDon", "ID_NgoaiTe", "dbo.DM_TienTe");
            DropForeignKey("dbo.Kho_HoaDon_ChiTiet", "ID_HoaDon", "dbo.Kho_HoaDon");
            DropForeignKey("dbo.Kho_TonKhoKhoiTao", "ID_Kho", "dbo.DM_Kho");
            DropForeignKey("dbo.Kho_TonKhoKhoiTao", "ID_LoHang", "dbo.DM_LoHang");
            DropForeignKey("dbo.Kho_HoaDon_ChiTiet", "ID_LoHang", "dbo.DM_LoHang");
            DropForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_LoHang", "dbo.DM_LoHang");
            DropForeignKey("dbo.Kho_HoaDon_ChiTiet", "ID_Kho", "dbo.DM_Kho");
            DropForeignKey("dbo.Kho_DonVi", "ID_Kho", "dbo.DM_Kho");
            DropForeignKey("dbo.DM_GiaBan_ChiTiet", "ID_KhoHang", "dbo.DM_Kho");
            DropForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_KhoHang", "dbo.DM_Kho");
            DropForeignKey("dbo.Quy_HoaDon", "LoaiHoaDon", "dbo.DM_LoaiChungTu");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_HoaDon", "dbo.Quy_HoaDon");
            DropForeignKey("dbo.The_TheKhachHang_ChiTiet", "ID_TheKhachHang", "dbo.The_TheKhachHang");
            DropForeignKey("dbo.The_TheKhachHang", "ID_NhomThe", "dbo.The_NhomThe");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_TheKhachHang", "dbo.The_TheKhachHang");
            DropForeignKey("dbo.DM_LienHe_Anh", "ID_LienHe", "dbo.DM_LienHe");
            DropForeignKey("dbo.The_TheKhachHang", "ID_LienHe", "dbo.DM_LienHe");
            DropForeignKey("dbo.NS_QuaTrinhCongTac", "ID_ChucVu", "dbo.DM_ChucVu");
            DropForeignKey("dbo.DM_LienHe", "ID_ChucVu", "dbo.DM_ChucVu");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_KhoanThuChi", "dbo.Quy_KhoanThuChi");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_NganHang", "dbo.DM_NganHang");
            DropForeignKey("dbo.HT_CongTy", "ID_NganHang", "dbo.DM_NganHang");
            DropForeignKey("dbo.DM_DonVi", "ID_NganHang", "dbo.DM_NganHang");
            DropForeignKey("dbo.DM_DoiTuong", "ID_NganHang", "dbo.DM_NganHang");
            DropForeignKey("dbo.Kho_HoaDon", "LoaiChungTu", "dbo.DM_LoaiChungTu");
            DropForeignKey("dbo.BH_HoaDon", "LoaiHoaDon", "dbo.DM_LoaiChungTu");
            DropForeignKey("dbo.DM_MauIn", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_MauIn", "ID_LoaiChungTu", "dbo.DM_LoaiChungTu");
            DropForeignKey("dbo.DM_TyGia", "ID_TienTe", "dbo.DM_TienTe");
            DropForeignKey("dbo.DM_GiaBan_ChiTiet", "ID_NgoaiTe", "dbo.DM_TienTe");
            DropForeignKey("dbo.BH_HoaDon", "ID_NgoaiTe", "dbo.DM_TienTe");
            DropForeignKey("dbo.DM_HangHoa", "ID_QuocGia", "dbo.DM_QuocGia");
            DropForeignKey("dbo.DM_DoiTuong", "ID_QuocGia", "dbo.DM_QuocGia");
            DropForeignKey("dbo.DM_HangHoa", "ID_PhanLoai", "dbo.DM_PhanLoaiHangHoaDichVu");
            DropForeignKey("dbo.DM_HangHoa_Anh", "ID_HangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.DM_KhuyenMai_ApDung", "ID_KhuyenMai", "dbo.DM_KhuyenMai");
            DropForeignKey("dbo.DM_GiaBan_ApDung", "ID_NhomKhachHang", "dbo.DM_NhomDoiTuong");
            DropForeignKey("dbo.DM_DoiTuong", "ID_NhomDoiTuong", "dbo.DM_NhomDoiTuong");
            DropForeignKey("dbo.BH_HoaDon", "ID_BangGia", "dbo.DM_GiaBan");
            DropForeignKey("dbo.DinhLuongDichVu", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.ChietKhauMacDinh_NhanVien", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.BH_HoaDon", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_DoiTuong", "ID_NguoiGioiThieu", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_KhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.BH_HoaDon", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.BH_NhanVienThucHien", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.BH_HoaDon", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_KhuyenMai", "dbo.DM_KhuyenMai");
            DropForeignKey("dbo.BH_HoaDon", "ID_KhuyenMai", "dbo.DM_KhuyenMai");
            DropForeignKey("dbo.DM_DoiTuong_Anh", "ID_DoiTuong", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.DM_TaiKhoanNganHang", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_TaiKhoanNganHang", "ID_NganHang", "dbo.DM_NganHang");
            DropForeignKey("dbo.Quy_HoaDon_ChiTiet", "ID_TaiKhoanNganHang", "dbo.DM_TaiKhoanNganHang");
            DropForeignKey("dbo.HT_ThongBao", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.ChotSo_KhachHang", "ID_KhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.ChotSo_KhachHang", "ID_KhachHang", "dbo.DM_DonVi");
            DropForeignKey("dbo.ChotSo_HangHoa", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.ChotSo", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.ChotSo_HangHoa", "ID_HangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.HT_ThongBao_CaiDat", "ID_NguoiDung", "dbo.HT_NguoiDung");
            DropForeignKey("dbo.HT_NguoiDung_Nhom", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.ChotSo_HangHoa", "ID_LoHang", "dbo.DM_LoHang");
            DropForeignKey("dbo.DM_GiaVon", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_GiaVon", "ID_DonViQuiDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.DM_GiaVon", "ID_LoHang", "dbo.DM_LoHang");
            DropForeignKey("dbo.NS_PhongBan", "ID_PhongBanCha", "dbo.NS_PhongBan");
            DropForeignKey("dbo.NS_NhanVien", "ID_NSPhongBan", "dbo.NS_PhongBan");
            DropForeignKey("dbo.NS_NhanVien", "ID_TinhThanhTT", "dbo.DM_TinhThanh");
            DropForeignKey("dbo.NS_NhanVien", "ID_TinhThanhHKTT", "dbo.DM_TinhThanh");
            DropForeignKey("dbo.NS_NhanVien", "ID_QuanHuyenTT", "dbo.DM_QuanHuyen");
            DropForeignKey("dbo.NS_NhanVien", "ID_QuanHuyenHKTT", "dbo.DM_QuanHuyen");
            DropForeignKey("dbo.NS_NhanVien", "ID_XaPhuongTT", "dbo.DM_XaPhuong");
            DropForeignKey("dbo.NS_NhanVien", "ID_XaPhuongHKTT", "dbo.DM_XaPhuong");
            DropForeignKey("dbo.DM_LienHe", "ID_TinhThanh", "dbo.DM_TinhThanh");
            DropForeignKey("dbo.DM_LienHe", "ID_QuanHuyen", "dbo.DM_QuanHuyen");
            DropForeignKey("dbo.NS_CongViec", "ID_NhanVienChiaSe", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_CongViec", "ID_KhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.NS_CongViec", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_CongViec", "ID_LienHe", "dbo.DM_LienHe");
            DropForeignKey("dbo.NS_CongViec", "ID_LoaiCongViec", "dbo.NS_CongViec_PhanLoai");
            DropForeignKey("dbo.NS_NhanVien_SucKhoe", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_NhanVien_GiaDinh", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_NhanVien_DaoTao", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_NhanVien_CongTac", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_NhanVien", "ID_QuocGia", "dbo.DM_QuocGia");
            DropForeignKey("dbo.NS_MienGiamThue", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_Luong_PhuCap", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_Luong_PhuCap", "ID_LoaiLuong", "dbo.NS_LoaiLuong");
            DropForeignKey("dbo.NS_KhenThuong", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_HopDong", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_BaoHiem", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_CongViec", "ID_NhanVienQuanLy", "dbo.NS_NhanVien");
            DropForeignKey("dbo.HeThong_SMS", "ID_KhachHang", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.NS_PhongBan", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.HeThong_SMS", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.HeThong_SMS_TaiKhoan", "ID_NguoiNhanTien", "dbo.HT_NguoiDung");
            DropForeignKey("dbo.HeThong_SMS_TaiKhoan", "ID_NguoiChuyenTien", "dbo.HT_NguoiDung");
            DropForeignKey("dbo.HeThong_SMS", "ID_NguoiGui", "dbo.HT_NguoiDung");
            DropForeignKey("dbo.HeThong_SMS", "ID_HoaDon", "dbo.BH_HoaDon");
            DropForeignKey("dbo.NS_QuaTrinhCongTac", "ID_PhongBan", "dbo.NS_PhongBan");
            DropForeignKey("dbo.NS_PhieuPhanCa_ChiTiet", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_PhieuPhanCa", "ID_NhanVienTao", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_MaChamCong", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.NS_ChamCong", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.DM_DoiTuong", "ID_TrangThai", "dbo.DM_DoiTuong_TrangThai");
            DropForeignKey("dbo.NS_MayChamCong", "ID_ChiNhanh", "dbo.DM_DonVi");
            DropForeignKey("dbo.NS_MaChamCong", "ID_MCC", "dbo.NS_MayChamCong");
            DropForeignKey("dbo.NS_ChamCong", "ID_MaChamCong", "dbo.NS_MaChamCong");
            DropForeignKey("dbo.NS_ChamCong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_ChamCong", "ID_KyHieu", "dbo.NS_KyHieuCong");
            DropForeignKey("dbo.NS_PhanCaChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.NS_PhieuPhanCa_ChiTiet", "ID_PhieuPhanCa", "dbo.NS_PhieuPhanCa");
            DropForeignKey("dbo.NS_PhanCaChiTiet", "ID_PhieuPhanCaChiTiet", "dbo.NS_PhieuPhanCa_ChiTiet");
            DropForeignKey("dbo.NS_ChamCong", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.NS_DuLieuCongTho", "ID_MCC", "dbo.NS_MayChamCong");
            DropForeignKey("dbo.DM_ViTriHangHoa", "ID_HangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.DM_ViTriHangHoa", "ID_ViTri", "dbo.DM_HangHoa_ViTri");
            DropForeignKey("dbo.ChietKhauMacDinh_HoaDon", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.BH_NhanVienThucHien", "ID_HoaDon", "dbo.BH_HoaDon");
            DropForeignKey("dbo.ChietKhauMacDinh_HoaDon_ChiTiet", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.ChietKhauDoanhThu_NhanVien", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.ChietKhauMacDinh_HoaDon_ChiTiet", "ID_ChietKhauHoaDon", "dbo.ChietKhauMacDinh_HoaDon");
            DropForeignKey("dbo.ChietKhauDoanhThu", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.ChietKhauDoanhThu_NhanVien", "ID_ChietKhauDoanhThu", "dbo.ChietKhauDoanhThu");
            DropForeignKey("dbo.ChietKhauDoanhThu_ChiTiet", "ID_ChietKhauDoanhThu", "dbo.ChietKhauDoanhThu");
            DropForeignKey("dbo.OptinForm_DoiTuong", "ID_NhanVienPhuTrach", "dbo.NS_NhanVien");
            DropForeignKey("dbo.ChamSocKhachHang_NhanVien", "ID_NhanVien", "dbo.NS_NhanVien");
            DropForeignKey("dbo.OptinForm_Link", "ID_ChamSocKhachHang", "dbo.ChamSocKhachHangs");
            DropForeignKey("dbo.OptinForm_DoiTuong", "ID_TinhThanh", "dbo.DM_TinhThanh");
            DropForeignKey("dbo.OptinForm_DoiTuong", "ID_QuanHuyen", "dbo.DM_QuanHuyen");
            DropForeignKey("dbo.OptinForm_Link", "ID_DoiTuongOptinForm", "dbo.OptinForm_DoiTuong");
            DropForeignKey("dbo.OptinForm_ThietLapThongBao", "ID_OptinForm", "dbo.OptinForm");
            DropForeignKey("dbo.OptinForm_ThietLap", "ID_OptinForm", "dbo.OptinForm");
            DropForeignKey("dbo.OptinForm_ThietLap", "ID_TruongThongTin", "dbo.OptinForm_TruongThongTin");
            DropForeignKey("dbo.OptinForm_NgayNghiLe", "ID_OptinForm", "dbo.OptinForm");
            DropForeignKey("dbo.OptinForm_NgayLamViec", "ID_OptinForm", "dbo.OptinForm");
            DropForeignKey("dbo.OptinForm_Link", "ID_OptinForm", "dbo.OptinForm");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_HangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_ChamSocKhachHang", "dbo.ChamSocKhachHangs");
            DropForeignKey("dbo.ChamSocKhachHang_NhanVien", "ID_ChamSocKhachHang", "dbo.ChamSocKhachHangs");
            DropForeignKey("dbo.ChamSocKhachHangs", "ID_LienHe", "dbo.DM_LienHe");
            DropForeignKey("dbo.DM_HangHoa_TonKho", "ID_DonVi", "dbo.DM_DonVi");
            DropForeignKey("dbo.DM_HangHoa_TonKho", "ID_DonViQuyDoi", "dbo.DonViQuiDoi");
            DropForeignKey("dbo.DM_HangHoa_TonKho", "ID_LoHang", "dbo.DM_LoHang");

            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_LienHe" });
            DropIndex("dbo.OptinForm_ThietLapThongBao", new[] { "ID_OptinForm" });
            DropIndex("dbo.OptinForm_ThietLap", new[] { "ID_TruongThongTin" });
            DropIndex("dbo.OptinForm_ThietLap", new[] { "ID_OptinForm" });
            DropIndex("dbo.OptinForm_NgayNghiLe", new[] { "ID_OptinForm" });
            DropIndex("dbo.OptinForm_NgayLamViec", new[] { "ID_OptinForm" });
            DropIndex("dbo.OptinForm_Link", new[] { "ID_ChamSocKhachHang" });
            DropIndex("dbo.OptinForm_Link", new[] { "ID_DoiTuongOptinForm" });
            DropIndex("dbo.OptinForm_Link", new[] { "ID_OptinForm" });
            DropIndex("dbo.OptinForm_DoiTuong", new[] { "ID_NhanVienPhuTrach" });
            DropIndex("dbo.OptinForm_DoiTuong", new[] { "ID_QuanHuyen" });
            DropIndex("dbo.OptinForm_DoiTuong", new[] { "ID_TinhThanh" });
            DropIndex("dbo.ChamSocKhachHang_NhanVien", new[] { "ID_NhanVien" });
            DropIndex("dbo.ChamSocKhachHang_NhanVien", new[] { "ID_ChamSocKhachHang" });
            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_ChamSocKhachHang" });
            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_HangHoa" });
            DropIndex("dbo.ChietKhauMacDinh_HoaDon_ChiTiet", new[] { "ID_NhanVien" });
            DropIndex("dbo.ChietKhauMacDinh_HoaDon_ChiTiet", new[] { "ID_ChietKhauHoaDon" });
            DropIndex("dbo.ChietKhauDoanhThu_NhanVien", new[] { "ID_NhanVien" });
            DropIndex("dbo.ChietKhauDoanhThu_NhanVien", new[] { "ID_ChietKhauDoanhThu" });
            DropIndex("dbo.ChietKhauDoanhThu_ChiTiet", new[] { "ID_ChietKhauDoanhThu" });
            DropIndex("dbo.ChietKhauDoanhThu", new[] { "ID_DonVi" });
            DropIndex("dbo.BH_NhanVienThucHien", new[] { "ID_HoaDon" });
            DropIndex("dbo.ChietKhauMacDinh_HoaDon", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_PhieuPhanCa", new[] { "ID_NhanVienTao" });
            DropIndex("dbo.NS_PhieuPhanCa_ChiTiet", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_PhieuPhanCa_ChiTiet", new[] { "ID_PhieuPhanCa" });
            DropIndex("dbo.NS_PhanCaChiTiet", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_PhanCaChiTiet", new[] { "ID_PhieuPhanCaChiTiet" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_KyHieu" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_ChamCong", new[] { "ID_MaChamCong" });
            DropIndex("dbo.NS_MaChamCong", new[] { "ID_MCC" });
            DropIndex("dbo.NS_MaChamCong", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_DuLieuCongTho", new[] { "ID_MCC" });
            DropIndex("dbo.NS_MayChamCong", new[] { "ID_ChiNhanh" });
            DropIndex("dbo.DM_ViTriHangHoa", new[] { "ID_ViTri" });
            DropIndex("dbo.DM_ViTriHangHoa", new[] { "ID_HangHoa" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_TrangThai" });
            DropIndex("dbo.HeThong_SMS_TaiKhoan", new[] { "ID_NguoiNhanTien" });
            DropIndex("dbo.HeThong_SMS_TaiKhoan", new[] { "ID_NguoiChuyenTien" });
            DropIndex("dbo.HeThong_SMS", new[] { "ID_DonVi" });
            DropIndex("dbo.HeThong_SMS", new[] { "ID_KhachHang" });
            DropIndex("dbo.HeThong_SMS", new[] { "ID_NguoiGui" });
            DropIndex("dbo.HeThong_SMS", new[] { "ID_HoaDon" });
            DropIndex("dbo.NS_PhongBan", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_QuaTrinhCongTac", new[] { "ID_PhongBan" });
            DropIndex("dbo.NS_CongViec", new[] { "ID_NhanVienQuanLy" });
            DropIndex("dbo.NS_MienGiamThue", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_Luong_PhuCap", new[] { "ID_LoaiLuong" });
            DropIndex("dbo.NS_Luong_PhuCap", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_KhenThuong", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_HopDong", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_BaoHiem", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_QuocGia" });
            DropIndex("dbo.DM_ViTri", new[] { "ID_LoaiPhong" });
            DropIndex("dbo.DM_ViTri", new[] { "ID_KhuVuc" });
            DropIndex("dbo.NS_LuongDoanhThu_ChiTiet", new[] { "ID_LuongDoanhThu" });
            DropIndex("dbo.NS_HoSoLuong", new[] { "ID_LuongDoanhThu" });
            DropIndex("dbo.NS_HoSoLuong", new[] { "ID_NhanVien" });
            DropIndex("dbo.HT_NhatKySuDung", new[] { "ID_DonVi" });
            DropIndex("dbo.HT_NhatKySuDung", new[] { "ID_NhanVien" });
            DropIndex("dbo.HT_QuyenMacDinh", new[] { "IDNguoiDung" });
            DropIndex("dbo.HT_Quyen_Nhom", new[] { "MaQuyen" });
            DropIndex("dbo.HT_Quyen_Nhom", new[] { "ID_NhomNguoiDung" });
            DropIndex("dbo.HT_NguoiDung_Nhom", new[] { "IDNguoiDung" });
            DropIndex("dbo.HT_NguoiDung_Nhom", new[] { "IDNhomNguoiDung" });
            DropIndex("dbo.HT_NguoiDung_Nhom", new[] { "ID_DonVi" });
            DropIndex("dbo.HT_NguoiDung", new[] { "ID_DonVi" });
            DropIndex("dbo.HT_NguoiDung", new[] { "ID_NhanVien" });
            DropIndex("dbo.HT_MaChungTu", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_LopHoc", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_NhomDoiTuong_ChiTiet", new[] { "ID_NhomDoiTuong" });
            DropIndex("dbo.DM_NhomDoiTuong_ChiTiet", new[] { "GiaTriVungMien" });
            DropIndex("dbo.DM_NhomDoiTuong_ChiTiet", new[] { "GiaTriKhuVuc" });
            DropIndex("dbo.DM_DoiTuong_Nhom", new[] { "ID_NhomDoiTuong" });
            DropIndex("dbo.DM_DoiTuong_Nhom", new[] { "ID_DoiTuong" });
            DropIndex("dbo.NhomDoiTuong_DonVi", new[] { "ID_DonVi" });
            DropIndex("dbo.NhomDoiTuong_DonVi", new[] { "ID_NhomDoiTuong" });
            DropIndex("dbo.HT_CauHinhPhanMem", new[] { "ID_DonVi" });
            DropIndex("dbo.HT_CauHinh_TichDiemChiTiet", new[] { "ID_CauHinh" });
            DropIndex("dbo.HT_CauHinh_TichDiemApDung", new[] { "ID_NhomDoiTuong" });
            DropIndex("dbo.HT_CauHinh_TichDiemApDung", new[] { "ID_TichDiem" });
            DropIndex("dbo.HT_CauHinh_GioiHanTraHang", new[] { "ID_CauHinh" });
            DropIndex("dbo.NhomHangHoa_DonVi", new[] { "ID_DonVi" });
            DropIndex("dbo.NhomHangHoa_DonVi", new[] { "ID_NhomHangHoa" });
            DropIndex("dbo.HangHoa_ThuocTinh", new[] { "ID_ThuocTinh" });
            DropIndex("dbo.HangHoa_ThuocTinh", new[] { "ID_HangHoa" });
            DropIndex("dbo.DM_XaPhuong", new[] { "ID_QuanHuyen" });
            DropIndex("dbo.DM_QuanHuyen", new[] { "ID_TinhThanh" });
            DropIndex("dbo.DM_TinhThanh", new[] { "ID_VungMien" });
            DropIndex("dbo.DM_TinhThanh", new[] { "ID_QuocGia" });
            DropIndex("dbo.Quy_TonQuyKhoiTao", new[] { "ID_TienTe" });
            DropIndex("dbo.Quy_TonQuyKhoiTao", new[] { "ID_DonVi" });
            DropIndex("dbo.Kho_TonKhoKhoiTao", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.Kho_TonKhoKhoiTao", new[] { "ID_DonVi" });
            DropIndex("dbo.Kho_TonKhoKhoiTao", new[] { "ID_LoHang" });
            DropIndex("dbo.Kho_TonKhoKhoiTao", new[] { "ID_Kho" });
            DropIndex("dbo.Kho_DonVi", new[] { "ID_DonVi" });
            DropIndex("dbo.Kho_DonVi", new[] { "ID_Kho" });
            DropIndex("dbo.Kho_HoaDon_ChiTiet", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.Kho_HoaDon_ChiTiet", new[] { "ID_CTChungTuLienQuan" });
            DropIndex("dbo.Kho_HoaDon_ChiTiet", new[] { "ID_LoHang" });
            DropIndex("dbo.Kho_HoaDon_ChiTiet", new[] { "ID_Kho" });
            DropIndex("dbo.Kho_HoaDon_ChiTiet", new[] { "ID_HoaDon" });
            DropIndex("dbo.The_TheKhachHang_ChiTiet", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.The_TheKhachHang_ChiTiet", new[] { "ID_TheKhachHang" });
            DropIndex("dbo.NS_QuaTrinhCongTac", new[] { "ID_ChucVu" });
            DropIndex("dbo.NS_QuaTrinhCongTac", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_QuaTrinhCongTac", new[] { "ID_NhanVien" });
            DropIndex("dbo.DM_LienHe_Anh", new[] { "ID_LienHe" });
            DropIndex("dbo.DM_LienHe", new[] { "ID_ChucVu" });
            DropIndex("dbo.DM_LienHe", new[] { "ID_DoiTuong" });
            DropIndex("dbo.DM_LienHe", new[] { "ID_QuanHuyen" });
            DropIndex("dbo.DM_LienHe", new[] { "ID_TinhThanh" });
            DropIndex("dbo.The_TheKhachHang", new[] { "ID_LienHe" });
            DropIndex("dbo.The_TheKhachHang", new[] { "ID_DonVi" });
            DropIndex("dbo.The_TheKhachHang", new[] { "ID_NhanVienLap" });
            DropIndex("dbo.The_TheKhachHang", new[] { "ID_TienTe" });
            DropIndex("dbo.The_TheKhachHang", new[] { "ID_DoiTuong" });
            DropIndex("dbo.The_TheKhachHang", new[] { "ID_NhomThe" });
            DropIndex("dbo.HT_CongTy", new[] { "ID_NganHang" });
            DropIndex("dbo.DM_TaiKhoanNganHang", new[] { "ID_NganHang" });
            DropIndex("dbo.DM_TaiKhoanNganHang", new[] { "ID_DonVi" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_KhoanThuChi" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_NganHang" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_HoaDonLienQuan" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_TheKhachHang" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_DoiTuong" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_NhanVien" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_HoaDon" });
            DropIndex("dbo.Quy_HoaDon_ChiTiet", new[] { "ID_TaiKhoanNganHang" });
            DropIndex("dbo.Quy_HoaDon", new[] { "LoaiHoaDon" });
            DropIndex("dbo.Quy_HoaDon", new[] { "ID_DonVi" });
            DropIndex("dbo.Quy_HoaDon", new[] { "ID_NgoaiTe" });
            DropIndex("dbo.Quy_HoaDon", new[] { "ID_NhanVien" });
            DropIndex("dbo.Kho_HoaDon", new[] { "ID_ChungTuLienQuan" });
            DropIndex("dbo.Kho_HoaDon", new[] { "ID_NgoaiTe" });
            DropIndex("dbo.Kho_HoaDon", new[] { "ID_NhanVien" });
            DropIndex("dbo.Kho_HoaDon", new[] { "ID_DoiTuong" });
            DropIndex("dbo.Kho_HoaDon", new[] { "LoaiChungTu" });
            DropIndex("dbo.Kho_HoaDon", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_TyGia", new[] { "ID_TienTe" });
            DropIndex("dbo.DM_TienTe", new[] { "ID_QuocGia" });
            DropIndex("dbo.DM_HangHoa_Anh", new[] { "ID_HangHoa" });
            DropIndex("dbo.DM_HangHoa", new[] { "ID_DoiTuong" });
            DropIndex("dbo.DM_HangHoa", new[] { "ID_QuocGia" });
            DropIndex("dbo.DM_HangHoa", new[] { "ID_PhanLoai" });
            DropIndex("dbo.DM_HangHoa", new[] { "ID_NhomHang" });
            DropIndex("dbo.DM_NhomHangHoa", new[] { "ID_Parent" });
            DropIndex("dbo.DM_KhuyenMai_ChiTiet", new[] { "ID_NhomHangHoaMua" });
            DropIndex("dbo.DM_KhuyenMai_ChiTiet", new[] { "ID_DonViQuiDoiMua" });
            DropIndex("dbo.DM_KhuyenMai_ChiTiet", new[] { "ID_NhomHangHoa" });
            DropIndex("dbo.DM_KhuyenMai_ChiTiet", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.DM_KhuyenMai_ChiTiet", new[] { "ID_KhuyenMai" });
            DropIndex("dbo.DM_KhuyenMai_ApDung", new[] { "ID_NhomKhachHang" });
            DropIndex("dbo.DM_KhuyenMai_ApDung", new[] { "ID_NhanVien" });
            DropIndex("dbo.DM_KhuyenMai_ApDung", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_KhuyenMai_ApDung", new[] { "ID_KhuyenMai" });
            DropIndex("dbo.DM_GiaBan_ApDung", new[] { "ID_NhomKhachHang" });
            DropIndex("dbo.DM_GiaBan_ApDung", new[] { "ID_NhanVien" });
            DropIndex("dbo.DM_GiaBan_ApDung", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_GiaBan_ApDung", new[] { "ID_GiaBan" });
            DropIndex("dbo.DM_GiaBan_ChiTiet", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.DM_GiaBan_ChiTiet", new[] { "ID_NgoaiTe" });
            DropIndex("dbo.DM_GiaBan_ChiTiet", new[] { "ID_KhoHang" });
            DropIndex("dbo.DM_GiaBan_ChiTiet", new[] { "ID_GiaBan" });
            DropIndex("dbo.DinhLuongDichVu", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.DonViQuiDoi", new[] { "ID_HangHoa" });
            DropIndex("dbo.ChietKhauMacDinh_NhanVien", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.ChietKhauMacDinh_NhanVien", new[] { "ID_DonVi" });
            DropIndex("dbo.ChietKhauMacDinh_NhanVien", new[] { "ID_NhanVien" });
            DropIndex("dbo.DM_MauIn", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_MauIn", new[] { "ID_LoaiChungTu" });
            DropIndex("dbo.DM_DonVi", new[] { "ID_NganHang" });
            DropIndex("dbo.DM_DonVi", new[] { "ID_Parent" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_NganHang" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_QuocGia" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_NguonKhach" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_NguoiGioiThieu" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_NhanVienPhuTrach" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_QuanHuyen" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_TinhThanh" });
            DropIndex("dbo.DM_DoiTuong", new[] { "ID_NhomDoiTuong" });
            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_NhanVien" });
            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_DonVi" });
            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_LoaiTuVan" });
            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_KhachHang" });
            DropIndex("dbo.ChamSocKhachHangs", new[] { "ID_NhanVienQuanLy" });
            DropIndex("dbo.BH_NhanVienThucHien", new[] { "ID_ChiTietHoaDon" });
            DropIndex("dbo.BH_NhanVienThucHien", new[] { "ID_NhanVien" });
            DropIndex("dbo.BH_HoaDon_ChiTiet", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.BH_HoaDon_ChiTiet", new[] { "ID_LoHang" });
            DropIndex("dbo.BH_HoaDon_ChiTiet", new[] { "ID_KhoHang" });
            DropIndex("dbo.BH_HoaDon_ChiTiet", new[] { "ID_HoaDon" });
            DropIndex("dbo.BH_HoaDon_ChiTiet", new[] { "ID_KhuyenMai" });
            DropIndex("dbo.BH_HoaDon_ChiTiet", new[] { "ID_ViTri" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_DonVi" });
            DropIndex("dbo.BH_HoaDon", new[] { "LoaiHoaDon" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_NhanVien" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_BangGia" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_NgoaiTe" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_DoiTuong" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_ViTri" });
            DropIndex("dbo.BH_HoaDon", new[] { "ID_KhuyenMai" });
            DropIndex("dbo.DM_DoiTuong_Anh", new[] { "ID_DoiTuong" });
            DropIndex("dbo.HT_ThongBao", new[] { "ID_DonVi" });
            DropIndex("dbo.ChotSo", new[] { "ID_DonVi" });
            DropIndex("dbo.ChotSo_HangHoa", new[] { "ID_HangHoa" });
            DropIndex("dbo.ChotSo_HangHoa", new[] { "ID_DonVi" });
            DropIndex("dbo.ChotSo_HangHoa", new[] { "ID_LoHang" });
            DropIndex("dbo.ChotSo_KhachHang", new[] { "ID_KhachHang" });
            DropIndex("dbo.HT_ThongBao_CaiDat", new[] { "ID_NguoiDung" });
            DropIndex("dbo.DM_GiaVon", new[] { "ID_LoHang" });
            DropIndex("dbo.DM_GiaVon", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_GiaVon", new[] { "ID_DonViQuiDoi" });
            DropIndex("dbo.NS_NhanVien_Anh", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_PhongBan", new[] { "ID_PhongBanCha" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_XaPhuongHKTT" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_QuanHuyenHKTT" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_TinhThanhHKTT" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_XaPhuongTT" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_QuanHuyenTT" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_TinhThanhTT" });
            DropIndex("dbo.NS_NhanVien", new[] { "ID_NSPhongBan" });
            DropIndex("dbo.NS_CongViec", new[] { "ID_NhanVienChiaSe" });
            DropIndex("dbo.NS_CongViec", new[] { "ID_DonVi" });
            DropIndex("dbo.NS_CongViec", new[] { "ID_LienHe" });
            DropIndex("dbo.NS_CongViec", new[] { "ID_KhachHang" });
            DropIndex("dbo.NS_CongViec", new[] { "ID_LoaiCongViec" });
            DropIndex("dbo.NS_NhanVien_SucKhoe", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_NhanVien_GiaDinh", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_NhanVien_DaoTao", new[] { "ID_NhanVien" });
            DropIndex("dbo.NS_NhanVien_CongTac", new[] { "ID_NhanVien" });
            DropIndex("dbo.DM_HangHoa_TonKho", new[] { "ID_LoHang" });
            DropIndex("dbo.DM_HangHoa_TonKho", new[] { "ID_DonVi" });
            DropIndex("dbo.DM_HangHoa_TonKho", new[] { "ID_DonViQuyDoi" });

            DropTable("dbo.NS_MienGiamThue");
            DropTable("dbo.NS_LoaiLuong");
            DropTable("dbo.NS_Luong_PhuCap");
            DropTable("dbo.NS_KhenThuong");
            DropTable("dbo.NS_HopDong");
            DropTable("dbo.NS_BaoHiem");
            DropTable("dbo.HT_ThongBao_CaiDat");
            DropTable("dbo.NS_ChamCong_ChiTiet");
            DropTable("dbo.HT_PhimTat");
            DropTable("dbo.DM_TichDiem");
            DropTable("dbo.DM_ThueSuat");
            DropTable("dbo.DM_NoiDungQuanTam");
            DropTable("dbo.DM_MayChamCong");
            DropTable("dbo.DM_MaVach");
            DropTable("dbo.ChotSo");
            DropTable("dbo.ChotSo_HangHoa");
            DropTable("dbo.ChotSo_KhachHang");
            DropTable("dbo.DM_LyDoHuyLichHen");
            DropTable("dbo.DM_LoaiPhieuThanhToan");
            DropTable("dbo.DM_LoaiNhapXuat");
            DropTable("dbo.DM_LoaiGiaPhong");
            DropTable("dbo.DM_KhoanPhuCap");
            DropTable("dbo.DM_HinhThucVanChuyen");
            DropTable("dbo.DM_HinhThucThanhToan");
            DropTable("dbo.DanhSachThi");
            DropTable("dbo.DanhSachThi_ChiTiet");
            DropTable("dbo.CongNoDauKi");
            DropTable("dbo.CongDoan_DichVu");
            DropTable("dbo.DM_LoaiPhong");
            DropTable("dbo.DM_KhuVuc");
            DropTable("dbo.DM_ViTri");
            DropTable("dbo.NS_LuongDoanhThu_ChiTiet");
            DropTable("dbo.NS_LuongDoanhThu");
            DropTable("dbo.NS_HoSoLuong");
            DropTable("dbo.DM_NguonKhachHang");
            DropTable("dbo.HT_NhatKySuDung");
            DropTable("dbo.HT_QuyenMacDinh");
            DropTable("dbo.HT_Quyen");
            DropTable("dbo.HT_Quyen_Nhom");
            DropTable("dbo.HT_NhomNguoiDung");
            DropTable("dbo.HT_NguoiDung_Nhom");
            DropTable("dbo.DM_LoaiTuVanLichHen");
            DropTable("dbo.HT_NguoiDung");
            DropTable("dbo.HT_MaChungTu");
            DropTable("dbo.HT_ThongBao");
            DropTable("dbo.DM_LopHoc");
            DropTable("dbo.NhomDoiTuong_DonVi");
            DropTable("dbo.HT_CauHinh_GioiHanTraHang");
            DropTable("dbo.HT_CauHinh_TichDiemChiTiet");
            DropTable("dbo.HT_CauHinh_TichDiemApDung");
            DropTable("dbo.HT_CauHinhPhanMem");
            DropTable("dbo.NhomHangHoa_DonVi");
            DropTable("dbo.DM_ThuocTinh");
            DropTable("dbo.HangHoa_ThuocTinh");
            DropTable("dbo.DM_VungMien");
            DropTable("dbo.DM_XaPhuong");
            DropTable("dbo.DM_QuanHuyen");
            DropTable("dbo.DM_TinhThanh");
            DropTable("dbo.Quy_TonQuyKhoiTao");
            DropTable("dbo.DM_GiaVon");
            DropTable("dbo.DM_LoHang");
            DropTable("dbo.Kho_TonKhoKhoiTao");
            DropTable("dbo.Kho_DonVi");
            DropTable("dbo.DM_Kho");
            DropTable("dbo.Kho_HoaDon_ChiTiet");
            DropTable("dbo.The_TheKhachHang_ChiTiet");
            DropTable("dbo.The_NhomThe");
            DropTable("dbo.NS_CongViec_PhanLoai");
            DropTable("dbo.NS_CongViec");
            DropTable("dbo.NS_QuaTrinhCongTac");
            DropTable("dbo.DM_ChucVu");
            DropTable("dbo.DM_LienHe_Anh");
            DropTable("dbo.DM_LienHe");
            DropTable("dbo.The_TheKhachHang");
            DropTable("dbo.Quy_KhoanThuChi");
            DropTable("dbo.HT_CongTy");
            DropTable("dbo.Quy_HoaDon_ChiTiet");
            DropTable("dbo.Quy_HoaDon");
            DropTable("dbo.DM_TaiKhoanNganHang");
            DropTable("dbo.DM_NganHang");
            DropTable("dbo.DM_MauIn");
            DropTable("dbo.DM_LoaiChungTu");
            DropTable("dbo.Kho_HoaDon");
            DropTable("dbo.DM_TyGia");
            DropTable("dbo.DM_TienTe");
            DropTable("dbo.DM_QuocGia");
            DropTable("dbo.DM_PhanLoaiHangHoaDichVu");
            DropTable("dbo.DM_HangHoa_Anh");
            DropTable("dbo.DM_HangHoa");
            DropTable("dbo.DM_NhomHangHoa");
            DropTable("dbo.DM_KhuyenMai_ChiTiet");
            DropTable("dbo.DM_KhuyenMai");
            DropTable("dbo.DM_KhuyenMai_ApDung");
            DropTable("dbo.DM_NhomDoiTuong_ChiTiet");
            DropTable("dbo.DM_DoiTuong_Nhom");
            DropTable("dbo.DM_NhomDoiTuong");
            DropTable("dbo.DM_GiaBan_ApDung");
            DropTable("dbo.DM_GiaBan");
            DropTable("dbo.DM_GiaBan_ChiTiet");
            DropTable("dbo.DinhLuongDichVu");
            DropTable("dbo.DonViQuiDoi");
            DropTable("dbo.ChietKhauMacDinh_NhanVien");
            DropTable("dbo.DM_DonVi");
            DropTable("dbo.DM_DoiTuong_Anh");
            DropTable("dbo.DM_DoiTuong");
            DropTable("dbo.ChamSocKhachHangs");
            DropTable("dbo.NS_NhanVien_SucKhoe");
            DropTable("dbo.NS_NhanVien_GiaDinh");
            DropTable("dbo.NS_NhanVien_DaoTao");
            DropTable("dbo.NS_NhanVien_CongTac");
            DropTable("dbo.NS_NhanVien_Anh");
            DropTable("dbo.NS_NhanVien");
            DropTable("dbo.NS_PhongBan");
            DropTable("dbo.BH_NhanVienThucHien");
            DropTable("dbo.BH_HoaDon_ChiTiet");
            DropTable("dbo.BH_HoaDon");
            DropTable("dbo.HeThong_SMS_TinMau");
            DropTable("dbo.HeThong_SMS_TaiKhoan");
            DropTable("dbo.HeThong_SMS");
            DropTable("dbo.DM_DoiTuong_TrangThai");
            DropTable("dbo.NS_KyTinhCong");
            DropTable("dbo.NS_KyHieuCong");
            DropTable("dbo.NS_PhieuPhanCa");
            DropTable("dbo.NS_PhieuPhanCa_ChiTiet");
            DropTable("dbo.NS_PhanCaChiTiet");
            DropTable("dbo.NS_CaLamViec");
            DropTable("dbo.NS_ChamCong");
            DropTable("dbo.NS_MaChamCong");
            DropTable("dbo.NS_DuLieuCongTho");
            DropTable("dbo.NS_MayChamCong");
            DropTable("dbo.DM_HangHoa_ViTri");
            DropTable("dbo.DM_ViTriHangHoa");
            DropTable("dbo.ChietKhauMacDinh_HoaDon");
            DropTable("dbo.ChietKhauMacDinh_HoaDon_ChiTiet");
            DropTable("dbo.ChietKhauDoanhThu_NhanVien");
            DropTable("dbo.ChietKhauDoanhThu_ChiTiet");
            DropTable("dbo.ChietKhauDoanhThu");
            DropTable("dbo.OptinForm_ThietLapThongBao");
            DropTable("dbo.OptinForm_TruongThongTin");
            DropTable("dbo.OptinForm_ThietLap");
            DropTable("dbo.OptinForm_NgayNghiLe");
            DropTable("dbo.OptinForm_NgayLamViec");
            DropTable("dbo.OptinForm");
            DropTable("dbo.OptinForm_Link");
            DropTable("dbo.OptinForm_DoiTuong");
            DropTable("dbo.ChamSocKhachHang_NhanVien");
            DropTable("dbo.DM_HangHoa_TonKho");
        }
    }
}
