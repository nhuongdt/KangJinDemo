using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;
using System.Linq.Expressions;
using libDM_DonVi;
using System.Data.SqlClient;

namespace libDM_NhomDoiTuong
{
    public class ClassDM_NhomDoiTuong
    {
        private SsoftvnContext db;
        public ClassDM_NhomDoiTuong(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public List<DM_NhomDoiTuong> Gets(Expression<Func<DM_NhomDoiTuong, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query != null)
                    return db.DM_NhomDoiTuong.Where(query).OrderBy(p => p.MaNhomDoiTuong).ToList();
                else
                    return db.DM_NhomDoiTuong.OrderBy(p => p.MaNhomDoiTuong).ToList();
            }
        }

        public List<DM_NhomDoiTuong> Select_NhomDoiTuong_IDDonVi(Guid idDonVi)
        {
            var classNhomDoiTuong_DonVi = new classNhomDoiTuong_DonVi(db);
            List<NhomDoiTuong_DonVi> lstDatas = classNhomDoiTuong_DonVi.Select_NhomDoiTuong_IDDonVi(idDonVi);
            if (lstDatas != null && lstDatas.Count > 0)
            {
                List<Guid> lstIDs = lstDatas.Select(p => p.ID_NhomDoiTuong).Distinct().ToList();
                var lstReturns = Gets(p => lstIDs.Contains(p.ID));
                return lstReturns;
            }
            else
                return null;
        }

        public List<DM_NhomDoiTuong> Select_NhomDoiTuong_User(string userName)
        {
            List<DM_DonVi> lstDatas = classDM_DonVi.Select_DMDonVi_User(userName);
            if (lstDatas != null && lstDatas.Count > 0)
            {
                List<DM_NhomDoiTuong> lstReturns = new List<DM_NhomDoiTuong>();
                foreach (DM_DonVi item in lstDatas)
                {
                    List<DM_NhomDoiTuong> lstItems = Select_NhomDoiTuong_IDDonVi(item.ID);
                    if (lstItems != null && lstItems.Count > 0)
                    {
                        lstReturns.AddRange(lstItems);
                    }
                }
                return lstReturns.Distinct().ToList();
            }
            else
                return null;
        }

