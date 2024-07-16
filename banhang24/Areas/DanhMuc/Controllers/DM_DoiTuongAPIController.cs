using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Model;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using libDM_DoiTuong;
using libQuy_HoaDon;
using System.Data;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Data.SqlClient;
using libDM_NhomDoiTuong;
using System.Threading;
using libReport;
using banhang24.Hellper;
using lib_ChamSocKhachHang;
using libHT_NguoiDung;
using banhang24.Compress;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_DoiTuongAPIController : BaseApiController
    {
        #region GET
        //trinhpv getlist NhomDoiTuong
        public List<DM_NhomDoiTuongDTO> getList_NhomDoiTuongbyID(Guid ID_NhomDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                List<DM_NhomDoiTuongDTO> lst = new List<DM_NhomDoiTuongDTO>();
                lst = classdoituong.getList_NhomDoiTuongbyID(ID_NhomDoiTuong);
                return lst;
            }
        }

        [HttpGet]
        public IHttpActionResult GetDM_DoiTuongByPhone(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong dt = new classDM_DoiTuong(db);
                DM_DoiTuong dtresult = dt.Get(p => p.DienThoai == id && p.LoaiDoiTuong == 1 && p.TheoDoi == false);
                if (dtresult != null)
                {
                    SP_DM_DoiTuong spdoituong = new SP_DM_DoiTuong();
                    spdoituong.ID = dtresult.ID;
                    spdoituong.MaDoiTuong = dtresult.MaDoiTuong;
                    spdoituong.TenDoiTuong = dtresult.TenDoiTuong;
                    spdoituong.DiaChi = dtresult.DiaChi;
                    spdoituong.NgaySinh_NgayTLap = dtresult.NgaySinh_NgayTLap;
                    spdoituong.DienThoai = dtresult.DienThoai;
                    return ActionTrueData(spdoituong);
                }
                return ActionTrueData(dtresult);
            }
        }

        [HttpGet, HttpPost]
        public List<JqAuto_DMDoiTuong> JqAuto_SearchDoiTuong(int loaiDoiTuong, string txtSearch, Guid? idChiNhanh = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    txtSearch = string.Concat("%", txtSearch, "%");
                    return new classDM_DoiTuong(db).JqAuto_SearchDoiTuong(loaiDoiTuong, txtSearch, idChiNhanh);
                }
                catch (Exception)
                {
                    return new List<JqAuto_DMDoiTuong>();
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IEnumerable<Object> getKhachHangSNByNgay(DateTime ngaysinh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                var monthFrom = ngaysinh.Month;
                var dateFrom = ngaysinh.Day;
                return classdoituong.Gets(p => p.NgaySinh_NgayTLap.Value.Month == monthFrom && p.NgaySinh_NgayTLap.Value.Day == dateFrom && p.LoaiDoiTuong == 1 && !string.IsNullOrEmpty(p.DienThoai)).Select(x => new { ID = x.ID, TenDoiTuong = x.TenDoiTuong, DienThoai = x.DienThoai }).ToList();
            }
        }

        [HttpPost]
        public IHttpActionResult GetCustomer_haveBirthday(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                    List<JqAuto_DMDoiTuong> lst = classDoiTuong.GetCustomer_haveBirthday(param);
                    return ActionTrueData(lst);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        [HttpPost]
        public IHttpActionResult GetCustomer_haveTransaction(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                    List<JqAuto_DMDoiTuong> lst = classDoiTuong.GetCustomer_haveTransaction(param);
                    return ActionTrueData(lst);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IEnumerable<Object> getKhachHangGDByNgay(DateTime ngayhd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classBHHoaDon = new ClassBH_HoaDon(db);
                var monthFrom = ngayhd.Month;
                var dateFrom = ngayhd.Day;
                var dateYear = ngayhd.Year;
                var tbl = from hd in db.BH_HoaDon
                          join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                          where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 19) && hd.ChoThanhToan == false && hd.NgayLapHoaDon.Month == monthFrom && hd.NgayLapHoaDon.Day == dateFrom && hd.NgayLapHoaDon.Year == dateYear && !string.IsNullOrEmpty(dt.DienThoai) && hd.ID_DoiTuong != null
                          select new
                          {
                              ID = hd.ID,
                              MaHoaDon = hd.MaHoaDon,
                              ID_DoiTuong = hd.ID_DoiTuong,
                              SoDienThoai = dt.DienThoai
                          };
                return tbl.ToList();
            }
        }

        //trinhpv getlist NhomDoiTuongChiTiet
        public List<DM_NhomDoiTuongChiTietDTO> getList_NhomDoiTuongChiTietbyID(Guid ID_NhomDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                List<DM_NhomDoiTuongChiTietDTO> lst = db.Database.SqlQuery<DM_NhomDoiTuongChiTietDTO>("exec getList_NhomDoiTuongChiTiet @ID_NhomDoiTuong", prm.ToArray()).ToList();
                return lst;
            }
        }
        [HttpGet, HttpPost]
        public IHttpActionResult GetNhomDoiTuong_DonVi(int loaiDT)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<DM_NhomDoiTuongDTO> lst = new List<DM_NhomDoiTuongDTO>();
                    var data = (from nhom in db.DM_NhomDoiTuong
                                join nh_dv in db.NhomDoiTuong_DonVi on nhom.ID equals nh_dv.ID_NhomDoiTuong into NhomDV
                                from nhomdv in NhomDV.DefaultIfEmpty()
                                where nhom.TrangThai != false
                                && nhom.LoaiDoiTuong == loaiDT
                                group new { nhom, nhomdv }
                                by new { nhom.ID, nhom.TenNhomDoiTuong, nhom.TuDongCapNhat, nhom.GiamGia, nhom.GiamGiaTheoPhanTram, nhom.GhiChu }).ToList();

                    foreach (var item in data)
                    {
                        DM_NhomDoiTuongDTO nhom = new DM_NhomDoiTuongDTO
                        {
                            ID = item.Key.ID,
                            TenNhomDoiTuong = item.Key.TenNhomDoiTuong,
                            GiamGia = item.Key.GiamGia,
                            GiamGiaTheoPhanTram = item.Key.GiamGiaTheoPhanTram,
                            TuDongCapNhat = item.Key.TuDongCapNhat,
                            GhiChu = item.Key.GhiChu,
                        };

                        List<NhomDoiTuong_DonViDTO> lstDV = new List<NhomDoiTuong_DonViDTO>();
                        foreach (var itemGr in item)
                        {
                            if (itemGr.nhomdv != null)
                            {
                                NhomDoiTuong_DonViDTO nhomdv = new NhomDoiTuong_DonViDTO
                                {
                                    ID = itemGr.nhomdv.ID_DonVi,
                                    ID_NhomDoiTuong = itemGr.nhomdv.ID_NhomDoiTuong,
                                    TenDonVi = itemGr.nhomdv.DM_DonVi.TenDonVi,
                                };
                                lstDV.Add(nhomdv);
                            }
                        }
                        nhom.NhomDT_DonVi = lstDV;
                        lst.Add(nhom);
                    }
                    return Json(new { res = true, data = lst });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = string.Concat("GetNhomDoiTuong_DonVi", e.InnerException, e.Message) });
            }
        }

        [HttpGet, HttpPost]
        [DeflateCompression]
        public IHttpActionResult Call_ManyFunction_OnsuccessBanHang(Guid idChiNhanh)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var nhomban = db.DM_KhuVuc.Select(x => new { x.ID, x.TenKhuVuc }).ToList();
                    var phongban = db.DM_ViTri.Where(x => x.TinhTrang != true).Select(x =>
                    new { ID = x.ID, ID_KhuVuc = x.ID_KhuVuc, MaViTri = x.MaViTri, TenViTri = x.TenViTri, NameFull = string.Concat(x.MaViTri, " ", x.TenViTri), TenKhuVuc = x.DM_KhuVuc.TenKhuVuc }).ToList();

                    var temprint = (from mi in db.DM_MauIn
                                    join ct in db.DM_LoaiChungTu
                                    on mi.ID_LoaiChungTu equals ct.ID
                                    where mi.ID_DonVi == idChiNhanh
                                    select new
                                    {
                                        ID = mi.ID,
                                        DuLieuMauIn = mi.DuLieuMauIn,
                                        MaLoaiChungTu = ct.MaLoaiChungTu,
                                        LaMacDinh = mi.LaMacDinh,
                                        TenMauIn = mi.TenMauIn,
                                    }).ToList();
                    var district = (from x in db.DM_QuanHuyen
                                    select new
                                    {
                                        x.ID,
                                        x.MaQuanHuyen,
                                        x.TenQuanHuyen,
                                        x.ID_TinhThanh
                                    }).ToList();
                    return Json(new
                    {
                        res = true,
                        District = district,
                        TemPrint = temprint,
                        NhomBan = nhomban,
                        PhongBan = phongban,
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = e.InnerException + e.Message });
            }
        }

        //trinhpv getlist tỉnh thành
        public List<DM_VungMienDTO> getList_VungMien()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                return classdoituong.getList_VungMien();
            }
        }

        public List<DM_VungMienDTO> getList_TinhThanhbyID(Guid ID)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                List<DM_VungMienDTO> lst = new List<DM_VungMienDTO>();
                lst = classdoituong.getList_TinhThanhbyID(ID);
                return lst;
            }
        }
        public List<DM_VungMienDTO> getList_VungMienbyID(Guid ID)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                List<DM_VungMienDTO> lst = new List<DM_VungMienDTO>();
                lst = classdoituong.getList_VungMienbyID(ID);
                return lst;
            }
        }
        // GET: api/DanhMuc/DM_DoiTuongAPI/GetDM_DoiTuong
        public List<DM_DoiTuong> GetDM_DoiTuong()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                return classdoituong.Gets(null);
            }
        }

        [HttpGet, HttpPost]
        public string ExportExcel_KhachHang(Params_GetListKhachHang lstParam)
        {
            string fileSave = string.Empty;
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);

                    var whereColumn = SearchColumn(lstParam.SearchColumns, string.Empty, ref lstParam);
                    lstParam.WhereSql = whereColumn;
                    List<SP_DM_DoiTuong> data = classdoituong.LoadDanhMuc_KhachHangNhaCungCap(lstParam);
                    DataTable excel = classOffice.ToDataTable<SP_DM_DoiTuong>(data);

                    excel.Columns.Remove("ID");
                    excel.Columns.Remove("LoaiDoiTuong");
                    excel.Columns.Remove("TheoDoi");
                    excel.Columns.Remove("GioiTinhNam");
                    excel.Columns.Remove("ID_NhomDoiTuong");
                    excel.Columns.Remove("TenDoiTuong_KhongDau");
                    excel.Columns.Remove("TenDoiTuong_ChuCaiDau");
                    excel.Columns.Remove("ID_NguonKhach");
                    excel.Columns.Remove("ID_NhanVienPhuTrach");
                    excel.Columns.Remove("ID_NguoiGioiThieu");
                    excel.Columns.Remove("LaCaNhan");
                    excel.Columns.Remove("ID_TinhThanh");
                    excel.Columns.Remove("ID_QuanHuyen");
                    excel.Columns.Remove("DienThoaiChiNhanh");
                    excel.Columns.Remove("TongMua");
                    excel.Columns.Remove("SoLanMuaHang");
                    excel.Columns.Remove("Name_Phone");
                    excel.Columns.Remove("DinhDang_NgaySinh");
                    excel.Columns.Remove("ID_TrangThai");
                    excel.Columns.Remove("TrangThai_TheGiaTri");
                    excel.Columns.Remove("NoTruoc");
                    excel.Columns.Remove("PhiDichVu");
                    excel.Columns.Remove("TongPhiDichVu");
                    excel.Columns.Remove("MaNVPhuTrach");
                    excel.Columns.Remove("TotalRow");
                    excel.Columns.Remove("TotalPage");
                    excel.Columns.Remove("TongBanAll");
                    excel.Columns.Remove("TongBanTruTraHangAll");
                    excel.Columns.Remove("TongTichDiemAll");
                    excel.Columns.Remove("NoHienTaiAll");
                    excel.Columns.Remove("NapCoc");
                    excel.Columns.Remove("SuDungCoc");
                    excel.Columns.Remove("SoDuCoc");
                    excel.Columns.Remove("NapCocAll");
                    excel.Columns.Remove("SuDungCocAll");
                    excel.Columns.Remove("SoDuCocAll");
                    excel.Columns.Remove("SumTongThuKhachHang");
                    excel.Columns.Remove("SumTongChiKhachHang");
                    excel.Columns.Remove("SumGiaTriDVSuDung");
                    excel.Columns.Remove("SumGiaTriDVHoanTra");
                    excel.Columns.Remove("Email");
                    excel.Columns.Remove("MaSoThue");
                    excel.Columns.Remove("TaiKhoanNganHang");
                    excel.Columns.Remove("TongBan");
                    excel.Columns.Remove("TongTichDiem");
                    excel.Columns.Remove("NgayGiaoDichGanNhat");
                    excel.Columns.Remove("TrangThaiKhachHang");
                    excel.Columns.Remove("SumSoTienChuaSD");

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhSachKhachHang.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachKhachHang.xlsx");
                    fileSave = classOffice.createFolder_Download(fileSave);
                    classOffice.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, lstParam.ColumnsHide);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ExportExcel_KhachHang_Params_GetListKhachHang " + ex.InnerException + ex.Message);
            }
            return fileSave;
        }

        [HttpGet, HttpPost]
        public string ExportExcel_NhaCungCap(Params_GetListKhachHang lstParam)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                Class_officeDocument classOffice = new Class_officeDocument(db);

                var whereColumn = SearchColumn(lstParam.SearchColumns, string.Empty, ref lstParam);
                lstParam.WhereSql = whereColumn;
                List<SP_DM_DoiTuong> lstKhachhangs = classdoituong.SP_GetListKhachHang_Where_Paging(lstParam);

                List<DM_NhaCungCap_Excel> lst = new List<DM_NhaCungCap_Excel>();
                foreach (var item in lstKhachhangs)
                {
                    DM_NhaCungCap_Excel DM = new DM_NhaCungCap_Excel();
                    DM.MaDoiTuong = item.MaDoiTuong;
                    DM.TenDoiTuong = item.TenDoiTuong;
                    DM.DienThoai = item.DienThoai;
                    DM.TenNhomDT = item.TenNhomDT;
                    DM.Email = item.Email;
                    DM.DiaChi = item.DiaChi;
                    DM.KhuVuc = item.KhuVuc;
                    DM.PhuongXa = item.PhuongXa;
                    DM.NguoiTao = item.NguoiTao;
                    DM.TongMua = item.TongMua ?? 0;
                    DM.GhiChu = item.GhiChu;
                    DM.NoCanTraHienTai = item.NoHienTai * -1 ?? 0;
                    DM.NapCoc = item.NapCoc ?? 0;
                    DM.SuDungCoc = item.SuDungCoc ?? 0;
                    DM.SoDuCoc = item.SoDuCoc ?? 0;
                    lst.Add(DM);
                }
                DataTable excel = classOffice.ToDataTable<DM_NhaCungCap_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhSachNhaCungCap.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachNhaCungCap.xlsx");
                fileSave = classOffice.createFolder_Download(fileSave);
                classOffice.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, lstParam.ColumnsHide);

                var index = fileSave.IndexOf(@"\Template");
                fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                fileSave = fileSave.Replace(@"\", "/");
                return fileSave;
            }
        }

        public string Insurance_ExportTabCongNo(Guid idDoiTuong, string maDoiTuog, string tenDoiTuong, string idChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {

                    var classhoadon = new ClassBH_HoaDon(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    var dataQuyHD = classhoadon.SP_GetHoaDonandSoQuy_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Temp_LichSu_CongNoBaoHiem.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/LichSu_CongNoBaoHiem.xlsx");

                    if (dataQuyHD != null && dataQuyHD.Count() > 0)
                    {
                        dataQuyHD = dataQuyHD.OrderByDescending(hd => hd.NgayLapHoaDon).ToList();

                        var ss3 = dataQuyHD.Select(x => new NhatKyTichDiem
                        {
                            MaHoaDon = x.MaHoaDon,
                            NgayLapHoaDon = x.NgayLapHoaDon,
                            SLoaiHoaDon = x.strLoaiHoaDon,
                            GiaTri = x.PhaiThanhToan ?? 0,
                            DiemGiaoDich = x.DuNoKH ?? 0,
                        });

                        DataTable excel_QuyHD = classOffice.ToDataTable<NhatKyTichDiem>(ss3.ToList());
                        excel_QuyHD.Columns.Remove("DiemSauGD");
                        classOffice.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel_QuyHD, 6, 31, 25, true, 0, null, maDoiTuog, tenDoiTuong);
                    }
                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return fileSave;
                }
                catch (Exception ex)
                {
                    return ex.InnerException + ex.Message;
                }
            }
        }

        // nhuongdt: export excel all tab at list KH (his ban/trahang, công nợ KH, his đặt hàng, his tích điểm)
        [HttpGet, HttpPost]
        public string ExportExcel_AllHis_ofKH(Guid idDoiTuong, string maDoiTuog, string tenDoiTuong, string idChiNhanh)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classhoadon = new ClassBH_HoaDon(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);

                    // HD ban/TraHang/DatHang
                    var dataHD = classhoadon.GetHoaDon_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                    IEnumerable<KhachHang_TabHoaDon> dataSell = null;
                    IEnumerable<KhachHang_TabHoaDon> dataReserved = null;

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Temp_LichSuMua_ThanhToan_TichLuy_ofKhachHang.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/LichSuMua_ThanhToan_TichLuy_ofKhachHang.xlsx");

                    if (dataHD != null && dataHD.Count() > 0)
                    {
                        dataSell = dataHD.Where(x => x.LoaiHoaDon != 3).OrderByDescending(HD => HD.NgayLapHoaDon);
                        dataReserved = dataHD.Where(x => x.LoaiHoaDon == 3).OrderByDescending(HD => HD.NgayLapHoaDon);

                        var ss1 = dataSell.Select(x => new Excel_HisHoaDon
                        {
                            MaHoaDon = x.MaHoaDon,
                            NgayLapHoaDon = x.NgayLapHoaDon,
                            NguoiTao = x.NguoiTao,
                            GiaTri = x.PhaiThanhToan,
                            TrangThai = x.LoaiHoaDon == 1 ? "Bán hàng" :
                            x.LoaiHoaDon == 6 ? "Trả hàng" :
                            x.LoaiHoaDon == 19 ? "Gói dịch vụ" :
                            x.LoaiHoaDon == 22 ? "Thẻ giá trị" :
                            x.LoaiHoaDon == 25 ? "Hóa đơn sữa chữa" :
                            "",
                        });

                        DataTable excel_BH = classOffice.ToDataTable<Excel_HisHoaDon>(ss1.ToList());
                        classOffice.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel_BH, 6, 25, 19, true, 0, null, maDoiTuog, tenDoiTuong);

                        if (dataReserved.Count() > 0)
                        {
                            var ss2 = dataReserved.Select(x => new Excel_HisHoaDon
                            {
                                MaHoaDon = x.MaHoaDon,
                                NgayLapHoaDon = x.NgayLapHoaDon,
                                NguoiTao = x.NguoiTao,
                                GiaTri = x.PhaiThanhToan,
                                TrangThai = "Đặt hàng",
                            });

                            DataTable excel_DT = classOffice.ToDataTable<Excel_HisHoaDon>(ss2.ToList());
                            classOffice.listToOfficeExcel_Sheet_KH(fileSave, fileSave, excel_DT, 6, 25, 19, true, 1, null, maDoiTuog, tenDoiTuong);
                        }
                    }

                    // Quy_HD
                    var dataQuyHD = classhoadon.SP_GetHoaDonandSoQuy_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                    if (dataQuyHD != null && dataQuyHD.Count() > 0)
                    {
                        dataQuyHD = dataQuyHD.OrderByDescending(hd => hd.NgayLapHoaDon).ToList();

                        var ss3 = dataQuyHD.Select(x => new NhatKyTichDiem
                        {
                            MaHoaDon = x.MaHoaDon,
                            NgayLapHoaDon = x.NgayLapHoaDon,
                            SLoaiHoaDon = x.strLoaiHoaDon,
                            GiaTri = x.PhaiThanhToan ?? 0,
                            DiemGiaoDich = x.DuNoKH ?? 0,
                        });

                        DataTable excel_QuyHD = classOffice.ToDataTable<NhatKyTichDiem>(ss3.ToList());
                        excel_QuyHD.Columns.Remove("DiemSauGD");
                        classOffice.listToOfficeExcel_Sheet_KH(fileSave, fileSave, excel_QuyHD, 6, 25, 19, true, 2, null, maDoiTuog, tenDoiTuong);
                    }

                    // TichDiem
                    var dataPoint = classhoadon.GetLichSu_TichDiem(idDoiTuong);
                    if (dataPoint != null && dataPoint.Count() > 0)
                    {
                        dataPoint = dataPoint.OrderByDescending(x => x.NgayLapHoaDon).ToList();
                        var ss4 = dataPoint.Select(x => new NhatKyTichDiem
                        {
                            MaHoaDon = x.MaHoaDon,
                            NgayLapHoaDon = x.NgayLapHoaDon,
                            SLoaiHoaDon = x.strLoaiHoaDon,
                            GiaTri = x.PhaiThanhToan,
                            DiemGiaoDich = x.DiemGiaoDich,
                            DiemSauGD = x.DiemSauGD,
                        });
                        DataTable excel_Point = classOffice.ToDataTable<NhatKyTichDiem>(ss4.ToList());
                        classOffice.listToOfficeExcel_Sheet_KH(fileSave, fileSave, excel_Point, 6, 25, 19, true, 3, null, maDoiTuog, tenDoiTuong);
                    }

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return fileSave;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_DoiTuongAPI_ExportExcel_AllHis_ofKH: " + ex.InnerException + ex.Message);
                return string.Empty;
            }
        }

        [HttpGet, HttpPost]
        public string ExportExcel_AllHis_ofNCC(Guid idDoiTuong, string maDoiTuog, string tenDoiTuong, string idChiNhanh)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classhoadon = new ClassBH_HoaDon(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);

                    // HD Nhap/Tra
                    var dataHD = classhoadon.GetHoaDon_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                    IEnumerable<KhachHang_TabHoaDon> dataSell = null;

                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/LichSuNhap_ThanhToan_ofNhaCungCap.xlsx");
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Temp_LichSuNhap_ThanhToan_ofNhaCungCap.xlsx");

                    if (dataHD != null && dataHD.Count() > 0)
                    {
                        dataSell = dataHD.Where(x => x.LoaiHoaDon != 3).OrderByDescending(HD => HD.NgayLapHoaDon);

                        var ss1 = dataSell.Select(x => new Excel_HisHoaDon
                        {
                            MaHoaDon = x.MaHoaDon,
                            NgayLapHoaDon = x.NgayLapHoaDon,
                            NguoiTao = x.NguoiTao,
                            GiaTri = x.PhaiThanhToan,
                            //TrangThai = x.LoaiHoaDon == 4 ? "Nhập hàng" : "Trả hàng nhập",
                        });

                        DataTable excel_BH = classOffice.ToDataTable<Excel_HisHoaDon>(ss1.ToList());
                        classOffice.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel_BH, 6, 25, 19, true, 0, null, maDoiTuog, tenDoiTuong);
                    }

                    // Quy_HD
                    var dataQuyHD = classhoadon.SP_GetHoaDonandSoQuy_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                    if (dataQuyHD != null && dataQuyHD.Count() > 0)
                    {
                        dataQuyHD = dataQuyHD.OrderByDescending(hd => hd.NgayLapHoaDon).ToList();

                        var ss3 = dataQuyHD.Select(x => new Excel_HisHoaDon
                        {
                            MaHoaDon = x.MaHoaDon,
                            NgayLapHoaDon = x.NgayLapHoaDon,
                            NguoiTao = x.strLoaiHoaDon,
                            GiaTri = x.PhaiThanhToan ?? 0,
                            TrangThai = x.DuNoKH.ToString(),
                        });

                        DataTable excel_QuyHD = classOffice.ToDataTable<Excel_HisHoaDon>(ss3.ToList());
                        classOffice.listToOfficeExcel_Sheet_KH(fileSave, fileSave, excel_QuyHD, 6, 25, 19, true, 1, null, maDoiTuog, tenDoiTuong);
                    }

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return fileSave;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_DoiTuongAPI_ExportExcel_AllHis_ofNCC: " + ex.InnerException + ex.Message);
                return string.Empty;
            }
        }

        [HttpGet, HttpPost]
        public List<SP_DM_DoiTuong> GetListKhachHang_Where_PassObject(Params_GetListKhachHang listParams)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    List<SP_DM_DoiTuong> lstAllKhacHangs = classdoituong.SP_GetListKhachHang_Where_PassObject(listParams);
                    if (lstAllKhacHangs != null)
                    {
                        return lstAllKhacHangs;
                    }
                    else
                    {
                        return new List<SP_DM_DoiTuong>();
                    }
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetListKhachHang_Where_PassObject " + ex.InnerException + ex.Message);
                return new List<SP_DM_DoiTuong>();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public JsonResult<JsonResulSMSKHDTO> GetListKhachHangSinhNhat(Params_GetListKHSMSDTOLT listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var tbl = from dt in db.DM_DoiTuong
                              join nhomdt in db.DM_DoiTuong_Nhom on dt.ID equals nhomdt.ID_DoiTuong into NDT
                              from ndt in NDT.DefaultIfEmpty()
                              join tenNhom in db.DM_NhomDoiTuong on ndt.ID_NhomDoiTuong equals tenNhom.ID into tenNDT
                              from tenN in tenNDT.DefaultIfEmpty()
                              where !string.IsNullOrEmpty(dt.DienThoai) && dt.NgaySinh_NgayTLap != null && dt.LoaiDoiTuong == 1
                              select new
                              {
                                  ID = dt.ID,
                                  ID_NhomDoiTuong = dt.IDNhomDoiTuongs == null ? Guid.Empty.ToString() : dt.IDNhomDoiTuongs,
                                  TenNhomDT = tenN.TenNhomDoiTuong,
                                  MaDoiTuong = dt.MaDoiTuong,
                                  TenDoiTuong = dt.TenDoiTuong,
                                  DienThoai = dt.DienThoai,
                                  NgaySinh_NgayTLap = dt.NgaySinh_NgayTLap,
                                  ID_NhanVienPhuTrach = dt.ID_NhanVienPhuTrach == null ? Guid.Empty : dt.ID_NhanVienPhuTrach,
                                  NguoiTao = dt.NguoiTao,
                                  NgayTao = dt.NgayTao
                              };

                    if (listParams.ID_NhomDoiTuong != null)
                    {
                        tbl = tbl.Where(p => p.ID_NhomDoiTuong.Contains(listParams.ID_NhomDoiTuong));
                    }

                    var ngaySinhFrom = listParams.NgaySinh_TuNgay;
                    var ngaySinhTo = listParams.NgaySinh_DenNgay;

                    if (listParams.NgaySinh_TuNgay != null && listParams.NgaySinh_DenNgay != null)
                    {
                        var monthFrom = ngaySinhFrom.Value.Month;
                        var monthTo = ngaySinhTo.Value.Month;
                        var dateFrom = ngaySinhFrom.Value.Day;
                        var dateTo = ngaySinhTo.Value.Day;

                        if (listParams.NgaySinh_TuNgay == listParams.NgaySinh_DenNgay)
                        {
                            tbl = tbl.Where(dt => dt.NgaySinh_NgayTLap != null
                            && dt.NgaySinh_NgayTLap.Value.Month == monthFrom && dt.NgaySinh_NgayTLap.Value.Day == dateFrom);
                        }
                        else
                        {
                            if (ngaySinhFrom != new DateTime(1918, 1, 1))
                            {
                                // get KH with NgaySinh != null
                                tbl = tbl.Where(dt => dt.NgaySinh_NgayTLap != null);

                                // compare nam
                                if (listParams.LoaiNgaySinh == 1)
                                {
                                    tbl = tbl.Where(dt => dt.NgaySinh_NgayTLap.Value.Year == ngaySinhFrom.Value.Year);
                                }
                                else
                                {
                                    // compare month/day
                                    if (monthFrom == monthTo)
                                    {
                                        // compare date (dateFrom <= ngaysinh <= dateTo)
                                        tbl = tbl.Where(dt => dt.NgaySinh_NgayTLap.Value.Month == monthFrom
                                     && dt.NgaySinh_NgayTLap.Value.Day >= dateFrom && dt.NgaySinh_NgayTLap.Value.Day <= dateTo);
                                    }
                                    else
                                    {
                                        // (monthFrom < thangsinh < monthTo) OR (same month and ngaysinh >= dateFrom) OR ( ngaysinh <= dateTo)
                                        tbl = tbl.Where(dt => (dt.NgaySinh_NgayTLap.Value.Month > monthFrom && dt.NgaySinh_NgayTLap.Value.Month < monthTo)
                                    || ((dt.NgaySinh_NgayTLap.Value.Month == monthFrom && dt.NgaySinh_NgayTLap.Value.Day >= dateFrom)
                                    || (dt.NgaySinh_NgayTLap.Value.Month == monthTo && dt.NgaySinh_NgayTLap.Value.Day <= dateFrom)));
                                    }
                                }
                            }
                        }
                    }

                    var lstIDManager = listParams.ID_NhanVienQuanLys;
                    if (lstIDManager != null)
                    {
                        if (lstIDManager.Count > 0)
                        {
                            var nguoiTao = listParams.NguoiTao;
                            tbl = tbl.Where(x => lstIDManager.Contains(x.ID_NhanVienPhuTrach.ToString())
                            || x.ID_NhanVienPhuTrach == null || x.NguoiTao.Contains(nguoiTao));
                        }
                    }

                    var data = tbl.AsEnumerable().OrderByDescending(p => p.NgayTao).GroupBy(p => p.ID).Select(p => new DoiTuongSMSDTO()
                    {
                        ID = p.FirstOrDefault().ID,
                        TenNhomDT = String.Join(",", p.Select(c => c.TenNhomDT)),
                        MaDoiTuong = p.FirstOrDefault().MaDoiTuong,
                        TenDoiTuong = p.FirstOrDefault().TenDoiTuong,
                        DienThoai = p.FirstOrDefault().DienThoai,
                        NgaySinh_NgayTLap = p.FirstOrDefault().NgaySinh_NgayTLap,

                    });

                    List<DoiTuongSMSDTO> lstReturn = new List<DoiTuongSMSDTO>();
                    foreach (var item in data)
                    {
                        var lst = db.HeThong_SMS.Where(p => p.ID_KhachHang == item.ID && p.LoaiTinNhan == 2 && p.ThoiGianGui.Year == DateTime.Now.Year);
                        DoiTuongSMSDTO doituong = new DoiTuongSMSDTO();
                        doituong.ID = item.ID;
                        doituong.TenNhomDT = item.TenNhomDT;
                        doituong.MaDoiTuong = item.MaDoiTuong;
                        doituong.TenDoiTuong = item.TenDoiTuong;
                        doituong.DienThoai = item.DienThoai;
                        doituong.NgaySinh_NgayTLap = item.NgaySinh_NgayTLap;
                        doituong.TrangThaiKHGuiTin = lst.Count() > 0 ? "Đã gửi tin" : "Chưa gửi tin";
                        doituong.CountTrangThaiGuiTin = lst.Count();
                        doituong.TrangThai = lst.Count() > 0 ? lst.FirstOrDefault().TrangThai : 999;
                        lstReturn.Add(doituong);
                    }

                    switch (listParams.TrangThai)
                    {
                        case 0:
                            break;
                        case 1:
                            lstReturn = lstReturn.Where(p => p.CountTrangThaiGuiTin > 0).ToList();
                            break;
                        case 2:
                            lstReturn = lstReturn.Where(p => p.CountTrangThaiGuiTin == 0).ToList();
                            break;
                        default:
                            break;
                    }
                    var totalRecords = lstReturn.Count();
                    lstReturn = lstReturn.Skip(listParams.CurrentPage ?? 0 * listParams.PageSize ?? 10).Take(listParams.PageSize ?? 10).ToList();

                    JsonResulSMSKHDTO json = new JsonResulSMSKHDTO
                    {
                        lst = lstReturn,
                        Rowcount = totalRecords,
                        pageCount = System.Math.Ceiling(totalRecords * 1.0 / listParams.PageSize ?? 10)
                    };
                    return Json(json);
                }
            }
        }

        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult SMS_KhachHangSinhNhat(Params_GetListKHSMSGiaoDich listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                    var idDonVis = string.Join(",", listParams.ID_DonViArr);
                    string where = string.Empty;
                    string where1 = string.Empty;
                    string idNhomKH = string.Empty;
                    if (listParams.ID_NhomDoiTuong == null)
                    {
                        idNhomKH = "%%";
                    }
                    else
                    {
                        idNhomKH = "%" + listParams.ID_NhomDoiTuong + "%";
                    }

                    var ngaySinhFrom = listParams.NgaySinh_TuNgay;
                    var ngaySinhTo = listParams.NgaySinh_DenNgay;

                    if (listParams.NgaySinh_TuNgay != null && listParams.NgaySinh_DenNgay != null)
                    {
                        var monthFrom = ngaySinhFrom.Value.Month;
                        var monthTo = ngaySinhTo.Value.Month;
                        var dateFrom = ngaySinhFrom.Value.Day;
                        var dateTo = ngaySinhTo.Value.Day;

                        if (listParams.NgaySinh_TuNgay == listParams.NgaySinh_DenNgay)
                        {
                            //lst = lst.Where(dt => dt.NgaySinh_NgayTLap != null
                            where1 = string.Concat(" NgaySinh_NgayTLap is not null AND DATEPART(month,NgaySinh_NgayTLap)= ", monthFrom, " AND DATEPART(day,NgaySinh_NgayTLap)= ", dateFrom);
                            where = classDoiTuong.GetStringWhere(where, where1);
                        }
                        else
                        {
                            if (ngaySinhFrom != new DateTime(1918, 1, 1))
                            {
                                // get KH with NgaySinh != null
                                where1 = " NgaySinh_NgayTLap is not null ";
                                where = classDoiTuong.GetStringWhere(where, where1);

                                // compare nam
                                if (listParams.LoaiNgaySinh == 1)
                                {
                                    where1 = " DATEPART(year,NgaySinh_NgayTLap) = " + ngaySinhFrom.Value.Year;
                                    where = classDoiTuong.GetStringWhere(where, where1);
                                }
                                else
                                {
                                    // compare month/day
                                    if (monthFrom == monthTo)
                                    {
                                        // compare date (dateFrom <= ngaysinh <= dateTo)
                                        where1 = string.Concat(" DATEPART(month,NgaySinh_NgayTLap) = ", monthFrom, " AND DATEPART(day,NgaySinh_NgayTLap) >= ", dateFrom,
                                           " AND DATEPART(day, NgaySinh_NgayTLap) <= ", dateTo, "");
                                        where = classDoiTuong.GetStringWhere(where, where1);
                                    }
                                    else
                                    {
                                        // tu 11/09 - 01/12 --> get all KH sinh nhat thang 10,11 OR (KH co ngay sinh >=11 va thang sinh = 9) OR (KH co ngay sinh <=01 va thang 12)
                                        //monthbetween = monthbetween.TrimEnd(',');
                                        where1 = string.Concat(" ((DATEPART(month,NgaySinh_NgayTLap) > ", monthFrom, " AND DATEPART(month,NgaySinh_NgayTLap) < ", monthTo,
                                          " ) OR ( DATEPART(month, NgaySinh_NgayTLap) = ", monthFrom, " AND DATEPART(day, NgaySinh_NgayTLap) >= ", dateFrom,
                                          " ) OR ( DATEPART(month, NgaySinh_NgayTLap) = ", monthTo, " AND DATEPART(day, NgaySinh_NgayTLap) <= ", dateTo, " ))");
                                        where = classDoiTuong.GetStringWhere(where, where1);
                                    }
                                }
                            }
                        }
                    }
                    var lstReturnIDManager = listParams.ID_NhanVienQuanLys;
                    if (lstReturnIDManager != null)
                    {
                        if (lstReturnIDManager.Count > 0)
                        {
                            var nguoiTao = listParams.NguoiTao;
                            string idManagers = string.Join(",", lstReturnIDManager);
                            where1 = string.Concat(" (exists (select Name from splitstring('", idManagers, "') tblMng where hd.ID_NhanVien = tblMng.Name ) OR hd.ID_NhanVien is null OR hd.NguoiTao like '%", nguoiTao, "%' )");
                            where = classDoiTuong.GetStringWhere(where, where1);
                        }
                    }

                    switch (listParams.TrangThai)
                    {
                        case 1:// dagui
                            where1 = " ISNULL(TrangThai,999) != 999";
                            where = classDoiTuong.GetStringWhere(where, where1);
                            break;
                        case 2:// chuagui
                            where1 = " ISNULL(TrangThai,999) = 999";
                            where = classDoiTuong.GetStringWhere(where, where1);
                            break;
                    }

                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", idDonVis));
                    paramlist.Add(new SqlParameter("ID_NhomKhachHang", idNhomKH));
                    paramlist.Add(new SqlParameter("Where", where));
                    paramlist.Add(new SqlParameter("CurrentPage", listParams.CurrentPage));
                    paramlist.Add(new SqlParameter("PageSize", listParams.PageSize));
                    List<DoiTuongSMSDTO> lst = db.Database.SqlQuery<DoiTuongSMSDTO>("EXEC SMS_KhachHangSinhNhat @ID_ChiNhanh," +
                        "@ID_NhomKhachHang, @Where, @CurrentPage, @PageSize ", paramlist.ToArray()).ToList();
                    return Json(new { res = true, data = lst });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult GetListCustomer_byIDs(Params_GetListKHSMSDTOLT lstParam)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var idCustomers = string.Join(",", lstParam.ID_NhanVienQuanLys);
                    SqlParameter param = new SqlParameter("IDCustomers", idCustomers);
                    var lst = db.Database.SqlQuery<SMS_DienThoai>("GetListCustomer_byIDs @IDCustomers", param).ToList();
                    return Json(new { res = true, data = lst });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = e.InnerException + e.Message });
            }
        }

        [AcceptVerbs("POST", "GET")]
        public JsonResult<JsonResulSMSGiaoDich> GetListKhachHangGiaoDich(Params_GetListKHSMSGiaoDich listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                //List<SqlParameter> paramlist = new List<SqlParameter>();
                //paramlist.Add(new SqlParameter("ID_NhomDoiTuong", listParams.ID_NhomDoiTuong));
                //List<BH_KiemKhoChiTiet_Excel> lst = db.Database.SqlQuery<BH_KiemKhoChiTiet_Excel>("EXEC GetListChiTietHoaDonKiemKhoXuatFile @ID_HoaDon", paramlist.ToArray()).ToList();
                var tbl = from hd in db.BH_HoaDon
                          join dt in db.DM_DoiTuong on hd.ID_DoiTuong equals dt.ID
                          join nhomdt in db.DM_DoiTuong_Nhom on dt.ID equals nhomdt.ID_DoiTuong into NDT
                          from ndt in NDT.DefaultIfEmpty()
                          join tenNhom in db.DM_NhomDoiTuong on ndt.ID_NhomDoiTuong equals tenNhom.ID into tenNDT
                          from tenN in tenNDT.DefaultIfEmpty()
                          where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 19) && hd.ID_DoiTuong != new Guid("00000000-0000-0000-0000-000000000000") && !string.IsNullOrEmpty(dt.DienThoai)
                          select new
                          {
                              MaHoaDon = hd.MaHoaDon,
                              ID_HoaDon = hd.ID,
                              ID = dt.ID,
                              ID_NhomDoiTuong = dt.IDNhomDoiTuongs == null ? Guid.Empty.ToString() : dt.IDNhomDoiTuongs,
                              TenNhomDT = tenN.TenNhomDoiTuong,
                              MaDoiTuong = dt.MaDoiTuong,
                              TenDoiTuong = dt.TenDoiTuong,
                              DienThoai = dt.DienThoai,
                              NgayLapHoaDon = hd.NgayLapHoaDon,
                              LoaiHoaDon = hd.LoaiHoaDon,
                              ID_DonVi = hd.ID_DonVi,
                              ID_NhanVienPhuTrach = dt.ID_NhanVienPhuTrach == null ? Guid.Empty : dt.ID_NhanVienPhuTrach,
                              NguoiTao = dt.NguoiTao,
                              NgayTao = dt.NgayTao,
                          };

                if (listParams.ID_NhomDoiTuong != null)
                {
                    tbl = tbl.Where(p => p.ID_NhomDoiTuong.Contains(listParams.ID_NhomDoiTuong));
                }

                var ngaySinhFrom = listParams.NgaySinh_TuNgay;
                var ngaySinhTo = listParams.NgaySinh_DenNgay;

                if (listParams.NgaySinh_TuNgay != null && listParams.NgaySinh_DenNgay != null)
                {
                    var monthFrom = ngaySinhFrom.Value.Month;
                    var monthTo = ngaySinhTo.Value.Month;
                    var dateFrom = ngaySinhFrom.Value.Day;
                    var dateTo = ngaySinhTo.Value.Day;

                    if (listParams.NgaySinh_TuNgay == listParams.NgaySinh_DenNgay)
                    {
                        tbl = tbl.Where(dt => dt.NgayLapHoaDon != null
                        && dt.NgayLapHoaDon.Month == monthFrom && dt.NgayLapHoaDon.Day == dateFrom);
                    }
                    else
                    {
                        if (ngaySinhFrom != new DateTime(1918, 1, 1))
                        {
                            // get KH with NgaySinh != null
                            tbl = tbl.Where(dt => dt.NgayLapHoaDon != null);

                            // compare nam
                            if (listParams.LoaiNgaySinh == 1)
                            {
                                tbl = tbl.Where(dt => dt.NgayLapHoaDon.Year == ngaySinhFrom.Value.Year);
                            }
                            else
                            {
                                // compare month/day
                                if (monthFrom == monthTo)
                                {
                                    // compare date (dateFrom <= ngaysinh <= dateTo)
                                    tbl = tbl.Where(dt => dt.NgayLapHoaDon.Month == monthFrom
                                 && dt.NgayLapHoaDon.Day >= dateFrom && dt.NgayLapHoaDon.Day <= dateTo);
                                }
                                else
                                {
                                    // (monthFrom < thangsinh < monthTo) OR (same month and ngaysinh >= dateFrom) OR ( ngaysinh <= dateTo)
                                    tbl = tbl.Where(dt => (dt.NgayLapHoaDon.Month > monthFrom && dt.NgayLapHoaDon.Month < monthTo)
                                || ((dt.NgayLapHoaDon.Month == monthFrom && dt.NgayLapHoaDon.Day >= dateFrom)
                                || (dt.NgayLapHoaDon.Month == monthTo && dt.NgayLapHoaDon.Day <= dateFrom)));
                                }
                            }
                        }
                    }
                }

                var lstIDManager = listParams.ID_NhanVienQuanLys;
                if (lstIDManager != null)
                {
                    if (lstIDManager.Count > 0)
                    {
                        var nguoiTao = listParams.NguoiTao;
                        tbl = tbl.Where(x => lstIDManager.Contains(x.ID_NhanVienPhuTrach.ToString())
                        || x.ID_NhanVienPhuTrach == null || x.NguoiTao.Contains(nguoiTao));
                    }
                }

                List<Guid> lstIDCN = new List<Guid>();
                if (listParams.ID_DonViArr != null)
                {
                    if (listParams.ID_DonViArr.Count() > 0)
                    {
                        for (int i = 0; i < listParams.ID_DonViArr.Count(); i++)
                        {
                            lstIDCN.Add(new Guid(listParams.ID_DonViArr[i]));
                        }
                    }
                }

                if (lstIDCN.Count > 0)
                {
                    tbl = tbl.Where(p => p.ID_DonVi != null).Where(hd => lstIDCN.Contains(hd.ID_DonVi));
                }
                else
                {
                    if (listParams.iddonvi != string.Empty && listParams.iddonvi != null)
                    {
                        tbl = tbl.Where(hd => hd.ID_DonVi.ToString().Contains(listParams.iddonvi));
                    }
                }

                switch (listParams.LoaiGiaoDich)
                {
                    case 0:
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 1);
                        break;
                    case 1:
                        tbl = tbl.Where(hd => hd.LoaiHoaDon == 19);
                        break;
                    case 2:
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }

                var data = tbl.AsEnumerable().OrderByDescending(p => p.NgayLapHoaDon).GroupBy(p => p.ID_HoaDon).Select(p => new DoiTuongSMSGiaoDich()
                {
                    ID = p.FirstOrDefault().ID,
                    ID_HoaDon = p.FirstOrDefault().ID_HoaDon,
                    TenNhomDT = String.Join(",", p.Select(c => c.TenNhomDT)),
                    MaDoiTuong = p.FirstOrDefault().MaDoiTuong,
                    TenDoiTuong = p.FirstOrDefault().TenDoiTuong,
                    DienThoai = p.FirstOrDefault().DienThoai,
                    MaHoaDon = p.FirstOrDefault().MaHoaDon,
                    LoaiGiaoDich = p.FirstOrDefault().LoaiHoaDon == 1 ? "Bán lẻ" : "Gói dịch vụ",
                    NgayLapHoaDon = p.FirstOrDefault().NgayLapHoaDon,
                });

                List<DoiTuongSMSGiaoDich> lstReturn = new List<DoiTuongSMSGiaoDich>();
                foreach (var item in data)
                {
                    List<HeThong_SMS> lst = db.HeThong_SMS.Where(p => p.ID_KhachHang == item.ID && p.LoaiTinNhan == 1 && p.ID_HoaDon == item.ID_HoaDon).ToList();
                    Guid idnguoigui = lst.Count() > 0 ? lst.FirstOrDefault().ID_NguoiGui : Guid.Empty;
                    DoiTuongSMSGiaoDich doituong = new DoiTuongSMSGiaoDich();
                    doituong.ID_HoaDon = item.ID_HoaDon;
                    doituong.TenNhomDT = item.TenNhomDT;
                    doituong.MaDoiTuong = item.MaDoiTuong;
                    doituong.MaHoaDon = item.MaHoaDon;
                    doituong.TenDoiTuong = item.TenDoiTuong;
                    doituong.DienThoai = item.DienThoai;
                    doituong.NgayLapHoaDon = item.NgayLapHoaDon;
                    doituong.TrangThaiGuiTin = lst.Count() > 0 ? "Đã gửi tin" : "Chưa gửi tin";
                    doituong.CountTrangThaiGuiTin = lst.Count();
                    doituong.LoaiGiaoDich = item.LoaiGiaoDich;
                    doituong.NguoiGui = lst.Count() > 0 ? db.HT_NguoiDung.Where(p => p.ID == idnguoigui).FirstOrDefault().TaiKhoan : "";
                    doituong.TrangThai = lst.Count() > 0 ? lst.FirstOrDefault().TrangThai : 999;
                    lstReturn.Add(doituong);
                }

                switch (listParams.TrangThai)
                {
                    case 0:
                        break;
                    case 1:
                        lstReturn = lstReturn.Where(p => p.CountTrangThaiGuiTin > 0).ToList();
                        break;
                    case 2:
                        lstReturn = lstReturn.Where(p => p.CountTrangThaiGuiTin == 0).ToList();
                        break;
                    default:
                        break;
                }

                var totalRecords = lstReturn.Count();
                lstReturn = lstReturn.Skip(listParams.CurrentPage * listParams.PageSize).Take(listParams.PageSize).ToList();

                JsonResulSMSGiaoDich json = new JsonResulSMSGiaoDich
                {
                    lst = lstReturn,
                    Rowcount = totalRecords,
                    pageCount = System.Math.Ceiling(totalRecords * 1.0 / listParams.PageSize)
                };
                return Json(json);
            }
        }

        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult SMS_GetListKhachHangGiaoDich(Params_GetListKHSMSGiaoDich listParams)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                    var idDonVis = string.Join(",", listParams.ID_DonViArr);
                    string where = string.Empty;
                    string where1 = string.Empty;
                    string idNhomKH = string.Empty;
                    if (listParams.ID_NhomDoiTuong == null)
                    {
                        idNhomKH = "%%";
                    }
                    else
                    {
                        idNhomKH = "%" + listParams.ID_NhomDoiTuong + "%";
                    }

                    switch (listParams.LoaiGiaoDich)
                    {
                        case 0:
                            where1 = " LoaiHoaDon = 1 ";
                            where = classDoiTuong.GetStringWhere(where, where1);
                            break;
                        case 1:
                            where1 = " LoaiHoaDon = 19 ";
                            where = classDoiTuong.GetStringWhere(where, where1);
                            break;
                        default:
                            where1 = " exists (select * from dbo.splitstring ('1,19') where LoaiHoaDon = Name) ";
                            where = classDoiTuong.GetStringWhere(where, where1);
                            break;
                    }
                    var lstReturnIDManager = listParams.ID_NhanVienQuanLys;
                    if (lstReturnIDManager != null)
                    {
                        if (lstReturnIDManager.Count > 0)
                        {
                            var nguoiTao = listParams.NguoiTao;
                            string idManagers = string.Join(",", lstReturnIDManager);
                            where1 = string.Concat(" (exists (select Name from splitstring('", idManagers, "') tblMng where hd.ID_NhanVien = tblMng.Name ) OR hd.ID_NhanVien is null OR hd.NguoiTao like '%", nguoiTao, "%' )");
                            where = classDoiTuong.GetStringWhere(where, where1);
                        }
                    }

                    switch (listParams.TrangThai)
                    {
                        case 1:// dagui
                            where1 = " ISNULL(TrangThai,999) != 999";
                            where = classDoiTuong.GetStringWhere(where, where1);
                            break;
                        case 2:// chuagui
                            where1 = " ISNULL(TrangThai,999) = 999";
                            where = classDoiTuong.GetStringWhere(where, where1);
                            break;
                    }

                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", idDonVis));
                    paramlist.Add(new SqlParameter("ID_NhomKhachHang", idNhomKH));
                    paramlist.Add(new SqlParameter("FromDate", listParams.NgaySinh_TuNgay));
                    paramlist.Add(new SqlParameter("ToDate", listParams.NgaySinh_DenNgay));
                    paramlist.Add(new SqlParameter("Where", where));
                    paramlist.Add(new SqlParameter("CurrentPage", listParams.CurrentPage));
                    paramlist.Add(new SqlParameter("PageSize", listParams.PageSize));
                    List<DoiTuongSMSGiaoDich> lst = db.Database.SqlQuery<DoiTuongSMSGiaoDich>("EXEC SMS_KhachHangGiaoDich @ID_ChiNhanh," +
                        "@ID_NhomKhachHang, @FromDate, @ToDate, @Where, @CurrentPage, @PageSize ", paramlist.ToArray()).ToList();
                    lst = lst.Select(x => new DoiTuongSMSGiaoDich
                    {
                        ID = x.ID,
                        ID_HoaDon = x.ID_HoaDon,
                        TenNhomDT = x.TenNhomDT,
                        MaDoiTuong = x.MaDoiTuong,
                        TenDoiTuong = x.TenDoiTuong,
                        DienThoai = x.DienThoai,
                        MaHoaDon = x.MaHoaDon,
                        NgayLapHoaDon = x.NgayLapHoaDon,
                        TotalPage = x.TotalPage,
                        TotalRow = x.TotalRow,
                        TrangThai = x.TrangThai,
                        LoaiGiaoDich = x.LoaiHoaDon == 1 ? "Bán lẻ" : "Gói dịch vụ",
                    }).ToList();
                    return Json(new { res = true, data = lst });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult SMS_LichHen(int status, ParamCalendar param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    class_LichHen classLichHen = new class_LichHen(db);
                    List<SP_Calendar> cskh = classLichHen.GetListCalendar(param);
                    var data = (from calendar in cskh
                                    //join sms in
                                    //     (from sms in db.HeThong_SMS
                                    //      join nd in db.HT_NguoiDung on sms.ID_NguoiGui equals nd.ID
                                    //      select new { sms.ID_KhachHang, sms.ThoiGianGui, sms.NoiDung, sms.TrangThai, nd.TaiKhoan }
                                    //    ) on calendar.ID_KhachHang equals sms.ID_KhachHang
                                    // into SMSLich
                                    //from smsl in SMSLich.DefaultIfEmpty()
                                where calendar.DienThoai != null && calendar.DienThoai != string.Empty
                                && calendar.IDNhomDoiTuongs.ToLower().Contains(param.IDNhomKH)
                                select new
                                {
                                    ID = calendar.ID,
                                    ID_KhachHang = calendar.ID_KhachHang,
                                    MaDoiTuong = calendar.MaDoiTuong,
                                    TenDoiTuong = calendar.TenDoiTuong,
                                    DienThoai = calendar.DienThoai,
                                    NoiDungLichHen = calendar.Ma_TieuDe,
                                    NgayHen = calendar.NgayGio,
                                    TenLoaiTuVanLichHen = calendar.TenLoaiTuVanLichHen,
                                    IDNhomDoiTuongs = calendar.IDNhomDoiTuongs,
                                    TenNhomDoiTuongs = calendar.TenNhomDoiTuongs,
                                    //ThoiGianGui = smsl == null ? (DateTime?)null : smsl.ThoiGianGui,
                                    //NguoiGui = smsl == null ? string.Empty : smsl.TaiKhoan,
                                    //TrangThai = smsl == null ? 999 : smsl.TrangThai,
                                    //NoiDungGui = smsl == null ? string.Empty : smsl.NoiDung,
                                }).ToList();

                    //switch (status)
                    //{
                    //    case 1:
                    //        data = data.Where(x => x.TrangThai != 999).ToList();
                    //        break;
                    //    case 2:
                    //        data = data.Where(x => x.TrangThai == 999).ToList();
                    //        break;
                    //}
                    var totalRow = data.Count;
                    var totalPage = Math.Ceiling(totalRow * 1.0 / param.PageSize);
                    // don't paging in this func, because had ch
                    data = data.OrderByDescending(x => x.NgayHen).ToList();
                    return Json(new { res = true, data = data, TotalRow = totalRow, TotalPage = totalPage });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        public IHttpActionResult GetListKeyColumKhachHang()
        {
            var ListComlumnSearchKhachHang = new List<ColumSearch>()
                {
                    new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.madoituong},
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tendoituong },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.dienthoai },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.nhomkhach, type=(int)commonEnumHellper.KeyCompare.bang },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.gioitinh ,type=(int)commonEnumHellper.KeyCompare.bang},
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.ngaysinh },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.email},
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.diachi },
                    new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tinhthanh },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.quanhuyen},
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.nguonkhach },
                    new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.nguoigioithieu },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.nguoitao},
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.nohientai, type=(int)commonEnumHellper.KeyCompare.bang },
                     new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tongban, type=(int)commonEnumHellper.KeyCompare.bang  },
                    new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tongbantrutrahang , type=(int)commonEnumHellper.KeyCompare.bang },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tongtichdiem, type=(int)commonEnumHellper.KeyCompare.bang },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.trangthaikhachhang, type=(int)commonEnumHellper.KeyCompare.bang },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.ghichu },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.trangthaiSoDuCoc },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.gtriNapCoc },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.gtriSuDungCoc },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.gtriSoDuCoc },
                    new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tongthuKhach },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tongChiKhach },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.gtriDVSuDung },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.gtriDVKhachTra },
                   new ColumSearch{Key=(int)commonEnum.ColumnKhachHang.tienChuaSD },
                };
            return Json(new { keycolumn = ListComlumnSearchKhachHang.ToList(), compareFile = commonEnumHellper.ListCompare.ToList() });
        }

        // use search NangCao KhachHang
        public string SearchColumn(List<ColumSearch> lstColumn, string whereSql, ref Params_GetListKhachHang listParam)
        {
            char[] whitespace = new char[] { ' ', '\t' };
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);

                foreach (var item in lstColumn)
                {
                    if (item.Value != null)
                    {
                        var value = item.Value.ToString().Trim();
                        string where = string.Empty;
                        switch (listParam.LoaiDoiTuong)
                        {
                            case 1:
                                switch (item.Key)
                                {
                                    case (int)commonEnum.ColumnKhachHang.nohientai:
                                        var debit = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(NoHienTai,0) = ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(NoHienTai,0) > ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(NoHienTai,0) >= ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(NoHienTai,0) < ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(NoHienTai,0) <= ", debit);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongban:
                                        var sale = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(TongBan,0) = ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(TongBan,0) > ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(TongBan,0) >= ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(TongBan,0) < ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(TongBan,0) <= ", sale);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongbantrutrahang:
                                        var sale_return = double.Parse(value.ToString().Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(TongBanTruTraHang,0) = ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(TongBanTruTraHang,0) > ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(TongBanTruTraHang,0) >= ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(TongBanTruTraHang,0) < ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(TongBanTruTraHang,0) <= ", sale_return);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongthuKhach:
                                        var tongKhachThanhToan = double.Parse(value.ToString().Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(TongThuKhachHang,0) = ", tongKhachThanhToan);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(TongThuKhachHang,0) > ", tongKhachThanhToan);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(TongThuKhachHang,0) >= ", tongKhachThanhToan);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(TongThuKhachHang,0) < ", tongKhachThanhToan);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(TongThuKhachHang,0) <= ", tongKhachThanhToan);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongChiKhach:
                                        var tongHoanCoc = double.Parse(value.ToString().Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(TongChiKhachHang,0) = ", tongHoanCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(TongChiKhachHang,0) > ", tongHoanCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(TongChiKhachHang,0) >= ", tongHoanCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(TongChiKhachHang,0) < ", tongHoanCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(TongChiKhachHang,0) <= ", tongHoanCoc);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriDVSuDung:
                                        var tongDVKhachSuDung = double.Parse(value.ToString().Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(GiaTriDVSuDung,0) = ", tongDVKhachSuDung);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(GiaTriDVSuDung,0) > ", tongDVKhachSuDung);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(GiaTriDVSuDung,0) >= ", tongDVKhachSuDung);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(GiaTriDVSuDung,0) < ", tongDVKhachSuDung);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(GiaTriDVSuDung,0) <= ", tongDVKhachSuDung);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriDVKhachTra:
                                        var gtriHoanDV = double.Parse(value.ToString().Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(GiaTriDVHoanTra,0) = ", gtriHoanDV);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(GiaTriDVHoanTra,0) > ", gtriHoanDV);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(GiaTriDVHoanTra,0) >= ", gtriHoanDV);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(GiaTriDVHoanTra,0) < ", gtriHoanDV);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(GiaTriDVHoanTra,0) <= ", gtriHoanDV);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tienChuaSD:
                                        var tienChuaSD = double.Parse(value.ToString().Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(SoTienChuaSD,0) = ", tienChuaSD);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" ISNULL(SoTienChuaSD,0) > ", tienChuaSD);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" ISNULL(SoTienChuaSD,0) >= ", tienChuaSD);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" ISNULL(SoTienChuaSD,0) < ", tienChuaSD);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" ISNULL(SoTienChuaSD,0) <= ", tienChuaSD);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongtichdiem:
                                        var point = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" TongTichDiem = ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" TongTichDiem > ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" TongTichDiem >= ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" TongTichDiem < ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" TongTichDiem <= ", point);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.madoituong:
                                        where = string.Concat(" MaDoiTuong like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tendoituong:
                                        where = string.Concat(" (TenDoiTuong like N'%", value, "%' OR TenDoiTuong_KhongDau like '%", CommonStatic.ConvertToUnSign(value), "%')");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.dienthoai:
                                        where = string.Concat(" DienThoai like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nhomkhach://??
                                        listParam.ID_NhomDoiTuong = value;
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gioitinh:
                                        listParam.GioiTinh = int.Parse(value);
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.ngaysinh:
                                        //listParam.NgaySinh_TuNgay = int.Parse(value);
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.email:
                                        where = string.Concat(" Email like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.diachi:
                                        where = string.Concat(" DiaChi like N'%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tinhthanh:
                                        where = string.Concat(" exists (select * from dbo.splitstring ('", value, "') tinh where tinh.Name = tbl.ID_TinhThanh) ");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.quanhuyen:
                                        where = string.Concat(" exists (select * from dbo.splitstring ('", value, "') huyen where huyen.Name = tbl.ID_QuanHuyen) ");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nguonkhach:
                                        listParam.ID_NguonKhach = value;
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nguoigioithieu:
                                        where = string.Concat(" ID_NguoiGioiThieu like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nguoitao:
                                        where = string.Concat(" NguoiTao like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.trangthaikhachhang:
                                        where = string.Concat(" TheoDoi = ", value, "");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.ghichu:
                                        where = string.Concat(" GhiChu like N'%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.trangthaiSoDuCoc:
                                        var gtriSD = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.lonhon:// Còn tồn cọc
                                                where = string.Concat(" SoDuCoc > ", gtriSD);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon: // Đã dùng hết
                                                where = string.Concat(" SoDuCoc = ", 0);
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriNapCoc:
                                        var gtriNap = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" NapCoc = ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" NapCoc > ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" NapCoc >= ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" NapCoc < ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" NapCoc <= ", gtriNap);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriSuDungCoc:
                                        var gtSuDungCoc = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" SuDungCoc = ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" SuDungCoc > ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" SuDungCoc >= ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" SuDungCoc < ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" SuDungCoc <= ", gtSuDungCoc);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriSoDuCoc:
                                        var gtriSoDuCoc = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" SoDuCoc = ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" SoDuCoc > ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" SoDuCoc >= ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" SoDuCoc < ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" SoDuCoc <= ", gtriSoDuCoc);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.laCaNhan:
                                        where = string.Concat(" LaCanhan = ", value);
                                        break;
                                }

                                break;
                            case 2:
                                switch (item.Key)
                                {
                                    case (int)commonEnum.ColumnKhachHang.nohientai:
                                        var debit = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(NoHienTai,0) = ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat("- ISNULL(NoHienTai,0) > ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" -ISNULL(NoHienTai,0) >= ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" -ISNULL(NoHienTai,0) < ", debit);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" -ISNULL(NoHienTai,0) <= ", debit);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongban:
                                        var sale = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" ISNULL(TongBan,0) = ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" -ISNULL(TongBan,0) > ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" -ISNULL(TongBan,0) >= ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" -ISNULL(TongBan,0) < ", sale);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" -ISNULL(TongBan,0) <= ", sale);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongbantrutrahang:
                                        var sale_return = double.Parse(value.ToString().Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" -ISNULL(TongBanTruTraHang,0) = ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" -ISNULL(TongBanTruTraHang,0) > ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" -ISNULL(TongBanTruTraHang,0) >= ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" -ISNULL(TongBanTruTraHang,0) < ", sale_return);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" -ISNULL(TongBanTruTraHang,0) <= ", sale_return);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tongtichdiem:
                                        var point = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" TongTichDiem = ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" TongTichDiem > ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" TongTichDiem >= ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" TongTichDiem < ", point);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" TongTichDiem <= ", point);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.madoituong:
                                        where = string.Concat(" MaDoiTuong like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tendoituong:
                                        where = string.Concat(" (TenDoiTuong like N'%", value, "%' OR TenDoiTuong_KhongDau like '%", CommonStatic.ConvertToUnSign(value), "%')");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.dienthoai:
                                        where = string.Concat(" DienThoai like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nhomkhach://??
                                        listParam.ID_NhomDoiTuong = value;
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gioitinh:
                                        listParam.GioiTinh = int.Parse(value);
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.ngaysinh:
                                        //listParam.NgaySinh_TuNgay = int.Parse(value);
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.email:
                                        where = string.Concat(" Email like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.diachi:
                                        where = string.Concat(" DiaChi like N'%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.tinhthanh:
                                        where = string.Concat(" ID_TinhThanh like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.quanhuyen:
                                        where = string.Concat(" ID_QuanHuyen like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nguonkhach:
                                        listParam.ID_NguonKhach = value;
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nguoigioithieu:
                                        where = string.Concat(" ID_NguoiGioiThieu like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.nguoitao:
                                        where = string.Concat(" NguoiTao like '%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.trangthaikhachhang:
                                        where = string.Concat(" TheoDoi = ", value, "");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.ghichu:
                                        where = string.Concat(" GhiChu like N'%", value, "%'");
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.trangthaiSoDuCoc:
                                        var gtriSD = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.nhohon:// Còn tồn cọc
                                                where = string.Concat(" isnull(SoDuCoc,0) > ", gtriSD);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang: // Đã dùng hết (có cọc nhưng dùng hết)
                                                where = string.Concat(" isnull(SoDuCoc,0) = 0 and isnull(NapCoc,0) > 0");
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriNapCoc:
                                        var gtriNap = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" isnull(NapCoc,0) = ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" isnull(NapCoc,0) > ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" isnull(NapCoc,0) >= ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" isnull(NapCoc,0) < ", gtriNap);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" isnull(NapCoc,0) <= ", gtriNap);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriSuDungCoc:
                                        var gtSuDungCoc = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" isnull(SuDungCoc,0) = ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" isnull(SuDungCoc,0) > ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" isnull(SuDungCoc,0) >= ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" isnull(SuDungCoc,0) < ", gtSuDungCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" isnull(SuDungCoc,0) <= ", gtSuDungCoc);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case (int)commonEnum.ColumnKhachHang.gtriSoDuCoc:
                                        var gtriSoDuCoc = double.Parse(value.Replace(",", ""));
                                        switch (item.type)
                                        {
                                            case (int)commonEnumHellper.KeyCompare.bang:
                                                where = string.Concat(" isnull(SoDuCoc,0) = ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhon:
                                                where = string.Concat(" isnull(SoDuCoc,0) > ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                                where = string.Concat(" isnull(SoDuCoc,0) >= ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohon:
                                                where = string.Concat(" isnull(SoDuCoc,0) < ", gtriSoDuCoc);
                                                break;
                                            case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                                where = string.Concat(" isnull(SoDuCoc,0) <= ", gtriSoDuCoc);
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }

                        whereSql = classDoiTuong.GetStringWhere(whereSql, where);
                    }
                }
            }
            return whereSql;
        }

        public List<SP_DM_DoiTuong> SearchColumn(List<ColumSearch> lstColumn, List<SP_DM_DoiTuong> lst)
        {
            char[] whitespace = new char[] { ' ', '\t' };
            var data = lst;
            foreach (var item in lstColumn)
            {
                if (item.Value != null)
                {
                    var value = item.Value.ToString();
                    string where = string.Empty;
                    switch (item.Key)
                    {
                        case (int)commonEnum.ColumnKhachHang.nohientai:
                            var debit = double.Parse(value.Replace(",", ""));
                            switch (item.type)
                            {
                                case (int)commonEnumHellper.KeyCompare.bang:
                                    data = data.Where(x => x.NoHienTai == debit).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhon:
                                    data = data.Where(x => x.NoHienTai > debit).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                    data = data.Where(x => x.NoHienTai >= debit).ToList(); ;
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohon:
                                    data = data.Where(x => x.NoHienTai < debit).ToList(); ;
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                    data = data.Where(x => x.NoHienTai <= debit).ToList(); ;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case (int)commonEnum.ColumnKhachHang.tongban:
                            var sale = double.Parse(value.Replace(",", ""));
                            switch (item.type)
                            {
                                case (int)commonEnumHellper.KeyCompare.bang:
                                    data = data.Where(x => x.TongBan == sale).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhon:
                                    data = data.Where(x => x.TongBan > sale).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                    data = data.Where(x => x.TongBan >= sale).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohon:
                                    data = data.Where(x => x.TongBan < sale).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                    data = data.Where(x => x.TongBan <= sale).ToList();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case (int)commonEnum.ColumnKhachHang.tongbantrutrahang:
                            var sale_return = double.Parse(value.ToString().Replace(",", ""));
                            switch (item.type)
                            {
                                case (int)commonEnumHellper.KeyCompare.bang:
                                    data = data.Where(x => x.TongBanTruTraHang == sale_return).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhon:
                                    data = data.Where(x => x.TongBanTruTraHang > sale_return).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                    data = data.Where(x => x.TongBanTruTraHang >= sale_return).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohon:
                                    data = data.Where(x => x.TongBanTruTraHang < sale_return).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                    data = data.Where(x => x.TongBanTruTraHang <= sale_return).ToList();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case (int)commonEnum.ColumnKhachHang.tongtichdiem:
                            var point = double.Parse(value.Replace(",", ""));
                            switch (item.type)
                            {
                                case (int)commonEnumHellper.KeyCompare.bang:
                                    data = data.Where(x => x.TongTichDiem == point).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhon:
                                    data = data.Where(x => x.TongTichDiem > point).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.lonhonhoacbang:
                                    data = data.Where(x => x.TongTichDiem >= point).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohon:
                                    data = data.Where(x => x.TongTichDiem < point).ToList();
                                    break;
                                case (int)commonEnumHellper.KeyCompare.nhohonhoacbang:
                                    data = data.Where(x => x.TongTichDiem <= point).ToList();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case (int)commonEnum.ColumnKhachHang.madoituong:
                            var s = data.Select(x => x.MaDoiTuong.ToLower());
                            data = data.Where(x => x.MaDoiTuong.ToLower().Contains(value)).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.tendoituong:
                            string[] textFilter = item.Value.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Trim().Split(whitespace);
                            string[] utf8 = textFilter.Where(o => o.Any(c => Model.Service.common.StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                            string[] utf = textFilter.Where(o => !o.Any(c => Model.Service.common.StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                            data = data.Where(o => o.TenDoiTuong != null
                                                && utf8.All(c => o.TenDoiTuong.ToLower().Contains(c))
                                                && (utf.All(c => CommonStatic.RemoveSign4VietnameseString(o.TenDoiTuong.ToLower()).Contains(c))
                                                 )).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.dienthoai:
                            data = data.Where(x => x.DienThoai.Contains(value)).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.email:
                            data = data.Where(x => x.Email.Contains(value)).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.diachi:
                            data = data.Where(x => x.DiaChi.Contains(value)).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.quanhuyen:
                            data = data.Where(x => x.ID_QuanHuyen != null && x.ID_QuanHuyen == new Guid(value)).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.nguoigioithieu:
                            data = data.Where(x => x.ID_NguoiGioiThieu != null && x.ID_NguoiGioiThieu == new Guid(value)).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.nguoitao:
                            data = data.Where(x => x.NguoiTao.Contains(value)).ToList();
                            break;
                        case (int)commonEnum.ColumnKhachHang.ghichu:
                            data = data.Where(x => x.GhiChu.Contains(value)).ToList();
                            break;
                    }
                }
            }
            return data;
        }

        [HttpGet, HttpPost]// use to at menu KhachHang
        public IHttpActionResult GetListKhachHang_Where_PassObject_Paging(Params_GetListKhachHang listParams)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var whereColumn = SearchColumn(listParams.SearchColumns, string.Empty, ref listParams);
                    listParams.WhereSql = whereColumn;
                    List<SP_DM_DoiTuong> data = classdoituong.SP_GetListKhachHang_Where_Paging(listParams);

                    if (data != null && data.Count() > 0)
                    {
                        var itFirst = data[0];
                        return Json(new
                        {
                            res = true,
                            data,
                            itFirst.TotalRow,
                            itFirst.TotalPage,
                            itFirst.NoHienTaiAll,
                            itFirst.TongBanAll,
                            itFirst.TongBanTruTraHangAll,
                            itFirst.TongTichDiemAll,
                            itFirst.TongPhiDichVu,
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            res = true,
                            data = new List<SP_DM_DoiTuong>()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = false,
                    mes = string.Concat("GetListKhachHang_Where_PassObject_Paging", ex.InnerException, ex.Message)
                });
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult LoadDanhMuc_KhachHangNhaCungCap(Params_GetListKhachHang listParams)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var whereColumn = SearchColumn(listParams.SearchColumns, string.Empty, ref listParams);
                    listParams.WhereSql = whereColumn;
                    List<SP_DM_DoiTuong> data = classdoituong.LoadDanhMuc_KhachHangNhaCungCap(listParams);

                    if (data != null && data.Count() > 0)
                    {
                        var itFirst = data[0];
                        return Json(new
                        {
                            res = true,
                            data,
                            itFirst.TotalRow,
                            itFirst.TotalPage,
                            itFirst.NoHienTaiAll,
                            itFirst.TongBanAll,
                            itFirst.TongBanTruTraHangAll,
                            itFirst.TongTichDiemAll,
                            itFirst.TongPhiDichVu,
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            res = true,
                            data = new List<SP_DM_DoiTuong>()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = false,
                    mes = string.Concat("GetListKhachHang_Where_PassObject_Paging", ex.InnerException, ex.Message)
                });
            }
        }

        [HttpGet, HttpPost]
        public IEnumerable<SP_DM_DoiTuong> GetListKhachHang_Where_PassObject_IEnumerable(Params_GetListKhachHang listParams)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var lstAll = classdoituong.SP_GetListKhachHang_Where_ReturnIEnumerable(listParams);
                    if (lstAll != null)
                    {
                        return lstAll;
                    }
                    else
                    {
                        //return Enumerable.Empty<SP_DM_DoiTuong>();
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetListKhachHang_Where_PassObject_IEnumerable " + ex.InnerException + ex.Message);
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult GetNoKhachHang_byDate(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    List<SP_DM_DoiTuong> data = classdoituong.GetNoKhachHang_byDate(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IEnumerable<SP_DM_DoiTuong> GetInforKhachHang_ByID(Guid idDoiTuong, Guid idChiNhanh, DateTime timeStart, DateTime timeEnd, bool wasChotSo)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                List<SP_DM_DoiTuong> lstAllKhacHangs = classdoituong.SP_GetInforKhachHang_ByID(idDoiTuong, idChiNhanh, timeStart, timeEnd, wasChotSo);

                if (lstAllKhacHangs != null)
                {
                    return lstAllKhacHangs;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpGet]
        public List<SP_KhachHang_HangHoa> SP_GetSoLuongHangMua_ofKhachHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                return classdoituong.SP_GetSoLuongHangMua_ofKhachHang();
            }
        }

        [HttpGet]
        public double GetDebitCustomer_allBrands(Guid idDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                return classdoituong.GetDebitCustomer_allBrands(idDoiTuong);
            }
        }

        [HttpGet]
        public IHttpActionResult GetTienDatCoc_ofDoiTuong(Guid idDoiTuong, Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var data = classdoituong.GetTienDatCoc_ofDoiTuong(idDoiTuong, idDonVi);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpPost]
        public IHttpActionResult GetNhatKyTienCoc_OfDoiTuong(ParamGetListBaoHiem_v1 param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                List<SoQuyDTO> data = classdoituong.GetNhatKyTienCoc_OfDoiTuong(param);
                var count = data.Count() > 0 ? (int)data[0].TotalRow : 0;
                int page = 0;
                var listpage = GetListPage(count, param.PageSize, param.CurrentPage, ref page);
                return ActionTrueData(new
                {
                    data,
                    ListPage = listpage,
                    PageView = string.Concat("Hiển thị ", (param.CurrentPage * param.PageSize + 1),
                    " - ", (param.CurrentPage * param.PageSize + data.Count()), " trên tổng số ", count, " bản ghi"),
                    TotalPage = page,
                    TotalRow = count,
                });
            }
        }

        [HttpGet]
        public IEnumerable<SP_KhachHang_TheGiaTri> Get_SoDuTheGiaTri_ofKhachHang(Guid idDoiTuong, string datetime)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                return classdoituong.SP_GetSoDuTheGiaTri_ofKhachHang(idDoiTuong, datetime).ToList();
            }
        }

        [HttpGet]
        public double Get_SoDuTheGiaTri_ofKhachHang_ByTime(Guid idDoiTuong, string datetime)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                return classdoituong.SP_GetSoDuTheGiaTri_ofKhachHang_byTime(idDoiTuong, datetime);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListHisUsed_ValueCard(ParamNKyGDV param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassReportTheGiaTri reportTheGiaTri = new ClassReportTheGiaTri(db);
                    List<SP_ReportValueCard_HisUsed> lst = reportTheGiaTri.SP_ValueCard_GetListHisUsed(param);
                    int count = 0, page = 0;
                    int currentPage = param.CurrentPage ?? 0, pageSize = param.PageSize ?? 20;
                    if (lst != null && lst.Count > 0)
                    {
                        count = lst[0].TotalRow;
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
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = false,
                    mes = ex.InnerException + ex.Message,
                });
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetList_LichSuNapTien(ParamNKyGDV param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassReportTheGiaTri reportTheGiaTri = new ClassReportTheGiaTri(db);
                    List<TheGiaTriDTO> data = reportTheGiaTri.TheGiaTri_GetLichSuNapTien(param);

                    return Json(new
                    {
                        res = true,
                        data = data,
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    res = false,
                    mes = ex.InnerException + ex.Message,
                });
            }
        }

        //get list lịch sử nạp tiền theo id đối tượng
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetLichSuNapTienByIDDoiTuong(Guid iddt, int currentpage, int pagesize, DateTime dayStart, DateTime dayEnd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var tbl = (from hd in db.BH_HoaDon
                               where hd.LoaiHoaDon == 22 && hd.ID_DoiTuong == iddt
                               select new
                               {
                                   MaHoaDon = hd.MaHoaDon,
                                   NgayLapHoaDon = hd.NgayLapHoaDon,
                                   MucNap = hd.TongChiPhi,
                                   KhuyenMai = hd.TongChietKhau,
                                   TongTienNap = hd.TongTienHang,
                                   SoDuSauNap = hd.TongTienThue
                               }).ToList();
                    if (dayStart == dayEnd)
                    {
                        tbl = tbl.Where(hd => hd.NgayLapHoaDon.Year == dayStart.Year
                        && hd.NgayLapHoaDon.Month == dayStart.Month
                        && hd.NgayLapHoaDon.Day == dayStart.Day).OrderByDescending(x => x.NgayLapHoaDon).ToList();
                    }
                    else
                    {
                        tbl = tbl.Where(hd => hd.NgayLapHoaDon >= dayStart && hd.NgayLapHoaDon < dayEnd).OrderByDescending(x => x.NgayLapHoaDon).ToList();
                    }
                    var totalrecord = tbl.Count();
                    return Json(new
                    {
                        lst = tbl,
                        pageCount = System.Math.Ceiling(totalrecord * 1.0 / pagesize),
                        Rowcount = totalrecord
                    });
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public string UpdateDieuChinhSoDu(double chenhlech, int trangthai, Guid iddt, Guid iddonvi, Guid idnhanvien, string nguoitao, double sodusaunap)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classhoadon = new ClassBH_HoaDon(db);
                if (db == null)
                {
                    return "Error";
                }
                else
                {
                    DM_DoiTuong dt = db.DM_DoiTuong.Find(iddt);
                    dt.TrangThai_TheGiaTri = trangthai;
                    db.Entry(dt).State = EntityState.Modified;
                    db.SaveChanges();

                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                    string sMaHoaDon = string.Empty;
                    sMaHoaDon = classhoadon.GetAutoCode(23);
                    itemBH_HoaDon.ID = Guid.NewGuid();
                    itemBH_HoaDon.MaHoaDon = sMaHoaDon;
                    itemBH_HoaDon.ID_NhanVien = idnhanvien;
                    itemBH_HoaDon.NguoiTao = nguoitao;
                    itemBH_HoaDon.ID_DoiTuong = iddt;
                    itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                    itemBH_HoaDon.TongChiPhi = chenhlech; // mức điều chỉnh
                    itemBH_HoaDon.TongTienHang = chenhlech;
                    itemBH_HoaDon.TongTienThue = sodusaunap; // số dư sau điều chỉnh
                    itemBH_HoaDon.ID_DonVi = iddonvi;
                    itemBH_HoaDon.NgayTao = DateTime.Now;
                    itemBH_HoaDon.ChoThanhToan = false;
                    itemBH_HoaDon.LoaiHoaDon = 23;
                    db.BH_HoaDon.Add(itemBH_HoaDon);
                    db.SaveChanges();

                    //List<SqlParameter> paramlist = new List<SqlParameter>();
                    //paramlist.Add(new SqlParameter("NgayLapHoaDonInput", itemBH_HoaDon.NgayLapHoaDon));
                    //paramlist.Add(new SqlParameter("IDDoiTuong", iddt));
                    //try
                    //{
                    //    Thread str1 = new Thread(() => db.Database.ExecuteSqlCommand("exec UpdateLaiSoDuTheNap @NgayLapHoaDonInput, @IDDoiTuong", paramlist.ToArray()));
                    //    str1.Start();
                    //}
                    //catch (Exception ex)
                    //{
                    //    CookieStore.WriteLog("DM_DoiTuongAPI_UpdateDieuChinhSoDu: " + ex.Message + ex.InnerException);
                    //}
                    return "";
                }
            }
        }

        [HttpGet]
        public IEnumerable<SP_DM_DoiTuong> GetListKhachHang(int loaiDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                var data = classdoituong.Gets(gr => gr.LoaiDoiTuong == loaiDoiTuong && gr.TheoDoi != true).Select(p => new SP_DM_DoiTuong()
                {
                    ID = p.ID,
                    ID_NhomDoiTuong = p.IDNhomDoiTuongs,
                    TenDoiTuong = p.TenDoiTuong,
                    MaDoiTuong = p.MaDoiTuong,
                    GioiTinhNam = p.GioiTinhNam,
                    NgaySinh_NgayTLap = p.NgaySinh_NgayTLap,
                    DienThoai = p.DienThoai,
                    Email = p.Email,
                    DiaChi = p.DiaChi,
                    TenNhomDT = p.TenNhomDoiTuongs,
                    MaSoThue = p.MaSoThue,
                    GhiChu = p.GhiChu,
                    NgayTao = p.NgayTao,
                    ID_NguonKhach = p.ID_NguonKhach,
                    ID_NhanVienPhuTrach = p.ID_NhanVienPhuTrach,
                    ID_NguoiGioiThieu = p.ID_NguoiGioiThieu,
                    LaCaNhan = p.LaCaNhan,
                });
                return data;
            }
        }

        [HttpGet]
        public List<SP_DM_DoiTuong> GetListKH_InforBasic(Guid idChiNhanh, int loaiDoiTuong)
        {
            var dateStart = Convert.ToDateTime("2016-01-01");
            var tomorrow = DateTime.Today.AddDays(1);
            string columsort = null;
            string sort = null;
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    List<SP_DM_DoiTuong> lstKhacHangs = classdoituong.SP_GetListKhachHang_Where(idChiNhanh, loaiDoiTuong,
                string.Empty, null, dateStart, tomorrow, dateStart, tomorrow, 0, 0, null, null, columsort, sort);
                    return lstKhacHangs;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_DoiTuongAPI_GetListKH_InforBasic: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        [HttpGet, HttpPost]
        [Compress.DeflateCompression]
        public IEnumerable<SP_DM_DoiTuong> GetListKH_PassObject_ByNhanVien(Params_GetListKhachHang lstParam)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    return new classDM_DoiTuong(db).SP_GetListKhachHang_ByNhanVien(lstParam);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_DoiTuongAPI_GetListKH_PassObject_ByNhanVien: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        [Compress.DeflateCompression]
        [HttpGet]
        public IEnumerable<SP_DM_DoiTuong> GetAllKhachHang_NotWhere()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    HT_NguoiDung user = contant.GetUserCookies();
                    return new classDM_DoiTuong(db).GetAllKhachHang_NotWhere(user.ID_DonVi, user.ID_NhanVien);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_DoiTuongAPI_GetListKH_PassObject_ByNhanVien: " + ex);
                return null;
            }
        }

        [HttpGet]
        public List<Guid> GetCustomer_wasDelete()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    return new classDM_DoiTuong(db).GetCustomer_wasDelete();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("GetCustomer_wasDelete: " + ex);
                return new List<Guid>();
            }
        }
        [HttpGet]
        public bool RestoreCus(Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    DM_DoiTuong obj = db.DM_DoiTuong.Find(id);
                    obj.TheoDoi = false;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("RestoreCus: " + ex);
                return false;
            }
        }

        [HttpGet]
        public List<SoQuyDTO> GetHoaDon_SoQuy_ofDoiTuong(Guid idDoiTuong, string idChiNhanh = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classhoadon = new ClassBH_HoaDon(db);
                var data = classhoadon.SP_GetHoaDonandSoQuy_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                if (data != null)
                {
                    try
                    {
                        // order by NgayLap DESC, LoaiHoaDon ASC --> display: SoQuy, HoaDon
                        data = data.OrderByDescending(hd => hd.NgayLapHoaDon).ThenByDescending(x => x.MaHoaDon).ToList();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("DM_DoiTuongAPI_GetHoaDon_SoQuy_ofDoiTuong: " + ex.Message + ex.InnerException);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetChiPhiDichVu_byVendor(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                try
                {
                    List<BH_HoaDonDTO> data = classDoiTuong.GetChiPhiDichVu_byVendor(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpGet]
        public List<BH_HoaDonDTO> GetLichSu_TichDiem(Guid idDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classhoadon = new ClassBH_HoaDon(db);
                var data = classhoadon.GetLichSu_TichDiem(idDoiTuong);
                List<BH_HoaDonDTO> lstReturn = new List<BH_HoaDonDTO>();
                if (data != null && data.Count() > 0)
                {
                    try
                    {
                        data = data.OrderBy(x => x.NgayLapHoaDon).Reverse().ToList();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("DM_DoiTuongAPI_GetLichSu_TichDiem: " + ex.Message + ex.InnerException);
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
        public int GetLoaiDoiTuong_ByTenDoiTuong(string tenDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                libNS_NhanVien.ClassNS_NhanVien classNhanVien = new libNS_NhanVien.ClassNS_NhanVien(db);

                DM_DoiTuong doituong = classdoituong.Gets(gr => gr.TenDoiTuong == tenDoiTuong).FirstOrDefault();
                if (doituong == null)
                {
                    NS_NhanVien nhanvien = classNhanVien.Gets(ct => ct.TenNhanVien == tenDoiTuong).FirstOrDefault();
                    return 3;
                }
                return doituong.LoaiDoiTuong;
            }
        }

        // GET: api/DM_DoiTuongAPI/5
        [ResponseType(typeof(DM_DoiTuongDTO))]
        public IHttpActionResult GetDM_DoiTuong(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                DM_DoiTuong DM_DoiTuong = classdoituong.Select_DoiTuong(id);
                DM_DoiTuongDTO ct = new DM_DoiTuongDTO();
                ct.ID = DM_DoiTuong.ID;
                ct.TenDoiTuong = DM_DoiTuong.TenDoiTuong;
                ct.MaDoiTuong = DM_DoiTuong.MaDoiTuong;
                ct.DienThoai = DM_DoiTuong.DienThoai;
                ct.Email = DM_DoiTuong.Email;
                ct.DiaChi = DM_DoiTuong.DiaChi;
                ct.NoHienTai = 0;
                ct.TongTichDiem = DM_DoiTuong.TongTichDiem;
                ct.GhiChu = DM_DoiTuong.GhiChu;
                ct.MaSoThue = DM_DoiTuong.MaSoThue;
                ct.ID_NguonKhach = DM_DoiTuong.ID_NguonKhach;
                ct.ID_NguoiGioiThieu = DM_DoiTuong.ID_NguoiGioiThieu;
                ct.ID_NhomDoiTuong = DM_DoiTuong.IDNhomDoiTuongs;
                ct.ID_NhanVienPhuTrach = DM_DoiTuong.ID_NhanVienPhuTrach;
                ct.ID_TinhThanh = DM_DoiTuong.ID_TinhThanh;
                ct.ID_QuanHuyen = DM_DoiTuong.ID_QuanHuyen;
                ct.NhomDoiTuong = DM_DoiTuong.TenNhomDoiTuongs;
                if (DM_DoiTuong == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        [HttpGet]
        public IHttpActionResult GetDoiTuong_hasDienThoai(string idNhoms)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                if (string.IsNullOrEmpty(idNhoms))
                {
                    var lst = classdoituong.Gets(x => x.LoaiDoiTuong == 1 && x.DienThoai != null && x.DienThoai.Trim() != string.Empty).
                        Select(x => new { x.DienThoai }).ToList();
                    return Json(new { res = true, data = lst });
                }
                else
                {
                    SqlParameter param = new SqlParameter("IDNhoms", idNhoms);
                    var lst = db.Database.SqlQuery<SMS_DienThoai>("GetKhachHanghasDienThoai_byIDNhoms @IDNhoms", param).ToList();
                    return Json(new { res = true, data = lst });
                }
            }
        }

        class SMS_DienThoai
        {
            public Guid ID { get; set; }
            public string MaDoiTuong { get; set; }
            public string TenDoiTuong { get; set; }
            public string DienThoai { get; set; }
            public string Email { get; set; }
            public double? TongTichDiem { get; set; }// used when update phieuthu/chi: get diemhientai of khachhang
        }

        public bool CheckDoiTuong_ExistHoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classhoadon = new ClassBH_HoaDon(db);
                bool exist = false;
                List<BH_HoaDon> lst = classhoadon.Gets(hd => hd.ID_DoiTuong == id).ToList();
                if (lst.Count > 0)
                {
                    exist = true;
                }
                return exist;
            }
        }

        public bool Check_ExistCode(string code, Guid? id = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                return classdoituong.Check_ExistCode(code, id);
            }
        }

        public string Check_Exist([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                DM_DoiTuong obj = data.ToObject<DM_DoiTuong>();
                string err = string.Empty;
                int countObj = 0;
                // update:
                //if (obj.ID.ToString().IndexOf("0000") == -1)
                if (Guid.Empty != obj.ID)
                {
                    if (obj.MaDoiTuong != null)
                    {
                        // check ma trung
                        countObj = classdoituong.Gets(maDT => maDT.MaDoiTuong == obj.MaDoiTuong && maDT.ID != obj.ID).Count();
                        if (countObj == 0)
                        {
                            // check SDT trung
                            if (obj.DienThoai != null && obj.DienThoai != "")
                            {
                                countObj = classdoituong.Gets(maDT => maDT.DienThoai == obj.DienThoai && maDT.ID != obj.ID).Count();
                                if (countObj > 0)
                                {
                                    err = "Số điện thoại đã tồn tại";
                                }
                            }
                        }
                        else
                        {
                            err = "Mã đối tượng đã tồn tại";
                        }
                    }
                    else
                    {
                        // check SDT trung
                        if (obj.DienThoai != null && obj.DienThoai != "")
                        {
                            countObj = classdoituong.Gets(maDT => maDT.DienThoai == obj.DienThoai && maDT.ID != obj.ID).Count();
                            if (countObj > 0)
                            {
                                err = "Số điện thoại đã tồn tại";
                            }
                        }
                    }
                }
                // insert
                else
                {
                    if (obj.MaDoiTuong != null)
                    {
                        // check ma trung
                        countObj = classdoituong.Gets(maDT => maDT.MaDoiTuong == obj.MaDoiTuong).Count();
                        if (countObj == 0)
                        {
                            // check SDT trung
                            if (obj.DienThoai != null)
                            {
                                countObj = classdoituong.Gets(maDT => maDT.DienThoai == obj.DienThoai).Count();
                                if (countObj > 0)
                                {
                                    err = "Số điện thoại đã tồn tại";
                                }
                            }
                        }
                        else
                        {
                            err = "Mã đối tượng đã tồn tại";
                        }
                    }
                    else
                    {
                        // check SDT trung
                        if (obj.DienThoai != null)
                        {
                            countObj = classdoituong.Gets(maDT => maDT.DienThoai == obj.DienThoai).Count();
                            if (countObj > 0)
                            {
                                err = "Số điện thoại đã tồn tại";
                            }
                        }
                    }
                }
                return err;
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult GetListHD_isDebit(Guid idDoiTuong, List<string> arrDonVi, int loaiDoiTuong = 1)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var data = classdoituong.SP_GetListHD_isDebit(idDoiTuong, arrDonVi, loaiDoiTuong).OrderBy(x => x.NgayLapHoaDon);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpGet]
        public IEnumerable<Object> GetImages_DoiTuong(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_DoiTuong_Anh classDoiTuongAnh = new ClassDM_DoiTuong_Anh(db);
                IEnumerable<Object> result = null;
                var data = classDoiTuongAnh.Gets(x => x.ID_DoiTuong == id);
                if (data != null)
                {
                    result = data.Select(x => new
                    {
                        ID = x.ID,
                        URLAnh = x.URLAnh,
                        SoThuTu = x.SoThuTu,
                    });
                }
                return result;
            }
        }

        [HttpGet]
        public IEnumerable<Object> GetAllImg_DoiTuong()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_DoiTuong_Anh classDoiTuongAnh = new ClassDM_DoiTuong_Anh(db);
                IEnumerable<Object> result = null;
                var data = classDoiTuongAnh.Gets(null);
                if (data != null)
                {
                    result = data.Select(x => new
                    {
                        ID = x.ID,
                        ID_DoiTuong = x.ID_DoiTuong, // use at BanLe_KhuyenMai
                        URLAnh = x.URLAnh,
                        SoThuTu = x.SoThuTu,
                    });
                }
                return result;
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetDSGoiDichVu_ofKhachHang(ParamNKyGDV param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBH_HoaDon_ChiTiet classBHChiTiet = new ClassBH_HoaDon_ChiTiet(db);
                    List<GoiDichVu_KhachHang> lst = classBHChiTiet.GetDSGoiDichVu_ofKhachHang(param);
                    // Mượn trường: LoaiHoaDon (1.Còn buổi sử dụng, 0. hết buổi sử dụng)
                    if (!string.IsNullOrEmpty(param.LoaiHoaDons))
                    {
                        switch (param.LoaiHoaDons)
                        {
                            case "0":
                                lst = lst.Where(x => x.SoLuongConLai == 0).ToList();
                                break;
                            case "1":
                                lst = lst.Where(x => x.SoLuongConLai > 0).ToList();
                                break;
                        }
                    }
                    var data = lst.GroupBy(x => new
                    {
                        x.ID_GoiDV,
                        x.MaHoaDon,
                        x.BienSo,
                        x.NgayLapHoaDon,
                        x.NgayApDungGoiDV,
                        x.HanSuDungGoiDV,
                        x.ID_Xe
                    }).Select(x => new
                    {
                        x.Key.ID_GoiDV,
                        x.Key.MaHoaDon,
                        x.Key.NgayLapHoaDon,
                        x.Key.NgayApDungGoiDV,
                        x.Key.HanSuDungGoiDV,
                        x.Key.BienSo,
                        x.Key.ID_Xe,
                        TongSLMua = x.Where(o => o.ID_ChiTietGoiDV != o.ID_ParentCombo).Sum(o => o.SoLuongMua),
                        TongSLDaDung = x.Where(o => o.ID_ChiTietGoiDV != o.ID_ParentCombo).Sum(o => o.SoLuongDung),
                        TongSLTang = x.Where(o => o.ID_ChiTietGoiDV != o.ID_ParentCombo).Sum(o => o.SoLuongTang),
                        TongSLConLai = x.Where(o => o.ID_ChiTietGoiDV != o.ID_ParentCombo).Sum(o => o.SoLuongConLai),
                        lstDetail = x.GroupBy(y => new { y.ID_ParentCombo })
                            .Select(y => new
                            {
                                y.Key.ID_ParentCombo,
                                ComBo = y.OrderBy(o => o.SoThuTu),
                            })
                    }).ToList();
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException + ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetListTrangThaiTimKiem()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var ttkhachhang = db.DM_DoiTuong_TrangThai.Select(o => new { IsSelected = false, o.ID, Name = o.TenTrangThai }).ToList();
                    return Json(new { res = true, data = ttkhachhang });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException });
            }
        }
        #endregion

        #region update
        // PUT: api/DM_DoiTuongAPI/5
        [ResponseType(typeof(string))]
        [HttpGet, HttpPost, HttpPut]
        public IHttpActionResult PutDM_DoiTuong([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    DM_DoiTuong objUp = data["objDoiTuong"].ToObject<DM_DoiTuong>();
                    var codeEx = classdoituong.Check_ExistCode(objUp.MaDoiTuong, objUp.ID);
                    if (codeEx)
                    {
                        return Json(new { res = false, mes = "Mã đối tượng đã tồn tại" });
                    }
                    if (objUp.MaDoiTuong == string.Empty)
                    {
                        switch (objUp.LoaiDoiTuong)
                        {
                            case 2:
                            case 3:
                            case 4:
                                objUp.MaDoiTuong = classdoituong.SP_GetautoCode(objUp.LoaiDoiTuong);
                                break;
                            default:
                                objUp.MaDoiTuong = classdoituong.GetMaDoiTuongMax_byTemp(objUp.LoaiDoiTuong, objUp.ID_DonVi);
                                break;
                        }
                    }
                    codeEx = classdoituong.SP_CheckSoDienThoai_Exist(objUp.DienThoai, objUp.ID);
                    if (codeEx)
                    {
                        return Json(new { res = false, mes = "Số điện thoại đã tồn tại" });
                    }
                    // check email
                    codeEx = classdoituong.CheckEmail_Exist(objUp.Email, objUp.ID);
                    if (codeEx)
                    {
                        return Json(new { res = false, mes = "Email đã tồn tại" });
                    }
                    string strUpd = classdoituong.Update_DoiTuong(objUp);
                    if (strUpd != string.Empty)
                    {
                        return Json(new { res = false, mes = strUpd });
                    }
                    return Json(new { res = true, data = new { objUp.ID, objUp.MaDoiTuong } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_DoiTuongNhapTraHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                Guid id = data["id"].ToObject<Guid>();
                DM_DoiTuong DM_DoiTuong = data["objDoiTuong"].ToObject<DM_DoiTuong>();
                DM_DoiTuong.PhuongXa = "";

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = classdoituong.Update_DoiTuong(DM_DoiTuong);

                // return
                if (DM_DoiTuong.ID_NhomDoiTuong != null)
                {
                    db.DM_DoiTuong_Nhom.RemoveRange(db.DM_DoiTuong_Nhom.Where(p => p.ID_DoiTuong == DM_DoiTuong.ID));
                    DM_DoiTuong_Nhom dtn = new DM_DoiTuong_Nhom();
                    dtn.ID = Guid.NewGuid();
                    dtn.ID_DoiTuong = DM_DoiTuong.ID;
                    dtn.ID_NhomDoiTuong = DM_DoiTuong.ID_NhomDoiTuong.Value;
                    db.DM_DoiTuong_Nhom.Add(dtn);
                    db.SaveChanges();
                }

                DM_DoiTuong objFind = db.DM_DoiTuong.Find(id);

                DM_DoiTuong objReturn = new DM_DoiTuong();
                objReturn.ID = id;
                objReturn.MaDoiTuong = objFind.MaDoiTuong;
                objReturn.TenDoiTuong = objFind.TenDoiTuong;
                objReturn.ID_NhomDoiTuong = objFind.ID_NhomDoiTuong;
                objReturn.NgaySinh_NgayTLap = objFind.NgaySinh_NgayTLap;
                objReturn.ID_TinhThanh = objFind.ID_TinhThanh;
                objReturn.ID_QuanHuyen = objFind.ID_QuanHuyen;
                objReturn.ID_NhanVienPhuTrach = objFind.ID_NhanVienPhuTrach;
                objReturn.ID_NguonKhach = objFind.ID_NguonKhach;
                objReturn.TenNhomDT = objFind.DM_NhomDoiTuong != null ? objFind.DM_NhomDoiTuong.TenNhomDoiTuong : "Nhóm mặc định";
                objReturn.Email = objFind.Email;
                objReturn.GhiChu = objFind.GhiChu;
                objReturn.MaSoThue = objFind.MaSoThue;
                objReturn.DienThoai = objFind.DienThoai;
                objReturn.DiaChi = objFind.DiaChi;
                objReturn.GioiTinhNam = objFind.GioiTinhNam;
                objReturn.KhuVuc = objFind.DM_TinhThanh != null ? objFind.DM_TinhThanh.TenTinhThanh : "";
                objReturn.PhuongXa = objFind.DM_QuanHuyen != null ? objFind.DM_QuanHuyen.TenQuanHuyen : "";

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage UpdateDiem_DMDoiTuong([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                DM_DoiTuong objDoiTuong = data["objDoiTuong"].ToObject<DM_DoiTuong>();

                DM_DoiTuong objUpd = db.DM_DoiTuong.Find(objDoiTuong.ID);

                if (objUpd != null)
                {
                    objUpd.TongTichDiem = objDoiTuong.TongTichDiem;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                    response.Content = new StringContent("");
                }
                else
                {
                    response.Content = new StringContent("Cập nhật điểm khách hàng lỗi"); ;
                }
                return response;
            }
        }

        [System.Web.Http.HttpPost, HttpGet]
        public HttpResponseMessage HuyHD_UpdateDiem(Guid idDoiTuong, int diemGiaoDich)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                HttpResponseMessage response = new HttpResponseMessage();

                DM_DoiTuong objUpd = db.DM_DoiTuong.Find(idDoiTuong);

                if (objUpd != null)
                {
                    objUpd.TongTichDiem = objUpd.TongTichDiem == null ? diemGiaoDich : objUpd.TongTichDiem + diemGiaoDich;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();

                    response.Content = new StringContent("");
                }
                else
                {
                    response.Content = new StringContent("Cập nhật điểm khách hàng lỗi"); ;
                }
                return response;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult MoveDoiTuong_toOtherGroup(Guid idNhom, Guid idDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                DM_DoiTuong objUpdate = classdoituong.Get(x => x.ID == idDoiTuong);
                if (objUpdate != null)
                {
                    if (idNhom == Guid.Empty)
                    {
                        objUpdate.ID_NhomDoiTuong = null;
                    }
                    else
                    {
                        objUpdate.ID_NhomDoiTuong = idNhom;
                    }

                    string strUpd = classdoituong.Update_DoiTuong(objUpdate);
                    if (strUpd != null && strUpd != string.Empty)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                    else
                        return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
        }
        #endregion

        #region insert
        //trinhpv nâng nhóm đối tượng
        [HttpGet, HttpPost]
        public IHttpActionResult insert_DoiTuong_Nhom(Guid ID_NhomDoiTuong, int LoaiCapNhat)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                string str = string.Empty;
                str = classdoituong.insert_DoiTuongNangNhom(ID_NhomDoiTuong, LoaiCapNhat);
                if (str == string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, str));
                else
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, str));
            }
        }
        [HttpGet]
        public IHttpActionResult NangNhomKhachhang_byIDDoituong(Guid? idDoiTuong, Guid? idChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    if (idDoiTuong != null && idChiNhanh != null)
                    {
                        var classdoituong = new classDM_DoiTuong(db);
                        classdoituong.NangNhomKhachhang_byID(idDoiTuong, idChiNhanh);
                    }
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult UpdateKhachHang_DuDKNangNhom(Guid? idNhomDT, List<Guid> lstIDDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    if (idNhomDT != null && lstIDDonVi != null && lstIDDonVi.Count > 0)
                    {
                        var classdoituong = new classDM_DoiTuong(db);
                        classdoituong.UpdateKhachHang_DuDKNangNhom(idNhomDT, lstIDDonVi);
                    }
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult Creater_NangNhomDoiTuong_ChiNhanh([FromBody] JObject data, Guid? ID_NhomDoiTuong, string User, bool Autocheck, Guid ID_DonVi, Guid ID_NhanVien, int phuongthuc)
        {
            string result = "";
            try
            {
                string chitiet_dt = string.Empty;
                string noidung_dt = string.Empty;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                    var classNhomDoiTuongDonVi = new classNhomDoiTuong_DonVi(db);

                    List<DM_NhomDoiTuongPRC> objNhomDoiTuong = data["objNhomDoiTuong"].ToObject<List<DM_NhomDoiTuongPRC>>();
                    List<DM_NhomDoiTuong_ChiTietPRC> objNhomDoiTuongChiTiet = new List<DM_NhomDoiTuong_ChiTietPRC>();
                    List<Guid> lstIDChiNhanh = data["lstIDChiNhanh"].ToObject<List<Guid>>();

                    Guid ID_NDT = Guid.NewGuid();
                    string sMaNhom = string.Empty;
                    int LoaiDieuKien = 1;
                    int loaiLS = 1;
                    if (ID_NhomDoiTuong == null)
                    {
                        LoaiDieuKien = 1;
                        sMaNhom = classNhomDoiTuong.GetautoCodeNhomDT(1);
                        loaiLS = 1;
                    }
                    else
                    {
                        LoaiDieuKien = 2;
                        ID_NDT = ID_NhomDoiTuong ?? Guid.Empty;
                        loaiLS = 2;
                    }
                    var ctNhom = data["objNhomDoiTuongChiTiet"];
                    if (ctNhom != null && ctNhom.Count() > 0)
                        objNhomDoiTuongChiTiet = data["objNhomDoiTuongChiTiet"].ToObject<List<DM_NhomDoiTuong_ChiTietPRC>>();

                    List<SqlParameter> prmt = new List<SqlParameter>();
                    prmt.Add(new SqlParameter("ID_NhomDoiTuong", ID_NDT));
                    db.Database.ExecuteSqlCommand("exec delete_NhomDoiTuongChiTiet @ID_NhomDoiTuong", prmt.ToArray());

                    string autoUpdate = string.Empty;
                    foreach (var item in objNhomDoiTuong)
                    {
                        var giamgia = item.GiamGia ?? 0;
                        string TenNhomDoiTuong_KhongDau = CommonStatic.ConvertToUnSign(item.TenNhomDoiTuong).ToLower();
                        string TenHangHoa_KyTuDau = CommonStatic.GetCharsStart(item.TenNhomDoiTuong).ToLower();
                        List<SqlParameter> lst_prm = new List<SqlParameter>();
                        lst_prm.Add(new SqlParameter("LoaiDieuKien", LoaiDieuKien));
                        lst_prm.Add(new SqlParameter("ID", ID_NDT));
                        lst_prm.Add(new SqlParameter("LoaiDoiTuong", "1"));
                        lst_prm.Add(new SqlParameter("MaNhomDoiTuong", sMaNhom));
                        lst_prm.Add(new SqlParameter("TenNhomDoiTuong", item.TenNhomDoiTuong));
                        lst_prm.Add(new SqlParameter("TenNhomDoiTuong_KhongDau", TenNhomDoiTuong_KhongDau));
                        lst_prm.Add(new SqlParameter("TenNhomDoiTuong_KyTuDau", TenHangHoa_KyTuDau));
                        lst_prm.Add(new SqlParameter("GhiChu", item.GhiChu));
                        lst_prm.Add(new SqlParameter("GiamGia", giamgia));
                        lst_prm.Add(new SqlParameter("GiamGiaTheoPhanTram", item.GiamGiaTheoPhanTram != null ? item.GiamGiaTheoPhanTram : true));
                        lst_prm.Add(new SqlParameter("NguoiTao", User));
                        lst_prm.Add(new SqlParameter("TimeCreate", DateTime.Now));
                        lst_prm.Add(new SqlParameter("TuDongCapNhat", Autocheck));
                        db.Database.ExecuteSqlCommand("exec insert_NhomDoiTuong @LoaiDieuKien, @ID, @LoaiDoiTuong, @MaNhomDoiTuong, @TenNhomDoiTuong,@TenNhomDoiTuong_KhongDau, @TenNhomDoiTuong_KyTuDau, @GhiChu, @GiamGia, @GiamGiaTheoPhanTram, @NguoiTao, @TimeCreate, @TuDongCapNhat", lst_prm.ToArray());
                    }
                    int i = 1;
                    foreach (var item in objNhomDoiTuongChiTiet)
                    {
                        List<SqlParameter> prm = new List<SqlParameter>();
                        prm.Add(new SqlParameter("ID_NhomDoiTuong", ID_NDT));
                        prm.Add(new SqlParameter("LoaiDieuKien", item.LoaiHinhThuc));
                        prm.Add(new SqlParameter("LoaiSoSang", item.SoSanh));
                        prm.Add(new SqlParameter("GiaTriSo", item.LoaiHinhThuc != 6 ? item.GiaTri : item.ThangSinh));
                        prm.Add(new SqlParameter("GiaTriBool", item.GioiTinh));
                        prm.Add(new SqlParameter("GiaTriThoiGian", item.TimeBy != null ? item.TimeBy : DateTime.Now));
                        prm.Add(new SqlParameter("GiaTriKhuVuc", item.ID_KhuVuc != null ? item.ID_KhuVuc : Guid.NewGuid()));
                        prm.Add(new SqlParameter("GiaTriVungMien", item.ID_VungMien != null ? item.ID_VungMien : Guid.NewGuid()));
                        prm.Add(new SqlParameter("STT", i));
                        db.Database.ExecuteSqlCommand("exec insert_DM_NhomDoiTuong_ChiTiet @ID_NhomDoiTuong, @LoaiDieuKien, @LoaiSoSang, @GiaTriSo, @GiaTriBool, @GiaTriThoiGian, @GiaTriKhuVuc, @GiaTriVungMien, @STT", prm.ToArray());
                        i = i + 1;
                    }

                    #region delete if exist & insert again NhomDoiTuong_DonVi
                    result = classNhomDoiTuongDonVi.Delete(ID_NDT);
                    foreach (var x in lstIDChiNhanh)
                    {
                        NhomDoiTuong_DonVi obj = new NhomDoiTuong_DonVi
                        {
                            ID = Guid.NewGuid(),
                            ID_NhomDoiTuong = ID_NDT,
                            ID_DonVi = x
                        };
                        classNhomDoiTuongDonVi.Add(obj);
                    }
                    #endregion
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, ID_NDT));
                }
            }
            catch (Exception ex)
            {
                result = string.Concat(ex.Message, ex.InnerException);
                CookieStore.WriteLog("Creater_NangNhomDoiTuong_ChiNhanh " + ex.InnerException + ex.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [HttpGet]
        public IHttpActionResult GetNVPhuTrach_ofCustomer(Guid customerId)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var data = (from nvpt in (from nvpt in db.KH_NVPhuTrach
                                          where nvpt.ID_KhachHang == customerId
                                          select new
                                          {
                                              nvpt.ID_NhanVienPhuTrach
                                          })
                            join nv in db.NS_NhanVien on nvpt.ID_NhanVienPhuTrach equals nv.ID
                            select new
                            {
                                nv.ID,
                                nv.MaNhanVien,
                                nv.TenNhanVien
                            }).ToList();
                return ActionTrueData(data);
            }
        }

        [HttpPost]
        public IHttpActionResult PostKH_NhanVienPhuTrach(List<Guid> arrIDNV, Guid idKhachHang)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (idKhachHang != Guid.Empty && arrIDNV.Count > 0)
                        {
                            var lstNVold = db.KH_NVPhuTrach.Where(x => x.ID_KhachHang == idKhachHang);
                            db.KH_NVPhuTrach.RemoveRange(lstNVold);

                            foreach (var item in arrIDNV)
                            {
                                KH_NVPhuTrach objNew = new KH_NVPhuTrach { ID = Guid.NewGuid(), ID_KhachHang = idKhachHang, ID_NhanVienPhuTrach = item };
                                db.KH_NVPhuTrach.Add(objNew);
                            }
                            db.SaveChanges();
                            trans.Commit();
                            return ActionTrueNotData(string.Empty);
                        }
                        return ActionFalseNotData(string.Empty);
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(string.Empty);
                    }
                }
            }
        }

        [HttpPost, ActionName("PostDM_DoiTuong")]
        [ResponseType(typeof(DM_DoiTuong))]
        public IHttpActionResult PostDM_DoiTuong([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                DM_DoiTuong objNew = data.ToObject<DM_DoiTuong>();

                string sMaDoiTuong = string.Empty;
                if (objNew.MaDoiTuong != null && objNew.MaDoiTuong != string.Empty)
                {
                    sMaDoiTuong = objNew.MaDoiTuong.ToUpper();
                    // check MaDoiTuong exist
                    var codeEx = classdoituong.Check_ExistCode(sMaDoiTuong, Guid.Empty);
                    if (codeEx)
                    {
                        return Json(new { res = false, mes = "Mã đối tượng đã tồn tại" });
                    }
                }
                else
                {
                    //mã khach hang tự động
                    switch (objNew.LoaiDoiTuong)
                    {
                        case 2:
                        case 3:
                        case 4:
                            sMaDoiTuong = classdoituong.SP_GetautoCode(objNew.LoaiDoiTuong);
                            break;
                        default:
                            sMaDoiTuong = classdoituong.GetMaDoiTuongMax_byTemp(objNew.LoaiDoiTuong, objNew.ID_DonVi);
                            break;
                    }
                }

                // check SDT
                bool checkEx = classdoituong.SP_CheckSoDienThoai_Exist(objNew.DienThoai);
                if (checkEx)
                {
                    return Json(new { res = false, mes = "Số điện thoại đã tồn tại" });
                }
                // check email
                checkEx = classdoituong.CheckEmail_Exist(objNew.Email);
                if (checkEx)
                {
                    return Json(new { res = false, mes = "Email đã tồn tại" });
                }

                #region DM_DoiTuong
                objNew.ID = Guid.NewGuid();
                objNew.MaDoiTuong = sMaDoiTuong;
                objNew.NgayTao = DateTime.Now;
                objNew.TheoDoi = false;
                objNew.TrangThai_TheGiaTri = 1;
                #endregion

                string strIns = classdoituong.Add_DoiTuong(objNew);
                if (strIns != string.Empty)
                {
                    return Json(new { res = false, mes = strIns });
                }
                return Json(new { res = true, data = new { objNew.ID, objNew.MaDoiTuong } });
            }
        }

        [HttpPost, ActionName("PostDM_DoiTuong_NhapTraHang")]
        [ResponseType(typeof(DM_DoiTuong))]
        public IHttpActionResult PostDM_DoiTuong_NhapTraHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                DM_DoiTuong objNew = data.ToObject<DM_DoiTuong>();

                string sMaDoiTuong = string.Empty;
                if (objNew.MaDoiTuong != null && objNew.MaDoiTuong != string.Empty)
                {
                    sMaDoiTuong = objNew.MaDoiTuong.ToUpper();
                }
                else
                {
                    //mã khach hang tự động
                    sMaDoiTuong = classdoituong.SP_GetautoCode(objNew.LoaiDoiTuong);
                }

                #region DM_DoiTuong
                DM_DoiTuong DM_DoiTuong = new DM_DoiTuong { };
                DM_DoiTuong.ID = Guid.NewGuid();
                DM_DoiTuong.MaDoiTuong = sMaDoiTuong;
                DM_DoiTuong.TenDoiTuong = objNew.TenDoiTuong;
                DM_DoiTuong.GioiTinhNam = objNew.GioiTinhNam;
                DM_DoiTuong.LoaiDoiTuong = objNew.LoaiDoiTuong;
                DM_DoiTuong.NgaySinh_NgayTLap = objNew.NgaySinh_NgayTLap;
                DM_DoiTuong.NgayTao = DateTime.Now;
                DM_DoiTuong.NguoiTao = objNew.NguoiTao;
                DM_DoiTuong.DiaChi = objNew.DiaChi;
                DM_DoiTuong.Email = objNew.Email;
                DM_DoiTuong.GhiChu = objNew.GhiChu;
                DM_DoiTuong.DienThoai = objNew.DienThoai;
                DM_DoiTuong.ID_NhomDoiTuong = objNew.ID_NhomDoiTuong;
                DM_DoiTuong.ID_QuanHuyen = objNew.ID_QuanHuyen;
                DM_DoiTuong.ID_TinhThanh = objNew.ID_TinhThanh;
                DM_DoiTuong.ID_DonVi = objNew.ID_DonVi;
                DM_DoiTuong.MaSoThue = objNew.MaSoThue;
                DM_DoiTuong.ID_NguonKhach = objNew.ID_NguonKhach;
                DM_DoiTuong.ID_NguoiGioiThieu = objNew.ID_NguoiGioiThieu;
                DM_DoiTuong.ID_NhanVienPhuTrach = objNew.ID_NhanVienPhuTrach;
                DM_DoiTuong.LaCaNhan = objNew.LaCaNhan;
                DM_DoiTuong.TenDoiTuong_ChuCaiDau = objNew.TenDoiTuong_ChuCaiDau;
                DM_DoiTuong.TenDoiTuong_KhongDau = objNew.TenDoiTuong_KhongDau;
                DM_DoiTuong.TheoDoi = false;
                DM_DoiTuong.DinhDang_NgaySinh = objNew.DinhDang_NgaySinh;

                #endregion

                string strIns = classdoituong.Add_DoiTuong(DM_DoiTuong);
                if (objNew.ID_NhomDoiTuong != null)
                {
                    DM_DoiTuong_Nhom dtn = new DM_DoiTuong_Nhom();
                    dtn.ID = Guid.NewGuid();
                    dtn.ID_DoiTuong = DM_DoiTuong.ID;
                    dtn.ID_NhomDoiTuong = objNew.ID_NhomDoiTuong.Value;
                    db.DM_DoiTuong_Nhom.Add(dtn);
                    db.SaveChanges();
                }
                if (db == null)
                {
                    return null;
                }
                else
                {
                    DM_DoiTuong objReturn = new DM_DoiTuong();
                    var objFind = db.DM_DoiTuong.Find(DM_DoiTuong.ID);

                    objReturn.ID = DM_DoiTuong.ID;
                    objReturn.MaDoiTuong = objFind.MaDoiTuong;
                    objReturn.TenDoiTuong = objFind.TenDoiTuong;
                    objReturn.GioiTinhNam = objFind.GioiTinhNam;
                    objReturn.LoaiDoiTuong = objFind.LoaiDoiTuong;
                    objReturn.NgaySinh_NgayTLap = objFind.NgaySinh_NgayTLap;
                    objReturn.NgayTao = objFind.NgayTao;
                    objReturn.NguoiTao = objFind.NguoiTao;
                    objReturn.DiaChi = objFind.DiaChi;
                    objReturn.Email = objFind.Email;
                    objReturn.GhiChu = objFind.GhiChu;
                    objReturn.DienThoai = objFind.DienThoai;
                    objReturn.ID_NhomDoiTuong = objFind.ID_NhomDoiTuong;
                    objReturn.ID_QuanHuyen = objFind.ID_QuanHuyen;
                    objReturn.ID_TinhThanh = objFind.ID_TinhThanh;
                    objReturn.ID_DonVi = objFind.ID_DonVi;
                    objReturn.MaSoThue = objFind.MaSoThue;
                    objReturn.TenNhomDT = objFind.DM_NhomDoiTuong != null ? objFind.DM_NhomDoiTuong.TenNhomDoiTuong : "Nhóm mặc định";
                    objReturn.ID_NguonKhach = objFind.ID_NguonKhach;
                    objReturn.ID_NguoiGioiThieu = objFind.ID_NguoiGioiThieu;
                    objReturn.ID_NhanVienPhuTrach = objFind.ID_NhanVienPhuTrach;
                    objReturn.LaCaNhan = objFind.LaCaNhan;
                    objReturn.KhuVuc = objFind.DM_TinhThanh != null ? objFind.DM_TinhThanh.TenTinhThanh : "";
                    objReturn.PhuongXa = objFind.DM_QuanHuyen != null ? objFind.DM_QuanHuyen.TenQuanHuyen : "";

                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult ImageUpload(Guid id, string rootFolder, string subFolder)
        {

            // path file: rootFoler/subDomain/MaDoiTuong/filename
            var path = "";
            var parentFolder = CookieStore.GetCookieAes("SubDomain");
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_DoiTuong_Anh classDoiTuongAnh = new ClassDM_DoiTuong_Anh(db);
                    var classdoituong = new classDM_DoiTuong(db);
                    var count = classDoiTuongAnh.Gets(p => p.ID_DoiTuong == id).Count();

                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Current.Request.Files[i];
                        var fileName = file.FileName;

                        var mapPath = HttpContext.Current.Server.MapPath("~/" + rootFolder + "/" + parentFolder + "/" + subFolder);
                        if (!Directory.Exists(mapPath))
                        {
                            Directory.CreateDirectory(mapPath);
                        }

                        path = Path.Combine(mapPath, fileName);
                        file.SaveAs(path);

                        var result = "/" + rootFolder + "/" + parentFolder + "/" + subFolder + "/" + fileName;

                        //Add db table Anh
                        DM_DoiTuong_Anh objAnh = new DM_DoiTuong_Anh();
                        objAnh.ID = Guid.NewGuid();
                        objAnh.ID_DoiTuong = id;
                        objAnh.SoThuTu = count + 1;
                        objAnh.URLAnh = result;
                        classDoiTuongAnh.Add_Image(objAnh);
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, path));
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_DoiTuongAPI_ImageUpload: " + ex.InnerException + ex.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, path));
            }
        }

        [HttpPost]
        public IHttpActionResult ImageUpload(Guid id, string pathFolder)
        {
            // path file: ImageUpload/AnhKhachHang/MaDoiTuong/filename
            var pathFile = "";

            try
            {
                string fullPathFolder = "ImageUpload/" + pathFolder;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    ClassDM_DoiTuong_Anh classDoiTuongAnh = new ClassDM_DoiTuong_Anh(db);

                    var count = classDoiTuongAnh.Gets(p => p.ID_DoiTuong == id).Count();
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Current.Request.Files[i];
                        var fileName = file.FileName;

                        var mapPath = HttpContext.Current.Server.MapPath("~/" + fullPathFolder);
                        if (!Directory.Exists(mapPath))
                        {
                            Directory.CreateDirectory(mapPath);
                        }

                        pathFile = Path.Combine(mapPath, fileName);
                        file.SaveAs(pathFile);

                        //Add db table Anh
                        DM_DoiTuong_Anh objAnh = new DM_DoiTuong_Anh();
                        objAnh.ID = Guid.NewGuid();
                        objAnh.ID_DoiTuong = id;
                        objAnh.SoThuTu = count + 1;
                        objAnh.URLAnh = "/" + fullPathFolder + "/" + fileName;
                        classDoiTuongAnh.Add_Image(objAnh);
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, pathFile));
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_DoiTuongAPI_ImageUpload1:" + ex.InnerException + ex.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, pathFile));
            }
        }

        /// <summary>
        /// use at gara: 
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        [HttpPost, HttpGet]
        public IHttpActionResult ImageUpload(string rootFolder, string subFolder = null)
        {
            // path file: ImageUpload/SubDomain/rootFolder/subFolder
            // ex: ImageUpload/0973474985/LogoHangXe/a.jpg
            // or: .../PhieuTiepNhan/MaPhieuTiepNhan/b.png
            var pathFile = "";
            var subDomain = CookieStore.GetCookieAes("SubDomain");
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Current.Request.Files[i];
                        var fileName = file.FileName;

                        var mapPath = string.Empty;
                        var pathFolder = string.Empty;
                        if (string.IsNullOrEmpty(subFolder))
                        {
                            pathFolder = string.Concat("~/ImageUpload/", subDomain, "/", rootFolder);
                            mapPath = HttpContext.Current.Server.MapPath(pathFolder);
                        }
                        else
                        {
                            pathFolder = string.Concat("~/ImageUpload/", subDomain, "/", rootFolder, "/", subFolder);
                            mapPath = HttpContext.Current.Server.MapPath(pathFolder);
                        }
                        if (!Directory.Exists(mapPath))
                        {
                            Directory.CreateDirectory(mapPath);
                        }
                        // rename file name
                        //string extension = Path.GetExtension(file.FileName);
                        //fileName = 
                        pathFile = Path.Combine(mapPath, fileName);
                        file.SaveAs(pathFile);
                    }
                    return ActionTrueData(pathFile);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString() + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateAnhDoiTuong([FromBody] List<string> files, Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_DoiTuong_Anh classdoituong = new ClassDM_DoiTuong_Anh(db);
                    var count = classdoituong.Gets(p => p.ID_DoiTuong == id).Count();
                    List<DM_DoiTuong_Anh> lst = new List<DM_DoiTuong_Anh>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        //Add db table Anh
                        DM_DoiTuong_Anh objAnh = new DM_DoiTuong_Anh();
                        objAnh.ID = Guid.NewGuid();
                        objAnh.ID_DoiTuong = id;
                        objAnh.SoThuTu = count + 1 + i;
                        objAnh.URLAnh = files[i];
                        lst.Add(objAnh);
                    }
                    db.DM_DoiTuong_Anh.AddRange(lst);
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return Ok("");
        }

        [HttpPost, HttpGet]
        public IHttpActionResult DeleteFile_inFolder([FromBody] JObject data)
        {
            try
            {
                List<string> lstFile = data["lstFile"].ToObject<List<string>>();
                if (lstFile != null && lstFile.Count > 0)
                {
                    foreach (var item in lstFile)
                    {

                        var pathFile = item;
                        string[] arrFolder = pathFile.Split('/');
                        var fileName = arrFolder[arrFolder.Length - 1]; // get element last in array
                        var pathFoler = pathFile.Substring(0, pathFile.Length - fileName.Length - 1);

                        System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(pathFoler));
                        if (Directory.Exists(HttpContext.Current.Server.MapPath(pathFoler)))
                        {
                            foreach (FileInfo file in di.GetFiles())
                            {
                                if (file.Name == fileName)
                                {
                                    file.Delete();
                                    break;
                                }
                            }
                        }
                    }
                }
                return ActionTrueNotData(string.Empty);
            }
            catch (Exception e)
            {
                return ActionFalseNotData(e.InnerException + e.Message);
            }
        }

        [ResponseType(typeof(string))]
        [HttpDelete]
        public string DeleteDM_DoiTuong_Anh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    DM_DoiTuong_Anh lst = db.DM_DoiTuong_Anh.Find(id);
                    if (lst != null)
                    {
                        // delete file in folder
                        var pathFile = lst.URLAnh;
                        string[] arrFolder = pathFile.Split('/');
                        var fileName = arrFolder[arrFolder.Length - 1]; // get element last in array
                        var pathFoler = pathFile.Substring(0, pathFile.Length - fileName.Length - 1);

                        System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath(pathFoler));
                        if (Directory.Exists(HttpContext.Current.Server.MapPath(pathFoler)))
                        {
                            foreach (FileInfo file in di.GetFiles())
                            {
                                if (file.Name == fileName)
                                {
                                    file.Delete();
                                    break;
                                }
                            }
                        }

                        db.DM_DoiTuong_Anh.Remove(lst);
                        db.SaveChanges();
                        List<DM_DoiTuong_Anh> lstanh = db.DM_DoiTuong_Anh.Where(p => p.ID_DoiTuong == lst.ID_DoiTuong).Where(p => p.SoThuTu > lst.SoThuTu).ToList();
                        foreach (var item in lstanh)
                        {
                            item.SoThuTu = item.SoThuTu - 1;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }

        #region import khách 
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortExcelToCustomer(Guid ID_DonVi, Guid ID_NhanVien, int LoaiUpdate, string RownError = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassNPOIExcel classNPOI = new ClassNPOIExcel();
                List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        var file = HttpContext.Current.Request.Files[0];
                        using (System.IO.Stream inputStream = file.InputStream)
                        {
                            XSSFWorkbook workbook = new XSSFWorkbook(inputStream);
                            ISheet sheet = workbook.GetSheetAt(0);

                            string str = classNPOI.CheckFileMau(sheet, "MẪU FILE IMPORT DANH MỤC KHÁCH HÀNG", 3);
                            if (string.IsNullOrEmpty(str))
                            {
                                if (!string.IsNullOrEmpty(RownError))
                                {
                                    // nếu có lỗi, và vẫn muốn import (bỏ qua dòng lỗi)
                                    lstErr = classNPOI.ImportDangMucKhachHang_toDB(sheet, ID_DonVi, ID_NhanVien, LoaiUpdate, RownError);
                                }
                                else
                                {
                                    lstErr = classNPOI.CheckData_FileImportCustomer(sheet);
                                    if (lstErr.Count == 0)
                                    {
                                        lstErr = classNPOI.ImportDangMucKhachHang_toDB(sheet, ID_DonVi, ID_NhanVien, LoaiUpdate);
                                    }
                                }
                            }
                            else
                            {
                                lstErr.Add(new ErrorDMHangHoa()
                                {
                                    TenTruongDuLieu = str,
                                    ViTri = "0",
                                    rowError = -1,
                                    loaiError = 1,
                                    ThuocTinh = str,
                                    DienGiai = str,
                                });
                            }
                        }
                    }
                    else
                    {
                        lstErr.Add(new ErrorDMHangHoa()
                        {
                            TenTruongDuLieu = "Không tồn tại file",
                            ViTri = "0",
                            rowError = -1,
                            loaiError = 1,
                            ThuocTinh = "Không tồn tại file",
                            DienGiai = "Không tồn tại file",
                        });
                    }
                }
                catch (Exception ex)
                {
                    lstErr.Add(new ErrorDMHangHoa()
                    {
                        TenTruongDuLieu = "Exception",
                        ViTri = "0",
                        rowError = -1,
                        loaiError = 1,
                        ThuocTinh = "Exception",
                        DienGiai = ex.InnerException + ex.Message,
                    });
                }
                if (lstErr != null && lstErr.Count() > 0)
                {
                    return ActionFalseWithData(lstErr);
                }
                else
                {
                    return ActionTrueData(lstErr);
                }
            }
        }


        [HttpPost]
        public IHttpActionResult ImportExcelToKhachHang(Guid ID_NhanVien, Guid ID_DonVi)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);

                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = classOffice.CheckFileMauKhachHang(excelstream);

                            if (str == null)
                            {
                                lstErr = classOffice.checkfileKhachHang(excelstream, ID_NhanVien, ID_DonVi);
                                if (lstErr == null)
                                {
                                    return Json(new { res = true });
                                }
                                else
                                {
                                    return Json(new { res = false, mes = "", data = lstErr });
                                }
                            }
                            else
                            {
                                return Json(new { res = false, mes = str, data = "null" });
                            }
                        }
                    }
                    return Json(new { res = false, mes = "Trang tính không có dữ liệu", data = "null" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException, data = "null" });
            }
        }
        [HttpPost]
        public IHttpActionResult ImportKhachHang_WithError(Guid ID_NhanVien, Guid ID_DonVi)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    string ListError = HttpContext.Current.Request.Form["ListErr"];

                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            classOffice.IgnoreErrorKhachHang(excelstream, ListError, ID_NhanVien, ID_DonVi);
                        }
                        return Json(new { res = true });
                    }
                    return Json(new { res = false, mes = "Trang tính không có dữ liệu" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException });
            }
        }
        [HttpPost]
        public IHttpActionResult ImportExcelToNhaCungCap(Guid ID_NhanVien, Guid ID_DonVi)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);

                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = classOffice.CheckFileMauNhaCungCap(excelstream);
                            if (str == null)
                            {
                                lstErr = classOffice.checkfileNhaCungCap(excelstream, ID_NhanVien, ID_DonVi);
                                if (lstErr == null)
                                {
                                    return Json(new { res = true });
                                }
                                else
                                {
                                    return Json(new { res = false, mes = "", data = lstErr });
                                }
                            }
                            else
                            {
                                return Json(new { res = false, mes = str, data = "null" });
                            }
                        }
                    }
                    return Json(new { res = false, mes = "Trang tính không có dữ liệu", data = "null" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException, data = "null" });
            }
        }
        [HttpPost]
        public IHttpActionResult ImportNhaCungCap_WithError(Guid ID_NhanVien, Guid ID_DonVi)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    string ListError = HttpContext.Current.Request.Form["ListErr"];
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            classOffice.IgnoreErrorNhaCungCap(excelstream, ListError, ID_NhanVien, ID_DonVi);
                        }
                        return Json(new { res = true });
                    }
                    return Json(new { res = false, mes = "Trang tính không có dữ liệu" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException });
            }
        }

        [HttpPost]
        public IHttpActionResult ImportExcelBaoHiem(Guid ID_NhanVien, Guid ID_DonVi, bool Continue, bool Update)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);

                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = classOffice.CheckFileMauBaoHiem(excelstream);
                            if (str == null)
                            {
                                lstErr = classOffice.checkfileBaoHiem(excelstream, ID_NhanVien, ID_DonVi, Continue, Update);
                                if (lstErr == null)
                                {
                                    return Json(new { res = true });
                                }
                                else
                                {
                                    return Json(new { res = false, mes = "", data = lstErr });
                                }
                            }
                            else
                            {
                                return Json(new { res = false, mes = str, data = "null" });
                            }
                        }
                    }
                    return Json(new { res = false, mes = "Trang tính không có dữ liệu", data = "null" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException, data = "null" });
            }
        }
        #endregion

        #region delete
        // DELETE: api/DM_DoiTuongAPI/5
        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult DeleteDM_DoiTuong(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                string strDel = classdoituong.UpdateTheoDoi_DoiTuong(id);

                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }
        }

        [HttpGet, HttpPost]
        public string Delete_ManyDoiTuong(string ids)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    db.Database.ExecuteSqlCommand(" Update DM_DoiTuong set TheoDoi= '1' where ID in (select Name from dbo.splitstring ('" + ids + "'))");
                    return string.Empty;
                }
                catch (Exception e)
                {
                    return string.Concat(e.InnerException, e.Message);
                }
            }
        }
        #endregion;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public IHttpActionResult GetListTinhThanh()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    var data = classdoituong.GetListTinhThanh(null).Select(x => new { x.ID, x.MaTinhThanh, x.TenTinhThanh, x.ID_VungMien }).ToList();
                    return Json(new { res = true, data = data });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = string.Concat("GetListTinhThanh", e.InnerException, e.Message) });
            }
        }
        [HttpGet]
        public IHttpActionResult GetListQuanHuyen(Guid idTinhThanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                var data = classdoituong.GetListQuanHuyen(id => id.ID_TinhThanh == idTinhThanh).Select(x => new { x.ID, x.TenQuanHuyen, x.ID_TinhThanh, x.MaQuanHuyen });
                return Json(new { data });
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetAllQuanHuyen()
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
                        var data = (from x in db.DM_QuanHuyen
                                    select new
                                    {
                                        x.ID,
                                        x.MaQuanHuyen,
                                        x.TenQuanHuyen,
                                        x.ID_TinhThanh
                                    }).ToList();
                        return Json(new { res = true, data = data });
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("DM_DoiTuongAPI_GetAllQuanHuyen: " + ex.Message + ex.InnerException);
                        return Json(new { res = false });
                    }
                }
            }
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<Object> SearchDistrict(string txtsearch)
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
                        var data = (from x in db.DM_QuanHuyen
                                    select new
                                    {
                                        x.ID,
                                        x.TenQuanHuyen,
                                        x.ID_TinhThanh
                                    }).ToList();
                        data = data.Where(x => CommonStatic.ConvertToUnSign(x.TenQuanHuyen).ToLower().Contains(txtsearch)).ToList();
                        return data.Select(p => new
                        {
                            label = p.TenQuanHuyen,
                            value = p.TenQuanHuyen,
                            actual = p.ID,
                            data = p
                        });
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("DM_DoiTuongAPI_GetAllQuanHuyen: " + ex.Message + ex.InnerException);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region DM_DoiTuong_Nhom

        #region "Insert DM_DoiTuong_Nhom"
        [HttpPost]
        public IHttpActionResult PostDM_DoiTuong_Nhom([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var sError = string.Empty;
                        var classdoituongnhom = new ClassDM_DoiTuong_Nhom(db);
                        List<DM_DoiTuong_Nhom> lstObj = data["lstDM_DoiTuong_Nhom"].ToObject<List<DM_DoiTuong_Nhom>>();
                        foreach (var item in lstObj)
                        {
                            DM_DoiTuong_Nhom objAdd = new DM_DoiTuong_Nhom
                            {
                                ID = Guid.NewGuid(),
                                ID_DoiTuong = item.ID_DoiTuong,
                                ID_NhomDoiTuong = item.ID_NhomDoiTuong,
                            };
                            sError += classdoituongnhom.Add_DM_DoiTuong_Nhom(objAdd);
                        }
                        trans.Commit();
                        return ActionTrueData(data);
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

        #region "Update DM_DoiTuong_Nhom"
        [System.Web.Http.AcceptVerbs("GET", "PUT", "POST")]
        [HttpPut]
        public IHttpActionResult PutDM_DoiTuong_Nhom([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // delete && add again
                        var classDoiTuongNhom = new ClassDM_DoiTuong_Nhom(db);
                        List<DM_DoiTuong_Nhom> lstObj = data["lstDM_DoiTuong_Nhom"].ToObject<List<DM_DoiTuong_Nhom>>();
                        List<Guid> lstID = lstObj.Select(x => x.ID_DoiTuong).Distinct().ToList();
                        classDoiTuongNhom.Delete_DM_DoiTuong_Nhom(lstID);

                        foreach (var item in lstObj)
                        {
                            DM_DoiTuong_Nhom objAdd = new DM_DoiTuong_Nhom
                            {
                                ID = Guid.NewGuid(),
                                ID_DoiTuong = item.ID_DoiTuong,
                                ID_NhomDoiTuong = item.ID_NhomDoiTuong,
                            };
                            classDoiTuongNhom.Add_DM_DoiTuong_Nhom(objAdd);
                        }
                        trans.Commit();
                        return ActionTrueData(data);
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

        #region "Delete"
        [HttpPut]
        public IHttpActionResult DeleteNhom_ofDoiTuong(Guid idDoiTuong, Guid idNhom)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classDoiTuongNhom = new ClassDM_DoiTuong_Nhom(db);
                var sErrDelete = classDoiTuongNhom.DeleteNhom_ofDoiTuong(idDoiTuong, idNhom);
                if (sErrDelete == string.Empty)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, sErrDelete));
                }
            }
        }

        [HttpPut, HttpGet, HttpPost]
        public IHttpActionResult DeleteAllNhom_ofDoiTuong(List<Guid> lstIDDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var classDoiTuongNhom = new ClassDM_DoiTuong_Nhom(db);
                        classDoiTuongNhom.Delete_DM_DoiTuong_Nhom(lstIDDoiTuong);
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
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
        #endregion

        #region "DM_DoiTuong_TrangThai"
        [HttpPost]
        public IHttpActionResult PostDM_DoiTuong_TrangThai([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                var sError = string.Empty;
                try
                {
                    DM_DoiTuong_TrangThai obj = data.ToObject<DM_DoiTuong_TrangThai>();
                    DM_DoiTuong_TrangThai objAdd = new DM_DoiTuong_TrangThai
                    {
                        ID = Guid.NewGuid(),
                        TenTrangThai = obj.TenTrangThai,
                        NguoiTao = obj.NguoiTao,
                        NgayTao = obj.NgayTao,
                    };
                    db.DM_DoiTuong_TrangThai.Add(objAdd);
                    db.SaveChanges();

                    return CreatedAtRoute("DefaultApi", new { id = objAdd.ID }, objAdd);
                }
                catch (Exception ex)
                {
                    sError = ex.InnerException + ex.Message;
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, sError));
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "PUT", "POST")]
        public string PutDM_DoiTuong_TrangThai([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                //var classdoituong = new classDM_DoiTuong(db);
                var sError = string.Empty;
                try
                {
                    DM_DoiTuong_TrangThai obj = data.ToObject<DM_DoiTuong_TrangThai>();
                    var objUpd = db.DM_DoiTuong_TrangThai.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TenTrangThai = obj.TenTrangThai;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = DateTime.Now;
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    sError = ex.InnerException + ex.Message;
                }
                return sError;
            }
        }

        [HttpPut]
        public string Delete_DoiTuong_TrangThai(Guid idTrangThai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                var sError = string.Empty;
                // get all DM_DoiTuong have this TrangThai and update ID_TrangThai = null
                var lstDoiTuong = classdoituong.Gets(x => x.ID_TrangThai == idTrangThai);
                if (lstDoiTuong != null && lstDoiTuong.Count() > 0)
                {
                    try
                    {
                        SqlParameter param = new SqlParameter("ID_TrangThai", idTrangThai);
                        db.Database.ExecuteSqlCommand("SP_UpdateTrangThaiKhachHang @ID_TrangThai", param);

                        var objUpd = db.DM_DoiTuong_TrangThai.Find(idTrangThai);
                        db.DM_DoiTuong_TrangThai.Remove(objUpd);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        sError = ex.InnerException + ex.Message;
                    }
                }
                return sError;
            }
        }
        #endregion

        #region Bảo hiểm
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListBaoHiem_v1([FromBody] JObject objIn)
        {
            try
            {
                ParamGetListBaoHiem_v1 param = new ParamGetListBaoHiem_v1();
                if (objIn["IdChiNhanhs"] != null)
                    param.IdChiNhanhs = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["NgayTaoFrom"] != null && objIn["NgayTaoFrom"].ToObject<string>() != "")
                    param.NgayTaoFrom = objIn["NgayTaoFrom"].ToObject<DateTime>();
                if (objIn["NgayTaoTo"] != null && objIn["NgayTaoTo"].ToObject<string>() != "")
                    param.NgayTaoTo = objIn["NgayTaoTo"].ToObject<DateTime>();
                if (objIn["TongBanDateFrom"] != null && objIn["TongBanDateFrom"].ToObject<string>() != "")
                    param.TongBanDateFrom = objIn["TongBanDateFrom"].ToObject<DateTime>();
                if (objIn["TongBanDateTo"] != null && objIn["TongBanDateTo"].ToObject<string>() != "")
                    param.TongBanDateTo = objIn["TongBanDateTo"].ToObject<DateTime>();
                if (objIn["TongBanFrom"] != null && objIn["TongBanFrom"].ToObject<string>() != "")
                    param.TongBanFrom = objIn["TongBanFrom"].ToObject<double>();
                if (objIn["TongBanTo"] != null && objIn["TongBanTo"].ToObject<string>() != "")
                    param.TongBanTo = objIn["TongBanTo"].ToObject<double>();
                if (objIn["NoFrom"] != null && objIn["NoFrom"].ToObject<string>() != "")
                    param.NoFrom = objIn["NoFrom"].ToObject<double>();
                if (objIn["NoTo"] != null && objIn["NoTo"].ToObject<string>() != "")
                    param.NoTo = objIn["NoTo"].ToObject<double>();
                if (objIn["TrangThais"] != null)
                {
                    string strTrangThai = "";
                    List<int> lstTrangThais = objIn["TrangThais"].ToObject<List<int>>();
                    if (lstTrangThais.Count > 0)
                    {
                        strTrangThai = string.Join(",", lstTrangThais);
                    }
                    param.TrangThais = strTrangThai;
                }
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classDM_DoiTuong doituong = new classDM_DoiTuong(db);
                    List<GetListBaoHiem_v1> databaohiem = doituong.GetListBaoHiem_v1(param);
                    int count = 0;
                    if (databaohiem.Count != 0)
                    {
                        count = databaohiem[0].TotalRow;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = databaohiem,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + databaohiem.Count) + " trên tổng số " + count + " bản ghi",
                        NumberOfPage = page
                    });
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ExportExcel_GetListBaoHiem_v1([FromBody] JObject objIn)
        {
            string fileSave = string.Empty;
            try
            {
                ParamGetListBaoHiem_v1 param = new ParamGetListBaoHiem_v1();
                if (objIn["IdChiNhanhs"] != null)
                    param.IdChiNhanhs = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["NgayTaoFrom"] != null && objIn["NgayTaoFrom"].ToObject<string>() != "")
                    param.NgayTaoFrom = objIn["NgayTaoFrom"].ToObject<DateTime>();
                if (objIn["NgayTaoTo"] != null && objIn["NgayTaoTo"].ToObject<string>() != "")
                    param.NgayTaoTo = objIn["NgayTaoTo"].ToObject<DateTime>();
                if (objIn["TongBanDateFrom"] != null && objIn["TongBanDateFrom"].ToObject<string>() != "")
                    param.TongBanDateFrom = objIn["TongBanDateFrom"].ToObject<DateTime>();
                if (objIn["TongBanDateTo"] != null && objIn["TongBanDateTo"].ToObject<string>() != "")
                    param.TongBanDateTo = objIn["TongBanDateTo"].ToObject<DateTime>();
                if (objIn["TongBanFrom"] != null && objIn["TongBanFrom"].ToObject<string>() != "")
                    param.TongBanFrom = objIn["TongBanFrom"].ToObject<double>();
                if (objIn["TongBanTo"] != null && objIn["TongBanTo"].ToObject<string>() != "")
                    param.TongBanTo = objIn["TongBanTo"].ToObject<double>();
                if (objIn["NoFrom"] != null && objIn["NoFrom"].ToObject<string>() != "")
                    param.NoFrom = objIn["NoFrom"].ToObject<double>();
                if (objIn["NoTo"] != null && objIn["NoTo"].ToObject<string>() != "")
                    param.NoTo = objIn["NoTo"].ToObject<double>();
                if (objIn["TrangThais"] != null)
                {
                    string strTrangThai = "";
                    List<int> lstTrangThais = objIn["TrangThais"].ToObject<List<int>>();
                    if (lstTrangThais.Count > 0)
                    {
                        strTrangThai = string.Join(",", lstTrangThais);
                    }
                    param.TrangThais = strTrangThai;
                }
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                param.PageSize = 0;
                List<int> lstColHide = new List<int>();
                if (objIn["ColHide"] != null)
                    lstColHide = objIn["ColHide"].ToObject<List<int>>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classDM_DoiTuong doituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    List<GetListBaoHiem_v1> databaohiem = doituong.GetListBaoHiem_v1(param);
                    List<GetListBaoHiem_v1_Export> lstExport = databaohiem.Select(p => new GetListBaoHiem_v1_Export
                    {
                        MaBaoHiem = p.MaDoiTuong,
                        TenbaoHiem = p.TenDoiTuong,
                        MaSoThue = p.MaSoThue,
                        DienThoai = p.DienThoai,
                        Email = p.Email,
                        DiaChi = p.DiaChi,
                        QuanHuyen = p.TenQuanHuyen,
                        TinhThanh = p.TenTinhThanh,
                        NoHienTai = p.NoHienTai,
                        TongChiPhiBaoHiem = p.TongTienBaoHiem,
                        GhiChu = p.GhiChu
                    }).ToList();
                    DataTable excel = classOffice.ToDataTable<GetListBaoHiem_v1_Export>(lstExport);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_DanhSachBaoHiem.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachBaoHiem.xlsx");
                    fileSave = classOffice.createFolder_Download(fileSave);
                    classOffice.listToOfficeExcel_v2(fileTeamplate, fileSave, excel, 3, 100, 97, false, lstColHide);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return ActionTrueNotData(fileSave);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ExportExcel_GetHoaDon_SoQuy_ofDoiTuong([FromBody] JObject objIn)
        {
            try
            {
                Guid idDoiTuong = Guid.Empty;
                if (objIn["IdDoiTuong"] != null)
                    idDoiTuong = objIn["IdDoiTuong"].ToObject<Guid>();
                string idChiNhanh = null;
                if (objIn["IdChiNhanhs"] != null)
                    idChiNhanh = objIn["IdChiNhanhs"].ToObject<string>();
                string MaDoiTuong = "";
                if (objIn["MaDoiTuong"] != null)
                    MaDoiTuong = objIn["MaDoiTuong"].ToObject<string>();
                string TenDoiTuong = "";
                if (objIn["TenDoiTuong"] != null)
                    TenDoiTuong = objIn["TenDoiTuong"].ToObject<string>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classhoadon = new ClassBH_HoaDon(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_BaoHiem_CongNo.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/BaoHiem_CongNo_" + MaDoiTuong + ".xlsx");
                    var data = classhoadon.SP_GetHoaDonandSoQuy_FromIDDoiTuong(idDoiTuong, idChiNhanh);
                    if (data != null)
                    {

                        // order by NgayLap DESC, LoaiHoaDon ASC --> display: SoQuy, HoaDon
                        data = data.OrderByDescending(hd => hd.NgayLapHoaDon).ThenByDescending(x => x.MaHoaDon).ToList();

                        var ss3 = data.Select(x => new NhatKyTichDiem
                        {
                            MaHoaDon = x.MaHoaDon,
                            NgayLapHoaDon = x.NgayLapHoaDon,
                            SLoaiHoaDon = x.strLoaiHoaDon,
                            GiaTri = x.PhaiThanhToan ?? 0,
                            DiemGiaoDich = x.DuNoKH ?? 0,
                        });

                        DataTable excel_QuyHD = classOffice.ToDataTable<NhatKyTichDiem>(ss3.ToList());
                        excel_QuyHD.Columns.Remove("DiemSauGD");
                        fileSave = classOffice.createFolder_Download(fileSave);
                        classOffice.listToOfficeExcel_Sheet_KH(fileTeamplate, fileSave, excel_QuyHD, 6, 25, 19, true, 0, null, MaDoiTuong, TenDoiTuong);
                        var index = fileSave.IndexOf(@"\Template");
                        fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                        fileSave = fileSave.Replace(@"\", "/");
                        return ActionTrueNotData(fileSave);
                    }
                    else
                    {
                        return ActionFalseNotData("Không có dữ liệu");
                    }
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("Export excel DM_DoiTuongAPI_GetHoaDon_SoQuy_ofDoiTuong: " + ex.Message + ex.InnerException);
                return ActionFalseNotData(ex.ToString());
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult CheckTenDoiTuong([FromBody] JObject objIn)
        {
            try
            {
                string TenDoiTuong = "";
                if (objIn["TenDoiTuong"] != null && objIn["TenDoiTuong"].ToObject<string>() != "")
                {
                    TenDoiTuong = objIn["TenDoiTuong"].ToObject<string>();
                }
                Guid IdDoiTuong = Guid.Empty;
                if (objIn["IdDoiTuong"] != null && objIn["IdDoiTuong"].ToObject<string>() != "")
                {
                    IdDoiTuong = objIn["IdDoiTuong"].ToObject<Guid>();
                }
                int LoaiDoiTuong = -1;
                if (objIn["LoaiDoiTuong"] != null && objIn["LoaiDoiTuong"].ToObject<string>() != "")
                {
                    LoaiDoiTuong = objIn["LoaiDoiTuong"].ToObject<int>();
                }
                bool checkExist = false;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (IdDoiTuong != Guid.Empty)
                    {
                        checkExist = db.DM_DoiTuong.Where(p => p.LoaiDoiTuong == LoaiDoiTuong && p.ID != IdDoiTuong && p.TenDoiTuong.ToLower() == TenDoiTuong.ToLower()).FirstOrDefault() != null;
                    }
                    else
                    {
                        checkExist = db.DM_DoiTuong.Where(p => p.LoaiDoiTuong == LoaiDoiTuong && p.TenDoiTuong.ToLower() == TenDoiTuong.ToLower()).FirstOrDefault() != null;
                    }
                }
                string message = "";
                if (checkExist)
                {
                    if (LoaiDoiTuong == 1)
                    {
                        message = "Tên khách hàng đã tồn tại";
                    }
                    else if (LoaiDoiTuong == 2)
                    {
                        message = "Tên nhà cung cấp đã tồn tại";
                    }
                    else if (LoaiDoiTuong == 3)
                    {
                        message = "Tên bảo hiểm đã tồn tại";
                    }
                    else
                    {
                        message = "Tên đối tượng đã tồn tại";
                    }
                    return ActionFalseNotData(message);
                }
                return ActionTrueNotData("");
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }
        #endregion
    }
}
