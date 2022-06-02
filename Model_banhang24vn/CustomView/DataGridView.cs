using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
   public class DataGridView
    {
        //------ Pageding----------------
        public int PageCount { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public object Data { get ; set; }
        public string PageItem { get; set; }
        public int? Sort { get; set; }
        public int? Columname { get; set; }
       //------- Filter ---------------------
        public DateTime? Datetime { get; set; }
        public int? TypeHsd { get; set; }
        public string Search { get; set; }
        public bool? Status { get; set; }
        public int? Version { get; set; }
    }
    public class Chart
    {
        public string label { get; set; }
        public double y { get; set; }
    }
}
