using SoftWareSsoft.Areas.Admin.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Areas.Admin.Controllers
{
    [RBACAuthorize]
    public class UserController : Controller
    {
        // GET: Admin/User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult User()
        {
            return View();
        }
    }
}