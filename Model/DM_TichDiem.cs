using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_TichDiem")]
    public partial class DM_TichDiem
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaTichDiem { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayBatDauApDung { get; set; }

        [Column(TypeName = "float")]
        public double TienThuVe_Min { get; set; }

        [Column(TypeName = "float")]
        public double? TienThuVe_Max { get; set; } = 0;

        [Column(TypeName = "float")]
        public double DiemQuyDoi { get; set; }

        [Column(TypeName = "float")]
        public double MucDiem { get; set; }

        [Column(TypeName = "float")]
        public double TienThanhToanQuyDoi { get; set; }

        [Column(TypeName = "float")]
        public double DiemToiThieu { get; set; }

        [Column(TypeName = "float")]
        public double TienToiThieu { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NhomKhachHang { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string ChungTu { get; set; } = string.Empty;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string ThuTrongTuan { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool ApDung { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayBatDau { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayKetThuc { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? ApDung_NguoiGioiThieu { get; set; } = false;
    }
}
