using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChamSocKhachHang_NhanVien")]
    public partial class ChamSocKhachHang_NhanVien
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ChamSocKhachHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        public virtual ChamSocKhachHang ChamSocKhachHang { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
