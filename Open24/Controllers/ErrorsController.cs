using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Open24.Controllers
{
    public class ErrorsController : BaseController
    {
        // GET: Errors
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error404()
        {
            ActionResult result;
            object model = Request.Url.PathAndQuery;
            var text = Request.Url.PathAndQuery.Split('/')[1];
            ViewBag.Title = "Không tìm thấy trang có từ khóa "+ text;
            if (!Request.IsAjaxRequest())
                result = View(model);
            else
                //result = PartialView("_NotFound", model);
                result = View(model);

            return result;
        }
      
    }
}