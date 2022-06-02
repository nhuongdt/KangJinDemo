using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Model;
using libHT;
using libDM_DoiTuong;
using libDM_NhomDoiTuong;
using System.Data.SqlClient;
using System.Data.Entity;
using libDM_DonVi;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class HT_ThietLapAPIController : BaseApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpGet]
        public bool DeleteDLHeThong()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classHTPM = new classHT_CauHinhPhanMem(db);
                return _classHTPM.DeleteData();
            }
        }
        [HttpGet]
        public string DeleteDuLieuHeThong(int checkhh, int checkkh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    if (db != null)
                    {
                        List<SqlParameter> paramlist = new List<SqlParameter>();
                        paramlist.Add(new SqlParameter("CheckHH", checkhh));
                        paramlist.Add(new SqlParameter("CheckKH", checkkh));
                        db.Database.ExecuteSqlCommand("exec XoaDuLieuHeThong @CheckHH, @CheckKH", paramlist.ToArray());

                        //db.Database.CommandTimeout = 60000;
                    }
                    return "";
                }
            }
        }

        [ResponseType(typeof(string))]
        [HttpPut]
        public IHttpActionResult Put_HT_CauHinhPhanMem1(HT_CauHinhPhanMem hT_CauHinhPhanMem)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classHTPM = new classHT_CauHinhPhanMem(db);
                if (!ModelState.IsValid)
                {
                    // return BadRequest(ModelState);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                string strUpd = _classHTPM.Update_ThietLap(hT_CauHinhPhanMem);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [ResponseType(typeof(string))]
        [HttpPut]
        public IHttpActionResult Put_HT_CauHinhPhanMem([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classHTPM = new classHT_CauHinhPhanMem(db);
                classDM_DonVi _classDV = new classDM_DonVi(db);
                Guid iddonvi = data["id_donvi"].ToObject<Guid>();
                string strUpd = "";
                HT_CauHinhPhanMem objThietLap = data["objThietLap"].ToObject<HT_CauHinhPhanMem>();
                HT_CauHinhPhanMem hT_CauHinhPhanMem = _classHTPM.SelectByIDDonVi(iddonvi);
                if (hT_CauHinhPhanMem != null)
                {
                    hT_CauHinhPhanMem.GiaVonTrungBinh = objThietLap.GiaVonTrungBinh;
                    hT_CauHinhPhanMem.CoDonViTinh = objThietLap.CoDonViTinh;
                    hT_CauHinhPhanMem.DatHang = objThietLap.DatHang;
                    hT_CauHinhPhanMem.XuatAm = objThietLap.XuatAm;
                    hT_CauHinhPhanMem.DatHangXuatAm = objThietLap.DatHangXuatAm;
                    hT_CauHinhPhanMem.ThayDoiThoiGianBanHang = objThietLap.ThayDoiThoiGianBanHang;
                    hT_CauHinhPhanMem.TinhNangTichDiem = objThietLap.TinhNangTichDiem;
                    hT_CauHinhPhanMem.GioiHanThoiGianTraHang = objThietLap.GioiHanThoiGianTraHang;
                    hT_CauHinhPhanMem.SanPhamCoThuocTinh = objThietLap.SanPhamCoThuocTinh;
                    hT_CauHinhPhanMem.BanVaChuyenKhiHangDaDat = objThietLap.BanVaChuyenKhiHangDaDat;
                    hT_CauHinhPhanMem.TinhNangSanXuatHangHoa = objThietLap.TinhNangSanXuatHangHoa;
                    hT_CauHinhPhanMem.SuDungCanDienTu = objThietLap.SuDungCanDienTu;
                    hT_CauHinhPhanMem.KhoaSo = objThietLap.KhoaSo;
                    hT_CauHinhPhanMem.InBaoGiaKhiBanHang = objThietLap.InBaoGiaKhiBanHang;
                    hT_CauHinhPhanMem.QuanLyKhachHangTheoDonVi = objThietLap.QuanLyKhachHangTheoDonVi;
                    hT_CauHinhPhanMem.SoLuongTrenChungTu = objThietLap.SoLuongTrenChungTu;
                    hT_CauHinhPhanMem.KhuyenMai = objThietLap.KhuyenMai;
                    hT_CauHinhPhanMem.LoHang = objThietLap.LoHang;
                    hT_CauHinhPhanMem.SuDungMauInMacDinh = objThietLap.SuDungMauInMacDinh;
                    hT_CauHinhPhanMem.ApDungGopKhuyenMai = objThietLap.ApDungGopKhuyenMai;
                    hT_CauHinhPhanMem.ThongTinChiTietNhanVien = objThietLap.ThongTinChiTietNhanVien;
                    hT_CauHinhPhanMem.BanHangOffline = objThietLap.BanHangOffline;
                    hT_CauHinhPhanMem.ThoiGianNhacHanSuDungLo = objThietLap.ThoiGianNhacHanSuDungLo;
                    hT_CauHinhPhanMem.SuDungMaChungTu = objThietLap.SuDungMaChungTu;
                    strUpd = _classHTPM.Update_ThietLap(hT_CauHinhPhanMem);
                }
                else
                {
                    objThietLap.ID = Guid.NewGuid();
                    objThietLap.ID_DonVi = iddonvi;
                    strUpd = _classHTPM.add_ThietLap(objThietLap);
                }

                // lohang, machungtu: setup all ChiNhanh same
                db.Database.ExecuteSqlCommand("UPDATE HT_CauHinhPhanMem set LoHang={0}, SuDungMaChungTu={1}, ChoPhepTrungSoDienThoai ={2}", objThietLap.LoHang, objThietLap.SuDungMaChungTu, objThietLap.ChoPhepTrungSoDienThoai);
                //var dv = _classDV.Gets(null);
                //foreach (var item in dv)
                //{

                //    HT_CauHinhPhanMem objUpdate = _classHTPM.SelectByIDDonVi(item.ID);
                //    objUpdate.LoHang = objThietLap.LoHang;
                //    _classHTPM.Update_ThietLap(objUpdate);
                //}

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPost]
        public IHttpActionResult ImageUpload(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
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
                        var filenameImage = "Logo.jpg";
                        //var filename = filenameImage.ToString();
                        string str = CookieStore.GetCookieAes("SubDomain");
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/imageLogo/" + str + "/")))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/imageLogo/" + str + "/"));
                        }

                        path = Path.Combine(HttpContext.Current.Server.MapPath("~/imageLogo/" + str + "/"), filenameImage);
                        System.IO.File.Delete(path);
                        file.SaveAs(path);
                        result = "/imageLogo/" + str + "/" + filenameImage;
                        //Add db table Anh
                        HT_CongTy objUpd = db.HT_CongTy.Find(id);
                        objUpd.DiaChiNganHang = result;
                        _classHTCT.Update_HoaDon(objUpd);
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
        public IHttpActionResult UpdateAnhCongTy([FromBody] List<string> files, Guid id)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    HT_CongTy objUpd = db.HT_CongTy.Find(id);
                    for (int i = 0; i < files.Count; i++)
                    {
                        objUpd.DiaChiNganHang = files[i];
                    }
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return Ok("");
        }

        [ResponseType(typeof(string))]
        [HttpPut]
        public IHttpActionResult Put_HT_ThongTinCuaHang1(HT_CongTy hT_CongTy)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                if (!ModelState.IsValid)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                string strUpd = _classHTCT.Update_HoaDon(hT_CongTy);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return CreatedAtRoute("DefaultApi", new { id = hT_CongTy.ID }, hT_CongTy);
            }
        }

        [ResponseType(typeof(string))]
        [HttpPut]
        public IHttpActionResult Put_HT_ThongTinCuaHang([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _classHTCT = new ClassHT_CongTy(db);
                string strUpd = "";
                HT_CongTy objThongTinCH = data["objThongTinCH"].ToObject<HT_CongTy>();
                strUpd = _classHTCT.Update_HoaDon(objThongTinCH);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objThongTinCH.ID }, objThongTinCH);
            }
        }

        [HttpPost, ActionName("Post_TichDiem")]
        [ResponseType(typeof(HT_CauHinh_TichDiemChiTiet))]
        public IHttpActionResult Post_TichDiem([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                //Guid idnhomkh = data["idnhomkh"].ToObject<Guid>();
                HT_CauHinh_TichDiemChiTiet objTichDiem = data["objTichDiem"].ToObject<HT_CauHinh_TichDiemChiTiet>();
                List<HT_CauHinh_TichDiemApDung> objNhomKhachHang = new List<HT_CauHinh_TichDiemApDung>();
                if (data["objNhomKhachHang"] != null)
                {
                    objNhomKhachHang = data["objNhomKhachHang"].ToObject<List<HT_CauHinh_TichDiemApDung>>();
                }
                HT_CauHinh_TichDiemChiTiet obj = new HT_CauHinh_TichDiemChiTiet();
                obj.ID = Guid.NewGuid();
                obj.ID_CauHinh = objTichDiem.ID_CauHinh;
                obj.TyLeDoiDiem = objTichDiem.TyLeDoiDiem;
                obj.ThanhToanBangDiem = objTichDiem.ThanhToanBangDiem;
                obj.DiemThanhToan = objTichDiem.DiemThanhToan;
                obj.TienThanhToan = objTichDiem.TienThanhToan;
                obj.TichDiemGiamGia = objTichDiem.TichDiemGiamGia;
                obj.TichDiemHoaDonDiemThuong = objTichDiem.TichDiemHoaDonDiemThuong;
                obj.ToanBoKhachHang = objTichDiem.ToanBoKhachHang;
                obj.SoLanMua = objTichDiem.SoLanMua;
                obj.KhoiTaoTichDiem = objTichDiem.KhoiTaoTichDiem;

                string strIns = _classCHPM.add_TichDiem(obj);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    foreach (var item in objNhomKhachHang)
                    {
                        HT_CauHinh_TichDiemApDung objApDung = new HT_CauHinh_TichDiemApDung
                        {
                            ID = Guid.NewGuid(),
                            ID_TichDiem = obj.ID,
                            ID_NhomDoiTuong = item.ID
                        };
                        _classCHPM.add_TichDiem_AD(objApDung);
                    }
                    return CreatedAtRoute("DefaultApi", new { id = obj.ID }, obj);
                }
            }
        }

        [HttpPost, ActionName("Post_GioiHanTH")]
        [ResponseType(typeof(HT_CauHinh_GioiHanTraHang))]
        public IHttpActionResult Post_GioiHanTH([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                //Guid idnhomkh = data["idnhomkh"].ToObject<Guid>();
                HT_CauHinh_GioiHanTraHang objTraHang = data["objTraHang"].ToObject<HT_CauHinh_GioiHanTraHang>();
                HT_CauHinh_GioiHanTraHang obj = new HT_CauHinh_GioiHanTraHang();
                obj.ID = Guid.NewGuid();
                obj.ID_CauHinh = objTraHang.ID_CauHinh;
                obj.SoNgayGioiHan = objTraHang.SoNgayGioiHan;
                obj.ChoPhepTraHang = objTraHang.ChoPhepTraHang;
                string strIns = _classCHPM.add_GioiHanTH(obj);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = obj.ID }, obj);
                }
            }
        }

        // PUT: api/DM_ViTriAPI/5
        [ResponseType(typeof(string))]
        [HttpPost, HttpPut]
        public IHttpActionResult Put_TichDiem([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                Guid id = data["id"].ToObject<Guid>();
                //Guid idnhomkh = data["idnhomkh"].ToObject<Guid>();
                HT_CauHinh_TichDiemChiTiet objTichDiem = data["objTichDiem"].ToObject<HT_CauHinh_TichDiemChiTiet>();
                List<HT_CauHinh_TichDiemApDung> objNhomKhachHang = new List<HT_CauHinh_TichDiemApDung>();
                if (data["objNhomKhachHang"] != null)
                {
                    objNhomKhachHang = data["objNhomKhachHang"].ToObject<List<HT_CauHinh_TichDiemApDung>>();
                }
                //DM_ViTri.ID = id;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strIns = _classCHPM.Update_TichDiem(objTichDiem);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    db.HT_CauHinh_TichDiemApDung.RemoveRange(db.HT_CauHinh_TichDiemApDung.Where(p => p.ID_TichDiem == objTichDiem.ID));
                    db.SaveChanges();
                    foreach (var item in objNhomKhachHang)
                    {
                        HT_CauHinh_TichDiemApDung objApDung = new HT_CauHinh_TichDiemApDung
                        {
                            ID = Guid.NewGuid(),
                            ID_TichDiem = objTichDiem.ID,
                            ID_NhomDoiTuong = item.ID
                        };
                        _classCHPM.add_TichDiem_AD(objApDung);
                    }
                    return CreatedAtRoute("DefaultApi", new { id = objTichDiem.ID }, objTichDiem);
                }
            }
        }

        #region HT_MaChungTu
        public IHttpActionResult Get_HTMaChungTu([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var htMa = (from ht in db.HT_MaChungTu
                                join loaict in db.DM_LoaiChungTu on ht.ID_LoaiChungTu equals loaict.ID
                                select new
                                {
                                    ID_LoaiChungTu = ht.ID_LoaiChungTu,
                                    MaLoaiChungTu = loaict.MaLoaiChungTu,
                                    SuDungMaDonVi = ht.SuDungMaDonVi,
                                    KiTuNganCach1 = ht.KiTuNganCach1,
                                    KiTuNganCach2 = ht.KiTuNganCach2,
                                    KiTuNganCach3 = ht.KiTuNganCach3,
                                    NgayThangNam = ht.NgayThangNam,
                                    DoDaiSTT = ht.DoDaiSTT,
                                    ApDung = true,
                                    LoaiChungTu = loaict.TenLoaiChungTu,
                                }).ToList();
                    return Json(new { res = true, data = htMa });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, data = "Get_HTMaChungTu " + e.Message + e.InnerException });
                }
            }
        }

        [HttpPost]
        public IHttpActionResult Post_HTMaChungTu([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                string strIns = string.Empty;
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                // delete & add again
                db.Database.ExecuteSqlCommand("delete from HT_MaChungTu");
                List<HT_MaChungTu> lst = new List<HT_MaChungTu>();
                if (data["lstThietLap"] != null)
                {
                    lst = data["lstThietLap"].ToObject<List<HT_MaChungTu>>();
                }
                foreach (var item in lst)
                {
                    HT_MaChungTu objNew = new HT_MaChungTu();
                    objNew.ID = Guid.NewGuid();
                    objNew.ID_LoaiChungTu = item.ID_LoaiChungTu;
                    objNew.SuDungMaDonVi = item.SuDungMaDonVi;
                    objNew.KiTuNganCach1 = item.KiTuNganCach1;
                    objNew.MaLoaiChungTu = item.MaLoaiChungTu;
                    objNew.KiTuNganCach2 = item.KiTuNganCach2;
                    objNew.NgayThangNam = item.NgayThangNam;
                    objNew.KiTuNganCach3 = item.KiTuNganCach3;
                    objNew.DoDaiSTT = item.DoDaiSTT;
                    strIns += _classCHPM.Insert_HTMaChungTu(objNew);
                }
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { string.Empty }, string.Empty);
                }
            }
        }
        #endregion

        [AcceptVerbs("POST", "GET")]
        public string UpdateSoNgayNhacLoHang(int ngay)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                classDM_DonVi _classDV = new classDM_DonVi(db);
                if (db == null)
                {
                    return "Error";
                }
                else
                {
                    var dv = _classDV.Gets(null);
                    foreach (var item in dv)
                    {
                        HT_CauHinhPhanMem objUpdate = _classCHPM.SelectByIDDonVi(item.ID);
                        objUpdate.ThoiGianNhacHanSuDungLo = ngay;
                        _classCHPM.Update_ThietLap(objUpdate);
                    }
                    return "";
                }
            }
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult Put_GioiHanTH([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                Guid id = data["id"].ToObject<Guid>();
                //Guid idnhomkh = data["idnhomkh"].ToObject<Guid>();
                HT_CauHinh_GioiHanTraHang objTraHang = data["objTraHang"].ToObject<HT_CauHinh_GioiHanTraHang>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strIns = _classCHPM.Update_GioiHanTH(objTraHang);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = objTraHang.ID }, objTraHang);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GetCauHinhAllChiNhanh()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var data = db.HT_CauHinhPhanMem.Select(x =>
                     new
                     {
                         x.ID,
                         x.ID_DonVi,
                         x.GiaVonTrungBinh,
                         x.CoDonViTinh,
                         x.DatHang,
                         x.XuatAm,
                         x.DatHangXuatAm,
                         x.SoLuongTrenChungTu,
                         x.ThayDoiThoiGianBanHang,
                         x.TinhNangTichDiem,
                         x.GioiHanThoiGianTraHang,
                         x.SanPhamCoThuocTinh,
                         x.BanVaChuyenKhiHangDaDat,
                         x.TinhNangSanXuatHangHoa,
                         x.SuDungCanDienTu,
                         x.KhoaSo,
                         x.InBaoGiaKhiBanHang,
                         x.QuanLyKhachHangTheoDonVi,
                         x.KhuyenMai,
                         x.LoHang,
                         x.ApDungGopKhuyenMai,
                         x.SuDungMauInMacDinh,
                         x.ThongTinChiTietNhanVien,
                         x.BanHangOffline,
                         x.ThoiGianNhacHanSuDungLo,
                         x.SuDungMaChungTu,
                         x.ChoPhepTrungSoDienThoai,
                     }).ToList();
                    return ActionTrueData(data);

                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        //get cấu hình hệ thống (phuongnd)
        [ResponseType(typeof(HT_CauHinhPhanMem))]
        public IHttpActionResult GetCauHinhHeThong(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                HT_CauHinhPhanMem cauhinh = _classCHPM.SelectByIDDonVi(id);
                HT_CauHinhPhanMem ch = new HT_CauHinhPhanMem();
                if (cauhinh == null)
                {
                    //ch.ID = Guid.NewGuid();
                    ch.ID_DonVi = id;
                    ch.GiaVonTrungBinh = false;
                    ch.CoDonViTinh = false;
                    ch.DatHang = false;
                    ch.XuatAm = false;
                    ch.DatHangXuatAm = false;
                    ch.SoLuongTrenChungTu = false;
                    ch.ThayDoiThoiGianBanHang = false;
                    ch.TinhNangTichDiem = false;
                    ch.GioiHanThoiGianTraHang = false;
                    ch.SanPhamCoThuocTinh = false;
                    ch.BanVaChuyenKhiHangDaDat = false;
                    ch.TinhNangSanXuatHangHoa = false;
                    ch.SuDungCanDienTu = false;
                    ch.KhoaSo = false;
                    ch.InBaoGiaKhiBanHang = false;
                    ch.QuanLyKhachHangTheoDonVi = false;
                    ch.KhuyenMai = false;
                    ch.LoHang = false;
                    ch.ApDungGopKhuyenMai = false;
                    ch.SuDungMauInMacDinh = false;
                    ch.ApDungGopKhuyenMai = false;
                    ch.ThongTinChiTietNhanVien = false;
                    ch.BanHangOffline = true; //default= true
                    ch.ThoiGianNhacHanSuDungLo = 1;
                    ch.SuDungMaChungTu = 0;
                    ch.ChoPhepTrungSoDienThoai = 0;
                }
                else
                {
                    ch.ID = cauhinh.ID;
                    ch.ID_DonVi = cauhinh.ID_DonVi;
                    ch.GiaVonTrungBinh = cauhinh.GiaVonTrungBinh;
                    ch.CoDonViTinh = cauhinh.CoDonViTinh;
                    ch.DatHang = cauhinh.DatHang;
                    ch.XuatAm = cauhinh.XuatAm;
                    ch.DatHangXuatAm = cauhinh.DatHangXuatAm;
                    ch.SoLuongTrenChungTu = cauhinh.SoLuongTrenChungTu;
                    ch.ThayDoiThoiGianBanHang = cauhinh.ThayDoiThoiGianBanHang;
                    ch.TinhNangTichDiem = cauhinh.TinhNangTichDiem;
                    ch.GioiHanThoiGianTraHang = cauhinh.GioiHanThoiGianTraHang;
                    ch.SanPhamCoThuocTinh = cauhinh.SanPhamCoThuocTinh;
                    ch.BanVaChuyenKhiHangDaDat = cauhinh.BanVaChuyenKhiHangDaDat;
                    ch.TinhNangSanXuatHangHoa = cauhinh.TinhNangSanXuatHangHoa;
                    ch.SuDungCanDienTu = cauhinh.SuDungCanDienTu;
                    ch.KhoaSo = cauhinh.KhoaSo;
                    ch.InBaoGiaKhiBanHang = cauhinh.InBaoGiaKhiBanHang;
                    ch.QuanLyKhachHangTheoDonVi = cauhinh.QuanLyKhachHangTheoDonVi;
                    ch.KhuyenMai = cauhinh.KhuyenMai;
                    ch.LoHang = cauhinh.LoHang;
                    ch.ApDungGopKhuyenMai = cauhinh.ApDungGopKhuyenMai;
                    ch.SuDungMauInMacDinh = cauhinh.SuDungMauInMacDinh;
                    ch.ApDungGopKhuyenMai = cauhinh.ApDungGopKhuyenMai;
                    ch.ThongTinChiTietNhanVien = cauhinh.ThongTinChiTietNhanVien;
                    ch.BanHangOffline = cauhinh.BanHangOffline ?? true;
                    ch.ThoiGianNhacHanSuDungLo = cauhinh.ThoiGianNhacHanSuDungLo;
                    ch.SuDungMaChungTu = cauhinh.SuDungMaChungTu;
                    ch.ChoPhepTrungSoDienThoai = cauhinh.ChoPhepTrungSoDienThoai;
                }
                return Ok(ch);
            }
        }

        [ResponseType(typeof(HT_CongTy))]
        public IHttpActionResult GetThongTinCuaHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassHT_CongTy _ClassHTCT = new ClassHT_CongTy(db);
                IQueryable<HT_CongTy> congty = _ClassHTCT.Gets(null);
                List<HT_CongTy> ctyc = new List<HT_CongTy>();
                string str = CookieStore.GetCookieAes("SubDomain");
                foreach (var item in congty)
                {
                    HT_CongTy cty = new HT_CongTy();
                    cty.ID = item.ID;
                    cty.Website = item.Website;
                    cty.TenCongTy = item.TenCongTy;
                    cty.DiaChi = item.DiaChi;
                    cty.SoDienThoai = item.SoDienThoai;
                    cty.DiaChiNganHang = item.DiaChiNganHang;
                    cty.HanSuDung = Model_banhang24vn.DAL.CuaHangDangKyService.Get(str).HanSuDung.Value;
                    cty.NgayCongChuan = item.NgayCongChuan;
                    ctyc.Add(cty);
                }
                return Ok(ctyc);
            }
        }

        [ResponseType(typeof(HT_CauHinhPhanMem))]
        public IHttpActionResult GetTichDiemByID_CauHinh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                ClassDM_NhomDoiTuong _classDMDT = new ClassDM_NhomDoiTuong(db);
                HT_CauHinh_TichDiemChiTiet objTichDiem = _classCHPM.selectbyID_CauHinh(id);
                HT_CauHinh_TichDiemChiTietDTO ch = new HT_CauHinh_TichDiemChiTietDTO();
                List<HT_CauHinh_TichDiemApDungDTO> CauHinhAD = new List<HT_CauHinh_TichDiemApDungDTO>();
                if (objTichDiem != null)
                {
                    ch.ID = objTichDiem.ID;
                    ch.ID_CauHinh = objTichDiem.ID_CauHinh;
                    ch.TyLeDoiDiem = objTichDiem.TyLeDoiDiem;
                    ch.ThanhToanBangDiem = objTichDiem.ThanhToanBangDiem;
                    ch.DiemThanhToan = objTichDiem.DiemThanhToan;
                    ch.TienThanhToan = objTichDiem.TienThanhToan;
                    ch.TichDiemGiamGia = objTichDiem.TichDiemGiamGia;
                    ch.TichDiemHoaDonDiemThuong = objTichDiem.TichDiemHoaDonDiemThuong;
                    ch.ToanBoKhachHang = objTichDiem.ToanBoKhachHang;
                    ch.SoLanMua = objTichDiem.SoLanMua;
                    ch.TichDiemHoaDonGiamGia = objTichDiem.TichDiemHoaDonGiamGia;
                    ch.KhoiTaoTichDiem = objTichDiem.KhoiTaoTichDiem;

                    if (objTichDiem.ToanBoKhachHang == false)
                    {
                        //ch.ID_NhomDoiTuong = classHT_CauHinhPhanMem.GetsTichDiemAD(p => p.ID_TichDiem == objTichDiem.ID).FirstOrDefault().ID_NhomDoiTuong;

                        List<HT_CauHinh_TichDiemApDung> objAD = _classCHPM.GetsTichDiemAD(idqd => idqd.ID_TichDiem == objTichDiem.ID);
                        foreach (var item in objAD)
                        {
                            HT_CauHinh_TichDiemApDungDTO cauHinhAD = new HT_CauHinh_TichDiemApDungDTO();
                            cauHinhAD.ID = item.ID_NhomDoiTuong;
                            cauHinhAD.ID_TichDiem = item.ID_TichDiem;
                            cauHinhAD.TenNhomDoiTuong = _classDMDT.Gets(p => p.ID == item.ID_NhomDoiTuong).FirstOrDefault().TenNhomDoiTuong;
                            CauHinhAD.Add(cauHinhAD);
                        }
                    }
                }
                else
                {
                    ch.ID_CauHinh = id;
                    ch.TyLeDoiDiem = 0;
                    ch.ThanhToanBangDiem = false;
                    ch.DiemThanhToan = 0;
                    ch.TienThanhToan = 0;
                    ch.TichDiemGiamGia = false;
                    ch.TichDiemHoaDonGiamGia = false;
                    ch.TichDiemHoaDonDiemThuong = false;
                    ch.ToanBoKhachHang = true;
                    ch.SoLanMua = 0;
                    ch.KhoiTaoTichDiem = true;
                }
                ch.HT_CauHinh_TichDiemApDung = CauHinhAD;
                return Ok(ch);
            }
        }

        [ResponseType(typeof(HT_CauHinhPhanMem))]
        public IHttpActionResult GetGioiHanTraHangByID_CauHinh(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_CauHinhPhanMem _classCHPM = new classHT_CauHinhPhanMem(db);
                HT_CauHinh_GioiHanTraHang objTraHang = _classCHPM.selectGioiHanTHbyID_CauHinh(id);
                HT_CauHinh_GioiHanTraHangDTO ch = new HT_CauHinh_GioiHanTraHangDTO();
                if (objTraHang != null)
                {
                    ch.ID = objTraHang.ID;
                    ch.ID_CauHinh = objTraHang.ID_CauHinh;
                    ch.SoNgayGioiHan = objTraHang.SoNgayGioiHan;
                    ch.ChoPhepTraHang = objTraHang.ChoPhepTraHang;
                }
                else
                {
                    ch.ID_CauHinh = id;
                    ch.SoNgayGioiHan = 0;
                    ch.ChoPhepTraHang = false;
                }
                return Ok(ch);
            }
        }

        //save file to server (phuongnd)
        public bool SaveFile([FromBody] JObject data)
        {
            try
            {
                string filename = data["filename"].ToString();
                string content = data["filecontent"].ToString();
                string subDomain = data["subDomain"].ToString();

                string targetFolder = HttpContext.Current.Server.MapPath("~/Template/MauIn");
                if (!subDomain.Contains("localhost"))
                {
                    targetFolder = HttpContext.Current.Server.MapPath("~/Template/" + subDomain + "/MauIn");
                }

                string targetPath = Path.Combine(targetFolder, filename);
                FileInfo fileInfo = new FileInfo(targetPath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                File.WriteAllText(targetPath, content);
                //StreamWriter file = new StreamWriter(targetPath);
                //file.Write(content);
                //file.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //readfile (phuongnd)
        public string readFile(string filename)
        {
            var subDomain = CookieStore.GetCookieAes("SubDomain");
            string defaultFolder = HttpContext.Current.Server.MapPath("~/Template/MauIn");
            string folderCus = HttpContext.Current.Server.MapPath("~/Template/MauIn/" + subDomain + "/");
            // combine 2 string --> full path
            string targetPath = Path.Combine(folderCus, filename);
            string result = "";
            if (File.Exists(targetPath))
            {
                result = File.ReadAllText(targetPath);
            }
            else
            {
                targetPath = Path.Combine(defaultFolder, filename);
                result = File.ReadAllText(targetPath);
            }
            return result;
        }

        public List<ChotSoDTO> GetDataChotSo(Guid idChiNhanh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var data = from x in db.ChotSo
                               where x.ID_DonVi == idChiNhanh
                               select new ChotSoDTO
                               {
                                   ID = x.ID,
                                   ID_DonVi = x.ID_DonVi,
                                   NgayChotSo = x.NgayChotSo
                               };

                    return data.ToList();
                }
                else
                {
                    return null;
                }
            }
        }
        public List<ChotSoDTO> GetDataChotSoAll()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db != null)
                {
                    var data = from x in db.ChotSo
                               select new ChotSoDTO
                               {
                                   ID = x.ID,
                                   ID_DonVi = x.ID_DonVi,
                                   NgayChotSo = x.NgayChotSo
                               };

                    return data.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult UpdateCaiDatLichNhacBaoDuong([FromBody] JObject objIn)
        {
            try
            {
                HT_ThongBao_CatDatThoiGian htcaidat = new HT_ThongBao_CatDatThoiGian();
                htcaidat.LoaiThongBao = 4;
                if (objIn["NhacTruoc"] != null)
                    htcaidat.NhacTruocThoiGian = objIn["NhacTruoc"].ToObject<int>();
                if (objIn["NhacTruocLoaiThoiGian"] != null)
                    htcaidat.NhacTruocLoaiThoiGian = objIn["NhacTruocLoaiThoiGian"].ToObject<int>();
                if (objIn["SoLanLapLai"] != null)
                    htcaidat.SoLanLapLai = objIn["SoLanLapLai"].ToObject<int>();
                if (objIn["LoaiThoiGianLapLai"] != null)
                    htcaidat.LoaiThoiGianLapLai = objIn["LoaiThoiGianLapLai"].ToObject<int>();
                string sId = "";
                if (objIn["Id"] != null)
                    sId = objIn["Id"].ToObject<string>();

                if (sId != "")
                    htcaidat.ID = new Guid(sId);

                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    HT_ThongBao_CatDatThoiGian httbcd = db.HT_ThongBao_CatDatThoiGian.Where(p => p.ID == htcaidat.ID).FirstOrDefault();
                    if (httbcd != null)
                    {
                        httbcd.NhacTruocThoiGian = htcaidat.NhacTruocThoiGian;
                        httbcd.NhacTruocLoaiThoiGian = htcaidat.NhacTruocLoaiThoiGian;
                        httbcd.SoLanLapLai = htcaidat.SoLanLapLai;
                        httbcd.LoaiThoiGianLapLai = htcaidat.LoaiThoiGianLapLai;
                        db.Entry(httbcd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        htcaidat.ID = Guid.NewGuid();
                        db.HT_ThongBao_CatDatThoiGian.Add(htcaidat);
                        db.SaveChanges();
                    }
                }
                return ActionTrueNotData("");
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetCaiDatLichNhacBaoDuong()
        {
            try
            {
                HT_ThongBao_CatDatThoiGian httbcd = new HT_ThongBao_CatDatThoiGian();
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    httbcd = db.HT_ThongBao_CatDatThoiGian.Where(p => p.LoaiThongBao == 4).FirstOrDefault();
                }
                return ActionTrueData(httbcd);
            }
            catch
            {
                return ActionFalseNotData("");
            }
        }
    }

    public class ChotSoDTO
    {
        public Guid ID { get; set; }
        public Guid? ID_DonVi { get; set; }
        public DateTime NgayChotSo { get; set; }
    }

    public class HT_CauHinh_TichDiemChiTietDTO
    {
        public Guid ID { get; set; }
        public Guid ID_CauHinh { get; set; }
        public double TyLeDoiDiem { get; set; }
        public int? SoLanMua { get; set; }
        public bool ThanhToanBangDiem { get; set; }
        public int DiemThanhToan { get; set; }
        public double TienThanhToan { get; set; }
        public bool TichDiemGiamGia { get; set; }
        public bool? TichDiemHoaDonGiamGia { get; set; }
        public bool TichDiemHoaDonDiemThuong { get; set; }
        public bool ToanBoKhachHang { get; set; }
        public bool KhoiTaoTichDiem { get; set; }
        public Guid ID_NhomDoiTuong { get; set; }
        public List<HT_CauHinh_TichDiemApDungDTO> HT_CauHinh_TichDiemApDung { get; set; }
    }

    public class HT_CauHinh_GioiHanTraHangDTO
    {
        public Guid ID { get; set; }
        public Guid ID_CauHinh { get; set; }
        public int SoNgayGioiHan { get; set; }
        public bool ChoPhepTraHang { get; set; }
    }

    public class HT_CauHinh_TichDiemApDungDTO
    {
        public Guid ID { get; set; }
        public Guid ID_TichDiem { get; set; }
        public string TenNhomDoiTuong { get; set; }
    }
}

public class CauHinh
{
    public Guid ID { get; set; }
    public Guid ID_DonVi { get; set; }
    public bool GiaVonTrungBinh { get; set; }
    public bool CoDonViTinh { get; set; }
    public bool DatHang { get; set; }
    public bool XuatAm { get; set; }
    public bool DatHangXuatAm { get; set; }
    public bool ThayDoiThoiGianBanHang { get; set; }
    public bool SoLuongTrenChungTu { get; set; }
}
