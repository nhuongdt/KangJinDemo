using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Model;
using libDM_HangHoa;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_PhanLoaiHangHoaDichVuAPIController : ApiController
    {
        // GET: api/danhmuc/DM_PhanLoaiHangHoaDichVuAPI
        public IQueryable<DM_PhanLoaiHangHoaDichVu> GetDM_PhanLoaiHangHoaDichVu()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_PhanLoaiHangHoaDichVu classPhanLoaiHHDV = new classDM_PhanLoaiHangHoaDichVu(db);
                return classPhanLoaiHHDV.Gets(null);
            }
        }

        // GET: api/danhmuc/DM_PhanLoaiHangHoaDichVuAPI/5
        [ResponseType(typeof(DM_PhanLoaiHangHoaDichVu))]
        public IHttpActionResult GetDM_PhanLoaiHangHoaDichVu(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_PhanLoaiHangHoaDichVu classPhanLoaiHHDV = new classDM_PhanLoaiHangHoaDichVu(db);
                DM_PhanLoaiHangHoaDichVu dM_PhanLoaiHangHoaDichVu = classPhanLoaiHHDV.Select_PhanLoaiHangHoaDichVu(id);
                if (dM_PhanLoaiHangHoaDichVu == null)
                {
                    return NotFound();
                }
                return Ok(dM_PhanLoaiHangHoaDichVu);
            }
        }

        // PUT: api/danhmuc/DM_PhanLoaiHangHoaDichVuAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_PhanLoaiHangHoaDichVu(Guid id, DM_PhanLoaiHangHoaDichVu dM_PhanLoaiHangHoaDichVu)
        {
            if (!ModelState.IsValid)
            {
                //  return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            if (id != dM_PhanLoaiHangHoaDichVu.ID)
            {
                //   return BadRequest();
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_PhanLoaiHangHoaDichVu classPhanLoaiHHDV = new classDM_PhanLoaiHangHoaDichVu(db);
                string strUpd = classPhanLoaiHHDV.Update_PhanLoaiHangHoaDichVu(dM_PhanLoaiHangHoaDichVu);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // POST: api/danhmuc/DM_PhanLoaiHangHoaDichVuAPI
        [ResponseType(typeof(DM_PhanLoaiHangHoaDichVu))]
        public IHttpActionResult PostDM_PhanLoaiHangHoaDichVu(DM_PhanLoaiHangHoaDichVu dM_PhanLoaiHangHoaDichVu)
        {
            if (!ModelState.IsValid)
            {
                // return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_PhanLoaiHangHoaDichVu classPhanLoaiHHDV = new classDM_PhanLoaiHangHoaDichVu(db);
                string strIns = classPhanLoaiHHDV.Add_PhanLoaiHangHoaDichVu(dM_PhanLoaiHangHoaDichVu);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = dM_PhanLoaiHangHoaDichVu.ID }, dM_PhanLoaiHangHoaDichVu);
            }
        }

        // DELETE: api/danhmuc/DM_PhanLoaiHangHoaDichVuAPI/5
        [ResponseType(typeof(DM_PhanLoaiHangHoaDichVu))]
        public IHttpActionResult DeleteDM_PhanLoaiHangHoaDichVu(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_PhanLoaiHangHoaDichVu classPhanLoaiHHDV = new classDM_PhanLoaiHangHoaDichVu(db);
                string strDel = classPhanLoaiHHDV.Delete_PhanLoaiHangHoaDichVu(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            base.Dispose(disposing);
        }

    }
}
