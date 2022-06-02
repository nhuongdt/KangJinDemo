using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_HangHoa_TonKho")]
    public partial class DM_HangHoa_TonKho
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_HangHoa_TonKho()
        { }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonViQuyDoi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoHang { get; set; }

        [Column(TypeName = "float")]
        public double TonKho { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual DonViQuiDoi DonViQuiDoi { get; set; }
        public virtual DM_LoHang DM_LoHang { get; set; }
    }
}
