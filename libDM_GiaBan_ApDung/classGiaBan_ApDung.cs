using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libDM_GiaBan_ApDung
{
    public class classGiaBan_ApDung
    {
        private SsoftvnContext db;

        public classGiaBan_ApDung(SsoftvnContext _db)
        {
            db = _db;
        }
        public string Add_GBApDung(DM_GiaBan_ApDung objApDungAdd)
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
                    db.DM_GiaBan_ApDung.Add(objApDungAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message + ex.InnerException;
                }
            }
            return strErr;
        }

        public string Update_GBApDung(DM_GiaBan_ApDung obj)
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
                    #region DM_GiaBan_ApDung
                    DM_GiaBan_ApDung objUpd = db.DM_GiaBan_ApDung.Find(obj.ID);
                    //objUpd.MaViTri = obj.MaViTri;
                    if (objUpd != null)
                    {
                        objUpd.ID = obj.ID;
                        objUpd.ID_GiaBan = obj.ID_GiaBan;
                        objUpd.ID_DonVi = obj.ID_DonVi;
                        objUpd.ID_NhanVien = obj.ID_NhanVien;
                        objUpd.ID_NhomKhachHang = obj.ID_NhomKhachHang;

                        #endregion
                        db.Entry(objUpd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        DM_GiaBan_ApDung apdung = new DM_GiaBan_ApDung();
                        apdung.ID = Guid.NewGuid();
                        apdung.ID_GiaBan = obj.ID_GiaBan;
                        apdung.ID_DonVi = obj.ID_DonVi;
                        apdung.ID_NhanVien = obj.ID_NhanVien;
                        apdung.ID_NhomKhachHang = obj.ID_NhomKhachHang;
                        Add_GBApDung(apdung);
                    }


                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
    }
}
