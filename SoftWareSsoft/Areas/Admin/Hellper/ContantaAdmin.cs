using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Web;
using SoftWareSsoft.Areas.Admin.Models;
using Model.Web.API;
using Newtonsoft.Json;
using Model.Web.Service;

namespace SoftWareSsoft.Areas.Admin.Hellper
{
    public class ContantaAdmin
    {
       private static string SessionUser = "_SessionUser_";

        /// <summary>
        /// Xét phiên làm việc của người dùng
        /// </summary>
        /// <param name="model"></param>
        public static void SetSessionWorkUser(HT_NguoiDung model)
        {
            var SessionAdminUser = SessionUser+ SystemDBContext.GetStrSubDomain();
            var result = new NguoiDungLogin()
            {
                UserID = model.ID,
                UserName = model.TaiKhoan,
                Name = model.TenNguoiDung,
                UserGroupID = model.ID_NhomNguoiDung,
                IsAdmin=model.LaAdmin
            };
            CookieStore.SetCookieAes(SessionAdminUser, JsonConvert.SerializeObject(result), new TimeSpan(30, 0, 0, 0, 0), string.Empty);
          
        }
        public static   NguoiDungLogin GetSessionWorkUser()
        {
            var SessionAdminUser = SessionUser + SystemDBContext.GetStrSubDomain();
            var value=  CookieStore.GetCookieAes(SessionAdminUser);
            if (string.IsNullOrWhiteSpace(value))
                return null;
            CookieStore.SetCookieAes(SessionAdminUser, value, new TimeSpan(30, 0, 0, 0, 0), string.Empty);
            return JsonConvert.DeserializeObject<NguoiDungLogin>(value);

        }

        public static void ClearSessionWorkUser()
        {
            var SessionAdminUser = SessionUser + SystemDBContext.GetStrSubDomain();
            HttpCookie currentUserCookie = HttpContext.Current.Request.Cookies[SessionAdminUser];
            if (currentUserCookie != null)
            {
                HttpContext.Current.Response.Cookies.Remove(SessionAdminUser);
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                HttpContext.Current.Response.SetCookie(currentUserCookie);
            }
        }
        /// <summary>
        /// Kiểm tra quyền
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>true: có quyền: false: ngược lại</returns>
        public static bool CheckRole(string roleKey)
        {
            try
            {
                var result = GetSessionWorkUser();
                if (result != null)
                {
                    if (result.IsAdmin)
                        return true;
                    else
                        return new UserService().CheckRoleUser( result.UserGroupID, roleKey);

                }
            }
            catch 
            {

            }

            return false;
        }


    }
}