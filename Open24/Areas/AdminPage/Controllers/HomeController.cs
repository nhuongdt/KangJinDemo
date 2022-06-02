using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model_banhang24vn;
using Open24.Areas.AdminPage.Hellper;
using System.IO;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Common;

namespace Open24.Areas.AdminPage.Controllers
{
    [RBACAuthorize]
    public class HomeController : Controller
    {
        // GET: AdminPage/Home
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult MenuTags()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleUpdate = contant.CheckRole(StaticRole.ADVERTISEMENT_UPDATE),
            };
            return View(checkRoleView);
        }
        public ActionResult HoaDon()
        {
            return View();
        }

        public ActionResult LoadValueCkeditor()
        {
            var rawURL = Request.Url.Host;

            string defaultFolder = Server.MapPath("~/Assets");
            string folderCus = "";
            if (rawURL != "localhost")
            {
                folderCus = Server.MapPath("~/Assets/" + rawURL + "/");
            }
            // combine 2 string --> full path
            string targetPath = Path.Combine(folderCus, "DatHang.txt");
            string result = "";
            if (System.IO.File.Exists(targetPath))
            {
                result = System.IO.File.ReadAllText(targetPath);
            }
            else
            {
                targetPath = Path.Combine(defaultFolder, "DatHang.txt");
                result = System.IO.File.ReadAllText(targetPath);
            }
            return  Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            return View();
        }
        [RBACAuthorize(RoleKey = StaticRole.GOIDICHVU_VIEW)]
        public ActionResult GoiDichVu()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.GOIDICHVU_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.GOIDICHVU_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.GOIDICHVU_DELETE),
            };
            return View(checkRoleView);
        }
        [RBACAuthorize(RoleKey = StaticRole.CUSTOMERCONTACTSALE_VIEW)]
        public ActionResult CustomerContactSales()
        {
            return View();
        }

        public ActionResult Contract()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleUpdate = contant.CheckRole(StaticRole.INTRODUCECUSTOMER_UPDATE),
            };
            return View(checkRoleView);
        }

        [RBACAuthorize(RoleKey = StaticRole.ADVERTISEMENT_VIEW)]
        public ActionResult Advertising()
        {

            var checkRoleView = new UserRoleView()
            {
                RoleUpdate = contant.CheckRole(StaticRole.ADVERTISEMENT_UPDATE),
            };
            return View(checkRoleView);
        }
        [RBACAuthorize(RoleKey = StaticRole.NOTIFICATIONSOFWARE_VIEW)]
        public ActionResult NotificationSoftware()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = true,
                RoleUpdate = true,
                RoleDelete = true
            };
            return View(checkRoleView);
        }

        public ActionResult EditNotificationSoftware(long? id)
        {
            ViewBag.KeyNotificationID = id;
            return View();
        }

        [RBACAuthorize(RoleKey = StaticRole.DICHVUSMS_VIEW)]
        public ActionResult RegisterServiceSms()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleUpdate = contant.CheckRole(StaticRole.DICHVUSMS_UPDATE),
            };
            return View(checkRoleView);
        }

        [RBACAuthorize(RoleKey = StaticRole.NAPTIEN_VIEW)]
        public ActionResult RechargeService()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleUpdate = contant.CheckRole(StaticRole.NAPTIEN_UPDATE),
                RoleInsert = contant.CheckRole(StaticRole.NAPTIEN_INSERT),
                RoleDelete = contant.CheckRole(StaticRole.NAPTIEN_DELETE),
            };
            return View(checkRoleView);
        }

        [RBACAuthorize(RoleKey = StaticRole.SEOURL_INSERT)]
        public ActionResult SeoUrl()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleUpdate = contant.CheckRole(StaticRole.SEOURL_INSERT),
                RoleInsert = contant.CheckRole(StaticRole.SEOURL_INSERT),
                RoleDelete = contant.CheckRole(StaticRole.SEOURL_INSERT),
            };
            return View(checkRoleView);
        }
    }
}