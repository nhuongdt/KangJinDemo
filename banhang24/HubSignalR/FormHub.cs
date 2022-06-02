using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.HubSignalR
{
    [HubName("formHub")]
    public class FormHub:Hub
    {
        public void changeDataHangHoa()
        {
            Clients.Others.returnChangeHangHoa(HttpContext.Current.Request.Url.Authority);
        }
        public void loadDatHang()
        {
            Clients.Others.returnLoadDatHang(HttpContext.Current.Request.Url.Authority);
        }
        public void loadHoaDon()
        {
            Clients.Others.returnloadHoaDon(HttpContext.Current.Request.Url.Authority);
        }
        public void loadTraHang()
        {
            Clients.Others.returnloadTraHang(HttpContext.Current.Request.Url.Authority);
        }
        public void loadThongBao()
        {
            Clients.All.returnLoadThongBao(HttpContext.Current.Request.Url.Authority);
        }
        public void loadThongBaoCountKhiCaiDat()
        {
            Clients.Others.returnLoadThongBao(HttpContext.Current.Request.Url.Authority);
        }

        public void loadSoDoPhong()
        {
            Clients.Others.returnLoadSoDoPhong(HttpContext.Current.Request.Url.Authority);
        }
    }
}