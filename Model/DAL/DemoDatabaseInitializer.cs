using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL
{
    public class DemoDatabaseInitializer
    {
        public static string InitializeDatabase(SsoftvnContext db, Guid ID_NganhKinhDoanh)
        {
            DM_DonVi objDvi = db.DM_DonVi.FirstOrDefault();
            if (objDvi != null)
            {
                try
                {
                    Guid idDonVi = objDvi.ID;

                    Model_banhang24vn.BanHang24vnContext dbMacDinh = new Model_banhang24vn.BanHang24vnContext();
                    #region du lieu mau

                    #region nhóm khách hàng / Khách hàng - ncc
                    db.NhomDoiTuong_DonVi.RemoveRange(db.NhomDoiTuong_DonVi);
                    db.DM_DoiTuong.RemoveRange(db.DM_DoiTuong);
                    db.DM_NhomDoiTuong.RemoveRange(db.DM_NhomDoiTuong);
                    db.SaveChanges();

                    DM_NhomDoiTuong objNhomKH = new DM_NhomDoiTuong();
                    objNhomKH.GhiChu = "";
                    objNhomKH.ID = Guid.NewGuid();
                    objNhomKH.LoaiDoiTuong = 2; //KH_NCC_KHNCC= 0_1_2
                    objNhomKH.MaNhomDoiTuong = "KH_NCC";
                    objNhomKH.NgayTao = DateTime.Now;
                    objNhomKH.NguoiTao = "ssoftvn";
                    objNhomKH.TenNhomDoiTuong = "Khách hàng - nhà cung cấp";
                    try
                    {
                        db.DM_NhomDoiTuong.Add(objNhomKH);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string strErr1 = ex.Message;
                        return strErr1;
                    }

                    NhomDoiTuong_DonVi objNhomKH_DVi = new NhomDoiTuong_DonVi();
                    objNhomKH_DVi.ID = Guid.NewGuid();
                    objNhomKH_DVi.ID_NhomDoiTuong = objNhomKH.ID;
                    objNhomKH_DVi.ID_DonVi = idDonVi;
                    try
                    {
                        db.NhomDoiTuong_DonVi.Add(objNhomKH_DVi);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string strErr1 = ex.Message;
                        return strErr1;
                    }

                    DM_DoiTuong objKhachHang = new DM_DoiTuong();
                    objKhachHang.ChiaSe = true;
                    objKhachHang.DiemKhoiTao = 0;
                    objKhachHang.DoanhSoKhoiTao = 0;
                    objKhachHang.GioiTinhNam = true;
                    objKhachHang.ID = Guid.NewGuid();
                    objKhachHang.ID_DonVi = idDonVi;
                    objKhachHang.ID_NhomDoiTuong = objNhomKH.ID;
                    objKhachHang.LaCaNhan = true;
                    objKhachHang.LoaiDoiTuong = 2;
                    objKhachHang.MaDoiTuong = "KH_NCC_SSOFTVN";
                    objKhachHang.NgayTao = DateTime.Now;
                    objKhachHang.NguoiTao = "ssoftvn";
                    objKhachHang.TenDoiTuong = "ssoftvn";
                    objKhachHang.TenKhac = "ssoftvn";
                    objKhachHang.TheoDoi = true;
                    objKhachHang.TheoDoiVanTay = false;
                    objKhachHang.XungHo = "anh";
                    try
                    {
                        db.DM_DoiTuong.Add(objKhachHang);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string strErr1 = ex.Message;
                        return strErr1;
                    }
                    #endregion

                    #region HangHoa - dịch vụ
                    Guid idKho_MD = Guid.NewGuid();
                    //
                    #region kho
                    db.Kho_DonVi.RemoveRange(db.Kho_DonVi);
                    db.DM_Kho.RemoveRange(db.DM_Kho);
                    db.SaveChanges();
                    //
                    Model.DM_Kho objKho = new Model.DM_Kho();
                    objKho.ID = idKho_MD;
                    objKho.MaKho = "KT";
                    objKho.NgayTao = DateTime.Now;
                    objKho.NguoiTao = "ssoftvn";
                    objKho.TenKho = "Kho tổng";
                    try
                    {
                        db.DM_Kho.Add(objKho);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string strErr1 = ex.Message;
                        return strErr1;
                    }
                    //
                    Model.Kho_DonVi objKho_DVi = new Kho_DonVi();
                    objKho_DVi.ID = Guid.NewGuid();
                    objKho_DVi.ID_DonVi = idDonVi;
                    objKho_DVi.ID_Kho = idKho_MD;
                    try
                    {
                        db.Kho_DonVi.Add(objKho_DVi);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string strErr1 = ex.Message;
                        return strErr1;
                    }
                    #endregion
                    //
                    List<Guid> lstIDNhomHHs = new List<Guid>();
                    //
                    #region DM_NhomHangHoa
                    //
                    db.DonViQuiDois.RemoveRange(db.DonViQuiDois);
                    db.DM_HangHoa.RemoveRange(db.DM_HangHoa);
                    db.NhomHangHoa_DonVi.RemoveRange(db.NhomHangHoa_DonVi);
                    db.DM_NhomHangHoa.RemoveRange(db.DM_NhomHangHoa);
                    //
                    try
                    {
                        List<Model_banhang24vn.DM_NhomHangHoa> lstNhomHHs = dbMacDinh.NhomHangHoa_NganhNgheKinhDoanh.Where(p => p.ID_NganhNgheKinhDoanh == ID_NganhKinhDoanh).Select(p => p.DM_NhomHangHoa).Distinct().ToList();
                        if (lstNhomHHs != null && lstNhomHHs.Count > 0)
                        {
                            foreach (Model_banhang24vn.DM_NhomHangHoa objNhom_Mau in lstNhomHHs)
                            {
                                DM_NhomHangHoa objNhomHH = new DM_NhomHangHoa();
                                objNhomHH.GhiChu = objNhom_Mau.GhiChu;
                                objNhomHH.HienThi_BanThe = true;
                                objNhomHH.HienThi_Chinh = true;
                                objNhomHH.HienThi_Phu = true;
                                objNhomHH.ID = objNhom_Mau.ID;
                                objNhomHH.ID_Kho = idKho_MD;
                                objNhomHH.ID_Parent = objNhom_Mau.ID_Parent;
                                objNhomHH.LaNhomHangHoa = objNhom_Mau.LaNhomHangHoa;
                                objNhomHH.MaNhomHangHoa = objNhom_Mau.MaNhomHangHoa;
                                objNhomHH.MauHienThi = objNhom_Mau.MauHienThi;
                                objNhomHH.MayIn = null;
                                objNhomHH.NgayTao = DateTime.Now;
                                objNhomHH.NguoiTao = "ssoftvn";
                                objNhomHH.TenNhomHangHoa = objNhom_Mau.TenNhomHangHoa;
                                //
                                db.DM_NhomHangHoa.Add(objNhomHH);
                                //
                                NhomHangHoa_DonVi objNhomHH_DVi = new NhomHangHoa_DonVi();
                                objNhomHH_DVi.ID = Guid.NewGuid();
                                objNhomHH_DVi.ID_DonVi = idDonVi;
                                objNhomHH_DVi.ID_NhomHangHoa = objNhomHH.ID;
                                //
                                db.NhomHangHoa_DonVi.Add(objNhomHH_DVi);
                                //
                                lstIDNhomHHs.Add(objNhom_Mau.ID);
                            }
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        string strErr1 = ex.Message;
                        return strErr1;
                    }
                    #endregion

                    #region DM_HangHoa
                    if (lstIDNhomHHs.Count > 0)
                    {
                        try
                        {
                            List<Model_banhang24vn.DM_HangHoa> lstHangHoas = dbMacDinh.DM_HangHoa.Where(p => p.ID_NhomHang != null && lstIDNhomHHs.Contains(p.ID_NhomHang.Value)).ToList();
                            if (lstHangHoas != null && lstHangHoas.Count > 0)
                            {
                                foreach (Model_banhang24vn.DM_HangHoa objHHDV_Mau in lstHangHoas)
                                {
                                    Model.DM_HangHoa objHHNew = new Model.DM_HangHoa();
                                    objHHNew.ChietKhauMD_NV = objHHDV_Mau.ChietKhauMD_NV;
                                    objHHNew.ChietKhauMD_NVTheoPT = objHHDV_Mau.ChietKhauMD_NVTheoPT;
                                    objHHNew.ChiPhiThucHien = objHHDV_Mau.ChiPhiThucHien;
                                    objHHNew.ChiPhiTinhTheoPT = objHHDV_Mau.ChiPhiTinhTheoPT;
                                    objHHNew.ID = objHHDV_Mau.ID;
                                    objHHNew.ID_DoiTuong = objKhachHang.ID;
                                    objHHNew.ID_NhomHang = objHHDV_Mau.ID_NhomHang;
                                    //objHHNew.ID_PhanLoai = objHHDV_Mau.ID_PhanLoai;
                                    //objHHNew.ID_QuocGia = objHHDV_Mau.ID_QuocGia;
                                    //objHHNew.ID_ThueSuatBan = objHHDV_Mau.ID_ThueSuatBan;
                                    //objHHNew.ID_ThueSuatNhap = objHHDV_Mau.ID_ThueSuatNhap;
                                    objHHNew.LaHangHoa = objHHDV_Mau.LaHangHoa;
                                    objHHNew.LoaiBaoHanh = objHHDV_Mau.LoaiBaoHanh;
                                    objHHNew.NgayTao = DateTime.Now;
                                    objHHNew.NguoiTao = "ssoftvn";
                                    //objHHNew.QuyCach 
                                    objHHNew.SoPhutThucHien = objHHDV_Mau.SoPhutThucHien;
                                    objHHNew.TenHangHoa = objHHDV_Mau.TenHangHoa;
                                    objHHNew.TenKhac = objHHDV_Mau.TenKhac;
                                    objHHNew.TenTGBaoHanh = objHHDV_Mau.TenTGBaoHanh;
                                    objHHNew.TheoDoi = objHHDV_Mau.TheoDoi;
                                    objHHNew.ThoiGianBaoHanh = objHHDV_Mau.ThoiGianBaoHanh;
                                    objHHNew.TinhCPSauChietKhau = objHHDV_Mau.TinhCPSauChietKhau;
                                    objHHNew.TinhGiaVon = objHHDV_Mau.TinhGiaVon;
                                    //
                                    List<Model_banhang24vn.DonViQuiDoi> lstDVTs_HH_Mau = dbMacDinh.DonViQuiDois.Where(p => p.ID_HangHoa == objHHDV_Mau.ID).ToList();
                                    if (lstDVTs_HH_Mau != null && lstDVTs_HH_Mau.Count > 0)
                                    {
                                        foreach (Model_banhang24vn.DonViQuiDoi objDVT_Mau in lstDVTs_HH_Mau)
                                        {
                                            DonViQuiDoi objDVT = new DonViQuiDoi();
                                            objDVT.GiaBan = objDVT_Mau.GiaBan;
                                            objDVT.GiaVon = objDVT_Mau.GiaNhap;
                                            objDVT.ID_HangHoa = objDVT_Mau.ID_HangHoa;
                                            objDVT.MaHangHoa = objDVT_Mau.MaHangHoa;
                                            objDVT.LaDonViChuan = objDVT_Mau.LaDonViChuan;
                                            objDVT.NgayTao = DateTime.Now;
                                            objDVT.NguoiTao = "ssoftvn";
                                            objDVT.TenDonViTinh = objDVT_Mau.TenDonViTinh;
                                            objDVT.TyLeChuyenDoi = objDVT_Mau.TyLeChuyenDoi;
                                            //
                                            objHHNew.DonViQuiDois.Add(objDVT);
                                        }
                                    }
                                    //
                                    db.DM_HangHoa.Add(objHHNew);
                                }
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            string strErr1 = ex.Message;
                            return strErr1;
                        }
                    }
                    #endregion
                    #endregion

                    #endregion

                    return "";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                return "Không tìm thấy thông tin đơn vị sử dụng dữ liệu nhập";
            }
        }
    }
}
