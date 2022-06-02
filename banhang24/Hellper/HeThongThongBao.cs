using libHT_NguoiDung;
using Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using libDM_DoiTuong;

namespace banhang24.Hellper
{
    public class HeThongThongBao : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DateTime now = DateTime.Now;
            int months = now.Month;
            int Day = now.Day;
            List<DM_DoiTuong> lst = classDM_DoiTuong.Gets(p => p.LoaiDoiTuong == 1).ToList();
            foreach (var item in lst)
            {
                if (item.NgaySinh_NgayTLap.Value.Month - months == 0 && item.NgaySinh_NgayTLap.Value.Day - Day == 0)
                {
                    HT_ThongBao httb = new HT_ThongBao();
                    httb.ID = Guid.NewGuid();
                    httb.ID_DonVi = item.ID_DonVi.Value;
                    httb.NoiDungThongBao = "<p>Khách hàng có mã <a href=\"javscript(void)\" onclick=\"loadthongbao(3)\">" + item.MaDoiTuong + " </a> có sinh nhật hôm nay</p>";
                    httb.NgayTao = DateTime.Now;
                }
            }
        }
    }
}