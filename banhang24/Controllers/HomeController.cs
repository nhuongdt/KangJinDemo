using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using libHT_NguoiDung;
using Newtonsoft.Json;
using Model_banhang24vn;
using System.Threading;
using System.Globalization;
using banhang24.AppCache;
using IWshRuntimeLibrary;
using banhang24.Hellper;
using libNS_NhanVien;
using banhang24.Models;
using Model_banhang24vn.DAL;

namespace banhang24.Controllers
{

    public class HomeController : Controller
    {
        //
        // GET: /Home/
        //public ActionResult Createconnection()
        //{
        //    string databaseName = "SSOFT_M";
        //    string strconn = "data source=103.28.37.146 ;initial catalog=" + databaseName + ";persist security info=True;user id=sa;password=Ssoftvn2015;MultipleActiveResultSets=True;App=EntityFramework";
        //    string strproviderName = "System.Data.SqlClient";
        //    //
        //    try
        //    {
        //        System.Web.HttpContext.Current.Session.Clear();
        //        string strCnn = ConnectionStringSystem.CreateConnectionString("M", strconn, strproviderName);
        //        CookieStore.SetCookie("SubDomain", "M", new TimeSpan(1, 0, 0, 0, 0), "M");
        //        //string strAddNewSubdomain = apirpc_Subdomain.AddNewSubdomain("M");
        //        //return Redirect("http://" + "M" + ".localhost:8657");
        //        return View("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        return View("Index");
        //    }
        //}
        //public ActionResult Index()
        //{
        //    if (Session["user"] != null)
        //    {
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //}

        //public ActionResult Index()
        //{
        //    if (Session["user"] != null)
        //    {
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //}

