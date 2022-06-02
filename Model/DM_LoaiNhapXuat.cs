using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_LoaiNhapXuat")]
    public partial class DM_LoaiNhapXuat
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaLoai { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenLoai { get; set; }

        [Column(TypeName = "ntext")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int Nhap_Xuat_DieuChuyen { get; set; }

        [Column(TypeName = "bit")]
        public bool SuDung { get; set; }

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
    }
}
