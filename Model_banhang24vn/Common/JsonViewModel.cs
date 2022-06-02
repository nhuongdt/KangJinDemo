using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.Common
{
   public class JsonViewModel<T>
    {
        public int ErrorCode { get; set; }
        public T Data { get; set; }
    }
}
