using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Web.API;

namespace Model.Web.Service
{
   public class TinhThanhService
    {
        private SsoftvnWebContext db;
        public TinhThanhService()
        {
            db = SystemDBContext.GetDBContext();
        }
        public IQueryable<DM_TinhThanh> GetAll()
        {
            return db.DM_TinhThanh.OrderBy(o=>o.Priority).ThenBy(o=>o.TenTinhThanh).AsQueryable();
        }

        public DM_TinhThanh GetByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            else
                return  db.DM_TinhThanh.FirstOrDefault(o => o.MaTinhThanh.Equals(key));
        }
    }
}
