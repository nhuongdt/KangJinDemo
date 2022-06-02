using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
     public  class UserRoleView
    {
        public bool RoleView { get; set; }
        public bool RoleInsert { get; set; }
        public bool RoleUpdate { get; set; }
        public bool RoleDelete { get; set; }

        public bool RoleUpdateNews { get; set; }
    }

    public class RoleStore : UserRoleView
    {
        public bool ServceSms_View { get; set; }
        public bool ServceSms_Update { get; set; }

        public bool Recharge_View { get; set; }
        public bool Recharge_Insert { get; set; }
        public bool Recharge_Update { get; set; }
        public bool Recharge_Delete { get; set; }
    }
}
