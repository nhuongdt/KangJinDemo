using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Net.Http;
using Model;
using libDonViQuiDoi;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DonViTinhsAPIController : ApiController
    {
        // GET: api/DonViTinhsAPI
        public List<DonViQuiDoi> GetDonViQuiDois()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                return _classDVQD.Gets(null);
            }
        }

        // GET: api/DonViTinhsAPI/5
        [ResponseType(typeof(DonViQuiDoi))]
        public IHttpActionResult GetDonViQuiDoi(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                DonViQuiDoi donViQuiDoi = _classDVQD.Select_DonViQuiDoi(id);
                if (donViQuiDoi == null)
                {
                    return NotFound();
                }
                return Ok(donViQuiDoi);
            }
        }

        // PUT: api/DonViTinhsAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDonViQuiDoi(string id, DonViQuiDoi donViQuiDoi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                if (!ModelState.IsValid)
                {
                    // return BadRequest(ModelState);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
                }

                if (id != donViQuiDoi.MaHangHoa)
                {
                    // return BadRequest();
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                string strUpd = _classDVQD.Update_DonViQuiDoi(donViQuiDoi);
                if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // POST: api/DonViTinhsAPI
        [ResponseType(typeof(DonViQuiDoi))]
        public IHttpActionResult PostDonViQuiDoi(DonViQuiDoi donViQuiDoi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
                }

                string strIns = _classDVQD.Add_DonViQuiDoi(donViQuiDoi);
                if (strIns != null && strIns != string.Empty && strIns.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = donViQuiDoi.MaHangHoa }, donViQuiDoi);
            }
        }

        // DELETE: api/DonViTinhsAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult DeleteDonViQuiDoi(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDonViQuiDoi _classDVQD = new classDonViQuiDoi(db);
                string strDel = _classDVQD.Delete_DonViQuiDoi(id);
                if (strDel != null && strDel != string.Empty && strDel.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
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
