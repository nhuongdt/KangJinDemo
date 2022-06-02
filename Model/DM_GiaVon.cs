using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_GiaVon")]
    public partial class DM_GiaVon
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonViQuiDoi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoHang { get; set; }

        [Column(TypeName = "float")]
        public double GiaVon { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual DonViQuiDoi DonViQuiDoi { get; set; }
        public virtual DM_LoHang DM_LoHang { get; set; }
    }
}
