using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lib_ChamSocKhachHang
{
    public class class_LichHen
    {
        private SsoftvnContext db;
        public class_LichHen(SsoftvnContext _db)
        {
            db = _db;
        }
        public ChamSocKhachHang Select_LichHen(Guid? id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.ChamSocKhachHang.Find(id);
            }
        }

        public List<ChamSocKhachHang> Gets(Expression<Func<ChamSocKhachHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.ChamSocKhachHang.Where(p => p.PhanLoai == 3).ToList();
                else
                    return db.ChamSocKhachHang.Where(query).Where(p => p.PhanLoai == 3).ToList();
            }
        }

        public List<ChamSocKhachHang> GetsFromTo(DateTime from, DateTime to, Guid ID_ChiNhanh)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return db.ChamSocKhachHang.Where(p => p.NgayGio >= from || p.NgayGioKetThuc >= from).Where(p => p.NgayGio <= to || p.NgayGioKetThuc <= to).Where(p => p.PhanLoai == 3).Where(p => p.ID_DonVi == ID_ChiNhanh).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }
        #region GET
        //trinhpv
        //Tìm kiếm theo tiêu đề hoặc theo khách hàng
        //Chuyển sang chuỗi không dấu
        public string ConvertToUnSign(string text)
        {
            if (text != null)
            {
                for (int i = 33; i < 48; i++)
                {
                    text = text.Replace(((char)i).ToString(), "");
                }

                for (int i = 58; i < 65; i++)
                {
                    text = text.Replace(((char)i).ToString(), "");
                }

                for (int i = 91; i < 97; i++)
                {
                    text = text.Replace(((char)i).ToString(), "");
                }
                for (int i = 123; i < 127; i++)
                {
                    text = text.Replace(((char)i).ToString(), "");
                }
                text = text.Replace(" ", "-");
                Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
                string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);
                return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            else
            {
                return null;
            }
        }

        public List<LH_ChamSocKhachHang> SeachLichHen(string giatri, string giatrinv, Guid IDChinhNhanh)
        {
            string a = ConvertToUnSign(giatri).ToLower();
            var tbl = from cs in db.ChamSocKhachHang
                      join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                      from j in cskh.DefaultIfEmpty()

                      join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                      from f in csnv.DefaultIfEmpty()

                      join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                      from k in cstv.DefaultIfEmpty()

                      join nd in db.HT_NguoiDung on f.ID equals nd.ID_NhanVien into ndf
                      from h in ndf.DefaultIfEmpty()

                      join dv in db.DM_DonVi on h.ID_DonVi equals dv.ID into dvh
                      from l in dvh.DefaultIfEmpty()
                          //where l.ID == IDChinhNhanh
                      orderby cs.NgayTao descending
                      select new
                      {
                          cs.ID,
                          cs.ID_KhachHang,
                          cs.ID_LoaiTuVan,
                          cs.ID_NhanVien,
                          cs.Ma_TieuDe,
                          cs.NgayGio,
                          cs.NgayGioKetThuc,
                          j.TenDoiTuong,
                          k.TenLoaiTuVanLichHen,
                          cs.NhacNho,
                          cs.TrangThai,
                          cs.NoiDung,
                          f.TenNhanVien,
                          cs.NguoiTao,
                          cs.NgayTao,
                          cs.NguoiSua,
                          cs.NgaySua,
                          cs.ID_NhanVienQuanLy
                      };
            List<LH_ChamSocKhachHang> lst = new List<LH_ChamSocKhachHang>();
            foreach (var item in tbl)
            {
                if (item.TenDoiTuong != null & item.Ma_TieuDe != null)
                {
                    string b = ConvertToUnSign(item.TenDoiTuong).ToLower();
                    string c = ConvertToUnSign(item.Ma_TieuDe).ToLower();

                    if (b.Contains(@a) || item.TenDoiTuong.Contains(@giatri) || c.Contains(@a) || item.Ma_TieuDe.Contains(@giatri))
                    {
                        LH_ChamSocKhachHang LH_lst = new LH_ChamSocKhachHang();
                        LH_lst.ID = item.ID;
                        LH_lst.ID_KhachHang = item.ID_KhachHang;
                        LH_lst.ID_LoaiTuVan = item.ID_LoaiTuVan;
                        LH_lst.ID_NhanVienQuanLy = item.ID_NhanVienQuanLy.Value;
                        LH_lst.NguoiSua = item.NguoiSua;
                        LH_lst.NguoiTao = item.NguoiTao;
                        LH_lst.ID_NhanVien = item.ID_NhanVien;

                        LH_lst.Ma_TieuDe = item.Ma_TieuDe;
                        LH_lst.NgayGio = item.NgayGio;
                        LH_lst.NgayGioKetThuc = item.NgayGioKetThuc;
                        LH_lst.TenKhachHang = item.TenDoiTuong;
                        LH_lst.TenLoaiTV = item.TenLoaiTuVanLichHen;
                        LH_lst.NhacNho = item.NhacNho;
                        LH_lst.TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : "")));
                        LH_lst.NoiDung = item.NoiDung;
                        LH_lst.TenNV = item.TenNhanVien;
                        lst.Add(LH_lst);
                    }
                }
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }

        public List<listPhanLoai> getPhanLoaiSeach(string giatri, string giatrinv, Guid IDchinhanh)
        {
            var tbl = (from cs in db.ChamSocKhachHang
                       join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                       from j in cskh.DefaultIfEmpty()

                       join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                       from f in csnv.DefaultIfEmpty()

                       join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                       from k in cstv.DefaultIfEmpty()

                       join nd in db.HT_NguoiDung on f.ID equals nd.ID_NhanVien into ndf
                       from h in ndf.DefaultIfEmpty()

                       join dv in db.DM_DonVi on h.ID_DonVi equals dv.ID into dvh
                       from l in dvh.DefaultIfEmpty()
                           // where cs.Ma_TieuDe == giatri | f.TenNhanVien == giatri /*& l.ID == IDChinhNhanh*/
                       where cs.Ma_TieuDe.Contains(@giatri) || j.TenDoiTuong.Contains(@giatrinv)
                       orderby cs.NgayTao descending
                       group k by new
                       {
                           k.TenLoaiTuVanLichHen
                       } into kg
                       select new
                       {
                           kg.Key.TenLoaiTuVanLichHen
                       }).OrderByDescending(x => x.TenLoaiTuVanLichHen);
            List<listPhanLoai> lst = new List<listPhanLoai>();
            foreach (var item in tbl)
            {
                listPhanLoai PH = new listPhanLoai();
                PH.PhanLoai = item.TenLoaiTuVanLichHen;
                lst.Add(PH);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        //load phân loại
        public List<listPhanLoai> getListPhanLoai(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            var tbl = (from cs in db.ChamSocKhachHang.Where(x => x.PhanLoai == 3)
                       join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                       from j in cskh.DefaultIfEmpty()

                       join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                       from f in csnv.DefaultIfEmpty()

                       join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                       from k in cstv.DefaultIfEmpty()

                       join nd in db.HT_NguoiDung on f.ID equals nd.ID_NhanVien into ndf
                       from h in ndf.DefaultIfEmpty()

                       join dv in db.DM_DonVi on h.ID_DonVi equals dv.ID into dvh
                       from l in dvh.DefaultIfEmpty()
                       where cs.NgayTao >= dayStart & cs.NgayTao < dayEnd /*& l.ID == IDChinhNhanh*/
                       group k by new
                       {
                           k.TenLoaiTuVanLichHen
                       } into kg
                       select new
                       {
                           kg.Key.TenLoaiTuVanLichHen
                       }).OrderByDescending(x => x.TenLoaiTuVanLichHen);
            List<listPhanLoai> lst = new List<listPhanLoai>();
            foreach (var item in tbl)
            {
                listPhanLoai PH = new listPhanLoai();
                PH.PhanLoai = item.TenLoaiTuVanLichHen;
                lst.Add(PH);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }

        public List<LH_ChamSocKhachHang> getAllLichHen(DateTime dayStart, DateTime dayEnd, Guid IDChinhNhanh)
        {
            var tbl = from cs in db.ChamSocKhachHang.Where(x => x.PhanLoai == 3)
                      join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                      from j in cskh.DefaultIfEmpty()

                      join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                      from f in csnv.DefaultIfEmpty()

                      join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                      from k in cstv.DefaultIfEmpty()

                      join nd in db.HT_NguoiDung on f.ID equals nd.ID_NhanVien into ndf
                      from h in ndf.DefaultIfEmpty()

                      join dv in db.DM_DonVi on h.ID_DonVi equals dv.ID into dvh
                      from l in dvh.DefaultIfEmpty()
                      where cs.NgayTao >= dayStart & cs.NgayTao < dayEnd /*& l.ID == IDChinhNhanh*/
                      orderby cs.NgayTao descending
                      select new
                      {
                          cs.ID,
                          cs.ID_KhachHang,
                          cs.ID_LoaiTuVan,
                          cs.ID_NhanVien,
                          cs.ID_NhanVienQuanLy,
                          cs.NguoiTao,
                          cs.NguoiSua,

                          cs.Ma_TieuDe,
                          cs.NgayGio,
                          cs.NgayGioKetThuc,
                          j.TenDoiTuong,
                          //kh.TenDoiTuong,
                          k.TenLoaiTuVanLichHen,
                          cs.NhacNho,
                          cs.TrangThai,
                          cs.NoiDung,
                          f.TenNhanVien
                      };
            List<LH_ChamSocKhachHang> lst = new List<LH_ChamSocKhachHang>();
            foreach (var item in tbl)
            {
                LH_ChamSocKhachHang LH_lst = new LH_ChamSocKhachHang();
                LH_lst.ID = item.ID;
                LH_lst.ID_KhachHang = item.ID_KhachHang;
                LH_lst.ID_LoaiTuVan = item.ID_LoaiTuVan;
                LH_lst.ID_NhanVienQuanLy = item.ID_NhanVienQuanLy.Value;
                LH_lst.NguoiSua = item.NguoiSua;
                LH_lst.NguoiTao = item.NguoiTao;
                LH_lst.ID_NhanVien = item.ID_NhanVien;

                LH_lst.Ma_TieuDe = item.Ma_TieuDe;
                LH_lst.NgayGio = item.NgayGio;
                LH_lst.NgayGioKetThuc = item.NgayGioKetThuc;
                LH_lst.TenKhachHang = item.TenDoiTuong;
                LH_lst.TenLoaiTV = item.TenLoaiTuVanLichHen;
                LH_lst.NhacNho = item.NhacNho;
                LH_lst.TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : "")));
                LH_lst.NoiDung = item.NoiDung;
                LH_lst.TenNV = item.TenNhanVien;
                lst.Add(LH_lst);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        // filter theo phân loại
        public List<LH_ChamSocKhachHang> getFilterPhanLoai(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, Guid IDChinhNhanh)
        {
            var tbl = from cs in db.ChamSocKhachHang.Where(x => x.PhanLoai == 3)
                      join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                      from j in cskh.DefaultIfEmpty()

                      join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                      from f in csnv.DefaultIfEmpty()

                      join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                      from k in cstv.DefaultIfEmpty()

                      join nd in db.HT_NguoiDung on f.ID equals nd.ID_NhanVien into ndf
                      from h in ndf.DefaultIfEmpty()

                      join dv in db.DM_DonVi on h.ID_DonVi equals dv.ID into dvh
                      from l in dvh.DefaultIfEmpty()
                      where cs.NgayTao >= dayStart & cs.NgayTao < dayEnd & k.TenLoaiTuVanLichHen == TenLoaiTV /*& l.ID == IDChinhNhanh*/
                      orderby cs.NgayTao descending
                      select new
                      {
                          cs.ID,
                          cs.ID_KhachHang,
                          cs.ID_LoaiTuVan,
                          cs.ID_NhanVien,
                          cs.ID_NhanVienQuanLy,
                          cs.NguoiTao,
                          cs.NguoiSua,

                          cs.Ma_TieuDe,
                          cs.NgayGio,
                          cs.NgayGioKetThuc,
                          j.TenDoiTuong,
                          k.TenLoaiTuVanLichHen,
                          cs.NhacNho,
                          cs.TrangThai,
                          cs.NoiDung,
                          f.TenNhanVien
                      };
            List<LH_ChamSocKhachHang> lst = new List<LH_ChamSocKhachHang>();
            foreach (var item in tbl)
            {
                LH_ChamSocKhachHang LH_lst = new LH_ChamSocKhachHang();
                LH_lst.ID = item.ID;
                LH_lst.ID_KhachHang = item.ID_KhachHang;
                LH_lst.ID_LoaiTuVan = item.ID_LoaiTuVan;
                LH_lst.ID_NhanVienQuanLy = item.ID_NhanVienQuanLy.Value;
                LH_lst.NguoiSua = item.NguoiSua;
                LH_lst.NguoiTao = item.NguoiTao;
                LH_lst.ID_NhanVien = item.ID_NhanVien;

                LH_lst.Ma_TieuDe = item.Ma_TieuDe;
                LH_lst.NgayGio = item.NgayGio;
                LH_lst.NgayGioKetThuc = item.NgayGioKetThuc;
                LH_lst.TenKhachHang = item.TenDoiTuong;
                LH_lst.TenLoaiTV = item.TenLoaiTuVanLichHen;
                LH_lst.NhacNho = item.NhacNho;
                LH_lst.TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : "")));
                LH_lst.NoiDung = item.NoiDung;
                LH_lst.TenNV = item.TenNhanVien;
                lst.Add(LH_lst);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }
        // filter theo trạng thai
        public List<LH_ChamSocKhachHang> getFilterTrangThai(DateTime dayStart, DateTime dayEnd, string TrangThai1, string TrangThai2, string TrangThai3, Guid IDChinhNhanh)
        {
            var tbl = from cs in db.ChamSocKhachHang.Where(x => x.PhanLoai == 3)
                      join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                      from j in cskh.DefaultIfEmpty()

                      join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                      from f in csnv.DefaultIfEmpty()

                      join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                      from k in cstv.DefaultIfEmpty()

                      join nd in db.HT_NguoiDung on f.ID equals nd.ID_NhanVien into ndf
                      from h in ndf.DefaultIfEmpty()

                      join dv in db.DM_DonVi on h.ID_DonVi equals dv.ID into dvh
                      from l in dvh.DefaultIfEmpty()
                      where cs.NgayTao >= dayStart & cs.NgayTao < dayEnd & (cs.TrangThai == TrangThai1 || cs.TrangThai == TrangThai2 || cs.TrangThai == TrangThai3) /*& l.ID == IDChinhNhanh*/
                      orderby cs.NgayTao descending
                      select new
                      {
                          cs.ID,
                          cs.ID_KhachHang,
                          cs.ID_LoaiTuVan,
                          cs.ID_NhanVien,
                          cs.ID_NhanVienQuanLy,
                          cs.NguoiTao,
                          cs.NguoiSua,

                          cs.Ma_TieuDe,
                          cs.NgayGio,
                          cs.NgayGioKetThuc,
                          j.TenDoiTuong,
                          k.TenLoaiTuVanLichHen,
                          cs.NhacNho,
                          cs.TrangThai,
                          cs.NoiDung,
                          f.TenNhanVien
                      };
            List<LH_ChamSocKhachHang> lst = new List<LH_ChamSocKhachHang>();
            foreach (var item in tbl)
            {
                LH_ChamSocKhachHang LH_lst = new LH_ChamSocKhachHang();
                LH_lst.ID = item.ID;
                LH_lst.ID_KhachHang = item.ID_KhachHang;
                LH_lst.ID_LoaiTuVan = item.ID_LoaiTuVan;
                LH_lst.ID_NhanVienQuanLy = item.ID_NhanVienQuanLy.Value;
                LH_lst.NguoiSua = item.NguoiSua;
                LH_lst.NguoiTao = item.NguoiTao;
                LH_lst.ID_NhanVien = item.ID_NhanVien;

                LH_lst.Ma_TieuDe = item.Ma_TieuDe;
                LH_lst.NgayGio = item.NgayGio;
                LH_lst.NgayGioKetThuc = item.NgayGioKetThuc;
                LH_lst.TenKhachHang = item.TenDoiTuong;
                LH_lst.TenLoaiTV = item.TenLoaiTuVanLichHen;
                LH_lst.NhacNho = item.NhacNho;
                LH_lst.TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : "")));
                LH_lst.NoiDung = item.NoiDung;
                LH_lst.TenNV = item.TenNhanVien;
                lst.Add(LH_lst);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }

        // filter theo phân loại, trạng thai
        public List<LH_ChamSocKhachHang> getFilterPLTT(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, string TrangThai1, string TrangThai2, string TrangThai3, Guid IDChinhNhanh)
        {
            var tbl = from cs in db.ChamSocKhachHang.Where(x => x.PhanLoai == 3)
                      join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                      from j in cskh.DefaultIfEmpty()

                      join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                      from f in csnv.DefaultIfEmpty()

                      join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                      from k in cstv.DefaultIfEmpty()

                      join nd in db.HT_NguoiDung on f.ID equals nd.ID_NhanVien into ndf
                      from h in ndf.DefaultIfEmpty()

                      join dv in db.DM_DonVi on h.ID_DonVi equals dv.ID into dvh
                      from l in dvh.DefaultIfEmpty()
                      where cs.NgayTao >= dayStart & cs.NgayTao < dayEnd & k.TenLoaiTuVanLichHen == TenLoaiTV & (cs.TrangThai == TrangThai1 || cs.TrangThai == TrangThai2 || cs.TrangThai == TrangThai3) /*& l.ID == IDChinhNhanh*/
                      orderby cs.NgayTao descending
                      select new
                      {
                          cs.ID,
                          cs.ID_KhachHang,
                          cs.ID_LoaiTuVan,
                          cs.ID_NhanVien,
                          cs.ID_NhanVienQuanLy,
                          cs.NguoiTao,
                          cs.NguoiSua,

                          cs.Ma_TieuDe,
                          cs.NgayGio,
                          cs.NgayGioKetThuc,
                          j.TenDoiTuong,
                          k.TenLoaiTuVanLichHen,
                          cs.NhacNho,
                          cs.TrangThai,
                          cs.NoiDung,
                          f.TenNhanVien
                      };
            List<LH_ChamSocKhachHang> lst = new List<LH_ChamSocKhachHang>();
            foreach (var item in tbl)
            {
                LH_ChamSocKhachHang LH_lst = new LH_ChamSocKhachHang();
                LH_lst.ID = item.ID;
                LH_lst.ID_KhachHang = item.ID_KhachHang;
                LH_lst.ID_LoaiTuVan = item.ID_LoaiTuVan;
                LH_lst.ID_NhanVienQuanLy = item.ID_NhanVienQuanLy.Value;
                LH_lst.NguoiSua = item.NguoiSua;
                LH_lst.NguoiTao = item.NguoiTao;
                LH_lst.ID_NhanVien = item.ID_NhanVien;

                LH_lst.Ma_TieuDe = item.Ma_TieuDe;
                LH_lst.NgayGio = item.NgayGio;
                LH_lst.NgayGioKetThuc = item.NgayGioKetThuc;
                LH_lst.TenKhachHang = item.TenDoiTuong;
                LH_lst.TenLoaiTV = item.TenLoaiTuVanLichHen;
                LH_lst.NhacNho = item.NhacNho;
                LH_lst.TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : "")));
                LH_lst.NoiDung = item.NoiDung;
                LH_lst.TenNV = item.TenNhanVien;
                lst.Add(LH_lst);
            }
            if (lst != null)
            {
                return lst;
            }
            else
            {
                return null;
            }
        }

        public List<LH_ChamSocKhachHang> getList_ChamSocKhachHang(int PhanLoai, Guid? ID_LoaiTV, string TieuDe, string TrangThai1, string TrangThai2, string TrangThai3, DateTime dayStart, DateTime dayEnd, Guid ID_ChiNhanh)
        {
            var tbl = from cs in db.ChamSocKhachHang.Where(x => x.PhanLoai == PhanLoai)
                      join kh in db.DM_DoiTuong on cs.ID_KhachHang equals kh.ID into cskh
                      from j in cskh.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on cs.ID_NhanVien equals nv.ID into csnv
                      from f in csnv.DefaultIfEmpty()
                      join tv in db.DM_LoaiTuVanLichHen on cs.ID_LoaiTuVan equals tv.ID into cstv
                      from k in cstv.DefaultIfEmpty()
                      where cs.NgayTao >= dayStart & cs.NgayTao < dayEnd & cs.ID_DonVi == ID_ChiNhanh & (cs.TrangThai == TrangThai1 || cs.TrangThai == TrangThai2 || cs.TrangThai == TrangThai3)
                      orderby cs.NgayTao descending
                      select new
                      {
                          ID = cs.ID,
                          ID_KhachHang = cs.ID_KhachHang,
                          ID_LoaiTuVan = cs.ID_LoaiTuVan,
                          ID_NhanVien = cs.ID_NhanVien,
                          ID_NhanVienQuanLy = cs.ID_NhanVienQuanLy,
                          NguoiTao = cs.NguoiTao,
                          NguoiSua = cs.NguoiSua,

                          Ma_TieuDe = cs.Ma_TieuDe,
                          NgayGio = cs.NgayGio,
                          NgayGioKetThuc = cs.NgayGioKetThuc,
                          TenDoiTuong = j.TenDoiTuong == null ? "Khách lẻ" : j.TenDoiTuong,
                          TenLoaiTuVanLichHen = k.TenLoaiTuVanLichHen,
                          NhacNho = cs.NhacNho,
                          TrangThai = cs.TrangThai == null ? "" : (cs.TrangThai == "1" ? "Tham khảo" : (cs.TrangThai == "2" ? "Tiềm năng" : (cs.TrangThai == "3" ? "Hủy" : ""))),
                          NoiDung = cs.NoiDung,
                          TenNhanVien = f.TenNhanVien
                      };
            var tbl_format = tbl.AsEnumerable().Select(t => new LH_ChamSocKhachHang
            {
                ID = t.ID,
                ID_KhachHang = t.ID_KhachHang,
                ID_LoaiTuVan = t.ID_LoaiTuVan,
                ID_NhanVienQuanLy = t.ID_NhanVienQuanLy.Value,
                NguoiSua = t.NguoiSua,
                NguoiTao = t.NguoiTao,
                ID_NhanVien = t.ID_NhanVien,
                Ma_TieuDe = t.Ma_TieuDe,
                Ma_TieuDe_GC = CommonStatic.GetCharsStart(t.Ma_TieuDe).ToLower(),
                Ma_TieuDe_CV = CommonStatic.ConvertToUnSign(t.Ma_TieuDe).ToLower(),
                NgayGio = t.NgayGio,
                NgayGioKetThuc = t.NgayGioKetThuc,
                TenKhachHang = t.TenDoiTuong,
                TenKhachHang_GC = CommonStatic.GetCharsStart(t.TenDoiTuong).ToLower(),
                TenKhachHang_CV = CommonStatic.ConvertToUnSign(t.TenDoiTuong).ToLower(),
                TenLoaiTV = t.TenLoaiTuVanLichHen,
                NhacNho = t.NhacNho,
                TrangThai = t.TrangThai,
                NoiDung = t.NoiDung,
                TenNV = t.TenNhanVien,
            });
            if (TieuDe != null & TieuDe != "null" & TieuDe != "")
            {
                TieuDe = CommonStatic.ConvertToUnSign(TieuDe).ToLower();
                tbl_format = tbl_format.Where(x => x.Ma_TieuDe_CV.Contains(@TieuDe) || x.Ma_TieuDe_GC.Contains(@TieuDe) || x.TenKhachHang_CV.Contains(@TieuDe) || x.TenKhachHang_GC.Contains(@TieuDe));
            }
            if (ID_LoaiTV != null)
            {
                tbl_format = tbl_format.Where(x => x.ID_LoaiTuVan == ID_LoaiTV);
            }
            List<LH_ChamSocKhachHang> lst = new List<LH_ChamSocKhachHang>();
            try
            {
                lst = tbl_format.ToList();
            }
            catch
            {
            }
            return lst;
        }

        public List<Calendar_DichVu> GetListDichVu_inLichHen_ByEventID(Guid id)
        {
            SqlParameter param = new SqlParameter("EventID", id);
            var data = db.Database.SqlQuery<Calendar_DichVu>("GetListDichVu_inLichHen_ByEventID @EventID", param).ToList();
            return data;
        }

        public List<SP_Calendar> GetListCalendar(ParamCalendar param)
        {
            string idDonVis = string.Join(",", param.ID_DonVis);
            string idLoaiTuVans = string.Join(",", param.IDLoaiTuVans);
            string idNVPhuTrachs = string.Join(",", param.IDNhanVienPhuTrachs);
            string idNVPhoiHops = string.Join(",", param.IDNhanVienPhuTrachs);
            string trangthaiCV = string.Join(",", param.TrangThaiCVs);
            string phanloai = param.PhanLoai;
            string uutien = param.MucDoUuTien;
            string loaiDT = param.LoaiDoiTuong;
            string txtSearch = param.TextSearch;

            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(new SqlParameter("IDChiNhanhs", idDonVis));
            lstParam.Add(new SqlParameter("IDLoaiTuVans", idLoaiTuVans));
            lstParam.Add(new SqlParameter("IDNhanVienPhuTrachs", idNVPhuTrachs));
            lstParam.Add(new SqlParameter("TrangThaiCVs", trangthaiCV));
            lstParam.Add(new SqlParameter("PhanLoai", phanloai));
            lstParam.Add(new SqlParameter("DoUuTien", uutien));
            lstParam.Add(new SqlParameter("LoaiDoiTuong", loaiDT));
            lstParam.Add(new SqlParameter("FromDate", param.FromDate));
            lstParam.Add(new SqlParameter("ToDate", param.ToDate));
            lstParam.Add(new SqlParameter("IDKhachHang", param.ID_KhachHang));
            lstParam.Add(new SqlParameter("TextSearch", param.TextSearch??(object)DBNull.Value));
            lstParam.Add(new SqlParameter("CurentPage", param.CurrentPage));
            lstParam.Add(new SqlParameter("PageSize", param.PageSize));
            db.Database.CommandTimeout = 300;
            var data = db.Database.SqlQuery<SP_Calendar>("GetListLichHen_FullCalendar @IDChiNhanhs,@IDLoaiTuVans,@IDNhanVienPhuTrachs,@TrangThaiCVs," +
                "@PhanLoai, @DoUuTien, @LoaiDoiTuong, @FromDate, @ToDate, @IDKhachHang, @TextSearch, @CurentPage, @PageSize", lstParam.ToArray()).ToList();
            
            return data;
        }
        #endregion
        #region insert
        public string Add_LichHen(ChamSocKhachHang objAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.ChamSocKhachHang.Add(objAdd);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in dbEx.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), dbEx);
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public string Update_LichHen(ChamSocKhachHang obj)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    #region ChamSocKhachHang
                    ChamSocKhachHang objUpd = db.ChamSocKhachHang.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.Ma_TieuDe = obj.Ma_TieuDe;
                    objUpd.ID_KhachHang = obj.ID_KhachHang;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.ID_LoaiTuVan = obj.ID_LoaiTuVan;
                    objUpd.PhanLoai = 3;
                    objUpd.NhacNho = obj.NhacNho;
                    objUpd.MucDoPhanHoi = 1;
                    objUpd.NguoiTao = obj.NguoiTao;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NoiDung = obj.NoiDung;
                    objUpd.TraLoi = obj.TraLoi;
                    objUpd.NgayGio = obj.NgayGio;
                    objUpd.NgayGioKetThuc = obj.NgayGioKetThuc;
                    objUpd.NgayTao = objUpd.NgayTao;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.TrangThai = obj.TrangThai;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    //
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion
        #region delete
        string CheckDelete_LichHen(SsoftvnContext db, ChamSocKhachHang obj)
        {
            string strCheck = string.Empty;

            return strCheck;
        }
        #endregion

        public string Delete_LichHen(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                ChamSocKhachHang objDel = db.ChamSocKhachHang.Find(id);
                if (objDel != null)
                {
                    string strCheck = CheckDelete_LichHen(db, objDel);
                    if (strCheck == string.Empty)
                    {
                        try
                        {
                            db.ChamSocKhachHang.Remove(objDel);
                            db.SaveChanges();
                        }
                        catch (Exception exxx)
                        {
                            strErr = exxx.Message;
                            return strErr;
                        }
                    }
                    else
                    {
                        strErr = strCheck;
                        return strErr;
                    }
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }
    }

    public class SP_Calendar
    {
        public Guid ID { get; set; }
        public Guid? ID_KhachHang { get; set; }
        public Guid? ID_LoaiTuVan { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_NhanVienQuanLy { get; set; }
        public DateTime NgayGio { get; set; }
        public DateTime? NgayGioKetThuc { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public bool? CaNgay { get; set; }
        public string Ma_TieuDe { get; set; }
        public int LoaiDoiTuong { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string TenNguonKhach { get; set; }
        public string TenNhomDoiTuongs { get; set; }
        public string IDNhomDoiTuongs { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
        public string NoiDung { get; set; }
        public string TenLoaiTuVanLichHen { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public int NhacNho { get; set; }
        public int? KieuNhacNho { get; set; }
        public string KetQua { get; set; }
        public int MucDoUuTien { get; set; }
        public int PhanLoai { get; set; }
        public int KieuLap { get; set; }
        public int SoLanLap { get; set; }
        public string GiaTriLap { get; set; }
        public int TuanLap { get; set; }
        public int TrangThaiKetThuc { get; set; }
        public string GiaTriKetThuc { get; set; }
        public string color { get; set; }
        public bool ExistDB { get; set; }
        public Guid? ID_Parent { get; set; }
        public DateTime? NgayCu { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public Guid id
        {
            get { return ID; }
        }
        public string title
        {
            get { return Ma_TieuDe; }
        }
        public DateTime start
        {
            get { return NgayGio; }
        }
        public DateTime? end
        {
            get { return NgayGioKetThuc; }
        }
        public bool? allDay
        {
            get { return CaNgay; }
        }
    }

    public class Event_ParamUpdate
    {
        public Guid ID { get; set; }
        public string SqlSet { get; set; }
    }

    public class ParamCalendar
    {
        public string[] ID_DonVis { get; set; }
        public string[] IDLoaiTuVans { get; set; }
        public string[] IDNhanVienPhuTrachs { get; set; }
        public string[] IDNhanVienPhoiHops { get; set; }
        public string[] TrangThaiCVs { get; set; }
        public string PhanLoai { get; set; }// congviec, lichhen
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string MucDoUuTien { get; set; }
        public string LoaiDoiTuong { get; set; }
        public string TextSearch { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string ColumnsHide { get; set; }
        public string TenChiNhanhs { get; set; }// luoi/ calendar
        public int TypeShow { get; set; }// luoi/ calendar
        public string ID_KhachHang { get; set; } // use at page KH/NCC
        public string IDNhomKH { get; set; } // search by nhom
    }

    public class Calendar_DichVu
    {
        public Guid ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
    }
    public class EmailModel
    {
        public Guid? ID_NguoiGui { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_KhachHang { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public string TenDoiTuong { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public int? LoaiTinNhan { get; set; }
    }
}
