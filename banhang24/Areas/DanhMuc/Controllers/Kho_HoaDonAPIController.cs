using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using libKho_HoaDon;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class Kho_HoaDonAPIController : ApiController
    {
        #region GET
        // GET: api/DanhMuc/DM_ViTriAPI/GetDM_ViTri
        public IQueryable<Kho_HoaDon> GetKho_HoaDon()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classKho_HoaDon classKhoHoaDon = new classKho_HoaDon(db);
                return classKhoHoaDon.Gets(null);
            }
        }
        // GET: api/DanhMuc/DM_ViTriAPI/GetDM_ViTri
        public List<Kho_HoaDonDTO> GetListKhoHoaDon()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classKho_HoaDon classKhoHoaDon = new classKho_HoaDon(db);
                IQueryable<Kho_HoaDon> lstAllKhos = classKhoHoaDon.Gets(null);
                if (lstAllKhos != null && lstAllKhos.Count() > 0)
                {
                    List<Kho_HoaDonDTO> lsrReturns = new List<Kho_HoaDonDTO>();
                    foreach (Kho_HoaDon item in lstAllKhos)
                    {
                        Kho_HoaDonDTO itemData = new Kho_HoaDonDTO
                        {
                            ID = item.ID,
                            MaHoaDon = item.MaHoaDon,
                            NgayLapHoaDon = item.NgayLapHoaDon,
                            DienGiai = item.DienGiai,
                            LoaiChungTu = item.LoaiChungTu,
                        };
                        lsrReturns.Add(itemData);
                    }

                    return lsrReturns;
                }
                else
                    return null;
            }
        }

        // GET: api/DM_ViTriAPI/5
        [ResponseType(typeof(Kho_HoaDonDTO))]
        public IHttpActionResult GetKho_HoaDon(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classKho_HoaDon classKhoHoaDon = new classKho_HoaDon(db);
                Kho_HoaDon Kho_HoaDon = classKhoHoaDon.SelectKho_HoaDon(id);
                Kho_HoaDonDTO ct = new Kho_HoaDonDTO();
                ct.ID = Kho_HoaDon.ID;
                ct.MaHoaDon = Kho_HoaDon.MaHoaDon;
                ct.LoaiChungTu = Kho_HoaDon.LoaiChungTu;
                ct.DienGiai = Kho_HoaDon.DienGiai;

                if (Kho_HoaDon == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }
        #endregion

        #region insert
        // POST: api/DM_ViTriAPI
        [HttpPost, ActionName("PostKho_HoaDon1")]
        [ResponseType(typeof(DM_ViTri))]
        public IHttpActionResult PostKho_HoaDon1(Kho_HoaDon kho_HoaDon)
        {
            if (!ModelState.IsValid)
            {
                // return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            kho_HoaDon.ID = Guid.NewGuid();
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classKho_HoaDon classKhoHoaDon = new classKho_HoaDon(db);
                string strIns = classKhoHoaDon.Add_KhoHoaDon(kho_HoaDon);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = kho_HoaDon.ID }, kho_HoaDon);
            }
        }

        [HttpPost, ActionName("PostKho_HoaDon")]
        [ResponseType(typeof(Kho_HoaDon))]
        public IHttpActionResult PostKho_HoaDon([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classKho_HoaDon classKhoHoaDon = new classKho_HoaDon(db);
                //DM_ViTri objNewVT = data.ToObject<Kho_HoaDon>();
                Kho_HoaDon objNewKho = data["objNewKho"].ToObject<Kho_HoaDon>();
                //string sTenViTri = objNewVT.TenViTri;            
                //string sGhiChu = objNewVT.GhiChu;
                string MaHoaDon = objNewKho.MaHoaDon;
                if (MaHoaDon == null)
                {
                    MaHoaDon = "";
                }

                #region DM_ViTri
                Kho_HoaDon Kho_HoaDon = new Kho_HoaDon();
                Kho_HoaDon.ID = Guid.NewGuid();
                Kho_HoaDon.MaHoaDon = objNewKho.MaHoaDon;
                Kho_HoaDon.NgayLapHoaDon = objNewKho.NgayLapHoaDon;
                Kho_HoaDon.LoaiChungTu = objNewKho.LoaiChungTu;
                Kho_HoaDon.DienGiai = objNewKho.DienGiai;


                #endregion

                string strIns = classKhoHoaDon.Add_KhoHoaDon(Kho_HoaDon);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    Kho_HoaDonDTO objReturn = new Kho_HoaDonDTO();
                    objReturn.ID = Kho_HoaDon.ID;
                    objReturn.MaHoaDon = Kho_HoaDon.MaHoaDon;
                    objReturn.NgayLapHoaDon = Kho_HoaDon.NgayLapHoaDon;
                    objReturn.LoaiChungTu = Kho_HoaDon.LoaiChungTu;
                    objReturn.DienGiai = Kho_HoaDon.DienGiai;

                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }
        #endregion
    }
}
