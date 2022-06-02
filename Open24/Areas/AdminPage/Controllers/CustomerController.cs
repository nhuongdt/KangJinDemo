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
    [RBACAuthorize]
    public class CustomerController : Controller
    {
        // GET: AdminPage/Customer
        [RBACAuthorize(RoleKey = StaticRole.KHACHHANG_VIEW)]
        public ActionResult Index()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.KHACHHANG_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.KHACHHANG_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.KHACHHANG_DELETE)
            };
            return View(checkRoleView);
        }

        public ActionResult Create(int? Id)
        {
            ViewBag.CustomerId = Id;
            return View();
        }
    }
}