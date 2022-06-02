using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;

using libDM_DonVi;

namespace libDM_Kho
{
    public class classDM_Kho
    {
        private  SsoftvnContext db;
        public classDM_Kho(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public  DM_Kho Select_DMKho(Guid id)
        {
          
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_Kho.Find(id);
            }
        }
        private bool DM_KhoExists(Guid id)
        {
           
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_Kho.Count(e => e.ID == id) > 0;
            }
        }
        public  DM_Kho Get(Expression<Func<DM_Kho, bool>> query)
        {
           
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_Kho.FirstOrDefault();
                else
                    return db.DM_Kho.Where(query).FirstOrDefault();
            }
        }
        public  IQueryable<DM_Kho> Gets(Expression<Func<DM_Kho, bool>> query)
        {
           
            if (db == null)
            {
                return null;
            }
            else
            {
                IQueryable<DM_Kho> values = db.DM_Kho.Where(query);
                return values;
            }
        }
        public  List<DM_Kho> Select_DMKhos_IDDonVi(Guid idDonVi)
        {
            List<Kho_DonVi> lstKho_DVs = Select_KhoDonVis_IDDonVi(idDonVi);
            if (lstKho_DVs != null && lstKho_DVs.Count > 0)
            {
                List<Guid> lstIDKhos = lstKho_DVs.Select(p => p.ID_Kho).Distinct().ToList();
                IQueryable<DM_Kho> lstDMKhos = Gets(p => lstIDKhos.Contains(p.ID));
                if (lstDMKhos != null)
                {
                    return lstDMKhos.OrderBy(p => p.MaKho).ToList();
                }
            }
            return null;
        }
        public  List<DM_Kho> Select_DMKhos_User(string UserName)
        {
            List<DM_DonVi> lstDonVis = classDM_DonVi.Select_DMDonVi_User(UserName);
            if (lstDonVis != null && lstDonVis.Count > 0)
            {
                List<Guid> lstIDDVis = lstDonVis.Select(p => p.ID).Distinct().ToList();

               
                if (db == null)
                {
                    return null;
                }
                else
                {
                    IQueryable<Kho_DonVi> values = db.Kho_DonVi.Where(p => lstIDDVis.Contains(p.ID_DonVi));
                    if (values != null && values.Count() > 0)
                    {
                        List<Guid> lstIDKhos = values.Select(p => p.ID_Kho).Distinct().ToList();
                        IQueryable<DM_Kho> lstKhos = db.DM_Kho.Where(p => lstIDKhos.Contains(p.ID));
                        if (lstKhos != null)
                            return lstKhos.OrderBy(p => p.MaKho).ToList();
                    }
                }
            }
            return null;
        }

        public  Kho_DonVi select_KhoDonVi(Guid ID_DonVi)
        {
          
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Kho_DonVi.Where(p => p.ID_DonVi == ID_DonVi).FirstOrDefault();
            }
        }
        #endregion

        #region insert
        public  string Add_DMKho(DM_Kho objAdd)
        {
          
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DM_Kho.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }

        public  string Add_Kho_DonVi(Kho_DonVi objAdd)
        {
            
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.Kho_DonVi.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }
        #endregion

        #region update
        public  string Update_DMKho(DM_Kho objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    DM_Kho objUpd = db.DM_Kho.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.GhiChu = objNew.GhiChu;
                        objUpd.DiaDiem = objNew.DiaDiem;
                        objUpd.MaKho = objNew.MaKho;
                        objUpd.TenKho = objNew.TenKho;

                        objUpd.NgaySua = objNew.NgaySua;
                        objUpd.NgayTao = objNew.NgayTao;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        objUpd.NguoiTao = objNew.NguoiTao;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        //
                        #endregion

                        #region Kho_DonVi
                        if (objNew.Kho_DonVi != null && objNew.Kho_DonVi.Count > 0)
                        {
                            List<Kho_DonVi> lstDonVis = objNew.Kho_DonVi.ToList();

                            List<Guid> previousIds = db.Kho_DonVi.Where(ep => ep.ID_Kho == objUpd.ID).Select(ep => ep.ID).ToList();
                            List<Guid> currentIds = lstDonVis.Select(o => o.ID).ToList();
                            List<Guid> deletedIds = previousIds.Except(currentIds).ToList();
                            foreach (var del_Id in deletedIds)
                            {
                                Kho_DonVi deletedOrderDetail = db.Kho_DonVi.Where(od => od.ID_Kho == objUpd.ID && od.ID == del_Id).Single();
                                db.Entry(deletedOrderDetail).State = EntityState.Deleted;
                            }
                            foreach (var itemDetail in lstDonVis)
                            {
                                if (previousIds.Contains(itemDetail.ID) && currentIds.Contains(itemDetail.ID))
                                {
                                    Kho_DonVi objUpd_MV = db.Kho_DonVi.Where(od => od.ID_Kho == objUpd.ID && od.ID == itemDetail.ID).Single();
                                    if (objUpd_MV != null)
                                    {
                                        objUpd_MV.ID_DonVi = itemDetail.ID_DonVi;
                                        //
                                        db.Entry(objUpd_MV).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        objUpd_MV = new Kho_DonVi();
                                        objUpd_MV.ID = Guid.NewGuid();
                                        objUpd_MV.ID_Kho = objUpd.ID;
                                        objUpd_MV.ID_DonVi = itemDetail.ID_DonVi;

                                        //
                                        db.Entry(objUpd_MV).State = EntityState.Added;
                                    }
                                }
                                else if (!previousIds.Contains(itemDetail.ID) && currentIds.Contains(itemDetail.ID))
                                {
                                    Kho_DonVi objUpd_MV = new Kho_DonVi();
                                    objUpd_MV.ID = Guid.NewGuid();
                                    objUpd_MV.ID_Kho = objUpd.ID;
                                    objUpd_MV.ID_DonVi = itemDetail.ID_DonVi;
                                    //
                                    db.Entry(objUpd_MV).State = EntityState.Added;
                                }
                            }
                        }
                        else
                        {
                            List<Kho_DonVi> lstKhoDVis = db.Kho_DonVi.Where(p => p.ID_Kho == objUpd.ID).ToList();
                            if (lstKhoDVis != null && lstKhoDVis.Count > 0)
                                db.Kho_DonVi.RemoveRange(lstKhoDVis);
                        }
                        #endregion
                        //
                        db.SaveChanges();
                    }
                    else
                    {
                        return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }
        #endregion

        #region delete
        static string CheckDelete_DMKho(DM_Kho objDel)
        {
            string strCheck = string.Empty;
            if (objDel != null)
            {
                if (objDel.BH_HoaDon_ChiTiet != null && objDel.BH_HoaDon_ChiTiet.Count > 0)
                {
                    strCheck = "Kho này đã được sử dụng để lập 'Hóa đơn'. Không thể xóa.";
                    return strCheck;
                }
                if (objDel.DM_GiaBan_ChiTiet != null && objDel.DM_GiaBan_ChiTiet.Count > 0)
                {
                    strCheck = "Kho này đã được sử dụng để khai báo  chương trình giá bán. Không thể xóa.";
                    return strCheck;
                }
                if (objDel.Kho_TonKhoKhoiTao != null && objDel.Kho_TonKhoKhoiTao.Count > 0)
                {
                    strCheck = "Kho này đã được sử dụng để khai báo 'Tồn kho khởi tạo' hàng hóa. Không thể xóa.";
                    return strCheck;
                }
            }
            return strCheck;
        }

        public  string Delete_DMKho(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                DM_Kho objDel = db.DM_Kho.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_DMKho(objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty)
                    {
                        try
                        {
                            IQueryable<Kho_DonVi> lstDVis = db.Kho_DonVi.Where(p => p.ID_Kho == id);
                            if (lstDVis != null && lstDVis.ToList().Count > 0)
                                db.Kho_DonVi.RemoveRange(lstDVis);
                            //
                            db.DM_Kho.Remove(objDel);
                            db.SaveChanges();
                        }
                        catch (Exception exxx)
                        {
                            return exxx.Message;
                        }
                    }
                    else
                    {
                        return strCheckDel;
                    }
                }
            }
            return string.Empty;
        }
        #endregion

        #region Kho_DonVi
        public  IQueryable<Kho_DonVi> GetKho_DonVis(Expression<Func<Kho_DonVi, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                IQueryable<Kho_DonVi> values = db.Kho_DonVi.Where(query);
                return values;
            }
        }

        public  List<Kho_DonVi> Select_KhoDonVis_IDDonVi(Guid idDonVi)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                IQueryable<Kho_DonVi> values = db.Kho_DonVi.Where(p => p.ID_DonVi == idDonVi);
                if (values != null && values.Count() > 0)
                    return values.ToList();
                else
                    return null;
            }
        }

        #endregion 
    }
}
