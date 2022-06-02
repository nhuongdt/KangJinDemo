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
    public class PhongBanController : Controller
    {
        [RBACAuthorize(RoleKey = RoleKey.PhongBan_XemDs)]
        public ActionResult Index()
        {
            string apiUri = Url.HttpRouteUrl("DefaultApi", new { controller = "PhongBan", });
            ViewBag.ApiUrl = new Uri(Request.Url, apiUri).AbsoluteUri.ToString();
            return View();
        }
        [RBACAuthorize(RoleKey = RoleKey.PhongBan_XemDs)]
        public ActionResult PhongBan()
        {
            string apiUri = Url.HttpRouteUrl("DefaultApi", new { controller = "PhongBan", });
            ViewBag.ApiUrl = new Uri(Request.Url, apiUri).AbsoluteUri.ToString();
            return View();
        }

        public ActionResult _themmoicapnhatvitri(string id)
        {
            return PartialView();
        }
        public ActionResult _themmoicapnhatkhuvuc(string id)
        {
            return PartialView();
        }
        public ActionResult _editkhuvuc(string id)
        {
            return PartialView();
        }
    }
}
