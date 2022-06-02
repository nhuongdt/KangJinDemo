using Model_banhang24vn.Common;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
 
namespace Open24.Areas.AdminPage.Controllers
{
    [RBACAuthorize]
    public class AppChatController : Controller
    {
        // GET: AdminPage/AppChat
        public ActionResult Index()
        {
            return View();

        }
    
    }
}