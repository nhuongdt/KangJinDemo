using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_NhanVien")]
    public partial class NS_NhanVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_NhanVien()
        {
            BH_HoaDon = new HashSet<BH_HoaDon>();
            ChietKhauMacDinh_NhanVien = new HashSet<ChietKhauMacDinh_NhanVien>();
            DM_DoiTuong = new HashSet<DM_DoiTuong>();
            HT_NguoiDung = new HashSet<HT_NguoiDung>();
            Kho_HoaDon = new HashSet<Kho_HoaDon>();
            NS_HoSoLuong = new HashSet<NS_HoSoLuong>();
            Quy_HoaDon = new HashSet<Quy_HoaDon>();
            The_TheKhachHang = new HashSet<The_TheKhachHang>();
            DM_GiaBan_ApDung = new HashSet<DM_GiaBan_ApDung>();
            NS_QuaTrinhCongTac = new HashSet<NS_QuaTrinhCongTac>();
            ChamSocKhachHang = new HashSet<ChamSocKhachHang>();
            ChamSocKhachHang1 = new HashSet<ChamSocKhachHang>();
            BH_NhanVienThucHien = new HashSet<BH_NhanVienThucHien>();
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
            HT_NhatKySuDung = new HashSet<HT_NhatKySuDung>();
            NS_NhanVien_Anh = new HashSet<NS_NhanVien_Anh>();
            NS_NhanVien_DaoTao = new HashSet<NS_NhanVien_DaoTao>();
            NS_NhanVien_CongTac = new HashSet<NS_NhanVien_CongTac>();
            NS_NhanVien_GiaDinh = new HashSet<NS_NhanVien_GiaDinh>();
            NS_NhanVien_SucKhoe = new HashSet<NS_NhanVien_SucKhoe>();
            NS_Luong_PhuCap = new HashSet<NS_Luong_PhuCap>();
            NS_BaoHiem = new HashSet<NS_BaoHiem>();
            NS_HopDong = new HashSet<NS_HopDong>();
            NS_KhenThuong = new HashSet<NS_KhenThuong>();
            NS_MienGiamThue = new HashSet<NS_MienGiamThue>();
            NS_CongViecQuanLy = new HashSet<NS_CongViec>();
            NS_MaChamCong = new HashSet<NS_MaChamCong>();
            NS_PhieuPhanCa = new HashSet<NS_PhieuPhanCa>();
            ChietKhauMacDinh_HoaDon_ChiTiet = new HashSet<ChietKhauMacDinh_HoaDon_ChiTiet>();
            ChietKhauDoanhThu_NhanVien = new HashSet<ChietKhauDoanhThu_NhanVien>();
            OptinForm_DoiTuong = new HashSet<OptinForm_DoiTuong>();
            ChamSocKhachHang_NhanVien = new HashSet<ChamSocKhachHang_NhanVien>();
            NS_PhieuPhanCa_NhanVien = new HashSet<NS_PhieuPhanCa_NhanVien>();
            NS_BangLuongDuyet = new HashSet<NS_BangLuong>();
            NS_BangLuong_ChiTiet = new HashSet<NS_BangLuong_ChiTiet>();
            NS_CongBoSung = new HashSet<NS_CongBoSung>();
            NS_CongNoTamUngLuong = new HashSet<NS_CongNoTamUngLuong>();
            Gara_PhieuTiepNhan = new HashSet<Gara_PhieuTiepNhan>();
            Gara_PhieuTiepNhanCoVan = new HashSet<Gara_PhieuTiepNhan>();
            CSKH_DatLich_NhanVien = new HashSet<CSKH_DatLich_NhanVien>();
            Gara_Xe_PhieuBanGiao = new HashSet<Gara_Xe_PhieuBanGiao>();
            Gara_Xe_PhieuBanGiao_NVGiao = new HashSet<Gara_Xe_PhieuBanGiao>();
            Gara_Xe_PhieuBanGiao_NVNhan = new HashSet<Gara_Xe_PhieuBanGiao>();
            Gara_Xe_NhatKyHoatDong_NVThucHien = new HashSet<Gara_Xe_NhatKyHoatDong>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaNhanVien { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenNhanVien { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenNhanVienKhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenNhanVienChuCaiDau { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySinh { get; set; }

        [Column(TypeName = "bit")]
        public bool GioiTinh { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiSinh { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string NguyenQuan { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string ThuongTru { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TamTru { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string DienThoaiCoQuan { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string DienThoaiNhaRieng { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string DienThoaiDiDong { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string SoFax { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChiCoQuan { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string SoCMND { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string SoBHXH { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string Website { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? CapTaiKhoan { get; set; } = false;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool DaNghiViec { get; set; }

        [Column(TypeName = "int")]
        public int? TinhTrangHonNhan { get; set; } = 0;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NSPhongBan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TinhThanhTT { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuanHuyenTT { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_XaPhuongTT { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TinhThanhHKTT { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuanHuyenHKTT { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_XaPhuongHKTT { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayCap { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiCap { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayVaoLamViec { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChiTT { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChiHKTT { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DanTocTonGiao { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TonGiao { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string NoiVaoDoan { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayVaoDoan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiSinhHoatDang { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayVaoDang { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayVaoDangChinhThuc { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayRoiDang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string LyDoRoiDang { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayNhapNgu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayXuatNgu { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ChucVuCaoNhat { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChuThongTinChinhTri { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuocGia { get; set; }

        [Column(TypeName = "int")]
        public int? TrangThai { get; set; } = 1;

        public virtual NS_PhongBan NS_PhongBan { get; set; }

        public virtual DM_TinhThanh DM_TinhThanhTT { get; set; }

        public virtual DM_QuanHuyen DM_QuanHuyenTT { get; set; }

        public virtual DM_XaPhuong DM_XaPhuongTT { get; set; }

        public virtual DM_TinhThanh DM_TinhThanhHKTT { get; set; }

        public virtual DM_QuanHuyen DM_QuanHuyenHKTT { get; set; }

        public virtual DM_XaPhuong DM_XaPhuongHKTT { get; set; }

        public virtual DM_QuocGia DM_QuocGia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauMacDinh_NhanVien> ChietKhauMacDinh_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong> DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NguoiDung> HT_NguoiDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon> Kho_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_HoSoLuong> NS_HoSoLuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon> Quy_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<The_TheKhachHang> The_TheKhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaBan_ApDung> DM_GiaBan_ApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_QuaTrinhCongTac> NS_QuaTrinhCongTac { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_NhanVienThucHien> BH_NhanVienThucHien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ApDung> DM_KhuyenMai_ApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NhatKySuDung> HT_NhatKySuDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien_Anh> NS_NhanVien_Anh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongViec> NS_CongViec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien_DaoTao> NS_NhanVien_DaoTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien_CongTac> NS_NhanVien_CongTac { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien_GiaDinh> NS_NhanVien_GiaDinh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien_SucKhoe> NS_NhanVien_SucKhoe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_Luong_PhuCap> NS_Luong_PhuCap { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BaoHiem> NS_BaoHiem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_HopDong> NS_HopDong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_KhenThuong> NS_KhenThuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_MienGiamThue> NS_MienGiamThue { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongViec> NS_CongViecQuanLy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_MaChamCong> NS_MaChamCong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhieuPhanCa> NS_PhieuPhanCa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauMacDinh_HoaDon_ChiTiet> ChietKhauMacDinh_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauDoanhThu_NhanVien> ChietKhauDoanhThu_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_DoiTuong> OptinForm_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang_NhanVien> ChamSocKhachHang_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhieuPhanCa_NhanVien> NS_PhieuPhanCa_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BangLuong> NS_BangLuongDuyet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BangLuong_ChiTiet> NS_BangLuong_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongBoSung> NS_CongBoSung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongNoTamUngLuong> NS_CongNoTamUngLuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_PhieuTiepNhan> Gara_PhieuTiepNhan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_PhieuTiepNhan> Gara_PhieuTiepNhanCoVan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CSKH_DatLich_NhanVien> CSKH_DatLich_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_PhieuBanGiao> Gara_Xe_PhieuBanGiao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_PhieuBanGiao> Gara_Xe_PhieuBanGiao_NVGiao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_PhieuBanGiao> Gara_Xe_PhieuBanGiao_NVNhan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_NhatKyHoatDong> Gara_Xe_NhatKyHoatDong_NVThucHien { get; set; }

        [NotMapped]
        public Guid? ID_NguoiDung { get; set; }
    }
}
