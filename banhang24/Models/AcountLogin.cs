using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.Models
{
    public class AcountLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
        public string Ipaddress { get; set; }
    }
}