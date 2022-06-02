using Model.Web.API;
using Ssoft.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web.Service
{
    public  class UserService
    {
        private SsoftvnWebContext db;
        public UserService()
        {
            db = SystemDBContext.GetDBContext();
        }
        /// <summary>
        /// Login khi đăng nhập
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="UserPassWord"></param>
        /// <returns></returns>
        public JsonViewModel<HT_NguoiDung> LogginAccount(string userName, string UserPassWord)
        {
            var result = new JsonViewModel<HT_NguoiDung>();
                string pasConverMd5 = ConvertMD5.GetMd5Hash(UserPassWord);
                var dataUser = db.HT_NguoiDung.FirstOrDefault(o => o.TaiKhoan.Equals(userName) && o.MatKhau.Equals(pasConverMd5));
                if (dataUser != null)
                {
                        result.Data = dataUser;
                        result.ErrorCode = (int)LibEnum.ErrorCode.Success;
                   
                }
                else
                {
                    result.ErrorCode = (int)LibEnum.ErrorCode.Error;
                }
          
            return result;
        }
        /// <summary>
        /// Check quyền của mỗi người dùng
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="rolekey"></param>
        /// <returns></returns>
        public bool CheckRoleUser( Guid userGroupId, string rolekey)
        {
            return db.HT_NhomNguoiDung_Quyen.Where(o => o.ID_NhomNguoiDung == userGroupId && o.MaQuyen.Equals(rolekey)).Any();
        }
    }
}
