using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_PhimTat")]
    public partial class HT_PhimTat
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaPhim { get; set; } = string.Empty;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenPhim { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? KeyFn { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? KeyCode { get; set; } = 0;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string DienGiai { get; set; } = string.Empty;
    }
}
