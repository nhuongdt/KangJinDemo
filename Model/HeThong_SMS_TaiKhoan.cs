using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HeThong_SMS_TaiKhoan")]
    public partial class HeThong_SMS_TaiKhoan
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NguoiChuyenTien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NguoiNhanTien { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ThoiGian { get; set; }

        [Column(TypeName = "float")]
        public double SoTien { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1;

        public virtual HT_NguoiDung HT_NguoiDungChuyen { get; set; }

        public virtual HT_NguoiDung HT_NguoiDungNhan { get; set; }
    }
}
