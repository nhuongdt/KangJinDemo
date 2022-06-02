using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_ChucVu")]
    public partial class DM_ChucVu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_ChucVu()
        {
            DM_LienHe = new HashSet<DM_LienHe>();
            NS_QuaTrinhCongTac = new HashSet<NS_QuaTrinhCongTac>();

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaChucVu { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenChucVu { get; set; }

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
        public virtual ICollection<DM_LienHe> DM_LienHe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_QuaTrinhCongTac> NS_QuaTrinhCongTac { get; set; }
    }
}
