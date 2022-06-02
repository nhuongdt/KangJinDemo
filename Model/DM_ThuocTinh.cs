using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_ThuocTinh")]
    public partial class DM_ThuocTinh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_ThuocTinh()
        {
            HangHoa_ThuocTinh = new HashSet<HangHoa_ThuocTinh>();
        }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenThuocTinh { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HangHoa_ThuocTinh> HangHoa_ThuocTinh { get; set; }
    }
}
