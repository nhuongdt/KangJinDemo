using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using libDM_DoiTuong;
using System.Data.SqlClient;

namespace libQuy_HoaDon
{
    public class Class_Report
    {
        #region Báo cáo hàng hóa

        //public static List<Report_HangHoa_BanHang> TinhGiaTriBanHang(string maHH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_BanHang> lst = new List<Report_HangHoa_BanHang>();
        //    var tbl = from hd in db.BH_HoaDon
        //              join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //              join dvqd in db.DonViQuiDois.Where(x => x.MaHangHoa == maHH) on bhct.ID_DonViQuiDoi equals dvqd.ID
        //              where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
        //              group new { dvqd, hd, bhct } by new
        //              {
        //                  dvqd.MaHangHoa,
        //              } into g
        //              select new Report_HangHoa_BanHang
        //              {
        //                  SoLuongBan = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.SoLuong) ?? 0,
        //                  GiaTriBan = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.ThanhTien) ?? 0,
        //                  SoLuongTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.SoLuong) ?? 0,
        //                  GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.ThanhTien) ?? 0,
        //              };
        //    try
        //    {
        //        lst = tbl.ToList();
        //    }
        //    catch
        //    {

        //    }
        //    return lst;
        //}

        //public static List<Report_HangHoa_BanHang> getListReportHH_BanHang(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_BanHang> lst = new List<Report_HangHoa_BanHang>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & /*dvqd.Xoa != true & hh.TheoDoi != false &*/ hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

        //                  group new { dvqd, hd, bhct, hh } by new
        //                  {
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang,
        //                  } into g
        //                  select new
        //                  {
        //                      MaHangHoa = g.Key.MaHangHoa,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      TenHangHoa_KhongDau = g.FirstOrDefault().hh.TenHangHoa_KhongDau,
        //                      TenHangHoa_KyTuDau = g.FirstOrDefault().hh.TenHangHoa_KyTuDau,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_BanHang
        //        {
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = t.TenHangHoa_KhongDau,
        //            TenHangHoaGC = t.TenHangHoa_KyTuDau
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.ToLower().Contains(@maHH));
        //        }
        //        tbl1 = tbl1.OrderByDescending(x => x.MaHangHoa);
        //        try
        //        {
        //            lst = tbl1.ToList();
        //        }
        //        catch { }
        //    }
        //    else
        //    {

        //    }
        //    return lst;

