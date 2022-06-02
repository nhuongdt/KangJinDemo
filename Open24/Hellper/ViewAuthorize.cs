using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Open24.Hellper
{
    public class ViewAuthorize: AuthorizeAttribute
    {
        public string Title { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
                base.OnAuthorization(filterContext);
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            new PageViewService().AddView(string.IsNullOrWhiteSpace(Title)?httpContext.Request.FilePath:Title);
            var appchat = Notification.ListAppChat.FirstOrDefault(o=>o.Id== Notification.AppChatId);
            if(appchat!=null)
            {
              if(!appchat.ChatPage.Any(o=>o.Page== httpContext.Request.FilePath))
               {
                    appchat.ChatPage.Add(new ChatPageView 
                    {
                        Page = httpContext.Request.FilePath,
                        Date = long.Parse(DateTime.Now.ToString("yyyyMMddHHmm")),
                        Minute=0
                    });

               }
            }
            return true;
        }
    }
}