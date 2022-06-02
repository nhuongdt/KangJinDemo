using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Model_banhang24vn.DAL
{
     class NumberUserAccessService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<NumberUserAccess> _NumberUserAccess;
        public NumberUserAccessService()
        {
            _NumberUserAccess = unitOfWork.GetRepository<NumberUserAccess>();
        }
        public IQueryable<NumberUserAccess> Query { get { return _NumberUserAccess.All(); } }

        /// <summary>
        /// Lấy dữ liệu biểu diễn lượng người dùng truy cập theo tháng
        /// </summary>
        /// <param name="thang"></param>
        /// <param name="nam"></param>
        /// <returns></returns>
        public object GetChartMonth(int thang, int nam)
        {
            int tungay = (nam * 100 + thang) * 100 + 01;
            int denngay = (nam * 100 + thang) * 100 + DateTime.DaysInMonth(nam, thang);
            var startDate = new DateTime(nam, thang, 01);
            var data = _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay);
            var result = new List<NumberUserAccess>();
            if (thang == DateTime.Now.Month && nam == DateTime.Now.Year)
            {
                for (int i = 0; i < (DateTime.Now.Day - 1); i++)
                {
                    int key = tungay + i;
                    var model = data.FirstOrDefault(o => o.ID == key);
                    if (model != null)
                    {
                        result.Add(model);
                    }
                    else
                    {
                        result.Add(new NumberUserAccess { ID = key, Number = 0, CreateDate = startDate.AddDays(i) });
                    }
                }
            }
            else
            {
                for (int i = 0; i < DateTime.DaysInMonth(nam, thang); i++)
                {
                    int key = tungay + i;
                    var model = data.FirstOrDefault(o => o.ID == key);
                    if (model != null)
                    {
                        result.Add(model);
                    }
                    else
                    {
                        result.Add(new NumberUserAccess { ID = key, Number = 0, CreateDate = startDate.AddDays(i) });
                    }
                }
            }
            return result.OrderBy(o => o.CreateDate).Select(o => new { y = o.Number, label = o.CreateDate.Value.ToString("dd-MM-yyyy") }).ToList();
        }

        /// <summary>
        /// Lấy dữ liệu biểu diễn lượng người dùng truy cập theo năm
        /// </summary>
        /// <param name="nam"></param>
        /// <returns></returns>
        public object GetChartYear(int nam)
        {
            int tungay = (nam * 100 + 1) * 100 + 1;
            int denngay = (nam * 100 + 12) * 100 + DateTime.DaysInMonth(nam, 12);
            var data = _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay);
            var result = new List<Chart>();
            if (nam == DateTime.Now.Year)
            {
                if (DateTime.Now.Month <= 10)
                {
                    for (int i = 1; i <= (DateTime.Now.Month - 1); i++)
                    {
                        var model = data.Where(o => o.CreateDate.Value.Month == i);
                        if (model.Any())
                        {
                            result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                        }
                        else
                        {
                            result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = 0 });
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        var model = data.Where(o => o.CreateDate.Value.Month == i);
                        if (model.Any())
                        {
                            result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                        }
                        else
                        {
                            result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = 0 });
                        }
                    }
                    for (int i = 10; i <= (DateTime.Now.Month - 1); i++)
                    {
                        var model = data.Where(o => o.CreateDate.Value.Month == i);
                        if (model.Any())
                        {
                            result.Add(new Chart { label = i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                        }
                        else
                        {
                            result.Add(new Chart { label = i + "-" + nam.ToString(), y = 0 });
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i <= 9; i++)
                {
                    var model = data.Where(o => o.CreateDate.Value.Month == i);
                    if (model.Any())
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                    }
                    else
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = 0 });
                    }
                }
                for (int i = 10; i <= 12; i++)
                {
                    var model = data.Where(o => o.CreateDate.Value.Month == i);
                    if (model.Any())
                    {
                        result.Add(new Chart { label = i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                    }
                    else
                    {
                        result.Add(new Chart { label = i + "-" + nam.ToString(), y = 0 });
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  Lấy dữ liệu biểu diễn lượng người dùng truy cập theo Quý
        /// </summary>
        /// <param name="quy"></param>
        /// <param name="nam"></param>
        /// <returns></returns>
        public object GetChartQuy(int quy, int nam)
        {
            var data = new List<NumberUserAccess>().AsQueryable();
            var result = new List<Chart>();
            if (quy == 1)
            {
                int tungay = (nam * 100 + 1) * 100 + 1;
                int denngay = (nam * 100 + 3) * 100 + DateTime.DaysInMonth(nam, 3);
                data = _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay);
                for (int i = 1; i <= 3; i++)
                {
                    var model = data.Where(o => o.CreateDate.Value.Month == i);
                    if (model.Any())
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                    }
                    else
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = 0 });
                    }
                }
            }
            else if (quy == 2)
            {
                int tungay = (nam * 100 + 4) * 100 + 1;
                int denngay = (nam * 100 + 6) * 100 + DateTime.DaysInMonth(nam, 6);
                data = _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay);
                for (int i = 4; i <= 6; i++)
                {
                    var model = data.Where(o => o.CreateDate.Value.Month == i);
                    if (model.Any())
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                    }
                    else
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = 0 });
                    }
                }
            }
            else if (quy == 3)
            {

                int tungay = (nam * 100 + 7) * 100 + 1;
                int denngay = (nam * 100 + 9) * 100 + DateTime.DaysInMonth(nam, 9);
                data = _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay);
                for (int i = 7; i <= 9; i++)
                {
                    var model = data.Where(o => o.CreateDate.Value.Month == i);
                    if (model.Any())
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                    }
                    else
                    {
                        result.Add(new Chart { label = "0" + i + "-" + nam.ToString(), y = 0 });
                    }
                }
            }
            else
            {
                int tungay = (nam * 100 + 10) * 100 + 1;
                int denngay = (nam * 100 + 12) * 100 + DateTime.DaysInMonth(nam, 12);
                data = _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay);
                for (int i = 10; i <= 12; i++)
                {
                    var model = data.Where(o => o.CreateDate.Value.Month == i);
                    if (model.Any())
                    {
                        result.Add(new Chart { label = i + "-" + nam.ToString(), y = (double)model.Sum(o => o.Number) });
                    }
                    else
                    {
                        result.Add(new Chart { label = i + "-" + nam.ToString(), y = 0 });
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Lấy tất cả số lượng người truy cập
        /// </summary>
        /// <returns></returns>
        public int? GetAllNumber()
        {
            return _NumberUserAccess.All().Sum(o => o.Number);
        }

        /// <summary>
        /// Tính lượng truy cập theo tháng
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int? GetNumberForMonth(int year, int month)
        {
            int tungay = (year * 100 + month) * 100 + 01;
            int denngay = (year * 100 + month) * 100 + DateTime.DaysInMonth(year, month);
            return _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay).Sum(o => o.Number);
        }
        /// <summary>
        /// Tính lượng truy cập theo giai đoạn
        /// </summary>
        /// <param name="tuthang"></param>
        /// <param name="denthang"></param>
        /// <returns></returns>
        public int? GetNumberForStage(DateTime tuthang, DateTime denthang)
        {
            var tungay = int.Parse(tuthang.ToString("yyyyMMdd"));
            var denngay = int.Parse(denthang.ToString("yyyyMMdd"));
            return _NumberUserAccess.Filter(o => o.ID <= denngay && o.ID >= tungay).Sum(o => o.Number);
        }

        /// <summary>
        /// Tính lượng truy cập theo quý
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quy"></param>
        /// <returns></returns>
        public int? GetNumberForQuy(int year, int quy)
        {
            int tungay;
            int denngay;
            if (quy == 1)
            {
                tungay = (year * 100 + 1) * 100 + 1;
                denngay = (year * 100 + 3) * 100 + DateTime.DaysInMonth(year, 3);
            }
            else if (quy == 2)
            {
                tungay = (year * 100 + 4) * 100 + 1;
                denngay = (year * 100 + 6) * 100 + DateTime.DaysInMonth(year, 6);
            }
            else if (quy == 3)
            {
                tungay = (year * 100 + 7) * 100 + 1;
                denngay = (year * 100 + 9) * 100 + DateTime.DaysInMonth(year, 9);
            }
            else
            {
                tungay = (year * 100 + 10) * 100 + 1;
                denngay = (year * 100 + 12) * 100 + DateTime.DaysInMonth(year, 12);

            }
            return _NumberUserAccess.Filter(o => o.ID <= denngay && o.ID >= tungay).Sum(o => o.Number);
        }

        /// <summary>
        /// Tính lượng truy cập theo năm
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quy"></param>
        /// <returns></returns>
        public int? GetNumberForYear(int year)
        {
            int tungay = (year * 100 + 1) * 100 + 1;
            int denngay = (year * 100 + 12) * 100 + DateTime.DaysInMonth(year, 12);
            return _NumberUserAccess.Filter(o => o.ID >= tungay && o.ID <= denngay).Sum(o => o.Number);
        }

        /// <summary>
        /// Thêm người truy cập vào dbs
        /// </summary>
        public void AddNumberAccess(HttpRequestBase request)
        {
            int keyID = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            var data = _NumberUserAccess.Find(keyID);
            int typeDevice = StaticVariable.GetDeviceType(request.UserAgent);
            if (data == null)
            {
                var model = new NumberUserAccess
                {
                    ID = keyID,
                    Number = 1,
                    Device_Desktop = 0,
                    Device_Mobile = 0,
                    Device_Tablet = 0,
                    Device_TV = 0,
                    Sys_Android = 0,
                    Sys_Linux = 0,
                    Sys_MAC_OS = 0,
                    Sys_Other = 0,
                    Sys_Windows = 0,
                    CreateDate = DateTime.Now
                };
                switch (typeDevice)
                {
                    case (int)Notification.Device.tablet:
                        model.Device_Tablet = 1;
                        break;
                    case (int)Notification.Device.desktop:
                        model.Device_Desktop = 1;
                        break;
                    case (int)Notification.Device.mobile:
                        model.Device_Mobile = 1;
                        break;
                    default:
                        model.Device_TV = 1;
                        break;
                }

                if (request.UserAgent.Contains("Android"))
                {
                    model.Sys_Android = 1;
                }
                else if (request.UserAgent.Contains("iPad") || request.UserAgent.Contains("iPhone") || request.UserAgent.Contains("Mac OS"))
                {
                    model.Sys_MAC_OS = 1;
                }
                else if (request.UserAgent.Contains("Linux") && request.UserAgent.Contains("KFAPWI"))
                {
                    model.Sys_Linux = 1;
                }
                else if (request.UserAgent.Contains("Windows NT"))
                {
                    model.Sys_Windows = 1;
                }
                else
                {
                    model.Sys_Other = 1;
                }
                if (typeDevice == (int)Notification.Device.tablet)
                {
                    model.Device_Tablet = 1;
                }
                else if (typeDevice == (int)Notification.Device.desktop)
                {
                    model.Device_Desktop = 1;
                }
                else if (typeDevice == (int)Notification.Device.mobile)
                {
                    model.Device_Mobile = 1;
                }
                else
                {
                    model.Device_TV = 1;
                }
                _NumberUserAccess.Create(model);
                unitOfWork.Save();
            }
            else
            {
                switch (typeDevice)
                {
                    case (int)Notification.Device.tablet:
                        data.Device_Tablet += 1;
                        break;
                    case (int)Notification.Device.desktop:
                        data.Device_Desktop += 1;
                        break;
                    case (int)Notification.Device.mobile:
                        data.Device_Mobile += 1;
                        break;
                    default:
                        data.Device_TV += 1;
                        break;
                }

                if (request.UserAgent.Contains("Android"))
                {
                    data.Sys_Android += 1;
                }
                else if (request.UserAgent.Contains("iPad") || request.UserAgent.Contains("iPhone") || request.UserAgent.Contains("Mac OS"))
                {
                    data.Sys_MAC_OS += 1;
                }
                else if (request.UserAgent.Contains("Linux") && request.UserAgent.Contains("KFAPWI"))
                {
                    data.Sys_Linux += 1;
                }
                else if (request.UserAgent.Contains("Windows NT"))
                {
                    data.Sys_Windows += 1;
                }
                else
                {
                    data.Sys_Other += 1;
                }

                data.Number += 1;
                unitOfWork.Save();
            }
        }

        /// <summary>
        /// tính tỷ lệ % các hệ điều hành truy cập
        /// </summary>
        /// <returns></returns>
        public object GetSystem()
        {
            var count = _NumberUserAccess.All().Sum(o => o.Sys_Android
                                                        + o.Sys_Linux
                                                        + o.Sys_MAC_OS
                                                        + o.Sys_Other
                                                        + o.Sys_Windows);
            var result = new List<Chart> { new Chart { label= "Android",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Sys_Android),(count??0)) },
                                           new Chart { label= "MAC_OS",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Sys_MAC_OS),(count??0)) },
                                            new Chart { label= "Linux",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Sys_Linux),(count??0)) },
                                            new Chart { label= "Windows",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Sys_Windows),(count??0)) },
                                             new Chart { label= "Other",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Sys_Other),(count??0)) }};
            return result;
        }

        /// <summary>
        /// tính tỷ lệ % các hệ điều hành truy cập
        /// </summary>
        /// <returns></returns>
        public object GetDevice()
        {
            var count = _NumberUserAccess.All().Sum(o => o.Device_Desktop
                                                        + o.Device_Mobile
                                                        + o.Device_Tablet
                                                        + o.Device_TV);
            var result = new List<Chart> { new Chart { label= "Desktop",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Device_Desktop),(count??0)) },
                                           new Chart { label= "Mobile",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Device_Mobile),(count??0)) },
                                            new Chart { label= "Tablet",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Device_Tablet),(count??0)) },
                                            new Chart { label= "Smart TV",y = StaticVariable.RateCalculation((double)_NumberUserAccess.All().Sum(o=>o.Device_TV),(count??0)) }  };
            return result;
        }
    }
}
