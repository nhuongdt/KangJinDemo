using banhang24.Hellper;
using libDM_DoiTuong;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class OptinFormKhachHangAPIController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult getlist_TruongThongTinOF(string SubDomain, Guid ID_OptinForm, int LoaiTruongThongTin)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext(SubDomain);
            List<OF_TruongThongTinPROC> lst = new List<OF_TruongThongTinPROC>();
            List<SqlParameter> sql = new List<SqlParameter>();
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
        public List<DMTinhThanhDTO> GetListTinhThanh(string SubDomain)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                return classDoiTuong.GetListTinhThanhOF(null, SubDomain);
            }
        }
        [AcceptVerbs("GET", "POST")]
        public List<DM_QuanHuyen> GetListQuanHuyen(Guid idTinhThanh, string SubDomain)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_DoiTuong classDoiTuong = new classDM_DoiTuong(db);
                return classDoiTuong.GetListQuanHuyenOF(id => id.ID_TinhThanh == idTinhThanh, SubDomain).ToList();
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult InsertOF_KhachHang([FromBody]JObject data, string SubDomain, Guid ID_OptinForm, string Link)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext(SubDomain);
            OptinForm_DoiTuong optinForm_DoiTuong = data["optinForm_DoiTuong"].ToObject<OptinForm_DoiTuong>();
            string TenKhongDau = CommonStatic.ConvertToUnSign(optinForm_DoiTuong.TenDoiTuong.Trim());
            string TenChuCaiDau = CommonStatic.GetCharsStart(optinForm_DoiTuong.TenDoiTuong.Trim());
            try
            {
                // insert optinForm
                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID_OptinForm", ID_OptinForm));
                sqlPRM.Add(new SqlParameter("AnhDaiDien", optinForm_DoiTuong.AnhDaiDien));
                sqlPRM.Add(new SqlParameter("TenDoiTuong", optinForm_DoiTuong.TenDoiTuong));
                sqlPRM.Add(new SqlParameter("TenDoiTuong_KhongDau", TenKhongDau));
                sqlPRM.Add(new SqlParameter("TenDoiTuong_ChuCaiDau", TenChuCaiDau));
                sqlPRM.Add(new SqlParameter("GioiTinh", optinForm_DoiTuong.GioiTinh));
                if (optinForm_DoiTuong.NgaySinh == null)
                    sqlPRM.Add(new SqlParameter("NgaySinh", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("NgaySinh", optinForm_DoiTuong.NgaySinh));
                sqlPRM.Add(new SqlParameter("SoDienThoai", optinForm_DoiTuong.SoDienThoai));
                sqlPRM.Add(new SqlParameter("Email", optinForm_DoiTuong.Email));
                sqlPRM.Add(new SqlParameter("DiaChi", optinForm_DoiTuong.DiaChi));
                if (optinForm_DoiTuong.ID_TinhThanh == null)
                    sqlPRM.Add(new SqlParameter("ID_TinhThanh", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("ID_TinhThanh", optinForm_DoiTuong.ID_TinhThanh));
                if (optinForm_DoiTuong.ID_QuanHuyen == null)
                    sqlPRM.Add(new SqlParameter("ID_QuanHuyen", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("ID_QuanHuyen", optinForm_DoiTuong.ID_QuanHuyen));
                sqlPRM.Add(new SqlParameter("MaSoThue", optinForm_DoiTuong.MaSoThue));
                sqlPRM.Add(new SqlParameter("LaCaNhan", optinForm_DoiTuong.LaCaNhan));
                sqlPRM.Add(new SqlParameter("NguoiGioiThieu", optinForm_DoiTuong.NguoiGioiThieu));
                if (optinForm_DoiTuong.ID_NhanVienPhuTrach == null)
                    sqlPRM.Add(new SqlParameter("ID_NhanVienPhuTrach", DBNull.Value));
                else
                    sqlPRM.Add(new SqlParameter("ID_NhanVienPhuTrach", optinForm_DoiTuong.ID_NhanVienPhuTrach));
                sqlPRM.Add(new SqlParameter("Link", Link));
                db.Database.ExecuteSqlCommand("exec insertOF_KhachHang @ID_OptinForm, @AnhDaiDien, @TenDoiTuong, @TenDoiTuong_KhongDau, @TenDoiTuong_ChuCaiDau, @GioiTinh," +
                    "@NgaySinh, @SoDienThoai, @Email, @DiaChi, @ID_TinhThanh, @ID_QuanHuyen, @MaSoThue, @LaCaNhan, @NguoiGioiThieu, @ID_NhanVienPhuTrach, @Link", sqlPRM.ToArray());
                return CreatedAtRoute("DefaultApi", new { id = ID_OptinForm }, optinForm_DoiTuong);
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex));
            }
        }
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult UploadImageStaff(string SubDomain)
        {
            try
            {
                var path = "";
                var URLAnh = "";
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];
                    var filenameImage = DateTime.Now.ToString("YYYY-MM-DD") + "_" + Guid.NewGuid().ToString() + ".jpg";
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/FolderImage/" + SubDomain + "/KhachHang_OptinFrom")))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/FolderImage/" + SubDomain + "/KhachHang_OptinFrom"));
                    }
                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/FolderImage/" + SubDomain + "/KhachHang_OptinFrom"), filenameImage);
                    file.SaveAs(path);
                    URLAnh = "/FolderImage/" + SubDomain + "/KhachHang_OptinFrom" + "/" + filenameImage;
                }
                return Json(new { res = true, mess = URLAnh});
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("IHttpActionResult UploadImageStaff(string SubDomain): " + ex.InnerException + ex.Message, SubDomain);
                return Json(new { res = true, mess = string.Empty });
            }
        }
        [AcceptVerbs("GET", "POST")]
        public Guid? getID_NhanVienPhuTrach(string SubDomain,string MaNhanVien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classdoituong = new classDM_DoiTuong(db);
                Guid? ID_NhanVienPhuTrach = classdoituong.getID_NhanVienPhuTrach(SubDomain, MaNhanVien);
                return ID_NhanVienPhuTrach;
            }
        }
    }

}
