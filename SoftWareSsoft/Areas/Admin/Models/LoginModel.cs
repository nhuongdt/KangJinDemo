using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftWareSsoft.Areas.Admin.Models
{
    public class LoginModel
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public bool GhiNho { get; set; }
    }
}