        //}
        //public static List<Report_HangHoa_LoiNhuan> getListReportHH_LoiNhuan(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_LoiNhuan> lst = new List<Report_HangHoa_LoiNhuan>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6 || hd.LoaiHoaDon == 7) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang,
        //                      dvqd.GiaVon
        //                  } into g
        //                  select new
        //                  {
        //                      MaHangHoa = g.Key.MaHangHoa,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      SoLuongBan = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.SoLuong) ?? 0,
        //                      GiaTriBan = (double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.ThanhTien) ?? 0,
        //                      SoLuongTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6 || x.hd.LoaiHoaDon == 7).Sum(x => x.bhct.SoLuong) ?? 0,
        //                      GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6 || x.hd.LoaiHoaDon == 7).Sum(x => x.bhct.ThanhTien) ?? 0,
        //                      GiaVon = g.Key.GiaVon,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_LoiNhuan
        //        {
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = (maHH != null & maHH != "" & maHH != "null")? CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(): null,
        //            TenHangHoaGC = (maHH != null & maHH != "" & maHH != "null")? CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(): null,
        //            SoLuongBan = t.SoLuongBan,
        //            DoanhThu = t.GiaTriBan,
        //            SoLuongTra = t.SoLuongTra,
        //            GiaTriTra = t.GiaTriTra,
        //            DoanhThuThuan = t.GiaTriBan - t.GiaTriTra,
        //            TongGiaVon = (t.SoLuongBan - t.SoLuongTra) * t.GiaVon,
        //            LoiNhuan = (t.GiaTriBan - t.GiaTriTra) - ((t.SoLuongBan - t.SoLuongTra) * t.GiaVon),
        //            TySuat = Math.Round((((t.GiaTriBan - t.GiaTriTra) - ((t.SoLuongBan - t.SoLuongTra) * t.GiaVon)) / (t.GiaTriBan - t.GiaTriTra) * 100), 2).ToString() + " %"
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH);
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        tbl1 = tbl1.OrderByDescending(x => x.MaHangHoa);
        //        if (tbl1 != null)
        //            return tbl1.ToList();
        //        else
        //            return lst;
        //    }
        //    else
        //        return lst;

        //}
        public static List<Report_HangHoa_LoiNhuan> TinhGiaTriLoiNhuan(string maHH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_HangHoa_LoiNhuan> lst = new List<Report_HangHoa_LoiNhuan>();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois.Where(x => x.MaHangHoa == maHH) on bhct.ID_DonViQuiDoi equals dvqd.ID
                      where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.MaHangHoa,
                      } into g
                      select new Report_HangHoa_LoiNhuan
                      {
                          SoLuongBan = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.SoLuong) ?? 0, 2),
                          DoanhThu = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.ThanhTien) ?? 0, 2),
                          SoLuongTra = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.SoLuong) ?? 0, 2),
                          GiaTriTra = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.ThanhTien) ?? 0, 2),
                          TongGiaVonBan = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.SoLuong * x.bhct.GiaVon) ?? 0, 2),
                          TongGiaVonTra = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.SoLuong * x.bhct.GiaVon) ?? 0, 2)
                      };
            try
            {
                lst = tbl.ToList();
            }
            catch
            {

            }
            return lst;
        }
        //public static List<Report_HangHoa_LoiNhuan> TongGiaTriHH_LoiNhuan(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_LoiNhuan> lst = new List<Report_HangHoa_LoiNhuan>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & /*dvqd.Xoa != true & hh.TheoDoi != false &*/ hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
        //                  select new
        //                  {
        //                      hh.ID_NhomHang,
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      bhct.SoLuong,
        //                      bhct.ThanhTien,
        //                      hd.LoaiHoaDon,
        //                      bhct.GiaVon
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_SumHangHoa_BanHang
        //        {
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //            GiaVon = t.GiaVon,
        //            SoLuong = t.SoLuong,
        //            ThanhTien = t.ThanhTien,
        //            LoaiHoaDon = t.LoaiHoaDon
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH);
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }

        //        var tbl2 = from sum in tbl1
        //                   group sum by new
        //                   {
        //                   } into g
        //                   select new Report_HangHoa_LoiNhuan
        //                   {
        //                       SoLuongBan = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => x.SoLuong) ?? 0, 2),
        //                       DoanhThu = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => x.ThanhTien) ?? 0, 2),
        //                       SoLuongTra = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 6).Sum(x => x.SoLuong) ?? 0, 2),
        //                       GiaTriTra = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 6).Sum(x => x.ThanhTien) ?? 0, 2),
        //                       TongGiaVonBan = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => x.SoLuong * x.GiaVon) ?? 0, 2),
        //                       TongGiaVonTra = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 6).Sum(x => x.SoLuong * x.GiaVon) ?? 0, 2)
        //                   };
        //        try
        //        {
        //            lst = tbl2.ToList();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    return lst;
        //}

        //public static List<Report_HangHoa_LoiNhuan> getListReportHH_LoiNhuan(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_LoiNhuan> lst = new List<Report_HangHoa_LoiNhuan>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang,
        //                      //dvqd.GiaVon
        //                  } into g
        //                  select new Report_HangHoa_LoiNhuan
        //                  {
        //                      MaHangHoa = g.Key.MaHangHoa,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_LoiNhuan
        //            {
        //                MaHangHoa = t.MaHangHoa,
        //                TenHangHoa = t.TenHangHoa,
        //                TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //                TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower()
        //            });
        //            maHH = CommonStatic.ConvertToUnSign(maHH);
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //            try
        //            {
        //                lst = tbl1.OrderByDescending(x => x.MaHangHoa).ToList();
        //            }
        //            catch
        //            {

        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                lst = tbl.OrderByDescending(x => x.MaHangHoa).ToList();
        //            }
        //            catch
        //            {

        //            }
        //        }
        //    }
        //    else
        //    { }
        //    return lst;

        //}

        public static List<Report_HangHoa_XuatNhapTon> TinhGiaTriXuatNhapTon(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon _classBHHD = new ClassBH_HoaDon(db);
                List<Report_HangHoa_XuatNhapTon> lst = new List<Report_HangHoa_XuatNhapTon>();
                if (laHangHoa != 3)
                {
                    var tbl = from hd in db.BH_HoaDon
                              join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                              join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                              join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                              where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                              orderby hd.NgayLapHoaDon descending
                              group new { dvqd, hd, bhct } by new
                              {
                                  dvqd.ID_HangHoa,
                                  hh.LaHangHoa,
                                  hh.ID_NhomHang,
                                  hh.TenHangHoa,
                              } into g
                              select new
                              {
                                  ID_HangHoa = g.Key.ID_HangHoa,
                                  LaHangHoa = g.Key.LaHangHoa,
                                  ID_NhomHang = g.Key.ID_NhomHang,
                                  TenHangHoa = g.Key.TenHangHoa,
                                  MaHangHoa = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? "" :
                                  g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.MaHangHoa,
                                  GiaVon = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? 0 :
                                  g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.GiaVon,
                              };
                    if (laHangHoa == 0)
                    {
                        tbl = tbl.Where(x => x.LaHangHoa == false);
                    }
                    else if (laHangHoa == 1)
                    {
                        tbl = tbl.Where(x => x.LaHangHoa == true);
                    }
                    if (ID_NhomHang != null)
                    {
                        tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
                    }
                    var tbl1 = tbl.AsEnumerable().Select(t => new
                    {
                        ID_HangHoa = t.ID_HangHoa,
                        TenHangHoa = t.TenHangHoa,
                        TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                        TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                        MaHangHoa = t.MaHangHoa,
                        GiaVon = t.GiaVon,
                        lstTinh = SumSoLuongXuatNhanTonCT(t.ID_HangHoa, timeStart, timeEnd, ID_ChiNhanh),
                        lstTonKho = _classBHHD.TinhSLTonHHKK(t.ID_HangHoa, ID_ChiNhanh, timeStart),
                    });
                    if (maHH != null & maHH != "" & maHH != "null")
                    {
                        maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                        tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
                    }

                    var tbl_gop = from tb in tbl1
                                  group tb by new
                                  {

                                  } into g
                                  select new Report_HangHoa_XuatNhapTon
                                  {
                                      TonDauKy = Math.Round(g.Sum(x => (double?)x.lstTonKho ?? 0), 3, MidpointRounding.ToEven),
                                      GiaTriDauKy = Math.Round(g.Sum(x => (double?)x.lstTonKho * x.GiaVon ?? 0), 3, MidpointRounding.ToEven),
                                      SoLuongNhap = Math.Round(g.Sum(x => x.lstTinh[0].SoLuongNhap), 3, MidpointRounding.ToEven),
                                      GiaTriNhap = Math.Round(g.Sum(x => x.lstTinh[0].GiaTriNhap), 3, MidpointRounding.ToEven),
                                      SoLuongXuat = Math.Round(g.Sum(x => x.lstTinh[0].SoLuongXuat), 3, MidpointRounding.ToEven),
                                      GiaTriXuat = Math.Round(g.Sum(x => x.lstTinh[0].GiaTriXuat), 3, MidpointRounding.ToEven),
                                      TonCuoiKy = Math.Round(g.Sum(x => (x.lstTonKho.Value + x.lstTinh[0].SoLuongNhap - x.lstTinh[0].SoLuongXuat)), 3, MidpointRounding.ToEven),
                                      GiaTriCuoiKy = Math.Round(g.Sum(x => (x.lstTonKho.Value + x.lstTinh[0].SoLuongNhap - x.lstTinh[0].SoLuongXuat) * x.GiaVon), 3, MidpointRounding.ToEven)
                                  };
                    try
                    {
                        lst = tbl_gop.ToList();
                    }
                    catch { }
                }
                return lst;
            }
        }
        //public static List<Report_HangHoa_XuatNhapTon> getListReportHH_XuatNhapTon1(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_XuatNhapTon> lst = new List<Report_HangHoa_XuatNhapTon>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
        //                  orderby hd.NgayLapHoaDon descending
        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.ID_HangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang,
        //                      hh.TenHangHoa,
        //                  } into g
        //                  select new
        //                  {
        //                      ID_HangHoa = g.Key.ID_HangHoa,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      MaHangHoa = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? "" :
        //                      g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.MaHangHoa,
        //                      GiaVon = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? 0 :
        //                      g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.GiaVon
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_XuatNhapTon
        //        {
        //            ID_HangHoa = t.ID_HangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //            MaHangHoa = t.MaHangHoa,
        //            GiaVon = t.GiaVon
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        if (tbl1 != null)
        //            return tbl1.ToList();
        //        else
        //            return lst;
        //    }
        //    else
        //        return lst;
        //}

        // tính lại xuất nhập tồn
        //public static List<Report_HangHoa_XuatNhapTon> getListReportHH_XuatNhapTon(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_XuatNhapTon> lst = new List<Report_HangHoa_XuatNhapTon>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl_TrongKy = from hd in db.BH_HoaDon
        //                          join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                          join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                          join dvtc in db.DonViQuiDois.Where(x => x.LaDonViChuan == true) on dvqd.ID_HangHoa equals dvtc.ID_HangHoa
        //                          join hh in db.DM_HangHoa on dvtc.ID_HangHoa equals hh.ID
        //                          where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & (hd.ID_DonVi == ID_ChiNhanh || hd.ID_CheckIn == ID_ChiNhanh)
        //                          select new
        //                          {
        //                              ID_NhomHang = hh.ID_NhomHang,
        //                              ID_HangHoa = dvqd.ID_HangHoa,
        //                              GiaVon = dvtc.GiaVon,
        //                              SoLuong = hd.LoaiHoaDon != 10 ? bhct.SoLuong : bhct.TienChietKhau,
        //                              TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
        //                              LoaiHoaDon = (hd.LoaiHoaDon == 4 || hd.LoaiHoaDon == 6 || hd.LoaiHoaDon == 9) ? 1 : ((hd.ID_CheckIn != null & hd.ID_CheckIn == ID_ChiNhanh & hd.LoaiHoaDon == 10 & hd.YeuCau == "4") ? 2 :
        //                              ((hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 5 || (hd.LoaiHoaDon == 10 & hd.YeuCau == "1") || hd.LoaiHoaDon == 7 || hd.LoaiHoaDon == 8) ? 3 : (hd.ID_CheckIn != null & hd.ID_CheckIn != ID_ChiNhanh & hd.LoaiHoaDon == 10 & hd.YeuCau == "4") ? 4 : 0)),
        //                              MaHangHoa = dvtc.MaHangHoa,
        //                              TenHangHoa = hh.TenHangHoa,
        //                              LaHangHoa = hh.LaHangHoa,
        //                              ThanhTien = hd.LoaiHoaDon != 9 ? bhct.ThanhTien : (double?)bhct.SoLuong * dvtc.GiaVon ?? 0
        //                          };
        //        if (laHangHoa == 0)
        //        {
        //            tbl_TrongKy = tbl_TrongKy.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl_TrongKy = tbl_TrongKy.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl_TrongKy = tbl_TrongKy.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));

        //            //tbl_TrongKy = tbl_TrongKy.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tb1 = from tb in tbl_TrongKy
        //                  group tb by new
        //                  {
        //                      tb.ID_HangHoa
        //                  } into g
        //                  select new
        //                  {
        //                      ID_HangHoa = g.Key.ID_HangHoa,
        //                      GiaVon = (double?)g.FirstOrDefault().GiaVon ?? 0,
        //                      MaHangHoa = g.FirstOrDefault().MaHangHoa,
        //                      TenHangHoa = g.FirstOrDefault().TenHangHoa,
        //                      SoLuongNhap = (double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => (double?)x.SoLuong * x.TyLeChuyenDoi ?? 0) ?? 0,
        //                      GiaTriNhap = (double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => (double?)x.ThanhTien ?? 0) ?? 0,
        //                      SoLuongXuat = (double?)g.Where(x => x.LoaiHoaDon == 3 || x.LoaiHoaDon == 4).Sum(x => (double?)x.SoLuong * x.TyLeChuyenDoi ?? 0) ?? 0,
        //                      GiaTriXuat = (double?)g.Where(x => x.LoaiHoaDon == 3 || x.LoaiHoaDon == 4).Sum(x => (double?)x.ThanhTien ?? 0) ?? 0
        //                  };

        //        //List<string> liST = new List<string>();
        //        //foreach (var item in tb1)
        //        //{
        //        //    liST.Add(item.ID_HangHoa.ToString());
        //        //}
        //        var tbl_DauKy = from hd in db.BH_HoaDon
        //                        join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                        //join dvqd in db.DonViQuiDois.Where(x => liST.Contains(x.ID_HangHoa.ToString())) on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                        join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                        join tb in tb1 on dvqd.ID_HangHoa equals tb.ID_HangHoa
        //                        where hd.NgayLapHoaDon < timeStart & hd.ChoThanhToan == false & (hd.ID_DonVi == ID_ChiNhanh || hd.ID_CheckIn == ID_ChiNhanh)
        //                        select new
        //                        {
        //                            ID_HangHoa = dvqd.ID_HangHoa,
        //                            SoLuong = hd.LoaiHoaDon != 10 ? bhct.SoLuong : bhct.TienChietKhau,
        //                            TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
        //                            LoaiHoaDon = (hd.LoaiHoaDon == 4 || hd.LoaiHoaDon == 6 || hd.LoaiHoaDon == 9) ? 1 : ((hd.ID_CheckIn != null & hd.ID_CheckIn == ID_ChiNhanh & hd.LoaiHoaDon == 10 & hd.YeuCau == "4") ? 2 :
        //                              ((hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 5 || (hd.LoaiHoaDon == 10 & hd.YeuCau == "1") || hd.LoaiHoaDon == 7 || hd.LoaiHoaDon == 8) ? 3 : (hd.ID_CheckIn != null & hd.ID_CheckIn != ID_ChiNhanh & hd.LoaiHoaDon == 10 & hd.YeuCau == "4") ? 4 : 0)),
        //                        };
        //        var tb2 = from tb in tbl_DauKy
        //                  group tb by new
        //                  {
        //                      tb.ID_HangHoa
        //                  } into g
        //                  select new
        //                  {
        //                      ID_HangHoa = g.Key.ID_HangHoa,
        //                      SoLuongNhap = (double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => (double?)x.SoLuong * x.TyLeChuyenDoi ?? 0) ?? 0,
        //                      SoLuongXuat = (double?)g.Where(x => x.LoaiHoaDon == 3 || x.LoaiHoaDon == 4).Sum(x => (double?)x.SoLuong * x.TyLeChuyenDoi ?? 0) ?? 0,
        //                  } into abc
        //                  select new
        //                  {
        //                      ID_HangHoa = abc.ID_HangHoa,
        //                      TonDauKy = abc.SoLuongNhap - abc.SoLuongXuat
        //                  };
        //        var tbl_gop = from t in tb1
        //                      join b in tb2 on t.ID_HangHoa equals b.ID_HangHoa into d
        //                      from bd in d.DefaultIfEmpty()
        //                      select new
        //                      {
        //                          t.MaHangHoa,
        //                          t.TenHangHoa,
        //                          TonDauKy = (double?)bd.TonDauKy ?? 0,
        //                          t.GiaVon,
        //                          t.SoLuongNhap,
        //                          t.GiaTriNhap,
        //                          t.SoLuongXuat,
        //                          t.GiaTriXuat
        //                      };
        //        var tbl_format = tbl_gop.AsEnumerable().Select(t => new Report_HangHoa_XuatNhapTon
        //        {
        //            //ID_HangHoa = t.ID_HangHoa,
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //            GiaVon = Math.Round(t.GiaVon, 0, MidpointRounding.ToEven),
        //            SoLuongNhap = Math.Round(t.SoLuongNhap, 3, MidpointRounding.ToEven),
        //            GiaTriNhap = Math.Round(t.GiaTriNhap, 0, MidpointRounding.ToEven),
        //            SoLuongXuat = Math.Round(t.SoLuongXuat, 3, MidpointRounding.ToEven),
        //            GiaTriXuat = Math.Round(t.GiaTriXuat, 0, MidpointRounding.ToEven),
        //            TonDauKy = Math.Round(t.TonDauKy, 3, MidpointRounding.ToEven),
        //            GiaTriDauKy = Math.Round(t.TonDauKy * t.GiaVon, 0, MidpointRounding.ToEven),
        //            TonCuoiKy = Math.Round(t.TonDauKy + t.SoLuongNhap - t.SoLuongXuat, 3, MidpointRounding.ToEven),
        //            GiaTriCuoiKy = Math.Round((t.TonDauKy + t.SoLuongNhap - t.SoLuongXuat) * t.GiaVon, 0, MidpointRounding.ToEven),
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl_format = tbl_format.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        try
        //        {
        //            lst = tbl_format.OrderByDescending(x => x.TonCuoiKy).ToList();
        //        }
        //        catch
        //        {

        //        }
        //    }

        //    return lst;
        //}
        // xuất nhập tồn new
        public static List<Report_HangHoa_XuatNhapTon> getListHangHoa_XNT(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_HangHoa_XuatNhapTon> lst = new List<Report_HangHoa_XuatNhapTon>();
            #region tách
            var tbl_TonDau_Xuat = from bhdct in db.BH_HoaDon_ChiTiet
                                  join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                                  join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                  where bhd.NgayLapHoaDon < timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                                  (bhd.LoaiHoaDon == 1 || bhd.LoaiHoaDon == 5 || bhd.LoaiHoaDon == 7 || bhd.LoaiHoaDon == 8 || (bhd.LoaiHoaDon == 10 & bhd.YeuCau == "1") || (bhd.ID_CheckIn != null & bhd.ID_CheckIn != ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4"))
                                  & dvqd.Xoa == null
                                  select new Report_HangHoa_XuatNhapTon_Union
                                  {
                                      ID_HangHoa = dvqd.ID_HangHoa,
                                      ID_DonViQuiDoi = dvqd.ID,
                                      ID_NhomHang = hh.ID_NhomHang,
                                      MaHangHoa = dvqd.MaHangHoa,
                                      TenHangHoa = hh.TenHangHoa,
                                      TonDau_SoLuongXuat = (double?)(bhd.LoaiHoaDon == 10 ? bhdct.TienChietKhau : bhdct.SoLuong) ?? 0,
                                      TyLeChuyenDoi = (double?)dvqd.TyLeChuyenDoi ?? 0,
                                      LaHangHoa = hh.LaHangHoa,
                                      LaDonViChuan = dvqd.LaDonViChuan,
                                      GiaVon = (double?)dvqd.GiaVon ?? 0
                                  };
            var tbl_TonDau_Nhap = from bhdct in db.BH_HoaDon_ChiTiet
                                  join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                                  join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                  where bhd.NgayLapHoaDon < timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                                  (bhd.LoaiHoaDon == 4 || bhd.LoaiHoaDon == 6 || bhd.LoaiHoaDon == 9 || (bhd.ID_CheckIn != null & bhd.ID_CheckIn == ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4"))
                                  & dvqd.Xoa == null
                                  select new Report_HangHoa_XuatNhapTon_Union
                                  {
                                      ID_HangHoa = dvqd.ID_HangHoa,
                                      ID_DonViQuiDoi = dvqd.ID,
                                      SoLuongNhap = bhd.LoaiHoaDon == 10 ? bhdct.TienChietKhau : bhdct.SoLuong,
                                      TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                                      //ThanhTien = bhd.LoaiHoaDon != 9 ? bhdct.ThanhTien : (double?)bhdct.SoLuong * bhdct.GiaVon ?? 0
                                  };
            var tbl_TrongKy_Xuat = from bhdct in db.BH_HoaDon_ChiTiet
                                   join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                                   join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                                   join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                   where bhd.NgayLapHoaDon >= timeStart & bhd.NgayLapHoaDon < timeEnd & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                                   (bhd.LoaiHoaDon == 1 || bhd.LoaiHoaDon == 5 || bhd.LoaiHoaDon == 7 || bhd.LoaiHoaDon == 8 || (bhd.LoaiHoaDon == 10 & bhd.YeuCau == "1") || (bhd.ID_CheckIn != null & bhd.ID_CheckIn != ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4"))
                                   & dvqd.Xoa == null
                                   select new Report_HangHoa_XuatNhapTon_Union
                                   {
                                       ID_HangHoa = dvqd.ID_HangHoa,
                                       ID_DonViQuiDoi = dvqd.ID,
                                       SoLuongXuat = bhd.LoaiHoaDon == 10 ? bhdct.TienChietKhau : bhdct.SoLuong,
                                       TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                                       GiaTriXuat = bhdct.ThanhTien,
                                   };

            var tbl_TrongKy_Nhap = from bhdct in db.BH_HoaDon_ChiTiet
                                   join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                                   join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                                   join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                   where bhd.NgayLapHoaDon >= timeStart & bhd.NgayLapHoaDon < timeEnd & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                                   (bhd.LoaiHoaDon == 4 || bhd.LoaiHoaDon == 6 || bhd.LoaiHoaDon == 9 || (bhd.ID_CheckIn != null & bhd.ID_CheckIn == ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4"))
                                   & dvqd.Xoa == null
                                   select new Report_HangHoa_XuatNhapTon_Union
                                   {
                                       ID_HangHoa = dvqd.ID_HangHoa,
                                       ID_DonViQuiDoi = dvqd.ID,
                                       SoLuongNhap = bhd.LoaiHoaDon == 10 ? bhdct.TienChietKhau : bhdct.SoLuong,
                                       TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                                       GiaTriNhap = bhd.LoaiHoaDon != 9 ? bhdct.ThanhTien : (double?)bhdct.SoLuong * bhdct.GiaVon ?? 0
                                   };
            //var tbl_gop = from tb in 
            #endregion
            try
            {
                //lst = tbl_TonDau_Xuat.ToList();
            }
            catch
            {
            }
            return lst;
        }
        // xuất nhập tồn có tồn kho
        //public static List<Report_HangHoa_XuatNhapTon> getListHangHoaBy_MaHangHoa(string maHH, DateTime timeStart, DateTime timeEnd, int LaHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_XuatNhapTon> lst = new List<Report_HangHoa_XuatNhapTon>();
        //    //
        //    //if (maHH != null & maHH != "" & maHH != "null")
        //    //{
        //        var tbl_timeCS = from cs in db.ChotSo
        //                            where cs.ID_DonVi == ID_ChiNhanh
        //                            select new
        //                            {
        //                                cs.NgayChotSo
        //                            };
        //        DateTime timeChotSo = DateTime.Parse("2018-01-01");
        //        try
        //        {
        //            timeChotSo = tbl_timeCS.FirstOrDefault().NgayChotSo;
        //        }
        //        catch
        //        {

        //        }
        //        var tbl_HH =  from dvqd in db.DonViQuiDois
        //                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID                                                       
        //                      select new
        //                      {
        //                          dvqd.ID,
        //                          dvqd.MaHangHoa,
        //                          dvqd.GiaVon,
        //                          hh.TenHangHoa,
        //                          hh.TenHangHoa_KhongDau,
        //                          hh.TenHangHoa_KyTuDau,
        //                          hh.ID_NhomHang,
        //                          hh.LaHangHoa
        //                      };

        //        var tbl1 = from bhdct in db.BH_HoaDon_ChiTiet
        //                   join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
        //                   join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
        //                   join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                   where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
        //                   (bhd.LoaiHoaDon == 1 || bhd.LoaiHoaDon == 5 || bhd.LoaiHoaDon == 7 || bhd.LoaiHoaDon == 8)
        //                   & dvqd.Xoa == null & dvqd.MaHangHoa.Contains(maHH)
        //                   group new { bhd, bhdct, dvqd } by new
        //                   {
        //                       bhdct.ID_DonViQuiDoi
        //                   } into g
        //                   select new
        //                   {
        //                       //ID_HangHoa = dvqd.ID_HangHoa,
        //                       ID_DonViQuiDoi = g.Key.ID_DonViQuiDoi,
        //                       SoLuongNhap = 0,
        //                       SoLuongXuat = g.Sum(x => (double?)x.bhdct.SoLuong * x.dvqd.TyLeChuyenDoi ?? 0),
        //                   };
        //        var tbl2 = from bhdct in db.BH_HoaDon_ChiTiet
        //                   join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
        //                   join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
        //                   join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                   where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
        //                   ((bhd.LoaiHoaDon == 10 & bhd.YeuCau == "1") || (bhd.ID_CheckIn != null & bhd.ID_CheckIn != ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4"))
        //                   & dvqd.Xoa == null & dvqd.MaHangHoa.Contains(maHH)
        //                   group new { bhd, bhdct, dvqd } by new
        //                   {
        //                       bhdct.ID_DonViQuiDoi
        //                   } into g1
        //                   select new
        //                   {
        //                       //ID_HangHoa = dvqd.ID_HangHoa,
        //                       ID_DonViQuiDoi = g1.Key.ID_DonViQuiDoi,
        //                       SoLuongNhapCHuyen = 0,
        //                       SoLuongXuatChuyen = g1.Sum(x => (double?)x.bhdct.TienChietKhau * x.dvqd.TyLeChuyenDoi ?? 0),
        //                   };
        //        var tbl3 = from bhdct in db.BH_HoaDon_ChiTiet
        //                   join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
        //                   join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
        //                   join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                   where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
        //                   (bhd.LoaiHoaDon == 4 || bhd.LoaiHoaDon == 6 || bhd.LoaiHoaDon == 9)
        //                   & dvqd.Xoa == null & dvqd.MaHangHoa.Contains(maHH)
        //                   group new { bhd, bhdct, dvqd } by new
        //                   {
        //                       bhdct.ID_DonViQuiDoi
        //                   } into g2
        //                   select new
        //                   {
        //                       //ID_HangHoa = dvqd.ID_HangHoa,
        //                       ID_DonViQuiDoi = g2.Key.ID_DonViQuiDoi,
        //                       SoLuongXuat1 = 0,
        //                       SoLuongNhap1 = g2.Sum(x => (double?)x.bhdct.SoLuong * x.dvqd.TyLeChuyenDoi ?? 0),
        //                   };
        //        var tbl4 = from bhdct in db.BH_HoaDon_ChiTiet
        //                   join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
        //                   join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
        //                   join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                   where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
        //                   (bhd.ID_CheckIn != null & bhd.ID_CheckIn == ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4")
        //                   & dvqd.Xoa == null & dvqd.MaHangHoa.Contains(maHH)
        //                   group new { bhd, bhdct, dvqd } by new
        //                   {
        //                       bhdct.ID_DonViQuiDoi
        //                   } into g3
        //                   select new
        //                   {
        //                       ID_DonViQuiDoi = g3.Key.ID_DonViQuiDoi,
        //                       SoLuongXuatchuyen1 = 0,
        //                       SoLuongNhapchuyen1 = g3.Sum(x => (double?)x.bhdct.TienChietKhau * x.dvqd.TyLeChuyenDoi ?? 0),
        //                   };

        //        var tbl_fomat = from tb in tbl_HH
        //                        join dvqd in db.DonViQuiDois on tb.ID equals dvqd.ID
        //                        join tb1 in tbl1 on tb.ID equals tb1.ID_DonViQuiDoi into t
        //                        from b1 in t.DefaultIfEmpty()
        //                        join tb2 in tbl2 on tb.ID equals tb2.ID_DonViQuiDoi into t2
        //                        from b2 in t2.DefaultIfEmpty()
        //                        join tb3 in tbl3 on tb.ID equals tb3.ID_DonViQuiDoi into t3
        //                        from b3 in t3.DefaultIfEmpty()
        //                        join tb4 in tbl4 on tb.ID equals tb4.ID_DonViQuiDoi into t4
        //                        from b4 in t4.DefaultIfEmpty()
        //                        join cs in db.ChotSo_HangHoa on dvqd.ID_HangHoa equals cs.ID_HangHoa into t5
        //                        from b5 in t5.DefaultIfEmpty()
        //                        select new
        //                        {
        //                            MaHangHoa = tb.MaHangHoa,
        //                            ID_DonViQuiDoi = tb.ID,
        //                            TenHangHoa = tb.TenHangHoa,
        //                            TenDonViTinh = dvqd.TenDonViTinh,
        //                            GiaVon = (double?)tb.GiaVon ?? 0,
        //                            GiaBan = (double?)dvqd.GiaBan ?? 0,
        //                            TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
        //                            SoLuongXuat = (double?)b1.SoLuongXuat ?? 0, //=0
        //                            SoLuongXuatChuyen = (double?)b2.SoLuongXuatChuyen ?? 0,
        //                            SoLuongNhap = (double?)b3.SoLuongNhap1 ?? 0,
        //                            SoLuongNhapChuyen = (double?)b4.SoLuongNhapchuyen1 ?? 0,
        //                            TonKho = (double?)b5.TonKho ?? 0
        //                        };
        //        var tbl_gop = tbl_fomat.AsEnumerable().Select(t => new Report_HangHoa_XuatNhapTon_Union
        //        {
        //            ID_DonViQuiDoi = t.ID_DonViQuiDoi,
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenDonViTinh = t.TenDonViTinh,
        //            GiaVon = Math.Round(t.GiaVon, 0, MidpointRounding.ToEven),
        //            GiaBan = Math.Round(t.GiaBan, 0, MidpointRounding.ToEven),
        //            TonCuoiKy = Math.Round((t.SoLuongNhap + t.SoLuongNhapChuyen - t.SoLuongXuat - t.SoLuongXuatChuyen) + t.TonKho / t.TyLeChuyenDoi, 3, MidpointRounding.ToEven)
        //        });
        //        try
        //        {
        //            lst = tbl_gop.ToList();
        //        }
        //        catch
        //        {

        //        }
        //    //}
        //    return lst;
        //}

        public static List<Report_HangHoa_XuatNhapTonChiTiet> TinhGiaTriXuatNhapTonChiTiet(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon _classBHHD = new ClassBH_HoaDon(db);
                List<Report_HangHoa_XuatNhapTonChiTiet> lst = new List<Report_HangHoa_XuatNhapTonChiTiet>();
                if (laHangHoa != 3)
                {
                    var tbl = from hd in db.BH_HoaDon
                              join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                              join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                              join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                              where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                              orderby dvqd.MaHangHoa descending
                              group new { dvqd, hd, bhct } by new
                              {
                                  dvqd.ID_HangHoa,
                                  hh.LaHangHoa,
                                  hh.ID_NhomHang,
                                  hh.TenHangHoa
                              } into g
                              select new
                              {
                                  ID_HangHoa = g.Key.ID_HangHoa,
                                  LaHangHoa = g.Key.LaHangHoa,
                                  ID_NhomHang = g.Key.ID_NhomHang,
                                  TenHangHoa = g.Key.TenHangHoa,
                                  MaHangHoa = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? "" :
                                  g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.MaHangHoa,
                                  GiaVon = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? 0 :
                                  g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.GiaVon
                              };
                    if (laHangHoa == 0)
                    {
                        tbl = tbl.Where(x => x.LaHangHoa == false);
                    }
                    else if (laHangHoa == 1)
                    {
                        tbl = tbl.Where(x => x.LaHangHoa == true);
                    }
                    if (ID_NhomHang != null)
                    {
                        tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
                    }
                    var tbl1 = tbl.AsEnumerable().Select(t => new
                    {
                        ID_HangHoa = t.ID_HangHoa,
                        TenHangHoa = t.TenHangHoa,
                        TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                        TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                        MaHangHoa = t.MaHangHoa,
                        GiaVon = t.GiaVon,
                        lstTinh = SumSoLuongXuatNhanTonCT_ChiTiet(t.ID_HangHoa, timeStart, timeEnd, ID_ChiNhanh),
                        lstTonKho = _classBHHD.TinhSLTonHHKK(t.ID_HangHoa, ID_ChiNhanh, timeStart),
                    });
                    if (maHH != null & maHH != "" & maHH != "null")
                    {
                        maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                        tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
                    }

                    var tbl_gop = from tb in tbl1
                                  group tb by new
                                  {

                                  } into g
                                  select new Report_HangHoa_XuatNhapTonChiTiet
                                  {
                                      TonDauKy = Math.Round(g.Sum(x => (double?)x.lstTonKho ?? 0), 3, MidpointRounding.ToEven),
                                      GiaTriDauKy = Math.Round(g.Sum(x => (double?)x.lstTonKho * x.GiaVon ?? 0), 3, MidpointRounding.ToEven),
                                      NCCNhap = Math.Round(g.Sum(x => x.lstTinh[0].NCCNhap), 3, MidpointRounding.ToEven),
                                      KiemNhap = Math.Round(g.Sum(x => x.lstTinh[0].KiemNhap), 3, MidpointRounding.ToEven),
                                      TraNhap = Math.Round(g.Sum(x => x.lstTinh[0].TraNhap), 3, MidpointRounding.ToEven),
                                      ChuyenNhap = Math.Round(g.Sum(x => x.lstTinh[0].ChuyenNhap), 3, MidpointRounding.ToEven),
                                      SxNhap = 0,
                                      BanXuat = Math.Round(g.Sum(x => x.lstTinh[0].BanXuat), 3, MidpointRounding.ToEven),
                                      HuyXuat = Math.Round(g.Sum(x => x.lstTinh[0].HuyXuat), 3, MidpointRounding.ToEven),
                                      NCCXuat = Math.Round(g.Sum(x => x.lstTinh[0].NCCXuat), 3, MidpointRounding.ToEven),
                                      KiemXuat = Math.Round(g.Sum(x => x.lstTinh[0].KiemXuat), 3, MidpointRounding.ToEven),
                                      ChuyenXuat = Math.Round(g.Sum(x => x.lstTinh[0].ChuyenXuat), 3, MidpointRounding.ToEven),
                                      SxXuat = 0,
                                      TonCuoiKy = Math.Round(g.Sum(x => (x.lstTonKho.Value + x.lstTinh[0].NCCNhap + x.lstTinh[0].KiemNhap + x.lstTinh[0].TraNhap + x.lstTinh[0].ChuyenNhap - x.lstTinh[0].BanXuat - x.lstTinh[0].HuyXuat - x.lstTinh[0].NCCXuat - x.lstTinh[0].KiemXuat - x.lstTinh[0].ChuyenXuat)), 3, MidpointRounding.ToEven),
                                      GiaTriCuoiKy = Math.Round(g.Sum(x => (x.lstTonKho.Value + x.lstTinh[0].NCCNhap + x.lstTinh[0].KiemNhap + x.lstTinh[0].TraNhap + x.lstTinh[0].ChuyenNhap - x.lstTinh[0].BanXuat - x.lstTinh[0].HuyXuat - x.lstTinh[0].NCCXuat - x.lstTinh[0].KiemXuat - x.lstTinh[0].ChuyenXuat) * x.GiaVon), 3, MidpointRounding.ToEven)
                                  };
                    try
                    {
                        lst = tbl_gop.ToList();
                    }
                    catch { }
                }
                return lst;
            }
        }
        //public static List<Report_HangHoa_XuatNhapTonChiTiet> getListReportHH_XuatNhapTonChiTiet(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_XuatNhapTonChiTiet> lst = new List<Report_HangHoa_XuatNhapTonChiTiet>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
        //                  orderby dvqd.MaHangHoa descending
        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.ID_HangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang,
        //                      hh.TenHangHoa
        //                  } into g
        //                  select new
        //                  {
        //                      ID_HangHoa = g.Key.ID_HangHoa,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      MaHangHoa = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? "" :
        //                      g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.MaHangHoa,
        //                      GiaVon = g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault() == null ? 0 :
        //                      g.Where(x => x.dvqd.LaDonViChuan == true).FirstOrDefault().dvqd.GiaVon
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_XuatNhapTonChiTiet
        //        {
        //            ID_HangHoa = t.ID_HangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //            MaHangHoa = t.MaHangHoa,
        //            GiaVon = t.GiaVon
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        if (tbl1 != null)
        //            return tbl1.ToList();
        //        else
        //            return lst;
        //    }
        //    else
        //        return lst;
        //}
        //public static List<Report_HangHoa_XuatHuy> getListReportHH_XuatHuy(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_XuatHuy> lst = new List<Report_HangHoa_XuatHuy>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where (hd.LoaiHoaDon == 8) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang
        //                  } into g
        //                  select new
        //                  {
        //                      MaHangHoa = g.Key.MaHangHoa,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      SoLuongHuy = (double?)g.Sum(x => x.bhct.SoLuong) ?? 0,
        //                      GiaTriHuy = (double?)g.Sum(x => x.bhct.ThanhTien) ?? 0,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang,
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_XuatHuy
        //        {
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //            TongSoLuongHuy = Math.Round(t.SoLuongHuy, 2, MidpointRounding.ToEven),
        //            TongGiaTriHuy = Math.Round(t.GiaTriHuy, 2, MidpointRounding.ToEven)

        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        tbl1 = tbl1.OrderByDescending(x => x.MaHangHoa);
        //        try
        //        {
        //            lst = tbl1.ToList();
        //        }
        //        catch { }
        //    }
        //    return lst;

        //}
        public static List<Report_HangHoa_NhanVien> TinhGiaTriHH_NhanVien(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_HangHoa_NhanVien> lst = new List<Report_HangHoa_NhanVien>();
            if (laHangHoa != 3)
            {
                var tbl = from hd in db.BH_HoaDon
                          join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                          where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

                          group new { dvqd, hd, bhct } by new
                          {
                              dvqd.MaHangHoa,
                              hh.TenHangHoa,
                              hh.LaHangHoa,
                              hh.ID_NhomHang
                          } into g
                          select new
                          {
                              MaHangHoa = g.Key.MaHangHoa,
                              TenHangHoa = g.Key.TenHangHoa,
                              LaHangHoa = g.Key.LaHangHoa,
                              ID_NhomHang = g.Key.ID_NhomHang,
                          };
                if (laHangHoa == 0)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == false);
                }
                else if (laHangHoa == 1)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == true);
                }
                if (ID_NhomHang != null)
                {
                    tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
                }
                var tbl1 = tbl.AsEnumerable().Select(t => new
                {
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenHangHoa,
                    TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                    TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                    LstTinh = SumSoLuongHH_NhanVien(t.MaHangHoa, timeStart, timeEnd, ID_ChiNhanh),
                    LstSoLuong = SoLuongNhanVien(t.MaHangHoa, timeStart, timeEnd, ID_ChiNhanh)
                });
                if (maHH != null & maHH != "" & maHH != "null")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
                }
                var tbl_gop = from tb in tbl1
                              group tb by new
                              {

                              } into g
                              select new Report_HangHoa_NhanVien
                              {
                                  SoluongNhanVien = g.Sum(x => x.LstSoLuong),
                                  SoLuongBan = g.Sum(x => x.LstTinh[0].SoLuongBan),
                                  GiaTri = g.Sum(x => x.LstTinh[0].GiaTri),
                              };
                try
                {
                    lst = tbl_gop.ToList();
                }
                catch
                {

                }
            }
            return lst;

        }
        //public static List<Report_HangHoa_NhanVien> getListReportHH_NhanVien(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_NhanVien> lst = new List<Report_HangHoa_NhanVien>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang
        //                  } into g
        //                  select new
        //                  {
        //                      MaHangHoa = g.Key.MaHangHoa,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang,
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_NhanVien
        //        {
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        tbl1 = tbl1.OrderByDescending(x => x.MaHangHoa);
        //        try
        //        {
        //            lst = tbl1.ToList();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    return lst;

        //}
        //public static List<Report_HangHoa_KhachHang> getListReportHH_KhachHang(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_KhachHang> lst = new List<Report_HangHoa_KhachHang>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang
        //                  } into g
        //                  select new
        //                  {
        //                      MaHangHoa = g.Key.MaHangHoa,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      SoLuongKhachHang = g.Count(),
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang,
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_KhachHang
        //        {
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        tbl1 = tbl1.OrderByDescending(x => x.MaHangHoa);
        //        try
        //        {
        //            lst = tbl1.ToList();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    return lst;
        //}
        public static List<Report_HangHoa_KhachHang> TinhGiaTriHH_KhachHang(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_HangHoa_KhachHang> lst = new List<Report_HangHoa_KhachHang>();
            if (laHangHoa != 3)
            {
                var tbl = from hd in db.BH_HoaDon
                          join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                          where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

                          group new { dvqd, hd, bhct } by new
                          {
                              dvqd.MaHangHoa,
                              hh.TenHangHoa,
                              hh.LaHangHoa,
                              hh.ID_NhomHang
                          } into g
                          select new
                          {
                              MaHangHoa = g.Key.MaHangHoa,
                              TenHangHoa = g.Key.TenHangHoa,
                              SoLuongKhachHang = g.Count(),
                              LaHangHoa = g.Key.LaHangHoa,
                              ID_NhomHang = g.Key.ID_NhomHang,
                          };
                if (laHangHoa == 0)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == false);
                }
                else if (laHangHoa == 1)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == true);
                }
                if (ID_NhomHang != null)
                {
                    tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
                }
                var tbl1 = tbl.AsEnumerable().Select(t => new
                {
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenHangHoa,
                    TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                    TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                    SoluongKhachHang = SoLuongKhachHang(t.MaHangHoa, timeStart, timeEnd, ID_ChiNhanh),
                    lstGiaTri = SumSoLuongHH_NhanVien(t.MaHangHoa, timeStart, timeEnd, ID_ChiNhanh)
                });
                if (maHH != null & maHH != "" & maHH != "null")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
                }
                var tbl_Gop = from tb in tbl1
                              group tb by new { } into g
                              select new Report_HangHoa_KhachHang
                              {
                                  SoluongKhachHang = g.Sum(x => x.SoluongKhachHang),
                                  SoLuongMua = g.Sum(x => x.lstGiaTri[0].SoLuongBan),
                                  GiaTri = g.Sum(x => x.lstGiaTri[0].GiaTri)
                              };
                try
                {
                    lst = tbl_Gop.ToList();
                }
                catch
                {

                }
            }
            return lst;
        }
        public static List<Report_HangHoa_NhaCungCap> TinhGiaTriHH_NhaCungCap(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_HangHoa_NhaCungCap> lst = new List<Report_HangHoa_NhaCungCap>();
            if (laHangHoa != 3)
            {
                var tbl = from hd in db.BH_HoaDon
                          join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                          where (hd.LoaiHoaDon == 4 /*|| hd.LoaiHoaDon == 9*/) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

                          group new { dvqd, hd, bhct } by new
                          {
                              dvqd.MaHangHoa,
                              hh.TenHangHoa,
                              hh.LaHangHoa,
                              hh.ID_NhomHang
                          } into g
                          select new
                          {
                              MaHangHoa = g.Key.MaHangHoa,
                              TenHangHoa = g.Key.TenHangHoa,
                              SoLuongKhachHang = g.Count(),
                              LaHangHoa = g.Key.LaHangHoa,
                              ID_NhomHang = g.Key.ID_NhomHang,
                          };
                if (laHangHoa == 0)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == false);
                }
                else if (laHangHoa == 1)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == true);
                }
                if (ID_NhomHang != null)
                {
                    tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
                }
                var tbl1 = tbl.AsEnumerable().Select(t => new
                {
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenHangHoa,
                    TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                    TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                    SoLuongNCC = SoLuongNCC(t.MaHangHoa, timeStart, timeEnd, ID_ChiNhanh),
                    lstTinh = SumSoLuongHH_NhaCungCap(t.MaHangHoa, timeStart, timeEnd, ID_ChiNhanh),
                });
                if (maHH != null & maHH != "" & maHH != "null")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
                }
                var tbl_Gop = from tb in tbl1
                              group tb by new { } into g
                              select new Report_HangHoa_NhaCungCap
                              {
                                  SoluongNhaCungCap = g.Sum(x => x.SoLuongNCC),
                                  SoLuongSanPham = g.Sum(x => x.lstTinh[0].SoLuongSanPham),
                                  GiaTri = g.Sum(x => x.lstTinh[0].GiaTri)
                              };
                try
                {
                    lst = tbl_Gop.ToList();
                }
                catch
                {

                }
            }
            return lst;

        }
        //public static List<Report_HangHoa_NhaCungCap> getListReportHH_NhaCungCap(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_HangHoa_NhaCungCap> lst = new List<Report_HangHoa_NhaCungCap>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
        //                  where (hd.LoaiHoaDon == 4 /*|| hd.LoaiHoaDon == 9*/) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh

        //                  group new { dvqd, hd, bhct } by new
        //                  {
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang
        //                  } into g
        //                  select new
        //                  {
        //                      MaHangHoa = g.Key.MaHangHoa,
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      SoLuongKhachHang = g.Count(),
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang,
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangHoa_NhaCungCap
        //        {
        //            MaHangHoa = t.MaHangHoa,
        //            TenHangHoa = t.TenHangHoa,
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        tbl1 = tbl1.OrderByDescending(x => x.MaHangHoa);
        //        try
        //        {
        //            lst = tbl1.ToList();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    return lst;

        //}
        public static string MaHangHoa(Guid ID_HangHoa)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            DonViQuiDoi DM = db.DonViQuiDois.Where(x => x.ID_HangHoa == ID_HangHoa & x.LaDonViChuan == true).FirstOrDefault();
            return DM.MaHangHoa;
        }
        public static List<Report_HangHoa_XuatNhapTon> getListXuatNhanTonCT(Guid ID_HangHoa)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hh in db.DM_HangHoa
                      join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                      where hh.ID == ID_HangHoa && dvqd.LaDonViChuan == true
                      select new
                      {
                          dvqd.MaHangHoa,
                          dvqd.GiaVon
                      };
            List<Report_HangHoa_XuatNhapTon> lst = new List<Report_HangHoa_XuatNhapTon>();
            foreach (var item in tbl)
            {
                Report_HangHoa_XuatNhapTon DM = new Report_HangHoa_XuatNhapTon();
                DM.MaHangHoa = item.MaHangHoa;
                DM.GiaVon = item.GiaVon;
                lst.Add(DM);
            }
            return lst;
        }

        public static List<Report_HangHoa_XuatNhapTon> SumSoLuongXuatNhanTonCT(Guid ID_HangHoa, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      where hh.ID == ID_HangHoa & hd.ChoThanhToan == false & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & (dv.ID == ID_DonVi || hd.ID_CheckIn == ID_DonVi)
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.ID_HangHoa,
                      } into g
                      select new
                      {
                          // hd.ID_CheckIn != null && hd.ID_CheckIn.Value == iddonvi ?
                          SoLuongNhap = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 4 || x.hd.LoaiHoaDon == 9 || x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          SoLuongNhanChuyen = Math.Round((double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value == ID_DonVi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          GiaTriNhap = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 4 || x.hd.LoaiHoaDon == 6 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4")).Sum(x => x.bhct.ThanhTien) ?? 0, 2),
                          GiaTriKiemKe = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 9).Sum(x => x.bhct.SoLuong * x.dvqd.GiaVon) ?? 0, 2),
                          SoLuongXuat = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 5 || x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1") || x.hd.LoaiHoaDon == 7 || x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          SoLuongChuyenXuat = Math.Round((double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value != ID_DonVi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          GiaTriXuat = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2 || (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1") || x.hd.LoaiHoaDon == 7 || x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.ThanhTien) ?? 0, 2)
                      }
                      into abc
                      select new Report_HangHoa_XuatNhapTon
                      {
                          SoLuongNhap = abc.SoLuongNhap + abc.SoLuongNhanChuyen,
                          SoLuongXuat = abc.SoLuongXuat + abc.SoLuongChuyenXuat,
                          GiaTriXuat = abc.GiaTriXuat,
                          GiaTriNhap = abc.GiaTriNhap + abc.GiaTriKiemKe
                      };
            List<Report_HangHoa_XuatNhapTon> lst = new List<Report_HangHoa_XuatNhapTon>();
            try
            {
                lst = tbl.ToList();
            }
            catch { }
            //foreach (var item in tbl)
            //{
            //    Report_HangHoa_XuatNhapTon DM = new Report_HangHoa_XuatNhapTon();
            //    DM.SoLuongNhap = item.SoLuongNhap;
            //    DM.SoLuongXuat = item.SoLuongXuat;
            //    DM.GiaTriXuat = item.GiaTriXuat;
            //    DM.GiaTriNhap = item.GiaTriNhap + item.GiaTriKiemKe;
            //    lst.Add(DM);
            //}
            return lst;
        }

        public static List<Report_HangHoa_XuatNhapTonChiTiet> SumSoLuongXuatNhanTonCT_ChiTiet(Guid ID_HangHoa, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      // where hh.ID == ID_HangHoa & hd.ChoThanhToan == false
                      where hh.ID == ID_HangHoa & hd.ChoThanhToan == false & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & (dv.ID == ID_DonVi || hd.ID_CheckIn == ID_DonVi)
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.ID_HangHoa,
                      } into g
                      select new
                      {

                          NCCNhap = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 4).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          KiemNhap = Math.Round((double?)g.Where(x => (x.hd.LoaiHoaDon == 9 & x.bhct.SoLuong > 0)).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          TraNhap = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          //ChuyenNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "3").Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                          SoLuongNhanChuyen = Math.Round((double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value == ID_DonVi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          //SxNhap = 0,
                          BanXuat = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 1 || x.hd.LoaiHoaDon == 2).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          HuyXuat = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 8).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          NCCXuat = Math.Round((double?)g.Where(x => x.hd.LoaiHoaDon == 7).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          KiemXuat = Math.Round((double?)g.Where(x => (x.hd.LoaiHoaDon == 9 & x.bhct.SoLuong < 0)).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi * (-1)) ?? 0, 2),
                          ChuyenXuat = Math.Round((double?)g.Where(x => (x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "1")).Sum(x => x.bhct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
                          SoLuongChuyenXuat = Math.Round((double?)g.Where(x => x.hd.ID_CheckIn != null & x.hd.ID_CheckIn.Value != ID_DonVi & x.hd.LoaiHoaDon == 10 && x.hd.YeuCau == "4").Sum(x => x.bhct.TienChietKhau * x.dvqd.TyLeChuyenDoi) ?? 0, 2)
                          //SxXuat= 0,
                      };
            List<Report_HangHoa_XuatNhapTonChiTiet> lst = new List<Report_HangHoa_XuatNhapTonChiTiet>();
            foreach (var item in tbl)
            {
                Report_HangHoa_XuatNhapTonChiTiet DM = new Report_HangHoa_XuatNhapTonChiTiet();
                DM.NCCNhap = item.NCCNhap;
                DM.KiemNhap = item.KiemNhap;
                DM.TraNhap = item.TraNhap;
                DM.ChuyenNhap = item.SoLuongNhanChuyen;
                DM.BanXuat = item.BanXuat;
                DM.HuyXuat = item.HuyXuat;
                DM.NCCXuat = item.NCCXuat;
                DM.KiemXuat = item.KiemXuat;
                DM.ChuyenXuat = item.ChuyenXuat + item.SoLuongChuyenXuat;
                lst.Add(DM);
            }
            return lst;
        }
        public static List<Report_HangHoa_NhanVien> SumSoLuongHH_NhanVien(string MaHangHoa, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & dvqd.MaHangHoa == MaHangHoa & hd.ChoThanhToan == false & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & dv.ID == ID_DonVi
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.MaHangHoa,
                      } into g
                      select new
                      {
                          SoLuongBan = (double?)g.Sum(x => x.bhct.SoLuong) ?? 0,
                          GiaTriBan = (double?)g.Sum(x => x.bhct.ThanhTien) ?? 0,
                      };
            List<Report_HangHoa_NhanVien> lst = new List<Report_HangHoa_NhanVien>();
            foreach (var item in tbl)
            {
                Report_HangHoa_NhanVien DM = new Report_HangHoa_NhanVien();
                DM.SoLuongBan = item.SoLuongBan;
                DM.GiaTri = item.GiaTriBan;
                lst.Add(DM);
            }
            return lst;
        }
        public static List<Report_HangHoa_NhaCungCap> SumSoLuongHH_NhaCungCap(string MaHangHoa, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      where (hd.LoaiHoaDon == 4/* || hd.LoaiHoaDon == 9*/) & dvqd.MaHangHoa == MaHangHoa & hd.ChoThanhToan == false & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & dv.ID == ID_DonVi
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.MaHangHoa,
                      } into g
                      select new
                      {
                          SoLuong = (double?)g.Sum(x => x.bhct.SoLuong) ?? 0,
                          GiaTri = (double?)g.Sum(x => x.bhct.ThanhTien) ?? 0,
                      };
            List<Report_HangHoa_NhaCungCap> lst = new List<Report_HangHoa_NhaCungCap>();
            foreach (var item in tbl)
            {
                Report_HangHoa_NhaCungCap DM = new Report_HangHoa_NhaCungCap();
                DM.SoLuongSanPham = item.SoLuong;
                DM.GiaTri = item.GiaTri;
                lst.Add(DM);
            }
            return lst;
        }
        public static int SoLuongNhanVien(string MaHangHoa, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      where dvqd.MaHangHoa == MaHangHoa & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & dv.ID == ID_DonVi
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.MaHangHoa,
                          hd.ID_NhanVien
                      } into g
                      select new
                      {
                          SoLuongNV = g.Count()
                      };
            return tbl.Count();
        }
        public static int SoLuongKhachHang(string MaHangHoa, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      where dvqd.MaHangHoa == MaHangHoa & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & dv.ID == ID_DonVi
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.MaHangHoa,
                          hd.ID_DoiTuong
                      } into g
                      select new
                      {
                          SoLuongNV = g.Count()
                      };
            return tbl.Count();
        }
        public static int SoLuongNCC(string MaHangHoa, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from hd in db.BH_HoaDon
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      where dvqd.MaHangHoa == MaHangHoa & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 4 /*|| hd.LoaiHoaDon == 9*/) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & dv.ID == ID_DonVi
                      group new { dvqd, hd, bhct } by new
                      {
                          dvqd.MaHangHoa,
                          hd.ID_DoiTuong
                      } into g
                      select new
                      {
                          SoLuongNV = g.Count(),
                      };
            return tbl.Count();
        }
        //public static List<LichSuThaoTac> getList_NhatKySuDung(string ID_NhanVien, string NoiDung, string ChucNang, DateTime timeStart, DateTime timeEnd, string thaotac, Guid ID_ChiNhanh)
        //{
        //    string[] mang = ID_NhanVien.Split(',');
        //    List<string> LstIS = new List<string>();
        //    for (int i = 0; i < mang.Length; i++)
        //    {
        //        LstIS.Add(mang[i].ToString());
        //    }
        //    string[] mangTT = thaotac.Split(',');
        //    List<string> LstTT = new List<string>();
        //    for (int j = 0; j < mangTT.Length; j++)
        //    {
        //        LstTT.Add(mangTT[j].ToString());
        //    }
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<LichSuThaoTac> lst = new List<LichSuThaoTac>();
        //    var tbl = from nk in db.HT_NhatKySuDung.Where(x => LstIS.Contains(x.ID_NhanVien.ToString()))
        //              join nv in db.NS_NhanVien on nk.ID_NhanVien equals nv.ID
        //              where nk.ThoiGian >= timeStart & nk.ThoiGian < timeEnd & nk.ID_DonVi == ID_ChiNhanh & LstTT.Contains(nk.LoaiNhatKy.ToString())
        //              select new
        //              {
        //                  TenNhanVien = nv.TenNhanVien,
        //                  ChucNang = nk.ChucNang,
        //                  ThoiGian = nk.ThoiGian,
        //                  NoiDung = nk.NoiDung,
        //                  NoiDungChiTiet = nk.NoiDungChiTiet,
        //                  LoaiNhatKy = nk.LoaiNhatKy
        //              };
        //    var tbl_format = tbl.AsEnumerable().Select(t => new LichSuThaoTac
        //    {
        //        TenNhanVien = t.TenNhanVien,
        //        ChucNang = t.ChucNang,
        //        ChucNang_CV = CommonStatic.ConvertToUnSign(t.ChucNang).ToLower(),
        //        ChucNang_GC = CommonStatic.GetCharsStart(t.ChucNang).ToLower(),
        //        ThoiGian = t.ThoiGian,
        //        NoiDung = t.NoiDung,
        //        NoiDungChiTiet = t.NoiDungChiTiet,
        //        NoiDung_CV = CommonStatic.ConvertToUnSign(t.NoiDung).ToLower(),
        //        NoiDung_GC = CommonStatic.GetCharsStart(t.NoiDung).ToLower()
        //    });
        //    if (ChucNang != null & ChucNang != "null" & ChucNang != "")
        //    {
        //        ChucNang = CommonStatic.ConvertToUnSign(ChucNang).ToLower();
        //        tbl_format = tbl_format.Where(x => x.ChucNang_CV.Contains(@ChucNang) || x.ChucNang_GC.Contains(@ChucNang));
        //    }
        //    if (NoiDung != null & NoiDung != "null" & NoiDung != "")
        //    {
        //        NoiDung = CommonStatic.ConvertToUnSign(NoiDung).ToLower();
        //        tbl_format = tbl_format.Where(x => x.NoiDung_CV.Contains(@NoiDung) || x.NoiDung_GC.Contains(@NoiDung));
        //    }
        //    try
        //    {
        //        lst = tbl_format.OrderByDescending(x => x.ThoiGian).ToList();
        //    }
        //    catch
        //    {

        //    }
        //    return lst;
        //}
        #endregion
        #region báo cáo bán hàng
        //public static List<BC_BH_HoaDonDTO> GetBaoCao_BanHang(string MaHoaDon, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into vv
        //                  from nn in vv.DefaultIfEmpty()
        //                  where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6)
        //                  select new BC_BH_HoaDonDTO
        //                  {
        //                      MaHoaDon = hd.MaHoaDon,
        //                      NgayLapHoaDon = hd.NgayLapHoaDon,
        //                      TenHangHoa = hh.TenHangHoa,
        //                      SoLuong = (double?)hdct.SoLuong ?? 0,
        //                      GiaBan = (double?)hdct.DonGia ?? 0 - (double?)hdct.TienChietKhau ?? 0,
        //                      GiaVon = (double?)hdct.GiaVon ?? 0,
        //                      TenNhanVien = nn.TenNhanVien,
        //                      TenDonViTinh = dvqd.TenDonViTinh,
        //                      LaHangHoa = hh.LaHangHoa,
        //                      ID_NhomHang = hh.ID_NhomHang,
        //                      ThanhTien = hdct.ThanhTien
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_NHH = new List<Report_NhomHangHoa>();
        //            lst_NHH = Class_Report.getList_ID_NhomHangHoa(lst_NHH, ID_NhomHang);
        //            List<string> lstID = new List<string>();
        //            foreach (var item in lst_NHH)
        //            {
        //                lstID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lstID.Contains(x.ID_NhomHang.ToString()));

        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }
        //        if (MaHoaDon != null & MaHoaDon != "null" & MaHoaDon != "")
        //            tbl = tbl.Where(x => x.MaHoaDon.Contains(@MaHoaDon));
        //        try
        //        {
        //            lst = tbl.OrderByDescending(x => x.MaHoaDon).OrderByDescending(x => x.NgayLapHoaDon).ToList();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    return lst;

        //}
        public static List<BC_BH_HoaDonDTO> GetMoneyBanHang(string MaHoaDon, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            // DateTime a = 
            var tbl = from hd in db.BH_HoaDon
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into vv
                      from nn in vv.DefaultIfEmpty()
                      where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6)
                      group hdct by new
                      {
                          hh.LaHangHoa,
                          hh.ID_NhomHang
                      } into g
                      select new
                      {
                          ThanhTien = g.Sum(x => (x.ThanhTien)),
                          TienVon = g.Sum(x => (x.SoLuong * x.GiaVon)),
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.ThanhTien = item.ThanhTien;
                bH_HoaDonDTO.TienVon = item.TienVon;
                bH_HoaDonDTO.LaiLo = item.ThanhTien - item.TienVon;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        #endregion
        #region báo cáo nhân viên bán hàng
        //public static List<Report_HangBan_NhanVien> GetBaoCaoHangBan_NhanVien(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh, string ID_NhanVien)
        //{
        //    string[] mang = ID_NhanVien.Split(',');
        //    List<string> LstIS = new List<string>();
        //    for (int i = 0; i < mang.Length; i++)
        //    {
        //        LstIS.Add(mang[i].ToString());
        //    }
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    // DateTime a = 
        //    List<Report_HangBan_NhanVien> lst = new List<Report_HangBan_NhanVien>();
        //    if (laHangHoa != 3)
        //    {
        //        var tbl = from hd in db.BH_HoaDon
        //                  join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
        //                  join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
        //                  join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
        //                  join nv in db.NS_NhanVien.Where(x => LstIS.Contains(x.ID.ToString())) on hd.ID_NhanVien equals nv.ID
        //                  where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2)
        //                  // orderby hd.NgayLapHoaDon, hd.MaHoaDon descending
        //                  group new { hd, hdct, nv, dvqd } by new
        //                  {
        //                      nv.ID,
        //                      nv.TenNhanVien,
        //                      hh.LaHangHoa,
        //                      hh.ID_NhomHang,
        //                      dvqd.MaHangHoa,
        //                      hh.TenHangHoa
        //                  } into g
        //                  select new
        //                  {
        //                      TenNhanVien = g.Key.TenNhanVien,
        //                      ID_NhanVien = g.Key.ID,
        //                      LaHangHoa = g.Key.LaHangHoa,
        //                      ID_NhomHang = g.Key.ID_NhomHang,
        //                      SoLuong = Math.Round((double?)g.Sum(x => x.hdct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0, 2),
        //                      GiaTri = Math.Round((double?)g.Sum(x => x.hd.TongTienHang) ?? 0, 2),
        //                      TenHangHoa = g.Key.TenHangHoa,
        //                      MaHangHoa = g.Key.MaHangHoa
        //                  };
        //        if (laHangHoa == 0)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == false);
        //        }
        //        else if (laHangHoa == 1)
        //        {
        //            tbl = tbl.Where(x => x.LaHangHoa == true);
        //        }
        //        if (ID_NhomHang != null)
        //        {
        //            List<Report_NhomHangHoa> lst_nh = new List<Report_NhomHangHoa>();
        //            lst_nh = Class_Report.getList_ID_NhomHangHoa(lst_nh, ID_NhomHang);
        //            List<String> lst_ID = new List<string>();
        //            foreach (var item in lst_nh)
        //            {
        //                lst_ID.Add(item.ID_NhomHangHoa.ToString());
        //            }
        //            tbl = tbl.Where(x => lst_ID.Contains(x.ID_NhomHang.ToString()));
        //            //tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
        //        }

        //        var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangBan_NhanVien
        //        {
        //            ID_NhanVien = t.ID_NhanVien,
        //            TenNhanVien = t.TenNhanVien,
        //            LaHangHoa = t.LaHangHoa,
        //            MaHangHoa = t.MaHangHoa,
        //            // DM.TenHangHoa = item.TenHangHoa;
        //            TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
        //            TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
        //            GiaTri = Math.Round(t.GiaTri, 3, MidpointRounding.ToEven),
        //            SoLuong = Math.Round(t.SoLuong, 3, MidpointRounding.ToEven)
        //        });
        //        if (maHH != null & maHH != "" & maHH != "null")
        //        {
        //            maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
        //            tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
        //        }
        //        // lst = lst.OrderByDescending(x => x.TenNhanVien).ToList();
        //        var tbl2 = from m in tbl1
        //                   group m by new
        //                   {
        //                       m.ID_NhanVien,
        //                       m.TenNhanVien
        //                   } into g
        //                   select new Report_HangBan_NhanVien
        //                   {
        //                       ID_NhanVien = g.Key.ID_NhanVien,
        //                       TenNhanVien = g.Key.TenNhanVien,
        //                       GiaTri = (double?)g.Sum(x => x.GiaTri) ?? 0,
        //                       SoLuong = (double?)g.Sum(x => x.SoLuong) ?? 0
        //                   };
        //        try
        //        {
        //            lst = tbl2.ToList();
        //        }
        //        catch { }
        //    }

        //    return lst;

        //}
        public static List<Report_HangBan_NhanVien> GetBaoCaoHangBanChiTiet_NhanVien(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh, Guid ID_NhanVien)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            // DateTime a = 
            List<Report_HangBan_NhanVien> lst = new List<Report_HangBan_NhanVien>();
            if (laHangHoa != 3)
            {
                var tbl = from hd in db.BH_HoaDon
                          join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          join nv in db.NS_NhanVien.Where(x => x.ID == ID_NhanVien) on hd.ID_NhanVien equals nv.ID
                          where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2)
                          // orderby hd.NgayLapHoaDon, hd.MaHoaDon descending
                          group new { hd, hdct, nv, dvqd } by new
                          {
                              nv.ID,
                              nv.TenNhanVien,
                              hh.LaHangHoa,
                              hh.ID_NhomHang,
                              dvqd.MaHangHoa,
                              hh.TenHangHoa

                          } into g
                          select new
                          {
                              LaHangHoa = g.Key.LaHangHoa,
                              ID_NhomHang = g.Key.ID_NhomHang,
                              SoLuong = (double?)g.Sum(x => x.hdct.SoLuong * x.dvqd.TyLeChuyenDoi) ?? 0,
                              GiaTri = (double?)g.Sum(x => x.hd.TongTienHang) ?? 0,
                              TenHangHoa = g.Key.TenHangHoa,
                              TenDonViTinh = g.FirstOrDefault().dvqd.TenDonViTinh,
                              MaHangHoa = g.Key.MaHangHoa
                          };
                if (laHangHoa == 0)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == false);
                }
                else if (laHangHoa == 1)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == true);
                }
                if (ID_NhomHang != null)
                {

                    tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
                }

                var tbl1 = tbl.AsEnumerable().Select(t => new Report_HangBan_NhanVien
                {
                    LaHangHoa = t.LaHangHoa,
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenDonViTinh != "" ? t.TenHangHoa + " (" + t.TenDonViTinh + ")" : t.TenHangHoa,
                    TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                    TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                    GiaTri = t.GiaTri,
                    SoLuong = t.SoLuong
                });
                if (maHH != null & maHH != "" & maHH != "null")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
                }
                try
                {
                    lst = tbl1.ToList();
                }
                catch { }
            }

            return lst;

        }

        public static List<Report_NhanVien_BanHang> GetBaoCaoNhanVien_BanHang(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh, string ID_NhanVien)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            // DateTime a = 
            List<Report_NhanVien_BanHang> lst = new List<Report_NhanVien_BanHang>();
            var tbl_BanHang = from nv in db.NS_NhanVien.Where(x => LstIS.Contains(x.ID.ToString()))
                              join hdb in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2) on nv.ID equals hdb.ID_NhanVien
                              where (hdb.NgayLapHoaDon >= timeStart & hdb.NgayLapHoaDon < timeEnd) & hdb.ID_DonVi == ID_ChiNhanh & hdb.ChoThanhToan == false
                              group new { hdb, nv } by new
                              {
                                  nv.ID,
                                  nv.TenNhanVien,
                              } into g
                              select new
                              {
                                  TenNhanVien = g.Key.TenNhanVien,
                                  ID_NhanVien = g.Key.ID,
                                  DoanhThu = (double?)g.Sum(x => (double?)x.hdb.PhaiThanhToan ?? 0) ?? 0,
                              };
            var tbl_ChiTiet_TraHang = from nv in db.NS_NhanVien.Where(x => LstIS.Contains(x.ID.ToString()))
                                      join hdb in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2) on nv.ID equals hdb.ID_NhanVien
                                      join hdt in db.BH_HoaDon on hdb.ID equals hdt.ID_HoaDon
                                      join ctth in db.BH_HoaDon_ChiTiet on hdt.ID equals ctth.ID_HoaDon
                                      where (hdt.NgayLapHoaDon >= timeStart & hdt.NgayLapHoaDon < timeEnd) & hdb.ID_DonVi == ID_ChiNhanh & hdb.ChoThanhToan == false
                                      group new { hdb, ctth, nv } by new
                                      {
                                          nv.ID,
                                          nv.TenNhanVien,
                                      } into g
                                      select new
                                      {
                                          TenNhanVien = g.Key.TenNhanVien,
                                          ID_NhanVien = g.Key.ID,
                                          Giatritra = (double?)g.Sum(x => (double?)x.ctth.ThanhTien ?? 0) ?? 0,
                                      };
            var tbl_Gop = from bh in tbl_BanHang
                          join ctth in tbl_ChiTiet_TraHang on bh.ID_NhanVien equals ctth.ID_NhanVien into hd
                          from hdct in hd.DefaultIfEmpty()
                          group new { hdct, bh } by new
                          {
                              bh.ID_NhanVien,
                              bh.TenNhanVien,
                              bh.DoanhThu,
                              hdct.Giatritra,
                          } into g
                          select new Report_NhanVien_BanHang
                          {
                              TenNhanVien = g.Key.TenNhanVien,
                              DoanhThu = (double?)g.Key.DoanhThu ?? 0,
                              GiaTriTra = (double?)g.Key.Giatritra * (-1) ?? 0,
                              DoanhThuThuan = (g.Key.DoanhThu > 0 ? g.Key.DoanhThu : 0) - (g.Key.Giatritra > 0 ? g.Key.Giatritra : 0),
                          };
            var tbl_fomat = tbl_Gop.AsEnumerable().Select(t => new Report_NhanVien_BanHang
            {
                TenNhanVien = t.TenNhanVien,
                DoanhThu = Math.Round(t.DoanhThu, 3, MidpointRounding.ToEven),
                GiaTriTra = Math.Round(t.GiaTriTra, 3, MidpointRounding.ToEven),
                DoanhThuThuan = Math.Round(t.DoanhThuThuan, 3, MidpointRounding.ToEven),
            });

            try
            {
                lst = tbl_fomat.ToList();
            }
            catch { }
            return lst;

        }

        public static List<Report_NhanVien_LoiNhuan> GetBaoCaoNhanVien_LoiNhuan(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh, string ID_NhanVien)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_NhanVien_LoiNhuan> lst = new List<Report_NhanVien_LoiNhuan>();
            var tbl_BanHang = from nv in db.NS_NhanVien.Where(x => LstIS.Contains(x.ID.ToString()))
                              join hdb in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2) on nv.ID equals hdb.ID_NhanVien
                              where (hdb.NgayLapHoaDon >= timeStart & hdb.NgayLapHoaDon < timeEnd) & hdb.ID_DonVi == ID_ChiNhanh & hdb.ChoThanhToan == false
                              group new { hdb, nv } by new
                              {
                                  nv.ID,
                                  nv.TenNhanVien,
                              } into g
                              select new
                              {
                                  TenNhanVien = g.Key.TenNhanVien,
                                  ID_NhanVien = g.Key.ID,
                                  DoanhThu = (double?)g.Sum(x => (double?)x.hdb.PhaiThanhToan ?? 0) ?? 0,
                              };
            var tbl_ChiTiet_BanHang = from nv in db.NS_NhanVien.Where(x => LstIS.Contains(x.ID.ToString()))
                                      join hdb in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2) on nv.ID equals hdb.ID_NhanVien
                                      join ctbh in db.BH_HoaDon_ChiTiet on hdb.ID equals ctbh.ID_HoaDon
                                      where (hdb.NgayLapHoaDon >= timeStart & hdb.NgayLapHoaDon < timeEnd) & hdb.ID_DonVi == ID_ChiNhanh & hdb.ChoThanhToan == false
                                      group new { hdb, ctbh, nv } by new
                                      {
                                          nv.ID,
                                          nv.TenNhanVien,
                                      } into g
                                      select new
                                      {
                                          TenNhanVien = g.Key.TenNhanVien,
                                          ID_NhanVien = g.Key.ID,
                                          TongTienHangBan = (double?)g.Sum(x => (double?)x.ctbh.ThanhTien ?? 0) ?? 0,
                                          TongGiaVonBan = (double?)g.Sum(x => (double?)(x.ctbh.SoLuong * x.ctbh.GiaVon) ?? 0) ?? 0,
                                      };
            var tbl_ChiTiet_TraHang = from nv in db.NS_NhanVien.Where(x => LstIS.Contains(x.ID.ToString()))
                                      join hdb in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2) on nv.ID equals hdb.ID_NhanVien
                                      join hdt in db.BH_HoaDon on hdb.ID equals hdt.ID_HoaDon
                                      join ctth in db.BH_HoaDon_ChiTiet on hdt.ID equals ctth.ID_HoaDon
                                      where (hdt.NgayLapHoaDon >= timeStart & hdt.NgayLapHoaDon < timeEnd) & hdb.ID_DonVi == ID_ChiNhanh & hdb.ChoThanhToan == false
                                      group new { hdb, ctth, nv } by new
                                      {
                                          nv.ID,
                                          nv.TenNhanVien,
                                      } into g
                                      select new
                                      {
                                          TenNhanVien = g.Key.TenNhanVien,
                                          ID_NhanVien = g.Key.ID,
                                          Giatritra = (double?)g.Sum(x => (double?)x.ctth.ThanhTien ?? 0) ?? 0,
                                          TongGiaVonTra = (double?)g.Sum(x => (double?)(x.ctth.SoLuong * x.ctth.GiaVon) ?? 0) ?? 0,
                                      };
            var tbl_Gop = from bh in tbl_BanHang
                          join ctbh in tbl_ChiTiet_BanHang on bh.ID_NhanVien equals ctbh.ID_NhanVien
                          join ctth in tbl_ChiTiet_TraHang on ctbh.ID_NhanVien equals ctth.ID_NhanVien into hd
                          from hdct in hd.DefaultIfEmpty()
                          group new { hdct, ctbh, bh } by new
                          {
                              bh.ID_NhanVien,
                              bh.TenNhanVien,
                              bh.DoanhThu,
                              ctbh.TongTienHangBan,
                              ctbh.TongGiaVonBan,
                              hdct.Giatritra,
                              hdct.TongGiaVonTra
                          } into g
                          select new Report_NhanVien_LoiNhuan
                          {
                              TenNhanVien = g.Key.TenNhanVien,
                              TongTienHang = (double?)g.Key.TongTienHangBan ?? 0,
                              GiamGiaHD = (double?)(g.Key.DoanhThu - g.Key.TongTienHangBan) ?? 0,
                              DoanhThu = (double?)g.Key.DoanhThu ?? 0,
                              GiaTriTra = (double?)g.Key.Giatritra * (-1) ?? 0,
                              DoanhThuThuan = (g.Key.DoanhThu > 0 ? g.Key.DoanhThu : 0) - (g.Key.Giatritra > 0 ? g.Key.Giatritra : 0),
                              TongGiaVon = (g.Key.TongGiaVonBan > 0 ? g.Key.TongGiaVonBan : 0) - (g.Key.TongGiaVonTra > 0 ? g.Key.TongGiaVonTra : 0),
                              LoiNhuanGop = (g.Key.DoanhThu > 0 ? g.Key.DoanhThu : 0) - (g.Key.Giatritra > 0 ? g.Key.Giatritra : 0) - (g.Key.TongGiaVonBan > 0 ? g.Key.TongGiaVonBan : 0) + (g.Key.TongGiaVonTra > 0 ? g.Key.TongGiaVonTra : 0)
                          };
            var tbl_fomat = tbl_Gop.AsEnumerable().Select(t => new Report_NhanVien_LoiNhuan
            {
                TenNhanVien = t.TenNhanVien,
                TongTienHang = Math.Round(t.TongTienHang, 3, MidpointRounding.ToEven),
                GiamGiaHD = Math.Round(t.GiamGiaHD, 3, MidpointRounding.ToEven),
                DoanhThu = Math.Round(t.DoanhThu, 3, MidpointRounding.ToEven),
                GiaTriTra = Math.Round(t.GiaTriTra, 3, MidpointRounding.ToEven),
                DoanhThuThuan = Math.Round(t.DoanhThuThuan, 3, MidpointRounding.ToEven),
                TongGiaVon = Math.Round(t.TongGiaVon, 3, MidpointRounding.ToEven),
                LoiNhuanGop = Math.Round(t.LoiNhuanGop, 3, MidpointRounding.ToEven),
            });

            try
            {
                lst = tbl_fomat.OrderByDescending(x => x.LoiNhuanGop).ToList();
            }
            catch { }
            return lst;
        }
        #endregion
        #region báo cáo nhà cung cấp
        public static List<Report_NCC_NhapHang> getListNCC_NhapHang(string maNCC, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_NCC_NhapHang> lst = new List<Report_NCC_NhapHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4 || x.LoaiHoaDon == 7)
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into dthd
                      from hd_dt in dthd.DefaultIfEmpty()
                      where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false
                      group new { hd, hd_dt } by new
                      {
                          hd.ID_DoiTuong
                      } into g
                      select new
                      {
                          ID_NCC = g.Key.ID_DoiTuong,
                          TenNhaCungCap = g.FirstOrDefault().hd_dt.TenDoiTuong,
                          MaNCC = g.FirstOrDefault().hd_dt.MaDoiTuong,
                          GiaTriNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                          GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 7).Sum(x => x.hd.PhaiThanhToan) ?? 0
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new Report_NCC_NhapHang
            {
                ID_NCC = t.ID_NCC,
                MaNCC = t.MaNCC != null ? t.MaNCC : "NCC Lẻ",
                TenNCC = t.TenNhaCungCap != null ? t.TenNhaCungCap : "Nhà cung cấp lẻ",
                TenNCC_CV = CommonStatic.ConvertToUnSign(t.TenNhaCungCap != null ? t.TenNhaCungCap : "Nhà cung cấp lẻ").ToLower(),
                TenNNC_GC = CommonStatic.GetCharsStart(t.TenNhaCungCap != null ? t.TenNhaCungCap : "Nhà cung cấp lẻ").ToLower(),
                GiaTriNhap = Math.Round(t.GiaTriNhap, 3, MidpointRounding.ToEven),
                GiaTriTra = Math.Round(t.GiaTriTra * (-1), 3, MidpointRounding.ToEven),
                GiaTriThuan = Math.Round(t.GiaTriNhap - t.GiaTriTra, 3, MidpointRounding.ToEven)
            });
            if (maNCC != null & maNCC != "null" & maNCC != "")
            {
                maNCC = CommonStatic.ConvertToUnSign(maNCC).ToLower();
                tbl1 = tbl1.Where(x => x.MaNCC.Contains(@maNCC) || x.TenNCC_CV.Contains(@maNCC) || x.TenNNC_GC.Contains(@maNCC));
            }
            tbl1 = tbl1.OrderByDescending(x => x.MaNCC);
            try
            {
                lst = tbl1.ToList();
            }
            catch
            {

            }
            return lst;
        }
        //báo cáo chi tiết
        public static List<Report_NCC_NhapHang> getListNCC_NhapHangChiTiet(Guid? ID_NCC, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {


            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_NCC_NhapHang> lst = new List<Report_NCC_NhapHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4 || x.LoaiHoaDon == 7)
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ID_DoiTuong == ID_NCC & hd.ChoThanhToan == false
                      group new { hd, hdct } by new
                      {
                          hd.MaHoaDon
                      } into g
                      select new Report_NCC_NhapHang
                      {
                          MaPhieu = g.Key.MaHoaDon,
                          NgayLapHoaDon = g.FirstOrDefault().hd.NgayLapHoaDon,
                          SoLuongSanPham = (double?)g.Sum(x => x.hdct.SoLuong) ?? 0,
                          TongGiaTri = g.FirstOrDefault().hd.LoaiHoaDon == 4 ? (double?)g.FirstOrDefault().hd.PhaiThanhToan ?? 0 : (double?)g.FirstOrDefault().hd.PhaiThanhToan * (-1) ?? 0
                      };
            tbl = tbl.OrderByDescending(x => x.NgayLapHoaDon);
            try
            {
                lst = tbl.ToList();
            }
            catch
            {

            }
            return lst;
        }
        //công nợ
        public static List<Report_NCC_CongNo> getListNCC_CongNo(string maNCC, double NoHienTaiFrom, double NoHienTaiTo, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_NCC_CongNo> lst = new List<Report_NCC_CongNo>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4 || x.LoaiHoaDon == 7)
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                      join q_hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals q_hdct.ID_HoaDonLienQuan into qhd
                      from q_hd in qhd.DefaultIfEmpty()
                      where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false
                      group new { hd, q_hd, dt } by new
                      {
                          hd.ID_DoiTuong
                      } into g
                      select new
                      {
                          ID_NCC = g.Key.ID_DoiTuong,
                          MaNCC = g.FirstOrDefault().dt.MaDoiTuong,
                          GiaTriNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                          GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 7).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                          TienThu = (double?)g.Where(x => x.hd.LoaiHoaDon == 7).Sum(x => x.q_hd.TienThu) ?? 0,
                          TienChi = (double?)g.Where(x => x.hd.LoaiHoaDon == 4).Sum(x => x.q_hd.TienThu) ?? 0
                      } into abc
                      select new
                      {
                          ID_NCC = abc.ID_NCC,
                          MaNCC = abc.MaNCC,
                          GhiNo = (double?)abc.GiaTriNhap - abc.GiaTriTra ?? 0,//tại thời điểm phát sinh giao dịch
                          GhiCo = (double?)abc.TienChi - abc.TienThu ?? 0
                      };

            var sq_TrongKhoang = from qhd in db.Quy_HoaDon
                                 join qhdct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qhdct.ID_HoaDon
                                 join dt in db.DM_DoiTuong on qhdct.ID_DoiTuong equals dt.ID
                                 where qhd.NgayLapHoaDon >= timeStart & qhd.NgayLapHoaDon < timeEnd & qhd.ID_DonVi == ID_ChiNhanh & qhd.TrangThai != false & qhdct.ID_HoaDonLienQuan == null
                                 group new { qhd, qhdct, dt } by new
                                 {
                                     qhdct.ID_DoiTuong
                                 } into g
                                 select new
                                 {
                                     ID_NCC = g.Key.ID_DoiTuong,
                                     TenNhaCungCap = g.FirstOrDefault().dt.TenDoiTuong,
                                     MaNCC = g.FirstOrDefault().dt.MaDoiTuong,
                                     TienThu = (double?)g.Where(x => x.qhd.LoaiHoaDon == 11).Sum(x => x.qhd.TongTienThu) ?? 0,
                                     TienChi = (double?)g.Where(x => x.qhd.LoaiHoaDon == 12).Sum(x => x.qhd.TongTienThu) ?? 0
                                 } into abc
                                 select new
                                 {
                                     ID_NCC = abc.ID_NCC,
                                     ThuChiTrongKy = abc.TienChi - abc.TienThu // chênh lệch tại thời điểm hiện tại
                                 };
            var sq_DauKy = from qhd in db.Quy_HoaDon
                           join qhdct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qhdct.ID_HoaDon
                           join dt in db.DM_DoiTuong on qhdct.ID_DoiTuong equals dt.ID
                           where qhd.NgayLapHoaDon < timeStart & qhd.ID_DonVi == ID_ChiNhanh & qhd.TrangThai != false & qhdct.ID_HoaDonLienQuan == null
                           group new { qhd, qhdct, dt } by new
                           {
                               qhdct.ID_DoiTuong
                           } into g
                           select new
                           {
                               ID_NCC = g.Key.ID_DoiTuong,
                               TenNhaCungCap = g.FirstOrDefault().dt.TenDoiTuong,
                               MaNCC = g.FirstOrDefault().dt.MaDoiTuong,
                               TienThu = (double?)g.Where(x => x.qhd.LoaiHoaDon == 11).Sum(x => x.qhd.TongTienThu) ?? 0,
                               TienChi = (double?)g.Where(x => x.qhd.LoaiHoaDon == 12).Sum(x => x.qhd.TongTienThu) ?? 0
                           } into abc
                           select new
                           {
                               ID_NCC = abc.ID_NCC,
                               ThuChiDauKy = abc.TienChi - abc.TienThu // chênh lệch tại thời điểm hiện tại
                           };
            var tbl_DauKy = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4 || x.LoaiHoaDon == 7)
                            join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                            join q_hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals q_hdct.ID_HoaDonLienQuan into qhd
                            from q_hd in qhd.DefaultIfEmpty()
                            where hd.NgayLapHoaDon < timeStart & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false
                            group new { hd, q_hd, dt } by new
                            {
                                hd.ID_DoiTuong
                            } into g
                            select new
                            {
                                ID_NCC = g.Key.ID_DoiTuong,
                                GiaTriNhap = (double?)g.Where(x => x.hd.LoaiHoaDon == 4).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                                GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 7).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                                TienThu = (double?)g.Where(x => x.hd.LoaiHoaDon == 7).Sum(x => x.q_hd.TienThu) ?? 0,
                                TienChi = (double?)g.Where(x => x.hd.LoaiHoaDon == 4).Sum(x => x.q_hd.TienThu) ?? 0
                            } into abc
                            select new
                            {
                                ID_NCC = abc.ID_NCC,
                                NoDauKy = (double?)abc.GiaTriNhap - abc.GiaTriTra - abc.TienChi + abc.TienThu ?? 0 // nợ đầu kỳ
                            };
            var tbl_gop = from dt in db.DM_DoiTuong.Where(x => x.LoaiDoiTuong == 2)
                          join tbdk in tbl_DauKy on dt.ID equals tbdk.ID_NCC into d
                          from dtdk in d.DefaultIfEmpty()
                          join tblcl in sq_TrongKhoang on dt.ID equals tblcl.ID_NCC into l
                          from cltk in l.DefaultIfEmpty()
                          join sqdk in sq_DauKy on dt.ID equals sqdk.ID_NCC into k
                          from cldk in k.DefaultIfEmpty()
                          join hd in tbl on dt.ID equals hd.ID_NCC into h
                          from dthd in h.DefaultIfEmpty()
                          select new
                          {
                              ID_NCC = dt.ID,
                              MaNCC = dt.MaDoiTuong,
                              TenNCC = dt.TenDoiTuong,
                              NoDauKy = (double?)dtdk.NoDauKy ?? 0,
                              GhiNo = (double?)dthd.GhiNo ?? 0,
                              GhiCo = (double?)dthd.GhiCo ?? 0,
                              SoQuy_TK = (double?)cltk.ThuChiTrongKy ?? 0,
                              SoQuy_DK = (double?)cldk.ThuChiDauKy ?? 0
                          };

            var tbl1 = tbl_gop.AsEnumerable().Select(t => new Report_NCC_CongNo
            {
                ID_NCC = t.ID_NCC,
                MaNCC = t.MaNCC,
                TenNCC = t.TenNCC,
                TenNCC_CV = CommonStatic.ConvertToUnSign(t.TenNCC).ToLower(),
                TenNNC_GC = CommonStatic.GetCharsStart(t.TenNCC).ToLower(),
                NoDauKy = Math.Round(t.NoDauKy - t.SoQuy_DK, 3, MidpointRounding.ToEven),
                GhiCo = Math.Round(t.GhiCo + t.SoQuy_TK, 3, MidpointRounding.ToEven),
                GhiNo = Math.Round(t.GhiNo, 3, MidpointRounding.ToEven),
                NoCuoiKy = Math.Round(t.NoDauKy - t.SoQuy_DK + t.GhiNo - t.GhiCo - t.SoQuy_TK, 3, MidpointRounding.ToEven)
            });
            tbl1 = tbl1.Where(x => x.NoDauKy != 0 || x.GhiCo != 0 || x.GhiNo != 0);
            if (maNCC != null & maNCC != "null" & maNCC != "")
            {
                maNCC = CommonStatic.ConvertToUnSign(maNCC).ToLower();
                tbl1 = tbl1.Where(x => x.MaNCC.Contains(@maNCC) || x.TenNCC_CV.Contains(@maNCC) || x.TenNNC_GC.Contains(@maNCC));
            }
            tbl1 = tbl1.OrderByDescending(x => x.MaNCC);
            if (NoHienTaiFrom > 0)
            {
                tbl1 = tbl1.Where(x => x.NoCuoiKy >= NoHienTaiFrom);
            }
            if (NoHienTaiTo > 0)
            {
                tbl1 = tbl1.Where(x => x.NoCuoiKy <= NoHienTaiTo);
            }
            try
            {
                lst = tbl1.ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_NCC_CongNoChiTiet> getListNCC_CongNoChiTiet(Guid? ID_NCC, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_NCC_CongNoChiTiet> lst = new List<Report_NCC_CongNoChiTiet>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4 || x.LoaiHoaDon == 7)
                      join dt in db.DM_DoiTuong.Where(x => x.ID == ID_NCC) on hd.ID_DoiTuong equals dt.ID
                      where hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      select new Report_NCC_CongNoChiTiet
                      {
                          MaHoaDon = hd.MaHoaDon,
                          ThoiGian = hd.NgayLapHoaDon,
                          LoaiPhieu = hd.LoaiHoaDon == 4 ? "Nhập hàng" : "Trả hàng nhập",
                          GiaTri = hd.LoaiHoaDon == 4 ? (double?)hd.PhaiThanhToan ?? 0 : (double?)hd.PhaiThanhToan * (-1) ?? 0
                      };
            var tbl1 = from hd in db.Quy_HoaDon.Where(x => x.LoaiHoaDon == 11 || x.LoaiHoaDon == 12)
                       join hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                       join dt in db.DM_DoiTuong.Where(x => x.ID == ID_NCC) on hdct.ID_DoiTuong equals dt.ID
                       where hd.NgayLapHoaDon < timeEnd & hd.TrangThai != false & hd.ID_DonVi == ID_ChiNhanh
                       select new Report_NCC_CongNoChiTiet
                       {
                           MaHoaDon = hd.MaHoaDon,
                           ThoiGian = hd.NgayLapHoaDon,
                           LoaiPhieu = hd.LoaiHoaDon == 11 ? "Phiếu thu" : "Phiếu chi",
                           GiaTri = hd.LoaiHoaDon == 11 ? (double?)hd.TongTienThu ?? 0 : (double?)hd.TongTienThu * (-1) ?? 0
                       };
            var tb_gop10 = (from tb in tbl
                            select new Report_NCC_CongNoChiTiet
                            {
                                MaHoaDon = tb.MaHoaDon,
                                ThoiGian = tb.ThoiGian,
                                LoaiPhieu = tb.LoaiPhieu,
                                GiaTri = (double?)tb.GiaTri ?? 0
                            }).Union(from tb1 in tbl1
                                     select new Report_NCC_CongNoChiTiet
                                     {
                                         MaHoaDon = tb1.MaHoaDon,
                                         ThoiGian = tb1.ThoiGian,
                                         LoaiPhieu = tb1.LoaiPhieu,
                                         GiaTri = (double?)tb1.GiaTri ?? 0
                                     });
            var tb_gop = tb_gop10.AsEnumerable().Select(t => new Report_NCC_CongNoChiTiet
            {
                MaHoaDon = t.MaHoaDon,
                ThoiGian = (t.LoaiPhieu == "Phiếu thu" || t.LoaiPhieu == "Phiếu chi") ? t.ThoiGian.AddSeconds(2) : t.ThoiGian,
                LoaiPhieu = t.LoaiPhieu,
                GiaTri = Math.Round(t.GiaTri, 3, MidpointRounding.ToEven)
            });
            tb_gop = tb_gop.OrderBy(x => x.ThoiGian);
            double nodau = 0;
            foreach (var item in tb_gop)
            {
                Report_NCC_CongNoChiTiet DM = new Report_NCC_CongNoChiTiet();
                DM.MaHoaDon = item.MaHoaDon;
                DM.ThoiGian = item.ThoiGian;
                DM.LoaiPhieu = item.LoaiPhieu;
                DM.GiaTri = Math.Round(item.GiaTri, 3, MidpointRounding.ToEven);
                DM.DuNoCuoi = Math.Round(item.GiaTri + nodau, 3, MidpointRounding.ToEven);
                lst.Add(DM);
                nodau = item.GiaTri + nodau;
            }
            lst = lst.OrderByDescending(x => x.ThoiGian).ToList();
            return lst;
        }
        public static List<Report_NCC_MuaHang> getListNCC_MuaHang(string maNCC, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_NCC_MuaHang> lst = new List<Report_NCC_MuaHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4)
                          //join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into dthd
                      from hd_dt in dthd.DefaultIfEmpty()
                      where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false
                      group new { hd, hd_dt } by new
                      {
                          hd.ID_DoiTuong
                      } into g
                      select new
                      {
                          ID_NCC = g.Key.ID_DoiTuong,
                          TenNhaCungCap = g.FirstOrDefault().hd_dt.TenDoiTuong,
                          MaNCC = g.FirstOrDefault().hd_dt.MaDoiTuong,
                          GiaTri = (double?)g.Sum(x => x.hd.PhaiThanhToan) ?? 0,
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new Report_NCC_MuaHang
            {
                ID_NCC = t.ID_NCC /*!= null ? t.ID_NCC : null*/,
                MaNCC = t.MaNCC != null ? t.MaNCC : "NCC Lẻ",
                TenNCC = t.TenNhaCungCap != null ? t.TenNhaCungCap : "Nhà cung cấp lẻ",
                TenNCC_CV = CommonStatic.ConvertToUnSign(t.TenNhaCungCap != null ? t.TenNhaCungCap : "Nhà cung cấp lẻ").ToLower(),
                TenNNC_GC = CommonStatic.GetCharsStart(t.TenNhaCungCap != null ? t.TenNhaCungCap : "Nhà cung cấp lẻ").ToLower(),
                GiaTri = Math.Round(t.GiaTri, 3, MidpointRounding.ToEven),
            });

            var tbl2 = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4)
                       join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                       join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into dthd
                       from hd_dt in dthd.DefaultIfEmpty()
                       where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false
                       group new { hd, hd_dt, hdct } by new
                       {
                           hd.ID_DoiTuong
                       } into g
                       select new Report_NCC_MuaHang
                       {
                           ID_NCC = g.Key.ID_DoiTuong,
                           SoLuongSanPham = (double?)g.Sum(x => x.hdct.SoLuong) ?? 0,
                       };

            var tbl3 = (from tb in tbl1
                        select new Report_NCC_MuaHang
                        {
                            ID_NCC = tb.ID_NCC,
                            MaNCC = tb.MaNCC,
                            TenNCC = tb.TenNCC,
                            TenNCC_CV = tb.TenNCC_CV,
                            TenNNC_GC = tb.TenNNC_GC,
                            GiaTri = tb.GiaTri
                        }).Union(from tb2 in tbl2
                                 select new Report_NCC_MuaHang
                                 {
                                     ID_NCC = tb2.ID_NCC,
                                     SoLuongSanPham = (double?)tb2.SoLuongSanPham ?? 0
                                 });
            var tbl_gop = from tb in tbl3
                          group tb by new
                          {
                              tb.ID_NCC
                          } into g
                          select new Report_NCC_MuaHang
                          {
                              ID_NCC = g.Key.ID_NCC,
                              TenNCC = g.FirstOrDefault().TenNCC,
                              MaNCC = g.FirstOrDefault().MaNCC,
                              TenNCC_CV = g.FirstOrDefault().TenNCC_CV,
                              TenNNC_GC = g.FirstOrDefault().TenNNC_GC,
                              SoLuongSanPham = g.Sum(x => (double?)x.SoLuongSanPham ?? 0),
                              GiaTri = g.Sum(x => (double?)x.GiaTri ?? 0)
                          };
            if (maNCC != null & maNCC != "null" & maNCC != "")
            {
                maNCC = CommonStatic.ConvertToUnSign(maNCC).ToLower();
                tbl_gop = tbl_gop.Where(x => x.MaNCC.Contains(@maNCC) || x.TenNCC_CV.Contains(@maNCC) || x.TenNNC_GC.Contains(@maNCC));
            }
            tbl_gop = tbl_gop.OrderByDescending(x => x.MaNCC);
            try
            {
                lst = tbl_gop.ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_NCC_MuaHangChiTiet> getListNCC_MuaHangChiTiet(Guid? ID_NCC, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_NCC_MuaHangChiTiet> lst = new List<Report_NCC_MuaHangChiTiet>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 4)
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.ID_DoiTuong == ID_NCC
                      group new { hd, hdct, hh } by new
                      {
                          dvqd.MaHangHoa
                      } into g
                      select new
                      {
                          MaHangHoa = g.Key.MaHangHoa,
                          TenHangHoa = g.FirstOrDefault().hh.TenHangHoa,
                          SoLuongSanPham = g.Sum(x => (double?)x.hdct.SoLuong ?? 0),
                          GiaTri = g.Sum(x => (double?)x.hdct.ThanhTien ?? 0 * (1 - ((double?)x.hd.TongGiamGia ?? 0 / (double?)x.hd.TongTienHang ?? 0)))
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new Report_NCC_MuaHangChiTiet
            {
                MaHangHoa = t.MaHangHoa,
                TenHangHoa = t.TenHangHoa,
                SoLuongSanPham = t.SoLuongSanPham,
                GiaTri = Math.Round(t.GiaTri, 3, MidpointRounding.ToEven)
            });
            tbl1 = tbl1.OrderByDescending(x => x.MaHangHoa);
            try
            {
                lst = tbl1.ToList();
            }
            catch
            {

            }
            return lst;
        }

        #endregion
        #region báo cáo khách hàng
        public static List<Report_KhachHang_BanHang> GetListKH_BanHang(string maKH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_KhachHang_BanHang> lst = new List<Report_KhachHang_BanHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                      join dt in db.DM_DoiTuong.Where(x => x.LoaiDoiTuong == 1) on hd.ID_DoiTuong equals dt.ID into d
                      from dt_hd in d.DefaultIfEmpty()
                      where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      group new { hd, dt_hd } by new
                      {
                          hd.ID_DoiTuong,
                      } into g
                      select new
                      {
                          ID_KhachHang = g.Key.ID_DoiTuong,
                          MaKhachHang = g.FirstOrDefault().dt_hd.MaDoiTuong == null ? "Khách lẻ" : g.FirstOrDefault().dt_hd.MaDoiTuong,
                          TenKhachHang = g.FirstOrDefault().dt_hd.TenDoiTuong == null ? "Khách lẻ" : g.FirstOrDefault().dt_hd.TenDoiTuong,
                          DoanhThu = (double?)g.Where(x => x.hd.LoaiHoaDon == 1).Sum(x => (double?)x.hd.PhaiThanhToan ?? 0) ?? 0,
                          GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => (double?)x.hd.PhaiThanhToan ?? 0) ?? 0,
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new Report_KhachHang_BanHang
            {
                ID_KhachHang = t.ID_KhachHang,
                MaKhachHang = t.MaKhachHang,
                TenKhachHang = t.TenKhachHang,
                TenKhachHangCV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                TenKhachHangGC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                DoanhThu = Math.Round(t.DoanhThu, 3, MidpointRounding.ToEven),
                GiaTriTra = Math.Round(t.GiaTriTra * (-1), 3, MidpointRounding.ToEven),
                DoanhThuThuan = Math.Round(t.DoanhThu - t.GiaTriTra, 3, MidpointRounding.ToEven)
            });
            if (maKH != null & maKH != "null" & maKH != "")
            {
                maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbl1 = tbl1.Where(x => x.TenKhachHangCV.Contains(@maKH) || x.TenKhachHangGC.Contains(@maKH) || x.MaKhachHang.Contains(@maKH));
            }
            try
            {
                lst = tbl1.OrderByDescending(x => x.DoanhThuThuan).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_KhachHang_BanHangChiTiet> GetListKH_BanHangChiTiet(Guid? ID_KhachHang, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_KhachHang_BanHangChiTiet> lst = new List<Report_KhachHang_BanHangChiTiet>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                      where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.ID_DoiTuong == ID_KhachHang
                      select new Report_KhachHang_BanHangChiTiet
                      {
                          MaHoaDon = hd.MaHoaDon,
                          ThoiGian = hd.NgayLapHoaDon,
                          DoanhThu = hd.LoaiHoaDon == 1 ? (double?)hd.PhaiThanhToan ?? 0 : (double?)hd.PhaiThanhToan * (-1) ?? 0
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new Report_KhachHang_BanHangChiTiet
            {
                MaHoaDon = t.MaHoaDon,
                ThoiGian = t.ThoiGian,
                DoanhThu = Math.Round(t.DoanhThu, 3, MidpointRounding.ToEven)
            });
            try
            {
                lst = tbl1.OrderByDescending(x => x.ThoiGian).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_KhachHang_LoiNhuan> getListKH_LoiNhuan(string maKH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_KhachHang_LoiNhuan> lst = new List<Report_KhachHang_LoiNhuan>();
            var tbl_BanHang = from hdb in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                              join dt in db.DM_DoiTuong.Where(x => x.LoaiDoiTuong == 1) on hdb.ID_DoiTuong equals dt.ID into d
                              from dt_hd in d.DefaultIfEmpty()
                              where (hdb.NgayLapHoaDon >= timeStart & hdb.NgayLapHoaDon < timeEnd) & hdb.ID_DonVi == ID_ChiNhanh & hdb.ChoThanhToan == false
                              group new { hdb, dt_hd } by new
                              {
                                  hdb.ID_DoiTuong,
                              } into g
                              select new
                              {
                                  ID_KhachHang = g.Key.ID_DoiTuong,
                                  MaKhachHang = g.FirstOrDefault().dt_hd.MaDoiTuong == null ? "Khách lẻ" : g.FirstOrDefault().dt_hd.MaDoiTuong,
                                  TenKhachHang = g.FirstOrDefault().dt_hd.TenDoiTuong == null ? "Khách lẻ" : g.FirstOrDefault().dt_hd.TenDoiTuong,
                                  DoanhThu = (double?)g.Where(x => x.hdb.LoaiHoaDon == 1).Sum(x => (double?)x.hdb.PhaiThanhToan ?? 0) ?? 0,
                                  GiaTriTra = (double?)g.Where(x => x.hdb.LoaiHoaDon == 6).Sum(x => (double?)x.hdb.PhaiThanhToan ?? 0) ?? 0,
                                  TongTienHang = (double?)g.Where(x => x.hdb.LoaiHoaDon == 1).Sum(x => (double?)x.hdb.TongTienHang ?? 0) ?? 0
                              };
            var tbl_ChiTiet = from hdb in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                              join ctbh in db.BH_HoaDon_ChiTiet on hdb.ID equals ctbh.ID_HoaDon
                              join dt in db.DM_DoiTuong.Where(x => x.LoaiDoiTuong == 1) on hdb.ID_DoiTuong equals dt.ID into d
                              from dt_hd in d.DefaultIfEmpty()
                              where (hdb.NgayLapHoaDon >= timeStart & hdb.NgayLapHoaDon < timeEnd) & hdb.ID_DonVi == ID_ChiNhanh & hdb.ChoThanhToan == false
                              group new { hdb, ctbh, dt_hd } by new
                              {
                                  hdb.ID_DoiTuong,
                              } into g
                              select new
                              {
                                  ID_KhachHang = g.Key.ID_DoiTuong,
                                  TongGiaVonBan = (double?)g.Where(x => x.hdb.LoaiHoaDon == 1).Sum(x => (double?)(x.ctbh.SoLuong * x.ctbh.GiaVon) ?? 0) ?? 0,
                                  TongGiaVonTra = (double?)g.Where(x => x.hdb.LoaiHoaDon == 6).Sum(x => (double?)(x.ctbh.SoLuong * x.ctbh.GiaVon) ?? 0) ?? 0,
                              };
            var tbl_Gop = from bh in tbl_BanHang
                          join ct in tbl_ChiTiet on bh.ID_KhachHang equals ct.ID_KhachHang into hd
                          from hdct in hd.DefaultIfEmpty()
                          select new
                          {
                              ID_KhachHang = bh.ID_KhachHang,
                              MaKhachHang = bh.MaKhachHang,
                              TenKhachHang = bh.TenKhachHang,
                              TongTienHang = (double?)bh.TongTienHang ?? 0,
                              DoanhThu = (double?)bh.DoanhThu ?? 0,
                              GiaTriTra = (double?)bh.GiaTriTra ?? 0,
                              TongGiaVonBan = (double?)hdct.TongGiaVonBan ?? 0,
                              TongGiaVonTra = (double?)hdct.TongGiaVonTra ?? 0
                          };
            var tbl_fomat = tbl_Gop.AsEnumerable().Select(t => new Report_KhachHang_LoiNhuan
            {
                ID_KhachHang = t.ID_KhachHang,
                TenKhachHang = t.TenKhachHang,
                TenKhachHangCV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                TenKhachHangGC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                MaKhachHang = t.MaKhachHang,
                TongTienHang = Math.Round(t.TongTienHang, 3, MidpointRounding.ToEven),
                DoanhThu = Math.Round(t.DoanhThu, 3, MidpointRounding.ToEven),
                GiamGiaHD = Math.Round((t.TongTienHang - t.DoanhThu) * (-1), 3, MidpointRounding.ToEven),
                GiaTriTra = Math.Round(t.GiaTriTra * (-1), 3, MidpointRounding.ToEven),
                DoanhThuThuan = Math.Round(t.DoanhThu - t.GiaTriTra, 3, MidpointRounding.ToEven),
                TongGiaVon = Math.Round(t.TongGiaVonBan - t.TongGiaVonTra, 3, MidpointRounding.ToEven),
                LoiNhuanGop = Math.Round(t.DoanhThu - t.GiaTriTra - t.TongGiaVonBan + t.TongGiaVonTra, 3, MidpointRounding.ToEven)
            });
            if (maKH != null & maKH != "null" & maKH != "")
            {
                maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbl_fomat = tbl_fomat.Where(x => x.MaKhachHang.Contains(@maKH) || x.TenKhachHangGC.Contains(@maKH) || x.TenKhachHangCV.Contains(@maKH));
            }
            try
            {
                lst = tbl_fomat.OrderByDescending(x => x.LoiNhuanGop).ToList();
            }
            catch { }
            return lst;
        }
        //công nợ
        public static List<Report_KhachHang_CongNo> getListKH_CongNo(string maKH, double NoHienTaiFrom, double NoHienTaiTo, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_KhachHang_CongNo> lst = new List<Report_KhachHang_CongNo>();
            var tbl_TrongKy = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                              join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                              where (hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd) & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false
                              group new { hd, dt } by new
                              {
                                  hd.ID_DoiTuong
                              } into g
                              select new
                              {
                                  ID_NCC = g.Key.ID_DoiTuong,
                                  DoanhThuTrongKy = (double?)g.Where(x => x.hd.LoaiHoaDon == 1).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                                  GiaTriTraTrongKy = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                              };
            var sq_TrongKhoang = from qhd in db.Quy_HoaDon
                                 join qhdct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qhdct.ID_HoaDon
                                 join dt in db.DM_DoiTuong on qhdct.ID_DoiTuong equals dt.ID
                                 where qhd.NgayLapHoaDon >= timeStart & qhd.NgayLapHoaDon < timeEnd & qhd.ID_DonVi == ID_ChiNhanh & qhd.TrangThai != false
                                 group new { qhd, qhdct, dt } by new
                                 {
                                     qhdct.ID_DoiTuong
                                 } into g
                                 select new
                                 {
                                     ID_NCC = g.Key.ID_DoiTuong,
                                     TienThuTrongKy = (double?)g.Where(x => x.qhd.LoaiHoaDon == 11).Sum(x => x.qhd.TongTienThu) ?? 0,
                                     TienChiTrongKy = (double?)g.Where(x => x.qhd.LoaiHoaDon == 12).Sum(x => x.qhd.TongTienThu) ?? 0
                                 };
            var sq_DauKy = from qhd in db.Quy_HoaDon
                           join qhdct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qhdct.ID_HoaDon
                           join dt in db.DM_DoiTuong on qhdct.ID_DoiTuong equals dt.ID
                           where qhd.NgayLapHoaDon < timeStart & qhd.ID_DonVi == ID_ChiNhanh & qhd.TrangThai != false
                           group new { qhd, qhdct, dt } by new
                           {
                               qhdct.ID_DoiTuong
                           } into g
                           select new
                           {
                               ID_NCC = g.Key.ID_DoiTuong,
                               TienThuDauKy = (double?)g.Where(x => x.qhd.LoaiHoaDon == 11).Sum(x => x.qhd.TongTienThu) ?? 0,
                               TienChiDauKy = (double?)g.Where(x => x.qhd.LoaiHoaDon == 12).Sum(x => x.qhd.TongTienThu) ?? 0
                           };
            var tbl_DauKy = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                            join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                            where hd.NgayLapHoaDon < timeStart & hd.ID_DonVi == ID_ChiNhanh & hd.ChoThanhToan == false
                            group new { hd, dt } by new
                            {
                                hd.ID_DoiTuong
                            } into g
                            select new
                            {
                                ID_NCC = g.Key.ID_DoiTuong,
                                DoanhThuDauKy = (double?)g.Where(x => x.hd.LoaiHoaDon == 1).Sum(x => x.hd.PhaiThanhToan) ?? 0,
                                GiaTriTraDauKy = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.hd.PhaiThanhToan) ?? 0
                            };
            var tbl_gop = from dt in db.DM_DoiTuong.Where(x => x.LoaiDoiTuong == 1)
                          join tb_trongky in tbl_TrongKy on dt.ID equals tb_trongky.ID_NCC into d
                          from dttb_TrongKy in d.DefaultIfEmpty()
                          join sq_TrongKy in sq_TrongKhoang on dt.ID equals sq_TrongKy.ID_NCC into l
                          from dtsq_TrongKy in l.DefaultIfEmpty()
                          join tb_dauky in tbl_DauKy on dt.ID equals tb_dauky.ID_NCC into k
                          from dttb_DauKy in k.DefaultIfEmpty()
                          join sq_DKy in sq_DauKy on dt.ID equals sq_DKy.ID_NCC into h
                          from dtsq_DauKy in h.DefaultIfEmpty()
                          select new
                          {
                              ID_NCC = dt.ID,
                              MaNCC = dt.MaDoiTuong,
                              TenNCC = dt.TenDoiTuong,
                              DoanhThuDauKy = (double?)dttb_DauKy.DoanhThuDauKy ?? 0,
                              GiaTriTraDauKy = (double?)dttb_DauKy.GiaTriTraDauKy ?? 0,
                              TienThuDauKy = (double?)dtsq_DauKy.TienThuDauKy ?? 0,
                              TienChiDauKy = (double?)dtsq_DauKy.TienChiDauKy ?? 0,
                              DoanhThuTrongKy = (double?)dttb_TrongKy.DoanhThuTrongKy ?? 0,
                              GiaTriTraTrongKy = (double?)dttb_TrongKy.GiaTriTraTrongKy ?? 0,
                              TienThuTrongKy = (double?)dtsq_TrongKy.TienThuTrongKy ?? 0,
                              TienChiTrongKy = (double?)dtsq_TrongKy.TienChiTrongKy ?? 0
                          };
            var tbl1 = tbl_gop.AsEnumerable().Select(t => new Report_KhachHang_CongNo
            {
                ID_KhachHang = t.ID_NCC,
                MaKhachHang = t.MaNCC,
                TenKhachHang = t.TenNCC,
                TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenNCC).ToLower(),
                TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenNCC).ToLower(),
                NoDauKy = Math.Round(t.DoanhThuDauKy + t.TienChiDauKy - t.GiaTriTraDauKy - t.TienThuDauKy, 3, MidpointRounding.ToEven),
                GhiNo = Math.Round(t.DoanhThuTrongKy + t.TienChiTrongKy, 3, MidpointRounding.ToEven),
                GhiCo = Math.Round(t.GiaTriTraTrongKy + t.TienThuTrongKy, 3, MidpointRounding.ToEven),
                NoCuoiKy = Math.Round(t.DoanhThuDauKy + t.TienChiDauKy - t.GiaTriTraDauKy - t.TienThuDauKy + t.DoanhThuTrongKy + t.TienChiTrongKy - t.GiaTriTraTrongKy - t.TienThuTrongKy, 3, MidpointRounding.ToEven)
            });
            tbl1 = tbl1.Where(x => x.NoDauKy != 0 || x.GhiCo != 0 || x.GhiNo != 0);
            if (maKH != null & maKH != "null" & maKH != "")
            {
                maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbl1 = tbl1.Where(x => x.MaKhachHang.Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH));
            }
            if (NoHienTaiFrom > 0)
            {
                tbl1 = tbl1.Where(x => x.NoCuoiKy >= NoHienTaiFrom);
            }
            if (NoHienTaiTo > 0)
            {
                tbl1 = tbl1.Where(x => x.NoCuoiKy <= NoHienTaiTo);
            }
            try
            {
                lst = tbl1.OrderByDescending(x => x.NoCuoiKy).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_KhachHang_CongNoChiTiet> getListKH_CongNoChiTiet(Guid? ID_KH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_KhachHang_CongNoChiTiet> lst = new List<Report_KhachHang_CongNoChiTiet>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                      join dt in db.DM_DoiTuong.Where(x => x.ID == ID_KH) on hd.ID_DoiTuong equals dt.ID
                      where hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      select new
                      {
                          MaHoaDon = hd.MaHoaDon,
                          ThoiGian = hd.NgayLapHoaDon,
                          LoaiPhieu = hd.LoaiHoaDon == 1 ? "Bán hàng" : "Trả hàng",
                          GiaTri = hd.LoaiHoaDon == 1 ? (double?)hd.PhaiThanhToan ?? 0 : (double?)hd.PhaiThanhToan * (-1) ?? 0
                      };
            var tbl1 = from hd in db.Quy_HoaDon.Where(x => x.LoaiHoaDon == 11 || x.LoaiHoaDon == 12)
                       join hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                       join dt in db.DM_DoiTuong.Where(x => x.ID == ID_KH) on hdct.ID_DoiTuong equals dt.ID
                       where hd.NgayLapHoaDon < timeEnd & hd.TrangThai != false & hd.ID_DonVi == ID_ChiNhanh
                       select new
                       {
                           MaHoaDon = hd.MaHoaDon,
                           ThoiGian = hd.NgayLapHoaDon,
                           LoaiPhieu = hdct.ID_HoaDonLienQuan != null ? "Thanh toán" : hd.LoaiHoaDon == 11 ? "Phiếu thu" : "Phiếu chi",
                           GiaTri = hd.LoaiHoaDon == 12 ? (double?)hd.TongTienThu ?? 0 : (double?)hd.TongTienThu * (-1) ?? 0
                       };
            var tb_gop10 = (from tb in tbl
                            select new Report_KhachHang_CongNoChiTiet
                            {
                                MaHoaDon = tb.MaHoaDon,
                                ThoiGian = tb.ThoiGian,
                                LoaiPhieu = tb.LoaiPhieu,
                                GiaTri = (double?)tb.GiaTri ?? 0
                            }).Union(from tb1 in tbl1
                                     select new Report_KhachHang_CongNoChiTiet
                                     {
                                         MaHoaDon = tb1.MaHoaDon,
                                         ThoiGian = tb1.ThoiGian,
                                         LoaiPhieu = tb1.LoaiPhieu,
                                         GiaTri = (double?)tb1.GiaTri ?? 0
                                     });
            var tb_gop = tb_gop10.AsEnumerable().Select(t => new Report_NCC_CongNoChiTiet
            {
                MaHoaDon = t.MaHoaDon,
                ThoiGian = (t.LoaiPhieu != "Bán hàng" && t.LoaiPhieu != "Trả hàng") ? t.ThoiGian.AddSeconds(2) : t.ThoiGian,
                LoaiPhieu = t.LoaiPhieu,
                GiaTri = Math.Round(t.GiaTri, 3, MidpointRounding.ToEven)
            });
            tb_gop = tb_gop.OrderBy(x => x.ThoiGian);
            double nodau = 0;
            foreach (var item in tb_gop)
            {
                Report_KhachHang_CongNoChiTiet DM = new Report_KhachHang_CongNoChiTiet();
                DM.MaHoaDon = item.MaHoaDon;
                DM.ThoiGian = item.ThoiGian;
                DM.LoaiPhieu = item.LoaiPhieu;
                DM.GiaTri = Math.Round(item.GiaTri, 3, MidpointRounding.ToEven);
                DM.DuNoCuoi = Math.Round(item.GiaTri + nodau, 3, MidpointRounding.ToEven);
                lst.Add(DM);
                nodau = item.GiaTri + nodau;
            }
            lst = lst.OrderByDescending(x => x.ThoiGian).ToList();
            return lst;
        }
        public static List<Report_KhachHang_MuaHang> getListKH_MuaHang(string maKH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_KhachHang_MuaHang> lst = new List<Report_KhachHang_MuaHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                      from dt_hd in d.DefaultIfEmpty()
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      group new { hd, hdct, dt_hd } by new
                      {
                          hd.ID_DoiTuong
                      } into g
                      select new
                      {
                          ID_KhachHang = g.Key.ID_DoiTuong,
                          TenKhachHang = g.FirstOrDefault().dt_hd.TenDoiTuong == null ? "Khách lẻ" : g.FirstOrDefault().dt_hd.TenDoiTuong,
                          MaKhachHang = g.FirstOrDefault().dt_hd.MaDoiTuong == null ? "Khách lẻ" : g.FirstOrDefault().dt_hd.MaDoiTuong,
                          SoLuongMua = (double?)g.Where(x => x.hd.LoaiHoaDon == 1).Sum(x => (double?)x.hdct.SoLuong ?? 0) ?? 0,
                          SoLuongTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => (double?)x.hdct.SoLuong ?? 0) ?? 0
                      };
            var tbl1 = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                       join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                       from dt_hd in d.DefaultIfEmpty()
                       where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                       group new { hd, dt_hd } by new
                       {
                           hd.ID_DoiTuong
                       } into g
                       select new
                       {
                           ID_KhachHang = g.Key.ID_DoiTuong,
                           GiaTriMua = (double?)g.Where(x => x.hd.LoaiHoaDon == 1).Sum(x => (double?)x.hd.PhaiThanhToan ?? 0) ?? 0,
                           GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => (double?)x.hd.PhaiThanhToan ?? 0) ?? 0
                       };
            var tb_gop = from tb in tbl
                         join tb1 in tbl1 on tb.ID_KhachHang equals tb1.ID_KhachHang
                         select new Report_KhachHang_MuaHang
                         {
                             ID_KhachHang = tb.ID_KhachHang,
                             MaKhachHang = tb.MaKhachHang,
                             TenKhachHang = tb.TenKhachHang,
                             SoLuongMua = (double?)tb.SoLuongMua ?? 0,
                             SoLuongTra = (double?)tb.SoLuongTra ?? 0,
                             GiaTriMua = (double?)tb1.GiaTriMua ?? 0,
                             GiaTriTra = (double?)tb1.GiaTriTra ?? 0
                         };
            var tb_format = tb_gop.AsEnumerable().Select(t => new Report_KhachHang_MuaHang
            {
                ID_KhachHang = t.ID_KhachHang,
                MaKhachHang = t.MaKhachHang,
                TenKhachHang = t.TenKhachHang,
                TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                SoLuongMua = Math.Round(t.SoLuongMua, 3, MidpointRounding.ToEven),
                SoLuongTra = Math.Round(t.SoLuongTra, 3, MidpointRounding.ToEven),
                GiaTriMua = Math.Round(t.GiaTriMua, 3, MidpointRounding.ToEven),
                GiaTriTra = Math.Round(t.GiaTriTra * (-1), 3, MidpointRounding.ToEven),
                GiaTriThuan = Math.Round(t.GiaTriMua - t.GiaTriTra, 3, MidpointRounding.ToEven)
            });
            if (maKH != null & maKH != "null" & maKH != "")
            {
                maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tb_format = tb_format.Where(x => x.MaKhachHang.Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH));
            }
            try
            {
                lst = tb_format.OrderByDescending(x => x.GiaTriThuan).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_KhachHang_MuaHangChiTiet> getListKH_MuaHangChiTiet(Guid? ID_KH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_KhachHang_MuaHangChiTiet> lst = new List<Report_KhachHang_MuaHangChiTiet>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.ID_DoiTuong == ID_KH
                      group new { hd, hdct, hh } by new
                      {
                          dvqd.MaHangHoa
                      } into g
                      select new
                      {
                          MaHangHoa = g.Key.MaHangHoa,
                          TenHangHoa = g.FirstOrDefault().hh.TenHangHoa,
                          SoLuongMua = (double?)g.Where(x => x.hd.LoaiHoaDon == 1).Sum(x => (double?)x.hdct.SoLuong ?? 0) ?? 0,
                          SoLuongTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => (double?)x.hdct.SoLuong ?? 0) ?? 0,
                      };
            var tbl_DoanhThu = from hd in db.BH_HoaDon.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 6)
                               join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                               join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                               join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                               where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.ID_DoiTuong == ID_KH
                               group new { hd, hdct, hh } by new
                               {
                                   dvqd.MaHangHoa,
                                   hd.ID,
                               } into g
                               select new
                               {
                                   MaHangHoa = g.Key.MaHangHoa,
                                   GiaTriMua = (double?)g.Where(x => x.hd.LoaiHoaDon == 1).FirstOrDefault().hd.PhaiThanhToan ?? 0,
                                   GiaTriTra = (double?)g.Where(x => x.hd.LoaiHoaDon == 6).FirstOrDefault().hd.PhaiThanhToan ?? 0,
                               } into abc
                               group abc by new
                               {
                                   abc.MaHangHoa
                               } into l
                               select new
                               {
                                   MaHangHoa = l.Key.MaHangHoa,
                                   GiaTriMua = (double?)l.Sum(x => x.GiaTriMua) ?? 0,
                                   GiaTriTra = (double?)l.Sum(x => x.GiaTriTra) ?? 0
                               };
            var tbl_gop = from tb in tbl
                          join tb1 in tbl_DoanhThu on tb.MaHangHoa equals tb1.MaHangHoa
                          select new
                          {
                              MaHangHoa = tb.MaHangHoa,
                              TenHangHoa = tb.TenHangHoa,
                              SoLuongMua = (double?)tb.SoLuongMua ?? 0,
                              GiaTriMua = (double?)tb1.GiaTriMua ?? 0,
                              SoLuongTra = (double?)tb.SoLuongTra ?? 0,
                              GiaTriTra = (double?)tb1.GiaTriTra ?? 0
                          };
            var tbl1 = tbl_gop.AsEnumerable().Select(t => new Report_KhachHang_MuaHangChiTiet
            {
                MaHangHoa = t.MaHangHoa,
                TenHangHoa = t.TenHangHoa,
                SoLuongMua = Math.Round(t.SoLuongMua, 3, MidpointRounding.ToEven),
                GiaTriMua = Math.Round(t.GiaTriMua, 3, MidpointRounding.ToEven),
                SoLuongTra = Math.Round(t.SoLuongTra, 3, MidpointRounding.ToEven),
                GiaTriTra = Math.Round(t.GiaTriTra * (-1), 3, MidpointRounding.ToEven),
                GiaTriThuan = Math.Round(t.GiaTriMua - t.GiaTriTra, 3, MidpointRounding.ToEven)
            });
            try
            {
                lst = tbl1.OrderByDescending(x => x.GiaTriThuan).ToList();
            }
            catch
            {

            }
            return lst;
        }
        #endregion
        #region báo cáo đặt hàng
        public static List<Report_DatHang_HangHoa> getListDatHang_HangHoa(string ID_NhanVien, string maKH, string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_DatHang_HangHoa> lst = new List<Report_DatHang_HangHoa>();
            string[] mang = ID_NhanVien.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            if (laHangHoa != 3)
            {
                var tbl = from hdct in db.BH_HoaDon_ChiTiet
                          join hd in db.BH_HoaDon.Where(x => LstIS.Contains(x.ID_NhanVien.ToString())) on hdct.ID_HoaDon equals hd.ID
                          join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                          from dt_hd in d.DefaultIfEmpty()
                          where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.LoaiHoaDon == 3 & hd.YeuCau != "4"
                          group new { hdct, hd, dvqd, hh } by new
                          {
                              dvqd.MaHangHoa,
                              dt_hd.MaDoiTuong,
                              dt_hd.TenDoiTuong,
                          } into g
                          select new
                          {
                              MaHangHoa = g.Key.MaHangHoa,
                              MaKhachHang = g.Key.MaDoiTuong != null ? g.Key.MaDoiTuong : "Khách lẻ",
                              TenKhachHang = g.Key.TenDoiTuong != null ? g.Key.TenDoiTuong : "Khách lẻ",
                              TenDonViTinh = g.FirstOrDefault().dvqd.TenDonViTinh,
                              ID_NhomHang = g.FirstOrDefault().hh.ID_NhomHang,
                              TenHangHoa = g.FirstOrDefault().hh.TenHangHoa,
                              LaHangHoa = g.FirstOrDefault().hh.LaHangHoa,
                              SoLuongDat = (double?)g.Sum(x => (double?)x.hdct.SoLuong ?? 0) ?? 0,
                              GiaTriDat = (double?)g.Sum(x => (double?)(x.hdct.ThanhTien * (1 - ((double?)(x.hd.TongGiamGia / x.hd.TongTienHang) ?? 0))) ?? 0) ?? 0
                          };
                if (laHangHoa == 0)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == false);
                }
                else if (laHangHoa == 1)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == true);
                }
                var tbl1 = tbl.AsEnumerable().Select(t => new
                {
                    MaHangHoa = t.MaHangHoa,
                    TenDonViTinh = t.TenDonViTinh,
                    TenHangHoa = t.TenHangHoa,
                    TenHangHoa_CV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                    TenHangHoa_GC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                    MaKhachHang = t.MaKhachHang,
                    TenKhachHang = t.TenKhachHang,
                    TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                    TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                    ID_NhomHang = t.ID_NhomHang,
                    SoLuongDat = (double?)t.SoLuongDat ?? 0,
                    GiaTriDat = (double?)t.GiaTriDat ?? 0
                });
                if (ID_NhomHang != null)
                {
                    tbl1 = tbl1.Where(x => x.ID_NhomHang == ID_NhomHang);
                }
                if (maKH != null & maKH != "null" & maKH != "")
                {
                    maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                    tbl1 = tbl1.Where(x => x.MaKhachHang.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH));
                }
                if (maHH != null & maHH != "null" & maHH != "")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    tbl1 = tbl1.Where(x => x.MaHangHoa.Contains(@maHH) || x.TenHangHoa_CV.Contains(@maKH) || x.TenHangHoa_GC.Contains(@maKH));
                }
                var tbl_gop = from tb in tbl1
                              group tb by new
                              {
                                  tb.MaHangHoa
                              } into g
                              select new
                              {
                                  MaHangHoa = g.Key.MaHangHoa,
                                  TenHangHoa = g.FirstOrDefault().TenHangHoa,
                                  TenDonViTinh = g.FirstOrDefault().TenDonViTinh,
                                  SoLuongDat = (double?)g.Sum(x => x.SoLuongDat) ?? 0,
                                  GiaTriDat = (double?)g.Sum(x => x.GiaTriDat) ?? 0
                              };
                var tbl_format = tbl_gop.AsEnumerable().Select(t => new Report_DatHang_HangHoa
                {
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenDonViTinh != "" ? t.TenHangHoa + " (" + t.TenDonViTinh + ")" : t.TenHangHoa,
                    SoLuongDat = Math.Round(t.SoLuongDat, 3, MidpointRounding.ToEven),
                    GiaTriDat = Math.Round(t.GiaTriDat, 3, MidpointRounding.ToEven)
                });
                try
                {
                    lst = tbl_format.OrderByDescending(x => x.GiaTriDat).ToList();
                }
                catch
                {

                }
            }
            return lst;
        }
        public static List<Report_DatHang_HangHoaChiTiet> getListDatHang_HangHoaChiTiet(string ID_NhanVien, string maKH, string maHH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_DatHang_HangHoaChiTiet> lst = new List<Report_DatHang_HangHoaChiTiet>();
            string[] mang = ID_NhanVien.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            var tbl = from hd in db.BH_HoaDon.Where(x => LstIS.Contains(x.ID_NhanVien.ToString()))
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois.Where(x => x.MaHangHoa == maHH) on hdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                      from dt_hd in d.DefaultIfEmpty()
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.LoaiHoaDon == 3 & hd.YeuCau != "4"
                      select new
                      {
                          hd.MaHoaDon,
                          hd.NgayLapHoaDon,
                          dt_hd.MaDoiTuong,
                          dt_hd.TenDoiTuong,
                          hdct.SoLuong,
                          hd.TongGiamGia,
                          hd.TongTienHang,
                          hdct.ThanhTien
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new
            {
                MaHoaDon = t.MaHoaDon,
                NgayLapHoaDon = t.NgayLapHoaDon,
                MaKhachHang = t.MaDoiTuong != null ? t.MaDoiTuong : "Khách lẻ",
                TenKhachHang = t.TenDoiTuong != null ? t.TenDoiTuong : "Khách lẻ",
                TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenDoiTuong).ToLower(),
                TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenDoiTuong).ToLower(),
                SoLuongDat = (double?)t.SoLuong ?? 0,
                TongGiamGia = (double?)t.TongGiamGia ?? 0,
                TongTienHang = (double?)t.TongTienHang ?? 0,
                ThanhTien = (double?)t.ThanhTien ?? 0
            });
            if (maKH != null & maKH != "null" & maKH != "")
            {
                maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbl1 = tbl1.Where(x => x.MaKhachHang.Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH));
            }
            var tbl_gop = from tb in tbl1
                          group tb by new
                          {
                              tb.MaHoaDon
                          } into g
                          select new Report_DatHang_HangHoaChiTiet
                          {
                              MaHoaDon = g.Key.MaHoaDon,
                              NgayLapHoaDon = g.FirstOrDefault().NgayLapHoaDon,
                              TenKhachHang = g.FirstOrDefault().TenKhachHang,
                              SoLuongDat = g.Sum(x => (double?)x.SoLuongDat ?? 0),
                              GiaTriDat = (double?)g.Sum(x => (double?)(x.ThanhTien * (1 - ((double?)(x.TongGiamGia / x.TongTienHang) ?? 0))) ?? 0) ?? 0
                          };
            var tbl_fomat = tbl_gop.AsEnumerable().Select(t => new Report_DatHang_HangHoaChiTiet
            {
                MaHoaDon = t.MaHoaDon,
                NgayLapHoaDon = t.NgayLapHoaDon,
                TenKhachHang = t.TenKhachHang,
                SoLuongDat = Math.Round(t.SoLuongDat, 3, MidpointRounding.ToEven),
                GiaTriDat = Math.Round(t.GiaTriDat, 3, MidpointRounding.ToEven)
            });
            try
            {
                lst = tbl_fomat.OrderByDescending(x => x.GiaTriDat).ToList();
            }
            catch
            { }
            return lst;
        }
        public static List<Report_DatHang_GiaoDich> getListDatHang_GiaoDich(string ID_NhanVien, string maKH, string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_DatHang_GiaoDich> lst = new List<Report_DatHang_GiaoDich>();
            string[] mang = ID_NhanVien.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            if (laHangHoa != 3)
            {
                var tbl = from hd in db.BH_HoaDon.Where(x => LstIS.Contains(x.ID_NhanVien.ToString()))
                          join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                          from dt_hd in d.DefaultIfEmpty()
                          where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.LoaiHoaDon == 3 & hd.YeuCau != "4"
                          group new { hd, hdct, dvqd, hh, dt_hd } by new
                          {
                              hd.MaHoaDon,
                              dvqd.MaHangHoa
                          } into g
                          select new
                          {
                              ID_HoaDon = g.FirstOrDefault().hd.ID,
                              MaHoaDon = g.Key.MaHoaDon,
                              NgayLapHoaDon = g.FirstOrDefault().hd.NgayLapHoaDon,
                              MaKhachHang = g.FirstOrDefault().dt_hd.MaDoiTuong != null ? g.FirstOrDefault().dt_hd.MaDoiTuong : "Khách lẻ",
                              TenKhachHang = g.FirstOrDefault().dt_hd.TenDoiTuong != null ? g.FirstOrDefault().dt_hd.TenDoiTuong : "Khách lẻ",
                              SoLuongDat = (double?)g.FirstOrDefault().hdct.SoLuong ?? 0,
                              TongGiamGia = (double?)g.FirstOrDefault().hd.TongGiamGia ?? 0,
                              TongTienHang = (double?)g.FirstOrDefault().hd.TongTienHang ?? 0,
                              ThanhTien = (double?)g.FirstOrDefault().hdct.ThanhTien ?? 0,
                              MaHangHoa = g.Key.MaHangHoa,
                              TenHangHoa = g.FirstOrDefault().hh.TenHangHoa,
                              LaHangHoa = g.FirstOrDefault().hh.LaHangHoa,
                              TenDonViTinh = g.FirstOrDefault().dvqd.TenDonViTinh,
                              ID_NhomHang = g.FirstOrDefault().hh.ID_NhomHang
                          };
                if (laHangHoa == 0)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == false);
                }
                else if (laHangHoa == 1)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == true);
                }
                var tbl1 = tbl.AsEnumerable().Select(t => new
                {
                    MaHoaDon = t.MaHoaDon,
                    MaHangHoa = t.MaHangHoa,
                    TenDonViTinh = t.TenDonViTinh,
                    TenHangHoa = t.TenHangHoa,
                    TenHangHoa_CV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                    TenHangHoa_GC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                    MaKhachHang = t.MaKhachHang,
                    TenKhachHang = t.TenKhachHang,
                    TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                    TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                    ID_NhomHang = t.ID_NhomHang,
                    SoLuongDat = (double?)t.SoLuongDat ?? 0,
                    TongGiamGia = (double?)t.TongGiamGia ?? 0,
                    TongTienHang = (double?)t.TongTienHang ?? 0,
                    ThanhTien = (double?)t.ThanhTien ?? 0,
                    NgayLapHoaDon = t.NgayLapHoaDon
                });
                if (ID_NhomHang != null)
                {
                    tbl1 = tbl1.Where(x => x.ID_NhomHang == ID_NhomHang);
                }
                if (maKH != null & maKH != "null" & maKH != "")
                {
                    maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                    tbl1 = tbl1.Where(x => x.MaKhachHang.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH));
                }
                if (maHH != null & maHH != "null" & maHH != "")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    tbl1 = tbl1.Where(x => x.MaHangHoa.Contains(@maHH) || x.TenHangHoa_CV.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH));
                }
                var tbl_gop = from tb in tbl1
                              group tb by new
                              {
                                  tb.MaHoaDon
                              } into g
                              select new Report_DatHang_GiaoDich
                              {
                                  MaHoaDon = g.Key.MaHoaDon,
                                  NgayLapHoaDon = g.FirstOrDefault().NgayLapHoaDon,
                                  TenKhachHang = g.FirstOrDefault().TenKhachHang,
                                  SoLuongDat = g.Sum(x => (double?)x.SoLuongDat ?? 0),
                                  GiaTriDat = (double?)g.Sum(x => (double?)(x.ThanhTien * (1 - ((double?)(x.TongGiamGia / x.TongTienHang) ?? 0))) ?? 0) ?? 0
                              };
                var tbl_fomat = tbl_gop.AsEnumerable().Select(t => new Report_DatHang_GiaoDich
                {
                    MaHoaDon = t.MaHoaDon,
                    NgayLapHoaDon = t.NgayLapHoaDon,
                    TenKhachHang = t.TenKhachHang,
                    SoLuongDat = Math.Round(t.SoLuongDat, 3, MidpointRounding.ToEven),
                    GiaTriDat = Math.Round(t.GiaTriDat, 3, MidpointRounding.ToEven)
                });
                try
                {
                    lst = tbl_fomat.OrderByDescending(x => x.GiaTriDat).ToList();
                }
                catch
                { }
            }
            return lst;
        }
        public static List<Report_DatHang_GiaoDichChiTiet> getListDatHang_GiaoDichChiTiet(string MaHoaDon, string maHH, DateTime timeStart, DateTime timeEnd, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_DatHang_GiaoDichChiTiet> lst = new List<Report_DatHang_GiaoDichChiTiet>();
            var tbl = from hd in db.BH_HoaDon.Where(x => x.MaHoaDon == MaHoaDon)
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                      from dt_hd in d.DefaultIfEmpty()
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh & hd.LoaiHoaDon == 3 & hd.YeuCau != "4"
                      select new
                      {
                          hd.ID,
                          hdct.ID_DonViQuiDoi,
                          dvqd.MaHangHoa,
                          dvqd.TenDonViTinh,
                          hh.TenHangHoa,
                          hdct.SoLuong,
                          hh.ID_NhomHang
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new Report_DatHang_GiaoDichChiTiet
            {
                ID_NhomHang = t.ID_NhomHang,
                MaHangHoa = t.MaHangHoa,
                TenHangHoa = t.TenDonViTinh != "" ? t.TenHangHoa + " (" + t.TenDonViTinh + ")" : t.TenHangHoa,
                TenHangHoa_CV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                TenHangHoa_GC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                SoLuongDat = Math.Round(t.SoLuong, 3, MidpointRounding.ToEven),
                SoLuongNhan = SoLuongNhan(t.ID, t.ID_DonViQuiDoi, ID_ChiNhanh),
            });
            if (maHH != null & maHH != "null" & maHH != "")
            {
                maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                tbl1 = tbl1.Where(x => x.MaHangHoa.Contains(@maHH) || x.TenHangHoa_CV.Contains(@maHH) || x.TenHangHoa_GC.Contains(@maHH));
            }
            if (ID_NhomHang != null)
            {
                tbl1 = tbl1.Where(x => x.ID_NhomHang == ID_NhomHang);
            }
            try
            {
                lst = tbl1.OrderByDescending(x => x.SoLuongDat).ToList();
            }
            catch
            { }
            return lst;
        }
        public static double SoLuongNhan(Guid? ID_HoaDon, Guid? ID_DonViQuiDoi, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            double a = 0;
            var tbl = from hd in db.BH_HoaDon.Where(x => x.ID_HoaDon == ID_HoaDon)
                      join hdct in db.BH_HoaDon_ChiTiet.Where(x => x.ID_DonViQuiDoi == ID_DonViQuiDoi) on hd.ID equals hdct.ID_HoaDon
                      where hd.LoaiHoaDon == 1 & hd.ID_DonVi == ID_ChiNhanh
                      select new
                      {
                          hdct.SoLuong
                      };
            try
            {
                a = (double?)tbl.FirstOrDefault().SoLuong ?? 0;
            }
            catch
            {

            }
            return Math.Round(a, 3, MidpointRounding.ToEven);
        }
        #endregion
        #region báo cáo tài chính
        
        #endregion
        #region báo cáo cuối ngày

        public static List<Report_CuoiNgay_BanHang> getListCuoiNgay_BanHang(string ID_NhanVien, string maKH, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            string[] mangCN = ID_ChiNhanh.Split(',');
            List<string> lstCN = new List<string>();
            for (int i = 0; i < mangCN.Length; i++)
            {
                lstCN.Add(mangCN[i].ToString());
            }

            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_BanHang> lst = new List<Report_CuoiNgay_BanHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()) & lstCN.Contains(x.ID_DonVi.ToString()))
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                      from dt_hd in d.DefaultIfEmpty()
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 3 || hd.LoaiHoaDon == 6)
                      select new
                      {
                          MaHoaDon = hd.MaHoaDon,
                          MaKhachHang = dt_hd.MaDoiTuong != null ? dt_hd.MaDoiTuong : "Khách lẻ",
                          TenKhachHang = dt_hd.TenDoiTuong != null ? dt_hd.TenDoiTuong : "Khách lẻ",
                          LoaiHoaDon = hd.LoaiHoaDon,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          TongChiPhi = hd.TongChiPhi
                      };
            var tbl_hd = tbl.AsEnumerable().Select(t => new
            {
                MaHoaDon = t.MaHoaDon,
                MaKhachHang = t.MaKhachHang,
                TenKhachHang = t.TenKhachHang,
                LoaiHoaDon = t.LoaiHoaDon,
                TongTienHang = t.TongTienHang,
                PhaiThanhToan = t.PhaiThanhToan,
                TongChiPhi = t.TongChiPhi
            });
            //if (maKH != null & maKH != "" & maKH != "null")
            //{
            //    maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
            //    tbl_hd = tbl_hd.Where(x => x.MaKhachHang.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH));
            //}
            var tbl_soluong = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()) & lstCN.Contains(x.ID_DonVi.ToString()))
                              join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                              where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 3 || hd.LoaiHoaDon == 6)
                              group new { hd, hdct } by new
                              {
                                  hd.MaHoaDon
                              } into g
                              select new
                              {
                                  MaHoaDon = g.Key.MaHoaDon,
                                  SoLuong = g.Sum(x => (double?)x.hdct.SoLuong ?? 0)
                              };
            var tbl_soquy = from hd in db.Quy_HoaDon
                            join hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                            join bhdh in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()) & lstCN.Contains(x.ID_DonVi.ToString())) on hdct.ID_HoaDonLienQuan equals bhdh.ID
                            join dt in db.DM_DoiTuong on bhdh.ID_DoiTuong equals dt.ID into d
                            from dt_hd in d.DefaultIfEmpty()
                            where bhdh.NgayLapHoaDon >= timeStart & bhdh.NgayLapHoaDon < timeEnd & bhdh.ChoThanhToan == false & (bhdh.LoaiHoaDon == 1 || bhdh.LoaiHoaDon == 3 || bhdh.LoaiHoaDon == 6)
                            select new
                            {
                                MaHoaDon = bhdh.MaHoaDon,
                                MaKhachHang = dt_hd.MaDoiTuong != null ? dt_hd.MaDoiTuong : "Khách lẻ",
                                TenKhachHang = dt_hd.TenDoiTuong != null ? dt_hd.TenDoiTuong : "Khách lẻ",
                                LoaiHoaDon = bhdh.LoaiHoaDon,
                                TongTienThu = (double?)hd.TongTienThu ?? 0
                            };
            var tbl_uni = (from hd in tbl_hd
                           join sl in tbl_soluong on hd.MaHoaDon equals sl.MaHoaDon
                           select new Report_CuoiNgay_Union
                           {
                               MaKhachHang = hd.MaKhachHang,
                               TenKhachHang = hd.TenKhachHang,
                               LoaiHoaDon = hd.LoaiHoaDon,
                               SoLuong = sl.SoLuong,
                               PhaiThanhToan = hd.PhaiThanhToan,
                               TongTienHang = hd.TongTienHang,
                               TongChiPhi = hd.TongChiPhi,
                           }).Union(from sq in tbl_soquy
                                    select new Report_CuoiNgay_Union
                                    {
                                        MaKhachHang = sq.MaKhachHang,
                                        TenKhachHang = sq.TenKhachHang,
                                        LoaiHoaDon = sq.LoaiHoaDon,
                                        TongTienThu = sq.TongTienThu
                                    });
            var tbl_uniCV = tbl_uni.AsEnumerable().Select(t => new Report_CuoiNgay_Union
            {
                MaKhachHang = t.MaKhachHang,
                TenKhachHang = t.TenKhachHang,
                LoaiHoaDon = t.LoaiHoaDon,
                SoLuong = (double?)t.SoLuong ?? 0,
                PhaiThanhToan = (double?)t.PhaiThanhToan ?? 0,
                TongTienHang = (double?)t.TongTienHang ?? 0,
                TongChiPhi = (double?)t.TongChiPhi ?? 0,
                TongTienThu = (double?)t.TongTienThu ?? 0
            });
            if (maKH != null & maKH != "" & maKH != "null")
            {
                var maKHang = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbl_uniCV = tbl_uniCV.Where(x => CommonStatic.ConvertToUnSign(x.MaKhachHang).ToLower().Contains(maKHang) || CommonStatic.GetCharsStart(x.TenKhachHang).ToLower().Contains(maKHang) || CommonStatic.ConvertToUnSign(x.TenKhachHang).ToLower().Contains(maKHang));
            }
            var tbl_gop = from tb in tbl_uniCV
                          group tb by new
                          {
                              tb.LoaiHoaDon
                          } into g
                          select new
                          {
                              MaHoaDon = g.Key.LoaiHoaDon == 1 ? "Hóa Đơn: " + g.Where(x => x.LoaiHoaDon == 1 & x.PhaiThanhToan >= 0).Count() : (g.Key.LoaiHoaDon == 3 ? "Đặt hàng: " + g.Where(x => x.LoaiHoaDon == 3 & x.PhaiThanhToan >= 0).Count() : ("Trả hàng: " + g.Where(x => x.LoaiHoaDon == 6 & x.PhaiThanhToan >= 0).Count())),
                              SoLuong = (double?)g.Sum(x => (double?)x.SoLuong ?? 0) ?? 0,
                              DoanhThu = (double?)g.Sum(x => (double?)x.PhaiThanhToan ?? 0) ?? 0,
                              GiaTriTra = (double?)g.Sum(x => (double?)x.TongTienHang ?? 0) ?? 0,
                              PhiTraHang = (double?)g.Sum(x => (double?)x.TongChiPhi ?? 0) ?? 0,
                              ThucThu = (double?)g.Sum(x => (double?)x.TongTienThu ?? 0) ?? 0,
                              LoaiHoaDon = g.Key.LoaiHoaDon
                          };
            var tbl_format = tbl_gop.AsEnumerable().Select(t => new Report_CuoiNgay_BanHang
            {
                MaHoaDon = t.MaHoaDon,
                SoLuongSanPham = Math.Round(t.SoLuong, 3, MidpointRounding.ToEven),
                DoanhThu = Math.Round(t.LoaiHoaDon == 6 ? t.GiaTriTra * (-1) : t.DoanhThu, 0, MidpointRounding.ToEven),
                ThuKhac = 0,
                PhiTraHang = Math.Round(t.PhiTraHang, 3, MidpointRounding.ToEven),
                ThucThu = Math.Round(t.LoaiHoaDon == 6 ? t.ThucThu * (-1) : t.ThucThu, 0, MidpointRounding.ToEven),
                LoaiHoaDon = t.LoaiHoaDon
            });
            try
            {
                lst = tbl_format.OrderBy(x => x.MaHoaDon).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_CuoiNgay_BanHang> getListCuoiNgay_ChiTietBanHang(int loaihoadon, String ID_NhanVien, string maKH, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            string[] mangCN = ID_ChiNhanh.Split(',');
            List<string> lstCN = new List<string>();
            for (int i = 0; i < mangCN.Length; i++)
            {
                lstCN.Add(mangCN[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_BanHang> lst = new List<Report_CuoiNgay_BanHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()) & lstCN.Contains(x.ID_DonVi.ToString()))
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                      from dt_hd in d.DefaultIfEmpty()
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.LoaiHoaDon == loaihoadon
                      select new
                      {
                          MaHoaDon = hd.MaHoaDon,
                          NgayLapHaoDon = hd.NgayLapHoaDon,
                          MaKhachHang = dt_hd.MaDoiTuong != null ? dt_hd.MaDoiTuong : "Khách lẻ",
                          TenKhachHang = dt_hd.TenDoiTuong != null ? dt_hd.TenDoiTuong : "Khách lẻ",
                          LoaiHoaDon = hd.LoaiHoaDon,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          TongChiPhi = hd.TongChiPhi
                      };
            var tbl_hd = tbl.AsEnumerable().Select(t => new
            {
                MaHoaDon = t.MaHoaDon,
                NgayLapHoaDon = t.NgayLapHaoDon,
                MaKhachHang = t.MaKhachHang,
                TenKhachHang = t.TenKhachHang,
                LoaiHoaDon = t.LoaiHoaDon,
                TongTienHang = t.TongTienHang,
                PhaiThanhToan = t.PhaiThanhToan,
                TongChiPhi = t.TongChiPhi
            });

            var tbl_soluong = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()) & lstCN.Contains(x.ID_DonVi.ToString()))
                              join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                              where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.LoaiHoaDon == loaihoadon
                              group new { hd, hdct } by new
                              {
                                  hd.MaHoaDon
                              } into g
                              select new
                              {
                                  MaHoaDon = g.Key.MaHoaDon,
                                  SoLuong = g.Sum(x => (double?)x.hdct.SoLuong ?? 0)
                              };
            var tbl_soquy = from hd in db.Quy_HoaDon
                            join hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                            join bhdh in db.BH_HoaDon.Where(x => lstCN.Contains(x.ID_DonVi.ToString())) on hdct.ID_HoaDonLienQuan equals bhdh.ID
                            join dt in db.DM_DoiTuong on bhdh.ID_DoiTuong equals dt.ID into d
                            from dt_hd in d.DefaultIfEmpty()
                            where bhdh.NgayLapHoaDon >= timeStart & bhdh.NgayLapHoaDon < timeEnd & bhdh.ChoThanhToan == false & bhdh.LoaiHoaDon == loaihoadon
                            select new
                            {
                                MaHoaDon = bhdh.MaHoaDon,
                                NgayLapHoaDon = bhdh.NgayLapHoaDon,
                                MaKhachHang = dt_hd.MaDoiTuong != null ? dt_hd.MaDoiTuong : "Khách lẻ",
                                TenKhachHang = dt_hd.TenDoiTuong != null ? dt_hd.TenDoiTuong : "Khách lẻ",
                                LoaiHoaDon = bhdh.LoaiHoaDon,
                                TongTienThu = (double?)hd.TongTienThu ?? 0
                            };
            var tbl_uni = (from hd in tbl_hd
                           join sl in tbl_soluong on hd.MaHoaDon equals sl.MaHoaDon
                           select new Report_CuoiNgay_Union
                           {
                               MaHoaDon = hd.MaHoaDon,
                               NgayLapHoaDon = hd.NgayLapHoaDon,
                               MaKhachHang = hd.MaKhachHang,
                               TenKhachHang = hd.TenKhachHang,
                               LoaiHoaDon = hd.LoaiHoaDon,
                               SoLuong = sl.SoLuong,
                               PhaiThanhToan = hd.PhaiThanhToan,
                               TongTienHang = hd.TongTienHang,
                               TongChiPhi = hd.TongChiPhi,
                           }).Union(from sq in tbl_soquy
                                    select new Report_CuoiNgay_Union
                                    {
                                        MaHoaDon = sq.MaHoaDon,
                                        NgayLapHoaDon = sq.NgayLapHoaDon,
                                        MaKhachHang = sq.MaKhachHang,
                                        TenKhachHang = sq.TenKhachHang,
                                        LoaiHoaDon = sq.LoaiHoaDon,
                                        TongTienThu = sq.TongTienThu
                                    });
            var tbl_uniCV = tbl_uni.AsEnumerable().Select(t => new Report_CuoiNgay_Union
            {
                MaHoaDon = t.MaHoaDon,
                NgayLapHoaDon = t.NgayLapHoaDon,
                MaKhachHang = t.MaKhachHang,
                TenKhachHang = t.TenKhachHang,
                LoaiHoaDon = t.LoaiHoaDon,
                SoLuong = (double?)t.SoLuong ?? 0,
                PhaiThanhToan = (double?)t.PhaiThanhToan ?? 0,
                TongTienHang = (double?)t.TongTienHang ?? 0,
                TongChiPhi = (double?)t.TongChiPhi ?? 0,
                TongTienThu = (double?)t.TongTienThu ?? 0
            });
            if (maKH != null & maKH != "" & maKH != "null")
            {
                var maKHang = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbl_uniCV = tbl_uniCV.Where(x => CommonStatic.ConvertToUnSign(x.MaKhachHang).ToLower().Contains(maKHang) || CommonStatic.GetCharsStart(x.TenKhachHang).ToLower().Contains(maKHang) || CommonStatic.ConvertToUnSign(x.TenKhachHang).ToLower().Contains(maKHang));
            }
            var tbl_gop = from tb in tbl_uniCV
                          group tb by new
                          {
                              tb.MaHoaDon
                          } into g
                          select new
                          {
                              MaHoaDon = g.Key.MaHoaDon,
                              NgayLapHoaDon = g.FirstOrDefault().NgayLapHoaDon,
                              SoLuong = (double?)g.Sum(x => (double?)x.SoLuong ?? 0) ?? 0,
                              DoanhThu = (double?)g.Sum(x => (double?)x.PhaiThanhToan ?? 0) ?? 0,
                              GiaTriTra = (double?)g.Sum(x => (double?)x.TongTienHang ?? 0) ?? 0,
                              PhiTraHang = (double?)g.Sum(x => (double?)x.TongChiPhi ?? 0) ?? 0,
                              ThucThu = (double?)g.Sum(x => (double?)x.TongTienThu ?? 0) ?? 0,
                          };
            var tbl_format = tbl_gop.AsEnumerable().Select(t => new Report_CuoiNgay_BanHang
            {
                MaHoaDon = t.MaHoaDon,
                NgayLapHoaDon = t.NgayLapHoaDon,
                SoLuongSanPham = Math.Round(t.SoLuong, 3, MidpointRounding.ToEven),
                DoanhThu = Math.Round(loaihoadon == 6 ? t.GiaTriTra * (-1) : t.DoanhThu, 0, MidpointRounding.ToEven),
                ThuKhac = 0,
                PhiTraHang = Math.Round(t.PhiTraHang, 3, MidpointRounding.ToEven),
                ThucThu = Math.Round(loaihoadon == 6 ? t.ThucThu * (-1) : t.ThucThu, 0, MidpointRounding.ToEven),
            });
            try
            {
                lst = tbl_format.OrderBy(x => x.NgayLapHoaDon).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_CuoiNgay_PhieuThu> getListCuoiNgay_PhieuThu(string LoaiThuChi, string ID_NhanVien, string maKH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_PhieuThu> lst = new List<Report_CuoiNgay_PhieuThu>();
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            string[] mangTC = LoaiThuChi.Split(',');
            List<string> lstThuChi = new List<string>();
            for (int i = 0; i < mangTC.Length; i++)
            {
                lstThuChi.Add(mangTC[i].ToString());
            }
            var tbl_hd = from hd in db.Quy_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()))
                         join hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                         join hdbh in db.BH_HoaDon on hdct.ID_HoaDonLienQuan equals hdbh.ID into d
                         from bh_hd in d.DefaultIfEmpty()
                         join dt in db.DM_DoiTuong on hdct.ID_DoiTuong equals dt.ID into l
                         from dt_hd in l.DefaultIfEmpty()
                         where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.TrangThai != false & hd.ID_DonVi == ID_ChiNhanh
                         select new
                         {
                             LoaiThuChi = hd.LoaiHoaDon == 11 & hdct.ID_HoaDonLienQuan == null ? "1" :
                             hd.LoaiHoaDon == 12 & hdct.ID_HoaDonLienQuan == null ? "2" :
                             bh_hd.LoaiHoaDon == 1 ? "3" :
                              bh_hd.LoaiHoaDon == 6 ? "4" :
                               bh_hd.LoaiHoaDon == 7 ? "5" :
                                bh_hd.LoaiHoaDon == 4 ? "6" : "",
                             MaKhachHang = dt_hd.MaDoiTuong != null ? dt_hd.MaDoiTuong : "Khách lẻ",
                             TenKhachHang = dt_hd.TenDoiTuong != null ? dt_hd.TenDoiTuong : "Khách lẻ",
                             MaPhieuThu = hd.MaHoaDon,
                             TenNguoiNop = hd.NguoiNopTien != null & hd.NguoiNopTien != "" ? hd.NguoiNopTien : "Khách lẻ",
                             ThuChi = hd.TongTienThu,
                             NgayLapHoaDon = hd.NgayLapHoaDon,
                             MaHoaDon = bh_hd.MaHoaDon == null & hd.LoaiHoaDon == 11 ? "Phiếu thu khác" : (bh_hd.MaHoaDon == null & hd.LoaiHoaDon == 12 ? "Phiếu chi khác" : bh_hd.MaHoaDon),
                         };
            var tbls = tbl_hd.AsEnumerable().Select(t => new
            {
                LoaiThuChi = t.LoaiThuChi,
                MaKhachHang = t.MaKhachHang,
                TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                MaPhieuThu = t.MaPhieuThu,
                TenNguoiNop = t.TenNguoiNop,
                ThuChi = t.ThuChi,
                NgayLapHoaDon = t.NgayLapHoaDon,
                MaHoaDon = t.MaHoaDon
            });
            if (maKH != null && maKH != "null" && maKH != "")
            {
                string ma_khachHang = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbls = tbls.Where(x => x.MaKhachHang.Contains(@ma_khachHang) || x.TenKhachHang_GC.Contains(@ma_khachHang) || x.TenKhachHang_CV.ToLower().Contains(@ma_khachHang));
            }
            tbls = tbls.Where(x => lstThuChi.Contains(x.LoaiThuChi));
            var tbl_format = tbls.AsEnumerable().Select(t => new Report_CuoiNgay_PhieuThu
            {
                MaPhieuThu = t.MaPhieuThu,
                TenNguoiNop = t.TenNguoiNop,
                ThuChi = Math.Round(t.ThuChi, 0, MidpointRounding.ToEven),
                NgayLapHoaDon = t.NgayLapHoaDon,
                MaHoaDon = t.MaHoaDon
            });
            try
            {
                lst = tbl_format.OrderByDescending(x => x.NgayLapHoaDon).ToList();
            }
            catch
            {

            }
            return lst;
        }

        public static List<Report_CuoiNgay_HangHoa> getListCuoiNgay_HangHoa(string ID_NhanVien, string maKH, string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_HangHoa> lst = new List<Report_CuoiNgay_HangHoa>();
            if (laHangHoa != 3)
            {
                var tbl = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()))
                          join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                          join dvqd in db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                          join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                          join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                          from dt_hd in d.DefaultIfEmpty()
                          join nh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nh.ID into l
                          from nh_hh in l.DefaultIfEmpty()
                          where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                          select new
                          {
                              LoaiHoaDon = hd.LoaiHoaDon,
                              MaHangHoa = dvqd.MaHangHoa,
                              TenHangHoa = hh.TenHangHoa,
                              MaKhachHang = dt_hd.MaDoiTuong != null ? dt_hd.MaDoiTuong : "Khách lẻ",
                              TenKhachHang = dt_hd.TenDoiTuong != null ? dt_hd.TenDoiTuong : "Khách lẻ",
                              LaHangHoa = hh.LaHangHoa,
                              ID_NhomHang = hh.ID_NhomHang,
                              ID_Parent = nh_hh.ID_Parent,
                              SoLuong = bhct.SoLuong,
                              DonGia = bhct.DonGia,
                              TienChietKhau = bhct.TienChietKhau
                          };
                if (laHangHoa == 0)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == false);
                }
                else if (laHangHoa == 1)
                {
                    tbl = tbl.Where(x => x.LaHangHoa == true);
                }
                if (ID_NhomHang != null)
                {
                    tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang || x.ID_Parent == ID_NhomHang);
                }
                var tbl1 = tbl.AsEnumerable().Select(t => new
                {
                    LoaiHoaDon = t.LoaiHoaDon,
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenHangHoa,
                    TenHangHoa_CV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                    TenHangHoa_GC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                    MaKhachHang = t.MaKhachHang,
                    TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                    TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                    SoLuong = t.SoLuong,
                    DonGia = t.DonGia,
                    TienChietKhau = t.TienChietKhau
                });
                if (maHH != "null" & maHH != null & maHH != "")
                {
                    maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                    tbl1 = tbl1.Where(x => x.MaHangHoa.ToLower().Contains(@maHH) || x.TenHangHoa_CV.Contains(@maHH) || x.TenHangHoa_GC.Contains(@maHH));
                }
                if (maKH != "null" & maKH != null & maKH != "")
                {
                    maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                    tbl1 = tbl1.Where(x => x.MaKhachHang.ToLower().Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH));
                }
                var tbl_Gop = from tb in tbl1
                              group tb by new
                              {
                                  tb.MaHangHoa
                              } into g
                              select new Report_CuoiNgay_HangHoa
                              {
                                  MaHangHoa = g.Key.MaHangHoa,
                                  TenHangHoa = g.FirstOrDefault().TenHangHoa,
                                  SoLuongBan = (double?)g.Where(x => x.LoaiHoaDon == 1).Sum(x => (double?)x.SoLuong ?? 0) ?? 0,
                                  SoLuongTra = (double?)g.Where(x => x.LoaiHoaDon == 6).Sum(x => (double?)x.SoLuong ?? 0) ?? 0,
                                  GiaTriBan = (double?)g.Where(x => x.LoaiHoaDon == 1).Sum(x => (double?)x.SoLuong * (x.DonGia - x.TienChietKhau) ?? 0) ?? 0,
                                  GiaTriTra = (double?)g.Where(x => x.LoaiHoaDon == 6).Sum(x => (double?)x.SoLuong * (x.DonGia - x.TienChietKhau) ?? 0) ?? 0,
                              };
                var tbl_format = tbl_Gop.AsEnumerable().Select(t => new Report_CuoiNgay_HangHoa
                {
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenHangHoa,
                    SoLuongBan = Math.Round(t.SoLuongBan, 3, MidpointRounding.ToEven),
                    GiaTriBan = Math.Round(t.GiaTriBan, 0, MidpointRounding.ToEven),
                    SoLuongTra = Math.Round(t.SoLuongTra, 3, MidpointRounding.ToEven),
                    GiaTriTra = Math.Round(t.GiaTriTra * (-1), 0, MidpointRounding.ToEven),
                    DoanhThu = Math.Round(t.GiaTriBan - t.GiaTriTra, 0, MidpointRounding.ToEven)
                });
                try
                {
                    lst = tbl_format.OrderByDescending(x => x.DoanhThu).ToList();
                }
                catch { }
            }
            return lst;
        }
        public static List<Report_CuoiNgay_HangHoaChiTiet> getListCuoiNgay_HangHoaChiTiet(string ID_NhanVien, string maKH, string maHH, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_HangHoaChiTiet> lst = new List<Report_CuoiNgay_HangHoaChiTiet>();
            var tbl = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()))
                      join bhct in db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                      join dvqd in db.DonViQuiDois.Where(x => x.MaHangHoa == maHH) on bhct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID into d
                      from dt_hd in d.DefaultIfEmpty()
                      join nh in db.DM_NhomHangHoa on hh.ID_NhomHang equals nh.ID into l
                      from nh_hh in l.DefaultIfEmpty()
                      where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      select new
                      {
                          MaHoaDon = hd.MaHoaDon,
                          LoaiHoaDon = hd.LoaiHoaDon,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          MaKhachHang = dt_hd.MaDoiTuong != null ? dt_hd.MaDoiTuong : "Khách lẻ",
                          TenKhachHang = dt_hd.TenDoiTuong != null ? dt_hd.TenDoiTuong : "Khách lẻ",
                          SoLuong = (double?)bhct.SoLuong ?? 0,
                          DonGia = (double?)bhct.DonGia ?? 0,
                          TienChietKhau = (double?)bhct.TienChietKhau ?? 0
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new Report_CuoiNgay_HangHoaChiTiet
            {
                MaHoaDon = t.MaHoaDon,
                NgayLapHoaDon = t.NgayLapHoaDon,
                MaKhachHang = t.MaKhachHang,
                TenKhachHang = t.TenKhachHang,
                TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenKhachHang).ToLower(),
                TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenKhachHang).ToLower(),
                SoLuongSanPham = Math.Round(t.SoLuong, 3, MidpointRounding.ToEven),
                DoanhThu = Math.Round(t.LoaiHoaDon == 1 ? t.SoLuong * (t.DonGia - t.TienChietKhau) : t.SoLuong * (-1) * (t.DonGia - t.TienChietKhau), 0, MidpointRounding.ToEven)
            });
            if (maKH != null & maKH != "null" & maKH != "")
            {
                maKH = CommonStatic.ConvertToUnSign(maKH).ToLower();
                tbl1 = tbl1.Where(x => x.MaKhachHang.ToLower().Contains(@maKH) || x.TenKhachHang_GC.Contains(@maKH) || x.TenKhachHang_CV.Contains(@maKH));
            }
            try
            {
                lst = tbl1.OrderByDescending(x => x.NgayLapHoaDon).ToList();
            }
            catch
            {

            }
            return lst;

        }
        public static List<Report_CuoiNgay_TongHop> getListCuoiNgay_TongKetThuChi(string ID_NhanVien, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_TongHop> lst = new List<Report_CuoiNgay_TongHop>();
            var tbl = from hd in db.Quy_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()))
                      join hdct in db.Quy_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      where hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.TrangThai != false & hd.ID_DonVi == ID_ChiNhanh
                      group new { hd, hdct } by new
                      {
                          hd.LoaiHoaDon
                      } into g
                      select new
                      {
                          LoaiHoaDon = g.Key.LoaiHoaDon,
                          ThuChi = g.Key.LoaiHoaDon == 11 ? "Tổng thu" : "Tổng chi",
                          TienMat = (double?)g.Sum(x => (double?)x.hdct.TienMat ?? 0) ?? 0,
                          ChuyenKhoan = (double?)g.Sum(x => (double?)x.hdct.TienGui ?? 0) ?? 0,
                          The = (double?)g.Sum(x => (double?)x.hdct.ThuTuThe ?? 0) ?? 0,
                          TongThucThu = (double?)g.Sum(x => (double?)x.hdct.TienThu ?? 0) ?? 0
                      };
            var tbl_format = tbl.AsEnumerable().Select(t => new Report_CuoiNgay_TongHop
            {
                ThuChi = t.ThuChi,
                TienMat = Math.Round(t.LoaiHoaDon == 11 ? t.TienMat : t.TienMat * (-1), 0, MidpointRounding.ToEven),
                ChuyenKhoan = Math.Round(t.LoaiHoaDon == 11 ? t.ChuyenKhoan : t.ChuyenKhoan * (-1), 0, MidpointRounding.ToEven),
                The = Math.Round(t.LoaiHoaDon == 11 ? t.The : t.The * (-1), 0, MidpointRounding.ToEven),
                Diem = 0,
                TongThucThu = Math.Round(t.LoaiHoaDon == 11 ? t.TongThucThu : t.TongThucThu * (-1), 0, MidpointRounding.ToEven)
            });
            try
            {
                lst = tbl_format.OrderByDescending(x => x.ThuChi).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_CuoiNgay_TongKetBanHang> getListCuoiNgay_TongKetBanHang(String ID_NhanVien, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_TongKetBanHang> lst = new List<Report_CuoiNgay_TongKetBanHang>();
            var tbl = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()))
                      join qhdct in db.Quy_HoaDon_ChiTiet on hd.ID equals qhdct.ID_HoaDonLienQuan into q
                      from q_hdct in q.DefaultIfEmpty()
                      join qhd in db.Quy_HoaDon.Where(x => x.TrangThai != false) on q_hdct.ID_HoaDon equals qhd.ID into l
                      from q_hd in l.DefaultIfEmpty()
                      where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 3 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      group new { hd, q_hd, q_hdct } by new
                      {
                          hd.LoaiHoaDon
                      } into g
                      select new
                      {
                          LoaiHoaDon = g.Key.LoaiHoaDon,
                          GiaoDich = g.Key.LoaiHoaDon == 1 ? "Bán hàng" : g.Key.LoaiHoaDon == 3 ? "Đặt hàng" : "Trả hàng",
                          GiaTri = (double?)g.Sum(x => (double?)x.hd.TongTienHang ?? 0) ?? 0,
                          TienMat = (double?)g.Sum(x => (double?)x.q_hdct.TienMat ?? 0) ?? 0,
                          ChuyenKhoan = (double?)g.Sum(x => (double?)x.q_hdct.TienGui ?? 0) ?? 0,
                          The = (double?)g.Sum(x => (double?)x.q_hdct.ThuTuThe ?? 0) ?? 0,
                          TongThucThu = (double?)g.Sum(x => (double?)x.q_hdct.TienThu ?? 0) ?? 0,
                      };
            var tbl_format = tbl.AsEnumerable().Select(t => new Report_CuoiNgay_TongKetBanHang
            {
                GiaoDich = t.GiaoDich,
                GiaTri = Math.Round(t.LoaiHoaDon != 6 ? t.GiaTri : t.GiaTri * (-1), 0, MidpointRounding.ToEven),
                TienMat = Math.Round(t.LoaiHoaDon != 6 ? t.TienMat : t.TienMat * (-1), 0, MidpointRounding.ToEven),
                ChuyenKhoan = Math.Round(t.LoaiHoaDon != 6 ? t.ChuyenKhoan : t.ChuyenKhoan * (-1), 0, MidpointRounding.ToEven),
                The = Math.Round(t.LoaiHoaDon != 6 ? t.The : t.The * (-1), 0, MidpointRounding.ToEven),
                Diem = 0,
                TongThucThu = Math.Round(t.LoaiHoaDon != 6 ? t.TongThucThu : t.TongThucThu * (-1), 0, MidpointRounding.ToEven)
            });
            try
            {
                lst = tbl_format.OrderByDescending(x => x.GiaoDich).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_CuoiNgay_SoLuongGiaoDich> getListCuoiNgay_SoLuongGiaoDich(String ID_NhanVien, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_SoLuongGiaoDich> lst = new List<Report_CuoiNgay_SoLuongGiaoDich>();
            var tbl = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()))
                      join qhdct in db.Quy_HoaDon_ChiTiet on hd.ID equals qhdct.ID_HoaDonLienQuan into q
                      from q_hdct in q.DefaultIfEmpty()
                      join qhd in db.Quy_HoaDon.Where(x => x.TrangThai != false) on q_hdct.ID_HoaDon equals qhd.ID into l
                      from q_hd in l.DefaultIfEmpty()
                      where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 3 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      group new { hd, q_hd, q_hdct } by new
                      {
                          hd.LoaiHoaDon
                      } into g
                      select new Report_CuoiNgay_SoLuongGiaoDich
                      {
                          GiaoDich = g.Key.LoaiHoaDon == 1 ? "Hóa đơn" : g.Key.LoaiHoaDon == 3 ? "Đặt hàng" : "Trả hàng",
                          SoGiaoDich = g.Count(),
                          TienMat = g.Where(x => ((double?)x.q_hdct.TienMat ?? 0) > 0).Count(),
                          ChuyenKhoan = g.Where(x => ((double?)x.q_hdct.TienGui ?? 0) > 0).Count(),
                          The = g.Where(x => ((double?)x.q_hdct.ThuTuThe ?? 0) > 0).Count(),
                          Diem = 0
                      };
            try
            {
                lst = tbl.OrderByDescending(x => x.GiaoDich).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public static List<Report_CuoiNgay_SoLuongHangHoa> getListCuoiNgay_SoLuongHangHoa(String ID_NhanVien, DateTime timeStart, DateTime timeEnd, Guid ID_ChiNhanh)
        {
            string[] mang = ID_NhanVien.Split(',');
            List<string> liST = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                liST.Add(mang[i].ToString());
            }
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<Report_CuoiNgay_SoLuongHangHoa> lst = new List<Report_CuoiNgay_SoLuongHangHoa>();
            var tbl = from hd in db.BH_HoaDon.Where(x => liST.Contains(x.ID_NhanVien.ToString()))
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 3 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                      group new { hd, dvqd, hdct } by new
                      {
                          hd.LoaiHoaDon
                      } into g
                      select new Report_CuoiNgay_SoLuongHangHoa
                      {
                          GiaoDich = g.Key.LoaiHoaDon == 1 ? "Hóa đơn" : g.Key.LoaiHoaDon == 3 ? "Đặt hàng" : "Trả hàng",
                          SoLuongMatHang = g.GroupBy(x => x.dvqd.MaHangHoa).Count(),
                          SoLuongSanPham = g.Sum(x => x.hdct.SoLuong)
                      };
            var tbl_format = tbl.AsEnumerable().Select(t => new Report_CuoiNgay_SoLuongHangHoa
            {
                GiaoDich = t.GiaoDich,
                SoLuongMatHang = Math.Round(t.SoLuongMatHang, 3, MidpointRounding.ToEven),
                SoLuongSanPham = Math.Round(t.SoLuongSanPham, 3, MidpointRounding.ToEven)
            });
            try
            {
                lst = tbl_format.OrderByDescending(x => x.GiaoDich).ToList();
            }
            catch
            {

            }
            return lst;
        }
        //public static List<Report_NhomHangHoa> getList_ID_NhomHangHoa(List<Report_NhomHangHoa> lst, Guid? ID_NhomHang)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    Report_NhomHangHoa DM = new Report_NhomHangHoa();
        //    DM.ID_NhomHangHoa = ID_NhomHang;
        //    lst.Add(DM);
        //    var tb1 = from nh1 in db.DM_NhomHangHoa
        //              where nh1.ID_Parent == ID_NhomHang
        //              select new
        //              {
        //                  ID_NhomHangHoa = nh1.ID
        //              };
        //    foreach (var item in tb1)
        //    {
        //        ID_NhomHang = item.ID_NhomHangHoa;
        //        lst = getList_ID_NhomHangHoa(lst, ID_NhomHang);
        //    }
        //    return lst;
        //}
        // lấy danh sách nhóm hàng hóa theo tên nhóm
        
        //public static List<Report_NhomDoiTuong_ByName> getList_NhomDoiTuongs(string TenNhomDoiTuong, int LoaiDoiTuong)
        //{
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    List<Report_NhomDoiTuong_ByName> lst = new List<Report_NhomDoiTuong_ByName>();
        //    var tbl = from dt in db.DM_DoiTuong.Where(x => x.LoaiDoiTuong == LoaiDoiTuong)
        //              join dt_n in db.DM_DoiTuong_Nhom on dt.ID equals dt_n.ID_DoiTuong into dn
        //              from ntd in dn.DefaultIfEmpty()
        //              join dmn in db.DM_NhomDoiTuong on ntd.ID_NhomDoiTuong equals dmn.ID into d
        //              from dtn in d.DefaultIfEmpty()
        //              group new { dt, dtn } by new
        //              {
        //                  ID_NhomDoiTuong = dtn.ID,
        //                  dtn.TenNhomDoiTuong,
        //              } into g
        //              select new Report_NhomDoiTuong_ByName()
        //              {
        //                  ID = g.Key.ID_NhomDoiTuong != null ? g.Key.ID_NhomDoiTuong : new Guid("00000010-0000-0000-0000-000000000010"),
        //                  TenNhomDoiTuong = g.Key.TenNhomDoiTuong != null ? g.Key.TenNhomDoiTuong : "Nhóm Mặc Định"
        //              };
        //    var tbl_fomat = tbl.AsEnumerable().Select(t => new Report_NhomDoiTuong_ByName
        //    {
        //        ID = t.ID,
        //        TenNhomDoiTuong = t.TenNhomDoiTuong,
        //        TenNhomDoiTuong_KhongDau = CommonStatic.ConvertToUnSign(t.TenNhomDoiTuong).ToLower(),
        //        TenNhomDoiTuong_KyTuDau = CommonStatic.GetCharsStart(t.TenNhomDoiTuong).ToLower()
        //    }).OrderBy(x => x.ID);
        //    if (TenNhomDoiTuong != "" & TenNhomDoiTuong != "null" & TenNhomDoiTuong != null)
        //    {
        //        TenNhomDoiTuong = CommonStatic.ConvertToUnSign(TenNhomDoiTuong).ToLower();
        //        tbl_fomat = tbl_fomat.Where(x => x.TenNhomDoiTuong_KhongDau.Contains(@TenNhomDoiTuong) || x.TenNhomDoiTuong_KyTuDau.Contains(@TenNhomDoiTuong)).OrderBy(x => x.ID);
        //    }
        //    try
        //    {
        //        lst = tbl_fomat.ToList();
        //    }
        //    catch
        //    {
        //        lst = null;
        //    }
        //    return lst;
        //}
        
        // lấy danh sách phòng ban
        public static List<Report_PhongBan_byName> getList_ID_PhongBan_ByName(List<Report_PhongBan_byName> lst, string TenPhongBan)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            //Report_NhomHangHoa DM = new Report_NhomHangHoa();
            var tbl = from nh in db.NS_PhongBan
                      where nh.TrangThai == 1
                      select new Report_PhongBan_byName
                      {
                          ID = nh.ID,
                          TenPhongBan = nh.TenPhongBan,
                          ID_PhongBanCha = nh.ID_PhongBanCha,
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new
            {
                ID = t.ID,
                ID_PhongBanCha = t.ID_PhongBanCha,
                TenPhongBan = t.TenPhongBan,
                //NgayTao = t.NgayTao,
                TenPhongBan_CV = CommonStatic.ConvertToUnSign(t.TenPhongBan).ToLower(),
                TenPhongBan_GC = CommonStatic.GetCharsStart(t.TenPhongBan).ToLower(),
            });
            if (TenPhongBan != null & TenPhongBan != "" & TenPhongBan != "null")
            {
                TenPhongBan = CommonStatic.ConvertToUnSign(TenPhongBan).ToLower();
                tbl1 = tbl1.Where(x => x.TenPhongBan_CV.Contains(@TenPhongBan) || x.TenPhongBan_GC.Contains(@TenPhongBan));
                foreach (var item in tbl1)
                {
                    lst = getList_PhongBan(lst, item.ID, item.TenPhongBan, null);
                }
            }
            else
            {
                lst = tbl.ToList();
            }
            return lst;
        }
        public static List<Report_PhongBan_byName> getList_ID_PhongBan_ByNameIDChiNhanh(List<Report_PhongBan_byName> lst, string TenPhongBan, Guid ID_ChiNhanh)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from nh in db.NS_PhongBan
                      where nh.TrangThai == 1 && (nh.ID_DonVi == ID_ChiNhanh || nh.ID_DonVi == null)
                      select new Report_PhongBan_byName
                      {
                          ID = nh.ID,
                          TenPhongBan = nh.TenPhongBan,
                          ID_PhongBanCha = nh.ID_PhongBanCha,
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new
            {
                ID = t.ID,
                ID_PhongBanCha = t.ID_PhongBanCha,
                TenPhongBan = t.TenPhongBan,
                //NgayTao = t.NgayTao,
                TenPhongBan_CV = CommonStatic.ConvertToUnSign(t.TenPhongBan).ToLower(),
                TenPhongBan_GC = CommonStatic.GetCharsStart(t.TenPhongBan).ToLower(),
            });
            if (TenPhongBan != null & TenPhongBan != "" & TenPhongBan != "null")
            {
                TenPhongBan = CommonStatic.ConvertToUnSign(TenPhongBan).ToLower();
                tbl1 = tbl1.Where(x => x.TenPhongBan_CV.Contains(@TenPhongBan) || x.TenPhongBan_GC.Contains(@TenPhongBan));
                foreach (var item in tbl1)
                {
                    lst = getList_PhongBan(lst, item.ID, item.TenPhongBan, null);
                }
            }
            else
            {
                lst = tbl.ToList();
            }
            return lst;
        }
        public static List<Report_PhongBan_byName> getList_PhongBan(List<Report_PhongBan_byName> lst, Guid ID_PhongBan, string TenPhongBan, Guid? ID_PhongBanCha)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            Report_PhongBan_byName DM = new Report_PhongBan_byName();
            if (lst.Count() > 0)
            {
                for (int i = 0; i < lst.Count(); i++)
                {
                    if (lst[i].ID == ID_PhongBan)
                        break;
                    if (i == lst.Count() - 1)
                    {
                        DM.ID = ID_PhongBan;
                        DM.TenPhongBan = TenPhongBan;
                        DM.ID_PhongBanCha = ID_PhongBanCha;
                        lst.Add(DM);
                    }
                }
            }
            else
            {
                DM.ID = ID_PhongBan;
                DM.TenPhongBan = TenPhongBan;
                DM.ID_PhongBanCha = ID_PhongBanCha;
                lst.Add(DM);
            }
            var tb1 = from nh1 in db.NS_PhongBan
                      where nh1.ID_PhongBanCha == ID_PhongBan
                      select new Report_PhongBan_byName
                      {
                          ID = nh1.ID,
                          ID_PhongBanCha = nh1.ID_PhongBanCha,
                          TenPhongBan = nh1.TenPhongBan
                      };
            foreach (var item in tb1)
            {
                lst = getList_PhongBan(lst, item.ID, item.TenPhongBan, item.ID_PhongBanCha);
            }
            return lst;
        }
        public static List<Report_PhongBan> getList_ID_PhongBan(List<Report_PhongBan> lst, Guid? ID_PhongBan)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            Report_PhongBan DM = new Report_PhongBan();
            DM.ID_PhongBan = ID_PhongBan;
            lst.Add(DM);
            var tb1 = from nh1 in db.NS_PhongBan
                      where nh1.ID_PhongBanCha == ID_PhongBan
                      select new
                      {
                          ID_PhongBan = nh1.ID
                      };
            foreach (var item in tb1)
            {
                ID_PhongBan = item.ID_PhongBan;
                lst = getList_ID_PhongBan(lst, ID_PhongBan);
            }
            return lst;
        }
        //tinhlv
        public static IEnumerable<Object> getlistNhomHHByTenNhom(string TenNhomHang)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var tbl = from nh in db.DM_NhomHangHoa
                      where nh.TrangThai != true
                      select new
                      {
                          ID = nh.ID,
                          TenNhomHang = nh.TenNhomHangHoa,
                          ID_Parent = nh.ID_Parent,
                          LaNhomHangHoa = nh.LaNhomHangHoa
                      };
            var tbl1 = tbl.AsEnumerable().Select(t => new
            {
                ID = t.ID,
                ID_Parent = t.ID_Parent,
                TenNhomHangHoa = t.TenNhomHang,
                LaNhomHangHoa = t.LaNhomHangHoa,
                TenNhomHangHoa_CV = CommonStatic.ConvertToUnSign(t.TenNhomHang).ToLower(),
                TenNhomHangHoa_GC = CommonStatic.GetCharsStart(t.TenNhomHang).ToLower(),
            });
            if (TenNhomHang != null & TenNhomHang != "" & TenNhomHang != "null")
            {
                TenNhomHang = CommonStatic.ConvertToUnSign(TenNhomHang).ToLower();
                tbl1 = tbl1.Where(x => x.TenNhomHangHoa_CV.Contains(@TenNhomHang) || x.TenNhomHangHoa_GC.Contains(@TenNhomHang));
            }
            return tbl1;
        }
        #endregion

        public class Report_HangHoa_LoiNhuan
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaCV { get; set; }
            public string TenHangHoaGC { get; set; }
            public double SoLuongBan { get; set; }
            public double DoanhThu { get; set; }
            public double SoLuongTra { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
            public double TongGiaVon { get; set; }
            public double LoiNhuan { get; set; }
            public String TySuat { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public double TongGiaVonBan { get; set; }
            public double TongGiaVonTra { get; set; }
            public bool? LaHangHoa { get; set; }

        }
        
        public class Report_HangHoa_LoiNhuanPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public double SoLuongBan { get; set; }
            public double DoanhThu { get; set; }
            public double SoLuongTra { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
            public double TongGiaVon { get; set; }
            public double LoiNhuan { get; set; }
            public double TySuat { get; set; }
        }

        public class jqAutoResult_HangHoa
        {
            public string label { get; set; }
            public string value { get; set; }
            public string actual { get; set; }
            public Report_HangHoa_XuatNhapTon_Union data { get; set; }
        }
        public class jqAutoResult_HangHoa_Search
        {
            public string label { get; set; }
            public string value { get; set; }
            public string actual { get; set; }
            public Search_HangHoa_XuatNhapTonPRC data { get; set; }
        }
        public class jqAutoResult_HangHoaLoHang_Search
        {
            public string label { get; set; }
            public string value { get; set; }
            public string actual { get; set; }
            public Search_HangHoaLoHang_XuatNhapTonPRC data { get; set; }
        }
        public class Search_HangHoa_XuatNhapTonPRC
        {
            public Guid ID_DonViQuiDoi { get; set; }
            public string MaHangHoa { get; set; }
            public string MaLoHang { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public double GiaVon { get; set; }
            public double GiaBan { get; set; }
            public double TonCuoiKy { get; set; }
            public string SrcImage { get; set; }
        }
        public class Search_HangHoa_importPRC
        {
            public Guid ID_DonViQuiDoi { get; set; }
            public Guid? ID_GiaVon { get; set; }
            public Guid? ID_TonKho { get; set; }
            public Guid? ID_LoHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public double GiaVon { get; set; }
            public double GiaBan { get; set; }
            public double TonCuoiKy { get; set; }
        }
        public class Search_HangHoaLoHang_XuatNhapTonPRC
        {
            public Guid ID_DonViQuiDoi { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public bool? QuanLyTheoLoHang { get; set; }
            public double GiaVon { get; set; }
            public double GiaBan { get; set; }
            public double TonCuoiKy { get; set; }
            public double? TonKho { get; set; }
            public string SrcImage { get; set; }
            public Guid? ID_LoHang { get; set; }
            public string TenLoHang { get; set; }
            public string MaLoHang { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public DateTime? NgayHetHan { get; set; }
        }
        public class Report_HangHoa_XuatNhapTon_Union
        {
            public Guid ID_HangHoa { get; set; }
            public Guid ID_DonViQuiDoi { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public int QuanLyTheoLoHang { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenHangHoaCV { get; set; }
            public string TenHangHoaGC { get; set; }
            public double TonDauKy { get; set; }
            public double GiaTriDauKy { get; set; }
            public double SoLuongNhap { get; set; }
            public double GiaTriNhap { get; set; }
            public double SoLuongXuat { get; set; }
            public double GiaTriXuat { get; set; }
            public double TonCuoiKy { get; set; }
            public double GiaTriCuoiKy { get; set; }
            public double TyLeChuyenDoi { get; set; }
            public double GiaVon { get; set; }
            public double GiaBan { get; set; }
            public bool? LaHangHoa { get; set; }
            public bool LaDonViChuan { get; set; }
            public double TonDau_SoLuongNhap { get; set; }
            public double TonDau_SoLuongXuat { get; set; }
        }

        public class Report_HangHoa_XuatNhapTon
        {
            public Guid ID_HangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaCV { get; set; }
            public string TenHangHoaGC { get; set; }
            public double TonDauKy { get; set; }
            public double GiaTriDauKy { get; set; }
            public double SoLuongNhap { get; set; }
            public double GiaTriNhap { get; set; }
            public double SoLuongXuat { get; set; }
            public double GiaTriXuat { get; set; }
            public double TonCuoiKy { get; set; }
            public double GiaTriCuoiKy { get; set; }
            public double TyLeChuyenDoi { get; set; }
            public double GiaVon { get; set; }
        }
        public class ReportHangHoa_XuatNhapTonPRC
        {
            public Guid ID_HangHoa { get; set; }
            public Guid ID_DonViQuiDoi { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double TonDauKy { get; set; }
            public double GiaTriDauKy { get; set; }
            public double SoLuongNhap { get; set; }
            public double GiaTriNhap { get; set; }
            public double SoLuongXuat { get; set; }
            public double GiaTriXuat { get; set; }
            public double TonCuoiKy { get; set; }
            public double GiaTriCuoiKy { get; set; }
        }
        public class ReportHangHoa_TonKhoPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string TenNhomHangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double TonCuoiKy { get; set; }
            public double GiaTriCuoiKy { get; set; }
        }
        public class ReportHangHoa_TongHopNhapKhoPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string TenNhomHangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongNhap { get; set; }
            public double GiaTriNhap { get; set; }
        }
        public class ReportHangHoa_TongHopNhapKhoChiTietPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string TenLoaiChungTu { get; set; }
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenNhomHangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongNhap { get; set; }
            public double GiaTriNhap { get; set; }
        }
        public class ReportHangHoa_TongHopXuatKhoPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string TenNhomHangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongXuat { get; set; }
            public double GiaTriXuat { get; set; }
        }
        public class ReportHangHoa_TongHopXuatKhoChiTietPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string TenLoaiChungTu { get; set; }
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenNhomHangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongXuat { get; set; }
            public double GiaTriXuat { get; set; }
        }
        public class Report_PhongBan
        {
            public Guid? ID_PhongBan { get; set; }
        }
        
        public class Report_PhongBan_byName
        {
            public Guid ID { get; set; }
            public string TenPhongBan { get; set; }
            public Guid? ID_PhongBanCha { get; set; }
        }
        
        public class Report_HangHoa_XuatNhapTonChiTiet
        {
            public Guid ID_HangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaGC { get; set; }
            public string TenHangHoaCV { get; set; }
            public double TonDauKy { get; set; }
            public double GiaTriDauKy { get; set; }
            public double NCCNhap { get; set; }
            public double KiemNhap { get; set; }
            public double TraNhap { get; set; }
            public double ChuyenNhap { get; set; }
            public double SxNhap { get; set; }
            public double BanXuat { get; set; }
            public double HuyXuat { get; set; }
            public double NCCXuat { get; set; }
            public double KiemXuat { get; set; }
            public double ChuyenXuat { get; set; }
            public double SxXuat { get; set; }
            public double TonCuoiKy { get; set; }
            public double GiaTriCuoiKy { get; set; }
            public double TyLeChuyenDoi { get; set; }
            public double GiaVon { get; set; }
        }
        public class Report_HangHoa_XuatNhapTonChiTietPRC
        {
            public Guid ID_HangHoa { get; set; }
            public Guid ID_DonViQuiDoi { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public bool LaDonViChuan { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double TonDauKy { get; set; }
            public double GiaTriDauKy { get; set; }
            public double SoLuongNhap_NCC { get; set; }
            public double SoLuongNhap_Kiem { get; set; }
            public double SoLuongNhap_Tra { get; set; }
            public double SoLuongNhap_Chuyen { get; set; }
            public double SoLuongNhap_SX { get; set; }
            public double SoLuongXuat_Ban { get; set; }
            public double SoLuongXuat_Huy { get; set; }
            public double SoLuongXuat_NCC { get; set; }
            public double SoLuongXuat_Kiem { get; set; }
            public double SoLuongXuat_Chuyen { get; set; }
            public double SoLuongXuat_SX { get; set; }
            public double TonCuoiKy { get; set; }
            public double GiaTriCuoiKy { get; set; }
        }
        public class Report_HangHoa_XuatHuy
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaGC { get; set; }
            public string TenHangHoaCV { get; set; }
            public double TongSoLuongHuy { get; set; }
            public double TongGiaTriHuy { get; set; }
        }
        public class Report_HangHoa_XuatHuyPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public double TongSoLuongHuy { get; set; }
            public double TongGiaTriHuy { get; set; }
        }
        public class Report_HangHoa_TraHangNhapPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public double SoLuong { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_TraHangNhapChiTietPRC
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public double SoLuong { get; set; }
            public double DonGia { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_ChuyenHangPRC
        {
            public Guid ID { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuong { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_HDChuyenHangPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public string ChiNhanhChuyen { get; set; }
            public string ChiNhanhNhan { get; set; }
            public double SoLuong { get; set; }
            public double DonGia { get; set; }
            public double ThanhTien { get; set; }
        }
        public class Report_HangHoa_ChuyenHangChiTietPRC
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TuDonVi { get; set; }
            public string ToiDonVi { get; set; }
            public double SoLuong { get; set; }
            public double DonGia { get; set; }
            public double ThanhTien { get; set; }
            public string TrangThai { get; set; }
        }
        public class Report_HangHoa_NhanVien
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaGC { get; set; }
            public string TenHangHoaCV { get; set; }
            public double SoluongNhanVien { get; set; }
            public double SoLuongBan { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_NhanVienPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public int SoLuongNhanVien { get; set; }
            public double SoLuongBan { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_KhachHang
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaGC { get; set; }
            public string TenHangHoaCV { get; set; }
            public double SoluongKhachHang { get; set; }
            public double SoLuongMua { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_KhachHangPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public int SoLuongKhachHang { get; set; }
            public double SoLuongMua { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_NhaCungCap
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaGC { get; set; }
            public string TenHangHoaCV { get; set; }
            public double SoluongNhaCungCap { get; set; }
            public double SoLuongSanPham { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangHoa_NhaCungCapPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public int SoLuongNhaCungCap { get; set; }
            public double SoLuongSanPham { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_NhanVien_BanHang
        {
            public string TenNhanVien { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaGC { get; set; }
            public string TenHangHoaCV { get; set; }
            public Guid ID_NhomHang { get; set; }
            public Guid ID_NhanVien { get; set; }
            public bool LaHangHoa { get; set; }
        }
        public class Report_NhanVien_BanHangPRC
        {
            public Guid ID_NhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
        }

        public class Report_NhanVien_LoiNhuan
        {
            public string TenNhanVien { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
            public double TongGiaVon { get; set; }
            public double LoiNhuanGop { get; set; }
        }
        public class Report_NhanVien_LoiNhuanPRC
        {
            public Guid ID_NhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
            public double TongGiaVon { get; set; }
            public double LoiNhuanGop { get; set; }
        }
        public class Report_HangBan_NhanVien
        {
            public Guid ID_NhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoaGC { get; set; }
            public string TenHangHoaCV { get; set; }
            public double SoLuong { get; set; }
            public double GiaTri { get; set; }
            public bool? LaHangHoa { get; set; }
        }
        public class Report_NhanVien_MuaHangChiTietPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuong { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_HangBan_NhanVienPRC
        {
            public Guid ID_NhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public double SoLuong { get; set; }
            public double GiaTri { get; set; }
            public Guid? ID_NhomHang { get; set; }
        }
        public class Report_NCC_NhapHang
        {
            public Guid? ID_NCC { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public double GiaTriNhap { get; set; }
            public double GiaTriTra { get; set; }
            public double GiaTriThuan { get; set; }
            public string MaPhieu { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenNCC_CV { get; set; }
            public string TenNNC_GC { get; set; }
            public double SoLuongSanPham { get; set; }
            public double TongGiaTri { get; set; }

        }
        public class Report_NCC_NhapHangPRC
        {
            public Guid? ID_NCC { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public string TenDoiTuong_KhongDau { get; set; }
            public string TenDoiTuong_ChuCaiDau { get; set; }
            public double GiaTriNhap { get; set; }
            public double GiaTriTra { get; set; }
            public double GiaTriThuan { get; set; }
            public Guid ID_NhomDoiTuong { get; set; }
        }
        public class Report_NCC_NhapHangChiTietPRC
        {
            public string MaPhieu { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public double SoLuongSanPham { get; set; }
            public double TongGiaTri { get; set; }
        }
        public class Report_NCC_CongNo
        {
            public Guid? ID_NCC { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public double NoDauKy { get; set; }
            public double GhiNo { get; set; }
            public double GhiCo { get; set; }
            public double NoCuoiKy { get; set; }
            public string TenNCC_CV { get; set; }
            public string TenNNC_GC { get; set; }
        }
        public class Report_NCC_CongNoChiTiet
        {
            public string MaHoaDon { get; set; }
            public DateTime ThoiGian { get; set; }
            public string LoaiPhieu { get; set; }
            public double GiaTri { get; set; }
            public double DuNoCuoi { get; set; }
        }
        public class Report_NCC_MuaHang
        {
            public Guid? ID_NCC { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public double SoLuongSanPham { get; set; }
            public double GiaTri { get; set; }
            public string TenNCC_CV { get; set; }
            public string TenNNC_GC { get; set; }
        }
        public class Report_NCC_MuaHangPRC
        {
            public Guid? ID_NCC { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public double SoLuongSanPham { get; set; }
            public double GiaTri { get; set; }
            public string TenDoiTuong_KhongDau { get; set; }
            public string TenDoiTuong_ChuCaiDau { get; set; }
            public Guid ID_NhomDoiTuong { get; set; }
        }
        public class Report_NCC_MuaHangChiTiet
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public double SoLuongSanPham { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_NCC_MuaHangChiTietPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongSanPham { get; set; }
            public double GiaTri { get; set; }
        }
        public class Report_KhachHang_BanHang
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenKhachHangCV { get; set; }
            public string TenKhachHangGC { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
        }
        public class Report_KhachHang_BanHangPRC
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenDoiTuong_KhongDau { get; set; }
            public string TenDoiTuong_ChuCaiDau { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
            public Guid ID_NhomDoiTuong { get; set; }
        }
        public class Report_KhachHang_BanHangChiTiet
        {
            public string MaHoaDon { get; set; }
            public DateTime ThoiGian { get; set; }
            public double DoanhThu { get; set; }
        }
        public class Report_KhachHang_BanHangChiTietPRC
        {
            public string MaHoaDon { get; set; }
            public DateTime ThoiGian { get; set; }
            public double SoLuong { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThu { get; set; }
        }
        public class Report_KhachHang_LoiNhuan
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenKhachHangCV { get; set; }
            public string TenKhachHangGC { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThuThuan { get; set; }
            public double TongGiaVon { get; set; }
            public double LoiNhuanGop { get; set; }

        }
        public class Report_KhachHang_LoiNhuanPRC
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenDoiTuong_KhongDau { get; set; }
            public string TenDoiTuong_ChuCaiDau { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThu { get; set; }
            public double GiaTriTra { get; set; }
            public double GiaTriThuan { get; set; }
            public double TongGiaVon { get; set; }
            public double LoiNhuanGop { get; set; }
            public Guid ID_NhomDoiTuong { get; set; }
        }
        public class Report_KhachHang_CongNo
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public double NoDauKy { get; set; }
            public double GhiNo { get; set; }
            public double GhiCo { get; set; }
            public double NoCuoiKy { get; set; }
            public string TenKhachHang_CV { get; set; }
            public string TenKhachHang_GC { get; set; }
        }
        public class Report_KhachHang_CongNoPRC
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public double NoDauKy { get; set; }
            public double GhiNo { get; set; }
            public double GhiCo { get; set; }
            public double NoCuoiKy { get; set; }
            public Guid ID_NhomDoiTuong { get; set; }
        }

        public class Report_KhachHang_CongNoChiTiet
        {
            public string MaHoaDon { get; set; }
            public DateTime ThoiGian { get; set; }
            public string LoaiPhieu { get; set; }
            public double GiaTri { get; set; }
            public double DuNoCuoi { get; set; }
        }
        public class Report_KhachHang_MuaHang
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public double SoLuongMua { get; set; }
            public double GiaTriMua { get; set; }
            public double SoLuongTra { get; set; }
            public double GiaTriTra { get; set; }
            public double GiaTriThuan { get; set; }
            public string TenKhachHang_CV { get; set; }
            public string TenKhachHang_GC { get; set; }
        }
        public class Report_KhachHang_MuaHangPRC
        {
            public Guid? ID_KhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenDoiTuong_KhongDau { get; set; }
            public string TenDoiTuong_ChuCaiDau { get; set; }
            public double TongTichDiem { get; set; }
            public double SoLuongMua { get; set; }
            public double GiaTriMua { get; set; }
            public double SoLuongTra { get; set; }
            public double GiaTriTra { get; set; }
            public double GiaTriThuan { get; set; }
            public Guid ID_NhomDoiTuong { get; set; }
        }
        public class Report_KhachHang_MuaHangChiTiet
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public double SoLuongMua { get; set; }
            public double GiaTriMua { get; set; }
            public double SoLuongTra { get; set; }
            public double GiaTriTra { get; set; }
            public double GiaTriThuan { get; set; }
        }
        public class Report_KhachHang_MuaHangChiTietPRC
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongMua { get; set; }
            public double GiaTriMua { get; set; }
            public double SoLuongTra { get; set; }
            public double GiaTriTra { get; set; }
            public double GiaTriThuan { get; set; }
        }
        public class Report_DatHang_HangHoa
        {
            public Guid? ID_HangHoa { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public double SoLuongDat { get; set; }
            public double GiaTriDat { get; set; }
            public string TenHangHoa_CV { get; set; }
            public string TenHangHoa_GC { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public Guid? ID_NhanVien { get; set; }
            public bool LaHangHoa { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenKhachHang_CV { get; set; }
            public string TenKhachHang_GC { get; set; }
        }
        public class Report_DatHang_HangHoaPRC
        {
            public Guid ID_DonViQuiDoi { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongDat { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double GiaTriDat { get; set; }
            public Guid? ID_NhomHang { get; set; }
        }
        public class Report_DatHang_HangHoaChiTiet
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenKhachHang { get; set; }
            public double SoLuongDat { get; set; }
            public double GiaTriDat { get; set; }
        }
        public class Report_DatHang_HangHoaChiTietPRC
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenKhachHang { get; set; }
            public double SoLuongDat { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double GiaTriDat { get; set; }
        }
        public class Report_DatHang_GiaoDich
        {
            public Guid ID_HoaDon { get; set; }
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenKhachHang { get; set; }
            public double SoLuongDat { get; set; }
            public double GiaTriDat { get; set; }
            public Guid? ID_NhomHang { get; set; }
            public Guid? ID_NhanVien { get; set; }
            public bool LaHangHoa { get; set; }
            public string MaKhachHang { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoa_CV { get; set; }
            public string TenHangHoa_GC { get; set; }
            public string TenKhachHang_CV { get; set; }
            public string TenKhachHang_GC { get; set; }
            public string MaHangHoa { get; set; }
        }
        public class Report_DatHang_GiaoDichPRC
        {
            public Guid ID_HoaDon { get; set; }
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenKhachHang { get; set; }
            public double SoLuongDat { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double GiaTriDat { get; set; }
            public Guid? ID_NhomHang { get; set; }
        }
        public class Report_DatHang_GiaoDichChiTiet
        {
            public Guid? ID_NhomHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public string TenHangHoa_CV { get; set; }
            public string TenHangHoa_GC { get; set; }
            public double SoLuongDat { get; set; }
            public double SoLuongNhan { get; set; }
        }
        public class Report_DatHang_GiaoDichChiTietPRC
        {
            public Guid? ID_NhomHang { get; set; }
            public string MaHangHoa { get; set; }
            public string TenHangHoaFull { get; set; }
            public string TenHangHoa { get; set; }
            public string ThuocTinh_GiaTri { get; set; }
            public string TenDonViTinh { get; set; }
            public string TenLoHang { get; set; }
            public double SoLuongDat { get; set; }
            public double SoLuongNhan { get; set; }
        }
        
        public class Report_CuoiNgay_BanHang
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public double SoLuongSanPham { get; set; }
            public double DoanhThu { get; set; }
            public double ThuKhac { get; set; }
            public double PhiTraHang { get; set; }
            public double ThucThu { get; set; }
            public int LoaiHoaDon { get; set; }
        }
        public class Report_CuoiNgay_BanHangPRC
        {
            public string MaHoaDon { get; set; }
            public double SoLuongSanPham { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThu { get; set; }
            public double PhiTraHang { get; set; }
            public double ThucThu { get; set; }
            public int LoaiHoaDon { get; set; }
        }
        public class Report_CuoiNgay_BanHangChiTietPRC
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public double SoLuongSanPham { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThu { get; set; }
            public double PhiTraHang { get; set; }
            public double ThucThu { get; set; }
        }

        public class Report_CuoiNgay_Union
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenKhachHang_GC { get; set; }
            public string TenKhachHang_CV { get; set; }
            public double SoLuong { get; set; }
            public double TongTienHang { get; set; }
            public double TongChiPhi { get; set; }
            public double PhaiThanhToan { get; set; }
            public double TongTienThu { get; set; }
            public int LoaiHoaDon { get; set; }
        }
        public class Report_CuoiNgay_PhieuThu
        {
            public string MaPhieuThu { get; set; }
            public string TenNguoiNop { get; set; }
            public double ThuChi { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string MaHoaDon { get; set; }
        }
        public class Report_CuoiNgay_PhieuThuPRC
        {
            public string MaPhieuThu { get; set; }
            public string TenNguoiNop { get; set; }
            public double ThuChi { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string MaHoaDon { get; set; }
        }
        public class Report_CuoiNgay_HangHoa
        {
            public string MaHangHoa { get; set; }
            public string TenHangHoa { get; set; }
            public double SoLuongBan { get; set; }
            public double GiaTriBan { get; set; }
            public double SoLuongTra { get; set; }
            public double GiaTriTra { get; set; }
            public double DoanhThu { get; set; }
        }
        public class Report_CuoiNgay_HangHoaChiTiet
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenKhachHang { get; set; }
            public double SoLuongSanPham { get; set; }
            public double DoanhThu { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang_GC { get; set; }
            public string TenKhachHang_CV { get; set; }
        }
        public class Report_CuoiNgay_HangHoaChiTietPRC
        {
            public string MaHoaDon { get; set; }
            public DateTime NgayLapHoaDon { get; set; }
            public string TenKhachHang { get; set; }
            public double SoLuongSanPham { get; set; }
            public double TongTienHang { get; set; }
            public double GiamGiaHD { get; set; }
            public double DoanhThuThuan { get; set; }
        }
        public class Report_CuoiNgay_TongHop
        {
            public string ThuChi { get; set; }
            public double TienMat { get; set; }
            public double ChuyenKhoan { get; set; }
            public double The { get; set; }
            public double Diem { get; set; }
            public double TongThucThu { get; set; }
        }
        public class Report_CuoiNgay_TongKetThuChiPRC
        {
            public string ThuChi { get; set; }
            public double TienMat { get; set; }
            public double ChuyenKhoan { get; set; }
            public double TienThe { get; set; }
            public double TienDiem { get; set; }
            public double TongThucThu { get; set; }
        }
        public class Report_CuoiNgay_TongKetBanHang
        {
            public string GiaoDich { get; set; }
            public double GiaTri { get; set; }
            public double TienMat { get; set; }
            public double ChuyenKhoan { get; set; }
            public double The { get; set; }
            public double Diem { get; set; }
            public double TongThucThu { get; set; }
        }
        public class Report_CuoiNgay_TongKetBanHangPRC
        {
            public string GiaoDich { get; set; }
            public double TongTienHang { get; set; }
            public double TienMat { get; set; }
            public double ChuyenKhoan { get; set; }
            public double TienThe { get; set; }
            public double TienDiem { get; set; }
            public double TongThucThu { get; set; }
        }
        public class Report_CuoiNgay_SoLuongGiaoDich
        {
            public string GiaoDich { get; set; }
            public double SoGiaoDich { get; set; }
            public double TienMat { get; set; }
            public double ChuyenKhoan { get; set; }
            public double The { get; set; }
            public double Diem { get; set; }
        }
        public class Report_CuoiNgay_SoLuongGiaoDichPRC
        {
            public string GiaoDich { get; set; }
            public double SoGiaoDich { get; set; }
            public double TienMat { get; set; }
            public double ChuyenKhoan { get; set; }
            public double TienThe { get; set; }
            public double TienDiem { get; set; }
        }
        public class Report_CuoiNgay_SoLuongHangHoa
        {
            public string GiaoDich { get; set; }
            public double SoLuongMatHang { get; set; }
            public double SoLuongSanPham { get; set; }
        }
        public class Report_CuoiNgay_SoLuongHangHoaPRC
        {
            public string GiaoDich { get; set; }
            public double SoLuongHangHoa { get; set; }
            public double SoLuongSanPham { get; set; }
        }
        
        public class Report_BieuDoi_Chart
        {
            public string Columnss { get; set; }
            public double Rowsn { get; set; }
        }
        public class Report_BieuDoi_BanHang
        {
            public string Datetime { get; set; }
            public string TenDonVi { get; set; }
            public double ThanhTien { get; set; }
        }
        #region TrinhPV Report_Open24

        public class Report_DoiTuongPRC
        {
            public Guid? ID { get; set; }
            public string TenDoiTuong { get; set; }
            public string TenDoiTuong_KhongDau { get; set; }
            public string TenDoiTuong_KyTuDau { get; set; }
        }
        
        
        #endregion
    }
}




