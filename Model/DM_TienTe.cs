using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_TienTe")]
    public partial class DM_TienTe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_TienTe()
        {
            BH_HoaDon = new HashSet<BH_HoaDon>();
            DM_GiaBan_ChiTiet = new HashSet<DM_GiaBan_ChiTiet>();
            DM_TyGia = new HashSet<DM_TyGia>();
            Kho_HoaDon = new HashSet<Kho_HoaDon>();
            Quy_HoaDon = new HashSet<Quy_HoaDon>();
            Quy_TonQuyKhoiTao = new HashSet<Quy_TonQuyKhoiTao>();
            The_TheKhachHang = new HashSet<The_TheKhachHang>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaNgoaiTe { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar")]
        public string TenNgoaiTe { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_QuocGia { get; set; }

        [Column(TypeName = "bit")]
        public bool LaNoiTe { get; set; }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaBan_ChiTiet> DM_GiaBan_ChiTiet { get; set; }

        public virtual DM_QuocGia DM_QuocGia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_TyGia> DM_TyGia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon> Kho_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon> Quy_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_TonQuyKhoiTao> Quy_TonQuyKhoiTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<The_TheKhachHang> The_TheKhachHang { get; set; }
    }
}
