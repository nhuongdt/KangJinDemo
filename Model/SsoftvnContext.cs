using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Model
{

    public partial class SsoftvnContext : DbContext
    {
        public SsoftvnContext()
            : base("name=SsoftvnContext")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<SsoftvnContext, Model.Migrations.Configuration>());
        }
        public SsoftvnContext(string connectionString)
            : base("name=" + connectionString)
        {
            
        }
        public virtual DbSet<BH_HoaDon> BH_HoaDon { get; set; }
        public virtual DbSet<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }
        public virtual DbSet<BH_NhanVienThucHien> BH_NhanVienThucHien { get; set; }
        public virtual DbSet<ChietKhauMacDinh_NhanVien> ChietKhauMacDinh_NhanVien { get; set; }
        public virtual DbSet<CongDoan_DichVu> CongDoan_DichVu { get; set; }
        public virtual DbSet<CongNoDauKi> CongNoDauKis { get; set; }
        public virtual DbSet<DanhSachThi> DanhSachThis { get; set; }
        public virtual DbSet<DanhSachThi_ChiTiet> DanhSachThi_ChiTiet { get; set; }
        public virtual DbSet<DinhLuongDichVu> DinhLuongDichVus { get; set; }
        public virtual DbSet<DM_ChucVu> DM_ChucVu { get; set; }
        public virtual DbSet<DM_DoiTuong> DM_DoiTuong { get; set; }
        public virtual DbSet<DM_DonVi> DM_DonVi { get; set; }
        public virtual DbSet<DM_GiaBan> DM_GiaBan { get; set; }
        public virtual DbSet<DM_GiaBan_ChiTiet> DM_GiaBan_ChiTiet { get; set; }
        public virtual DbSet<DM_GiaBan_ApDung> DM_GiaBan_ApDung { get; set; }
        public virtual DbSet<DM_HangHoa> DM_HangHoa { get; set; }
        public virtual DbSet<DM_HinhThucThanhToan> DM_HinhThucThanhToan { get; set; }
        public virtual DbSet<DM_HinhThucVanChuyen> DM_HinhThucVanChuyen { get; set; }
        public virtual DbSet<DM_Kho> DM_Kho { get; set; }
        public virtual DbSet<DM_KhoanPhuCap> DM_KhoanPhuCap { get; set; }
        public virtual DbSet<DM_KhuVuc> DM_KhuVuc { get; set; }
        public virtual DbSet<DM_LienHe> DM_LienHe { get; set; }
        public virtual DbSet<DM_LoaiChungTu> DM_LoaiChungTu { get; set; }
        public virtual DbSet<DM_LoaiGiaPhong> DM_LoaiGiaPhong { get; set; }
        public virtual DbSet<DM_LoaiNhapXuat> DM_LoaiNhapXuat { get; set; }
        public virtual DbSet<DM_LoaiPhieuThanhToan> DM_LoaiPhieuThanhToan { get; set; }
        public virtual DbSet<DM_LoaiPhong> DM_LoaiPhong { get; set; }
        public virtual DbSet<DM_LoHang> DM_LoHang { get; set; }
        public virtual DbSet<DM_LopHoc> DM_LopHoc { get; set; }
        public virtual DbSet<DM_LyDoHuyLichHen> DM_LyDoHuyLichHen { get; set; }
        public virtual DbSet<DM_MaVach> DM_MaVach { get; set; }
        public virtual DbSet<DM_MayChamCong> DM_MayChamCong { get; set; }
        public virtual DbSet<DM_NganHang> DM_NganHang { get; set; }
        public virtual DbSet<DM_NhomDoiTuong> DM_NhomDoiTuong { get; set; }
        public virtual DbSet<DM_NhomHangHoa> DM_NhomHangHoa { get; set; }
        public virtual DbSet<DM_NoiDungQuanTam> DM_NoiDungQuanTam { get; set; }
        public virtual DbSet<DM_PhanLoaiHangHoaDichVu> DM_PhanLoaiHangHoaDichVu { get; set; }
        public virtual DbSet<DM_QuanHuyen> DM_QuanHuyen { get; set; }
        public virtual DbSet<DM_QuocGia> DM_QuocGia { get; set; }
        public virtual DbSet<DM_ThueSuat> DM_ThueSuat { get; set; }
        public virtual DbSet<DM_ThuocTinh> DM_ThuocTinh { get; set; }
        public virtual DbSet<DM_TichDiem> DM_TichDiem { get; set; }
        public virtual DbSet<DM_TienTe> DM_TienTe { get; set; }
        public virtual DbSet<DM_TinhThanh> DM_TinhThanh { get; set; }
        public virtual DbSet<DM_TyGia> DM_TyGia { get; set; }
        public virtual DbSet<DM_ViTri> DM_ViTri { get; set; }
        public virtual DbSet<DM_VungMien> DM_VungMien { get; set; }
        public virtual DbSet<DonViQuiDoi> DonViQuiDois { get; set; }
        public virtual DbSet<HangHoa_ThuocTinh> HangHoa_ThuocTinh { get; set; }
        public virtual DbSet<HT_CauHinhPhanMem> HT_CauHinhPhanMem { get; set; }
        public virtual DbSet<HT_CongTy> HT_CongTy { get; set; }
        public virtual DbSet<HT_MaChungTu> HT_MaChungTu { get; set; }
        public virtual DbSet<HT_NguoiDung> HT_NguoiDung { get; set; }
        public virtual DbSet<HT_NguoiDung_Nhom> HT_NguoiDung_Nhom { get; set; }
        public virtual DbSet<HT_NhomNguoiDung> HT_NhomNguoiDung { get; set; }
        public virtual DbSet<HT_PhimTat> HT_PhimTat { get; set; }
        public virtual DbSet<HT_Quyen> HT_Quyen { get; set; }
        public virtual DbSet<HT_Quyen_Nhom> HT_Quyen_Nhom { get; set; }
        public virtual DbSet<HT_QuyenMacDinh> HT_QuyenMacDinh { get; set; }
        public virtual DbSet<Kho_DonVi> Kho_DonVi { get; set; }
        public virtual DbSet<Kho_HoaDon> Kho_HoaDon { get; set; }
        public virtual DbSet<Kho_HoaDon_ChiTiet> Kho_HoaDon_ChiTiet { get; set; }
        public virtual DbSet<Kho_TonKhoKhoiTao> Kho_TonKhoKhoiTao { get; set; }
        public virtual DbSet<NhomDoiTuong_DonVi> NhomDoiTuong_DonVi { get; set; }
        public virtual DbSet<NhomHangHoa_DonVi> NhomHangHoa_DonVi { get; set; }
        public virtual DbSet<NS_HoSoLuong> NS_HoSoLuong { get; set; }
        public virtual DbSet<NS_LuongDoanhThu> NS_LuongDoanhThu { get; set; }
        public virtual DbSet<NS_LuongDoanhThu_ChiTiet> NS_LuongDoanhThu_ChiTiet { get; set; }
        public virtual DbSet<NS_NhanVien> NS_NhanVien { get; set; }
        public virtual DbSet<NS_QuaTrinhCongTac> NS_QuaTrinhCongTac { get; set; }
        public virtual DbSet<Quy_HoaDon> Quy_HoaDon { get; set; }
        public virtual DbSet<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }
        public virtual DbSet<Quy_KhoanThuChi> Quy_KhoanThuChi { get; set; }
        public virtual DbSet<Quy_TonQuyKhoiTao> Quy_TonQuyKhoiTao { get; set; }
        public virtual DbSet<The_NhomThe> The_NhomThe { get; set; }
        public virtual DbSet<The_TheKhachHang> The_TheKhachHang { get; set; }
        public virtual DbSet<The_TheKhachHang_ChiTiet> The_TheKhachHang_ChiTiet { get; set; }
        public virtual DbSet<DM_LoaiTuVanLichHen> DM_LoaiTuVanLichHen { get; set; }
        public virtual DbSet<ChamSocKhachHang> ChamSocKhachHang { get; set; }
        public virtual DbSet<DM_NguonKhachHang> DM_NguonKhachHang { get; set; }
        public virtual DbSet<DM_KhuyenMai> DM_KhuyenMai { get; set; }
        public virtual DbSet<DM_KhuyenMai_ApDung> DM_KhuyenMai_ApDung { get; set; }
        public virtual DbSet<DM_KhuyenMai_ChiTiet> DM_KhuyenMai_ChiTiet { get; set; }
        public virtual DbSet<DM_HangHoa_Anh> DM_HangHoa_Anh { get; set; }
        public virtual DbSet<HT_CauHinh_TichDiemChiTiet> HT_CauHinh_TichDiemChiTiet { get; set; }
        public virtual DbSet<HT_CauHinh_TichDiemApDung> HT_CauHinh_TichDiemApDung { get; set; }
        public virtual DbSet<HT_NhatKySuDung> HT_NhatKySuDung { get; set; }
        public virtual DbSet<ChotSo> ChotSo { get; set; }
        public virtual DbSet<ChotSo_HangHoa> ChotSo_HangHoa { get; set; }
        public virtual DbSet<ChotSo_KhachHang> ChotSo_KhachHang { get; set; }
        public virtual DbSet<DM_DoiTuong_Anh> DM_DoiTuong_Anh { get; set; }
        public virtual DbSet<DM_NhomDoiTuong_ChiTiet> DM_NhomDoiTuong_ChiTiet { get; set; }
        public virtual DbSet<DM_DoiTuong_Nhom> DM_DoiTuong_Nhom { get; set; }
        public virtual DbSet<DM_MauIn> DM_MauIn { get; set; }
        public virtual DbSet<DM_TaiKhoanNganHang> DM_TaiKhoanNganHang { get; set; }
        public virtual DbSet<HT_ThongBao> HT_ThongBao { get; set; }
        public virtual DbSet<HT_ThongBao_CaiDat> HT_ThongBao_CaiDat { get; set; }
        public virtual DbSet<DM_GiaVon> DM_GiaVon { get; set; }
        public virtual DbSet<HT_CauHinh_GioiHanTraHang> HT_CauHinh_GioiHanTraHang { get; set; }
        public virtual DbSet<NS_NhanVien_Anh> NS_NhanVien_Anh { get; set; }
        public virtual DbSet<DM_LienHe_Anh> DM_LienHe_Anh { get; set; }
        public virtual DbSet<DM_XaPhuong> DM_XaPhuong { get; set; }
        public virtual DbSet<NS_PhongBan> NS_PhongBan { get; set; }
        public virtual DbSet<NS_CongViec_PhanLoai> NS_CongViec_PhanLoai { get; set; }
        public virtual DbSet<NS_CongViec> NS_CongViec { get; set; }
        public virtual DbSet<NS_NhanVien_CongTac> NS_NhanVien_CongTac { get; set; }
        public virtual DbSet<NS_NhanVien_DaoTao> NS_NhanVien_DaoTao { get; set; }
        public virtual DbSet<NS_NhanVien_GiaDinh> NS_NhanVien_GiaDinh { get; set; }
        public virtual DbSet<NS_NhanVien_SucKhoe> NS_NhanVien_SucKhoe { get; set; }
        public virtual DbSet<NS_LoaiLuong> NS_LoaiLuong { get; set; }
        public virtual DbSet<NS_Luong_PhuCap> NS_Luong_PhuCap { get; set; }
        public virtual DbSet<NS_BaoHiem> NS_BaoHiem { get; set; }
        public virtual DbSet<NS_HopDong> NS_HopDong { get; set; }
        public virtual DbSet<NS_KhenThuong> NS_KhenThuong { get; set; }
        public virtual DbSet<NS_MienGiamThue> NS_MienGiamThue { get; set; }
        public virtual DbSet<HeThong_SMS> HeThong_SMS { get; set; }
        public virtual DbSet<HeThong_SMS_TaiKhoan> HeThong_SMS_TaiKhoan { get; set; }
        public virtual DbSet<HeThong_SMS_TinMau> HeThong_SMS_TinMau { get; set; }
        public virtual DbSet<DM_DoiTuong_TrangThai> DM_DoiTuong_TrangThai { get; set; }
        public virtual DbSet<DM_HangHoa_ViTri> DM_HangHoa_ViTri { get; set; }
        public virtual DbSet<DM_ViTriHangHoa> DM_ViTriHangHoa { get; set; }
        public virtual DbSet<NS_MayChamCong> NS_MayChamCong { get; set; }
        public virtual DbSet<NS_DuLieuCongTho> NS_DuLieuCongTho { get; set; }
        public virtual DbSet<NS_MaChamCong> NS_MaChamCong { get; set; }
        public virtual DbSet<NS_PhieuPhanCa> NS_PhieuPhanCa { get; set; }
        public virtual DbSet<NS_CaLamViec> NS_CaLamViec { get; set; }
        public virtual DbSet<NS_KyHieuCong> NS_KyHieuCong { get; set; }
        public virtual DbSet<NS_KyTinhCong> NS_KyTinhCong { get; set; }
        public virtual DbSet<ChietKhauMacDinh_HoaDon> ChietKhauMacDinh_HoaDon { get; set; }
        public virtual DbSet<ChietKhauMacDinh_HoaDon_ChiTiet> ChietKhauMacDinh_HoaDon_ChiTiet { get; set; }
        public virtual DbSet<ChietKhauDoanhThu> ChietKhauDoanhThu { get; set; }
        public virtual DbSet<ChietKhauDoanhThu_ChiTiet> ChietKhauDoanhThu_ChiTiet { get; set; }
        public virtual DbSet<ChietKhauDoanhThu_NhanVien> ChietKhauDoanhThu_NhanVien { get; set; }
        public virtual DbSet<OptinForm_TruongThongTin> OptinForm_TruongThongTin { get; set; }
        public virtual DbSet<OptinForm_ThietLap> OptinForm_ThietLap { get; set; }
        public virtual DbSet<OptinForm> OptinForm { get; set; }
        public virtual DbSet<OptinForm_ThietLapThongBao> OptinForm_ThietLapThongBao { get; set; }
        public virtual DbSet<OptinForm_NgayNghiLe> OptinForm_NgayNghiLe { get; set; }
        public virtual DbSet<OptinForm_NgayLamViec> OptinForm_NgayLamViec { get; set; }
        public virtual DbSet<OptinForm_Link> OptinForm_Link { get; set; }
        public virtual DbSet<OptinForm_DoiTuong> OptinForm_DoiTuong { get; set; }
        public virtual DbSet<ChamSocKhachHang_NhanVien> ChamSocKhachHang_NhanVien { get; set; }
        public virtual DbSet<DM_HangHoa_TonKho> DM_HangHoa_TonKho { get; set; }
        public virtual DbSet<NS_CaLamViec_DonVi> NS_CaLamViec_DonVi { get; set; }
        public virtual DbSet<NS_PhieuPhanCa_CaLamViec> NS_PhieuPhanCa_CaLamViec { get; set; }
        public virtual DbSet<NS_PhieuPhanCa_NhanVien> NS_PhieuPhanCa_NhanVien { get; set; }
        public virtual DbSet<NS_NgayNghiLe> NS_NgayNghiLe { get; set; }
        public virtual DbSet<NS_CongBoSung> NS_CongBoSung { get; set; }
        public virtual DbSet<NS_BangLuong> NS_BangLuong { get; set; }
        public virtual DbSet<NS_BangLuong_ChiTiet> NS_BangLuong_ChiTiet { get; set; }
        public virtual DbSet<NS_LoaiBaoHiem> NS_LoaiBaoHiem { get; set; }
        public virtual DbSet<NS_LoaiKhenThuong> NS_LoaiKhenThuong { get; set; }
        public virtual DbSet<DM_DoiTuong_CongNo> DM_DoiTuong_CongNo { get; set; }
        public virtual DbSet<NS_ThietLapLuongChiTiet> NS_ThietLapLuongChiTiet { get; set; }
        public virtual DbSet<NS_BangLuongOTChiTiet> NS_BangLuongOTChiTiet { get; set; }
        public virtual DbSet<NS_CongNoTamUngLuong> NS_CongNoTamUngLuong { get; set; }
        public virtual DbSet<Gara_HangXe> Gara_HangXe { get; set; }
        public virtual DbSet<Gara_LoaiXe> Gara_LoaiXe { get; set; }
        public virtual DbSet<Gara_MauXe> Gara_MauXe { get; set; }
        public virtual DbSet<Gara_DanhMucXe> Gara_DanhMucXe { get; set; }
        public virtual DbSet<Gara_PhieuTiepNhan> Gara_PhieuTiepNhan { get; set; }
        public virtual DbSet<Gara_HangMucSuaChua> Gara_HangMucSuaChua { get; set; }
        public virtual DbSet<Gara_GiayToKemTheo> Gara_GiayToKemTheo { get; set; }
        public virtual DbSet<HT_NhatKySuDung_Backup> HT_NhatKySuDung_Backup { get; set; }
        public virtual DbSet<DM_HangHoa_BaoDuongChiTiet> DM_HangHoa_BaoDuongChiTiet { get; set; }
        public virtual DbSet<Gara_LichBaoDuong> Gara_LichBaoDuong { get; set; }
        public virtual DbSet<HT_ThongBao_CatDatThoiGian> HT_ThongBao_CatDatThoiGian { get; set; }
        public virtual DbSet<BH_HoaDon_ChiPhi> BH_HoaDon_ChiPhi { get; set; }
        public virtual DbSet<CSKH_DatLich> CSKH_DatLich { get; set; }
        public virtual DbSet<CSKH_DatLich_HangHoa> CSKH_DatLich_HangHoa { get; set; }
        public virtual DbSet<CSKH_DatLich_NhanVien> CSKH_DatLich_NhanVien { get; set; }
        public virtual DbSet<BH_HoaDon_Anh> BH_HoaDon_Anh { get; set; }
        public virtual DbSet<Gara_Xe_PhieuBanGiao> Gara_Xe_PhieuBanGiao { get; set; }
        public virtual DbSet<Gara_Xe_NhatKyHoatDong> Gara_Xe_NhatKyHoatDong { get; set; }
        public virtual DbSet<NhomHang_ChiTietSanPhamHoTro> NhomHang_ChiTietSanPhamHoTro { get; set; }
        public virtual DbSet<NhomHang_KhoangApDung> NhomHang_KhoangApDung { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SsoftvnContext, Migrations.Configuration>());

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.BH_HoaDon_ChiTiet)
                .WithRequired(e => e.BH_HoaDon)
                .HasForeignKey(e => e.ID_HoaDon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.Kho_HoaDon)
                .WithOptional(e => e.BH_HoaDon)
                .HasForeignKey(e => e.ID_ChungTuLienQuan);

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.BH_HoaDon)
                .HasForeignKey(e => e.ID_HoaDonLienQuan);

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.BH_NhanVienThucHien)
                .WithOptional(e => e.BH_HoaDon)
                .HasForeignKey(e => e.ID_HoaDon);

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.HeThong_SMS)
                .WithOptional(e => e.BH_HoaDon)
                .HasForeignKey(e => e.ID_HoaDon);

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.Gara_LichBaoDuong)
                .WithRequired(e => e.BH_HoaDon)
                .HasForeignKey(e => e.ID_HoaDon);

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.BH_HoaDon_ChiPhi)
                .WithRequired(e => e.BH_HoaDon)
                .HasForeignKey(e => e.ID_HoaDon);

            modelBuilder.Entity<BH_HoaDon>()
                .HasMany(e => e.BH_HoaDon_Anh)
                .WithRequired(e => e.BH_HoaDon)
                .HasForeignKey(e => e.IdHoaDon);

            modelBuilder.Entity<BH_HoaDon_ChiTiet>()
                .HasMany(e => e.Kho_HoaDon_ChiTiet)
                .WithOptional(e => e.BH_HoaDon_ChiTiet)
                .HasForeignKey(e => e.ID_CTChungTuLienQuan);

            modelBuilder.Entity<BH_HoaDon_ChiTiet>()
                .HasMany(e => e.BH_NhanVienThucHien)
                .WithOptional(e => e.BH_HoaDonChiTiet)
                .HasForeignKey(e => e.ID_ChiTietHoaDon);

            modelBuilder.Entity<BH_HoaDon_ChiTiet>()
                .HasMany(e => e.BH_HoaDon_ChiPhi)
                .WithRequired(e => e.BH_HoaDon_ChiTiet)
                .HasForeignKey(e => e.ID_HoaDon_ChiTiet);

            modelBuilder.Entity<DM_ChucVu>()
                .HasMany(e => e.DM_LienHe)
                .WithOptional(e => e.DM_ChucVu)
                .HasForeignKey(e => e.ID_ChucVu);

            modelBuilder.Entity<DM_ChucVu>()
                .HasMany(e => e.NS_QuaTrinhCongTac)
                .WithOptional(e => e.DM_ChucVu)
                .HasForeignKey(e => e.ID_ChucVu);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.DM_DoiTuong1)
                .WithOptional(e => e.DM_DoiTuong2)
                .HasForeignKey(e => e.ID_NguoiGioiThieu);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.DM_HangHoa)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.DM_LienHe)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.Kho_HoaDon)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.The_TheKhachHang)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_DoiTuong>().
                HasMany(e => e.ChamSocKhachHang)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_KhachHang);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.ChotSo_KhachHang)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_KhachHang);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.DM_DoiTuong_Anh)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.DM_DoiTuong_Nhom)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.NS_CongViec)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_KhachHang);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.HeThong_SMS)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_KhachHang);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.DM_DoiTuong_CongNo)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.Gara_DanhMucXe)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_KhachHang);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.Gara_PhieuTiepNhan)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_KhachHang);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.Gara_PhieuTiepNhanBaoHiem)
                .WithOptional(e => e.DM_DoiTuongBaoHiem)
                .HasForeignKey(e => e.ID_BaoHiem);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.BH_HoaDonBaoHiem)
                .WithOptional(e => e.DM_DoiTuongBaoHiem)
                .HasForeignKey(e => e.ID_BaoHiem);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.BH_HoaDon_ChiPhi)
                .WithRequired(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.ID_NhaCungCap);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.CSKH_DatLich)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.IDDoiTuong);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.Gara_Xe_PhieuBanGiao)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.IdKhachHang);

            modelBuilder.Entity<DM_DoiTuong>()
                .HasMany(e => e.Gara_Xe_NhatKyHoatDong)
                .WithOptional(e => e.DM_DoiTuong)
                .HasForeignKey(e => e.IdKhachHang);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.BH_HoaDon)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_DonVi1)
                .WithOptional(e => e.DM_DonVi2)
                .HasForeignKey(e => e.ID_Parent);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_LopHoc)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.HT_NguoiDung)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.Kho_DonVi)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.Kho_HoaDon)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.Kho_TonKhoKhoiTao)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NhomDoiTuong_DonVi)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NhomHangHoa_DonVi)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.Quy_HoaDon)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.Quy_TonQuyKhoiTao)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.The_TheKhachHang)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_GiaBan_ApDung)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_QuaTrinhCongTac)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.ChietKhauMacDinh_NhanVien)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.HT_CauHinhPhanMem)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_KhuyenMai_ApDung)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.ChamSocKhachHang)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.HT_NhatKySuDung)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.ChotSo)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.ChotSo_HangHoa)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.ChotSo_KhachHang)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_MauIn)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_TaiKhoanNganHang)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.HT_ThongBao)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.HT_NguoiDung_Nhom)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_GiaVon)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_CongViec)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_PhongBan)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.HeThong_SMS)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_MayChamCong)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_ChiNhanh);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.ChietKhauMacDinh_HoaDon)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.ChietKhauDoanhThu)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_HangHoa_TonKho)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_CaLamViec_DonVi)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_PhieuPhanCa)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.DM_DoiTuong_CongNo)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_Luong_PhuCap)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_BangLuong)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_KyHieuCong)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_CongBoSung)
                .WithOptional(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.NS_CongNoTamUngLuong)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.Gara_PhieuTiepNhan)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.ID_DonVi);

            modelBuilder.Entity<DM_DonVi>()
                .HasMany(e => e.CSKH_DatLich)
                .WithRequired(e => e.DM_DonVi)
                .HasForeignKey(e => e.IDDonVi);

            modelBuilder.Entity<DM_GiaBan>()
                .HasMany(e => e.DM_GiaBan_ChiTiet)
                .WithRequired(e => e.DM_GiaBan)
                .HasForeignKey(e => e.ID_GiaBan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_GiaBan>()
                .HasMany(e => e.DM_GiaBan_ApDung)
                .WithRequired(e => e.DM_GiaBan)
                .HasForeignKey(e => e.ID_GiaBan);

            modelBuilder.Entity<DM_GiaBan>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.DM_GiaBan)
                .HasForeignKey(e => e.ID_BangGia);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.DonViQuiDois)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.DM_HangHoa_Anh)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.HangHoa_ThuocTinh)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.ChotSo_HangHoa)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.DM_ViTriHangHoa)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.ChamSocKhachHang)
                .WithOptional(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.DM_HangHoa_BaoDuongChiTiet)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.Gara_LichBaoDuong)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.ID_HangHoa);

            modelBuilder.Entity<DM_HangHoa>()
                .HasMany(e => e.CSKH_DatLich_HangHoa)
                .WithRequired(e => e.DM_HangHoa)
                .HasForeignKey(e => e.IDHangHoa);

            modelBuilder.Entity<Gara_DanhMucXe>()
                .HasMany(e => e.DM_HangHoa)
                .WithOptional(e => e.Gara_DanhMucXe)
                .HasForeignKey(e => e.ID_Xe);

            modelBuilder.Entity<Gara_DanhMucXe>()
                .HasMany(e => e.Gara_LichBaoDuong)
                .WithOptional(e => e.Gara_DanhMucXe)
                .HasForeignKey(e => e.ID_Xe);

            modelBuilder.Entity<Gara_DanhMucXe>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.Gara_DanhMucXe)
                .HasForeignKey(e => e.ID_Xe);

            modelBuilder.Entity<Gara_DanhMucXe>()
                .HasMany(e => e.CSKH_DatLich)
                .WithOptional(e => e.Gara_DanhMucXe)
                .HasForeignKey(e => e.IDXe);

            modelBuilder.Entity<Gara_DanhMucXe>()
                .HasMany(e => e.Gara_Xe_PhieuBanGiao)
                .WithRequired(e => e.Gara_DanhMucXe)
                .HasForeignKey(e => e.IdXe);

            modelBuilder.Entity<DM_Kho>()
                .HasMany(e => e.BH_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_Kho)
                .HasForeignKey(e => e.ID_KhoHang);

            modelBuilder.Entity<DM_Kho>()
                .HasMany(e => e.DM_GiaBan_ChiTiet)
                .WithRequired(e => e.DM_Kho)
                .HasForeignKey(e => e.ID_KhoHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_Kho>()
                .HasMany(e => e.Kho_DonVi)
                .WithRequired(e => e.DM_Kho)
                .HasForeignKey(e => e.ID_Kho)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_Kho>()
                .HasMany(e => e.Kho_HoaDon_ChiTiet)
                .WithRequired(e => e.DM_Kho)
                .HasForeignKey(e => e.ID_Kho)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_Kho>()
                .HasMany(e => e.Kho_TonKhoKhoiTao)
                .WithRequired(e => e.DM_Kho)
                .HasForeignKey(e => e.ID_Kho)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_KhuVuc>()
                .HasMany(e => e.DM_ViTri)
                .WithRequired(e => e.DM_KhuVuc)
                .HasForeignKey(e => e.ID_KhuVuc)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_LienHe>()
                .HasMany(e => e.The_TheKhachHang)
                .WithOptional(e => e.DM_LienHe)
                .HasForeignKey(e => e.ID_LienHe);

            modelBuilder.Entity<DM_LienHe>()
                .HasMany(e => e.DM_LienHe_Anh)
                .WithRequired(e => e.DM_LienHe)
                .HasForeignKey(e => e.ID_LienHe);

            modelBuilder.Entity<DM_LienHe>()
                .HasMany(e => e.NS_CongViec)
                .WithOptional(e => e.DM_LienHe)
                .HasForeignKey(e => e.ID_LienHe);

            modelBuilder.Entity<DM_LienHe>()
                .HasMany(e => e.ChamSocKhachHang)
                .WithOptional(e => e.DM_LienHe)
                .HasForeignKey(e => e.ID_LienHe);

            modelBuilder.Entity<DM_LoaiChungTu>()
                .HasMany(e => e.Kho_HoaDon)
                .WithRequired(e => e.DM_LoaiChungTu)
                .HasForeignKey(e => e.LoaiChungTu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_LoaiChungTu>()
                .HasMany(e => e.Quy_HoaDon)
                .WithOptional(e => e.DM_LoaiChungTu)
                .HasForeignKey(e => e.LoaiHoaDon);

            modelBuilder.Entity<DM_LoaiChungTu>()
                .HasMany(e => e.BH_HoaDon)
                .WithRequired(e => e.DM_LoaiChungTu)
                .HasForeignKey(e => e.LoaiHoaDon);

            modelBuilder.Entity<DM_LoaiChungTu>()
                .HasMany(e => e.DM_MauIn)
                .WithRequired(e => e.DM_LoaiChungTu)
                .HasForeignKey(e => e.ID_LoaiChungTu);

            modelBuilder.Entity<DM_LoaiChungTu>()
                .HasMany(e => e.HT_MaChungTu)
                .WithRequired(e => e.DM_LoaiChungTu)
                .HasForeignKey(e => e.ID_LoaiChungTu);

            modelBuilder.Entity<DM_LoaiPhong>()
                .HasMany(e => e.DM_ViTri)
                .WithOptional(e => e.DM_LoaiPhong)
                .HasForeignKey(e => e.ID_LoaiPhong);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.BH_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.ID_LoHang);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.Kho_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.ID_LoHang);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.Kho_TonKhoKhoiTao)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.ID_LoHang);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.ChotSo_HangHoa)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.ID_LoHang);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.DM_GiaVon)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.ID_LoHang);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.DM_HangHoa_TonKho)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.ID_LoHang);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.DinhLuongDichVu)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.ID_LoHang);

            modelBuilder.Entity<DM_LoHang>()
                .HasMany(e => e.NhomHang_ChiTietSanPhamHoTro)
                .WithOptional(e => e.DM_LoHang)
                .HasForeignKey(e => e.Id_LoHang);

            modelBuilder.Entity<DM_MayChamCong>()
                .Property(e => e.IP)
                .IsFixedLength();

            modelBuilder.Entity<DM_NganHang>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_NganHang)
                .HasForeignKey(e => e.ID_NganHang);

            modelBuilder.Entity<DM_NganHang>()
                .HasMany(e => e.DM_DonVi)
                .WithOptional(e => e.DM_NganHang)
                .HasForeignKey(e => e.ID_NganHang);

            modelBuilder.Entity<DM_NganHang>()
                .HasMany(e => e.HT_CongTy)
                .WithOptional(e => e.DM_NganHang)
                .HasForeignKey(e => e.ID_NganHang);

            modelBuilder.Entity<DM_NganHang>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_NganHang)
                .HasForeignKey(e => e.ID_NganHang);

            modelBuilder.Entity<DM_NganHang>()
                .HasMany(e => e.DM_TaiKhoanNganHang)
                .WithRequired(e => e.DM_NganHang)
                .HasForeignKey(e => e.ID_NganHang);

            modelBuilder.Entity<DM_TaiKhoanNganHang>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_TaiKhoanNganHang)
                .HasForeignKey(e => e.ID_TaiKhoanNganHang);

            modelBuilder.Entity<DM_NguonKhachHang>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_NguonKhachHang)
                .HasForeignKey(e => e.ID_NguonKhach);

            modelBuilder.Entity<DM_NhomDoiTuong>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_NhomDoiTuong)
                .HasForeignKey(e => e.ID_NhomDoiTuong);

            modelBuilder.Entity<DM_NhomDoiTuong>()
                .HasMany(e => e.NhomDoiTuong_DonVi)
                .WithRequired(e => e.DM_NhomDoiTuong)
                .HasForeignKey(e => e.ID_NhomDoiTuong)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_NhomDoiTuong>()
                .HasMany(e => e.DM_GiaBan_ApDung)
                .WithOptional(e => e.DM_NhomDoiTuong)
                .HasForeignKey(e => e.ID_NhomKhachHang);

            modelBuilder.Entity<DM_NhomDoiTuong>()
                .HasMany(e => e.DM_KhuyenMai_ApDung)
                .WithOptional(e => e.DM_NhomDoiTuong)
                .HasForeignKey(e => e.ID_NhomKhachHang);

            modelBuilder.Entity<DM_NhomDoiTuong>()
                .HasMany(e => e.HT_CauHinh_TichDiemApDung)
                .WithRequired(e => e.DM_NhomDoiTuong)
                .HasForeignKey(e => e.ID_NhomDoiTuong);

            modelBuilder.Entity<DM_NhomDoiTuong>()
                .HasMany(e => e.DM_DoiTuong_Nhom)
                .WithRequired(e => e.DM_NhomDoiTuong)
                .HasForeignKey(e => e.ID_NhomDoiTuong);

            modelBuilder.Entity<DM_NhomDoiTuong>()
                .HasMany(e => e.DM_NhomDoiTuong_ChiTiet)
                .WithRequired(e => e.DM_NhomDoiTuong)
                .HasForeignKey(e => e.ID_NhomDoiTuong);

            modelBuilder.Entity<DM_NhomHangHoa>()
                .HasMany(e => e.DM_HangHoa)
                .WithOptional(e => e.DM_NhomHangHoa)
                .HasForeignKey(e => e.ID_NhomHang);

            modelBuilder.Entity<DM_NhomHangHoa>()
                .HasMany(e => e.DM_NhomHangHoa1)
                .WithOptional(e => e.DM_NhomHangHoa2)
                .HasForeignKey(e => e.ID_Parent);

            modelBuilder.Entity<DM_NhomHangHoa>()
                .HasMany(e => e.NhomHangHoa_DonVi)
                .WithRequired(e => e.DM_NhomHangHoa)
                .HasForeignKey(e => e.ID_NhomHangHoa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_NhomHangHoa>()
                .HasMany(e => e.DM_KhuyenMai_ChiTiet)
                .WithOptional(e => e.DM_NhomHangHoa)
                .HasForeignKey(e => e.ID_NhomHangHoa);

            modelBuilder.Entity<DM_NhomHangHoa>()
                .HasMany(e => e.DM_KhuyenMai_ChiTiet1)
                .WithOptional(e => e.DM_NhomHangHoa1)
                .HasForeignKey(e => e.ID_NhomHangHoaMua);

            modelBuilder.Entity<DM_NhomHangHoa>()
                .HasMany(e => e.NhomHang_ChiTietSanPhamHoTro)
                .WithRequired(e => e.DM_NhomHangHoa)
                .HasForeignKey(e => e.Id_NhomHang);

            modelBuilder.Entity<DM_NhomHangHoa>()
                .HasMany(e => e.NhomHang_KhoangApDung)
                .WithRequired(e => e.DM_NhomHangHoa)
                .HasForeignKey(e => e.Id_NhomHang);

            modelBuilder.Entity<DM_PhanLoaiHangHoaDichVu>()
                .HasMany(e => e.DM_HangHoa)
                .WithOptional(e => e.DM_PhanLoaiHangHoaDichVu)
                .HasForeignKey(e => e.ID_PhanLoai);

            modelBuilder.Entity<DM_QuanHuyen>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_QuanHuyen)
                .HasForeignKey(e => e.ID_QuanHuyen);

            modelBuilder.Entity<DM_QuanHuyen>()
                .HasMany(e => e.DM_XaPhuong)
                .WithRequired(e => e.DM_QuanHuyen)
                .HasForeignKey(e => e.ID_QuanHuyen);

            modelBuilder.Entity<DM_QuanHuyen>()
                .HasMany(e => e.NS_NhanVienHKTT)
                .WithOptional(e => e.DM_QuanHuyenHKTT)
                .HasForeignKey(e => e.ID_QuanHuyenHKTT);

            modelBuilder.Entity<DM_QuanHuyen>()
                .HasMany(e => e.NS_NhanVienTT)
                .WithOptional(e => e.DM_QuanHuyenTT)
                .HasForeignKey(e => e.ID_QuanHuyenTT);

            modelBuilder.Entity<DM_QuanHuyen>()
                .HasMany(e => e.DM_LienHe)
                .WithOptional(e => e.DM_QuanHuyen)
                .HasForeignKey(e => e.ID_QuanHuyen);

            modelBuilder.Entity<DM_QuanHuyen>()
                .HasMany(e => e.OptinForm_DoiTuong)
                .WithOptional(e => e.DM_QuanHuyen)
                .HasForeignKey(e => e.ID_QuanHuyen);

            modelBuilder.Entity<DM_XaPhuong>()
                .HasMany(e => e.NS_NhanVienHKTT)
                .WithOptional(e => e.DM_XaPhuongHKTT)
                .HasForeignKey(e => e.ID_XaPhuongHKTT);

            modelBuilder.Entity<DM_XaPhuong>()
                .HasMany(e => e.NS_NhanVienTT)
                .WithOptional(e => e.DM_XaPhuongTT)
                .HasForeignKey(e => e.ID_XaPhuongTT);

            modelBuilder.Entity<DM_QuocGia>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_QuocGia)
                .HasForeignKey(e => e.ID_QuocGia);

            modelBuilder.Entity<DM_QuocGia>()
                .HasMany(e => e.DM_HangHoa)
                .WithOptional(e => e.DM_QuocGia)
                .HasForeignKey(e => e.ID_QuocGia);

            modelBuilder.Entity<DM_QuocGia>()
                .HasMany(e => e.DM_TienTe)
                .WithRequired(e => e.DM_QuocGia)
                .HasForeignKey(e => e.ID_QuocGia)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_QuocGia>()
                .HasMany(e => e.DM_TinhThanh)
                .WithOptional(e => e.DM_QuocGia)
                .HasForeignKey(e => e.ID_QuocGia);

            modelBuilder.Entity<DM_QuocGia>()
                .HasMany(e => e.NS_NhanVien)
                .WithOptional(e => e.DM_QuocGia)
                .HasForeignKey(e => e.ID_QuocGia);

            modelBuilder.Entity<DM_ThuocTinh>()
                .HasMany(e => e.HangHoa_ThuocTinh)
                .WithRequired(e => e.DM_ThuocTinh)
                .HasForeignKey(e => e.ID_ThuocTinh)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_TienTe>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.DM_TienTe)
                .HasForeignKey(e => e.ID_NgoaiTe);

            modelBuilder.Entity<DM_TienTe>()
                .HasMany(e => e.DM_GiaBan_ChiTiet)
                .WithOptional(e => e.DM_TienTe)
                .HasForeignKey(e => e.ID_NgoaiTe);

            modelBuilder.Entity<DM_TienTe>()
                .HasMany(e => e.DM_TyGia)
                .WithRequired(e => e.DM_TienTe)
                .HasForeignKey(e => e.ID_TienTe)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_TienTe>()
                .HasMany(e => e.Kho_HoaDon)
                .WithOptional(e => e.DM_TienTe)
                .HasForeignKey(e => e.ID_NgoaiTe);

            modelBuilder.Entity<DM_TienTe>()
                .HasMany(e => e.Quy_HoaDon)
                .WithOptional(e => e.DM_TienTe)
                .HasForeignKey(e => e.ID_NgoaiTe);

            modelBuilder.Entity<DM_TienTe>()
                .HasMany(e => e.Quy_TonQuyKhoiTao)
                .WithRequired(e => e.DM_TienTe)
                .HasForeignKey(e => e.ID_TienTe)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_TienTe>()
                .HasMany(e => e.The_TheKhachHang)
                .WithOptional(e => e.DM_TienTe)
                .HasForeignKey(e => e.ID_TienTe);

            modelBuilder.Entity<DM_TinhThanh>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_TinhThanh)
                .HasForeignKey(e => e.ID_TinhThanh);

            modelBuilder.Entity<DM_TinhThanh>()
                .HasMany(e => e.DM_QuanHuyen)
                .WithRequired(e => e.DM_TinhThanh)
                .HasForeignKey(e => e.ID_TinhThanh)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_TinhThanh>()
                .HasMany(e => e.DM_NhomDoiTuong_ChiTiet)
                .WithOptional(e => e.DM_TinhThanh)
                .HasForeignKey(e => e.GiaTriKhuVuc);

            modelBuilder.Entity<DM_TinhThanh>()
                .HasMany(e => e.NS_NhanVienHKTT)
                .WithOptional(e => e.DM_TinhThanhHKTT)
                .HasForeignKey(e => e.ID_TinhThanhHKTT);

            modelBuilder.Entity<DM_TinhThanh>()
                .HasMany(e => e.NS_NhanVienTT)
                .WithOptional(e => e.DM_TinhThanhTT)
                .HasForeignKey(e => e.ID_TinhThanhTT);

            modelBuilder.Entity<DM_TinhThanh>()
                .HasMany(e => e.DM_LienHe)
                .WithOptional(e => e.DM_TinhThanh)
                .HasForeignKey(e => e.ID_TinhThanh);

            modelBuilder.Entity<DM_TinhThanh>()
                .HasMany(e => e.OptinForm_DoiTuong)
                .WithOptional(e => e.DM_TinhThanh)
                .HasForeignKey(e => e.ID_TinhThanh);

            modelBuilder.Entity<DM_ViTri>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.DM_ViTri)
                .HasForeignKey(e => e.ID_ViTri);

            modelBuilder.Entity<DM_ViTri>()
                .HasMany(e => e.BH_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_ViTri)
                .HasForeignKey(e => e.ID_ViTri);

            modelBuilder.Entity<DM_VungMien>()
                .HasMany(e => e.DM_TinhThanh)
                .WithOptional(e => e.DM_VungMien)
                .HasForeignKey(e => e.ID_VungMien);

            modelBuilder.Entity<DM_VungMien>()
                .HasMany(e => e.DM_NhomDoiTuong_ChiTiet)
                .WithOptional(e => e.DM_VungMien)
                .HasForeignKey(e => e.GiaTriVungMien);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.BH_HoaDon_ChiTiet)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.ChietKhauMacDinh_NhanVien)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.DinhLuongDichVus)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.DM_GiaBan_ChiTiet)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.Kho_HoaDon_ChiTiet)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.Kho_TonKhoKhoiTao)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.The_TheKhachHang_ChiTiet)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.DM_KhuyenMai_ChiTiet)
                .WithOptional(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.DM_KhuyenMai_ChiTiet1)
                .WithOptional(e => e.DonViQuiDoi1)
                .HasForeignKey(e => e.ID_DonViQuiDoiMua);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.DM_GiaVon)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuiDoi);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.DM_HangHoa_TonKho)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.ID_DonViQuyDoi);

            modelBuilder.Entity<DonViQuiDoi>()
                .HasMany(e => e.NhomHang_ChiTietSanPhamHoTro)
                .WithRequired(e => e.DonViQuiDoi)
                .HasForeignKey(e => e.Id_DonViQuiDoi);

            modelBuilder.Entity<HT_NguoiDung>()
                .HasMany(e => e.HT_NguoiDung_Nhom)
                .WithRequired(e => e.HT_NguoiDung)
                .HasForeignKey(e => e.IDNguoiDung)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HT_NguoiDung>()
                .HasMany(e => e.HT_QuyenMacDinh)
                .WithRequired(e => e.HT_NguoiDung)
                .HasForeignKey(e => e.IDNguoiDung)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HT_NguoiDung>()
                .HasMany(e => e.HT_ThongBao_CaiDat)
                .WithRequired(e => e.HT_NguoiDung)
                .HasForeignKey(e => e.ID_NguoiDung);

            modelBuilder.Entity<HT_NguoiDung>()
                .HasMany(e => e.HeThong_SMS)
                .WithRequired(e => e.HT_NguoiDung)
                .HasForeignKey(e => e.ID_NguoiGui);

            modelBuilder.Entity<HT_NguoiDung>()
                .HasMany(e => e.HeThong_SMS_TaiKhoanChuyen)
                .WithRequired(e => e.HT_NguoiDungChuyen)
                .HasForeignKey(e => e.ID_NguoiChuyenTien)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HT_NguoiDung>()
                .HasMany(e => e.HeThong_SMS_TaiKhoanNhan)
                .WithRequired(e => e.HT_NguoiDungNhan)
                .HasForeignKey(e => e.ID_NguoiNhanTien)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HT_NhomNguoiDung>()
                .HasMany(e => e.HT_NguoiDung_Nhom)
                .WithRequired(e => e.HT_NhomNguoiDung)
                .HasForeignKey(e => e.IDNhomNguoiDung)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HT_NhomNguoiDung>()
                .HasMany(e => e.HT_Quyen_Nhom)
                .WithRequired(e => e.HT_NhomNguoiDung)
                .HasForeignKey(e => e.ID_NhomNguoiDung)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HT_Quyen>()
                .HasMany(e => e.HT_Quyen_Nhom)
                .WithRequired(e => e.HT_Quyen)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kho_HoaDon>()
                .HasMany(e => e.Kho_HoaDon_ChiTiet)
                .WithRequired(e => e.Kho_HoaDon)
                .HasForeignKey(e => e.ID_HoaDon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NS_LuongDoanhThu>()
                .HasMany(e => e.NS_HoSoLuong)
                .WithOptional(e => e.NS_LuongDoanhThu)
                .HasForeignKey(e => e.ID_LuongDoanhThu);

            modelBuilder.Entity<NS_LuongDoanhThu>()
                .HasMany(e => e.NS_LuongDoanhThu_ChiTiet)
                .WithRequired(e => e.NS_LuongDoanhThu)
                .HasForeignKey(e => e.ID_LuongDoanhThu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.ChietKhauMacDinh_NhanVien)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVienPhuTrach);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.HT_NguoiDung)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Kho_HoaDon)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_HoSoLuong)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Quy_HoaDon)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.The_TheKhachHang)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVienLap);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.DM_GiaBan_ApDung)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_QuaTrinhCongTac)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.BH_NhanVienThucHien)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.DM_KhuyenMai_ApDung)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.HT_NhatKySuDung)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.ChamSocKhachHang)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.ChamSocKhachHang1)
                .WithOptional(e => e.NS_NhanVien1)
                .HasForeignKey(e => e.ID_NhanVienQuanLy);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_NhanVien_Anh)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_CongViec)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVienChiaSe);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_CongViecQuanLy)
                .WithOptional(e => e.NS_NhanVienQuanLy)
                .HasForeignKey(e => e.ID_NhanVienQuanLy);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_NhanVien_CongTac)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_NhanVien_DaoTao)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_NhanVien_GiaDinh)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_NhanVien_SucKhoe)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_Luong_PhuCap)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_BaoHiem)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_HopDong)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_KhenThuong)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_MienGiamThue)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_MaChamCong)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_PhieuPhanCa)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVienTao)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.ChietKhauMacDinh_HoaDon_ChiTiet)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.ChietKhauDoanhThu_NhanVien)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.OptinForm_DoiTuong)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVienPhuTrach);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.ChamSocKhachHang_NhanVien)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_PhieuPhanCa_NhanVien)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_BangLuongDuyet)
                .WithOptional(e => e.NS_NhanVienDuyet)
                .HasForeignKey(e => e.ID_NhanVienDuyet);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_BangLuong_ChiTiet)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_CongBoSung)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.NS_CongNoTamUngLuong)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Gara_PhieuTiepNhan)
                .WithRequired(e => e.NS_NhanVien)
                .HasForeignKey(e => e.ID_NhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Gara_PhieuTiepNhanCoVan)
                .WithOptional(e => e.NS_NhanVienCoVan)
                .HasForeignKey(e => e.ID_CoVanDichVu);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Gara_Xe_PhieuBanGiao_NVGiao)
                .WithRequired(e => e.NS_NhanVienBanGiao)
                .HasForeignKey(e => e.IdNhanVienBanGiao);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Gara_Xe_PhieuBanGiao_NVNhan)
                .WithOptional(e => e.NS_NhanVienTiepNhan)
                .HasForeignKey(e => e.IdNhanVienTiepNhan);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Gara_Xe_PhieuBanGiao)
                .WithOptional(e => e.NS_NhanVien)
                .HasForeignKey(e => e.IdNhanVien);

            modelBuilder.Entity<NS_NhanVien>()
                .HasMany(e => e.Gara_Xe_NhatKyHoatDong_NVThucHien)
                .WithOptional(e => e.NS_NhanVien_NVThucHien)
                .HasForeignKey(e => e.IdNhanVienThucHien);

            modelBuilder.Entity<Quy_HoaDon>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithRequired(e => e.Quy_HoaDon)
                .HasForeignKey(e => e.ID_HoaDon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Quy_HoaDon>()
                .HasMany(e => e.BH_NhanVienThucHien)
                .WithOptional(e => e.Quy_HoaDon)
                .HasForeignKey(e => e.ID_QuyHoaDon);

            modelBuilder.Entity<Quy_KhoanThuChi>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.Quy_KhoanThuChi)
                .HasForeignKey(e => e.ID_KhoanThuChi);

            modelBuilder.Entity<The_NhomThe>()
                .HasMany(e => e.The_TheKhachHang)
                .WithRequired(e => e.The_NhomThe)
                .HasForeignKey(e => e.ID_NhomThe)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<The_TheKhachHang>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.The_TheKhachHang)
                .HasForeignKey(e => e.ID_TheKhachHang);

            modelBuilder.Entity<The_TheKhachHang>()
                .HasMany(e => e.The_TheKhachHang_ChiTiet)
                .WithRequired(e => e.The_TheKhachHang)
                .HasForeignKey(e => e.ID_TheKhachHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DM_LoaiTuVanLichHen>()
                .HasMany(e => e.ChamSocKhachHang)
                .WithOptional(e => e.DM_LoaiTuVanLichHen)
                .HasForeignKey(e => e.ID_LoaiTuVan);

            modelBuilder.Entity<DM_KhuyenMai>()
                .HasMany(e => e.DM_KhuyenMai_ApDung)
                .WithRequired(e => e.DM_KhuyenMai)
                .HasForeignKey(e => e.ID_KhuyenMai);

            modelBuilder.Entity<DM_KhuyenMai>()
                .HasMany(e => e.DM_KhuyenMai_ChiTiet)
                .WithRequired(e => e.DM_KhuyenMai)
                .HasForeignKey(e => e.ID_KhuyenMai);

            modelBuilder.Entity<DM_KhuyenMai>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.DM_KhuyenMai)
                .HasForeignKey(e => e.ID_KhuyenMai);

            modelBuilder.Entity<DM_KhuyenMai>()
                .HasMany(e => e.BH_HoaDon_ChiTiet)
                .WithOptional(e => e.DM_KhuyenMai)
                .HasForeignKey(e => e.ID_KhuyenMai);

            modelBuilder.Entity<HT_CauHinhPhanMem>()
                .HasMany(e => e.HT_CauHinh_TichDiemChiTiet)
                .WithRequired(e => e.HT_CauHinhPhanMem)
                .HasForeignKey(e => e.ID_CauHinh);

            modelBuilder.Entity<HT_CauHinhPhanMem>()
                .HasMany(e => e.HT_CauHinh_GioiHanTraHang)
                .WithRequired(e => e.HT_CauHinhPhanMem)
                .HasForeignKey(e => e.ID_CauHinh);

            modelBuilder.Entity<HT_CauHinh_TichDiemChiTiet>()
                .HasMany(e => e.HT_CauHinh_TichDiemApDung)
                .WithRequired(e => e.HT_CauHinh_TichDiemChiTiet)
                .HasForeignKey(e => e.ID_TichDiem);

            modelBuilder.Entity<NS_PhongBan>()
                .HasMany(e => e.NS_PhongBanCha)
                .WithOptional(e => e.NS_PhongBan1)
                .HasForeignKey(e => e.ID_PhongBanCha);

            modelBuilder.Entity<NS_PhongBan>()
                .HasMany(e => e.NS_NhanVien)
                .WithOptional(e => e.NS_PhongBan)
                .HasForeignKey(e => e.ID_NSPhongBan);

            modelBuilder.Entity<NS_PhongBan>()
                .HasMany(e => e.NS_QuaTrinhCongTac)
                .WithOptional(e => e.NS_PhongBan)
                .HasForeignKey(e => e.ID_PhongBan);

            modelBuilder.Entity<NS_CongViec_PhanLoai>()
                .HasMany(e => e.NS_CongViec)
                .WithRequired(e => e.NS_CongViec_PhanLoai)
                .HasForeignKey(e => e.ID_LoaiCongViec);

            modelBuilder.Entity<NS_LoaiLuong>()
                .HasMany(e => e.NS_Luong_PhuCap)
                .WithOptional(e => e.NS_LoaiLuong)
                .HasForeignKey(e => e.ID_LoaiLuong);

            modelBuilder.Entity<DM_DoiTuong_TrangThai>()
                .HasMany(e => e.DM_DoiTuong)
                .WithOptional(e => e.DM_DoiTuong_TrangThai)
                .HasForeignKey(e => e.ID_TrangThai);

            modelBuilder.Entity<DM_HangHoa_ViTri>()
                .HasMany(e => e.DM_ViTriHangHoa)
                .WithRequired(e => e.DM_HangHoa_ViTri)
                .HasForeignKey(e => e.ID_ViTri);

            modelBuilder.Entity<NS_MayChamCong>()
                .HasMany(e => e.NS_DuLieuCongTho)
                .WithRequired(e => e.NS_MayChamCong)
                .HasForeignKey(e => e.ID_MCC);

            modelBuilder.Entity<NS_MayChamCong>()
                .HasMany(e => e.NS_MaChamCong)
                .WithRequired(e => e.NS_MayChamCong)
                .HasForeignKey(e => e.ID_MCC);

            modelBuilder.Entity<NS_MayChamCong>()
                .HasMany(e => e.NS_CongBoSung)
                .WithOptional(e => e.NS_MayChamCong)
                .HasForeignKey(e => e.ID_MayChamCong);

            modelBuilder.Entity<NS_CaLamViec>()
                .HasMany(e => e.NS_CaLamViec_DonVi)
                .WithRequired(e => e.NS_CaLamViec)
                .HasForeignKey(e => e.ID_CaLamViec);

            modelBuilder.Entity<NS_CaLamViec>()
                .HasMany(e => e.NS_BangLuongOTChiTiet)
                .WithRequired(e => e.NS_CaLamViec)
                .HasForeignKey(e => e.ID_CaLamViec);

            modelBuilder.Entity<NS_CaLamViec>()
                .HasMany(e => e.NS_ThietLapLuongChiTiet)
                .WithOptional(e => e.NS_CaLamViec)
                .HasForeignKey(e => e.ID_CaLamViec);

            modelBuilder.Entity<NS_CaLamViec>()
                .HasMany(e => e.NS_CongBoSung)
                .WithOptional(e => e.NS_CaLamViec)
                .HasForeignKey(e => e.ID_CaLamViec);

            modelBuilder.Entity<NS_KyTinhCong>()
                .HasMany(e => e.NS_KhenThuong)
                .WithOptional(e => e.NS_KyTinhCong)
                .HasForeignKey(e => e.ID_KyTinhCong);

            modelBuilder.Entity<ChietKhauMacDinh_HoaDon>()
                .HasMany(e => e.ChietKhauMacDinh_HoaDon_ChiTiet)
                .WithRequired(e => e.ChietKhauMacDinh_HoaDon)
                .HasForeignKey(e => e.ID_ChietKhauHoaDon);

            modelBuilder.Entity<ChietKhauDoanhThu>()
                .HasMany(e => e.ChietKhauDoanhThu_ChiTiet)
                .WithRequired(e => e.ChietKhauDoanhThu)
                .HasForeignKey(e => e.ID_ChietKhauDoanhThu);

            modelBuilder.Entity<ChietKhauDoanhThu>()
                .HasMany(e => e.ChietKhauDoanhThu_NhanVien)
                .WithRequired(e => e.ChietKhauDoanhThu)
                .HasForeignKey(e => e.ID_ChietKhauDoanhThu);

            modelBuilder.Entity<OptinForm_TruongThongTin>()
                .HasMany(e => e.OptinForm_ThietLap)
                .WithRequired(e => e.OptinForm_TruongThongTin)
                .HasForeignKey(e => e.ID_TruongThongTin);

            modelBuilder.Entity<OptinForm>()
                .HasMany(e => e.OptinForm_ThietLap)
                .WithRequired(e => e.OptinForm)
                .HasForeignKey(e => e.ID_OptinForm);

            modelBuilder.Entity<OptinForm>()
                .HasMany(e => e.OptinForm_ThietLapThongBao)
                .WithRequired(e => e.OptinForm)
                .HasForeignKey(e => e.ID_OptinForm);

            modelBuilder.Entity<OptinForm>()
                .HasMany(e => e.OptinForm_NgayNghiLe)
                .WithRequired(e => e.OptinForm)
                .HasForeignKey(e => e.ID_OptinForm);

            modelBuilder.Entity<OptinForm>()
                .HasMany(e => e.OptinForm_NgayLamViec)
                .WithRequired(e => e.OptinForm)
                .HasForeignKey(e => e.ID_OptinForm);

            modelBuilder.Entity<OptinForm>()
                .HasMany(e => e.OptinForm_Link)
                .WithRequired(e => e.OptinForm)
                .HasForeignKey(e => e.ID_OptinForm);

            modelBuilder.Entity<OptinForm_DoiTuong>()
                .HasMany(e => e.OptinForm_Link)
                .WithOptional(e => e.OptinForm_DoiTuong)
                .HasForeignKey(e => e.ID_DoiTuongOptinForm);

            modelBuilder.Entity<ChamSocKhachHang>()
                .HasMany(e => e.OptinForm_Link)
                .WithOptional(e => e.ChamSocKhachHang)
                .HasForeignKey(e => e.ID_ChamSocKhachHang);

            modelBuilder.Entity<ChamSocKhachHang>()
                .HasMany(e => e.ChamSocKhachHang2)
                .WithOptional(e => e.ChamSocKhachHang1)
                .HasForeignKey(e => e.ID_ChamSocKhachHang);

            modelBuilder.Entity<ChamSocKhachHang>()
                .HasMany(e => e.ChamSocKhachHang_NhanVien)
                .WithRequired(e => e.ChamSocKhachHang)
                .HasForeignKey(e => e.ID_ChamSocKhachHang);

            modelBuilder.Entity<NS_PhieuPhanCa>()
                .HasMany(e => e.NS_PhieuPhanCa_CaLamViec)
                .WithRequired(e => e.NS_PhieuPhanCa)
                .HasForeignKey(e => e.ID_PhieuPhanCa);

            modelBuilder.Entity<NS_PhieuPhanCa>()
                .HasMany(e => e.NS_PhieuPhanCa_NhanVien)
                .WithRequired(e => e.NS_PhieuPhanCa)
                .HasForeignKey(e => e.ID_PhieuPhanCa);

            modelBuilder.Entity<NS_CaLamViec>()
                .HasMany(e => e.NS_PhieuPhanCa_CaLamViec)
                .WithRequired(e => e.NS_CaLamViec)
                .HasForeignKey(e => e.ID_CaLamViec);

            modelBuilder.Entity<NS_BangLuong>()
                .HasMany(e => e.NS_BangLuong_ChiTiet)
                .WithRequired(e => e.NS_BangLuong)
                .HasForeignKey(e => e.ID_BangLuong);

            //modelBuilder.Entity<NS_BangLuong>()
            //    .HasMany(e => e.NS_NgayNghiLe)
            //    .WithOptional(e => e.NS_BangLuong)
            //    .HasForeignKey(e => e.ID_BangLuong);

            modelBuilder.Entity<NS_LoaiBaoHiem>()
                .HasMany(e => e.NS_BaoHiem)
                .WithOptional(e => e.NS_LoaiBaoHiem)
                .HasForeignKey(e => e.ID_LoaiBaoHiem);

            modelBuilder.Entity<NS_LoaiKhenThuong>()
                .HasMany(e => e.NS_KhenThuong)
                .WithOptional(e => e.NS_LoaiKhenThuong)
                .HasForeignKey(e => e.ID_LoaiKhenThuong);

            modelBuilder.Entity<NS_Luong_PhuCap>()
                .HasMany(e => e.NS_ThietLapLuongChiTiet)
                .WithRequired(e => e.NS_Luong_PhuCap)
                .HasForeignKey(e => e.ID_LuongPhuCap);

            modelBuilder.Entity<NS_BangLuong_ChiTiet>()
                .HasMany(e => e.NS_BangLuongOTChiTiet)
                .WithRequired(e => e.NS_BangLuong_ChiTiet)
                .HasForeignKey(e => e.ID_BangLuongChiTiet);

            modelBuilder.Entity<NS_BangLuong_ChiTiet>()
                .HasMany(e => e.NS_CongBoSung)
                .WithOptional(e => e.NS_BangLuong_ChiTiet)
                .HasForeignKey(e => e.ID_BangLuongChiTiet);

            modelBuilder.Entity<NS_BangLuong_ChiTiet>()
                .HasMany(e => e.NS_CongBoSung)
                .WithOptional(e => e.NS_BangLuong_ChiTiet)
                .HasForeignKey(e => e.ID_BangLuongChiTiet);

            modelBuilder.Entity<NS_BangLuong_ChiTiet>()
                .HasMany(e => e.Quy_HoaDon_ChiTiet)
                .WithOptional(e => e.NS_BangLuong_ChiTiet)
                .HasForeignKey(e => e.ID_BangLuongChiTiet);

            modelBuilder.Entity<Gara_HangXe>()
                .HasMany(e => e.Gara_MauXe)
                .WithRequired(e => e.Gara_HangXe)
                .HasForeignKey(e => e.ID_HangXe);

            modelBuilder.Entity<Gara_LoaiXe>()
                .HasMany(e => e.Gara_MauXe)
                .WithRequired(e => e.Gara_LoaiXe)
                .HasForeignKey(e => e.ID_LoaiXe);

            modelBuilder.Entity<Gara_MauXe>()
                .HasMany(e => e.Gara_DanhMucXe)
                .WithRequired(e => e.Gara_MauXe)
                .HasForeignKey(e => e.ID_MauXe);

            modelBuilder.Entity<Gara_DanhMucXe>()
                .HasMany(e => e.Gara_PhieuTiepNhan)
                .WithRequired(e => e.Gara_DanhMucXe)
                .HasForeignKey(e => e.ID_Xe);

            modelBuilder.Entity<Gara_PhieuTiepNhan>()
                .HasMany(e => e.Gara_HangMucSuaChua)
                .WithRequired(e => e.Gara_PhieuTiepNhan)
                .HasForeignKey(e => e.ID_PhieuTiepNhan);

            modelBuilder.Entity<Gara_PhieuTiepNhan>()
                .HasMany(e => e.Gara_GiayToKemTheo)
                .WithRequired(e => e.Gara_PhieuTiepNhan)
                .HasForeignKey(e => e.ID_PhieuTiepNhan);

            modelBuilder.Entity<Gara_PhieuTiepNhan>()
                .HasMany(e => e.BH_HoaDon)
                .WithOptional(e => e.Gara_PhieuTiepNhan)
                .HasForeignKey(e => e.ID_PhieuTiepNhan);

            modelBuilder.Entity<Gara_LichBaoDuong>()
                .HasMany(e => e.BH_HoaDon_ChiTiet)
                .WithOptional(e => e.Gara_LichBaoDuong)
                .HasForeignKey(e => e.ID_LichBaoDuong);

            modelBuilder.Entity<CSKH_DatLich>()
                .HasMany(e => e.CSKH_DatLich_HangHoa)
                .WithRequired(e => e.CSKH_DatLich)
                .HasForeignKey(e => e.IDDatLich);

            modelBuilder.Entity<CSKH_DatLich>()
                .HasMany(e => e.CSKH_DatLich_NhanVien)
                .WithRequired(e => e.CSKH_DatLich)
                .HasForeignKey(e => e.IDDatLich);

            modelBuilder.Entity<Gara_Xe_PhieuBanGiao>()
                .HasMany(e => e.Gara_Xe_NhatKyHoatDong)
                .WithRequired(e => e.Gara_Xe_PhieuBanGiao)
                .HasForeignKey(e => e.IdPhieuBanGiao);
        }
    }

    //public class MigrateDBConfiguration : System.Data.Entity.Migrations.DbMigrationsConfiguration<SsoftvnContext>
    //{
    //    public MigrateDBConfiguration()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //        AutomaticMigrationDataLossAllowed = true;
    //    }
    //}
}
