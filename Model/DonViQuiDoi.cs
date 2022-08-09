using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DonViQuiDoi")]
    public partial class DonViQuiDoi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonViQuiDoi()
        {
            BH_HoaDon_ChiTiet = new HashSet<BH_HoaDon_ChiTiet>();
            ChietKhauMacDinh_NhanVien = new HashSet<ChietKhauMacDinh_NhanVien>();
            DinhLuongDichVus = new HashSet<DinhLuongDichVu>();
            DM_GiaBan_ChiTiet = new HashSet<DM_GiaBan_ChiTiet>();
            Kho_HoaDon_ChiTiet = new HashSet<Kho_HoaDon_ChiTiet>();
            Kho_TonKhoKhoiTao = new HashSet<Kho_TonKhoKhoiTao>();
            The_TheKhachHang_ChiTiet = new HashSet<The_TheKhachHang_ChiTiet>();
            DM_KhuyenMai_ChiTiet = new HashSet<DM_KhuyenMai_ChiTiet>();
            DM_KhuyenMai_ChiTiet1 = new HashSet<DM_KhuyenMai_ChiTiet>();
            DM_GiaVon = new HashSet<DM_GiaVon>();
            DM_HangHoa_TonKho = new HashSet<DM_HangHoa_TonKho>();
            NhomHang_ChiTietSanPhamHoTro = new HashSet<NhomHang_ChiTietSanPhamHoTro>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "nvarchar")]
        public string MaHangHoa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangHoa { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string TenDonViTinh { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double TyLeChuyenDoi { get; set; }

        [Column(TypeName = "bit")]
        public bool LaDonViChuan { get; set; }

        [Column(TypeName = "float")]
        public double GiaVon { get; set; }

        [Column(TypeName = "float")]
        public double GiaNhap { get; set; }

        [Column(TypeName = "float")]
        public double GiaBan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? Xoa { get; set; } = false;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ThuocTinhGiaTri { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauMacDinh_NhanVien> ChietKhauMacDinh_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DinhLuongDichVu> DinhLuongDichVus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaBan_ChiTiet> DM_GiaBan_ChiTiet { get; set; }

        public virtual DM_HangHoa DM_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon_ChiTiet> Kho_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_TonKhoKhoiTao> Kho_TonKhoKhoiTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<The_TheKhachHang_ChiTiet> The_TheKhachHang_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ChiTiet> DM_KhuyenMai_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ChiTiet> DM_KhuyenMai_ChiTiet1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaVon> DM_GiaVon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa_TonKho> DM_HangHoa_TonKho { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhomHang_ChiTietSanPhamHoTro> NhomHang_ChiTietSanPhamHoTro { get; set; }
    }
}
