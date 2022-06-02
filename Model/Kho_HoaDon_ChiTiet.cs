using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Kho_HoaDon_ChiTiet")]
    public partial class Kho_HoaDon_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int? SoThuTu { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HoaDon { get; set; }

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

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_CTChungTuLienQuan { get; set; }

        [Column(TypeName = "bit")]
        public bool? An_Hien { get; set; } = true;

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonViQuiDoi { get; set; }

        public virtual BH_HoaDon_ChiTiet BH_HoaDon_ChiTiet { get; set; }

        public virtual DM_Kho DM_Kho { get; set; }

        public virtual DM_LoHang DM_LoHang { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }

        public virtual Kho_HoaDon Kho_HoaDon { get; set; }
    }
}
