using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("CSKH_DatLich_HangHoa")]
    public partial class CSKH_DatLich_HangHoa
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDDatLich { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDHangHoa { get; set; }

        public virtual CSKH_DatLich CSKH_DatLich { get; set; }
        public virtual DM_HangHoa DM_HangHoa { get; set; }
    }
}
