using log4net;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView.Client;
using Model_banhang24vn.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
 
namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiPostController : ApiBaseController
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly NewPostService _NewPostService;
        private readonly GroupPostService _GroupPostService;
        public ApiPostController()
        {
            _NewPostService = new NewPostService();
            _GroupPostService = new GroupPostService();
        }

        /// <summary>
        /// Lấy danh sách client các bài 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetDetail(int? page)
        {
            try
            {

                var data = _NewPostService.Query.OrderByDescending(o=>o.Gender).ThenByDescending(o=>o.CreateDate).Where(o=>o.Status==true).Skip(((page??1)-1)*Notification.PageDefault)
                                            .Take(Notification.PageDefault)
                                                                .Select(o => new
                                                                {
                                                                    Id = o.ID,
                                                                    Title = o.Title,
                                                                    Image = o.UrlImage,
                                                                    Url = o.Url,
                                                                    Summary = o.Summary,
                                                                    CreatDate = o.CreateDate
                                                                }).AsEnumerable();

               
                return RetunJsonAction(true, string.Empty, 
                    new {
                        Data =data,
                        countPage = (int)Math.Ceiling((double)_NewPostService.Count/ Notification.PageDefault)
                    });
                
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        [HttpGet]
        public IHttpActionResult Getpage(int? page)
        {
            try
            {

                var data = _NewPostService.Query.OrderByDescending(o => o.Gender).ThenByDescending(o => o.CreateDate).Where(o => o.Status == true).Skip((page ?? 1) * Notification.PageDefault)
                                            .Take(Notification.PageDefault)
                                                                .Select(o => new
                                                                {
                                                                    Id = o.ID,
                                                                    Title = o.Title,
                                                                    Image = o.UrlImage,
                                                                    Url = o.Url,
                                                                    Summary = o.Summary,
                                                                    CreatDate = o.CreateDate
                                                                }).AsEnumerable();


                return RetunJsonAction(true, string.Empty,
                    new
                    {
                        Data = data,
                        iShow = data.Count()== Notification.PageDefault
                    });

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpGet]
        public IHttpActionResult GetHome()
        {
            try
            {

                var data = _NewPostService.Query.OrderByDescending(o => o.Gender)
                                            .ThenByDescending(o => o.CreateDate)
                                            .Where(o => o.Status == true)
                                            .Take(4)
                                            .Select(o => new
                                            {
                                                                    Id = o.ID,
                                                                    Title = o.Title,
                                                                    Image = o.UrlImage,
                                                                    Url = o.Url,
                                                                    Summary = o.Summary,
                                                                    CreatDate = o.CreateDate
                                             }).AsEnumerable();


                return RetunJsonAction(true, string.Empty,data);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Load dữ liệu bài viết theo tags
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IHttpActionResult GetArticleByTag(string tagId, int? page)
        {
            try
            {

                var data = _NewPostService.GetArticleByTag(tagId).OrderByDescending(o=>o.CreateDate).Skip((page ?? 0) * Notification.PageDefault)
                                            .Take(Notification.PageDefault)
                                                                .Select(o => new
                                                                {
                                                                    Id = o.ID,
                                                                    Title = o.Title,
                                                                    Image = o.UrlImage,
                                                                    Url = o.Url,
                                                                    Summary = o.Summary,
                                                                    CreatDate = o.CreateDate
                                                                }).AsEnumerable();


                return RetunJsonAction(true, string.Empty,
                    new
                    {
                        Data = data,
                        iShow = data.Count() == Notification.PageDefault
                    });

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// lấy danh sách client các bài viết theo nhóm
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCategory(int? categoryId, int? page)
        {
            try
            {
                var data = _NewPostService.GetCategory(categoryId).Skip((page ?? 0) * Notification.PageDefault)
                                           .Take(Notification.PageDefault)
                                                               .Select(o => new NewsPageView
                                                               {
                                                                   ID = o.ID,
                                                                   Describe = o.Summary,
                                                                   Image = o.UrlImage,
                                                                   Title = o.Title,
                                                                   Url = o.Url,
                                                                   CreatDate = o.CreateDate
                                                               }).AsEnumerable();
                var category = _GroupPostService.Query.FirstOrDefault(o => o.ID == categoryId);
                return RetunJsonAction(true, string.Empty,
                    new
                    {
                        Data = data,
                        category = category != null ? category.Name : string.Empty,
                        iShow = data.Count() == Notification.PageDefault
                    });
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Lấy danh sách các bài viết mới nhất
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetNewDate()
        {
            try
            {
                if (!CacheHellper.IsSet(CacheKey.News_NewDate))
                {
                    var data = _NewPostService.Query.Where(o=>o.Status==true)
                                           .Take(Notification.PageClientDefault)
                                                               .Select(o => new NewsPageView
                                                               {
                                                                   ID = o.ID,
                                                                   Describe = o.Summary,
                                                                   Image = o.UrlImage,
                                                                   Title = o.Title,
                                                                   Url = o.Url,
                                                                   CreatDate = o.CreateDate
                                                               }).AsEnumerable();
                        CacheHellper.Set(CacheKey.News_NewDate, data);
                    return RetunJsonAction(true, string.Empty, data);
                }
                else
                    return RetunJsonAction(true, string.Empty, CacheHellper.Get(CacheKey.News_NewDate));
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Lấy danh sách các bài viết xem nhiều nhất
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetNewsView()
        {
            try
            {
                if (!CacheHellper.IsSet(CacheKey.News_ViewCount))
                {
                    var data = _NewPostService.Query.Where(o => o.Status == true).OrderByDescending(o=>o.ViewCount)
                                           .Take(Notification.PageClientDefault)
                                                               .Select(o => new 
                                                               {
                                                                   ID = o.ID,
                                                                   Describe = o.Summary,
                                                                   Image = o.UrlImage,
                                                                   Title = o.Title,
                                                                   Url = o.Url,
                                                                   CreatDate = o.CreateDate,
                                                                   View=o.ViewCount
                                                               }).AsEnumerable();
                    CacheHellper.Set(CacheKey.News_ViewCount, data);
                    return RetunJsonAction(true, string.Empty, data);
                }
                else
                    return RetunJsonAction(true, string.Empty, CacheHellper.Get(CacheKey.News_ViewCount));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

    }
}
