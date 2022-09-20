using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Model;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using libDM_HangHoa;
using libDM_Kho;
using libDonViQuiDoi;
using libDM_DoiTuong;
using libDM_GiaBan;
using System.Data.Entity;
using System.Data;
using libQuy_HoaDon;
using System.Web;
using libHT_NguoiDung;
using System.IO;
using libDM_DonVi;
using Newtonsoft.Json;
using System.Data.SqlClient;
using libDM_NhomHangHoa;
using Model.Service.common;
using banhang24.Models;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;
using Model_banhang24vn.DAL;
using System.Web.Http.Results;
using banhang24.Compress;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_HangHoaAPIController : BaseApiController
    {
        //
        // GET: /DanhMuc/DM_HangHoaAPI/

        #region GET
        // GET: api/DanhMuc/DM_HangHoaAPI/GetDM_HangHoa
        public IQueryable<DM_HangHoa> GetDM_HangHoa()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.Gets(null);
            }
        }

        [HttpGet]
        public IHttpActionResult GetHangHoa(string id)
        {
            try
            {
                using (SsoftvnContext db = new SsoftvnContext(id))
                {
                    var _classDMHH = new ClassDM_HangHoa(db);
                    List<GetListHangHoaDatLichCheckinResult> result = new List<GetListHangHoaDatLichCheckinResult>();
                    result = _classDMHH.GetListHangHoaDatLich();
                    return ActionTrueData(result);
                }
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllDichVu()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var data = (from hh in db.DM_HangHoa
                                join qd in db.DonViQuiDois on hh.ID equals qd.ID_HangHoa
                                where hh.LaHangHoa == false && qd.Xoa == false && hh.TheoDoi == true
                                select new
                                {
                                    ID_DonViQuiDoi = qd.ID,
                                    MaHangHoa = qd.MaHangHoa,
                                    TenHangHoa = hh.TenHangHoa,
                                    Name = string.Concat(qd.MaHangHoa, " ", hh.TenHangHoa)
                                }).ToList();
                    return Json(new { res = true, data = data });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [HttpGet]
        public double GetTonKho_ofHangHoa(Guid? idDonVi, Guid idQuiDoi, Guid? idLoHang)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var lstTon = from tk in db.DM_HangHoa_TonKho
                                 where tk.ID_DonVi == idDonVi
                                 & tk.ID_DonViQuyDoi == idQuiDoi & tk.ID_LoHang == idLoHang
                                 select tk.TonKho;
                    if (lstTon != null && lstTon.Count() > 0)
                    {
                        return lstTon.FirstOrDefault();
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }
        [DeflateCompression]
        [HttpGet]
        public IHttpActionResult GetInforProduct_ByIDQuidoi(Guid idQuiDoi, Guid idChiNhanh, Guid? idLoHang = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDonViQuiDoi classQuiDoi = new classDonViQuiDoi(db);
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("IDQuiDoi", idQuiDoi));
                    lstParam.Add(new SqlParameter("ID_ChiNhanh", idChiNhanh));
                    lstParam.Add(new SqlParameter("ID_LoHang", idLoHang ?? (object)DBNull.Value));
                    var lst = db.Database.SqlQuery<DM_HangHoaSearch>("GetInforProduct_ByIDQuiDoi @IDQuiDoi, @ID_ChiNhanh, @ID_LoHang", lstParam.ToArray()).ToList();
                    if (lst.Count() > 0)
                    {
                        lst = lst.Select(p => new DM_HangHoaSearch()
                        {
                            ID_DonViQuiDoi = p.ID_DonViQuiDoi,
                            TyLeChuyenDoi = p.TyLeChuyenDoi,
                            ID = p.ID,
                            MaHangHoa = p.MaHangHoa,
                            QuyCach = p.QuyCach,
                            TenHangHoa = p.TenHangHoa,
                            ThuocTinh_GiaTri = p.ThuocTinh_GiaTri,
                            TenDonViTinh = p.TenDonViTinh,
                            QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                            LaHangHoa = p.LaHangHoa,
                            GiaVon = p.GiaVon,
                            GiaBan = p.GiaBan,
                            GiaNhap = p.GiaNhap,
                            TonKho = p.TonKho,
                            SrcImage = p.SrcImage,
                            ID_LoHang = p.ID_LoHang,
                            MaLoHang = p.MaLoHang,
                            NgaySanXuat = p.NgaySanXuat,
                            NgayHetHan = p.NgayHetHan,
                            DonViTinh = classQuiDoi.Gets(ct => ct.ID_HangHoa == p.ID && ct.Xoa != true).Select(x => new DonViTinh
                            {
                                ID_HangHoa = p.ID,
                                TenDonViTinh = x.TenDonViTinh,
                                ID_DonViQuiDoi = x.ID,
                                QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                                Xoa = x.Xoa,
                                TyLeChuyenDoi = x.TyLeChuyenDoi
                            }).ToList()
                        }).ToList();
                        return Json(new { res = true, data = lst });
                    }
                    else
                    {
                        return Json(new { res = false, mes = "Data null" });
                    }
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        //Thuộc tính hh
        public List<DM_ThuocTinh> GetallThuocTinh()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                var data = _classDMHH.GetAllThuocTinh(null);
                List<DM_ThuocTinh> lst = new List<DM_ThuocTinh>();
                foreach (var item in data)
                {
                    DM_ThuocTinh dto = new DM_ThuocTinh
                    {
                        ID = item.ID,
                        TenThuocTinh = item.TenThuocTinh
                    };
                    lst.Add(dto);
                }
                return lst;
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public static List<DM_NhomHangHoa> GetDM_NhomHangHoa()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_NhomHangHoa> lst = _classDMHH.getNhomHangHoa();
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

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public DM_HangHoaInMaVach GetHangHoaByID_BangGia(Guid idgiaban, Guid iddvqd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetHangHoaByID_BangGIa(idgiaban, iddvqd).FirstOrDefault();
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string InMaVachITextSharp(string tenhanghoa, string mahh, float giaban, bool ingia, bool inmahh, bool intenhh, bool intench, int sobanghi, int somavach)
        {
            return PrintBarcode3Tem(tenhanghoa, mahh, giaban, ingia, inmahh, intenhh, intench, sobanghi, somavach);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string InMaVachITextSharp2Tem(string tenhanghoa, string mahh, float giaban, bool ingia, bool inmahh, bool intenhh, bool intench, int sobanghi, int somavach)
        {
            return PrintBarcode2Tem(tenhanghoa, mahh, giaban, ingia, inmahh, intenhh, intench, sobanghi, somavach);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string InMaVachITextSharp1(string tenhanghoa, string mahh, float giaban, bool ingia, bool inmahh, bool intenhh, bool intench, int sobanghi, int somavach)
        {
            return PrintBarcode1(tenhanghoa, mahh, giaban, ingia, inmahh, intenhh, intench, sobanghi, somavach);
        }

        public List<libDM_HangHoa.List_TenDonViTinh> getdonviquidoibymahanghoa(string maHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<libDM_HangHoa.List_TenDonViTinh> lst = _classDMHH.getdonviquidoibymahanghoa(maHangHoa);
                return lst.ToList();
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImageUpload(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                var path = "";
                string result = "";
                try
                {

                    //if (HttpContext.Current.Request.Files.Count != 0)
                    //{
                    Guid idhanghoa = new Guid(id);
                    var count = _classDMHH.GetsAnh(p => p.ID_HangHoa == idhanghoa).Count();
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Current.Request.Files[i];
                        //var fileName = Path.GetFileName(file.FileName);
                        var filenameImage = Guid.NewGuid().ToString() + ".jpg";
                        //var filename = filenameImage.ToString();
                        string str = CookieStore.GetCookieAes("SubDomain");
                        DateTime time = DateTime.Now;
                        //var dt = time.ToString(format);
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/imageHH/" + str + "/" + id)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/imageHH/" + str + "/" + id));
                        }

                        path = Path.Combine(HttpContext.Current.Server.MapPath("~/imageHH/" + str + "/" + id), filenameImage);
                        file.SaveAs(path);
                        result = "/imageHH/" + str + "/" + id + "/" + filenameImage;
                        //Add db table Anh
                        DM_HangHoa_Anh objAnh = new DM_HangHoa_Anh();
                        objAnh.ID = Guid.NewGuid();
                        objAnh.ID_HangHoa = new Guid(id);
                        objAnh.SoThuTu = count + 1;
                        objAnh.URLAnh = result;
                        _classDMHH.Add_Image(objAnh);
                        var subDomain = CookieStore.GetCookieAes("SubDomain");
                        banhang24.AppCache.EventUpdateCache.AddFileImeagesCache(HttpContext.Current.Server.MapPath("/AppCache/CacheSubDomain/" + subDomain + "/manifest.appcache"), result);
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
        }

        [HttpPost]
        public IHttpActionResult UpdateAnhHangHoa([FromBody] List<string> files, Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    ClassDM_HangHoa classhanghoa = new ClassDM_HangHoa(db);
                    var count = classhanghoa.GetsAnh(p => p.ID_HangHoa == id).Count();
                    List<DM_HangHoa_Anh> lst = new List<DM_HangHoa_Anh>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        //Add db table Anh
                        DM_HangHoa_Anh objAnh = new DM_HangHoa_Anh();
                        objAnh.ID = Guid.NewGuid();
                        objAnh.ID_HangHoa = id;
                        objAnh.SoThuTu = count + 1 + i;
                        objAnh.URLAnh = files[i];
                        lst.Add(objAnh);
                    }
                    db.DM_HangHoa_Anh.AddRange(lst);
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return Ok("");
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult UpLoadFileCongViec(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var path = "";
                string result = "";
                var loaiHD = "CongViec";
                try
                {
                    Guid idcongviec = new Guid(id);
                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Current.Request.Files[i];
                        var fileName = Path.GetFileName(file.FileName);
                        //var filename = filenameImage.ToString();
                        string str = CookieStore.GetCookieAes("SubDomain");
                        DateTime time = DateTime.Now;
                        //var dt = time.ToString(format);
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/FileUpLoad/" + loaiHD + "/" + str + "/" + id)))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/FileUpLoad/" + loaiHD + "/" + str + "/" + id));
                        }

                        path = Path.Combine(HttpContext.Current.Server.MapPath("~/FileUpLoad/" + loaiHD + "/" + str + "/" + id), fileName);
                        file.SaveAs(path);
                        result = "/FileUpLoad/" + loaiHD + "/" + str + "/" + id + "/" + fileName;

                        ChamSocKhachHang cskh = db.ChamSocKhachHang.Find(idcongviec);
                        cskh.FileDinhKem = result;
                        db.Entry(cskh).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
            //return response;
        }

        public string PrintBarcode2Tem(string tenhanghoa, string mahh, float giaban, bool ingia, bool inmahh, bool intenhh, bool intench, int sobanghi, int somavach)
        {
            var pgSize = new iTextSharp.text.Rectangle(211f, 62.8f);
            var doc = new iTextSharp.text.Document(pgSize, 0, 0, 0, 0);
            //PdfPTable

            string str = CookieStore.GetCookieAes("SubDomain");
            string url = "/download/" + str;
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIAL.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(sobanghi);
                float[] widths = new float[] { 100, 100 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;

                //chỉ in mã vạch
                if (ingia == false && inmahh == false && intenhh == false && intench == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        code128.Font = null;
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 37;

                        PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                        cellTenHang.Border = 0;
                        cellTenHang.FixedHeight = 10;

                        table1.AddCell(cellTenHang);
                        table1.AddCell(cellBarcode);

                        PdfPCell cellrong = new PdfPCell(new Phrase(""));
                        cellrong.Border = 0;
                        cellrong.FixedHeight = 12;
                        table1.AddCell(cellrong);

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else if (ingia == false && inmahh == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        code128.Font = null;
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -10));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 29;

                        if (intench == true)
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 16;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 14;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;
                                table1.AddCell(cellTenHang);
                                table1.AddCell(cellBarcode);
                            }
                            else
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 24;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                table1.AddCell(cellBarcode);

                                PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                cellrongadd.Border = 0;
                                cellrongadd.FixedHeight = 6;
                                table1.AddCell(cellrongadd);
                            }
                        }
                        else
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 24;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;
                                table1.AddCell(cellTenHang);

                                table1.AddCell(cellBarcode);

                                PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                cellrongadd.Border = 0;
                                cellrongadd.FixedHeight = 6;
                                table1.AddCell(cellrongadd);
                            }
                        }

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else if (intenhh == false && intench == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        if (inmahh == false)
                        {
                            code128.Font = null;
                        }
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 37;

                        PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                        cellTenHang.Border = 0;
                        cellTenHang.FixedHeight = 10;

                        table1.AddCell(cellTenHang);
                        table1.AddCell(cellBarcode);

                        if (ingia == true)
                        {
                            Font fontextgia = new Font(bf, 8, Font.BOLD);
                            PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", giaban).Replace(".", ",") + "VNĐ", fontextgia));
                            cellGia.Border = 0;
                            cellGia.FixedHeight = 12;
                            cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            table1.AddCell(cellGia);
                        }
                        else
                        {
                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);
                        }

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        if (inmahh == false)
                        {
                            code128.Font = null;
                        }
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 29;

                        if (intench == true)
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 9;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 9;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;

                                table1.AddCell(cellTenHang);
                            }
                            else
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 18;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);
                            }
                        }
                        else
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 18;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;

                                table1.AddCell(cellTenHang);
                            }
                        }

                        table1.AddCell(cellBarcode);
                        if (ingia == true)
                        {
                            Font fontextgia = new Font(bf, 8, Font.BOLD);
                            PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", giaban).Replace(".", ",") + "VNĐ", fontextgia));
                            cellGia.Border = 0;
                            cellGia.FixedHeight = 12;
                            cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            table1.AddCell(cellGia);
                        }
                        else
                        {
                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);
                        }
                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }

                }
                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        public string PrintBarcode3Tem(string tenhanghoa, string mahh, float giaban, bool ingia, bool inmahh, bool intenhh, bool intench, int sobanghi, int somavach)
        {
            var pgSize = new iTextSharp.text.Rectangle(99 * sobanghi, 62);
            var doc = new iTextSharp.text.Document(pgSize, 0, 0, 0, 0);
            //PdfPTable

            string str = CookieStore.GetCookieAes("SubDomain");
            string url = "/download/" + str;
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                //url = createFolder_SubDoMain(url);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIAL.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(sobanghi);
                float[] widths = new float[] { 99, 99, 99 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;

                //chỉ in mã vạch
                if (ingia == false && inmahh == false && intenhh == false && intench == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        code128.Font = null;
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 37;

                        PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                        cellTenHang.Border = 0;
                        cellTenHang.FixedHeight = 10;

                        table1.AddCell(cellTenHang);
                        table1.AddCell(cellBarcode);

                        PdfPCell cellrong = new PdfPCell(new Phrase(""));
                        cellrong.Border = 0;
                        cellrong.FixedHeight = 12;
                        table1.AddCell(cellrong);

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else if (ingia == false && inmahh == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        code128.Font = null;
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -10));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 29;

                        if (intench == true)
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 16;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 14;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;
                                table1.AddCell(cellTenHang);
                                table1.AddCell(cellBarcode);
                            }
                            else
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 24;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                table1.AddCell(cellBarcode);

                                PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                cellrongadd.Border = 0;
                                cellrongadd.FixedHeight = 6;
                                table1.AddCell(cellrongadd);
                            }
                        }
                        else
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 24;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;
                                table1.AddCell(cellTenHang);

                                table1.AddCell(cellBarcode);

                                PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                cellrongadd.Border = 0;
                                cellrongadd.FixedHeight = 6;
                                table1.AddCell(cellrongadd);
                            }
                        }

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else if (intenhh == false && intench == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        if (inmahh == false)
                        {
                            code128.Font = null;
                        }
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 37;

                        PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                        cellTenHang.Border = 0;
                        cellTenHang.FixedHeight = 10;

                        table1.AddCell(cellTenHang);
                        table1.AddCell(cellBarcode);

                        if (ingia == true)
                        {
                            Font fontextgia = new Font(bf, 8, Font.BOLD);
                            PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", giaban).Replace(".", ",") + "VNĐ", fontextgia));
                            cellGia.Border = 0;
                            cellGia.FixedHeight = 12;
                            cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            table1.AddCell(cellGia);
                        }
                        else
                        {
                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);
                        }

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        if (inmahh == false)
                        {
                            code128.Font = null;
                        }
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(90);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -9, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 29;

                        if (intench == true)
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 9;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 9;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;

                                table1.AddCell(cellTenHang);
                            }
                            else
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 18;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);
                            }
                        }
                        else
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 18;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;

                                table1.AddCell(cellTenHang);
                            }
                        }

                        table1.AddCell(cellBarcode);
                        if (ingia == true)
                        {
                            Font fontextgia = new Font(bf, 8, Font.BOLD);
                            PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", giaban).Replace(".", ",") + "VNĐ", fontextgia));
                            cellGia.Border = 0;
                            cellGia.FixedHeight = 12;
                            cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            table1.AddCell(cellGia);
                        }
                        else
                        {
                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);
                        }
                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        //A4
        public string PrintBarcode1(string tenhanghoa, string mahh, float giaban, bool ingia, bool inmahh, bool intenhh, bool intench, int sobanghi, int somavach)
        {
            var doc = new Document(PageSize.A4, 5, 0, 35, 0);
            BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIAL.TTF", BaseFont.IDENTITY_H, true);
            Font fontext = new Font(bf, 7, Font.NORMAL);
            Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
            string str = CookieStore.GetCookieAes("SubDomain");
            string url = "/download/" + str;
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                PdfPTable table = new PdfPTable(sobanghi);
                float[] widths = new float[] { 107, 107, 107, 107, 107 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;

                //chỉ in mã vạch
                if (ingia == false && inmahh == false && intenhh == false && intench == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        code128.Font = null;
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(95);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -7, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 37;

                        PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                        cellTenHang.Border = 0;
                        cellTenHang.FixedHeight = 10;

                        table1.AddCell(cellTenHang);
                        table1.AddCell(cellBarcode);

                        PdfPCell cellrong = new PdfPCell(new Phrase(""));
                        cellrong.Border = 0;
                        cellrong.FixedHeight = 12;
                        table1.AddCell(cellrong);

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else if (ingia == false && inmahh == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        code128.Font = null;
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(95);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -7, -10));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 29;

                        if (intench == true)
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 16;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 14;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;
                                table1.AddCell(cellTenHang);
                                table1.AddCell(cellBarcode);
                            }
                            else
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 24;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                table1.AddCell(cellBarcode);

                                PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                cellrongadd.Border = 0;
                                cellrongadd.FixedHeight = 6;
                                table1.AddCell(cellrongadd);
                            }
                        }
                        else
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 24;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;
                                table1.AddCell(cellTenHang);

                                table1.AddCell(cellBarcode);

                                PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                cellrongadd.Border = 0;
                                cellrongadd.FixedHeight = 6;
                                table1.AddCell(cellrongadd);
                            }
                        }

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else if (intenhh == false && intench == false)
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        if (inmahh == false)
                        {
                            code128.Font = null;
                        }
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(95);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -7, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 37;

                        PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                        cellTenHang.Border = 0;
                        cellTenHang.FixedHeight = 10;

                        table1.AddCell(cellTenHang);
                        table1.AddCell(cellBarcode);

                        if (ingia == true)
                        {
                            Font fontextgia = new Font(bf, 8, Font.BOLD);
                            PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", giaban).Replace(".", ",") + "VNĐ", fontextgia));
                            cellGia.Border = 0;
                            cellGia.FixedHeight = 12;
                            cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            table1.AddCell(cellGia);
                        }
                        else
                        {
                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);
                        }

                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                else
                {
                    for (int i = 0; i < somavach; i++)
                    {
                        PdfPTable table1 = new PdfPTable(1);
                        PdfContentByte cb = writer.DirectContentUnder;
                        Barcode128 code128 = new Barcode128();
                        code128.Code = mahh;
                        code128.CodeType = Barcode.CODE128;
                        code128.Size = 7;
                        if (inmahh == false)
                        {
                            code128.Font = null;
                        }
                        Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                        imagecode128.ScaleAbsoluteWidth(95);
                        Phrase phrase = new Phrase();
                        phrase.Add(new Chunk(imagecode128, -7, -15));

                        PdfPCell cellBarcode = new PdfPCell();
                        cellBarcode.AddElement(phrase);
                        cellBarcode.Border = 0;
                        cellBarcode.HorizontalAlignment = 1;
                        cellBarcode.FixedHeight = 29;

                        if (intench == true)
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 9;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);

                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 9;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;

                                table1.AddCell(cellTenHang);
                            }
                            else
                            {
                                PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                cellTenCuaHang.Border = 0;
                                cellTenCuaHang.FixedHeight = 18;
                                cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenCuaHang.Padding = 0;
                                table1.AddCell(cellTenCuaHang);
                            }
                        }
                        else
                        {
                            if (intenhh == true)
                            {
                                PdfPCell cellTenHang = new PdfPCell(new Phrase(tenhanghoa, fontext));
                                cellTenHang.Border = 0;
                                cellTenHang.FixedHeight = 18;
                                cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                cellTenHang.Padding = 0;

                                table1.AddCell(cellTenHang);
                            }
                        }

                        table1.AddCell(cellBarcode);
                        if (ingia == true)
                        {
                            Font fontextgia = new Font(bf, 8, Font.BOLD);
                            PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", giaban).Replace(".", ",") + "VNĐ", fontextgia));
                            cellGia.Border = 0;
                            cellGia.FixedHeight = 12;
                            cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            table1.AddCell(cellGia);
                        }
                        else
                        {
                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);
                        }
                        Phrase phrasetable1 = new Phrase();
                        phrasetable1.Add(table1);
                        PdfPCell cell = new PdfPCell();
                        cell.AddElement(phrasetable1);
                        cell.FixedHeight = 59.5f;
                        cell.Border = 0;
                        cell.Padding = 0;
                        table.AddCell(cell);
                    }
                }
                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string PrintBarcodeThaoTac(GridModelHHMaVach model)
        {
            var pgSize = new iTextSharp.text.Rectangle(99 * (model.SoBanGhi), 62);
            var doc = new iTextSharp.text.Document(pgSize, 0, 0, 0, 0);
            //PdfPTable
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                string str = CookieStore.GetCookieAes("SubDomain");
                string url = "/download/" + str;
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIAL.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(3);
                float[] widths = new float[] { 99, 99, 99 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;
                double tongsotem = 0;
                //chỉ in mã vạch
                if (model.InGia == false && model.InMaHH == false && model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InGia == false && model.InMaHH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        string tenhang = "";
                        for (int i = 0; i < item.HangHoa_ThuocTinh.Count; i++)
                        {
                            tenhang += "-" + item.HangHoa_ThuocTinh[i].GiaTri;
                        }
                        tenhang = tenhang + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -10));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 16;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 14;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);
                                    table1.AddCell(cellBarcode);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 24;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 24;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else
                {
                    foreach (var item in model.listHH)
                    {
                        string tenhang = "";
                        for (int i = 0; i < item.HangHoa_ThuocTinh.Count; i++)
                        {
                            tenhang += "-" + item.HangHoa_ThuocTinh[i].GiaTri;
                        }
                        tenhang = tenhang + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 9;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 9;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 18;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 18;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                            }

                            table1.AddCell(cellBarcode);
                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string PrintBarcodeThaoTac2Tem(GridModelHHMaVach model)
        {
            var pgSize = new iTextSharp.text.Rectangle(211f, 62.8f);
            var doc = new iTextSharp.text.Document(pgSize, 0, 0, 0, 0);
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                //PdfPTable
                string str = CookieStore.GetCookieAes("SubDomain");
                string url = "/download/" + str;
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();

                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIAL.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(2);
                float[] widths = new float[] { 100f, 100f };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;
                double tongsotem = 0;
                //chỉ in mã vạch
                if (model.InGia == false && model.InMaHH == false && model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InGia == false && model.InMaHH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        string tenhang = "";
                        for (int i = 0; i < item.HangHoa_ThuocTinh.Count; i++)
                        {
                            tenhang += "-" + item.HangHoa_ThuocTinh[i].GiaTri;
                        }
                        tenhang = tenhang + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -10));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 16;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 14;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);
                                    table1.AddCell(cellBarcode);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 24;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 24;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else
                {
                    foreach (var item in model.listHH)
                    {
                        string tenhang = "";
                        for (int i = 0; i < item.HangHoa_ThuocTinh.Count; i++)
                        {
                            tenhang += "-" + item.HangHoa_ThuocTinh[i].GiaTri;
                        }
                        tenhang = tenhang + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 9;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 9;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 18;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 18;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                            }

                            table1.AddCell(cellBarcode);
                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        //A4
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string PrintBarcodeThaoTac1(GridModelHHMaVach model)
        {
            var doc = new Document(PageSize.A4, 5, 0, 35, 0);
            //PdfPTable
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                string str = CookieStore.GetCookieAes("SubDomain");
                string url = "/download/" + str;
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIAL.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(model.SoBanGhi);
                float[] widths = new float[] { 107, 107, 107, 107, 107 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;
                double tongsotem = 0;
                //chỉ in mã vạch
                if (model.InGia == false && model.InMaHH == false && model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InGia == false && model.InMaHH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        string tenhang = "";
                        for (int i = 0; i < item.HangHoa_ThuocTinh.Count; i++)
                        {
                            tenhang += "-" + item.HangHoa_ThuocTinh[i].GiaTri;
                        }
                        tenhang = tenhang + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -10));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 16;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 14;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);
                                    table1.AddCell(cellBarcode);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 24;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 24;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else
                {
                    foreach (var item in model.listHH)
                    {
                        string tenhang = "";
                        for (int i = 0; i < item.HangHoa_ThuocTinh.Count; i++)
                        {
                            tenhang += "-" + item.HangHoa_ThuocTinh[i].GiaTri;
                        }
                        tenhang = tenhang + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.TonKho.Value;
                        for (int i = 0; i < item.TonKho; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 9;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 9;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 18;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 18;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                            }

                            table1.AddCell(cellBarcode);
                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string PrintBarcodeThaoTacNhapHang(GridModelHHMaVach model)
        {
            var pgSize = new iTextSharp.text.Rectangle(99 * (model.SoBanGhi), 62);
            var doc = new iTextSharp.text.Document(pgSize, 0, 0, 0, 0);
            //PdfPTable
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                string str = CookieStore.GetCookieAes("SubDomain");
                string url = "/download/" + str;
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIAL.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(3);
                float[] widths = new float[] { 99, 99, 99 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;
                double tongsotem = 0;
                //chỉ in mã vạch
                if (model.InGia == false && model.InMaHH == false && model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InGia == false && model.InMaHH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        string tenhang = "";
                        tenhang = tenhang + item.ThuocTinh_GiaTri + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -10));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 16;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 14;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);
                                    table1.AddCell(cellBarcode);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 24;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 24;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        string tenhang = "";
                        tenhang = tenhang + item.ThuocTinh_GiaTri + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 9;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 9;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 18;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 18;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                            }

                            table1.AddCell(cellBarcode);
                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 3 - (tongsotem % 3);
                    if (temthieu != 3)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }

                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string PrintBarcodeThaoTacNhapHang2Tem(GridModelHHMaVach model)
        {
            var pgSize = new iTextSharp.text.Rectangle(211f, 62.8f);
            var doc = new iTextSharp.text.Document(pgSize, 0, 0, 0, 0);
            //PdfPTable
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                string str = CookieStore.GetCookieAes("SubDomain");
                string url = "/download/" + str;
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\TAHOMA.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(2);
                float[] widths = new float[] { 100, 100 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;
                double tongsotem = 0;
                //chỉ in mã vạch
                if (model.InGia == false && model.InMaHH == false && model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InGia == false && model.InMaHH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        string tenhang = "";
                        tenhang = tenhang + item.ThuocTinh_GiaTri + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -10));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 16;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 14;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);
                                    table1.AddCell(cellBarcode);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 24;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 24;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        string tenhang = "";
                        tenhang = tenhang + item.ThuocTinh_GiaTri + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(90);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -9, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 9;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 9;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 18;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 18;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                            }

                            table1.AddCell(cellBarcode);
                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 2 - (tongsotem % 2);
                    if (temthieu != 2)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }

                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        //A4
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public string PrintBarcodeThaoTacNhapHang1(GridModelHHMaVach model)
        {
            var doc = new Document(PageSize.A4, 5, 0, 35, 0);
            //PdfPTable
            string tencuahang = "";
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    tencuahang = db.HT_CongTy.FirstOrDefault().TenCongTy;
                }
                string str = CookieStore.GetCookieAes("SubDomain");
                string url = "/download/" + str;
                url = Path.Combine(HttpContext.Current.Server.MapPath(url));
                if (!Directory.Exists(url))
                {
                    Directory.CreateDirectory(url);
                }
                url = url + "/barcode.pdf";
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create));
                doc.Open();
                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\TAHOMA.TTF", BaseFont.IDENTITY_H, true);
                Font fontext = new Font(bf, 7, Font.NORMAL);
                Font fontextTenCuaHang = new Font(bf, 8, Font.BOLD);
                PdfPTable table = new PdfPTable(model.SoBanGhi);
                float[] widths = new float[] { 107, 107, 107, 107, 107 };
                table.SetTotalWidth(widths);
                table.LockedWidth = true;
                double tongsotem = 0;
                if (model.InGia == false && model.InMaHH == false && model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            PdfPCell cellrong = new PdfPCell(new Phrase(""));
                            cellrong.Border = 0;
                            cellrong.FixedHeight = 12;
                            table1.AddCell(cellrong);

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InGia == false && model.InMaHH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        string tenhang = "";
                        tenhang = tenhang + item.ThuocTinh_GiaTri + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            code128.Font = null;
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -10));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 16;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 14;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);
                                    table1.AddCell(cellBarcode);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 24;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 24;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;
                                    table1.AddCell(cellTenHang);

                                    table1.AddCell(cellBarcode);

                                    PdfPCell cellrongadd = new PdfPCell(new Phrase(""));
                                    cellrongadd.Border = 0;
                                    cellrongadd.FixedHeight = 6;
                                    table1.AddCell(cellrongadd);
                                }
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else if (model.InTenHH == false && model.InTenCH == false)
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 37;

                            PdfPCell cellTenHang = new PdfPCell(new Phrase(""));
                            cellTenHang.Border = 0;
                            cellTenHang.FixedHeight = 10;

                            table1.AddCell(cellTenHang);
                            table1.AddCell(cellBarcode);

                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                else
                {
                    foreach (var item in model.listHH)
                    {
                        item.GiaBan = item.GiaBanMaVach;
                        string tenhang = "";
                        tenhang = tenhang + item.ThuocTinh_GiaTri + (item.TenDonViTinh != "" ? "(" + item.TenDonViTinh + ")" : "");
                        if (model.ID_BangGia != Guid.Empty && model.InGia == true)
                        {
                            DM_HangHoaInMaVach hhmv = GetHangHoaByID_BangGia(model.ID_BangGia, item.ID_DonViQuiDoi);
                            if (hhmv != null)
                            {
                                item.GiaBan = hhmv.GiaBan;
                            }
                        }
                        tongsotem += item.SoLuong.Value;
                        for (int i = 0; i < item.SoLuong; i++)
                        {
                            PdfPTable table1 = new PdfPTable(1);
                            PdfContentByte cb = writer.DirectContentUnder;
                            Barcode128 code128 = new Barcode128();
                            code128.Code = item.MaHangHoa;
                            code128.CodeType = Barcode.CODE128;
                            code128.Size = 7;
                            if (model.InMaHH == false)
                            {
                                code128.Font = null;
                            }
                            Image imagecode128 = code128.CreateImageWithBarcode(cb, null, null);
                            imagecode128.ScaleAbsoluteWidth(95);
                            Phrase phrase = new Phrase();
                            phrase.Add(new Chunk(imagecode128, -7, -15));

                            PdfPCell cellBarcode = new PdfPCell();
                            cellBarcode.AddElement(phrase);
                            cellBarcode.Border = 0;
                            cellBarcode.HorizontalAlignment = 1;
                            cellBarcode.FixedHeight = 29;

                            if (model.InTenCH == true)
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 9;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);

                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 9;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                                else
                                {
                                    PdfPCell cellTenCuaHang = new PdfPCell(new Phrase(tencuahang, fontextTenCuaHang));
                                    cellTenCuaHang.Border = 0;
                                    cellTenCuaHang.FixedHeight = 18;
                                    cellTenCuaHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenCuaHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenCuaHang.Padding = 0;
                                    table1.AddCell(cellTenCuaHang);
                                }
                            }
                            else
                            {
                                if (model.InTenHH == true)
                                {
                                    PdfPCell cellTenHang = new PdfPCell(new Phrase(item.TenHangHoa + tenhang, fontext));
                                    cellTenHang.Border = 0;
                                    cellTenHang.FixedHeight = 18;
                                    cellTenHang.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    cellTenHang.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                    cellTenHang.Padding = 0;

                                    table1.AddCell(cellTenHang);
                                }
                            }

                            table1.AddCell(cellBarcode);
                            if (model.InGia == true)
                            {
                                Font fontextgia = new Font(bf, 8, Font.BOLD);
                                PdfPCell cellGia = new PdfPCell(new Phrase(string.Format("{0:n0}", item.GiaBan).Replace(".", ",") + "VNĐ", fontextgia));
                                cellGia.Border = 0;
                                cellGia.FixedHeight = 12;
                                cellGia.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                cellGia.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                table1.AddCell(cellGia);
                            }
                            else
                            {
                                PdfPCell cellrong = new PdfPCell(new Phrase(""));
                                cellrong.Border = 0;
                                cellrong.FixedHeight = 12;
                                table1.AddCell(cellrong);
                            }

                            Phrase phrasetable1 = new Phrase();
                            phrasetable1.Add(table1);
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(phrasetable1);
                            cell.FixedHeight = 59.5f;
                            cell.Border = 0;
                            cell.Padding = 0;
                            table.AddCell(cell);

                        }
                    }
                    double temthieu = 5 - (tongsotem % 5);
                    if (temthieu != 5)
                    {
                        for (int j = 0; j < temthieu; j++)
                        {
                            PdfPCell celltemthieu = new PdfPCell(new Phrase(""));
                            celltemthieu.Border = 0;
                            table.AddCell(celltemthieu);
                        }
                    }
                }
                doc.Add(table);
                doc.Close();
                return "/download/" + str + "/barcode.pdf";
            }
        }

        private void RotatePDF(string inputFile, string outputFile)
        {
            using (FileStream outStream = new FileStream(outputFile, FileMode.Create))
            {
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(inputFile);
                iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(reader, outStream);
                int numberOfpage = reader.NumberOfPages;
                for (int i = 0; i < numberOfpage; i++)
                {
                    iTextSharp.text.pdf.PdfDictionary pageDict = reader.GetPageN(i + 1);
                    int desiredRot = 90; // 90 degrees clockwise from what it is now
                    iTextSharp.text.pdf.PdfNumber rotation = pageDict.GetAsNumber(iTextSharp.text.pdf.PdfName.ROTATE);

                    if (rotation != null)
                    {
                        desiredRot += rotation.IntValue;
                        desiredRot %= 360; // must be 0, 90, 180, or 270
                    }
                    pageDict.Put(iTextSharp.text.pdf.PdfName.ROTATE, new iTextSharp.text.pdf.PdfNumber(desiredRot));
                }
                stamper.Close();
                reader.Close();
            }
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<DM_NhomHangHoa> SeachDM_NhomHangHoa(string TenNhom)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_NhomHangHoa> lst = _classDMHH.seachNhomHangHoa(TenNhom);
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

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<DM_HangHoa> GetIDHangHoaByID_CungLoai(Guid idcungloai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_HangHoa> lst = _classDMHH.GetIDHangHoaByID_CungLoai(idcungloai);
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

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool getThuocTinhGhepChuoi(string id_hanghoa, string chuoi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                bool check = false;
                try
                {
                    var _classDMHH = new ClassDM_HangHoa(db);
                    if (!string.IsNullOrEmpty(chuoi))
                    {
                        Guid idcungloai = new Guid(id_hanghoa);
                        List<Guid> idhh = _classDMHH.gethanghoacungloainotxoa(idcungloai).Select(p => p.ID).ToList();
                        //IQueryable<HangHoa_ThuocTinh> lst = ClassDM_HangHoa.GetHH_ThuocTinh(p => idhh.Contains(p.ID_HangHoa)).OrderBy(p => p.ThuTuNhap);
                        foreach (var item in idhh)
                        {
                            string chuoi1 = "";
                            List<HangHoa_ThuocTinh> lsttt = _classDMHH.GetHH_ThuocTinh(p => p.ID_HangHoa == item).OrderBy(p => p.ThuTuNhap).ToList();
                            foreach (var item1 in lsttt)
                            {
                                chuoi1 = chuoi1 + item1.GiaTri;
                            }
                            if (chuoi1.Replace(" ", String.Empty) == chuoi.Replace(" ", String.Empty))
                            {
                                check = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                return check;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public int getThuocTinhGhepChuoiEdit(string idcungloai, string chuoi, Guid idhanghoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                var count = 0;
                try
                {
                    if (!string.IsNullOrEmpty(chuoi))
                    {
                        Guid id_cungloai = new Guid(idcungloai);
                        List<Guid> idhh = _classDMHH.gethanghoacungloainotxoa(id_cungloai).Select(p => p.ID).ToList();
                        foreach (var item in idhh)
                        {
                            string chuoi1 = "";
                            List<HangHoa_ThuocTinh> lsttt = _classDMHH.GetHH_ThuocTinh(p => p.ID_HangHoa == item).OrderBy(p => p.ThuTuNhap).ToList();
                            foreach (var item1 in lsttt)
                            {
                                chuoi1 = chuoi1 + item1.GiaTri;
                            }
                            if (chuoi1.Replace(" ", String.Empty) == chuoi.Replace(" ", String.Empty) && item != idhanghoa)
                            {
                                count = count + 1;
                            }
                        }
                    }
                }
                catch { }
                return count;
            }
        }

        public List<DM_HangHoaDTO> GetHangHoa_ByIDNhom(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_HangHoaDTO> lstAllHHs = _classDMHH.GetHangHoa_ByIDNhom(id);
                if (lstAllHHs.Count > 0)
                {
                    return lstAllHHs;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<DemCheck> GetlistCheckBoxHH(Guid id_dvqd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                var id_hanghoa = _classDVQD.Get(p => p.ID == id_dvqd).ID_HangHoa;
                var id_cungloai = _classDMHH.Get(p => p.ID == id_hanghoa).ID_HangHoaCungLoai;
                List<DemCheck> dto = _classDMHH.gethanghoacungloainotxoaDEMCL(id_cungloai.Value).ToList();
                return dto;
            }
        }

        public List<DemCheck> GetlistCheckBoxKhongCoCL(Guid id_dvqd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                var id_hanghoa = _classDVQD.Get(p => p.ID == id_dvqd).ID_HangHoa;
                List<DemCheck> dto = _classDMHH.gethanghoacungloainotxoaKoCoCL(id_hanghoa).ToList();
                return dto;
            }
        }

        public List<DemCheck> GetlistCheckBoxHHNotDemCL(Guid id_dvqd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                var id_hanghoa = _classDVQD.Get(p => p.ID == id_dvqd).ID_HangHoa;
                List<DemCheck> dto = _classDMHH.gethanghoacungloainotxoaKoCoCL(id_hanghoa).ToList();
                return dto;
            }
        }

        public List<DM_HangHoaDTO> GetHangHoa_ByIDNhomKiemKho(string id, Guid iddonvi, DateTime timeKK)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                if (id == "null")
                {
                    id = "";
                }
                DateTime timeEnd = timeKK.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                paramlist.Add(new SqlParameter("ID_NhomHangHoa", id));
                paramlist.Add(new SqlParameter("TimeKK", timeEnd));
                var listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec AddHHByNhomHangHoaKiemKho @ID_ChiNhanh, @ID_NhomHangHoa, @TimeKK", paramlist.ToArray()).ToList();
                listTon = listTon.Select(p => new DM_HangHoaDTO
                {
                    ID_DonViQuiDoi = p.ID_DonViQuiDoi,
                    ID = p.ID,
                    ID_Random = p.ID_Random,
                    MaHangHoa = p.MaHangHoa,
                    TenHangHoa = p.TenHangHoa,
                    ThuocTinh_GiaTri = p.ThuocTinh_GiaTri,
                    TenDonViTinh = p.TenDonViTinh,
                    QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                    TyLeChuyenDoi = p.TyLeChuyenDoi,
                    LaHangHoa = p.LaHangHoa,
                    GiaVon = p.GiaVon,
                    DonGia = p.DonGia,
                    GiaBan = p.GiaBan,
                    GiaNhap = p.GiaNhap,
                    SoLuong = p.SoLuong,
                    ThanhTien = p.SoLuong,// set default TonThucTe = TonKho (DB)
                    TienChietKhau = 0,// chenh lech = 0
                    ThanhToan = 0,// gia tri lech = 0
                    SrcImage = p.SrcImage,
                    ID_LoHang = p.ID_LoHang,
                    MaLoHang = p.MaLoHang == "" ? null : p.MaLoHang,
                    NgaySanXuat = p.NgaySanXuat,
                    NgayHetHan = p.NgayHetHan,
                    DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == p.ID && ct.Xoa != true).Select(x => new DonViTinh
                    {
                        ID_HangHoa = p.ID,
                        TenDonViTinh = x.TenDonViTinh,
                        ID_DonViQuiDoi = x.ID,
                        QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                        Xoa = true,
                        TyLeChuyenDoi = x.TyLeChuyenDoi
                    }).ToList(),
                }).ToList();
                return listTon.ToList();
            }
        }

        public string CapNhatQuanLyTheoLoHangAllHH()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "";
                }
                else
                {
                    db.DM_HangHoa.SqlQuery("UPDATE DM_HangHoa SET QuanLyTheoLoHang = 0");
                    return "";
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public PageListDTO CountRecords_FirstLoad(float pageSize)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                IQueryable<DM_HangHoa> lstKhacHangs = _classDMHH.Gets(null);

                var totalRecords = lstKhacHangs.Count();
                PageListDTO pageDTO = new PageListDTO
                {
                    TotalRecord = totalRecords,
                    PageCount = Math.Ceiling(totalRecords / pageSize)
                };
                return pageDTO;
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ExportExel_DMHH(GridModel model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    double tongton = 0;
                    int total = 0, pagecount = 0;
                    ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                    Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                    List<DM_HangHoaDTO> lstAllHHs = _classDMHH.GetListHangHoas_WhereNew(model.currentPage, model.pageSize, model.maHoaDon, model.tonkho, model.kinhdoanh, model.loaihangs,
                                                                                                model.idnhomhang, model.iddonvi, string.Join(",", model.listthuoctinh), model.columsort, model.sort,
                                                                                                model.listSearchColumn.Where(o => o.Value != null && !string.IsNullOrWhiteSpace(o.Value.ToString())).ToList(), ref tongton, ref total, ref pagecount);
                    List<DM_HangHoa_Excel> lst = new List<DM_HangHoa_Excel>();
                    foreach (var item in lstAllHHs)
                    {
                        DM_HangHoa_Excel DM = new DM_HangHoa_Excel();
                        DM.MaHangHoa = item.MaHangHoa;
                        DM.TenHangHoa = item.TenHangHoa;
                        DM.TenDonViTinh = item.TenDonViTinh;
                        DM.NhomHangHoa = item.NhomHangHoa;
                        DM.LoaiHangHoa = item.LaHangHoa != null ? item.LaHangHoa.Value == true ? "Hàng hóa" : "Dịch vụ" : "Combo - đóng gói";
                        DM.GiaBan = item.GiaBan;
                        DM.GiaVon = item.QuanLyTheoLoHang == true ? 0 : item.GiaVon;
                        DM.TonKho = item.LaHangHoa == true ? item.TonKho : 0;
                        DM.GhiChu = item.GhiChu;
                        DM.TrangThai = item.TrangThai ? "Cho phép kinh doanh" : "Ngừng kinh doanh";
                        lst.Add(DM);
                    }
                    DataTable excel = _classOFDCM.ToDataTable<DM_HangHoa_Excel>(lst);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhMucHangHoa.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhMucHangHoa.xlsx");
                    fileSave = _classOFDCM.createFolder_Download(fileSave);
                    _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, model.ColumnHides);
                    var index = fileSave.IndexOf(@"\Template");
                    fileSave = "~" + fileSave.Substring(index, fileSave.Length - index);
                    fileSave = fileSave.Replace(@"\", "/");
                    return ActionTrueData(fileSave);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<DM_DonViTinhClick> GetDonViTinhKhacGiaoDich(Guid iddvqd, Guid iddv, DateTime? ngaykk = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (ngaykk == null)
                {
                    ngaykk = DateTime.Now;
                }
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_DonViQuiDoi", iddvqd));
                paramlist.Add(new SqlParameter("ID_DonVi", iddv));
                paramlist.Add(new SqlParameter("ThoiGian", ngaykk));
                List<DM_DonViTinhClick> listTon = db.Database.SqlQuery<DM_DonViTinhClick>("exec GetDVTKhacInGiaoDich @ID_DonViQuiDoi,@ID_DonVi, @ThoiGian", paramlist.ToArray()).ToList();
                return listTon;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExel_DMLoHang(string maHoaDon, int tonkho, string idnhomhang, string columnsHide, Guid iddonvi, string listthuoctinh, DateTime? dayStart, DateTime? dayEnd, string time)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var roleXemGiaVon = CheckRoleXemGiaVon(db);
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<BCDM_LoHangDTO> lstAllHHs = _classDMHH.XuatFileDMLoHang(maHoaDon, tonkho, idnhomhang, iddonvi, listthuoctinh, dayStart, dayEnd);
                List<DM_LoHang_Excel> lst = new List<DM_LoHang_Excel>();
                foreach (var item in lstAllHHs)
                {
                    DM_LoHang_Excel DM = new DM_LoHang_Excel();
                    DM.MaHangHoa = item.MaHangHoa;
                    DM.TenHangHoa = item.TenHangHoa;
                    DM.TenDonViTinh = item.TenDonViTinh;
                    DM.NhomHangHoa = item.NhomHangHoa;
                    DM.GiaBan = item.GiaBan;
                    DM.TonKho = item.TonKho;
                    if (roleXemGiaVon)
                    {
                        DM.GiaVon = item.GiaVon;
                        DM.GiaTriTon = item.TonKho * item.GiaVon;
                    }
                    else
                    {
                        DM.GiaVon = 0;
                        DM.GiaTriTon = 0;
                    }

                    DM.MaLoHang = item.MaLoHang;
                    DM.NgaySanXuat = item.NgaySanXuat;
                    DM.NgayHetHan = item.NgayHetHan;
                    DM.TrangThai = item.TrangThai;
                    DM.SoNgayConHan = item.SoNgayConHan;
                    lst.Add(DM);
                }
                DataTable excel = _classOFDCM.ToDataTable<DM_LoHang_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_DanhMucLoHang.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/DanhMucLoHang.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcelLoHang(fileTeamplate, fileSave, excel, 3, 27, 24, true, columnsHide, time);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void ExportExel_TheKhoHH(Guid id, Guid iddonvi, string columnsHide)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<DM_TheKhoDTO> lstAllHHTKs = _classDMHH.GetListTheKho(id, iddonvi).ToList();
                List<DM_TheKhoHangHoa_Excel> lst = new List<DM_TheKhoHangHoa_Excel>();
                foreach (var item in lstAllHHTKs)
                {
                    DM_TheKhoHangHoa_Excel DM = new DM_TheKhoHangHoa_Excel();
                    DM.MaHoaDon = item.MaHoaDon;
                    DM.LoaiChungTu = item.LoaiChungTu;
                    DM.NgayLapHoaDon = item.NgayLapHoaDon;
                    DM.GiaVon = item.GiaVon;
                    DM.SoLuong = item.SoLuong;
                    DM.TonKho = item.TonKho;
                    lst.Add(DM);
                }
                DataTable excel = _classOFDCM.ToDataTable<DM_TheKhoHangHoa_Excel>(lst);
                string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/Teamplate_TheKhoDanhMucHangHoa.xlsx");
                string fileSave = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/TheKhoDanhMucHangHoa.xlsx");
                fileSave = _classOFDCM.createFolder_Download(fileSave);
                _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, true, columnsHide);
                HttpResponse Response = HttpContext.Current.Response;
                _classOFDCM.downloadFile(fileSave);
            }
        }

        public IHttpActionResult GetListKeyColumHanghoa()
        {
            var ListComlumnSearchHangHoa = new List<ColumSearch>()
                {
                    new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.mahanghoa},
                   new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.tenhanghoa },
                   new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.nhomhang },
                   new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.loaihang },
                   new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.giaban ,type=(int)commonEnumHellper.KeyCompare.bang},
                   new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.giavon ,type=(int)commonEnumHellper.KeyCompare.bang},
                    new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.tonkho ,type=(int)commonEnumHellper.KeyCompare.bang},
                   new ColumSearch{Key=(int)GridHellper.ColumnHangHoa.trangthai },

                };
            return Json(new { keycolumn = ListComlumnSearchHangHoa.ToList(), compareFile = commonEnumHellper.ListCompare.ToList() });
        }

        public IHttpActionResult GetListDMLoHang(int currentPage, int pageSize, string maHoaDon, int tonkho, string idnhomhang, Guid iddonvi, string listthuoctinh, DateTime? dayStart, DateTime? dayEnd, int checkngay)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                int total = 0;
                int pagecount = 0;
                List<BCDM_LoHangDTO> lsrReturns = new List<BCDM_LoHangDTO>();
                List<BCDM_LoHangDTO> lstAllHHs = _classDMHH.GetlistDM_LoHang(currentPage, pageSize, maHoaDon, tonkho, idnhomhang, iddonvi, listthuoctinh, dayStart, dayEnd, ref total, ref pagecount, checkngay);

                foreach (BCDM_LoHangDTO item in lstAllHHs)
                {
                    try
                    {
                        BCDM_LoHangDTO itemData = new BCDM_LoHangDTO
                        {
                            ID = item.ID,
                            ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                            ID_NhomHangHoa = item.ID_NhomHangHoa,
                            ID_LoHang = item.ID_LoHang,
                            TenHangHoa = item.TenHangHoa,
                            MaHangHoa = item.MaHangHoa,
                            MaLoHang = item.MaLoHang,
                            NhomHangHoa = item.NhomHangHoa,
                            GiaBan = item.GiaBan,
                            GiaVon = item.GiaVon,
                            TonKho = item.TonKho,
                            NgaySanXuat = item.NgaySanXuat,
                            NgayHetHan = item.NgayHetHan,
                            TrangThai = item.TrangThai,
                            SoNgayConHan = item.SoNgayConHan,
                            TonToiThieu = item.TonToiThieu,
                            TonToiDa = item.TonToiDa,
                            NgayTao = item.NgayTao,
                            TenDonViTinh = item.TenDonViTinh,
                            HangHoa_ThuocTinh = _classDMHH.SelectHangHoa_ThuocTinh(item.ID).Select(x => new HangHoa_ThuocTinh
                            {
                                GiaTri = x.GiaTri,
                                ThuTuNhap = x.ThuTuNhap
                            }).OrderBy(p => p.ThuTuNhap).ToList(),
                        };
                        lsrReturns.Add(itemData);
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("DM_HangHoaAPI_GetListDMLoHang: " + ex.Message + ex.InnerException);
                        lsrReturns = new List<BCDM_LoHangDTO>();
                    }
                }

                JsonResultExampleDM_Lo jsonobj = new JsonResultExampleDM_Lo
                {
                    lstHH = lsrReturns,
                };
                return Json(new { data = jsonobj, TotalRecord = total, PageCount = pagecount });
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public int? GetNgayNhacLoHetHan(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return 0;
                }
                else
                {
                    return db.HT_CauHinhPhanMem.Where(p => p.ID_DonVi == iddonvi).FirstOrDefault().ThoiGianNhacHanSuDungLo;
                }
            }
        }

        [HttpPost]
        public IHttpActionResult SearchHangHoaForColumn(GridModel model)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                double tongton = 0;
                int total = 0;
                int pagecount = 0;
                List<DM_HangHoaDTO> lsrReturns = new List<DM_HangHoaDTO>();
                if (model != null)
                {
                    List<DM_HangHoaDTO> lstAllHHs = _classDMHH.GetListHangHoas_WhereNew(model.currentPage, model.pageSize, model.maHoaDon, model.tonkho, model.kinhdoanh, model.loaihangs,
                                                                                            model.idnhomhang, model.iddonvi, string.Join(",", model.listthuoctinh), model.columsort, model.sort,
                                                                                            model.listSearchColumn.Where(o => o.Value != null && !string.IsNullOrWhiteSpace(o.Value.ToString())).ToList(), ref tongton, ref total, ref pagecount);

                    if (lstAllHHs != null && lstAllHHs.Count() > 0)
                    {
                        foreach (DM_HangHoaDTO item in lstAllHHs)
                        {
                            try
                            {
                                DM_HangHoaDTO itemData = new DM_HangHoaDTO
                                {
                                    ID = item.ID,
                                    ID_HangHoaCungLoai = item.ID_HangHoaCungLoai,
                                    TenHangHoa = item.TenHangHoa,
                                    LaChaCungLoai = item.LaChaCungLoai,
                                    MaHangHoa = item.MaHangHoa,
                                    ID_NhomHangHoa = item.ID_NhomHangHoa,
                                    NhomHangHoa = item.NhomHangHoa,
                                    GiaBan = Math.Round((double)item.GiaBan, MidpointRounding.ToEven),
                                    GiaVon = (double)item.GiaVon,
                                    TonKho = item.LaHangHoa == true ? Math.Round(item.TonKho.Value, 3, MidpointRounding.ToEven) : 0,
                                    TrangThai = item.TrangThai,
                                    sLoaiHangHoa = item.sLoaiHangHoa,
                                    DuocBanTrucTiep = item.DuocBanTrucTiep,
                                    TonToiThieu = item.TonToiThieu,
                                    TonToiDa = item.TonToiDa,
                                    DonViTinhChuan = item.TenDonViTinh,
                                    TenDonViTinh = item.TenDonViTinh,
                                    LaHangHoa = item.LaHangHoa,
                                    ID_DonViQuiDoi = item.ID_DonViQuiDoi,
                                    GhiChu = item.GhiChu,
                                    QuanLyTheoLoHang = item.QuanLyTheoLoHang,
                                    SoPhutThucHien = item.SoPhutThucHien,
                                    DichVuTheoGio = item.DichVuTheoGio,
                                    DuocTichDiem = item.DuocTichDiem,
                                    QuanLyBaoDuong = item.QuanLyBaoDuong,
                                    LoaiBaoDuong = item.LoaiBaoDuong,
                                    LoaiHangHoa = item.LoaiHangHoa,
                                    SoKmBaoHanh = item.SoKmBaoHanh,
                                    HoaHongTruocChietKhau = item.HoaHongTruocChietKhau,
                                    Xoa = item.Xoa,
                                    ID_Xe = item.ID_Xe,
                                    BienSo = item.BienSo,
                                    ChietKhauMD_NV = item.ChietKhauMD_NV,
                                    ChietKhauMD_NVTheoPT = item.ChietKhauMD_NVTheoPT,
                                    DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == item.ID && ct.Xoa != true).Select(x => new DonViTinh
                                    {
                                        ID_HangHoa = item.ID,
                                        TenDonViTinh = x.TenDonViTinh,
                                        ID_DonViQuiDoi = x.ID,
                                        DonViTinhChuan = item.TenDonViTinh,
                                        QuanLyTheoLoHang = item.QuanLyTheoLoHang
                                    }).ToList(),
                                    ThuocTinhGiaTri = item.ThuocTinhGiaTri,
                                    ListChildren = item.ListChildren.Select(o => new DM_HangHoaDTO
                                    {
                                        ID = o.ID,
                                        ID_HangHoaCungLoai = o.ID_HangHoaCungLoai,
                                        TenHangHoa = o.TenHangHoa,
                                        LaChaCungLoai = o.LaChaCungLoai,
                                        MaHangHoa = o.MaHangHoa,
                                        ID_NhomHangHoa = o.ID_NhomHangHoa,
                                        NhomHangHoa = o.NhomHangHoa,
                                        GiaBan = Math.Round((double)o.GiaBan, MidpointRounding.ToEven),
                                        GiaVon = (double)o.GiaVon,
                                        TonKho = o.LaHangHoa == true ? Math.Round(o.TonKho.Value, 3, MidpointRounding.ToEven) : 0,
                                        TrangThai = o.TrangThai,
                                        sLoaiHangHoa = o.sLoaiHangHoa,
                                        DuocBanTrucTiep = o.DuocBanTrucTiep,
                                        TonToiThieu = o.TonToiThieu,
                                        TonToiDa = o.TonToiDa,
                                        DonViTinhChuan = o.TenDonViTinh,
                                        TenDonViTinh = o.TenDonViTinh,
                                        LaHangHoa = o.LaHangHoa,
                                        ID_DonViQuiDoi = o.ID_DonViQuiDoi,
                                        GhiChu = o.GhiChu,
                                        QuanLyTheoLoHang = o.QuanLyTheoLoHang,
                                        DichVuTheoGio = o.DichVuTheoGio,
                                        DuocTichDiem = o.DuocTichDiem,
                                        QuanLyBaoDuong = item.QuanLyBaoDuong,
                                        LoaiHangHoa = item.LoaiHangHoa,
                                        LoaiBaoDuong = item.LoaiBaoDuong,
                                        SoKmBaoHanh = item.SoKmBaoHanh,
                                        HoaHongTruocChietKhau = item.HoaHongTruocChietKhau,
                                        Xoa = o.Xoa,
                                        ID_Xe = item.ID_Xe,
                                        BienSo = item.BienSo,
                                        ChietKhauMD_NV = item.ChietKhauMD_NV,
                                        ChietKhauMD_NVTheoPT = item.ChietKhauMD_NVTheoPT,
                                        DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == o.ID && ct.Xoa != true).Select(x => new DonViTinh
                                        {
                                            TenDonViTinh = x.TenDonViTinh,
                                            ID_DonViQuiDoi = x.ID,
                                            DonViTinhChuan = o.TenDonViTinh,
                                            QuanLyTheoLoHang = o.QuanLyTheoLoHang
                                        }).ToList(),
                                        ThuocTinhGiaTri = o.ThuocTinhGiaTri
                                    }).ToList(),
                                };
                                lsrReturns.Add(itemData);
                            }
                            catch (Exception ex)
                            {
                                CookieStore.WriteLog("DM_HangHoaAPI_SearchHangHoaForColumn: " + ex.Message + ex.InnerException);
                                lsrReturns = new List<DM_HangHoaDTO>();
                            }
                        }
                    }
                }
                return Json(new
                {
                    data = lsrReturns,
                    tongton = Math.Round(tongton, 2, MidpointRounding.ToEven),
                    TotalRecord = total,
                    PageCount = pagecount
                });
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult TraCuuHangHoa(int currentPage, int pageSize, Guid id_donvi, string txtSearch)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDV = new classDM_DonVi(db);
                List<DM_HangHoaDTO> listTon = new List<DM_HangHoaDTO>();

                var dv = _classDV.Gets(null);
                var totalRecords = 0;
                foreach (var item in dv)
                {
                    var listTon1 = new List<DM_HangHoaDTO>();
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    if (txtSearch != null & txtSearch != "" & txtSearch != "null")
                    {
                        char[] whitespace = new char[] { ' ', '\t' };
                        string[] textFilter = txtSearch.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                        string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                        paramlist.Add(new SqlParameter("MaHH", string.Join(",", utf)));
                        paramlist.Add(new SqlParameter("MaHHCoDau", string.Join(",", utf8)));
                        paramlist.Add(new SqlParameter("ID_ChiNhanh", item.ID));
                        paramlist.Add(new SqlParameter("currentPage", currentPage));
                        paramlist.Add(new SqlParameter("pageSize", pageSize));
                        listTon1 = db.Database.SqlQuery<DM_HangHoaDTO>("exec TraCuuHangHoa @ID_ChiNhanh, @MaHH,@MaHHCoDau", paramlist.ToArray()).ToList();
                        totalRecords = listTon1.Count();
                        listTon1 = listTon1.Skip(currentPage * pageSize).Take(pageSize).ToList();
                    }
                    listTon.AddRange(listTon1);
                }
                JsonResultExampleHH jsonobj = new JsonResultExampleHH
                {
                    lstHH = listTon.OrderBy(p => p.MaHangHoa).ToList(),
                    Rowcount = totalRecords,
                    pageCount = System.Math.Ceiling(totalRecords / (float)pageSize)
                };
                return Json(new { data = jsonobj });
            }
        }

        public PageListDTO GetPageCountTheKho_Where(float pageSize, Guid iddonvi, string txtSearch)
        {
            var totalRecords = 0;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                List<DM_HangHoaDTO> lst = new List<DM_HangHoaDTO>();

                if (txtSearch != null & txtSearch != "" & txtSearch != "null")
                {
                    var Search1 = CommonStatic.ConvertToUnSign(txtSearch.Trim()).ToLower();
                    Guid ID_ChiNhanh = iddonvi;
                    string Search = "%" + Search1 + "%";
                    var MaHHCoDau = txtSearch != null ? "%" + txtSearch.Trim() + "%" : "%%";
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                    paramlist.Add(new SqlParameter("MaHH", Search));
                    paramlist.Add(new SqlParameter("MaHHCoDau", MaHHCoDau));
                    lst = db.Database.SqlQuery<DM_HangHoaDTO>("exec PageCountTraCuuHangHoa @ID_ChiNhanh, @MaHH, @MaHHCoDau", paramlist.ToArray()).ToList();
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

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult SearchHangHoaByText(Guid id_donvi, string txtSearch, int ConTonKho = 0)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                List<DM_HangHoaSearch> listTon = new List<DM_HangHoaSearch>();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (!string.IsNullOrWhiteSpace(txtSearch))
                {
                    char[] whitespace = new char[] { ' ', '\t' };
                    string[] textFilter = txtSearch.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                    string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    Guid ID_ChiNhanh = id_donvi;
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                    paramlist.Add(new SqlParameter("Search", string.Join(",", utf)));
                    paramlist.Add(new SqlParameter("SearchCoDau", string.Join(",", utf8)));
                    paramlist.Add(new SqlParameter("ConTonKho", ConTonKho));
                    listTon = db.Database.SqlQuery<DM_HangHoaSearch>("exec GetListHHSearch @ID_ChiNhanh, @Search, @SearchCoDau, @ConTonKho", paramlist.ToArray()).ToList();
                }
                listTon = listTon.Select(p => new DM_HangHoaSearch
                {
                    ID_DonViQuiDoi = p.ID_DonViQuiDoi,
                    ID = p.ID,
                    MaHangHoa = p.MaHangHoa,
                    TenHangHoa = p.TenHangHoa,
                    ThuocTinh_GiaTri = p.ThuocTinh_GiaTri,
                    TenDonViTinh = p.TenDonViTinh,
                    QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                    TyLeChuyenDoi = p.TyLeChuyenDoi,
                    LaHangHoa = p.LaHangHoa,
                    GiaVon = p.GiaVon,
                    GiaBan = p.GiaBan,
                    GiaNhap = p.GiaNhap,
                    TonKho = p.TonKho,
                    SrcImage = p.SrcImage,
                    ID_LoHang = p.ID_LoHang,
                    MaLoHang = p.MaLoHang,
                    NgaySanXuat = p.NgaySanXuat,
                    NgayHetHan = p.NgayHetHan,
                    DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == p.ID && ct.Xoa != true).Select(x => new DonViTinh
                    {
                        ID_HangHoa = p.ID,
                        TenDonViTinh = x.TenDonViTinh,
                        ID_DonViQuiDoi = x.ID,
                        QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                        Xoa = true,
                        TyLeChuyenDoi = x.TyLeChuyenDoi
                    }).ToList(),
                }).ToList();
                if (listTon != null)
                {
                    return Json(listTon.Where(p => p.LaHangHoa == true).Select(p => new
                    {
                        label = p.TenHangHoa,
                        value = p.MaHangHoa,
                        actual = p.MaHangHoa,
                        data = p
                    }).AsEnumerable());
                }
                else
                {
                    return Json(new List<jqAutoResultNH>());
                }
            }
        }
        /// <summary>
        /// ConTonKho: 0.Không check tồn kho, 1. Có check Tồn kho > 0
        /// </summary>
        /// <param name="id_donvi"></param>
        /// <param name="txtSearch"></param>
        /// <param name="ConTonKho"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<jqAutoResultNH> SearchHangHoaByTextCTG(Guid id_donvi, string txtSearch, int ConTonKho = 0)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<DM_HangHoaSearch> listTon = new List<DM_HangHoaSearch>();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (!string.IsNullOrWhiteSpace(txtSearch))
                {
                    char[] whitespace = new char[] { ' ', '\t' };
                    string[] textFilter = txtSearch.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                    string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    Guid ID_ChiNhanh = id_donvi;
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                    paramlist.Add(new SqlParameter("Search", string.Join(",", utf)));
                    paramlist.Add(new SqlParameter("SearchCoDau", string.Join(",", utf8)));
                    paramlist.Add(new SqlParameter("ConTonKho", ConTonKho));
                    listTon = db.Database.SqlQuery<DM_HangHoaSearch>("exec GetListHHSearch @ID_ChiNhanh, @Search, @SearchCoDau, @ConTonKho", paramlist.ToArray()).ToList();
                }

                return listTon.Select(p => new jqAutoResultNH
                {
                    label = p.TenHangHoa,
                    value = p.MaHangHoa,
                    actual = p.ID_DonViQuiDoi.ToString(),
                    data = p
                }).ToList();
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<jqAutoResultNH> SearchHangHoaByTextTP(Guid id_donvi, string txtSearch, int ConTonKho = 0)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<DM_HangHoaSearch> listTon = new List<DM_HangHoaSearch>();
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (!string.IsNullOrWhiteSpace(txtSearch))
                {
                    char[] whitespace = new char[] { ' ', '\t' };
                    string[] textFilter = txtSearch.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLower().Split(whitespace);
                    string[] utf8 = textFilter.Where(o => o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    string[] utf = textFilter.Where(o => !o.Any(c => StaticVariable.VietnameseSigns.ToList().Contains(c.ToString()))).ToArray();
                    Guid ID_ChiNhanh = id_donvi;
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                    paramlist.Add(new SqlParameter("Search", string.Join(",", utf)));
                    paramlist.Add(new SqlParameter("SearchCoDau", string.Join(",", utf8)));
                    paramlist.Add(new SqlParameter("ConTonKho", ConTonKho));
                    listTon = db.Database.SqlQuery<DM_HangHoaSearch>("exec GetListHHSearch @ID_ChiNhanh, @Search, @SearchCoDau, @ConTonKho", paramlist.ToArray()).ToList();
                }
                if (listTon.Where(p => p.LaHangHoa == true && p.QuanLyTheoLoHang == false).Count() > 0)
                {
                    return listTon.Where(p => p.LaHangHoa == true && p.QuanLyTheoLoHang == false).Select(p => new jqAutoResultNH
                    {
                        label = p.TenHangHoa,
                        value = p.MaHangHoa,
                        actual = p.ID_DonViQuiDoi.ToString(),
                        data = p
                    }).ToList();
                }
                else
                {
                    return new List<jqAutoResultNH>();
                }
            }
        }

        public double GetPageCountHoaDon(float pageSize)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                var totalRecord = _classDMHH.Gets(null).Count();
                // round 6.1 --> 7
                return System.Math.Ceiling(totalRecord / pageSize);
            }
        }

        public double GetTotalRecord()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.Gets(null).Count();
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas1(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                IQueryable<DM_HangHoa> lstAllHHs = _classDMHH.Gets(null);
                if (id == null)
                {
                    lstAllHHs = _classDMHH.Gets(null);
                }
                else
                {
                    lstAllHHs = _classDMHH.Gets(gr => gr.ID_NhomHang == id);
                }
                List<DM_HangHoaDTO> lsrReturns = new List<DM_HangHoaDTO>();
                if (lstAllHHs != null && lstAllHHs.Count() > 0)
                {

                    foreach (DM_HangHoa item in lstAllHHs)
                    {
                        string strMaHH = "";
                        double DGia = 0;
                        Guid dvtc = new Guid();
                        string tendonvitinhchuan = "";
                        int SLuong = 1;
                        DonViQuiDoi dvtctemp = item.DonViQuiDois.Where(p => p.LaDonViChuan == true).FirstOrDefault();
                        if (dvtctemp != null)
                        {
                            strMaHH = dvtctemp.MaHangHoa;
                            DGia = dvtctemp.GiaBan;
                            dvtc = dvtctemp.ID;
                            tendonvitinhchuan = dvtctemp.TenDonViTinh;
                        }
                        DM_HangHoaDTO itemData = new DM_HangHoaDTO
                        {
                            ID = item.ID,
                            TenHangHoa = item.TenHangHoa,
                            MaHangHoa = strMaHH,
                            NhomHangHoa = item.DM_NhomHangHoa != null ? item.DM_NhomHangHoa.TenNhomHangHoa : "",
                            PhanLoaiHangHoa = item.DM_PhanLoaiHangHoaDichVu != null ? item.DM_PhanLoaiHangHoaDichVu.TenPhanLoai : "",
                            GiaBan = DGia,
                            GiaVon = 0,
                            TonKho = 0,
                            TrangThai = item.TheoDoi,
                            sLoaiHangHoa = item.LaHangHoa != null ? item.LaHangHoa.Value == true ? "Hàng hóa" : "Dịch vụ" : "Combo - đóng gói",
                            DonViTinhChuan = tendonvitinhchuan,
                            LaHangHoa = item.LaHangHoa,
                            SoLuong = SLuong,
                            GiamGia = 0,
                            ThanhTien = DGia * SLuong,
                            ID_DonViQuiDoi = dvtc,
                            DonViTinh = item.DonViQuiDois.OrderByDescending(p => p.LaDonViChuan).ThenBy(p => p.TenDonViTinh).Select(p => new DonViTinh
                            {
                                MaHangHoa = p.MaHangHoa,
                                TenDonViTinh = p.TenDonViTinh,
                                ID = item.ID
                            }).ToList(),
                        };
                        lsrReturns.Add(itemData);
                    }
                }
                return lsrReturns;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool CheckMaLoHangTrung(string malohang, Guid idhanghoa)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                var _classDMLH = new classDM_LoHang(db);
                return _classDMLH.CheckMaLoHangTrung(malohang, idhanghoa);
            }
        }

        public bool Check_MaHangHoaExist(string maHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.Check_MaHangHoaExist(maHangHoa);
            }
        }

        public bool Check_TenDVTExist(string tendvt)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.Check_TenDVTExist(tendvt);
            }
        }

        public bool Check_MaHangHoaExistDVT(string maHangHoa, Guid idhangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.Check_MaHangHoaExistDVT(maHangHoa, idhangHoa);
            }
        }

        public bool Check_HangHoaLaThanhPhanDichVu(Guid idhanghoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.Check_HangHoaLaThanhPhanDichVu(idhanghoa);
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoi(Guid idBangGia)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                var _ClassGiaBanCT = new classDM_GiaBan_ChiTiet(db);
                List<GiaBanChiTietDTO> lstGB = _ClassGiaBanCT.SelectChiTiet(idBangGia.ToString()).ToList();
                List<DM_HangHoaDTO> lstHH = new List<DM_HangHoaDTO>();
                foreach (GiaBanChiTietDTO item in lstGB)
                {
                    DM_HangHoaDTO dmHH = new DM_HangHoaDTO();
                    dmHH.ID_DonViQuiDoi = item.IDQuyDoi ?? Guid.Empty;
                    dmHH.TenHangHoa = item.TenHangHoa;
                    dmHH.MaHangHoa = item.MaHangHoa;
                    dmHH.SoLuong = 1;
                    dmHH.GiaBan = item.GiaMoi;
                    dmHH.GiaVon = item.GiaMoi;
                    dmHH.ThanhTien = item.GiaMoi; // 1* DonGia
                    dmHH.GiamGia = 0;
                    dmHH.TenDonViTinh = item.DonViTinh;
                    dmHH.ID_NhomHangHoa = item.ID_NhomHang;
                    lstHH.Add(dmHH);
                }
                return lstHH;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IEnumerable<SP_GiaBanChiTietDTO> SelectAll_GBChiTiet()
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                var _ClassGiaBanCT = new classDM_GiaBan_ChiTiet(db);
                try
                {
                    return _ClassGiaBanCT.SP_GettAll_BangGiaChiTiet(); ;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("DM_HangHoaAPI_SelectAll_GBChiTiet: " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IQueryable<Object> SelectAll_GBChiTiet_IQueryable()
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                var _ClassGiaBanCT = new classDM_GiaBan_ChiTiet(db);
                try
                {
                    return _ClassGiaBanCT.SelectAll_GBChiTiet();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("DM_HangHoaAPI_SelectAll_GBChiTiet_IQueryable: " + ex.InnerException + ex.Message);
                    return null;
                }
            }
        }

        public System.Web.Http.Results.JsonResult<JsonResultExampleHH> GetListHangHoaKiemKho(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                var lst = _classDMHH.GetListHangHoaKiemKho(iddonvi);
                JsonResultExampleHH jsonobj = new JsonResultExampleHH
                {
                    lstHH = lst,
                };

                return Json(jsonobj);
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoiNH(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetListHangHoas_QuyDoiNH(iddonvi);
            }
        }
        [HttpGet]
        [Compress.DeflateCompression]
        public IEnumerable<SP_DM_HangHoaDTO> GetListHangHoas_QuyDoiNH_Anh_IEnumerable(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.Sp_GetListHangHoas_QuyDoiNH_IEnumerable(iddonvi);
            }
        }

        [HttpGet, HttpPost]
        [Compress.DeflateCompression]
        public IHttpActionResult Gara_JqAutoHangHoa(Gara_ParamSearchHangHoa param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SP_DM_HangHoaDTO> data = new ClassDM_HangHoa(db).Gara_JqAutoHangHoa(param);

                    var arrIDHangHoa = data.Select(x => x.ID).ToList();
                    List<DonViTinh> lstDVT = db.DonViQuiDois.Where(x => arrIDHangHoa.Contains(x.ID_HangHoa) && x.Xoa != true)
                        .Select(x => new DonViTinh
                        {
                            ID_HangHoa = x.ID_HangHoa,
                            TenDonViTinh = x.TenDonViTinh,
                            ID_DonViQuiDoi = x.ID,
                            Xoa = false,
                            TyLeChuyenDoi = x.TyLeChuyenDoi
                        }).ToList();

                    var lst = data.Select(p => new
                    {
                        p.ID_DonViQuiDoi,
                        p.TyLeChuyenDoi,
                        p.ID,
                        p.MaHangHoa,
                        p.TenHangHoa,
                        p.ThuocTinh_GiaTri,
                        p.TenDonViTinh,
                        p.QuanLyTheoLoHang,
                        p.LaHangHoa,
                        p.GiaVon,
                        p.GiaBan,
                        p.TonKho,
                        p.SrcImage,
                        p.ID_LoHang,
                        p.MaLoHang,
                        p.NgaySanXuat,
                        p.NgayHetHan,
                        p.QuyCach,
                        p.DonViTinhQuyCach,
                        p.PhiDichVu,
                        p.LaPTPhiDichVu,
                        p.LaDonViChuan,
                        p.ID_NhomHangHoa,
                        p.TenNhomHangHoa,
                        p.ThoiGianBaoHanh,
                        p.LoaiBaoHanh,
                        p.SoPhutThucHien,
                        p.GhiChuHH,
                        p.DichVuTheoGio,
                        p.DuocTichDiem,
                        p.TonToiThieu,// used to check thong bao tonkho
                        p.LoaiHangHoa,
                        p.HoaHongTruocChietKhau,
                        DonViTinh = lstDVT.Where(ct => ct.ID_HangHoa == p.ID)
                            .Select(x => new DonViTinh
                            {
                                ID_HangHoa = p.ID,
                                TenDonViTinh = x.TenDonViTinh,
                                ID_DonViQuiDoi = x.ID,
                                QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                                Xoa = false,
                                TyLeChuyenDoi = x.TyLeChuyenDoi
                            }).ToList(),
                    }).ToList();
                    return ActionTrueData(lst);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [HttpGet, HttpPost]
        [Compress.DeflateCompression]
        public IHttpActionResult Gara_GetListHangHoa_ByIDQuiDoi(Gara_ParamSearchHangHoa param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<SP_DM_HangHoaDTO> data = new ClassDM_HangHoa(db).Gara_GetListHangHoa_ByIDQuiDoi(param);
                    return ActionTrueData(data);
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.ToString());
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetTonKho_byIDQuyDois(KiemKhoParamSearch param)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<KiemKho_HangHoaTonKho> data = new ClassDM_HangHoa(db).GetTonKho_byIDQuyDois(param);
                    return Json(new { res = true, data });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex });
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoiBangGia(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetListHangHoas_QuyDoiBangGia(iddonvi);
            }
        }

        public JsonResult<JSONTheKho> GetListTheKho(int currentPage, int pageSize, Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_TheKhoDTO> lstreturn = _classDMHH.GetListTheKho(id, iddonvi).ToList();
                int totalRecords = lstreturn.Count();
                lstreturn = lstreturn.Skip(currentPage * pageSize).Take(pageSize).ToList();
                JSONTheKho json = new JSONTheKho
                {
                    lst = lstreturn.ToList(),
                    Rowcount = totalRecords,
                    pageCount = System.Math.Ceiling(totalRecords * 1.0 / pageSize),
                };
                return Json(json);
            }
        }

        public JsonResult<JSONTheKho> GetListTheKhoByMaLoHang(int currentPage, int pageSize, Guid idlohang, Guid iddonvi, Guid idhanghoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_TheKhoDTO> lstreturn = _classDMHH.GetListTheKhoByMaLoHang(idlohang, iddonvi, idhanghoa).ToList();
                int totalRecords = lstreturn.Count();
                lstreturn = lstreturn.Skip(currentPage * pageSize).Take(pageSize).ToList();
                JSONTheKho json = new JSONTheKho
                {
                    lst = lstreturn.ToList(),
                    Rowcount = totalRecords,
                    pageCount = System.Math.Ceiling(totalRecords * 1.0 / pageSize),
                };
                return Json(json);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<ListTonTheoLoHang> ListTonTheoLoHang(Guid iddonvi, Guid idhanghoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                DateTime timeEnd = DateTime.Now;
                Guid ID_ChiNhanh = iddonvi;
                Guid ID_HangHoa = idhanghoa;

                paramlist.Add(new SqlParameter("timeEnd", timeEnd));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
                List<ListTonTheoLoHang> listTon = db.Database.SqlQuery<ListTonTheoLoHang>("exec GetListTonTheoLoHangHoa @timeEnd, @ID_ChiNhanh,@ID_HangHoa", paramlist.ToArray()).ToList();
                return listTon;
            }
        }

        public List<List_TonKho> GetListTonKho(Guid id, Guid? idnhanvien)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                var _classDV = new classDM_DonVi(db);
                List<List_TonKho> lstTon = new List<List_TonKho>();
                List<DM_DonViDTO> lst = _classDV.getListDVByIDNguoiDung(idnhanvien).OrderBy(p => p.TenDonVi).ToList();
                foreach (var item in lst)
                {
                    List_TonKho ton = new List_TonKho();
                    ton.TenDonVi = item.TenDonVi;
                    ton.TonKho = _classDMHH.TinhSLTonHH(id, item.ID).Value;
                    lstTon.Add(ton);
                }
                return lstTon;
            }
        }

        public List<DM_LoHangDTO> GetListLoHang(Guid idhanghoa)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                var _classDMLH = new classDM_LoHang(db);
                return _classDMLH.getlistLoHang(idhanghoa).ToList();
            }
        }

        public List<DM_HangHoa_Anh> GetListAnh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_HangHoa_Anh> lstAnh = new List<DM_HangHoa_Anh>();
                List<DM_HangHoa_Anh> objAnh = _classDMHH.GetsAnh(p => p.ID_HangHoa == id);
                foreach (var item in objAnh)
                {
                    DM_HangHoa_Anh anh = new DM_HangHoa_Anh();
                    anh.ID = item.ID;
                    anh.URLAnh = item.URLAnh;
                    lstAnh.Add(anh);
                }
                return lstAnh;
            }
        }

        public List<DM_HangHoaDTO> GetListHangHoas_QuyDoiTH(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetListHangHoas_QuyDoiTH(iddonvi);
            }
        }

        [HttpGet]
        public IHttpActionResult GetDinhLuongDV_FromIDQuyDoi(Guid idQuiDoi, Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var data = (from dl in db.DinhLuongDichVus
                                join qd in db.DonViQuiDois on dl.ID_DonViQuiDoi equals qd.ID
                                join tp in db.DM_HangHoa on qd.ID_HangHoa equals tp.ID
                                join nhom in db.DM_NhomHangHoa on tp.ID_NhomHang equals nhom.ID
                                join lo in db.DM_LoHang on dl.ID_LoHang equals lo.ID into TPLo
                                from tplo in TPLo.DefaultIfEmpty()
                                join tk in db.DM_HangHoa_TonKho
                                on new
                                {
                                    ID_DonViQuyDoi = dl.ID_DonViQuiDoi,
                                    ID_LoHang = dl.ID_LoHang,
                                    ID_DonVi = idDonVi
                                }
                                equals new
                                {
                                    tk.ID_DonViQuyDoi,
                                    tk.ID_LoHang,
                                    tk.ID_DonVi
                                }
                                into HH_TonKho
                                from hhTK in HH_TonKho.DefaultIfEmpty()
                                where dl.ID_DichVu == idQuiDoi
                                select new
                                {
                                    dl.ID_DichVu,
                                    dl.ID_DonViQuiDoi,
                                    dl.ID_LoHang,
                                    dl.SoLuong,
                                    dl.GhiChu,
                                    qd.ID_HangHoa,
                                    qd.MaHangHoa,
                                    MaLoHang = tplo != null ? tplo.MaLoHang : string.Empty,
                                    GiaBan = dl.DonGia ?? qd.GiaBan,
                                    qd.TenDonViTinh,
                                    qd.LaDonViChuan,
                                    qd.TyLeChuyenDoi,
                                    ThuocTinh_GiaTri = qd.ThuocTinhGiaTri,
                                    tp.TenHangHoa,
                                    tp.LaHangHoa,
                                    tp.LoaiHangHoa,
                                    tp.QuanLyBaoDuong,
                                    tp.QuanLyTheoLoHang,
                                    tp.SoPhutThucHien,
                                    tp.ThoiGianBaoHanh,
                                    tp.LoaiBaoHanh,
                                    PhiDichVu = tp.ChiPhiThucHien,
                                    LaPTPhiDichVu = tp.ChiPhiTinhTheoPT,
                                    ID_NhomHangHoa = tp.ID_NhomHang,
                                    nhom.TenNhomHangHoa,
                                    tp.QuyCach,
                                    tp.DuocTichDiem,
                                    tp.DichVuTheoGio,
                                    TonKho = hhTK != null ? hhTK.TonKho : 0,
                                    HoaHongTruocChietKhau = tp.HoaHongTruocChietKhau != null ? tp.HoaHongTruocChietKhau : 0,
                                    ChietKhauMD_NV = tp.ChietKhauMD_NV != null ? tp.ChietKhauMD_NV : 0,
                                    ChietKhauMD_NVTheoPT = tp.ChietKhauMD_NVTheoPT != null ? tp.ChietKhauMD_NVTheoPT : true,
                                }).ToList();
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        // GET: api/DM_HangHoaAPI/5
        [ResponseType(typeof(DM_HangHoaDTO))]
        public IHttpActionResult GetDM_HangHoa(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                DM_HangHoa dM_HangHoa = db.DM_HangHoa.Where(p => p.ID == id).FirstOrDefault();
                DM_HangHoaDTO ct = new DM_HangHoaDTO();
                ct.ID = dM_HangHoa.ID;
                ct.ID_HangHoaCungLoai = dM_HangHoa.ID_HangHoaCungLoai;
                ct.TenHangHoa = dM_HangHoa.TenHangHoa;
                ct.TenNhomHangHoa = dM_HangHoa.ID_NhomHang != null ? db.DM_NhomHangHoa.Where(p => p.ID == dM_HangHoa.ID_NhomHang).FirstOrDefault().TenNhomHangHoa : "";
                ct.ID_NhomHangHoa = dM_HangHoa.ID_NhomHang;
                ct.LaHangHoa = dM_HangHoa.LaHangHoa;
                ct.GhiChu = dM_HangHoa.GhiChu;
                ct.QuyCach = dM_HangHoa.QuyCach;
                ct.TonToiDa = dM_HangHoa.TonToiDa;
                ct.TonToiThieu = dM_HangHoa.TonToiThieu;
                ct.DuocBanTrucTiep = dM_HangHoa.DuocBanTrucTiep;
                ct.QuanLyTheoLoHang = dM_HangHoa.QuanLyTheoLoHang;
                ct.LaChaCungLoai = dM_HangHoa.LaChaCungLoai;
                ct.ThoiGianBaoHanh = dM_HangHoa.ThoiGianBaoHanh;
                ct.LoaiBaoHanh = dM_HangHoa.LoaiBaoHanh;
                ct.DonViTinhQuyCach = dM_HangHoa.DonViTinhQuyCach;
                ct.ChiPhiThucHien = dM_HangHoa.ChiPhiThucHien;
                ct.ChiPhiTinhTheoPT = dM_HangHoa.ChiPhiTinhTheoPT;
                ct.SoPhutThucHien = dM_HangHoa.SoPhutThucHien;
                ct.DichVuTheoGio = dM_HangHoa.DichVuTheoGio; //todo dv theogio
                ct.DuocTichDiem = dM_HangHoa.DuocTichDiem;
                ct.QuanLyBaoDuong = dM_HangHoa.QuanLyBaoDuong ?? 0;
                ct.LoaiHangHoa = dM_HangHoa.LoaiHangHoa == null ? dM_HangHoa.LaHangHoa == true ? 1 : 2 : dM_HangHoa.LoaiHangHoa;
                ct.LoaiBaoDuong = dM_HangHoa.LoaiBaoDuong ?? 0;
                ct.SoKmBaoHanh = dM_HangHoa.SoKmBaoHanh ?? 0;
                ct.ID_Xe = dM_HangHoa.ID_Xe;  
                ct.ChietKhauMD_NV = dM_HangHoa.ChietKhauMD_NV ?? 0;
                ct.ChietKhauMD_NVTheoPT = dM_HangHoa.ChietKhauMD_NVTheoPT??true;

                DonViQuiDoi dm_DVQD = dM_HangHoa.DonViQuiDois.Where(p => p.LaDonViChuan == true && p.Xoa != true).FirstOrDefault();
                DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == dm_DVQD.ID && p.ID_DonVi == iddonvi && p.ID_LoHang == null).FirstOrDefault();
                if (dm_DVQD != null)
                {
                    ct.ID = dm_DVQD.ID_HangHoa;
                    ct.MaHangHoa = dm_DVQD.MaHangHoa;
                    ct.GiaBan = dm_DVQD.GiaBan;
                    ct.GiaVon = dM_HangHoa.LaHangHoa == true ? ((dmgv != null && (dM_HangHoa.QuanLyTheoLoHang == false || dM_HangHoa.QuanLyTheoLoHang == null)) ? Math.Round(dmgv.GiaVon, 2, MidpointRounding.ToEven) : 0) : Math.Round(dm_DVQD.GiaVon, 2, MidpointRounding.ToEven);
                    ct.TonKho = _classDMHH.TinhSLTonHH(dM_HangHoa.ID, iddonvi);
                    ct.DonViTinhChuan = dm_DVQD.TenDonViTinh;
                    ct.ID_DonViQuiDoi = dm_DVQD.ID;
                    ct.ID_Xe = dM_HangHoa.ID_Xe;
                }

                // get thanhphan dinhluong at js
                List<DinhLuongDichVuDTO> dldv = new List<DinhLuongDichVuDTO>();


                List<DonViTinh> lqd = new List<DonViTinh>();
                List<DonViQuiDoi> lqdtemp = db.DonViQuiDois.Where(p => p.ID_HangHoa == ct.ID).Where(p => p.LaDonViChuan == false && p.Xoa != true).ToList();
                foreach (var item in lqdtemp)
                {
                    DonViTinh hhqd = new DonViTinh();
                    hhqd.ID = item.ID;
                    hhqd.ID_HangHoa = item.ID_HangHoa;
                    hhqd.GiaBan = item.GiaBan;
                    hhqd.MaHangHoa = item.MaHangHoa;
                    hhqd.TenDonViTinh = item.TenDonViTinh;
                    hhqd.TyLeChuyenDoi = item.TyLeChuyenDoi;
                    lqd.Add(hhqd);
                }
                List<HangHoa_ThuocTinh> hhtt = new List<HangHoa_ThuocTinh>();
                hhtt.AddRange(dM_HangHoa.HangHoa_ThuocTinh.Where(p => p.ID_HangHoa == dM_HangHoa.ID).OrderBy(p => p.ThuTuNhap).Select(p => new HangHoa_ThuocTinh
                {
                    ID = p.ID,
                    ID_HangHoa = p.ID_HangHoa,
                    ID_ThuocTinh = p.ID_ThuocTinh,
                    GiaTri = p.GiaTri,
                    ThuTuNhap = p.ThuTuNhap,
                    index = p.ThuTuNhap.Value
                }));

                List<DM_HangHoa_AnhDTO> lstAnh = new List<DM_HangHoa_AnhDTO>();
                lstAnh.AddRange(dM_HangHoa.DM_HangHoa_Anh.Where(p => p.ID_HangHoa == dM_HangHoa.ID).OrderBy(p => p.SoThuTu).Select(p => new DM_HangHoa_AnhDTO
                {
                    ID = p.ID,
                    URLAnh = p.URLAnh
                }));

                List<DM_HangHoa_ViTriDTO> lstViTriHH = new List<DM_HangHoa_ViTriDTO>();
                lstViTriHH.AddRange(dM_HangHoa.DM_ViTriHangHoa.Where(p => p.ID_HangHoa == dM_HangHoa.ID).Select(p => new DM_HangHoa_ViTriDTO
                {
                    ID = p.ID_ViTri,
                    TenViTri = db.DM_HangHoa_ViTri.Where(c => c.ID == p.ID_ViTri).FirstOrDefault().TenViTri
                }));
                ct.DM_HangHoa_Anh = lstAnh;
                ct.DonViTinh = lqd;
                ct.DinhLuongDichVu = dldv;
                ct.HangHoa_ThuocTinh = hhtt;
                ct.DM_HangHoa_ViTri = lstViTriHH;
                if (dM_HangHoa == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        [ResponseType(typeof(DM_HangHoaDTO))]
        public IHttpActionResult GetDM_HangHoaTP(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                DonViQuiDoi dvqd = db.DonViQuiDois.Where(p => p.ID == id).FirstOrDefault();
                DM_HangHoa dM_HangHoa = db.DM_HangHoa.Where(p => p.ID == dvqd.ID_HangHoa).FirstOrDefault();
                DM_HangHoaDTO ct = new DM_HangHoaDTO();
                ct.ID = dM_HangHoa.ID;
                ct.ID_HangHoaCungLoai = dM_HangHoa.ID_HangHoaCungLoai;
                ct.TenHangHoa = dM_HangHoa.TenHangHoa;
                ct.TenNhomHangHoa = dM_HangHoa.ID_NhomHang != null ? db.DM_NhomHangHoa.Where(p => p.ID == dM_HangHoa.ID_NhomHang).FirstOrDefault().TenNhomHangHoa : "";
                ct.ID_NhomHangHoa = dM_HangHoa.ID_NhomHang;
                ct.LaHangHoa = dM_HangHoa.LaHangHoa;
                ct.GhiChu = dM_HangHoa.GhiChu;
                ct.QuyCach = dM_HangHoa.QuyCach == null || dM_HangHoa.QuyCach == 0 ? 1 : dM_HangHoa.QuyCach;
                ct.TonToiDa = dM_HangHoa.TonToiDa;
                ct.TonToiThieu = dM_HangHoa.TonToiThieu;
                ct.DuocBanTrucTiep = dM_HangHoa.DuocBanTrucTiep;
                ct.QuanLyTheoLoHang = dM_HangHoa.QuanLyTheoLoHang;
                ct.SoLuongQuyCach = dM_HangHoa.QuyCach == null || dM_HangHoa.QuyCach == 0 ? 1 : dM_HangHoa.QuyCach * dvqd.TyLeChuyenDoi;
                DonViQuiDoi dm_DVQD = dM_HangHoa.DonViQuiDois.Where(p => p.ID == id && p.Xoa != true).FirstOrDefault();
                DM_GiaVon dmgv = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == dm_DVQD.ID && p.ID_DonVi == iddonvi && p.ID_LoHang == null).FirstOrDefault();
                if (dM_HangHoa.DonViQuiDois != null && dm_DVQD != null)
                {
                    ct.ID = dm_DVQD.ID_HangHoa;
                    ct.MaHangHoa = dm_DVQD.MaHangHoa;
                    ct.GiaBan = dm_DVQD.GiaBan;
                    ct.GiaVon = (dmgv != null && dM_HangHoa.QuanLyTheoLoHang == false) ? Math.Round(dmgv.GiaVon, MidpointRounding.ToEven) : 0;
                    ct.TonKho = _classDMHH.TinhSLTonHH(dM_HangHoa.ID, iddonvi);
                    ct.DonViTinhChuan = dm_DVQD.TenDonViTinh;
                    ct.ID_DonViQuiDoi = dm_DVQD.ID;
                }

                List<DinhLuongDichVuDTO> dldv = new List<DinhLuongDichVuDTO>();
                List<DinhLuongDichVu> dluong = db.DinhLuongDichVus.Where(p => p.ID_DichVu == dm_DVQD.ID).ToList();
                foreach (var item in dluong)
                {
                    DinhLuongDichVuDTO dinhluongDV = new DinhLuongDichVuDTO();
                    dinhluongDV.ID = item.ID;
                    dinhluongDV.ID_DichVu = item.ID_DichVu;
                    dinhluongDV.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                    DonViQuiDoi dvqdtmp = db.DonViQuiDois.Where(p => p.ID == item.ID_DonViQuiDoi).FirstOrDefault();
                    DM_GiaVon dmgv1 = db.DM_GiaVon.Where(p => p.ID_DonViQuiDoi == dvqdtmp.ID && p.ID_DonVi == iddonvi && p.ID_LoHang == null).FirstOrDefault();
                    dinhluongDV.MaHangHoa = dvqdtmp.MaHangHoa;
                    dinhluongDV.TenHangHoa = db.DM_HangHoa.Where(p => p.ID == dvqdtmp.ID_HangHoa).FirstOrDefault().TenHangHoa;
                    dinhluongDV.GiaVon = dmgv1 != null ? Math.Round(dmgv1.GiaVon, MidpointRounding.ToEven) : 0;
                    dinhluongDV.SoLuong = item.SoLuong;
                    dinhluongDV.ThanhTien = dinhluongDV.GiaVon * item.SoLuong;
                    dldv.Add(dinhluongDV);
                }

                List<DonViTinh> lqd = new List<DonViTinh>();
                List<DonViQuiDoi> lqdtemp = db.DonViQuiDois.Where(p => p.ID_HangHoa == ct.ID).Where(p => p.LaDonViChuan == false && p.Xoa != true).ToList();
                foreach (var item in lqdtemp)
                {
                    DonViTinh hhqd = new DonViTinh();
                    hhqd.ID = item.ID;
                    hhqd.ID_HangHoa = item.ID_HangHoa;
                    hhqd.GiaBan = item.GiaBan;
                    hhqd.MaHangHoa = item.MaHangHoa;
                    hhqd.TenDonViTinh = item.TenDonViTinh;
                    hhqd.TyLeChuyenDoi = item.TyLeChuyenDoi;
                    lqd.Add(hhqd);
                }
                List<HangHoa_ThuocTinh> hhtt = new List<HangHoa_ThuocTinh>();
                hhtt.AddRange(dM_HangHoa.HangHoa_ThuocTinh.Where(p => p.ID_HangHoa == dM_HangHoa.ID).OrderBy(p => p.ThuTuNhap).Select(p => new HangHoa_ThuocTinh
                {
                    ID = p.ID,
                    ID_HangHoa = p.ID_HangHoa,
                    ID_ThuocTinh = p.ID_ThuocTinh,
                    GiaTri = p.GiaTri,
                    ThuTuNhap = p.ThuTuNhap,
                    index = p.ThuTuNhap.Value
                    //DM_ThuocTinh = p.DM_ThuocTinh
                }));

                List<DM_HangHoa_AnhDTO> lstAnh = new List<DM_HangHoa_AnhDTO>();
                lstAnh.AddRange(dM_HangHoa.DM_HangHoa_Anh.Where(p => p.ID_HangHoa == dM_HangHoa.ID).Select(p => new DM_HangHoa_AnhDTO
                {
                    ID = p.ID,
                    URLAnh = p.URLAnh
                }));
                ct.DM_HangHoa_Anh = lstAnh;
                ct.DonViTinh = lqd;
                ct.DinhLuongDichVu = dldv;
                ct.HangHoa_ThuocTinh = hhtt;
                if (dM_HangHoa == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        [ResponseType(typeof(DM_LoHangDTO))]
        public IHttpActionResult GetDM_LoHang(Guid id)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                var _classDMLH = new classDM_LoHang(db);
                DM_LoHang dmlo = _classDMLH.Select_LoHang(id);
                DM_LoHangDTO ct = new DM_LoHangDTO();
                ct.ID = dmlo.ID;
                ct.MaLoHang = dmlo.MaLoHang;
                ct.NgaySanXuat = dmlo.NgaySanXuat;
                ct.NgayHetHan = dmlo.NgayHetHan;
                ct.TrangThai = dmlo.TrangThai;
                if (dmlo == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        //Giới hạn số mặt hàng
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool GioiHanSoMatHang()
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            int gioihan = CuaHangDangKyService.GetGioiHanMatHang(str);
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var lst = from hh in db.DM_HangHoa
                          join dvqd in db.DonViQuiDois on hh.ID equals dvqd.ID_HangHoa
                          where dvqd.Xoa == false
                          select new DM_HangHoaDTO
                          {
                              ID = hh.ID
                          };
                if (gioihan == 0)
                {
                    return false;
                }
                else
                {
                    if (lst.Count() >= gioihan)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public int number_GioiHanSoMatHang()
        {
            string str = CookieStore.GetCookieAes("SubDomain");
            int gioihan = CuaHangDangKyService.GetGioiHanMatHang(str);
            return gioihan;
        }

        public IHttpActionResult GetAllDinhLuongDichVu(Guid iddvqd, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);
                if (db == null)
                {
                    return ActionFalseNotData("Chưa kết nối DB");
                }
                else
                {
                    var data = classHangHoa.SP_GetInfor_TPDinhLuong(iddonvi, iddvqd);
                    if (data != null && data.Count > 0)
                    {
                        return ActionTrueData(data);
                    }
                    return ActionFalseNotData(string.Empty);
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetDM_HangHoa_ThuocTinh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                DM_HangHoa dM_HangHoa = _classDMHH.Select_HangHoa(id);
                List<HangHoa_ThuocTinhDTO> hhtt = new List<HangHoa_ThuocTinhDTO>();
                hhtt.AddRange(dM_HangHoa.HangHoa_ThuocTinh.Where(p => p.ID_HangHoa == dM_HangHoa.ID).OrderBy(p => p.ThuTuNhap).Select(p => new HangHoa_ThuocTinhDTO
                {
                    ID_ThuocTinh = p.ID_ThuocTinh,
                    GiaTri = p.GiaTri,
                    ThuTuNhap = p.ThuTuNhap,
                    TenThuocTinh = _classDMHH.Select_ThuocTinh(p.ID_ThuocTinh).TenThuocTinh,
                    checkboxChecked = true
                }));
                return Ok(hhtt);
            }
        }

        //Tính tồn hàng hóa theo lô hàng
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<DM_HangHoaDTO> TinhTonTheoLoHang(Guid idhanghoa, Guid id_lohang, Guid id_donvi, DateTime? ngaykk = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                if (ngaykk == null)
                {
                    ngaykk = DateTime.Now;
                }
                Guid ID_ChiNhanh = id_donvi;
                Guid ID_HangHoa = idhanghoa;
                Guid ID_LoHang = id_lohang;

                paramlist.Add(new SqlParameter("timeEnd", ngaykk));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("ID_LoHang", ID_LoHang));
                paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
                List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec TinhTonTheoLoHangHoa @timeEnd, @ID_ChiNhanh,@ID_LoHang, @ID_HangHoa", paramlist.ToArray()).ToList();
                //return listTon.Count > 0 ? listTon.FirstOrDefault().TonKho : 0;
                return listTon;
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public double? TonCuaHangHoaNotByLoHang(Guid idhanghoa, Guid id_donvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.TinhSLTonHH(idhanghoa, id_donvi);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public double? TonCuaHangHoaNotByLoHangKiemKho(Guid idhanghoa, Guid id_donvi, DateTime timeKK, Guid? idlohang)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                DateTime timeEnd = timeKK.AddSeconds(DateTime.Now.Second).AddMilliseconds(DateTime.Now.Millisecond);
                Guid ID_ChiNhanh = id_donvi;
                Guid ID_HangHoa = idhanghoa;
                Guid? ID_LoHang = idlohang;

                paramlist.Add(new SqlParameter("timeEnd", timeEnd));
                paramlist.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                paramlist.Add(new SqlParameter("ID_LoHang", idlohang == null ? (object)DBNull.Value : ID_LoHang));
                paramlist.Add(new SqlParameter("ID_HangHoa", ID_HangHoa));
                List<DM_HangHoaDTO> listTon = db.Database.SqlQuery<DM_HangHoaDTO>("exec TinhTonTheoLoHangHoa @timeEnd, @ID_ChiNhanh,@ID_LoHang, @ID_HangHoa", paramlist.ToArray()).ToList();
                return listTon.Count > 0 ? listTon.FirstOrDefault().TonKho : 0;
            }
        }

        //Thuộc tính
        [ResponseType(typeof(DM_ThuocTinh))]
        public IHttpActionResult GetDM_ThuocTinh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                DM_ThuocTinh dm_ThuocTinh = _classDMHH.Select_ThuocTinh(id);
                DM_ThuocTinh ct = new DM_ThuocTinh();
                ct.ID = id;
                ct.TenThuocTinh = dm_ThuocTinh.TenThuocTinh;
                if (dm_ThuocTinh == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<DM_HangHoa_ViTriDTO> getAllViTri()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var tbl = from vitri in db.DM_HangHoa_ViTri
                              select new DM_HangHoa_ViTriDTO
                              {
                                  ID = vitri.ID,
                                  TenViTri = vitri.TenViTri
                              };
                    if (tbl.ToList().Count() > 0)
                    {
                        return tbl.ToList();
                    }
                    else
                    {
                        return new List<DM_HangHoa_ViTriDTO>();
                    }
                }
                else
                {
                    return new List<DM_HangHoa_ViTriDTO>();
                }
            }
        }

        [ResponseType(typeof(HangHoa_ThuocTinh))]
        public IHttpActionResult GetHangHoa_ThuocTinh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                DM_HangHoa dM_HangHoa = _classDMHH.Select_HangHoa(id);
                DM_HangHoaDTO ct = new DM_HangHoaDTO();
                List<HangHoa_ThuocTinh> hhtt = new List<HangHoa_ThuocTinh>();
                hhtt.AddRange(dM_HangHoa.HangHoa_ThuocTinh.Where(p => p.ID_HangHoa == dM_HangHoa.ID).OrderBy(p => p.ThuTuNhap).Select(p => new HangHoa_ThuocTinh
                {
                    ID = p.ID,
                    ID_HangHoa = p.ID_HangHoa,
                    ID_ThuocTinh = p.ID_ThuocTinh,
                    GiaTri = p.GiaTri,
                    ThuTuNhap = p.ThuTuNhap
                }));
                ct.HangHoa_ThuocTinh = hhtt;
                if (dM_HangHoa == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<HangHoa_ThuocTinh> getGiaTriTTByID_ThuocTinh(Guid idthuoctinh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                List<HangHoa_ThuocTinh> lst = _classDMHH.GetHangHoaThuocTinh(idthuoctinh);
                return lst;
            }
        }

        [ResponseType(typeof(DM_HangHoaDTO))]
        public IHttpActionResult GetChoose_HangHoa(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                var id_hanghoa = _classDVQD.Get(p => p.ID == id).ID_HangHoa;
                DM_HangHoa dM_HangHoa = _classDMHH.Select_HangHoa(id_hanghoa);
                DM_HangHoaDTO ct = new DM_HangHoaDTO();
                ct.ID = dM_HangHoa.ID;
                ct.ID_HangHoaCungLoai = dM_HangHoa.ID_HangHoaCungLoai;
                ct.TenHangHoa = dM_HangHoa.TenHangHoa;
                ct.TheoDoi = dM_HangHoa.TheoDoi;
                ct.TonKho = Math.Round(_classDMHH.TinhSLTonHH(id_hanghoa, iddonvi).Value, 3, MidpointRounding.ToEven);
                ct.HangHoa_ThuocTinh = _classDMHH.SelectHangHoa_ThuocTinh(dM_HangHoa.ID).Select(x => new HangHoa_ThuocTinh
                {
                    GiaTri = x.GiaTri,
                    ThuTuNhap = x.ThuTuNhap
                }).OrderBy(p => p.ThuTuNhap).ToList();
                DonViQuiDoi dm_DVQD = _classDVQD.Get(p => p.ID == id && p.Xoa != true);
                if (dm_DVQD != null)
                {
                    ct.ID_DonViQuiDoi = dm_DVQD.ID;
                    ct.MaHangHoa = dm_DVQD.MaHangHoa;
                    ct.GiaBan = dm_DVQD.GiaBan;
                    ct.TenDonViTinh = dm_DVQD.TenDonViTinh;
                }
                if (dM_HangHoa == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        [ResponseType(typeof(DM_HangHoaDTO))]
        public IHttpActionResult GetChoose_HangHoaInTemKe(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                var id_hanghoa = _classDVQD.Get(p => p.ID == id).ID_HangHoa;
                DM_HangHoa dM_HangHoa = _classDMHH.Select_HangHoa(id_hanghoa);
                DM_HangHoaDTO ct = new DM_HangHoaDTO();
                ct.ID = dM_HangHoa.ID;
                ct.ID_HangHoaCungLoai = dM_HangHoa.ID_HangHoaCungLoai;
                ct.TenHangHoa = dM_HangHoa.TenHangHoa;
                ct.TheoDoi = dM_HangHoa.TheoDoi;
                ct.TonKho = Math.Round(_classDMHH.TinhSLTonHH(id_hanghoa, iddonvi).Value, 3, MidpointRounding.ToEven);
                ct.HangHoa_ThuocTinh = _classDMHH.SelectHangHoa_ThuocTinh(dM_HangHoa.ID).Select(x => new HangHoa_ThuocTinh
                {
                    GiaTri = x.GiaTri,
                    ThuTuNhap = x.ThuTuNhap
                }).OrderBy(p => p.ThuTuNhap).ToList();
                DonViQuiDoi dm_DVQD = _classDVQD.Get(p => p.ID == id && p.Xoa != true);
                if (dM_HangHoa.DonViQuiDois != null && dm_DVQD != null)
                {
                    ct.ID_DonViQuiDoi = dm_DVQD.ID;
                    ct.MaHangHoa = dm_DVQD.MaHangHoa;
                    ct.GiaBan = dm_DVQD.GiaBan;
                    ct.TenDonViTinh = dm_DVQD.TenDonViTinh;
                    //ct.SrcImage = GenerateBacode(dm_DVQD.MaHangHoa, "");
                }
                if (dM_HangHoa == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }
        // GET: api/DM_HangHoaAPI/5
        [ResponseType(typeof(DM_HangHoaSearch))]
        public List<DM_HangHoaSearch> GetHangHoa_ByMaHangHoa(string mahh, Guid iddonvi)
        {
            //var hh = ClassDM_HangHoa.GetHangHoa_QuyDoiMH(mahh, iddonvi).FirstOrDefault();
            List<DM_HangHoaSearch> lst = new List<DM_HangHoaSearch>();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                if (!string.IsNullOrEmpty(mahh))
                {
                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("MaHH", mahh));
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                    var tbl_timeCSt = from cs in db.ChotSo
                                      where cs.ID_DonVi == iddonvi
                                      select cs;
                    if (tbl_timeCSt.Count() > 0)
                    {
                        lst = db.Database.SqlQuery<DM_HangHoaSearch>("exec getListHangHoaLoHang_ChotSo_EnTer @MaHH, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                    }
                    else
                    {
                        lst = db.Database.SqlQuery<DM_HangHoaSearch>("exec getListHangHoaLoHang_EnTer @MaHH, @ID_ChiNhanh", paramlist.ToArray()).ToList();
                    }

                    lst = lst.Select(p => new DM_HangHoaSearch()
                    {
                        ID_DonViQuiDoi = p.ID_DonViQuiDoi,
                        TyLeChuyenDoi = p.TyLeChuyenDoi,
                        ID = p.ID,
                        MaHangHoa = p.MaHangHoa,
                        QuyCach = p.QuyCach,
                        TenHangHoa = p.TenHangHoa,
                        ThuocTinh_GiaTri = p.ThuocTinh_GiaTri,
                        TenDonViTinh = p.TenDonViTinh,
                        QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                        LaHangHoa = p.LaHangHoa,
                        GiaVon = p.GiaVon,
                        GiaBan = p.GiaBan,
                        GiaNhap = p.GiaNhap,
                        TonKho = p.TonKho,
                        SrcImage = p.SrcImage,
                        ID_LoHang = p.ID_LoHang,
                        MaLoHang = p.MaLoHang,
                        NgaySanXuat = p.NgaySanXuat,
                        NgayHetHan = p.NgayHetHan,
                        DonViTinh = _classDVQD.Gets(ct => ct.ID_HangHoa == p.ID && ct.Xoa != true).Select(x => new DonViTinh
                        {
                            ID_HangHoa = p.ID,
                            TenDonViTinh = x.TenDonViTinh,
                            ID_DonViQuiDoi = x.ID,
                            QuanLyTheoLoHang = p.QuanLyTheoLoHang,
                            Xoa = true,
                            TyLeChuyenDoi = x.TyLeChuyenDoi
                        }).ToList()
                        //CheckDinhLuongDV = db.DinhLuongDichVus.Where(c => c.ID_DichVu == p.ID_DonViQuiDoi).FirstOrDefault() != null ? true : false
                    }).ToList();
                    return lst/*.Where(p=>p.CheckDinhLuongDV == true || p.LaHangHoa == true).ToList()*/;
                }
                else
                {
                    return null;
                }
            }
        }

        [ResponseType(typeof(DM_HangHoaDTO))]
        public DM_HangHoaDTO GetHangHoa_ByIDQuyDoi(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetHangHoa_QuyDoi(id, iddonvi).FirstOrDefault();
            }
        }

        [ResponseType(typeof(DM_HangHoaDTO))]
        public DM_HangHoaDTO GetHangHoa_ByIDQuyDoiDVT(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetHangHoa_QuyDoiDVT(id, iddonvi).FirstOrDefault();
            }
        }

        [ResponseType(typeof(DM_HangHoaDTO))]
        public DM_HangHoaDTO GetHangHoa_ByIDQuyDoiDVTByLo(Guid id, Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetHangHoa_ByIDQuyDoiDVTByLo(id, iddonvi).FirstOrDefault();
            }
        }

        public DM_HangHoaDTO GetHangHoa_ByIDQuyDoi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                return _classDMHH.GetHangHoa_ByIDDonViQuiDoi(id).FirstOrDefault();
            }
        }

        [ResponseType(typeof(DonViQuiDoi))]
        public IHttpActionResult getDonViQuiDoi_ByMaHangHoa(string maHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                DonViQuiDoi dvqd = _classDVQD.Select_DonViQuiDoi(maHangHoa);
                if (dvqd == null)
                {
                    return NotFound();
                }
                return Ok(dvqd);
            }
        }


        [ResponseType(typeof(DM_HangHoa))]
        public IHttpActionResult GetChiTietHangHoa(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                List<DM_HangHoaDTO> dM_HangHoa = _classDMHH.GetChiTietHangHoa(id);
                if (dM_HangHoa == null)
                {
                    return NotFound();
                }
                return Ok(dM_HangHoa);
            }
        }

        [ResponseType(typeof(DonViQuiDoi))]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ChiTietDonViTinh(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                DonViQuiDoi dvqd = _classDVQD.Select_DonViQuiDoi(id);
                if (dvqd == null)
                {
                    return NotFound();
                }
                return Ok(dvqd);
            }
        }


        #endregion

        #region update

        [HttpGet]
        public IHttpActionResult UpdateHienThiDatLich(string id, string v)
        {
            try
            {
                Guid idhanghoa = new Guid(id);
                int hienthidatlich = int.Parse(v);
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    DM_HangHoa hangHoa = db.DM_HangHoa.Where(p => p.ID == idhanghoa).FirstOrDefault();
                    if (hangHoa != null)
                    {
                        hangHoa.HienThiDatLich = hienthidatlich;
                        db.SaveChanges();
                        return ActionTrueData("");
                    }
                    else
                    {
                        return ActionFalseNotData("");
                    }
                }
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }
        public IHttpActionResult MoveNhomHH(Guid idNhom, Guid id_dvqd)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                var id_hanghoa = _classDVQD.Get(p => p.ID == id_dvqd).ID_HangHoa;
                DM_HangHoa dmHH = _classDMHH.Get(idhh => idhh.ID == id_hanghoa);
                dmHH.ID_NhomHang = idNhom;
                string strUpd = _classDMHH.Update_HangHoa(dmHH, null);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPut, HttpPost]
        public IHttpActionResult PutDM_HangHoa([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                    classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                    classDinhLuongDichVu _classDLDV = new classDinhLuongDichVu(db);
                    ClassBH_HoaDon _classBHHD = new ClassBH_HoaDon(db);
                    ClassBH_HoaDon_ChiTiet _classBHHDCT = new ClassBH_HoaDon_ChiTiet(db);
                    Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                    Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                    DM_HangHoa objNewHH = data["objNewHH"].ToObject<DM_HangHoa>();

                    // check if change status quanlytheolo
                    var oldProduct = db.DM_HangHoa.Find(objNewHH.ID);
                    if (oldProduct.QuanLyTheoLoHang != objNewHH.QuanLyTheoLoHang)
                    {
                        // check exist in bh_hoadonchitiet
                        var count = (from ct in db.BH_HoaDon_ChiTiet
                                     join qd in db.DonViQuiDois on ct.ID_DonViQuiDoi equals qd.ID
                                     where qd.ID_HangHoa == oldProduct.ID
                                     select new { ct.ID }).Count();
                        if (count > 0)
                        {
                            if (oldProduct.QuanLyTheoLoHang.Value)
                            {
                                return ActionFalseNotData("Sản phẩm đã được quản lý theo lô, không thể thay đổi ngược lại");
                            }
                            else
                            {
                                return ActionFalseNotData("Sản phẩm đã có tồn kho hoặc đã phát sinh giao dịch, không thể chuyển sang quản lý theo lô");
                            }
                        }
                    }

                    List<DM_HangHoa_ViTri> listViTri = new List<DM_HangHoa_ViTri>();
                    if (data["listViTri"] != null)
                    {
                        listViTri = data["listViTri"].ToObject<List<DM_HangHoa_ViTri>>();
                    }

                    List<HangHoa_ThuocTinh> objThuocTinh = new List<HangHoa_ThuocTinh>();
                    if (data["ListThuocTinh"] != null)
                    {
                        objThuocTinh = data["ListThuocTinh"].ToObject<List<HangHoa_ThuocTinh>>();
                    }

                    List<DinhLuongDichVu> dlDV = new List<DinhLuongDichVu>();
                    if (data["listDLDV"] != null)
                    {
                        dlDV = data["listDLDV"].ToObject<List<DinhLuongDichVu>>();
                    }

                    Kho_TonKhoKhoiTao objNewTonKhoKT = null;
                    if (data["objNewTonKhoKT"] != null)
                    {
                        objNewTonKhoKT = data["objNewTonKhoKT"].ToObject<Kho_TonKhoKhoiTao>();
                    }
                    List<DonViQuiDoi> objlstDVTKhacs = new List<DonViQuiDoi>();
                    List<DonViQuiDoi> dvqdOld = _classDVQD.Select_DonViQuiDois_IDHangHoa(objNewHH.ID);
                    DonViQuiDoi objNewDVT = data["objNewDVT"].ToObject<DonViQuiDoi>();
                    objNewDVT.GiaBan = double.Parse(data["objNewHH"]["GiaBan"].ToString());
                    objNewDVT.ID = dvqdOld.Where(p => p.LaDonViChuan == true).Select(p => p.ID).FirstOrDefault();
                    objNewDVT.NguoiSua = objNewHH.NguoiSua;
                    objNewDVT.NgaySua = DateTime.Now;
                    objNewDVT.GiaVon = double.Parse(data["objNewHH"]["GiaVon"].ToString());
                    objNewDVT.ID_HangHoa = objNewHH.ID;
                    objNewDVT.LaDonViChuan = true;
                    objNewDVT.TyLeChuyenDoi = 1;
                    objNewDVT.Xoa = false;
                    _classDVQD.Update_DonViQuiDoi(objNewDVT);

                    //add update don vi quy doi
                    if (data["listDVTQuydois"] != null)
                        objlstDVTKhacs = data["listDVTQuydois"].ToObject<List<DonViQuiDoi>>();
                    foreach (var item in objlstDVTKhacs)
                    {
                        DonViQuiDoi dvqdtemp = dvqdOld.Where(p => p.ID == item.ID).FirstOrDefault();
                        if (dvqdtemp != null)
                        {
                            item.NguoiSua = objNewHH.NguoiSua;
                            item.NgaySua = DateTime.Now;
                            item.ID_HangHoa = item.ID_HangHoa;
                            item.GiaBan = item.GiaBan;
                            item.GiaVon = objNewDVT.GiaVon * item.TyLeChuyenDoi;
                            item.Xoa = false;
                            string strDvqdUd = _classDVQD.Update_DonViQuiDoi(item);

                        }
                        else
                        {
                            item.ID = Guid.NewGuid();
                            item.NgayTao = DateTime.Now;
                            item.NguoiTao = objNewHH.NguoiSua;
                            item.ID_HangHoa = objNewHH.ID;
                            item.Xoa = false;
                            if (item.MaHangHoa == "" || item.MaHangHoa == null)
                            {
                                item.MaHangHoa = _classDVQD.GetMaHangHoa();
                            }
                            string strDvqdAdd = _classDVQD.Add_DonViQuiDoi(item);

                            DM_GiaVon dvGV = new DM_GiaVon
                            {
                                ID = Guid.NewGuid(),
                                ID_DonVi = iddonvi,
                                ID_DonViQuiDoi = item.ID,
                                ID_LoHang = null,
                                GiaVon = item.GiaVon,
                            };

                            db.DM_GiaVon.Add(dvGV);
                            _classDMHH.AddMultiple_DMGiaVon(iddonvi, dvGV);

                            DM_HangHoa_TonKho dmTK = new DM_HangHoa_TonKho
                            {
                                ID = Guid.NewGuid(),
                                ID_DonVi = iddonvi,
                                ID_DonViQuyDoi = item.ID,
                                ID_LoHang = null,
                                TonKho = 0,
                            };
                            if (objNewTonKhoKT != null)
                            {
                                dmTK.TonKho = objNewTonKhoKT.SoLuong / item.TyLeChuyenDoi;
                            }
                            db.DM_HangHoa_TonKho.Add(dmTK);
                            _classDMHH.AddMultiple_DMHangHoaTonKho(iddonvi, dmTK);
                        }
                    }

                    DonViQuiDoi dvqd = _classDVQD.Get(id => id.ID == objNewDVT.ID);
                    Guid iddvqdold = dvqdOld.Where(pc => pc.LaDonViChuan == true).FirstOrDefault().ID;
                    List<DM_GiaVon> dmgiavon = db.DM_GiaVon.Where(p => p.ID_DonVi == iddonvi && p.ID_DonViQuiDoi == iddvqdold).ToList();
                    double GiaVonOld = dmgiavon.Count != 0 ? Math.Round(dmgiavon.FirstOrDefault().GiaVon, 2) : 0;
                    if (objNewDVT.GiaVon != GiaVonOld && objNewHH.QuanLyTheoLoHang == false)
                    {
                        if (dmgiavon.Count > 0)
                        {
                            dmgiavon[0].GiaVon = (float)objNewDVT.GiaVon;
                            _classDVQD.Update_DMGiaVon(dmgiavon[0]);
                        }
                        else
                        {
                            DM_GiaVon gv = new DM_GiaVon { };
                            gv.ID = Guid.NewGuid();
                            gv.ID_DonVi = iddonvi;
                            gv.ID_DonViQuiDoi = objNewDVT.ID;
                            gv.ID_LoHang = null;
                            gv.GiaVon = (float)objNewDVT.GiaVon;
                            _classDMHH.AddDM_GiaVon(gv);
                        }

                        BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                        itemBH_HoaDon.ID = Guid.NewGuid();
                        itemBH_HoaDon.DienGiai = "Phiếu điều chỉnh được tạo tự động khi sửa giá vốn hàng hóa: " + dvqd.MaHangHoa;
                        itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                        itemBH_HoaDon.ID_DonVi = iddonvi;
                        itemBH_HoaDon.MaHoaDon = _classBHHD.SP_GetMaHoaDon_byTemp(18, itemBH_HoaDon.ID_DonVi, itemBH_HoaDon.NgayLapHoaDon);
                        itemBH_HoaDon.ID_NhanVien = idnhanvien;
                        itemBH_HoaDon.ChoThanhToan = false;
                        itemBH_HoaDon.YeuCau = "Hoàn thành";
                        itemBH_HoaDon.LoaiHoaDon = 18;
                        itemBH_HoaDon.NguoiTao = objNewHH.NguoiSua;
                        string strInsKK = _classBHHD.Add_HoaDon(itemBH_HoaDon);

                        //chitiet hóa đơn kiểm hàng
                        if (strInsKK != null && strInsKK != string.Empty)
                            return Json(new { res = false, mes = strInsKK });
                        else
                        {
                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = objNewDVT.ID,
                                ID_HoaDon = itemBH_HoaDon.ID,
                                GiaVon = objNewDVT.GiaVon,
                                PTChietKhau = objNewDVT.GiaVon - GiaVonOld > 0 ? objNewDVT.GiaVon - GiaVonOld : 0,
                                TienChietKhau = objNewDVT.GiaVon - GiaVonOld > 0 ? 0 : objNewDVT.GiaVon - GiaVonOld,
                                DonGia = GiaVonOld,
                                SoThuTu = 1
                            };
                            strInsKK = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);
                            // tạo lịch sử hđ
                            HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                            hT_NhatKySuDung.ID = Guid.NewGuid();
                            hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                            hT_NhatKySuDung.ChucNang = "Điều chỉnh giá vốn";
                            hT_NhatKySuDung.ThoiGian = DateTime.Now;
                            hT_NhatKySuDung.NoiDung = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: " + dvqdOld.FirstOrDefault().MaHangHoa + " :" + objNewDVT.GiaVon;
                            hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('" + dvqdOld.FirstOrDefault().MaHangHoa + "')\">" + dvqdOld.FirstOrDefault().MaHangHoa + "</a> :" + objNewDVT.GiaVon;
                            hT_NhatKySuDung.LoaiNhatKy = 1;
                            hT_NhatKySuDung.ID_DonVi = iddonvi;
                            hT_NhatKySuDung.ID_HoaDon = itemBH_HoaDon.ID;
                            hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                            hT_NhatKySuDung.LoaiHoaDon = itemBH_HoaDon.LoaiHoaDon;
                            SaveDiary.add_Diary(hT_NhatKySuDung);
                        }
                    }

                    //Tạo phiếu vào kiểm hàng
                    double tonhientai = _classDMHH.TinhSLTonHH(objNewHH.ID, iddonvi).Value;
                    if (objNewTonKhoKT != null)
                    {
                        if (objNewTonKhoKT.SoLuong != tonhientai)
                        {
                            BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                            itemBH_HoaDon.ID = Guid.NewGuid();
                            itemBH_HoaDon.NguoiTao = objNewHH.NguoiTao;
                            itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                            itemBH_HoaDon.ID_DonVi = iddonvi;
                            itemBH_HoaDon.MaHoaDon = _classBHHD.SP_GetMaHoaDon_byTemp(9, itemBH_HoaDon.ID_DonVi, itemBH_HoaDon.NgayLapHoaDon);
                            itemBH_HoaDon.DienGiai = "Phiếu kiểm kho được tạo tự động khi cập nhật hàng hóa:" + " " + objNewDVT.MaHangHoa;
                            itemBH_HoaDon.ID_NhanVien = idnhanvien;
                            if (objNewTonKhoKT.SoLuong > tonhientai)
                            {
                                itemBH_HoaDon.TongChiPhi = objNewTonKhoKT.SoLuong - tonhientai; //slLtang
                                itemBH_HoaDon.TongTienHang = 0; //SLLgiam
                                itemBH_HoaDon.TongGiamGia = (objNewTonKhoKT.SoLuong - tonhientai); // tongchenhlech
                            }
                            else
                            {
                                itemBH_HoaDon.TongChiPhi = 0;
                                itemBH_HoaDon.TongTienHang = objNewTonKhoKT.SoLuong - tonhientai;
                                itemBH_HoaDon.TongGiamGia = (objNewTonKhoKT.SoLuong - tonhientai);
                            }
                            itemBH_HoaDon.PhaiThanhToan = 0; // Giatritang
                            itemBH_HoaDon.TongChietKhau = 0; //GiaTriGiam
                            itemBH_HoaDon.TongTienThue = 0; //TongTienlech
                                                            //itemBH_HoaDon.TongGiamGia = objNewHH.TongGiamGia; //Tổng chênh lệch
                                                            //itemBH_HoaDon.TongChiPhi = objNewHH.TongChiPhi; //SL lệch tăng
                                                            //itemBH_HoaDon.TongTienHang = objNewHH.TongTienHang; // SL lệch giảm
                            itemBH_HoaDon.ChoThanhToan = false;
                            itemBH_HoaDon.LoaiHoaDon = 9;
                            itemBH_HoaDon.NguoiTao = objNewHH.NguoiSua;
                            string strIns = _classBHHD.Add_HoaDon(itemBH_HoaDon);
                            //chitiet hóa đơn kiểm hàng
                            if (strIns != null && strIns != string.Empty)
                                return Json(new { res = false, mes = strIns });
                            else
                            {
                                BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                {
                                    ID = Guid.NewGuid(),
                                    ID_DonViQuiDoi = objNewDVT.ID,
                                    ID_HoaDon = itemBH_HoaDon.ID,
                                    TienChietKhau = tonhientai, //Tồn kho
                                    ThanhTien = objNewTonKhoKT.SoLuong, // Thực tế
                                    SoLuong = objNewTonKhoKT.SoLuong - tonhientai, // SL lệch
                                    ThanhToan = (objNewTonKhoKT.SoLuong - tonhientai) * objNewDVT.GiaVon, // Giá trị lệch
                                    GiaVon = objNewDVT.GiaVon,
                                    TonLuyKe = objNewTonKhoKT.SoLuong,
                                    SoThuTu = 1
                                };
                                strIns = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);


                                string listCT = "- <a onclick=\"FindMaHangHoa('" + dvqd.MaHangHoa + "')\">" + dvqd.MaHangHoa + " </a> :" + ctHoaDon.ThanhTien + "/" + ctHoaDon.SoLuong + "</br>";
                                string listND = "- " + dvqd.MaHangHoa + ":" + ctHoaDon.ThanhTien + "/" + ctHoaDon.SoLuong;
                                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                                hT_NhatKySuDung.ID = Guid.NewGuid();
                                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                                hT_NhatKySuDung.ID_HoaDon = itemBH_HoaDon.ID;
                                hT_NhatKySuDung.ThoiGianUpdateGV = itemBH_HoaDon.NgayLapHoaDon;
                                hT_NhatKySuDung.LoaiHoaDon = 9;
                                hT_NhatKySuDung.ChucNang = "Kiểm kho";
                                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                                hT_NhatKySuDung.NoiDung = "Tạo mới phiếu kiểm kho : " + itemBH_HoaDon.MaHoaDon + ", ngày cân bằng kho:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: " + listND;
                                hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu kiểm kho:  <a onclick=\"FindKiemKho('" + itemBH_HoaDon.MaHoaDon + "')\"> " + itemBH_HoaDon.MaHoaDon + "</a>, ngày cân bằng kho:" + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyy HH:mm:ss") + ", bao gồm: </br>" + listCT;
                                hT_NhatKySuDung.LoaiNhatKy = 1;
                                hT_NhatKySuDung.ID_DonVi = itemBH_HoaDon.ID_DonVi;
                                SaveDiary.add_Diary(hT_NhatKySuDung);
                            }
                        }
                    }

                    List<DM_HangHoa> lsthanghoacungloai = db.DM_HangHoa.Where(p => p.ID_HangHoaCungLoai == objNewHH.ID_HangHoaCungLoai).ToList();
                    //thuộc tính
                    if (objThuocTinh.Count() > 0)
                    {
                        List<Guid> ID_ThuocTinhOfHHOld = new List<Guid>();
                        List<HangHoa_ThuocTinh> lstThuocTinhOld = db.HangHoa_ThuocTinh.Where(p => p.ID_HangHoa == objNewHH.ID).ToList();
                        ID_ThuocTinhOfHHOld = lstThuocTinhOld.Select(p => p.ID_ThuocTinh).ToList();
                        List<Guid> ID_ThuocTinhOfHHNew = objThuocTinh.Select(p => p.ID_ThuocTinh).ToList();

                        List<Guid> ID_ThuocTinhAdd = ID_ThuocTinhOfHHNew.Except(ID_ThuocTinhOfHHOld).ToList();
                        List<Guid> ID_ThuocTinhRemove = ID_ThuocTinhOfHHOld.Except(ID_ThuocTinhOfHHNew).ToList();

                        foreach (var itemCL in lsthanghoacungloai)
                        {
                            db.HangHoa_ThuocTinh.RemoveRange(db.HangHoa_ThuocTinh.Where(p => p.ID_HangHoa == itemCL.ID).Where(p => ID_ThuocTinhRemove.Contains(p.ID_ThuocTinh)));
                            db.SaveChanges();
                            foreach (var itemAddCL in ID_ThuocTinhAdd)
                            {
                                HangHoa_ThuocTinh itemadd = new HangHoa_ThuocTinh();
                                itemadd.ID = Guid.NewGuid();
                                itemadd.ID_HangHoa = itemCL.ID;
                                itemadd.ID_ThuocTinh = itemAddCL;
                                itemadd.GiaTri = objThuocTinh.Where(p => p.ID_ThuocTinh == itemAddCL).FirstOrDefault().GiaTri;
                                itemadd.ThuTuNhap = objThuocTinh.Where(p => p.ID_ThuocTinh == itemAddCL).FirstOrDefault().index;
                                _classDMHH.add_HangHoa_ThuocTinh(itemadd);
                            }
                        }

                        foreach (var item in objThuocTinh.Where(p => !ID_ThuocTinhAdd.Contains(p.ID_ThuocTinh)))
                        {
                            HangHoa_ThuocTinh objHHTT = _classDMHH.SelectHH_ThuocTinh(item.ID);
                            if (objHHTT != null)
                            {
                                objHHTT.ID_ThuocTinh = item.ID_ThuocTinh;
                                objHHTT.GiaTri = item.GiaTri;
                                objHHTT.ThuTuNhap = item.index;
                                string strDvqdUd = _classDMHH.UpdateHH_thuocTinh(objHHTT);
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemXoaCL in lsthanghoacungloai)
                        {
                            var lstTT = db.HangHoa_ThuocTinh.Where(idDV => idDV.ID_HangHoa == itemXoaCL.ID);
                            if (lstTT != null && lstTT.Count() > 0)
                            {
                                db.HangHoa_ThuocTinh.RemoveRange(lstTT);
                            }
                        }
                    }
                    var lstDinhLuong = db.DinhLuongDichVus.Where(idDV => idDV.ID_DichVu == objNewDVT.ID);
                    if (lstDinhLuong != null && lstDinhLuong.Count() > 0)
                    {
                        db.DinhLuongDichVus.RemoveRange(lstDinhLuong);
                    }
                    db.SaveChanges();
                    foreach (var item in dlDV)
                    {
                        DinhLuongDichVu dl = new DinhLuongDichVu();
                        dl.ID = Guid.NewGuid();
                        dl.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                        dl.ID_DichVu = objNewDVT.ID;
                        dl.SoLuong = item.SoLuong;
                        dl.DonGia = item.DonGia;
                        dl.ID_LoHang = item.ID_LoHang;
                        _classDLDV.Add_DinhLuongDichVu(dl);
                    }

                    db.DM_ViTriHangHoa.RemoveRange(db.DM_ViTriHangHoa.Where(idDV => idDV.ID_HangHoa == objNewHH.ID));
                    db.SaveChanges();

                    foreach (var item in listViTri)
                    {
                        DM_ViTriHangHoa vthh = new DM_ViTriHangHoa();
                        vthh.ID = Guid.NewGuid();
                        vthh.ID_HangHoa = objNewHH.ID;
                        vthh.ID_ViTri = item.ID;
                        _classDMHH.AddViTriChoHangHoa(vthh);
                    }


                    string strUpd = _classDMHH.Update_HangHoa(objNewHH, null);
                    if (strUpd != null && strUpd != string.Empty)
                        return ActionFalseNotData(strUpd);
                    else
                        return ActionTrueNotData(strUpd);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }

        [ResponseType(typeof(string))]
        [HttpPut, HttpGet]
        public IHttpActionResult PutDM_LoHang([FromBody] JObject data)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                classDM_LoHang _classDMLH = new classDM_LoHang(db);
                Guid id = data["ID"].ToObject<Guid>();
                DM_LoHang objLoHang = data["objLoHang"].ToObject<DM_LoHang>();
                string strUpd = _classDMLH.Update_LoHang(objLoHang);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "update lô hàng lỗi!"));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpGet]
        public IHttpActionResult GetChiTietBaoDuong_TheoHangHoa(Guid idHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var data = db.DM_HangHoa_BaoDuongChiTiet.Where(x => x.ID_HangHoa == idHangHoa)
                        .Select(x => new
                        {
                            x.ID_HangHoa,
                            x.LanBaoDuong,
                            x.GiaTri,
                            x.LoaiGiaTri,
                            x.BaoDuongLapDinhKy,
                            LapDinhKy = x.BaoDuongLapDinhKy == 1 ? true : false,
                        }).OrderBy(x => x.LanBaoDuong).ToList();
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }

        [HttpGet]
        public void DMHangHoa_UpdateIDXe(Guid idHangHoa, Guid? idXe)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                try
                {
                    DM_HangHoa hh = db.DM_HangHoa.Find(idHangHoa);
                    if (hh != null)
                    {
                        hh.ID_Xe = idXe;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("DMHangHoa_ResetIDXe " + ex.ToString());
                }
            }
        }
        #endregion

        #region insert
        [HttpPost]
        public IHttpActionResult PostChiTietBaoDuong([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<DM_HangHoa_BaoDuongChiTiet> lstObj = data["lstDetail"].ToObject<List<DM_HangHoa_BaoDuongChiTiet>>();
                        Guid idHangHoa = lstObj.FirstOrDefault().ID_HangHoa;
                        var lstOld = db.DM_HangHoa_BaoDuongChiTiet.Where(x => x.ID_HangHoa == idHangHoa);
                        db.DM_HangHoa_BaoDuongChiTiet.RemoveRange(lstOld);

                        // check if quanlybaoduong 
                        var qlBaoDuong = db.DM_HangHoa.Where(x => x.ID == idHangHoa).FirstOrDefault().QuanLyBaoDuong;
                        if (qlBaoDuong == 1)
                        {
                            List<DM_HangHoa_BaoDuongChiTiet> lstNew = new List<DM_HangHoa_BaoDuongChiTiet>();
                            foreach (var item in lstObj)
                            {
                                item.ID = Guid.NewGuid();
                                lstNew.Add(item);
                            }
                            db.DM_HangHoa_BaoDuongChiTiet.AddRange(lstNew);
                        }

                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueNotData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.Message + ex.InnerException);
                    }
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult BaoDuong_InsertListDetail_ByNhomHang(Guid idHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {

                try
                {
                    SqlParameter param = new SqlParameter("ID_HangHoa", idHangHoa);
                    db.Database.ExecuteSqlCommand("exec BaoDuong_InsertListDetail_ByNhomHang @ID_HangHoa", param);
                    return ActionTrueNotData(string.Empty);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.Message + ex.InnerException);
                }
            }
        }
        [HttpPost, HttpGet]
        public IHttpActionResult PostDM_HangHoaNhieuHangCL([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                        classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                        ClassBH_HoaDon _classBHHD = new ClassBH_HoaDon(db);
                        ClassBH_HoaDon_ChiTiet _classBHHDCT = new ClassBH_HoaDon_ChiTiet(db);
                        List<DM_HangHoaDTO> lstReturn = new List<DM_HangHoaDTO>();
                        Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                        Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                        DM_HangHoa objNewHH = data["objNewHH"].ToObject<DM_HangHoa>();
                        DonViQuiDoi objNewDVT = data["objNewDVT"].ToObject<DonViQuiDoi>();

                        List<DM_HangHoa_ViTri> listViTri = new List<DM_HangHoa_ViTri>();
                        if (data["listViTri"] != null)
                        {
                            listViTri = data["listViTri"].ToObject<List<DM_HangHoa_ViTri>>();
                        }

                        List<DM_HangHoaCungLoai> lstHHCungLoai = new List<DM_HangHoaCungLoai>();
                        if (data["objHangHoaCungLoai"] != null)
                        {
                            lstHHCungLoai = data["objHangHoaCungLoai"].ToObject<List<DM_HangHoaCungLoai>>();
                        }
                        List<DinhLuongDichVu> dlDV = new List<DinhLuongDichVu>();
                        if (data["listDLDV"] != null)
                        {
                            dlDV = data["listDLDV"].ToObject<List<DinhLuongDichVu>>();
                        }

                        List<DonViQuiDoi> objlstDVTKhacs = new List<DonViQuiDoi>();
                        if (data["listDVTQuydois"] != null)
                            objlstDVTKhacs = data["listDVTQuydois"].ToObject<List<DonViQuiDoi>>();

                        Guid idhanghoacungloainew = Guid.NewGuid();
                        var lachacungloai = true;
                        List<DM_HangHoaCungLoai> lstCungLoaiForeach = lstHHCungLoai.Where(p => p.LaDonViChuan == true && p.TrangThai == true).ToList();
                        foreach (var itemcungloai in lstCungLoaiForeach)
                        {
                            string MaHangHoa = string.Empty;
                            if (itemcungloai.MaHangHoa == "" || itemcungloai.MaHangHoa == null)
                            {
                                MaHangHoa = _classDVQD.GetMaHangHoa();
                            }
                            else
                            {
                                MaHangHoa = itemcungloai.MaHangHoa;
                            }
                            string TenHangHoa = objNewHH.TenHangHoa;
                            Guid? idNhom = objNewHH.ID_NhomHang;
                            double GiaBan = itemcungloai.GiaBan;

                            double GiaVon = itemcungloai.GiaVon;
                            double TonKho = itemcungloai.TonKho;

                            #region DM_HangHoa
                            DM_HangHoa objCL = new DM_HangHoa();
                            objCL.ID = Guid.NewGuid();
                            objCL.ID_HangHoaCungLoai = idhanghoacungloainew;
                            objCL.LaChaCungLoai = lachacungloai;
                            objCL.ID_NhomHang = idNhom == null ? Guid.Empty : idNhom;
                            objCL.LaHangHoa = true;
                            objCL.NgayTao = DateTime.Now;
                            objCL.NguoiTao = objNewHH.NguoiTao;
                            objCL.TenHangHoa = TenHangHoa;
                            objCL.TenHangHoa_KhongDau = CommonStatic.ConvertToUnSign(TenHangHoa).ToLower();
                            objCL.TenHangHoa_KyTuDau = CommonStatic.GetCharsStart(TenHangHoa).ToLower();
                            objCL.TenKhac = "";
                            objCL.TheoDoi = true;
                            objCL.GhiChu = objNewHH.GhiChu;
                            objCL.ChiPhiThucHien = 0;
                            objCL.ChiPhiTinhTheoPT = true;
                            objCL.QuyCach = objNewHH.QuyCach;
                            objCL.TonToiThieu = objNewHH.TonToiThieu;
                            objCL.TonToiDa = objNewHH.TonToiDa;
                            objCL.DuocBanTrucTiep = objNewHH.DuocBanTrucTiep;
                            objCL.QuanLyTheoLoHang = objNewHH.QuanLyTheoLoHang;
                            objCL.ThoiGianBaoHanh = objNewHH.ThoiGianBaoHanh;
                            objCL.LoaiBaoHanh = objNewHH.LoaiBaoHanh;
                            objCL.DonViTinhQuyCach = objNewHH.DonViTinhQuyCach;
                            objCL.DuocTichDiem = objNewHH.DuocTichDiem;
                            objCL.QuanLyBaoDuong = objNewHH.QuanLyBaoDuong;
                            objCL.SoKmBaoHanh = objNewHH.SoKmBaoHanh;
                            objCL.HoaHongTruocChietKhau = objNewHH.HoaHongTruocChietKhau;
                            objCL.ID_Xe = objNewHH.ID_Xe;
                            #endregion

                            #region dvt
                            DonViQuiDoi objDVT = new DonViQuiDoi();
                            objDVT.ID = Guid.NewGuid();
                            objDVT.GiaBan = GiaBan;
                            objDVT.GiaVon = GiaVon;
                            objDVT.ID_HangHoa = objCL.ID;
                            objDVT.LaDonViChuan = true;
                            objDVT.MaHangHoa = MaHangHoa;
                            objDVT.NgayTao = DateTime.Now;
                            objDVT.NguoiTao = objCL.NguoiTao;
                            objDVT.TenDonViTinh = objNewDVT.TenDonViTinh == null ? "" : objNewDVT.TenDonViTinh;
                            objDVT.TyLeChuyenDoi = 1;
                            objDVT.Xoa = false;
                            #endregion

                            string strIns = _classDMHH.Add_HangHoa(objCL);
                            if (strIns != null && strIns != string.Empty)
                                return ActionFalseNotData(strIns);
                            else
                            {
                                foreach (var itemViTri in listViTri)
                                {
                                    DM_ViTriHangHoa vitrihh = new DM_ViTriHangHoa();
                                    vitrihh.ID = Guid.NewGuid();
                                    vitrihh.ID_HangHoa = objCL.ID;
                                    vitrihh.ID_ViTri = itemViTri.ID;
                                    _classDMHH.AddViTriChoHangHoa(vitrihh);
                                }

                                _classDVQD.Add_DonViQuiDoi(objDVT);

                                //add DM_HangHoa_TonKho cho đơn vị tính chuẩn
                                _classDVQD.AddDM_HangHoa_TonKho(objDVT.ID, TonKho, iddonvi);

                                objCL.DonViQuiDois.Add(objDVT);
                                // add đơn vị tính khác
                                if (objlstDVTKhacs != null && objlstDVTKhacs.Count > 0)
                                {
                                    foreach (DonViQuiDoi itemDVT in objlstDVTKhacs)
                                    {

                                        DonViQuiDoi objDVT_NewItem = new DonViQuiDoi();
                                        objDVT_NewItem.ID = Guid.NewGuid();
                                        objDVT_NewItem.GiaBan = lstHHCungLoai.Where(p => p.ID_ThuocTinh == itemcungloai.ID_ThuocTinh && p.TenDonViTinh.Trim().ToLower() == itemDVT.TenDonViTinh.Trim().ToLower()).FirstOrDefault().GiaBan;
                                        objDVT_NewItem.GiaVon = lstHHCungLoai.Where(p => p.ID_ThuocTinh == itemcungloai.ID_ThuocTinh && p.TenDonViTinh.Trim().ToLower() == itemDVT.TenDonViTinh.Trim().ToLower()).FirstOrDefault().GiaVon;
                                        objDVT_NewItem.ID_HangHoa = objCL.ID;
                                        objDVT_NewItem.LaDonViChuan = false;
                                        objDVT_NewItem.MaHangHoa = lstHHCungLoai.Where(p => p.ID_ThuocTinh == itemcungloai.ID_ThuocTinh && p.TenDonViTinh.Trim().ToLower() == itemDVT.TenDonViTinh.Trim().ToLower()).FirstOrDefault().MaHangHoa;
                                        if (objDVT_NewItem.MaHangHoa == null || objDVT_NewItem.MaHangHoa == "")
                                        {
                                            objDVT_NewItem.MaHangHoa = _classDVQD.GetMaHangHoa();
                                        }
                                        else
                                        {
                                            //mã hàng hóa tự động
                                            objDVT_NewItem.MaHangHoa = lstHHCungLoai.Where(p => p.ID_ThuocTinh == itemcungloai.ID_ThuocTinh && p.TenDonViTinh.Trim().ToLower() == itemDVT.TenDonViTinh.Trim().ToLower()).FirstOrDefault().MaHangHoa;
                                        }
                                        objDVT_NewItem.NgayTao = DateTime.Now;
                                        objDVT_NewItem.NguoiTao = objCL.NguoiTao;
                                        objDVT_NewItem.TenDonViTinh = itemDVT.TenDonViTinh;
                                        objDVT_NewItem.TyLeChuyenDoi = itemDVT.TyLeChuyenDoi;
                                        objDVT_NewItem.Xoa = false;
                                        _classDVQD.Add_DonViQuiDoi(objDVT_NewItem);

                                        //add DM_HangHoa_TonKho cho đơn vị tính phụ
                                        _classDVQD.AddDM_HangHoa_TonKho(objDVT_NewItem.ID, TonKho / itemDVT.TyLeChuyenDoi, iddonvi);

                                        if (objCL.QuanLyTheoLoHang == false)
                                        {
                                            DM_GiaVon gv = new DM_GiaVon { };
                                            gv.ID = Guid.NewGuid();
                                            gv.ID_DonVi = iddonvi;
                                            gv.ID_DonViQuiDoi = objDVT_NewItem.ID;
                                            gv.ID_LoHang = null;
                                            gv.GiaVon = (float)objDVT_NewItem.GiaVon;
                                            _classDMHH.AddDM_GiaVon(gv);
                                        }
                                    }
                                }

                                //add thuộc tính hàng hóa
                                var chuoi = itemcungloai.ID_ThuocTinh.Split('_');
                                for (int i = 0; i < chuoi.Length; i++)
                                {
                                    if (chuoi[i] != "")
                                    {
                                        HangHoa_ThuocTinh hhtt = new HangHoa_ThuocTinh();
                                        hhtt.ID = Guid.NewGuid();
                                        hhtt.index = i;
                                        hhtt.ID_HangHoa = objCL.ID;
                                        hhtt.ID_ThuocTinh = new Guid(chuoi[i].Split(',')[0]);
                                        hhtt.GiaTri = chuoi[i].Split(',')[1];
                                        hhtt.ThuTuNhap = i;
                                        _classDMHH.add_HangHoa_ThuocTinh(hhtt);
                                    }
                                }

                                //tạo phiếu điều chỉnh giá vốn khi giá vốn của HH cùng loại # 0
                                if (GiaVon != 0)
                                {
                                    DM_GiaVon gv = new DM_GiaVon { };
                                    gv.ID = Guid.NewGuid();
                                    gv.ID_DonVi = iddonvi;
                                    gv.ID_DonViQuiDoi = objDVT.ID;
                                    gv.ID_LoHang = null;
                                    gv.GiaVon = (float)GiaVon;
                                    _classDMHH.AddDM_GiaVon(gv);

                                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                                    itemBH_HoaDon.ID = Guid.NewGuid();
                                    itemBH_HoaDon.MaHoaDon = _classBHHD.GetAutoCode(18);
                                    itemBH_HoaDon.NguoiTao = objCL.NguoiTao;
                                    itemBH_HoaDon.DienGiai = "Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ";
                                    itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                                    itemBH_HoaDon.ID_DonVi = iddonvi;
                                    itemBH_HoaDon.ID_NhanVien = idnhanvien;
                                    itemBH_HoaDon.ChoThanhToan = false;
                                    itemBH_HoaDon.YeuCau = "Hoàn thành";
                                    itemBH_HoaDon.LoaiHoaDon = 18;
                                    string strInsKK = _classBHHD.Add_HoaDon(itemBH_HoaDon);
                                    // tạo lịch sử hđ
                                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                                    hT_NhatKySuDung.ID = Guid.NewGuid();
                                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                                    hT_NhatKySuDung.ChucNang = "Điều chỉnh giá vốn";
                                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                                    hT_NhatKySuDung.NoiDung = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm:" + MaHangHoa + " :" + GiaVon;
                                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('" + MaHangHoa + "')\">" + MaHangHoa + "</a> :" + GiaVon;
                                    hT_NhatKySuDung.LoaiNhatKy = 1;
                                    hT_NhatKySuDung.ID_DonVi = iddonvi;
                                    SaveDiary.add_Diary(hT_NhatKySuDung);
                                    //chitiet hóa đơn kiểm hàng
                                    if (strInsKK != null && strInsKK != string.Empty)
                                        return ActionFalseNotData(strInsKK);
                                    else
                                    {
                                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                        {
                                            ID = Guid.NewGuid(),
                                            ID_DonViQuiDoi = objDVT.ID,
                                            ID_HoaDon = itemBH_HoaDon.ID,
                                            GiaVon = GiaVon,
                                            PTChietKhau = GiaVon,
                                            TienChietKhau = 0,
                                            DonGia = 0,
                                            SoThuTu = 1
                                        };
                                        strInsKK = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);
                                    }
                                }

                                //tạo phiếu kiểm kê khi tồn kho lớn hơn 0
                                if (TonKho != 0)
                                {
                                    //Tạo phiếu vào kiểm hàng
                                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                                    itemBH_HoaDon.ID = Guid.NewGuid();
                                    itemBH_HoaDon.MaHoaDon = _classBHHD.GetAutoCode(9);
                                    itemBH_HoaDon.NguoiTao = objCL.NguoiTao;
                                    itemBH_HoaDon.DienGiai = "Phiếu kiểm kho được tạo tự động khi thêm mới hàng hóa:" + " " + objDVT.MaHangHoa;
                                    itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                                    itemBH_HoaDon.ID_DonVi = iddonvi;
                                    itemBH_HoaDon.ID_NhanVien = idnhanvien;
                                    itemBH_HoaDon.TongChiPhi = TonKho; //slLtang
                                    itemBH_HoaDon.TongTienHang = 0; //SLLgiam
                                    itemBH_HoaDon.TongGiamGia = TonKho; // tongchenhlech
                                    itemBH_HoaDon.PhaiThanhToan = 0; // Giatritang
                                    itemBH_HoaDon.TongChietKhau = 0; //GiaTriGiam
                                    itemBH_HoaDon.TongTienThue = 0; //TongTienlech
                                                                    //itemBH_HoaDon.TongGiamGia = objCL.TongGiamGia; //Tổng chênh lệch
                                                                    //itemBH_HoaDon.TongChiPhi = objCL.TongChiPhi; //SL lệch tăng
                                                                    //itemBH_HoaDon.TongTienHang = objCL.TongTienHang; // SL lệch giảm
                                    itemBH_HoaDon.ChoThanhToan = false;
                                    itemBH_HoaDon.LoaiHoaDon = 9;
                                    string strInsKK = _classBHHD.Add_HoaDon(itemBH_HoaDon);

                                    // tạo lịch sử hđ
                                    HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                                    hT_NhatKySuDung.ID = Guid.NewGuid();
                                    hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                                    hT_NhatKySuDung.ChucNang = "Kiểm kho";
                                    hT_NhatKySuDung.ThoiGian = DateTime.Now;
                                    hT_NhatKySuDung.NoiDung = "Tạo mới phiếu kiểm kho: " + itemBH_HoaDon.MaHoaDon + ", ngày cân bằng kho: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: " + MaHangHoa + " :" + TonKho + "/" + TonKho;
                                    hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu kiểm kho: <a onclick=\"FindKiemKho('" + itemBH_HoaDon.MaHoaDon + "')\">" + itemBH_HoaDon.MaHoaDon + " </a>, ngày cân bằng kho: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('" + MaHangHoa + "')\">" + MaHangHoa + "</a> :" + TonKho + "/" + TonKho;
                                    hT_NhatKySuDung.LoaiNhatKy = 1;
                                    hT_NhatKySuDung.ID_DonVi = iddonvi;
                                    SaveDiary.add_Diary(hT_NhatKySuDung);
                                    //chitiet hóa đơn kiểm hàng
                                    if (strInsKK != null && strInsKK != string.Empty)
                                        return ActionFalseNotData(strInsKK);
                                    else
                                    {
                                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                        {
                                            ID = Guid.NewGuid(),
                                            ID_DonViQuiDoi = objDVT.ID,
                                            ID_HoaDon = itemBH_HoaDon.ID,
                                            TienChietKhau = 0, //Tồn kho
                                            ThanhTien = TonKho, // Thực tế
                                            SoLuong = TonKho, // SL lệch
                                            ThanhToan = TonKho * GiaVon, // Giá trị lệch
                                            GiaVon = GiaVon,
                                            SoThuTu = 1,
                                            TonLuyKe = TonKho
                                        };
                                        strInsKK = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);
                                    }
                                }
                            }

                            lachacungloai = false;
                            DM_HangHoaDTO objReturn = new DM_HangHoaDTO();
                            objReturn.ID = objCL.ID;
                            objReturn.ID_HangHoaCungLoai = objCL.ID_HangHoaCungLoai;
                            objReturn.GiaBan = GiaBan;
                            objReturn.GiaVon = GiaVon;
                            objReturn.sLoaiHangHoa = "Hàng hóa";
                            objReturn.MaHangHoa = MaHangHoa;
                            objReturn.LaHangHoa = true;
                            objReturn.GhiChu = objCL.GhiChu;
                            objReturn.QuyCach = objCL.QuyCach;
                            objReturn.TonToiThieu = objCL.TonToiThieu;
                            objReturn.TonToiDa = objCL.TonToiDa;
                            objReturn.DuocBanTrucTiep = objCL.DuocBanTrucTiep;
                            if (idNhom == null)
                            {
                                objReturn.NhomHangHoa = "";
                            }
                            else
                            {
                                objReturn.NhomHangHoa = _classDMNHH.Select_NhomHangHoa(idNhom.Value).TenNhomHangHoa;
                            }
                            objReturn.TenHangHoa = TenHangHoa;
                            objReturn.TonKho = TonKho;
                            objReturn.TrangThai = true;
                            objReturn.ID_DonViQuiDoi = objDVT.ID;
                            objReturn.SoLuong = 1;
                            objReturn.GiamGia = 0;
                            objReturn.ThanhTien = 0;
                            lstReturn.Add(objReturn);
                        }
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueData(lstReturn);
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
        public IHttpActionResult PostDM_HangHoa([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                        classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                        classDinhLuongDichVu _classDLDV = new classDinhLuongDichVu(db);
                        ClassBH_HoaDon _classBHHD = new ClassBH_HoaDon(db);
                        classDM_Kho _classDMKho = new classDM_Kho(db);

                        ClassBH_HoaDon_ChiTiet _classBHHDCT = new ClassBH_HoaDon_ChiTiet(db);
                        Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                        Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                        DM_HangHoa objNewHH = data["objNewHH"].ToObject<DM_HangHoa>();
                        DonViQuiDoi objNewDVT = data["objNewDVT"].ToObject<DonViQuiDoi>();

                        List<DM_HangHoa_ViTri> listViTri = new List<DM_HangHoa_ViTri>();
                        if (data["listViTri"] != null)
                        {
                            listViTri = data["listViTri"].ToObject<List<DM_HangHoa_ViTri>>();
                        }
                        List<DinhLuongDichVu> dlDV = new List<DinhLuongDichVu>();
                        if (data["listDLDV"] != null)
                        {
                            dlDV = data["listDLDV"].ToObject<List<DinhLuongDichVu>>();
                        }

                        List<HangHoa_ThuocTinh> objThuocTinh = new List<HangHoa_ThuocTinh>();
                        if (data["ListThuocTinh"] != null)
                        {
                            objThuocTinh = data["ListThuocTinh"].ToObject<List<HangHoa_ThuocTinh>>();
                        }
                        Kho_TonKhoKhoiTao objNewTonKhoKT = null;

                        List<DonViQuiDoi> objlstDVTKhacs = new List<DonViQuiDoi>();
                        if (data["listDVTQuydois"] != null)
                            objlstDVTKhacs = data["listDVTQuydois"].ToObject<List<DonViQuiDoi>>();

                        if (data["objNewTonKhoKT"] != null)
                        {
                            objNewTonKhoKT = data["objNewTonKhoKT"].ToObject<Kho_TonKhoKhoiTao>();
                        }
                        string MaHangHoa = string.Empty;
                        if (objNewDVT.MaHangHoa == null || objNewDVT.MaHangHoa == "")
                        {
                            MaHangHoa = _classDVQD.GetMaHangHoa();
                        }
                        else
                        {
                            //mã hàng hóa tự động
                            MaHangHoa = objNewDVT.MaHangHoa;
                        }
                        string TenHangHoa = objNewHH.TenHangHoa;
                        Guid? idNhom = objNewHH.ID_NhomHang;
                        string ghiChu = objNewHH.GhiChu;

                        double GiaBan = objNewDVT.GiaBan;

                        double GiaVon = objNewDVT.GiaVon;
                        double TonKho = objNewTonKhoKT != null ? objNewTonKhoKT.SoLuong : 0;
                        if (MaHangHoa != null && MaHangHoa.Trim() != "")
                        {
                            var exists = _classDMHH.Check_MaHangHoaExist(MaHangHoa);
                            if (exists)
                                return ActionFalseNotData("Mã hàng hóa đã tồn tại");
                        }
                        #region DM_HangHoa
                        objNewHH.ID = Guid.NewGuid();
                        objNewHH.LaChaCungLoai = objNewHH.ID_HangHoaCungLoai != null ? false : true;
                        objNewHH.ID_HangHoaCungLoai = objNewHH.ID_HangHoaCungLoai != null ? objNewHH.ID_HangHoaCungLoai : Guid.NewGuid();
                        objNewHH.ID_NhomHang = idNhom == null ? Guid.Empty : idNhom;
                        objNewHH.LaHangHoa = true;
                        objNewHH.NgayTao = DateTime.Now;
                        objNewHH.TenHangHoa_KhongDau = CommonStatic.ConvertToUnSign(TenHangHoa).ToLower();
                        objNewHH.TenHangHoa_KyTuDau = CommonStatic.GetCharsStart(TenHangHoa).ToLower();
                        objNewHH.TenKhac = "";
                        objNewHH.TheoDoi = true;
                        objNewHH.ChiPhiThucHien = 0;
                        objNewHH.ChiPhiTinhTheoPT = true;
                        #endregion

                        #region dvt
                        DonViQuiDoi objDVT = new DonViQuiDoi();
                        objDVT.ID = Guid.NewGuid();
                        objDVT.GiaBan = GiaBan;
                        objDVT.GiaVon = GiaVon;
                        objDVT.ID_HangHoa = objNewHH.ID;
                        objDVT.LaDonViChuan = true;
                        objDVT.MaHangHoa = MaHangHoa;
                        objDVT.NgayTao = DateTime.Now;
                        objDVT.NguoiTao = objNewHH.NguoiTao;
                        objDVT.TenDonViTinh = objNewDVT.TenDonViTinh == null ? "" : objNewDVT.TenDonViTinh;
                        objDVT.Xoa = false;
                        objDVT.TyLeChuyenDoi = 1;

                        #endregion
                        string strIns = _classDMHH.Add_HangHoa(objNewHH);
                        if (strIns != null && strIns != string.Empty)
                            return ActionFalseNotData(strIns);

                        foreach (var itemViTri in listViTri)
                        {
                            DM_ViTriHangHoa vitrihh = new DM_ViTriHangHoa();
                            vitrihh.ID = Guid.NewGuid();
                            vitrihh.ID_HangHoa = objNewHH.ID;
                            vitrihh.ID_ViTri = itemViTri.ID;
                            _classDMHH.AddViTriChoHangHoa(vitrihh);
                        }
                        _classDVQD.Add_DonViQuiDoi(objDVT);
                        _classDVQD.AddDM_HangHoa_TonKho(objDVT.ID, TonKho, iddonvi);

                        // add Dinh luong dich vu
                        foreach (var item in dlDV)
                        {
                            DinhLuongDichVu dinhluongDV = new DinhLuongDichVu();
                            dinhluongDV.ID_DichVu = objDVT.ID;
                            dinhluongDV.ID = Guid.NewGuid();
                            dinhluongDV.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                            dinhluongDV.SoLuong = item.SoLuong;
                            dinhluongDV.DonGia = item.DonGia;
                            dinhluongDV.ID_LoHang = item.ID_LoHang;
                            _classDLDV.Add_DinhLuongDichVu(dinhluongDV);
                        }

                        if (objlstDVTKhacs != null && objlstDVTKhacs.Count > 0)
                        {
                            foreach (DonViQuiDoi itemDVT in objlstDVTKhacs)
                            {

                                DonViQuiDoi objDVT_NewItem = new DonViQuiDoi();
                                objDVT_NewItem.ID = Guid.NewGuid();
                                objDVT_NewItem.GiaBan = itemDVT.GiaBan;
                                objDVT_NewItem.GiaVon = GiaVon * itemDVT.TyLeChuyenDoi;
                                objDVT_NewItem.ID_HangHoa = objNewHH.ID;
                                objDVT_NewItem.LaDonViChuan = false;
                                objDVT_NewItem.MaHangHoa = itemDVT.MaHangHoa;
                                if (objDVT_NewItem.MaHangHoa == null || objDVT_NewItem.MaHangHoa == "")
                                {
                                    objDVT_NewItem.MaHangHoa = _classDVQD.GetMaHangHoa();
                                }
                                else
                                {
                                    objDVT_NewItem.MaHangHoa = itemDVT.MaHangHoa;
                                }
                                objDVT_NewItem.NgayTao = DateTime.Now;
                                objDVT_NewItem.NguoiTao = objNewHH.NguoiTao;
                                objDVT_NewItem.TenDonViTinh = itemDVT.TenDonViTinh;
                                objDVT_NewItem.TyLeChuyenDoi = itemDVT.TyLeChuyenDoi;
                                objDVT_NewItem.Xoa = false;
                                _classDVQD.Add_DonViQuiDoi(objDVT_NewItem);

                                //add DM_HangHoa_TonKho cho đơn vị tính phụ
                                _classDVQD.AddDM_HangHoa_TonKho(objDVT_NewItem.ID, TonKho / itemDVT.TyLeChuyenDoi, iddonvi);

                                if (objNewHH.QuanLyTheoLoHang == false)
                                {
                                    DM_GiaVon gv = new DM_GiaVon { };
                                    gv.ID = Guid.NewGuid();
                                    gv.ID_DonVi = iddonvi;
                                    gv.ID_DonViQuiDoi = objDVT_NewItem.ID;
                                    gv.ID_LoHang = null;
                                    gv.GiaVon = (float)objDVT_NewItem.GiaVon;
                                    _classDMHH.AddDM_GiaVon(gv);
                                }
                            }
                        }

                        //thuộc tính hàng hóa
                        if (objThuocTinh != null && objThuocTinh.Count > 0)
                        {
                            foreach (var item in objThuocTinh)
                            {
                                HangHoa_ThuocTinh hhtt = new HangHoa_ThuocTinh();
                                hhtt.ID = Guid.NewGuid();
                                hhtt.index = item.index;
                                hhtt.ID_HangHoa = objNewHH.ID;
                                hhtt.ID_ThuocTinh = item.ID_ThuocTinh;
                                hhtt.GiaTri = item.GiaTri;
                                hhtt.ThuTuNhap = item.index;
                                _classDMHH.add_HangHoa_ThuocTinh(hhtt);
                            }
                        }
                        if (GiaVon != 0)
                        {
                            DM_GiaVon gv = new DM_GiaVon { };
                            gv.ID = Guid.NewGuid();
                            gv.ID_DonVi = iddonvi;
                            gv.ID_DonViQuiDoi = objDVT.ID;
                            gv.ID_LoHang = null;
                            gv.GiaVon = (float)GiaVon;
                            _classDMHH.AddDM_GiaVon(gv);

                            BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                            itemBH_HoaDon.ID = Guid.NewGuid();
                            itemBH_HoaDon.MaHoaDon = _classBHHD.GetAutoCode(18);
                            itemBH_HoaDon.NguoiTao = objNewHH.NguoiTao;
                            itemBH_HoaDon.DienGiai = "Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ";
                            itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                            itemBH_HoaDon.ID_DonVi = iddonvi;
                            itemBH_HoaDon.ID_NhanVien = idnhanvien;
                            itemBH_HoaDon.ChoThanhToan = false;
                            itemBH_HoaDon.YeuCau = "Hoàn thành";
                            itemBH_HoaDon.LoaiHoaDon = 18;
                            string strInsKK = _classBHHD.Add_HoaDon(itemBH_HoaDon);

                            // tạo lịch sử hđ
                            HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                            hT_NhatKySuDung.ID = Guid.NewGuid();
                            hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                            hT_NhatKySuDung.ChucNang = "Điều chỉnh giá vốn";
                            hT_NhatKySuDung.ThoiGian = DateTime.Now;
                            hT_NhatKySuDung.NoiDung = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm:" + MaHangHoa + " :" + GiaVon;
                            hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('" + MaHangHoa + "')\">" + MaHangHoa + "</a> :" + GiaVon;
                            hT_NhatKySuDung.LoaiNhatKy = 1;
                            hT_NhatKySuDung.ID_DonVi = iddonvi;
                            SaveDiary.add_Diary(hT_NhatKySuDung);

                            if (strInsKK != null && strInsKK != string.Empty)
                                return ActionFalseNotData(strInsKK);

                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = objDVT.ID,
                                ID_HoaDon = itemBH_HoaDon.ID,
                                GiaVon = GiaVon,
                                PTChietKhau = GiaVon,
                                TienChietKhau = 0,
                                DonGia = 0,
                                SoThuTu = 1
                            };
                            strInsKK = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);
                        }

                        if (objNewTonKhoKT != null && TonKho != 0)
                        {
                            Kho_DonVi objKho = _classDMKho.select_KhoDonVi(iddonvi);

                            //Tạo phiếu vào kiểm hàng
                            BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                            itemBH_HoaDon.ID = Guid.NewGuid();
                            itemBH_HoaDon.MaHoaDon = _classBHHD.GetAutoCode(9);
                            itemBH_HoaDon.NguoiTao = objNewHH.NguoiTao;
                            itemBH_HoaDon.DienGiai = "Phiếu kiểm kho được tạo tự động khi thêm mới hàng hóa:" + " " + objDVT.MaHangHoa;
                            itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                            itemBH_HoaDon.ID_DonVi = iddonvi;
                            itemBH_HoaDon.ID_NhanVien = idnhanvien;
                            itemBH_HoaDon.TongChiPhi = objNewTonKhoKT.SoLuong; //slLtang
                            itemBH_HoaDon.TongTienHang = 0; //SLLgiam
                            itemBH_HoaDon.TongGiamGia = objNewTonKhoKT.SoLuong; // tongchenhlech
                            itemBH_HoaDon.PhaiThanhToan = 0; // Giatritang
                            itemBH_HoaDon.TongChietKhau = 0; //GiaTriGiam
                            itemBH_HoaDon.TongTienThue = 0; //TongTienlech
                                                            //itemBH_HoaDon.TongGiamGia = objNewHH.TongGiamGia; //Tổng chênh lệch
                                                            //itemBH_HoaDon.TongChiPhi = objNewHH.TongChiPhi; //SL lệch tăng
                                                            //itemBH_HoaDon.TongTienHang = objNewHH.TongTienHang; // SL lệch giảm
                            itemBH_HoaDon.ChoThanhToan = false;
                            itemBH_HoaDon.LoaiHoaDon = 9;
                            string strInsKK = _classBHHD.Add_HoaDon(itemBH_HoaDon);

                            // tạo lịch sử hđ
                            HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                            hT_NhatKySuDung.ID = Guid.NewGuid();
                            hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                            hT_NhatKySuDung.ChucNang = "Kiểm kho";
                            hT_NhatKySuDung.ThoiGian = DateTime.Now;
                            hT_NhatKySuDung.NoiDung = "Tạo mới phiếu kiểm kho: " + itemBH_HoaDon.MaHoaDon + ", ngày cân bằng kho: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: " + MaHangHoa + " :" + objNewTonKhoKT.SoLuong + "/" + objNewTonKhoKT.SoLuong;
                            hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu kiểm kho: <a onclick=\"FindKiemKho('" + itemBH_HoaDon.MaHoaDon + "')\">" + itemBH_HoaDon.MaHoaDon + " </a>, ngày cân bằng kho: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('" + MaHangHoa + "')\">" + MaHangHoa + "</a> :" + objNewTonKhoKT.SoLuong + "/" + objNewTonKhoKT.SoLuong;
                            hT_NhatKySuDung.LoaiNhatKy = 1;
                            hT_NhatKySuDung.ID_DonVi = iddonvi;
                            SaveDiary.add_Diary(hT_NhatKySuDung);

                            //chitiet hóa đơn kiểm hàng
                            if (strInsKK != null && strInsKK != string.Empty)
                                return ActionFalseNotData(strInsKK);

                            BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                            {
                                ID = Guid.NewGuid(),
                                ID_DonViQuiDoi = objDVT.ID,
                                ID_HoaDon = itemBH_HoaDon.ID,
                                TienChietKhau = 0, //Tồn kho
                                ThanhTien = objNewTonKhoKT.SoLuong, // Thực tế
                                SoLuong = objNewTonKhoKT.SoLuong, // SL lệch
                                ThanhToan = objNewTonKhoKT.SoLuong * objNewDVT.GiaVon, // Giá trị lệch
                                GiaVon = objNewDVT.GiaVon,
                                SoThuTu = 1,
                                TonLuyKe = objNewTonKhoKT.SoLuong
                            };
                            strInsKK = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);
                        }

                        DM_HangHoaDTO objReturn = new DM_HangHoaDTO();
                        objReturn.ID = objNewHH.ID;
                        objReturn.GiaBan = GiaBan;
                        objReturn.GiaVon = GiaVon;
                        objReturn.sLoaiHangHoa = "Hàng hóa";
                        objReturn.MaHangHoa = MaHangHoa;
                        objReturn.LaHangHoa = true;
                        objReturn.GhiChu = ghiChu;
                        objReturn.QuyCach = objNewHH.QuyCach;
                        objReturn.TonToiThieu = objNewHH.TonToiThieu;
                        objReturn.TonToiDa = objNewHH.TonToiDa;
                        objReturn.DuocBanTrucTiep = objNewHH.DuocBanTrucTiep;
                        if (idNhom == null)
                        {
                            objReturn.NhomHangHoa = "";
                        }
                        else
                        {
                            objReturn.NhomHangHoa = _classDMNHH.Select_NhomHangHoa(idNhom.Value).TenNhomHangHoa;
                        }
                        objReturn.TenHangHoa = TenHangHoa;
                        objReturn.TonKho = TonKho;
                        objReturn.TrangThai = true;
                        objReturn.ID_DonViQuiDoi = objDVT.ID;
                        objReturn.SoLuong = 1;
                        objReturn.GiamGia = 0;
                        objReturn.ThanhTien = 0;

                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueData(objReturn);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionTrueNotData(ex.Message + ex.InnerException);
                    }
                }
            }
        }
        [HttpPost, ActionName("PostDM_HangHoaCungLoai")]
        public IHttpActionResult PostDM_HangHoaCungLoai([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                        classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                        classDinhLuongDichVu _classDLDV = new classDinhLuongDichVu(db);
                        ClassBH_HoaDon _classBHHD = new ClassBH_HoaDon(db);
                        classDM_Kho _classDMKho = new classDM_Kho(db);

                        ClassBH_HoaDon_ChiTiet _classBHHDCT = new ClassBH_HoaDon_ChiTiet(db);
                        Guid iddonvi = data["iddonvi"].ToObject<Guid>();
                        Guid idnhanvien = data["idnhanvien"].ToObject<Guid>();
                        DM_HangHoa objNewHH = data["objNewHH"].ToObject<DM_HangHoa>();
                        DonViQuiDoi objNewDVT = data["objNewDVT"].ToObject<DonViQuiDoi>();

                        List<DM_HangHoa_ViTri> listViTri = new List<DM_HangHoa_ViTri>();
                        if (data["listViTri"] != null)
                        {
                            listViTri = data["listViTri"].ToObject<List<DM_HangHoa_ViTri>>();
                        }

                        List<DinhLuongDichVu> dlDV = new List<DinhLuongDichVu>();
                        if (data["listDLDV"] != null)
                        {
                            dlDV = data["listDLDV"].ToObject<List<DinhLuongDichVu>>();
                        }

                        List<HangHoa_ThuocTinh> objThuocTinh = new List<HangHoa_ThuocTinh>();
                        if (data["ListThuocTinh"] != null)
                        {
                            objThuocTinh = data["ListThuocTinh"].ToObject<List<HangHoa_ThuocTinh>>();
                        }
                        Kho_TonKhoKhoiTao objNewTonKhoKT = null;

                        List<DonViQuiDoi> objlstDVTKhacs = new List<DonViQuiDoi>();
                        if (data["listDVTQuydois"] != null)
                            objlstDVTKhacs = data["listDVTQuydois"].ToObject<List<DonViQuiDoi>>();

                        if (data["objNewTonKhoKT"] != null)
                        {
                            objNewTonKhoKT = data["objNewTonKhoKT"].ToObject<Kho_TonKhoKhoiTao>();
                        }
                        string MaHangHoa = string.Empty;
                        if (objNewDVT.MaHangHoa == null || objNewDVT.MaHangHoa == "")
                        {
                            MaHangHoa = _classDVQD.GetMaHangHoa();
                        }
                        else
                        {
                            MaHangHoa = objNewDVT.MaHangHoa;
                        }
                        string TenHangHoa = objNewHH.TenHangHoa;
                        Guid? idNhom = objNewHH.ID_NhomHang;
                        string ghiChu = objNewHH.GhiChu;

                        double GiaBan = objNewDVT.GiaBan;

                        double GiaVon = objNewDVT.GiaVon;
                        double TonKho = objNewTonKhoKT != null ? objNewTonKhoKT.SoLuong : 0;
                        if (MaHangHoa != null && MaHangHoa.Trim() != "")
                        {
                            var exists = _classDMHH.Check_MaHangHoaExist(MaHangHoa);
                            if (exists)
                                return ActionFalseNotData("Mã hàng hóa đã tồn tại");
                        }
                        #region DM_HangHoa
                        objNewHH.ID = Guid.NewGuid();
                        objNewHH.LaChaCungLoai = objNewHH.ID_HangHoaCungLoai != null ? false : true;
                        objNewHH.ID_HangHoaCungLoai = objNewHH.ID_HangHoaCungLoai != null ? objNewHH.ID_HangHoaCungLoai : Guid.NewGuid();
                        objNewHH.ID_NhomHang = idNhom == null ? Guid.Empty : idNhom;
                        objNewHH.LaHangHoa = true;
                        objNewHH.NgayTao = DateTime.Now;
                        objNewHH.TenHangHoa_KhongDau = CommonStatic.ConvertToUnSign(TenHangHoa).ToLower();
                        objNewHH.TenHangHoa_KyTuDau = CommonStatic.GetCharsStart(TenHangHoa).ToLower();
                        objNewHH.GhiChu = ghiChu;
                        objNewHH.ChiPhiThucHien = 0;
                        objNewHH.ChiPhiTinhTheoPT = true;
                        #endregion

                        #region dvt
                        DonViQuiDoi objDVT = new DonViQuiDoi();
                        objDVT.ID = Guid.NewGuid();
                        objDVT.GiaBan = GiaBan;
                        objDVT.GiaVon = GiaVon;
                        objDVT.ID_HangHoa = objNewHH.ID;
                        objDVT.LaDonViChuan = true;
                        objDVT.MaHangHoa = MaHangHoa;
                        objDVT.NgayTao = DateTime.Now;
                        objDVT.NguoiTao = objNewHH.NguoiTao;
                        objDVT.TenDonViTinh = objNewDVT.TenDonViTinh == null ? "" : objNewDVT.TenDonViTinh;
                        objDVT.TyLeChuyenDoi = 1;
                        objDVT.Xoa = false;
                        #endregion
                        string strIns = _classDMHH.Add_HangHoa(objNewHH);

                        if (strIns != null && strIns != string.Empty)
                            return ActionFalseNotData(strIns);
                        else
                        {
                            foreach (var itemViTri in listViTri)
                            {
                                DM_ViTriHangHoa vitrihh = new DM_ViTriHangHoa();
                                vitrihh.ID = Guid.NewGuid();
                                vitrihh.ID_HangHoa = objNewHH.ID;
                                vitrihh.ID_ViTri = itemViTri.ID;
                                _classDMHH.AddViTriChoHangHoa(vitrihh);
                            }

                            Kho_DonVi objKho = null;
                            _classDVQD.Add_DonViQuiDoi(objDVT);

                            //add DM_HangHoa_TonKho cho đơn vị tính chuẩn
                            _classDVQD.AddDM_HangHoa_TonKho(objDVT.ID, TonKho, iddonvi);

                            // add Dinh luong dich vu
                            foreach (var item in dlDV)
                            {
                                DinhLuongDichVu dinhluongDV = new DinhLuongDichVu();
                                dinhluongDV.ID_DichVu = objDVT.ID;
                                dinhluongDV.ID = Guid.NewGuid();
                                dinhluongDV.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                                dinhluongDV.SoLuong = item.SoLuong;
                                dinhluongDV.DonGia = item.DonGia;
                                dinhluongDV.ID_LoHang = item.ID_LoHang;
                                _classDLDV.Add_DinhLuongDichVu(dinhluongDV);
                                objDVT.DinhLuongDichVus.Add(dinhluongDV);
                            }

                            objNewHH.DonViQuiDois.Add(objDVT);
                            if (objlstDVTKhacs != null && objlstDVTKhacs.Count > 0)
                            {
                                foreach (DonViQuiDoi itemDVT in objlstDVTKhacs)
                                {
                                    DonViQuiDoi objDVT_NewItem = new DonViQuiDoi();
                                    objDVT_NewItem.ID = Guid.NewGuid();
                                    objDVT_NewItem.GiaBan = itemDVT.GiaBan;
                                    objDVT_NewItem.GiaVon = GiaVon * itemDVT.TyLeChuyenDoi;
                                    objDVT_NewItem.ID_HangHoa = objNewHH.ID;
                                    objDVT_NewItem.LaDonViChuan = false;
                                    objDVT_NewItem.MaHangHoa = itemDVT.MaHangHoa;
                                    if (objDVT_NewItem.MaHangHoa == null || objDVT_NewItem.MaHangHoa == "")
                                    {
                                        objDVT_NewItem.MaHangHoa = _classDVQD.GetMaHangHoa();
                                    }
                                    else
                                    {
                                        //mã hàng hóa tự động
                                        objDVT_NewItem.MaHangHoa = itemDVT.MaHangHoa;
                                    }
                                    objDVT_NewItem.NgayTao = DateTime.Now;
                                    objDVT_NewItem.NguoiTao = objNewHH.NguoiTao;
                                    objDVT_NewItem.TenDonViTinh = itemDVT.TenDonViTinh;
                                    objDVT_NewItem.TyLeChuyenDoi = itemDVT.TyLeChuyenDoi;
                                    objDVT_NewItem.Xoa = false;
                                    _classDVQD.Add_DonViQuiDoi(objDVT_NewItem);

                                    //add DM_HangHoa_TonKho cho đơn vị tính phụ
                                    _classDVQD.AddDM_HangHoa_TonKho(objDVT_NewItem.ID, TonKho / itemDVT.TyLeChuyenDoi, iddonvi);
                                    //End add DM_HangHoa_TonKho cho đơn vị tính phụ


                                    if (objNewHH.QuanLyTheoLoHang == false)
                                    {
                                        DM_GiaVon gv = new DM_GiaVon { };
                                        gv.ID = Guid.NewGuid();
                                        gv.ID_DonVi = iddonvi;
                                        gv.ID_DonViQuiDoi = objDVT_NewItem.ID;
                                        gv.ID_LoHang = null;
                                        gv.GiaVon = (float)objDVT_NewItem.GiaVon;
                                        _classDMHH.AddDM_GiaVon(gv);
                                    }
                                    objNewHH.DonViQuiDois.Add(objDVT_NewItem);
                                }
                            }

                            //thuộc tính hàng hóa
                            if (objThuocTinh != null && objThuocTinh.Count > 0)
                            {
                                foreach (var item in objThuocTinh)
                                {
                                    HangHoa_ThuocTinh hhtt = new HangHoa_ThuocTinh();
                                    hhtt.ID = Guid.NewGuid();
                                    hhtt.index = item.index;
                                    hhtt.ID_HangHoa = objNewHH.ID;
                                    hhtt.ID_ThuocTinh = item.ID_ThuocTinh;
                                    hhtt.GiaTri = item.GiaTri;
                                    hhtt.ThuTuNhap = item.index;
                                    _classDMHH.add_HangHoa_ThuocTinh(hhtt);
                                    objNewHH.HangHoa_ThuocTinh.Add(hhtt);
                                }
                            }
                            if (GiaVon != 0)
                            {
                                DM_GiaVon gv = new DM_GiaVon { };
                                gv.ID = Guid.NewGuid();
                                gv.ID_DonVi = iddonvi;
                                gv.ID_DonViQuiDoi = objDVT.ID;
                                gv.ID_LoHang = null;
                                gv.GiaVon = (float)GiaVon;
                                _classDMHH.AddDM_GiaVon(gv);

                                BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                                itemBH_HoaDon.ID = Guid.NewGuid();
                                itemBH_HoaDon.MaHoaDon = _classBHHD.GetAutoCode(18);
                                itemBH_HoaDon.NguoiTao = objNewHH.NguoiTao;
                                itemBH_HoaDon.DienGiai = "Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ";
                                itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                                itemBH_HoaDon.ID_DonVi = iddonvi;
                                itemBH_HoaDon.ID_NhanVien = idnhanvien;
                                itemBH_HoaDon.ChoThanhToan = false;
                                itemBH_HoaDon.YeuCau = "Hoàn thành";
                                itemBH_HoaDon.LoaiHoaDon = 18;
                                string strInsKK = _classBHHD.Add_HoaDon(itemBH_HoaDon);
                                // tạo lịch sử hđ
                                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                                hT_NhatKySuDung.ID = Guid.NewGuid();
                                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                                hT_NhatKySuDung.ChucNang = "Điều chỉnh giá vốn";
                                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                                hT_NhatKySuDung.NoiDung = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: " + MaHangHoa + " :" + GiaVon;
                                hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('" + MaHangHoa + "')\">" + MaHangHoa + "</a> :" + GiaVon;
                                hT_NhatKySuDung.LoaiNhatKy = 1;
                                hT_NhatKySuDung.ID_DonVi = iddonvi;
                                SaveDiary.add_Diary(hT_NhatKySuDung);
                                //chitiet hóa đơn kiểm hàng
                                if (strInsKK != null && strInsKK != string.Empty)
                                    return ActionFalseNotData(strInsKK);
                                else
                                {
                                    BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonViQuiDoi = objDVT.ID,
                                        ID_HoaDon = itemBH_HoaDon.ID,
                                        GiaVon = GiaVon,
                                        PTChietKhau = GiaVon,
                                        TienChietKhau = 0,
                                        DonGia = 0,
                                        SoThuTu = 1
                                    };
                                    strInsKK = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);
                                }
                            }

                            if (objNewTonKhoKT != null && TonKho != 0)
                            {
                                objKho = _classDMKho.select_KhoDonVi(iddonvi);

                                //Tạo phiếu vào kiểm hàng
                                BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                                itemBH_HoaDon.ID = Guid.NewGuid();
                                itemBH_HoaDon.MaHoaDon = _classBHHD.GetAutoCode(9);
                                itemBH_HoaDon.NguoiTao = objNewHH.NguoiTao;
                                itemBH_HoaDon.DienGiai = "Phiếu kiểm kho được tạo tự động khi thêm mới hàng hóa:" + " " + objDVT.MaHangHoa;
                                itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                                itemBH_HoaDon.ID_DonVi = iddonvi;
                                itemBH_HoaDon.ID_NhanVien = idnhanvien;
                                itemBH_HoaDon.TongChiPhi = objNewTonKhoKT.SoLuong; //slLtang
                                itemBH_HoaDon.TongTienHang = 0; //SLLgiam
                                itemBH_HoaDon.TongGiamGia = objNewTonKhoKT.SoLuong; // tongchenhlech
                                itemBH_HoaDon.PhaiThanhToan = 0; // Giatritang
                                itemBH_HoaDon.TongChietKhau = 0; //GiaTriGiam
                                itemBH_HoaDon.TongTienThue = 0; //TongTienlech
                                                                //itemBH_HoaDon.TongGiamGia = objNewHH.TongGiamGia; //Tổng chênh lệch
                                                                //itemBH_HoaDon.TongChiPhi = objNewHH.TongChiPhi; //SL lệch tăng
                                                                //itemBH_HoaDon.TongTienHang = objNewHH.TongTienHang; // SL lệch giảm
                                itemBH_HoaDon.ChoThanhToan = false;
                                itemBH_HoaDon.LoaiHoaDon = 9;
                                string strInsKK = _classBHHD.Add_HoaDon(itemBH_HoaDon);

                                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung();
                                hT_NhatKySuDung.ID = Guid.NewGuid();
                                hT_NhatKySuDung.ID_NhanVien = idnhanvien;
                                hT_NhatKySuDung.ChucNang = "Kiểm kho";
                                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                                hT_NhatKySuDung.NoiDung = "Tạo mới phiếu kiểm kho: " + itemBH_HoaDon.MaHoaDon + ", ngày cân bằng kho: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bap gồm: " + MaHangHoa + " :" + objNewTonKhoKT.SoLuong + "/" + objNewTonKhoKT.SoLuong; ;
                                hT_NhatKySuDung.NoiDungChiTiet = "Tạo mới phiếu kiểm kho: <a onclick=\"FindKiemKho('" + itemBH_HoaDon.MaHoaDon + "')\">" + itemBH_HoaDon.MaHoaDon + " </a>, ngày cân bằng kho: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('" + MaHangHoa + "')\">" + MaHangHoa + "</a> :" + objNewTonKhoKT.SoLuong + "/" + objNewTonKhoKT.SoLuong;
                                hT_NhatKySuDung.LoaiNhatKy = 1;
                                hT_NhatKySuDung.ID_DonVi = iddonvi;
                                SaveDiary.add_Diary(hT_NhatKySuDung);
                                //chitiet hóa đơn kiểm hàng
                                if (strInsKK != null && strInsKK != string.Empty)
                                    return ActionFalseNotData(strInsKK);
                                else
                                {
                                    BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                                    {
                                        ID = Guid.NewGuid(),
                                        ID_DonViQuiDoi = objDVT.ID,
                                        ID_HoaDon = itemBH_HoaDon.ID,
                                        TienChietKhau = 0, //Tồn kho
                                        ThanhTien = objNewTonKhoKT.SoLuong, // Thực tế
                                        SoLuong = objNewTonKhoKT.SoLuong, // SL lệch
                                        ThanhToan = objNewTonKhoKT.SoLuong * objNewDVT.GiaVon, // Giá trị lệch
                                        GiaVon = objNewDVT.GiaVon,
                                        TonLuyKe = objNewTonKhoKT.SoLuong
                                    };
                                    strInsKK = _classBHHDCT.Add_ChiTietHoaDon(ctHoaDon);
                                }
                            }
                            DM_HangHoaDTO objReturn = new DM_HangHoaDTO();
                            objReturn.ID = objNewHH.ID;
                            objReturn.ID_HangHoaCungLoai = objNewHH.ID_HangHoaCungLoai;
                            objReturn.GiaBan = GiaBan;
                            objReturn.GiaVon = GiaVon;
                            objReturn.sLoaiHangHoa = "Hàng hóa";
                            objReturn.MaHangHoa = MaHangHoa;
                            objReturn.LaHangHoa = true;
                            objReturn.GhiChu = ghiChu;
                            objReturn.QuyCach = objNewHH.QuyCach;
                            objReturn.TonToiThieu = objNewHH.TonToiThieu;
                            objReturn.TonToiDa = objNewHH.TonToiThieu;
                            objReturn.DuocBanTrucTiep = objNewHH.DuocBanTrucTiep;
                            if (idNhom == null)
                            {
                                objReturn.NhomHangHoa = "";
                            }
                            else
                            {
                                objReturn.NhomHangHoa = _classDMNHH.Select_NhomHangHoa(idNhom.Value).TenNhomHangHoa;
                            }
                            objReturn.TenHangHoa = TenHangHoa;
                            objReturn.TonKho = TonKho;
                            objReturn.TrangThai = true;
                            objReturn.ID_DonViQuiDoi = objDVT.ID;
                            objReturn.SoLuong = 1;
                            objReturn.GiamGia = 0;
                            objReturn.ThanhTien = 0;
                            db.SaveChanges();
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
        [HttpPost, ActionName("PostDM_DichVu")]
        [ResponseType(typeof(DM_HangHoaDTO))]
        public IHttpActionResult PostDM_DichVu([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                        classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                        classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                        classDinhLuongDichVu _classDLDV = new classDinhLuongDichVu(db);
                        classDM_Kho _classDMKho = new classDM_Kho(db);

                        DM_HangHoa objNewHH = data["objNewHH"].ToObject<DM_HangHoa>();
                        DonViQuiDoi objNewDVT = data["objNewDVT"].ToObject<DonViQuiDoi>();

                        List<DinhLuongDichVu> dlDV = new List<DinhLuongDichVu>();
                        if (data["listDLDV"] != null)
                        {
                            dlDV = data["listDLDV"].ToObject<List<DinhLuongDichVu>>();
                        }

                        Kho_TonKhoKhoiTao objNewTonKhoKT = null;

                        List<DonViQuiDoi> objlstDVTKhacs = new List<DonViQuiDoi>();
                        if (data["listDVTQuydois"] != null)
                            objlstDVTKhacs = data["listDVTQuydois"].ToObject<List<DonViQuiDoi>>();

                        if (data["objNewTonKhoKT"] != null)
                        {
                            objNewTonKhoKT = data["objNewTonKhoKT"].ToObject<Kho_TonKhoKhoiTao>();
                        }
                        string MaHangHoa = string.Empty;
                        if (objNewDVT.MaHangHoa == null || objNewDVT.MaHangHoa == "")
                        {
                            MaHangHoa = _classDVQD.GetMaHangHoa();
                        }
                        else
                        {
                            //mã hàng hóa tự động
                            MaHangHoa = objNewDVT.MaHangHoa;
                        }
                        string TenHangHoa = objNewHH.TenHangHoa;
                        Guid? idNhom = objNewHH.ID_NhomHang;
                        string ghiChu = objNewHH.GhiChu;

                        double GiaBan = objNewDVT.GiaBan;

                        double GiaVon = objNewDVT.GiaVon;
                        double TonKho = objNewTonKhoKT != null ? objNewTonKhoKT.SoLuong : 0;

                        if (MaHangHoa != null && MaHangHoa.Trim() != "")
                        {
                            var exists = _classDMHH.Check_MaHangHoaExist(MaHangHoa);
                            if (exists)
                                return ActionFalseNotData("Mã hàng hóa đã tồn tại");
                        }
                        DM_Kho objKho = null;
                        if (objNewTonKhoKT != null && TonKho != 0)
                        {
                            DM_Kho objKho2 = _classDMKho.Get(null);
                            if (objKho2 == null)
                            {
                                return ActionFalseNotData("Khai báo danh mục Kho trước khi nhập tồn kho khởi tạo");
                            }
                            else
                                objKho = objKho2;
                        }
                        #region DM_HangHoa
                        objNewHH.ID = Guid.NewGuid();
                        objNewHH.ID_HangHoaCungLoai = Guid.NewGuid();
                        objNewHH.LaChaCungLoai = true;
                        objNewHH.ID_NhomHang = idNhom == null ? new Guid("00000000-0000-0000-0000-000000000001") : idNhom;
                        objNewHH.LaHangHoa = false;
                        objNewHH.NgayTao = DateTime.Now;
                        objNewHH.TenHangHoa_KhongDau = CommonStatic.ConvertToUnSign(TenHangHoa).ToLower();
                        objNewHH.TenHangHoa_KyTuDau = CommonStatic.GetCharsStart(TenHangHoa).ToLower();
                        objNewHH.TenKhac = "";
                        objNewHH.TheoDoi = true;
                        objNewHH.QuanLyTheoLoHang = false;
                        idNhom = objNewHH.ID_NhomHang;
                        #endregion

                        #region dvt
                        DonViQuiDoi objDVT = new DonViQuiDoi();
                        objDVT.ID = Guid.NewGuid();
                        objDVT.GiaBan = GiaBan;
                        objDVT.ID_HangHoa = objNewHH.ID;
                        objDVT.LaDonViChuan = true;
                        objDVT.MaHangHoa = MaHangHoa;
                        objDVT.NgayTao = DateTime.Now;
                        objDVT.NguoiTao = objNewHH.NguoiTao;
                        objDVT.TenDonViTinh = objNewDVT.TenDonViTinh == null ? "" : objNewDVT.TenDonViTinh;
                        objDVT.TyLeChuyenDoi = 1;
                        objDVT.Xoa = false;

                        #endregion
                        string strIns = _classDMHH.Add_HangHoa(objNewHH);
                        if (strIns != null && strIns != string.Empty)
                            return ActionFalseNotData(strIns);
                        else
                        {
                            _classDVQD.Add_DonViQuiDoi(objDVT);
                            // add Dinh luong dich vu
                            foreach (var item in dlDV)
                            {
                                DinhLuongDichVu dinhluongDV = new DinhLuongDichVu();
                                dinhluongDV.ID_DichVu = objDVT.ID;
                                dinhluongDV.ID = Guid.NewGuid();
                                dinhluongDV.ID_DonViQuiDoi = item.ID_DonViQuiDoi;
                                dinhluongDV.SoLuong = item.SoLuong;
                                dinhluongDV.DonGia = item.DonGia;
                                dinhluongDV.ID_LoHang = item.ID_LoHang;
                                _classDLDV.Add_DinhLuongDichVu(dinhluongDV);
                            }

                            if (objlstDVTKhacs != null && objlstDVTKhacs.Count > 0)
                            {
                                foreach (DonViQuiDoi itemDVT in objlstDVTKhacs)
                                {
                                    DonViQuiDoi objDVT_NewItem = new DonViQuiDoi();
                                    objDVT_NewItem.ID = Guid.NewGuid();
                                    objDVT_NewItem.GiaBan = itemDVT.GiaBan;
                                    objDVT_NewItem.ID_HangHoa = objNewHH.ID;
                                    objDVT_NewItem.LaDonViChuan = false;
                                    objDVT_NewItem.MaHangHoa = itemDVT.MaHangHoa;
                                    if (objDVT_NewItem.MaHangHoa == null || objDVT_NewItem.MaHangHoa == "")
                                    {
                                        objDVT_NewItem.MaHangHoa = _classDVQD.GetMaHangHoa();
                                    }
                                    else
                                    {
                                        //mã hàng hóa tự động
                                        objDVT_NewItem.MaHangHoa = itemDVT.MaHangHoa;
                                    }
                                    objDVT_NewItem.NgayTao = DateTime.Now;
                                    objDVT_NewItem.NguoiTao = objNewHH.NguoiTao;
                                    objDVT_NewItem.TenDonViTinh = itemDVT.TenDonViTinh;
                                    objDVT_NewItem.TyLeChuyenDoi = itemDVT.TyLeChuyenDoi;
                                    objDVT_NewItem.Xoa = false;
                                    _classDVQD.Add_DonViQuiDoi(objDVT_NewItem);
                                }
                            }
                            #region ton kho
                            #endregion
                            DM_HangHoaDTO objReturn = new DM_HangHoaDTO();
                            objReturn.ID = objNewHH.ID;
                            objReturn.GiaBan = GiaBan;
                            objReturn.GiaVon = GiaVon;
                            objReturn.sLoaiHangHoa = "Hàng hóa";
                            objReturn.MaHangHoa = MaHangHoa;
                            objReturn.LaHangHoa = false;
                            if (idNhom == null)
                            {
                                objReturn.NhomHangHoa = "";
                            }
                            else
                            {
                                objReturn.NhomHangHoa = _classDMNHH.Select_NhomHangHoa(idNhom.Value).TenNhomHangHoa;
                            }
                            objReturn.TenHangHoa = TenHangHoa;
                            objReturn.TonKho = TonKho;
                            objReturn.TrangThai = true;
                            objReturn.ID_DonViQuiDoi = objDVT.ID;
                            objReturn.SoLuong = 1;
                            objReturn.GiamGia = 0;
                            objReturn.ThanhTien = 0;
                            db.SaveChanges();
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
        [HttpPost, ActionName("PostDM_LoHang")]
        [ResponseType(typeof(DM_LoHangDTO))]
        public IHttpActionResult PostDM_LoHang([FromBody] JObject data)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                classDM_LoHang _classDMLH = new classDM_LoHang(db);
                DM_LoHang objLoHang = data["objLoHang"].ToObject<DM_LoHang>();

                DM_LoHang obj = new DM_LoHang();
                obj.ID = Guid.NewGuid();
                obj.ID_HangHoa = objLoHang.ID;
                obj.MaLoHang = objLoHang.MaLoHang;
                obj.NgaySanXuat = objLoHang.NgaySanXuat;
                obj.NgayHetHan = objLoHang.NgayHetHan;
                obj.NgayTao = DateTime.Now;
                obj.NguoiTao = objLoHang.NguoiTao;
                obj.TrangThai = true;

                _classDMLH.Add_LoHang(obj);
                DM_LoHangDTO objReturn = new DM_LoHangDTO();
                objReturn.ID = obj.ID;
                objReturn.ID_HangHoa = obj.ID_HangHoa;
                objReturn.MaLoHang = obj.MaLoHang;
                objReturn.NgaySanXuat = obj.NgaySanXuat;
                objReturn.NgayHetHan = obj.NgayHetHan;
                objReturn.TrangThai = obj.TrangThai;
                objReturn.NgayTao = obj.NgayTao;
                objReturn.NguoiTao = obj.NguoiTao;
                return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
            }
        }

        [ResponseType(typeof(DM_LoHangDTO))]
        public IHttpActionResult PostDM_LoHang_AddList([FromBody] JObject data)
        {
            using (var db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDM_LoHang _classDMLH = new classDM_LoHang(db);
                    List<DM_LoHang> objLoHang = data["objLoHang"].ToObject<List<DM_LoHang>>();

                    var lst = objLoHang.Select(x => new DM_LoHang
                    {
                        ID = Guid.NewGuid(),
                        ID_HangHoa = x.ID_HangHoa,
                        MaLoHang = x.MaLoHang,
                        TenLoHang = x.MaLoHang,
                        NgaySanXuat = x.NgaySanXuat,
                        NgayHetHan = x.NgayHetHan,
                        NguoiTao = x.NguoiTao,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    }).ToList();
                    _classDMLH.Inserts(lst);
                    return Json(new { res = true, data = lst });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult PostDM_HangHoaDV([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);
                classDonViQuiDoi classDonViQD = new classDonViQuiDoi(db);
                ClassBH_HoaDon classHoaDon = new ClassBH_HoaDon(db);
                ClassBH_HoaDon_ChiTiet classHoaDonCT = new ClassBH_HoaDon_ChiTiet(db);
                NewHangHoaBasic objNew = data["objNew"].ToObject<NewHangHoaBasic>();
                List<HT_NhatKySuDung> lstDiary = new List<HT_NhatKySuDung>();

                string err = string.Empty;
                string productCode = string.Empty;
                if (objNew.MaHangHoa == null || objNew.MaHangHoa == "")
                {
                    productCode = classDonViQD.GetMaHangHoa();
                }
                else
                {
                    productCode = objNew.MaHangHoa;
                    DonViQuiDoi objEx = classDonViQD.Select_DonViQuiDoi(productCode.Trim());
                    if (objEx != null)
                    {
                        err = "Mã hàng hóa đã tồn tại";
                        return Json(new { res = false, mes = err });
                    }
                }

                #region DM_hangHoa
                DM_HangHoa product = new DM_HangHoa();
                product.ID = Guid.NewGuid();
                product.TenHangHoa = objNew.TenHangHoa;
                product.TenHangHoa_KhongDau = objNew.TenHangHoa_KhongDau;
                product.TenHangHoa_KyTuDau = objNew.TenHangHoa_KyTuDau;
                product.ID_NhomHang = objNew.ID_NhomHang;
                product.LaHangHoa = objNew.LaHangHoa;
                product.LoaiHangHoa = objNew.LaHangHoa ? 1 : 2;
                product.QuanLyTheoLoHang = objNew.QuanLyTheoLoHang;
                product.ChiPhiTinhTheoPT = false;
                product.ChiPhiThucHien = 0;
                product.TonToiDa = 0;
                product.TonToiThieu = 0;
                product.NgayTao = DateTime.Now;
                product.NguoiTao = objNew.NguoiTao;
                product.TheoDoi = true;
                product.LaChaCungLoai = true;
                product.ID_HangHoaCungLoai = Guid.NewGuid();
                product.ID_Xe = objNew.ID_Xe;
                err = classHangHoa.Add_HangHoa(product);

                string sLaHH = (product.LaHangHoa ?? false) == true ? " hàng hóa" : " dịch vụ";
                lstDiary.Add(new HT_NhatKySuDung
                {
                    ID = Guid.NewGuid(),
                    ID_DonVi = objNew.ID_DonVi,
                    ID_NhanVien = objNew.ID_NhanVien,
                    LoaiNhatKy = 1,
                    ThoiGian = DateTime.Now,
                    ChucNang = string.Concat("Thêm mới", sLaHH),
                    NoiDung = string.Concat(" Thêm mới ", sLaHH, ": ", product.TenHangHoa, " có mã ", productCode),
                    NoiDungChiTiet = string.Concat("Thêm mới ", sLaHH, " ", product.TenHangHoa,
                    " <a onclick=\"FindMaHangHoa('", productCode, "')\">", productCode, "</a> <br /> Giá bán:", objNew.GiaBan,
                    " <br /> Giá vốn:", objNew.GiaVon, "<br /> Quản lý theo lô ", objNew.QuanLyTheoLoHang)
                });
                #endregion

                #region DonViQuiDoi
                DonViQuiDoi quydoi = new DonViQuiDoi();
                if (err == string.Empty)
                {
                    quydoi.ID = Guid.NewGuid();
                    quydoi.ID_HangHoa = product.ID;
                    quydoi.MaHangHoa = productCode;
                    quydoi.TenDonViTinh = objNew.TenDonViTinh;
                    quydoi.GiaBan = objNew.GiaBan;
                    quydoi.GiaVon = objNew.GiaVon;
                    quydoi.NgayTao = DateTime.Now;
                    quydoi.NguoiTao = objNew.NguoiTao;
                    quydoi.Xoa = false;
                    quydoi.TyLeChuyenDoi = 1;
                    quydoi.LaDonViChuan = true;
                    err = classDonViQD.Add_DonViQuiDoi(quydoi);
                    if (objNew.LaHangHoa)
                    {
                        classDonViQD.AddDM_HangHoa_TonKho(quydoi.ID, objNew.TonKho, objNew.ID_DonVi);
                    }
                }
                #endregion

                if (objNew.GiaVon != 0)
                {
                    DM_GiaVon gv = new DM_GiaVon { };
                    gv.ID = Guid.NewGuid();
                    gv.ID_DonVi = objNew.ID_DonVi;
                    gv.ID_DonViQuiDoi = quydoi.ID;
                    gv.ID_LoHang = null;
                    gv.GiaVon = quydoi.GiaVon;
                    err = classHangHoa.AddDM_GiaVon(gv);

                    #region Phieu dieu chinh giavon
                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                    itemBH_HoaDon.ID = Guid.NewGuid();
                    itemBH_HoaDon.MaHoaDon = classHoaDon.GetAutoCode(18);
                    itemBH_HoaDon.NguoiTao = objNew.NguoiTao;
                    itemBH_HoaDon.DienGiai = "Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ";
                    itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                    itemBH_HoaDon.ID_DonVi = objNew.ID_DonVi;
                    itemBH_HoaDon.ID_NhanVien = objNew.ID_NhanVien;
                    itemBH_HoaDon.ChoThanhToan = false;
                    itemBH_HoaDon.YeuCau = "Hoàn thành";
                    itemBH_HoaDon.LoaiHoaDon = 18;
                    err += classHoaDon.Add_HoaDon(itemBH_HoaDon);

                    if (err == string.Empty)
                    {
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = quydoi.ID,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            GiaVon = objNew.GiaVon,
                            PTChietKhau = objNew.GiaVon,
                            TienChietKhau = 0,
                            DonGia = 0,
                            SoThuTu = 1
                        };
                        classHoaDonCT.Add_ChiTietHoaDon(ctHoaDon);
                    }
                    #endregion

                    // tạo lịch sử hđ
                    lstDiary.Add(new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = objNew.ID_DonVi,
                        ID_NhanVien = objNew.ID_NhanVien,
                        LoaiNhatKy = 1,
                        ThoiGian = DateTime.Now,
                        ChucNang = "Điều chỉnh giá vốn",
                        NoiDung = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon + ", ngày lập: " +
                                itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm:"
                                + productCode + " :" + objNew.GiaVon,
                        NoiDungChiTiet = "Tạo mới phiếu điều chỉnh: " + itemBH_HoaDon.MaHoaDon
                                + ", ngày lập: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('"
                                + productCode + "')\">" + productCode + "</a> :" + objNew.GiaVon
                    });
                }

                if (objNew.TonKho != 0)
                {
                    #region phieu kiem hang
                    BH_HoaDon itemBH_HoaDon = new BH_HoaDon { };
                    itemBH_HoaDon.ID = Guid.NewGuid();
                    itemBH_HoaDon.MaHoaDon = classHoaDon.GetAutoCode(9);
                    itemBH_HoaDon.NguoiTao = objNew.NguoiTao;
                    itemBH_HoaDon.DienGiai = "Phiếu kiểm kho được tạo tự động khi thêm mới hàng hóa:" + " " + productCode;
                    itemBH_HoaDon.NgayLapHoaDon = DateTime.Now;
                    itemBH_HoaDon.ID_DonVi = objNew.ID_DonVi;
                    itemBH_HoaDon.ID_NhanVien = objNew.ID_NhanVien;
                    itemBH_HoaDon.TongChiPhi = objNew.TonKho; //slLtang
                    itemBH_HoaDon.TongTienHang = 0; //SLLgiam
                    itemBH_HoaDon.TongGiamGia = objNew.TonKho; // tongchenhlech
                    itemBH_HoaDon.PhaiThanhToan = 0; // Giatritang
                    itemBH_HoaDon.TongChietKhau = 0; //GiaTriGiam
                    itemBH_HoaDon.TongTienThue = 0; //TongTienlech
                    itemBH_HoaDon.ChoThanhToan = false;
                    itemBH_HoaDon.LoaiHoaDon = 9;
                    err = classHoaDon.Add_HoaDon(itemBH_HoaDon);

                    if (err == string.Empty)
                    {
                        BH_HoaDon_ChiTiet ctHoaDon = new BH_HoaDon_ChiTiet
                        {
                            ID = Guid.NewGuid(),
                            ID_DonViQuiDoi = quydoi.ID,
                            ID_HoaDon = itemBH_HoaDon.ID,
                            TienChietKhau = 0, //Tồn kho
                            ThanhTien = objNew.TonKho, // Thực tế
                            SoLuong = objNew.TonKho, // SL lệch
                            ThanhToan = objNew.TonKho * objNew.GiaVon, // Giá trị lệch
                            GiaVon = objNew.GiaVon,
                            SoThuTu = 1,
                            TonLuyKe = objNew.TonKho
                        };
                        err = classHoaDonCT.Add_ChiTietHoaDon(ctHoaDon);
                    }
                    #endregion

                    // tạo lịch sử hđ
                    lstDiary.Add(new HT_NhatKySuDung
                    {
                        ID = Guid.NewGuid(),
                        ID_DonVi = objNew.ID_DonVi,
                        ID_NhanVien = objNew.ID_NhanVien,
                        LoaiNhatKy = 1,
                        ThoiGian = DateTime.Now,
                        ChucNang = "Kiểm kho",
                        NoiDung = "Tạo mới phiếu kiểm kho: " + itemBH_HoaDon.MaHoaDon + ", ngày cân bằng kho: " + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss")
                                    + ", bao gồm: " + productCode + " :" + objNew.TonKho + "/" + objNew.TonKho,
                        NoiDungChiTiet = "Tạo mới phiếu kiểm kho: <a onclick=\"FindKiemKho('"
                                + itemBH_HoaDon.MaHoaDon + "')\">" + itemBH_HoaDon.MaHoaDon + " </a>, ngày cân bằng kho: "
                                + itemBH_HoaDon.NgayLapHoaDon.ToString("dd/MM/yyyy HH:mm:ss") + ", bao gồm: -<a onclick=\"FindMaHangHoa('"
                                + productCode + "')\">"
                                + productCode + "</a> :"
                                + objNew.TonKho + "/" + objNew.TonKho
                    });
                }

                err = SaveDiary.Add_ListDiary(lstDiary);

                if (err == string.Empty)
                {
                    return Json(new { res = true, data = new { ID = product.ID, MaHangHoa = productCode, ID_DonViQuiDoi = quydoi.ID } });
                }
                else
                {
                    return Json(new { res = false, mes = err });
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult ApplyByGroup([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var checkAll = false;
                        var typeProp = 0;// loai thuoctinh (1.Tinh hoahong truocCK)
                        var valUpdate = 0;
                        List<int> arrType = null;
                        List<string> arrID = null;
                        if (data["checkAll"] != null)
                        {
                            checkAll = data["checkAll"].ToObject<bool>();
                        }
                        if (data["typeProp"] != null)
                        {
                            typeProp = data["typeProp"].ToObject<int>();
                        }
                        if (data["arrType"] != null)
                        {
                            arrType = data["arrType"].ToObject<List<int>>(); //loaihang:1.hanghoa, 2.dichvu, 3.combo
                        }
                        if (data["arrID"] != null)
                        {
                            arrID = data["arrID"].ToObject<List<string>>();
                        }
                        if (data["valUpdate"] != null)
                        {
                            valUpdate = data["valUpdate"].ToObject<int>();
                        }

                        var sql = string.Empty;
                        var sqlWhere = string.Empty;
                        switch (typeProp)
                        {
                            case 1:
                                sql = string.Concat(" HoaHongTruocChietKhau = ", valUpdate);
                                break;
                            case 2:
                                sql = string.Concat(" DuocTichDiem = ", valUpdate);
                                break;
                            case 3:
                                sql = string.Concat(" DuocBanTrucTiep = ", valUpdate);
                                break;
                            case 4:
                                sql = string.Concat(" QuanLyBaoDuong = ", valUpdate);
                                break;
                        }

                        if (!checkAll)
                        {
                            if (arrID != null && arrID.Count > 0)
                            {
                                sqlWhere = string.Concat(@" WHERE ID_NhomHang in (select name from dbo.splitstring('", string.Join(",", arrID), "'))");
                            }
                            else
                            {
                                if (arrType != null && arrType.Count > 0)
                                {
                                    sqlWhere = string.Concat(@" WHERE iif(LoaiHangHoa is null, iif(LaHangHoa=0,2,1),LoaiHangHoa)
                                    in (select name from dbo.splitstring('", string.Join(",", arrType), "'))");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(sql))
                        {
                            sql = string.Concat(@" UPDATE DM_HangHoa SET ", sql, sqlWhere);
                            db.Database.ExecuteSqlCommand(sql);
                        }

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
        // DELETE: api/DM_HangHoaAPI/5
        [HttpDelete]
        [ResponseType(typeof(string))]
        public string DeleteDM_HangHoa(Guid idquidoi, Guid idcungloai)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                DonViQuiDoi dvqd = _classDVQD.Get(p => p.ID == idquidoi);
                if (dvqd != null)
                {
                    if (dvqd.LaDonViChuan == true)
                    {
                        List<DonViQuiDoi> lst = _classDVQD.Select_DonViQuiDois_IDHangHoa(dvqd.ID_HangHoa);
                        foreach (var item in lst)
                        {
                            item.Xoa = true;
                            item.MaHangHoa = item.MaHangHoa + "{DEL}";
                            _classDVQD.Update_DonViQuiDoi(item);
                        }
                        var aa = _classDMHH.Get(p => p.ID == dvqd.ID_HangHoa);
                        if (aa.LaChaCungLoai == true)
                        {
                            List<DM_HangHoa> lsthh = _classDMHH.gethanghoacungloainotxoa(idcungloai).ToList();
                            DM_HangHoa ttt = lsthh.Where(p => p.NgayTao > dvqd.NgayTao).OrderBy(p => p.NgayTao).ToList().FirstOrDefault();
                            if (ttt != null)
                            {
                                ttt.LaChaCungLoai = true;
                                ttt.NgayTao = dvqd.NgayTao;
                                _classDMHH.Update_HangHoaKhiNgungKD(ttt, null);
                            }
                        }
                    }
                    else
                    {
                        dvqd.Xoa = true;
                        dvqd.MaHangHoa = dvqd.MaHangHoa + "{DEL}";
                        _classDVQD.Update_DonViQuiDoi(dvqd);
                    }
                }
                return "";
            }
        }
        [HttpDelete]
        [ResponseType(typeof(string))]
        public string DeleteDM_HangHoa_Anh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    DM_HangHoa_Anh lst = db.DM_HangHoa_Anh.Find(id);
                    if (lst != null)
                    {
                        db.DM_HangHoa_Anh.Remove(lst);
                        db.SaveChanges();
                        List<DM_HangHoa_Anh> lstanh = db.DM_HangHoa_Anh.Where(p => p.ID_HangHoa == lst.ID_HangHoa).Where(p => p.SoThuTu > lst.SoThuTu).ToList();
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

        [HttpDelete]
        [ResponseType(typeof(string))]
        public string DeleteDM_HangHoaThaoTac(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    DonViQuiDoi dvqd = _classDVQD.Get(p => p.ID_HangHoa == id);
                    if (dvqd != null)
                    {
                        dvqd.Xoa = true;
                        dvqd.MaHangHoa = dvqd.MaHangHoa + "{DEL}";
                        _classDVQD.Update_DonViQuiDoi(dvqd);
                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }
        [HttpDelete]
        [ResponseType(typeof(string))]
        public string NgungKinhDoanh_HH(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    var id_hanghoa = _classDVQD.Get(p => p.ID == id).ID_HangHoa;
                    DM_HangHoa hh = _classDMHH.Get(p => p.ID == id_hanghoa);
                    if (hh != null)
                    {
                        if (hh.LaChaCungLoai == true)
                        {
                            List<DM_HangHoa> lsthh = _classDMHH.gethanghoacungloainotxoa(hh.ID_HangHoaCungLoai.Value).ToList();
                            DM_HangHoa ttt = lsthh.Where(p => p.NgayTao > hh.NgayTao).OrderBy(p => p.NgayTao).ToList().FirstOrDefault();
                            if (ttt != null)
                            {
                                ttt.LaChaCungLoai = true;
                                _classDMHH.Update_HangHoaKhiChoKD(ttt, null);
                            }
                        }
                        hh.DuocBanTrucTiep = false;
                        hh.TheoDoi = false;
                        _classDMHH.Update_HangHoa_KD(hh, null);
                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }

        [HttpDelete]
        [ResponseType(typeof(string))]
        public string ChoKinhDoanh_HH(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    var id_hanghoa = _classDVQD.Get(p => p.ID == id).ID_HangHoa;
                    DM_HangHoa hh = _classDMHH.Get(p => p.ID == id_hanghoa);
                    if (hh != null)
                    {
                        if (hh.LaChaCungLoai == true)
                        {
                            List<DM_HangHoa> lsthh = _classDMHH.gethanghoacungloainotxoa(hh.ID_HangHoaCungLoai.Value).ToList();
                            DM_HangHoa ttt = lsthh.Where(p => p.NgayTao > hh.NgayTao).OrderBy(p => p.NgayTao).ToList().FirstOrDefault();
                            if (ttt != null)
                            {
                                ttt.LaChaCungLoai = false;
                                _classDMHH.Update_HangHoaKhiChoKD(ttt, null);
                            }
                        }
                        hh.DuocBanTrucTiep = true;
                        hh.TheoDoi = true;
                        _classDMHH.Update_HangHoa_KD(hh, null);
                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }
        //xóa thuộc tính háng hóa
        public IHttpActionResult DeleteDinhLuongDichVu(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDinhLuongDichVu _classDLDV = new classDinhLuongDichVu(db);
                string strDel = _classDLDV.Delete_DinhLuongDichVu(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpGet]
        public bool RestoreProduct(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var sql = " Update DonViQuiDoi set Xoa = 0, MaHangHoa = Replace(MaHangHoa, '{DEL}','') where ID='" + id + "'";
                    db.Database.ExecuteSqlCommand(sql);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public IHttpActionResult DeleteHangHoaThuocTinh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                string strDel = _classDMHH.DeleteHH_ThuocTinh(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion

        #region ###

        [HttpGet, ActionName("CretateMaHangHoa")]
        public string CretateMaHangHoa()
        {
            DateTime _now = DateTime.Now;
            string strMa = "HH" + _now.ToString("yyyyMMddHHmmss");
            return strMa;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Chương trình giá
        public IQueryable<DM_GiaBan_ChiTiet> GetDM_GiaBan_ChiTiet()
        {
            return null;
        }
        #endregion
        #region Trinhpv Dowload teamplate Import DM_HangHoa
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void Download_TeamplateImport(string fileSave)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string filePatch = HttpContext.Current.Server.MapPath("~/Template/ImportExcel/" + fileSave);
                _classOFDCM.downloadFile(filePatch);
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public void Download_fileExcel(string fileSave)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string filePatch = HttpContext.Current.Server.MapPath(fileSave);
                _classOFDCM.downloadFile(filePatch);
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        public void Upload_DMHH(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    String path = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + upload.FileName);
                    upload.SaveAs(path);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public System.Web.Mvc.ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    String path = HttpContext.Current.Server.MapPath("~/Template/ExportExcel/" + upload.FileName);
                    upload.SaveAs(path);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return null;
        }

        [HttpPost]
        public IHttpActionResult ImportExcelDinhLuong(Guid idDonVi, Guid idNhanVien, int? typeUpdate = 0)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<ErrorDMHangHoa> lstError = new List<ErrorDMHangHoa>();
                try
                {
                    var classdoituong = new classDM_DoiTuong(db);
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    string indexErrs = HttpContext.Current.Request.Form["ListErr"];
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;

                            lstErr = classOffice.CheckImportFileDinhLuong(excelstream, indexErrs, idDonVi, idNhanVien, typeUpdate);
                            if (lstErr.Count == 0)
                            {
                                return Json(new { res = true });
                            }
                            else
                            {
                                return Json(new { res = false, mes = "", data = lstErr });
                            }
                        }
                        return Json(new { res = true });
                    }
                    else
                    {
                        ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                        {
                            TenTruongDuLieu = "Exception",
                            ViTri = string.Empty,
                            ThuocTinh = "Exception",
                            DienGiai = "Không có dữ liệu",
                            rowError = -1,
                        };
                        lstError.Add(itemErr);
                        return Json(new
                        {
                            res = false,
                            data = lstError
                        });
                    }
                }
                catch (Exception ex)
                {
                    ErrorDMHangHoa itemErr = new ErrorDMHangHoa()
                    {
                        TenTruongDuLieu = "Exception",
                        ViTri = string.Empty,
                        ThuocTinh = "Exception",
                        DienGiai = ex.InnerException + ex.Message,
                        rowError = -1,
                    };
                    lstError.Add(itemErr);
                    return Json(new { res = false, data = lstError });
                }
            }
        }
        #endregion
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortExcelNhapHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
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
                            string str = _classOFDCM.CheckFileMau_NhapHang(excelstream);
                            if (str == null)
                            {
                                abc = _classOFDCM.checkExcel_NhapHang(excelstream);
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
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_DanhSachHangNhap(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string result = "";
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<DM_HangHoaDTO> abc = new List<DM_HangHoaDTO>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            abc = _classOFDCM.getList_DanhSachHangnhap(excelstream, iddonvi);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortExcelTraHangNhap()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
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
                            string str = _classOFDCM.CheckFileMau_TraHangNhap(excelstream);
                            if (str == null)
                            {
                                abc = _classOFDCM.checkExcel_TraHangNhap(excelstream);
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
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_DanhSachTraHangNhap(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string result = "";
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<DM_HangHoaDTO> abc = new List<DM_HangHoaDTO>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            abc = _classOFDCM.getList_DanhSachTraHangNhap(excelstream, iddonvi);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortExcelToDanhMucHH(Guid ID_DonVi, Guid ID_NhanVien, int LoaiUpdate)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        string SubDomain = CookieStore.GetCookieAes("SubDomain");
                        int gioihan = CuaHangDangKyService.GetGioiHanMatHang(SubDomain);

                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = _classOFDCM.CheckFileMau(excelstream, gioihan);
                            if (str == null)
                            {
                                lstErr = _classOFDCM.checkExcel_HangHoa(excelstream, ID_DonVi, ID_NhanVien, LoaiUpdate);
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
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortExcelToDanhMucHH_LoHang(Guid ID_DonVi, Guid ID_NhanVien, int LoaiUpdate)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        string SubDomain = CookieStore.GetCookieAes("SubDomain");
                        int gioihan = CuaHangDangKyService.GetGioiHanMatHang(SubDomain);

                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            string str = _classOFDCM.CheckFileMau_LoHang(excelstream, gioihan);
                            if (str == null)
                            {
                                lstErr = _classOFDCM.checkExcel_HangHoa_LoHang(excelstream, ID_DonVi, ID_NhanVien, LoaiUpdate);
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
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult insert_TonKhoKhoiTao()
        {
            string result = "";
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    db.Database.ExecuteSqlCommand("Exec update_DanhMucHangHoa");
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.OK, result));
                }
            }
            catch (Exception ex)
            {
                result = ex.InnerException + ex.Message;
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        //xuất file hàng hóa cập nhật
        public IHttpActionResult ExportHangHoa_CapNhat(Guid ID_DonVi, string RownError, int LoaiUpdate)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<exportHangHoaCapNhatPRC> lst = new List<exportHangHoaCapNhatPRC>();
                if (HttpContext.Current.Request.Files.Count != 0)
                {

                    for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        try
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            lst = _classOFDCM.export_HangHoaCapNhat(excelstream, ID_DonVi, RownError, LoaiUpdate);
                        }
                        catch (Exception ex)
                        {
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, ex.Message));
                        }
                    }
                    DataTable excel = _classOFDCM.ToDataTable<exportHangHoaCapNhatPRC>(lst);
                    string fileTeamplate = HttpContext.Current.Server.MapPath("~/Template/ImportExcel/Teamplate_DanhSachHangHoaCapNhat.xlsx");
                    string fileSave = HttpContext.Current.Server.MapPath("~/Template/ImportExcel/DanhSachHangHoaCapNhat.xlsx");
                    _classOFDCM.listToOfficeExcel(fileTeamplate, fileSave, excel, 3, 27, 24, false, null);
                    HttpResponse Response = HttpContext.Current.Response;
                    _classOFDCM.downloadFile(fileSave);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, lst));
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, lst));
            }
        }
        public IHttpActionResult ImfortExcelToBangGia(Guid ID_DonVi, Guid ID_BangGia)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
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
                            string str = _classOFDCM.CheckFileMauBangGia(excelstream);
                            if (str == null)
                            {
                                abc = _classOFDCM.checkExcel_BangGia(excelstream, ID_DonVi, ID_BangGia);
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
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImportHangHoaHoaDon()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
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
                            abc = _classOFDCM.CheckImportExcel_HangHoaHoaDon(excelstream);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_HangHoaHoaDon()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string result = "";
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<DM_HangHoaHoaDon> abc = new List<DM_HangHoaHoaDon>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            abc = _classOFDCM.getList_HangHoaHoaDon(excelstream);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortHangHoa_WithError(Guid ID_DonVi, Guid ID_NhanVien, string RownError, int LoaiUpdate)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            lstErr = _classOFDCM.CheckImport_withError(excelstream, ID_DonVi, ID_NhanVien, RownError, LoaiUpdate);
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
                    return ActionTrueNotData(string.Empty);
                }
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortHangHoa_LoHang_WithError(Guid ID_DonVi, Guid ID_NhanVien, string RownError, int LoaiUpdate)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                List<ErrorDMHangHoa> lstErr = new List<ErrorDMHangHoa>();
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            lstErr = _classOFDCM.CheckImport_LoHang_withError(excelstream, ID_DonVi, ID_NhanVien, RownError, LoaiUpdate);
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
                    return ActionTrueNotData(string.Empty);
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImportBangGiaBoQuaLoi(Guid ID_DonVi, string RownError, Guid ID_BangGia)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string result = "";
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            _classOFDCM.CheckImportBG_withError(excelstream, ID_DonVi, RownError, ID_BangGia);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult PostDM_ThuocTinh(DM_ThuocTinh dM_ThuocTinh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                dM_ThuocTinh.ID = Guid.NewGuid();

                string strIns = _classDMHH.add_ThuocTinh(dM_ThuocTinh);
                if (strIns != null && strIns != string.Empty && strIns.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = dM_ThuocTinh.ID }, dM_ThuocTinh);
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult POST_DM_HangHoa_ViTri([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                DM_HangHoa_ViTri objnew = data["objDMHHViTri"].ToObject<DM_HangHoa_ViTri>();
                objnew.ID = Guid.NewGuid();
                objnew.NgayTao = DateTime.Now;
                string strIns = _classDMHH.addViTriHangHoa(objnew);
                if (strIns != null && strIns != string.Empty && strIns.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objnew.ID }, objnew);
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult PutDM_ThuocTinh([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                Guid id = data["id"].ToObject<Guid>();
                DM_ThuocTinh dM_ThuocTinh = JsonConvert.DeserializeObject<DM_ThuocTinh>(data["ThuocTinh"].ToString());
                if (id != dM_ThuocTinh.ID)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "ID thuộc tính cần cập nhật không trùng khớp với ID dữ liệu"));
                }
                string strUpd = _classDMHH.Update_thuocTinh(dM_ThuocTinh);
                if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return CreatedAtRoute("DefaultApi", new { id = dM_ThuocTinh.ID }, dM_ThuocTinh);
            }
        }

        [HttpPost, HttpGet, HttpPut]
        public IHttpActionResult PUT_DM_HangHoa_ViTri([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                DM_HangHoa_ViTri objnew = data["objDMHHViTri"].ToObject<DM_HangHoa_ViTri>();
                string strUpd = _classDMHH.UpdateViTriHangHoa(objnew);
                if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objnew.ID }, objnew);
            }
        }

        [ResponseType(typeof(string))]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteDM_ThuocTinh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var _classDMHH = new ClassDM_HangHoa(db);
                string strDel = _classDMHH.Delete_ThuocTinh(id);
                if (strDel != null && strDel != string.Empty && strDel.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ImfortExcelKiemKho()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
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
                            string str = _classOFDCM.CheckFileMau_KiemKho(excelstream);
                            if (str == null)
                            {
                                abc = _classOFDCM.checkExcel_KiemKho(excelstream);
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
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult getList_DanhSachHangKiemKho(Guid iddonvi, DateTime timeKK)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                Class_officeDocument _classOFDCM = new Class_officeDocument(db);
                string result = "";
                try
                {
                    if (HttpContext.Current.Request.Files.Count != 0)
                    {
                        List<Report_HangHoa_Chuyenhang_Import> abc = new List<Report_HangHoa_Chuyenhang_Import>();
                        for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                        {
                            var file = HttpContext.Current.Request.Files[i];
                            System.IO.Stream excelstream = file.InputStream;
                            abc = _classOFDCM.getList_DanhSachHangKiemKho(excelstream, iddonvi, timeKK);
                        }
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, abc));
                    }
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result));
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Compress.DeflateCompression]
        public List<SP_DM_LoHang> SP_GetAll_DMLoHang(Guid iddonvi, DateTime? timeChotSo = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    if (db != null)
                    {
                        if (timeChotSo == null)
                        {
                            timeChotSo = new DateTime(2016, 1, 1);
                        }
                    }

                    List<SqlParameter> paramlist = new List<SqlParameter>();
                    paramlist.Add(new SqlParameter("ID_ChiNhanh", iddonvi));
                    paramlist.Add(new SqlParameter("timeChotSo", timeChotSo));
                    var listTon = db.Database.SqlQuery<SP_DM_LoHang>("exec SP_GetAll_DMLoHang_TonKho @ID_ChiNhanh, @timeChotSo", paramlist.ToArray()).ToList();
                    db.Database.CommandTimeout = 3000;
                    return listTon;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("DM_HangHoaAPI_SP_GetAll_DMLoHang: " + ex.InnerException + ex.Message);
                    return new List<SP_DM_LoHang>();
                }
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IQueryable<Object> GetAll_DMLoHang(Guid iddonvi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var data = from hd in db.BH_HoaDon
                               join ct in db.BH_HoaDon_ChiTiet
                               on hd.ID equals ct.ID_HoaDon
                               join lo in db.DM_LoHang on ct.ID_LoHang equals lo.ID
                               where (hd.ChoThanhToan == null || hd.ChoThanhToan == false) && hd.ID_DonVi == iddonvi
                               group new { hd, ct, lo } by new
                               {
                                   ID = lo.ID,
                                   MaLoHang = lo.MaLoHang,
                                   ID_DonViQuiDoi = ct.ID_DonViQuiDoi,
                                   ID_HangHoa = lo.ID_HangHoa,
                                   NgaySanXuat = lo.NgaySanXuat,
                                   NgayHetHan = lo.NgayHetHan,
                               }
                               into g
                               select new
                               {
                                   ID = g.Key.ID,
                                   ID_HangHoa = g.Key.ID_HangHoa,
                                   MaLoHang = g.Key.MaLoHang,
                                   NgaySanXuat = g.Key.NgaySanXuat,
                                   NgayHetHan = g.Key.NgayHetHan,
                                   ID_DonViQuiDoi = g.Key.ID_DonViQuiDoi,
                                   // sum(NhapHang)- sum(TraHangNhap)- sum(BanHang)+ sum(TraHang)
                                   TonKho = (double?)g.Where(x => x.hd.LoaiHoaDon == 4).Sum(x => x.ct.SoLuong) ?? 0
                                   - (double?)g.Where(x => x.hd.LoaiHoaDon == 7).Sum(x => x.ct.SoLuong) ?? 0
                                   - (double?)g.Where(x => x.hd.LoaiHoaDon == 1).Sum(x => x.ct.SoLuong) ?? 0
                                   + (double?)g.Where(x => x.hd.LoaiHoaDon == 6).Sum(x => x.ct.SoLuong) ?? 0,
                               };

                    if (data != null && data.Count() > 0)
                    {
                        return data;
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
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult getListDM_LoHang(Guid ID_DonViQuiDoi, Guid ID_ChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var tbl_timeCSt = from cs in db.ChotSo
                                  where cs.ID_DonVi == ID_ChiNhanh
                                  select new
                                  {
                                      cs.NgayChotSo
                                  };
                string timeCS = "2016-01-01";
                if (tbl_timeCSt != null && tbl_timeCSt.Count() > 0)
                {
                    timeCS = tbl_timeCSt.FirstOrDefault().NgayChotSo.ToString("yyyy-MM-dd");
                }

                List<SqlParameter> sqlPRM = new List<SqlParameter>();
                sqlPRM.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sqlPRM.Add(new SqlParameter("ID_DonViQuiDoi", ID_DonViQuiDoi));
                sqlPRM.Add(new SqlParameter("timeChotSo", timeCS));
                List<ListDM_LoHangDTO> lst = db.Database.SqlQuery<ListDM_LoHangDTO>("exec getList_DMLoHangByIDDonViQD @ID_ChiNhanh, @ID_DonViQuiDoi, @timeChotSo", sqlPRM.ToArray()).ToList();
                JsonResultExample<ListDM_LoHangDTO> json = new JsonResultExample<ListDM_LoHangDTO>
                {
                    LstData = lst
                };
                return Json(json);
            }
        }

        [HttpGet, HttpPost]
        public void UpdateHangHoa_TichDiem(int tichdiem)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                db.Database.ExecuteSqlCommand(" Update DM_HangHoa set DuocTichDiem =" + tichdiem);
            }
        }
    }


    public class ListTonTheoLoHang
    {
        public string MaLoHang { get; set; }
        public Guid? ID_LoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double TonKho { get; set; }
    }

    public class List_TonKho
    {
        public string TenDonVi { get; set; }
        public double TonKho { get; set; }
        public double SLDatHang { get; set; }
    }

    public class JsonResultExampleHH
    {
        public int Rowcount { get; set; }
        public double? TongTon { get; set; }
        public double pageCount { get; set; }
        public List<DM_HangHoaDTO> lstHH { get; set; }
    }

    public class JsonResultExampleDM_Lo
    {
        public int Rowcount { get; set; }
        public double pageCount { get; set; }
        public List<BCDM_LoHangDTO> lstHH { get; set; }
    }

    public class jqAutoResultNH
    {
        public string label { get; set; }
        public string value { get; set; }
        public string actual { get; set; }
        public DM_HangHoaSearch data { get; set; }
    }

    public class HangHoa_ThuocTinhDTO
    {
        public Guid ID_ThuocTinh { get; set; }
        public string GiaTri { get; set; }
        public int? ThuTuNhap { get; set; }
        public string TenThuocTinh { get; set; }
        public bool checkboxChecked { get; set; }
    }

}
