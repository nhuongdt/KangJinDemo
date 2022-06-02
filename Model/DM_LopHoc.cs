using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_LopHoc")]
    public partial class DM_LopHoc
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaLopHoc { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenLopHoc { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayBD { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayKT { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioBD { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioKT { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NgayTrongTuan { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? GiaoVienPhuTrach { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "ntext")]
        public string GhiChu { get; set; } = string.Empty;

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

        [Column(TypeName = "float")]
        public double? ChietKhau_GiaoVienPhuTrach { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? LaPhanTram_CKGV { get; set; } = false;

        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
