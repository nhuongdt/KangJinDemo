using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_QuanHuyen")]
    public partial class DM_QuanHuyen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_QuanHuyen()
        {
            DM_DoiTuong = new HashSet<DM_DoiTuong>();
            DM_XaPhuong = new HashSet<DM_XaPhuong>();
            NS_NhanVienTT = new HashSet<NS_NhanVien>();
            NS_NhanVienHKTT = new HashSet<NS_NhanVien>();
            DM_LienHe = new HashSet<DM_LienHe>();
            OptinForm_DoiTuong = new HashSet<OptinForm_DoiTuong>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaQuanHuyen { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenQuanHuyen { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_TinhThanh { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong> DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_XaPhuong> DM_XaPhuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien> NS_NhanVienTT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien> NS_NhanVienHKTT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_LienHe> DM_LienHe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_DoiTuong> OptinForm_DoiTuong { get; set; }

        public virtual DM_TinhThanh DM_TinhThanh { get; set; }
    }
}
