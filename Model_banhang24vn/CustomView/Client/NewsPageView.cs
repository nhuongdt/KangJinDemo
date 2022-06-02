using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView.Client
{
   public class NewsPageView
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Describe { get; set; }
        public DateTime? CreatDate { get; set; }
    }
}
