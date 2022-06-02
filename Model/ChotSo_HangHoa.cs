using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChotSo_HangHoa")]
    public partial class ChotSo_HangHoa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChotSo_HangHoa()
        {

        }
        [Column(TypeName = "uniqueidentifier")]
        [Key]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangHoa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoHang { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayChotSo { get; set; }

        [Column(TypeName = "float")]
        public double TonKho { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual DM_HangHoa DM_HangHoa { get; set; }
        public virtual DM_LoHang DM_LoHang { get; set; }
    }
}
