using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model.Web.Service;
using Ssoft.Common.Common;
using SoftWareSsoft.Models.ThemeSsoft;
using Model.Web;
using System.Text;
using System.Web;
using Model.Web.API;
using System.IO;

namespace SoftWareSsoft.Areas.Admin.Api
{
    public class ApiRecruitmentController : ApiBaseController
    {
        private RecruitmentService _RecruitmentService;
        public ApiRecruitmentController()
        {
            _RecruitmentService = new RecruitmentService();
        }


        [HttpGet]
        public IHttpActionResult GetRecruitmentPage(int page = 0)
        {
            try
            {
                var data = _RecruitmentService.GetAllActive()
                    .Skip(page * GridPagedingHellper.PageDefault).Take(GridPagedingHellper.PageDefault).Select(o => new
                    {
                        o.TieuDe,
                        o.MoTa,
                        o.TuNgay,
                        o.DenNgay,
                        o.DiaChi,
                        o.MucLuong,
                    }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpGet]
        public IHttpActionResult GetRecruitmentHome()
        {
            try
            {
                var dataHN = _RecruitmentService.GetHome("01")
                    .Take(GridPagedingHellper.PageDefault).Select(o => new
                    {
                        o.TieuDe,
                        o.Link
                    }).AsEnumerable();
                var dataHCM = _RecruitmentService.GetHome("79")
               .Take(GridPagedingHellper.PageDefault).Select(o => new
               {
                   o.TieuDe,
                   o.Link
               }).AsEnumerable();
                return ActionTrueData(new { dataHN, dataHCM });
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
                var data = _RecruitmentService.GetGroup()
                    .Select(o => new
                    {
                        o.ID,
                        o.TenNhomBaiViet,
                        o.GhiChu,
                        o.Link
                    }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpGet]
        public IHttpActionResult GetRecruitmentDetailGroup(int? groupReq)
        {
            try
            {
                var data = _RecruitmentService.GetDetailGroup(groupReq).AsEnumerable()
                    .Select(o => new
                    {
                        o.ID,
                        ThoiGian = o.TuNgay.ToString("dd/MM/yyyy") + " - " + o.DenNgay.ToString("dd/MM/yyyy"),
                        o.SoLuong,
                        o.MoTa,
                        MucLuong = _RecruitmentService.ConvertMucLuong(o.MucLuong),
                        o.TieuDe,
                        ConHan = _RecruitmentService.CheckConHan(o.DenNgay),
                        TinhThanh = _RecruitmentService.GetTinhThanh(o.MaTinhThanh),
                        o.Link
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
                var data = _RecruitmentService.SearchNewsGrid(model.text, model.groupId);
                if (model.limit == 0)
                {
                    model.limit = GridPagedingHellper.PageDefault;
                }
                model.pageCount = (int)Math.Ceiling((double)data.Count() / model.limit);
                if (model.pageCount <= 1)
                {
                    model.pageCount = 1;
                    model.page = 1;
                }
                model.pageItem = GridPagedingHellper.PageItems(model.page, model.pageCount, data.Count());

                model.data = data.Skip(model.limit * (model.page - 1)).Take(model.limit).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.TieuDe,
                    o.MoTa,
                    TheLoai = o.DM_NhomBaiViet != null ? o.DM_NhomBaiViet.TenNhomBaiViet : string.Empty,
                    o.NgayTao,
                    MucLuong = _RecruitmentService.ConvertMucLuong(o.MucLuong),
                    o.TrangThai,
                    o.SoLuong,
                    o.TuNgay,
                    o.DenNgay,
                });
                return ActionTrueData(model);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpPost]
        public IHttpActionResult EditApiRecruitment(RecruitmentObjectModel model)
        {
            try
            {
                if (model != null)
                {
                    var data = new DM_TuyenDung();
                    data.TieuDe = model.TieuDe.Normalize(NormalizationForm.FormC);
                    data.DiaChi = model.DiaChi;
                    data.MoTa = model.Mota.Normalize(NormalizationForm.FormC);
                    data.TrangThai = model.TrangThai;
                    data.MetaTitle = model.MetaTitle;
                    data.MetaDescription = model.MetaDescriptions;
                    data.ID_NhomBaiViet = model.ID_NhomBaiViet;
                    data.NgayTao = DateTime.Now;
                    data.NguoiTao = "Admin";
                    data.SoLuong = model.Soluong;
                    data.TuNgay = model.Tungay;
                    data.DenNgay = model.Denngay;
                    data.MaTinhThanh = model.MaTinhThanh;
                    data.MucLuong = model.MucLuong;
                    data.ID = model.ID;
                    if (string.IsNullOrWhiteSpace(data.MetaTitle))
                    {
                        data.MetaTitle = data.TieuDe;
                    }
                    if (string.IsNullOrWhiteSpace(data.MetaDescription))
                    {
                        data.MetaDescription = data.TieuDe;
                    }
                    if (model.IsNews)
                    {
                        _RecruitmentService.Insert(data, model.Tags);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _RecruitmentService.Update(data, model.Tags);
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

        public IHttpActionResult GetEditRecruitment(int id)
        {
            try
            {
                var data = _RecruitmentService.GetAll().Where(o => o.ID == id).Select(o => new RecruitmentObjectModel
                {
                    ID = o.ID,
                    ID_NhomBaiViet = o.ID_NhomBaiViet,
                    IsLichHen = o.NgayDangBai != null ? true : false,
                    Link = o.Link,
                    IsNews = false,
                    MetaDescriptions = o.MetaDescription,
                    MetaTitle = o.MetaTitle,
                    NgayDangBai = o.NgayDangBai,
                    Mota = o.MoTa,
                    TieuDe = o.TieuDe,
                    TrangThai = o.TrangThai,
                    TenNhom = o.DM_NhomBaiViet != null ? o.DM_NhomBaiViet.TenNhomBaiViet : string.Empty,
                    Denngay = o.DenNgay,
                    DiaChi = o.DiaChi,
                    MaTinhThanh = o.MaTinhThanh,
                    MucLuong = o.MucLuong,
                    Soluong = o.SoLuong,
                    Tungay = o.TuNgay
                }).FirstOrDefault();
                if (data != null)
                {

                    return ActionTrueData(data);
                }
                return ActionFalseNotData("Bài tuyển dụng không tồn tại hoặc đã bị xóa");
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpGet]
        public IHttpActionResult RemoveRecruitment(int id)
        {
            try
            {
                var result = _RecruitmentService.Delete(id);
                if (result)
                {
                    return DeleteSuccess();
                }
                return ActionFalseNotData("Bài tuyển dụng không tồn tại hoặc đã bị xóa");
            }
            catch (Exception e)
            {
                return Exception(e);
            }

        }

        [HttpPost]
        public IHttpActionResult InsertHoSo(DS_HoSoUngTuyen model)
        {
            try
            {
                if (model != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.KyNang))
                    {
                        model.KyNang = model.KyNang.Replace("\n", "<br />\n");
                    }
                    var result = _RecruitmentService.InsertHoSoUngTuyen(model);
                    return ActionTrueData(result);
                }
                return ActionFalseNotData("Không lấy được thông tin ứng tuyển");
            }
            catch (Exception e)
            {
                return Exception();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadFile(int KeyId)
        {
            try
            {
                var path = "";
                string result = "";
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];
                    var filenameImage = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var subdomain = CookieStore.GetCookieAes(SqlConnection.subdoamin);
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/File/TuyenDung/" + subdomain)))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/File/TuyenDung/" + subdomain));
                    }

                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/File/TuyenDung/" + subdomain), filenameImage);

                    file.SaveAs(path);
                    result = "/File/TuyenDung/" + subdomain + "/" + filenameImage;
                    _RecruitmentService.InsertFileDinhKem(result, file.FileName, KeyId, file.ContentLength);
                }
                return ActionTrueData(result);
            }
            catch (Exception ex)
            {
                return Exception(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult SearchGridHoSoUngTuyen(SearchModel model)
        {
            try
            {
                var data = _RecruitmentService.SearchGridHoSoUngTuyen(model.text, model.TrangThais);
                var count = data.Count();
                if (model.limit == 0)
                {
                    model.limit = GridPagedingHellper.PageDefault;
                }
                model.pageCount = (int)Math.Ceiling((double)count / model.limit);
                if (model.pageCount == 0 || model.pageCount == 1)
                {
                    model.pageCount = 1;
                    model.page = 1;
                }
                model.data = data.OrderByDescending(o => o.NgayTao).Skip(model.limit * (model.page - 1)).Take(model.limit).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.HoTen,
                    o.Email,
                    o.TruongTotNghiep,
                    o.SoDienThoai,
                    o.DiaChi,
                    ViTri= o.DM_TuyenDung!=null?o.DM_TuyenDung.TieuDe:string.Empty,
                    NgayTao = o.NgayTao.ToString("yyyy/MM/dd hh:mm tt"),
                    TrangThai = o.TrangThai == (int)LibEnum.IsStatusTuyenDung.taomoi ? true : false,
                    o.ChuyenNganh,
                    GioiTinh = o.GioiTinh == true ? "Nam" : (o.GioiTinh == false ? "Nữ" : string.Empty),
                    o.HeDaoTao,
                    o.KyNang,
                    NgaySinh = o.NgaySinh != null ? o.NgaySinh.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DS_FileDinhKems = o.DS_FileDinhKems.Select(c => new { c.TenFile, c.LinkFile, c.Size }).AsEnumerable(),
                    count = o.DS_FileDinhKems.Count,

                }).AsEnumerable();
                model.pageItem = string.Format("{0} - {1} of {2} ", ((model.page - 1) * model.limit) + 1, data.Count(), count);
                return ActionTrueData(model);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpGet]
        public IHttpActionResult UpdateHoSoUngTuyen(int id = 0)
        {
            try
            {
                if (id != 0)
                {
                    _RecruitmentService.UpdateTrangThaiHoSoUngTuyen(id);
                    return ActionTrueNotData("");
                }
                return ActionFalseNotData("Không thể cập nhật được hồ sơ đã đọc");

            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
        [HttpGet]
        public IHttpActionResult RemoveHoSoUngTuyen(int id = 0)
        {
            try
            {
                if (id != 0)
                {
                    _RecruitmentService.RemoveHoSoUngTuyen(id);
                    return DeleteSuccess();
                }
                return ActionFalseNotData("Không lấy được thông tin cần xóa");

            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
    }
}
