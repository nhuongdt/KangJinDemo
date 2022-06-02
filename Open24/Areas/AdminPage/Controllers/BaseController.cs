using Model_banhang24vn.Common;
using Model_banhang24vn.DAL;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
 
namespace Open24.Areas.AdminPage.Controllers
{
    public class BaseController : Controller
    {
        public JsonResult RetunJsonAction<T>(bool resultRes, string resultMess, T data)
        {
            return Json(new { res = resultRes, mess = resultMess, DataSoure = data });
        }
        public JsonResult ActionTrueNotData(string resultMess)
        {
            return Json(new { res = true, mess = resultMess });
        }
        public JsonResult ActionFalseNotData(string resultMess)
        {
            return Json(new { res = false, mess = resultMess });
        }
        public JsonResult Exeption()
        {
            return Json(new { res = false, mess = Notification.Messager_Exception });
        }
        public JsonResult InsertSuccess()
        {
            return Json(new { res = true, mess = Notification.Messager_InsertSuccess });
        }
        public JsonResult UpdateSuccess()
        {
            return Json(new { res = true, mess = Notification.Messager_UpdateSuccess });
        }
        public JsonResult DeleteSuccess()
        {
            return Json(new { res = true, mess = Notification.Messager_DeleteSuccess });
        }

        public JsonResult RetunJsonGetAction<T>(bool resultRes, string resultMess, T data)
        {
            return Json(new { res = resultRes, mess = resultMess, DataSoure = data }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetExeption()
        {
            return Json(new { res = false, mess = Notification.Messager_Exception }, JsonRequestBehavior.AllowGet);
        }
    }
}