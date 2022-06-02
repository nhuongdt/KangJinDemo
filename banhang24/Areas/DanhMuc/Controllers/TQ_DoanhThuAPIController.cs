using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using libQuy_HoaDon;
using System.Web;
using Model;
using System.Web.Http.Description;
using System.Data.SqlClient;
using banhang24.Models;
using Newtonsoft.Json.Linq;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class TQ_DoanhThuAPIController : BaseApiController
    {
        #region GET
        [HttpPost]
        public IHttpActionResult getListDoanhThuTT(CommonParamSearch param)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                ClassDoanhThu classDoanhThu = new ClassDoanhThu(db);
                List<BC_DoanhThu> lst = classDoanhThu.getDoanhThuTT(param);
                return ActionTrueData(lst);
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException + ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult getListDoanhThuSL(CommonParamSearch param)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                ClassDoanhThu classDoanhThu = new ClassDoanhThu(db);
                List<BC_DoanhThu> lst = classDoanhThu.getDoanhThuSL(param);
                return ActionTrueData(lst);
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException + ex.Message);
            }
        }

        public IHttpActionResult getDoanhThu_PieChiNhanh(DateTime dayStart, DateTime dayEnd, Guid ID_NguoiDung)
        {
            List<PieChart_ChiNhanh> lst = ClassDoanhThu.getDoanhThu_PieChiNhanh(dayStart, dayEnd, ID_NguoiDung);
            double ThanhTien = (double?)lst.Sum(x => x.vl) ?? 0;
            JsonResultExample_BieuDo<PieChart_ChiNhanh> jsonobj = new JsonResultExample_BieuDo<PieChart_ChiNhanh>
            {
                LstData = lst,
                TongTien = Math.Round(ThanhTien, 0, MidpointRounding.ToEven)
            };
            return Json(jsonobj);
        }
        public IHttpActionResult getDoanhThu_ColumnChiNhanh(DateTime dayStart, DateTime dayEnd, Guid ID_NguoiDung)
        {
            List<PieChart_ChiNhanh> lst = ClassDoanhThu.getDoanhThu_ColumnChiNhanh(dayStart, dayEnd, ID_NguoiDung);
            double ThanhTien = (double?)lst.Sum(x => x.y) ?? 0;
            JsonResultExample_BieuDo<PieChart_ChiNhanh> jsonobj = new JsonResultExample_BieuDo<PieChart_ChiNhanh>
            {
                LstData = lst,
                TongTien = Math.Round(ThanhTien, 0, MidpointRounding.ToEven)
            };
            return Json(jsonobj);
        }

        public IHttpActionResult getDoanhThuToday(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<BC_DoanhThu> lst = ClassDoanhThu.getDoanhThuToday(dayStart, dayEnd, IDchinhanh);
            List<BC_DoanhThu> lst_date = lst.GroupBy(x => x.NgayLapHoaDon).Select(t => new BC_DoanhThu
            {
                NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon
            }).OrderBy(x => int.Parse(x.NgayLapHoaDon)).ToList();
            List<BC_DoanhThu> lst_ChiNhanh = lst.GroupBy(x => x.TenChiNhanh).Select(t => new BC_DoanhThu
            {
                TenChiNhanh = t.FirstOrDefault().TenChiNhanh
            }).OrderBy(x => x.TenChiNhanh).ToList();
            JsonResultExample_BieuDo<BC_DoanhThu> jsonobj = new JsonResultExample_BieuDo<BC_DoanhThu>
            {
                LstData = lst,
                LstDate = lst_date,
                LstChiNhanh = lst_ChiNhanh
            };
            return Json(jsonobj);
        }
        public string getNameNhanVien(Guid ID_NhanVien)
        {
            string lst = ClassDoanhThu.getList_NhanVien(ID_NhanVien);
            return lst;
        }
        public IHttpActionResult getList_SoSanhCungKyHoaDon(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<SoSanhCungKyPRC> lst = new List<SoSanhCungKyPRC>();
            try
            {
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID_ChiNhanh", IDchinhanh));
                sqlPRM.Add(new SqlParameter("timeStart", dayStart));
                sqlPRM.Add(new SqlParameter("timeEnd", dayEnd));
                lst = db.Database.SqlQuery<SoSanhCungKyPRC>("exec getList_SoSanhCungKyHoaDon @ID_ChiNhanh, @timeStart, @timeEnd", sqlPRM.ToArray()).ToList();
            }
            catch
            {
                SoSanhCungKyPRC DM = new SoSanhCungKyPRC();
                DM.SoSanhCungKy = 100;
                lst.Add(DM);
            }
            JsonResultExample_BieuDo<SoSanhCungKyPRC> jsonobj = new JsonResultExample_BieuDo<SoSanhCungKyPRC>
            {
                LstData = lst,
            };
            return Json(jsonobj);
        }
        public IHttpActionResult getList_SoSanhCungKyKhachHang(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<SoSanhCungKyPRC> lst = new List<SoSanhCungKyPRC>();
            try
            {
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID_ChiNhanh", IDchinhanh));
                sqlPRM.Add(new SqlParameter("timeStart", dayStart));
                sqlPRM.Add(new SqlParameter("timeEnd", dayEnd));
                lst = db.Database.SqlQuery<SoSanhCungKyPRC>("exec getList_SoSanhCungKyKhachHang @ID_ChiNhanh, @timeStart, @timeEnd", sqlPRM.ToArray()).ToList();
            }
            catch
            {
                SoSanhCungKyPRC DM = new SoSanhCungKyPRC();
                DM.SoSanhCungKy = 100;
                lst.Add(DM);
            }
            JsonResultExample_BieuDo<SoSanhCungKyPRC> jsonobj = new JsonResultExample_BieuDo<SoSanhCungKyPRC>
            {
                LstData = lst,
            };
            return Json(jsonobj);
        }
        public IHttpActionResult getList_SoSanhCungKyThuChi(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<SoSanhCungKyThuChiPRC> lst = new List<SoSanhCungKyThuChiPRC>();
            try
            {
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID_ChiNhanh", IDchinhanh));
                sqlPRM.Add(new SqlParameter("timeStart", dayStart));
                sqlPRM.Add(new SqlParameter("timeEnd", dayEnd));
                lst = db.Database.SqlQuery<SoSanhCungKyThuChiPRC>("exec getList_SoSanhCungKyThuChi @ID_ChiNhanh, @timeStart, @timeEnd", sqlPRM.ToArray()).ToList();
            }
            catch
            {
                SoSanhCungKyThuChiPRC DM = new SoSanhCungKyThuChiPRC();
                DM.ThuCungKy = 100;
                DM.ChiCungKy = 100;
                lst.Add(DM);
            }
            JsonResultExample_BieuDo<SoSanhCungKyThuChiPRC> jsonobj = new JsonResultExample_BieuDo<SoSanhCungKyThuChiPRC>
            {
                LstData = lst,
            };
            return Json(jsonobj);
        }
        public IHttpActionResult getList_TongQuanKhachHang(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<TongQuanKhachHangPRC> lst = new List<TongQuanKhachHangPRC>();
            List<SqlParameter> sqlPRM = new List<SqlParameter>();
            sqlPRM.Add(new SqlParameter("ID_ChiNhanh", IDchinhanh));
            sqlPRM.Add(new SqlParameter("timeStart", dayStart));
            sqlPRM.Add(new SqlParameter("timeEnd", dayEnd));
            lst = db.Database.SqlQuery<TongQuanKhachHangPRC>("exec getList_TongQuanKhachHang @ID_ChiNhanh, @timeStart, @timeEnd", sqlPRM.ToArray()).ToList();
            JsonResultExample_BieuDo<TongQuanKhachHangPRC> jsonobj = new JsonResultExample_BieuDo<TongQuanKhachHangPRC>
            {
                LstData = lst,
            };
            return Json(jsonobj);
        }
        public IHttpActionResult getList_TongQuanThuChi(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<TongQuanThuChiPRC> lst = new List<TongQuanThuChiPRC>();
            List<SqlParameter> sqlPRM = new List<SqlParameter>();
            sqlPRM.Add(new SqlParameter("ID_ChiNhanh", IDchinhanh));
            sqlPRM.Add(new SqlParameter("timeStart", dayStart));
            sqlPRM.Add(new SqlParameter("timeEnd", dayEnd));
            lst = db.Database.SqlQuery<TongQuanThuChiPRC>("exec getList_TongQuanThuChi @ID_ChiNhanh, @timeStart, @timeEnd", sqlPRM.ToArray()).ToList();
            JsonResultExample_BieuDo<TongQuanThuChiPRC> jsonobj = new JsonResultExample_BieuDo<TongQuanThuChiPRC>
            {
                LstData = lst,
            };
            return Json(jsonobj);
        }
        public IHttpActionResult getDoanhThuTodaybyChiNhanh(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<BC_DoanhThu> lst = ClassDoanhThu.getDoanhThuToDaybyChiNhanh(dayStart, dayEnd, IDchinhanh);
            List<BC_DoanhThu> lst_date = lst.GroupBy(x => x.NgayLapHoaDon).Select(t => new BC_DoanhThu
            {
                NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon
            }).OrderBy(x => int.Parse(x.NgayLapHoaDon)).ToList();
            List<BC_DoanhThu> lst_ChiNhanh = lst.GroupBy(x => x.TenChiNhanh).Select(t => new BC_DoanhThu
            {
                TenChiNhanh = t.FirstOrDefault().TenChiNhanh
            }).OrderBy(x => x.TenChiNhanh).ToList();
            JsonResultExample_BieuDo<BC_DoanhThu> jsonobj = new JsonResultExample_BieuDo<BC_DoanhThu>
            {
                LstData = lst,
                LstDate = lst_date,
                LstChiNhanh = lst_ChiNhanh
            };
            return Json(jsonobj);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult TongQuan_DoanhThuToDay(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<TongQuan_DoanhThuPROC> lst = new List<TongQuan_DoanhThuPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", dayStart));
            sql.Add(new SqlParameter("timeEnd", dayEnd));
            sql.Add(new SqlParameter("ID_ChiNhanh", IDchinhanh));
            lst = db.Database.SqlQuery<TongQuan_DoanhThuPROC>("exec BaoCaoTongQuan_DoanhThuToDay @timeStart, @timeEnd, @ID_ChiNhanh", sql.ToArray()).ToList();
            JsonResultExample<TongQuan_DoanhThuPROC> jsonobj = new JsonResultExample<TongQuan_DoanhThuPROC>
            {
                LstData = lst,
            };
            return Json(jsonobj);
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult TongQuanDoanhThuCongNo([FromBody] JObject objIn)
        {
            try
            {
                string idChiNhanh = "";
                if (objIn["IdChiNhanhs"] != null)
                    idChiNhanh = string.Join(",", objIn["IdChiNhanhs"].ToObject<List<string>>());
                DateTime DateFrom = DateTime.Now;
                DateTime DateTo = DateTime.Now;
                if (objIn["DateFrom"] != null)
                    DateFrom = objIn["DateFrom"].ToObject<DateTime>();
                if (objIn["DateTo"] != null)
                    DateTo = objIn["DateTo"].ToObject<DateTime>();
                int LoaiThoiGian = 1;
                if (objIn["LoaiThoiGian"] != null)
                    LoaiThoiGian = objIn["LoaiThoiGian"].ToObject<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    TongQuanDoanhThuCongNo tongquan = new TongQuanDoanhThuCongNo();
                    List<SqlParameter> sql = new List<SqlParameter>();
                    sql.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                    sql.Add(new SqlParameter("DateFrom", DateFrom));
                    sql.Add(new SqlParameter("DateTo", DateTo));
                    tongquan = db.Database.SqlQuery<TongQuanDoanhThuCongNo>("exec TongQuanDoanhThuCongNo @IdChiNhanhs, @DateFrom, @DateTo", sql.ToArray()).ToList()[0];
                    TongQuanDoanhThuCongNo tongquanSoSanh = null;
                    List<SqlParameter> sqlSoSanh = new List<SqlParameter>();
                    switch (LoaiThoiGian)
                    {
                        case 1:
                            sqlSoSanh.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                            sqlSoSanh.Add(new SqlParameter("DateFrom", DateFrom.AddDays(-1)));
                            sqlSoSanh.Add(new SqlParameter("DateTo", DateFrom));
                            tongquanSoSanh = db.Database.SqlQuery<TongQuanDoanhThuCongNo>("exec TongQuanDoanhThuCongNo @IdChiNhanhs, @DateFrom, @DateTo", sqlSoSanh.ToArray()).ToList()[0];
                            break;
                        case 3:
                            sqlSoSanh.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                            sqlSoSanh.Add(new SqlParameter("DateFrom", DateFrom.AddDays(-7)));
                            sqlSoSanh.Add(new SqlParameter("DateTo", DateFrom));
                            tongquanSoSanh = db.Database.SqlQuery<TongQuanDoanhThuCongNo>("exec TongQuanDoanhThuCongNo @IdChiNhanhs, @DateFrom, @DateTo", sqlSoSanh.ToArray()).ToList()[0];
                            break;
                        case 5:
                            sqlSoSanh.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                            sqlSoSanh.Add(new SqlParameter("DateFrom", DateFrom.AddMonths(-1)));
                            sqlSoSanh.Add(new SqlParameter("DateTo", DateFrom));
                            tongquanSoSanh = db.Database.SqlQuery<TongQuanDoanhThuCongNo>("exec TongQuanDoanhThuCongNo @IdChiNhanhs, @DateFrom, @DateTo", sqlSoSanh.ToArray()).ToList()[0];
                            break;
                        case 7:
                            sqlSoSanh.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                            sqlSoSanh.Add(new SqlParameter("DateFrom", DateFrom.AddDays(-3)));
                            sqlSoSanh.Add(new SqlParameter("DateTo", DateFrom));
                            tongquanSoSanh = db.Database.SqlQuery<TongQuanDoanhThuCongNo>("exec TongQuanDoanhThuCongNo @IdChiNhanhs, @DateFrom, @DateTo", sqlSoSanh.ToArray()).ToList()[0];
                            break;
                        case 9:
                            sqlSoSanh.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                            sqlSoSanh.Add(new SqlParameter("DateFrom", DateFrom.AddYears(-1)));
                            sqlSoSanh.Add(new SqlParameter("DateTo", DateFrom));
                            tongquanSoSanh = db.Database.SqlQuery<TongQuanDoanhThuCongNo>("exec TongQuanDoanhThuCongNo @IdChiNhanhs, @DateFrom, @DateTo", sqlSoSanh.ToArray()).ToList()[0];
                            break;
                        default:
                            break;
                    }
                    double DoanhThuPT = 0, DoanhThuSuaChuaPT = 0, DoanhThuBanHangPT = 0, CongNoPT = 0, CongNoPhaiThuPT = 0,
                        CongNoPhaiTraPT = 0, TienThuPT = 0, TienThuTienMatPT = 0, TienThuNganHangPT = 0,
                        TienChiPT = 0, TienChiTienMatPT = 0, TienChiNganHangPT = 0;
                    if (tongquanSoSanh != null)
                    {
                        //TongDoanhThu
                        if (tongquan.TongDoanhThu == 0 && tongquanSoSanh.TongDoanhThu == 0)
                        {
                            DoanhThuPT = 0;
                        }
                        else if (tongquan.TongDoanhThu == 0 && tongquanSoSanh.TongDoanhThu != 0)
                        {
                            DoanhThuPT = -100;
                        }
                        else if (tongquan.TongDoanhThu != 0 && tongquanSoSanh.TongDoanhThu == 0)
                        {
                            DoanhThuPT = 100;
                        }
                        else
                        {
                            DoanhThuPT = 100 * (tongquan.TongDoanhThu - tongquanSoSanh.TongDoanhThu) / tongquanSoSanh.TongDoanhThu;
                        }
                        //DoanhThuSuaChua
                        if (tongquan.DoanhThuSuaChua == 0 && tongquanSoSanh.DoanhThuSuaChua == 0)
                        {
                            DoanhThuSuaChuaPT = 0;
                        }
                        else if (tongquan.DoanhThuSuaChua == 0 && tongquanSoSanh.DoanhThuSuaChua != 0)
                        {
                            DoanhThuSuaChuaPT = -100;
                        }
                        else if (tongquan.DoanhThuSuaChua != 0 && tongquanSoSanh.DoanhThuSuaChua == 0)
                        {
                            DoanhThuSuaChuaPT = 100;
                        }
                        else
                        {
                            DoanhThuSuaChuaPT = 100 * (tongquan.DoanhThuSuaChua - tongquanSoSanh.DoanhThuSuaChua) / tongquanSoSanh.DoanhThuSuaChua;
                        }
                        //DoanhThuBanHang
                        if (tongquan.DoanhThuBanHang == 0 && tongquanSoSanh.DoanhThuBanHang == 0)
                        {
                            DoanhThuBanHangPT = 0;
                        }
                        else if (tongquan.DoanhThuBanHang == 0 && tongquanSoSanh.DoanhThuBanHang != 0)
                        {
                            DoanhThuBanHangPT = -100;
                        }
                        else if (tongquan.DoanhThuBanHang != 0 && tongquanSoSanh.DoanhThuBanHang == 0)
                        {
                            DoanhThuBanHangPT = 100;
                        }
                        else
                        {
                            DoanhThuBanHangPT = 100 * (tongquan.DoanhThuBanHang - tongquanSoSanh.DoanhThuBanHang) / tongquanSoSanh.DoanhThuBanHang;
                        }
                        //TongCongNo
                        if (tongquan.TongCongNo == 0 && tongquanSoSanh.TongCongNo == 0)
                        {
                            CongNoPT = 0;
                        }
                        else if (tongquan.TongCongNo == 0 && tongquanSoSanh.TongCongNo != 0)
                        {
                            CongNoPT = -100;
                        }
                        else if (tongquan.TongCongNo != 0 && tongquanSoSanh.TongCongNo == 0)
                        {
                            CongNoPT = 100;
                        }
                        else
                        {
                            CongNoPT = 100 * (tongquan.TongCongNo - tongquanSoSanh.TongCongNo) / tongquanSoSanh.TongCongNo;
                        }
                        //CongNoPhaiThu
                        if (tongquan.CongNoPhaiThu == 0 && tongquanSoSanh.CongNoPhaiThu == 0)
                        {
                            CongNoPhaiThuPT = 0;
                        }
                        else if (tongquan.CongNoPhaiThu == 0 && tongquanSoSanh.CongNoPhaiThu != 0)
                        {
                            CongNoPhaiThuPT = -100;
                        }
                        else if (tongquan.CongNoPhaiThu != 0 && tongquanSoSanh.CongNoPhaiThu == 0)
                        {
                            CongNoPhaiThuPT = 100;
                        }
                        else
                        {
                            CongNoPhaiThuPT = 100 * (tongquan.CongNoPhaiThu - tongquanSoSanh.CongNoPhaiThu) / tongquanSoSanh.CongNoPhaiThu;
                        }
                        //CongNoPhaiTra
                        if (tongquan.CongNoPhaiTra == 0 && tongquanSoSanh.CongNoPhaiTra == 0)
                        {
                            CongNoPhaiTraPT = 0;
                        }
                        else if (tongquan.CongNoPhaiTra == 0 && tongquanSoSanh.CongNoPhaiTra != 0)
                        {
                            CongNoPhaiTraPT = -100;
                        }
                        else if (tongquan.CongNoPhaiTra != 0 && tongquanSoSanh.CongNoPhaiTra == 0)
                        {
                            CongNoPhaiTraPT = 100;
                        }
                        else
                        {
                            CongNoPhaiTraPT = 100 * (tongquan.CongNoPhaiTra - tongquanSoSanh.CongNoPhaiTra) / tongquanSoSanh.CongNoPhaiTra;
                        }
                        //TongTienThu
                        if (tongquan.TongTienThu == 0 && tongquanSoSanh.TongTienThu == 0)
                        {
                            TienThuPT = 0;
                        }
                        else if (tongquan.TongTienThu == 0 && tongquanSoSanh.TongTienThu != 0)
                        {
                            TienThuPT = -100;
                        }
                        else if (tongquan.TongTienThu != 0 && tongquanSoSanh.TongTienThu == 0)
                        {
                            TienThuPT = 100;
                        }
                        else
                        {
                            TienThuPT = 100 * (tongquan.TongTienThu - tongquanSoSanh.TongTienThu) / tongquanSoSanh.TongTienThu;
                        }
                        //TienThuTienMat
                        if (tongquan.ThuTienMat == 0 && tongquanSoSanh.ThuTienMat == 0)
                        {
                            TienThuTienMatPT = 0;
                        }
                        else if (tongquan.ThuTienMat == 0 && tongquanSoSanh.ThuTienMat != 0)
                        {
                            TienThuTienMatPT = -100;
                        }
                        else if (tongquan.ThuTienMat != 0 && tongquanSoSanh.ThuTienMat == 0)
                        {
                            TienThuTienMatPT = 100;
                        }
                        else
                        {
                            TienThuTienMatPT = 100 * (tongquan.ThuTienMat - tongquanSoSanh.ThuTienMat) / tongquanSoSanh.ThuTienMat;
                        }
                        //TienThuNganHang
                        if (tongquan.ThuNganHang == 0 && tongquanSoSanh.ThuNganHang == 0)
                        {
                            TienThuNganHangPT = 0;
                        }
                        else if (tongquan.ThuNganHang == 0 && tongquanSoSanh.ThuNganHang != 0)
                        {
                            TienThuNganHangPT = -100;
                        }
                        else if (tongquan.ThuNganHang != 0 && tongquanSoSanh.ThuNganHang == 0)
                        {
                            TienThuNganHangPT = 100;
                        }
                        else
                        {
                            TienThuNganHangPT = 100 * (tongquan.ThuNganHang - tongquanSoSanh.ThuNganHang) / tongquanSoSanh.ThuNganHang;
                        }
                        //TongTienChi
                        if (tongquan.TongTienChi == 0 && tongquanSoSanh.TongTienChi == 0)
                        {
                            TienChiPT = 0;
                        }
                        else if (tongquan.TongTienChi == 0 && tongquanSoSanh.TongTienChi != 0)
                        {
                            TienChiPT = -100;
                        }
                        else if (tongquan.TongTienChi != 0 && tongquanSoSanh.TongTienChi == 0)
                        {
                            TienChiPT = 100;
                        }
                        else
                        {
                            TienChiPT = 100 * (tongquan.TongTienChi - tongquanSoSanh.TongTienChi) / tongquanSoSanh.TongTienChi;
                        }
                        //TienChiTienMat
                        if (tongquan.ChiTienMat == 0 && tongquanSoSanh.ChiTienMat == 0)
                        {
                            TienChiTienMatPT = 0;
                        }
                        else if (tongquan.ChiTienMat == 0 && tongquanSoSanh.ChiTienMat != 0)
                        {
                            TienChiTienMatPT = -100;
                        }
                        else if (tongquan.ChiTienMat != 0 && tongquanSoSanh.ChiTienMat == 0)
                        {
                            TienChiTienMatPT = 100;
                        }
                        else
                        {
                            TienChiTienMatPT = 100 * (tongquan.ChiTienMat - tongquanSoSanh.ChiTienMat) / tongquanSoSanh.ChiTienMat;
                        }
                        //TienChiNganHang
                        if (tongquan.ChiNganHang == 0 && tongquanSoSanh.ChiNganHang == 0)
                        {
                            TienChiNganHangPT = 0;
                        }
                        else if (tongquan.ChiNganHang == 0 && tongquanSoSanh.ChiNganHang != 0)
                        {
                            TienChiNganHangPT = -100;
                        }
                        else if (tongquan.ChiNganHang != 0 && tongquanSoSanh.ChiNganHang == 0)
                        {
                            TienChiNganHangPT = 100;
                        }
                        else
                        {
                            TienChiNganHangPT = 100 * (tongquan.ChiNganHang - tongquanSoSanh.ChiNganHang) / tongquanSoSanh.ChiNganHang;
                        }
                    }
                    return ActionTrueData(new
                    {
                        DoanhThu = new
                        {
                            TongDoanhThu = Math.Round(tongquan.TongDoanhThu, 0),
                            PhanTram = Math.Round(DoanhThuPT, 0),
                            SuaChua = new
                            {
                                DoanhThu = Math.Round(tongquan.DoanhThuSuaChua, 0),
                                PhanTram = Math.Round(DoanhThuSuaChuaPT, 0)
                            },
                            BanHang = new
                            {
                                DoanhThu = Math.Round(tongquan.DoanhThuBanHang, 0),
                                PhanTram = Math.Round(DoanhThuBanHangPT, 0)
                            }
                        },
                        CongNo = new
                        {
                            TongCongNo = Math.Round(tongquan.TongCongNo, 0),
                            PhanTram = Math.Round(CongNoPT, 0),
                            PhaiThu = new
                            {
                                CongNo = Math.Round(tongquan.CongNoPhaiThu, 0),
                                PhanTram = Math.Round(CongNoPhaiThuPT, 0)
                            },
                            PhaiTra = new
                            {
                                CongNo = Math.Round(tongquan.CongNoPhaiTra, 0),
                                PhanTram = Math.Round(CongNoPhaiTraPT, 0)
                            }
                        },
                        TienThu = new
                        {
                            TongTienThu = Math.Round(tongquan.TongTienThu, 0),
                            PhanTram = Math.Round(TienThuPT, 0),
                            TienMat = new
                            {
                                TienThu = Math.Round(tongquan.ThuTienMat, 0),
                                PhanTram = Math.Round(TienThuTienMatPT, 0)
                            },
                            NganHang = new
                            {
                                TienThu = Math.Round(tongquan.ThuNganHang, 0),
                                PhanTram = Math.Round(TienThuNganHangPT, 0)
                            }
                        },
                        TienChi = new
                        {
                            TongTienChi = Math.Round(tongquan.TongTienChi, 0),
                            PhanTram = Math.Round(TienChiPT, 0),
                            TienMat = new
                            {
                                TienChi = Math.Round(tongquan.ChiTienMat, 0),
                                PhanTram = Math.Round(TienChiTienMatPT, 0)
                            },
                            NganHang = new
                            {
                                TienChi = Math.Round(tongquan.ChiNganHang, 0),
                                PhanTram = Math.Round(TienChiNganHangPT, 0)
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        public IHttpActionResult getDoanhThuToHour(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<BC_DoanhThu> lst = ClassDoanhThu.getDoanhThuToHour(dayStart, dayEnd, IDchinhanh);
            List<BC_DoanhThu> lst_date = lst.GroupBy(x => x.NgayLapHoaDon).Select(t => new BC_DoanhThu
            {
                NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon
            }).OrderBy(x => int.Parse(x.NgayLapHoaDon)).ToList();
            List<BC_DoanhThu> lst_ChiNhanh = lst.GroupBy(x => x.TenChiNhanh).Select(t => new BC_DoanhThu
            {
                TenChiNhanh = t.FirstOrDefault().TenChiNhanh
            }).OrderBy(x => x.TenChiNhanh).ToList();
            JsonResultExample_BieuDo<BC_DoanhThu> jsonobj = new JsonResultExample_BieuDo<BC_DoanhThu>
            {
                LstData = lst,
                LstDate = lst_date,
                LstChiNhanh = lst_ChiNhanh
            };
            return Json(jsonobj);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult TongQuan_BieuDoDoanhThuToDay(array_TongQuan array_TongQuan)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<BieuDo_DoanhThuPROC> lst = new List<BieuDo_DoanhThuPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", array_TongQuan.dayStart));
            sql.Add(new SqlParameter("timeEnd", array_TongQuan.dayEnd));
            sql.Add(new SqlParameter("ID_NguoiDung", array_TongQuan.ID_NguoiDung));
            sql.Add(new SqlParameter("ID_DonVi", array_TongQuan.ID_DonVi));
            lst = db.Database.SqlQuery<BieuDo_DoanhThuPROC>("exec BaoCaoTongQuan_BieuDoDoanhThuToDay @timeStart, @timeEnd, @ID_NguoiDung, @ID_DonVi", sql.ToArray()).ToList();
            List<BieuDo_DoanhThuPROC> lst_date = lst.GroupBy(x => x.NgayLapHoaDon).Select(t => new BieuDo_DoanhThuPROC
            {
                NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon
            }).OrderBy(x => x.NgayLapHoaDon).ToList();
            List<BieuDo_DoanhThuPROC> lst_ChiNhanh = lst.GroupBy(x => x.TenChiNhanh).Select(t => new BieuDo_DoanhThuPROC
            {
                TenChiNhanh = t.FirstOrDefault().TenChiNhanh,
                ThanhTien = t.Sum(x => x.ThanhTien)
            }).OrderBy(x => x.TenChiNhanh).ToList();
            JsonResultExample_BieuDo<BieuDo_DoanhThuPROC> jsonobj = new JsonResultExample_BieuDo<BieuDo_DoanhThuPROC>
            {
                LstData = lst,
                LstDate = lst_date,
                LstChiNhanh = lst_ChiNhanh
            };
            return Json(jsonobj);
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetBieuDoDoanhThu([FromBody] JObject objIn)
        {
            try
            {
                List<Guid> ListChiNhanh = new List<Guid>();
                string idChiNhanh = "";
                if (objIn["IdChiNhanhs"] != null)
                {
                    ListChiNhanh = objIn["IdChiNhanhs"].ToObject<List<Guid>>();
                    idChiNhanh = string.Join(",", objIn["IdChiNhanhs"].ToObject<List<string>>());
                }
                int LoaiBieuDo = 1;
                if (objIn["LoaiBieuDo"] != null)
                    LoaiBieuDo = objIn["LoaiBieuDo"].ToObject<int>();
                int Thang = 0;
                if (objIn["Thang"] != null)
                    Thang = objIn["Thang"].ToObject<int>();
                string Nam = "";
                if (objIn["Nam"] != null)
                    Nam = string.Join(",", objIn["Nam"].ToObject<List<int>>());
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("LoaiBieuDo", LoaiBieuDo));
                sql.Add(new SqlParameter("Thang", Thang));
                sql.Add(new SqlParameter("Nam", Nam));
                sql.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                List<TongQuanBieuDoDoanhThuThuan> lst = new List<TongQuanBieuDoDoanhThuThuan>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    lst = db.Database.SqlQuery<TongQuanBieuDoDoanhThuThuan>("exec TongQuanBieuDoDoanhThuThuan @LoaiBieuDo, @Thang, @Nam, @IdChiNhanhs", sql.ToArray()).ToList();
                }
                List<int> lstx = lst.Select(p => p.ThoiGian).Distinct().OrderBy(p => p).ToList();
                List<objBieuDoDoanhTu> lstdataDoanhThu = new List<objBieuDoDoanhTu>();
                foreach (Guid chinhanh in ListChiNhanh)
                {
                    objBieuDoDoanhTu objBieuDo = new objBieuDoDoanhTu();
                    objBieuDo.ID_DonVi = chinhanh;
                    List<TongQuanBieuDoDoanhThuThuan> lstByChiNhanh = lst.Where(p => p.ID_DonVi == chinhanh).ToList();
                    List<BieuDo> lstBieuDo = new List<BieuDo>();
                    foreach (int thoigian in lstx)
                    {
                        BieuDo bieuDo = new BieuDo();
                        bieuDo.x = thoigian;
                        TongQuanBieuDoDoanhThuThuan tongquan = lstByChiNhanh.Where(p => p.ThoiGian == thoigian).FirstOrDefault();
                        if (tongquan == null)
                        {
                            bieuDo.y = 0;
                        }
                        else
                        {
                            bieuDo.y = tongquan.DoanhThuThuan;
                        }
                        lstBieuDo.Add(bieuDo);
                    }
                    objBieuDo.dataBieuDo = lstBieuDo;
                    lstdataDoanhThu.Add(objBieuDo);
                }
                List<BieuDo> loinhuan = lst.GroupBy(p => p.ThoiGian).Select(p => new BieuDo
                {
                    x = p.Key,
                    y = p.Sum(c => c.LoiNhuan)
                }).OrderBy(p => p.x).ToList();
                return ActionTrueData(new
                {
                    DoanhThu = lstdataDoanhThu,
                    LoiNhuan = loinhuan,
                    Label = lstx
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetBieuDoThucThu([FromBody] JObject objIn)
        {
            try
            {
                List<Guid> ListChiNhanh = new List<Guid>();
                string idChiNhanh = "";
                if (objIn["IdChiNhanhs"] != null)
                {
                    ListChiNhanh = objIn["IdChiNhanhs"].ToObject<List<Guid>>();
                    idChiNhanh = string.Join(",", objIn["IdChiNhanhs"].ToObject<List<string>>());
                }
                int LoaiBieuDo = 1;
                if (objIn["LoaiBieuDo"] != null)
                    LoaiBieuDo = objIn["LoaiBieuDo"].ToObject<int>();
                int Thang = 0;
                if (objIn["Thang"] != null)
                    Thang = objIn["Thang"].ToObject<int>();
                string Nam = "";
                if (objIn["Nam"] != null)
                    Nam = string.Join(",", objIn["Nam"].ToObject<List<int>>());
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("LoaiBieuDo", LoaiBieuDo));
                sql.Add(new SqlParameter("Thang", Thang));
                sql.Add(new SqlParameter("Nam", Nam));
                sql.Add(new SqlParameter("IdChiNhanhs", idChiNhanh));
                List<TongQuanBieuDoThucThu> lst = new List<TongQuanBieuDoThucThu>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    lst = db.Database.SqlQuery<TongQuanBieuDoThucThu>("exec TongQuanBieuDoThucThu @LoaiBieuDo, @Thang, @Nam, @IdChiNhanhs", sql.ToArray()).ToList();
                }
                List<int> lstx = lst.Select(p => p.ThoiGian).Distinct().OrderBy(p => p).ToList();
                List<objBieuDoDoanhTu> lstDataThucThu = new List<objBieuDoDoanhTu>();
                foreach (Guid chinhanh in ListChiNhanh)
                {
                    objBieuDoDoanhTu objBieuDo = new objBieuDoDoanhTu();
                    objBieuDo.ID_DonVi = chinhanh;
                    List<TongQuanBieuDoThucThu> lstByChiNhanh = lst.Where(p => p.ID_DonVi == chinhanh).ToList();
                    List<BieuDo> lstBieuDo = new List<BieuDo>();
                    foreach (int thoigian in lstx)
                    {
                        BieuDo bieuDo = new BieuDo();
                        bieuDo.x = thoigian;
                        TongQuanBieuDoThucThu tongquan = lstByChiNhanh.Where(p => p.ThoiGian == thoigian).FirstOrDefault();
                        if (tongquan == null)
                        {
                            bieuDo.y = 0;
                        }
                        else
                        {
                            bieuDo.y = tongquan.ThucThu;
                        }
                        lstBieuDo.Add(bieuDo);
                    }
                    objBieuDo.dataBieuDo = lstBieuDo;
                    lstDataThucThu.Add(objBieuDo);
                }
                return ActionTrueData(new
                {
                    ThucThu = lstDataThucThu,
                    Label = lstx
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult TongQuan_BieuDoDoanhThuToHour(array_TongQuan array_TongQuan)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<BieuDo_DoanhThuPROC> lst = new List<BieuDo_DoanhThuPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", array_TongQuan.dayStart));
            sql.Add(new SqlParameter("timeEnd", array_TongQuan.dayEnd));
            sql.Add(new SqlParameter("ID_NguoiDung", array_TongQuan.ID_NguoiDung));
            sql.Add(new SqlParameter("ID_DonVi", array_TongQuan.ID_DonVi));
            lst = db.Database.SqlQuery<BieuDo_DoanhThuPROC>("exec BaoCaoTongQuan_BieuDoDoanhThuToHour @timeStart, @timeEnd, @ID_NguoiDung, @ID_DonVi", sql.ToArray()).ToList();
            List<BieuDo_DoanhThuPROC> lst_date = lst.GroupBy(x => x.NgayLapHoaDon).Select(t => new BieuDo_DoanhThuPROC
            {
                NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon
            }).OrderBy(x => x.NgayLapHoaDon).ToList();
            List<BieuDo_DoanhThuPROC> lst_ChiNhanh = lst.GroupBy(x => x.TenChiNhanh).Select(t => new BieuDo_DoanhThuPROC
            {
                TenChiNhanh = t.FirstOrDefault().TenChiNhanh,
                ThanhTien = t.Sum(x => x.ThanhTien)
            }).OrderBy(x => x.TenChiNhanh).ToList();
            JsonResultExample_BieuDo<BieuDo_DoanhThuPROC> jsonobj = new JsonResultExample_BieuDo<BieuDo_DoanhThuPROC>
            {
                LstData = lst,
                LstDate = lst_date,
                LstChiNhanh = lst_ChiNhanh
            };
            return Json(jsonobj);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult TongQuan_BieuDoThucThuToDay(array_TongQuan array_TongQuan)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<BieuDo_DoanhThuPROC> lst = new List<BieuDo_DoanhThuPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", array_TongQuan.dayStart));
            sql.Add(new SqlParameter("timeEnd", array_TongQuan.dayEnd));
            sql.Add(new SqlParameter("ID_NguoiDung", array_TongQuan.ID_NguoiDung));
            sql.Add(new SqlParameter("ID_DonVi", array_TongQuan.ID_DonVi));
            lst = db.Database.SqlQuery<BieuDo_DoanhThuPROC>("exec BaoCaoTongQuan_BieuDoThucThuToDay @timeStart, @timeEnd, @ID_NguoiDung, @ID_DonVi", sql.ToArray()).ToList();
            List<BieuDo_DoanhThuPROC> lst_date = lst.GroupBy(x => x.NgayLapHoaDon).Select(t => new BieuDo_DoanhThuPROC
            {
                NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon
            }).OrderBy(x => x.NgayLapHoaDon).ToList();
            List<BieuDo_DoanhThuPROC> lst_ChiNhanh = lst.GroupBy(x => x.TenChiNhanh).Select(t => new BieuDo_DoanhThuPROC
            {
                TenChiNhanh = t.FirstOrDefault().TenChiNhanh,
                ThanhTien = t.Sum(x => x.ThanhTien)
            }).OrderBy(x => x.TenChiNhanh).ToList();
            JsonResultExample_BieuDo<BieuDo_DoanhThuPROC> jsonobj = new JsonResultExample_BieuDo<BieuDo_DoanhThuPROC>
            {
                LstData = lst,
                LstDate = lst_date,
                LstChiNhanh = lst_ChiNhanh
            };
            return Json(jsonobj);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult TongQuan_BieuDoThucThuToHour(array_TongQuan array_TongQuan)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<BieuDo_DoanhThuPROC> lst = new List<BieuDo_DoanhThuPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", array_TongQuan.dayStart));
            sql.Add(new SqlParameter("timeEnd", array_TongQuan.dayEnd));
            sql.Add(new SqlParameter("ID_NguoiDung", array_TongQuan.ID_NguoiDung));
            sql.Add(new SqlParameter("ID_DonVi", array_TongQuan.ID_DonVi));
            lst = db.Database.SqlQuery<BieuDo_DoanhThuPROC>("exec BaoCaoTongQuan_BieuDoThucThuToHour @timeStart, @timeEnd, @ID_NguoiDung, @ID_DonVi", sql.ToArray()).ToList();
            List<BieuDo_DoanhThuPROC> lst_date = lst.GroupBy(x => x.NgayLapHoaDon).Select(t => new BieuDo_DoanhThuPROC
            {
                NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon
            }).OrderBy(x => x.NgayLapHoaDon).ToList();
            List<BieuDo_DoanhThuPROC> lst_ChiNhanh = lst.GroupBy(x => x.TenChiNhanh).Select(t => new BieuDo_DoanhThuPROC
            {
                TenChiNhanh = t.FirstOrDefault().TenChiNhanh,
                ThanhTien = t.Sum(x => x.ThanhTien)
            }).OrderBy(x => x.TenChiNhanh).ToList();
            JsonResultExample_BieuDo<BieuDo_DoanhThuPROC> jsonobj = new JsonResultExample_BieuDo<BieuDo_DoanhThuPROC>
            {
                LstData = lst,
                LstDate = lst_date,
                LstChiNhanh = lst_ChiNhanh
            };
            return Json(jsonobj);
        }
        //[AcceptVerbs("GET", "POST")]
        //public IHttpActionResult getList_SuKienToDay(Guid ID_DonVi)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<SuKienToDayPROC> lst = new List<SuKienToDayPROC>();
        //    List<SqlParameter> sql = new List<SqlParameter>();
        //    sql.Add(new SqlParameter("ID_DonVi", ID_DonVi));
        //    lst = db.Database.SqlQuery<SuKienToDayPROC>("exec getlist_SuKienToDay @ID_DonVi", sql.ToArray()).ToList();
        //    JsonResultExample<SuKienToDayPROC> jsonobj = new JsonResultExample<SuKienToDayPROC>
        //    {
        //        LstData = lst,
        //    };
        //    return Json(jsonobj);
        //}

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_SuKienToDay_v2(Guid ID_DonVi, DateTime date)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<getlist_SuKienToDay_v2> lst = new List<getlist_SuKienToDay_v2>();
                    List<SqlParameter> sql = new List<SqlParameter>();
                    sql.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                    sql.Add(new SqlParameter("Date", date));
                    lst = db.Database.SqlQuery<getlist_SuKienToDay_v2>("exec getlist_SuKienToDay_v2 @ID_DonVi, @Date", sql.ToArray()).ToList();
                    return ActionTrueData(new
                    {
                        data = lst[0]
                    });
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        public List<BC_DoanhThu> getDiary1(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<BC_DoanhThu> lst = ClassDoanhThu.getdiary(dayStart, dayEnd, IDchinhanh);
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        public List<BC_DoanhThuPRC> getDiary(Guid IDchinhanh, string TongQuan_XemDS_PhongBan, string TongQuan_XemDS_HeThong, Guid ID_NguoiDung)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<SqlParameter> parama = new List<SqlParameter>();
            parama.Add(new SqlParameter("ID_DonVi", IDchinhanh));
            parama.Add(new SqlParameter("TongQuan_XemDS_PhongBan", TongQuan_XemDS_PhongBan));
            parama.Add(new SqlParameter("TongQuan_XemDS_HeThong", TongQuan_XemDS_HeThong));
            parama.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
            List<BC_DoanhThuPRC> lst = db.Database.SqlQuery<BC_DoanhThuPRC>("exec BaoCaoTongQuan_NhatKyHoatDong @ID_DonVi, @TongQuan_XemDS_PhongBan, @TongQuan_XemDS_HeThong, @ID_NguoiDung", parama.ToArray()).ToList();
            return lst;
        }

        public List<BC_DoanhThu> getResultToday(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<BC_DoanhThu> lst = ClassDoanhThu.getResultHDToday(dayStart, dayEnd, IDchinhanh);
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }

        }

        public List<BC_DoanhThu> getResulCountPT(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<BC_DoanhThu> lst = ClassDoanhThu.getResultCountPT(dayStart, dayEnd, IDchinhanh);
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }

        }

        public List<BC_DoanhThu> getResultMoneyPT(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            List<BC_DoanhThu> lst = ClassDoanhThu.getResultMoneyPT(dayStart, dayEnd, IDchinhanh);
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        #endregion
        #region tạo dữ liệu mẫu
        //[HttpPost, ActionName("Creater_DuLieuMau")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult Creater_DuLieuMau(Guid ID_DonVi, Guid ID_NhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument classOffice = new Class_officeDocument(db);
                var ID_LoaiHinhKD = CookieStore.GetCookieAes("shop").ToUpper();
                string file_DuLieuMau = string.Empty;
                if (ID_LoaiHinhKD == "AC9DF2ED-FF08-488F-9A64-08433E541020") //Thẩm mỹ viện
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_TMV.xlsx");
                else if (ID_LoaiHinhKD == "325543AB-52EE-408F-A7CC-1565E2A08938")// điện máy
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_DTDM.xlsx");
                else if (ID_LoaiHinhKD == "83894499-AEFA-4F58-96B4-5EC1A0B16A76")// spa, gim
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_SGY.xlsx");
                else if (ID_LoaiHinhKD == "40B6BFA7-1DD3-46D3-9C72-6F430C1E6841")//Mỹ phẩm
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_MP.xlsx");
                else if (ID_LoaiHinhKD == "B287F9B2-AA62-4ECB-988D-710F84FB59A7")//Thời trang
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_TT.xlsx");
                else if (ID_LoaiHinhKD == "C16EDDA0-F6D0-43E1-A469-844FAB143014")//Phụ tùng ô tô, xe máy
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_PTOXM.xlsx");
                else if (ID_LoaiHinhKD == "60DFD18A-549C-4456-AFF0-9076A6B7C1A3")//Nhà thuốc
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_NT.xlsx");
                else if (ID_LoaiHinhKD == "C1D14B5A-6E81-4893-9F73-E11C63C8E6BC")//Nhà hàng
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_NHCFB.xlsx");
                else if (ID_LoaiHinhKD == "4615C021-006F-4D59-9FB0-E1AD2F329138")//Siêu thị mini
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_STMN.xlsx");
                else if (ID_LoaiHinhKD == "4ACAB614-7C84-48D7-A530-914AFC95AD9B") //Đồ chơi trẻ em
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_DCTE.xlsx");
                else if (ID_LoaiHinhKD == "EDB8AAC0-14A8-4AEE-A149-DC3850DBB094") //Nông sản thực phẩm
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_NSTP.xlsx");
                else if (ID_LoaiHinhKD == "50BB1456-39DE-4802-A584-B0248EE4FE88") //Nội thất
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_NOITHAT.xlsx");
                else if (ID_LoaiHinhKD == "1D3E0481-4E03-4408-9BB1-E59EEDE0C8B7") //Thiết bị công nghệ
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_TBCN.xlsx");
                else if (ID_LoaiHinhKD == "1576F07C-FF9F-4094-AF5F-2996E4CE9A08") //Văn phòng phẩm quà
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_VPPQ.xlsx");
                else if (ID_LoaiHinhKD == "88043CCD-7ED6-491F-82D6-E5DAD7754F98")//Lĩnh vực khác
                    file_DuLieuMau = HttpContext.Current.Server.MapPath("~/Template/DuLieuGoc/DuLieuGoc_STMN.xlsx");
                string result = "";
                try
                {
                    if (ID_LoaiHinhKD == "40B6BFA7-1DD3-46D3-9C72-6F430C1E6841" || ID_LoaiHinhKD == "60DFD18A-549C-4456-AFF0-9076A6B7C1A3" || ID_LoaiHinhKD == "EDB8AAC0-14A8-4AEE-A149-DC3850DBB094")
                        classOffice.Create_DuLieuGoc_LoHang(file_DuLieuMau, ID_DonVi, ID_NhanVien);
                    else
                        classOffice.Create_DuLieuGoc(file_DuLieuMau, ID_DonVi, ID_NhanVien);
                    if (ID_LoaiHinhKD == "C1D14B5A-6E81-4893-9F73-E11C63C8E6BC")
                    {
                        List<SqlParameter> prm_KV1 = new List<SqlParameter>();
                        prm_KV1.Add(new SqlParameter("ID", "61000000-0000-0000-0000-000000000001"));
                        prm_KV1.Add(new SqlParameter("MaKhuVuc", "KV00001"));
                        prm_KV1.Add(new SqlParameter("TenKhuVuc", "Tầng 1"));
                        prm_KV1.Add(new SqlParameter("TimeCreate", DateTime.Now.AddDays(-50)));
                        db.Database.ExecuteSqlCommand("exec insert_DM_KhuVuc @ID, @MaKhuVuc, @TenKhuVuc, @TimeCreate", prm_KV1.ToArray());
                        List<SqlParameter> prm_KV2 = new List<SqlParameter>();
                        prm_KV2.Add(new SqlParameter("ID", "61000000-0000-0000-0000-000000000002"));
                        prm_KV2.Add(new SqlParameter("MaKhuVuc", "KV00002"));
                        prm_KV2.Add(new SqlParameter("TenKhuVuc", "Tầng 2"));
                        prm_KV2.Add(new SqlParameter("TimeCreate", DateTime.Now.AddDays(-50)));
                        db.Database.ExecuteSqlCommand("exec insert_DM_KhuVuc @ID, @MaKhuVuc, @TenKhuVuc, @TimeCreate", prm_KV2.ToArray());
                        List<SqlParameter> prm_KV3 = new List<SqlParameter>();
                        prm_KV3.Add(new SqlParameter("ID", "61000000-0000-0000-0000-000000000003"));
                        prm_KV3.Add(new SqlParameter("MaKhuVuc", "KV00003"));
                        prm_KV3.Add(new SqlParameter("TenKhuVuc", "Tầng 3"));
                        prm_KV3.Add(new SqlParameter("TimeCreate", DateTime.Now.AddDays(-50)));
                        db.Database.ExecuteSqlCommand("exec insert_DM_KhuVuc @ID, @MaKhuVuc, @TenKhuVuc, @TimeCreate", prm_KV3.ToArray());
                        // insert phòng bàn
                        List<SqlParameter> prm_PB1 = new List<SqlParameter>();
                        prm_PB1.Add(new SqlParameter("ID", "62000000-0000-0000-0000-000000000001"));
                        prm_PB1.Add(new SqlParameter("ID_KhuVuc", "61000000-0000-0000-0000-000000000001"));
                        prm_PB1.Add(new SqlParameter("MaViTri", "PB00001"));
                        prm_PB1.Add(new SqlParameter("TenViTri", "Bàn 1"));
                        db.Database.ExecuteSqlCommand("exec insert_DM_ViTri @ID, @ID_KhuVuc, @MaViTri, @TenViTri", prm_PB1.ToArray());
                        List<SqlParameter> prm_PB2 = new List<SqlParameter>();
                        prm_PB2.Add(new SqlParameter("ID", "62000000-0000-0000-0000-000000000002"));
                        prm_PB2.Add(new SqlParameter("ID_KhuVuc", "61000000-0000-0000-0000-000000000001"));
                        prm_PB2.Add(new SqlParameter("MaViTri", "PB00002"));
                        prm_PB2.Add(new SqlParameter("TenViTri", "Bàn 2"));
                        db.Database.ExecuteSqlCommand("exec insert_DM_ViTri @ID, @ID_KhuVuc, @MaViTri, @TenViTri", prm_PB2.ToArray());
                        List<SqlParameter> prm_PB3 = new List<SqlParameter>();
                        prm_PB3.Add(new SqlParameter("ID", "62000000-0000-0000-0000-000000000003"));
                        prm_PB3.Add(new SqlParameter("ID_KhuVuc", "61000000-0000-0000-0000-000000000002"));
                        prm_PB3.Add(new SqlParameter("MaViTri", "PB00003"));
                        prm_PB3.Add(new SqlParameter("TenViTri", "Bàn 3"));
                        db.Database.ExecuteSqlCommand("exec insert_DM_ViTri @ID, @ID_KhuVuc, @MaViTri, @TenViTri", prm_PB3.ToArray());
                        List<SqlParameter> prm_PB4 = new List<SqlParameter>();
                        prm_PB4.Add(new SqlParameter("ID", "62000000-0000-0000-0000-000000000004"));
                        prm_PB4.Add(new SqlParameter("ID_KhuVuc", "61000000-0000-0000-0000-000000000002"));
                        prm_PB4.Add(new SqlParameter("MaViTri", "PB00004"));
                        prm_PB4.Add(new SqlParameter("TenViTri", "Bàn 4"));
                        db.Database.ExecuteSqlCommand("exec insert_DM_ViTri @ID, @ID_KhuVuc, @MaViTri, @TenViTri", prm_PB4.ToArray());
                        List<SqlParameter> prm_PB5 = new List<SqlParameter>();
                        prm_PB5.Add(new SqlParameter("ID", "62000000-0000-0000-0000-000000000005"));
                        prm_PB5.Add(new SqlParameter("ID_KhuVuc", "61000000-0000-0000-0000-000000000003"));
                        prm_PB5.Add(new SqlParameter("MaViTri", "PB00005"));
                        prm_PB5.Add(new SqlParameter("TenViTri", "Bàn 5"));
                        db.Database.ExecuteSqlCommand("exec insert_DM_ViTri @ID, @ID_KhuVuc, @MaViTri, @TenViTri", prm_PB5.ToArray());
                        List<SqlParameter> prm_PB6 = new List<SqlParameter>();
                        prm_PB6.Add(new SqlParameter("ID", "62000000-0000-0000-0000-000000000006"));
                        prm_PB6.Add(new SqlParameter("ID_KhuVuc", "61000000-0000-0000-0000-000000000003"));
                        prm_PB6.Add(new SqlParameter("MaViTri", "PB00006"));
                        prm_PB6.Add(new SqlParameter("TenViTri", "Bàn 6"));
                        db.Database.ExecuteSqlCommand("exec insert_DM_ViTri @ID, @ID_KhuVuc, @MaViTri, @TenViTri", prm_PB6.ToArray());
                        // update ID_ViTri
                        db.Database.ExecuteSqlCommand("exec Update_HoaDonNHCFB");
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, ID_LoaiHinhKD));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }
        [HttpPost, ActionName("NK_SuDung")]
        public IHttpActionResult NK_SuDung(Guid ID_DonVi, Guid ID_NhanVien, string NoiDung)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            string result = "";
            try
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_NhanVien", ID_NhanVien));
                paramlist.Add(new SqlParameter("ID_DonVi", ID_DonVi));
                paramlist.Add(new SqlParameter("timeCreate", DateTime.Now));
                paramlist.Add(new SqlParameter("NoiDung", NoiDung));
                db.Database.ExecuteSqlCommand("exec NK_SuDung @ID_NhanVien, @ID_DonVi, @timeCreate, @NoiDung", paramlist.ToArray());
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, ""));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        #endregion
        [HttpGet]
        public bool check_CreateDL()
        {
            bool ex = true;
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from nk in db.HT_NhatKySuDung
                      where nk.LoaiNhatKy == 20
                      select nk;
            if (tbl.Count() > 0)
                ex = false;
            return ex;
        }
        [HttpGet]
        public bool update_QuanLyTheoLoHang()
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            bool ex = true;
            var ID_LoaiHinhKD = CookieStore.GetCookieAes("shop").ToUpper();
            if (ID_LoaiHinhKD == "40B6BFA7-1DD3-46D3-9C72-6F430C1E6841" || ID_LoaiHinhKD == "60DFD18A-549C-4456-AFF0-9076A6B7C1A3" || ID_LoaiHinhKD == "EDB8AAC0-14A8-4AEE-A149-DC3850DBB094")
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                db.Database.ExecuteSqlCommand("exec update_QuanLyLoHang", paramlist.ToArray());
            }
            return ex;
        }
    }
}
