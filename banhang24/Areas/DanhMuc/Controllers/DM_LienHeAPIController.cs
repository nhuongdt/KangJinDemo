using System;
using System.Linq;
using System.Web.Http;
using System.IO;
using Model;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using libDM_DoiTuong;
using libQuy_HoaDon;
using System.Data;
using System.Web;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_LienHeAPIController : ApiController
    {

        #region Get
        [HttpGet]
        public IHttpActionResult GetInforContact_byID(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);
                    var data = classLienHe.GetInforContact_byID(id);
                    return Json(new { res = true, data });
                }
                catch (Exception ex)
                {
                    return Json(new { res = false, mes = ex.InnerException+ ex.Message });
                }
            }
        }
        [HttpGet]
        public IEnumerable<Object> GetImages_byIDLienHe(Guid id)
        {
            IEnumerable<Object> result = null;
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);
                    var data = classLienHe.Gets_DM_LienHeAnh(x => x.ID_LienHe == id);
                    if (data != null)
                    {
                        result = data.Select(x => new
                        {
                            ID = x.ID,
                            URLAnh = x.URLAnh,
                            SoThuTu = x.SoThuTu,
                        });
                    }
                }
            }
            catch (Exception e)
            {
                CookieStore.WriteLog("DM_LienHeAPI_GetImages_byIDLienHe: " + e.InnerException + e.Message);
            }
            return result;
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<SP_DM_LienHe> GetAllLienHe_byLoaiDoiTuong(int loaiDoiTuong)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db != null)
            {
                var tbl = from lienhe in db.DM_LienHe
                          join doituong in db.DM_DoiTuong on lienhe.ID_DoiTuong equals doituong.ID
                          where doituong.LoaiDoiTuong == loaiDoiTuong
                          select new SP_DM_LienHe()
                          {
                              ID = lienhe.ID,
                              ID_DoiTuong = lienhe.ID_DoiTuong,
                              MaLienHe = lienhe.MaLienHe,
                              TenLienHe = lienhe.TenLienHe,
                              SoDienThoai = lienhe.SoDienThoai,
                          };
                return tbl.ToList();
            }
            else
            {
                return new List<SP_DM_LienHe>();
            }
        }

        public List<SP_DM_LienHe> GetAllLienHe()
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db != null)
            {
                var tbl = from lienhe in db.DM_LienHe
                          select new SP_DM_LienHe()
                          {
                              ID = lienhe.ID,
                              ID_DoiTuong = lienhe.ID_DoiTuong,
                              MaLienHe = lienhe.MaLienHe,
                              TenLienHe = lienhe.TenLienHe,
                              SoDienThoai = lienhe.SoDienThoai,
                          };
                return tbl.ToList();
            }
            else
            {
                return new List<SP_DM_LienHe>();
            }
        }

        public IQueryable<SP_DM_LienHe> GetDoiTuongByID_LienHe(Guid idlienhe)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db != null)
            {
                var tbl = from lienhe in db.DM_LienHe
                          join doituong in db.DM_DoiTuong on lienhe.ID_DoiTuong equals doituong.ID
                          where lienhe.ID == idlienhe
                          select new SP_DM_LienHe
                          {
                              ID = lienhe.ID,
                              TenDoiTuong = doituong.TenDoiTuong,
                              ID_DoiTuong = lienhe.ID_DoiTuong,
                              MaLienHe = lienhe.MaLienHe,
                              TenLienHe = lienhe.TenLienHe,
                          };
                if (tbl.Count() > 0)
                {
                    return tbl;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public IQueryable<SP_DM_LienHe> GetDM_LienHeByIDDoiTuong(Guid iddoituong)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db != null)
            {
                var tbl = from lienhe in db.DM_LienHe
                          join doituong in db.DM_DoiTuong on lienhe.ID_DoiTuong equals doituong.ID
                          where lienhe.ID_DoiTuong == iddoituong
                          select new SP_DM_LienHe
                          {
                              ID = lienhe.ID,
                              ID_DoiTuong = lienhe.ID_DoiTuong,
                              MaLienHe = lienhe.MaLienHe,
                              TenLienHe = lienhe.TenLienHe,
                          };
                if (tbl.Count() > 0)
                {
                    return tbl;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Object> GetAllImg_DoiTuong()
        {
            IEnumerable<Object> result = null;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_DoiTuong_Anh classDoiTuongAnh = new ClassDM_DoiTuong_Anh(db);
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

        [HttpGet]
        public IEnumerable<SP_DM_LienHe> GetAllUserContact_byWhere_FilterNhanVien(string txtSearch, string idManagers = null, string nguoitao = null)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);
                    return classLienHe.SP_GetAllUserContact_byWhere_FilterNhanVien(txtSearch, idManagers, nguoitao);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("SP_GetAllUserContact_byWhere_FilterNhanVien " + ex.InnerException + ex.Message);
                return null;
            }
        }

        #region Excel
        [HttpGet]
        public void ExportExcel_ListLienHe(string txtSearch, string idManagers, string columnsHide)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);
                List<SP_DM_LienHe> lst = classLienHe.SP_GetAllUserContact_byWhere_FilterNhanVien(txtSearch, idManagers).OrderBy(x => x.MaLienHe).ToList();

                Class_officeDocument classOffice = new Class_officeDocument(db);
                DataTable excel = classOffice.ToDataTable<SP_DM_LienHe>(lst);
                excel.Columns.Remove("ID");
                excel.Columns.Remove("ID_DoiTuong");
                excel.Columns.Remove("XungHo");
                excel.Columns.Remove("NgayTao");
                excel.Columns.Remove("NguoiTao");
                excel.Columns.Remove("ID_TinhThanh");
                excel.Columns.Remove("ID_QuanHuyen");
                excel.Columns.Remove("LoaiLienHe");
                excel.Columns.Remove("ID_NhanVienPhuTrach");
                excel.Columns.Remove("DienThoaiCoDinh");

                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhSachNguoiLienHe.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhSachNguoiLienHe.xlsx");
                classOffice.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, false, columnsHide);
                HttpResponse Response = HttpContext.Current.Response;
                classOffice.downloadFile(fileSave);
            }
        }

        [HttpPost]
        public IHttpActionResult ImportExcel_DMLienHe(string nguoitao)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = classOffice.Check_FormatTypeExcel(excelstream, "LIÊN HỆ");
                            if (str == null)
                            {
                                abc = classOffice.CheckImport_DMLienHe(excelstream, nguoitao);
                            }
                            else
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, str));
                            }
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }

        [HttpPost]
        public IHttpActionResult ImportDMLienHe_PassError(string RowsError, string nguoitao)
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            classOffice.ImportDMLienHe_PassError(excelstream, RowsError, nguoitao);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        #endregion

        #endregion

        [System.Web.Http.HttpPost]
        public IHttpActionResult AddDM_LienHe(DM_LienHe obj)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);

                    string sCode = string.Empty;
                    if (string.IsNullOrEmpty(obj.MaLienHe))
                    {
                        sCode = classLienHe.SP_GetautoCode();
                    }
                    else
                    {
                        sCode = obj.MaLienHe.Trim().ToUpper();
                    }

                    DM_LienHe objNew = obj;
                    objNew.ID = Guid.NewGuid();
                    objNew.MaLienHe = sCode;
                    objNew.NgayTao = DateTime.Now;
                    db.DM_LienHe.Add(objNew);
                    db.SaveChanges();

                    return Json(new { res = true, data = new { objNew.ID, objNew.MaLienHe } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException });
            }
        }

        [System.Web.Http.AcceptVerbs("PUT", "POST")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateDM_LienHe(DM_LienHe data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);

                    if (string.IsNullOrEmpty(data.MaLienHe))
                    {
                        data.MaLienHe = classLienHe.SP_GetautoCode();
                    }
                    classLienHe.Update(data);
                    return Json(new { res = true, data = new { data.ID, data.MaLienHe } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.Message + ex.InnerException });
            }
        }

        [HttpPost]
        public IHttpActionResult ImageUpload(Guid id, string pathFolder)
        {
            // path file: ImageUpload/pathFolder/filename
            var pathFile = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);
                    string fullPathFolder = "ImageUpload/" + pathFolder;
                    var count = classLienHe.Get_ImgUserContact(p => p.ID_LienHe == id).Count();
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
                        DM_LienHe_Anh objAnh = new DM_LienHe_Anh();
                        objAnh.ID = Guid.NewGuid();
                        objAnh.ID_LienHe = id;
                        objAnh.SoThuTu = count + 1;
                        objAnh.URLAnh = "/" + fullPathFolder + "/" + fileName; // path file
                        classLienHe.Add_Image(objAnh);
                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, pathFile));
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("DM_LienHeAPI_ImageUpload: " + ex.InnerException + ex.Message);
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, pathFile));
            }
        }

        [ResponseType(typeof(string))]
        [HttpDelete]
        public string DeleteDM_LienHe_Anh(Guid id)
        {
            SsoftvnContext db = SystemDBContext.GetDBContext();
            if (db == null)
            {
                return "Chưa kết nối DB";
            }
            else
            {
                DM_LienHe_Anh lst = db.DM_LienHe_Anh.Find(id);
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

                    db.DM_LienHe_Anh.Remove(lst);
                    db.SaveChanges();
                    List<DM_LienHe_Anh> lstanh = db.DM_LienHe_Anh.Where(p => p.ID_LienHe == lst.ID_LienHe).Where(p => p.SoThuTu > lst.SoThuTu).ToList();
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

        [System.Web.Http.HttpDelete, HttpPost, HttpGet]
        public IHttpActionResult DeleteDM_LienHe(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_LienHe classLienHe = new ClassDM_LienHe(db);
                string strDel = classLienHe.Delete(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
    }
}