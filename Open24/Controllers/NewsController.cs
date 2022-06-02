using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView.Client;
using Model_banhang24vn.DAL;
using Open24.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Open24.Controllers
{

    public class NewsController : Controller
    {
        //index news
        private News_UserService _News_UserService = new News_UserService();

        [ViewAuthorize]
        public ActionResult tintuc()
        {
            var menuTag = new MenuTagsService().GetByAction(Request.RequestContext.RouteData.Values["controller"].ToString(),
                                                        Request.RequestContext.RouteData.Values["action"].ToString());
            if (menuTag != null)
            {
                ViewBag.MetaDescription = menuTag.Description;
                ViewBag.MetaKeywords = menuTag.Tags +","+ StaticVariable.RemoveSign4VietnameseString(menuTag.Tags); ;
            }
            return View();
        } 

        [ViewAuthorize(Title = "/tin-tuc/chi-tiet")]
        public ActionResult detailnews(string title, long? keyId)
        {
            if (keyId == null)
            {
                return tintuc();
            }
            var _M_News_Post = new M_News_Post();
            var model = new newsDetailView();
            model.NewsModel = _M_News_Post.GetArticleNewsDetailUpdateView(title, keyId ?? -1);
            var user = _News_UserService.Getbykey(model.NewsModel.CreatedBy ?? new Guid());
            if (user != null)
            {
                model.CreateByUser = user.Name;
            }
            if (model.NewsModel == null)
            {
                return tintuc();
            }
            model.ListRlatedArticles = _M_News_Post.GetListRlatedArticles(model.NewsModel.CategoryID ?? -1, model.NewsModel.Tag, model.NewsModel.ID);
            var tag = new NewPostService().GetTagsByArticle(keyId ?? 0);
            ViewBag.Tags = tag;
            var menuTag = new MenuTagsService().GetByAction(Request.RequestContext.RouteData.Values["controller"].ToString(), "tintuc");
            if (tag.Any())
            {
                setkeywords(model.NewsModel.Summary, string.Format("{0}, {1}, {2}",
                    menuTag!=null? menuTag.Tags + "," + StaticVariable.RemoveSign4VietnameseString(menuTag.Tags) : Notification.TagsTinTuc, 
                    string.Join(",", tag.Select(o => o.Name)), 
                    string.Join(",", tag.Select(o => o.KeyWords))));
            }
            else
            {
                setkeywords(model.NewsModel.Summary, menuTag != null ? menuTag.Tags + "," + StaticVariable.RemoveSign4VietnameseString(menuTag.Tags) : Notification.TagsTinTuc);
            }
                return View(model);
        }

        [ViewAuthorize]
        public ActionResult Tags(string tagId)
        {
            var tag = new NewPostService().GetTagById(tagId);
            var menuTag = new MenuTagsService().GetByAction(Request.RequestContext.RouteData.Values["controller"].ToString(), "tintuc");
            if (tag != null)
            {
                var keywork = (menuTag != null ? menuTag.Tags + "," + StaticVariable.RemoveSign4VietnameseString(menuTag.Tags) : string.Empty) + "," + tag.Name + "," + tag.KeyWords;
                var Description = (menuTag != null ? menuTag.Description  : string.Empty) + "," + tag.Name + "," + tag.KeyWords;
                setkeywords(Description, keywork);
                return View(tag);
            }
            return tintuc();
        }

        [ViewAuthorize(Title = "/tin-tuc/nhom-bai-viet")]
        public ActionResult GroupNews(int? CategoryId)
        {
            var category = new GroupPostService().Query.FirstOrDefault(o => o.ID == CategoryId);
            if (category == null) return tintuc();
            setkeywords(category.Description,string.Format("{0},{1},{2}", Notification.TagsTinTuc, category.Name, StaticVariable.RemoveSign4VietnameseString(category.Name)));
            ViewBag.CategoryId = CategoryId;
            return View();
        }

        private void setkeywords(string description, string keywords)
        {
            ViewBag.MetaDescription = description;
            ViewBag.MetaKeywords = keywords;
        }
    }

}
