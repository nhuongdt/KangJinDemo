using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Kho_TonKhoKhoiTao")]
    public partial class Kho_TonKhoKhoiTao
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayChungTu { get; set; }

        [Column(TypeName = "int")]
        public int NamHachToan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_Kho { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoHang { get; set; }

        [Column(TypeName = "float")]
        public double SoLuong { get; set; }

        [Column(TypeName = "float")]
        public double DonGia { get; set; }

        [Column(TypeName = "float")]
        public double ThanhTien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

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

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonViQuiDoi { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_Kho DM_Kho { get; set; }

        public virtual DM_LoHang DM_LoHang { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }
    }
}
