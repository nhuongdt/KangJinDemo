using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using libDM_DoiTuong;
using Newtonsoft.Json.Linq;

namespace banhang24.HubSignalR
{
    [HubName("AlertHub")]
    public class AlertHub : Hub
    {
        // Use this variable to track user count
        private static int _userCount = 0;
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string message, string msg2)
        {
            Clients.All.send(message, msg2);
        }

        public void sendData_BanHang(JObject obj)
        {
            Clients.All.receiveData_BanHang(obj);
        }

        public void sendData_NhaHang(JObject obj)
        {
            Clients.All.receiveData_NhaHang(obj);
        }

        public void SendData_ChuyenGhep(string json1, string json2, string idViTriNew, string maHoaDon, string idViTriOld)
        {
            Clients.All.readData_afterChuyenGhep(json1, json2, idViTriNew, maHoaDon, idViTriOld);
        }

        public void SendData_GhepHoaDon(string jsonHD, string jsonCTHD, string idViTri, string maHoaDon)
        {
            Clients.All.readData_afterGhepHoaDon(jsonHD, jsonCTHD, idViTri, maHoaDon);
        }

        public void SendHD_CTHD_afterSave(string idViTri, string maHoaDon, string idHoaDonDB)
        {
            Clients.All.readHD_CTHD_afterSave(idViTri, maHoaDon, idHoaDonDB);
        }

        public void SendData_NhaBep_toThuNgan(string idViTri, string maHoaDon, string dataDB)
        {
            Clients.All.readData_NhaBep_toThuNgan(idViTri, maHoaDon, dataDB);
        }

        public void UpdateThucDonYC_andWait(string lstRequest, string lstWait, string dataDB)
        {
            Clients.All.bindAgain_ThucDonYC_Wait(lstRequest, lstWait, dataDB);
        }

        public void sendData_ThuNgan_ToNhaBep(JObject obj)
        {
            Clients.All.receiveData_fromThuNgan(obj);
        }

        public void sendDataBanHang_ToDisplayCustomer(JObject obj)
        {
            Clients.All.receiveData_fromBanHang(obj);
        }

        public void sendDataBanHangToPos_byUser(string idUser, JObject obj)
        {
            Clients.User(idUser).receiveData_fromBanHang_byUser(obj);
        }

        public void sendDataBanHangToPOS_bySubDomain(string groupName, JObject obj)
        {
            Clients.Group(groupName).receiveData_fromBanHang_bySubDomain(obj);
        }

        public override Task OnConnected()
        {
            _userCount++;
            var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
            context.Clients.All.online(_userCount);
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            _userCount++;
            var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
            context.Clients.All.online(_userCount);
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            _userCount--;
            var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
            context.Clients.All.online(_userCount);
            return base.OnReconnected();
        }
    }
}