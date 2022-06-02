using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_PhieuPhanCa_NhanVien")]
    public partial class NS_PhieuPhanCa_NhanVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_PhieuPhanCa_NhanVien ()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_PhieuPhanCa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        public virtual NS_PhieuPhanCa NS_PhieuPhanCa { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
