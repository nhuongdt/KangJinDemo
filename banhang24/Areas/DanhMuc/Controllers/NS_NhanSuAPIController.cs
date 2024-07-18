using banhang24.Hellper;
using banhang24.Models;
using ExcelDataReader;
using iTextSharp.text.pdf.qrcode;
using libDM_DoiTuong;
using libHT_NguoiDung;
using libNS_NhanVien;
using libQuy_HoaDon;
using Model;
using Model.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using static libQuy_HoaDon.Class_Report;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class NS_NhanSuAPIController : BaseApiController
    {

        public NS_NhanSuAPIController()
        {
        }
        #region Danh Mục Ca
        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult PostNS_CaLamViec([FromBody] JObject data, Guid ID_NhanVien, Guid ID_DonVi)
        {

            NS_CaLamViec objCaLamViec = data["objCaLamViec"].ToObject<NS_CaLamViec>();
            string loaiEdit = "tạo";
            HT_NhatKySuDung hT_NhatKySuDung;
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    bool a = new ClassBH_HoaDon(db).Check_MaCaLamViec(objCaLamViec.MaCa.Trim());
                    if (a == true)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Mã ca làm việc đã tồn tại trong cơ sở dữ liệu"));
                    }
                    else
                    {
                        string CaLamViec_KhongDau = CommonStatic.ConvertToUnSign(objCaLamViec.TenCa.Trim()).ToLower();
                        string CaLamViec_ChuCaiDau = CommonStatic.convertchartstart(objCaLamViec.TenCa.Trim()).ToLower();
                        string sMaCaLamViec = objCaLamViec.MaCa.Trim();
                        if (sMaCaLamViec == null || sMaCaLamViec == "")
                        {
                            List<SqlParameter> sql = new List<SqlParameter>();
                            sql.Add(new SqlParameter("MaNhanSu", "CA00001"));
                            sql.Add(new SqlParameter("LoaiMa", "1"));
                            sMaCaLamViec = db.Database.SqlQuery<string>("exec get_MaNhanSu @MaNhanSu, @LoaiMa", sql.ToArray()).FirstOrDefault().Trim();
                        }
                        //set thời gian tạo phiếu
                        List<SqlParameter> sqlPRM = new List<SqlParameter>();
                        sqlPRM.Add(new SqlParameter("MaCa", sMaCaLamViec));
                        sqlPRM.Add(new SqlParameter("TenCa", objCaLamViec.TenCa));
                        sqlPRM.Add(new SqlParameter("CaLamViec_KhongDau", CaLamViec_KhongDau));
                        sqlPRM.Add(new SqlParameter("CaLamViec_ChuCaiDau", CaLamViec_ChuCaiDau));
                        sqlPRM.Add(new SqlParameter("GioVao", objCaLamViec.GioVao));
                        sqlPRM.Add(new SqlParameter("GioRa", objCaLamViec.GioRa));
                        sqlPRM.Add(new SqlParameter("TongGioCong", objCaLamViec.TongGioCong));
                        if (objCaLamViec.NghiGiuaCaTu == null)
                            sqlPRM.Add(new SqlParameter("NghiGiuaCaTu", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("NghiGiuaCaTu", objCaLamViec.NghiGiuaCaTu));
                        if (objCaLamViec.NghiGiuaCaDen == null)
                            sqlPRM.Add(new SqlParameter("NghiGiuaCaDen", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("NghiGiuaCaDen", objCaLamViec.NghiGiuaCaDen));
                        if (objCaLamViec.GioOTBanNgayTu == null)
                            sqlPRM.Add(new SqlParameter("GioOTBanNgayTu", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioOTBanNgayTu", objCaLamViec.GioOTBanNgayTu));
                        if (objCaLamViec.GioOTBanNgayDen == null)
                            sqlPRM.Add(new SqlParameter("GioOTBanNgayDen", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioOTBanNgayDen", objCaLamViec.GioOTBanNgayDen));
                        if (objCaLamViec.GioOTBanDemTu == null)
                            sqlPRM.Add(new SqlParameter("GioOTBanDemTu", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioOTBanDemTu", objCaLamViec.GioOTBanDemTu));
                        if (objCaLamViec.GioOTBanDemDen == null)
                            sqlPRM.Add(new SqlParameter("GioOTBanDemDen", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioOTBanDemDen", objCaLamViec.GioOTBanDemDen));
                        if (objCaLamViec.SoPhutDiMuon == null)
                            sqlPRM.Add(new SqlParameter("SoPhutDiMuon", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("SoPhutDiMuon", objCaLamViec.SoPhutDiMuon));
                        if (objCaLamViec.SoPhutVeSom == null)
                            sqlPRM.Add(new SqlParameter("SoPhutVeSom", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("SoPhutVeSom", objCaLamViec.SoPhutVeSom));
                        if (objCaLamViec.GioVaoTu == null)
                            sqlPRM.Add(new SqlParameter("GioVaoTu", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioVaoTu", objCaLamViec.GioVaoTu));
                        if (objCaLamViec.GioVaoDen == null)
                            sqlPRM.Add(new SqlParameter("GioVaoDen", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioVaoDen", objCaLamViec.GioVaoDen));
                        if (objCaLamViec.GioRaTu == null)
                            sqlPRM.Add(new SqlParameter("GioRaTu", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioRaTu", objCaLamViec.GioRaTu));
                        if (objCaLamViec.GioRaDen == null)
                            sqlPRM.Add(new SqlParameter("GioRaDen", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GioRaDen", objCaLamViec.GioRaDen));
                        if (objCaLamViec.TinhOTBanNgayTu == null)
                            sqlPRM.Add(new SqlParameter("TinhOTBanNgayTu", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("TinhOTBanNgayTu", objCaLamViec.TinhOTBanNgayTu));
                        if (objCaLamViec.TinhOTBanNgayDen == null)
                            sqlPRM.Add(new SqlParameter("TinhOTBanNgayDen", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("TinhOTBanNgayDen", objCaLamViec.TinhOTBanNgayDen));
                        if (objCaLamViec.TinhOTBanDemTu == null)
                            sqlPRM.Add(new SqlParameter("TinhOTBanDemTu", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("TinhOTBanDemTu", objCaLamViec.TinhOTBanDemTu));
                        if (objCaLamViec.TinhOTBanDemDen == null)
                            sqlPRM.Add(new SqlParameter("TinhOTBanDemDen", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("TinhOTBanDemDen", objCaLamViec.TinhOTBanDemDen));
                        sqlPRM.Add(new SqlParameter("LaCaDem", objCaLamViec.LaCaDem));
                        sqlPRM.Add(new SqlParameter("CachLayGioCong", objCaLamViec.CachLayGioCong));
                        sqlPRM.Add(new SqlParameter("SoGioOTToiThieu", objCaLamViec.SoGioOTToiThieu));
                        if (objCaLamViec.GhiChuCaLamViec == null)
                            sqlPRM.Add(new SqlParameter("GhiChuCaLamVec", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GhiChuCaLamVec", objCaLamViec.GhiChuCaLamViec));
                        if (objCaLamViec.GhiChuTinhGio == null)
                            sqlPRM.Add(new SqlParameter("GhiChuTinhGio", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("GhiChuTinhGio", objCaLamViec.GhiChuTinhGio));
                        sqlPRM.Add(new SqlParameter("TrangThai", objCaLamViec.TrangThai));
                        sqlPRM.Add(new SqlParameter("NguoiTao", objCaLamViec.NguoiTao));
                        db.Database.ExecuteSqlCommand("exec insert_CaLamViec @MaCa, @TenCa, @CaLamViec_KhongDau, @CaLamViec_ChuCaiDau, @GioVao, @GioRa, @TongGioCong, @NghiGiuaCaTu, @NghiGiuaCaDen, @GioOTBanNgayTu, @GioOTBanNgayDen," +
                            "@GioOTBanDemTu, @GioOTBanDemDen, @SoPhutDiMuon, @SoPhutVeSom, @GioVaoTu, @GioVaoDen, @GioRaTu, @GioRaDen, @TinhOTBanNgayTu, @TinhOTBanNgayDen, " +
                            "@TinhOTBanDemTu, @TinhOTBanDemDen, @LaCaDem, @CachLayGioCong, @SoGioOTToiThieu, @GhiChuCaLamVec, @GhiChuTinhGio, @TrangThai, @NguoiTao", sqlPRM.ToArray());

                        string _trangthai = objCaLamViec.TrangThai == 1 ? "Đang áp dụng" : "Không áp dụng";
                        hT_NhatKySuDung = new HT_NhatKySuDung
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = ID_NhanVien,
                            ID_DonVi = ID_DonVi,
                            ChucNang = "Danh mục ca làm việc",
                            ThoiGian = DateTime.Now,
                            NoiDung = loaiEdit + " ca làm việc: " + sMaCaLamViec + " Giờ vào: " + objCaLamViec.GioVao + ", giờ ra: " + objCaLamViec.GioRa + ". Trạng thái: " + _trangthai,
                            NoiDungChiTiet = loaiEdit + " ca làm việc: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + sMaCaLamViec + "')\" >" + sMaCaLamViec + "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ", Giờ vào: " + objCaLamViec.GioVao + ", giờ ra: " + objCaLamViec.GioRa + ". Trạng thái: " + _trangthai,
                            LoaiNhatKy = 1
                        };
                    }
                }
                if (hT_NhatKySuDung != null)
                {
                    string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                }
                return CreatedAtRoute("DefaultApi", new { id = objCaLamViec.ID }, objCaLamViec);
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
            }
        }
        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult PutNS_CaLamViec([FromBody] JObject data, Guid ID_NhanVien, Guid ID_DonVi, Guid ID_CaLamViec)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            NS_CaLamViec objCaLamViec = data["objCaLamViec"].ToObject<NS_CaLamViec>();
            string loaiEdit = "Cập nhật";
            try
            {
                //set thời gian tạo phiếu
                string CaLamViec_KhongDau = CommonStatic.ConvertToUnSign(objCaLamViec.TenCa.Trim()).ToLower();
                string CaLamViec_ChuCaiDau = CommonStatic.convertchartstart(objCaLamViec.TenCa.Trim()).ToLower();
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID", ID_CaLamViec));
                sqlPRM.Add(new SqlParameter("TenCa", objCaLamViec.TenCa));
                sqlPRM.Add(new SqlParameter("CaLamViec_KhongDau", CaLamViec_KhongDau));
                sqlPRM.Add(new SqlParameter("CaLamViec_ChuCaiDau", CaLamViec_ChuCaiDau));
                sqlPRM.Add(new SqlParameter("GioVao", objCaLamViec.GioVao));
                sqlPRM.Add(new SqlParameter("GioRa", objCaLamViec.GioRa));
                sqlPRM.Add(new SqlParameter("TongGioCong", objCaLamViec.TongGioCong));
                if (objCaLamViec.NghiGiuaCaTu == null)
                    sqlPRM.Add(new SqlParameter("NghiGiuaCaTu", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("NghiGiuaCaTu", objCaLamViec.NghiGiuaCaTu));
                if (objCaLamViec.NghiGiuaCaDen == null)
                    sqlPRM.Add(new SqlParameter("NghiGiuaCaDen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("NghiGiuaCaDen", objCaLamViec.NghiGiuaCaDen));
                if (objCaLamViec.GioOTBanNgayTu == null)
                    sqlPRM.Add(new SqlParameter("GioOTBanNgayTu", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioOTBanNgayTu", objCaLamViec.GioOTBanNgayTu));
                if (objCaLamViec.GioOTBanNgayDen == null)
                    sqlPRM.Add(new SqlParameter("GioOTBanNgayDen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioOTBanNgayDen", objCaLamViec.GioOTBanNgayDen));
                if (objCaLamViec.GioOTBanDemTu == null)
                    sqlPRM.Add(new SqlParameter("GioOTBanDemTu", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioOTBanDemTu", objCaLamViec.GioOTBanDemTu));
                if (objCaLamViec.GioOTBanDemDen == null)
                    sqlPRM.Add(new SqlParameter("GioOTBanDemDen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioOTBanDemDen", objCaLamViec.GioOTBanDemDen));
                if (objCaLamViec.SoPhutDiMuon == null)
                    sqlPRM.Add(new SqlParameter("SoPhutDiMuon", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("SoPhutDiMuon", objCaLamViec.SoPhutDiMuon));
                if (objCaLamViec.SoPhutVeSom == null)
                    sqlPRM.Add(new SqlParameter("SoPhutVeSom", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("SoPhutVeSom", objCaLamViec.SoPhutVeSom));
                if (objCaLamViec.GioVaoTu == null)
                    sqlPRM.Add(new SqlParameter("GioVaoTu", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioVaoTu", objCaLamViec.GioVaoTu));
                if (objCaLamViec.GioVaoDen == null)
                    sqlPRM.Add(new SqlParameter("GioVaoDen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioVaoDen", objCaLamViec.GioVaoDen));
                if (objCaLamViec.GioRaTu == null)
                    sqlPRM.Add(new SqlParameter("GioRaTu", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioRaTu", objCaLamViec.GioRaTu));
                if (objCaLamViec.GioRaDen == null)
                    sqlPRM.Add(new SqlParameter("GioRaDen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GioRaDen", objCaLamViec.GioRaDen));
                if (objCaLamViec.TinhOTBanNgayTu == null)
                    sqlPRM.Add(new SqlParameter("TinhOTBanNgayTu", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("TinhOTBanNgayTu", objCaLamViec.TinhOTBanNgayTu));
                if (objCaLamViec.TinhOTBanNgayDen == null)
                    sqlPRM.Add(new SqlParameter("TinhOTBanNgayDen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("TinhOTBanNgayDen", objCaLamViec.TinhOTBanNgayDen));
                if (objCaLamViec.TinhOTBanDemTu == null)
                    sqlPRM.Add(new SqlParameter("TinhOTBanDemTu", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("TinhOTBanDemTu", objCaLamViec.TinhOTBanDemTu));
                if (objCaLamViec.TinhOTBanDemDen == null)
                    sqlPRM.Add(new SqlParameter("TinhOTBanDemDen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("TinhOTBanDemDen", objCaLamViec.TinhOTBanDemDen));
                sqlPRM.Add(new SqlParameter("LaCaDem", objCaLamViec.LaCaDem));
                sqlPRM.Add(new SqlParameter("CachLayGioCong", objCaLamViec.CachLayGioCong));
                sqlPRM.Add(new SqlParameter("SoGioOTToiThieu", objCaLamViec.SoGioOTToiThieu));
                if (objCaLamViec.GhiChuCaLamViec == null)
                    sqlPRM.Add(new SqlParameter("GhiChuCaLamVec", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GhiChuCaLamVec", objCaLamViec.GhiChuCaLamViec));
                if (objCaLamViec.GhiChuTinhGio == null)
                    sqlPRM.Add(new SqlParameter("GhiChuTinhGio", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("GhiChuTinhGio", objCaLamViec.GhiChuTinhGio));
                sqlPRM.Add(new SqlParameter("TrangThai", objCaLamViec.TrangThai));
                sqlPRM.Add(new SqlParameter("NguoiSua", objCaLamViec.NguoiSua));
                db.Database.ExecuteSqlCommand("exec Update_CaLamViec @ID, @TenCa, @CaLamViec_KhongDau, @CaLamViec_ChuCaiDau, @GioVao, @GioRa, @TongGioCong, @NghiGiuaCaTu, @NghiGiuaCaDen, @GioOTBanNgayTu, @GioOTBanNgayDen," +
                    "@GioOTBanDemTu, @GioOTBanDemDen, @SoPhutDiMuon, @SoPhutVeSom, @GioVaoTu, @GioVaoDen, @GioRaTu, @GioRaDen, @TinhOTBanNgayTu, @TinhOTBanNgayDen, " +
                    "@TinhOTBanDemTu, @TinhOTBanDemDen, @LaCaDem, @CachLayGioCong, @SoGioOTToiThieu, @GhiChuCaLamVec, @GhiChuTinhGio, @TrangThai, @NguoiSua", sqlPRM.ToArray());
                #region NhatKySuDung
                string _trangthai = objCaLamViec.TrangThai == 1 ? "Đang áp dụng" : "Không áp dụng";
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                {
                    ID = Guid.NewGuid(),
                    ID_NhanVien = ID_NhanVien,
                    ID_DonVi = ID_DonVi,
                    ChucNang = "Danh mục ca làm việc",
                    ThoiGian = DateTime.Now,
                    NoiDung = loaiEdit + " ca làm việc: " + objCaLamViec.MaCa + " Giờ vào: " + objCaLamViec.GioVao + ", giờ ra: " + objCaLamViec.GioRa + ". Trạng thái: " + _trangthai,
                    NoiDungChiTiet = loaiEdit + " ca làm việc: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + objCaLamViec.MaCa + "')\" >" + objCaLamViec.MaCa + "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ", Giờ vào: " + objCaLamViec.GioVao + ", giờ ra: " + objCaLamViec.GioRa + ". Trạng thái: " + _trangthai,
                    LoaiNhatKy = 1
                };
                string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                #endregion
                return CreatedAtRoute("DefaultApi", new { id = objCaLamViec.ID }, objCaLamViec);
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
            }
        }
        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult DeleteNS_CaLamViec(Guid ID_CaLamViec)
        {
            try
            {
                SsoftvnContext db = SystemDBContext.GetDBContext();
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ID", ID_CaLamViec));
                db.Database.ExecuteSqlCommand("exec Delete_CaLamViec @ID", sql.ToArray());
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
            }
        }
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
        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult getListNhanSu_CaLamViec(int trangthaiAD, int trangthaiKAD, int trangthaiXoa, int paperSize, DateTime timeStart, DateTime timeEnd, string MaCa)
        {
            List<DM_CaLamViecPROC> lst = new List<DM_CaLamViecPROC>();
            SsoftvnContext db = SystemDBContext.GetDBContext();
            string MaCa_search = "%";
            string MaCa_TV = "%";
            if (MaCa != null && MaCa != "" && MaCa != "null")
            {
                MaCa_TV = "%" + MaCa.Trim() + "%";
                MaCa_search = "%" + CommonStatic.ConvertToUnSign(MaCa.Trim()).ToLower() + "%";
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("MaCa", MaCa_search));
            sql.Add(new SqlParameter("MaCa_TV", MaCa_TV));
            sql.Add(new SqlParameter("timeStart", timeStart));
            sql.Add(new SqlParameter("timeEnd", timeEnd));
            sql.Add(new SqlParameter("TrangThaiAD", trangthaiAD));
            sql.Add(new SqlParameter("TrangThaiKAD", trangthaiKAD));
            sql.Add(new SqlParameter("TrangThaiXoa", trangthaiXoa));
            lst = db.Database.SqlQuery<DM_CaLamViecPROC>("exec NhanSu_CaLamViec @MaCa, @MaCa_TV, @timeStart, @timeEnd, @TrangThaiAD, @TrangThaiKAD, @TrangThaiXoa", sql.ToArray()).ToList();
            int Rown = lst.Count();
            int lstPages = getNumber_Page(Rown, paperSize);
            JsonResultExampleTr<DM_CaLamViecPROC> json = new JsonResultExampleTr<DM_CaLamViecPROC>
            {
                LstData = lst,
                Rowcount = Rown,
                numberPage = lstPages,
            };
            return Json(json);
        }
        //import
        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult ImportExcel_ThongTinCaLamViec(string NguoiTao)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string TieuDe = "MẪU FILE IMPORT THÔNG TIN CA LÀM VIỆC";
                            string str = _Class_officeDocument.CheckFileMauTheoTieuDe(excelstream, TieuDe);
                            // string str = Class_officeDocument.CheckFileMauThongTinNhanVien(excelstream);
                            if (str == null)
                            {
                                abc = _Class_officeDocument.checkfileThongTinCaLamViec(excelstream, NguoiTao);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [AcceptVerbs("POST", "GET")]
        public IHttpActionResult ImportThongTinCaLamViec_WithError(string RownError, string NguoiTao)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Request.Files.Count != 0)
                {
                    using (SsoftvnContext db = SystemDBContext.GetDBContext())
                    {
                        Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _Class_officeDocument.IgnoreErrorThongTinCaLamViec(excelstream, RownError, NguoiTao);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                }
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [AcceptVerbs("GET", "POST")]
        public void Export_NhanSu_CaLamViec(int trangthaiAD, int trangthaiKAD, int trangthaiXoa, DateTime timeStart, DateTime timeEnd, string MaCa, string columnsHide, string TodayBC)
        {
            try
            {


                List<Export_CaLamViecPROC> lst = new List<Export_CaLamViecPROC>();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    string MaCa_search = "%";
                    string MaCa_TV = "%";
                    if (MaCa != null && MaCa != "" && MaCa != "null")
                    {
                        MaCa_TV = "%" + MaCa.Trim() + "%";
                        MaCa_search = "%" + CommonStatic.ConvertToUnSign(MaCa.Trim()).ToLower() + "%";
                    }
                    List<SqlParameter> sql = new List<SqlParameter>();
                    sql.Add(new SqlParameter("MaCa", MaCa_search));
                    sql.Add(new SqlParameter("MaCa_TV", MaCa_TV));
                    sql.Add(new SqlParameter("timeStart", timeStart));
                    sql.Add(new SqlParameter("timeEnd", timeEnd));
                    sql.Add(new SqlParameter("TrangThaiAD", trangthaiAD));
                    sql.Add(new SqlParameter("TrangThaiKAD", trangthaiKAD));
                    sql.Add(new SqlParameter("TrangThaiXoa", trangthaiXoa));
                    Class_officeDocument _Class_officeDocument = new Class_officeDocument(db);
                    lst = db.Database.SqlQuery<Export_CaLamViecPROC>("exec Export_NhanSu_CaLamViec @MaCa, @MaCa_TV, @timeStart, @timeEnd, @TrangThaiAD, @TrangThaiKAD, @TrangThaiXoa", sql.ToArray()).ToList();
                    DataTable excel = _Class_officeDocument.ToDataTable<Export_CaLamViecPROC>(lst);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/CaLamViec_ChamCong/Teamplate_BaoCaoDanhMucCaLamViec.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/CaLamViec_ChamCong/BaoCaoDanhMucCaLamViec.xlsx");
                    fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                    _Class_officeDocument.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 5, 29, 24, true, columnsHide, TodayBC, "");
                    HttpResponse Response = HttpContext.Current.Response;
                    FileInfo file = new FileInfo(fileSave);

                    if (file.Exists)
                    {
                        Response.ClearContent();
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                        Response.AddHeader("Content-Length", file.Length.ToString());
                        Response.ContentType = "text/plain";
                        Response.TransmitFile(file.FullName);
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                string str = CookieStore.GetCookieAes("SubDomain");
                CookieStore.WriteLog("Export_NhanSu_CaLamViec(int trangthaiAD, int trangthaiKAD, int trangthaiXoa, DateTime timeStart, DateTime timeEnd, string MaCa, string columnsHide, string TodayBC): " + ex.InnerException + ex.Message, str);
            }
        }
        #endregion
        #region Phân công ca làm việc
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostNS_PhieuPhanCa([FromBody] JObject data, Guid ID_NhanVien, Guid ID_DonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                NS_PhieuPhanCa objPhieuPhanCa = data["objPhieuPhanCa"].ToObject<NS_PhieuPhanCa>();
                string loaiEdit = "tạo";
                bool a = new ClassBH_HoaDon(db).Check_MaCaLamViec(objPhieuPhanCa.MaPhieu.Trim());
                if (a == true)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Mã phiếu phân ca làm việc đã tồn tại trong cơ sở dữ liệu"));
                }
                else
                {
                    try
                    {
                        string sMaPhieuPhanCa = objPhieuPhanCa.MaPhieu.Trim();
                        if (sMaPhieuPhanCa == null || sMaPhieuPhanCa == "")
                        {
                            List<SqlParameter> sql = new List<SqlParameter>();
                            sql.Add(new SqlParameter("MaNhanSu", "PH00001"));
                            sql.Add(new SqlParameter("LoaiMa", "2"));
                            sMaPhieuPhanCa = db.Database.SqlQuery<string>("exec get_MaNhanSu @MaNhanSu, @LoaiMa", sql.ToArray()).FirstOrDefault().Trim();
                        }
                        Guid ID_PhieuPhanCa = Guid.NewGuid();
                        //set thời gian tạo phiếu
                        List<SqlParameter> sqlPRM = new List<SqlParameter>();
                        sqlPRM.Add(new SqlParameter("ID", ID_PhieuPhanCa));
                        sqlPRM.Add(new SqlParameter("MaPhieu", sMaPhieuPhanCa));
                        sqlPRM.Add(new SqlParameter("TuNgay", objPhieuPhanCa.TuNgay));
                        if (objPhieuPhanCa.DenNgay == null)
                            sqlPRM.Add(new SqlParameter("DenNgay", DBNull.Value));
                        else
                            sqlPRM.Add(new SqlParameter("DenNgay", objPhieuPhanCa.DenNgay));
                        sqlPRM.Add(new SqlParameter("ID_NhanVien", objPhieuPhanCa.ID_NhanVienTao));
                        sqlPRM.Add(new SqlParameter("TrangThai", objPhieuPhanCa.TrangThai));
                        sqlPRM.Add(new SqlParameter("NguoiTao", objPhieuPhanCa.NgayTao));
                        sqlPRM.Add(new SqlParameter("GhiChu", objPhieuPhanCa.NgayTao));
                        db.Database.ExecuteSqlCommand("exec insert_PhieuPhanCa @ID, @MaPhieu, @TuNgay, @DenNgay, @ID_NhanVien, @TrangThai, @NguoiTao, @GhiChu", sqlPRM.ToArray());
                        //insert Phân Ca
                        //List<NS_PhieuPhanCa_ChiTiet> objPhanCa = data["objPhanCa"].ToObject<List<NS_PhieuPhanCa_ChiTiet>>();
                        //foreach (var item in objPhanCa)
                        //{

                        //}
                        #region NhatKySuDung
                        string _trangthai = objPhieuPhanCa.TrangThai == 1 ? "Đã duyệt" : "Không duyệt";
                        HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                        {
                            ID = Guid.NewGuid(),
                            ID_NhanVien = ID_NhanVien,
                            ID_DonVi = ID_DonVi,
                            ChucNang = "Phân công ca làm việc",
                            ThoiGian = DateTime.Now,
                            NoiDung = loaiEdit + " phiếu phân ca: " + sMaPhieuPhanCa + " Từ ngày: " + objPhieuPhanCa.TuNgay + ", Đến Ngày: " + objPhieuPhanCa.DenNgay + ". Trạng thái: " + _trangthai,
                            NoiDungChiTiet = loaiEdit + " phiếu phân ca: <a style= \"cursor: pointer\" onclick = \"loadPhieuPhanCabyMaPhieu('" + sMaPhieuPhanCa + "')\" >" + sMaPhieuPhanCa + "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ", Từ ngày: " + objPhieuPhanCa.TuNgay + ", đến ngày: " + objPhieuPhanCa.DenNgay + ". Trạng thái: " + _trangthai,
                            LoaiNhatKy = 1
                        };
                        string strIns12 = SaveDiary.add_Diary(hT_NhatKySuDung);
                        #endregion
                        return CreatedAtRoute("DefaultApi", new { id = objPhieuPhanCa.ID }, objPhieuPhanCa);
                    }
                    catch (Exception ex)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
                    }
                }
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_NhanVienCaTuan(array_NhanVienPhanCa array_NhanVienPhanCa)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<CaTuan_NhanVienPhanCaPROC> lst = new List<CaTuan_NhanVienPhanCaPROC>();
            string ID_PhongBan_SP = string.Empty;
            string ID_PhongBan_search = "%";
            if (array_NhanVienPhanCa.ID_PhongBan != null)
            {
                List<Report_PhongBan> lst_NHH = new List<Report_PhongBan>();
                lst_NHH = Class_Report.getList_ID_PhongBan(lst_NHH, array_NhanVienPhanCa.ID_PhongBan);
                foreach (var item in lst_NHH)
                {
                    if (ID_PhongBan_SP == string.Empty)
                        ID_PhongBan_SP = item.ID_PhongBan.ToString();
                    else
                        ID_PhongBan_SP = ID_PhongBan_SP + "," + item.ID_PhongBan.ToString();
                }
                ID_PhongBan_search = "%" + ID_PhongBan_SP + "%";
            }
            else
                ID_PhongBan_SP = Guid.NewGuid().ToString();
            string MaNV_search = "%";
            string MaNV_TV = "%";
            string a = array_NhanVienPhanCa.MaNhanVien;
            if (array_NhanVienPhanCa.MaNhanVien != null && array_NhanVienPhanCa.MaNhanVien != "" && array_NhanVienPhanCa.MaNhanVien != "null")
            {
                MaNV_TV = "%" + array_NhanVienPhanCa.MaNhanVien.Trim() + "%";
                MaNV_search = "%" + CommonStatic.ConvertToUnSign(array_NhanVienPhanCa.MaNhanVien).ToLower() + "%";
            }
            string ID_ChiNhanh = "%";
            if (array_NhanVienPhanCa.ID_ChiNhanh != null && array_NhanVienPhanCa.ID_ChiNhanh.ToString() != "" && array_NhanVienPhanCa.ID_ChiNhanh.ToString() != "null")
            {
                ID_ChiNhanh = array_NhanVienPhanCa.ID_ChiNhanh.ToString();
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("MaNV", MaNV_search));
            sql.Add(new SqlParameter("Ma_TV", MaNV_TV));
            sql.Add(new SqlParameter("LoaiCa", array_NhanVienPhanCa.LoaiCa));
            sql.Add(new SqlParameter("TrangThai", array_NhanVienPhanCa.TrangThai));
            sql.Add(new SqlParameter("NgayApDung", array_NhanVienPhanCa.TuNgay));
            sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
            sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
            lst = db.Database.SqlQuery<CaTuan_NhanVienPhanCaPROC>("exec getList_NhanVienPhanCa @MaNV," +
                "@Ma_TV,@LoaiCa,@TrangThai,@NgayApDung,@ID_ChiNhanh," +
                "@ID_PhongBan,@ID_PhongBan_SP", sql.ToArray()).ToList();
            int Rown = lst.Count();
            int lstPages = getNumber_Page(Rown, 10);
            JsonResultExampleTr<CaTuan_NhanVienPhanCaPROC> json = new JsonResultExampleTr<CaTuan_NhanVienPhanCaPROC>
            {
                LstData = lst,
            };
            return Json(json);
        }
        public IHttpActionResult getList_NhanVienCaTHang(array_NhanVienPhanCa array_NhanVienPhanCa)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<CaThang_NhanVienPhanCaPROC> lst = new List<CaThang_NhanVienPhanCaPROC>();
            string ID_PhongBan_SP = string.Empty;
            string ID_PhongBan_search = "%";
            if (array_NhanVienPhanCa.ID_PhongBan != null)
            {
                List<Report_PhongBan> lst_NHH = new List<Report_PhongBan>();
                lst_NHH = Class_Report.getList_ID_PhongBan(lst_NHH, array_NhanVienPhanCa.ID_PhongBan);
                foreach (var item in lst_NHH)
                {
                    if (ID_PhongBan_SP == string.Empty)
                        ID_PhongBan_SP = item.ID_PhongBan.ToString();
                    else
                        ID_PhongBan_SP = ID_PhongBan_SP + "," + item.ID_PhongBan.ToString();
                }
                ID_PhongBan_search = "%" + ID_PhongBan_SP + "%";
            }
            else
                ID_PhongBan_SP = Guid.NewGuid().ToString();
            string MaNV_search = "%";
            string MaNV_TV = "%";
            string a = array_NhanVienPhanCa.MaNhanVien;
            if (array_NhanVienPhanCa.MaNhanVien != null && array_NhanVienPhanCa.MaNhanVien != "" && array_NhanVienPhanCa.MaNhanVien != "null")
            {
                MaNV_TV = "%" + array_NhanVienPhanCa.MaNhanVien.Trim() + "%";
                MaNV_search = "%" + CommonStatic.ConvertToUnSign(array_NhanVienPhanCa.MaNhanVien).ToLower() + "%";
            }
            string ID_ChiNhanh = "%";
            if (array_NhanVienPhanCa.ID_ChiNhanh != null && array_NhanVienPhanCa.ID_ChiNhanh.ToString() != "" && array_NhanVienPhanCa.ID_ChiNhanh.ToString() != "null")
            {
                ID_ChiNhanh = array_NhanVienPhanCa.ID_ChiNhanh.ToString();
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("MaNV", MaNV_search));
            sql.Add(new SqlParameter("Ma_TV", MaNV_TV));
            sql.Add(new SqlParameter("LoaiCa", array_NhanVienPhanCa.LoaiCa));
            sql.Add(new SqlParameter("TrangThai", array_NhanVienPhanCa.TrangThai));
            sql.Add(new SqlParameter("NgayApDung", array_NhanVienPhanCa.TuNgay));
            sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
            sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
            lst = db.Database.SqlQuery<CaThang_NhanVienPhanCaPROC>("exec getList_NhanVienPhanCa @MaNV," +
                "@Ma_TV,@LoaiCa,@TrangThai,@NgayApDung,@ID_ChiNhanh," +
                "@ID_PhongBan,@ID_PhongBan_SP", sql.ToArray()).ToList();
            int Rown = lst.Count();
            int lstPages = getNumber_Page(Rown, 10);
            JsonResultExampleTr<CaThang_NhanVienPhanCaPROC> json = new JsonResultExampleTr<CaThang_NhanVienPhanCaPROC>
            {
                LstData = lst,
            };
            return Json(json);
        }
        public IHttpActionResult getList_NhanVienCaMacDinh(array_NhanVienPhanCa array_NhanVienPhanCa)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            List<MacDinh_NhanVienPhanCaPROC> lst = new List<MacDinh_NhanVienPhanCaPROC>();
            string ID_PhongBan_SP = string.Empty;
            string ID_PhongBan_search = "%";
            if (array_NhanVienPhanCa.ID_PhongBan != null)
            {
                List<Report_PhongBan> lst_NHH = new List<Report_PhongBan>();
                lst_NHH = Class_Report.getList_ID_PhongBan(lst_NHH, array_NhanVienPhanCa.ID_PhongBan);
                foreach (var item in lst_NHH)
                {
                    if (ID_PhongBan_SP == string.Empty)
                        ID_PhongBan_SP = item.ID_PhongBan.ToString();
                    else
                        ID_PhongBan_SP = ID_PhongBan_SP + "," + item.ID_PhongBan.ToString();
                }
                ID_PhongBan_search = "%" + ID_PhongBan_SP + "%";
            }
            else
                ID_PhongBan_SP = Guid.NewGuid().ToString();
            string MaNV_search = "%";
            string MaNV_TV = "%";
            string a = array_NhanVienPhanCa.MaNhanVien;
            if (array_NhanVienPhanCa.MaNhanVien != null && array_NhanVienPhanCa.MaNhanVien != "" && array_NhanVienPhanCa.MaNhanVien != "null")
            {
                MaNV_TV = "%" + array_NhanVienPhanCa.MaNhanVien.Trim() + "%";
                MaNV_search = "%" + CommonStatic.ConvertToUnSign(array_NhanVienPhanCa.MaNhanVien).ToLower() + "%";
            }
            string ID_ChiNhanh = "%";
            if (array_NhanVienPhanCa.ID_ChiNhanh != null && array_NhanVienPhanCa.ID_ChiNhanh.ToString() != "" && array_NhanVienPhanCa.ID_ChiNhanh.ToString() != "null")
            {
                ID_ChiNhanh = array_NhanVienPhanCa.ID_ChiNhanh.ToString();
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("MaNV", MaNV_search));
            sql.Add(new SqlParameter("Ma_TV", MaNV_TV));
            sql.Add(new SqlParameter("LoaiCa", array_NhanVienPhanCa.LoaiCa));
            sql.Add(new SqlParameter("TrangThai", array_NhanVienPhanCa.TrangThai));
            sql.Add(new SqlParameter("NgayApDung", array_NhanVienPhanCa.TuNgay));
            sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
            sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
            sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
            lst = db.Database.SqlQuery<MacDinh_NhanVienPhanCaPROC>("exec getList_NhanVienPhanCa @MaNV," +
                "@Ma_TV,@LoaiCa,@TrangThai,@NgayApDung,@ID_ChiNhanh," +
                "@ID_PhongBan,@ID_PhongBan_SP", sql.ToArray()).ToList();
            int Rown = lst.Count();
            int lstPages = getNumber_Page(Rown, 10);
            JsonResultExampleTr<MacDinh_NhanVienPhanCaPROC> json = new JsonResultExampleTr<MacDinh_NhanVienPhanCaPROC>
            {
                LstData = lst,
            };
            return Json(json);
        }
        #endregion


        #region TuanDL Ca Làm việc, phân ca, chấm công

        [HttpPost]
        public IHttpActionResult GetForSearchCalamviec(SearchFilter model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.GetCaLamViecFilter(model);
                    if (model.Sort)
                    {
                        switch (model.Order)
                        {
                            case (int)commonEnum.columncalamviec.maca:
                                data = data.OrderBy(o => o.MaCa);
                                break;
                            case (int)commonEnum.columncalamviec.tenca:
                                data = data.OrderBy(o => o.TenCa);
                                break;
                            case (int)commonEnum.columncalamviec.trangthai:
                                data = data.OrderBy(o => o.TrangThai);
                                break;
                            case (int)commonEnum.columncalamviec.giovao:
                                data = data.OrderBy(o => o.GioVao);
                                break;
                            case (int)commonEnum.columncalamviec.giora:
                                data = data.OrderBy(o => o.GioRa);
                                break;
                            case (int)commonEnum.columncalamviec.nghigiuacatu:
                                data = data.OrderBy(o => o.NghiGiuaCaTu);
                                break;
                            case (int)commonEnum.columncalamviec.nghigiuacaden:
                                data = data.OrderBy(o => o.NghiGiuaCaDen);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemngaytu:
                                data = data.OrderBy(o => o.GioOTBanNgayTu);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemngayden:
                                data = data.OrderBy(o => o.GioOTBanNgayDen);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemdemtu:
                                data = data.OrderBy(o => o.GioOTBanDemTu);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemdemden:
                                data = data.OrderBy(o => o.GioOTBanDemDen);
                                break;
                            case (int)commonEnum.columncalamviec.tongconggio:
                                data = data.OrderBy(o => o.TongGioCong);
                                break;
                            case (int)commonEnum.columncalamviec.cachlaygiocong:
                                data = data.OrderBy(o => o.CachLayGioCong);
                                break;
                            case (int)commonEnum.columncalamviec.lacadem:
                                data = data.OrderBy(o => o.LaCaDem);
                                break;
                            case (int)commonEnum.columncalamviec.nguoitao:
                                data = data.OrderBy(o => o.NguoiTao);
                                break;
                            case (int)commonEnum.columncalamviec.ghichu:
                                data = data.OrderBy(o => o.GhiChuCaLamViec);
                                break;
                            default:
                                data = data.OrderBy(o => o.NgayTao);

                                break;
                        }
                    }
                    else
                    {
                        switch (model.Order)
                        {
                            case (int)commonEnum.columncalamviec.maca:
                                data = data.OrderByDescending(o => o.MaCa);
                                break;
                            case (int)commonEnum.columncalamviec.tenca:
                                data = data.OrderByDescending(o => o.TenCa);
                                break;
                            case (int)commonEnum.columncalamviec.trangthai:
                                data = data.OrderByDescending(o => o.TrangThai);
                                break;
                            case (int)commonEnum.columncalamviec.giovao:
                                data = data.OrderByDescending(o => o.GioVao);
                                break;
                            case (int)commonEnum.columncalamviec.giora:
                                data = data.OrderByDescending(o => o.GioRa);
                                break;
                            case (int)commonEnum.columncalamviec.nghigiuacatu:
                                data = data.OrderByDescending(o => o.NghiGiuaCaTu);
                                break;
                            case (int)commonEnum.columncalamviec.nghigiuacaden:
                                data = data.OrderByDescending(o => o.NghiGiuaCaDen);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemngaytu:
                                data = data.OrderByDescending(o => o.GioOTBanNgayTu);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemngayden:
                                data = data.OrderByDescending(o => o.GioOTBanNgayDen);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemdemtu:
                                data = data.OrderByDescending(o => o.GioOTBanDemTu);
                                break;
                            case (int)commonEnum.columncalamviec.giolamthemdemden:
                                data = data.OrderByDescending(o => o.GioOTBanDemDen);
                                break;
                            case (int)commonEnum.columncalamviec.tongconggio:
                                data = data.OrderByDescending(o => o.TongGioCong);
                                break;
                            case (int)commonEnum.columncalamviec.cachlaygiocong:
                                data = data.OrderByDescending(o => o.CachLayGioCong);
                                break;
                            case (int)commonEnum.columncalamviec.lacadem:
                                data = data.OrderByDescending(o => o.LaCaDem);
                                break;
                            case (int)commonEnum.columncalamviec.nguoitao:
                                data = data.OrderByDescending(o => o.NguoiTao);
                                break;
                            case (int)commonEnum.columncalamviec.ghichu:
                                data = data.OrderByDescending(o => o.GhiChuCaLamViec);
                                break;
                            default:
                                data = data.OrderByDescending(o => o.NgayTao);

                                break;
                        }

                    }
                    var result = data.Skip((model.pageNow - 1) * model.pageSize).Take(model.pageSize).AsEnumerable().Select(o =>
                                       new NS_CaLamViecModel()
                                       {
                                           ID = o.ID,
                                           CachLayGioCong = o.CachLayGioCong,
                                           GhiChuCaLamViec = o.GhiChuCaLamViec,
                                           GioOTBanDemDen = o.GioOTBanDemDen,
                                           GhiChuTinhGio = o.GhiChuTinhGio,
                                           GioOTBanDemTu = o.GioOTBanDemTu,
                                           GioOTBanNgayDen = o.GioOTBanNgayDen,
                                           GioOTBanNgayTu = o.GioOTBanNgayTu,
                                           GioRa = o.GioRa,
                                           GioRaDen = o.GioRaDen,
                                           GioRaTu = o.GioRaTu,
                                           GioVao = o.GioVao,
                                           GioVaoDen = o.GioVaoDen,
                                           GioVaoTu = o.GioVaoTu,
                                           LaCaDem = o.LaCaDem,
                                           MaCa = o.MaCa,
                                           NgayTao = o.NgayTao,
                                           NghiGiuaCaDen = o.NghiGiuaCaDen,
                                           NghiGiuaCaTu = o.NghiGiuaCaTu,
                                           NguoiTao = o.NguoiTao,
                                           SoGioOTToiThieu = o.SoGioOTToiThieu,
                                           SoPhutDiMuon = o.SoPhutDiMuon,
                                           SoPhutVeSom = o.SoPhutVeSom,
                                           TenCa = o.TenCa,
                                           ThoiGianDiMuonVeSom = o.ThoiGianDiMuonVeSom,
                                           TinhOTBanDemDen = o.TinhOTBanDemDen,
                                           TinhOTBanDemTu = o.TinhOTBanDemTu,
                                           TinhOTBanNgayDen = o.TinhOTBanNgayDen,
                                           TinhOTBanNgayTu = o.TinhOTBanNgayTu,
                                           TongGioCong = o.TongGioCong,
                                           TrangThai = o.TrangThai,
                                           ListIdDonVI = o.NS_CaLamViec_DonVi != null ? o.NS_CaLamViec_DonVi.Select(c => c.ID_DonVi).ToList() : new List<Guid>(),
                                           DaApDungPhanCa = (o.NS_CongBoSung != null && o.NS_CongBoSung.Any())
                                       }
                                  ).ToList();
                    var count = data.Count();
                    int page = 0;
                    var listpage = GetListPage(count, model.pageSize, model.pageNow, ref page);
                    return ActionTrueData(new
                    {
                        data = result,
                        listpage = listpage,
                        pagenow = model.pageNow,
                        pageview = "Hiển thị " + ((model.pageNow - 1) * model.pageSize + 1) + " - " + ((model.pageNow - 1) * model.pageSize + result.Count) + " trên tổng số " + count + " ca làm việc",
                        isprev = model.pageNow > 3 && page > 5,
                        isnext = model.pageNow < page - 2 && page > 5,
                        countpage = page
                    });
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult InsertCaLamViec(CaLamViecModelInsert data, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (data == null || data.Model == null || data.ListDonVi == null || data.ListDonVi.Count == 0)
                    {
                        return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
                    }
                    var model = data.Model;
                    if (_NhanSuService.CheckExitMaCa(model.MaCa))
                    {
                        return ActionFalseNotData("Mã ca đã bị trùng");
                    }
                    if ((model.NghiGiuaCaTu != null && (model.NghiGiuaCaTu.Value.TimeOfDay < model.GioVao.TimeOfDay || model.NghiGiuaCaTu.Value.TimeOfDay > model.GioRa.TimeOfDay))
                        || (model.NghiGiuaCaDen != null && (model.NghiGiuaCaDen.Value.TimeOfDay < model.GioVao.TimeOfDay || model.NghiGiuaCaDen.Value.TimeOfDay > model.GioRa.TimeOfDay))
                        )
                    {
                        return ActionFalseNotData("Nghỉ giữa ca không được nằm ngoài giờ vào và giờ ra");
                    }
                    if (string.IsNullOrWhiteSpace(model.MaCa))
                    {
                        model.MaCa = _NhanSuService.GetMaCa();
                    }

                    model.ID = Guid.NewGuid();
                    model.CaLamViec_KhongDau = CommonStatic.RemoveSign4VietnameseString(model.TenCa.Trim()).ToLower();
                    model.CaLamViec_ChuCaiDau = CommonStatic.convertchartstart(model.TenCa.Trim()).ToLower();
                    model.NgayTao = DateTime.Now;
                    var trangthai = commonEnumHellper.ListTrangThaiCaLamViec.FirstOrDefault(o => o.Key == model.TrangThai);
                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_NhanVien = ID_NhanVien,
                        ID_DonVi = ID_DonVi,
                        ChucNang = "Danh mục ca làm việc",
                        ThoiGian = DateTime.Now,
                        NoiDung = "Thêm mới ca làm việc - Mã ca: " + model.MaCa,
                        NoiDungChiTiet = "Thêm mới ca làm việc: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" >" + model.MaCa + "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ", Giờ vào: " + model.GioVao.ToString("HH:mm") + ", giờ ra: " + model.GioRa.ToString("HH:mm") + ". Trạng thái: " + trangthai.Value,
                        LoaiNhatKy = 1
                    };
                    _NhanSuService.InsertCaLamViec(model, ID_NhanVien, data.ListDonVi);
                    _NhanSuService.InsertNhatKy(hT_NhatKySuDung);
                    return InsertSuccess();
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }
        [HttpPost]
        public IHttpActionResult UpdateCaLamViec(CaLamViecModelInsert data, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (data == null || data.Model == null || data.ListDonVi == null || data.ListDonVi.Count == 0)
                    {
                        return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
                    }
                    var model = data.Model;
                    var reult = _NhanSuService.UpdateCaLamViec(model, ID_NhanVien, ID_DonVi, data.ListDonVi);
                    if (reult)
                    {
                        return UpdateSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData("Ca làm việc không tồn tại hoặc đã xóa không thể cập nhật");
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }
        [HttpPost]
        public IHttpActionResult ExportExcelToCaLamViec(SearchFilter model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    string time = string.Empty;
                    var lst = _NhanSuService.GetExportExcelToCaLamViec(model, ref time).OrderByDescending(o => o.NgayTao).ToList();
                    var chinhanh = "Toàn bộ chi nhánh";
                    if (model.ListDonVi != null && model.ListDonVi.Count > 0)
                    {
                        chinhanh = _NhanSuService.GetListDonViById(model.ListDonVi);
                    }
                    Class_officeDocument _Class_officeDocument = new Class_officeDocument(_dbcontext);
                    ClassNPOIExcel classNPOI = new ClassNPOIExcel();
                    DataTable excel = _Class_officeDocument.ToDataTable<NS_CaLamViecExport>(lst);
                    excel.Columns.Remove("ID");
                    excel.Columns.Remove("TrangThai");
                    excel.Columns.Remove("CachLayGioCong");
                    excel.Columns.Remove("NgayTao");
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/CaLamViec_ChamCong/Teamplate_BaoCaoDanhMucCaLamViec.xlsx");
                    //string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/CaLamViec_ChamCong/BaoCaoDanhMucCaLamViec.xlsx");
                    //fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                    //_Class_officeDocument.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 5, 29, 24, true, "", time, chinhanh);
                    //System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                    List<ClassExcel_CellData> lstCell = classNPOI.GetValue_forCell(chinhanh, time);
                    classNPOI.ExportDataToExcel(fileTeamplate, excel, 4, "", lstCell, -1);
                    return ActionTrueData(string.Empty);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteCaLamViec(NS_CaLamViec model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null || model.ID == null || model.ID == new Guid())
                    {
                        return ActionFalseNotData("Không lấy được thông tin cần xóa");
                    }
                    if (!_NhanSuService.DeleteCaLamViec(model, ID_NhanVien, ID_DonVi))
                    {
                        return ActionFalseNotData("Ca làm việc không tồn tại");
                    }
                    return DeleteSuccess();
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        public IHttpActionResult ImportFileCaLamViec(Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var path = "";
                    for (int j = 0; j < HttpContext.Current.Request.Files.Count; j++)
                    {
                        var file = HttpContext.Current.Request.Files[j];
                        if (file != null && file.ContentLength > 0)
                        {
                            //ExcelDataReader works on binary excel file
                            Stream stream = file.InputStream;
                            //We need to written the Interface.
                            IExcelDataReader reader = null;
                            if (file.FileName.EndsWith(".xls"))
                            {
                                //reads the excel file with .xls extension
                                reader = ExcelReaderFactory.CreateBinaryReader(stream);
                            }
                            else if (file.FileName.EndsWith(".xlsx"))
                            {
                                //reads excel file with .xlsx extension
                                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            }
                            else
                            {
                                return ActionFalseNotData("File không đúng định dạng");
                            }

                            var conf = new ExcelDataSetConfiguration
                            {
                                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                                {
                                    UseHeaderRow = true
                                }
                            };
                            var table = reader.AsDataSet(conf).Tables[0];
                            // Khai báo
                            var listInsert = new List<NS_CaLamViec>();
                            var listExport = new List<ExportCaLamViecError>();
                            var IsCheck = true;
                            int countEmty = 0;
                            int number = 0;
                            double doublenumber;
                            TimeSpan timeSpan;
                            StringBuilder ErrorLog;
                            var compareTime = false;
                            var dateNow = new DateTime(2020, 1, 1, 0, 0, 0);
                            for (int i = 3; i < table.Rows.Count; i++)
                            {
                                if (table.Rows[i].ItemArray.All(o => string.IsNullOrWhiteSpace(o.ToString())))
                                {
                                    countEmty += 1;
                                    continue;
                                }
                                IsCheck = true;
                                var model = new NS_CaLamViec();
                                var modelerror = new ExportCaLamViecError();
                                ErrorLog = new StringBuilder();
                                compareTime = false;
                                //Mã ca
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][0].ToString()))
                                {
                                    if (!_NhanSuService.CheckExitMaCa(table.Rows[i][0].ToString()))
                                    {
                                        model.MaCa = table.Rows[i][0].ToString().ToUpper();
                                    }
                                    else
                                    {
                                        modelerror.MaCa = table.Rows[i][0].ToString().ToUpper();
                                        ErrorLog.Append("- Mã ca đã tồn tại trong hệ thống \r\n");
                                        IsCheck = false;
                                    }
                                }
                                // Tên ca
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][1].ToString()))
                                {
                                    modelerror.TenCa = table.Rows[i][1].ToString().ToUpper();
                                    model.TenCa = table.Rows[i][1].ToString();
                                }
                                else
                                {
                                    ErrorLog.Append("=> Tên ca không được để trống \r\n,");
                                    IsCheck = false;
                                }
                                // trạng thái
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][2].ToString()))
                                {
                                    modelerror.TrangThai = table.Rows[i][2].ToString();
                                    if (CommonStatic.IsValidNumberFormat(table.Rows[i][2].ToString(), ref number))
                                    {
                                        model.TrangThai = number;
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Trạng thái không đúng định dạng \r\n");
                                        IsCheck = false;

                                    }
                                }
                                else
                                {
                                    ErrorLog.Append("- Trạng thái không được để trống \r\n,");
                                    IsCheck = false;
                                }
                                // Giờ vào 
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][3].ToString()))
                                {
                                    modelerror.GioVao = table.Rows[i][3].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][3].ToString(), out timeSpan))
                                    {
                                        model.GioVao = dateNow.Add(timeSpan);
                                        compareTime = true;
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Giờ vào không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                else
                                {
                                    ErrorLog.Append("- Giờ vào không được để trống \r\n,");
                                    IsCheck = false;
                                }

                                // Giờ ra
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][4].ToString()))
                                {
                                    modelerror.GioRa = table.Rows[i][4].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][4].ToString(), out timeSpan))
                                    {
                                        model.GioRa = dateNow.Add(timeSpan);
                                        if (compareTime && DateTime.Compare(model.GioVao, model.GioRa) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Giờ vào lớn hơn giờ ra \r\n");
                                        }
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Giờ ra không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                else
                                {
                                    ErrorLog.Append("- Giờ ra không được để trống \r\n,");
                                    IsCheck = false;
                                }
                                // Nghỉ giữa ca từ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][5].ToString()))
                                {
                                    modelerror.NghiGiuaCaTu = table.Rows[i][5].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][5].ToString(), out timeSpan))
                                    {
                                        model.NghiGiuaCaTu = dateNow.Add(timeSpan);
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Nghỉ giữa ca từ không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }

                                // Nghỉ giữa ca đến
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][6].ToString()))
                                {
                                    modelerror.NghiGiuaCaDen = table.Rows[i][6].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][6].ToString(), out timeSpan))
                                    {
                                        model.NghiGiuaCaDen = dateNow.Add(timeSpan);
                                        if (model.NghiGiuaCaTu.HasValue && DateTime.Compare(model.NghiGiuaCaTu.Value, model.NghiGiuaCaDen.Value) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Nghỉ giữa ca từ lớn hơn nghỉ giữa ca đến \r\n");
                                        }
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Nghỉ giữa ca đến không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Giờ làm thêm ban ngày từ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][7].ToString()))
                                {
                                    modelerror.GioOTBanNgayTu = table.Rows[i][7].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][7].ToString(), out timeSpan))
                                    {
                                        model.GioOTBanNgayTu = dateNow.Add(timeSpan);
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Giờ làm thêm ban ngày từ không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }

                                // Giờ làm thêm ban ngày đến
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][8].ToString()))
                                {
                                    modelerror.GioOTBanNgayDen = table.Rows[i][8].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][8].ToString(), out timeSpan))
                                    {

                                        model.GioOTBanNgayDen = dateNow.Add(timeSpan);
                                        if (model.GioOTBanNgayTu.HasValue && DateTime.Compare(model.GioOTBanNgayTu.Value, model.GioOTBanNgayDen.Value) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Giờ làm thêm ban ngày từ lớn hơn giờ làm thêm ban ngày đến \r\n");
                                        }

                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Giờ làm thêm ban ngày đến không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Giờ làm thêm ban đêm từ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][9].ToString()))
                                {
                                    modelerror.GioOTBanDemTu = table.Rows[i][9].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][9].ToString(), out timeSpan))
                                    {
                                        model.GioOTBanDemTu = dateNow.Add(timeSpan);
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Giờ làm thêm ban đêm từ không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }

                                // Giờ làm thêm ban đêm đến
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][10].ToString()))
                                {
                                    modelerror.GioOTBanDemDen = table.Rows[i][10].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][10].ToString(), out timeSpan))
                                    {
                                        model.GioOTBanDemDen = dateNow.Add(timeSpan);
                                        if (model.GioOTBanDemTu.HasValue && DateTime.Compare(model.GioOTBanDemTu.Value, model.GioOTBanDemDen.Value) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Giờ làm thêm ban đêm từ lớn hơn giờ làm thêm ban đêm đến \r\n");
                                        }
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Giờ làm thêm ban đêm đến không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Cách lấy công
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][11].ToString()))
                                {
                                    modelerror.CachLayGioCong = table.Rows[i][11].ToString();
                                    if (CommonStatic.IsValidNumberFormat(table.Rows[i][11].ToString(), ref number))
                                    {
                                        model.CachLayGioCong = number;
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Cách lấy công không đúng định dạng \r\n");
                                        IsCheck = false;

                                    }
                                }
                                else
                                {
                                    ErrorLog.Append("- Cách lấy công không được để trống \r\n");
                                    IsCheck = false;
                                }
                                // Là ca đêm
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][12].ToString()))
                                {
                                    modelerror.LaCaDem = table.Rows[i][12].ToString();
                                    if (table.Rows[i][12].ToString().ToUpper().Equals("X"))
                                    {
                                        model.LaCaDem = 1;
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Là ca đêm không đúng định dạng \r\n");
                                        IsCheck = false;

                                    }
                                }
                                else
                                {
                                    model.LaCaDem = 0;
                                }
                                // Ghi chú ca làm việc
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][13].ToString()))
                                {
                                    model.GhiChuCaLamViec = table.Rows[i][13].ToString();
                                }
                                // Số phút đi muộn
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][14].ToString()))
                                {
                                    modelerror.SoPhutDiMuon = table.Rows[i][14].ToString();
                                    if (CommonStatic.IsValidNumberFormat(table.Rows[i][14].ToString(), ref number))
                                    {
                                        model.SoPhutDiMuon = number;
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Số phút đi muộn không đúng định dạng \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Số phút về sớm
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][15].ToString()))
                                {
                                    modelerror.SoPhutVeSom = table.Rows[i][15].ToString();
                                    if (CommonStatic.IsValidNumberFormat(table.Rows[i][15].ToString(), ref number))
                                    {
                                        model.SoPhutVeSom = number;
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Số phút đi muộn không đúng định dạng \r\n");
                                        IsCheck = false;

                                    }
                                }

                                // Thời gian tính giờ vào từ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][16].ToString()))
                                {
                                    modelerror.GioVaoTu = table.Rows[i][16].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][16].ToString(), out timeSpan))
                                    {
                                        model.GioVaoTu = dateNow.Add(timeSpan);
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Thời gian tính giờ vào từ không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Thời gian tính giờ vào đến
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][17].ToString()))
                                {
                                    modelerror.GioVaoDen = table.Rows[i][17].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][17].ToString(), out timeSpan))
                                    {
                                        model.GioVaoDen = dateNow.Add(timeSpan);
                                        if (model.GioVaoTu != null && DateTime.Compare(model.GioVaoTu.Value, model.GioVaoDen.Value) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Giờ vào từ lớn hơn giờ vào đến \r\n");
                                        }
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Thời gian tính giờ vào đến không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }

                                // Thời gian tính giờ ra từ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][18].ToString()))
                                {
                                    modelerror.GioRaTu = table.Rows[i][18].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][18].ToString(), out timeSpan))
                                    {
                                        model.GioRaTu = dateNow.Add(timeSpan);
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Thời gian tính giờ ra từ không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Thời gian tính giờ ra đến
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][19].ToString()))
                                {
                                    modelerror.GioRaDen = table.Rows[i][19].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][19].ToString(), out timeSpan))
                                    {
                                        model.GioRaDen = dateNow.Add(timeSpan);
                                        if (model.GioRaTu != null && DateTime.Compare(model.GioRaTu.Value, model.GioRaDen.Value) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Giờ ra từ > giờ ra đến \r\n");
                                        }
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Thời gian tính giờ ra đến không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Tính Giờ làm thêm ban ngày từ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][20].ToString()))
                                {
                                    modelerror.TinhOTBanNgayTu = table.Rows[i][20].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][20].ToString(), out timeSpan))
                                    {
                                        model.TinhOTBanNgayTu = dateNow.Add(timeSpan);
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Tính giờ làm thêm ban ngày từ không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Tính Giờ làm thêm ban ngày đến
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][21].ToString()))
                                {
                                    modelerror.TinhOTBanNgayDen = table.Rows[i][21].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][21].ToString(), out timeSpan))
                                    {
                                        model.TinhOTBanNgayDen = dateNow.Add(timeSpan);
                                        if (model.TinhOTBanNgayTu != null && DateTime.Compare(model.TinhOTBanNgayTu.Value, model.TinhOTBanNgayDen.Value) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Tính Giờ làm thêm ban ngày từ > tính giờ làm thêm ban ngày đến \r\n");
                                        }

                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Tính Giờ làm thêm ban ngày đến không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Tính Giờ làm thêm ban đêm từ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][22].ToString()))
                                {
                                    modelerror.TinhOTBanDemTu = table.Rows[i][22].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][22].ToString(), out timeSpan))
                                    {
                                        model.TinhOTBanDemTu = dateNow.Add(timeSpan);
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Tính giờ làm thêm ban đêm từ không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Tính Giờ làm thêm ban đêm đến
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][23].ToString()))
                                {
                                    modelerror.TinhOTBanDemDen = table.Rows[i][23].ToString();
                                    if (CommonStatic.IsValidTimeFormat(table.Rows[i][23].ToString(), out timeSpan))
                                    {
                                        model.TinhOTBanDemDen = dateNow.Add(timeSpan);
                                        if (model.TinhOTBanDemTu != null && DateTime.Compare(model.TinhOTBanDemTu.Value, model.TinhOTBanDemDen.Value) > 0)
                                        {
                                            IsCheck = false;
                                            ErrorLog.Append("- Tính giờ làm thêm ban đêm từ > tính giờ làm thêm ban đêm đến \r\n");
                                        }

                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Tính Giờ làm thêm ban đêm đến không đúng định dạng HH:mm \r\n");
                                        IsCheck = false;

                                    }
                                }
                                // Số giờ làm thêm tối thiểu
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][24].ToString()))
                                {
                                    modelerror.SoGioOTToiThieu = table.Rows[i][24].ToString();
                                    if (CommonStatic.IsValiddoubleFormat(table.Rows[i][24].ToString(), out doublenumber))
                                    {
                                        model.SoGioOTToiThieu = doublenumber;
                                    }
                                    else
                                    {
                                        ErrorLog.Append("- Số giờ làm thêm tối thiểu không đúng định dạng \r\n");
                                        IsCheck = false;

                                    }
                                }
                                else
                                {
                                    ErrorLog.Append("- Số giờ làm thêm tối thiểu không được để trống \r\n");
                                    IsCheck = false;
                                }
                                // Ghi chú  tính giờ
                                if (!string.IsNullOrWhiteSpace(table.Rows[i][25].ToString()))
                                {
                                    model.GhiChuTinhGio = table.Rows[i][25].ToString();
                                }

                                if (IsCheck)
                                {
                                    model.NgayTao = DateTime.Now;
                                    model.CaLamViec_KhongDau = CommonStatic.RemoveSign4VietnameseString(model.TenCa.Trim()).ToLower();
                                    model.CaLamViec_ChuCaiDau = CommonStatic.convertchartstart(model.TenCa.Trim()).ToLower();
                                    var tonggio = (model.GioRa.Hour * 60 + model.GioRa.Minute) - (model.GioVao.Hour * 60 + model.GioVao.Minute);
                                    if (model.NghiGiuaCaTu.HasValue && model.NghiGiuaCaDen.HasValue)
                                    {
                                        tonggio -= ((model.NghiGiuaCaDen.Value.Hour * 60 + model.NghiGiuaCaDen.Value.Minute) - (model.NghiGiuaCaTu.Value.Hour * 60 + model.NghiGiuaCaTu.Value.Minute));
                                    }
                                    model.TongGioCong = Math.Round(((double)tonggio) / 60, 1);
                                    model.SoGioOTToiThieu = Math.Round(model.SoGioOTToiThieu, 1);
                                    listInsert.Add(model);
                                }
                                else
                                {
                                    modelerror.Error = "Note \r\n " + ErrorLog.ToString();

                                    listExport.Add(modelerror);
                                }
                            }
                            var countsum = (table.Rows.Count - (3 + countEmty));
                            reader.Close();
                            if (listInsert.Count == 0 && listExport.Count == 0)
                            {
                                return ActionFalseNotData("File import trống vui lòng kiểm tra lại");
                            }

                            //Insert và thông báo
                            _NhanSuService.ImprtToDataCaLamViec(listInsert, ID_NhanVien, ID_DonVi, countsum, listExport.Count);
                            if (listExport.Count > 0)
                            {
                                Class_officeDocument _Class_officeDocument = new Class_officeDocument(_dbcontext);
                                DataTable excel = _Class_officeDocument.ToDataTable<ExportCaLamViecError>(listExport);
                                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/FIleImportError/CaLamViec/FileImport_ThongTinCaLamViecError.xlsx");
                                string fileSave = HttpContext.Current.Server.MapPath("~/Template/FIleImportError/CaLamViec/FileImportCaLamViecLoi.xlsx");
                                fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                                _Class_officeDocument.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 27, 24, true, "", "", "");

                                return ActionTrueData(new
                                {
                                    succes = false,
                                    mess = "Hệ thống đã thêm mới thành công " + listInsert.Count + " / " + countsum + " bản ghi",
                                    file = fileSave,
                                });
                            }
                            return ActionTrueData(new
                            {
                                succes = true,
                                mess = "Hệ thống thêm mới thành công " + countsum + " bản ghi"
                            });

                        }
                        break;
                    }
                    return ActionFalseNotData("Không lấy được file cần import");
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetColumnCaLamViec()
        {
            return ActionTrueData(commonEnum.listdanhmucalamviec.Select(o => new { Key = o.Key, Value = o.Value, Checked = true }).ToList());
        }

        /// <summary>
        /// Lấy danh sách chi nhánh theo nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListChiNhanhNhanVien(Guid id)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.GetDonViByNhanVien(id).Select(o => new
                    {
                        Id = o.ID,
                        Ten = o.TenDonVi,
                        Checked = false
                    }).ToList();
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo chi nhánh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListNhanVienTheoChiNhanh(Guid id)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetNhanVienByChiNhanh(id);
                return ActionTrueData(data);

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách phòng ban theo chi nhánh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListPhongBanTheoChiNhanh(Guid id)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.GetListPhongBanChiNhanh(id).ToList();
                    return ActionTrueData(data);
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo phòng ban
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListNhanVienTheoPhongBan(Guid id)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetNhanVienByPhongBan(id);
                return ActionTrueData(data);

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        /// <summary>
        /// Lấy danh sách ca theo chi nhánh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListCaTheoChiNhanh(Guid id)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetListCatheoChiNhanh(id).AsEnumerable().Select(o => new
                {
                    Id = o.ID,
                    Ma = o.MaCa,
                    Ten = o.TenCa,
                    Checked = false,
                    GioVao = o.GioVao,
                    GioRa = o.GioRa,
                    o.TrangThai,
                    TongGioCong = o.TongGioCong,
                    ListIdDonVI = o.NS_CaLamViec_DonVi != null ? o.NS_CaLamViec_DonVi.Select(c => c.ID_DonVi).ToList() : new List<Guid>(),
                }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Thêm mới phiếu phân ca
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertPhanCaLamViec(PhanCaModel model, Guid ID_DonVi)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
                }
                if (model.PhieuPhanCa == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
                }
                if (model.PhieuPhanCa.DenNgay != null && model.PhieuPhanCa.TuNgay.Date > model.PhieuPhanCa.DenNgay.Value.Date)
                {
                    return ActionFalseNotData("Từ ngày bắt đầu không được lớn hơn ngày kết thúc");
                }
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var lstNVSameTime = _NhanSuService.CheckSameTime_CaLamViec(model);
                    if (lstNVSameTime.Count() > 0)
                    {
                        string nviens = string.Empty;
                        foreach (var nv in lstNVSameTime)
                        {
                            nviens += nv.MaNhanVien + ", ";
                        }
                        return ActionFalseNotData(string.Concat("Nhân viên ", nviens.Substring(0, nviens.Length - 2), " đã được phân ca trong khoảng thời gian này"));
                    }

                    model.PhieuPhanCa.NgayTao = DateTime.Now;
                    var result = _NhanSuService.InsertPhanCaLamViec(model);
                    if (!result)
                    {
                        return ActionFalseNotData("Tên phiếu đã tồn tại.");
                    }
                    string noidung = "Thêm mới phiếu phân ca - Mã phiếu: " + model.PhieuPhanCa.MaPhieu;
                    string chitiet = "Thêm mới phiếu phân ca: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.PhieuPhanCa.MaPhieu + "')\" >" + model.PhieuPhanCa.MaPhieu +
                                       "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                       ", : Từ ngày" + model.PhieuPhanCa.TuNgay.ToString("dd/MM/yyyy") +
                                       (model.PhieuPhanCa.DenNgay.HasValue ? ", Đến ngày: " + model.PhieuPhanCa.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty) +
                                       ", Trạng thái: " + (model.PhieuPhanCa.TrangThai == 1 ? "Tạo mới" : "Đang áp dụng");
                    _NhanSuService.InsertNhatKySuDung("Phiếu phân ca", noidung, chitiet, ID_DonVi, model.PhieuPhanCa.ID_NhanVienTao, 1);
                    _dbcontext.SaveChanges();
                    return InsertSuccess();
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Tìm kiếm phân ca
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetForSearchPhanCa(PhieuPhanCaFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                string time = string.Empty;
                List<NS_PhieuPhanCaDTO> data = _NhanSuService.GetListPhieuPhanCa(model, ref time);
                var count = 0;
                int page = 0;
                if (data.Count() > 0)
                {
                    count = data[0].TotalRow;
                }
                var listpage = GetListPage(count, model.pageSize, model.pageNow, ref page);
                return ActionTrueData(new
                {
                    data,
                    listpage,
                    pagenow = model.pageNow,
                    pageview = "Hiển thị " + ((model.pageNow - 1) * model.pageSize + 1) + " - " + ((model.pageNow - 1) * model.pageSize + data.Count()) + " trên tổng số " + count + " phiếu phân ca",
                    isprev = model.pageNow > 3 && page > 5,
                    isnext = model.pageNow < page - 2 && page > 5,
                    countpage = page,
                    totalRows= count,
                });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách phân ca theo chi nhánh
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetPhanCaByChiNhanh(PhieuPhanCaFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetPhanCaChiNhanh(model);
                var result = data.AsEnumerable().Select(o =>
                                 new
                                 {
                                     o.ID,
                                     o.MaPhieu,
                                     Checked = false

                                 }
                            ).ToList();

                return ActionTrueData(result);

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách chi tiết phiếu phân ca
        /// </summary>
        /// <param name="idPhieu"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetChiTietPhieuPhanCa(Guid idPhieu)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var modelChiTietCa = _NhanSuService.GetChiTietCaOfPhieu(idPhieu);
                var modelNhanVienCa = _NhanSuService.GetChiTietNhanVienOfPhieu(idPhieu).Select(o => new
                {
                    o.ID,
                    o.TenNhanVien,
                    o.MaNhanVien,
                    o.SoCMND,
                    o.DienThoaiDiDong,
                    o.NgaySinh,
                    o.GioiTinh,

                }).AsEnumerable();
                return ActionTrueData(new
                {
                    modelChiTietCa,
                    modelNhanVienCa

                });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        [HttpPost, HttpGet]
        public IHttpActionResult UpdatePhieuPhanCa_CheckExistBangLuongCong(Guid idPhieuPhanCa)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.UpdatePhieuPhanCa_CheckExistCong(idPhieuPhanCa);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Cập nhật phiếu phân ca
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdatePhanCaLamViec(PhanCaModel model, Guid ID_DonVi)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần cập nhật");
                }
                if (model.PhieuPhanCa == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần cập nhật");
                }
                if (model.PhieuPhanCa.DenNgay != null && model.PhieuPhanCa.TuNgay.Date > model.PhieuPhanCa.DenNgay.Value.Date)
                {
                    return ActionFalseNotData("Từ ngày bắt đầu không được lớn hơn ngày kết thúc");
                }
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {

                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var resul = _NhanSuService.UpdatePhanCaLamViec(model);
                    if (resul.ErrorCode)
                    {
                        string noidung = "Cập nhật phiếu phân ca - Mã phiếu: " + model.PhieuPhanCa.MaPhieu;
                        string chitiet = "Cập nhật phiếu phân ca: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.PhieuPhanCa.MaPhieu + "')\" >" + model.PhieuPhanCa.MaPhieu +
                                           "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", : Từ ngày" + model.PhieuPhanCa.TuNgay.ToString("dd/MM/yyyy") +
                                           (model.PhieuPhanCa.DenNgay.HasValue ? ", Đến ngày: " + model.PhieuPhanCa.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty) +
                                           ", Trạng thái: " + (model.PhieuPhanCa.TrangThai == 1 ? "Tạo mới" : "Đang áp dụng");
                        _NhanSuService.InsertNhatKySuDung("Phiếu phân ca", noidung, chitiet, ID_DonVi, model.PhieuPhanCa.ID_NhanVienTao, 2);
                        _dbcontext.SaveChanges();
                        return UpdateSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  lấy loại ca để so sánh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListLoaiCa()
        {
            try
            {
                return ActionTrueData(new
                {
                    newchar = new { CaCoDinh = ((int)commonEnumHellper.LoaiCa.cacodinh).ToString(), CaTuan = ((int)commonEnumHellper.LoaiCa.catuan).ToString() },
                    newnumber = new { CaCoDinh = (int)commonEnumHellper.LoaiCa.cacodinh, CaTuan = (int)commonEnumHellper.LoaiCa.catuan }
                });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  lấy danh sach ca để tìm kiếm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListLoaiCaFilter()
        {
            try
            {
                return ActionTrueData(commonEnumHellper.ListLoaiCa.Select(o => new
                {
                    Id = o.Key,
                    Ten = o.Value,
                    Checked = false
                }));
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  lấy danh sach ca column phiếu phân ca
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListColumnPhieu()
        {
            return ActionTrueData(commonEnum.ListPhieuPhanCa.Select(o => new { Key = o.Key, Value = o.Value, Checked = true }).ToList());
        }

        /// <summary>
        /// Xóa phiếu phân ca
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeletePhanCaLamViec(NS_PhieuPhanCa model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                if (model == null) return ActionFalseNotData("Không lấy được thông tin cần xóa");
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {

                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var result = _NhanSuService.DeletePhieuPhanCa(model.ID);
                    if (result)
                    {
                        string noidung = "Xóa phiếu phân ca - Mã phiếu: " + model.MaPhieu;
                        string chitiet = "Xóa phiếu phân ca: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" >" + model.MaPhieu +
                                           "</a>, Ngày xóa: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", Thông tin phiếu: Từ ngày " + model.TuNgay.ToString("dd/MM/yyyy") +
                                           (model.DenNgay.HasValue ? ", Đến ngày: " + model.DenNgay.Value.ToString("dd/MM/yyyy") : string.Empty) +
                                           ", Trạng thái: Đã hủy";
                        _NhanSuService.InsertNhatKySuDung("Phiếu phân ca", noidung, chitiet, ID_DonVi, ID_NhanVien, 3);
                        _dbcontext.SaveChanges();
                        return DeleteSuccess();
                    }
                    return ActionFalseNotData("Phiếu phân ca đã bị xóa hoặc không tồn tại");
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost, HttpGet]
        public void ChamCong_UpdatePhieuPhanCa(Guid idPhieuPC)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                var _NhanSuService = new NhanSuService(_dbcontext);
                _NhanSuService.ChamCong_UpdatePhieuPhanCa(idPhieuPC);
            }
        }

        /// <summary>
        /// Xuất danh sách phiếu phân ca
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportExcelToPhieuPhanCa(PhieuPhanCaFilter model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var time = "Toàn thời gian";
                    var lst = _NhanSuService.GetListPhieuPhanCa(model, ref time);

                    var chinhanh = "Toàn bộ chi nhánh";
                    if (model.ListDonVi != null && model.ListDonVi.Count > 0)
                    {
                        chinhanh = _NhanSuService.GetListDonViById(model.ListDonVi);
                    }
                    Class_officeDocument _Class_officeDocument = new Class_officeDocument(_dbcontext);
                    ClassNPOIExcel classNPOI = new ClassNPOIExcel();
                    DataTable excel = _Class_officeDocument.ToDataTable<NS_PhieuPhanCaDTO>(lst);
                    excel.Columns.Remove("ID");
                    excel.Columns.Remove("LoaiPhanCa");
                    excel.Columns.Remove("TrangThai");
                    excel.Columns.Remove("ID_DonVi");
                    excel.Columns.Remove("TenNhanViens");
                    excel.Columns.Remove("TotalRow");
                    excel.Columns.Remove("TotalPage");

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/CaLamViec_ChamCong/Teamplate_DanhSachPhieuPhanCa.xlsx");
                    //string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/CaLamViec_ChamCong/DanhSachPhieuPhanCa.xlsx");
                    //fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                    //_Class_officeDocument.listToOfficeExcel_Stype(fileTeamplate, fileSave, excel, 4, 8, 8, true, "", time, chinhanh);
                    //System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                    List<ClassExcel_CellData> lstCell = classNPOI.GetValue_forCell(chinhanh, time);
                    classNPOI.ExportDataToExcel(fileTeamplate, excel, 4, "", lstCell, -1);
                    return ActionTrueData(string.Empty);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Tìm kiếm kỳ tính công
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetForSearchKyTinhCong(SearchFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();

                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetKyTinhCongFilter(model);

                var result = data.Skip((model.pageNow - 1) * model.pageSize).Take(model.pageSize).AsEnumerable().Select(o =>
                                   new
                                   {
                                       o.ID,
                                       o.Ky,
                                       o.NamNhuan,
                                       o.TuNgay,
                                       o.DenNgay,
                                       o.NgayTao,
                                       o.NguoiTao,
                                       o.TrangThai,
                                       TrangThaiText = commonEnumHellper.ListTrangThaiKyTinhCong.FirstOrDefault(c => c.Key == o.TrangThai).Value
                                   }
                              ).AsEnumerable();
                var count = data.Count();
                int page = 0;
                var listpage = GetListPage(count, model.pageSize, model.pageNow, ref page);
                return ActionTrueData(new
                {
                    data = result,
                    listpage = listpage,
                    pagenow = model.pageNow,
                    pageview = "Hiển thị " + ((model.pageNow - 1) * model.pageSize + 1) + " - " + ((model.pageNow - 1) * model.pageSize + result.Count()) + " trên tổng số " + count + " kỳ tính công",
                    isprev = model.pageNow > 3 && page > 5,
                    isnext = model.pageNow < page - 2 && page > 5,
                    countpage = page
                });


            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy all danh sách kỳ tính công
        /// Sau này cần chỉnh lại lấy theo năm
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetAllKyTinhCong()
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();

                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetAllKyTinhCongActive().Select(o => new
                {
                    o.ID,
                    o.Ky,
                    o.TuNgay,
                    o.DenNgay,
                    o.TrangThai,
                    Checked = false
                }).OrderByDescending(o => o.Ky);
                return ActionTrueData(data.AsEnumerable());

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        /// <summary>
        /// Thêm mới kỳ tính công
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertKyTinhCong(NS_KyTinhCong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
                    }
                    if (model.DenNgay != null && model.TuNgay.Date > model.DenNgay.Date)
                    {
                        return ActionFalseNotData("Từ ngày bắt đầu không được lớn hơn ngày kết thúc");
                    }
                    if (int.Parse(model.DenNgay.ToString("yyyyMM")) > int.Parse(DateTime.Now.ToString("yyyyMM")))
                    {
                        return ActionFalseNotData("Thời gian vượt quá tháng hiện tại");
                    }
                    model.ID = Guid.NewGuid();
                    model.NgayTao = DateTime.Now;

                    var resul = _NhanSuService.InsertKyTinhCong(model, ID_NhanVien);
                    if (resul.ErrorCode)
                    {
                        string noidung = "Thêm mới kỳ tính công - kỳ: " + model.Ky;
                        string chitiet = "Thêm mới kỳ tính công: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > kỳ: " + model.Ky +
                                           "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", : Từ ngày" + model.TuNgay.ToString("dd/MM/yyyy") +
                                          ", Đến ngày: " + model.DenNgay.ToString("dd/MM/yyyy");
                        _NhanSuService.InsertNhatKySuDung("Kỳ tính công", noidung, chitiet, ID_DonVi, ID_NhanVien, 1);
                        _dbcontext.SaveChanges();
                        return InsertSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        /// <summary>
        /// Cập nhật kỳ tính công
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateKyTinhCong(NS_KyTinhCong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ActionFalseNotData("Không lấy được thông tin cần cập nhật");
                    }
                    if (model.DenNgay != null && model.TuNgay.Date > model.DenNgay.Date)
                    {
                        return ActionFalseNotData("Từ ngày bắt đầu không được lớn hơn ngày kết thúc");
                    }
                    if (int.Parse(model.DenNgay.ToString("yyyyMM")) > int.Parse(DateTime.Now.ToString("yyyyMM")))
                    {
                        return ActionFalseNotData("Thời gian vượt quá tháng hiện tại");
                    }
                    model.NgaySua = DateTime.Now;

                    var resul = _NhanSuService.UpdateKyTinhCong(model, ID_NhanVien, ID_DonVi);
                    if (resul.ErrorCode)
                    {
                        return UpdateSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }

            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Xóa kỳ tính công
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteKyTinhCong(NS_KyTinhCong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
                    }

                    var resul = _NhanSuService.DeleteKyTinhCong(model, ID_NhanVien);
                    if (resul.ErrorCode)
                    {
                        string noidung = "Xóa kỳ tính công - kỳ: " + model.Ky;
                        string chitiet = "Xóa kỳ tính công: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > kỳ: " + model.Ky +
                                           "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", : Từ ngày" + model.TuNgay.ToString("dd/MM/yyyy") +
                                          ", Đến ngày: " + model.DenNgay.ToString("dd/MM/yyyy") +
                                           ", Trạng thái: " + (model.TrangThai == 1 ? "Đang áp dụng" : "Không áp dụng");
                        _NhanSuService.InsertNhatKySuDung("Kỳ tính công", noidung, chitiet, ID_DonVi, ID_NhanVien, 3);
                        _dbcontext.SaveChanges();
                        return DeleteSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }

            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  Lấy danh sách ký hiệu công
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetForSearchKyHieuCong(Guid idDonVi)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetAllKyHieuCong(idDonVi).OrderBy(o => o.KyHieu).Select(o => new
                {
                    o.ID,
                    o.KyHieu,
                    o.MoTa,
                    o.CongQuyDoi,
                    o.TrangThai,
                    o.GioVao,
                    o.GioRa,
                    o.LayGioMacDinh
                }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }
        /// <summary>
        ///  Lấy danh sách ký hiệu công by kỳ tính công (notsue)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetForSearchKyHieuCongByKy(Guid? id = null)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetAllKyHieuCong(id).OrderBy(o => o.KyHieu).Select(o => new
                {
                    o.ID,
                    o.KyHieu,
                    o.MoTa,
                    o.CongQuyDoi,
                    o.TrangThai,
                    o.GioVao,
                    o.GioRa,
                    o.LayGioMacDinh
                }).AsEnumerable();
                return ActionTrueData(data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  Thêm mới ký hiệu công
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertKyHieuCong(NS_KyHieuCong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
                    }

                    model.ID = Guid.NewGuid();
                    var resul = _NhanSuService.InsertKyHieuCong(model, ID_NhanVien);
                    if (resul.ErrorCode == false)
                    {
                        string noidung = "Thêm mới ký hiệu công - ký hiệu: " + model.KyHieu;
                        string chitiet = "Thêm mới ký hiệu công: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > ký hiệu: " + model.KyHieu +
                                           "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", Mô tả: " + model.MoTa +
                                          ", Công quy đổi: " + model.CongQuyDoi +
                                           ", Lấy giờ theo ca: " + (model.LayGioMacDinh == true ? "Có" : "Không");
                        if (!model.LayGioMacDinh)
                        {
                            chitiet += ", Giờ vào: " + model.GioVao.Value.ToString("dd/MM") +
                                          ", Giờ ra: " + model.GioRa.Value.ToString("dd/MM");
                        }
                        _NhanSuService.InsertNhatKySuDung("Danh mục ký hiệu công", noidung, chitiet, ID_DonVi, ID_NhanVien, 1);
                        _dbcontext.SaveChanges();
                        return InsertSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        /// <summary>
        ///  Cập nhật ký hiệu công
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateKyHieuCong(NS_KyHieuCong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return NotFound();
                    }
                    var resul = _NhanSuService.UpdateKyHieuCong(model, ID_NhanVien, ID_DonVi);
                    if (resul.ErrorCode == false)
                    {
                        return UpdateSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Xóa ký hiệu công
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteKyHieuCong(NS_KyHieuCong model, Guid ID_DonVi, Guid ID_NhanVien)
        {

            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }

                    var resul = _NhanSuService.DeleteKyHieuCong(model);
                    if (resul.ErrorCode)
                    {
                        string noidung = "Xóa ký hiệu công - ký hiệu: " + model.KyHieu;
                        string chitiet = "Xóa ký hiệu công: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > ký hiệu: " + model.KyHieu +
                                           "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", Mô tả: " + model.MoTa +
                                          ", Công quy đổi: " + model.CongQuyDoi +
                                           ", Lấy giờ theo ca: " + (model.LayGioMacDinh == true ? "Có" : "Không");

                        _NhanSuService.InsertNhatKySuDung("Danh mục ký hiệu công", noidung, chitiet, ID_DonVi, ID_NhanVien, 3);
                        _dbcontext.SaveChanges();
                        return DeleteSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  Lấy danh sách Ngày nghỉ lễ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetForSearchNgayNghiLe()
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetAllNgayNghiLe();
                return ActionTrueData(data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Thêm mới ngày nghỉ lễ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertNgayNghiLe(NS_NgayNghiLe model, Guid ID_DonVi, Guid ID_NhanVien)
        {

            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }

                    var resul = _NhanSuService.InsertNgayNghiLe(model, ID_NhanVien, ID_DonVi);
                    if (resul.ErrorCode == false)
                    {
                        string noidung = "Thêm mới ";
                        if (model.LoaiNgay == (int)commonEnumHellper.LoaiNgaynghiLe.ngaythuong)
                        {
                            noidung = string.Concat(noidung, "ngày thường: ", commonEnumHellper.ListWeek.Where(x => x.Key == model.Thu).FirstOrDefault().Value);
                        }
                        else
                        {
                            noidung = string.Concat(noidung, "ngày nghỉ lễ - Ngày: ", model.Ngay.Value.ToString("dd/MM/yyyy"));
                        }
                        string chitiet = string.Concat(noidung,
                                   " <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > </a>," +
                                           " Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", Mô tả: " + model.MoTa +
                                           ", Loại ngày: " + commonEnumHellper.ListLoaiNgaynghiLe.FirstOrDefault(o => o.Key == model.LoaiNgay).Value);

                        _NhanSuService.InsertNhatKySuDung("Danh mục ngày nghỉ lễ", noidung, chitiet, ID_DonVi, ID_NhanVien, 1);
                        _dbcontext.SaveChanges();
                        return InsertSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Cập nhật ngày nghỉ lễ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateNgayNghiLe(NS_NgayNghiLe model, Guid ID_DonVi, Guid ID_NhanVien)
        {

            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }

                    var resul = _NhanSuService.UpdateNgayNghiLe(model, ID_NhanVien, ID_DonVi);
                    if (resul.ErrorCode == false)
                    {
                        return UpdateSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Xóa ngày nghỉ lễ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteNgayNghiLe(NS_NgayNghiLe model, Guid ID_DonVi, Guid ID_NhanVien)
        {

            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    var resul = _NhanSuService.DeleteNgayNghiLe(model, ID_NhanVien, ID_DonVi);
                    if (resul.ErrorCode == false)
                    {
                        string noidung = "Xóa Ngày nghỉ lễ - " + (model.Thu < 0 ? "Ngày: " + model.Ngay.Value.ToString("dd/MM/yyyy") : "Thứ: " + commonEnumHellper.ListWeek.FirstOrDefault(o => o.Key == model.Thu).Value);
                        string chitiet = "Xóa Ngày nghỉ lễ: <a style= \"cursor: pointer\" onclick = \"loadCaLamViecbyMaCa('" + model.ID + "')\" > Ngày: " + model.Ngay.Value.ToString("dd/MM/yyyy") +
                                           "</a>, Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
                                           ", Mô tả: " + model.MoTa;
                        _NhanSuService.InsertNhatKySuDung("Danh mục ngày nghỉ lễ", noidung, chitiet, ID_DonVi, ID_NhanVien, 3);
                        _dbcontext.SaveChanges();
                        return DeleteSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách chi tiết các ký nghỉ lễ và ngày nghỉ lễ
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetChiTietKyTinhCong(Guid Id)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var dataNghiLe = _NhanSuService.GetAllNgayNghiLe();
                var dataKyHieuCong = _NhanSuService.GetAllKyHieuCong(Id).Select(o => new
                {
                    o.KyHieu,
                    o.MoTa,
                    o.CongQuyDoi,
                    o.LayGioMacDinh,
                    o.GioVao,
                    o.GioRa,
                    o.ID
                }).AsEnumerable();
                return ActionTrueData(new { NgayNghiLe = dataNghiLe, KyHieuCong = dataKyHieuCong });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GetForSearchChamCong(ChamCongFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                List<ChamCongModel> data = _NhanSuService.GetChamCongFilter(model).ToList();
                var result = data.GroupBy(o => new { o.Thang, o.Nam}).Select(o => new
                {
                    Thang = o.Key.Thang,
                    Nam = o.Key.Nam,
                    listCa = o.GroupBy(x => new { x.MaCa, x.TenCa, x.GioVao, x.GioRa }).Select(x => new
                    {
                        MaCa = x.Key.MaCa,
                        TenCa = x.Key.TenCa,
                        GioVao = x.Key.GioVao,
                        GioRa = x.Key.GioRa,
                        listCong = x.ToList()
                    })
                }).AsEnumerable();

                var count = data.Count() > 0 ? (int)data[0].TotalRow : 0;
                int page = 0;
                var listpage = GetListPage(count, model.pageSize, model.pageNow, ref page);
                return ActionTrueData(new
                {
                    data = result,
                    ListPage = listpage,
                    PageView = string.Concat("Hiển thị " + (model.pageNow * model.pageSize + 1), " - ", (model.pageNow * model.pageSize + data.Count()), " trên tổng số ", count, " ca chấm công"),
                    NumOfPage = page,
                    TotalRow = count,
                    sum = new
                    {
                        TongCong = 0,
                        TongCongNgayNghi = 0,
                        TongDiMuon = 0,
                        TongOt = 0
                    }
                });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Tìm kiếm hồ sơ bảng công
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetForSearchBangCong(ChamCongFilter model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    List<BangCongDTO> data = _NhanSuService.GetBangCongNhanVien(model);

                    int count = 0, page = 0;
                    double tongcong = 0, tongcongngaynghi = 0, tongdimuon = 0, tongot = 0;
                    if (data != null && data.Count() > 0)
                    {
                        BangCongDTO first = data.FirstOrDefault();
                        count = first.TotalRow;
                        tongcong = first.TongCong;
                        tongcongngaynghi = first.TongCongNgayNghi;
                        tongdimuon = first.TongDiMuon;
                        tongot = first.TongOT;
                    }

                    int[] listpage = GetListPage(count, model.pageSize, model.pageNow, ref page);
                    return ActionTrueData(new
                    {
                        data = data,
                        listpage = listpage,
                        pagenow = model.pageNow,
                        pageview = "Hiển thị " + ((model.pageNow - 1) * model.pageSize + 1) + " - " + ((model.pageNow - 1) * model.pageSize + data.Count()) + " trên tổng số " + count + " nhân viên",
                        isprev = model.pageNow > 3 && page > 5,
                        isnext = model.pageNow < page - 2 && page > 5,
                        countpage = page,
                        sum = new
                        {
                            TongCong = tongcong,
                            TongCongNgayNghi = tongcongngaynghi,
                            TongDiMuon = tongdimuon,
                            TongOT = tongot
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  Bổ sung nhân viên chấm công theo ca
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertHoSoChamCong(HoSoChamCongModel model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }

                    var resul = _NhanSuService.InsertHoSoChamCong(model, ID_NhanVien, ID_DonVi);
                    if (resul.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return InsertSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(resul.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }


        [HttpPost, HttpGet]
        public IHttpActionResult CheckCong_ExistBangLuong(Guid idChiNhanh, int ngay, int thang, int nam)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var exist = _NhanSuService.CheckCong_ExistBangLuong(idChiNhanh, ngay, thang, nam);
                    return ActionTrueData(exist);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult CheckNhanVienExist_ChamCong(Guid idNhanVien, Guid idDonVi)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var exist = _NhanSuService.CheckNhanVienExist_ChamCong(idNhanVien, idDonVi);
                    return ActionTrueData(exist);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult XoaCong_CheckExistBangLuong(Guid idNhanVien, DateTime fromdate, DateTime todate)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                try
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var data = _NhanSuService.XoaCong_CheckExistBangLuong(idNhanVien, fromdate, todate);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return Exeption(ex);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult RemoveCong_ofNhanVien(Guid idChiNhanh, Guid idNhanVien, Guid idCaLamViec, string fromdate, string todate)
        {
            using (var _dbcontext = (SystemDBContext.GetDBContext()))
            {
                try
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    _NhanSuService.RemoveCong_ofNhanVien(idChiNhanh, idNhanVien, idCaLamViec, fromdate, todate);
                    return ActionTrueNotData("Xóa công nhân viên thành công");
                }
                catch (Exception ex)
                {
                    return Exeption(ex);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult AddChamThuCong(AddCongModel model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    var resul = new JsonViewModel<string>();

                    var noidung = string.Concat("- Ngày: ", model.Ngay, "/", model.Thang, "/", model.Nam);
                    var chitiet = string.Concat(noidung, "<br /> Ngày tạo: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                              "<br /> Ký hiệu công: " + model.KyHieuCong,
                              "<br /> Công: " + model.Cong,
                              "<br /> Số giờ OT: " + model.SoGioOT,
                              "<br /> Số phút đi muộn: " + model.SoPhutDiMuon,
                              "<br /> Ghi chú: " + model.GhiChu
                        );
                    var calam = string.Concat("<br /> Mã ca: " + model.MaCa, "<br /> Tên ca: " + model.TenCa);

                    if (model.ApDungToanNhanVien)
                    {
                        resul = _NhanSuService.ApplyAllChamCong(model, ID_NhanVien, ID_DonVi);
                        if (resul.ErrorCode == false)
                        {
                            _NhanSuService.InsertNhatKySuDung("Chấm công",
                                "Áp dụng công cho toàn nhân viên " + noidung,
                                "Áp dụng công cho toàn nhân viên " + chitiet, ID_DonVi, ID_NhanVien, 1);
                            _dbcontext.SaveChanges();
                            return ActionTrueNotData("Áp dụng cho toàn bộ nhân viên thành công");
                        }
                    }
                    else if (model.IsNew)
                    {
                        resul = _NhanSuService.AddChamThuCong(model, ID_NhanVien, ID_DonVi);
                        if (resul.ErrorCode == false)
                        {
                            _NhanSuService.InsertNhatKySuDung("Chấm công",
                                "Thêm mới công cho nhân viên " + model.TenNhanVien + " (" + model.MaNhanVien + ")" + noidung,
                                "Thêm mới công cho nhân viên " + model.TenNhanVien + " (" + model.MaNhanVien + ")" + chitiet + calam,
                                 ID_DonVi, ID_NhanVien, 1);
                            _dbcontext.SaveChanges();
                            return InsertSuccess();
                        }
                    }
                    else
                    {
                        _NhanSuService.InsertNhatKySuDung("Chấm công",
                           "Cập nhật công cho nhân viên " + model.TenNhanVien + " (" + model.MaNhanVien + ")" + noidung,
                           "Cập nhật công cho nhân viên " + model.TenNhanVien + " (" + model.MaNhanVien + ")" + chitiet + calam,
                       ID_DonVi, ID_NhanVien, 2);
                        resul = _NhanSuService.UpdateChamThuCong(model, ID_NhanVien, ID_DonVi);
                        if (resul.ErrorCode == false)
                        {
                            _dbcontext.SaveChanges();
                            return UpdateSuccess();
                        }
                    }
                    return ActionFalseNotData(resul.Data);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Lấy chi tiết công chấm theo ngày để sửa đổi
        /// </summary>
        /// <param name="IdCong"></param>
        /// <param name="date"> thang,nam,ngay</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetCongBoSungByNgay(Guid idDonVi, Guid idNhanVien, Guid idCaLamViec, string date)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);

                    var dateNew = date.Split(',');
                    if (dateNew.Length < 3)
                    {
                        return ActionFalseNotData("Tham số không hợp lệ");
                    }
                    var data = _NhanSuService.GetConBoSungByCong(idDonVi, idNhanVien, idCaLamViec, new DateTime(int.Parse(dateNew[1]), int.Parse(dateNew[0]), int.Parse(dateNew[2])))
                        .Select(o => new
                        {
                            o.ID,
                            o.SoGioOT,
                            o.SoPhutDiMuon,
                            o.Cong,
                            o.GhiChu,
                            o.TrangThai
                        }).FirstOrDefault();
                    return ActionTrueData(data);
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Xuất excel bảng chấm công
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportExcelToChamCong(ChamCongFilter model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    List<ChamCongModel> lst = _NhanSuService.GetChamCongFilter(model).OrderBy(o => o.MaCa).ThenBy(o => o.Thang).ToList();
                    var chinhanh = _NhanSuService.GetDonViExportChamCong(model);
                    var fileName = "DanhSachChamCong_" + model.TuNgay.Value.ToString("MM_yyyy");
                    string time = "Tháng " + model.TuNgay.Value.Month + " Năm " + model.TuNgay.Value.Year;
                    string kyText = "Thời gian: " + model.TuNgay.Value.ToString("dd/MM/yyyy") + " - " + model.DenNgay.Value.ToString("dd/MM/yyyy");
                    var columhide = "";
                    var numberHide = Enumerable.Range(1, 31 - DateTime.DaysInMonth(model.TuNgay.Value.Year, model.TuNgay.Value.Month)).ToArray();
                    var listRowPan = lst.GroupBy(o => o.MaCa).Select(o => o.Count()).ToArray();
                    switch (numberHide.Length)
                    {
                        case 1:
                            columhide = "36";
                            break;
                        case 2:
                            columhide = "35_36";
                            break;
                        case 3:
                            columhide = "34_35_36";
                            break;
                    }

                    Class_officeDocument _Class_officeDocument = new Class_officeDocument(_dbcontext);
                    DataTable excel = _Class_officeDocument.ToDataTable<ChamCongModel>(lst);

                    excel.Columns.Remove("MaCa");
                    excel.Columns.Remove("ID_CaLamViec");
                    excel.Columns.Remove("ID_NhanVien");
                    excel.Columns.Remove("ID_PhieuPhanCa");
                    excel.Columns.Remove("TrangThaiNV");
                    excel.Columns.Remove("LoaiCong");
                    excel.Columns.Remove("Thang");
                    excel.Columns.Remove("Nam");
                    excel.Columns.Remove("NguoiTao");
                    excel.Columns.Remove("GioVao");
                    excel.Columns.Remove("GioRa");
                    excel.Columns.Remove("TuNgay");
                    excel.Columns.Remove("DenNgay");
                    excel.Columns.Remove("TotalRow");
                    excel.Columns.Remove("TotalPage");

                    excel.Columns.Remove("DisNgay1");
                    excel.Columns.Remove("DisNgay2");
                    excel.Columns.Remove("DisNgay3");
                    excel.Columns.Remove("DisNgay4");
                    excel.Columns.Remove("DisNgay5");
                    excel.Columns.Remove("DisNgay6");
                    excel.Columns.Remove("DisNgay7");
                    excel.Columns.Remove("DisNgay8");
                    excel.Columns.Remove("DisNgay9");
                    excel.Columns.Remove("DisNgay10");

                    excel.Columns.Remove("DisNgay11");
                    excel.Columns.Remove("DisNgay12");
                    excel.Columns.Remove("DisNgay13");
                    excel.Columns.Remove("DisNgay14");
                    excel.Columns.Remove("DisNgay15");
                    excel.Columns.Remove("DisNgay16");
                    excel.Columns.Remove("DisNgay17");
                    excel.Columns.Remove("DisNgay18");
                    excel.Columns.Remove("DisNgay19");

                    excel.Columns.Remove("DisNgay20");
                    excel.Columns.Remove("DisNgay21");
                    excel.Columns.Remove("DisNgay22");
                    excel.Columns.Remove("DisNgay23");
                    excel.Columns.Remove("DisNgay24");
                    excel.Columns.Remove("DisNgay25");
                    excel.Columns.Remove("DisNgay26");
                    excel.Columns.Remove("DisNgay27");
                    excel.Columns.Remove("DisNgay28");
                    excel.Columns.Remove("DisNgay29");
                    excel.Columns.Remove("DisNgay30");
                    excel.Columns.Remove("DisNgay31");

                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/NhanSu/Teamplate_ChamCong.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/NhanSu/" + fileName + ".xlsx");
                    fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                    _Class_officeDocument.ExportExcelToFileChamCong(fileTeamplate, fileSave, excel, 6, 29, 24, true, columhide, time,
                                                                    kyText, chinhanh.Split('-')[0].Trim(),
                                                                    new int[] { 0 }, listRowPan, model.PhonBanId != null ? chinhanh.Split('-')[1].Trim() : string.Empty);
                    System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                    return ActionTrueData(fileSave);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GetChiTietBangCong(ChamCongFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                List<BangCongBSChiTietModel> dataBoSung = _NhanSuService.GetBangCongChiTiet(model);
                var congBoSung = dataBoSung.GroupBy(o => new
                {
                    o.ID_Ca,
                    o.MaCa,
                    o.TenCa
                })
                             .Select(o => new
                             {
                                 ID = o.Key.ID_Ca,
                                 MaCa = o.Key.MaCa,
                                 TenCa = o.Key.TenCa,
                                 ListCong = o.OrderBy(c => c.NgayCham).ToList()
                             });
                var congOT = dataBoSung.GroupBy(o => new
                {
                    o.ID_Ca,
                    o.MaCa,
                    o.TenCa
                })
                           .Select(o => new
                           {
                               ID = o.Key.ID_Ca,
                               MaCa = o.Key.MaCa,
                               TenCa = o.Key.TenCa,
                               ListCong = o.OrderBy(c => c.NgayCham).ToList()
                           }).Select(o => new
                           {
                               ID = o.ID,
                               MaCa = o.MaCa,
                               TenCa = o.TenCa,
                               TongOT = o.ListCong.FirstOrDefault().TongSoGioOT,
                               TongDiMuon = o.ListCong.FirstOrDefault().TongSoPhutDiMuon,
                               Ngay1 = o.ListCong.Any(c => c.NgayCham.Day == 1) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 1) : null,
                               Ngay2 = o.ListCong.Any(c => c.NgayCham.Day == 2) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 2) : null,
                               Ngay3 = o.ListCong.Any(c => c.NgayCham.Day == 3) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 3) : null,
                               Ngay4 = o.ListCong.Any(c => c.NgayCham.Day == 4) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 4) : null,
                               Ngay5 = o.ListCong.Any(c => c.NgayCham.Day == 5) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 5) : null,
                               Ngay6 = o.ListCong.Any(c => c.NgayCham.Day == 6) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 6) : null,
                               Ngay7 = o.ListCong.Any(c => c.NgayCham.Day == 7) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 7) : null,
                               Ngay8 = o.ListCong.Any(c => c.NgayCham.Day == 8) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 8) : null,
                               Ngay9 = o.ListCong.Any(c => c.NgayCham.Day == 9) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 9) : null,
                               Ngay10 = o.ListCong.Any(c => c.NgayCham.Day == 10) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 10) : null,
                               Ngay11 = o.ListCong.Any(c => c.NgayCham.Day == 11) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 11) : null,
                               Ngay12 = o.ListCong.Any(c => c.NgayCham.Day == 12) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 12) : null,
                               Ngay13 = o.ListCong.Any(c => c.NgayCham.Day == 13) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 13) : null,
                               Ngay14 = o.ListCong.Any(c => c.NgayCham.Day == 14) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 14) : null,
                               Ngay15 = o.ListCong.Any(c => c.NgayCham.Day == 15) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 15) : null,
                               Ngay16 = o.ListCong.Any(c => c.NgayCham.Day == 16) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 16) : null,
                               Ngay17 = o.ListCong.Any(c => c.NgayCham.Day == 17) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 17) : null,
                               Ngay18 = o.ListCong.Any(c => c.NgayCham.Day == 18) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 18) : null,
                               Ngay19 = o.ListCong.Any(c => c.NgayCham.Day == 19) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 19) : null,
                               Ngay20 = o.ListCong.Any(c => c.NgayCham.Day == 20) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 20) : null,
                               Ngay21 = o.ListCong.Any(c => c.NgayCham.Day == 21) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 21) : null,
                               Ngay22 = o.ListCong.Any(c => c.NgayCham.Day == 22) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 22) : null,
                               Ngay23 = o.ListCong.Any(c => c.NgayCham.Day == 23) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 23) : null,
                               Ngay24 = o.ListCong.Any(c => c.NgayCham.Day == 24) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 24) : null,
                               Ngay25 = o.ListCong.Any(c => c.NgayCham.Day == 25) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 25) : null,
                               Ngay26 = o.ListCong.Any(c => c.NgayCham.Day == 26) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 26) : null,
                               Ngay27 = o.ListCong.Any(c => c.NgayCham.Day == 27) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 27) : null,
                               Ngay28 = o.ListCong.Any(c => c.NgayCham.Day == 28) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 28) : null,
                               Ngay29 = o.ListCong.Any(c => c.NgayCham.Day == 29) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 29) : null,
                               Ngay30 = o.ListCong.Any(c => c.NgayCham.Day == 30) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 30) : null,
                               Ngay31 = o.ListCong.Any(c => c.NgayCham.Day == 31) ? o.ListCong.FirstOrDefault(c => c.NgayCham.Day == 31) : null,

                           });
                return ActionTrueData(new { congBoSung, congOT });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  Lấy danh sách role người dùng cho danh mục ngày nghỉ lễ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetRoleNgayNghiLe()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    NhanSuService _NhanSuService = new NhanSuService(_dbcontext);
                    var RoleModel = new RoleModel() { Delete = true, Insert = true, Update = true, View = true, NhanSu = _NhanSuService.GetDangKySuDungHRM() };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.View = listQuyen.Any(o => o.Equals(RoleNhanSu.NgayNghiLe_XemDS));
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.NgayNghiLe_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.NgayNghiLe_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.NgayNghiLe_Xoa));
                    }
                    return ActionTrueData(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetRoleCaLamViec()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleModel() { Delete = true, Insert = true, Update = true, View = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.View = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_XemDS));
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.CaLamViec_Xoa));
                    }
                    return ActionTrueData(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetRoleKyHieuCong()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleModel() { Delete = true, Insert = true, Update = true, View = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.View = listQuyen.Any(o => o.Equals(RoleNhanSu.KyHieuCong_XemDS));
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.KyHieuCong_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.KyHieuCong_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.KyHieuCong_Xoa));
                    }
                    return ActionTrueData(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult GetRoleThietLapLuong()
        {
            try
            {
                using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
                {
                    var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                    var RoleModel = new RoleModel() { Delete = true, Insert = true, Update = true, View = true };
                    if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                    {
                        classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                        var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                        RoleModel.View = listQuyen.Any(o => o.Equals(RoleNhanSu.ThietLapLuong_XemDS));
                        RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleNhanSu.ThietLapLuong_ThemMoi));
                        RoleModel.Update = listQuyen.Any(o => o.Equals(RoleNhanSu.ThietLapLuong_CapNhat));
                        RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleNhanSu.ThietLapLuong_Xoa));
                    }
                    return ActionTrueData(RoleModel);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        ///  Xuất excel bảng công
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportBangCong(ChamCongFilter model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    List<ExportBangCong> result = _NhanSuService.GetExportBangCong(model);
                    var modelNhanVien = _NhanSuService.GetNhanVienById(model.IDNhanVien ?? new Guid());
                    var fileName = "BangCong_" + modelNhanVien.MaNhanVien + "_" + model.TuNgay.Value.ToString("MM_yyyy");
                    string time = "Tháng " + model.TuNgay.Value.Month + " Năm " + model.TuNgay.Value.Year;
                    string kyText = "Thời gian: " + model.TuNgay.Value.ToString("dd/MM/yyyy") + " - " + model.DenNgay.Value.ToString("dd/MM/yyyy");
                    string textNhanVien = modelNhanVien.MaNhanVien + " - " + modelNhanVien.TenNhanVien;
                    var numberHide = Enumerable.Range(1, 31 - DateTime.DaysInMonth(model.TuNgay.Value.Year, model.TuNgay.Value.Month)).ToArray();
                    var listRowPan = result.GroupBy(o => o.MaCa).Select(o => o.Count()).ToArray();

                    Class_officeDocument _Class_officeDocument = new Class_officeDocument(_dbcontext);
                    DataTable excel = _Class_officeDocument.ToDataTable<ExportBangCong>(result);
                    excel.Columns.Remove("GioVao");
                    excel.Columns.Remove("GioRa");
                    excel.Columns.Remove("GioVaoText");
                    excel.Columns.Remove("GioRaText");
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/NhanSu/Teamplate_BangCong.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/NhanSu/" + fileName + ".xlsx");
                    fileSave = _Class_officeDocument.createFolder_Download(fileSave);
                    _Class_officeDocument.ExportExcelToFileBangCong(fileTeamplate, fileSave, excel, 5, 29, 24, true, string.Empty, time, kyText, textNhanVien, new int[] { 0, 1 }, listRowPan);
                    System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                    return ActionTrueData(fileSave);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Tìm kiếm danh sách bảng lưởng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetSearchForBangLuong(BangLuongFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetKyBangLuongFilter(model);
                var result = data.Skip((model.pageNow - 1) * model.pageSize).Take(model.pageSize).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.MaBangLuong,
                    o.NgayTao,
                    o.NguoiTao,
                    o.TenBangLuong,
                    o.GhiChu,
                    o.NgayThanhToanLuong,
                    o.ID_NhanVienDuyet,
                    o.TuNgay,
                    o.DenNgay,
                    NguoiDuyet = o.NS_NhanVienDuyet != null ? o.NS_NhanVienDuyet.TenNhanVien : string.Empty,
                    o.TrangThai
                });
                var count = data.Count();
                int page = 0;
                var listpage = GetListPage(count, model.pageSize, model.pageNow, ref page);
                return ActionTrueData(new
                {
                    data = result,
                    listpage = listpage,
                    pagenow = model.pageNow,
                    pageview = "Hiển thị " + ((model.pageNow - 1) * model.pageSize + 1) + " - " + ((model.pageNow - 1) * model.pageSize + result.Count()) + " trên tổng số " + count + " bảng lương",
                    isprev = model.pageNow > 3 && page > 5,
                    isnext = model.pageNow < page - 2 && page > 5,
                    countpage = page
                });

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GetAllBangLuong(BangLuongFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                List<BangLuongDTO> result = _NhanSuService.GetAllBangLuong(model);
                var count = result.Count() > 0 ? result[0].TotalRow : 0;
                int page = 0;
                var listpage = GetListPage(count ?? 0, model.pageSize, model.pageNow, ref page);
                return ActionTrueData(new
                {
                    data = result,
                    listpage = listpage,
                    pagenow = model.pageNow,
                    pageview = "Hiển thị " + ((model.pageNow - 1) * model.pageSize + 1) + " - " + ((model.pageNow - 1) * model.pageSize + result.Count()) + " trên tổng số " + count + " bảng lương",
                    isprev = model.pageNow > 3 && page > 5,
                    isnext = model.pageNow < page - 2 && page > 5,
                    countpage = page
                });

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        [HttpGet]
        public IHttpActionResult CheckBangLuongExist(Guid idDonVi, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var check = _NhanSuService.CheckBangLuongExist(idDonVi, fromDate, toDate);
                    if (check.ErrorCode)
                    {
                        return ActionFalseNotData(check.Data);
                    }
                    else
                    {
                        return ActionTrueData(check.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        /// <summary>
        /// Thêm mới bảng lương
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertBangLuong(NS_BangLuong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    model.ID = Guid.NewGuid();
                    model.NgayTao = DateTime.Now;
                    var result = _NhanSuService.CreateBangLuong(model, ID_NhanVien, ID_DonVi);
                    if (result.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return ActionTrueNotData("Thêm mới bảng lương thành công");
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        [HttpGet]
        public IHttpActionResult UpdateNgayThanhToanLuong(Guid idBangLuong, DateTime ngaythanhtoan)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    NS_BangLuong bl = db.NS_BangLuong.Find(idBangLuong);
                    var _NhanSuService = new NhanSuService(db);

                    if (bl.NgayThanhToanLuong == null)
                    {
                        bl.NgayThanhToanLuong = ngaythanhtoan;
                        bl.TrangThai = (int)commonEnumHellper.eBangLuong.dathanhtoan;
                        db.SaveChanges();
                    }
                    // update trangthai cong after update trangthai bangluong
                    _NhanSuService.UpdateStatusCongBoSung_WhenCreatBangLuong(idBangLuong, bl.TuNgay ?? DateTime.Now, bl.DenNgay ?? DateTime.Now);
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception e)
                {
                    return Exeption(e);
                }
            }
        }

        [HttpPost]
        public IHttpActionResult PostBangLuong([FromBody] JObject data)
        {
            using (var _dbcontext = SystemDBContext.GetDBContext())
            {
                using (var trans = _dbcontext.Database.BeginTransaction())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    BangLuongDTO bangluong = data["bangluong"].ToObject<BangLuongDTO>();
                    List<NS_BangLuong_ChiTiet> bangluongct = data["bangluongchitiet"].ToObject<List<NS_BangLuong_ChiTiet>>();
                    string mabangluong = bangluong.MaBangLuong;
                    var tenbangluong = bangluong.TenBangLuong;
                    Guid idBangLuong = Guid.NewGuid();
                    List<NS_BangLuong_ChiTiet> details = new List<NS_BangLuong_ChiTiet>();
                    var history = GetHistory();
                    history.ChucNang = "Bảng lương";

                    var inforOld = string.Empty;
                    try
                    {
                        if (bangluong.ID == Guid.Empty)
                        {
                            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.insert;
                            Guid idKy = Guid.NewGuid();
                            if (mabangluong == string.Empty || mabangluong == null)
                            {
                                mabangluong = _NhanSuService.GetMaBangLuongMax_byTemp(bangluong.ID_DonVi);
                            }
                            else
                            {
                                if (_dbcontext.NS_BangLuong.Any(x => x.MaBangLuong == mabangluong))
                                    return ActionFalseNotData("Mã bảng lương đã tồn tại");
                            }

                            NS_BangLuong bl = new NS_BangLuong
                            {
                                ID = idBangLuong,
                                ID_DonVi = bangluong.ID_DonVi,
                                ID_NhanVienDuyet = bangluong.ID_NhanVienDuyet,
                                MaBangLuong = mabangluong.ToUpper(),
                                TenBangLuong = tenbangluong,
                                TuNgay = bangluong.TuNgay,
                                DenNgay = bangluong.DenNgay,
                                NguoiTao = bangluong.NguoiTao,
                                TrangThai = bangluong.TrangThai,
                                GhiChu = bangluong.GhiChu,
                                NgayTao = DateTime.Now,
                                SuDungHRM = _dbcontext.HT_CongTy.FirstOrDefault().DangKyNhanSu ?? false,
                                LaBangLuongBoSung = false,
                            };

                            var maxPL = _NhanSuService.GetMaPhieuLuongChiTiet_Max(Guid.Empty);
                            foreach (var item in bangluongct)
                            {
                                item.ID = Guid.NewGuid();
                                item.MaBangLuongChiTiet = string.Concat("PL", _NhanSuService.GetChuoiSoKhong(maxPL.ToString()), maxPL);
                                item.ID_BangLuong = idBangLuong;
                                item.TrangThai = bangluong.TrangThai; // same TrangThai in NS_BangLuong
                                item.NgayTao = DateTime.Now;
                                details.Add(item);
                                maxPL = maxPL + 1;
                            }
                            _dbcontext.NS_BangLuong.Add(bl);
                            _dbcontext.NS_BangLuong_ChiTiet.AddRange(details);
                        }
                        else
                        {
                            history.LoaiNhatKy = (int)commonEnumHellper.TypeHoatDong.update;

                            NS_BangLuong objUp = _dbcontext.NS_BangLuong.Find(bangluong.ID);
                            if (string.IsNullOrEmpty(mabangluong))
                            {
                                mabangluong = objUp.MaBangLuong;
                            }
                            else
                            {
                                if (_dbcontext.NS_BangLuong.Any(x => x.MaBangLuong == mabangluong && x.ID != bangluong.ID))
                                    return ActionFalseNotData("Mã bảng lương đã tồn tại");
                            }
                            idBangLuong = objUp.ID;
                            objUp.ID_NhanVienDuyet = bangluong.ID_NhanVienDuyet;
                            objUp.MaBangLuong = mabangluong.ToUpper();
                            objUp.TenBangLuong = tenbangluong;
                            objUp.TuNgay = bangluong.TuNgay;
                            objUp.DenNgay = bangluong.DenNgay;
                            objUp.NguoiSua = bangluong.NguoiTao;
                            objUp.GhiChu = bangluong.GhiChu;
                            objUp.NgaySua = DateTime.Now;
                            objUp.TrangThai = bangluong.TrangThai;

                            inforOld = _dbcontext.Database.SqlQuery<string>(" SELECT dbo.Diary_BangLuong('" + idBangLuong + "')").First();
                            inforOld = string.Concat(" <br /> <br /> <b> Thông tin cũ: </b>", inforOld);

                            //  if bangluong da thanhtoan --> get chi tiet phieuluong (ID_QuyCT, ID_bangluongCT, ID_NhanVien)
                            var quyct = (from qct in _dbcontext.Quy_HoaDon_ChiTiet
                                         join blct in _dbcontext.NS_BangLuong_ChiTiet on qct.ID_BangLuongChiTiet equals blct.ID
                                         where blct.ID_BangLuong == idBangLuong
                                         select new { qct.ID, qct.ID_NhanVien }).ToList();

                            _NhanSuService.DeleteBangLuongChiTietBy(idBangLuong);

                            // add again
                            var maxPL = _NhanSuService.GetMaPhieuLuongChiTiet_Max(idBangLuong);
                            foreach (var item in bangluongct)
                            {
                                item.ID = Guid.NewGuid();
                                item.MaBangLuongChiTiet = string.Concat("PL", _NhanSuService.GetChuoiSoKhong(maxPL.ToString()), maxPL);
                                item.ID_BangLuong = idBangLuong;
                                item.TrangThai = bangluong.TrangThai;
                                item.NguoiSua = objUp.NguoiSua;
                                item.NgaySua = DateTime.Now;
                                item.NguoiTao = objUp.NguoiTao;
                                item.NgayTao = objUp.NgayTao;
                                details.Add(item);
                                maxPL = maxPL + 1;
                            }
                            _dbcontext.NS_BangLuong_ChiTiet.AddRange(details);

                            // update again IDbangluongnew to qct
                            var xx = (from ctlNew in details
                                      join qct in quyct on ctlNew.ID_NhanVien equals qct.ID_NhanVien
                                      select new
                                      {
                                          ID = qct.ID,
                                          ID_BangLuongChiTiet = ctlNew.ID
                                      }).ToList();

                            foreach (var item in xx)
                            {
                                var qct = _dbcontext.Quy_HoaDon_ChiTiet.Find(item.ID);
                                qct.ID_BangLuongChiTiet = item.ID_BangLuongChiTiet;
                            }
                        }
                        
                        _dbcontext.SaveChanges();
                        trans.Commit();

                        _NhanSuService.UpdateStatusCongBoSung_WhenCreatBangLuong(idBangLuong, bangluong.TuNgay, bangluong.DenNgay);
                        var inforNew = _dbcontext.Database.SqlQuery<string>(" SELECT dbo.Diary_BangLuong('" + idBangLuong + "')").First();
                        history.NoiDung = history.LoaiNhatKy == (int)commonEnumHellper.TypeHoatDong.insert ? "Thêm mới " : "Cập nhật ";
                        history.NoiDung = string.Concat(history.NoiDung, mabangluong);
                        history.NoiDungChiTiet = string.Concat("<b> Thông tin bảng lương: </b>", inforNew, inforOld);
                        _dbcontext.HT_NhatKySuDung.Add(history);
                        _dbcontext.SaveChanges();

                        return ActionTrueData(new { ID = idBangLuong, MaBangLuong = mabangluong, TenBangLuong = tenbangluong });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return Exeption(ex);
                    }
                }
            }
        }

        [HttpGet]
        public IHttpActionResult HuyBangLuong(Guid idBangLuong)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var diary = GetHistory();
                    var result = _NhanSuService.HuyBangLuong(idBangLuong, diary);
                    if (result.ErrorCode == false)
                    {
                        return ActionTrueNotData("Hủy bảng lương thành công");
                    }
                    return ActionFalseNotData("Hủy bảng lương thất bại");
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetListDebitSalaryDetail(Guid idBangLuong, string textSearch, int currentPage, int pageSize)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var diary = GetHistory();
                    List<QuyChiTietPhieuLuong> data = _NhanSuService.GetListDebitSalaryDetail(idBangLuong, textSearch, currentPage, pageSize);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult TinhLuongNhanVien(ParamSearchLuong param)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    List<BangLuongChiTietDTO> bangluong = _NhanSuService.TinhLuongNhanVien(param);
                    var luongchinh = bangluong.Sum(x => x.LuongChinh);
                    var luongOT = bangluong.Sum(x => x.LuongOT);
                    var hoahong = bangluong.Sum(x => x.ChietKhau);
                    var phucap = bangluong.Sum(x => x.PhuCap);
                    var khenthuong = bangluong.Sum(x => x.KhenThuong);
                    var giamtru = bangluong.Sum(x => x.TongTienPhat + x.GiamTruCoDinh_TheoPTram);
                    var tongluong = bangluong.Sum(x => x.LuongThucNhan);

                    return ActionTrueData(new
                    {
                        LuongChinh = luongchinh,
                        LuongOT = luongOT,
                        HoaHong = hoahong,
                        PhuCap = phucap,
                        KhenThuong = khenthuong,
                        GiamTru = giamtru,
                        TongLuong = tongluong,
                        data = bangluong,
                    });
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetLuongChinh_ofNhanVien(Guid idChiNhanh, Guid idNhanVien, DateTime tungay, DateTime denngay, int ngaycongchuan)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    List<LuongChinhDTO> luongchinh = _NhanSuService.GetLuongChinh_ofNhanVien(idChiNhanh, idNhanVien, tungay, denngay, ngaycongchuan);
                    var xx = luongchinh.GroupBy(x => new { x.ID_NhanVien, x.MaNhanVien, x.TenNhanVien }).Select(x => new
                    {
                        ID_NhanVien = x.Key.ID_NhanVien,
                        MaNhanVien = x.Key.MaNhanVien,
                        TenNhanVien = x.Key.TenNhanVien,
                        LstLuongChinh = x.GroupBy(o => o.LoaiLuong).Select(o => new
                        {
                            LoaiLuong = o.Key,
                            ListCa = o.GroupBy(y => new { y.ID_CaLamViec, y.TenCa }).Select(y =>
                           new
                           {
                               ID_CaLamViec = y.Key.ID_CaLamViec,
                               TenCa = y.Key.TenCa,
                               LstDetail = y.ToList(),
                           })
                        }),
                    });
                    return ActionTrueData(xx);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetLuongOT_ofNhanVien(Guid idChiNhanh, List<string> lstIDNhanVien, DateTime tungay, DateTime denngay, int ngaycongchuan)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    List<LuongOT> luongOT = _NhanSuService.GetLuongOT_ofNhanVien(idChiNhanh, lstIDNhanVien, tungay, denngay, ngaycongchuan);
                    var xx = luongOT.GroupBy(x => new { x.ID_NhanVien, x.MaNhanVien, x.TenNhanVien }).Select(x => new
                    {
                        ID_NhanVien = x.Key.ID_NhanVien,
                        MaNhanVien = x.Key.MaNhanVien,
                        TenNhanVien = x.Key.TenNhanVien,
                        LstLuongOT = x.GroupBy(o => o.LoaiLuong).Select(o => new
                        {
                            LoaiLuong = o.Key,
                            ListCa = o.GroupBy(y => new { y.ID_CaLamViec, y.TenCa }).Select(y =>
                            new
                            {
                                ID_CaLamViec = y.Key.ID_CaLamViec,
                                TenCa = y.Key.TenCa,
                                LstDetail = y.ToList(),
                            })
                        }),
                    });
                    return ActionTrueData(xx);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult PhuCap_ofNhanVien(Guid idChiNhanh, List<string> lstIDNhanVien, DateTime tungay, DateTime denngay)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    List<PhuCap> phucap = _NhanSuService.PhuCap_ofNhanVien(idChiNhanh, lstIDNhanVien, tungay, denngay);
                    var lst = phucap.GroupBy(x => new { x.ID_NhanVien, x.MaNhanVien, x.TenNhanVien }).Select(x => new
                    {
                        ID_NhanVien = x.Key.ID_NhanVien,
                        MaNhanVien = x.Key.MaNhanVien,
                        TenNhanVien = x.Key.TenNhanVien,
                        LstLoaiPhuCap = x.GroupBy(o => o.LoaiPhuCap).Select(o => new
                        {
                            LoaiPhuCap = o.Key,
                            LstDetail = o.ToList(),
                        }),
                    });
                    return ActionTrueData(lst);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GiamTru_ofNhanVien(Guid idChiNhanh, List<string> lstIDNhanVien, DateTime tungay, DateTime denngay)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    List<GiamTru> giamtru = _NhanSuService.GiamTru_ofNhanVien(idChiNhanh, lstIDNhanVien, tungay, denngay);
                    var lst = giamtru.GroupBy(x => new { x.ID_NhanVien, x.MaNhanVien, x.TenNhanVien }).Select(x => new
                    {
                        ID_NhanVien = x.Key.ID_NhanVien,
                        MaNhanVien = x.Key.MaNhanVien,
                        TenNhanVien = x.Key.TenNhanVien,
                        LstLoaiGiamTru = x.GroupBy(o => o.LoaiGiamTru).Select(o => new
                        {
                            LoaiGiamTru = o.Key,
                            LstDetail = o.ToList(),
                        }),
                    });
                    return ActionTrueData(lst);
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Cập nhật bảng lương
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ID_DonVi"></param>
        /// <param name="ID_NhanVien"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateBangLuong(NS_BangLuong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    var result = _NhanSuService.UpdateBangLuong(model, ID_NhanVien, ID_DonVi);
                    if (result.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return ActionTrueNotData("Cập nhật bảng lương thành công");
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        /// <summary>
        /// Lấy danh sách bảng lương chi tiết
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetBangLuongChiTiet(Guid Id, int currentPage, int pageSize)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                List<BangLuongChiTietModel> data = _NhanSuService.GetAllBangLuongChiTiet(Id, currentPage, pageSize);
                var result = data.GroupBy(o => new { o.ID_NhanVien, o.MaNhanVien, o.TenNhanVien, o.DaNghiViec })
                    .Select(o => new
                    {
                        ID_NhanVien = o.Key.ID_NhanVien,
                        MaNhanVien = o.Key.MaNhanVien,
                        TenNhanVien = o.Key.TenNhanVien,
                        DaNghiViec = o.Key.DaNghiViec,
                        data = o.ToList()
                    }).ToList();
                var count = data.Count() > 0 ? (int)data[0].TotalRow : 0;
                int page = 0;
                var listpage = GetListPage(count, pageSize, currentPage, ref page);
                return ActionTrueData(new
                {
                    data = result,
                    listpage = listpage,
                    pagenow = currentPage + 1,
                    page = pageSize,
                    pageview = string.Concat("Hiển thị " + (currentPage * pageSize + 1), " - ", (currentPage * pageSize + data.Count()), " trên tổng số ", count, " bảng lương nhân viên"),
                    isprev = currentPage > 3 && page > 5,
                    isnext = currentPage < page - 2 && page > 5,
                    countpage = page,
                    idbangluong = Id
                });
                return ActionTrueData(data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult GetBangLuongChiTiet_ofNhanVien(ParamSearchLuong lstParam)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                List<BangLuongChiTietModel> data = _NhanSuService.GetBangLuongChiTiet_ofNhanVien(lstParam);
                var count = data.Count() > 0 ? (int)data[0].TotalRow : 0;
                int page = 0;
                var listpage = GetListPage(count, (int)lstParam.PageSize, lstParam.CurrentPage, ref page);
                var currentPageView = lstParam.CurrentPage + 1;
                return ActionTrueData(new
                {
                    data = data,
                    listpage = listpage,
                    pagenow = currentPageView,
                    page = lstParam.PageSize,
                    pageview = string.Concat("Hiển thị " + (lstParam.CurrentPage * lstParam.PageSize + 1), " - ", (lstParam.CurrentPage * lstParam.PageSize + data.Count()), " trên tổng số ", count, " bảng lương nhân viên"),
                    isprev = currentPageView > 3 && page > 5,
                    isnext = currentPageView < page - 2 && page > 5,
                    countpage = page,
                });
                return ActionTrueData(data);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }


        /// <summary>
        ///  xuất excel bảng lương chi tiết
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ExportExcelBangLuongChiTiet(NS_BangLuong model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var lst = _NhanSuService.GetAllBangLuongChiTiet(model.ID, 0, 200);
                var fileName = "DanhSachBangLuong_" + model.MaBangLuong;

                var listRowPan = lst.GroupBy(o => o.MaNhanVien).Select(o => o.Count()).ToArray();
                Class_officeDocument _Class_officeDocument = new Class_officeDocument(_dbcontext);
                DataTable excel = _Class_officeDocument.ToDataTable<BangLuongChiTietModel>(lst);
                excel.Columns.Remove("ID_NhanVien");
                excel.Columns.Remove("ID_BangLuong_ChiTiet");
                excel.Columns.Remove("LuongTruocGiamTru");
                excel.Columns.Remove("KhenThuong");
                excel.Columns.Remove("KyLuat");
                excel.Columns.Remove("DaTra");
                excel.Columns.Remove("NgayThanhToan");
                excel.Columns.Remove("DaNghiViec");

                excel.Columns.Remove("TotalRow");
                excel.Columns.Remove("TotalPage");
                excel.Columns.Remove("TongLuongChinh");
                excel.Columns.Remove("TongLuongOT");
                excel.Columns.Remove("TongPhuCapCoBan");
                excel.Columns.Remove("TongPhuCapKhac");
                excel.Columns.Remove("TongKhenThuong");
                excel.Columns.Remove("TongKyLuat");
                excel.Columns.Remove("TongChietKhau");
                excel.Columns.Remove("TongLuongTruocGiamTru");
                excel.Columns.Remove("TongTienPhatAll");
                excel.Columns.Remove("TongLuongSauGiamTru");
                excel.Columns.Remove("TongTamUng");
                excel.Columns.Remove("TongThanhToan");
                excel.Columns.Remove("TongDaTra");
                excel.Columns.Remove("TongConLai");
                excel.Columns.Remove("TongNgayCongThuc");

                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/NhanSu/Teamplate_BangLuong.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/NhanSu/" + fileName + ".xlsx");
                fileSave = _Class_officeDocument.createFolder_Download(fileSave);

                _Class_officeDocument.ExportExcelToFileBangLuongCT(fileTeamplate, fileSave, excel, 4, 29, 24, true, _NhanSuService.GetDangKySuDungHRM() ? string.Empty : "9_8", "", model.MaBangLuong + " - " + model.TenBangLuong,
                                                                new int[] { 0, 1 }, listRowPan);
                System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                return ActionTrueData(fileSave);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Phê duyệt bảng lương
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult PheDuyetBangLuong(NS_BangLuong model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    var history = GetHistory();
                    history.ChucNang = "Bảng lương";
                    var result = _NhanSuService.PheDuyetBangLuong(model.ID, history);
                    if (result.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return ActionTrueNotData("Phê duyệt bảng lương thành công");
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult MoLaiBangLuongDaChot(NS_BangLuong model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    var history = GetHistory();
                    history.ChucNang = "Bảng lương";
                    var result = _NhanSuService.MoLaiBangLuongDaChot(model.ID, history);
                    if (result.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return ActionTrueNotData("Phê duyệt bảng lương thành công");
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult TinhLaiBangLuong(Guid idBangLuong, string nguoiSua)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var history = GetHistory();
                    history.ChucNang = "Bảng lương";
                    _NhanSuService.TinhLaiBangLuong(idBangLuong, nguoiSua);
                    return ActionTrueNotData("Đã cập nhật lại bảng lương");
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetRoleNhanSu()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {

                    return ActionTrueData(new NhanSuService(db).GetDangKySuDungHRM());
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Tìm kiếm danh sách loại bảo hiểm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetSearchForLoaiBaoHiem(SearchFilter model)
        {
            try
            {
                var _dbcontext = SystemDBContext.GetDBContext();
                var _NhanSuService = new NhanSuService(_dbcontext);
                var data = _NhanSuService.GetSearchLoaiBaoHiem(model);
                var result = data.Skip((model.pageNow - 1) * model.pageSize).Take(model.pageSize).AsEnumerable().Select(o => new
                {
                    o.ID,
                    o.NgayApDung,
                    o.NguoiTao,
                    o.NgayTao,
                    o.TenBaoHiem,
                    o.TyLeCongTy,
                    o.TyLeNhanVien,
                    o.TrangThai,
                    o.GhiChu
                });
                var count = data.Count();
                int page = 0;
                var listpage = GetListPage(count, model.pageSize, model.pageNow, ref page);
                return ActionTrueData(new
                {
                    data = result,
                    listpage = listpage,
                    pagenow = model.pageNow,
                    pageview = "Hiển thị " + ((model.pageNow - 1) * model.pageSize + 1) + " - " + ((model.pageNow - 1) * model.pageSize + result.Count()) + " trên tổng số " + count + " loại bảo hiểm",
                    isprev = model.pageNow > 3 && page > 5,
                    isnext = model.pageNow < page - 2 && page > 5,
                    countpage = page
                });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Thêm mới loại bảo hiểm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult InsertLoaiBaoHiem(NS_LoaiBaoHiem model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    model.ID = Guid.NewGuid();
                    model.NgayTao = DateTime.Now;
                    var history = GetHistory();
                    history.ChucNang = "Danh mục Loại bảo hiểm";
                    var result = _NhanSuService.InsertLoaiBaoHiem(model, history);
                    if (result.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return InsertSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        /// <summary>
        /// Cập nhật loại bảo hiểm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateLoaiBaoHiem(NS_LoaiBaoHiem model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    var history = GetHistory();
                    history.ChucNang = "Danh mục Loại bảo hiểm";
                    var result = _NhanSuService.UpdateLoaiBaoHiem(model, history);
                    if (result.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return UpdateSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        /// <summary>
        /// Xóa loại bảo hiểm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteLoaiBaoHiem(NS_LoaiBaoHiem model)
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    if (model == null)
                    {
                        return ErrorNotFound();
                    }
                    var history = GetHistory();
                    history.ChucNang = "Danh mục Loại bảo hiểm";
                    var result = _NhanSuService.DeleteLoaiBaoHiem(model, history);
                    if (result.ErrorCode)
                    {
                        _dbcontext.SaveChanges();
                        return DeleteSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// Khởi tạo tham số tính công
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult SetUpThamSoCong()
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    var _NhanSuService = new NhanSuService(_dbcontext);
                    var history = GetHistory();
                    history.ChucNang = "Nhân sự";
                    var result = _NhanSuService.KhoiTaoThamSoTinhCong(history);
                    if (result.ErrorCode)
                    {
                        return ActionTrueNotData("Khởi tạo dữ liệu thành công");
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }
        /// <summary>
        /// check khởi tạo chấm công, ngày nghỉ lễ,ca làm việc
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult CheckKhoiTaoDuLieuChamCong()
        {
            try
            {
                using (var _dbcontext = SystemDBContext.GetDBContext())
                {
                    return ActionTrueData(_dbcontext.NS_NgayNghiLe.Any());

                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }
        #endregion


        #region Quản lý máy chấm công
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListMayChamCong()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                //var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                //classDM_DonVi classDM_DonVi = new classDM_DonVi(db);
                //List<DM_DonVi> lstDonVi = classDM_DonVi.GetDonVi_User(ID_ND);
                MayChamCongServices cNSMayChamCong = new MayChamCongServices(db);
                List<GridMayChamCong> lstMayChamCong = cNSMayChamCong.GetAll().Select(p => new GridMayChamCong
                {
                    ID = p.ID,
                    MaMCC = p.MaMCC,
                    TenMCC = p.TenMCC,
                    TenHienThi = p.TenHienThi,
                    IP = p.IP,
                    MaChiNhanh = "",
                    TenChiNhanh = "",
                    SoSeries = p.SoSeries,
                    GhiChu = p.GhiChu,
                    IDChiNhanh = p.ID_ChiNhanh,
                    LoaiKetNoi = p.LoaiKetNoi,
                    CongCOM = p.CongCOM,
                    TocDoCom = p.TocDoCOM,
                    LoaiHinh = p.LoaiHinh,
                    MatMa = p.MatMa,
                    Port = p.Port,
                    IDMay = p.SoDangKy
                }).ToList();
                return ActionTrueData(new
                {
                    data = lstMayChamCong
                });
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult AddUpdateMayChamCong(NS_MayChamCong model, Guid ID_DonVi, Guid ID_NhanVien)
        {
            if (model == null)
            {
                return ActionFalseNotData("Không lấy được thông tin cần thêm mới");
            }
            try
            {
                HT_NguoiDung UserLogin = contant.GetUserCookies();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (model.ID != Guid.Empty)
                    {
                        //model.ID = Guid.NewGuid();
                        model.NguoiSua = UserLogin.TaiKhoan;
                        model.NgaySua = DateTime.Now;
                        MayChamCongServices mayChamCongServices = new MayChamCongServices(db);
                        JsonViewModel<string> result = mayChamCongServices.UpdateMayChamCong(model, ID_DonVi, ID_NhanVien);
                        if (result.ErrorCode)
                        {
                            return UpdateSuccess();
                        }
                        else
                        {
                            return ActionFalseNotData(result.Data);
                        }
                    }
                    else
                    {
                        model.ID = Guid.NewGuid();
                        model.NguoiTao = UserLogin.TaiKhoan;
                        MayChamCongServices mayChamCongServices = new MayChamCongServices(db);
                        JsonViewModel<string> result = mayChamCongServices.InsertMayChamCong(model, ID_DonVi, ID_NhanVien);
                        if (result.ErrorCode)
                        {
                            return InsertSuccess();
                        }
                        else
                        {
                            return ActionFalseNotData(result.Data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListMayChamCongByChiNhanh([FromBody] JObject objIn)
        {
            List<string> lstIDChiNhanh1 = objIn["IDs"].ToObject<List<string>>();
            if (lstIDChiNhanh1 != null)
            {
                lstIDChiNhanh1.RemoveAt(0);
            }
            //List<Guid> lstIDChiNhanh = lstIDChiNhanh1.ConvertAll(Guid.Parse);
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                MayChamCongServices cNSMayChamCong = new MayChamCongServices(db);
                List<GridMayChamCong> lstMayChamCong = cNSMayChamCong.GetByListIDChiNhanh(lstIDChiNhanh1);
                return ActionTrueData(new
                {
                    data = lstMayChamCong
                });
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetDuLieuCongTho([FromBody] JObject objIn)
        {
            Guid IDMayChamCong = objIn["IDMayChamCong"].ToObject<Guid>();
            int InYear = objIn["InYear"].ToObject<int>();
            int InMonth = objIn["InMonth"].ToObject<int>();
            int PageSize = objIn["PageSize"].ToObject<int>();
            int CurrentPage = objIn["CurrentPage"].ToObject<int>();
            using (SsoftvnContext context = SystemDBContext.GetDBContext())
            {
                MayChamCongServices mayChamCongServices = new MayChamCongServices(context);
                List<ObjGetDuLieuCongThoTheoThang> congtho = mayChamCongServices.GetDuLieuCongThoTheoThang(IDMayChamCong, InMonth, InYear, PageSize, CurrentPage);
                int count = 0;
                if (congtho.Count != 0)
                {
                    count = congtho[0].MaxRow;
                }
                int page = 0;
                var listpage = GetListPage(count, PageSize, CurrentPage, ref page);
                return ActionTrueData(new
                {
                    data = congtho,
                    ListPage = listpage,
                    PageView = "Hiển thị " + ((CurrentPage - 1) * PageSize + 1) + " - " + ((CurrentPage - 1) * PageSize + congtho.Count) + " trên tổng số " + count + " bản ghi",
                    NumOfPage = page
                });
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult TaiDuLieuMayChamCong([FromBody] JObject objIn)
        {
            HT_NguoiDung UserLogin = contant.GetUserCookies();
            Guid IDMayChamCong = objIn["IDMayChamCong"].ToObject<Guid>();
            int InMonth = objIn["InMonth"].ToObject<int>();
            int InYear = objIn["InYear"].ToObject<int>();
            DateTime firstDay = new DateTime(InYear, InMonth, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddMilliseconds(-1);
            string mess = "";
            using (SsoftvnContext context = SystemDBContext.GetDBContext())
            {
                MayChamCongServices mayChamCongServices = new MayChamCongServices(context);
                NS_MayChamCong mcc = mayChamCongServices.GetByIdMayChamCong(IDMayChamCong);
                if (mcc.SoDangKy == null || mcc.SoDangKy == "")
                {
                    mcc.SoDangKy = "1";
                }
                if (mcc.MatMa == "" || mcc.MatMa == null)
                {
                    mcc.MatMa = "0";
                }
                bool check = mayChamCongServices.TaiDuLieuFromDevice(mcc, firstDay, lastDay, UserLogin, ref mess);
                if (check)
                {
                    CookieStore.WriteProgress("1", "TaiDuLieuMayChamCong");
                    return ActionTrueNotData(mess);
                }
            }
            CookieStore.WriteProgress("1", "TaiDuLieuMayChamCong");
            return ActionFalseNotData(mess);
        }
        #endregion
    }
}
