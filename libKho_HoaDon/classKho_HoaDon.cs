using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace libKho_HoaDon
{
    public class classKho_HoaDon
    {
        private SsoftvnContext db;
        public classKho_HoaDon(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public  Kho_HoaDon SelectKho_HoaDon(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Kho_HoaDon.Find(id);
            }
        }
        public  IQueryable<Kho_HoaDon> Gets(Expression<Func<Kho_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.Kho_HoaDon;
                else
                    return db.Kho_HoaDon.Where(query);
            }
        }

        public  Kho_HoaDon Get(Expression<Func<Kho_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Kho_HoaDon.Where(query).FirstOrDefault();
            }
        }

        public  bool Quy_HoaDOnExists(Guid id)
        {

            if (db == null)
            {
                return false;
            }
            else
            {

                return db.Quy_HoaDon.Count(e => e.ID == id) > 0;
            }
        }

        public  bool Check_MaSoQuyExist(string maHoaDon)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                return db.Kho_HoaDon.Count(e => e.MaHoaDon == maHoaDon) > 0;
            }
        }
        #endregion

        #region insert
        public  string Add_KhoHoaDon(Kho_HoaDon objKHDAdd)
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
                    db.Kho_HoaDon.Add(objKHDAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public  string Update_KhoHoaDon(Kho_HoaDon obj)
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
                    #region Kho_HoaDon
                    Kho_HoaDon objUpd = db.Kho_HoaDon.Find(obj.ID);
                    //objUpd.MaViTri = obj.MaViTri;
                    objUpd.ID = obj.ID;
                    objUpd.MaHoaDon = obj.MaHoaDon;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.LoaiChungTu = obj.LoaiChungTu;
                    objUpd.DienGiai = obj.DienGiai;

                    #endregion


                    db.Entry(objUpd).State = EntityState.Modified;
                    //
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion
    }

    public class Kho_HoaDonDTO
    {
        public Guid ID { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public int? LoaiChungTu { get; set; }
        public string DienGiai { get; set; }
        public double TongChenhLech { get; set; }
        public double SoLuongLechTang { get; set; }
        public double SoLuongLechGiam { get; set; }
    }
}
