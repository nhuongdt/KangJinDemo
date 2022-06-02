using log4net;
using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;
using Open24.Areas.AdminPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class BusinessController : ApiBaseController
    {
        NganhNgheKinhDoanhService _NganhNgheKinhDoanhService = new NganhNgheKinhDoanhService();
        TinhNangNghanhNgheService _TinhNangNghanhNgheService = new TinhNangNghanhNgheService();
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        public IHttpActionResult GetAllPage()
        {
            try
            {
                var data = _NganhNgheKinhDoanhService.GetAllSelected().OrderBy(o => o.TenNganhNghe);
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
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult SearchGrid(DataGridView model)
        {
            try
            {
                var data = _NganhNgheKinhDoanhService.SearchSelected(model.Search);
                if (model.Sort == (int)GridPagedingHellper.GridSort.SortUp)
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columBusiness.ma:
                            data = data.OrderBy(o => o.MaNganhNghe);
                            break;
                        case (int)GridPagedingHellper.columBusiness.ten:
                            data = data.OrderBy(o => o.TenNganhNghe);
                            break;
                        case (int)GridPagedingHellper.columBusiness.creatby:
                            data = data.OrderBy(o => o.CreatedBy);
                            break;
                        case (int)GridPagedingHellper.columBusiness.creatdate:
                            data = data.OrderBy(o => o.CreatDate);
                            break;
                        case (int)GridPagedingHellper.columBusiness.modifiedby:
                            data = data.OrderBy(o => o.ModifiedBy);
                            break;
                        case (int)GridPagedingHellper.columBusiness.modifieddate:
                            data = data.OrderBy(o => o.ModifiedDate);
                            break;
                        case (int)GridPagedingHellper.columBusiness.status:
                            data = data.OrderBy(o => o.Status);
                            break;
                        default:
                            data = data.OrderBy(o => o.CreatDate);
                            break;
                    }
                }
                else
                {
                    switch (model.Columname)
                    {
                        case (int)GridPagedingHellper.columBusiness.ma:
                            data = data.OrderByDescending(o => o.MaNganhNghe);
                            break;
                        case (int)GridPagedingHellper.columBusiness.ten:
                            data = data.OrderByDescending(o => o.TenNganhNghe);
                            break;
                        case (int)GridPagedingHellper.columBusiness.creatby:
                            data = data.OrderByDescending(o => o.CreatedBy);
                            break;
                        case (int)GridPagedingHellper.columBusiness.creatdate:
                            data = data.OrderByDescending(o => o.CreatDate);
                            break;
                        case (int)GridPagedingHellper.columBusiness.modifiedby:
                            data = data.OrderByDescending(o => o.ModifiedBy);
                            break;
                        case (int)GridPagedingHellper.columBusiness.modifieddate:
                            data = data.OrderByDescending(o => o.ModifiedDate);
                            break;
                        case (int)GridPagedingHellper.columBusiness.status:
                            data = data.OrderByDescending(o => o.Status);
                            break;
                        default:
                            data = data.OrderByDescending(o => o.CreatDate);
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
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
        }


        public IHttpActionResult GetAllRole()
        {
            var data = _NganhNgheKinhDoanhService.GetRole().ToList();
            var json = data.Where(o => o.QuyenCha == null || o.QuyenCha == "").Select(o =>
                   new RoleParentView
                   {
                       id = o.MaQuyen,
                       text = o.TenQuyen,
                       children = GetChildren(data, o.MaQuyen)
                   });
            return Json(json.OrderBy(o => o.text));
        }

        private List<RoleParentView> GetChildren(List<HT_Quyen> data, string roleKey)
        {
            return data.Where(o => o.QuyenCha != null && o.QuyenCha.Equals(roleKey)).Select(o =>
                       new RoleParentView
                       {
                           id = o.MaQuyen,
                           text = o.TenQuyen,
                           children = GetChildren(data, o.MaQuyen)
                       }).ToList();
        }

        [HttpGet]
        public IHttpActionResult GetDetailRole(Guid? KeyId)
        {
            return Json(_NganhNgheKinhDoanhService.getDetailRole(KeyId).Select(o => o.MaQuyen));
        }

        [HttpPost]
        public IHttpActionResult Update(BusinessRole model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.TenNganhNghe))
                {
                    return ActionFalseNotData("Vui lòng nhập tên nghành nghề kinh doanh!");
                }
                if (model.ID == null)
                {
                    return ActionFalseNotData("Không lấy được bản ghi cần sửa, vui lòng chọn lại!");
                }
                model.UserCurent = contant.SESSIONNGUOIDUNG.UserName;
                var result = _NganhNgheKinhDoanhService.Update(model);
                if (result.ErrorCode != (int)Notification.ErrorCode.success)
                {
                    return ActionFalseNotData(result.Data);
                }
                return UpdateSuccess();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Exeption();
            }
        }

        [HttpPost]
        public IHttpActionResult Insert(BusinessRole model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.TenNganhNghe))
                {
                    return ActionFalseNotData("Vui lòng nhập tên nghành nghề kinh doanh!");
                }
                var data = new NganhNgheKinhDoanh
                {
                    ID = Guid.NewGuid(),
                    CreatDate = DateTime.Now,
                    CreatedBy = contant.SESSIONNGUOIDUNG.UserName,
                    Status = model.Status,
                    TenNganhNghe = model.TenNganhNghe,
                    Image = model.Image,
                    ImageMobile=model.ImageMobile
                };
                var result = _NganhNgheKinhDoanhService.Insert(data, model.checkList);
                if (result.ErrorCode != (int)Notification.ErrorCode.success)
                {
                    return ActionFalseNotData(result.Data);
                }
                return InsertSuccess();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Exeption();
            }
        }
        [HttpPost]
        public IHttpActionResult Delete(BusinessView model)
        {
            try
            {
                var result = _NganhNgheKinhDoanhService.Delete(model.ID);
                if (result.ErrorCode != (int)Notification.ErrorCode.success)
                {
                    return ActionFalseNotData(result.Data);
                }
                return DeleteSuccess();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Exeption();
            }
        }

        [HttpGet]
        public IHttpActionResult GetDetailForId(Guid? id)
        {
            try
            {
                var data = _TinhNangNghanhNgheService.GetDetailForNganhNgheId(id).OrderBy(o => o.STT);
                var view = new DataGridView()
                {
                    Page = 1,
                    PageCount = (int)Math.Ceiling((double)data.Count() / Notification.PageClientDefault),
                    Data = data.Take(Notification.PageClientDefault).Select(o => new
                    {
                        AnhTinhNangNghanhNghes = o.AnhTinhNangNghanhNghes.Select(c => new
                        {
                            Id = c.Id,
                            Note = c.Note,
                            SrcImage = c.SrcImage
                        }),
                        DateEdit = o.DateEdit,
                        Id = o.Id,
                        Id_NganhNghe = o.Id_NganhNghe,
                        NoiDung = o.NoiDung,
                        Status = o.Status,
                        STT = o.STT,
                        TenTinhNang = o.TenTinhNang,
                        TieuDe = o.TieuDe,
                        UserEdit = o.UserEdit

                    }).ToList(),
                };
                if (view.PageCount == 0)
                {
                    view.PageCount = 1;
                }
                view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, view);
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult InsertDetail(BussinessDetailModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return ActionFalseNotData("Vui lòng nhập tên tính năng ngành nghề kinh doanh!");
                }
                var data = new TinhNangNghanhNghe
                {
                    DateEdit = DateTime.Now,
                    UserEdit = contant.SESSIONNGUOIDUNG.UserID,
                    Status = model.Status,
                    TenTinhNang = model.Name,
                    Id_NganhNghe = model.NganhNgheId ?? Guid.NewGuid(),
                    NoiDung = model.Note,
                    TieuDe = model.Title,
                    STT=model.STT
                };
                var result = _TinhNangNghanhNgheService.Insert(data, model.Images);
                if (result.ErrorCode != (int)Notification.ErrorCode.success)
                {
                    return ActionFalseNotData(result.Data);
                }
                return InsertSuccess();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Exeption();
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateDetail(BussinessDetailModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return ActionFalseNotData("Vui lòng nhập tên tính năng ngành nghề kinh doanh!");
                }
                var data = new TinhNangNghanhNghe
                {
                    Id = model.ID,
                    DateEdit = DateTime.Now,
                    UserEdit = contant.SESSIONNGUOIDUNG.UserID,
                    Status = model.Status,
                    TenTinhNang = model.Name,
                    NoiDung = model.Note,
                    TieuDe = model.Title,
                    AnhTinhNangNghanhNghes=model.Images,
                    STT=model.STT
                };
                var result = _TinhNangNghanhNgheService.Update(data);
                if (result.ErrorCode != (int)Notification.ErrorCode.success)
                {
                    return ActionFalseNotData(result.Data);
                }
                return InsertSuccess();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Exeption();
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteDetail(TinhNangNghanhNghe model)
        {
            try
            {
                var result = _TinhNangNghanhNgheService.Delete(model.Id);
                if (result.ErrorCode != (int)Notification.ErrorCode.success)
                {
                    return ActionFalseNotData(result.Data);
                }
                return DeleteSuccess();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Exeption();
            }
        }

        [HttpGet]
        public IHttpActionResult GetAll()
        {
            return Json(_NganhNgheKinhDoanhService.Query.Select(o => new { ID = o.ID, TenNganhNghe = o.TenNganhNghe }));
        }


        [HttpGet]
        public IHttpActionResult SearchGridDetail(Guid? id, int page = 0, int numberpage = 0)
        {
            try
            {
                var data = _TinhNangNghanhNgheService.GetDetailForNganhNgheId(id).OrderBy(o => o.STT);
                var model = new DataGridView();
                model.PageCount = (int)Math.Ceiling((double)data.Count() / numberpage);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                    model.Data = data.Select(o => new
                    {
                        AnhTinhNangNghanhNghes = o.AnhTinhNangNghanhNghes.Select(c => new
                        {
                            Id = c.Id,
                            Note = c.Note,
                            SrcImage = c.SrcImage
                        }),
                        DateEdit = o.DateEdit,
                        Id = o.Id,
                        Id_NganhNghe = o.Id_NganhNghe,
                        NoiDung = o.NoiDung,
                        Status = o.Status,
                        STT = o.STT,
                        TenTinhNang = o.TenTinhNang,
                        TieuDe = o.TieuDe,
                        UserEdit = o.UserEdit

                    }).ToList();
                }
                else
                {
                    model.Limit = numberpage;
                    model.Page = page;
                    model.Data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit).Select(o => new
                    {
                        AnhTinhNangNghanhNghes = o.AnhTinhNangNghanhNghes.Select(c => new
                        {
                            Id = c.Id,
                            Note = c.Note,
                            SrcImage = c.SrcImage
                        }),
                        DateEdit = o.DateEdit,
                        Id = o.Id,
                        Id_NganhNghe = o.Id_NganhNghe,
                        NoiDung = o.NoiDung,
                        Status = o.Status,
                        STT = o.STT,
                        TenTinhNang = o.TenTinhNang,
                        TieuDe = o.TieuDe,
                        UserEdit = o.UserEdit

                    }).ToList();
                }
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
        }

        public IHttpActionResult GetDetailForNganhNghe(string code)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var data = _NganhNgheKinhDoanhService.Query.FirstOrDefault(o => o.MaNganhNghe.ToUpper().Equals(code.ToUpper()));
                    if (data != null)
                    {
                        var business = new
                        {
                            Id = data.ID,
                            TenNganhNghe = code==MaNganhNgheKinhDoanh.Other?"Open24": data.TenNganhNghe,
                            ImageNganhNghe = data.Image,
                            ImageMobile=data.ImageMobile
                        };
                        var listdata = _TinhNangNghanhNgheService.Query.Where(o => o.Id_NganhNghe == data.ID && o.Status==true).OrderBy(o=>o.STT).Select(o => new
                        {
                            ID = o.Id,
                            srcImage = o.AnhTinhNangNghanhNghes.Select(d => d.SrcImage).FirstOrDefault(),
                            Name = o.TenTinhNang,
                            Title = o.TieuDe,
                            Note = o.NoiDung,
                            AnhTinhNangNghanhNghes = o.AnhTinhNangNghanhNghes.Select(c => new
                            {
                                Id = c.Id,
                                SrcImage = c.SrcImage,
                                Note = c.Note,
                            }),

                        });


                        return RetunJsonAction(true, string.Empty, new { Business = business ,BusinessDetail=listdata });
                    }
                }
                return ActionFalseNotData("Mã ngành nghề không tồn tại");
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message);
            }
        }
    }
}
