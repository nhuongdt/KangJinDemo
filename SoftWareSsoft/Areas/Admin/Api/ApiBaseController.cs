using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ssoft.Common.Cache;
using Ssoft.Common.Common;
using Model.Web.Service;
using System.Web;
using System.IO;
using SoftWareSsoft.Hellper;
using Model.Web.API;

namespace SoftWareSsoft.Areas.Admin.Api
{
    public class ApiBaseController : ApiController
    {
        public TinhThanhService _TinhThanhService = new TinhThanhService();
        public ICacheHelper CacheHellper { get; set; }
        public ApiBaseController() : this(new CacheHelper()) { }

        public ApiBaseController(ICacheHelper cacheHelper)
        {
            // TODO: Complete member initialization
            this.CacheHellper = cacheHelper;
        }
        public IHttpActionResult ActionTrueData<T>( T data)
        {
            return Json(new { res = true, dataSoure = data });
        }
        public IHttpActionResult ActionTrueNotData(string resultMess)
        {
            return Json(new { res = true, mess = resultMess });
        }
        public IHttpActionResult ActionFalseNotData(string resultMess)
        {
            return Json(new { res = false, mess = resultMess });
        }
        public IHttpActionResult Exception(Exception ex)
        {
            return Json(new { res = false, mess = ex.Message });
        }
        public IHttpActionResult Exception()
        {
            return Json(new { res = false, mess = "Đã xảy ra lỗi, vui lòng thử lại sau" });
        }
        public IHttpActionResult InsertSuccess()
        {
            return Json(new { res = true, mess = LibNotification.Messager_InsertSuccess });
        }
        public IHttpActionResult UpdateSuccess()
        {
            return Json(new { res = true, mess = LibNotification.Messager_UpdateSuccess });
        }
        public IHttpActionResult DeleteSuccess()
        {
            return Json(new { res = true, mess = LibNotification.Messager_DeleteSuccess });
        }
        [HttpGet]
        public IHttpActionResult GetAllTinhThanh()
        {
            var result = _TinhThanhService.GetAll().Select(o => new
            {
                Key = o.MaTinhThanh,
                Value = o.TenTinhThanh,
                IsSelect = false
            }).AsEnumerable();
            return  Json( result);
           
        }
        /// <summary>
        /// upload ảnh đối tác
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UploadImages()
        {
            try
            {
                var path = "";
                string result = "";
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];
                    var filenameImage = Guid.NewGuid().ToString() + ".jpg";
                    var subdomain = CookieStore.GetCookieAes(SqlConnection.subdoamin);
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/SubDomainImages/"+ subdomain )))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/SubDomainImages/" + subdomain));
                    }

                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/SubDomainImages/" + subdomain), filenameImage);

                    file.SaveAs(path);
                    result = "/SubDomainImages/" + subdomain +"/" + filenameImage;
                }
                return ActionTrueData( result);
            }
            catch (Exception ex)
            {
                return Exception(ex);
            }
        }
        
    }
}
