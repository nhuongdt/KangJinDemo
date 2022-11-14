using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;

namespace libDM_DoiTuong
{
    public class classDM_DoiTuong
    {
        private SsoftvnContext db;
        public classDM_DoiTuong(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DM_DoiTuong Select_DoiTuong(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_DoiTuong.Find(id);
            }
        }

        public List<JqAuto_DMDoiTuong> JqAuto_SearchDoiTuong(int loaiDoiTuong, string txtSearch, Guid? idChiNhanh = null)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            paramlist.Add(new SqlParameter("LoaiDoiTuong", loaiDoiTuong));
            paramlist.Add(new SqlParameter("Search", txtSearch));
            paramlist.Add(new SqlParameter("ID_DonVi", idChiNhanh ?? (object)DBNull.Value));
            return db.Database.SqlQuery<JqAuto_DMDoiTuong>("exec GetListDoiTuongByLoai @LoaiDoiTuong, @Search, @ID_DonVi", paramlist.ToArray()).ToList();
        }
        public List<DM_VungMienDTO> getList_VungMien()
        {
            try
            {
                var tb = (from tt in db.DM_VungMien
                          select new DM_VungMienDTO
                          {
                              ID = tt.ID,
                              TenVung = tt.TenVung
                          }).ToList();
                return tb;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog(string.Concat("getList_VungMien ", e.InnerException, e.Message));
                return null;
            }
        }

