using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model_banhang24vn;
using Newtonsoft.Json.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Model_banhang24vn.Common;
using System.Web.Services;
using Open24.Areas.AdminPage.Hellper;
using Model_banhang24vn.CustomView;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Model_banhang24vn.CustomView.Client;
using Open24.Appcache;
using Open24.Models;
using Model_banhang24vn.DAL;
using Open24.Hellper;
using System.Web.Configuration;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class PostAPIController : ApiBaseController
    {
        CuaHangDangKyService _CuaHangDangKyService = new CuaHangDangKyService();
        //Guid filenameImage = Guid.NewGuid();
        #region Insert
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult AddArticle([FromBody]JObject model)
        {
            News_Articles obj = model.ToObject<News_Articles>();
            string data = "";
            try
            {
                if (string.IsNullOrWhiteSpace(obj.Title))
                {
                    return ActionFalseNotData("Vui lòng nhập tiêu đề bài viết");
                }
                if (string.IsNullOrWhiteSpace(obj.Title))
                {
                    return ActionFalseNotData("Vui lòng nhập mô tả bài viết");
                }
                if (string.IsNullOrWhiteSpace(obj.Content))
                {
                    return ActionFalseNotData("Vui lòng nhập nội dung bài viết");
                }
                DateTime time = DateTime.Now;
                string format = "dd-MM-yyyy";
                var dt = time.ToString(format);
                News_Articles item = new News_Articles { };
                item.ID = obj.ID;
                item.Title = obj.Title;
                item.Summary = obj.Summary;
                item.UrlImage = obj.UrlImage;
                item.Content = obj.Content;
                item.Tag = obj.Tag;
                item.CategoryID = obj.CategoryID;
                item.CreatedBy = obj.CreatedBy;
                item.UpdatedBy = obj.UpdatedBy;
                item.CreateDate = DateTime.Now;
                item.UpdateDate = DateTime.Now;
                item.Status = obj.Status;
                item.Salary = obj.Salary;
                item.Address = obj.Address;
                item.Experience = obj.Experience;
                item.Position = obj.Position;
                item.Degree = obj.Degree;
                item.WorkingForm = obj.WorkingForm;
                item.NumberOfRecruits = obj.NumberOfRecruits;
                item.Gender = obj.Gender??false;
                item.Trades = obj.Trades;
                item.DatePost = obj.DatePost;
                item.ExpirationDate = obj.ExpirationDate;
                item.Url = obj.Url;
                //item.Url = StaticVariable.ConvetTitleToUrl(obj.Title);
                if (!string.IsNullOrWhiteSpace(obj.UrlImage))
                {
                    EventUpdateCache.AddFileImeagesCache(HttpContext.Current.Server.MapPath("/Appcache/manifest.appcache"), obj.UrlImage);
                }
                data = M_News_Post.InsertArticle(item, contant.SESSIONNGUOIDUNG!=null?contant.SESSIONNGUOIDUNG.UserID:(Guid?)null);
                    CacheHellper.Invalidate(CacheKey.News_Home_Slider);
                    CacheHellper.Invalidate(CacheKey.News_NewDate);
                return RetunJsonAction(true,string.Empty,new { timeout= contant.SESSIONNGUOIDUNG != null ?false:true, data=data});
            }
            catch (Exception ex)
            {
                data = ex.ToString();
            }
            return ActionFalseNotData("Đã xảy ra lỗi vui lòng thử lại sau");//ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, data));
        }

        [HttpPost]
        public IHttpActionResult UpdatePin(m_Article model)
        {
            try
            {
                if (model != null)
                {
                    var result = M_News_Post.UpdateArticlesPin(model.ID);
                    if (result == (int)Notification.ErrorCode.success)
                    {
                        return UpdateSuccess();
                    }
                }

                    return ActionFalseNotData("Không tim thấy bản ghi cần cập nhật");
                
            }
            catch (Exception e)
            {
                return Exeption(e);
            }
        }
        [HttpPost]
        public IHttpActionResult InsertCategory([FromBody]JObject model)
        {
            string status = "";
            string data = "";
            try
            {
                News_Categories obj = model.ToObject<News_Categories>();
                News_Categories item = new News_Categories { };
                item.Name = obj.Name;
                item.CategoryTypeID = obj.CategoryTypeID;

                data = M_News_Post.insertCategory(item);
            }
            catch (Exception ex)
            {
                status = ex.ToString();
            }
            return Ok(data);
        }

        [HttpPost]
        public IHttpActionResult ImageUploadFolder()
        {
            var path = "";
            string result = "";
            try
            {
                //if (HttpContext.Current.Request.Files.Count != 0)
                //{
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Current.Request.Files[i];
                        //var fileName = Path.GetFileName(file.FileName);
                        var filenameImage = Guid.NewGuid().ToString() + ".jpg";
                        //var filename = filenameImage.ToString();

                        DateTime time = DateTime.Now;
                        string format = "yyyyMMdd";
                        var dt = time.ToString(format);
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Img/" + dt)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Img/" + dt));
                        }

                        path = Path.Combine(HttpContext.Current.Server.MapPath("~/Img/" + dt), filenameImage);

                        file.SaveAs(path);
                        result = "/Img/" + dt + "/" + filenameImage;
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                //}
                //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
            //return response;
        }

        public JsonViewModel<string> ValidateCuaHangDK(CuaHangDangkyModel model)
        {
            var result = new JsonViewModel<string>() { ErrorCode=(int)Notification.ErrorCode.error};
            if (string.IsNullOrWhiteSpace(model.UserKT))
            {
                result.Data = "Vui lòng nhập tên đăng nhập.";
            }
            else if (string.IsNullOrWhiteSpace(model.MatKhauKT))
            {
                result.Data = "Vui lòng nhập mật khẩu.";
            }
            else if(_CuaHangDangKyService.checkPhoneExist(model.SoDienThoai))
            {
                result.Data = "Số điện thoại đã được đăng ký, vui lòng kiểm tra lại.";
            }
             else if (_CuaHangDangKyService.checkEmailExist(model.Email))
            {
                result.Data = "Địa chỉ email đã được đăng ký, vui lòng kiểm tra lại.";
            }
            else if (_CuaHangDangKyService.checkSubdomainExist(model.SubDomain))
            {
                result.Data = "Địa chỉ tài khoản Open24 đã được đăng ký, vui lòng kiểm tra lại.";
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }
        public JsonViewModel<string> ValidateCuaHangDKv2(CuaHangDangkyModel model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.error };
            if (string.IsNullOrWhiteSpace(model.UserKT))
            {
                result.Data = "Vui lòng nhập tên đăng nhập.";
            }
            else if (string.IsNullOrWhiteSpace(model.MatKhauKT))
            {
                result.Data = "Vui lòng nhập mật khẩu.";
            }
           
            else if (_CuaHangDangKyService.checkSubdomainExist(model.SubDomain))
            {
                result.Data = "Địa chỉ tài khoản Open24 đã được đăng ký, vui lòng kiểm tra lại.";
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }
        public JsonViewModel<string> ValidateCuaHangDKv1(CuaHangDangkyModel model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.error };
             if (_CuaHangDangKyService.checkPhoneExist(model.SoDienThoai))
            {
                result.Data = "Số điện thoại đã được đăng ký, vui lòng kiểm tra lại.";
            }
            else if (_CuaHangDangKyService.checkEmailExist(model.Email))
            {
                result.Data = "Địa chỉ email đã được đăng ký, vui lòng kiểm tra lại.";
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult DangKySuDung([FromBody]JObject model)
        {
            CuaHangDangkyModel obj = model.ToObject<CuaHangDangkyModel>();
            try
            {
                var nganhNgheKD = new NganhNgheKinhDoanhService().GetByMa(obj.MaNganhKinhDoanh);
                if(nganhNgheKD!=null || obj.ID_NganhKinhDoanh!=null){
                    var validate = ValidateCuaHangDK(obj);
                    if (validate.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        Random random = new Random();
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        string makichhoat = new string(Enumerable.Repeat(chars, 6)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
                        CuaHangDangKy item = new CuaHangDangKy();
                        item.SoDienThoai = obj.SoDienThoai;
                        item.SubDomain = obj.SubDomain;
                        item.TenCuaHang = obj.TenCuaHang;
                        //item.DiaChi = obj.DiaChi;
                        item.Email = obj.Email;
                        item.ID_NganhKinhDoanh = obj.ID_NganhKinhDoanh != null ? obj.ID_NganhKinhDoanh : nganhNgheKD.ID;
                        item.HoTen = obj.HoTen??obj.SoDienThoai;
                        item.NgayTao = DateTime.Now;
                        item.UserKT = obj.UserKT;
                        item.KhuVuc_DK = obj.KhuVuc_DK;
                        item.DiaChiIP_DK = obj.DiaChiIP_DK;
                        item.HeDieuHanh_DK = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                        item.ThietBi_DK = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                        item.TrinhDuyet_DK ="Other";
                        if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                        {
                            item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[3].ToString();
                        }
                        else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                        {
                            item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[6].ToString();
                        }
                        else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                        {
                            item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[4].ToString();
                        }
                        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                        byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(obj.MatKhauKT);
                        byte[] byteHash = md5.ComputeHash(byteValue);
                        string strPassWordConnect = Convert.ToBase64String(byteHash);

                        item.MatKhauKT = strPassWordConnect;
                        item.MaKichHoat = makichhoat;
                        item.HanSuDung = DateTime.Now.AddDays(15);
                        item.TrangThai = true;
                        item.version = (int)Notification.VersionStore.dungthu;
                        item.ID_GoiDichVu = KeyGoiDichVu.TieuChuan.ToString() ;
                        item.IsCreateDatabase = false;
                        string checkdangkysudung = M_News_Post.AddNewCuaHangDangKy(item);
                        if (checkdangkysudung == "")
                        {
                            item.MatKhauKT = obj.MatKhauKT;
                            Open24.App_Start.App_API.MailHelper.SendMail(item);
                            Open24.App_Start.App_API.MailHelper.SendMailToDangKy(item);
                            return RetunJsonAction<string>(true, string.Empty, obj.SubDomain);
                        }
                        else
                        {
                            return ActionFalseNotData("Đã xảy ra lỗi, vui lòng thử lại sau.");
                        }
                    }
                    else
                    {
                        return ActionFalseNotData(validate.Data);
                    }
                }
                else
                {
                    return ActionFalseNotData("Ngành nghề không được tìm thấy, vui lòng kiểm tra lại.");
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
            //return Ok(data);
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult CheckPhone(string id)
        {
            News_UserService _News_UserService = new News_UserService();
            string name = _News_UserService.GetNameByPhone(id);
            if(name == "")
            {
                return ActionFalseNotData("Số điện thoại không đúng");
            }
            else
            {
                return ActionTrueNotData(name);
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult DangKySuDungv1([FromBody]JObject model)
        {
            CuaHangDangkyModel obj = model.ToObject<CuaHangDangkyModel>();
            try
            {
                var nganhNgheKD = new NganhNgheKinhDoanhService().GetByMa(obj.MaNganhKinhDoanh);
                if (nganhNgheKD != null || obj.ID_NganhKinhDoanh != null)
                {
                    var validate = ValidateCuaHangDKv1(obj);
                    if (validate.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        Random random = new Random();
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        string makichhoat = new string(Enumerable.Repeat(chars, 6)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
                        CuaHangDangKy item = new CuaHangDangKy();
                        item.SoDienThoai = obj.SoDienThoai;
                        item.Email = obj.Email;
                        item.ID_NganhKinhDoanh = obj.ID_NganhKinhDoanh != null ? obj.ID_NganhKinhDoanh : nganhNgheKD.ID;
                        item.HoTen = obj.HoTen==null || obj.HoTen=="" || obj.HoTen.Trim() == "" ? obj.SoDienThoai:obj.HoTen;
                        item.NgayTao = DateTime.Now;
                        item.TenCuaHang = " ";
                        item.KhuVuc_DK = obj.KhuVuc_DK;
                        item.DiaChiIP_DK = obj.DiaChiIP_DK;
                        item.HeDieuHanh_DK = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                        item.ThietBi_DK = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                        item.TrinhDuyet_DK = "Other";
                        if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                        {
                            item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[3].ToString();
                        }
                        else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                        {
                            item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[6].ToString();
                        }
                        else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                        {
                            item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[4].ToString();
                        }
                      
                        item.MaKichHoat = makichhoat;
                        item.HanSuDung = DateTime.Now.AddDays(15);
                        item.TrangThai = true;
                        item.version = (int)Notification.VersionStore.chuadangky;
                        item.ID_GoiDichVu = KeyGoiDichVu.TieuChuan.ToString();
                        item.IsCreateDatabase = false;
                        string checkdangkysudung = M_News_Post.UpdateDangKyV1(item);
                        if (string.IsNullOrWhiteSpace(checkdangkysudung))
                        {
                           
                            return RetunJsonAction<string>(true, string.Empty,string.Empty);
                        }
                        else
                        {
                            return ActionFalseNotData(checkdangkysudung);
                        }
                    }
                    else
                    {
                        return ActionFalseNotData(validate.Data);
                    }
                }
                else
                {
                    return ActionFalseNotData("Ngành nghề không được tìm thấy, vui lòng kiểm tra lại.");
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
            //return Ok(data);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult DangKySuDungv2([FromBody]JObject model)
        {
            CuaHangDangkyModel obj = model.ToObject<CuaHangDangkyModel>();
            try
            {
               var nganhNgheKD = new NganhNgheKinhDoanhService().GetByMa(obj.MaNganhKinhDoanh);
               if (nganhNgheKD != null || obj.ID_NganhKinhDoanh != null)            
                    {
                        var validate = ValidateCuaHangDKv2(obj);
                        if (validate.ErrorCode == (int)Notification.ErrorCode.success)
                        {
                            Random random = new Random();
                            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            string makichhoat = new string(Enumerable.Repeat(chars, 6)
                                .Select(s => s[random.Next(s.Length)]).ToArray());
                            CuaHangDangKy item = new CuaHangDangKy();
                            item.SoDienThoai = obj.SoDienThoai;
                            item.SubDomain = obj.SubDomain;
                            item.TenCuaHang = obj.TenCuaHang;
                            //item.DiaChi = obj.DiaChi;
                            item.Email = obj.Email;
                            item.ID_NganhKinhDoanh = obj.ID_NganhKinhDoanh != null ? obj.ID_NganhKinhDoanh : nganhNgheKD.ID;
                            item.HoTen = obj.HoTen ?? obj.SoDienThoai;
                            item.NgayTao = DateTime.Now;
                            item.UserKT = obj.UserKT;
                            item.KhuVuc_DK = obj.KhuVuc_DK;
                            item.DiaChiIP_DK = obj.DiaChiIP_DK;
                            item.HeDieuHanh_DK = StaticVariable.CheckSystem(Request.Headers.UserAgent.ToString());
                            item.ThietBi_DK = StaticVariable.GetNameDeviceType(Request.Headers.UserAgent.ToString());
                            item.TrinhDuyet_DK = "Other";
                            if (Request.Headers.UserAgent.ToString().Contains("Firefox"))
                            {
                                item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[3].ToString();
                            }
                            else if (Request.Headers.UserAgent.ToString().Contains("Edge"))
                            {
                                item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[6].ToString();
                            }
                            else if (Request.Headers.UserAgent.ToString().Contains("Chrome") || Request.Headers.UserAgent.ToString().Contains("coc_coc_browser"))
                            {
                                item.TrinhDuyet_DK = Request.Headers.UserAgent.ToArray()[4].ToString();
                            }
                            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(obj.MatKhauKT);
                            byte[] byteHash = md5.ComputeHash(byteValue);
                            string strPassWordConnect = Convert.ToBase64String(byteHash);

                            item.MatKhauKT = strPassWordConnect;
                            item.MaKichHoat = makichhoat;
                            item.HanSuDung = DateTime.Now.AddDays(7);
                            item.TrangThai = true;
                            item.version = (int)Notification.VersionStore.dungthu;
                            item.ID_GoiDichVu = KeyGoiDichVu.TieuChuan.ToString();
                            item.IsCreateDatabase = false;
                            string checkdangkysudung = M_News_Post.UpdateDangKyV2(item);
                            if (string.IsNullOrWhiteSpace(checkdangkysudung ))
                            {
                            
                            _CuaHangDangKyService.InsertLichSuGiaHan(item.SoDienThoai, obj.DienThoaiNhanVien, obj.TenNhanVien);
                            var modelsendmail = _CuaHangDangKyService.Query.FirstOrDefault(o => o.SoDienThoai.Equals(item.SoDienThoai));
                                if (modelsendmail != null)
                                {
                                    modelsendmail.MatKhauKT = obj.MatKhauKT;
                                    Open24.App_Start.App_API.MailHelper.SendMail(modelsendmail);
                                    Open24.App_Start.App_API.MailHelper.SendMailToDangKy(modelsendmail);
                                }
                                return RetunJsonAction<string>(true, string.Empty, obj.SubDomain);
                            }
                            else
                            {
                                return ActionFalseNotData(checkdangkysudung);
                            }
                        }
                        else
                        {
                            return ActionFalseNotData(validate.Data);
                        }
                    }
                else
                {
                    return ActionFalseNotData("Ngành nghề không được tìm thấy, vui lòng kiểm tra lại.");
                }

            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult CheckDangKySuDung([FromBody]JObject model)
        {
            var data = "";
            CuaHangDangKy obj = model.ToObject<CuaHangDangKy>();
            try
            {
                Regex regex = new Regex(@"^(?!411|911)\d{10,12}$");
                Match phone = regex.Match(obj.SoDienThoai);
                Regex spechar = new Regex(@"^[a-zA-Z0-9 ]*$");
                //Regex spechar = new Regex(@"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");
                //Match subdomain = regex.Match(obj.SubDomain);
                //var email = new System.Net.Mail.MailAddress(obj.Email);
                Regex regexe = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match email = regexe.Match(obj.Email);

                CuaHangDangKy objCheck_SDT = M_DangKySuDung.Get(p => p.SoDienThoai.Trim().ToLower() == obj.SoDienThoai.Trim().ToLower());
                if (obj.SoDienThoai == "" || obj.SoDienThoai == null || objCheck_SDT != null || !phone.Success)
                {
                    data = "sdt";
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, data));
                }
                if (obj.TenCuaHang == null || obj.TenCuaHang == "")
                {
                    data = "tencuahang";
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, data));
                }
                if (obj.DiaChi == null || obj.DiaChi == "")
                {
                    data = "diachi";
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, data));
                }
                if (obj.UserKT == null || obj.UserKT == "")
                {
                    data = "user";
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, data));
                }
                CuaHangDangKy objCheck_TenMien = M_DangKySuDung.Get(p => p.SubDomain.Trim().ToLower() == obj.SubDomain.Trim().ToLower());
                if (obj.SubDomain == "" || obj.SubDomain == null || objCheck_TenMien != null || !spechar.IsMatch(obj.SubDomain))
                {
                    data = "tenmien";
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, data));
                }
                if (obj.MatKhauKT == null || obj.MatKhauKT == "")
                {
                    data = "pass";
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, data));
                }
                if (obj.Email != "")
                {
                    CuaHangDangKy objCheck_Email = M_DangKySuDung.Get(p => p.Email.Trim().ToLower() == obj.Email.Trim().ToLower());
                    if (objCheck_Email != null || !email.Success)
                    {
                        data = "email";
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, data));
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, data));
        }
        
        [HttpPost]
        public IHttpActionResult ThemLienHe([FromBody]JObject model)
        {
            string data = "";
            try
            {
                Contact obj = model.ToObject<Contact>();
                Contact item = new Contact { };
                item.ID = obj.ID;
                item.FullName = obj.FullName;
                item.Email = obj.Email;
                item.Phone = obj.Phone;
                item.Address = obj.Address;
                item.Note = obj.Note;
                item.CreateDate = DateTime.Now;
                item.Type = (int)Notification.TypeContact.lienhe;
                item.Status = (int)Notification.StatusContact.Moi;
                string body = "<h3> Thông tin khách hàng liên hệ với Open24</h3><br>"
                         + "<span>Họ tên: " + item.FullName + "</span><br>"
                         + "<span>Số điện thoại: " + item.Phone + "</span><br>"
                         + "<span>Email: " + item.Email + "</span><br>"
                         + "<span>Địa chỉ: " + item.Address + "</span><br><br>"
                         + "<span style='text-align: center'>--- Nội dung liên hệ --- </span>"
                         + "<p>  " + item.Note + "</p>";
                MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(), "[Phản hồi Open24] KH: " + item.FullName + " gửi liên hệ", body);
                data = M_News_Post.InsertContact(item);
            }
            catch (Exception ex)
            {
                data = ex.ToString();
            }

            return Ok(data);
        }
        [HttpPost]
        public IHttpActionResult ThemLienHeNew(Contact obj)
        {
            string data = "";
            try
            {
                Contact item = new Contact { };
                item.ID = obj.ID;
                item.FullName = obj.FullName;
                item.Email = obj.Email;
                item.Phone = obj.Phone;
                item.Address = obj.Address;
                item.Note = obj.Note;
                item.CreateDate = DateTime.Now;
                item.Type = (int)Notification.TypeContact.lienhe;
                item.Status = (int)Notification.StatusContact.Moi;
                string body = "<h3> Thông tin khách hàng liên hệ với Open24</h3><br>"
                         + "<span>Họ tên: " + item.FullName + "</span><br>"
                         + "<span>Số điện thoại: " + item.Phone + "</span><br>"
                         + "<span>Email: " + item.Email + "</span><br>"
                         + "<span>Địa chỉ: " + item.Address + "</span><br><br>"
                         + "<span style='text-align: center'>--- Nội dung liên hệ --- </span>"
                         + "<p>  " + item.Note + "</p>";
                MailHellper.SendThreadEmail(WebConfigurationManager.AppSettings["SPGmail"].ToString(), "[Phản hồi Open24] KH: " + item.FullName + " gửi liên hệ", body);
                data = M_News_Post.InsertContact(item);
            }
            catch 
            {
                return ActionFalseNotData("Đã xảy ra lỗi, vui lòng thử lại sau");
            }

            return InsertSuccess();
        }
        #endregion

        #region select
        public IHttpActionResult GetAllArticle()
        {
            List<m_Article> list = new List<m_Article>();
            list = M_News_Post.GetAllArticle().OrderByDescending(p => p.CreateDate).ToList();

            if (list != null)
            {
                int countItem = list.Count();
                var view = new DataGridView()
                {
                    Page = 1,
                    PageCount = (int)Math.Ceiling((double)countItem / Notification.PageDefault),
                    Data = list.Take(Notification.PageDefault).ToList(),
                };
                if (view.PageCount == 0)
                {
                    view.PageCount = 1;
                }
                view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, countItem);
                //list = list.Skip(currentPage * pageSize).Take(pageSize).ToList();
                return Ok(view);
            }
            return NotFound();
        }
        [HttpPost]
        public IHttpActionResult SearchCustomGrid([FromBody]JObject model)
        {
            DataGridView view = model.ToObject<DataGridView>();

           var  list = M_News_Post.GetDataForSearch(view.Search);

            if (view.Sort == (int)GridPagedingHellper.GridSort.SortUp)
            {

                switch (view.Columname)
                {
                    case (int)GridPagedingHellper.columtablePot.Category:
                        list = list.OrderBy(o => o.CategoryName);
                        break;
                    case (int)GridPagedingHellper.columtablePot.creatby:
                        list = list.OrderBy(o => o.UserName);
                        break;
                    case (int)GridPagedingHellper.columtablePot.creatdate:
                        list = list.OrderBy(o => o.CreateDate);
                        break;
                    case (int)GridPagedingHellper.columtablePot.view:
                        list = list.OrderBy(o => o.View);
                        break;
                    default:
                        list = list.OrderBy(o => o.Title);
                        break;

                }
            }
            else
            {
                switch (view.Columname)
                {
                    case (int)GridPagedingHellper.columtablePot.Category:
                        list = list.OrderByDescending(o => o.CategoryName);
                        break;
                    case (int)GridPagedingHellper.columtablePot.creatby:
                        list = list.OrderByDescending(o => o.UserName);
                        break;
                    case (int)GridPagedingHellper.columtablePot.creatdate:
                        list = list.OrderByDescending(o => o.CreateDate);
                        break;
                    case (int)GridPagedingHellper.columtablePot.view:
                        list = list.OrderByDescending(o => o.View);
                        break;
                    default:
                        list = list.OrderByDescending(o => o.Title);
                        break;

                }
            }

            if (list != null)
            {
                int countItem = list.Count();
                view.PageCount = (int)Math.Ceiling((double)countItem / view.Limit);
                if (view.PageCount == 0 || view.PageCount == 1)
                {
                    view.PageCount = 1;
                    view.Page = 1;
                    view.Data = list.ToList();
                }
                else
                {
                    view.Data = list.Skip(view.Limit * (view.Page - 1)).Take(view.Limit).ToList();
                }
                view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, countItem);

                return Ok(view);
            }
            return NotFound();
        }

        public IHttpActionResult GetAllArticleNews(int currentPage, int pageSize)
        {
            IEnumerable<m_Article> list = new List<m_Article>();
            list = M_News_Post.GetArticleNews().OrderByDescending(p => p.CreateDate).AsEnumerable();
            if (list != null)
            {
                list = list.Skip(currentPage * pageSize).Take(pageSize).AsEnumerable();
                return Ok(list);

                //return NotFound();
            }
            return NotFound();
        }

        public IHttpActionResult GetAllArticleNewsHome()
        {
            if (!CacheHellper.IsSet(CacheKey.News_Home_Slider))
            {
                List<m_Article> list = new List<m_Article>();
                list = M_News_Post.GetArticleNews().OrderByDescending(p => p.CreateDate).Take(6).ToList().Select(o=>new m_Article
                {
                    ID = o.ID,
                    Title = o.Title,
                    Summary = getSummary(o.Summary),
                    Image = o.Image,
                    CategoryID = o.CategoryID,
                    CreateDate = o.CreateDate,
                    Url = o.Url

                }).ToList();
                CacheHellper.Set(CacheKey.News_Home_Slider, list);
                return Ok(list);
            }
            else
                return Ok(CacheHellper.Get(CacheKey.News_Home_Slider));
        }
        public string getSummary(string values)
        {
            if (string.IsNullOrWhiteSpace(values))
            {
                return string.Empty;
            }
            else if(values.Length<101)
            {
                return values;
            }
            else
            {
                var result = values.Substring(100, values.Length- 100);
                int visit = result.IndexOf(" ", 0);
                return values.Substring(0, 100 + visit)+"...";
            }
        }
        public IHttpActionResult GetNextArticleNews()
        {
            List<m_Article> list = new List<m_Article>();
            list = M_News_Post.GetNextArticleNews().ToList();
            if (list != null)
            {
                return Ok(list);
                //return NotFound();
            }
            return NotFound();
        }

        public IHttpActionResult GetArticleNewsDetail(long id)
        {
            List<m_ArticleDetailNews> list = new List<m_ArticleDetailNews>();
            list = M_News_Post.GetArticleNewsDetail(id).OrderByDescending(p => p.CreateDate).ToList();

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        public PageListDTO GetPageCountArticle(float pageSize)
        {
            var totalRecords = 0;
            var data = M_News_Post.GetAllArticleNews();
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

        public IHttpActionResult GetCateIDArticleforUpdate(long id)
        {
            List<m_ArticleAll> list = new List<m_ArticleAll>();

            list = M_News_Post.getCateIDArticleforUpdate(id).ToList();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        public IHttpActionResult GetAllCategories(int cateTypeID)
        {
            List<m_Categories> list = new List<m_Categories>();
            list = M_News_Post.getallCategories(cateTypeID).Select(p => new m_Categories
            {
                ID = p.ID,
                Name = p.Name,
                CategoryTypeID = p.CategoryTypeID
            }).ToList();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        public IHttpActionResult GetAllCategoryType(int postionID)
        {
            List<News_CategoriesType> list = new List<News_CategoriesType>();
            list = M_News_Post.getallCategoryType(postionID).ToList();
            if (list == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(list);
            }
        }

        public IHttpActionResult GetNganhNgheKinhDoanh()
        {
            List<m_NganhNgheKinhDoanh> list = new List<m_NganhNgheKinhDoanh>();
            if (list != null)
            {
                list = M_News_Post.getNganhNgheKinhDoanh().Select(p => new m_NganhNgheKinhDoanh
                {
                    ID = p.ID,
                    MaNganhNghe = p.MaNganhNghe,
                    TenNganhNghe = p.TenNganhNghe
                }).ToList();
                return Ok(list);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult CheckDangKy()
        {
            List<m_CuaHangDangKy> list = new List<m_CuaHangDangKy>();
            list = M_News_Post.getCuaHangDangKy().Select(p => new m_CuaHangDangKy
            {
                SoDienThoai = p.SoDienThoai,
                SubDomain = p.SubDomain,
                TenTaiKhoan = p.UserKT
            }).ToList();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        public IHttpActionResult GetListCateGroup()
        {
            List<News_Categories> list = new List<News_Categories>();
            list = M_News_Post.getListCateGroup().ToList();
            if (list == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(list);
            }
        }

        public IHttpActionResult GetCateGroup()
        {
            List<m_Category> list = new List<m_Category>();
            list = M_News_Post.getCateGroup().OrderByDescending(p => p.ID).ToList();

            //list = M_News_Post.getCateGroup().SelectMany(p => p.nodes,(parent, child) => new { parent.text, child.parentID }).ToList();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        #endregion

        #region delete
        [HttpGet]
        public IHttpActionResult DeleteArticle(long id)
        {
            CacheHellper.Invalidate(CacheKey.News_Home_Slider);
            CacheHellper.Invalidate(CacheKey.News_NewDate);
            var idDel = M_News_Post.deleteArticle(id);
            if (idDel.ErrorCode != (int)Notification.ErrorCode.success)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, idDel.Data));
            else
            {
                EventUpdateCache.DeleteFileImeagesCache(HttpContext.Current.Server.MapPath("/Appcache/manifest.appcache"), idDel.Data);
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion

    }

    #region model
    //public class m_Article
    //{
    //    public string Title { get; set; }
    //    public string Summary { get; set; }
    //    public string CategoryID { get; set; }
    //}


    public class m_Categories
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? CategoryTypeID { get; set; }
    }

    #endregion
}
