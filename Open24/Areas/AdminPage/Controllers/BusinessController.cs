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
    public class BusinessController : BaseController
    {
        // GET: AdminPage/Business
        public ActionResult Index()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.BUSINESS_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.BUSINESS_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.BUSINESS_DELETE)
            };
            return View(checkRoleView);
        }


    }
}