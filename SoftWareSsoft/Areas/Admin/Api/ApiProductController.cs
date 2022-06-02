using SoftWareSsoft.Models.ThemeSsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Ssoft.Common.Common;
using System.Web.Configuration;
using System.Web.Http;
using Model.Web.Service;

namespace SoftWareSsoft.Areas.Admin.Api
{
    public class ApiProductController : ApiBaseController
    {
        private ProductService _ProductService;
        public ApiProductController()
        {
            _ProductService = new ProductService();
        }

        [HttpGet]
        public IHttpActionResult GetCombobox()
        {
            try
            {
                var data = _ProductService.GetAll().Where(o => o.TrangThai != 1 || o.TrangThai==null).OrderByDescending(o => o.NgayTao)
                   .Select(o => new
                    {
                       Key= o.MaSanPham,
                       Value= o.TenSanPham,
                    }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpPost]
        public IHttpActionResult OrderedSsoftSoftWare(Model_banhang24vn.Contact model, string ip4, string ipAdress)
        {
            try
            {
                if (model != null)
                {
                    model.ID = Guid.NewGuid();
                    model.Type = model.Type;
                    model.Ipv4 = ip4;
                    model.IpAdress = ipAdress;
                    model.CreateDate = DateTime.Now;
                    model.Status = 0;
                    model.System = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                    model.Devicess = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                    model.Browser = "Other";
                    if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[3].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[6].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[4].ToString();
                    }
                    var tinhthanh = _TinhThanhService.GetByKey(model.Address);
                    model.Address = tinhthanh != null ? tinhthanh.TenTinhThanh : string.Empty;
                    model.TypeSoftWare = (int)Model_banhang24vn.Common.Notification.TypeSoftWare.Ssoft;
                    new Model_banhang24vn.DAL.ContactService().Insert(model);
                    string body = "<h3> Thông tin đặt hàng phần mềm Ssoft</h3><br>"
                         + "<span>Họ tên: " + model.FullName + "</span><br>"
                         + "<span>Số điện thoại: " + model.Phone + "</span><br>"
                         + "<span>Email: " + model.Email + "</span><br>"
                         + "<span>Địa chỉ: " + model.Address + "</span><br>"
                         + "<span>Gói sản phẩm: " + model.Software + "</span><br><br>"
                         + "<span>Thiết bị: " + model.Devicess + "</span><br>"
                         + "<span>Hệ điều hành: " + model.System + "</span><br>"
                         + "<span>Trình duyệt: " + model.Browser + "</span><br>"
                         + "<span>IPV4: " + ip4 + "</span><br>"
                         + "<span>Khu vực: " + ipAdress + "</span><br><br>"
                         + "<span style='text-align: center'>--- Nội dung đặt hàng --- </span>"
                         + "<p>  " + model.Note + "</p>";
                    MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(),
                        WebConfigurationManager.AppSettings["SPPassWord"].ToString(),
                        WebConfigurationManager.AppSettings["SPGmail"].ToString(),
                        "[Web Ssoft] KH: " + model.FullName + (model.Type == 1 ? "đặt mua" : "muốn dùng thử"),
                        body);
                    return InsertSuccess();
                }
                return ActionFalseNotData("Không thể lấy được thông tin, vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                return Exception(ex);
            }
        }
    }
}
