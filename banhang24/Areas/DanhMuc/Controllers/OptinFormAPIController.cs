using banhang24.Models;
using libDM_DoiTuong;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class OptinFormAPIController : ApiController
    {
        #region lấy danh sách thông tin
        public int getNumber_Page(float sohang, int pageSize)
        {
            if (sohang > 0)
            {
                float SoTrang = sohang / pageSize;
                if (SoTrang > (int)SoTrang)
                    return (int)SoTrang + 1;
                else
                    return (int)SoTrang;
            }
            else
            {
                return 0;
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getlist_TruongThongTinOF(Guid? ID_OptinForm, int LoaiTruongThongTin)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<OF_TruongThongTinPROC> lst = new List<OF_TruongThongTinPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            if (ID_OptinForm == null)
                sql.Add(new SqlParameter("ID_OptinForm", DBNull.Value));
            else
                sql.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
            sql.Add(new SqlParameter("LoaiTruongThongTin", LoaiTruongThongTin));
            lst = db.Database.SqlQuery<OF_TruongThongTinPROC>("exec getList_TruongThongTinOF @ID_OptinForm, @LoaiTruongThongTin", sql.ToArray()).ToList();
            JsonResultExample<OF_TruongThongTinPROC> json = new JsonResultExample<OF_TruongThongTinPROC>
            {
                LstData = lst
            };
            return Json(json);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getlist_OptinForm(int LoaiOF)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<OF_OptinFormPROC> lst = new List<OF_OptinFormPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("LoaiOF", LoaiOF));
            lst = db.Database.SqlQuery<OF_OptinFormPROC>("exec getList_OptinFrom @LoaiOF", sql.ToArray()).ToList();
            int Rown = lst.Count();
            int lstPages = getNumber_Page(Rown, 10);
            double SoLuotTruyCap = lst.Sum(x => x.SoLuotTruyCap);
            double SoLuotDangKy = lst.Sum(x => x.SoLuotDangKy);
            JsonResultExampleTr<OF_OptinFormPROC> json = new JsonResultExampleTr<OF_OptinFormPROC>
            {
                LstData = lst,
                Rowcount = Rown,
                numberPage = lstPages,
                a1 = Math.Round(SoLuotTruyCap, 3, MidpointRounding.ToEven),
                a2 = Math.Round(SoLuotDangKy, 0, MidpointRounding.ToEven),
            };
            return Json(json);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_linkKhachHangDangKyOF(Guid ID_OptinForm)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<OF_linkKhachHangDangKyPROC> lst = new List<OF_linkKhachHangDangKyPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
            lst = db.Database.SqlQuery<OF_linkKhachHangDangKyPROC>("exec getList_linkKhachHangDangKyOF @ID_OptinForm", sql.ToArray()).ToList();
            JsonResultExample<OF_linkKhachHangDangKyPROC> json = new JsonResultExample<OF_linkKhachHangDangKyPROC>
            {
                LstData = lst
            };
            return Json(json);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_BieuDoDangKyOF(Guid ID_OptinForm, DateTime timeStart, DateTime timeEnd)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<OF_BieuDoDangKyPROC> lst = new List<OF_BieuDoDangKyPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
            sql.Add(new SqlParameter("timeStart", timeStart));
            sql.Add(new SqlParameter("timeEnd", timeEnd));
            lst = db.Database.SqlQuery<OF_BieuDoDangKyPROC>("exec getList_BieuDoDangKyOF @ID_OptinForm, @timeStart, @timeEnd", sql.ToArray()).ToList();
            JsonResultExample<OF_BieuDoDangKyPROC> json = new JsonResultExample<OF_BieuDoDangKyPROC>
            {
                LstData = lst
            };
            return Json(json);
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_KhachHangOptinForm(array_DoiTuongOptinForm array_DoiTuongOptinForm)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<OF_DoiTuongPRC> lst = new List<OF_DoiTuongPRC>();
            string TenKhachHang = "%%";
            if (array_DoiTuongOptinForm.TenKhachHang != null & array_DoiTuongOptinForm.TenKhachHang != "" & array_DoiTuongOptinForm.TenKhachHang != "null")
                TenKhachHang = "%" + CommonStatic.ConvertToUnSign(array_DoiTuongOptinForm.TenKhachHang).ToLower() + "%";
            List<SqlParameter> sql = new List<SqlParameter>();
            if (array_DoiTuongOptinForm.ID_OptinFrom == null)
            sql.Add(new SqlParameter("ID_OptinForm", DBNull.Value));
            else
                sql.Add(new SqlParameter("ID_OptinForm", array_DoiTuongOptinForm.ID_OptinFrom));
            sql.Add(new SqlParameter("TenKhachHang", TenKhachHang));
            sql.Add(new SqlParameter("TimeStart", array_DoiTuongOptinForm.TimeStart));
            sql.Add(new SqlParameter("TimeEnd", array_DoiTuongOptinForm.TimeEnd));
            sql.Add(new SqlParameter("LoaiOptinForm", array_DoiTuongOptinForm.LoaiOptinForm));
            sql.Add(new SqlParameter("TrangThaiXuLy", array_DoiTuongOptinForm.TrangThaiXuLy));
            sql.Add(new SqlParameter("TrangThaiChuaXuLy", array_DoiTuongOptinForm.TrangThaiChuaXuLy));
            sql.Add(new SqlParameter("TrangThaiHuyBo", array_DoiTuongOptinForm.TrangThaiHuyBo));
            sql.Add(new SqlParameter("TrangThaiFromBat", array_DoiTuongOptinForm.TrangThaiFromBat));
            sql.Add(new SqlParameter("TrangThaiFromTat", array_DoiTuongOptinForm.TrangThaiFromTat));
            sql.Add(new SqlParameter("TrangThaiFromXoa", array_DoiTuongOptinForm.TrangThaiFromXoa));
            lst = db.Database.SqlQuery<OF_DoiTuongPRC>("exec getList_DoiTuongOptinForm @ID_OptinForm, @TenKhachHang, @TimeStart, @TimeEnd, @LoaiOptinForm, @TrangThaiXuLy, @TrangThaiChuaXuLy, @TrangThaiHuyBo, @TrangThaiFromBat, @TrangThaiFromTat, @TrangThaiFromXoa", sql.ToArray()).ToList();
            int Rown = lst.Count();
            int lstPages = getNumber_Page(Rown, array_DoiTuongOptinForm.paperSize);
            JsonResultExampleTr<OF_DoiTuongPRC> json = new JsonResultExampleTr<OF_DoiTuongPRC>
            {
                LstData = lst,
                Rowcount = Rown,
                numberPage = lstPages,
            };
            return Json(json);
        }
        [AcceptVerbs("GET", "POST")]
        public string get_SubDomain()
        {
            string sub = CookieStore.GetCookieAes("SubDomain");
            return sub;
        }
        #endregion
        #region insert
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostOF_KhachHang([FromBody]JObject data, Guid ID_NhanVien, Guid ID_DonVi)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            OptinForm optinForm = data["OptinForm"].ToObject<OptinForm>();
            string TenKhongDau = CommonStatic.ConvertToUnSign(optinForm.TenOptinForm.Trim());
            string TenChuCaiDau = CommonStatic.GetCharsStart(optinForm.TenOptinForm.Trim());
            string loaiEdit = "tạo";
            try
            {
                // insert optinForm
                Guid ID_OptinForm = Guid.NewGuid();
                string MaNhungOF = optinForm.MaNhung.Replace(",null", "," + ID_OptinForm.ToString());
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID", ID_OptinForm));
                sqlPRM.Add(new SqlParameter("TenOpntinFrom", optinForm.TenOptinForm));
                sqlPRM.Add(new SqlParameter("TenOpntinFrom_KhongDau", TenKhongDau));
                sqlPRM.Add(new SqlParameter("TenOpntinFrom_ChuCaiDau", TenChuCaiDau));
                sqlPRM.Add(new SqlParameter("LoaiOptinForm", optinForm.LoaiOptinForm));
                sqlPRM.Add(new SqlParameter("NoiDung", optinForm.NoiDung == null ? "" : optinForm.NoiDung));
                sqlPRM.Add(new SqlParameter("MaNhung", MaNhungOF));
                if (optinForm.TuNgay == null)
                    sqlPRM.Add(new SqlParameter("TuNgay", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("TuNgay", optinForm.TuNgay));
                if (optinForm.DenNgay == null)
                    sqlPRM.Add(new SqlParameter("DenNgay", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("DenNgay", optinForm.DenNgay));
                sqlPRM.Add(new SqlParameter("TrangThaiThoiGian", optinForm.TrangThaiThoiGian));
                sqlPRM.Add(new SqlParameter("LoaiThoiGian", optinForm.LoaiThoiGian));
                sqlPRM.Add(new SqlParameter("KhoangCachThoiGian", optinForm.KhoangCachThoiGian));
                sqlPRM.Add(new SqlParameter("NguoiTao", optinForm.NguoiTao));
                sqlPRM.Add(new SqlParameter("GhiChu", optinForm.GhiChu == null ? "" : optinForm.GhiChu));
                db.Database.ExecuteSqlCommand("exec Insert_OptinFrom @ID, @TenOpntinFrom, @TenOpntinFrom_KhongDau, @TenOpntinFrom_ChuCaiDau, @LoaiOptinForm, @NoiDung, @MaNhung, @TuNgay, @DenNgay, " +
                    "@TrangThaiThoiGian, @LoaiThoiGian, @KhoangCachThoiGian, @NguoiTao, @GhiChu", sqlPRM.ToArray());
                //insert thiết lập trường thông tin
                List<OF_TruongThongTinPROC> objThietLapTruongThongTin = data["objThietLapTruongThongTin"].ToObject<List<OF_TruongThongTinPROC>>();
                foreach (var item in objThietLapTruongThongTin)
                {
                    List<SqlParameter> prm = new List<SqlParameter>();
                    prm.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
                    prm.Add(new SqlParameter("ID_TruongThongTin", item.ID));
                    prm.Add(new SqlParameter("TrangThaiSuDung", item.checkSuDung));
                    prm.Add(new SqlParameter("TrangThaiBatBuoc", item.checkBatBuoc));
                    prm.Add(new SqlParameter("HienThiGoiY", item.GoiY == null ? "" : item.GoiY));
                    db.Database.ExecuteSqlCommand("exec insert_ThietLapTruongThongTinOF @ID_OptinForm, @ID_TruongThongTin, @TrangThaiSuDung, @TrangThaiBatBuoc, @HienThiGoiY", prm.ToArray());
                }
                //insert thiết lập thông báo
                OptinForm_ThietLapThongBao objThietLapThongBao = data["objThietLapThongBao"].ToObject<OptinForm_ThietLapThongBao>();
                List<SqlParameter> prmtb = new List<SqlParameter>();
                prmtb.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
                prmtb.Add(new SqlParameter("NoiDungThongBao", objThietLapThongBao.NoiDungThongBao == null ? "" : objThietLapThongBao.NoiDungThongBao));
                prmtb.Add(new SqlParameter("WebDieuHuong", objThietLapThongBao.WebDieuHuong == null ? "" : objThietLapThongBao.WebDieuHuong));
                prmtb.Add(new SqlParameter("NoiDungHieuLuc", objThietLapThongBao.NoiDungHieuLuc == null ? "" : objThietLapThongBao.NoiDungHieuLuc));
                prmtb.Add(new SqlParameter("ButtonName", objThietLapThongBao.ButtonName == null ? "" : objThietLapThongBao.ButtonName));
                db.Database.ExecuteSqlCommand("exec insert_ThietLapThongBaoOF @ID_OptinForm, @NoiDungThongBao, @WebDieuHuong, @NoiDungHieuLuc, @ButtonName", prmtb.ToArray());
                #region NhatKySuDung
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                {
                    ID = Guid.NewGuid(),
                    ID_NhanVien = ID_NhanVien,
                    ID_DonVi = ID_DonVi,
                    ChucNang = "OptinForm",
                    ThoiGian = DateTime.Now,
                    NoiDung = loaiEdit + " optinForm: " + optinForm.TenOptinForm,
                    NoiDungChiTiet = loaiEdit + " optinForm: <a style= \"cursor: pointer\" onclick = \"loadOptinForm('" + optinForm.TenOptinForm + "')\" >" + optinForm.TenOptinForm + "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    LoaiNhatKy = 1
                };
                string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                #endregion
                optinForm.ID = ID_OptinForm;
                return CreatedAtRoute("DefaultApi", new { id = ID_OptinForm }, optinForm);
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
            }
        }
        #endregion
        #region Edit
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult Edit_OptinForm([FromBody]JObject data, Guid ID_NhanVien, Guid ID_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                OptinForm optinForm = data["OptinForm"].ToObject<OptinForm>();
                string loaiEdit = "Cập nhật";
                try
                {
                    string strUpd = classhoadon.Update_OptinForm(optinForm);
                    if (strUpd != null && strUpd != string.Empty)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                    else
                        return StatusCode(HttpStatusCode.NoContent);
                    #region NhatKySuDung
                    string trangthai = optinForm.TrangThai == 1 ? "Áp dụng" : "Không áp dụng";
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "OptinForm",
                        ThoiGian = DateTime.Now,
                        NoiDung = loaiEdit + " optinForm: " + optinForm.TenOptinForm + " sang trạng thái: " + trangthai,
                        NoiDungChiTiet = loaiEdit + " optinForm: <a style= \"cursor: pointer\" onclick = \"loadOptinForm('" + optinForm.TenOptinForm + "')\" >" + optinForm.TenOptinForm + "</a>, Sang trạng thái:" + trangthai + ", Ngày sửa: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        LoaiNhatKy = 1
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    #endregion
                    return CreatedAtRoute("DefaultApi", new { id = optinForm.ID }, optinForm);
                }
                catch (Exception ex)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult PutOF_KhachHang([FromBody]JObject data, Guid ID_NhanVien, Guid ID_DonVi, Guid ID_OptinForm)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            OptinForm optinForm = data["OptinForm"].ToObject<OptinForm>();
            string TenKhongDau = CommonStatic.ConvertToUnSign(optinForm.TenOptinForm.Trim());
            string TenChuCaiDau = CommonStatic.GetCharsStart(optinForm.TenOptinForm.Trim());
            string loaiEdit = "Cập nhật";
            try
            {
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID", ID_OptinForm));
                sqlPRM.Add(new SqlParameter("TenOpntinFrom", optinForm.TenOptinForm));
                sqlPRM.Add(new SqlParameter("TenOpntinFrom_KhongDau", TenKhongDau));
                sqlPRM.Add(new SqlParameter("TenOpntinFrom_ChuCaiDau", TenChuCaiDau));
                sqlPRM.Add(new SqlParameter("NoiDung", optinForm.NoiDung == null ? "" : optinForm.NoiDung));
                sqlPRM.Add(new SqlParameter("MaNhung", optinForm.MaNhung == null ? "" : optinForm.MaNhung));
                if (optinForm.TuNgay == null)
                    sqlPRM.Add(new SqlParameter("TuNgay", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("TuNgay", optinForm.TuNgay));
                if (optinForm.DenNgay == null)
                    sqlPRM.Add(new SqlParameter("DenNgay", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("DenNgay", optinForm.DenNgay));
                sqlPRM.Add(new SqlParameter("TrangThaiThoiGian", optinForm.TrangThaiThoiGian));
                sqlPRM.Add(new SqlParameter("LoaiThoiGian", optinForm.LoaiThoiGian));
                sqlPRM.Add(new SqlParameter("KhoangCachThoiGian", optinForm.KhoangCachThoiGian));
                sqlPRM.Add(new SqlParameter("NguoiTao", optinForm.NguoiTao));
                sqlPRM.Add(new SqlParameter("GhiChu", optinForm.GhiChu == null ? "" : optinForm.GhiChu));
                db.Database.ExecuteSqlCommand("exec Update_OptinFrom @ID, @TenOpntinFrom, @TenOpntinFrom_KhongDau, @TenOpntinFrom_ChuCaiDau, @NoiDung, @MaNhung, @TuNgay, @DenNgay, " +
                    "@TrangThaiThoiGian, @LoaiThoiGian, @KhoangCachThoiGian, @NguoiTao, @GhiChu", sqlPRM.ToArray());
                //insert thiết lập trường thông tin
                List<OF_TruongThongTinPROC> objThietLapTruongThongTin = data["objThietLapTruongThongTin"].ToObject<List<OF_TruongThongTinPROC>>();
                foreach (var item in objThietLapTruongThongTin)
                {
                    List<SqlParameter> prm = new List<SqlParameter>();
                    prm.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
                    prm.Add(new SqlParameter("ID_TruongThongTin", item.ID));
                    prm.Add(new SqlParameter("TrangThaiSuDung", item.checkSuDung));
                    prm.Add(new SqlParameter("TrangThaiBatBuoc", item.checkBatBuoc));
                    prm.Add(new SqlParameter("HienThiGoiY", item.GoiY == null ? "" : item.GoiY));
                    db.Database.ExecuteSqlCommand("exec Update_ThietLapTruongThongTinOF @ID_OptinForm, @ID_TruongThongTin, @TrangThaiSuDung, @TrangThaiBatBuoc, @HienThiGoiY", prm.ToArray());
                }
                //insert thiết lập thông báo
                OptinForm_ThietLapThongBao objThietLapThongBao = data["objThietLapThongBao"].ToObject<OptinForm_ThietLapThongBao>();
                List<SqlParameter> prmtb = new List<SqlParameter>();
                prmtb.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
                prmtb.Add(new SqlParameter("NoiDungThongBao", objThietLapThongBao.NoiDungThongBao == null ? "" : objThietLapThongBao.NoiDungThongBao));
                prmtb.Add(new SqlParameter("WebDieuHuong", objThietLapThongBao.WebDieuHuong == null ? "" : objThietLapThongBao.WebDieuHuong));
                prmtb.Add(new SqlParameter("NoiDungHieuLuc", objThietLapThongBao.NoiDungHieuLuc == null ? "" : objThietLapThongBao.NoiDungHieuLuc));
                prmtb.Add(new SqlParameter("ButtonName", objThietLapThongBao.ButtonName == null ? "" : objThietLapThongBao.ButtonName));
                db.Database.ExecuteSqlCommand("exec Update_ThietLapThongBaoOF @ID_OptinForm, @NoiDungThongBao, @WebDieuHuong, @NoiDungHieuLuc, @ButtonName", prmtb.ToArray());
                #region NhatKySuDung
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                {
                    ID = Guid.NewGuid(),
                    ID_NhanVien = ID_NhanVien,
                    ID_DonVi = ID_DonVi,
                    ChucNang = "OptinForm",
                    ThoiGian = DateTime.Now,
                    NoiDung = loaiEdit + " optinForm: " + optinForm.TenOptinForm,
                    NoiDungChiTiet = loaiEdit + " optinForm: <a style= \"cursor: pointer\" onclick = \"loadOptinForm('" + optinForm.TenOptinForm + "')\" >" + optinForm.TenOptinForm + "</a>, Ngày sửa: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    LoaiNhatKy = 1
                };
                string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                #endregion
                return CreatedAtRoute("DefaultApi", new { id = ID_OptinForm }, optinForm);
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult Delete_OptinForm([FromBody]JObject data, Guid ID_NhanVien, Guid ID_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                OptinForm optinForm = data["OptinForm"].ToObject<OptinForm>();
                string loaiEdit = "Xóa";
                try
                {
                    string strUpd = classhoadon.Delete_OptinForm(optinForm);
                    if (strUpd != null && strUpd != string.Empty)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                    else
                        return StatusCode(HttpStatusCode.NoContent);
                    #region NhatKySuDung
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "OptinForm",
                        ThoiGian = DateTime.Now,
                        NoiDung = loaiEdit + " optinForm: " + optinForm.TenOptinForm,
                        NoiDungChiTiet = loaiEdit + " optinForm: <a style= \"cursor: pointer\" onclick = \"loadOptinForm('" + optinForm.TenOptinForm + "')\" >" + optinForm.TenOptinForm + "</a>, Ngày xóa: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        LoaiNhatKy = 3
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    #endregion
                    return CreatedAtRoute("DefaultApi", new { id = optinForm.ID }, optinForm);
                }
                catch (Exception ex)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
                }
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult Delete_DoiTuongOF([FromBody]JObject data, Guid ID_NhanVien, Guid ID_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                OF_DoiTuongPRC optinForm = data["OptinForm_DoiTuong"].ToObject<OF_DoiTuongPRC>();
                string loaiEdit = "Xóa";
                try
                {
                    string strUpd = classhoadon.Delete_DoiTuongOF(optinForm);
                    if (strUpd != null && strUpd != string.Empty)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                    else
                        return StatusCode(HttpStatusCode.NoContent);
                    #region NhatKySuDung
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "Danh sách đăng ký OptinForm",
                        ThoiGian = DateTime.Now,
                        NoiDung = loaiEdit + " đối tượng đăng ký OptinForm: " + optinForm.TenDoiTuong,
                        NoiDungChiTiet = loaiEdit + " đối tượng đăng ký OptinForm: <a style= \"cursor: pointer\" onclick = \"loadDoiTuongOptinForm('" + optinForm.TenDoiTuong + "')\" >" + optinForm.TenDoiTuong + "</a>, Ngày xóa: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        LoaiNhatKy = 3
                    };
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                    #endregion
                    return CreatedAtRoute("DefaultApi", new { id = optinForm.ID }, optinForm);
                }
                catch (Exception ex)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
                }
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult Update_DoiTuongOF([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassBH_HoaDon classhoadon = new ClassBH_HoaDon(db);
                OF_DoiTuongPRC optinForm = data["OptinForm_DoiTuong"].ToObject<OF_DoiTuongPRC>();
                //string loaiEdit = "Xóa";
                try
                {
                    string strUpd = classhoadon.Update_DoiTuongOF(optinForm);
                    if (strUpd != null && strUpd != string.Empty)
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                    else
                        return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
                }
            }
        }
        #endregion
    }
}
