using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("HT_ThongTinCuaHang")]
    public partial class HT_ThongTinCuaHang
    {
        [Key]
        public Guid ID { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public int? PageTinTucHome { get; set; }
        public int? PageSanPhamHome { get; set; }
        public int? PageTinTuc { get; set; }
        public int? PageKhachHangHome { get; set; }
        public int? PageKhachHang { get; set; }
        public string DiaChi { get; set; }
        public int? Theme { get; set; }
        public string AnhLogo { get; set; }
        public string ApiMap { get; set; }
        public string LinkPageFacebook { get; set; }
        public string TenCuaHang { get; set; }
        public string DomainOpen { get; set; }
        public string APIKey { get; set; }
    }
}
