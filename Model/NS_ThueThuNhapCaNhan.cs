using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_ThueThuNhapCaNhan")]
    public partial class NS_ThueThuNhapCaNhan
    {
        [Column(TypeName = "uniqueidentifier")]
        [Key]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar")]
        public string MoTa { get; set; }

        [Column(TypeName = "float")]
        public double SoTienTu { get; set; } = 0;

        [Column(TypeName = "float")]
        public double SoTienDen { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime NgayApDung { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "int")]
        public int LoaiChiuThue { get; set; } = 1; //1 - Theo %, 2 - Theo số tiền

        [Column(TypeName = "float")]
        public double ThueSuat { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;
    }
}
