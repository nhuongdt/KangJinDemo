using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
//using System.Web.Mvc;
using System.Net.Http;

using Model;
using libDM_NhomHangHoa;
using System.Data.SqlClient;
using libDM_HangHoa;

namespace banhang24.Areas.DanhMuc.Controllers
{
    public class DM_NhomHangHoaAPIController : BaseApiController
    {
        // GET: api/DM_NhomHangHoaAPI
        public List<DM_NhomHangHoaSelect> GetDM_NhomHangHoa()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<DM_NhomHangHoa> lstDatas = db.DM_NhomHangHoa.Where(p => p.TrangThai != true).ToList();
                List<DM_NhomHangHoaSelect> list = new List<DM_NhomHangHoaSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_NhomHangHoaSelect temp = new DM_NhomHangHoaSelect();
                        temp.ID = item.ID;
                        temp.TenNhomHangHoa = item.TenNhomHangHoa;
                        temp.ID_Parent = item.ID_Parent;
                        temp.NgayTao = item.NgayTao;
                        temp.LaNhomHangHoa = item.LaNhomHangHoa;
                        list.Add(temp);
                    }
                }
                return list.OrderByDescending(p => p.NgayTao).ToList();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetAllDMNhomHangHoa()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDMNhomHangHoa classDMNhomHangHoa = new classDMNhomHangHoa(db);
                List<DMNhomHangHoa> nhh = classDMNhomHangHoa.GetDMNhomHangHoas();
                var lstnhh = nhh.GenerateTree(p => p.ID, p => p.ParentId).ToList();
                return ActionTrueData(new
                {
                    data = lstnhh
                });
            }
        }

        public List<DM_NhomHangHoaSelect> GetDM_NhomHangHoaByLaNhomHH(bool lanhomhh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<DM_NhomHangHoa> lstDatas = db.DM_NhomHangHoa.Where(p => p.TrangThai != true && p.LaNhomHangHoa == lanhomhh).ToList();
                List<DM_NhomHangHoaSelect> list = new List<DM_NhomHangHoaSelect>();
                if (lstDatas != null)
                {
                    foreach (var item in lstDatas)
                    {
                        DM_NhomHangHoaSelect temp = new DM_NhomHangHoaSelect();
                        temp.ID = item.ID;
                        temp.TenNhomHangHoa = item.TenNhomHangHoa;
                        temp.ID_Parent = item.ID_Parent;
                        temp.NgayTao = item.NgayTao;
                        temp.LaNhomHangHoa = item.LaNhomHangHoa;
                        list.Add(temp);
                    }
                }
                return list.OrderByDescending(p => p.NgayTao).ToList();
            }
        }

        public List<DM_NhomHangHoaSelect> GetDM_HHParent()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                List<DM_NhomHangHoaSelect> lstDatas = _classDMNHH.GetAll();
                return lstDatas;
            }
        }

        public IHttpActionResult GetTree_NhomHangHoa()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                try
                {
                    var data = _classDMNHH.Gets(null)
                        .Select(x =>
                        new DM_NhomHangHoaSelect
                        {
                            ID = x.ID,
                            ID_Parent = x.ID_Parent,
                            TenNhomHangHoa = x.TenNhomHangHoa,
                            LaNhomHangHoa = x.LaNhomHangHoa,
                            TrangThai = x.TrangThai
                        }).ToList();
                    var json = data.Where(x => x.ID_Parent == null && x.TrangThai != true).Select(o =>
                             new NhomHangHoaParent
                             {
                                 id = o.ID,
                                 ID_Parent = o.ID_Parent,
                                 LaNhomHangHoa = o.LaNhomHangHoa,
                                 text = o.TenNhomHangHoa,
                                 children = GetChildren(data, o.ID)
                             });
                    return Json(new { res = true, data = json });
                }
                catch (Exception e)
                {
                    var err = string.Concat("GetTree_NhomHangHoa ", e.InnerException, e.Message);
                    CookieStore.WriteLog(err);
                    return Json(new { res = false, mes = err });
                }
            }
        }

        public List<NhomHangHoaParent> GetChildren(List<DM_NhomHangHoaSelect> data, Guid? idParent)
        {
            return data.Where(o => o.ID != null && o.ID_Parent.Equals(idParent) && o.TrangThai != true).Select(o =>
                         new NhomHangHoaParent
                         {
                             id = o.ID,
                             ID_Parent = o.ID_Parent,
                             text = o.TenNhomHangHoa,
                             children = GetChildren(data, o.ID)
                         }).ToList();
        }

        // GET: api/DM_NhomHangHoaAPI/5
        [ResponseType(typeof(DM_NhomHangHoaDTO))]
        public IHttpActionResult GetDM_NhomHangHoa(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                DM_NhomHangHoa dM_NhomHangHoa = db.DM_NhomHangHoa.Where(p => p.ID == id).FirstOrDefault();
                DM_NhomHangHoaDTO temp = new DM_NhomHangHoaDTO();
                temp.ID = dM_NhomHangHoa.ID;
                temp.TenNhomHangHoa = dM_NhomHangHoa.TenNhomHangHoa;
                temp.TenNhomHangHoaCha = dM_NhomHangHoa.ID_Parent == null ? "---Chọn nhóm---" : _classDMNHH.Get(p => p.ID == dM_NhomHangHoa.ID_Parent).TenNhomHangHoa;
                temp.ID_Parent = dM_NhomHangHoa.ID_Parent;
                temp.LaNhomHangHoa = dM_NhomHangHoa.LaNhomHangHoa;
                if (dM_NhomHangHoa == null)
                {
                    return NotFound();
                }
                return Ok(temp);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<DM_NhomHangHoaSelect> SeachDM_NhomHangHoa(string TenNhom)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa nhomHangHoa = new classDM_NhomHangHoa(db);
                //var _classDMHH = new ClassDM_HangHoa(db);
                List<DM_NhomHangHoaSelect> lst = nhomHangHoa.getlistNhomHHByTenNhom(TenNhom).Select(p => new DM_NhomHangHoaSelect
                {
                    ID = p.ID,
                    TenNhomHangHoa = p.TenNhomHangHoa,
                    ID_Parent = p.ID_Parent,
                    LaNhomHangHoa = p.LaNhomHangHoa
                }).ToList();
                if (lst != null)
                {
                    return lst;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool Check_TenNhomHangHoaExist(string tenNhomHang)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                return _classDMNHH.Check_TenNhomHangHoaExist(tenNhomHang);
            }
        }

        public bool Check_TenNhomHangHoaExistEdit(string tenNhomHang, Guid idnhomhh)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                return _classDMNHH.Check_TenNhomHangHoaExistEdit(tenNhomHang, idnhomhh);
            }
        }

        // PUT: api/DM_NhomHangHoaAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_NhomHangHoa(Guid id, DM_NhomHangHoa dM_NhomHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                if (!ModelState.IsValid)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
                }
                if (id != dM_NhomHangHoa.ID)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "ID nhóm hàng hóa cần cập nhật không trùng khớp với ID dữ liệu"));
                }
                string strUpd = _classDMNHH.Update_NhomHangHoa(dM_NhomHangHoa);
                if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // PUT: api/DM_NhomHangHoaAPI/5
        [ResponseType(typeof(string))]
        public IHttpActionResult PutDM_NhomHangHoa([FromBody]JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                Guid id = data["id"].ToObject<Guid>();
                DM_NhomHangHoa dM_NhomHangHoa = data["dM_NhomHangHoa"].ToObject<DM_NhomHangHoa>();
                if (id != dM_NhomHangHoa.ID)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "ID nhóm hàng hóa cần cập nhật không trùng khớp với ID dữ liệu"));
                }
                dM_NhomHangHoa.HienThi_BanThe = true;
                dM_NhomHangHoa.HienThi_Chinh = true;
                dM_NhomHangHoa.HienThi_Phu = true;
                dM_NhomHangHoa.TenNhomHangHoa_KhongDau = CommonStatic.ConvertToUnSign(dM_NhomHangHoa.TenNhomHangHoa).ToLower();
                dM_NhomHangHoa.TenNhomHangHoa_KyTuDau = CommonStatic.GetCharsStart(dM_NhomHangHoa.TenNhomHangHoa).ToLower();

                //dM_NhomHangHoa.NguoiSua = "admin";
                dM_NhomHangHoa.NgaySua = DateTime.Now;

                string strUpd = _classDMNHH.Update_NhomHangHoa(dM_NhomHangHoa);
                if (strUpd != null && strUpd != string.Empty && strUpd.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strUpd));
                else
                    return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // POST: api/DM_NhomHangHoaAPI
        [ResponseType(typeof(DM_NhomHangHoa))]
        [HttpPost, HttpGet]
        public IHttpActionResult PostDM_NhomHangHoa(DM_NhomHangHoa dM_NhomHangHoa)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                dM_NhomHangHoa.ID = Guid.NewGuid();
                dM_NhomHangHoa.MaNhomHangHoa = DateTime.Now.ToString("yyyyMMddHHmmss");
                dM_NhomHangHoa.HienThi_BanThe = true;
                dM_NhomHangHoa.HienThi_Chinh = true;
                dM_NhomHangHoa.HienThi_Phu = true;
                dM_NhomHangHoa.NgayTao = DateTime.Now;
                dM_NhomHangHoa.TenNhomHangHoa_KhongDau = CommonStatic.ConvertToUnSign(dM_NhomHangHoa.TenNhomHangHoa).ToLower();
                dM_NhomHangHoa.TenNhomHangHoa_KyTuDau = CommonStatic.GetCharsStart(dM_NhomHangHoa.TenNhomHangHoa).ToLower();
                string strIns = _classDMNHH.Add_NhomHangHoa(dM_NhomHangHoa);

                DM_NhomHangHoaDTO objReturn = new DM_NhomHangHoaDTO
                {
                    ID = dM_NhomHangHoa.ID,
                    TenNhomHangHoa = dM_NhomHangHoa.TenNhomHangHoa,
                    ID_Parent = dM_NhomHangHoa.ID_Parent
                };

                if (strIns != null && strIns != string.Empty && strIns.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, strIns));
                else
                    return CreatedAtRoute("DefaultApi", new { id = objReturn.ID }, objReturn);
            }
        }

        // DELETE: api/DM_NhomHangHoaAPI/5
        [ResponseType(typeof(string))]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteDM_NhomHangHoa(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                string strDel = _classDMNHH.Delete_NhomHangHoa(id);
                if (strDel != null && strDel != string.Empty && strDel.Trim() != "")
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Dữ liệu cập nhật chưa hợp lệ"));
                else
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
            }
        }

        [System.Web.Http.HttpDelete]
        [ResponseType(typeof(string))]
        public string DeleteDM_NhomHangHoa1(Guid id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classDM_NhomHangHoa _classDMNHH = new classDM_NhomHangHoa(db);
                ClassDM_HangHoa _classDMHH = new ClassDM_HangHoa(db);
                if (db == null)
                {
                    return "Chưa kết nối DB";
                }
                else
                {
                    DM_NhomHangHoa nhh = db.DM_NhomHangHoa.Where(p => p.ID == id).FirstOrDefault();
                    List<DM_NhomHangHoa> lst = db.DM_NhomHangHoa.Where(p => p.ID_Parent == id).ToList();
                    if (nhh != null)
                    {
                        nhh.TrangThai = true;
                        _classDMNHH.Update_NhomHangHoa(nhh);
                        _classDMHH.UpdateHHKhiXoaNhomHH(id, nhh.LaNhomHangHoa);
                        if (lst != null)
                        {
                            foreach (var item in lst)
                            {
                                item.ID_Parent = null;
                                _classDMNHH.Update_NhomHangHoa(item);
                            }
                        }
                        return "";
                    }
                    else
                    {
                        return "Lỗi";
                    }
                }
            }
        }

        [System.Web.Http.HttpGet]
        public string UpdateHangHoaByIDNhom(Guid id_nhom)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("ID_NhomHangHoa", id_nhom));
                db.Database.ExecuteSqlCommand("exec UpdateHangHoaByID_NhomHangHoa @ID_NhomHangHoa", paramlist.ToArray());
                return "";
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

    internal class DM_NhomHangHoaDTO
    {
        public string TenNhomHangHoa { get; set; }
        public string TenNhomHangHoaCha { get; set; }
        public string GhiChu { get; set; }
        public Guid? ID_Parent { get; set; }
        public Guid? ID { get; set; }
        public bool LaNhomHangHoa { get; set; }
    }

    //public class DM_NhomHangHoaSelect
    //{
    //    public Guid ID { get; set; }
    //    public Guid? ID_Parent { get; set; }
    //    public string TenNhomCha { get; set; }
    //    public string TenNhomHangHoa { get; set; }
    //}
}
