using banhang24.Hellper;
using libHT_NguoiDung;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace banhang24.Controllers
{
    [App_Start.App_API.CheckwebAuthorize]
    public class BaoCaoController : Controller
    {
        [RBACAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult CuoiNgay()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult BanHang()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult HangHoa()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult Nhanvien()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult TaiChinh()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult KhachHang()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult NhaCungCap()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult DatHang()
        {
            return View();

        }
    }
}
