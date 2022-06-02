using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;

namespace libDM_HangHoa
{
    class classCongDoan_DichVu
    {
        #region select
        public static IQueryable<CongDoan_DichVu> Gets(Expression<Func<CongDoan_DichVu, bool>> query)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return null;
            }
            else
            {
                IQueryable<CongDoan_DichVu> values = db.CongDoan_DichVu.Where(query);
                return values;
            }
        }

        public static List<CongDoan_DichVu> Select_CongDoan_DichVus_IDDichVu(Guid idDichVu)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return null;
            }
            else
            {
                IQueryable<CongDoan_DichVu> values = db.CongDoan_DichVu.Where(p => p.ID_DichVu == idDichVu);
                if (values != null && values.Count() > 0)
                    return values.ToList();
                else
                    return null;
            }
        }

        public static CongDoan_DichVu Select_CongDoan_DichVu(Guid id)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.CongDoan_DichVu.Find(id);
            }
        }
        private bool CongDoan_DichVuExists(Guid idHHDV, Guid IDCongDoan)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.CongDoan_DichVu.Count(e => e.ID_CongDoan == IDCongDoan && e.ID_DichVu == idHHDV) > 0;
            }
        }

        private bool CongDoan_DichVuExists(Guid id)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.CongDoan_DichVu.Count(e => e.ID == id) > 0;
            }
        }
        public static CongDoan_DichVu Get(Expression<Func<CongDoan_DichVu, bool>> query)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.CongDoan_DichVu.Where(query).FirstOrDefault();
            }
        }

        #endregion

        #region insert
        public static string Add_CongDoan_DichVu(CongDoan_DichVu objAdd)
        {
            string strErr = string.Empty;
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.CongDoan_DichVu.Add(objAdd);
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
        public static string Update_CongDoan_DichVus_IDDichVu(List<CongDoan_DichVu> lstCongDoans, Guid idDichVu)
        {
            string strErr = string.Empty;
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
                return strErr;
            }
            else
            {
                try
                {
                    if (lstCongDoans != null && lstCongDoans.Count() > 0)
                    {
                        #region update
                        List<Guid> previousIds = db.CongDoan_DichVu.Where(p => p.ID_DichVu == idDichVu).Select(ep => ep.ID).ToList();
                        List<Guid> currentIds = lstCongDoans.Select(o => o.ID).ToList();
                        List<Guid> deletedIds = previousIds.Except(currentIds).ToList();
                        foreach (var del_Id in deletedIds)
                        {
                            CongDoan_DichVu deletedOrderDetail = db.CongDoan_DichVu.Where(od => od.ID_DichVu == idDichVu && od.ID == del_Id).Single();
                            db.Entry(deletedOrderDetail).State = EntityState.Deleted;
                        }
                        foreach (var itemDetail in lstCongDoans)
                        {
                            if (previousIds.Contains(itemDetail.ID) && currentIds.Contains(itemDetail.ID))
                            {
                                CongDoan_DichVu objUpd_MV = db.CongDoan_DichVu.Where(od => od.ID_DichVu == idDichVu && od.ID == itemDetail.ID).Single();
                                if (objUpd_MV != null)
                                {
                                    objUpd_MV.ID_CongDoan = itemDetail.ID_CongDoan;
                                    objUpd_MV.SoPhutThucHien = itemDetail.SoPhutThucHien;
                                    objUpd_MV.STT = itemDetail.STT;
                                    objUpd_MV.ThoiGian = itemDetail.ThoiGian;
                                    //
                                    db.Entry(objUpd_MV).State = EntityState.Modified;
                                }
                                else
                                {
                                    objUpd_MV = new CongDoan_DichVu();
                                    objUpd_MV.ID = Guid.NewGuid();
                                    objUpd_MV.ID_CongDoan = itemDetail.ID_CongDoan;
                                    objUpd_MV.ID_DichVu = idDichVu;
                                    objUpd_MV.SoPhutThucHien = itemDetail.SoPhutThucHien;
                                    objUpd_MV.STT = itemDetail.STT;
                                    objUpd_MV.ThoiGian = itemDetail.ThoiGian;
                                    //
                                    db.Entry(objUpd_MV).State = EntityState.Added;
                                }
                            }
                            else if (!previousIds.Contains(itemDetail.ID) && currentIds.Contains(itemDetail.ID))
                            {
                                CongDoan_DichVu objUpd_MV = new CongDoan_DichVu();
                                objUpd_MV.ID = Guid.NewGuid();
                                objUpd_MV.ID_CongDoan = itemDetail.ID_CongDoan;
                                objUpd_MV.SoPhutThucHien = itemDetail.SoPhutThucHien;
                                objUpd_MV.STT = itemDetail.STT;
                                objUpd_MV.ThoiGian = itemDetail.ThoiGian;
                                objUpd_MV.ID_DichVu = idDichVu;
                                //
                                db.Entry(objUpd_MV).State = EntityState.Added;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        List<CongDoan_DichVu> lstCongDoanDels = db.CongDoan_DichVu.Where(p => p.ID_DichVu == idDichVu).ToList();
                        if (lstCongDoanDels != null && lstCongDoanDels.Count > 0)
                            db.CongDoan_DichVu.RemoveRange(lstCongDoanDels);
                    }
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }


        public static string Update_CongDoan_DichVu(CongDoan_DichVu objNew)
        {
            string strErr = string.Empty;
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    CongDoan_DichVu objUpd = db.CongDoan_DichVu.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.ID_CongDoan = objNew.ID_CongDoan;
                        objUpd.ID_DichVu = objNew.ID_DichVu;
                        objUpd.SoPhutThucHien = objNew.SoPhutThucHien;
                        objUpd.STT = objNew.STT;
                        objUpd.ThoiGian = objNew.ThoiGian;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        //
                        #endregion
                        //
                        db.SaveChanges();
                    }
                    else
                    {
                        #region insert new
                        strErr = "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                        return strErr;
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion

        #region delete
        public static string Delete_CongDoan_DichVu(Guid id)
        {
            string strErr = string.Empty;
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return null;
            }
            else
            {
                CongDoan_DichVu objDel = db.CongDoan_DichVu.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.CongDoan_DichVu.Remove(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        return exxx.Message;
                    }
                }
            }
            return strErr;
        }

        public static string Delete_CongDoan_DichVus_IDDichVu(Guid idDichVu)
        {
            string strErr = string.Empty;
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return null;
            }
            else
            {
                List<CongDoan_DichVu> lstDels = db.CongDoan_DichVu.Where(p => p.ID_DichVu == idDichVu).ToList();
                if (lstDels != null && lstDels.Count() > 0)
                {
                    try
                    {
                        db.CongDoan_DichVu.RemoveRange(lstDels);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        return exxx.Message;
                    }
                }
            }
            return strErr;
        }
        #endregion
    }
}
