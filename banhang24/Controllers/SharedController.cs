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
using libDM_DonVi;
using System.Web.Security;
using libNS_NhanVien;
using banhang24.Hellper;
using banhang24.Models;
using System.Data.Entity;
using Model.Service;
using Microsoft.AspNet.SignalR.Client;

namespace banhang24.Controllers
{
    [App_Start.App_API.CheckwebAuthorize]
    public class SharedController : Controller
    {
        private CommonService _CommonService = new CommonService();
        public ActionResult _header()
        {
            //ConnectSignalR(HttpContext.Request);
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi classDonVi = new classDM_DonVi(db);
                ClassNS_NhanVien classNhanVien = new ClassNS_NhanVien(db);
                classHT_NguoiDung_Nhom classHTNguoiDungNhom = new classHT_NguoiDung_Nhom(db);
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                libHT.classHT_CauHinhPhanMem classHTCauHinh = new libHT.classHT_CauHinhPhanMem(db);
                classHT_Quyen classHTQuyen = new classHT_Quyen(db);

                //ViewBag.Permision = CookieStore.GetCookieAes("permision");
                var cookie1 = HttpContext.Request.Cookies.Get("Account");
                string str = CookieStore.GetCookieAes("SubDomain");
                var json1 = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(cookie1.Value), "SSOFTVN");
                var ison2 = json1.Replace("%0d%0a", "\r\n");
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = serializer.Deserialize<HT_NguoiDung>(ison2);
                var idnd = CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID);
                Guid ID_ND = Guid.Empty;
                if (string.IsNullOrWhiteSpace(idnd))
                {
                    ID_ND = result.ID;
                    CookieStore.SetCookieAes(SystemConsts.NGUOIDUNGID, result.ID.ToString(), new TimeSpan(30, 0, 0, 0, 0), str);
                }
                else
                {
                    ID_ND = new Guid(idnd);
                }
                //HT_NguoiDung_Nhom nguoidungnhom = classHTNguoiDungNhom.Gets(p => p.IDNguoiDung == ID_ND).FirstOrDefault();
                List<string> lstMaquyen = new List<string>();
                if (result.LaAdmin == false)
                {
                    // check quyen duco sudung/or khong
                    HT_NguoiDung_Nhom nguoidungnhom = classHTNguoiDungNhom.Gets(p => p.IDNguoiDung == ID_ND && p.ID_DonVi == result.ID_DonVi).FirstOrDefault();
                    if (nguoidungnhom != null)
                    {
                        lstMaquyen = classHTNguoiDung.Select_HT_Quyen_Nhom(nguoidungnhom.IDNhomNguoiDung)
                            .Join(db.HT_Quyen.Where(x => x.DuocSuDung == true),
                            quyennhom => quyennhom.MaQuyen,
                            quyen => quyen.MaQuyen,
                            (quyennhom, quyen) => new { HT_Quyen_Nhom = quyennhom, HT_Quyen = quyen })
                                            .Select(p => p.HT_Quyen_Nhom.MaQuyen).ToList();
                    }
                }
                else
                {
                    lstMaquyen = classHTQuyen.Gets(x => x.DuocSuDung == true).Select(p => p.MaQuyen).ToList();
                }
                string strMaquyen = String.Join(",", lstMaquyen.ToArray());
                ViewBag.Permision = strMaquyen;

