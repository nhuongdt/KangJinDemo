using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HangHoa_ThuocTinh")]
    public partial class HangHoa_ThuocTinh
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangHoa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ThuocTinh { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GiaTri { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? ThuTuNhap { get; set; } = 0;

        public virtual DM_ThuocTinh DM_ThuocTinh { get; set; }

        public virtual DM_HangHoa DM_HangHoa { get; set; }

        [NotMapped]
        public int index { get; set; }
    }
}
