using Model.Web;
using Model.Web.Service;
using SoftWareSsoft.Models.ThemeSsoft;
using Ssoft.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SoftWareSsoft.Areas.Admin.Api
{
    public class NewsController : ApiBaseController
    {
        private NewsService _NewsService = new NewsService();
        public NewsController()
        {

        }

        [HttpGet]
        public IHttpActionResult GetNewsPage(int page=0)
        {
            try
            {
                var data = _NewsService.GetClient().Skip(page* GridPagedingHellper.PageDefault).Take(GridPagedingHellper.PageDefault).Select(o => new
                {
                    o.Anh,
                    o.NoiDung,
                    o.TenBaiViet,
                    o.Mota,
                    o.NguoiTao,
                    o.NgayTao,
                    o.Link
                }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch(Exception e)
            {
                return Exception(e);
            }
        }
        [HttpGet]
        public IHttpActionResult GetNewsDate()
        {
            try
            {
                var data = _NewsService.GetNewsDate().Take(5).Select(o => new
                {
                    o.Anh,
                    o.TenBaiViet,
                    o.Mota,
                    o.NguoiTao,
                    o.NgayTao,
                    o.LuotXem,
                    o.Link
                }).ToList();
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpGet]
        public IHttpActionResult GetNewsOrderbyView()
        {
            try
            {
                var data = _NewsService.GetNewsView().Take(5).Select(o => new
                {
                    o.Anh,
                    o.TenBaiViet,
                    o.Mota,
                    o.NguoiTao,
                    o.NgayTao,
                    o.LuotXem,
                    o.Link
                }).ToList();
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        public IHttpActionResult GetNewsHome()
        {
            try
            {
                var data = _NewsService.GetClient().Skip(0).Take(6).Select(o => new
                {
                    o.Anh,
                    o.NoiDung,
                    o.TenBaiViet,
                    o.Mota,
                    o.NguoiTao,
                    o.NgayTao,
                    o.Link,
                    o.MetaTitle
                }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }


        ///---------------------Admin ----------------------///

        [HttpPost]
        public IHttpActionResult SearchGrid(SearchModel model)
        {
            try
            {
                var data = _NewsService.SearchNewsGrid(model.text,model.groupId);
                if (model.limit == 0)
                {
                    model.limit = GridPagedingHellper.PageDefault;
                }
                model.pageCount = (int)Math.Ceiling((double)data.Count() / model.limit);
                if (model.pageCount == 0 || model.pageCount == 1)
                {
                    model.pageCount = 1;
                    model.page = 1;
                }
                model.pageItem = GridPagedingHellper.PageItems(model.page, model.pageCount, data.Count());
                model.data = data.Skip(model.limit * (model.page - 1)).Take(model.limit).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.TenBaiViet,
                    o.Link,
                    o.Anh,
                    TheLoai=o.DM_NhomBaiViet!=null? o.DM_NhomBaiViet.TenNhomBaiViet:string.Empty,
                    o.NgayTao,
                    o.LuotXem

                });
              
                return ActionTrueData(model);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpGet]
        public IHttpActionResult GetNewsGroup()
        {
            try
            {
                var data = _NewsService.GetGroup((int)LibEnum.StatusGroupNews.tintuc).Select(o=>new { o.TenNhomBaiViet,o.GhiChu,o.ID,o.LoaiNhomBaiViet,o.ID_NhomCha});
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpGet]
        public IHttpActionResult GetRecruitmentGroup()
        {
            try
            {
                var data = _NewsService.GetGroup((int)LibEnum.StatusGroupNews.tuyendung).Select(o => new { o.TenNhomBaiViet, o.GhiChu, o.ID, o.LoaiNhomBaiViet, o.ID_NhomCha });
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpPost]
        public IHttpActionResult EditNews(NewsObjectModel model)
        {
            try
            {
                if (model != null)
                {
                    var data = new DM_BaiViet();
                    data.TenBaiViet = model.TenBaiViet.Normalize(NormalizationForm.FormC);
                    data.Anh = model.Anh;
                    data.NoiDung = model.NoiDung.Normalize(NormalizationForm.FormC);
                    data.Mota = model.Mota.Normalize(NormalizationForm.FormC);
                    data.TrangThai = model.TrangThai;
                    data.MetaTitle = model.MetaTitle;
                    data.MetaDescriptions = model.MetaDescriptions;
                    data.ID_NhomBaiViet = model.ID_NhomBaiViet;
                    data.NgayTao = DateTime.Now;
                    data.NguoiTao = "Admin";
                    data.LuotXem = 0;
                    data.ID = model.ID;
                    if (string.IsNullOrWhiteSpace(data.MetaTitle))
                    {
                        data.MetaTitle = data.TenBaiViet;

                    }
                    if (string.IsNullOrWhiteSpace(data.MetaDescriptions))
                    {
                        data.MetaDescriptions = data.Mota;
                    }
                    if (model.IsLichHen)
                    {
                        data.NgayDangBai = model.NgayDangBai;
                    }
                    if (model.IsNews)
                    {
                        _NewsService.Insert(data, model.Tags);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _NewsService.Update(data, model.Tags);
                        if (result.ErrorCode == (int)LibEnum.ErrorCode.Success)
                        {

                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
                return ActionFalseNotData("Không lấy được thông tin, vui lòng kiểm tra lại");
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpGet]
        public IHttpActionResult GetEditNews(int id)
        {
            try
            {
                var data = _NewsService.getAll().Where(o => o.ID == id).Select(o => new NewsObjectModel {
                    ID = o.ID,
                    Anh = o.Anh,
                    ID_NhomBaiViet = o.ID_NhomBaiViet,
                    IsLichHen = o.NgayDangBai != null ? true : false,
                    Link = o.Link,
                    IsNews = false,
                    MetaDescriptions = o.MetaDescriptions,
                    MetaTitle = o.MetaTitle,
                    NgayDangBai = o.NgayDangBai,
                    Mota = o.Mota,
                    NoiDung = o.NoiDung,
                    TenBaiViet = o.TenBaiViet,
                    TrangThai = o.TrangThai,
                    TenNhom = o.DM_NhomBaiViet != null ? o.DM_NhomBaiViet.TenNhomBaiViet : string.Empty
                }).FirstOrDefault();
                if (data != null)
                {
                    data.Tags =string.Join(",", _NewsService.GetTagsNews(data.ID).Where(o=> o.DM_Tags!=null).AsEnumerable().Select(o=>o.DM_Tags.TenTheTag).ToList());
                    return ActionTrueData(data);
                }
                return ActionFalseNotData("Bài viết không tồn tại hoặc đã bị xóa");
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpGet]
        public IHttpActionResult RemoveNews(int id)
        {
            try
            {
                var result = _NewsService.Delete(id);
                if(result)
                {
                    return DeleteSuccess();
                }
                return ActionFalseNotData("Bài viết không tồn tại hoặc đã bị xóa");
            }
            catch (Exception e)
            {
                return Exception(e);
            }

        }

        [HttpPost]
        public IHttpActionResult EditGroupNews(GroupNewsObjectModel model)
        {
            try
            {
                if (model != null)
                {
                    var data = new DM_NhomBaiViet()
                    {
                        ID=model.ID,
                        TenNhomBaiViet = model.Ten.Trim(),
                        GhiChu = model.GhiChu,
                        LoaiNhomBaiViet = model.Loai,
                        NgayTao = DateTime.Now,
                        NguoiTao = "Admin",
                        TrangThai = true,
                        ID_NhomCha = model.NhomID
                    };
                    if (model.IsNews)
                    {
                        if (_NewsService.InsertGroupNews(data))
                        {
                            return InsertSuccess();
                        }
                       return  ActionFalseNotData("Tên thể loại đã bị trùng");
                    }
                    else
                    {
                        var result = _NewsService.UpdateGroupNews(data);
                        if (result.ErrorCode==(int)LibEnum.ErrorCode.Success)
                        {
                            return UpdateSuccess();
                        }
                        return ActionFalseNotData(result.Data);
                    }
                }
                return ActionFalseNotData("Không lấy được thông tin, vui lòng kiểm tra lại");
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }


        [HttpPost]
        public IHttpActionResult RemoveGroupNews(DM_NhomBaiViet model)
        {
            try
            {
                if (model != null)
                {
                    var resul = _NewsService.DeleteGroupNews(model.ID);
                    if (resul.ErrorCode== (int)LibEnum.ErrorCode.Success)
                    {
                        return DeleteSuccess();
                    }
                    return ActionFalseNotData(resul.Data);

                }
                return ActionFalseNotData("Không lấy được thông tin, vui lòng kiểm tra lại");
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
    }
}
