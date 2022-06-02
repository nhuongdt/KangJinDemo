using SoftWareSsoft.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Web;
using Model.Web.API;
using Model.Web.DAL;


namespace SoftWareSsoft.Controllers
{
    [ViewAuthorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var subdomain = SystemDBContext.GetStrSubDomain();
            ViewBag.Subdomain = subdomain;
            if (subdomain != null && subdomain.Trim() != "")
            {
                if (ConnectionStringSystem.CreateConnectionString(subdomain) == "")
                {
                    SystemDBContext.MigrationDatabase(subdomain);
                    
                    return View();
                }
            }
            return Redirect("http://open24.vn");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

     
    }
}