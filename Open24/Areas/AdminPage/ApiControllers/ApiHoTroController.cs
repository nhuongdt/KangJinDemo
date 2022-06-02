using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;
using Open24.Areas.AdminPage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiHoTroController : ApiBaseController
    {
        private readonly HoTroService _HoTroService;
        public ApiHoTroController()
        {
            _HoTroService = new HoTroService();
        }


        /// <summary>
        ///  Tìm kiếm grid 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SearchGridHoTro(DataGridView model)
        {
            try
            {
                var data = _HoTroService.GetSearchHoiDap(model.Search);
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                }
                else
                {
                    data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit);
                }
                model.Data = data.Select(o => new
                {
                    o.CauHoi,
                    o.CauTraLoi,
                    o.GhiChu,
                    o.TrangThai,
                    o.NgayTao,
                    o.ViTri,
                    o.ID
                }).AsEnumerable();
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
            return Exeption();
        }

        /// <summary>
        ///  Tìm kiếm grid 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SearchGridNhomVaiTro(DataGridView model)
        {
            try
            {
                var data = _HoTroService.GetSearchNhomVaiTro(model.Search);
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                }
                else
                {
                    data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit);
                }
                model.Data = data.Select(o=>new {
                    o.ID,
                    o.Ten,
                    o.GhiChu,
                    o.Icon,
                    o.NgayTao,
                    o.ViTri,
                    o.TrangThai,
                    Listcheck=o.LH_NhomNganh_TinhNang.Select(c=>c.ID_TinhNang).ToList()
                }).AsEnumerable();
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
            return Exeption();
        }


        [HttpPost]
        public IHttpActionResult SearchGridTinhNang(DataGridView model)
        {
            try
            {
                var data = _HoTroService.GetSearchTinhNang(model.Search);
                model.PageCount = (int)Math.Ceiling((double)data.Count() / model.Limit);
                if (model.PageCount == 0 || model.PageCount == 1)
                {
                    model.PageCount = 1;
                    model.Page = 1;
                }
                else
                {
                    data = data.Skip(model.Limit * (model.Page - 1)).Take(model.Limit);
                }
                model.Data = data.Select(o => new {
                    o.ID,
                    o.Ten,
                    o.GhiChu,
                    o.Icon,
                    o.NgayTao,
                    o.TrangThai,
                    o.NoiDung,
                    o.ViTri,
                    o.Video,
                    o.ID_Cha
                }).AsEnumerable();
                model.PageItem = GridPagedingHellper.PageItems(model.Page, model.PageCount, data.Count());
                return RetunJsonAction(true, string.Empty, model);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
            return Exeption();
        }

        [HttpPost]
        public IHttpActionResult EditHoiDap(LH_HoiDap model)
        {
            try
            {
                if (model.ID ==0)
                {
                    _HoTroService.InsertHoiDap(model);
                    return InsertSuccess();
                }
                else
                {
                    if (_HoTroService.UpdateHoiDap(model))
                        return UpdateSuccess();
                    else
                        return ActionFalseNotData("Không tồn tại câu hỏi hoặc câu hỏi đã bị xóa");
                }
            }
            catch(Exception ex)
            {
                return Exeption(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult DeleteHoiDap(LH_HoiDap model)
        {
            try
            {
                    _HoTroService.DeleteHoiDap(model);
                    return DeleteSuccess();
               
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        /// <summary>
        /// upload ảnh 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UploadImages()
        {
            try
            {
                var path = "";
                string result = "";
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];
                    var filenameImage = Guid.NewGuid().ToString() + ".png";
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Content/images/hotroopen24")))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Content/images/hotroopen24"));
                    }

                    path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/images/hotroopen24"), filenameImage);

                    file.SaveAs(path);
                    result = "/Content/images/hotroopen24/" + filenameImage;
                }
                return ActionTrueNotData(result);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteNhomNganh(long id)
        {
            try
            {
                if (id == 0)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa.");
                }
                else
                {
                    _HoTroService.DeleteNhomNganh(id);
                }
                return DeleteSuccess();
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult DeleteTinhNang(long id)
        {
            try
            {
                if (id == 0)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần xóa.");
                }
                else
                {
                    _HoTroService.DeleteTinhNang(id);
                }
                return DeleteSuccess();
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult EditNhomNganh(NhomNganhModel model)
        {
            try
            {
                var result = new LH_NhomNganh
                {
                    ID = model.ID,
                    Ten = model.Ten,
                    GhiChu = model.GhiChu,
                    Icon = model.Icon,
                    TrangThai = model.TrangThai,
                    ViTri=model.ViTri
                };
                if (result.ID == 0)
                {
                    _HoTroService.InsertNhomNganh(result,model.ListTinhNang);
                    return InsertSuccess();
                }
                else
                {
                    if (_HoTroService.UpdateNHomNganh(result,model.ListTinhNang))
                        return UpdateSuccess();
                    else
                        return ActionFalseNotData("Không tồn tại nhóm ngành hoặc nhóm ngành đã bị xóa");
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult EditTinhNang(LH_TinhNang model)
        {
            try
            {
                if (model.ID == 0)
                {
                    _HoTroService.InsertTinhNang(model);
                    return InsertSuccess();
                }
                else
                {
                    if (_HoTroService.UpdateTinhNang(model))
                        return UpdateSuccess();
                    else
                        return ActionFalseNotData("Không tồn tại câu hỏi hoặc câu hỏi đã bị xóa");
                }
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }
        }


        [HttpGet]
        public IHttpActionResult GetDefaultTinhNang(long id)
        {
            try
            {
                if (id == 0)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần cập nhật.");
                }
                else
                {
                    var data = _HoTroService.GetAllTinhNang.Where(o => o.ID == id).AsEnumerable().Select(
                        o => new
                        {
                            o.ID,
                            o.ID_Cha,
                            o.Icon,
                            o.NoiDung,
                            o.Ten,
                            o.TrangThai,
                            o.Video,
                            o.ViTri,
                            o.GhiChu,
                            Textcha = getTextCha(o.ID_Cha)
                        }).FirstOrDefault();
                    return RetunJsonAction(true, string.Empty, data);
                }
            }

            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }
        public string getTextCha(long? id)
        {
            var first = _HoTroService.GetAllTinhNang.FirstOrDefault(o => o.ID == id);
            return first != null ? first.Ten : string.Empty;
        }

        /// <summary>
        /// Load dữ liệu tree
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Loadtree()
        {
            var data = _HoTroService.GetAllTinhNang.AsEnumerable();
            bool roleEdit = contant.CheckRole(StaticRole.HT_TINHNANG_UPDATE);
            bool roledelete = contant.CheckRole(StaticRole.HT_TINHNANG_DELETE);
            var json = data.Where(o => o.ID_Cha == null).OrderBy(o => o.ViTri).ThenByDescending(o => o.NgaySua).Select(o =>
                    new LH_TinhNangParentView
                    {
                        id = o.ID,
                        edit = roleEdit ? "<a href='#' class='blue'onclick='btnUpdate(" + o.ID + ")'><span class='glyphicon glyphicon-pencil' ></span> </a>" : string.Empty,
                        delete = roledelete ? "<a href='#' class='red' onclick='btnDelete(" + o.ID + ")'><span class='glyphicon glyphicon-trash' ></span></a>" : string.Empty,
                        text = o.Ten,
                        children = GetChildren(data, o.ID, roleEdit, roledelete,true)
                    });
            return Json(json);
        }

        private List<LH_TinhNangParentView> GetChildren(IEnumerable<LH_TinhNang> data, long roleKey, bool roleEdit, bool roledelete,bool load)
        {
            return data.Where(o => o.ID_Cha != null && o.ID_Cha.Equals(roleKey)).OrderBy(o => o.ViTri).ThenByDescending(o => o.NgaySua).Select(o =>
                       new LH_TinhNangParentView
                       {
                           id = o.ID,
                           edit = roleEdit ? "<a href='#' class='blue'onclick='btnUpdate(" + o.ID + ")'><span class='glyphicon glyphicon-pencil' ></span> </a>" : string.Empty,
                           delete = roledelete ? "<a href='#' class='red' onclick='btnDelete(" + o.ID + ")'><span class='glyphicon glyphicon-trash' ></span></a>" : string.Empty,
                           text = o.Ten,
                           children = load?GetChildren(data, o.ID, roleEdit, roledelete,false):new List<LH_TinhNangParentView>()
                       }).ToList();
        }


        [HttpGet]
        public IHttpActionResult GetViewHoTro()
        {
                try
                {
                    var listnhomnganh = _HoTroService.GetSearchNhomVaiTro(string.Empty).Where(o => o.TrangThai == true).OrderBy(o=>o.ViTri).ThenByDescending(o=>o.NgaySua).AsEnumerable().Select(
                        o => new
                        {
                            o.ID,
                            o.Ten,
                            o.Icon,
                            Title = StaticVariable.ConvetTitleToUrl(o.Ten)
                        }).AsEnumerable();
                    var listtinhnang = _HoTroService.GetAllTinhNang.Where(o => o.ID_Cha == null).Where(o => o.TrangThai == true).OrderBy(o => o.ViTri).ThenByDescending(o => o.NgaySua).AsEnumerable().Select(
                       o => new
                       {
                           o.ID,
                           o.Ten,
                           o.Icon,
                           Title = StaticVariable.ConvetTitleToUrl(o.Ten)
                       }).AsEnumerable();
                    return RetunJsonAction(true, string.Empty, new { NhomNganh = listnhomnganh, TinhNang = listtinhnang });
                }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }


        [HttpGet]
        public IHttpActionResult GetViewCauHoi(int page=1)
        {
            try
            {
                var cauhoi = _HoTroService.GetSearchHoiDap(string.Empty).Where(o=>o.TrangThai==true).OrderBy(o => o.ViTri).Skip((page-1)*8).Take(8).Select(
                    o => new
                    {
                        o.ID,
                        o.CauHoi,
                        o.CauTraLoi
                    }).AsEnumerable();
                return RetunJsonAction(true, string.Empty, cauhoi);
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        [HttpGet]
        public IHttpActionResult GetViewTinhNang(long group=0,long? tinhnang=0)
        {
            try
            {
                long ong = 0;
                long cha = 0;
                long con =0;
              
                var data = _HoTroService.GetJoinNhomNganhTinhNang(group).Where(o=>o.TrangThai==true).ToList();

                var list1 = _HoTroService.GetAllTinhNang.ToList();
                if (group != 0 && data.Any())
                {
                    if (tinhnang == 0) {
                        ong = data.OrderBy(o => o.ViTri).ThenByDescending(o => o.NgaySua).FirstOrDefault().ID;
                    }
                    else
                    {
                        getongchacon(tinhnang, ref ong, ref cha, ref con);
                    }
                }
                if (group == 0)
                {
                    getongchacon(tinhnang, ref ong, ref cha, ref con);
                }
                var json = data.AsEnumerable().OrderBy(o => o.ViTri).ThenByDescending(o => o.NgaySua).Select(o =>
                        new LH_TinhNangParentView
                        {
                            id = o.ID,
                            text = o.Ten,
                            noidung=o.NoiDung,
                            children =o.ID== ong? GetChildren1(list1.Where(c=>c.ID_Cha!=null).Where(c => c.TrangThai == true).AsEnumerable(), o.ID):new List<LH_TinhNangParentView>(),
                            video = o.Video,
                            IsCha=true,
                            Title = StaticVariable.ConvetTitleToUrl(o.Ten)
                        });
                var list = list1.Select(o => new { o.ID, o.Ten, o.NoiDung, o.Video }).ToList();
                return RetunJsonAction(true, string.Empty,new { json ,ong,con,cha, list });
            }
            catch (Exception ex)
            {
                return Exeption(ex);
            }

        }

        public void getongchacon(long? tinhnang,ref long ong,ref long cha,ref long con)
        {
            var result = _HoTroService.GetAllTinhNang.FirstOrDefault(o => o.ID == tinhnang);
            if (result != null)
            {
                if (result.ID_Cha == null)
                {
                    ong = result.ID;
                }
                else
                {
                    var result1 = _HoTroService.GetAllTinhNang.FirstOrDefault(o => o.ID == result.ID_Cha);
                    if (result1.ID_Cha == null)
                    {
                        ong = result1.ID;
                        cha = result.ID;
                    }
                    else
                    {
                        var result2 = _HoTroService.GetAllTinhNang.FirstOrDefault(o => o.ID == result1.ID_Cha);
                        if (result2.ID_Cha == null)
                        {
                            ong = result2.ID;
                            cha = result1.ID;
                            con = result.ID;
                        }

                    }
                }
            }
        }
        private List<LH_TinhNangParentView> GetChildren1(IEnumerable<LH_TinhNang> data, long roleKey,bool Isload=true)
        {
            return data.Where(o => o.ID_Cha != null && o.ID_Cha.Equals(roleKey)).OrderBy(o => o.ViTri).ThenByDescending(o => o.NgaySua).Select(o =>
                       new LH_TinhNangParentView
                       {
                           id = o.ID,
                           text = o.Ten,
                           noidung = o.NoiDung,
                           video = o.Video,
                           children = Isload==true?GetChildren1(data, o.ID,false) : new List<LH_TinhNangParentView>(),
                       }).ToList();
        }


    }
}
