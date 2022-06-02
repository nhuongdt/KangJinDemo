using banhang24.Models;
using libHT_NguoiDung;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class ApiOpen24HRMController : BaseApiController
    {

        public IHttpActionResult GetNhanSu(Guid? donviId,DateTime? date)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                string text = "Tóm tắt tình hình nhan sự ngày hôm nay";
                if (date == null)
                {
                    date = DateTime.Now;

                }
                else if (date.Value.Date != DateTime.Now.Date)
                {
                    text = "Tóm tắt tình hình nhan sự ngày " +date.Value.ToString("dd/MM/yyyy");
                }
                var result = (from o in db.NS_NhanVien
                              join p in db.NS_QuaTrinhCongTac
                                 on o.ID equals p.ID_NhanVien
                              where p.ID_DonVi == donviId
                                    && (o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa || o.TrangThai == null)
                                    && o.DaNghiViec == false
                              select o).Distinct();
                var start = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 00, 00, 00);
                var end = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 23, 59, 59);
                result = result.Where(o => o.NgayVaoLamViec.HasValue && o.NgayVaoLamViec >= start && o.NgayVaoLamViec <= end);
                return ActionTrueData(new { title= text,all = result.Count(), boy = result.Where(o => o.GioiTinh).Count(), girl = result.Where(o => !o.GioiTinh).Count() });
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        public IHttpActionResult GetChartColumnDashBoard(Guid? donviId,int? type)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                var    date = DateTime.Now;
                var start = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
                var end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                switch (type) {
                    case (int)commonEnumHellper.TypeTimeChart.homnay:
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.homqua:
                        date = date.AddDays(-1);
                         start = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
                         end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.thangnay:
                        start = new DateTime(date.Year, date.Month, 1, 00, 00, 00);
                        end = new DateTime(date.Year, date.Month,DateTime.DaysInMonth(date.Year,date.Month), 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.thangtruoc:
                        date = date.AddMonths(-1);
                        start = new DateTime(date.Year, date.Month, 1, 00, 00, 00);
                        end = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.namnay:
                        start = new DateTime(date.Year, 1, 1, 00, 00, 00);
                        end = new DateTime(date.Year, 12, 31, 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.namtruoc:
                        date = date.AddYears(-1);
                        start = new DateTime(date.Year,1, 1, 00, 00, 00);
                        end = new DateTime(date.Year,12,31, 23, 59, 59);
                        break;
                    default:
                        break;

                }
                
                
                var result = db.NS_HopDong.Where(o => o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa)
                    .Where(o => o.NgayKy >= start && o.NgayKy <= end).AsEnumerable().GroupBy(o=>o.LoaiHopDong).Select(o=> new 
                    {
                        name= GetNameHopDong(o.Key),
                        y=o.Count(),
                        drilldown=o.Key
                    }).AsEnumerable();
                return ActionTrueData(result);
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }

        private string GetNameHopDong(int type)
        {
            if (commonEnumHellper.ListLoaiHopDong.Any(o => o.Key == type))
                return commonEnumHellper.ListLoaiHopDong.FirstOrDefault(o => o.Key == type).Value;
            return string.Empty;
        }

        public IHttpActionResult GetChartLineDashBoard(Guid? donviId,int? type)
        {
            try
            {
                if (type == null)
                {
                    type = (int)commonEnumHellper.TypeTimeChart.thangnay;
                }
                SsoftvnContext db = SystemDBContext.GetDBContext();
                var date = DateTime.Now;
                var start = new DateTime(date.Year, date.Month, 1, 00, 00, 00);
                var end = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
                switch (type)
                {
                    case (int)commonEnumHellper.TypeTimeChart.thangnay:
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.thangtruoc:
                        date = date.AddMonths(-1);
                        start = new DateTime(date.Year, date.Month, 1, 00, 00, 00);
                        end = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.namnay:
                        start = new DateTime(date.Year, 1, 1, 00, 00, 00);
                        end = new DateTime(date.Year, 12, 31, 23, 59, 59);
                        break;
                    case (int)commonEnumHellper.TypeTimeChart.namtruoc:
                        date = date.AddYears(-1);
                        start = new DateTime(date.Year, 1, 1, 00, 00, 00);
                        end = new DateTime(date.Year, 12, 31, 23, 59, 59);
                        break;
                    default:
                        break;

                }
                var dsNhanVienDonVi = (from o in db.NS_NhanVien
                              join p in db.NS_QuaTrinhCongTac
                                 on o.ID equals p.ID_NhanVien
                              where p.ID_DonVi == donviId
                                    && (o.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa || o.TrangThai == null)
                                    && o.DaNghiViec==false
                              select o).Distinct();

                var result = from x in db.NS_PhongBan
                             join o in dsNhanVienDonVi
                             on x.ID equals o.ID_NSPhongBan
                           where (x.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa)
                              && o.NgayVaoLamViec >= start && o.NgayVaoLamViec <= end
                          select new {
                              x.ID,
                              x.TenPhongBan,
                              o.NgayVaoLamViec };
                var data = result.AsEnumerable().GroupBy(o => new { o.ID, o.NgayVaoLamViec, o.TenPhongBan }).Select(o => new {
                    o.Key.ID,
                    o.Key.TenPhongBan,
                    o.Key.NgayVaoLamViec,
                    count = o.Count()
                }).AsEnumerable();
                var model = new List<ChartOpenHRMModel>();
                string pointFormat = string.Empty;
                   if (type == (int)commonEnumHellper.TypeTimeChart.thangnay || type == (int)commonEnumHellper.TypeTimeChart.thangtruoc)
                        {
                        pointFormat = "<b>Ngày: <span style='color:var(--color-main);'>{point.x}</span></b><br /> <b>Số nhân viên : <span style='color:var(--color-main);'>{point.y} người</span></b>";
                             var ten = data.GroupBy(o =>new { o.ID, o.TenPhongBan }).Select(o=>o.Key).ToList();
                            foreach (var item in ten)
                            {
                                var ds = data.Where(o => o.ID == item.ID);
                                var listData = new List<int>() {};
                                  foreach(var it in Enumerable.Range(1,DateTime.DaysInMonth(date.Year, date.Month)))
                                    {
                                    if (data.Any(o => o.ID == item.ID && o.NgayVaoLamViec.Value.Day == it))
                                        listData.Add(data.FirstOrDefault(o => o.ID == item.ID && o.NgayVaoLamViec.Value.Day == it).count);
                                    else
                                        listData.Add(0);
                                    }
                                model.Add(new ChartOpenHRMModel()
                                {
                                    name = item.TenPhongBan,
                                    data=listData,
                                    
                                });
                            }
                            return ActionTrueData(new { data = model, pointFormat = pointFormat });
                        }

                        else
                        {
                    pointFormat = "<b>Tháng: <span style='color:var(--color-main);'>{point.x}</span></b><br /> <b>Số nhân viên : <span style='color:var(--color-main);'>{point.y} người</span></b>";
                    var ten = data.GroupBy(o => new { o.ID, o.TenPhongBan }).Select(o => o.Key).ToList();
                            foreach (var item in ten)
                            {
                                var ds = data.Where(o => o.ID == item.ID);
                                var listData = new List<int>();
                                  for(int j=1;j<=12;j++)
                                    {
                                        if (data.Any(o => o.ID == item.ID && o.NgayVaoLamViec.Value.Month == j))
                                            listData.Add(data.FirstOrDefault(o => o.ID == item.ID && o.NgayVaoLamViec.Value.Month == j).count);
                                        else
                                            listData.Add(0);
                                    }
                                    model.Add(new ChartOpenHRMModel()
                                    {
                                        name = item.TenPhongBan,
                                        data = listData,

                                    });
                            }
                            return ActionTrueData(new { data = model, pointFormat= pointFormat });
                        }
                   
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }

        }

        public IHttpActionResult GetHoatDongHRM(Guid? donviId)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                var dsHoatDongNhanVien = (from o in db.HT_NhatKySuDung
                                       join p in db.NS_NhanVien
                                          on o.ID_NhanVien equals p.ID
                                       where o.ID_DonVi == donviId
                                          && (o.LoaiNhatKy == (int)commonEnumHellper.TypeHoatDong.insert
                                                                      || o.LoaiNhatKy == (int)commonEnumHellper.TypeHoatDong.update
                                                                      || o.LoaiNhatKy == (int)commonEnumHellper.TypeHoatDong.delete)
                                       select new {
                                            TenNhanVien=p.TenNhanVien,
                                            MaNhanVien=p.MaNhanVien,
                                               o.LoaiNhatKy,
                                               o.NoiDung,
                                               o.ChucNang,
                                               o.ThoiGian,
                                               o.ID
                                       });
                var result = dsHoatDongNhanVien.OrderByDescending(o => o.ThoiGian).Take(10).AsEnumerable()
                                               .Select(o => new { o.ID, o.NoiDung,o.TenNhanVien, ThoiGian = o.ThoiGian }).AsEnumerable();
                return ActionTrueData(result);
            }
            catch (Exception exx)
            {
                return Exeption(exx);
            }
        }
        private string ConvertnameHoatDong(int? type)
        {
            if(type== (int)commonEnumHellper.TypeHoatDong.insert)
            {
                return "đã thực hiện thao tác Thêm mới nhân viên";
            }
            else if (type == (int)commonEnumHellper.TypeHoatDong.update)
            {
                return "đã thực hiện thao tác Cập nhật nhân viên";
            }
            else if (type == (int)commonEnumHellper.TypeHoatDong.delete)
            {
                return "đã thực hiện thao tác xóa nhân viên";
            }
            return string.Empty;
        }
    }
}
