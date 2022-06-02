
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Open24.Areas.AdminPage.Hellper;
using Model_banhang24vn.DAL;

namespace Open24
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            Application["So_nguoi_Online"] = 0;
            Application["App_Chat_Online"] = new List<AppChat>();
            ScheduleTimer.SetTimer();
        }
        protected void Session_Start()
        {
            Session["AppChatId"] = Guid.NewGuid();
            Application["So_nguoi_Online"] = (int)Application["So_nguoi_Online"] + 1;
            (HttpContext.Current.Application["App_Chat_Online"] as List<AppChat>).Add(new AppChat { Id=(Guid)Session["AppChatId"],ChatPage=new List<ChatPageView>() });
        }
        protected void Session_End()
        {
           
         } 

        protected void Application_End()
        {

        }
        protected void Application_EndRequest()
        {
            //if (Context.Response.StatusCode == 404 || Context.Response.StatusCode == 500)
            //{
            //    Response.Clear();

             
            //}
        }
        protected void application_beginrequest()
        {
            try
            {
                var urlOld = new SeoRedirectUrlService().GetByUrlNew(HttpContext.Current.Request.Url.AbsoluteUri);
                if (urlOld != null)
                {
                    Response.Clear();
                    // 301 it to the new url
                    Response.RedirectPermanent(urlOld.UrlOld);
                }
            }
            catch
            {

            }
        }
        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            // Clear the error
            Response.Clear();
            Server.ClearError();

            if (exception is HttpException)
            {
                HttpException ex = exception as HttpException;
                if (ex.GetHttpCode() == 404 || ex.GetHttpCode() == 500)
                {
                    try
                    {
                        var url = HttpContext.Current.Request.Url.AbsoluteUri;
                        var urlOld = new SeoRedirectUrlService().GetByUrlNew(url);
                        if (urlOld != null)
                        {
                            // 301 it to the new url
                            Response.RedirectPermanent(urlOld.UrlOld);
                        }
                        else
                        {
                            var rd = new RouteData();
                            rd.DataTokens["area"] = ""; // In case controller is in another area
                            rd.Values["controller"] = "Errors";
                            rd.Values["action"] = "Error404";
                            IController c = new Controllers.ErrorsController();
                            c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        protected void Application_PostAuthorizeRequest()
        {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }
    }
}
