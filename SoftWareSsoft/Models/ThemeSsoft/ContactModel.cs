using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftWareSsoft.Models.ThemeSsoft
{
    public class ContactModel
    {
        public Guid ID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string Software { get; set; }
        public string Ipv4 { get; set; }
        public string IpAdress { get; set; }
        public string Devicess { get; set; }
        public string Browser { get; set; }
        public string System { get; set; }
        public int?  Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Status { get; set; }
    }
}