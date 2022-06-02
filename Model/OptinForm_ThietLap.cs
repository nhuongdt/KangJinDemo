using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm_ThietLap")]
    public partial class OptinForm_ThietLap
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_OptinForm { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_TruongThongTin { get; set; }

        [Column(TypeName = "bit")]
        public bool TrangThaiSuDung { get; set; }

        [Column(TypeName = "bit")]
        public bool TrangThaiBatBuoc { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string HienThiGoiY { get; set; } = string.Empty;

        public virtual OptinForm_TruongThongTin OptinForm_TruongThongTin { get; set; }
        public virtual OptinForm OptinForm { get; set; }
    }
}
