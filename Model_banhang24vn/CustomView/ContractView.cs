using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
   public class ContractView
    {
        public long ID { get; set; }
        public string StoreOpen { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int? Status { get; set; }
        public string Statuss { get; set; }
        public string IT_Name { get; set; }
        public string IT_Phone { get; set; }
    }
}
