using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class SaveDiaryController : ApiController
    {
        [ActionName("post_NhatKySuDung")]
        [HttpPost, HttpGet]
        public IHttpActionResult post_NhatKySuDung([FromBody] JObject data)
        {
            HT_NhatKySuDung objNhatKySuDung = data["objDiary"].ToObject<HT_NhatKySuDung>();
            HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung { };
            hT_NhatKySuDung.ID = Guid.NewGuid();
            hT_NhatKySuDung.ID_NhanVien = objNhatKySuDung.ID_NhanVien;
            hT_NhatKySuDung.ChucNang = objNhatKySuDung.ChucNang;
            hT_NhatKySuDung.ThoiGian = DateTime.Now;
            hT_NhatKySuDung.NoiDung = objNhatKySuDung.NoiDung;
            hT_NhatKySuDung.NoiDungChiTiet = objNhatKySuDung.NoiDungChiTiet;
            hT_NhatKySuDung.LoaiNhatKy = objNhatKySuDung.LoaiNhatKy;
            hT_NhatKySuDung.ID_DonVi = objNhatKySuDung.ID_DonVi;
            string strIns = SaveDiary.add_Diary(hT_NhatKySuDung);
            if (strIns != null && strIns != string.Empty)
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
            else
                return CreatedAtRoute("DefaultApi", new { id = hT_NhatKySuDung.ID }, hT_NhatKySuDung);

        }

        [HttpPost]
        public IHttpActionResult Post_NhatKySuDung_UpdateGiaVon([FromBody] JObject data)
        {
            try
            {
                HT_NhatKySuDung objNhatKySuDung = data["objDiary"].ToObject<HT_NhatKySuDung>();
                HT_NhatKySuDung hT_NhatKySuDung = new HT_NhatKySuDung { };
                hT_NhatKySuDung.ID = Guid.NewGuid();
                hT_NhatKySuDung.ID_HoaDon = objNhatKySuDung.ID_HoaDon;
                hT_NhatKySuDung.LoaiHoaDon = objNhatKySuDung.LoaiHoaDon;
                hT_NhatKySuDung.ThoiGianUpdateGV = objNhatKySuDung.ThoiGianUpdateGV;
                hT_NhatKySuDung.ID_NhanVien = objNhatKySuDung.ID_NhanVien;
                hT_NhatKySuDung.ChucNang = objNhatKySuDung.ChucNang;
                hT_NhatKySuDung.ThoiGian = DateTime.Now;
                hT_NhatKySuDung.NoiDung = objNhatKySuDung.NoiDung;
                hT_NhatKySuDung.NoiDungChiTiet = objNhatKySuDung.NoiDungChiTiet;
                hT_NhatKySuDung.LoaiNhatKy = objNhatKySuDung.LoaiNhatKy;
                hT_NhatKySuDung.ID_DonVi = objNhatKySuDung.ID_DonVi;
                var stErr = SaveDiary.add_Diary(hT_NhatKySuDung);
                if (stErr != string.Empty)
                {
                    return Json(new { res = false, mes = stErr });
                }
                else
                {
                    return Json(new { res = true, data = hT_NhatKySuDung });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }
    }
}
