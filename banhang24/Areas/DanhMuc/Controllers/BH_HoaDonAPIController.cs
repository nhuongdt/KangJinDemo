using banhang24.Compress;
using banhang24.Hellper;
using libBH_NhanVienThucHien;
using libDM_DoiTuong;
using libDM_DonVi;
using libDM_GiaBan;
using libDM_HangHoa;
using libDM_Kho;
using libDM_NhomDoiTuong;
using libDM_ViTri;
using libDonViQuiDoi;
using libHT;
using libHT_NguoiDung;
using libNS_NhanVien;
using libQuy_HoaDon;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using static libDM_DoiTuong.ClassBH_HoaDon;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class BH_HoaDonAPIController : BaseApiController
    {
        //
        // GET: /DanhMuc/BH_HoaDonAPI/

        #region GET
        // GET: api/DanhMuc/BH_HoaDonAPI/GetBH_HoaDon
        public IQueryable<BH_HoaDon> GetBH_HoaDon()
        {
            //SerialPort sp = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
            //sp.Open();
            //sp.Write("\f");// Clear pole display, \r - next line \v - first row first column
            //sp.DataBits = 8;
            //sp.BaudRate = 9600;
            //sp.PortName = "COM";
            //sp.NewLine = "abc";
            //byte[] bytesToSend = new byte[1] { 0x0C }; // send hex code 0C to clear screen
            //sp.Write(bytesToSend, 0, 1);
            //sp.Close();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.Gets(null);
            }
        }

        public IQueryable<BH_HoaDonDTO> GetAllHoaDon()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.GetAllHoaDon();
            }
        }

        [DeflateCompression]
        [HttpGet]
        public List<ClassDM_HangHoa.SP_ThanhPhan_DinhLuong> GetAllDinhLuongDV_byChiNhanh(Guid idChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.SP_GetInfor_TPDinhLuong(idChiNhanh, Guid.Empty);
            }
        }
        [HttpGet, HttpPost]
        public IHttpActionResult GetTPDinhLuong_ofHoaDon([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    if (data != null && data["lstIDHoaDon"] != null)
                    {
                        List<Guid> lstIDHoDon = data["lstIDHoaDon"].ToObject<List<Guid>>();
                        ClassBH_HoaDon_ChiTiet classHDChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                        List<SP_ThanhPhanDinhLuong> lst = classHDChiTiet.GetTPDinhLuong_ofHoaDon(lstIDHoDon);
                        return ActionTrueData(lst);
                    }
                    return ActionFalseNotData("Tham số null");
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult InsertChietKhauTraHang_TheoThucThu(Guid idHoaDonTra, Guid idPhieuChi)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_HoaDonTra", idHoaDonTra));
                    lstParam.Add(new SqlParameter("ID_PhieuChi", idPhieuChi));
                    db.Database.ExecuteSqlCommand("exec InsertChietKhauTraHang_TheoThucThu @ID_HoaDonTra, @ID_PhieuChi", lstParam.ToArray());
                }
                return ActionTrueNotData(string.Empty);
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException + ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult HDTraHang_InsertTPDinhLuong(Guid idHoaDonTra)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon_ChiTiet classHDChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                    classHDChiTiet.HDTraHang_InsertTPDinhLuong(idHoaDonTra);
                }
                return ActionTrueNotData(string.Empty);
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException + ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetChietKhauNV_HoaDon(Guid idHoaDon)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    // truong hop hoadon thanhtoan nhieulan --> get chietkhau max
                    var data = (from nvth in db.BH_NhanVienThucHien
                                join nv in db.NS_NhanVien on nvth.ID_NhanVien equals nv.ID
                                where nvth.ID_HoaDon == idHoaDon
                                && nvth.TinhChietKhauTheo == 1 // only get NV was setup by ThucThu
                                group new { nvth, nv } by new { nvth.ID_HoaDon, nvth.ID_NhanVien, nv.TenNhanVien } into g
                                select new
                                {
                                    g.Key.ID_HoaDon,
                                    g.Key.ID_NhanVien,
                                    g.Key.TenNhanVien,
                                    ThucHien_TuVan = false,
                                    TheoYeuCau = false,
                                    TinhChietKhauTheo = 1,
                                    HeSo = g.Max(x => x.nvth.HeSo),
                                    PT_ChietKhau = g.Max(x => x.nvth.PT_ChietKhau),
                                    TienChietKhau = g.Max(x => x.nvth.TienChietKhau),
                                    ChietKhauMacDinh = g.Max(x => x.nvth.PT_ChietKhau),
                                }).ToList();
                    return Json(new { data, res = true });
                }
            }
            catch (Exception ex)
            {
                string str = CookieStore.GetCookieAes("SubDomain");
                CookieStore.WriteLog("GetChietKhauNV_HoaDon(Guid idHoaDon): " + ex.InnerException + ex.Message, str);
                return Json(new { res = false });
            }
        }

        [HttpGet]
        public bool ServicePackage_CheckUsed(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon_ChiTiet hoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                return hoadonchitiet.ServicePackage_CheckUsed(idHoaDon);
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult GetListNVChietKhau_ManyHoaDon([FromBody] JObject data)
        {
            try
            {
                List<Guid?> lstID = data["IDHoaDons"].ToObject<List<Guid?>>();
                Guid? idQuy = Guid.Empty;
                if (data["idQuyHD"] != null)
                {
                    idQuy = data["idQuyHD"].ToObject<Guid?>();
                }
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var lstNV = (from nvth in db.BH_NhanVienThucHien
                                 join nv in db.NS_NhanVien on nvth.ID_NhanVien equals nv.ID
                                 where lstID.Contains(nvth.ID_HoaDon)
                                 && (idQuy == Guid.Empty || nvth.ID_QuyHoaDon == idQuy)
                                 && nvth.TinhChietKhauTheo == 1 // only get NV was setup by ThucThu
                                 group new { nvth, nv } by new { nvth.ID_HoaDon, nvth.ID_NhanVien, nv.TenNhanVien } into g
                                 select new
                                 {
                                     g.Key.ID_HoaDon,
                                     g.Key.ID_NhanVien,
                                     g.Key.TenNhanVien,
                                     ThucHien_TuVan = false,
                                     TheoYeuCau = false,
                                     TinhChietKhauTheo = 1,
                                     HeSo = g.Max(x => x.nvth.HeSo),
                                     PT_ChietKhau = g.Max(x => x.nvth.PT_ChietKhau),
                                     TienChietKhau = g.Max(x => x.nvth.TienChietKhau),
                                     ChietKhauMacDinh = g.Max(x => x.nvth.PT_ChietKhau),
                                 }).ToList();

                    return Json(new { data = lstNV, res = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IEnumerable<BH_HoaDonDTO> LoadHoaDonByIDViTri(int currentPage, int pageSize, Guid idvitri)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lstAllHDs = classhoadon.Gets(gr => gr.ID_ViTri == idvitri).OrderByDescending(od => od.NgayLapHoaDon).Skip(currentPage * pageSize).Take(pageSize).Select(p => new BH_HoaDonDTO
                {
                    MaHoaDon = p.MaHoaDon,
                    NgayLapHoaDon = p.NgayLapHoaDon,
                    TongTienHang = p.TongTienHang,
                    TenDoiTuong = p.DM_DoiTuong != null ? p.DM_DoiTuong.TenDoiTuong : "",
                    LoaiHoaDon = p.LoaiHoaDon
                });
                return lstAllHDs.ToList();
            }
        }

        public PageListDTO GetPageCountLoadHoaDon(int currentPage, float pageSize, Guid idvitri)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var totalRecords = 0;
                var lstAllHDs = classhoadon.Gets(gr => gr.ID_ViTri == idvitri).Select(p => new BH_HoaDonDTO
                {
                    MaHoaDon = p.MaHoaDon,
                    NgayLapHoaDon = p.NgayLapHoaDon,
                    TongTienHang = p.TongTienHang,
                    TenDoiTuong = p.DM_DoiTuong != null ? p.DM_DoiTuong.TenDoiTuong : "",
                    LoaiHoaDon = p.LoaiHoaDon
                });
                if (lstAllHDs != null)
                {
                    totalRecords = lstAllHDs.Count();
                }

                PageListDTO pageListDTO = new PageListDTO
                {
                    TotalRecord = totalRecords,
                    PageCount = System.Math.Ceiling(totalRecords / pageSize) // round 6.1 --> 7
                };
                return pageListDTO;
            }
        }

        //Trinhpv_xuất excle
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel_KiemKho(int loaiHoaDon, string maHoaDon,
            int trangThai, string dayStart, string dayEnd, string columnsHide, string iddonvi, string arrChiNhanh, string time, string TenChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string columsort = null;
                string sort = null;
                var lstAllHDs = classhoadon.GetListHoaDonsKK_Where(loaiHoaDon, maHoaDon, trangThai, dayStart, dayEnd, iddonvi, arrChiNhanh, columsort, sort);
                List<BH_KiemKho_Excel> lst = new List<BH_KiemKho_Excel>();
                foreach (var item in lstAllHDs)
                {
                    BH_KiemKho_Excel DM = new BH_KiemKho_Excel();
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.TongGiamGia = item.TongGiamGia;
                    DM.TongChiPhi = item.TongChiPhi;
                    DM.TongTienHang = item.TongTienHang;
                    DM.TenDonVi = item.TenDonVi;
                    DM.DienGiai = item.DienGiai;
                    DM.ChoThanhToan = item.ChoThanhToan == null ? "Đã hủy" : (item.ChoThanhToan == false ? "Đã cân bằng kho" : "Phiếu tạm");
                    lst.Add(DM);
                }
                DataTable excel = _classOFDCM.ToDataTable<BH_KiemKho_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_KiemKhoHangHoa.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/KiemKhoHangHoa.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, columnsHide, time, TenChiNhanh);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel_KiemKhoChiTiet(Guid ID_HoaDon, string columnsHide)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                List<BH_KiemKhoChiTiet_Excel> lst = db.Database.SqlQuery<BH_KiemKhoChiTiet_Excel>("EXEC GetListChiTietHoaDonKiemKhoXuatFile @ID_HoaDon", paramlist.ToArray()).ToList();

                DataTable excel = _classOFDCM.ToDataTable<BH_KiemKhoChiTiet_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_KiemKhoHangHoa _ChiTiet.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/KiemKhoHangHoa_ChiTiet.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, false, columnsHide);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }
        public List<BH_HoaDonDTO> GetListHoaDonsKiemKho_where(int currentPage, int pageSize, int loaiHoaDon, string maHoaDon,
            int trangThai, string dayStart, string dayEnd, string iddonvi, string arrChiNhanh, string columsort, string sort)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lstAllHDs = classhoadon.GetListHoaDonsKK_Where(loaiHoaDon, maHoaDon, trangThai, dayStart, dayEnd, iddonvi, arrChiNhanh, columsort, sort);
                List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();

                if (lstAllHDs != null && lstAllHDs.Count() > 0)
                {
                    lstAllHDs = lstAllHDs.Skip(currentPage * pageSize).Take(pageSize).ToList();
                    foreach (BH_HoaDonDTO item in lstAllHDs)
                    {
                        BH_HoaDonDTO itemData = new BH_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            TenDonVi = item.TenDonVi,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            PhaiThanhToan = item.PhaiThanhToan, // Giatritang
                            TongChietKhau = item.TongChietKhau, //GiaTriGiam
                            TongTienThue = item.PhaiThanhToan - item.TongChietKhau, //TongTienlech
                            TongGiamGia = Math.Round(item.TongGiamGia, 3, MidpointRounding.ToEven),
                            TongTienHang = Math.Round(item.TongTienHang, 3, MidpointRounding.ToEven),
                            TongChiPhi = Math.Round(item.TongChiPhi, 3, MidpointRounding.ToEven),
                            TenDoiTuong = item.TenDoiTuong ?? "Khách lẻ",
                            DienGiai = item.DienGiai,
                            TenPhongBan = item.TenPhongBan,
                            ChoThanhToan = item.ChoThanhToan,
                            NguoiTao = item.NguoiTao,
                            TenNhanVien = item.TenNhanVien,
                            ID_DonVi = item.ID_DonVi
                        };
                        lsrReturns.Add(itemData);
                    }
                }
                return lsrReturns;
            }
        }

        public PageListDTO GetPageCountHoaDonKK_Where(int currentPage, float pageSize, int loaiHoaDon, string maHoaDon,
            int trangThai, string dayStart, string dayEnd, string iddonvi, string arrChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var totalRecords = 0;
                {
                    string columsort = null;
                    string sort = null;
                    var data = classhoadon.GetListHoaDonsKK_Where(loaiHoaDon, maHoaDon, trangThai, dayStart, dayEnd, iddonvi, arrChiNhanh, columsort, sort);
                    if (data != null)
                    {
                        totalRecords = data.Count();
                    }

                    PageListDTO pageListDTO = new PageListDTO
                    {
                        TotalRecord = totalRecords,
                        PageCount = System.Math.Ceiling(totalRecords / pageSize) // round 6.1 --> 7
                    };
                    return pageListDTO;
                }
            }
        }

        public double GetPageCountHoaDonKK(float pageSize, int loaiHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var totalRecord = classhoadon.Gets(p => p.LoaiHoaDon == loaiHoaDon).Count();
                // round 6.1 --> 7
                return System.Math.Ceiling(totalRecord / pageSize);
            }
        }

        public double GetTotalRecordKK(int loaiHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.Gets(p => p.LoaiHoaDon == loaiHoaDon).Count();
            }
        }

        //trinhpv xuất excecl chuyển hàng
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel_ChuyenHang(int loaiHoaDon, string maHoaDon,
           int trangThai, string dayStart, string dayEnd, Guid id_donvi, string ColumnsHide, string arrChiNhanh, string time, string TenChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                classDM_DonVi _classDV = new classDM_DonVi(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string columsort = null;
                string sort = null;
                var roleXemGiaVon = CheckRoleXemGiaVon(db);

                List<BH_HoaDonDTO> lstAllHDs = classhoadon.GetListHoaDons_QuyHD_where(loaiHoaDon, maHoaDon, trangThai, dayStart, dayEnd, id_donvi, arrChiNhanh, columsort, sort);
                List<BH_ChuyenHang_Excel> lst = new List<BH_ChuyenHang_Excel>();
                foreach (var item in lstAllHDs)
                {
                    BH_ChuyenHang_Excel DM = new BH_ChuyenHang_Excel();
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.TenDonViChuyen = item.TenDonVi;
                    DM.TenNhanVien = item.NguoiTao;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.TenDonVi = item.ID_CheckIn == null ? "" : _classDV.Get(p => p.ID == item.ID_CheckIn).TenDonVi;
                    DM.TenDoiTuong = item.NguoiTaoHD;// nguoinhan
                    DM.NgayNhan = item.NgaySua;
                    if (roleXemGiaVon)
                    {
                        DM.TongTienHang = item.TongTienHang;
                        DM.TongChiPhi = item.TongChiPhi;// grinhan
                    }
                    else
                    {
                        DM.TongTienHang = 0;
                        DM.TongChiPhi = 0;// grinhan
                    }

                    DM.DienGiai = item.DienGiai;
                    DM.YeuCau = item.YeuCau == "1" ? "Đang Chuyển" : (item.YeuCau == "2" ? "Tạm lưu" : (item.YeuCau == "3" ? "Đã hủy" : "Đã nhận"));
                    lst.Add(DM);
                }
                DataTable excel = _classOFDCM.ToDataTable<BH_ChuyenHang_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuChuyenHang.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuChuyenHang.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, ColumnsHide, time, TenChiNhanh);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel__ChiTietPhieuChuyenHang(Guid ID_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                BH_HoaDon bhhd = classhoadon.Get(p => p.ID == ID_HoaDon);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                List<BH_ChiTietHoaDon_Excel> lstHDCT = db.Database.SqlQuery<BH_ChiTietHoaDon_Excel>("EXEC GetListChiTietHoaDonXuatFile @ID_HoaDon", paramlist.ToArray()).ToList();
                List<BH_ChiTietPhieuChuyenHang_Excel> lst = new List<BH_ChiTietPhieuChuyenHang_Excel>();
                var roleXemGiaVon = CheckRoleXemGiaVon(db);

                foreach (var item in lstHDCT)
                {
                    BH_ChiTietPhieuChuyenHang_Excel DM = new BH_ChiTietPhieuChuyenHang_Excel();
                    DM.MaHangHoa = item.MaHangHoa;
                    DM.TenHangHoa = item.TenHangHoa;
                    DM.TenDonViTinh = item.TenDonViTinh;
                    DM.MaLoHang = item.MaLoHang;
                    DM.SoLuong = item.SoLuong;
                    if (roleXemGiaVon)
                    {
                        DM.GiaTriChuyen = item.DonGia;
                        DM.ThanhTien = item.ThanhTien;
                    }
                    else
                    {
                        DM.GiaTriChuyen = 0;
                        DM.ThanhTien = 0;
                    }
                    DM.GhiChu = item.GhiChu;
                    lst.Add(DM);
                }
                DataTable excel = _classOFDCM.ToDataTable<BH_ChiTietPhieuChuyenHang_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuChuyenHang_ChiTiet.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuChuyenHang_ChiTiet.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, null);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }
        public System.Web.Http.Results.JsonResult<JsonResultExampleCH> GetListHoaDonsChuyenHang_Where(int currentPage, int pageSize, int loaiHoaDon, string maHoaDon,
        int trangThai, string dayStart, string dayEnd, Guid id_donvi, string arrChiNhanh, string columsort, string sort)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                classDM_DonVi _classDV = new classDM_DonVi(db);
                List<BH_HoaDonDTO> lstAllHDs = classhoadon.GetListHoaDons_QuyHD_where(loaiHoaDon, maHoaDon, trangThai, dayStart, dayEnd, id_donvi, arrChiNhanh, columsort, sort);
                try
                {
                    if (lstAllHDs != null)
                    {
                        var tongGiaTriChuyen = lstAllHDs.Sum(p => p.TongTienHang);
                        var tongGiaTriNhan = lstAllHDs.Sum(p => p.TongChiPhi);
                        List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                        int TotalRecord = lstAllHDs.Count;
                        double PageCount = System.Math.Ceiling(lstAllHDs.Count / (float)pageSize);// round 6.1 --> 7
                        lstAllHDs = lstAllHDs.Skip(currentPage * pageSize).Take(pageSize).ToList();
                        foreach (BH_HoaDonDTO item in lstAllHDs)
                        {
                            BH_HoaDonDTO itemData = new BH_HoaDonDTO
                            {
                                ID = item.ID,
                                MaHoaDon = item.MaHoaDon,
                                //TonKho = slton,
                                TenNhanVien = item.TenNhanVien,
                                NguoiTao = item.NguoiTao,
                                TenDonVi = item.TenDonVi,
                                DiaChiChiNhanh = item.DiaChiChiNhanh,
                                DienThoaiChiNhanh = item.DienThoaiChiNhanh,
                                ID_DonVi = item.ID_DonVi,
                                ID_CheckIn = item.ID_CheckIn,
                                TenDonViChuyen = item.ID_CheckIn == null ? "" : _classDV.Get(p => p.ID == item.ID_CheckIn).TenDonVi,
                                NgayLapHoaDon = item.NgayLapHoaDon,
                                NgaySua = item.NgaySua,
                                TongGiamGia = item.TongGiamGia,
                                TongTienHang = item.TongTienHang,
                                TongChiPhi = item.TongChiPhi,
                                PhaiThanhToan = item.PhaiThanhToan,
                                TenDoiTuong = item.TenDoiTuong ?? "Khách lẻ",
                                DienGiai = item.DienGiai,
                                Email = item.Email,
                                DienThoai = item.DienThoai,
                                TenPhongBan = item.TenPhongBan,
                                //KhachDaTra = khachTra,
                                ID_NhanVien = item.ID_NhanVien,
                                NguoiTaoHD = item.NguoiTaoHD,
                                TenBangGia = item.TenBangGia,
                                ChoThanhToan = item.ChoThanhToan,
                                YeuCau = item.YeuCau == "1" ? "Đang Chuyển" : (item.YeuCau == "2" ? "Tạm lưu" : (item.YeuCau == "3" ? "Đã hủy" : "Đã nhận")),
                                NguoiSua = item.YeuCau,
                                ID_DoiTuong = item.ID_DoiTuong,
                                TrangThai = item.ChoThanhToan == null ? "Đã hủy" : (item.ChoThanhToan == false ? "Hoàn thành" : "Tạm lưu"),

                            };
                            lsrReturns.Add(itemData);
                        }
                        JsonResultExampleCH jsonobj = new JsonResultExampleCH
                        {
                            Rowcount = TotalRecord,
                            pageCount = PageCount,
                            lstCH = lsrReturns,
                            TongTienHang = tongGiaTriChuyen,
                            TongChiPhi = tongGiaTriNhan
                        };
                        return Json(jsonobj);
                    }
                    else
                    {
                        CookieStore.WriteLog(string.Concat("GetListHoaDonsChuyenHang_Where: ", loaiHoaDon, maHoaDon, trangThai, dayStart, dayEnd, id_donvi, arrChiNhanh));
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_HoaDonAPI_GetListHoaDonsChuyenHang_Where: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public IHttpActionResult GetListHoaDons_ChoThanhToan(int loaiHoaDon, string idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);

                var lstHD = classhoadon.SP_GetHoaDonChoThanhToan(loaiHoaDon, idDonVi).Where(x => x.ChoThanhToan == true);
                List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                try
                {
                    if (lstHD != null && lstHD.Count() > 0)
                    {
                        foreach (BH_HoaDonDTO item in lstHD)
                        {
                            item.BH_NhanVienThucHiens = nhanvienThucHien.Gets(x => x.ID_HoaDon == item.ID).
                                                Select(x => new BH_NhanVienThucHienDTO
                                                {
                                                    ID = x.ID,
                                                    ID_ChiTietHoaDon = x.ID_ChiTietHoaDon,
                                                    ID_NhanVien = x.ID_NhanVien,
                                                    ID_HoaDon = x.ID_HoaDon,
                                                    TenNhanVien = x.NS_NhanVien.TenNhanVien,
                                                    TienChietKhau = x.TienChietKhau,
                                                    ThucHien_TuVan = x.ThucHien_TuVan,
                                                    TheoYeuCau = x.TheoYeuCau,
                                                    PT_ChietKhau = x.PT_ChietKhau,
                                                    TinhChietKhauTheo = x.TinhChietKhauTheo,
                                                    HeSo = x.HeSo,
                                                })
                                                .ToList();
                            item.BH_HoaDon_ChiTiet = classhoadon.SP_GetChiTietHD_byIDHoaDon_ChietKhauNV(item.ID ?? Guid.Empty);
                            lsrReturns.Add(item);
                        }
                    }
                    return Json(new { res = true, data = lsrReturns });
                }
                catch (Exception ex)
                {
                    return Json(new { res = false, mes = string.Concat("GetListHoaDons_ChoThanhToan", ex.Message, ex.InnerException) });
                }
            }
        }
        public List<BH_HoaDonDTO> GetListHoaDons_ChoThanhToanNhaBep(int loaiHoaDon, string idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lstHD = classhoadon.SP_GetHoaDonChoThanhToanNhaBep(loaiHoaDon, idDonVi).Where(x => x.ChoThanhToan == true);
                List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                try
                {
                    if (lstHD != null)
                    {
                        foreach (BH_HoaDonDTO item in lstHD)
                        {
                            item.BH_HoaDon_ChiTiet = classhoadon.SP_GetChiTietHD_byIDHoaDon(item.ID ?? Guid.Empty);
                            lsrReturns.Add(item);
                        }
                    }
                    return lsrReturns;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("GetListHoaDons_ChoThanhToan " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public PageListDTO GetPageCountHoaDon(int loaiHoaDon, float pageSize)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var totalRecord = classhoadon.GetListHoaDons_QuyHD(loaiHoaDon).Count();
                PageListDTO dto = new PageListDTO
                {
                    TotalRecord = totalRecord,
                    PageCount = System.Math.Ceiling(totalRecord / pageSize)
                };
                return dto;
            }
        }

        public double GetTotalRecord(int loaiHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.GetListHoaDons_QuyHD(loaiHoaDon).Count();
            }
        }

        #region Export Excel
        [HttpGet, HttpPost]
        public string XuatFileHD_TongQuan([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                    List<BH_HoaDon_Excel> lstExport = data["LstExport"].ToObject<List<BH_HoaDon_Excel>>();
                    var loaiHoaDon = data["LoaiHoaDon"].ToObject<int>();
                    var dayStart = data["DayStart"].ToObject<DateTime>();
                    var dayEnd = data["DayEnd"].ToObject<string>();
                    var columnsHide = data["ColumnsHide"].ToObject<string>();
                    var valExcel2 = data["ChiNhanhs"].ToObject<string>();

                    DataTable excel = _classOFDCM.ToDataTable<BH_HoaDon_Excel>(lstExport);
                    var tempFile = string.Empty;
                    var tempDown = string.Empty;
                    var columnRemove = string.Empty;

                    var nganhnghe = CookieStore.GetCookieAes("shop").ToUpper();
                    switch (loaiHoaDon)
                    {
                        case 1:
                            tempFile = "Teamplate_GiaoDichHoaDon.xlsx";
                            tempDown = "GiaoDichHoaDon.xlsx";
                            if (nganhnghe != "C16EDDA0-F6D0-43E1-A469-844FAB143014")// gara
                            {
                                // remove phieutn, biensoxe, ma, tenbaohiem, bhcantra, bhdatra, thutucoc...
                                columnRemove = "3_4_10_11_25_26_27_28_29_30_31_32_";
                            }
                            if (!string.IsNullOrEmpty(columnsHide))
                            {
                                columnRemove += columnsHide;
                            }

                            excel.Columns.Remove("NgayApDungGoiDV");
                            excel.Columns.Remove("HanSuDungGoiDV");
                            break;
                        case 19:
                            tempFile = "Teamplate_GoiDichVu.xlsx";
                            tempDown = "GoiDichVu.xlsx";
                            break;
                    }

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + tempFile);
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + tempDown);
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    var valExcel1 = string.Empty;
                    if (dayStart == new DateTime(2016, 1, 1))
                    {
                        valExcel1 = "Toàn thời gian";
                    }
                    else
                    {
                        valExcel1 = dayStart + " - " + dayEnd;
                    }
                    _classOFDCM.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel, 6, 30, 24, true, 0, columnsHide, valExcel1, valExcel2);

                    // find index of string "Template"
                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return fileSave;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_HoaDonAPI_XuatFileHD_TongQuan:" + ex.InnerException + ex.Message);
                    return "";
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string ExportExcel_HoaDons(Params_GetListHoaDon listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string fileSave = string.Empty;
                try
                {
                    List<BH_HoaDonDTO> lstAllHDs = new List<BH_HoaDonDTO>();
                    if (listParams.LoaiHoaDon == 19)
                    {
                        lstAllHDs = classhoadon.SP_GetListHoaDons_Where_PassObject(listParams);
                    }
                    else
                    {
                        lstAllHDs = classhoadon.GetListInvoice_Paging(listParams);
                    }
                    List<BH_HoaDon_Excel> lst = new List<BH_HoaDon_Excel>();
                    foreach (var item in lstAllHDs)
                    {
                        BH_HoaDon_Excel DM = new BH_HoaDon_Excel();
                        DM.MaHoaDon = item.MaHoaDon;
                        DM.MaHoaDonGoc = item.MaHoaDonGoc;
                        DM.MaPhieuTiepNhan = item.MaPhieuTiepNhan;
                        DM.BienSo = item.BienSo;
                        DM.NgayLapHoaDon = item.NgayLapHoaDon;
                        DM.NgayApDungGoiDV = item.NgayApDungGoiDV;
                        DM.HanSuDungGoiDV = item.HanSuDungGoiDV;
                        DM.MaDoiTuong = item.MaDoiTuong;
                        DM.TenDoiTuong = item.TenDoiTuong;
                        DM.DienThoai = item.DienThoai;
                        DM.DiaChiKhachHang = item.DiaChiKhachHang;
                        DM.KhuVuc = item.KhuVuc;
                        DM.MaBaoHiem = item.MaBaoHiem;
                        DM.TenBaoHiem = item.TenBaoHiem;
                        DM.TenDonVi = item.TenDonVi;
                        DM.TenNhanVien = item.TenNhanVien;
                        DM.NguoiTao = item.NguoiTaoHD;
                        DM.DienGiai = item.DienGiai;
                        DM.ThanhTienChuaCK = item.ThanhTienChuaCK;
                        DM.GiamGiaCT = item.GiamGiaCT;
                        DM.GiaTriSuDung = item.GiaTriSDDV;
                        DM.TongTienHang = item.TongTienHang;
                        DM.TongGiamGia = item.TongGiamGia + item.KhuyeMai_GiamGia ?? 0;
                        DM.TongTienThue = item.TongTienThue;
                        DM.TongChiPhi = item.TongChiPhi;
                        DM.TongPhaiTra = item.TongThanhToan ?? 0;

                        DM.TongTienBHDuyet = item.TongTienBHDuyet;
                        DM.KhauTruTheoVu = item.KhauTruTheoVu;
                        DM.GiamTruBoiThuong = item.GiamTruBoiThuong;
                        DM.BHThanhToanTruocThue = item.BHThanhToanTruocThue;
                        DM.TongTienThueBaoHiem = item.TongTienThueBaoHiem;

                        DM.PhaiThanhToan = item.PhaiThanhToan;
                        DM.TienMat = item.TienMat;
                        DM.ChuyenKhoan = item.ChuyenKhoan;
                        DM.TienATM = item.TienATM;
                        DM.TienDoiDiem = item.TienDoiDiem;
                        DM.ThuTuCoc = item.TienDatCoc;
                        DM.ThuTuThe = item.ThuTuThe;
                        DM.KhachDaTra = item.KhachDaTra;
                        DM.PhaiThanhToanBaoHiem = item.PhaiThanhToanBaoHiem ?? 0;
                        DM.BaoHiemDaTra = item.BaoHiemDaTra ?? 0;
                        DM.ConNo = item.ConNo ?? 0;
                        DM.TrangThai = item.TrangThai;
                        lst.Add(DM);
                    }

                    DataTable excel = _classOFDCM.ToDataTable<BH_HoaDon_Excel>(lst);
                    var tempFile = string.Empty;
                    var tempDown = string.Empty;
                    var columnRemove = string.Empty;

                    var nganhnghe = CookieStore.GetCookieAes("shop").ToUpper();
                    switch (listParams.LoaiHoaDon)
                    {
                        case 1:
                        case 25:
                            tempFile = "Teamplate_GiaoDichHoaDon.xlsx";
                            tempDown = "GiaoDichHoaDon.xlsx";
                            if (nganhnghe != "C16EDDA0-F6D0-43E1-A469-844FAB143014")// gara
                            {
                                // remove phieutn, biensoxe, ma, tenbaohiem, bhcantra, bhdatra, thutucoc...
                                columnRemove = "3_4_10_11_25_26_27_28_29_30_31_32_";
                            }
                            if (!string.IsNullOrEmpty(listParams.ColumnsHide))
                            {
                                columnRemove += listParams.ColumnsHide;
                            }
                            excel.Columns.Remove("NgayApDungGoiDV");
                            excel.Columns.Remove("HanSuDungGoiDV");
                            break;
                        case 19:
                            tempFile = "Teamplate_GoiDichVu.xlsx";
                            tempDown = "GoiDichVu.xlsx";
                            if (nganhnghe != "C16EDDA0-F6D0-43E1-A469-844FAB143014")// gara
                            {
                                // remove biensoxe
                                columnRemove = "4_";
                            }
                            if (!string.IsNullOrEmpty(listParams.ColumnsHide))
                            {
                                columnRemove += listParams.ColumnsHide;
                            }

                            excel.Columns.Remove("MaHoaDonGoc");
                            excel.Columns.Remove("ThuTuCoc");
                            excel.Columns.Remove("TongChiPhi");
                            excel.Columns.Remove("MaBaoHiem");
                            excel.Columns.Remove("TenBaoHiem");
                            excel.Columns.Remove("PhaiThanhToanBaoHiem");
                            excel.Columns.Remove("BaoHiemDaTra");
                            excel.Columns.Remove("TongTienBHDuyet");
                            excel.Columns.Remove("KhauTruTheoVu");
                            excel.Columns.Remove("GiamTruBoiThuong");
                            excel.Columns.Remove("BHThanhToanTruocThue");
                            excel.Columns.Remove("TongTienThueBaoHiem");
                            excel.Columns.Remove("TongPhaiTra");
                            excel.Columns.Remove("MaPhieuTiepNhan");
                            excel.Columns.Remove("ThanhTienChuaCK");
                            excel.Columns.Remove("GiamGiaCT");
                            excel.Columns.Remove("GiaTriSuDung");
                            break;
                    }
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + tempFile);
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + tempDown);
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    var valExcel1 = string.Empty;
                    if (listParams.NgayTaoHD_TuNgay == new DateTime(2016, 1, 1))
                    {
                        valExcel1 = "Toàn thời gian";
                    }
                    else
                    {
                        valExcel1 = listParams.NgayTaoHD_TuNgay + " - " + listParams.NgayTaoHD_DenNgay;
                    }
                    // xuất excel hoadon: không ẩn cột (assign columnhide = null)
                    _classOFDCM.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel, 6, 30, 24, true, 0, columnRemove, valExcel1, listParams.ValueText);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ExportExcel_HoaDons listParams " + ex.InnerException + ex.Message);
                }
                return fileSave;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel__ChiTietHoaDon(Guid ID_HoaDon, int loaiHoaDon, string columHides)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                SqlParameter param = new SqlParameter("ID_HoaDon", ID_HoaDon);
                List<BH_ChiTietHoaDon_Excel> lst = db.Database.SqlQuery<BH_ChiTietHoaDon_Excel>("EXEC GetChiTietHoaDon_ByIDHoaDon @ID_HoaDon", param).ToList();
                if (lst != null && lst.Count() > 0)
                {
                    DataTable excel = _classOFDCM.ToDataTable<BH_ChiTietHoaDon_Excel>(lst);
                    excel.Columns.Remove("TenHangHoaFull");

                    var tempFile = string.Empty;
                    var tempDown = string.Empty;
                    switch (loaiHoaDon)
                    {
                        case 1:
                        case 25:
                            tempFile = "Teamplate_GiaoDichHoaDon_ChiTiet.xlsx";
                            tempDown = "GiaoDichHoaDon_ChiTiet.xlsx";
                            excel.Columns.Remove("ThanhTien");// get value of colum ThanhToan = ThanhTien - Thue
                            break;
                        case 19:
                            tempFile = "Teamplate_GoiDichVu_ChiTiet.xlsx";
                            tempDown = "GoiDichVu_ChiTiet.xlsx";
                            excel.Columns.Remove("ThanhToan");// get value of colum ThanhToan = ThanhTien - Thue
                            excel.Columns.Remove("TienThue");// get value of colum ThanhToan = ThanhTien - Thue
                            break;
                    }
                    excel.Columns.Remove("GhiChu");
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + tempFile);
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + tempDown);
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, columHides);
                    HttpResponse Response = HttpContext.Current.Response;
                    _classOFDCM.downloadFile(fileSave);
                }
            }
        }
        // trinhpv xuất phiếu trả hàng
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string ExportExcel_PhieuTraHang(Params_GetListHoaDon listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string fileSave = string.Empty;
                try
                {
                    var lstAllHDs = classhoadon.GetListInvoice_Paging(listParams);
                    List<BH_PhieuTraHang_Excel> lst = new List<BH_PhieuTraHang_Excel>();
                    foreach (var item in lstAllHDs)
                    {
                        BH_PhieuTraHang_Excel DM = new BH_PhieuTraHang_Excel();
                        DM.MaHoaDon = item.MaHoaDon;
                        DM.MaHoaDonGoc = item.MaHoaDonGoc;
                        DM.NgayLapHoaDon = item.NgayLapHoaDon;
                        DM.MaDoiTuong = item.MaDoiTuong;
                        DM.TenDoiTuong = item.TenDoiTuong;
                        DM.DienThoai = item.DienThoai;
                        DM.DiaChi = item.DiaChiKhachHang;
                        DM.KhuVuc = item.KhuVuc;
                        DM.ChiNhanh = item.TenDonVi;
                        DM.NguoiTraNhan = item.TenNhanVien;
                        DM.NVLapHoaDon = item.NguoiTaoHD;
                        DM.TongTienHang = item.TongTienHang;
                        DM.GiamGia = item.TongGiamGia;
                        DM.TongSauGiamGia = item.TongTienHang - item.TongGiamGia;
                        DM.PhiTraHang = item.TongChiPhi;
                        DM.TongTienThue = item.TongTienThue;
                        DM.CanTraKhach = item.PhaiThanhToan;
                        DM.DaTraKhach = item.KhachDaTra;
                        DM.ConNo = item.PhaiThanhToan - item.KhachDaTra;
                        DM.GhiChu = item.DienGiai;
                        DM.TrangThai = item.TrangThai;
                        lst.Add(DM);
                    }
                    DataTable excel = _classOFDCM.ToDataTable<BH_PhieuTraHang_Excel>(lst);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuTraHang.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuTraHang.xlsx");
                    fileSave = _classOFDCM.createFolder_Download(fileSave);

                    var valExcel1 = string.Empty;
                    if (listParams.NgayTaoHD_TuNgay == new DateTime(2016, 1, 1))
                    {
                        valExcel1 = "Toàn thời gian";
                    }
                    else
                    {
                        valExcel1 = listParams.NgayTaoHD_TuNgay + " - " + listParams.NgayTaoHD_DenNgay;
                    }
                    _classOFDCM.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel, 6, 30, 24, true, 0, listParams.ColumnsHide, valExcel1, listParams.ValueText);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ExportExcel_PhieuTraHang passObject" + ex.InnerException + ex.Message);
                }
                return fileSave;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel__ChiTietPhieuTraHang(Guid ID_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                SqlParameter param = new SqlParameter("ID_HoaDon", ID_HoaDon);
                List<BH_ChiTietHoaDon_Excel> lstHDCT = db.Database.SqlQuery<BH_ChiTietHoaDon_Excel>("EXEC GetChiTietHoaDon_ByIDHoaDon @ID_HoaDon", param).ToList();
                if (lstHDCT != null && lstHDCT.Count() > 0)
                {
                    DataTable excel = _classOFDCM.ToDataTable<BH_ChiTietHoaDon_Excel>(lstHDCT);
                    excel.Columns.Remove("TenHangHoa");
                    excel.Columns.Remove("ThanhTien");// use cloumn ThanhToan

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuTraHang_ChiTiet.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuTraHang_ChiTiet.xlsx");
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, null);
                    HttpResponse Response = HttpContext.Current.Response;
                    _classOFDCM.downloadFile(fileSave);
                }
            }
        }
        // trinhpv xuất phiếu nhập hàng
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string ExportExcel_PhieuNhapHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                ModelHoaDon model = data["objExcel"].ToObject<ModelHoaDon>();
                List<BH_HoaDonDTO> lstAllHDs = classhoadon.GetList_HoaDonNhapHang(model);
                List<BH_PhieuNhapHang_Excel> lst = new List<BH_PhieuNhapHang_Excel>();
                foreach (var item in lstAllHDs)
                {
                    BH_PhieuNhapHang_Excel DM = new BH_PhieuNhapHang_Excel();
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.MaDoiTuong = item.MaDoiTuong;
                    DM.TenDoiTuong = item.TenDoiTuong ?? "Nhà cung cấp lẻ";
                    DM.SoDienThoai = item.DienThoai;
                    DM.DiaChi = item.DiaChiKhachHang;
                    DM.NguoiBan = item.TenNhanVien;
                    DM.TenDonVi = item.TenDonVi;
                    DM.TongTienHang = item.TongTienHang;
                    DM.TongTienThue = item.TongTienThue;
                    DM.TongGiamGia = item.TongGiamGia;
                    DM.PhaiThanhToan = item.PhaiThanhToan;
                    DM.ThanhTienChuaCK = item.ThanhTienChuaCK;
                    DM.TongChietKhau = item.GiamGiaCT;
                    DM.TienMat = item.TienMat;
                    DM.TienMat = item.TienMat;
                    DM.TienPOS = item.TienATM;
                    DM.ChuyenKhoan = item.ChuyenKhoan;
                    DM.TienDatCoc = item.TienDatCoc;
                    DM.DaThanhToan = item.KhachDaTra;
                    DM.ConNo = item.ConNo;
                    DM.DienGiai = item.DienGiai;
                    DM.TrangThai = item.ChoThanhToan == null ? "Đã hủy" : (item.ChoThanhToan == false ? "Đã nhập hàng" : "Tạm lưu");
                    lst.Add(DM);
                }
                var tencn = "";
                if (model.tenchinhanh != null)
                {
                    for (int i = 0; i < model.tenchinhanh.Count(); i++)
                    {
                        if (tencn == "")
                        {
                            tencn = model.tenchinhanh[i];
                        }
                        else
                        {
                            tencn = tencn + "," + model.tenchinhanh[i];
                        }
                    }
                }
                DataTable excel = _classOFDCM.ToDataTable<BH_PhieuNhapHang_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuNhapHang.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuNhapHang.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, model.columnsHide, model.time, tencn);
                HttpResponse Response = HttpContext.Current.Response;
                fileSave = _classOFDCM.createFolder_Export("~/Template/ExportExcel/PhieuNhapHang.xlsx");
                return fileSave;
            }
        }

        [AcceptVerbs("GET")]
        public void ExportExcel__ChiTietPhieuNhapHang(Guid ID_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                BH_HoaDon bhhd = classhoadon.Get(p => p.ID == ID_HoaDon);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                List<BH_ChiTietHoaDon_Excel> lst = db.Database.SqlQuery<BH_ChiTietHoaDon_Excel>("EXEC GetListChiTietHoaDonXuatFile @ID_HoaDon", paramlist.ToArray()).ToList();

                DataTable excel = _classOFDCM.ToDataTable<BH_ChiTietHoaDon_Excel>(lst);
                excel.Columns.Remove("TenHangHoaFull");
                excel.Columns.Remove("ThanhTien");
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuNhapHang_ChiTiet.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuNhapHang_ChiTiet.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, null);
                _classOFDCM.listToOfficeExcelNhapHang(fileSave, fileSave, bhhd.TongGiamGia, lst.Count() + 5, 7);

                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }
        // trinhpv xuất phiếu nhập hàng
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string ExportExcel_PhieuTraHangNhap([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                ModelHoaDon model = data["objExcel"].ToObject<ModelHoaDon>();
                var lstAllHDs = classhoadon.GetList_HoaDonNhapHang(model);
                List<BH_PhieuTraHangNhap_Excel> lst = new List<BH_PhieuTraHangNhap_Excel>();
                foreach (var item in lstAllHDs)
                {
                    BH_PhieuTraHangNhap_Excel DM = new BH_PhieuTraHangNhap_Excel();
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.MaDoiTuong = item.MaDoiTuong;
                    DM.TenDoiTuong = item.TenDoiTuong ?? "Nhà cung cấp lẻ";
                    DM.SoDienThoai = item.DienThoai;
                    DM.DiaChi = item.DiaChiKhachHang;
                    DM.NguoiBan = item.TenNhanVien;
                    DM.TenDonVi = item.TenDonVi;
                    DM.TongTienHang = item.TongTienHang;
                    DM.TongGiamGia = item.TongGiamGia;
                    DM.TongTienThue = item.TongTienThue;
                    DM.KhachCanTra = item.PhaiThanhToan;
                    DM.KhachDaTra = item.KhachDaTra;
                    DM.ConNo = item.PhaiThanhToan - item.KhachDaTra;
                    DM.GhiChu = item.DienGiai;
                    DM.TrangThai = item.ChoThanhToan == null ? "Đã hủy" : (item.ChoThanhToan == false ? "Đã trả hàng" : "Tạm lưu");
                    lst.Add(DM);
                }
                var tencn = "";
                if (model.tenchinhanh != null)
                {
                    for (int i = 0; i < model.tenchinhanh.Count(); i++)
                    {
                        if (tencn == "")
                        {
                            tencn = model.tenchinhanh[i];
                        }
                        else
                        {
                            tencn = tencn + "," + model.tenchinhanh[i];
                        }
                    }
                }
                DataTable excel = _classOFDCM.ToDataTable<BH_PhieuTraHangNhap_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuTraHangNhap.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuTraHangNhap.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, model.columnsHide, model.time, tencn);

                HttpResponse Response = HttpContext.Current.Response;
                fileSave = _classOFDCM.createFolder_Export("~/Template/ExportExcel/PhieuTraHangNhap.xlsx");
                return fileSave;
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel__ChiTietPhieuTraHangNhap(Guid ID_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                BH_HoaDon bhhd = classhoadon.Get(p => p.ID == ID_HoaDon);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_HoaDon", ID_HoaDon));
                List<BH_ChiTietHoaDon_Excel> lst = db.Database.SqlQuery<BH_ChiTietHoaDon_Excel>("EXEC GetListChiTietHoaDonXuatFile @ID_HoaDon", paramlist.ToArray()).ToList();

                DataTable excel = _classOFDCM.ToDataTable<BH_ChiTietHoaDon_Excel>(lst);
                excel.Columns.Remove("TenHangHoaFull");
                excel.Columns.Remove("ThanhTien");
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_PhieuTraHangNhap_ChiTiet.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuTraHangNhap_ChiTiet.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, null);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }
        // nhuongdt phiếu đặt hàng
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string ExportExcel_DatHang(Params_GetListHoaDon listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string fileSave = string.Empty;
                try
                {
                    var nganhnghe = CookieStore.GetCookieAes("shop").ToUpper();
                    var isGara = false;
                    if (nganhnghe == "C16EDDA0-F6D0-43E1-A469-844FAB143014")
                    {
                        isGara = true;
                    }
                    var lstAllHDs = classhoadon.GetListInvoice_Paging(listParams);
                    List<BH_PhieuDatHang_Excel> lst = new List<BH_PhieuDatHang_Excel>();
                    foreach (var item in lstAllHDs)
                    {
                        BH_PhieuDatHang_Excel DM = new BH_PhieuDatHang_Excel();
                        DM.MaHoaDon = item.MaHoaDon;
                        DM.NgayLapHoaDon = item.NgayLapHoaDon;
                        DM.MaPhieuTiepNhan = item.MaPhieuTiepNhan;
                        DM.BienSoXe = item.BienSo;
                        DM.MaKhachHang = item.MaDoiTuong;
                        DM.TenDoiTuong = item.TenDoiTuong ?? "Khách lẻ";
                        DM.DienThoaiKH = item.DienThoai;
                        DM.DiaChiKH = item.DiaChiKhachHang;
                        DM.KhuVuc = item.KhuVuc;
                        DM.TenChiNhanh = item.TenDonVi;
                        DM.NguoiBan = item.TenNhanVien;
                        DM.NguoiTao = item.NguoiTaoHD;
                        DM.TongTienHang = item.TongTienHang;
                        DM.TongTienThue = item.TongTienThue;
                        DM.GiamGia = item.TongGiamGia;
                        DM.TongChiPhi = item.TongChiPhi;
                        DM.KhachCanTra = item.PhaiThanhToan;
                        DM.KhachDaTra = item.KhachDaTra;
                        DM.GhiChu = item.DienGiai;
                        DM.TrangThai = isGara ? item.Gara_TrangThaiBG : item.TrangThai;
                        lst.Add(DM);
                    }
                    var columnRemove = string.Empty;
                    DataTable excel = _classOFDCM.ToDataTable<BH_PhieuDatHang_Excel>(lst);
                    if (!isGara)
                    {
                        columnRemove = "2_3_";
                    }
                    if (!string.IsNullOrEmpty(listParams.ColumnsHide))
                    {
                        columnRemove += listParams.ColumnsHide;
                    }

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + "Teamplate_HoaDonDatHang.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + "HoaDonDatHang.xlsx");
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    var valExcel1 = string.Empty;
                    if (listParams.NgayTaoHD_TuNgay == new DateTime(2016, 1, 1))
                    {
                        valExcel1 = "Toàn thời gian";
                    }
                    else
                    {
                        valExcel1 = listParams.NgayTaoHD_TuNgay + " - " + listParams.NgayTaoHD_DenNgay;
                    }
                    _classOFDCM.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel, 6, 30, 24, true, 0, columnRemove, valExcel1, listParams.ValueText);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ExportExcel_DatHang listParams " + ex.InnerException + ex.Message);
                }
                return fileSave;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExcel_ChiTietPhieuDatHang(Guid ID_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                SqlParameter param = new SqlParameter("ID_HoaDon", ID_HoaDon);
                List<BH_ChiTietHoaDon_Excel> lst = db.Database.SqlQuery<BH_ChiTietHoaDon_Excel>("EXEC GetChiTietHoaDon_ByIDHoaDon @ID_HoaDon", param).ToList();
                if (lst != null && lst.Count() > 0)
                {
                    DataTable excel = _classOFDCM.ToDataTable<BH_ChiTietHoaDon_Excel>(lst);
                    excel.Columns.Remove("TenHangHoa");
                    excel.Columns.Remove("ThanhTien");
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_HoaDonDatHang_ChiTiet.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuDatHang_ChiTiet.xlsx");
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, null);
                    HttpResponse Response = HttpContext.Current.Response;
                    _classOFDCM.downloadFile(fileSave);
                }
            }
        }
        #endregion

        //Load all CT thao tác chi tiết phiếu
        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDon(Guid idHoaDon, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lst = classhoadon.GetChiTietHD_byIDHoaDonLT(idHoaDon, iddonvi);
                return lst;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<BH_HoaDon_ChiTietDTO> SP_GetChiTietHD_byIDHoaDon(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lst = classhoadon.SP_GetChiTietHD_byIDHoaDon(idHoaDon);
                return lst;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<BH_HoaDon_ChiTietDTO> SP_GetChiTietHD_byIDHoaDon_ChietKhauNV(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                List<BH_HoaDon_ChiTietDTO> lst = classhoadon.SP_GetChiTietHD_byIDHoaDon_ChietKhauNV(idHoaDon);
                return lst;
            }
        }
        [HttpGet]
        public IHttpActionResult GetListCombo_ofCTHD(Guid idHoaDon, string idCTHD = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    List<BH_HoaDon_ChiTietDTO> lst = classhoadon.GetListComBo_ofCTHD(idHoaDon, idCTHD);
                    return ActionTrueData(lst);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GetNVThucHienDichVu(Guid idChiTiet)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    List<BH_NhanVienThucHienDTO> lst = classhoadon.GetNVThucHienDichVu(idChiTiet);
                    return ActionTrueData(lst);
                }
                catch (Exception e)
                {
                    return ActionFalseNotData(e.ToString());
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetChietKhauNV_byIDHoaDon(Guid idHoaDon, Guid? idPhieuThu = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);
                try
                {
                    // if idPhieuThu = null: get all ck
                    // else only get ck thucthu
                    var lst = (from nvth in db.BH_NhanVienThucHien
                               join nv in db.NS_NhanVien on nvth.ID_NhanVien equals nv.ID
                               where nvth.ID_HoaDon == idHoaDon
                               && (idPhieuThu == null || (nvth.TinhChietKhauTheo == 1 && nvth.ID_QuyHoaDon == idPhieuThu))
                               group new { nvth, nv } by new { nvth.ID_HoaDon, nvth.ID_NhanVien, nvth.ThucHien_TuVan, nvth.TheoYeuCau, nvth.TinhChietKhauTheo, nv.TenNhanVien } into g
                               select new
                               {
                                   g.Key.ID_HoaDon,
                                   g.Key.ID_NhanVien,
                                   g.Key.TenNhanVien,
                                   g.Key.ThucHien_TuVan,
                                   g.Key.TheoYeuCau,
                                   g.Key.TinhChietKhauTheo,
                                   HeSo = g.Max(x => x.nvth.HeSo),
                                   PT_ChietKhau = g.Max(x => x.nvth.PT_ChietKhau),
                                   TienChietKhau = g.Max(x => x.nvth.TienChietKhau),
                               }).ToList();

                    return Json(new { res = true, data = lst });
                }
                catch (Exception ex)
                {
                    return Json(new { res = false, mes = "GetChietKhauNV_byIDHoaDon " + ex.InnerException + ex.Message });
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListHDbyIDs(List<string> lstID)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    List<BH_HoaDonDTO> lstHD = new List<BH_HoaDonDTO>();
                    if (lstID != null && lstID.Count > 0)
                    {
                        var sIDs = string.Join(",", lstID);
                        lstHD = classhoadon.GetListHDbyIDs(sIDs);
                        List<BH_HoaDon_ChiTietDTO> lstCTHD = classhoadon.SP_GetChiTietHD_MultipleHoaDon(sIDs);
                        return Json(new { res = true, lstHD, lstCTHD });
                    }
                    return Json(new { res = false });
                }
                catch (Exception)
                {
                    return Json(new { res = false });
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_MultipleHoaDon(string arrID_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lst = classhoadon.SP_GetChiTietHD_MultipleHoaDon(arrID_HoaDon);
                return lst;
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonKiemKho(Guid idHoaDon, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                List<BH_HoaDon_ChiTietDTO> lst = classhoadon.GetChiTietHD_byIDHoaDonLT(idHoaDon, iddonvi);
                return lst;
            }
        }
        //phân trang chi tiết phiếu nhập, trả
        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonLoadChiTiet(Guid idHoaDon, int currentpage, int pageSize, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lst = classhoadon.GetChiTietHD_byIDHoaDonLT(idHoaDon, iddonvi);
                return lst.Skip(currentpage * pageSize).Take(pageSize).ToList();
            }
        }

        public PageListDTO GetPageCountCTGiaoDich_Where(float pageSize, Guid idHoaDon, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var totalRecords = 0;
                var lst = classhoadon.GetChiTietHD_byIDHoaDonLT(idHoaDon, iddonvi);
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

        //GetallChiTiet để thao tác phiếu chuyển
        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonChuyenHang(Guid idHoaDon, int currentpage, int pageSize, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lst = classhoadon.GetChiTietHD_byIDHoaDonLT(idHoaDon, iddonvi);
                return lst.Skip(currentpage * pageSize).Take(pageSize).ToList();
            }
        }

        //phân trang chi tiết phiếu chuyển
        public List<BH_HoaDon_ChiTietDTO> GetChiTietHD_byIDHoaDonChuyenHangThaoTac(Guid idHoaDon, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lst = classhoadon.GetChiTietHD_byIDHoaDonLT(idHoaDon, iddonvi);
                return lst;
            }
        }

        public PageListDTO GetPageCountCTCH_Where(float pageSize, Guid idHoaDon, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var totalRecords = 0;
                var lst = classhoadon.GetChiTietHD_byIDHoaDonLT(idHoaDon, iddonvi);
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
        [HttpPost, HttpGet]
        public IHttpActionResult GetListInvoice_Paging(Params_GetListHoaDon param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);

                try
                {
                    //db.Database.CommandTimeout = 60 * 60;
                    List<BH_HoaDonDTO> data = classhoadon.GetListInvoice_Paging(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult GetAllHoaDons_Where_PassObject(Params_GetListHoaDon listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                try
                {
                    List<BH_HoaDonDTO> lstAllHDs = classhoadon.SP_GetListHoaDons_Where_PassObject(listParams);

                    List<BH_HoaDonDTO> lstReturns = new List<BH_HoaDonDTO>();
                    if (lstAllHDs != null)
                    {
                        int totalRecords = lstAllHDs.Count();
                        double tongGiamGia = lstAllHDs.Sum(x => x.TongGiamGia);
                        double tongGiamGiaKM = lstAllHDs.Sum(x => x.KhuyeMai_GiamGia ?? 0);
                        double khachDaTra = lstAllHDs.Sum(x => (double?)x.KhachDaTra ?? 0);
                        double tongTienHang = lstAllHDs.Sum(x => x.TongTienHang);
                        double tongChiPhi = lstAllHDs.Sum(x => x.TongChiPhi);
                        double tongPhaiTraKhach = 0;
                        double tongHDoiTra = lstAllHDs.Sum(x => (double?)x.TongTienHDDoiTra ?? 0);
                        double tongPhaiThanhToan = lstAllHDs.Sum(x => x.PhaiThanhToan);
                        double tienDoiDiem = lstAllHDs.Sum(x => x.TienDoiDiem ?? 0);
                        double thuTuThe = lstAllHDs.Sum(x => x.ThuTuThe ?? 0);
                        double thanhtienct = lstAllHDs.Sum(x => x.ThanhTienChuaCK ?? 0);
                        double giamgiact = lstAllHDs.Sum(x => x.GiamGiaCT ?? 0);
                        double tongthue = lstAllHDs.Sum(x => x.TongTienThue);
                        double khachCanTra = 0;
                        double khachConNo = 0;

                        khachCanTra = tongPhaiThanhToan;
                        khachConNo = khachCanTra - khachDaTra;
                        tongPhaiTraKhach = tongPhaiThanhToan;

                        return Json(new
                        {
                            lstCH = lstAllHDs,
                            Rowcount = totalRecords,
                            pageCount = System.Math.Ceiling(totalRecords * 1.0 / listParams.PageSize),
                            TongTienHang = tongTienHang,
                            TongChiPhi = tongChiPhi,
                            TongTienThue = tongthue,
                            TongGiamGia = tongGiamGia,
                            TongGiamGiaKM = tongGiamGiaKM,
                            TongKhachTra = khachDaTra,
                            TongThanhToan = khachCanTra,
                            TongKhachNo = khachConNo,
                            TongPhaiTraKhach = tongPhaiTraKhach,
                            TienDoiDiem = tienDoiDiem,
                            ThuTuThe = thuTuThe,
                            ThanhTienChuaCK = thanhtienct,
                            GiamGiaCT = giamgiact,
                        });
                    }
                    else
                    {
                        //(don't return null, because JsonResult: IHttpActionResult cant't return null (error) 
                        return Json(new JsonResultExampleCH());
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_HoaDonAPI_GetAllHoaDons_Where_PassObject: " + ex.Message + ex.InnerException);
                    return Json(new JsonResultExampleCH());
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListTheNap(ModelHoaDonTheNap model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    List<BH_HoaDonTheNapDTO> dataxx = classHoaDon.LoadDanhMucTheGiaTri(model);
                    return Json(new
                    {
                        res = true,
                        lst = dataxx,
                    });
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        res = false,
                        mes = e,
                    });
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult Comapre_DoanhThuByTheGiaTri(ModelHoaDonTheNap model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    if (db == null)
                    {
                        return Json(new { res = false, mes = "DB null" });
                    }
                    else
                    {
                        List<SqlParameter> lstParam = new List<SqlParameter>();
                        lstParam.Add(new SqlParameter("ID_ChiNhanhs", model.arrChiNhanh.ToString()));
                        lstParam.Add(new SqlParameter("FromDate", model.dayStart));
                        lstParam.Add(new SqlParameter("ToDate", model.dayEnd));
                        var tbl = db.Database.SqlQuery<SP_CompareDoanhThuThe>("exec Comapre_DoanhThuByTheGiaTri @ID_ChiNhanhs, @FromDate, @ToDate", lstParam.ToArray()).ToList();
                        if (tbl != null)
                        {
                            return Json(new { res = true, data = tbl });
                        }
                        else
                        {
                            return Json(new { res = false, mes = "Data null" });
                        }
                    }
                }
                catch (Exception e)
                {
                    var err = string.Concat("Comapre_DoanhThuByTheGiaTri ", e.Message, e.InnerException);
                    return Json(new { res = false, mes = err });
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetChietKhauNhanVienTheNap(Guid idnv)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var tbl = (from cknv in db.ChietKhauMacDinh_HoaDon
                               join cknv_ct in db.ChietKhauMacDinh_HoaDon_ChiTiet on cknv.ID equals cknv_ct.ID_ChietKhauHoaDon
                               where cknv_ct.ID_NhanVien == idnv && cknv.ChungTuApDung.Contains("22")
                               select new
                               {
                                   TinhChietKhauTheo = cknv.TinhChietKhauTheo,
                                   GiaTriChietKhau = cknv.GiaTriChietKhau
                               }).ToList();
                    if (tbl != null)
                    {
                        return Json(new { data = tbl });
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListNhatKySDThe(ModelNhatKySDThe model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    List<BH_NhatKySDTheDTO> lstAllHDs = classhoadon.GetListNhatKySDThe(model);
                    if (lstAllHDs.Count() > 0)
                    {
                        return Json(new
                        {
                            res = true,
                            lst = lstAllHDs,
                            Rowcount = lstAllHDs[0].TotalRow,
                            pageCount = lstAllHDs[0].TotalPage,
                            TongTienTang = lstAllHDs[0].TongTienTang,
                            TongTienGiam = lstAllHDs[0].TongTienGiam
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            res = true,
                            lst = lstAllHDs,
                            Rowcount = 0,
                            pageCount = 0,
                            TongTienTang = 0,
                            TongTienGiam = 0
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        res = false,
                        mes = ex,
                    });
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public JsonResult<JsonResultExampleLsNapThe> GetListLichSuNT(ModelLichSuNapThe model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var tbl = from hd in db.BH_HoaDon
                              join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                              join dv in db.DM_DonVi on hd.ID_DonVi equals dv.ID
                              where (hd.LoaiHoaDon == 22 || hd.LoaiHoaDon == 23) && hd.ID_DoiTuong == model.iddt && hd.ChoThanhToan != null
                              orderby hd.NgayLapHoaDon descending
                              select new BH_HoaDonTheNapDTO
                              {
                                  ID = hd.ID,
                                  MaHoaDon = hd.MaHoaDon,
                                  LoaiHoaDon = hd.LoaiHoaDon,
                                  NgayLapHoaDon = hd.NgayLapHoaDon,
                                  TenDonVi = dv.TenDonVi,
                                  TenKhachHang = dt.TenDoiTuong,
                                  NguoiTao = hd.NguoiTao,
                                  SoDienThoai = dt.DienThoai,
                                  MucNap = hd.TongChiPhi,
                                  KhuyenMaiVND = hd.TongChietKhau,
                                  KhuyenMaiPT = (hd.TongChietKhau / hd.TongChiPhi) * 100,
                                  TongTienNap = hd.TongTienHang,
                                  SoDuSauNap = hd.TongTienThue,
                                  ChietKhauVND = hd.TongGiamGia,
                                  ChietKhauPT = (hd.TongGiamGia / hd.TongTienHang) * 100,
                                  PhaiThanhToan = hd.PhaiThanhToan,
                                  KhachDaTra = hd.PhaiThanhToan,
                                  GhiChu = hd.DienGiai,
                                  ChoThanhToan = hd.ChoThanhToan
                              };

                    List<BH_HoaDonTheNapDTO> lstReturn = new List<BH_HoaDonTheNapDTO>();
                    foreach (var item in tbl)
                    {
                        Quy_HoaDon_ChiTiet qct = db.Quy_HoaDon_ChiTiet.Where(p => p.ID_HoaDonLienQuan == item.ID).FirstOrDefault();
                        BH_HoaDonTheNapDTO objTN = new BH_HoaDonTheNapDTO();
                        objTN.ID = item.ID;
                        objTN.IDThuChi = qct == null ? (Guid?)null : qct.ID_HoaDon;
                        objTN.MaHoaDon = item.MaHoaDon;
                        objTN.NgayLapHoaDon = item.NgayLapHoaDon;
                        objTN.TenKhachHang = item.TenKhachHang;
                        objTN.TenDonVi = item.TenDonVi;
                        objTN.NguoiTao = item.NguoiTao;
                        objTN.SoDienThoai = item.SoDienThoai;
                        objTN.MucNap = item.MucNap;
                        objTN.KhuyenMaiVND = item.KhuyenMaiVND;
                        objTN.KhuyenMaiPT = Math.Round((item.KhuyenMaiVND / item.MucNap) * 100, MidpointRounding.ToEven);
                        objTN.TongTienNap = item.TongTienNap;
                        objTN.SoDuSauNap = item.SoDuSauNap;
                        objTN.ThanhTien = item.MucNap;
                        objTN.ChietKhauVND = item.ChietKhauVND;
                        objTN.ChietKhauPT = Math.Round((item.ChietKhauVND / item.MucNap) * 100, MidpointRounding.ToEven);
                        objTN.PhaiThanhToan = item.PhaiThanhToan;
                        objTN.KhachDaTra = item.PhaiThanhToan;
                        objTN.MaPhieuThu = qct != null ? db.Quy_HoaDon.Where(p => p.ID == qct.ID_HoaDon).FirstOrDefault().MaHoaDon : "";
                        objTN.ChoThanhToan = item.ChoThanhToan;
                        objTN.GhiChu = item.GhiChu;
                        lstReturn.Add(objTN);
                    }
                    if (model.loai == 1)
                    {
                        lstReturn = lstReturn.Take(10).ToList();
                    }

                    if (model.loai == 2)
                    {
                        DateTime loai2 = DateTime.Now.AddDays(-30);
                        lstReturn = lstReturn.Where(p => p.NgayLapHoaDon > loai2).ToList();
                    }

                    if (model.loai == 3)
                    {
                        if (model.dayStart == model.dayEnd)
                        {
                            lstReturn = lstReturn.Where(hd => hd.NgayLapHoaDon.Year == model.dayStart.Year
                            && hd.NgayLapHoaDon.Month == model.dayStart.Month
                            && hd.NgayLapHoaDon.Day == model.dayStart.Day).ToList();
                        }
                        else
                        {
                            lstReturn = lstReturn.Where(hd => hd.NgayLapHoaDon >= model.dayStart && hd.NgayLapHoaDon < model.dayEnd).ToList();
                        }
                    }
                    var totalRecords = lstReturn.Count();
                    lstReturn = lstReturn.OrderByDescending(p => p.NgayLapHoaDon).Skip(model.currentPage * model.pageSize).Take(model.pageSize).ToList();

                    JsonResultExampleLsNapThe json = new JsonResultExampleLsNapThe
                    {
                        lst = lstReturn,
                        Rowcount = totalRecords,
                        pageCount = System.Math.Ceiling(totalRecords * 1.0 / model.pageSize),
                    };
                    return Json(json);
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetHisChargeValueCard(ModelLichSuNapThe model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                        var data = classHoaDon.GetHisChargeValueCard(model);
                        if (data.Count() > 0)
                        {
                            return Json(new
                            {
                                res = true,
                                lst = data,
                                Rowcount = data[0].TotalRow,
                                pageCount = data[0].TotalPage,
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                res = true,
                                lst = data,
                                Rowcount = 0,
                                pageCount = 0,
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        return Json(new
                        {
                            res = false,
                            mes = e,
                        });
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string XuatFileThenNap(ModelHoaDonTheNap model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var fileSave = string.Empty;
                try
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                    List<BH_HoaDonTheNapDTOXuatFile> lstReturn = new List<BH_HoaDonTheNapDTOXuatFile>();

                    List<BH_HoaDonTheNapDTO> dataxx = classHoaDon.LoadDanhMucTheGiaTri(model);
                    foreach (var item in dataxx)
                    {
                        BH_HoaDonTheNapDTOXuatFile objTN = new BH_HoaDonTheNapDTOXuatFile();
                        objTN.MaHoaDon = item.MaHoaDon;
                        objTN.NgayLapHoaDon = item.NgayLapHoaDon;
                        objTN.MaKhachHang = item.MaKhachHang;
                        objTN.TenKhachHang = item.TenKhachHang;
                        objTN.SoDienThoai = item.SoDienThoai;
                        objTN.MucNap = item.MucNap;
                        objTN.KhuyenMaiVND = item.KhuyenMaiVND;
                        objTN.TongTienNap = item.TongTienNap;
                        objTN.SoDuSauNap = item.SoDuSauNap;
                        objTN.ChietKhauVND = item.ChietKhauVND;
                        objTN.PhaiThanhToan = item.PhaiThanhToan;
                        objTN.TienMat = item.TienMat ?? 0;
                        objTN.TienATM = item.TienATM ?? 0;
                        objTN.TienGui = item.TienGui ?? 0;
                        objTN.KhachDaTra = item.KhachDaTra;
                        objTN.MaNhanVienThucHien = item.NhanVienThucHien;
                        objTN.GhiChu = item.GhiChu;
                        objTN.TrangThai = item.ChoThanhToan == false ? "Hoàn thành" : "Đã hủy";
                        lstReturn.Add(objTN);
                    }
                    DataTable excel = _classOFDCM.ToDataTable<BH_HoaDonTheNapDTOXuatFile>(lstReturn);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_TheNap.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuNapThe.xlsx");
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    _classOFDCM.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 28, 24, true, model.columnsHide, model.time, "");
                    HttpResponse Response = HttpContext.Current.Response;
                    fileSave = _classOFDCM.createFolder_Export("~/Template/ExportExcel/PhieuNapThe.xlsx");
                }
                catch (Exception e)
                {
                    fileSave = e.InnerException + e.Message;
                }
                return fileSave;
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetList_HoaDonNhapHang(ModelHoaDon model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                try
                {
                    List<BH_HoaDonDTO> data = classhoadon.GetList_HoaDonNhapHang(model);
                    return ActionTrueData(data);
                }
                catch (Exception e)
                {
                    return ActionFalseNotData(e.InnerException + e.Message);
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public List<BH_HoaDon> GetAllHoaDonTraHangg()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lstAllTraHang = classhoadon.Gets(hd => hd.LoaiHoaDon == 6);
                if (lstAllTraHang != null)
                {
                    List<BH_HoaDon> lstGroup = new List<BH_HoaDon>();
                    foreach (BH_HoaDon item in lstAllTraHang)
                    {
                        if (lstGroup.Any(hd => hd.ID_HoaDon == item.ID_HoaDon) == false)
                        {
                            lstGroup.Add(item);
                        }
                    }

                    List<BH_HoaDon_ChiTiet> lstHDByID = new List<BH_HoaDon_ChiTiet>();
                    try
                    {
                        foreach (BH_HoaDon item in lstGroup)
                        {
                            lstHDByID = classhoadon.GroupjoinHD_CTHD(item.ID_HoaDon ?? Guid.Empty);

                            item.BH_HoaDon_ChiTiet = lstHDByID;
                        }
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("BH_HoaDonAPI_GetAllHoaDonTraHangg: " + ex.Message + ex.InnerException);
                    }
                    return lstGroup;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<BH_HoaDonDTO> GetListHoaDonsTraHang_NotWhere(int currentPage, int pageSize, int loaiHoaDon)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                    ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                    ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                    var lstAllHDs = classhoadon.GetListHoaDons_QuyHD(loaiHoaDon).Where(hd => hd.LoaiHoaDon != 6).OrderByDescending(hd => hd.NgayLapHoaDon);
                    var lstAllHDTraHang = classhoadon.Gets(hd => hd.LoaiHoaDon == 6);
                    List<BH_HoaDonDTO> lstReturns = new List<BH_HoaDonDTO>();

                    if (lstAllHDs != null)
                    {
                        try
                        {
                            foreach (BH_HoaDonDTO itemAll in lstAllHDs)
                            {
                                double khachTra = 0;
                                Quy_HoaDon_ChiTiet quyHD_CT = _classQHDCT.Gets(idHD => idHD.ID_HoaDonLienQuan == itemAll.ID).FirstOrDefault();
                                if (quyHD_CT != null)
                                {
                                    khachTra = quyHD_CT.TienGui + quyHD_CT.TienMat;
                                }
                                BH_HoaDonDTO itemData = new BH_HoaDonDTO();
                                itemData.ID = itemAll.ID;
                                itemData.ID_HoaDon = itemAll.ID_HoaDon;
                                itemData.MaHoaDon = itemAll.MaHoaDon;
                                itemData.TenNhanVien = itemAll.TenNhanVien;
                                itemData.TenDonVi = itemAll.TenDonVi;
                                itemData.NgayLapHoaDon = itemAll.NgayLapHoaDon;
                                itemData.TongGiamGia = itemAll.TongGiamGia;
                                itemData.TongTienHang = itemAll.TongTienHang;
                                itemData.TongChiPhi = itemAll.TongChiPhi;
                                itemData.PhaiThanhToan = itemAll.PhaiThanhToan;
                                itemData.TenDoiTuong = itemAll.TenDoiTuong ?? "Khách lẻ";
                                itemData.DienGiai = itemAll.DienGiai;
                                itemData.Email = itemAll.Email;
                                itemData.DienThoai = itemAll.DienThoai;
                                itemData.TenPhongBan = itemAll.TenPhongBan;
                                itemData.KhachDaTra = khachTra;
                                itemData.ID_NhanVien = itemAll.ID_NhanVien;
                                itemData.NguoiTaoHD = itemAll.NguoiTaoHD;
                                itemData.TenBangGia = itemAll.TenBangGia;
                                itemData.ChoThanhToan = itemAll.ChoThanhToan;
                                itemData.YeuCau = itemAll.YeuCau;
                                itemData.ID_DoiTuong = itemAll.ID_DoiTuong;
                                itemData.ID_BangGia = itemAll.ID_BangGia;
                                itemData.TrangThai = itemAll.ChoThanhToan == null ? "Đã hủy" : (itemAll.ChoThanhToan == false ? "Hoàn thành" : "Tạm lưu");
                                itemData.BH_HoaDon_ChiTiet = classhoadonchitiet.Gets(ct => ct.ID_HoaDon == itemAll.ID)
                                    .Select(x => new
                                    {
                                        ID = x.ID,
                                        ID_HoaDon = itemAll.ID,
                                        DonGia = x.DonGia,
                                        GiaVon = x.GiaVon,
                                        SoLuong = x.SoLuong,
                                        ThanhTien = x.ThanhTien,
                                        ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                        ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                                        MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                                        GiamGia = x.TienChietKhau,
                                        ThoiGian = x.ThoiGian,
                                        GhiChu = x.GhiChu
                                    }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                                    {

                                        ID = c.ID,
                                        ID_HoaDon = c.ID_HoaDon,
                                        DonGia = c.DonGia,
                                        GiaVon = c.GiaVon,
                                        SoLuong = c.SoLuong,
                                        ThanhTien = c.ThanhTien,
                                        ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                                        MaHangHoa = c.MaHangHoa,
                                        TenHangHoa = _classDMHH.Select_HangHoa(c.ID_HangHoa).TenHangHoa,
                                        GiamGia = c.GiamGia,
                                        ThoiGian = c.ThoiGian,
                                        GhiChu = c.GhiChu
                                    }).ToList();

                                List<BH_HoaDon> lstbhhoadontra = lstAllHDTraHang.Where(p => p.ID_HoaDon == itemAll.ID).ToList();

                                if (lstbhhoadontra.Count > 0)
                                {
                                    double soluongtra = 0;
                                    foreach (var itemtra in lstbhhoadontra)
                                    {
                                        soluongtra = soluongtra + itemtra.BH_HoaDon_ChiTiet.Sum(p => p.SoLuong);
                                    }
                                    if (itemData.BH_HoaDon_ChiTiet.Sum(p => p.SoLuong) <= soluongtra)
                                    {
                                        continue;
                                    }

                                    // 1. collection TH, 2. key of collection All (itemall - outerkey), 3. key of collection TH (inner key), 
                                    List<BH_HoaDon_ChiTietDTO> lstChiTietTemp = new List<BH_HoaDon_ChiTietDTO>();
                                    List<BH_HoaDon_ChiTiet> lsthdchitiet = new List<BH_HoaDon_ChiTiet>();
                                    foreach (var item in lstbhhoadontra)
                                    {
                                        lsthdchitiet = lsthdchitiet.Union(item.BH_HoaDon_ChiTiet).ToList();
                                    }
                                    var chitiet = lsthdchitiet.GroupBy(p => new { ID_DonViQuiDoi = p.ID_DonViQuiDoi, DonGia = p.DonGia }).Select(p => new
                                    {
                                        ID_DonViQuiDoi = p.Key.ID_DonViQuiDoi,
                                        SoLuong = p.Sum(x => x.SoLuong),
                                        DonGia = p.Key.DonGia
                                    });

                                    lstChiTietTemp = itemData.BH_HoaDon_ChiTiet.GroupJoin(chitiet, hd => hd.ID_DonViQuiDoi, th => th.ID_DonViQuiDoi, (hd, th) => new { hd, th })
                                           .SelectMany(s => s.th.DefaultIfEmpty(), (s, th) => new BH_HoaDon_ChiTietDTO
                                           {
                                               ID_DonViQuiDoi = s.hd.ID_DonViQuiDoi,
                                               // check result is DefaultIfEmpty
                                               SoLuong = Math.Round(s.hd.SoLuong - (s.th.FirstOrDefault() == null ? 0 : th.SoLuong), 3, MidpointRounding.ToEven),
                                               DonGia = s.hd.DonGia,
                                               TenHangHoa = s.hd.TenHangHoa
                                           }).ToList();


                                    // chi get CTHD co SoLuong chua tra > 0
                                    lstChiTietTemp.RemoveAll(x => x.SoLuong <= 0);
                                    if (lstChiTietTemp.Count <= 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        itemData.BH_HoaDon_ChiTiet = lstChiTietTemp;
                                        goto addToList;
                                    }
                                }
                            addToList:
                                lstReturns.Add(itemData);
                            }
                        }
                        catch (Exception ex)
                        {
                            CookieStore.WriteLog("BH_HoaDonAPI_GetListHoaDonsTraHang_NotWhere1: " + ex.Message + ex.InnerException);
                            lstReturns = null;
                        }
                    }
                    return lstReturns;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("BH_HoaDonAPI_GetListHoaDonsTraHang_NotWhere2: " + ex.Message + ex.InnerException);
                return null;
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult GetListHDTraHang_afterUseAndTra(Params_GetListHoaDon param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    List<BH_HoaDonDTO> lst = classhoadon.GetListHDTraHang_afterUseAndTra(param);
                    List<BH_HoaDonDTO> lstReturns = new List<BH_HoaDonDTO>();

                    foreach (BH_HoaDonDTO itemAll in lst)
                    {
                        itemAll.BH_HoaDon_ChiTiet = classhoadon.SP_GetChiTietHoaDon_afterTraHang(itemAll.ID).Where(x => x.SoLuongConLai > 0).ToList();
                        lstReturns.Add(itemAll);
                    }

                    int count = 0, page = 0;
                    if (lst != null && lst.Count > 0)
                    {
                        count = lst[0].TotalRow ?? 0;
                        page = lst[0].TotalRow ?? 0;
                    }
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage, ref page);
                    return ActionTrueData(new
                    {
                        data = lstReturns,
                        listpage,
                        pagenow = param.CurrentPage,
                        pageview = "Hiển thị " + (param.CurrentPage * param.PageSize + 1) + " - " + (param.CurrentPage * param.PageSize + lst.Count) + " trên tổng số " + count + " bản ghi",
                        isprev = param.CurrentPage > 3 && page > 5,
                        isnext = param.CurrentPage < page - 2 && page > 5,
                        countpage = page
                    });
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException + ex.Message);
            }
        }

        [HttpGet, HttpPost]
        public JsonResult<JsonResultExampleCH> GetHD_CTHDDatHang_afterMuaHang(Params_GetListHoaDon lstParam)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    List<BH_HoaDonDTO> lstReturns = new List<BH_HoaDonDTO>();
                    var lstAllHDs = classhoadon.GetHoaDonDatHang_afterXuLy(lstParam);

                    if (lstAllHDs != null)
                    {
                        var pageSize = lstParam.PageSize;
                        var lst10Row = lstAllHDs.Skip(lstParam.CurrentPage * pageSize).Take(pageSize);

                        foreach (BH_HoaDonDTO itemAll in lst10Row)
                        {
                            // khong can gan lai gtri cho BH_HoaDon
                            itemAll.BH_HoaDon_ChiTiet = classhoadon.SP_GetChiTietHoaDon_afterTraHang(itemAll.ID);
                            lstReturns.Add(itemAll);
                        }

                        var totalRecords = lstAllHDs.Count();
                        JsonResultExampleCH json = new JsonResultExampleCH
                        {
                            lstCH = lstReturns, // 10 rows
                            Rowcount = totalRecords,
                            pageCount = System.Math.Ceiling(totalRecords * 1.0 / lstParam.PageSize),
                        };
                        return Json(json);
                    }
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetHD_CTHDDatHang_afterMuaHang " + ex.Message + ex.InnerException);
            }
            return Json(new JsonResultExampleCH());
        }

        [HttpGet, HttpPost]
        public bool CheckXuLyHet_DonDathang(Guid idHoaDon, Guid idDatHang)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    return classhoadon.CheckXuLyHet_DonDathang(idHoaDon, idDatHang);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("CheckXuLyHet_DonDathang " + ex.Message + ex.InnerException);
                return false;
            }
        }


        // lay DS chi tiet HD con lai sau khi tao HD tu HD dat hang
        public List<BH_HoaDon_ChiTietDTO> GetCTHoaDon_afterDatHang(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var data = classhoadon.SP_GetChiTietHoaDon_afterTraHang(idHoaDon);
                return data;
            }
        }

        public IHttpActionResult GetCTHDSuaChua_afterXuatKho(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);

                    var cthd = classhoadon.GetCTHDSuaChua_afterXuatKho(idHoaDon);
                    // get tpdinhluong
                    foreach (var item in cthd)
                    {
                        item.ThanhPhan_DinhLuong = classhoadonchitiet.CTHD_GetDichVubyDinhLuong(idHoaDon, item.ID_DonViQuiDoi, item.ID_LoHang);
                    }
                    return ActionTrueData(cthd);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString() + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult HDSC_GetChiTietXuatKho(Guid idHoaDon, Guid idChiTietHD, int? loaiHang = 0)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                    List<SP_ThanhPhanDinhLuong> data = classhoadonchitiet.HDSC_GetChiTietXuatKho(idHoaDon, idChiTietHD, loaiHang);
                    if (loaiHang == 1)
                    {
                        // hanghoa
                        return ActionTrueData(data);
                    }
                    else
                    {
                        var lst = data.GroupBy(o =>
                    new
                    {
                        o.MaHangHoa,
                        o.TenHangHoa,
                        o.TenDonViTinh,
                        o.MaLoHang,
                        o.SoLuongDinhLuong_BanDau,
                        o.GiaTriDinhLuong_BanDau,
                        o.LaDinhLuongBoSung,
                    }).Select(o => new
                    {
                        o.Key.MaHangHoa,
                        o.Key.TenHangHoa,
                        o.Key.TenDonViTinh,
                        o.Key.MaLoHang,
                        o.Key.SoLuongDinhLuong_BanDau,
                        o.Key.GiaTriDinhLuong_BanDau,
                        o.Key.LaDinhLuongBoSung,
                        CTXuats = o,
                    });
                        return ActionTrueData(lst);
                    }
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString() + ex.Message);
                }
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetCTHoaDon_afterTraHang(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var data = classhoadon.SP_GetChiTietHoaDon_afterTraHang(idHoaDon).Where(x => x.SoLuongConLai > 0).ToList();
                return data;
            }
        }

        [HttpGet]
        /// <summary>
        /// get BH_HoaDon_ChiTiet have SoLuongYeuCau > 0 and HD chua huy
        /// </summary>
        /// <returns></returns>
        public List<BH_HoaDon_ChiTietDTO> SP_GetThucDonYeuCau(string idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    SqlParameter param = new SqlParameter("ID_DonVi", idDonVi);
                    var data = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC SP_GetThucDonYeuCau @ID_DonVi", param).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetHoaDonChoThanhToan: " + ex.Message + ex.InnerException);
                }
                return null;
            }
        }

        [HttpGet]
        /// <summary>
        /// get BH_HoaDon_ChiTiet have Bep_SoLuongChoCungUng > 0 and HD chua huy
        /// </summary>
        /// <returns></returns>
        public List<BH_HoaDon_ChiTietDTO> SP_GetThucDonWait(string idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    SqlParameter param = new SqlParameter("ID_DonVi", idDonVi);
                    var data = db.Database.SqlQuery<BH_HoaDon_ChiTietDTO>("EXEC SP_GetThucDonWait @ID_DonVi", param).ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_GetThucDonWait: " + ex.Message + ex.InnerException);
                }
                return null;
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetThucDonYeuCau()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                List<BH_HoaDon_ChiTietDTO> lstAllHDs = classhoadonchitiet.Gets(ct => ct.Bep_SoLuongYeuCau > 0).
                    Select(x => new
                    {
                        ID = x.ID,
                        SoLuong = x.SoLuong,
                        ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                        ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                        MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                        Bep_SoLuongYeuCau = x.Bep_SoLuongYeuCau,
                        Bep_SoLuongHoanThanh = x.Bep_SoLuongHoanThanh,
                        Bep_SoLuongChoCungUng = x.Bep_SoLuongChoCungUng,
                        ThoiGian = x.ThoiGian,
                        TenPhongBan = x.BH_HoaDon.DM_ViTri.TenViTri,
                        MaHoaDon = x.BH_HoaDon.MaHoaDon,
                        TenHangHoa = x.DonViQuiDoi.DM_HangHoa.TenHangHoa,
                        ID_HoaDon = x.ID_HoaDon,
                        ID_ViTri = x.BH_HoaDon.ID_ViTri,
                        GhiChu = x.GhiChu,
                        TienThue = x.TienThue,
                    }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                    {

                        ID = c.ID,
                        ID_HoaDon = c.ID_HoaDon,
                        SoLuong = c.SoLuong,
                        ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                        MaHangHoa = c.MaHangHoa,
                        ThoiGian = c.ThoiGian,
                        Bep_SoLuongYeuCau = c.Bep_SoLuongYeuCau,
                        Bep_SoLuongHoanThanh = c.Bep_SoLuongHoanThanh,
                        Bep_SoLuongChoCungUng = c.Bep_SoLuongChoCungUng,
                        TenPhongBan = c.TenPhongBan,
                        MaHoaDon = c.MaHoaDon,
                        TenHangHoa = c.TenHangHoa,
                        ID_ViTri = c.ID_ViTri,
                        GhiChu = c.GhiChu,
                        TienThue = c.TienThue,
                    }).ToList();
                return lstAllHDs;
            }
        }

        public List<BH_HoaDon_ChiTietDTO> GetThucDonWait()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                List<BH_HoaDon_ChiTietDTO> lstAllHDs = classhoadonchitiet.Gets(ct => ct.Bep_SoLuongChoCungUng > 0).
                Select(x => new
                {
                    ID = x.ID,
                    ID_HoaDon = x.ID_HoaDon,
                    SoLuong = x.SoLuong,
                    ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                    ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                    MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                    Bep_SoLuongYeuCau = x.Bep_SoLuongYeuCau,
                    Bep_SoLuongHoanThanh = x.Bep_SoLuongHoanThanh,
                    Bep_SoLuongChoCungUng = x.Bep_SoLuongChoCungUng,
                    ThoiGian = x.ThoiGian,
                    TenPhongBan = x.BH_HoaDon.DM_ViTri.TenViTri,
                    MaHoaDon = x.BH_HoaDon.MaHoaDon,
                    TenHangHoa = x.DonViQuiDoi.DM_HangHoa.TenHangHoa,
                    ID_ViTri = x.BH_HoaDon.ID_ViTri,
                    GhiChu = x.GhiChu
                }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
                {

                    ID = c.ID,
                    ID_HoaDon = c.ID_HoaDon,
                    SoLuong = c.SoLuong,
                    ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                    MaHangHoa = c.MaHangHoa,
                    ThoiGian = c.ThoiGian,
                    Bep_SoLuongYeuCau = c.Bep_SoLuongYeuCau,
                    Bep_SoLuongHoanThanh = c.Bep_SoLuongHoanThanh,
                    Bep_SoLuongChoCungUng = c.Bep_SoLuongChoCungUng,
                    TenPhongBan = c.TenPhongBan,
                    MaHoaDon = c.MaHoaDon,
                    TenHangHoa = c.TenHangHoa,
                    ID_ViTri = c.ID_ViTri,
                    GhiChu = c.GhiChu
                }).ToList();
                return lstAllHDs;
            }
        }

        public BH_HoaDonDTO GetHoaDon_ByID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var hdDoiTra = classhoadon.Get(x => x.ID_HoaDon == id);
                try
                {
                    if (hdDoiTra != null)
                    {
                        BH_HoaDonDTO itemData = classhoadon.SP_GetInforHoaDon_byID(hdDoiTra.ID);
                        return itemData;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("BH_HoaDonAPI_GetHoaDon_ByID: " + ex.Message + ex.InnerException);
                    return null;
                }
            }
        }

        public List<BH_HoaDonDTO> GetChiTietHoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                classDM_DoiTuong classDM_DoiTuong = new classDM_DoiTuong(db);
                classDM_DonVi _ClassDV = new classDM_DonVi(db);
                ClassNS_NhanVien classNhanVien = new ClassNS_NhanVien(db);
                classDM_ViTri _classVT = new classDM_ViTri(db);

                var lstAllHDs = classhoadon.Gets(gr => gr.ID == id);
                if (lstAllHDs != null && lstAllHDs.Count() > 0)
                {
                    List<BH_HoaDonDTO> lsrReturns = new List<BH_HoaDonDTO>();
                    foreach (BH_HoaDon item in lstAllHDs)
                    {
                        string email = "", phone = "", maDT = "KL", tenDT = "Khách lẻ";
                        string tenNhanVien = "";
                        var tongTienHDTra = 0.0;
                        DM_DoiTuong itemDT = new DM_DoiTuong();
                        if (item.ID_DoiTuong != null)
                        {
                            itemDT = classDM_DoiTuong.Select_DoiTuong(item.ID_DoiTuong.Value);
                        }
                        if (itemDT != null)
                        {
                            email = itemDT.Email;
                            phone = itemDT.DienThoai;
                            maDT = itemDT.MaDoiTuong;
                            tenDT = itemDT.TenDoiTuong;
                        }
                        NS_NhanVien itemNV = new NS_NhanVien();
                        if (item.ID_NhanVien != null)
                        {
                            itemNV = classNhanVien.Select_NhanVien(item.ID_NhanVien.Value);
                        }
                        if (itemNV != null)
                        {
                            tenNhanVien = itemNV.TenNhanVien;
                        }
                        var itemHD = classhoadon.JoinHoaDon_SoQuy_byIDHoaDon(id);
                        // check IQueryable has element
                        if (itemHD != null)
                        {
                            tongTienHDTra = itemHD.TongTienHDTra;
                        }
                        BH_HoaDonDTO itemData = new BH_HoaDonDTO
                        {
                            ID = item.ID,
                            TenDonViChuyen = item.ID_DonVi != null ? _ClassDV.Get(p => p.ID == item.ID_DonVi).TenDonVi : "",
                            TenDonViNhan = item.ID_CheckIn != null ? _ClassDV.Get(p => p.ID == item.ID_CheckIn).TenDonVi : "",
                            MaHoaDon = item.MaHoaDon,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            NgaySua = item.NgaySua,
                            NguoiTao = item.KhuyenMai_GhiChu,
                            YeuCau = item.YeuCau == "1" ? "Đang Chuyển" : (item.YeuCau == "2" ? "Tạm lưu" : (item.YeuCau == "3" ? "Đã hủy" : "Đã nhận")),
                            LoaiYeuCau = item.YeuCau,
                            TongGiamGia = item.TongGiamGia,
                            TongTienHang = item.TongTienHang,
                            PhaiThanhToan = item.PhaiThanhToan,
                            TenDoiTuong = tenDT,
                            MaDoiTuong = maDT,
                            TenNhanVien = tenNhanVien,
                            NguoiTaoHD = item.NguoiTao,
                            DienGiai = item.DienGiai,
                            Email = email,
                            DienThoai = phone,
                            LoaiHoaDon = item.LoaiHoaDon,
                            MaPhieuChi = item.LoaiHoaDon == 6 ? classhoadon.getMaSoQuy(id).FirstOrDefault().MaPhieuChi : "",
                            TongTienHDTra = tongTienHDTra,
                            TenPhongBan = item.DM_ViTri != null ? _classVT.Select_ViTri((Guid)item.ID_ViTri).TenViTri : "",
                            TenDonVi = item.DM_DonVi != null ? item.DM_DonVi.TenDonVi : "",

                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                else
                    return null;
            }
        }

        public List<BH_HoaDonDTO> GetHoaDonFrom_IDViTri(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lstHD_PB = classhoadon.GetHoaDonFrom_IDViTri(id);
                List<BH_HoaDonDTO> lstReturn = new List<BH_HoaDonDTO>();
                foreach (BH_HoaDonDTO item in lstHD_PB)
                {
                    BH_HoaDonDTO dto = new BH_HoaDonDTO
                    {
                        ID = item.ID,
                        ID_DoiTuong = item.ID_DoiTuong,
                        ID_NhanVien = item.ID_NhanVien,
                        ID_BangGia = item.ID_BangGia,
                        ID_ViTri = item.ID_ViTri,
                        TenDoiTuong = item.TenDoiTuong,
                        NgayLapHoaDon = item.NgayLapHoaDon,
                        TongTienHang = item.TongTienHang,
                        PhaiThanhToan = item.PhaiThanhToan,
                        TongGiamGia = item.TongGiamGia, // DaThanhToan (0 - because ChothanhToan = true)
                        DienGiai = item.DienGiai,
                        MaHoaDon = item.MaHoaDon,
                        TenPhongBan = item.TenPhongBan,
                    };
                    lstReturn.Add(dto);
                }
                if (lstReturn != null && lstReturn.Count > 0)
                {
                    return lstReturn;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<BH_HoaDonDTO> GetHDTraHang_byIDHoaDon(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lstReturn = classhoadon.GetHDTraHang_ofHoaDon(idHoaDon);

                if (lstReturn != null && lstReturn.Count > 0)
                {
                    return lstReturn;
                }
                else
                {
                    return null;
                }
            }
        }

        // get CTHD with IDHoaDon and ChoThanhToan = true;
        public List<BH_HoaDon_ChiTietDTO> GetCTHoaDon_ByIDHoaDon(Guid idHD)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                List<BH_HoaDon_ChiTietDTO> lstAllHDs = classhoadonchitiet.Gets(ct => ct.ID_HoaDon == idHD
            && ct.BH_HoaDon.ChoThanhToan == true).Select(x => new
            {
                ID = x.ID,
                ID_HoaDon = x.ID_HoaDon,
                SoLuong = x.SoLuong,
                ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                TenDonViTinh = x.DonViQuiDoi.TenDonViTinh,
                ID_HangHoa = x.DonViQuiDoi.ID_HangHoa,
                MaHangHoa = x.DonViQuiDoi.MaHangHoa,
                Bep_SoLuongYeuCau = x.Bep_SoLuongYeuCau,
                Bep_SoLuongHoanThanh = x.Bep_SoLuongHoanThanh,
                ThoiGian = x.ThoiGian,
                TenPhongBan = x.BH_HoaDon.DM_ViTri.TenViTri,
                MaHoaDon = x.BH_HoaDon.MaHoaDon,
                TenHangHoa = x.DonViQuiDoi.DM_HangHoa.TenHangHoa,
                DonGia = x.DonGia,
                ThanhTien = x.ThanhTien,
                PTChietKhau = x.PTChietKhau,
                TienChietKhau = x.TienChietKhau
            }).AsEnumerable().Select(c => new BH_HoaDon_ChiTietDTO
            {

                ID = c.ID,
                ID_HoaDon = c.ID_HoaDon,
                SoLuong = c.SoLuong,
                ID_DonViQuiDoi = c.ID_DonViQuiDoi,
                MaHangHoa = c.MaHangHoa,
                ThoiGian = c.ThoiGian,
                Bep_SoLuongYeuCau = c.Bep_SoLuongYeuCau,
                Bep_SoLuongHoanThanh = c.Bep_SoLuongHoanThanh,
                TenPhongBan = c.TenPhongBan,
                MaHoaDon = c.MaHoaDon,
                TenHangHoa = c.TenHangHoa,
                TenDonViTinh = c.TenDonViTinh,
                DonGia = c.DonGia,
                ThanhTien = c.ThanhTien,
                PTChietKhau = c.PTChietKhau,
                TienChietKhau = c.TienChietKhau,
                GiaBan = c.DonGia - c.TienChietKhau,
            }).ToList();
                return lstAllHDs;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IEnumerable<Object> GetAllHoaDon_ofDoiTuong(Guid idDoiTuong, string idChiNhanh = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var data = classhoadon.GetHoaDon_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                if (data != null)
                {
                    try
                    {
                        // Get HoaDon chua bi Huy
                        var result = data.Where(x => x.LoaiHoaDon != 3 && x.ChoThanhToan != null).OrderByDescending(x => x.NgayLapHoaDon);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("BH_HoaDonAPI_GetAllHoaDon_ofDoiTuong: " + ex.Message + ex.InnerException);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<Object> GetHDDatHang_ofDoiTuong_andCount(Guid idDoiTuong, string idChiNhanh = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var data = classhoadon.GetHoaDon_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                if (data != null)
                {
                    try
                    {
                        // Get HoaDon chua bi Huy
                        var result = data.Where(x => x.LoaiHoaDon == 3).OrderByDescending(HD => HD.NgayLapHoaDon);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("BH_HoaDonAPI_GetHDDatHang_ofDoiTuong_andCount: " + ex.Message + ex.InnerException);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpGet]
        public BH_HoaDonDTO Get_InforHoaDon_byID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.SP_GetInforHoaDon_byID(id);
            }
        }

        [HttpGet]
        public BH_HoaDonDTO Get_InforHoaDon_byMaHoaDon(string maHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.SP_GetInforHoaDon_byMaHoaDon(maHoaDon);
            }
        }

        public bool GetDSHoaDon_chuaHuy_byIDDatHang(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var data = from hd in db.BH_HoaDon
                               where hd.ID_HoaDon == id && hd.ChoThanhToan != null
                               select hd;

                    if (data != null && data.Count() > 0)
                    {
                        return true;
                    }
                    else
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

        public bool Check_MaHoaDonExist(string maHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.Check_MaHoaDonExist(maHoaDon);
            }
        }

        #region Get_Report
        //trinhpv báo cáo bán hàng
        //Lấy danh sách theo trang
        public List<BC_BH_HoaDonDTO> getPageBaoCaoBanHang(List<BC_BH_HoaDonDTO> lst, List<BC_BH_HoaDonDTO> lstPage, int sohang, int Page)
        {
            if (lst != null)
            {
                for (int j = (Page - 1) * sohang; j < Page * sohang; j++)
                {
                    if (j < lst.Count)
                    {
                        lstPage.Add(lst[j]);
                    }
                }
            }
            if (lstPage.Count > 0)
                return lstPage;
            else
                return null;
        }
        //Lấy số bản ghi trong data
        public int getRowsCountList(List<BC_BH_HoaDonDTO> lstBCBHs)
        {
            if (lstBCBHs != null)
            {
                return lstBCBHs.Count;
            }
            else
            {
                return 0;
            }
        }
        //Lấy số trang hiển thị danh sách
        public List<ListPages> getAllPage(List<BC_BH_HoaDonDTO> lstBCBHs, List<ListPages> listPage, float sohang)
        {
            if (lstBCBHs != null)
            {
                int dem = 1;
                float SoTrang = lstBCBHs.Count / sohang;
                for (int i = 0; i < SoTrang; i++)
                {
                    ListPages BCHH_page = new ListPages();
                    BCHH_page.SoTrang = dem;
                    listPage.Add(BCHH_page);
                    dem = dem + 1;
                }
                return listPage;
            }
            else
            {
                return null;
            }
        }

        //Tính tổng tiền
        public List<BC_BH_HoaDonDTO> getMoneyBanHang(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lst = _classQHD.GetMoneyBanHang(dayStart, dayEnd, IDchinhanh);
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
        //báo cáo theo toàn thời gian
        public List<BC_BH_HoaDonDTO> getAllBC_BanHang(int sohang, int Page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstallBaocaoBanHang = _classQHD.getAllBaoCaoBanHang(IDchinhanh);
                List<BC_BH_HoaDonDTO> lstpage = new List<BC_BH_HoaDonDTO>();
                getPageBaoCaoBanHang(lstallBaocaoBanHang, lstpage, sohang, Page);
                if (lstpage.Count > 0)
                    return lstpage;
                else
                    return null;
            }
        }
        public int getRowAllBaocaoBanHang(Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstallBaocaoBanHang = _classQHD.getAllBaoCaoBanHang(IDchinhanh);
                return getRowsCountList(lstallBaocaoBanHang);
            }
        }
        public List<ListPages> getPageAllBaoCaoBanHang(float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstallBaocaoBanHang = _classQHD.getAllBaoCaoBanHang(IDchinhanh);
                List<ListPages> lstpage = new List<ListPages>();
                return getAllPage(lstallBaocaoBanHang, lstpage, sohang);
            }
        }
        //báo cáo bán hàng tại thời điểm nhất định
        List<BC_BH_HoaDonDTO> lstBCBHtoday = new List<BC_BH_HoaDonDTO>();
        public List<BC_BH_HoaDonDTO> getBC_HanHang_Today(DateTime today, int sohang, int Page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                lstBCBHtoday = _classQHD.getBaoCaoBanHangToDay(today, IDchinhanh);
                List<BC_BH_HoaDonDTO> lstpageToday = new List<BC_BH_HoaDonDTO>();
                getPageBaoCaoBanHang(lstBCBHtoday, lstpageToday, sohang, Page);
                if (lstpageToday.Count > 0)
                    return lstpageToday;
                else
                    return null;
            }
        }
        public int getRowBCBHtoday(DateTime today, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCtoday = _classQHD.getBaoCaoBanHangToDay(today, IDchinhanh);
                return getRowsCountList(lstBCtoday);
            }
        }
        public List<ListPages> getPageBCBHtoday(DateTime today, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCtoday = _classQHD.getBaoCaoBanHangToDay(today, IDchinhanh);
                List<ListPages> lstpagetoaday = new List<ListPages>();
                return getAllPage(lstBCtoday, lstpagetoaday, sohang);
            }
        }
        //Báo cáo trong khoảng thời gian
        public List<BC_BH_HoaDonDTO> getBC_BanHang_inday(DateTime DateStart, DateTime DateEnd, int sohang, int Page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCBHs = _classQHD.GetBaoCao_BanHang(DateStart, DateEnd, IDchinhanh);
                List<BC_BH_HoaDonDTO> lstpage = new List<BC_BH_HoaDonDTO>();
                getPageBaoCaoBanHang(lstBCBHs, lstpage, sohang, Page);
                if (lstpage.Count > 0)
                    return lstpage;
                else
                    return null;
            }
        }
        public int getRowBCBHinday(DateTime DateStart, DateTime DateEnd, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCinday = _classQHD.GetBaoCao_BanHang(DateStart, DateEnd, IDchinhanh);
                return getRowsCountList(lstBCinday);
            }
        }
        public List<ListPages> getPageBCBHinday(DateTime DateStart, DateTime DateEnd, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCinday = _classQHD.GetBaoCao_BanHang(DateStart, DateEnd, IDchinhanh);
                List<ListPages> lstpageinday = new List<ListPages>();
                return getAllPage(lstBCinday, lstpageinday, sohang);
            }
        }

        public List<BC_BH_HoaDonDTO> GetBC_BanHang(DateTime TuNgayLapBaoCao, DateTime DenNgayLapBaoCao, int sohang, int Page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCBHs = _classQHD.GetBaoCao_BanHang(TuNgayLapBaoCao, DenNgayLapBaoCao, IDchinhanh);
                List<BC_BH_HoaDonDTO> lstpage = new List<BC_BH_HoaDonDTO>();

                for (int j = (Page - 1) * sohang; j < Page * sohang; j++)
                {
                    if (j < lstBCBHs.Count)
                    {
                        lstpage.Add(lstBCBHs[j]);
                    }
                }
                if (lstpage.Count > 0)
                    return lstpage;
                else
                    return null;
            }
        }
        public int getTonghangBCBanHang(DateTime TuNgayLapBaoCao, DateTime DenNgayLapBaoCao, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCBHs = _classQHD.GetBaoCao_BanHang(TuNgayLapBaoCao, DenNgayLapBaoCao, IDchinhanh);
                if (lstBCBHs.Count > 0)
                {
                    return lstBCBHs.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public List<ListPages> getSoTrang(DateTime TuNgayLapBaoCao, DateTime DenNgayLapBaoCao, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                List<BC_BH_HoaDonDTO> lstBCBHs = _classQHD.GetBaoCao_BanHang(TuNgayLapBaoCao, DenNgayLapBaoCao, IDchinhanh);
                List<ListPages> listPage = new List<ListPages>();
                if (lstBCBHs.Count > 0)
                {
                    int dem = 1;
                    float SoTrang = lstBCBHs.Count / sohang;
                    //float SoTrang = float.Parse ((lstBCBHs.Count / sohang).ToString());
                    for (int i = 0; i < SoTrang; i++)
                    {
                        ListPages BCHH_page = new ListPages();
                        BCHH_page.SoTrang = dem;
                        listPage.Add(BCHH_page);
                        dem = dem + 1;
                    }
                    return listPage;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Service package
        [HttpPost, HttpGet]
        public IHttpActionResult Get_HisUsed_ofServicePackage([FromBody] JObject data, int currentPage, int pageSize)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var idDonVi = data["ID_DonVi"].ToObject<Guid>();
                var lstID = data["lstID"].ToObject<List<Guid>>();
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                List<SP_NhatKySuDung_GoiDV> lst = classhoadon.SP_NhatKySuDung_GoiDV(lstID, idDonVi, currentPage, pageSize);

                int count = 0, page = 0;
                if (lst != null && lst.Count > 0)
                {
                    count = lst[0].TotalRow ?? 0;
                    page = lst[0].TotalRow ?? 0;
                }
                var listpage = GetListPage(count, pageSize, currentPage, ref page);
                return ActionTrueData(new
                {
                    data = lst,
                    listpage,
                    pagenow = currentPage,
                    pageview = "Hiển thị " + (currentPage * pageSize + 1) + " - " + (currentPage * pageSize + lst.Count) + " trên tổng số " + count + " bản ghi",
                    isprev = currentPage > 3 && page > 5,
                    isnext = currentPage < page - 2 && page > 5,
                    countpage = page
                });
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetNhatKySuDung_GDV(ParamNKyGDV param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    if (param != null)
                    {
                        List<SP_NhatKySuDung_GoiDV> lst = classhoadon.GetNhatKySuDung_GDV(param);
                        int count = 0, page = 0;
                        int currentPage = param.CurrentPage ?? 0, pageSize = param.PageSize ?? 20;
                        if (lst != null && lst.Count > 0)
                        {
                            count = lst[0].TotalRow ?? 0;
                        }
                        var listpage = GetListPage(count, pageSize, currentPage + 1, ref page);
                        return ActionTrueData(new
                        {
                            data = lst,
                            listpage,
                            pagenow = currentPage,
                            pageview = "Hiển thị " + (currentPage * pageSize + 1) + " - " + (currentPage * pageSize + lst.Count) + " trên tổng số " + count + " bản ghi",
                            isprev = currentPage > 3 && page > 5,
                            isnext = currentPage < page - 2 && page > 5,
                            countpage = page
                        });
                    }
                    return ActionFalseNotData("Không có tham số");
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetNhatKyGiaoDich_ofCus(ParamNKyGDV param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    if (param != null)
                    {
                        List<SP_InforServicePackage> lst = classhoadon.GetNhatKyGiaoDich_ofCus(param);
                        int count = 0, page = 0;
                        int currentPage = param.CurrentPage ?? 0, pageSize = param.PageSize ?? 20;
                        if (lst != null && lst.Count > 0)
                        {
                            count = lst[0].TotalRow ?? 0;
                        }
                        var listpage = GetListPage(count, pageSize, currentPage + 1, ref page);
                        return ActionTrueData(new
                        {
                            data = lst,
                            listpage,
                            pagenow = currentPage,
                            pageview = "Hiển thị " + (currentPage * pageSize + 1) + " - " + (currentPage * pageSize + lst.Count) + " trên tổng số " + count + " bản ghi",
                            isprev = currentPage > 3 && page > 5,
                            isnext = currentPage < page - 2 && page > 5,
                            countpage = page
                        });
                    }
                    return ActionFalseNotData("Không có tham số");
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IEnumerable<SP_InforServicePackage> GetListHoaDon_UseService(Guid ID_DonVi, Guid ID_DoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                var lst = classhoadon.SP_GetListHoaDon_UseService(ID_DoiTuong, ID_DonVi);
                return lst;
            }
        }

        #endregion

        #region calendar - appointement
        [HttpPost, HttpGet]
        public IHttpActionResult GetInvoiceUseServive_Newest(Guid idKhachHang)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    SqlParameter param = new SqlParameter("ID_DoiTuong", idKhachHang);
                    var data = db.Database.SqlQuery<SP_InvoiceNewest>("GetInvoiceUseServive_Newest @ID_DoiTuong", param).ToList();
                    return Json(new { res = true, data = data });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = string.Concat(e.InnerException, e.Message) });
                }
            }
        }
        #endregion

        #endregion

        #region update

        [ResponseType(typeof(string))]
        [HttpGet, HttpPost]
        public IHttpActionResult PutBH_HoaDon([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Guid id = data["id"].ToObject<Guid>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                BH_HoaDon BH_HoaDon = data["objNewHoaDon"].ToObject<BH_HoaDon>();
                BH_HoaDon.NgayLapHoaDon = BH_HoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                DateTime ngaylaphdold = db.BH_HoaDon.Where(p => p.ID == id).FirstOrDefault().NgayLapHoaDon;
                string strUpd = classhoadon.Update_HoaDon(BH_HoaDon);

                Guid? idhoadon = null;
                if (BH_HoaDon.ChoThanhToan == false)
                {
                    idhoadon = BH_HoaDon.ID;
                }
                string sLoai = BH_HoaDon.LoaiHoaDon == 4 ? "Nhập hàng" : "Trả hàng nhập";
                HT_NhatKySuDung htnksd = db.HT_NhatKySuDung.Where(p => p.NoiDung.Contains(BH_HoaDon.MaHoaDon) && p.LoaiNhatKy == 1).OrderByDescending(p => p.ThoiGian).FirstOrDefault();
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                hT_NhatKySuDung.ID_HoaDon = idhoadon;
                hT_NhatKySuDung.LoaiHoaDon = BH_HoaDon.LoaiHoaDon;
                hT_NhatKySuDung.ThoiGianUpdateGV = ngaylaphdold;
                hT_NhatKySuDung.ChucNang = sLoai;
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = string.Concat("Cập nhật phiếu ", sLoai.ToLower(), " ", BH_HoaDon.MaHoaDon);
                hT_NhatKySuDung.NoiDungChiTiet = string.Concat("Cập nhật phiếu ", sLoai.ToLower(), " ", BH_HoaDon.MaHoaDon, " , Ngày cũ: ", ngaylaphdold.ToString("dd/MM/yyyy HH:mm:ss"), ", Ngày mới: ", BH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss"));
                hT_NhatKySuDung.LoaiNhatKy = 2;
                hT_NhatKySuDung.ID_DonVi = iddonvi;
                SaveDiary.add_Diary(hT_NhatKySuDung);

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO();
                    objReturn.ID = BH_HoaDon.ID;
                    objReturn.MaHoaDon = BH_HoaDon.MaHoaDon;
                    objReturn.NgayLapHoaDon = BH_HoaDon.NgayLapHoaDon;
                    objReturn.NgayTao = BH_HoaDon.NgayTao;
                    objReturn.TongGiamGia = BH_HoaDon.TongGiamGia;
                    objReturn.TongTienHang = BH_HoaDon.TongTienHang;
                    objReturn.PhaiThanhToan = BH_HoaDon.PhaiThanhToan;
                    objReturn.TenDoiTuong = BH_HoaDon.DM_DoiTuong != null ? BH_HoaDon.DM_DoiTuong.TenDoiTuong : "";
                    objReturn.DienGiai = BH_HoaDon.DienGiai;
                    objReturn.Email = BH_HoaDon.DM_DoiTuong != null ? BH_HoaDon.DM_DoiTuong.Email : "";
                    objReturn.DienThoai = BH_HoaDon.DM_DoiTuong != null ? BH_HoaDon.DM_DoiTuong.DienThoai : "";
                    objReturn.ChoThanhToan = BH_HoaDon.ChoThanhToan;
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [ResponseType(typeof(string))]
        [HttpPut, HttpPost, HttpGet]
        public IHttpActionResult PutBH_HoaDon2([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    Guid id = data["id"].ToObject<Guid>();
                    BH_HoaDon BH_HoaDon = data["objNewHoaDon"].ToObject<BH_HoaDon>();
                    BH_HoaDon oldBH_HD = classhoadon.Select_HoaDon(id);
                    var ngayLapHDOld = oldBH_HD.NgayLapHoaDon;

                    oldBH_HD.ID_NhanVien = BH_HoaDon.ID_NhanVien;
                    oldBH_HD.DienGiai = BH_HoaDon.DienGiai;
                    oldBH_HD.MaHoaDon = BH_HoaDon.MaHoaDon;
                    oldBH_HD.NgaySua = DateTime.Now;
                    oldBH_HD.NguoiSua = BH_HoaDon.NguoiSua;
                    oldBH_HD.NgayLapHoaDon = BH_HoaDon.NgayLapHoaDon;
                    oldBH_HD.NgayApDungGoiDV = BH_HoaDon.NgayApDungGoiDV;
                    oldBH_HD.HanSuDungGoiDV = BH_HoaDon.HanSuDungGoiDV;

                    if (!ModelState.IsValid)
                    {
                        return Json(new { res = false, mes = ModelState.ToString() });
                    }
                    string strUpd = classhoadon.Update_HoaDon(oldBH_HD);
                    // update ID_NhanVien in Quy_HoaDon if same datetime
                    if (strUpd != null && strUpd != string.Empty)
                        return Json(new
                        {
                            res = false,
                            mes = "PutBH_HoaDon2" + strUpd
                        });
                    else
                    {
                        var dataQuy = from qct in db.Quy_HoaDon_ChiTiet
                                      join qhd in db.Quy_HoaDon on qct.ID_HoaDon equals qhd.ID
                                      where qct.ID_HoaDonLienQuan == id
                                      select qhd;
                        if (dataQuy != null && dataQuy.Count() > 0)
                        {
                            if (dataQuy.FirstOrDefault().NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm tt") == ngayLapHDOld.ToString("yyyy-MM-dd HH:mm tt"))
                            {
                                Quy_HoaDon qhd = db.Quy_HoaDon.Find(dataQuy.FirstOrDefault().ID);
                                qhd.ID_NhanVien = oldBH_HD.ID_NhanVien;
                                qhd.NgayLapHoaDon = BH_HoaDon.NgayLapHoaDon;
                                qhd.NgaySua = DateTime.Now;
                                qhd.NguoiSua = BH_HoaDon.NguoiSua;
                                db.Entry(qhd).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    return Json(new
                    {
                        res = true,
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = string.Concat("PutBH_HoaDon2: ", e.InnerException, e.Message) });
            }
        }

        [HttpPost]
        public IHttpActionResult Update_ThongTinBaoHiem([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    BH_HoaDon objUp = data["objHoaDon"].ToObject<BH_HoaDon>();
                    BH_HoaDon objOld = classhoadon.Select_HoaDon(objUp.ID);

                    objOld.CongThucBaoHiem = objUp.CongThucBaoHiem;
                    objOld.TongTienBHDuyet = objUp.TongTienBHDuyet;
                    objOld.KhauTruTheoVu = objUp.KhauTruTheoVu;
                    objOld.SoVuBaoHiem = objUp.SoVuBaoHiem;
                    objOld.PTGiamTruBoiThuong = objUp.PTGiamTruBoiThuong;
                    objOld.GiamTruBoiThuong = objUp.GiamTruBoiThuong;
                    objOld.BHThanhToanTruocThue = objUp.BHThanhToanTruocThue;
                    objOld.PTThueBaoHiem = objUp.PTThueBaoHiem;
                    objOld.TongTienThueBaoHiem = objUp.TongTienThueBaoHiem;
                    objOld.GiamTruThanhToanBaoHiem = objUp.GiamTruThanhToanBaoHiem;
                    objOld.PhaiThanhToanBaoHiem = objUp.PhaiThanhToanBaoHiem;
                    objOld.PhaiThanhToan = objUp.PhaiThanhToan;
                    objOld.TongThueKhachHang = objUp.TongThueKhachHang;
                    objOld.TongGiamGia = objUp.TongGiamGia;
                    objOld.TongTienThue = objUp.TongTienThue;
                    objOld.TongThanhToan = objUp.TongThanhToan;
                    objOld.NgaySua = DateTime.Now;
                    objOld.NguoiSua = objUp.NguoiSua;
                    classhoadon.Update_HoaDon(objOld);

                    return ActionTrueNotData(string.Empty);
                }
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message + e.InnerException);
            }
        }

        [HttpGet]
        public IHttpActionResult ResetDonGiaBaoHiem(Guid idHoaDon)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    db.BH_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == idHoaDon).ToList().ForEach(x => x.DonGiaBaoHiem = 0);
                    db.SaveChanges();
                    return ActionTrueNotData(string.Empty);
                }
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message + e.InnerException);
            }
        }
        [HttpGet]
        public IHttpActionResult ResetThueChiTiet(Guid idHoaDon)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    db.BH_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == idHoaDon).ToList()
                        .ForEach(x => { x.TienThue = 0; x.PTThue = 0; });
                    db.SaveChanges();
                    return ActionTrueNotData(string.Empty);
                }
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.Message + e.InnerException);
            }
        }


        [ResponseType(typeof(string))]
        [HttpGet, HttpPost]
        public IHttpActionResult Update_CTHoaDon_Bep([FromBody] JObject data)
        {
            try
            {
                Guid id = data["id"].ToObject<Guid>();
                BH_HoaDon_ChiTiet objData = data["objUpdate"].ToObject<BH_HoaDon_ChiTiet>();

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    BH_HoaDon_ChiTiet objUpdate = db.BH_HoaDon_ChiTiet.Find(id);
                    objUpdate.Bep_SoLuongYeuCau = objData.Bep_SoLuongYeuCau;
                    objUpdate.Bep_SoLuongHoanThanh = objData.Bep_SoLuongHoanThanh;
                    objUpdate.Bep_SoLuongChoCungUng = objData.Bep_SoLuongChoCungUng;
                    db.Entry(objUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    BH_HoaDon_ChiTietDTO objReturn = new BH_HoaDon_ChiTietDTO();
                    objReturn.ID = objUpdate.ID;
                    objReturn.Bep_SoLuongYeuCau = objUpdate.Bep_SoLuongYeuCau;
                    objReturn.Bep_SoLuongHoanThanh = objUpdate.Bep_SoLuongHoanThanh;
                    objReturn.Bep_SoLuongChoCungUng = objUpdate.Bep_SoLuongChoCungUng;
                    return Json(new
                    {
                        res = true,
                        data = objReturn
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    res = false,
                    mes = e.InnerException + e.Message
                });
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public string UpdateHD_ChoThanToan(Guid id, Guid iddonvi, int loaiHoaDon, Guid idnhanvien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    BH_HoaDon item = db.BH_HoaDon.Find(id);
                    bool? chothanhtoanxoa = item.ChoThanhToan;
                    if (item != null)
                    {
                        item.ChoThanhToan = null;
                        classhoadon.Update_HoaDon(item);
                        // update DaThanhToan at QuyHD_ChiTiet
                        Quy_HoaDon_ChiTiet qct = _classQHDCT.Get(idhd => idhd.ID_HoaDonLienQuan == id);
                        if (qct != null)
                        {
                            // update status Quy_HoaDon
                            Quy_HoaDon qhd = _classQHD.Get(x => x.ID == qct.ID_HoaDon);
                            Quy_HoaDon qhdUpdate = db.Quy_HoaDon.Find(qhd.ID);
                            qhdUpdate.TrangThai = false; // Huy: false
                            _classQHD.Update_QuyHoaDon(qhdUpdate);
                        }
                        string str = CookieStore.GetCookieAes("SubDomain");

                        return "";
                    }
                    else
                    {
                        return "Update lỗi";
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string UpdateGiaVonDM_GiaVon(Guid id, Guid iddonvi, DateTime? ngaynew, int loai)
        {
            // loai = 1 : Thêm mới, loai = 2 : editHD , loai= 3 : Xóa HD
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                BH_HoaDon item = db.BH_HoaDon.Find(id);
                DateTime? NgayHDEdit = ngaynew;
                if (loai == 1)
                {
                    NgayHDEdit = ngaynew;
                }
                if (loai == 2)
                {
                    if (item.NgayLapHoaDon < ngaynew)
                    {
                        NgayHDEdit = item.NgayLapHoaDon;
                    }
                    else
                    {
                        NgayHDEdit = ngaynew;
                    }
                }
                if (loai == 3)
                {
                    NgayHDEdit = ngaynew;
                }
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("ID_HoaDon", id));
                paramlist.Add(new SqlParameter("NgayHDEdit", NgayHDEdit));
                db.Database.ExecuteSqlCommand("exec UpDateGiaVonDMHangHoaKhiTaoHD @ID_ChiNhanh, @ID_HoaDon, @NgayHDEdit", paramlist.ToArray());
                return "";
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public string Huy_HoaDon(Guid id, string nguoiSua, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                        ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                        BH_HoaDon item = db.BH_HoaDon.Find(id);

                        if (item != null)
                        {
                            item.ChoThanhToan = null;// use for HuyHoaDon
                            item.YeuCau = "4"; // use for HuyDatHang
                            item.NguoiSua = nguoiSua;
                            item.NgaySua = DateTime.Now;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();

                            // neu HD tao tu HD Dat Hag--> update again Trang Thai for HD DatHang
                            if (item.ID_HoaDon != null)
                            {
                                classhoadon.UpdateStatus_HDDatHang(item.ID_HoaDon, nguoiSua);
                            }

                            classhoadon.HuyHoaDonLienQuan(id);

                            //if (item.LoaiHoaDon == 1)
                            //{
                            //    ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                            //    classHoaDonCT.UpdateTonKhoGiaVon_whenUpdateCTHD(id, item.ID_DonVi, item.NgayLapHoaDon);
                            //}

                            // update TrangThai = false in Quy_HoaDon
                            var qct = _classQHDCT.Gets(idhd => idhd.ID_HoaDonLienQuan == id).GroupBy(x => x.ID_HoaDon).ToList();
                            foreach (var itemCT in qct)
                            {
                                var qhd = _classQHD.Get(x => x.ID == itemCT.Key);
                                Quy_HoaDon qhdUpdate = db.Quy_HoaDon.Find(qhd.ID);
                                qhdUpdate.TrangThai = false; // Huy: false
                                qhdUpdate.NgaySua = DateTime.Now;
                                qhdUpdate.NguoiSua = nguoiSua;
                                db.Entry(qhdUpdate).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            trans.Commit();
                        }
                        return string.Empty;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public string HuyHoaDon_DoiTraHang(Guid id, string nguoiSua, Guid idHDTra, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    BH_HoaDon item = db.BH_HoaDon.Find(id);

                    if (item != null)
                    {
                        #region " Huy HD Doi Hang"
                        item.ChoThanhToan = null;
                        item.NguoiSua = nguoiSua;
                        item.NgaySua = DateTime.Now;

                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();

                        Quy_HoaDon_ChiTiet qct = _classQHDCT.Get(idhd => idhd.ID_HoaDonLienQuan == id);
                        if (qct != null)
                        {
                            // update status Quy_HoaDon
                            Quy_HoaDon qhd = _classQHD.Get(x => x.ID == qct.ID_HoaDon);
                            Quy_HoaDon qhdUpdate = db.Quy_HoaDon.Find(qhd.ID);
                            qhdUpdate.TrangThai = false; // Huy: false
                            qhdUpdate.NgaySua = DateTime.Now;
                            qhdUpdate.NguoiSua = nguoiSua;

                            db.Entry(qhdUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        #region " Huy HD TraHang"
                        BH_HoaDon hdTra = db.BH_HoaDon.Find(idHDTra);
                        hdTra.ChoThanhToan = null;
                        hdTra.NguoiSua = nguoiSua;
                        hdTra.NgaySua = DateTime.Now;

                        db.Entry(hdTra).State = EntityState.Modified;
                        db.SaveChanges();

                        Quy_HoaDon_ChiTiet qctTraHang = _classQHDCT.Get(idhd => idhd.ID_HoaDonLienQuan == idHDTra);
                        if (qctTraHang != null)
                        {
                            Quy_HoaDon qhd = _classQHD.Get(x => x.ID == qctTraHang.ID_HoaDon);
                            Quy_HoaDon qhdUpdate = db.Quy_HoaDon.Find(qhd.ID);
                            qhdUpdate.TrangThai = false;
                            qhdUpdate.NgaySua = DateTime.Now;
                            qhdUpdate.NguoiSua = nguoiSua;

                            db.Entry(qhdUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        #region Update GiaVon
                        //string str = CookieStore.GetCookieAes("SubDomain");
                        //Thread st1 = new Thread(() => ClassBH_HoaDon.UpdateGiaVonDM_GiaVonLT(id, iddonvi, item.NgayLapHoaDon, 3, str));
                        //Thread st2 = new Thread(() => ClassBH_HoaDon.UpdateGiaVonDM_GiaVonLT(idHDTra, iddonvi, hdTra.NgayLapHoaDon, 3, str));
                        //st1.Start();
                        //st2.Start();
                        #endregion

                        return "";
                    }
                    else
                    {
                        return "Update lỗi";
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string UPdateKH_ChoThanhToan(Guid id, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    BH_HoaDon item = db.BH_HoaDon.Find(id);
                    bool? chothanhtoanxoa = item.ChoThanhToan;
                    if (item != null)
                    {

                        item.ChoThanhToan = null;
                        classhoadon.Update_HoaDon(item);
                        //string str = CookieStore.GetCookieAes("SubDomain");
                        Guid? idhoadon = null;
                        if (chothanhtoanxoa == false)
                        {
                            idhoadon = item.ID;
                            //Thread st1 = new Thread(() => ClassBH_HoaDon.UpdateGiaVonDM_GiaVonLT(item.ID, item.ID_DonVi, item.NgayLapHoaDon, 3, str));
                            //st1.Start();
                        }
                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                        hT_NhatKySuDung.ID = Guid.NewGuid();
                        hT_NhatKySuDung.ID_HoaDon = idhoadon;
                        hT_NhatKySuDung.LoaiHoaDon = 9;
                        hT_NhatKySuDung.ThoiGianUpdateGV = item.NgayLapHoaDon;
                        hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                        hT_NhatKySuDung.ChucNang = "Kiểm kho";
                        hT_NhatKySuDung.ThoiGian = DateTime.Now;
                        hT_NhatKySuDung.NoiDung = "Xóa phiếu kiểm kho : " + item.MaHoaDon;
                        hT_NhatKySuDung.NoiDungChiTiet = "Xóa phiếu kiểm kho : <a onclick=\"FindKiemKho('" + item.MaHoaDon + "')\">" + item.MaHoaDon + " </a> ";
                        hT_NhatKySuDung.LoaiNhatKy = 3;
                        hT_NhatKySuDung.ID_DonVi = iddonvi;
                        SaveDiary.add_Diary(hT_NhatKySuDung);
                        return "";
                    }
                    else
                    {
                        return "Update lỗi";
                    }
                }
            }
        }

        public string UPdate_YeuCauChuyenHang(Guid id, Guid idnhanvien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    BH_HoaDon item = db.BH_HoaDon.Find(id);
                    string yecauxoa = item.YeuCau;
                    if (item != null)
                    {

                        item.YeuCau = "3";
                        item.ChoThanhToan = null;
                        classhoadon.Update_HDChuyenHang(item);

                        string str = CookieStore.GetCookieAes("SubDomain");
                        Guid? idhoadon = null;
                        if (yecauxoa != "2")
                        {
                            idhoadon = item.ID;
                            //Thread st1 = new Thread(() => ClassBH_HoaDon.UpdateGiaVonDM_GiaVonLT(item.ID, item.ID_DonVi, item.NgayLapHoaDon, 3, str));
                            //st1.Start();
                        }

                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                        hT_NhatKySuDung.ID = Guid.NewGuid();
                        hT_NhatKySuDung.ID_HoaDon = idhoadon;
                        hT_NhatKySuDung.LoaiHoaDon = 10;
                        hT_NhatKySuDung.ThoiGianUpdateGV = item.NgaySua != null ? item.NgaySua : item.NgayLapHoaDon;
                        hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                        hT_NhatKySuDung.ChucNang = "Chuyển hàng";
                        hT_NhatKySuDung.ThoiGian = DateTime.Now;
                        hT_NhatKySuDung.NoiDung = "Xóa phiếu chuyển hàng : " + item.MaHoaDon;
                        hT_NhatKySuDung.NoiDungChiTiet = "Xóa phiếu chuyển hàng: <a onclick=\"FindMaHD('" + item.MaHoaDon + "')\"> " + item.MaHoaDon + "</a>";
                        hT_NhatKySuDung.LoaiNhatKy = 3;
                        hT_NhatKySuDung.ID_DonVi = iddonvi;
                        SaveDiary.add_Diary(hT_NhatKySuDung);

                        return "";
                    }
                    else
                    {
                        return "Update lỗi";
                    }
                }
            }
        }

        public string Update_StatusHD(Guid id, string Status)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    BH_HoaDon item = db.BH_HoaDon.Find(id);
                    if (item != null)
                    {
                        item.YeuCau = Status; // 1.Tam, 2.Dang giao, 3.HThanh, 4.Huy
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        return "";
                    }
                    else
                    {
                        return "Update lỗi";
                    }
                }
            }
        }

        #endregion

        #region update hoa dong chuyen hang
        // PUT: api/BH_HoaDonAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutBH_HDChuyenHang(Guid id, BH_HoaDon BH_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                if (!ModelState.IsValid)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                string strUpd = classhoadon.Update_HDChuyenHang(BH_HoaDon);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult PutBH_HDChuyenHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                Guid id = data["id"].ToObject<Guid>();
                BH_HoaDon BH_HoaDon = data["objNewHoaDon"].ToObject<BH_HoaDon>();

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = classhoadon.Update_HDChuyenHang(BH_HoaDon);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO();
                    objReturn.ID = BH_HoaDon.ID;
                    objReturn.MaHoaDon = BH_HoaDon.MaHoaDon;
                    objReturn.NgayLapHoaDon = BH_HoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                    objReturn.TongTienHang = BH_HoaDon.TongTienHang;
                    objReturn.TenDonVi = BH_HoaDon.DM_DonVi != null ? BH_HoaDon.DM_DonVi.TenDonVi : "";
                    objReturn.TenDonViChuyen = BH_HoaDon.DM_DonVi != null ? BH_HoaDon.DM_DonVi.TenDonVi : "";
                    objReturn.TenNhanVien = BH_HoaDon.NS_NhanVien != null ? BH_HoaDon.NS_NhanVien.TenNhanVien : "";
                    objReturn.TenDoiTuong = BH_HoaDon.DM_DoiTuong != null ? BH_HoaDon.DM_DoiTuong.TenDoiTuong : "";
                    objReturn.DienGiai = BH_HoaDon.DienGiai;
                    HT_NhatKySuDung htnksd = db.HT_NhatKySuDung.Where(p => p.NoiDung.Contains(objReturn.MaHoaDon)).OrderByDescending(p => p.ThoiGian).FirstOrDefault();
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = htnksd.ID_NhanVien;
                    hT_NhatKySuDung.ChucNang = "Chuyển hàng";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = htnksd.NoiDung.Replace("Thêm mới", "Cập nhật");
                    hT_NhatKySuDung.NoiDungChiTiet = htnksd.NoiDungChiTiet.Replace("Thêm mới", "Cập nhật");
                    hT_NhatKySuDung.LoaiNhatKy = 2;
                    hT_NhatKySuDung.ID_DonVi = htnksd.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }
        #endregion

        [ResponseType(typeof(string))]
        [HttpGet, HttpPost]
        public IHttpActionResult PutBH_HDChuyenHang_where([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classDM_DonVi _classDMDV = new classDM_DonVi(db);

                Guid id = data["id"].ToObject<Guid>();
                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                BH_HoaDon itemBH_HoaDon = classhoadon.Select_HoaDon(id);
                itemBH_HoaDon.MaHoaDon = objHoaDon.MaHoaDon;
                itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                itemBH_HoaDon.NguoiTao = itemBH_HoaDon.NguoiTao;
                itemBH_HoaDon.KhuyenMai_GhiChu = objHoaDon.YeuCau == "4" ? objHoaDon.NguoiTao : "";
                itemBH_HoaDon.NgaySua = objHoaDon.YeuCau == "4" ? objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond) : (DateTime?)null;
                itemBH_HoaDon.PhaiThanhToan = 0;
                itemBH_HoaDon.TongGiamGia = 0;
                itemBH_HoaDon.TongChiPhi = objHoaDon.YeuCau == "4" ? objHoaDon.TongTienHang : 0; //tổng giá trị nhận
                itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                itemBH_HoaDon.NguoiSua = objHoaDon.NguoiSua;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strIns = classhoadon.Update_HoaDon_Chuyenhang(itemBH_HoaDon);

                HT_ThongBao httbCH = new HT_ThongBao();
                httbCH.ID = Guid.NewGuid();
                httbCH.ID_DonVi = itemBH_HoaDon.ID_DonVi;
                httbCH.LoaiThongBao = 1; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                httbCH.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httbCH.ID + "')\"> Đơn chuyển hàng <a onclick=\"loadthongbao('2', '" + itemBH_HoaDon.MaHoaDon + "','" + httbCH.ID + "')\">" + " <span class=\"blue\">" + itemBH_HoaDon.MaHoaDon + " </span>" + " </a> đã được nhận thành công </p>";
                httbCH.NgayTao = DateTime.Now;
                httbCH.NguoiDungDaDoc = "";
                db.HT_ThongBao.Add(httbCH);
                db.SaveChanges();

                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    #region BH_ChiTietHoaDon
                    string listCT = "";
                    string listND = "";
                    foreach (var item in objCTHoaDon)
                    {
                        DonViQuiDoi dvqd = _classDVQD.Get(idhd => idhd.ID == item.ID_DonViQuiDoi);
                        //gán chuỗi bao gồm trong ls thao tác
                        listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";
                        listND = listND + "- " + dvqd.MaHangHoa + ":" + item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";
                        BH_HoaDon_ChiTiet ctHoaDon = classhoadonchitiet.Get(p => p.ID == item.ID);
                        DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == itemBH_HoaDon.ID_DonVi && p.ID_LoHang == ctHoaDon.ID_LoHang).FirstOrDefault();
                        ctHoaDon.TienChietKhau = item.SoLuong; // Tổng số lượng nhận
                        ctHoaDon.GiaVon_NhanChuyenHang = dmgv != null ? dmgv.GiaVon : 0;
                        ctHoaDon.GhiChu = item.GhiChu;
                        strIns = classhoadonchitiet.Update_ChiTietHoaDon(ctHoaDon);
                    }

                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ID_HoaDon = itemBH_HoaDon.ID;
                    hT_NhatKySuDung.LoaiHoaDon = 10;
                    hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgaySua;
                    hT_NhatKySuDung.ChucNang = "Chuyển hàng";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Cập nhật phiếu chuyển hàng : " + itemBH_HoaDon.MaHoaDon + "(Đang chuyển-> Đã nhận), từ chi nhánh:" + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_DonVi).TenDonVi + ", tới chi nhánh: " + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_CheckIn).TenDonVi + ", ngày chuyển: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", ngày nhận: " + itemBH_HoaDon.NgaySua.Value.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: </br> " + listND;
                    hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật phiếu chuyển hàng:  <a onclick=\"FindMaHDCH('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, (Đang chuyển-> Đã nhận), từ chi nhánh:" + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_DonVi).TenDonVi + ", tới chi nhánh: " + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_CheckIn).TenDonVi + ", ngày chuyển: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", ngày nhận: " + itemBH_HoaDon.NgaySua.Value.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: </br> " + listCT;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = itemBH_HoaDon.ID_CheckIn.Value;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    #endregion
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult PutBH_HDChuyenHang_whereTL([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classDM_DonVi _classDMDV = new classDM_DonVi(db);

                Guid id = data["id"].ToObject<Guid>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                BH_HoaDon itemBH_HoaDon = classhoadon.Select_HoaDon(id);
                itemBH_HoaDon.MaHoaDon = objHoaDon.MaHoaDon;
                itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                //#region "nguoitao"
                //itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                itemBH_HoaDon.PhaiThanhToan = 0;
                itemBH_HoaDon.TongGiamGia = 0;
                itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                //itemBH_HoaDon.TongChiPhi = objHoaDon.TongTienHang; //tổng giá trị nhận
                itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                itemBH_HoaDon.NguoiSua = objHoaDon.NguoiTao;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strIns = classhoadon.Update_HoaDon_ChuyenhangTL(itemBH_HoaDon);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(itemBH_HoaDon.ID);
                    if (sErr == string.Empty)
                    {
                        #region BH_ChiTietHoaDon
                        var dem = objCTHoaDon.Count;
                        string listCT = "";
                        string listND = "";
                        foreach (var item in objCTHoaDon)
                        {
                            dem = dem - 1;
                            DonViQuiDoi dvqd = _classDVQD.Get(p => p.ID == item.ID_DonViQuiDoi);
                            //gán chuỗi bao gồm trong ls thao tác
                            listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";
                            listND = listND + "- " + dvqd.MaHangHoa + ":" + item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                DonGia = item.GiaVon.Value,
                                ID_HoaDon = itemBH_HoaDon.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                TienChietKhau = item.SoLuong,
                                GhiChu = item.GhiChu,
                                GiaVon = dvqd.GiaVon,
                                SoThuTu = dem,
                                ChatLieu = "",
                                MauSac = "",
                                KichCo = "",
                                PTChietKhau = 0,
                                TienThue = 0,
                                PTChiPhi = 0,
                                TienChiPhi = 0,
                                ThanhToan = 0,
                                An_Hien = true,
                                ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang
                            };

                            strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        }

                        Guid? idhoadon = null;
                        if (objHoaDon.YeuCau == "1")
                        {
                            idhoadon = itemBH_HoaDon.ID;
                        }

                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                        hT_NhatKySuDung.ID = Guid.NewGuid();
                        hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                        hT_NhatKySuDung.ID_HoaDon = idhoadon;
                        hT_NhatKySuDung.LoaiHoaDon = 10;
                        hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                        hT_NhatKySuDung.ChucNang = "Chuyển hàng";
                        hT_NhatKySuDung.ThoiGian = DateTime.Now;
                        hT_NhatKySuDung.NoiDung = "Cập nhật phiếu chuyển hàng : " + itemBH_HoaDon.MaHoaDon + ", từ chi nhánh:" + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_DonVi).TenDonVi + ", tới chi nhánh: " + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_CheckIn).TenDonVi + ", ngày chuyển: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: </br> " + listND;
                        hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật phiếu chuyển hàng:  <a onclick=\"FindMaHDCH('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, từ chi nhánh:" + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_DonVi).TenDonVi + ", tới chi nhánh: " + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_CheckIn).TenDonVi + ", ngày chuyển: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: </br> " + listCT;
                        hT_NhatKySuDung.LoaiNhatKy = 1;
                        hT_NhatKySuDung.ID_DonVi = itemBH_HoaDon.ID_DonVi;
                        SaveDiary.add_Diary(hT_NhatKySuDung);
                    }
                    #endregion
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
                }
            }
        }


        [HttpGet, HttpPost]
        public IHttpActionResult Put_KiemKho([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);

                        Guid id = data["id"].ToObject<Guid>();
                        Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                        Guid id_hoadon = data["idhoadon"].ToObject<Guid>();
                        BH_HoaDon objnewKho = data["objnewKho"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objChiTietKho = new List<BH_HoaDon_ChiTiet>();
                        if (data["objChiTietKho"] != null)
                        {
                            objChiTietKho = data["objChiTietKho"].ToObject<List<BH_HoaDon_ChiTiet>>();
                        }

                        if (!string.IsNullOrEmpty(objnewKho.MaHoaDon))
                        {
                            bool exist = classhoadon.Check_MaHoaDonExist(objnewKho.MaHoaDon, id);
                            if (exist)
                            {
                                return ActionFalseNotData("Mã hóa đơn đã tồn tại");
                            }
                        }
                        else
                        {
                            objnewKho.MaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objnewKho.LoaiHoaDon, objnewKho.ID_DonVi, objnewKho.NgayLapHoaDon);
                        }

                        BH_HoaDon oldBH_HD = classhoadon.Select_HoaDon(id);
                        oldBH_HD.ID_NhanVien = objnewKho.ID_NhanVien == Guid.Empty ? null : objnewKho.ID_NhanVien;
                        oldBH_HD.DienGiai = objnewKho.DienGiai;
                        oldBH_HD.MaHoaDon = objnewKho.MaHoaDon;
                        oldBH_HD.PhaiThanhToan = objnewKho.PhaiThanhToan; // Giatritang
                        oldBH_HD.TongChietKhau = objnewKho.TongChietKhau; //GiaTriGiam
                        oldBH_HD.TongTienThue = objnewKho.TongTienThue; //TongTienlech
                        oldBH_HD.TongGiamGia = objnewKho.TongGiamGia; //Tổng chênh lệch
                        oldBH_HD.TongChiPhi = objnewKho.TongChiPhi; //SL lệch tăng
                        oldBH_HD.TongTienHang = objnewKho.TongTienHang; // SL lệch giảm
                        oldBH_HD.ChoThanhToan = objnewKho.ChoThanhToan;
                        oldBH_HD.NgayLapHoaDon = objnewKho.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);

                        string strUpd = classhoadon.Update_HoaDon(oldBH_HD);
                        if (strUpd != null && strUpd != string.Empty)
                            return ActionFalseNotData(strUpd);

                        string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(oldBH_HD.ID);
                        if (sErr == string.Empty)
                        {
                            var dem = 0;
                            string listCT = "";
                            string listND = "";
                            foreach (var item in objChiTietKho)
                            {
                                dem = dem + 1;
                                //gán chuỗi bao gồm trong ls thao tác
                                DonViQuiDoi dvqd = _classDVQD.Get(idhd => idhd.ID == item.ID_DonViQuiDoi);
                                listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + item.ThanhTien + "/" + item.SoLuong + "</br>";
                                listND = listND + "- " + dvqd.MaHangHoa + ":" + item.ThanhTien + "/" + item.SoLuong;
                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    ID_HoaDon = oldBH_HD.ID,
                                    TienChietKhau = item.SoLuong, //Tồn kho
                                    ThanhTien = item.ThanhTien, // Thực tế
                                    SoLuong = item.TienChietKhau, // SL lệch
                                    ThanhToan = item.ThanhToan, // Giá trị lệch
                                    GiaVon = item.GiaVon,
                                    SoThuTu = dem,
                                    ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang
                                    // tien giam 
                                };

                                strUpd = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                            }
                            Guid? idhoadon = null;
                            if (oldBH_HD.ChoThanhToan == false)
                            {
                                idhoadon = oldBH_HD.ID;
                            }
                            HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                            hT_NhatKySuDung.ID = Guid.NewGuid();
                            hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                            hT_NhatKySuDung.ID_HoaDon = idhoadon;
                            hT_NhatKySuDung.LoaiHoaDon = 9;
                            hT_NhatKySuDung.ThoiGianUpdateGV = oldBH_HD.NgayLapHoaDon;
                            hT_NhatKySuDung.ChucNang = "Kiểm kho";
                            hT_NhatKySuDung.ThoiGian = DateTime.Now;
                            hT_NhatKySuDung.NoiDung = "Cập nhật phiếu kiểm kho : " + oldBH_HD.MaHoaDon + ", ngày cân bằng kho:" + oldBH_HD.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: " + listND;
                            hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật phiếu kiểm kho:  <a onclick=\"FindKiemKho('" + oldBH_HD.MaHoaDon + "')\"> " + oldBH_HD.MaHoaDon + "</a>, ngày cân bằng kho:" + oldBH_HD.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: </br> " + listCT;
                            hT_NhatKySuDung.LoaiNhatKy = 1;
                            hT_NhatKySuDung.ID_DonVi = oldBH_HD.ID_DonVi;
                            SaveDiary.add_Diary(hT_NhatKySuDung);
                        }
                        trans.Commit();
                        db.SaveChanges();

                        BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                        {
                            ID = oldBH_HD.ID,
                            MaHoaDon = oldBH_HD.MaHoaDon,
                            ID_NhanVien = oldBH_HD.ID_NhanVien,
                            DienGiai = oldBH_HD.DienGiai,
                            ID_DoiTuong = oldBH_HD.ID_DoiTuong,
                            NgayLapHoaDon = oldBH_HD.NgayLapHoaDon,
                            PhaiThanhToan = oldBH_HD.PhaiThanhToan,
                            TongChietKhau = oldBH_HD.TongChietKhau,
                            TongGiamGia = oldBH_HD.TongGiamGia,
                            TongChiPhi = oldBH_HD.TongChiPhi,
                            TongTienHang = oldBH_HD.TongTienHang,
                            ChoThanhToan = oldBH_HD.ChoThanhToan,
                            NgayTao = oldBH_HD.NgayTao,
                            ID_DonVi = oldBH_HD.ID_DonVi,
                            LoaiHoaDon = oldBH_HD.LoaiHoaDon,
                            TongTienThue = oldBH_HD.TongTienThue
                        };
                        return ActionTrueData(objReturn);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.Message + ex.InnerException);
                    }
                }
            }
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult Put_TraHangNhap(Guid id, BH_HoaDon BH_HoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                if (!ModelState.IsValid)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                string strUpd = classhoadon.Update_HoaDon(BH_HoaDon);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult Put_TraHangNhap([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDM_DoiTuong classDM_DoiTuong = new classDM_DoiTuong(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                Guid id = data["id"].ToObject<Guid>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                //string tkNganHang = data["TKNganHang"].ToObject<string>();
                double dKHDaTra = data["KHDaTra"].ToObject<double>();
                double giatritienMat = data["GiaTriTienMat"].ToObject<double>();
                double giatriTienNH = data["GiaTriTienNH"].ToObject<double>();
                double giatriTienCK = data["GiaTriTienChuyenKhoan"].ToObject<double>();
                Guid IDTKPOS = Guid.Empty;
                Guid IDTKChuyenKhoan = Guid.Empty;
                if (data["IDTKPOS"] != null)
                {
                    IDTKPOS = data["IDTKPOS"].ToObject<Guid>();
                }
                if (data["IDTKChuyenKhoan"] != null)
                {
                    IDTKChuyenKhoan = data["IDTKChuyenKhoan"].ToObject<Guid>();
                }

                BH_HoaDon itemBH_HoaDon = classhoadon.Select_HoaDon(id);
                itemBH_HoaDon.MaHoaDon = objHoaDon.MaHoaDon;
                itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? new Guid("00000000-0000-0000-0000-000000000002") : objHoaDon.ID_DoiTuong;
                itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                itemBH_HoaDon.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia;
                itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;
                itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                itemBH_HoaDon.NgayTao = DateTime.Now;
                itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                itemBH_HoaDon.TyGia = 1;
                // neu luu tam => cho thanh toan == false
                itemBH_HoaDon.TongChietKhau = 0;
                itemBH_HoaDon.TongTienThue = 0;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strIns = classhoadon.Update_HoaDon(itemBH_HoaDon);
                #region "Insert Quy_Hoadon"
                DM_DoiTuong dt0 = classDM_DoiTuong.Select_DoiTuong(itemBH_HoaDon.ID_DoiTuong ?? Guid.Empty);
                string sNguoiNop = string.Empty;
                string ghiChu = string.Empty;
                if (dt0 != null)
                {
                    sNguoiNop = dt0.TenDoiTuong;
                }

                if (giatriTienNH != 0)
                {
                    ghiChu = " NCC trả tiền qua NH: " + (giatriTienNH + giatriTienCK);
                }
                if (dKHDaTra > 0 && itemBH_HoaDon.ChoThanhToan == false)
                {
                    Quy_HoaDon qhd = new Quy_HoaDon
                    {
                        ID = Guid.NewGuid(),
                        LoaiHoaDon = 11, // Phieu chi
                        MaHoaDon = "SQPT" + itemBH_HoaDon.MaHoaDon,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                        NguoiNopTien = sNguoiNop, // Khachhang or Khach le
                        NguoiTao = itemBH_HoaDon.NguoiTao,
                        ID_DonVi = itemBH_HoaDon.ID_DonVi,
                        NoiDungThu = itemBH_HoaDon.DienGiai + ghiChu,
                        TongTienThu = dKHDaTra, // khach da tra
                    };
                    _classQHD.Add_SoQuy(qhd);
                    #endregion;
                    //add nhật ký hoạt động
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ChucNang = "Phiếu thu";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Tạo phiếu thu : " + qhd.MaHoaDon + ", cho đơn nhập hàng:" + itemBH_HoaDon.MaHoaDon + ", với giá trị: " + string.Format("{0:n0}", dKHDaTra).Replace(".", ",") + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", ghi chú: " + qhd.NoiDungThu;
                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo phiếu thu:  <a onclick=\"FindSoQuy('" + qhd.MaHoaDon + "')\"> " + qhd.MaHoaDon + "</a>, cho đơn trả hàng nhập: <a onclick=\"FindMaHDTHN('" + itemBH_HoaDon.MaHoaDon + "')\">" + itemBH_HoaDon.MaHoaDon + "</a> với giá trị: " + string.Format("{0:n0}", dKHDaTra).Replace(".", ",") + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", ghi chú: " + qhd.NoiDungThu;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    #region "Insert Quy_Hoadon_ChiTiet (QuyHD(1)- QuyHD_CT(1)) "

                    if (giatritienMat == 0)
                    {
                        if (giatriTienNH == 0)
                        {
                            if (giatriTienCK == 0)
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = dKHDaTra;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = dKHDaTra;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                            else
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = giatriTienCK;
                                qct.TienMat = 0;
                                qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatriTienCK;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                        }
                        else if (giatriTienCK == 0)
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = giatriTienNH;
                            qct.ID_TaiKhoanNganHang = IDTKPOS;
                            qct.TienMat = 0;
                            qct.TienThu = giatriTienNH;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                        }
                        else
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = giatriTienNH;
                            qct.ID_TaiKhoanNganHang = IDTKPOS;
                            qct.TienMat = 0;
                            qct.TienThu = giatriTienNH;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienCK;
                            qct1.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct1.TienMat = 0;
                            qct1.TienThu = giatriTienCK;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                        }
                    }
                    else
                    {
                        if (giatriTienNH == 0)
                        {
                            if (giatriTienCK == 0)
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = giatritienMat;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatritienMat;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                            else
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = giatritienMat;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatritienMat;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                                Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                                qct1.ID = Guid.NewGuid();
                                qct1.ID_HoaDon = qhd.ID;
                                qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct1.TienGui = giatriTienCK;
                                qct1.TienMat = 0;
                                qct1.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct1.TienThu = giatriTienCK;
                                qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct1.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                            }
                        }
                        else if (giatriTienCK == 0)
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = 0;
                            qct.TienMat = giatritienMat;
                            //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct.TienThu = giatritienMat;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienNH;
                            qct1.TienMat = 0;
                            qct1.ID_TaiKhoanNganHang = IDTKPOS;
                            qct1.TienThu = giatriTienNH;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                        }
                        else
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = 0;
                            qct.TienMat = giatritienMat;
                            //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct.TienThu = giatritienMat;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienNH;
                            qct1.TienMat = 0;
                            qct1.ID_TaiKhoanNganHang = IDTKPOS;
                            qct1.TienThu = giatriTienNH;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);

                            Quy_HoaDon_ChiTiet qct2 = new Quy_HoaDon_ChiTiet();
                            qct2.ID = Guid.NewGuid();
                            qct2.ID_HoaDon = qhd.ID;
                            qct2.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct2.TienGui = giatriTienCK;
                            qct2.TienMat = 0;
                            qct2.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct2.TienThu = giatriTienCK;
                            qct2.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct2.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct2);
                        }
                    }
                    #endregion;
                }

                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(itemBH_HoaDon.ID);
                    if (sErr == string.Empty)
                    {
                        #region BH_ChiTietHoaDon
                        var dem = objCTHoaDon.Count;
                        string listCT = "";
                        string listND = "";
                        foreach (var item in objCTHoaDon)
                        {
                            dem = dem - 1;
                            DonViQuiDoi dvqd = _classDVQD.Get(idqd => idqd.ID == item.ID_DonViQuiDoi);
                            DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                            listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + item.SoLuong + " Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",") + "</br>";
                            listND = listND + "- " + dvqd.MaHangHoa + ":" + item.SoLuong + " Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",");
                            HT_CauHinhPhanMem cauhinh = _classCHPM.SelectByIDDonVi(objHoaDon.ID_DonVi);
                            double? soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi);
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                SoThuTu = dem,
                                DonGia = item.DonGia,
                                ID_HoaDon = itemBH_HoaDon.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                TienChietKhau = item.TienChietKhau,
                                GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                                // tien giam 
                                ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                                GhiChu = item.GhiChu
                            };

                            strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        }

                        Guid? idhoadon = null;
                        if (itemBH_HoaDon.ChoThanhToan == false)
                        {
                            idhoadon = itemBH_HoaDon.ID;
                        }
                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                        hT_NhatKySuDung.ID = Guid.NewGuid();
                        hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                        hT_NhatKySuDung.ID_HoaDon = idhoadon;
                        hT_NhatKySuDung.LoaiHoaDon = 7;
                        hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                        hT_NhatKySuDung.ChucNang = "Trả hàng nhập";
                        hT_NhatKySuDung.ThoiGian = DateTime.Now;
                        hT_NhatKySuDung.NoiDung = "Cập nhật phiếu trả hàng nhập: " + itemBH_HoaDon.MaHoaDon + ", thời gian:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: " + listND;
                        hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật phiếu trả hàng nhập:  <a onclick=\"FindMaHDTHN('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, thời gian:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: </br> " + listCT;
                        hT_NhatKySuDung.LoaiNhatKy = 1;
                        hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                        SaveDiary.add_Diary(hT_NhatKySuDung);
                        #endregion
                    }
                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = itemBH_HoaDon.ID,
                        MaHoaDon = itemBH_HoaDon.MaHoaDon,
                        ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                        ID_ViTri = itemBH_HoaDon.ID_ViTri,
                        DienGiai = itemBH_HoaDon.DienGiai,
                        ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        PhaiThanhToan = itemBH_HoaDon.PhaiThanhToan,
                        TongChietKhau = itemBH_HoaDon.TongChietKhau,
                        TongGiamGia = itemBH_HoaDon.TongGiamGia,
                        TongChiPhi = itemBH_HoaDon.TongChiPhi,
                        TongTienHang = itemBH_HoaDon.TongTienHang,
                        ChoThanhToan = itemBH_HoaDon.ChoThanhToan,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        ID_DonVi = itemBH_HoaDon.ID_DonVi,
                        LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon,
                        TongTienThue = itemBH_HoaDon.TongTienThue
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [ResponseType(typeof(string))]
        [HttpPost]
        public IHttpActionResult Post_NhapHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                try
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                    classDonViQuiDoi classQuiDoi = new classDonViQuiDoi(db);
                    string err = string.Empty;
                    string noidung = "";
                    string chitiet = "";

                    Guid idnhanvien = data["idNhanVien"].ToObject<Guid>();
                    BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                    List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                    List<DM_GiaBan_ChiTiet> objBangGia = new List<DM_GiaBan_ChiTiet>();
                    if (data["objBangGia"] != null && data["objBangGia"].Count() > 0)
                    {
                        objBangGia = data["objBangGia"].ToObject<List<DM_GiaBan_ChiTiet>>();
                    }

                    var ngaylapHD = objHoaDon.NgayLapHoaDon;
                    var idDoiTuong = objHoaDon.ID_DoiTuong;
                    if (idDoiTuong == null || idDoiTuong == Guid.Empty)
                    {
                        idDoiTuong = new Guid("00000000-0000-0000-0000-000000000002");
                    }
                    var newHD = objHoaDon;
                    newHD.ID_DoiTuong = idDoiTuong;
                    newHD.ID = Guid.NewGuid();
                    newHD.NgayLapHoaDon = ngaylapHD;
                    string sMaHoaDon = string.Empty;
                    if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                    {
                        sMaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                    }
                    else
                    {
                        bool exist = classHoaDon.Check_MaHoaDonExist(objHoaDon.MaHoaDon);
                        if (exist)
                        {
                            return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                        }
                        sMaHoaDon = classHoaDon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                    }
                    newHD.MaHoaDon = sMaHoaDon;
                    newHD.NgayTao = DateTime.Now;
                    newHD.TyGia = 1;
                    err = classHoaDon.Add_HoaDon(newHD);
                    if (err == string.Empty)
                    {
                        foreach (var item in objCTHoaDon)
                        {
                            var malo = item.MaLoHang != null && item.MaLoHang != string.Empty ? string.Concat("(Lô: ", item.MaLoHang, ") : Số lượng: ") : " Số lượng:  ";
                            chitiet = chitiet + "- <a onclick=\"FindMaHangHoa('" + item.MaHangHoa + "')\">" + item.MaHangHoa + " </a> " + malo + item.SoLuong + "; Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",") + "</br>";

                            // update GiaBan in tbl DonViQuiDoi
                            DonViQuiDoi dvqd = classQuiDoi.Gets(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                            if (item.GiaBanHH != null && item.GiaBanHH != 0)
                            {
                                dvqd.GiaBan = item.GiaBanHH.Value;
                                classQuiDoi.UpdateGiaBanTuNhapHang(dvqd);
                            }

                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                SoThuTu = item.SoThuTu,
                                DonGia = item.DonGia,
                                ID_HoaDon = newHD.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                TienChietKhau = item.TienChietKhau,
                                PTChietKhau = item.PTChietKhau,
                                PTThue = item.PTThue,
                                TienThue = item.TienThue,
                                ThanhToan = item.ThanhToan,
                                GiaVon = item.GiaVon,
                                ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                                GhiChu = item.GhiChu,
                                TenHangHoaThayThe = item.TenHangHoaThayThe
                            };
                            double GiaNhapTB = ctHoaDon.DonGia / dvqd.TyLeChuyenDoi;
                            if (newHD.ChoThanhToan == false)
                            {
                                List<DonViQuiDoi> lstdvqd = classQuiDoi.Gets(p => p.ID_HangHoa == item.ID_HangHoa).OrderByDescending(p => p.LaDonViChuan).ToList();
                                foreach (var itemdvqd in lstdvqd)
                                {
                                    DonViQuiDoi dvqd1 = classQuiDoi.Get(p => p.ID == itemdvqd.ID);
                                    dvqd1.GiaNhap = GiaNhapTB * itemdvqd.TyLeChuyenDoi;
                                    classQuiDoi.Update_DonViQuiDoiKhiTaoHD(dvqd1);
                                }
                            }
                            err = classHoaDonCT.Add_ChiTietHoaDon(ctHoaDon);

                            if (err == string.Empty)
                            {
                                //Update DM_HangHoa_TonKho cho lô hàng
                                DM_HangHoa_TonKho hhtonkho = db.DM_HangHoa_TonKho.Where(p => p.ID_DonViQuyDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                                if (hhtonkho == null)
                                {
                                    List<DonViQuiDoi> lstdvqd = db.DonViQuiDois.Where(p => p.ID_HangHoa == item.ID_HangHoa).ToList();
                                    foreach (var itemdvqd in lstdvqd)
                                    {
                                        classQuiDoi.UpdateID_LoHangChoCacChiNhanh(itemdvqd.ID, item.ID_LoHang);
                                    }
                                }
                            }
                        }

                        #region update GiaBan in BangGiaChiTiet
                        classDM_GiaBan_ChiTiet classGiaBanChiTiet = new classDM_GiaBan_ChiTiet(db);
                        foreach (var item in objBangGia)
                        {
                            DM_GiaBan_ChiTiet gbct = db.DM_GiaBan_ChiTiet.Where(p => p.ID_GiaBan == item.ID_GiaBan && p.ID_DonViQuiDoi == item.ID_DonViQuiDoi).FirstOrDefault();
                            if (gbct == null)
                            {
                                classGiaBanChiTiet.AddChiTietBangGiaTuNhapHang(item.ID_DonViQuiDoi, item.ID_GiaBan, item.GiaBan);
                            }
                            else
                            {
                                classGiaBanChiTiet.Update_GiaBanCT(gbct.ID, item.GiaBan);
                            }
                        }
                        #endregion

                        #region nhatky
                        string tenChucNang = string.Empty;
                        string txtFirst = string.Empty;

                        switch (objHoaDon.LoaiHoaDon)
                        {
                            case 4:
                                tenChucNang = "Nhập hàng";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    txtFirst = "Tạo mới phiếu nhập hàng: ";
                                }
                                else
                                {
                                    txtFirst = "Lưu tạm phiếu nhập hàng: ";
                                }
                                break;
                            case 7:
                                tenChucNang = "Trả hàng nhập";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    txtFirst = "Tạo mới phiếu trả hàng nhập: ";
                                }
                                else
                                {
                                    txtFirst = "Lưu tạm phiếu trả hàng nhập: ";
                                }
                                break;  
                            case 13:
                                tenChucNang = "Nhập kho nội bộ";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    txtFirst = "Tạo mới phiếu nhập kho nội bộ: ";
                                }
                                else
                                {
                                    txtFirst = "Lưu tạm phiếu nhập kho nội bộ: ";
                                }
                                break; 
                            case 14:
                                tenChucNang = "Nhập hàng khách thừa";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    txtFirst = "Tạo mới phiếu nhập hàng thừa của khách: ";
                                }
                                else
                                {
                                    txtFirst = "Lưu tạm phiếu nhập hàng thừa của khách: ";
                                }
                                break;  
                            case 31:
                                tenChucNang = "Đặt hàng nhà cung cấp";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    txtFirst = "Tạo mới phiếu đặt hàng nhà cung cấp: ";
                                }
                                else
                                {
                                    txtFirst = "Lưu tạm phiếu đặt hàng nhà cung cấp: ";
                                }
                                break;
                        }
                        noidung = string.Concat(txtFirst, sMaHoaDon, " Giá trị: ", objHoaDon.PhaiThanhToan.ToString("#,#", CultureInfo.InvariantCulture), ", Thời gian: ", ngaylapHD.ToString("dd/MM/yyy HH:mm:ss"));
                        chitiet = string.Concat(noidung, " bao gồm: <br />", chitiet);

                        HT_NhatKySuDung nky = new HT_NhatKySuDung
                        {
                            ID = Guid.NewGuid(),
                            ID_DonVi = objHoaDon.ID_DonVi,
                            LoaiHoaDon = objHoaDon.LoaiHoaDon,
                            ID_NhanVien = idnhanvien,
                            ChucNang = tenChucNang,
                            LoaiNhatKy = 1,
                            NoiDung = noidung,
                            NoiDungChiTiet = chitiet,
                            ThoiGian = DateTime.Now,
                            ID_HoaDon = newHD.ID,
                            ThoiGianUpdateGV = newHD.NgayLapHoaDon,
                        };
                        db.HT_NhatKySuDung.Add(nky);
                        db.SaveChanges();
                        if (objHoaDon.ChoThanhToan.Value == false)
                        {
                            new SaveDiary().AddQueueJob(nky);
                        }

                        #endregion
                        return Json(new { res = true, data = new { ID = newHD.ID, MaHoaDon = sMaHoaDon, NgayLapHoaDon = ngaylapHD, ID_DoiTuong = newHD.ID_DoiTuong, ID_NhanVien = newHD.ID_NhanVien } });
                    }
                    else
                    {
                        return Json(new { res = false, mes = err });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [HttpPost]
        public IHttpActionResult PostHdPhuTungTheoDoi([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classbhhoadon = new ClassBH_HoaDon(db);
                    DateTime NgayTheoDoi = DateTime.Now;
                    if (data["NgayTheoDoi"] != null && data["NgayTheoDoi"].ToObject<string>() != "")
                    {
                        NgayTheoDoi = data["NgayTheoDoi"].ToObject<DateTime>();
                    }
                    Guid IdDonVi = data["IdDonVi"].ToObject<Guid>();
                    string UserName = "";
                    UserName = data["UserName"].ToObject<string>();
                    List<Guid> lstIdDonViQuiDoi = new List<Guid>();
                    lstIdDonViQuiDoi = data["IdDonViQuiDois"].ToObject<List<Guid>>();
                    Guid IdXe = data["IdXe"].ToObject<Guid>();
                    int loaihoadon = 29;

                    BH_HoaDon newhd = new BH_HoaDon();
                    newhd.ID = Guid.NewGuid();
                    newhd.MaHoaDon = classbhhoadon.SP_GetMaHoaDon_byTemp(loaihoadon, IdDonVi, NgayTheoDoi);
                    newhd.LoaiHoaDon = loaihoadon;
                    newhd.ChoThanhToan = false;
                    newhd.TongTienHang = 0;
                    newhd.NguoiTao = UserName;
                    newhd.NgayTao = DateTime.Now;
                    newhd.NguoiSua = "";
                    newhd.YeuCau = "";
                    newhd.ID_DonVi = IdDonVi;
                    newhd.NgayLapHoaDon = NgayTheoDoi;
                    newhd.ID_Xe = IdXe;
                    List<BH_HoaDon_ChiTiet> lsthdChiTiet = new List<BH_HoaDon_ChiTiet>();
                    int stt = 1;
                    foreach (var item in lstIdDonViQuiDoi)
                    {
                        BH_HoaDon_ChiTiet newhdChiTiet = new BH_HoaDon_ChiTiet();
                        newhdChiTiet.ID = Guid.NewGuid();
                        newhdChiTiet.ID_HoaDon = newhd.ID;
                        newhdChiTiet.SoThuTu = stt;
                        stt++;
                        newhdChiTiet.SoLuong = 1;
                        newhdChiTiet.ID_DonViQuiDoi = item;
                        lsthdChiTiet.Add(newhdChiTiet);
                    }
                    db.BH_HoaDon.Add(newhd);
                    db.BH_HoaDon_ChiTiet.AddRange(lsthdChiTiet);
                    db.SaveChanges();
                    return ActionTrueNotData("");
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult PutHdPhuTungTheoDoi([FromBody] JObject data)
        {
            try
            {
                DateTime NgayTheoDoi = DateTime.Now;
                if (data["NgayTheoDoi"] != null && data["NgayTheoDoi"].ToObject<string>() != "")
                {
                    NgayTheoDoi = data["NgayTheoDoi"].ToObject<DateTime>();
                }

                List<Guid> lstRemove = new List<Guid>();
                if (data["IdRemove"] != null)
                {
                    lstRemove = data["IdRemove"].ToObject<List<Guid>>();
                }

                List<Guid> lstAdd = new List<Guid>();
                if (data["IdAdd"] != null)
                {
                    lstAdd = data["IdAdd"].ToObject<List<Guid>>();
                }
                Guid IdHoaDon = Guid.Empty;
                if (data["IdHoaDon"] != null && data["IdHoaDon"].ToObject<string>() != "")
                {
                    IdHoaDon = data["IdHoaDon"].ToObject<Guid>();
                }

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<BH_HoaDon_ChiTiet> lsthdChiTietRemove = new List<BH_HoaDon_ChiTiet>();
                    if (lstRemove.Count > 0)
                    {
                        foreach (Guid id in lstRemove)
                        {
                            BH_HoaDon_ChiTiet HdChiTietRemove = db.BH_HoaDon_ChiTiet.Where(p => p.ID == id).FirstOrDefault();
                            if (HdChiTietRemove != null)
                            {
                                lsthdChiTietRemove.Add(HdChiTietRemove);
                            }
                        }
                    }
                    List<BH_HoaDon_ChiTiet> lsthdChiTietAdd = new List<BH_HoaDon_ChiTiet>();
                    int stt = db.BH_HoaDon_ChiTiet.Where(p => p.ID_HoaDon == IdHoaDon).Select(p => p.SoThuTu).Max();
                    stt++;
                    if (lstAdd.Count > 0)
                    {
                        foreach (var item in lstAdd)
                        {
                            BH_HoaDon_ChiTiet newhdChiTiet = new BH_HoaDon_ChiTiet();
                            newhdChiTiet.ID = Guid.NewGuid();
                            newhdChiTiet.ID_HoaDon = IdHoaDon;
                            newhdChiTiet.SoThuTu = stt;
                            stt++;
                            newhdChiTiet.SoLuong = 1;
                            newhdChiTiet.ID_DonViQuiDoi = item;
                            lsthdChiTietAdd.Add(newhdChiTiet);
                        }
                    }
                    BH_HoaDon HdUpdate = db.BH_HoaDon.Where(p => p.ID == IdHoaDon).FirstOrDefault();
                    if (HdUpdate != null)
                    {
                        HdUpdate.NgayLapHoaDon = NgayTheoDoi;
                    }

                    db.BH_HoaDon_ChiTiet.RemoveRange(lsthdChiTietRemove);
                    db.BH_HoaDon_ChiTiet.AddRange(lsthdChiTietAdd);
                    db.SaveChanges();
                    int CountChiTiet = db.BH_HoaDon_ChiTiet.Where(p => p.ID_HoaDon == IdHoaDon).Count();
                    if (CountChiTiet == 0)
                    {
                        BH_HoaDon HdEmpty = db.BH_HoaDon.Where(p => p.ID == IdHoaDon).FirstOrDefault();
                        if (HdEmpty != null)
                        {
                            db.BH_HoaDon.Remove(HdEmpty);
                            db.SaveChanges();
                        }
                    }
                }
                return ActionTrueNotData("");
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult GetListPhuTungTheoDoiByIdChiTiet(string id)
        {
            try
            {
                Guid IdChiTiet = new Guid(id);
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Guid IdHoaDon = db.BH_HoaDon_ChiTiet.Where(p => p.ID == IdChiTiet).FirstOrDefault().ID_HoaDon;

                }
                return ActionTrueNotData("");
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Update_NhapHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                try
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                    classDonViQuiDoi classQuiDoi = new classDonViQuiDoi(db);
                    List<string> lstID_NewOld = new List<string>();

                    string err = string.Empty;
                    string noidung = "";
                    string chitiet = "";

                    Guid idnhanvien = data["idNhanVien"].ToObject<Guid>();
                    BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                    List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                    List<DM_GiaBan_ChiTiet> objBangGia = new List<DM_GiaBan_ChiTiet>();
                    if (data["objBangGia"] != null && data["objBangGia"].Count() > 0)
                    {
                        objBangGia = data["objBangGia"].ToObject<List<DM_GiaBan_ChiTiet>>();
                    }
                    var ngaylapHD = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                    BH_HoaDon hdOld = db.BH_HoaDon.Find(objHoaDon.ID);
                    var ngaylapOld = hdOld.NgayLapHoaDon;
                    List<BH_HoaDon_ChiTiet> ctDelete_newID = new List<BH_HoaDon_ChiTiet>();

                    #region "Get cthd old was delete"
                    var idOldCustomer = hdOld.ID_DoiTuong;
                    var inforOld = string.Empty;
                    if (hdOld.ChoThanhToan == false)
                    {
                        var cthdOld = classHoaDonCT.Gets(x => x.ID_HoaDon == objHoaDon.ID); // get cthd old
                                                                                            // if date new < date old: date old = date new - milisencond
                        string sDateOld = hdOld.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                        string sDateNew = objHoaDon.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                        if (string.Compare(sDateNew, sDateOld) == 0)
                        {
                            ngaylapHD = hdOld.NgayLapHoaDon;
                        }
                        // compare cthd old & new --> get cthd was delete
                        var cthdDelete = (from ctold in cthdOld
                                          join ctnew in objCTHoaDon on
                                          new { ctold.ID_DonViQuiDoi, ctold.ID_LoHang }
                                          equals new { ctnew.ID_DonViQuiDoi, ctnew.ID_LoHang }
                                          into ctDelete
                                          from de in ctDelete.DefaultIfEmpty()
                                          where de == null
                                          select ctold).ToList();

                        ctDelete_newID = cthdDelete.Select(x =>
                      new BH_HoaDon_ChiTiet
                      {
                          ID = Guid.NewGuid(),
                          ID_HoaDon = x.ID_HoaDon,
                          ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                          ID_LoHang = x.ID_LoHang,
                          SoLuong = x.SoLuong,
                          PTChietKhau = x.PTChietKhau,
                          TienChietKhau = x.TienChietKhau,
                          ThanhTien = x.ThanhTien,
                          ThanhToan = x.ThanhToan,
                          ChatLieu = "5", // ct delete assign chatlie="5" !important
                          GiaVon = x.GiaVon,
                          TienThue = x.TienThue,
                          PTChiPhi = x.PTChiPhi,
                          TienChiPhi = x.TienChiPhi,
                      }).ToList();
                    }
                    #endregion

                    var newHD = objHoaDon;
                    var idDoiTuong = objHoaDon.ID_DoiTuong;
                    if (idDoiTuong == Guid.Empty)
                    {
                        idDoiTuong = new Guid("00000000-0000-0000-0000-000000000002");
                    }
                    newHD.NgayLapHoaDon = ngaylapHD;
                    newHD.ID_DoiTuong = idDoiTuong;
                    string sMaHoaDon = string.Empty;
                    if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                    {
                        sMaHoaDon = classHoaDon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                    }
                    else
                    {
                        bool exist = classHoaDon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, objHoaDon.ID);
                        if (exist)
                        {
                            return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                        }
                        sMaHoaDon = objHoaDon.MaHoaDon;
                    }
                    newHD.MaHoaDon = sMaHoaDon;
                    newHD.NgayTao = DateTime.Now;
                    newHD.TyGia = 1;
                    inforOld = "Thông tin hóa đơn cũ: <br /> " + db.Database.SqlQuery<string>(" SELECT dbo.Diary_GetInforOldInvoice('" + objHoaDon.ID + "')").First();
                    err = classHoaDon.Update_HoaDon(newHD);
                    if (err == string.Empty)
                    {
                        err = classHoaDonCT.Delete_HoaDon_ChiTiet_ByIDHoaDon(newHD.ID);
                        if (newHD.ID_DoiTuong != idOldCustomer)
                        {
                            ClassQuy_HoaDon_ChiTiet classQuyCT = new ClassQuy_HoaDon_ChiTiet(db);
                            err += classQuyCT.UpdateIDKhachHang_inSoQuy(newHD.ID);
                        }
                        if (err == string.Empty)
                        {
                            foreach (var item in objCTHoaDon)
                            {
                                var malo = item.MaLoHang != null && item.MaLoHang != string.Empty ? string.Concat("(Lô: ", item.MaLoHang, ") : Số lượng: ") : " Số lượng:  ";
                                chitiet = chitiet + "- <a onclick=\"FindMaHangHoa('" + item.MaHangHoa + "')\">" + item.MaHangHoa + " </a> " + malo + item.SoLuong + "; Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",") + "</br>";

                                // update GiaBan in tbl DonViQuiDoi
                                DonViQuiDoi dvqd = classQuiDoi.Gets(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                                dvqd.GiaBan = item.GiaBanHH.Value;
                                classQuiDoi.UpdateGiaBanTuNhapHang(dvqd);

                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                    SoThuTu = item.SoThuTu,
                                    DonGia = item.DonGia,
                                    ID_HoaDon = newHD.ID,
                                    SoLuong = item.SoLuong,
                                    ThanhTien = item.ThanhTien,
                                    TienChietKhau = item.TienChietKhau,
                                    PTChietKhau = item.PTChietKhau,
                                    PTThue = item.PTThue,
                                    TienThue = item.TienThue,
                                    ThanhToan = item.ThanhToan,
                                    GiaVon = item.GiaVon,
                                    ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                                    GhiChu = item.GhiChu,
                                    TenHangHoaThayThe = item.TenHangHoaThayThe
                                };
                                double GiaNhapTB = ctHoaDon.DonGia / dvqd.TyLeChuyenDoi;
                                if (newHD.ChoThanhToan == false)
                                {
                                    List<DonViQuiDoi> lstdvqd = classQuiDoi.Gets(p => p.ID_HangHoa == item.ID_HangHoa).OrderByDescending(p => p.LaDonViChuan).ToList();
                                    foreach (var itemdvqd in lstdvqd)
                                    {
                                        DonViQuiDoi dvqd1 = classQuiDoi.Get(p => p.ID == itemdvqd.ID);
                                        dvqd1.GiaNhap = GiaNhapTB * itemdvqd.TyLeChuyenDoi;
                                        classQuiDoi.Update_DonViQuiDoiKhiTaoHD(dvqd1);
                                    }
                                }
                                err = classHoaDonCT.Add_ChiTietHoaDon(ctHoaDon);

                                if (err == string.Empty)
                                {
                                    //Update DM_HangHoa_TonKho cho lô hàng
                                    DM_HangHoa_TonKho hhtonkho = db.DM_HangHoa_TonKho.Where(p => p.ID_DonViQuyDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                                    if (hhtonkho == null)
                                    {
                                        List<DonViQuiDoi> lstdvqd = db.DonViQuiDois.Where(p => p.ID_HangHoa == item.ID_HangHoa).ToList();
                                        foreach (var itemdvqd in lstdvqd)
                                        {
                                            classQuiDoi.UpdateID_LoHangChoCacChiNhanh(itemdvqd.ID, item.ID_LoHang);
                                        }
                                    }
                                }
                            }

                            // insert again cthd old was delete (ChatLieu = 5) into hdUpdate --> caculator TonLuyKe
                            if (hdOld.ChoThanhToan == false)
                            {
                                err = classHoaDonCT.Add_ChiTietHoaDon(ctDelete_newID);
                                if (err != string.Empty)
                                {
                                    return Json(new
                                    {
                                        res = false,
                                        mess = err,
                                    });
                                }
                            }

                            #region update GiaBan in BangGiaChiTiet
                            classDM_GiaBan_ChiTiet classGiaBanChiTiet = new classDM_GiaBan_ChiTiet(db);
                            foreach (var item in objBangGia)
                            {
                                DM_GiaBan_ChiTiet gbct = db.DM_GiaBan_ChiTiet.Where(p => p.ID_GiaBan == item.ID_GiaBan && p.ID_DonViQuiDoi == item.ID_DonViQuiDoi).FirstOrDefault();
                                if (gbct == null)
                                {
                                    classGiaBanChiTiet.AddChiTietBangGiaTuNhapHang(item.ID_DonViQuiDoi, item.ID_GiaBan, item.GiaBan);
                                }
                                else
                                {
                                    classGiaBanChiTiet.Update_GiaBanCT(gbct.ID, item.GiaBan);
                                }
                            }
                            #endregion

                            #region nhatky
                            string tenChucNang = string.Empty;
                            string txtFirst = string.Empty;

                            switch (objHoaDon.LoaiHoaDon)
                            {
                                case 4:
                                    tenChucNang = "Nhập hàng";
                                    txtFirst = "Cập nhật phiếu nhập hàng: ";
                                    break;
                                case 7:
                                    tenChucNang = "Trả hàng nhập";
                                    txtFirst = "Cập nhật phiếu trả hàng nhập: ";
                                    break;  
                                case 31:
                                    tenChucNang = "Đặt hàng nhà cung cấp";
                                    txtFirst = "Cập nhật phiếu đặt hàng nhà cung cấp: ";
                                    break; 
                                case 13:
                                    tenChucNang = "Nhập kho nội bộ";
                                    txtFirst = "Cập nhật phiếu nhập kho nội bộ: ";
                                    break;  
                                case 14:
                                    tenChucNang = "Nhập hàng khách thừa";
                                    txtFirst = "Cập nhật phiếu nhập hàng khách thừa: ";
                                    break;
                            }
                            noidung = string.Concat(txtFirst, sMaHoaDon, " Giá trị: ", objHoaDon.PhaiThanhToan.ToString("#,#", CultureInfo.InvariantCulture), ", Thời gian: ", ngaylapHD.ToString("dd/MM/yyy HH:mm:ss"));
                            chitiet = string.Concat(noidung, " bao gồm: <br />", chitiet);

                            HT_NhatKySuDung nky = new HT_NhatKySuDung
                            {
                                ID = Guid.NewGuid(),
                                ID_DonVi = objHoaDon.ID_DonVi,
                                LoaiHoaDon = objHoaDon.LoaiHoaDon,
                                ID_NhanVien = idnhanvien,
                                ChucNang = tenChucNang,
                                LoaiNhatKy = 2,
                                NoiDung = noidung,
                                NoiDungChiTiet = chitiet + inforOld,
                                ThoiGian = DateTime.Now,
                                ID_HoaDon = newHD.ID,
                                ThoiGianUpdateGV = ngaylapOld,
                            };
                            db.HT_NhatKySuDung.Add(nky);
                            db.SaveChanges();
                            new SaveDiary().AddQueueJob(nky);
                            #endregion
                        }
                        return Json(new { res = true, data = new { ID = newHD.ID, MaHoaDon = sMaHoaDon, NgayLapHoaDon = ngaylapHD, ID_DoiTuong = newHD.ID_DoiTuong, ID_NhanVien = newHD.ID_NhanVien } });
                    }
                    else
                    {
                        return Json(new { res = false, mes = err });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public List<DM_GiaBan_NhapHang_DTO> getAllGiaBanByIDDVQD(Guid iddvqd, Guid iddonvi)
        {

            List<DM_GiaBan_NhapHang_DTO> lstReturn = new List<DM_GiaBan_NhapHang_DTO>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);

                if (db == null)
                {
                    return new List<DM_GiaBan_NhapHang_DTO>();
                }
                else
                {
                    List<DM_GiaBanSelect1> lst = classhoadon.GetDM_GiaBanByIDDonVi(iddonvi);
                    foreach (var item in lst)
                    {
                        DM_GiaBan_NhapHang_DTO objgb = new DM_GiaBan_NhapHang_DTO();
                        DM_GiaBan_ChiTiet ctgb = db.DM_GiaBan_ChiTiet.Where(p => p.ID_GiaBan == item.ID && p.ID_DonViQuiDoi == iddvqd).FirstOrDefault();
                        objgb.ID_DonViQuiDoi = iddvqd;
                        objgb.ID_GiaBan = item.ID;
                        objgb.TenGiaBan = item.TenGiaBan;
                        objgb.GiaBan = ctgb == null ? (double?)null : ctgb.GiaBan;
                        lstReturn.Add(objgb);
                    }
                    return lstReturn;
                }
            };
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImportExcel_TonGDV(Guid idDonVi, Guid idNhanVien, string nguoitao)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument classOffice = new Class_officeDocument(db);
                string result = "";
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            var lstErr = classOffice.ReadFileExcel(excelstream, idDonVi, idNhanVien, nguoitao);
                            if (lstErr != null && lstErr.Count() == 0)
                            {
                                return Json(new { res = true });
                            }
                            else
                            {
                                return Json(new { res = false, mes = "", data = lstErr });
                            }
                        }
                    }
                    return Json(new { res = false, mes = "data not found" });
                }
                catch (Exception ex)
                {
                    result = string.Concat("ImportExcel_TonGDV ", ex.InnerException, ex.Message);
                    return Json(new { res = false, mes = result });
                }
            }
        }

        #region insert
        // POST: api/BH_HoaDonAPI
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost, ActionName("PostBH_HoaDon")]
        [ResponseType(typeof(BH_HoaDon))]
        public IHttpActionResult PostBH_HoaDon([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDM_DoiTuong classDM_DoiTuong = new classDM_DoiTuong(db);
                classDM_GiaBan_ChiTiet _classDMGBCT = new classDM_GiaBan_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                ClassNS_NhanVien classNhanVien = new ClassNS_NhanVien(db);
                classHT_CauHinhPhanMem _classHTCH = new classHT_CauHinhPhanMem(db);
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                Guid IDTKPOS = Guid.Empty;
                Guid IDTKChuyenKhoan = Guid.Empty;
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                if (data["IDTKPOS"] != null)
                {
                    IDTKPOS = data["IDTKPOS"].ToObject<Guid>();
                }
                if (data["IDTKChuyenKhoan"] != null)
                {
                    IDTKChuyenKhoan = data["IDTKChuyenKhoan"].ToObject<Guid>();
                }
                List<DM_GiaBan_ChiTiet> objBangGia = new List<DM_GiaBan_ChiTiet>();
                if (data["objBangGia"] != null)
                {
                    objBangGia = data["objBangGia"].ToObject<List<DM_GiaBan_ChiTiet>>();
                }

                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                //string tkNganHang = data["TKNganHang"].ToObject<string>();
                double dKHDaTra = data["KHDaTra"].ToObject<double>();
                double giatritienMat = data["GiaTriTienMat"].ToObject<double>();
                double giatriTienNH = data["GiaTriTienNH"].ToObject<double>();
                double giatriTienCK = data["GiaTriTienChuyenKhoan"].ToObject<double>();

                #region BH_HoaDon
                BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                itemBH_HoaDon.ID = Guid.NewGuid();

                string sMaHoaDon = string.Empty;
                if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                {
                    //sMaHoaDon = classhoadon.GetAutoCode(objHoaDon.LoaiHoaDon);
                    sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                }
                else
                {
                    sMaHoaDon = objHoaDon.MaHoaDon;
                }
                itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                itemBH_HoaDon.ID_ViTri = objHoaDon.ID_ViTri;
                itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? new Guid("00000000-0000-0000-0000-000000000002") : objHoaDon.ID_DoiTuong;
                itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                itemBH_HoaDon.PhaiThanhToan = objHoaDon.TongTienHang - objHoaDon.TongGiamGia;
                itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia;
                itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;
                itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                itemBH_HoaDon.NgayTao = objHoaDon.NgayLapHoaDon;
                itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                itemBH_HoaDon.TyGia = 1;
                itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon; // Hoa don nhap hang
                                                                 // neu luu tam => cho thanh toan == false
                itemBH_HoaDon.TongChietKhau = 0;
                itemBH_HoaDon.TongTienThue = 0;

                #endregion

                string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);
                if (dKHDaTra != 0)
                {
                    #region "Insert Quy_Hoadon"
                    DM_DoiTuong dt0 = classDM_DoiTuong.Select_DoiTuong(itemBH_HoaDon.ID_DoiTuong ?? Guid.Empty);
                    string sNguoiNop = string.Empty;
                    string ghiChu = string.Empty;
                    if (dt0 != null)
                    {
                        sNguoiNop = dt0.TenDoiTuong;
                    }

                    if (giatriTienNH != 0)
                    {
                        ghiChu = " Tiền trả NCC qua ngân hàng: " + (giatriTienNH + giatriTienCK);
                    }
                    List<Quy_HoaDon_ChiTiet> lstct = _classQHDCT.Gets(p => p.ID_HoaDonLienQuan == itemBH_HoaDon.ID).ToList();
                    var itemcount = 0;
                    if (lstct.Count > 0)
                    {
                        itemcount = lstct.Count();
                    }
                    string mahoadon = "";
                    if (itemcount >= 1)
                    {
                        mahoadon = "_" + itemcount;
                    }

                    Quy_HoaDon qhd = new Quy_HoaDon
                    {
                        ID = Guid.NewGuid(),
                        LoaiHoaDon = 12, // Phieu chi
                        MaHoaDon = "SQPC" + sMaHoaDon + mahoadon,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                        NguoiNopTien = sNguoiNop, // Khachhang or Khach le
                        NoiDungThu = itemBH_HoaDon.DienGiai + ghiChu,
                        TongTienThu = dKHDaTra, // khach da tra
                        ID_DonVi = itemBH_HoaDon.ID_DonVi,
                        NguoiTao = classNhanVien.Select_NhanVien(itemBH_HoaDon.ID_NhanVien).TenNhanVien
                    };
                    _classQHD.Add_SoQuy(qhd);
                    #endregion;
                    //add nhật ký hoạt động
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ChucNang = "Phiếu chi";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Tạo phiếu chi : " + qhd.MaHoaDon + ", cho đơn nhập hàng:" + itemBH_HoaDon.MaHoaDon + ", với giá trị: " + string.Format("{0:n0}", dKHDaTra).Replace(".", ",") + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", ghi chú: " + qhd.NoiDungThu;
                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo phiếu chi:  <a onclick=\"FindSoQuy('" + qhd.MaHoaDon + "')\"> " + qhd.MaHoaDon + "</a>, cho đơn nhập hàng: <a onclick=\"FindMaHD('" + itemBH_HoaDon.MaHoaDon + "')\">" + itemBH_HoaDon.MaHoaDon + "</a> với giá trị: " + string.Format("{0:n0}", dKHDaTra).Replace(".", ",") + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", ghi chú: " + qhd.NoiDungThu;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    #region "Insert Quy_Hoadon_ChiTiet (QuyHD(1)- QuyHD_CT(1)) "

                    if (giatritienMat == 0)
                    {
                        if (giatriTienNH == 0)
                        {
                            if (giatriTienCK == 0)
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = dKHDaTra;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = dKHDaTra;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                            else
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = giatriTienCK;
                                qct.TienMat = 0;
                                qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatriTienCK;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                        }
                        else if (giatriTienCK == 0)
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = giatriTienNH;
                            qct.ID_TaiKhoanNganHang = IDTKPOS;
                            qct.TienMat = 0;
                            qct.TienThu = giatriTienNH;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                        }
                        else
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = giatriTienNH;
                            qct.ID_TaiKhoanNganHang = IDTKPOS;
                            qct.TienMat = 0;
                            qct.TienThu = giatriTienNH;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienCK;
                            qct1.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct1.TienMat = 0;
                            qct1.TienThu = giatriTienCK;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                        }
                    }
                    else
                    {
                        if (giatriTienNH == 0)
                        {
                            if (giatriTienCK == 0)
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = giatritienMat;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatritienMat;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                            else
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = giatritienMat;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatritienMat;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                                Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                                qct1.ID = Guid.NewGuid();
                                qct1.ID_HoaDon = qhd.ID;
                                qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct1.TienGui = giatriTienCK;
                                qct1.TienMat = 0;
                                qct1.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct1.TienThu = giatriTienCK;
                                qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct1.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                            }
                        }
                        else if (giatriTienCK == 0)
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = 0;
                            qct.TienMat = giatritienMat;
                            //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct.TienThu = giatritienMat;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienNH;
                            qct1.TienMat = 0;
                            qct1.ID_TaiKhoanNganHang = IDTKPOS;
                            qct1.TienThu = giatriTienNH;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                        }
                        else
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = 0;
                            qct.TienMat = giatritienMat;
                            //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct.TienThu = giatritienMat;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienNH;
                            qct1.TienMat = 0;
                            qct1.ID_TaiKhoanNganHang = IDTKPOS;
                            qct1.TienThu = giatriTienNH;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);

                            Quy_HoaDon_ChiTiet qct2 = new Quy_HoaDon_ChiTiet();
                            qct2.ID = Guid.NewGuid();
                            qct2.ID_HoaDon = qhd.ID;
                            qct2.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct2.TienGui = giatriTienCK;
                            qct2.TienMat = 0;
                            qct2.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct2.TienThu = giatriTienCK;
                            qct2.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct2.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct2);
                        }
                    }
                    #endregion;
                }
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {


                    #region BH_ChiTietHoaDon
                    var dem = objCTHoaDon.Count;
                    string listCT = "";
                    string listND = "";
                    foreach (var item in objCTHoaDon)
                    {
                        dem = dem - 1;
                        DonViQuiDoi dvqd = _classDVQD.Gets(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                        dvqd.GiaBan = item.GiaBanHH.Value;
                        _classDVQD.UpdateGiaBanTuNhapHang(dvqd);
                        DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                        HT_CauHinhPhanMem cauhinh = _classHTCH.SelectByIDDonVi(objHoaDon.ID_DonVi);
                        listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + item.SoLuong + " Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",") + "</br>";
                        listND = listND + "- " + dvqd.MaHangHoa + ":" + item.SoLuong + " Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",");
                        double? soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi);
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            SoThuTu = dem,
                            DonGia = item.DonGia,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            SoLuong = item.SoLuong,
                            ThanhTien = item.ThanhTien,
                            TienChietKhau = item.TienChietKhau,
                            GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                            ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                            GhiChu = item.GhiChu
                        };
                        double GiaNhapTB = ctHoaDon.DonGia / dvqd.TyLeChuyenDoi;
                        if (objHoaDon.ChoThanhToan == false)
                        {
                            Guid idhanghoa = _classDVQD.Get(p => p.ID == dvqd.ID).ID_HangHoa;
                            List<DonViQuiDoi> lstdvqd = _classDVQD.Gets(p => p.ID_HangHoa == idhanghoa).OrderByDescending(p => p.LaDonViChuan).ToList();
                            foreach (var itemdvqd in lstdvqd)
                            {
                                DonViQuiDoi dvqd1 = _classDVQD.Get(p => p.ID == itemdvqd.ID);
                                dvqd1.GiaNhap = GiaNhapTB * itemdvqd.TyLeChuyenDoi;
                                _classDVQD.Update_DonViQuiDoiKhiTaoHD(dvqd1);
                            }
                        }

                        strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);

                        //Update DM_HangHoa_TonKho cho lô hàng
                        DM_HangHoa_TonKho hhtonkho = db.DM_HangHoa_TonKho.Where(p => p.ID_DonViQuyDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                        if (hhtonkho == null)
                        {
                            DonViQuiDoi dvqdct = db.DonViQuiDois.Where(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                            List<DonViQuiDoi> lstdvqd = db.DonViQuiDois.Where(p => p.ID_HangHoa == dvqdct.ID_HangHoa).ToList();
                            foreach (var itemdvqd in lstdvqd)
                            {
                                _classDVQD.UpdateID_LoHangChoCacChiNhanh(itemdvqd.ID, item.ID_LoHang);
                            }
                        }
                        //itemBH_HoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon
                    }

                    #endregion
                    Guid? idhoadon = null;
                    if (itemBH_HoaDon.ChoThanhToan == false)
                    {
                        idhoadon = itemBH_HoaDon.ID;
                    }
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_HoaDon = idhoadon;
                    hT_NhatKySuDung.LoaiHoaDon = 4;
                    hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ChucNang = "Nhập hàng";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Tạo mới phiếu nhập hàng: " + itemBH_HoaDon.MaHoaDon + ", thời gian:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: " + listND;
                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu nhập hàng:  <a onclick=\"FindMaHD('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, thời gian:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: </br> " + listCT;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);

                    foreach (var item in objBangGia)
                    {
                        DM_GiaBan_ChiTiet gbct = db.DM_GiaBan_ChiTiet.Where(p => p.ID_GiaBan == item.ID_GiaBan && p.ID_DonViQuiDoi == item.ID_DonViQuiDoi).FirstOrDefault();
                        if (gbct == null)
                        {
                            _classDMGBCT.AddChiTietBangGiaTuNhapHang(item.ID_DonViQuiDoi, item.ID_GiaBan, item.GiaBan);
                        }
                        else
                        {
                            _classDMGBCT.Update_GiaBanCT(gbct.ID, item.GiaBan);
                        }
                    }

                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = itemBH_HoaDon.ID,
                        MaHoaDon = itemBH_HoaDon.MaHoaDon,
                        ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                        ID_ViTri = itemBH_HoaDon.ID_ViTri,
                        DienGiai = itemBH_HoaDon.DienGiai,
                        ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        PhaiThanhToan = itemBH_HoaDon.PhaiThanhToan,
                        TongChietKhau = itemBH_HoaDon.TongChietKhau,
                        TongGiamGia = itemBH_HoaDon.TongGiamGia,
                        TongChiPhi = itemBH_HoaDon.TongChiPhi,
                        TongTienHang = itemBH_HoaDon.TongTienHang,
                        ChoThanhToan = itemBH_HoaDon.ChoThanhToan,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        ID_DonVi = itemBH_HoaDon.ID_DonVi,
                        LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon,
                        TongTienThue = itemBH_HoaDon.TongTienThue
                    };

                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult PostBH_HoaDonNapThe([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {


                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        string sMaHoaDon = string.Empty;

                        BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                        itemBH_HoaDon.ID = Guid.NewGuid();
                        if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                        {
                            sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }
                        else
                        {
                            sMaHoaDon = objHoaDon.MaHoaDon;
                        }
                        itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                        itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien;
                        itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                        itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong;
                        itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon;
                        itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi; //mức nạp
                        itemBH_HoaDon.TongChietKhau = objHoaDon.TongChietKhau; //khuyến mại nạp thẻ
                        itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang; //tổng tiền nạp
                        itemBH_HoaDon.TongTienThue = objHoaDon.TongTienThue; //số dư sau nạp
                        itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia; //giảm giá cả hóa đơn
                        itemBH_HoaDon.PhaiThanhToan = objHoaDon.PhaiThanhToan; //phải thanh toán (= Tiền khách trả)
                        itemBH_HoaDon.TongThanhToan = objHoaDon.TongThanhToan;
                        itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                        itemBH_HoaDon.NgayTao = DateTime.Now;
                        itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;
                        itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                        itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon;
                        classhoadon.Add_HoaDon(itemBH_HoaDon);
                        trans.Commit();

                        BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                        {
                            ID = itemBH_HoaDon.ID,
                            MaHoaDon = itemBH_HoaDon.MaHoaDon,
                            ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                            ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong,
                            DienGiai = itemBH_HoaDon.DienGiai,
                            NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                            PhaiThanhToan = itemBH_HoaDon.PhaiThanhToan,
                            TongChietKhau = itemBH_HoaDon.TongChietKhau,
                            TongGiamGia = itemBH_HoaDon.TongGiamGia,
                            TongChiPhi = itemBH_HoaDon.TongChiPhi,
                            TongTienHang = itemBH_HoaDon.TongTienHang,
                            ChoThanhToan = itemBH_HoaDon.ChoThanhToan,
                            NgayTao = itemBH_HoaDon.NgayTao,
                            ID_DonVi = itemBH_HoaDon.ID_DonVi,
                            LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon,
                            TongTienThue = itemBH_HoaDon.TongTienThue,
                        };
                        return ActionTrueData(objReturn);
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(e.ToString());
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostNhanVien_ThucHien([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    List<BH_NhanVienThucHien> objCT = data["objCT"].ToObject<List<BH_NhanVienThucHien>>();
                    Guid idthegiatri = data["idthegiatri"].ToObject<Guid>();
                    Guid idquyhoadon = data["idquyhoadon"].ToObject<Guid>();

                    // delete by phieuthu & add again (used at thegiatri +  update phieuthu hoadon)
                    var nvExist = db.BH_NhanVienThucHien.Where(x => x.ID_HoaDon == idthegiatri && x.ID_QuyHoaDon == idquyhoadon && x.TinhChietKhauTheo == 1);
                    db.BH_NhanVienThucHien.RemoveRange(nvExist);

                    foreach (var item in objCT)
                    {
                        BH_NhanVienThucHien nvth = new BH_NhanVienThucHien
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = item.ID_NhanVien,
                            TienChietKhau = item.TienChietKhau,
                            PT_ChietKhau = item.PT_ChietKhau,
                            ID_HoaDon = idthegiatri,
                            TheoYeuCau = false,
                            ThucHien_TuVan = false,
                            TinhChietKhauTheo = item.TinhChietKhauTheo,
                            ID_QuyHoaDon = idquyhoadon,
                            HeSo = item.HeSo
                        };
                        db.BH_NhanVienThucHien.Add(nvth);
                        db.SaveChanges();
                    }
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult UpdateCKNhanVien_HoaDon(List<BH_NhanVienThucHien> nv, Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // delete & add again
                        var nvthHD = db.BH_NhanVienThucHien.Where(x => x.ID_HoaDon == idHoaDon);
                        db.BH_NhanVienThucHien.RemoveRange(nvthHD);

                        // get all soquy of hoadon
                        var lstSQ = (from qhd in db.Quy_HoaDon
                                     join qct in db.Quy_HoaDon_ChiTiet on qhd.ID equals qct.ID_HoaDon
                                     join hd in db.BH_HoaDon on qct.ID_HoaDonLienQuan equals hd.ID
                                     where hd.ID == idHoaDon && qhd.TrangThai != false
                                     && qct.ID_HoaDonLienQuan == idHoaDon
                                     group new { qhd, qct } by new { qhd.ID, } into g
                                     select new
                                     {
                                         ID = g.Key.ID,
                                         TongTienThu = g.Where(x => x.qct.HinhThucThanhToan != 4 && x.qct.HinhThucThanhToan != 5)
                                         .Sum(x => x.qct.TienThu)
                                     }).ToList();

                        Guid? idSQFirst = Guid.Empty;
                        List<BH_NhanVienThucHien> lstAdd = new List<BH_NhanVienThucHien>();
                        if (lstSQ != null && lstSQ.Count() > 0)
                        {
                            idSQFirst = lstSQ.FirstOrDefault().ID;
                        }
                        var lstNV_notThucThu = nv.Where(x => x.TinhChietKhauTheo != 1).ToList();
                        foreach (var nv1 in lstNV_notThucThu)
                        {
                            nv1.ID = Guid.NewGuid();
                            nv1.ID_QuyHoaDon = idSQFirst == Guid.Empty ? null : idSQFirst;
                            lstAdd.Add(nv1);
                        }

                        // 1 phiếu thu - N nhân viên
                        var lstNV_ThucThu = nv.Where(x => x.TinhChietKhauTheo == 1).ToList();
                        foreach (var nvien in lstNV_ThucThu)
                        {
                            foreach (var sq in lstSQ)
                            {
                                BH_NhanVienThucHien objNew = new BH_NhanVienThucHien
                                {
                                    ID = Guid.NewGuid(),
                                    ID_QuyHoaDon = sq.ID,
                                    ID_NhanVien = nvien.ID_NhanVien,
                                    ID_HoaDon = nvien.ID_HoaDon,
                                    PT_ChietKhau = nvien.PT_ChietKhau,
                                    TienChietKhau = (nvien.HeSo * sq.TongTienThu * nvien.PT_ChietKhau / 100) ?? 1,
                                    TinhChietKhauTheo = nvien.TinhChietKhauTheo,
                                    TheoYeuCau = nvien.TheoYeuCau,
                                    ThucHien_TuVan = nvien.ThucHien_TuVan,
                                    HeSo = nvien.HeSo,
                                };
                                lstAdd.Add(objNew);
                            }
                        }

                        db.BH_NhanVienThucHien.AddRange(lstAdd);
                        db.SaveChanges();
                        trans.Commit();
                        return Json(new { res = true });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return Json(new { res = false, mes = ex });
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult UpdateCKNhanVien_DichVu([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<BH_NhanVienThucHien> nv = data["NhanViens"].ToObject<List<BH_NhanVienThucHien>>();
                        List<Guid?> ids = data["IDChiTiets"].ToObject<List<Guid?>>();// ds idchitiet must update

                        // delete & add again
                        var nvthHD = db.BH_NhanVienThucHien.Where(x => ids.Any(y => y == x.ID_ChiTietHoaDon));
                        db.BH_NhanVienThucHien.RemoveRange(nvthHD);

                        List<BH_NhanVienThucHien> lstAdd = new List<BH_NhanVienThucHien>();
                        foreach (var item in nv)
                        {
                            item.ID = Guid.NewGuid();
                            lstAdd.Add(item);
                        }
                        db.BH_NhanVienThucHien.AddRange(lstAdd);
                        db.SaveChanges();
                        trans.Commit();
                        return Json(new { res = true });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return Json(new { res = false, mes = ex });
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string UpdateThoiGianPhieuNap(Guid id, DateTime time, string ghichu)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Error";
                }
                else
                {
                    BH_HoaDon objUpd = db.BH_HoaDon.Find(id);
                    objUpd.NgayLapHoaDon = time.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                    objUpd.DienGiai = ghichu;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                    //List<SqlParameter> paramlist = new List<SqlParameter>();
                    //paramlist.Add(new SqlParameter("NgayLapHoaDonInput", objUpd.NgayLapHoaDon));
                    //paramlist.Add(new SqlParameter("IDDoiTuong", objUpd.ID_DoiTuong));
                    //try
                    //{
                    //    Thread str1 = new Thread(() => db.Database.ExecuteSqlCommand("exec UpdateLaiSoDuTheNap @NgayLapHoaDonInput, @IDDoiTuong", paramlist.ToArray()));
                    //    str1.Start();
                    //}
                    //catch (Exception ex)
                    //{
                    //    CookieStore.WriteLog("BH_HoaDonAPI_UpdateThoiGianPhieuNap: " + ex.Message + ex.InnerException);
                    //}
                    return "";
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string XoaTheNap(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    if (db == null)
                    {
                        return "DB null";
                    }
                    else
                    {

                        classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                        ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                        BH_HoaDon objUpd = db.BH_HoaDon.Find(id);
                        objUpd.ChoThanhToan = null;
                        db.Entry(objUpd).State = EntityState.Modified;
                        db.SaveChanges();

                        var qct = _classQHDCT.Gets(idhd => idhd.ID_HoaDonLienQuan == id).GroupBy(x => x.ID_HoaDon).ToList();
                        foreach (var itemCT in qct)
                        {
                            var qhd = _classQHD.Get(x => x.ID == itemCT.Key);
                            Quy_HoaDon qhdUpdate = db.Quy_HoaDon.Find(qhd.ID);
                            qhdUpdate.TrangThai = false; // Huy: false
                            qhdUpdate.NgaySua = DateTime.Now;
                            db.Entry(qhdUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        List<SqlParameter> paramlist = new List<SqlParameter>();
                        paramlist.Add(new SqlParameter("NgayLapHoaDonInput", objUpd.NgayLapHoaDon));
                        paramlist.Add(new SqlParameter("IDDoiTuong", objUpd.ID_DoiTuong));
                        try
                        {
                            db.Database.ExecuteSqlCommand("exec UpdateLaiSoDuTheNap @NgayLapHoaDonInput, @IDDoiTuong", paramlist.ToArray());
                        }
                        catch (Exception ex)
                        {
                            CookieStore.WriteLog("BH_HoaDonAPI_XoaTheNap: " + ex.Message + ex.InnerException);
                        }
                        return "";
                    }
                }
                catch (Exception e)
                {
                    return e.InnerException + e.Message;
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string CheckTheDaSuDung(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    if (db == null)
                    {
                        return "DB null";
                    }
                    else
                    {
                        ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                        return classHoaDon.CheckTheDaSuDung(id);
                    }
                }
                catch (Exception e)
                {
                    return e.InnerException + e.Message;
                }
            }
        }


        [HttpPost, HttpGet, ActionName("PostBH_HoaDonKiemKho")]
        [ResponseType(typeof(BH_HoaDon))]
        public IHttpActionResult PostBH_HoaDonKiemKho([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);

                BH_HoaDon objnewKho = data["objnewKho"].ToObject<BH_HoaDon>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                List<BH_HoaDon_ChiTiet> objChiTietKho = new List<BH_HoaDon_ChiTiet>();
                if (data["objChiTietKho"] != null)
                {
                    objChiTietKho = data["objChiTietKho"].ToObject<List<BH_HoaDon_ChiTiet>>();
                }
                BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                itemBH_HoaDon.ID = Guid.NewGuid();
                string sMaHoaDon = string.Empty;
                if (string.IsNullOrEmpty(objnewKho.MaHoaDon))
                {
                    sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objnewKho.LoaiHoaDon, objnewKho.ID_DonVi, objnewKho.NgayLapHoaDon);
                }
                else
                {
                    bool exist = classhoadon.Check_MaHoaDonExist(objnewKho.MaHoaDon);
                    if (exist)
                    {
                        return ActionFalseNotData("Mã hóa đơn đã tồn tại");
                    }
                    sMaHoaDon = objnewKho.MaHoaDon;
                }
                itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                itemBH_HoaDon.ID_NhanVien = objnewKho.ID_NhanVien;
                itemBH_HoaDon.ID_DonVi = objnewKho.ID_DonVi;
                itemBH_HoaDon.NguoiTao = objnewKho.NguoiTao;
                itemBH_HoaDon.DienGiai = objnewKho.DienGiai;
                itemBH_HoaDon.NgayLapHoaDon = objnewKho.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                itemBH_HoaDon.PhaiThanhToan = objnewKho.PhaiThanhToan; // Giatritang
                itemBH_HoaDon.TongChietKhau = objnewKho.TongChietKhau; //GiaTriGiam
                itemBH_HoaDon.TongTienThue = objnewKho.TongTienThue; //TongTienlech
                itemBH_HoaDon.TongGiamGia = objnewKho.TongGiamGia; //Tổng chênh lệch
                itemBH_HoaDon.TongChiPhi = objnewKho.TongChiPhi; //SL lệch tăng
                itemBH_HoaDon.TongTienHang = objnewKho.TongTienHang; // SL lệch giảm
                itemBH_HoaDon.ChoThanhToan = objnewKho.ChoThanhToan;
                itemBH_HoaDon.LoaiHoaDon = objnewKho.LoaiHoaDon;
                string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);

                if (strIns != null && strIns != string.Empty)
                    return ActionFalseNotData(strIns);
                else
                {
                    #region BH_ChiTietHoaDon
                    var dem = objChiTietKho.Count;
                    string listCT = "";
                    string listND = "";
                    foreach (var item in objChiTietKho)
                    {
                        dem = dem - 1;
                        //gán chuỗi bao gồm trong ls thao tác
                        listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + item.MaHangHoa + "')\">"
                            + item.MaHangHoa + " </a> :" + item.ThanhTien + "/" + item.SoLuong + "</br>";
                        listND = listND + "- " + item.MaHangHoa + ":" + item.ThanhTien + "/" + item.SoLuong;
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            TienChietKhau = item.SoLuong, //Tồn kho
                            ThanhTien = item.ThanhTien, // Thực tế
                            SoLuong = item.TienChietKhau, // SL lệch
                            ThanhToan = item.ThanhToan, // Giá trị lệch
                            GiaVon = item.GiaVon,
                            SoThuTu = dem,
                            ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                            TonLuyKe = item.ThanhTien
                            // tien giam 
                        };

                        strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                    }
                    Guid? idhoadon = null;
                    if (itemBH_HoaDon.ChoThanhToan == false)
                    {
                        idhoadon = itemBH_HoaDon.ID;
                    }
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ID_HoaDon = idhoadon;
                    hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                    hT_NhatKySuDung.LoaiHoaDon = 9;
                    hT_NhatKySuDung.ChucNang = "Kiểm kho";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Tạo mới phiếu kiểm kho : " + itemBH_HoaDon.MaHoaDon + ", ngày cân bằng kho:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: " + listND;
                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu kiểm kho:  <a onclick=\"FindKiemKho('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, ngày cân bằng kho:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: </br>" + listCT;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = objnewKho.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    #endregion
                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = itemBH_HoaDon.ID,
                        MaHoaDon = itemBH_HoaDon.MaHoaDon,
                        ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                        DienGiai = itemBH_HoaDon.DienGiai,
                        ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        PhaiThanhToan = itemBH_HoaDon.PhaiThanhToan,
                        TongChietKhau = itemBH_HoaDon.TongChietKhau,
                        TongGiamGia = itemBH_HoaDon.TongGiamGia,
                        TongChiPhi = itemBH_HoaDon.TongChiPhi,
                        TongTienHang = itemBH_HoaDon.TongTienHang,
                        ChoThanhToan = itemBH_HoaDon.ChoThanhToan,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        ID_DonVi = itemBH_HoaDon.ID_DonVi,
                        LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon,
                        TongTienThue = itemBH_HoaDon.TongTienThue
                    };
                    return ActionTrueData(objReturn);
                }
            }
        }

        [HttpPost, ActionName("PostBH_HoaDonTH")]
        [ResponseType(typeof(BH_HoaDon))]
        public IHttpActionResult PostBH_HoaDonTH([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDM_DoiTuong classDM_DoiTuong = new classDM_DoiTuong(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classHT_CauHinhPhanMem _classHTCH = new classHT_CauHinhPhanMem(db);
                classQuy_HoaDon _classQHD = new classQuy_HoaDon(db);
                ClassQuy_HoaDon_ChiTiet _classQHDCT = new ClassQuy_HoaDon_ChiTiet(db);

                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                //string tkNganHang = data["TKNganHang"].ToObject<string>();
                double dKHDaTra = data["KHDaTra"].ToObject<double>();
                double giatritienMat = data["GiaTriTienMat"].ToObject<double>();
                double giatriTienNH = data["GiaTriTienNH"].ToObject<double>();
                double giatriTienCK = data["GiaTriTienChuyenKhoan"].ToObject<double>();
                Guid IDTKPOS = Guid.Empty;
                Guid IDTKChuyenKhoan = Guid.Empty;
                if (data["IDTKPOS"] != null)
                {
                    IDTKPOS = data["IDTKPOS"].ToObject<Guid>();
                }
                if (data["IDTKChuyenKhoan"] != null)
                {
                    IDTKChuyenKhoan = data["IDTKChuyenKhoan"].ToObject<Guid>();
                }
                #region BH_HoaDon
                BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                itemBH_HoaDon.ID = Guid.NewGuid();

                string sMaHoaDon = string.Empty;
                if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                {
                    sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                }
                else
                {
                    sMaHoaDon = classhoadon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                }
                itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                itemBH_HoaDon.ID_ViTri = objHoaDon.ID_ViTri;
                itemBH_HoaDon.ID_HoaDon = objHoaDon.ID_HoaDon;
                itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? new Guid("00000000-0000-0000-0000-000000000002") : objHoaDon.ID_DoiTuong;
                itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                itemBH_HoaDon.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia;
                itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;
                itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                itemBH_HoaDon.NgayTao = objHoaDon.NgayLapHoaDon;
                itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                itemBH_HoaDon.TyGia = 1;
                itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon; // Hoa don nhap hang
                                                                 // neu luu tam => cho thanh toan == false
                itemBH_HoaDon.TongChietKhau = 0;
                itemBH_HoaDon.TongTienThue = 0;

                #endregion

                string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);
                #region "Insert Quy_Hoadon"
                DM_DoiTuong dt0 = classDM_DoiTuong.Select_DoiTuong(itemBH_HoaDon.ID_DoiTuong ?? Guid.Empty);
                string sNguoiNop = string.Empty;
                string ghiChu = string.Empty;
                if (dt0 != null)
                {
                    sNguoiNop = dt0.TenDoiTuong;
                }

                if (giatriTienNH != 0)
                {
                    ghiChu = " NCC trả tiền qua NH: " + (giatriTienNH + giatriTienCK);
                }
                if (dKHDaTra != 0 && objHoaDon.ChoThanhToan == false)
                {
                    Quy_HoaDon qhd = new Quy_HoaDon
                    {
                        ID = Guid.NewGuid(),
                        LoaiHoaDon = 11, // Phieu chi
                        MaHoaDon = "SQPT" + sMaHoaDon,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                        NguoiNopTien = sNguoiNop, // Khachhang or Khach le
                        NguoiTao = itemBH_HoaDon.NguoiTao,
                        NoiDungThu = itemBH_HoaDon.DienGiai + ghiChu,
                        TongTienThu = dKHDaTra, // khach da tra
                        ID_DonVi = itemBH_HoaDon.ID_DonVi
                    };
                    _classQHD.Add_SoQuy(qhd);
                    #endregion;
                    //add nhật ký hoạt động
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ChucNang = "Phiếu thu";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Tạo phiếu thu : " + qhd.MaHoaDon + ", cho đơn nhập hàng:" + itemBH_HoaDon.MaHoaDon + ", với giá trị: " + string.Format("{0:n0}", dKHDaTra).Replace(".", ",") + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", ghi chú: " + qhd.NoiDungThu;
                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo phiếu thu:  <a onclick=\"FindSoQuy('" + qhd.MaHoaDon + "')\"> " + qhd.MaHoaDon + "</a>, cho đơn trả hàng nhập : <a onclick=\"FindMaHDTHN('" + itemBH_HoaDon.MaHoaDon + "')\">" + itemBH_HoaDon.MaHoaDon + "</a> với giá trị: " + string.Format("{0:n0}", dKHDaTra).Replace(".", ",") + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", ghi chú: " + qhd.NoiDungThu;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    #region "Insert Quy_Hoadon_ChiTiet (QuyHD(1)- QuyHD_CT(1)) "

                    if (giatritienMat == 0)
                    {
                        if (giatriTienNH == 0)
                        {
                            if (giatriTienCK == 0)
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = dKHDaTra;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = dKHDaTra;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                            else
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = giatriTienCK;
                                qct.TienMat = 0;
                                qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatriTienCK;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                        }
                        else if (giatriTienCK == 0)
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = giatriTienNH;
                            qct.ID_TaiKhoanNganHang = IDTKPOS;
                            qct.TienMat = 0;
                            qct.TienThu = giatriTienNH;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                        }
                        else
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = giatriTienNH;
                            qct.ID_TaiKhoanNganHang = IDTKPOS;
                            qct.TienMat = 0;
                            qct.TienThu = giatriTienNH;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienCK;
                            qct1.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct1.TienMat = 0;
                            qct1.TienThu = giatriTienCK;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                        }
                    }
                    else
                    {
                        if (giatriTienNH == 0)
                        {
                            if (giatriTienCK == 0)
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = giatritienMat;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatritienMat;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);
                            }
                            else
                            {
                                Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                                qct.ID = Guid.NewGuid();
                                qct.ID_HoaDon = qhd.ID;
                                qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct.TienGui = 0;
                                qct.TienMat = giatritienMat;
                                //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct.TienThu = giatritienMat;
                                qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                                Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                                qct1.ID = Guid.NewGuid();
                                qct1.ID_HoaDon = qhd.ID;
                                qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                                qct1.TienGui = giatriTienCK;
                                qct1.TienMat = 0;
                                qct1.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                                qct1.TienThu = giatriTienCK;
                                qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                                qct1.GhiChu = "Đã thanh toán";
                                _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                            }
                        }
                        else if (giatriTienCK == 0)
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = 0;
                            qct.TienMat = giatritienMat;
                            //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct.TienThu = giatritienMat;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienNH;
                            qct1.TienMat = 0;
                            qct1.ID_TaiKhoanNganHang = IDTKPOS;
                            qct1.TienThu = giatriTienNH;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);
                        }
                        else
                        {
                            Quy_HoaDon_ChiTiet qct = new Quy_HoaDon_ChiTiet();
                            qct.ID = Guid.NewGuid();
                            qct.ID_HoaDon = qhd.ID;
                            qct.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct.TienGui = 0;
                            qct.TienMat = giatritienMat;
                            //qct.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct.TienThu = giatritienMat;
                            qct.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct);

                            Quy_HoaDon_ChiTiet qct1 = new Quy_HoaDon_ChiTiet();
                            qct1.ID = Guid.NewGuid();
                            qct1.ID_HoaDon = qhd.ID;
                            qct1.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct1.TienGui = giatriTienNH;
                            qct1.TienMat = 0;
                            qct1.ID_TaiKhoanNganHang = IDTKPOS;
                            qct1.TienThu = giatriTienNH;
                            qct1.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct1.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct1);

                            Quy_HoaDon_ChiTiet qct2 = new Quy_HoaDon_ChiTiet();
                            qct2.ID = Guid.NewGuid();
                            qct2.ID_HoaDon = qhd.ID;
                            qct2.ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong;
                            qct2.TienGui = giatriTienCK;
                            qct2.TienMat = 0;
                            qct2.ID_TaiKhoanNganHang = IDTKChuyenKhoan;
                            qct2.TienThu = giatriTienCK;
                            qct2.ID_HoaDonLienQuan = itemBH_HoaDon.ID;
                            qct2.GhiChu = "Đã thanh toán";
                            _classQHDCT.Add_ChiTietQuyHoaDon(qct2);
                        }
                    }
                    #endregion;
                }
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    #region BH_ChiTietHoaDon
                    var dem = objCTHoaDon.Count;
                    string listCT = "";
                    string listND = "";
                    foreach (var item in objCTHoaDon)
                    {
                        dem = dem - 1;
                        DonViQuiDoi dvqd = _classDVQD.Get(id => id.ID == item.ID_DonViQuiDoi);
                        DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                        listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + item.SoLuong + " Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",") + "</br>";
                        listND = listND + "- " + dvqd.MaHangHoa + ":" + item.SoLuong + " Đơn giá: " + string.Format("{0:n0}", item.DonGia).Replace(".", ",");
                        DM_HangHoa hanghoa = db.DM_HangHoa.Where(p => p.ID == dvqd.ID_HangHoa).FirstOrDefault();
                        HT_CauHinhPhanMem cauhinh = _classHTCH.SelectByIDDonVi(objHoaDon.ID_DonVi);
                        double? soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi);
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            SoThuTu = dem,
                            DonGia = item.DonGia,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            SoLuong = item.SoLuong,
                            ThanhTien = item.ThanhTien,
                            TienChietKhau = item.TienChietKhau,
                            //GiaVon = dvqd.GiaVon
                            GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                            ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                            GhiChu = item.GhiChu
                        };
                        if (objHoaDon.ChoThanhToan == false)
                        {
                            if (soluongton - item.SoLuong <= hanghoa.TonToiThieu)
                            {
                                HT_ThongBao httb = new HT_ThongBao();
                                httb.ID = Guid.NewGuid();
                                httb.ID_DonVi = objHoaDon.ID_DonVi;
                                httb.LoaiThongBao = 0; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                                httb.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httb.ID + "')\"> Hàng hóa <a onclick=\"loadthongbao('1', '" + dvqd.MaHangHoa + "','" + httb.ID + "')\">" + " <span class=\"blue\">" + dvqd.MaHangHoa + " </span>" + " </a> đã hết số lượng tồn kho. Xin vui lòng nhập thêm để tiếp tục kinh doanh </p>";
                                httb.NgayTao = DateTime.Now;
                                httb.NguoiDungDaDoc = "";
                                db.HT_ThongBao.Add(httb);
                                db.SaveChanges();
                            }
                        }

                        strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                    }
                    Guid? idhoadon = null;
                    if (itemBH_HoaDon.ChoThanhToan == false)
                    {
                        idhoadon = itemBH_HoaDon.ID;
                    }
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ID_HoaDon = idhoadon;
                    hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                    hT_NhatKySuDung.LoaiHoaDon = 7;
                    hT_NhatKySuDung.ChucNang = "Trả hàng nhập";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Tạo mới phiếu trả hàng nhập: " + itemBH_HoaDon.MaHoaDon + ", thời gian:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: " + listND;
                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu trả hàng nhập:  <a onclick=\"FindMaHDTHN('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, thời gian:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: </br> " + listCT;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    #endregion
                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = itemBH_HoaDon.ID,
                        MaHoaDon = itemBH_HoaDon.MaHoaDon,
                        ID_NhanVien = itemBH_HoaDon.ID_NhanVien,
                        ID_ViTri = itemBH_HoaDon.ID_ViTri,
                        DienGiai = itemBH_HoaDon.DienGiai,
                        ID_DoiTuong = itemBH_HoaDon.ID_DoiTuong,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        PhaiThanhToan = itemBH_HoaDon.PhaiThanhToan,
                        TongChietKhau = itemBH_HoaDon.TongChietKhau,
                        TongGiamGia = itemBH_HoaDon.TongGiamGia,
                        TongChiPhi = itemBH_HoaDon.TongChiPhi,
                        TongTienHang = itemBH_HoaDon.TongTienHang,
                        ChoThanhToan = itemBH_HoaDon.ChoThanhToan,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        ID_DonVi = itemBH_HoaDon.ID_DonVi,
                        LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon,
                        TongTienThue = itemBH_HoaDon.TongTienThue
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult Post_TraHangNhap([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                        string err = string.Empty;
                        var ngaylapHD = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);

                        #region BH_HoaDon
                        objHoaDon.ID = Guid.NewGuid();

                        string sMaHoaDon = string.Empty;
                        if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                        {
                            sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }
                        else
                        {
                            bool exist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon);
                            if (exist)
                            {
                                return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                            }
                            sMaHoaDon = classhoadon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                        }
                        objHoaDon.MaHoaDon = sMaHoaDon;
                        var idDoiTuong = objHoaDon.ID_DoiTuong;
                        if (idDoiTuong == null || idDoiTuong == Guid.Empty)
                        {
                            idDoiTuong = new Guid("00000000-0000-0000-0000-000000000002");
                        }
                        objHoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                        objHoaDon.ID_DoiTuong = idDoiTuong;
                        objHoaDon.NgayLapHoaDon = ngaylapHD;
                        objHoaDon.TyGia = 1;
                        err = classhoadon.Add_HoaDon(objHoaDon);
                        if (err != string.Empty)
                        {
                            return Json(new { res = false, mes = err });
                        }
                        #endregion

                        #region BH_HoaDon_ChiTiet
                        foreach (var item in objCTHoaDon)
                        {
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                SoThuTu = item.SoThuTu,
                                DonGia = item.DonGia,
                                ID_HoaDon = objHoaDon.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                ThanhToan = item.ThanhToan,
                                TienChietKhau = item.TienChietKhau,
                                PTChietKhau = item.PTChietKhau,
                                PTThue = item.PTThue,
                                TienThue = item.TienThue,
                                GiaVon = item.GiaVon,
                                ID_LoHang = item.ID_LoHang,
                                GhiChu = item.GhiChu
                            };
                            err = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        }
                        #endregion
                        db.SaveChanges();
                        trans.Commit();

                        string diary = string.Empty;
                        try
                        {
                            diary = db.Database.SqlQuery<string>(" SELECT dbo.Diary_GetInforOldInvoice('" + objHoaDon.ID + "')").First();
                        }
                        catch (Exception)
                        {
                        }
                        return Json(new
                        {
                            res = true,
                            data = new { objHoaDon.ID, NgayLapHoaDon = ngaylapHD, MaHoaDon = sMaHoaDon, Diary = diary }
                        });
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
        public IHttpActionResult Update_TraHangNhap([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                        var hdOld = db.BH_HoaDon.Find(objHoaDon.ID);
                        var ngaylapOld = hdOld.NgayLapHoaDon;
                        string err = string.Empty, diary = string.Empty, diaryOld = string.Empty;
                        string sMaHoaDon = string.Empty;

                        var ngaylapHD = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                        List<BH_HoaDon_ChiTiet> ctDelete_newID = new List<BH_HoaDon_ChiTiet>();

                        #region "Get cthd old was delete"
                        var idOldCustomer = hdOld.ID_DoiTuong;
                        var inforOld = string.Empty;
                        if (hdOld.ChoThanhToan == false)
                        {
                            var cthdOld = classhoadonchitiet.Gets(x => x.ID_HoaDon == objHoaDon.ID); // get cthd old
                                                                                                     // if date new < date old: date old = date new - milisencond
                            string sDateOld = hdOld.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                            string sDateNew = objHoaDon.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                            if (string.Compare(sDateNew, sDateOld) == 0)
                            {
                                ngaylapHD = hdOld.NgayLapHoaDon;
                            }
                            // compare cthd old & new --> get cthd was delete
                            var cthdDelete = (from ctold in cthdOld
                                              join ctnew in objCTHoaDon on
                                              new { ctold.ID_DonViQuiDoi, ctold.ID_LoHang }
                                              equals new { ctnew.ID_DonViQuiDoi, ctnew.ID_LoHang }
                                              into ctDelete
                                              from de in ctDelete.DefaultIfEmpty()
                                              where de == null
                                              select ctold).ToList();

                            ctDelete_newID = cthdDelete.Select(x =>
                          new BH_HoaDon_ChiTiet
                          {
                              ID = Guid.NewGuid(),
                              ID_HoaDon = x.ID_HoaDon,
                              ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                              ID_LoHang = x.ID_LoHang,
                              SoLuong = x.SoLuong,
                              PTChietKhau = x.PTChietKhau,
                              TienChietKhau = x.TienChietKhau,
                              ThanhTien = x.ThanhTien,
                              ThanhToan = x.ThanhToan,
                              ChatLieu = "5", // ct delete assign chatlie="5" !important
                              GiaVon = x.GiaVon,
                              TienThue = x.TienThue,
                              PTChiPhi = x.PTChiPhi,
                              TienChiPhi = x.TienChiPhi,
                          }).ToList();
                        }
                        #endregion

                        try
                        {
                            diaryOld = db.Database.SqlQuery<string>(" SELECT dbo.Diary_GetInforOldInvoice('" + objHoaDon.ID + "')").First();
                        }
                        catch (Exception ex)
                        {
                            CookieStore.WriteLog("Update_TraHangNhap_DiaryOld " + ex.InnerException + ex.Message);
                        }

                        if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                        {
                            sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }
                        else
                        {
                            bool exist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, objHoaDon.ID);
                            if (exist)
                            {
                                return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                            }
                            sMaHoaDon = objHoaDon.MaHoaDon;
                        }
                        objHoaDon.MaHoaDon = sMaHoaDon;
                        objHoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                        objHoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? new Guid("00000000-0000-0000-0000-000000000002") : objHoaDon.ID_DoiTuong;
                        objHoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                        err = classhoadon.Update_HoaDon(objHoaDon);

                        if (err != string.Empty)
                        {
                            return Json(new { res = false, mes = err });
                        }
                        err = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);

                        if (objHoaDon.ID_DoiTuong != idOldCustomer)
                        {
                            ClassQuy_HoaDon_ChiTiet classQuyCT = new ClassQuy_HoaDon_ChiTiet(db);
                            err += classQuyCT.UpdateIDKhachHang_inSoQuy(objHoaDon.ID);
                        }

                        if (err != string.Empty)
                        {
                            return Json(new { res = false, mes = err });
                        }

                        #region BH_ChiTietHoaDon
                        foreach (var item in objCTHoaDon)
                        {
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                SoThuTu = item.SoThuTu,
                                DonGia = item.DonGia,
                                ID_HoaDon = objHoaDon.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                ThanhToan = item.ThanhToan,
                                TienChietKhau = item.TienChietKhau,
                                PTChietKhau = item.PTChietKhau,
                                GiaVon = item.GiaVon,
                                ID_LoHang = item.ID_LoHang,
                                GhiChu = item.GhiChu,
                                PTThue = item.PTThue,
                                TienThue = item.TienThue
                            };
                            err = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        }

                        // insert again cthd old was delete (ChatLieu = 5) into hdUpdate --> caculator TonLuyKe
                        if (hdOld.ChoThanhToan == false)
                        {
                            err = classhoadonchitiet.Add_ChiTietHoaDon(ctDelete_newID);
                            if (err != string.Empty)
                            {
                                return Json(new
                                {
                                    res = false,
                                    mess = err,
                                });
                            }
                        }
                        #endregion
                        db.SaveChanges();
                        trans.Commit();

                        try
                        {
                            diary = db.Database.SqlQuery<string>(" SELECT dbo.Diary_GetInforOldInvoice('" + objHoaDon.ID + "')").First();
                        }
                        catch (Exception ex)
                        {
                            CookieStore.WriteLog("Update_TraHangNhap_DiaryNew " + ex.InnerException + ex.Message);
                        }
                        return Json(new
                        {
                            res = true,
                            data = new
                            {
                                objHoaDon.ID,
                                MaHoaDon = sMaHoaDon,
                                NgayLapHoaDon = objHoaDon.NgayLapHoaDon,
                                NgayLapHoaDonOld = ngaylapOld,
                                Diary = diary,
                                DiaryOld = diaryOld,
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.Message + ex.InnerException);
                    }
                }
            }
        }

        #region ChiPhi GiaCong
        [HttpPost, HttpGet]
        public IHttpActionResult CTHD_PostPutChiPhi([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<BH_HoaDon_ChiPhi> lstData = new List<BH_HoaDon_ChiPhi>();
                        List<Guid> arrIDHoaDon = new List<Guid>();
                        if (data["lstChiPhi"] != null)
                        {
                            lstData = data["lstChiPhi"].ToObject<List<BH_HoaDon_ChiPhi>>();

                            // only update ChiPhi to BH_HoaDon if update ChiPhi in BH_HoaDon_ChiPhi
                            if (data["arrIDHoaDon"] != null)
                            {
                                var dataGr = lstData.GroupBy(x => x.ID_HoaDon)
                                    .Select(x => new
                                    {
                                        ID_HoaDon = x.Key,
                                        TongChiPhi = x.Sum(o => o.ThanhTien)
                                    }).ToList();

                                foreach (var item in dataGr)
                                {
                                    BH_HoaDon hd = db.BH_HoaDon.Find(item.ID_HoaDon);
                                    if (hd != null)
                                    {
                                        hd.ChiPhi = item.TongChiPhi;
                                    }
                                }
                            }
                        }
                        if (data["arrIDHoaDon"] != null)
                        {
                            arrIDHoaDon = data["arrIDHoaDon"].ToObject<List<Guid>>();
                        }
                        // delete chiphi old & add again
                        var cpOld = db.BH_HoaDon_ChiPhi.Where(x => arrIDHoaDon.Contains(x.ID_HoaDon) == true);
                        db.BH_HoaDon_ChiPhi.RemoveRange(cpOld);

                        List<BH_HoaDon_ChiPhi> lstAdd = new List<BH_HoaDon_ChiPhi>();
                        foreach (var item in lstData)
                        {
                            item.ID = Guid.NewGuid();
                            lstAdd.Add(item);
                        }
                        db.BH_HoaDon_ChiPhi.AddRange(lstAdd);
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.Message + ex.InnerException);
                    }
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult CTHD_GetChiPhiDichVu([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<string> arrID = new List<string>();
                        List<string> arrVendor = new List<string>();
                        if (data["arrID"] != null)
                        {
                            arrID = data["arrID"].ToObject<List<string>>();
                        }
                        if (data["arrIDVendor"] != null)
                        {
                            arrVendor = data["arrIDVendor"].ToObject<List<string>>();
                        }
                        ClassBH_HoaDon_ChiTiet classHDCT = new ClassBH_HoaDon_ChiTiet(db);
                        List<ChiPhiDichVuDTO> lst = classHDCT.CTHD_GetChiPhiDichVu(arrID, arrVendor);
                        return ActionTrueData(lst);
                    }
                    catch (Exception ex)
                    {
                        return ActionFalseNotData(ex.Message + ex.InnerException);
                    }
                }
            }
        }

        #endregion

        #region insert chuyen hang
        [HttpPost, HttpGet]
        public IHttpActionResult Insert_ChuyenHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classDM_DonVi _classDMDV = new classDM_DonVi(db);

                try
                {
                    BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                    Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                    List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                    string err = string.Empty;

                    #region BH_HoaDon
                    int yeucau = Int32.Parse(objHoaDon.YeuCau);
                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                    itemBH_HoaDon.ID = Guid.NewGuid();
                    string sMaHoaDon = string.Empty;
                    if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                    {
                        sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                    }
                    else
                    {
                        bool exist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon);
                        if (exist)
                        {
                            return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                        }
                        sMaHoaDon = classhoadon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                    }
                    itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                    itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                    itemBH_HoaDon.ID_CheckIn = objHoaDon.ID_CheckIn;
                    itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon;
                    itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                    itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                    itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                    itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                    itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                    itemBH_HoaDon.PhaiThanhToan = 0;
                    itemBH_HoaDon.TongGiamGia = 0;
                    itemBH_HoaDon.TongChiPhi = 0;
                    itemBH_HoaDon.TongChietKhau = 0;
                    itemBH_HoaDon.TongTienThue = 0;
                    itemBH_HoaDon.ChoThanhToan = false;
                    itemBH_HoaDon.NgayTao = DateTime.Now;
                    itemBH_HoaDon.TyGia = 1;
                    err = classhoadon.Add_HoaDon(itemBH_HoaDon);
                    #endregion

                    if (err == string.Empty)
                    {
                        Guid? idHoaDon = null;
                        if (yeucau == 1)
                        {
                            idHoaDon = itemBH_HoaDon.ID;// used to save diary (update TonKho)
                            HT_ThongBao httbCH = new HT_ThongBao();
                            httbCH.ID = Guid.NewGuid();
                            httbCH.ID_DonVi = objHoaDon.ID_CheckIn.Value;
                            httbCH.LoaiThongBao = 1; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                            httbCH.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httbCH.ID + "')\"> Đơn chuyển hàng <a onclick=\"loadthongbao('2', '" + sMaHoaDon + "','" + httbCH.ID + "')\">" + "<span class=\"blue\">" + sMaHoaDon + " </span>" + " </a> đang đợi nhận. Vui lòng theo dõi và nhận đơn hàng để thêm hàng hóa vào kho </p>";
                            httbCH.NgayTao = DateTime.Now;
                            httbCH.NguoiDungDaDoc = "";
                            db.HT_ThongBao.Add(httbCH);
                            db.SaveChanges();
                        }

                        #region BH_ChiTietHoaDon
                        string listCT = "";
                        string listND = "";
                        foreach (var item in objCTHoaDon)
                        {
                            DonViQuiDoi dvqd = _classDVQD.Get(id => id.ID == item.ID_DonViQuiDoi);
                            DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();

                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                DonGia = item.GiaVon.Value,
                                ID_HoaDon = itemBH_HoaDon.ID,
                                SoLuong = item.SoLuong,
                                SoThuTu = item.SoThuTu,
                                TienChietKhau = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                GhiChu = item.GhiChu,
                                GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                                ChatLieu = "",
                                MauSac = "",
                                KichCo = "",
                                PTChietKhau = 0,
                                TienThue = 0,
                                PTChiPhi = 0,
                                TienChiPhi = 0,
                                ThanhToan = 0,
                                An_Hien = true,
                                ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                            };

                            var malo = item.MaLoHang != null && item.MaLoHang != string.Empty ? string.Concat("(Lô: ", item.MaLoHang, ") : Số lượng: ") : " Số lượng:  ";
                            listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> " + malo +
                                item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";

                            err += classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        }
                        listND = string.Concat(", Tổng giá trị chuyển: ", string.Format("{0:n0}", itemBH_HoaDon.TongTienHang).Replace(".", ","));

                        //classhoadonchitiet.UpdateTonKhoGiaVon_whenUpdateCTHD(itemBH_HoaDon.ID, objHoaDon.ID_DonVi, itemBH_HoaDon.NgayLapHoaDon);
                        #endregion

                        string fromto = string.Concat(", từ chi nhánh:", _classDMDV.Get(p => p.ID == objHoaDon.ID_DonVi).TenDonVi,
                            ", tới chi nhánh: ", _classDMDV.Get(p => p.ID == objHoaDon.ID_CheckIn).TenDonVi,
                            ", thời gian: ", itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss"));
                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                        hT_NhatKySuDung.ID = Guid.NewGuid();
                        hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                        hT_NhatKySuDung.ID_HoaDon = idHoaDon;
                        hT_NhatKySuDung.ThoiGianUpdateGV = yeucau == 1 ? itemBH_HoaDon.NgayLapHoaDon : (DateTime?)null;
                        hT_NhatKySuDung.LoaiHoaDon = 10;
                        hT_NhatKySuDung.ChucNang = "Chuyển hàng";
                        hT_NhatKySuDung.ThoiGian = DateTime.Now;
                        hT_NhatKySuDung.NoiDung = string.Concat("Chuyển hàng - ", yeucau == 1 ? "Thêm mới" : "Tạm lưu", ", Mã phiếu: " + itemBH_HoaDon.MaHoaDon, fromto, listND);
                        hT_NhatKySuDung.NoiDungChiTiet = string.Concat("Chuyển hàng - ", yeucau == 1 ? "Thêm mới" : "Tạm lưu", ", Mã phiếu:  <a onclick=\"FindMaHDCH('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, ", fromto, ", bao gồm: </br> " + listCT);
                        hT_NhatKySuDung.LoaiNhatKy = 1;
                        hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                        db.HT_NhatKySuDung.Add(hT_NhatKySuDung);
                        db.SaveChanges();
                        new SaveDiary().AddQueueJob(hT_NhatKySuDung);

                        BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                        {
                            ID = itemBH_HoaDon.ID,
                            MaHoaDon = itemBH_HoaDon.MaHoaDon,
                            DienGiai = itemBH_HoaDon.DienGiai,
                            NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                            PhaiThanhToan = itemBH_HoaDon.PhaiThanhToan,
                            TongChietKhau = itemBH_HoaDon.TongChietKhau,
                            TongGiamGia = itemBH_HoaDon.TongGiamGia,
                            TongChiPhi = itemBH_HoaDon.TongChiPhi,
                            TongTienHang = itemBH_HoaDon.TongTienHang,
                            ChoThanhToan = itemBH_HoaDon.ChoThanhToan,
                            NgayTao = itemBH_HoaDon.NgayTao,
                            NguoiTao = itemBH_HoaDon.NguoiTao,
                            ID_DonVi = itemBH_HoaDon.ID_DonVi,
                            LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon,
                            TongTienThue = itemBH_HoaDon.TongTienThue,
                            YeuCau = itemBH_HoaDon.YeuCau,
                            ID_CheckIn = itemBH_HoaDon.ID_CheckIn
                        };
                        return Json(new { res = true, data = objReturn });
                    }
                    else
                    {
                        return Json(new { res = false, mes = err });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = string.Concat(e.InnerException, e.Message) });
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult Update_ChuyenHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                string sMaHoaDon = string.Empty;

                try
                {
                    int nhanhang = data["IsNhanHang"].ToObject<int>();// 0.chuyenhang, 1.nhanhang
                    BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                    Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                    List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                    int yeucau = int.Parse(objHoaDon.YeuCau);
                    string err = string.Empty;

                    if (!string.IsNullOrEmpty(objHoaDon.MaHoaDon))
                    {
                        bool exist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, objHoaDon.ID);
                        if (exist)
                        {
                            return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                        }
                        sMaHoaDon = objHoaDon.MaHoaDon;
                    }
                    else
                    {
                        sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                    }

                    #region BH_HoaDon
                    BH_HoaDon itemBH_HoaDon = classhoadon.Select_HoaDon(objHoaDon.ID);
                    itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                    itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                    itemBH_HoaDon.NguoiTao = itemBH_HoaDon.NguoiTao;
                    itemBH_HoaDon.KhuyenMai_GhiChu = yeucau == 4 ? objHoaDon.NguoiTao : "";
                    itemBH_HoaDon.NgaySua = yeucau == 4 ? objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond) : (DateTime?)null;
                    itemBH_HoaDon.PhaiThanhToan = 0;
                    itemBH_HoaDon.TongGiamGia = 0;
                    itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                    itemBH_HoaDon.TongTienHang = yeucau == 1 ? objHoaDon.TongTienHang : itemBH_HoaDon.TongTienHang;// if update tamluu to chuyenhang themmoi ==> assign again TongTienHang
                    itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                    itemBH_HoaDon.NguoiSua = objHoaDon.NguoiSua;
                    err = classhoadon.Update_HoaDon_Chuyenhang(itemBH_HoaDon);
                    #endregion

                    string listCT = "";
                    string listND = "";
                    bool thongbao = nhanhang == 0 && yeucau == 1;
                    string sGia = nhanhang == 0 ? " Giá chuyển: " : " Giá nhận: ";
                    if (err == string.Empty)
                    {
                        #region BH_ChiTietHoaDon
                        foreach (var item in objCTHoaDon)
                        {
                            DonViQuiDoi dvqd = _classDVQD.Get(x => x.ID == item.ID_DonViQuiDoi);
                            BH_HoaDon_ChiTiet ctHoaDon = classhoadonchitiet.Get(p => p.ID == item.ID);
                            DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == itemBH_HoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                            if (ctHoaDon != null)
                            {
                                ctHoaDon.SoLuong = yeucau == 1 ? item.SoLuong : ctHoaDon.SoLuong;// if tamluu sau do themmoi  --> change soluong
                                ctHoaDon.ThanhTien = yeucau == 1 ? item.ThanhTien : ctHoaDon.ThanhTien;
                                ctHoaDon.TienChietKhau = item.SoLuong; // !!important: set TienChietKhau above SoLuong
                                ctHoaDon.GiaVon_NhanChuyenHang = dmgv != null ? dmgv.GiaVon : 0;
                                ctHoaDon.GhiChu = item.GhiChu;
                                err = classhoadonchitiet.Update_ChiTietHoaDon(ctHoaDon);
                            }
                            else
                            {
                                BH_HoaDon_ChiTiet ctHoaDonNew = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    DonGia = item.GiaVon.Value,
                                    ID_HoaDon = itemBH_HoaDon.ID,
                                    SoLuong = item.SoLuong,
                                    SoThuTu = item.SoThuTu,
                                    TienChietKhau = item.SoLuong,
                                    ThanhTien = item.ThanhTien,
                                    GhiChu = item.GhiChu,
                                    GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                                    ChatLieu = "",
                                    MauSac = "",
                                    KichCo = "",
                                    PTChietKhau = 0,
                                    TienThue = 0,
                                    PTChiPhi = 0,
                                    TienChiPhi = 0,
                                    ThanhToan = 0,
                                    An_Hien = true,
                                    ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                                };
                                err = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDonNew);
                            }

                            if (thongbao)
                            {
                                // only insert if chuyenhang (not tamluu)
                                DM_HangHoa hanghoa = db.DM_HangHoa.Where(p => p.ID == dvqd.ID_HangHoa).FirstOrDefault();
                                double? soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi);
                                if (soluongton - item.SoLuong * dvqd.TyLeChuyenDoi <= hanghoa.TonToiThieu)
                                {
                                    HT_ThongBao httb = new HT_ThongBao();
                                    httb.ID = Guid.NewGuid();
                                    httb.ID_DonVi = objHoaDon.ID_DonVi;
                                    httb.LoaiThongBao = 0; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                                    httb.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httb.ID + "')\"> Hàng hóa <a onclick=\"loadthongbao('1', '" + dvqd.MaHangHoa + "','" + httb.ID + "')\">" + "<span class=\"blue\">" + dvqd.MaHangHoa + " </span>" + " </a> đã hết số lượng tồn kho. Xin vui lòng nhập thêm để tiếp tục kinh doanh </p>";
                                    httb.NgayTao = DateTime.Now;
                                    httb.NguoiDungDaDoc = "";
                                    db.HT_ThongBao.Add(httb);
                                    db.SaveChanges();
                                }
                            }

                            var malo = item.MaLoHang != null && item.MaLoHang != string.Empty ? string.Concat("(Lô: ", item.MaLoHang, ") : Số lượng: ") : " Số lượng:  ";
                            listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> " + malo +
                                item.SoLuong + sGia + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";
                        }
                        listND = string.Concat(nhanhang == 0 ? "Tổng giá trị chuyển: " + string.Format("{0:n0}", itemBH_HoaDon.TongTienHang) :
                            "Tổng giá trị nhận: " + string.Format("{0:n0}", itemBH_HoaDon.TongChiPhi));
                        #endregion

                        #region HT_NhatKySuDung
                        Guid? idhoadon = null;
                        DateTime? thoiGianUpdateGV = null;
                        var ngaynhan = string.Empty;

                        string strFirst = string.Empty;
                        string mahoadon = itemBH_HoaDon.MaHoaDon;
                        string linkMaHD = string.Concat("<a onclick=\"FindMaHDCH('", mahoadon, "')\"> ");
                        string fromto = string.Concat(", từ chi nhánh:", _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_DonVi).TenDonVi,
                            ", tới chi nhánh: " + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_CheckIn).TenDonVi,
                             ", ngày chuyển: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss"));

                        switch (yeucau)
                        {
                            case 1:
                                strFirst = "Thêm mới phiếu chuyển hàng từ phiếu tạm ";
                                idhoadon = itemBH_HoaDon.ID;
                                thoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                                break;
                            case 2:
                                if (nhanhang == 0)
                                {
                                    strFirst = "Cập nhật - Tạm lưu phiếu chuyển hàng ";
                                }
                                else
                                {
                                    strFirst = "Nhận hàng - Tạm lưu, Mã phiếu ";
                                }
                                break;
                            case 4:
                                strFirst = "Nhận hàng từ phiếu chuyển hàng ";
                                ngaynhan = string.Concat(", ngày nhận: ", itemBH_HoaDon.NgaySua.Value.ToString("dd/MM/yyyy HH:mm:ss"));
                                idhoadon = itemBH_HoaDon.ID;
                                thoiGianUpdateGV = itemBH_HoaDon.NgaySua;

                                HT_ThongBao httbCH = new HT_ThongBao();
                                httbCH.ID = Guid.NewGuid();
                                httbCH.ID_DonVi = itemBH_HoaDon.ID_DonVi;
                                httbCH.LoaiThongBao = 1; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                                httbCH.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httbCH.ID + "')\"> Đơn chuyển hàng <a onclick=\"loadthongbao('2', '" + mahoadon + "','" + httbCH.ID + "')\">" + " <span class=\"blue\">" + mahoadon + " </span>" + " </a> đã được nhận thành công </p>";
                                httbCH.NgayTao = DateTime.Now;
                                httbCH.NguoiDungDaDoc = "";
                                db.HT_ThongBao.Add(httbCH);
                                db.SaveChanges();
                                break;
                        }
                        HT_NhatKySuDung nhatky = new HT_NhatKySuDung();
                        nhatky.ID = Guid.NewGuid();
                        nhatky.LoaiNhatKy = 1;
                        nhatky.ChucNang = "Chuyển hàng";
                        nhatky.ThoiGian = DateTime.Now;
                        nhatky.ID_NhanVien = idnhanvien;
                        nhatky.ID_HoaDon = idhoadon;
                        nhatky.LoaiHoaDon = 10;
                        nhatky.ThoiGianUpdateGV = thoiGianUpdateGV;
                        nhatky.ID_DonVi = nhanhang == 1 ? itemBH_HoaDon.ID_CheckIn.Value : itemBH_HoaDon.ID_DonVi;
                        nhatky.NoiDung = string.Concat(strFirst, mahoadon, fromto, ngaynhan, ", " + listND);
                        nhatky.NoiDungChiTiet = string.Concat(strFirst, linkMaHD, mahoadon, " </a>",
                            yeucau == 4 ? " (Đang chuyển-> Đã nhận)" : string.Empty,
                            fromto, ngaynhan, ", bao gồm: </br> " + listCT);
                        db.HT_NhatKySuDung.Add(nhatky);
                        db.SaveChanges();
                        new SaveDiary().AddQueueJob(nhatky);

                        //if (yeucau != 2)
                        //{
                        //    if (yeucau == 1)
                        //    {
                        //        classhoadonchitiet.UpdateTonKhoGiaVon_whenUpdateCTHD(itemBH_HoaDon.ID, nhatky.ID_DonVi, itemBH_HoaDon.NgayLapHoaDon);
                        //    }
                        //    else
                        //    {
                        //        // nhanhang: run douple: update chinhanhnhan --> chinhanhchuyen
                        //        classhoadonchitiet.UpdateTonKhoGiaVon_whenUpdateCTHD(itemBH_HoaDon.ID, nhatky.ID_DonVi, itemBH_HoaDon.NgaySua ?? DateTime.Now);
                        //        classhoadonchitiet.UpdateTonKhoGiaVon_whenUpdateCTHD(itemBH_HoaDon.ID, itemBH_HoaDon.ID_DonVi, itemBH_HoaDon.NgayLapHoaDon);
                        //    }
                        //}
                        #endregion

                        return Json(new
                        {
                            res = true,
                            data = new
                            {
                                ID = itemBH_HoaDon.ID,
                                MaHoaDon = itemBH_HoaDon.MaHoaDon,
                                ID_DonVi = itemBH_HoaDon.ID_DonVi,
                                ID_CheckIn = itemBH_HoaDon.ID_CheckIn,
                                TongTienHang = itemBH_HoaDon.TongTienHang,
                                TongChiPhi = itemBH_HoaDon.TongChiPhi,
                                NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                                NgaySua = itemBH_HoaDon.NgaySua,
                            }
                        });
                    }
                    else
                    {
                        return Json(new { res = false, mes = err });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = string.Concat(e.InnerException, e.Message) });
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult UpdateAgain_ChuyenHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classDM_DonVi _classDMDV = new classDM_DonVi(db);
                string sMaHoaDon = string.Empty;

                try
                {
                    BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                    List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                    if (!string.IsNullOrEmpty(objHoaDon.MaHoaDon))
                    {
                        bool exist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, objHoaDon.ID);
                        if (exist)
                        {
                            return Json(new { res = false, mes = "Mã hóa đơn đã tồn tại" });
                        }
                        sMaHoaDon = objHoaDon.MaHoaDon;
                    }
                    else
                    {
                        sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                    }
                    string err = string.Empty;

                    #region BH_HoaDon
                    BH_HoaDon itemBH_HoaDon = classhoadon.Select_HoaDon(objHoaDon.ID);
                    DateTime ngaylapHD = objHoaDon.NgayLapHoaDon;
                    DateTime dateOld = itemBH_HoaDon.NgayLapHoaDon;
                    string sDateOld = dateOld.ToString("yyyy-MM-dd HH:mm");
                    string sDateNew = objHoaDon.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                    if (string.Compare(sDateNew, sDateOld) == 0)
                    {
                        ngaylapHD = itemBH_HoaDon.NgayLapHoaDon;
                    }
                    itemBH_HoaDon.NgayLapHoaDon = ngaylapHD;
                    itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                    itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien;
                    itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                    itemBH_HoaDon.ID_CheckIn = objHoaDon.ID_CheckIn;
                    itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                    itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                    itemBH_HoaDon.KhuyenMai_GhiChu = string.Empty;
                    itemBH_HoaDon.NgaySua = null;
                    itemBH_HoaDon.PhaiThanhToan = 0;
                    itemBH_HoaDon.TongGiamGia = 0;
                    itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                    itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                    itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                    itemBH_HoaDon.ChoThanhToan = false;
                    db.SaveChanges();
                    #endregion

                    string listCT = "";
                    string listND = "";
                    if (err == string.Empty)
                    {
                        #region "Get cthd old was delete"
                        var cthdOld = classhoadonchitiet.Gets(x => x.ID_HoaDon == objHoaDon.ID); // get cthd old
                                                                                                 // compare cthd old & new --> get cthd was delete
                        var cthdDelete = (from ctold in cthdOld
                                          join ctnew in objCTHoaDon on
                                          new { ctold.ID_DonViQuiDoi, ctold.ID_LoHang }
                                          equals new { ctnew.ID_DonViQuiDoi, ctnew.ID_LoHang }
                                          into ctDelete
                                          from de in ctDelete.DefaultIfEmpty()
                                          where de == null
                                          select ctold).ToList();

                        var ctDelete_newID = cthdDelete.Select(x =>
                       new BH_HoaDon_ChiTiet
                       {
                           ID = Guid.NewGuid(),
                           ID_HoaDon = x.ID_HoaDon,
                           ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                           ID_LoHang = x.ID_LoHang,
                           SoLuong = x.SoLuong,
                           PTChietKhau = x.PTChietKhau,
                           TienChietKhau = x.TienChietKhau,
                           ThanhTien = x.ThanhTien,
                           ThanhToan = x.ThanhToan,
                           ChatLieu = "5", // ct delete assign chatlie="5" !important
                           GiaVon = x.GiaVon,
                           TienThue = x.TienThue,
                           PTChiPhi = x.PTChiPhi,
                           TienChiPhi = x.TienChiPhi,
                       }).ToList();
                        #endregion

                        #region BH_ChiTietHoaDon - delete & add again
                        err = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                        if (err != string.Empty)
                        {
                            return Json(new { res = false, mes = err });
                        }
                        err = classhoadonchitiet.Add_ChiTietHoaDon(ctDelete_newID);
                        if (err != string.Empty)
                        {
                            return Json(new { res = false, mes = err });
                        }
                        foreach (var item in objCTHoaDon)
                        {
                            DonViQuiDoi dvqd = _classDVQD.Get(id => id.ID == item.ID_DonViQuiDoi);
                            DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                            DM_HangHoa hanghoa = db.DM_HangHoa.Where(p => p.ID == dvqd.ID_HangHoa).FirstOrDefault();
                            double? soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi);

                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                DonGia = item.GiaVon.Value,
                                ID_HoaDon = itemBH_HoaDon.ID,
                                SoLuong = item.SoLuong,
                                SoThuTu = item.SoThuTu,
                                TienChietKhau = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                GhiChu = item.GhiChu,
                                GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                                ChatLieu = "",
                                MauSac = "",
                                KichCo = "",
                                PTChietKhau = 0,
                                TienThue = 0,
                                PTChiPhi = 0,
                                TienChiPhi = 0,
                                ThanhToan = 0,
                                An_Hien = true,
                                ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                            };
                            if (soluongton - item.SoLuong * dvqd.TyLeChuyenDoi <= hanghoa.TonToiThieu)
                            {
                                HT_ThongBao httb = new HT_ThongBao();
                                httb.ID = Guid.NewGuid();
                                httb.ID_DonVi = objHoaDon.ID_DonVi;
                                httb.LoaiThongBao = 0; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                                httb.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httb.ID + "')\"> Hàng hóa <a onclick=\"loadthongbao('1', '" + dvqd.MaHangHoa + "','" + httb.ID + "')\">" + "<span class=\"blue\">" + dvqd.MaHangHoa + " </span>" + " </a> đã hết số lượng tồn kho. Xin vui lòng nhập thêm để tiếp tục kinh doanh </p>";
                                httb.NgayTao = DateTime.Now;
                                httb.NguoiDungDaDoc = "";
                                db.HT_ThongBao.Add(httb);
                                db.SaveChanges();
                            }
                            var malo = item.MaLoHang != null && item.MaLoHang != string.Empty ? string.Concat("(Lô: ", item.MaLoHang, ") : Số lượng: ") : " Số lượng:  ";
                            listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> " + malo +
                                item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";

                            err += classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        }
                        listND = string.Concat(", Tổng giá trị chuyển: ", string.Format("{0:n0}", itemBH_HoaDon.TongTienHang).Replace(".", ","));
                        //classhoadonchitiet.UpdateTonKhoGiaVon_whenUpdateCTHD(itemBH_HoaDon.ID, itemBH_HoaDon.ID_DonVi, dateOld);// alway pass ngaylaphd old --> check in store getngaymin
                        #endregion

                        #region HT_NhatKySuDung
                        Guid? idhoadon = null;
                        DateTime? thoiGianUpdateGV = null;
                        var ngaynhan = string.Empty;

                        string strFirst = "Cập nhật lại phiếu chuyển hàng ";
                        string mahoadon = itemBH_HoaDon.MaHoaDon;
                        string linkMaHD = string.Concat("<a onclick=\"FindMaHDCH('", mahoadon, "')\"> ");
                        string fromto = string.Concat(", chi nhánh chuyển:", _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_DonVi).TenDonVi,
                            ", chi nhánh nhận: " + _classDMDV.Get(p => p.ID == itemBH_HoaDon.ID_CheckIn).TenDonVi,
                             ", ngày chuyển: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss"));

                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                        hT_NhatKySuDung.ID = Guid.NewGuid();
                        hT_NhatKySuDung.LoaiNhatKy = 2;
                        hT_NhatKySuDung.ChucNang = "Chuyển hàng";
                        hT_NhatKySuDung.ThoiGian = DateTime.Now;
                        hT_NhatKySuDung.ID_NhanVien = itemBH_HoaDon.ID_NhanVien;
                        hT_NhatKySuDung.ID_HoaDon = itemBH_HoaDon.ID;
                        hT_NhatKySuDung.LoaiHoaDon = 10;
                        hT_NhatKySuDung.ThoiGianUpdateGV = dateOld;
                        hT_NhatKySuDung.ID_DonVi = itemBH_HoaDon.ID_DonVi;
                        hT_NhatKySuDung.NoiDung = string.Concat(strFirst, mahoadon, fromto, ngaynhan, ", " + listND);
                        hT_NhatKySuDung.NoiDungChiTiet = string.Concat(strFirst, linkMaHD, mahoadon, " </a>",
                            fromto, ngaynhan, ", bao gồm: </br> " + listCT);
                        db.HT_NhatKySuDung.Add(hT_NhatKySuDung);
                        db.SaveChanges();
                        new SaveDiary().AddQueueJob(hT_NhatKySuDung);
                        #endregion

                        return Json(new
                        {
                            res = true,
                            data = new
                            {
                                ID = itemBH_HoaDon.ID,
                                MaHoaDon = itemBH_HoaDon.MaHoaDon,
                                ID_DonVi = itemBH_HoaDon.ID_DonVi,
                                ID_CheckIn = itemBH_HoaDon.ID_CheckIn,
                                TongTienHang = itemBH_HoaDon.TongTienHang,
                                TongChiPhi = itemBH_HoaDon.TongChiPhi,
                                NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                                NgaySua = itemBH_HoaDon.NgaySua,
                            }
                        });
                    }
                    else
                    {
                        return Json(new { res = false, mes = err });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = string.Concat(e.InnerException, e.Message) });
                }
            }
        }

        // POST: api/BH_HoaDonAPI
        [HttpPost, HttpGet]
        public IHttpActionResult PostBH_HDChuyenHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                classDM_DonVi _classDMDV = new classDM_DonVi(db);

                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                #region BH_HoaDon
                BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                itemBH_HoaDon.ID = Guid.NewGuid();

                string sMaHoaDon = string.Empty;
                if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                {
                    sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                }
                else
                {
                    sMaHoaDon = objHoaDon.MaHoaDon;
                }

                itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                itemBH_HoaDon.PhaiThanhToan = 0;
                itemBH_HoaDon.TongGiamGia = 0;
                itemBH_HoaDon.TongChiPhi = 0;
                itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                itemBH_HoaDon.NgayTao = DateTime.Now;
                itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                itemBH_HoaDon.TyGia = 1;
                itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon; // Hoa don chuyen hang = 9
                itemBH_HoaDon.ChoThanhToan = false; // neu luu tam => cho thanh toan == false
                itemBH_HoaDon.TongChietKhau = 0;
                itemBH_HoaDon.TongTienThue = 0;
                itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                itemBH_HoaDon.ID_CheckIn = objHoaDon.ID_CheckIn;
                #endregion

                string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);

                if (objHoaDon.YeuCau == "1")
                {
                    HT_ThongBao httbCH = new HT_ThongBao();
                    httbCH.ID = Guid.NewGuid();
                    httbCH.ID_DonVi = objHoaDon.ID_CheckIn.Value;
                    httbCH.LoaiThongBao = 1; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                    httbCH.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httbCH.ID + "')\"> Đơn chuyển hàng <a onclick=\"loadthongbao('2', '" + sMaHoaDon + "','" + httbCH.ID + "')\">" + "<span class=\"blue\">" + sMaHoaDon + " </span>" + " </a> đang đợi nhận. Vui lòng theo dõi và nhận đơn hàng để thêm hàng hóa vào kho </p>";
                    httbCH.NgayTao = DateTime.Now;
                    httbCH.NguoiDungDaDoc = "";
                    db.HT_ThongBao.Add(httbCH);
                    db.SaveChanges();

                }

                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    #region BH_ChiTietHoaDon
                    var dem = objCTHoaDon.Count;
                    string listCT = "";
                    string listND = "";
                    foreach (var item in objCTHoaDon)
                    {
                        dem = dem - 1;
                        DonViQuiDoi dvqd = _classDVQD.Get(id => id.ID == item.ID_DonViQuiDoi);
                        DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi && p.ID_DonVi == objHoaDon.ID_DonVi && p.ID_LoHang == item.ID_LoHang).FirstOrDefault();
                        DM_HangHoa hanghoa = db.DM_HangHoa.Where(p => p.ID == dvqd.ID_HangHoa).FirstOrDefault();
                        //gán chuỗi bao gồm trong ls thao tác
                        listCT = listCT + "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",") + "</br>";
                        listND = listND + "- " + dvqd.MaHangHoa + ":" + item.SoLuong + " Giá chuyển: " + string.Format("{0:n0}", item.GiaVon).Replace(".", ",");
                        //thông báo khi hết tồn kho
                        double? soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi);
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            DonGia = item.GiaVon.Value,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            SoLuong = item.SoLuong,
                            SoThuTu = dem,
                            TienChietKhau = item.SoLuong,
                            ThanhTien = item.ThanhTien,
                            GhiChu = item.GhiChu,
                            GiaVon = dmgv != null ? dmgv.GiaVon : 0,
                            ChatLieu = "",
                            MauSac = "",
                            KichCo = "",
                            PTChietKhau = 0,
                            TienThue = 0,
                            PTChiPhi = 0,
                            TienChiPhi = 0,
                            ThanhToan = 0,
                            An_Hien = true,
                            ID_LoHang = item.ID_LoHang == null ? null : item.ID_LoHang,
                        };
                        if (soluongton - item.SoLuong * dvqd.TyLeChuyenDoi <= hanghoa.TonToiThieu)
                        {
                            HT_ThongBao httb = new HT_ThongBao();
                            httb.ID = Guid.NewGuid();
                            httb.ID_DonVi = objHoaDon.ID_DonVi;
                            httb.LoaiThongBao = 0; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                            httb.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httb.ID + "')\"> Hàng hóa <a onclick=\"loadthongbao('1', '" + dvqd.MaHangHoa + "','" + httb.ID + "')\">" + "<span class=\"blue\">" + dvqd.MaHangHoa + " </span>" + " </a> đã hết số lượng tồn kho. Xin vui lòng nhập thêm để tiếp tục kinh doanh </p>";
                            httb.NgayTao = DateTime.Now;
                            httb.NguoiDungDaDoc = "";
                            db.HT_ThongBao.Add(httb);
                            db.SaveChanges();
                        }
                        strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                    }

                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                    hT_NhatKySuDung.ID = Guid.NewGuid();
                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                    hT_NhatKySuDung.ID_HoaDon = itemBH_HoaDon.ID;
                    hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                    hT_NhatKySuDung.LoaiHoaDon = 10;
                    hT_NhatKySuDung.ChucNang = "Chuyển hàng";
                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                    hT_NhatKySuDung.NoiDung = "Thêm mới phiếu chuyển hàng : " + itemBH_HoaDon.MaHoaDon + ", từ chi nhánh:" + _classDMDV.Get(p => p.ID == objHoaDon.ID_DonVi).TenDonVi + ", tới chi nhánh: " + _classDMDV.Get(p => p.ID == objHoaDon.ID_CheckIn).TenDonVi + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: </br> " + listND;
                    hT_NhatKySuDung.NoiDungChiTiet = "Thêm mới phiếu chuyển hàng:  <a onclick=\"FindMaHDCH('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, từ chi nhánh: " + _classDMDV.Get(p => p.ID == objHoaDon.ID_DonVi).TenDonVi + ", tới chi nhánh: " + _classDMDV.Get(p => p.ID == objHoaDon.ID_CheckIn).TenDonVi + ", thời gian: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: </br> " + listCT;
                    hT_NhatKySuDung.LoaiNhatKy = 1;
                    hT_NhatKySuDung.ID_DonVi = objHoaDon.ID_DonVi;
                    SaveDiary.add_Diary(hT_NhatKySuDung);
                    #endregion

                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = itemBH_HoaDon.ID,
                        MaHoaDon = itemBH_HoaDon.MaHoaDon,
                        DienGiai = itemBH_HoaDon.DienGiai,
                        NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                        PhaiThanhToan = itemBH_HoaDon.PhaiThanhToan,
                        TongChietKhau = itemBH_HoaDon.TongChietKhau,
                        TongGiamGia = itemBH_HoaDon.TongGiamGia,
                        TongChiPhi = itemBH_HoaDon.TongChiPhi,
                        TongTienHang = itemBH_HoaDon.TongTienHang,
                        ChoThanhToan = itemBH_HoaDon.ChoThanhToan,
                        NgayTao = itemBH_HoaDon.NgayTao,
                        NguoiTao = itemBH_HoaDon.NguoiTao,
                        ID_DonVi = itemBH_HoaDon.ID_DonVi,
                        LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon,
                        TongTienThue = itemBH_HoaDon.TongTienThue,
                        YeuCau = itemBH_HoaDon.YeuCau,
                        ID_CheckIn = itemBH_HoaDon.ID_CheckIn
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }
        #endregion
        [HttpPost, HttpGet]
        public string Check_Exist([FromBody] JObject data)
        {
            try
            {
                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                string err = string.Empty;
                return err;
            }
            catch (Exception ex)
            {
                string err = string.Empty;
                if (ex.Message.EndsWith("'ID_DonVi'."))
                {
                    return err = "Tên chi nhánh không tồn tại!";
                }
                else
                {
                    return err = "Thêm mới thất bại: " + ex.Message;
                }
            }
        }

        //public IHttpActionResult GetListPrinter()
        //{
        //    List<Printersclass> Printersfor = new List<Printersclass>();
        //    ManagementObjectSearcher printers = new ManagementObjectSearcher("Select * from Win32_Printer");
        //    foreach (ManagementObject printer in printers.Get())
        //    {
        //        Printersclass obj = new Printersclass { };
        //        obj.Name = (string)printer.GetPropertyValue("Name");
        //        obj.PortName = (string)printer.GetPropertyValue("PortName");
        //        Printersfor.Add(obj);
        //    }
        //    return Json(new
        //    {
        //        printer = Printersfor,
        //    });
        //}

        public class Printersclass
        {
            public string Name { get; set; }
            public string PortName { get; set; }
        }

        public void PrintBar_Bep(Guid idHoadon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var ctbep = from hd in db.BH_HoaDon
                            join ban in db.DM_ViTri on hd.ID_ViTri equals ban.ID
                            join ct in db.BH_HoaDon_ChiTiet on hd.ID equals ct.ID_HoaDon
                            select new
                            {
                                ct.ID_DonViQuiDoi,
                                ct.Bep_SoLuongChoCungUng,
                                ct.Bep_SoLuongYeuCau,
                                ban.TenViTri,
                            };

                var arrIDQuyDoi = ctbep.Select(x => x.ID_DonViQuiDoi).ToList();
                var lstMayIn = (from qd in db.DonViQuiDois
                                join ct in ctbep on qd.ID equals ct.ID_DonViQuiDoi
                                join hh in db.DM_HangHoa on qd.ID_HangHoa equals hh.ID
                                join tl in db.The_NhomThe on hh.ID_NhomHang equals new Guid(tl.TenNhomThe)
                                join mi in db.DM_MauIn on new Guid(tl.GhiChu) equals mi.ID
                                where arrIDQuyDoi.Contains(qd.ID)
                                group new { ct, hh, tl } by new { hh.ID_NhomHang, tl.MaNhomThe, mi.DuLieuMauIn } into g
                                select new
                                {
                                    ID_NhomHang = g.Key.ID_NhomHang,
                                    PrintPort = g.Key.MaNhomThe,
                                    Content = g.Key.DuLieuMauIn,
                                    ThucDon = g.Where(x => x.hh.ID_NhomHang == g.Key.ID_NhomHang).Select(x =>
                                        new
                                        {
                                            TenHangHoa = x.hh.TenHangHoa,
                                            TenViTri = x.ct.TenViTri,
                                            Bep_SoLuongChoCungUng = x.ct.Bep_SoLuongChoCungUng,
                                            Bep_SoLuongYeuCau = x.ct.Bep_SoLuongYeuCau
                                        })
                                });
                foreach (var item in lstMayIn)
                {
                    //System.IO.Ports.SerialPort myPort = new System.IO.Ports.SerialPort(item.PrintPort);
                    //if (myPort.IsOpen == false) //if not open, open the port
                    //    myPort.Open();
                    //do your work here

                    PrintDocument recordDoc = new PrintDocument();
                    recordDoc.DocumentName = "abdd";
                    PrinterSettings ps = new PrinterSettings();
                    ps.PrinterName = "XP-58";
                    recordDoc.PrinterSettings = ps;
                    recordDoc.Print();
                }
            };
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Insert_ThongBaoNhabep([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);

                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                // insert
                #region BH_HoaDon
                var idViTri = objHoaDon.ID_ViTri;
                idViTri = idViTri == Guid.Empty ? null : idViTri;
                BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                itemBH_HoaDon.ID = Guid.NewGuid();

                itemBH_HoaDon.MaHoaDon = classhoadon.SP_GetAutoCode_HDDatHang();
                itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien;
                itemBH_HoaDon.ID_ViTri = idViTri;
                itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? Guid.Empty : objHoaDon.ID_DoiTuong;
                itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                itemBH_HoaDon.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia;
                itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                itemBH_HoaDon.NgayTao = DateTime.Now;
                itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                itemBH_HoaDon.TyGia = 1;
                itemBH_HoaDon.LoaiHoaDon = 3; // TB Bep: assign LoaiHoaDon = 3 --> to do Tinh + a.Trinh check loaiHoaDon when TinhTon
                itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;
                itemBH_HoaDon.TongChietKhau = 0;
                itemBH_HoaDon.TongTienThue = objHoaDon.TongTienThue;
                itemBH_HoaDon.YeuCau = "1";// assign YeuCau = 1: PhieuTam

                #endregion

                string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    #region BH_ChiTietHoaDon
                    foreach (var item in objCTHoaDon)
                    {
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            DonGia = item.DonGia,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            SoLuong = item.SoLuong,
                            ThanhTien = item.ThanhTien,
                            TienChietKhau = item.TienChietKhau,
                            PTChietKhau = item.PTChietKhau,
                            GiaVon = item.GiaVon,
                            Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh,
                            Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau,
                            Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng,
                            ThoiGian = item.ThoiGian,
                            ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                            ThoiGianThucHien = item.ThoiGianThucHien,
                            GhiChu = item.GhiChu,
                            PTThue = item.PTThue,
                            TienThue = item.TienThue,
                            ID_ViTri = idViTri,
                        };

                        strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        itemBH_HoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon);
                    }
                    #endregion

                    return CreatedAtRoute("DefaultApi", new { id = itemBH_HoaDon.ID }, itemBH_HoaDon);
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Update_ThongBaoNhabep([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);

                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();

                #region "Update Hoa Don"
                var idViTri = objHoaDon.ID_ViTri;
                idViTri = idViTri == Guid.Empty ? null : idViTri;
                BH_HoaDon objUpHD = new BH_HoaDon();
                objUpHD.ID = objHoaDon.ID;
                objUpHD.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                objUpHD.ID_ViTri = idViTri;
                objUpHD.DienGiai = objHoaDon.DienGiai;
                objUpHD.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? Guid.Empty : objHoaDon.ID_DoiTuong;
                objUpHD.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                objUpHD.TongGiamGia = objHoaDon.TongGiamGia;
                objUpHD.TongChiPhi = objHoaDon.TongChiPhi;
                objUpHD.TongTienHang = objHoaDon.TongTienHang;
                objUpHD.ChoThanhToan = objHoaDon.ChoThanhToan;
                objUpHD.DiemGiaoDich = objHoaDon.DiemGiaoDich;

                string err = classhoadon.Update_HoaDon_NhaBep(objUpHD);
                #endregion

                if (err == string.Empty)
                {
                    #region " BH_HoaDon_ChiTiet "
                    // delete all CTHD with ID_HoaDon
                    string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                    if (sErr == string.Empty)
                    {
                        foreach (var item in objCTHoaDon)
                        {
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                DonGia = item.DonGia,
                                GiaVon = item.GiaVon,
                                ID_HoaDon = objUpHD.ID,
                                SoLuong = item.SoLuong,
                                ThanhTien = item.ThanhTien,
                                TienChietKhau = item.TienChietKhau,
                                PTChietKhau = item.PTChietKhau,
                                ThoiGian = item.ThoiGian,
                                ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                                ThoiGianThucHien = item.ThoiGianThucHien,
                                Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh,
                                Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau,
                                Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng,
                                ID_ViTri = idViTri,
                            };
                            classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                            objUpHD.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon
                        }
                    }

                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = objHoaDon.ID,
                        MaHoaDon = objUpHD.MaHoaDon,
                        ID_NhanVien = objUpHD.ID_NhanVien,
                        ID_ViTri = objUpHD.ID_ViTri,
                        DienGiai = objUpHD.DienGiai,
                        ID_DoiTuong = objUpHD.ID_DoiTuong,
                        PhaiThanhToan = objUpHD.PhaiThanhToan,
                        TongChietKhau = objUpHD.TongChietKhau,
                        TongGiamGia = objUpHD.TongGiamGia,
                        TongChiPhi = objUpHD.TongChiPhi,
                        TongTienHang = objUpHD.TongTienHang,
                        ChoThanhToan = objUpHD.ChoThanhToan,
                        BH_HoaDon_ChiTiet = objUpHD.BH_HoaDon_ChiTiet.Select(x => new BH_HoaDon_ChiTietDTO { ID = x.ID, ID_DonViQuiDoi = x.ID_DonViQuiDoi, ID_ChiTietDinhLuong = x.ID_ChiTietDinhLuong, ID_ChiTietGoiDV = x.ID_ChiTietGoiDV, ID_LoHang = x.ID_LoHang, SoLuong = x.SoLuong, DonGia = x.DonGia, ThanhTien = x.ThanhTien }).ToList()
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objUpHD.ID }, objReturn);
                    #endregion
                }
                else
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
        }

        // use at NhaHang
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult PostBH_HoaDon_NhaHang([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                    classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                    classDM_DonVi _classDMDV = new classDM_DonVi(db);
                    ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);

                    BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                    List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                    bool isTinhGVTB = data["IsSetGiaVonTrungBinh"].ToObject<bool>();

                    // insert
                    if (objHoaDon.ID == Guid.Empty)
                    {
                        #region BH_HoaDon
                        BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                        itemBH_HoaDon.ID = Guid.NewGuid();
                        itemBH_HoaDon.ID_HoaDon = objHoaDon.ID_HoaDon;
                        itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                        itemBH_HoaDon.ID_ViTri = objHoaDon.ID_ViTri == Guid.Empty ? null : objHoaDon.ID_ViTri;
                        itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                        itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                        itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                        itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? Guid.Empty : objHoaDon.ID_DoiTuong;
                        itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon;
                        itemBH_HoaDon.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                        itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia;
                        itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                        itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                        itemBH_HoaDon.NgayTao = DateTime.Now;
                        itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                        itemBH_HoaDon.TyGia = 1;
                        itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon;
                        itemBH_HoaDon.SoLuongKhachHang = objHoaDon.SoLuongKhachHang;
                        itemBH_HoaDon.GioVao = objHoaDon.GioVao;
                        itemBH_HoaDon.GioRa = objHoaDon.GioRa;
                        itemBH_HoaDon.ID_BangGia = objHoaDon.ID_BangGia == Guid.Empty ? null : objHoaDon.ID_BangGia;

                        itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;// neu luu tam => cho thanh toan == false
                        itemBH_HoaDon.TongChietKhau = objHoaDon.TongChietKhau; // PTGiam
                        itemBH_HoaDon.TongTienThue = objHoaDon.TongTienThue;
                        itemBH_HoaDon.DiemGiaoDich = objHoaDon.DiemGiaoDich;

                        itemBH_HoaDon.ID_KhuyenMai = objHoaDon.ID_KhuyenMai;
                        itemBH_HoaDon.KhuyeMai_GiamGia = objHoaDon.KhuyeMai_GiamGia; // tongGiamGia of KM
                        itemBH_HoaDon.KhuyenMai_GhiChu = objHoaDon.KhuyenMai_GhiChu;// = note KMHD + note KMCT
                        string sMaHoaDon = string.Empty;
                        if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                        {
                            // neu la chuyen ghep ban/thong bao bep
                            if (objHoaDon.ChoThanhToan.Value)
                            {
                                sMaHoaDon = classhoadon.SP_GetAutoCode_HDDatHang();
                            }
                            else
                            {
                                sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, itemBH_HoaDon.ID_DonVi, itemBH_HoaDon.NgayLapHoaDon);
                            }
                        }
                        else
                        {
                            sMaHoaDon = objHoaDon.MaHoaDon;
                        }
                        itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                        string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);
                        #endregion

                        if (strIns != null && strIns != string.Empty)
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                        else
                        {
                            #region BH_ChiTietHoaDon

                            // nha hang: khong co tra goi DV --> only check ChoThanhToan = true
                            var isTraGoiDV = objHoaDon.LoaiHoaDon == 3 || objHoaDon.ChoThanhToan == true;

                            foreach (var item in objCTHoaDon)
                            {
                                DonViQuiDoi dvqd = _classDVQD.Gets(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                                DM_HangHoa hanghoa = db.DM_HangHoa.Where(p => p.ID == dvqd.ID_HangHoa).FirstOrDefault();
                                double soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi).Value;

                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    SoThuTu = item.SoThuTu,
                                    DonGia = item.DonGia,
                                    GiaVon = item.GiaVon,
                                    ID_HoaDon = itemBH_HoaDon.ID,
                                    SoLuong = item.SoLuong,
                                    ThanhTien = item.ThanhTien,
                                    PTChietKhau = item.PTChietKhau, // PT giam
                                    TienChietKhau = item.TienChietKhau, // tien giam 
                                    ThoiGian = item.ThoiGian,
                                    ThoiGianHoanThanh = item.ThoiGianHoanThanh,// used to dichvu theogio
                                    ThoiGianThucHien = item.ThoiGianThucHien,// if dv theogio ThoiGianThucHien = soluong
                                    GhiChu = item.GhiChu,
                                    TangKem = item.TangKem,
                                    ID_TangKem = item.ID_TangKem,
                                    ID_KhuyenMai = item.ID_KhuyenMai,
                                    ID_LoHang = item.ID_LoHang,
                                    ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                    ID_ViTri = item.ID_ViTri == Guid.Empty ? null : item.ID_ViTri,

                                    PTThue = item.PTThue,
                                    TienThue = item.TienThue,
                                };
                                // Mua gói dịch vụ: không lưu TP định lượng, vì để a Trịnh get giá vốn của dịch vụ (= tổng giá vốn của Tp dịch vụ)
                                if (isTraGoiDV == false)
                                {
                                    var dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(itemBH_HoaDon.ID_DonVi, item.ID_DonViQuiDoi);

                                    if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                    {
                                        // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                        ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                        double sumGiaVonDL = 0;
                                        foreach (var itemDL in dinhluongDV)
                                        {
                                            // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                            var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;
                                            // sum giavon all TP dinh luong
                                            sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                            BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                            {
                                                ID = Guid.NewGuid(),
                                                ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                ID_LoHang = itemDL.ID_LoHang,
                                                SoLuong = soluongTPDL,
                                                GiaVon = itemDL.GiaVon,
                                                ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                ID_HoaDon = itemBH_HoaDon.ID,
                                                ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                                SoLuongDinhLuong_BanDau = soluongTPDL,
                                                ID_ViTri = ctHoaDon.ID_ViTri,
                                                ChatLieu = ctHoaDon.ChatLieu,
                                            };
                                            if (isTinhGVTB == false)
                                            {
                                                ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                            }
                                            classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon_DL);
                                        }
                                    }
                                }

                                strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                                itemBH_HoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon

                                if (hanghoa.LaHangHoa != null && hanghoa.LaHangHoa.Value && soluongton - item.SoLuong <= hanghoa.TonToiThieu)
                                {
                                    HT_ThongBao httbCH = new HT_ThongBao();
                                    httbCH.ID = Guid.NewGuid();
                                    httbCH.ID_DonVi = objHoaDon.ID_DonVi;
                                    httbCH.LoaiThongBao = 0; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                                    httbCH.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httbCH.ID + "')\"> Hàng hóa <a onclick=\"loadthongbao('1', '" + dvqd.MaHangHoa + "','" + httbCH.ID + "')\">" + "<span class=\"blue\">" + dvqd.MaHangHoa + " </span>" + " </a> đã hết số lượng tồn kho. Vui lòng nhập thêm để tiếp tục kinh doanh </p>";
                                    httbCH.NgayTao = DateTime.Now;
                                    httbCH.NguoiDungDaDoc = "";
                                    db.HT_ThongBao.Add(httbCH);
                                    db.SaveChanges();
                                }
                            }
                            #endregion
                            BH_HoaDonDTO dto = new BH_HoaDonDTO
                            {
                                ID = itemBH_HoaDon.ID,
                                MaHoaDon = itemBH_HoaDon.MaHoaDon,
                                NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                                BH_HoaDon_ChiTiet = itemBH_HoaDon.BH_HoaDon_ChiTiet
                                .Select(x => new BH_HoaDon_ChiTietDTO { ID = x.ID, ID_ViTri = x.ID_ViTri, ID_DonViQuiDoi = x.ID_DonViQuiDoi, SoLuong = x.SoLuong, DonGia = x.DonGia, ThanhTien = x.ThanhTien }).ToList()
                            };
                            return Json(new { res = true, data = dto });
                        }
                    }
                    // update
                    else
                    {
                        #region "Update Hoa Don"
                        BH_HoaDon objUpHD = classhoadon.Update_HoaDon_DatHang(objHoaDon);
                        #endregion

                        #region " BH_HoaDon_ChiTiet "
                        // if TBbep, after ThanhToan: save TP dinh luong in CTHD
                        var isTraGoiDV = objHoaDon.LoaiHoaDon == 3 || objHoaDon.ChoThanhToan == true;

                        // delete all CTHD with ID_HoaDon
                        string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                        if (sErr == string.Empty)
                        {
                            // insert again
                            foreach (var item in objCTHoaDon)
                            {
                                // khong thay doi GiaVon, chi thay doi DonGia
                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet();
                                ctHoaDon.ID = Guid.NewGuid();
                                ctHoaDon.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                                ctHoaDon.SoThuTu = item.SoThuTu;
                                ctHoaDon.DonGia = item.DonGia;
                                ctHoaDon.GiaVon = item.GiaVon;
                                ctHoaDon.ID_HoaDon = objHoaDon.ID;
                                ctHoaDon.SoLuong = item.SoLuong;
                                ctHoaDon.ThanhTien = item.ThanhTien;
                                ctHoaDon.TienChietKhau = item.TienChietKhau; // tien giam 
                                ctHoaDon.PTChietKhau = item.PTChietKhau;
                                ctHoaDon.ThoiGian = item.ThoiGian;
                                ctHoaDon.ThoiGianHoanThanh = item.ThoiGianHoanThanh;
                                ctHoaDon.ThoiGianThucHien = item.ThoiGianThucHien;
                                ctHoaDon.Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh;
                                ctHoaDon.Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau;
                                ctHoaDon.Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng;
                                ctHoaDon.GhiChu = item.GhiChu;
                                ctHoaDon.PTThue = item.PTThue;
                                ctHoaDon.TienThue = item.TienThue;
                                ctHoaDon.ID_ViTri = item.ID_ViTri == Guid.Empty ? null : item.ID_ViTri;

                                if (isTraGoiDV == false)
                                {
                                    var dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(objUpHD.ID_DonVi, item.ID_DonViQuiDoi);

                                    if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                    {
                                        // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                        ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                        double sumGiaVonDL = 0;
                                        foreach (var itemDL in dinhluongDV)
                                        {
                                            // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                            var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;
                                            // sum giavon all TP dinh luong
                                            sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                            BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                            {
                                                ID = Guid.NewGuid(),
                                                ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                ID_LoHang = itemDL.ID_LoHang,
                                                // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                                SoLuong = itemDL.SoLuong * ctHoaDon.SoLuong,
                                                GiaVon = itemDL.GiaVon,
                                                ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                ID_HoaDon = objUpHD.ID,
                                                ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV, // save Tp định lượng of dich vụ
                                                ID_ViTri = ctHoaDon.ID_ViTri,
                                                ChatLieu = ctHoaDon.ChatLieu,
                                            };
                                            if (isTinhGVTB == false)
                                            {
                                                ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                            }
                                            classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon_DL);
                                        }
                                    }
                                }
                                classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                                objUpHD.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon
                            }
                        }
                        #endregion

                        BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                        {
                            ID = objHoaDon.ID,
                            MaHoaDon = objUpHD.MaHoaDon,
                            ID_NhanVien = objUpHD.ID_NhanVien,
                            ID_ViTri = objUpHD.ID_ViTri,
                            DienGiai = objUpHD.DienGiai,
                            ID_DoiTuong = objUpHD.ID_DoiTuong,
                            PhaiThanhToan = objUpHD.PhaiThanhToan,
                            TongChietKhau = objUpHD.TongChietKhau,
                            TongGiamGia = objUpHD.TongGiamGia,
                            TongChiPhi = objUpHD.TongChiPhi,
                            TongTienHang = objUpHD.TongTienHang,
                            ChoThanhToan = objUpHD.ChoThanhToan,
                            BH_HoaDon_ChiTiet = objUpHD.BH_HoaDon_ChiTiet
                            .Select(x => new BH_HoaDon_ChiTietDTO { ID = x.ID, ID_DonViQuiDoi = x.ID_DonViQuiDoi, SoLuong = x.SoLuong, DonGia = x.DonGia, ThanhTien = x.ThanhTien }).ToList()
                        };
                        return Json(new { res = true, data = objReturn });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        public DM_DoiTuong GetDM_DoiTuongAdd(DM_DoiTuong objDoiTuong, DM_NguonKhachHang objNguonKhach)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classDM_DoiTuong = new classDM_DoiTuong(db);
                if (objDoiTuong != null)
                {
                    Guid? idNguonKhach = null;
                    if (objNguonKhach != null)
                    {
                        DM_NguonKhachHang nguonKhachReturn = GetDM_NguonKhachAdd(objNguonKhach);
                        idNguonKhach = nguonKhachReturn.ID;
                    }

                    string sMaDoiTuong = string.Empty;
                    if (objDoiTuong.MaDoiTuong != null && objDoiTuong.MaDoiTuong != string.Empty)
                    {
                        sMaDoiTuong = objDoiTuong.MaDoiTuong;
                    }
                    else
                    {
                        //mã khach hang tự động
                        sMaDoiTuong = classDM_DoiTuong.SP_GetautoCode(objDoiTuong.LoaiDoiTuong);
                    }
                    #region DM_DoiTuong
                    DM_DoiTuong DM_DoiTuong = new DM_DoiTuong { };
                    DM_DoiTuong.ID = Guid.NewGuid();
                    DM_DoiTuong.MaDoiTuong = sMaDoiTuong;
                    DM_DoiTuong.TenDoiTuong = objDoiTuong.TenDoiTuong;
                    DM_DoiTuong.GioiTinhNam = objDoiTuong.GioiTinhNam;
                    DM_DoiTuong.LoaiDoiTuong = objDoiTuong.LoaiDoiTuong;
                    DM_DoiTuong.NgaySinh_NgayTLap = objDoiTuong.NgaySinh_NgayTLap;
                    DM_DoiTuong.NgayTao = DateTime.Now;
                    DM_DoiTuong.NguoiTao = "";
                    DM_DoiTuong.DiaChi = objDoiTuong.DiaChi;
                    DM_DoiTuong.Email = objDoiTuong.Email;
                    DM_DoiTuong.GhiChu = objDoiTuong.GhiChu;
                    DM_DoiTuong.DienThoai = objDoiTuong.DienThoai;
                    DM_DoiTuong.ID_NhomDoiTuong = objDoiTuong.ID_NhomDoiTuong;
                    DM_DoiTuong.ID_QuanHuyen = objDoiTuong.ID_QuanHuyen;
                    DM_DoiTuong.ID_TinhThanh = objDoiTuong.ID_TinhThanh;
                    DM_DoiTuong.MaSoThue = objDoiTuong.MaSoThue;
                    DM_DoiTuong.ID_NguonKhach = idNguonKhach;
                    DM_DoiTuong.ID_NguoiGioiThieu = objDoiTuong.ID_NguoiGioiThieu;
                    DM_DoiTuong.ID_NhanVienPhuTrach = objDoiTuong.ID_NhanVienPhuTrach;
                    DM_DoiTuong.LaCaNhan = objDoiTuong.LaCaNhan;
                    DM_DoiTuong.TenDoiTuong_ChuCaiDau = objDoiTuong.TenDoiTuong_ChuCaiDau;
                    DM_DoiTuong.TenDoiTuong_KhongDau = objDoiTuong.TenDoiTuong_KhongDau;
                    DM_DoiTuong.PhuongXa = "";
                    DM_DoiTuong.TheoDoi = true;
                    string strIns = classDM_DoiTuong.Add_DoiTuong(DM_DoiTuong);
                    #endregion
                    return DM_DoiTuong;
                }
                else
                {
                    return null;
                }
            }
        }

        public DM_NguonKhachHang GetDM_NguonKhachAdd(DM_NguonKhachHang objNguonKhach)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_NguonKhach classNguonKhach = new ClassDM_NguonKhach(db);
                if (objNguonKhach != null)
                {
                    DM_NguonKhachHang DM_NguonKhachHang = new DM_NguonKhachHang { };
                    DM_NguonKhachHang.ID = Guid.NewGuid();
                    DM_NguonKhachHang.TenNguonKhach = objNguonKhach.TenNguonKhach;
                    DM_NguonKhachHang.NguoiSua = objNguonKhach.NguoiSua;
                    DM_NguonKhachHang.NguoiTao = objNguonKhach.NguoiTao;
                    DM_NguonKhachHang.NgayTao = DateTime.Now;
                    string strIns = classNguonKhach.Add(DM_NguonKhachHang);
                    return DM_NguonKhachHang;
                }
                else
                {
                    return null;
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult SP_PostBH_HoaDon([FromBody] JObject data)
        {
            BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
            List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
            bool isTinhGVTB = data["IsSetGiaVonTrungBinh"].ToObject<bool>();

            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);

                // insert
                if (objHoaDon.ID == Guid.Empty)
                {
                    #region BH_HoaDon
                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                    itemBH_HoaDon.ID = Guid.NewGuid();

                    string sMaHoaDon = string.Empty;
                    if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                    {
                        // neu la chuyen ghep ban
                        if (objHoaDon.ChoThanhToan.Value)
                        {
                            sMaHoaDon = classhoadon.SP_GetAutoCode_HDDatHang();
                        }
                        else
                        {
                            sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, itemBH_HoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                        }
                    }
                    else
                    {
                        sMaHoaDon = objHoaDon.MaHoaDon;
                    }
                    itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                    itemBH_HoaDon.ID_HoaDon = objHoaDon.ID_HoaDon;
                    itemBH_HoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                    itemBH_HoaDon.ID_ViTri = objHoaDon.ID_ViTri == Guid.Empty ? null : objHoaDon.ID_ViTri; ;
                    itemBH_HoaDon.NguoiTao = objHoaDon.NguoiTao;
                    itemBH_HoaDon.DienGiai = objHoaDon.DienGiai;
                    itemBH_HoaDon.YeuCau = objHoaDon.YeuCau;
                    itemBH_HoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? Guid.Empty : objHoaDon.ID_DoiTuong;
                    itemBH_HoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon.AddMilliseconds(DateTime.Now.Millisecond);
                    itemBH_HoaDon.PhaiThanhToan = objHoaDon.PhaiThanhToan;
                    itemBH_HoaDon.TongGiamGia = objHoaDon.TongGiamGia;
                    itemBH_HoaDon.TongChiPhi = objHoaDon.TongChiPhi;
                    itemBH_HoaDon.TongTienHang = objHoaDon.TongTienHang;
                    itemBH_HoaDon.NgayTao = DateTime.Now;
                    itemBH_HoaDon.ID_DonVi = objHoaDon.ID_DonVi;
                    itemBH_HoaDon.TyGia = 1;
                    itemBH_HoaDon.LoaiHoaDon = objHoaDon.LoaiHoaDon;
                    itemBH_HoaDon.ID_BangGia = objHoaDon.ID_BangGia == Guid.Empty ? null : objHoaDon.ID_BangGia;

                    itemBH_HoaDon.ChoThanhToan = objHoaDon.ChoThanhToan;// neu luu tam => cho thanh toan == false
                    itemBH_HoaDon.TongChietKhau = objHoaDon.TongChietKhau; // PTGiam
                    itemBH_HoaDon.TongTienThue = objHoaDon.TongTienThue;
                    itemBH_HoaDon.DiemGiaoDich = objHoaDon.DiemGiaoDich;
                    itemBH_HoaDon.TongThanhToan = objHoaDon.TongThanhToan;
                    itemBH_HoaDon.PhaiThanhToanBaoHiem = objHoaDon.PhaiThanhToanBaoHiem;
                    itemBH_HoaDon.ChiPhi = objHoaDon.ChiPhi;
                    itemBH_HoaDon.ChiPhi_GhiChu = objHoaDon.ChiPhi_GhiChu;
                    itemBH_HoaDon.LienHeBaoHiem = objHoaDon.LienHeBaoHiem;
                    itemBH_HoaDon.SoDienThoaiLienHeBaoHiem = objHoaDon.SoDienThoaiLienHeBaoHiem;
                    itemBH_HoaDon.ID_BaoHiem = objHoaDon.ID_BaoHiem;
                    itemBH_HoaDon.ID_PhieuTiepNhan = objHoaDon.ID_PhieuTiepNhan;

                    itemBH_HoaDon.ID_KhuyenMai = objHoaDon.ID_KhuyenMai;
                    itemBH_HoaDon.KhuyeMai_GiamGia = objHoaDon.KhuyeMai_GiamGia; // tongGiamGia of KM
                    itemBH_HoaDon.KhuyenMai_GhiChu = objHoaDon.KhuyenMai_GhiChu;// = note KMHD + note KMCT

                    itemBH_HoaDon.NgayApDungGoiDV = objHoaDon.NgayApDungGoiDV;
                    itemBH_HoaDon.HanSuDungGoiDV = objHoaDon.HanSuDungGoiDV;

                    itemBH_HoaDon.GioVao = objHoaDon.GioVao;
                    itemBH_HoaDon.GioRa = objHoaDon.GioRa;

                    string strIns = classhoadon.Add_HoaDon(itemBH_HoaDon);

                    #endregion

                    if (strIns != null && strIns != string.Empty)
                        return Json(new { res = false, mes = strIns });
                    else
                    {
                        try
                        {
                            #region "BH_NhanVienThucHien of HoaDon
                            // insert BH_NhanVienThucHien if set up ChietKhau TraHang
                            if (objHoaDon.LoaiHoaDon == 6 && objHoaDon.ID_HoaDon != null)
                            {
                                nhanvienThucHien.SP_InsertChietKhauTraHang(objHoaDon.ID_HoaDon, objHoaDon.PhaiThanhToan, objHoaDon.ID, objHoaDon.ID_DonVi);
                            }
                            #endregion

                            #region BH_ChiTietHoaDon

                            // if tra goiDV OR DatHang OR Mua goiDV --> not save TP dinh luong in CTHD (không trừ tồn ở thẻ kho)
                            var isTraGoiDV = objHoaDon.LoaiHoaDon == 19;
                            if (isTraGoiDV == false)
                            {
                                var hdGoiDV = classhoadon.Get(x => x.ID == itemBH_HoaDon.ID_HoaDon && x.LoaiHoaDon == 19);
                                isTraGoiDV = hdGoiDV == null ? false : true;
                            }

                            foreach (var item in objCTHoaDon)
                            {
                                DonViQuiDoi dvqd = _classDVQD.Gets(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                                DM_HangHoa hanghoa = db.DM_HangHoa.Where(p => p.ID == dvqd.ID_HangHoa).FirstOrDefault();
                                double soluongton = classhoadon.TinhSLTonHH(dvqd.ID_HangHoa, objHoaDon.ID_DonVi).Value;

                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    SoThuTu = item.SoThuTu,
                                    DonGia = item.DonGia,
                                    GiaVon = item.GiaVon,
                                    ID_HoaDon = itemBH_HoaDon.ID,
                                    SoLuong = item.SoLuong,
                                    ThanhTien = item.ThanhTien,
                                    ThanhToan = item.ThanhToan, // default: assign = ThanhTien
                                    PTChietKhau = item.PTChietKhau, // PT giam
                                    TienChietKhau = item.TienChietKhau, // tien giam 
                                    ThoiGian = item.ThoiGian,
                                    GhiChu = item.GhiChu,
                                    TangKem = item.TangKem,
                                    ID_TangKem = item.ID_TangKem,
                                    ID_KhuyenMai = item.ID_KhuyenMai,
                                    ID_LoHang = item.ID_LoHang,
                                    ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                    PTThue = item.PTThue,
                                    TienThue = item.TienThue,
                                    LoaiThoiGianBH = item.LoaiThoiGianBH,
                                    ThoiGianBaoHanh = item.ThoiGianBaoHanh,
                                    ThoiGianThucHien = item.ThoiGianThucHien,
                                    ID_ViTri = item.ID_ViTri,
                                    ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                                    QuaThoiGian = item.QuaThoiGian
                                };

                                #region DinhLuong_DichVu

                                double? sumGiaVonDL = 0;

                                if (isTraGoiDV == false)
                                {
                                    // get TP_DinhLuong from .js
                                    if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                                    {
                                        ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                        foreach (var itemDL in item.ThanhPhan_DinhLuong)
                                        {
                                            if (itemDL.SoLuong > 0)
                                            {
                                                // đã gấp SLuong TPDinhLuong in .js
                                                var soluongTPDL = itemDL.SoLuong;

                                                // sum giavon all TP dinh luong
                                                sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                    ID_LoHang = itemDL.ID_LoHang,
                                                    SoLuong = soluongTPDL,
                                                    GiaVon = itemDL.GiaVon,
                                                    ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                    ID_HoaDon = itemBH_HoaDon.ID,
                                                    ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                                    GhiChu = itemDL.GhiChu,
                                                    ChatLieu = ctHoaDon.ChatLieu,
                                                    SoLuongDinhLuong_BanDau = itemDL.SoLuongDinhLuong_BanDau * ctHoaDon.SoLuong,
                                                };
                                                classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon_DL);
                                            }
                                        }
                                        // GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                        if (isTinhGVTB == false)
                                        {
                                            ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                        }
                                    }
                                    else
                                    {
                                        // check ThietLap.GiaVonTrungBinh
                                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                                        var dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(itemBH_HoaDon.ID_DonVi, item.ID_DonViQuiDoi);

                                        if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                        {
                                            // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                            ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                            foreach (var itemDL in dinhluongDV)
                                            {
                                                // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                                var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;

                                                // sum giavon all TP dinh luong
                                                sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                    ID_LoHang = itemDL.ID_LoHang,
                                                    SoLuong = soluongTPDL,
                                                    GiaVon = itemDL.GiaVon,
                                                    ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                    ID_HoaDon = itemBH_HoaDon.ID,
                                                    ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                                    ChatLieu = ctHoaDon.ChatLieu,
                                                    SoLuongDinhLuong_BanDau = soluongTPDL,
                                                };
                                                classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon_DL);
                                            }

                                            // GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                            if (isTinhGVTB == false)
                                            {
                                                ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                            }
                                        }
                                    }
                                }

                                strIns = classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                                itemBH_HoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon
                                #endregion

                                #region BH_NhanVienThucHien
                                foreach (var itemNV in item.BH_NhanVienThucHien)
                                {
                                    BH_NhanVienThucHien nvien = new BH_NhanVienThucHien
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_ChiTietHoaDon = ctHoaDon.ID,
                                        ID_NhanVien = itemNV.ID_NhanVien,
                                        ThucHien_TuVan = itemNV.ThucHien_TuVan,
                                        TienChietKhau = itemNV.TienChietKhau,
                                        PT_ChietKhau = itemNV.PT_ChietKhau,
                                        TheoYeuCau = itemNV.TheoYeuCau,
                                        HeSo = itemNV.HeSo,
                                        TinhChietKhauTheo = itemNV.TinhChietKhauTheo,
                                        TinhHoaHongTruocCK = itemNV.TinhHoaHongTruocCK,
                                    };
                                    nhanvienThucHien.Insert(nvien);
                                }
                                #endregion

                                if (hanghoa.LaHangHoa != null && hanghoa.LaHangHoa.Value && soluongton - item.SoLuong <= hanghoa.TonToiThieu)
                                {
                                    HT_ThongBao httbCH = new HT_ThongBao();
                                    httbCH.ID = Guid.NewGuid();
                                    httbCH.ID_DonVi = objHoaDon.ID_DonVi;
                                    httbCH.LoaiThongBao = 0; //loai = 0 thông báo hết hàng, 1: thông báo có đơn chuyển hàng, 3: thông báo ngày sinh nhật
                                    httbCH.NoiDungThongBao = "<p onclick=\"loaddadoc('" + httbCH.ID + "')\"> Hàng hóa <a onclick=\"loadthongbao('1', '" + dvqd.MaHangHoa + "','" + httbCH.ID + "')\">" + "<span class=\"blue\">" + dvqd.MaHangHoa + " </span>" + " </a> đã hết số lượng tồn kho. Vui lòng nhập thêm để tiếp tục kinh doanh </p>";
                                    httbCH.NgayTao = DateTime.Now;
                                    httbCH.NguoiDungDaDoc = "";
                                    db.HT_ThongBao.Add(httbCH);
                                    db.SaveChanges();
                                }
                            }

                            #endregion

                            #region TraHang- tru chietkhau
                            if (objHoaDon.LoaiHoaDon == 6 && objHoaDon.ID_HoaDon != null)
                            {
                                nhanvienThucHien.ChiTietTraHang_insertChietKhauNV(objHoaDon.ID_HoaDon);
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            CookieStore.WriteLog("PostHoadon: ChiTietHoaDon " + ex.InnerException + ex.Message);
                            return Json(new { res = false, data = ex.InnerException + ex.Message });
                        }

                        BH_HoaDonDTO dt0 = new BH_HoaDonDTO
                        {
                            ID = itemBH_HoaDon.ID,
                            MaHoaDon = itemBH_HoaDon.MaHoaDon,
                            NgayLapHoaDon = itemBH_HoaDon.NgayLapHoaDon,
                            BH_HoaDon_ChiTiet = itemBH_HoaDon.BH_HoaDon_ChiTiet
                                .Select(x => new BH_HoaDon_ChiTietDTO { ID = x.ID, ID_DonViQuiDoi = x.ID_DonViQuiDoi, ID_ChiTietDinhLuong = x.ID_ChiTietDinhLuong, ID_ChiTietGoiDV = x.ID_ChiTietGoiDV, ID_LoHang = x.ID_LoHang, SoLuong = x.SoLuong, DonGia = x.DonGia, ThanhTien = x.ThanhTien }).ToList()
                        };
                        return Json(new { res = true, data = dt0 });
                    }
                }
                // update
                else
                {
                    #region "Update Hoa Don"
                    BH_HoaDon objUpHD = classhoadon.Update_HoaDon_DatHang(objHoaDon);
                    #endregion

                    #region " BH_HoaDon_ChiTiet "
                    // delete all CTHD with ID_HoaDon
                    string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                    if (sErr != string.Empty)
                    {
                        return Json(new { res = false, mes = sErr });
                    }
                    // insert again
                    foreach (var item in objCTHoaDon)
                    {
                        // khong thay doi GiaVon, chi thay doi DonGia
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet();
                        ctHoaDon.ID = Guid.NewGuid();
                        ctHoaDon.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                        ctHoaDon.SoThuTu = item.SoThuTu;
                        ctHoaDon.DonGia = item.DonGia;
                        ctHoaDon.GiaVon = item.GiaVon;
                        ctHoaDon.ID_HoaDon = objHoaDon.ID;
                        ctHoaDon.SoLuong = item.SoLuong;
                        ctHoaDon.ThanhTien = item.ThanhTien;
                        ctHoaDon.ThanhToan = item.ThanhToan; // default: assign = ThanhTien
                        ctHoaDon.TienChietKhau = item.TienChietKhau; // tien giam 
                        ctHoaDon.PTChietKhau = item.PTChietKhau; // tien giam 
                        ctHoaDon.ThoiGian = item.ThoiGian;
                        ctHoaDon.Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh;
                        ctHoaDon.Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau;
                        ctHoaDon.Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng;
                        ctHoaDon.GhiChu = item.GhiChu;
                        ctHoaDon.ID_LoHang = item.ID_LoHang;
                        ctHoaDon.PTThue = item.PTThue;
                        ctHoaDon.TienThue = item.TienThue;
                        ctHoaDon.LoaiThoiGianBH = item.LoaiThoiGianBH;
                        ctHoaDon.ThoiGianBaoHanh = item.ThoiGianBaoHanh;
                        ctHoaDon.ID_ViTri = item.ID_ViTri;
                        ctHoaDon.ThoiGianHoanThanh = item.ThoiGianHoanThanh;
                        ctHoaDon.QuaThoiGian = item.QuaThoiGian;

                        classhoadonchitiet.Add_ChiTietHoaDon(ctHoaDon);
                        objUpHD.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon

                        // không cần update TonKho, NVienThucHien, DinhLuongDichVu vì HD đặt hàng/ gói dịch vụ không lưu NVThucHien, DinhLuongDichVu
                    }
                    #endregion

                    BH_HoaDonDTO objReturn = new BH_HoaDonDTO
                    {
                        ID = objHoaDon.ID,
                        MaHoaDon = objUpHD.MaHoaDon,
                        ID_NhanVien = objUpHD.ID_NhanVien == Guid.Empty ? null : objUpHD.ID_NhanVien,
                        ID_ViTri = objUpHD.ID_ViTri,
                        DienGiai = objUpHD.DienGiai,
                        ID_DoiTuong = objUpHD.ID_DoiTuong == Guid.Empty ? null : objUpHD.ID_DoiTuong,
                        PhaiThanhToan = objUpHD.PhaiThanhToan,
                        TongChietKhau = objUpHD.TongChietKhau,
                        TongGiamGia = objUpHD.TongGiamGia,
                        TongChiPhi = objUpHD.TongChiPhi,
                        TongTienHang = objUpHD.TongTienHang,
                        ChoThanhToan = objUpHD.ChoThanhToan,
                        BH_HoaDon_ChiTiet = objUpHD.BH_HoaDon_ChiTiet.Select(x => new BH_HoaDon_ChiTietDTO { ID = x.ID, ID_DonViQuiDoi = x.ID_DonViQuiDoi, ID_ChiTietDinhLuong = x.ID_ChiTietDinhLuong, ID_ChiTietGoiDV = x.ID_ChiTietGoiDV, ID_LoHang = x.ID_LoHang, SoLuong = x.SoLuong, DonGia = x.DonGia, ThanhTien = x.ThanhTien }).ToList()
                    };
                    return Json(new { res = true, data = objReturn });
                }
            }
        }
        [HttpGet, HttpPost]
        public void PostPutBH_HoaDonBan([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                        bool isSetGiaVonTB = data["IsSetGiaVonTrungBinh"].ToObject<bool>();
                        List<string> lstID_NewOld = new List<string>();

                        var ngaylapHD = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                        var ngaylapHDOld = ngaylapHD;
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                        ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);
                        ClassQuy_HoaDon_ChiTiet classQuyCT = new ClassQuy_HoaDon_ChiTiet(db);

                        int loaiHoaDon = objHoaDon.LoaiHoaDon;
                        string sMaHoaDon = string.Empty;
                        Guid idHoaDon = Guid.Empty;

                        if (objHoaDon.ID == Guid.Empty)
                        {
                            #region BH_HoaDon
                            objHoaDon.ID = Guid.NewGuid();
                            idHoaDon = objHoaDon.ID;
                            objHoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                            objHoaDon.ID_ViTri = objHoaDon.ID_ViTri == Guid.Empty ? null : objHoaDon.ID_ViTri;
                            objHoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? Guid.Empty : objHoaDon.ID_DoiTuong;
                            objHoaDon.NgayLapHoaDon = ngaylapHD;
                            objHoaDon.NgayTao = DateTime.Now;
                            objHoaDon.TyGia = 1;
                            objHoaDon.LoaiHoaDon = loaiHoaDon;
                            objHoaDon.ID_BangGia = objHoaDon.ID_BangGia == Guid.Empty ? null : objHoaDon.ID_BangGia;

                            if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                            {
                                sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                            }
                            else
                            {
                                sMaHoaDon = classhoadon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                            }
                            objHoaDon.MaHoaDon = sMaHoaDon;

                            classhoadon.Add_HoaDon(objHoaDon);

                            #endregion

                            #region "BH_NhanVienThucHien of HoaDon
                            if (objHoaDon.LoaiHoaDon == 6 && objHoaDon.ID_HoaDon != null)
                            {
                                var phaiTT = objHoaDon.PhaiThanhToan - objHoaDon.TongTienThue;
                                nhanvienThucHien.SP_InsertChietKhauTraHang(objHoaDon.ID_HoaDon, phaiTT, objHoaDon.ID, objHoaDon.ID_DonVi);
                            }
                            #endregion

                            #region BH_ChiTietHoaDon
                            foreach (var item in objCTHoaDon)
                            {
                                var giaban = item.DonGia - item.TienChietKhau;
                                var sMaLoHang = string.Empty;

                                List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    SoThuTu = item.SoThuTu,
                                    DonGia = item.DonGia,
                                    GiaVon = item.GiaVon,
                                    ID_HoaDon = objHoaDon.ID,
                                    SoLuong = item.SoLuong,
                                    ThanhTien = item.ThanhTien,
                                    ThanhToan = item.ThanhToan,
                                    PTChietKhau = item.PTChietKhau, // PT giam
                                    TienChietKhau = item.TienChietKhau, // tien giam 
                                    GhiChu = item.GhiChu,
                                    TangKem = item.TangKem,
                                    ID_TangKem = item.ID_TangKem,
                                    ID_KhuyenMai = item.ID_KhuyenMai,
                                    ID_LoHang = item.ID_LoHang,
                                    ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                    PTThue = item.PTThue,
                                    TienThue = item.TienThue,
                                    LoaiThoiGianBH = item.LoaiThoiGianBH,
                                    ThoiGianBaoHanh = item.ThoiGianBaoHanh,
                                    ThoiGianThucHien = item.ThoiGianThucHien,
                                    ID_ViTri = item.ID_ViTri,
                                    ThoiGian = item.ThoiGian,
                                    ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                                    QuaThoiGian = item.QuaThoiGian,
                                    ChatLieu = item.ChatLieu,// 1.TraHD, 2.TraGoiDV, 3.HDXuLy DH, 4.Sudung GoiDV, else: empty/null
                                    DiemKhuyenMai = item.DiemKhuyenMai,
                                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                                    TenHangHoaThayThe = item.TenHangHoaThayThe,
                                    ID_LichBaoDuong = item.ID_LichBaoDuong,
                                    ThanhPhanComBo = item.ThanhPhanComBo
                                };

                                #region DinhLuong_DichVu

                                double? sumGiaVonDL = 0;

                                // Mua/tra gói dịch vụ: không lưu TP định lượng, vì để a Trịnh get giá vốn của dịch vụ (= tổng giá vốn của Tp dịch vụ)
                                // get TP_DinhLuong from .js
                                if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                                {
                                    // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                    ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                    foreach (var itemDL in item.ThanhPhan_DinhLuong)
                                    {
                                        if (itemDL.SoLuong > 0)
                                        {
                                            // đã nhân số lượng ở .js (show in view)
                                            var soluongTPDL = itemDL.SoLuong;

                                            // sum giavon all TP dinh luong
                                            sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                            BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                            {
                                                ID = Guid.NewGuid(),
                                                ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                ID_LoHang = itemDL.ID_LoHang,
                                                SoLuong = soluongTPDL,
                                                GiaVon = itemDL.GiaVon,
                                                ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                ID_HoaDon = objHoaDon.ID,
                                                ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,// used to tính giá vốn dịch vụ khi sử dụng gdv
                                                GhiChu = itemDL.GhiChu,
                                                ChatLieu = ctHoaDon.ChatLieu,
                                                SoLuongDinhLuong_BanDau = itemDL.SoLuongDinhLuong_BanDau * ctHoaDon.SoLuong,
                                                TenHangHoaThayThe = itemDL.TenHangHoaThayThe,
                                            };
                                            lstCT.Add(ctHoaDon_DL);
                                        }
                                    }

                                    // only assign GiaVon (CTHD) if isSetGiaVonTB = false, GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                    if (isSetGiaVonTB == false)
                                    {
                                        ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                    }
                                }
                                else
                                {
                                    if (item.ThanhPhanComBo != null && item.ThanhPhanComBo.Count > 0)
                                    {
                                        ctHoaDon.ID_ParentCombo = ctHoaDon.ID;
                                        var lstCombo = GetListCombo_andTPDLuong_ofThis(db, ctHoaDon, objHoaDon.ID_DonVi,
                                            objHoaDon.LoaiHoaDon, false, ref lstID_NewOld);
                                        lstCT.AddRange(lstCombo);
                                    }
                                    else
                                    {
                                        if (objHoaDon.LoaiHoaDon != 6)
                                        {
                                            List<libDM_HangHoa.ClassDM_HangHoa.SP_ThanhPhan_DinhLuong> dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(objHoaDon.ID_DonVi, item.ID_DonViQuiDoi);

                                            if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                            {
                                                ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                                foreach (var itemDL in dinhluongDV)
                                                {
                                                    // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                                    var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;

                                                    // sum giavon all TP dinh luong
                                                    sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                    BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                    {
                                                        ID = Guid.NewGuid(),
                                                        ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                        ID_LoHang = itemDL.ID_LoHang,
                                                        SoLuong = soluongTPDL,
                                                        GiaVon = itemDL.GiaVon,
                                                        ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                        ID_HoaDon = objHoaDon.ID,
                                                        ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV, // save Tp định lượng of dich vụ
                                                        SoLuongDinhLuong_BanDau = soluongTPDL,
                                                        ChatLieu = ctHoaDon.ChatLieu,
                                                        TenHangHoaThayThe = ctHoaDon.TenHangHoaThayThe,
                                                    };
                                                    lstCT.Add(ctHoaDon_DL);
                                                }

                                                // GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                                if (isSetGiaVonTB == false)
                                                {
                                                    ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                                }
                                            }
                                        }
                                    }
                                }

                                lstCT.Add(ctHoaDon);
                                db.BH_HoaDon_ChiTiet.AddRange(lstCT);
                                objHoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon

                                #endregion

                                #region BH_NhanVienThucHien of HangHoa
                                foreach (var itemNV in item.BH_NhanVienThucHien)
                                {
                                    BH_NhanVienThucHien nvien = new BH_NhanVienThucHien
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_ChiTietHoaDon = ctHoaDon.ID,
                                        ID_NhanVien = itemNV.ID_NhanVien,
                                        ThucHien_TuVan = itemNV.ThucHien_TuVan,
                                        TienChietKhau = itemNV.TienChietKhau,
                                        PT_ChietKhau = itemNV.PT_ChietKhau,
                                        TheoYeuCau = itemNV.TheoYeuCau,
                                        HeSo = itemNV.HeSo,
                                        TinhChietKhauTheo = itemNV.TinhChietKhauTheo,
                                        TinhHoaHongTruocCK = itemNV.TinhHoaHongTruocCK,
                                    };
                                    nhanvienThucHien.Insert(nvien);
                                }
                                #endregion
                            }
                            #endregion

                            #region TraHang- tru chietkhau
                            if (objHoaDon.LoaiHoaDon == 6 && objHoaDon.ID_HoaDon != null)
                            {
                                nhanvienThucHien.ChiTietTraHang_insertChietKhauNV(objHoaDon.ID_HoaDon);
                            }
                            #endregion
                        }
                        else
                        {
                            ngaylapHDOld = db.BH_HoaDon.Find(objHoaDon.ID).NgayLapHoaDon;//used to save diary
                            objHoaDon.NgayLapHoaDon = ngaylapHD;
                            BH_HoaDon objUpHD = classhoadon.Update_HoaDon_DatHang(objHoaDon);
                            idHoaDon = objUpHD.ID;
                            sMaHoaDon = objUpHD.MaHoaDon;

                            foreach (var item in objCTHoaDon)
                            {
                                var ctOld = db.BH_HoaDon_ChiTiet.Find(item.ID);
                                if (ctOld != null)
                                {

                                }
                                // khong thay doi GiaVon, chi thay doi DonGia
                                List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
                                var giaban = item.DonGia - item.TienChietKhau;
                                string sMaLoHang = string.Empty;
                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet();
                                ctHoaDon.ID = Guid.NewGuid();
                                if (item.ID != null && item.ID != Guid.Empty)
                                {
                                    string sID = string.Concat(ctHoaDon.ID, ",", item.ID, ";");
                                    lstID_NewOld.Add(sID);
                                }
                                ctHoaDon.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                                ctHoaDon.SoThuTu = item.SoThuTu;
                                ctHoaDon.DonGia = item.DonGia;
                                ctHoaDon.GiaVon = item.GiaVon;
                                ctHoaDon.ID_HoaDon = objHoaDon.ID;
                                ctHoaDon.SoLuong = item.SoLuong;
                                ctHoaDon.ThanhTien = item.ThanhTien;
                                ctHoaDon.ThanhToan = item.ThanhToan; // thanhtoan = thanhtien + soluong * thue
                                ctHoaDon.TienChietKhau = item.TienChietKhau; // tien giam 
                                ctHoaDon.PTChietKhau = item.PTChietKhau;
                                ctHoaDon.ThoiGian = item.ThoiGian;
                                ctHoaDon.Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh;
                                ctHoaDon.Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau;
                                ctHoaDon.Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng;
                                ctHoaDon.GhiChu = item.GhiChu;
                                ctHoaDon.ID_LoHang = item.ID_LoHang;
                                ctHoaDon.PTThue = item.PTThue;
                                ctHoaDon.TienThue = item.TienThue;
                                ctHoaDon.LoaiThoiGianBH = item.LoaiThoiGianBH;
                                ctHoaDon.ThoiGianBaoHanh = item.ThoiGianBaoHanh;
                                ctHoaDon.ID_ViTri = item.ID_ViTri;
                                ctHoaDon.ID_ChiTietGoiDV = item.ID_ChiTietGoiDV;// DatHang, MuaGoiDV (ID_ChiTietGoiDV = null)
                                ctHoaDon.QuaThoiGian = item.QuaThoiGian;
                                ctHoaDon.ThoiGianThucHien = item.ThoiGianThucHien;
                                ctHoaDon.ThoiGianHoanThanh = item.ThoiGianHoanThanh;
                                ctHoaDon.ChatLieu = item.ChatLieu;
                                ctHoaDon.DiemKhuyenMai = item.DiemKhuyenMai;
                                ctHoaDon.DonGiaBaoHiem = item.DonGiaBaoHiem;
                                ctHoaDon.TenHangHoaThayThe = item.TenHangHoaThayThe;
                                ctHoaDon.ID_LichBaoDuong = item.ID_LichBaoDuong;
                                ctHoaDon.ThanhPhanComBo = item.ThanhPhanComBo;

                                #region DinhLuong_DichVu

                                double? sumGiaVonDL = 0;

                                if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                                {
                                    // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                    ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                    foreach (var itemDL in item.ThanhPhan_DinhLuong)
                                    {
                                        if (itemDL.SoLuong > 0)
                                        {
                                            // đã nhân số lượng ở .js (show in view)
                                            var soluongTPDL = itemDL.SoLuong;

                                            // sum giavon all TP dinh luong
                                            sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                            BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                            {
                                                ID = Guid.NewGuid(),
                                                ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                ID_LoHang = itemDL.ID_LoHang,
                                                SoLuong = soluongTPDL,
                                                GiaVon = itemDL.GiaVon,
                                                ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                ID_HoaDon = objHoaDon.ID,
                                                ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                                GhiChu = itemDL.GhiChu,
                                                ChatLieu = ctHoaDon.ChatLieu,
                                                SoLuongDinhLuong_BanDau = itemDL.SoLuongDinhLuong_BanDau * ctHoaDon.SoLuong,
                                                TenHangHoaThayThe = itemDL.TenHangHoaThayThe,
                                            };
                                            lstCT.Add(ctHoaDon_DL);
                                        }
                                    }

                                    // only assign GiaVon (CTHD) if isSetGiaVonTB = false, GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                    if (isSetGiaVonTB == false)
                                    {
                                        ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                    }
                                }
                                else
                                {
                                    if (item.ThanhPhanComBo != null && item.ThanhPhanComBo.Count > 0)
                                    {
                                        ctHoaDon.ID_ParentCombo = ctHoaDon.ID;
                                        var lstCombo = GetListCombo_andTPDLuong_ofThis(db, ctHoaDon, objHoaDon.ID_DonVi,
                                            objHoaDon.LoaiHoaDon, true, ref lstID_NewOld);
                                        lstCT.AddRange(lstCombo);
                                    }
                                    else
                                    {
                                        var dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(objHoaDon.ID_DonVi, item.ID_DonViQuiDoi);
                                        if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                        {
                                            ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;
                                            // get TPdinhLuong in DB
                                            foreach (var itemDL in dinhluongDV)
                                            {
                                                // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                                var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;

                                                // sum giavon all TP dinh luong
                                                sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                    ID_LoHang = itemDL.ID_LoHang,
                                                    SoLuong = soluongTPDL,
                                                    GiaVon = itemDL.GiaVon,
                                                    ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                    ID_HoaDon = objHoaDon.ID,
                                                    ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV, // save Tp định lượng of dich vụ
                                                    SoLuongDinhLuong_BanDau = soluongTPDL,
                                                    ChatLieu = ctHoaDon.ChatLieu,
                                                    TenHangHoaThayThe = ctHoaDon.TenHangHoaThayThe,
                                                };
                                                lstCT.Add(ctHoaDon_DL);
                                            }

                                            // GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                            if (isSetGiaVonTB == false)
                                            {
                                                ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                            }
                                        }
                                    }
                                }

                                lstCT.Add(ctHoaDon);
                                db.BH_HoaDon_ChiTiet.AddRange(lstCT);
                                objHoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon);// used to restunr list js

                                #endregion

                                #region BH_NhanVienThucHien of HangHoa
                                foreach (var itemNV in item.BH_NhanVienThucHien)
                                {
                                    BH_NhanVienThucHien nvien = new BH_NhanVienThucHien
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_ChiTietHoaDon = ctHoaDon.ID,
                                        ID_NhanVien = itemNV.ID_NhanVien,
                                        ThucHien_TuVan = itemNV.ThucHien_TuVan,
                                        TienChietKhau = itemNV.TienChietKhau,
                                        PT_ChietKhau = itemNV.PT_ChietKhau,
                                        TheoYeuCau = itemNV.TheoYeuCau,
                                        HeSo = itemNV.HeSo,
                                        TinhHoaHongTruocCK = itemNV.TinhHoaHongTruocCK,
                                    };
                                    nhanvienThucHien.Insert(nvien);
                                }
                                #endregion
                            }

                        }
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        CookieStore.WriteLog(string.Concat("PostBH_HoaDon ", ex.InnerException + ex.Message));
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostBH_HoaDon_SoQuy_Spa_NKySuDung([FromBody] JObject data, Guid? idNhanVien = null)
        {
            var tenChucNang = "";
            var noiDung = "";
            var noiDungChiTiet = "";
            var txtFirst = "";

            var style1 = "<a style= \"cursor: pointer\" onclick = \"";
            var style2 = "('";
            var style3 = "')\" >";
            var style4 = "</a>";
            var styleMaHD = string.Empty;
            var styleHangHoa = string.Empty;
            string sMaHoaDon = string.Empty;
            var idHoaDon = Guid.Empty;
            var insert = true;
            List<string> lstID_NewOld = new List<string>();// used to update again idChiTietGDDV

            HT_NhatKySuDung nky = new HT_NhatKySuDung();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                        bool isSetGiaVonTB = data["IsSetGiaVonTrungBinh"].ToObject<bool>();
                        var ngaylapHD = objHoaDon.NgayLapHoaDon.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                        var ngaylapHDOld = ngaylapHD;
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                        ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);
                        ClassQuy_HoaDon_ChiTiet classQuyCT = new ClassQuy_HoaDon_ChiTiet(db);

                        int loaiHoaDon = objHoaDon.LoaiHoaDon;
                        // insert
                        if (objHoaDon.ID == Guid.Empty)
                        {
                            #region BH_HoaDon
                            objHoaDon.ID = Guid.NewGuid();
                            idHoaDon = objHoaDon.ID;
                            objHoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                            objHoaDon.ID_ViTri = objHoaDon.ID_ViTri == Guid.Empty ? null : objHoaDon.ID_ViTri;
                            objHoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? Guid.Empty : objHoaDon.ID_DoiTuong;
                            objHoaDon.NgayLapHoaDon = ngaylapHD;
                            objHoaDon.NgayTao = DateTime.Now;
                            objHoaDon.TyGia = 1;
                            objHoaDon.LoaiHoaDon = loaiHoaDon;
                            objHoaDon.ID_BangGia = objHoaDon.ID_BangGia == Guid.Empty ? null : objHoaDon.ID_BangGia;

                            if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                            {
                                sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                            }
                            else
                            {
                                bool exist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon);
                                if (exist)
                                {
                                    return Json(new
                                    {
                                        res = false,
                                        mes = "Mã hóa đơn đã tồn tại",
                                    });
                                }
                                sMaHoaDon = classhoadon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                            }
                            objHoaDon.MaHoaDon = sMaHoaDon;

                            string strIns = classhoadon.Add_HoaDon(objHoaDon);
                            styleMaHD = string.Concat(style1, "LoadHoaDon_byMaHD", style2, sMaHoaDon, style3, sMaHoaDon, style4);

                            #endregion

                            #region "BH_NhanVienThucHien of HoaDon
                            // insert BH_NhanVienThucHien if set up ChietKhau TraHang
                            if (objHoaDon.LoaiHoaDon == 6 && objHoaDon.ID_HoaDon != null)
                            {
                                var phaiTT = objHoaDon.PhaiThanhToan - objHoaDon.TongTienThue;
                                nhanvienThucHien.SP_InsertChietKhauTraHang(objHoaDon.ID_HoaDon, phaiTT, objHoaDon.ID, objHoaDon.ID_DonVi);
                            }
                            #endregion

                            if (strIns != null && strIns != string.Empty)
                                return Json(new
                                {
                                    res = false,
                                    mes = strIns,
                                });
                            else
                            {
                                #region BH_ChiTietHoaDon
                                foreach (var item in objCTHoaDon)
                                {
                                    var giaban = item.DonGia - item.TienChietKhau;
                                    var sMaLoHang = string.Empty;

                                    List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
                                    BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                        SoThuTu = item.SoThuTu,
                                        DonGia = item.DonGia,
                                        GiaVon = item.GiaVon,
                                        ID_HoaDon = objHoaDon.ID,
                                        SoLuong = item.SoLuong,
                                        ThanhTien = item.ThanhTien,
                                        ThanhToan = item.ThanhToan,
                                        PTChietKhau = item.PTChietKhau, // PT giam
                                        TienChietKhau = item.TienChietKhau, // tien giam 
                                        GhiChu = item.GhiChu,
                                        TangKem = item.TangKem,
                                        ID_TangKem = item.ID_TangKem,
                                        ID_KhuyenMai = item.ID_KhuyenMai,
                                        ID_LoHang = item.ID_LoHang,
                                        ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                        PTThue = item.PTThue,
                                        TienThue = item.TienThue,
                                        LoaiThoiGianBH = item.LoaiThoiGianBH,
                                        ThoiGianBaoHanh = item.ThoiGianBaoHanh,
                                        ThoiGianThucHien = item.ThoiGianThucHien,
                                        ID_ViTri = item.ID_ViTri,
                                        ThoiGian = item.ThoiGian,
                                        ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                                        QuaThoiGian = item.QuaThoiGian,
                                        ChatLieu = item.ChatLieu,// 1.TraHD, 2.TraGoiDV, 3.HDXuLy DH, 4.Sudung GoiDV, else: empty/null
                                        DiemKhuyenMai = item.DiemKhuyenMai,
                                        DonGiaBaoHiem = item.DonGiaBaoHiem,
                                        TenHangHoaThayThe = item.TenHangHoaThayThe,
                                        ID_LichBaoDuong = item.ID_LichBaoDuong,
                                        ThanhPhanComBo = item.ThanhPhanComBo
                                    };

                                    #region DinhLuong_DichVu

                                    double? sumGiaVonDL = 0;

                                    // Mua/tra gói dịch vụ: không lưu TP định lượng, vì để a Trịnh get giá vốn của dịch vụ (= tổng giá vốn của Tp dịch vụ)
                                    // get TP_DinhLuong from .js
                                    if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                                    {
                                        // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                        ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                        foreach (var itemDL in item.ThanhPhan_DinhLuong)
                                        {
                                            if (itemDL.SoLuong > 0)
                                            {
                                                // đã nhân số lượng ở .js (show in view)
                                                var soluongTPDL = itemDL.SoLuong;

                                                // sum giavon all TP dinh luong
                                                sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                    ID_LoHang = itemDL.ID_LoHang,
                                                    SoLuong = soluongTPDL,
                                                    GiaVon = itemDL.GiaVon,
                                                    ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                    ID_HoaDon = objHoaDon.ID,
                                                    ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,// used to tính giá vốn dịch vụ khi sử dụng gdv
                                                    GhiChu = itemDL.GhiChu,
                                                    ChatLieu = ctHoaDon.ChatLieu,
                                                    SoLuongDinhLuong_BanDau = itemDL.SoLuongDinhLuong_BanDau * ctHoaDon.SoLuong,
                                                    TenHangHoaThayThe = itemDL.TenHangHoaThayThe,
                                                };
                                                lstCT.Add(ctHoaDon_DL);
                                            }
                                        }

                                        // only assign GiaVon (CTHD) if isSetGiaVonTB = false, GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                        if (isSetGiaVonTB == false)
                                        {
                                            ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                        }
                                    }
                                    else
                                    {
                                        if (item.ThanhPhanComBo != null && item.ThanhPhanComBo.Count > 0)
                                        {
                                            ctHoaDon.ID_ParentCombo = ctHoaDon.ID;
                                            var lstCombo = GetListCombo_andTPDLuong_ofThis(db, ctHoaDon, objHoaDon.ID_DonVi,
                                                objHoaDon.LoaiHoaDon, false, ref lstID_NewOld);
                                            lstCT.AddRange(lstCombo);
                                        }
                                        else
                                        {
                                            if (objHoaDon.LoaiHoaDon != 6)
                                            {
                                                List<libDM_HangHoa.ClassDM_HangHoa.SP_ThanhPhan_DinhLuong> dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(objHoaDon.ID_DonVi, item.ID_DonViQuiDoi);

                                                if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                                {
                                                    ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                                    foreach (var itemDL in dinhluongDV)
                                                    {
                                                        // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                                        var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;

                                                        // sum giavon all TP dinh luong
                                                        sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                        BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                        {
                                                            ID = Guid.NewGuid(),
                                                            ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                            ID_LoHang = itemDL.ID_LoHang,
                                                            SoLuong = soluongTPDL,
                                                            GiaVon = itemDL.GiaVon,
                                                            ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                            ID_HoaDon = objHoaDon.ID,
                                                            ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV, // save Tp định lượng of dich vụ
                                                            SoLuongDinhLuong_BanDau = soluongTPDL,
                                                            ChatLieu = ctHoaDon.ChatLieu,
                                                            TenHangHoaThayThe = ctHoaDon.TenHangHoaThayThe,
                                                        };
                                                        lstCT.Add(ctHoaDon_DL);
                                                    }

                                                    // GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                                    if (isSetGiaVonTB == false)
                                                    {
                                                        ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    lstCT.Add(ctHoaDon);
                                    strIns = classhoadonchitiet.Add_ChiTietHoaDon(lstCT);
                                    objHoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon

                                    if (item.ID_LoHang != null)
                                    {
                                        sMaLoHang = string.Concat(" (Số lô: ", item.MaLoHang, ")");
                                    }
                                    styleHangHoa += string.Concat(style1, "loadHangHoabyMaHH", style2, item.MaHangHoa, style3, item.MaHangHoa, style4, sMaLoHang,
                                        ": Số lượng: ", item.SoLuong,
                                        ", Giá bán: ", giaban == 0 ? "0" : giaban.ToString("#,#", CultureInfo.InvariantCulture),
                                        ", Chiết khấu: ", item.TienChietKhau,
                                        ", Thuế: ", item.TienThue,
                                        "<br />");

                                    #endregion

                                    #region BH_NhanVienThucHien of HangHoa
                                    foreach (var itemNV in item.BH_NhanVienThucHien)
                                    {
                                        BH_NhanVienThucHien nvien = new BH_NhanVienThucHien
                                        {
                                            ID = Guid.NewGuid(),
                                            ID_ChiTietHoaDon = ctHoaDon.ID,
                                            ID_NhanVien = itemNV.ID_NhanVien,
                                            ThucHien_TuVan = itemNV.ThucHien_TuVan,
                                            TienChietKhau = itemNV.TienChietKhau,
                                            PT_ChietKhau = itemNV.PT_ChietKhau,
                                            TheoYeuCau = itemNV.TheoYeuCau,
                                            HeSo = itemNV.HeSo,
                                            TinhChietKhauTheo = itemNV.TinhChietKhauTheo,
                                            TinhHoaHongTruocCK = itemNV.TinhHoaHongTruocCK,
                                        };
                                        nhanvienThucHien.Insert(nvien);
                                    }
                                    #endregion
                                }
                                #endregion

                                #region TraHang- tru chietkhau
                                if (objHoaDon.LoaiHoaDon == 6 && objHoaDon.ID_HoaDon != null)
                                {
                                    nhanvienThucHien.ChiTietTraHang_insertChietKhauNV(objHoaDon.ID_HoaDon);
                                }
                                #endregion

                            }
                        }
                        // update
                        else
                        {
                            insert = false;
                            #region "Update Hoa Don"
                            if (!string.IsNullOrEmpty(objHoaDon.MaHoaDon))
                            {
                                bool exist = classhoadon.Check_MaHoaDonExist(objHoaDon.MaHoaDon, objHoaDon.ID);
                                if (exist)
                                {
                                    return Json(new
                                    {
                                        res = false,
                                        mes = "Mã hóa đơn đã tồn tại",
                                    });
                                }
                            }
                            ngaylapHDOld = db.BH_HoaDon.Find(objHoaDon.ID).NgayLapHoaDon;
                            objHoaDon.NgayLapHoaDon = ngaylapHD;
                            BH_HoaDon objUpHD = classhoadon.Update_HoaDon_DatHang(objHoaDon);
                            idHoaDon = objUpHD.ID;
                            sMaHoaDon = objUpHD.MaHoaDon;
                            #endregion

                            #region "BH_NhanVienThucHien of HoaDon
                            // delete cthd + nvthuchien with ID_HoaDon
                            string sErr = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                            if (sErr != string.Empty)
                            {
                                return Json(new { res = false, mes = sErr });
                            }
                            #endregion

                            #region " BH_HoaDon_ChiTiet "
                            if (sErr == string.Empty)
                            {

                                foreach (var item in objCTHoaDon)
                                {
                                    // khong thay doi GiaVon, chi thay doi DonGia
                                    List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
                                    var giaban = item.DonGia - item.TienChietKhau;
                                    string sMaLoHang = string.Empty;
                                    BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet();
                                    ctHoaDon.ID = Guid.NewGuid();
                                    if (item.ID != null && item.ID != Guid.Empty)
                                    {
                                        string sID = string.Concat(ctHoaDon.ID, ",", item.ID, ";");
                                        lstID_NewOld.Add(sID);
                                    }
                                    ctHoaDon.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                                    ctHoaDon.SoThuTu = item.SoThuTu;
                                    ctHoaDon.DonGia = item.DonGia;
                                    ctHoaDon.GiaVon = item.GiaVon;
                                    ctHoaDon.ID_HoaDon = objHoaDon.ID;
                                    ctHoaDon.SoLuong = item.SoLuong;
                                    ctHoaDon.ThanhTien = item.ThanhTien;
                                    ctHoaDon.ThanhToan = item.ThanhToan; // thanhtoan = thanhtien + soluong * thue
                                    ctHoaDon.TienChietKhau = item.TienChietKhau; // tien giam 
                                    ctHoaDon.PTChietKhau = item.PTChietKhau;
                                    ctHoaDon.ThoiGian = item.ThoiGian;
                                    ctHoaDon.Bep_SoLuongHoanThanh = item.Bep_SoLuongHoanThanh;
                                    ctHoaDon.Bep_SoLuongYeuCau = item.Bep_SoLuongYeuCau;
                                    ctHoaDon.Bep_SoLuongChoCungUng = item.Bep_SoLuongChoCungUng;
                                    ctHoaDon.GhiChu = item.GhiChu;
                                    ctHoaDon.ID_LoHang = item.ID_LoHang;
                                    ctHoaDon.PTThue = item.PTThue;
                                    ctHoaDon.TienThue = item.TienThue;
                                    ctHoaDon.LoaiThoiGianBH = item.LoaiThoiGianBH;
                                    ctHoaDon.ThoiGianBaoHanh = item.ThoiGianBaoHanh;
                                    ctHoaDon.ID_ViTri = item.ID_ViTri;
                                    ctHoaDon.ID_ChiTietGoiDV = item.ID_ChiTietGoiDV;// DatHang, MuaGoiDV (ID_ChiTietGoiDV = null)
                                    ctHoaDon.QuaThoiGian = item.QuaThoiGian;
                                    ctHoaDon.ThoiGianThucHien = item.ThoiGianThucHien;
                                    ctHoaDon.ThoiGianHoanThanh = item.ThoiGianHoanThanh;
                                    ctHoaDon.ChatLieu = item.ChatLieu;
                                    ctHoaDon.DiemKhuyenMai = item.DiemKhuyenMai;
                                    ctHoaDon.DonGiaBaoHiem = item.DonGiaBaoHiem;
                                    ctHoaDon.TenHangHoaThayThe = item.TenHangHoaThayThe;
                                    ctHoaDon.ID_LichBaoDuong = item.ID_LichBaoDuong;
                                    ctHoaDon.ThanhPhanComBo = item.ThanhPhanComBo;

                                    #region DinhLuong_DichVu

                                    double? sumGiaVonDL = 0;

                                    if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                                    {
                                        // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                        ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                        foreach (var itemDL in item.ThanhPhan_DinhLuong)
                                        {
                                            if (itemDL.SoLuong > 0)
                                            {
                                                // đã nhân số lượng ở .js (show in view)
                                                var soluongTPDL = itemDL.SoLuong;

                                                // sum giavon all TP dinh luong
                                                sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                    ID_LoHang = itemDL.ID_LoHang,
                                                    SoLuong = soluongTPDL,
                                                    GiaVon = itemDL.GiaVon,
                                                    ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                    ID_HoaDon = objHoaDon.ID,
                                                    ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                                    GhiChu = itemDL.GhiChu,
                                                    ChatLieu = ctHoaDon.ChatLieu,
                                                    SoLuongDinhLuong_BanDau = itemDL.SoLuongDinhLuong_BanDau * ctHoaDon.SoLuong,
                                                    TenHangHoaThayThe = itemDL.TenHangHoaThayThe,
                                                };
                                                lstCT.Add(ctHoaDon_DL);
                                            }
                                        }

                                        // only assign GiaVon (CTHD) if isSetGiaVonTB = false, GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                        if (isSetGiaVonTB == false)
                                        {
                                            ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                        }
                                    }
                                    else
                                    {
                                        if (item.ThanhPhanComBo != null && item.ThanhPhanComBo.Count > 0)
                                        {
                                            ctHoaDon.ID_ParentCombo = ctHoaDon.ID;
                                            var lstCombo = GetListCombo_andTPDLuong_ofThis(db, ctHoaDon, objHoaDon.ID_DonVi,
                                                objHoaDon.LoaiHoaDon, true, ref lstID_NewOld);
                                            lstCT.AddRange(lstCombo);
                                        }
                                        else
                                        {
                                            var dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(objHoaDon.ID_DonVi, item.ID_DonViQuiDoi);
                                            if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                            {
                                                ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;
                                                // get TPdinhLuong in DB
                                                foreach (var itemDL in dinhluongDV)
                                                {
                                                    // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                                    var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;

                                                    // sum giavon all TP dinh luong
                                                    sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                                    BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                    {
                                                        ID = Guid.NewGuid(),
                                                        ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                        ID_LoHang = itemDL.ID_LoHang,
                                                        SoLuong = soluongTPDL,
                                                        GiaVon = itemDL.GiaVon,
                                                        ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                        ID_HoaDon = objHoaDon.ID,
                                                        ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV, // save Tp định lượng of dich vụ
                                                        SoLuongDinhLuong_BanDau = soluongTPDL,
                                                        ChatLieu = ctHoaDon.ChatLieu,
                                                        TenHangHoaThayThe = ctHoaDon.TenHangHoaThayThe,
                                                    };
                                                    lstCT.Add(ctHoaDon_DL);
                                                }

                                                // GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                                                if (isSetGiaVonTB == false)
                                                {
                                                    ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                                                }
                                            }
                                        }
                                    }

                                    lstCT.Add(ctHoaDon);
                                    classhoadonchitiet.Add_ChiTietHoaDon(lstCT);
                                    objHoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon);// used to restunr list js

                                    if (item.ID_LoHang != null)
                                    {
                                        sMaLoHang = string.Concat(" (Số lô: ", item.MaLoHang, ")");
                                    }
                                    styleHangHoa += string.Concat(style1, "loadHangHoabyMaHH", style2, item.MaHangHoa, style3, item.MaHangHoa, style4, sMaLoHang, ": Số lượng: ", item.SoLuong, ", Giá bán: ", giaban == 0 ? "0" : giaban.ToString("#,#", CultureInfo.InvariantCulture), "<br />");

                                    #endregion

                                    #region BH_NhanVienThucHien of HangHoa
                                    foreach (var itemNV in item.BH_NhanVienThucHien)
                                    {
                                        BH_NhanVienThucHien nvien = new BH_NhanVienThucHien
                                        {
                                            ID = Guid.NewGuid(),
                                            ID_ChiTietHoaDon = ctHoaDon.ID,
                                            ID_NhanVien = itemNV.ID_NhanVien,
                                            ThucHien_TuVan = itemNV.ThucHien_TuVan,
                                            TienChietKhau = itemNV.TienChietKhau,
                                            PT_ChietKhau = itemNV.PT_ChietKhau,
                                            TheoYeuCau = itemNV.TheoYeuCau,
                                            HeSo = itemNV.HeSo,
                                            TinhHoaHongTruocCK = itemNV.TinhHoaHongTruocCK,
                                        };
                                        nhanvienThucHien.Insert(nvien);
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                        }

                        #region NhatKySuDung
                        switch (objHoaDon.LoaiHoaDon)
                        {
                            case 1:
                            case 0:
                                tenChucNang = "Bán hàng";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    if (insert)
                                    {
                                        txtFirst = "Tạo hóa đơn: ";
                                    }
                                    else
                                    {
                                        txtFirst = "Cập nhật hóa đơn: ";
                                    }
                                }
                                else
                                {
                                    if (insert)
                                    {
                                        txtFirst = "Tạo hóa đơn tạm lưu: ";
                                    }
                                    else
                                    {
                                        txtFirst = "Cập nhật hóa đơn tạm lưu: ";
                                    }
                                }
                                break;
                            case 3:
                                tenChucNang = "Báo giá";
                                if (insert)
                                {
                                    txtFirst = "Tạo báo giá: ";
                                }
                                else
                                {
                                    txtFirst = "Cập nhật báo giá: ";
                                }
                                break;
                            case 6:
                                tenChucNang = "Trả hàng";
                                txtFirst = "Tạo phiếu trả hàng: ";
                                break;
                            case 19:
                                tenChucNang = "Gói dịch vu";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    if (insert)
                                    {
                                        txtFirst = "Mua gói dịch vụ: ";
                                    }
                                    else
                                    {
                                        txtFirst = "Cập nhật gói dịch vụ: ";
                                    }
                                }
                                else
                                {
                                    if (insert)
                                    {
                                        txtFirst = "Tạm lưu gói dịch vụ: ";
                                    }
                                    else
                                    {
                                        txtFirst = "Cập nhật gói dịch vụ tạm lưu: ";
                                    }
                                }
                                break;
                            case 25:
                                tenChucNang = "Hóa đơn sửa chữa";
                                if (objHoaDon.ChoThanhToan.Value == false)
                                {
                                    if (insert)
                                    {
                                        txtFirst = "Tạo hóa đơn sửa chữa ";
                                    }
                                    else
                                    {
                                        txtFirst = "Cập nhật hóa đơn sửa chữa: ";
                                    }
                                }
                                else
                                {
                                    if (insert)
                                    {
                                        txtFirst = "Tạm lưu hóa đơn sửa chữa: ";
                                    }
                                    else
                                    {
                                        txtFirst = "Cập nhật hóa đơn tạm lưu: ";
                                    }
                                }
                                break;
                        }
                        noiDung = string.Concat(txtFirst, sMaHoaDon, " Giá trị: ", (objHoaDon.TongThanhToan ?? 0).ToString("#,#", CultureInfo.InvariantCulture), ", Thời gian: ", objHoaDon.NgayLapHoaDon);
                        noiDungChiTiet = string.Concat(noiDung, "<br /> - Tổng thuế: ", objHoaDon.TongTienThue,
                            "<br /> - Giảm trừ: ", objHoaDon.TongGiamGia, "<br /> bao gồm: <br />", styleHangHoa);


                        nky.ID = Guid.NewGuid();
                        nky.ID_DonVi = objHoaDon.ID_DonVi;
                        nky.LoaiHoaDon = objHoaDon.LoaiHoaDon;
                        nky.ID_NhanVien = idNhanVien != Guid.Empty && idNhanVien != null ? idNhanVien : objHoaDon.ID_NhanVien;
                        nky.ChucNang = tenChucNang;
                        nky.LoaiNhatKy = 1;
                        nky.NoiDung = noiDung;
                        nky.NoiDungChiTiet = noiDungChiTiet;
                        nky.ThoiGian = DateTime.Now;
                        nky.ID_HoaDon = idHoaDon;
                        nky.ThoiGianUpdateGV = ngaylapHDOld;

                        db.HT_NhatKySuDung.Add(nky);
                        db.SaveChanges();
                        #endregion
                        trans.Commit();

                        new SaveDiary().AddQueueJob(nky);
                        //if (objHoaDon.LoaiHoaDon != 1 && objHoaDon.LoaiHoaDon != 25)
                        //{
                        // update id_chitietgdv if update again dathang
                        //classhoadonchitiet.UpdateIDCTNew_forCTOld(lstID_NewOld);
                        //}

                        return Json(new
                        {
                            res = true,
                            data = new
                            {
                                objHoaDon.ID,
                                MaHoaDon = sMaHoaDon,
                                NgayLapHoaDon = ngaylapHD,
                                NgayLapHoaDonOld = ngaylapHDOld,
                                BH_HoaDon_ChiTiet = objHoaDon.BH_HoaDon_ChiTiet
                                .Select(x => new BH_HoaDon_ChiTietDTO
                                {
                                    ID = x.ID,
                                    ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                                    ID_ChiTietDinhLuong = x.ID_ChiTietDinhLuong,
                                    ID_ParentCombo = x.ID_ParentCombo,
                                    ID_ChiTietGoiDV = x.ID_ChiTietGoiDV,
                                    ID_LoHang = x.ID_LoHang,
                                    SoLuong = x.SoLuong,
                                    DonGia = x.DonGia,
                                    ThanhTien = x.ThanhTien,
                                    ThanhPhanComBo = new List<BH_HoaDon_ChiTietDTO>(),
                                }).ToList()
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        string mes = string.Concat("PostBH_HoaDon_SoQuy_Spa_NKySuDung ", e.InnerException, e.Message);
                        CookieStore.WriteLog(mes);
                        return Json(new
                        {
                            res = false,
                            mes,
                        });
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult Post_NhatKyHoatDongXe([FromBody] JObject data)
        {
            string sMaHoaDon = string.Empty;
            var idHoaDon = Guid.Empty;
            var insert = true;
            List<string> lstID_NewOld = new List<string>();// used to update again idChiTietGDDV

            HT_NhatKySuDung nky = new HT_NhatKySuDung();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                        List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                        ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                        ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);
                        ClassQuy_HoaDon_ChiTiet classQuyCT = new ClassQuy_HoaDon_ChiTiet(db);

                        int loaiHoaDon = objHoaDon.LoaiHoaDon;
                        // insert
                        if (objHoaDon.ID == Guid.Empty)
                        {
                            #region BH_HoaDon
                            objHoaDon.ID = Guid.NewGuid();
                            idHoaDon = objHoaDon.ID;
                            objHoaDon.ID_NhanVien = objHoaDon.ID_NhanVien == Guid.Empty ? null : objHoaDon.ID_NhanVien;
                            objHoaDon.ID_ViTri = objHoaDon.ID_ViTri == Guid.Empty ? null : objHoaDon.ID_ViTri;
                            objHoaDon.ID_DoiTuong = objHoaDon.ID_DoiTuong == null ? Guid.Empty : objHoaDon.ID_DoiTuong;
                            objHoaDon.NgayLapHoaDon = objHoaDon.NgayLapHoaDon;
                            objHoaDon.NgayTao = DateTime.Now;
                            objHoaDon.TyGia = 1;
                            objHoaDon.LoaiHoaDon = loaiHoaDon;
                            objHoaDon.ID_BangGia = objHoaDon.ID_BangGia == Guid.Empty ? null : objHoaDon.ID_BangGia;

                            if (objHoaDon.MaHoaDon == null || objHoaDon.MaHoaDon == "")
                            {
                                sMaHoaDon = classhoadon.SP_GetMaHoaDon_byTemp(objHoaDon.LoaiHoaDon, objHoaDon.ID_DonVi, objHoaDon.NgayLapHoaDon);
                            }
                            else
                            {
                                sMaHoaDon = classhoadon.GetMaHoaDon_Copy(objHoaDon.MaHoaDon);
                            }
                            objHoaDon.MaHoaDon = sMaHoaDon;
                            db.BH_HoaDon.Add(objHoaDon);

                            #endregion

                            #region BH_ChiTietHoaDon
                            foreach (var item in objCTHoaDon)
                            {
                                var giaban = item.DonGia - item.TienChietKhau;
                                var sMaLoHang = string.Empty;

                                List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    SoThuTu = item.SoThuTu,
                                    DonGia = item.DonGia,
                                    GiaVon = item.GiaVon,
                                    ID_HoaDon = objHoaDon.ID,
                                    SoLuong = item.SoLuong,
                                    ThanhTien = item.ThanhTien,
                                    ThanhToan = item.ThanhToan,
                                    PTChietKhau = item.PTChietKhau, // PT giam
                                    TienChietKhau = item.TienChietKhau, // tien giam 
                                    GhiChu = item.GhiChu,
                                    TangKem = item.TangKem,
                                    ID_TangKem = item.ID_TangKem,
                                    ID_KhuyenMai = item.ID_KhuyenMai,
                                    ID_LoHang = item.ID_LoHang,
                                    ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                                    PTThue = item.PTThue,
                                    TienThue = item.TienThue,
                                    LoaiThoiGianBH = item.LoaiThoiGianBH,
                                    ThoiGianBaoHanh = item.ThoiGianBaoHanh,
                                    ThoiGianThucHien = item.ThoiGianThucHien,
                                    ID_ViTri = item.ID_ViTri,
                                    ThoiGian = item.ThoiGian,
                                    ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                                    QuaThoiGian = item.QuaThoiGian,
                                    ChatLieu = item.ChatLieu,// 1.TraHD, 2.TraGoiDV, 3.HDXuLy DH, 4.Sudung GoiDV, else: empty/null
                                    DiemKhuyenMai = item.DiemKhuyenMai,
                                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                                    TenHangHoaThayThe = item.TenHangHoaThayThe,
                                    ID_LichBaoDuong = item.ID_LichBaoDuong,
                                    ThanhPhanComBo = item.ThanhPhanComBo
                                };

                                #region DinhLuong_DichVu

                                double? sumGiaVonDL = 0;

                                // Mua/tra gói dịch vụ: không lưu TP định lượng, vì để a Trịnh get giá vốn của dịch vụ (= tổng giá vốn của Tp dịch vụ)
                                // get TP_DinhLuong from .js
                                if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                                {
                                    // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                                    ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                    foreach (var itemDL in item.ThanhPhan_DinhLuong)
                                    {
                                        if (itemDL.SoLuong > 0)
                                        {
                                            BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                            {
                                                ID = Guid.NewGuid(),
                                                ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                ID_LoHang = itemDL.ID_LoHang,
                                                SoLuong = itemDL.SoLuong,
                                                GiaVon = itemDL.GiaVon,
                                                ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                ID_HoaDon = objHoaDon.ID,
                                                ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,// used to tính giá vốn dịch vụ khi sử dụng gdv
                                                GhiChu = itemDL.GhiChu,
                                                ChatLieu = ctHoaDon.ChatLieu,
                                                SoLuongDinhLuong_BanDau = itemDL.SoLuongDinhLuong_BanDau * ctHoaDon.SoLuong,
                                                TenHangHoaThayThe = itemDL.TenHangHoaThayThe,
                                            };
                                            lstCT.Add(ctHoaDon_DL);
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.ThanhPhanComBo != null && item.ThanhPhanComBo.Count > 0)
                                    {
                                        ctHoaDon.ID_ParentCombo = ctHoaDon.ID;
                                        var lstCombo = GetListCombo_andTPDLuong_ofThis(db, ctHoaDon, objHoaDon.ID_DonVi,
                                            objHoaDon.LoaiHoaDon, false, ref lstID_NewOld);
                                        lstCT.AddRange(lstCombo);
                                    }
                                    else
                                    {
                                        List<libDM_HangHoa.ClassDM_HangHoa.SP_ThanhPhan_DinhLuong> dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(objHoaDon.ID_DonVi, item.ID_DonViQuiDoi);

                                        if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                        {
                                            ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                                            foreach (var itemDL in dinhluongDV)
                                            {
                                                BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                                    ID_LoHang = itemDL.ID_LoHang,
                                                    SoLuong = itemDL.SoLuong * ctHoaDon.SoLuong,
                                                    GiaVon = itemDL.GiaVon,
                                                    ID_ChiTietDinhLuong = ctHoaDon.ID,
                                                    ID_HoaDon = objHoaDon.ID,
                                                    ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV, // save Tp định lượng of dich vụ
                                                    SoLuongDinhLuong_BanDau = itemDL.SoLuong * ctHoaDon.SoLuong,
                                                    ChatLieu = ctHoaDon.ChatLieu,
                                                    TenHangHoaThayThe = ctHoaDon.TenHangHoaThayThe,
                                                };
                                                lstCT.Add(ctHoaDon_DL);
                                            }

                                        }
                                    }
                                }

                                lstCT.Add(ctHoaDon);
                                db.BH_HoaDon_ChiTiet.AddRange(lstCT);
                                objHoaDon.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon
                                #endregion
                            }
                            #endregion
                        }

                        db.SaveChanges();
                        trans.Commit();

                        return Json(new
                        {
                            res = true,
                        });
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Json(new
                        {
                            res = false,
                            mes = e.Message + e.InnerException,
                        });
                    }
                }
            }
        }
        public List<BH_HoaDon_ChiTiet> GetListCombo_andTPDLuong_ofThis(SsoftvnContext db, BH_HoaDon_ChiTiet item,
            Guid idDonVi, int loaiHoaDon, bool isUpdate, ref List<string> lstID_NewOld)
        {
            ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);
            List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
            int stt = 1;
            foreach (var tpCombo in item.ThanhPhanComBo)
            {
                if (tpCombo.SoLuong > 0)
                {
                    BH_HoaDon_ChiTiet combo = new BH_HoaDon_ChiTiet
                    {
                        ID = Guid.NewGuid(),
                        SoThuTu = stt,
                        ID_DonViQuiDoi = tpCombo.ID_DonViQuiDoi,
                        ID_LoHang = tpCombo.ID_LoHang,
                        ID_ParentCombo = item.ID,
                        ID_HoaDon = item.ID_HoaDon,
                        ID_ChiTietGoiDV = tpCombo.ID_ChiTietGoiDV,
                        SoLuong = tpCombo.SoLuong,
                        GiaVon = tpCombo.GiaVon,
                        DonGia = tpCombo.DonGia,
                        PTChietKhau = tpCombo.PTChietKhau,
                        TienChietKhau = tpCombo.TienChietKhau,
                        PTThue = tpCombo.PTThue,
                        TienThue = tpCombo.TienThue,
                        ThanhTien = tpCombo.ThanhTien,
                        ThanhToan = tpCombo.ThanhToan,
                        GhiChu = tpCombo.GhiChu,
                        SoLuongDinhLuong_BanDau = tpCombo.SoLuongDinhLuong_BanDau * item.SoLuong,
                        TenHangHoaThayThe = tpCombo.TenHangHoaThayThe,
                        ChatLieu = item.ChatLieu,// used to check trahd/ tra gdv --> tinh tonkho
                    };
                    stt += 1;

                    if (isUpdate && tpCombo.ID != null && tpCombo.ID != Guid.Empty)
                    {
                        string sID = string.Concat(combo.ID, ",", tpCombo.ID, ";");
                        lstID_NewOld.Add(sID);
                    }

                    #region dinhluong of each combo
                    if (tpCombo.ThanhPhan_DinhLuong != null && tpCombo.ThanhPhan_DinhLuong.Count > 0)
                    {
                        double? sumGiaVonDL = 0;
                        foreach (var tpdl in tpCombo.ThanhPhan_DinhLuong)
                        {
                            if (tpdl.SoLuong > 0)
                            {
                                sumGiaVonDL += tpdl.GiaVon * tpdl.SoLuong;
                                BH_HoaDon_ChiTiet dluong = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = tpdl.ID_DonViQuiDoi,
                                    ID_LoHang = tpdl.ID_LoHang,
                                    SoLuong = tpdl.SoLuong,
                                    GiaVon = tpdl.GiaVon,
                                    ID_ParentCombo = null,
                                    ID_HoaDon = item.ID_HoaDon,
                                    ID_ChiTietDinhLuong = combo.ID,
                                    ID_ChiTietGoiDV = combo.ID_ChiTietGoiDV,
                                    GhiChu = tpdl.GhiChu,
                                    SoLuongDinhLuong_BanDau = tpdl.SoLuongDinhLuong_BanDau * combo.SoLuong,
                                    TenHangHoaThayThe = tpdl.TenHangHoaThayThe,
                                    ChatLieu = item.ChatLieu,// used to check trahd/ tra gdv --> tinh tonkho
                                };
                                lstCT.Add(dluong);

                                // tp dinhluong la dichvu (used to combo long combo)
                                var dinhluongDV = classHangHoa.SP_GetInfor_TPDinhLuong(idDonVi, tpdl.ID_DonViQuiDoi);
                                if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                {
                                    foreach (var itemDL in dinhluongDV)
                                    {
                                        var soluongTPDL = itemDL.SoLuong * dluong.SoLuong;
                                        sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                        BH_HoaDon_ChiTiet combo_DL = new BH_HoaDon_ChiTiet
                                        {
                                            ID = Guid.NewGuid(),
                                            ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                            ID_LoHang = itemDL.ID_LoHang,
                                            SoLuong = soluongTPDL,
                                            GiaVon = itemDL.GiaVon,
                                            ID_ChiTietDinhLuong = dluong.ID,
                                            ID_HoaDon = item.ID_HoaDon,
                                            ID_ChiTietGoiDV = dluong.ID_ChiTietGoiDV,
                                            SoLuongDinhLuong_BanDau = soluongTPDL,
                                            ChatLieu = item.ChatLieu,
                                        };
                                        lstCT.Add(combo_DL);
                                    }
                                }
                            }
                        }
                        combo.ID_ChiTietDinhLuong = combo.ID;
                        combo.GiaVon = sumGiaVonDL;
                    }
                    else
                    {
                        if (loaiHoaDon != 6)
                        {
                            // get TPdinhLuong in DB
                            var dinhluongDV = classHangHoa.SP_GetInfor_TPDinhLuong(idDonVi, combo.ID_DonViQuiDoi);
                            double? sumGiaVonDL = 0;
                            if (dinhluongDV != null && dinhluongDV.Count() > 0)
                            {
                                foreach (var itemDL in dinhluongDV)
                                {
                                    // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                    var soluongTPDL = itemDL.SoLuong * combo.SoLuong;

                                    // sum giavon all TP dinh luong
                                    sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                    BH_HoaDon_ChiTiet combo_DL = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                        ID_LoHang = itemDL.ID_LoHang,
                                        SoLuong = soluongTPDL,
                                        GiaVon = itemDL.GiaVon,
                                        ID_ChiTietDinhLuong = combo.ID,
                                        ID_HoaDon = item.ID_HoaDon,
                                        ID_ChiTietGoiDV = combo.ID_ChiTietGoiDV,
                                        SoLuongDinhLuong_BanDau = soluongTPDL,
                                        ChatLieu = item.ChatLieu,// used to check trahd/ tra gdv --> tinh tonkho
                                    };
                                    lstCT.Add(combo_DL);

                                    // tp dinhluong la dichvu (used to combo long combo)
                                    var dinhluongCB = classHangHoa.SP_GetInfor_TPDinhLuong(idDonVi, itemDL.ID_DonViQuiDoi);
                                    if (dinhluongCB != null && dinhluongCB.Count() > 0)
                                    {
                                        foreach (var tpdl in dinhluongCB)
                                        {
                                            var sluong = soluongTPDL * tpdl.SoLuong;
                                            sumGiaVonDL += tpdl.GiaVon * sluong;

                                            BH_HoaDon_ChiTiet itDL = new BH_HoaDon_ChiTiet
                                            {
                                                ID = Guid.NewGuid(),
                                                ID_DonViQuiDoi = tpdl.ID_DonViQuiDoi,
                                                ID_LoHang = tpdl.ID_LoHang,
                                                SoLuong = sluong,
                                                GiaVon = tpdl.GiaVon,
                                                ID_ChiTietDinhLuong = combo_DL.ID,
                                                ID_HoaDon = item.ID_HoaDon,
                                                ID_ChiTietGoiDV = combo.ID_ChiTietGoiDV,
                                                SoLuongDinhLuong_BanDau = sluong,
                                                ChatLieu = item.ChatLieu,
                                            };
                                            lstCT.Add(itDL);
                                        }
                                    }
                                }

                                combo.ID_ChiTietDinhLuong = combo.ID;
                                combo.GiaVon = tpCombo.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / tpCombo.SoLuong;
                            }
                        }
                    }
                    lstCT.Add(combo);
                    #endregion

                    #region BH_NhanVienThucHien of tpCombo
                    foreach (var itemNV in tpCombo.BH_NhanVienThucHien)
                    {
                        BH_NhanVienThucHien nvien = new BH_NhanVienThucHien
                        {
                            ID = Guid.NewGuid(),
                            ID_ChiTietHoaDon = combo.ID,
                            ID_NhanVien = itemNV.ID_NhanVien,
                            ThucHien_TuVan = itemNV.ThucHien_TuVan,
                            TienChietKhau = itemNV.TienChietKhau,
                            PT_ChietKhau = itemNV.PT_ChietKhau,
                            TheoYeuCau = itemNV.TheoYeuCau,
                            HeSo = itemNV.HeSo,
                            TinhChietKhauTheo = itemNV.TinhChietKhauTheo,
                            TinhHoaHongTruocCK = itemNV.TinhHoaHongTruocCK,
                        };
                        db.BH_NhanVienThucHien.Add(nvien);
                    }
                    #endregion
                }
            }
            return lstCT;
        }

        [HttpPost, HttpGet]
        public IHttpActionResult UpdateHoaDon_OpenFromList([FromBody] JObject data, Guid? idNhanVien = null)
        {
            try
            {
                BH_HoaDon objHoaDon = data["objHoaDon"].ToObject<BH_HoaDon>();
                List<BH_HoaDon_ChiTiet> objCTHoaDon = data["objCTHoaDon"].ToObject<List<BH_HoaDon_ChiTiet>>();
                bool isSetGiaVonTB = data["IsSetGiaVonTrungBinh"].ToObject<bool>();
                string style1 = "<a style= \"cursor: pointer\" onclick = \"";
                string style2 = "('";
                string style3 = "')\" >";
                string style4 = "</a>";
                string styleMaHD = string.Empty;
                string styleHangHoa = string.Empty;
                string inforOld = string.Empty;
                List<string> lstID_NewOld = new List<string>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                    ClassBH_HoaDon_ChiTiet classhoadonchitiet = new ClassBH_HoaDon_ChiTiet(db);
                    classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                    ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                    ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);
                    ClassQuy_HoaDon_ChiTiet classQuyCT = new ClassQuy_HoaDon_ChiTiet(db);
                    DateTime ngaylapHD = objHoaDon.NgayLapHoaDon.AddMilliseconds(DateTime.Now.Millisecond);
                    BH_HoaDon hdOld = db.BH_HoaDon.Find(objHoaDon.ID);
                    string err = string.Empty;

                    #region "Get cthd old was delete"
                    var cthdOld = classhoadonchitiet.Gets(x => x.ID_HoaDon == objHoaDon.ID); // get cthd old
                    // if date new < date old: date old = date new - milisencond
                    var idOldCustomer = hdOld.ID_DoiTuong;
                    var idNewCustomer = objHoaDon.ID_DoiTuong;
                    var ngaylapOld = hdOld.NgayLapHoaDon;
                    string sDateOld = hdOld.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                    string sDateNew = objHoaDon.NgayLapHoaDon.ToString("yyyy-MM-dd HH:mm");
                    if (string.Compare(sDateNew, sDateOld) == 0)
                    {
                        ngaylapHD = hdOld.NgayLapHoaDon;
                    }
                    // compare cthd old & new --> get cthd was delete
                    var cthdDelete = (from ctold in cthdOld
                                      join ctnew in objCTHoaDon on
                                      new { ctold.ID_DonViQuiDoi, ctold.ID_LoHang }
                                      equals new { ctnew.ID_DonViQuiDoi, ctnew.ID_LoHang }
                                      into ctDelete
                                      from de in ctDelete.DefaultIfEmpty()
                                      where de == null
                                      select ctold).ToList();

                    var ctDelete_newID = cthdDelete.Select(x =>
                   new BH_HoaDon_ChiTiet
                   {
                       ID = Guid.NewGuid(),
                       ID_HoaDon = x.ID_HoaDon,
                       ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                       ID_LoHang = x.ID_LoHang,
                       SoLuong = x.SoLuong,
                       PTChietKhau = x.PTChietKhau,
                       TienChietKhau = x.TienChietKhau,
                       ThanhTien = x.ThanhTien,
                       ThanhToan = x.ThanhToan,
                       ChatLieu = "5", // ct delete assign chatlie="5" !important
                       GiaVon = x.GiaVon,
                       TienThue = x.TienThue,
                       PTChiPhi = x.PTChiPhi,
                       TienChiPhi = x.TienChiPhi,
                   }).ToList();

                    #region baoduong_change
                    // get cthd chuyen tu baoduong --> khong baoduong
                    var cthdUpdate = (from ctold in cthdOld
                                      join ctnew in objCTHoaDon on
                                      new { ctold.ID_DonViQuiDoi, ctold.ID_LoHang }
                                      equals new { ctnew.ID_DonViQuiDoi, ctnew.ID_LoHang }
                                      where ctold.ID_LichBaoDuong != null && ctnew.ID_LichBaoDuong == null
                                      select ctold).ToList();
                    var lstLichOld = cthdUpdate.Select(x => x.ID_LichBaoDuong).ToList();
                    var hangthaymoi = cthdUpdate.Select(x => x.ID_DonViQuiDoi).ToList();

                    // update trangthai = 4 (neu chuyen tu baoduong --> khong baoduong)
                    db.Gara_LichBaoDuong.Where(x => lstLichOld.Contains(x.ID)).ToList()
                        .ForEach(x => x.TrangThai = 4);

                    // update trangthai = 0 (neu baoduong --> nhung bi xoa)
                    var bdDelete = cthdOld.Where(x => x.ID_LichBaoDuong != null &&
                    cthdDelete.Select(y => y.ID).Contains(x.ID)).Select(x => x.ID_LichBaoDuong);
                    db.Gara_LichBaoDuong.Where(x => bdDelete.Contains(x.ID)).ToList()
                        .ForEach(x => x.TrangThai = 0);
                    #endregion

                    // style diary inforHD old
                    try
                    {
                        inforOld = "Thông tin hóa đơn cũ: <br /> " + db.Database.SqlQuery<string>(" SELECT dbo.Diary_GetInforOldInvoice('" + objHoaDon.ID + "')").First();
                    }
                    catch (Exception e)
                    {
                        CookieStore.WriteLog("Diary_GetInforOldInvoice " + e.InnerException + e.Message);
                    }
                    #endregion

                    objHoaDon.NgayLapHoaDon = ngaylapHD;
                    BH_HoaDon objUpHD = classhoadon.Update_HoaDon_DatHang(objHoaDon);

                    // if change ID_khachhang--> update IDKhach new to Soquy
                    if (idOldCustomer != idNewCustomer)
                    {
                        err = classQuyCT.UpdateIDKhachHang_inSoQuy(objHoaDon.ID);
                    }

                    if (err != string.Empty)
                    {
                        CookieStore.WriteLog("UpdateIDKhachHang_inSoQuy " + err);
                        return Json(new
                        {
                            res = false,
                            mes = err,
                        });
                    }

                    styleMaHD = string.Concat(style1, "LoadHoaDon_byMaHD", style2, objUpHD.MaHoaDon, style3, objUpHD.MaHoaDon, style4);

                    #region "BH_NhanVienThucHien of HoaDon
                    // delete all CTHD with ID_HoaDon
                    err = classhoadonchitiet.Delete_HoaDon_ChiTiet_ByIDHoaDon(objHoaDon.ID);
                    if (err != string.Empty)
                    {
                        CookieStore.WriteLog("Delete_HoaDon_ChiTiet_ByIDHoaDon " + err);
                        return Json(new
                        {
                            res = false,
                            mes = err,
                        });
                    }

                    #endregion

                    #region " BH_HoaDon_ChiTiet "


                    // insert again cthd old was delete (ChatLieu = 5) into hdUpdate --> caculator TonLuyKe
                    err = classhoadonchitiet.Add_ChiTietHoaDon(ctDelete_newID);

                    if (err != string.Empty)
                    {
                        return Json(new
                        {
                            res = false,
                            mes = err,
                        });
                    }

                    foreach (var item in objCTHoaDon)
                    {
                        var giaban = item.DonGia - item.TienChietKhau;
                        var sMaLoHang = string.Empty;

                        List<BH_HoaDon_ChiTiet> lstCT = new List<BH_HoaDon_ChiTiet>();
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            SoThuTu = item.SoThuTu,
                            DonGia = item.DonGia,
                            GiaVon = item.GiaVon,
                            ID_HoaDon = objHoaDon.ID,
                            SoLuong = item.SoLuong,
                            ThanhTien = item.ThanhTien,
                            ThanhToan = item.ThanhToan,
                            PTChietKhau = item.PTChietKhau,
                            TienChietKhau = item.TienChietKhau,
                            GhiChu = item.GhiChu,
                            TangKem = item.TangKem,
                            ID_TangKem = item.ID_TangKem,
                            ID_KhuyenMai = item.ID_KhuyenMai,
                            ID_LoHang = item.ID_LoHang,
                            ID_ChiTietGoiDV = item.ID_ChiTietGoiDV,
                            PTThue = item.PTThue,
                            TienThue = item.TienThue,
                            LoaiThoiGianBH = item.LoaiThoiGianBH,
                            ThoiGianBaoHanh = item.ThoiGianBaoHanh,
                            ThoiGianThucHien = item.ThoiGianThucHien,
                            ID_ViTri = item.ID_ViTri,
                            ThoiGian = item.ThoiGian,
                            ThoiGianHoanThanh = item.ThoiGianHoanThanh,
                            QuaThoiGian = item.QuaThoiGian,
                            ChatLieu = item.ChatLieu,// 1.TraHD, 2.TraGoiDV, 3.HDXuLy DH, 4.Sudung GoiDV, else: empty/null
                            DiemKhuyenMai = item.DiemKhuyenMai,
                            DonGiaBaoHiem = item.DonGiaBaoHiem,
                            TenHangHoaThayThe = item.TenHangHoaThayThe,
                            ID_LichBaoDuong = item.ID_LichBaoDuong,
                            ThanhPhanComBo = item.ThanhPhanComBo,
                        };

                        if (item.ID != null && item.ID != Guid.Empty)
                        {
                            string sID = string.Concat(ctHoaDon.ID, ",", item.ID, ";");
                            lstID_NewOld.Add(sID);
                        }

                        #region DinhLuong_DichVu

                        double? sumGiaVonDL = 0;

                        // get TP_DinhLuong from .js
                        if (item.ThanhPhan_DinhLuong != null && item.ThanhPhan_DinhLuong.Count > 0)
                        {
                            // nếu là dịch vụ: save ID_ChiTietDinhLuong = ct.ID (new Guid)
                            ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;

                            foreach (var itemDL in item.ThanhPhan_DinhLuong)
                            {
                                if (itemDL.SoLuong > 0)
                                {
                                    // đã nhân số lượng ở .js (show in view)
                                    var soluongTPDL = itemDL.SoLuong;

                                    // sum giavon all TP dinh luong
                                    sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                    BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                        ID_LoHang = itemDL.ID_LoHang,
                                        SoLuong = soluongTPDL,
                                        GiaVon = itemDL.GiaVon,
                                        ID_ChiTietDinhLuong = ctHoaDon.ID,
                                        ID_HoaDon = objHoaDon.ID,
                                        ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                        GhiChu = itemDL.GhiChu,
                                        ChatLieu = ctHoaDon.ChatLieu,
                                        SoLuongDinhLuong_BanDau = itemDL.SoLuongDinhLuong_BanDau * ctHoaDon.SoLuong,
                                        TenHangHoaThayThe = itemDL.TenHangHoaThayThe,
                                    };
                                    lstCT.Add(ctHoaDon_DL);
                                }
                            }
                        }
                        else
                        {
                            if (ctHoaDon.ThanhPhanComBo != null && ctHoaDon.ThanhPhanComBo.Count > 0)
                            {
                                ctHoaDon.ID_ParentCombo = ctHoaDon.ID;
                                var lstCombo = GetListCombo_andTPDLuong_ofThis(db, ctHoaDon, objHoaDon.ID_DonVi,
                                    objHoaDon.LoaiHoaDon, true, ref lstID_NewOld);
                                lstCT.AddRange(lstCombo);
                            }
                            else
                            {
                                // get TPdinhLuong in DB
                                var dinhluongDV = _classDMHH.SP_GetInfor_TPDinhLuong(objHoaDon.ID_DonVi, item.ID_DonViQuiDoi);
                                if (dinhluongDV != null && dinhluongDV.Count() > 0)
                                {
                                    ctHoaDon.ID_ChiTietDinhLuong = ctHoaDon.ID;
                                    foreach (var itemDL in dinhluongDV)
                                    {
                                        // dùng SoLuong bao nhiêu --> gấp TPDinhLuong lên số lần đó
                                        var soluongTPDL = itemDL.SoLuong * ctHoaDon.SoLuong;

                                        // sum giavon all TP dinh luong
                                        sumGiaVonDL += itemDL.GiaVon * soluongTPDL;

                                        BH_HoaDon_ChiTiet ctHoaDon_DL = new BH_HoaDon_ChiTiet
                                        {
                                            ID = Guid.NewGuid(),
                                            ID_DonViQuiDoi = itemDL.ID_DonViQuiDoi,
                                            ID_LoHang = itemDL.ID_LoHang,
                                            SoLuong = soluongTPDL,
                                            GiaVon = itemDL.GiaVon,
                                            ID_ChiTietDinhLuong = ctHoaDon.ID,
                                            ID_HoaDon = objHoaDon.ID,
                                            ID_ChiTietGoiDV = ctHoaDon.ID_ChiTietGoiDV,
                                            SoLuongDinhLuong_BanDau = soluongTPDL,
                                            ChatLieu = ctHoaDon.ChatLieu,
                                            TenHangHoaThayThe = ctHoaDon.TenHangHoaThayThe,
                                        };
                                        lstCT.Add(ctHoaDon_DL);
                                    }
                                }
                            }
                        }

                        // GiaVon of dich vu = (sum GiaVon of all TP DinhLuong) / SoLuong CTHD
                        if (isSetGiaVonTB == false)
                        {
                            ctHoaDon.GiaVon = item.SoLuong == 0 ? sumGiaVonDL : sumGiaVonDL / item.SoLuong;
                        }

                        lstCT.Add(ctHoaDon);
                        err = classhoadonchitiet.Add_ChiTietHoaDon(lstCT);
                        objUpHD.BH_HoaDon_ChiTiet.Add(ctHoaDon); // add lstCTHD into HoaDon

                        if (item.ID_LoHang != null)
                        {
                            sMaLoHang = string.Concat(" (Số lô: ", item.MaLoHang, ")");
                        }
                        styleHangHoa += string.Concat(style1, "loadHangHoabyMaHH", style2, item.MaHangHoa, style3, item.MaHangHoa, style4, sMaLoHang, ": Số lượng: ", item.SoLuong, ", Giá bán: ", giaban == 0 ? "0" : giaban.ToString("#,#", CultureInfo.InvariantCulture), "<br />");

                        #endregion

                        #region BH_NhanVienThucHien of HangHoa
                        foreach (var itemNV in item.BH_NhanVienThucHien)
                        {
                            BH_NhanVienThucHien nvien = new BH_NhanVienThucHien
                            {
                                ID = Guid.NewGuid(),
                                ID_ChiTietHoaDon = ctHoaDon.ID,
                                ID_NhanVien = itemNV.ID_NhanVien,
                                ThucHien_TuVan = itemNV.ThucHien_TuVan,
                                TienChietKhau = itemNV.TienChietKhau,
                                PT_ChietKhau = itemNV.PT_ChietKhau,
                                TheoYeuCau = itemNV.TheoYeuCau,
                                HeSo = itemNV.HeSo,
                                TinhChietKhauTheo = itemNV.TinhChietKhauTheo,
                                TinhHoaHongTruocCK = itemNV.TinhHoaHongTruocCK,
                            };
                            nhanvienThucHien.Insert(nvien);
                        }
                        #endregion
                    }
                    #endregion

                    #region "NhatKy ThaoTac"
                    string tenChucNang = string.Empty;
                    switch (objHoaDon.LoaiHoaDon)
                    {
                        case 1:
                        case 0:
                            tenChucNang = "Bán hàng";
                            break;
                        case 3:
                            tenChucNang = "Báo giá";
                            break;
                        case 6:
                            tenChucNang = "Trả hàng";
                            break;
                        case 19:
                            tenChucNang = "Gói dịch vu";
                            break;
                        case 25:
                            tenChucNang = "Hóa đơn sửa chữa";
                            break;
                    }
                    string noiDung = string.Concat("Cập nhật hóa đơn ", objUpHD.MaHoaDon, " Giá trị: ", objHoaDon.PhaiThanhToan == 0 ? "0" : objHoaDon.PhaiThanhToan.ToString("#,#", CultureInfo.InvariantCulture), ", Thời gian: ", objHoaDon.NgayLapHoaDon);
                    HT_NhatKySuDung nky = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = objHoaDon.ID_DonVi,
                        LoaiHoaDon = objHoaDon.LoaiHoaDon,
                        ID_NhanVien = idNhanVien != null && idNhanVien != Guid.Empty ? idNhanVien : objHoaDon.ID_NhanVien,
                        ChucNang = tenChucNang,
                        LoaiNhatKy = 2,
                        NoiDung = noiDung,
                        NoiDungChiTiet = string.Concat(noiDung, " bao gồm: <br />", styleHangHoa, inforOld),
                        ThoiGian = DateTime.Now,
                        ID_HoaDon = objHoaDon.ID,
                        ThoiGianUpdateGV = ngaylapOld,
                    };
                    db.HT_NhatKySuDung.Add(nky);
                    db.SaveChanges();
                    new SaveDiary().AddQueueJob(nky);

                    //if (objHoaDon.LoaiHoaDon == 19)
                    //{
                    // update id_chitietgdv if update again goidichvu
                    // gdv da sudung nhung muon capnhat lai
                    //classhoadonchitiet.UpdateIDCTNew_forCTOld(lstID_NewOld);
                    //}

                    #endregion
                    var cthd = db.BH_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == objHoaDon.ID)
                        .Select(x => new BH_HoaDon_ChiTietDTO
                        {
                            ID = x.ID,
                            ID_DonViQuiDoi = x.ID_DonViQuiDoi,
                            ID_ChiTietGoiDV = x.ID_ChiTietGoiDV,
                            ID_LoHang = x.ID_LoHang,
                            ID_ParentCombo = x.ID_ParentCombo,
                            ID_ChiTietDinhLuong = x.ID_ChiTietDinhLuong,
                            DonGia = x.DonGia,
                            SoLuong = x.SoLuong,
                            ThanhTien = x.ThanhTien,
                        }).ToList();

                    return Json(new
                    {
                        res = true,
                        mes = string.Empty,
                        data = new
                        {
                            objHoaDon.ID,
                            objUpHD.MaHoaDon,
                            objUpHD.ID_NhanVien,
                            objUpHD.ID_DoiTuong,
                            objHoaDon.ID_ViTri,
                            objHoaDon.DienGiai,
                            objHoaDon.PhaiThanhToan,
                            objHoaDon.TongChietKhau,
                            objHoaDon.TongGiamGia,
                            objHoaDon.TongChiPhi,
                            objHoaDon.TongTienHang,
                            objHoaDon.ChoThanhToan,
                            NgayLapHoaDon = ngaylapHD,
                            NgayLapHoaDonOld = ngaylapOld,
                            BH_HoaDon_ChiTiet = cthd,
                            hangthaymoi,
                        }
                    });
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog(string.Concat("UpdateHoaDon_OpenFromList: ", e.InnerException, e.Message));
                return Json(new
                {
                    res = false,
                    mes = e.InnerException + e.Message,
                });
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult Post_BHNhanVienThucHien([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<BH_NhanVienThucHien> lstAdd = new List<BH_NhanVienThucHien>();
                    ClassBH_NhanVienThucHien nhanvienThucHien = new ClassBH_NhanVienThucHien(db);
                    List<BH_NhanVienThucHien> lstObj = data["lstObj"].ToObject<List<BH_NhanVienThucHien>>();
                    foreach (var item in lstObj)
                    {
                        item.ID = Guid.NewGuid();
                        lstAdd.Add(item);
                    }
                    nhanvienThucHien.Inserts(lstAdd);
                    return Json(new
                    {
                        res = true,
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    res = false,
                    mess = e.InnerException + e.Message,
                });
            }
        }

        [HttpPost, HttpGet]
        public string UpdateViTri_CTHD(Guid idCTHD, Guid idViTri)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    BH_HoaDon_ChiTiet objUpd = db.BH_HoaDon_ChiTiet.Find(idCTHD);
                    objUpd.ID_ViTri = idViTri;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return string.Concat("HoaDonAPI_UpdateViTri_CTHD ", ex.InnerException, ex.InnerException);
            }
            return string.Empty;
        }
        [HttpGet]
        public IHttpActionResult GetListImgInvoice(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var _classDMHH = new ClassDM_HangHoa(db);
                    var lstAnh = db.BH_HoaDon_Anh.Where(x => x.IdHoaDon == id).Select(x => new { x.Id, x.IdHoaDon, x.URLAnh }).ToList();
                    return ActionTrueData(lstAnh);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        [HttpGet, HttpPost]
        public IHttpActionResult GetListImgInvoice_byCus(ParamNKyGDV param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                    List<SP_InforServicePackage> lst = classHoaDon.GetListImgInvoice_byCus(param);
                    int count = 0, page = 0;
                    int currentPage = param.CurrentPage ?? 0, pageSize = param.PageSize ?? 20;
                    if (lst != null && lst.Count > 0)
                    {
                        count = lst[0].TotalRow ?? 0;
                    }
                    var listpage = GetListPage(count, pageSize, currentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = lst,
                        listpage,
                        pagenow = currentPage,
                        pageview = "Hiển thị " + (currentPage * pageSize + 1) + " - " + (currentPage * pageSize + lst.Count) + " trên tổng số " + count + " bản ghi",
                        isprev = currentPage > 3 && page > 5,
                        isnext = currentPage < page - 2 && page > 5,
                        countpage = page
                    });
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        [HttpGet]
        public IHttpActionResult DeleteImgInvoice(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var _classDMHH = new ClassDM_HangHoa(db);
                    var obj = db.BH_HoaDon_Anh.Find(id);
                    if (obj != null)
                    {
                        db.BH_HoaDon_Anh.Remove(obj);
                        db.SaveChanges();
                    }
                    return ActionTrueData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteImgInvoice([FromBody] List<Guid> imgs)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<BH_HoaDon_Anh> lst = new List<BH_HoaDon_Anh>();
                        lst = db.BH_HoaDon_Anh.Where(p => imgs.Contains(p.Id)).ToList();
                        db.BH_HoaDon_Anh.RemoveRange(lst);
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }

        [HttpPost]
        public IHttpActionResult UploadImgInvoice([FromBody] List<string> files, Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<BH_HoaDon_Anh> lst = new List<BH_HoaDon_Anh>();
                        for (int i = 0; i < files.Count; i++)
                        {
                            BH_HoaDon_Anh objAnh = new BH_HoaDon_Anh();
                            objAnh.Id = Guid.NewGuid();
                            objAnh.IdHoaDon = id;
                            objAnh.URLAnh = files[i];
                            objAnh.NgayTao = DateTime.Now;
                            lst.Add(objAnh);
                        }
                        db.BH_HoaDon_Anh.AddRange(lst);
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }
        #endregion

        #region delete
        [System.Web.Http.HttpDelete, HttpGet]
        public IHttpActionResult DeleteBH_HoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                string strDel = classhoadon.Delete_HoaDon(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // delete HoaDon by NgayLapHoaDon (use when timeout)
        [System.Web.Http.HttpDelete, HttpPost]
        public string DeleteBH_HoaDon_ByNgayLapHD(string ngayLapHD, string idChiNhanh, string loaiHoaDon, string userLogin)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.SP_DeleteHoaDon_WhenTimeOut(ngayLapHD, idChiNhanh, loaiHoaDon, userLogin);
            }
        }

        [System.Web.Http.HttpDelete, HttpPost, HttpGet]
        public string DeleteHoaDon_ByID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                return classhoadon.SP_DeleteHoaDon_byID(id);
            }
        }
        #endregion

        #region ###

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        // get temp path
        public string GetTempPath()
        {
            return CommonStatic.GetTempPath();
        }

        [HttpPost, HttpGet]
        public void UpdateAppcache()
        {
            try
            {
                banhang24.AppCache.EventUpdateCache.CreatFIleAppcache();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("UpdateAppcache " + ex.InnerException + ex.Message);
            }
        }

        [HttpGet]
        public string GetLocalIPAddress()
        {
            string localIP;
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
            }
            catch (Exception e)
            {
                localIP = "127.01.0.1";
            }
            return localIP;
        }

        [banhang24.Compress.DeflateCompression]
        [HttpGet, HttpPost]
        public IHttpActionResult BanHang_ManyFunctionDB_PageLoad(Guid idChiNhanh)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classDM_DoiTuong classDM_DoiTuong = new classDM_DoiTuong(db);
                    ClassDM_NhomDoiTuong classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                    ClassDM_NguonKhach classNguonKhach = new ClassDM_NguonKhach(db);
                    ClassHT_CongTy _classCT = new ClassHT_CongTy(db);

                    if (db != null)
                    {
                        var chotso = (from x in db.ChotSo
                                      where x.ID_DonVi == idChiNhanh
                                      select new
                                      {
                                          x.ID,
                                          x.ID_DonVi,
                                          x.NgayChotSo
                                      }).ToList();
                        var cty = _classCT.Gets(null).Select(x => new
                        {
                            ID = x.ID,
                            TaiKhoanNH = x.TaiKhoanNganHang,
                            TenCongTy = x.TenCongTy,
                            SoDienThoai = x.SoDienThoai,
                            DiaChiNganHang = x.DiaChiNganHang,
                            DiaChi = x.DiaChi
                        }).ToList();
                        var nguonkhach = classNguonKhach.Gets(null).Select(x => new { x.ID, x.TenNguonKhach }).ToList();
                        var tinhthanh = classDM_DoiTuong.GetListTinhThanh(null).Select(x => new { x.ID, x.TenTinhThanh }).ToList();
                        var nhomdoituong = classNhomDoiTuong.Gets(null)
                             .Where(type => (type.LoaiDoiTuong == 1 || type.LoaiDoiTuong == 0)
                             && (type.TrangThai == true || type.TrangThai == null))
                             .Select(x => new
                             {
                                 ID = x.ID,
                                 TenNhomDoiTuong = x.TenNhomDoiTuong,
                                 MaNhomDoiTuong = x.MaNhomDoiTuong,
                                 GiamGia = x.GiamGia == null ? 0 : x.GiamGia,
                                 GiamGiaTheoPhanTram = x.GiamGiaTheoPhanTram,
                                 TuDongCapNhat = x.TuDongCapNhat,
                                 GhiChu = x.GhiChu,
                             }).ToList();

                        var vungmien = (from tt in db.DM_VungMien
                                        select new
                                        {
                                            tt.ID,
                                            tt.TenVung
                                        }).ToList();

                        var cauhinhtrahang = (from ct in db.HT_CauHinh_GioiHanTraHang
                                              join ch in db.HT_CauHinhPhanMem on ct.ID_CauHinh equals ch.ID
                                              where ch.ID_DonVi == idChiNhanh
                                              select new
                                              {
                                                  ct.ID,
                                                  ct.SoNgayGioiHan,
                                                  ct.ChoPhepTraHang,
                                              }).ToList();
                        return Json(new { res = true, ChotSo = chotso, CongTy = cty, NguonKhach = nguonkhach, TinhThanh = tinhthanh, NhomDoiTuong = nhomdoituong, VungMien = vungmien, CauHinhTraHang = cauhinhtrahang });
                    }
                    else
                    {
                        return Json(new { res = false });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        public IHttpActionResult GetChungTuApDung()
        {
            List<KeyValuePair<int, string>> data = null;
            try
            {
                data = data = commonEnum.ListChungTuApDung.ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetChungTuApDung " + ex.InnerException + ex.Message);
            }
            return Json(data);
        }

        public IHttpActionResult Get_LoaiTinhChietKhau_byIDHoaDon(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return Json(new { res = false });
                }
                else
                {
                    try
                    {
                        var data = from nvth in db.BH_NhanVienThucHien
                                   where nvth.ID_HoaDon == idHoaDon
                                   //group nvth.ID_HoaDon into g
                                   select nvth.TinhChietKhauTheo;
                        if (data != null && data.Count() > 0)
                        {
                            return Json(new { res = true, value = data.Min().Value });
                        }
                        else
                        {
                            return Json(new { res = true, value = 1 });
                        }
                    }
                    catch (Exception e)
                    {
                        return Json(new { res = false, mes = e.InnerException + e.Message });
                    }
                }
            }
        }

        [HttpGet]
        public IHttpActionResult CheckGoiDV_isUsed(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return Json(new { res = false, mes = "DB null" });
                }
                else
                {
                    try
                    {
                        var data = from ct in db.BH_HoaDon_ChiTiet
                                   join hd in db.BH_HoaDon on ct.ID_HoaDon equals hd.ID
                                   join ctsd in db.BH_HoaDon_ChiTiet on ct.ID equals ctsd.ID_ChiTietGoiDV
                                   join hdsu in db.BH_HoaDon on ctsd.ID_HoaDon equals hdsu.ID
                                   where hd.ID == idHoaDon && hdsu.ChoThanhToan == false
                                   select ct.ID;
                        if (data != null && data.Count() > 0)
                        {
                            return Json(new { res = true });
                        }
                        else
                        {
                            return Json(new { res = false, mes = string.Empty });
                        }
                    }
                    catch (Exception e)
                    {
                        return Json(new { res = false, mes = e.InnerException + e.Message });
                    }
                }
            }
        }

        #endregion

        //public void AddQueueJob(HT_NhatKySuDung nky)
        //{
        //    Model_banhang24vn.DAL.QueueJobService queueJobService = new Model_banhang24vn.DAL.QueueJobService();
        //    Model_banhang24vn.QueueJob qj = new Model_banhang24vn.QueueJob();
        //    string subdomain = CookieStore.GetCookieAes("SubDomain");
        //    qj.ID = Guid.NewGuid();
        //    qj.IDNhatKySuDung = nky.ID;
        //    qj.SoLanDaChay = 0;
        //    qj.Subdomain = subdomain;
        //    qj.ThoiGianTao = DateTime.Now;
        //    qj.TrangThai = 0;
        //    queueJobService.Insert(qj);
        //    HttpClient httpClient = new HttpClient();
        //    httpClient.BaseAddress = new Uri("https://qj.open24.vn/");
        //    //httpClient.BaseAddress = new Uri("https://localhost:44309/");
        //    httpClient.DefaultRequestHeaders.Accept.Clear();
        //    httpClient.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/json"));
        //    httpClient.GetAsync("api/Queues/AddQueue/" + subdomain);
        //}
    }

    public class DM_GiaBan_NhapHang_DTO
    {
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid ID_GiaBan { get; set; }
        public string TenGiaBan { get; set; }
        public double? GiaBan { get; set; }
    }

    public class JsonResultExampleCH
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public double TongTienHang { get; set; }
        public double TongChiPhi { get; set; }
        public double TongGiamGia { get; set; }
        public double TongGiamGiaKM { get; set; }
        public double TongKhachTra { get; set; }
        public double TongThanhToan { get; set; }
        public double TongKhachNo { get; set; }
        public double TongPhaiTraKhach { get; set; } // lst TraHang
        public List<BH_HoaDonDTO> lstCH { get; set; }
    }

    public class JsonResulSMSKHDTO
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<DoiTuongSMSDTO> lst { get; set; }
    }

    public class JsonResulSMSGiaoDich
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<DoiTuongSMSGiaoDich> lst { get; set; }
    }

    public class JSONTheKho
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<DM_TheKhoDTO> lst { get; set; }
    }

    public class JsonResultExampleNapTien
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<BH_HoaDonTheNapDTO> lst { get; set; }
    }

    public class JsonResultExampleNhatKySDThe
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<BH_NhatKySDTheDTO> lst { get; set; }
        public double TongTienTang { get; set; }
        public double TongTienGiam { get; set; }
    }

    public class JsonResultExampleLsNapThe
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<BH_HoaDonTheNapDTO> lst { get; set; }
    }

    public class JsonResultExampleLichSuNapTien
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public IQueryable<Object> lst { get; set; }
    }

    public class JsonResultChuyenNhanTien
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public double? TongTienNap { get; set; }
        public double? TongTienChuyen { get; set; }
        public double? TongTienDungGuiTien { get; set; }
        public double? TongTienNhan { get; set; }
        public double? TongTienConLai { get; set; }
        public List<HeThong_SMS_TaiKhoanDTO> lstChuyenNhan { get; set; }
    }

    public class JsonResultNapTien
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public IQueryable<Object> lst { get; set; }
    }

    public class HeThong_SMS_TaiKhoanDTO
    {
        public Guid ID { get; set; }
        public Guid ID_NguoiChuyenTien { get; set; }
        public Guid ID_NguoiNhanTien { get; set; }
        public double SoTien { get; set; }
        public DateTime ThoiGian { get; set; }
        public string GhiChu { get; set; }
        public string NguoiChuyenTien { get; set; }
        public string NguoiNhanTien { get; set; }
    }

    public class SP_CompareDoanhThuThe
    {
        public double TongDoanhThu { get; set; }
        public double DoanhThuCompare { get; set; }
    }
}