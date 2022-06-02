using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Web.API;

namespace Model.Web.Service
{
   public  class ProductService
    {
        private SsoftvnWebContext db;
        public ProductService()
        {
            db = SystemDBContext.GetDBContext();
        }

        public IQueryable<DM_SanPham> GetAll()
        {
            return db.DM_SanPham.AsQueryable();
        }
    }
}
