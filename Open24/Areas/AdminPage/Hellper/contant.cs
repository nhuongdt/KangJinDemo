using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Open24.Areas.AdminPage.Models;
using Model_banhang24vn.DAL;
using Model_banhang24vn;
using Model_banhang24vn.Common;
using log4net;
using System.Reflection;
using Model_banhang24vn.CustomView;
using System.Threading.Tasks;
 
namespace Open24.Areas.AdminPage.Hellper
{
    public class contant
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Gán session người dùng cố định
        /// </summary>
        private static string SessionAdminUser = "_SessionAdminUser_";

        /// <summary>
        ///  Dùng để thao tác lấy thông tin người dùng 
        /// </summary>
        //public static NguoiDungLogin nguoidung { get; set; }

        /// <summary>
        /// Dùng để check quyền convert session người dùng sang model
        /// </summary>

        public static NguoiDungLogin SESSIONNGUOIDUNG { get {

                return HttpContext.Current.Session[SessionAdminUser] as NguoiDungLogin;
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
                if (SESSIONNGUOIDUNG != null) {
                    
                        return new UserGroupRoleService().CheckRoleUser(SESSIONNGUOIDUNG.UserGroupID, roleKey);
                    
                }
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
            }

            return false;
        }

        /// <summary>
        /// Xét phiên làm việc của người dùng
        /// </summary>
        /// <param name="model"></param>
        public static void SetSessionWorkUser(News_User model)
        {
            var groupUser = new News_UserGroupService().Getbykey(model.GroupID ?? Guid.NewGuid());
            HttpContext.Current.Session[SessionAdminUser] = new NguoiDungLogin()
            {
                UserID = model.ID,
                UserName = model.UserName,
                Name = model.Name,
                UserGroupID = groupUser != null ? groupUser.ID : new Guid()
            };
        }

        /// <summary>
        /// Xóa các người dùng không còn truy cập
        /// </summary>
        public static void RemoveAppChat(long now)
        {
            var data = Notification.ListAppChat.Where(o => ((o.ChatPage == null || o.ChatPage.Count == 0) &&( now - long.Parse(o.CreateDate.ToString("yyyyMMddHHmm"))>3)) || (o.ChatPage.Count>0 &&!o.ChatPage.Any(c=>(now-c.Date-c.Minute)<=2))).ToList();
            foreach (var item in data)
            {
                (HttpContext.Current.Application["App_Chat_Online"] as List<AppChat>).Remove(item);
                if ((int)HttpContext.Current.Application["So_nguoi_Online"] > 0)
                {
                    HttpContext.Current.Application["So_nguoi_Online"] = (int)HttpContext.Current.Application["So_nguoi_Online"] - 1;
                }
                else
                {
                    HttpContext.Current.Application["So_nguoi_Online"] = 0;
                }
            }
        }
    }
}