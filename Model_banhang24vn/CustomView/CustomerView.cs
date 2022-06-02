using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
   public class CustomerView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string DistrictCityname { get; set; }
        public string TypeBusinessname { get; set; }
        public Guid?  DistrictCityId { get; set; }
        public Guid? TypeBusinessId { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Images { get; set; }
        public bool? Status { get; set; }
        public string Url { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? prioritize { get; set; }
    }
}
