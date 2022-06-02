using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("The_TheKhachHang_ChiTiet")]
    public partial class The_TheKhachHang_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_TheKhachHang { get; set; }

        [Column(TypeName = "float")]
        public double? SoLuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? DonGia { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? PTChietKhau { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TienChietKhau { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? ThanhToan { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LopHoc { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? SoLuongTang { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTraLai { get; set; }

        [Column(TypeName = "float")]
        public double? SoLuongTraLai { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TienDaSuDung { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? TraLaiHHDV { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_SanPhamChinh { get; set; }

        [Column(TypeName = "bit")]
        public bool? LaTangKem { get; set; } = false;

        [Column(TypeName = "float")]
        public double? SoLuongDaSuDung { get; set; } = 0;

        [Required]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonViQuiDoi { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }

        public virtual The_TheKhachHang The_TheKhachHang { get; set; }
    }
}
