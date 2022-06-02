using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_NhanVien_DaoTao")]
    public partial class NS_NhanVien_DaoTao
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime TuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DenNgay { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiHoc { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string NganhHoc { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string HeDaoTao { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string BangCap { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
