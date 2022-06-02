using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace banhang24.Hellper
{
    public static class UrlHelperExtensions
    {
        public static string ContentVersioned(this UrlHelper self, string contentPath)
        {
            string versionedContentPath = contentPath + "?v=" + WebConfigurationManager.AppSettings["VersionCache"].ToString();
            return self.Content(versionedContentPath);
        }
    }
}