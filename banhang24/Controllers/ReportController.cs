
using banhang24.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace banhang24.Controllers
{
    [RBACAuthorize]
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BanHang()
        {
            return View();
        }

        public ActionResult nhaphang()
        {
            return View();
        }
        public ActionResult taichinh()
        {
            return View();
        }

        public ActionResult taichinh_v2()
        {
            return View();
        }
        public ActionResult DatHang()
        {
            return View();
        }
        public ActionResult Kho()
        {
            return View();
        }
        public ActionResult chietkhaunhanvien()
        {
            return View();
        }

        public ActionResult GoiDichVu()
        {

            return View();
        }
        public ActionResult NhanVien()
        {

            return View();
        }

        public ActionResult TheGiaTri()
        {

            return View();
        }
        public ActionResult HoaHong()
        {

            return View();
        } 
        public ActionResult BaoCaoHoatDongXe()
        {

            return View();
        }
    }
}
