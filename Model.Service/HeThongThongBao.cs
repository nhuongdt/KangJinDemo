using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model_banhang24vn.DAL;
using System.Threading;

namespace Model.Service
{
  public  class HeThongThongBao : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var _CuaHangDangKyService = new CuaHangDangKyService();
                var lstData = BusinessService.GetDataBaseList();
                var dataSubdomain = _CuaHangDangKyService.Query.Where(o => o.HanSuDung != null && o.HanSuDung >= DateTime.Now).Select(o => "SSOFT_" + o.SubDomain.ToUpper()).ToList();
                List<string> connectionData = lstData.Where(p => dataSubdomain.Contains(p)).ToList();

                foreach (var item in connectionData)
                {
                    Thread st1 = new Thread(() => BusinessService.UpdateBirthday(item.ToUpper()));
                    Thread st2 = new Thread(() => BusinessService.NotifyHanSuDungLo(item.ToUpper()));
                    st1.Start();
                    st2.Start();
                }
                //Thread st1 = new Thread(() => BusinessService.NotifyHanSuDungLo("0973474985"));
                //st1.Start();
            }
            catch
            {

            }
        }

       
    }

    public class LichHenJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //var a = 1;
        }
    }
}
