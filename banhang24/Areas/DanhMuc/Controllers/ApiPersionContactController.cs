using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class ApiPersionContactController : BaseApiController
    {
        private SsoftvnContext db;
        public ApiPersionContactController()
        {
            db = SystemDBContext.GetDBContext();
        }

        [HttpGet]
        public IHttpActionResult GetpersionContactForCustomer(Guid? id)
        {
            try
            {
                if(id==null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                else
                {
                    var result = db.DM_LienHe.Where(o=>o.ID_DoiTuong==id).AsEnumerable();
                    return ActionTrueData(result);
                }
            }
            catch(Exception e)
            {
                return Exeption(e);
            }
        }
    }
}
