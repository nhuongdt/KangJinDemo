using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using Model;
using System.Data.SqlClient;

namespace libQuy_HoaDon
{
    public class ClassDoanhThu
    {
        private SsoftvnContext _db;
        public ClassDoanhThu(SsoftvnContext db)
        {
            _db = db;
        }

        #region Select
        public List<BC_DoanhThu> getDoanhThuTT(CommonParamSearch param)
        {
            var idDonVis = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idDonVis = string.Join(",", param.IDChiNhanhs);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idDonVis ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
            return _db.Database.SqlQuery<BC_DoanhThu>("exec BaoCaoTongQuan_HangBanTheoDoanhThu @IDChiNhanhs," +
                " @DateFrom, @DateTo", sql.ToArray()).ToList();
        }

        public List<BC_DoanhThu> getDoanhThuSL(CommonParamSearch param)
        {
            var idDonVis = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idDonVis = string.Join(",", param.IDChiNhanhs);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", idDonVis ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateFrom", param.DateFrom ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
            return _db.Database.SqlQuery<BC_DoanhThu>("exec BaoCaoTongQuan_HangBanTheoSoLuong @IDChiNhanhs," +
                " @DateFrom, @DateTo", sql.ToArray()).ToList();
        }
        // load doanh thu theo chi nhanh
        public static List<PieChart_ChiNhanh> getDoanhThu_PieChiNhanh(DateTime dayStart, DateTime dayEnd, Guid ID_NguoiDung)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<PieChart_ChiNhanh> lst = new List<PieChart_ChiNhanh>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", dayStart));
            sql.Add(new SqlParameter("timeEnd", dayEnd));
            sql.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
            var tbl = db.Database.SqlQuery<BieuDo_DoanhThuPROC>("exec BaoCaoTongQuan_DoanhThuChiNhanh @timeStart, @timeEnd, @ID_NguoiDung", sql.ToArray()).ToList();
            try
            {
                double Tong = (double?)tbl.Sum(x => x.ThanhTien) ?? 1;
                foreach (var item in tbl)
                {
                    PieChart_ChiNhanh bC_DoanhThu = new PieChart_ChiNhanh();
                    bC_DoanhThu.name = item.TenChiNhanh.ToString();
                    bC_DoanhThu.y = Math.Round(item.ThanhTien / Tong, 2, MidpointRounding.ToEven);
                    if (Double.IsNaN(bC_DoanhThu.y)) bC_DoanhThu.y = 0;
                    bC_DoanhThu.vl = Math.Round(item.ThanhTien, 0, MidpointRounding.ToEven);
                    lst.Add(bC_DoanhThu);
                }
                if (lst != null)
                    return lst.OrderBy(x => x.name).ToList();
                else
                    return null;
            }
            catch
            {
                return lst;
            }
        }

