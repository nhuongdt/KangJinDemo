using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Transports;

namespace banhang24.HubSignalR
{
    [HubName("DisplayHub")]
    public class DisplayHub : Hub
    {
        // Use this variable to track user count
        private static int _userCount = 0;
        ///// <summary>
        ///// The count of users connected.
        ///// </summary>
        //// Overridable hub methods  
        public override Task OnConnected()
        {
            _userCount++;
            var context = GlobalHost.ConnectionManager.GetHubContext<DisplayHub>();
            context.Clients.All.online(_userCount);
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            _userCount++;
            var context = GlobalHost.ConnectionManager.GetHubContext<DisplayHub>();
            context.Clients.All.online(_userCount);
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            _userCount--;
            var context = GlobalHost.ConnectionManager.GetHubContext<DisplayHub>();
            context.Clients.All.online(_userCount);
            return base.OnReconnected();
        }
    }
}