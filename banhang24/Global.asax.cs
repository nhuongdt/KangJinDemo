
using Model;
using Model.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace banhang24
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //ScheduledTasks.Start();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
            var path = HttpContext.Current.Server.MapPath("/AppCache/BanLe.appcache");
            if (System.IO.File.Exists(path))
            {
                var text = System.IO.File.ReadAllLines(path).ToArray();
                if (text[1].Split('_').Length >= 2)
                {
                    long NumberVersion = long.Parse(text[1].Split('_')[1].Trim());
                    text[1] = (string.Format("#Version _ {0} _ {1}", ++NumberVersion, DateTime.Now.ToString("dd-MM-yyyy")));
                }
                string cacheVersion = WebConfigurationManager.AppSettings["VersionCache"].ToString();
                string oldCacheVersion = text[2].Substring(1);
                if (oldCacheVersion != cacheVersion)
                {
                    var rsArray = Array.FindAll(text, p => p.Contains(oldCacheVersion));
                    foreach(var item in rsArray)
                    {
                        int indexItem = Array.IndexOf(text, item);
                        text[indexItem] = item.Replace(oldCacheVersion, cacheVersion);
                    }
                    System.IO.File.WriteAllLines(path, text.ToArray());
                }
            }
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];
            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("vi");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi");
            }
        }
    }
}