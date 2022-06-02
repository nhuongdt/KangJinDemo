using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
 
namespace Open24.Areas.AdminPage.ApiControllers
{
    public class AppChatController : ApiController
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<UserVisit> _UserVisit;
       
        public AppChatController()
        {
            _UserVisit = unitOfWork.GetRepository<UserVisit>();
        }

        /// <summary>
        /// Lấy thông tin danh sách người dùng đang online
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<AppChat> GetAll()
        {
            contant.RemoveAppChat(long.Parse(DateTime.Now.ToString("yyyyMMddHHmm")));
            return Notification.ListAppChat.ToList();
        }

        /// <summary>
        /// thêm người dùng online và cập nhật vào db
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="name"></param>
        /// <param name="city"></param>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult AppChat(string ip, string name, string city)
        {
            int typeDevice = StaticVariable.GetDeviceType(Request.Headers.UserAgent.ToString());
            if (Notification.ListAppChat!=null 
                && !Notification.ListAppChat.Any(o => o.Id == Notification.AppChatId) 
                && !Notification.AppChatId.Equals(new Guid()))
            {
                var browser = "Other";
                if(Request.Headers.UserAgent.ToString().Contains("Firefox"))
                {
                    browser = Request.Headers.UserAgent.ToArray()[3].ToString();
                }
                else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                {
                    browser = Request.Headers.UserAgent.ToArray()[6].ToString();
                }
                else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                {
                    browser = Request.Headers.UserAgent.ToArray()[4].ToString();
                }
                var data = new AppChat
                {
                    Id = Notification.AppChatId,
                    City = city,
                    Country = name,
                    Ip = ip,
                    CreateDate = DateTime.Now,
                    ChatPage = new List<ChatPageView>(),
                    Browser = browser
                };
                switch (typeDevice)
                {
                    case (int)Notification.Device.tablet:
                        data.Device = "Tablet";
                        break;
                    case (int)Notification.Device.desktop:
                        data.Device = "Desktop";
                        break;
                    case (int)Notification.Device.mobile:
                        data.Device = "Mobile";
                        break;
                    default:
                        data.Device = "Other";
                        break;
                }
                data.System = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
               (HttpContext.Current.Application["App_Chat_Online"] as List<AppChat>).Add(data);
                HttpContext.Current.Application["So_nguoi_Online"] = (int)HttpContext.Current.Application["So_nguoi_Online"] + 1;
                SaveUserVisit(data);
            }
            else if(Notification.ListAppChat.Any(o => o.Id == Notification.AppChatId && o.Ip==null))
            {
                var browser = "Other";
                if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                {
                    browser = Request.Headers.UserAgent.ToArray()[3].ToString();
                }
                else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                {
                    browser = Request.Headers.UserAgent.ToArray()[6].ToString();
                }
                else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                {
                    browser = Request.Headers.UserAgent.ToArray()[4].ToString();
                }
                var data = Notification.ListAppChat.Find(o => o.Id == Notification.AppChatId);
                data.City = city;
                data.Country = name;
                data.Ip = ip;
                data.CreateDate = DateTime.Now;
                data.ChatPage = new List<ChatPageView>();
                data.Browser = browser;
                switch (typeDevice)
                {
                    case (int)Notification.Device.tablet:
                        data.Device = "Tablet";
                        break;
                    case (int)Notification.Device.desktop:
                        data.Device = "Desktop";
                        break;
                    case (int)Notification.Device.mobile:
                        data.Device = "Mobile";
                        break;
                    default:
                        data.Device = "Other";
                        break;
                }
                data.System = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                SaveUserVisit(data);
            }
            return Json("");
        }
        

        private void SaveUserVisit( AppChat data)
        {
            // Lưu thông tin người dùng truy cập
            if (_UserVisit.Find(Notification.AppChatId) == null)
            {
                _UserVisit.Create(new UserVisit
                {
                    ID = Notification.AppChatId,
                    City = data.City,
                    Country = data.Country,
                    Ip = data.Ip,
                    CreateDate = DateTime.Now,
                    System = data.System,
                    Device = data.Device,
                    Browser = data.Browser
                    
                });
                unitOfWork.Save();
            }
        }


        /// <summary>
        /// Cập nhật người dùng đang online
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult CheckAppchat()
        {
            var appchat = Notification.ListAppChat.FirstOrDefault(o => o.Id == Notification.AppChatId);
            var now = long.Parse(DateTime.Now.ToString("yyyyMMddHHmm"));
            if (appchat != null)
            {
                appchat.ChatPage = appchat.ChatPage.Where(o => (now - o.Date - o.Minute) <= 2).ToList();
                if (appchat.ChatPage.Any(o => o.Page.Equals(Request.Headers.Referrer.AbsolutePath)))
                {
                    var model = appchat.ChatPage.Find(o => o.Page.Equals(Request.Headers.Referrer.AbsolutePath));
                    model.Minute += (now - model.Date - model.Minute);
                }

            }
            return Json("");
        }
    }
}
