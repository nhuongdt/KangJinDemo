using Model_banhang24vn.Common;
using Model_banhang24vn.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model_banhang24vn.CustomView;
using log4net;
using System.Reflection;
using Model_banhang24vn;
using System.Web;
using System.IO;
using Open24.Areas.AdminPage.Hellper;
using Open24.Appcache;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiCustomerController : ApiBaseController
    {
        #region Declare

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CustomerService _CustomerService;
        private readonly NganhNgheKinhDoanhService _NganhNgheKinhDoanhService;
        private readonly TinhThanhService _TinhThanhService;
        public ApiCustomerController()
        {
            _CustomerService = new CustomerService();
            _NganhNgheKinhDoanhService = new NganhNgheKinhDoanhService();
            _TinhThanhService = new TinhThanhService();
        }

        #endregion

        [HttpGet]
        public IHttpActionResult GetHome()
        {
            try
            {
                    var data = _CustomerService.GetAll()
                                                .OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate).Take(8)
                                                .Select(o => new
                                                 {
                                                   Id = o.ID,
                                                   Name = o.Name,
                                                   note = o.Note,
                                                   Images = o.Images,
                                                   Url = o.Url
                                               }).AsEnumerable();
                    
                    return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Lấy danh sách khi vào form đối tác
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            try
            {
                var data = _CustomerService.GetAllDetailJoin().OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
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
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        ///  Tìm kiếm grid 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SearchGrid(DataGridView model)
        {
            try
            {
                var data = _CustomerService.SearhGrid(model.Search);
                if (model.Sort == (int)GridPagedingHellper.GridSort.SortUp)
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columCustomer.City:
                            data = data.OrderBy(o => o.DistrictCityname);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Prioritize:
                            data = data.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
                            break;
                        case (int)GridPagedingHellper.columCustomer.CreateDate:
                            data = data.OrderBy(o => o.CreatedDate);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Email:
                            data = data.OrderBy(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Name:
                            data = data.OrderBy(o => o.Name);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Phone:
                            data = data.OrderBy(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columCustomer.status:
                            data = data.OrderBy(o => o.Status);
                            break;
                        case (int)GridPagedingHellper.columCustomer.TypeBussines:
                            data = data.OrderBy(o => o.TypeBusinessname);
                            break;
                        default:
                            data = data.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
                            break;
                    }
                }
                else
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columCustomer.City:
                            data = data.OrderByDescending(o => o.DistrictCityname);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Prioritize:
                            data = data.OrderByDescending(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
                            break;
                        case (int)GridPagedingHellper.columCustomer.CreateDate:
                            data = data.OrderByDescending(o => o.CreatedDate);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Email:
                            data = data.OrderByDescending(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Name:
                            data = data.OrderByDescending(o => o.Name);
                            break;
                        case (int)GridPagedingHellper.columCustomer.Phone:
                            data = data.OrderByDescending(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columCustomer.status:
                            data = data.OrderByDescending(o => o.Status);
                            break;
                        case (int)GridPagedingHellper.columCustomer.TypeBussines:
                            data = data.OrderByDescending(o => o.TypeBusinessname);
                            break;
                        default:
                            data = data.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
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
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// load dữ liệu combobox
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetValueCombobox()
        {
            try
            {
                var dataNghekd = _NganhNgheKinhDoanhService.Query.OrderBy(o => o.TenNganhNghe).Select(o => new { ID = o.ID, TEN = o.TenNganhNghe }).ToList();
                var dataTinhthanh = _TinhThanhService.GetTinhThanhQuanHuyen().OrderBy(o => o.TinhThanh).Select(o => new { ID = o.ID, TEN = o.QuanHuyen+" - "+o.TinhThanh }).AsEnumerable();
                return RetunJsonAction(true, string.Empty, new { DataTT = dataTinhthanh, DataNN = dataNghekd });
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();

        }

        /// <summary>
        /// Lấy chi tiết đối tượng để sửa
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetDetail(int Id)
        {
            try
            {
                var result = _CustomerService.GetBykey(Id);
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }
        /// <summary>
        /// Lấy chi tiết đối tượng để View
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetViewDetail(int Id)
        {
            try
            {
                var result = _CustomerService.GetViewDetail(Id);
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }
        /// <summary>
        /// Thêm mới đối tác
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertCustomer(Customer model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Lõi không lấy được thông tin đối tác cần thêm mới.");
                }
                if (!string.IsNullOrWhiteSpace(model.Email) && !StaticVariable.EmailIsValid(model.Email.Trim()))
                {
                    return ActionFalseNotData("Địa chỉ email không hợp lệ.");
                }
                else if (!string.IsNullOrWhiteSpace(model.Phone) && !StaticVariable.PhoneIsValid(model.Phone.Trim()))
                {
                    return ActionFalseNotData("Số điện thoại không hợp lệ.");
                }
                model.CreatedBy = contant.SESSIONNGUOIDUNG.UserName;
                var result = _CustomerService.Insert(model);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    RemoveCache();
                    return InsertSuccess();
                }

            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Xóa đối tác
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteCustomer(Customer model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Lõi không lấy được thông tin đối tác cần xóa.");
                }
                var result = _CustomerService.Delete(model.ID);

                if (result == (int)Notification.ErrorCode.success)
                {
                    RemoveCache();
                    return DeleteSuccess();
                }
                else if (result == (int)Notification.ErrorCode.notfound)
                {
                    return ActionFalseNotData("Đối tác đã bị xóa hoặc không tồn tại. vui lòng tải lại trang.");
                }

            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        ///  Cập nhật lại đối tác
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateCustomer(Customer model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Lỗi không lấy được thông tin đối tác cần thêm mới.");
                }
                if (!string.IsNullOrWhiteSpace(model.Email) && !StaticVariable.EmailIsValid(model.Email.Trim()))
                {
                    return ActionFalseNotData("Địa chỉ email không hợp lệ.");
                }
                else if (!string.IsNullOrWhiteSpace(model.Phone) && !StaticVariable.PhoneIsValid(model.Phone.Trim()))
                {
                    return ActionFalseNotData("Số điện thoại không hợp lệ.");
                }
                model.ModifiedBy = contant.SESSIONNGUOIDUNG.UserName;
                var result = _CustomerService.Update(model);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    if(!string.IsNullOrWhiteSpace(result.Data))
                    {
                        EventUpdateCache.DeleteFileImeagesCache(HttpContext.Current.Server.MapPath("/Appcache/manifest.appcache"), result.Data);
                    }
                    RemoveCache();
                    return UpdateSuccess();
                }
                else
                {
                    return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        ///  Lấy dữ liệu cho slider ở home index client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetShowSlider()
        {
            try
            {
                if (!CacheHellper.IsSet(CacheKey.Customer_Home_Slider))
                {
                    var data = _CustomerService.GetAllDetailJoin()
                                                .OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate).Take(20)
                                                                    .Select(o => new
                                                                    {
                                                                        Id = o.ID,
                                                                        Name = o.Name,
                                                                        note = o.Note,
                                                                        Images = o.Images,
                                                                        Url = o.Url,
                                                                        Adress = o.Adress + " " + o.DistrictCityname,
                                                                    }).ToList();

                    CacheHellper.Set(CacheKey.Customer_Home_Slider, data);
                    return RetunJsonAction(true, string.Empty, data);
                }
                else
                    return RetunJsonAction(true, string.Empty, CacheHellper.Get(CacheKey.Customer_Home_Slider));

            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Lấy dữ liệu trang khách hàng client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetClient()
        {
            try
            {
                if (!CacheHellper.IsSet(CacheKey.Customer_All))
                {
                    int pageCount = (int)Math.Ceiling((double)_CustomerService.GetAllDetailJoin().Count() / Notification.CustomerClientDefault);
                    var data = _CustomerService.GetAllDetailJoin()
                                                 .OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate)
                                                .Take(Notification.CustomerClientDefault)
                                                                    .Select(o => new
                                                                    {
                                                                        Id = o.ID,
                                                                        Name = o.Name,
                                                                        note = o.Note,
                                                                        Images = o.Images,
                                                                        Url = o.Url,
                                                                        Adress = o.Adress == null || o.Adress == "" ? o.DistrictCityname : o.Adress + ", " + o.DistrictCityname,
                                                                        date = o.CreatedDate
                                                                    }).OrderByDescending(o => o.date).ToList();

                    if (data.Count > (Notification.CustomerClientDefault / 2))
                    {
                        var left = data.Take(Notification.CustomerClientDefault / 2).ToList();
                        var right = data.Skip(left.Count).Take(Notification.CustomerClientDefault).ToList();
                        CacheHellper.Set(CacheKey.Customer_All, new { left = left, right = right, pageCount = pageCount });
                        return RetunJsonAction(true, string.Empty, new { left = left, right = right, pageCount = pageCount });
                    }
                    else
                    {
                        var left = data.Take(1).ToList();
                        var right = data.Skip(1).Take(Notification.CustomerClientDefault).ToList();
                        CacheHellper.Set(CacheKey.Customer_All, new { left = left, right = right, pageCount = pageCount });
                        return RetunJsonAction(true, string.Empty, new { left = left, right = right, pageCount = pageCount });
                    }
                }
                else
                    return RetunJsonAction(true, string.Empty, CacheHellper.Get(CacheKey.Customer_All));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Tìm kiếm trên trang khách hàng client
        /// </summary>
        /// <param name="Search"></param>
        /// <param name="Adress"></param>
        /// <param name="business"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult SearchFilter(string Search, Guid? Adress, Guid? business,int? page)
        {
            try
            {
                var model = _CustomerService.SearFilter(Search, business, Adress);
                int pageCount = (int)Math.Ceiling((double)model.Count() / Notification.CustomerClientDefault);
                var data = model.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate)
                                  .Skip((page ?? 0) * Notification.CustomerClientDefault)
                                  .Take(Notification.CustomerClientDefault)
                                                                .Select(o => new
                                                                {
                                                                    Id = o.ID,
                                                                    Name = o.Name,
                                                                    note = o.Note,
                                                                    Images = o.Images,
                                                                    Url = o.Url,
                                                                    Adress = o.Adress == null || o.Adress == "" ? o.DistrictCityname : o.Adress + ", " + o.DistrictCityname,
                                                                    date = o.CreatedDate
                                                                }).ToList();
                if (data.Count == 0)
                {
                    var left = data.ToList();
                    var right = data.ToList();
                    return RetunJsonAction(true, string.Empty, new { left = left, right = right, pageCount = pageCount, IsShow = false });
                }
                else if (data.Count > (Notification.CustomerClientDefault / 2))
                {
                    var left = data.Take(Notification.CustomerClientDefault / 2).ToList();
                    var right = data.Skip(left.Count).Take(Notification.CustomerClientDefault).ToList();
                    return RetunJsonAction(true, string.Empty, new { left = left, right = right, pageCount = pageCount, IsShow = true });
                }
                else
                {
                    var left = data.Take(1).ToList();
                    var right = data.Skip(1).Take(Notification.CustomerClientDefault).ToList();
                    return RetunJsonAction(true, string.Empty, new { left = left, right = right, pageCount = pageCount, IsShow = true });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Tìm kiếm trên trang khách hàng client
        /// </summary>
        /// <param name="Search"></param>
        /// <param name="Adress"></param>
        /// <param name="business"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult SearchformFilter(string Search, Guid? Adress, Guid? business, int? page)
        {
            try
            {
                var model = _CustomerService.SearFilter(Search, business, Adress);
                int pageCount = (int)Math.Ceiling((double)model.Count() / Notification.CustomerClientDefault);
                var data = model.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate)
                                  .Skip((page ?? 0) * Notification.CustomerClientDefault)
                                  .Take(Notification.CustomerClientDefault)
                                                                .Select(o => new
                                                                {
                                                                    Id = o.ID,
                                                                    Name = o.Name,
                                                                    note = o.Note,
                                                                    Images = o.Images,
                                                                    Url = o.Url,
                                                                    Adress = o.Adress == null || o.Adress == "" ? o.DistrictCityname : o.Adress + ", " + o.DistrictCityname,
                                                                    date = o.CreatedDate
                                                                }).ToList();
               
                    return RetunJsonAction(true, string.Empty,new { data= data, isshow= data.Count == Notification.CustomerClientDefault });
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Lấy dữ liệu combobox có defautl thêm dữ liệu client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCombobxforSearch()
        {
            try
            {
                var dataNghekd = _NganhNgheKinhDoanhService.Query.OrderBy(o => o.TenNganhNghe).Select(o => new { ID = o.ID, TEN = o.TenNganhNghe }).ToList();
                var dataTinhthanh = _TinhThanhService.GetTinhThanhQuanHuyen().AsEnumerable().GroupBy(o=>o.TinhThanh).Select(o => new { ID = o.FirstOrDefault().ID, TEN = o.Key }).OrderBy(o=>o.TEN).ToList();
                dataTinhthanh.Insert(0, new { ID = new Guid(), TEN = "Khu vực" });
                dataNghekd.Insert(0, new { ID = new Guid(), TEN = "Chọn loại hình kinh doanh" });
                return RetunJsonAction(true, string.Empty, new { DataTT = dataTinhthanh, DataNN = dataNghekd });
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();

        }
        /// <summary>
        /// Lấy các bản ghi khách hàng mới nhât
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCustomerNewDate()
        {
            try
            {
                if (!CacheHellper.IsSet(CacheKey.Customer_NewDate))
                {
                    var data = _CustomerService.GetAllDetailJoin().Take(Notification.PageClientDefault).ToList();
                    CacheHellper.Set(CacheKey.Customer_NewDate, data);
                    return RetunJsonAction(true, string.Empty, data);
                }
                else
                {
                    return RetunJsonAction(true, string.Empty, CacheHellper.Get(CacheKey.Customer_NewDate));
                }
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// upload ảnh đối tác
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UploadImagesCustomer()
        {
            try
            {
                var path = "";
                string result = "";
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];
                    var filenameImage = Guid.NewGuid().ToString() + ".jpg";
                    DateTime time = DateTime.Now;
                    string format = "yyyyMMdd";
                    var dt = time.ToString(format);
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Img/Doitac/" + dt)))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Img/doitac/" + dt));
                    }

                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/Img/doitac/" + dt), filenameImage);

                    file.SaveAs(path);
                    result = "/Img/doitac/" + dt + "/" + filenameImage;
                    EventUpdateCache.AddFileImeagesCache(HttpContext.Current.Server.MapPath("/Appcache/manifest.appcache"), result);
                }
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(" - UploadImage :{0}", ex.Message);
            }
            return Exeption();
        }

        private void RemoveCache()
        {
            CacheHellper.Invalidate(CacheKey.Customer_All);
            CacheHellper.Invalidate(CacheKey.Customer_Home_Slider);
            CacheHellper.Invalidate(CacheKey.Customer_NewDate);
        }
    }
}