        public static List<PieChart_ChiNhanh> getDoanhThu_ColumnChiNhanh(DateTime dayStart, DateTime dayEnd, Guid ID_NguoiDung)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<PieChart_ChiNhanh> lst = new List<PieChart_ChiNhanh>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("timeStart", dayStart));
            sql.Add(new SqlParameter("timeEnd", dayEnd));
            sql.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
            var tbl = db.Database.SqlQuery<BieuDo_DoanhThuPROC>("exec BaoCaoTongQuan_DoanhThuChiNhanh @timeStart, @timeEnd, @ID_NguoiDung", sql.ToArray()).ToList();
            foreach (var item in tbl)
            {
                PieChart_ChiNhanh bC_DoanhThu = new PieChart_ChiNhanh();
                bC_DoanhThu.name = item.TenChiNhanh.ToString();
                bC_DoanhThu.y = Math.Round(item.ThanhTien, 0, MidpointRounding.ToEven);
                lst.Add(bC_DoanhThu);
            }
            if (lst != null)
                return lst.OrderBy(x => x.name).ToList();
            else
                return null;
        }
        public static string getList_NhanVien(Guid ID_NhanVien)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            string lst = string.Empty;
            var tbl = from nv in db.NS_NhanVien
                      where nv.ID == ID_NhanVien
                      select new
                      {
                          TenNhanVien = nv.TenNhanVien
                      };
            try
            {
                lst = tbl.FirstOrDefault().TenNhanVien;
            }
            catch
            {

            }
            return lst;
        }
        //Load doanh thu
        public static List<BC_DoanhThu> getDoanhThuToday(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = (from bhhd in db.BH_HoaDon
                       join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon into dd
                       from tt in dd.DefaultIfEmpty()
                       join dvqd in db.DonViQuiDois on tt.ID_DonViQuiDoi equals dvqd.ID into qq
                       from vv in qq.DefaultIfEmpty()
                       join hh in db.DM_HangHoa on vv.ID_HangHoa equals hh.ID into vg
                       from kk in vg.DefaultIfEmpty()
                       join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into bv
                       from nn in bv.DefaultIfEmpty()
                       join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                       where (bhhd.NgayLapHoaDon >= dayStart & bhhd.NgayLapHoaDon < dayEnd) /*& dv.ID == IDchinhanh*/ & bhhd.ChoThanhToan == false & bhhd.LoaiHoaDon == 1
                       orderby bhhd.NgayLapHoaDon.Day
                       group tt by new
                       {
                           PatientDOB = bhhd.NgayLapHoaDon.Day,
                           TenChiNhanh = dv.TenDonVi,
                       } into g
                       select new
                       {
                           g.Key.PatientDOB,
                           g.Key.TenChiNhanh,
                           ThanhTien = g.Sum(x => (double?)x.ThanhTien ?? 0)
                       });

            List<BC_DoanhThu> lst = new List<BC_DoanhThu>();
            foreach (var item in tbl)
            {
                BC_DoanhThu bC_DoanhThu = new BC_DoanhThu();
                bC_DoanhThu.NgayLapHoaDon = item.PatientDOB.ToString();
                bC_DoanhThu.TenChiNhanh = item.TenChiNhanh.ToString();
                bC_DoanhThu.ThanhTien = Math.Round(item.ThanhTien, 0, MidpointRounding.ToEven);
                lst.Add(bC_DoanhThu);
            }
            if (lst != null)
                return lst.OrderBy(x => int.Parse(x.NgayLapHoaDon)).ToList();
            else
                return null;
        }

        public static List<BC_DoanhThu> getDoanhThuToDaybyChiNhanh(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = (from bhhd in db.BH_HoaDon
                       join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon into dd
                       from tt in dd.DefaultIfEmpty()
                       join dvqd in db.DonViQuiDois on tt.ID_DonViQuiDoi equals dvqd.ID into qq
                       from vv in qq.DefaultIfEmpty()
                       join hh in db.DM_HangHoa on vv.ID_HangHoa equals hh.ID into vg
                       from kk in vg.DefaultIfEmpty()
                       join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into bv
                       from nn in bv.DefaultIfEmpty()
                       join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                       where (bhhd.NgayLapHoaDon >= dayStart & bhhd.NgayLapHoaDon < dayEnd) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false & bhhd.LoaiHoaDon == 1
                       orderby bhhd.NgayLapHoaDon.Day
                       group tt by new
                       {
                           PatientDOB = bhhd.NgayLapHoaDon.Day,
                           TenChiNhanh = dv.TenDonVi,
                       } into g
                       select new
                       {
                           g.Key.PatientDOB,
                           g.Key.TenChiNhanh,
                           ThanhTien = g.Sum(x => (double?)x.ThanhTien ?? 0)
                       });

            List<BC_DoanhThu> lst = new List<BC_DoanhThu>();
            foreach (var item in tbl)
            {
                BC_DoanhThu bC_DoanhThu = new BC_DoanhThu();
                bC_DoanhThu.NgayLapHoaDon = item.PatientDOB.ToString();
                bC_DoanhThu.TenChiNhanh = item.TenChiNhanh.ToString();
                bC_DoanhThu.ThanhTien = Math.Round(item.ThanhTien, 0, MidpointRounding.ToEven);
                lst.Add(bC_DoanhThu);
            }
            if (lst != null)
                return lst.OrderBy(x => int.Parse(x.NgayLapHoaDon)).ToList();
            else
                return null;
        }

        public static List<BC_DoanhThu> getDoanhThuToHour(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = (from bhhd in db.BH_HoaDon
                       join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon into dd
                       from tt in dd.DefaultIfEmpty()
                       join dvqd in db.DonViQuiDois on tt.ID_DonViQuiDoi equals dvqd.ID into qq
                       from vv in qq.DefaultIfEmpty()
                       join hh in db.DM_HangHoa on vv.ID_HangHoa equals hh.ID into hv
                       from kk in hv.DefaultIfEmpty()
                       join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vg
                       from nn in vg.DefaultIfEmpty()
                       join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                       where (bhhd.NgayLapHoaDon >= dayStart & bhhd.NgayLapHoaDon < dayEnd) & bhhd.ChoThanhToan == false & bhhd.LoaiHoaDon == 1
                       orderby bhhd.NgayLapHoaDon.Day
                       group tt by new
                       {
                           PatientDOB = bhhd.NgayLapHoaDon.Hour,
                           TenChiNhanh = dv.TenDonVi,
                       } into g
                       select new
                       {
                           g.Key.PatientDOB,
                           g.Key.TenChiNhanh,
                           ThanhTien = g.Sum(x => (double?)x.ThanhTien ?? 0)
                       });

            List<BC_DoanhThu> lst = new List<BC_DoanhThu>();
            foreach (var item in tbl)
            {
                BC_DoanhThu bC_DoanhThu = new BC_DoanhThu();
                bC_DoanhThu.NgayLapHoaDon = item.PatientDOB.ToString();
                bC_DoanhThu.TenChiNhanh = item.TenChiNhanh.ToString();
                bC_DoanhThu.ThanhTien = Math.Round(item.ThanhTien, 0, MidpointRounding.ToEven);
                lst.Add(bC_DoanhThu);
            }
            if (lst != null)
                return lst.OrderBy(x => int.Parse(x.NgayLapHoaDon)).ToList();
            else
                return null;
        }

        public static List<BC_DoanhThu> getdiary(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = (from bhhd in db.BH_HoaDon
                       join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                       join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID //into dd
                                                                                            // from qq in dd.DefaultIfEmpty()
                       join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID //into qh
                                                                                // from kk in qh.DefaultIfEmpty()
                       join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID //into nd
                                                                                  // from nn in nd.DefaultIfEmpty()
                       join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                       join ct in db.DM_LoaiChungTu on bhhd.LoaiHoaDon equals ct.ID
                       where (bhhd.NgayLapHoaDon >= dayStart & bhhd.NgayLapHoaDon < dayEnd) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false & (bhhd.LoaiHoaDon == 1 || bhhd.LoaiHoaDon == 4 || bhhd.LoaiHoaDon == 5 || bhhd.LoaiHoaDon == 6 || bhhd.LoaiHoaDon == 7 || bhhd.LoaiHoaDon == 8)
                       //orderby bhhd.NgayLapHoaDon descending
                       group bhhdct by new
                       {
                           nv.TenNhanVien,
                           ct.TenLoaiChungTu,
                           bhhd.NgayLapHoaDon
                       } into g
                       select new
                       {
                           TenNhanVien = g.Key.TenNhanVien,
                           TenLoaiChungTu = g.Key.TenLoaiChungTu,
                           NgayLapHoaDon = g.Key.NgayLapHoaDon,
                           ThanhTien = g.Sum(x => (double?)x.ThanhTien ?? 0)
                       }).OrderByDescending(x => x.NgayLapHoaDon).Take(12);
            List<BC_DoanhThu> lst = new List<BC_DoanhThu>();
            foreach (var item in tbl)
            {
                BC_DoanhThu bC_DoanhThu = new BC_DoanhThu();
                bC_DoanhThu.TenNhanVien = item.TenNhanVien;
                bC_DoanhThu.ThanhTien = Math.Round(item.ThanhTien, 0, MidpointRounding.ToEven);
                bC_DoanhThu.NgayLapHoaDon = item.NgayLapHoaDon.ToString();
                bC_DoanhThu.TenLoaiChungTu = item.TenLoaiChungTu == null ? "" : (item.TenLoaiChungTu == "Hóa đơn bán lẻ" ? "bán đơn hàng" : (item.TenLoaiChungTu == "Phiếu nhập kho" ? "nhập hàng" : (item.TenLoaiChungTu == "Trả hàng nhà cung cấp" ? "trả hàng" : "")));
                lst.Add(bC_DoanhThu);
            }
            if (lst != null)
                return lst;
            else
                return null;
        }

        // load kết quả BH hôm nay
        public static List<BC_DoanhThu> getResultHDToday(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from bhhd in db.BH_HoaDon
                      where (bhhd.NgayLapHoaDon >= dayStart & bhhd.NgayLapHoaDon < dayEnd) & bhhd.ID_DonVi == IDchinhanh & bhhd.ChoThanhToan == false & bhhd.LoaiHoaDon == 1
                      group bhhd by new
                      {
                          bhhd.MaHoaDon,
                      } into g
                      select new BC_DoanhThu
                      {
                          SoHoaDon = g.Count(),
                      };
            List<BC_DoanhThu> lst = new List<BC_DoanhThu>();
            try
            {
                lst = tbl.ToList();
            }
            catch
            {
                lst = null;
            }
            return lst;
        }
        //load phieu tra hang
        public static List<BC_DoanhThu> getResultCountPT(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from bhhd in db.BH_HoaDon
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon into dd
                      from tt in dd.DefaultIfEmpty()
                      join dvqd in db.DonViQuiDois on tt.ID_DonViQuiDoi equals dvqd.ID into qq
                      from vv in qq.DefaultIfEmpty()
                      join hh in db.DM_HangHoa on vv.ID_HangHoa equals hh.ID into hv
                      from kk in hv.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vg
                      from nn in vg.DefaultIfEmpty()
                      join ct in db.DM_LoaiChungTu on bhhd.LoaiHoaDon equals ct.ID into cc
                      from rr in cc.DefaultIfEmpty()
                      where (bhhd.NgayLapHoaDon >= dayStart & bhhd.NgayLapHoaDon < dayEnd) & (bhhd.LoaiHoaDon == 6) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      group tt by new
                      {
                          bhhd.MaHoaDon,
                      } into g
                      select new
                      {
                          NumberResolved = g.Count(),
                      };
            List<BC_DoanhThu> lst = new List<BC_DoanhThu>();
            foreach (var item in tbl)
            {
                BC_DoanhThu bC_DoanhThu = new BC_DoanhThu();
                bC_DoanhThu.SoHoaDon = item.NumberResolved;
                lst.Add(bC_DoanhThu);
            }
            if (lst != null)
                return lst;
            else
                return null;
        }
        public static List<BC_DoanhThu> getResultMoneyPT(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from bhhd in db.BH_HoaDon
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon into dd
                      from tt in dd.DefaultIfEmpty()
                      join dvqd in db.DonViQuiDois on tt.ID_DonViQuiDoi equals dvqd.ID into qq
                      from vv in qq.DefaultIfEmpty()
                      join hh in db.DM_HangHoa on vv.ID_HangHoa equals hh.ID into hv
                      from kk in hv.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vg
                      from nn in vg.DefaultIfEmpty()
                      join ct in db.DM_LoaiChungTu on bhhd.LoaiHoaDon equals ct.ID
                      where (bhhd.NgayLapHoaDon >= dayStart & bhhd.NgayLapHoaDon < dayEnd) & (bhhd.LoaiHoaDon == 6) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      group tt by new
                      {
                          PatientDOB = bhhd.NgayLapHoaDon.Day
                      } into g
                      select new
                      {
                          ThanhTien = g.Sum(x => (double?)x.ThanhTien ?? 0)
                      };
            List<BC_DoanhThu> lst = new List<BC_DoanhThu>();
            foreach (var item in tbl)
            {
                BC_DoanhThu bC_DoanhThu = new BC_DoanhThu();
                bC_DoanhThu.ThanhTien = Math.Round(item.ThanhTien, 0, MidpointRounding.ToEven);
                lst.Add(bC_DoanhThu);
            }
            if (lst != null)
                return lst;
            else
                return null;
        }
        #endregion
    }

    public class SoSanhCungKyPRC
    {
        public double SoSanhCungKy { get; set; }
    }
    public class SoSanhCungKyThuChiPRC
    {
        public double ThuCungKy { get; set; }
        public double ChiCungKy { get; set; }
    }
    public class TongQuanKhachHangPRC
    {
        public double KhachHangGiaoDichLanDau { get; set; }
        public double KhachHangQuayLai { get; set; }
        public double KhachHangTaoMoi { get; set; }
    }
    public class TongQuanThuChiPRC
    {
        public double TienThu_Thu { get; set; }
        public double TienMat_Thu { get; set; }
        public double NganHang_Thu { get; set; }
        public double TienThu_Chi { get; set; }
        public double TienMat_Chi { get; set; }
        public double NganHang_Chi { get; set; }
        public double? ThuNo_Mat { get; set; }
        public double? ThuNo_NganHang { get; set; }
        public double? ThuNo_Tong { get; set; }
    }
    public class BC_DoanhThu
    {
        public string NgayLapHoaDon { get; set; }
        public string TenChiNhanh { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double ThanhTien { get; set; }
        public double SoLuong { get; set; }
        public string TenNhanVien { get; set; }
        public int LoaiHoaDon { get; set; }
        public string TenLoaiChungTu { get; set; }
        public int SoHoaDon { get; set; }
    }
    public class BieuDo_DoanhThuPROC
    {
        public int NgayLapHoaDon { get; set; }
        public string TenChiNhanh { get; set; }
        public double ThanhTien { get; set; }
    }

    public class TongQuanBieuDoDoanhThuThuan
    {
        public Guid ID_DonVi { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public int ThoiGian { get; set; }
        public double DoanhThuThuan { get; set; }
        public double LoiNhuan { get; set; }
    }

    public class TongQuanBieuDoThucThu
    {
        public Guid ID_DonVi { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public int ThoiGian { get; set; }
        public double ThucThu { get; set; }
    }

    public class SuKienToDayPROC
    {
        public double SinhNhat { get; set; }
        public double CongViec { get; set; }
        public double LichHen { get; set; }
        public double SoLoSapHetHan { get; set; }
        public double SoLoHetHan { get; set; }
    }

    public class getlist_SuKienToDay_v2
    {
        public double SinhNhat { get; set; }
        public double CongViec { get; set; }
        public double LichHen { get; set; }
        public double SoLoSapHetHan { get; set; }
        public double SoLoHetHan { get; set; }
        public double XeMoiTiepNhan { get; set; }
        public double XeXuatXuong { get; set; }
        public double KhachHangMoi { get; set; }
    }
    public class TongQuan_DoanhThuPROC
    {
        public double HD_SoLuongBan { get; set; }
        public double HD_ThanhTien { get; set; }
        public double GDV_SoLuongBan { get; set; }
        public double GDV_ThanhTien { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double DoanhThuThangNay { get; set; }
        public double DoanhThuThangTruoc { get; set; }
        public double SoSanhCungKy { get; set; }
    }

    public class TongQuanDoanhThuCongNo
    {
        public double DoanhThuSuaChua { get; set; }
        public double DoanhThuBanHang { get; set; }
        public double TongDoanhThu { get; set; }
        public double CongNoPhaiThu { get; set; }
        public double CongNoPhaiTra { get; set; }
        public double TongCongNo { get; set; }
        public double ThuTienMat { get; set; }
        public double ThuNganHang { get; set; }
        public double TongTienThu { get; set; }
        public double ChiTienMat { get; set; }
        public double ChiNganHang { get; set; }
        public double TongTienChi { get; set; }
    }

    public class PieChart_ChiNhanh
    {
        public string name { get; set; }
        public double y { get; set; }
        public double vl { get; set; }
    }
    public class BieuDo
    {
        public int x { get; set; }
        public double y { get; set; }
    }

    public class objBieuDoDoanhTu
    {
        public Guid ID_DonVi { get; set; }
        public List<BieuDo> dataBieuDo { get; set; }
    }
    public class BC_DoanhThuPRC
    {
        public string TenNhanVien { get; set; }
        public string MaHoaDon { get; set; }
        public double ThanhTien { get; set; }
        public string NgayLapHoaDon { get; set; }
        public string TenLoaiChungTu { get; set; }
        public int LoaiHoaDon { get; set; }
    }
}
