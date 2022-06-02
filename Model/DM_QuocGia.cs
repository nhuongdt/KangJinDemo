using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_QuocGia")]
    public partial class DM_QuocGia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_QuocGia()
        {
            DM_DoiTuong = new HashSet<DM_DoiTuong>();
            DM_HangHoa = new HashSet<DM_HangHoa>();
            DM_TienTe = new HashSet<DM_TienTe>();
            DM_TinhThanh = new HashSet<DM_TinhThanh>();
            NS_NhanVien = new HashSet<NS_NhanVien>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaQuocGia { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenQuocGia { get; set; }

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
        public virtual ICollection<DM_HangHoa> DM_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_TienTe> DM_TienTe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_TinhThanh> DM_TinhThanh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien> NS_NhanVien { get; set; }
    }
}
