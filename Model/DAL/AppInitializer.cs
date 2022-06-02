using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Model.DAL
{
    public class AppInitializer : System.Data.Entity.CreateDatabaseIfNotExists<SsoftvnContext>
    {
        #region seedold
        //protected override void Seed(SsoftvnContext context)
        //{
        //    string str = CookieStore.GetCookieAes("SubDomain");
        //    string strErr = string.Empty;
        //    if (str == null || str.Trim() == "")
        //    {
        //        CookieStore.WriteLog("Khong the xac dinh subdomain", str);
        //        return;
        //    }
        //    else
        //    {
        //        using (context = new SsoftvnContext(str))
        //        {
        //            if (context != null && context.Database.Exists())
        //            {
        //                Model_banhang24vn.CuaHangDangKy objCheck_TenMien = Model_banhang24vn.M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == str.Trim().ToLower());
        //                if (objCheck_TenMien != null)
        //                {
        //                    Model_banhang24vn.BanHang24vnContext dbMacDinh = new Model_banhang24vn.BanHang24vnContext();
        //                    string TenCuaHang = "Open24.vn";
        //                    string DiaChi = "Open24.vn";
        //                    string Email = "";
        //                    DateTime _now = DateTime.Now;
        //                    if (objCheck_TenMien.TenCuaHang != null && objCheck_TenMien.TenCuaHang != "")
        //                    {
        //                        TenCuaHang = objCheck_TenMien.TenCuaHang;
        //                    }
        //                    if (objCheck_TenMien.DiaChi != null && objCheck_TenMien.DiaChi != "")
        //                    {
        //                        DiaChi = objCheck_TenMien.DiaChi;
        //                    }
        //                    if (objCheck_TenMien.Email != null && objCheck_TenMien.Email != "")
        //                    {
        //                        Email = objCheck_TenMien.Email;
        //                    }
        //                    #region DM_DonVi
        //                    HT_CongTy objCTy = new HT_CongTy();
        //                    objCTy.DangHoatDong = true;
        //                    objCTy.DiaChi = DiaChi;
        //                    objCTy.DiaChiNganHang = "";
        //                    objCTy.DiaChiVT = "";
        //                    objCTy.GhiChu = "";
        //                    objCTy.ID = new Guid("4DE12030-B7FE-487A-B6B2-A99665E8AE7C");
        //                    objCTy.Mail = Email;
        //                    objCTy.SoDienThoai = objCheck_TenMien.SoDienThoai;
        //                    objCTy.SoFax = "";
        //                    objCTy.TaiKhoanNganHang = "";
        //                    objCTy.TenCongTy = TenCuaHang;
        //                    objCTy.TenGiamDoc = "";
        //                    objCTy.TenKeToanTruong = "";
        //                    objCTy.TenVT = TenCuaHang;
        //                    objCTy.Website = "";
        //                    try
        //                    {
        //                        context.HT_CongTy.Add(objCTy);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao cong ty: " + ex.Message + ex.InnerException, str);
        //                        string strErr1 = ex.Message;
        //                    }
        //                    //
        //                    DM_DonVi objDVi = new DM_DonVi();
        //                    objDVi.DiaChi = DiaChi;
        //                    objDVi.HienThi_Chinh = true;
        //                    objDVi.HienThi_Phu = true;
        //                    objDVi.ID = new Guid("D93B17EA-89B9-4ECF-B242-D03B8CDE71DE");
        //                    objDVi.KiTuDanhMa = objCheck_TenMien.SubDomain;
        //                    objDVi.MaDonVi = "CTY";
        //                    objDVi.NgayTao = DateTime.Now;
        //                    objDVi.NguoiTao = "ssoftvn";
        //                    objDVi.SoDienThoai = objCheck_TenMien.SoDienThoai;
        //                    objDVi.TenDonVi = TenCuaHang;
        //                    objDVi.Website = objCheck_TenMien.SubDomain + ".open24.vn";
        //                    try
        //                    {
        //                        context.DM_DonVi.Add(objDVi);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao don vi: " + ex.Message + ex.InnerException, str);
        //                        string strErr1 = ex.Message;
        //                    }
        //                    #endregion

        //                    #region nguoi dung, quyền

        //                    #region Nhân viên
        //                    string HoTen = "Open24.vn";
        //                    if (objCheck_TenMien.HoTen != null && objCheck_TenMien.HoTen != "")
        //                    {
        //                        HoTen = objCheck_TenMien.HoTen;
        //                    }
        //                    Guid IDNhanvien = Guid.NewGuid();
        //                    try
        //                    {
        //                        NS_NhanVien nhanvien = new NS_NhanVien();
        //                        nhanvien.ID = IDNhanvien;
        //                        nhanvien.MaNhanVien = "NV01";
        //                        nhanvien.TenNhanVien = HoTen;
        //                        nhanvien.GioiTinh = true;
        //                        nhanvien.DienThoaiDiDong = objCheck_TenMien.SoDienThoai;
        //                        nhanvien.Email = Email;
        //                        nhanvien.CapTaiKhoan = true;
        //                        nhanvien.DaNghiViec = false;
        //                        nhanvien.NguoiTao = "ssoftvn";
        //                        nhanvien.NgayTao = DateTime.Now;
        //                        context.NS_NhanVien.Add(nhanvien);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao nhan vien: " + ex.Message + ex.InnerException, str);
        //                    }

        //                    try
        //                    {
        //                        NS_QuaTrinhCongTac quatrinhcongtac = new NS_QuaTrinhCongTac();
        //                        quatrinhcongtac.ID = Guid.NewGuid();
        //                        quatrinhcongtac.ID_NhanVien = IDNhanvien;
        //                        quatrinhcongtac.ID_DonVi = objDVi.ID;
        //                        quatrinhcongtac.NgayApDung = DateTime.Now;
        //                        quatrinhcongtac.LaChucVuHienThoi = true;
        //                        quatrinhcongtac.LaDonViHienThoi = true;
        //                        quatrinhcongtac.NguoiLap = "ssoftvn";
        //                        quatrinhcongtac.NgayLap = DateTime.Now;
        //                        context.NS_QuaTrinhCongTac.Add(quatrinhcongtac);
        //                        //  context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao qua trinh cong tac: " + ex.Message + ex.InnerException, str);
        //                    }
        //                    #endregion

        //                    #region user 
        //                    Guid idUserMD = new Guid("28FEF5A1-F0F2-4B94-A4AD-081B227F3B77");
        //                    try
        //                    {
        //                        //string MatKhau = objCheck_TenMien.MatKhauKT;
        //                        string User = objCheck_TenMien.UserKT;
        //                        HT_NguoiDung objUserNew = new HT_NguoiDung();
        //                        objUserNew.ID = idUserMD;
        //                        objUserNew.ID_NhanVien = IDNhanvien;
        //                        objUserNew.DangHoatDong = true;
        //                        objUserNew.IsSystem = true;
        //                        objUserNew.LaAdmin = true;
        //                        objUserNew.LaNhanVien = true;
        //                        objUserNew.MatKhau = objCheck_TenMien.MatKhauKT;
        //                        //
        //                        objUserNew.TaiKhoan = User;
        //                        objUserNew.NguoiTao = "ssoftvn";
        //                        objUserNew.NgayTao = DateTime.Now;
        //                        objUserNew.ID_DonVi = objDVi.ID;
        //                        objUserNew.XemGiaVon = false;
        //                        //
        //                        context.HT_NguoiDung.Add(objUserNew);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao nguoi dung: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region nhóm ng dùng
        //                    HT_NhomNguoiDung objAdmin = new HT_NhomNguoiDung();
        //                    objAdmin.ID = new Guid("EE609285-F6A6-43D8-A517-BDA52F426AE5");
        //                    objAdmin.MaNhom = "ADMIN";
        //                    objAdmin.MoTa = "Nhóm quản trị";
        //                    objAdmin.NgayTao = DateTime.Now;
        //                    objAdmin.NguoiTao = "ssoftvn";
        //                    objAdmin.TenNhom = "ADMIN";
        //                    //
        //                    context.HT_NhomNguoiDung.Add(objAdmin);
        //                    try
        //                    {
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao nhom nguoi dung: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region HT_NguoiDung_Nhom
        //                    try
        //                    {
        //                        HT_NguoiDung_Nhom objNhom_User = new HT_NguoiDung_Nhom();
        //                        objNhom_User.ID = Guid.NewGuid();
        //                        objNhom_User.IDNguoiDung = idUserMD;
        //                        objNhom_User.IDNhomNguoiDung = objAdmin.ID;
        //                        objNhom_User.ID_DonVi = objDVi.ID;
        //                        //
        //                        context.HT_NguoiDung_Nhom.Add(objNhom_User);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao nguoi dung nhom: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region  HT_Quyen / HT_Quyen_Nhom
        //                    //IQueryable<Model_banhang24vn.HT_Quyen_NganhNgheKinhDoanh> lstQuyenSD = dbMacDinh.HT_Quyen_NganhNgheKinhDoanh.Where(p => p.ID_NganhKinhDoanh == objCheck_TenMien.ID_NganhKinhDoanh);
        //                    List<HT_Quyen> lstHTQuyen = dbMacDinh.HT_Quyen_NganhNgheKinhDoanh.Where(p => p.ID_NganhKinhDoanh == objCheck_TenMien.ID_NganhKinhDoanh).Select(p => new HT_Quyen
        //                    {
        //                        MaQuyen = p.HT_Quyen.MaQuyen,
        //                        TenQuyen = p.HT_Quyen.TenQuyen,
        //                        MoTa = p.HT_Quyen.MoTa,
        //                        QuyenCha = p.HT_Quyen.QuyenCha,
        //                        DuocSuDung = p.HT_Quyen.DuocSuDung
        //                    }).ToList();

        //                    try
        //                    {
        //                        if (lstHTQuyen != null)
        //                        {
        //                            context.HT_Quyen.AddRange(lstHTQuyen);
        //                            //context.SaveChanges();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh muc quyen: " + ex.Message + ex.InnerException, str);
        //                        string strErr1 = ex.Message;
        //                    }

        //                    try
        //                    {
        //                        if (lstHTQuyen != null)
        //                        {
        //                            List<HT_Quyen_Nhom> lstQuyenNhom = lstHTQuyen.Select(p => new HT_Quyen_Nhom
        //                            {
        //                                ID = Guid.NewGuid(),
        //                                ID_NhomNguoiDung = objAdmin.ID,
        //                                MaQuyen = p.MaQuyen
        //                            }).ToList();
        //                            //foreach (HT_Quyen item in lstHTQuyen)
        //                            //{
        //                            //    HT_Quyen_Nhom itemQ_N = new HT_Quyen_Nhom();
        //                            //    itemQ_N.ID = Guid.NewGuid();
        //                            //    itemQ_N.ID_NhomNguoiDung = objAdmin.ID;
        //                            //    itemQ_N.MaQuyen = item.MaQuyen;
        //                            //    //
        //                            //    context.HT_Quyen_Nhom.Add(itemQ_N);
        //                            //}
        //                            context.HT_Quyen_Nhom.AddRange(lstQuyenNhom);
        //                            //context.SaveChanges();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao quyen cho nhom nguoi dung: " + ex.Message + ex.InnerException, str);
        //                    }
        //                    #endregion

        //                    #region HT_CauHinhPhanMem
        //                    try
        //                    {
        //                        context.HT_CauHinhPhanMem.Add(new HT_CauHinhPhanMem
        //                        {
        //                            ID = Guid.NewGuid(),
        //                            ID_DonVi = new Guid("D93B17EA-89B9-4ECF-B242-D03B8CDE71DE"),
        //                            GiaVonTrungBinh = true,
        //                            CoDonViTinh = true,
        //                            DatHang = true,
        //                            XuatAm = false,
        //                            DatHangXuatAm = false,
        //                            ThayDoiThoiGianBanHang = false,
        //                            SoLuongTrenChungTu = false,
        //                            TinhNangTichDiem = false,
        //                            GioiHanThoiGianTraHang = false,
        //                            SanPhamCoThuocTinh = true,
        //                            BanVaChuyenKhiHangDaDat = false,
        //                            TinhNangSanXuatHangHoa = false,
        //                            SuDungCanDienTu = false,
        //                            KhoaSo = true,
        //                            InBaoGiaKhiBanHang = false,
        //                            QuanLyKhachHangTheoDonVi = false,
        //                            KhuyenMai = true,
        //                            LoHang = false,
        //                        });
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao cau hinh phan mem: " + ex.Message + ex.InnerException, str);
        //                    }

        //                    #endregion

        //                    #endregion

        //                    #region danh mục khác
        //                    //Task updatekiemke = new Task(() => AddAll(str));
        //                    //updatekiemke.Start();
        //                    //AddAll(str, lstHTQuyen);

        //                    #region quốc gia
        //                    try
        //                    {
        //                        List<DM_QuocGia> dmQuocGias = QuocGiaInitializer.lstQGiaInitializer;
        //                        //   var dmQuocGias = QuocGiaInitializer.GetQuocGiasKT();
        //                        //dmQuocGias.ForEach(s => context.DM_QuocGia.Add(s));
        //                        context.DM_QuocGia.AddRange(dmQuocGias);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh muc quoc gia: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region tỉnh/TP
        //                    try
        //                    {
        //                        List<DM_VungMien> dmVungs = TinhTPInitializer.GetVungMiensKT();
        //                        //dmVungs.ForEach(s => context.DM_VungMien.Add(s));
        //                        context.DM_VungMien.AddRange(dmVungs);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh muc vung mien: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    try
        //                    {
        //                        List<DM_TinhThanh> dmTinhtps = TinhTPInitializer.GetTinhThanhsKT();
        //                        //dmTinhtps.ForEach(s => context.DM_TinhThanh.Add(s));
        //                        context.DM_TinhThanh.AddRange(dmTinhtps);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh muc tinh thanh: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region quận huyện
        //                    try
        //                    {
        //                        List<DM_QuanHuyen> dmQHuyens = TinhTPInitializer.GetQuanHuyensKT();
        //                        //dmQHuyens.ForEach(s => context.DM_QuanHuyen.Add(s));
        //                        context.DM_QuanHuyen.AddRange(dmQHuyens);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh muc quan huyen: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region DM_ThueSuat
        //                    try
        //                    {
        //                        var lstVats = new List<DM_ThueSuat>()
        //                        {
        //                            new DM_ThueSuat{ID=Guid.NewGuid (),MaThueSuat="0%",ThueSuat=0, GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
        //                            new DM_ThueSuat{ID=Guid.NewGuid (), MaThueSuat="5%",ThueSuat=5,GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
        //                            new DM_ThueSuat{ID= Guid.NewGuid (),  MaThueSuat="10%",ThueSuat=10,GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"}
        //                        };
        //                        //lstVats.ForEach(s => context.DM_ThueSuat.Add(s));
        //                        context.DM_ThueSuat.AddRange(lstVats);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh muc thue suat: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region tiền tệ
        //                    DM_TienTe objTienTe = new DM_TienTe();
        //                    objTienTe.ID = new Guid("406eed2d-faae-4520-aef2-12912f83dda2");
        //                    objTienTe.ID_QuocGia = QuocGiaInitializer.idQGia_VNi;
        //                    objTienTe.LaNoiTe = true;
        //                    objTienTe.MaNgoaiTe = "VND";
        //                    objTienTe.NgayTao = DateTime.Now;
        //                    objTienTe.NguoiTao = "ssoftvn";
        //                    objTienTe.TenNgoaiTe = "Việt Nam đồng";
        //                    try
        //                    {
        //                        context.DM_TienTe.Add(objTienTe);
        //                        //context.SaveChanges();
        //                        //
        //                        DM_TyGia objTygia = new DM_TyGia();
        //                        objTygia.ID = Guid.NewGuid();
        //                        objTygia.ID_TienTe = objTienTe.ID;
        //                        objTygia.NgayTao = DateTime.Now;
        //                        objTygia.NgayTyGia = DateTime.Now;
        //                        objTygia.NguoiTao = "ssoftvn";
        //                        objTygia.TyGia = 1;
        //                        //
        //                        context.DM_TyGia.Add(objTygia);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh muc tien te: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region kho
        //                    Guid idKhoTong = new Guid("01CD02F2-4612-4104-B790-1C0373CBD72D");
        //                    try
        //                    {
        //                        DM_Kho objKho = new Model.DM_Kho();
        //                        objKho.ID = idKhoTong;
        //                        objKho.MaKho = "KHO_CTy";
        //                        objKho.NgayTao = DateTime.Now;
        //                        objKho.NguoiTao = "ssoftvn";
        //                        objKho.TenKho = "Kho tổng";

        //                        context.DM_Kho.Add(objKho);
        //                        //context.SaveChanges();
        //                        try
        //                        {
        //                            Kho_DonVi objKho_DVi = new Model.Kho_DonVi();
        //                            objKho_DVi.ID = Guid.NewGuid();
        //                            objKho_DVi.ID_DonVi = objDVi.ID;
        //                            objKho_DVi.ID_Kho = idKhoTong;
        //                            //
        //                            context.Kho_DonVi.Add(objKho_DVi);
        //                            //context.SaveChanges();
        //                            //
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            CookieStore.WriteLog("Khong the tao kho: " + ex.Message + ex.InnerException, str);
        //                            strErr = ex.Message;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        strErr = ex.Message;
        //                    }

        //                    #endregion

        //                    #region dm_loaichungtu
        //                    List<DM_LoaiChungTu> listloaichungtu = new List<DM_LoaiChungTu>()
        //                    {
        //                        new DM_LoaiChungTu{ ID = 1, MaLoaiChungTu = "HDBL", TenLoaiChungTu = "Hóa đơn bán lẻ", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ ID = 2, MaLoaiChungTu = "HDB", TenLoaiChungTu = "Hóa đơn bán", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 3, MaLoaiChungTu = "BG", TenLoaiChungTu = "Báo giá", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 4, MaLoaiChungTu = "PNK", TenLoaiChungTu = "Phiếu nhập kho", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 5, MaLoaiChungTu = "PXK", TenLoaiChungTu = "Phiếu xuất kho", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 6, MaLoaiChungTu = "TH", TenLoaiChungTu = "Trả hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 7, MaLoaiChungTu = "THNCC", TenLoaiChungTu = "Trả hàng nhà cung cấp", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 8, MaLoaiChungTu = "XH", TenLoaiChungTu = "Xuất kho", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 9, MaLoaiChungTu = "PKK", TenLoaiChungTu = "Phiếu kiểm kê", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 10, MaLoaiChungTu = "CH", TenLoaiChungTu = "Chuyển hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 11, MaLoaiChungTu = "SQPT", TenLoaiChungTu = "Phiếu thu", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 12, MaLoaiChungTu = "SQPC", TenLoaiChungTu = "Phiếu chi", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 13, MaLoaiChungTu = "NH", TenLoaiChungTu = "Nhân hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 14, MaLoaiChungTu = "DH", TenLoaiChungTu = "Đặt hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 15, MaLoaiChungTu = "CB", TenLoaiChungTu = "Điều chỉnh", GhiChu = "Điều chỉnh công nợ Khách hàng, Nhà CC", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 16, MaLoaiChungTu = "KTGV", TenLoaiChungTu = "Khởi tạo giá vốn", GhiChu = "Khởi tạo giá vốn khi tạo hàng hóa", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 17, MaLoaiChungTu = "DTH", TenLoaiChungTu = "Đổi trả hàng", GhiChu = "Đổi trả hàng hóa", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 18, MaLoaiChungTu = "DCGV", TenLoaiChungTu = "Điều chỉnh giá vốn", GhiChu = "Điều chỉnh giá vốn", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 19, MaLoaiChungTu = "GDV", TenLoaiChungTu = "Gói dịch vụ", GhiChu = "Bán gói dịch vụ", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 20, MaLoaiChungTu = "TGDV", TenLoaiChungTu = "Trả gói dịch vụ", GhiChu = "Trả gói dịch vụ", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 21, MaLoaiChungTu = "IMV", TenLoaiChungTu = "Tem - Mã vạch", GhiChu = "Tem - Mã vạch", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 22, MaLoaiChungTu = "TGT", TenLoaiChungTu = "Thẻ giá trị", GhiChu = "Bán, nạp thẻ giá trị", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                        new DM_LoaiChungTu{ID = 23, MaLoaiChungTu = "DCGT", TenLoaiChungTu = "Điều chỉnh thẻ giá trị", GhiChu = "Điều chỉnh giá trị thẻ giá trị", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
        //                    };
        //                    try
        //                    {
        //                        //listloaichungtu.ForEach(s => context.DM_LoaiChungTu.Add(s));
        //                        context.DM_LoaiChungTu.AddRange(listloaichungtu);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao loai chung tu: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion
        //                    #endregion

        //                    #region Thongbao
        //                    try
        //                    {
        //                        HT_ThongBao_CaiDat objhtthongbaocaidat = new HT_ThongBao_CaiDat();
        //                        objhtthongbaocaidat.ID = Guid.NewGuid();
        //                        objhtthongbaocaidat.ID_NguoiDung = idUserMD;
        //                        objhtthongbaocaidat.NhacSinhNhat = true;
        //                        objhtthongbaocaidat.NhacCongNo = true;
        //                        objhtthongbaocaidat.NhacTonKho = true;
        //                        objhtthongbaocaidat.NhacDieuChuyen = true;
        //                        objhtthongbaocaidat.NhacLoHang = true;
        //                        context.HT_ThongBao_CaiDat.Add(objhtthongbaocaidat);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao cai dat thong bao: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }

        //                    #endregion

        //                    #region Phòng ban
        //                    try
        //                    {
        //                        NS_PhongBan nsphongban = new NS_PhongBan();
        //                        nsphongban.ID = new Guid("6DE963A7-50AF-4E51-91E8-E242D7E7B476");
        //                        nsphongban.MaPhongBan = "PB0000";
        //                        nsphongban.TenPhongBan = "Phòng ban mặc định";
        //                        nsphongban.TrangThai = 1;
        //                        context.NS_PhongBan.Add(nsphongban);
        //                        //context.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao nhan su phong ban: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }

        //                    #endregion

        //                    #region DM_NganHang
        //                    List<DM_NganHang> lstNganHang = new List<DM_NganHang>()
        //                    {
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "ABC", TenNganHang = "Ngân hàng Á Châu (ACB)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "TPBANK", TenNganHang = "Ngân hàng Tiên Phong (TPBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "DAF", TenNganHang = "Ngân hàng Đông Á (DAF)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SEABANK", TenNganHang = "Ngân hàng Đông Nam Á (SeABank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "ABBANK", TenNganHang = "Ngân hàng An Bình (ABBANK)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "BACABANK", TenNganHang = "Ngân hàng Bắc Á (BacABank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VIETCAPITALBANK", TenNganHang = "Ngân hàng Bản Việt (VietCapitalBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "MSB", TenNganHang = "Ngân hàng Hàng Hải Việt Nam (MaritimeBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "TECHCOMBANK", TenNganHang = "Ngân hàng Kỹ thương Việt Nam (Techcombank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "KIENLONGBANK", TenNganHang = "Ngân hàng Kiên Long (KienLongBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "NAMABANK", TenNganHang = "Ngân hàng Nam Á (Nam A Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "NCB", TenNganHang = "Ngân hàng Quốc Dân (National Citizen Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VPBANK", TenNganHang = "Ngân hàng Việt Nam Thịnh Vượng (VPBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "HDBANK", TenNganHang = "Ngân hàng Phát triển nhà Thành Phố Hồ Chí Minh (HDBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "OCB", TenNganHang = "Ngân hàng Phương Đông (Orient Commercial Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "MBB", TenNganHang = "Ngân hàng Quân đội (Military Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "PVCOMBANK", TenNganHang = "Ngân hàng Đại chúng (PVcom Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VIB", TenNganHang = "Ngân hàng Quốc tế (VIBBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SCB", TenNganHang = "Ngân hàng Sài Gòn (Sài Gòn)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SGB", TenNganHang = "Ngân hàng Sài Gòn Công Thương (Saigonbank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SHB", TenNganHang = "Ngân hàng Sài Gòn - Hà Nội (SHBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "STB", TenNganHang = "Ngân hàng Sài Gòn Thương Tín (Sacombank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VAB", TenNganHang = "Ngân hàng Việt Á (VietABank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "BVB", TenNganHang = "Ngân hàng Bảo Việt (BaoVietBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VIETBANK", TenNganHang = "Ngân hàng Việt Nam Thương Tín (VietBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "PGBANK", TenNganHang = "Ngân hàng Xăng dầu Petrolimex (Petrolimex Group Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "EIB", TenNganHang = "Ngân hàng Xuất Nhập khẩu Việt Nam (Eximbank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "LPB", TenNganHang = "Ngân hàng Bưu điện Liên Việt (LienVietPostBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VCB", TenNganHang = "Ngân hàng Ngoại thương Việt Nam (Vietcombank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "CTG", TenNganHang = "Ngân hàng Công thương Việt Nam (VietinBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "BID", TenNganHang = "Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "NHCSXH/VBSP", TenNganHang = "Ngân hàng Chính sách xã hội (NHCSXH/VBSP)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VDB", TenNganHang = "Ngân hàng Phát triển Việt Nam (VDB)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "CB", TenNganHang = "Ngân hàng Xây dựng (CB)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "GPBANK", TenNganHang = "Ngân hàng Dầu khí Toàn Cầu (GPBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "OCEANBANK", TenNganHang = "Ngân hàng Đại Dương (Oceanbank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                        new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "AGRIBANK", TenNganHang = "Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam (Agribank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
        //                    };

        //                    try
        //                    {
        //                        //lstNganHang.ForEach(s => context.DM_NganHang.Add(s));
        //                        context.DM_NganHang.AddRange(lstNganHang);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao danh sach ngan hang: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region OptinForm trường thông tin
        //                    List<OptinForm_TruongThongTin> lstOFTruongThongTin = new List<OptinForm_TruongThongTin>()
        //                    {
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Ảnh đại diện", TrangThai = 1, STT = 1, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Tên khách hàng", TrangThai = 1, STT = 2, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Giới tính", TrangThai = 1, STT = 3, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Ngày sinh", TrangThai = 1, STT = 4, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Số điện thoại", TrangThai = 1, STT = 5, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Email", TrangThai = 1, STT = 6, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Địa chỉ", TrangThai = 1, STT = 7, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Tỉnh thành", TrangThai = 1, STT = 8, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Quận huyện", TrangThai = 1, STT = 9, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Mã số thuế", TrangThai = 1, STT = 10, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Khách hàng lẻ", TrangThai = 1, STT = 11, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Người giới thiệu", TrangThai = 1, STT = 12, LoaiTruongThongTin = 1},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Chọn chi nhánh", TrangThai = 1, STT = 1, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Xưng hô", TrangThai = 1, STT = 2, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Khách hàng", TrangThai = 1, STT = 3, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Bạn đi theo nhóm", TrangThai = 1, STT = 4, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Số lượng", TrangThai = 1, STT = 5, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Số điện thoại", TrangThai = 1, STT = 6, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Email", TrangThai = 1, STT = 7, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Dịch vụ yêu cầu", TrangThai = 1, STT = 8, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Thời gian", TrangThai = 1, STT = 9, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Nhân viên thực hiện", TrangThai = 1, STT = 10, LoaiTruongThongTin = 2},
        //                        new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Ghi chú", TrangThai = 1, STT = 11, LoaiTruongThongTin = 2},
        //                    };

        //                    try
        //                    {
        //                        //lstOFTruongThongTin.ForEach(s => context.OptinForm_TruongThongTin.Add(s));
        //                        context.OptinForm_TruongThongTin.AddRange(lstOFTruongThongTin);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao optinform truong thong tin: " + ex.Message + ex.InnerException, str);
        //                        strErr = ex.Message;
        //                    }
        //                    #endregion

        //                    #region DM_DoiTuong
        //                    List<DM_DoiTuong> lstDM_DoiTuong = new List<DM_DoiTuong>()
        //                    {
        //                        new DM_DoiTuong {ID = new Guid("00000000-0000-0000-0000-000000000000"), LoaiDoiTuong = 1, LaCaNhan = true, MaDoiTuong = "KL00001", TenDoiTuong = "Khách lẻ", TenDoiTuong_KhongDau = "Khach le", TenDoiTuong_ChuCaiDau = "Kl", ChiaSe = false, TheoDoi = false, ID_DonVi = objDVi.ID, TenNhomDoiTuongs = "Nhóm mặc định", NguoiTao = "ssoftvn", NgayTao = _now},
        //                        new DM_DoiTuong {ID = new Guid("00000000-0000-0000-0000-000000000002"), LoaiDoiTuong = 2, LaCaNhan = true, MaDoiTuong = "NCCL001", TenDoiTuong = "Nhà cung cấp lẻ", TenDoiTuong_KhongDau = "Nha cung cap le", TenDoiTuong_ChuCaiDau = "nccl", ChiaSe = false, TheoDoi = false, ID_DonVi = objDVi.ID, TenNhomDoiTuongs = "Nhóm mặc định", NguoiTao = "ssoftvn", NgayTao = _now},
        //                    };

        //                    try
        //                    {
        //                        //lstDM_DoiTuong.ForEach(s => context.DM_DoiTuong.Add(s));
        //                        context.DM_DoiTuong.AddRange(lstDM_DoiTuong);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao doi tuong mac dinh: " + ex.Message + ex.InnerException, str);
        //                    }
        //                    #endregion

        //                    #region DM_NhomHangHoa
        //                    List<DM_NhomHangHoa> lstDM_NhomHangHoa = new List<DM_NhomHangHoa>()
        //                    {
        //                        new DM_NhomHangHoa {ID = new Guid("00000000-0000-0000-0000-000000000000"), MaNhomHangHoa = "NHMD00001", TenNhomHangHoa = "Nhóm hàng hóa mặc định", LaNhomHangHoa = true, NguoiTao = "ssoftvn", NgayTao = _now, HienThi_Chinh = true, HienThi_Phu = true, HienThi_BanThe = true, TenNhomHangHoa_KhongDau = "Nhom hang hoa mac dinh", TenNhomHangHoa_KyTuDau = "Nhhmd"},
        //                        new DM_NhomHangHoa { ID = new Guid("00000000-0000-0000-0000-000000000001"), MaNhomHangHoa = "DVMD00001", TenNhomHangHoa = "Nhóm dịch vụ mặc định", LaNhomHangHoa = false, NguoiTao = "ssoftvn", NgayTao = _now, HienThi_Chinh = true, HienThi_Phu = true, HienThi_BanThe = true, TenNhomHangHoa_KhongDau = "Nhom dich vu mac dinh", TenNhomHangHoa_KyTuDau = "Ndvmd"},
        //                    };

        //                    try
        //                    {
        //                        //lstDM_NhomHangHoa.ForEach(s => context.DM_NhomHangHoa.Add(s));
        //                        context.DM_NhomHangHoa.AddRange(lstDM_NhomHangHoa);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        CookieStore.WriteLog("Khong the tao nhom hang hoa mac dinh: " + ex.Message + ex.InnerException, str);
        //                    }
        //                    #endregion
        //                }
        //                try
        //                {
        //                    context.SaveChanges();
        //                }
        //                catch(Exception ex)
        //                {
        //                    CookieStore.WriteLog("Khởi tạo dữ liệu không thành công: " + ex.Message + ex.InnerException, str);
        //                }
        //            }
        //        }
        //    }
        //    base.Seed(context);
        //}
        #endregion
        protected override void Seed(SsoftvnContext context)
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            if(str == null || str.Trim() == "")
            {
                CookieStore.WriteLog("Không thể xác định subdomain.", str);
                return;
            }
            else
            {
                using (context = new SsoftvnContext(str))
                {
                    if (context != null && context.Database.Exists())
                    {
                        Model_banhang24vn.CuaHangDangKy objCheck_TenMien = Model_banhang24vn.M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == str.Trim().ToLower());
                        if (objCheck_TenMien != null)
                        {
                            List<SqlParameter> paramlist1 = new List<SqlParameter>();
                            paramlist1.Add(new SqlParameter("Subdomain", str));
                            context.Database.ExecuteSqlCommand("exec KhoiTaoDuLieuLanDau @Subdomain", paramlist1.ToArray());
                        }
                    }
                }
            }
        }
        protected static async void AddAll(string str, List<HT_Quyen> lstHTQuyen)
        {
            SystemDBContext systemDBContext = new SystemDBContext();
            using (SsoftvnContext db = systemDBContext.GetDBContextNonStatic(str))
            {
                Task taskHTQuyen = AddHTQuyen(db, lstHTQuyen, str);
                Task taskHTCauHinhPhanMem = AddHTCauHinhPhanMem(db, str);
                Task taskDMQuocGia = AddDMQuocGiaTinhThanhQuanHuyen(db, str);
                Task taskThueSuat = AddDMThueSuat(db, str);
                Task taskDMKho = AddDMKho(db, str);
                Task taskDMLoaiChungTu = AddDMLoaiChungTu(db, str);
                Task taskHTThongBao = AddHTThongBao(db, str);
                Task taskNSPhongBan = AddNSPhongBan(db, str);
                Task taskDMNganHang = AddDMNganHang(db, str);
                Task taskOptinForm = AddOptinForm(db, str);
                Task taskDMDoiTuong = AddDMDoiTuong(db, str);
                Task taskDMNhomHangHoa = AddDMNhomHangHoa(db, str);
                await Task.WhenAll(taskHTQuyen, taskHTCauHinhPhanMem, taskDMQuocGia, taskThueSuat, taskDMKho, taskDMLoaiChungTu, taskHTThongBao, taskNSPhongBan, taskDMNganHang, taskOptinForm, taskDMDoiTuong, taskDMNhomHangHoa);
            }
        }

        protected async static Task<string> AddHTQuyen(SsoftvnContext context, List<HT_Quyen> lstHTQuyen, string str)
        {
            try
            {
                if (lstHTQuyen != null)
                {
                    context.HT_Quyen.AddRange(lstHTQuyen);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh muc quyen: " + ex.Message + ex.InnerException, str);
                string strErr1 = ex.Message;
            }

            try
            {
                if (lstHTQuyen != null)
                {
                    foreach (HT_Quyen item in lstHTQuyen)
                    {
                        HT_Quyen_Nhom itemQ_N = new HT_Quyen_Nhom();
                        itemQ_N.ID = Guid.NewGuid();
                        itemQ_N.ID_NhomNguoiDung = new Guid("EE609285-F6A6-43D8-A517-BDA52F426AE5");
                        itemQ_N.MaQuyen = item.MaQuyen;
                        //
                        context.HT_Quyen_Nhom.Add(itemQ_N);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao quyen cho nhom nguoi dung: " + ex.Message + ex.InnerException, str);
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddHTCauHinhPhanMem(SsoftvnContext context, string str)
        {
            try
            {
                context.HT_CauHinhPhanMem.Add(new HT_CauHinhPhanMem
                {
                    ID = Guid.NewGuid(),
                    ID_DonVi = new Guid("D93B17EA-89B9-4ECF-B242-D03B8CDE71DE"),
                    GiaVonTrungBinh = true,
                    CoDonViTinh = true,
                    DatHang = true,
                    XuatAm = false,
                    DatHangXuatAm = false,
                    ThayDoiThoiGianBanHang = false,
                    SoLuongTrenChungTu = false,
                    TinhNangTichDiem = false,
                    GioiHanThoiGianTraHang = false,
                    SanPhamCoThuocTinh = true,
                    BanVaChuyenKhiHangDaDat = false,
                    TinhNangSanXuatHangHoa = false,
                    SuDungCanDienTu = false,
                    KhoaSo = true,
                    InBaoGiaKhiBanHang = false,
                    QuanLyKhachHangTheoDonVi = false,
                    KhuyenMai = true,
                    LoHang = false,
                });
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao cau hinh phan mem: " + ex.Message + ex.InnerException, str);
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddDMQuocGiaTinhThanhQuanHuyen(SsoftvnContext context, string str)
        {
            try
            {
                var dmQuocGias = QuocGiaInitializer.lstQGiaInitializer;
                //   var dmQuocGias = QuocGiaInitializer.GetQuocGiasKT();
                dmQuocGias.ForEach(s => context.DM_QuocGia.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh muc quoc gia: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }

            try
            {
                var dmVungs = TinhTPInitializer.GetVungMiensKT();
                dmVungs.ForEach(s => context.DM_VungMien.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh muc vung mien: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }

            try
            {
                var dmTinhtps = TinhTPInitializer.GetTinhThanhsKT();
                dmTinhtps.ForEach(s => context.DM_TinhThanh.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh muc tinh thanh: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }

            try
            {
                var dmQHuyens = TinhTPInitializer.GetQuanHuyensKT();
                dmQHuyens.ForEach(s => context.DM_QuanHuyen.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh muc quan huyen: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }

            DM_TienTe objTienTe = new DM_TienTe();
            objTienTe.ID = new Guid("406eed2d-faae-4520-aef2-12912f83dda2");
            objTienTe.ID_QuocGia = QuocGiaInitializer.idQGia_VNi;
            objTienTe.LaNoiTe = true;
            objTienTe.MaNgoaiTe = "VND";
            objTienTe.NgayTao = DateTime.Now;
            objTienTe.NguoiTao = "ssoftvn";
            objTienTe.TenNgoaiTe = "Việt Nam đồng";
            try
            {
                context.DM_TienTe.Add(objTienTe);
                context.SaveChanges();
                //
                DM_TyGia objTygia = new DM_TyGia();
                objTygia.ID = Guid.NewGuid();
                objTygia.ID_TienTe = objTienTe.ID;
                objTygia.NgayTao = DateTime.Now;
                objTygia.NgayTyGia = DateTime.Now;
                objTygia.NguoiTao = "ssoftvn";
                objTygia.TyGia = 1;
                //
                context.DM_TyGia.Add(objTygia);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh muc tien te: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddDMThueSuat(SsoftvnContext context, string str)
        {
            try
            {
                var lstVats = new List<DM_ThueSuat>()
                                {
                                    new DM_ThueSuat{ID= Guid.NewGuid (), MaThueSuat="0%",ThueSuat=0, GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
                                    new DM_ThueSuat{ID= Guid.NewGuid (), MaThueSuat="5%",ThueSuat=5, GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"},
                                    new DM_ThueSuat{ID= Guid.NewGuid (), MaThueSuat="10%",ThueSuat=10, GhiChu="",NgayTao=DateTime .Now ,NguoiTao="ssoftvn"}
                                };
                lstVats.ForEach(s => context.DM_ThueSuat.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh muc thue suat: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddDMKho(SsoftvnContext context, string str)
        {
            Guid idKhoTong = new Guid("01CD02F2-4612-4104-B790-1C0373CBD72D");
            try
            {
                DM_Kho objKho = new Model.DM_Kho();
                objKho.ID = idKhoTong;
                objKho.MaKho = "KHO_CTy";
                objKho.NgayTao = DateTime.Now;
                objKho.NguoiTao = "ssoftvn";
                objKho.TenKho = "Kho tổng";

                context.DM_Kho.Add(objKho);
                context.SaveChanges();
                
                Kho_DonVi objKho_DVi = new Model.Kho_DonVi();
                objKho_DVi.ID = Guid.NewGuid();
                objKho_DVi.ID_DonVi = new Guid("D93B17EA-89B9-4ECF-B242-D03B8CDE71DE");
                objKho_DVi.ID_Kho = idKhoTong;
                //
                context.Kho_DonVi.Add(objKho_DVi);
                context.SaveChanges();
                    //
                
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao kho: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddDMLoaiChungTu(SsoftvnContext context, string str)
        {
            List<DM_LoaiChungTu> listloaichungtu = new List<DM_LoaiChungTu>()
                        {
                            new DM_LoaiChungTu{ ID = 1, MaLoaiChungTu = "HDBL", TenLoaiChungTu = "Hóa đơn bán lẻ", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ ID = 2, MaLoaiChungTu = "HDB", TenLoaiChungTu = "Hóa đơn bán", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 3, MaLoaiChungTu = "BG", TenLoaiChungTu = "Báo giá", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 4, MaLoaiChungTu = "PNK", TenLoaiChungTu = "Phiếu nhập kho", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 5, MaLoaiChungTu = "PXK", TenLoaiChungTu = "Phiếu xuất kho", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 6, MaLoaiChungTu = "TH", TenLoaiChungTu = "Trả hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 7, MaLoaiChungTu = "THNCC", TenLoaiChungTu = "Trả hàng nhà cung cấp", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 8, MaLoaiChungTu = "XH", TenLoaiChungTu = "Xuất kho", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 9, MaLoaiChungTu = "PKK", TenLoaiChungTu = "Phiếu kiểm kê", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 10, MaLoaiChungTu = "CH", TenLoaiChungTu = "Chuyển hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 11, MaLoaiChungTu = "SQPT", TenLoaiChungTu = "Phiếu thu", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 12, MaLoaiChungTu = "SQPC", TenLoaiChungTu = "Phiếu chi", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 13, MaLoaiChungTu = "NH", TenLoaiChungTu = "Nhân hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 14, MaLoaiChungTu = "DH", TenLoaiChungTu = "Đặt hàng", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 15, MaLoaiChungTu = "CB", TenLoaiChungTu = "Điều chỉnh", GhiChu = "Điều chỉnh công nợ Khách hàng, Nhà CC", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 16, MaLoaiChungTu = "KTGV", TenLoaiChungTu = "Khởi tạo giá vốn", GhiChu = "Khởi tạo giá vốn khi tạo hàng hóa", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 17, MaLoaiChungTu = "DTH", TenLoaiChungTu = "Đổi trả hàng", GhiChu = "Đổi trả hàng hóa", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 18, MaLoaiChungTu = "DCGV", TenLoaiChungTu = "Điều chỉnh giá vốn", GhiChu = "Điều chỉnh giá vốn", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 19, MaLoaiChungTu = "GDV", TenLoaiChungTu = "Gói dịch vụ", GhiChu = "Bán gói dịch vụ", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 20, MaLoaiChungTu = "TGDV", TenLoaiChungTu = "Trả gói dịch vụ", GhiChu = "Trả gói dịch vụ", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 21, MaLoaiChungTu = "IMV", TenLoaiChungTu = "Tem - Mã vạch", GhiChu = "Tem - Mã vạch", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 22, MaLoaiChungTu = "TGT", TenLoaiChungTu = "Thẻ giá trị", GhiChu = "Bán, nạp thẻ giá trị", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_LoaiChungTu{ID = 23, MaLoaiChungTu = "DCGT", TenLoaiChungTu = "Điều chỉnh thẻ giá trị", GhiChu = "Điều chỉnh giá trị thẻ giá trị", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                        };
            try
            {
                listloaichungtu.ForEach(s => context.DM_LoaiChungTu.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao loai chung tu: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddHTThongBao(SsoftvnContext context, string str)
        {
            try
            {
                HT_ThongBao_CaiDat objhtthongbaocaidat = new HT_ThongBao_CaiDat();
                objhtthongbaocaidat.ID = Guid.NewGuid();
                objhtthongbaocaidat.ID_NguoiDung = new Guid("28FEF5A1-F0F2-4B94-A4AD-081B227F3B77");
                objhtthongbaocaidat.NhacSinhNhat = true;
                objhtthongbaocaidat.NhacCongNo = true;
                objhtthongbaocaidat.NhacTonKho = true;
                objhtthongbaocaidat.NhacDieuChuyen = true;
                objhtthongbaocaidat.NhacLoHang = true;
                context.HT_ThongBao_CaiDat.Add(objhtthongbaocaidat);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao cai dat thong bao: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddNSPhongBan(SsoftvnContext context, string str)
        {
            try
            {
                NS_PhongBan nsphongban = new NS_PhongBan();
                nsphongban.ID = new Guid("6DE963A7-50AF-4E51-91E8-E242D7E7B476");
                nsphongban.MaPhongBan = "PB0000";
                nsphongban.TenPhongBan = "Phòng ban mặc định";
                nsphongban.TrangThai = 1;
                context.NS_PhongBan.Add(nsphongban);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao nhan su phong ban: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddDMNganHang(SsoftvnContext context, string str)
        {
            List<DM_NganHang> lstNganHang = new List<DM_NganHang>()
                        {
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "ABC", TenNganHang = "Ngân hàng Á Châu (ACB)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "TPBANK", TenNganHang = "Ngân hàng Tiên Phong (TPBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "DAF", TenNganHang = "Ngân hàng Đông Á (DAF)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SEABANK", TenNganHang = "Ngân hàng Đông Nam Á (SeABank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "ABBANK", TenNganHang = "Ngân hàng An Bình (ABBANK)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "BACABANK", TenNganHang = "Ngân hàng Bắc Á (BacABank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VIETCAPITALBANK", TenNganHang = "Ngân hàng Bản Việt (VietCapitalBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "MSB", TenNganHang = "Ngân hàng Hàng Hải Việt Nam (MaritimeBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "TECHCOMBANK", TenNganHang = "Ngân hàng Kỹ thương Việt Nam (Techcombank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "KIENLONGBANK", TenNganHang = "Ngân hàng Kiên Long (KienLongBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "NAMABANK", TenNganHang = "Ngân hàng Nam Á (Nam A Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "NCB", TenNganHang = "Ngân hàng Quốc Dân (National Citizen Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VPBANK", TenNganHang = "Ngân hàng Việt Nam Thịnh Vượng (VPBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "HDBANK", TenNganHang = "Ngân hàng Phát triển nhà Thành Phố Hồ Chí Minh (HDBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "OCB", TenNganHang = "Ngân hàng Phương Đông (Orient Commercial Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "MBB", TenNganHang = "Ngân hàng Quân đội (Military Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "PVCOMBANK", TenNganHang = "Ngân hàng Đại chúng (PVcom Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VIB", TenNganHang = "Ngân hàng Quốc tế (VIBBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SCB", TenNganHang = "Ngân hàng Sài Gòn (Sài Gòn)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SGB", TenNganHang = "Ngân hàng Sài Gòn Công Thương (Saigonbank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "SHB", TenNganHang = "Ngân hàng Sài Gòn - Hà Nội (SHBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "STB", TenNganHang = "Ngân hàng Sài Gòn Thương Tín (Sacombank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VAB", TenNganHang = "Ngân hàng Việt Á (VietABank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "BVB", TenNganHang = "Ngân hàng Bảo Việt (BaoVietBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VIETBANK", TenNganHang = "Ngân hàng Việt Nam Thương Tín (VietBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "PGBANK", TenNganHang = "Ngân hàng Xăng dầu Petrolimex (Petrolimex Group Bank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "EIB", TenNganHang = "Ngân hàng Xuất Nhập khẩu Việt Nam (Eximbank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "LPB", TenNganHang = "Ngân hàng Bưu điện Liên Việt (LienVietPostBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VCB", TenNganHang = "Ngân hàng Ngoại thương Việt Nam (Vietcombank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "CTG", TenNganHang = "Ngân hàng Công thương Việt Nam (VietinBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "BID", TenNganHang = "Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "NHCSXH/VBSP", TenNganHang = "Ngân hàng Chính sách xã hội (NHCSXH/VBSP)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "VDB", TenNganHang = "Ngân hàng Phát triển Việt Nam (VDB)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "CB", TenNganHang = "Ngân hàng Xây dựng (CB)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "GPBANK", TenNganHang = "Ngân hàng Dầu khí Toàn Cầu (GPBank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "OCEANBANK", TenNganHang = "Ngân hàng Đại Dương (Oceanbank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                            new DM_NganHang { ID = Guid.NewGuid(), MaNganHang = "AGRIBANK", TenNganHang = "Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam (Agribank)", ChiNhanh = "", GhiChu = "", NguoiTao = "ssoftvn", NgayTao = DateTime.Now, ChiPhiThanhToan = 0, TheoPhanTram = false, MacDinh = false, ThuPhiThanhToan = false},
                        };

            try
            {
                lstNganHang.ForEach(s => context.DM_NganHang.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao danh sach ngan hang: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddOptinForm(SsoftvnContext context, string str)
        {
            List<OptinForm_TruongThongTin> lstOFTruongThongTin = new List<OptinForm_TruongThongTin>()
                        {
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Ảnh đại diện", TrangThai = 1, STT = 1, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Tên khách hàng", TrangThai = 1, STT = 2, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Giới tính", TrangThai = 1, STT = 3, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Ngày sinh", TrangThai = 1, STT = 4, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Số điện thoại", TrangThai = 1, STT = 5, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Email", TrangThai = 1, STT = 6, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Địa chỉ", TrangThai = 1, STT = 7, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Tỉnh thành", TrangThai = 1, STT = 8, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Quận huyện", TrangThai = 1, STT = 9, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Mã số thuế", TrangThai = 1, STT = 10, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Khách hàng lẻ", TrangThai = 1, STT = 11, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Người giới thiệu", TrangThai = 1, STT = 12, LoaiTruongThongTin = 1},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Chọn chi nhánh", TrangThai = 1, STT = 1, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Xưng hô", TrangThai = 1, STT = 2, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Khách hàng", TrangThai = 1, STT = 3, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Bạn đi theo nhóm", TrangThai = 1, STT = 4, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Số lượng", TrangThai = 1, STT = 5, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Số điện thoại", TrangThai = 1, STT = 6, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Email", TrangThai = 1, STT = 7, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Dịch vụ yêu cầu", TrangThai = 1, STT = 8, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Thời gian", TrangThai = 1, STT = 9, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Nhân viên thực hiện", TrangThai = 1, STT = 10, LoaiTruongThongTin = 2},
                            new OptinForm_TruongThongTin { ID = Guid.NewGuid(), TenTruongThongTin = "Ghi chú", TrangThai = 1, STT = 11, LoaiTruongThongTin = 2},
                        };

            try
            {
                lstOFTruongThongTin.ForEach(s => context.OptinForm_TruongThongTin.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao optinform truong thong tin: " + ex.Message + ex.InnerException, str);
                //strErr = ex.Message;
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddDMDoiTuong(SsoftvnContext context, string str)
        {
            List<DM_DoiTuong> lstDM_DoiTuong = new List<DM_DoiTuong>()
                        {
                            new DM_DoiTuong {ID = new Guid("00000000-0000-0000-0000-000000000000"), LoaiDoiTuong = 1, LaCaNhan = true, MaDoiTuong = "KL00001", TenDoiTuong = "Khách lẻ", TenDoiTuong_KhongDau = "Khach le", TenDoiTuong_ChuCaiDau = "Kl", ChiaSe = false, TheoDoi = false, ID_DonVi = new Guid("D93B17EA-89B9-4ECF-B242-D03B8CDE71DE"), TenNhomDoiTuongs = "Nhóm mặc định", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                            new DM_DoiTuong {ID = new Guid("00000000-0000-0000-0000-000000000002"), LoaiDoiTuong = 2, LaCaNhan = true, MaDoiTuong = "NCCL001", TenDoiTuong = "Nhà cung cấp lẻ", TenDoiTuong_KhongDau = "Nha cung cap le", TenDoiTuong_ChuCaiDau = "nccl", ChiaSe = false, TheoDoi = false, ID_DonVi = new Guid("D93B17EA-89B9-4ECF-B242-D03B8CDE71DE"), TenNhomDoiTuongs = "Nhóm mặc định", NguoiTao = "ssoftvn", NgayTao = DateTime.Now},
                        };

            try
            {
                lstDM_DoiTuong.ForEach(s => context.DM_DoiTuong.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao doi tuong mac dinh: " + ex.Message + ex.InnerException, str);
            }
            return await Task.FromResult("");
        }

        protected async static Task<string> AddDMNhomHangHoa(SsoftvnContext context, string str)
        {
            List<DM_NhomHangHoa> lstDM_NhomHangHoa = new List<DM_NhomHangHoa>()
                        {
                            new DM_NhomHangHoa {ID = new Guid("00000000-0000-0000-0000-000000000000"), MaNhomHangHoa = "NHMD00001", TenNhomHangHoa = "Nhóm hàng hóa mặc định", LaNhomHangHoa = true, NguoiTao = "ssoftvn", NgayTao = DateTime.Now, HienThi_Chinh = true, HienThi_Phu = true, HienThi_BanThe = true, TenNhomHangHoa_KhongDau = "Nhom hang hoa mac dinh", TenNhomHangHoa_KyTuDau = "Nhhmd"},
                            new DM_NhomHangHoa { ID = new Guid("00000000-0000-0000-0000-000000000001"), MaNhomHangHoa = "DVMD00001", TenNhomHangHoa = "Nhóm dịch vụ mặc định", LaNhomHangHoa = false, NguoiTao = "ssoftvn", NgayTao = DateTime.Now, HienThi_Chinh = true, HienThi_Phu = true, HienThi_BanThe = true, TenNhomHangHoa_KhongDau = "Nhom dich vu mac dinh", TenNhomHangHoa_KyTuDau = "Ndvmd"},
                        };

            try
            {
                lstDM_NhomHangHoa.ForEach(s => context.DM_NhomHangHoa.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Khong the tao nhom hang hoa mac dinh: " + ex.Message + ex.InnerException, str);
            }
            return await Task.FromResult("");
        }
    }
}
