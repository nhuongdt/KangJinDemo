using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using log4net;
using System.Reflection;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiStoreRegistrationController : ApiBaseController
    {
        #region Declare
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private CuaHangDangKyService _CuaHangDangKyService;

        public ApiStoreRegistrationController()
        {
            _CuaHangDangKyService = new CuaHangDangKyService();
        }
        #endregion
        #region get data bind custom pageding grid

        /// <summary>
        /// Lấy danh sách đăng ký cửa hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DataGridView GetListAllData()
        {
            var listUser = _CuaHangDangKyService.Query;
            var view = new DataGridView()
            {
                Page = 1,
                PageCount = (int)Math.Ceiling((double)listUser.Count() / Notification.PageDefault),
                Data = listUser.Take(Notification.PageDefault).Select(o => new StoreRegistrationView
                {
                    Mobile = o.SoDienThoai,
                    SubDomain = o.SubDomain + ".Open24.vn",
                    Name = o.HoTen,
                    ExpiryDate = o.HanSuDung,
                    Business = o.NganhNgheKinhDoanh.TenNganhNghe,
                    Status = o.TrangThai,
                    SoLanKichHoat = o.SoLanKichHoat,
                    DiaChi = o.DiaChi,
                    Email = o.Email,
                    MaKichHoat = o.MaKichHoat,
                    NgayTao = o.NgayTao,
                    Quanhuyen = o.TinhThanh_QuanHuyen != null ? o.TinhThanh_QuanHuyen.QuanHuyen + " - " + o.TinhThanh_QuanHuyen.TinhThanh : string.Empty,
                    TenCuaHang = o.TenCuaHang,
                    DiaChiIP_DK = o.DiaChiIP_DK,
                    HeDieuHanh_DK = o.HeDieuHanh_DK,
                    KhuVuc_DK = o.KhuVuc_DK,
                    ThietBi_DK = o.ThietBi_DK
                }).ToList(),
            };
            if (view.PageCount == 0)
            {
                view.PageCount = 1;
            }
            view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, listUser.Count());
            return view;
        }

        /// <summary>
        /// Tìm kiếm danh sách khách hàng được giới thiệu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SearchContract(DataGridView model)
        {
            try
            {
                var data = _CuaHangDangKyService.SearchContract(model.Search);
                if (model.Sort == (int)GridPagedingHellper.GridSort.SortUp)
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columContract.Name:
                            data = data.OrderBy(o => o.Name);
                            break;
                        case (int)GridPagedingHellper.columContract.Phone:
                            data = data.OrderBy(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columContract.IT_Phone:
                            data = data.OrderBy(o => o.IT_Phone);
                            break;
                        case (int)GridPagedingHellper.columContract.IT_Name:
                            data = data.OrderBy(o => o.IT_Name);
                            break;
                        case (int)GridPagedingHellper.columContract.ModifiedDate:
                            data = data.OrderBy(o => o.ModifiedDate);
                            break;
                        case (int)GridPagedingHellper.columContract.ModifiedBy:
                            data = data.OrderBy(o => o.ModifiedBy);
                            break;
                        case (int)GridPagedingHellper.columContract.Status:
                            data = data.OrderBy(o => o.Status);
                            break;
                        default:
                            data = data.OrderBy(o => o.CreatedDate);
                            break;
                    }
                }
                else
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columContract.Name:
                            data = data.OrderByDescending(o => o.Name);
                            break;
                        case (int)GridPagedingHellper.columContract.Phone:
                            data = data.OrderByDescending(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columContract.IT_Name:
                            data = data.OrderByDescending(o => o.IT_Name);
                            break;
                        case (int)GridPagedingHellper.columContract.IT_Phone:
                            data = data.OrderByDescending(o => o.IT_Phone);
                            break;
                        case (int)GridPagedingHellper.columContract.ModifiedDate:
                            data = data.OrderByDescending(o => o.ModifiedDate);
                            break;
                        case (int)GridPagedingHellper.columContract.ModifiedBy:
                            data = data.OrderByDescending(o => o.ModifiedBy);
                            break;
                        case (int)GridPagedingHellper.columContract.Status:
                            data = data.OrderByDescending(o => o.Status);
                            break;
                        default:
                            data = data.OrderBy(o => o.CreatedDate);
                            break;
                    }
                }
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.AsEnumerable().Select(o => new
                    
                    {
                        ID = o.ID,
                        ModifiedBy = o.ModifiedBy,
                        ModifiedDate = o.ModifiedDate,
                        Name = o.Name,
                        CreatedDate = o.CreatedDate,
                        Phone = o.Phone,
                        StoreOpen = o.StoreOpen,
                        IT_Name = o.IT_Name,
                        IT_Phone = o.IT_Phone,
                        Status = o.Status,
                        IsPhone = (o.Status == (int)Notification.StatusContract.DangXacThuc || o.Status == (int)Notification.StatusContract.DaXacThuc),
                        Statuss = Notification.TrangThaiContract.Where(c => c.Key == o.Status).AsEnumerable().Select(c => c.Value).FirstOrDefault()
                    }).AsEnumerable();
                }
                else
                {
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).AsEnumerable().Select(o => new
                   
                    {
                        ID = o.ID,
                        ModifiedBy = o.ModifiedBy,
                        ModifiedDate = o.ModifiedDate,
                        Name = o.Name,
                        CreatedDate = o.CreatedDate,
                        Phone = o.Phone,
                        StoreOpen = o.StoreOpen,
                        IT_Name = o.IT_Name,
                        IT_Phone = o.IT_Phone,
                        Status = o.Status,
                        IsPhone= (o.Status==(int)Notification.StatusContract.DangXacThuc||o.Status== (int)Notification.StatusContract.DaXacThuc),
                        Statuss = Notification.TrangThaiContract.Where(c => c.Key == o.Status).AsEnumerable().Select(c => c.Value).FirstOrDefault()
                    }).AsEnumerable();
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }

        /// <summary>
        /// Tìm kiếm danh sách gói dịch vụ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ///  [HttpPost]
        public IHttpActionResult SearchGoiDichVu(DataGridView model)
        {
            try
            {
                var data = _CuaHangDangKyService.GetAllGoiDichVu();
                if (!string.IsNullOrWhiteSpace(model.Search))
                {
                    model.Search = model.Search.ToUpper();
                     data = _CuaHangDangKyService.GetAllGoiDichVu().Where(o => o.TenGoi.ToUpper().Contains(model.Search));

                }
                var result = data.Select(o => new
                {
                    o.ID,
                    o.Gia,
                    o.GhiChu,
                    o.SLChiNhanh,
                    o.SLMatHang,
                    o.SLNguoiDung,
                    o.TenGoi,
                    o.TrangThai
                }).AsEnumerable();
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        /// <summary>
        ///  Cập nhật + thêm mới gói dịch vụ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult UpdateGoiDichVu(DM_GoiDichVu model, bool IsNew = false)
        {
            try
            {
                if (IsNew)
                {
                    var result = _CuaHangDangKyService.InsertGoiDichVu(model);
                    if (result)
                    {
                        return InsertSuccess();
                    }
                    return ActionFalseNotData("Tên gói đã tồn tại");
                }
                else
                {
                    var result = _CuaHangDangKyService.UpdateGoiDichVu(model);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        return UpdateSuccess();
                    }
                    return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception ex)
            {

                return Exeption(ex);
            }
        }

        /// <summary>
        /// thay đổi trạng thái
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SaveStatusContract(Contract model)
        {
            try
            {
                if(model==null|| model.ID==0 || model.Status==null)
                {
                    return ActionFalseNotData("Không tìm thấy dữ liệu cần cập nhật");
                }
                model.ModifiedBy = contant.SESSIONNGUOIDUNG.UserName;
                var result = _CuaHangDangKyService.ChangeStatusContract(model);
                if(result.ErrorCode==(int)Notification.ErrorCode.success)
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

        [HttpGet]
        public IHttpActionResult GetHanSuDung()
        {
            return Ok(Notification.HanSuDung.Select(o => new { Key = o.Key, Value = o.Value }).AsEnumerable());
        }

        [HttpGet]
        public IHttpActionResult GetDataFirst()
        {
            try
            {
                var hansudung = Notification.HanSuDung.Select(o => new { Key = o.Key, Value = o.Value }).AsEnumerable();
                var goidichvu = _CuaHangDangKyService.GetAllGoiDichVu().Where(o=>o.TrangThai==true).Select(o=>new {
                    o.ID,
                    o.TenGoi

                }).AsEnumerable();
                var model = new { ListHSD = hansudung, GoiDichVu = goidichvu };
                return RetunJsonAction(true,string.Empty, model);
            }
            catch(Exception exx)
            {
                return Exeption(exx);
            }
        }

        ///// <summary>
        ///// Tìm kiếm gridview
        ///// </summary>
        ///// <param name="daTatable"></param>
        ///// <returns></returns>
        // [HttpGet]
        //public IHttpActionResult GetDataForShearch([FromBody]DataGridView data)
        //{
        //    var model = _CuaHangDangKyService.Search(data.Search, data.Datetime, data.Status);
        //    if (data.Sort == (int)GridPagedingHellper.GridSort.SortUp)
        //    {

        //        switch (data.Columname)
        //        {
        //            case (int)GridPagedingHellper.columtableStoreRegistration.bussines:
        //                model = model.Where(o => o.NganhNgheKinhDoanh != null).OrderBy(o => o.NganhNgheKinhDoanh.TenNganhNghe);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.expiryDate:
        //                model = model.OrderBy(o => o.HanSuDung);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.mobile:
        //                model = model.OrderBy(o => o.SoDienThoai);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.name:
        //                model = model.OrderBy(o => o.HoTen);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.status:
        //                model = model.OrderBy(o => o.TrangThai);
        //                break;
        //            default:
        //                model = model.OrderBy(o => o.SubDomain);
        //                break;

        //        }
        //    }
        //    else
        //    {
        //        switch (data.Columname)
        //        {
        //            case (int)GridPagedingHellper.columtableStoreRegistration.bussines:
        //                model = model.Where(o => o.NganhNgheKinhDoanh != null).OrderByDescending(o => o.NganhNgheKinhDoanh.TenNganhNghe);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.expiryDate:
        //                model = model.OrderByDescending(o => o.HanSuDung);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.mobile:
        //                model = model.OrderByDescending(o => o.SoDienThoai);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.name:
        //                model = model.OrderByDescending(o => o.HoTen);
        //                break;
        //            case (int)GridPagedingHellper.columtableStoreRegistration.status:
        //                model = model.OrderByDescending(o => o.TrangThai);
        //                break;
        //            default:
        //                model = model.OrderByDescending(o => o.SubDomain);
        //                break;

        //        }
        //    }
        //    data.PageCount = (int)Math.Ceiling((double)model.Count() / data.Limit);
        //    if (data.PageCount == 0 || data.PageCount == 1)
        //    {
        //        data.PageCount = 1;
        //        data.Page = 1;
        //        data.Data = model.Select(o => new StoreRegistrationView
        //        {
        //            Mobile = o.SoDienThoai,
        //            SubDomain = o.SubDomain + ".Open24.vn",
        //            Name = o.HoTen,
        //            ExpiryDate = o.HanSuDung,
        //            Business = o.NganhNgheKinhDoanh.TenNganhNghe,
        //            Status = o.TrangThai
        //        }).ToList();
        //    }
        //    else
        //    {
        //        data.Data = model.Skip(data.Limit * (data.Page - 1)).Take(data.Limit).Select(o => new StoreRegistrationView
        //        {
        //            Mobile = o.SoDienThoai,
        //            SubDomain = o.SubDomain + ".Open24.vn",
        //            Name = o.HoTen,
        //            ExpiryDate = o.HanSuDung,
        //            Business = o.NganhNgheKinhDoanh.TenNganhNghe,
        //            Status = o.TrangThai
        //        }).ToList();
        //    }
        //    data.PageItem = GridPagedingHellper.PageItems(data.Page, data.PageCount, model.Count());
        //    return CreatedAtRoute("DefaultApi", new { ress=true }, data);

        //}


        #endregion

        /// <summary>
        /// Cập nhật trạng thái và hạn sử dụng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateStore(StoreRegistrationView model)
        {
            try
            {
                if (model == null) { return ActionFalseNotData("Không lấy được thông tin cửa hàng cần cập nhật"); }
                var ngdung = contant.SESSIONNGUOIDUNG;
                if (ngdung == null) { return ActionFalseNotData("Đã hết phiên làm việc, vui lòng tải lại trang"); }
                var result = _CuaHangDangKyService.UpdateStore(model, ngdung.UserName);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
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

        [HttpGet]
        public IHttpActionResult GetHistory(string subdomain)
        {
            try
            {
                var result = _CuaHangDangKyService.GetBySubDomain(subdomain).AsEnumerable().Select(o=>new {
                    o.ID,
                    TenGoi=o.DM_GoiDichVu!=null?o.DM_GoiDichVu.TenGoi:string.Empty,
                    o.GhiChu,
                    o.GiaGoiHienTai,
                    o.LoaiGiaHan,
                    o.NgayTao,
                    o.NguoiTao,
                    o.ThoiGianGiaHan

                }).AsEnumerable();
                return RetunJsonAction(true, string.Empty, result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        [HttpGet]
        public IHttpActionResult LoadChartStore()
        {
            var data = _CuaHangDangKyService.GetCharWeeks(7);
            return Json(data);
        }
        [HttpGet]
        public IHttpActionResult LoadChartStoreCity()
        {
            var data = _CuaHangDangKyService.GetChartCity();
            return Json(data);
        }
        [HttpPost]
        public IHttpActionResult SaveContract(Contract model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin giới thiệu");
                }
                if (!StaticVariable.PhoneIsValid(model.Phone))
                {
                    return ActionFalseNotData("Số điện thoại không hợp lệ");
                }
                var result = _CuaHangDangKyService.SaveContract(model);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    return InsertSuccess();
                }
                else
                {
                    return ActionFalseNotData(result.Data);
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        #region checkinput đăng ký sử dụng
        public IHttpActionResult PhoneCheck([FromBody]JObject model)
        {
            string phonenum = model["PhoneNumber"].ToObject<string>();
            if (CuaHangDangKyService.CheckPhoneNumber(phonenum))
            {
                return Conflict();
            }
            return Ok("ok");
        }

        public IHttpActionResult SubdomainCheck([FromBody]JObject model)
        {
            string subdomain = model["subdomain"].ToObject<string>();
            if (CuaHangDangKyService.CheckSubdomain(subdomain.ToLower()))
            {
                return Conflict();
            }
            return Ok("ok");
        }

        public IHttpActionResult EmailCheck([FromBody]JObject model)
        {
            string email = model["ckemail"].ToObject<string>();
            if (CuaHangDangKyService.CheckEmail(email.ToLower()))
            {
                return Conflict();
            }
            return Ok("ok");
        }
        #endregion
    }
}
