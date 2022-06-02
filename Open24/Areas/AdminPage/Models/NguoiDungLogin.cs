using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
namespace Open24.Areas.AdminPage.Models
{
    public class NguoiDungLogin
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public Guid UserGroupID { get; set; }
    }
}