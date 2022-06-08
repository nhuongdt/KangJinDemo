using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using Model;
using libDM_DoiTuong;
using System.Data.SqlClient;

namespace libQuy_HoaDon
{
    public class classQuy_HoaDon
    {
        private SsoftvnContext db;

        public classQuy_HoaDon(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DM_TaiKhoanNganHang selectedDMTKNH(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_TaiKhoanNganHang.Find(id);
            }
        }

        public Quy_HoaDon Select_SoQuy(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Quy_HoaDon.Find(id);
            }
        }
        public List<Quy_HoaDon> Gets(Expression<Func<Quy_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.Quy_HoaDon.ToList();
                else
                    return db.Quy_HoaDon.Where(query).ToList();
            }
        }

        public Quy_HoaDon Get(Expression<Func<Quy_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Quy_HoaDon.Where(query).FirstOrDefault();
            }
        }

        public DM_NganHang GetNganHang(Expression<Func<DM_NganHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_NganHang.Where(query).FirstOrDefault();
            }
        }

        public Quy_HoaDonDTO GetFistQuy_HoaDon(Expression<Func<Quy_HoaDon, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Quy_HoaDon.Select(vt => new Quy_HoaDonDTO { ID = vt.ID, MaHoaDon = vt.MaHoaDon }).FirstOrDefault();
            }
        }

        public List<Quy_HoaDon_ChiTietDTO> GetListHoaDons_QuyHD_Group(Guid iddonvi, string maHoaDon, int trangThai, string dayStart, string dayEnd, string idnhanvien, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string idTKNganHang, int kinhdoanh)
        {
            List<Quy_HoaDon_ChiTietDTO> lst = new List<Quy_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                IEnumerable<Quy_HoaDon_ChiTietDTO> tbl = null;
                if (idTKNganHang == "undefined")
                {
                    tbl = from qhd in db.Quy_HoaDon.Where(p => p.PhieuDieuChinhCongNo == null || p.PhieuDieuChinhCongNo == 0)
                          join ctq in db.Quy_HoaDon_ChiTiet.Where(p => (p.DiemThanhToan == null || p.DiemThanhToan == 0) && p.ThuTuThe == 0) on qhd.ID equals ctq.ID_HoaDon
                          join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                          from loaithu in LOAITHU.DefaultIfEmpty()
                          join hd in db.BH_HoaDon on ctq.ID_HoaDonLienQuan equals hd.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join dv in db.DM_DonVi on qhd.ID_DonVi equals dv.ID into QHD_DV
                          from qhd_dv in QHD_DV.DefaultIfEmpty()
                          join dt in db.DM_DoiTuong on ctq.ID_DoiTuong equals dt.ID into DT_NCC
                          from dt_ncc in DT_NCC.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on qhd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          orderby qhd.NgayLapHoaDon descending
                          group new { ctq } by new
                          {
                              ID = qhd.ID,
                              ID_DonVi = qhd_dv.ID,
                              ID_NhanVien = qhd.ID_NhanVien,
                              ID_HoaDonLienQuan = ctq.ID_HoaDonLienQuan,
                              MaHoaDon = qhd.MaHoaDon,
                              MaHoaDonHD = hd_dt.MaHoaDon,
                              ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang,
                              NguoiNopTien = qhd.NguoiNopTien,
                              NgayLapHoaDon = qhd.NgayLapHoaDon,
                              TongTienThu = qhd.TongTienThu,
                              LoaiHoaDon = qhd.LoaiHoaDon,
                              NoiDungThu = qhd.NoiDungThu,
                              TrangThai = qhd.TrangThai,
                              ID_KhoanThuChi = loaithu.ID,
                              LaKhoanThu = loaithu.LaKhoanThu,
                              NoiDungThuChi = loaithu.NoiDungThuChi,
                              TenNhanVien = hd_nv.TenNhanVien,
                              NguoiSua = qhd.NguoiSua,
                              TenChiNhanh = qhd_dv.TenDonVi,
                              DiaChiChiNhanh = qhd_dv.DiaChi,
                              DienThoaiChiNhanh = qhd_dv.SoDienThoai,
                              DiemThanhToan = ctq.DiemThanhToan,
                              HachToanKinhDoanh = qhd.HachToanKinhDoanh,
                              LoaiDoiTuong = ctq.ID_NhanVien != null ? 3 : ctq.ID_DoiTuong != null ? (dt_ncc.LoaiDoiTuong == 1 ? 1 : 2) : 0,
                              ID_DoiTuong = ctq.ID_NhanVien != null ? ctq.ID_NhanVien : ctq.ID_DoiTuong

                          } into g

                          select new Quy_HoaDon_ChiTietDTO
                          {
                              ID_DonVi = g.Key.ID_DonVi,
                              ID = g.Key.ID,
                              ID_HoaDonLienQuan = g.Key.ID_HoaDonLienQuan,
                              MaHoaDon = g.Key.MaHoaDon,
                              ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang,
                              MaHoaDonHD = g.Key.MaHoaDonHD,
                              NguoiNopTien = g.Key.NguoiNopTien,
                              TenNhanVien = g.Key.TenNhanVien,
                              NgayLapHoaDon = g.Key.NgayLapHoaDon,
                              LoaiHoaDon = g.Key.LoaiHoaDon,
                              TienMat = g.Sum(x => x.ctq.TienMat),
                              TienGui = g.Sum(x => x.ctq.TienGui),
                              TienThu = g.Sum(x => x.ctq.TienThu),
                              TongTienThu = g.Sum(x => x.ctq.TienMat) + g.Sum(x => x.ctq.TienGui),
                              NoiDungThu = g.Key.NoiDungThu,
                              NguoiSua = g.Key.NguoiSua,
                              ID_NhanVien = g.Key.ID_NhanVien,
                              TenChiNhanh = g.Key.TenChiNhanh,
                              DienThoaiChiNhanh = g.Key.DienThoaiChiNhanh,
                              DiaChiChiNhanh = g.Key.DiaChiChiNhanh,
                              TrangThai = g.Key.TrangThai,
                              ID_KhoanThuChi = g.Key.ID_KhoanThuChi,
                              LaKhoanThu = g.Key.LaKhoanThu,
                              NoiDungThuChi = g.Key.NoiDungThuChi,
                              DiemThanhToan = g.Key.DiemThanhToan,
                              LoaiDoiTuong = g.Key.LoaiDoiTuong,
                              GhiChu = "Đã thanh toán",
                              HachToanKinhDoanh = g.Key.HachToanKinhDoanh,
                              ID_DoiTuong = g.Key.ID_DoiTuong
                              //ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang
                          };
                }
                else
                {
                    tbl = from qhd in db.Quy_HoaDon.Where(p => p.PhieuDieuChinhCongNo == null || p.PhieuDieuChinhCongNo == 0)
                          join ctq in db.Quy_HoaDon_ChiTiet.Where(p => p.ID_TaiKhoanNganHang.ToString() == idTKNganHang && (p.DiemThanhToan == null || p.DiemThanhToan == 0) && p.ThuTuThe == 0) on qhd.ID equals ctq.ID_HoaDon
                          join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                          from loaithu in LOAITHU.DefaultIfEmpty()
                          join hd in db.BH_HoaDon on ctq.ID_HoaDonLienQuan equals hd.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join dv in db.DM_DonVi on qhd.ID_DonVi equals dv.ID into QHD_DV
                          from qhd_dv in QHD_DV.DefaultIfEmpty()
                          join dt in db.DM_DoiTuong on ctq.ID_DoiTuong equals dt.ID into DT_NCC
                          from dt_ncc in DT_NCC.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on qhd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          orderby qhd.NgayLapHoaDon descending
                          group new { ctq } by new
                          {
                              ID = qhd.ID,
                              ID_DonVi = qhd_dv.ID,
                              ID_NhanVien = qhd.ID_NhanVien,
                              MaHoaDon = qhd.MaHoaDon,
                              ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang,
                              MaHoaDonHD = hd_dt.MaHoaDon,
                              NguoiNopTien = qhd.NguoiNopTien,
                              NgayLapHoaDon = qhd.NgayLapHoaDon,
                              TongTienThu = qhd.TongTienThu,
                              LoaiHoaDon = qhd.LoaiHoaDon,
                              NoiDungThu = qhd.NoiDungThu,
                              TrangThai = qhd.TrangThai,
                              ID_KhoanThuChi = loaithu.ID,
                              LaKhoanThu = loaithu.LaKhoanThu,
                              NoiDungThuChi = loaithu.NoiDungThuChi,
                              TenNhanVien = hd_nv.TenNhanVien,
                              NguoiSua = qhd.NguoiSua,
                              TenChiNhanh = qhd_dv.TenDonVi,
                              DiaChiChiNhanh = qhd_dv.DiaChi,
                              DienThoaiChiNhanh = qhd_dv.SoDienThoai,
                              DiemThanhToan = ctq.DiemThanhToan,
                              ID_HoaDonLienQuan = ctq.ID_HoaDonLienQuan,
                              HachToanKinhDoanh = qhd.HachToanKinhDoanh,
                              LoaiDoiTuong = ctq.ID_NhanVien != null ? 3 : ctq.ID_DoiTuong != null ? (dt_ncc.LoaiDoiTuong == 1 ? 1 : 2) : 0,
                              ID_DoiTuong = ctq.ID_NhanVien != null ? ctq.ID_NhanVien : ctq.ID_DoiTuong
                              //ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang

                          } into g

                          select new Quy_HoaDon_ChiTietDTO
                          {
                              ID_DonVi = g.Key.ID_DonVi,
                              ID = g.Key.ID,
                              ID_HoaDonLienQuan = g.Key.ID_HoaDonLienQuan,
                              MaHoaDon = g.Key.MaHoaDon,
                              ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang,
                              MaHoaDonHD = g.Key.MaHoaDonHD,
                              NguoiNopTien = g.Key.NguoiNopTien,
                              TenNhanVien = g.Key.TenNhanVien,
                              NgayLapHoaDon = g.Key.NgayLapHoaDon,
                              LoaiHoaDon = g.Key.LoaiHoaDon,
                              TienMat = g.Sum(x => x.ctq.TienMat),
                              TienGui = g.Sum(x => x.ctq.TienGui),
                              TienThu = g.Sum(x => x.ctq.TienThu),
                              TongTienThu = g.Sum(x => x.ctq.TienMat) + g.Sum(x => x.ctq.TienGui),
                              NoiDungThu = g.Key.NoiDungThu,
                              NguoiSua = g.Key.NguoiSua,
                              ID_NhanVien = g.Key.ID_NhanVien,
                              TenChiNhanh = g.Key.TenChiNhanh,
                              DienThoaiChiNhanh = g.Key.DienThoaiChiNhanh,
                              DiaChiChiNhanh = g.Key.DiaChiChiNhanh,
                              TrangThai = g.Key.TrangThai,
                              ID_KhoanThuChi = g.Key.ID_KhoanThuChi,
                              LaKhoanThu = g.Key.LaKhoanThu,
                              NoiDungThuChi = g.Key.NoiDungThuChi,
                              DiemThanhToan = g.Key.DiemThanhToan,
                              LoaiDoiTuong = g.Key.LoaiDoiTuong,
                              GhiChu = "Đã thanh toán",
                              HachToanKinhDoanh = g.Key.HachToanKinhDoanh,
                              ID_DoiTuong = g.Key.ID_DoiTuong
                              //ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang
                          };
                }

                if (idnhanvien != "undefined")
                {
                    tbl = tbl.Where(qhd => qhd.ID_NhanVien.ToString().Contains(idnhanvien));
                }

                if (loaithuchi != "undefined")
                {
                    tbl = tbl.Where(qhd => qhd.ID_KhoanThuChi.ToString().Contains(loaithuchi));
                }

                List<Guid> lstIDCN = new List<Guid>();
                if (arrChiNhanh != null)
                {
                    var arrIDCN = arrChiNhanh.Split(',');
                    for (int i = 0; i < arrIDCN.Length; i++)
                    {
                        lstIDCN.Add(new Guid(arrIDCN[i]));
                    }
                }
                if (lstIDCN.Count > 0)
                {
                    tbl = tbl.Where(p => p.ID_DonVi != null).Where(hd => lstIDCN.Contains(hd.ID_DonVi.Value));
                }
                else
                {
                    if (iddonvi != Guid.Empty && iddonvi != null)
                    {
                        tbl = tbl.Where(qhd => qhd.ID_DonVi == iddonvi);
                    }
                }


                if (dayStart != null && dayEnd != null && dayStart != string.Empty && dayEnd != string.Empty)
                {
                    DateTime dtStart = DateTime.Parse(dayStart);
                    DateTime dtEnd = DateTime.Parse(dayEnd);
                    if (dayStart == dayEnd)
                    {
                        tbl = tbl.Where(hd => hd.NgayLapHoaDon.Year == dtStart.Year
                        && hd.NgayLapHoaDon.Month == dtStart.Month
                        && hd.NgayLapHoaDon.Day == dtEnd.Day);
                    }
                    else
                    {
                        tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dtStart && hd.NgayLapHoaDon < dtEnd);
                    }
                }
                // trang thai HoaDon
                switch (trangThai)
                {
                    case 1: // HT
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 11);
                        break;
                    case 2: // Huy
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 12);
                        break;
                    case 3: // HT + Huy
                        break;
                    default: // tam luu
                        break;
                }

                switch (locthanhtoan)
                {
                    case 1: // HT
                        tbl = tbl.Where(hd => hd.TrangThai != false);
                        break;
                    case 2: // Huy
                        tbl = tbl.Where(hd => hd.TrangThai == false);
                        break;
                    case 3: // HT + Huy
                        break;
                    case 4: // HT + Huy
                        break;
                    default: // tam luu
                        tbl = tbl.Where(hd => hd.TrangThai != false);
                        break;
                }

                switch (kinhdoanh)
                {
                    case 0:
                        tbl = tbl.Where(hd => hd.HachToanKinhDoanh == null || hd.HachToanKinhDoanh == true);
                        break;
                    case 1:
                        tbl = tbl.Where(hd => hd.HachToanKinhDoanh == false);
                        break;
                    case 2:
                        break;
                }
                //tbl = tbl.OrderByDescending(dt => dt.NgayLapHoaDon);
                string stSearch = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                string stSearchGC = CommonStatic.ConvertToUnSign(ghichu).ToLower();
                if (tbl != null)
                {
                    foreach (var item in tbl)
                    {
                        string tenNN = CommonStatic.ConvertToUnSign(item.NguoiNopTien).ToLower();
                        string ghiChu = CommonStatic.ConvertToUnSign(item.NoiDungThu).ToLower();
                        string maHD = CommonStatic.ConvertToUnSign(item.MaHoaDon).ToLower();
                        string tenNNsplit = CommonStatic.GetCharsStart(item.NguoiNopTien).ToLower();
                        string maHDsplit = CommonStatic.GetCharsStart(item.MaHoaDon).ToLower();
                        string ghiChusplit = CommonStatic.GetCharsStart(item.NoiDungThu).ToLower();
                        if ((tenNN.Contains(@stSearch.Trim()) || maHD.Contains(@stSearch.Trim()) || tenNNsplit.Contains(@stSearch.Trim()) || maHDsplit.Contains(@stSearch.Trim())) || (ghiChu.Contains(@stSearch.Trim()) || ghiChusplit.Contains(@stSearch.Trim())))
                        {
                            Quy_HoaDon_ChiTietDTO quy_HoaDon_DTO = new Quy_HoaDon_ChiTietDTO();
                            quy_HoaDon_DTO.ID_DonVi = item.ID;
                            quy_HoaDon_DTO.ID = item.ID;
                            quy_HoaDon_DTO.ID_HoaDonLienQuan = item.ID_HoaDonLienQuan;
                            quy_HoaDon_DTO.MaHoaDon = item.MaHoaDon;
                            quy_HoaDon_DTO.ID_TaiKhoanNganHang = item.ID_TaiKhoanNganHang;
                            quy_HoaDon_DTO.MaHoaDonHD = item.MaHoaDonHD;
                            quy_HoaDon_DTO.NguoiNopTien = item.NguoiNopTien;
                            quy_HoaDon_DTO.TenNhanVien = item.TenNhanVien;
                            quy_HoaDon_DTO.NgayLapHoaDon = item.NgayLapHoaDon;
                            quy_HoaDon_DTO.LoaiHoaDon = item.LoaiHoaDon;
                            quy_HoaDon_DTO.TienMat = item.TienMat;
                            quy_HoaDon_DTO.TienGui = item.TienGui;
                            quy_HoaDon_DTO.TienThu = item.TienThu;
                            //quy_HoaDon_DTO.TongTienThu = item.TienGui == 0 ? item.TienMat : item.TienGui;
                            quy_HoaDon_DTO.TongTienThu = item.TongTienThu;
                            //MaKhoanThuChi = s.MaKhoanThuChi
                            quy_HoaDon_DTO.NoiDungThu = item.NoiDungThu;
                            quy_HoaDon_DTO.NguoiSua = item.NguoiSua;
                            quy_HoaDon_DTO.ID_NhanVien = item.ID_NhanVien;
                            quy_HoaDon_DTO.TenChiNhanh = item.TenChiNhanh;
                            quy_HoaDon_DTO.DienThoaiChiNhanh = item.DienThoaiChiNhanh;
                            quy_HoaDon_DTO.DiaChiChiNhanh = item.DiaChiChiNhanh;
                            quy_HoaDon_DTO.TrangThai = item.TrangThai;
                            quy_HoaDon_DTO.ID_KhoanThuChi = item.ID_KhoanThuChi;
                            quy_HoaDon_DTO.LaKhoanThu = item.LaKhoanThu;
                            quy_HoaDon_DTO.NoiDungThuChi = item.NoiDungThuChi;
                            quy_HoaDon_DTO.DiemThanhToan = item.DiemThanhToan;
                            quy_HoaDon_DTO.GhiChu = item.GhiChu;
                            quy_HoaDon_DTO.LoaiDoiTuong = item.LoaiDoiTuong;
                            quy_HoaDon_DTO.HachToanKinhDoanh = item.HachToanKinhDoanh;
                            quy_HoaDon_DTO.ID_DoiTuong = item.ID_DoiTuong;
                            lst.Add(quy_HoaDon_DTO);
                        }
                    }

                    lst = lst.GroupBy(o => o.ID).Select(t => new Quy_HoaDon_ChiTietDTO
                    {
                        ID = t.Select(c => c.ID).FirstOrDefault(),
                        ID_HoaDonLienQuan = t.Where(c => c.ID_HoaDonLienQuan != null).Select(c => c.ID_HoaDonLienQuan).FirstOrDefault(),
                        MaHoaDon = t.Select(c => c.MaHoaDon).FirstOrDefault(),
                        ID_TaiKhoanNganHang = t.Select(c => c.ID_TaiKhoanNganHang).FirstOrDefault(),
                        MaHoaDonHD = t.Select(c => c.MaHoaDonHD).FirstOrDefault(),
                        NguoiNopTien = t.Select(c => c.NguoiNopTien).FirstOrDefault(),
                        TenNhanVien = t.Select(c => c.TenNhanVien).FirstOrDefault(),
                        NgayLapHoaDon = t.Select(c => c.NgayLapHoaDon).FirstOrDefault(),
                        LoaiHoaDon = t.Select(c => c.LoaiHoaDon).FirstOrDefault(),
                        TienMat = t.Sum(c => c.TienMat),
                        TienGui = t.Sum(c => c.TienGui),
                        TienThu = t.Sum(c => c.TienThu),
                        //quy_HoaDon_DTO.TongTienThu = item.TienGui == 0 ? item.TienMat : item.TienGui,
                        TongTienThu = t.Sum(c => c.TongTienThu),
                        //MaKhoanThuChi = s.MaKhoanThuChi
                        NoiDungThu = t.Select(c => c.NoiDungThu).FirstOrDefault(),
                        NguoiSua = t.Select(c => c.NguoiSua).FirstOrDefault(),
                        ID_NhanVien = t.Select(c => c.ID_NhanVien).FirstOrDefault(),
                        TenChiNhanh = t.Select(c => c.TenChiNhanh).FirstOrDefault(),
                        DienThoaiChiNhanh = t.Select(c => c.DienThoaiChiNhanh).FirstOrDefault(),
                        DiaChiChiNhanh = t.Select(c => c.DiaChiChiNhanh).FirstOrDefault(),
                        TrangThai = t.Select(c => c.TrangThai).FirstOrDefault(),
                        ID_KhoanThuChi = t.Select(c => c.ID_KhoanThuChi).FirstOrDefault(),
                        LaKhoanThu = t.Select(c => c.LaKhoanThu).FirstOrDefault(),
                        NoiDungThuChi = t.Select(c => c.NoiDungThuChi).FirstOrDefault(),
                        DiemThanhToan = t.Select(c => c.DiemThanhToan).FirstOrDefault(),
                        GhiChu = t.Select(c => c.GhiChu).FirstOrDefault(),
                        LoaiDoiTuong = t.Select(c => c.LoaiDoiTuong).FirstOrDefault(),
                        HachToanKinhDoanh = t.Select(c => c.HachToanKinhDoanh).FirstOrDefault(),
                        ID_DoiTuong = t.Select(c => c.ID_DoiTuong).FirstOrDefault(),
                    }).ToList();
                    IEnumerable<Quy_HoaDon_ChiTietDTO> iEnumber_Quy_HoaDon = lst;
                    return iEnumber_Quy_HoaDon.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        //ton dau ky
        public List<Quy_HoaDon_ChiTietDTO> GetListHoaDons_QuyHD_GroupDauKy(Guid iddonvi, string maHoaDon, int trangThai, string dayDauky, string idnhanvien, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string idTKNganHang, int kinhdoanh)
        {
            List<Quy_HoaDon_ChiTietDTO> lst = new List<Quy_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                IEnumerable<Quy_HoaDon_ChiTietDTO> tbl = null;
                if (idTKNganHang == "undefined")
                {
                    tbl = from qhd in db.Quy_HoaDon.Where(p => p.PhieuDieuChinhCongNo == null || p.PhieuDieuChinhCongNo == 0)
                          join ctq in db.Quy_HoaDon_ChiTiet.Where(p => (p.DiemThanhToan == null || p.DiemThanhToan == 0) && p.ThuTuThe == 0) on qhd.ID equals ctq.ID_HoaDon
                          join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                          from loaithu in LOAITHU.DefaultIfEmpty()
                          join hd in db.BH_HoaDon on ctq.ID_HoaDonLienQuan equals hd.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join dv in db.DM_DonVi on qhd.ID_DonVi equals dv.ID into QHD_DV
                          from qhd_dv in QHD_DV.DefaultIfEmpty()
                          join dt in db.DM_DoiTuong on ctq.ID_DoiTuong equals dt.ID into DT_NCC
                          from dt_ncc in DT_NCC.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on qhd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          orderby qhd.NgayLapHoaDon descending
                          group new { ctq } by new
                          {
                              ID = qhd.ID,
                              ID_DonVi = qhd_dv.ID,
                              ID_NhanVien = qhd.ID_NhanVien,
                              ID_HoaDonLienQuan = ctq.ID_HoaDonLienQuan,
                              MaHoaDon = qhd.MaHoaDon,
                              MaHoaDonHD = hd_dt.MaHoaDon,
                              ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang,
                              NguoiNopTien = qhd.NguoiNopTien,
                              NgayLapHoaDon = qhd.NgayLapHoaDon,
                              TongTienThu = qhd.TongTienThu,
                              LoaiHoaDon = qhd.LoaiHoaDon,
                              NoiDungThu = qhd.NoiDungThu,
                              TrangThai = qhd.TrangThai,
                              ID_KhoanThuChi = loaithu.ID,
                              LaKhoanThu = loaithu.LaKhoanThu,
                              NoiDungThuChi = loaithu.NoiDungThuChi,
                              TenNhanVien = hd_nv.TenNhanVien,
                              NguoiSua = qhd.NguoiSua,
                              TenChiNhanh = qhd_dv.TenDonVi,
                              DiaChiChiNhanh = qhd_dv.DiaChi,
                              DienThoaiChiNhanh = qhd_dv.SoDienThoai,
                              DiemThanhToan = ctq.DiemThanhToan,
                              HachToanKinhDoanh = qhd.HachToanKinhDoanh,
                              LoaiDoiTuong = ctq.ID_NhanVien != null ? 3 : ctq.ID_DoiTuong != null ? (dt_ncc.LoaiDoiTuong == 1 ? 1 : 2) : 0,
                              ID_DoiTuong = ctq.ID_NhanVien != null ? ctq.ID_NhanVien : ctq.ID_DoiTuong

                          } into g

                          select new Quy_HoaDon_ChiTietDTO
                          {
                              ID_DonVi = g.Key.ID_DonVi,
                              ID = g.Key.ID,
                              ID_HoaDonLienQuan = g.Key.ID_HoaDonLienQuan,
                              MaHoaDon = g.Key.MaHoaDon,
                              ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang,
                              MaHoaDonHD = g.Key.MaHoaDonHD,
                              NguoiNopTien = g.Key.NguoiNopTien,
                              TenNhanVien = g.Key.TenNhanVien,
                              NgayLapHoaDon = g.Key.NgayLapHoaDon,
                              LoaiHoaDon = g.Key.LoaiHoaDon,
                              TienMat = g.Sum(x => x.ctq.TienMat),
                              TienGui = g.Sum(x => x.ctq.TienGui),
                              TienThu = g.Sum(x => x.ctq.TienThu),
                              TongTienThu = g.Sum(x => x.ctq.TienMat) + g.Sum(x => x.ctq.TienGui),
                              NoiDungThu = g.Key.NoiDungThu,
                              NguoiSua = g.Key.NguoiSua,
                              ID_NhanVien = g.Key.ID_NhanVien,
                              TenChiNhanh = g.Key.TenChiNhanh,
                              DienThoaiChiNhanh = g.Key.DienThoaiChiNhanh,
                              DiaChiChiNhanh = g.Key.DiaChiChiNhanh,
                              TrangThai = g.Key.TrangThai,
                              ID_KhoanThuChi = g.Key.ID_KhoanThuChi,
                              LaKhoanThu = g.Key.LaKhoanThu,
                              NoiDungThuChi = g.Key.NoiDungThuChi,
                              DiemThanhToan = g.Key.DiemThanhToan,
                              LoaiDoiTuong = g.Key.LoaiDoiTuong,
                              GhiChu = "Đã thanh toán",
                              HachToanKinhDoanh = g.Key.HachToanKinhDoanh,
                              ID_DoiTuong = g.Key.ID_DoiTuong
                              //ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang
                          };
                }
                else
                {
                    tbl = from qhd in db.Quy_HoaDon.Where(p => p.PhieuDieuChinhCongNo == null || p.PhieuDieuChinhCongNo == 0)
                          join ctq in db.Quy_HoaDon_ChiTiet.Where(p => p.ID_TaiKhoanNganHang.ToString() == idTKNganHang && (p.DiemThanhToan == null || p.DiemThanhToan == 0) && p.ThuTuThe == 0) on qhd.ID equals ctq.ID_HoaDon
                          join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                          from loaithu in LOAITHU.DefaultIfEmpty()
                          join hd in db.BH_HoaDon on ctq.ID_HoaDonLienQuan equals hd.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join dv in db.DM_DonVi on qhd.ID_DonVi equals dv.ID into QHD_DV
                          from qhd_dv in QHD_DV.DefaultIfEmpty()
                          join dt in db.DM_DoiTuong on ctq.ID_DoiTuong equals dt.ID into DT_NCC
                          from dt_ncc in DT_NCC.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on qhd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          orderby qhd.NgayLapHoaDon descending
                          group new { ctq } by new
                          {
                              ID = qhd.ID,
                              ID_DonVi = qhd_dv.ID,
                              ID_NhanVien = qhd.ID_NhanVien,
                              MaHoaDon = qhd.MaHoaDon,
                              ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang,
                              MaHoaDonHD = hd_dt.MaHoaDon,
                              NguoiNopTien = qhd.NguoiNopTien,
                              NgayLapHoaDon = qhd.NgayLapHoaDon,
                              TongTienThu = qhd.TongTienThu,
                              LoaiHoaDon = qhd.LoaiHoaDon,
                              NoiDungThu = qhd.NoiDungThu,
                              TrangThai = qhd.TrangThai,
                              ID_KhoanThuChi = loaithu.ID,
                              LaKhoanThu = loaithu.LaKhoanThu,
                              NoiDungThuChi = loaithu.NoiDungThuChi,
                              TenNhanVien = hd_nv.TenNhanVien,
                              NguoiSua = qhd.NguoiSua,
                              TenChiNhanh = qhd_dv.TenDonVi,
                              DiaChiChiNhanh = qhd_dv.DiaChi,
                              DienThoaiChiNhanh = qhd_dv.SoDienThoai,
                              DiemThanhToan = ctq.DiemThanhToan,
                              ID_HoaDonLienQuan = ctq.ID_HoaDonLienQuan,
                              HachToanKinhDoanh = qhd.HachToanKinhDoanh,
                              LoaiDoiTuong = ctq.ID_NhanVien != null ? 3 : ctq.ID_DoiTuong != null ? (dt_ncc.LoaiDoiTuong == 1 ? 1 : 2) : 0,
                              ID_DoiTuong = ctq.ID_NhanVien != null ? ctq.ID_NhanVien : ctq.ID_DoiTuong
                              //ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang

                          } into g

                          select new Quy_HoaDon_ChiTietDTO
                          {
                              ID_DonVi = g.Key.ID_DonVi,
                              ID = g.Key.ID,
                              ID_HoaDonLienQuan = g.Key.ID_HoaDonLienQuan,
                              MaHoaDon = g.Key.MaHoaDon,
                              ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang,
                              MaHoaDonHD = g.Key.MaHoaDonHD,
                              NguoiNopTien = g.Key.NguoiNopTien,
                              TenNhanVien = g.Key.TenNhanVien,
                              NgayLapHoaDon = g.Key.NgayLapHoaDon,
                              LoaiHoaDon = g.Key.LoaiHoaDon,
                              TienMat = g.Sum(x => x.ctq.TienMat),
                              TienGui = g.Sum(x => x.ctq.TienGui),
                              TienThu = g.Sum(x => x.ctq.TienThu),
                              TongTienThu = g.Sum(x => x.ctq.TienMat) + g.Sum(x => x.ctq.TienGui),
                              NoiDungThu = g.Key.NoiDungThu,
                              NguoiSua = g.Key.NguoiSua,
                              ID_NhanVien = g.Key.ID_NhanVien,
                              TenChiNhanh = g.Key.TenChiNhanh,
                              DienThoaiChiNhanh = g.Key.DienThoaiChiNhanh,
                              DiaChiChiNhanh = g.Key.DiaChiChiNhanh,
                              TrangThai = g.Key.TrangThai,
                              ID_KhoanThuChi = g.Key.ID_KhoanThuChi,
                              LaKhoanThu = g.Key.LaKhoanThu,
                              NoiDungThuChi = g.Key.NoiDungThuChi,
                              DiemThanhToan = g.Key.DiemThanhToan,
                              LoaiDoiTuong = g.Key.LoaiDoiTuong,
                              GhiChu = "Đã thanh toán",
                              HachToanKinhDoanh = g.Key.HachToanKinhDoanh,
                              ID_DoiTuong = g.Key.ID_DoiTuong
                              //ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang
                          };
                }

                if (idnhanvien != "undefined")
                {
                    tbl = tbl.Where(qhd => qhd.ID_NhanVien.ToString().Contains(idnhanvien));
                }

                if (loaithuchi != "undefined")
                {
                    tbl = tbl.Where(qhd => qhd.ID_KhoanThuChi.ToString().Contains(loaithuchi));
                }

                List<Guid> lstIDCN = new List<Guid>();
                if (arrChiNhanh != null)
                {
                    var arrIDCN = arrChiNhanh.Split(',');
                    for (int i = 0; i < arrIDCN.Length; i++)
                    {
                        lstIDCN.Add(new Guid(arrIDCN[i]));
                    }
                }
                if (lstIDCN.Count > 0)
                {
                    tbl = tbl.Where(p => p.ID_DonVi != null).Where(hd => lstIDCN.Contains(hd.ID_DonVi.Value));
                }
                else
                {
                    if (iddonvi != Guid.Empty && iddonvi != null)
                    {
                        tbl = tbl.Where(qhd => qhd.ID_DonVi == iddonvi);
                    }
                }


                if (dayDauky != null && dayDauky != string.Empty)
                {
                    DateTime dayFirst = DateTime.Parse(dayDauky);
                    tbl = tbl.Where(hd => hd.NgayLapHoaDon < dayFirst);
                }
                // trang thai HoaDon
                switch (trangThai)
                {
                    case 1: // HT
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 11);
                        break;
                    case 2: // Huy
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 12);
                        break;
                    case 3: // HT + Huy
                        break;
                    default: // tam luu
                        break;
                }

                switch (locthanhtoan)
                {
                    case 1: // HT
                        tbl = tbl.Where(hd => hd.TrangThai != false);
                        break;
                    case 2: // Huy
                        tbl = tbl.Where(hd => hd.TrangThai == false);
                        break;
                    case 3: // HT + Huy
                        break;
                    case 4: // HT + Huy
                        break;
                    default: // tam luu
                        tbl = tbl.Where(hd => hd.TrangThai != false);
                        break;
                }

                switch (kinhdoanh)
                {
                    case 0:
                        tbl = tbl.Where(hd => hd.HachToanKinhDoanh == null || hd.HachToanKinhDoanh == true);
                        break;
                    case 1:
                        tbl = tbl.Where(hd => hd.HachToanKinhDoanh == false);
                        break;
                    case 2:
                        break;
                }
                //tbl = tbl.OrderByDescending(dt => dt.NgayLapHoaDon);
                string stSearch = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                string stSearchGC = CommonStatic.ConvertToUnSign(ghichu).ToLower();
                if (tbl != null)
                {
                    foreach (var item in tbl)
                    {
                        string tenNN = CommonStatic.ConvertToUnSign(item.NguoiNopTien).ToLower();
                        string ghiChu = CommonStatic.ConvertToUnSign(item.NoiDungThu).ToLower();
                        string maHD = CommonStatic.ConvertToUnSign(item.MaHoaDon).ToLower();
                        string tenNNsplit = CommonStatic.GetCharsStart(item.NguoiNopTien).ToLower();
                        string maHDsplit = CommonStatic.GetCharsStart(item.MaHoaDon).ToLower();
                        string ghiChusplit = CommonStatic.GetCharsStart(item.NoiDungThu).ToLower();
                        if ((tenNN.Contains(@stSearch.Trim()) || maHD.Contains(@stSearch.Trim()) || tenNNsplit.Contains(@stSearch.Trim()) || maHDsplit.Contains(@stSearch.Trim())) || (ghiChu.Contains(@stSearch.Trim()) || ghiChusplit.Contains(@stSearch.Trim())))
                        {
                            Quy_HoaDon_ChiTietDTO quy_HoaDon_DTO = new Quy_HoaDon_ChiTietDTO();
                            quy_HoaDon_DTO.ID_DonVi = item.ID;
                            quy_HoaDon_DTO.ID = item.ID;
                            quy_HoaDon_DTO.ID_HoaDonLienQuan = item.ID_HoaDonLienQuan;
                            quy_HoaDon_DTO.MaHoaDon = item.MaHoaDon;
                            quy_HoaDon_DTO.ID_TaiKhoanNganHang = item.ID_TaiKhoanNganHang;
                            quy_HoaDon_DTO.MaHoaDonHD = item.MaHoaDonHD;
                            quy_HoaDon_DTO.NguoiNopTien = item.NguoiNopTien;
                            quy_HoaDon_DTO.TenNhanVien = item.TenNhanVien;
                            quy_HoaDon_DTO.NgayLapHoaDon = item.NgayLapHoaDon;
                            quy_HoaDon_DTO.LoaiHoaDon = item.LoaiHoaDon;
                            quy_HoaDon_DTO.TienMat = item.TienMat;
                            quy_HoaDon_DTO.TienGui = item.TienGui;
                            quy_HoaDon_DTO.TienThu = item.TienThu;
                            //quy_HoaDon_DTO.TongTienThu = item.TienGui == 0 ? item.TienMat : item.TienGui;
                            quy_HoaDon_DTO.TongTienThu = item.TongTienThu;
                            //MaKhoanThuChi = s.MaKhoanThuChi
                            quy_HoaDon_DTO.NoiDungThu = item.NoiDungThu;
                            quy_HoaDon_DTO.NguoiSua = item.NguoiSua;
                            quy_HoaDon_DTO.ID_NhanVien = item.ID_NhanVien;
                            quy_HoaDon_DTO.TenChiNhanh = item.TenChiNhanh;
                            quy_HoaDon_DTO.DienThoaiChiNhanh = item.DienThoaiChiNhanh;
                            quy_HoaDon_DTO.DiaChiChiNhanh = item.DiaChiChiNhanh;
                            quy_HoaDon_DTO.TrangThai = item.TrangThai;
                            quy_HoaDon_DTO.ID_KhoanThuChi = item.ID_KhoanThuChi;
                            quy_HoaDon_DTO.LaKhoanThu = item.LaKhoanThu;
                            quy_HoaDon_DTO.NoiDungThuChi = item.NoiDungThuChi;
                            quy_HoaDon_DTO.DiemThanhToan = item.DiemThanhToan;
                            quy_HoaDon_DTO.GhiChu = item.GhiChu;
                            quy_HoaDon_DTO.LoaiDoiTuong = item.LoaiDoiTuong;
                            quy_HoaDon_DTO.HachToanKinhDoanh = item.HachToanKinhDoanh;
                            quy_HoaDon_DTO.ID_DoiTuong = item.ID_DoiTuong;
                            lst.Add(quy_HoaDon_DTO);
                        }
                    }

                    lst = lst.GroupBy(o => o.ID).Select(t => new Quy_HoaDon_ChiTietDTO
                    {
                        ID = t.Select(c => c.ID).FirstOrDefault(),
                        ID_HoaDonLienQuan = t.Where(c => c.ID_HoaDonLienQuan != null).Select(c => c.ID_HoaDonLienQuan).FirstOrDefault(),
                        MaHoaDon = t.Select(c => c.MaHoaDon).FirstOrDefault(),
                        ID_TaiKhoanNganHang = t.Select(c => c.ID_TaiKhoanNganHang).FirstOrDefault(),
                        MaHoaDonHD = t.Select(c => c.MaHoaDonHD).FirstOrDefault(),
                        NguoiNopTien = t.Select(c => c.NguoiNopTien).FirstOrDefault(),
                        TenNhanVien = t.Select(c => c.TenNhanVien).FirstOrDefault(),
                        NgayLapHoaDon = t.Select(c => c.NgayLapHoaDon).FirstOrDefault(),
                        LoaiHoaDon = t.Select(c => c.LoaiHoaDon).FirstOrDefault(),
                        TienMat = t.Sum(c => c.TienMat),
                        TienGui = t.Sum(c => c.TienGui),
                        TienThu = t.Sum(c => c.TienThu),
                        //quy_HoaDon_DTO.TongTienThu = item.TienGui == 0 ? item.TienMat : item.TienGui,
                        TongTienThu = t.Sum(c => c.TongTienThu),
                        //MaKhoanThuChi = s.MaKhoanThuChi
                        NoiDungThu = t.Select(c => c.NoiDungThu).FirstOrDefault(),
                        NguoiSua = t.Select(c => c.NguoiSua).FirstOrDefault(),
                        ID_NhanVien = t.Select(c => c.ID_NhanVien).FirstOrDefault(),
                        TenChiNhanh = t.Select(c => c.TenChiNhanh).FirstOrDefault(),
                        DienThoaiChiNhanh = t.Select(c => c.DienThoaiChiNhanh).FirstOrDefault(),
                        DiaChiChiNhanh = t.Select(c => c.DiaChiChiNhanh).FirstOrDefault(),
                        TrangThai = t.Select(c => c.TrangThai).FirstOrDefault(),
                        ID_KhoanThuChi = t.Select(c => c.ID_KhoanThuChi).FirstOrDefault(),
                        LaKhoanThu = t.Select(c => c.LaKhoanThu).FirstOrDefault(),
                        NoiDungThuChi = t.Select(c => c.NoiDungThuChi).FirstOrDefault(),
                        DiemThanhToan = t.Select(c => c.DiemThanhToan).FirstOrDefault(),
                        GhiChu = t.Select(c => c.GhiChu).FirstOrDefault(),
                        LoaiDoiTuong = t.Select(c => c.LoaiDoiTuong).FirstOrDefault(),
                        HachToanKinhDoanh = t.Select(c => c.HachToanKinhDoanh).FirstOrDefault(),
                        ID_DoiTuong = t.Select(c => c.ID_DoiTuong).FirstOrDefault(),
                    }).ToList();
                    IEnumerable<Quy_HoaDon_ChiTietDTO> iEnumber_Quy_HoaDon = lst;
                    return iEnumber_Quy_HoaDon.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Quy_HoaDon_ChiTietDTO> GetListHoaDons_QuyHD(Guid iddonvi, string maHoaDon, int trangThai, string dayStart, string dayEnd, string idnhanvien, string ghichu, int locthanhtoan, string loaithuchi)
        {
            List<Quy_HoaDon_ChiTietDTO> lst = new List<Quy_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var tbl = from qhd in db.Quy_HoaDon
                              join dv in db.DM_DonVi on qhd.ID_DonVi equals dv.ID into QHD_DV
                              from qhd_dv in QHD_DV.DefaultIfEmpty()
                              join ctq in db.Quy_HoaDon_ChiTiet on qhd.ID equals ctq.ID_HoaDon
                              join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                              from loaithu in LOAITHU.DefaultIfEmpty()
                              where qhd.ID_DonVi == iddonvi
                              select new Quy_HoaDon_ChiTietDTO
                              {
                                  ID = qhd.ID,
                                  ID_DonVi = qhd_dv.ID,
                                  ID_NhanVien = qhd.ID_NhanVien,
                                  MaHoaDon = qhd.MaHoaDon,
                                  NguoiNopTien = qhd.NguoiNopTien,
                                  NgayLapHoaDon = qhd.NgayLapHoaDon,
                                  LoaiHoaDon = qhd.LoaiHoaDon,
                                  TienMat = ctq.TienMat,
                                  TienGui = ctq.TienGui,
                                  TienThu = ctq.TienMat + ctq.TienGui,
                                  TongTienThu = ctq.TienGui == 0 ? ctq.TienMat : ctq.TienGui,
                                  //TongTienThu = s.TongTienThu,
                                  //MaKhoanThuChi = s.MaKhoanThuChi
                                  NoiDungThu = qhd.NoiDungThu,
                                  TrangThai = qhd.TrangThai,
                                  ID_KhoanThuChi = loaithu.ID,
                                  NoiDungThuChi = loaithu.NoiDungThuChi
                              };
                    if (idnhanvien != "undefined")
                    {
                        tbl = tbl.Where(qhd => qhd.ID_NhanVien.ToString().Contains(idnhanvien));
                    }

                    if (loaithuchi != "undefined")
                    {
                        tbl = tbl.Where(qhd => qhd.ID_KhoanThuChi.ToString().Contains(loaithuchi));
                    }

                    if (iddonvi != Guid.Empty && iddonvi != null)
                    {
                        tbl = tbl.Where(qhd => qhd.ID_DonVi == iddonvi);
                    }

                    if (dayStart != null && dayEnd != null && dayStart != string.Empty && dayEnd != string.Empty)
                    {
                        DateTime dtStart = DateTime.Parse(dayStart);
                        DateTime dtEnd = DateTime.Parse(dayEnd);
                        if (dayStart == dayEnd)
                        {
                            tbl = tbl.Where(hd => hd.NgayLapHoaDon.Year == dtStart.Year
                            && hd.NgayLapHoaDon.Month == dtStart.Month
                            && hd.NgayLapHoaDon.Day == dtEnd.Day);
                        }
                        else
                        {
                            tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dtStart && hd.NgayLapHoaDon < dtEnd);
                        }
                    }
                    // trang thai HoaDon
                    switch (trangThai)
                    {
                        case 1: // HT
                            tbl = tbl.Where(hd => hd.LoaiHoaDon == 11);
                            break;
                        case 2: // Huy
                            tbl = tbl.Where(hd => hd.LoaiHoaDon == 12);
                            break;
                        case 3: // HT + Huy
                            break;
                        default: // tam luu
                            tbl = tbl.Where(hd => hd.LoaiHoaDon == 11);
                            break;
                    }
                    switch (locthanhtoan)
                    {
                        case 1: // HT
                            tbl = tbl.Where(hd => hd.TrangThai != false);
                            break;
                        case 2: // Huy
                            tbl = tbl.Where(hd => hd.TrangThai == false);
                            break;
                        case 3: // HT + Huy
                            break;
                        case 4: // HT + Huy
                            break;
                        default: // tam luu
                            tbl = tbl.Where(hd => hd.TrangThai != false);
                            break;
                    }
                    //tbl = tbl.OrderByDescending(dt => dt.NgayLapHoaDon);
                    string stSearch = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                    string stSearchGC = CommonStatic.ConvertToUnSign(ghichu).ToLower();
                    if (tbl != null)
                    {
                        foreach (var item in tbl)
                        {
                            string tenNN = CommonStatic.ConvertToUnSign(item.NguoiNopTien).ToLower();
                            string ghiChu = CommonStatic.ConvertToUnSign(item.NoiDungThu).ToLower();
                            string maHD = CommonStatic.ConvertToUnSign(item.MaHoaDon).ToLower();
                            string tenNNsplit = CommonStatic.GetCharsStart(item.NguoiNopTien).ToLower();
                            string maHDsplit = CommonStatic.GetCharsStart(item.MaHoaDon).ToLower();
                            string ghiChusplit = CommonStatic.GetCharsStart(item.NoiDungThu).ToLower();
                            if ((tenNN.Contains(@stSearch) || maHD.Contains(@stSearch) || tenNNsplit.Contains(@stSearch) || maHDsplit.Contains(@stSearch)) && (ghiChu.Contains(@stSearch) || ghiChusplit.Contains(@stSearch)))
                            {
                                Quy_HoaDon_ChiTietDTO quy_HoaDon_DTO = new Quy_HoaDon_ChiTietDTO();
                                quy_HoaDon_DTO.ID_DonVi = item.ID;
                                quy_HoaDon_DTO.ID = item.ID;
                                quy_HoaDon_DTO.MaHoaDon = item.MaHoaDon;
                                quy_HoaDon_DTO.NguoiNopTien = item.NguoiNopTien;
                                quy_HoaDon_DTO.TenNhanVien = item.TenNhanVien;
                                quy_HoaDon_DTO.NgayLapHoaDon = item.NgayLapHoaDon;
                                quy_HoaDon_DTO.LoaiHoaDon = item.LoaiHoaDon;
                                quy_HoaDon_DTO.TienMat = item.TienMat;
                                quy_HoaDon_DTO.TienGui = item.TienGui;
                                quy_HoaDon_DTO.TienThu = item.TienThu;
                                quy_HoaDon_DTO.TongTienThu = item.TongTienThu;
                                //TongTienThu = s.TongTienThu;
                                //MaKhoanThuChi = s.MaKhoanThuChi
                                quy_HoaDon_DTO.NoiDungThu = item.NoiDungThu;
                                quy_HoaDon_DTO.NguoiSua = item.NguoiSua;
                                quy_HoaDon_DTO.ID_NhanVien = item.ID_NhanVien;
                                quy_HoaDon_DTO.TenChiNhanh = item.TenChiNhanh;
                                quy_HoaDon_DTO.DienThoaiChiNhanh = item.DienThoaiChiNhanh;
                                quy_HoaDon_DTO.DiaChiChiNhanh = item.DiaChiChiNhanh;
                                quy_HoaDon_DTO.TrangThai = item.TrangThai;
                                lst.Add(quy_HoaDon_DTO);
                            }
                        }
                        IEnumerable<Quy_HoaDon_ChiTietDTO> iEnumber_Quy_HoaDon = lst;
                        return iEnumber_Quy_HoaDon.ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetListHoaDons_QuyHD: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public List<DM_NganHangDTO> GetAllNganhang()
        {
            if (db != null)
            {
                var tbl = from nh in db.DM_NganHang
                          select new DM_NganHangDTO
                          {
                              ID = nh.ID,
                              MaNganHang = nh.MaNganHang,
                              TenNganHang = nh.TenNganHang,
                              ChiNhanh = nh.ChiNhanh
                          };
                return tbl.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<DM_TaiKhoanNganHangDTO> GetAllTaiKhoanNganHang()
        {
            if (db != null)
            {
                var tbl = from nh in db.DM_TaiKhoanNganHang
                          select new DM_TaiKhoanNganHangDTO
                          {
                              ID = nh.ID,
                              ID_DonVi = nh.ID_DonVi,
                              ID_NganHang = nh.ID_NganHang,
                              TenChuThe = nh.TenChuThe,
                              SoTaiKhoan = nh.SoTaiKhoan,
                              GhiChu = nh.GhiChu,
                              TaiKhoanPOS = nh.TaiKhoanPOS
                          };
                return tbl.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<Quy_HoaDonDTO> GetLichSuThanhToanSoQuy(Guid idHoaDon)
        {
            if (db != null)
            {
                var tbl = from hd in db.BH_HoaDon
                          join qct in db.Quy_HoaDon_ChiTiet on hd.ID equals qct.ID_HoaDonLienQuan
                          join qhd in db.Quy_HoaDon on qct.ID_HoaDon equals qhd.ID
                          where hd.ID == idHoaDon
                          orderby qhd.NgayLapHoaDon descending
                          select new
                          {
                              ID = qhd.ID,
                              MaHoaDon = qhd.MaHoaDon,
                              NgayLapHoaDon = qhd.NgayLapHoaDon,
                              NguoiNopTien = qhd.NguoiNopTien,
                              TongTienThu = qct.TienThu,
                              TrangThai = qhd.TrangThai
                          };
                return tbl.GroupBy(p => p.ID).Select(t => new Quy_HoaDonDTO
                {
                    ID = t.FirstOrDefault().ID,
                    MaHoaDon = t.FirstOrDefault().MaHoaDon,
                    NgayLapHoaDon = t.FirstOrDefault().NgayLapHoaDon,
                    NguoiNopTien = t.FirstOrDefault().NguoiNopTien,
                    TongTienThu = t.Sum(p => p.TongTienThu),
                    TrangThai = t.FirstOrDefault().TrangThai
                }).OrderByDescending(p => p.NgayLapHoaDon).ToList();
            }
            else
            {
                return null;
            }
        }

        public List<BH_HoaDonDTO> getNgayLapHDByIDLQ(Guid idhoadonlq)
        {
            if (db != null)
            {
                var tbl = from hd in db.BH_HoaDon
                          where hd.ID == idhoadonlq
                          select new BH_HoaDonDTO
                          {
                              NgayLapHoaDon = hd.NgayLapHoaDon
                          };
                return tbl.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<DM_MauInDTO> getlistMauInSQ(Guid iddonvi)
        {
            if (db != null)
            {
                var tbl = from mi in db.DM_MauIn
                          where mi.ID_DonVi == iddonvi && (mi.ID_LoaiChungTu == 11 || mi.ID_LoaiChungTu == 12)
                          select new DM_MauInDTO
                          {
                              ID = mi.ID,
                              ID_DonVi = mi.ID_DonVi,
                              ID_LoaiChungTu = mi.ID_LoaiChungTu,
                              TenMauIn = mi.TenMauIn,
                              DuLieuMauIn = mi.DuLieuMauIn,
                              LaMacDinh = mi.LaMacDinh
                          };
                return tbl.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<SP_GetListCashFlow> GetListCashFlow_Paging(ParamCashFlow lstParam)
        {
            string txtSearch = lstParam.TxtSearch;
            string searchSql, loaisoquy;
            string loaiChungTu = string.Empty;
            string loaiNapTien = "0";
            string hachtoan = string.Empty;
            string trangthai = string.Empty;
            string idDonVis = string.Join(",", lstParam.IDDonVis);

            switch (lstParam.LoaiSoQuy)
            {
                case 0: // chuyenkhoan
                    loaisoquy = "0,2";
                    break;
                case 1: // mat
                    loaisoquy = "1,2";
                    break;
                default://all
                    loaisoquy = "0,1,2,3";
                    break;
            }

            switch (lstParam.LoaiChungTu)
            {
                case 1: // thu
                    loaiChungTu = "11";
                    break;
                case 2: // chi
                    loaiChungTu = "12";
                    break;
            }
            // todo
            if (lstParam.LoaiNapTien != null)
            {
                loaiNapTien = lstParam.LoaiNapTien;
            }

            switch (lstParam.TrangThaiSoQuy)
            {
                case 1: // ht
                    trangthai = "11";
                    break;
                case 2: // huy
                    trangthai = "10";
                    break;
                case 3:// ht + huy (all)
                case 4:
                    break;
            }

            switch (lstParam.TrangThaiHachToan)
            {
                case 0: // dua vao hach toan
                    hachtoan = "11";
                    break;
                case 1: // khong hach toan
                    hachtoan = "10";
                    break;
                case 2:// all
                    break;
            }

            //if (txtSearch == null || txtSearch == string.Empty)
            //{
            //    searchSql = "%%";
            //}
            //else
            //{
            //    searchSql = string.Concat("%", txtSearch, "%");
            //}

            List<SqlParameter> lstPr = new List<SqlParameter>();
            lstPr.Add(new SqlParameter("IDDonVis", idDonVis));
            lstPr.Add(new SqlParameter("ID_NhanVien", lstParam.ID_NhanVien));
            lstPr.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            lstPr.Add(new SqlParameter("ID_TaiKhoanNganHang", lstParam.ID_TaiKhoanNganHang));
            lstPr.Add(new SqlParameter("ID_KhoanThuChi", lstParam.ID_KhoanThuChi));
            lstPr.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
            lstPr.Add(new SqlParameter("DateTo", lstParam.DateTo));
            lstPr.Add(new SqlParameter("LoaiSoQuy", loaisoquy));
            lstPr.Add(new SqlParameter("LoaiChungTu", loaiChungTu));
            lstPr.Add(new SqlParameter("TrangThaiSoQuy", trangthai));
            lstPr.Add(new SqlParameter("TrangThaiHachToan", hachtoan));
            lstPr.Add(new SqlParameter("TxtSearch", txtSearch ?? (object)DBNull.Value));
            lstPr.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            lstPr.Add(new SqlParameter("PageSize", lstParam.PageSize));
            lstPr.Add(new SqlParameter("LoaiNapTien", loaiNapTien));
            List<SP_GetListCashFlow> data = db.Database.SqlQuery<SP_GetListCashFlow>("exec GetListCashFlow_Paging @IDDonVis, @ID_NhanVien, @ID_NhanVienLogin, @ID_TaiKhoanNganHang," +
                "@ID_KhoanThuChi, @DateFrom, @DateTo, @LoaiSoQuy, @LoaiChungTu, @TrangThaiSoQuy, @TrangThaiHachToan, @TxtSearch," +
                " @CurrentPage, @PageSize, @LoaiNapTien", lstPr.ToArray()).ToList();
            return data;
        }

        public List<Quy_HoaDon_ChiTietDTO> GetAllQuyHoaDon(int loaiHoaDon, int loai, string maHoaDon, int trangThai, string dayStart, string dayEnd, string idnhanvien, Guid iddonvi, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string idTKNganHang, string columsort, string sort, int kinhdoanh)
        {
            List<Quy_HoaDon_ChiTietDTO> lst = new List<Quy_HoaDon_ChiTietDTO>();
            if (db == null)
            {
                return null;
            }
            else
            {
                IEnumerable<Quy_HoaDon_ChiTietDTO> tbl = null;
                if (idTKNganHang == "undefined")
                {
                    tbl = from qhd in db.Quy_HoaDon.Where(p => p.PhieuDieuChinhCongNo == null || p.PhieuDieuChinhCongNo == 0)
                          join ctq in db.Quy_HoaDon_ChiTiet.Where(p => (p.DiemThanhToan == null || p.DiemThanhToan == 0) && p.ThuTuThe == 0) on qhd.ID equals ctq.ID_HoaDon
                          join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                          from loaithu in LOAITHU.DefaultIfEmpty()
                          join hd in db.BH_HoaDon on ctq.ID_HoaDonLienQuan equals hd.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join dv in db.DM_DonVi on qhd.ID_DonVi equals dv.ID into QHD_DV
                          from qhd_dv in QHD_DV.DefaultIfEmpty()
                          join dt in db.DM_DoiTuong on ctq.ID_DoiTuong equals dt.ID into DT_NCC
                          from dt_ncc in DT_NCC.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on qhd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          join tknh in db.DM_TaiKhoanNganHang on ctq.ID_TaiKhoanNganHang equals tknh.ID into TK_NH
                          from tk_nh in TK_NH.DefaultIfEmpty()
                          orderby qhd.NgayLapHoaDon descending
                          group new { ctq } by new
                          {
                              ID = qhd.ID,
                              ID_DonVi = qhd_dv.ID,
                              ID_NhanVien = qhd.ID_NhanVien,
                              ID_HoaDonLienQuan = ctq.ID_HoaDonLienQuan,
                              MaHoaDon = qhd.MaHoaDon,
                              MaHoaDonHD = hd_dt.MaHoaDon,
                              ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang,
                              NguoiNopTien = qhd.NguoiNopTien,
                              NgayLapHoaDon = qhd.NgayLapHoaDon,
                              TongTienThu = qhd.TongTienThu,
                              LoaiHoaDon = qhd.LoaiHoaDon,
                              NoiDungThu = qhd.NoiDungThu,
                              TrangThai = qhd.TrangThai,
                              ID_KhoanThuChi = loaithu.ID,
                              LaKhoanThu = loaithu.LaKhoanThu,
                              NoiDungThuChi = loaithu.NoiDungThuChi,
                              TenNhanVien = hd_nv.TenNhanVien,
                              NguoiSua = qhd.NguoiSua,
                              TenChiNhanh = qhd_dv.TenDonVi,
                              DiaChiChiNhanh = qhd_dv.DiaChi,
                              DienThoaiChiNhanh = qhd_dv.SoDienThoai,
                              SoDienThoai = dt_ncc.DienThoai,
                              DiaChi = dt_ncc.DiaChi,
                              DiemThanhToan = ctq.DiemThanhToan,
                              HachToanKinhDoanh = qhd.HachToanKinhDoanh,
                              MaDoiTuong = ctq.ID_NhanVien != null ? hd_nv.MaNhanVien : dt_ncc.MaDoiTuong,
                              LoaiDoiTuong = ctq.ID_NhanVien != null ? 3 : ctq.ID_DoiTuong != null ? (dt_ncc.LoaiDoiTuong == 1 ? 1 : 2) : 0,
                              ID_DoiTuong = ctq.ID_NhanVien != null ? ctq.ID_NhanVien : ctq.ID_DoiTuong,
                              TenChuThe = tk_nh.TenChuThe,
                              TaiKhoanPOS = tk_nh.TaiKhoanPOS

                          } into g

                          select new Quy_HoaDon_ChiTietDTO
                          {
                              ID_DonVi = g.Key.ID_DonVi,
                              ID = g.Key.ID,
                              ID_HoaDonLienQuan = g.Key.ID_HoaDonLienQuan,
                              MaHoaDon = g.Key.MaHoaDon,
                              ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang,
                              MaHoaDonHD = g.Key.MaHoaDonHD,
                              NguoiNopTien = g.Key.NguoiNopTien,
                              TenNhanVien = g.Key.TenNhanVien,
                              NgayLapHoaDon = g.Key.NgayLapHoaDon,
                              LoaiHoaDon = g.Key.LoaiHoaDon,
                              TienMat = g.Sum(x => x.ctq.TienMat),
                              TienGui = g.Sum(x => x.ctq.TienGui),
                              TienThu = g.Sum(x => x.ctq.TienThu),
                              TongTienThu = g.Sum(x => x.ctq.TienMat) + g.Sum(x => x.ctq.TienGui),
                              NoiDungThu = g.Key.NoiDungThu,
                              NguoiSua = g.Key.NguoiSua,
                              ID_NhanVien = g.Key.ID_NhanVien,
                              TenChiNhanh = g.Key.TenChiNhanh,
                              DienThoaiChiNhanh = g.Key.DienThoaiChiNhanh,
                              DiaChiChiNhanh = g.Key.DiaChiChiNhanh,
                              SoDienThoai = g.Key.SoDienThoai,
                              TrangThai = g.Key.TrangThai,
                              ID_KhoanThuChi = g.Key.ID_KhoanThuChi,
                              LaKhoanThu = g.Key.LaKhoanThu,
                              NoiDungThuChi = g.Key.NoiDungThuChi,
                              DiemThanhToan = g.Key.DiemThanhToan,
                              LoaiDoiTuong = g.Key.LoaiDoiTuong,
                              GhiChu = "Đã thanh toán",
                              HachToanKinhDoanh = g.Key.HachToanKinhDoanh,
                              ID_DoiTuong = g.Key.ID_DoiTuong,
                              TenChuThe = g.Key.TenChuThe,
                              TaiKhoanPOS = g.Key.TaiKhoanPOS,
                              DiaChiKhachHang = g.Key.DiaChi,
                              MaDoiTuong = g.Key.MaDoiTuong
                              //ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang
                          };
                }
                else
                {
                    tbl = from qhd in db.Quy_HoaDon.Where(p => p.PhieuDieuChinhCongNo == null || p.PhieuDieuChinhCongNo == 0)
                          join ctq in db.Quy_HoaDon_ChiTiet.Where(p => p.ID_TaiKhoanNganHang.ToString() == idTKNganHang && (p.DiemThanhToan == null || p.DiemThanhToan == 0) && p.ThuTuThe == 0) on qhd.ID equals ctq.ID_HoaDon
                          join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                          from loaithu in LOAITHU.DefaultIfEmpty()
                          join hd in db.BH_HoaDon on ctq.ID_HoaDonLienQuan equals hd.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join dv in db.DM_DonVi on qhd.ID_DonVi equals dv.ID into QHD_DV
                          from qhd_dv in QHD_DV.DefaultIfEmpty()
                          join dt in db.DM_DoiTuong on ctq.ID_DoiTuong equals dt.ID into DT_NCC
                          from dt_ncc in DT_NCC.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on qhd.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          join tknh in db.DM_TaiKhoanNganHang on ctq.ID_TaiKhoanNganHang equals tknh.ID into TK_NH
                          from tk_nh in TK_NH.DefaultIfEmpty()
                          orderby qhd.NgayLapHoaDon descending
                          group new { ctq } by new
                          {
                              ID = qhd.ID,
                              ID_DonVi = qhd_dv.ID,
                              ID_NhanVien = qhd.ID_NhanVien,
                              MaHoaDon = qhd.MaHoaDon,
                              ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang,
                              MaHoaDonHD = hd_dt.MaHoaDon,
                              NguoiNopTien = qhd.NguoiNopTien,
                              NgayLapHoaDon = qhd.NgayLapHoaDon,
                              TongTienThu = qhd.TongTienThu,
                              LoaiHoaDon = qhd.LoaiHoaDon,
                              NoiDungThu = qhd.NoiDungThu,
                              TrangThai = qhd.TrangThai,
                              ID_KhoanThuChi = loaithu.ID,
                              LaKhoanThu = loaithu.LaKhoanThu,
                              NoiDungThuChi = loaithu.NoiDungThuChi,
                              TenNhanVien = hd_nv.TenNhanVien,
                              NguoiSua = qhd.NguoiSua,
                              TenChiNhanh = qhd_dv.TenDonVi,
                              DiaChiChiNhanh = qhd_dv.DiaChi,
                              DienThoaiChiNhanh = qhd_dv.SoDienThoai,
                              SoDienThoai = dt_ncc.DienThoai,
                              DiemThanhToan = ctq.DiemThanhToan,
                              ID_HoaDonLienQuan = ctq.ID_HoaDonLienQuan,
                              HachToanKinhDoanh = qhd.HachToanKinhDoanh,
                              MaDoiTuong = ctq.ID_NhanVien != null ? hd_nv.MaNhanVien : dt_ncc.MaDoiTuong,
                              LoaiDoiTuong = ctq.ID_NhanVien != null ? 3 : ctq.ID_DoiTuong != null ? (dt_ncc.LoaiDoiTuong == 1 ? 1 : 2) : 0,
                              ID_DoiTuong = ctq.ID_NhanVien != null ? ctq.ID_NhanVien : ctq.ID_DoiTuong,
                              TenChuThe = tk_nh.TenChuThe,
                              TaiKhoanPOS = tk_nh.TaiKhoanPOS,
                              DiaChi = dt_ncc.DiaChi
                              //ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang

                          } into g

                          select new Quy_HoaDon_ChiTietDTO
                          {
                              ID_DonVi = g.Key.ID_DonVi,
                              ID = g.Key.ID,
                              ID_HoaDonLienQuan = g.Key.ID_HoaDonLienQuan,
                              MaHoaDon = g.Key.MaHoaDon,
                              ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang,
                              MaHoaDonHD = g.Key.MaHoaDonHD,
                              NguoiNopTien = g.Key.NguoiNopTien,
                              TenNhanVien = g.Key.TenNhanVien,
                              NgayLapHoaDon = g.Key.NgayLapHoaDon,
                              LoaiHoaDon = g.Key.LoaiHoaDon,
                              TienMat = g.Sum(x => x.ctq.TienMat),
                              TienGui = g.Sum(x => x.ctq.TienGui),
                              TienThu = g.Sum(x => x.ctq.TienThu),
                              TongTienThu = g.Sum(x => x.ctq.TienMat) + g.Sum(x => x.ctq.TienGui),
                              NoiDungThu = g.Key.NoiDungThu,
                              NguoiSua = g.Key.NguoiSua,
                              ID_NhanVien = g.Key.ID_NhanVien,
                              TenChiNhanh = g.Key.TenChiNhanh,
                              DienThoaiChiNhanh = g.Key.DienThoaiChiNhanh,
                              SoDienThoai = g.Key.SoDienThoai,
                              DiaChiChiNhanh = g.Key.DiaChiChiNhanh,
                              TrangThai = g.Key.TrangThai,
                              ID_KhoanThuChi = g.Key.ID_KhoanThuChi,
                              LaKhoanThu = g.Key.LaKhoanThu,
                              NoiDungThuChi = g.Key.NoiDungThuChi,
                              DiemThanhToan = g.Key.DiemThanhToan,
                              LoaiDoiTuong = g.Key.LoaiDoiTuong,
                              GhiChu = "Đã thanh toán",
                              HachToanKinhDoanh = g.Key.HachToanKinhDoanh,
                              ID_DoiTuong = g.Key.ID_DoiTuong,
                              TenChuThe = g.Key.TenChuThe,
                              TaiKhoanPOS = g.Key.TaiKhoanPOS,
                              DiaChiKhachHang = g.Key.DiaChi,
                              MaDoiTuong = g.Key.MaDoiTuong
                              //ID_TaiKhoanNganHang = g.Key.ID_TaiKhoanNganHang
                          };
                }

                if (idnhanvien != "undefined")
                {
                    tbl = tbl.Where(qhd => qhd.ID_NhanVien.ToString().Contains(idnhanvien));
                }

                if (loaithuchi != "undefined")
                {
                    tbl = tbl.Where(qhd => qhd.ID_KhoanThuChi.ToString().Contains(loaithuchi));
                }

                List<Guid> lstIDCN = new List<Guid>();
                if (arrChiNhanh != null)
                {
                    var arrIDCN = arrChiNhanh.Split(',');
                    for (int i = 0; i < arrIDCN.Length; i++)
                    {
                        lstIDCN.Add(new Guid(arrIDCN[i]));
                    }
                }
                if (lstIDCN.Count > 0)
                {
                    tbl = tbl.Where(p => p.ID_DonVi != null).Where(hd => lstIDCN.Contains(hd.ID_DonVi.Value));
                }
                else
                {
                    if (iddonvi != Guid.Empty && iddonvi != null)
                    {
                        tbl = tbl.Where(qhd => qhd.ID_DonVi == iddonvi);
                    }
                }


                if (dayStart != null && dayEnd != null && dayStart != string.Empty && dayEnd != string.Empty)
                {
                    DateTime dtStart = DateTime.Parse(dayStart);
                    DateTime dtEnd = DateTime.Parse(dayEnd);
                    if (dayStart == dayEnd)
                    {
                        tbl = tbl.Where(hd => hd.NgayLapHoaDon.Year == dtStart.Year
                        && hd.NgayLapHoaDon.Month == dtStart.Month
                        && hd.NgayLapHoaDon.Day == dtEnd.Day);
                    }
                    else
                    {
                        tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dtStart && hd.NgayLapHoaDon < dtEnd);
                    }
                }
                // trang thai HoaDon
                switch (trangThai)
                {
                    case 1: // HT
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 11);
                        break;
                    case 2: // Huy
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 12);
                        break;
                    case 3: // HT + Huy
                        break;
                    default: // tam luu
                        break;
                }

                switch (locthanhtoan)
                {
                    case 1: // HT
                        tbl = tbl.Where(hd => hd.TrangThai == true || hd.TrangThai == null);
                        break;
                    case 2: // Huy
                        tbl = tbl.Where(hd => hd.TrangThai == false);
                        break;
                    case 3: // HT + Huy
                        break;
                    case 4: // HT + Huy
                        break;
                    default: // tam luu
                        tbl = tbl.Where(hd => hd.TrangThai == true || hd.TrangThai == null);
                        break;
                }

                switch (kinhdoanh)
                {
                    case 0:
                        tbl = tbl.Where(hd => hd.HachToanKinhDoanh == null || hd.HachToanKinhDoanh == true);
                        break;
                    case 1:
                        tbl = tbl.Where(hd => hd.HachToanKinhDoanh == false);
                        break;
                    case 2:
                        break;
                }
                //tbl = tbl.OrderByDescending(dt => dt.NgayLapHoaDon);
                string stSearch = CommonStatic.ConvertToUnSign(maHoaDon).ToLower();
                string stSearchGC = CommonStatic.ConvertToUnSign(ghichu).ToLower();
                if (tbl != null)
                {
                    foreach (var item in tbl)
                    {
                        string tenNN = CommonStatic.ConvertToUnSign(item.NguoiNopTien).ToLower();
                        string ghiChu = CommonStatic.ConvertToUnSign(item.NoiDungThu).ToLower();
                        string maHD = CommonStatic.ConvertToUnSign(item.MaHoaDon).ToLower();
                        string tenNNsplit = CommonStatic.GetCharsStart(item.NguoiNopTien).ToLower();
                        string maHDsplit = CommonStatic.GetCharsStart(item.MaHoaDon).ToLower();
                        string ghiChusplit = CommonStatic.GetCharsStart(item.NoiDungThu).ToLower();
                        if ((tenNN.Contains(@stSearch.Trim()) || maHD.Contains(@stSearch.Trim()) || tenNNsplit.Contains(@stSearch.Trim()) || maHDsplit.Contains(@stSearch.Trim())) || (ghiChu.Contains(@stSearch.Trim()) || ghiChusplit.Contains(@stSearch.Trim())))
                        {
                            Quy_HoaDon_ChiTietDTO quy_HoaDon_DTO = new Quy_HoaDon_ChiTietDTO();
                            quy_HoaDon_DTO.ID_DonVi = item.ID;
                            quy_HoaDon_DTO.ID = item.ID;
                            quy_HoaDon_DTO.ID_HoaDonLienQuan = item.ID_HoaDonLienQuan;
                            quy_HoaDon_DTO.MaHoaDon = item.MaHoaDon;
                            quy_HoaDon_DTO.ID_TaiKhoanNganHang = item.ID_TaiKhoanNganHang;
                            quy_HoaDon_DTO.MaHoaDonHD = item.MaHoaDonHD;
                            quy_HoaDon_DTO.NguoiNopTien = item.NguoiNopTien;
                            quy_HoaDon_DTO.TenNhanVien = item.TenNhanVien;
                            quy_HoaDon_DTO.NgayLapHoaDon = item.NgayLapHoaDon;
                            quy_HoaDon_DTO.LoaiHoaDon = item.LoaiHoaDon;
                            quy_HoaDon_DTO.TienMat = item.TienMat;
                            quy_HoaDon_DTO.TienGui = item.TienGui;
                            quy_HoaDon_DTO.TienThu = item.TienThu;
                            //quy_HoaDon_DTO.TongTienThu = item.TienGui == 0 ? item.TienMat : item.TienGui;
                            quy_HoaDon_DTO.TongTienThu = item.TongTienThu;
                            //MaKhoanThuChi = s.MaKhoanThuChi
                            quy_HoaDon_DTO.NoiDungThu = item.NoiDungThu;
                            quy_HoaDon_DTO.NguoiSua = item.NguoiSua;
                            quy_HoaDon_DTO.ID_NhanVien = item.ID_NhanVien;
                            quy_HoaDon_DTO.TenChiNhanh = item.TenChiNhanh;
                            quy_HoaDon_DTO.DienThoaiChiNhanh = item.DienThoaiChiNhanh;
                            quy_HoaDon_DTO.SoDienThoai = item.SoDienThoai;
                            quy_HoaDon_DTO.DiaChiChiNhanh = item.DiaChiChiNhanh;
                            quy_HoaDon_DTO.TrangThai = item.TrangThai;
                            quy_HoaDon_DTO.ID_KhoanThuChi = item.ID_KhoanThuChi;
                            quy_HoaDon_DTO.LaKhoanThu = item.LaKhoanThu;
                            quy_HoaDon_DTO.NoiDungThuChi = item.NoiDungThuChi;
                            quy_HoaDon_DTO.DiemThanhToan = item.DiemThanhToan;
                            quy_HoaDon_DTO.GhiChu = item.GhiChu;
                            quy_HoaDon_DTO.LoaiDoiTuong = item.LoaiDoiTuong;
                            quy_HoaDon_DTO.HachToanKinhDoanh = item.HachToanKinhDoanh;
                            quy_HoaDon_DTO.ID_DoiTuong = item.ID_DoiTuong;
                            quy_HoaDon_DTO.TenChuThe = item.TenChuThe;
                            quy_HoaDon_DTO.TaiKhoanPOS = item.TaiKhoanPOS;
                            quy_HoaDon_DTO.DiaChiKhachHang = item.DiaChiKhachHang;
                            quy_HoaDon_DTO.MaDoiTuong = item.MaDoiTuong;
                            lst.Add(quy_HoaDon_DTO);
                        }
                    }

                    lst = lst.GroupBy(o => o.ID).Select(t => new Quy_HoaDon_ChiTietDTO
                    {
                        ID = t.Select(c => c.ID).FirstOrDefault(),
                        ID_HoaDonLienQuan = t.Where(c => c.ID_HoaDonLienQuan != null).Select(c => c.ID_HoaDonLienQuan).FirstOrDefault(),
                        MaHoaDon = t.Select(c => c.MaHoaDon).FirstOrDefault(),
                        ID_TaiKhoanNganHang = t.Select(c => c.ID_TaiKhoanNganHang).FirstOrDefault(),
                        MaHoaDonHD = t.Select(c => c.MaHoaDonHD).FirstOrDefault(),
                        NguoiNopTien = t.Select(c => c.NguoiNopTien).FirstOrDefault(),
                        TenNhanVien = t.Select(c => c.TenNhanVien).FirstOrDefault(),
                        NgayLapHoaDon = t.Select(c => c.NgayLapHoaDon).FirstOrDefault(),
                        LoaiHoaDon = t.Select(c => c.LoaiHoaDon).FirstOrDefault(),
                        TienMat = t.Sum(c => c.TienMat),
                        TienGui = t.Sum(c => c.TienGui),
                        TienThu = t.Sum(c => c.TienThu),
                        //quy_HoaDon_DTO.TongTienThu = item.TienGui == 0 ? item.TienMat : item.TienGui,
                        TongTienThu = t.Sum(c => c.TongTienThu),
                        //MaKhoanThuChi = s.MaKhoanThuChi
                        NoiDungThu = t.Select(c => c.NoiDungThu).FirstOrDefault(),
                        NguoiSua = t.Select(c => c.NguoiSua).FirstOrDefault(),
                        ID_NhanVien = t.Select(c => c.ID_NhanVien).FirstOrDefault(),
                        TenChiNhanh = t.Select(c => c.TenChiNhanh).FirstOrDefault(),
                        DienThoaiChiNhanh = t.Select(c => c.DienThoaiChiNhanh).FirstOrDefault(),
                        SoDienThoai = t.Select(c => c.SoDienThoai).FirstOrDefault(),
                        DiaChiChiNhanh = t.Select(c => c.DiaChiChiNhanh).FirstOrDefault(),
                        TrangThai = t.Select(c => c.TrangThai).FirstOrDefault(),
                        ID_KhoanThuChi = t.Select(c => c.ID_KhoanThuChi).FirstOrDefault(),
                        LaKhoanThu = t.Select(c => c.LaKhoanThu).FirstOrDefault(),
                        NoiDungThuChi = t.Select(c => c.NoiDungThuChi).FirstOrDefault(),
                        DiemThanhToan = t.Select(c => c.DiemThanhToan).FirstOrDefault(),
                        GhiChu = t.Select(c => c.GhiChu).FirstOrDefault(),
                        LoaiDoiTuong = t.Select(c => c.LoaiDoiTuong).FirstOrDefault(),
                        HachToanKinhDoanh = t.Select(c => c.HachToanKinhDoanh).FirstOrDefault(),
                        ID_DoiTuong = t.Select(c => c.ID_DoiTuong).FirstOrDefault(),
                        DiaChiKhachHang = t.Select(c => c.DiaChiKhachHang).FirstOrDefault(),
                        TenTaiKhoanPOS = t.Select(c => c.ID_TaiKhoanNganHang).FirstOrDefault() != null ? t.Where(p => p.TaiKhoanPOS == true).Select(c => c.TenChuThe).FirstOrDefault() : "",
                        TenTaiKhoanNOTPOS = t.Select(c => c.ID_TaiKhoanNganHang).FirstOrDefault() != null ? t.Where(p => p.TaiKhoanPOS == false).Select(c => c.TenChuThe).FirstOrDefault() : "",
                        MaDoiTuong = t.Select(p => p.MaDoiTuong).FirstOrDefault()
                    }).ToList();

                    if (sort != "null")
                    {
                        if (sort == "0")
                        {
                            if (columsort == "MaPhieu")
                            {
                                lst = lst.OrderBy(p => p.MaHoaDon).ToList();
                            }
                            if (columsort == "ThoiGian")
                            {
                                lst = lst.OrderBy(p => p.NgayLapHoaDon).ToList();
                            }
                            if (columsort == "LoaiThuChi")
                            {
                                lst = lst.OrderBy(p => p.NoiDungThuChi).ToList();
                            }
                            if (columsort == "NguoiNopTien")
                            {
                                lst = lst.OrderBy(p => p.NguoiNopTien).ToList();
                            }
                            if (columsort == "ChiNhanh")
                            {
                                lst = lst.OrderBy(p => p.TenChiNhanh).ToList();
                            }
                            if (columsort == "GiaTri")
                            {
                                if (loai == 0)
                                {
                                    lst = lst.OrderBy(p => p.TienGui).ToList();
                                }
                                if (loai == 1)
                                {
                                    lst = lst.OrderBy(p => p.TienMat).ToList();
                                }
                                if (loai == 2)
                                {
                                    lst = lst.OrderBy(p => p.TongTienThu).ToList();
                                }
                            }
                        }
                        else
                        {
                            if (columsort == "MaPhieu")
                            {
                                lst = lst.OrderByDescending(p => p.MaHoaDon).ToList();
                            }
                            if (columsort == "ThoiGian")
                            {
                                lst = lst.OrderByDescending(p => p.NgayLapHoaDon).ToList();
                            }
                            if (columsort == "LoaiThuChi")
                            {
                                lst = lst.OrderByDescending(p => p.NoiDungThuChi).ToList();
                            }
                            if (columsort == "NguoiNopTien")
                            {
                                lst = lst.OrderByDescending(p => p.NguoiNopTien).ToList();
                            }
                            if (columsort == "ChiNhanh")
                            {
                                lst = lst.OrderByDescending(p => p.TenChiNhanh).ToList();
                            }
                            if (columsort == "GiaTri")
                            {
                                if (loai == 0)
                                {
                                    lst = lst.OrderByDescending(p => p.TienGui).ToList();
                                }
                                if (loai == 1)
                                {
                                    lst = lst.OrderByDescending(p => p.TienMat).ToList();
                                }
                                if (loai == 2)
                                {
                                    lst = lst.OrderByDescending(p => p.TongTienThu).ToList();
                                }
                            }
                        }
                    }
                    else
                    {
                        lst = lst.OrderByDescending(x => x.NgayLapHoaDon).ToList();
                    }
                    IEnumerable<Quy_HoaDon_ChiTietDTO> iEnumber_Quy_HoaDon = lst;
                    return iEnumber_Quy_HoaDon.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Quy_HoaDon_ChiTietDTO> GetQuyChiTiet_byIDQuy(Guid id)
        {
            SqlParameter param = new SqlParameter("ID", id);
            return db.Database.SqlQuery<Quy_HoaDon_ChiTietDTO>("GetQuyChiTiet_byIDQuy @ID", param).ToList();
        }
        public List<Quy_HoaDon_ChiTietDTO> GetCT_QuyHoaDon(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                var tbl = from qhd in db.Quy_HoaDon
                          join ctq in db.Quy_HoaDon_ChiTiet on qhd.ID equals ctq.ID_HoaDon
                          join qktc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals qktc.ID into LOAITHU
                          from loaithu in LOAITHU.DefaultIfEmpty()
                          join hd in db.BH_HoaDon on ctq.ID_HoaDonLienQuan equals hd.ID into HD_DT
                          from hd_dt in HD_DT.DefaultIfEmpty()
                          join nv in db.NS_NhanVien on hd_dt.ID_NhanVien equals nv.ID into HD_NV
                          from hd_nv in HD_NV.DefaultIfEmpty()
                          join dt in db.DM_DoiTuong on ctq.ID_DoiTuong equals dt.ID into DT_NCC
                          from dt_ncc in DT_NCC.DefaultIfEmpty()
                          join nvloai in db.NS_NhanVien on ctq.ID_NhanVien equals nvloai.ID into NV_LOAI
                          from nvct in NV_LOAI.DefaultIfEmpty()
                          where qhd.ID == id

                          //join tc in db.Quy_KhoanThuChi on ctq.ID_KhoanThuChi equals tc.ID
                          select new Quy_HoaDon_ChiTietDTO
                          {
                              ID = qhd.ID,
                              MaHoaDon = qhd.MaHoaDon,
                              NguoiNopTien = qhd.NguoiNopTien,
                              NgayLapHoaDon = hd_dt.NgayLapHoaDon == null ? qhd.NgayLapHoaDon : hd_dt.NgayLapHoaDon,
                              LoaiHoaDon = qhd.LoaiHoaDon,
                              LoaiDoiTuong = nvct.ID != null ? 3 : (dt_ncc.ID != null && dt_ncc.ID != Guid.Empty) ? (dt_ncc.LoaiDoiTuong == 1 ? 1 : 2) : 0,
                              TienMat = ctq.TienMat,
                              TienGui = ctq.TienGui,
                              TienThu = ctq.TienGui == 0 ? (ctq.TienMat == 0 ? ctq.TienThu : ctq.TienMat) : ctq.TienGui,
                              PhuongThuc = ctq.TienMat == 0 ? (ctq.TienGui == 0 ? (ctq.ThuTuThe > 0 ? "Thu từ thẻ" : "Đổi điểm") : "Tiền ngân hàng") : "Tiền mặt",
                              TongTienThu = hd_dt.PhaiThanhToan.ToString() == "" ? 0 : hd_dt.PhaiThanhToan,
                              NoiDungThu = qhd.NoiDungThu,
                              ID_HoaDonLienQuan = ctq.ID_HoaDonLienQuan,
                              ID_KhoanThuChi = loaithu.ID,
                              NoiDungThuChi = loaithu.NoiDungThuChi,
                              LaKhoanThu = loaithu.LaKhoanThu,
                              ID_NhanVien = qhd.ID_NhanVien,
                              ID_DoiTuong = dt_ncc.ID,
                              ID_NhanVienCT = nvct.ID,
                              GhiChu = "Đã thanh toán",
                              MaHoaDonHD = hd_dt.MaHoaDon == null ? "" : hd_dt.MaHoaDon,
                              NguoiSua = qhd.NguoiSua,
                              NguoiTao = qhd.NguoiTao,
                              TenNhanVien = hd_nv.TenNhanVien == null ? "" : hd_nv.TenNhanVien,
                              DaChi = 0,
                              ID_TaiKhoanNganHang = ctq.ID_TaiKhoanNganHang
                          };

                List<Quy_HoaDon_ChiTietDTO> tblchitiet = tbl.OrderBy(p => p.MaHoaDonHD).ToList();
                foreach (var item in tblchitiet)
                {
                    if (item.ID_HoaDonLienQuan.HasValue)
                    {
                        item.DaChi = _classQHDCT.SelectDaChiByID_HoaDonLienQuan(item.ID_HoaDonLienQuan.Value, item.NgayLapHoaDon);
                    }
                    else
                    {
                        item.MaHoaDonHD = item.LoaiHoaDon == 11 ? "Thu thêm" : "Chi thêm";
                    }
                }
                return tblchitiet;
            }
        }


        public bool Quy_HoaDOnExists(Guid id)
        {

            if (db == null)
            {
                return false;
            }
            else
            {

                return db.Quy_HoaDon.Count(e => e.ID == id) > 0;
            }
        }

        public bool Check_MaSoQuyExist(string maSoQuy, Guid? id = null)
        {

            if (db == null)
            {
                return false;
            }
            else
            {
                if (id != null && id != Guid.Empty)
                {
                    return db.Quy_HoaDon.Where(x => x.ID != id).Count(e => e.MaHoaDon == maSoQuy) > 0;
                }
                else
                {
                    return db.Quy_HoaDon.Count(e => e.MaHoaDon == maSoQuy) > 0;
                }
            }
        }

        /// <summary>
        /// Get list Quy_hoaDon (Nếu HD tạo từ HĐ đặt hàng --> bind HD đặt hàng với mã {Chuyển tạm ứng})
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Quy_HoaDon> GetQuyHoaDon_byIDHoaDon(Guid id, Guid? idHoadonParent = null)
        {
            List<Quy_HoaDon> lstReturn = new List<Quy_HoaDon>();
            if (db == null)
            {
                return null;
            }
            else
            {
                // get HD was create from HD DatHang
                var data = from hd in db.BH_HoaDon
                           where hd.ID_HoaDon == idHoadonParent
                           orderby hd.NgayLapHoaDon
                           select hd;

                var isHDFirst = false;
                if (data != null && data.Count() > 0)
                {
                    // if is HD was create first
                    var idHDFirst = data.FirstOrDefault().ID;
                    if (idHDFirst == id)
                    {
                        isHDFirst = true;
                    }
                }

                // get phieuthu/chi from dathang
                var tblCoc = from qhd in db.Quy_HoaDon
                             join qct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qct.ID_HoaDon
                             where qct.ID_HoaDonLienQuan == idHoadonParent && isHDFirst == true
                             && qhd.TrangThai != false
                             group new { qct } by new
                             {
                                 MaHoaDon = qhd.MaHoaDon,
                                 NgayLapHoaDon = qhd.NgayLapHoaDon,
                                 NguoiTao = qhd.NguoiTao,
                                 NgaySua = qhd.NgaySua,
                                 NguoiSua = qhd.NguoiSua,
                                 TrangThai = qhd.TrangThai,
                                 ID = qhd.ID,
                                 LoaiHoaDon = qhd.LoaiHoaDon,
                             };
                // get phieuthu/chi of hoadon
                var tbl2 = from qhd in db.Quy_HoaDon
                           join qct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qct.ID_HoaDon
                           where qct.ID_HoaDonLienQuan == id
                           group new { qct } by new
                           {
                               MaHoaDon = qhd.MaHoaDon,
                               NgayLapHoaDon = qhd.NgayLapHoaDon,
                               NguoiTao = qhd.NguoiTao,
                               NgaySua = qhd.NgaySua,
                               NguoiSua = qhd.NguoiSua,
                               TrangThai = qhd.TrangThai,
                               ID = qhd.ID,
                               LoaiHoaDon = qhd.LoaiHoaDon,
                           };


                foreach (var item in tblCoc)
                {
                    Quy_HoaDon quyHD = new Quy_HoaDon();
                    quyHD.ID = item.Key.ID;
                    quyHD.MaHoaDon = item.Key.MaHoaDon;
                    quyHD.NgayLapHoaDon = item.Key.NgayLapHoaDon;
                    quyHD.NguoiTao = item.Key.NguoiTao;
                    quyHD.NgaySua = item.Key.NgaySua;
                    quyHD.NguoiSua = item.Key.NguoiSua;
                    quyHD.TrangThai = item.Key.TrangThai;
                    quyHD.LoaiHoaDon = item.Key.LoaiHoaDon;

                    var phuongthuc = "";
                    double tongTT = 0;
                    foreach (var itemGr in item)
                    {
                        tongTT += itemGr.qct.TienThu;
                        switch (itemGr.qct.HinhThucThanhToan)
                        {
                            case 1:
                                phuongthuc += "Tiền mặt, ";
                                break;
                            case 2:
                                phuongthuc += "POS, ";
                                break;
                            case 3:
                                phuongthuc += "Chuyển khoản, ";
                                break;
                            case 4:
                                phuongthuc += "Thẻ giá trị, ";
                                break;
                            case 5:
                                phuongthuc += "Điểm, ";
                                break;
                            case 6:
                                phuongthuc += "Thu từ cọc, ";
                                break;
                            default:
                                phuongthuc += "";
                                break;
                        }
                    }
                    phuongthuc = phuongthuc.Trim().TrimEnd(',');
                    quyHD.PhuongThucTT = phuongthuc;
                    quyHD.TongTienThu = tongTT;

                    lstReturn.Add(quyHD);
                }

                foreach (var item in tbl2)
                {
                    Quy_HoaDon quyHD = new Quy_HoaDon();
                    quyHD.ID = item.Key.ID;
                    quyHD.MaHoaDon = item.Key.MaHoaDon;
                    quyHD.NgayLapHoaDon = item.Key.NgayLapHoaDon;
                    quyHD.NguoiTao = item.Key.NguoiTao;
                    quyHD.NgaySua = item.Key.NgaySua;
                    quyHD.NguoiSua = item.Key.NguoiSua;
                    quyHD.TrangThai = item.Key.TrangThai;
                    quyHD.LoaiHoaDon = item.Key.LoaiHoaDon;

                    var phuongthuc = "";
                    double tongTT = 0;
                    foreach (var itemGr in item)
                    {
                        tongTT += itemGr.qct.TienThu;
                        switch (itemGr.qct.HinhThucThanhToan)
                        {
                            case 1:
                                phuongthuc += "Tiền mặt, ";
                                break;
                            case 2:
                                phuongthuc += "POS, ";
                                break;
                            case 3:
                                phuongthuc += "Chuyển khoản, ";
                                break;
                            case 4:
                                phuongthuc += "Thẻ giá trị, ";
                                break;
                            case 5:
                                phuongthuc += "Điểm, ";
                                break;
                            case 6:
                                phuongthuc += "Thu từ cọc, ";
                                break;
                            default:
                                phuongthuc += "";
                                break;
                        }
                    }
                    phuongthuc = phuongthuc.Trim().TrimEnd(',');
                    quyHD.PhuongThucTT = phuongthuc;
                    quyHD.TongTienThu = tongTT;

                    lstReturn.Add(quyHD);
                }
            }
            return lstReturn;
        }

        public List<Quy_HoaDon> GetLichSuThanhToan_ofDatHang(Guid id)
        {
            List<Quy_HoaDon> lstQHD = new List<Quy_HoaDon>();

            if (db == null)
            {
                return null;
            }
            else
            {
                // get Quy_HoaDon by  ID_DatHang
                var data1 = from qhd in db.Quy_HoaDon
                            join qct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qct.ID_HoaDon
                            join hd in db.BH_HoaDon on qct.ID_HoaDonLienQuan equals hd.ID
                            where hd.ID == id || hd.ID_HoaDon == id
                            group new { qct }
                            by new
                            {
                                ID = qhd.ID,
                                MaHoaDon = qhd.MaHoaDon,
                                NgayLapHoaDon = qhd.NgayLapHoaDon,
                                NguoiTao = qhd.NguoiTao,
                                TongTienThu = qhd.TongTienThu,
                                LoaiHoaDon = qhd.LoaiHoaDon,

                                // HD da huy
                                NgaySua = qhd.NgaySua,
                                NguoiSua = qhd.NguoiSua,
                                TrangThai = qhd.TrangThai,
                            };


                foreach (var item in data1)
                {
                    Quy_HoaDon quyHD = new Quy_HoaDon();
                    quyHD.ID = item.Key.ID;
                    quyHD.MaHoaDon = item.Key.MaHoaDon;
                    quyHD.NgayLapHoaDon = item.Key.NgayLapHoaDon;
                    quyHD.NguoiTao = item.Key.NguoiTao;
                    quyHD.TongTienThu = item.Key.LoaiHoaDon == 11 ? item.Key.TongTienThu : -item.Key.TongTienThu;
                    quyHD.NgaySua = item.Key.NgaySua;
                    quyHD.NguoiSua = item.Key.NguoiSua;
                    quyHD.TrangThai = item.Key.TrangThai;

                    var phuongthuc = "";
                    foreach (var itemGr in item)
                    {
                        switch (itemGr.qct.HinhThucThanhToan)
                        {
                            case 1:
                                phuongthuc += "Tiền mặt, ";
                                break;
                            case 2:
                                phuongthuc += "POS, ";
                                break;
                            case 3:
                                phuongthuc += "Chuyển khoản, ";
                                break;
                            case 4:
                                phuongthuc += "Thẻ giá trị, ";
                                break;
                            case 5:
                                phuongthuc += "Điểm, ";
                                break;
                            case 6:
                                phuongthuc += "Tiền cọc, ";
                                break;
                        }
                    }
                    phuongthuc = phuongthuc.Trim().TrimEnd(',');
                    quyHD.PhuongThucTT = phuongthuc;

                    lstQHD.Add(quyHD);
                }
            }
            return lstQHD;
        }

        public double GetKhachDaTra(Guid? id)
        {
            List<Quy_HoaDon> lstQHD = new List<Quy_HoaDon>();
            if (db == null)
            {
                return 0;
            }
            else
            {
                // get Quy_HoaDon by id (HD Dat Hang)
                var data = from qhd in db.Quy_HoaDon
                           join qct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qct.ID_HoaDon
                           join hd in db.BH_HoaDon on qct.ID_HoaDonLienQuan equals hd.ID
                           where hd.ID == id && hd.LoaiHoaDon == 3
                           select new
                           {
                               qhd.TongTienThu,
                           };
                if (data != null && data.Count() > 0)
                {
                    return data.FirstOrDefault().TongTienThu;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion


        #region insert
        public string Add_SoQuy(Quy_HoaDon objSoQuyAdd)
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
                    db.Quy_HoaDon.Add(objSoQuyAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message + ex.InnerException;
                }
            }
            return strErr;
        }


        public string Add_TaiKhoan(DM_TaiKhoanNganHang objTaiKhoan)
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
                    db.DM_TaiKhoanNganHang.Add(objTaiKhoan);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message + ex.InnerException;
                }
            }
            return strErr;
        }
        #endregion


        #region update
        public string Update_SoQuy(Quy_HoaDon obj, List<Quy_HoaDon_ChiTiet> listCT)
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
                    ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                    #region Quy_HoaDon
                    Quy_HoaDon objUpd = db.Quy_HoaDon.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.MaHoaDon = obj.MaHoaDon;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.NguoiNopTien = obj.NguoiNopTien;
                    objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
                    objUpd.TongTienThu = obj.TongTienThu;
                    objUpd.NoiDungThu = obj.NoiDungThu;
                    objUpd.LoaiHoaDon = obj.LoaiHoaDon;
                    objUpd.HachToanKinhDoanh = obj.HachToanKinhDoanh;
                    objUpd.PhieuDieuChinhCongNo = obj.PhieuDieuChinhCongNo;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.NguoiSua = obj.NguoiSua;
                    #endregion

                    var lstOld = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == obj.ID);
                    db.Quy_HoaDon_ChiTiet.RemoveRange(lstOld);
                    foreach (var item in listCT)
                    {
                        item.ID = Guid.NewGuid();
                        item.ID_HoaDon = obj.ID;
                        db.Quy_HoaDon_ChiTiet.Add(item);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_SoQuy: " + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }

        public string Update_TaiKHoan(DM_TaiKhoanNganHang obj)
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
                    DM_TaiKhoanNganHang objUpd = db.DM_TaiKhoanNganHang.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.ID_DonVi = obj.ID_DonVi;
                    objUpd.ID_NganHang = obj.ID_NganHang;
                    objUpd.TenChuThe = obj.TenChuThe;
                    objUpd.SoTaiKhoan = obj.SoTaiKhoan;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.TaiKhoanPOS = obj.TaiKhoanPOS;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_TaiKHoan" + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }
        #endregion

        public string Update_QuyHoaDon(Quy_HoaDon obj)
        {
            string err = string.Empty;
            Quy_HoaDon objUpd = db.Quy_HoaDon.Find(obj.ID);

            objUpd.NgayLapHoaDon = obj.NgayLapHoaDon;
            objUpd.NgayTao = obj.NgayTao;
            objUpd.ID_NhanVien = obj.ID_NhanVien;
            objUpd.NguoiTao = obj.NguoiTao;
            objUpd.NoiDungThu = obj.NoiDungThu;
            objUpd.TongTienThu = obj.TongTienThu;
            objUpd.TrangThai = obj.TrangThai;
            db.Entry(objUpd).State = EntityState.Modified;
            db.SaveChanges();

            UpdateSoDuThe_WhenChangeSoQuy(obj.ID, objUpd.NgayLapHoaDon);
            return err;
        }

        public string UpdateQuyKhoanThuChi(Quy_KhoanThuChi obj)
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
                    Quy_KhoanThuChi objUpd = db.Quy_KhoanThuChi.Find(obj.ID);
                    objUpd.NoiDungThuChi = obj.NoiDungThuChi;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.LaKhoanThu = obj.LaKhoanThu;
                    objUpd.TinhLuong = obj.TinhLuong;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = DateTime.Now;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("UpdateQuyKhoanThuChi: " + ex.InnerException + ex.Message);
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        /// <summary>
        /// Cập nhật số dư (sau phát sinh) của thẻ giá trị
        /// </summary>
        /// <param name="idQuyHoaDon"></param>
        /// <param name="ngaylapPT"></param>
        public void UpdateSoDuThe_WhenChangeSoQuy(Guid idQuyHoaDon, DateTime ngaylapPT)
        {
            try
            {
                //var lstIDKhachHang = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == idQuyHoaDon && x.HinhThucThanhToan == 4).Select(x => x.ID_DoiTuong).Distinct().ToList();
                //if (lstIDKhachHang.Count() > 0)
                //{
                //    ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                //    foreach (var item in lstIDKhachHang)
                //    {
                //        classHoaDonCT.UpdateTheGiaTri(item, ngaylapPT);
                //    }
                //}
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("UpdateSoDuThe_WhenChangeSoQuy " + ex);
            }
        }

        public string UpdateSoQuy_Basic(Guid idSoQuy, DateTime ngaylap, string noidung, Guid? ID_KhoanThuChi)
        {
            try
            {
                Quy_HoaDon qhd = db.Quy_HoaDon.Find(idSoQuy);
                if (qhd != null)
                {
                    qhd.NgayLapHoaDon = ngaylap;
                    qhd.NoiDungThu = noidung;
                    var qct = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == idSoQuy);
                    if (qct.Count() > 0)
                    {
                        qct.ToList().ForEach(x => x.ID_KhoanThuChi = ID_KhoanThuChi);
                    }
                    db.SaveChanges();

                    UpdateSoDuThe_WhenChangeSoQuy(idSoQuy, ngaylap);
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                return string.Concat("UpdateSoQuy_Basic ", e.InnerException, e.Message);
            }
        }

        #region delete
        public string Delete_SoQuy(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                Quy_HoaDon objDel = db.Quy_HoaDon.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.Quy_HoaDon_ChiTiet.RemoveRange(db.Quy_HoaDon_ChiTiet.Where(idHD => idHD.ID_HoaDon == id));

                        db.Quy_HoaDon.Remove(objDel);
                        //
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
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }

        public bool DeleteTaiKhoanNganHang(Guid Id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                DM_TaiKhoanNganHang objDel = db.DM_TaiKhoanNganHang.Find(Id);
                if (objDel != null)
                {
                    try
                    {
                        objDel.TrangThai = 0;
                        db.SaveChanges();
                        return true;
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
        }
        #endregion
        public string GetAutoCode(int? loaiHoaDon)
        {
            string format = "{0:0000000}";
            string mahoadon = string.Empty;
            if (loaiHoaDon.HasValue)
            {
                mahoadon = db.DM_LoaiChungTu.Where(p => p.ID == loaiHoaDon).Select(p => p.MaLoaiChungTu).FirstOrDefault();
            }
            else
            {
                mahoadon = "SQPC";
            }
            string madv = db.Quy_HoaDon.Where(p => p.MaHoaDon.Contains(mahoadon))
                .Where(p => p.MaHoaDon.Length == 6 || p.MaHoaDon.Length == 7 || p.MaHoaDon.Length == 8 || p.MaHoaDon.Length == 9).OrderByDescending(p => p.MaHoaDon).Select(p => p.MaHoaDon).FirstOrDefault();
            if (madv == null)
            {
                mahoadon = mahoadon + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(mahoadon.Length, 4)) + 1;
                mahoadon = mahoadon + string.Format(format, tempstt);
            }
            return mahoadon;
        }

        /// <summary>
        /// get infor HoaDon by ID SoQuy {MaHoaDon,TienThu, PhuongThucTT}
        /// </summary>
        /// <param name="idSoQuy"></param>
        /// <returns></returns>
        public List<Quy_HoaDon_ChiTietDTO> SP_GetAllHoaDon_byIDSoQuy(Guid idSoQuy)
        {
            try
            {
                SqlParameter param = new SqlParameter("ID_PhieuThuChi", idSoQuy);
                return db.Database.SqlQuery<Quy_HoaDon_ChiTietDTO>("EXEC SP_GetAllHoaDon_byIDPhieuThuChi @ID_PhieuThuChi", param).ToList();
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetAllHoaDon_byIDSoQuy " + e.InnerException + e.Message);
                return null;
            }
        }

        /// <summary>
        /// get infor Quy_HoaDon + all HoaDon of this SoQuy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Quy_HoaDonDTO SP_GetInforSoQuy_And_HoaDonLienQuan(Guid id)
        {
            try
            {
                SqlParameter param = new SqlParameter("ID_PhieuThuChi", id);
                var data = db.Database.SqlQuery<Quy_HoaDonDTO>("EXEC GetInforSoQuy_ByID @ID_PhieuThuChi", param).ToList();

                if (data != null && data.Count() > 0)
                {
                    var itemHD = data.FirstOrDefault();
                    // get all hoa don lien quan
                    itemHD.Quy_HoaDon_ChiTiet = SP_GetAllHoaDon_byIDSoQuy(id);
                    return itemHD;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("SP_GetInforSoQuy_And_HoaDonLienQuan " + e.InnerException + e.Message);
                return null;
            }
        }

        public string SP_GetAutoCode(int? loaiHoaDon)
        {
            string format = string.Empty;
            string mahoadon = string.Empty;
            if (loaiHoaDon.HasValue)
            {
                mahoadon = db.DM_LoaiChungTu.Where(p => p.ID == loaiHoaDon).Select(p => p.MaLoaiChungTu).FirstOrDefault();
            }
            else
            {
                mahoadon = "SQ";
            }
            var lenMaChungTu = mahoadon.Length;
            switch (lenMaChungTu)
            {
                case 2:
                    format = "{0:00000000}";
                    break;
                case 3:
                    format = "{0:0000000}";
                    break;
                case 4:
                    format = "{0:000000}";
                    break;
                case 5:
                    format = "{0:00000}";
                    break;
            }
            try
            {
                SqlParameter param = new SqlParameter("LoaiHoaDon", loaiHoaDon);
                var objReturn = db.Database.SqlQuery<SP_MaxCode>("EXEC SP_GetMaQuyHoaDon_Max @LoaiHoaDon", param).ToList();
                if (objReturn.Count() > 0)
                {
                    mahoadon = mahoadon + string.Format(format, objReturn.FirstOrDefault().MaxCode + 1);
                }
                else
                {
                    mahoadon = mahoadon + string.Format(format, 1);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetAutoCode_SoQuy " + ex.InnerException + ex.Message + ex.HResult);
                return mahoadon + string.Format(format, 1);
            }
            return mahoadon;
        }

        public string SP_GetMaPhieuThuChiMax_byTemp(int? loaiHoaDon, Guid? idDonVi, DateTime ngayLapHoaDon)
        {
            string mahoadon = string.Empty;
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("LoaiHoaDon", loaiHoaDon));
                lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));
                lstParam.Add(new SqlParameter("NgayLapHoaDon", ngayLapHoaDon));
                var objReturn = db.Database.SqlQuery<SP_MaxCodeTemp>("EXEC GetMaPhieuThuChiMax_byTemp @LoaiHoaDon, @ID_DonVi, @NgayLapHoaDon", lstParam.ToArray()).FirstOrDefault();
                mahoadon = objReturn.MaxCode;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassQuy_HoaDon.GetMaPhieuThuChiMax_byTemp: " + ex.InnerException + ex.Message + ex.HResult);
                return string.Empty;
            }
            return mahoadon;
        }

        /// <summary>
        /// use when update hoadon (multiple phieuthu/chi)
        /// </summary>
        /// <param name="maphieuthuchi"></param>
        /// <returns></returns>
        public string GetMaPhieuThuChi_whenUpdateHD(string maphieuthuchi)
        {
            string mahoadon = string.Empty;
            try
            {
                SqlParameter param = new SqlParameter("MaPhieuThuChiGoc", maphieuthuchi);
                var objReturn = db.Database.SqlQuery<SP_MaxCodeTemp>("EXEC GetMaPhieuThuChi_whenUpdateHD @MaPhieuThuChiGoc", param).FirstOrDefault();
                mahoadon = objReturn.MaxCode;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassQuy_HoaDon.GetMaPhieuThuChi_whenUpdateHD: " + ex.InnerException + ex.Message + ex.HResult);
                return string.Empty;
            }
            return mahoadon;
        }

        public bool HuyTienCoc_CheckVuotHanMuc(Guid id)
        {
            try
            {
                BH_HoaDon objUpd = db.BH_HoaDon.Find(id);
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("ID_PhieuThuChi", id));
                var obj = db.Database.SqlQuery<Model.SP_ReturnBool>("EXEC HuyTienCoc_CheckVuotHanMuc @ID_PhieuThuChi", lstParam.ToArray()).FirstOrDefault();
                return obj.Exist;
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("HuyTienCoc_Check: " + e.InnerException + e.Message);
                return true;
            }
        }
        //trinhpv
        #region báo cáo bán hàng
        public List<BC_BH_HoaDonDTO> getAllBaoCaoBanHang(Guid IDchinhanh)
        {
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into hv
                      from nn in hv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      where dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      orderby bhhd.NgayLapHoaDon, bhhd.MaHoaDon descending
                      select new
                      {
                          bhhd.MaHoaDon,
                          bhhd.NgayLapHoaDon,
                          hh.TenHangHoa,
                          bhhdct.SoLuong,
                          bhhdct.DonGia,
                          bhhdct.GiaVon,
                          nn.TenNhanVien,
                          dvqd.TenDonViTinh
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                bH_HoaDonDTO.TenHangHoa = item.TenHangHoa;
                bH_HoaDonDTO.SoLuong = item.SoLuong;
                bH_HoaDonDTO.GiaBan = item.DonGia;
                bH_HoaDonDTO.ThanhTien = item.SoLuong * item.DonGia;
                bH_HoaDonDTO.GiaVon = item.GiaVon;
                bH_HoaDonDTO.TienVon = item.SoLuong * item.GiaVon;
                bH_HoaDonDTO.LaiLo = bH_HoaDonDTO.ThanhTien - bH_HoaDonDTO.TienVon;
                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                bH_HoaDonDTO.TenDonViTinh = item.TenDonViTinh;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        public List<BC_BH_HoaDonDTO> getBaoCaoBanHangToDay(DateTime Today, Guid IDchinhanh)
        {
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon into dt
                      from bb in dt.DefaultIfEmpty()
                      join dvqd in db.DonViQuiDois on bb.ID_DonViQuiDoi equals dvqd.ID into bq
                      from vv in bq.DefaultIfEmpty()
                      join hh in db.DM_HangHoa on vv.ID_HangHoa equals hh.ID into hv
                      from rr in hv.DefaultIfEmpty()
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into rv
                      from nn in rv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID into nd
                      from dd in nd.DefaultIfEmpty()
                      where (bhhd.NgayLapHoaDon.Year == Today.Year & bhhd.NgayLapHoaDon.Month == Today.Month & bhhd.NgayLapHoaDon.Day == Today.Day) & dd.ID == IDchinhanh & bb.GiaVon != null & bhhd.ChoThanhToan == false

                      //where bhhd.NgayLapHoaDon == Today
                      orderby bhhd.NgayLapHoaDon, bhhd.MaHoaDon descending
                      select new
                      {
                          bhhd.MaHoaDon,
                          bhhd.NgayLapHoaDon,
                          rr.TenHangHoa,
                          bb.SoLuong,
                          bb.DonGia,
                          bb.GiaVon,
                          nn.TenNhanVien,
                          vv.TenDonViTinh
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                bH_HoaDonDTO.TenHangHoa = item.TenHangHoa;
                bH_HoaDonDTO.SoLuong = item.SoLuong;
                bH_HoaDonDTO.GiaBan = item.DonGia;
                bH_HoaDonDTO.ThanhTien = item.SoLuong * item.DonGia;
                bH_HoaDonDTO.GiaVon = item.GiaVon;
                bH_HoaDonDTO.TienVon = item.SoLuong * item.GiaVon;
                bH_HoaDonDTO.LaiLo = bH_HoaDonDTO.ThanhTien - bH_HoaDonDTO.TienVon;
                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                bH_HoaDonDTO.TenDonViTinh = item.TenDonViTinh;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        public List<BC_BH_HoaDonDTO> GetBaoCao_BanHang(DateTime TuNgayLapHD, DateTime DenNgaylapHD, Guid IDchinhanh)
        {
            // DateTime a = 
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vv
                      from nn in vv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      where (bhhd.NgayLapHoaDon >= TuNgayLapHD & bhhd.NgayLapHoaDon < DenNgaylapHD) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      orderby bhhd.NgayLapHoaDon, bhhd.MaHoaDon descending
                      select new
                      {
                          bhhd.MaHoaDon,
                          bhhd.NgayLapHoaDon,
                          hh.TenHangHoa,
                          bhhdct.SoLuong,
                          bhhdct.DonGia,
                          bhhdct.GiaVon,
                          nn.TenNhanVien,
                          dvqd.TenDonViTinh
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                bH_HoaDonDTO.TenHangHoa = item.TenHangHoa;
                bH_HoaDonDTO.SoLuong = item.SoLuong;
                bH_HoaDonDTO.GiaBan = item.DonGia;
                bH_HoaDonDTO.ThanhTien = item.SoLuong * item.DonGia;
                bH_HoaDonDTO.GiaVon = item.GiaVon;
                bH_HoaDonDTO.TienVon = item.SoLuong * item.GiaVon;
                bH_HoaDonDTO.LaiLo = bH_HoaDonDTO.ThanhTien - bH_HoaDonDTO.TienVon;
                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                bH_HoaDonDTO.TenDonViTinh = item.TenDonViTinh;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        public List<BC_BH_HoaDonDTO> GetMoneyBanHang(DateTime TuNgayLapHD, DateTime DenNgaylapHD, Guid IDchinhanh)
        {
            // DateTime a = 
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vv
                      from nn in vv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      where (bhhd.NgayLapHoaDon >= TuNgayLapHD & bhhd.NgayLapHoaDon < DenNgaylapHD) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      group bhhdct by new
                      {
                      } into g
                      select new
                      {
                          ThanhTien = g.Sum(x => (x.SoLuong * x.DonGia)),
                          TienVon = g.Sum(x => (x.SoLuong * x.GiaVon)),
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.ThanhTien = item.ThanhTien;
                bH_HoaDonDTO.TienVon = item.TienVon;
                bH_HoaDonDTO.LaiLo = item.ThanhTien - item.TienVon;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        // Mối quan tâm
        public List<BC_BH_HoaDonDTO> GetBaoCao_ThoiGian(DateTime TuNgayLapHD, DateTime DenNgaylapHD, Guid IDchinhanh)
        {
            // DateTime a = 
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vv
                      from nn in vv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      where (bhhd.NgayLapHoaDon >= TuNgayLapHD & bhhd.NgayLapHoaDon < DenNgaylapHD) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      orderby bhhd.NgayLapHoaDon, bhhd.MaHoaDon descending
                      select new
                      {
                          bhhd.MaHoaDon,
                          bhhd.NgayLapHoaDon,
                          hh.TenHangHoa,
                          bhhdct.SoLuong,
                          bhhdct.DonGia,
                          bhhdct.GiaVon,
                          nn.TenNhanVien,
                          dvqd.TenDonViTinh
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                bH_HoaDonDTO.TenHangHoa = item.TenHangHoa;
                bH_HoaDonDTO.SoLuong = item.SoLuong;
                bH_HoaDonDTO.GiaBan = item.DonGia;
                bH_HoaDonDTO.ThanhTien = item.SoLuong * item.DonGia;
                bH_HoaDonDTO.GiaVon = item.GiaVon;
                bH_HoaDonDTO.TienVon = item.SoLuong * item.GiaVon;
                bH_HoaDonDTO.LaiLo = bH_HoaDonDTO.ThanhTien - bH_HoaDonDTO.TienVon;
                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                bH_HoaDonDTO.TenDonViTinh = item.TenDonViTinh;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        public List<BC_BH_HoaDonDTO> GetBaoCao_LoiNhan(DateTime TuNgayLapHD, DateTime DenNgaylapHD, Guid IDchinhanh)
        {
            // DateTime a = 
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vv
                      from nn in vv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      where (bhhd.NgayLapHoaDon >= TuNgayLapHD & bhhd.NgayLapHoaDon < DenNgaylapHD) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      orderby ((bhhdct.SoLuong * bhhdct.DonGia) - (bhhdct.SoLuong * bhhdct.GiaVon)), bhhd.MaHoaDon descending
                      select new
                      {
                          bhhd.MaHoaDon,
                          bhhd.NgayLapHoaDon,
                          hh.TenHangHoa,
                          bhhdct.SoLuong,
                          bhhdct.DonGia,
                          bhhdct.GiaVon,
                          nn.TenNhanVien,
                          dvqd.TenDonViTinh
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                bH_HoaDonDTO.TenHangHoa = item.TenHangHoa;
                bH_HoaDonDTO.SoLuong = item.SoLuong;
                bH_HoaDonDTO.GiaBan = item.DonGia;
                bH_HoaDonDTO.ThanhTien = item.SoLuong * item.DonGia;
                bH_HoaDonDTO.GiaVon = item.GiaVon;
                bH_HoaDonDTO.TienVon = item.SoLuong * item.GiaVon;
                bH_HoaDonDTO.LaiLo = bH_HoaDonDTO.ThanhTien - bH_HoaDonDTO.TienVon;
                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                bH_HoaDonDTO.TenDonViTinh = item.TenDonViTinh;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        public List<BC_BH_HoaDonDTO> GetBaoCao_GiamGia(DateTime TuNgayLapHD, DateTime DenNgaylapHD, Guid IDchinhanh)
        {
            // DateTime a = 
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vv
                      from nn in vv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      where (bhhd.NgayLapHoaDon >= TuNgayLapHD & bhhd.NgayLapHoaDon < DenNgaylapHD) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      orderby bhhd.TongGiamGia, bhhd.MaHoaDon descending
                      select new
                      {
                          bhhd.MaHoaDon,
                          bhhd.NgayLapHoaDon,
                          hh.TenHangHoa,
                          bhhdct.SoLuong,
                          bhhdct.DonGia,
                          bhhdct.GiaVon,
                          nn.TenNhanVien,
                          dvqd.TenDonViTinh
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                bH_HoaDonDTO.TenHangHoa = item.TenHangHoa;
                bH_HoaDonDTO.SoLuong = item.SoLuong;
                bH_HoaDonDTO.GiaBan = item.DonGia;
                bH_HoaDonDTO.ThanhTien = item.SoLuong * item.DonGia;
                bH_HoaDonDTO.GiaVon = item.GiaVon;
                bH_HoaDonDTO.TienVon = item.SoLuong * item.GiaVon;
                bH_HoaDonDTO.LaiLo = bH_HoaDonDTO.ThanhTien - bH_HoaDonDTO.TienVon;
                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                bH_HoaDonDTO.TenDonViTinh = item.TenDonViTinh;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        public List<BC_BH_HoaDonDTO> GetBaoCao_TraHang(DateTime TuNgayLapHD, DateTime DenNgaylapHD, Guid IDchinhanh)
        {
            // DateTime a = 
            var tbl = from bhhd in db.BH_HoaDon
                      join bhhdct in db.BH_HoaDon_ChiTiet on bhhd.ID equals bhhdct.ID_HoaDon
                      join dvqd in db.DonViQuiDois on bhhdct.ID_DonViQuiDoi equals dvqd.ID
                      join hh in db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                      join nv in db.NS_NhanVien on bhhd.ID_NhanVien equals nv.ID into vv
                      from nn in vv.DefaultIfEmpty()
                      join dv in db.DM_DonVi on bhhd.ID_DonVi equals dv.ID
                      where (bhhd.NgayLapHoaDon >= TuNgayLapHD & bhhd.NgayLapHoaDon < DenNgaylapHD) & dv.ID == IDchinhanh & bhhd.ChoThanhToan == false
                      orderby bhhd.LoaiHoaDon, bhhd.MaHoaDon descending
                      select new
                      {
                          bhhd.MaHoaDon,
                          bhhd.NgayLapHoaDon,
                          hh.TenHangHoa,
                          bhhdct.SoLuong,
                          bhhdct.DonGia,
                          bhhdct.GiaVon,
                          nn.TenNhanVien,
                          dvqd.TenDonViTinh
                      };
            List<BC_BH_HoaDonDTO> lst = new List<BC_BH_HoaDonDTO>();
            foreach (var item in tbl)
            {
                BC_BH_HoaDonDTO bH_HoaDonDTO = new BC_BH_HoaDonDTO();
                bH_HoaDonDTO.MaHoaDon = item.MaHoaDon;
                bH_HoaDonDTO.NgayLapHoaDon = item.NgayLapHoaDon;
                bH_HoaDonDTO.TenHangHoa = item.TenHangHoa;
                bH_HoaDonDTO.SoLuong = item.SoLuong;
                bH_HoaDonDTO.GiaBan = item.DonGia;
                bH_HoaDonDTO.ThanhTien = item.SoLuong * item.DonGia;
                bH_HoaDonDTO.GiaVon = item.GiaVon;
                bH_HoaDonDTO.TienVon = item.SoLuong * item.GiaVon;
                bH_HoaDonDTO.LaiLo = bH_HoaDonDTO.ThanhTien - bH_HoaDonDTO.TienVon;
                bH_HoaDonDTO.TenNhanVien = item.TenNhanVien;
                bH_HoaDonDTO.TenDonViTinh = item.TenDonViTinh;
                lst.Add(bH_HoaDonDTO);
            }
            if (lst.Count > 0)
                return lst;
            else
                return null;
        }
        #endregion
    }

    public class DM_MauInDTO
    {
        public Guid ID { get; set; }
        public int ID_LoaiChungTu { get; set; }
        public Guid ID_DonVi { get; set; }
        public string TenMauIn { get; set; }
        public string KhoGiay { get; set; }
        public string DuLieuMauIn { get; set; }
        public bool LaMacDinh { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }


    public class Quy_HoaDonDTO
    {
        public Guid ID { get; set; }
        public string MaHoaDon { get; set; }
        public string MaNguoiNop { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public string NhomNguoiNop { get; set; }
        public bool? HachToanKinhDoanh { get; set; }
        public string NguoiNopTien_KhongDau { get; set; }
        public string NguoiNopTien_ChuCaiDau { get; set; }
        public string NguoiNopTien { get; set; }
        public string SoDienThoai { get; set; }
        public double TongTienThu { get; set; }
        public string NoiDungThu { get; set; }
        public bool? TrangThai { get; set; }
        public string TenNhanVien { get; set; } // NhanVien tao phieu Thu/Chi
        public virtual List<Quy_HoaDon_ChiTietDTO> Quy_HoaDon_ChiTiet { get; set; }
        public int? PhieuDieuChinhCongNo { get; set; }// check when delete soquy
        public int? PhieuDieuChinhDiem { get; set; }
    }

    public class DM_NganHangDTO
    {
        public Guid ID { get; set; }
        public string MaNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string ChiNhanh { get; set; }
    }

    public class DM_TaiKhoanNganHangDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DonVi { get; set; }
        public Guid ID_NganHang { get; set; }
        public string TenChuThe { get; set; }
        public string TenNganHang { get; set; }
        public string SoTaiKhoan { get; set; }
        public bool TaiKhoanPOS { get; set; }
        public string GhiChu { get; set; }
    }

    public class Quy_HoaDon_NhanVienPRC
    {
        public String MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string NguoiNopTien { get; set; }
        public string LoaiPhieu { get; set; }
        public double TongTienThu { get; set; }
    }
    public class BC_BH_HoaDonDTO
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string TenHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double GiaBan { get; set; }
        public double ThanhTien { get; set; }
        public double? GiaVon { get; set; }
        public double? TienVon { get; set; }
        public double? LaiLo { get; set; }
        public string TenNhanVien { get; set; }
        public bool? LaHangHoa { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public string TenDonViTinh { get; set; }
    }
    public class BC_BH_HoaDonPRC
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double GiaBan { get; set; }
        public double TienChietKhau { get; set; }
        public double ThanhTien { get; set; }
        public double? GiaVon { get; set; }
        public double? TienVon { get; set; }
        public double GiamGiaHD { get; set; }
        public double? LaiLo { get; set; }
        public string TenNhanVien { get; set; }
        public Guid? ID_NhomHang { get; set; }
    }
    public class BieuDo_BanHangPRC
    {
        public DateTime NgayLapHoaDon { get; set; }
        public double ThanhTien { get; set; }
        public string TenDonVi { get; set; }
        public Guid? ID_NhomHang { get; set; }
    }
    public class ListPages
    {
        public int SoTrang { get; set; }
    }
}