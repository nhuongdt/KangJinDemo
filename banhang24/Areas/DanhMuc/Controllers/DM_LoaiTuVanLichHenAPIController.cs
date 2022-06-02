using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using libDM_LoaiTuVanLichHen;
using Newtonsoft.Json.Linq;

namespace banhang24.Areas.DanhMuc.Controllers
{
    [Route("api/DanhMuc/[Controller]")]
    public class DM_LoaiTuVanLichHenAPIController : ApiController
    {
        #region select

        public List<DM_LoaiTuVanLichHenSelect> GetDM_LoaiTuVan()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                List<DM_LoaiTuVanLichHen> lstDatas = classLoaiTVLichHen.Gets(n => n.TuVan_LichHen == 1);
                List<DM_LoaiTuVanLichHenSelect> list = new List<DM_LoaiTuVanLichHenSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_LoaiTuVanLichHenSelect temp = new DM_LoaiTuVanLichHenSelect();
                        temp.ID = item.ID;
                        temp.TenLoaiTuVanLichHen = item.TenLoaiTuVanLichHen;
                        list.Add(temp);
                    }
                }
                return list;
            }
        }
        //trinhpv
        public List<DM_LoaiTuVanLichHenSelect> GetDM_LoaiTuVanWhere(string TenLoaiTV)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                List<DM_LoaiTuVanLichHenSelect_Tpv> lstDatas = classLoaiTVLichHen.GetList_TVLH(TenLoaiTV);
                List<DM_LoaiTuVanLichHenSelect> list = new List<DM_LoaiTuVanLichHenSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_LoaiTuVanLichHenSelect temp = new DM_LoaiTuVanLichHenSelect();
                        temp.ID = item.ID;
                        temp.TenLoaiTuVanLichHen = item.TenLoaiTuVanLichHen;
                        list.Add(temp);
                    }
                }
                return list;
            }
        }
        public List<DM_LoaiTuVanLichHenSelect> GetDM_LoaiLichHen()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                List<DM_LoaiTuVanLichHen> lstDatas = classLoaiTVLichHen.Gets(n => n.TuVan_LichHen == 2);
                List<DM_LoaiTuVanLichHenSelect> list = new List<DM_LoaiTuVanLichHenSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_LoaiTuVanLichHenSelect temp = new DM_LoaiTuVanLichHenSelect();
                        temp.ID = item.ID;
                        temp.TenLoaiTuVanLichHen = item.TenLoaiTuVanLichHen;
                        list.Add(temp);
                    }
                }
                return list;
            }
        }

        public List<DM_LoaiTuVanLichHenSelect> GetDM_LoaiPhanHoi()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                List<DM_LoaiTuVanLichHen> lstDatas = classLoaiTVLichHen.Gets(n => n.TuVan_LichHen == 3);
                List<DM_LoaiTuVanLichHenSelect> list = new List<DM_LoaiTuVanLichHenSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_LoaiTuVanLichHenSelect temp = new DM_LoaiTuVanLichHenSelect();
                        temp.ID = item.ID;
                        temp.TenLoaiTuVanLichHen = item.TenLoaiTuVanLichHen;
                        list.Add(temp);
                    }
                }
                return list;
            }
        }

        [HttpGet]
        public IHttpActionResult GetDM_LoaiCongViec()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                    var list = classLoaiTVLichHen.Gets(n => n.TrangThai != 0).Select(x => new { x.ID, x.TenLoaiTuVanLichHen, x.TuVan_LichHen }).ToList();
                    return Json(new { res = true, data = list });
                }
                catch (Exception e)
                {
                    return Json(new { res = false, mes = e.InnerException + e.Message });
                }
            }
        }

        // GET: api/DM_LoaiTuVanLichHenAPI/5
        [ResponseType(typeof(DM_LoaiTuVanLichHen))]
        public IHttpActionResult GetDM_LoaiTuVan(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                DM_LoaiTuVanLichHen dM_LoaiTuVanLichHen = classLoaiTVLichHen.Select_LoaiTuVanLichHen(id);
                DM_LoaiTuVanLichHen temp = new DM_LoaiTuVanLichHen();
                temp.ID = dM_LoaiTuVanLichHen.ID;
                temp.TenLoaiTuVanLichHen = dM_LoaiTuVanLichHen.TenLoaiTuVanLichHen;
                if (dM_LoaiTuVanLichHen == null)
                {
                    return NotFound();
                }
                return Ok(temp);
            }
        }

        #endregion

        [ResponseType(typeof(DM_LoaiTuVanLichHen))]
        public IHttpActionResult GetLoaiTuVan(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                DM_LoaiTuVanLichHen loailichhen = classLoaiTVLichHen.Select_LoaiTuVanLichHen(id);
                DM_LoaiTuVanLichHen ct = new DM_LoaiTuVanLichHen();
                ct.ID = loailichhen.ID;
                ct.TenLoaiTuVanLichHen = loailichhen.TenLoaiTuVanLichHen;
                ct.TuVan_LichHen = loailichhen.TuVan_LichHen;
                ct.NgaySua = loailichhen.NgaySua;
                ct.NgayTao = loailichhen.NgayTao;
                ct.NguoiSua = loailichhen.NguoiSua;
                ct.NguoiTao = loailichhen.NguoiTao;
                if (loailichhen == null)
                {
                    return NotFound();
                }
                return Ok(ct);
            }
        }


        #region insert
        // POST: api/NS_NhanVienAPI
        //[HttpPost, ActionName("PostLoaiTuVan1")]
        //[ResponseType(typeof(DM_LoaiTuVanLichHen))]
        //public IHttpActionResult PostLoaiTuVan1(DM_LoaiTuVanLichHen DM_LoaiTuVanLichHen)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // return BadRequest(ModelState);
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
        //    }
        //    DM_LoaiTuVanLichHen.ID = Guid.NewGuid();

        //    string strIns = classLoaiTVLichHen.Add_TuVanLichHen(DM_LoaiTuVanLichHen);
        //    if (strIns != null && strIns != string.Empty)
        //        return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
        //    else
        //        return CreatedAtRoute("DefaultApi", new { id = DM_LoaiTuVanLichHen.ID }, DM_LoaiTuVanLichHen);
        //}

        [HttpPost, ActionName("PostLoaiTuVan")]
        [ResponseType(typeof(DM_LoaiTuVanLichHen))]
        public IHttpActionResult PostLoaiTuVan([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                DM_LoaiTuVanLichHen objLoaiTVLH = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                #region loai tu van lich hen
                DM_LoaiTuVanLichHen itemLoaiTuVanLichHen = new DM_LoaiTuVanLichHen();
                itemLoaiTuVanLichHen.ID = Guid.NewGuid();
                itemLoaiTuVanLichHen.TenLoaiTuVanLichHen = objLoaiTVLH.TenLoaiTuVanLichHen;
                itemLoaiTuVanLichHen.TuVan_LichHen = 1;//loai tu van
                itemLoaiTuVanLichHen.NguoiTao = nguoidung.TaiKhoan;
                itemLoaiTuVanLichHen.NgayTao = DateTime.Now;
                itemLoaiTuVanLichHen.NgaySua = DateTime.Now;
                #endregion
                string strIns = classLoaiTVLichHen.Add_TuVanLichHen(itemLoaiTuVanLichHen);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    DM_LoaiTuVanLichHenDTO objReturn = new DM_LoaiTuVanLichHenDTO
                    {
                        ID = itemLoaiTuVanLichHen.ID,
                        TenLoaiTuVanLichHen = itemLoaiTuVanLichHen.TenLoaiTuVanLichHen,
                        TuVan_LichHen = itemLoaiTuVanLichHen.TuVan_LichHen,
                        NguoiTao = itemLoaiTuVanLichHen.NguoiTao,
                        NgayTao = itemLoaiTuVanLichHen.NgayTao,
                        NgaySua = itemLoaiTuVanLichHen.NgaySua,
                        TrangThai = itemLoaiTuVanLichHen.TrangThai
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, ActionName("PostLoaiLichHen")]
        [ResponseType(typeof(DM_LoaiTuVanLichHen))]
        public IHttpActionResult PostLoaiLichHen([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                DM_LoaiTuVanLichHen objLoaiTVLH = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

                #region loai tu van lich hen
                DM_LoaiTuVanLichHen itemLoaiTuVanLichHen = new DM_LoaiTuVanLichHen();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                itemLoaiTuVanLichHen.ID = Guid.NewGuid();
                itemLoaiTuVanLichHen.TenLoaiTuVanLichHen = objLoaiTVLH.TenLoaiTuVanLichHen;
                itemLoaiTuVanLichHen.TuVan_LichHen = 2;//loai lich hen
                itemLoaiTuVanLichHen.NguoiTao = nguoidung.TaiKhoan;
                itemLoaiTuVanLichHen.NgayTao = DateTime.Now;
                itemLoaiTuVanLichHen.NgaySua = DateTime.Now;
                #endregion
                string strIns = classLoaiTVLichHen.Add_TuVanLichHen(itemLoaiTuVanLichHen);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    DM_LoaiTuVanLichHenDTO objReturn = new DM_LoaiTuVanLichHenDTO
                    {
                        ID = itemLoaiTuVanLichHen.ID,
                        TenLoaiTuVanLichHen = itemLoaiTuVanLichHen.TenLoaiTuVanLichHen,
                        TuVan_LichHen = itemLoaiTuVanLichHen.TuVan_LichHen,
                        NguoiTao = itemLoaiTuVanLichHen.NguoiTao,
                        NgayTao = itemLoaiTuVanLichHen.NgayTao,
                        NgaySua = itemLoaiTuVanLichHen.NgaySua,
                        TrangThai = itemLoaiTuVanLichHen.TrangThai
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, ActionName("PostLoaiPhanHoi")]
        [ResponseType(typeof(DM_LoaiTuVanLichHen))]
        public IHttpActionResult PostLoaiPhanHoi([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                DM_LoaiTuVanLichHen objLoaiTVLH = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

                #region loai tu van lich hen
                DM_LoaiTuVanLichHen itemLoaiTuVanLichHen = new DM_LoaiTuVanLichHen();
                string account = CookieStore.GetCookieAes("Account");
                HT_NguoiDung nguoidung = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<HT_NguoiDung>(account);
                itemLoaiTuVanLichHen.ID = Guid.NewGuid();
                itemLoaiTuVanLichHen.TenLoaiTuVanLichHen = objLoaiTVLH.TenLoaiTuVanLichHen;
                itemLoaiTuVanLichHen.TuVan_LichHen = 3;//loai phan hoi
                itemLoaiTuVanLichHen.NguoiTao = nguoidung.TaiKhoan;
                itemLoaiTuVanLichHen.NgayTao = DateTime.Now;
                itemLoaiTuVanLichHen.NgaySua = DateTime.Now;
                #endregion
                string strIns = classLoaiTVLichHen.Add_TuVanLichHen(itemLoaiTuVanLichHen);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    DM_LoaiTuVanLichHenDTO objReturn = new DM_LoaiTuVanLichHenDTO
                    {
                        ID = itemLoaiTuVanLichHen.ID,
                        TenLoaiTuVanLichHen = itemLoaiTuVanLichHen.TenLoaiTuVanLichHen,
                        TuVan_LichHen = itemLoaiTuVanLichHen.TuVan_LichHen,
                        NguoiTao = itemLoaiTuVanLichHen.NguoiTao,
                        NgayTao = itemLoaiTuVanLichHen.NgayTao,
                        NgaySua = itemLoaiTuVanLichHen.NgaySua,
                        TrangThai = itemLoaiTuVanLichHen.TrangThai
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, ActionName("PostLoaiCongViec")]
        [ResponseType(typeof(DM_LoaiTuVanLichHen))]
        public IHttpActionResult PostLoaiCongViec([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                DM_LoaiTuVanLichHen objLoaiTVLH = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

                #region loai tu van lich hen
                DM_LoaiTuVanLichHen itemLoaiTuVanLichHen = new DM_LoaiTuVanLichHen();
                itemLoaiTuVanLichHen.ID = Guid.NewGuid();
                itemLoaiTuVanLichHen.TenLoaiTuVanLichHen = objLoaiTVLH.TenLoaiTuVanLichHen;
                itemLoaiTuVanLichHen.TuVan_LichHen = objLoaiTVLH.TuVan_LichHen;
                itemLoaiTuVanLichHen.NguoiTao = objLoaiTVLH.NguoiTao;
                itemLoaiTuVanLichHen.NgayTao = DateTime.Now;
                itemLoaiTuVanLichHen.TrangThai = 1;
                #endregion
                string strIns = classLoaiTVLichHen.Add_TuVanLichHen(itemLoaiTuVanLichHen);
                if (strIns != null && strIns != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                {
                    DM_LoaiTuVanLichHenDTO objReturn = new DM_LoaiTuVanLichHenDTO
                    {
                        ID = itemLoaiTuVanLichHen.ID,
                        TenLoaiTuVanLichHen = itemLoaiTuVanLichHen.TenLoaiTuVanLichHen,
                        TuVan_LichHen = itemLoaiTuVanLichHen.TuVan_LichHen,
                        NguoiTao = itemLoaiTuVanLichHen.NguoiTao,
                        NgayTao = itemLoaiTuVanLichHen.NgayTao,
                        NgaySua = itemLoaiTuVanLichHen.NgaySua,
                        TrangThai = itemLoaiTuVanLichHen.TrangThai
                    };
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
                }
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult Post_LoaiTuVanLichHen([FromBody]JObject data)
        {
            try
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                    DM_LoaiTuVanLichHen objLoaiTVLH = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

                    DM_LoaiTuVanLichHen itemLoaiTuVanLichHen = new DM_LoaiTuVanLichHen();
                    itemLoaiTuVanLichHen.ID = Guid.NewGuid();
                    itemLoaiTuVanLichHen.TenLoaiTuVanLichHen = objLoaiTVLH.TenLoaiTuVanLichHen;
                    itemLoaiTuVanLichHen.TuVan_LichHen = objLoaiTVLH.TuVan_LichHen;
                    itemLoaiTuVanLichHen.NguoiTao = objLoaiTVLH.NguoiTao;
                    itemLoaiTuVanLichHen.NgayTao = DateTime.Now;
                    itemLoaiTuVanLichHen.NgaySua = DateTime.Now;
                    itemLoaiTuVanLichHen.TrangThai = 1;
                    classLoaiTVLichHen.Add_TuVanLichHen(itemLoaiTuVanLichHen);

                    DM_LoaiTuVanLichHenDTO objReturn = new DM_LoaiTuVanLichHenDTO
                    {
                        ID = itemLoaiTuVanLichHen.ID,
                        TenLoaiTuVanLichHen = itemLoaiTuVanLichHen.TenLoaiTuVanLichHen,
                        TuVan_LichHen = itemLoaiTuVanLichHen.TuVan_LichHen,
                        NguoiTao = itemLoaiTuVanLichHen.NguoiTao,
                        NgayTao = itemLoaiTuVanLichHen.NgayTao,
                        NgaySua = itemLoaiTuVanLichHen.NgaySua,
                        TrangThai = itemLoaiTuVanLichHen.TrangThai
                    };
                    return Json(new { res = true, data = objReturn });
                }
            }
            catch (Exception ex)
            {
                return Json(new { res = false, mes = ex.InnerException + ex.Message });
            }
        }

        #endregion


        #region update
        // PUT: api/DM_LoaiTuVanLichHenAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutLoaiTuVan(Guid id, DM_LoaiTuVanLichHen DM_LoaiTuVanLichHen)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Dữ liệu cập nhật chưa hợp lệ"));
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                string strUpd = classLoaiTVLichHen.Update_LoaiTuVan(DM_LoaiTuVanLichHen);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/DM_LoaiTuVanLichHenAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutLoaiTuVan([FromBody]JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            DM_LoaiTuVanLichHen DM_LoaiTuVanLichHen = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                string strUpd = classLoaiTVLichHen.Update_LoaiTuVan(DM_LoaiTuVanLichHen);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/DM_LoaiTuVanLichHenAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutLoaiLichHen([FromBody]JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            DM_LoaiTuVanLichHen DM_LoaiTuVanLichHen = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                string strUpd = classLoaiTVLichHen.Update_LoaiLichHen(DM_LoaiTuVanLichHen);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }

        }

        // PUT: api/DM_LoaiTuVanLichHenAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutLoaiPhanHoi([FromBody]JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            DM_LoaiTuVanLichHen DM_LoaiTuVanLichHen = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                string strUpd = classLoaiTVLichHen.Update_LoaiPhanHoi(DM_LoaiTuVanLichHen);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }

        }

        [ResponseType(typeof(string))]
        public IHttpActionResult PutLoaiCongViec([FromBody]JObject data)
        {
            Guid id = data["id"].ToObject<Guid>();
            DM_LoaiTuVanLichHen DM_LoaiTuVanLichHen = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                string strUpd = classLoaiTVLichHen.Update_LoaiCongViec(DM_LoaiTuVanLichHen);
                if (strUpd != null && strUpd != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPost, HttpGet]
        public IHttpActionResult Put_LoaiTuVanLichHen([FromBody]JObject data)
        {
            DM_LoaiTuVanLichHen objTuVan = data["objLoaiTVLH"].ToObject<DM_LoaiTuVanLichHen>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                string strUpd = classLoaiTVLichHen.Update_LoaiTuVanLichHen(objTuVan);
                if (strUpd != null && strUpd != string.Empty)
                    return Json(new { res = false, mes = strUpd });
                else
                    return Json(new { res = true, data = objTuVan });
            }
        }
        #endregion

        public bool Check_TenLoaiTuVanLichHenExist(string tenLoaiTuVan)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                return classLoaiTVLichHen.Check_TenLoaiTuVanLichHenExist(tenLoaiTuVan);
            }
        }


        // loại tư vấn
        [HttpDelete]
        [ResponseType(typeof(string))]
        public IHttpActionResult Delete_LoaiTuVan(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                class_DM_LoaiTuVanLichHen classLoaiTVLichHen = new class_DM_LoaiTuVanLichHen(db);
                string strDel = classLoaiTVLichHen.Delete_LoaiTuVan(id);
                if (strDel != null && strDel != string.Empty)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strDel));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        public class DM_LoaiTuVanLichHenSelect
        {
            public Guid ID { get; set; }
            public string TenLoaiTuVanLichHen { get; set; }
        }
    }
}
