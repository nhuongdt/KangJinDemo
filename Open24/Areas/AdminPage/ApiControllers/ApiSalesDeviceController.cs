using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiSalesDeviceController : ApiBaseController
    {
        private SalesDeviceService _SalesDeviceService;

        public ApiSalesDeviceController()
        {
            _SalesDeviceService = new SalesDeviceService();
        }
        /// <summary>
        /// Tìm kiếm trên grid 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SearchGrid(DataGridView model)
        {
            try
            {
                var data = _SalesDeviceService.GetSelectJoin(model.Search);
                //if (model.Sort == (int)GridPagedingHellper.GridSort.SortUp)
                //{
                //    switch (model.Columname)
                //    {
                //        case (int)GridPagedingHellper.columCustomer.City:
                //            data = data.OrderBy(o => o.DistrictCityname);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Prioritize:
                //            data = data.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.CreateDate:
                //            data = data.OrderBy(o => o.CreatedDate);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Email:
                //            data = data.OrderBy(o => o.Email);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Name:
                //            data = data.OrderBy(o => o.Name);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Phone:
                //            data = data.OrderBy(o => o.Phone);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.status:
                //            data = data.OrderBy(o => o.Status);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.TypeBussines:
                //            data = data.OrderBy(o => o.TypeBusinessname);
                //            break;
                //        default:
                //            data = data.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
                //            break;
                //    }
                //}
                //else
                //{
                //    switch (model.Columname)
                //    {
                //        case (int)GridPagedingHellper.columCustomer.City:
                //            data = data.OrderByDescending(o => o.DistrictCityname);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Prioritize:
                //            data = data.OrderByDescending(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.CreateDate:
                //            data = data.OrderByDescending(o => o.CreatedDate);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Email:
                //            data = data.OrderByDescending(o => o.Email);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Name:
                //            data = data.OrderByDescending(o => o.Name);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.Phone:
                //            data = data.OrderByDescending(o => o.Phone);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.status:
                //            data = data.OrderByDescending(o => o.Status);
                //            break;
                //        case (int)GridPagedingHellper.columCustomer.TypeBussines:
                //            data = data.OrderByDescending(o => o.TypeBusinessname);
                //            break;
                //        default:
                //            data = data.OrderBy(o => o.prioritize).ThenByDescending(o => o.ModifiedDate);
                //            break;
                //    }
                //}
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
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Get combobox loại thiết bị
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetGroupDevices()
        {
            return Json(_SalesDeviceService.GetAllGroupDevices().Select(o=>new { ID=o.ID, Encoder=o.Encoder, Name = o.Name,Note=o.Note, Status = o.Status}).AsEnumerable());
        }

        /// <summary>
        /// thêm mới và sửa 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SaveSalesDevices(SalesDevice model)
        {
            try
            {
                if (model != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Name))
                    {
                        return ActionFalseNotData("Vui lòng nhập tên thiết bị");
                    }
                    if (model.ID != 0)// Update
                    {
                        model.ViewBuy = 0;
                        model.EditDate = DateTime.Now;
                        model.EditUser = contant.SESSIONNGUOIDUNG.UserID;
                       var result= _SalesDeviceService.UpdateSalesDevices(model);
                        if (result.ErrorCode == (int)Notification.ErrorCode.success)
                        {
                            return UpdateSuccess();
                        }
                        else
                        {
                            return ActionFalseNotData(result.Data);
                        }

                    }
                    else
                    {
                        model.ViewBuy = 0;
                        model.EditDate = DateTime.Now;
                        model.EditUser = contant.SESSIONNGUOIDUNG.UserID;
                        var result = _SalesDeviceService.InsertSalesDevices(model);
                        if (result == (int)Notification.ErrorCode.success)
                        { return InsertSuccess(); }
                        else
                        {
                            return ActionFalseNotData("Nhóm thiết bị đã chọn không tồn tại hoặc đã bị xóa");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Exeption(e);
            }
            return ActionFalseNotData("Không lấy được dữ liệu");
        }

        /// <summary>
        /// xóa thiết bị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteSalesDevices(SalesDeviceView model)
        {
            try
            {
                if (model != null)
                {
                    _SalesDeviceService.DeleteSalesDevices(model.ID);
                    return DeleteSuccess();
                }
            }
            catch (Exception e)
            {
                return Exeption(e);
            }
            return ActionFalseNotData("Không lấy được dữ liệu cần xóa");
        }

       /// <summary>
       /// Thêm mới và sửa nhóm thiết bị
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SaveSalesGroupDevices(SalesGroupDevice model)
        {
            try
            {
                if (model != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Name))
                    {
                        return ActionFalseNotData("Vui lòng nhập tên nhóm thiết bị");
                    }
                    var result = new JsonViewModel<string>();
                    if (model.ID != 0)// Update
                    {
                         result = _SalesDeviceService.UpdateSalesGroupDevices(model);
                        if (result.ErrorCode == (int)Notification.ErrorCode.success)
                        {
                            return UpdateSuccess();
                        }
                    }
                    else
                    {
                        result = _SalesDeviceService.InsertSalesGroupDevices(model);
                        if (result.ErrorCode == (int)Notification.ErrorCode.success)
                        {
                            return InsertSuccess();
                        }
                    }
                    return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception e)
            {
                return Exeption(e);
            }
            return ActionFalseNotData("Không lấy được dữ liệu");
        }

        /// <summary>
        /// Xóa nhóm thiết bị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteSalesGroupDevices(SalesGroupDevice model)
        {
            try
            {
                if (model != null)
                {
                    var result= _SalesDeviceService.DeleteSalesGroupDevices(model.ID);
                    if(result.ErrorCode == (int)Notification.ErrorCode.success)
                    {

                        return DeleteSuccess();
                    }
                        return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception e)
            {
                return Exeption(e);
            }
            return ActionFalseNotData("Không lấy được dữ liệu cần xóa");

        }

        [HttpGet]
        public IHttpActionResult GetAllDevice()
        {
            try
            {
                var data = _SalesDeviceService.Query.Where(o=>o.Status==true).Select(o => new {
                    ID = o.ID,
                    Name = o.Name,
                    Encoder=o.Encoder,
                    TimeGuarantee = o.TimeGuarantee!=null&&o.TimeGuarantee>0? "Bảo hành " + o.TimeGuarantee + " tháng":string.Empty,
                    PriceSale=o.PriceSale,
                    Price=o.Price,
                    Url= o.Url,
                    IsSalePrice =o.IsSalePrice,
                    SrcImg=o.SalesImgDevices.Select(c=>c.SrcImage).FirstOrDefault()
                }).AsEnumerable();
                return RetunJsonAction(true, string.Empty,data );
            }
            catch (Exception e)
            {
                return Exeption(e);
            }
        }

        [HttpGet]
        public IHttpActionResult GetDetailDevice(long? id)
        {
            try
            {
                if (id != null)
                {
                    var model = _SalesDeviceService.GetSelectJoin(null).Where(o=>o.Status==true).FirstOrDefault(o => o.ID == id);
                    if (model != null)
                    {
                        var dataRelated = _SalesDeviceService.GetDataForGroupDevices(model.GroupDeviceId).Where(o=>o.ID!=model.ID && o.Status==true).Select(o => new {
                            ID = o.ID,
                            Name = o.Name,
                            TimeGuarantee = o.TimeGuarantee != null && o.TimeGuarantee > 0 ? "Bảo hành " + o.TimeGuarantee + " tháng" : string.Empty,
                            PriceSale = o.PriceSale,
                            Price = o.Price,
                            Url = o.Url,
                            IsSalePrice = o.IsSalePrice,
                            Encoder = o.Encoder,
                            SrcImg = o.SalesImgDevices.Select(c => c.SrcImage).FirstOrDefault()
                        }).AsEnumerable();
                        var listMenu = _SalesDeviceService.GetAllGroupDevices().Where(o=>o.Status==true).Select(o =>
                         new
                         {
                             ID = o.ID,
                             Name = o.Name,
                             SalesDevices=o.SalesDevices.Where(c=>c.Status==true).Select(c=>new {
                                 Img_ID=c.ID,
                                 Img_Name=c.Name,
                                 Img_Url=c.Url
                             }).AsEnumerable()

                         }).AsEnumerable();
                        return RetunJsonAction(true, string.Empty, new {detail=model, dataRelated = dataRelated , listMenu = listMenu });
                    }
                }
                return ActionFalseNotData("Không tồn tại sản phẩm ");
            }
            catch (Exception e)
            {
                return Exeption(e);
            }
        }
    }
}
