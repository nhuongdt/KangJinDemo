using Model.Web.Service;
using SoftWareSsoft.Areas.Admin.Hellper;
using SoftWareSsoft.Areas.Admin.Models;
using Ssoft.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoftWareSsoft.Areas.Admin.Api
{
    public class ApiUserController : ApiBaseController
    {
        private UserService _UserService;

        public ApiUserController()
        {
            _UserService = new UserService();
        }

        [HttpPost]
        public IHttpActionResult LoginAcount(LoginModel model)
        {
                try
                {
                    if (model != null)
                    {
                        var result = _UserService.LogginAccount(model.TaiKhoan, model.MatKhau);
                        if (result.ErrorCode == (int)LibEnum.ErrorCode.Success)
                        {
                            ContantaAdmin.SetSessionWorkUser(result.Data);
                            return ActionTrueNotData("Đăng nhập thành công." );
                        }
                        else if (result.ErrorCode == (int)LibEnum.ErrorCode.Error)
                        {
                            return ActionFalseNotData("Tài khoản hoặc mật khẩu sai vui lòng thử lại." );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exception(ex);
                }
              return ActionFalseNotData("Đã xảy ra lỗi trong quá trình truyền dữ liệu.");

        }


    }
}
