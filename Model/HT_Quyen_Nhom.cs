using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_Quyen_Nhom")]
    public partial class HT_Quyen_Nhom
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhomNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar")]
        public string MaQuyen { get; set; }

        public virtual HT_NhomNguoiDung HT_NhomNguoiDung { get; set; }

        public virtual HT_Quyen HT_Quyen { get; set; }
    }
}
