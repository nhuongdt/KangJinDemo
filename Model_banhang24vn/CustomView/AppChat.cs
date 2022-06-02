using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
   public class AppChat
    {
        public Guid Id { get; set; }
        public string Ip { get; set; }
        public string Browser { get; set; }
        public string City { get; set; }
        public string Device { get; set; }
        public string Country { get; set; }
        public string System { get; set; }
        public DateTime CreateDate { get; set; }
        public List<ChatPageView> ChatPage { get; set; }

    }
    public class ChatPageView
    {
        public string Page { get; set; }
        public long Date { get; set; }
        public long Minute { get; set; }
    }

   
}
