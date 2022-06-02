using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_MauIn")]
    public partial class DM_MauIn
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int ID_LoaiChungTu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenMauIn { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string KhoGiay { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DuLieuMauIn { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool LaMacDinh { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        public virtual DM_LoaiChungTu DM_LoaiChungTu { get; set; }
        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
