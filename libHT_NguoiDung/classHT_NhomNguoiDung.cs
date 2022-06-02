using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.SqlClient;

namespace libHT_NguoiDung
{
    public class classHT_NhomNguoiDung
    {
        private SsoftvnContext db;

        public classHT_NhomNguoiDung(SsoftvnContext _db)
        {
            db = _db;
        }
        #region Select
        public  IQueryable<HT_NhomNguoiDung> Gets(Expression<Func<HT_NhomNguoiDung, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NhomNguoiDung.Where(query);
            }
        }
        public  HT_NhomNguoiDung Select_NhomNguoiDung(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NhomNguoiDung.Find(id);
            }
        }

        /// <summary>
        /// get list quyen by ID_NguoiDung and ID_DonVi {ID, MaQuyen}
        /// </summary>
        /// <param name="idNguoiDung"></param>
        /// <param name="idChiNhanh"></param>
        /// <returns></returns>
        public  List<HT_Quyen_NhomDTO> SP_GetQuyen_ByIDNguoiDung(string idNguoiDung, string idChiNhanh)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_NguoiDung", idNguoiDung));
                    lstParam.Add(new SqlParameter("ID_DonVi", idChiNhanh));
                    return db.Database.SqlQuery<HT_Quyen_NhomDTO>("EXEC SP_GetQuyen_ByIDNguoiDung @ID_NguoiDung, @ID_DonVi", lstParam.ToArray()).ToList();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetQuyen_ByIDNguoiDung: " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// get list role {ID, MaQuyen} by where
        /// </summary>
        /// <param name="whereSql"></param>
        /// <returns></returns>
        public  List<HT_Quyen_NhomDTO> SP_GetListQuyen_Where(string whereSql)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    SqlParameter param = new SqlParameter("Where", whereSql);
                    return db.Database.SqlQuery<HT_Quyen_NhomDTO>("EXEC SP_GetListQuyen_Where @Where", param).ToList();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetListQuyen_Where: "+ ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        public  List<HT_NhomNguoiDungDTO> getallvaitro()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nd in db.HT_NhomNguoiDung
                          select new
                          {
                              nd.ID,
                              nd.TenNhom,
                              nd.MaNhom,
                          };
                var list = tbl.Select(s =>
                            new HT_NhomNguoiDungDTO
                            {
                                ID = s.ID,
                                TenNhom = s.TenNhom,
                                MaNhom = s.MaNhom,
                            }).ToList();

                return list;
            }

        }

        public  HT_NhomNguoiDung Get(Expression<Func<HT_NhomNguoiDung, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NhomNguoiDung.Where(query).FirstOrDefault();
            }
        }

        private bool HT_NhomNguoiDungExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.HT_NhomNguoiDung.Count(e => e.ID == id) > 0;
            }
        }

        public  List<string> GetListQuyenChaFromListQuyen(List<string> listQuyenInput)
        {
            //List<string> listResult = new List<string>();
            while(true)
            {
                List<string> lstQuyenCha = db.HT_Quyen.Where(p => listQuyenInput.Contains(p.MaQuyen)).Select(p => p.QuyenCha).Distinct().ToList();
                List<string> add = lstQuyenCha.Where(p=>p != "").Except(listQuyenInput).ToList();
                if (add.Count == 0)
                    break;
                else
                {
                    listQuyenInput.AddRange(add);
                }
            }
            return listQuyenInput;
        }
        #endregion

        #region Insert
        public  string Add_NhomNguoiDung(HT_NhomNguoiDung objUserAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    db.HT_NhomNguoiDung.Add(objUserAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        public  string AddQuyenNhomNew(IEnumerable<HT_Quyen_Nhom> list)
        {
            db.HT_Quyen_Nhom.AddRange(list);
            db.SaveChanges();
            return "";
        }

        public  string Add_QuyenNhom(HT_Quyen_Nhom objUserAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    db.HT_Quyen_Nhom.Add(objUserAdd);
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
        public  string Update_NhomNguoiDung(HT_NhomNguoiDung objNew)
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
                    HT_NhomNguoiDung objUpd = db.HT_NhomNguoiDung.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.MaNhom = objNew.MaNhom;
                        objUpd.MoTa = objNew.MoTa;
                        objUpd.TenNhom = objNew.TenNhom;

                        objUpd.NgaySua = objNew.NgaySua;
                        objUpd.NgayTao = objNew.NgayTao;
                        objUpd.NguoiSua = objNew.NguoiSua;
                        objUpd.NguoiTao = objNew.NguoiTao;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion
                        //
                        #region HT_Quyen_Nhom
                        if (objNew.HT_Quyen_Nhom != null && objNew.HT_Quyen_Nhom.Count > 0)
                        {
                            List<HT_Quyen_Nhom> lstDonVis = objNew.HT_Quyen_Nhom.ToList();

                            List<Guid> previousIds = db.HT_Quyen_Nhom.Where(ep => ep.ID_NhomNguoiDung == objNew.ID).Select(ep => ep.ID).ToList();
                            List<Guid> currentIds = lstDonVis.Select(o => o.ID).ToList();
                            List<Guid> deletedIds = previousIds.Except(currentIds).ToList();
                            foreach (var del_Id in deletedIds)
                            {
                                HT_Quyen_Nhom deletedOrderDetail = db.HT_Quyen_Nhom.Where(od => od.ID_NhomNguoiDung == objNew.ID && od.ID == del_Id).Single();
                                db.Entry(deletedOrderDetail).State = EntityState.Deleted;
                            }
                            foreach (var orderDetail in lstDonVis)
                            {
                                if (previousIds.Contains(orderDetail.ID) && currentIds.Contains(orderDetail.ID))
                                {
                                    HT_Quyen_Nhom objUpd_Quyen = db.HT_Quyen_Nhom.Where(od => od.ID_NhomNguoiDung == objNew.ID && od.ID == orderDetail.ID).Single();
                                    if (objUpd_Quyen != null)
                                    {
                                        objUpd_Quyen.MaQuyen = orderDetail.MaQuyen;
                                        //
                                        db.Entry(objUpd_Quyen).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        objUpd_Quyen = new HT_Quyen_Nhom();
                                        objUpd_Quyen.ID = Guid.NewGuid();
                                        objUpd_Quyen.ID_NhomNguoiDung = objUpd.ID;
                                        objUpd_Quyen.MaQuyen = orderDetail.MaQuyen;
                                        //
                                        db.Entry(orderDetail).State = EntityState.Added;
                                    }
                                }
                                else if (!previousIds.Contains(orderDetail.ID) && currentIds.Contains(orderDetail.ID))
                                {
                                    HT_Quyen_Nhom objUpd_Quyen = new HT_Quyen_Nhom();
                                    objUpd_Quyen.ID = Guid.NewGuid();
                                    objUpd_Quyen.ID_NhomNguoiDung = objUpd.ID;
                                    objUpd_Quyen.MaQuyen = orderDetail.MaQuyen;
                                    //
                                    db.Entry(orderDetail).State = EntityState.Added;
                                }
                            }
                        }
                        else
                        {
                            List<HT_Quyen_Nhom> lstQuyen_NhomDels = db.HT_Quyen_Nhom.Where(p => p.ID_NhomNguoiDung == objNew.ID).ToList();
                            if (lstQuyen_NhomDels != null && lstQuyen_NhomDels.Count > 0)
                                db.HT_Quyen_Nhom.RemoveRange(lstQuyen_NhomDels);
                        }
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
         string CheckDelete_NhomNguoiDung(HT_NhomNguoiDung objDel)
        {
            string strCheck = string.Empty;
            if (objDel != null)
            {
                if (objDel.HT_NguoiDung_Nhom != null && objDel.HT_NguoiDung_Nhom.Count > 0)
                {
                    strCheck = "Nhóm người dùng này đã được sử dụng để khai báo danh mục 'Người dùng'. Không thể xóa.";
                    return strCheck;
                }
            }
            return strCheck;
        }

        public  string Delete_NhomNguoiDung(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return null;
            }
            else
            {
                HT_NhomNguoiDung objDel = db.HT_NhomNguoiDung.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_NhomNguoiDung(objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty || strCheckDel.Trim() == "")
                    {
                        try
                        {
                            //HT_Quyen_Nhom
                            if (objDel.HT_Quyen_Nhom != null && objDel.HT_Quyen_Nhom.Count > 0)
                                db.HT_Quyen_Nhom.RemoveRange(objDel.HT_Quyen_Nhom.ToList());

                            //HT_NhomNguoiDung
                            db.HT_NhomNguoiDung.Remove(objDel);
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
            return strErr;
        }

        #endregion

        #region HT_Quyen / Nhom user
        public  List<HT_Quyen> Select_Quyens_IDNhomNguoiDung(Guid idNhomNguoiDung)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                List<HT_Quyen> lstDatas = db.HT_Quyen_Nhom.Where(p => p.ID_NhomNguoiDung == idNhomNguoiDung).Select(p => p.HT_Quyen).Distinct().ToList();
                return lstDatas;
            }
        }
        #endregion
    }
}
