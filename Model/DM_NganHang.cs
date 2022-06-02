using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_NganHang")]
    public partial class DM_NganHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_NganHang()
        {
            DM_DoiTuong = new HashSet<DM_DoiTuong>();
            DM_DonVi = new HashSet<DM_DonVi>();
            HT_CongTy = new HashSet<HT_CongTy>();
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
            DM_TaiKhoanNganHang = new HashSet<DM_TaiKhoanNganHang>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaNganHang { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenNganHang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ChiNhanh { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

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

        [Column(TypeName = "float")]
        public double? ChiPhiThanhToan { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? TheoPhanTram { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? MacDinh { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? ThuPhiThanhToan { get; set; } = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong> DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DonVi> DM_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_CongTy> HT_CongTy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_TaiKhoanNganHang> DM_TaiKhoanNganHang { get; set; }
    }
}
