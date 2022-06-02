using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity.Validation;
using System.Linq.Expressions;
using System.Data.Entity;

namespace libDM_LoaiTuVanLichHen
{
    public class class_DM_LoaiTuVanLichHen
    {
        private SsoftvnContext db;
        public class_DM_LoaiTuVanLichHen(SsoftvnContext _db)
        {
            db = _db;

        }

        public DM_LoaiTuVanLichHen Select_LoaiTuVanLichHen(Guid id)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LoaiTuVanLichHen.Find(id);
            }

        }

        public DM_LoaiTuVanLichHen Select_LoaiTV_LichHen(Guid? id)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LoaiTuVanLichHen.Find(id);
            }

        }

        public List<DM_LoaiTuVanLichHen> Gets(Expression<Func<DM_LoaiTuVanLichHen, bool>> query)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_LoaiTuVanLichHen.ToList();
                else
                    return db.DM_LoaiTuVanLichHen.Where(query).ToList();
            }

        }
        public List<DM_LoaiTuVanLichHenSelect_Tpv> GetList_TVLH(string TenLoaiTV)
        {

            List<DM_LoaiTuVanLichHenSelect_Tpv> lst = new List<DM_LoaiTuVanLichHenSelect_Tpv>();
            var tbl = from lh in db.DM_LoaiTuVanLichHen.Where(x => x.TuVan_LichHen == 1)
                      select new DM_LoaiTuVanLichHenSelect_Tpv
                      {
                          ID = lh.ID,
                          TenLoaiTuVanLichHen = lh.TenLoaiTuVanLichHen
                      };
            var tbl_foemat = tbl.AsEnumerable().Select(t => new DM_LoaiTuVanLichHenSelect_Tpv
            {
                ID = t.ID,
                TenLoaiTuVanLichHen = t.TenLoaiTuVanLichHen,
                TenLoaiTuVanLichHen_CV = CommonStatic.ConvertToUnSign(t.TenLoaiTuVanLichHen).ToLower(),
                TenLoaiTuVanLichHen_GC = CommonStatic.GetCharsStart(t.TenLoaiTuVanLichHen).ToLower()
            });
            if (TenLoaiTV != null & TenLoaiTV != "" & TenLoaiTV != "null")
            {
                TenLoaiTV = CommonStatic.ConvertToUnSign(TenLoaiTV).ToLower();
                tbl_foemat = tbl_foemat.Where(x => x.TenLoaiTuVanLichHen_CV.Contains(TenLoaiTV) || x.TenLoaiTuVanLichHen_GC.Contains(TenLoaiTV));
            }
            try
            {
                lst = tbl_foemat.OrderByDescending(x => x.TenLoaiTuVanLichHen).ToList();
            }
            catch
            {

            }
            return lst;

        }

        #region insert
        public string Add_TuVanLichHen(DM_LoaiTuVanLichHen objAdd)
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
                    db.DM_LoaiTuVanLichHen.Add(objAdd);
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
        #endregion

        #region update
        public string Update_LoaiTuVan(DM_LoaiTuVanLichHen obj)
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
                    #region DM_LoaiTuVanLichHen
                    DM_LoaiTuVanLichHen objUpd = db.DM_LoaiTuVanLichHen.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TenLoaiTuVanLichHen = obj.TenLoaiTuVanLichHen;
                    objUpd.TuVan_LichHen = 1;
                    objUpd.NguoiTao = obj.NguoiTao;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgayTao = objUpd.NgayTao;
                    objUpd.NgaySua = DateTime.Now;
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

        public string Update_LoaiLichHen(DM_LoaiTuVanLichHen obj)
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
                    #region DM_LoaiTuVanLichHen
                    DM_LoaiTuVanLichHen objUpd = db.DM_LoaiTuVanLichHen.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TenLoaiTuVanLichHen = obj.TenLoaiTuVanLichHen;
                    objUpd.TuVan_LichHen = 2;
                    objUpd.NguoiTao = obj.NguoiTao;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgayTao = objUpd.NgayTao;
                    objUpd.NgaySua = DateTime.Now;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;

        }
        public string Update_LoaiPhanHoi(DM_LoaiTuVanLichHen obj)
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
                    #region DM_LoaiTuVanLichHen
                    DM_LoaiTuVanLichHen objUpd = db.DM_LoaiTuVanLichHen.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TenLoaiTuVanLichHen = obj.TenLoaiTuVanLichHen;
                    objUpd.TuVan_LichHen = 3;
                    objUpd.NguoiTao = obj.NguoiTao;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgayTao = objUpd.NgayTao;
                    objUpd.NgaySua = DateTime.Now;
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

        public string Update_LoaiCongViec(DM_LoaiTuVanLichHen obj)
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
                    DM_LoaiTuVanLichHen objUpd = db.DM_LoaiTuVanLichHen.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TenLoaiTuVanLichHen = obj.TenLoaiTuVanLichHen;
                    objUpd.TuVan_LichHen = obj.TuVan_LichHen;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = DateTime.Now;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        /// <summary>
        /// dùng chung được cho LoaiCV, TuVan, LichHen, PhanHoi
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Update_LoaiTuVanLichHen(DM_LoaiTuVanLichHen obj)
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
                    DM_LoaiTuVanLichHen objUpd = db.DM_LoaiTuVanLichHen.Find(obj.ID);
                    objUpd.TenLoaiTuVanLichHen = obj.TenLoaiTuVanLichHen;
                    objUpd.TuVan_LichHen = obj.TuVan_LichHen;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = DateTime.Now;
                    db.Entry(objUpd).State = EntityState.Modified;
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


        public string Delete_LoaiTuVan(Guid id)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_LoaiTuVanLichHen objDel = db.DM_LoaiTuVanLichHen.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        // update ID_LoaiTuVan = null in ChamSoKhachHanngs
                        db.Database.ExecuteSqlCommand("Update ChamSocKhachHangs set ID_LoaiTuVan= null where ID_LoaiTuVan='" + id + "'");
                        objDel.TrangThai = 0;
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        strErr = exxx.Message;
                        return strErr;
                    }
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }

        public bool Check_TenLoaiTuVanLichHenExist(string tenLoaiTuVan)
        {


            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoaiTuVanLichHen.Count(e => e.TenLoaiTuVanLichHen.Contains(tenLoaiTuVan)) > 0;
            }

        }

    }
    public class DM_LoaiTuVanLichHenDTO : DM_LoaiTuVanLichHen { }

    public class DM_LoaiTuVanLichHenSelect_Tpv
    {
        public Guid ID { get; set; }
        public string TenLoaiTuVanLichHen { get; set; }
        public string TenLoaiTuVanLichHen_CV { get; set; }
        public string TenLoaiTuVanLichHen_GC { get; set; }
    }
}
