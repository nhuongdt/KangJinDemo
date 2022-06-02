using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn
{
    public class banhang24NganhNgheKinhDoanh
    {
        public static List<NganhNgheKinhDoanh> SelectAll()
        {
            using (BanHang24vnContext entities = new BanHang24vnContext())
            {
                return entities.NganhNgheKinhDoanhs.ToList();
            }
        }
    }
}
