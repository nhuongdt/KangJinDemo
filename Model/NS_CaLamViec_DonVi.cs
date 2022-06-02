using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_CaLamViec_DonVi")]
    public partial class NS_CaLamViec_DonVi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_CaLamViec_DonVi()
        {

        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_CaLamViec { get; set; } //foreign key NS_CaLamViec

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; } //foreign key DM_DonVi

        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual NS_CaLamViec NS_CaLamViec { get; set; }
    }
}
