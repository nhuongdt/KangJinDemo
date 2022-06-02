using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace banhang24.Controllers
{
    public class MarketingController : Controller
    {
        public ActionResult OptinForm()
        {
            return View();
        }
        public ActionResult AddOptinFormKH()
        {
            return View();
        }

        public ActionResult AddOptinFormLH()
        {
            return View();
        }

        public ActionResult DanhSachDangKyOptinForm()
        {
            return View();

        }
        public ActionResult CongViec()
        {
            return View();

        }
        public ActionResult LichHen()
        {
            return View();

        }
        public ActionResult Calendar()
        {
            return View();

        }

        public ActionResult DatLich()
        {
            return View();
        }

        public ActionResult ErrorOptinform()
        {
            return View();

        }
        public ActionResult GetLinkFormCustomer1()
        {
            return Json("avv0",JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLinkFormCustomer(string sub,string utm_user,string id_optin)
        {
            ViewBag.Subdomain = sub;
            ViewBag.utm_user = utm_user;
            ViewBag.ID_OptinForm = id_optin;
            //return RedirectToAction("ErrorOptinform");
            return View();
        }
        public ActionResult GetLinkFormschedule(string sub, string utm_user, string id_optin)
        {
            ViewBag.Subdomain = sub;
            ViewBag.utm_user = utm_user;
            ViewBag.ID_OptinForm = id_optin;
            return View();
        }
    }
}
