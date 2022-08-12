using libGara;
using libQuy_HoaDon;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using libReport;
using AsposeCellsDocument;
using System.IO;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class GaraAPIController : BaseApiController
    {
        #region GET
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetAllHangXes()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHangXe classHangXe = new ClassHangXe(db);
                List<HangXe> data = classHangXe.GetListHangXes();
                return ActionTrueData(data);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetAllLoaiXes()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassLoaiXe classLoaiXe = new ClassLoaiXe(db);
                    List<LoaiXe> data = classLoaiXe.GetListLoaiXes();
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetAllDanhMucMauXe()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDanhMucMauXe classMauXe = new ClassDanhMucMauXe(db);
                    List<MauXe> data = classMauXe.GetListMauXes();
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [HttpGet]
        public IHttpActionResult GetInforCar_ByID(Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassXe classXe = new ClassXe(db);
                    List<Xe> data = classXe.GetInforCar_ByID(id);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [HttpGet]
        public IHttpActionResult JqAuto_SearchXe(string txt = null, string statusTN = null,
            string idCustomer = null, int? laHangHoa = null, int? nguoisohuu = null)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassXe classMauXe = new ClassXe(db);
                    List<Xe> data = classMauXe.JqAuto_SearchXe(txt, statusTN, idCustomer, laHangHoa, nguoisohuu);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }
        #region Phieu tiep nhan
        [HttpPost]
        public IHttpActionResult PTN_CheckChangeCus(Gara_PhieuTiepNhan paramNew)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPTN = new ClassPhieuTiepNhan(db);
                    var data = classPTN.PTN_CheckChangeCus(paramNew);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult ChangePTN_updateCus([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Gara_PhieuTiepNhan ptnOld = data["objPhieuTN"].ToObject<Gara_PhieuTiepNhan>();
                        List<int> arrType = data["arrType"].ToObject<List<int>>();
                        ClassPhieuTiepNhan classPTN = new ClassPhieuTiepNhan(db);
                        classPTN.ChangePTN_updateCus(ptnOld, arrType);
                        trans.Commit();
                        return ActionTrueData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GDV_CheckXeDangTiepNhan(Guid idXe, Guid idDonVi)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var data = db.Gara_PhieuTiepNhan.Where(x => x.ID_Xe == idXe && x.ID_DonVi == idDonVi && (x.TrangThai == 1 || x.TrangThai == 2)).Select(x => new { x.ID, x.MaPhieuTiepNhan }).ToList();
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.InnerException + ex.Message);
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetInforCustomer_byIDXe(Guid idXe)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var data = (from xe in db.Gara_DanhMucXe
                                join cus in db.DM_DoiTuong on xe.ID_KhachHang equals cus.ID
                                where xe.ID == idXe
                                select new
                                {
                                    cus.ID,
                                    cus.MaDoiTuong,
                                    cus.TenDoiTuong,
                                    cus.DienThoai,
                                    cus.Email,
                                    cus.DiaChi,
                                }).ToList();
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }
        [HttpGet, HttpPost]
        public IHttpActionResult JqAuto_PhieuTiepNhan(ParamSearch param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                    List<GetListPhieuTiepNhan_v2> data = classPhieuTN.JqAuto_PhieuTiepNhan(param);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListPhieuTiepNhan_v2([FromBody] JObject objIn)
        {
            try
            {
                ParamGetListPhieuTiepNhan_v2 param = new ParamGetListPhieuTiepNhan_v2();
                if (objIn["IdChiNhanhs"] != null)
                    param.IdChiNhanhs = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["NgayTiepNhanFrom"] != null && objIn["NgayTiepNhanFrom"].ToObject<string>() != "")
                    param.NgayTiepNhan_From = objIn["NgayTiepNhanFrom"].ToObject<DateTime>();
                if (objIn["NgayTiepNhanTo"] != null && objIn["NgayTiepNhanTo"].ToObject<string>() != "")
                    param.NgayTiepNhan_To = objIn["NgayTiepNhanTo"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongDuKienFrom"] != null && objIn["NgayXuatXuongDuKienFrom"].ToObject<string>() != "")
                    param.NgayXuatXuongDuKien_From = objIn["NgayXuatXuongDuKienFrom"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongDuKienTo"] != null && objIn["NgayXuatXuongDuKienTo"].ToObject<string>() != "")
                    param.NgayXuatXuongDuKien_To = objIn["NgayXuatXuongDuKienTo"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongFrom"] != null && objIn["NgayXuatXuongFrom"].ToObject<string>() != "")
                    param.NgayXuatXuong_From = objIn["NgayXuatXuongFrom"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongTo"] != null && objIn["NgayXuatXuongTo"].ToObject<string>() != "")
                    param.NgayXuatXuong_To = objIn["NgayXuatXuongTo"].ToObject<DateTime>();
                if (objIn["TrangThais"] != null)
                    param.TrangThais = objIn["TrangThais"].ToObject<List<int>>();
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                if (objIn["BaoHiemFilter"] != null)
                    param.BaoHiem = objIn["BaoHiemFilter"].ToObject<int>();

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<GetListPhieuTiepNhan_v2> dataPhieuTiepNhan = classPhieuTiepNhan.GetListPhieuTiepNhan_v2(param);
                    int count = 0;
                    if (dataPhieuTiepNhan.Count != 0)
                    {
                        count = dataPhieuTiepNhan[0].TotalRow;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = dataPhieuTiepNhan,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + dataPhieuTiepNhan.Count) + " trên tổng số " + count + " bản ghi",
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
        public IHttpActionResult GetListGaraDanhMucXe_v1([FromBody] JObject objIn)
        {
            try
            {
                ParamGetListGaraDanhMucXe_v1 param = new ParamGetListGaraDanhMucXe_v1();
                if (objIn["IdHangXe"] != null)
                    param.IdHangXe = objIn["IdHangXe"].ToObject<string>();
                if (objIn["IdLoaiXe"] != null)
                    param.IdLoaiXe = objIn["IdLoaiXe"].ToObject<string>();
                if (objIn["IdMauXe"] != null)
                    param.IdMauXe = objIn["IdMauXe"].ToObject<string>();
                if (objIn["TrangThais"] != null)
                    param.TrangThais = objIn["TrangThais"].ToObject<List<int>>();
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassXe classXe = new ClassXe(db);
                    List<GetListGaraDanhMucXe_v1> dataXe = classXe.GetListGaraDanhMucXe_v1(param);
                    int count = 0;
                    if (dataXe.Count != 0)
                    {
                        count = dataXe[0].TotalRow;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = dataXe,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + dataXe.Count) + " trên tổng số " + count + " bản ghi",
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
        public IHttpActionResult Gara_GetListBaoGia_v2([FromBody] JObject objIn)
        {
            try
            {
                ParamSearch param = new ParamSearch();
                if (objIn["IdChiNhanhs"] != null)
                    param.LstIDChiNhanh = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["BaoGiaDateFrom"] != null && objIn["BaoGiaDateFrom"].ToObject<string>() != "")
                    param.FromDate = objIn["BaoGiaDateFrom"].ToObject<string>();
                if (objIn["BaoGiaDateTo"] != null && objIn["BaoGiaDateTo"].ToObject<string>() != "")
                    param.ToDate = objIn["BaoGiaDateTo"].ToObject<string>();
                if (objIn["IdPhieuTiepNhan"] != null && objIn["IdPhieuTiepNhan"].ToObject<string>() != "")
                    param.ID_HangXe = objIn["IdPhieuTiepNhan"].ToObject<string>();
                if (objIn["TrangThais"] != null)
                {
                    string strTrangThai = "";
                    List<int> lstTrangThais = objIn["TrangThais"].ToObject<List<int>>();
                    if (lstTrangThais.Count > 0)
                    {
                        strTrangThai = string.Join(",", lstTrangThais);
                    }
                    param.TrangThai = strTrangThai;
                }
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<Gara_BaoGia> xx = classPhieuTiepNhan.Gara_GetListBaoGia(param);
                    int count = 0;
                    if (xx.Count != 0)
                    {
                        count = xx[0].TotalRow.Value;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = xx,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + xx.Count) + " trên tổng số " + count + " bản ghi",
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
        public IHttpActionResult Gara_GetListPhieuNhapXuatKho_v1([FromBody] JObject objIn)
        {
            try
            {
                ParamGetListPhieuNhapXuatKhoByIDPhieuTiepNhan param = new ParamGetListPhieuNhapXuatKhoByIDPhieuTiepNhan();
                if (objIn["IdPhieuTiepNhan"] != null && objIn["IdPhieuTiepNhan"].ToObject<string>() != "")
                    param.IDPhieuTiepNhan = objIn["IdPhieuTiepNhan"].ToObject<Guid>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<GetListPhieuNhapXuatKhoByIDPhieuTiepNhan> xx = classPhieuTiepNhan.Gara_GetListPhieuNhapXuatKho(param);
                    int count = 0;
                    if (xx.Count != 0)
                    {
                        count = xx[0].TotalRow;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = xx,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + xx.Count) + " trên tổng số " + count + " bản ghi",
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
        public IHttpActionResult Gara_GetListHoaDonSuaChua_v2([FromBody] JObject objIn)
        {
            try
            {
                ParamSearch param = new ParamSearch();
                if (objIn["IdChiNhanhs"] != null)
                    param.LstIDChiNhanh = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["BaoGiaDateFrom"] != null && objIn["BaoGiaDateFrom"].ToObject<string>() != "")
                    param.FromDate = objIn["BaoGiaDateFrom"].ToObject<string>();
                if (objIn["BaoGiaDateTo"] != null && objIn["BaoGiaDateTo"].ToObject<string>() != "")
                    param.ToDate = objIn["BaoGiaDateTo"].ToObject<string>();
                if (objIn["IdPhieuTiepNhan"] != null && objIn["IdPhieuTiepNhan"].ToObject<string>() != "")
                    param.ID_HangXe = objIn["IdPhieuTiepNhan"].ToObject<string>();
                if (objIn["TrangThais"] != null)
                {
                    string strTrangThai = "";
                    List<int> lstTrangThais = objIn["TrangThais"].ToObject<List<int>>();
                    if (lstTrangThais.Count > 0)
                    {
                        strTrangThai = string.Join(",", lstTrangThais);
                    }
                    param.TrangThai = strTrangThai;
                }
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                if (objIn["IDXe"] != null)
                    param.IDXe = objIn["IDXe"].ToObject<string>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<Gara_BaoGia> xx = classPhieuTiepNhan.Gara_GetListHoaDonSuaChua(param);
                    int count = 0;
                    if (xx.Count != 0)
                    {
                        count = xx[0].TotalRow.Value;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = xx,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + xx.Count) + " trên tổng số " + count + " bản ghi",
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
        public IHttpActionResult Gara_GetListNhatKyBaoDuongTheoXe([FromBody] JObject objIn)
        {
            try
            {
                ParamSearch param = new ParamSearch();

                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                if (objIn["IDXe"] != null)
                    param.IDXe = objIn["IDXe"].ToObject<string>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassXe classxe = new ClassXe(db);
                    List<GetListNhatKyBaoDuongTheoXe> xx = classxe.GetListNhatKyBaoDuongTheoXe(new Guid(param.IDXe), param.CurrentPage, param.PageSize);
                    int count = 0;
                    if (xx.Count != 0)
                    {
                        count = xx[0].TotalRow;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = xx,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + xx.Count) + " trên tổng số " + count + " bản ghi",
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
        public IHttpActionResult Gara_GetListLichBaoDuongTheoXe([FromBody] JObject objIn)
        {
            try
            {
                ParamSeachLichBaoDuong param = new ParamSeachLichBaoDuong();

                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                if (objIn["IDXe"] != null)
                    param.ID_Xe = objIn["IDXe"].ToObject<string>();
                string TrangThai = "";
                if (objIn["TrangThai"] != null)
                {
                    param.TrangThais = objIn["TrangThai"].ToObject<string>();
                }
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classptn = new ClassPhieuTiepNhan(db);
                    List<PhuTung_LichBaoDuong> xx = classptn.GetLichBaoDuong(param);
                    int count = 0;
                    if (xx.Count != 0)
                    {
                        count = xx[0].TotalRow.Value;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, param.PageSize.Value, param.CurrentPage.Value + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = xx,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + xx.Count) + " trên tổng số " + count + " bản ghi",
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
        public IHttpActionResult Gara_GetListBaoGia(ParamSearch param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<Gara_BaoGia> xx = classPhieuTiepNhan.Gara_GetListBaoGia(param);
                    return ActionTrueData(xx);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [HttpGet]
        public bool CheckBaoGia_DaTaoHoaDonSuaChua_VaXuatKho(Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    return classPhieuTiepNhan.CheckBaoGia_DaTaoHoaDonSuaChua_VaXuatKho(id);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult Gara_GetListHoaDonSuaChua(ParamSearch param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<Gara_BaoGia> xx = classPhieuTiepNhan.Gara_GetListHoaDonSuaChua(param);
                    return ActionTrueData(xx);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult PhieuTiepNhan_GetThongTinChiTiet(Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<GetListPhieuTiepNhan_v2> xx = classPhieuTiepNhan.PhieuTiepNhan_GetThongTinChiTiet(id);
                    return ActionTrueData(xx);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult PhieuTiepNhan_GetTinhTrangXe(Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<PhieuTiepNhan_TinhTrang> hangmuc = classPhieuTiepNhan.PhieuTiepNhan_GetHangMucSuaChua(id);
                    List<PhieuTiepNhan_VatDungDinhKem> vatdung = classPhieuTiepNhan.PhieuTiepNhan_GetVatDungKemTheo(id);
                    return ActionTrueData(new
                    {
                        hangmuc,
                        vatdung,
                    });
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [HttpGet, HttpPut, HttpPost]
        public IHttpActionResult Duyet_HuyBaoGia(Guid id, bool? trangthai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var hd = db.BH_HoaDon.Find(id);
                    hd.ChoThanhToan = trangthai;
                    db.SaveChanges();
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        #endregion
        #region HoaDon
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListBaoGia_AfterXuLy(ParamSearch param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<Gara_BaoGia> data = classPhieuTiepNhan.GetListBaoGia_AfterXuLy(param);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }
        [HttpGet]
        public bool CheckHoaDon_DaXuLy(Guid idHoaDon, int loaiHoaDon)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var xx = db.BH_HoaDon.Where(x => x.ID_HoaDon == idHoaDon && x.LoaiHoaDon == loaiHoaDon && x.ChoThanhToan == false); ;
                    return xx.Count() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public bool Check_HDSC_ContainsHangHoa(Guid idHoaDon)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var xx = from ct in db.BH_HoaDon_ChiTiet
                             join qd in db.DonViQuiDois on ct.ID_DonViQuiDoi equals qd.ID
                             join hh in db.DM_HangHoa on qd.ID_HangHoa equals hh.ID
                             where ct.ID_HoaDon == idHoaDon && hh.LaHangHoa == true
                             select ct.ID;
                    return xx.Count() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #endregion
        #region BaoDuong
        [HttpPost]
        public IHttpActionResult GetLichBaoDuong(ParamSeachLichBaoDuong param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<PhuTung_LichBaoDuong> data = classPhieuTiepNhan.GetLichBaoDuong(param);
                    if (string.IsNullOrEmpty(param.ID_Xe))
                    {
                        int count = 0;
                        if (data.Count > 0)
                        {
                            count = data.FirstOrDefault().TotalRow ?? 0;
                        }
                        int page = 0;
                        int currentPage = param.CurrentPage ?? 0;
                        var listpage = GetListPage(count, param.PageSize ?? 10, currentPage + 1, ref page);
                        return ActionTrueData(new
                        {
                            data,
                            ListPage = listpage,
                            PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + (currentPage * param.PageSize + data.Count) + " trên tổng số " + count + " bản ghi",
                            TotalPage = page
                        });
                        return ActionTrueData(data);
                    }
                    var xx = data.GroupBy(x => new { x.ID_HangHoa, x.MaHangHoa, x.TenHangHoa, x.HanBaoHanh, x.GhiChu })
                        .Select(x => new
                        {
                            x.Key.ID_HangHoa,
                            x.Key.MaHangHoa,
                            x.Key.TenHangHoa,
                            x.Key.HanBaoHanh,
                            x.Key.GhiChu,
                            LanBaoDuong = x.Select(y => y.LanBaoDuong).Min(),
                            isCheckParent = false,
                            CTBaoDuongs = x.Select(o =>
                            new
                            {
                                o.ID,
                                o.ID_HangHoa,
                                o.MaHangHoa,
                                o.LanBaoDuong,
                                o.NgayBaoDuongDuKien,
                                o.SoKmBaoDuong,
                                isCheck = o.LanBaoDuong == x.Select(y => y.LanBaoDuong).Min(),
                            }).OrderBy(y => y.LanBaoDuong)
                        }).ToList();
                    return ActionTrueData(xx);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult GetKhachCoLichBaoDuong(ParamSeachLichBaoDuong param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<PhuTung_LichBaoDuong> data = classPhieuTiepNhan.GetLichBaoDuong(param);
                    var xx = data.GroupBy(x => new { x.ID_DoiTuong, x.MaDoiTuong, x.TenDoiTuong, x.DienThoai, x.Email })
                        .Select(x => new
                        {
                            x.Key.ID_DoiTuong,
                            x.Key.MaDoiTuong,
                            x.Key.TenDoiTuong,
                            x.Key.DienThoai,
                            x.Key.Email,
                        }).ToList();
                    return ActionTrueData(xx);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        [HttpGet]
        public IHttpActionResult GetNhatKyBaoDuong_byCar(Guid idCar)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<NhatKyBaoDuong> data = classPhieuTiepNhan.GetNhatKyBaoDuong_byCar(idCar);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        [HttpGet]
        public IHttpActionResult Insert_LichNhacBaoDuong(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    classPhieuTiepNhan.Insert_LichNhacBaoDuong(idHoaDon);
                    return ActionTrueData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult HuyHoaDon_UpdateLichBaoDuong(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var cthdBD = db.BH_HoaDon_ChiTiet.Where(x => x.ID_HoaDon == idHoaDon && x.ID_LichBaoDuong != null)
                        .Select(x => x.ID_LichBaoDuong).ToList();
                    // update lich dasudung --> chuasudung
                    db.Gara_LichBaoDuong.Where(x => cthdBD.Contains(x.ID)).ToList().ForEach(x => x.TrangThai = 1);

                    // update lich co idhoadon --> huy
                    db.Gara_LichBaoDuong.Where(x => x.ID_HoaDon == idHoaDon).ToList().ForEach(x => x.TrangThai = 0);
                    db.SaveChanges();
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult UpdateLichBD_whenChangeNgayLapHD(Guid idHoaDon, DateTime ngaylapOld, DateTime ngaylapNew)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    classPhieuTiepNhan.UpdateLichBD_whenChangeNgayLapHD(idHoaDon, ngaylapOld, ngaylapNew);
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult ExportExcel_LichBaoDuong(ParamSeachLichBaoDuong param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    var columnsHide = string.Empty;
                    if (param.ColumnsHide != null && param.ColumnsHide.Count > 0)
                    {
                        columnsHide = string.Join("_", param.ColumnsHide);
                    }
                    List<PhuTung_LichBaoDuong> data = classPhieuTiepNhan.GetLichBaoDuong(param);
                    DataTable excel = classOffice.ToDataTable<PhuTung_LichBaoDuong>(data);
                    excel.Columns.Remove("ID");
                    excel.Columns.Remove("ID_HangHoa");
                    excel.Columns.Remove("ID_Xe");
                    excel.Columns.Remove("ID_DoiTuong");
                    excel.Columns.Remove("TrangThai");
                    excel.Columns.Remove("sThoiGianBaoHanh");
                    excel.Columns.Remove("HanBaoHanh");
                    excel.Columns.Remove("TotalRow");

                    string tempFile = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_LichNhacBaoDuong.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/LichNhacBaoDuong.xlsx");
                    fileSave = classOffice.createFolder_Download(fileSave);
                    classOffice.listToOfficeExcel(tempFile, fileSave, excel, 3, 100, 97, true, columnsHide);

                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return ActionTrueNotData(fileSave);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        #endregion

        #endregion
        #region POST, PUT
        #region HangXe
        [HttpPost]
        public IHttpActionResult Post_HangXe(Gara_HangXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassHangXe classHangXe = new ClassHangXe(db);
                        obj.ID = Guid.NewGuid();
                        obj.NgayTao = DateTime.Now;
                        classHangXe.AddHangXe(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpPost, HttpPut]
        public IHttpActionResult Put_HangXe(Gara_HangXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassHangXe classHangXe = new ClassHangXe(db);
                        classHangXe.UpdateHangXe(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpDelete, HttpGet]
        public IHttpActionResult Delete_HangXe(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Gara_MauXe.Where(x => x.ID_HangXe == id).ToList().ForEach(x => x.ID_HangXe = Guid.Empty);

                        Gara_HangXe obj = db.Gara_HangXe.Find(id);
                        obj.TrangThai = 0;
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        #endregion

        #region LoaiXe
        [HttpPost]
        public IHttpActionResult Post_LoaiXe(Gara_LoaiXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassLoaiXe classLoaiXe = new ClassLoaiXe(db);
                        obj.ID = Guid.NewGuid();
                        obj.NgayTao = DateTime.Now;
                        classLoaiXe.AddLoaiXe(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        [HttpPost, HttpPut]
        public IHttpActionResult Put_LoaiXe(Gara_LoaiXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassLoaiXe classLoaiXe = new ClassLoaiXe(db);
                        classLoaiXe.UpdateLoaiXe(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpDelete, HttpGet]
        public IHttpActionResult Delete_LoaiXe(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // reset ID_LoaiXe in DM_MauXe
                        db.Gara_MauXe.Where(x => x.ID_LoaiXe == id).ToList().ForEach(x => x.ID_LoaiXe = Guid.Empty);

                        Gara_LoaiXe obj = db.Gara_LoaiXe.Find(id);
                        obj.TrangThai = 0;
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        #endregion
        #region MauXe
        [HttpPost]
        public IHttpActionResult Post_MauXe(Gara_MauXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassDanhMucMauXe classMauXe = new ClassDanhMucMauXe(db);
                        obj.ID = Guid.NewGuid();
                        obj.NgayTao = DateTime.Now;
                        classMauXe.AddMauXe(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        [HttpPost, HttpPut]
        public IHttpActionResult Put_MauXe(Gara_MauXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassDanhMucMauXe classMauXe = new ClassDanhMucMauXe(db);
                        classMauXe.UpdateMauXe(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpDelete, HttpGet]
        public IHttpActionResult Delete_MauXe(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Gara_DanhMucXe.Where(x => x.ID_MauXe == id).ToList().ForEach(x => x.ID_MauXe = Guid.Empty);

                        Gara_MauXe obj = db.Gara_MauXe.Find(id);
                        obj.TrangThai = 0;
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        #endregion
        #region DanhMucXe
        [HttpPost]
        public IHttpActionResult Post_DanhMucXe(Gara_DanhMucXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassXe classXe = new ClassXe(db);
                        obj.ID = Guid.NewGuid();
                        obj.NgayTao = DateTime.Now;
                        classXe.Insert(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        [HttpPost, HttpPut]
        public IHttpActionResult Put_DanhMucXe(Gara_DanhMucXe obj)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassXe classXe = new ClassXe(db);
                        classXe.Update(obj);
                        trans.Commit();
                        return ActionTrueData(obj);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        [HttpGet]
        public IHttpActionResult CheckBienSoXe(string id)
        {
            try
            {
                Gara_DanhMucXe garaXe = new Gara_DanhMucXe();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    garaXe = db.Gara_DanhMucXe.Where(p => p.BienSo.ToLower() == id.ToLower() && p.TrangThai == 1).FirstOrDefault();
                }
                if (garaXe != null)
                {
                    return ActionTrueData(new
                    {
                        ID = garaXe.ID
                    });
                }
                else
                {
                    return ActionFalseNotData("");
                }
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult CheckSaveDanhMucXe([FromBody] JObject objIn)
        {
            try
            {
                string BienSo = "";
                bool BienSoExist = true;
                bool isNew = true;
                if (objIn["BienSo"] != null)
                    BienSo = objIn["BienSo"].ToObject<string>();
                if (objIn["New"] != null)
                    isNew = objIn["New"].ToObject<bool>();

                if (BienSo != "")
                {
                    if (isNew)
                    {
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            BienSoExist = db.Gara_DanhMucXe.Where(p => p.BienSo.ToLower() == BienSo.ToLower() && p.TrangThai == 1).FirstOrDefault() != null;
                        }
                    }
                    else
                    {
                        string BienSoOld = "";
                        Guid IDXe = Guid.Empty;
                        if (objIn["IDXe"] != null)
                            IDXe = objIn["IDXe"].ToObject<Guid>();
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            BienSoOld = db.Gara_DanhMucXe.Where(p => p.ID == IDXe && p.TrangThai == 1).FirstOrDefault().BienSo;
                            if (BienSoOld.ToLower() != BienSo.ToLower())
                            {
                                BienSoExist = db.Gara_DanhMucXe.Where(p => p.BienSo.ToLower() == BienSo.ToLower() && p.TrangThai == 1).FirstOrDefault() != null;
                            }
                            else
                            {
                                BienSoExist = false;
                            }
                        }
                    }
                }
                return ActionTrueData(new
                {
                    BienSo = BienSoExist
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult CheckSaveHangXe([FromBody] JObject objIn)
        {
            try
            {
                string TenHangXe = "";
                bool HangXeExist = true;
                bool isNew = true;
                if (objIn["TenHangXe"] != null)
                    TenHangXe = objIn["TenHangXe"].ToObject<string>();
                if (objIn["New"] != null)
                    isNew = objIn["New"].ToObject<bool>();

                if (TenHangXe != "")
                {
                    if (isNew)
                    {
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            HangXeExist = db.Gara_HangXe.Where(p => p.TenHangXe.ToLower() == TenHangXe.ToLower() && p.TrangThai == 1).FirstOrDefault() != null;
                        }
                    }
                    else
                    {
                        string TenHangXeOld = "";
                        Guid IDHangXe = Guid.Empty;
                        if (objIn["ID"] != null)
                            IDHangXe = objIn["ID"].ToObject<Guid>();
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            TenHangXeOld = db.Gara_HangXe.Where(p => p.ID == IDHangXe && p.TrangThai == 1).FirstOrDefault().TenHangXe;
                            if (TenHangXeOld.ToLower() != TenHangXe.ToLower())
                            {
                                HangXeExist = db.Gara_HangXe.Where(p => p.TenHangXe.ToLower() == TenHangXe.ToLower() && p.TrangThai == 1).FirstOrDefault() != null;
                            }
                            else
                            {
                                HangXeExist = false;
                            }
                        }
                    }
                }
                return ActionTrueData(new
                {
                    TenHangXe = HangXeExist
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult CheckSaveLoaiXe([FromBody] JObject objIn)
        {
            try
            {
                string TenLoaiXe = "";
                bool LoaiXeExist = true;
                bool isNew = true;
                if (objIn["TenLoaiXe"] != null)
                    TenLoaiXe = objIn["TenLoaiXe"].ToObject<string>();
                if (objIn["New"] != null)
                    isNew = objIn["New"].ToObject<bool>();

                if (TenLoaiXe != "")
                {
                    if (isNew)
                    {
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            LoaiXeExist = db.Gara_LoaiXe.Where(p => p.TenLoaiXe.ToLower() == TenLoaiXe.ToLower() && p.TrangThai == 1).FirstOrDefault() != null;
                        }
                    }
                    else
                    {
                        string TenLoaiXeOld = "";
                        Guid IDLoaiXe = Guid.Empty;
                        if (objIn["ID"] != null)
                            IDLoaiXe = objIn["ID"].ToObject<Guid>();
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            TenLoaiXeOld = db.Gara_LoaiXe.Where(p => p.ID == IDLoaiXe && p.TrangThai == 1).FirstOrDefault().TenLoaiXe;
                            if (TenLoaiXeOld.ToLower() != TenLoaiXe.ToLower())
                            {
                                LoaiXeExist = db.Gara_LoaiXe.Where(p => p.TenLoaiXe.ToLower() == TenLoaiXe.ToLower() && p.TrangThai == 1).FirstOrDefault() != null;
                            }
                            else
                            {
                                LoaiXeExist = false;
                            }
                        }
                    }
                }
                return ActionTrueData(new
                {
                    TenLoaiXe = LoaiXeExist
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult CheckSaveMauXe([FromBody] JObject objIn)
        {
            try
            {
                string TenMauXe = "";
                bool MauXeExist = true;
                bool isNew = true;
                if (objIn["TenMauXe"] != null)
                    TenMauXe = objIn["TenMauXe"].ToObject<string>();
                if (objIn["New"] != null)
                    isNew = objIn["New"].ToObject<bool>();
                Guid IDLoaiXe = Guid.Empty;
                if (objIn["ID_LoaiXe"] != null)
                    IDLoaiXe = objIn["ID_LoaiXe"].ToObject<Guid>();
                Guid IDHangXe = Guid.Empty;
                if (objIn["ID_HangXe"] != null && (string)objIn["ID_HangXe"] != string.Empty)
                    IDHangXe = objIn["ID_HangXe"].ToObject<Guid>();
                if (TenMauXe != "")
                {
                    if (isNew)
                    {
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            MauXeExist = db.Gara_MauXe.Where(p => p.TenMauXe.ToLower() == TenMauXe.ToLower() && p.TrangThai == 1
                            && p.ID_HangXe == IDHangXe && p.ID_LoaiXe == IDLoaiXe).FirstOrDefault() != null;
                        }
                    }
                    else
                    {
                        string TenLoaiXeOld = "";
                        Guid IDMauXe = Guid.Empty;
                        if (objIn["ID"] != null)
                            IDMauXe = objIn["ID"].ToObject<Guid>();
                        using (SsoftvnContext db = SystemDBContext.GetDBContext())
                        {
                            Gara_MauXe mauxe = db.Gara_MauXe.Where(p => p.ID == IDMauXe && p.TrangThai == 1).FirstOrDefault();
                            //TenLoaiXeOld = db.Gara_LoaiXe.Where(p => p.ID == IDMauXe && p.TrangThai == 1).FirstOrDefault().TenLoaiXe;
                            if (mauxe.TenMauXe.ToLower() != TenMauXe.ToLower() || mauxe.ID_HangXe != IDHangXe || mauxe.ID_LoaiXe != IDLoaiXe)
                            {
                                MauXeExist = db.Gara_MauXe.Where(p => p.TenMauXe.ToLower() == TenMauXe.ToLower() && p.TrangThai == 1
                                && p.ID_HangXe == IDHangXe && p.ID_LoaiXe == IDLoaiXe).FirstOrDefault() != null;
                            }
                            else
                            {
                                MauXeExist = false;
                            }
                        }
                    }
                }
                return ActionTrueData(new
                {
                    TenMauXe = MauXeExist
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }
        #endregion
        #region Phieu tiep nhan
        [HttpPost]
        public IHttpActionResult Post_PhieuTiepNhan(Gara_PhieuTiepNhan phieuTN)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                        var idPhieuTN = Guid.NewGuid();
                        if (!string.IsNullOrEmpty(phieuTN.MaPhieuTiepNhan))
                        {
                            bool exists = classPhieuTN.CheckExist_MaPhieuTN(phieuTN.MaPhieuTiepNhan);
                            if (exists)
                            {
                                return ActionFalseNotData("Mã tiếp nhận đã tồn tại");
                            }
                        }
                        else
                        {
                            phieuTN.MaPhieuTiepNhan = classPhieuTN.SP_GetPhieuTiepNhan_byTemp(26, phieuTN.ID_DonVi, DateTime.Now);
                        }

                        phieuTN.ID = idPhieuTN;
                        phieuTN.NgayTao = DateTime.Now;
                        classPhieuTN.AddPhieuTiepNhan(phieuTN);
                        trans.Commit();
                        return ActionTrueData(new { ID = idPhieuTN, phieuTN.MaPhieuTiepNhan });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        [HttpPost, HttpPut]
        public IHttpActionResult Put_PhieuTiepNhan(Gara_PhieuTiepNhan phieuTN)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                        if (!string.IsNullOrEmpty(phieuTN.MaPhieuTiepNhan))
                        {
                            bool exists = classPhieuTN.CheckExist_MaPhieuTN(phieuTN.MaPhieuTiepNhan, phieuTN.ID);
                            if (exists)
                            {
                                return ActionFalseNotData("Mã tiếp nhận đã tồn tại");
                            }
                        }
                        else
                        {
                            phieuTN.MaPhieuTiepNhan = classPhieuTN.SP_GetPhieuTiepNhan_byTemp(26, phieuTN.ID_DonVi, DateTime.Now);
                        }

                        classPhieuTN.UpdatePhieuTiepNhan(phieuTN);
                        trans.Commit();
                        return ActionTrueData(phieuTN);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpPost, HttpPut]
        public IHttpActionResult PhieuTN_XuatXuong(Gara_PhieuTiepNhan phieuTN)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                        classPhieuTN.PhieuTN_XuatXuong(phieuTN);
                        trans.Commit();
                        return ActionTrueData(phieuTN);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpPost, HttpPut]
        public IHttpActionResult Put_HangMucSuaChua([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Guid idPhieuTN = data["idPhieuTN"].ToObject<Guid>();
                        List<Gara_HangMucSuaChua> hangmucs = new List<Gara_HangMucSuaChua>();
                        ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);

                        if (data["hangmuc"] != null)
                        {
                            hangmucs = data["hangmuc"].ToObject<List<Gara_HangMucSuaChua>>();
                        }

                        // delete & add again
                        classPhieuTN.RemoveHangMucSuaChua(idPhieuTN);

                        List<Gara_HangMucSuaChua> lstHangMuc = new List<Gara_HangMucSuaChua>();
                        foreach (var item in hangmucs)
                        {
                            item.ID = Guid.NewGuid();
                            item.ID_PhieuTiepNhan = idPhieuTN;
                            lstHangMuc.Add(item);
                        }
                        classPhieuTN.AddHangMucSuaChua(lstHangMuc);

                        trans.Commit();
                        return ActionTrueData(lstHangMuc);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        [HttpPost, HttpPut]
        public IHttpActionResult Put_GiayToKemTheo([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Guid idPhieuTN = data["idPhieuTN"].ToObject<Guid>();
                        List<Gara_GiayToKemTheo> vatdungs = new List<Gara_GiayToKemTheo>();
                        ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);

                        if (data["vatdung"] != null)
                        {
                            vatdungs = data["vatdung"].ToObject<List<Gara_GiayToKemTheo>>();
                        }

                        // delete & add again
                        classPhieuTN.RemoveVatDungKemTheo(idPhieuTN);

                        List<Gara_GiayToKemTheo> lstVatDung = new List<Gara_GiayToKemTheo>();
                        foreach (var item in vatdungs)
                        {
                            item.ID = Guid.NewGuid();
                            item.ID_PhieuTiepNhan = idPhieuTN;
                            lstVatDung.Add(item);
                        }
                        classPhieuTN.AddVatDungKemTheo(lstVatDung);

                        trans.Commit();
                        return ActionTrueData(lstVatDung);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpGet]
        public IHttpActionResult PhieuTiepNhan_UpdateTrangThai(Guid id, int status)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                        classPhieuTN.PhieuTiepNhan_UpdateTrangThai(id, status);
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteDanhMucXe_v1(Guid id, int trangthai = 0)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassXe classXe = new ClassXe(db);
                        classXe.DeleteDanhMucXe(id, trangthai);
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.ToString());
                    }
                }
            }
        }
        #endregion

        #region PhieuXuatKho

        [HttpGet]
        public IHttpActionResult XuatKhoToanBo_FromHoaDonSC(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                    Param_XuatKhoToanBo data = classPhieuTN.XuatKhoToanBo_FromHoaDonSC(idHoaDon);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("XuatKhoToanBo_FromHoaDonSC " + ex.InnerException + ex.Message);
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpGet]
        public IHttpActionResult PhieuXuatKho_NguyenVatLieu(Guid idHoaDon)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                    Param_XuatKhoToanBo data = classPhieuTN.PhieuXuatKho_NguyenVatLieu(idHoaDon);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("PhieuXuatKho_NguyenVatLieu " + ex.InnerException + ex.Message);
                    return ActionFalseNotData(ex.ToString());
                }
            }
        }

        [HttpPost]
        public IHttpActionResult JqAuto_HoaDonSC(ParamSearch param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                    List<XuatKho_JqautoHDSC> data = classPhieuTN.JqAuto_HoaDonSC(param);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult ImageUpload(string rootFolder, string subFolder = null)
        {
            // path file: ImageUpload/SubDomain/rootFolder/subFolder
            // ex: ImageUpload/0973474985/LogoHangXe/MaHangXe.jpg
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
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, pathFile));
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, pathFile));
            }
        }
        #endregion

        #region LichBaoDuong
        [HttpPost]
        public IHttpActionResult LichNhacBaoDuong_updateStatus([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    List<Guid> idLichs = new List<Guid>();
                    if (data["arrIDLich"] != null)
                    {
                        idLichs = data["arrIDLich"].ToObject<List<Guid>>();
                        var status = data["status"].ToObject<int>();
                        ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                        classPhieuTiepNhan.SuDungBaoDuong_UpdateStatus(idLichs, status);
                        return ActionTrueData(string.Empty);
                    }
                    return ActionTrueData("Không có dữ liệu được truyền vào");
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }
        [HttpPost]
        // used to when send sms, email
        public IHttpActionResult LichNhacBaoDuong_updateSoLanNhac([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    List<Guid> idLichs = new List<Guid>();
                    if (data["arrIDLich"] != null)
                    {
                        idLichs = data["arrIDLich"].ToObject<List<Guid>>();
                        db.Gara_LichBaoDuong.Where(x => idLichs.Contains(x.ID)
                   && (x.TrangThai == 1 || x.TrangThai == 3)).ToList().ForEach(x => { x.LanNhac = (x.LanNhac ?? 0) + 1; x.TrangThai = 3; });
                        db.SaveChanges();
                        return ActionTrueData(string.Empty);
                    }
                    return ActionFalseNotData("Không có dữ liệu được truyền vào");
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }
        [HttpPost]
        public IHttpActionResult LichNhacBaoDuong_Update(Gara_LichBaoDuong data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    Gara_LichBaoDuong objUp = db.Gara_LichBaoDuong.Find(data.ID);
                    objUp.NgayBaoDuongDuKien = data.NgayBaoDuongDuKien;
                    objUp.GhiChu = data.GhiChu;
                    objUp.LanNhac = data.LanNhac;
                    if (data.LanNhac != null)
                    {
                        if (data.LanNhac > 0)
                        {
                            if (objUp.TrangThai == 1)
                            {
                                objUp.TrangThai = 3;
                            }
                        }
                        else
                        {
                            if (data.LanNhac == 0 && objUp.TrangThai == 3)
                            {
                                objUp.TrangThai = 1;
                            }
                        }
                    }
                    db.SaveChanges();
                    return ActionTrueData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }
        [HttpPost, HttpGet]
        public IHttpActionResult UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN(Guid idPhieuTN, float chenhLechKM)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTN = new ClassPhieuTiepNhan(db);
                    classPhieuTN.UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN(idPhieuTN, chenhLechKM);
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }
        [HttpPost]
        public IHttpActionResult UpdateHD_UpdateBaoDuong(ParamUpdateLichBaoDuong param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    classPhieuTiepNhan.UpdateHD_UpdateLichBaoDuong(param);
                    return ActionTrueData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac(Guid idLichNhac, int typeUpdate = 0)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    classPhieuTiepNhan.InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac(idLichNhac, typeUpdate);
                    return ActionTrueData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        #endregion
        #endregion

        #region ExportExcel
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ExportExcel_GetListPhieuTiepNhan_v2([FromBody] JObject objIn)
        {
            string fileSave = string.Empty;
            try
            {
                ParamGetListPhieuTiepNhan_v2 param = new ParamGetListPhieuTiepNhan_v2();
                if (objIn["IdChiNhanhs"] != null)
                    param.IdChiNhanhs = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["NgayTiepNhanFrom"] != null && objIn["NgayTiepNhanFrom"].ToObject<string>() != "")
                    param.NgayTiepNhan_From = objIn["NgayTiepNhanFrom"].ToObject<DateTime>();
                if (objIn["NgayTiepNhanTo"] != null && objIn["NgayTiepNhanTo"].ToObject<string>() != "")
                    param.NgayTiepNhan_To = objIn["NgayTiepNhanTo"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongDuKienFrom"] != null && objIn["NgayXuatXuongDuKienFrom"].ToObject<string>() != "")
                    param.NgayXuatXuongDuKien_From = objIn["NgayXuatXuongDuKienFrom"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongDuKienTo"] != null && objIn["NgayXuatXuongDuKienTo"].ToObject<string>() != "")
                    param.NgayXuatXuongDuKien_To = objIn["NgayXuatXuongDuKienTo"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongFrom"] != null && objIn["NgayXuatXuongFrom"].ToObject<string>() != "")
                    param.NgayXuatXuong_From = objIn["NgayXuatXuongFrom"].ToObject<DateTime>();
                if (objIn["NgayXuatXuongTo"] != null && objIn["NgayXuatXuongTo"].ToObject<string>() != "")
                    param.NgayXuatXuong_To = objIn["NgayXuatXuongTo"].ToObject<DateTime>();
                if (objIn["TrangThais"] != null)
                    param.TrangThais = objIn["TrangThais"].ToObject<List<int>>();
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["BaoHiemFilter"] != null)
                    param.BaoHiem = objIn["BaoHiemFilter"].ToObject<int>();
                param.PageSize = 0;
                List<int> lstColHide = new List<int>();
                if (objIn["ColHide"] != null)
                    lstColHide = objIn["ColHide"].ToObject<List<int>>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    List<GetListPhieuTiepNhan_v2> dataPhieuTiepNhan = classPhieuTiepNhan.GetListPhieuTiepNhan_v2(param);
                    List<GetListPhieuTiepNhan_v2_Export> lst = dataPhieuTiepNhan.Select(p => new GetListPhieuTiepNhan_v2_Export
                    {
                        MaPhieuTiepNhan = p.MaPhieuTiepNhan,
                        NgayVaoXuong = p.NgayVaoXuong,
                        BienSo = p.BienSo,
                        MaDoiTuong = p.MaDoiTuong,
                        TenDoiTuong = p.TenDoiTuong,
                        DienThoaiKhachHang = p.DienThoaiKhachHang,
                        TenLienHe = p.TenLienHe,
                        SoDienThoaiLienHe = p.SoDienThoaiLienHe,
                        TenMauXe = p.TenMauXe,
                        SoKmVao = p.SoKmVao,
                        NgayXuatXuongDuKien = p.NgayXuatXuongDuKien,
                        NgayXuatXuong = p.NgayXuatXuong,
                        SoKmRa = p.SoKmRa,
                        CoVanDichVu = p.CoVanDichVu,
                        GhiChu = p.GhiChu,
                        NgayTao = p.NgayTao,
                        NhanVienTiepNhan = p.NhanVienTiepNhan,
                        TrangThaiPhieuTiepNhan = p.TrangThai == 1 ? "Đang sửa" : p.TrangThai == 2 ? "Hoàn thành" : p.TrangThai == 3 ? "Đã xuất xưởng" : "Hủy",
                        TenDonVi = p.TenDonVi,
                        TenBaoHiem = p.TenBaoHiem
                    }).ToList();
                    DataTable excel = classOffice.ToDataTable<GetListPhieuTiepNhan_v2_Export>(lst);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_DanhSachPhieuTiepNhan.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachPhieuTiepNhan.xlsx");
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
                CookieStore.WriteLog("ExportExcel_GetListPhieuTiepNhan_v2 " + ex.InnerException + ex.Message);
                return ActionFalseNotData(ex.Message);
            }
        }
        public IHttpActionResult ExportExcel_GetListGaraDanhMucXe_v1([FromBody] JObject objIn)
        {
            string fileSave = string.Empty;
            try
            {
                ParamGetListGaraDanhMucXe_v1 param = new ParamGetListGaraDanhMucXe_v1();
                if (objIn["IdHangXe"] != null)
                    param.IdHangXe = objIn["IdHangXe"].ToObject<string>();
                if (objIn["IdLoaiXe"] != null)
                    param.IdLoaiXe = objIn["IdLoaiXe"].ToObject<string>();
                if (objIn["IdMauXe"] != null)
                    param.IdMauXe = objIn["IdMauXe"].ToObject<string>();
                if (objIn["TrangThais"] != null)
                    param.TrangThais = objIn["TrangThais"].ToObject<List<int>>();
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                param.PageSize = 0;
                List<int> lstColHide = new List<int>();
                if (objIn["ColHide"] != null)
                    lstColHide = objIn["ColHide"].ToObject<List<int>>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    ClassXe classXe = new ClassXe(db);
                    List<GetListGaraDanhMucXe_v1> dataXe = classXe.GetListGaraDanhMucXe_v1(param);
                    List<GetListGaraDanhMucXe_v1_Export> lst = dataXe.Select(p => new GetListGaraDanhMucXe_v1_Export
                    {
                        BienSo = p.BienSo,
                        MaChuXe = p.MaDoiTuong,
                        ChuXe = p.TenDoiTuong,
                        DienThoai = p.DienThoai,
                        HangXe = p.TenHangXe,
                        LoaiXe = p.TenLoaiXe,
                        MauXe = p.TenMauXe,
                        NamSanXuat = p.NamSanXuat,
                        SoKhung = p.SoKhung,
                        SoMay = p.SoMay,
                        MauSon = p.MauSon,
                        DungTich = p.DungTich,
                        HopSo = p.HopSo,
                        GhiChu = p.GhiChu,
                        TrangThai = p.TrangThai == 1 ? "Đang sử dụng" : "Hủy"
                    }).ToList();
                    DataTable excel = classOffice.ToDataTable<GetListGaraDanhMucXe_v1_Export>(lst);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_DanhSachXe.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachXe.xlsx");
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
                CookieStore.WriteLog("ExportExcel_GetListGaraDanhMucXe_v1 " + ex.InnerException + ex.Message);
                return ActionFalseNotData(ex.Message);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ExportExcel_Gara_GetListHoaDonSuaChua_v2([FromBody] JObject objIn)
        {
            string fileSave = "";
            try
            {
                ParamSearch param = new ParamSearch();
                if (objIn["IdChiNhanhs"] != null)
                    param.LstIDChiNhanh = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["BaoGiaDateFrom"] != null && objIn["BaoGiaDateFrom"].ToObject<string>() != "")
                    param.FromDate = objIn["BaoGiaDateFrom"].ToObject<string>();
                if (objIn["BaoGiaDateTo"] != null && objIn["BaoGiaDateTo"].ToObject<string>() != "")
                    param.ToDate = objIn["BaoGiaDateTo"].ToObject<string>();
                if (objIn["IdPhieuTiepNhan"] != null && objIn["IdPhieuTiepNhan"].ToObject<string>() != "")
                    param.ID_HangXe = objIn["IdPhieuTiepNhan"].ToObject<string>();
                if (objIn["TrangThais"] != null)
                {
                    string strTrangThai = "";
                    List<int> lstTrangThais = objIn["TrangThais"].ToObject<List<int>>();
                    if (lstTrangThais.Count > 0)
                    {
                        strTrangThai = string.Join(",", lstTrangThais);
                    }
                    param.TrangThai = strTrangThai;
                }
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                param.PageSize = 0;
                if (objIn["IDXe"] != null)
                    param.IDXe = objIn["IDXe"].ToObject<string>();
                List<int> lstColHide = new List<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    List<Gara_BaoGia> xx = classPhieuTiepNhan.Gara_GetListHoaDonSuaChua(param);
                    List<LichSuSuaChua_Export> lst = xx.Select(p => new LichSuSuaChua_Export
                    {
                        MaHoaDon = p.MaHoaDon,
                        MaBaoGia = p.MaBaoGia,
                        NgayLapHoaDon = p.NgayLapHoaDon.Value,
                        PhaiThanhToanKhachHang = p.PhaiThanhToan,
                        PhaiThanhToanBaoHiem = p.PhaiThanhToanBaoHiem == null ? 0 : p.PhaiThanhToanBaoHiem.Value,
                        DaThanhToanKhacchHang = p.KhachDaTra == null ? 0 : p.KhachDaTra.Value,
                        DaThanhToanBaoHiem = p.BaoHiemDaTra == null ? 0 : p.BaoHiemDaTra.Value,
                        ConThieuKhachHang = p.PhaiThanhToan - (p.KhachDaTra == null ? 0 : p.KhachDaTra.Value),
                        ConThieuBaoHiem = (p.PhaiThanhToanBaoHiem == null ? 0 : p.PhaiThanhToanBaoHiem.Value) - (p.BaoHiemDaTra == null ? 0 : p.BaoHiemDaTra.Value),
                        GhiChu = p.DienGiai
                    }).ToList();
                    DataTable excel = classOffice.ToDataTable<LichSuSuaChua_Export>(lst);
                    string BienSo = "";
                    string TenDoiTuong = "";
                    string fileTeamplate = "";
                    if (param.IDXe != "")
                    {
                        fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_DanhSachXe_LichSuSuaChua.xlsx");
                        fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachXe_LichSuSuaChua.xlsx");
                        fileSave = classOffice.createFolder_Download(fileSave);
                        ClassXe classXe = new ClassXe(db);
                        Gara_DanhMucXe gara_DanhMucXe = classXe.GetGara_DanhMucXeById(new Guid(param.IDXe));

                        if (gara_DanhMucXe != null)
                        {
                            BienSo = "Biển số: " + gara_DanhMucXe.BienSo;
                            libDM_DoiTuong.classDM_DoiTuong classdt = new libDM_DoiTuong.classDM_DoiTuong(db);

                            if (gara_DanhMucXe.ID_KhachHang != null)
                            {
                                DM_DoiTuong dt = classdt.Select_DoiTuong(gara_DanhMucXe.ID_KhachHang.Value);
                                if (dt != null)
                                {
                                    TenDoiTuong = "Khách hàng: " + dt.TenDoiTuong;
                                }
                            }
                        }
                    }
                    else
                    {
                        fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_PhieuTiepNhan_HoaDon.xlsx");
                        fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuTiepNhan_HoaDon.xlsx");
                        fileSave = classOffice.createFolder_Download(fileSave);
                        Gara_PhieuTiepNhan ptn = classPhieuTiepNhan.GetGara_PhieuTiepNhanById(new Guid(param.ID_HangXe));
                        if (ptn != null)
                        {
                            BienSo = "Phiếu tiếp nhận: " + ptn.MaPhieuTiepNhan;

                            libDM_DoiTuong.classDM_DoiTuong classdt = new libDM_DoiTuong.classDM_DoiTuong(db);
                            DM_DoiTuong dt = classdt.Select_DoiTuong(ptn.ID_KhachHang);
                            if (dt != null)
                            {
                                TenDoiTuong = "Khách hàng: " + dt.TenDoiTuong;
                            }

                        }
                    }

                    classOffice.listToOfficeExcel_v2(fileTeamplate, fileSave, excel, 6, 100, 94, false, lstColHide, BienSo, TenDoiTuong);

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
        public IHttpActionResult ExportExcel_Gara_GetListBaoGia_v2([FromBody] JObject objIn)
        {
            string fileSave = "";
            try
            {
                ParamSearch param = new ParamSearch();
                if (objIn["IdChiNhanhs"] != null)
                    param.LstIDChiNhanh = objIn["IdChiNhanhs"].ToObject<List<string>>();
                if (objIn["BaoGiaDateFrom"] != null && objIn["BaoGiaDateFrom"].ToObject<string>() != "")
                    param.FromDate = objIn["BaoGiaDateFrom"].ToObject<string>();
                if (objIn["BaoGiaDateTo"] != null && objIn["BaoGiaDateTo"].ToObject<string>() != "")
                    param.ToDate = objIn["BaoGiaDateTo"].ToObject<string>();
                if (objIn["IdPhieuTiepNhan"] != null && objIn["IdPhieuTiepNhan"].ToObject<string>() != "")
                    param.ID_HangXe = objIn["IdPhieuTiepNhan"].ToObject<string>();
                if (objIn["TrangThais"] != null)
                {
                    string strTrangThai = "";
                    List<int> lstTrangThais = objIn["TrangThais"].ToObject<List<int>>();
                    if (lstTrangThais.Count > 0)
                    {
                        strTrangThai = string.Join(",", lstTrangThais);
                    }
                    param.TrangThai = strTrangThai;
                }
                if (objIn["TextSearch"] != null)
                    param.TextSearch = objIn["TextSearch"].ToObject<string>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                if (objIn["PageSize"] != null)
                    param.PageSize = objIn["PageSize"].ToObject<int>();
                List<int> lstColHide = new List<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<Gara_BaoGia> xx = classPhieuTiepNhan.Gara_GetListBaoGia(param);
                    List<PhieuTiepNhanBaoGia_Export> lst = xx.Select(p => new PhieuTiepNhanBaoGia_Export
                    {
                        MaBaoGia = p.MaHoaDon,
                        NgayLapBaoGia = p.NgayLapHoaDon.Value,
                        TongTienHang = p.TongTienHang,
                        KhachCanTra = p.TongThanhToan == null ? 0 : p.TongThanhToan.Value,
                        TrangThai = p.TrangThaiText,
                        GhiChu = p.DienGiai
                    }).ToList();
                    DataTable excel = classOffice.ToDataTable<PhieuTiepNhanBaoGia_Export>(lst);
                    string fileTeamplate = "", TenDoiTuong = "", BienSo = "";
                    fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_PhieuTiepNhan_BaoGia.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuTiepNhan_BaoGia.xlsx");
                    fileSave = classOffice.createFolder_Download(fileSave);
                    Gara_PhieuTiepNhan ptn = classPhieuTiepNhan.GetGara_PhieuTiepNhanById(new Guid(param.ID_HangXe));
                    if (ptn != null)
                    {
                        BienSo = "Phiếu tiếp nhận: " + ptn.MaPhieuTiepNhan;

                        libDM_DoiTuong.classDM_DoiTuong classdt = new libDM_DoiTuong.classDM_DoiTuong(db);
                        DM_DoiTuong dt = classdt.Select_DoiTuong(ptn.ID_KhachHang);
                        if (dt != null)
                        {
                            TenDoiTuong = "Khách hàng: " + dt.TenDoiTuong;
                        }

                    }
                    classOffice.listToOfficeExcel_v2(fileTeamplate, fileSave, excel, 5, 99, 94, false, lstColHide, BienSo, TenDoiTuong);

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
        public IHttpActionResult ExportExcel_Gara_GetListPhieuNhapXuatKho_v1([FromBody] JObject objIn)
        {
            string fileSave = "";
            try
            {
                ParamGetListPhieuNhapXuatKhoByIDPhieuTiepNhan param = new ParamGetListPhieuNhapXuatKhoByIDPhieuTiepNhan();
                if (objIn["IdPhieuTiepNhan"] != null && objIn["IdPhieuTiepNhan"].ToObject<string>() != "")
                    param.IDPhieuTiepNhan = objIn["IdPhieuTiepNhan"].ToObject<Guid>();
                if (objIn["CurrentPage"] != null)
                    param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
                param.PageSize = 0;
                List<int> lstColHide = new List<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    ClassPhieuTiepNhan classPhieuTiepNhan = new ClassPhieuTiepNhan(db);
                    List<GetListPhieuNhapXuatKhoByIDPhieuTiepNhan> xx = classPhieuTiepNhan.Gara_GetListPhieuNhapXuatKho(param);
                    List<GetListPhieuNhapXuatKhoByIDPhieuTiepNhan_Export> lst = xx.Select(p => new GetListPhieuNhapXuatKhoByIDPhieuTiepNhan_Export
                    {
                        MaPhieu = p.MaHoaDon,
                        NgayLap = p.NgayLapHoaDon,
                        LoaiPhieu = p.LoaiHoaDon == 8 ? "Xuất kho" : "Nhập kho",
                        MaHoaDon = p.HoaDonSuaChua,
                        SoLuong = p.SoLuong,
                        GiaTri = p.GiaTri
                    }).ToList();
                    DataTable excel = classOffice.ToDataTable<GetListPhieuNhapXuatKhoByIDPhieuTiepNhan_Export>(lst);
                    string fileTeamplate = "", TenDoiTuong = "", BienSo = "";
                    fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Gara/Template_PhieuTiepNhan_PhieuNhapXuat.xlsx");
                    fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/PhieuTiepNhan_PhieuNhapXuat.xlsx");
                    fileSave = classOffice.createFolder_Download(fileSave);
                    Gara_PhieuTiepNhan ptn = classPhieuTiepNhan.GetGara_PhieuTiepNhanById(param.IDPhieuTiepNhan);
                    if (ptn != null)
                    {
                        BienSo = "Phiếu tiếp nhận: " + ptn.MaPhieuTiepNhan;

                        libDM_DoiTuong.classDM_DoiTuong classdt = new libDM_DoiTuong.classDM_DoiTuong(db);
                        DM_DoiTuong dt = classdt.Select_DoiTuong(ptn.ID_KhachHang);
                        if (dt != null)
                        {
                            TenDoiTuong = "Khách hàng: " + dt.TenDoiTuong;
                        }

                    }
                    classOffice.listToOfficeExcel_v2(fileTeamplate, fileSave, excel, 5, 99, 94, false, lstColHide, BienSo, TenDoiTuong);

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

        #endregion

        #region ImportExcel
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImportDanhSachXe()
        {
            List<ImportErrorList> lstError = new List<ImportErrorList>();
            try
            {
                var file = HttpContext.Current.Request.Files[0];
                System.IO.Stream excelstream = file.InputStream;
                GaraDocument garaDocument = new GaraDocument();
                DataTable dt = garaDocument.ToDataTable(excelstream, 2, 0);
                bool checkImport = false;
                if (dt.Columns["Biển số"] == null || dt.Columns["Tên mẫu xe"] == null ||
                    dt.Columns["Tên hãng xe"] == null || dt.Columns["Tên loại xe"] == null ||
                    dt.Columns["Số máy"] == null || dt.Columns["Số khung"] == null ||
                    dt.Columns["Năm sản xuất"] == null || dt.Columns["Màu sơn"] == null ||
                    dt.Columns["Dung tích"] == null || dt.Columns["Hộp số"] == null |
                    dt.Columns["Mã khách hàng (Chủ xe)"] == null || dt.Columns["Ghi chú"] == null)
                {
                    return ActionFalseWithData(new
                    {
                        ErrorList = lstError,
                        Message = "File không đúng định dạng, vui lòng chọn đúng file import"
                    });
                }
                int namsanxuatresult = 0;
                string cxd = "Chưa xác định";
                List<DanhSachXeImport> list = dt.AsEnumerable().Select(p => new DanhSachXeImport
                {
                    BienSo = p["Biển số"].ToString(),
                    TenMauXe = p["Tên mẫu xe"].ToString() == "" ? cxd : p["Tên mẫu xe"].ToString(),
                    TenHangXe = p["Tên mẫu xe"].ToString() == "" ? cxd : p["Tên hãng xe"].ToString() == "" ? cxd : p["Tên hãng xe"].ToString(),
                    TenLoaiXe = p["Tên mẫu xe"].ToString() == "" ? cxd : p["Tên loại xe"].ToString() == "" ? cxd : p["Tên loại xe"].ToString(),
                    SoMay = p["Số máy"].ToString(),
                    SoKhung = p["Số khung"].ToString(),
                    NamSanXuat = p["Năm sản xuất"].ToString() == "" ? 0 : Int32.TryParse(p["Năm sản xuất"].ToString(), out namsanxuatresult) ? namsanxuatresult : -1,
                    MauSon = p["Màu sơn"].ToString(),
                    DungTich = p["Dung tích"].ToString(),
                    HopSo = p["Hộp số"].ToString(),
                    MaKhachHang = p["Mã khách hàng (Chủ xe)"].ToString(),
                    GhiChu = p["Ghi chú"].ToString(),
                    index = dt.Rows.IndexOf(p) + 4
                }).ToList();
                if (list.Count > 6000)
                {
                    ImportErrorList importErrorList = new ImportErrorList()
                    {
                        index = 0,
                        ViTri = "",
                        MoTa = "Vượt quá giới hạn số bản ghi cho phép."
                    };
                    return ActionFalseWithData(new
                    {
                        ErrorList = lstError,
                        Message = "Nhập dữ liệu thất bại"
                    });
                }
                else
                {
                    List<ImportErrorList> lstErrorNamSanXuat = list.Where(p => p.NamSanXuat == -1).Select(p => new ImportErrorList
                    {
                        index = p.index,
                        ViTri = "Dòng số " + p.index,
                        MoTa = @"<span style=""color: red"">Năm sản xuất</span> không đúng"
                    }).ToList();
                    lstError.AddRange(lstErrorNamSanXuat);
                    string account = CookieStore.GetCookieAes("Account");
                    HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {

                        List<ImportErrorList> lstErrorBienSo = (from dmx in db.Gara_DanhMucXe.Select(p => p.BienSo).AsEnumerable()
                                                                join l in list
                                                                on dmx.ToUpper() equals l.BienSo.ToUpper()
                                                                select new ImportErrorList
                                                                {
                                                                    index = l.index,
                                                                    ViTri = "Dòng số " + (l.index),
                                                                    MoTa = @"Biển số <span style=""color: red"">" + l.BienSo + "</span> đã tồn tại"
                                                                }).ToList();
                        lstError.AddRange(lstErrorBienSo);
                        List<ImportErrorList> lstErrorKhachHang = (from l in list.Where(p => p.MaKhachHang != "")
                                                                   join dmdt in db.DM_DoiTuong.Select(p => p.MaDoiTuong).AsEnumerable()
                                                                on l.MaKhachHang equals dmdt into dmdts
                                                                   from dm in dmdts.DefaultIfEmpty()
                                                                   where dm == null
                                                                   select new ImportErrorList
                                                                   {
                                                                       index = l.index,
                                                                       ViTri = "Dòng số " + (l.index),
                                                                       MoTa = @"Mã khách hàng <span style=""color: red"">" + l.MaKhachHang + "</span> không tồn tại"
                                                                   }).ToList();
                        lstError.AddRange(lstErrorKhachHang);
                        lstError.AddRange(list.GroupBy(p => p.BienSo).Where(g => g.Count() > 1)
                        .Select(p => p.Key).ToList().GroupJoin(list, l => l, l1 => l1.BienSo, (l, l1) => new { l, l1 }).SelectMany(p => p.l1.DefaultIfEmpty()
                        , (p, l1) => new ImportErrorList
                        {
                            index = l1.index,
                            ViTri = "Dòng số " + l1.index,
                            MoTa = @"Biển số <span style=""color: red"">" + l1.BienSo + "</span> lặp lại trong file dữ liệu"
                        }).ToList());
                        lstError.AddRange(list.Where(p => p.BienSo.Trim() == "")
                            .Select(p => new ImportErrorList
                            {
                                index = p.index,
                                ViTri = "Dòng số " + p.index,
                                MoTa = @"<span style=""color: red"">" + (p.BienSo.Trim() == "" ? "Biển số, " : "") + "</span> không được để trống"
                            }).ToList());
                        lstError.AddRange(list.Where(p => p.BienSo.Trim().Length > 30)
                            .Select(p => new ImportErrorList
                            {
                                index = p.index,
                                ViTri = "Dòng số " + p.index,
                                MoTa = @"Biển số <span style=""color: red"">" + p.BienSo + "</span> vượt quá số ký tự cho phép. Số ký tự tối đa là 30 ký tự"
                            }).ToList());
                        if (lstError.Count > 0)
                        {
                            return ActionFalseWithData(new
                            {
                                ErrorList = lstError.OrderBy(p => p.index).ToList(),
                                Message = "Nhập dữ liệu thất bại"
                            });
                        }
                        else
                        {
                            ClassXe classXe = new ClassXe(db);
                            checkImport = classXe.ImportDanhSachXe(list, nguoidung.TaiKhoan);
                        }
                    }
                    if (checkImport)
                    {
                        return ActionTrueNotData("Nhập file thành công");
                    }
                    else
                    {
                        return ActionFalseWithData(new
                        {
                            ErrorList = lstError,
                            Message = "Nhập dữ liệu thất bại"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return ActionFalseWithData(new
                {
                    ErrorList = lstError,
                    Message = "Nhập dữ liệu thất bại"
                });
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImportDanhSachXeWithoutError()
        {
            List<ImportErrorList> lstError = new List<ImportErrorList>();
            var file = HttpContext.Current.Request.Files[0];
            string ListError = HttpContext.Current.Request.Form["errorList"];
            List<int> ErrorIndex = ListError.Split(',').Select(Int32.Parse).Distinct().ToList();
            System.IO.Stream excelstream = file.InputStream;
            GaraDocument garaDocument = new GaraDocument();
            DataTable dt = garaDocument.ToDataTable(excelstream, 2, 0);
            int namsanxuatresult = 0;
            string cxd = "Chưa xác định";
            List<DanhSachXeImport> list = dt.AsEnumerable().Select(p => new DanhSachXeImport
            {
                BienSo = p["Biển số"].ToString(),
                TenMauXe = p["Tên mẫu xe"].ToString() == "" ? cxd : p["Tên mẫu xe"].ToString(),
                TenHangXe = p["Tên mẫu xe"].ToString() == "" ? cxd : p["Tên hãng xe"].ToString() == "" ? cxd : p["Tên hãng xe"].ToString(),
                TenLoaiXe = p["Tên mẫu xe"].ToString() == "" ? cxd : p["Tên loại xe"].ToString() == "" ? cxd : p["Tên loại xe"].ToString(),
                SoMay = p["Số máy"].ToString(),
                SoKhung = p["Số khung"].ToString(),
                NamSanXuat = p["Năm sản xuất"].ToString() == "" ? 0 : Int32.TryParse(p["Năm sản xuất"].ToString(), out namsanxuatresult) ? namsanxuatresult : -1,
                MauSon = p["Màu sơn"].ToString(),
                DungTich = p["Dung tích"].ToString(),
                HopSo = p["Hộp số"].ToString(),
                MaKhachHang = p["Mã khách hàng (Chủ xe)"].ToString(),
                GhiChu = p["Ghi chú"].ToString(),
                index = dt.Rows.IndexOf(p) + 4
            }).ToList();
            list = list.Where(p => !ErrorIndex.Contains(p.index)).ToList();
            if (list.Count > 0)
            {
                bool checkImport = false;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    string account = CookieStore.GetCookieAes("Account");
                    HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                    ClassXe classXe = new ClassXe(db);
                    checkImport = classXe.ImportDanhSachXe(list, nguoidung.TaiKhoan);
                }
                if (checkImport)
                {
                    return ActionTrueNotData("Nhập file thành công");
                }
                else
                {
                    return ActionFalseWithData(new
                    {
                        ErrorList = lstError,
                        Message = "Nhập dữ liệu thất bại"
                    });
                }
            }
            return ActionFalseWithData(new
            {
                ErrorList = lstError,
                Message = "Nhập dữ liệu thất bại"
            });
        }
        #endregion

        #region Theo dõi nhật ký hoạt động xe
        [HttpPost]
        public IHttpActionResult POST_PhieuBanGiao([FromBody] JObject data)
        {
            try
            {
                Guid IdDonVi = Guid.Empty;
                if (data["IdDonVi"] != null && data["IdDonVi"].ToObject<string>() != "")
                {
                    IdDonVi = data["IdDonVi"].ToObject<Guid>();
                }
                string MaPhieu = "";
                if (data["MaPhieu"] != null && data["MaPhieu"].ToObject<string>() != "")
                {
                    MaPhieu = data["MaPhieu"].ToObject<string>();
                }
                Guid IdXe = Guid.Empty;
                if (data["IdXe"] != null && data["IdXe"].ToObject<string>() != "")
                {
                    IdXe = data["IdXe"].ToObject<Guid>();
                }
                Guid? IdKhachHang = null;
                if (data["IdKhachHang"] != null && data["IdKhachHang"].ToObject<string>() != "")
                {
                    IdKhachHang = data["IdKhachHang"].ToObject<Guid>();
                }
                Guid? IdNhanVien = null;
                if (data["IdNhanVien"] != null && data["IdNhanVien"].ToObject<string>() != "")
                {
                    IdNhanVien = data["IdNhanVien"].ToObject<Guid>();
                }
                Guid IdNhanVienGiao = Guid.Empty;
                if (data["IdNhanVienGiao"] != null && data["IdNhanVienGiao"].ToObject<string>() != "")
                {
                    IdNhanVienGiao = data["IdNhanVienGiao"].ToObject<Guid>();
                }
                DateTime ThoiGianGiao = DateTime.Now;
                if (data["ThoiGianGiao"] != null && data["ThoiGianGiao"].ToObject<string>() != "")
                {
                    ThoiGianGiao = data["ThoiGianGiao"].ToObject<DateTime>();
                }
                string GhiChu = "";
                if (data["GhiChu"] != null && data["GhiChu"].ToObject<string>() != "")
                {
                    GhiChu = data["GhiChu"].ToObject<string>();
                }
                int SoKmHienTai = 0;
                if (data["SoKmHienTai"] != null && data["SoKmHienTai"].ToObject<string>() != "")
                {
                    SoKmHienTai = data["SoKmHienTai"].ToObject<int>();
                }
                int LaNhanVien = 0;
                if (data["LaNhanVien"] != null && data["LaNhanVien"].ToObject<string>() != "")
                {
                    LaNhanVien = data["LaNhanVien"].ToObject<int>();
                }
                string NguoiTao = "";
                if (data["NguoiTao"] != null && data["NguoiTao"].ToObject<string>() != "")
                {
                    NguoiTao = data["NguoiTao"].ToObject<string>();
                }
                Gara_Xe_PhieuBanGiao pbg = new Gara_Xe_PhieuBanGiao();
                bool checkinsert = false;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    libDM_DoiTuong.ClassBH_HoaDon classhd = new libDM_DoiTuong.ClassBH_HoaDon(db);
                    ClassBanGiaoXe classbangiaoxe = new ClassBanGiaoXe(db);
                    if (MaPhieu == "")
                    {
                        MaPhieu = classhd.SP_GetMaHoaDon_byTemp(30, IdDonVi, ThoiGianGiao);
                    }
                    else
                    {
                        MaPhieu = MaPhieu.Trim();
                        if (classbangiaoxe.CheckExistMaPhieu(MaPhieu))
                        {
                            return ActionFalseNotData("Mã phiếu đã tồn tại.");
                        }
                    }
                    pbg.Id = Guid.NewGuid();
                    pbg.MaPhieu = MaPhieu;
                    pbg.IdXe = IdXe;
                    pbg.NgayGiaoXe = ThoiGianGiao;
                    pbg.SoKmBanGiao = SoKmHienTai;
                    pbg.IdNhanVienBanGiao = IdNhanVienGiao;
                    pbg.IdNhanVien = IdNhanVien;
                    pbg.IdKhachHang = IdKhachHang;
                    pbg.LaNhanVien = LaNhanVien;
                    pbg.GhiChuBanGiao = GhiChu;
                    pbg.TrangThai = 1;
                    pbg.NgayTaoBanGiao = DateTime.Now;
                    pbg.NguoiTaoBanGiao = NguoiTao;
                    checkinsert = classbangiaoxe.InsertPhieuBanGiao(pbg);
                }
                if (checkinsert)
                {
                    return ActionTrueData(new
                    {
                        Id = pbg.Id,
                        MaPhieu = MaPhieu
                    });
                }
                else
                {
                    return ActionFalseNotData("InsertPhieuBanGiao failed!");
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult PUT_PhieuBanGiao([FromBody] JObject data)
        {
            try
            {
                Guid Id = Guid.Empty;
                if (data["Id"] != null && data["Id"].ToObject<string>() != "")
                {
                    Id = data["Id"].ToObject<Guid>();
                }
                Guid IdDonVi = Guid.Empty;
                if (data["IdDonVi"] != null && data["IdDonVi"].ToObject<string>() != "")
                {
                    IdDonVi = data["IdDonVi"].ToObject<Guid>();
                }
                string MaPhieu = "";
                if (data["MaPhieu"] != null && data["MaPhieu"].ToObject<string>() != "")
                {
                    MaPhieu = data["MaPhieu"].ToObject<string>();
                }
                Guid IdXe = Guid.Empty;
                if (data["IdXe"] != null && data["IdXe"].ToObject<string>() != "")
                {
                    IdXe = data["IdXe"].ToObject<Guid>();
                }
                Guid? IdKhachHang = null;
                if (data["IdKhachHang"] != null && data["IdKhachHang"].ToObject<string>() != "")
                {
                    IdKhachHang = data["IdKhachHang"].ToObject<Guid>();
                }
                Guid? IdNhanVien = null;
                if (data["IdNhanVien"] != null && data["IdNhanVien"].ToObject<string>() != "")
                {
                    IdKhachHang = data["IdNhanVien"].ToObject<Guid>();
                }
                Guid IdNhanVienGiao = Guid.Empty;
                if (data["IdNhanVienGiao"] != null && data["IdNhanVienGiao"].ToObject<string>() != "")
                {
                    IdNhanVienGiao = data["IdNhanVienGiao"].ToObject<Guid>();
                }
                DateTime ThoiGianGiao = DateTime.Now;
                if (data["ThoiGianGiao"] != null && data["ThoiGianGiao"].ToObject<string>() != "")
                {
                    ThoiGianGiao = data["ThoiGianGiao"].ToObject<DateTime>();
                }
                string GhiChu = "";
                if (data["GhiChu"] != null && data["GhiChu"].ToObject<string>() != "")
                {
                    GhiChu = data["GhiChu"].ToObject<string>();
                }
                int SoKmHienTai = 0;
                if (data["SoKmHienTai"] != null && data["SoKmHienTai"].ToObject<string>() != "")
                {
                    SoKmHienTai = data["SoKmHienTai"].ToObject<int>();
                }
                int LaNhanVien = 0;
                if (data["LaNhanVien"] != null && data["LaNhanVien"].ToObject<string>() != "")
                {
                    LaNhanVien = data["LaNhanVien"].ToObject<int>();
                }
                string NguoiTao = "";
                if (data["NguoiTao"] != null && data["NguoiTao"].ToObject<string>() != "")
                {
                    NguoiTao = data["NguoiTao"].ToObject<string>();
                }
                //int TrangThai = 1;
                //if (data["TrangThai"] != null && data["TrangThai"].ToObject<string>() != "")
                //{
                //    TrangThai = data["TrangThai"].ToObject<int>();
                //}
                Gara_Xe_PhieuBanGiao pbg = new Gara_Xe_PhieuBanGiao();
                bool checkinsert = false;
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    libDM_DoiTuong.ClassBH_HoaDon classhd = new libDM_DoiTuong.ClassBH_HoaDon(db);
                    Gara_Xe_PhieuBanGiao pbgOld = db.Gara_Xe_PhieuBanGiao.Where(p => p.Id == Id).FirstOrDefault();
                    if (pbgOld == null)
                    {
                        return ActionFalseNotData("Không tồn tại phiếu bàn giao.");
                    }
                    else
                    {
                        Guid IdKhachHangOld = Guid.Empty;
                        if (pbgOld.LaNhanVien == 0 && pbgOld.IdKhachHang != null)
                        {
                            IdKhachHangOld = pbgOld.IdKhachHang.Value;
                        }
                        ClassBanGiaoXe classbangiaoxe = new ClassBanGiaoXe(db);
                        if (MaPhieu == "")
                        {
                            MaPhieu = classhd.SP_GetMaHoaDon_byTemp(30, IdDonVi, ThoiGianGiao);
                        }
                        else
                        {
                            MaPhieu = MaPhieu.Trim();
                            if (MaPhieu != pbgOld.MaPhieu)
                            {
                                if (classbangiaoxe.CheckExistMaPhieu(MaPhieu))
                                {
                                    return ActionFalseNotData("Mã phiếu đã tồn tại.");
                                }
                            }
                        }
                        pbg.Id = Id;
                        pbg.MaPhieu = MaPhieu;
                        pbg.IdXe = IdXe;
                        pbg.NgayGiaoXe = ThoiGianGiao;
                        pbg.SoKmBanGiao = SoKmHienTai;
                        pbg.IdNhanVienBanGiao = IdNhanVienGiao;
                        pbg.IdNhanVien = IdNhanVien;
                        pbg.IdKhachHang = IdKhachHang;
                        pbg.LaNhanVien = LaNhanVien;
                        pbg.GhiChuBanGiao = GhiChu;
                        //pbg.TrangThai = TrangThai;
                        pbg.NgaySuaBanGiao = DateTime.Now;
                        pbg.NguoiSuaBanGiao = NguoiTao;
                        checkinsert = classbangiaoxe.UpdatePhieubanGiao(pbg);
                        if (checkinsert)
                        {
                            if (LaNhanVien == 0)
                            {
                                if (pbg.IdKhachHang != null && pbg.IdKhachHang != IdKhachHangOld)
                                {
                                    classbangiaoxe.UpdateKhachHangNhatKyHoatDong(pbg.Id, pbg.IdKhachHang.Value);
                                }
                            }
                        }
                    }
                }
                if (checkinsert)
                {
                    return ActionTrueData(new
                    {
                        Id = pbg.Id,
                        MaPhieu = MaPhieu
                    });
                }
                else
                {
                    return ActionFalseNotData("UpdatePhieuBanGiao failed!");
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult PUT_HoanThanhPhieuBanGiao([FromBody] JObject data)
        {
            try
            {
                Guid Id = Guid.Empty;
                if (data["Id"] != null && data["Id"].ToObject<string>() != "")
                {
                    Id = data["Id"].ToObject<Guid>();
                }

                Guid IdNhanVienNhan = Guid.Empty;
                if (data["IdNhanVienNhan"] != null && data["IdNhanVienNhan"].ToObject<string>() != "")
                {
                    IdNhanVienNhan = data["IdNhanVienNhan"].ToObject<Guid>();
                }
                string GhiChu = "";
                if (data["GhiChu"] != null && data["GhiChu"].ToObject<string>() != "")
                {
                    GhiChu = data["GhiChu"].ToObject<string>();
                }
                DateTime ThoiGianNhan = DateTime.Now;
                if (data["ThoiGianNhan"] != null && data["ThoiGianNhan"].ToObject<string>() != "")
                {
                    ThoiGianNhan = data["ThoiGianNhan"].ToObject<DateTime>();
                }
                string UserName = "";
                if (data["UserName"] != null && data["UserName"].ToObject<string>() != "")
                {
                    UserName = data["UserName"].ToObject<string>();
                }
                bool isNew = true;
                if (data["isNew"] != null && data["isNew"].ToObject<string>() != "")
                {
                    isNew = data["isNew"].ToObject<bool>();
                }

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Gara_Xe_PhieuBanGiao pbg = new Gara_Xe_PhieuBanGiao();
                    pbg = db.Gara_Xe_PhieuBanGiao.Where(p => p.Id == Id).FirstOrDefault();
                    if (pbg != null)
                    {
                        pbg.IdNhanVienTiepNhan = IdNhanVienNhan;
                        pbg.GhiChuTiepNhan = GhiChu;
                        pbg.NgayNhanXe = ThoiGianNhan;
                        if (isNew)
                        {
                            pbg.NguoiTaoTiepNhan = UserName;
                            pbg.NgayTaoTiepNhan = DateTime.Now;
                            pbg.TrangThai = 2;
                        }
                        else
                        {
                            pbg.NguoiSuaTiepNhan = UserName;
                            pbg.NgaySuaTiepNhan = DateTime.Now;
                        }
                        db.SaveChanges();
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
        public IHttpActionResult UpdateTrangThaiPhieuBanGiao([FromBody] JObject data)
        {
            try
            {
                Guid Id = Guid.Empty;
                if (data["Id"] != null && data["Id"].ToObject<string>() != "")
                {
                    Id = data["Id"].ToObject<Guid>();
                }
                int TrangThai = 1;
                if (data["TrangThai"] != null && data["TrangThai"].ToObject<string>() != "")
                {
                    TrangThai = data["TrangThai"].ToObject<int>();
                }
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Gara_Xe_PhieuBanGiao pbg = new Gara_Xe_PhieuBanGiao();
                    pbg = db.Gara_Xe_PhieuBanGiao.Where(p => p.Id == Id).FirstOrDefault();
                    if (pbg != null)
                    {
                        pbg.TrangThai = TrangThai;
                        db.SaveChanges();
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
        public IHttpActionResult GetListPhieuBanGiao_v1([FromBody] JObject data)
        {
            try
            {
                GetListPhieuBanGiao_v1_Input lstInput = new GetListPhieuBanGiao_v1_Input();
                if (data["NgayGiaoXeFrom"] != null && data["NgayGiaoXeFrom"].ToObject<string>() != "")
                {
                    lstInput.NgayGiaoXeFrom = data["NgayGiaoXeFrom"].ToObject<DateTime>();
                }
                if (data["NgayGiaoXeTo"] != null && data["NgayGiaoXeTo"].ToObject<string>() != "")
                {
                    lstInput.NgayGiaoXeTo = data["NgayGiaoXeTo"].ToObject<DateTime>();
                }
                if (data["TrangThais"] != null)
                {
                    lstInput.TrangThais = data["TrangThais"].ToObject<List<int>>();
                }
                if (data["TextSearch"] != null)
                    lstInput.TextSearch = data["TextSearch"].ToObject<string>();
                if (data["CurrentPage"] != null)
                    lstInput.CurrentPage = data["CurrentPage"].ToObject<int>() - 1;
                if (data["PageSize"] != null)
                    lstInput.PageSize = data["PageSize"].ToObject<int>();

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBanGiaoXe classbangiaoxe = new ClassBanGiaoXe(db);
                    List<GetListPhieuBanGiao_v1_Result> dataphieubangiao = classbangiaoxe.GetListPhieuBanGiao_v1(lstInput);
                    int count = 0;
                    if (dataphieubangiao.Count != 0)
                    {
                        count = dataphieubangiao[0].TotalRow;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, lstInput.PageSize, lstInput.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = dataphieubangiao,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((lstInput.CurrentPage) * lstInput.PageSize + 1) + " - " + ((lstInput.CurrentPage) * lstInput.PageSize + dataphieubangiao.Count) + " trên tổng số " + count + " bản ghi",
                        NumberOfPage = page
                    });
                }

                return ActionTrueNotData("");
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult GetNhatKyHoatDongByIdPhieuBanGiao([FromBody] JObject data)
        {
            try
            {
                GetListNhatKyByIdPhieuBanGiao_v1_Input input = new GetListNhatKyByIdPhieuBanGiao_v1_Input();
                if (data["IdPhieuBanGiao"] != null && data["IdPhieuBanGiao"].ToObject<string>() != "")
                    input.IdPhieuBanGiao = data["IdPhieuBanGiao"].ToObject<Guid>();
                if (data["TrangThais"] != null)
                {
                    input.TrangThais = data["TrangThais"].ToObject<List<int>>();
                }
                if (data["CurrentPage"] != null)
                    input.CurrentPage = data["CurrentPage"].ToObject<int>() - 1;
                if (data["PageSize"] != null)
                    input.PageSize = data["PageSize"].ToObject<int>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBanGiaoXe classbangiaoxe = new ClassBanGiaoXe(db);
                    List<GetListNhatKyByIdPhieuBanGiao_v1_Result> datanhatky = classbangiaoxe.GetListNhatKyByIdPhieuBanGiao_v1(input);
                    int count = 0;
                    if (datanhatky.Count != 0)
                    {
                        count = datanhatky[0].TotalRow;
                    }
                    int page = 0;
                    var listpage = GetListPage(count, input.PageSize, input.CurrentPage + 1, ref page);
                    return ActionTrueData(new
                    {
                        data = datanhatky,
                        ListPage = listpage,
                        PageView = "Hiển thị " + ((input.CurrentPage) * input.PageSize + 1) + " - " + ((input.CurrentPage) * input.PageSize + datanhatky.Count) + " trên tổng số " + count + " bản ghi",
                        NumberOfPage = page
                    });
                }
                return ActionTrueNotData("");
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult POST_NhatKyHoatDong([FromBody] JObject data)
        {
            try
            {
                NhatKyHoatDong_Input input = new NhatKyHoatDong_Input();
                if (data["IdPhieuBanGiao"] != null && data["IdPhieuBanGiao"].ToObject<string>() != "")
                    input.IdPhieuBanGiao = data["IdPhieuBanGiao"].ToObject<Guid>();
                if (data["IdNhanVienThucHien"] != null && data["IdNhanVienThucHien"].ToObject<string>() != "")
                    input.IdNhanVienThucHien = data["IdNhanVienThucHien"].ToObject<Guid>();
                if (data["IdKhachHang"] != null && data["IdKhachHang"].ToObject<string>() != "")
                    input.IdKhachHang = data["IdKhachHang"].ToObject<Guid>();
                if (data["LaNhanVien"] != null && data["LaNhanVien"].ToObject<string>() != "")
                    input.LaNhanVien = data["LaNhanVien"].ToObject<int>();
                if (data["ThoiGianHoatDong"] != null && data["ThoiGianHoatDong"].ToObject<string>() != "")
                    input.ThoiGianHoatDong = data["ThoiGianHoatDong"].ToObject<DateTime>();
                if (data["SoGioHoatDong"] != null && data["SoGioHoatDong"].ToObject<string>() != "")
                    input.SoGioHoatDong = data["SoGioHoatDong"].ToObject<double>();
                if (data["SoKmHienTai"] != null && data["SoKmHienTai"].ToObject<string>() != "")
                    input.SoKmHienTai = data["SoKmHienTai"].ToObject<int>();
                if (data["GhiChu"] != null && data["GhiChu"].ToObject<string>() != "")
                    input.GhiChu = data["GhiChu"].ToObject<string>();
                if (data["UserName"] != null && data["UserName"].ToObject<string>() != "")
                    input.UserName = data["UserName"].ToObject<string>();
                bool checkinsert = false;
                Gara_Xe_NhatKyHoatDong nky = new Gara_Xe_NhatKyHoatDong();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBanGiaoXe classbangiaoxe = new ClassBanGiaoXe(db);
                    nky.Id = Guid.NewGuid();
                    nky.IdPhieuBanGiao = input.IdPhieuBanGiao;
                    nky.IdNhanVienThucHien = input.IdNhanVienThucHien;
                    nky.IdKhachHang = input.IdKhachHang;
                    nky.LaNhanVien = input.LaNhanVien;
                    nky.ThoiGianHoatDong = input.ThoiGianHoatDong;
                    nky.SoGioHoatDong = input.SoGioHoatDong;
                    nky.SoKmHienTai = input.SoKmHienTai;
                    nky.GhiChu = input.GhiChu;
                    nky.TrangThai = 1;
                    nky.NgayTao = DateTime.Now;
                    nky.NguoiTao = input.UserName;
                    checkinsert = classbangiaoxe.InsertNhatKyHoatDong(nky);
                }
                if (checkinsert)
                {
                    return ActionTrueData(new
                    {
                        Id = nky.Id
                    });
                }
                else
                {
                    return ActionFalseNotData("InsertPhieuBanGiao failed!");
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult PUT_NhatKyHoatDong([FromBody] JObject data)
        {
            try
            {
                NhatKyHoatDong_Input input = new NhatKyHoatDong_Input();
                if (data["Id"] != null && data["Id"].ToObject<string>() != "")
                    input.Id = data["Id"].ToObject<Guid>();
                if (data["IdNhanVienThucHien"] != null && data["IdNhanVienThucHien"].ToObject<string>() != "")
                    input.IdNhanVienThucHien = data["IdNhanVienThucHien"].ToObject<Guid>();
                if (data["IdKhachHang"] != null && data["IdKhachHang"].ToObject<string>() != "")
                    input.IdKhachHang = data["IdKhachHang"].ToObject<Guid>();
                if (data["LaNhanVien"] != null && data["LaNhanVien"].ToObject<string>() != "")
                    input.LaNhanVien = data["LaNhanVien"].ToObject<int>();
                if (data["ThoiGianHoatDong"] != null && data["ThoiGianHoatDong"].ToObject<string>() != "")
                    input.ThoiGianHoatDong = data["ThoiGianHoatDong"].ToObject<DateTime>();
                if (data["SoGioHoatDong"] != null && data["SoGioHoatDong"].ToObject<string>() != "")
                    input.SoGioHoatDong = data["SoGioHoatDong"].ToObject<double>();
                if (data["SoKmHienTai"] != null && data["SoKmHienTai"].ToObject<string>() != "")
                    input.SoKmHienTai = data["SoKmHienTai"].ToObject<int>();
                if (data["GhiChu"] != null && data["GhiChu"].ToObject<string>() != "")
                    input.GhiChu = data["GhiChu"].ToObject<string>();
                if (data["UserName"] != null && data["UserName"].ToObject<string>() != "")
                    input.UserName = data["UserName"].ToObject<string>();
                bool checkinsert = false;
                Gara_Xe_NhatKyHoatDong nky = new Gara_Xe_NhatKyHoatDong();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBanGiaoXe classbangiaoxe = new ClassBanGiaoXe(db);
                    nky.Id = input.Id;
                    nky.IdNhanVienThucHien = input.IdNhanVienThucHien;
                    nky.IdKhachHang = input.IdKhachHang;
                    nky.LaNhanVien = input.LaNhanVien;
                    nky.ThoiGianHoatDong = input.ThoiGianHoatDong;
                    nky.SoGioHoatDong = input.SoGioHoatDong;
                    nky.SoKmHienTai = input.SoKmHienTai;
                    nky.GhiChu = input.GhiChu;
                    nky.NgayTao = DateTime.Now;
                    nky.NguoiTao = input.UserName;
                    checkinsert = classbangiaoxe.UpdateNhatKyHoatDong(nky);
                }
                if (checkinsert)
                {
                    return ActionTrueData(new
                    {
                        Id = nky.Id
                    });
                }
                else
                {
                    return ActionFalseNotData("InsertPhieuBanGiao failed!");
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteNhatKyHoatDong([FromBody] JObject data)
        {
            try
            {
                Guid Id = Guid.Empty;
                if (data["Id"] != null && data["Id"].ToObject<string>() != "")
                    Id = data["Id"].ToObject<Guid>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Gara_Xe_NhatKyHoatDong nky = new Gara_Xe_NhatKyHoatDong();
                    nky = db.Gara_Xe_NhatKyHoatDong.Where(p => p.Id == Id).FirstOrDefault();
                    if (nky != null)
                    {
                        nky.TrangThai = 0;
                        db.SaveChanges();
                    }
                    else
                    {
                        return ActionFalseNotData("");
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
        public IHttpActionResult GetListPhuTungTheoDoiByIdXe_v1([FromBody] JObject data)
        {
            try
            {
                Guid IdXe = Guid.Empty;
                if (data["IdXe"] != null && data["IdXe"].ToObject<string>() != "")
                    IdXe = data["IdXe"].ToObject<Guid>();
                List<GetListPhuTungTheoDoiByIdXe_v1_Result> lstResult = new List<GetListPhuTungTheoDoiByIdXe_v1_Result>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassBanGiaoXe classbangiao = new ClassBanGiaoXe(db);
                    lstResult = classbangiao.GetListPhuTungTheoDoiByIdXe_v1(IdXe);
                }
                return ActionTrueData(new
                {
                    data = lstResult
                });
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        #endregion
    }
}