using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.SqlClient;

namespace libNS_NhanVien
{
    public class classNS_NhanVien_ChiTiet
    {
        private readonly SsoftvnContext db;
        public classNS_NhanVien_ChiTiet(SsoftvnContext _db)
        {

            db = _db;
        }
        #region select
        public IQueryable<ChietKhauMacDinh_NhanVien> Gets(Expression<Func<ChietKhauMacDinh_NhanVien, bool>> query)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.ChietKhauMacDinh_NhanVien;
                else
                    return db.ChietKhauMacDinh_NhanVien.Where(query);
            }
        }
        public IQueryable<NhanVienChiTietDTO> SelectChiTiet(string _id)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                //NhanVienChiTietDTO chitiet = new NhanVienChiTietDTO();
                if (_id != "undefined")
                {
                    Guid id = new Guid(_id);
                    return db.ChietKhauMacDinh_NhanVien.Where(p => p.ID_NhanVien == id).GroupJoin(db.DonViQuiDois, ct => ct.ID_DonViQuiDoi, qd => qd.ID, (ct, qd) => new { ct, qd })
                        .SelectMany(s => s.qd.DefaultIfEmpty(), (s, qd) => new
                        {
                            ID = s.ct.ID,
                            ID_HangHoa = qd.ID_HangHoa,
                            MaHangHoa = qd.MaHangHoa,
                            DonViTinh = qd.TenDonViTinh,
                            GiaBan = qd.GiaBan,
                            ChietKhau = s.ct.ChietKhau,
                            TuVan = s.ct.ChietKhau_TuVan,
                            YeuCau = s.ct.ChietKhau_YeuCau,
                        }).GroupJoin(db.DM_HangHoa, s => s.ID_HangHoa, hh => hh.ID, (s, hh) => new { s, hh })
                        .SelectMany(p => p.hh.DefaultIfEmpty(), (p, hh) => new NhanVienChiTietDTO
                        {
                            ID = p.s.ID,
                            MaHangHoa = p.s.MaHangHoa,
                            TenHangHoa = hh.TenHangHoa,
                            GiaBan = p.s.GiaBan,
                            DonViTinh = p.s.DonViTinh,
                            ChietKhau = p.s.ChietKhau,
                            TuVan = p.s.TuVan,
                            YeuCau = p.s.YeuCau
                        }).OrderBy(p => p.TenHangHoa);
                }
                else
                {
                    return null;
                }
            }

        }

        public List<Report_NhomHangHoa> getList_ID_NhomHangHoa(List<Report_NhomHangHoa> lst, Guid? ID_NhomHang)
        {

            Report_NhomHangHoa DM = new Report_NhomHangHoa();
            DM.ID_NhomHangHoa = ID_NhomHang;
            lst.Add(DM);
            var tb1 = from nh1 in db.DM_NhomHangHoa
                      where nh1.ID_Parent == ID_NhomHang
                      select new
                      {
                          ID_NhomHangHoa = nh1.ID
                      };
            foreach (var item in tb1)
            {
                ID_NhomHang = item.ID_NhomHangHoa;
                lst = getList_ID_NhomHangHoa(lst, ID_NhomHang);
            }
            return lst;
        }
        public List<NhanVienChiTietDTO> GetNhanVien_NhomHang(Guid ID_NhanVien, Guid? ID_NhomHang, string maHH, Guid? ID_DonVi)
        {
            List<NhanVienChiTietDTO> lst = new List<NhanVienChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from ctCK in db.ChietKhauMacDinh_NhanVien
                          join dvt in db.DonViQuiDois on ctCK.ID_DonViQuiDoi equals dvt.ID
                          join hh in db.DM_HangHoa on dvt.ID_HangHoa equals hh.ID
                          join nhh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nhh.ID into kk
                          from nhh in kk.DefaultIfEmpty()
                          where ctCK.ID_NhanVien == ID_NhanVien & ctCK.ID_DonVi == ID_DonVi
                          select new NhanVienChiTietDTO
                          {
                              ID = ctCK.ID,
                              IDQuyDoi = dvt.ID,
                              MaHangHoa = dvt.MaHangHoa,
                              TenHangHoa = hh.TenHangHoa,
                              TenHangHoa_KhongDau = hh.TenHangHoa_KhongDau,
                              TenHangHoa_KyTuDau = hh.TenHangHoa_KyTuDau,
                              ChietKhau = ctCK.ChietKhau,
                              LaPTChietKhau = ctCK.LaPhanTram,
                              YeuCau = ctCK.ChietKhau_YeuCau,
                              LaPTYeuCau = ctCK.LaPhanTram_YeuCau,
                              TuVan = ctCK.ChietKhau_TuVan,
                              LaPTTuVan = ctCK.LaPhanTram_TuVan,
                              ID_NhanVien = ctCK.ID_NhanVien,
                              ID_NhomHang = hh.ID_NhomHang,
                              GiaBan = dvt.GiaBan
                          };
                if (ID_NhomHang != null)
                {
                    List<Report_NhomHangHoa> lst_NHH = new List<Report_NhomHangHoa>();
                    lst_NHH = getList_ID_NhomHangHoa(lst_NHH, ID_NhomHang);
                    List<string> lstID = new List<string>();
                    foreach (var item in lst_NHH)
                    {
                        lstID.Add(item.ID_NhomHangHoa.ToString());
                    }
                    tbl = tbl.Where(x => lstID.Contains(x.ID_NhomHang.ToString()));
                }
                if (maHH != null & maHH != "" & maHH != "null")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    string maHH1 = CommonStatic.ConvertToUnSign(maHH).ToUpper();
                    tbl = tbl.Where(x => x.MaHangHoa.Contains(@maHH1) || x.TenHangHoa_KhongDau.Contains(@maHH) || x.TenHangHoa_KyTuDau.Contains(@maHH));
                }
                try
                {
                    lst = tbl.OrderByDescending(x => x.MaHangHoa).ToList();
                }
                catch
                {

                }
                return lst;
            }
        }

        public List<ChietKhauMacDinh_NhanVienPRC> GetCaiDatChietKhau_HangHoa(Param_ChietKhauNhomHang lstParam)
        {
            var idNhoms = string.Empty;
            if (lstParam.ID_NhomHangs != null && lstParam.ID_NhomHangs.Count > 0)
            {
                idNhoms = string.Join(",", lstParam.ID_NhomHangs);
            }
            List<SqlParameter> paramSQL = new List<SqlParameter>();
            paramSQL.Add(new SqlParameter("ID_DonVi", lstParam.ID_DonVi));
            paramSQL.Add(new SqlParameter("ID_NhanVien", lstParam.ID_NhanVien));
            paramSQL.Add(new SqlParameter("IDNhomHangs", idNhoms));
            paramSQL.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSQL.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSQL.Add(new SqlParameter("PageSize", lstParam.PageSize));
            return db.Database.SqlQuery<ChietKhauMacDinh_NhanVienPRC>("getList_HoaHongNhanVien @ID_DonVi, @ID_NhanVien, @IDNhomHangs, @TextSearch, @CurrentPage, @PageSize", paramSQL.ToArray()).ToList();
        }

        public List<NhanVienChiTietDTO_Excel> GetNhanVien_NhomHang_excel(Guid ID_NhanVien, Guid? ID_NhomHang, string maHH, Guid? ID_DonVi)
        {
            List<NhanVienChiTietDTO_Excel> lst = new List<NhanVienChiTietDTO_Excel>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from ctCK in db.ChietKhauMacDinh_NhanVien
                          join dvt in db.DonViQuiDois on ctCK.ID_DonViQuiDoi equals dvt.ID
                          join hh in db.DM_HangHoa on dvt.ID_HangHoa equals hh.ID
                          join nhh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nhh.ID into kk
                          from nhh in kk.DefaultIfEmpty()
                          where ctCK.ID_NhanVien == ID_NhanVien & ctCK.ID_DonVi == ID_DonVi
                          select new NhanVienChiTietDTO_Excel
                          {
                              ID = ctCK.ID,
                              IDQuyDoi = dvt.ID,
                              MaHangHoa = dvt.MaHangHoa,
                              TenHangHoa = hh.TenHangHoa,
                              TenHangHoa_KhongDau = hh.TenHangHoa_KhongDau,
                              TenHangHoa_KyTuDau = hh.TenHangHoa_KyTuDau,
                              ChietKhau = ctCK.ChietKhau,
                              ChietKhau_text = ctCK.LaPhanTram == true ? ctCK.ChietKhau + " %" : ctCK.ChietKhau + " vnđ",
                              LaPTChietKhau = ctCK.LaPhanTram,
                              YeuCau = ctCK.ChietKhau_YeuCau,
                              YeuCau_text = ctCK.LaPhanTram_YeuCau == true ? ctCK.ChietKhau_YeuCau + " %" : ctCK.ChietKhau_YeuCau + " vnđ",
                              LaPTYeuCau = ctCK.LaPhanTram_YeuCau,
                              TuVan = ctCK.ChietKhau_TuVan,
                              TuVan_text = ctCK.LaPhanTram_TuVan == true ? ctCK.ChietKhau_TuVan + " %" : ctCK.ChietKhau_TuVan + " vnđ",
                              LaPTTuVan = ctCK.LaPhanTram_TuVan,
                              ID_NhanVien = ctCK.ID_NhanVien,
                              ID_NhomHang = hh.ID_NhomHang,
                              GiaBan = dvt.GiaBan
                          };
                if (ID_NhomHang != null)
                {
                    List<Report_NhomHangHoa> lst_NHH = new List<Report_NhomHangHoa>();
                    lst_NHH = getList_ID_NhomHangHoa(lst_NHH, ID_NhomHang);
                    List<string> lstID = new List<string>();
                    foreach (var item in lst_NHH)
                    {
                        lstID.Add(item.ID_NhomHangHoa.ToString());
                    }
                    tbl = tbl.Where(x => lstID.Contains(x.ID_NhomHang.ToString()));
                }
                if (maHH != null & maHH != "" & maHH != "null")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    string maHH1 = CommonStatic.ConvertToUnSign(maHH).ToUpper();
                    tbl = tbl.Where(x => x.MaHangHoa.Contains(@maHH1) || x.TenHangHoa_KhongDau.Contains(@maHH) || x.TenHangHoa_KyTuDau.Contains(@maHH));
                }
                try
                {
                    lst = tbl.OrderByDescending(x => x.MaHangHoa).ToList();
                }
                catch
                {

                }
                return lst;

            }
        }
        #endregion

        #region update
        public bool AddChiTietByIDNhom(Guid idnhomhanghoa, Guid idnhanvien, Guid iddonvi)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                try
                {
                    List<Report_NhomHangHoa> lst_NHH = new List<Report_NhomHangHoa>();
                    lst_NHH = getList_ID_NhomHangHoa(lst_NHH, idnhomhanghoa);
                    List<string> lstID = new List<string>();
                    foreach (var item in lst_NHH)
                    {
                        lstID.Add(item.ID_NhomHangHoa.ToString());
                    }
                    List<Guid> listidhanghoa = db.DM_HangHoa.Where(p => lstID.Contains(p.ID_NhomHang.ToString())).Select(p => p.ID).ToList();
                    List<Guid> listiddvqd = db.ChietKhauMacDinh_NhanVien.Where(p => p.ID_NhanVien == idnhanvien).Select(p => p.ID_DonViQuiDoi).ToList();
                    List<DonViQuiDoi> listqd = db.DonViQuiDois.Where(p => listidhanghoa.Contains(p.ID_HangHoa)).Where(p => !listiddvqd.Contains(p.ID)).ToList();
                    foreach (var item in listqd)
                    {
                        ChietKhauMacDinh_NhanVien ct = new ChietKhauMacDinh_NhanVien();
                        ct.ID = Guid.NewGuid();
                        ct.ID_DonVi = iddonvi;
                        ct.ChietKhau = 0;
                        ct.ChietKhau_TuVan = 0;
                        ct.ChietKhau_YeuCau = 0;
                        ct.LaPhanTram = false;
                        ct.LaPhanTram_TuVan = false;
                        ct.LaPhanTram_YeuCau = false;
                        ct.ID_NhanVien = idnhanvien;
                        ct.ID_DonViQuiDoi = item.ID;
                        ct.NgayNhap = DateTime.Now;
                        db.ChietKhauMacDinh_NhanVien.Add(ct);
                        db.SaveChanges();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    string str = CookieStore.GetCookieAes("SubDomain");
                    CookieStore.WriteLog("AddChiTietByIDNhom(Guid idnhomhanghoa, Guid idnhanvien, Guid iddonvi): " + ex.InnerException + ex.Message, str);
                    return false;
                }
            }
        }
        /// <summary>
        /// Insert all products in idNhoms in ChietKhauMacDinh_NhanVien
        /// </summary>
        /// <param name="idNhomHHs"></param>
        /// <param name="idNhanVien"></param>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public bool SP_AddChietKhau_ByIDNhom(Param_ChietKhauNhomHang lst)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                try
                {
                    var idNhomHHs = string.Join(",", lst.ID_NhomHangs);
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_NhomHangs", idNhomHHs));
                    lstParam.Add(new SqlParameter("ID_NhanVien", lst.ID_NhanVien));
                    lstParam.Add(new SqlParameter("ID_DonVi", lst.ID_DonVi));
                    var data = db.Database.SqlQuery<ChietKhauMacDinh_NhanVien>("GetListIDQuiDoi_SetupHoaHongByNhom @ID_NhomHangs, @ID_NhanVien, @ID_DonVi", lstParam.ToArray()).ToList();
                    db.ChietKhauMacDinh_NhanVien.AddRange(data);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_AddChietKhau_ByIDNhom: " + ex.InnerException + ex.Message);
                    return false;
                }
            }

        }

        public bool AddChiTietBymaHH(string maHH, Guid idnhanvien, Guid iddonvi)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                try
                {
                    bool dk = false;
                    List<Guid> listiddvqd = db.DonViQuiDois.Where(p => p.MaHangHoa == maHH).Select(p => p.ID).ToList();
                    List<Guid> listiddvqd_ck = db.ChietKhauMacDinh_NhanVien.Where(p => p.ID_NhanVien == idnhanvien).Select(p => p.ID_DonViQuiDoi).ToList();
                    List<DonViQuiDoi> listqd = db.DonViQuiDois.Where(p => listiddvqd.Contains(p.ID)).Where(p => !listiddvqd_ck.Contains(p.ID)).ToList();
                    foreach (var item in listqd)
                    {
                        ChietKhauMacDinh_NhanVien ct = new ChietKhauMacDinh_NhanVien();
                        ct.ID = Guid.NewGuid();
                        ct.ID_DonVi = iddonvi;
                        ct.ChietKhau = 0;
                        ct.ChietKhau_TuVan = 0;
                        ct.ChietKhau_YeuCau = 0;
                        ct.LaPhanTram = false;
                        ct.LaPhanTram_TuVan = false;
                        ct.LaPhanTram_YeuCau = false;
                        ct.ID_NhanVien = idnhanvien;
                        ct.ID_DonViQuiDoi = item.ID;
                        ct.NgayNhap = DateTime.Now;
                        ct.LaPhanTram_BanGoi = false;
                        ct.ChietKhau_BanGoi = 0;
                        ct.TheoChietKhau_ThucHien = 0;// default tinhtheo GiaBan
                        db.ChietKhauMacDinh_NhanVien.Add(ct);
                        db.SaveChanges();
                        dk = true;
                    }
                    return dk;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("AddChiTietBymaHH " + ex.InnerException + ex.Message);
                    return false;
                }
            }
        }
        #endregion
        public string Update_ChietKhauCT(Guid id, double chietkhau, bool laphantram)
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
                    #region ChietKhauMacDinh_NhanVien
                    ChietKhauMacDinh_NhanVien objUpd = db.ChietKhauMacDinh_NhanVien.Find(id);
                    objUpd.ChietKhau = chietkhau;
                    objUpd.LaPhanTram = laphantram;

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

        public string Update_YeuCauCT(Guid id, double yeucau, bool laphantram_yeucau)
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
                    #region ChietKhauMacDinh_NhanVien
                    ChietKhauMacDinh_NhanVien objUpd = db.ChietKhauMacDinh_NhanVien.Find(id);
                    objUpd.ChietKhau_YeuCau = yeucau;
                    objUpd.LaPhanTram_YeuCau = laphantram_yeucau;
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

        public string Update_TuVanCT(Guid id, double tuvan, bool laphantram_tuvan)
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
                    #region ChietKhauMacDinh_NhanVien
                    ChietKhauMacDinh_NhanVien objUpd = db.ChietKhauMacDinh_NhanVien.Find(id);
                    objUpd.ChietKhau_TuVan = tuvan;
                    objUpd.LaPhanTram_TuVan = laphantram_tuvan;
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

        #region delete
        public bool deleteChiTietbyID(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                try
                {
                    ChietKhauMacDinh_NhanVien ct = db.ChietKhauMacDinh_NhanVien.Find(id);
                    db.ChietKhauMacDinh_NhanVien.Remove(ct);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    CookieStore.WriteLog("deleteChiTietbyID " + e.InnerException + e.Message);
                    return false;
                }
            }
        }
        #endregion

        #region "ChietKhau HoaDon"
        /// <summary>
        /// get ChietKhau HoaDon by ChiNhanh (all NhanVien)
        /// </summary>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public List<SP_GetChietKhauHoaDon> SP_Get_ChietKhauHoaDon_byDonVi(Guid? idDonVi)
        {
            if (db == null)
            {
                return new List<SP_GetChietKhauHoaDon>();
            }
            else
            {
                try
                {
                    SqlParameter param = new SqlParameter("ID_DonVi", idDonVi);
                    var data = db.Database.SqlQuery<SP_GetChietKhauHoaDon>("SP_Get_ChietKhauHoaDon_byDonVi @ID_DonVi", param).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_Get_ChietKhauHoaDon_byNhanVien: " + ex.InnerException + ex.Message);
                    return new List<SP_GetChietKhauHoaDon>();
                }
            }
        }

        public List<NS_NhanVien_DonVi> SP_GetChietKhauHoaDon_ChiTiet_byID(Guid idChietKhau, Guid idDonVi)
        {
            if (db == null)
            {
                return new List<NS_NhanVien_DonVi>();
            }
            else
            {
                try
                {
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_ChietKhauHoaDon", idChietKhau));
                    lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                    var data = db.Database.SqlQuery<NS_NhanVien_DonVi>("SP_GetChietKhauHoaDon_ChiTiet @ID_ChietKhauHoaDon, @ID_DonVi", lstParam.ToArray()).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetChietKhauHoaDon_ChiTiet_byID: " + ex.InnerException + ex.Message);
                    return new List<NS_NhanVien_DonVi>();
                }
            }

        }

        public string Add_ChietKhauHoaDon(ChietKhauMacDinh_HoaDon objAdd)
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
                    db.ChietKhauMacDinh_HoaDon.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.InnerException + ex.Message;
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.Add_ChietKhauHoaDon " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        public string Update_ChietKhauHoaDon(ChietKhauMacDinh_HoaDon objUpdate)
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
                    ChietKhauMacDinh_HoaDon objUpd = db.ChietKhauMacDinh_HoaDon.Find(objUpdate.ID);
                    objUpd.ID_DonVi = objUpdate.ID_DonVi;
                    objUpd.TinhChietKhauTheo = objUpdate.TinhChietKhauTheo;
                    objUpd.GiaTriChietKhau = objUpdate.GiaTriChietKhau;
                    objUpd.ChungTuApDung = objUpdate.ChungTuApDung;
                    objUpd.GhiChu = objUpdate.GhiChu;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.InnerException + ex.Message;
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.Update_ChietKhauHoaDon " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        public string Add_ChietKhauHoaDon_ChiTiet(ChietKhauMacDinh_HoaDon_ChiTiet objAdd)
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
                    db.ChietKhauMacDinh_HoaDon_ChiTiet.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.InnerException + ex.Message;
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.Add_ChietKhauHoaDon_ChiTiet " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        public List<NS_NhanVien_DonVi> CheckExist_ChietKhauHD_NhanVien(Param_GetChietKhauHoaDon lstParam)
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
                    var idNhanViens = lstParam.ID_NhanViens;
                    var idDonVi = lstParam.ID_DonVi;
                    var chungtu = lstParam.ChungTuApDung;
                    var idChietKhau = lstParam.ID_ChietKhauHoaDon;

                    List<SqlParameter> sqlParam = new List<SqlParameter>();
                    sqlParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                    sqlParam.Add(new SqlParameter("ID_NhanViens", idNhanViens));
                    sqlParam.Add(new SqlParameter("ChungTuApDung", chungtu));
                    sqlParam.Add(new SqlParameter("ID_ChietKhauHoaDon", idChietKhau));

                    var data = db.Database.SqlQuery<NS_NhanVien_DonVi>("EXEC SP_CheckExist_ChietKhauHoaDonNhanVien @ID_DonVi, @ID_NhanViens, @ChungTuApDung, @ID_ChietKhauHoaDon ", sqlParam.ToArray()).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.CheckExist_ChietKhauHD_NhanVien " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// update TrangThai = 0 in ChietKhauMacDinh_HoaDon (deleted)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Delete_ChietKhauHoaDon(Guid id)
        {
            string sErr = string.Empty;
            try
            {
                ChietKhauMacDinh_HoaDon data = db.ChietKhauMacDinh_HoaDon.Find(id);
                data.TrangThai = 0;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                sErr = "Delete_ChietKhauHoaDon: " + ex.Message + ex.InnerException;
            }
            return sErr;
        }

        /// <summary>
        /// delete list ChietKhauMacDinh_HoaDon_ChiTiet by ID_ChietKhauHoaDon
        /// </summary>
        /// <param name="idChietKhauHoaDon"></param>
        /// <returns></returns>
        public string Delete_ChietKhauHoaDon_ChiTiet(Guid idChietKhauHoaDon)
        {
            string sErr = string.Empty;
            try
            {
                IQueryable<ChietKhauMacDinh_HoaDon_ChiTiet> lstCT = db.ChietKhauMacDinh_HoaDon_ChiTiet.Where(p => p.ID_ChietKhauHoaDon == idChietKhauHoaDon);
                db.ChietKhauMacDinh_HoaDon_ChiTiet.RemoveRange(lstCT);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                sErr = "Delete_ChietKhauHoaDon_ChiTiet: " + ex.Message + ex.InnerException;
            }
            return sErr;
        }

        #endregion

        #region "ChietKhau DoanhThu"
        public List<SP_GetChietKhauDoanhThu> SP_Get_ChietKhauDoanhThu_byDonVi(Param_GetChietKhauHoaDon lstParam)
        {
            if (db == null)
            {
                return new List<SP_GetChietKhauDoanhThu>();
            }
            else
            {
                try
                {
                    List<SqlParameter> sql = new List<SqlParameter>();
                    sql.Add(new SqlParameter("ID_DonVi", lstParam.ID_DonVi));
                    sql.Add(new SqlParameter("LoaiNVApDung", lstParam.ApDungTuNgay));// muontam truong
                    sql.Add(new SqlParameter("TrangThaiConHan", lstParam.ChungTuApDung));// muontam truong
                    sql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
                    sql.Add(new SqlParameter("PageSize", lstParam.PageSize));
                    var data = db.Database.SqlQuery<SP_GetChietKhauDoanhThu>("SP_Get_ChietKhauDoanhThu_byDonVi @ID_DonVi," +
                        "@LoaiNVApDung, @TrangThaiConHan, @CurrentPage, @PageSize", sql.ToArray()).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_Get_ChietKhauDoanhThu_byDonVi: " + ex.InnerException + ex.Message);
                    return new List<SP_GetChietKhauDoanhThu>();
                }
            }
        }

        public List<ChietKhauDoanhThu_ChiTiet> SP_GetChietKhauDoanhThuChiTiet_byID(Guid idChietKhau)
        {

            if (db == null)
            {
                return new List<ChietKhauDoanhThu_ChiTiet>();
            }
            else
            {
                try
                {
                    SqlParameter param = new SqlParameter("ID_ChietKhauDoanhThu", idChietKhau);
                    var data = db.Database.SqlQuery<ChietKhauDoanhThu_ChiTiet>("SP_GetChietKhauDoanhThuChiTiet_byID @ID_ChietKhauDoanhThu", param).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetChietKhauDoanhThuChiTiet_byID: " + ex.InnerException + ex.Message);
                    return new List<ChietKhauDoanhThu_ChiTiet>();
                }
            }
        }

        public List<NS_NhanVien_DonVi> SP_GetChietKhauDoanhThuNhanVien_byID(Guid idChietKhau, Guid idDonVi)
        {
            if (db == null)
            {
                return new List<NS_NhanVien_DonVi>();
            }
            else
            {
                try
                {
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_ChietKhauDoanhThu", idChietKhau));
                    lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                    var data = db.Database.SqlQuery<NS_NhanVien_DonVi>("SP_GetChietKhauDoanhThuNhanVien_byID @ID_ChietKhauDoanhThu, @ID_DonVi", lstParam.ToArray()).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetChietKhauDoanhThuNhanVien_byID: " + ex.InnerException + ex.Message);
                    return new List<NS_NhanVien_DonVi>();
                }
            }
        }

        public string Add_ChietKhauDoanhThu(ChietKhauDoanhThu objAdd)
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
                    db.ChietKhauDoanhThu.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.InnerException + ex.Message;
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.Add_ChietKhauHoaDon " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        public string Update_ChietKhauDoanhThu(ChietKhauDoanhThu objUpdate)
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
                    ChietKhauDoanhThu objUpd = db.ChietKhauDoanhThu.Find(objUpdate.ID);
                    objUpd.ID_DonVi = objUpdate.ID_DonVi;
                    objUpd.TinhChietKhauTheo = objUpdate.TinhChietKhauTheo;
                    objUpd.ApDungTuNgay = objUpdate.ApDungTuNgay;
                    objUpd.ApDungDenNgay = objUpdate.ApDungDenNgay;
                    objUpd.GhiChu = objUpdate.GhiChu;
                    objUpd.LoaiNhanVienApDung = objUpdate.LoaiNhanVienApDung;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.InnerException + ex.Message;
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.Update_ChietKhauDoanhThu " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        public string Add_ChietKhauDoanhThu_ChiTiet(ChietKhauDoanhThu_ChiTiet objAdd)
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
                    db.ChietKhauDoanhThu_ChiTiet.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.InnerException + ex.Message;
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.Add_ChietKhauDoanhThu_ChiTiet " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        public string Add_ChietKhauDoanhThu_NhanVien(ChietKhauDoanhThu_NhanVien objAdd)
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
                    db.ChietKhauDoanhThu_NhanVien.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.InnerException + ex.Message;
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.Add_ChietKhauDoanhThu_NhanVien " + ex.InnerException + ex.Message);
                }
            }
            return strErr;
        }

        /// <summary>
        /// update TrangThai = 0 in ChietKhauDoanhThu (deleted)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Delete_ChietKhauDoanhThu(Guid id)
        {
            string sErr = string.Empty;
            try
            {
                ChietKhauDoanhThu data = db.ChietKhauDoanhThu.Find(id);
                data.TrangThai = 0;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                sErr = "Delete_ChietKhauDoanhThu: " + ex.Message + ex.InnerException;
            }
            return sErr;
        }

        /// <summary>
        /// delete list ChietKhauDoanhThu_ChiTiet by ID_ChietKhauDaonhThu
        /// </summary>
        /// <param name="idChietKhauDoanhThu"></param>
        /// <returns></returns>
        public string Delete_ChietKhauDoanhThu_ChiTiet(Guid idChietKhauDoanhThu)
        {
            string sErr = string.Empty;
            try
            {
                IQueryable<ChietKhauDoanhThu_ChiTiet> lstCT = db.ChietKhauDoanhThu_ChiTiet.Where(p => p.ID_ChietKhauDoanhThu == idChietKhauDoanhThu);
                db.ChietKhauDoanhThu_ChiTiet.RemoveRange(lstCT);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                sErr = "Delete_ChietKhauDoanhThu_ChiTiet: " + ex.Message + ex.InnerException;
            }
            return sErr;
        }

        /// <summary>
        /// delete list ChietKhauDoanhThu_NhanVien by ID_ChietKhauDaonhThu
        /// </summary>
        /// <param name="idChietKhauDoanhThu"></param>
        /// <returns></returns>
        public string Delete_ChietKhauDoanhThu_NhanVien(Guid idChietKhauDoanhThu)
        {
            string sErr = string.Empty;
            try
            {
                IQueryable<ChietKhauDoanhThu_NhanVien> lstCT = db.ChietKhauDoanhThu_NhanVien.Where(p => p.ID_ChietKhauDoanhThu == idChietKhauDoanhThu);
                db.ChietKhauDoanhThu_NhanVien.RemoveRange(lstCT);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                sErr = "Delete_ChietKhauDoanhThu_NhanVien: " + ex.Message + ex.InnerException;
            }
            return sErr;
        }

        public List<NS_NhanVien_DonVi> CheckExist_ChietKhauDoanhThu_NhanVien(Param_GetChietKhauHoaDon lstParam)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return new List<NS_NhanVien_DonVi>();
            }
            else
            {
                try
                {
                    var idNhanViens = lstParam.ID_NhanViens;
                    var idDonVi = lstParam.ID_DonVi;
                    var tuNgay = lstParam.ApDungTuNgay;
                    var denNgay = lstParam.ApDungDenNgay;
                    var idChietKhau = lstParam.ID_ChietKhauHoaDon;
                    var tinhCKTheo = lstParam.TinhChietKhauTheo;

                    List<SqlParameter> sqlParam = new List<SqlParameter>();
                    sqlParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                    sqlParam.Add(new SqlParameter("ID_NhanViens", idNhanViens));
                    sqlParam.Add(new SqlParameter("ApDungTuNgay", tuNgay));
                    sqlParam.Add(new SqlParameter("ApDungDenNgay", denNgay));
                    sqlParam.Add(new SqlParameter("ID_ChietKhauDoanhThu", idChietKhau));
                    sqlParam.Add(new SqlParameter("TinhChietKhauTheo", tinhCKTheo));
                    sqlParam.Add(new SqlParameter("LoaiNhanVienApDung", lstParam.LoaiNhanVienApDung));

                    var data = db.Database.SqlQuery<NS_NhanVien_DonVi>("SP_CheckExist_ChietKhauDoanhThuNhanVien @ID_DonVi, @ID_NhanViens," +
                        "@ApDungTuNgay,@ApDungDenNgay, @ID_ChietKhauDoanhThu, @TinhChietKhauTheo, @LoaiNhanVienApDung ", sqlParam.ToArray()).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("classNS_NhanVien_ChiTiet.CheckExist_ChietKhauDoanhThu_NhanVien " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        #endregion
    }


    public class NhanVienChiTietDTO
    {
        public Guid ID { get; set; }
        public Guid? IDQuyDoi { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public string MaHangHoa { get; set; }
        public double? GiaBan { get; set; }
        public string DonViTinh { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoa_KhongDau { get; set; }
        public string TenHangHoa_KyTuDau { get; set; }
        public double ChietKhau { get; set; }
        public double YeuCau { get; set; }
        public double TuVan { get; set; }
        public bool LaPTChietKhau { get; set; }
        public bool LaPTTuVan { get; set; }
        public bool LaPTYeuCau { get; set; }
    }
    public class ChietKhauMacDinh_NhanVienPRC
    {
        public Guid ID { get; set; }
        public Guid IDQuyDoi { get; set; }
        public Guid? ID_NhomHang { get; set; } // used to check when setup discount
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public double ChietKhau { get; set; }
        public bool LaPTChietKhau { get; set; }
        public double YeuCau { get; set; }
        public bool LaPTYeuCau { get; set; }
        public double TuVan { get; set; }
        public bool LaPTTuVan { get; set; }
        public double BanGoi { get; set; }
        public bool LaPTBanGoi { get; set; }
        public double GiaBan { get; set; }
        public int? TheoChietKhau_ThucHien { get; set; }

        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
    }
    public class ChietKhauMacDinh_NhanVien_ExcelPRC
    {
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string ChietKhau { get; set; }
        public string YeuCau { get; set; }
        public string TuVan { get; set; }
        public string BanGoi { get; set; }
        public double GiaBan { get; set; }
    }
    public class NhanVienChiTietDTO_Excel
    {
        public Guid ID { get; set; }
        public Guid? IDQuyDoi { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public string MaHangHoa { get; set; }
        public double? GiaBan { get; set; }
        public string DonViTinh { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoa_KhongDau { get; set; }
        public string TenHangHoa_KyTuDau { get; set; }
        public double ChietKhau { get; set; }
        public string ChietKhau_text { get; set; }
        public double YeuCau { get; set; }
        public string YeuCau_text { get; set; }
        public double TuVan { get; set; }
        public string TuVan_text { get; set; }
        public bool LaPTChietKhau { get; set; }
        public bool LaPTTuVan { get; set; }
        public bool LaPTYeuCau { get; set; }
    }
    public class Report_NhomHangHoa
    {
        public Guid? ID_NhomHangHoa { get; set; }
    }

    public class Param_GetChietKhauHoaDon
    {
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        // used to check exist ChietKhauHD_NhanVien
        public string ChungTuApDung { get; set; }
        public Guid? ID_ChietKhauHoaDon { get; set; }
        public string ID_NhanViens { get; set; }

        // used to check exist ChietKhauDoanhThu_NhanVien
        public string ApDungTuNgay { get; set; }
        public string ApDungDenNgay { get; set; }
        public int? TinhChietKhauTheo { get; set; }
        public int? LoaiNhanVienApDung { get; set; }
    }

    public class SP_GetChietKhauHoaDon
    {
        public Guid ID { get; set; }
        public double GiaTriChietKhau { get; set; }
        public int TinhChietKhauTheo { get; set; }
        public string ChungTuApDung { get; set; }
        public string GhiChu { get; set; }
        public bool? LaPhanTram { get; set; }
        public DateTime? NgayTao { get; set; }
        public List<NS_NhanVien_DonVi> NhanViens { get; set; }
    }

    public class SP_GetChietKhauDoanhThu
    {
        public Guid ID { get; set; }
        public int? LoaiNhanVienApDung { get; set; }
        public string Text_LoaiNhanVienApDung { get; set; }
        public int TinhChietKhauTheo { get; set; }
        public DateTime ApDungTuNgay { get; set; }
        public DateTime? ApDungDenNgay { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgayTao { get; set; }
        public List<NS_NhanVien_DonVi> NhanViens { get; set; }
        public List<ChietKhauDoanhThu_ChiTiet> DoanhThuChiTiet { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
    }

    public class Param_ChietKhauNhomHang
    {
        public Guid ID_DonVi { get; set; }
        public Guid ID_NhanVien { get; set; }
        public List<string> ID_NhomHangs { get; set; }
        public string TextSearch { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
