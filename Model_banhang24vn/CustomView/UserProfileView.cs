using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
    public class UserProfileView
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserNameModified { get; set; }
        public string PasswordNew { get; set; }
        public string PasswordOld { get; set; }
        public string Passwordconfluent { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
