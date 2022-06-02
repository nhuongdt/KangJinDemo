using Model_banhang24vn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Open24.Models
{
    public class CuaHangDangkyModel
    {
        public string SoDienThoai { get; set; }
        public string SubDomain { get; set; }
        public string TenCuaHang { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public string MaNganhKinhDoanh { get; set; }
        public string UserKT { get; set; }
        public string MatKhauKT { get; set; }
        public string KhuVuc_DK { get; set; }
        public string DiaChiIP_DK { get; set; }
        public Guid? ID_NganhKinhDoanh { get; set; }
        public string DienThoaiNhanVien { get; set; }
        public string TenNhanVien { get; set; }
    }
    public class CustomerContactsale:Contact
    {
        public string StatusNews { get; set; }
        public string TypeSoftWareNews { get; set; }
        public string Website { get; set; }
        public int? TypeContact { get; set; }
    }
}