using Model_banhang24vn.Common;
using Model_banhang24vn.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Open24.Controllers
{
    public class BaseController : Controller
    {
        private MenuTagsService _MenuTagsService = new MenuTagsService();
        public void setkeywords(string description, string keywords)
        {
            ViewBag.MetaDescription = description;
            ViewBag.MetaKeywords = keywords;
            SetCanonical();
        }
        public void setkeywords()
        {
            var menuTag = _MenuTagsService.GetByAction(Request.RequestContext.RouteData.Values["controller"].ToString(),
                                                        Request.RequestContext.RouteData.Values["action"].ToString());
            if (menuTag != null)
            {
                ViewBag.MetaDescription = menuTag.Description;
                ViewBag.MetaKeywords = menuTag.Tags + "," + StaticVariable.RemoveSign4VietnameseString(menuTag.Tags);
            }
            SetCanonical();
        }
        public void setkeywordsTitle()
        {
            var menutag = _MenuTagsService.GetMetaTags(Request.Url.AbsoluteUri);
            SetCanonical();
            if (menutag != null)
            {
                SetMetaSeo(menutag.Title, menutag.Description, menutag.Tags);
            }
        }
        public void SetCanonical()
        {
            ViewBag.UrlPage = Request.Url.AbsoluteUri;
        }
        public void SetMetaSeo(string title, string metadescription, string keyword)
        {
            ViewBag.MetaDescription = metadescription;
            ViewBag.Title = title;
            ViewBag.MetaKeywords = keyword;
        }
    }
}