using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ssoft.Common.CustomView
{
    public class UserRoleView
    {
        public bool RoleView { get; set; }
        public bool RoleInsert { get; set; }
        public bool RoleUpdate { get; set; }
        public bool RoleDelete { get; set; }

        public bool RoleUpdateNews { get; set; }
    }
}
