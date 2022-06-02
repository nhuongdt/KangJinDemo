using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("CongDoan_DichVu")]
    public partial class CongDoan_DichVu
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DichVu { get; set; }

        [Column(TypeName = "int")]
        public int STT { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_CongDoan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ThoiGian { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? SoPhutThucHien { get; set; } = 0;
    }
}
