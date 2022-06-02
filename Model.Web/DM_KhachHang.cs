using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_KhachHang")]
    public partial class DM_KhachHang
    {
        [Key]
        public int ID { get; set; }
        public string TenKhachHang { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string MaTinhThanh { get; set; }
        public string ID_SanPham { get; set; }
        public string Mota { get; set; }
        public string NoiDung { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public int? TrangThai { get; set; }
        public string Anh { get; set; }
        public string Link { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        public int SoLuotXem { get; set; }
        public bool HienThiTrangChu { get; set; }
    }
}
