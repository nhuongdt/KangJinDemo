using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftWareSsoft.Models.ThemeSsoft
{
    public class SearchModel
    {
        public string pageItem { get; set; }
        public int pageCount { get; set; }
        public int page { get; set; }
        public string text { get; set; }
        public int? groupId { get; set; }
        public int? groupNewId { get; set; }
        public int limit { get; set; }
        public object data { get; set; }
        public List<int> TrangThais { get; set; }
    }
  

}