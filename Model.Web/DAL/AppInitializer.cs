using Model.Web.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ssoft.Common.Common;

namespace Model.Web.DAL
{
    public class AppInitializer : System.Data.Entity.CreateDatabaseIfNotExists<SsoftvnWebContext>
    {
        protected override void Seed(SsoftvnWebContext context)
        {
            string str = CookieStore.GetCookieAes(SqlConnection.subdoamin);
            string strErr = string.Empty;
            if (str == null || str.Trim() == "")
            {
                return;
            }
            else
            {
                context = SystemDBContext.GetDBContext(str);

                if (context != null && context.Database.Exists())
                {
                    HT_ThongTinCuaHang webCuaHang = new HT_ThongTinCuaHang();
                    Model_banhang24vn.CuaHangDangKy cuaHang = Model_banhang24vn.M_DangKySuDung.Get(p => p.SubDomainWeb == str);
                    if(cuaHang != null)
                    {
                        
                        webCuaHang.ID = Guid.NewGuid();
                        webCuaHang.DiaChi = cuaHang.DiaChi;
                        webCuaHang.Email = cuaHang.Email;
                        webCuaHang.PageKhachHang = 6;
                        webCuaHang.PageKhachHangHome = 6;
                        webCuaHang.PageSanPhamHome = 6;
                        webCuaHang.PageTinTuc = 6;
                        webCuaHang.PageTinTucHome = 6;
                        webCuaHang.SoDienThoai = cuaHang.SoDienThoai;
                        webCuaHang.TenCuaHang = cuaHang.TenCuaHang;
                        webCuaHang.TenKhachHang = cuaHang.HoTen;
                        webCuaHang.Theme = 0;
                        try
                        {
                            context.HT_ThongTinCuaHang.Add(webCuaHang);
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            strErr += ex.Message;
                        }
                    }

                    HT_NhomNguoiDung htNhomNguoiDung = new HT_NhomNguoiDung();
                    htNhomNguoiDung.ID = Guid.NewGuid();
                    htNhomNguoiDung.NguoiTao = "Ssoftvn";
                    htNhomNguoiDung.NgayTao = DateTime.Now;
                    htNhomNguoiDung.TenNhomNguoiDung = "Admin";
                    htNhomNguoiDung.TrangThai = true;
                    htNhomNguoiDung.GhiChu = "Nhóm khởi tạo";
                    try
                    {
                        context.HT_NhomNguoiDung.Add(htNhomNguoiDung);
                        context.SaveChanges();
                        HT_NguoiDung htNguoiDung = new HT_NguoiDung();
                        htNguoiDung.ID = Guid.NewGuid();
                        htNguoiDung.TaiKhoan = "Admin";
                        htNguoiDung.MatKhau = "ICy5YqxZB1uWSwcVLSNLcA==";
                        htNguoiDung.ID_NhomNguoiDung = htNhomNguoiDung.ID;
                        htNguoiDung.NgayTao = DateTime.Now;
                        htNguoiDung.NguoiTao = "Ssoftvn";
                        htNguoiDung.DiaChi = webCuaHang.DiaChi;
                        htNguoiDung.DienThoai = webCuaHang.SoDienThoai;
                        htNguoiDung.Email = webCuaHang.Email;
                        htNguoiDung.LaAdmin = true;
                        htNguoiDung.TenNguoiDung = webCuaHang.TenKhachHang;
                        context.HT_NguoiDung.Add(htNguoiDung);
                        context.SaveChanges();
                        
                        for(int i = 0; i < 2;i++)
                        {
                            DM_NhomBaiViet NhomBai = new DM_NhomBaiViet();
                            NhomBai.TenNhomBaiViet = i==0?"Tin tức":"Tuyển dụng";
                            NhomBai.LoaiNhomBaiViet = i == 0 ?(int)LibEnum.StatusGroupNews.tintuc : (int)LibEnum.StatusGroupNews.tuyendung;
                            NhomBai.NgayTao = DateTime.Now;
                            NhomBai.NguoiTao = "Admin";
                            NhomBai.TrangThai = true;
                            context.DM_NhomBaiViet.Add(NhomBai);
                            
                        }
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        strErr += ex.Message;
                    }
                }
            }
        }
    }
}
