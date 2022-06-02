using Model_banhang24vn;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiAccountController : ApiController
    {

        private News_UserService _News_UserService;

        public ApiAccountController()
        {
            _News_UserService = new News_UserService();
        }

        /// <summary>
        /// Lấy dữ liệu người dùng cần cập nhật
        /// </summary>
        /// <returns></returns>
        public News_User GetProfileUser()
        {
            try
            {
                var result = _News_UserService.Getbykey(contant.SESSIONNGUOIDUNG.UserID);
                return result;
            }
            catch (Exception ex)
            {

            }
            return  new News_User();
        }

    }
}
