using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using libQuy_HoaDon;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using libDM_DoiTuong;
using System.Data;
using System.Web;
using libNS_NhanVien;
using System.Data.SqlClient;
using System.Data.Entity;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class Quy_HoaDonAPIController : BaseApiController
    {
        #region GET

        [ResponseType(typeof(DM_TaiKhoanNganHang))]
        public IHttpActionResult GetDM_TaiKhoanNganHang(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                DM_TaiKhoanNganHang tknh = _classQHD.selectedDMTKNH(id);
                DM_TaiKhoanNganHangDTO temp = new DM_TaiKhoanNganHangDTO();
                temp.ID = tknh.ID;
                temp.ID_DonVi = tknh.ID_DonVi;
                temp.ID_NganHang = tknh.ID_NganHang;
                temp.TenChuThe = tknh.TenChuThe;
                temp.TenNganHang = _classQHD.GetNganHang(p => p.ID == tknh.ID_NganHang).TenNganHang;
                temp.SoTaiKhoan = tknh.SoTaiKhoan;
                temp.GhiChu = tknh.GhiChu;
                temp.TaiKhoanPOS = tknh.TaiKhoanPOS;
                if (tknh == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(temp);
                }
            }
        }

        [HttpGet]
        public Quy_HoaDonDTO Get_InforSoQuy_HoaDonLienQuan_ByID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.SP_GetInforSoQuy_And_HoaDonLienQuan(id);
            }
        }

        // GET: api/DanhMuc/BH_HoaDonAPI/GetBH_HoaDon
        public List<Quy_HoaDon> GetQuy_HoaDon()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.Gets(null);
            }
        }
        public List<Quy_HoaDon_ChiTietDTO> GetallSoQuy(Guid iddonvi, int loai, string maHoaDon, int trangThai, string dayStart, string dayEnd, string idnhanvien, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string idTKNganHang, int kinhdoanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<Quy_HoaDon_ChiTietDTO> lst = null;
                if (loai == 0)
                {
                    lst = _classQHD.GetListHoaDons_QuyHD_Group(iddonvi, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, kinhdoanh).Where(l => l.TienGui > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                }
                else
                {
                    if (loai == 2)
                    {
                        lst = _classQHD.GetListHoaDons_QuyHD_Group(iddonvi, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, kinhdoanh).Where(l => (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                    else
                    {
                        lst = _classQHD.GetListHoaDons_QuyHD_Group(iddonvi, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, kinhdoanh).Where(l => l.TienMat > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                }

                return lst;
            }
        }

        //tính tồn đầu kỳ
        public List<Quy_HoaDon_ChiTietDTO> GetallSoQuyTonDau(Guid iddonvi, int loai, string maHoaDon, int trangThai, string dayDauky, string idnhanvien, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string idTKNganHang, int kinhdoanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<Quy_HoaDon_ChiTietDTO> lst = null;
                if (loai == 0)
                {
                    lst = _classQHD.GetListHoaDons_QuyHD_GroupDauKy(iddonvi, maHoaDon, trangThai, dayDauky, idnhanvien, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, kinhdoanh).Where(l => l.TienGui > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                }
                else
                {
                    if (loai == 2)
                    {
                        lst = _classQHD.GetListHoaDons_QuyHD_GroupDauKy(iddonvi, maHoaDon, trangThai, dayDauky, idnhanvien, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, kinhdoanh).Where(l => (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                    else
                    {
                        lst = _classQHD.GetListHoaDons_QuyHD_GroupDauKy(iddonvi, maHoaDon, trangThai, dayDauky, idnhanvien, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, kinhdoanh).Where(l => l.TienMat > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                }

                return lst;
            }
        }
        //Trinhpv xuất excel sổ quỹ
        [HttpPost]
        public string ExportExcel_SoQuy(ParamCashFlow lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon classQuyHoaDon = new classQuy_HoaDon(db);
                Class_officeDocument classOffice = new Class_officeDocument(db);
                List<SP_GetListCashFlow> data = classQuyHoaDon.GetListCashFlow_Paging(lstParam);
                if (data.Count > 0)
                {
                    double tongthu = 0, tongchi = 0, tonquy = 0;

                    List<Excel_SoQuy> lstEx = new List<Excel_SoQuy>();
                    foreach (var item in data)
                    {
                        Excel_SoQuy DM = new Excel_SoQuy();
                        DM.LoaiHoaDon = item.LoaiHoaDon == 11 ? "Phiếu thu" : "Phiếu chi";
                        DM.MaHoaDon = item.MaHoaDon;
                        DM.NgayLapHoaDon = item.NgayLapHoaDon;
                        DM.NoiDungThuChi = item.NoiDungThuChi;
                        switch (lstParam.LoaiSoQuy)
                        {
                            case 0: // chuyenkhoan
                                DM.TongTienThu = (item.LoaiHoaDon == 11) ? item.TienGui : item.TienGui * (-1);
                                break;
                            case 1: // mat
                                DM.TongTienThu = (item.LoaiHoaDon == 11) ? item.TienMat : item.TienMat * (-1);
                                break;
                            default://all
                                DM.TongTienThu = (item.LoaiHoaDon == 11) ? item.TienThu : item.TienThu * (-1);
                                break;
                        }
                        DM.PhuongThuc = item.PhuongThuc;
                        DM.TaiKhoanChuyen = item.TenTaiKhoanNOTPOS;
                        DM.TaiKhoanPos = item.TenTaiKhoanPOS;
                        DM.MaDoiTuong = item.MaDoiTuong;
                        DM.NguoiNopTien = item.NguoiNopTien;
                        DM.SoDienThoai = item.SoDienThoai;
                        DM.DiaChiKhachHang = item.DiaChiKhachHang;
                        DM.TenNguonKhach = item.TenNguonKhach;
                        DM.NoiDungThu = item.NoiDungThu;
                        DM.TrangThai = item.TrangThai == false ? "Đã hủy" : "Đã thanh toán";
                        DM.TenNhanVien = item.TenNhanVien;
                        DM.TenDonVi = item.TenChiNhanh;
                        lstEx.Add(DM);
                    }
                    DataTable excel = classOffice.ToDataTable<Excel_SoQuy>(lstEx.OrderByDescending(p => p.NgayLapHoaDon).ToList());

                    string teamSave, teamdown;
                    switch (lstParam.LoaiSoQuy)
                    {
                        case 0: // chuyenkhoan
                            teamSave = "Teamplate_SoQuyNganHang.xlsx";
                            teamdown = "SoQuyNganHang.xlsx";
                            excel.Columns.Remove("PhuongThuc");
                            tongthu = data[0].TongThuCK ?? 0;
                            tongchi = data[0].TongChiCK ?? 0;
                            break;
                        case 1: // mat
                            teamSave = "Teamplate_SoQuyTienMat.xlsx";
                            teamdown = "SoQuyTienMat.xlsx";
                            excel.Columns.Remove("PhuongThuc");
                            excel.Columns.Remove("TaiKhoanChuyen");
                            excel.Columns.Remove("TaiKhoanPOS");
                            tongthu = data[0].TongThuMat ?? 0;
                            tongchi = data[0].TongChiMat ?? 0;
                            break;
                        default://all
                            teamSave = "Teamplate_SoQuyTongQuy.xlsx";
                            teamdown = "SoQuyTongQuy.xlsx";
                            tongthu = (data[0].TongThuMat + data[0].TongThuCK) ?? 0;
                            tongchi = (data[0].TongChiMat + data[0].TongChiCK) ?? 0;
                            break;
                    }
                    tonquy = tongthu - tongchi;
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + teamSave);
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + teamdown);
                    fileSave = classOffice.createFolder_Download(fileSave);
                    classOffice.listToOfficeExcel_StypeSQ(fileTeamplate, fileSave, excel, 10, 34, 24, true,
                        lstParam.ColumnHides, lstParam.TextTime, lstParam.TextChiNhanhs,
                        tongthu, tongchi, tonquy, lstParam.TonDauKy);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return fileSave;
                }
                return string.Empty;
            }
        }
        [HttpGet]
        public void ExportExcel_SoQuyTienMat(int loai, int loaiHoaDon, string maHoaDon,
            int trangThai, string dayStart, string dayEnd, string idnhanvien, string ColumnsHide, string teamSave, string teamdown, Guid iddonvi, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string time, string TenChiNhanh, string idTKNganHang, double tongthu, double tongchi, double tonquy, int kinhdoanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                Class_officeDocument classOffice = new Class_officeDocument(db);
                string columsort = null;
                string sort = null;
                List<Quy_HoaDon_ChiTietDTO> lst = null;
                if (loai == 0)
                {
                    lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienGui > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                }
                else
                {
                    if (loai == 2)
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                    else
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienMat > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                }
                List<Quy_SoQuyTienMat_Excel> lstEx = new List<Quy_SoQuyTienMat_Excel>();
                foreach (var item in lst)
                {
                    Quy_SoQuyTienMat_Excel DM = new Quy_SoQuyTienMat_Excel();
                    DM.LoaiHoaDon = item.LoaiHoaDon == 11 ? "Phiếu thu" : "Phiếu chi";
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.NoiDungThuChi = item.NoiDungThuChi;
                    DM.TongTienThu = (item.LoaiHoaDon == 11) ? item.TienThu : item.TienThu * (-1);
                    DM.MaDoiTuong = item.MaDoiTuong;
                    DM.NguoiNopTien = item.NguoiNopTien;
                    DM.SoDienThoai = item.SoDienThoai;
                    DM.NoiDungThu = item.NoiDungThu;
                    DM.TrangThai = item.TrangThai == false ? "Đã hủy" : "Đã thanh toán";
                    DM.TenNhanVien = item.TenNhanVien;
                    DM.TenDonVi = item.TenChiNhanh;
                    lstEx.Add(DM);
                }
                DataTable excel = classOffice.ToDataTable<Quy_SoQuyTienMat_Excel>(lstEx.OrderByDescending(p => p.NgayLapHoaDon).ToList());
                string fileTeamplate = HttpContext.Current.Server.MapPath(teamSave);
                string fileSave = HttpContext.Current.Server.MapPath(teamdown);
                fileSave = classOffice.createFolder_Download(fileSave);
                classOffice.listToOfficeExcel_StypeSQ(fileTeamplate, fileSave, excel, 7, 28, 24, true, ColumnsHide, time, TenChiNhanh, tongthu, tongchi, tonquy);
                HttpResponse Response = HttpContext.Current.Response;
                classOffice.downloadFile(fileSave);
            }
        }

        [HttpGet]
        public void ExportExcel_NganHang(int loai, int loaiHoaDon, string maHoaDon,
         int trangThai, string dayStart, string dayEnd, string idnhanvien, string ColumnsHide, string teamSave, string teamdown, Guid iddonvi, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string time, string TenChiNhanh, string idTKNganHang, double tongthu, double tongchi, double tonquy, int kinhdoanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                Class_officeDocument classOffice = new Class_officeDocument(db);
                string columsort = null;
                string sort = null;
                List<Quy_HoaDon_ChiTietDTO> lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienGui > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList(); ;
                List<SoQuyNganHang_Excel> lstEx = new List<SoQuyNganHang_Excel>();
                foreach (var item in lst)
                {
                    SoQuyNganHang_Excel DM = new SoQuyNganHang_Excel();
                    DM.LoaiHoaDon = item.LoaiHoaDon == 11 ? "Phiếu thu" : "Phiếu chi";
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.NoiDungThuChi = item.NoiDungThuChi;
                    DM.TongTienThu = (item.LoaiHoaDon == 11) ? item.TienThu : item.TienThu * (-1);
                    DM.TenTaiKhoanPOS = item.TenTaiKhoanPOS;
                    DM.TenTaiKhoanNOTPOS = item.TenTaiKhoanNOTPOS;
                    DM.MaDoiTuong = item.MaDoiTuong;
                    DM.NguoiNopTien = item.NguoiNopTien;
                    DM.SoDienThoai = item.SoDienThoai;
                    DM.NoiDungThu = item.NoiDungThu;
                    DM.TrangThai = item.TrangThai == false ? "Đã hủy" : "Đã thanh toán";
                    DM.TenNhanVien = item.TenNhanVien;
                    DM.TenDonVi = item.TenChiNhanh;
                    lstEx.Add(DM);
                }
                DataTable excel = classOffice.ToDataTable<SoQuyNganHang_Excel>(lstEx.OrderByDescending(p => p.NgayLapHoaDon).ToList());
                string fileTeamplate = HttpContext.Current.Server.MapPath(teamSave);
                string fileSave = HttpContext.Current.Server.MapPath(teamdown);
                fileSave = classOffice.createFolder_Download(fileSave);
                classOffice.listToOfficeExcel_StypeSQ(fileTeamplate, fileSave, excel, 7, 28, 24, true, ColumnsHide, time, TenChiNhanh, tongthu, tongchi, tonquy);
                HttpResponse Response = HttpContext.Current.Response;
                classOffice.downloadFile(fileSave);
            }
        }

        [HttpGet]
        public void ExportExcel_SoQuyTongQuat(int loai, int loaiHoaDon, string maHoaDon,
           int trangThai, string dayStart, string dayEnd, string idnhanvien, string ColumnsHide, string teamSave, string teamdown, Guid iddonvi, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string time, string TenChiNhanh, string idTKNganHang, double tongthu, double tongchi, double tonquy, int kinhdoanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                Class_officeDocument classOffice = new Class_officeDocument(db);
                string columsort = null;
                string sort = null;
                List<Quy_HoaDon_ChiTietDTO> lst = null;
                if (loai == 0)
                {
                    lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienGui > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                }
                else
                {
                    if (loai == 2)
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                    else
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienMat > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                }
                List<Quy_SoQuyTQ_Excel> lstEx = new List<Quy_SoQuyTQ_Excel>();
                foreach (var item in lst)
                {
                    Quy_SoQuyTQ_Excel DM = new Quy_SoQuyTQ_Excel();
                    DM.LoaiHoaDon = item.LoaiHoaDon == 11 ? "Phiếu thu" : "Phiếu chi";
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.NoiDungThuChi = item.NoiDungThuChi;
                    DM.TongTienThu = (item.LoaiHoaDon == 11) ? item.TienThu : item.TienThu * (-1);
                    DM.HinhThuc = (item.TienMat != 0 & item.TienGui == 0) ? "Tiền mặt" : (item.TienMat == 0 ? "Ngân hàng" : "Tiền mặt, Ngân hàng");
                    DM.TenTaiKhoanNOTPOS = item.TenTaiKhoanNOTPOS;
                    DM.TenTaiKhoanPOS = item.TenTaiKhoanPOS;
                    DM.MaDoiTuong = item.MaDoiTuong;
                    DM.NguoiNopTien = item.NguoiNopTien;
                    DM.SoDienThoai = item.SoDienThoai;
                    DM.NoiDungThu = item.NoiDungThu;
                    DM.TrangThai = item.TrangThai == false ? "Đã hủy" : "Đã thanh toán";
                    DM.TenNhanVien = item.TenNhanVien;
                    DM.TenDonVi = item.TenChiNhanh;
                    lstEx.Add(DM);
                }
                DataTable excel = classOffice.ToDataTable<Quy_SoQuyTQ_Excel>(lstEx.OrderByDescending(p => p.NgayLapHoaDon).ToList());
                string fileTeamplate = HttpContext.Current.Server.MapPath(teamSave);
                string fileSave = HttpContext.Current.Server.MapPath(teamdown);
                fileSave = classOffice.createFolder_Download(fileSave);
                classOffice.listToOfficeExcel_StypeSQ(fileTeamplate, fileSave, excel, 7, 28, 24, true, ColumnsHide, time, TenChiNhanh, tongthu, tongchi, tonquy);
                HttpResponse Response = HttpContext.Current.Response;
                classOffice.downloadFile(fileSave);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public List<Quy_HoaDon_ChiTietDTO> GetAllQuyHoaDon(int loai, int currentPage, int pageSize, int loaiHoaDon, string maHoaDon,
            int trangThai, string dayStart, string dayEnd, string idnhanvien, Guid iddonvi, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string idTKNganHang, string columsort, string sort, int kinhdoanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<Quy_HoaDon_ChiTietDTO> lst = null;
                if (loai == 0)
                {
                    lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienGui > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).Skip(currentPage * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    if (loai == 2)
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.DiemThanhToan == 0 || l.DiemThanhToan == null).Skip(currentPage * pageSize).Take(pageSize).ToList();
                    }
                    else
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienMat > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).Skip(currentPage * pageSize).Take(pageSize).ToList();
                    }
                }

                return lst;
            }
        }

        public PageListDTO GetPageCountHoaDon_Where(float pageSize, int loaiHoaDon, string maHoaDon, int loai,
             int trangThai, string dayStart, string dayEnd, string idnhanvien, Guid iddonvi, string ghichu, int locthanhtoan, string loaithuchi, string arrChiNhanh, string idTKNganHang, int kinhdoanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                var totalRecords = 0;
                string columsort = null;
                string sort = null;
                List<Quy_HoaDon_ChiTietDTO> lst = null;
                if (loai == 0)
                {
                    lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienGui > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                }
                else
                {
                    if (loai == 2)
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.DiemThanhToan == 0 || l.DiemThanhToan == null).ToList();
                    }
                    else
                    {
                        lst = _classQHD.GetAllQuyHoaDon(loaiHoaDon, loai, maHoaDon, trangThai, dayStart, dayEnd, idnhanvien, iddonvi, ghichu, locthanhtoan, loaithuchi, arrChiNhanh, idTKNganHang, columsort, sort, kinhdoanh).Where(l => l.TienMat > 0 && (l.DiemThanhToan == 0 || l.DiemThanhToan == null)).ToList();
                    }
                }
                if (lst != null)
                {
                    totalRecords = lst.Count();
                }

                PageListDTO pageListDTO = new PageListDTO
                {
                    TotalRecord = totalRecords,
                    PageCount = System.Math.Ceiling(totalRecords / pageSize) // round 6.1 --> 7
                };
                return pageListDTO;
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetListCashFlow_Paging2(ParamCashFlow lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classQuy_HoaDon classQuyHoaDon = new classQuy_HoaDon(db);
                    var data = classQuyHoaDon.GetListCashFlow_Paging(lstParam);
                    double? tongthuMat = 0, tongchiMat = 0, tongthuCK = 0, tongchiCK = 0, totalPage = 0;
                    int? totalRow = 0;
                    if (data.Count > 0)
                    {
                        tongthuMat = data[0].TongThuMat;
                        tongchiMat = data[0].TongChiMat;
                        tongthuCK = data[0].TongThuCK;
                        tongchiCK = data[0].TongChiCK;
                        totalRow = data[0].TotalRow;
                        totalPage = data[0].TotalPage;
                    }

                    return Json(new
                    {
                        res = true,
                        TotalRow = totalRow,
                        TotalPage = totalPage,
                        TongThuMat = tongthuMat,
                        TongChiMat = tongchiMat,
                        TongThuCK = tongthuCK,
                        TongChiCK = tongchiCK,
                        TongThuAll = tongthuMat + tongthuCK,
                        TongChiAll = tongchiMat + tongchiCK,
                        data
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        res = false,
                        mes = ex
                    });
                }
            }
        }

        [HttpGet, HttpPost]
        // nots use
        public IHttpActionResult GetListCashFlow_Paging(ParamCashFlow lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string idKhoanThuChi = "%%";
                string idNhanVien = "%%";
                string idTaiKhoanNganHang = "%%";
                string txtSearch = lstParam.TxtSearch;
                string searchSql = "%%";
                string loaiChungTu = string.Empty;
                string hachtoan = string.Empty;
                string trangthai = string.Empty;
                string idDonVis = string.Join(",", lstParam.IDDonVis);
                string loaisoquy = string.Empty;

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

                if (txtSearch == null || txtSearch == string.Empty)
                {
                    searchSql = "%%";
                }
                else
                {
                    searchSql = string.Concat("%", txtSearch, "%");
                }

                try
                {
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
                    lstPr.Add(new SqlParameter("TxtSearch", searchSql));
                    var data = db.Database.SqlQuery<SP_GetListCashFlow>("exec GetListCashFlow_Paging @IDDonVis, @ID_NhanVien, @ID_NhanVienLogin, @ID_TaiKhoanNganHang," +
                        "@ID_KhoanThuChi, @DateFrom, @DateTo, @LoaiSoQuy, @LoaiChungTu, @TrangThaiSoQuy, @TrangThaiHachToan, @TxtSearch, @CurrentPage, @PageSize", lstPr.ToArray()).ToList();
                    db.Database.CommandTimeout = 3000;

                    var thu = data.Where(x => x.LoaiHoaDon == 11);
                    var chi = data.Where(x => x.LoaiHoaDon == 12);
                    var tongthuMat = thu.Sum(x => x.TienMat);
                    var tongchiMat = chi.Sum(x => x.TienMat);
                    var tongthuCK = thu.Sum(x => x.TienGui);
                    var tongchiCK = chi.Sum(x => x.TienGui);
                    var tongthuAll = thu.Sum(x => x.TienThu);
                    var tongchiAll = chi.Sum(x => x.TienThu);

                    var totalRow = data.Count();
                    var totalPage = Math.Ceiling(totalRow * 1.0 / lstParam.PageSize);

                    return Json(new
                    {
                        res = true,
                        TotalRow = totalRow,
                        TotalPage = totalPage,
                        TongThuMat = tongthuMat,
                        TongChiMat = tongchiMat,
                        TongThuCK = tongthuCK,
                        TongChiCK = tongchiCK,
                        TongThuAll = tongthuAll,
                        TongChiAll = tongchiAll,
                        data = data.Skip(lstParam.CurrentPage * lstParam.PageSize).Take(lstParam.PageSize)
                    });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = "GetListCashFlow_Paging " + e });
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetListCashFlow_Before(ParamCashFlow lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string searchSql = "%%";
                string txtSearch = lstParam.TxtSearch;
                string loaiChungTu = string.Empty;
                string hachtoan = string.Empty;
                string trangthai = string.Empty;
                string idDonVis = string.Join(",", lstParam.IDDonVis);
                string loaisoquy = string.Empty;

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

                if (txtSearch == null || txtSearch == string.Empty)
                {
                    searchSql = "%%";
                }
                else
                {
                    searchSql = string.Concat("%", txtSearch, "%");
                }

                try
                {
                    List<SqlParameter> lstPr = new List<SqlParameter>();
                    lstPr.Add(new SqlParameter("IDDonVis", idDonVis));
                    lstPr.Add(new SqlParameter("ID_NhanVien", lstParam.ID_NhanVien));
                    lstPr.Add(new SqlParameter("ID_TaiKhoanNganHang", lstParam.ID_TaiKhoanNganHang));
                    lstPr.Add(new SqlParameter("ID_KhoanThuChi", lstParam.ID_KhoanThuChi));
                    lstPr.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
                    lstPr.Add(new SqlParameter("DateTo", lstParam.DateTo));
                    lstPr.Add(new SqlParameter("LoaiSoQuy", loaisoquy));
                    lstPr.Add(new SqlParameter("LoaiChungTu", loaiChungTu));
                    lstPr.Add(new SqlParameter("TrangThaiSoQuy", trangthai));
                    lstPr.Add(new SqlParameter("TrangThaiHachToan", hachtoan));
                    lstPr.Add(new SqlParameter("TxtSearch", searchSql));
                    var dataTonDauKy = db.Database.SqlQuery<SP_GetListCashFlow>("exec GetListCashFlow_Before @IDDonVis, @ID_NhanVien, @ID_TaiKhoanNganHang," +
                       "@ID_KhoanThuChi, @DateFrom, @DateTo, @LoaiSoQuy, @LoaiChungTu, @TrangThaiSoQuy, @TrangThaiHachToan, @TxtSearch", lstPr.ToArray()).ToList();
                    if (dataTonDauKy.Count > 0)
                    {
                        return Json(new
                        {
                            TonDauKyMat = dataTonDauKy[0].TongThuMat,
                            TonDauKyNH = dataTonDauKy[0].TongThuCK,
                            TonDauKyAll = dataTonDauKy[0].TongThuMat + dataTonDauKy[0].TongThuCK,
                            res = true,
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            TonDauKyMat = 0,
                            TonDauKyNH = 0,
                            TonDauKyAll = 0,
                            res = true,
                        });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = "GetListCashFlow_Before " + e });
                }
            }
        }

        [HttpGet]
        public bool CheckExistMaNganHang(Guid? id = null, string bankCode = "")
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    IQueryable<DM_NganHang> lst = null;
                    if (id != null && id != Guid.Empty)
                    {
                        lst = db.DM_NganHang.Where(x => x.MaNganHang.ToUpper() == bankCode && x.ID != id);
                    }
                    else
                    {
                        lst = db.DM_NganHang.Where(x => x.MaNganHang.ToUpper() == bankCode);
                    }
                    if (lst != null && lst.Count() > 0)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult PostDM_NganHang(DM_NganHang obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    obj.ID = Guid.NewGuid();
                    obj.NgayTao = DateTime.Now;
                    db.DM_NganHang.Add(obj);
                    db.SaveChanges();
                    return ActionTrueData(obj);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpPost, HttpPut]
        public IHttpActionResult PutDM_NganHang(DM_NganHang obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    DM_NganHang objUpdate = db.DM_NganHang.Find(obj.ID);
                    objUpdate.MaNganHang = obj.MaNganHang;
                    objUpdate.TenNganHang = obj.TenNganHang;
                    objUpdate.ChiNhanh = obj.ChiNhanh;
                    objUpdate.ChiPhiThanhToan = obj.ChiPhiThanhToan;
                    objUpdate.TheoPhanTram = obj.TheoPhanTram;
                    objUpdate.MacDinh = obj.MacDinh;
                    objUpdate.ThuPhiThanhToan = obj.ThuPhiThanhToan;
                    objUpdate.GhiChu = obj.GhiChu;
                    objUpdate.NguoiSua = obj.NguoiSua;
                    objUpdate.NgaySua = DateTime.Now;
                    db.Entry(objUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return ActionTrueData(obj);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet, HttpDelete]
        public IHttpActionResult DeleteNganHang(Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    string sErr = string.Empty;
                    var donvi = db.DM_DonVi.Where(x => x.ID_NganHang == id);
                    if (donvi != null && donvi.Count() > 0)
                    {
                        sErr = "Danh mục chi nhánh, ";
                    }
                    var khachhang = db.DM_DoiTuong.Where(x => x.ID_NganHang == id);
                    if (khachhang != null && khachhang.Count() > 0)
                    {
                        sErr += "Danh mục khách hàng, nhà cung cấp, ";
                    }
                    var quyct = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_NganHang == id);
                    if (quyct != null && quyct.Count() > 0)
                    {
                        sErr += "Danh mục sổ quỹ, ";
                    }
                    var hethong = db.HT_CongTy.Where(x => x.ID_NganHang == id);
                    if (hethong != null && hethong.Count() > 0)
                    {
                        sErr += "Danh mục công ty ";
                    }
                    var taikhoan = db.DM_TaiKhoanNganHang.Where(x => x.ID_NganHang == id);
                    if (taikhoan != null && taikhoan.Count() > 0)
                    {
                        sErr += "Danh mục tài khoản ngân hàng";
                    }
                    if (string.IsNullOrEmpty(sErr))
                    {
                        var obj = db.DM_NganHang.Find(id);
                        if (obj != null)
                        {
                            db.DM_NganHang.Remove(obj);
                            db.SaveChanges();
                        }
                        return ActionTrueNotData(sErr);
                    }
                    else
                    {
                        return ActionFalseNotData(sErr);
                    }
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        public List<DM_NganHangDTO> GetAllNganHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.GetAllNganhang();
            }
        }

        public List<DM_TaiKhoanNganHangDTO> GetAllTaiKhoanNganHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.GetAllTaiKhoanNganHang();
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetAllTaiKhoanNganHang_ByDonVi(Guid? idDonVi = null)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (db != null)
                    {
                        var tbl = (from tk in db.DM_TaiKhoanNganHang
                                   join nh in db.DM_NganHang on tk.ID_NganHang equals nh.ID
                                   // dung chung TK ngan hang
                                   // where nh.ID_DonVi == idDonVi
                                   select new
                                   {
                                       tk.ID,
                                       tk.ID_DonVi,
                                       tk.ID_NganHang,
                                       tk.TenChuThe,
                                       tk.SoTaiKhoan,
                                       tk.GhiChu,
                                       tk.TaiKhoanPOS,
                                       nh.TenNganHang,
                                       TrangThai = tk.TrangThai == null ? 1 : tk.TrangThai.Value,
                                       ChiPhiThanhToan = nh.ChiPhiThanhToan == null ? 0 : nh.ChiPhiThanhToan,
                                       TheoPhanTram = nh.TheoPhanTram == null ? true : nh.TheoPhanTram,
                                       nh.MacDinh,
                                       nh.ThuPhiThanhToan,
                                       TextSearchAuto = string.Concat(tk.TenChuThe, " ", tk.SoTaiKhoan, " ", nh.TenNganHang, " ", nh.MaNganHang)
                                   }).Where(p => p.TrangThai == 1).ToList();
                        return Json(new { res = true, data = tbl });
                    }
                    else
                    {
                        return Json(new { res = false, mes = "GetAllTaiKhoanNganHang_ByDonVi: DB null" });
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = string.Concat("GetAllTaiKhoanNganHang_ByDonVi", e.Message, e.InnerException) });
            }
        }

        public List<Quy_HoaDonDTO> GetLichSuThanhToanSoQuy(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<Quy_HoaDonDTO> lst = _classQHD.GetLichSuThanhToanSoQuy(idHoaDon);
                return lst;
            }
        }

        public List<BH_HoaDonDTO> getNgayLapHoaDonByIDLienQuan(Guid idhoadonlq)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BH_HoaDonDTO> lst = _classQHD.getNgayLapHDByIDLQ(idhoadonlq);
                return lst;
            }
        }

        public List<DM_MauInDTO> GetListMauInByLoaiHoaDonSQ(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.getlistMauInSQ(iddonvi);
            }
        }

        public List<Quy_HoaDonDTO> GetListQuyHoaDons(Guid? id_nhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<Quy_HoaDon> lstAllHDs = _classQHD.Gets(null);
                if (id_nhanVien == null)
                {
                    lstAllHDs = _classQHD.Gets(null);
                }
                else
                {
                    lstAllHDs = _classQHD.Gets(id => id.ID_NhanVien == id_nhanVien);
                }

                if (lstAllHDs != null && lstAllHDs.Count() > 0)
                {
                    List<Quy_HoaDonDTO> lsrReturns = new List<Quy_HoaDonDTO>();
                    foreach (Quy_HoaDon item in lstAllHDs)
                    {
                        Quy_HoaDonDTO itemData = new Quy_HoaDonDTO
                        {
                            ID = item.ID,
                            ID_NhanVien = item.ID_NhanVien,
                            MaHoaDon = item.MaHoaDon,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            LoaiHoaDon = item.LoaiHoaDon,
                            NguoiNopTien = item.NguoiNopTien,
                            TongTienThu = item.TongTienThu,
                            NoiDungThu = item.NoiDungThu,
                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                else
                    return null;
            }
        }

        [HttpGet]
        public List<jqAutoResultSQDT> GetListDoiTuongByLoai(int loaiDoiTuong, string txtSearch, Guid? idDonVi = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<Quy_HoaDonDTO> listTon = new List<Quy_HoaDonDTO>();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (txtSearch != null & txtSearch != "" & txtSearch != "null")
                {
                    var txtSearch1 = txtSearch.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower();
                    var text = CommonStatic.ConvertToUnSign(txtSearch1.Trim());
                    string Search = "%" + text + "%";
                    paramlist.Add(new SqlParameter("LoaiDoiTuong", loaiDoiTuong));
                    paramlist.Add(new SqlParameter("Search", Search));
                    paramlist.Add(new SqlParameter("ID_DonVi", idDonVi ?? (object)DBNull.Value));
                    listTon = db.Database.SqlQuery<Quy_HoaDonDTO>("exec GetListDoiTuongByLoai @LoaiDoiTuong, @Search, @ID_DonVi", paramlist.ToArray()).ToList();
                }
                return listTon.Select(p => new jqAutoResultSQDT
                {
                    label = p.NguoiNopTien,
                    value = p.NguoiNopTien,
                    actual = p.ID,
                    data = p
                }).ToList();
            }
        }

        [HttpGet]
        public List<jqAutoResultSQDT> GetListNhanVienSoQuy(Guid iddonvi, string txtSearch)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<Quy_HoaDonDTO> listTon = new List<Quy_HoaDonDTO>();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (txtSearch != null & txtSearch != "" & txtSearch != "null")
                {
                    txtSearch = CommonStatic.ConvertToUnSign(txtSearch).ToLower();
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                    List<Quy_HoaDonDTO> lst = db.Database.SqlQuery<Quy_HoaDonDTO>("exec GetListNhanVienAddSoQuy @ID_ChiNhanh", paramlist.ToArray()).ToList();

                    foreach (var item in lst)
                    {
                        Quy_HoaDonDTO obj = new Quy_HoaDonDTO();
                        obj.ID = item.ID;
                        obj.MaNguoiNop = item.MaNguoiNop;
                        obj.NguoiNopTien = item.NguoiNopTien;
                        obj.SoDienThoai = item.SoDienThoai;
                        obj.NguoiNopTien_KhongDau = CommonStatic.ConvertToUnSign(item.NguoiNopTien).ToLower();
                        obj.NguoiNopTien_ChuCaiDau = CommonStatic.GetCharsStart(item.NguoiNopTien).ToLower();
                        listTon.Add(obj);
                    }
                    listTon = listTon.Where(x => x.MaNguoiNop.ToLower().Contains(@txtSearch) || x.NguoiNopTien_KhongDau.Contains(@txtSearch) || x.NguoiNopTien_ChuCaiDau.Contains(@txtSearch)).ToList();
                }
                return listTon.Select(p => new jqAutoResultSQDT
                {
                    label = p.NguoiNopTien,
                    value = p.NguoiNopTien,
                    actual = p.ID,
                    data = p
                }).ToList();
            }
        }

        public List<Quy_HoaDonDTO> GetListQuyHD_CTQuy(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                List<Quy_HoaDon> lstAllHDs = _classQHD.Gets(null);


                if (lstAllHDs != null && lstAllHDs.Count() > 0)
                {
                    List<Quy_HoaDonDTO> lsrReturns = new List<Quy_HoaDonDTO>();
                    foreach (Quy_HoaDon item in lstAllHDs)
                    {
                        Quy_HoaDonDTO itemData = new Quy_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            LoaiHoaDon = item.LoaiHoaDon,
                            NguoiNopTien = item.NguoiNopTien,
                            TongTienThu = item.TongTienThu,
                            NoiDungThu = item.NoiDungThu,

                            Quy_HoaDon_ChiTiet = _classQHDCT.Gets(ct => ct.ID_HoaDon == item.ID)
                            .Where(p => p.TienGui == 0).Select(x => new Quy_HoaDon_ChiTietDTO
                            {
                                ID = x.ID,
                                TienMat = x.TienMat,
                                TienGui = x.TienGui

                            }).ToList()
                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                else
                    return null;
            }
        }

        public List<NhatKyThanhToanDTO> GetQuyHoaDon_byIDHoaDon(Guid idHoaDon, Guid? idHoaDonParent = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.GetQuyHoaDon_ofHoaDon(idHoaDon, idHoaDonParent);
            }
        }

        public List<Quy_HoaDon> GetQuyHoaDonFrom_ID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.Gets(idVT => idVT.ID == id);
            }
        }

        public List<Quy_HoaDon> GetLichSuThanhToan_ofDatHang(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.GetLichSuThanhToan_ofDatHang(id).OrderByDescending(x => x.NgayLapHoaDon).ToList();
            }
        }

        public IHttpActionResult GetQuyChiTiet_byIDQuy(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                    List<KangJin_QuyChiTietDTO> data = _classQHD.GetQuyChiTiet_byIDQuy(id);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        // GET: api/BH_HoaDonAPI/5
        [ResponseType(typeof(Quy_HoaDonDTO))]
        public IHttpActionResult GetQuy_HoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<Quy_HoaDon_ChiTietDTO> result = _classQHD.GetCT_QuyHoaDon(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
        }

        public bool Check_MaSoQuyExist(string maSoQuy)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                return _classQHD.Check_MaSoQuyExist(maSoQuy);
            }
        }

        [HttpGet]
        public bool HuyTienCoc_CheckVuotHanMuc(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classQuy_HoaDon classQuyHD = new classQuy_HoaDon(db);
                    return classQuyHD.HuyTienCoc_CheckVuotHanMuc(id);
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        #endregion

        #region update
        [HttpPost, HttpGet]
        public IHttpActionResult UpdatePhieuThuChi(Guid idSoQuy, DateTime ngaylap, string noidung, Guid? idKhoanThuChi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon classQuyHD = new classQuy_HoaDon(db);
                var str = classQuyHD.UpdateSoQuy_Basic(idSoQuy, ngaylap, noidung, idKhoanThuChi);
                if (str == string.Empty)
                {
                    return Json(new { res = true });
                }
                else
                {
                    return Json(new { res = false, mes = str });
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult PutQuy_HoaDon([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                        Quy_HoaDon Quy_HoaDon = data["objQuyHoaDon"].ToObject<Quy_HoaDon>();
                        List<Quy_HoaDon_ChiTiet> objCTQuyHoaDon = data["objCTQuyHoaDon"].ToObject<List<Quy_HoaDon_ChiTiet>>();

                        // update congnoluong: neu thaydoi khoanthuchi (tamungluong --> khongtamung or nguoclai)  
                        var tamungluong = false;
                        var lstCTOld = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == Quy_HoaDon.ID && x.ID_KhoanThuChi != null)
                            .Select(x => new { x.ID, x.ID_KhoanThuChi, x.TienThu });
                        var ctNew = objCTQuyHoaDon.Where(x => x.ID_HoaDon == Quy_HoaDon.ID && x.ID_KhoanThuChi != null)
                            .Select(x => new { x.ID, x.ID_KhoanThuChi, x.TienThu });
                        Guid? idKhoanChiOld = Guid.Empty, idKhoanChiNew = Guid.Empty;

                        double chiOld = 0, chiNew = 0;
                        if (lstCTOld != null && lstCTOld.Count() > 0)
                        {
                            idKhoanChiOld = lstCTOld.FirstOrDefault().ID_KhoanThuChi;
                            chiOld = lstCTOld.Sum(x => x.TienThu);
                        }
                        if (ctNew != null && ctNew.Count() > 0)
                        {
                            idKhoanChiNew = ctNew.FirstOrDefault().ID_KhoanThuChi;
                            chiNew = ctNew.Sum(x => x.TienThu);
                        }

                        string sMaHoaDon = string.Empty;
                        if (string.IsNullOrEmpty(Quy_HoaDon.MaHoaDon))
                        {
                            sMaHoaDon = _classQHD.SP_GetMaPhieuThuChiMax_byTemp(Quy_HoaDon.LoaiHoaDon, Quy_HoaDon.ID_DonVi, Quy_HoaDon.NgayLapHoaDon);
                        }
                        else
                        {
                            sMaHoaDon = Quy_HoaDon.MaHoaDon;
                        }
                        Quy_HoaDon.MaHoaDon = sMaHoaDon;
                        _classQHD.Update_SoQuy(Quy_HoaDon, objCTQuyHoaDon);

                        var tinhluong = db.Quy_KhoanThuChi.Where(x => (x.ID == idKhoanChiNew || x.ID == idKhoanChiOld) && x.TinhLuong == true).Select(x => x.TinhLuong);
                        if (tinhluong != null && tinhluong.Count() > 0)
                        {
                            var khoanNew = db.Quy_KhoanThuChi.Where(x => x.ID == idKhoanChiNew && x.TinhLuong == true);
                            ClassQuy_HoaDon_ChiTiet classQuyCT = new ClassQuy_HoaDon_ChiTiet(db);
                            var quyct = lstCTOld.Select(x => x.ID).ToList();
                            NhanSuService nhanSuService = new NhanSuService(db);
                            if (khoanNew != null && khoanNew.Count() > 0)
                            {
                                var khoanOld = db.Quy_KhoanThuChi.Where(x => x.ID == idKhoanChiOld && x.TinhLuong == true);
                                if (khoanOld.Count() == 0)
                                {
                                    // chi khac --> chi tamung 
                                    nhanSuService.UpdateCongNo_TamUngLuong(Quy_HoaDon.ID_DonVi, quyct, true);
                                }
                                else
                                {
                                    // tamungluong: thay doi tongtien
                                    var chenhlech = chiNew - chiOld;
                                    if (chenhlech != 0)
                                    {
                                        db.Database.ExecuteSqlCommand(@" UPDATE NS_CongNoTamUngLuong SET CongNo = CongNo + {0} where ID_NhanVien = {1} and ID_DonVi = {2}",
                                           chenhlech, objCTQuyHoaDon.FirstOrDefault().ID_NhanVien, Quy_HoaDon.ID_DonVi);
                                    }
                                }
                            }
                            else
                            {
                                // chi tamung --> chi khac
                                nhanSuService.HuyPhieuThu_UpdateCongNoTamUngLuong(Quy_HoaDon.ID_DonVi, quyct, true);
                            }
                        }

                        trans.Commit();
                        return ActionTrueData(Quy_HoaDon);
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(e.ToString());
                    }
                }
            }
        }

        #endregion

        #region delete
        [HttpDelete]
        [ResponseType(typeof(string))]
        public string DeleteQuy_HoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                string err = string.Empty; ;
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    Quy_HoaDon quyhd = _classQHD.Get(p => p.ID == id);
                    if (quyhd != null)
                    {
                        var qhdct = db.Quy_HoaDon_ChiTiet.Where(p => p.ID_HoaDon == id);
                        if (qhdct != null)
                        {
                            var quyctCus = qhdct.Where(x => x.ID_DoiTuong != null).FirstOrDefault();
                            if (quyctCus != null && quyctCus.ID_HoaDonLienQuan != null)
                            {
                                BH_HoaDon hd = db.BH_HoaDon.Find(quyctCus.ID_HoaDonLienQuan);
                                if (hd.LoaiHoaDon == 22)
                                {
                                    err = classHoaDon.CheckTheDaSuDung(quyctCus.ID_HoaDonLienQuan ?? Guid.Empty);
                                    if (err == string.Empty)
                                    {
                                        // không hủy phiếu thu TGT
                                        //if (hd.PhaiThanhToan == quyhd.TongTienThu)
                                        //{
                                        //    hd.ChoThanhToan = null;
                                        //    db.Entry(hd).State = EntityState.Modified;
                                        //    db.SaveChanges();
                                        //}
                                        quyhd.TrangThai = false;
                                        _classQHD.Update_QuyHoaDon(quyhd);
                                    }
                                }
                                else
                                {
                                    quyhd.TrangThai = false;
                                    _classQHD.Update_QuyHoaDon(quyhd);
                                }
                            }
                            else
                            {
                                quyhd.TrangThai = false;
                                _classQHD.Update_QuyHoaDon(quyhd);
                            }
                        }
                        else
                        {
                            quyhd.TrangThai = false;
                            _classQHD.Update_QuyHoaDon(quyhd);
                        }

                        var arrIDQuyCT = from quy in qhdct
                                         join ktc in db.Quy_KhoanThuChi on quy.ID_KhoanThuChi equals ktc.ID
                                         where ktc.TinhLuong == true
                                         select quy.ID;

                        NhanSuService nhansuService = new NhanSuService(db);
                        if (arrIDQuyCT != null && arrIDQuyCT.Count() > 0)
                        {
                            // huy phieuchi tamung
                            nhansuService.HuyPhieuThu_UpdateCongNoTamUngLuong(quyhd.ID_DonVi, arrIDQuyCT.ToList(), true);
                        }
                        else
                        {
                            // huy phieuchi thanhtoanluong
                            var trutamung = qhdct.Where(x => x.TruTamUngLuong > 0).Select(x => x.ID);
                            if (trutamung != null && trutamung.Count() > 0)
                            {
                                nhansuService.HuyPhieuThu_UpdateCongNoTamUngLuong(quyhd.ID_DonVi, trutamung.ToList(), false);
                            }
                        }
                        nhansuService.HuyPhieuThu_UpdateTrangThaiCong(qhdct.FirstOrDefault().ID);
                    }
                    else
                    {
                        err = "Lỗi";
                    }
                }
                return err;
            }
        }

        [HttpGet]
        public IHttpActionResult KhoiPhucQuy_HoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    Quy_HoaDon obj = db.Quy_HoaDon.Find(id);
                    if (obj != null)
                    {
                        obj.TrangThai = true;
                        db.SaveChanges();
                        return ActionTrueNotData(string.Empty);
                    }
                    else
                    {
                        return ActionFalseNotData(string.Empty);
                    }
                }
                catch (Exception)
                {
                    return ActionFalseNotData(string.Empty);
                }
            }
        }
        [AcceptVerbs("GET", "POST")]
        public string DeleteQuy_KhoanThuChi(Guid idloaithuchi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    Quy_KhoanThuChi ktc = db.Quy_KhoanThuChi.Find(idloaithuchi);
                    if (ktc != null)
                    {
                        ktc.TrangThai = 0;
                        db.SaveChanges();
                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult DeleteTaiKhoanNganHang([FromBody] JObject data)
        {
            try
            {
                bool check = false;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                    Guid ID = new Guid(data["ID"].ToObject<string>());
                    check = _classQHD.DeleteTaiKhoanNganHang(ID);

                }
                if (check)
                    return ActionTrueNotData("");
                else
                    return ActionFalseNotData("");
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }
        #endregion

        #region insert
        [HttpPost, ActionName("PostQuy_HoaDon")]
        public IHttpActionResult PostQuy_HoaDon([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Quy_HoaDon objHoaDon = data["objQuyHoaDon"].ToObject<Quy_HoaDon>();
                        Quy_HoaDon_ChiTiet objCTQuyHoaDon = data["objCTQuyHoaDon"].ToObject<Quy_HoaDon_ChiTiet>();
                        classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                        ClassQuy_KhoanThuChi _classQKTC = new ClassQuy_KhoanThuChi(db);
                        ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                        ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);

                        #region Quy_HoaDon
                        Quy_HoaDon itemQuy_HoaDon = new Quy_HoaDon();
                        itemQuy_HoaDon.ID = Guid.NewGuid();

                        string sMaHoaDon = string.Empty;
                        if (string.IsNullOrEmpty(objHoaDon.MaHoaDon))
                        {
                            sMaHoaDon = _classQHD.SP_GetMaPhieuThuChiMax_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }
                        else
                        {
                            sMaHoaDon = objHoaDon.MaHoaDon;
                        }
                        itemQuy_HoaDon.MaHoaDon = sMaHoaDon;
                        itemQuy_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien;
                        itemQuy_HoaDon.NguoiTao = _ClassNS_NhanVien.Get(p => p.ID == objHoaDon.ID_NhanVien).TenNhanVien;
                        itemQuy_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon;
                        itemQuy_HoaDon.NgayTao = DateTime.Now;
                        itemQuy_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                        itemQuy_HoaDon.NguoiNopTien = objHoaDon.NguoiNopTien;
                        itemQuy_HoaDon.TongTienThu = objHoaDon.TongTienThu;
                        itemQuy_HoaDon.NoiDungThu = objHoaDon.NoiDungThu;
                        itemQuy_HoaDon.TienMat = 0;
                        itemQuy_HoaDon.TienGui = 0;
                        itemQuy_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon;
                        itemQuy_HoaDon.TenChiNhanh = "";
                        itemQuy_HoaDon.HachToanKinhDoanh = objHoaDon.HachToanKinhDoanh;
                        itemQuy_HoaDon.PhieuDieuChinhCongNo = objHoaDon.PhieuDieuChinhCongNo;
                        itemQuy_HoaDon.TenNhanVien = _ClassNS_NhanVien.Get(p => p.ID == objHoaDon.ID_NhanVien).TenNhanVien;
                        itemQuy_HoaDon.NoiDungThuChi = objCTQuyHoaDon.ID_KhoanThuChi != null ? _classQKTC.SelectQuy_KhoanThuChi(objCTQuyHoaDon.ID_KhoanThuChi).NoiDungThuChi : "";
                        #endregion

                        string strIns = _classQHD.Add_SoQuy(itemQuy_HoaDon);
                        if (!string.IsNullOrEmpty(strIns))
                            return ActionFalseNotData(strIns);
                        else
                        {
                            #region Quy_HoaDonChiTiet
                            Quy_HoaDon_ChiTiet ctQuyHoaDon = new Quy_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_HoaDon = itemQuy_HoaDon.ID,
                                ID_HoaDonLienQuan = objCTQuyHoaDon.ID_HoaDonLienQuan,
                                TienMat = objCTQuyHoaDon.TienMat,
                                TienGui = objCTQuyHoaDon.TienGui,
                                HinhThucThanhToan = objCTQuyHoaDon.HinhThucThanhToan,
                                LoaiThanhToan = objCTQuyHoaDon.LoaiThanhToan,
                                TienThu = objCTQuyHoaDon.TienThu,
                                ID_DoiTuong = objCTQuyHoaDon.ID_DoiTuong,
                                ID_NhanVien = objCTQuyHoaDon.ID_NhanVien,
                                ID_KhoanThuChi = objCTQuyHoaDon.ID_KhoanThuChi,
                                ID_TaiKhoanNganHang = objCTQuyHoaDon.ID_TaiKhoanNganHang,
                                ChiPhiNganHang = objCTQuyHoaDon.ChiPhiNganHang,
                                LaPTChiPhiNganHang = objCTQuyHoaDon.LaPTChiPhiNganHang,
                                ID_NganHang = objCTQuyHoaDon.ID_TaiKhoanNganHang != null ? db.DM_TaiKhoanNganHang.Find(objCTQuyHoaDon.ID_TaiKhoanNganHang).ID_NganHang : (Guid?)null,
                            };
                            strIns = _classQHDCT.Add_ChiTietQuyHoaDon(ctQuyHoaDon);

                            if (objCTQuyHoaDon.ID_KhoanThuChi != null)
                            {
                                // update nợ tạm ưng lương of nhanvien 
                                NhanSuService nhanSuService = new NhanSuService(db);
                                nhanSuService.UpdateCongNo_TamUngLuong(itemQuy_HoaDon.ID_DonVi, new List<Guid>() { ctQuyHoaDon.ID }, true);
                            }
                            #endregion

                            Quy_HoaDonDTO objReturn = new Quy_HoaDonDTO
                            {
                                ID = itemQuy_HoaDon.ID,
                                MaHoaDon = itemQuy_HoaDon.MaHoaDon,
                                ID_NhanVien = itemQuy_HoaDon.ID_NhanVien,
                                NguoiTao = itemQuy_HoaDon.NguoiTao,
                                NgayTao = itemQuy_HoaDon.NgayTao,
                                ID_DonVi = itemQuy_HoaDon.ID_DonVi,
                                NguoiNopTien = itemQuy_HoaDon.NguoiNopTien,
                                TongTienThu = itemQuy_HoaDon.TongTienThu,
                                NoiDungThu = itemQuy_HoaDon.NoiDungThu,
                                LoaiHoaDon = itemQuy_HoaDon.LoaiHoaDon,
                                HachToanKinhDoanh = itemQuy_HoaDon.HachToanKinhDoanh,
                                TenNhanVien = itemQuy_HoaDon.TenNhanVien
                            };
                            trans.Commit();
                            return ActionTrueData(objReturn);
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }

        [HttpPost, ActionName("PostTaiKhoan_NganHang")]
        [ResponseType(typeof(DM_TaiKhoanNganHang))]
        public IHttpActionResult PostTaiKhoan_NganHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                DM_TaiKhoanNganHang objNganHang = data["objTaiKhoan"].ToObject<DM_TaiKhoanNganHang>();

                #region DM_TaiKhoanNganHang
                DM_TaiKhoanNganHang itemTaiKhoan = new DM_TaiKhoanNganHang();
                itemTaiKhoan.ID = Guid.NewGuid();
                itemTaiKhoan.ID_DonVi = objNganHang.ID_DonVi;
                itemTaiKhoan.ID_NganHang = objNganHang.ID_NganHang;
                itemTaiKhoan.TenChuThe = objNganHang.TenChuThe;
                itemTaiKhoan.SoTaiKhoan = objNganHang.SoTaiKhoan;
                itemTaiKhoan.GhiChu = objNganHang.GhiChu;
                itemTaiKhoan.TaiKhoanPOS = objNganHang.TaiKhoanPOS;
                #endregion

                string strIns = _classQHD.Add_TaiKhoan(itemTaiKhoan);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    DM_TaiKhoanNganHangDTO objReturn = new DM_TaiKhoanNganHangDTO
                    {
                        ID = itemTaiKhoan.ID,
                        ID_DonVi = itemTaiKhoan.ID_DonVi,
                        ID_NganHang = itemTaiKhoan.ID_NganHang,
                        TenChuThe = itemTaiKhoan.TenChuThe,
                        SoTaiKhoan = itemTaiKhoan.SoTaiKhoan,
                        GhiChu = itemTaiKhoan.GhiChu,
                        TaiKhoanPOS = itemTaiKhoan.TaiKhoanPOS
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, HttpPut]
        public IHttpActionResult Put_TaiKhoanNganHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                DM_TaiKhoanNganHang objTaiKhoan = data["objTaiKhoan"].ToObject<DM_TaiKhoanNganHang>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = _classQHD.Update_TaiKHoan(objTaiKhoan);

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    DM_TaiKhoanNganHangDTO objReturn = new DM_TaiKhoanNganHangDTO();
                    objReturn.ID = objTaiKhoan.ID;
                    objReturn.ID_DonVi = objTaiKhoan.ID_DonVi;
                    objReturn.ID_NganHang = objTaiKhoan.ID_NganHang;
                    objReturn.TenChuThe = objTaiKhoan.TenChuThe;
                    objReturn.SoTaiKhoan = objTaiKhoan.SoTaiKhoan;
                    objReturn.GhiChu = objTaiKhoan.GhiChu;
                    objReturn.TaiKhoanPOS = objTaiKhoan.TaiKhoanPOS;

                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, HttpPut]
        public IHttpActionResult PutQuy_KhoanThuChi(Quy_KhoanThuChi objQuy_KhoanThuChi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                string strUpd = _classQHD.UpdateQuyKhoanThuChi(objQuy_KhoanThuChi);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }

        }

        [HttpPost, HttpGet]
        public IHttpActionResult PostQuy_HoaDon_DieuChinh([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Quy_HoaDon objHoaDon = data["objQuyHoaDon"].ToObject<Quy_HoaDon>();
                        Quy_HoaDon_ChiTiet objCTQuyHoaDon = data["objCTQuyHoaDon"].ToObject<Quy_HoaDon_ChiTiet>();
                        classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);

                        var idQuyHD = objHoaDon.ID;
                        var sMaHoaDon = string.Empty;
                        if (!string.IsNullOrEmpty(objHoaDon.MaHoaDon))
                        {
                            if (idQuyHD != Guid.Empty)
                            {
                                var exists = _classQHD.Check_MaSoQuyExist(objHoaDon.MaHoaDon, idQuyHD);
                                if (exists)
                                {
                                    return ActionFalseNotData("Mã phiếu đặt cọc đã tồn tại");
                                }
                                sMaHoaDon = objHoaDon.MaHoaDon;
                            }
                            else
                            {
                                sMaHoaDon = _classQHD.GetMaPhieuThuChi_whenUpdateHD(objHoaDon.MaHoaDon);
                            }
                        }
                        else
                        {
                            sMaHoaDon = _classQHD.SP_GetMaPhieuThuChiMax_byTemp(15, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }

                        if (idQuyHD != Guid.Empty && idQuyHD != null)
                        {
                            var qctOld = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == objHoaDon.ID);
                            db.Quy_HoaDon_ChiTiet.RemoveRange(qctOld);

                            Quy_HoaDon qhd = db.Quy_HoaDon.Find(idQuyHD);
                            qhd.ID_DonVi = objHoaDon.ID_DonVi;
                            qhd.ID_NhanVien = objHoaDon.ID_NhanVien;
                            qhd.MaHoaDon = sMaHoaDon;
                            qhd.LoaiHoaDon = objHoaDon.LoaiHoaDon;
                            qhd.NgayLapHoaDon = objHoaDon.NgayLapHoaDon;
                            qhd.NoiDungThu = objHoaDon.NoiDungThu;
                            qhd.TongTienThu = objHoaDon.TongTienThu;
                            qhd.NguoiNopTien = objHoaDon.NguoiNopTien;
                            qhd.HachToanKinhDoanh = objHoaDon.HachToanKinhDoanh;
                            qhd.PhieuDieuChinhCongNo = objHoaDon.PhieuDieuChinhCongNo;// muontamtruong: phieudatcoc
                            qhd.NguoiSua = objHoaDon.NguoiSua;
                            qhd.NgaySua = DateTime.Now;
                        }
                        else
                        {
                            Quy_HoaDon objQuy = objHoaDon;
                            objQuy.ID = Guid.NewGuid();
                            objQuy.NgayTao = DateTime.Now;
                            objQuy.MaHoaDon = sMaHoaDon;
                            db.Quy_HoaDon.Add(objQuy);
                        }

                        var diemThanhToan = objCTQuyHoaDon.DiemThanhToan;
                        Quy_HoaDon_ChiTiet ctQuy = new Quy_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_HoaDon = objHoaDon.ID,
                            TienMat = objCTQuyHoaDon.TienMat,
                            TienGui = objCTQuyHoaDon.TienGui,
                            TienThu = objCTQuyHoaDon.TienThu,
                            ThuTuThe = objCTQuyHoaDon.ThuTuThe,
                            DiemThanhToan = diemThanhToan,
                            ID_DoiTuong = objCTQuyHoaDon.ID_DoiTuong,
                            ID_KhoanThuChi = objCTQuyHoaDon.ID_KhoanThuChi
                        };
                        db.Quy_HoaDon_ChiTiet.Add(ctQuy);

                        if (diemThanhToan != null && diemThanhToan > 0)
                        {
                            // update TongTichDiem for DoiTuong
                            if (objHoaDon.LoaiHoaDon == 12)
                            {
                                diemThanhToan = -diemThanhToan; // TT = Diem: bị trừ điểm, MuaHang: cộng điểm
                            }
                            classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                            DM_DoiTuong objUpd = db.DM_DoiTuong.Find(ctQuy.ID_DoiTuong);
                            if (objUpd != null)
                            {
                                var tongdiemOld = objUpd.TongTichDiem;
                                objUpd.TongTichDiem = tongdiemOld == null ? diemThanhToan : tongdiemOld + diemThanhToan;
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueData(new { objHoaDon.ID, MaHoaDon = sMaHoaDon, objHoaDon.TongTienThu });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult PostQuy_HoaDon_LstQuyChiTiet([FromBody] JObject data)// used to nhahang.js
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                Quy_HoaDon objQuyHD = data["objQuyHoaDon"].ToObject<Quy_HoaDon>();
                List<Quy_HoaDon_ChiTiet> lstCTQuyHoaDon = data["lstCTQuyHoaDon"].ToObject<List<Quy_HoaDon_ChiTiet>>();
                var ngaylapHD = objQuyHD.NgayLapHoaDon.AddMilliseconds(DateTime.Now.Millisecond);

                #region Quy_HoaDon
                Quy_HoaDon itemQuy_HoaDon = new Quy_HoaDon();
                itemQuy_HoaDon.ID = Guid.NewGuid();
                itemQuy_HoaDon.ID_NhanVien = objQuyHD.ID_NhanVien;
                itemQuy_HoaDon.NguoiTao = objQuyHD.NguoiTao;
                itemQuy_HoaDon.NgayLapHoaDon = ngaylapHD;
                itemQuy_HoaDon.NgayTao = DateTime.Now;
                itemQuy_HoaDon.ID_DonVi = objQuyHD.ID_DonVi;
                itemQuy_HoaDon.NguoiNopTien = objQuyHD.NguoiNopTien;
                itemQuy_HoaDon.TongTienThu = objQuyHD.TongTienThu;
                itemQuy_HoaDon.NoiDungThu = objQuyHD.NoiDungThu;
                itemQuy_HoaDon.TienMat = 0;
                itemQuy_HoaDon.TienGui = 0;
                itemQuy_HoaDon.LoaiHoaDon = objQuyHD.LoaiHoaDon;
                itemQuy_HoaDon.TrangThai = true;

                string sMaHoaDon = "";
                if (!string.IsNullOrEmpty(objQuyHD.MaHoaDon))
                {
                    sMaHoaDon = objQuyHD.MaHoaDon;
                }
                else
                {
                    sMaHoaDon = _classQHD.SP_GetMaPhieuThuChiMax_byTemp(objQuyHD.LoaiHoaDon, objQuyHD.ID_DonVi, ngaylapHD);
                }
                itemQuy_HoaDon.MaHoaDon = sMaHoaDon;
                #endregion

                string strIns = _classQHD.Add_SoQuy(itemQuy_HoaDon);
                if (strIns != null && strIns != string.Empty)
                    return Json(new { res = false, mes = HttpStatusCode.InternalServerError + strIns });
                else
                {
                    #region Quy_HoaDonChiTiet
                    foreach (var item in lstCTQuyHoaDon)
                    {
                        if (item.TienThu > 0)
                        {
                            Quy_HoaDon_ChiTiet ctQuyHoaDon = new Quy_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_HoaDon = itemQuy_HoaDon.ID,
                                ID_HoaDonLienQuan = item.ID_HoaDonLienQuan,
                                TienMat = item.TienMat,
                                TienGui = item.TienGui,
                                TienThu = item.TienThu,
                                ThuTuThe = item.ThuTuThe, // thu từ thẻ giá trị
                                HinhThucThanhToan = item.HinhThucThanhToan,
                                LoaiThanhToan = item.LoaiThanhToan,
                                DiemThanhToan = item.DiemThanhToan,
                                ID_NhanVien = item.ID_NhanVien,
                                ID_DoiTuong = item.ID_DoiTuong == null ? Guid.Empty : item.ID_DoiTuong,
                                ID_KhoanThuChi = item.ID_KhoanThuChi,
                                ChiPhiNganHang = item.ChiPhiNganHang,
                                LaPTChiPhiNganHang = item.LaPTChiPhiNganHang,
                                ID_TaiKhoanNganHang = item.ID_TaiKhoanNganHang,
                                ID_NganHang = item.ID_TaiKhoanNganHang != null ? db.DM_TaiKhoanNganHang.Find(item.ID_TaiKhoanNganHang).ID_NganHang : (Guid?)null,
                            };
                            strIns = _classQHDCT.Add_ChiTietQuyHoaDon(ctQuyHoaDon);
                        }
                    }
                    #endregion

                    _classQHD.UpdateSoDuThe_WhenChangeSoQuy(itemQuy_HoaDon.ID, itemQuy_HoaDon.NgayLapHoaDon);

                    return Json(new { res = true, data = new { itemQuy_HoaDon.ID, itemQuy_HoaDon.MaHoaDon, itemQuy_HoaDon.TongTienThu } });
                }
            }
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult Post_HDDatCoc([FromBody] JObject data)
        {
            try
            {
                if (data["objQuyHoaDon"] == null || data["objQuyHoaDonCT"] == null)
                {
                    return ActionFalseNotData("Vui lòng nhập dữ liệu");
                }

                Quy_HoaDon quyHD = data["objQuyHoaDon"].ToObject<Quy_HoaDon>();
                List<Quy_HoaDon_ChiTiet> quyHDCT = data["objQuyHoaDonCT"].ToObject<List<Quy_HoaDon_ChiTiet>>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                            classQuy_HoaDon classQuyHD = new classQuy_HoaDon(db);
                            ClassQuy_HoaDon_ChiTiet classQuyHDChiTiet = new ClassQuy_HoaDon_ChiTiet(db);

                            #region GetMaPhieuThu
                            string sMaHoaDon = "";
                            var idQuyHD = quyHD.ID;

                            if (!string.IsNullOrEmpty(quyHD.MaHoaDon))
                            {
                                if (idQuyHD != Guid.Empty)
                                {
                                    var exists = classQuyHD.Check_MaSoQuyExist(quyHD.MaHoaDon, idQuyHD);
                                    if (exists)
                                    {
                                        return ActionFalseNotData("Mã phiếu đặt cọc đã tồn tại");
                                    }
                                    sMaHoaDon = quyHD.MaHoaDon;
                                }
                                else
                                {
                                    sMaHoaDon = classQuyHD.GetMaPhieuThuChi_whenUpdateHD(quyHD.MaHoaDon);
                                }
                            }
                            else
                            {
                                sMaHoaDon = classQuyHD.SP_GetMaPhieuThuChiMax_byTemp(quyHD.LoaiHoaDon, quyHD.ID_DonVi, quyHD.NgayLapHoaDon);
                            }
                            #endregion

                            if (idQuyHD != Guid.Empty)
                            {
                                var qctOld = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == quyHD.ID);
                                db.Quy_HoaDon_ChiTiet.RemoveRange(qctOld);

                                Quy_HoaDon qhd = db.Quy_HoaDon.Find(idQuyHD);
                                qhd.ID_DonVi = quyHD.ID_DonVi;
                                qhd.ID_NhanVien = quyHD.ID_NhanVien;
                                qhd.MaHoaDon = sMaHoaDon;
                                qhd.NgayLapHoaDon = quyHD.NgayLapHoaDon;
                                qhd.NoiDungThu = quyHD.NoiDungThu;
                                qhd.TongTienThu = quyHD.TongTienThu;
                                qhd.NguoiNopTien = quyHD.NguoiNopTien;
                                qhd.HachToanKinhDoanh = quyHD.HachToanKinhDoanh;
                                qhd.PhieuDieuChinhCongNo = quyHD.PhieuDieuChinhCongNo;// 2. phieudatcoc, 4.hoantra sodu TGT
                                qhd.NguoiSua = quyHD.NguoiTao;
                                qhd.NgaySua = DateTime.Now;
                            }
                            else
                            {
                                idQuyHD = Guid.NewGuid();
                                quyHD.ID = idQuyHD;
                                quyHD.MaHoaDon = sMaHoaDon;
                                quyHD.PhieuDieuChinhCongNo = quyHD.PhieuDieuChinhCongNo;//2. phieudatcoc, 4.hoantra sodu TGT
                                classQuyHD.Add_SoQuy(quyHD);
                            }

                            #region Add Again QuyCT
                            List<Quy_HoaDon_ChiTiet> lst = new List<Quy_HoaDon_ChiTiet>();
                            foreach (var item in quyHDCT)
                            {
                                item.ID = Guid.NewGuid();
                                item.ID_HoaDon = idQuyHD;
                                lst.Add(item);
                            }
                            db.Quy_HoaDon_ChiTiet.AddRange(lst);
                            #endregion

                            db.SaveChanges();
                            trans.Commit();
                            return ActionTrueData(sMaHoaDon);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            return ActionFalseNotData(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        // use at BanHang
        [HttpPost, HttpGet]
        public IHttpActionResult PostQuy_HoaDon_DefaultIDDoiTuong([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                libHT.classHT_CauHinhPhanMem classCauHinh = new libHT.classHT_CauHinhPhanMem(db);

                Quy_HoaDon objQuyHD = data["objQuyHoaDon"].ToObject<Quy_HoaDon>();
                List<Quy_HoaDon_ChiTiet> lstCTQuyHoaDon = data["lstCTQuyHoaDon"].ToObject<List<Quy_HoaDon_ChiTiet>>();

                var idDoiTuong = objQuyHD.ID_DoiTuong == null ? "00000000-0000-0000-0000-000000000000" : objQuyHD.ID_DoiTuong.ToString();
                var nguoiNopTien = string.Empty;
                switch (idDoiTuong)
                {
                    case "00000000-0000-0000-0000-000000000000":
                        nguoiNopTien = "Khách Lẻ";
                        break;
                    case "00000000-0000-0000-0000-000000000002":
                        nguoiNopTien = "Nhà Cung Cấp Lẻ";
                        break;
                    default:
                        nguoiNopTien = objQuyHD.NguoiNopTien;
                        break;
                }

                #region Quy_HoaDon
                DateTime ngaylapHD = objQuyHD.NgayLapHoaDon.AddMilliseconds(1);
                Quy_HoaDon itemQuy_HoaDon = new Quy_HoaDon();
                itemQuy_HoaDon.ID = Guid.NewGuid();
                itemQuy_HoaDon.ID_NhanVien = objQuyHD.ID_NhanVien;
                itemQuy_HoaDon.NguoiTao = objQuyHD.NguoiTao;
                itemQuy_HoaDon.NgayLapHoaDon = ngaylapHD;
                itemQuy_HoaDon.NgayTao = DateTime.Now;
                itemQuy_HoaDon.ID_DonVi = objQuyHD.ID_DonVi;
                itemQuy_HoaDon.NguoiNopTien = nguoiNopTien;
                itemQuy_HoaDon.TongTienThu = objQuyHD.TongTienThu;
                itemQuy_HoaDon.NoiDungThu = objQuyHD.NoiDungThu;
                itemQuy_HoaDon.TienMat = 0;
                itemQuy_HoaDon.TienGui = 0;
                itemQuy_HoaDon.LoaiHoaDon = objQuyHD.LoaiHoaDon;
                itemQuy_HoaDon.TrangThai = true;
                itemQuy_HoaDon.PhieuDieuChinhCongNo = objQuyHD.PhieuDieuChinhCongNo;
                itemQuy_HoaDon.HachToanKinhDoanh = objQuyHD.HachToanKinhDoanh;

                string sMaHoaDon;
                if (!string.IsNullOrEmpty(objQuyHD.MaHoaDon))
                {
                    var exists = _classQHD.Check_MaSoQuyExist(objQuyHD.MaHoaDon);
                    if (exists)
                    {
                        sMaHoaDon = _classQHD.GetMaPhieuThuChi_whenUpdateHD(objQuyHD.MaHoaDon);
                    }
                    else
                    {
                        sMaHoaDon = objQuyHD.MaHoaDon;
                    }
                }
                else
                {
                    sMaHoaDon = _classQHD.SP_GetMaPhieuThuChiMax_byTemp(objQuyHD.LoaiHoaDon, objQuyHD.ID_DonVi, objQuyHD.NgayLapHoaDon);
                }
                itemQuy_HoaDon.MaHoaDon = sMaHoaDon;
                #endregion

                string strIns = _classQHD.Add_SoQuy(itemQuy_HoaDon);
                if (strIns != null && strIns != string.Empty)
                    return Json(new { res = false, mes = strIns });
                else
                {
                    #region Quy_HoaDonChiTiet
                    foreach (var item in lstCTQuyHoaDon)
                    {
                        if (item.TienThu > 0)
                        {
                            Quy_HoaDon_ChiTiet ctQuyHoaDon = new Quy_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_HoaDon = itemQuy_HoaDon.ID,
                                ID_HoaDonLienQuan = item.ID_HoaDonLienQuan,
                                TienMat = item.TienMat,
                                TienGui = item.TienGui,
                                TienThu = item.TienThu,
                                ThuTuThe = item.ThuTuThe, // thu từ thẻ giá trị
                                DiemThanhToan = item.DiemThanhToan,
                                ID_DoiTuong = item.ID_DoiTuong ?? new Guid(idDoiTuong),
                                ID_NhanVien = item.ID_NhanVien,
                                ID_KhoanThuChi = item.ID_KhoanThuChi,
                                ID_TaiKhoanNganHang = item.ID_TaiKhoanNganHang,
                                ID_NganHang = item.ID_TaiKhoanNganHang != null ? db.DM_TaiKhoanNganHang.Find(item.ID_TaiKhoanNganHang).ID_NganHang : (Guid?)null,
                                HinhThucThanhToan = item.HinhThucThanhToan,
                                LoaiThanhToan = item.LoaiThanhToan,
                                GhiChu = item.GhiChu,
                                ChiPhiNganHang = item.ChiPhiNganHang,
                                LaPTChiPhiNganHang = item.LaPTChiPhiNganHang,
                            };
                            strIns = _classQHDCT.Add_ChiTietQuyHoaDon(ctQuyHoaDon);
                        }
                    }
                    #endregion

                    _classQHD.UpdateSoDuThe_WhenChangeSoQuy(itemQuy_HoaDon.ID, itemQuy_HoaDon.NgayLapHoaDon);

                    return Json(new { res = true, data = new { itemQuy_HoaDon.ID, itemQuy_HoaDon.MaHoaDon, itemQuy_HoaDon.TongTienThu, itemQuy_HoaDon.NgayLapHoaDon } });
                }
            }
        }

        /// <summary>
        /// update couln TongTienThue (SoDuSauNap) when napthe
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="ngaylapHD"></param>
        [HttpGet]
        public void UpdateSoDuThe(Guid? idDoiTuong, DateTime ngaylapHD)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                classHoaDonCT.UpdateTheGiaTri(idDoiTuong, ngaylapHD);
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult PostQuy_HoaDon_BangLuong([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                libHT.classHT_CauHinhPhanMem classCauHinh = new libHT.classHT_CauHinhPhanMem(db);

                List<Quy_HoaDon> lstQuy = new List<Quy_HoaDon>();
                List<Quy_HoaDon_ChiTiet> lstQuyCT = new List<Quy_HoaDon_ChiTiet>();
                List<QuyHD_QuyCT> lstPost = data["lstQuy"].ToObject<List<QuyHD_QuyCT>>();

                try
                {
                    var noidung = string.Empty;
                    foreach (var item in lstPost)
                    {
                        var objQuyHD = item.Quy_HoaDon;
                        var quyct = item.Quy_HoaDon_ChiTiet[0];

                        #region Quy_HoaDon
                        string mahoadon = "PC" + quyct.MaBangLuongChiTiet;
                        var countPC = db.Quy_HoaDon_ChiTiet.Where(x => x.ID_BangLuongChiTiet == quyct.ID_BangLuongChiTiet).Count();
                        if (countPC > 0)
                        {
                            mahoadon = mahoadon + "_" + countPC;
                        }
                        Quy_HoaDon newQuyHD = new Quy_HoaDon();
                        newQuyHD.MaHoaDon = mahoadon;
                        newQuyHD.ID = Guid.NewGuid();
                        newQuyHD.ID_NhanVien = objQuyHD.ID_NhanVien;
                        newQuyHD.NguoiTao = objQuyHD.NguoiTao;
                        newQuyHD.NgayLapHoaDon = objQuyHD.NgayLapHoaDon.AddMilliseconds(1);
                        newQuyHD.NgayTao = DateTime.Now;
                        newQuyHD.ID_DonVi = objQuyHD.ID_DonVi;
                        newQuyHD.NguoiNopTien = objQuyHD.NguoiNopTien;
                        newQuyHD.TongTienThu = objQuyHD.TongTienThu;
                        newQuyHD.NoiDungThu = objQuyHD.NoiDungThu;
                        newQuyHD.LoaiHoaDon = objQuyHD.LoaiHoaDon;
                        newQuyHD.HachToanKinhDoanh = objQuyHD.HachToanKinhDoanh;
                        newQuyHD.PhieuDieuChinhCongNo = objQuyHD.PhieuDieuChinhCongNo;
                        newQuyHD.TrangThai = true;
                        lstQuy.Add(newQuyHD);
                        #endregion

                        Quy_HoaDon_ChiTiet ctQuyHoaDon = new Quy_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_HoaDon = newQuyHD.ID,
                            ID_HoaDonLienQuan = null,
                            ID_BangLuongChiTiet = quyct.ID_BangLuongChiTiet,
                            TienMat = quyct.TienMat,
                            TienGui = quyct.TienGui,
                            TienThu = quyct.TienThu,
                            ThuTuThe = quyct.ThuTuThe,
                            DiemThanhToan = quyct.DiemThanhToan,
                            ID_DoiTuong = null,
                            ID_NhanVien = quyct.ID_NhanVien,
                            ID_KhoanThuChi = quyct.ID_KhoanThuChi,
                            ID_TaiKhoanNganHang = quyct.ID_TaiKhoanNganHang,
                            ID_NganHang = quyct.ID_NganHang,
                            TruTamUngLuong = quyct.TruTamUngLuong,// tru tamung
                            HinhThucThanhToan = quyct.HinhThucThanhToan,
                            LoaiThanhToan = quyct.LoaiThanhToan,
                        };
                        lstQuyCT.Add(ctQuyHoaDon);
                    }

                    db.Quy_HoaDon.AddRange(lstQuy);
                    db.Quy_HoaDon_ChiTiet.AddRange(lstQuyCT);
                    db.SaveChanges();
                    // update nợ tạm ưng lương of nhanvien 
                    NhanSuService nhanSuService = new NhanSuService(db);
                    nhanSuService.UpdateCongNo_TamUngLuong(lstQuy.Select(x => x.ID_DonVi).First(), lstQuyCT.Select(x => x.ID).ToList(), false);
                    return Json(new { res = true, mes = string.Empty, data = lstQuyCT.Select(x => x.ID) });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [HttpPost, ActionName("PostQuy_KhoanThuChi")]
        [ResponseType(typeof(Quy_KhoanThuChi))]
        public IHttpActionResult PostQuy_KhoanThuChi(Quy_KhoanThuChi objQuy_KhoanThuChi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassQuy_KhoanThuChi _classQKTC = new ClassQuy_KhoanThuChi(db);
                objQuy_KhoanThuChi.ID = Guid.NewGuid();
                objQuy_KhoanThuChi.MaKhoanThuChi = DateTime.Now.ToString("yyyyMMddHHmmss");
                objQuy_KhoanThuChi.NgayTao = DateTime.Now;
                #endregion

                string strIns = _classQKTC.AddQuy_KhoanThuChi(objQuy_KhoanThuChi);
                if (strIns != null && strIns != string.Empty && strIns.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objQuy_KhoanThuChi.ID }, objQuy_KhoanThuChi);
            }
        }

        [ResponseType(typeof(Quy_KhoanThuChi))]
        public IHttpActionResult GetQuy_KhoanThuChi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassQuy_KhoanThuChi _classQKTC = new ClassQuy_KhoanThuChi(db);
                Quy_KhoanThuChi quy_KhoanThuChi = _classQKTC.SelectQuy_KhoanThuChi(id);
                Quy_KhoanThuChi temp = new Quy_KhoanThuChi();
                temp.ID = quy_KhoanThuChi.ID;
                temp.NoiDungThuChi = quy_KhoanThuChi.NoiDungThuChi;
                temp.GhiChu = quy_KhoanThuChi.GhiChu;
                temp.LaKhoanThu = quy_KhoanThuChi.LaKhoanThu;
                temp.TinhLuong = quy_KhoanThuChi.TinhLuong;
                if (quy_KhoanThuChi == null)
                {
                    return NotFound();
                }
                return Ok(temp);
            }
        }
        public IHttpActionResult GetQuy_KhoanThuChi()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassQuy_KhoanThuChi _classQKTC = new ClassQuy_KhoanThuChi(db);
                try
                {
                    var data = _classQKTC.Gets(x => x.TrangThai == null || x.TrangThai != 0)
                        .Select(x => new
                        {
                            x.ID,
                            x.MaKhoanThuChi,
                            NoiDungThuChi = x.NoiDungThuChi ?? string.Empty,
                            x.LaKhoanThu,
                            x.BuTruCongNo,
                            x.TinhLuong,
                            LoaiChungTu = x.LoaiChungTu ?? string.Empty,
                        });
                    return Json(new { res = true, data = data });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = string.Concat("GetQuy_KhoanThuChi", e.Message, e.InnerException) });
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult UpdateIDQuyHoaDon_toBHThucHien(Guid idHoaDon, Guid idQuyHD)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                if (db == null)
                {
                    return Json(new { res = false, mes = "DB null" });
                }
                else
                {
                    db.Database.ExecuteSqlCommand(string.Concat("Update BH_NhanVienThucHien set ID_QuyHoaDon ='", idQuyHD, "' WHERE ID_HoaDon = '", idHoaDon, "'"));
                    return Json(new { res = true, mes = string.Empty });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = string.Concat("UpdateIDQuyHoaDon_toBHThucHien", e.InnerException, e.Message) });
            }
        }
        #region ###

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion
    }


    public class jqAutoResultSQDT
    {
        public string label { get; set; }
        public string value { get; set; }
        public Guid actual { get; set; }
        public Quy_HoaDonDTO data { get; set; }
    }

    public class QuyHD_QuyCT
    {
        public Quy_HoaDon Quy_HoaDon { get; set; }
        public List<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }
    }
}
