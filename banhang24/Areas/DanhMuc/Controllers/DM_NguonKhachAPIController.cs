using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Model;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using libDM_DoiTuong;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_NguonKhachAPIController : ApiController
    {
        #region " GET "
        [HttpGet]
        public List<DM_NguonKhachHang> GetDM_NguonKhach()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_NguonKhach classNguonKhach = new ClassDM_NguonKhach(db);
                var iqDM_NguonKhach = classNguonKhach.Gets(null);
                List<DM_NguonKhachHang> lst = new List<DM_NguonKhachHang>();
                if (iqDM_NguonKhach != null)
                {
                    foreach (DM_NguonKhachHang item in iqDM_NguonKhach)
                    {
                        DM_NguonKhachHang obj = new DM_NguonKhachHang
                        {
                            ID = item.ID,
                            TenNguonKhach = item.TenNguonKhach
                        };
                        lst.Add(obj);
                    }
                }
                return lst;
            }
        }
        #endregion

        #region " POST "
        [HttpPost, ActionName("PostDM_NguonKhachHang")]
        [ResponseType(typeof(DM_NguonKhachHang))]
        public IHttpActionResult PostDM_NguonKhachHang([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_NguonKhach classNguonKhach = new ClassDM_NguonKhach(db);

                DM_NguonKhachHang objNew = data.ToObject<DM_NguonKhachHang>();
                DM_NguonKhachHang DM_NguonKhachHang = new DM_NguonKhachHang { };
                DM_NguonKhachHang.ID = Guid.NewGuid();
                DM_NguonKhachHang.TenNguonKhach = objNew.TenNguonKhach;
                DM_NguonKhachHang.NguoiTao = objNew.NguoiTao;
                DM_NguonKhachHang.NgayTao = DateTime.Now;

                string strIns = classNguonKhach.Add(DM_NguonKhachHang);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = DM_NguonKhachHang.ID }, DM_NguonKhachHang);
                }
            }
        }

        public bool CheckExist(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_NguonKhach classNguonKhach = new ClassDM_NguonKhach(db);
                DM_NguonKhachHang obj = classNguonKhach.Select_ByID(id);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        #region " PUT "
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_NguonKhachHang([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_NguonKhach classNguonKhach = new ClassDM_NguonKhach(db);
                DM_NguonKhachHang objNew = data.ToObject<DM_NguonKhachHang>();

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string strUpd = classNguonKhach.Update(objNew);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion

        #region DELETE
        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult DeleteDM_NguonKhach(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                ClassDM_NguonKhach classNguonKhach = new ClassDM_NguonKhach(db);
                string strDel = classNguonKhach.Delete(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }
        #endregion
    }
}