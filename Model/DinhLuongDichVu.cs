using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DinhLuongDichVu")]
    public partial class DinhLuongDichVu
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DichVu { get; set; }

        [Column(TypeName = "float")]
        public double SoLuong { get; set; }

        [Column(TypeName = "ntext")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? STT { get; set; } = 0;

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonViQuiDoi { get; set; }

        [Column(TypeName = "float")]
        public double? DonGia { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoHang { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }
        public virtual DM_LoHang DM_LoHang { get; set; }
    }
}
