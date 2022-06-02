using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_MaChungTu")]
    public partial class HT_MaChungTu
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int ID_LoaiChungTu { get; set; }

        [Column(TypeName = "bit")]
        public bool SuDungMaDonVi { get; set; } = false;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string KiTuNganCach1 { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaLoaiChungTu { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string KiTuNganCach2 { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NgayThangNam { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string KiTuNganCach3 { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int DoDaiSTT { get; set; }

        public virtual DM_LoaiChungTu DM_LoaiChungTu { get; set; }
    }
}
