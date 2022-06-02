using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using libDM_ViTri;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using libDM_DoiTuong;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_ViTriAPIController : ApiController
    {
        #region GET
        // GET: api/DanhMuc/DM_ViTriAPI/GetDM_ViTri
        public List<DM_ViTri> GetDM_ViTri()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                return _classDMVT.Gets(null);
            }
        }

        // GET: api/DanhMuc/DM_ViTriAPI/GetDM_ViTri
        public DM_ViTriDTO GetFistDM_ViTri()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                return _classDMVT.GetFistDM_ViTri(null);
            }
        }

        public List<DM_ViTriDTO> GetListViTris(string condition = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                List<DM_ViTriDTO> lstAllVTs = _classDMVT.GetViTri_KhuVuc();
                if (condition != null)
                {
                    lstAllVTs = lstAllVTs.Where(x => x.TenViTri.Contains(condition) || x.MaViTri.Contains(condition)).ToList();
                }
                if (lstAllVTs != null && lstAllVTs.Count() > 0)
                {
                    return lstAllVTs.OrderBy(x => x.TenKhuVuc).OrderBy(x => x.TenViTri).ToList();
                }
                else
                    return null;
            }
        }

        // faster
        public IHttpActionResult GetListViTris_NotWhere()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                try
                {
                    var data = _classDMVT.Gets(null).Where(x => x.TinhTrang != true).Select(x => new
                    {
                        ID = x.ID,
                        ID_KhuVuc = x.ID_KhuVuc,
                        TenViTri = x.TenViTri,
                        TenKhuVuc = x.DM_KhuVuc.TenKhuVuc,
                        WorkingTime = 1,
                        SoLuongKhachHang = 0,
                        DateStart = "0"
                    }).ToList();
                    return Json(new { res = true, data = data });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        public List<DM_ViTriDTO> GetListViTris_where(int currentPage, int pageSize, string maHoaDon, string idkhuvuc)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                List<DM_ViTriDTO> lstAllVTs = _classDMVT.getlistViTri_where(maHoaDon, idkhuvuc);
                lstAllVTs = lstAllVTs.Skip(currentPage * pageSize).Take(pageSize).ToList();
                return lstAllVTs;
            }
        }

        public PageListDTO GetPageCountViTris_Where(int currentPage, float pageSize, string maHoaDon, string idkhuvuc)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                var totalRecords = 0;
                var data = _classDMVT.getlistViTri_where(maHoaDon, idkhuvuc);
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
        }

        public bool Check_TenVitriExist(string tenViTri, Guid? id_khuvuc, Guid? id = null)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                return _classDMVT.Check_TenVitriExist(tenViTri, id_khuvuc, id);
            }
        }

        public bool Check_TenVitriExistEDit(string tenViTri, Guid? id_khuvuc, Guid? id_vitri)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                return _classDMVT.Check_TenVitriExistEdit(tenViTri, id_khuvuc, id_vitri);
            }
        }

        public List<DM_ViTriDTO> GetViTri_ByIDKhuVuc(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                List<DM_ViTriDTO> lstAllVTs = _classDMVT.GetViTri_KhuVuc().Where(x => x.ID_KhuVuc.Equals(id)).ToList();
                if (lstAllVTs != null && lstAllVTs.Count() > 0)
                {
                    return lstAllVTs;
                }
                else
                    return null;
            }
        }

        // GET: api/DM_ViTriAPI/5
        [ResponseType(typeof(DM_ViTriDTO))]
        public IHttpActionResult GetDM_ViTri(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                var ct = _classDMVT.GetViTri_KhuVuc();
                if (ct == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }

        #endregion
        #region update
        // PUT: api/DM_ViTriAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_ViTri(Guid id, DM_ViTri DM_ViTri)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                string strUpd = _classDMVT.Update_ViTri(DM_ViTri);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/DM_ViTriAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_ViTri([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                libDM_KhuVuc.classDM_KhuVuc classKhuVuc = new libDM_KhuVuc.classDM_KhuVuc(db);

                Guid id = data["id"].ToObject<Guid>();
                DM_ViTri DM_ViTri = data["objNewViTri"].ToObject<DM_ViTri>();
                //DM_ViTri.ID = id;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = _classDMVT.Update_ViTri(DM_ViTri);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                {
                    DM_ViTriDTO objReturn = new DM_ViTriDTO();
                    objReturn.ID = DM_ViTri.ID;
                    objReturn.MaViTri = DM_ViTri.MaViTri;
                    objReturn.TenViTri = DM_ViTri.TenViTri;
                    objReturn.GhiChu = DM_ViTri.GhiChu;
                    objReturn.TenKhuVuc = classKhuVuc.Select_KhuVuc(DM_ViTri.ID_KhuVuc).TenKhuVuc;

                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }
        #endregion

        #region insert
        // POST: api/DM_ViTriAPI
        [HttpPost, ActionName("PostDM_ViTri1")]
        [ResponseType(typeof(DM_ViTri))]
        public IHttpActionResult PostDM_ViTri1(DM_ViTri dM_ViTri)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                if (!ModelState.IsValid)
                {
                    // return BadRequest(ModelState);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                dM_ViTri.ID = Guid.NewGuid();

                string strIns = _classDMVT.Add_ViTri(dM_ViTri);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = dM_ViTri.ID }, dM_ViTri);
            }
        }

        [HttpPost, ActionName("PostDM_ViTri")]
        [ResponseType(typeof(DM_ViTri))]
        public IHttpActionResult PostDM_ViTri([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                libDM_KhuVuc.classDM_KhuVuc classKhuVuc = new libDM_KhuVuc.classDM_KhuVuc(db);

                DM_ViTri objNewVT = data["objNewViTri"].ToObject<DM_ViTri>();
                string MaViTri = objNewVT.MaViTri;
                if (MaViTri == null)
                {
                    MaViTri = _classDMVT.GetAutoCode();
                }
                string TenViTri = objNewVT.TenViTri;
                Guid idNhom = objNewVT.ID_KhuVuc;
                string GhiChu = objNewVT.GhiChu;

                #region DM_ViTri
                DM_ViTri dM_ViTri = new DM_ViTri();
                dM_ViTri.ID = Guid.NewGuid();
                dM_ViTri.TenViTri = objNewVT.TenViTri;
                dM_ViTri.GhiChu = objNewVT.GhiChu;
                dM_ViTri.ID_KhuVuc = idNhom;
                dM_ViTri.MaViTri = MaViTri;
                #endregion

                string strIns = _classDMVT.Add_ViTri(dM_ViTri);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    DM_ViTriDTO objReturn = new DM_ViTriDTO();
                    objReturn.ID = dM_ViTri.ID;
                    objReturn.MaViTri = MaViTri;
                    objReturn.TenViTri = TenViTri;
                    objReturn.GhiChu = GhiChu;
                    objReturn.TenKhuVuc = classKhuVuc.Select_KhuVuc(idNhom).TenKhuVuc;
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        #endregion
        #region delete
        // DELETE: api/DM_ViTriAPI/5
        [HttpDelete]
        [ResponseType(typeof(string))]
        public string DeleteDM_ViTri(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_ViTri _classDMVT = new classDM_ViTri(db);
                DM_ViTri vitri = _classDMVT.Get(p => p.ID == id);
                if (vitri != null)
                {
                    vitri.TinhTrang = true; // xóa ==true
                    _classDMVT.Update_ViTri(vitri);
                    return "";
                }
                else
                {
                    return "Lỗi";
                }
            }
        }
        #endregion
        #region ###

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
