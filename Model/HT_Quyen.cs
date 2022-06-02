using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_Quyen")]
    public partial class HT_Quyen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HT_Quyen()
        {
            HT_Quyen_Nhom = new HashSet<HT_Quyen_Nhom>();
        }
        [Key]
        [StringLength(100)]
        [Column(TypeName = "nvarchar")]
        public string MaQuyen { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenQuyen { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string MoTa { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string QuyenCha { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? DuocSuDung { get; set; } = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_Quyen_Nhom> HT_Quyen_Nhom { get; set; }
    }
}
