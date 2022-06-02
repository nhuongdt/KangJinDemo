using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace banhang24.App_API
{
    public class SubdomainRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (httpContext.Request == null || httpContext.Request.Url == null)
            {
                return null;
            }
            var host = httpContext.Request.Url.Host;
            var index = host.IndexOf(".");
            string[] segments = httpContext.Request.Url.PathAndQuery.TrimStart('/').Split('/');
            if (index < 0 && host != "localhost")
            {
                return null;
            }
            List<Subdomain> lstSubdomain = ReadXmlSubdomainMap();
            var subdomain = "";
            Subdomain hostSubdomain = lstSubdomain.Where(p => p.Host == host).FirstOrDefault();
            if (hostSubdomain != null)
            {
                subdomain = hostSubdomain.domain;
                //subdomain = "THEFIRSTBEAUTYCENTER";
            }
            else
            {
                subdomain = host.Substring(0, index);
                string[] blacklist = { "www", "open24", "mail" };//yourdomain

                if (blacklist.Contains(subdomain))
                {
                    return null;
                }
            }

            //if (host != "localhost")
            //{
            //    subdomain = host.Substring(0, index);
            //    string[] blacklist = { "www", "open24", "mail" };//yourdomain

            //    if (blacklist.Contains(subdomain))
            //    {
            //        return null;
            //    }
            //}
            //else
            //{
            //    //subdomain = "kayauto";
            //    subdomain = "0973474985";
            //}
            string controller = "";
            string action = "";
            string id = "";
            if (segments.Length > 1)
            {
                controller = segments[0] == "" ? "Home" : segments[0];
                action = segments[1] == "" ? "Index" : segments[1].Split('?')[0];
                if (string.IsNullOrWhiteSpace(action))
                    action = "Index";
            }
            else
            {
                controller = "Home";
                action = segments[0] == "" ? "Index" : segments[0].Split('?')[0];
                if (string.IsNullOrWhiteSpace(action))
                    action = "Index";
            }

            switch (controller)
            {
                case "p":
                    {
                        controller = "HangHoa";
                        break;
                    }
                case "x":
                    {
                        controller = "Report";
                        break;
                    }
                case "e":
                    {
                        controller = "GiaoDich";
                        break;
                    }
                case "c":
                    {
                        controller = "DoiTac";
                        break;
                    }
                case "s":
                    {
                        controller = "TuVan";
                        break;
                    }
                case "f":
                    {
                        controller = "SoQuy";
                        break;
                    }
                case "r":
                    {
                        controller = "BaoCao";
                        break;
                    }
                case "$":
                    {
                        controller = "BanHang";
                        break;
                    }
                case "b":
                    {
                        controller = "Spa";
                        break;
                    }
                case "t":
                    {
                        controller = "ThietLap";
                        break;
                    }
                case "m":
                    {
                        controller = "PhongBan";
                        break;
                    }
                case "n":
                    {
                        controller = "NhanSu";
                        break;
                    }
                case "mark":
                    {
                        controller = "Marketing";
                        break;
                    }
                case "g":
                    {
                        controller = "Gara";
                        break;
                    }
                case "Shared":
                    {
                        controller = "Shared";
                        break;
                    }
                default:
                    {
                        controller = "Home";
                        break;
                    }
            }
            if (segments.Length > 2)
            {
                if (segments[2] != null && segments[2] != "")
                {
                    id = segments[2];
                }
            }
            //controller = controller == "" ? "Home" : controller;
            //string action = (segments.Length > 1) ? segments[1] : "Index";
            var routeData = new RouteData(this, new MvcRouteHandler());
            routeData.Values.Add("controller", controller); //Goes to the relevant Controller  class
            routeData.Values.Add("action", action); //Goes to the relevant action method on the specified Controller
            routeData.Values.Add("subdomain", subdomain); //pass subdomain as argument to action method
            if (id != "")
            {
                routeData.Values.Add("id", id);
            }
            //if (segments.Length > 2 && segments[2] != null && segments[2].ToString().Trim() != "")
            //{
            //    object id = segments[2];
            //    routeData.Values.Add("id", id); //pass subdomain as argument to action method
            //}
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //Implement your formating Url formating here
            return null;
        }

        public List<Subdomain> ReadXmlSubdomainMap()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "SubdomainMap.xml";
                List<Subdomain> lst = new List<Subdomain>();
                using (var reader = new StreamReader(path))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subdomain>), new XmlRootAttribute("SubdomainMap"));
                    lst = (List<Subdomain>)xmlSerializer.Deserialize(reader);
                }
                return lst;
            }
            catch
            {
                return new List<Subdomain>();
            }
        }

    }

    public class SubdomainMap
    {
        public List<Subdomain> Subdomain { get; set; }
    }

    public class Subdomain
    {
        public string Host { get; set; }
        public string domain { get; set; }
    }
}