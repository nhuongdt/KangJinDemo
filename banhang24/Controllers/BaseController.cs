using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace banhang24.Controllers
{
    public class BaseController : Controller
    {
        public ActionResult ResultPostTrue(string mess,object model)
        {
            return Json(new { res = true,mess=mess, data = model });
        }

        public ActionResult ResultPostFalse(string mess,string log, object model)
        {
            return Json(new { res = false, mess = mess,exceptions=log, data = model });
        }

        public ActionResult ResultGetTrue( object model)
        {
            return Json(new { res = true, data = model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResultGetFalse(string log, object model)
        {
            return Json(new { res = false, exceptions = log, data = model }, JsonRequestBehavior.AllowGet);
        }
    }
}
