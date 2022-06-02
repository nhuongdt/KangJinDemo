using SoftWareSsoft.Areas.Admin.Hellper;
using SoftWareSsoft.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            ContantaAdmin.ClearSessionWorkUser();
            return View();
        }
    }
}