        public DM_NhomDoiTuong Select_NhomDoiTuong(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_NhomDoiTuong.Find(id);
            }
        }

        private bool DM_NhomDoiTuongExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_NhomDoiTuong.Count(e => e.ID == id) > 0;
            }
        }
        #endregion

        #region insert
        public string Add_NhomDoiTuong(DM_NhomDoiTuong objAdd)
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
                    db.DM_NhomDoiTuong.Add(objAdd);
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
        public string Update_NhomDoiTuong(DM_NhomDoiTuong objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    DM_NhomDoiTuong objUpd = db.DM_NhomDoiTuong.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        var sql = string.Concat("Update DM_DoiTuong set TenNhomDoiTuongs =Replace(TenNhomDoiTuongs,N'",
                             objUpd.TenNhomDoiTuong, "',N'" + objNew.TenNhomDoiTuong, "') where IDNhomDoiTuongs like '%'+'", objNew.ID,
                              "' + '%' OR ID_NhomDoiTuong like '%'+'", objNew.ID, "' + '%' ; " +
                              "");
                        db.Database.ExecuteSqlCommand(sql);

                        objUpd.GhiChu = objNew.GhiChu;
                        objUpd.LoaiDoiTuong = objNew.LoaiDoiTuong;
                        if (objNew.MaNhomDoiTuong != string.Empty && objNew.MaNhomDoiTuong != null)
                        {
                            objUpd.MaNhomDoiTuong = objNew.MaNhomDoiTuong; // NOT UPDATE 
                        }
                        objUpd.NgaySua = DateTime.Now;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        objUpd.TenNhomDoiTuong = objNew.TenNhomDoiTuong;
                        objUpd.TenNhomDoiTuong_KhongDau = objNew.TenNhomDoiTuong_KhongDau;
                        objUpd.TenNhomDoiTuong_KyTuDau = objNew.TenNhomDoiTuong_KyTuDau;
                        db.Entry(objUpd).State = EntityState.Modified;

                        #endregion
                        #region NhomDoiTuong_DonVi
                        if (objNew.NhomDoiTuong_DonVi != null && objNew.NhomDoiTuong_DonVi.Count > 0)
                        {
                            List<NhomDoiTuong_DonVi> lstDonVis = objNew.NhomDoiTuong_DonVi.ToList();

                            List<Guid> previousIds = db.NhomDoiTuong_DonVi.Where(ep => ep.ID_NhomDoiTuong == objNew.ID).Select(ep => ep.ID).ToList();
                            List<Guid> currentIds = lstDonVis.Select(o => o.ID).ToList();
                            List<Guid> deletedIds = previousIds.Except(currentIds).ToList();
                            foreach (var del_Id in deletedIds)
                            {
                                NhomDoiTuong_DonVi deletedOrderDetail = db.NhomDoiTuong_DonVi.Where(od => od.ID_NhomDoiTuong == objNew.ID && od.ID == del_Id).Single();
                                db.Entry(deletedOrderDetail).State = EntityState.Deleted;
                            }
                            foreach (var orderDetail in lstDonVis)
                            {
                                if (previousIds.Contains(orderDetail.ID) && currentIds.Contains(orderDetail.ID))
                                {
                                    NhomDoiTuong_DonVi objUpd_DVi = db.NhomDoiTuong_DonVi.Where(od => od.ID_NhomDoiTuong == objNew.ID && od.ID == orderDetail.ID).Single();
                                    if (objUpd_DVi != null)
                                    {
                                        objUpd_DVi.ID_DonVi = orderDetail.ID_DonVi;
                                        db.Entry(objUpd_DVi).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        orderDetail.ID_NhomDoiTuong = objNew.ID;
                                        db.Entry(orderDetail).State = EntityState.Added;
                                    }
                                }
                                else if (!previousIds.Contains(orderDetail.ID) && currentIds.Contains(orderDetail.ID))
                                {
                                    orderDetail.ID_NhomDoiTuong = objNew.ID;
                                    db.Entry(orderDetail).State = EntityState.Added;
                                }
                            }
                        }
                        else
                        {
                            List<NhomDoiTuong_DonVi> lstDonVis = db.NhomDoiTuong_DonVi.Where(p => p.ID_NhomDoiTuong == objNew.ID).ToList();
                            if (lstDonVis != null && lstDonVis.Count > 0)
                                db.NhomDoiTuong_DonVi.RemoveRange(lstDonVis);
                        }
                        #endregion
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
        public string Delete_NhomDoiTuong(Guid id)
        {
            string strDel = string.Empty;
            try
            {
                if (strDel == string.Empty)
                {
                    SqlParameter prmt = new SqlParameter("ID_NhomDoiTuong", id);
                    db.Database.ExecuteSqlCommand("exec delete_NhomDoiTuong @ID_NhomDoiTuong", prmt);
                }
                return strDel;
            }
            catch (Exception ex)
            {
                return string.Concat("DM_NhomDoiTuongAPI_DeleteDM_NhomDoiTuong: ", ex.Message, ex.Message);
            }
        }

        #endregion

        #region fun other
        public string GetautoCodeNhomDT(int loaiDoiTuong)
        {
            string format = "{0:0000}";
            string autoCode = string.Empty;

            switch (loaiDoiTuong)
            {
                case 1:
                    autoCode = "NKH0";
                    break;
                case 2:
                    autoCode = "NNCC";
                    break;
                default:
                    autoCode = "NTD0";
                    break;
            }
            string sCode = db.DM_NhomDoiTuong.Where(p => p.MaNhomDoiTuong.Contains(autoCode)).Where(p => p.MaNhomDoiTuong.Length == 8)
                .OrderByDescending(p => p.MaNhomDoiTuong).Select(p => p.MaNhomDoiTuong).FirstOrDefault();
            if (sCode == null)
            {
                autoCode = autoCode + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(sCode.Substring(autoCode.Length, 4)) + 1;
                autoCode = autoCode + string.Format(format, tempstt);
            }
            return autoCode;
        }
        #endregion
    }
}
