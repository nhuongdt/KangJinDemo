using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
   public class BusinessView
    {
        public Guid ID { get; set; }

        public string MaNganhNghe { get; set; }

        public string TenNganhNghe { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public string Image { get; set; }

        public string ImageMobile { get; set; }

        public bool? Status { get; set; }
    }

    public class BusinessRole
    {
        public Guid ID { get; set; }
        public string TenNganhNghe { get; set; }
        public string UserCurent  { get; set; }
        public string Image { get; set; }
        public string ImageMobile { get; set; }
        public bool? Status { get; set; }
        public List<string > checkList { get; set; }
    }
}
