using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using banhang24.Areas.DanhMuc.Controllers;
using Model;
using libHT_NguoiDung;
using banhang24.Hellper;

namespace banhang24.Controllers
{
    [App_Start.App_API.CheckwebAuthorize]
    public class TuVanController : Controller
    {
        [RBACAuthorize]
        public ActionResult Index()
        {

            return View();
        }
        [RBACAuthorize(RoleKey = RoleKey.TuVan_XemDs)]
        public ActionResult TuVan()
        {
            return View();

        }

        public ActionResult _themmoituvan(string id)
        {
            return PartialView();
        }

        public ActionResult _themmoi_loaituvan()
        {
            return PartialView();
        }
        public ActionResult _themmoi_loaiphanhoi()
        {
            return PartialView();
        }

        public ActionResult _themmoi_loailichhen()
        {
            return PartialView();
        }

        public ActionResult _themmoilichhen()
        {
            return PartialView();
        }

        public ActionResult _themmoiphanhoi()
        {
            return PartialView();
        }
        [RBACAuthorize(RoleKey = RoleKey.PhanHoi_XemDs)]
        public ActionResult PhanHoi()
        {
            return View();

        }
        [RBACAuthorize(RoleKey = RoleKey.LichHen_XemDs)]
        public ActionResult LichHen()
        {
            return View();
        }

        public ActionResult NguoiLienHe()
        {
            return View();
        }
        public ActionResult CongViec()
        {
            return View();
        }

        public ActionResult DanhSachSMS()
        {
            return View();
        }

        public JsonResult GetListLichHen(string id)
        {
            //string [] id_new = id.Split('&'); 
            Guid id_donvi = new Guid(id.Substring(0, id.IndexOf('&')));
            string url = Request.RawUrl;
            string sfrom = url.Substring(url.IndexOf("=") + 1, 13);
            string sto = url.Substring(url.IndexOf("=", url.IndexOf("=") + 14) + 1, 13);
            try
            {
                double ifrom = 0, ito = 0;
                ifrom = double.Parse(sfrom);
                ito = double.Parse(sto);
                DateTime tempFrom = App_API.CvtDatetimetoTimestamps.TimestampToDatetime(ifrom);
                DateTime tempTo = App_API.CvtDatetimetoTimestamps.TimestampToDatetime(ito);
                List<DTStamp> evt = ChamSocKhachHangAPIController.GetListCalendar(tempFrom, tempTo, id_donvi);
                return Json(new { success = 1, result = evt }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string str = CookieStore.GetCookieAes("SubDomain");
                CookieStore.WriteLog("GetListLichHen(string id): " + ex.InnerException + ex.Message, str);
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Datlich()
        {
            return View();
        }
        public JsonResult GetListLichHen_FromTo(DateTime daySart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<DTStamp> evt = ChamSocKhachHangAPIController.GetListCalendar(daySart, dayEnd, IDchinhanh);
            return Json(new { success = 1, result = evt }, JsonRequestBehavior.AllowGet);
        }
    }
}
