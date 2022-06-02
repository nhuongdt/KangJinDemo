using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Open24.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Configuration;
using System.Web.Http;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiProductController : ApiBaseController
    {
        TinhThanhService _TinhThanhService;
        OrderService _OrderService;
        public ApiProductController()
        {
            _OrderService = new OrderService();
            _TinhThanhService = new TinhThanhService();
        }

        [HttpGet]
        public IHttpActionResult GetAllTinhThanh()
        {
            return Json(_TinhThanhService.GetTinhThanhQuanHuyen().OrderBy(o => o.TinhThanh).Select(o => new { ID = o.ID, Value = o.QuanHuyen + " - " + o.TinhThanh }).AsEnumerable());
        }

        [HttpPost]
        public IHttpActionResult SaveOrder(OrderView model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.EmailOrder))
                {
                    if (!StaticVariable.EmailIsValid(model.EmailOrder))
                    {
                        return ActionFalseNotData("Email không hợp lệ.");
                    }
                }
                else
                {
                    return ActionFalseNotData("Vui lòng nhập địa chỉ Email.");
                }

                if (!string.IsNullOrWhiteSpace(model.PhoneOrder))
                {
                    if (!StaticVariable.EmailIsValid(model.EmailOrder))
                    {
                        return ActionFalseNotData("Số điện thoại người đặt hàng không hợp lệ.");
                    }

                }
                else
                {
                    return ActionFalseNotData("Vui lòng nhập số điện thoại người đặt hàng.");
                }

                if (model.TinhThanhOrder_ID == null)
                {
                    return ActionFalseNotData("Vui lòng chọn địa chỉ tỉnh thành đặt hàng.");
                }
                if (model.CheckReceived == true)
                {
                    if (!string.IsNullOrWhiteSpace(model.PhoneReceived))
                    {
                        if (!StaticVariable.EmailIsValid(model.EmailOrder))
                        {
                            return ActionFalseNotData("Vui lòng nhập số điện thoại người nhận hàng.");
                        }

                    }
                    else
                    {
                        return ActionFalseNotData("Vui lòng nhập số điện thoại người nhận hàng.");
                    }
                    if (model.TinhThanhReceived_ID == null)
                    {
                        return ActionFalseNotData("Vui lòng chọn tỉnh thành người nhận hàng.");
                    }
                }
                if (model.ProductDevices != null && model.ProductDevices.Count > 0)
                {
                    var result = _OrderService.InsertOrder(model);
                    var tinhthanhOrder = _TinhThanhService.GetTinhThanhQuanHuyen().Where(o => o.ID == model.TinhThanhOrder_ID).Select(o => o.QuanHuyen + " - " + o.TinhThanh).FirstOrDefault();
                    var tinhthanhReceived = _TinhThanhService.GetTinhThanhQuanHuyen().Where(o => o.ID == model.TinhThanhReceived_ID).Select(o => o.QuanHuyen + " - " + o.TinhThanh).FirstOrDefault();
                    string header1 = "<p> Cảm ơn anh/chị <span style='font-weight: bold;    font-size: 16px;'>" + model.UserOrder + "</span> đã đặt hàng tại <span style='font-weight: bold;    font-size: 16px;'>Open24</span></p>"
                       + "<p>Đơn hàng của Anh/chị đã được tiếp nhận, chúng tôi sẽ nhanh chóng liên hệ với Anh/chị.</p>"
                       + "<p>Thông tin trạng thái đơn hàng sẽ được cập nhật vào Email.</p>";
                    string header2 = "<h2> Thông tin đơn hàng mới có mã đơn hàng: #" + result.ID + " !";
                    string body = "<h3>Thông tin người mua</h3>"
                       + "<p>Họ tên: " + model.UserOrder + "</p>"
                       + "<p>Số điện thoại: " + model.PhoneOrder + "</p>"
                       + "<p>Email: <a href='" + model.EmailOrder + "' target='_blank'>" + model.EmailOrder + "</a></p>"
                       + "<p>Địa chỉ: " + model.AdressOrder + " - " + tinhthanhOrder + "</p><br />"
                        + "<h3>Thông tin người nhận</h3>"
                       + "<p>Họ tên: " + (model.CheckReceived == true ? model.UserReceived : model.UserOrder) + "</p>"
                       + "<p>Số điện thoại: " + (model.CheckReceived == true ? model.PhoneReceived : model.PhoneOrder) + "</p>"
                       + "<p>Địa chỉ: " + (model.CheckReceived == true ? model.AdressReceived + " - " + tinhthanhReceived : model.AdressOrder + " - " + tinhthanhOrder) + "</p><br />"
                       + "<h3>--- Thông tin đơn hàng --- </h3>"
                        + "<h3 > Mã đơn hàng: #" + result.ID + " - Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy") + "</h3>"
                       + "<table><tr><td>Sản phẩm</td><td style='text-align:center; width: 100px'>Mã SP</td><td style='text-align:center; width: 100px'>Số lượng</td><td style='text-align:right; width: 100px'>Tổng</td></tr>";
                    foreach (var item in model.ProductDevices)
                    {
                        body += "<tr style=' height: 40px;   border-top: 1px solid darkblue'><td>" + item.Name + "</td><td style='text-align:center; width: 100px'>" + item.Encoder + "</td><td style='text-align:center; width: 100px'>" + item.Quantity + "</td><td style='text-align:right; width: 100px'>" + StaticVariable.ConvertVND(item.Money ?? 0) + "</td></tr>";
                    }
                    var thanhtien = StaticVariable.ConvertVND(model.ProductDevices.Select(o => o.Money ?? 0).Sum());
                    body = body + "<tr style=' height: 40px;   border-top: 1px solid darkblue'><td></td><td colspan='2'>Giảm giá:</td><td style='text-align:right; width: 100px'>" + StaticVariable.ConvertVND(0) + "</td></tr>"
                          + "<tr style=' height: 40px;   border-top: 1px solid darkblue'><td></td><td colspan='2'>Tổng giá trị sản phẩm:</td><td style='text-align:right; width: 100px'>" + thanhtien + "</td></tr>"
                          + "<tr style=' height: 40px;   border-top: 1px solid darkblue'><td></td><td colspan='2'>Thành tiền:</td><td style='text-align:right; width: 100px'>" + thanhtien + "</td></tr></table>";
                    MailHellper.SendThreadEmail(model.EmailOrder, "Xác nhận đơn hàng #" + result.ID + " từ Open24", header1 + body);
                    MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(), "Đơn đặt hàng mới #" + result.ID + " từ Open24", header2 + body);
                    return RetunJsonAction(true, string.Empty, result.ID);
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
            return ActionFalseNotData("Không lấy được thông tin cần đặt hafngv ui lòng thử lại sau");

        }



        [HttpGet]
        public IHttpActionResult GetDetailOrder(long? id)
        {
            try
            {
                var data = _OrderService.GetJoinOrderDevices(id ?? 0).ToList();
                return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult OrderSearchGrid(DataGridView model)
        {
            try
            {
                var data = _OrderService.searhOrder(model.Search).OrderBy(o => o.ID);
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.ToList().Select(o =>
                    new OrderDetailView
                    {
                        ID = o.ID,
                        AdressOrder = o.AdressOrder,
                        AdressReceived = !string.IsNullOrWhiteSpace(o.AdressReceived) ? o.AdressReceived : o.AdressOrder,
                        EmailOrder = o.EmailOrder,
                        Encoder = "#" + o.ID,
                        Note = o.Note,
                        payment = Notification.HinhThucVanChuyen.Where(c => c.Key == o.payment).Select(c => c.Value).FirstOrDefault(),
                        PhoneOrder = o.PhoneOrder,
                        PhoneReceived = !string.IsNullOrWhiteSpace(o.PhoneReceived) ? o.PhoneReceived : o.PhoneOrder,
                        UserOrder = o.UserOrder,
                        UserReceived = !string.IsNullOrWhiteSpace(o.UserReceived) ? o.UserReceived : o.UserOrder,
                        CreatedDate = o.CreatedDate,
                        ModifiedBy = o.ModifiedBy,
                        ModifiedDate = o.ModifiedDate,
                        Sale = o.Sale,
                        Money = o.OrderDetails.Sum(c => (c.Quantity ?? 0) * (c.Price ?? 0)),
                        statusShow = Notification.TrangThaiOrder.Where(c => c.Key == o.Status).Select(c => c.Value).FirstOrDefault(),
                        status = o.Status,
                    });
                }
                else
                {
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList().Select(o =>
                    new OrderDetailView
                    {
                        ID = o.ID,
                        AdressOrder = o.AdressOrder,
                        AdressReceived = !string.IsNullOrWhiteSpace(o.AdressReceived) ? o.AdressReceived : o.AdressOrder,
                        EmailOrder = o.EmailOrder,
                        Encoder = "#" + o.ID,
                        Note = o.Note,
                        payment = Notification.HinhThucVanChuyen.Where(c => c.Key == o.payment).Select(c => c.Value).FirstOrDefault(),
                        PhoneOrder = o.PhoneOrder,
                        PhoneReceived = !string.IsNullOrWhiteSpace(o.PhoneReceived) ? o.PhoneReceived : o.PhoneOrder,
                        UserOrder = o.UserOrder,
                        UserReceived = !string.IsNullOrWhiteSpace(o.UserReceived) ? o.UserReceived : o.UserOrder,
                        CreatedDate = o.CreatedDate,
                        ModifiedBy = o.ModifiedBy,
                        ModifiedDate = o.ModifiedDate,
                        Sale = o.Sale,
                        Money = o.OrderDetails.Sum(c => (c.Quantity ?? 0) * (c.Price ?? 0)),
                        statusShow = Notification.TrangThaiOrder.Where(c => c.Key == o.Status).Select(c => c.Value).FirstOrDefault(),
                        status = o.Status,
                    });
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult ChangeStatusOrder(OrderDetailView model)
        {
            try
            {
                if (model == null || model.ID == 0 || model.status == null)
                {
                    return ActionFalseNotData("Không lấy được đơn đặt hàng cần cập nhật");
                }
                if (_OrderService.ChangeStatusOrder(model.ID, model.status ?? 0) == (int)Notification.ErrorCode.success)
                {
                    return UpdateSuccess();
                }
                return ActionFalseNotData("Đơn đặt hàng không tồn tại hoặc đã bị xóa, vui lòng tải lại trang");
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteOrder(OrderDetailView model)
        {
            try
            {
                if (model == null || model.ID == 0)
                {
                    return ActionFalseNotData("Không lấy được đơn đặt hàng cần cập nhật");
                }
                _OrderService.DeleteOrder(model.ID);
                return DeleteSuccess();

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

      

    }
}
