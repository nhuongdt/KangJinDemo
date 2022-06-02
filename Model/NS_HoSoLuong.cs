using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_HoSoLuong")]
    public partial class NS_HoSoLuong
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayBD { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayKT { get; set; }

        [Column(TypeName = "float")]
        public double MucLuongCB { get; set; }

        [Column(TypeName = "nvarchar(max)")]
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

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LuongDoanhThu { get; set; }

        [Column(TypeName = "bit")]
        public bool? LuongTuVan { get; set; } = false;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual NS_LuongDoanhThu NS_LuongDoanhThu { get; set; }
    }
}
