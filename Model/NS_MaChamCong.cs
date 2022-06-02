using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_MaChamCong")]
    public partial class NS_MaChamCong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_MaChamCong()
        {
            
            //NS_ChamCong_ChiTiet = new HashSet<NS_ChamCong_ChiTiet>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_MCC { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaChamCong { get; set; } = string.Empty;

        public virtual NS_NhanVien NS_NhanVien { get; set; }
        public virtual NS_MayChamCong NS_MayChamCong { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NS_ChamCong_ChiTiet> NS_ChamCong_ChiTiet { get; set; }
    }
}
