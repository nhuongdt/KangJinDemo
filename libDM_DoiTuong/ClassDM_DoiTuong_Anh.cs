using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;

namespace libDM_DoiTuong
{
    public class ClassDM_DoiTuong_Anh
    {
        private SsoftvnContext db;
        public ClassDM_DoiTuong_Anh(SsoftvnContext _db)
        {
            db = _db;
        }
        public string Add_Image(Model.DM_DoiTuong_Anh objAnh)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DM_DoiTuong_Anh.Add(objAnh);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in dbEx.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), dbEx);
                }
            }
            return strErr;
        }

        public List<DM_DoiTuong_Anh> Gets(Expression<Func<DM_DoiTuong_Anh, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_DoiTuong_Anh.ToList();
                else
                    return db.DM_DoiTuong_Anh.Where(query).ToList();
            }
        }
    }
}
