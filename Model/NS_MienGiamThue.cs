using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_MienGiamThue")]
    public partial class NS_MienGiamThue
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string KhoanMienGiam { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double SoTien { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayApDung { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayKetThuc { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
