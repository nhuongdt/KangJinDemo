using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libDM_HangHoa
{
    class DMHangHoaObject
    {
    }

    public class GetListHangHoaDatLichCheckinResult
    {
        public Guid ID { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string GhiChu { get; set; }
        public double DonGia { get; set; }

        public string URLAnh { get; set; }
    }
}
