
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Office.Interop;
using libHT_NguoiDung;
using static libQuy_HoaDon.Class_Report;
using libDM_DoiTuong;
using libDM_HangHoa;
using Aspose.Cells;

namespace libQuy_HoaDon
{
    public class ClassXuatHuy
    {
        private SsoftvnContext db;

        public ClassXuatHuy(SsoftvnContext _db)
        {
            db = _db;
        }
        // lọc dấu
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
        public HT_NguoiDung Select_NguoiDung(Guid id)
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
        public HT_NguoiDung getAllNguoidung()
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NguoiDung.Find();
            }
        }
        // Tìm kiếm theo mã phiếu xuất hủy     
        public List<BC_XuatHuy> seachMaXH(string giatriSeach, int loaihoadon, Guid IDchinhanh)
        {
            var tbl = from hd in db.BH_HoaDon
                      join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                      from hd_dt in HD_DT.DefaultIfEmpty()
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                      from hd_dv in HD_DV.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                      from hd_nv in HD_NV.DefaultIfEmpty()
                      join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                      from hd_vt in HD_VT.DefaultIfEmpty()
                      join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                      from hd_bg in HD_BG.DefaultIfEmpty()
                      where hd.LoaiHoaDon == loaihoadon & hd.MaHoaDon.Contains(@giatriSeach) /*& hd_dv.ID == IDchinhanh*/
                      orderby hd.NgayLapHoaDon descending
                      select new
                      {
                          ID = hd.ID,
                          MaHoaDon = hd.MaHoaDon,
                          TenDonVi = hd_dv.TenDonVi,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TongGiamGia = hd.TongGiamGia,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          ID_NhanVien = hd.ID_NhanVien,
                          TenNhanVien = hd_nv.TenNhanVien,
                          TenDoiTuong = hd_dt.TenDoiTuong,
                          DienGiai = hd.DienGiai,
                          Email = hd_dt.Email,
                          DienThoai = hd_dt.DienThoai,
                          TenPhongBan = hd_vt.TenViTri,
                          NguoiTaoHD = hd.NguoiTao,
                          TenBangGia = hd_bg.TenGiaBan,
                          ChoThanhToan = hd.ChoThanhToan,
                          // trạng thái lưu vào trường yêu cầu
                          YeuCau = hd.YeuCau,
                          ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                      };
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            foreach (var item in tbl)
            {
                BC_XuatHuy LH_lst = new BC_XuatHuy();
                LH_lst.ID = item.ID;
                LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                LH_lst.ID_NhanVien = item.ID_NhanVien;
                LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                LH_lst.TenDonVi = item.TenDonVi;
                LH_lst.TenNhanVien = item.TenNhanVien;
                LH_lst.TongTienHang = item.TongTienHang;
                LH_lst.YeuCau = item.YeuCau;
                LH_lst.MaHoaDon = item.MaHoaDon;
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

        // Tìm kiếm theo người xuất hủy   
        public List<BC_XuatHuy> seachIDNhanVien(string giatriSeach, int loaihoadon, Guid IDchinhanh)
        {
            var tbl = from hd in db.BH_HoaDon
                      join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                      from hd_dt in HD_DT.DefaultIfEmpty()
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                      from hd_dv in HD_DV.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                      from hd_nv in HD_NV.DefaultIfEmpty()
                      join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                      from hd_vt in HD_VT.DefaultIfEmpty()
                      join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                      from hd_bg in HD_BG.DefaultIfEmpty()
                      where hd.LoaiHoaDon == loaihoadon & hd_nv.ID.ToString().Contains(@giatriSeach) /*& hd_dv.ID == IDchinhanh*/
                      orderby hd.NgayLapHoaDon descending
                      select new
                      {
                          ID = hd.ID,
                          MaHoaDon = hd.MaHoaDon,
                          TenDonVi = hd_dv.TenDonVi,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TongGiamGia = hd.TongGiamGia,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          ID_NhanVien = hd.ID_NhanVien,
                          TenNhanVien = hd_nv.TenNhanVien,
                          TenDoiTuong = hd_dt.TenDoiTuong,
                          DienGiai = hd.DienGiai,
                          Email = hd_dt.Email,
                          DienThoai = hd_dt.DienThoai,
                          TenPhongBan = hd_vt.TenViTri,
                          NguoiTaoHD = hd.NguoiTao,
                          TenBangGia = hd_bg.TenGiaBan,
                          ChoThanhToan = hd.ChoThanhToan,
                          // trạng thái lưu vào trường yêu cầu
                          YeuCau = hd.YeuCau,
                          ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                      };
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            foreach (var item in tbl)
            {
                BC_XuatHuy LH_lst = new BC_XuatHuy();
                LH_lst.ID = item.ID;
                LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                LH_lst.ID_NhanVien = item.ID_NhanVien;
                LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                LH_lst.TenDonVi = item.TenDonVi;
                LH_lst.TenNhanVien = item.TenNhanVien;
                LH_lst.TongTienHang = item.TongTienHang;
                LH_lst.YeuCau = item.YeuCau;
                LH_lst.MaHoaDon = item.MaHoaDon;
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
        // Tìm kiếm theo người tạo  
        public List<BC_XuatHuy> seachTenND(string giatriSeach, int loaihoadon, Guid IDchinhanh)
        {
            var tbl = from hd in db.BH_HoaDon
                      join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                      from hd_dt in HD_DT.DefaultIfEmpty()
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                      from hd_dv in HD_DV.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                      from hd_nv in HD_NV.DefaultIfEmpty()
                      join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                      from hd_vt in HD_VT.DefaultIfEmpty()
                      join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                      from hd_bg in HD_BG.DefaultIfEmpty()
                      where hd.LoaiHoaDon == loaihoadon & hd.NguoiTao.Contains(@giatriSeach) /*& hd_dv.ID == IDchinhanh*/
                      orderby hd.NgayLapHoaDon descending
                      select new
                      {
                          ID = hd.ID,
                          MaHoaDon = hd.MaHoaDon,
                          TenDonVi = hd_dv.TenDonVi,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TongGiamGia = hd.TongGiamGia,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          ID_NhanVien = hd.ID_NhanVien,
                          TenNhanVien = hd_nv.TenNhanVien,
                          TenDoiTuong = hd_dt.TenDoiTuong,
                          DienGiai = hd.DienGiai,
                          Email = hd_dt.Email,
                          DienThoai = hd_dt.DienThoai,
                          TenPhongBan = hd_vt.TenViTri,
                          NguoiTaoHD = hd.NguoiTao,
                          TenBangGia = hd_bg.TenGiaBan,
                          ChoThanhToan = hd.ChoThanhToan,
                          YeuCau = hd.YeuCau,
                          ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                      };
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            foreach (var item in tbl)
            {
                BC_XuatHuy LH_lst = new BC_XuatHuy();
                LH_lst.ID = item.ID;
                LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                LH_lst.ID_NhanVien = item.ID_NhanVien;
                LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                LH_lst.TenDonVi = item.TenDonVi;
                LH_lst.TenNhanVien = item.TenNhanVien;
                LH_lst.TongTienHang = item.TongTienHang;
                LH_lst.YeuCau = item.YeuCau;
                LH_lst.MaHoaDon = item.MaHoaDon;
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
        // Tìm kiếm theo hàng hóa
        public List<BC_XuatHuy> seachMaHH(string giatriSeach, int loaihoadon, Guid IDchinhanh)
        {
            var tbl = from hd in db.BH_HoaDon
                      join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                      from hd_dt in HD_DT.DefaultIfEmpty()
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon into HD_CT
                      from hd_ct in HD_CT.DefaultIfEmpty()
                      join dvqd in db.DonViQuiDois on hd_ct.ID_DonViQuiDoi equals dvqd.ID into HD_QD
                      from dv_qd in HD_QD.DefaultIfEmpty()
                      join hh in db.DM_HangHoa on dv_qd.ID_HangHoa equals hh.ID into QD_HH
                      from hhqd in QD_HH.DefaultIfEmpty()
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                      from hd_dv in HD_DV.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                      from hd_nv in HD_NV.DefaultIfEmpty()
                      join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                      from hd_vt in HD_VT.DefaultIfEmpty()
                      join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                      from hd_bg in HD_BG.DefaultIfEmpty()
                      where hd.LoaiHoaDon == loaihoadon /*& hd_dv.ID == IDchinhanh*/ & (hhqd.TenHangHoa.Contains(@giatriSeach) || dv_qd.MaHangHoa.Contains(@giatriSeach))
                      orderby hd.NgayLapHoaDon descending
                      select new
                      {
                          ID = hd.ID,
                          MaHoaDon = hd.MaHoaDon,
                          TenDonVi = hd_dv.TenDonVi,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TongGiamGia = hd.TongGiamGia,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          ID_NhanVien = hd.ID_NhanVien,
                          TenNhanVien = hd_nv.TenNhanVien,
                          TenDoiTuong = hd_dt.TenDoiTuong,
                          DienGiai = hd.DienGiai,
                          Email = hd_dt.Email,
                          DienThoai = hd_dt.DienThoai,
                          TenPhongBan = hd_vt.TenViTri,
                          NguoiTaoHD = hd.NguoiTao,
                          TenBangGia = hd_bg.TenGiaBan,
                          ChoThanhToan = hd.ChoThanhToan,
                          // trạng thái lưu vào trường yêu cầu
                          YeuCau = hd.YeuCau,
                          ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                      };
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            foreach (var item in tbl)
            {
                BC_XuatHuy LH_lst = new BC_XuatHuy();
                LH_lst.ID = item.ID;
                LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                LH_lst.ID_NhanVien = item.ID_NhanVien;
                LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                LH_lst.TenDonVi = item.TenDonVi;
                LH_lst.TenNhanVien = item.TenNhanVien;
                LH_lst.TongTienHang = item.TongTienHang;
                LH_lst.YeuCau = item.YeuCau;
                LH_lst.MaHoaDon = item.MaHoaDon;
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
        // tìm kiem full
        public List<BC_XuatHuy> getListXuatHuy(string maXH, string hangHoa, string nhanvien, string NguoiTao, int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            string[] mang = chinhanh.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            var tbl = from hd in db.BH_HoaDon.Where(x => LstIS.Contains(x.ID_DonVi.ToString()))
                      join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on hdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into n
                      from nv_hd in n.DefaultIfEmpty()
                      where hd.LoaiHoaDon == loaihoadon & hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd & (hd.YeuCau.Contains(trangthai1) || hd.YeuCau.Contains(trangthai2) || hd.YeuCau.Contains(trangthai3))
                      select new
                      {
                          ID = hd.ID,
                          ID_NhanVien = hd.ID_NhanVien,
                          MaHoaDon = hd.MaHoaDon,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TenChiNhanh = dv.TenDonVi,
                          TongTienHang = hd.TongTienHang,
                          DienGiai = hd.DienGiai,
                          YeuCau = hd.YeuCau,
                          MaHangHoa = dvqd.MaHangHoa,
                          TenHangHoa = hh.TenHangHoa,
                          NguoiTao = hd.NguoiTao,
                          TenNhanVien = nv_hd.TenNhanVien != null ? nv_hd.TenNhanVien : "",
                          ChoThanhToan = hd.ChoThanhToan
                      };
            var tbl_format = tbl.AsEnumerable().Select(t => new
            {
                ID = t.ID,
                ID_NhanVien = t.ID_NhanVien,
                MaHoaDon = t.MaHoaDon,
                NgayLapHoaDon = t.NgayLapHoaDon,
                TenDonVi = t.TenChiNhanh,
                TongTienHang = Math.Round(t.TongTienHang, 0, MidpointRounding.ToEven),
                DienGiai = t.DienGiai,
                YeuCau = t.YeuCau,
                MaHangHoa = t.MaHangHoa,
                TenHangHoa_GC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                TenHangHoa_CV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                NguoiTaoHD = t.NguoiTao,
                TenNhanVien = t.TenNhanVien,
                ChoThanhToan = t.ChoThanhToan
            });
            if (maXH != "null" & maXH != "" & maXH != null)
            {
                tbl_format = tbl_format.Where(q => q.MaHoaDon.Contains(@maXH));
            }
            if (nhanvien != "null" & nhanvien != "" & nhanvien != null)
            {
                tbl_format = tbl_format.Where(q => q.TenNhanVien.Contains(@nhanvien));
            }
            if (NguoiTao != "null" & NguoiTao != "" & NguoiTao != null)
            {
                tbl_format = tbl_format.Where(q => q.NguoiTaoHD.Contains(@NguoiTao));
            }
            if (hangHoa != "null" & hangHoa != "" & hangHoa != null)
            {
                hangHoa = CommonStatic.ConvertToUnSign(hangHoa).ToLower();
                tbl_format = tbl_format.Where(q => q.MaHangHoa.Contains(@hangHoa) || q.TenHangHoa_CV.Contains(@hangHoa) || q.TenHangHoa_GC.Contains(@hangHoa));
            }
            var tbl_Gop = from tb in tbl_format
                          group tb by new
                          {
                              tb.MaHoaDon
                          } into g
                          select new BC_XuatHuy
                          {
                              ID = g.FirstOrDefault().ID,
                              MaHoaDon = g.Key.MaHoaDon,
                              TenNhanVien = g.FirstOrDefault().TenNhanVien,
                              NgayLapHoaDon = g.FirstOrDefault().NgayLapHoaDon,
                              TenDonVi = g.FirstOrDefault().TenDonVi,
                              TongTienHang = g.FirstOrDefault().TongTienHang,
                              DienGiai = g.FirstOrDefault().DienGiai,
                              YeuCau = g.FirstOrDefault().YeuCau,
                              ChoThanhToan = g.FirstOrDefault().ChoThanhToan,
                              NguoiTaoHD = g.FirstOrDefault().NguoiTaoHD,
                              ID_NhanVien = g.FirstOrDefault().ID_NhanVien,
                          };
            try
            {
                lst = tbl_Gop.OrderByDescending(x => x.NgayLapHoaDon).ToList();
            }
            catch
            {

            }
            return lst;
        }
        public List<BC_XuatHuy> getAllXH(string maXH, string hangHoa, string nhanvien, string NguoiTao, int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            string[] mang = chinhanh.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            if (hangHoa == "null" || hangHoa == "" || hangHoa == null)
            {
                var tbl = from hd in db.BH_HoaDon
                          join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join hd_dv in db.DM_DonVi.Where(p => LstIS.Contains(p.ID.ToString())) on hd.ID_DonVi equals hd_dv.ID /*into HD_DV */
                          //from hd_dv in HD_DV.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                          from hd_vt in HD_VT.DefaultIfEmpty()
                          join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                          from hd_bg in HD_BG.DefaultIfEmpty()
                          where (hd.LoaiHoaDon == loaihoadon & hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd) & (hd.YeuCau.Contains(trangthai1) || hd.YeuCau.Contains(trangthai2) || hd.YeuCau.Contains(trangthai3))
                          orderby hd.NgayLapHoaDon descending
                          select new
                          {
                              ID = hd.ID,
                              MaHoaDon = hd.MaHoaDon,
                              TenDonVi = hd_dv.TenDonVi,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              TongGiamGia = hd.TongGiamGia,
                              TongTienHang = hd.TongTienHang,
                              PhaiThanhToan = hd.PhaiThanhToan,
                              ID_NhanVien = hd.ID_NhanVien,
                              TenNhanVien = hd_nv.TenNhanVien,
                              TenDoiTuong = hd_dt.TenDoiTuong,
                              DienGiai = hd.DienGiai,
                              Email = hd_dt.Email,
                              DienThoai = hd_dt.DienThoai,
                              TenPhongBan = hd_vt.TenViTri,
                              NguoiTaoHD = hd.NguoiTao,
                              TenBangGia = hd_bg.TenGiaBan,
                              ChoThanhToan = hd.ChoThanhToan,
                              YeuCau = hd.YeuCau,
                              ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty,
                          };
                if (maXH != "null" & maXH != "" & maXH != null)
                {
                    tbl = tbl.Where(q => q.MaHoaDon.Contains(@maXH));
                }
                if (nhanvien != "null" & nhanvien != "" & nhanvien != null)
                {
                    tbl = tbl.Where(q => q.TenNhanVien.ToString().Contains(@nhanvien));
                }
                if (NguoiTao != "null" & NguoiTao != "" & NguoiTao != null)
                {
                    tbl = tbl.Where(q => q.NguoiTaoHD.Contains(@NguoiTao));
                }
                List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
                foreach (var item in tbl)
                {
                    BC_XuatHuy LH_lst = new BC_XuatHuy();
                    LH_lst.ID = item.ID;
                    LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                    LH_lst.ID_NhanVien = item.ID_NhanVien;
                    LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                    LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                    LH_lst.TenDonVi = item.TenDonVi;
                    LH_lst.TenNhanVien = item.TenNhanVien;
                    LH_lst.TongTienHang = item.TongTienHang;
                    LH_lst.YeuCau = item.YeuCau;
                    LH_lst.MaHoaDon = item.MaHoaDon;
                    LH_lst.DienGiai = item.DienGiai;
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
            else
            {
                var tbl = from hd in db.BH_HoaDon
                          join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon into HD_CT
                          from hd_ct in HD_CT.DefaultIfEmpty()
                          join dvqd in db.DonViQuiDois on hd_ct.ID_DonViQuiDoi equals dvqd.ID into HD_QD
                          from dv_qd in HD_QD.DefaultIfEmpty()
                          join hh in db.DM_HangHoa on dv_qd.ID_HangHoa equals hh.ID into QD_HH
                          from hhqd in QD_HH.DefaultIfEmpty()
                          join hd_dv in db.DM_DonVi.Where(p => LstIS.Contains(p.TenDonVi)) on hd.ID_DonVi equals hd_dv.ID
                          //join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                          //from hd_dv in HD_DV.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                          from hd_vt in HD_VT.DefaultIfEmpty()
                          join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                          from hd_bg in HD_BG.DefaultIfEmpty()
                          where hd.LoaiHoaDon == loaihoadon & (hhqd.TenHangHoa.Contains(@hangHoa) || dv_qd.MaHangHoa.Contains(@hangHoa)) & (hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd) & (hd.YeuCau.Contains(trangthai1) || hd.YeuCau.Contains(trangthai2) || hd.YeuCau.Contains(trangthai3))
                          orderby hd.NgayLapHoaDon descending
                          select new
                          {
                              ID = hd.ID,
                              MaHoaDon = hd.MaHoaDon,
                              TenDonVi = hd_dv.TenDonVi,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              TongGiamGia = hd.TongGiamGia,
                              TongTienHang = hd.TongTienHang,
                              PhaiThanhToan = hd.PhaiThanhToan,
                              ID_NhanVien = hd.ID_NhanVien,
                              TenNhanVien = hd_nv.TenNhanVien,
                              TenDoiTuong = hd_dt.TenDoiTuong,
                              DienGiai = hd.DienGiai,
                              Email = hd_dt.Email,
                              DienThoai = hd_dt.DienThoai,
                              TenPhongBan = hd_vt.TenViTri,
                              NguoiTaoHD = hd.NguoiTao,
                              TenBangGia = hd_bg.TenGiaBan,
                              ChoThanhToan = hd.ChoThanhToan,
                              // trạng thái lưu vào trường yêu cầu
                              YeuCau = hd.YeuCau,
                              ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                          };
                if (maXH != "null" & maXH != "" & maXH != null)
                {
                    tbl = tbl.Where(q => q.MaHoaDon.Contains(@maXH));
                }
                if (nhanvien != "null" & nhanvien != "" & nhanvien != null)
                {
                    tbl = tbl.Where(q => q.TenNhanVien.ToString().Contains(@nhanvien));
                }
                if (NguoiTao != "null" & NguoiTao != "" & NguoiTao != null)
                {
                    tbl = tbl.Where(q => q.NguoiTaoHD.Contains(@NguoiTao));
                }
                List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
                foreach (var item in tbl)
                {
                    BC_XuatHuy LH_lst = new BC_XuatHuy();
                    LH_lst.ID = item.ID;
                    LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                    LH_lst.ID_NhanVien = item.ID_NhanVien;
                    LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                    LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                    LH_lst.TenDonVi = item.TenDonVi;
                    LH_lst.TenNhanVien = item.TenNhanVien;
                    LH_lst.TongTienHang = item.TongTienHang;
                    LH_lst.YeuCau = item.YeuCau;
                    LH_lst.MaHoaDon = item.MaHoaDon;
                    LH_lst.DienGiai = item.DienGiai;
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
        }
        //load danh mục phiếu hủy
        public List<BC_XuatHuy> getAllPhieuHuy(int loaihoadon, DateTime dayStart, DateTime dayEnd)
        {
            var tbl = from hd in db.BH_HoaDon
                      join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                      from hd_dt in HD_DT.DefaultIfEmpty()
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                      from hd_dv in HD_DV.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                      from hd_nv in HD_NV.DefaultIfEmpty()
                      join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                      from hd_vt in HD_VT.DefaultIfEmpty()
                      join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                      from hd_bg in HD_BG.DefaultIfEmpty()
                      where hd.LoaiHoaDon == loaihoadon & hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd
                      orderby hd.NgayLapHoaDon descending
                      select new
                      {
                          ID = hd.ID,
                          MaHoaDon = hd.MaHoaDon,
                          TenDonVi = hd_dv.TenDonVi,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TongGiamGia = hd.TongGiamGia,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          ID_NhanVien = hd.ID_NhanVien,
                          TenNhanVien = hd_nv.TenNhanVien,
                          TenDoiTuong = hd_dt.TenDoiTuong,
                          DienGiai = hd.DienGiai,
                          Email = hd_dt.Email,
                          DienThoai = hd_dt.DienThoai,
                          TenPhongBan = hd_vt.TenViTri,
                          NguoiTaoHD = hd.NguoiTao,
                          TenBangGia = hd_bg.TenGiaBan,
                          ChoThanhToan = hd.ChoThanhToan,
                          YeuCau = hd.YeuCau,
                          ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                      };
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            foreach (var item in tbl)
            {
                BC_XuatHuy LH_lst = new BC_XuatHuy();
                LH_lst.ID = item.ID;
                LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                LH_lst.ID_NhanVien = item.ID_NhanVien;
                LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                LH_lst.TenDonVi = item.TenDonVi;
                LH_lst.TenNhanVien = item.TenNhanVien;
                LH_lst.TongTienHang = item.TongTienHang;
                LH_lst.YeuCau = item.YeuCau;
                LH_lst.MaHoaDon = item.MaHoaDon;
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
        //load phiếu hủy theo trạng thái
        public List<BC_XuatHuy> getAllTrangThai(int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3)
        {
            var tbl = from hd in db.BH_HoaDon
                      join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                      from hd_dt in HD_DT.DefaultIfEmpty()
                      join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                      from hd_dv in HD_DV.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                      from hd_nv in HD_NV.DefaultIfEmpty()
                      join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                      from hd_vt in HD_VT.DefaultIfEmpty()
                      join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                      from hd_bg in HD_BG.DefaultIfEmpty()
                      where (hd.LoaiHoaDon == loaihoadon & hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd) & (hd.YeuCau.Contains(trangthai1) || hd.YeuCau.Contains(trangthai2) || hd.YeuCau.Contains(trangthai3))
                      orderby hd.NgayLapHoaDon descending
                      select new
                      {
                          ID = hd.ID,
                          MaHoaDon = hd.MaHoaDon,
                          TenDonVi = hd_dv.TenDonVi,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TongGiamGia = hd.TongGiamGia,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          ID_NhanVien = hd.ID_NhanVien,
                          TenNhanVien = hd_nv.TenNhanVien,
                          TenDoiTuong = hd_dt.TenDoiTuong,
                          DienGiai = hd.DienGiai,
                          Email = hd_dt.Email,
                          DienThoai = hd_dt.DienThoai,
                          TenPhongBan = hd_vt.TenViTri,
                          NguoiTaoHD = hd.NguoiTao,
                          TenBangGia = hd_bg.TenGiaBan,
                          ChoThanhToan = hd.ChoThanhToan,
                          YeuCau = hd.YeuCau,
                          ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                      };
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            foreach (var item in tbl)
            {
                BC_XuatHuy LH_lst = new BC_XuatHuy();
                LH_lst.ID = item.ID;
                LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                LH_lst.ID_NhanVien = item.ID_NhanVien;
                LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                LH_lst.TenDonVi = item.TenDonVi;
                LH_lst.TenNhanVien = item.TenNhanVien;
                LH_lst.TongTienHang = item.TongTienHang;
                LH_lst.YeuCau = item.YeuCau;
                LH_lst.MaHoaDon = item.MaHoaDon;
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
        // load phiếu hủy theo chi nhánh
        public List<BC_XuatHuy> getAllChiNhanh(int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            //string a = ConvertToUnSign(giatriSeach).ToLower();
            string[] mang = chinhanh.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            var tbl = from hd in db.BH_HoaDon
                      join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                      from hd_dt in HD_DT.DefaultIfEmpty()
                      join hd_dv in db.DM_DonVi.Where(p => LstIS.Contains(p.ID.ToString())) on hd.ID_DonVi equals hd_dv.ID /*into HD_DV */
                      //from hd_dv in HD_DV.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                      from hd_nv in HD_NV.DefaultIfEmpty()
                      join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                      from hd_vt in HD_VT.DefaultIfEmpty()
                      join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                      from hd_bg in HD_BG.DefaultIfEmpty()
                      where (hd.LoaiHoaDon == loaihoadon & hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd) & (hd.YeuCau.Contains(trangthai1) || hd.YeuCau.Contains(trangthai2) || hd.YeuCau.Contains(trangthai3))
                      orderby hd.NgayLapHoaDon descending
                      select new
                      {
                          ID = hd.ID,
                          MaHoaDon = hd.MaHoaDon,
                          TenDonVi = hd_dv.TenDonVi,
                          NgayLapHoaDon = hd.NgayLapHoaDon,
                          TongGiamGia = hd.TongGiamGia,
                          TongTienHang = hd.TongTienHang,
                          PhaiThanhToan = hd.PhaiThanhToan,
                          ID_NhanVien = hd.ID_NhanVien,
                          TenNhanVien = hd_nv.TenNhanVien,
                          TenDoiTuong = hd_dt.TenDoiTuong,
                          DienGiai = hd.DienGiai,
                          Email = hd_dt.Email,
                          DienThoai = hd_dt.DienThoai,
                          TenPhongBan = hd_vt.TenViTri,
                          NguoiTaoHD = hd.NguoiTao,
                          TenBangGia = hd_bg.TenGiaBan,
                          ChoThanhToan = hd.ChoThanhToan,
                          YeuCau = hd.YeuCau,
                          ID_DoiTuong = hd.ID_DoiTuong ?? Guid.Empty
                      };
            List<BC_XuatHuy> lst = new List<BC_XuatHuy>();
            foreach (var item in tbl)
            {
                BC_XuatHuy LH_lst = new BC_XuatHuy();
                LH_lst.ID = item.ID;
                LH_lst.ID_DoiTuong = item.ID_DoiTuong;
                LH_lst.ID_NhanVien = item.ID_NhanVien;
                LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                LH_lst.NguoiTaoHD = item.NguoiTaoHD;
                LH_lst.TenDonVi = item.TenDonVi;
                LH_lst.TenNhanVien = item.TenNhanVien;
                LH_lst.TongTienHang = item.TongTienHang;
                LH_lst.YeuCau = item.YeuCau;
                LH_lst.MaHoaDon = item.MaHoaDon;
                LH_lst.DienGiai = item.DienGiai;
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
        public BH_HoaDon getlist_BH_HoaDon(Guid ID)
        {
            BH_HoaDon HD = db.BH_HoaDon.Where(x => x.ID == ID).FirstOrDefault();
            return HD;
        }
        // xóa hóa đơn
        string CheckDelete_HoaDon(SsoftvnContext db, BH_HoaDon obj)
        {
            string strCheck = string.Empty;

            List<CongDoan_DichVu> lstCongDoans = db.CongDoan_DichVu.Where(p => p.ID_CongDoan == obj.ID).ToList();
            if (lstCongDoans != null && lstCongDoans.Count > 0)
            {
                strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để lập danh mục công đoạn cho hàng hóa/dịch vụ khác.";
                return strCheck;
            }

            return strCheck;
        }
        public string Delete_HoaDon(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                BH_HoaDon objDel = db.BH_HoaDon.Find(id);
                if (objDel != null)
                {
                    string strCheck = CheckDelete_HoaDon(db, objDel);
                    if (strCheck == string.Empty)
                    {
                        try
                        {
                            // delete ChiTietHD
                            db.BH_HoaDon_ChiTiet.RemoveRange(db.BH_HoaDon_ChiTiet.Where(idHD => idHD.ID_HoaDon == id));
                            // remove CT_QuyHD, Quy_HD
                            Quy_HoaDon_ChiTiet qct = db.Quy_HoaDon_ChiTiet.Where(idHD => idHD.ID_HoaDonLienQuan == id).ToList().FirstOrDefault();
                            if (qct != null)
                            {
                                db.Quy_HoaDon_ChiTiet.RemoveRange(db.Quy_HoaDon_ChiTiet.Where(idHD => idHD.ID_HoaDonLienQuan == id));
                                db.Quy_HoaDon.RemoveRange(db.Quy_HoaDon.Where(idQHD => idQHD.ID == qct.ID_HoaDon));
                            }

                            db.BH_HoaDon.Remove(objDel);
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException exxx)
                        {
                            //strErr = exxx.Message;
                            //return strErr;

                            StringBuilder sb = new StringBuilder();
                            foreach (var eve in exxx.EntityValidationErrors)
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
                            throw new DbEntityValidationException(sb.ToString(), exxx);
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
        public string checkNgayChotSo(Guid? ID_ChiNhanh, Guid ID_HoaDon, DateTime StartTime, string LoaEdit)
        {
            string strER = string.Empty;
            if (db == null)
            {
                strER = "Kết nối CSDL không hợp lệ";
            }
            else
            {
                var tb = from cs in db.ChotSo
                         where cs.ID_DonVi == ID_ChiNhanh
                         select new
                         {
                             cs.NgayChotSo
                         };
                var tbhd = from hd in db.BH_HoaDon
                           where hd.ID == ID_HoaDon
                           select new
                           {
                               hd.NgayLapHoaDon
                           };
                if (tb != null && tb.Count() > 0)
                {
                    string time = tb.FirstOrDefault().NgayChotSo.ToString("dd/MM/yyyy");
                    if (tb.FirstOrDefault().NgayChotSo > StartTime)
                        strER = "Bạn không thể " + LoaEdit + " phiếu xuất hủy về thời gian giao dịch trước ngày khóa sổ " + time;
                }
            }
            return strER;
        }
        public string Update_HDChuyenHang(BH_HoaDon obj)
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
                    #region BH_HoaDon
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.MaHoaDon = obj.MaHoaDon;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.DienGiai = obj.DienGiai;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.YeuCau = obj.YeuCau;
                    objUpd.ChoThanhToan = obj.ChoThanhToan;
                    objUpd.TongTienHang = obj.TongTienHang;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        public string Update_ChiTietHoaDon(BH_HoaDon_ChiTiet obj, int k)
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
                    #region BH_HoaDon_ChiTiet
                    BH_HoaDon_ChiTiet objUpd = db.BH_HoaDon_ChiTiet.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.SoLuong = obj.SoLuong;
                    objUpd.GiaVon = obj.GiaVon;
                    objUpd.ThanhTien = obj.ThanhTien;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    try
                    {
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = obj.ID_DonViQuiDoi,
                            DonGia = 0,
                            ID_HoaDon = obj.ID_HoaDon,
                            SoLuong = obj.SoLuong,
                            ThanhTien = obj.ThanhTien,
                            TienChietKhau = 0,
                            GiaVon = obj.GiaVon,
                            SoThuTu = k,
                            ChatLieu = "",
                            MauSac = "",
                            KichCo = "",
                            PTChietKhau = 0,
                            TienThue = 0,
                            PTChiPhi = 0,
                            TienChiPhi = 0,
                            ThanhToan = 0,
                            An_Hien = true
                        };
                        Add_ChiTietHoaDon(ctHoaDon);
                    }
                    catch
                    {
                        strErr = ex.Message;
                    }
                }
            }
            return strErr;
        }

        public string Add_ChiTietHoaDon(BH_HoaDon_ChiTiet objAdd)
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
                    db.BH_HoaDon_ChiTiet.Add(objAdd);
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

        // xuất báo cáo
        public List<BC_XuatHuy_Excel> getXuatHuy_Excel(string maXH, string hangHoa, string nhanvien, string NguoiTao, int loaihoadon, DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, string chinhanh)
        {
            string[] mang = chinhanh.Split(',');
            List<string> LstIS = new List<string>();
            for (int i = 0; i < mang.Length; i++)
            {
                LstIS.Add(mang[i].ToString());
            }
            if (hangHoa == "null" || hangHoa == "" || hangHoa == null)
            {
                var tbl = from hd in db.BH_HoaDon
                          join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join hd_dv in db.DM_DonVi.Where(p => LstIS.Contains(p.ID.ToString())) on hd.ID_DonVi equals hd_dv.ID /*into HD_DV */
                          //from hd_dv in HD_DV.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                          from hd_vt in HD_VT.DefaultIfEmpty()
                          join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                          from hd_bg in HD_BG.DefaultIfEmpty()
                          where (hd.LoaiHoaDon == loaihoadon & hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd) & (hd.YeuCau.Contains(trangthai1) || hd.YeuCau.Contains(trangthai2) || hd.YeuCau.Contains(trangthai3))
                          orderby hd.NgayLapHoaDon descending
                          select new
                          {
                              ID = hd.ID,
                              MaHoaDon = hd.MaHoaDon,
                              TenDonVi = hd_dv.TenDonVi,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              TongTienHang = hd.TongTienHang,
                              TenNhanVien = hd_nv.TenNhanVien,
                              DienGiai = hd.DienGiai,
                              NguoiTaoHD = hd.NguoiTao,
                              YeuCau = hd.YeuCau,
                          };
                if (maXH != "null" & maXH != "" & maXH != null)
                {
                    tbl = tbl.Where(q => q.MaHoaDon.Contains(@maXH));
                }
                if (nhanvien != "null" & nhanvien != "" & nhanvien != null)
                {
                    tbl = tbl.Where(q => q.TenNhanVien.ToString().Contains(@nhanvien));
                }
                if (NguoiTao != "null" & NguoiTao != "" & NguoiTao != null)
                {
                    tbl = tbl.Where(q => q.NguoiTaoHD.Contains(@NguoiTao));
                }
                List<BC_XuatHuy_Excel> lst = new List<BC_XuatHuy_Excel>();
                foreach (var item in tbl)
                {
                    BC_XuatHuy_Excel LH_lst = new BC_XuatHuy_Excel();
                    LH_lst.MaHoaDon = item.MaHoaDon;
                    LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                    LH_lst.TenDonVi = item.TenDonVi;
                    LH_lst.TongTienHang = item.TongTienHang;
                    LH_lst.DienGiai = item.DienGiai;
                    LH_lst.YeuCau = item.YeuCau;
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
            else
            {
                var tbl = from hd in db.BH_HoaDon
                          join kh in db.DM_DoiTuong on hd.ID_DoiTuong equals kh.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join hdct in db.BH_HoaDon_ChiTiet on hd.ID equals hdct.ID_HoaDon into HD_CT
                          from hd_ct in HD_CT.DefaultIfEmpty()
                          join dvqd in db.DonViQuiDois on hd_ct.ID_DonViQuiDoi equals dvqd.ID into HD_QD
                          from dv_qd in HD_QD.DefaultIfEmpty()
                          join hh in db.DM_HangHoa on dv_qd.ID_HangHoa equals hh.ID into QD_HH
                          from hhqd in QD_HH.DefaultIfEmpty()
                          join hd_dv in db.DM_DonVi.Where(p => LstIS.Contains(p.TenDonVi)) on hd.ID_DonVi equals hd_dv.ID
                          //join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID into HD_DV
                          //from hd_dv in HD_DV.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on hd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          join vt in db.DM_ViTri on hd.ID_ViTri equals vt.ID into HD_VT
                          from hd_vt in HD_VT.DefaultIfEmpty()
                          join bg in db.DM_GiaBan on hd.ID_BangGia equals bg.ID into HD_BG
                          from hd_bg in HD_BG.DefaultIfEmpty()
                          where hd.LoaiHoaDon == loaihoadon & (hhqd.TenHangHoa.Contains(@hangHoa) || dv_qd.MaHangHoa.Contains(@hangHoa)) & (hd.NgayLapHoaDon >= dayStart & hd.NgayLapHoaDon < dayEnd) & (hd.YeuCau.Contains(trangthai1) || hd.YeuCau.Contains(trangthai2) || hd.YeuCau.Contains(trangthai3))
                          orderby hd.NgayLapHoaDon descending
                          select new
                          {
                              ID = hd.ID,
                              MaHoaDon = hd.MaHoaDon,
                              TenDonVi = hd_dv.TenDonVi,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              TongTienHang = hd.TongTienHang,
                              TenNhanVien = hd_nv.TenNhanVien,
                              DienGiai = hd.DienGiai,
                              NguoiTaoHD = hd.NguoiTao,
                              YeuCau = hd.YeuCau,
                          };
                if (maXH != "null" & maXH != "" & maXH != null)
                {
                    tbl = tbl.Where(q => q.MaHoaDon.Contains(@maXH));
                }
                if (nhanvien != "null" & nhanvien != "" & nhanvien != null)
                {
                    tbl = tbl.Where(q => q.TenNhanVien.ToString().Contains(@nhanvien));
                }
                if (NguoiTao != "null" & NguoiTao != "" & NguoiTao != null)
                {
                    tbl = tbl.Where(q => q.NguoiTaoHD.Contains(@NguoiTao));
                }
                List<BC_XuatHuy_Excel> lst = new List<BC_XuatHuy_Excel>();
                foreach (var item in tbl)
                {
                    BC_XuatHuy_Excel LH_lst = new BC_XuatHuy_Excel();
                    LH_lst.MaHoaDon = item.MaHoaDon;
                    LH_lst.NgayLapHoaDon = item.NgayLapHoaDon;
                    LH_lst.TenDonVi = item.TenDonVi;
                    LH_lst.TongTienHang = item.TongTienHang;
                    LH_lst.DienGiai = item.DienGiai;
                    LH_lst.YeuCau = item.YeuCau;
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
        }

        public System.Data.DataTable ToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (var item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        // xuất dữ liệu sử dụng Aspose.cell
        public void listToOfficeExcel(string strFileTemplatePath, string exportPath, System.Data.DataTable tblDuLieu, int sourceRowIndex, int destinationRowIndex, int rowNumber)
        {
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook(strFileTemplatePath);
            Aspose.Cells.Worksheet wSheet = wbook.Worksheets[0];
            // định dạng
            int dkrange = (tblDuLieu.Rows.Count) / rowNumber;
            if (dkrange > 1)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, destinationRowIndex + 1, tblDuLieu.Rows.Count + sourceRowIndex + 2, 3);
            }
            for (int i = 1; i < dkrange; i++)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (rowNumber * i) + sourceRowIndex, rowNumber);
            }
            if (dkrange * rowNumber < tblDuLieu.Rows.Count)
            {
                wSheet.Cells.CopyRows(wSheet.Cells, sourceRowIndex, (dkrange * rowNumber) + sourceRowIndex, tblDuLieu.Rows.Count - dkrange * rowNumber);
            }
            //wSheet.Cells.ImportDataTable(tblDuLieu, false, sourceRowIndex, 0, false);
            ImportTableOptions importTableOptions = new ImportTableOptions();
            wSheet.Cells.ImportData(tblDuLieu, sourceRowIndex, 0, importTableOptions);

            wbook.Save(exportPath, Aspose.Cells.SaveFormat.Xlsx);
        }

        public void exportDataToExcel(List<BC_XuatHuy_Excel> lst, string pathExel, string fileName)
        {
            System.Data.DataTable excel = new System.Data.DataTable();
            string strPathFile = pathExel + @"\" + fileName;
            string exportPathFile = pathExel + @"\" + "DanhSachXuatHuy.xlsx";
            excel = ToDataTable<BC_XuatHuy_Excel>(lst);
            listToOfficeExcel(strPathFile, exportPathFile, excel, 3, 27, 24);
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

        public List<jqAutoResult_HangHoa> getListHangHoa_XNT(string maHH, Guid ID_ChiNhanh)
        {
            List<Report_HangHoa_XuatNhapTon_Union> lst = new List<Report_HangHoa_XuatNhapTon_Union>();
            //
            if (maHH != null & maHH != "" & maHH != "null")
            {

                var tbl_timeStart = from cs in db.ChotSo
                                    where cs.ID_DonVi == ID_ChiNhanh
                                    select new
                                    {
                                        cs.NgayChotSo
                                    };
                DateTime timeStart = DateTime.Parse("2018-01-01");
                try
                {
                    timeStart = tbl_timeStart.FirstOrDefault().NgayChotSo;
                }
                catch
                {

                }
                string maTolower = CommonStatic.ConvertToUnSign(maHH).ToLower();
                string maToUpper = CommonStatic.ConvertToUnSign(maHH).ToUpper();
                var tbl_HH = (from dvqd in db.DonViQuiDois
                              join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                              where hh.TenHangHoa_KhongDau.Contains(maTolower) || hh.TenHangHoa_KyTuDau.Contains(maTolower) || dvqd.MaHangHoa.Contains(maTolower) || dvqd.MaHangHoa.Contains(maToUpper)
                              select new
                              {
                                  dvqd.ID,
                                  dvqd.ID_HangHoa,
                                  dvqd.MaHangHoa,
                                  dvqd.GiaVon,
                                  hh.TenHangHoa,
                              }).Take(10);
                var tbl1 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           (bhd.LoaiHoaDon == 1 || bhd.LoaiHoaDon == 5 || bhd.LoaiHoaDon == 7 || bhd.LoaiHoaDon == 8)
                           & dvqd.Xoa == null //& (hh.TenHangHoa_KhongDau.Contains(maTolower) || hh.TenHangHoa_KyTuDau.Contains(maTolower) || dvqd.MaHangHoa.Contains(maTolower) || dvqd.MaHangHoa.Contains(maToUpper))
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g
                           select new
                           {
                               ID_HangHoa = g.Key.ID_HangHoa,
                               SoLuongNhap = 0,
                               SoLuongXuat = g.Sum(x => (double?)x.bhdct.SoLuong * x.dvqd.TyLeChuyenDoi ?? 0),
                           };
                var tbl2 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           ((bhd.LoaiHoaDon == 10 & bhd.YeuCau == "1") || (bhd.ID_CheckIn != null & bhd.ID_CheckIn != ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4"))
                           & dvqd.Xoa == null //& (hh.TenHangHoa_KhongDau.Contains(maTolower) || hh.TenHangHoa_KyTuDau.Contains(maTolower) || dvqd.MaHangHoa.Contains(maTolower) || dvqd.MaHangHoa.Contains(maToUpper))
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g1
                           select new
                           {
                               ID_HangHoa = g1.Key.ID_HangHoa,
                               SoLuongNhapCHuyen = 0,
                               SoLuongXuatChuyen = g1.Sum(x => (double?)x.bhdct.TienChietKhau * x.dvqd.TyLeChuyenDoi ?? 0),
                           };
                var tbl3 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           (bhd.LoaiHoaDon == 4 || bhd.LoaiHoaDon == 6 || bhd.LoaiHoaDon == 9)
                           & dvqd.Xoa == null //& (hh.TenHangHoa_KhongDau.Contains(maTolower) || hh.TenHangHoa_KyTuDau.Contains(maTolower) || dvqd.MaHangHoa.Contains(maTolower) || dvqd.MaHangHoa.Contains(maToUpper))
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g2
                           select new
                           {
                               ID_HangHoa = g2.Key.ID_HangHoa,
                               SoLuongXuat1 = 0,
                               SoLuongNhap1 = g2.Sum(x => (double?)x.bhdct.SoLuong * x.dvqd.TyLeChuyenDoi ?? 0),
                           };
                var tbl4 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           (bhd.ID_CheckIn != null & bhd.ID_CheckIn == ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4")
                           & dvqd.Xoa == null //& (hh.TenHangHoa_KhongDau.Contains(maTolower) || hh.TenHangHoa_KyTuDau.Contains(maTolower) || dvqd.MaHangHoa.Contains(maTolower) || dvqd.MaHangHoa.Contains(maToUpper))
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g3
                           select new
                           {
                               ID_HangHoa = g3.Key.ID_HangHoa,
                               SoLuongXuatchuyen1 = 0,
                               SoLuongNhapchuyen1 = g3.Sum(x => (double?)x.bhdct.TienChietKhau * x.dvqd.TyLeChuyenDoi ?? 0),
                           };

                var tbl_fomat = from tb in tbl_HH
                                join dvqd in db.DonViQuiDois on tb.ID equals dvqd.ID
                                //join tb1 in tbl1 on tb.ID equals tb1.ID_DonViQuiDoi into t
                                join tb1 in tbl1 on tb.ID_HangHoa equals tb1.ID_HangHoa into t
                                from b1 in t.DefaultIfEmpty()
                                join tb2 in tbl2 on tb.ID_HangHoa equals tb2.ID_HangHoa into t2
                                from b2 in t2.DefaultIfEmpty()
                                join tb3 in tbl3 on tb.ID_HangHoa equals tb3.ID_HangHoa into t3
                                from b3 in t3.DefaultIfEmpty()
                                join tb4 in tbl4 on tb.ID_HangHoa equals tb4.ID_HangHoa into t4
                                from b4 in t4.DefaultIfEmpty()
                                join cs in db.ChotSo_HangHoa on dvqd.ID_HangHoa equals cs.ID_HangHoa into t5
                                from b5 in t5.DefaultIfEmpty()
                                select new
                                {
                                    MaHangHoa = tb.MaHangHoa,
                                    ID_DonViQuiDoi = tb.ID,
                                    TenHangHoa = tb.TenHangHoa,
                                    TenDonViTinh = dvqd.TenDonViTinh,
                                    GiaVon = (double?)tb.GiaVon ?? 0,
                                    GiaBan = (double?)dvqd.GiaBan ?? 0,
                                    TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                                    SoLuongXuat = (double?)b1.SoLuongXuat ?? 0, //=0
                                    SoLuongXuatChuyen = (double?)b2.SoLuongXuatChuyen ?? 0,
                                    SoLuongNhap = (double?)b3.SoLuongNhap1 ?? 0,
                                    SoLuongNhapChuyen = (double?)b4.SoLuongNhapchuyen1 ?? 0,
                                    TonKho = (double?)b5.TonKho ?? 0
                                };
                var tbl_gop = tbl_fomat.AsEnumerable().Select(t => new Report_HangHoa_XuatNhapTon_Union
                {
                    ID_DonViQuiDoi = t.ID_DonViQuiDoi,
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenHangHoa,
                    TenDonViTinh = t.TenDonViTinh,
                    GiaVon = Math.Round(t.GiaVon, 0, MidpointRounding.ToEven),
                    GiaBan = Math.Round(t.GiaBan, 0, MidpointRounding.ToEven),
                    TonCuoiKy = Math.Round((t.SoLuongNhap + t.SoLuongNhapChuyen - t.SoLuongXuat - t.SoLuongXuatChuyen) / t.TyLeChuyenDoi + t.TonKho / t.TyLeChuyenDoi, 3, MidpointRounding.ToEven)
                });
                return tbl_gop.Select(p => new jqAutoResult_HangHoa
                {
                    label = p.TenHangHoa,
                    value = p.MaHangHoa,
                    actual = p.MaHangHoa.ToString(),
                    data = p
                }).ToList();
            }
            else
            {
                return null;
            }
        }
        public List<ListDM_LoHang> getList_DMLoHang(Guid ID_DonViQuiDoi)
        {
            List<ListDM_LoHang> lst = new List<ListDM_LoHang>();
            var tbl = from lh in db.DM_LoHang
                      join hh in db.DM_HangHoa on lh.ID_HangHoa equals hh.ID
                      join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                      where dvqd.ID == ID_DonViQuiDoi
                      orderby lh.NgayTao
                      select new ListDM_LoHang
                      {
                          ID_LoHang = lh.ID,
                          ID_HangHoa = hh.ID,
                          TenLoHang = lh.MaLoHang,
                          NgaySanXuat = lh.NgaySanXuat,
                          NgayHetHan = lh.NgayHetHan
                      };
            try
            {
                lst = tbl.ToList();
            }
            catch
            {
                lst = null;
            }
            return lst;
        }
        public List<ListDM_LoHang> getList_DMLoHangbyMaLoHang(string maHangHoa, string MaLoHang, Guid ID_ChiNhanh)
        {
            List<ListDM_LoHang> lst = new List<ListDM_LoHang>();
            var tbl = from lh in db.DM_LoHang
                      join hh in db.DM_HangHoa on lh.ID_HangHoa equals hh.ID
                      join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                      join gv in db.DM_GiaVon on dvqd.ID equals gv.ID_DonViQuiDoi into tem
                      from gv1 in tem.DefaultIfEmpty()
                      where dvqd.MaHangHoa == maHangHoa & lh.MaLoHang == MaLoHang
                      & gv1.ID_LoHang == lh.ID & gv1.ID_DonVi == ID_ChiNhanh
                      select new ListDM_LoHang
                      {
                          ID_LoHang = lh.ID,
                          ID_HangHoa = hh.ID,
                          TenLoHang = lh.MaLoHang,
                          NgaySanXuat = lh.NgaySanXuat,
                          NgayHetHan = lh.NgayHetHan,
                          GiaVon = (double?)gv1.GiaVon ?? 0
                      };
            try
            {
                lst = tbl.ToList();
            }
            catch
            {
                lst = null;
            }
            return lst;
        }

        public List<Report_HangHoa_XuatNhapTon_Union> getListHangHoaBy_MaHangHoa(string maHH, Guid ID_ChiNhanh)
        {
            ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
            List<Report_HangHoa_XuatNhapTon_Union> lst = new List<Report_HangHoa_XuatNhapTon_Union>();
            //      
            if (maHH != null & maHH != "" & maHH != "null")
            {

                var tbl_timeStart = from cs in db.ChotSo
                                    where cs.ID_DonVi == ID_ChiNhanh
                                    select new
                                    {
                                        cs.NgayChotSo
                                    };
                DateTime timeStart = DateTime.Parse("2016-01-01");
                try
                {
                    timeStart = tbl_timeStart.FirstOrDefault().NgayChotSo;
                }
                catch
                {

                }
                var tbl_HH = (from dvqd in db.DonViQuiDois
                              join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                              where dvqd.MaHangHoa == maHH & hh.TheoDoi == true
                              select new
                              {
                                  dvqd.ID,
                                  dvqd.ID_HangHoa,
                                  dvqd.MaHangHoa,
                                  dvqd.GiaVon,
                                  hh.TenHangHoa,
                              }).Take(10);

                var tbl1 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           //join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           (bhd.LoaiHoaDon == 1 || bhd.LoaiHoaDon == 5 || bhd.LoaiHoaDon == 7 || bhd.LoaiHoaDon == 8)
                           & dvqd.Xoa == null //& dvqd.MaHangHoa.Contains(maHH)
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g
                           select new
                           {
                               ID_HangHoa = g.Key.ID_HangHoa,
                               SoLuongNhap = 0,
                               SoLuongXuat = g.Sum(x => (double?)x.bhdct.SoLuong * x.dvqd.TyLeChuyenDoi ?? 0),
                           };
                var tbl2 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           ((bhd.LoaiHoaDon == 10 & bhd.YeuCau == "1") || (bhd.ID_CheckIn != null & bhd.ID_CheckIn != ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4"))
                           & dvqd.Xoa == null //& dvqd.MaHangHoa.Contains(maHH)
                           & hh.TheoDoi == true
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g1
                           select new
                           {
                               ID_HangHoa = g1.Key.ID_HangHoa,
                               SoLuongNhapCHuyen = 0,
                               SoLuongXuatChuyen = g1.Sum(x => (double?)x.bhdct.TienChietKhau * x.dvqd.TyLeChuyenDoi ?? 0),
                           };
                var tbl3 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           (bhd.LoaiHoaDon == 4 || bhd.LoaiHoaDon == 6 || bhd.LoaiHoaDon == 9)
                           & dvqd.Xoa == null //& dvqd.MaHangHoa.Contains(maHH)
                           & hh.TheoDoi == true
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g2
                           select new
                           {

                               ID_HangHoa = g2.Key.ID_HangHoa,
                               SoLuongXuat1 = 0,
                               SoLuongNhap1 = g2.Sum(x => (double?)x.bhdct.SoLuong * x.dvqd.TyLeChuyenDoi ?? 0),
                           };
                var tbl4 = from bhdct in db.BH_HoaDon_ChiTiet
                           join bhd in db.BH_HoaDon on bhdct.ID_HoaDon equals bhd.ID
                           join dvqd in db.DonViQuiDois on bhdct.ID_DonViQuiDoi equals dvqd.ID
                           join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                           join tb in tbl_HH on dvqd.ID_HangHoa equals tb.ID_HangHoa
                           where bhd.NgayLapHoaDon >= timeStart & bhd.ChoThanhToan == false & (bhd.ID_DonVi == ID_ChiNhanh || bhd.ID_CheckIn == ID_ChiNhanh) &
                           (bhd.ID_CheckIn != null & bhd.ID_CheckIn == ID_ChiNhanh & bhd.LoaiHoaDon == 10 & bhd.YeuCau == "4")
                           & dvqd.Xoa == null //& dvqd.MaHangHoa.Contains(maHH)
                           & hh.TheoDoi == true
                           group new { bhd, bhdct, dvqd } by new
                           {
                               dvqd.ID_HangHoa
                           } into g3
                           select new
                           {
                               ID_HangHoa = g3.Key.ID_HangHoa,
                               SoLuongXuatchuyen1 = 0,
                               SoLuongNhapchuyen1 = g3.Sum(x => (double?)x.bhdct.TienChietKhau * x.dvqd.TyLeChuyenDoi ?? 0),
                           };
                var tblcs = from cs in db.ChotSo_HangHoa
                            group cs by new
                            {
                                cs.ID_HangHoa
                            } into g
                            select new
                            {
                                ID_HangHoa = g.Key.ID_HangHoa,
                                TonKho = g.Sum(x => x.TonKho)
                            };
                var tbl_fomat = from tb in tbl_HH
                                join dvqd in db.DonViQuiDois on tb.ID equals dvqd.ID
                                join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                                //join tb1 in tbl1 on tb.ID equals tb1.ID_DonViQuiDoi into t
                                join tb1 in tbl1 on tb.ID_HangHoa equals tb1.ID_HangHoa into t
                                from b1 in t.DefaultIfEmpty()
                                join tb2 in tbl2 on tb.ID_HangHoa equals tb2.ID_HangHoa into t2
                                from b2 in t2.DefaultIfEmpty()
                                join tb3 in tbl3 on tb.ID_HangHoa equals tb3.ID_HangHoa into t3
                                from b3 in t3.DefaultIfEmpty()
                                join tb4 in tbl4 on tb.ID_HangHoa equals tb4.ID_HangHoa into t4
                                from b4 in t4.DefaultIfEmpty()
                                join cs in tblcs on dvqd.ID_HangHoa equals cs.ID_HangHoa into t5
                                from b5 in t5.DefaultIfEmpty()
                                select new
                                {
                                    MaHangHoa = tb.MaHangHoa,
                                    ID_HangHoa = tb.ID_HangHoa,
                                    ID_DonViQuiDoi = tb.ID,
                                    TenHangHoa = tb.TenHangHoa,
                                    TenDonViTinh = dvqd.TenDonViTinh,
                                    QuanLyTheoLoHang = hh.QuanLyTheoLoHang.Value ? 1 : 0,
                                    GiaVon = (double?)tb.GiaVon ?? 0,
                                    GiaBan = (double?)dvqd.GiaBan ?? 0,
                                    TyLeChuyenDoi = dvqd.TyLeChuyenDoi,
                                    SoLuongXuat = (double?)b1.SoLuongXuat ?? 0, //=0
                                    SoLuongXuatChuyen = (double?)b2.SoLuongXuatChuyen ?? 0,
                                    SoLuongNhap = (double?)b3.SoLuongNhap1 ?? 0,
                                    SoLuongNhapChuyen = (double?)b4.SoLuongNhapchuyen1 ?? 0,
                                    TonKho = (double?)b5.TonKho ?? 0
                                };
                var tbl_gop = tbl_fomat.AsEnumerable().Select(t => new Report_HangHoa_XuatNhapTon_Union
                {
                    ID_DonViQuiDoi = t.ID_DonViQuiDoi,
                    MaHangHoa = t.MaHangHoa,
                    TenHangHoa = t.TenHangHoa,
                    ThuocTinh_GiaTri = _classDMHH.Select_HangHoaPRG(t.ID_HangHoa).FirstOrDefault().ThuocTinh_GiaTri,
                    TenDonViTinh = t.TenDonViTinh,
                    QuanLyTheoLoHang = t.QuanLyTheoLoHang,
                    GiaVon = Math.Round(t.GiaVon, 0, MidpointRounding.ToEven),
                    GiaBan = Math.Round(t.GiaBan, 0, MidpointRounding.ToEven),
                    TonCuoiKy = Math.Round((t.SoLuongNhap + t.SoLuongNhapChuyen - t.SoLuongXuat - t.SoLuongXuatChuyen) / t.TyLeChuyenDoi + t.TonKho / t.TyLeChuyenDoi, 3, MidpointRounding.ToEven)
                });
                try
                {
                    lst = tbl_gop.ToList();
                }
                catch
                {

                }
            }
            return lst;
        }

        public List<List_TenDonViTinh> GetList_TenDonViTinh(string MaHH)
        {
            List<List_TenDonViTinh> lst = new List<List_TenDonViTinh>();
            var tbl = from dvqd in db.DonViQuiDois
                      join dvhh in db.DonViQuiDois on dvqd.ID_HangHoa equals dvhh.ID_HangHoa
                      where dvqd.MaHangHoa == MaHH
                      //orderby dvhh.LaDonViChuan descending
                      select new List_TenDonViTinh
                      {
                          MaHangHoa = dvhh.MaHangHoa,
                          TenDonViTinh = dvhh.TenDonViTinh
                      };
            try
            {
                lst = tbl.ToList();
            }
            catch
            {
                lst = null;
            }
            return lst;
        }
    }
    public class ListDM_LoHang
    {
        public Guid ID_LoHang { get; set; }
        public Guid ID_HangHoa { get; set; }
        public string TenLoHangFull { get; set; }
        public string TenLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double TonKho { get; set; }
        public double GiaVon { get; set; }
    }

    public class List_TenDonViTinh
    {
        public string MaHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
    }
    public class BC_XuatHuy
    {
        public Guid? ID { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenDonVi { get; set; }
        public string TenNhanVien { get; set; }
        public string NguoiTaoHD { get; set; }
        public string DienGiai { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string TrangThai { get; set; }
        public double TongTienHang { get; set; }
        public double TongChiPhi { get; set; }
        public double TongGiamGia { get; set; }
        public double PhaiThanhToan { get; set; }
        public string TenPhongBan { get; set; }
        public Guid? ID_ViTri { get; set; }
        public virtual List<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }
        public double KhachDaTra { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_BangGia { get; set; }
        public string TenBangGia { get; set; }
        public bool? ChoThanhToan { get; set; }
        public string YeuCau { get; set; }
        public Guid ID_DoiTuong { get; set; }
    }
    public class TenDonViTinh_PRC
    {
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public double GiaVon { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public int TrangThai { get; set; }
    }
    public class BC_XuatHuyPRC
    {
        public Guid ID { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_HoaDon { get; set; }// idhoadon suachua
        public Guid? ID_PhieuTiepNhan { get; set; }
        public string MaHoaDon { get; set; }
        public string LoaiPhieu { get; set; }
        public DateTime? NgaySua { get; set; }
        public string MaHoaDonSuaChua { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenChiNhanh { get; set; }
        public string TenNhanVien { get; set; }
        public double TongTienHang { get; set; }
        public string DienGiai { get; set; }
        public string YeuCau { get; set; }
        public string NguoiTaoHD { get; set; }
        public bool? ChoThanhToan { get; set; }
        public int LoaiHoaDon { get; set; }
        public string showTime { get; set; }
    }
    public class HD_DieuChinhPRC
    {
        public Guid ID_HoaDon { get; set; }
        public Guid ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
        public string MaHoaDon { get; set; }
        public bool? ChoThanhToan { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string TenDonVi { get; set; }
        public string DienGiai { get; set; }
        public int SoLuongHangHoa { get; set; }
        public double TongGiaVonTang { get; set; }
        public double TongGiaVonGiam { get; set; }
        public string TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiDieuChinh { get; set; }
    }
    public class HD_DieuChinhChiTietPRC
    {
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public bool QuanLyTheoLoHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double GiaVonHienTai { get; set; }
        public double GiaVonMoi { get; set; }
        public double GiaVonTang { get; set; }
        public double GiaVonGiam { get; set; }
        public double ChenhLech { get; set; }
        public int SoThuTu { get; set; }
    }
    public class BC_XuatHuy_Excel
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string TenDonVi { get; set; }
        public double TongTienHang { get; set; }
        public string DienGiai { get; set; }
        public string YeuCau { get; set; }
    }
    public class DM_HangHoa_XNT
    {
        public Guid ID_DonViQuiDoi { get; set; } // ID_DonViQuiDoi
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenHangHoa_KhongDau { get; set; }
        public string TenHangHoa_KyTuDau { get; set; }
        public double GiaVon { get; set; }
        public double TonCuoiKy { get; set; }
    }

    public class XH_HoaDon_ChiTietDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public double DonGia { get; set; }
        public double? GiaVon { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double ThanhToan { get; set; }
        public double GiamGia { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public DateTime? ThoiGian { get; set; }
        //public string TenPhongBan { get; set; }
        public double? Bep_SoLuongYeuCau { get; set; }
        public double? Bep_SoLuongHoanThanh { get; set; }
        public double? Bep_SoLuongChoCungUng { get; set; }
        public string TenPhongBan { get; set; }
        public string MaHoaDon { get; set; }
        public string TenDonViTinh { get; set; }
        public double PTTienChietKhau { get; set; }
        public double TienChietKhau { get; set; }
        public string GhiChu { get; set; }
        public Guid? ID_ViTri { get; set; }
    }
}
