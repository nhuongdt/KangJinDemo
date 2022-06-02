using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_MayChamCong")]
    public partial class DM_MayChamCong
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int MaMayChamCong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenMayChamCong { get; set; } = string.Empty;

        [StringLength(15)]
        [Column(TypeName = "nvarchar")]
        public string IP { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? Port { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string TenMien { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? COMPort { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? KieuKetNoi { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? BaudRate { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? SuDungTenMien { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool SuDung { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }
    }
}
