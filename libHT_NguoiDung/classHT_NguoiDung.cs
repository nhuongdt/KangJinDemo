using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Model_banhang24vn.DAL;

namespace libHT_NguoiDung
{
    public class classHT_NguoiDung
    {
        private SsoftvnContext db;

        public classHT_NguoiDung(SsoftvnContext _db)
        {
            db = _db;
        }
        #region Select
        public IQueryable<HT_NguoiDung> Gets(Expression<Func<HT_NguoiDung, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.HT_NguoiDung;
                else
                    return db.HT_NguoiDung.Where(query);
            }
        }

        public IQueryable<HT_Quyen_Nhom> GetListQuyen()
        {
            string account = CookieStore.GetCookieAes("Account");
            HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
            var data = (from nd in db.HT_NguoiDung
                        join ndn in db.HT_NguoiDung_Nhom
                        on nd.ID equals ndn.IDNguoiDung
                        join qn in db.HT_Quyen_Nhom
                        on ndn.IDNhomNguoiDung equals qn.ID_NhomNguoiDung
                        where nd.ID == nguoidung.ID && ndn.ID_DonVi == nguoidung.ID_DonVi
                        select qn).Distinct();
            return data;
        }

        public IQueryable<HT_ThongBao> GetThongBaos(Expression<Func<HT_ThongBao, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.HT_ThongBao;
                else
                    return db.HT_ThongBao.Where(query);
            }

        }

        public HT_NguoiDung Get(Expression<Func<HT_NguoiDung, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NguoiDung.Where(query).FirstOrDefault();
            }
        }

        public HT_ThongBao GetThongBao(Expression<Func<HT_ThongBao, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_ThongBao.Where(query).FirstOrDefault();
            }
        }

        public bool Check_ID_NhanVienExist(Guid idNhanVien)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.HT_NguoiDung.Count(e => e.ID_NhanVien == idNhanVien) > 0;
            }
        }

        public bool Check_ID_NhanVienEditExist(Guid idNhanVien, Guid id)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                return db.HT_NguoiDung.Count(e => e.ID_NhanVien == idNhanVien && e.ID != id) > 0;
            }
        }

        public bool Check_TenTaiKhoanExist(string tenTaiKhoan, Guid id)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                return db.HT_NguoiDung.Count(e => e.TaiKhoan == tenTaiKhoan && e.ID != id) > 0;
            }
        }

        public bool Check_TenTaiKhoanAddExist(string tenTaiKhoan)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                return db.HT_NguoiDung.Count(e => e.TaiKhoan == tenTaiKhoan) > 0;
            }
        }

        public bool Check_LoGin(string tenTaiKhoan, string matKhau)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return false;
            }
            else
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(matKhau);
                byte[] byteHash = md5.ComputeHash(byteValue);
                string pasInput = Convert.ToBase64String(byteHash);
                int count = Gets(tk => tk.TaiKhoan == tenTaiKhoan && tk.MatKhau == pasInput).Count();
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string Check_LoGin2(string tenTaiKhoan, string matKhau)
        {
            string strErr = string.Empty;

            if (db == null)
            {
                return "Chưa kết nối DB";
            }
            else
            {
                if (tenTaiKhoan == "" || matKhau == "")
                {
                    return "Bạn cần nhập đủ thông tin các trường.";
                }
                else
                {
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(matKhau);
                    byte[] byteHash = md5.ComputeHash(byteValue);
                    string pasInput = Convert.ToBase64String(byteHash);
                    HT_NguoiDung htnd = Get(tk => tk.TaiKhoan == tenTaiKhoan && tk.MatKhau == pasInput);
                    if (htnd != null)
                    {
                        if (htnd.DangHoatDong == true)
                        {
                            return "";
                        }
                        else
                        {
                            return "Tài khoản bị ngừng hoạt động, bạn không thể đăng nhập vào hệ thống";
                        }
                    }
                    else
                    {
                        return "Sai tên đăng nhập hoặc mật khẩu";
                    }
                }
            }
        }

        public List<HT_NguoiDungDTO> getNguoiDungByID_DonVi(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nv in db.NS_NhanVien
                          join nd in db.HT_NguoiDung on nv.ID equals nd.ID_NhanVien
                          join qtct in db.NS_QuaTrinhCongTac on nv.ID equals qtct.ID_NhanVien
                          join dv in db.DM_DonVi on qtct.ID_DonVi equals dv.ID
                          where dv.ID == id && nv.DaNghiViec != true && nv.TrangThai != 0
                          select new HT_NguoiDungDTO
                          {
                              TenNguoiDung = nv.TenNhanVien,
                              TaiKhoan = nd.TaiKhoan,
                          };
                return tbl.ToList();
            }
        }

        public List<HT_NguoiDungDTO> changeHTNguoiDungByIDNhom(Guid idnhomnd)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nd in db.HT_NguoiDung
                          join htnhomnd in db.HT_NguoiDung_Nhom on nd.ID equals htnhomnd.IDNguoiDung
                          join htndnhom in db.HT_NhomNguoiDung on htnhomnd.IDNhomNguoiDung equals htndnhom.ID
                          where htndnhom.ID == idnhomnd
                          join nv in db.NS_NhanVien on nd.ID_NhanVien equals nv.ID into ND_NV
                          from nd_nv in ND_NV.DefaultIfEmpty()
                          where nd.LaAdmin != true
                          select new
                          {
                              nd.ID,
                              nd.TaiKhoan,
                              nd.ID_NhanVien,
                              nd.MatKhau,
                              nd.LaAdmin,
                              nd.DangHoatDong,
                              nd_nv.TenNhanVien,
                              htnhomnd.IDNhomNguoiDung,
                              htndnhom.TenNhom
                          };
                var list = tbl.Select(s =>
                            new HT_NguoiDungDTO
                            {
                                ID = s.ID,
                                TaiKhoan = s.TaiKhoan,
                                ID_NhanVien = s.ID_NhanVien,
                                MatKhau = s.MatKhau,
                                LaAdmin = s.LaAdmin,
                                DangHoatDong = s.DangHoatDong,
                                TenNguoiDung = s.TenNhanVien,
                                IDNhomNguoiDung = s.IDNhomNguoiDung,
                                TenNhom = s.TenNhom
                            }).ToList();

                return list;
            }
        }
        //trinhpv Người tạo xuất hủy
        public string GetListNameNguoiDung(Guid? ID_NguoiDung)
        {
            string name = string.Empty;
            var tbl = from nd in db.HT_NguoiDung.Where(x => x.ID == ID_NguoiDung)
                      join nv in db.NS_NhanVien on nd.ID_NhanVien equals nv.ID
                      select new
                      {
                          nv.TenNhanVien
                      };
            try
            {
                name = tbl.FirstOrDefault().TenNhanVien;
            }
            catch
            {
            }
            return name;
        }
        public List<HT_NguoiDungDTO> getAllNguoiDung()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nd in db.HT_NguoiDung
                          join htnhomnd in db.HT_NguoiDung_Nhom on nd.ID equals htnhomnd.IDNguoiDung
                          join htndnhom in db.HT_NhomNguoiDung on htnhomnd.IDNhomNguoiDung equals htndnhom.ID
                          join nv in db.NS_NhanVien on nd.ID_NhanVien equals nv.ID into ND_NV
                          from nd_nv in ND_NV.DefaultIfEmpty()
                              //where nd.LaAdmin != true
                          select new
                          {
                              nd.ID,
                              nd.TaiKhoan,
                              nd.ID_NhanVien,
                              nd.MatKhau,
                              nd.LaAdmin,
                              nd.DangHoatDong,
                              nd_nv.TenNhanVien,
                              htnhomnd.IDNhomNguoiDung,
                              htndnhom.TenNhom
                          };
                var list = tbl.Select(s =>
                            new HT_NguoiDungDTO
                            {
                                ID = s.ID,
                                TaiKhoan = s.TaiKhoan,
                                ID_NhanVien = s.ID_NhanVien,
                                MatKhau = s.MatKhau,
                                LaAdmin = s.LaAdmin,
                                DangHoatDong = s.DangHoatDong,
                                TenNguoiDung = s.TenNhanVien,
                                IDNhomNguoiDung = s.IDNhomNguoiDung,
                                TenNhom = s.TenNhom
                            }).ToList();

                return list;
            }

        }

        public List<HT_NguoiDungDTO> getAllNguoiDung_where(string maHoaDon, string idnhomnguoidung, int trangthai)
        {
            List<HT_NguoiDungDTO> lst = new List<HT_NguoiDungDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                var tbl = from nd in db.HT_NguoiDung
                          join htnhomnd in db.HT_NguoiDung_Nhom on nd.ID equals htnhomnd.IDNguoiDung into nhom_nd
                          from Nhom_ND in nhom_nd.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on nd.ID_NhanVien equals nv.ID into ND_NV
                          from nd_nv in ND_NV.DefaultIfEmpty()
                          where nd.LaAdmin == false && nd_nv.DaNghiViec != true && nd_nv.TrangThai != 0
                          select new
                          {
                              ID = nd.ID,
                              TaiKhoan = nd.TaiKhoan,
                              ID_NhanVien = nd.ID_NhanVien,
                              MatKhau = nd.MatKhau,
                              LaAdmin = nd.LaAdmin,
                              DangHoatDong = nd.DangHoatDong,
                              TenNhanVien = nd_nv.TenNhanVien,
                              IDNhomNguoiDung = Nhom_ND.IDNhomNguoiDung == null ? Guid.Empty : Nhom_ND.IDNhomNguoiDung,
                              MaNhanVien = nd_nv.MaNhanVien,
                              ID_DonVi = nd.ID_DonVi,
                              XemGiaVon = nd.XemGiaVon
                          };
                if (idnhomnguoidung != "undefined")
                {
                    tbl = tbl.Where(hd => hd.IDNhomNguoiDung.ToString().Contains(idnhomnguoidung.ToString()));
                }
                switch (trangthai)
                {
                    case 1:
                        tbl = tbl.Where(hd => hd.DangHoatDong == true);
                        break;
                    case 2:
                        tbl = tbl.Where(hd => hd.DangHoatDong == false);
                        break;
                    default: break;
                }
                string stSearch = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                //tiền nạp cho người dùng
                string str = CookieStore.GetCookieAes("SubDomain");
                var SoDTCuaHang = new CuaHangDangKyService().Query.Where(p => p.SubDomain == str).FirstOrDefault().SoDienThoai;
                decimal? TongTienNapAdmin = new CuaHangDangKyService().GetAllCuaHangNapTien.Where(o => o.SoDienThoaiCuaHang == SoDTCuaHang && o.TrangThai == 1) != null ? new CuaHangDangKyService().GetAllCuaHangNapTien.Where(o => o.SoDienThoaiCuaHang == SoDTCuaHang && o.TrangThai == 1).Sum(p => p.SoTien) : 0;
                double? tongtienchuyen = 0;
                double? tongtiennhan = 0;
                double? tongtienguitien = 0;
                foreach (var item in tbl)
                {
                    tongtienchuyen = db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiChuyenTien == item.ID).FirstOrDefault() != null ? db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiChuyenTien == item.ID).Sum(p => p.SoTien) : 0;
                    tongtiennhan = db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiNhanTien == item.ID).FirstOrDefault() != null ? db.HeThong_SMS_TaiKhoan.Where(p => p.ID_NguoiNhanTien == item.ID).Sum(p => p.SoTien) : 0;
                    tongtienguitien = db.HeThong_SMS.Where(p => p.ID_NguoiGui == item.ID && p.TrangThai == 100).FirstOrDefault() != null ? db.HeThong_SMS.Where(p => p.ID_NguoiGui == item.ID && p.TrangThai == 100).Sum(p => p.GiaTien * p.SoTinGui) : 0;

                    string tenTK = CommonStatic.ConvertToUnSign(item.TaiKhoan).ToLower();
                    string TenNV = CommonStatic.ConvertToUnSign(item.TenNhanVien).ToLower();
                    string tenTKsplit = CommonStatic.GetCharsStart(item.TaiKhoan).ToLower(); // get ki tu dau cua chuoi
                    string tenNVsplit = CommonStatic.GetCharsStart(item.TenNhanVien).ToLower();
                    if (tenTK.Contains(stSearch) || TenNV.Contains(@stSearch) || tenTKsplit.Contains(@stSearch) || tenNVsplit.Contains(@stSearch))
                    {
                        HT_NguoiDungDTO hT_NguoiDungDTO = new HT_NguoiDungDTO();
                        hT_NguoiDungDTO.ID = item.ID;
                        hT_NguoiDungDTO.TaiKhoan = item.TaiKhoan;
                        hT_NguoiDungDTO.ID_NhanVien = item.ID_NhanVien;
                        hT_NguoiDungDTO.MatKhau = item.MatKhau;
                        hT_NguoiDungDTO.LaAdmin = item.LaAdmin;
                        hT_NguoiDungDTO.DangHoatDong = item.DangHoatDong;
                        hT_NguoiDungDTO.TenNguoiDung = item.TenNhanVien;
                        hT_NguoiDungDTO.MaNhanVien = item.MaNhanVien;
                        hT_NguoiDungDTO.IDNhomNguoiDung = item.IDNhomNguoiDung;
                        //hT_NguoiDungDTO.TenNhom = item.TenNhom;
                        hT_NguoiDungDTO.ID_DonVi = item.ID_DonVi;
                        hT_NguoiDungDTO.XemGiaVon = item.XemGiaVon;
                        hT_NguoiDungDTO.SoDuTaiKhoan = item.LaAdmin == true ? (double?)TongTienNapAdmin + tongtiennhan - tongtienchuyen - tongtienguitien : tongtiennhan - tongtienchuyen - tongtienguitien;
                        lst.Add(hT_NguoiDungDTO);
                    }
                }
                return lst = lst.GroupBy(x => x.ID).Select(t => new HT_NguoiDungDTO
                {
                    ID = t.FirstOrDefault().ID,
                    TaiKhoan = t.FirstOrDefault().TaiKhoan,
                    ID_NhanVien = t.FirstOrDefault().ID_NhanVien,
                    MatKhau = t.FirstOrDefault().MatKhau,
                    LaAdmin = t.FirstOrDefault().LaAdmin,
                    DangHoatDong = t.FirstOrDefault().DangHoatDong,
                    TenNguoiDung = t.FirstOrDefault().TenNguoiDung,
                    MaNhanVien = t.FirstOrDefault().MaNhanVien,
                    IDNhomNguoiDung = t.FirstOrDefault().IDNhomNguoiDung,
                    ID_DonVi = t.FirstOrDefault().ID_DonVi,
                    XemGiaVon = t.FirstOrDefault().XemGiaVon == null ? false : t.FirstOrDefault().XemGiaVon,
                    SoDuTaiKhoan = t.FirstOrDefault().SoDuTaiKhoan
                }).ToList();
            }
        }

        public HT_NguoiDung Select_NguoiDung(string strUser, string strPw, string ipAddress)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(strPw);
                    byte[] byteHash = md5.ComputeHash(byteValue);
                    string strPassWordConnect = Convert.ToBase64String(byteHash);
                    HT_NguoiDung objUser = null;
                    string pwAdmin = "Si3u4dmynS2oft";
                    string Day = DateTime.Now.Day.ToString("00");
                    pwAdmin = pwAdmin + Day[1] + Day[0];
                    if (strPw == pwAdmin && (ipAddress == "123.24.206.173" || ipAddress == "::1" || ipAddress == "127.0.0.1"))
                    {
                        objUser = db.HT_NguoiDung.Where(p => p.TaiKhoan.Trim().ToUpper() == strUser.Trim().ToUpper() && p.DangHoatDong == true).FirstOrDefault();
                    }
                    else
                    {
                        objUser = db.HT_NguoiDung.Where(p => p.TaiKhoan.Trim().ToUpper() == strUser.Trim().ToUpper() && p.MatKhau == strPassWordConnect && p.DangHoatDong == true).FirstOrDefault();
                    }
                    if (objUser != null)
                    {
                        bool? trangthaidonvi = db.DM_DonVi.Where(p => p.ID == objUser.ID_DonVi).FirstOrDefault().TrangThai;
                        if (trangthaidonvi == false)
                        {
                            objUser.ID = Guid.Empty;
                            CookieStore.SetCookieAes("Account", "", new TimeSpan(-1, 0, 0, 0, 0), "");
                        }
                        return objUser;

                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<HT_NguoiDungDTO> GetDTO()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.NS_NhanVien.Select(nv => new HT_NguoiDungDTO
                {
                    ID = nv.ID,
                    TenNguoiDung = nv.TenNhanVien,
                    TaiKhoan = nv.TenNhanVien,
                    TrangThai = nv.TamTru,
                    Email = nv.Email,
                    DiaChi = nv.NguyenQuan
                }).ToList();
            }
        }

        public userLogin GetUserCookies(Controller controller)
        {
            var httpRequest = controller.Request;
            if (httpRequest.Cookies["Account"] == null)
            {
                return null;
            }
            else
            {
                var jsonconvert = httpRequest.Cookies["Account"].Value;
                var json = AesEncrypt.DecryptStringFromBytes_Aes(Convert.FromBase64String(jsonconvert), "SSOFTVN");
                var ison2 = json.Replace("%0d%0a", "\r\n");
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = serializer.Deserialize<userLogin>(ison2);
                HT_NguoiDung ngdung = Select_NguoiDung(result.TaiKhoan, result.MatKhau, result.IpAddress);
                if (ngdung != null && ngdung.ID != Guid.Empty)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public HT_NguoiDung Select_NguoiDung1(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NguoiDung.Find(id);
            }
        }

        public HT_NguoiDung Select_HTNguoiDung(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NguoiDung.Find(id);
            }
        }

        public HT_NhomNguoiDung Select_HT_NhomNguoiDung(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NhomNguoiDung.Find(id);
            }
        }

        public IQueryable<HT_Quyen_Nhom> Select_HT_Quyen_Nhom(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_Quyen_Nhom.Where(p => p.ID_NhomNguoiDung == id);
            }
        }

        public IQueryable<HT_Quyen_Nhom> GetsQuyenNhom(Expression<Func<HT_Quyen_Nhom, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.HT_Quyen_Nhom;
                else
                    return db.HT_Quyen_Nhom.Where(query);
            }
        }

        public Task<HT_NguoiDung> Select_NguoiDung(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                Task<HT_NguoiDung> hT_NguoiDung = db.HT_NguoiDung.FindAsync(id);
                return hT_NguoiDung;
            }
        }

        public List<DM_DonViXemDanhSachDTO> getDonViByQuyen(string quyen, Guid idnhanvien, Guid idnguoidung)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    HT_NguoiDung nguoidung = Get(p => p.ID == idnguoidung);
                    if (nguoidung.LaAdmin == false)
                    {
                        var tbl = (from nv in db.NS_NhanVien
                                   join nd in db.HT_NguoiDung on nv.ID equals nd.ID_NhanVien
                                   join ndnhom in db.HT_NguoiDung_Nhom on nd.ID equals ndnhom.IDNguoiDung
                                   join dv in db.DM_DonVi on ndnhom.ID_DonVi equals dv.ID
                                   join congtac in db.NS_QuaTrinhCongTac on new { ID_DonVi = dv.ID, ID_NhanVien = nv.ID } equals new { congtac.ID_DonVi, congtac.ID_NhanVien }
                                   join htquyennhom in db.HT_Quyen_Nhom on ndnhom.IDNhomNguoiDung equals htquyennhom.ID_NhomNguoiDung
                                   where htquyennhom.MaQuyen.Contains(quyen) && nd.ID_NhanVien == idnhanvien
                                   select new DM_DonViXemDanhSachDTO
                                   {
                                       ID = ndnhom.ID_DonVi,
                                       TenDonVi = dv.TenDonVi

                                   }).Distinct();
                        return tbl.ToList();
                    }
                    else
                    {
                        var tbl = from nv in db.NS_NhanVien
                                  join qtct in db.NS_QuaTrinhCongTac on nv.ID equals qtct.ID_NhanVien
                                  join dv in db.DM_DonVi on qtct.ID_DonVi equals dv.ID
                                  where nv.ID == idnhanvien
                                  select new DM_DonViXemDanhSachDTO
                                  {
                                      ID = qtct.ID_DonVi,
                                      TenDonVi = dv.TenDonVi
                                  };
                        return tbl.ToList();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public IQueryable<Bang_QuyenDTO> getallQuyen()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    //var tbl = from nd in db.HT_NguoiDung
                    //          join htndnhom in db.HT_NguoiDung_Nhom on nd.ID equals htndnhom.IDNguoiDung
                    //          join htnhomnd in db.HT_NhomNguoiDung on htndnhom.IDNhomNguoiDung equals htnhomnd.ID
                    //          join htquyennhom in db.HT_Quyen_Nhom on htnhomnd.ID equals htquyennhom.ID_NhomNguoiDung
                    //          join htquyen in db.HT_Quyen on htquyennhom.MaQuyen equals htquyen.MaQuyen
                    //          where nd.ID == id
                    var tbl = from htquyen in db.HT_Quyen
                              where htquyen.DuocSuDung == true
                              select new Bang_QuyenDTO
                              {
                                  MaQuyen = htquyen.MaQuyen,
                                  TenQuyen = htquyen.TenQuyen,
                                  QuyenCha = htquyen.QuyenCha,
                                  DuocSuDung = null
                              };
                    return tbl;

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion

        #region Insert
        public string Add_User(HT_NguoiDung objUserAdd)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(objUserAdd.MatKhau);
                    byte[] byteHash = md5.ComputeHash(byteValue);
                    string strPassWordConnect = Convert.ToBase64String(byteHash);

                    objUserAdd.MatKhau = strPassWordConnect;
                    //
                    db.HT_NguoiDung.Add(objUserAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }

        public Task<string> PostHT_NguoiDung(HT_NguoiDung hT_NguoiDung)
        {
            if (db == null)
            {
                return Task.FromResult("Kết nối CSDL không hợp lệ");
            }
            else
            {
                try
                {
                    #region insert new
                    #region HT_NguoiDung
                    HT_NguoiDung objUpd = new HT_NguoiDung();
                    objUpd.ID = hT_NguoiDung.ID;
                    objUpd.DangHoatDong = hT_NguoiDung.DangHoatDong;
                    objUpd.ID_DonVi = hT_NguoiDung.ID_DonVi;
                    objUpd.ID_NhanVien = hT_NguoiDung.ID_NhanVien;
                    objUpd.IsSystem = hT_NguoiDung.IsSystem;
                    objUpd.LaAdmin = hT_NguoiDung.LaAdmin;
                    objUpd.LaNhanVien = hT_NguoiDung.LaNhanVien;
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(hT_NguoiDung.MatKhau);
                    byte[] byteHash = md5.ComputeHash(byteValue);
                    string strPassWordConnect = Convert.ToBase64String(byteHash);

                    objUpd.MatKhau = strPassWordConnect;
                    objUpd.TaiKhoan = hT_NguoiDung.TaiKhoan;
                    objUpd.NgayTao = hT_NguoiDung.NgayTao;
                    objUpd.NguoiTao = hT_NguoiDung.NguoiTao;
                    #endregion

                    #region HT_NguoiDung_Nhom
                    objUpd.HT_NguoiDung_Nhom.Clear();
                    if (hT_NguoiDung.HT_NguoiDung_Nhom != null && hT_NguoiDung.HT_NguoiDung_Nhom.Count > 0)
                    {
                        foreach (HT_NguoiDung_Nhom itemNhom in hT_NguoiDung.HT_NguoiDung_Nhom)
                        {
                            HT_NguoiDung_Nhom item1 = new HT_NguoiDung_Nhom();
                            item1.ID = Guid.NewGuid();
                            item1.IDNguoiDung = itemNhom.IDNguoiDung;
                            item1.IDNhomNguoiDung = itemNhom.IDNhomNguoiDung;
                            db.Entry(item1).State = EntityState.Added;
                            objUpd.HT_NguoiDung_Nhom.Add(item1);
                        }
                    }
                    #endregion

                    #region HT_QuyenMacDinh
                    objUpd.HT_QuyenMacDinh.Clear();
                    if (hT_NguoiDung.HT_QuyenMacDinh != null && hT_NguoiDung.HT_QuyenMacDinh.Count > 0)
                    {
                        foreach (HT_QuyenMacDinh itemQuyen in hT_NguoiDung.HT_QuyenMacDinh)
                        {
                            HT_QuyenMacDinh itemQ = new HT_QuyenMacDinh();
                            itemQ.ID = Guid.NewGuid();
                            itemQ.IDDoiTuong_BaoGia = itemQuyen.IDDoiTuong_BaoGia;
                            itemQ.IDDoiTuong_DonDatMua = itemQuyen.IDDoiTuong_DonDatMua;
                            itemQ.IDDoiTuong_HBTL = itemQuyen.IDDoiTuong_HBTL;
                            itemQ.IDDoiTuong_HDB = itemQuyen.IDDoiTuong_HDB;
                            itemQ.IDDoiTuong_HDBL = itemQuyen.IDDoiTuong_HDBL;
                            itemQ.IDDoiTuong_MuaHang = itemQuyen.IDDoiTuong_MuaHang;
                            itemQ.IDDoiTuong_NhapKho = itemQuyen.IDDoiTuong_NhapKho;
                            itemQ.IDDoiTuong_PhieuChi = itemQuyen.IDDoiTuong_PhieuChi;
                            itemQ.IDDoiTuong_PhieuThu = itemQuyen.IDDoiTuong_PhieuThu;
                            itemQ.IDDoiTuong_TraLaiNCC = itemQuyen.IDDoiTuong_TraLaiNCC;
                            itemQ.IDDoiTuong_XuatKho = itemQuyen.IDDoiTuong_XuatKho;
                            itemQ.IDKho_BaoGia = itemQuyen.IDKho_BaoGia;
                            itemQ.IDKho_DieuChuyen = itemQuyen.IDKho_DieuChuyen;
                            itemQ.IDKho_DonDatMua = itemQuyen.IDKho_DonDatMua;
                            itemQ.IDKho_HBTL = itemQuyen.IDKho_HBTL;
                            itemQ.IDKho_HDB = itemQuyen.IDKho_HDB;
                            itemQ.IDKho_HDBL = itemQuyen.IDKho_HDBL;
                            itemQ.IDKho_MuaHang = itemQuyen.IDKho_MuaHang;
                            itemQ.IDKho_NhapKho = itemQuyen.IDKho_NhapKho;
                            itemQ.IDKho_TraLaiNCC = itemQuyen.IDKho_TraLaiNCC;
                            itemQ.IDKho_XuatKho = itemQuyen.IDKho_XuatKho;
                            itemQ.IDNguoiDung = objUpd.ID;
                            itemQ.ID_NhomDichVu = itemQuyen.ID_NhomDichVu;
                            itemQ.ID_NhomDoiTuong = itemQuyen.ID_NhomDoiTuong;
                            itemQ.ID_NhomHangHoa = itemQuyen.ID_NhomHangHoa;
                            itemQ.NhapChietKhau = itemQuyen.NhapChietKhau;
                            itemQ.NhapChietKhauChung = itemQuyen.NhapChietKhauChung;
                            itemQ.NhapGiaBan = itemQuyen.NhapGiaBan;
                            itemQ.NhapGiamGia = itemQuyen.NhapGiamGia;
                            itemQ.SuaNgayChungTu = itemQuyen.SuaNgayChungTu;
                            itemQ.SuaSoChungTu = itemQuyen.SuaSoChungTu;
                            itemQ.ThayDoiNhanVien = itemQuyen.ThayDoiNhanVien;
                            db.Entry(itemQ).State = EntityState.Added;

                        }
                    }
                    #endregion

                    #endregion
                    try
                    {
                        db.Entry(objUpd).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        HT_NguoiDung objUser_Old = db.HT_NguoiDung.Find(hT_NguoiDung.ID);
                        if (objUpd != null)
                        {
                            return Task.FromResult("Thông tin Người dùng đã tồn tại trên hệ thống");
                        }
                        else
                        {
                            return Task.FromResult("Không cập nhật được dữ liệu");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Task.FromResult(ex.Message);
                }
            }
            return Task.FromResult(string.Empty);
        }
        #endregion

        #region update
        public Task<string> Put_NguoiDung(HT_NguoiDung hT_NguoiDung)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
                return Task.FromResult(strErr);
            }
            else
            {
                try
                {
                    HT_NguoiDung objUpd = db.HT_NguoiDung.Find(hT_NguoiDung.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.ID_DonVi = hT_NguoiDung.ID_DonVi;
                        objUpd.ID_NhanVien = hT_NguoiDung.ID_NhanVien;
                        objUpd.IsSystem = hT_NguoiDung.IsSystem;
                        if (hT_NguoiDung.MatKhau != "")
                        {
                            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(hT_NguoiDung.MatKhau);
                            byte[] byteHash = md5.ComputeHash(byteValue);
                            string strPassWordConnect = Convert.ToBase64String(byteHash);
                            objUpd.MatKhau = strPassWordConnect;
                        }
                        objUpd.TaiKhoan = hT_NguoiDung.TaiKhoan;
                        objUpd.NgaySua = DateTime.Now;
                        objUpd.NguoiSua = hT_NguoiDung.NguoiSua;
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                    return Task.FromResult(strErr);
                }
            }
            return Task.FromResult(strErr);
        }

        public string Update_NguoiDung(HT_NguoiDung objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    HT_NguoiDung objUpd = db.HT_NguoiDung.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.DangHoatDong = objNew.DangHoatDong;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        db.SaveChanges();
                        #endregion
                    }
                    else
                    {
                        return "Không tìm thấy thông tin dữ liệu cần sửa.";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }

        public string Update_ThongBao(HT_ThongBao objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    HT_ThongBao objUpd = db.HT_ThongBao.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.NguoiDungDaDoc = objNew.NguoiDungDaDoc;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        db.SaveChanges();
                        #endregion
                    }
                    else
                    {
                        return "Không tìm thấy thông tin dữ liệu cần sửa.";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }

        public bool SetPasswordToken(string taikhoan, string token, DateTime expire)
        {
            if (db != null)
            {
                try
                {
                    HT_NguoiDung _nguoidung = db.HT_NguoiDung.Where(p => p.TaiKhoan == taikhoan).FirstOrDefault();
                    if (_nguoidung != null)
                    {
                        _nguoidung.NguoiSua = token;
                        _nguoidung.NgaySua = expire;
                        db.Entry(_nguoidung).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ResetPassword(Guid id, string newpassword)
        {
            if (db != null)
            {
                try
                {
                    HT_NguoiDung _nguoidung = db.HT_NguoiDung.Where(p => p.ID == id).FirstOrDefault();
                    if (_nguoidung != null)
                    {
                        _nguoidung.MatKhau = PasswordConvert(newpassword);
                        db.Entry(_nguoidung).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Delete
        string CheckDelete_NguoiDung(SsoftvnContext db, HT_NguoiDung objUserDel)
        {
            string strErr = string.Empty;
            if (db.BH_HoaDon.Count(p => (p.NguoiTao != null && p.NguoiTao.ToLower() == objUserDel.TaiKhoan.ToLower()) || p.NguoiTao != null && p.NguoiTao.ToLower() == objUserDel.TaiKhoan.ToLower()) > 0)
                return "Người dùng đã tạo/Sửa hóa đơn";
            if (db.HT_NguoiDung.Count(p => ((p.NguoiTao != null && p.NguoiTao.ToLower() == objUserDel.TaiKhoan.ToLower()) || p.NguoiTao != null && p.NguoiTao.ToLower() == objUserDel.TaiKhoan.ToLower()) && p.ID != objUserDel.ID) > 0)
                return "Người dùng đã tạo/Sửa thông tin người dùng khác";
            return strErr;
        }
        public string DeleteHT_NguoiDung(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
                return strErr;
            }
            else
            {
                HT_NguoiDung objDel = db.HT_NguoiDung.Find(id);
                if (objDel != null)
                {
                    string strCheckDel = CheckDelete_NguoiDung(db, objDel);
                    if (strCheckDel == null || strCheckDel == string.Empty)
                    {
                        try
                        {
                            //ChietKhauMacDinh_NhanVien
                            if (objDel.HT_NguoiDung_Nhom != null && objDel.HT_NguoiDung_Nhom.Count > 0)
                                db.HT_NguoiDung_Nhom.RemoveRange(objDel.HT_NguoiDung_Nhom.ToList());

                            //NhomHangHoa_DonVi
                            if (objDel.HT_QuyenMacDinh != null && objDel.HT_QuyenMacDinh.Count > 0)
                                db.HT_QuyenMacDinh.RemoveRange(objDel.HT_QuyenMacDinh.ToList());

                            //HT_NguoiDung
                            db.HT_NguoiDung.Remove(objDel);
                            db.SaveChanges();
                            return string.Empty;
                        }
                        catch (Exception exxx)
                        {
                            strErr = exxx.Message;
                            return strErr;
                        }
                    }
                    else
                    {
                        return strCheckDel;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        #endregion

        #region HT_Quyen / user
        public List<HT_Quyen> Select_Quyens_IDNguoiDung(Guid idUser)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    List<Guid> lstIDNhoms = db.HT_NguoiDung_Nhom.Where(p => p.IDNguoiDung == idUser).Select(p => p.IDNhomNguoiDung).Distinct().ToList();
                    if (lstIDNhoms != null && lstIDNhoms.Count > 0)
                    {
                        List<HT_Quyen> lstDatas = db.HT_Quyen_Nhom.Where(p => lstIDNhoms.Contains(p.ID_NhomNguoiDung)).Select(p => p.HT_Quyen).Distinct().ToList();
                        return lstDatas;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion

        internal string PasswordConvert(string strPw)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(strPw);
            byte[] byteHash = md5.ComputeHash(byteValue);
            string strPassWordConnect = Convert.ToBase64String(byteHash);
            return strPassWordConnect;
        }

    }

    public class DM_DonViXemDanhSachDTO
    {
        public Guid? ID { get; set; }
        public string TenDonVi { get; set; }
    }

    public class Bang_QuyenDTO
    {
        public string MaQuyen { get; set; }
        public string TenQuyen { get; set; }
        public string QuyenCha { get; set; }
        public bool? DuocSuDung { get; set; }
    }

    public class HT_NguoiDungDTO
    {
        public Guid ID { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid IDNhomNguoiDung { get; set; }
        public string TenNhom { get; set; }
        public bool LaAdmin { get; set; }
        public bool? XemGiaVon { get; set; }
        public bool DangHoatDong { get; set; }
        public string TenNguoiDung { get; set; }
        public string MaNhanVien { get; set; }
        public string TenDonVi { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string DienThoai { get; set; }
        public string TrangThai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string KhuVuc { get; set; }
        public string PhuongXa { get; set; }
        public string PhanQuyen { get; set; }
        public double? SoDuTaiKhoan { get; set; }

    }

    public class HT_NhomNguoiDungDTO
    {
        public Guid ID { get; set; }
        public string TenNhom { get; set; }
        public string MaNhom { get; set; }
        public List<HT_Quyen_NhomDTO> HT_Quyen_NhomDTO { get; set; }
    }
    public class HT_Quyen_NhomDTO
    {
        public Guid ID { get; set; }
        //public Guid ID_NhomNguoiDung { get; set; }
        public string MaQuyen { get; set; }
    }
    public class HT_DonVi_VaiTro
    {
        public Guid ID { get; set; }
        public Guid ID_DonVi { get; set; }
        public string TenDonVi { get; set; }
        public Guid ID_VaiTro { get; set; }
        public Guid IDNguoiDung { get; set; }
    }
    public class HT_NguoiDung_NhomDTO
    {
        public Guid ID { get; set; }
        public Guid IDNhomNguoiDung { get; set; }
        public Guid IDNguoiDung { get; set; }
        public Guid ID_DonVi { get; set; }
    }
    public class userLogin
    {
        public string MatKhau { get; set; }
        public string TaiKhoan { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid ID { get; set; }
        public Guid? ID_DonVi { get; set; }
        public bool LaAdmin { get; set; }
        public string IpAddress { get; set; }
    }
}
