using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn
{
    [MetadataTypeAttribute(typeof(CuaHangDangKyMetadata))]
    public partial class CuaHangDangKy
    {
        internal sealed class CuaHangDangKyMetadata
        {
            [Required(ErrorMessage = "Không để trống 'Số điện thoại'")]
            [Display(Name = "Điện thoại")]
            public string SoDienThoai { get; set; }

            [Required(ErrorMessage = "Không để trống 'Địa chỉ Website muốn đăng ký'")]
            //[Display(Name = "Điện thoại")]
            public string SubDomain { get; set; }

            [Required(ErrorMessage = "Không để trống 'Tên cửa hàng'")]
            [Display(Name = "Tên cửa hàng")]
            public string TenCuaHang { get; set; }

            [Display(Name = "Địa chỉ")]
            public string DiaChi { get; set; }

            [Required(ErrorMessage = "Không để trống 'Email'")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Không để trống 'Họ tên'")]
            [Display(Name = "Họ tên")]
            public string HoTen { get; set; }

            [Display(Name = "Ngành nghề kinh doanh")]
            public Guid ID_NganhKinhDoanh { get; set; }

            [Display(Name = "Ngày đăng ký")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public Nullable<System.DateTime> NgayTao { get; set; }

            [Required(ErrorMessage = "Không để trống 'Tên tài khoản'")]
            [Display(Name = "Họ tên")]
            public string UserKT { get; set; }

            [Required(ErrorMessage = "Không để trống 'Mật khẩu'")]
            [Display(Name = "Họ tên")]
            public string MatKhauKT { get; set; }
        }
    }
}
