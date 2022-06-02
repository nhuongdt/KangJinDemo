using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
   public class OrderService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<Order> _Order;
        IRepository<OrderDetail> _OrderDetail;
        IRepository<TinhThanh_QuanHuyen> _TinhThanh_QuanHuyen;
        IRepository<SalesDevice> _SalesDevice;
        public OrderService()
        {
            _Order = unitOfWork.GetRepository<Order>();
            _OrderDetail = unitOfWork.GetRepository<OrderDetail>();
            _SalesDevice = unitOfWork.GetRepository<SalesDevice>();
            _TinhThanh_QuanHuyen= unitOfWork.GetRepository<TinhThanh_QuanHuyen>();
        }
        public IQueryable<Order> Query { get {  return _Order.All(); } }

        public Order InsertOrder(OrderView data)
        {
          
            var model = new Order();
            model.AdressOrder = data.AdressOrder +" - "+ _TinhThanh_QuanHuyen.Filter(o=>o.ID== data.TinhThanhOrder_ID).Select(o=>o.QuanHuyen+" - "+o.TinhThanh).FirstOrDefault();
            model.EmailOrder = data.EmailOrder;
            model.Note = data.Note;
            model.PhoneOrder = data.PhoneOrder;
            model.Status = (int)Notification.StatusOrder.TaoMoi;
            model.UserOrder = data.UserOrder;
            model.payment = data.payment;
            model.Sale = 0;
            model.CreatedDate = DateTime.Now;
            if (data.CheckReceived==true)
            {
                model.AdressReceived = data.AdressReceived + " - " + _TinhThanh_QuanHuyen.Filter(o => o.ID == data.TinhThanhReceived_ID).Select(o => o.QuanHuyen + " - " + o.TinhThanh).FirstOrDefault();
                model.PhoneReceived = data.PhoneReceived;
                model.UserReceived = data.UserReceived;
            }
            _Order.Create(model);
            var listSalesDeviceID = data.ProductDevices.Select(o => o.SalesDevice_ID).ToList();
            var listOrderDetail = data.ProductDevices.Select(o => new OrderDetail
            {
                Order_ID = model.ID,
                Price=o.Price,
                Quantity=o.Quantity,
                SalesDevice_ID=o.SalesDevice_ID

            }).ToList();
            _OrderDetail.Create(listOrderDetail);
            var listDevices = _SalesDevice.Filter(o => listSalesDeviceID.Contains(o.ID)).ToList();
            listDevices.ForEach(o => o.ViewBuy += 1);
            unitOfWork.Save();
            return model;
        }

        public IQueryable<Order> searhOrder(string search)
        {
            if(!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToUpper();
                return _Order.Filter(o => search.Contains(o.ID.ToString()) 
                || o.UserOrder.ToUpper().Contains(search));
            }
            return _Order.All();
        }

        public IQueryable<OrderSalesDeviceView> GetJoinOrderDevices(long id)
        {
            return from o in _OrderDetail.All()
                   join d in _SalesDevice.All()
                   on o.SalesDevice_ID equals d.ID
                   where o.Order_ID == id
                   select new OrderSalesDeviceView
                   {
                       OrderDetail_ID=o.ID,
                       Order_Money=o.Price*o.Quantity,
                       Order_price=o.Price,
                       Order_Quantity=o.Quantity,
                       SalesDevice_Encoder=d.Encoder,
                       SalesDevice_Name=d.Name
                   };

        }

        public int ChangeStatusOrder(long id,int status)
        {
            var model = _Order.Filter(o => o.ID == id).FirstOrDefault();
            if (model != null)
            {
                model.Status = status;
                unitOfWork.Save();
                return (int)Notification.ErrorCode.success;
            }
            return (int)Notification.ErrorCode.error;
        }
        public void DeleteOrder(long id)
        {
            _OrderDetail.Delete(o => o.Order_ID == id);
            _Order.Delete(o => o.ID == id);
            unitOfWork.Save();
        }
    }
}
