using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
    public class SalesDeviceView
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Trademark { get; set; }
        public int? ViewBuy { get; set; }
        public int? TimeGuarantee { get; set; }
        public string GroupDeviceName { get; set; }
        public DateTime? EditDate { get; set; }
        public long? GroupDeviceId { get; set; }
        public string EditUser { get; set; }
        public string SpecialPoint { get; set; }
        public string ApplicationReal { get; set; }
        public string DigitalInformation { get; set; }
        public bool? Status { get; set; }
        public int? Price { get; set; }
        public int? PriceSale { get; set; }
        public bool? IsSalePrice { get; set; }
        public string Encoder { get; set; }
        public string Url { get; set; }
        public IQueryable<object> SalesImgDevices { get; set; }
    }
}
