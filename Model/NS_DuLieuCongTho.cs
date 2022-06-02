using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_DuLieuCongTho")]
    public partial class NS_DuLieuCongTho
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaChamCong { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime ThoiGian { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_MCC { get; set; }

        [Column(TypeName = "int")]
        public int VaoRa { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        public virtual NS_MayChamCong NS_MayChamCong { get; set; }
    }
}
