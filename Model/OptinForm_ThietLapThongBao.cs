using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm_ThietLapThongBao")]
    public partial class OptinForm_ThietLapThongBao
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_OptinForm { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDungThongBao { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string WebDieuHuong { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string ButtonName { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDungHieuLuc { get; set; } = string.Empty;

        public virtual OptinForm OptinForm { get; set; }
    }
}
