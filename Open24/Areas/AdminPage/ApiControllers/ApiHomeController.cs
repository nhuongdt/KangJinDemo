using log4net;
using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using Newtonsoft.Json.Linq;
using Open24.Areas.AdminPage.Hellper;
using Open24.Hellper;
using Open24.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Http;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiHomeController : ApiBaseController
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private CuaHangDangKyService _CuaHangDangKyService = new CuaHangDangKyService();
        private NewPostService _NewPostService = new NewPostService();
        private ContactService _ContactService = new ContactService();
        private AdvertisingService _AdvertisingService = new AdvertisingService();
        private NotificationSoftwareService _NotificationSoftwareService = new NotificationSoftwareService();
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        private RegisterServiceSmsService _RegisterServiceSmsService = new RegisterServiceSmsService();

        [HttpGet]
        public IHttpActionResult GetCount(int? filter, DateTime? tungay, DateTime? denngay)
        {
            try
            {
                long countStore = 0;
                long countNews = 0;
                filter = filter == null ? (int)Notification.selectTime.homnay : filter;
                if (filter == (int)Notification.selectTime.homnay)
                {
                    countStore = _CuaHangDangKyService.GetCountStoreagrea(DateTime.Now);
                    countNews = _NewPostService.GetCountStoreagrea(DateTime.Now);
                }
                else if (filter == (int)Notification.selectTime.homqua)
                {
                    countStore = _CuaHangDangKyService.GetCountStoreagrea(DateTime.Now.AddDays(-1));
                    countNews = _NewPostService.GetCountStoreagrea(DateTime.Now.AddDays(-1));
                }
                else if (filter == (int)Notification.selectTime.namnay)
                {
                    countStore = _CuaHangDangKyService.GetCountForYear(DateTime.Now.Year);
                    countNews = _NewPostService.GetCountForYear(DateTime.Now.Year);
                }
                else if (filter == (int)Notification.selectTime.namtruoc)
                {
                    countStore = _CuaHangDangKyService.GetCountForYear(DateTime.Now.AddYears(-1).Year);
                    countNews = _NewPostService.GetCountForYear(DateTime.Now.AddYears(-1).Year);
                }
                else if (filter == (int)Notification.selectTime.quynay)
                {
                    int quy = 4;
                    if (DateTime.Now.Month <= 3)
                    {
                        quy = 1;
                    }
                    else if (DateTime.Now.Month > 3 && DateTime.Now.Month <= 6)
                    {
                        quy = 2;
                    }
                    else if (DateTime.Now.Month > 6 && DateTime.Now.Month <= 9)
                    {

                        quy = 3;
                    }
                    countStore = _CuaHangDangKyService.GetCountForQuy(DateTime.Now.Year, quy);
                    countNews = _NewPostService.GetCountForQuy(DateTime.Now.Year, quy);
                }
                else if (filter == (int)Notification.selectTime.quytruoc)
                {
                    if (DateTime.Now.Month <= 3)
                    {
                        countStore = _CuaHangDangKyService.GetCountForQuy((DateTime.Now.Year - 1), 4);
                        countNews = _NewPostService.GetCountForQuy((DateTime.Now.Year - 1), 4);
                    }
                    else if (DateTime.Now.Month > 3 && DateTime.Now.Month <= 6)
                    {
                        countStore = _CuaHangDangKyService.GetCountForQuy(DateTime.Now.Year, 1);
                        countNews = _NewPostService.GetCountForQuy(DateTime.Now.Year, 1);
                    }
                    else if (DateTime.Now.Month > 6 && DateTime.Now.Month <= 9)
                    {
                        countStore = _CuaHangDangKyService.GetCountForQuy(DateTime.Now.Year, 2);
                        countNews = _NewPostService.GetCountForQuy(DateTime.Now.Year, 2);
                    }
                    else
                    {
                        countStore = _CuaHangDangKyService.GetCountForQuy(DateTime.Now.Year, 3);
                        countNews = _NewPostService.GetCountForQuy(DateTime.Now.Year, 3);
                    }
                }
                else if (filter == (int)Notification.selectTime.thangnay)
                {
                    countStore = _CuaHangDangKyService.GetCountForMonth(DateTime.Now.Year, DateTime.Now.Month);
                    countNews = _NewPostService.GetCountMonth(DateTime.Now.Year, DateTime.Now.Month);
                }
                else if (filter == (int)Notification.selectTime.thangtruoc)
                {
                    var day = DateTime.Now.AddMonths(-1);
                    countStore = _CuaHangDangKyService.GetCountForMonth(day.Year, day.Month);
                    countNews = _NewPostService.GetCountMonth(day.Year, day.Month);
                }
                else if (filter == (int)Notification.selectTime.tuannay)
                {
                    var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
                    var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek);
                    startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1));
                    countStore = _CuaHangDangKyService.GetCountForStage(startday, Endday);
                    countNews = _NewPostService.GetCountForStage(startday, Endday);
                }
                else if (filter == (int)Notification.selectTime.tuantruoc)
                {
                    var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
                    var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek).AddDays(-7);
                    startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1)).AddDays(-7);
                    countStore = _CuaHangDangKyService.GetCountForStage(startday, Endday);
                    countNews = _NewPostService.GetCountForStage(startday, Endday);
                }
                else if (tungay != null && denngay != null)
                {
                    var startday = new DateTime(tungay.Value.Year, tungay.Value.Month, tungay.Value.Day, 00, 00, 01);
                    var Endday = new DateTime(denngay.Value.Year, denngay.Value.Month, denngay.Value.Day, 23, 59, 59);
                    countStore = _CuaHangDangKyService.GetCountForStage(startday, Endday);
                    countNews = _NewPostService.GetCountForStage(startday, Endday);
                }



                return RetunJsonAction(true,
                    string.Empty,
                    new
                    {
                        countStore = countStore,
                        countNews = countNews
                    });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return ActionFalseNotData(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult Loadsonguoitruycap(int? filter, DateTime? tungay, DateTime? denngay)
        {
            try
            {
                var data = unitOfWork.GetRepository<UserVisit>().All();
                filter = filter == null ? (int)Notification.selectTime.homnay : filter;
                // Tìm kiếm theo filter
                if (filter == (int)Notification.selectTime.homnay)
                {
                    data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                      && o.CreateDate.Month == DateTime.Now.Month
                                      && o.CreateDate.Day == DateTime.Now.Day);
                }
                else if (filter == (int)Notification.selectTime.homqua)
                {
                    var day = DateTime.Now.AddDays(-1);
                    data = data.Where(o => o.CreateDate.Year == day.Year
                                     && o.CreateDate.Month == day.Month
                                     && o.CreateDate.Day == day.Day);
                }
                else if (filter == (int)Notification.selectTime.namnay)
                {
                    data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year);
                }
                else if (filter == (int)Notification.selectTime.namtruoc)
                {
                    data = data.Where(o => o.CreateDate.Year == (DateTime.Now.Year - 1));
                }
                else if (filter == (int)Notification.selectTime.quynay)
                {
                    if (DateTime.Now.Month <= 3)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month <= 3);
                    }
                    else if (DateTime.Now.Month > 3 && DateTime.Now.Month <= 6)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month > 3 && o.CreateDate.Month < 7);
                    }
                    else if (DateTime.Now.Month > 6 && DateTime.Now.Month <= 9)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month > 6 && o.CreateDate.Month < 10);
                    }
                    else
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                          && o.CreateDate.Month > 9);
                    }
                }
                else if (filter == (int)Notification.selectTime.quytruoc)
                {
                    if (DateTime.Now.Month <= 3)
                    {
                        data = data.Where(o => o.CreateDate.Year == (DateTime.Now.Year - 1)
                                            && o.CreateDate.Month > 9);
                    }
                    else if (DateTime.Now.Month > 3 && DateTime.Now.Month <= 6)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month < 4);
                    }
                    else if (DateTime.Now.Month > 6 && DateTime.Now.Month <= 9)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month > 3 && o.CreateDate.Month < 7);
                    }
                    else
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                          && o.CreateDate.Month > 6 && o.CreateDate.Month < 10);
                    }
                }
                else if (filter == (int)Notification.selectTime.thangnay)
                {
                    data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month == DateTime.Now.Month);
                }
                else if (filter == (int)Notification.selectTime.thangtruoc)
                {
                    var day = DateTime.Now.AddMonths(-1);
                    data = data.Where(o => o.CreateDate.Year == day.Year
                                            && o.CreateDate.Month == day.Month);
                }
                else if (filter == (int)Notification.selectTime.tuannay)
                {
                    var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
                    var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek);
                    startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1));
                    data = data.Where(o => o.CreateDate <= Endday
                                            && o.CreateDate >= startday);
                }
                else if (filter == (int)Notification.selectTime.tuantruoc)
                {
                    var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
                    var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek).AddDays(-7);
                    startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1)).AddDays(-7);
                    data = data.Where(o => o.CreateDate <= Endday
                                            && o.CreateDate >= startday);
                }
                else if (tungay != null && denngay != null)
                {
                    var startday = new DateTime(tungay.Value.Year, tungay.Value.Month, tungay.Value.Day, 00, 00, 01);
                    var Endday = new DateTime(denngay.Value.Year, denngay.Value.Month, denngay.Value.Day, 23, 59, 59);
                    data = data.Where(o => o.CreateDate <= Endday
                                                               && o.CreateDate >= startday);
                }
                return RetunJsonAction(true, string.Empty, data.Count());
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Exeption(ex);
            }
        }

        private void Tinhchiso(ref double? compare, ref bool up, int cHientai, int Ctruoc)
        {
            if (cHientai > Ctruoc)
            {
                compare = Ctruoc > 0 ? (((double)((cHientai - Ctruoc) * 100)) / Ctruoc) : 100;
                up = true;
            }
            else
            {
                up = false;
                compare = Ctruoc > 0 ? (((double)((Ctruoc - cHientai) * 100)) / Ctruoc) : 100;
            }
        }

        [HttpGet]
        public IHttpActionResult LoadChartVisit(int? type, int? filter, string countryName, DateTime? tungay, DateTime? denngay)
        {
            try
            {
                var data = unitOfWork.GetRepository<UserVisit>().All();
                // tìm kiếm theo nước
                if (countryName != null)
                {
                    data = data.Where(o => o.Country.Equals(countryName));
                }

                // Tìm kiếm theo filter
                if (filter == (int)Notification.selectTime.homnay)
                {
                    data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                      && o.CreateDate.Month == DateTime.Now.Month
                                      && o.CreateDate.Day == DateTime.Now.Day);
                }
                else if (filter == (int)Notification.selectTime.homqua)
                {
                    var day = DateTime.Now.AddDays(-1);
                    data = data.Where(o => o.CreateDate.Year == day.Year
                                     && o.CreateDate.Month == day.Month
                                     && o.CreateDate.Day == day.Day);
                }
                else if (filter == (int)Notification.selectTime.namnay)
                {
                    data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year);
                }
                else if (filter == (int)Notification.selectTime.namtruoc)
                {
                    data = data.Where(o => o.CreateDate.Year == (DateTime.Now.Year - 1));
                }
                else if (filter == (int)Notification.selectTime.quynay)
                {
                    if (DateTime.Now.Month <= 3)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month <= 3);
                    }
                    else if (DateTime.Now.Month > 3 && DateTime.Now.Month <= 6)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month > 3 && o.CreateDate.Month < 7);
                    }
                    else if (DateTime.Now.Month > 6 && DateTime.Now.Month <= 9)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month > 6 && o.CreateDate.Month < 10);
                    }
                    else
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                          && o.CreateDate.Month > 9);
                    }
                }
                else if (filter == (int)Notification.selectTime.quytruoc)
                {
                    if (DateTime.Now.Month <= 3)
                    {
                        data = data.Where(o => o.CreateDate.Year == (DateTime.Now.Year - 1)
                                            && o.CreateDate.Month > 9);
                    }
                    else if (DateTime.Now.Month > 3 && DateTime.Now.Month <= 6)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month < 4);
                    }
                    else if (DateTime.Now.Month > 6 && DateTime.Now.Month <= 9)
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month > 3 && o.CreateDate.Month < 7);
                    }
                    else
                    {
                        data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                          && o.CreateDate.Month > 6 && o.CreateDate.Month < 10);
                    }
                }
                else if (filter == (int)Notification.selectTime.thangnay)
                {
                    data = data.Where(o => o.CreateDate.Year == DateTime.Now.Year
                                            && o.CreateDate.Month == DateTime.Now.Month);
                }
                else if (filter == (int)Notification.selectTime.thangtruoc)
                {
                    var day = DateTime.Now.AddMonths(-1);
                    data = data.Where(o => o.CreateDate.Year == day.Year
                                            && o.CreateDate.Month == day.Month);
                }
                else if (filter == (int)Notification.selectTime.tuannay)
                {
                    var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
                    var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek);
                    startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1));
                    data = data.Where(o => o.CreateDate <= Endday
                                            && o.CreateDate >= startday);
                }
                else if (filter == (int)Notification.selectTime.tuantruoc)
                {
                    var startday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
                    var Endday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    Endday = Endday.AddDays(7 - (int)DateTime.Now.DayOfWeek).AddDays(-7);
                    startday = startday.AddDays(-((int)DateTime.Now.DayOfWeek - 1)).AddDays(-7);
                    data = data.Where(o => o.CreateDate <= Endday
                                            && o.CreateDate >= startday);
                }
                else if (tungay != null && denngay != null)
                {
                    var startday = new DateTime(tungay.Value.Year, tungay.Value.Month, tungay.Value.Day, 00, 00, 01);
                    var Endday = new DateTime(denngay.Value.Year, denngay.Value.Month, denngay.Value.Day, 23, 59, 59);
                    data = data.Where(o => o.CreateDate <= Endday
                                                               && o.CreateDate >= startday);
                }


                var list = new List<Model_banhang24vn.CustomView.Chart>();

                if (type == (int)Notification.Time.gio)
                {
                    var result = data.AsEnumerable();
                    var time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 01);
                    for (int i = 0; i < 48; i++)
                    {
                        int min = int.Parse(time.ToString("HHmm"));
                        int max = 0;
                        if (i == 47)
                        {
                            max = int.Parse(time.AddMinutes(29).ToString("HHmm"));
                            list.Add(new Model_banhang24vn.CustomView.Chart
                            {
                                label = time.AddMinutes(29).ToString("HH:mm"),
                                y = result.Where(o => min <= int.Parse(o.CreateDate.ToString("HHmm"))
                                                    && max >= int.Parse(o.CreateDate.ToString("HHmm"))).Count()
                            });
                        }
                        else
                        {
                            max = int.Parse(time.AddMinutes(30).ToString("HHmm"));
                            list.Add(new Model_banhang24vn.CustomView.Chart
                            {
                                label = time.AddMinutes(30).ToString("HH:mm"),
                                y = result.Where(o => min <= int.Parse(o.CreateDate.ToString("HHmm"))
                                                    && max >= int.Parse(o.CreateDate.ToString("HHmm"))).Count()
                            });
                        }

                        time = time.AddMinutes(30);
                    }
                    return RetunJsonAction(true, string.Empty, list);
                }
                else if (type == (int)Notification.Time.ngay)
                {
                    for (int i = 1; i < 32; i++)
                    {
                        list.Add(new Model_banhang24vn.CustomView.Chart { label = "Ngày " + i, y = data.Where(o => o.CreateDate.Day == i).Count() });
                    }
                    return RetunJsonAction(true, string.Empty, list);
                }
                else if (type == (int)Notification.Time.thang)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        list.Add(new Model_banhang24vn.CustomView.Chart { label = "Tháng " + i, y = data.Where(o => o.CreateDate.Month == i).Count() });
                    }
                    return RetunJsonAction(true, string.Empty, list);
                }
                else if (type == (int)Notification.Time.quy)
                {
                    list.Add(new Model_banhang24vn.CustomView.Chart { label = " Quý I", y = data.Where(o => o.CreateDate.Month >= 1 && o.CreateDate.Month <= 3).Count() });
                    list.Add(new Model_banhang24vn.CustomView.Chart { label = "Quý II", y = data.Where(o => o.CreateDate.Month >= 4 && o.CreateDate.Month <= 6).Count() });
                    list.Add(new Model_banhang24vn.CustomView.Chart { label = "Quý III", y = data.Where(o => o.CreateDate.Month >= 7 && o.CreateDate.Month <= 9).Count() });
                    list.Add(new Model_banhang24vn.CustomView.Chart { label = "Quý IV", y = data.Where(o => o.CreateDate.Month >= 10 && o.CreateDate.Month <= 12).Count() });
                    return RetunJsonAction(true, string.Empty, list);
                }
                else if (type == (int)Notification.Time.nam)
                {

                    if (filter == (int)Notification.selectTime.tuychon && tungay != null && denngay != null)
                    {
                        for (int i = tungay.Value.Year; i <= denngay.Value.Year; i++)
                        {
                            list.Add(new Model_banhang24vn.CustomView.Chart { label = "Năm" + i, y = data.Where(o => o.CreateDate.Year == i).Count() });
                        }
                        return RetunJsonAction(true, string.Empty, list);
                    }
                    var result = data.AsEnumerable().GroupBy(o => o.CreateDate.Year).OrderBy(o => o.Key).Select(o => new { label = "Năm " + o.Key, y = o.Count() });
                    return RetunJsonAction(true, string.Empty, result);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Exeption(ex);
            }
            return ActionFalseNotData(string.Empty);
        }

        [HttpGet]
        public IHttpActionResult LoadChartSystem()
        {
            try
            {
                var data = unitOfWork.GetRepository<UserVisit>().All();
                var count = data.Count();
                var result = data.AsEnumerable().GroupBy(o => o.System).Select(o => new Model_banhang24vn.CustomView.Chart { label = o.Key, y = o.Count() }).ToList();
                foreach (var item in result)
                {
                    item.y = StaticVariable.RateCalculation(item.y, count);
                }
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult LoadChartDevice()
        {
            try
            {
                var data = unitOfWork.GetRepository<UserVisit>().All();
                var count = data.Count();
                var result = data.AsEnumerable().GroupBy(o => o.Device).Select(o => new Model_banhang24vn.CustomView.Chart { label = o.Key, y = o.Count() }).ToList();
                foreach (var item in result)
                {
                    item.y = StaticVariable.RateCalculation(item.y, count);
                }
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult LoadChartCountry()
        {
            try
            {
                var _UserVisit = unitOfWork.GetRepository<UserVisit>();
                var sum = _UserVisit.Count;
                var data = _UserVisit.All().AsEnumerable().GroupBy(o => o.Country).Select(o => new { label = o.Key == null ? "Other" : o.Key, y = StaticVariable.RateCalculation((double)o.Count(), sum) }).OrderByDescending(o => o.label);
                return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult loadCountryName()
        {
            try
            {
                var _UserVisit = unitOfWork.GetRepository<UserVisit>();
                var data = _UserVisit.All().Select(o => o.Country).AsEnumerable().Select(o => !string.IsNullOrWhiteSpace(o) ? o : "Other").Distinct().ToList().OrderBy(o => o);
                return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult LoadChartCityVn()
        {
            try
            {
                var _UserVisit = unitOfWork.GetRepository<UserVisit>();
                var sum = _UserVisit.Count;
                var data = _UserVisit.All().Where(o => o.Country != null && o.Country.ToUpper().Equals("VIETNAM")).AsEnumerable().GroupBy(o => o.City).Select(o => new { label = o.Key == null ? "Other" : o.Key, y = StaticVariable.RateCalculation((double)o.Count(), sum) }).OrderByDescending(o => o.label);
                return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetPageView()
        {
            try
            {
                var data = new PageViewService().GetAll().GroupBy(o => o.PageName)
                    .Select(o => new { PageName = o.Key, Count = o.Sum(c => c.Count) })
                    .AsEnumerable().OrderByDescending(o => o.Count);
                var view = new DataGridView()
                {
                    Page = 1,
                    PageCount = (int)Math.Ceiling((double)data.Count() / Notification.PageDefault),
                    Data = data.Take(Notification.PageDefault).ToList(),
                };
                if (view.PageCount == 0)
                {
                    view.PageCount = 1;
                }
                view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, view);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        public IHttpActionResult GetMenuTags()
        {
            return Json(new MenuTagsService().GetAll().Where(o=>o.Status==true || o.Status==null));
        }
        [HttpPost]
        public IHttpActionResult UpdateTagsMenu(MenuTag model)
        {
            try
            {
                var result = new JsonViewModel<string>();
                if (model.ID == 0)
                {
                     result = new MenuTagsService().Insert(model);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        return InsertSuccess();
                    }
                }
                else
                {
                    result = new MenuTagsService().Update(model);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        return UpdateSuccess();
                    }
                }
                  return ActionFalseNotData(result.Data);
                
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult DeleteTagsMenu(MenuTag model)
        {
            try
            {

                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                else
                {
                  var   result = new MenuTagsService().Delete(model);
                    if (result)
                    {
                        return DeleteSuccess();
                    }
                    return ActionFalseNotData("Bản ghi không tồn tại hoặc đã bị xóa");
                }

            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult SearchGridMenu(DataGridView model) {

            try
            {
                var data = new MenuTagsService().SearhGrid(model.Search);
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.ToList();
                }
                else
                {
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList();
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult SearchPageView(DataGridView model)
        {
            var data = new PageViewService().GetAll().GroupBy(o => o.PageName).Select(o => new { PageName = o.Key, Count = o.Sum(c => c.Count) }).AsEnumerable().OrderByDescending(o => o.Count);
            model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
            if (model.PageCount == 0 || model.PageCount == 1)
            {
                model.PageCount = 1;
                model.Page = 1;
                model.Data = data.ToList();
            }
            else
            {
                model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList();
            }
            model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
            return RetunJsonAction(true, string.Empty, model);
        }

        [HttpGet]
        public IHttpActionResult GetContact()
        {
            try
            {
                var data = _ContactService.GetAllLienHe().OrderByDescending(o => o.CreateDate);
                var view = new DataGridView()
                {
                    Page = 1,
                    PageCount = (int)Math.Ceiling((double)data.Count() / Notification.PageDefault),
                    Data = data.Take(Notification.PageDefault).ToList(),
                };
                if (view.PageCount == 0)
                {
                    view.PageCount = 1;
                }
                view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, view);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult SearchContact(DataGridView model)
        {
            try
            {
                var data = _ContactService.SearhGrid(model.Search);
                if (model.Sort == (int)GridPagedingHellper.GridSort.SortUp)
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columContact.Name:
                            data = data.OrderBy(o => o.FullName);
                            break;
                        case (int)GridPagedingHellper.columContact.Phone:
                            data = data.OrderBy(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columContact.Email:
                            data = data.OrderBy(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columContact.Adress:
                            data = data.OrderBy(o => o.Address);
                            break;
                        case (int)GridPagedingHellper.columContact.Note:
                            data = data.OrderBy(o => o.Note);
                            break;
                        default:
                            data = data.OrderBy(o => o.CreateDate);
                            break;
                    }
                }
                else
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columContact.Name:
                            data = data.OrderByDescending(o => o.FullName);
                            break;
                        case (int)GridPagedingHellper.columContact.Phone:
                            data = data.OrderByDescending(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columContact.Email:
                            data = data.OrderByDescending(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columContact.Adress:
                            data = data.OrderByDescending(o => o.Address);
                            break;
                        case (int)GridPagedingHellper.columContact.Note:
                            data = data.OrderByDescending(o => o.Note);
                            break;
                        default:
                            data = data.OrderByDescending(o => o.CreateDate);
                            break;
                    }
                }
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.ToList();
                }
                else
                {
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList();
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult SearchCustomerContactSales(DataGridView model)
        {
            try
            {
                var data = _ContactService.SearhOrderGrid(model.Search, model.TypeHsd ?? -1);
                if (model.Sort == (int)GridPagedingHellper.GridSort.SortUp)
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columContact.Name:
                            data = data.OrderBy(o => o.FullName);
                            break;
                        case (int)GridPagedingHellper.columContact.Phone:
                            data = data.OrderBy(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columContact.Email:
                            data = data.OrderBy(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columContact.Adress:
                            data = data.OrderBy(o => o.Address);
                            break;
                        case (int)GridPagedingHellper.columContact.Note:
                            data = data.OrderBy(o => o.Note);
                            break;
                        default:
                            data = data.OrderBy(o => o.CreateDate);
                            break;
                    }
                }
                else
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columContact.Name:
                            data = data.OrderByDescending(o => o.FullName);
                            break;
                        case (int)GridPagedingHellper.columContact.Phone:
                            data = data.OrderByDescending(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columContact.Email:
                            data = data.OrderByDescending(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columContact.Adress:
                            data = data.OrderByDescending(o => o.Address);
                            break;
                        case (int)GridPagedingHellper.columContact.Note:
                            data = data.OrderByDescending(o => o.Note);
                            break;
                        default:
                            data = data.OrderByDescending(o => o.CreateDate);
                            break;
                    }
                }
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.AsEnumerable().Select(o =>
                        new CustomerContactsale
                        {
                            ID = o.ID,
                            Address = o.Address,
                            Browser = o.Browser,
                            CreateDate = o.CreateDate,
                            Devicess = o.Devicess,
                            Email = o.Email,
                            FullName = o.FullName,
                            IpAdress = o.IpAdress,
                            Ipv4 = o.Ipv4,
                            Note = o.Note,
                            Phone = o.Phone,
                            System = o.System,
                            Status = o.Status,
                            TypeContact = o.Type,
                            TypeSoftWareNews = o.Software,
                            Website = Notification.listSoftWare.Any(c => c.Key == o.TypeSoftWare) ? Notification.listSoftWare.FirstOrDefault(c => c.Key == o.TypeSoftWare).Value : string.Empty,
                            StatusNews = Notification.listStatusContact.Any(c => c.Key == o.Status) ? Notification.listStatusContact.FirstOrDefault(c => c.Key == o.Status).Value : string.Empty,
                        }
                        );
                }
                else
                {
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList().Select(o =>
                        new CustomerContactsale
                        {
                            ID = o.ID,
                            Address = o.Address,
                            Browser = o.Browser,
                            CreateDate = o.CreateDate,
                            Devicess = o.Devicess,
                            Email = o.Email,
                            FullName = o.FullName,
                            IpAdress = o.IpAdress,
                            Ipv4 = o.Ipv4,
                            Note = o.Note,
                            Phone = o.Phone,
                            System = o.System,
                            Status = o.Status,
                            TypeSoftWareNews = o.Software,
                            TypeContact = o.Type,
                            Website = Notification.listSoftWare.Any(c => c.Key == o.TypeSoftWare) ? Notification.listSoftWare.FirstOrDefault(c => c.Key == o.TypeSoftWare).Value : string.Empty,
                            StatusNews = Notification.listStatusContact.Any(c => c.Key == o.Status) ? Notification.listStatusContact.FirstOrDefault(c => c.Key == o.Status).Value : string.Empty,
                        }
                        );
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult SaveStatusContact(Contact model)
        {
            try
            {
                if (model != null)
                {
                    var result = _ContactService.SaveContact(model);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        return ActionTrueNotData(result.Data);
                    }
                    return ActionFalseNotData(result.Data);
                }

                return ActionFalseNotData("Không lấy được thông tin cần cập nhật");
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult SearchAdvertising(DataGridView model)
        {
            try
            {
                var data = _AdvertisingService.SearhGrid(model.Search).OrderBy(o => o.EditDate);
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.ToList();
                }
                else
                {
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList();
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult InsertAdvertising(Advertisement model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Title)
                  || string.IsNullOrWhiteSpace(model.UrlImage)
                     )
                {
                    return ActionFalseNotData("Vui lòng chọn ảnh và tiêu đề ảnh");
                }
                if (model.FromDate != null && model.ToDate != null
                     && DateTime.Compare(model.FromDate.Value.Date, model.ToDate.Value.Date) > 0)
                {
                    return ActionFalseNotData("Hiện thị từ ngày lớn hơn đến ngày.");
                }
                model.EditUser = contant.SESSIONNGUOIDUNG.UserName;
                _AdvertisingService.Insert(model);
                return InsertSuccess();
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult UpdateAdvertising(Advertisement model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Title)
                  || string.IsNullOrWhiteSpace(model.UrlImage)
                     )
                {
                    return ActionFalseNotData("Vui lòng chọn ảnh và tiêu đề ảnh");
                }
                if (model.FromDate != null && model.ToDate != null
                   && DateTime.Compare(model.FromDate ?? DateTime.Now, model.ToDate ?? DateTime.Now) > 0)
                {
                    return ActionFalseNotData("Hiện thị từ ngày lớn hơn đến ngày.");
                }
                model.EditUser = contant.SESSIONNGUOIDUNG.UserName;
                var result = _AdvertisingService.Update(model);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    return UpdateSuccess();
                }
                return ActionFalseNotData(result.Data);
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult DeleteAdvertising(Advertisement model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được dữ liệu cần xáo");
                }
                _AdvertisingService.delete(model.ID);
                return DeleteSuccess();
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }


        public IHttpActionResult SearchNotificationSoftware(DataGridView model)
        {
            try
            {
                var data = _NotificationSoftwareService.SearhGrid(model.Search).OrderByDescending(p=> p.ApplyDate);

                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = (from o in data.ToList()
                                  join b in Notification.ListNotificationSoftware.ToList()
                                  on o.Type equals b.Key
                                  select new
                                  {
                                      ID = o.ID,
                                      Subdomain = o.Subdomain,
                                      Title = o.Title,
                                      BodyContent = o.BodyContent,
                                      Type = b.Value,
                                      EditUser = o.EditUser,
                                      ApplyDate = o.ApplyDate,
                                      Status = o.Status
                                  }).ToList();
                }
                else
                {
                    List<NotificationSoftware> lstResult = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).ToList();
                    model.Data = (from o in lstResult
                                  join b in Notification.ListNotificationSoftware.ToList()
                                  on o.Type equals b.Key
                                  select new
                                  {
                                      ID = o.ID,
                                      Subdomain = o.Subdomain,
                                      Title = o.Title,
                                      Type = b.Value,
                                      BodyContent = o.BodyContent,
                                      EditUser = o.EditUser,
                                      ApplyDate = o.ApplyDate,
                                      Status = o.Status
                                  }).ToList();

                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult EditNotificationSoftware(NotificationSoftware model)
        {
            try
            {
                if (model != null)
                {
                    model.EditDate = DateTime.Now;
                    model.EditUser = contant.SESSIONNGUOIDUNG.UserName;
                    if (model.ID == 0)
                    {
                        _NotificationSoftwareService.Insert(model);
                        return InsertSuccess();
                    }
                    else
                    {
                        if (_NotificationSoftwareService.Update(model) == (int)Notification.ErrorCode.success)
                            return UpdateSuccess();
                    }
                }
                else
                {
                    return ActionFalseNotData("Không lấy được dữ liệu.");
                }
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
            return Exeption();
        }

        public IHttpActionResult GetNotificationSoftwareById(int id)
        {
            try
            {
                var model = _NotificationSoftwareService.Query.FirstOrDefault(o => o.ID == id);
                if (model != null)
                {
                    return RetunJsonAction(true, string.Empty, model);
                }
                else
                {
                    return ActionFalseNotData("Thông báo không tồn tại.");
                }
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteNotificationSoftware(NotificationSoftware model)
        {
            try
            {
                if (model != null)
                {
                    _NotificationSoftwareService.Delete(model);
                    return DeleteSuccess();
                }
                else
                {
                    return ActionFalseNotData("Không lấy được dữ liệu.");
                }
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult OrderedSoftWare(Contact model, string ip4, string ipAdress)
        {
            try
            {
                if (model != null)
                {
                    model.ID = Guid.NewGuid();
                    model.CreateDate = DateTime.Now;
                    model.Status = (int)Notification.StatusContact.Moi;
                    model.Type = model.Type == null ? (int)Notification.TypeContact.datmua : model.Type;
                    model.Ipv4 = ip4;
                    model.IpAdress = ipAdress;
                    model.System = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                    model.Devicess = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                    model.Browser = "Other";
                    model.Software = Notification.listTypeSalesSoftWare.Where(o => o.Key == model.TypeSoftWare).Select(o => o.Value).FirstOrDefault();
                    model.TypeSoftWare = model.TypeSoftWare = (int)Model_banhang24vn.Common.Notification.TypeSoftWare.Open24;
                    if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[3].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[6].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[4].ToString();
                    }
                    var tengoi = Notification.listTypeSalesSoftWare.Where(o => o.Key == model.TypeSoftWare);
                    if (string.IsNullOrWhiteSpace(model.Software) && model.TypeSoftWare != null)
                    {
                        model.Software = tengoi.Any() ? tengoi.FirstOrDefault().Value : string.Empty;
                    }
                    _ContactService.Insert(model);
                    //thay đổi thêm mới bảng gói dịch vụ, ko fix cứng nữa sau có thời gian sẽ làm lại ^.^
                    string body = "<h3> Thông tin đặt hàng Open24</h3><br>"
                         + "<span>Họ tên: " + model.FullName + "</span><br>"
                         + "<span>Số điện thoại: " + model.Phone + "</span><br>"
                         + "<span>Email: " + model.Email + "</span><br>"
                         + "<span>Địa chỉ: " + model.Address + "</span><br>"
                         + "<span>Gói sản phẩm: " + (tengoi.Any() ? tengoi.FirstOrDefault().Value : string.Empty) + "</span><br><br>"
                         + "<span>Thiết bị: " + model.Devicess + "</span><br>"
                         + "<span>Hệ điều hành: " + model.System + "</span><br>"
                         + "<span>Trình duyệt: " + model.Browser + "</span><br>"
                         + "<span>IPV4: " + ip4 + "</span><br>"
                         + "<span>Khu vực: " + ipAdress + "</span><br><br>"
                         + "<span style='text-align: center'>--- Nội dung đặt hàng --- </span>"
                         + "<p>  " + model.Note + "</p>";
                    MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(), "[Đặt mua Open24] KH: " + model.FullName + " đặt mua phần mềm", body);
                    return InsertSuccess();
                }
                return ActionFalseNotData("Không thể lấy được thông tin, vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult OrderedSsoftSoftWare(Contact model, string ip4, string ipAdress)
        {
            try
            {
                if (model != null)
                {
                    model.ID = Guid.NewGuid();
                    model.Type = model.Type;
                    model.Ipv4 = ip4;
                    model.IpAdress = ipAdress;
                    model.CreateDate = DateTime.Now;
                    model.Status = (int)Notification.StatusContact.Moi;
                    model.System = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                    model.Devicess = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                    model.Browser = "Other";
                    if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[3].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[6].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[4].ToString();
                    }
                    _ContactService.Insert(model);
                    string body = "<h3> Thông tin đặt hàng phần mềm Ssoft</h3><br>"
                         + "<span>Họ tên: " + model.FullName + "</span><br>"
                         + "<span>Số điện thoại: " + model.Phone + "</span><br>"
                         + "<span>Email: " + model.Email + "</span><br>"
                         + "<span>Địa chỉ: " + model.Address + "</span><br>"
                         + "<span>Gói sản phẩm: " + model.Software + "</span><br><br>"
                         + "<span>Thiết bị: " + model.Devicess + "</span><br>"
                         + "<span>Hệ điều hành: " + model.System + "</span><br>"
                         + "<span>Trình duyệt: " + model.Browser + "</span><br>"
                         + "<span>IPV4: " + ip4 + "</span><br>"
                         + "<span>Khu vực: " + ipAdress + "</span><br><br>"
                         + "<span style='text-align: center'>--- Nội dung đặt hàng --- </span>"
                         + "<p>  " + model.Note + "</p>";
                    MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(), "[Web Ssoft] KH: " + model.FullName + (model.Type == 1 ? "đặt mua" : "muốn dùng thử"), body);
                    return InsertSuccess();
                }
                return ActionFalseNotData("Không thể lấy được thông tin, vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult OrderedOpen24SoftWare(Contact model, string ip4, string ipAdress)
        {
            try
            {
                if (model != null)
                {
                    model.ID = Guid.NewGuid();
                    model.Type = (int)Notification.TypeContact.dungthu;
                    model.Ipv4 = ip4;
                    model.IpAdress = ipAdress;
                    model.CreateDate = DateTime.Now;
                    model.Software = Notification.ListKeyInTitle.Where(o => o.Key == model.Software.Trim()).Select(o => o.Value).FirstOrDefault() ;
                    model.TypeSoftWare = (int)Notification.TypeSoftWare.Open24;
                    model.Status = (int)Notification.StatusContact.Moi;
                    model.System = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                    model.Devicess = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                    model.Browser = "Other";
                    if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[3].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[6].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[4].ToString();
                    }
                    _ContactService.Insert(model);
                    string body = "<h3> Thông tin đăng ký phần mềm open24</h3><br>"
                         + "<span>Họ tên: " + model.FullName + "</span><br>"
                         + "<span>Số điện thoại: " + model.Phone + "</span><br>"
                         + "<span>Email: " + model.Email + "</span><br>"
                         + "<span>Địa chỉ: " + model.Address + "</span><br>"
                         + "<span>Gói sản phẩm: " + model.Software + "</span><br><br>"
                         + "<span>Thiết bị: " + model.Devicess + "</span><br>"
                         + "<span>Hệ điều hành: " + model.System + "</span><br>"
                         + "<span>Trình duyệt: " + model.Browser + "</span><br>"
                         + "<span>IPV4: " + ip4 + "</span><br>"
                         + "<span>Khu vực: " + ipAdress + "</span><br><br>"
                         + "<span style='text-align: center'>--- Ghi chú --- </span>"
                         + "<p>  " + model.Note + "</p>";
                    MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(), "[open24] KH: " + model.FullName + (model.Type == 1 ? "đặt mua" : "muốn dùng thử"), body);
                    return InsertSuccess();
                }
                return ActionFalseNotData("Không thể lấy được thông tin, vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult FormDangKyTuVan([FromBody] JObject objIn)
        {
            try
            {
                if (objIn != null)
                {
                    string Software = "OTXMXDD";
                    if (objIn["Software"] != null)
                        Software = objIn["Software"].ToObject<string>();
                    
                    string FullName = "";
                    if (objIn["FullName"] != null)
                        FullName = objIn["FullName"].ToObject<string>();

                    string Address = "";
                    if (objIn["Address"] != null)
                        Address = objIn["Address"].ToObject<string>();

                    string Noted = "";
                    if (objIn["Noted"] != null)
                        Noted = objIn["Noted"].ToObject<string>();

                    string Phone = "";
                    if (objIn["Phone"] != null)
                        Phone = objIn["Phone"].ToObject<string>();

                    string Email = "";
                    if (objIn["Email"] != null)
                        Email = objIn["Email"].ToObject<string>();

                    string Ipv4 = "";
                    if (objIn["IPv4"] != null)
                        Ipv4 = objIn["IPv4"].ToObject<string>();

                    string IpAdress = "";
                    if (objIn["IpAddress"] != null)
                        IpAdress = objIn["IpAddress"].ToObject<string>();

                    string Company = "";
                    if (objIn["Company"] != null)
                        Company = objIn["Company"].ToObject<string>();

                    int Type = 1;//dùng thử(1) hay đặt mua(2)
                    if (objIn["Type"] != null)
                        Type = objIn["Type"].ToObject<int>();


                    Contact model = new Contact();
                    model.ID = Guid.NewGuid();
                    model.Type = Type;
                    model.FullName = FullName;
                    model.Phone = Phone;
                    model.Email = Email;
                    model.Address = Address;
                    model.Note = Noted;

                    model.Ipv4 = Ipv4;
                    model.IpAdress = IpAdress;
                    model.CreateDate = DateTime.Now;
                    model.Software = Notification.ListKeyInTitle.Where(o => o.Key == Software.Trim()).Select(o => o.Value).FirstOrDefault();
                    model.TypeSoftWare = (int)Notification.TypeSoftWare.Open24;
                    model.Status = (int)Notification.StatusContact.Moi;
                    model.System = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                    model.Devicess = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                    model.Browser = "Other";

                    if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[3].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[6].ToString();
                    }
                    else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                    {
                        model.Browser = Request.Headers.UserAgent.ToArray()[4].ToString();
                    }
                    _ContactService.Insert(model);
                    string body = "<h3> Thông tin đăng ký phần mềm open24</h3><br>"
                         + "<span>Họ tên: " + model.FullName + "</span><br>"
                         + "<span>Số điện thoại: " + model.Phone + "</span><br>"
                         + "<span>Email: " + model.Email + "</span><br>"
                         + "<span>Địa chỉ: " + model.Address + "</span><br>"
                         + "<span>Gói sản phẩm: " + model.Software + "</span><br><br>"
                         + "<span>Thiết bị: " + model.Devicess + "</span><br>"
                         + "<span>Hệ điều hành: " + model.System + "</span><br>"
                         + "<span>Trình duyệt: " + model.Browser + "</span><br>"
                         + "<span>IPV4: " + model.Ipv4 + "</span><br>"
                         + "<span>Khu vực: " + model.IpAdress + "</span><br><br>"
                         + "<span>Công ty: " +Company + "</span><br><br>"
                         + "<span style='text-align: center'>--- Ghi chú --- </span>"
                         + "<p>  " + model.Note + "</p>";

                    MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(), "[open24] KH: " + model.FullName + (model.Type == 1 ? "đặt mua" : "muốn dùng thử"), body);
                    return InsertSuccess();
                }
                else
                {
                    return ActionFalseNotData("Không thể lấy được thông tin, vui lòng thử lại sau.");
                }



            }
            catch (Exception ex) { return Exeption(ex); }

        }

        [HttpPost]
        public IHttpActionResult SearchServiceSms(DataGridView model)
        {
            var data = _RegisterServiceSmsService.SearGridServiceSms(model.Search).OrderByDescending(o => o.CreateDate);
            model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
            if (model.PageCount == 0 || model.PageCount == 1)
            {
                model.PageCount = 1;
                model.Page = 1;
                model.Data = data.AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.Name,
                    o.Note,
                    o.Price,
                    o.SoDienThoaiCuaHang,
                    o.Status,
                    o.CreateDate,
                    SupplierSm = o.SupplierSm != null ? o.SupplierSm.Name : string.Empty,
                    o.ID_SupplierSms
                }).AsEnumerable();
            }
            else
            {
                model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit)
                .AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.Name,
                    o.Note,
                    o.Price,
                    o.SoDienThoaiCuaHang,
                    o.Status,
                    o.CreateDate,
                    SupplierSm = o.SupplierSm != null ? o.SupplierSm.Name : string.Empty,
                    o.ID_SupplierSms
                }).AsEnumerable();
            }
            model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
            return RetunJsonAction(true, string.Empty, model);
        }

        [HttpPost]
        public IHttpActionResult SearchDetailServiceSms(DataGridView model)
        {
            var data = _RegisterServiceSmsService.SearGridDetailServiceSms(model.Search).OrderByDescending(o => o.CreateDate)
                .Select(o => new
                {
                    o.ID,
                    o.Name,
                    o.Note,
                    o.Price,
                    o.SoDienThoaiCuaHang,
                    o.Status,
                    o.CreateDate,
                    SupplierSm = o.SupplierSm != null ? o.SupplierSm.Name : string.Empty,
                    o.ID_SupplierSms
                });
            model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
            if (model.PageCount == 0 || model.PageCount == 1)
            {
                model.PageCount = 1;
                model.Page = 1;
                model.Data = data.AsEnumerable();
            }
            else
            {
                model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).AsEnumerable();
            }
            model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
            return RetunJsonAction(true, string.Empty, model);
        }

        public IHttpActionResult GetSupplierSms()
        {
            try
            {
                var data = _RegisterServiceSmsService.GetAllSupplierSms()
                    .Select(o => new
                    {
                        o.ID,
                        o.IsDefault,
                        o.Name

                    }).AsEnumerable();
                return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {
                Exeption(ex);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult UpdateServiceSms(RegisterServiceSm model)
        {
            try
            {
                model.UserActivated = contant.SESSIONNGUOIDUNG.UserName;
                var result = _RegisterServiceSmsService.Update(model);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    return RetunJsonAction(true, "Cập nhật thành công", "Cập nhật thành công");
                }
                return ActionFalseNotData(result.Data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult SearchRechargeService(DataGridView model)
        {
            var data = _CuaHangDangKyService.SearchDichvuNaptien(model.Search).OrderByDescending(o => o.NgayTao);
            model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
            if (model.PageCount == 0 || model.PageCount == 1)
            {
                model.PageCount = 1;
                model.Page = 1;
                model.Data = data.AsEnumerable();
            }
            else
            {if(model.Page == 0)
                {
                    model.Page = 1;
                }
                model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).AsEnumerable();
            }
            model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
            return RetunJsonAction(true, string.Empty, model);
        }

        [HttpPost]
        public IHttpActionResult SearchDetailRechargeService(DataGridView model)
        {
            var data = _CuaHangDangKyService.SearchDichvuNaptien(string.Empty).Where(o => o.SoDienThoai.Equals(model.Search)).OrderByDescending(o => o.NgayTao);
            model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
            if (model.PageCount == 0 || model.PageCount == 1)
            {
                model.PageCount = 1;
                model.Page = 1;
                model.Data = data.AsEnumerable();
            }
            else
            {
                model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).AsEnumerable();
            }
            model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
            return RetunJsonAction(true, string.Empty, model);
        }

        [HttpPost]
        public IHttpActionResult EditPhieuNapTien(DichVuNapTien model)
        {
            try
            {
                if (model == null)

                {
                    return ActionFalseNotData("Không lấy được thông tin");
                }
                if (model.ID == new Guid())
                {
                    var isGet = false;
                    if (string.IsNullOrWhiteSpace(model.SoDienThoai))
                    {
                        var cuahang = _CuaHangDangKyService.Query.FirstOrDefault(o => o.SubDomain.ToUpper().Equals(model.TenCuaHang.ToUpper()));
                        if (cuahang != null)
                        {
                            isGet = true;
                            model.SoDienThoai = cuahang.SoDienThoai;
                        }
                    }
                    if (isGet||_CuaHangDangKyService.Query.Any(o => o.SoDienThoai.Equals(model.SoDienThoai)))
                    {
                        var modelNew = new CuaHangNapTienDichVu()
                        {
                            GhiChu = model.GhiChu,
                            NgayTao = DateTime.Now,
                            SoDienThoaiCuaHang = model.SoDienThoai,
                            SoTien = model.SoTien,
                            TenKhachHang = model.TenKhachNap,
                            TrangThai=model.TrangThai,
                            ID = Guid.NewGuid()
                        };
                        _CuaHangDangKyService.InsertDichvuNaptien(modelNew);
                        return InsertSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData("Tên gian hàng không tồn tại");
                    }

                }
                else
                {
                    var modelNew = new CuaHangNapTienDichVu()
                    {
                        GhiChu = model.GhiChu,
                        SoTien = model.SoTien,
                        TenKhachHang = model.TenKhachNap,
                        TrangThai=model.TrangThai,
                        ID = model.ID
                    };
                    var result = _CuaHangDangKyService.UpdateDichvuNaptien(modelNew);
                    if (result.ErrorCode==(int)Notification.ErrorCode.success)
                    {
                        return RetunJsonAction(true,"Cập nhật thành công", result.Data);
                    }
                    return ActionFalseNotData("Không tồn tại phiếu nạp tiền này hoặc phiếu đã bị xóa");
                }


            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult RemovePhieuNapTien(Guid Key)
        {
            try
            {
                if (Key == new Guid())

                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa");
                }
                _CuaHangDangKyService.RemoveDichvuNaptien(Key);
                return DeleteSuccess();

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        public IHttpActionResult GetAllTrangThaiNapTien()
        {
            try
            {
               
                return RetunJsonAction(true,string.Empty,Notification.listDichVuNapTien.ToArray());

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetSupplierSmById(string id)
        {
            try
            {
                RegisterServiceSm objBrand = new RegisterServiceSmsService().GetBrandNameById(new Guid(id));
                SupplierSm objKey = new SupplierSmService().GetAPIKeyByIDSupllierSMS(objBrand.ID_SupplierSms.Value);
                return ActionTrueWithData(new
                {
                    BrandName = objBrand.Name,
                    ApiKey = objKey.ApiKey,
                    ApiSecret = objKey.ApiSecret,
                    Price = objKey.Price
                });
            }
            catch
            {
                return null;
            }
        }
    }

}
