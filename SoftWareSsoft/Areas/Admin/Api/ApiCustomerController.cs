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

namespace SoftWareSsoft.Areas.Admin.Api
{
    public class ApiCustomerController : ApiBaseController
    {
        private CustomerService _CustomerService;
        private ProductService _ProductService;
        public ApiCustomerController()
        {
            _CustomerService = new CustomerService();
            _ProductService = new ProductService();
        }

        [HttpGet]
        public IHttpActionResult GetCustomerPage(string text,string adress ,string product, int page = 0)
        {
            try
            {
                var data = _CustomerService.SearchClient(text,adress,product)
                    .Skip(page * GridPagedingHellper.PageDefault).Take(GridPagedingHellper.PageDefault).Select(o => new
                {
                    o.Anh,
                    o.NoiDung,
                    o.TenKhachHang,
                    o.Mota,
                    o.NguoiTao,
                    o.NgayTao,
                    o.DiaChi,
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
        [HttpGet]
        public IHttpActionResult GetCustomerOrderbyDate()
        {
            try
            {
                var data = _CustomerService.getAll().Where(o => o.TrangThai != (int)LibEnum.IsStatus.an)
                    .OrderByDescending(o => o.NgayTao).Skip(0).Take(5).Select(o => new
                    {
                        o.Anh,
                        o.NoiDung,
                        o.TenKhachHang,
                        o.Mota,
                        o.NguoiTao,
                        o.NgayTao,
                        o.Link
                    }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpPost]
        public IHttpActionResult SearchGrid(SearchModel model)
        {
            try
            {
                var data = _CustomerService.SearchCustomerGrid(model.text);
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
                    o.TenKhachHang,
                    o.Link,
                    o.Anh,
                    o.NgayTao,
                    DiaChi=o.DiaChi!=null?o.DiaChi:"",
                    TinhThanh="",
                    o.SoDienThoai,
                    o.NoiDung,
                    o.Mota,
                    o.Email,
                    o.TrangThai
                    

                });
            
                return ActionTrueData(model);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        public IHttpActionResult GetProductAndTinhThanh()
        {
            try
            {
                var tinhthanh = _TinhThanhService.GetAll().Select(o => new
                {
                    Key=o.MaTinhThanh,
                    Value=o.TenTinhThanh,
                    IsSelect=false
                }).AsEnumerable();
                var product = _ProductService.GetAll().Select(o => new
                {
                    o.ID,
                    Key = o.MaSanPham,
                    Value = o.TenSanPham,
                    IsSelect = false
                }).AsEnumerable();
                return ActionTrueData(new {tt=tinhthanh,sp=product });
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }

        [HttpPost]
        public IHttpActionResult EditCustomer(CustomerObjectModel model)
        {
            try
            {
                if (model != null)
                {
                    var data = new DM_KhachHang();
                    data.TenKhachHang = model.TenKhach.Normalize(NormalizationForm.FormC);
                    data.Anh = model.Anh;
                    data.NoiDung = model.NoiDung.Normalize(NormalizationForm.FormC);
                    data.Mota = model.Mota.Normalize(NormalizationForm.FormC);
                    data.TrangThai = model.TrangThai;
                    data.MetaTitle = model.MetaTitle;
                    data.MetaDescription = model.MetaDescriptions;
                    data.Email = model.Email;
                    data.SoDienThoai = model.SoDienThoai;
                    data.DiaChi = model.DiaChi;
                    data.MaTinhThanh = model.MaTinhThanh;
                    data.ID_SanPham = model.MaSanPham;
                    data.NgayTao = DateTime.Now;
                    data.NguoiTao = "Admin";
                    data.SoLuotXem = 0;
                    data.ID = model.ID;
                    if (string.IsNullOrWhiteSpace(data.MetaTitle))
                    {
                        data.MetaTitle = data.TenKhachHang;
                    }
                    if (string.IsNullOrWhiteSpace(data.MetaDescription))
                    {
                        data.MetaDescription = data.Mota;
                    }
                    if (model.IsNews)
                    {
                        _CustomerService.Insert(data, model.Tags);
                        return InsertSuccess();
                    }
                    else
                    {
                        var result = _CustomerService.Update(data, model.Tags);
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
        public IHttpActionResult RemoveCustomer(int id)
        {
            try
            {
                var result = _CustomerService.Delete(id);
                if (result)
                {
                    return DeleteSuccess();
                }
                return ActionFalseNotData("Khách hàng không tồn tại hoặc đã bị xóa");
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
                var data = _CustomerService.getAll().Where(o => o.ID == id).Select(o => new CustomerObjectModel
                {
                    ID = o.ID,
                    Anh = o.Anh,
                    Link = o.Link,
                    IsNews = false,
                    MetaDescriptions = o.MetaDescription,
                    MetaTitle = o.MetaTitle,
                    Mota = o.Mota,
                    NoiDung = o.NoiDung,
                    TenKhach = o.TenKhachHang,
                    TrangThai = o.TrangThai,
                    MaSanPham=o.ID_SanPham,
                    MaTinhThanh=o.MaTinhThanh,
                    DiaChi=o.DiaChi,
                    Email=o.Email,
                    SoDienThoai=o.SoDienThoai,
                }).FirstOrDefault();
                if (data != null)
                {
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
        public IHttpActionResult GetCustomerForproduct(int keyId = (int)LibEnum.TypeSoftWareSsoft.beauty, int page = 0)
        {
            try
            {
                var data = _CustomerService.getAll().Where(o => o.TrangThai != (int)LibEnum.IsStatus.an);
                if (keyId == (int)LibEnum.TypeSoftWareSsoft.gara)
                {
                    data = data.Where(o => o.ID_SanPham.Contains(LibNotification.KeyLuckyGara));
                }
                else if (keyId == (int)LibEnum.TypeSoftWareSsoft.hrm)
                {
                    data = data.Where(o => o.ID_SanPham.Contains(LibNotification.KeyLuckyHrm));
                }
                else
                {
                    data = data.Where(o => o.ID_SanPham.Contains(LibNotification.KeyLuckyBeauty));
                }
                var model = data.OrderByDescending(o => o.NgayTao).Skip((page )* LibNotification.CustomerClientDefault).Take(LibNotification.CustomerClientDefault).Select(o => new { o.TenKhachHang, o.Link, o.Anh, o.DiaChi }).AsEnumerable();
                return ActionTrueData(model);
            }
            catch (Exception e)
            {
                return Exception(e);
            }
        }
    }
}
