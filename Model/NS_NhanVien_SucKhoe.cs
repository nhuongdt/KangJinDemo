using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_NhanVien_SucKhoe")]
    public partial class NS_NhanVien_SucKhoe
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "float")]
        public double ChieuCao { get; set; }

        [Column(TypeName = "float")]
        public double CanNang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TinhHinhSucKhoe { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayKham { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
