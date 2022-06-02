using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model.Service;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class TongQuanAPIController : BaseApiController
    {
        private CommonService _CommonService;
        public TongQuanAPIController()
        {
            _CommonService = new CommonService();
        }
        [HttpGet]
        public IHttpActionResult GetAdvertisement()
        {
            try
            {
                return ActionTrueData(_CommonService.GetAllAdvertisement().AsEnumerable());
            }
            catch(Exception ex)
            {
                return Exeption(ex);
            }
        }

    }
}
