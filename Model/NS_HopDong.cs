using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_HopDong")]
    public partial class NS_HopDong
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string SoHopDong { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int LoaiHopDong { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayKy { get; set; }

        [Column(TypeName = "int")]
        public int ThoiHan { get; set; }

        [Column(TypeName = "int")]
        public int DonViThoiHan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
