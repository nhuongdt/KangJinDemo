using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_KhenThuong")]
    public partial class NS_KhenThuong
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoaiKhenThuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KyTinhCong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string HinhThuc { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SoQuyetDinh { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayBanHang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDung { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "float")]
        public double? SoTien { get; set; } = 0;

        public virtual NS_NhanVien NS_NhanVien { get; set; }
        
        public virtual NS_LoaiKhenThuong NS_LoaiKhenThuong { get; set; }

        public virtual NS_KyTinhCong NS_KyTinhCong { get; set; }
    }
}
