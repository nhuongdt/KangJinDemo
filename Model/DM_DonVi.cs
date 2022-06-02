using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_DonVi")]
    public partial class DM_DonVi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_DonVi()
        {
            BH_HoaDon = new HashSet<BH_HoaDon>();
            DM_DoiTuong = new HashSet<DM_DoiTuong>();
            DM_DonVi1 = new HashSet<DM_DonVi>();
            DM_LopHoc = new HashSet<DM_LopHoc>();
            HT_NguoiDung = new HashSet<HT_NguoiDung>();
            Kho_DonVi = new HashSet<Kho_DonVi>();
            Kho_HoaDon = new HashSet<Kho_HoaDon>();
            Kho_TonKhoKhoiTao = new HashSet<Kho_TonKhoKhoiTao>();
            NhomDoiTuong_DonVi = new HashSet<NhomDoiTuong_DonVi>();
            NhomHangHoa_DonVi = new HashSet<NhomHangHoa_DonVi>();
            Quy_HoaDon = new HashSet<Quy_HoaDon>();
            Quy_TonQuyKhoiTao = new HashSet<Quy_TonQuyKhoiTao>();
            The_TheKhachHang = new HashSet<The_TheKhachHang>();
            DM_GiaBan_ApDung = new HashSet<DM_GiaBan_ApDung>();
            NS_QuaTrinhCongTac = new HashSet<NS_QuaTrinhCongTac>();
            ChietKhauMacDinh_NhanVien = new HashSet<ChietKhauMacDinh_NhanVien>();
            HT_CauHinhPhanMem = new HashSet<HT_CauHinhPhanMem>();
            DM_KhuyenMai_ApDung = new HashSet<DM_KhuyenMai_ApDung>();
            ChamSocKhachHang = new HashSet<ChamSocKhachHang>();
            HT_NhatKySuDung = new HashSet<HT_NhatKySuDung>();
            ChotSo = new HashSet<ChotSo>();
            ChotSo_HangHoa = new HashSet<ChotSo_HangHoa>();
            ChotSo_KhachHang = new HashSet<ChotSo_KhachHang>();
            DM_MauIn = new HashSet<DM_MauIn>();
            DM_TaiKhoanNganHang = new HashSet<DM_TaiKhoanNganHang>();
            HT_ThongBao = new HashSet<HT_ThongBao>();
            HT_NguoiDung_Nhom = new HashSet<HT_NguoiDung_Nhom>();
            DM_GiaVon = new HashSet<DM_GiaVon>();
            NS_CongViec = new HashSet<NS_CongViec>();
            HeThong_SMS = new HashSet<HeThong_SMS>();
            NS_PhongBan = new HashSet<NS_PhongBan>();
            NS_MayChamCong = new HashSet<NS_MayChamCong>();
            ChietKhauMacDinh_HoaDon = new HashSet<ChietKhauMacDinh_HoaDon>();
            ChietKhauDoanhThu = new HashSet<ChietKhauDoanhThu>();
            DM_HangHoa_TonKho = new HashSet<DM_HangHoa_TonKho>();
            NS_PhieuPhanCa = new HashSet<NS_PhieuPhanCa>();
            NS_CaLamViec_DonVi = new HashSet<NS_CaLamViec_DonVi>();
            DM_DoiTuong_CongNo = new HashSet<DM_DoiTuong_CongNo>();
            NS_Luong_PhuCap = new HashSet<NS_Luong_PhuCap>();
            NS_BangLuong = new HashSet<NS_BangLuong>();
            NS_KyHieuCong = new HashSet<NS_KyHieuCong>();
            NS_CongBoSung = new HashSet<NS_CongBoSung>();
            NS_CongNoTamUngLuong = new HashSet<NS_CongNoTamUngLuong>();
            Gara_PhieuTiepNhan = new HashSet<Gara_PhieuTiepNhan>();
            CSKH_DatLich = new HashSet<CSKH_DatLich>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string MaDonVi { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenDonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_Parent { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChi { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Website { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string MaSoThue { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SoTaiKhoan { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NganHang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SoFax { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string KiTuDanhMa { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? HienThi_Chinh { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? HienThi_Phu { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? TrangThai { get; set; }// 0: xoa, 1 or NUll. Đang sử dụng

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeThong_SMS> HeThong_SMS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong> DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DonVi> DM_DonVi1 { get; set; }

        public virtual DM_DonVi DM_DonVi2 { get; set; }

        public virtual DM_NganHang DM_NganHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_LopHoc> DM_LopHoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NguoiDung> HT_NguoiDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_DonVi> Kho_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon> Kho_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_TonKhoKhoiTao> Kho_TonKhoKhoiTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhomDoiTuong_DonVi> NhomDoiTuong_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhomHangHoa_DonVi> NhomHangHoa_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon> Quy_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_TonQuyKhoiTao> Quy_TonQuyKhoiTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<The_TheKhachHang> The_TheKhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaBan_ApDung> DM_GiaBan_ApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_QuaTrinhCongTac> NS_QuaTrinhCongTac { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauMacDinh_NhanVien> ChietKhauMacDinh_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_CauHinhPhanMem> HT_CauHinhPhanMem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ApDung> DM_KhuyenMai_ApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NhatKySuDung> HT_NhatKySuDung { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChotSo> ChotSo { get; set; }
        public virtual ICollection<ChotSo_HangHoa> ChotSo_HangHoa { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChotSo_KhachHang> ChotSo_KhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_MauIn> DM_MauIn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_TaiKhoanNganHang> DM_TaiKhoanNganHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_ThongBao> HT_ThongBao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NguoiDung_Nhom> HT_NguoiDung_Nhom { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaVon> DM_GiaVon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongViec> NS_CongViec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhongBan> NS_PhongBan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_MayChamCong> NS_MayChamCong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauMacDinh_HoaDon> ChietKhauMacDinh_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauDoanhThu> ChietKhauDoanhThu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa_TonKho> DM_HangHoa_TonKho { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhieuPhanCa> NS_PhieuPhanCa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CaLamViec_DonVi> NS_CaLamViec_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong_CongNo> DM_DoiTuong_CongNo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_Luong_PhuCap> NS_Luong_PhuCap { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BangLuong> NS_BangLuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_KyHieuCong> NS_KyHieuCong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongBoSung> NS_CongBoSung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongNoTamUngLuong> NS_CongNoTamUngLuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_PhieuTiepNhan> Gara_PhieuTiepNhan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CSKH_DatLich> CSKH_DatLich { get; set; }
    }
}
