using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Model;

namespace banhang24.HubSignalR
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send()
        {
            string nguoidungId=Hellper.contant.IdNguoiDung;
            string usertVersion = Hellper.contant.UserVerSion;
            if(string.IsNullOrWhiteSpace(usertVersion))
            {
                usertVersion = Guid.NewGuid().ToString();
                CookieStore.SetCookieAes(Hellper.SystemConsts.UserVersion, usertVersion, new TimeSpan(30, 0, 0, 0, 0), CookieStore.GetCookieAes("SubDomain"));
            }
            Clients.All.addNewMessageToPage(HttpContext.Current.Request.Url.Authority, nguoidungId, usertVersion);
        }
    }
}