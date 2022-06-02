using Model.Web.Service;
using SoftWareSsoft.Areas.Admin.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Areas.Admin.Controllers
{
    [RBACAuthorize]
    public class NewsController : Controller
    {
        private NewsService _NewsService = new NewsService();
        private RecruitmentService _RecruitmentService = new RecruitmentService();
        // GET: Admin/News
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult News()
        {
            return View();
        }
        public ActionResult EditNews(int? id)
        {
            ViewBag.KeyId = id;
            ViewBag.Tags= string.Join(",", _NewsService.GetTagsNews(id??0).Where(o => o.DM_Tags != null).AsEnumerable().Select(o => o.DM_Tags.TenTheTag).ToList());
            return View();
        }


        public ActionResult Recruitment()
        {
            return View();
        }
        public ActionResult EditRecruitment(int? id)
        {
            ViewBag.KeyId = id;
            var tag = _RecruitmentService.GetTagsRecruitment(id ?? 0).Where(o => o.DM_Tags != null).Select(o => o.DM_Tags.TenTheTag).ToList();
            ViewBag.Tags = string.Join(",", tag);
            return View();
        }
    }
}