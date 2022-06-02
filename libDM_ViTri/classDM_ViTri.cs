using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace libDM_ViTri
{
    public class classDM_ViTri
    {
        private SsoftvnContext db;

        public classDM_ViTri(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DM_ViTri Select_ViTri(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_ViTri.Find(id);
            }
        }

        public List<DM_ViTri> Gets(Expression<Func<DM_ViTri, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_ViTri.ToList();
                else
                    return db.DM_ViTri.Where(query).ToList();
            }
        }

        /// <summary>
        /// get infor basic in DM_ViTri
        /// </summary>
        /// <param name="query"></param>
        /// <returns> List<DM_ViTriDTO></returns>
        public List<DM_ViTriDTO> GetViTri_KhuVuc()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = (from vt in db.DM_ViTri
                            join kv in db.DM_KhuVuc on vt.ID_KhuVuc equals kv.ID into VT_KV
                            from vtkv in VT_KV.DefaultIfEmpty()
                            where vt.TinhTrang != true
                            select new DM_ViTriDTO
                            {
                                ID = vt.ID,
                                MaViTri = vt.MaViTri,
                                TenViTri = vt.TenViTri,
                                TenKhuVuc = vtkv == null ? "" : vtkv.TenKhuVuc,
                                ID_KhuVuc = vt.ID_KhuVuc,
                                GhiChu = vt.GhiChu,
                            }).ToList();
                return data;
            }
        }

        public List<DM_ViTriDTO> getlistViTri_where(string maHoaDon, string idkhuvuc)
        {
            List<DM_ViTriDTO> lst = new List<DM_ViTriDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from vt in db.DM_ViTri
                          join kv in db.DM_KhuVuc on vt.ID_KhuVuc equals kv.ID
                          where vt.TinhTrang != true
                          select new
                          {
                              ID = vt.ID,
                              MaViTri = vt.MaViTri,
                              TenViTri = vt.TenViTri,
                              GhiChu = vt.GhiChu,
                              TenKhuVuc = kv.TenKhuVuc,
                              ID_KhuVuc = kv.ID
                          };
                if (idkhuvuc != "undefined")
                {
                    tbl = tbl.Where(hd => hd.ID_KhuVuc.ToString().Contains(idkhuvuc.ToString()));
                }
                string stSearch = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                foreach (var item in tbl)
                {
                    string tenPB = CommonStatic.ConvertToUnSign(item.TenViTri).ToLower();
                    string tenKV = CommonStatic.ConvertToUnSign(item.TenKhuVuc).ToLower();
                    string tenPBsplit = CommonStatic.GetCharsStart(item.TenViTri).ToLower(); // get ki tu dau cua chuoi
                    string tenKVsplit = CommonStatic.GetCharsStart(item.TenKhuVuc).ToLower();
                    if (tenPB.Contains(stSearch) || tenKV.Contains(@stSearch) || tenPBsplit.Contains(@stSearch) || tenKVsplit.Contains(@stSearch))
                    {
                        DM_ViTriDTO dM_ViTriDTO = new DM_ViTriDTO();
                        dM_ViTriDTO.ID = item.ID;
                        dM_ViTriDTO.MaViTri = item.MaViTri;
                        dM_ViTriDTO.TenViTri = item.TenViTri;
                        dM_ViTriDTO.GhiChu = item.GhiChu;
                        dM_ViTriDTO.TenKhuVuc = item.TenKhuVuc;
                        dM_ViTriDTO.ID_KhuVuc = item.ID_KhuVuc;
                        lst.Add(dM_ViTriDTO);
                    }
                }
                return lst;
            }
        }

        public DM_ViTri Get(Expression<Func<DM_ViTri, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_ViTri.Where(query).FirstOrDefault();
            }
        }

        public DM_ViTriDTO GetFistDM_ViTri(Expression<Func<DM_ViTri, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_ViTri.OrderBy(x => x.TenViTri).Select(vt => new DM_ViTriDTO { ID = vt.ID, TenViTri = vt.TenViTri }).FirstOrDefault();
            }
        }
        public bool DM_ViTriExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_ViTri.Count(e => e.ID == id) > 0;
            }
        }

        public bool Check_TenVitriExist(string tenViTri, Guid? id_khuvuc, Guid? id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                if (id != null && id.ToString() != "null")
                {
                    // check ma khi update: Neu trung ma va khac ID --> return true
                    return db.DM_ViTri.Count(e => e.TenViTri.Trim() == tenViTri.Trim() && e.ID_KhuVuc == id_khuvuc && e.TinhTrang != true) > 0;
                }
                else
                {
                    return db.DM_ViTri.Count(e => e.TenViTri.Trim() == tenViTri.Trim() && e.ID_KhuVuc == id_khuvuc && e.TinhTrang != true) > 0;
                }
            }
        }

        public bool Check_TenVitriExistEdit(string tenViTri, Guid? id_khuvuc, Guid? id_vitri)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                if (id_vitri != null && id_vitri.ToString() != "null")
                {
                    // check ma khi update: Neu trung ma va khac ID --> return true
                    return db.DM_ViTri.Count(e => e.TenViTri.Trim() == tenViTri.Trim() && e.ID_KhuVuc == id_khuvuc && e.TinhTrang != true && e.ID != id_vitri) > 0;
                }
                else
                {
                    return db.DM_ViTri.Count(e => e.TenViTri.Trim() == tenViTri.Trim() && e.ID_KhuVuc == id_khuvuc && e.TinhTrang != true) > 0;
                }
            }
        }
        #endregion

        #region insert
        public string Add_ViTri(DM_ViTri objVTAdd)
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
                    db.DM_ViTri.Add(objVTAdd);
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
        public string Update_ViTri(DM_ViTri obj)
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
                    DM_ViTri objUpd = db.DM_ViTri.Find(obj.ID);
                    //objUpd.MaViTri = obj.MaViTri;
                    objUpd.ID = obj.ID;
                    objUpd.TenViTri = obj.TenViTri;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.ID_KhuVuc = obj.ID_KhuVuc;
                    objUpd.TinhTrang = obj.TinhTrang;
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

        #region delete
        public string Delete_ViTri(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_ViTri objDel = db.DM_ViTri.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.DM_ViTri.Remove(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        strErr = exxx.Message + exxx.InnerException;
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
        #endregion

        #region
        public string GetAutoCode()
        {
            string format = "{0:0000}";
            string mahoadon = "PB0";
            string madv = db.DM_ViTri.Where(p => p.MaViTri.Contains(mahoadon)).Where(p => p.MaViTri.Length == 7).OrderByDescending(p => p.MaViTri).Select(p => p.MaViTri).FirstOrDefault();
            if (madv == null)
            {
                mahoadon = mahoadon + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(mahoadon.Length, 4)) + 1;
                mahoadon = mahoadon + string.Format(format, tempstt);
            }
            return mahoadon;
        }
        #endregion
    }
}
