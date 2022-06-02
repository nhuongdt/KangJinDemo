using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_PhieuPhanCa_CaLamViec")]
    public partial class NS_PhieuPhanCa_CaLamViec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_PhieuPhanCa_CaLamViec()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_PhieuPhanCa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_CaLamViec { get; set; }

        [Column(TypeName = "int")]
        public int GiaTri { get; set; } // thứ mấy trong tuần: 0.sun, 1.mon,..

        public virtual NS_PhieuPhanCa NS_PhieuPhanCa { get; set; }

        public virtual NS_CaLamViec NS_CaLamViec { get; set; }
    }
}
