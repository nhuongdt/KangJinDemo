using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace libHT_NguoiDung
{
    public class classHT_Quyen
    {
        private SsoftvnContext db;
        public classHT_Quyen(SsoftvnContext _db)
        {
            db = _db;
        }
        public IQueryable<HT_Quyen> Gets(Expression<Func<HT_Quyen, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.HT_Quyen;
                else
                    return db.HT_Quyen.Where(query);
            }
        }
    }
}