        public ActionResult CreateShortcut(string subdomain)
        {
            string browser = Request.Browser.Type;
            string winver = Request.Headers.Get("User-Agent");
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            WshShell shell = new WshShell();
            string shortcutAddress = deskDir + @"\Open24.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Phan mem open24.vn";
            shortcut.Arguments = "--kiosk-printing --kiosk \"https://" + subdomain + ".open24.vn\"";
            string folder1 = "";
            if (winver.Contains("coc_coc_browser"))
            {
                folder1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CocCoc\\Browser\\Application\\browser.exe";
            }
            else if (winver.Contains("chrome"))
            {
                folder1 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Google\\Chrome\\Application\\chrome.exe";
            }
            //folder1 = folder1 + " --kiosk-printing –kiosk";
            shortcut.TargetPath = folder1;
            if (folder1 != "")
            {
                shortcut.Save();
            }

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DashBoard()
        {
            string subdomain = CookieStore.GetCookieAes("SubDomain");
            CuaHangDangKy shop = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == subdomain);
            HttpCookie cookie = Request.Cookies["SubDomain"];
            if (shop.ID_NganhKinhDoanh == new Guid("C16EDDA0-F6D0-43E1-A469-844FAB143014"))
            {
                return RedirectToAction("TongQuanGaRa", "Gara");
            }
            else
            {
                return View();
            }
            return View();
        }
        public ActionResult KeepSession(string subdomain, string id, string checkremember)
        {
            //Guid id = data["iddonvi"].ToObject<Guid>();
            //subdomain = "0973474985";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);

                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                var cookie1 = HttpContext.Request.Cookies.Get("Account");
                var json1 = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(cookie1.Value), "SSOFTVN");
                var ison2 = json1.Replace("%0d%0a", "\r\n");
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = serializer.Deserialize<userLogin>(ison2);
                HT_NguoiDung ht_nguoiDung = classHTNguoiDung.Select_NguoiDung(objUser_Cookies.TaiKhoan, result.MatKhau, result.IpAddress);
                //if (ht_nguoiDung != null)
                //{
                    dynamic user = new
                    {
                        MatKhau = result.MatKhau,
                        TaiKhoan = ht_nguoiDung.TaiKhoan,
                        ID_NhanVien = ht_nguoiDung.ID_NhanVien,
                        ID = ht_nguoiDung.ID,
                        ID_DonVi = id,
                        LaAdmin = objUser_Cookies.LaAdmin,
                        IpAddress = objUser_Cookies.IpAddress
                    };
                    var json = JsonConvert.SerializeObject(user);
                    string jsonconvert = Convert.ToBase64String(Model.AesEncrypt.EncryptStringToBytes_Aes(json, "SSOFTVN"));
                    //var cookie = HttpContext.Request.Cookies.Get("Account");
                    var response = HttpContext.Response;
                    //response.Cookies.Remove("Account");
                    //1 thang xoa cookies 1 lan
                    //if (checkremember != "undefined")
                    //{
                    //    cookie.Expires = DateTime.Now.AddDays(30);
                    //}
                    cookie1.Value = jsonconvert;
                if (checkremember != "undefined")
                {
                    cookie1.Expires = DateTime.Now.AddDays(30);
                }
                else
                {
                    cookie1.Expires = DateTime.Now.AddDays(1);
                }
                if (HttpContext.Request.Url.Scheme == "https")
                {
                    cookie1.Secure = true;
                }
                response.Cookies.Remove("Account");
                response.Cookies.Add(cookie1);
                return Json(true, JsonRequestBehavior.AllowGet);
                //}
                //return RedirectToAction("QuanLyNguoiDung", "ThietLap");
            }
        }

        public ActionResult Active(string subdomain)
        {
            //if (M_DangKySuDung.Get(p => p.SubDomain == subdomain).TrangThai == false)
            //{
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Index");
            //}
            return View();
        }
        public ActionResult SendActive(string subdomain, string id)
        {
            if (M_DangKySuDung.CheckCodeActive(id, subdomain))
            {
                try
                {
                    //

                    string strCnn = ConnectionStringSystem.CreateConnectionString(subdomain);
                    //banhang24.App_Start.App_API.VMGsms.SendMsg(M_DangKySuDung.Get(p => p.SubDomain == subdomain).SoDienThoai, id);
                    if (strCnn == "")
                    {
                        return Json(subdomain + ".open24.vn", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(strCnn, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("ActiveFail", JsonRequestBehavior.AllowGet);
        }

        [App_Start.App_API.CheckwebAuthorize]
        public ActionResult Index(string subdomain)
        {
            //subdomain = "0973474985";
            #region dangkythanhcong
            ViewBag.Subdomain = subdomain;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                //db.HT_CongTy.FirstOrDefault();
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                if (subdomain != null && subdomain.Trim() != "")
                {
                    if (ConnectionStringSystem.CreateConnectionString(subdomain) == "")
                    {
                        SystemDBContext.MigrationDatabase(subdomain);

                        CookieStore.SetCookieAes("SubDomain", subdomain, new TimeSpan(30, 0, 0, 0, 0), subdomain);
                        userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                        if (objUser_Cookies != null)
                        {
                            var cookie = HttpContext.Request.Cookies.Get("Account");
                            var json = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(cookie.Value), "SSOFTVN");
                            var ison2 = json.Replace("%0d%0a", "\r\n");
                            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                            var result = serializer.Deserialize<userLogin>(ison2);

                            HT_NguoiDung objUser = classHTNguoiDung.Select_NguoiDung(objUser_Cookies.TaiKhoan, result.MatKhau, result.IpAddress);
                            if (objUser != null)
                            {
                                CuaHangDangKy shop = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == subdomain);
                                CookieStore.SetCookieAes("shop", shop.ID_NganhKinhDoanh.ToString(), new TimeSpan(30, 0, 0, 0, 0), subdomain);

                                //EventUpdateCache.CreatFIleAppcache();

                                System.DateTime HanSuDung = Model_banhang24vn.DAL.CuaHangDangKyService.Get(subdomain).HanSuDung.Value;
                                System.DateTime now = DateTime.Now;
                                System.TimeSpan diff = HanSuDung.Subtract(now);
                                ViewBag.HanSuDung = diff.Days + 1;

                                return View();
                            }
                            else
                            {
                                TempData["TenDangNhap"] = objUser_Cookies.TaiKhoan;
                                TempData["MatKhau"] = objUser_Cookies.MatKhau;
                                TempData["Loi"] = "";
                                return Redirect("/Login");
                            }
                        }
                        else
                        {
                            return Redirect("/Login");
                        }
                    }
                }
                return Redirect("http://open24.vn");
            }
            #endregion

        }

        public ActionResult TongQuan()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null && objUser_Cookies.ID_NhanVien != null)
                {
                    if (db.NS_NhanVien.Any(o => o.ID == objUser_Cookies.ID_NhanVien))
                        ViewBag.TenNhanVien = db.NS_NhanVien.FirstOrDefault(o => o.ID == objUser_Cookies.ID_NhanVien).TenNhanVien;
                }
                return View();
            }
        }

        public ActionResult RedirectToUrl()
        {
            return Redirect("http://open24.vn");
        }

        //change language
        public ActionResult ChangeLanguage(string id)
        {
            if (id != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(id);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(id);
            }
            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = id;
            cookie.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Add(cookie);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #region Login
        //[System.Web.Http.Route("Login")]
        //[App_Start.App_API.CheckwebAuthorize]
        //public ActionResult Login(string id)
        //{
        //    return View(id);
        //}

        //[HttpPost]
        //public ActionResult Login(FormCollection fc)
        //{
        //    string user = fc["txtUser"].ToString();
        //    string pass = fc["txtPassWord"].ToString();
        //    bool objUser = classHTNguoiDung.Check_LoGin(user, pass);
        //    if (objUser ==true)
        //    {
        //        Session["user"] = user;
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public ActionResult Login(FormCollection fc)
        //{
        //    string user = fc["txtUser"].ToString();
        //    string pass = fc["txtPassWord"].ToString();
        //    string objUser = classHTNguoiDung.Check_LoGin2(user, pass);
        //    var htNguoiDung = classHTNguoiDung.Select_NguoiDung(user, pass);
        //    var tennhanvien = "";
        //    if (htNguoiDung != null)
        //    {
        //        tennhanvien = htNguoiDung.TaiKhoan;
        //        if (htNguoiDung.ID_NhanVien != null)
        //        {
        //            tennhanvien = ClassNS_NhanVien.Get(s => s.ID == htNguoiDung.ID_NhanVien).TenNhanVien;
        //            Session["TenNhanVien"] = tennhanvien;
        //        }
        //        else
        //        {
        //            Session["TenNhanVien"] = "";
        //        }
        //        var laAdmin = htNguoiDung.LaAdmin == true ? "Admin" : "Người dùng";
        //        Session["LaAdmin"] = laAdmin;
        //        var id_nhanvien = htNguoiDung.ID_NhanVien;
        //        Session["ID_NhanVien"] = id_nhanvien;
        //        var id_donvi = htNguoiDung.ID_DonVi;
        //        Session["ID_DonVi"] = id_donvi;
        //        if (id_donvi != null)
        //        {
        //            var tendonvi = classDM_DonVi.Get(q => q.ID == id_donvi).TenDonVi;
        //            Session["TenDonVi"] = tendonvi;
        //        }
        //        var id = htNguoiDung.ID;
        //        Session["ID"] = id;
        //        //get nhómnguoidung
        //        var nhomnguoiDung = classHT_NguoiDung_Nhom.Select_HT_NguoiDung_Nhom(id);
        //        var idnhomnguoidung = nhomnguoiDung.IDNhomNguoiDung;
        //        Session["IDNhomNguoiDung"] = idnhomnguoidung;
        //    }
        //    //ModelState.AddModelError("error_msg", "");
        //    ViewBag.Error = "";
        //    if (objUser == "")
        //    {
        //        Session["user"] = user;
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        ViewBag.Error = objUser;
        //        //ModelState.AddModelError("error_msg", objUser);
        //        return View();
        //    }
        //}
        //public ActionResult Login()
        //{
        //    ViewBag.TenDangNhap = TempData["TenDangNhap"];
        //    ViewBag.MatKhau = TempData["MatKhau"];
        //    ViewBag.Loi = TempData["Loi"];

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Login(string tenTaiKhoan, string matKhau)
        //{
        //    bool exUser = classHTNguoiDung.Check_LoGin(tenTaiKhoan, matKhau);
        //    if (exUser)
        //    {
        //        Session["user"] = tenTaiKhoan;
        //        return RedirectToAction("Index", "Home");

        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

        [HttpPost, HttpGet]
        public ActionResult Login(FormCollection fc, string subdomain)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                if (fc.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        var id_nhanvien = "";
                        string strUserName = "";
                        string strPassword = "";
                        string remember_me = "";
                        if (fc["remember_me"] != null)
                            remember_me = fc["remember_me"].ToString().Trim();
                        if (fc["txtUser"] != null)
                            strUserName = fc["txtUser"].ToString().Trim();
                        if (fc["txtPassWord"] != null)
                            strPassword = fc["txtPassWord"].ToString().Trim();
                        if (strUserName != "" && strPassword != "")
                        {
                            string str = CookieStore.GetCookieAes("SubDomain");
                            if (str != subdomain)
                            {
                                CookieStore.SetCookieAes("SubDomain", subdomain, new TimeSpan(30, 0, 0, 0, 0), subdomain);
                            }
                            HT_NguoiDung objUser = classHTNguoiDung.Select_NguoiDung(strUserName, strPassword, "");

                            if (objUser != null)
                            {
                                NS_NhanVien objnv;
                                objnv = new ClassNS_NhanVien(db).Select_NhanVien(objUser.ID_NhanVien);
                                if (objUser.DangHoatDong == true && objnv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa && objnv.DaNghiViec != true)
                                {
                                    if (objUser.ID_NhanVien != null)
                                    {
                                        id_nhanvien = objUser.ID_NhanVien.ToString();
                                    }
                                    else
                                    {
                                        id_nhanvien = "";
                                    }
                                    objUser.MatKhau = strPassword;
                                    objUser.TaiKhoan = strUserName;
                                    dynamic user = new
                                    {
                                        MatKhau = objUser.MatKhau,
                                        TaiKhoan = objUser.TaiKhoan,
                                        ID_NhanVien = id_nhanvien,
                                        ID = objUser.ID,
                                        ID_DonVi = objUser.ID_DonVi,
                                        LaAdmin = objUser.LaAdmin,
                                    };
                                    var json = JsonConvert.SerializeObject(user);
                                    //string jsonconvert = Convert.ToBase64String(Model.AesEncrypt.EncryptStringToBytes_Aes(json, "SSOFTVN"));
                                    //var userCookie = new HttpCookie("Account", jsonconvert);
                                    ////1 thang xoa cookies 1 lan
                                    ////userCookie.Expires = DateTime.Now.AddMonths(1);
                                    ////userCookie.Domain = Request.Url.Host;
                                    //HttpContext.Response.Cookies.Add(userCookie);
                                    if (remember_me != "")
                                    {
                                        CookieStore.SetCookieAes("Account", json, new TimeSpan(30, 0, 0, 0, 0), subdomain);
                                    }
                                    else
                                    {
                                        CookieStore.SetCookieAes("Account", json, new TimeSpan(0, 0, 0, 0, 0), subdomain);
                                    }
                                    CookieStore.SetCookieAes(Hellper.SystemConsts.UserVersion, Guid.NewGuid().ToString(), new TimeSpan(30, 0, 0, 0, 0), subdomain);
                                    //List<string> lstMaquyen = classHTNguoiDung.Select_HT_Quyen_Nhom(classHT_NguoiDung_Nhom.Gets(p => p.IDNguoiDung == objUser.ID).FirstOrDefault().IDNhomNguoiDung)
                                    //    .Select(p => p.MaQuyen).ToList();
                                    //string strMaquyen = String.Join(",", lstMaquyen.ToArray());
                                    //ViewBag.Permision = strMaquyen;
                                    CookieStore.SetCookieAes(SystemConsts.NGUOIDUNGID, objUser.ID.ToString(), new TimeSpan(30, 0, 0, 0, 0), subdomain);
                                    //CookieStore.SetCookieAes("permision", strMaquyen, new TimeSpan(30, 0, 0, 0, 0), subdomain);
                                    //EventUpdateCache.CreatFIleAppcache();
                                    //CookieStore.SetCookie("SubDomain", subdomain, new TimeSpan(1, 0, 0, 0, 0), subdomain);

                                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                                    hT_NhatKySuDung.ID = Guid.NewGuid();
                                    hT_NhatKySuDung.ID_NhanVien = objUser.ID_NhanVien;
                                    hT_NhatKySuDung.ChucNang = "Hệ thống";
                                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                                    hT_NhatKySuDung.NoiDung = "Đăng nhập vào hệ thống";
                                    hT_NhatKySuDung.NoiDungChiTiet = "Đăng nhập vào hệ thống";
                                    hT_NhatKySuDung.LoaiNhatKy = 7;
                                    hT_NhatKySuDung.ID_DonVi = objUser.ID_DonVi.Value;
                                    SaveDiary.add_Diary(hT_NhatKySuDung);

                                    CuaHangDangKy shop = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == subdomain);
                                    if (shop.ID_NganhKinhDoanh== new Guid("C16EDDA0-F6D0-43E1-A469-844FAB143014"))
                                    {
                                        return RedirectToAction("TongQuanGaRa", "Gara");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                                else
                                {
                                    ViewBag.Error = "Tài khoản bị ngừng hoạt động, bạn không thể đăng nhập vào hệ thống";
                                }
                            }
                            else
                            {
                                ViewBag.Error = "'Tên đăng nhập' hoặc 'Mật khẩu' chưa hợp lệ";
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Vui lòng điền 'Tên đăng nhập' hoặc 'Mật khẩu'";
                        }
                    }
                }
                return View();
            }
        }


        [HttpPost]
        public ActionResult LoginNews(AcountLogin model, string subdomain)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (model != null)
                    {
                        
                        string ip = Request.UserHostAddress;
                        model.Ipaddress = ip;
                        classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                        Guid? id_nhanvien = null;
                        if (ModelState.IsValid)
                        {
                            if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.Password))
                            {
                                string str = CookieStore.GetCookieAes("SubDomain");
                                if (str != subdomain)
                                {
                                    CookieStore.SetCookieAes("SubDomain", subdomain, new TimeSpan(30, 0, 0, 0, 0), subdomain);
                                }
                                HT_NguoiDung objUser = classHTNguoiDung.Select_NguoiDung(model.UserName, model.Password, model.Ipaddress);

                                if (objUser != null)
                                {
                                    if (objUser.ID == Guid.Empty)
                                    {
                                        DM_DonVi donVi = new libDM_DonVi.classDM_DonVi(db).Get(p => p.ID == objUser.ID_DonVi);
                                        return Json(new { res = false, mess = "Chi nhánh " + donVi.TenDonVi + " đang tạm ngưng hoạt động. Quý khách vui lòng liên hệ tổng đài Open24 để được trợ giúp." });
                                    }
                                    else
                                    {
                                        NS_NhanVien objnv = new ClassNS_NhanVien(db).Select_NhanVien(objUser.ID_NhanVien);
                                        if (objUser.DangHoatDong == true && objnv.TrangThai != (int)commonEnumHellper.TypeIsDelete.daxoa && objnv.DaNghiViec != true)
                                        {
                                            if (objUser.ID_NhanVien != null)
                                            {
                                                id_nhanvien = new Guid(objUser.ID_NhanVien.ToString());
                                            }
                                            objUser.MatKhau = model.Password;
                                            objUser.TaiKhoan = model.UserName;
                                            userLogin user = new userLogin
                                            {
                                                MatKhau = objUser.MatKhau,
                                                TaiKhoan = objUser.TaiKhoan,
                                                ID_NhanVien = id_nhanvien,
                                                ID = objUser.ID,
                                                ID_DonVi = objUser.ID_DonVi,
                                                LaAdmin = objUser.LaAdmin,
                                                IpAddress = model.Ipaddress
                                            };
                                            var json = JsonConvert.SerializeObject(user);
                                            if (model.Remember)
                                            {
                                                CookieStore.SetCookieAes("Account", json, new TimeSpan(30, 0, 0, 0, 0), subdomain);
                                            }
                                            else
                                            {
                                                CookieStore.SetCookieAes("Account", json, new TimeSpan(0, 0, 0, 0, 0), subdomain);
                                            }
                                            CookieStore.SetCookieAes(Hellper.SystemConsts.UserVersion, Guid.NewGuid().ToString(), new TimeSpan(30, 0, 0, 0, 0), subdomain);

                                            CookieStore.SetCookieAes(SystemConsts.NGUOIDUNGID, objUser.ID.ToString(), new TimeSpan(30, 0, 0, 0, 0), subdomain);
                                            if (model.Ipaddress != "123.24.206.173")
                                            {
                                                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                                                hT_NhatKySuDung.ID = Guid.NewGuid();
                                                hT_NhatKySuDung.ID_NhanVien = objUser.ID_NhanVien;
                                                hT_NhatKySuDung.ChucNang = "Hệ thống";
                                                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                                                hT_NhatKySuDung.NoiDung = "Đăng nhập vào hệ thống";
                                                hT_NhatKySuDung.NoiDungChiTiet = "Đăng nhập vào hệ thống";
                                                hT_NhatKySuDung.LoaiNhatKy = 7;
                                                hT_NhatKySuDung.ID_DonVi = objUser.ID_DonVi.Value;
                                                SaveDiary.add_Diary(hT_NhatKySuDung);
                                            }
                                            if (new CuaHangDangKyService().UpdateCreateDatabase(subdomain))
                                                return Json(new { res = true });
                                            else
                                                return Json(new { res = false, mess = "Đã xảy ra lỗi vui lòng thử lại sau" });
                                        }
                                        else
                                        {
                                            return Json(new { res = false, mess = "Tài khoản bị ngừng hoạt động, bạn không thể đăng nhập vào hệ thống" });
                                        }
                                    }
                                }
                                else
                                {
                                    return Json(new { res = false, mess = "Tên đăng nhập' hoặc 'Mật khẩu' chưa hợp lệ" });
                                }
                            }
                            else
                            {
                                return Json(new { res = false, mess = "Vui lòng điền 'Tên đăng nhập' hoặc 'Mật khẩu" });

                            }
                        }
                    }
                    return Json(new { res = false, mess = "Không lấy được thông tin  đăng nhập" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mess = "Đã xảy ra lỗi vui lòng thử lại sau", log = ex });
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            string subdomain = CookieStore.GetCookieAes("SubDomain");
            if(subdomain != "" && subdomain != string.Empty)
            {
                SystemDBContext.MigrationDatabase(subdomain);
            }
            CuaHangDangKy shop = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == subdomain);
            ViewBag.TenCuaHang = shop.TenCuaHang;
            //ViewBag.IDNganhNgheKinhDoanh = shop.ID_NganhKinhDoanh;
            switch (shop.ID_NganhKinhDoanh.ToString().ToUpper())
            {
                case "C16EDDA0-F6D0-43E1-A469-844FAB143014":
                    ViewBag.IDNganhNgheKinhDoanh = "gara";
                    break;

                case "AC9DF2ED-FF08-488F-9A64-08433E541020":
                    ViewBag.IDNganhNgheKinhDoanh = "beauty";
                    break;

                default:
                    ViewBag.IDNganhNgheKinhDoanh = "sale";
                    break;
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login2()
        {
            string subdomain = CookieStore.GetCookieAes("SubDomain");
            if (subdomain != "" && subdomain != string.Empty)
            {
                SystemDBContext.MigrationDatabase(subdomain);
            }
            CuaHangDangKy shop = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == subdomain);
            ViewBag.TenCuaHang = shop.TenCuaHang;
            ViewBag.IDNganhNgheKinhDoanh = shop.ID_NganhKinhDoanh ;
            switch (shop.ID_NganhKinhDoanh.ToString())
            {
                case "AC9DF2ED-FF08-488F-9A64-08433E541020":
                    ViewBag.IDNganhNgheKinhDoanh = "gara";
                    break;

                case "C16EDDA0-F6D0-43E1-A469-844FAB143014":
                    ViewBag.IDNganhNgheKinhDoanh = "beauty";
                    break;

                default:
                    ViewBag.IDNganhNgheKinhDoanh = "sale";
                    break;
            }
            return View();
        }

        public ActionResult LogOut()
        {
            if (Request.Cookies["Account"] != null)
            {
                HttpCookie aCookie;
                string cookieName;
                int limit = Request.Cookies.Count;
                for (int i = 0; i < limit; i++)
                {
                    cookieName = Request.Cookies[i].Name;
                    if (cookieName != "deviceId")
                    {
                        aCookie = new HttpCookie(cookieName);
                        aCookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(aCookie);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult TaiKhoan()
        {
            return View();
        }

        public ActionResult QuanLyCheckIn()
        {
            return View();
        }

        #endregion

        #region Đăng ký sử dụng
        //public ActionResult DangKySuDung()
        //{
        //    List<NganhNgheKinhDoanh> lstNganhNghes = M_NganhNgheKinhDoanh.SelectAll();
        //    ViewBag.ID_NganhKinhDoanh = new SelectList(lstNganhNghes, "ID", "TenNganhNghe");

        //    return View();
        //}

        //[System.Web.Http.HttpPost]
        //public ActionResult DangKySuDung1(CuaHangDangKy objDangKy)
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
        //                        CookieStore.SetCookieAes("SubDomain", Indetifier, new TimeSpan(30, 0, 0, 0, 0), Indetifier);
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

        #region

        public ActionResult modal()
        {
            return View();
        }

        public ActionResult Expire()
        {
            return View();
        }

        #endregion
        [System.Web.Http.AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string subdomain, string txtEmail)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                CuaHangDangKy cuahang = Model_banhang24vn.DAL.CuaHangDangKyService.CheckEmailReturnString(subdomain.ToLower().Trim(), txtEmail.ToLower().Trim());
                if (cuahang != null)
                {
                    var token = GenerateRandomPassword(22);
                    classHTNguoiDung.SetPasswordToken(cuahang.UserKT, token, DateTime.Now.AddHours(1));
                    var resetlink = "<a href= '" + Url.Action("ResetPassword", "User", new { un = cuahang.UserKT, rt = token }, "https") + "'>Reset Password</a>";
                    string subject = "Open24.vn - Lấy lại mật khẩu";
                    string body = "<b>" + cuahang.HoTen + " thân mến,</b><br />Bạn vừa gửi yêu cầu đặt lại mật khẩu tài khoản của bạn trên hệ thống Open24. Vui lòng click link bên dưới để lấy lại mật khẩu:<br />"
                        + resetlink + "<br />Trong quá trình lấy lại mật khẩu bạn gặp phải bất kì vấn đề hay thắc mắc nào, vui lòng liên hệ với chúng tôi qua hotline: 1900 6914. <br />Thanks,<br />Open24.vn";
                    try
                    {
                        banhang24.App_Start.App_API.MailHelper.SendEmail(cuahang.Email, subject, body);
                        TempData["Message"] = "OK";
                    }
                    catch
                    {
                        TempData["Message"] = "Đã xảy ra lỗi trong quá trình lấy lại mật khẩu. Quý khách vui lòng gọi hotline: 1900 6914 để được hỗ trợ. Xin cảm ơn!";
                    }
                }
                else
                {
                    TempData["Message"] = "Email chưa đăng ký!";
                }
                return View();
            }
        }

        [System.Web.Http.AllowAnonymous]
        public ActionResult ResetPassword(string un, string rt)
        {
            using (SsoftvnContext context = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(context);
                HT_NguoiDung user = context.HT_NguoiDung.Where(p => p.TaiKhoan.ToLower().Trim() == un.ToLower()).FirstOrDefault();
                bool any = context.HT_NguoiDung.Where(p => p.ID == user.ID).Where(p => p.NguoiSua == rt).Where(p => p.NgaySua > DateTime.Now).Any();
                string subdomain = CookieStore.GetCookieAes("SubDomain");
                if (any)
                {
                    string newpassword = GenerateRandomPassword(6);
                    bool response = classHTNguoiDung.ResetPassword(user.ID, newpassword);
                    if (response)
                    {

                        var email = Model_banhang24vn.DAL.CuaHangDangKyService.Get(subdomain).Email;
                        string subject = "Open24.vn - Mật khẩu mới";
                        string body = "<b>Mật khẩu mới của bạn là:</b><br />" + newpassword;
                        try
                        {
                            banhang24.App_Start.App_API.MailHelper.SendEmail(email, subject, body);
                            TempData["Message"] = "Lấy lại mật khẩu thành công. Vui lòng kiểm tra email!";
                        }
                        catch (Exception ex)
                        {
                            TempData["Message"] = "Đã xảy ra lỗi: " + ex.Message;
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Đã xảy ra lỗi trong quá trình lấy lại mật khẩu. Vui lòng liên hệ hotline: 1900 6914 để được hỗ trợ. Xin cảm ơn!";
                    }
                }
                else
                {
                    TempData["Message"] = "Tài khoản và token không đúng. Vui lòng thử lại.";
                }
                TempData["locationhref"] = "https://" + subdomain + ".open24.vn";
                return View();
            }
        }

        private string GenerateRandomPassword(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-*&#+";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }

    }
}
