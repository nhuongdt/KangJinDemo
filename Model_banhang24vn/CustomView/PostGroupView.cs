using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
  public  class PostGroupView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ParentID { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public bool? Status { get; set; }
        public string ParentName { get; set; }
    }
}
