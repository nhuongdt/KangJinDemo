using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Data.Entity.Infrastructure;
using System.Web.Http.Description;
using System.Data.Entity;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_ChucVuAPIController : ApiController
    {
        //
        // GET: /DanhMuc/DM_ChucVuAPI/

        private SsoftvnContext db = SystemDBContext.GetDBContext();

        // GET: api/DM_ChucVu
        public IQueryable<DM_ChucVu> GetDM_ChucVu()
        {
            return db.DM_ChucVu;
        }

        // GET: api/DM_ChucVu/5
        [ResponseType(typeof(DM_ChucVu))]
        public IHttpActionResult GetDM_ChucVu(Guid id)
        {
            DM_ChucVu dM_ChucVu = db.DM_ChucVu.Find(id);
            if (dM_ChucVu == null)
            {
                return NotFound();
            }

            return Ok(dM_ChucVu);
        }

        // PUT: api/DM_ChucVu/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDM_ChucVu(Guid id, DM_ChucVu dM_ChucVu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dM_ChucVu.ID)
            {
                return BadRequest();
            }

            db.Entry(dM_ChucVu).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DM_ChucVuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DM_ChucVu
        [ResponseType(typeof(DM_ChucVu))]
        public IHttpActionResult PostDM_ChucVu(DM_ChucVu dM_ChucVu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DM_ChucVu.Add(dM_ChucVu);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DM_ChucVuExists(dM_ChucVu.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = dM_ChucVu.ID }, dM_ChucVu);
        }

        // DELETE: api/DM_ChucVu/5
        [ResponseType(typeof(DM_ChucVu))]
        public IHttpActionResult DeleteDM_ChucVu(Guid id)
        {
            DM_ChucVu dM_ChucVu = db.DM_ChucVu.Find(id);
            if (dM_ChucVu == null)
            {
                return NotFound();
            }

            db.DM_ChucVu.Remove(dM_ChucVu);
            db.SaveChanges();

            return Ok(dM_ChucVu);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DM_ChucVuExists(Guid id)
        {
            return db.DM_ChucVu.Count(e => e.ID == id) > 0;
        }

    }
}
