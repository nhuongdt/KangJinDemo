using banhang24.Hellper;
using banhang24.Models;
using libQuy_HoaDon;
using Model;
using Model_banhang24vn.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class BaseApiController : ApiController
    {
        public IHttpActionResult ActionTrueData<T>(T data)
        {
            return Json(new { res = true, dataSoure = data });
        }
        public IHttpActionResult ActionTrueNotData(string resultMess)
        {
            return Json(new { res = true, mess = resultMess });
        }
        public IHttpActionResult UpdateSuccess()
        {
            return Json(new { res = true, mess = "Cập nhật thành công" });
        }
        public IHttpActionResult InsertSuccess()
        {
            return Json(new { res = true, mess = "Thêm mới thành công" });
        }
        public IHttpActionResult DeleteSuccess()
        {
            return Json(new { res = true, mess = "Xóa thành công" });
        }
        public IHttpActionResult ErrorNotFoundUser()
        {
            return Json(new { res = false, mess = "Lỗi hệ thống người dùng truy cập" });
        }
        public IHttpActionResult ErrorNotFound()
        {
            return Json(new { res = false, mess = "Không lấy được thông tin" });
        }
        public IHttpActionResult ActionFalseNotData(string resultMess)
        {
            return Json(new { res = false, mess = resultMess, log = resultMess });
        }

        public IHttpActionResult ActionFalseWithData<T>(T data)
        {
            return Json(new { res = false, dataSoure = data });
        }
        public IHttpActionResult Exeption(Exception ex)
        {
            return Json(new { res = false, mess = "Đã xảy ra lỗi vui lòng thử lại sau", log = ex });
        }
        [HttpGet]
        public void DownloadFileExecl(string fileSave)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument classOffice = new Class_officeDocument(db);
                classOffice.downloadFile(fileSave);
            }
        }

        public bool CheckRoleXemGiaVon(SsoftvnContext db)
        {
            bool role = false;
            var nv_nd = banhang24.Hellper.contant.GetUserCookies();
            var idUser = new Guid(nv_nd.ID.ToString());
            var user = db.HT_NguoiDung.Where(x => x.ID == idUser).Select(x => new { x.ID, x.XemGiaVon, x.LaAdmin }).ToList();
            if (user != null && user.Count() > 0)
            {
                role = user.FirstOrDefault().LaAdmin;
                if (!role)
                {
                    role = user.FirstOrDefault().XemGiaVon ?? false;
                }
            }
            return role;
        }
        public IHttpActionResult GetCheckedStatic(int? type)
        {
            List<KeyValuePair<string, string>> data = null;
            switch (type)
            {
                case (int)commonEnum.CheckBoxColum.lienhe:
                    data = commonEnum.UserContact.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.goiDichVu:
                    data = commonEnum.ServicePackage.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.listKhachHang:
                    data = commonEnum.ListKhachHang.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.listNhaCungCap:
                    data = commonEnum.ListNhaCungCap.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpValueCardBalance:
                    data = commonEnum.ReportValueCard_Balance.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpValueCardHisUsed:
                    data = commonEnum.ReportValueCard_HisUsed.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpDiscountProduct:
                    data = commonEnum.TypeRChietKhauTH.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpDiscountProduct_Detail:
                    data = commonEnum.TypeRChietKhauCT.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpDiscountInvoice:
                    data = commonEnum.RpDiscountInvoice.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpDiscountInvoice_Detail:
                    data = commonEnum.RpDiscountInvoice_Detail.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpDioscountRevenue:
                    data = commonEnum.RpDiscountRevenue.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpDioscountAll:
                    data = commonEnum.RpDiscountAll.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.RpValueCardServiveUsed:
                    data = commonEnum.ReportValueCard_ServiceUsed.ToList();
                    break;
                case (int)commonEnum.CheckBoxColum.lstsoquy:
                    data = commonEnum.ListCheckCashFlow.ToList();
                    break;
            }
            return Json(data);
        }
        [HttpGet]
        public IHttpActionResult GetAllPhongBan()
        {
            return Json(SystemDBContext.GetDBContext().NS_PhongBan.Select(o => new { Key = o.ID, Value = o.TenPhongBan }).OrderBy(o => o.Value).AsEnumerable());
        }
        [HttpGet]
        public IHttpActionResult GetAllTinhThanh()
        {
            return Json(SystemDBContext.GetDBContext().DM_TinhThanh.Select(o => new { Key = o.ID, Value = o.TenTinhThanh }).OrderBy(o => o.Value).AsEnumerable());

        }
        [HttpGet]
        public IHttpActionResult GetTinhThanhforKey(Guid quocgiaId)
        {
            return Json(SystemDBContext.GetDBContext().DM_TinhThanh.Where(o => o.ID_QuocGia == quocgiaId).Select(o => new { Key = o.ID, Value = o.TenTinhThanh }).OrderBy(o => o.Value).AsEnumerable());
        }
        [HttpGet]
        public IHttpActionResult GetQuanHuyen(Guid tinhthanhID)
        {
            return Json(SystemDBContext.GetDBContext().DM_QuanHuyen.Where(o => o.ID_TinhThanh.Equals(tinhthanhID)).Select(o => new { Key = o.ID, Value = o.TenQuanHuyen }).OrderBy(o => o.Value).AsEnumerable());
        }
        [HttpGet]
        public IHttpActionResult GetXaPhuong(Guid quanhuyenID)
        {
            return Json(SystemDBContext.GetDBContext().DM_XaPhuong.Where(o => o.ID_QuanHuyen.Equals(quanhuyenID)).Select(o => new { Key = o.ID, Value = o.TenXaPhuong }).OrderBy(o => o.Value).AsEnumerable());
        }

        [HttpGet]
        public IHttpActionResult GetQuocGia()
        {
            return Json(SystemDBContext.GetDBContext().DM_QuocGia.Select(o => new { Key = o.ID, Value = o.TenQuocGia }).OrderBy(o => o.Value).AsEnumerable());
        }

        [HttpGet]
        public IHttpActionResult GetListColumnInvoices(int loaiHD)
        {
            List<KeyValuePair<string, string>> data = null;
            switch (loaiHD)
            {
                case 1:
                    data = commonEnum.ListColumnInvoices.ToList();
                    break;  
                case 2:// hoadon baohanh
                    data = commonEnum.ListColumnInvoicesBaoHanh.ToList();
                    break;  
                case 25:
                    data = commonEnum.ListColumnInvoicesSuaChua.ToList();
                    break;
                case 3:
                    data = commonEnum.ListColumnOrders.ToList();
                    break;
                case 4:
                    data = commonEnum.ListColumnPurchaseOrder.ToList();
                    break;  
                case 13:
                    data = commonEnum.ListColumnNhapNoiBo.ToList();
                    break;
                case 6:
                    data = commonEnum.ListColumnReturns.ToList();
                    break;
                case 7:// trahangnhap
                    data = commonEnum.ListColumnPurchaseReturns.ToList();
                    break;
                case 8:// xuatkho
                    data = commonEnum.ListColumnDamageItems.ToList();
                    break;
                case 10:// chuyenhang
                    data = commonEnum.ListColumnTransfers.ToList();
                    break;
                case 22:// thegiatri
                    data = commonEnum.ListColumnValueCard.ToList();
                    break;
            }
            return Json(data);
        }

        public HT_NhatKySuDung GetHistory()
        {
            var UserLogin = contant.GetUserCookies();
            if (UserLogin.ID_DonVi == null)
            {
                return null;
            }
            var model = new HT_NhatKySuDung();
            model.ID = Guid.NewGuid();
            model.ID_DonVi = UserLogin.ID_DonVi ?? new Guid();
            model.ThoiGian = DateTime.Now;
            model.ID_NhanVien = UserLogin.ID_NhanVien;
            return model;
        }

        /// <summary>
        /// Lấy danh sách hiển thị phân trang
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagenow"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public int[] GetListPage(int count, int pagesize, int pagenow, ref int page)
        {
            page = (int)Math.Ceiling((double)count / pagesize);
            int[] listpage = new int[] { 1 };
            if (page > 5)
            {
                if (pagenow > 2 && pagenow < (page - 2))
                {
                    listpage = Enumerable.Range(pagenow - 2, 5).ToArray();
                }
                else if (pagenow >= (page - 2))
                {
                    listpage = Enumerable.Range(page - 4, 5).ToArray();
                }
                else
                {
                    listpage = new int[] { 1, 2, 3, 4, 5 };
                }
            }
            else
            {
                if (page != 0)
                {
                    listpage = Enumerable.Range(1, page).ToArray();
                }
            }
            return listpage;
        }

        [HttpGet]
        public IHttpActionResult CheckCreateDatabase()
        {
            string subdomain = string.Empty;
            var host = Request.RequestUri.Host;
            if (host.Contains("localhost"))
            {
                subdomain = "0973474985";
            }
            else
            {
                string[] blacklist = { "www", "mail", "vn" };
                string[] domainlist = { "open24" };
                var result = host.Split('.').Where(o => !blacklist.Contains(o.ToLower())).ToArray();
                if (result.Skip(1).Any(o => domainlist.Contains(o.ToLower())))
                {
                    subdomain = result[0];
                }
                else
                {
                    subdomain = string.Join(".", result);
                }
            }
            var model = new CuaHangDangKyService().Query.FirstOrDefault(o => o.SubDomain.Equals(subdomain));
            if (model != null)
            {
                return Json(model.IsCreateDatabase ?? true);
            }
            return Json(true);
        }
    }
}
