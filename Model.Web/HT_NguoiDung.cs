using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("HT_NguoiDung")]
    public partial class HT_NguoiDung
    {
        [Key]
        public Guid ID { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string TenNguoiDung { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public bool LaAdmin { get; set; }
        public DateTime? SinhNhat { get; set; }
        public Guid ID_NhomNguoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }

        public virtual HT_NhomNguoiDung HT_NhomNguoiDung { get; set; }
    }
}
