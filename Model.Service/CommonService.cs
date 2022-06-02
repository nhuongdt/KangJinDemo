using System;
using System.Collections.Generic;
using System.Linq;
using Model_banhang24vn;
using Model_banhang24vn.DAL;
using Model_banhang24vn.Common;
using System.Threading;

namespace Model.Service
{
   public class CommonService
    {
        private AdvertisingService _AdvertisingService;
        private NotificationSoftwareService _NotificationSoftwareService;
        private CuaHangDangKyService _CuaHangDangKyService;
        public CommonService()
        {
            _CuaHangDangKyService = new CuaHangDangKyService();
            _AdvertisingService = new AdvertisingService();
            _NotificationSoftwareService = new NotificationSoftwareService();
        }
       public IQueryable<Advertisement> GetAllAdvertisement()
        {
            return _AdvertisingService.GetAll().Where(o=>o.Status==true 
                                                        && (o.FromDate<=DateTime.Now
                                                        || o.FromDate==null)
                                                        &&(o.ToDate>=DateTime.Now
                                                        || o.ToDate==null)).OrderByDescending(o=>o.EditDate);
        }
        public NotificationSoftware GetNotificationSofware(string subdomain)
        {
            var reuslt = _NotificationSoftwareService.Query.FirstOrDefault(o => o.ApplyDate.HasValue
                                                          && o.ApplyDate.Value.Year == DateTime.Now.Year
                                                          && o.ApplyDate.Value.Month == DateTime.Now.Month
                                                          && o.ApplyDate.Value.Day == DateTime.Now.Day
                                                          && o.Status==true
                                                          && o.Type == (int)Notification.NotificationSoftware.thongbaochung);
           if (reuslt!=null)
            {
                return reuslt;
            }
            var model= _NotificationSoftwareService.Query.FirstOrDefault(o => o.ApplyDate.HasValue
                                                           && o.ApplyDate.Value.Year == DateTime.Now.Year
                                                           && o.ApplyDate.Value.Month == DateTime.Now.Month
                                                           && o.ApplyDate.Value.Day == DateTime.Now.Day
                                                           && o.Status == true
                                                           && o.Type == (int)Notification.NotificationSoftware.thongbaorieng
                                                           && o.Subdomain.ToUpper().Equals(subdomain.ToUpper()));
            return model ?? new NotificationSoftware();
        }

        public bool CheckIsHRM(string Subdomain)
        {
            var guiid = Guid.Parse("649700e6-eea1-416b-b42c-9c7c96e41545");
            return _CuaHangDangKyService.Query.Any(o => o.SubDomain == Subdomain && o.ID_NganhKinhDoanh == guiid);
        }
        public void SendWebClientThread(string url, string model, string typeMethod="GET")
        {
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    var uri = new Uri(url);
                    Thread st1;
                    if (typeMethod.Equals("POST"))
                    {
                        client.Headers.Add("Content-Type:application/json");
                        client.Headers.Add("Accept:application/json");
                         st1 = new Thread(() =>
                            client.UploadStringAsync(uri, "POST", model)
                         );
                    }
                    else
                    {
                        client.UseDefaultCredentials = true;
                         st1 = new Thread(() =>
                           client.DownloadStringAsync(uri)
                        );
                    }
                    st1.Start();
                }
            }
            catch
            {

            }
        }

        public static List<string> GetAllPrinter()
        {
            List<string> lst = new List<string>();
            try
            {
                foreach (string sPrinters in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    lst.Add(sPrinters);
                }
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetAllPrinter " + ex.InnerException + ex.Message);
                return null;
            }

        }
    }
}
