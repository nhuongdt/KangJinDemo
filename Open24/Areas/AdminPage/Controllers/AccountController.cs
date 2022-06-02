using Model_banhang24vn.Common;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Open24.Areas.AdminPage.Hellper;
using Model_banhang24vn;
using Model_banhang24vn.CustomView;
using log4net;
using System.Reflection;
 
namespace Open24.Areas.AdminPage.Controllers
{
    public class AccountController : BaseController
    {
        #region Declare
        // GET: AdminPage/Account
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private News_UserService _News_UserService;
        private News_UserGroupService _News_UserGroupService;
        public AccountController()
        {
            _News_UserService = new News_UserService();
            _News_UserGroupService = new News_UserGroupService();
        }
        #endregion

        #region View
        public ActionResult Index()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult Profile()
        {
            return View();
        }
        #endregion

        #region Event

        /// <summary>
        /// Update thông tin người dùng hiện tại
        /// </summary>
        /// <returns></returns>
        [RBACAuthorize]
        [HttpPost]
        public JsonResult UpdateProfile(UserProfileView model)
        {
            try
            {
                var validate = ValidateUser(model);
                if (validate.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    model.UserId = contant.SESSIONNGUOIDUNG.UserID;
                    model.UserNameModified = contant.SESSIONNGUOIDUNG.UserName;
                    var result = _News_UserService.UpdateProfile(model);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        return RetunJsonAction(true, Notification.Messager_UpdateSuccess, new News_User());
                    }
                    else
                    {
                        return RetunJsonAction(false, result.Data, new News_User());
                    }
                }
                else
                {
                    return RetunJsonAction(false, validate.Data, new News_User());
                }

            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return RetunJsonAction(false, Notification.Messager_Exception, new News_User());
        }


        /// <summary>
        ///  admin login
        /// </summary>
        /// <param name="UserLogin"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(UserLogin UserLogin)
        {
            try
            {
                if (UserLogin != null)
                {
                    JsonViewModel<News_User> result = _News_UserService.LogginAccount(UserLogin.UserName, UserLogin.UserPassword);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        contant.SetSessionWorkUser(result.Data);
                        return Json(new { res = true, mess = "Đăng nhập thành công." });
                    }
                    else if (result.ErrorCode == (int)Notification.ErrorCode.notfound)
                    {
                        return Json(new { res = false, mess = "Tài khoản hoặc mật khẩu sai vui lòng thử lại." });
                    }
                    else if (result.ErrorCode == (int)Notification.ErrorCode.notactive)
                    {
                        return Json(new { res = false, mess = "Tài khoản đã bị khóa không được phép truy cập." });
                    }
                }
                else
                {
                    return Json(new { res = false, mess = "Đã xảy ra lỗi trong quá trình truyền dữ liệu." });
                }
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Json(new { res = false, mess = "Đã xảy ra lỗi vui lòng thử lại sau." });
        }

        #endregion

        #region GetData + Validate

        /// <summary>
        /// Lấy dữ liệu người dùng cần cập nhật
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProfileUser()
        {
            try
            {
                var result = _News_UserService.Getbykey(contant.SESSIONNGUOIDUNG.UserID);
                if (result == null)
                {
                    return RetunJsonAction(false, "Không tìm thấy thông tin tài khoản.", new News_User());
                }
                else
                {
                    return RetunJsonAction(true, "", result);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return RetunJsonAction(false, Notification.Messager_Exception, new News_User());
        }


        /// <summary>
        /// Kiểm tra dữ liệu user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private JsonViewModel<string> ValidateUser(UserProfileView model)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập tên tài khoản.";
            }
            else if (string.IsNullOrWhiteSpace(model.PasswordOld))
            {
                if (!string.IsNullOrWhiteSpace(model.PasswordNew)
                    || !string.IsNullOrWhiteSpace(model.Passwordconfluent))
                {
                    result.ErrorCode = (int)Notification.ErrorCode.error;
                    result.Data = "Vui lòng nhập mật khẩu cũ khi đổi mật khẩu.";
                }
            }
            else if (string.IsNullOrWhiteSpace(model.PasswordNew)
                     || string.IsNullOrWhiteSpace(model.Passwordconfluent))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập mật khẩu mới để thay đổi. ";

            }
            else if (string.IsNullOrWhiteSpace(model.Name))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập họ tên.";
            }
            else if (!string.IsNullOrWhiteSpace(model.Email) && !StaticVariable.EmailIsValid(model.Email.Trim()))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Địa chỉ email không hợp lệ.";
            }
            else if (!string.IsNullOrWhiteSpace(model.Phone) && !StaticVariable.PhoneIsValid(model.Phone.Trim()))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Số điện thoại không hợp lệ.";
            }
            return result;
        }
        #endregion
    }
}