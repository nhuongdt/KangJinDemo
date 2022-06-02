using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using libDM_KhuVuc;
using Model;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_KhuVucAPIController : ApiController
    {

        // GET: api/DM_KhuVucAPI/5
        public List<DM_KhuVucSelect> GetDM_KhuVuc()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                var lstDatas = classKhuVuc.Gets(null);
                List<DM_KhuVucSelect> list = new List<DM_KhuVucSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_KhuVucSelect temp = new DM_KhuVucSelect();
                        temp.ID = item.ID;
                        temp.TenKhuVuc = item.TenKhuVuc;
                        list.Add(temp);
                    }
                    list.OrderBy(x => x.TenKhuVuc);
                }
                return list;
            }
        }

        // faster func GetDM_KhuVuc
        public IHttpActionResult GetAllDM_KhuVuc()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                    var data = classKhuVuc.Gets(null).Select(x => new { x.ID, x.TenKhuVuc });
                    return Json(new { res = true, data = data });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = false, mes = e.InnerException + e.Message });
            }
        }

        // GET: api/DM_KhuVucAPI/5
        [ResponseType(typeof(DM_KhuVuc))]
        public IHttpActionResult GetDM_KhuVuc(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                DM_KhuVuc dM_KhuVuc = classKhuVuc.Select_KhuVuc(id);
                DM_KhuVuc temp = new DM_KhuVuc();
                temp.ID = dM_KhuVuc.ID;
                temp.TenKhuVuc = dM_KhuVuc.TenKhuVuc;
                temp.GhiChu = dM_KhuVuc.GhiChu;

                if (dM_KhuVuc == null)
                {
                    return NotFound();
                }
                return Ok(temp);
            }
        }

        public bool Check_TenKhuVucExist(string tenKhuVuc)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                return classKhuVuc.Check_TenKhuVucExist(tenKhuVuc);
            }
        }

        public bool Check_TenKhuVucEditExist(string tenKhuVuc, Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                return classKhuVuc.Check_TenKhuVucEditExist(tenKhuVuc, id);
            }
        }

        // PUT: api/DM_KhuVucAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_KhuVuc(Guid id, DM_KhuVuc dM_KhuVuc)
        {
            if (!ModelState.IsValid)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            if (id != dM_KhuVuc.ID)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "ID nhóm hàng hóa cần cập nhật không trùng khớp với ID dữ liệu"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                string strUpd = classKhuVuc.Update_KhuVuc(dM_KhuVuc);
                if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/DM_KhuVucAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_KhuVuc([FromBody]JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            DM_KhuVuc dM_KhuVuc = data["dM_NhomKhuVuc"].ToObject<DM_KhuVuc>();
            if (id != dM_KhuVuc.ID)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "ID nhóm hàng hóa cần cập nhật không trùng khớp với ID dữ liệu"));
            }
            dM_KhuVuc.NguoiSua = "admin";
            dM_KhuVuc.NgaySua = DateTime.Now;
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                string strUpd = classKhuVuc.Update_KhuVuc(dM_KhuVuc);
                if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // POST: api/DM_KhuVucAPI/5
        [ResponseType(typeof(DM_KhuVuc))]
        public IHttpActionResult PostDM_KhuVuc(DM_KhuVuc dM_KhuVuc)
        {
            dM_KhuVuc.MaKhuVuc = DateTime.Now.ToString("yyyyMMddddHHmmss");
            dM_KhuVuc.NguoiTao = "admin";
            dM_KhuVuc.NgayTao = DateTime.Now;
            dM_KhuVuc.ID = Guid.NewGuid();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                DM_KhuVucDTO objReturn = new DM_KhuVucDTO
                {
                    MaKhuVuc = dM_KhuVuc.MaKhuVuc,
                    NguoiTao = dM_KhuVuc.NguoiTao,
                    NgayTao = dM_KhuVuc.NgayTao,
                    ID = dM_KhuVuc.ID
                };
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                string strIns = classKhuVuc.Add_KhuVuc(dM_KhuVuc);
                if (strIns != null && strIns != string.Empty && strIns.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
            }
        }



        // DELETE: api/DM_KhuVucAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult DeleteDM_KhuVuc(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_KhuVuc classKhuVuc = new classDM_KhuVuc(db);
                string strDel = classKhuVuc.Delete_KhuVuc(id);
                if (strDel != null && strDel != string.Empty && strDel.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
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

    public class DM_KhuVucSelect
    {
        public Guid ID { get; set; }
        public string TenKhuVuc { get; set; }
    }
}
