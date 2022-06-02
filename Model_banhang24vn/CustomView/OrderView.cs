using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
    public class OrderView
    {
        public string UserOrder { get; set; }
        public string AdressOrder { get; set; }
        public string PhoneOrder { get; set; }
        public string EmailOrder { get; set; }
        public Guid? TinhThanhOrder_ID { get; set; }
        public string UserReceived { get; set; }
        public string AdressReceived { get; set; }
        public string PhoneReceived { get; set; }
        public Guid? TinhThanhReceived_ID { get; set; }
        public bool? CheckReceived { get; set; }
        public int? payment { get; set; }
        public string Note { get; set; }
        public List<ProductDevice> ProductDevices{get;set;}
    }
    public class ProductDevice
    {
        public long? SalesDevice_ID { get; set; }
        public int? Price { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Encoder { get; set; }
        public int? Money { get; set; }

    }
    public class OrderDetailView
    {
        public long ID { get; set; }
        public string Encoder { get; set; }
        public string UserOrder { get; set; }
        public string AdressOrder { get; set; }
        public string PhoneOrder { get; set; }
        public string EmailOrder { get; set; }
        public string UserReceived { get; set; }
        public string AdressReceived { get; set; }
        public string PhoneReceived { get; set; }
        public string payment { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int? status { get; set; }
        public string statusShow { get; set; }
        public int? Money { get; set; }
        public int? Sale { get; set; }
        public int? Total { get { return (Money ?? 0) - (Sale ?? 0); } }
    }

    public class OrderSalesDeviceView
    {
        public long OrderDetail_ID{get;set;}
        public string SalesDevice_Name { get; set; }
        public string SalesDevice_Encoder { get; set; }
        public int? Order_Quantity { get; set; }
        public int? Order_Money { get; set; }
        public int? Order_price { get; set; }
    }
}
