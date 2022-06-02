using SoftWareSsoft.Areas.Admin.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Areas.Admin.Controllers
{
    [RBACAuthorize]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult CandidateApplyFile()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return View();
        }
    }
}