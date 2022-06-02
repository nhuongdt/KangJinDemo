using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("The_TheKhachHang")]
    public partial class The_TheKhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public The_TheKhachHang()
        {
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
            The_TheKhachHang_ChiTiet = new HashSet<The_TheKhachHang_ChiTiet>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaThe { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhomThe { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayMua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayApDung { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayHetHan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DoiTuong { get; set; }

        [Column(TypeName = "float")]
        public double MenhGiaThe { get; set; }

        [Column(TypeName = "float")]
        public double? PTChietKhau { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TienChietKhau { get; set; } = 0;

        [Column(TypeName = "float")]
        public double PhaiThanhToan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TienTe { get; set; }

        [Column(TypeName = "float")]
        public double? TyGia { get; set; } = 1;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienLap { get; set; }

        [Column(TypeName = "bit")]
        public bool ApDungTatCaSanPham { get; set; }

        [Column(TypeName = "bit")]
        public bool DuocChoMuon { get; set; }

        [Column(TypeName = "int")]
        public int TheGiaTri_SoLan_GiamGia { get; set; }

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
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "float")]
        public double? PTTangThem { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TienTangThem { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LienHe { get; set; }

        [Column(TypeName = "bit")]
        public bool? HuyThe { get; set; } = false;

        [Column(TypeName = "datetime")]
        public DateTime? NgayHuy { get; set; }

        [Column(TypeName = "int")]
        public int? SoLanDuocSuDung { get; set; } = 0;

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_LienHe DM_LienHe { get; set; }

        public virtual DM_TienTe DM_TienTe { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }

        public virtual The_NhomThe The_NhomThe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<The_TheKhachHang_ChiTiet> The_TheKhachHang_ChiTiet { get; set; }
    }
}
