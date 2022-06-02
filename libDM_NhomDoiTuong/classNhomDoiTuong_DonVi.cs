using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libDM_NhomDoiTuong
{
    public class classNhomDoiTuong_DonVi
    {
        private SsoftvnContext db;
        public classNhomDoiTuong_DonVi(SsoftvnContext _db)
        {
            db = _db;
        }

        public List<NhomDoiTuong_DonVi> Select_NhomDoiTuong_IDDonVi(Guid idDonVi)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.NhomDoiTuong_DonVi.Where(p => p.ID_DonVi == idDonVi).ToList();
            }
        }

        public string Add(NhomDoiTuong_DonVi objAdd)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    objAdd.ID = Guid.NewGuid();
                    db.NhomDoiTuong_DonVi.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }

        public string Delete(Guid idNhomDT)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    var data = from dt in db.NhomDoiTuong_DonVi where dt.ID_NhomDoiTuong == idNhomDT select dt;
                    if (data != null && data.Count() > 0)
                    {
                        db.NhomDoiTuong_DonVi.RemoveRange(data);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }
    }
}
