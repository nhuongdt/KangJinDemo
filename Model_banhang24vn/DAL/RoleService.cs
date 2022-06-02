using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
   public class RoleService
    {
        BanHang24vnContext db;
        public RoleService()
        {
            db = new BanHang24vnContext();
        }

        /// <summary>
        /// Lấy toàn bộ danh sách người dùng
        /// </summary>
        /// <returns></returns>
        public IQueryable<Role> GetAll()
        {
            return db.Roles.AsQueryable();
        }
        
        /// <summary>
        /// Lấy danh sách loại bỏ các lớp quyền cha
        /// </summary>
        /// <param name="listRole"></param>
        /// <returns></returns>
        public List<Role> SelectRoleChildren(List<string>listRole)
        {
            var data= db.Roles.Where(o => listRole.Contains(o.RoleKey) && o.RoleParent != null).ToList();
            return data.Where(o => !CheckRoleChildrenParent(o.RoleKey)).ToList();
        }

        public bool CheckRoleChildrenParent(string key)
        {
            return db.Roles.Where(o => o.RoleParent == key).Any();
        }
        /// <summary>
        ///  lấy tên quyền theo key
        /// </summary>
        /// <param name="roleKey"></param>
        /// <returns></returns>
        public string GetRoleNameByKey(string roleKey)
        {
            var result = db.Roles.FirstOrDefault(o => o.RoleKey == roleKey);
            if (result != null)
            {
                return result.Name;
            }
            return string.Empty;
        }
    }
}
