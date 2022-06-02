using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using libDM_LoaiChungTu;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_LoaiChungTuAPIController : ApiController
    {
        // GET: api/DM_LoaiChungTuAPI/5
        public List<DM_loaiChungTuSelect> GetDM_LoaiChungTu()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_LoaiChungTu classLoaiChungTu = new ClassDM_LoaiChungTu(db);
                IQueryable<DM_LoaiChungTu> lstDatas = classLoaiChungTu.Gets(null);
                List<DM_loaiChungTuSelect> list = new List<DM_loaiChungTuSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_loaiChungTuSelect temp = new DM_loaiChungTuSelect();
                        temp.ID = item.ID;
                        temp.TenLoaiChungTu = item.TenLoaiChungTu;
                        list.Add(temp);
                    }
                }
                return list;
            }
        }

        public IHttpActionResult GetDM_LoaiChungTu(int id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_LoaiChungTu classLoaiChungTu = new ClassDM_LoaiChungTu(db);
                DM_LoaiChungTu dM_LoaiChungTu = classLoaiChungTu.Select_LoaiChungTu(id);
                DM_LoaiChungTu temp = new DM_LoaiChungTu();
                temp.ID = dM_LoaiChungTu.ID;
                temp.TenLoaiChungTu = dM_LoaiChungTu.TenLoaiChungTu;

                if (dM_LoaiChungTu == null)
                {
                    return NotFound();
                }
                return Ok(temp);
            }
        }

        //public bool Check_TenLoaiChungTuExist(string tenLoaiChungTu)
        //{
        //    return classLoaiChungTu.Check_TenLoaiChungTuExist(tenLoaiChungTu);
        //}

        // PUT: api/DM_LoaiChungTuAPI/5
        //[ResponseType(typeof(string))]
        //public IHttpActionResult PutDM_LoaiChungTu(int id, DM_LoaiChungTu dM_LoaiChungTu)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
        //    }
        //    if (id != dM_LoaiChungTu.ID)
        //    {
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "ID nhóm hàng hóa cần cập nhật không trùng khớp với ID dữ liệu"));
        //    }
        //    string strUpd = classLoaiChungTu.Update_LoaiChungTu(dM_LoaiChungTu);
        //    if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
        //    else
        //        return StatusCode(HttpStatusCode.NoContent);
        //}

        // PUT: api/DM_LoaiChungTuAPI/5
        //[ResponseType(typeof(string))]
        //public IHttpActionResult PutDM_LoaiChungTu([FromBody]JObject data)
        //{
        //    var id = data["id"].ToObject<int>();
        //    DM_LoaiChungTu dM_LoaiChungTu = data["dM_LoaiChungTu"].ToObject<DM_LoaiChungTu>();
        //    if (id != dM_LoaiChungTu.ID)
        //    {
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "ID nhóm hàng hóa cần cập nhật không trùng khớp với ID dữ liệu"));
        //    }

        //    dM_LoaiChungTu.NguoiSua = "admin";
        //    dM_LoaiChungTu.NgaySua = DateTime.Now;

        //    string strUpd = classLoaiChungTu.Update_LoaiChungTu(dM_LoaiChungTu);
        //    if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
        //    else
        //        return StatusCode(HttpStatusCode.NoContent);
        //}

            // POST: api/DM_LoaiChungTuAPI/5
        //[ResponseType(typeof(DM_LoaiChungTu))]
        //public IHttpActionResult PostDM_LoaiChungTu(DM_LoaiChungTu dM_LoaiChungTu)
        //{
        //    dM_LoaiChungTu.MaLoaiChungTu = DateTime.Now.ToString("yyyyMMddddHHmmss");
        //    dM_LoaiChungTu.NguoiTao = "admin";
        //    dM_LoaiChungTu.NgayTao = DateTime.Now;
        //    dM_LoaiChungTu.ID = ;

        //    string strIns = classLoaiChungTu.Add_LoaiChungTu(dM_LoaiChungTu);
        //    if (strIns != null && strIns != string.Empty && strIns.Trim() != "")
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
        //    else
        //        return CreatedAtRoute("DefaultApi", new { id = dM_LoaiChungTu.ID }, dM_LoaiChungTu);
        //}

        //public IHttpActionResult DeleteDM_LoaiChungTu(int id)
        //{
        //    string strDel = classLoaiChungTu.Delete_LoaiChungTu(id);
        //    if (strDel != null && strDel != string.Empty && strDel.Trim() != "")
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
        //    else
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //    }
        //    base.Dispose(disposing);
        //}
    }
    public class DM_loaiChungTuSelect
    {
        public int ID { get; set; }
        public string TenLoaiChungTu { get; set; }
    }
}
