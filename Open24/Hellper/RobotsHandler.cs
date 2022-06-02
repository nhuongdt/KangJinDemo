using Model_banhang24vn.Common;
using Open24.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Open24.Hellper
{
    public class RobotsHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string domain = context.Request.Url.Authority;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("user-agent: *");
            stringBuilder.AppendLine("disallow: /AdminPage/");
            stringBuilder.AppendLine("disallow: /tag/");
            stringBuilder.AppendLine("allow: /AdminPage/Home");
            stringBuilder.Append("sitemap: ");
            stringBuilder.AppendLine("https://open24.vn/sitemap.xml");
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = Encoding.UTF8;

            // return the robots content
            context.Response.Write(stringBuilder);
        }
        public bool IsReusable { get { return false; } }
    }

    public class SitemapHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/xml";
            context.Response.ContentEncoding = Encoding.UTF8;
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var sitemapNodes = new Open24Controller().GetSitemapNodes(url);
            string xml = StaticVariable.GetSitemapDocument(sitemapNodes);
            //return this.Content(xml, "text/xml", Encoding.UTF8);
            context.Response.Write(xml);
        }
        public bool IsReusable { get { return false; } }
    }
}