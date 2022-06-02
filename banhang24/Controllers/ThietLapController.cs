using libHT_NguoiDung;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using libHT;
using System.Linq;
using banhang24.Hellper;
using banhang24.Models;
using banhang24.Resources;
using System.Data;
using libQuy_HoaDon;
using System.Web.Script.Serialization;
using System.Globalization;

namespace banhang24.Controllers
{
    //[App_Start.App_API.CheckwebAuthorize]
    public class ThietLapController : BaseController
    {
        [RBACAuthorize(RoleKey = RoleKey.HeThong_ThietLap)]
        public ActionResult ThietLapChung()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }
        public ActionResult _themmoinhanvien(string id)
        {
            return PartialView();
        }

        public ActionResult _themmoinguoidung(string id)
        {
            return PartialView();
        }

        public ActionResult _themmoicapnhatvaitro(string id)
        {
            return PartialView();
        }

        public ActionResult _suavaitro(string id)
        {
            return PartialView();
        }

        public ActionResult _themchinhanh(string id)
        {
            return PartialView();
        }

        [RBACAuthorize(RoleKey = RoleKey.MauIn_xemDs)]
        public ActionResult QuanLyMauIn()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                classHT_NguoiDung_Nhom _classHTNDN = new classHT_NguoiDung_Nhom(db);
                userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                var model = db.HT_CongTy.FirstOrDefault();
                var result = new ModelMauIn();
                result.Model = model;
                if (db.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                {
                    result.RolePermission = new PermissionMauIn()
                    {
                        TraHang = true,
                        DatHang = true,
                        HoaDon = true,
                        ChuyenHang = true,
                        DoiTraHang = true,
                        NhapHang = true,
                        PhieuChi = true,
                        PhieuThu = true,
                        TraHangNhap = true,
                        XuatHuy = true,
                        IsCopy = true,
                        IsDelete = true,
                        IsInsert = true,
                        IsUpdate = true,
                        GoiDichVu = true,
                    };
                }
                else
                {
                    var lstMaquyen = _classHTND.Select_HT_Quyen_Nhom(_classHTNDN.Gets(p => p.IDNguoiDung == ID_ND).Any() ? _classHTNDN.Gets(p => p.IDNguoiDung == ID_ND).FirstOrDefault().IDNhomNguoiDung : new Guid())
                                            .Select(p => p.MaQuyen).ToList().Where(O => !string.IsNullOrWhiteSpace(O)).ToList();
                    result.RolePermission = new PermissionMauIn()
                    {
                        TraHang = true,
                        GoiDichVu = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.GoiDichVu_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.GoiDichVu_Insert.ToUpper())),
                        DatHang = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.DatHang_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.DatHang_Insert.ToUpper())),
                        HoaDon = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.HoaDon_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.HoaDon_Insert.ToUpper())),
                        ChuyenHang = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.ChuyenHang_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.ChuyenHang_Insert.ToUpper())),
                        DoiTraHang = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.TraHang_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.TraHang_Insert.ToUpper())),
                        NhapHang = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.NhapHang_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.NhapHang_Insert.ToUpper())),
                        PhieuChi = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.SoQuy_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.SoQuy_Insert.ToUpper())),
                        PhieuThu = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.SoQuy_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.SoQuy_Insert.ToUpper())),
                        TraHangNhap = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.TraHangNhap_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.TraHangNhap_Insert.ToUpper())),
                        XuatHuy = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.XuatHuy_View.ToUpper()) || o.ToUpper().Equals(RoleMauIn.XuatHuy_Insert.ToUpper())),
                        IsCopy = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_SaoChep.ToUpper()))
                                && lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_ThemMoi.ToUpper()))
                                && lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_xemDs.ToUpper())),
                        IsDelete = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_Xoa.ToUpper()))
                                    && lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_xemDs.ToUpper())),
                        IsInsert = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_ThemMoi.ToUpper()))
                                && lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_xemDs.ToUpper())),
                        IsUpdate = lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_CapNhat.ToUpper()))
                                && lstMaquyen.Any(o => o.ToUpper().Equals(RoleMauIn.MauIn_xemDs.ToUpper())),
                    };
                }
                result.RolePermission.IsExits = (result.RolePermission.DatHang || result.RolePermission.HoaDon || result.RolePermission.ChuyenHang
                                                        || result.RolePermission.DoiTraHang || result.RolePermission.NhapHang || result.RolePermission.PhieuChi
                                                        || result.RolePermission.PhieuThu || result.RolePermission.XuatHuy) ? true : false;
                return View(result);
            }
        }
        [RBACAuthorize(RoleKey = RoleKey.NguoiDung_XemDS)]
        public ActionResult QuanLyNguoiDung()
        {
            ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
            return View();
        }
        public ActionResult QuanLyNguoiDung_2()
        {
            ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
            return View();
        }
        [RBACAuthorize(RoleKey = RoleKey.ChiNhanh_XemDs)]
        public ActionResult QuanLyChiNhanh()
        {
            return View();
        }
        [RBACAuthorize]
        public ActionResult LichSuThaoTac()
        {
            return View();

        }
        [RBACAuthorize(RoleKey = RoleKey.NhanVien_XemDs)]
        public ActionResult NhanVien()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                //try
                //{
                //    db.Database.Connection.BeginTransaction();
                //    db.Database.CommandTimeout = 1;
                //    var i = 0;
                //    while (i < 1)
                //    {
                //        var a = db.BH_HoaDon_ChiTiet.ToList();
                //        var b = db.BH_HoaDon.ToList();

                //    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine("Got expected SqlException due to command timeout ");
                //    Console.WriteLine(e);
                //}
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                var resul = new NhanVienRoleModel() { IsHRM = false };
                if (objUser_Cookies != null)
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    if (db.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        resul = new NhanVienRoleModel()
                        {
                            IsHRM = true,
                            RoleExport = true,
                            RoleImport = true,
                            RoleLoaiLuong_Update = true,
                            RoleLoaiLuong_Insert = true,
                            RoleLoaiLuong_Delete = true,
                            RolePhongBan_Insert = true,
                            RolePhongBan_Update = true,
                            RolePhongBan_Delete = true,
                            UserRoleDelete = true,
                            UserRoleInsert = true,
                            UserRoleUpdate = true
                        };
                    }
                    else
                    {
                        var nhomnguoidung = (from o in db.HT_NguoiDung_Nhom.AsQueryable()
                                             join b in db.HT_Quyen_Nhom.AsQueryable()
                                             on o.IDNhomNguoiDung equals b.ID_NhomNguoiDung
                                             where o.IDNguoiDung == ID_ND
                                             select b.MaQuyen).Distinct().ToList();
                        // .Where(o => o.IDNguoiDung == ID_ND).Select(o=>o.IDNhomNguoiDung).ToList();
                        if (nhomnguoidung.Any())
                        {
                            var nhom = nhomnguoidung;
                            resul.RoleExport = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NhanVien_XuatFile.ToUpper()));
                            resul.RoleImport = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NhanVien_Import.ToUpper()));
                            resul.RoleLoaiLuong_Delete = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NS_LoaiLuong_Xoa.ToUpper()));
                            resul.RoleLoaiLuong_Insert = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NS_LoaiLuong_ThemMoi.ToUpper()));
                            resul.RoleLoaiLuong_Update = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NS_LoaiLuong_CapNhat.ToUpper()));
                            resul.RolePhongBan_Delete = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NS_PhongBan_Xoa.ToUpper()));
                            resul.RolePhongBan_Insert = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NS_PhongBan_ThemMoi.ToUpper()));
                            resul.RolePhongBan_Update = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NS_PhongBan_CapNhat.ToUpper()));
                            resul.UserRoleDelete = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NhanVien_Delete.ToUpper()));
                            resul.UserRoleInsert = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NhanVien_Insert.ToUpper()));
                            resul.UserRoleUpdate = nhom.Any(o => o != null && o.ToUpper().Equals(RoleNhanVien.NhanVien_Update.ToUpper()));
                        }

                    }

                    resul.IsHRM = db.HT_CauHinhPhanMem.Any(o => o.ThongTinChiTietNhanVien == true && o.ID_DonVi == objUser_Cookies.ID_DonVi);
                }
                return View(resul);
            }

        }
        [RBACAuthorize(RoleKey = RoleKey.ChietKhau_XemDs)]
        public ActionResult ChietKhau()
        {

            return View();

        }

        public ActionResult ChietKhauNew()
        {

            return View();

        }

        public ActionResult MauInHoaDon(string id)
        {
            string content = new banhang24.Areas.DanhMuc.Controllers.HT_ThietLapAPIController().readFile(id);
            return Content(content);
        }


        public ActionResult LoadMauInHoaDon(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                var subDomain = CookieStore.GetCookieAes("SubDomain");
                string defaultFolder = Server.MapPath("~/Template/MauIn");
                string folderCus = Server.MapPath("~/Template/MauIn/" + subDomain + "/");

                // combine 2 string --> full path
                string targetPath = Path.Combine(folderCus, id);
                string result = "";
                if (System.IO.File.Exists(targetPath))
                {
                    result = System.IO.File.ReadAllText(targetPath);
                }
                else
                {
                    targetPath = Path.Combine(defaultFolder, id);
                    result = System.IO.File.ReadAllText(targetPath);
                }

                // get img from HT_CongTy
                var data = _classHTCT.Get(null);
                var logo = string.Empty;
                if (data != null)
                {
                    logo = data.DiaChiNganHang;
                }

                // replace {Logo} to img in HT_CongTy
                var content = result;
                if (content.IndexOf("{Logo}") > -1)
                {
                    content = content.Replace("{Logo}", "<img src=\"" + logo + "\" style=\"width:34px; height: 30px\" />");
                }
                else
                {
                    // find img and replace
                    if (content.IndexOf("<img") > -1)
                    {
                        var open = content.IndexOf("img", content.IndexOf("img")) - 1; ;
                        var close = content.IndexOf("</", content.IndexOf("</"));
                        var temptable = content.Substring(open, close - open);
                        content = content.Replace(temptable, "<img src=\"" + logo + "\" style=\"width:34px; height: 30px\" />");
                    }
                }
                return Json(content, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult EventChangeLoaiMauIn(string id, string MaChungTu)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                var mauinId = new Guid();
                Guid.TryParse(id, out mauinId);
                var result = db.DM_MauIn.FirstOrDefault(o => o.ID == mauinId && o.ID_DonVi == objUser_Cookies.ID_DonVi);
                if (result != null && mauinId != new Guid())
                {
                    var selectedKhoGiay = commonEnum.DanhSachTypeMauIn.Any(o => o.Value.Equals(result.KhoGiay)) ?
                        commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Value.Equals(result.KhoGiay)).Key : (int?)null;
                    var dsMauIn = db.DM_MauIn.Where(o => o.ID_LoaiChungTu == result.ID_LoaiChungTu && o.ID_DonVi == objUser_Cookies.ID_DonVi)
                        .Select(o => new ListTypeMauIn { Key = o.ID, Value = o.TenMauIn }).ToList();
                    dsMauIn.Add(new ListTypeMauIn { Key = null, Value = "Chọn mẫu in" });
                    return ResultGetTrue(new MauInModel
                    {
                        selectedKhoGiay = selectedKhoGiay,
                        Data = result.DuLieuMauIn,
                        Name = result.TenMauIn,
                        SelectedMauIn = mauinId,
                        listMauIn = dsMauIn
                    });

                }
                else
                {
                    var file = "";
                    if (commonEnum.DanhSachMauInA4.Any(o => o.Key.Equals(MaChungTu)))
                    {
                        file = commonEnum.DanhSachMauInK80.FirstOrDefault(o => o.Key.Equals(MaChungTu)).Value;
                    }
                    var dsMauIn = new List<ListTypeMauIn>();
                    var chungTu = db.DM_LoaiChungTu.FirstOrDefault(o => o.MaLoaiChungTu.Equals(MaChungTu));
                    if (chungTu != null && chungTu.DM_MauIn != null && chungTu.DM_MauIn.Count > 0)
                    {
                        if (chungTu.DM_MauIn.Any(o => o.ID_DonVi == objUser_Cookies.ID_DonVi))
                        {
                            dsMauIn = chungTu.DM_MauIn.Where(o => o.ID_DonVi == objUser_Cookies.ID_DonVi).Select(o => new ListTypeMauIn { Key = o.ID, Value = o.TenMauIn }).ToList();
                            dsMauIn.Add(new ListTypeMauIn { Key = null, Value = "Chọn mẫu in" });
                        }
                        else
                        {
                            dsMauIn = new List<ListTypeMauIn>() { new ListTypeMauIn { Key = null, Value = "Chọn mẫu in" } };
                        }
                    }
                    else
                    {
                        dsMauIn = new List<ListTypeMauIn>() { new ListTypeMauIn { Key = null, Value = "Chọn mẫu in" } };
                    }
                    return ResultGetTrue(new MauInModel
                    {
                        selectedKhoGiay = (int)commonEnum.TypeMauin.a4,
                        Data = GetFileMauIn(file),
                        Name = string.Empty,
                        SelectedMauIn = mauinId,
                        listMauIn = dsMauIn
                    });
                }
            }
        }
        [HttpGet]
        public ActionResult EventChangeMauIn(string MaChungTu, int? typeKhoGiay)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                var file = "";
                if (!commonEnum.DanhSachMauInA4.Any(o => o.Key.Equals(MaChungTu)))
                {
                    file = commonEnum.DanhSachMauInK80.FirstOrDefault(o => o.Key.Equals(MaChungTu)).Value;
                }
                else
                {
                    file = commonEnum.DanhSachMauInA4.FirstOrDefault(o => o.Key.Equals(MaChungTu)).Value;
                }
                userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                var chungTu = db.DM_LoaiChungTu.FirstOrDefault(o => o.MaLoaiChungTu.Equals(MaChungTu));
                if (chungTu != null && chungTu.DM_MauIn != null && chungTu.DM_MauIn.Count > 0 && typeKhoGiay == (int)commonEnum.TypeMauin.dangdung)
                {
                    if (chungTu.DM_MauIn.Any(o => o.ID_DonVi == objUser_Cookies.ID_DonVi))
                    {
                        var resultMauIn = chungTu.DM_MauIn.Where(o => o.ID_DonVi == objUser_Cookies.ID_DonVi);
                        var dsMauIn = resultMauIn.Select(o => new ListTypeMauIn { Key = o.ID, Value = o.TenMauIn }).ToList();
                        dsMauIn.Add(new ListTypeMauIn { Key = null, Value = "Chọn mẫu in" });
                        if (resultMauIn.Any(o => o.LaMacDinh))
                        {
                            var resultMacDinh = resultMauIn.FirstOrDefault(o => o.LaMacDinh);
                            var selectedKhoGiay = commonEnum.DanhSachTypeMauIn.Any(o => o.Value.Equals(resultMacDinh.KhoGiay)) ? commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Value.Equals(resultMacDinh.KhoGiay)).Key : (int?)null;
                            return ResultGetTrue(new MauInModel
                            {
                                selectedKhoGiay = selectedKhoGiay,
                                Data = resultMacDinh.DuLieuMauIn,
                                Name = resultMacDinh.TenMauIn,
                                SelectedMauIn = resultMacDinh.ID,
                                listMauIn = dsMauIn
                            });
                        }
                        else
                        {
                            return ResultGetTrue(new MauInModel
                            {
                                selectedKhoGiay = (int)commonEnum.TypeMauin.a4,
                                Data = GetFileMauIn(file),
                                Name = string.Empty,
                                SelectedMauIn = null,
                                listMauIn = dsMauIn
                            });
                        }
                    }
                }

                return ResultGetTrue(new MauInModel
                {
                    selectedKhoGiay = (int)commonEnum.TypeMauin.a4,
                    Data = GetFileMauIn(file),
                    Name = string.Empty,
                    SelectedMauIn = new Guid(),
                    listMauIn = new List<ListTypeMauIn>() { new ListTypeMauIn { Key = null, Value = "Chọn mẫu in" } }
                });
            }
        }


        /// <summary>
        /// Sao chép mẫu in
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CopyMauIn(string Id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                try
                {
                    Guid mauInId = new Guid();
                    Guid.TryParse(Id, out mauInId);
                    var dataMauIn = db.DM_MauIn.FirstOrDefault(o => o.ID == mauInId);

                    if (dataMauIn != null)
                    {
                        userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                        var ModelInsert = new DM_MauIn();
                        ModelInsert.ID = Guid.NewGuid();
                        ModelInsert.ID_LoaiChungTu = dataMauIn.ID_LoaiChungTu;
                        ModelInsert.TenMauIn = "Copy of " + dataMauIn.TenMauIn;
                        ModelInsert.KhoGiay = dataMauIn.KhoGiay;
                        ModelInsert.DuLieuMauIn = dataMauIn.DuLieuMauIn;
                        ModelInsert.LaMacDinh = false;
                        ModelInsert.ID_DonVi = objUser_Cookies.ID_DonVi ?? new Guid();
                        ModelInsert.NguoiTao = objUser_Cookies.TaiKhoan;
                        ModelInsert.NgayTao = DateTime.Now;
                        db.DM_MauIn.Add(ModelInsert);
                        db.SaveChanges();

                        var dsMauIn = db.DM_MauIn.Where(o => o.ID_LoaiChungTu == ModelInsert.ID_LoaiChungTu && o.ID_DonVi == objUser_Cookies.ID_DonVi);
                        return ResultPostTrue(NotificationResource.MauInSaveSuccess, new MauInModel
                        {
                            selectedKhoGiay = commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Value.Equals(ModelInsert.KhoGiay)).Key,
                            Data = ModelInsert.DuLieuMauIn,
                            Name = ModelInsert.TenMauIn,
                            SelectedMauIn = ModelInsert.ID,
                            listMauIn = dsMauIn.Select(o => new { Key = o.ID, Value = o.TenMauIn }).ToList()
                        });
                    }
                }
                catch (Exception e)
                {
                    return ResultPostFalse(NotificationResource.MauInSaveError, e.Message, new MauInModel
                    {
                        SelectedMauIn = new Guid()
                    });
                }
                return ResultPostFalse(NotificationResource.MauInSaveError, "Không tìm thấy dữ liệu.", new MauInModel
                {
                    SelectedMauIn = new Guid(),
                });
            }
        }

        /// <summary>
        /// Xóa mẫu in
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMauIn(string Id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                try
                {
                    Guid mauInId = new Guid();
                    Guid.TryParse(Id, out mauInId);
                    var dataMauIn = db.DM_MauIn.FirstOrDefault(o => o.ID == mauInId);

                    if (dataMauIn != null)
                    {
                        userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                        var dsMauIn = db.DM_MauIn.Where(o => o.ID != mauInId && o.ID_LoaiChungTu == dataMauIn.ID_LoaiChungTu && o.ID_DonVi == objUser_Cookies.ID_DonVi);

                        var result = new MauInModel();
                        if (dsMauIn.Any())
                        {
                            var mauInNew = dsMauIn.FirstOrDefault();
                            result = new MauInModel
                            {
                                selectedKhoGiay = commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Value.Equals(mauInNew.KhoGiay)).Key,
                                Data = mauInNew.DuLieuMauIn,
                                Name = mauInNew.TenMauIn,
                                SelectedMauIn = mauInNew.ID,
                                listMauIn = dsMauIn.Select(o => new { Key = o.ID, Value = o.TenMauIn }).ToList()
                            };
                        }
                        else
                        {
                            result = new MauInModel
                            {
                                selectedKhoGiay = (int)commonEnum.TypeMauin.a4,
                                Data = "",
                                Name = null,
                                SelectedMauIn = null,
                                listMauIn = null
                            };
                        }

                        db.DM_MauIn.Remove(dataMauIn);
                        db.SaveChanges();
                        return ResultPostTrue(NotificationResource.MauInDeleteSuccess, result);
                    }
                }
                catch (Exception e)
                {
                    return ResultPostFalse(NotificationResource.MauInDeleteError, e.Message, new MauInModel
                    {
                        SelectedMauIn = new Guid()
                    });
                }
                return ResultPostFalse(NotificationResource.MauInDeleteError, "Không tìm thấy dữ liệu.", new MauInModel
                {
                    SelectedMauIn = new Guid(),
                });
            }
        }

        /// <summary>
        /// Hàm lưu mẫu in hiện tại trên màn hình
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="DuLieuMauIn"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult SaveMauIn(string Id, string DuLieuMauIn)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                try
                {
                    Guid mauInId = new Guid();
                    Guid.TryParse(Id, out mauInId);
                    var dataMauIn = db.DM_MauIn.FirstOrDefault(o => o.ID == mauInId);
                    if (dataMauIn != null)
                    {
                        userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                        dataMauIn.ID_DonVi = objUser_Cookies.ID_DonVi ?? new Guid();
                        dataMauIn.NguoiSua = objUser_Cookies.TaiKhoan;
                        dataMauIn.NgaySua = DateTime.Now;
                        dataMauIn.DuLieuMauIn = DuLieuMauIn;
                        db.SaveChanges();
                        return ResultPostTrue(NotificationResource.MauInUpdateSuccess, new MauInModel
                        {
                            selectedKhoGiay = commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Value.Equals(dataMauIn.KhoGiay)).Key,
                            Data = dataMauIn.DuLieuMauIn,
                            Name = dataMauIn.TenMauIn,
                            SelectedMauIn = dataMauIn.ID,
                            listMauIn = db.DM_MauIn.Where(o => o.ID_LoaiChungTu == dataMauIn.ID_LoaiChungTu && o.ID_DonVi == objUser_Cookies.ID_DonVi).Select(o => new { Key = o.ID, Value = o.TenMauIn }).ToList()
                        });
                    }


                }
                catch (Exception e)
                {
                    return ResultPostFalse(NotificationResource.MauInUpdateError, e.Message, new MauInModel
                    {
                        SelectedMauIn = new Guid()
                    });
                }
                return ResultPostFalse(NotificationResource.MauInUpdateError, "Không tìm thấy dữ liệu.", new MauInModel
                {
                    SelectedMauIn = new Guid(),
                });
            }
        }

        /// <summary>
        /// Sửa mẫu in khi nhấn vào edit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMauIn(AddNewMauInModel model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                try
                {
                    bool checkExits = false;
                    int selectedKhoGiay = model.KhoGiayId ?? 1;
                    string file = string.Empty;
                    switch (model.KhoGiayId)
                    {
                        case (int)commonEnum.TypeMauin.a4:
                            checkExits = commonEnum.DanhSachMauInA4.Any(o => o.Key.Equals(model.MaChungTu));
                            if (checkExits)
                            {
                                file = commonEnum.DanhSachMauInA4.FirstOrDefault(o => o.Key.Equals(model.MaChungTu)).Value;
                            }
                            break;
                        case (int)commonEnum.TypeMauin.k80:
                            checkExits = commonEnum.DanhSachMauInK80.Any(o => o.Key.Equals(model.MaChungTu));
                            if (checkExits)
                            {
                                file = commonEnum.DanhSachMauInK80.FirstOrDefault(o => o.Key.Equals(model.MaChungTu)).Value;
                            }
                            break;
                        case (int)commonEnum.TypeMauin.hanghoa_dv:
                            checkExits = commonEnum.Gara_DanhSachMauHoaDon.Any(o => o.Key.Equals(commonEnum.TypeMauin.hanghoa_dv.ToString()));
                            if (checkExits)
                            {
                                file = commonEnum.Gara_DanhSachMauHoaDon.FirstOrDefault(o => o.Key.Equals(commonEnum.TypeMauin.hanghoa_dv.ToString())).Value;
                            }
                            break;
                        case (int)commonEnum.TypeMauin.nhomhang:
                            checkExits = commonEnum.Gara_DanhSachMauHoaDon.Any(o => o.Key.Equals(commonEnum.TypeMauin.nhomhang.ToString()));
                            if (checkExits)
                            {
                                file = commonEnum.Gara_DanhSachMauHoaDon.FirstOrDefault(o => o.Key.Equals(commonEnum.TypeMauin.nhomhang.ToString())).Value;
                            }
                            break; 
                        case (int)commonEnum.TypeMauin.combo:
                            checkExits = commonEnum.Gara_DanhSachMauHoaDon.Any(o => o.Key.Equals(commonEnum.TypeMauin.combo.ToString()));
                            if (checkExits)
                            {
                                file = commonEnum.Gara_DanhSachMauHoaDon.FirstOrDefault(o => o.Key.Equals(commonEnum.TypeMauin.combo.ToString())).Value;
                            }
                            break;
                        case (int)commonEnum.TypeMauin.HHDV_vaNhom:
                            checkExits = commonEnum.Gara_DanhSachMauHoaDon.Any(o => o.Key.Equals(commonEnum.TypeMauin.HHDV_vaNhom.ToString()));
                            if (checkExits)
                            {
                                file = commonEnum.Gara_DanhSachMauHoaDon.FirstOrDefault(o => o.Key.Equals(commonEnum.TypeMauin.HHDV_vaNhom.ToString())).Value;
                            }
                            break;
                    }
                    
                    userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                    var chungTu = db.DM_LoaiChungTu.FirstOrDefault(o => o.MaLoaiChungTu.Equals(model.MaChungTu));
                    int chungTuID = chungTu != null ? chungTu.ID : 0;
                    var dataMauIn = db.DM_MauIn.FirstOrDefault(o => o.ID == model.Id);
                    if (dataMauIn != null)
                    {
                        dataMauIn.ID_DonVi = objUser_Cookies.ID_DonVi ?? new Guid();
                        dataMauIn.NguoiSua = objUser_Cookies.TaiKhoan;
                        dataMauIn.NgaySua = DateTime.Now;
                        dataMauIn.TenMauIn = model.Name;
                        /// Cập nhật thêm khổ giấy
                        if (!dataMauIn.KhoGiay.Equals(commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Key.Equals(selectedKhoGiay)).Value))
                        {
                            // check xem file mẫu còn tồn tại không
                            if (checkExits)
                            {
                                dataMauIn.KhoGiay = commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Key.Equals(selectedKhoGiay)).Value;
                                dataMauIn.DuLieuMauIn = GetFileMauIn(file); ;
                            }
                            else
                            {
                                return ResultPostFalse(NotificationResource.MauInUpdateError, "Không tìm thấy dữ liệu.", new MauInModel
                                {
                                    SelectedMauIn = new Guid()
                                });
                            }
                        }
                        db.SaveChanges();
                        return ResultPostTrue(NotificationResource.MauInUpdateSuccess, new MauInModel
                        {
                            selectedKhoGiay = selectedKhoGiay,
                            Data = dataMauIn.DuLieuMauIn,
                            Name = model.Name,
                            SelectedMauIn = dataMauIn.ID,
                            listMauIn = chungTu.DM_MauIn.Where(o => o.ID_LoaiChungTu == chungTuID && o.ID_DonVi == objUser_Cookies.ID_DonVi).Select(o => new { Key = o.ID, Value = o.TenMauIn }).ToList()
                        });
                    }
                }
                catch (Exception e)
                {
                    return ResultPostFalse(NotificationResource.MauInUpdateError, e.Message, new MauInModel
                    {
                        SelectedMauIn = new Guid()
                    });
                }
                return ResultPostFalse(NotificationResource.MauInUpdateError, "Không tìm thấy dữ liệu.", new MauInModel
                {
                    SelectedMauIn = new Guid()
                });
            }
        }

        /// <summary>
        ///  Thêm mới mẫu in
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewMauIn(AddNewMauInModel model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                try
                {
                    bool checkExits;
                    int selectedKhoGiay = 1;
                    string file = string.Empty;
                    bool khoA4K80 = false;
                    if (model.KhoGiayId == (int)commonEnum.TypeMauin.k80 || model.KhoGiayId == (int)commonEnum.TypeMauin.a4)
                    {
                        khoA4K80 = true;
                        if (model.KhoGiayId == (int)commonEnum.TypeMauin.k80)
                        {
                            selectedKhoGiay = (int)commonEnum.TypeMauin.k80;
                            checkExits = commonEnum.DanhSachMauInK80.Any(o => o.Key.Equals(model.MaChungTu));
                            if (checkExits)
                            {
                                file = commonEnum.DanhSachMauInK80.FirstOrDefault(o => o.Key.Equals(model.MaChungTu)).Value;
                            }

                        }
                        else
                        {
                            selectedKhoGiay = (int)commonEnum.TypeMauin.a4;
                            checkExits = commonEnum.DanhSachMauInA4.Any(o => o.Key.Equals(model.MaChungTu));
                            if (checkExits)
                            {
                                file = commonEnum.DanhSachMauInA4.FirstOrDefault(o => o.Key.Equals(model.MaChungTu)).Value;
                            }
                        }
                    }
                    else
                    {
                        selectedKhoGiay = model.KhoGiayId ?? 3;
                        checkExits = commonEnum.Gara_MauInHoaDon.Any(o => o.Key.Equals(model.KhoGiayId));
                        if (checkExits)
                        {
                            string valuebyKey = string.Empty;
                            switch (selectedKhoGiay)
                            {
                                case (int)commonEnum.TypeMauin.hanghoa_dv:
                                    valuebyKey = commonEnum.TypeMauin.hanghoa_dv.ToString();
                                    break;
                                case (int)commonEnum.TypeMauin.nhomhang:
                                    valuebyKey = commonEnum.TypeMauin.nhomhang.ToString();
                                    break; 
                                case (int)commonEnum.TypeMauin.combo:
                                    valuebyKey = commonEnum.TypeMauin.combo.ToString();
                                    break;
                                case (int)commonEnum.TypeMauin.HHDV_vaNhom:
                                    valuebyKey = commonEnum.TypeMauin.HHDV_vaNhom.ToString();
                                    break;
                            }
                            file = commonEnum.Gara_DanhSachMauHoaDon.FirstOrDefault(o => o.Key.Equals(valuebyKey)).Value;
                        }
                    }
                    
                    if (checkExits)
                    {

                        var content = GetFileMauIn(file);
                        userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                        var chungTu = db.DM_LoaiChungTu.FirstOrDefault(o => o.MaLoaiChungTu.Equals(model.MaChungTu));
                        int chungTuID = chungTu != null ? chungTu.ID : 0;
                        var khogiay = commonEnum.DanhSachTypeMauIn.FirstOrDefault(o => o.Key.Equals(selectedKhoGiay)).Value;
                        var MauInId = InsertMauIn(model.Name, khogiay, chungTuID, content, db, objUser_Cookies);
                        return ResultPostTrue(NotificationResource.MauInInsertSuccess, new MauInModel
                        {
                            selectedKhoGiay = selectedKhoGiay,
                            Data = content,
                            Name = model.Name,
                            SelectedMauIn = MauInId,
                            listMauIn = chungTu.DM_MauIn.Where(o => o.ID_LoaiChungTu == chungTuID && o.ID_DonVi == objUser_Cookies.ID_DonVi).Select(o => new { Key = o.ID, Value = o.TenMauIn }).ToList()
                        });
                    }
                }
                catch (Exception e)
                {
                    return ResultPostFalse(NotificationResource.MauInInsertError, e.Message, new MauInModel
                    {
                        SelectedMauIn = new Guid()
                    });
                }
                return ResultPostFalse(NotificationResource.MauInInsertError, "Không tìm thấy dữ liệu.", new MauInModel
                {
                    SelectedMauIn = new Guid()
                });
            }
        }


        /// <summary>
        /// Thêm mới mẫu in vào DB
        /// </summary>
        /// <param name="tenmauin"></param>
        /// <param name="khogiay"></param>
        /// <param name="chungTuID"></param>
        /// <param name="dlmauin"></param>
        /// <param name="db"></param>
        /// <param name="objUser_Cookies"></param>
        /// <returns></returns>
        public Guid InsertMauIn(string tenmauin, string khogiay, int chungTuID, string dlmauin, SsoftvnContext db, userLogin objUser_Cookies)
        {
            var ModelInsert = new DM_MauIn();
            ModelInsert.ID = Guid.NewGuid();
            ModelInsert.ID_LoaiChungTu = chungTuID;
            ModelInsert.TenMauIn = tenmauin;
            ModelInsert.KhoGiay = khogiay;
            ModelInsert.DuLieuMauIn = dlmauin;
            ModelInsert.LaMacDinh = false;
            ModelInsert.ID_DonVi = objUser_Cookies.ID_DonVi ?? new Guid();
            ModelInsert.NguoiTao = objUser_Cookies.TaiKhoan;
            ModelInsert.NgayTao = DateTime.Now;
            db.DM_MauIn.Add(ModelInsert);
            db.SaveChanges();
            return ModelInsert.ID;
        }


        /// <summary>
        ///  Lấy file Default 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetFileMauIn(string file)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                var subDomain = CookieStore.GetCookieAes("SubDomain");
                string defaultFolder = Server.MapPath("~/Template/MauIn");
                string folderCus = Server.MapPath("~/Template/MauIn/");

                // combine 2 string --> full path
                string targetPath = Path.Combine(folderCus, file);
                string result = "";
                if (System.IO.File.Exists(targetPath))
                {
                    result = System.IO.File.ReadAllText(targetPath);
                }
                else
                {
                    targetPath = Path.Combine(defaultFolder, file);
                    result = System.IO.File.ReadAllText(targetPath);
                }

                // get img from HT_CongTy
                var data = _classHTCT.Get(null);
                var logo = string.Empty;
                if (data != null)
                {
                    logo = data.DiaChiNganHang;
                }

                // replace {Logo} to img in HT_CongTy
                //var content = result;
                //if (content.IndexOf("{Logo}") > -1)
                //{
                //    content = content.Replace("{Logo}", "<img src=\"" + logo + "\" style=\"width:34px; height: 30px\" />");
                //}
                //else
                //{
                //    // find img and replace
                //    if (content.IndexOf("<img") > -1)
                //    {
                //        var open = content.IndexOf("img", content.IndexOf("img")) - 1; ;
                //        var close = content.IndexOf("</", content.IndexOf("</"));
                //        var temptable = content.Substring(open, close - open);
                //        content = content.Replace(temptable, "<img src=\"" + logo + "\" style=\"width:34px; height: 30px\" />");
                //    }
                //}
                return result;
            }
        }

        [HttpGet]
        public ActionResult LoadMauInSoQuy(Guid id)
        {
            var db = SystemDBContext.GetDBContext();
            string content1 = db.DM_MauIn.Where(p => p.ID == id).FirstOrDefault().DuLieuMauIn;

            if (content1.IndexOf("{TenHangHoaMoi}") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa}")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa}")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;

                temptable = temptable.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrint\"");
                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable = temptable.Replace("{TenHangHoa}", "<span data-bind=\"text: TenHangHoa\"></span>");
                temptable = temptable.Replace("{GiaVonHienTai}", "<span data-bind=\"text: GiaVonHienTai\"></span>");
                temptable = temptable.Replace("{GiaVonMoi}", "<span data-bind=\"text: GiaVonMoi\"></span>");
                temptable = temptable.Replace("{ChenhLech}", "<span data-bind=\"text: ChenhLech\"></span>");
                temptable = temptable.Replace("{DonViTinh}", "<span data-bind=\"text: DonViTinh\"></span>");
                temptable = temptable.Replace("{DonGia}", "<span data-bind=\"text: DonGia\"></span>");
                temptable = temptable.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                temptable = temptable.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");

                content1 = content1.Replace(temptable1, temptable);

                var openTbl2 = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) - 1;
                var closeTbl2 = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) + 6;
                var temptable2 = content1.Substring(openTbl2, closeTbl2 - openTbl2);
                var temptableMH = temptable2;

                temptable2 = temptable2.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrintMH\"");
                temptable2 = temptable2.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable2 = temptable2.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable2 = temptable2.Replace("{TenHangHoaMoi}", "<span data-bind=\"text: TenHangHoa\"></span>");
                temptable2 = temptable2.Replace("{DonViTinh}", "<span data-bind=\"text: DonViTinh\"></span>");
                temptable2 = temptable2.Replace("{DonGia}", "<span data-bind=\"text: DonGia\"></span>");
                temptable2 = temptable2.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                temptable2 = temptable2.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");
                content1 = content1.Replace(temptableMH, temptable2);

                content1 = content1.Replace("{TongTienTraHang}", "<span data-bind=\"text: InforHDprintf().TongTienTraHang\"></span>");
                content1 = content1.Replace("{TongTienHoaDonMua}", "<span data-bind=\"text: InforHDprintf().TongTienHoaDonMua\"></span>");
                content1 = content1.Replace("{TienTraKhach}", "<span data-bind=\"text: InforHDprintf().PhaiTraKhach\"></span>");
                content1 = content1.Replace("{KhachCanTra}", "<span data-bind=\"text: InforHDprintf().PhaiThanhToan\"></span>");
            }
            else if (content1.IndexOf("{TenHangHoa}") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa}")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa}")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;
                temptable = temptable.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrint\"");
                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable = temptable.Replace("{TenHangHoa}", "<span data-bind=\"text: TenHangHoa\"></span>");
                temptable = temptable.Replace("{GiaVonHienTai}", "<span data-bind=\"text: GiaVonHienTai\"></span>");
                temptable = temptable.Replace("{GiaVonMoi}", "<span data-bind=\"text: GiaVonMoi\"></span>");
                temptable = temptable.Replace("{ChenhLech}", "<span data-bind=\"text: ChenhLech\"></span>");
                temptable = temptable.Replace("{DonViTinh}", "<span data-bind=\"text: DonViTinh\"></span>");
                temptable = temptable.Replace("{DonGia}", "<span data-bind=\"text: DonGia\"></span>");
                temptable = temptable.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                temptable = temptable.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");

                // chi tiet chuyen hang
                temptable = temptable.Replace("{SoLuongChuyen}", "<span data-bind=\"text: SoLuongChuyen\"></span>");
                temptable = temptable.Replace("{SoLuongNhan}", "<span data-bind=\"text: SoLuongNhan\"></span>");
                temptable = temptable.Replace("{GiaChuyen}", "<span data-bind=\"text: GiaChuyen\"></span>");

                // xuat huy
                temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable = temptable.Replace("{SoLuongHuy}", "<span data-bind=\"text: SoLuongHuy\"></span>");
                temptable = temptable.Replace("{GiaVon}", "<span data-bind=\"text: GiaVon\"></span>");
                temptable = temptable.Replace("{GiaTriHuy}", "<span data-bind=\"text: GiaTriHuy\"></span>");

                content1 = content1.Replace(temptable1, temptable);
            }
            content1 = content1.Replace("{TenCuaHang}", "<span data-bind=\"text: InforHDprintf().TenCuaHang\"></span>");
            content1 = content1.Replace("{TenChiNhanh}", "<span data-bind=\"text: InforHDprintf().TenChiNhanh\"></span>");
            content1 = content1.Replace("{DienThoaiChiNhanh}", "<span data-bind=\"text: InforHDprintf().DienThoaiChiNhanh\"></span>");
            content1 = content1.Replace("{Logo}", "<img data-bind=\"attr: {src: InforHDprintf().LogoCuaHang}\" style=\"width:100% \" />");

            content1 = content1.Replace("{NgayBan}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
            content1 = content1.Replace("{NgayLapHoaDon}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
            content1 = content1.Replace("{MaHoaDon}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{TenKhachHang}", "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
            content1 = content1.Replace("{TenNhaCungCap}", "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
            content1 = content1.Replace("{DiaChi}", "<span data-bind=\"text: InforHDprintf().DiaChiKhachHang\"></span>");
            content1 = content1.Replace("{DienThoai}", "<span data-bind=\"text: InforHDprintf().DienThoaiKhachHang\"></span>");
            content1 = content1.Replace("{NhanVienBanHang}", "<span data-bind=\"text: InforHDprintf().NhanVienBanHang\"></span>");
            content1 = content1.Replace("{TenPhongBan}", "<span data-bind=\"text: InforHDprintf().TenPhongBan\"></span>");
            content1 = content1.Replace("{NguoiTao}", "<span data-bind=\"text: InforHDprintf().NguoiTaoHD\"></span>");

            content1 = content1.Replace("{GhiChu}", "<span data-bind=\"text: InforHDprintf().GhiChu\"></span>");
            content1 = content1.Replace("{TongTienHang}", "<span data-bind=\"text: InforHDprintf().TongTienHang\"></span>");
            content1 = content1.Replace("{DaThanhToan}", "<span data-bind=\"text: InforHDprintf().DaThanhToan\"></span>");
            content1 = content1.Replace("{ChietKhauHoaDon}", "<span data-bind=\"text: InforHDprintf().TongGiamGia\"></span>");
            content1 = content1.Replace("{DiaChiCuaHang}", "<span data-bind=\"text: InforHDprintf().DiaChiCuaHang\"></span>");
            content1 = content1.Replace("{PhiTraHang}", "<span data-bind=\"text: InforHDprintf().TongChiPhi\"></span>");

            content1 = content1.Replace("{TongTienTraHang}", "<span data-bind=\"text: InforHDprintf().TongTienTraHang\"></span>");
            content1 = content1.Replace("{TongCong}", "<span data-bind=\"text: InforHDprintf().TongCong\"></span>");
            content1 = content1.Replace("{TongSoLuongHang}", "<span data-bind=\"text: InforHDprintf().TongSoLuongHang\"></span>");
            content1 = content1.Replace("{ChiPhiNhap}", "<span data-bind=\"text: InforHDprintf().ChiPhiNhap\"></span>");
            content1 = content1.Replace("{GhiChu}", "<span data-bind=\"text: InforHDprintf().GhiChu\"></span>");
            content1 = content1.Replace("{NoTruoc}", "<span data-bind=\"text: InforHDprintf().NoTruoc\"></span>");
            content1 = content1.Replace("{NoSau}", "<span data-bind=\"text: InforHDprintf().NoSau\"></span>");

            // thong tin TK ngan hang cua cua hang
            //content1 = content1.Replace("{TenNganHangPOS}", "<span data-bind=\"text: InforHDprintf().TenNganHangPOS\"></span>");
            //content1 = content1.Replace("{TenChuThePOS}", "<span data-bind=\"text: InforHDprintf().TenChuThePOS\"></span>");
            //content1 = content1.Replace("{SoTaiKhoanPOS}", "<span data-bind=\"text: InforHDprintf().SoTaiKhoanPOS\"></span>");
            //content1 = content1.Replace("{TenNganHangChuyenKhoan}", "<span data-bind=\"text: InforHDprintf().TenNganHangChuyenKhoan\"></span>");
            //content1 = content1.Replace("{TenChuTheChuyenKhoan}", "<span data-bind=\"text: InforHDprintf().TenChuTheChuyenKhoan\"></span>");
            //content1 = content1.Replace("{SoTaiKhoanChuyenKhoan}", "<span data-bind=\"text: InforHDprintf().SoTaiKhoanChuyenKhoan\"></span>");

            // ChuyenHang
            content1 = content1.Replace("{ChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().ChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{NguoiChuyen}", "<span data-bind=\"text: InforHDprintf().NguoiChuyen\"></span>");
            content1 = content1.Replace("{ChiNhanhNhan}", "<span data-bind=\"text: InforHDprintf().ChiNhanhNhan\"></span>");
            content1 = content1.Replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
            content1 = content1.Replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{TongSoLuongChuyen}", "<span data-bind=\"text: InforHDprintf().TongSoLuongChuyen\"></span>");
            content1 = content1.Replace("{TongSoLuongNhan}", "<span data-bind=\"text: InforHDprintf().TongSoLuongNhan\"></span>");
            content1 = content1.Replace("{TongTienChuyen}", "<span data-bind=\"text: InforHDprintf().TongTienChuyen\"></span>");
            content1 = content1.Replace("{TongTienNhan}", "<span data-bind=\"text: InforHDprintf().TongTienNhan\"></span>");

            // phieu thu, chi
            content1 = content1.Replace("{MaPhieu}", "<span data-bind=\"text: InforHDprintf().MaPhieu\"></span>");
            content1 = content1.Replace("{NguoiNopTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
            content1 = content1.Replace("{NguoiNhanTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
            content1 = content1.Replace("{GiaTriPhieu}", "<span data-bind=\"text: InforHDprintf().GiaTriPhieu\"></span>");
            content1 = content1.Replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
            content1 = content1.Replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{NoiDungThu}", "<span data-bind=\"text: InforHDprintf().NoiDungThu\"></span>");
            content1 = content1.Replace("{TienBangChu}", "<span data-bind=\"text: InforHDprintf().TienBangChu\"></span>");
            content1 = content1.Replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{ChiNhanhBanHang}", "<span data-bind=\"text: InforHDprintf().ChiNhanhBanHang\"></span>");

            return Json(content1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadMauIn(string id)
        {
            string content1 = new banhang24.Areas.DanhMuc.Controllers.HT_ThietLapAPIController().readFile(id);

            if (content1.IndexOf("{TenHangHoaMoi}") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa}")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa}")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;

                temptable = temptable.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrint\"");
                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable = temptable.Replace("{TenHangHoa}", "<span data-bind=\"text: TenHangHoa\"></span>");
                temptable = temptable.Replace("{GiaVonHienTai}", "<span data-bind=\"text: GiaVonHienTai\"></span>");
                temptable = temptable.Replace("{GiaVonMoi}", "<span data-bind=\"text: GiaVonMoi\"></span>");
                temptable = temptable.Replace("{ChenhLech}", "<span data-bind=\"text: ChenhLech\"></span>");
                temptable = temptable.Replace("{DonViTinh}", "<span data-bind=\"text: DonViTinh\"></span>");
                temptable = temptable.Replace("{DonGia}", "<span data-bind=\"text: DonGia\"></span>");
                temptable = temptable.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                temptable = temptable.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");
                temptable = temptable.Replace("{MaLoHang}", "<span data-bind=\"text: MaLoHang\"></span>");

                temptable = temptable.Replace("{TonKho}", "<span data-bind=\"text: TonKho\"></span>");
                temptable = temptable.Replace("{KThucTe}", "<span data-bind=\"text: KThucTe\"></span>");
                temptable = temptable.Replace("{SLLech}", "<span data-bind=\"text: SLLech\"></span>");
                temptable = temptable.Replace("{GiaTriLech}", "<span data-bind=\"text: GiaTriLech\"></span>");

                content1 = content1.Replace(temptable1, temptable);

                var openTbl2 = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) - 1;
                var closeTbl2 = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoaMoi}")) + 6;
                var temptable2 = content1.Substring(openTbl2, closeTbl2 - openTbl2);
                var temptableMH = temptable2;

                temptable2 = temptable2.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrintMH\"");
                temptable2 = temptable2.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable2 = temptable2.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable2 = temptable2.Replace("{TenHangHoaMoi}", "<span data-bind=\"text: TenHangHoa\"></span>");
                temptable2 = temptable2.Replace("{DonViTinh}", "<span data-bind=\"text: DonViTinh\"></span>");
                temptable2 = temptable2.Replace("{DonGia}", "<span data-bind=\"text: DonGia\"></span>");
                temptable2 = temptable2.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                temptable2 = temptable2.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");
                content1 = content1.Replace(temptableMH, temptable2);

                content1 = content1.Replace("{TongTienTraHang}", "<span data-bind=\"text: InforHDprintf().TongTienTraHang\"></span>");
                content1 = content1.Replace("{TongTienHoaDonMua}", "<span data-bind=\"text: InforHDprintf().TongTienHoaDonMua\"></span>");
                content1 = content1.Replace("{TienTraKhach}", "<span data-bind=\"text: InforHDprintf().PhaiTraKhach\"></span>");
                content1 = content1.Replace("{KhachCanTra}", "<span data-bind=\"text: InforHDprintf().PhaiThanhToan\"></span>");
            }
            else if (content1.IndexOf("{TenHangHoa}") != -1)
            {
                var open = content1.LastIndexOf("tbody", content1.IndexOf("{TenHangHoa}")) - 1;
                var close = content1.IndexOf("tbody", content1.IndexOf("{TenHangHoa}")) + 6;
                var temptable = content1.Substring(open, close - open);
                var temptable1 = temptable;
                temptable = temptable.Replace("tbody", "tbody data-bind=\"foreach: CTHoaDonPrint\"");
                temptable = temptable.Replace("{STT}", "<span data-bind=\"text: SoThuTu\"></span>");
                temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable = temptable.Replace("{TenHangHoa}", "<span data-bind=\"text: TenHangHoa\"></span>");
                temptable = temptable.Replace("{GiaVonHienTai}", "<span data-bind=\"text: GiaVonHienTai\"></span>");
                temptable = temptable.Replace("{GiaVonMoi}", "<span data-bind=\"text: GiaVonMoi\"></span>");
                temptable = temptable.Replace("{ChenhLech}", "<span data-bind=\"text: ChenhLech\"></span>");
                temptable = temptable.Replace("{DonViTinh}", "<span data-bind=\"text: DonViTinh\"></span>");
                temptable = temptable.Replace("{DonGia}", "<span data-bind=\"text: DonGia\"></span>");
                temptable = temptable.Replace("{SoLuong}", "<span data-bind=\"text: SoLuong\"></span>");
                temptable = temptable.Replace("{ThanhTien}", "<span data-bind=\"text: ThanhTien\"></span>");
                temptable = temptable.Replace("{MaLoHang}", "<span data-bind=\"text: MaLoHang\"></span>");

                // chi tiet chuyen hang
                temptable = temptable.Replace("{SoLuongChuyen}", "<span data-bind=\"text: SoLuongChuyen\"></span>");
                temptable = temptable.Replace("{SoLuongNhan}", "<span data-bind=\"text: SoLuongNhan\"></span>");
                temptable = temptable.Replace("{GiaChuyen}", "<span data-bind=\"text: GiaChuyen\"></span>");

                // xuat huy
                temptable = temptable.Replace("{MaHangHoa}", "<span data-bind=\"text: MaHangHoa\"></span>");
                temptable = temptable.Replace("{SoLuongHuy}", "<span data-bind=\"text: SoLuongHuy\"></span>");
                temptable = temptable.Replace("{GiaVon}", "<span data-bind=\"text: GiaVon\"></span>");
                temptable = temptable.Replace("{GiaTriHuy}", "<span data-bind=\"text: GiaTriHuy\"></span>");

                content1 = content1.Replace(temptable1, temptable);
            }
            content1 = content1.Replace("{TenCuaHang}", "<span data-bind=\"text: InforHDprintf().TenCuaHang\"></span>");
            content1 = content1.Replace("{TenChiNhanh}", "<span data-bind=\"text: InforHDprintf().TenChiNhanh\"></span>");
            content1 = content1.Replace("{DienThoaiChiNhanh}", "<span data-bind=\"text: InforHDprintf().DienThoaiChiNhanh\"></span>");
            content1 = content1.Replace("{Logo}", "<img data-bind=\"attr: {src: InforHDprintf().LogoCuaHang}\" style=\"width:100% \" />");

            content1 = content1.Replace("{NgayBan}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
            content1 = content1.Replace("{NgayLapHoaDon}", "<span data-bind=\"text: InforHDprintf().NgayLapHoaDon\"></span>");
            content1 = content1.Replace("{MaHoaDon}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{MaKhachHang}", "<span data-bind=\"text: InforHDprintf().MaDoiTuong\"></span>");
            content1 = content1.Replace("{TenKhachHang}", "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
            content1 = content1.Replace("{TenNhaCungCap}", "<span data-bind=\"text: InforHDprintf().TenDoiTuong\"></span>");
            content1 = content1.Replace("{DiaChi}", "<span data-bind=\"text: InforHDprintf().DiaChiKhachHang\"></span>");
            content1 = content1.Replace("{DienThoai}", "<span data-bind=\"text: InforHDprintf().DienThoaiKhachHang\"></span>");
            content1 = content1.Replace("{NhanVienBanHang}", "<span data-bind=\"text: InforHDprintf().NhanVienBanHang\"></span>");
            content1 = content1.Replace("{TenPhongBan}", "<span data-bind=\"text: InforHDprintf().TenPhongBan\"></span>");
            content1 = content1.Replace("{NguoiTao}", "<span data-bind=\"text: InforHDprintf().NguoiTaoHD\"></span>");

            content1 = content1.Replace("{GhiChu}", "<span data-bind=\"text: InforHDprintf().GhiChu\"></span>");
            content1 = content1.Replace("{TongTienHang}", "<span data-bind=\"text: InforHDprintf().TongTienHang\"></span>");
            content1 = content1.Replace("{DaThanhToan}", "<span data-bind=\"text: InforHDprintf().DaThanhToan\"></span>");
            content1 = content1.Replace("{ChietKhauHoaDon}", "<span data-bind=\"text: InforHDprintf().TongGiamGia\"></span>");
            content1 = content1.Replace("{DiaChiCuaHang}", "<span data-bind=\"text: InforHDprintf().DiaChiCuaHang\"></span>");
            content1 = content1.Replace("{PhiTraHang}", "<span data-bind=\"text: InforHDprintf().TongChiPhi\"></span>");

            content1 = content1.Replace("{TongTienTraHang}", "<span data-bind=\"text: InforHDprintf().TongTienTraHang\"></span>");
            content1 = content1.Replace("{TongCong}", "<span data-bind=\"text: InforHDprintf().TongCong\"></span>");
            content1 = content1.Replace("{TongSoLuongHang}", "<span data-bind=\"text: InforHDprintf().TongSoLuongHang\"></span>");
            content1 = content1.Replace("{ChiPhiNhap}", "<span data-bind=\"text: InforHDprintf().ChiPhiNhap\"></span>");
            content1 = content1.Replace("{GhiChu}", "<span data-bind=\"text: InforHDprintf().GhiChu\"></span>");
            content1 = content1.Replace("{NoTruoc}", "<span data-bind=\"text: InforHDprintf().NoTruoc\"></span>");
            content1 = content1.Replace("{NoSau}", "<span data-bind=\"text: InforHDprintf().NoSau\"></span>");

            content1 = content1.Replace("{TongGiamGiaHang}", "<span data-bind=\"text: InforHDprintf().TongGiamGiaHang\"></span>");
            content1 = content1.Replace("{TongTienHangChuaChietKhau}", "<span data-bind=\"text: InforHDprintf().TongTienHangChuaCK\"></span>");
            content1 = content1.Replace("{TienPOS}", "<span data-bind=\"text: InforHDprintf().TienATM\"></span>");
            content1 = content1.Replace("{TienMat}", "<span data-bind=\"text: InforHDprintf().TienMat\"></span>");
            content1 = content1.Replace("{PTChietKhauHD}", "<span data-bind=\"text: InforHDprintf().TongChietKhau\"></span>");
            content1 = content1.Replace("{TongGiamGiaHD_HH}", "<span data-bind=\"text: InforHDprintf().TongGiamGiaHD_HH\"></span>");

            // ChuyenHang
            content1 = content1.Replace("{ChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().ChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{NguoiChuyen}", "<span data-bind=\"text: InforHDprintf().NguoiChuyen\"></span>");
            content1 = content1.Replace("{ChiNhanhNhan}", "<span data-bind=\"text: InforHDprintf().ChiNhanhNhan\"></span>");
            content1 = content1.Replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
            content1 = content1.Replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{TongSoLuongChuyen}", "<span data-bind=\"text: InforHDprintf().TongSoLuongChuyen\"></span>");
            content1 = content1.Replace("{TongSoLuongNhan}", "<span data-bind=\"text: InforHDprintf().TongSoLuongNhan\"></span>");
            content1 = content1.Replace("{TongTienChuyen}", "<span data-bind=\"text: InforHDprintf().TongTienChuyen\"></span>");
            content1 = content1.Replace("{TongTienNhan}", "<span data-bind=\"text: InforHDprintf().TongTienNhan\"></span>");

            // phieu thu, chi
            content1 = content1.Replace("{MaPhieu}", "<span data-bind=\"text: InforHDprintf().MaPhieu\"></span>");
            content1 = content1.Replace("{NguoiNopTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
            content1 = content1.Replace("{NguoiNhanTien}", "<span data-bind=\"text: InforHDprintf().NguoiNopTien\"></span>");
            content1 = content1.Replace("{GiaTriPhieu}", "<span data-bind=\"text: InforHDprintf().GiaTriPhieu\"></span>");
            content1 = content1.Replace("{NguoiNhan}", "<span data-bind=\"text: InforHDprintf().NguoiNhan\"></span>");
            content1 = content1.Replace("{MaChuyenHang}", "<span data-bind=\"text: InforHDprintf().MaHoaDon\"></span>");
            content1 = content1.Replace("{NoiDungThu}", "<span data-bind=\"text: InforHDprintf().NoiDungThu\"></span>");
            content1 = content1.Replace("{TienBangChu}", "<span data-bind=\"text: InforHDprintf().TienBangChu\"></span>");
            content1 = content1.Replace("{GhiChuChiNhanhChuyen}", "<span data-bind=\"text: InforHDprintf().GhiChuChiNhanhChuyen\"></span>");
            content1 = content1.Replace("{ChiNhanhBanHang}", "<span data-bind=\"text: InforHDprintf().ChiNhanhBanHang\"></span>");
            content1 = content1.Replace("{HoaDonLienQuan}", "<span data-bind=\"text: InforHDprintf().HoaDonLienQuan\"></span>");

            // Kiểm kho
            content1 = content1.Replace("{NguoiCanBang}", "<span data-bind=\"text: InforHDprintf().NguoiCanBang\"></span>");
            content1 = content1.Replace("{TrangThaiKK}", "<span data-bind=\"text: InforHDprintf().TrangThaiKK\"></span>");
            content1 = content1.Replace("{NgayTao}", "<span data-bind=\"text: InforHDprintf().NgayTao\"></span>");

            content1 = content1.Replace("{NgayCanBang}", "<span data-bind=\"text: InforHDprintf().NgayCanBang\"></span>");
            content1 = content1.Replace("{TongThucTe}", "<span data-bind=\"text: InforHDprintf().TongThucTe\"></span>");
            content1 = content1.Replace("{TongLechTang}", "<span data-bind=\"text: InforHDprintf().TongLechTang\"></span>");
            content1 = content1.Replace("{TongLechGiam}", "<span data-bind=\"text: InforHDprintf().TongLechGiam\"></span>");
            content1 = content1.Replace("{TongChenhLech}", "<span data-bind=\"text: InforHDprintf().TongChenhLech\"></span>");

            return Content(content1);
        }

        public ActionResult PrintPreview(string id)
        {
            return Content("");
        }

        public ActionResult PrintBlood()
        {
            return View();
        }

        [RBACAuthorize(RoleKey = RoleKey.KhuyenMai_XemDs)]
        public ActionResult KhuyenMai()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung _classHTND = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = _classHTND.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        public ActionResult InMaVach(Guid mauId, string masp, string tensp, string gia, int? item, Guid hanghoaId, Guid? banggiaId, int sobanghi = 3)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            var fileMau = db.DM_MauIn.FirstOrDefault(o => o.ID == mauId);
            var rowtable = fileMau.DuLieuMauIn;
            var tencongty = db.HT_CongTy.First().TenCongTy;
            var model = new InMaVach()
            {
                GiaSP = gia,
                MaSP = masp,
                TenSP = tensp,
                Item = item ?? 3,
                SoBanGhi = sobanghi
            };
            var size = 12;
            var highttr = 0;
            var sized = 8;
            var linh = 7;
            var height = 10;
            if (rowtable.Contains("{MaHangHoaN}") || rowtable.Contains("{MaHangHoa}"))
            {
                highttr = highttr + 1;
                size = size - 1;
            }
            if (rowtable.Contains("{GiaN}") || rowtable.Contains("{Gia}"))
            {
                highttr = highttr + 1;
                size = size - 1;
            }
            if (rowtable.Contains("{TenSanPhamN}") || rowtable.Contains("{TenSanPham}"))
            {
                highttr = highttr + 1;
                size = size - 1;
            }
            if (rowtable.Contains("{TenCuaHangN}") || rowtable.Contains("{TenCuaHang}"))
            {
                highttr = highttr + 1;
                size = size - 1;
            }

            if (highttr == 3)
            {
                height = 14;
                sized = 9;
                linh = 7;
            }
            else if (highttr == 2)
            {
                height = 22;
                sized = 10;
                linh = 8;
            }
            else if (highttr == 1)
            {
                height = 35;
                sized = 10;
                linh = 8;
            }

            string style = string.Format("style ='height:{0}px; line-height:{1}px; font-size:{2}px'", height, linh, sized);
            if (rowtable.Contains("{MaHangHoaN}") || rowtable.Contains("{MaHangHoa}"))
            {
                rowtable = rowtable.Replace("{MaHangHoaN}", "<div class=\"x-print-span-n\" " + style + ">" + masp + "</div>");
                rowtable = rowtable.Replace("{MaHangHoa}", "<div " + style + ">" + masp + "</div>");
            }
            if (rowtable.Contains("{GiaN}") || rowtable.Contains("{Gia}"))
            {
                var dmgia = (from o in db.DM_GiaBan_ChiTiet
                             join b in db.DonViQuiDois on o.ID_DonViQuiDoi equals b.ID
                             where o.ID_GiaBan == banggiaId && b.ID_HangHoa == hanghoaId
                             select o).FirstOrDefault();

                string giasp = CommonStatic.FormatVND(double.Parse((dmgia == null ? gia : dmgia.GiaBan.ToString())));
                rowtable = rowtable.Replace("{GiaN}", "<div class=\"x-print-span-n\" " + style + ">" + giasp + " VNĐ </div>");
                rowtable = rowtable.Replace("{Gia}", "<div " + style + ">" + giasp + " VNĐ</div>");
            }
            if (rowtable.Contains("{TenSanPhamN}") || rowtable.Contains("{TenSanPham}"))
            {
                rowtable = rowtable.Replace("{TenSanPhamN}", "<div class=\"x-print-span-n\" " + style + ">" + tensp + "</div>");
                rowtable = rowtable.Replace("{TenSanPham}", "<div " + style + ">" + tensp + "</div>");
            }
            if (rowtable.Contains("{TenCuaHangN}") || rowtable.Contains("{TenCuaHang}"))
            {
                rowtable = rowtable.Replace("{TenCuaHangN}", "<div style='font-size:" + size + "px; line-height:" + linh + "px;  height:" + height + "px;' class=\"x-print-span-n\">" + tencongty + "</div>");
                rowtable = rowtable.Replace("{TenCuaHang}", "<div style='font-size:" + size + "px; line-height:" + linh + "px; height:" + height + "px; '>" + tencongty + "</div>");
            }
            rowtable = rowtable.Replace("<table", "<table class=\"table-print\"");
            model.Link = "/Barcode.ashx?ht=24&vCode=" + masp;
            rowtable = rowtable.Replace("{MaVachN}", "<div style='text-align: center'><img style='max-width:132px' class=\"x-print-span-n\" src=\" " + model.Link + "\"  height='24' /></div>");
            rowtable = rowtable.Replace("{MaVach}", "<div style='text-align: center'><img style='max-width:132px' src=\" " + model.Link + "\"  height='24' /></div>");
            model.Table = rowtable;
            return View(model);
        
        }
        public ActionResult InTemMaVach(Guid mauId, Guid ID_SanPham)
        {
           
            return View();
            
        }

        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            var viewData = new ViewDataDictionary(model);
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString().ToString();
            }
        }
        [HttpPost]
        public ActionResult PostInMaVach(InListMaVach model)
        {
            var modelResult = new InListMaVach()
            {
                Item = model.Item,
                data = new List<Models.InMaVach>(),
            };
            SsoftvnContext db = SystemDBContext.GetDBContext();
            foreach (var item in model.data)
            {

                var fileMau = db.DM_MauIn.FirstOrDefault(o => o.ID == item.MauInId);
                var rowtable = fileMau.DuLieuMauIn;
                var tencongty = db.HT_CongTy.First().TenCongTy;

                var size = 12;
                var highttr = 0;
                var sized = 8;
                var linh = 7;
                var height = 10;
                if (rowtable.Contains("{MaHangHoaN}") || rowtable.Contains("{MaHangHoa}"))
                {
                    highttr = highttr + 1;
                    size = size - 1;
                }
                if (rowtable.Contains("{GiaN}") || rowtable.Contains("{Gia}"))
                {
                    highttr = highttr + 1;
                    size = size - 1;
                }
                if (rowtable.Contains("{TenSanPhamN}") || rowtable.Contains("{TenSanPham}"))
                {
                    highttr = highttr + 1;
                    size = size - 1;
                }
                if (rowtable.Contains("{TenCuaHangN}") || rowtable.Contains("{TenCuaHang}"))
                {
                    highttr = highttr + 1;
                    size = size - 1;
                }

                if (highttr == 3)
                {
                    height = 14;
                    sized = 9;
                    linh = 7;
                }
                else if (highttr == 2)
                {
                    height = 22;
                    sized = 10;
                    linh = 8;
                }
                else if (highttr == 1)
                {
                    height = 35;
                    sized = 10;
                    linh = 8;
                }

                string style = string.Format("style ='height:{0}px; line-height:{1}px; font-size:{2}px'", height, linh, sized);
                if (rowtable.Contains("{MaHangHoaN}") || rowtable.Contains("{MaHangHoa}"))
                {
                    rowtable = rowtable.Replace("{MaHangHoaN}", "<div class=\"x-print-span-n\" " + style + ">" + item.MaSP + "</div>");
                    rowtable = rowtable.Replace("{MaHangHoa}", "<div " + style + ">" + item.MaSP + "</div>");
                }
                if (rowtable.Contains("{GiaN}") || rowtable.Contains("{Gia}"))
                {
                    var dmgia = (from o in db.DM_GiaBan_ChiTiet
                                 join b in db.DonViQuiDois on o.ID_DonViQuiDoi equals b.ID
                                 where o.ID_GiaBan == model.BangGiaId && b.ID_HangHoa == item.ID_HangHoa
                                 select o).FirstOrDefault();

                    string gia = CommonStatic.FormatVND(double.Parse((dmgia == null ? item.GiaSP : dmgia.GiaBan.ToString())));
                    rowtable = rowtable.Replace("{GiaN}", "<div class=\"x-print-span-n\" " + style + ">" + gia + " VNĐ </div>");
                    rowtable = rowtable.Replace("{Gia}", "<div " + style + ">" + gia + " VNĐ</div>");
                }
                if (rowtable.Contains("{TenSanPhamN}") || rowtable.Contains("{TenSanPham}"))
                {
                    rowtable = rowtable.Replace("{TenSanPhamN}", "<div class=\"x-print-span-n\" " + style + ">" + item.TenSP + "</div>");
                    rowtable = rowtable.Replace("{TenSanPham}", "<div " + style + ">" + item.TenSP + "</div>");
                }
                if (rowtable.Contains("{TenCuaHangN}") || rowtable.Contains("{TenCuaHang}"))
                {
                    rowtable = rowtable.Replace("{TenCuaHangN}", "<div style='font-size:" + size + "px; line-height:" + linh + "px;  height:" + height + "px;' class=\"x-print-span-n\">" + tencongty + "</div>");
                    rowtable = rowtable.Replace("{TenCuaHang}", "<div style='font-size:" + size + "px; line-height:" + linh + "px; height:" + height + "px; '>" + tencongty + "</div>");
                }
                rowtable = rowtable.Replace("<table", "<table class=\"table-print\"");
                item.Link = "/Barcode.ashx?ht=24&vCode=" + item.MaSP;
                rowtable = rowtable.Replace("{MaVachN}", "<div style='text-align: center'><img style='max-width:132px' class=\"x-print-span-n\" src=\" " + item.Link + "\"  height='24' /></div>");
                rowtable = rowtable.Replace("{MaVach}", "<div style='text-align: center'><img style='max-width:132px' src=\" " + item.Link + "\"  height='24' /></div>");
                modelResult.data.Add(new Models.InMaVach() { Table = rowtable, Item = item.Item, SoBanGhi = item.SoBanGhi });
            }
            var result = RenderViewToString(this.ControllerContext, "InListMaVach", modelResult);
            //var result1 = InListMaVach(modelResult);
            return Json(result);

        }

        public ActionResult InListMaVach(InListMaVach model)
        {
            return View(model);
        }

        public ActionResult QuanLyMayChamCong()
        {
            return View();

        }
        public ActionResult TaiDuLieuMayChamCong()
        {
            return View();

        }
    }

}