        public List<DM_VungMienDTO> getList_VungMienbyID(Guid ID)
        {
            List<DM_VungMienDTO> lst = new List<DM_VungMienDTO>();
            var tb = from tt in db.DM_VungMien
                     where tt.ID == ID
                     select new DM_VungMienDTO
                     {
                         ID = tt.ID,
                         TenVung = tt.TenVung
                     };
            try
            {
                lst = tb.ToList();
            }
            catch
            {

            }
            return lst;
        }
        public List<DM_VungMienDTO> getList_TinhThanhbyID(Guid ID)
        {
            List<DM_VungMienDTO> lst = new List<DM_VungMienDTO>();
            var tb = (from tt in db.DM_TinhThanh
                      where tt.ID == ID
                      select new DM_VungMienDTO
                      {
                          ID = tt.ID,
                          TenVung = tt.TenTinhThanh
                      }).ToList();
            return lst;
        }
        public List<DM_DoiTuong> Gets(Expression<Func<DM_DoiTuong, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_DoiTuong.ToList();
                else
                    return db.DM_DoiTuong.Where(query).ToList();
            }
        }
        public List<DM_NhomDoiTuongDTO> getList_NhomDoiTuongbyID(Guid ID)
        {
            List<DM_NhomDoiTuongDTO> lst = new List<DM_NhomDoiTuongDTO>();
            var tb = (from tt in db.DM_NhomDoiTuong
                      where tt.ID == ID
                      select new DM_NhomDoiTuongDTO
                      {
                          ID = tt.ID,
                          TenNhomDoiTuong = tt.TenNhomDoiTuong,
                          GiamGia = tt.GiamGia,
                          GiamGiaTheoPhanTram = tt.GiamGiaTheoPhanTram,
                          GhiChu = tt.GhiChu,
                          TuDongCapNhat = tt.TuDongCapNhat == true ? tt.TuDongCapNhat : false
                      }).ToList();
            return lst;
        }
        public List<DM_NhomDoiTuongChiTietDTO> getList_NhomDoiTuongChiTietbyID(Guid ID_NhomDoiTuong)
        {
            List<DM_NhomDoiTuongChiTietDTO> lst = new List<DM_NhomDoiTuongChiTietDTO>();
            var tb = (from tt in db.DM_NhomDoiTuong_ChiTiet
                      where tt.ID_NhomDoiTuong == ID_NhomDoiTuong
                      select new DM_NhomDoiTuongChiTietDTO
                      {
                          IDHT = tt.LoaiDieuKien,
                          IDDK = tt.STT,
                          LoaiHinhThuc = tt.LoaiDieuKien,
                          SoSanh = tt.LoaiSoSanh,
                          GiaTri = (double?)tt.GiaTriSo ?? 0,
                      }).ToList();
            return lst;
        }
        public DM_DoiTuong Get(Expression<Func<DM_DoiTuong, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                DM_DoiTuong dataReturn = new DM_DoiTuong();
                if (query == null)
                {
                    dataReturn = db.DM_DoiTuong.FirstOrDefault();
                }
                else
                {
                    var data = db.DM_DoiTuong.Where(query);
                    if (data != null)
                    {
                        dataReturn = data.FirstOrDefault();
                    }
                    else
                    {
                        dataReturn = null;
                    }
                }
                return dataReturn;
            }
        }

        public bool DM_DoiTuongExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {

                return db.DM_DoiTuong.Count(e => e.ID == id) > 0;
            }
        }

        public bool Check_ExistCode(string code, Guid? id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                if (id != null && id.ToString() != "null" && id != Guid.Empty)
                {
                    // check ma khi update: Neu trung ma va khac ID --> return true
                    return db.DM_DoiTuong.Count(e => e.MaDoiTuong.Trim() == code.Trim() && e.ID != id) > 0;
                }
                else
                {
                    return db.DM_DoiTuong.Count(e => e.MaDoiTuong.Trim() == code.Trim()) > 0;
                }
            }
        }

        /// <summary>
        /// get infor basic of doituong by ID: used get after add|update DoiTuong
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <returns></returns>

        public IEnumerable<SP_DM_DoiTuong> SP_GetInforBasic_DoiTuongByID(Guid idDoiTuong)
        {
            List<SP_DM_DoiTuong> lst = null;
            try
            {
                SqlParameter param = new SqlParameter("ID_DoiTuong", idDoiTuong);
                return db.Database.SqlQuery<SP_DM_DoiTuong>("exec GetInforBasic_DoiTuongByID @ID_DoiTuong", param).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetInforBasic_DoiTuongByID: " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        /// <summary>
        /// get infor NoKH, TongMua, TongTra of KH to date search
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<SP_DM_DoiTuong> GetNoKhachHang_byDate(CommonParamSearch param)
        {
            var idChiNhanhs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count() > 0)
            {
                idChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_DoiTuong", param.TextSearch));//muon tamtruong
            lstParam.Add(new SqlParameter("ToDate", param.DateTo));
            lstParam.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            return db.Database.SqlQuery<SP_DM_DoiTuong>("exec GetNoKhachHang_byDate @ID_DoiTuong, @ToDate, @IDChiNhanhs", lstParam.ToArray()).ToList();
        }

        /// <summary>
        /// get infor of KhachHang --> used to check NangNhom and KhuyenMai
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="idChiNhanh"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="wasChotSo"></param>
        /// <returns></returns>
        public List<SP_DM_DoiTuong> SP_GetInforKhachHang_ByID(Guid idDoiTuong, Guid idChiNhanh, DateTime timeStart, DateTime timeEnd, bool wasChotSo)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("timeStart", timeStart));
            lstParam.Add(new SqlParameter("timeEnd", timeEnd));
            List<SP_DM_DoiTuong> lst = null;
            try
            {
                if (wasChotSo)
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec SP_GetInforKhachHang_ByID @ID_DoiTuong, @ID_ChiNhanh, @timeStart, @timeEnd", lstParam.ToArray()).ToList();
                }
                else
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec SP_GetInforKhachHang_ByID @ID_DoiTuong,  @ID_ChiNhanh, @timeStart, @timeEnd", lstParam.ToArray()).ToList();
                }
                return lst;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("classDM_DoiTuong.SP_GetInforKhachHang_ByID: " + e.InnerException + e.Message);
                return null;
            }
        }

        public List<JqAuto_DMDoiTuong> GetCustomer_haveBirthday(CommonParamSearch param)
        {
            var idChiNhanhs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
            return db.Database.SqlQuery<JqAuto_DMDoiTuong>("exec GetCustomer_haveBirthday @IDChiNhanhs," +
                "@DateFrom, @DateTo", sql.ToArray()).ToList();
        }
        public List<JqAuto_DMDoiTuong> GetCustomer_haveTransaction(CommonParamSearch param)
        {
            var idChiNhanhs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
            return db.Database.SqlQuery<JqAuto_DMDoiTuong>("exec GetCustomer_haveTransaction @IDChiNhanhs," +
                "@DateFrom, @DateTo", sql.ToArray()).ToList();
        }

        public List<SP_DM_DoiTuong> SP_GetListKhachHang_Where(Guid idChiNhanh, int loaiDoiTuong, string maDoiTuong, string idNhomDT, DateTime ngayTaoStart, DateTime ngayTaoEnd,
        DateTime tongBanStart, DateTime tongBanEnd, double tongBanFrom, double tongBanTo, string noHienTaiFrom, string noHienTaiTo, string columsort, string sort,
        int loaiKhach = 0, int gioiTinh = 0, string idNguonKhach = null, string idTinhThanhs = null, DateTime? ngaySinhFrom = null,
        DateTime? ngaySinhTo = null, int typeNgaySinh = 0, int customerDebit = 0, string idManagers = null)
        {
            var tbl_timeCSt = from cs in db.ChotSo
                              where cs.ID_DonVi == idChiNhanh
                              select cs;
            string maKH_Search = string.Empty;

            if (maDoiTuong != null & maDoiTuong != "" & maDoiTuong != "null")
            {
                maKH_Search = "%" + maDoiTuong + "%";
            }
            else
            {
                maKH_Search = "%%";
            }

            var idNhomDTSearch = string.Empty;
            if (idNhomDT == "null")
            {
                idNhomDTSearch = "%%";
            }
            else
            {
                idNhomDTSearch = "%" + idNhomDT + "%";
            }

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("timeStart", tongBanStart));
            lstParam.Add(new SqlParameter("timeEnd", tongBanEnd));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("MaKH", "%%"));
            lstParam.Add(new SqlParameter("LoaiKH", loaiDoiTuong));
            lstParam.Add(new SqlParameter("ID_NhomKhachHang", idNhomDTSearch));
            lstParam.Add(new SqlParameter("timeStartKH", ngayTaoStart));
            lstParam.Add(new SqlParameter("timeEndKH", ngayTaoEnd));

            List<SP_DM_DoiTuong> lst = null;
            try
            {
                if (tbl_timeCSt.Count() > 0)
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo_ChotSo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH", lstParam.ToArray()).ToList();
                }
                else
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH", lstParam.ToArray()).ToList();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetListKhachHang_Where: " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        /// <summary>
        /// get list KhachHang: pass param is object {Params_GetListKhachHang}
        /// </summary>
        /// <param name="listParams"></param>
        /// <returns></returns>

        public List<SP_DM_DoiTuong> SP_GetListKhachHang_Where_PassObject(Params_GetListKhachHang listParams)
        {
            var idChiNhanh = listParams.ID_DonVis.FirstOrDefault();
            var tbl_timeCSt = from cs in db.ChotSo
                              where cs.ID_DonVi == idChiNhanh
                              select cs;

            var idNhomDTSearch = string.Empty;
            if (listParams.ID_NhomDoiTuong == null)
            {
                idNhomDTSearch = "%%";
            }
            else
            {
                idNhomDTSearch = "%" + listParams.ID_NhomDoiTuong + "%";
            }

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("timeStart", listParams.TongBan_TuNgay));
            lstParam.Add(new SqlParameter("timeEnd", listParams.TongBan_DenNgay));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("MaKH", "%%"));
            lstParam.Add(new SqlParameter("LoaiKH", listParams.LoaiDoiTuong));
            lstParam.Add(new SqlParameter("ID_NhomKhachHang", idNhomDTSearch));
            lstParam.Add(new SqlParameter("timeStartKH", listParams.NgayTao_TuNgay));
            lstParam.Add(new SqlParameter("timeEndKH", listParams.NgayTao_DenNgay));

            List<SP_DM_DoiTuong> lst = new List<SP_DM_DoiTuong>();
            try
            {
                if (tbl_timeCSt.Count() > 0)
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo_ChotSo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH", lstParam.ToArray()).ToList();
                }
                else
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH", lstParam.ToArray()).ToList();
                }

                if (listParams.LoaiDoiTuong == 1)
                {
                    if (listParams.TongBan_Tu > 0)
                    {
                        lst = lst.Where(x => x.TongBan >= listParams.TongBan_Tu).ToList();
                    }

                    if (listParams.TongBan_Den > 0)
                    {
                        lst = lst.Where(x => x.TongBan <= listParams.TongBan_Den).ToList();
                    }
                }
                else
                {
                    if (listParams.TongBan_Tu > 0)
                    {
                        lst = lst.Where(x => x.TongMua >= listParams.TongBan_Tu).ToList();
                    }

                    if (listParams.TongBan_Den > 0)
                    {
                        lst = lst.Where(x => x.TongMua <= listParams.TongBan_Den).ToList();
                    }
                }

                switch (listParams.LoaiKhach)
                {
                    case 1:
                        lst = lst.Where(x => x.LaCaNhan).ToList();
                        break;
                    case 2:
                        lst = lst.Where(x => x.LaCaNhan == false).ToList();
                        break;
                }

                switch (listParams.GioiTinh)
                {
                    case 1:
                        lst = lst.Where(x => x.GioiTinhNam == true).ToList();
                        break;
                    case 2:
                        lst = lst.Where(x => x.GioiTinhNam == false).ToList();
                        break;
                }

                if (listParams.ID_NguonKhach != null)
                {
                    lst = lst.Where(x => x.ID_NguonKhach == new Guid(listParams.ID_NguonKhach)).ToList();
                }

                var lstIDTinhThanh = listParams.ID_TinhThanhs;
                if (listParams.ID_TinhThanhs != null)
                {
                    if (lstIDTinhThanh.Count > 0)
                    {
                        lst = lst.Where(hd => lstIDTinhThanh.Contains(hd.ID_TinhThanh.ToString())).ToList();
                    }
                }

                var ngaySinhFrom = listParams.NgaySinh_TuNgay;
                var ngaySinhTo = listParams.NgaySinh_DenNgay;

                if (listParams.NgaySinh_TuNgay != null && listParams.NgaySinh_DenNgay != null)
                {
                    var monthFrom = ngaySinhFrom.Value.Month;
                    var monthTo = ngaySinhTo.Value.Month;
                    var dateFrom = ngaySinhFrom.Value.Day;
                    var dateTo = ngaySinhTo.Value.Day;

                    if (listParams.NgaySinh_TuNgay == listParams.NgaySinh_DenNgay)
                    {
                        lst = lst.Where(dt => dt.NgaySinh_NgayTLap != null
                        && dt.NgaySinh_NgayTLap.Value.Month == monthFrom && dt.NgaySinh_NgayTLap.Value.Day == dateFrom).ToList();
                    }
                    else
                    {
                        if (ngaySinhFrom != new DateTime(1918, 1, 1))
                        {
                            // get KH with NgaySinh != null
                            lst = lst.Where(dt => dt.NgaySinh_NgayTLap != null).ToList();

                            // compare nam
                            if (listParams.LoaiNgaySinh == 1)
                            {
                                lst = lst.Where(dt => dt.NgaySinh_NgayTLap.Value.Year == ngaySinhFrom.Value.Year).ToList();
                            }
                            else
                            {
                                // compare month/day
                                if (monthFrom == monthTo)
                                {
                                    // compare date (dateFrom <= ngaysinh <= dateTo)
                                    lst = lst.Where(dt => dt.NgaySinh_NgayTLap.Value.Month == monthFrom
                                 && dt.NgaySinh_NgayTLap.Value.Day >= dateFrom && dt.NgaySinh_NgayTLap.Value.Day <= dateTo).ToList();
                                }
                                else
                                {
                                    // (monthFrom < thangsinh < monthTo) OR (same month and ngaysinh >= dateFrom) OR ( ngaysinh <= dateTo)
                                    lst = lst.Where(dt => (dt.NgaySinh_NgayTLap.Value.Month > monthFrom && dt.NgaySinh_NgayTLap.Value.Month < monthTo)
                                || ((dt.NgaySinh_NgayTLap.Value.Month == monthFrom && dt.NgaySinh_NgayTLap.Value.Day >= dateFrom)
                                || (dt.NgaySinh_NgayTLap.Value.Month == monthTo && dt.NgaySinh_NgayTLap.Value.Day <= dateFrom))).ToList();
                                }
                            }
                        }
                    }
                }

                // còn nợ
                var customerDebit = listParams.No_TrangThai;
                var loaiDoiTuong = listParams.LoaiDoiTuong;
                if (customerDebit == 1)
                {
                    if (loaiDoiTuong == 2)
                    {
                        lst = lst.Where(x => x.NoHienTai * (-1) != 0).ToList();
                    }
                    else
                    {
                        lst = lst.Where(x => x.NoHienTai != 0).ToList();
                    }
                }

                // hết nợ
                if (customerDebit == 2)
                {
                    if (loaiDoiTuong == 2)
                    {
                        lst = lst.Where(x => x.NoHienTai * (-1) <= 0).ToList();
                    }
                    else
                    {
                        lst = lst.Where(x => x.NoHienTai <= 0).ToList();
                    }
                }

                //double noHienTaiFromSearch = 0;
                var noHienTaiFrom = listParams.NoHienTai_Tu;
                var noHienTaiTo = listParams.NoHienTai_Den;

                if (noHienTaiFrom != null)
                {
                    if (loaiDoiTuong == 2)
                    {
                        lst = lst.Where(x => x.NoHienTai * (-1) >= noHienTaiFrom).ToList();
                    }
                    else
                    {
                        lst = lst.Where(x => x.NoHienTai >= noHienTaiFrom).ToList();
                    }
                }

                if (noHienTaiTo != null)
                {
                    if (loaiDoiTuong == 2)
                    {
                        lst = lst.Where(x => x.NoHienTai * (-1) <= noHienTaiTo).ToList();
                    }
                    else
                    {
                        lst = lst.Where(x => x.NoHienTai <= noHienTaiTo).ToList();
                    }
                }

                var lstIDManager = listParams.ID_NhanVienQuanLys;
                if (lstIDManager != null)
                {
                    if (lstIDManager.Count > 0)
                    {
                        var nguoiTao = listParams.NguoiTao;
                        lst = lst.Where(x => lstIDManager.Contains(x.ID_NhanVienPhuTrach.ToString())
                        || x.ID_NhanVienPhuTrach == null || x.NguoiTao.Contains(nguoiTao)).ToList();
                    }
                }

                var maDoiTuong = listParams.MaDoiTuong;
                if (!string.IsNullOrWhiteSpace(maDoiTuong))
                {
                    char[] whitespace = new char[] { ' ', '\t' };
                    string[] textFilter = maDoiTuong.ToLower().Split(whitespace);
                    string[] utf8 = textFilter.Where(o => o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    string[] utf = textFilter.Where(o => !o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    var maDoiTuongUng = CommonStatic.ConvertToUnSign(maDoiTuong).ToLower();
                    lst = lst.Where(o =>
                    o.MaDoiTuong.ToLower().Contains(@maDoiTuongUng)
                    || o.TenDoiTuong.ToLower().Contains(maDoiTuongUng)
                    // find TenDoiTuong like maDoiTuong (chua unsign)
                    || o.TenDoiTuong.ToLower().Contains(maDoiTuong.ToLower())
                    || (o.DienThoai != null && o.DienThoai.Contains(@maDoiTuongUng))
                    || o.Email.ToLower().Contains(maDoiTuong.ToLower())
                    || o.NguoiTao.ToLower().Contains(maDoiTuong.ToLower())

                    // nguoi gioi thieu
                    || (utf8.All(c => o.NguoiGioiThieu.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.NguoiGioiThieu).ToLower().Contains(d)))

                    // tinh thanh
                    || (utf8.All(c => o.KhuVuc.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.KhuVuc).ToLower().Contains(d)))
                    // quan huyen
                    || (utf8.All(c => o.PhuongXa.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.PhuongXa).ToLower().Contains(d)))

                    // quan huyen
                    || (utf8.All(c => o.DiaChi.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.DiaChi).ToLower().Contains(d)))

                    // ghi chu
                    || (utf8.All(c => o.GhiChu.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.GhiChu).ToLower().Contains(d)))

                    || (utf8.All(c => o.TenDoiTuong.ToLower().IndexOf(c) >= 0)
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.TenDoiTuong).ToLower().Contains(d)
                    || o.MaDoiTuong.ToLower().Contains(d)))).ToList();
                }

                var idTrangThai = listParams.ID_TrangThai;
                if (idTrangThai != null)
                {
                    lst = lst.Where(x => x.ID_TrangThai == idTrangThai).ToList();
                }

                var sort = listParams.TrangThai_SapXep;
                var columsort = listParams.Cot_SapXep;
                if (sort != 0)
                {
                    if (sort == 1)
                    {
                        switch (columsort)
                        {
                            case "MaDoiTac":
                                lst = lst.OrderBy(p => p.MaDoiTuong).ToList();
                                break;
                            case "TenDoiTac":
                                lst = lst.OrderBy(p => p.TenDoiTuong).ToList();
                                break;
                            case "DienThoai":
                                lst = lst.OrderBy(p => p.DienThoai).ToList();
                                break;
                            case "NhomDoiTac":
                                lst = lst.OrderBy(p => p.TenNhomDT).ToList();
                                break;
                            case "NgaySinh":
                                lst = lst.OrderBy(p => p.NgaySinh_NgayTLap).ToList();
                                break;
                            case "Email":
                                lst = lst.OrderBy(p => p.Email).ToList();
                                break;
                            case "DiaChi":
                                lst = lst.OrderBy(p => p.DiaChi).ToList();
                                break;
                            case "TinhThanh":
                                lst = lst.OrderBy(p => p.KhuVuc).ToList();
                                break;
                            case "QuanHuyen":
                                lst = lst.OrderBy(p => p.PhuongXa).ToList();
                                break;
                            case "NoHienTai":
                                lst = lst.OrderBy(p => p.NoHienTai).ToList();
                                break;
                            case "TongBan":
                                lst = lst.OrderBy(p => p.TongBan).ToList();
                                break;
                            case "BanTruTraHang":
                                lst = lst.OrderBy(p => p.TongBanTruTraHang).ToList();
                                break;
                            case "TongTichDiem":
                                lst = lst.OrderBy(p => p.TongTichDiem).ToList();
                                break;
                            case "TongMua":
                                lst = lst.OrderBy(p => p.TongMua).ToList();
                                break;
                            case "NguoiTao":
                                lst = lst.OrderBy(p => p.NguoiTao).ToList();
                                break;
                            case "NguonKhach":
                                lst = lst.OrderBy(p => p.TenNguonKhach).ToList();
                                break;
                            case "NguoiGioiThieu":
                                lst = lst.OrderBy(p => p.NguoiGioiThieu).ToList();
                                break;
                            case "NgayTao":
                                lst = lst.OrderBy(p => p.NgayTao).ToList();
                                break;

                        }
                    }
                    else
                    {
                        switch (columsort)
                        {
                            case "MaDoiTac":
                                lst = lst.OrderByDescending(p => p.MaDoiTuong).ToList();
                                break;
                            case "TenDoiTac":
                                lst = lst.OrderByDescending(p => p.TenDoiTuong).ToList();
                                break;
                            case "DienThoai":
                                lst = lst.OrderByDescending(p => p.DienThoai).ToList();
                                break;
                            case "NhomDoiTac":
                                lst = lst.OrderByDescending(p => p.TenNhomDT).ToList();
                                break;
                            case "NgaySinh":
                                lst = lst.OrderByDescending(p => p.NgaySinh_NgayTLap).ToList();
                                break;
                            case "Email":
                                lst = lst.OrderByDescending(p => p.Email).ToList();
                                break;
                            case "DiaChi":
                                lst = lst.OrderByDescending(p => p.DiaChi).ToList();
                                break;
                            case "TinhThanh":
                                lst = lst.OrderByDescending(p => p.KhuVuc).ToList();
                                break;
                            case "QuanHuyen":
                                lst = lst.OrderByDescending(p => p.PhuongXa).ToList();
                                break;
                            case "NoHienTai":
                                lst = lst.OrderByDescending(p => p.NoHienTai).ToList();
                                break;
                            case "TongBan":
                                lst = lst.OrderByDescending(p => p.TongBan).ToList();
                                break;
                            case "BanTruTraHang":
                                lst = lst.OrderByDescending(p => p.TongBanTruTraHang).ToList();
                                break;
                            case "TongTichDiem":
                                lst = lst.OrderByDescending(p => p.TongTichDiem).ToList();
                                break;
                            case "TongMua":
                                lst = lst.OrderByDescending(p => p.TongMua).ToList();
                                break;
                            case "NguoiTao":
                                lst = lst.OrderByDescending(p => p.NguoiTao).ToList();
                                break;
                            case "NguonKhach":
                                lst = lst.OrderByDescending(p => p.TenNguonKhach).ToList();
                                break;
                            case "NguoiGioiThieu":
                                lst = lst.OrderByDescending(p => p.NguoiGioiThieu).ToList();
                                break;
                            case "NgayTao":
                                lst = lst.OrderByDescending(p => p.NgayTao).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lst = lst.OrderByDescending(x => x.NgayTao).ToList();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetListKhachHang_Where_1Param: " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        public string GetStringWhere(string wherePr, string whereitem)
        {
            return wherePr == string.Empty ? whereitem : string.Concat(wherePr, " AND ", whereitem);
        }


        /// <summary>
        /// get list khachhang , paging in sql
        /// </summary>
        /// <param name="listParams"></param>
        /// <returns></returns>
        public List<SP_DM_DoiTuong> SP_GetListKhachHang_Where_Paging(Params_GetListKhachHang listParams)
        {
            var idChiNhanh = string.Join(",", listParams.ID_DonVis);
            var idNhomDTSearch = string.Empty;
            int loaiDoiTuong = listParams.LoaiDoiTuong;
            if (listParams.ID_NhomDoiTuong == null)
            {
                idNhomDTSearch = "%%";
            }
            else
            {
                idNhomDTSearch = "%" + listParams.ID_NhomDoiTuong + "%";
            }

            var maDoiTuong = listParams.MaDoiTuong;
            string txtSearch = "%%";
            if (!string.IsNullOrWhiteSpace(maDoiTuong))
            {
                txtSearch = maDoiTuong;
            }

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("timeStart", listParams.TongBan_TuNgay));
            lstParam.Add(new SqlParameter("timeEnd", listParams.TongBan_DenNgay));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("MaKH", txtSearch));
            lstParam.Add(new SqlParameter("LoaiKH", loaiDoiTuong));
            lstParam.Add(new SqlParameter("ID_NhomKhachHang", idNhomDTSearch));
            lstParam.Add(new SqlParameter("timeStartKH", listParams.NgayTao_TuNgay));
            lstParam.Add(new SqlParameter("timeEndKH", listParams.NgayTao_DenNgay));
            lstParam.Add(new SqlParameter("CurrentPage", listParams.CurrentPage));
            lstParam.Add(new SqlParameter("PageSize", listParams.PageSize));

            List<SP_DM_DoiTuong> lst = null;
            try
            {
                string whereSql = listParams.WhereSql;
                string where1 = string.Empty;
                if (listParams.LoaiDoiTuong == 1)
                {
                    if (listParams.TongBan_Tu > 0)
                    {
                        whereSql = " ISNULL(TongBan,0) >=" + listParams.TongBan_Tu;
                    }

                    if (listParams.TongBan_Den > 0)
                    {
                        where1 = " ISNULL(TongBan,0) <=" + listParams.TongBan_Den;
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                }
                else
                {
                    if (listParams.TongBan_Tu > 0)
                    {
                        where1 = " ISNULL(TongMua,0) >=" + listParams.TongBan_Tu;
                        whereSql = GetStringWhere(whereSql, where1);
                    }

                    if (listParams.TongBan_Den > 0)
                    {
                        where1 = " ISNULL(TongMua,0) <=" + listParams.TongBan_Den;
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                }

                switch (listParams.LoaiKhach)
                {
                    case 1:
                        where1 = " LaCaNhan = '1' ";
                        whereSql = GetStringWhere(whereSql, where1);
                        break;
                    case 2:
                        where1 = " LaCaNhan = '0' ";
                        whereSql = GetStringWhere(whereSql, where1);
                        break;
                }

                switch (listParams.GioiTinh)
                {
                    case 1:
                        where1 = " GioiTinhNam = '1' ";
                        whereSql = GetStringWhere(whereSql, where1);
                        break;
                    case 2:
                        where1 = " GioiTinhNam = '0' ";
                        whereSql = GetStringWhere(whereSql, where1);
                        break;
                }

                if (listParams.ID_NguonKhach != null)
                {
                    where1 = " ID_NguonKhach = '" + listParams.ID_NguonKhach + "' ";
                    whereSql = GetStringWhere(whereSql, where1);
                }

                var lstIDTinhThanh = listParams.ID_TinhThanhs;
                if (listParams.ID_TinhThanhs != null)
                {
                    if (lstIDTinhThanh.Count > 0)
                    {
                        where1 = string.Concat(" ID_TinhThanh in (select * from splitstring('", string.Join(",", lstIDTinhThanh), "')) ");
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                }

                var ngaySinhFrom = listParams.NgaySinh_TuNgay;
                var ngaySinhTo = listParams.NgaySinh_DenNgay;

                if (listParams.NgaySinh_TuNgay != null && listParams.NgaySinh_DenNgay != null)
                {
                    var monthFrom = ngaySinhFrom.Value.Month;
                    var monthTo = ngaySinhTo.Value.Month;
                    var dateFrom = ngaySinhFrom.Value.Day;
                    var dateTo = ngaySinhTo.Value.Day;

                    if (listParams.NgaySinh_TuNgay == listParams.NgaySinh_DenNgay)
                    {
                        //lst = lst.Where(dt => dt.NgaySinh_NgayTLap != null
                        where1 = string.Concat(" NgaySinh_NgayTLap is not null AND DATEPART(month,NgaySinh_NgayTLap)= ", monthFrom, " AND DATEPART(day,NgaySinh_NgayTLap)= ", dateFrom);
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                    else
                    {
                        if (ngaySinhFrom != new DateTime(1918, 1, 1))
                        {
                            // get KH with NgaySinh != null
                            where1 = " NgaySinh_NgayTLap is not null ";
                            whereSql = GetStringWhere(whereSql, where1);

                            // compare nam
                            if (listParams.LoaiNgaySinh == 1)
                            {
                                where1 = " DATEPART(year,NgaySinh_NgayTLap) = " + ngaySinhFrom.Value.Year;
                                whereSql = GetStringWhere(whereSql, where1);
                            }
                            else
                            {
                                // compare month/day
                                if (monthFrom == monthTo)
                                {
                                    // compare date (dateFrom <= ngaysinh <= dateTo)
                                    where1 = string.Concat(" DATEPART(month,NgaySinh_NgayTLap) = ", monthFrom, " AND DATEPART(day,NgaySinh_NgayTLap) >= ", dateFrom,
                                       " AND DATEPART(day, NgaySinh_NgayTLap) <= ", dateTo, "");
                                    whereSql = GetStringWhere(whereSql, where1);
                                }
                                else
                                {
                                    // tu 11/09 - 01/12 --> get all KH sinh nhat thang 10,11 OR (KH co ngay sinh >=11 va thang sinh = 9) OR (KH co ngay sinh <=01 va thang 12)
                                    //monthbetween = monthbetween.TrimEnd(',');
                                    where1 = string.Concat(" (DATEPART(month,NgaySinh_NgayTLap) > ", monthFrom, " AND DATEPART(month,NgaySinh_NgayTLap) < ", monthTo,
                                      " ) OR ( DATEPART(month, NgaySinh_NgayTLap) = ", monthFrom, " AND DATEPART(day, NgaySinh_NgayTLap) >= ", dateFrom,
                                      " ) OR ( DATEPART(month, NgaySinh_NgayTLap) = ", monthTo, " AND DATEPART(day, NgaySinh_NgayTLap) <= ", dateTo, " )");
                                    whereSql = GetStringWhere(whereSql, where1);
                                }
                            }
                        }
                    }
                }

                var customerDebit = listParams.No_TrangThai;
                switch (customerDebit)
                {
                    case 1:// còn nợ
                        if (loaiDoiTuong == 2)
                        {
                            where1 = " ISNULL(NoHienTai,0) * (-1) > 0 ";
                            whereSql = GetStringWhere(whereSql, where1);
                        }
                        else
                        {
                            where1 = " ISNULL(NoHienTai,0) > 0 ";
                            whereSql = GetStringWhere(whereSql, where1);
                        }
                        break;
                    case 2:  // hết nợ
                        if (loaiDoiTuong == 2)
                        {
                            where1 = " ISNULL(NoHienTai,0) * (-1) = 0 ";
                            whereSql = GetStringWhere(whereSql, where1);
                        }
                        else
                        {
                            where1 = " ISNULL(NoHienTai,0) = 0 ";
                            whereSql = GetStringWhere(whereSql, where1);
                        }
                        break;
                }

                //double noHienTaiFromSearch = 0;
                var noHienTaiFrom = listParams.NoHienTai_Tu;
                var noHienTaiTo = listParams.NoHienTai_Den;

                if (noHienTaiFrom != null)
                {
                    if (loaiDoiTuong == 2)
                    {
                        where1 = " ISNULL(NoHienTai,0) * (-1) >=  " + noHienTaiFrom;
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                    else
                    {
                        where1 = " ISNULL(NoHienTai,0) >=  " + noHienTaiFrom;
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                }

                if (noHienTaiTo != null)
                {
                    if (loaiDoiTuong == 2)
                    {
                        where1 = " ISNULL(NoHienTai,0) * (-1) <=  " + noHienTaiTo;
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                    else
                    {
                        where1 = " ISNULL(NoHienTai,0) <=  " + noHienTaiTo;
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                }

                var lstReturnIDManager = listParams.ID_NhanVienQuanLys;
                if (lstReturnIDManager != null)
                {
                    if (lstReturnIDManager.Count > 0)
                    {
                        var nguoiTao = listParams.NguoiTao;
                        string idManagers = string.Join(",", lstReturnIDManager);
                        //where1 = string.Concat(" (exists (select Name from splitstring('", idManagers, "') tblMng where ID_NhanVienPhuTrach = tblMng.Name ) OR ID_NhanVienPhuTrach is null OR NguoiTao like '%", nguoiTao, "%' )");
                        where1 = string.Concat(" (exists (select Name from splitstring('", idManagers, "') tblMng where ID_NhanVienPhuTrach = tblMng.Name ))");
                        whereSql = GetStringWhere(whereSql, where1);
                    }
                }

                var idTrangThai = listParams.ID_TrangThai;
                if (idTrangThai != null)
                {
                    where1 = string.Concat(" ID_TrangThai = '", idTrangThai, "'");
                    whereSql = GetStringWhere(whereSql, where1);
                }

                lstParam.Add(new SqlParameter("Where", whereSql));

                var sort = listParams.TrangThai_SapXep;
                var columsort = listParams.Cot_SapXep;
                string sortby = string.Empty;
                if (sort != 0)
                {
                    switch (columsort)
                    {
                        case "MaDoiTac":
                            sortby = " MaDoiTuong";
                            break;
                        case "TenDoiTac":
                            sortby = " TenDoiTuong";
                            break;
                        case "DienThoai":
                            sortby = " DienThoai";
                            break;
                        case "NhomDoiTac":
                            sortby = " TenNhomDT";
                            break;
                        case "NgaySinh":
                            sortby = " NgaySinh_NgayTLap";
                            break;
                        case "Email":
                            sortby = " Email";
                            break;
                        case "DiaChi":
                            sortby = " DiaChi";
                            break;
                        case "TinhThanh":
                            sortby = " KhuVuc";
                            break;
                        case "QuanHuyen":
                            sortby = " PhuongXa";
                            break;
                        case "NoHienTai":
                            sortby = " NoHienTai";
                            break;
                        case "TongBan":
                            sortby = " TongBan";
                            break;
                        case "TongBanTruTraHang":
                            sortby = " TongBanTruTraHang";
                            break;
                        case "TongTichDiem":
                            sortby = " TongTichDiem";
                            break;
                        case "TongMua":
                            sortby = " TongMua";
                            break;
                        case "NguoiTao":
                            sortby = " NguoiTao";
                            break;
                        case "NguonKhach":
                            sortby = " TenNguonKhach";
                            break;
                        case "NguoiGioiThieu":
                            sortby = " NguoiGioiThieu";
                            break;
                        case "NgayTao":
                            sortby = " dt.NgayTao";
                            break;
                        case "NgayGiaoDichGanNhat":
                            sortby = " dt.NgayGiaoDichGanNhat";
                            break;
                        case "PhiDichVu":
                            sortby = " PhiDichVu";
                            break; 
                        case "NapCoc":
                            sortby = " NapCoc";
                            break;
                        case "SuDungCoc":
                            sortby = " SuDungCoc";
                            break;
                        case "SoDuCoc":
                            sortby = " SoDuCoc";
                            break;
                    }

                    if (sort == 2 && !string.IsNullOrEmpty(sortby))
                    {
                        sortby = string.Concat(sortby, " DESC");
                    }
                }
                lstParam.Add(new SqlParameter("SortBy", sortby));

                lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo_Paging @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH," +
                         "@CurrentPage, @PageSize, @Where, @SortBy", lstParam.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetListKhachHang_Where_Paging: " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        /// <summary>
        /// get danh sach khach hang, return IEnumerable
        /// </summary>
        /// <param name="listParams"></param>
        /// <returns></returns>
        public List<SP_DM_DoiTuong> SP_GetListKhachHang_Where_ReturnIEnumerable(Params_GetListKhachHang listParams)
        {
            var idChiNhanh = listParams.ID_DonVis.FirstOrDefault();
            var tbl_timeCSt = from cs in db.ChotSo
                              where cs.ID_DonVi == idChiNhanh
                              select cs;

            var idNhomDTSearch = string.Empty;
            if (listParams.ID_NhomDoiTuong == null)
            {
                idNhomDTSearch = "%%";
            }
            else
            {
                idNhomDTSearch = "%" + listParams.ID_NhomDoiTuong + "%";
            }

            var loaiDoiTuong = listParams.LoaiDoiTuong;

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Clear();
            lstParam.Add(new SqlParameter("timeStart", listParams.TongBan_TuNgay));
            lstParam.Add(new SqlParameter("timeEnd", listParams.TongBan_DenNgay));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("MaKH", "%%"));
            lstParam.Add(new SqlParameter("LoaiKH", loaiDoiTuong));
            lstParam.Add(new SqlParameter("ID_NhomKhachHang", idNhomDTSearch));
            lstParam.Add(new SqlParameter("timeStartKH", listParams.NgayTao_TuNgay));
            lstParam.Add(new SqlParameter("timeEndKH", listParams.NgayTao_DenNgay));

            //System.Data.Entity.Infrastructure.DbRawSqlQuery<SP_DM_DoiTuong> lst1 = null;
            IEnumerable<SP_DM_DoiTuong> lstReturn = null;

            try
            {
                if (tbl_timeCSt.Count() > 0)
                {
                    lstReturn = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo_ChotSo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH", lstParam.ToArray()).AsEnumerable();
                }
                else
                {
                    lstReturn = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH", lstParam.ToArray()).AsEnumerable();
                }

                if (loaiDoiTuong == 1)
                {
                    if (listParams.TongBan_Tu > 0)
                    {
                        lstReturn = lstReturn.Where(x => x.TongBan >= listParams.TongBan_Tu);
                    }

                    if (listParams.TongBan_Den > 0)
                    {
                        lstReturn = lstReturn.Where(x => x.TongBan <= listParams.TongBan_Den);
                    }
                }
                else
                {
                    if (listParams.TongBan_Tu > 0)
                    {
                        lstReturn = lstReturn.Where(x => x.TongMua >= listParams.TongBan_Tu);
                    }

                    if (listParams.TongBan_Den > 0)
                    {
                        lstReturn = lstReturn.Where(x => x.TongMua <= listParams.TongBan_Den);
                    }
                }

                switch (listParams.LoaiKhach)
                {
                    case 1:
                        lstReturn = lstReturn.Where(x => x.LaCaNhan);
                        break;
                    case 2:
                        lstReturn = lstReturn.Where(x => x.LaCaNhan == false);
                        break;
                }

                switch (listParams.GioiTinh)
                {
                    case 1:
                        lstReturn = lstReturn.Where(x => x.GioiTinhNam == true);
                        break;
                    case 2:
                        lstReturn = lstReturn.Where(x => x.GioiTinhNam == false);
                        break;
                }

                if (listParams.ID_NguonKhach != null)
                {
                    lstReturn = lstReturn.Where(x => x.ID_NguonKhach == new Guid(listParams.ID_NguonKhach));
                }

                var lstIDTinhThanh = listParams.ID_TinhThanhs;
                if (listParams.ID_TinhThanhs != null)
                {
                    if (lstIDTinhThanh.Count > 0)
                    {
                        lstReturn = lstReturn.Where(hd => lstIDTinhThanh.Contains(hd.ID_TinhThanh.ToString()));
                    }
                }

                var ngaySinhFrom = listParams.NgaySinh_TuNgay;
                var ngaySinhTo = listParams.NgaySinh_DenNgay;

                if (listParams.NgaySinh_TuNgay != null && listParams.NgaySinh_DenNgay != null)
                {
                    var monthFrom = ngaySinhFrom.Value.Month;
                    var monthTo = ngaySinhTo.Value.Month;
                    var dateFrom = ngaySinhFrom.Value.Day;
                    var dateTo = ngaySinhTo.Value.Day;

                    if (listParams.NgaySinh_TuNgay == listParams.NgaySinh_DenNgay)
                    {
                        //lst = lst.Where(dt => dt.NgaySinh_NgayTLap != null
                        lstReturn = lstReturn.Where(dt => dt.NgaySinh_NgayTLap != null
                        && dt.NgaySinh_NgayTLap.Value.Month == monthFrom && dt.NgaySinh_NgayTLap.Value.Day == dateFrom);
                    }
                    else
                    {
                        if (ngaySinhFrom != new DateTime(1918, 1, 1))
                        {
                            // get KH with NgaySinh != null
                            lstReturn = lstReturn.Where(dt => dt.NgaySinh_NgayTLap != null);

                            // compare nam
                            if (listParams.LoaiNgaySinh == 1)
                            {
                                lstReturn = lstReturn.Where(dt => dt.NgaySinh_NgayTLap.Value.Year == ngaySinhFrom.Value.Year);
                            }
                            else
                            {
                                // compare month/day
                                if (monthFrom == monthTo)
                                {
                                    // compare date (dateFrom <= ngaysinh <= dateTo)
                                    lstReturn = lstReturn.Where(dt => dt.NgaySinh_NgayTLap.Value.Month == monthFrom
                                   && dt.NgaySinh_NgayTLap.Value.Day >= dateFrom && dt.NgaySinh_NgayTLap.Value.Day <= dateTo);
                                }
                                else
                                {
                                    lstReturn = lstReturn.Where(dt => (dt.NgaySinh_NgayTLap.Value.Month > monthFrom && dt.NgaySinh_NgayTLap.Value.Month < monthTo)
                                   || ((dt.NgaySinh_NgayTLap.Value.Month == monthFrom && dt.NgaySinh_NgayTLap.Value.Day >= dateFrom)
                                   || (dt.NgaySinh_NgayTLap.Value.Month == monthTo && dt.NgaySinh_NgayTLap.Value.Day <= dateFrom)));
                                }
                            }
                        }
                    }
                }

                var customerDebit = listParams.No_TrangThai;
                switch (customerDebit)
                {
                    case 1:// còn nợ
                        if (loaiDoiTuong == 2)
                        {
                            lstReturn = lstReturn.Where(x => x.NoHienTai * (-1) > 0);
                        }
                        else
                        {
                            lstReturn = lstReturn.Where(x => x.NoHienTai > 0);
                        }
                        break;
                    case 2:  // hết nợ
                        if (loaiDoiTuong == 2)
                        {
                            lstReturn = lstReturn.Where(x => x.NoHienTai * (-1) == 0);
                        }
                        else
                        {
                            lstReturn = lstReturn.Where(x => x.NoHienTai == 0);
                        }
                        break;
                }

                //double noHienTaiFromSearch = 0;
                var noHienTaiFrom = listParams.NoHienTai_Tu;
                var noHienTaiTo = listParams.NoHienTai_Den;

                if (noHienTaiFrom != null)
                {
                    if (loaiDoiTuong == 2)
                    {
                        lstReturn = lstReturn.Where(x => x.NoHienTai * (-1) >= noHienTaiFrom);
                    }
                    else
                    {
                        lstReturn = lstReturn.Where(x => x.NoHienTai >= noHienTaiFrom);
                    }
                }

                if (noHienTaiTo != null)
                {
                    if (loaiDoiTuong == 2)
                    {
                        lstReturn = lstReturn.Where(x => x.NoHienTai * (-1) <= noHienTaiTo);
                    }
                    else
                    {
                        lstReturn = lstReturn.Where(x => x.NoHienTai <= noHienTaiTo);
                    }
                }

                var lstReturnIDManager = listParams.ID_NhanVienQuanLys;
                if (lstReturnIDManager != null)
                {
                    if (lstReturnIDManager.Count > 0)
                    {
                        var nguoiTao = listParams.NguoiTao;
                        lstReturn = lstReturn.Where(x => lstReturnIDManager.Contains(x.ID_NhanVienPhuTrach.ToString())
                        || x.ID_NhanVienPhuTrach == null || x.NguoiTao.Contains(nguoiTao));
                    }
                }

                var maDoiTuong = listParams.MaDoiTuong;
                if (!string.IsNullOrWhiteSpace(maDoiTuong))
                {
                    char[] whitespace = new char[] { ' ', '\t' };
                    string[] textFilter = maDoiTuong.ToLower().Split(whitespace);
                    string[] utf8 = textFilter.Where(o => o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    string[] utf = textFilter.Where(o => !o.Any(c => VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    var maDoiTuongUng = CommonStatic.ConvertToUnSign(maDoiTuong).ToLower();
                    lstReturn = lstReturn.Where(o =>
                    // find Ma, TenDT in store
                     o.DienThoai != null && o.DienThoai.Contains(@maDoiTuongUng)
                    || o.Email.ToLower().Contains(maDoiTuong.ToLower())
                    || o.NguoiTao.ToLower().Contains(maDoiTuong.ToLower())

                    // nguoi gioi thieu
                    || (utf8.All(c => o.NguoiGioiThieu.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.NguoiGioiThieu).ToLower().Contains(d)))

                    // tinh thanh
                    || (utf8.All(c => o.KhuVuc.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.KhuVuc).ToLower().Contains(d)))
                    // quan huyen
                    || (utf8.All(c => o.PhuongXa.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.PhuongXa).ToLower().Contains(d)))

                    // quan huyen
                    || (utf8.All(c => o.DiaChi.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.DiaChi).ToLower().Contains(d)))

                    // ghi chu
                    || (utf8.All(c => o.GhiChu.ToLower().Contains(c))
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.GhiChu).ToLower().Contains(d)))

                    || (utf8.All(c => o.TenDoiTuong.ToLower().IndexOf(c) >= 0)
                    && utf.All(d => CommonStatic.ConvertToUnSign(o.TenDoiTuong).ToLower().Contains(d)
                    || o.MaDoiTuong.ToLower().Contains(d))));
                }

                var idTrangThai = listParams.ID_TrangThai;
                if (idTrangThai != null)
                {
                    lstReturn = lstReturn.Where(x => x.ID_TrangThai == idTrangThai);
                }

                var sort = listParams.TrangThai_SapXep;
                var columsort = listParams.Cot_SapXep;
                if (sort != 0)
                {
                    if (sort == 1)
                    {
                        switch (columsort)
                        {
                            case "MaDoiTac":
                                lstReturn = lstReturn.OrderBy(p => p.MaDoiTuong);
                                break;
                            case "TenDoiTac":
                                lstReturn = lstReturn.OrderBy(p => p.TenDoiTuong);
                                break;
                            case "DienThoai":
                                lstReturn = lstReturn.OrderBy(p => p.DienThoai);
                                break;
                            case "NhomDoiTac":
                                lstReturn = lstReturn.OrderBy(p => p.TenNhomDT);
                                break;
                            case "NgaySinh":
                                lstReturn = lstReturn.OrderBy(p => p.NgaySinh_NgayTLap);
                                break;
                            case "Email":
                                lstReturn = lstReturn.OrderBy(p => p.Email);
                                break;
                            case "DiaChi":
                                lstReturn = lstReturn.OrderBy(p => p.DiaChi);
                                break;
                            case "TinhThanh":
                                lstReturn = lstReturn.OrderBy(p => p.KhuVuc);
                                break;
                            case "QuanHuyen":
                                lstReturn = lstReturn.OrderBy(p => p.PhuongXa);
                                break;
                            case "NoHienTai":
                                lstReturn = lstReturn.OrderBy(p => p.NoHienTai);
                                break;
                            case "TongBan":
                                lstReturn = lstReturn.OrderBy(p => p.TongBan);
                                break;
                            case "BanTruTraHang":
                                lstReturn = lstReturn.OrderBy(p => p.TongBanTruTraHang);
                                break;
                            case "TongTichDiem":
                                lstReturn = lstReturn.OrderBy(p => p.TongTichDiem);
                                break;
                            case "TongMua":
                                lstReturn = lstReturn.OrderBy(p => p.TongMua);
                                break;
                            case "NguoiTao":
                                lstReturn = lstReturn.OrderBy(p => p.NguoiTao);
                                break;
                            case "NguonKhach":
                                lstReturn = lstReturn.OrderBy(p => p.TenNguonKhach);
                                break;
                            case "NguoiGioiThieu":
                                lstReturn = lstReturn.OrderBy(p => p.NguoiGioiThieu);
                                break;
                            case "NgayTao":
                                lstReturn = lstReturn.OrderBy(p => p.NgayTao);
                                break;

                        }
                    }
                    else
                    {
                        switch (columsort)
                        {
                            case "MaDoiTac":
                                lstReturn = lstReturn.OrderByDescending(p => p.MaDoiTuong);
                                break;
                            case "TenDoiTac":
                                lstReturn = lstReturn.OrderByDescending(p => p.TenDoiTuong);
                                break;
                            case "DienThoai":
                                lstReturn = lstReturn.OrderByDescending(p => p.DienThoai);
                                break;
                            case "NhomDoiTac":
                                lstReturn = lstReturn.OrderByDescending(p => p.TenNhomDT);
                                break;
                            case "NgaySinh":
                                lstReturn = lstReturn.OrderByDescending(p => p.NgaySinh_NgayTLap);
                                break;
                            case "Email":
                                lstReturn = lstReturn.OrderByDescending(p => p.Email);
                                break;
                            case "DiaChi":
                                lstReturn = lstReturn.OrderByDescending(p => p.DiaChi);
                                break;
                            case "TinhThanh":
                                lstReturn = lstReturn.OrderByDescending(p => p.KhuVuc);
                                break;
                            case "QuanHuyen":
                                lstReturn = lstReturn.OrderByDescending(p => p.PhuongXa);
                                break;
                            case "NoHienTai":
                                lstReturn = lstReturn.OrderByDescending(p => p.NoHienTai);
                                break;
                            case "TongBan":
                                lstReturn = lstReturn.OrderByDescending(p => p.TongBan);
                                break;
                            case "BanTruTraHang":
                                lstReturn = lstReturn.OrderByDescending(p => p.TongBanTruTraHang);
                                break;
                            case "TongTichDiem":
                                lstReturn = lstReturn.OrderByDescending(p => p.TongTichDiem);
                                break;
                            case "TongMua":
                                lstReturn = lstReturn.OrderByDescending(p => p.TongMua);
                                break;
                            case "NguoiTao":
                                lstReturn = lstReturn.OrderByDescending(p => p.NguoiTao);
                                break;
                            case "NguonKhach":
                                lstReturn = lstReturn.OrderByDescending(p => p.TenNguonKhach);
                                break;
                            case "NguoiGioiThieu":
                                lstReturn = lstReturn.OrderByDescending(p => p.NguoiGioiThieu);
                                break;
                            case "NgayTao":
                                lstReturn = lstReturn.OrderByDescending(p => p.NgayTao);
                                break;
                        }
                    }
                }
                else
                {
                    lstReturn = lstReturn.OrderByDescending(x => x.NgayTao);
                }
                return lstReturn.ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetListKhachHang_Where_ReturnIEnumerable: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// get list KH with ID_NhanVien, ID_ChiNhanh
        /// </summary>
        /// <param name="listParams"></param>
        /// <returns></returns>
        public List<SP_DM_DoiTuong> SP_GetListKhachHang_ByNhanVien(Params_GetListKhachHang listParams)
        {
            var idChiNhanh = listParams.ID_DonVis.FirstOrDefault();
            var tbl_timeCSt = from cs in db.ChotSo
                              where cs.ID_DonVi == idChiNhanh
                              select cs;

            var idNhomDTSearch = string.Empty;
            if (listParams.ID_NhomDoiTuong == null)
            {
                idNhomDTSearch = "%%";
            }
            else
            {
                idNhomDTSearch = "%" + listParams.ID_NhomDoiTuong + "%";
            }

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("timeStart", listParams.TongBan_TuNgay));
            lstParam.Add(new SqlParameter("timeEnd", listParams.TongBan_DenNgay));
            lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("MaKH", "%%"));
            lstParam.Add(new SqlParameter("LoaiKH", listParams.LoaiDoiTuong));
            lstParam.Add(new SqlParameter("ID_NhomKhachHang", idNhomDTSearch));
            lstParam.Add(new SqlParameter("timeStartKH", listParams.NgayTao_TuNgay));
            lstParam.Add(new SqlParameter("timeEndKH", listParams.NgayTao_DenNgay));

            List<SP_DM_DoiTuong> lst = new List<SP_DM_DoiTuong>();
            try
            {
                if (tbl_timeCSt.Count() > 0)
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo_ChotSo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH", lstParam.ToArray()).ToList();
                }
                else
                {
                    lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec DanhMucKhachHang_CongNo @timeStart, @timeEnd, @ID_ChiNhanh, @MaKH, @LoaiKH,@ID_NhomKhachHang ,@timeStartKH, @timeEndKH"
                       , lstParam.ToArray()).ToList();
                }

                var lstIDManager = listParams.ID_NhanVienQuanLys;
                if (lstIDManager != null)
                {
                    if (lstIDManager.Count > 0)
                    {
                        var nguoiTao = listParams.NguoiTao;
                        lst = lst.Where(x => lstIDManager.Contains(x.ID_NhanVienPhuTrach.ToString())
                        || x.ID_NhanVienPhuTrach == null || x.NguoiTao.Contains(nguoiTao)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetListKhachHang_ByNhanVien: " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idChiNhanh"></param>
        /// <param name="idNhanVien"></param>
        /// <returns></returns>
        public List<SP_DM_DoiTuong> GetAllKhachHang_NotWhere(Guid? idChiNhanh, Guid? idNhanVien)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            lstParam.Add(new SqlParameter("ID_NhanVienLogin", idNhanVien));

            try
            {
                return db.Database.SqlQuery<SP_DM_DoiTuong>("exec GetAllKhachHang_NotWhere @ID_ChiNhanh, @ID_NhanVienLogin", lstParam.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetAllKhachHang_NotWhere: " + ex);
                return new List<SP_DM_DoiTuong>();
            }
        }

        public List<Guid> GetCustomer_wasDelete()
        {
            var data = db.DM_DoiTuong.Where(x => x.TheoDoi == true).Select(x => x.ID).ToList();
            return data;
        }

        /// <summary>
        /// Get infor MuaHang KhachHang by ID_DoiTuong (SP_DM_DoiTuong {Ma,Ten,ID_NhomDoiTuong, TongMuaTruTraHang, TongTichDiem,..})
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <returns></returns>
        public IEnumerable<SP_DM_DoiTuong> SP_GetInforMuaHang_ofKhachHang(Guid idDoiTuong)
        {
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("ID", idDoiTuong));
            List<SP_DM_DoiTuong> lst = null;
            try
            {
                lst = db.Database.SqlQuery<SP_DM_DoiTuong>("exec GetInforMuaHang_ofKhachHang @ID", lstParam.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetInforMuaHang_ofKhachHang " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        /// <summary>
        /// Get SoLuong HangHoa (by ID_QuiDoi) of Khachhang (Mua - Tra)
        /// </summary>
        /// <returns></returns>
        public List<SP_KhachHang_HangHoa> SP_GetSoLuongHangMua_ofKhachHang()
        {
            List<SP_KhachHang_HangHoa> lst = null;
            try
            {
                lst = db.Database.SqlQuery<SP_KhachHang_HangHoa>("exec SP_GetSoLuongHangMua_ofKhachHang").ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetSoLuongHangMua_ofKhachHang " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        /// <summary>
        /// get NoKhachHang to ToDate
        /// </summary>
        /// <param name="ID_DoiTuong"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public double GetDebitCustomer_allBrands(Guid idDoiTuong)
        {
            double nohientai = 0;
            try
            {
                var sql = string.Concat("SELECT dbo.GetDebitCustomer_allBrands('", idDoiTuong, "')");
                nohientai = db.Database.SqlQuery<double>(sql).First();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetDebitCustomer_allBrands " + ex.InnerException + ex.Message);
            }
            return nohientai;
        }


        /// <summary>
        /// Get SoLuong HangHoa (by ID_QuiDoi) of Khachhang (Mua - Tra) (not use store procedure)
        /// </summary>
        /// <returns></returns>
        public IQueryable<Object> GetSoLuongHangMua_ofKhachHang()
        {
            if (db != null)
            {
                try
                {
                    var hdMua = from hd in db.BH_HoaDon
                                join ct in db.BH_HoaDon_ChiTiet
                                on hd.ID equals ct.ID_HoaDon
                                where hd.ID_DoiTuong != null && hd.LoaiHoaDon == 1
                                group new { hd, ct }
                                by new
                                {
                                    ID_DoiTuong = hd.ID_DoiTuong,
                                    ID_DonViQuiDoi = ct.ID_DonViQuiDoi,
                                } into g
                                select new
                                {
                                    ID_DoiTuong = g.Key.ID_DoiTuong,
                                    ID_DonViQuiDoi = g.Key.ID_DonViQuiDoi,
                                    SoLuong = g.Sum(x => x.ct.SoLuong)
                                };

                    var hdTra = from hd in db.BH_HoaDon
                                join ct in db.BH_HoaDon_ChiTiet
                                on hd.ID equals ct.ID_HoaDon
                                where hd.ID_DoiTuong != null && hd.LoaiHoaDon == 6
                                group new { hd, ct }
                                by new
                                {
                                    ID_DoiTuong = hd.ID_DoiTuong,
                                    ID_DonViQuiDoi = ct.ID_DonViQuiDoi,
                                } into g
                                select new
                                {
                                    ID_DoiTuong = g.Key.ID_DoiTuong,
                                    ID_DonViQuiDoi = g.Key.ID_DonViQuiDoi,
                                    SoLuong = -g.Sum(x => x.ct.SoLuong)
                                };

                    var dtUnion = hdMua.Union(hdTra);

                    var dtGroup = from hd in dtUnion
                                  group new { hd }
                                 by new
                                 {
                                     ID_DoiTuong = hd.ID_DoiTuong,
                                     ID_DonViQuiDoi = hd.ID_DonViQuiDoi,
                                 } into g
                                  select new
                                  {
                                      ID_DoiTuong = g.Key.ID_DoiTuong,
                                      ID_DonViQuiDoi = g.Key.ID_DonViQuiDoi,
                                      SoLuong = g.Sum(x => x.hd.SoLuong)
                                  };
                    return dtGroup;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetSoLuongHangMua_ofKhachHang " + ex.InnerException + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public List<SP_KhachHang_TheGiaTri> GetTienDatCoc_ofDoiTuong(Guid idDoiTuong, Guid idDonVi)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
            param.Add(new SqlParameter("ID_DonVi", idDonVi));
            return db.Database.SqlQuery<SP_KhachHang_TheGiaTri>("exec GetSoDuDatCoc_ofDoiTuong @ID_DoiTuong, @ID_DonVi", param.ToArray()).ToList();
        }

        public List<SoQuyDTO> GetNhatKyTienCoc_OfDoiTuong(ParamGetListBaoHiem_v1 param)
        {
            var idDoiTuong = Guid.Empty.ToString();
            if (!string.IsNullOrEmpty(param.TextSearch))// muon tam truong
            {
                idDoiTuong = param.TextSearch;
            }
            var idChiNhanhs = string.Empty;
            if (param.IdChiNhanhs != null && param.IdChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.IdChiNhanhs);
            }
            int curentPage = param.CurrentPage;
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
            sql.Add(new SqlParameter("IDDonVis", idChiNhanhs));
            sql.Add(new SqlParameter("FromDate", param.NgayTaoFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("ToDate", param.NgayTaoTo ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("CurrentPage", curentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            return db.Database.SqlQuery<SoQuyDTO>("exec GetNhatKyTienCoc_OfDoiTuong @ID_DoiTuong, @IDDonVis," +
                "@FromDate, @ToDate, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }


        /// <summary>
        ///  get so du the gia tri of khach hang (all chi nhanh) and return table {TongThuTheGiaTri, SuDungThe,HoanTraTheGiaTri, SoDuTheGiaTri}
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public List<SP_KhachHang_TheGiaTri> SP_GetSoDuTheGiaTri_ofKhachHang(Guid idDoiTuong, string datetime)
        {
            List<SP_KhachHang_TheGiaTri> lst = null;
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
                lstParam.Add(new SqlParameter("DateTime", datetime));
                lst = db.Database.SqlQuery<SP_KhachHang_TheGiaTri>("exec GetSoDuTheGiaTri_ofKhachHang @ID_DoiTuong, @DateTime", lstParam.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetSoDuTheGiaTri_ofKhachHang " + ex.InnerException + ex.Message);
            }
            return lst;
        }

        /// <summary>
        /// get get so du the gia tri cu khach hang theo thoi gian (return double)
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public double SP_GetSoDuTheGiaTri_ofKhachHang_byTime(Guid idDoiTuong, string time)
        {
            double sodu = 0;
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
                lstParam.Add(new SqlParameter("Time", time));
                sodu = double.Parse(db.Database.SqlQuery<double>("exec SP_GetSoDuTheGiaTri_ByTime @ID_DoiTuong, @Time", lstParam.ToArray()).FirstOrDefault().ToString());
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetSoDuTheGiaTri_ofKhachHang_byTime " + ex.InnerException + ex.Message);
            }
            return sodu;
        }

        public string SP_UpdateNhomDoiTuongs_inDMDoiTuong()
        {
            try
            {
                db.Database.ExecuteSqlCommand("exec UpdateNhomDoiTuongs_inDMDoiTuong");
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.InnerException + ex.Message;
            }
        }

        public static readonly string[] VietnameseSigns = new string[]

         { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};

        /// <summary>
        /// get list HoaDon (Loai 1, 6, 19) is Debit. If HoaDon from DatHang, get TongThu from HDDatHang
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="arrDonVi"></param>
        /// <param name="loaiDoiTuong"></param>
        /// <returns></returns>
        public List<BH_HoaDonDTO> SP_GetListHD_isDebit(Guid idDoiTuong, List<string> arrDonVi, int loaiDoiTuong)
        {
            var idDonVis = Guid.Empty.ToString();
            if (arrDonVi != null && arrDonVi.Count > 0)
            {
                idDonVis = string.Join(",", arrDonVi);
            }
            List<BH_HoaDonDTO> lstReturn = new List<BH_HoaDonDTO>();
            // get list HoaDon of KhachHang (gtrị PhaiThanhToan được tính sau khi trừ Trả hàng)
            List<SqlParameter> lstPram1 = new List<SqlParameter>();
            lstPram1.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
            lstPram1.Add(new SqlParameter("ID_DonVi", idDonVis));
            lstPram1.Add(new SqlParameter("LoaiDoiTuong", loaiDoiTuong));
            var tblHD = db.Database.SqlQuery<SP_HoaDonDebit>("EXEC SP_GetHDDebit_ofKhachHang @ID_DoiTuong, @ID_DonVi, @LoaiDoiTuong", lstPram1.ToArray()).ToList();

            List<SqlParameter> lstPram2 = new List<SqlParameter>();
            lstPram2.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
            lstPram2.Add(new SqlParameter("ID_DonVi", idDonVis));
            var tblJoinQuy = db.Database.SqlQuery<SP_QuyHoaDonLienQuan>("EXEC SP_GetQuyHoaDon_ofDoiTuong @ID_DoiTuong, @ID_DonVi", lstPram2.ToArray()).ToList();

            var tblLeftjoin = from hd in tblHD
                              join qhd_qct in tblJoinQuy on hd.ID equals qhd_qct.ID_HoaDonLienQuan into QCT_QHD
                              from qct_qhd in QCT_QHD.DefaultIfEmpty()
                              group new { hd, qct_qhd }
                              by new
                              {
                                  ID = hd.ID,
                                  MaHoaDon = hd.MaHoaDon,
                                  NgayLapHoaDon = hd.NgayLapHoaDon,
                                  PhaiThanhToan = hd.PhaiThanhToan,
                                  TinhChietKhauTheo = hd.TinhChietKhauTheo,
                                  TongThanhToan = hd.TongThanhToan,
                                  TongTienThue = hd.TongTienThue,
                                  LoaiHoaDon = hd.LoaiHoaDon,
                              } into g
                              select new
                              {
                                  ID = g.Key.ID,
                                  MaHoaDon = g.Key.MaHoaDon,
                                  NgayLapHoaDon = g.Key.NgayLapHoaDon,
                                  PhaiThanhToan = g.Key.PhaiThanhToan,
                                  TongThanhToan = g.Key.TongThanhToan,
                                  TongTienThue = g.Key.TongTienThue,
                                  TinhChietKhauTheo = g.Key.TinhChietKhauTheo,
                                  LoaiHoaDon = g.Key.LoaiHoaDon,
                                  TongTienThu = g.Sum(x => x.qct_qhd == null ? 0 : x.qct_qhd.TongTienThu ?? 0),
                              };

            foreach (var item in tblLeftjoin)
            {
                var debit = Math.Round(item.PhaiThanhToan) - Math.Round(item.TongTienThu);
                if (debit > 0)
                {
                    BH_HoaDonDTO dto = new BH_HoaDonDTO();
                    dto.ID = item.ID;
                    dto.MaHoaDon = item.MaHoaDon;
                    dto.NgayLapHoaDon = item.NgayLapHoaDon;
                    dto.PhaiThanhToan = item.PhaiThanhToan;
                    dto.TongThanhToan = item.TongThanhToan;
                    dto.TongTienThue = item.TongTienThue ?? 0;
                    dto.KhachDaTra = item.TongTienThu;
                    dto.SoThuTu = item.TinhChietKhauTheo; // mượn tạm trường STT
                    dto.TienMat = debit;  // mượn tạm trường TienMat
                    dto.LoaiHoaDon = item.LoaiHoaDon ?? 0;
                    lstReturn.Add(dto);
                }
            }
            return lstReturn;
        }

        #endregion

        #region insert
        public string Add_DoiTuong(DM_DoiTuong objAdd)
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
                    db.DM_DoiTuong.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = string.Concat(ex.InnerException, ex.Message);
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public string Update_DoiTuong(DM_DoiTuong obj)
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
                    #region DM_DoiTuong
                    DM_DoiTuong objUpd = db.DM_DoiTuong.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TenDoiTuong = obj.TenDoiTuong;
                    objUpd.DienThoai = obj.DienThoai;
                    objUpd.GioiTinhNam = obj.GioiTinhNam;
                    objUpd.Email = obj.Email;
                    objUpd.DiaChi = obj.DiaChi;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.GioiHanCongNo = obj.GioiHanCongNo;
                    objUpd.ID_DonVi = obj.ID_DonVi;
                    objUpd.ID_NganHang = obj.ID_NganHang;
                    objUpd.ID_NhomCu = obj.ID_NhomCu;
                    objUpd.ID_NhomDoiTuong = obj.ID_NhomDoiTuong ?? null;
                    objUpd.ID_QuanHuyen = obj.ID_QuanHuyen;
                    objUpd.ID_QuocGia = obj.ID_QuocGia;
                    objUpd.ID_TinhThanh = obj.ID_TinhThanh;
                    objUpd.LaCaNhan = obj.LaCaNhan;
                    objUpd.LoaiDoiTuong = obj.LoaiDoiTuong;
                    objUpd.MaDoiTuong = obj.MaDoiTuong;
                    objUpd.MaSoThue = obj.MaSoThue;
                    objUpd.NgayDoiNhom = obj.NgayDoiNhom;
                    objUpd.NgayGiaoDichGanNhat = obj.NgayGiaoDichGanNhat;
                    objUpd.NgaySinh_NgayTLap = obj.NgaySinh_NgayTLap;
                    objUpd.TaiKhoanNganHang = obj.TaiKhoanNganHang;
                    objUpd.NgayGiaoDichGanNhat = obj.NgayGiaoDichGanNhat;
                    objUpd.TenKhac = obj.TenKhac;
                    objUpd.The_TheKhachHang = obj.The_TheKhachHang;
                    objUpd.ThuongTru = obj.ThuongTru;
                    objUpd.Website = obj.Website;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NguoiTao = obj.NguoiTao;
                    objUpd.TenKhac = obj.TenKhac;
                    objUpd.TheoDoi = false;

                    objUpd.ID_NguonKhach = obj.ID_NguonKhach;
                    objUpd.ID_NguoiGioiThieu = obj.ID_NguoiGioiThieu;
                    objUpd.ID_NhanVienPhuTrach = obj.ID_NhanVienPhuTrach;
                    objUpd.ID_DonVi = obj.ID_DonVi;

                    objUpd.TenDoiTuong_ChuCaiDau = obj.TenDoiTuong_ChuCaiDau;
                    objUpd.TenDoiTuong_KhongDau = obj.TenDoiTuong_KhongDau;
                    objUpd.DinhDang_NgaySinh = obj.DinhDang_NgaySinh;
                    objUpd.ID_TrangThai = obj.ID_TrangThai;

                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message + ex.InnerException;
                }
            }
            return strErr;
        }
        #endregion

        #region delete
        public string Delete_DoiTuong(Guid id)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_DoiTuong objDel = db.DM_DoiTuong.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        List<BH_HoaDon> lstHD = db.BH_HoaDon.Where(dt => dt.ID_DoiTuong == id).ToList();
                        if (lstHD != null && lstHD.Count > 0)
                        {
                            // delete in BH_CTHoaDon
                            foreach (var item in lstHD)
                            {
                                List<BH_HoaDon_ChiTiet> lstCtHD = db.BH_HoaDon_ChiTiet.Where(ct => ct.ID_HoaDon == item.ID).ToList();
                                db.BH_HoaDon_ChiTiet.RemoveRange(lstCtHD);
                            }
                            // delete BH_HoaDon
                            db.BH_HoaDon.RemoveRange(lstHD);
                        }
                        db.DM_DoiTuong.Remove(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        strErr = "Delete_DoiTuong " + exxx.Message + exxx.InnerException;
                        Model.CookieStore.WriteLog(strErr);
                        return strErr;
                    }
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    Model.CookieStore.WriteLog(strErr);
                    return strErr;
                }
            }
            return strErr;
        }

        public string UpdateTheoDoi_DoiTuong(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_DoiTuong objDel = db.DM_DoiTuong.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        objDel.TheoDoi = true; // true: khong theo doi, false: dang theo doi
                        db.Entry(objDel).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        strErr = "classDM_DoiTuong - UpdateTheoDoi_DoiTuong" + exxx.Message + exxx.InnerException;
                        Model.CookieStore.WriteLog(strErr);
                        return strErr;
                    }
                }
                else
                {
                    strErr = "classDM_DoiTuong - Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    Model.CookieStore.WriteLog(strErr);
                    return strErr;
                }
            }
            return strErr;
        }
        #endregion

        #region Func Other
        public string GetautoCode(int loaiDoiTuong)
        {

            string format = string.Empty;
            string autoCode = string.Empty;

            switch (loaiDoiTuong)
            {
                case 1:
                    autoCode = "KH";
                    break;
                case 2:
                    autoCode = "NCC";
                    break;
                default:
                    autoCode = "TD";
                    break;
            }

            // maHD offline (contains ("O")
            var objTop = db.DM_DoiTuong.Where(p => p.MaDoiTuong.Contains(autoCode))
                .Where(p => p.LoaiDoiTuong == loaiDoiTuong && (p.MaDoiTuong.Length == 9));

            if (objTop != null && objTop.Count() > 0)
            {
                var maHDlast = objTop.OrderByDescending(p => p.MaDoiTuong).ThenByDescending(x => x.NgayTao).FirstOrDefault().MaDoiTuong;
                var number2 = Regex.Match(maHDlast, @"\d+").Value;
                var number = 0;
                if (number2 != string.Empty)
                {
                    number = int.Parse(number2);
                }

                int tempstt = number + 1;
                autoCode = autoCode + string.Format(format, tempstt);
            }
            else
            {
                autoCode = autoCode + string.Format(format, 1);
            }
            return autoCode;
        }
        public string GetMaNhanVien()
        {
            string format = "{0:0000}";
            string manhanvien = "NV0";
            string madv = db.NS_NhanVien.Where(p => p.MaNhanVien.Contains(manhanvien)).Where(p => p.MaNhanVien.Length == 7).OrderByDescending(p => p.MaNhanVien).Select(p => p.MaNhanVien).FirstOrDefault();
            if (madv == null)
            {
                manhanvien = manhanvien + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(manhanvien.Length, 4)) + 1;
                manhanvien = manhanvien + string.Format(format, tempstt);
            }
            return manhanvien;
        }
        public string SP_GetautoCode(int loaiDoiTuong)
        {
            string format = string.Empty;
            string autoCode = string.Empty;

            switch (loaiDoiTuong)
            {
                case 1:
                    autoCode = "KH";
                    break;
                case 2:
                    autoCode = "NCC";
                    break;
                case 3:
                    autoCode = "BH";
                    break;
                case 4:
                    autoCode = "GT";// nguoi gioithieu
                    break;
                default:
                    autoCode = "TD";
                    break;
            }

            var len = autoCode.Length;
            switch (len)
            {
                case 2:
                    format = "{0:0000000}";
                    break;
                case 3:
                    format = "{0:000000}";
                    break;
            }
            try
            {
                SqlParameter param = new SqlParameter("LoaiDoiTuong", loaiDoiTuong);
                var objReturn = db.Database.SqlQuery<SP_MaxCode>("EXEC GetMaDoiTuong_Max @LoaiDoiTuong", param).ToList();
                if (objReturn.Count() > 0)
                {
                    autoCode = autoCode + string.Format(format, objReturn.FirstOrDefault().MaxCode + 1);
                }
                else
                {
                    autoCode = autoCode + string.Format(format, 1);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("classDM_Doituong.SP_GetautoCode " + ex.InnerException + ex.Message);

                // if SP return null
                Random rd = new Random();
                autoCode = autoCode + string.Format(format, rd.Next(1, 1000000));
            }
            return autoCode;
        }

        public string GetMaDoiTuongMax_byTemp(int loaiDoiTuong, Guid? idDonVi)
        {
            string autoCode = string.Empty;
            int loaiHoaDon = 0;
            switch (loaiDoiTuong)
            {
                case 1:
                    loaiHoaDon = 33;
                    break;
                case 2:
                    loaiHoaDon = 34;
                    break;
            }
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
                lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                var objReturn = db.Database.SqlQuery<SP_MaxCodeTemp>("EXEC GetMaDoiTuongMax_byTemp @LoaiHoaDon, @ID_DonVi", lstParam.ToArray()).FirstOrDefault();
                autoCode = objReturn.MaxCode;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetMaDoiTuongMax_byTemp: " + ex.InnerException + ex.Message + ex.HResult);
            }
            return autoCode;
        }
        // check time to date
        public bool check_TimetoDate(string time)
        {
            try
            {
                string[] ar_time = time.Split(':');
                DateTime timeToUse = new DateTime(2000, 01, 01, int.Parse(ar_time[0]), int.Parse(ar_time[1]), 00);
                return true;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("check_TimetoDate " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }
        /// <summary>
        /// Check MaCaLamViec exist in DB
        /// </summary>
        /// <param name="MaCaLamViec"></param>
        /// <returns></returns>
        public bool SP_CheckMaCaLamViec_Exist(string MaCaLamViec)
        {
            try
            {
                NS_CaLamViec objReturn = db.NS_CaLamViec.Where(x => x.MaCa.Contains(MaCaLamViec)).FirstOrDefault();
                if (objReturn != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckMaCaLamViec_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }
        /// <summary>
        /// Check MaNhanVien exist in DB
        /// </summary>
        /// <param name="MaNhanVien"></param>
        /// <returns></returns>
        public bool SP_CheckMaNhanVien_Exist(string MaNhanVien)
        {
            try
            {
                NS_NhanVien objReturn = db.NS_NhanVien.Where(x => x.MaNhanVien.Contains(MaNhanVien)).FirstOrDefault();
                if (objReturn != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckMaNhanVien_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }
        /// <summary>
        /// Check MaPhongBan exist in DB
        /// </summary>
        /// <param name="MaPhongBan"></param>
        /// <returns></returns>
        public bool SP_CheckMaPhongBan_Exist(string MaPhongBan)
        {
            try
            {
                NS_PhongBan objReturn = db.NS_PhongBan.Where(x => x.MaPhongBan.Contains(MaPhongBan)).FirstOrDefault();
                if (objReturn != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckMaPhongBan_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }
        /// <summary>
        /// Check MaDoiTuong exist in DB
        /// </summary>
        /// <param name="maDoiTuong"></param>
        /// <returns></returns>
        public bool SP_CheckMaDoiTuong_Exist(string maDoiTuong)
        {
            try
            {
                SqlParameter parma = new SqlParameter("MaDoiTuong", maDoiTuong);
                var objReturn = db.Database.SqlQuery<SP_ReturnBool>("EXEC CheckMaDoiTuong_Exist @MaDoiTuong", parma).ToList();
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
                CookieStore.WriteLog("CheckMaDoiTuong_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }
        /// <summary>
        /// Check DienThoai KH exist in DB
        /// </summary>
        /// <param name="dienthoai"></param>
        /// <returns></returns>
        public bool SP_CheckSoDienThoai_Exist(string dienthoai, Guid? idDoiTuong = null)
        {
            try
            {
                var data = db.HT_CauHinhPhanMem.Select(x => new { x.ChoPhepTrungSoDienThoai });
                // ChoPhepTrungSoDienThoai: setup all chinhanh same 
                if (data.Count() > 0 && data.FirstOrDefault().ChoPhepTrungSoDienThoai != 1 || data.Count() == 0)
                {
                    if (dienthoai != null)
                    {
                        dienthoai = dienthoai.Trim();
                        if (dienthoai != string.Empty)
                        {
                            if (idDoiTuong == null || idDoiTuong == Guid.Empty)
                            {
                                return db.DM_DoiTuong.Select(x => new { x.ID, x.DienThoai, x.TheoDoi }).Count(e => e.DienThoai == dienthoai && e.TheoDoi == false) > 0;
                            }
                            else
                            {
                                return db.DM_DoiTuong.Select(x => new { x.ID, x.DienThoai, x.TheoDoi }).Count(e => e.ID != idDoiTuong && e.DienThoai == dienthoai && e.TheoDoi == false) > 0;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckSoDienThoai_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }

        /// <summary>
        /// check email exist in DM_DoiTuong
        /// </summary>
        /// <param name="email"></param>
        /// <param name="idDoiTuong"></param>
        /// <returns></returns>
        public bool CheckEmail_Exist(string email, Guid? idDoiTuong = null)
        {
            try
            {
                if (email != null)
                {
                    email = email.Trim();
                    if (email != string.Empty)
                    {
                        if (idDoiTuong == null || idDoiTuong == Guid.Empty)
                        {
                            return db.DM_DoiTuong.Count(e => e.Email.Contains(email) && e.TheoDoi == false) > 0;
                        }
                        else
                        {
                            return db.DM_DoiTuong.Count(e => e.ID != idDoiTuong && e.Email.Contains(email) && e.TheoDoi == false) > 0;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("CheckEmail_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }

        public bool TR_CheckSoDienThoai_Exist(string dienthoai)
        {
            try
            {
                var objReturn = db.NS_NhanVien.Where(x => x.DienThoaiDiDong.Contains(dienthoai));
                if (objReturn != null && objReturn.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("TR_CheckSoDienThoai_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }
        public bool SP_CheckEmail_Exist(string email)
        {
            try
            {
                var objReturn = db.NS_NhanVien.Where(x => x.Email.Contains(email));
                if (objReturn != null && objReturn.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_CheckEmail_Exist " + ex.InnerException + ex.Message + ex.HResult);
                return false;
            }
        }
        public List<DM_TinhThanh> GetListTinhThanh(Expression<Func<DM_TinhThanh, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_TinhThanh.ToList();
            }
        }

        public List<DM_QuanHuyen> GetListQuanHuyen(Expression<Func<DM_QuanHuyen, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_QuanHuyen.Where(query).ToList();
            }
        }
        // trinhpv optinForm
        public List<DMTinhThanhDTO> GetListTinhThanhOF(Expression<Func<DM_TinhThanh, bool>> query, string sub)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_TinhThanh.Select(prv => new DMTinhThanhDTO { ID = prv.ID, MaTinhThanh = prv.MaTinhThanh, TenTinhThanh = prv.TenTinhThanh }).ToList();
            }
        }

        public IQueryable<DM_QuanHuyen> GetListQuanHuyenOF(Expression<Func<DM_QuanHuyen, bool>> query, string sub)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_QuanHuyen.Where(query).Select(qh => new DM_QuanHuyenDTO { ID = qh.ID, MaQuanHuyen = qh.MaQuanHuyen, TenQuanHuyen = qh.TenQuanHuyen });
            }
        }
        public Guid? getID_NhanVienPhuTrach(string SubDomain, string MaNhanVien)
        {
            try
            {
                var tb = from nv in db.NS_NhanVien
                         where nv.MaNhanVien == MaNhanVien
                         select new
                         {
                             ID = nv.ID
                         };
                return tb.FirstOrDefault().ID;
            }
            catch
            {
                return null;
            }
        }
        public List<DM_QuanHuyen> Get_DMQuanHuyen(Expression<Func<DM_QuanHuyen, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                {
                    return db.DM_QuanHuyen.ToList();
                }
                else
                {
                    return db.DM_QuanHuyen.Where(query).ToList();
                }
            }
        }

        public List<DM_TinhThanh> Get_DMTinhThanh(Expression<Func<DM_TinhThanh, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                {
                    return db.DM_TinhThanh.ToList();
                }
                else
                {
                    return db.DM_TinhThanh.Where(query).ToList();
                }
            }
        }

        #endregion

        #region trinhpv Nâng nhóm đối tượng
        public string insert_DoiTuongNangNhom(Guid ID_NhomDoiTuong, int LoaiCapNhat)
        {
            string str = string.Empty;
            try
            {
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                List<DM_NhomDoiTuongChiTietDTO> lst = db.Database.SqlQuery<DM_NhomDoiTuongChiTietDTO>("exec getList_NhomDoiTuongChiTiet @ID_NhomDoiTuong", prm.ToArray()).ToList();
                string sqlHinhThuc1 = string.Empty;
                string sqlHinhThuc2 = string.Empty;
                string sqlHinhThuc3 = string.Empty;
                string sqlHinhThuc4 = string.Empty;
                string sqlHinhThuc5 = string.Empty;
                string sqlHinhThuc6 = string.Empty;
                string sqlHinhThuc7 = string.Empty;
                string sqlHinhThuc8 = string.Empty;
                string sqlHinhThuc9 = string.Empty;
                string sqlHinhThuc10 = string.Empty;
                foreach (var item in lst)
                {
                    switch (item.LoaiHinhThuc)
                    {
                        case 1:
                            switch (item.SoSanh)
                            {
                                case 1:
                                    if (sqlHinhThuc1 == string.Empty)
                                        sqlHinhThuc1 = " > " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc1 = sqlHinhThuc1 + " and DoanhThuThuan > " + item.GiaTri.ToString();
                                    break;
                                case 2:
                                    if (sqlHinhThuc1 == string.Empty)
                                        sqlHinhThuc1 = " >= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc1 = sqlHinhThuc1 + " and DoanhThuThuan >= " + item.GiaTri.ToString();
                                    break;
                                case 3:
                                    if (sqlHinhThuc1 == string.Empty)
                                        sqlHinhThuc1 = " = " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc1 = sqlHinhThuc1 + " and DoanhThuThuan = " + item.GiaTri.ToString();
                                    break;
                                case 4:
                                    if (sqlHinhThuc1 == string.Empty)
                                        sqlHinhThuc1 = " <= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc1 = sqlHinhThuc1 + " and DoanhThuThuan <= " + item.GiaTri.ToString();
                                    break;
                                case 5:
                                    if (sqlHinhThuc1 == string.Empty)
                                        sqlHinhThuc1 = " < " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc1 = sqlHinhThuc1 + " and DoanhThuThuan < " + item.GiaTri.ToString();
                                    break;
                            }
                            break;
                        case 2:
                            switch (item.SoSanh)
                            {
                                case 1:
                                    if (sqlHinhThuc2 == string.Empty)
                                        sqlHinhThuc2 = " > " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc2 = sqlHinhThuc2 + " and GiaTriBan > " + item.GiaTri.ToString();
                                    break;
                                case 2:
                                    if (sqlHinhThuc2 == string.Empty)
                                        sqlHinhThuc2 = " >= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc2 = sqlHinhThuc2 + " and GiaTriBan >= " + item.GiaTri.ToString();
                                    break;
                                case 3:
                                    if (sqlHinhThuc2 == string.Empty)
                                        sqlHinhThuc2 = " = " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc2 = sqlHinhThuc2 + " and GiaTriBan = " + item.GiaTri.ToString();
                                    break;
                                case 4:
                                    if (sqlHinhThuc2 == string.Empty)
                                        sqlHinhThuc2 = " <= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc2 = sqlHinhThuc2 + " and GiaTriBan <= " + item.GiaTri.ToString();
                                    break;
                                case 5:
                                    if (sqlHinhThuc2 == string.Empty)
                                        sqlHinhThuc2 = " < " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc2 = sqlHinhThuc2 + " and GiaTriBan < " + item.GiaTri.ToString();
                                    break;
                            }
                            break;
                        case 3:
                            var t = item.TimeBy.ToString().Split(' ');
                            var t1 = t[0].Split('/');
                            string timeby = string.Join("-", t1.Reverse());
                            switch (item.SoSanh)
                            {
                                case 1:
                                    if (sqlHinhThuc3 == string.Empty)
                                        sqlHinhThuc3 = " >= DATEADD(day, 1, '" + timeby + "')";
                                    else
                                        sqlHinhThuc3 = sqlHinhThuc3 + " and NgayLapHoaDon >= DATEADD(day, 1, '" + item.TimeBy + "')";
                                    break;
                                case 2:
                                    if (sqlHinhThuc3 == string.Empty)
                                        sqlHinhThuc3 = " >= '" + timeby + "'";
                                    else
                                        sqlHinhThuc3 = sqlHinhThuc3 + " and NgayLapHoaDon >= '" + timeby + "')";
                                    break;
                                case 3:
                                    if (sqlHinhThuc3 == string.Empty)
                                        sqlHinhThuc3 = " >= '" + timeby + "' and NgayLapHoaDon <= DATEADD(day, 1, '" + timeby + "')";
                                    else
                                        sqlHinhThuc3 = sqlHinhThuc3 + " and NgayLapHoaDon >= '" + timeby + "' and NgayLapHoaDon <= DATEADD(day, 1, '" + timeby + "')";
                                    break;
                                case 4:
                                    if (sqlHinhThuc3 == string.Empty)
                                        sqlHinhThuc3 = " <= DATEADD(day, 1, '" + timeby + "')";
                                    else
                                        sqlHinhThuc3 = sqlHinhThuc3 + " and NgayLapHoaDon <= DATEADD(day, 1, '" + timeby + "')";
                                    break;
                                case 5:
                                    if (sqlHinhThuc3 == string.Empty)
                                        sqlHinhThuc3 = " < '" + timeby + "'";
                                    else
                                        sqlHinhThuc3 = sqlHinhThuc3 + " and NgayLapHoaDon < '" + timeby + "'";
                                    break;
                            }
                            break;
                        case 4:
                            switch (item.SoSanh)
                            {
                                case 1:
                                    if (sqlHinhThuc4 == string.Empty)
                                        sqlHinhThuc4 = " > " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc4 = sqlHinhThuc4 + " and SoLanMuaHang > " + item.GiaTri.ToString();
                                    break;
                                case 2:
                                    if (sqlHinhThuc4 == string.Empty)
                                        sqlHinhThuc4 = " >= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc4 = sqlHinhThuc4 + " and SoLanMuaHang >= " + item.GiaTri.ToString();
                                    break;
                                case 3:
                                    if (sqlHinhThuc4 == string.Empty)
                                        sqlHinhThuc4 = " = " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc4 = sqlHinhThuc4 + " and SoLanMuaHang = " + item.GiaTri.ToString();
                                    break;
                                case 4:
                                    if (sqlHinhThuc4 == string.Empty)
                                        sqlHinhThuc4 = " <= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc4 = sqlHinhThuc4 + " and SoLanMuaHang <= " + item.GiaTri.ToString();
                                    break;
                                case 5:
                                    if (sqlHinhThuc4 == string.Empty)
                                        sqlHinhThuc4 = " < " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc4 = sqlHinhThuc4 + " and SoLanMuaHang < " + item.GiaTri.ToString();
                                    break;
                            }
                            break;
                        case 5:
                            switch (item.SoSanh)
                            {
                                case 1:
                                    if (sqlHinhThuc5 == string.Empty)
                                        sqlHinhThuc5 = " > " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc5 = sqlHinhThuc5 + " and NoCuoiKy > " + item.GiaTri.ToString();
                                    break;
                                case 2:
                                    if (sqlHinhThuc5 == string.Empty)
                                        sqlHinhThuc5 = " >= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc5 = sqlHinhThuc5 + " and NoCuoiKy >= " + item.GiaTri.ToString();
                                    break;
                                case 3:
                                    if (sqlHinhThuc5 == string.Empty)
                                        sqlHinhThuc5 = " = " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc5 = sqlHinhThuc5 + " and NoCuoiKy = " + item.GiaTri.ToString();
                                    break;
                                case 4:
                                    if (sqlHinhThuc5 == string.Empty)
                                        sqlHinhThuc5 = " <= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc5 = sqlHinhThuc5 + " and NoCuoiKy <= " + item.GiaTri.ToString();
                                    break;
                                case 5:
                                    if (sqlHinhThuc5 == string.Empty)
                                        sqlHinhThuc5 = " < " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc5 = sqlHinhThuc5 + " and NoCuoiKy < " + item.GiaTri.ToString();
                                    break;
                            }
                            break;
                        case 6:
                            switch (item.SoSanh)
                            {
                                case 1:
                                    if (sqlHinhThuc6 == string.Empty)
                                        sqlHinhThuc6 = " > " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc6 = sqlHinhThuc6 + " and ThangSinh > " + item.GiaTri.ToString();
                                    break;
                                case 2:
                                    if (sqlHinhThuc6 == string.Empty)
                                        sqlHinhThuc6 = " >= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc6 = sqlHinhThuc6 + " and ThangSinh >= " + item.GiaTri.ToString();
                                    break;
                                case 3:
                                    if (sqlHinhThuc6 == string.Empty)
                                        sqlHinhThuc6 = " = " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc6 = sqlHinhThuc6 + " and ThangSinh = " + item.GiaTri.ToString();
                                    break;
                                case 4:
                                    if (sqlHinhThuc6 == string.Empty)
                                        sqlHinhThuc6 = " <= " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc6 = sqlHinhThuc6 + " and ThangSinh <= " + item.GiaTri.ToString();
                                    break;
                                case 5:
                                    if (sqlHinhThuc6 == string.Empty)
                                        sqlHinhThuc6 = " < " + item.GiaTri.ToString();
                                    else
                                        sqlHinhThuc6 = sqlHinhThuc6 + " and ThangSinh < " + item.GiaTri.ToString();
                                    break;
                            }
                            break;
                        case 7:
                            string timeby1 = "'" + DateTime.Now.ToString("yyyy-MM-dd") + "')";

                            switch (item.SoSanh)
                            {
                                case 1:
                                    if (sqlHinhThuc7 == string.Empty)
                                        sqlHinhThuc7 = " < DATEADD(YEAR, -" + (item.GiaTri + 1).ToString() + ", " + timeby1;
                                    else
                                        sqlHinhThuc7 = sqlHinhThuc7 + " and NgaySinh_NgayTLap < DATEADD(YEAR, -" + (item.GiaTri + 1).ToString() + ", " + timeby1;
                                    break;
                                case 2:
                                    if (sqlHinhThuc7 == string.Empty)
                                        sqlHinhThuc7 = " <= DATEADD(YEAR, -" + item.GiaTri.ToString() + ", " + timeby1;
                                    else
                                        sqlHinhThuc7 = sqlHinhThuc7 + " and NgaySinh_NgayTLap <= DATEADD(YEAR, -" + item.GiaTri.ToString() + ", " + timeby1;
                                    break;
                                case 3:
                                    if (sqlHinhThuc7 == string.Empty)
                                        sqlHinhThuc7 = " <= DATEADD(YEAR, -" + item.GiaTri.ToString() + ", " + timeby1 + " and NgaySinh_NgayTLap >= DATEADD(YEAR, -" + (item.GiaTri + 1).ToString() + ", " + timeby1;
                                    else
                                        sqlHinhThuc7 = sqlHinhThuc7 + " and NgaySinh_NgayTLap <= DATEADD(YEAR, -" + item.GiaTri.ToString() + ", " + timeby1 + " and NgaySinh_NgayTLap >= DATEADD(YEAR, -" + (item.GiaTri + 1).ToString() + ", " + timeby1;
                                    break;
                                case 4:
                                    if (sqlHinhThuc7 == string.Empty)
                                        sqlHinhThuc7 = " >= DATEADD(YEAR, -" + (item.GiaTri + 1).ToString() + ", " + timeby1;
                                    else
                                        sqlHinhThuc7 = sqlHinhThuc7 + " and NgaySinh_NgayTLap >= DATEADD(YEAR, -" + (item.GiaTri + 1).ToString() + ", " + timeby1;
                                    break;
                                case 5:
                                    if (sqlHinhThuc7 == string.Empty)
                                        sqlHinhThuc7 = " > DATEADD(YEAR, -" + (item.GiaTri).ToString() + ", " + timeby1;
                                    else
                                        sqlHinhThuc7 = sqlHinhThuc7 + " and NgaySinh_NgayTLap > DATEADD(YEAR, -" + (item.GiaTri).ToString() + ", " + timeby1;
                                    break;
                            }
                            break;
                        case 8:
                            switch (item.GioiTinh)
                            {
                                case false:
                                    if (sqlHinhThuc8 == string.Empty)
                                        sqlHinhThuc8 = " = 0";
                                    else
                                        sqlHinhThuc8 = sqlHinhThuc8 + " and GioiTinhNam = 0";
                                    break;
                                case true:
                                    if (sqlHinhThuc8 == string.Empty)
                                        sqlHinhThuc8 = " = 1";
                                    else
                                        sqlHinhThuc8 = sqlHinhThuc8 + " and GioiTinhNam = 1";
                                    break;
                            }
                            break;
                        case 9:
                            switch (item.SoSanh)
                            {
                                case 3:
                                    if (sqlHinhThuc9 == string.Empty)
                                        sqlHinhThuc9 = " = '" + item.ID_KhuVuc.ToString() + "'";
                                    else
                                        sqlHinhThuc9 = sqlHinhThuc9 + " and ID_TinhThanh = '" + item.ID_KhuVuc.ToString() + "'";
                                    break;
                                case 6:
                                    if (sqlHinhThuc9 == string.Empty)
                                        sqlHinhThuc9 = " != '" + item.ID_KhuVuc.ToString() + "'";
                                    else
                                        sqlHinhThuc9 = sqlHinhThuc9 + " and ID_TinhThanh != '" + item.ID_KhuVuc.ToString() + "'";
                                    break;
                            }
                            break;
                        case 10:
                            switch (item.SoSanh)
                            {
                                case 3:
                                    if (sqlHinhThuc10 == string.Empty)
                                        sqlHinhThuc10 = " = '" + item.ID_VungMien.ToString() + "'";
                                    else
                                        sqlHinhThuc10 = sqlHinhThuc10 + " and ID_VungMien = '" + item.ID_VungMien.ToString() + "'";
                                    break;
                                case 6:
                                    if (sqlHinhThuc10 == string.Empty)
                                        sqlHinhThuc10 = " != '" + item.ID_VungMien.ToString() + "'";
                                    else
                                        sqlHinhThuc10 = sqlHinhThuc10 + " and ID_VungMien != '" + item.ID_VungMien.ToString() + "'";
                                    break;
                            }
                            break;
                    }
                }

                List<List<DM_DoiTuongNangNhomPRC>> data = new List<List<DM_DoiTuongNangNhomPRC>>();

                if (sqlHinhThuc1 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc1));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc1 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc2 != string.Empty)
                {
                    List<SqlParameter> prm2 = new List<SqlParameter>();
                    prm2.Add(new SqlParameter("SqlQuery", sqlHinhThuc2));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc2 @SqlQuery", prm2.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc3 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc3));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc3 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc4 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc4));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc4 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc5 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc5));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc5 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc6 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc6));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc6 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc7 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc7));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc7 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc8 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc8));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc8 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc9 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc9));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc9 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }
                if (sqlHinhThuc10 != string.Empty)
                {
                    List<SqlParameter> prm1 = new List<SqlParameter>();
                    prm1.Add(new SqlParameter("SqlQuery", sqlHinhThuc10));
                    List<DM_DoiTuongNangNhomPRC> lst1 = db.Database.SqlQuery<DM_DoiTuongNangNhomPRC>("exec getlist_DoiTuong_HinhThuc10 @SqlQuery", prm1.ToArray()).ToList();
                    if (lst1.Count() > 0)
                        data.Add(lst1);
                    else str = "Không tìm thấy khách hàng nào phù hợp";
                }

                var lstDT = new List<DM_DoiTuongNangNhomPRC>();
                if (str == string.Empty && data.Count() > 0)
                {
                    for (int k = 0; k < data.Count(); k++)
                    {
                        if (k == 0)
                        {
                            lstDT = data[k];
                        }
                        else
                        {
                            lstDT = (from a in data[k]
                                     join b in lstDT on a.ID_DoiTuong equals b.ID_DoiTuong
                                     select new DM_DoiTuongNangNhomPRC
                                     {
                                         ID_DoiTuong = a.ID_DoiTuong
                                     }).ToList();
                        }
                    }
                }
                else
                {
                    if (LoaiCapNhat == 1)
                    {
                        List<SqlParameter> prmdl = new List<SqlParameter>();
                        prmdl.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                        db.Database.ExecuteSqlCommand("exec delete_DoiTuong_Nhom @ID_NhomDoiTuong", prmdl.ToArray());
                    }
                }

                // delete Nhom old in DM_DoiTuong if not enough condition
                var idDoiTuong = string.Join(", ", lstDT.Select(x => x.ID_DoiTuong));
                if (idDoiTuong == string.Empty)
                {
                    idDoiTuong = "00000000-0000-0000-0000-000000000000";
                }
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_DoiTuongs", idDoiTuong));
                lstParam.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                db.Database.ExecuteSqlCommand("exec UpdateAgainNhomDT_InDMDoiTuong_AfterChangeDKNangNhom @ID_DoiTuongs, @ID_NhomDoiTuong", lstParam.ToArray());

                if (lstDT.Count() > 0)
                {
                    for (int i = 0; i < lstDT.Count(); i++)
                    {
                        List<SqlParameter> paramer = new List<SqlParameter>();
                        paramer.Add(new SqlParameter("LoaiCapNhat", LoaiCapNhat));
                        paramer.Add(new SqlParameter("DK_xoa", i + 1));
                        paramer.Add(new SqlParameter("ID_DoiTuong", lstDT[i].ID_DoiTuong));
                        paramer.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                        // insert DM_DoiTuong_nhom --> run trigger update IDNhoms, TenNhoms in DMDoiTuong
                        db.Database.ExecuteSqlCommand("exec insert_DoiTuong_Nhom @LoaiCapNhat, @DK_xoa, @ID_DoiTuong, @ID_NhomDoiTuong", paramer.ToArray());
                    }
                }
                else
                    //str = "Không tìm thấy khách hàng nào phù hợp";
                    str = string.Empty; // insert suscess
            }
            catch (Exception e)
            {
                str = e.InnerException + e.Message;
            }
            return str;
        }

        public void NangNhomKhachhang_byID(Guid? idDoiTuong, Guid? idChiNhanh)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_DoiTuong", idDoiTuong));
            sql.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
            db.Database.ExecuteSqlCommand(" exec NangNhom_KhachHangbyID @ID_DoiTuong, @ID_ChiNhanh", sql.ToArray());
        }

        public void UpdateKhachHang_DuDKNangNhom(Guid? idNhomDT, List<Guid> lstIDDonVi)
        {
            var isChiNhanhs = string.Join(",", lstIDDonVi);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_NhomDoiTuong", idNhomDT));
            sql.Add(new SqlParameter("IDChiNhanhs", isChiNhanhs));
            db.Database.ExecuteSqlCommand(" exec UpdateKhachHang_DuDKNangNhom @ID_NhomDoiTuong, @IDChiNhanhs", sql.ToArray());
        }
        public List<BH_HoaDonDTO> GetChiPhiDichVu_byVendor(CommonParamSearch param)
        {
            var isChiNhanhs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                isChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", isChiNhanhs ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("ID_NhaCungCap", param.TextSearch ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
            var data = db.Database.SqlQuery<BH_HoaDonDTO>(" exec GetChiPhiDichVu_byVendor @IDChiNhanhs, @ID_NhaCungCap, @DateFrom, @DateTo," +
                "@CurrentPage, @PageSize", sql.ToArray()).ToList();
            return data;
        }
        #endregion

        #region Bảo hiểm
        public List<GetListBaoHiem_v1> GetListBaoHiem_v1(ParamGetListBaoHiem_v1 param)
        {
            var idChiNhanh = string.Join(",", param.IdChiNhanhs);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
            sql.Add(new SqlParameter("NgayTaoFrom", param.NgayTaoFrom == null ? (object)DBNull.Value : param.NgayTaoFrom.Value));
            sql.Add(new SqlParameter("NgayTaoTo", param.NgayTaoTo == null ? (object)DBNull.Value : param.NgayTaoTo.Value));
            sql.Add(new SqlParameter("TongBanDateFrom", param.TongBanDateFrom == null ? (object)DBNull.Value : param.TongBanDateFrom.Value));
            sql.Add(new SqlParameter("TongBanDateTo", param.TongBanDateTo == null ? (object)DBNull.Value : param.TongBanDateTo.Value));
            sql.Add(new SqlParameter("TongBanFrom", param.TongBanFrom == null ? (object)DBNull.Value : param.TongBanFrom.Value));
            sql.Add(new SqlParameter("TongBanTo", param.TongBanTo == null ? (object)DBNull.Value : param.TongBanTo.Value));
            sql.Add(new SqlParameter("NoFrom", param.NoFrom == null ? (object)DBNull.Value : param.NoFrom.Value));
            sql.Add(new SqlParameter("NoTo", param.NoTo == null ? (object)DBNull.Value : param.NoTo.Value));
            sql.Add(new SqlParameter("TrangThais", param.TrangThais));
            sql.Add(new SqlParameter("TextSearch", param.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
            sql.Add(new SqlParameter("PageSize", param.PageSize));
            string sqlquery = "GetListBaoHiem_v1 @IdChiNhanhs, @NgayTaoFrom, @NgayTaoTo, " +
                "@TongBanDateFrom, @TongBanDateTo, @TongBanFrom, @TongBanTo, " +
                "@NoFrom, @NoTo," +
                "@TrangThais, @TextSearch, @CurrentPage, @PageSize";
            List<GetListBaoHiem_v1> xx = db.Database.SqlQuery<GetListBaoHiem_v1>(sqlquery, sql.ToArray()).ToList();
            return xx;
        }
        #endregion
    }

    public class DMTinhThanhDTO : DM_TinhThanh { };
    public class DM_QuanHuyenDTO : DM_QuanHuyen { };

    public class JqAuto_DMDoiTuong
    {
        public Guid ID { get; set; }
        public string MaNguoiNop { get; set; }
        public string NguoiNopTien { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string IDNhomDoiTuongs { get; set; }//used to check banggia by nhomkhach
    };

    public class DM_VungMienDTO
    {
        public Guid ID { get; set; }
        public string TenVung { get; set; }
    }
    public class DM_NhomDoiTuongDTO
    {
        public Guid ID { get; set; }
        public string TenNhomDoiTuong { get; set; }
        public double? GiamGia { get; set; }
        public bool? GiamGiaTheoPhanTram { get; set; }
        public string GhiChu { get; set; }
        public bool? TuDongCapNhat { get; set; }
        public List<NhomDoiTuong_DonViDTO> NhomDT_DonVi { get; set; }
    }
    public class DM_NhomDoiTuongChiTietDTO
    {
        public int? IDHT { get; set; }
        public int? IDDK { get; set; }
        public string HinhThuc { get; set; }
        public int? LoaiHinhThuc { get; set; }
        public string LoaiSoSanh { get; set; }
        public int? SoSanh { get; set; }
        public double? GiaTri { get; set; }
        public DateTime? TimeBy { get; set; }
        public bool? GioiTinh { get; set; }
        public double? ThangSinh { get; set; }
        public Guid? ID_KhuVuc { get; set; }
        public string KhuVuc { get; set; }
        public Guid? ID_VungMien { get; set; }
        public string VungMien { get; set; }
    }
    public class DM_DoiTuongNangNhomPRC
    {
        public Guid ID_DoiTuong { get; set; }
    }
    public class DM_NhomDoiTuong_ChiTietPRC
    {
        public int IDHT { get; set; }
        public int IDDK { get; set; }
        public string HinhThuc { get; set; }
        public int LoaiHinhThuc { get; set; }
        public string LoaiSoSanh { get; set; }
        public int SoSanh { get; set; }
        public double? GiaTri { get; set; }
        public DateTime? TimeBy { get; set; }
        public bool? GioiTinh { get; set; }
        public double? ThangSinh { get; set; }
        public Guid? ID_KhuVuc { get; set; }
        public string KhuVuc { get; set; }
        public Guid? ID_VungMien { get; set; }
        public string VungMien { get; set; }
    }
    public class DM_NhomDoiTuongPRC
    {
        public Guid? ID { get; set; }
        public string TenNhomDoiTuong { get; set; }
        public double? GiamGia { get; set; }
        public bool? GiamGiaTheoPhanTram { get; set; }
        public string GhiChu { get; set; }
    }

    public class SP_QuyHoaDonLienQuan
    {
        public Guid ID_HoaDonLienQuan { get; set; }
        public double? TongTienThu { get; set; }
    }

    public class SP_HoaDonDebit
    {
        public Guid? ID { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double PhaiThanhToan { get; set; }
        public double? TongThanhToan { get; set; }
        public int TinhChietKhauTheo { get; set; }
        public double? TongTienThue { get; set; }
        public int? LoaiHoaDon { get; set; }
    }

    public class ParamGetListBaoHiem_v1
    {
        public List<string> IdChiNhanhs { get; set; } = new List<string>();
        public DateTime? NgayTaoFrom { get; set; } = null;
        public DateTime? NgayTaoTo { get; set; } = null;
        public DateTime? TongBanDateFrom { get; set; } = null;
        public DateTime? TongBanDateTo { get; set; } = null;
        public double? TongBanFrom { get; set; } = null;
        public double? TongBanTo { get; set; } = null;
        public double? NoFrom { get; set; } = null;
        public double? NoTo { get; set; } = null;
        public string TrangThais { get; set; } = "";
        public string TextSearch { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetListBaoHiem_v1
    {
        public Guid ID { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string MaSoThue { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public Guid? ID_TinhThanh { get; set; }
        public string TenTinhThanh { get; set; }
        public Guid? ID_QuanHuyen { get; set; }
        public string TenQuanHuyen { get; set; }
        public string GhiChu { get; set; }
        public Guid ID_DonVi { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public DateTime NgayTao { get; set; }
        public int LoaiDoiTuong { get; set; }
        public string NguoiTao { get; set; }
        public double NoHienTai { get; set; }
        public double TongTienBaoHiem { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class GetListBaoHiem_v1_Export
    {
        public string MaBaoHiem { get; set; }
        public string TenbaoHiem { get; set; }
        public string MaSoThue { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string QuanHuyen { get; set; }
        public string TinhThanh { get; set; }
        public double NoHienTai { get; set; }
        public double TongChiPhiBaoHiem { get; set; }
        public string GhiChu { get; set; }
    }

    public class GetHoaDon_SoQuy_ofDoiTuong_Export
    {
        public string MaHoaDon { get; set; }
        public DateTime ThoiGian { get; set; }
        public string LoaiHoaDon { get; set; }
        public double GiaTriHoaDon { get; set; }
        public double DuNoLuyKe { get; set; }
    }
}

