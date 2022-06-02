using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Areas.Admin.Hellper
{
    public static class HtmlWrite
    {
        public static string UserText(this HtmlHelper html)
        {
            var user = ContantaAdmin.GetSessionWorkUser();
            return user != null ? user.UserName : string.Empty;
        }
    }
}