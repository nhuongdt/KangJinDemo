
using SoftWareSsoft.Controllers;
using Ssoft.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace SoftWareSsoft.Hellper
{
    public class RobotsHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string domain = context.Request.Url.Authority;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("user-agent: *");
            stringBuilder.AppendLine("disallow: /Admin/");
            stringBuilder.AppendLine("allow: /Admin/Home");
            stringBuilder.AppendLine("sitemap:"+"http://ssoft.vn/sitemap.xml");
            context.Response.Clear();
            context.Response.StatusCode = 200;
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "text/plain";

            // return the robots content
            context.Response.Write(stringBuilder);
        }
        public bool IsReusable { get { return false; } }
    }

    public class SitemapHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/xml";
            context.Response.ContentEncoding = Encoding.UTF8;
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var sitemapNodes = new ThemeSsoftController().GetSitemapNodes(url);
            string xml = StaticVariable.GetSitemapDocument(sitemapNodes);
            //return this.Content(xml, "text/xml", Encoding.UTF8);
            context.Response.Write(xml);
        }
        public bool IsReusable { get { return false; } }
    }

    public class StaticFileRouteHandler : System.Web.Routing.IRouteHandler
    {
        public string VirtualPath { get; set; }
        public StaticFileRouteHandler(string virtualPath)
        {
            VirtualPath = virtualPath;
        }

        public System.Web.IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
            HttpContext.Current.RewritePath(VirtualPath);
            return new DefaultHttpHandler();
        }
    }
}