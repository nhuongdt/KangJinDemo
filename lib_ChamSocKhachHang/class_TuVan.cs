using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity.Validation;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.SqlClient;

namespace lib_ChamSocKhachHang
{
    public class class_TuVan
    {
        private SsoftvnContext db;
        public class_TuVan(SsoftvnContext _db)
        {
            db = _db;
        }
        public ChamSocKhachHang Select_PhieuTuVan(Guid? id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.ChamSocKhachHang.Find(id);
            }
        }

        public bool ChecLoaiCongViec(string loaicongviec)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoaiTuVanLichHen.Count(e => e.TenLoaiTuVanLichHen == loaicongviec.Trim() && e.TuVan_LichHen == 4) > 0;
            }
        }
        public bool ChecLoaiCongViecEdit(string loaicongviec, Guid idloaicv)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoaiTuVanLichHen.Count(e => e.TenLoaiTuVanLichHen == loaicongviec.Trim() && e.ID != idloaicv && e.TuVan_LichHen == 4) > 0;
            }
        }

        /// <summary>
        /// check exist LoaiCongViec in NS_CongViec_PhanLoai (if insert: assign ID ='null')
        /// </summary>
        /// <param name="loaicongviec"></param>
        /// <param name="idloaicv"></param>
        /// <returns>true/false</returns>

        public bool SP_Check_LoaiCongViec_Exist(string loaicongviec, Guid idloaicv, int loaiTuVan)
        {
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("LoaiCongViec", loaicongviec));
                lstParam.Add(new SqlParameter("ID_LoaiCongViec", idloaicv));
                lstParam.Add(new SqlParameter("LoaiTuVan_LichHen", loaiTuVan));

                var objReturn = db.Database.SqlQuery<Model.SP_ReturnBool>("EXEC SP_Check_LoaiCongViec_Exist @LoaiCongViec, @ID_LoaiCongViec, @LoaiTuVan_LichHen", lstParam.ToArray()).ToList();
                if (objReturn != null)
                {
                    return objReturn.FirstOrDefault().Exist;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_Check_LoaiCongViec_Exist: " + ex.InnerException + ex.Message + ex.HResult);
                return true;
            }
        }

        public List<ChamSocKhachHang> Gets(Expression<Func<ChamSocKhachHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.ChamSocKhachHang.ToList();
                else
                    return db.ChamSocKhachHang.Where(query).ToList();
            }
        }

        /// <summary>
        /// get list cong viec by where
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        public List<SP_NSCongViec> SP_GetListCongViec_Where(string txtSearch = null)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (txtSearch == null || txtSearch == "null")
                {
                    txtSearch = string.Empty;
                }

                SqlParameter param = new SqlParameter("txtSearch", txtSearch);
                var lst = db.Database.SqlQuery<SP_NSCongViec>("exec SP_GetListCongViec_Where @txtSearch", param).ToList();

                return lst;
            }
        }

        /// <summary>
        /// get list cong viec by ID_DoiTuong, only get CongViec of NhanVien (if login in or share work) not use
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="idNhanVien"></param>
        /// <returns></returns>
        public List<SP_NSCongViec> SP_GetListCongViec_ByDoiTuong(string idDoiTuong, string idNhanVien)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
                lstParam.Add(new SqlParameter("ID_NhanVien", idNhanVien));
                var lst = db.Database.SqlQuery<SP_NSCongViec>("exec SP_GetListCongViec_ByKhachHang @ID_DoiTuong, @ID_NhanVien", lstParam.ToArray()).ToList();
                return lst;
            }
        }

        /// <summary>
        /// get list cong viec by ID_DoiTuong, only get CongViec of NhanVien (if login in or share work)
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="idNhanVien"></param>
        /// <returns></returns>
        public List<SP_ChamSocKhachHang> SP_GetListCongViec_ByKhachHang(Guid idDoiTuong, Guid idDonVi)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                lstParam.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
                var lst = db.Database.SqlQuery<SP_ChamSocKhachHang>("exec SP_GetListCongViec_ByKhachHang @ID_DonVi, @ID_DoiTuong", lstParam.ToArray()).ToList();
                return lst;
            }
        }

        /// <summary>
        /// update ID_TrangThai in DM_DoiTuong where ID_DoiTuong
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="idTrangThai"></param>
        /// <returns></returns>
        public bool SP_UpdateCusType_DMDoiTuong(string idDoiTuong, Guid? idTrangThai)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                if (idTrangThai == Guid.Empty)
                {
                    idTrangThai = null;
                }
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
                lstParam.Add(new SqlParameter("ID_TrangThai", idTrangThai ?? (object)DBNull.Value));
                var lst = db.Database.SqlQuery<SP_NSCongViec>("exec SP_UpdateCusType_DMDoiTuong @ID_DoiTuong, @ID_TrangThai", lstParam.ToArray()).ToList();
                return true;
            }
        }

        #region insert
        public string Add_PhieuTuVan(ChamSocKhachHang objAdd)
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
                    db.ChamSocKhachHang.Add(objAdd);
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

        public string Add_LoaiCongViec(NS_CongViec_PhanLoai objAdd)
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
                    db.NS_CongViec_PhanLoai.Add(objAdd);
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

        public string AddNS_CongViecLT(ChamSocKhachHang objAdd)
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
                    db.ChamSocKhachHang.Add(objAdd);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    strErr = "errors";
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

        public string AddNS_CongViec(NS_CongViec objAdd)
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
                    db.NS_CongViec.Add(objAdd);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    strErr = "errors";
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

        public readonly string[] VietnameseSigns = new string[]

         { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};

        public List<ChamSocKhachHangDTO> GetListCongViec(string txtSearch, Guid iddonvi, string[] arrIDNhanVienPhoiHop, string[] arrIDDonVi, string[] arrLoaiCongViec, int loaiDoiTuong, string[] arrIDNhanVien, string[] arrMangIDNhanVien, DateTime dayStart, DateTime dayEnd, List<Guid?> TrangThaiKhach, int MucDoUuTien, List<int> TrangThaiCongViec)
        {
            if (db != null)
            {
                //var tbl = from cv in db.ChamSocKhachHang
                //          join loaicv in db.DM_LoaiTuVanLichHen on cv.ID_LoaiTuVan equals loaicv.ID
                //          join dt in db.DM_DoiTuong on cv.ID_KhachHang equals dt.ID
                //          join dm_nk in db.DM_NguonKhachHang on dt.ID_NguonKhach equals dm_nk.ID into dmnguonkhach
                //          from dM_nguon in dmnguonkhach.DefaultIfEmpty()
                //          join lienhe in db.DM_LienHe on cv.ID_LienHe equals lienhe.ID into dmlienhe
                //          from DMLIENHE in dmlienhe.DefaultIfEmpty()
                //          join nv in db.NS_NhanVien on cv.ID_NhanVien equals nv.ID into nhanvien
                //          from ns_nv in nhanvien.DefaultIfEmpty()
                //          join nvph in db.ChamSocKhachHang_NhanVien on cv.ID equals nvph.ID_ChamSocKhachHang into NVPH
                //          from nv_ph in NVPH.DefaultIfEmpty()
                //          where cv.PhanLoai == 4 && cv.TrangThai != ((int)(commonEnumHellper.TypeCongViec.daxoa)).ToString()
                //          orderby cv.NgayTao descending
                //          select new ChamSocKhachHangDTO()
                //          {
                //              ID = cv.ID,
                //              ID_DonVi = cv.ID_DonVi,
                //              ID_LoaiTuVan = cv.ID_LoaiTuVan,
                //              ID_NhanVienPH = nv_ph.ID_NhanVien == null ? Guid.Empty : nv_ph.ID_NhanVien,
                //              ID_NhanVienQuanLy = cv.ID_NhanVienQuanLy == null ? Guid.Empty : cv.ID_NhanVienQuanLy,
                //              ID_NhanVien = cv.ID_NhanVien == null ? Guid.Empty : cv.ID_NhanVien,
                //              LoaiCongViec = loaicv.TenLoaiTuVanLichHen,
                //              Ma_TieuDe = cv.Ma_TieuDe,
                //              TenKhachHang = dt.TenDoiTuong,
                //              MaDoiTuong = dt.MaDoiTuong,
                //              TenLienHe = DMLIENHE.TenLienHe,
                //              TenNhanVienPhuTrach = ns_nv.TenNhanVien,
                //              NgayGio = cv.NgayGio,
                //              NgayGioKetThuc = cv.NgayGioKetThuc,
                //              NhacNho = cv.NhacNho,
                //              NoiDung = cv.NoiDung,
                //              LoaiDoiTuongCV = dt.LoaiDoiTuong,
                //              TrangThai = cv.TrangThai,
                //              TrangThaiKhach = dt.ID_TrangThai,
                //              NguoiTao = cv.NguoiTao,
                //              NgayTao = cv.NgayTao,
                //              GhiChu = cv.GhiChu,
                //              KetQua = cv.KetQua,
                //              MucDoUuTien = cv.MucDoUuTien,
                //              FileDinhKem = cv.FileDinhKem,
                //              NgayHoanThanh = cv.NgayHoanThanh,
                //              SoDienThoai = dt.DienThoai,
                //              NguonKhach = dM_nguon.TenNguonKhach
                //          };

                string lstIDDV = "";
                if (arrIDDonVi.Count() > 0)
                {
                    lstIDDV = string.Join(",", arrIDDonVi);

                }
                else
                {
                    lstIDDV = "%%";
                }

                string lstIDNVPH = "";
                if (arrIDNhanVienPhoiHop.Count() > 0)
                {
                    lstIDNVPH = string.Join(",", arrIDNhanVienPhoiHop);

                }
                else
                {
                    lstIDNVPH = "%%";
                }

                string lstIDLoaiCV = "";
                if (arrLoaiCongViec.Count() > 0)
                {
                    lstIDLoaiCV = string.Join(",", arrLoaiCongViec);

                }
                else
                {
                    lstIDLoaiCV = "%%";
                }

                string lstID_NhanVien = "";
                if (arrIDNhanVien.Count() > 0)
                {
                    lstID_NhanVien = string.Join(",", arrIDNhanVien);

                }
                else
                {
                    lstID_NhanVien = "%%";
                }

                string lstID_NhanVienQL = "";
                if (arrMangIDNhanVien.Count() > 0)
                {
                    lstID_NhanVienQL = string.Join(",", arrMangIDNhanVien);

                }
                else
                {
                    lstID_NhanVienQL = "%%";
                }

                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ListID_DonVi", lstIDDV.Replace("%", "")));
                paramlist.Add(new SqlParameter("ListID_NVPhoiHop", lstIDNVPH.Replace("%", "")));
                paramlist.Add(new SqlParameter("ListID_LoaiCongViec", lstIDLoaiCV.Replace("%", "")));
                paramlist.Add(new SqlParameter("ListID_NhanVien", lstID_NhanVien.Replace("%", "")));
                paramlist.Add(new SqlParameter("ListID_NhanVienQL", lstID_NhanVienQL.Replace("%", "")));
                paramlist.Add(new SqlParameter("DayStart", dayStart));
                paramlist.Add(new SqlParameter("DayEnd", dayEnd));
                var tbl = db.Database.SqlQuery<ChamSocKhachHangDTO>("exec LoadDanhMucCongViec @ListID_DonVi, @ListID_NVPhoiHop, @ListID_LoaiCongViec, @ListID_NhanVien, @ListID_NhanVienQL, @DayStart, @DayEnd", paramlist.ToArray()).AsEnumerable();

                switch (loaiDoiTuong)
                {
                    case 1:
                        tbl = tbl.Where(hd => hd.LoaiDoiTuongCV == 1);
                        break;
                    case 2:
                        tbl = tbl.Where(hd => hd.LoaiDoiTuongCV == 2);
                        break;
                    case 3:
                        break;
                    case 0: // Hoàn thành + Đã hủy
                        break;
                    default: // tam luu
                        break;
                }

                if (TrangThaiCongViec != null && TrangThaiCongViec.Count > 0)
                {
                    tbl = tbl.Where(x => TrangThaiCongViec.Select(p => p.ToString()).Contains(x.TrangThai));
                }

                if (TrangThaiKhach != null && TrangThaiKhach.Count > 0)
                {
                    tbl = tbl.Where(x => TrangThaiKhach.Contains(x.TrangThaiKhach));
                }

                if (MucDoUuTien != 0)
                {
                    tbl = tbl.Where(p => p.MucDoUuTien == MucDoUuTien);
                }

                if (txtSearch != "undefined" && txtSearch != null)
                {
                    tbl = tbl.ToList().Where(p => (p.Ma_TieuDe != null && p.Ma_TieuDe.Contains(txtSearch.Trim())) || (p.LoaiCongViec != null && p.LoaiCongViec.Contains(txtSearch.Trim())) || (p.TenKhachHang != null && p.TenKhachHang.Contains(txtSearch.Trim())) || (p.TenLienHe != null && p.TenLienHe.Contains(txtSearch.Trim())) || (p.TenNhanVienPhuTrach != null && p.TenNhanVienPhuTrach.Contains(txtSearch.Trim())));
                }

                return tbl.ToList();
            }
            else
            {
                return new List<ChamSocKhachHangDTO>();
            }
        }

        public List<ChamSocKhachHangDTO> GetListTuVan(string txtSearch, Guid iddonvi)
        {
            if (db != null)
            {
                var tbl = from cskh in db.ChamSocKhachHang
                          join loaitv in db.DM_LoaiTuVanLichHen on cskh.ID_LoaiTuVan equals loaitv.ID
                          join dt in db.DM_DoiTuong on cskh.ID_KhachHang equals dt.ID
                          join nv in db.NS_NhanVien on cskh.ID_NhanVien equals nv.ID
                          where cskh.ID_DonVi == iddonvi && cskh.PhanLoai == 1
                          orderby cskh.NgayTao descending
                          select new ChamSocKhachHangDTO()
                          {
                              ID = cskh.ID,
                              Ma_TieuDe = cskh.Ma_TieuDe,
                              ID_LoaiTuVan = cskh.ID_LoaiTuVan,
                              ID_NhanVien = cskh.ID_NhanVien,
                              ID_NhanVienQuanLy = cskh.ID_NhanVienQuanLy,
                              TenLoaiTV = loaitv.TenLoaiTuVanLichHen,
                              TenKhachHang = dt.TenDoiTuong,
                              TenNV = nv.TenNhanVien,
                              NgayGio = cskh.NgayGio,
                              NoiDung = cskh.NoiDung,
                              TraLoi = cskh.TraLoi,
                              //ThoiGianHenLaiTuVan = cskh.ThoiGianHenLaiTuVan,
                              TrangThai = cskh.TrangThai
                          };

                var data = tbl.AsEnumerable().Select(hd => new ChamSocKhachHangDTO
                {
                    ID = hd.ID,
                    Ma_TieuDe = hd.Ma_TieuDe,
                    ID_LoaiTuVan = hd.ID_LoaiTuVan,
                    ID_NhanVien = hd.ID_NhanVien,
                    ID_NhanVienQuanLy = hd.ID_NhanVienQuanLy,
                    TenLoaiTV = hd.TenLoaiTV,
                    TenKhachHang = hd.TenKhachHang,
                    TenNV = hd.TenNV,
                    NgayGio = hd.NgayGio,
                    NoiDung = hd.NoiDung,
                    TrangThai = hd.TrangThai,
                    TraLoi = hd.TraLoi,
                    TenLoaiTuVanUnSign = CommonStatic.ConvertToUnSign(hd.TenLoaiTV).ToLower(),
                    TenKhachHangUnSign = CommonStatic.ConvertToUnSign(hd.TenKhachHang).ToLower(),
                    TenNhanVienUnSign = CommonStatic.ConvertToUnSign(hd.TenNV).ToLower(),
                });

                if (txtSearch != "undefined" && txtSearch != null)
                {
                    var Search = CommonStatic.ConvertToUnSign(txtSearch).ToLower();
                    data = data.Where(p => p.Ma_TieuDe.Contains(txtSearch) || p.TenLoaiTV.Contains(txtSearch) || p.TenLoaiTuVanUnSign.Contains(Search) || p.TenKhachHang.Contains(txtSearch) || p.TenKhachHangUnSign.Contains(Search) || p.TenNV.Contains(txtSearch) || p.TenNhanVienUnSign.Contains(Search));
                }
                return data.ToList();
            }
            else
            {
                return new List<ChamSocKhachHangDTO>();
            }
        }

        public List<ChamSocKhachHangDTO> GetListLichHen(string txtSearch, Guid iddonvi)
        {
            if (db != null)
            {
                var tbl = from cskh in db.ChamSocKhachHang
                          join loaitv in db.DM_LoaiTuVanLichHen on cskh.ID_LoaiTuVan equals loaitv.ID
                          join dt in db.DM_DoiTuong on cskh.ID_KhachHang equals dt.ID
                          join nv in db.NS_NhanVien on cskh.ID_NhanVien equals nv.ID
                          where cskh.ID_DonVi == iddonvi && cskh.PhanLoai == 3
                          orderby cskh.NgayTao descending
                          select new ChamSocKhachHangDTO()
                          {
                              ID = cskh.ID,
                              Ma_TieuDe = cskh.Ma_TieuDe,
                              ID_LoaiTuVan = cskh.ID_LoaiTuVan,
                              ID_NhanVien = cskh.ID_NhanVien,
                              ID_NhanVienQuanLy = cskh.ID_NhanVienQuanLy,
                              TenLoaiTV = loaitv.TenLoaiTuVanLichHen,
                              TenKhachHang = dt.TenDoiTuong,
                              SoDienThoai = dt.DienThoai,
                              TenNV = nv.TenNhanVien,
                              NgayGio = cskh.NgayGio,
                              NgayGioKetThuc = cskh.NgayGioKetThuc,
                              NoiDung = cskh.NoiDung,
                              TrangThai = cskh.TrangThai,
                              NhacNho = cskh.NhacNho
                          };

                var data = tbl.AsEnumerable().Select(hd => new ChamSocKhachHangDTO
                {
                    ID = hd.ID,
                    Ma_TieuDe = hd.Ma_TieuDe,
                    MaTieuDeUnsign = CommonStatic.ConvertToUnSign(hd.Ma_TieuDe).ToLower(),
                    ID_LoaiTuVan = hd.ID_LoaiTuVan,
                    ID_NhanVien = hd.ID_NhanVien,
                    ID_NhanVienQuanLy = hd.ID_NhanVienQuanLy,
                    TenLoaiTV = hd.TenLoaiTV,
                    SoDienThoai = hd.SoDienThoai,
                    TenKhachHang = hd.TenKhachHang,
                    TenNV = hd.TenNV,
                    NgayGio = hd.NgayGio,
                    NgayGioKetThuc = hd.NgayGioKetThuc,
                    NoiDung = hd.NoiDung,
                    TrangThai = hd.TrangThai,
                    NhacNho = hd.NhacNho,
                    TenLoaiTuVanUnSign = CommonStatic.ConvertToUnSign(hd.TenLoaiTV).ToLower(),
                    TenKhachHangUnSign = CommonStatic.ConvertToUnSign(hd.TenKhachHang).ToLower(),
                    TenNhanVienUnSign = CommonStatic.ConvertToUnSign(hd.TenNV).ToLower(),
                });

                if (txtSearch != "undefined" && txtSearch != null)
                {
                    var Search = CommonStatic.ConvertToUnSign(txtSearch).ToLower();
                    data = data.Where(p => p.MaTieuDeUnsign.Contains(Search) || p.TenLoaiTV.Contains(txtSearch) || p.TenLoaiTuVanUnSign.Contains(Search) || p.TenKhachHang.Contains(txtSearch) || p.TenKhachHangUnSign.Contains(Search) || p.TenNV.Contains(txtSearch) || p.TenNhanVienUnSign.Contains(Search));
                }
                return data.ToList();
            }
            else
            {
                return new List<ChamSocKhachHangDTO>();
            }
        }

        public List<ChamSocKhachHangDTO> GetListPhanHoi(string txtSearch, Guid iddonvi)
        {
            if (db != null)
            {
                var tbl = from cskh in db.ChamSocKhachHang
                          join loaitv in db.DM_LoaiTuVanLichHen on cskh.ID_LoaiTuVan equals loaitv.ID
                          join dt in db.DM_DoiTuong on cskh.ID_KhachHang equals dt.ID
                          join nv in db.NS_NhanVien on cskh.ID_NhanVien equals nv.ID
                          where cskh.ID_DonVi == iddonvi && cskh.PhanLoai == 2
                          orderby cskh.NgayTao descending
                          select new ChamSocKhachHangDTO()
                          {
                              ID = cskh.ID,
                              Ma_TieuDe = cskh.Ma_TieuDe,
                              ID_LoaiTuVan = cskh.ID_LoaiTuVan,
                              ID_NhanVien = cskh.ID_NhanVien,
                              ID_NhanVienQuanLy = cskh.ID_NhanVienQuanLy,
                              TenLoaiTV = loaitv.TenLoaiTuVanLichHen,
                              TenKhachHang = dt.TenDoiTuong,
                              TenNV = nv.TenNhanVien,
                              NgayGio = cskh.NgayGio,
                              ThoiGianHenLai = cskh.ThoiGianHenLai,
                              MucDoPhanHoi = cskh.MucDoPhanHoi,
                              NoiDung = cskh.NoiDung,
                              TrangThai = cskh.TrangThai,
                              TraLoi = cskh.TraLoi
                          };

                var data = tbl.AsEnumerable().Select(hd => new ChamSocKhachHangDTO
                {
                    ID = hd.ID,
                    Ma_TieuDe = hd.Ma_TieuDe,
                    MaTieuDeUnsign = CommonStatic.ConvertToUnSign(hd.Ma_TieuDe).ToLower(),
                    ID_LoaiTuVan = hd.ID_LoaiTuVan,
                    ID_NhanVien = hd.ID_NhanVien,
                    ID_NhanVienQuanLy = hd.ID_NhanVienQuanLy,
                    TenLoaiTV = hd.TenLoaiTV,
                    TenKhachHang = hd.TenKhachHang,
                    TenNV = hd.TenNV,
                    NgayGio = hd.NgayGio,
                    ThoiGianHenLai = hd.ThoiGianHenLai,
                    NoiDung = hd.NoiDung,
                    MucDoPhanHoi = hd.MucDoPhanHoi,
                    TrangThai = hd.TrangThai,
                    TraLoi = hd.TraLoi,
                    TenLoaiTuVanUnSign = CommonStatic.ConvertToUnSign(hd.TenLoaiTV).ToLower(),
                    TenKhachHangUnSign = CommonStatic.ConvertToUnSign(hd.TenKhachHang).ToLower(),
                    TenNhanVienUnSign = CommonStatic.ConvertToUnSign(hd.TenNV).ToLower(),
                });

                if (txtSearch != "undefined" && txtSearch != null)
                {
                    var Search = CommonStatic.ConvertToUnSign(txtSearch).ToLower();
                    data = data.Where(p => p.MaTieuDeUnsign.Contains(Search) || p.TenLoaiTV.Contains(txtSearch) || p.TenLoaiTuVanUnSign.Contains(Search) || p.TenKhachHang.Contains(txtSearch) || p.TenKhachHangUnSign.Contains(Search) || p.TenNV.Contains(txtSearch) || p.TenNhanVienUnSign.Contains(Search));
                }
                return data.ToList();
            }
            else
            {
                return new List<ChamSocKhachHangDTO>();
            }
        }
        #endregion


        #region update
        public string Update_PhieuTuVan(ChamSocKhachHang obj)
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
                    #region ChamSocKhachHang
                    ChamSocKhachHang objUpd = db.ChamSocKhachHang.Find(obj.ID);
                    string sMa_TieuDe = string.Empty;
                    if (obj.Ma_TieuDe == null)
                    {
                        sMa_TieuDe = GetMaPhieuChamSoc();
                    }
                    else
                    {
                        sMa_TieuDe = obj.Ma_TieuDe;
                    }
                    objUpd.ID = obj.ID;
                    objUpd.Ma_TieuDe = sMa_TieuDe;
                    objUpd.ID_KhachHang = obj.ID_KhachHang;
                    objUpd.ID_LoaiTuVan = obj.ID_LoaiTuVan;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.PhanLoai = 1;
                    objUpd.NhacNho = 0;
                    objUpd.MucDoPhanHoi = 1;
                    objUpd.NoiDung = obj.NoiDung;
                    objUpd.NgayGio = obj.NgayGio;
                    objUpd.ThoiGianHenLai = obj.ThoiGianHenLai;
                    objUpd.TraLoi = obj.TraLoi;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.TrangThai = obj.TrangThai;
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

        public string Update_LoaiCongViec(NS_CongViec_PhanLoai obj)
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
                    NS_CongViec_PhanLoai objUpd = db.NS_CongViec_PhanLoai.Find(obj.ID);
                    objUpd.LoaiCongViec = obj.LoaiCongViec;
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

        public string UpdateCongViec(ChamSocKhachHang obj)
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
                    ChamSocKhachHang objUpd = db.ChamSocKhachHang.Find(obj.ID);
                    objUpd.ID_KhachHang = obj.ID_KhachHang;
                    objUpd.ID_LoaiTuVan = obj.ID_LoaiTuVan;
                    objUpd.PhanLoai = obj.PhanLoai;
                    objUpd.Ma_TieuDe = obj.Ma_TieuDe;
                    objUpd.ID_LienHe = obj.ID_LienHe;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.NgayGio = obj.NgayGio;
                    objUpd.MucDoUuTien = obj.MucDoUuTien;
                    objUpd.NgayGioKetThuc = obj.NgayGioKetThuc;
                    objUpd.NoiDung = obj.NoiDung;
                    objUpd.TrangThai = obj.TrangThai;
                    objUpd.KetQua = obj.KetQua;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.NgayHoanThanh = obj.NgayHoanThanh;
                    objUpd.CaNgay = obj.CaNgay;
                    objUpd.NhacNho = obj.NhacNho;
                    objUpd.KieuNhacNho = obj.KieuNhacNho;

                    objUpd.ID_Parent = obj.ID_Parent;
                    objUpd.KieuLap = obj.KieuLap;
                    objUpd.SoLanLap = obj.SoLanLap;
                    objUpd.GiaTriLap = obj.GiaTriLap;
                    objUpd.TuanLap = obj.TuanLap;
                    objUpd.TrangThaiKetThuc = obj.TrangThaiKetThuc;
                    objUpd.GiaTriKetThuc = obj.GiaTriKetThuc;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("UpdateCongViec: " + ex.InnerException + ex.Message);
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion

        public List<ChamSocKhachHangDTO> GetTuVanByLoaiTuVan()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from ctCK in db.ChamSocKhachHang
                          select new ChamSocKhachHangDTO
                          {
                              ID = ctCK.ID,
                              Ma_TieuDe = ctCK.Ma_TieuDe,
                              NgayGio = ctCK.NgayGio,
                              ID_KhachHang = ctCK.ID_KhachHang,
                              ID_LoaiTuVan = ctCK.ID_LoaiTuVan,
                              ID_NhanVien = ctCK.ID_NhanVien,
                              TrangThai = ctCK.TrangThai,
                              NoiDung = ctCK.NoiDung,
                              TraLoi = ctCK.TraLoi,
                          };
                if (tbl == null)
                {
                    return null;
                }
                else
                {
                    return tbl.ToList();
                }
            }
        }

        public string GetMaPhieuChamSoc()
        {
            string format = "{0:0000}";

            string machamsoc = "CS0";
            string madv = db.ChamSocKhachHang.Where(p => p.Ma_TieuDe.Contains(machamsoc)).Where(p => p.Ma_TieuDe.Length == 7).OrderByDescending(p => p.Ma_TieuDe).Select(p => p.Ma_TieuDe).FirstOrDefault();
            if (madv == null)
            {
                machamsoc = machamsoc + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(machamsoc.Length, 4)) + 1;
                machamsoc = machamsoc + string.Format(format, tempstt);
            }
            return machamsoc;
        }


        #region delete
        string CheckDelete_PhieuTV(SsoftvnContext db, ChamSocKhachHang obj)
        {
            string strCheck = string.Empty;

            return strCheck;
        }

        bool Check_LoaiCongViec_IsUsed(Guid idLoaiCongViec)
        {
            try
            {
                SqlParameter parma = new SqlParameter("ID_LoaiCongViec", idLoaiCongViec);
                var objReturn = db.Database.SqlQuery<Model.SP_ReturnBool>("EXEC SP_Check_LoaiCongViec_IsUsed @ID_LoaiCongViec", parma).ToList();
                if (objReturn != null)
                {
                    return objReturn.FirstOrDefault().Exist;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Check_LoaiCongViec_IsUsed: " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }

        public string Delete_LoaiCongViec(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                NS_CongViec_PhanLoai objDel = db.NS_CongViec_PhanLoai.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        if (Check_LoaiCongViec_IsUsed(id))
                        {
                            return "Loại công việc đang được sử dụng trong hệ thống";
                        }
                        else
                        {
                            objDel.TrangThai = 0; // 0: xoa, 1: chua xoa
                            db.Entry(objDel).State = EntityState.Modified;
                            db.SaveChanges();
                            return "";
                        }
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("Delete_LoaiCongViec: " + ex.InnerException + ex.Message);
                        return ex.Message;
                    }
                }
                else
                {
                    return "Không tìm thấy dữ liệu cần xử lý trên hệ thống";
                }
            }
        }
        #endregion

        public string Delete_PhieuTuVan(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                ChamSocKhachHang objDel = db.ChamSocKhachHang.Find(id);
                if (objDel != null)
                {
                    string strCheck = CheckDelete_PhieuTV(db, objDel);
                    if (strCheck == string.Empty)
                    {
                        try
                        {
                            //List<BH_HoaDon> lstHoaDon = db.BH_HoaDon.Where(p => p.ID_NhanVien == id).ToList();
                            //if (lstHoaDon != null && lstHoaDon.Count > 0)
                            //{
                            //    db.BH_HoaDon.RemoveRange(lstHoaDon);
                            //}
                            db.ChamSocKhachHang.Remove(objDel);
                            //
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
                        strErr = strCheck;
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
    }
    public class ChamSocKhachHangDTO : ChamSocKhachHang
    {
        public Guid? ID_NhanVienPH { get; set; }
        public string LoaiCongViec { get; set; }
        public string TenNhanVienPhoiHop { get; set; }
        public string TenLienHe { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenNhanVienPhuTrach { get; set; }
        public string ChuoiNhanVienPhoiHop { get; set; }
        public int LoaiDoiTuongCV { get; set; }
        public Guid? TrangThaiKhach { get; set; }
        public string TenCongViecUnSign { get; set; }
        public string TenCongViecStartChar { get; set; }
        public string TenKhachHangUnsign { get; set; }
        public string TenKhachHangStartChar { get; set; }
        public string TenLienHeUnSign { get; set; }
        public string TenLienHeStartChar { get; set; }
        public string TenNhanVienPhuTrachUnSign { get; set; }
        public string TenNhanVienPhuTrachStartChar { get; set; }
        public string FileHienThi { get; set; }
        public string NguonKhach { get; set; }
    }

    public class ChamSocKhachHangXuatFileDTO
    {
        public string PhanLoai { get; set; }
        public string LoaiCongViec { get; set; }
        public string Ma_TieuDe { get; set; }
        public string MucDoUuTien { get; set; }
        public DateTime NgayGio { get; set; }
        public DateTime? NgayGioKetThuc { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string NguonKhach { get; set; }
        public string TenNhanVienPhuTrach { get; set; }
        public int NhacNho { get; set; }
        public string TrangThai { get; set; }
        public string KetQua { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string GhiChu { get; set; }
    }

    public class SP_NSCongViec
    {

        public Guid ID { get; set; }

        public Guid? ID_KhachHang { get; set; }

        public Guid? ID_LienHe { get; set; }

        public Guid ID_LoaiCongViec { get; set; }

        public string LoaiCongViec { get; set; }

        public string TenDoiTuong { get; set; }

        public string TenLienHe { get; set; }

        public DateTime? ThoiGianTu { get; set; }

        public DateTime? ThoiGianDen { get; set; } // anh, chi, ong, ba

        public Guid? ID_NhanVienQuanLy { get; set; }

        public Guid? ID_NhanVienChiaSe { get; set; }

        public string TenNVThucHien { get; set; }

        public string TenNVChiaSe { get; set; }

        public string NoiDung { get; set; }

        public DateTime? ThoiGianLienHeLai { get; set; }

        public int NhacTruoc { get; set; }

        public int NhacTruocLienHeLai { get; set; }

        public int? TrangThai { get; set; }

        public string LyDoHenLai { get; set; }

        public string KetQuaCongViec { get; set; }

        public string TrangThaiStr { get; set; }

        public string NhacTruocStr { get; set; }

        public string NhacTruocLienHeLaiStr { get; set; }

        public Guid? ID_CustomerType { get; set; } // tiềm năng, không tiềm năng
    }

    public class SP_ChamSocKhachHang
    {

        public Guid ID { get; set; }

        public Guid? ID_KhachHang { get; set; }

        public Guid? ID_LienHe { get; set; }

        public Guid ID_LoaiTuVan { get; set; }

        public string TenDoiTuong { get; set; }

        public string TenLienHe { get; set; }

        public string Ma_TieuDe { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? ThoiGianTu { get; set; }

        public DateTime? ThoiGianDen { get; set; }

        public DateTime? NgayHoanThanh { get; set; }

        public Guid? ID_NhanVienQuanLy { get; set; }

        public Guid? ID_NhanVien { get; set; } // NV Phu Trach

        public string StaffIDs { get; set; } // IDs NV PhoiHop

        public string StaffNames { get; set; } // Tens NV PhoiHop

        public string NoiDung { get; set; }

        public string KetQua { get; set; }

        public string GhiChu { get; set; }

        public string TrangThai { get; set; }

        public string NguoiTao { get; set; }

        public int? MucDoUuTien { get; set; }

        public string FileDinhKem { get; set; }

        public string TenNhanVien { get; set; }

        public string TenLoaiTuVanLichHen { get; set; }

        public Guid? ID_TrangThai { get; set; } // tiềm năng, không tiềm năng

        public bool? CaNgay { get; set; } // tiềm năng, không tiềm năng

    }

}