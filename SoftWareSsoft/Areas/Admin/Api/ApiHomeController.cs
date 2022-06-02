using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model.Web.Service;
using Model.Web;
using Ssoft.Common.Common;
using System.Web.Configuration;
using SoftWareSsoft.Models.ThemeSsoft;

namespace SoftWareSsoft.Areas.Admin.Api
{
    public class ApiHomeController : ApiBaseController
    {
        private ContactService _ContactService;
        private DM_MenuService _DM_MenuService;
        public ApiHomeController()
        {
            _ContactService = new ContactService();
            _DM_MenuService = new DM_MenuService();
        }

        [HttpPost]
        public IHttpActionResult SendContact(DM_LienHe model)
        {
            try
            {
                if(model==null)
                {
                    return ActionFalseNotData("Không lấy được dữ liệu cần gửi.");
                }
                model.ID = Guid.NewGuid();
                model.TrangThai = (int)LibEnum.StatusContact.taomoi;
                model.NgayTao = DateTime.Now;
                _ContactService.Insert(model);
                return ActionTrueNotData("Gửi yêu cầu thành công");
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

        [HttpPost]
        public IHttpActionResult SearchGridContact(SearchModel model)
        {
            try
            {
                var data = _ContactService.SearchGrid(model.text,model.TrangThais);
                var count = data.Count();
                if (model.limit == 0)
                {
                    model.limit = GridPagedingHellper.PageDefault;
                }
                model.pageCount = (int)Math.Ceiling((double)count / model.limit);
                if (model.pageCount == 0 || model.pageCount == 1)
                {
                    model.pageCount = 1;
                    model.page = 1;
                }
                model.data = data.OrderByDescending(o=>o.NgayTao).Skip(model.limit * (model.page - 1)).Take(model.limit).AsEnumerable().Select(o=>new {
                    o.ID,
                    o.TenNguoiLienHe,
                    o.Email,
                    o.GhiChu,
                    o.SoDienThoai,
                    o.DiaChi,
                    NgayTao=o.NgayTao.ToString("yyyy/MM/dd hh:mm tt"),
                    TrangThai=o.TrangThai==(int)LibEnum.IsStatus.hoatdong?true:false
                }).AsEnumerable();
                model.pageItem = string.Format("{0} - {1} of {2} ", ((model.page - 1) * model.limit) + 1, data.Count(), count);
                return ActionTrueData(model);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }


        [HttpGet]
        public IHttpActionResult UpdateContactRead(Guid id)
        {
            try
            {
                _ContactService.UpdateContactRead(id);
                return ActionTrueData("");
            }
            catch(Exception ex)
            {
                return Exception(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GetListMenuMeta(SearchModel model)
        {
            try
            {
                var data = _DM_MenuService.SearchGrid(model.text);
                if (model.limit == 0)
                {
                    model.limit = GridPagedingHellper.PageDefault;
                }
                model.pageCount = (int)Math.Ceiling((double)data.Count() / model.limit);
                if (model.pageCount == 0 || model.pageCount == 1)
                {
                    model.pageCount = 1;
                    model.page = 1;
                }
                model.pageItem = GridPagedingHellper.PageItems(model.page, model.pageCount, data.Count());
                model.data = data.Skip(model.limit * (model.page - 1)).Take(model.limit).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.Title,
                    o.DuongDan,
                    o.Link,
                    o.Description,
                    o.KeyWord,
                    o.ID_Loaimenu,
                    o.TrangThai

                });

                return ActionTrueData(model);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpPost]
        public IHttpActionResult Savemenu(DM_Menu model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.ID==new Guid())
                {
                    _DM_MenuService.Insert(model);
                    return InsertSuccess();
                }
                else
                {
                    if (_DM_MenuService.Update(model))
                        return UpdateSuccess();
                    else
                        return ActionFalseNotData("Không tồn tại menu, hoặc menu đã bị xóa");
                }
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpPost]
        public IHttpActionResult RemoveMenu(DM_Menu model)
        {
            try
            {
                if (model==null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                else
                {
                    _DM_MenuService.Delete(model);
                    return DeleteSuccess();
                }
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
    }
}
