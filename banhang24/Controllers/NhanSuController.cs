using banhang24.Hellper;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using banhang24.Models;
using libNS_NhanVien;
using libHT_NguoiDung;

namespace banhang24.Controllers
{
    public class NhanSuController : Controller
    {
        [RBACAuthorize(RoleKey = RoleNhanSu.CaLamViec_XemDS)]
        public ActionResult CaLamViec()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleModel() { Delete = true, Export = true, Import = true, Insert = true, Update = true, View = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_Xoa));
                        RoleModel.Export = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_XuatFile));
                        RoleModel.Import = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_NhapFile));
                    }
                    return View(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return View(new RoleModel() { Delete = false, Export = false, Import = false, Insert = false, Update = false, Log = ex.Message });
            }
        }

        [RBACAuthorize(RoleKey = RoleNhanSu.PhieuPhanCa_XemDS)]
        public ActionResult PhanCongCaLamViec()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleModel() { Delete = true, Insert = true, Update = true, Export = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.PhieuPhanCa_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.PhieuPhanCa_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.PhieuPhanCa_Xoa));
                        RoleModel.Export = listQuyen.Any(o => o.Equals(RoleNhanSu.PhieuPhanCa_XuatFile));
                    }
                    return View(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return View(new RoleModel() { Delete = false, Insert = false, Update = false, Export = false, Log = ex.Message });
            }
            return View();
        }

        [RBACAuthorize(RoleKey = RoleNhanSu.KyTinhCong_XemDS)]
        public ActionResult KyTinhCong()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleKyTinhCongModel() { Delete = true, Insert = true, Update = true, ChotCong = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);

                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.KyTinhCong_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.KyTinhCong_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.KyTinhCong_Xoa));
                        RoleModel.ChotCong = listQuyen.Any(o => o.Equals(RoleNhanSu.KyTinhCong_ChotCong));
                    }
                    return View(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return View(new RoleKyTinhCongModel() { Delete = false, Insert = false, Update = false, ChotCong = false, Log = ex.Message });
            }
        }

        public ActionResult KyHieuCong()
        {
            return View();
        }

        [RBACAuthorize(RoleKey = RoleNhanSu.ChamCong_XemDS)]
        public ActionResult ChamCong()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleChamCongModel() { Export = true, Insert = true, GuiBangCong = true, ChamCong = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.ChamCong_AddHoSo));
                        RoleModel.ChamCong = listQuyen.Any(o => o.Equals(RoleNhanSu.ChamCong_ChamCong));
                        RoleModel.Export = listQuyen.Any(o => o.Equals(RoleNhanSu.ChamCong_XuatFile));
                        RoleModel.NhanSu = listQuyen.Any(o => o.Equals(RoleNhanSu.PhieuPhanCa_ThemMoi)); // muontamtruong
                    }
                    return View(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return View(new RoleChamCongModel() { Export = false, Insert = false, GuiBangCong = false, ChamCong = false, Log = ex.Message });
            }
        }

        [RBACAuthorize(RoleKey = RoleNhanSu.ChamCong_XemDS)]
        public ActionResult ChamCong2_1()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleChamCongModel() { Export = true, Insert = true, GuiBangCong = true, ChamCong = true, NhanSu=true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.ChamCong_AddHoSo));
                        RoleModel.ChamCong = listQuyen.Any(o => o.Equals(RoleNhanSu.ChamCong_ChamCong));// update
                        RoleModel.NhanSu = listQuyen.Any(o => o.Equals(RoleNhanSu.PhieuPhanCa_ThemMoi));// muontamtruong
                        RoleModel.Export = listQuyen.Any(o => o.Equals(RoleNhanSu.ChamCong_XuatFile));
                    }
                    return View(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return View(new RoleChamCongModel() { Export = false, Insert = false, GuiBangCong = false, ChamCong = false, NhanSu = false, Log = ex.Message });
            }
        }

        [RBACAuthorize(RoleKey = RoleNhanSu.BangLuong_XemDS)]
        public ActionResult BangLuong()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleKyTinhCongModel() { Insert = true, Update = true, Delete = true, Export = true, ChotCong = true, ThanhToan = true, MoLaiBangLuong = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_HuyBo));
                        RoleModel.ChotCong = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_PheDuyet));
                        RoleModel.ThanhToan = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_ThanhToan));
                        RoleModel.MoLaiBangLuong = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_MoLai));
                    }
                    return View(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return View(new RoleKyTinhCongModel()
                {
                    Export = false,
                    Insert = false,
                    Update = false,
                    Delete = false,
                    ChotCong = false,
                    ThanhToan = false,
                    MoLaiBangLuong = false,
                    Log = ex.Message
                });

            }
        }

        [RBACAuthorize(RoleKey = RoleNhanSu.BangLuong_ThemMoi)]
        public ActionResult BangLuongChiTiet()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleKyTinhCongModel() { View = true, Insert = true, Update = true, NhanSu = true, Export = true, ChotCong = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.View = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuongChiTiet_XemThongTin));
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_ThemMoi));
                        RoleModel.NhanSu = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuongChiTiet_ThayDoiThietLapLuong));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuongChiTiet_SuaDoi));
                        RoleModel.ChotCong = listQuyen.Any(o => o.Equals(RoleNhanSu.BangLuong_PheDuyet));
                    }
                    return View(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return View(new RoleKyTinhCongModel() { View = false, Export = false, Insert = false, Update = false, NhanSu = false, ChotCong = false, Log = ex.Message });
            }
        }

        public ActionResult LoaiBaoHiem()
        {
            return View();
        }
        public ActionResult LoaiKhenThuong()
        {
            return View();
        }
        public ActionResult TongQuanNhanSu()
        {
            return View();

        }
    }
}