                List<SelectListItem> selectListItems = new List<SelectListItem>();
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                var lstDonVi = classDonVi.getListDVByIDNguoiDung(objUser_Cookies.ID_NhanVien);
                var lstDonViAll = classDonVi.Gets(null);
                foreach (DM_DonViDTO item in lstDonVi)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = item.TenDonVi,
                        Value = item.ID.ToString(),
                        Selected = true
                    };
                    selectListItems.Add(selectListItem);
                }
                List<SelectListItem> selectListItems1 = new List<SelectListItem>();
                foreach (NS_NhanVien item in db.NS_NhanVien)
                {
                    SelectListItem selectListItem1 = new SelectListItem
                    {
                        Text = item.TenNhanVien,
                        Value = item.ID.ToString(),
                        Selected = true
                    };
                    selectListItems1.Add(selectListItem1);
                }
                HT_CongTy cty = db.HT_CongTy.FirstOrDefault();
                bool checkkichhoatsms = cty.DangHoatDong;
                string strIDDonVi = CookieStore.GetCookieAes("IdDonVi");
                if(strIDDonVi == null || strIDDonVi == "")
                {
                    strIDDonVi = objUser_Cookies.ID_DonVi.ToString();
                }
                CuaHangDangKy shop = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == str);
                ViewBag.Registered = shop.version == 1? true: false;
                ViewBag.TenCongTy = cty.TenCongTy;
                ViewBag.CheckSMS = checkkichhoatsms;
                ViewBag.TenTaiKhoan = objUser_Cookies.TaiKhoan.ToString();
                ViewBag.ID_NhanVien = objUser_Cookies.ID_NhanVien;
                ViewBag.TenNhanVien = objUser_Cookies.ID_NhanVien != null ? classNhanVien.Get(s => s.ID == objUser_Cookies.ID_NhanVien).TenNhanVien : "";
                ViewBag.TenDonVi = classDonVi.Get(q => q.ID == objUser_Cookies.ID_DonVi).TenDonVi;
                ViewBag.LaAdmin = objUser_Cookies.LaAdmin == true ? "Admin" : "Người dùng";
                ViewBag.ID_DonVi = strIDDonVi;
                ViewBag.IDNhomNguoiDung = classHTNguoiDungNhom.Select_HT_NguoiDung_Nhom(objUser_Cookies.ID) != null ? classHTNguoiDungNhom.Select_HT_NguoiDung_Nhom(objUser_Cookies.ID).IDNhomNguoiDung : Guid.Empty;
                ViewBag.ID = objUser_Cookies.ID;
                ViewBag.DM_DonVi = selectListItems.OrderBy(p => p.Text).ToList();
                ViewBag.DM_DonViAll = lstDonViAll.ToList();
                ViewBag.NS_NhanVien = selectListItems1;
                ViewBag.ID_NganhKinhDoanh = CookieStore.GetCookieAes("shop").ToUpper();
                ViewBag.LoadOk = true;
                ViewBag.SubDomain =  str;
                ViewBag.IsHRM = new CommonService().CheckIsHRM(CookieStore.GetCookieAes("SubDomain"));
                ViewBag.IsChiTietNhanVien = db.HT_CauHinhPhanMem.Any(o => o.ID_DonVi == result.ID_DonVi) ? db.HT_CauHinhPhanMem.FirstOrDefault(o => o.ID_DonVi == result.ID_DonVi).ThongTinChiTietNhanVien : false;
                //ViewBag.TenDon = selectListItems[0];
                string Avatar = "/Content/images/images-user.png";
                NS_NhanVien_Anh NVAnh = new NS_NhanVien_Anh();
                NVAnh = db.NS_NhanVien_Anh.Where(p => p.ID_NhanVien == objUser_Cookies.ID_NhanVien).FirstOrDefault();
                if(NVAnh != null)
                {
                    Avatar = NVAnh.URLAnh;
                }
                ViewBag.Avatar = Avatar;
                // check hide/show menu DatHang
                Guid idDonVi = result.ID_DonVi ?? Guid.Empty;
                HT_CauHinhPhanMem cauhinh = classHTCauHinh.SelectByIDDonVi(idDonVi);
                ViewBag.ThietLapDatHang = false;
                if (cauhinh != null)
                {
                    ViewBag.ThietLapDatHang = cauhinh.DatHang;
                }

                return PartialView(classDonVi.Get(q => q.ID == objUser_Cookies.ID_DonVi));
            }
        }
        public ActionResult _header2()
        {
            //ConnectSignalR(HttpContext.Request);
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DonVi classDonVi = new classDM_DonVi(db);
                ClassNS_NhanVien classNhanVien = new ClassNS_NhanVien(db);
                classHT_NguoiDung_Nhom classHTNguoiDungNhom = new classHT_NguoiDung_Nhom(db);
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                libHT.classHT_CauHinhPhanMem classHTCauHinh = new libHT.classHT_CauHinhPhanMem(db);
                classHT_Quyen classHTQuyen = new classHT_Quyen(db);

                //ViewBag.Permision = CookieStore.GetCookieAes("permision");
                var cookie1 = HttpContext.Request.Cookies.Get("Account");
                string str = CookieStore.GetCookieAes("SubDomain");
                var json1 = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(cookie1.Value), "SSOFTVN");
                var ison2 = json1.Replace("%0d%0a", "\r\n");
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = serializer.Deserialize<HT_NguoiDung>(ison2);
                var idnd = CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID);
                Guid ID_ND = Guid.Empty;
                if (string.IsNullOrWhiteSpace(idnd))
                {
                    ID_ND = result.ID;
                    CookieStore.SetCookieAes(SystemConsts.NGUOIDUNGID, result.ID.ToString(), new TimeSpan(30, 0, 0, 0, 0), str);
                }
                else
                {
                    ID_ND = new Guid(idnd);
                }
                //HT_NguoiDung_Nhom nguoidungnhom = classHTNguoiDungNhom.Gets(p => p.IDNguoiDung == ID_ND).FirstOrDefault();
                List<string> lstMaquyen = new List<string>();
                if (result.LaAdmin == false)
                {
                    // check quyen duco sudung/or khong
                    HT_NguoiDung_Nhom nguoidungnhom = classHTNguoiDungNhom.Gets(p => p.IDNguoiDung == ID_ND && p.ID_DonVi == result.ID_DonVi).FirstOrDefault();
                    if (nguoidungnhom != null)
                    {
                        lstMaquyen = classHTNguoiDung.Select_HT_Quyen_Nhom(nguoidungnhom.IDNhomNguoiDung)
                            .Join(db.HT_Quyen.Where(x => x.DuocSuDung == true),
                            quyennhom => quyennhom.MaQuyen,
                            quyen => quyen.MaQuyen,
                            (quyennhom, quyen) => new { HT_Quyen_Nhom = quyennhom, HT_Quyen = quyen })
                                            .Select(p => p.HT_Quyen_Nhom.MaQuyen).ToList();
                    }
                }
                else
                {
                    lstMaquyen = classHTQuyen.Gets(x => x.DuocSuDung == true).Select(p => p.MaQuyen).ToList();
                }
                string strMaquyen = String.Join(",", lstMaquyen.ToArray());
                ViewBag.Permision = strMaquyen;

                List<SelectListItem> selectListItems = new List<SelectListItem>();
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                var lstDonVi = classDonVi.getListDVByIDNguoiDung(objUser_Cookies.ID_NhanVien);
                var lstDonViAll = classDonVi.Gets(null);
                foreach (DM_DonViDTO item in lstDonVi)
                {
                    SelectListItem selectListItem = new SelectListItem
                    {
                        Text = item.TenDonVi,
                        Value = item.ID.ToString(),
                        Selected = true
                    };
                    selectListItems.Add(selectListItem);
                }
                List<SelectListItem> selectListItems1 = new List<SelectListItem>();
                foreach (NS_NhanVien item in db.NS_NhanVien)
                {
                    SelectListItem selectListItem1 = new SelectListItem
                    {
                        Text = item.TenNhanVien,
                        Value = item.ID.ToString(),
                        Selected = true
                    };
                    selectListItems1.Add(selectListItem1);
                }
                bool checkkichhoatsms = db.HT_CongTy.FirstOrDefault().DangHoatDong;

                ViewBag.CheckSMS = checkkichhoatsms;
                ViewBag.TenTaiKhoan = objUser_Cookies.TaiKhoan.ToString();
                ViewBag.ID_NhanVien = objUser_Cookies.ID_NhanVien;
                ViewBag.TenNhanVien = objUser_Cookies.ID_NhanVien != null ? classNhanVien.Get(s => s.ID == objUser_Cookies.ID_NhanVien).TenNhanVien : "";
                ViewBag.TenDonVi = classDonVi.Get(q => q.ID == objUser_Cookies.ID_DonVi).TenDonVi;
                ViewBag.LaAdmin = objUser_Cookies.LaAdmin == true ? "Admin" : "Người dùng";
                ViewBag.ID_DonVi = objUser_Cookies.ID_DonVi.ToString();
                ViewBag.IDNhomNguoiDung = classHTNguoiDungNhom.Select_HT_NguoiDung_Nhom(objUser_Cookies.ID) != null ? classHTNguoiDungNhom.Select_HT_NguoiDung_Nhom(objUser_Cookies.ID).IDNhomNguoiDung : Guid.Empty;
                ViewBag.ID = objUser_Cookies.ID;
                ViewBag.DM_DonVi = selectListItems.OrderBy(p => p.Text).ToList();
                ViewBag.DM_DonViAll = lstDonViAll.ToList();
                ViewBag.NS_NhanVien = selectListItems1;
                ViewBag.ID_NganhKinhDoanh = CookieStore.GetCookieAes("shop").ToUpper();
                ViewBag.LoadOk = true;
                ViewBag.SubDomain = CookieStore.GetCookieAes("SubDomain");
                ViewBag.IsHRM = new CommonService().CheckIsHRM(CookieStore.GetCookieAes("SubDomain"));
                ViewBag.IsChiTietNhanVien = db.HT_CauHinhPhanMem.Any(o => o.ID_DonVi == result.ID_DonVi) ? db.HT_CauHinhPhanMem.FirstOrDefault(o => o.ID_DonVi == result.ID_DonVi).ThongTinChiTietNhanVien : false;
                //ViewBag.TenDon = selectListItems[0];
                string Avatar = "/Content/images/images-user.png";
                NS_NhanVien_Anh NVAnh = new NS_NhanVien_Anh();
                NVAnh = db.NS_NhanVien_Anh.Where(p => p.ID_NhanVien == objUser_Cookies.ID_NhanVien).FirstOrDefault();
                if (NVAnh != null)
                {
                    Avatar = NVAnh.URLAnh;
                }
                ViewBag.Avatar = Avatar;
                // check hide/show menu DatHang
                Guid idDonVi = result.ID_DonVi ?? Guid.Empty;
                HT_CauHinhPhanMem cauhinh = classHTCauHinh.SelectByIDDonVi(idDonVi);
                ViewBag.ThietLapDatHang = false;
                if (cauhinh != null)
                {
                    ViewBag.ThietLapDatHang = cauhinh.DatHang;
                }

                return PartialView(classDonVi.Get(q => q.ID == objUser_Cookies.ID_DonVi));
            }
        }

        [HttpPost]
        public ActionResult CapNhatNguoiDung(FormCollection fc, string subdomain)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung_Nhom classHTNguoiDungNhom = new classHT_NguoiDung_Nhom(db);
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);

                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                string tenNhanVien = fc["txtTenNhanVien"].ToString();
                HT_NguoiDung ht_nguoiDung = new HT_NguoiDung();
                string tenDangNhap = fc["txtTenDangNhap"].ToString();
                string passRepeat = fc["passRepeat"].ToString();
                ht_nguoiDung.TaiKhoan = tenDangNhap;
                ht_nguoiDung.MatKhau = passRepeat;
                ht_nguoiDung.ID = new Guid(objUser_Cookies.ID.ToString());
                ht_nguoiDung.ID_DonVi = new Guid(objUser_Cookies.ID_DonVi.ToString());
                ht_nguoiDung.ID_NhanVien = new Guid(objUser_Cookies.ID_NhanVien.ToString());
                var objUser = classHTNguoiDung.Put_NguoiDung(ht_nguoiDung);
                //ViewBag.LoiMK = "Sửa người dùng thành công";
                //Session["user"] = tenDangNhap;
                dynamic user = new
                {
                    MatKhau = ht_nguoiDung.MatKhau,
                    TaiKhoan = ht_nguoiDung.TaiKhoan,
                    ID_NhanVien = ht_nguoiDung.ID_NhanVien,
                    ID = ht_nguoiDung.ID,
                    ID_DonVi = ht_nguoiDung.ID_DonVi,
                    LaAdmin = objUser_Cookies.LaAdmin,
                };
                var json = JsonConvert.SerializeObject(user);
                string jsonconvert = Convert.ToBase64String(Model.AesEncrypt.EncryptStringToBytes_Aes(json, "SSOFTVN"));
                var userCookie = new HttpCookie("Account", jsonconvert);
                //1 thang xoa cookies 1 lan
                userCookie.Expires = DateTime.Now.AddMonths(1);
                userCookie.Domain = subdomain;
                userCookie.Path = "/";
                HttpContext.Response.Cookies.Add(userCookie);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult _thongbao()
        {
            return PartialView();
        }

        public ActionResult _PopupNotificationPartial()
        {
            var subdomain = CookieStore.GetCookieAes("SubDomain");
            var result = _CommonService.GetNotificationSofware(subdomain ?? string.Empty);
            return PartialView(result);
        }

        [HttpGet]
        public ActionResult SearchPageMessage(int? numberPage, bool? notifyBirth = false, bool? notifyTonKho = false, bool? notifyDieuChuyen = false, bool? notifyLoHang = false)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classHT_NguoiDung_Nhom classHTNguoiDungNhom = new classHT_NguoiDung_Nhom(db);
                    classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                    userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                    HT_ThongBao_CaiDat httbcd = db.HT_ThongBao_CaiDat.Where(p => p.ID_NguoiDung == objUser_Cookies.ID).FirstOrDefault();
                    if (httbcd != null)
                    {
                        httbcd.NhacSinhNhat = notifyBirth.Value;
                        httbcd.NhacTonKho = notifyTonKho.Value;
                        httbcd.NhacDieuChuyen = notifyDieuChuyen.Value;
                        httbcd.NhacLoHang = notifyLoHang.Value;
                        db.Entry(httbcd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        HT_ThongBao_CaiDat tbcd = new HT_ThongBao_CaiDat();
                        tbcd.ID = Guid.NewGuid();
                        tbcd.NhacSinhNhat = notifyBirth.Value;
                        tbcd.NhacTonKho = notifyTonKho.Value;
                        tbcd.NhacDieuChuyen = notifyDieuChuyen.Value;
                        httbcd.NhacLoHang = notifyLoHang.Value;
                        tbcd.ID_NguoiDung = objUser_Cookies.ID;
                        db.HT_ThongBao_CaiDat.Add(tbcd);
                        db.SaveChanges();
                    }

                    List<HT_ThongBaoDTO> lstTB = new List<HT_ThongBaoDTO>();
                    List<HT_ThongBao> lstThongBao = new List<HT_ThongBao>();
                    var count = 0;
                    if (notifyBirth == true && notifyTonKho == true && notifyDieuChuyen == true && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString())).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == true && notifyTonKho == false && notifyDieuChuyen == false && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao == 3 || p.LoaiThongBao == 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao == 3 || p.LoaiThongBao == 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == true && notifyDieuChuyen == false && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao == 0 || p.LoaiThongBao == 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao == 0 || p.LoaiThongBao == 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == false && notifyDieuChuyen == true && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao == 1 || p.LoaiThongBao == 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao == 1 || p.LoaiThongBao == 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == true && notifyTonKho == true && notifyDieuChuyen == false && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao != 1).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao != 1).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == true && notifyTonKho == false && notifyDieuChuyen == true && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao != 0).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao != 0).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == true && notifyDieuChuyen == true && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao != 3).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao != 3).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == false && notifyDieuChuyen == false && notifyLoHang == true)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao == 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao == 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }

                    if (notifyBirth == true && notifyTonKho == true && notifyDieuChuyen == true && notifyLoHang == false)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao != 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao != 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == true && notifyTonKho == false && notifyDieuChuyen == false && notifyLoHang == false)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao == 3).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao == 3).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == true && notifyDieuChuyen == false && notifyLoHang == false)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao == 0).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao == 0).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == false && notifyDieuChuyen == true && notifyLoHang == false)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao == 1).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao == 1).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == true && notifyTonKho == true && notifyDieuChuyen == false && notifyLoHang == false)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao != 1 && p.LoaiThongBao != 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao != 1 && p.LoaiThongBao != 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == true && notifyTonKho == false && notifyDieuChuyen == true && notifyLoHang == false)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao != 0 && p.LoaiThongBao != 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao != 0 && p.LoaiThongBao != 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == true && notifyDieuChuyen == true && notifyLoHang == false)
                    {
                        count = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && !p.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) && p.LoaiThongBao != 3 && p.LoaiThongBao != 4).Count();
                        lstThongBao = db.HT_ThongBao.Where(p => p.ID_DonVi == objUser_Cookies.ID_DonVi && p.LoaiThongBao != 3 && p.LoaiThongBao != 4).OrderByDescending(p => p.NgayTao).Take(100).ToList();
                    }
                    if (notifyBirth == false && notifyTonKho == false && notifyDieuChuyen == false && notifyLoHang == false)
                    {
                        lstThongBao = new List<HT_ThongBao>();
                    }

                    foreach (var item in lstThongBao)
                    {
                        item.NoiDungThongBao = item.NoiDungThongBao.Replace("key", item.ID.ToString());
                        System.DateTime ngaytao = item.NgayTao;
                        System.DateTime now = DateTime.Now;
                        System.TimeSpan diff = now.Subtract(ngaytao);
                        string date = "";

                        double tonggio = diff.TotalHours;
                        if (1 < tonggio && tonggio < 24)
                        {
                            date = Math.Floor(tonggio) + " giờ trước";
                        }
                        if (tonggio < 1)
                        {
                            if (diff.TotalMinutes < 1)
                            {
                                date = "vài giây trước";
                            }
                            else
                            {
                                date = Math.Floor(diff.TotalMinutes) + " phút trước";
                            }
                        }
                        if (tonggio > 24 && (item.NgayTao.Day + 1) == now.Day)
                        {
                            date = "Hôm qua lúc " + item.NgayTao.ToString("HH:ss");
                        }
                        if (tonggio > 24 && (item.NgayTao.Day + 1) != now.Day)
                        {
                            date = item.NgayTao.Day + " Tháng " + item.NgayTao.Month + " lúc " + item.NgayTao.ToString("HH:ss");
                        }
                        HT_ThongBaoDTO thongbao = new HT_ThongBaoDTO
                        {
                            NoiDungThongBao = item.NoiDungThongBao.ToString(),
                            NgayTao = date,
                            DaDoc = item.NguoiDungDaDoc != "" ? (item.NguoiDungDaDoc.Contains(objUser_Cookies.ID.ToString()) ? true : false) : false,
                            Image = item.LoaiThongBao == 3 ? "<img src=\"/Content/images/anhhh/gato.png\" height=\"30\"/>" : (item.LoaiThongBao == 1 ? "<img src=\"/Content/images/anhhh/trao.png\" height=\"30\"/>" : "<img src=\"/Content/images/anhhh/hetkho.png\" height=\"30\"/>"),
                        };
                        lstTB.Add(thongbao);
                    }
                    return Json(new { res = true, data = lstTB, CountTB = count }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mess = e.Message, data = new List<HT_ThongBaoDTO>() }, JsonRequestBehavior.AllowGet);
            }
        }


        static Connection _connection;
        public void ConnectSignalR(HttpRequestBase requestBase)
        {
            if (_connection == null || _connection.ConnectionId == null)
            {
                _connection = new Connection("http://localhost:50302/echo/", requestBase.ToString());
                //_connection = new HubConnection("http://localhost:50302/");
                //_hub = _connection.CreateHubProxy("hitCounter");
                _connection.Start().ContinueWith((t) =>
                {
                    if (t.IsFaulted)
                    {
                        Console.WriteLine("Connection fail");
                    }
                    else
                    {
                        Console.WriteLine("Connected");
                    }
                });
            }
        }
    }
}
