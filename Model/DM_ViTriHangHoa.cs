using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_ViTriHangHoa")]
    public partial class DM_ViTriHangHoa
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangHoa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ViTri { get; set; }

        public virtual DM_HangHoa DM_HangHoa { get; set; }
        public virtual DM_HangHoa_ViTri DM_HangHoa_ViTri { get; set; }
    }
}
