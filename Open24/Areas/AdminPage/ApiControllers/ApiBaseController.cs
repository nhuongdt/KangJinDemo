using log4net;
using Model_banhang24vn.Cache;
using Model_banhang24vn.Common;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
 
namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiBaseController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ICacheHelper CacheHellper { get; set; }
        public ApiBaseController() : this(new CacheHelper()) { }

        public ApiBaseController(ICacheHelper cacheHelper)
        {
            // TODO: Complete member initialization
            this.CacheHellper = cacheHelper;
        }
        public IHttpActionResult RetunJsonAction<T>(bool resultRes, string resultMess, T data)
        {
            return Json(new { res = resultRes, mess = resultMess, DataSoure = data });
        }
        public IHttpActionResult ActionTrueNotData(string resultMess)
        {
            return Json(new { res = true, mess = resultMess});
        }
        public IHttpActionResult ActionTrueWithData<T>(T data)
        {
            return Json(new { res = true, DataSource = data });
        }
        public IHttpActionResult ActionFalseNotData(string resultMess)
        {
            return Json(new { res = false, mess = resultMess });
        }
        public IHttpActionResult Exeption()
        {
            return Json(new { res = false, mess = Notification.Messager_Exception });
        }
        public IHttpActionResult Exeption(Exception ex)
        {
            return Json(new { res = false, mess = ex.Message });
        }
        public IHttpActionResult InsertSuccess()
        {
            return Json(new { res = true, mess = Notification.Messager_InsertSuccess });
        }
        public IHttpActionResult UpdateSuccess()
        {
            return Json(new { res = true, mess = Notification.Messager_UpdateSuccess });
        }
        public IHttpActionResult DeleteSuccess()
        {
            return Json(new { res = true, mess = Notification.Messager_DeleteSuccess });
        }
        [HttpPost]
        public IHttpActionResult UploadImage()
        {
            try
            {
                var path = "";
                string result = "";
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];
                    var filenameImage = Guid.NewGuid().ToString() + ".jpg";

                    DateTime time = DateTime.Now;
                    string format = "yyyyMMdd";
                    var dt = time.ToString(format);
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Images/Doitac/" + dt)))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Images/Doitac/" + dt));
                    }

                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/Doitac/" + dt), filenameImage);

                    file.SaveAs(path);
                    result = "~/Images/Doitac/" + dt + "/" + filenameImage;
                }
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(" - UploadImage :{0}", ex.Message);
            }
            return Exeption();
        }

        [HttpGet]
        public IHttpActionResult GetDatetime()
        {
            return Json(new {nam = Notification.Nam.OrderByDescending(o=>o),
                            thang= Notification.Thang,
                            quy= Notification.Quy.ToList()
            });
        }
        [HttpGet]
        public IHttpActionResult getSonguoionl()
        {
            contant.RemoveAppChat(long.Parse(DateTime.Now.ToString("yyyyMMddHHmm")));
            return Json(HttpContext.Current.Application["So_nguoi_Online"]);
        }
    }
}
