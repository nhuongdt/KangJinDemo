using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
namespace libDM_HangHoa
{
    public class classChietKhauMacDinh_NhanVien
    {
        private SsoftvnContext db;
        public classChietKhauMacDinh_NhanVien(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public ChietKhauMacDinh_NhanVien Select_ChietKhauMacDinh(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.ChietKhauMacDinh_NhanVien.Find(id);
            }
        }
        public IQueryable<ChietKhauMacDinh_NhanVien> Gets(Expression<Func<ChietKhauMacDinh_NhanVien, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.ChietKhauMacDinh_NhanVien.Where(query);
            }
        }
        public ChietKhauMacDinh_NhanVien Get(Expression<Func<ChietKhauMacDinh_NhanVien, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.ChietKhauMacDinh_NhanVien.Where(query).FirstOrDefault();
            }
        }
        #endregion
        #region khong su dung
        //public  List<ChietKhauMacDinh_NhanVien> Select_ChietKhauMacDinhs_HangHoa(Guid? idHangHoa, Guid? idNhomHH)
        //{
        //    
        //    if (db == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        if (idNhomHH != null)
        //        {
        //            return db.ChietKhauMacDinh_NhanVien.Where(p => p.TheoNhomHangHoa == true && p.ID_NhomHangHoa == idNhomHH).ToList();
        //        }
        //    }
        //    return null;
        //}
        //public  List<ChietKhauMacDinh_NhanVien> Select_ChietKhauMacDinhs_NhanVien(Guid? idNhanVien, Guid? idDonVi)
        //{
        //    
        //    if (db == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        if (idDonVi != null)
        //        {
        //            return db.ChietKhauMacDinh_NhanVien.Where(p => p.TheoNhomNhanVien == true && p.ID_NhomNhanVien == idDonVi).ToList();
        //        }
        //        else if (idNhanVien != null)
        //        {
        //            NS_NhanVien objNVien = db.NS_NhanVien.Find(idNhanVien.Value);
        //            if (objNVien != null)
        //            {
        //                List<ChietKhauMacDinh_NhanVien> lst_NViens = db.ChietKhauMacDinh_NhanVien.Where(p => p.TheoNhomNhanVien == false && p.ID_NhanVien == idNhanVien).ToList();
        //                IQueryable<NS_QuaTrinhCongTac> lstQTCongTacs = db.NS_QuaTrinhCongTac.Where(p => p.ID_NhanVien == idNhanVien);
        //                if (lstQTCongTacs != null && lstQTCongTacs.Count() > 0)
        //                {
        //                    List<Guid> lstIDDVis = lstQTCongTacs.Select(p => p.ID_DonVi).Distinct().ToList();
        //                    List<ChietKhauMacDinh_NhanVien> lst_NhomNVs = db.ChietKhauMacDinh_NhanVien.Where(p => p.TheoNhomNhanVien == true && p.ID_NhomNhanVien != null && lstIDDVis.Contains(p.ID_NhomNhanVien.Value)).ToList();
        //                    if (lst_NhomNVs != null)
        //                    {
        //                        if (lst_NViens != null && lst_NViens.Count() > 0)
        //                        {
        //                            lst_NViens.AddRange(lst_NhomNVs);
        //                        }
        //                        else
        //                        {
        //                            lst_NViens = new List<ChietKhauMacDinh_NhanVien>();
        //                            lst_NViens.AddRange(lst_NhomNVs);
        //                        }
        //                    }
        //                }
        //                return lst_NViens;
        //            }
        //        }
        //    }
        //    return null;
        //}
        #endregion
    }
}
