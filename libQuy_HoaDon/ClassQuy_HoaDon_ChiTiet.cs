using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;

namespace libQuy_HoaDon
{
    public class ClassQuy_HoaDon_ChiTiet
    {
        private SsoftvnContext db;

        public ClassQuy_HoaDon_ChiTiet(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public Quy_HoaDon_ChiTiet Select_Quy_HoaDon(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Quy_HoaDon_ChiTiet.Find(id);
            }
        }

        public IQueryable<Quy_HoaDon_ChiTiet> Gets(Expression<Func<Quy_HoaDon_ChiTiet, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.Quy_HoaDon_ChiTiet;
                else
                    return db.Quy_HoaDon_ChiTiet.Where(query);
            }
        }

        public Quy_HoaDon_ChiTiet Get(Expression<Func<Quy_HoaDon_ChiTiet, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Quy_HoaDon_ChiTiet.Where(query).FirstOrDefault();
            }
        }

        public IQueryable<Quy_HoaDon_ChiTietDTO> GetQuyHoaDon_ByIDHoaDon(Guid idHDLienQuan)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                var data = from qhd in db.Quy_HoaDon
                           join ctQHD in db.Quy_HoaDon_ChiTiet on qhd.ID equals ctQHD.ID_HoaDon
                           where ctQHD.ID_HoaDonLienQuan == idHDLienQuan
                           select new Quy_HoaDon_ChiTietDTO
                           {
                               TongTienThu = qhd.TongTienThu
                           };
                return data;
            }
        }

        public bool Quy_HoaDon_ChiTietExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {

                return db.Quy_HoaDon_ChiTiet.Count(e => e.ID == id) > 0;
            }
        }

        public Double SelectDaChiByID_HoaDonLienQuan(Guid ID_HoaDonLienQuan, DateTime ngayhoadon)
        {
            double DaChi = 0;
            if (db != null)
            {
                try
                {
                    DaChi = db.Quy_HoaDon_ChiTiet.Where(p => p.ID_HoaDonLienQuan == ID_HoaDonLienQuan).Select(p => new
                    {
                        ID_HoaDon = p.ID_HoaDon,
                        TongTienChi = p.TienGui + p.TienMat
                    }).GroupJoin(db.Quy_HoaDon, ct => ct.ID_HoaDon, hd => hd.ID, (ct, hd) => new { ct, hd })
                    .SelectMany(s => s.hd.DefaultIfEmpty(), (s, hd) => new
                    {
                        TongTienChi = s.ct.TongTienChi,
                        NgayLapHoaDon = hd.NgayLapHoaDon
                    }).Where(p => p.NgayLapHoaDon < ngayhoadon).Sum(p => p.TongTienChi);
                }
                catch
                { }
                return DaChi;
            }
            else
            {
                return DaChi;
            }
        }

        #endregion

        #region insert
        public string Add_ChiTietQuyHoaDon(Quy_HoaDon_ChiTiet objAdd)
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
                    db.Quy_HoaDon_ChiTiet.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception dbEx)
                {
                    CookieStore.WriteLog("AddChiTiet_QuyHoaDon: " + dbEx.InnerException + dbEx.Message);
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public string Update_ChiTietQuyHoaDon(Quy_HoaDon_ChiTiet obj)
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
                    #region Quy_HoaDon_ChiTiet
                    Quy_HoaDon_ChiTiet objUpd = db.Quy_HoaDon_ChiTiet.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TienMat = obj.TienMat;
                    objUpd.TienGui = obj.TienGui;
                    objUpd.TienThu = obj.TienThu;
                    //objUpd.ID_DoiTuong = obj.ID_DoiTuong;
                    //objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.ID_KhoanThuChi = obj.ID_KhoanThuChi;
                    //objUpd.BH_HoaDon_ChiTiet1 = obj.BH_HoaDon_ChiTiet1;
                    //objUpd.BH_HoaDon_ChiTiet2 = obj.BH_HoaDon_ChiTiet2;
                    //objUpd.DM_DonVi = obj.DM_DonVi;
                    //objUpd.DM_HangHoa = obj.DM_HangHoa;
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
        string CheckDelete_QuyHoaDon(SsoftvnContext db, Quy_HoaDon_ChiTiet obj)
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

        public string DeleteQuyHoaDon_ofHoaDonLienQuan(Guid idHoaDonLienQuan)
        {
            string strErr = string.Empty;
            try
            {
                if (db == null)
                {
                    return "Kết nối CSDL không hợp lệ";
                }
                else
                {
                    var lstQuyCT = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_HoaDonLienQuan == idHoaDonLienQuan);
                    if (lstQuyCT != null)
                    {
                        var lstIDQuy = lstQuyCT.Select(x => x.ID_HoaDon).Distinct();
                        var lstQuy = db.Quy_HoaDon.Where(x => lstIDQuy.Contains(x.ID));
                        db.Quy_HoaDon_ChiTiet.RemoveRange(lstQuyCT);
                        db.Quy_HoaDon.RemoveRange(lstQuy);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                strErr = string.Concat(e.InnerException, " ", e.Message);
            }
            return strErr;
        }

        public string UpdateIDKhachHang_inSoQuy(Guid idHoaDonLienQuan, int? loaiDoiTuong = 1)
        {
            string strErr = string.Empty;
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    List<SqlParameter> lstPr = new List<SqlParameter>();
                    lstPr.Add(new SqlParameter("ID_HoaDonLienQuan", idHoaDonLienQuan));
                    lstPr.Add(new SqlParameter("LoaiDoiTuong", loaiDoiTuong));
                    db.Database.ExecuteSqlCommand(" exec UpdateIDKhachHang_inSoQuy @ID_HoaDonLienQuan, @LoaiDoiTuong ", lstPr.ToArray());
                }
                catch (Exception e)
                {
                    strErr = String.Concat("UpdateIDKhachHang_inSoQuy ", e.InnerException, e.Message);
                    CookieStore.WriteLog(strErr);
                }
            }
            return strErr;
        }

        #endregion
    }
    public class Quy_HoaDon_ChiTietDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_HoaDonLienQuan { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_KhoanThuChi { get; set; }
        public Guid? ID_TaiKhoanNganHang { get; set; }
        public Guid? ID_BangLuongChiTiet { get; set; }
        public Guid? ID_DoiTuong { get; set; }
        public Guid? ID_NhanVienCT { get; set; }
        public double TienMat { get; set; }
        public bool? TrangThai { get; set; }
        public bool? HachToanKinhDoanh { get; set; }
        public bool? LaKhoanThu { get; set; }
        public double TienGui { get; set; }
        public double TienThu { get; set; }
        public double TongTienThu { get; set; }
        public string PhuongThuc { get; set; }
        public double DaChi { get; set; }
        public string NguoiNopTien { get; set; }
        public string NoiDungThuChi { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
        public string SoDienThoai { get; set; }
        public string NoiDungThu { get; set; }
        public string NguoiSua { get; set; }
        public string NguoiTao { get; set; }
        public string MaHoaDon { get; set; }
        public string MaHoaDonHD { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public int? LoaiDoiTuong { get; set; }
        public string TenChiNhanh { get; set; } // add to bind print PhieuThu/Chi
        public string DienThoaiChiNhanh { get; set; }
        public string DiaChiChiNhanh { get; set; }
        public string DiaChiKhachHang { get; set; }
        public double? DiemThanhToan { get; set; }
        public string TenChuThe { get; set; }
        public bool? TaiKhoanPOS { get; set; }
        public string TenTaiKhoanPOS { get; set; }
        public string TenTaiKhoanNOTPOS { get; set; }
        public string MaDoiTuong { get; set; }
        public int LaTienCoc { get; set; }
        public int? HinhThucThanhToan { get; set; }
        public double? TongThanhToanHD { get; set; }
        public int? LoaiHoaDonHD { get; set; }
        public double? TongTienThue { get; set; }
        public double? DaThuTruoc { get; set; }
        public DateTime? NgayLapPhieuThu { get; set; }
    }

    public class KangJin_QuyChiTietDTO: Quy_HoaDon_ChiTietDTO
    {
        public string TenNganHang { get; set; }
        public double? ChiPhiThanhToan { get; set; }// chi phí cà thẻ POS
        public bool? MacDinh { get; set; }// hiện tại không dùng làm gì cả
        public bool? TheoPhanTram { get; set; }
        public bool? ThuPhiThanhToan { get; set; }
    }
    public class SP_GetListCashFlow
    {
        public Guid? ID { get; set; }
        public int? LoaiHoaDon { get; set; }
        public int? LoaiDoiTuong { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public bool? TrangThai { get; set; }
        public double TongTienThu { get; set; }
        public string NoiDungThu { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_KhoanThuChi { get; set; }
        public string ID_TaiKhoanNganHang { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenNhanVien { get; set; }
        public string NguoiTao { get; set; }// tkhoan tao
        public string NguoiNopTien { get; set; }
        public string TenDoiTuong_KhongDau { get; set; }
        public string DiaChiKhachHang { get; set; }//diachikhachhang
        public string NoiDungThuChi { get; set; }
        public double TienMat { get; set; }
        public double TienGui { get; set; }
        public double TienThu { get; set; }
        public string PhuongThuc { get; set; }// mat, chuyenkhoan
        public string SoDienThoai { get; set; }
        public string TenChiNhanh { get; set; }
        public string DienThoaiChiNhanh { get; set; }
        public string DiaChiChiNhanh { get; set; }
        public string TenTaiKhoanNOTPOS { get; set; }
        public string TenTaiKhoanPOS { get; set; }
        public bool? HachToanKinhDoanh { get; set; }
        public int? PhieuDieuChinhCongNo { get; set; }// 1.phieu dc congno, 2.naptiencoc, 3.chitracoc
        public string TenNguonKhach { get; set; }
        public double? TongThuMat { get; set; }
        public double? TongChiMat { get; set; }
        public double? TongThuCK { get; set; }
        public double? TongChiCK { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public double? TienPOS { get; set; }
        public double? ChuyenKhoan { get; set; }  
        public double? TienDoiDiem { get; set; }
        public double? TTBangTienCoc { get; set; }
        public double? TienTheGiaTri { get; set; }
    }
    public class Excel_SoQuy
    {
        public string LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string NoiDungThuChi { get; set; }
        public double TongTienThu { get; set; }
        public string PhuongThuc { get; set; }
        public string TaiKhoanChuyen { get; set; }
        public string TaiKhoanPos { get; set; }
        public string MaDoiTuong { get; set; }
        public string NguoiNopTien { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string TenNguonKhach { get; set; }
        public string NoiDungThu { get; set; }
        public string TrangThai { get; set; }
        public string TenNhanVien { get; set; }
        public string TenDonVi { get; set; }
    }


    public class Quy_SoQuyTienMat_Excel
    {
        public string LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string NoiDungThuChi { get; set; }
        public double TongTienThu { get; set; }
        public string MaDoiTuong { get; set; }
        public string NguoiNopTien { get; set; }
        public string SoDienThoai { get; set; }
        public string NoiDungThu { get; set; }
        public string TrangThai { get; set; }
        public string TenNhanVien { get; set; }
        public string TenDonVi { get; set; }
    }

    public class SoQuyNganHang_Excel
    {
        public string LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string NoiDungThuChi { get; set; }
        public double TongTienThu { get; set; }
        public string TenTaiKhoanNOTPOS { get; set; }
        public string TenTaiKhoanPOS { get; set; }
        public string MaDoiTuong { get; set; }
        public string NguoiNopTien { get; set; }
        public string SoDienThoai { get; set; }
        public string NoiDungThu { get; set; }
        public string TrangThai { get; set; }
        public string TenNhanVien { get; set; }
        public string TenDonVi { get; set; }
    }

    public class Quy_SoQuyTQ_Excel
    {
        public string LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string NoiDungThuChi { get; set; }
        public double TongTienThu { get; set; }
        public string HinhThuc { get; set; }
        public string TenTaiKhoanNOTPOS { get; set; }
        public string TenTaiKhoanPOS { get; set; }
        public string MaDoiTuong { get; set; }
        public string NguoiNopTien { get; set; }
        public string SoDienThoai { get; set; }
        public string NoiDungThu { get; set; }
        public string TrangThai { get; set; }
        public string TenNhanVien { get; set; }
        public string TenDonVi { get; set; }
    }
}

public class ParamCashFlow
{
    public int LoaiSoQuy { get; set; }// mat, chuyenkhoan, all
    public int LoaiChungTu { get; set; } // 1.thu, 2.chi, 3.thu + chi
    public string LoaiNapTien { get; set; } // 1.datcoc, 2.the, empty: all
    public string TxtSearch { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public List<string> IDDonVis { get; set; }
    public string ID_NhanVien { get; set; }
    public Guid? ID_NhanVienLogin { get; set; }
    public string ID_KhoanThuChi { get; set; }
    public string ID_TaiKhoanNganHang { get; set; }
    public int TrangThaiHachToan { get; set; } // 0.co, 1.khong, 2.all hach toan
    public int TrangThaiSoQuy { get; set; } // 1.da thanh toan, 2.huy, 3.all
    public string ColumSort { get; set; }
    public string SortBy { get; set; } // 1.ASC, 2.DESC
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public string ColumnHides { get; set; }
    public string TextTime { get; set; }
    public string TextChiNhanhs { get; set; }
    public double? TonDauKy { get; set; } = 0;
}
