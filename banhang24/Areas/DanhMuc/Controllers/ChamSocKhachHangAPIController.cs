using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.Entity;
using System.Net.Mail;
using System.Threading.Tasks;
using banhang24.Models;
using banhang24.Hellper;
using banhang24.App_API;
using libHT_NguoiDung;
using libQuy_HoaDon;
using libNS_NhanVien;
using lib_ChamSocKhachHang;
using System.IO;

namespace banhang24.Areas.DanhMuc.Controllers
{
    //[Route("api/DanhMuc/[Controller]")]
    public class ChamSocKhachHangAPIController : BaseApiController
    {
        #region insert
        [HttpPost, HttpGet]
        public IHttpActionResult SendEmail([FromBody] JObject objData)
        {
            List<EmailModel> lst = new List<EmailModel>();
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    if (objData["lst"] != null)
                    {
                        lst = objData["lst"].ToObject<List<EmailModel>>();
                        var from = "SSOFT VN <dangky@open24.vn>";
                        foreach (var item in lst)
                        {
                            var sms = new HeThong_SMS()
                            {
                                ID = Guid.NewGuid(),
                                ID_NguoiGui = item.ID_NguoiGui ?? new Guid("28FEF5A1-F0F2-4B94-A4AD-081B227F3B77"),
                                ID_DonVi = item.ID_DonVi ?? new Guid("D93B17EA-89B9-4ECF-B242-D03B8CDE71DE"),
                                ID_KhachHang = item.ID_KhachHang ?? Guid.Empty,
                                ID_HoaDon = item.ID_HoaDon ?? Guid.Empty,
                                SoDienThoai = item.TieuDe,
                                NoiDung = item.NoiDung,
                                LoaiTinNhan = item.LoaiTinNhan ?? 0,
                                TrangThai = 1,
                                SoTinGui = 1,
                                ThoiGianGui = DateTime.Now,
                            };
                            db.HeThong_SMS.Add(sms);

                            var message = new MailMessage();
                            message.From = new MailAddress(from);
                            message.Bcc.Add(new MailAddress(from));
                            message.To.Add(new MailAddress(item.TenDoiTuong + " <" + item.Email + ">"));
                            message.Subject = item.TieuDe;
                            message.Body = CreateEmailBody(item);
                            message.IsBodyHtml = true;

                            using (var smtp = new SmtpClient())
                            {
                                smtp.Port = 587;
                                smtp.Host = "smtp.gmail.com";
                                smtp.EnableSsl = true;
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = new NetworkCredential(item.Email, item.Password);
                                smtp.Send(message);
                            }
                        }
                        db.SaveChanges();

                        return Json(new { res = true });
                    }
                }
                return Json(new { res = false });
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }
        private string CreateEmailBody(EmailModel email)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Marketing/EmailTemplate.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{TieuDe}", email.TieuDe);
            body = body.Replace("{TenDoiTuong}", email.TenDoiTuong);
            body = body.Replace("{NoiDung}", email.NoiDung);
            return body;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostPhieuTuVan([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                ChamSocKhachHang objTuVan = data["objTuVan"].ToObject<ChamSocKhachHang>();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                #region TUVAN
                string sMa_TieuDe = string.Empty;
                if (objTuVan.Ma_TieuDe == null)
                {
                    sMa_TieuDe = classTuVan.GetMaPhieuChamSoc();
                }
                else
                {
                    sMa_TieuDe = objTuVan.Ma_TieuDe;
                }
                #region get ngaygio
                #endregion
                ChamSocKhachHang itemTuVan = new ChamSocKhachHang();
                itemTuVan.ID = Guid.NewGuid();
                itemTuVan.Ma_TieuDe = sMa_TieuDe;
                itemTuVan.ID_KhachHang = objTuVan.ID_KhachHang;
                itemTuVan.ID_LoaiTuVan = objTuVan.ID_LoaiTuVan;
                itemTuVan.ID_NhanVien = objTuVan.ID_NhanVien;
                itemTuVan.PhanLoai = 1;// 1= tu van
                itemTuVan.NhacNho = 0;
                itemTuVan.MucDoPhanHoi = 1;
                itemTuVan.NguoiTao = nguoidung.TaiKhoan;
                itemTuVan.ID_DonVi = nguoidung.ID_DonVi.Value;
                itemTuVan.ID_NhanVienQuanLy = nguoidung.ID_NhanVien;
                itemTuVan.NoiDung = objTuVan.NoiDung;
                itemTuVan.TraLoi = objTuVan.TraLoi;
                itemTuVan.NgayGio = objTuVan.NgayGio;
                itemTuVan.NgayTao = DateTime.Now;
                itemTuVan.NgayGioKetThuc = DateTime.Now;
                itemTuVan.ThoiGianHenLai = objTuVan.ThoiGianHenLai;
                itemTuVan.NgaySua = null;
                itemTuVan.TrangThai = objTuVan.TrangThai;
                #endregion

                string strIns = classTuVan.Add_PhieuTuVan(itemTuVan);
                string tenKhacHang = db.DM_DoiTuong.Where(p => p.ID == objTuVan.ID_KhachHang).FirstOrDefault().TenDoiTuong;
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = nguoidung.ID_NhanVien;
                hT_NhatKySuDung.ChucNang = "Tư vấn";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Tạo mới phiếu tư vấn: " + sMa_TieuDe + ", ngày tạo: " + itemTuVan.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + " cho khách hàng: " + tenKhacHang;
                hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu tư vấn: " + sMa_TieuDe + ", ngày tạo: " + itemTuVan.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + " cho khách hàng: " + tenKhacHang;
                hT_NhatKySuDung.LoaiNhatKy = 1;
                hT_NhatKySuDung.ID_DonVi = nguoidung.ID_DonVi.Value;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = objTuVan.ID }, objTuVan);
                }
            }
        }

        //công việc
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<NS_CongViec_PhanLoaiDTO> GetAllLoaiCongViec()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var tbl = (from cv in db.NS_CongViec_PhanLoai
                               select new NS_CongViec_PhanLoaiDTO()
                               {
                                   ID = cv.ID,
                                   LoaiCongViec = cv.LoaiCongViec,
                                   TrangThai = cv.TrangThai,
                                   NguoiTao = cv.NguoiTao,
                                   NgayTao = cv.NgayTao,
                                   NguoiSua = cv.NguoiSua,
                                   NgaySua = cv.NgaySua,
                               }).Where(x => x.TrangThai != 0);
                    return tbl.ToList();
                }
                else
                {
                    return new List<NS_CongViec_PhanLoaiDTO>();
                }
            }
        }

        //get list nhân viên liên quan đến nhân viên đăng nhập
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<Guid> GetListNhanVienLienQuanByIDLoGin(Guid idnvlogin)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<Guid> lstReTurn = new List<Guid>();
                List<Guid?> lstid_nsphongban = new List<Guid?>();
                NS_NhanVien nhanvienlogin = db.NS_NhanVien.Where(p => p.ID == idnvlogin).FirstOrDefault();
                HT_NguoiDung nguoidung = db.HT_NguoiDung.Where(p => p.ID_NhanVien == idnvlogin).FirstOrDefault();
                if (nguoidung.LaAdmin == false)
                {
                    if (nhanvienlogin.ID_NSPhongBan != null)
                    {
                        NS_PhongBan phongbanbylogin = db.NS_PhongBan.Where(p => p.ID == nhanvienlogin.ID_NSPhongBan).FirstOrDefault();
                        List<NS_PhongBan> lstPhongBanByIDParent = db.NS_PhongBan.Where(p => p.ID_PhongBanCha == phongbanbylogin.ID).ToList();
                        if (lstPhongBanByIDParent.Count() == 0)
                        {
                            lstid_nsphongban.Add(phongbanbylogin.ID);
                        }
                        else
                        {
                            lstid_nsphongban.Add(phongbanbylogin.ID);
                            foreach (var item in lstPhongBanByIDParent)
                            {
                                lstid_nsphongban = GetlistIDPhongBanChild(lstid_nsphongban, item.ID);
                                lstid_nsphongban.Add(item.ID);
                            }
                        }
                        List<NS_NhanVien> lstNhanVien = db.NS_NhanVien.ToList();
                        lstNhanVien = lstNhanVien.Where(p => lstid_nsphongban.Contains(p.ID_NSPhongBan)).ToList();
                        foreach (var item in lstNhanVien)
                        {
                            lstReTurn.Add(item.ID);
                        }
                        return lstReTurn;
                    }
                    else
                    {
                        lstReTurn.Add(idnvlogin);
                        return lstReTurn;
                    }
                }
                else
                {
                    return new List<Guid>();
                }
            }
        }

        public List<Guid?> GetlistIDPhongBanChild(List<Guid?> lstid_nsphongban, Guid idphongban)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<NS_PhongBan> lstPhongBanByIDParent = db.NS_PhongBan.Where(p => p.ID_PhongBanCha == idphongban).ToList();
                if (lstPhongBanByIDParent.Count() == 0)
                {
                    return lstid_nsphongban;
                }
                else
                {
                    foreach (var item in lstPhongBanByIDParent)
                    {
                        lstid_nsphongban = GetlistIDPhongBanChild(lstid_nsphongban, item.ID);

                        lstid_nsphongban.Add(item.ID);
                    }
                    return lstid_nsphongban;
                }
            }
        }

        [HttpGet]
        public List<Guid> GetListNhanVienLienQuanByIDLoGin_inDepartment(Guid? idnvlogin, Guid idChiNhanh, string funcName = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NhomNguoiDung classNhomNguoiDung = new classHT_NhomNguoiDung(db);
                List<Guid> lstReTurn = new List<Guid>();
                List<Guid?> lstid_nsphongban = new List<Guid?>();
                try
                {
                    if (idnvlogin == Guid.Empty || idnvlogin == null)
                    {
                        idnvlogin = contant.GetUserCookies().ID_NhanVien;// use at banhang (because appcache not new User)
                    }
                    // if LaAdmin = true OR NVien khong thuoc phong ban nao --> return []
                    var htNguoiDung = db.HT_NguoiDung.Where(x => x.ID_NhanVien == idnvlogin).FirstOrDefault();
                    var laAdmin = false;
                    if (htNguoiDung != null)
                    {
                        laAdmin = htNguoiDung.LaAdmin == true;
                    }

                    if (laAdmin == false)
                    {
                        var roleView_inDepatment = string.Empty;
                        var roleView_inBranch = string.Empty;

                        if (funcName != null)
                        {
                            switch (funcName)
                            {
                                case RoleKey.DatHang:
                                    roleView_inDepatment = RoleKey.DatHang_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.DatHang_XemDS_HeThong;
                                    break;
                                case RoleKey.HoaDon:
                                    roleView_inDepatment = RoleKey.HoaDon_XemDs_PhongBan;
                                    roleView_inBranch = RoleKey.HoaDon_XemDs_HeThong;
                                    break;
                                case RoleKey.TraHang:
                                    roleView_inDepatment = RoleKey.TraHang_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.TraHang_XemDS_HeThong;
                                    break;
                                case RoleKey.GoiDichVu:
                                    roleView_inDepatment = RoleKey.GoiDichVu_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.GoiDichVu_XemDS_HeThong;
                                    break;
                                case RoleKey.NhapHang:
                                    roleView_inDepatment = RoleKey.NhapHang_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.NhapHang_XemDS_HeThong;
                                    break;
                                case RoleKey.TraHangNhap:
                                    roleView_inDepatment = RoleKey.TraHangNhap_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.TraHangNhap_XemDS_HeThong;
                                    break;
                                case RoleKey.KhachHang:
                                    roleView_inDepatment = RoleKey.KhachHang_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.KhachHang_XemDS_HeThong;
                                    break;
                                case RoleKey.NhaCungCap:
                                    roleView_inDepatment = RoleKey.NhaCungCap_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.NhaCungCap_XemDS_HeThong;
                                    break;
                                case RoleKey.LienHe:
                                    roleView_inDepatment = RoleKey.LienHe_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.LienHe_XemDS_HeThong;
                                    break;
                                case RoleKey.CongViec:
                                    roleView_inDepatment = RoleKey.CongViec_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.CongViec_XemDS_HeThong;
                                    break;
                                case RoleKey.PhanHoi: //(todo)
                                    roleView_inDepatment = RoleKey.CongViec_XemDS_PhongBan;
                                    roleView_inBranch = RoleKey.CongViec_XemDS_HeThong;
                                    break;
                            }
                        }

                        // check role view he thong
                        var where = " WHERE htnd.ID_NhanVien like '" + idnvlogin + "' AND MaQuyen like '" + roleView_inBranch + "' and nnd.ID_DonVi='" + idChiNhanh + "'";
                        var lstRole = classNhomNguoiDung.SP_GetListQuyen_Where(where);
                        if (lstRole == null || (lstRole != null && lstRole.Count() == 0))
                        {
                            // check role view in Department
                            where = " WHERE htnd.ID_NhanVien like '" + idnvlogin + "' AND MaQuyen like '" + roleView_inDepatment + "' and nnd.ID_DonVi='" + idChiNhanh + "'";
                            lstRole = classNhomNguoiDung.SP_GetListQuyen_Where(where);

                            IQueryable<NS_QuaTrinhCongTac> listDepartment = db.NS_QuaTrinhCongTac.Where(p => p.ID_NhanVien == idnvlogin).Distinct();

                            if (listDepartment.Any())
                            {

                                if (lstRole == null || (lstRole != null && lstRole.Count() == 0))
                                {
                                    // don't view in Department
                                    foreach (var item in listDepartment)
                                    {
                                        // chek phongban is parent
                                        var parent = db.NS_PhongBan.Where(x => x.ID_PhongBanCha == item.ID_PhongBan);
                                        if (parent.Any())
                                        {
                                            // only get phongban parent
                                            //lstid_nsphongban.Add(item.ID_PhongBan);
                                            lstid_nsphongban = GetlistIDPhongBanChild(lstid_nsphongban, item.ID_PhongBan ?? Guid.Empty);
                                        }
                                    }
                                }
                                else
                                {
                                    // if have view in Department
                                    foreach (var item in listDepartment)
                                    {
                                        // get phong ban current --> get all NhanVien of this department
                                        lstid_nsphongban.Add(item.ID_PhongBan);
                                        // chek phongban is parent
                                        var parent = db.NS_PhongBan.Where(x => x.ID_PhongBanCha == item.ID_PhongBan);
                                        if (parent.Any())
                                        {
                                            lstid_nsphongban = GetlistIDPhongBanChild(lstid_nsphongban, item.ID_PhongBan ?? Guid.Empty);
                                        }
                                    }
                                }

                                // get all nhanvien in parent
                                lstReTurn = db.NS_QuaTrinhCongTac.Where(p => lstid_nsphongban.Contains(p.ID_PhongBan) && (p.ID_DonVi == idChiNhanh || p.ID_DonVi == null))
                                 .Select(x => x.ID_NhanVien).ToList();
                                lstReTurn.Add(idnvlogin ?? Guid.Empty);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ChamSocKhachHangAPI_GetListNhanVienLienQuanByIDLoGin_inDepartment: " + ex);
                }
                return lstReTurn;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostLoaiCongViec([FromBody] JObject data)
        {
            NS_CongViec_PhanLoai objLoaiCV = data["objLoaiCongViec"].ToObject<NS_CongViec_PhanLoai>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                NS_CongViec_PhanLoai cvpl = new NS_CongViec_PhanLoai();
                cvpl.ID = Guid.NewGuid();
                cvpl.LoaiCongViec = objLoaiCV.LoaiCongViec;
                cvpl.NguoiTao = objLoaiCV.NguoiTao;
                cvpl.NgayTao = DateTime.Now;
                cvpl.TrangThai = 1;//=1 là đang sử dụng

                string strIns = classTuVan.Add_LoaiCongViec(cvpl);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = cvpl.ID }, cvpl);
                }
            }
        }
        #endregion

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostCongViec([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                ChamSocKhachHang objcv = data["objCongViec"].ToObject<ChamSocKhachHang>();
                Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                Guid? ID_TrangThai = data["ID_TrangThai"].ToObject<Guid?>();
                List<NS_NhanVien> objnvcs = new List<NS_NhanVien>();
                if (data["objNVCS"] != null)
                {
                    objnvcs = data["objNVCS"].ToObject<List<NS_NhanVien>>();
                }
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);

                ChamSocKhachHang congv = new ChamSocKhachHang();
                congv.ID = Guid.NewGuid();
                congv.ID_LoaiTuVan = objcv.ID_LoaiTuVan;
                congv.ID_KhachHang = objcv.ID_KhachHang;
                congv.Ma_TieuDe = objcv.Ma_TieuDe;
                congv.ID_LienHe = objcv.ID_LienHe;
                congv.ID_DonVi = objcv.ID_DonVi;
                congv.ID_NhanVien = objcv.ID_NhanVien;
                congv.MucDoUuTien = objcv.MucDoUuTien;
                congv.MucDoPhanHoi = 0;
                congv.NhacNho = 0;
                congv.PhanLoai = 4; // 4: Lọai công việc
                congv.NgayGio = objcv.NgayGio;
                congv.NgayGioKetThuc = objcv.NgayGioKetThuc;
                congv.NgayHoanThanh = objcv.NgayHoanThanh;
                congv.GhiChu = objcv.GhiChu;
                congv.KetQua = objcv.KetQua;
                congv.NoiDung = objcv.NoiDung;
                congv.ID_NhanVienQuanLy = nguoidung.ID_NhanVien;
                congv.NguoiTao = objcv.NguoiTao;
                congv.NgayTao = DateTime.Now;
                congv.TrangThai = objcv.TrangThai;// 1: Đang xử lý, 2: Hoàn thành, 3: Hủy
                congv.CaNgay = objcv.CaNgay;

                string strIns = classTuVan.AddNS_CongViecLT(congv);
                var tencongviec = "";
                var tennhanvien = "";
                foreach (var item in objnvcs)
                {
                    ChamSocKhachHang_NhanVien csnv = new ChamSocKhachHang_NhanVien();
                    csnv.ID = Guid.NewGuid();
                    csnv.ID_NhanVien = item.ID;
                    csnv.ID_ChamSocKhachHang = congv.ID;

                    db.ChamSocKhachHang_NhanVien.Add(csnv);
                    db.SaveChanges();
                }

                tencongviec = objcv.Ma_TieuDe;
                tennhanvien = objcv.ID_NhanVien != null ? db.NS_NhanVien.Where(p => p.ID == objcv.ID_NhanVien).FirstOrDefault().TenNhanVien : "";

                var khachhang = db.DM_DoiTuong.FirstOrDefault(o => o.ID == congv.ID_KhachHang);
                if (khachhang != null && ID_TrangThai != null)
                {
                    khachhang.ID_TrangThai = ID_TrangThai;
                    db.Entry(khachhang).State = EntityState.Modified;
                    db.SaveChanges();
                }
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                hT_NhatKySuDung.ChucNang = "Công việc";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Tạo mới công việc: " + tencongviec + ", ngày tạo: " + congv.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + (tennhanvien != "" ? " cho nhân viên: " + tennhanvien : "");
                hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới công việc: " + tencongviec + ", ngày tạo: " + congv.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + (tennhanvien != "" ? " cho nhân viên: " + tennhanvien : "");
                hT_NhatKySuDung.LoaiNhatKy = 1;
                hT_NhatKySuDung.ID_DonVi = objcv.ID_DonVi;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    ChamSocKhachHangDTO objReturn = new ChamSocKhachHangDTO
                    {
                        ID = congv.ID,
                        ID_LoaiTuVan = congv.ID_LoaiTuVan,
                        ID_KhachHang = congv.ID_KhachHang,
                        Ma_TieuDe = congv.Ma_TieuDe,
                        ID_LienHe = congv.ID_LienHe,
                        ID_DonVi = congv.ID_DonVi,
                        ID_NhanVien = congv.ID_NhanVien,
                        MucDoUuTien = congv.MucDoUuTien,
                        MucDoPhanHoi = congv.MucDoPhanHoi,
                        NhacNho = congv.NhacNho,
                        PhanLoai = congv.PhanLoai,
                        NgayGio = congv.NgayGio,
                        NgayGioKetThuc = congv.NgayGioKetThuc,
                        NgayHoanThanh = congv.NgayHoanThanh,
                        GhiChu = congv.GhiChu,
                        KetQua = congv.KetQua,
                        NoiDung = congv.NoiDung,
                        ID_NhanVienQuanLy = congv.ID_NhanVienQuanLy,
                        NguoiTao = congv.NguoiTao,
                        NgayTao = congv.NgayTao,
                        TrangThai = congv.TrangThai,
                        CaNgay = congv.CaNgay
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult Post_ChamSocKhachHang([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_TuVan classTuVan = new class_TuVan(db);
                    ChamSocKhachHang objWork = data["objCongViec"].ToObject<ChamSocKhachHang>();
                    List<NS_NhanVien> lstStaffShare = new List<NS_NhanVien>();
                    if (data["LstStaffShare"] != null)
                    {
                        lstStaffShare = data["LstStaffShare"].ToObject<List<NS_NhanVien>>();
                    }
                    ChamSocKhachHang congv = new ChamSocKhachHang();
                    congv.ID = Guid.NewGuid();
                    congv.ID_LoaiTuVan = objWork.ID_LoaiTuVan;
                    congv.ID_KhachHang = objWork.ID_KhachHang;
                    congv.Ma_TieuDe = objWork.Ma_TieuDe;
                    congv.ID_LienHe = objWork.ID_LienHe;
                    congv.ID_DonVi = objWork.ID_DonVi;
                    congv.ID_NhanVien = objWork.ID_NhanVien;
                    congv.MucDoUuTien = objWork.MucDoUuTien;
                    congv.MucDoPhanHoi = 0;
                    congv.NhacNho = objWork.NhacNho;
                    congv.KieuNhacNho = objWork.KieuNhacNho;
                    congv.PhanLoai = objWork.PhanLoai; // 4: Lọai công việc
                    congv.NgayGio = objWork.NgayGio;
                    congv.NgayGioKetThuc = objWork.NgayGioKetThuc;
                    congv.NgayHoanThanh = objWork.NgayHoanThanh;
                    congv.CaNgay = objWork.CaNgay;
                    congv.GhiChu = objWork.GhiChu;
                    congv.KetQua = objWork.KetQua;
                    congv.NoiDung = objWork.NoiDung;
                    congv.ID_NhanVienQuanLy = objWork.ID_NhanVienQuanLy;
                    congv.NguoiTao = objWork.NguoiTao;
                    congv.NgayTao = DateTime.Now;
                    congv.TrangThai = objWork.TrangThai;// 1: Đang xử lý, 2: Hoàn thành, 3: Hủy

                    congv.ID_Parent = objWork.ID_Parent;
                    congv.KieuLap = objWork.KieuLap;
                    congv.SoLanLap = objWork.SoLanLap;
                    congv.GiaTriLap = objWork.GiaTriLap;
                    congv.TuanLap = objWork.TuanLap;
                    congv.TrangThaiKetThuc = objWork.TrangThaiKetThuc;
                    congv.GiaTriKetThuc = objWork.GiaTriKetThuc;
                    congv.NgayCu = objWork.NgayCu;

                    string strIns = classTuVan.AddNS_CongViecLT(congv);
                    foreach (var item in lstStaffShare)
                    {
                        ChamSocKhachHang_NhanVien csnv = new ChamSocKhachHang_NhanVien();
                        csnv.ID = Guid.NewGuid();
                        csnv.ID_NhanVien = item.ID;
                        csnv.ID_ChamSocKhachHang = congv.ID;

                        db.ChamSocKhachHang_NhanVien.Add(csnv);
                        db.SaveChanges();
                    }
                    return Json(new { res = true, data = new { congv.ID } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        [HttpPost]
        public IHttpActionResult Put_ChamSocKhachHang([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_TuVan classTuVan = new class_TuVan(db);
                    ChamSocKhachHang ChamSocKhachHang = data["objCongViec"].ToObject<ChamSocKhachHang>();
                    List<NS_NhanVien> lstStaffShare = new List<NS_NhanVien>();
                    if (data["LstStaffShare"] != null)
                    {
                        lstStaffShare = data["LstStaffShare"].ToObject<List<NS_NhanVien>>();
                    }

                    if (!ModelState.IsValid)
                    {
                        return Json(new { res = false, mes = "ModelState.IsValid" });
                    }

                    string strUpd = classTuVan.UpdateCongViec(ChamSocKhachHang);

                    IQueryable<ChamSocKhachHang_NhanVien> lstCSNV = db.ChamSocKhachHang_NhanVien.Where(p => p.ID_ChamSocKhachHang == ChamSocKhachHang.ID);
                    db.ChamSocKhachHang_NhanVien.RemoveRange(lstCSNV);
                    foreach (var item in lstStaffShare)
                    {
                        ChamSocKhachHang_NhanVien csnv = new ChamSocKhachHang_NhanVien();
                        csnv.ID = Guid.NewGuid();
                        csnv.ID_NhanVien = item.ID;
                        csnv.ID_ChamSocKhachHang = ChamSocKhachHang.ID;

                        db.ChamSocKhachHang_NhanVien.Add(csnv);
                        db.SaveChanges();
                    }
                    return Json(new { res = true, data = ChamSocKhachHang });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        [HttpPost]
        public IHttpActionResult Event_UpdateStartEnd(Event_ParamUpdate obj)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var sql = "Update ChamSocKhachHangs set " + obj.SqlSet +
                        " WHERE ID='" + obj.ID + "'";
                    db.Database.ExecuteSqlCommand(sql);
                    return Json(new { res = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        // not insert Ht_NhatKySuDung
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult PostNS_CongViec([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                NS_CongViec objcv = data["objCongViec"].ToObject<NS_CongViec>();

                NS_CongViec congv = new NS_CongViec();
                congv.ID = Guid.NewGuid();
                congv.ID_LoaiCongViec = objcv.ID_LoaiCongViec;
                congv.ID_KhachHang = objcv.ID_KhachHang;
                congv.ID_LienHe = objcv.ID_LienHe;
                congv.ID_DonVi = objcv.ID_DonVi;
                congv.ID_NhanVienChiaSe = objcv.ID_NhanVienChiaSe;
                congv.ThoiGianTu = objcv.ThoiGianTu;
                congv.ThoiGianDen = objcv.ThoiGianDen;
                congv.NhacTruoc = objcv.NhacTruoc;
                congv.NoiDung = objcv.NoiDung;
                congv.ID_NhanVienQuanLy = objcv.ID_NhanVienQuanLy;
                congv.ThoiGianLienHeLai = objcv.ThoiGianLienHeLai;
                congv.NhacTruocLienHeLai = objcv.NhacTruocLienHeLai;
                congv.KetQuaCongViec = objcv.KetQuaCongViec;
                congv.LyDoHenLai = objcv.LyDoHenLai;
                congv.NguoiTao = objcv.NguoiTao;
                congv.NgayTao = DateTime.Now;
                congv.TrangThai = (int)commonEnumHellper.TypeCongViec.dangxuly;// 0 đang sử dụng, 1 xóa

                string strIns = classTuVan.AddNS_CongViec(congv);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = congv.ID }, congv);
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [ResponseType(typeof(ChamSocKhachHang))]
        public IHttpActionResult PostLichHen([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                class_LichHen classLichHen = new class_LichHen(db);

                ChamSocKhachHang objLichHen = data["objLichHen"].ToObject<ChamSocKhachHang>();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                #region LICHHEN
                ChamSocKhachHang itemLichHen = new ChamSocKhachHang();
                itemLichHen.ID = Guid.NewGuid();
                itemLichHen.Ma_TieuDe = objLichHen.Ma_TieuDe;
                itemLichHen.ID_DonVi = objLichHen.ID_DonVi;
                itemLichHen.ID_KhachHang = objLichHen.ID_KhachHang;
                itemLichHen.ID_NhanVien = objLichHen.ID_NhanVien;
                itemLichHen.ID_LoaiTuVan = objLichHen.ID_LoaiTuVan;
                itemLichHen.PhanLoai = 3;// 3 = lich hen
                itemLichHen.NhacNho = objLichHen.NhacNho; ;
                itemLichHen.MucDoPhanHoi = 1;
                itemLichHen.NguoiTao = nguoidung.TaiKhoan;
                itemLichHen.ID_NhanVienQuanLy = nguoidung.ID_NhanVien;
                itemLichHen.NoiDung = objLichHen.NoiDung;
                itemLichHen.TraLoi = objLichHen.TraLoi;
                itemLichHen.NgayGio = objLichHen.NgayGio;
                itemLichHen.NgayGioKetThuc = objLichHen.NgayGioKetThuc;
                itemLichHen.NgayTao = DateTime.Now;
                itemLichHen.NgaySua = null;
                itemLichHen.TrangThai = objLichHen.TrangThai;
                #endregion

                string strIns = classLichHen.Add_LichHen(itemLichHen);
                var tenkhach = db.DM_DoiTuong.Where(p => p.ID == objLichHen.ID_KhachHang).FirstOrDefault().TenDoiTuong;
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = nguoidung.ID_NhanVien;
                hT_NhatKySuDung.ChucNang = "Lịch hẹn";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Tạo mới lịch hẹn mã: " + objLichHen.Ma_TieuDe + ", ngày tạo: " + itemLichHen.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + " với khách hàng: " + tenkhach;
                hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới lịch hẹn mã: " + objLichHen.Ma_TieuDe + ", ngày tạo: " + itemLichHen.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + " với khách hàng: " + tenkhach;
                hT_NhatKySuDung.LoaiNhatKy = 1;
                hT_NhatKySuDung.ID_DonVi = nguoidung.ID_DonVi.Value;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = objLichHen.ID }, objLichHen);
                }
            }
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [ResponseType(typeof(ChamSocKhachHang))]
        public IHttpActionResult PostPhanHoi([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_PhanHoi classPhanHoi = new class_PhanHoi(db);
                ChamSocKhachHang objPhanHoi = data["objPhanHoi"].ToObject<ChamSocKhachHang>();

                #region PHANHOI
                string sMa_TieuDe = string.Empty;
                if (objPhanHoi.Ma_TieuDe == null || objPhanHoi.Ma_TieuDe == "")
                {
                    sMa_TieuDe = classPhanHoi.GetMaPhieuChamSoc();
                }
                else
                {
                    sMa_TieuDe = objPhanHoi.Ma_TieuDe;
                }
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                ChamSocKhachHang itemPhanHoi = new ChamSocKhachHang();
                itemPhanHoi.ID = Guid.NewGuid();
                itemPhanHoi.Ma_TieuDe = sMa_TieuDe;
                itemPhanHoi.ID_KhachHang = objPhanHoi.ID_KhachHang;
                itemPhanHoi.ID_LoaiTuVan = objPhanHoi.ID_LoaiTuVan;
                itemPhanHoi.ID_DonVi = nguoidung.ID_DonVi.Value;
                itemPhanHoi.ID_NhanVien = objPhanHoi.ID_NhanVien;
                itemPhanHoi.PhanLoai = 2;// 2 = phan hoi
                itemPhanHoi.NhacNho = 0;
                itemPhanHoi.MucDoPhanHoi = objPhanHoi.MucDoPhanHoi;
                itemPhanHoi.NguoiTao = nguoidung.TaiKhoan;
                itemPhanHoi.ID_NhanVienQuanLy = nguoidung.ID_NhanVien;
                itemPhanHoi.NoiDung = objPhanHoi.NoiDung;//noi dung tra loi
                itemPhanHoi.TraLoi = objPhanHoi.TraLoi;// noi dung phan hoi
                itemPhanHoi.NgayGio = objPhanHoi.NgayGio;
                itemPhanHoi.NgayGioKetThuc = DateTime.Now;
                itemPhanHoi.ThoiGianHenLai = objPhanHoi.ThoiGianHenLai;
                itemPhanHoi.NgayTao = DateTime.Now;
                itemPhanHoi.NgaySua = null;
                itemPhanHoi.TrangThai = objPhanHoi.TrangThai;
                #endregion

                string strIns = classPhanHoi.Add_PhanHoi(itemPhanHoi);
                var tenkhach = db.DM_DoiTuong.Where(p => p.ID == objPhanHoi.ID_KhachHang).FirstOrDefault().TenDoiTuong;
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = nguoidung.ID_NhanVien;
                hT_NhatKySuDung.ChucNang = "Phản hồi";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Tạo mới phiếu phản hồi: " + sMa_TieuDe + ", ngày tạo: " + itemPhanHoi.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + " khách hàng: " + tenkhach;
                hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu phản hồi: " + sMa_TieuDe + ", ngày tạo: " + itemPhanHoi.NgayTao.ToString("dd/MM/yyyy HH:mm:ss") + " khách hàng: " + tenkhach;
                hT_NhatKySuDung.LoaiNhatKy = 1;
                hT_NhatKySuDung.ID_DonVi = nguoidung.ID_DonVi.Value;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    ChamSocKhachHangDTO objReturnDTO = new ChamSocKhachHangDTO();
                    objReturnDTO.ID = itemPhanHoi.ID;
                    objReturnDTO.Ma_TieuDe = sMa_TieuDe;
                    objReturnDTO.ID_KhachHang = objPhanHoi.ID_KhachHang;
                    objReturnDTO.ID_LoaiTuVan = objPhanHoi.ID_LoaiTuVan;
                    objReturnDTO.ID_DonVi = nguoidung.ID_DonVi.Value;
                    objReturnDTO.ID_NhanVien = objPhanHoi.ID_NhanVien;
                    objReturnDTO.PhanLoai = 2;// 2 = phan hoi
                    objReturnDTO.NhacNho = 0;
                    objReturnDTO.MucDoPhanHoi = objPhanHoi.MucDoPhanHoi;
                    objReturnDTO.NguoiTao = nguoidung.TaiKhoan;
                    objReturnDTO.ID_NhanVienQuanLy = nguoidung.ID_NhanVien;
                    objReturnDTO.NoiDung = objPhanHoi.NoiDung;//noi dung tra loi
                    objReturnDTO.TraLoi = objPhanHoi.TraLoi;// noi dung phan hoi
                    objReturnDTO.NgayGio = objPhanHoi.NgayGio;
                    objReturnDTO.NgayGioKetThuc = itemPhanHoi.NgayGioKetThuc;
                    objReturnDTO.ThoiGianHenLai = objPhanHoi.ThoiGianHenLai;
                    objReturnDTO.NgayTao = itemPhanHoi.NgayTao;
                    objReturnDTO.NgaySua = null;
                    objReturnDTO.TrangThai = objPhanHoi.TrangThai;

                    return CreatedAtRoute("DefaultApi", new { id = objReturnDTO.ID }, objReturnDTO);
                }
            }
        }


        #region update tu van
        // PUT: api/ChamSocKhachHangAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutPhieuTuVan(Guid id, ChamSocKhachHang ChamSocKhachHang)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                string strUpd = classTuVan.Update_PhieuTuVan(ChamSocKhachHang);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/ChamSocKhachHangAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutPhieuTuVan([FromBody] JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            ChamSocKhachHang ChamSocKhachHang = data["objTuVan"].ToObject<ChamSocKhachHang>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                string strUpd = classTuVan.Update_PhieuTuVan(ChamSocKhachHang);

                ChamSocKhachHang cskh = db.ChamSocKhachHang.Where(p => p.ID == id).FirstOrDefault();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = nguoidung.ID_NhanVien;
                hT_NhatKySuDung.ChucNang = "Tư vấn";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Cập nhật phiếu tư vấn có mã: " + cskh.Ma_TieuDe;
                hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật phiếu tư vấn có mã: " + cskh.Ma_TieuDe;
                hT_NhatKySuDung.LoaiNhatKy = 2;
                hT_NhatKySuDung.ID_DonVi = nguoidung.ID_DonVi.Value;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [ResponseType(typeof(string))]
        [HttpPost, HttpPut]
        public IHttpActionResult PutLoaiCongViec([FromBody] JObject data)
        {
            NS_CongViec_PhanLoai objLoaiCV = data["objLoaiCongViec"].ToObject<NS_CongViec_PhanLoai>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                string strUpd = classTuVan.Update_LoaiCongViec(objLoaiCV);

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objLoaiCV.ID }, objLoaiCV);
            }
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult PutCongViec([FromBody] JObject data)
        {
            ChamSocKhachHang ChamSocKhachHang = data["objCongViec"].ToObject<ChamSocKhachHang>();
            Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
            Guid? ID_TrangThai = data["ID_TrangThai"].ToObject<Guid?>();

            List<NS_NhanVien> objnvcs = new List<NS_NhanVien>();
            if (data["objNVCS"] != null)
            {
                objnvcs = data["objNVCS"].ToObject<List<NS_NhanVien>>();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                ChamSocKhachHang objUpd = db.ChamSocKhachHang.Find(ChamSocKhachHang.ID);
                var tencongvieccu = db.ChamSocKhachHang.Where(p => p.ID == objUpd.ID).FirstOrDefault().Ma_TieuDe;
                string strUpd = classTuVan.UpdateCongViec(ChamSocKhachHang);

                IQueryable<ChamSocKhachHang_NhanVien> lstCSNV = db.ChamSocKhachHang_NhanVien.Where(p => p.ID_ChamSocKhachHang == ChamSocKhachHang.ID);
                db.ChamSocKhachHang_NhanVien.RemoveRange(lstCSNV);
                foreach (var item in objnvcs)
                {
                    ChamSocKhachHang_NhanVien csnv = new ChamSocKhachHang_NhanVien();
                    csnv.ID = Guid.NewGuid();
                    csnv.ID_NhanVien = item.ID;
                    csnv.ID_ChamSocKhachHang = ChamSocKhachHang.ID;

                    db.ChamSocKhachHang_NhanVien.Add(csnv);
                    db.SaveChanges();
                }

                var khachhang = db.DM_DoiTuong.FirstOrDefault(o => o.ID == ChamSocKhachHang.ID_KhachHang);
                if (khachhang != null && ID_TrangThai != null)
                {
                    khachhang.ID_TrangThai = ID_TrangThai;
                    db.SaveChanges();
                }
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                hT_NhatKySuDung.ChucNang = "Công việc";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Cập nhật công việc " + tencongvieccu;
                hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật công việc " + tencongvieccu;
                hT_NhatKySuDung.LoaiNhatKy = 2;
                hT_NhatKySuDung.ID_DonVi = objUpd.ID_DonVi;
                SaveDiary.add_Diary(hT_NhatKySuDung);

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [ResponseType(typeof(string))]
        [HttpPut, HttpGet]
        public IHttpActionResult PutNS_CongViec([FromBody] JObject data)
        {
            ChamSocKhachHang objUpdate = data["objCongViec"].ToObject<ChamSocKhachHang>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                string strUpd = classTuVan.UpdateCongViec(objUpdate);

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        public bool CheckLoaiCongViecDaTonTai(string loaicongviec)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                return classTuVan.ChecLoaiCongViec(loaicongviec);
            }
        }

        public bool CheckLoaiCongViecDaTonTaiEdit(string loaicongviec, Guid idloaicv)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                return classTuVan.ChecLoaiCongViecEdit(loaicongviec, idloaicv);
            }
        }

        [HttpGet, HttpPost]
        public bool Check_LoaiCongViec_Exist(string loaicongviec, Guid idloaicv, int loaiTuVan)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                return classTuVan.SP_Check_LoaiCongViec_Exist(loaicongviec, idloaicv, loaiTuVan);
            }
        }
        #endregion

        #region update lich hen
        // PUT: api/ChamSocKhachHangAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutLichHen(Guid id, ChamSocKhachHang ChamSocKhachHang)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                string strUpd = classLichHen.Update_LichHen(ChamSocKhachHang);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/ChamSocKhachHangAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutLichHen([FromBody] JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            ChamSocKhachHang ChamSocKhachHang = data["objLichHen"].ToObject<ChamSocKhachHang>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                string strUpd = classLichHen.Update_LichHen(ChamSocKhachHang);

                ChamSocKhachHang cskh = db.ChamSocKhachHang.Where(p => p.ID == id).FirstOrDefault();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = nguoidung.ID_NhanVien;
                hT_NhatKySuDung.ChucNang = "Lịch hẹn";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Cập nhật lịch hẹn có tiêu đề: " + cskh.Ma_TieuDe;
                hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật lịch hẹn có tiêu đề: " + cskh.Ma_TieuDe;
                hT_NhatKySuDung.LoaiNhatKy = 2;
                hT_NhatKySuDung.ID_DonVi = nguoidung.ID_DonVi.Value;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion 

        #region update phan hoi
        // PUT: api/ChamSocKhachHangAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutPhanHoi(Guid id, ChamSocKhachHang ChamSocKhachHang)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_PhanHoi classPhanHoi = new class_PhanHoi(db);
                string strUpd = classPhanHoi.Update_PhanHoi(ChamSocKhachHang);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/ChamSocKhachHangAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutPhanHoi([FromBody] JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            ChamSocKhachHang ChamSocKhachHang = data["objPhanHoi"].ToObject<ChamSocKhachHang>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_PhanHoi classPhanHoi = new class_PhanHoi(db);
                string strUpd = classPhanHoi.Update_PhanHoi(ChamSocKhachHang);

                ChamSocKhachHang cskh = db.ChamSocKhachHang.Where(p => p.ID == id).FirstOrDefault();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_NhanVien = nguoidung.ID_NhanVien;
                hT_NhatKySuDung.ChucNang = "Phản hồi";
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = "Cập nhật phản hồi mã: " + cskh.Ma_TieuDe;
                hT_NhatKySuDung.NoiDungChiTiet = "Cập nhật phản hồi mã: " + cskh.Ma_TieuDe;
                hT_NhatKySuDung.LoaiNhatKy = 2;
                hT_NhatKySuDung.ID_DonVi = nguoidung.ID_DonVi.Value;
                SaveDiary.add_Diary(hT_NhatKySuDung);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion


        public string Check_Exist([FromBody] JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_TuVan classTuVan = new class_TuVan(db);
                    ChamSocKhachHang obj = data.ToObject<ChamSocKhachHang>();
                    string err = string.Empty;
                    int countObj = 0;
                    // update:
                    if (Guid.Empty != obj.ID)
                    {
                        if (obj.Ma_TieuDe != null)
                        {
                            // check ma trung
                            countObj = classTuVan.Gets(manv => manv.Ma_TieuDe == obj.Ma_TieuDe && manv.ID != obj.ID).Count();
                            if (countObj == 0)
                            {
                            }
                            else
                            {
                                //err = "Mã tiêu đề đã tồn tại";
                            }
                        }
                        else
                        {
                        }
                    }
                    // insert
                    else
                    {
                        if (obj.Ma_TieuDe != null)
                        {
                            // check ma trung
                            countObj = classTuVan.Gets(manv => manv.Ma_TieuDe == obj.Ma_TieuDe).Count();
                            if (countObj == 0)
                            {

                            }
                            else
                            {
                                //err = "Mã tiêu đề đã tồn tại";
                            }
                        }
                        else
                        {
                        }
                    }
                    return err;
                }
            }
            catch (Exception ex)
            {
                string err = string.Empty;
                if (ex.Message.EndsWith("'ID_KhachHang'."))
                {
                    return err = "Tên khách hàng không tồn tại!";
                }
                if (ex.Message.EndsWith("'ID_NhanVien'."))
                {
                    return err = "Tên nhân viên không tồn tại!";
                }
                else
                {
                    return err = "Thêm mới thất bại: " + ex.Message;
                }
            }
        }
        [ResponseType(typeof(ChamSocKhachHang))]
        public IHttpActionResult GetPhieuTuVan(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ChamSocKhachHang ChamSocKhachHang = db.ChamSocKhachHang.Where(p => p.ID == id).FirstOrDefault();
                ChamSocKhachHangDTO ct = new ChamSocKhachHangDTO();
                ct.ID = ChamSocKhachHang.ID;
                ct.Ma_TieuDe = ChamSocKhachHang.Ma_TieuDe;
                ct.ID_KhachHang = ChamSocKhachHang.ID_KhachHang;
                ct.ID_NhanVien = ChamSocKhachHang.ID_NhanVien;
                ct.TenKhachHang = db.DM_DoiTuong.Where(p => p.ID == ChamSocKhachHang.ID_KhachHang).FirstOrDefault().TenDoiTuong;
                ct.TenNV = db.NS_NhanVien.Where(p => p.ID == ChamSocKhachHang.ID_NhanVien).FirstOrDefault().TenNhanVien;
                ct.ID_LoaiTuVan = ChamSocKhachHang.ID_LoaiTuVan;
                ct.NgayGio = ChamSocKhachHang.NgayGio;
                ct.ThoiGianHenLai = ChamSocKhachHang.ThoiGianHenLai;
                ct.TraLoi = ChamSocKhachHang.TraLoi;
                ct.NoiDung = ChamSocKhachHang.NoiDung;
                ct.TrangThai = ChamSocKhachHang.TrangThai;

                if (ChamSocKhachHang == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }
        public static NS_NhanVien getTenNhanVienByID(Guid? id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    return db.NS_NhanVien.Find(id);
                }
            }
        }
        // nhan vien
        public List<NS_NhanVienSelect> GetNS_NhanVien(Guid? id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                IQueryable<NS_NhanVien> lstdata = new ClassNS_NhanVien(db).Gets(null);
                List<NS_NhanVienSelect> lst = new List<NS_NhanVienSelect>();
                foreach (var item in lstdata)
                {
                    NS_NhanVienSelect select = new NS_NhanVienSelect();
                    select.ID = item.ID;
                    select.TenNhanVien = item.TenNhanVien;
                    if (select.ID == id)
                    {
                        lst.Add(select);
                    }
                }
                return lst;
            }
        }


        public List<ChamSocKhachHang> GetListPhieuTuVans()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                List<ChamSocKhachHang> lstAllPhieuTVs = classTuVan.Gets(n => n.PhanLoai == 1).OrderByDescending(p => p.NgayGio).ToList();// Phân loại = 1 là tư vấn
                if (lstAllPhieuTVs != null && lstAllPhieuTVs.Count() > 0)
                {
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    List<ChamSocKhachHang> lsrReturns = new List<ChamSocKhachHang>();
                    foreach (ChamSocKhachHang item in lstAllPhieuTVs)
                    {
                        NS_NhanVien dt0 = _ClassNS_NhanVien.Select_NhanVien(item.ID_NhanVien);

                        string sNguoiTao = string.Empty;
                        if (dt0 != null)
                        {
                            sNguoiTao = dt0.TenNhanVien;
                        }
                        ChamSocKhachHang itemData = new ChamSocKhachHang
                        {
                            ID = item.ID,
                            Ma_TieuDe = item.Ma_TieuDe,
                            NgayGio = item.NgayGio,
                            //TrangThai = item.TrangThai,
                            TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : ""))),
                            NoiDung = item.NoiDung,
                            TenNV = sNguoiTao,
                            TenLoaiTV = item.DM_LoaiTuVanLichHen != null ? item.DM_LoaiTuVanLichHen.TenLoaiTuVanLichHen : "",
                            TenKhachHang = item.DM_DoiTuong != null ? item.DM_DoiTuong.TenDoiTuong : "",
                            PhanLoai = item.PhanLoai,
                            TraLoi = item.TraLoi,
                        };
                        lsrReturns.Add(itemData);
                    }
                    return lsrReturns;
                }
                else
                    return null;
            }
        }


        [ResponseType(typeof(ChamSocKhachHang))]
        public IHttpActionResult GetLichHen(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ChamSocKhachHang ChamSocKhachHang = db.ChamSocKhachHang.Where(p => p.ID == id).FirstOrDefault();
                ChamSocKhachHangDTO ct = new ChamSocKhachHangDTO();
                ct.ID = ChamSocKhachHang.ID;
                ct.Ma_TieuDe = ChamSocKhachHang.Ma_TieuDe;
                ct.ID_KhachHang = ChamSocKhachHang.ID_KhachHang;
                ct.ID_NhanVien = ChamSocKhachHang.ID_NhanVien;
                ct.TenKhachHang = db.DM_DoiTuong.Where(p => p.ID == ChamSocKhachHang.ID_KhachHang).FirstOrDefault().TenDoiTuong;
                ct.TenNV = db.NS_NhanVien.Where(p => p.ID == ChamSocKhachHang.ID_NhanVien).FirstOrDefault().TenNhanVien;
                ct.NgayGio = ChamSocKhachHang.NgayGio;
                ct.NgayGioKetThuc = ChamSocKhachHang.NgayGioKetThuc;
                ct.NhacNho = ChamSocKhachHang.NhacNho;
                ct.NoiDung = ChamSocKhachHang.NoiDung;
                ct.TrangThai = ChamSocKhachHang.TrangThai;
                ct.ID_LoaiTuVan = ChamSocKhachHang.ID_LoaiTuVan;

                if (ChamSocKhachHang == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }
        #region Trinhpv
        //Tìm kiếm
        public List<LH_ChamSocKhachHang> getAllLichHenSeach(string giatri, string giatrinv, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.SeachLichHen(giatri, giatrinv, IDchinhanh);
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
        public List<LH_ChamSocKhachHang> getseachLichHens(string giatri, string giatrinv, int sohang, int page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.SeachLichHen(giatri, giatrinv, IDchinhanh);
                List<LH_ChamSocKhachHang> lstPage = new List<LH_ChamSocKhachHang>();
                getPageLichHen(lst, lstPage, sohang, page);
                if (lstPage != null)
                {
                    return lstPage;
                }
                else
                {
                    return null;
                }
            }
        }
        public int getRowSeach(string giatri, string giatrinv, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLH = classLichHen.SeachLichHen(giatri, giatrinv, IDchinhanh);
                return getRowsCountList(lstLH);
            }
        }
        public List<ListLHPages> getPageSeach(string giatri, string giatrinv, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLichHen = classLichHen.SeachLichHen(giatri, giatrinv, IDchinhanh);
                List<ListLHPages> lstpageLH = new List<ListLHPages>();
                return getAllPage(lstLichHen, lstpageLH, sohang);
            }
        }
        public List<listPhanLoai> getPhanLoaiSeach(string giatri, string giatrinv, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<listPhanLoai> PL = classLichHen.getPhanLoaiSeach(giatri, giatrinv, IDchinhanh);
                if (PL != null)
                {
                    return PL;
                }
                else
                {
                    return null;
                }
            }
        }

        // load phân loại
        public List<listPhanLoai> getPhanLoai(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<listPhanLoai> PL = classLichHen.getListPhanLoai(dayStart, dayEnd, IDchinhanh);
                if (PL != null)
                {
                    return PL;
                }
                else
                {
                    return null;
                }
            }
        }
        // load danh mục lịch hẹn

        public List<LH_ChamSocKhachHang> getAllDMLicHen(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.getAllLichHen(dayStart, dayEnd, IDchinhanh);
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
        public List<LH_ChamSocKhachHang> getDMLichHen(DateTime dayStart, DateTime dayEnd, int sohang, int page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.getAllLichHen(dayStart, dayEnd, IDchinhanh);
                List<LH_ChamSocKhachHang> lstPage = new List<LH_ChamSocKhachHang>();
                getPageLichHen(lst, lstPage, sohang, page);
                if (lstPage != null)
                {
                    return lstPage;
                }
                else
                {
                    return null;
                }
            }
        }

        public int getRowLichHen(DateTime dayStart, DateTime dayEnd, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLH = classLichHen.getAllLichHen(dayStart, dayEnd, IDchinhanh);
                return getRowsCountList(lstLH);
            }
        }
        public List<ListLHPages> getPageLichHen(DateTime dayStart, DateTime dayEnd, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLichHen = classLichHen.getAllLichHen(dayStart, dayEnd, IDchinhanh);
                List<ListLHPages> lstpageLH = new List<ListLHPages>();
                return getAllPage(lstLichHen, lstpageLH, sohang);
            }
        }
        //load filter phanloai
        public List<LH_ChamSocKhachHang> getFilterPhanLoai(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, int sohang, int page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.getFilterPhanLoai(dayStart, dayEnd, TenLoaiTV, IDchinhanh);
                List<LH_ChamSocKhachHang> lstPage = new List<LH_ChamSocKhachHang>();
                getPageLichHen(lst, lstPage, sohang, page);
                if (lstPage != null)
                {
                    return lstPage;
                }
                else
                {
                    return null;
                }
            }
        }
        public int getRowFilterPhanLoai(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLH = classLichHen.getFilterPhanLoai(dayStart, dayEnd, TenLoaiTV, IDchinhanh);
                return getRowsCountList(lstLH);
            }
        }
        public List<ListLHPages> getPagePhanLoai(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLichHen = classLichHen.getFilterPhanLoai(dayStart, dayEnd, TenLoaiTV, IDchinhanh);
                List<ListLHPages> lstpageLH = new List<ListLHPages>();
                return getAllPage(lstLichHen, lstpageLH, sohang);
            }
        }
        //load filter Trạng thái
        public List<LH_ChamSocKhachHang> getFiltertrangthai(DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, int sohang, int page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.getFilterTrangThai(dayStart, dayEnd, trangthai1, trangthai2, trangthai3, IDchinhanh);
                List<LH_ChamSocKhachHang> lstPage = new List<LH_ChamSocKhachHang>();
                getPageLichHen(lst, lstPage, sohang, page);
                if (lstPage != null)
                {
                    return lstPage;
                }
                else
                {
                    return null;
                }
            }
        }
        public int getRowFilterTrangThai(DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLH = classLichHen.getFilterTrangThai(dayStart, dayEnd, trangthai1, trangthai2, trangthai3, IDchinhanh);
                return getRowsCountList(lstLH);
            }
        }
        public List<ListLHPages> getPageTrangthai(DateTime dayStart, DateTime dayEnd, string trangthai1, string trangthai2, string trangthai3, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLichHen = classLichHen.getFilterTrangThai(dayStart, dayEnd, trangthai1, trangthai2, trangthai3, IDchinhanh);
                List<ListLHPages> lstpageLH = new List<ListLHPages>();
                return getAllPage(lstLichHen, lstpageLH, sohang);
            }
        }
        //load filter phân loại, trạng thái
        public List<LH_ChamSocKhachHang> getFilterPLTT(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, string trangthai1, string trangthai2, string trangthai3, int sohang, int page, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.getFilterPLTT(dayStart, dayEnd, TenLoaiTV, trangthai1, trangthai2, trangthai3, IDchinhanh);
                List<LH_ChamSocKhachHang> lstPage = new List<LH_ChamSocKhachHang>();
                getPageLichHen(lst, lstPage, sohang, page);
                if (lstPage != null)
                {
                    return lstPage;
                }
                else
                {
                    return null;
                }
            }
        }
        public int getRowFilterPLTT(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, string trangthai1, string trangthai2, string trangthai3, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLH = classLichHen.getFilterPLTT(dayStart, dayEnd, TenLoaiTV, trangthai1, trangthai2, trangthai3, IDchinhanh);
                return getRowsCountList(lstLH);
            }
        }
        public List<ListLHPages> getPagePLTT(DateTime dayStart, DateTime dayEnd, string TenLoaiTV, string trangthai1, string trangthai2, string trangthai3, float sohang, Guid IDchinhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lstLichHen = classLichHen.getFilterPLTT(dayStart, dayEnd, TenLoaiTV, trangthai1, trangthai2, trangthai3, IDchinhanh);
                List<ListLHPages> lstpageLH = new List<ListLHPages>();
                return getAllPage(lstLichHen, lstpageLH, sohang);
            }
        }
        //Trả về dữ liệu theo trang selection
        public List<LH_ChamSocKhachHang> getPageLichHen(List<LH_ChamSocKhachHang> lst, List<LH_ChamSocKhachHang> lstPage, int sohang, int Page)
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
        public int getRowsCountList(List<LH_ChamSocKhachHang> lstLHs)
        {
            if (lstLHs != null)
            {
                return lstLHs.Count;
            }
            else
            {
                return 0;
            }
        }
        //Lấy số trang hiển thị danh sách
        public List<ListLHPages> getAllPage<T>(List<T> lstLHs, float sohang)
        {
            List<ListLHPages> listPage = new List<ListLHPages>();
            if (lstLHs != null)
            {
                int dem = 1;
                float SoTrang = lstLHs.Count / sohang;
                for (int i = 0; i < SoTrang; i++)
                {
                    ListLHPages LH_page = new ListLHPages();
                    LH_page.SoTrang = dem;
                    listPage.Add(LH_page);
                    dem = dem + 1;
                }
                return listPage;
            }
            else
            {
                return null;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<ChamSocKhachHangDTO> GetCongViecByID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var tbl = from congviec in db.ChamSocKhachHang
                              join loaicv in db.DM_LoaiTuVanLichHen on congviec.ID_LoaiTuVan equals loaicv.ID
                              join doituong in db.DM_DoiTuong on congviec.ID_KhachHang equals doituong.ID
                              join lienhe in db.DM_LienHe on congviec.ID_LienHe equals lienhe.ID into DMLIENHE
                              from dmlienhe in DMLIENHE.DefaultIfEmpty()
                              join nhanvien in db.NS_NhanVien on congviec.ID_NhanVien equals nhanvien.ID into NHANVIEN
                              from nsnhanvien in NHANVIEN.DefaultIfEmpty()
                              join nvph in db.ChamSocKhachHang_NhanVien on congviec.ID equals nvph.ID_ChamSocKhachHang into NHANVIENPHOIHOP
                              from nv_ph in NHANVIENPHOIHOP.DefaultIfEmpty()
                              where congviec.ID == id && congviec.PhanLoai == 4
                              select new ChamSocKhachHangDTO()
                              {
                                  ID = congviec.ID,
                                  Ma_TieuDe = congviec.Ma_TieuDe,
                                  ID_LoaiTuVan = congviec.ID_LoaiTuVan,
                                  MucDoUuTien = congviec.MucDoUuTien,
                                  ID_KhachHang = congviec.ID_KhachHang.Value,
                                  ID_LienHe = congviec.ID_LienHe == null ? null : congviec.ID_LienHe,
                                  ID_NhanVien = congviec.ID_NhanVien.Value,
                                  LoaiCongViec = loaicv.TenLoaiTuVanLichHen,
                                  TenKhachHang = doituong.TenDoiTuong,
                                  TenLienHe = dmlienhe.TenLienHe,
                                  TenNhanVienPhuTrach = nsnhanvien.TenNhanVien,
                                  NgayGio = congviec.NgayGio,
                                  NgayGioKetThuc = congviec.NgayGioKetThuc,
                                  NhacNho = congviec.NhacNho,
                                  NoiDung = congviec.NoiDung,
                                  TrangThai = congviec.TrangThai,
                                  KetQua = congviec.KetQua,
                                  LoaiDoiTuongCV = doituong.LoaiDoiTuong,
                                  TrangThaiKhach = doituong.ID_TrangThai,
                                  ID_NhanVienPH = nv_ph.ID_NhanVien,
                                  NgayHoanThanh = congviec.NgayHoanThanh,
                                  FileDinhKem = congviec.FileDinhKem,
                                  CaNgay = congviec.CaNgay
                              };
                    return tbl.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<NS_CongViec_PhanLoaiDTO> GetLoaiCongViecByID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var tbl = from nscv in db.NS_CongViec_PhanLoai
                              where nscv.ID == id
                              select new NS_CongViec_PhanLoaiDTO()
                              {
                                  ID = nscv.ID,
                                  LoaiCongViec = nscv.LoaiCongViec
                              };
                    return tbl.ToList();
                }
                else
                {
                    return null;
                }
            }
        }


        public System.Web.Http.Results.JsonResult<JsonResultExample<LH_ChamSocKhachHang>> getList_ChamSocKhachHang(int loaiLH, Guid? ID_LoaiTV, string TieuDe, string TrangThai1, string TrangThai2, string TrangThai3, DateTime timeStart, DateTime timeEnd, int pageNumber, int pageSize, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<LH_ChamSocKhachHang> lst = classLichHen.getList_ChamSocKhachHang(loaiLH, ID_LoaiTV, TieuDe, TrangThai1, TrangThai2, TrangThai3, timeStart, timeEnd, ID_ChiNhanh);
                int Rows = lst.Count;
                List<ListLHPages> lstPage = getAllPage<LH_ChamSocKhachHang>(lst, pageSize);
                List<LH_ChamSocKhachHang> lstLH = lst.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                JsonResultExample<LH_ChamSocKhachHang> jsonResult = new JsonResultExample<LH_ChamSocKhachHang>
                {
                    Rowcount = Rows,
                    LstPageNumber = lstPage,
                    LstData = lstLH
                };
                return Json(jsonResult);
            }
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string ExportExcel_CongViec(ParamCalendar model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                Class_officeDocument classOffice = new Class_officeDocument(db);
                //INS 10.07.2024
                ClassNPOIExcel classNPOI = new ClassNPOIExcel();
                List<SP_Calendar> lstReturn = classLichHen.GetListCalendar(model);
                List<ChamSocKhachHangXuatFileDTO> lst = new List<ChamSocKhachHangXuatFileDTO>();
                foreach (var item in lstReturn)
                {
                    ChamSocKhachHangXuatFileDTO DM = new ChamSocKhachHangXuatFileDTO();
                    DM.PhanLoai = item.PhanLoai == 3 ? "Lịch hẹn" : "Công việc";
                    DM.LoaiCongViec = item.TenLoaiTuVanLichHen;
                    DM.MucDoUuTien = item.MucDoUuTien == 3 ? "Thấp" : (item.MucDoUuTien == 2 ? "Trung bình" : "Cao");
                    DM.NgayGio = item.NgayGio;
                    DM.NgayGioKetThuc = item.NgayGioKetThuc;
                    DM.Ma_TieuDe = item.Ma_TieuDe;
                    DM.MaDoiTuong = item.MaDoiTuong;
                    DM.TenKhachHang = item.TenDoiTuong;
                    DM.SoDienThoai = item.DienThoai;
                    DM.NguonKhach = item.TenNguonKhach;
                    DM.TenNhanVienPhuTrach = item.TenNhanVien;
                    DM.NhacNho = item.NhacNho;
                    DM.TrangThai = item.TrangThai == "1" ? "Đang xử lý" : (item.TrangThai == "2" ? "Hoàn thành" : "Hủy");
                    DM.KetQua = item.KetQua;
                    DM.NguoiTao = item.NguoiTao;
                    DM.NgayTao = item.NgayTao;
                    DM.GhiChu = item.GhiChu;
                    lst.Add(DM);
                }
                DataTable excel = classOffice.ToDataTable<ChamSocKhachHangXuatFileDTO>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_CongViec.xlsx");
                var time = model.FromDate + " - " + model.ToDate;
                List<ClassExcel_CellData> lstCell = classNPOI.GetValue_forCell(model.TenChiNhanhs, time);
                classNPOI.ExportDataToExcel(fileTeamplate, excel, 4, model.ColumnsHide, lstCell,-1);               
                return string.Empty;
            }
        }


        [AcceptVerbs("POST", "GET")]
        public string UpdateCongViecXoaFile(Guid idcv)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Error";
                }
                else
                {
                    ChamSocKhachHang cskh = db.ChamSocKhachHang.Find(idcv);
                    cskh.FileDinhKem = null;
                    db.Entry(cskh).State = EntityState.Modified;
                    db.SaveChanges();
                    return "";
                }
            }
        }

        //tư vấn
        public System.Web.Http.Results.JsonResult<JsonResultExampleCongViec<ChamSocKhachHangDTO>> GetAllTuVanWhere(int currentPage, int pageSize, string arrLoaiTuVan, DateTime dayStart, DateTime dayEnd, string txtSearch, int trangthai, string arrMangIDNhanVien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                List<ChamSocKhachHangDTO> lst = classTuVan.GetListTuVan(txtSearch, iddonvi);
                List<Guid> lstLoaiCV = new List<Guid>();
                if (arrLoaiTuVan != null)
                {
                    var arrIDCN = arrLoaiTuVan.Split(',');
                    for (int i = 0; i < arrIDCN.Length; i++)
                    {
                        lstLoaiCV.Add(new Guid(arrIDCN[i]));
                    }
                }
                if (lstLoaiCV.Count > 0)
                {
                    lst = lst.Where(hd => lstLoaiCV.Contains(hd.ID_LoaiTuVan.Value)).ToList();
                }

                List<Guid> lstIDNV = new List<Guid>();
                if (arrMangIDNhanVien != null)
                {
                    var arrIDNV = arrMangIDNhanVien.Split(',');
                    for (int i = 0; i < arrIDNV.Length; i++)
                    {
                        lstIDNV.Add(new Guid(arrIDNV[i]));
                    }
                }
                if (lstIDNV.Count > 0)
                {
                    lst = lst.Where(hd => lstIDNV.Contains(hd.ID_NhanVien.Value) || lstIDNV.Contains(hd.ID_NhanVienQuanLy.Value)).ToList();
                }

                if (dayStart == dayEnd)
                {
                    lst = lst.Where(hd => hd.NgayGio.Year == dayStart.Year
                    && hd.NgayGio.Month == dayStart.Month
                    && hd.NgayGio.Day == dayStart.Day).ToList();
                }
                else
                {
                    lst = lst.Where(hd => hd.NgayGio >= dayStart && hd.NgayGio < dayEnd).ToList();
                }

                switch (trangthai)
                {
                    case 1: // Đã hủy
                        lst = lst.Where(hd => hd.TrangThai == "3").ToList();
                        break;
                    case 2: // phiếu tạm
                        lst = lst.Where(hd => hd.TrangThai == "1").ToList();
                        break;
                    case 3: // Hoàn thành
                        lst = lst.Where(hd => hd.TrangThai == "2").ToList();
                        break;
                    case 4: // phiếu tạm + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "2").ToList();
                        break;
                    case 5: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "1").ToList();
                        break;
                    case 6: // Hoàn thành + Đã hủy
                        break;
                    case 7: //
                        lst = new List<ChamSocKhachHangDTO>();
                        break;
                    case 0: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "3").ToList();
                        break;
                    default: // tam luu
                        break;
                }

                int Rows = lst.Count;
                List<ChamSocKhachHangDTO> lstReturn = lst.Skip(currentPage * pageSize).Take(pageSize).ToList();
                double round = lst.Count();
                var pageCount = System.Math.Ceiling(round / pageSize);
                JsonResultExampleCongViec<ChamSocKhachHangDTO> jsonResult = new JsonResultExampleCongViec<ChamSocKhachHangDTO>
                {
                    TotalRow = Rows,
                    PageCount = pageCount,
                    LstData = lstReturn
                };
                return Json(jsonResult);
            }
        }

        //lịch hẹn
        public System.Web.Http.Results.JsonResult<JsonResultExampleCongViec<ChamSocKhachHangDTO>> GetAllLichHenWhere(int currentPage, int pageSize, string arrLoaiLichHen, DateTime dayStart, DateTime dayEnd, string txtSearch, int trangthai, string arrMangIDNhanVien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                List<ChamSocKhachHangDTO> lst = classTuVan.GetListLichHen(txtSearch, iddonvi);
                List<Guid> lstLoaiCV = new List<Guid>();
                if (arrLoaiLichHen != null)
                {
                    var arrIDCN = arrLoaiLichHen.Split(',');
                    for (int i = 0; i < arrIDCN.Length; i++)
                    {
                        lstLoaiCV.Add(new Guid(arrIDCN[i]));
                    }
                }
                if (lstLoaiCV.Count > 0)
                {
                    lst = lst.Where(hd => lstLoaiCV.Contains(hd.ID_LoaiTuVan.Value)).ToList();
                }

                List<Guid> lstIDNV = new List<Guid>();
                if (arrMangIDNhanVien != null)
                {
                    var arrIDNV = arrMangIDNhanVien.Split(',');
                    for (int i = 0; i < arrIDNV.Length; i++)
                    {
                        lstIDNV.Add(new Guid(arrIDNV[i]));
                    }
                }
                if (lstIDNV.Count > 0)
                {
                    lst = lst.Where(hd => lstIDNV.Contains(hd.ID_NhanVien.Value) || lstIDNV.Contains(hd.ID_NhanVienQuanLy.Value)).ToList();
                }

                if (dayStart == dayEnd)
                {
                    lst = lst.Where(hd => hd.NgayGio.Year == dayStart.Year
                    && hd.NgayGio.Month == dayStart.Month
                    && hd.NgayGio.Day == dayStart.Day).ToList();
                }
                else
                {
                    lst = lst.Where(hd => hd.NgayGio >= dayStart && hd.NgayGio < dayEnd).ToList();
                }

                switch (trangthai)
                {
                    case 1: // Đã hủy
                        lst = lst.Where(hd => hd.TrangThai == "3").ToList();
                        break;
                    case 2: // phiếu tạm
                        lst = lst.Where(hd => hd.TrangThai == "1").ToList();
                        break;
                    case 3: // Hoàn thành
                        lst = lst.Where(hd => hd.TrangThai == "2").ToList();
                        break;
                    case 4: // phiếu tạm + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "2").ToList();
                        break;
                    case 5: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "1").ToList();
                        break;
                    case 6: // Hoàn thành + Đã hủy
                        break;
                    case 7: //
                        lst = new List<ChamSocKhachHangDTO>();
                        break;
                    case 0: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "3").ToList();
                        break;
                    default: // tam luu
                        break;
                }

                int Rows = lst.Count;
                List<ChamSocKhachHangDTO> lstReturn = lst.Skip(currentPage * pageSize).Take(pageSize).ToList();
                double round = lst.Count();
                var pageCount = System.Math.Ceiling(round / pageSize);
                JsonResultExampleCongViec<ChamSocKhachHangDTO> jsonResult = new JsonResultExampleCongViec<ChamSocKhachHangDTO>
                {
                    TotalRow = Rows,
                    PageCount = pageCount,
                    LstData = lstReturn
                };
                return Json(jsonResult);
            }
        }

        //phản hồi
        public System.Web.Http.Results.JsonResult<JsonResultExampleCongViec<ChamSocKhachHangDTO>> GetAllPhanHoiWhere(int currentPage, int pageSize, string arrLoaiLichHen, DateTime dayStart, DateTime dayEnd, string txtSearch, int trangthai, int xuly, string arrMangIDNhanVien, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                List<ChamSocKhachHangDTO> lst = classTuVan.GetListPhanHoi(txtSearch, iddonvi);
                List<Guid> lstLoaiCV = new List<Guid>();
                if (arrLoaiLichHen != null)
                {
                    var arrIDCN = arrLoaiLichHen.Split(',');
                    for (int i = 0; i < arrIDCN.Length; i++)
                    {
                        lstLoaiCV.Add(new Guid(arrIDCN[i]));
                    }
                }
                if (lstLoaiCV.Count > 0)
                {
                    lst = lst.Where(hd => lstLoaiCV.Contains(hd.ID_LoaiTuVan.Value)).ToList();
                }

                List<Guid> lstIDNV = new List<Guid>();
                if (arrMangIDNhanVien != null)
                {
                    var arrIDNV = arrMangIDNhanVien.Split(',');
                    for (int i = 0; i < arrIDNV.Length; i++)
                    {
                        lstIDNV.Add(new Guid(arrIDNV[i]));
                    }
                }
                if (lstIDNV.Count > 0)
                {
                    lst = lst.Where(hd => lstIDNV.Contains(hd.ID_NhanVien.Value) || lstIDNV.Contains(hd.ID_NhanVienQuanLy.Value)).ToList();
                }
                if (dayStart == dayEnd)
                {
                    lst = lst.Where(hd => hd.NgayGio.Year == dayStart.Year
                    && hd.NgayGio.Month == dayStart.Month
                    && hd.NgayGio.Day == dayStart.Day).ToList();
                }
                else
                {
                    lst = lst.Where(hd => hd.NgayGio >= dayStart && hd.NgayGio < dayEnd).ToList();
                }

                switch (trangthai)
                {
                    case 1: // Đã hủy
                        lst = lst.Where(hd => hd.MucDoPhanHoi == 3).ToList();
                        break;
                    case 2: // phiếu tạm
                        lst = lst.Where(hd => hd.MucDoPhanHoi == 1).ToList();
                        break;
                    case 3: // Hoàn thành
                        lst = lst.Where(hd => hd.MucDoPhanHoi == 2).ToList();
                        break;
                    case 4: // phiếu tạm + Đã hủy
                        lst = lst.Where(hd => hd.MucDoPhanHoi != 2).ToList();
                        break;
                    case 5: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.MucDoPhanHoi != 1).ToList();
                        break;
                    case 6: // Hoàn thành + Đã hủy
                        break;
                    case 7: //
                        lst = new List<ChamSocKhachHangDTO>();
                        break;
                    case 0: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.MucDoPhanHoi != 3).ToList();
                        break;
                    default: // tam luu
                        break;
                }

                switch (xuly)
                {
                    case 1: // Đã hủy
                        lst = lst.Where(hd => hd.TrangThai == "3").ToList();
                        break;
                    case 2: // phiếu tạm
                        lst = lst.Where(hd => hd.TrangThai == "1").ToList();
                        break;
                    case 3: // Hoàn thành
                        lst = lst.Where(hd => hd.TrangThai == "2").ToList();
                        break;
                    case 4: // phiếu tạm + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "2").ToList();
                        break;
                    case 5: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "1").ToList();
                        break;
                    case 6: // Hoàn thành + Đã hủy
                        break;
                    case 7: //
                        lst = new List<ChamSocKhachHangDTO>();
                        break;
                    case 0: // Hoàn thành + Đã hủy
                        lst = lst.Where(hd => hd.TrangThai != "3").ToList();
                        break;
                    default: // tam luu
                        break;
                }

                int Rows = lst.Count;
                List<ChamSocKhachHangDTO> lstReturn = lst.Skip(currentPage * pageSize).Take(pageSize).ToList();
                double round = lst.Count();
                var pageCount = System.Math.Ceiling(round / pageSize);
                JsonResultExampleCongViec<ChamSocKhachHangDTO> jsonResult = new JsonResultExampleCongViec<ChamSocKhachHangDTO>
                {
                    TotalRow = Rows,
                    PageCount = pageCount,
                    LstData = lstReturn
                };
                return Json(jsonResult);
            }
        }

        public List<ListLHPages> getAllPage(List<LH_ChamSocKhachHang> lstLHs, List<ListLHPages> listPage, float sohang)
        {
            if (lstLHs != null)
            {
                int dem = 1;
                float SoTrang = lstLHs.Count / sohang;
                for (int i = 0; i < SoTrang; i++)
                {
                    ListLHPages LH_page = new ListLHPages();
                    LH_page.SoTrang = dem;
                    listPage.Add(LH_page);
                    dem = dem + 1;
                }
                return listPage;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public IEnumerable<SP_NSCongViec> SP_GetListCongViec_Where(string txtSearch)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_TuVan classTuVan = new class_TuVan(db);
                    return classTuVan.SP_GetListCongViec_Where(txtSearch);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ChamSocKhachHangAPI_SP_GetListCongViec_Where: " + ex.InnerException + ex.Message);
                return null;
            }
        }
        [HttpGet]
        public IEnumerable<SP_NSCongViec> SP_GetListCongViec_ByDoiTuong(string idDoiTuong, string idNhanVien)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_TuVan classTuVan = new class_TuVan(db);
                    classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                    var guidIDNhanVien = new Guid(idNhanVien);
                    var data = classTuVan.SP_GetListCongViec_ByDoiTuong(idDoiTuong, idNhanVien);

                    // LaAdmin: get all
                    var htNguoiDung = classHTNguoiDung.Get(x => x.ID_NhanVien == guidIDNhanVien && x.LaAdmin == true);
                    if (htNguoiDung == null)
                    {
                        data = data.Where(x => x.ID_NhanVienQuanLy == guidIDNhanVien || x.ID_NhanVienChiaSe == guidIDNhanVien).ToList();
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ChamSocKhachHangAPI_SP_GetListCongViec_ByDoiTuong: " + ex.InnerException + ex.Message);
                return null;
            }
        }

        [HttpGet]
        public IHttpActionResult SP_GetListCongViec_ByDoiTuong(Guid idDoiTuong, Guid idNhanVien, Guid idChiNhanh)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_TuVan classTuVan = new class_TuVan(db);
                    classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                    var data = classTuVan.SP_GetListCongViec_ByKhachHang(idDoiTuong, idChiNhanh);
                    // LaAdmin: get all
                    var htNguoiDung = classHTNguoiDung.Get(x => x.ID_NhanVien == idNhanVien && x.LaAdmin == true);
                    if (htNguoiDung == null)
                    {
                        data = data.Where(x => x.ID_NhanVienQuanLy == idNhanVien || x.StaffIDs.Contains(idNhanVien.ToString())).ToList();
                    }
                    return Json(new { res = true, data = data });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException });
            }
        }

        [HttpGet, HttpPost]
        public bool Update_LoaiKhachHang_DMDoituong(string idDoiTuong, Guid? cusType)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_TuVan classTuVan = new class_TuVan(db);
                    return classTuVan.SP_UpdateCusType_DMDoiTuong(idDoiTuong, cusType);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ChamSocKhachHangAPI_Update_LoaiKhachHang_DMDoituong: " + ex.InnerException + ex.Message);
                return false;
            }
        }

        #endregion

        public List<ChamSocKhachHang> GetListLichHens()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<ChamSocKhachHang> lstAllLichHens = classLichHen.Gets(n => n.PhanLoai == 3).OrderByDescending(n => n.NgayGio).ToList();// Phân loại = 3 là lịch hẹn

                if (lstAllLichHens != null && lstAllLichHens.Count() > 0)
                {
                    List<ChamSocKhachHang> lsrReturns = new List<ChamSocKhachHang>();
                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    foreach (ChamSocKhachHang item in lstAllLichHens)
                    {
                        NS_NhanVien dt0 = _ClassNS_NhanVien.Select_NhanVien(item.ID_NhanVien);

                        string sNguoiTao = string.Empty;
                        if (dt0 != null)
                        {
                            sNguoiTao = dt0.TenNhanVien;

                        }
                        ChamSocKhachHang itemData = new ChamSocKhachHang
                        {
                            ID = item.ID,
                            Ma_TieuDe = item.Ma_TieuDe,
                            NgayGio = item.NgayGio,
                            NgayGioKetThuc = item.NgayGioKetThuc,
                            TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : ""))),
                            NoiDung = item.NoiDung,
                            TenKhachHang = item.DM_DoiTuong != null ? item.DM_DoiTuong.TenDoiTuong : "",
                            TenNV = sNguoiTao,
                            TenLoaiTV = item.DM_LoaiTuVanLichHen != null ? item.DM_LoaiTuVanLichHen.TenLoaiTuVanLichHen : "",
                            TraLoi = item.TraLoi,
                            NhacNho = item.NhacNho,
                        };
                        lsrReturns.Add(itemData);
                    }
                    return lsrReturns;
                }
                else
                    return null;
            }
        }
        [HttpPost, HttpGet]
        public static List<DTStamp> GetListCalendar(DateTime from, DateTime to, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                List<ChamSocKhachHang> cskh = classLichHen.GetsFromTo(from, to, ID_ChiNhanh).ToList();
                List<DTStamp> listcskh = new List<DTStamp>();
                foreach (var item in cskh)
                {
                    DTStamp dttemp = new DTStamp();
                    dttemp.id = item.ID;
                    dttemp.ID = item.ID;
                    dttemp.title = item.Ma_TieuDe;
                    dttemp.@class = "event-important";
                    dttemp.start = CvtDatetimetoTimestamps.ConvertToMiliSeconds(item.NgayGio);
                    if (item.NgayGioKetThuc.HasValue)
                    {
                        dttemp.end = CvtDatetimetoTimestamps.ConvertToMiliSeconds(item.NgayGioKetThuc.Value);
                    }

                    //trinhpv
                    dttemp.ID_KhachHang = item.ID_KhachHang;
                    dttemp.ID_LoaiTuVan = item.ID_LoaiTuVan;
                    dttemp.ID_NhanVienQuanLy = item.ID_NhanVienQuanLy ?? Guid.Empty;
                    dttemp.NguoiSua = item.NguoiSua;
                    dttemp.NguoiTao = item.NguoiTao;
                    dttemp.ID_NhanVien = item.ID_NhanVien;
                    dttemp.ID_DonVi = item.ID_DonVi;

                    dttemp.Ma_TieuDe = item.Ma_TieuDe;
                    dttemp.TenDoiTuong = item.DM_DoiTuong.TenDoiTuong;
                    dttemp.TenLoaiTuVanLichHen = item.TenLoaiTV;
                    dttemp.NhacNho = item.NhacNho;
                    dttemp.TrangThai = item.TrangThai;
                    dttemp.NoiDung = item.NoiDung;
                    dttemp.TenNhanVien = item.NS_NhanVien == null ? "" : item.NS_NhanVien.TenNhanVien;
                    listcskh.Add(dttemp);
                }
                return listcskh;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListDichVu_inLichHen_ByEventID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    class_LichHen classLichHen = new class_LichHen(db);
                    List<Calendar_DichVu> dichvu = classLichHen.GetListDichVu_inLichHen_ByEventID(id);
                    return Json(new { res = true, data = dichvu });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetListLichHen_FullCalendar(ParamCalendar param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    class_LichHen classLichHen = new class_LichHen(db);
                    List<SP_Calendar> cskh = classLichHen.GetListCalendar(param);
                    int? totalRow = 1; double? totalPage = 1;
                    if (cskh != null && cskh.Count() > 0)
                    {
                        totalRow = cskh.FirstOrDefault().TotalRow;
                        totalPage = cskh.FirstOrDefault().TotalPage;
                    }
                    return Json(new { res = true, data = cskh, TotalRow = totalRow, PageCount = totalPage });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [ResponseType(typeof(ChamSocKhachHang))]
        public IHttpActionResult GetPhanHoi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ChamSocKhachHang ChamSocKhachHang = db.ChamSocKhachHang.Where(p => p.ID == id).FirstOrDefault();
                ChamSocKhachHangDTO ct = new ChamSocKhachHangDTO();
                ct.ID = ChamSocKhachHang.ID;
                ct.Ma_TieuDe = ChamSocKhachHang.Ma_TieuDe;
                ct.ID_LoaiTuVan = ChamSocKhachHang.ID_LoaiTuVan;
                ct.ID_KhachHang = ChamSocKhachHang.ID_KhachHang;
                ct.ID_NhanVien = ChamSocKhachHang.ID_NhanVien;
                ct.TenKhachHang = db.DM_DoiTuong.Where(p => p.ID == ChamSocKhachHang.ID_KhachHang).FirstOrDefault().TenDoiTuong;
                ct.TenNV = db.NS_NhanVien.Where(p => p.ID == ChamSocKhachHang.ID_NhanVien).FirstOrDefault().TenNhanVien;
                ct.NgayGio = ChamSocKhachHang.NgayGio;
                ct.ThoiGianHenLai = ChamSocKhachHang.ThoiGianHenLai;
                ct.NoiDung = ChamSocKhachHang.NoiDung;
                ct.TraLoi = ChamSocKhachHang.TraLoi;
                ct.TrangThai = ChamSocKhachHang.TrangThai;
                ct.MucDoPhanHoi = ChamSocKhachHang.MucDoPhanHoi;
                if (ChamSocKhachHang == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }


        public List<ChamSocKhachHang> GetListPhanHois()
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                class_PhanHoi classPhanHoi = new class_PhanHoi(db);
                List<ChamSocKhachHang> lstAllPhanHois = classPhanHoi.Gets(n => n.PhanLoai == 2);// Phân loại = 2 là phản hồi

                if (lstAllPhanHois != null && lstAllPhanHois.Count() > 0)
                {

                    ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                    List<ChamSocKhachHang> lsrReturns = new List<ChamSocKhachHang>();
                    foreach (ChamSocKhachHang item in lstAllPhanHois)
                    {

                        NS_NhanVien dt = _ClassNS_NhanVien.Select_NhanVien(item.ID_NhanVien);
                        string sNguoiTao = string.Empty;
                        if (dt != null)
                        {
                            sNguoiTao = dt.TenNhanVien;
                        }
                        ChamSocKhachHang itemData = new ChamSocKhachHang
                        {
                            ID = item.ID,
                            Ma_TieuDe = item.Ma_TieuDe,
                            NgayGio = item.NgayGio,
                            TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Chưa xử lý" : (item.TrangThai == "2" ? "Đang xử lý" : (item.TrangThai == "3" ? "Đã xong" : ""))),
                            strMucDoPhanHoi = item.MucDoPhanHoi.ToString() == null ? "" : (item.MucDoPhanHoi == 1 ? "Cao" : (item.MucDoPhanHoi == 2 ? "Trung bình" : (item.MucDoPhanHoi == 3 ? "Thấp" : ""))),
                            NoiDung = item.NoiDung,
                            TenKhachHang = item.DM_DoiTuong != null ? item.DM_DoiTuong.TenDoiTuong : "",
                            TenNV = sNguoiTao,
                            TenLoaiTV = item.DM_LoaiTuVanLichHen != null ? item.DM_LoaiTuVanLichHen.TenLoaiTuVanLichHen : "",
                            TraLoi = item.TraLoi,
                        };
                        lsrReturns.Add(itemData);

                    }
                    return lsrReturns;
                }
                else
                    return null;
            }

        }

        //Get Tu vấn by Loai Tu Van
        [HttpGet, ActionName("GetListTuVanByLoaiTuVan")]
        public List<ChamSocKhachHangDTO> GetListTuVanByLoaiTuVan(Guid id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                List<ChamSocKhachHang> lstAllGBs = classTuVan.Gets(n => n.PhanLoai == 1);
                List<ChamSocKhachHangDTO> lstAll = new List<ChamSocKhachHangDTO>();

                ClassNS_NhanVien _ClassNS_NhanVien = new ClassNS_NhanVien(db);
                foreach (var item in lstAllGBs)
                {
                    NS_NhanVien dt = _ClassNS_NhanVien.Select_NhanVien(item.ID_NhanVien);
                    string sNguoiTao = string.Empty;
                    if (dt != null)
                    {
                        sNguoiTao = dt.TenNhanVien;
                    }
                    //done
                    ChamSocKhachHangDTO itemData = new ChamSocKhachHangDTO();
                    itemData.ID = item.ID;
                    itemData.Ma_TieuDe = item.Ma_TieuDe;
                    itemData.NgayGio = item.NgayGio;
                    itemData.NoiDung = item.NoiDung;
                    itemData.TraLoi = item.TraLoi;
                    itemData.ID_KhachHang = item.ID_KhachHang;
                    itemData.ID_NhanVien = item.ID_NhanVien;
                    itemData.ID_LoaiTuVan = item.ID_LoaiTuVan;
                    itemData.TrangThai = item.TrangThai;
                    //itemData.TrangThai = item.TrangThai == null ? "" : (item.TrangThai == "1" ? "Tham khảo" : (item.TrangThai == "2" ? "Tiềm năng" : (item.TrangThai == "3" ? "Hủy" : "")));
                    itemData.TenLoaiTV = item.DM_LoaiTuVanLichHen != null ? item.DM_LoaiTuVanLichHen.TenLoaiTuVanLichHen : "";
                    itemData.TenKhachHang = item.DM_DoiTuong != null ? item.DM_DoiTuong.TenDoiTuong : "";
                    itemData.TenNV = sNguoiTao;
                    if (itemData.ID_LoaiTuVan == id)
                    {
                        lstAll.Add(itemData);
                    }
                };
                return lstAll;
            }

        }

        // Phiếu tư vấn
        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult Delete_PhieuTuVan(Guid id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                class_TuVan classTuVan = new class_TuVan(db);
                string strDel = classTuVan.Delete_PhieuTuVan(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // Lịch hẹn
        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult Delete_LichHen(Guid id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                class_LichHen classLichHen = new class_LichHen(db);
                string strDel = classLichHen.Delete_LichHen(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // Phản hồi
        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult Delete_PhanHoi(Guid id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                class_PhanHoi classPhanHoi = new class_PhanHoi(db);
                string strDel = classPhanHoi.Delete_PhanHoi(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string UpdateCongViec(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    ChamSocKhachHang item = db.ChamSocKhachHang.Find(id);
                    if (item != null)
                    {
                        item.TrangThai = "3";
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

        #region Delete
        [HttpDelete]
        [ResponseType(typeof(string))]
        public string Delete_LoaiCongViec(Guid id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                libDM_LoaiTuVanLichHen.class_DM_LoaiTuVanLichHen classLoaiTV = new libDM_LoaiTuVanLichHen.class_DM_LoaiTuVanLichHen(db);
                return classLoaiTV.Delete_LoaiTuVan(id);
            }
        }

        [HttpDelete]
        [ResponseType(typeof(string))]
        public string Delete_LoaiCongViecLT(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Error DB null";
                }
                else
                {
                    DM_LoaiTuVanLichHen objLoaiCV = db.DM_LoaiTuVanLichHen.Find(id);
                    List<ChamSocKhachHang> lst = db.ChamSocKhachHang.Where(p => p.ID_LoaiTuVan == objLoaiCV.ID).ToList();
                    if (lst.Count == 0)
                    {
                        objLoaiCV.TrangThai = 0;
                        db.Entry(objLoaiCV).State = EntityState.Modified;
                        db.SaveChanges();
                        return "";
                    }
                    else
                    {
                        return "Loại công việc đang được sử dụng trong hệ thống";
                    }
                }
            }
        }
        #endregion


        [HttpGet]
        public IHttpActionResult GetTrangThaiTimKiem()
        {
            try
            {

                var ttcongviec = commonEnumHellper.ListCongViec.Select(o => new { IsSelected = false, ID = o.Key, Name = o.Value });
                var db = SystemDBContext.GetDBContext();
                var ttkhachhang = db.DM_DoiTuong_TrangThai.Select(o => new { IsSelected = false, o.ID, Name = o.TenTrangThai });

                return ActionTrueData(new { ttcongviec, ttkhachhang });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        #region Đặt lịch - checkin
        [HttpPost]
        public IHttpActionResult PostDatLichCheckin([FromBody] JObject objIn)
        {
            //if (objIn["IdChiNhanhs"] != null)
            //    param.IdChiNhanhs = objIn["IdChiNhanhs"].ToObject<List<string>>();
            DatLichIn param = new DatLichIn();
            if (objIn["IdChiNhanh"] != null)
            {
                param.IdChiNhanh = objIn["IdChiNhanh"].ToObject<Guid>();
            }
            if (objIn["TenKhachHang"] != null)
            {
                param.TenKhachHang = objIn["TenKhachHang"].ToObject<string>();
            }
            if (objIn["SoDienThoai"] != null)
            {
                param.SoDienThoai = objIn["SoDienThoai"].ToObject<string>();
            }
            if (objIn["DiaChi"] != null)
            {
                param.DiaChi = objIn["DiaChi"].ToObject<string>();
            }
            if (objIn["NgaySinh"] != null)
            {
                param.NgaySinh = objIn["NgaySinh"].ToObject<DateTime>();
            }
            if (objIn["BienSo"] != null)
            {
                param.BienSo = objIn["BienSo"].ToObject<string>();
            }
            if (objIn["LoaiXe"] != null)
            {
                param.LoaiXe = objIn["LoaiXe"].ToObject<string>();
            }
            if (objIn["ThoiGian"] != null)
            {
                param.ThoiGian = objIn["ThoiGian"].ToObject<DateTime>();
            }
            if (objIn["ListHangHoa"] != null)
            {
                param.ListIdHangHoa = objIn["ListHangHoa"].ToObject<List<Guid>>();
            }
            if (objIn["ListNhanVien"] != null)
            {
                param.ListIdNhanVien = objIn["ListNhanVien"].ToObject<List<Guid>>();
            }
            string subdomain = "";
            if (objIn["Subdomain"] != null)
            {
                subdomain = objIn["Subdomain"].ToObject<string>();
            }
            using (SsoftvnContext db = new SsoftvnContext(subdomain))
            {
                CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                cdatlich.InsertDatLich(param);
            }
            return ActionTrueNotData("");
        }
        [HttpPost]
        public IHttpActionResult PostDatLichCheckin1([FromBody] JObject objIn)
        {
            Guid IdChiNhanh = Guid.Empty;
            if (objIn["IdChiNhanh"] != null)
            {
                IdChiNhanh = objIn["IdChiNhanh"].ToObject<Guid>();
            }
            Guid IdKhachHang = Guid.Empty;
            if (objIn["IdKhachHang"] != null)
            {
                IdKhachHang = objIn["IdKhachHang"].ToObject<Guid>();
            }
            Guid IdXe = Guid.Empty;
            if (objIn["IdXe"] != null)
            {
                IdXe = objIn["IdXe"].ToObject<Guid>();
            }
            DateTime ThoiGian = DateTime.Now;
            if (objIn["ThoiGian"] != null)
            {
                ThoiGian = objIn["ThoiGian"].ToObject<DateTime>();
            }
            List<Guid> ListHangHoa = new List<Guid>();
            if (objIn["ListHangHoa"] != null)
            {
                ListHangHoa = objIn["ListHangHoa"].ToObject<List<Guid>>();
            }
            List<Guid> ListNhanVien = new List<Guid>();
            if (objIn["ListNhanVien"] != null)
            {
                ListNhanVien = objIn["ListNhanVien"].ToObject<List<Guid>>();
            }
            bool check = false;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                check = cdatlich.InsertDatLich(IdKhachHang, IdChiNhanh, IdXe, ThoiGian, ListHangHoa, ListNhanVien, 1, 1);
            }
            if (check)
            {
                return ActionTrueNotData("");
            }
            else
            {
                return ActionFalseNotData("");
            }
        }

        [HttpPost]
        public IHttpActionResult PutDatLichCheckin([FromBody] JObject objIn)
        {
            Guid Id = Guid.Empty;
            if (objIn["Id"] != null)
            {
                Id = objIn["Id"].ToObject<Guid>();
            }
            Guid IdChiNhanh = Guid.Empty;
            if (objIn["IdChiNhanh"] != null)
            {
                IdChiNhanh = objIn["IdChiNhanh"].ToObject<Guid>();
            }
            Guid IdKhachHang = Guid.Empty;
            if (objIn["IdKhachHang"] != null)
            {
                IdKhachHang = objIn["IdKhachHang"].ToObject<Guid>();
            }
            Guid IdXe = Guid.Empty;
            if (objIn["IdXe"] != null)
            {
                IdXe = objIn["IdXe"].ToObject<Guid>();
            }
            DateTime ThoiGian = DateTime.Now;
            if (objIn["ThoiGian"] != null)
            {
                ThoiGian = objIn["ThoiGian"].ToObject<DateTime>();
            }
            List<Guid> ListHangHoaAdd = new List<Guid>();
            if (objIn["ListHangHoaAdd"] != null)
            {
                ListHangHoaAdd = objIn["ListHangHoaAdd"].ToObject<List<Guid>>();
            }
            List<Guid> ListHangHoaRemove = new List<Guid>();
            if (objIn["ListHangHoaRemove"] != null)
            {
                ListHangHoaRemove = objIn["ListHangHoaRemove"].ToObject<List<Guid>>();
            }
            List<Guid> ListNhanVienAdd = new List<Guid>();
            if (objIn["ListNhanVienAdd"] != null)
            {
                ListNhanVienAdd = objIn["ListNhanVienAdd"].ToObject<List<Guid>>();
            }
            List<Guid> ListNhanVienRemove = new List<Guid>();
            if (objIn["ListNhanVienRemove"] != null)
            {
                ListNhanVienRemove = objIn["ListNhanVienRemove"].ToObject<List<Guid>>();
            }
            bool check = false;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                check = cdatlich.UpdateDatLich(Id, IdKhachHang, IdChiNhanh, IdXe, ThoiGian, ListHangHoaAdd, ListHangHoaRemove, ListNhanVienAdd, ListNhanVienRemove);
            }
            if (check)
            {
                return ActionTrueNotData("");
            }
            else
            {
                return ActionFalseNotData("");
            }
        }

        [HttpPost]
        public IHttpActionResult GetListDatLich([FromBody] JObject objIn)
        {
            ParamGetListDatLich param = new ParamGetListDatLich();
            if (objIn["IdChiNhanhs"] != null)
                param.IdChiNhanhs = objIn["IdChiNhanhs"].ToObject<List<string>>();
            if (objIn["ThoiGianFrom"] != null && objIn["ThoiGianFrom"].ToObject<string>() != "")
                param.ThoiGianFrom = objIn["ThoiGianFrom"].ToObject<DateTime>();
            if (objIn["ThoiGianTo"] != null && objIn["ThoiGianTo"].ToObject<string>() != "")
                param.ThoiGianTo = objIn["ThoiGianTo"].ToObject<DateTime>();
            if (objIn["TrangThais"] != null)
                param.TrangThais = objIn["TrangThais"].ToObject<List<int>>();
            if (objIn["TextSearch"] != null)
                param.TextSearch = objIn["TextSearch"].ToObject<string>();
            if (objIn["CurrentPage"] != null)
                param.CurrentPage = objIn["CurrentPage"].ToObject<int>() - 1;
            if (objIn["PageSize"] != null)
                param.PageSize = objIn["PageSize"].ToObject<int>();
            string subdomain = "";
            if (objIn["Subdomain"] != null)
                subdomain = objIn["Subdomain"].ToObject<string>();
            List<GetListDatLichResult> dataDatLich = new List<GetListDatLichResult>();
            if (subdomain != "")
            {
                using (SsoftvnContext db = new SsoftvnContext(subdomain))
                {
                    CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                    dataDatLich = cdatlich.GetListDatLich(param);
                }
            }
            else
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                    dataDatLich = cdatlich.GetListDatLich(param);
                }
            }
            int count = 0;
            if (dataDatLich.Count != 0)
            {
                count = dataDatLich[0].TotalRow;
            }
            int page = 0;
            int[] listpage = new int[] { };
            if (param.PageSize != 0)
            {
                listpage = GetListPage(count, param.PageSize, param.CurrentPage + 1, ref page);
            }
            return ActionTrueData(new
            {
                data = dataDatLich,
                ListPage = listpage,
                PageView = "Hiển thị " + ((param.CurrentPage) * param.PageSize + 1) + " - " + ((param.CurrentPage) * param.PageSize + dataDatLich.Count) + " trên tổng số " + count + " bản ghi",
                NumberOfPage = page
            });
        }

        [HttpPost]
        public IHttpActionResult UpdateTrangThaiDatLich([FromBody] JObject objIn)
        {
            Guid id = Guid.Empty;
            if (objIn["Id"] != null)
                id = objIn["Id"].ToObject<Guid>();
            int TrangThai = -1;
            if (objIn["TrangThai"] != null)
                TrangThai = objIn["TrangThai"].ToObject<int>();
            Guid idnhanvien = Guid.Empty;
            if (objIn["IdNhanVien"] != null)
                idnhanvien = objIn["IdNhanVien"].ToObject<Guid>();
            Guid iddonvi = Guid.Empty;
            if (objIn["IdDonVi"] != null)
                iddonvi = objIn["IdDonVi"].ToObject<Guid>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (TrangThai != -1)
                {
                    CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                    bool check = cdatlich.UpdateTrangThaiDatLich(TrangThai, id, iddonvi, idnhanvien);
                    if (check)
                    {
                        return ActionTrueNotData("");
                    }
                    else
                    {
                        return ActionFalseNotData("");
                    }
                }
            }
            return ActionFalseNotData("");
        }

        [HttpPost]
        public IHttpActionResult UpdateIdDoiTuong([FromBody] JObject objIn)
        {
            Guid id = Guid.Empty;
            if (objIn["Id"] != null)
                id = objIn["Id"].ToObject<Guid>();
            Guid iddoituong = Guid.Empty;
            if (objIn["IdDoiTuong"] != null)
                iddoituong = objIn["IdDoiTuong"].ToObject<Guid>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (iddoituong != Guid.Empty)
                {
                    CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                    bool check = cdatlich.UpdateDoiTuongDatLich(id, iddoituong);
                    if (check)
                    {
                        return ActionTrueNotData("");
                    }
                    else
                    {
                        return ActionFalseNotData("");
                    }
                }
            }
            return ActionFalseNotData("");
        }

        [HttpPost]
        public IHttpActionResult UpdateIdXe([FromBody] JObject objIn)
        {
            Guid id = Guid.Empty;
            if (objIn["Id"] != null)
                id = objIn["Id"].ToObject<Guid>();
            Guid idxe = Guid.Empty;
            if (objIn["IdXe"] != null)
                idxe = objIn["IdXe"].ToObject<Guid>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (idxe != Guid.Empty)
                {
                    CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                    bool check = cdatlich.UpdateXeDatLich(id, idxe);
                    if (check)
                    {
                        return ActionTrueNotData("");
                    }
                    else
                    {
                        return ActionFalseNotData("");
                    }
                }
            }
            return ActionFalseNotData("");
        }

        [HttpGet]
        public IHttpActionResult GetChiTietDatLich(string id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    CDatLichCheckin cdatlich = new CDatLichCheckin(db);
                    List<HangHoaDatLichChiTiet> lstHangHoa = new List<HangHoaDatLichChiTiet>();
                    List<NhanVienDatLichChiTiet> lstNhanVien = new List<NhanVienDatLichChiTiet>();
                    lstHangHoa = cdatlich.GetHangHoaDatLichChiTiet(new Guid(id));
                    lstNhanVien = cdatlich.GetNhanVienDatLichChiTiet(new Guid(id));
                    return ActionTrueData(new
                    {
                        HangHoa = lstHangHoa,
                        NhanVien = lstNhanVien
                    });
                }
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        #endregion
    }

    public class NS_CongViec_PhanLoaiDTO
    {
        public Guid ID { get; set; }
        public string LoaiCongViec { get; set; }
        public int TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }

    public class DTStamp
    {
        public Guid id { get; set; }
        public Guid ID { get; set; }
        public string @class { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string url { get; set; }
        //trinhpv
        public Guid? ID_KhachHang { get; set; } //foreign key: NS_NhanVien
        public Guid? ID_LoaiTuVan { get; set; } //foreign key: DM_LoaiTuVanLichHen
        public Guid? ID_NhanVien { get; set; }
        public Guid ID_NhanVienQuanLy { get; set; } //foreign key: NS_NguoiDung. tài khoản quản lý phiếu tư vấn, ... Mặc định người tạo là người quản lý
        public string NguoiTao { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu
        public string NguoiSua { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu 
        public Guid ID_DonVi { get; set; }

        public string Ma_TieuDe { get; set; }
        public DateTime NgayGio { get; set; }
        public DateTime NgayGioKetThuc { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenLoaiTuVanLichHen { get; set; }
        public int NhacNho { get; set; }
        public string TrangThai { get; set; }
        public string NoiDung { get; set; }
        public string TenNhanVien { get; set; }
    }

    public class list_LHCSKH
    {
        public Guid ID { get; set; }
        public Guid? ID_KhachHang { get; set; } //foreign key: NS_NhanVien
        public Guid? ID_LoaiTuVan { get; set; } //foreign key: DM_LoaiTuVanLichHen
        public Guid? ID_NhanVien { get; set; }
        public Guid ID_NguoiQuanLy { get; set; } //foreign key: NS_NguoiDung. tài khoản quản lý phiếu tư vấn, ... Mặc định người tạo là người quản lý
        public Guid ID_NguoiTao { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu
        public Guid? ID_NguoiSua { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu 

        public string Ma_TieuDe { get; set; }
        public DateTime NgayGio { get; set; }
        public DateTime NgayGioKetThuc { get; set; }
        public string TenKhachHang { get; set; }
        public string TenLoaiTV { get; set; }
        public int NhacNho { get; set; }
        public string TrangThai { get; set; }
        public string NoiDung { get; set; }
        public string TenNV { get; set; }
    }
}
