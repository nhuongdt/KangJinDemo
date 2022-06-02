using Model;
using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Newtonsoft.Json.Linq;
using Open24.Areas.AdminPage.Models;
using Open24.Hellper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ZaloConnectApp;

namespace Open24.Controllers
{

    public class HomeController : BaseController
    {
        private MenuTagsService _MenuTagsService = new MenuTagsService();
        private OrderService _OrderService = new OrderService();
        private HoTroService _HoTroService = new HoTroService();
        [ViewAuthorize]
        [OutputCache(Duration = 2*60*1000, VaryByParam = "none",  Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Index()
        {
            setkeywords();
            return View();
        }

        public ActionResult DangKyDungThu()
        {
            ViewBag.MetaDescription = "Đăng ký dùng thử phần mềm kinh doanh open24";
            ViewBag.MetaKeywords = "dung thu mien phi, dùng thử miễn phí, đăng ký phần mềm kinh doanh , dung thu phan mem kinh doanh, miễn phí đăng ký ";
            return View();
        }

        [ViewAuthorize]
        public ActionResult DangKyVersion1()
        {
            setkeywordsTitle();
            return View();
        }

        public ActionResult DangKyThanhCongVersion1()
        {
            return View();
        }

        [ViewAuthorize]
        public ActionResult HoTroPhanMem()
        {
            setkeywordsTitle();
            return View();
        }

        [ViewAuthorize]
        public ActionResult HoTroChiTiet(int? keyId, long? id, string title)
        {
            ViewBag.keyId = keyId;
            ViewBag.ID_Group = (id == null ? 0 : id);
            SetCanonical();
            return View();
        }

        [ViewAuthorize]
        public ActionResult SearchHoTroPhanMem(string search, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return RedirectToAction("HoTroChiTiet", "", new { keyId = (int)Notification.HoTroNhom.theotinhnang, id = 0 });
            }
            var data = _HoTroService.GetSearch(search).AsEnumerable().Select(o => new SearchInputHoTro
            {

                ID = o.ID,
                Mota = o.GhiChu,
                Ten = o.Ten,
                NgayTao = o.NgayTao,
                Title = StaticVariable.ConvetTitleToUrl(o.Ten)
            });
            var model = data.OrderByDescending(x => x.NgayTao).ToPagedList(page, pageSize);
            ViewBag.SearchString = search;
            ViewBag.PageCount = data.Count();
            SetCanonical();
            return View(model);
        }

        public ActionResult ThanhToan()
        {
            setkeywordsTitle();
            return View();
        }

        public ActionResult GioHang()
        {
            setkeywordsTitle();
            return View();
        }


        public ActionResult OrderSuccess(long? ID)
        {
            var model = _OrderService.Query.Where(o => o.ID == ID).ToList().Select(o =>
                new OrderDetailView
                {
                    AdressOrder = o.AdressOrder,
                    AdressReceived = !string.IsNullOrWhiteSpace(o.AdressReceived) ? o.AdressReceived : o.AdressOrder,
                    EmailOrder = o.EmailOrder,
                    Encoder = "#" + o.ID,
                    Note = o.Note,
                    payment = Notification.HinhThucVanChuyen.Where(c => c.Key == o.payment).Select(c => c.Value).FirstOrDefault(),
                    PhoneOrder = o.PhoneOrder,
                    PhoneReceived = !string.IsNullOrWhiteSpace(o.PhoneReceived) ? o.PhoneReceived : o.PhoneOrder,
                    UserOrder = o.UserOrder,
                    UserReceived = !string.IsNullOrWhiteSpace(o.UserReceived) ? o.UserReceived : o.UserOrder
                }).FirstOrDefault();
            if (model == null) return RedirectToAction("Index", "Home");


            return View(model);
        }

        [ViewAuthorize]
        public ActionResult GioiThieu()
        {
            setkeywordsTitle();
            return View();
        }

        [ViewAuthorize]
        public ActionResult DieuKhoan()
        {
            setkeywordsTitle();
            return View();
        }

        [ViewAuthorize]
        public ActionResult Contact()
        {
            setkeywordsTitle();
            return View();
        }

        public ActionResult Cart(long? keyId, string title)
        {
            if (keyId != null)
            {
                SetCanonical();
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [ViewAuthorize]
        public ActionResult Registration()
        {
            return RedirectToAction("ActionOrViewName", "ControllerName");
            //return View(GetDataDangky());
        }

        public ActionResult _CuaHangDangKy()
        {
            return PartialView(GetDataDangky());
        }

        private DangkyViewModel GetDataDangky()
        {
            var cache = new Model_banhang24vn.Cache.CacheHelper();
            if (!cache.IsSet(CacheKey.Home_Dangky))
            {
                DangkyViewModel vm = new DangkyViewModel();
                List<NganhNgheKinhDoanh> lstNganhNghes = M_NganhNgheKinhDoanh.SelectAll();
                vm.NganhNghe = lstNganhNghes;
                vm.TinhThanh = CuaHangDangKyService.GetsTinhThanhQuanHuyen(null).OrderBy(p => p.TinhThanh).ThenBy(p => p.QuanHuyen).Select(p => new TinhThanhQuanHuyenDAO
                {
                    ID = p.ID,
                    TenTinhThanhQuanHuyen = p.TinhThanh + " - " + p.QuanHuyen
                }).ToList();
                cache.Set(CacheKey.Home_Dangky, vm);
                return vm;
            }
            return cache.Get(CacheKey.Home_Dangky) as DangkyViewModel;
        }




        #region Các Trang cũ ko dùng



        public ActionResult HotroBanhang24()
        {
            return View();
        }

        public ActionResult TinhNang(string keyId)
        {
            var key = !string.IsNullOrWhiteSpace(keyId) ? keyId : MaNganhNgheKinhDoanh.Other;
            var title = Notification.KeyInTitle.Where(o => o.Key.ToUpper().Equals(key.ToUpper()));
            ViewBag.KeyCode = key;
            ViewBag.Title = title.Any() ? "Phần mềm quản lý kinh doanh " + title.First().Value + " " : "Tính năng phần mềm open24";
            setkeywords();
            return View();

        }


        [ViewAuthorize]
        public ActionResult PhiDV()
        {
            setkeywords();
            return View();
        }

        public ActionResult HopTac()
        {
            return View();
        }

        public ActionResult Supper()
        {
            return View();
        }
      
        public ActionResult Linhvuckhac()
        {
            return View();
        }
    

        public ActionResult TestOptinform()
        {
            return View();
        }
        public ActionResult TestOptinformLh()
        {

            return View();
        }
        
        [ViewAuthorize]
        public ActionResult KhachHang()
        {
            setkeywords();
            return View();
        }

        public ActionResult Success(string id)
        {
            CuaHangDangKy cuahang = new CuaHangDangKyService().SelectBySubdomain(id);
            ShopInfo shopinfo = new ShopInfo();
            if (cuahang != null)
            {
                shopinfo.DiaChiQuanLy = "https://" + cuahang.SubDomain + ".open24.vn";
                shopinfo.TenCuaHang = cuahang.TenCuaHang;
                shopinfo.TenDangNhap = cuahang.UserKT;
            }
            return View(shopinfo);
        }

        [ViewAuthorize(Title = "/khach-hang/chi-tiet")]
        public ActionResult KhachHangChiTiet(int? keyId, string title)
        {
            ViewBag.CustomerId = keyId;
            var data = new CustomerService().GetByDetail(keyId ?? -1);
            ViewBag.httpUrl = data != null ? System.Configuration.ConfigurationManager.AppSettings["Webhttp"] + data.Url : string.Empty;
            ViewBag.TenKhachHang = data.Name;
            ViewBag.MetaDescription = data.Note;
            ViewBag.MetaKeywords = data.Name + "," + StaticVariable.RemoveSign4VietnameseString(data.Name);
            return View();
        }
        #endregion

        #region Đăng ký sử dụng
        //public ActionResult DangKySuDung()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult DangKySuDung(CuaHangDangKy objDangKy)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            string Indetifier = objDangKy.SubDomain.Trim().ToLower();
        //            if (Indetifier != "")
        //            {
        //                CuaHangDangKy objCheck_TenMien = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == objDangKy.SubDomain.Trim().ToLower());
        //                if (objCheck_TenMien != null)
        //                {
        //                    ViewBag.Message = "Địa chỉ WebSite đã được sử dụng. Hãy nhập địa chỉ Web khác.";
        //                    List<NganhNgheKinhDoanh> lstNganhNghe1s = M_NganhNgheKinhDoanh.SelectAll();
        //                    ViewBag.ID_NganhKinhDoanh = new SelectList(lstNganhNghe1s, "ID", "TenNganhNghe");
        //                    return View();
        //                }
        //                //
        //                objDangKy.NgayTao = DateTime.Now;
        //                string strIns = M_DangKySuDung.AddNewCuaHangDangKy(objDangKy);
        //                if (strIns != null && strIns != string.Empty)
        //                {
        //                    ViewBag.Message = strIns;
        //                    List<NganhNgheKinhDoanh> lstNganhNghe2s = M_NganhNgheKinhDoanh.SelectAll();
        //                    ViewBag.ID_NganhKinhDoanh = new SelectList(lstNganhNghe2s, "ID", "TenNganhNghe");
        //                    return View();
        //                }
        //                //

        //                System.Web.HttpContext.Current.Session.Clear();
        //                //
        //                string strAddNewSubdomain = apirpc_Subdomain.AddNewSubdomain(Indetifier);
        //                if (strAddNewSubdomain != null && strAddNewSubdomain != string.Empty)
        //                {
        //                    M_DangKySuDung.Delete_SDT(objDangKy.SoDienThoai);
        //                    ViewBag.Message = strAddNewSubdomain;
        //                    List<NganhNgheKinhDoanh> lstNganhNghe3s = M_NganhNgheKinhDoanh.SelectAll();
        //                    ViewBag.ID_NganhKinhDoanh = new SelectList(lstNganhNghe3s, "ID", "TenNganhNghe");
        //                    return View();
        //                }
        //                try
        //                {
        //                    //tạo connect string
        //                    string databaseName = "SSOFT_" + Indetifier.ToUpper();
        //                    string strconn = "data source=data.ssoft.vn;initial catalog=" + databaseName + ";persist security info=True;user id=lucky;password=Lucky123;MultipleActiveResultSets=True;App=EntityFramework";
        //                    string strproviderName = "System.Data.SqlClient";
        //                    //
        //                    string strCnn = ConnectionStringSystem.CreateConnectionString(Indetifier, strconn, strproviderName);
        //                    if (strCnn.Trim() != "")
        //                    {
        //                        ViewBag.Message = strCnn;
        //                        List<NganhNgheKinhDoanh> lstNganhNghe4s = M_NganhNgheKinhDoanh.SelectAll();
        //                        ViewBag.ID_NganhKinhDoanh = new SelectList(lstNganhNghe4s, "ID", "TenNganhNghe");
        //                        return View();
        //                    }
        //                    else
        //                    {
        //                        CookieStore.SetCookie("SubDomain", Indetifier, new TimeSpan(1, 0, 0, 0, 0), Indetifier);
        //                    }
        //                    //
        //                    return Redirect("http://" + Indetifier + ".open24.vn");
        //                    //return Redirect("http://" + Indetifier + ".localhost:49807");
        //                }
        //                catch (Exception ex)
        //                {
        //                    ViewBag.Message = "Lỗi tạo chuỗi kết nối dữ liệu: " + ex.Message;
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.Message = "Địa chỉ Website không hợp lệ";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = "Lỗi cập nhật:" + ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Dữ liệu cập nhật chưa hợp lệ";
        //    }
        //    List<NganhNgheKinhDoanh> lstNganhNghes = M_NganhNgheKinhDoanh.SelectAll();
        //    ViewBag.ID_NganhKinhDoanh = new SelectList(lstNganhNghes, "ID", "TenNganhNghe");
        //    return View();
        //}
        #endregion

        #region ZaloConnect
        public ActionResult zalologin(string oa_id, string code, string state, string code_challenge)
        {
            
            //string domain = "https://" + state + "open24.vn";
            //string codeverifier = GetCodeVerifier(state);
            
            bool model = false;
            try
            {
                CZaloApi zaloapi = new CZaloApi();
                CZaloApi.ZaloApiToKen zaloApiToKen = zaloapi.GetToken(code, "");
                if (zaloApiToKen.access_token != "")
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        string url = "https://" + state + ".open24.vn/api/danhmuc/ht_api/UpdateAccessTokenAndRefreshTokenZaloApi?subdomain=0973474985&access_token="
                            + zaloApiToKen.access_token + "&refresh_token=" + zaloApiToKen.refresh_token;
                        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                        var response = httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                        string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        model = true;
                    }
                }
            }
            catch
            {

            }
            return View(model);
        }

        public string GetCodeVerifier(string subdomain)
        {
            string result = "";
            using (HttpClient httpClient = new HttpClient())
            {
                string url = "https://" + subdomain + ".open24.vn/api/danhmuc/ht_api/GetZaloCodeVerifier?subdomain=" + subdomain;
                var response = httpClient.GetAsync(url).GetAwaiter().GetResult();
                string apiResult = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                dynamic dresult = JObject.Parse(apiResult);
                result = dresult.mess;
            }
            return result;
        }
        #endregion
    }

    public class DangkyViewModel
    {
        public List<NganhNgheKinhDoanh> NganhNghe { get; set; }
        public List<TinhThanhQuanHuyenDAO> TinhThanh { get; set; }
    }
    public class TinhThanhQuanHuyenDAO
    {
        public Guid ID { get; set; }
        public string TenTinhThanhQuanHuyen { get; set; }
    }
    public class ShopInfo
    {
        public string TenCuaHang { get; set; }
        public string TenDangNhap { get; set; }
        public string DiaChiQuanLy { get; set; }
    }
}