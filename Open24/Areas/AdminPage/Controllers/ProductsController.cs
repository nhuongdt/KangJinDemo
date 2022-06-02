using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Open24.Areas.AdminPage.Controllers
{
    public class ProductsController : Controller
    {
        // GET: AdminPage/Products
        public ActionResult Index()
        {
            return View();
        }
        [RBACAuthorize(RoleKey = StaticRole.SALESDEVICES_VIEW)]
        public ActionResult SalesDevice()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.SALESDEVICES_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.SALESDEVICES_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.SALESDEVICES_INSERT)
            };
            return View(checkRoleView);
        }
        [RBACAuthorize(RoleKey = StaticRole.ORDER_VIEW)]
        public ActionResult Order()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.ORDER_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.ORDER_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.ORDER_DELETE)
            };
            return View(checkRoleView);
        }
    }
}