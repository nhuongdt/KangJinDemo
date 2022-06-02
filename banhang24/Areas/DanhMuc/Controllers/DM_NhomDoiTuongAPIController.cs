using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net;
using System.Net.Http;
using Model;
using libDM_NhomDoiTuong;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_NhomDoiTuongAPIController : BaseApiController
    {
        // GET: '/api/danhmuc/DM_NhomDoiTuongAPI/';
        public List<DM_NhomDoiTuong> GetDM_NhomDoiTuong()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                return classNhomDoiTuong.Gets(null);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetAllDMNhomDoiTuong()
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    List<DM_NhomDoiTuong> list = new List<DM_NhomDoiTuong>();
                    ClassDM_NhomDoiTuong classDM_NhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                    list = classDM_NhomDoiTuong.Gets(p => p.TrangThai == null || p.TrangThai.Value == true);
                    return ActionTrueData(new
                    {
                        data = list.Select(p => new
                        {
                            ID = p.ID,
                            LoaiDoiTuong = p.LoaiDoiTuong,
                            MaNhomDoiTuong = p.MaNhomDoiTuong,
                            TenNhomDoiTuong = p.TenNhomDoiTuong
                        }).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                return ActionFalseNotData(ex.Message);
            }
        }

        [HttpGet]
        public List<Model.DM_NhomDoiTuong> GetDM_NhomDoiTuong(int loaiDoiTuong)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                List<DM_NhomDoiTuong> lst = classNhomDoiTuong.Gets(null);
                if (lst != null && lst.Count() > 0)
                {
                    lst = classNhomDoiTuong.Gets(null)
                        .Where(type => (type.LoaiDoiTuong == loaiDoiTuong || type.LoaiDoiTuong == 0)
                        && (type.TrangThai == true || type.TrangThai == null)).ToList();

                    List<DM_NhomDoiTuong> lsrReturns = new List<DM_NhomDoiTuong>();
                    foreach (DM_NhomDoiTuong item in lst)
                    {
                        DM_NhomDoiTuong itemData = new DM_NhomDoiTuong
                        {
                            ID = item.ID,
                            TenNhomDoiTuong = item.TenNhomDoiTuong,
                            MaNhomDoiTuong = item.MaNhomDoiTuong,
                            GiamGia = item.GiamGia == null ? 0 : item.GiamGia,
                            GiamGiaTheoPhanTram = item.GiamGiaTheoPhanTram,
                            TuDongCapNhat = item.TuDongCapNhat,
                            GhiChu = item.GhiChu,
                        };
                        lsrReturns.Add(itemData);
                    }
                    return lsrReturns;
                }
                else
                    return null;
            }
        }

        public IHttpActionResult GetDM_NhomDoiTuong_ByLoaiDoiTuong(int loaiDoiTuong)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                    var data = classNhomDoiTuong.Gets(null)
                     .Where(type => (type.LoaiDoiTuong == loaiDoiTuong || type.LoaiDoiTuong == 0)
                     && (type.TrangThai == true || type.TrangThai == null))
                     .Select(x => new
                     {
                         ID = x.ID,
                         TenNhomDoiTuong = x.TenNhomDoiTuong,
                         MaNhomDoiTuong = x.MaNhomDoiTuong,
                         GiamGia = x.GiamGia == null ? 0 : x.GiamGia,
                         GiamGiaTheoPhanTram = x.GiamGiaTheoPhanTram,
                         TuDongCapNhat = x.TuDongCapNhat,
                         GhiChu = x.GhiChu,
                     });
                    return Json(new { res = true, data = data });
                }
            }
            catch (Exception e)
            {
                return Json(new { res = true, mes = string.Concat(e.InnerException, " ", e.Message) });
            }
        }

        // GET: '/api/danhmuc/DM_NhomDoiTuongAPI/5';
        [ResponseType(typeof(DM_NhomDoiTuong))]
        public IHttpActionResult GetDM_NhomDoiTuong(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                DM_NhomDoiTuong dM_NhomDoiTuong = classNhomDoiTuong.Select_NhomDoiTuong(id);
                DM_NhomDoiTuong dtreturn = new DM_NhomDoiTuong
                {
                    ID = dM_NhomDoiTuong.ID,
                    TenNhomDoiTuong = dM_NhomDoiTuong.TenNhomDoiTuong,
                    GhiChu = dM_NhomDoiTuong.GhiChu
                };
                if (dM_NhomDoiTuong == null)
                {
                    return NotFound();
                }
                return Ok(dtreturn);
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetDM_NhomDoiTuong_ChiTiets(Guid idDonVi)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                if (db == null)
                {
                    return null;
                }
                else
                {
                    var data = (from ct in db.DM_NhomDoiTuong_ChiTiet
                                join ndt in db.DM_NhomDoiTuong
                                on ct.ID_NhomDoiTuong equals ndt.ID 
                                where ndt.TuDongCapNhat == true
                                   && (from ndv in db.NhomDoiTuong_DonVi // same exists sql: only get ct of nhom thuoc donvi
                                       where ndv.ID_DonVi == idDonVi
                                       select ndv.ID_NhomDoiTuong).Contains(ndt.ID)
                                select new
                                {
                                    ct.ID,
                                    ct.ID_NhomDoiTuong,
                                    ct.LoaiDieuKien,
                                    ct.LoaiSoSanh,
                                    ct.GiaTriSo,
                                    ct.GiaTriBool,
                                    ct.GiaTriThoiGian,
                                    ct.GiaTriKhuVuc,
                                    ct.GiaTriVungMien,
                                    ndt.GiamGiaTheoPhanTram,
                                    ndt.TuDongCapNhat,
                                }).ToList();
                    return Json(new { data = data });
                }
            }
        }

        [ResponseType(typeof(string))]
        [HttpPost, HttpPut]
        public IHttpActionResult PutDM_NhomDoiTuong([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                Guid id = data["id"].ToObject<Guid>();
                DM_NhomDoiTuong dm_NhomDoiTuong = data["objNhomDoiTuong"].ToObject<DM_NhomDoiTuong>();

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string strUpd = classNhomDoiTuong.Update_NhomDoiTuong(dm_NhomDoiTuong);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPost, ActionName("PostDM_NhomDoiTuong")]
        [ResponseType(typeof(DM_NhomDoiTuong))]
        public IHttpActionResult PostDM_NhomDoiTuong([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                DM_NhomDoiTuong objNew = data.ToObject<DM_NhomDoiTuong>();

                string sMaNhom = string.Empty;
                if (objNew.MaNhomDoiTuong != null && objNew.MaNhomDoiTuong != string.Empty)
                {
                    sMaNhom = objNew.MaNhomDoiTuong;
                }
                else
                {
                    sMaNhom = classNhomDoiTuong.GetautoCodeNhomDT(objNew.LoaiDoiTuong);
                }

                Guid? idNhom = objNew.ID;

                #region DM_NhomDoiTuong
                DM_NhomDoiTuong DM_NhomDoiTuong = new DM_NhomDoiTuong();
                DM_NhomDoiTuong.ID = Guid.NewGuid();
                DM_NhomDoiTuong.MaNhomDoiTuong = sMaNhom;
                DM_NhomDoiTuong.TenNhomDoiTuong = objNew.TenNhomDoiTuong;
                DM_NhomDoiTuong.TenNhomDoiTuong_KhongDau = objNew.TenNhomDoiTuong_KhongDau;
                DM_NhomDoiTuong.TenNhomDoiTuong_KyTuDau = objNew.TenNhomDoiTuong_KyTuDau;
                DM_NhomDoiTuong.LoaiDoiTuong = objNew.LoaiDoiTuong;
                DM_NhomDoiTuong.GhiChu = objNew.GhiChu;
                DM_NhomDoiTuong.NgayTao = DateTime.Now;
                DM_NhomDoiTuong.NguoiTao = objNew.NguoiTao;
                DM_NhomDoiTuong.TrangThai = true; // true: Dang hoat dong/ false: Da xoa 

                #endregion
                string strIns = classNhomDoiTuong.Add_NhomDoiTuong(DM_NhomDoiTuong);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    return CreatedAtRoute("DefaultApi", new { id = DM_NhomDoiTuong.ID }, DM_NhomDoiTuong);
                }
            }
        }
        // DELETE: '/api/danhmuc/DM_NhomDoiTuongAPI/5';
        [ResponseType(typeof(string))]
        public string DeleteDM_NhomDoiTuong(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                var classNhomDoiTuong = new ClassDM_NhomDoiTuong(db);
                return classNhomDoiTuong.Delete_NhomDoiTuong(id);
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
