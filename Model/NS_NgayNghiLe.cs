using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_NgayNghiLe")]
    public class NS_NgayNghiLe
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        //[Column(TypeName = "uniqueidentifier")]
        //public Guid? ID_KyTinhCong { get; set; }

        //[Column(TypeName = "uniqueidentifier")]
        //public Guid? ID_BangLuong { get; set; }

        [Column(TypeName = "int")]
        public int Thu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Ngay { get; set; }

        [Column(TypeName = "int")]
        public int LoaiNgay { get; set; }

        [Column(TypeName = "nvarchar")]
        public string MoTa { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }// 0.Xoa, 1.ApDung, 2.Huy

        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }
        //public virtual NS_BangLuong NS_BangLuong { get; set; }
        //public virtual NS_KyTinhCong NS_KyTinhCong { get; set; }
    }
}
