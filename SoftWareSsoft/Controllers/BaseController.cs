using Model.Web.API;
using Model.Web.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Controllers
{
    public class BaseController : Controller
    {
        protected override void HandleUnknownAction(string actionName)
        {
            if (Request.Path == "/robots.txt")
            {
                RedirectToAction("RobotsText").ExecuteResult(ControllerContext);
            }
            else
            {
                base.HandleUnknownAction(actionName);
            }
        }
        public DM_MenuService _DM_MenuService;
        public NewsService _NewsService;
        public CustomerService _CustomerService;
        public RecruitmentService _RecruitmentService;
        public BaseController()
        {
            _DM_MenuService = new DM_MenuService();
            _NewsService = new NewsService();
            _CustomerService = new CustomerService();
            _RecruitmentService = new RecruitmentService();
        }

        public void SetKeyWorkMeta()
        {
            var menutag = _DM_MenuService.GetMetaTags(Request.Url.AbsoluteUri);
            if (menutag != null)
            {
                SetMetaSeo(menutag.Title, menutag.Description, menutag.KeyWord);
            }

        }
        public void SetMetaSeo(string title, string metadescription,string keyword)
        {
            ViewBag.MetaDescription = metadescription;
            ViewBag.Title = title;
            ViewBag.MetaKeywords = keyword;
        }
    }
}