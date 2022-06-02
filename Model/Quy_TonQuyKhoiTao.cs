using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Quy_TonQuyKhoiTao")]
    public partial class Quy_TonQuyKhoiTao
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayChungTu { get; set; }

        [Column(TypeName = "int")]
        public int NamHachToan { get; set; }

        [Column(TypeName = "float")]
        public double TonQuy { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_TienTe { get; set; }

        [Column(TypeName = "float")]
        public double TyGia { get; set; }

        [Column(TypeName = "bit")]
        public bool? LaTienMat { get; set; } = true;

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

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_TienTe DM_TienTe { get; set; }
    }
}
