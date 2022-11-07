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

        [HttpGet, HttpPost]
        public IHttpActionResult GetListNhomHang_SetupHoTro(CommonParamSearch param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);
                    List<NhomHangHoa_KhoangApDungDTO> data = classHangHoa.GetListNhomHang_SetupHoTro(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult GetTongGiaTriSuDung_ofKhachHang(ParamNKyGDV param)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    classDMNhomHangHoa classNhomHangHoa = new classDMNhomHangHoa(db);
                    List<NhomHangHoa_TongSuDung> data = classNhomHangHoa.GetTongGiaTriSuDung_ofKhachHang(param);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GetListDonVi_byIDNhomHang(Guid idNhom)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var data = (from nhomdv in db.NhomHangHoa_DonVi
                                join dv in db.DM_DonVi on nhomdv.ID_DonVi equals dv.ID
                                where nhomdv.ID_NhomHangHoa == idNhom
                                select new
                                {
                                    nhomdv.ID_DonVi,
                                    nhomdv.ID_NhomHangHoa,
                                    dv.TenDonVi
                                }).ToList();
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }

        [HttpGet]
        public IHttpActionResult NhomHang_GetListKhoangApDung(Guid idNhom)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    var data = db.NhomHang_KhoangApDung.Where(x => x.Id_NhomHang == idNhom).Select(x =>
                    new
                    {
                        x.Id_NhomHang,
                        x.GiaTriSuDungTu,
                        x.GiaTriSuDungDen,
                        x.GiaTriHoTro,
                        x.KieuHoTro,
                    }).ToList();
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
            }
        }
        [HttpGet]
        public IHttpActionResult NhomHang_GetListSanPhamHoTro(Guid idNhom)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                try
                {
                    ClassDM_HangHoa classHangHoa = new ClassDM_HangHoa(db);
                    List<NhomHangHoa_SanPhamHoTroDTO> data = classHangHoa.NhomHang_GetListSanPhamHoTro(idNhom);
                    return ActionTrueData(data);
                }
                catch (Exception ex)
                {
                    return ActionFalseNotData(ex.InnerException + ex.Message);
                }
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
                temp.MaNhomHangHoa = dM_NhomHangHoa.MaNhomHangHoa;
                temp.TenNhomHangHoa = dM_NhomHangHoa.TenNhomHangHoa;
                temp.TenNhomHangHoaCha = dM_NhomHangHoa.ID_Parent == null ? "---Chọn nhóm---" : _classDMNHH.Get(p => p.ID == dM_NhomHangHoa.ID_Parent).TenNhomHangHoa;
                temp.ID_Parent = dM_NhomHangHoa.ID_Parent;
                temp.LaNhomHangHoa = dM_NhomHangHoa.LaNhomHangHoa;
                temp.TrangThai = dM_NhomHangHoa.TrangThai;
                temp.GhiChu = dM_NhomHangHoa.GhiChu;
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
                List<DM_NhomHangHoaSelect> lst = nhomHangHoa.getlistNhomHHByTenNhom(TenNhom).Select(p => new DM_NhomHangHoaSelect
                {
                    ID = p.ID,
                    TenNhomHangHoa = p.TenNhomHangHoa,
                    ID_Parent = p.ID_Parent,
                    LaNhomHangHoa = p.LaNhomHangHoa,
                    TrangThai = p.TrangThai
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
        [HttpGet, HttpPost, HttpPut]
        public IHttpActionResult PutDM_NhomHangHoa([FromBody] JObject data)
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

        [HttpGet, HttpPost]
        public IHttpActionResult PostNhomHang_DonVi(bool checkAll, Guid idNhomHang, List<NhomHangHoa_DonVi> lst)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        // delete && add again
                        var lstOld = db.NhomHangHoa_DonVi.Where(x => x.ID_NhomHangHoa == idNhomHang);
                        db.NhomHangHoa_DonVi.RemoveRange(lstOld);

                        if (checkAll)
                        {
                            var lstCN = db.DM_DonVi.Where(x => x.TrangThai != false).Select(x => x.ID).ToList();
                            foreach (var item in lstCN)
                            {
                                NhomHangHoa_DonVi obj = new NhomHangHoa_DonVi
                                {
                                    ID = Guid.NewGuid(),
                                    ID_NhomHangHoa = idNhomHang,
                                    ID_DonVi = item,
                                };
                                db.NhomHangHoa_DonVi.Add(obj);
                            }
                        }
                        else
                        {
                            foreach (var item in lst)
                            {
                                item.ID = Guid.NewGuid();
                                db.NhomHangHoa_DonVi.Add(item);
                            }
                        }
                        db.SaveChanges();
                        trans.Commit();
                        return ActionTrueData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }

        [HttpGet, HttpPost]
        public IHttpActionResult PostNhomHang_ChiTietApDung(Guid idNhomHang, [FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<NhomHang_KhoangApDung> lstAD = new List<NhomHang_KhoangApDung>();
                        List<NhomHang_ChiTietSanPhamHoTro> lstSP = new List<NhomHang_ChiTietSanPhamHoTro>();
                        if (data["lstKhoangApDung"] != null)
                        {
                            lstAD = data["lstKhoangApDung"].ToObject<List<NhomHang_KhoangApDung>>();
                        }
                        if (data["lstSPHoTro"] != null)
                        {
                            lstSP = data["lstSPHoTro"].ToObject<List<NhomHang_ChiTietSanPhamHoTro>>();
                        }

                        var apdungOld = db.NhomHang_KhoangApDung.Where(x => x.Id_NhomHang == idNhomHang);
                        db.NhomHang_KhoangApDung.RemoveRange(apdungOld);

                        var spOld = db.NhomHang_ChiTietSanPhamHoTro.Where(x => x.Id_NhomHang == idNhomHang && (x.LaSanPhamNgayThuoc == 1 || x.LaSanPhamNgayThuoc == 2));
                        db.NhomHang_ChiTietSanPhamHoTro.RemoveRange(spOld);

                        foreach (var item in lstAD)
                        {
                            item.Id = Guid.NewGuid();
                            db.NhomHang_KhoangApDung.Add(item);
                        }
                        foreach (var item in lstSP)
                        {
                            item.Id = Guid.NewGuid();
                            db.NhomHang_ChiTietSanPhamHoTro.Add(item);
                        }

                        db.SaveChanges();
                        trans.Commit();

                        return ActionTrueData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }
        [HttpGet, HttpPost]
        public IHttpActionResult AddListSanPham_toNhomHoTro(Guid idNhomHang, [FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<NhomHang_ChiTietSanPhamHoTro> lstSP = new List<NhomHang_ChiTietSanPhamHoTro>();
                        if (data["lstSPHoTro"] != null)
                        {
                            lstSP = data["lstSPHoTro"].ToObject<List<NhomHang_ChiTietSanPhamHoTro>>();
                        }

                        var spOld = db.NhomHang_ChiTietSanPhamHoTro.Where(x => x.Id_NhomHang == idNhomHang && x.LaSanPhamNgayThuoc == 2);
                        db.NhomHang_ChiTietSanPhamHoTro.RemoveRange(spOld);

                        foreach (var item in lstSP)
                        {
                            item.Id = Guid.NewGuid();
                            db.NhomHang_ChiTietSanPhamHoTro.Add(item);
                        }

                        db.SaveChanges();
                        trans.Commit();

                        return ActionTrueData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
            }
        }
        public IHttpActionResult MoveHangHoa_toNhomHoTro([FromBody] JObject data)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<Guid> arrID = new List<Guid>();
                        Guid idNhomHang = new Guid();
                        Guid idChiNhanh = new Guid();

                        if (data["idNhomHoTro"] != null)
                        {
                            idNhomHang = data["idNhomHoTro"].ToObject<Guid>();
                        }
                        if (data["idChiNhanh"] != null)
                        {
                            idChiNhanh = data["idChiNhanh"].ToObject<Guid>();
                        }
                        if (data["arrIDQuiDoi"] != null)
                        {
                            arrID = data["arrIDQuiDoi"].ToObject<List<Guid>>();
                        }

                        // get list sp exist by idChiNhanh
                        var arrEx = (from dt in db.NhomHang_ChiTietSanPhamHoTro
                                     join nhomDV in db.NhomHangHoa_DonVi on dt.Id_NhomHang equals nhomDV.ID_NhomHangHoa
                                     where arrID.Contains(dt.Id_DonViQuiDoi) && dt.LaSanPhamNgayThuoc == 2 && nhomDV.ID_DonVi == idChiNhanh
                                     select dt.Id).ToList();

                        // remove if exists
                        var xx = db.NhomHang_ChiTietSanPhamHoTro.Where(x => arrEx.Contains(x.Id)).AsEnumerable();
                        db.NhomHang_ChiTietSanPhamHoTro.RemoveRange(xx);

                        // add again
                        foreach (var item in arrID)
                        {
                            NhomHang_ChiTietSanPhamHoTro obj = new NhomHang_ChiTietSanPhamHoTro
                            {
                                Id = Guid.NewGuid(),
                                Id_NhomHang = idNhomHang,
                                Id_DonViQuiDoi = item,
                                LaSanPhamNgayThuoc = 2,
                            };
                            db.NhomHang_ChiTietSanPhamHoTro.Add(obj);
                        }

                        db.SaveChanges();
                        trans.Commit();

                        return ActionTrueData(string.Empty);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return ActionFalseNotData(ex.InnerException + ex.Message);
                    }
                }
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
        public string MaNhomHangHoa { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string TenNhomHangHoaCha { get; set; }
        public string GhiChu { get; set; }
        public Guid? ID_Parent { get; set; }
        public Guid? ID { get; set; }
        public bool LaNhomHangHoa { get; set; }
        public bool? TrangThai { get; set; }
    }
}
